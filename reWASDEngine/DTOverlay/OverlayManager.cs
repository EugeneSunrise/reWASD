using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoftReWASDServiceNamespace;
using Microsoft.Win32;
using Overlay.NET.Wpf;
using Prism.Ioc;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using reWASDEngine;
using reWASDEngine.OverlayAPI.RemapWindow;
using reWASDEngine.Services.OverlayAPI;
using Unity;
using XBEliteWPF.DataModels;
using XBEliteWPF.DataModels.GamepadActiveProfiles;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.Controller;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Services;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils.Extensions;

namespace DTOverlay
{
	public class OverlayManager
	{
		private OverlayTracker overlayTracker
		{
			get
			{
				return this._overlayTracker;
			}
			set
			{
				this._overlayTracker = value;
				this.overlayMenuE.overlayTracker = value;
			}
		}

		private GamepadWindowVM _gwvm
		{
			get
			{
				return OverlayManager.gamepadWindow.ViewModel;
			}
		}

		private MessagesVM _mvm
		{
			get
			{
				if (this.messagesWindow == null)
				{
					return null;
				}
				return this.messagesWindow.ViewModel;
			}
		}

		private RemapWindowVM remapWindowVM
		{
			get
			{
				return this.remapWindow.ViewModel;
			}
		}

		public OverlayManager()
		{
			EventProcessor eventProcessor = (EventProcessor)IContainerProviderExtensions.Resolve<IEventProcessor>(Engine.SContainer);
			eventProcessor.OverlayGamepadHotkey += this.OverlayGamepadHotkey;
			eventProcessor.OverlayMappingHotkey += this.OverlayMappingHotkey;
			Engine.EventsProxy.OffileDevice += this.OffileDevice;
			Engine.EventsProxy.ShiftShowUI += this.ShiftShowUI;
			Engine.EventsProxy.ShiftHideOverlayUI += this.ShiftHideUI;
			Engine.EventsProxy.OnSlotChangedUI += this.OverlaySlotChange;
			Engine.EventsProxy.OnControllerRemovedUI += this.OnControllerRemoved;
			Engine.EventsProxy.OnRemapOffUI += this.OnRemapOffUI;
			Engine.EventsProxy.OnConfigAppliedToSlotUI += this.OnConfigAppliedToSlot;
			Engine.EventsProxy.OnBatteryLevelChangedUI += new BatteryLevelChangedHandler(this.SetBatteryNotification);
			Engine.GamepadService.SetBatteryNotificationEvent += this.SetBatteryNotification;
			SystemEvents.DisplaySettingsChanged += this.SystemEvents_DisplaySettingsChanged;
			this.StartPollerMessages();
			this.CheckAndCreateDirectoryForOverlayData();
			IContainerProviderExtensions.Resolve<IForegroundApplicationMonitorService>(Engine.SContainer).Changed += this.ForegroundApplicationMonitorServiceOnChanged;
			this.LoadBlackList();
		}

		~OverlayManager()
		{
			this.StopKeyThread();
			this.StopPollerMessages();
			this.overlayTracker = null;
		}

		private void StartPollerMessages()
		{
			this._pollingTimerMessages = new DispatcherTimer();
			this._pollingTimerMessages.Tick += delegate([Nullable(2)] object o, EventArgs e)
			{
				this.OnTickTimerMessages();
			};
			this._pollingTimerMessages.Interval = new TimeSpan(0, 0, 0, 0, 100);
			this._pollingTimerMessages.Start();
		}

		private void StopPollerMessages()
		{
			DispatcherTimer pollingTimerMessages = this._pollingTimerMessages;
			if (pollingTimerMessages != null)
			{
				pollingTimerMessages.Stop();
			}
			this._pollingTimerMessages = null;
		}

		private void OnRemapOffUI(string ID, string deviceName)
		{
			if (this.remapWindow != null && OverlayUtils.IsContainsInLongID(this.remapWindow.ViewModel.ID, ID))
			{
				this.HideRemap();
			}
			if (OverlayManager.gamepadWindow != null && OverlayUtils.IsContainsInLongID(OverlayManager.gamepadWindow.ViewModel.ID, ID))
			{
				this.HideGamepad();
			}
			if (Engine.UserSettingsService.IsOverlayEnable && this.showMessagesSettings != 0 && this.showRemapIsONOFF != 0)
			{
				this.ShowMessages();
				MessagesVM mvm = this._mvm;
				if (mvm != null)
				{
					mvm.ShowRemapChange(ID, deviceName, this.IsReplace(), this.showShortMessagesSettings == 1, OverlayUtils.ConvertToWindowsAligment(this.AlignNotification));
				}
				this.SavePictureMessages();
			}
		}

		private void OnTickTimerMessages()
		{
			if (this.messagesWindow != null)
			{
				MessagesVM mvm = this._mvm;
				if (mvm != null)
				{
					mvm.RemoveOld(this.timeMessageSec);
				}
				MessagesVM mvm2 = this._mvm;
				if (mvm2 != null && mvm2.Count() == 0)
				{
					this.HideMessages();
				}
			}
			if (Engine.UserSettingsService.ShowDirectXOverlay && this.timeCloseMessagesWindow != null && (DateTime.Now - this.timeCloseMessagesWindow.Value).TotalSeconds > 1.0)
			{
				this.timeCloseMessagesWindow = null;
				this.DeleteTmpFiles("MessagesWindow", this.idCloseMessage);
			}
		}

		private void DeleteTmpFiles(string prefix, int ID)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Disc-Soft", "reWASD");
			text += "\\OverlayData";
			try
			{
				foreach (string text2 in Directory.EnumerateFiles(text, prefix + "*.png", SearchOption.TopDirectoryOnly))
				{
					if (int.Parse(Regex.Match(text2, "\\d+").Value) < ID)
					{
						File.Delete(text2);
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private void OnTick_GamepadPoller()
		{
			bool flag = true;
			if (this.showGamepadSettings != 0 && this.timeFinishLastKeyPress != null && (DateTime.Now - this.timeFinishLastKeyPress.Value).TotalSeconds < 5.0)
			{
				flag = false;
			}
			if (flag && this.autohideGamepad != 0)
			{
				this.HideGamepad();
			}
		}

		public void OverlayKeyPressed(string ID, GamepadState gamepadState, ControllerTypeEnum controllerType)
		{
			if (!Engine.UserSettingsService.IsOverlayEnable || this.showGamepadSettings == 0 || OverlayManager.gamepadWindow == null)
			{
				return;
			}
			this.timeFinishLastKeyPress = new DateTime?(DateTime.Now);
			Action <>9__1;
			ThreadHelper.ExecuteInMainDispatcher(delegate
			{
				this.ShowGamepad(ID, new ControllerTypeEnum?(controllerType));
				this._gwvm.IsGamepadVisible = this.showGamepadSettings != 0;
				this._gwvm.IsTableVisible = this.showControllerOnly == 0;
				GamepadWindowVM gwvm = this._gwvm;
				if (gwvm != null)
				{
					gwvm.OverlayKeyPressed(gamepadState, controllerType, (float)this.PollingRate);
				}
				if (Engine.UserSettingsService.IsOverlayEnable && Engine.UserSettingsService.ShowDirectXOverlay && this.showGamepadSettings != 0 && this.overlayTracker != null && this.overlayTracker.IsDirectXAttached())
				{
					Dispatcher dispatcher = Application.Current.Dispatcher;
					Action action;
					if ((action = <>9__1) == null)
					{
						action = (<>9__1 = delegate
						{
							this.DoSavePictureGamepadWindow();
						});
					}
					dispatcher.BeginInvoke(action, DispatcherPriority.ContextIdle, null);
				}
			}, true);
		}

		private void ShowMessages()
		{
			Tracer.TraceWrite("OverlayManager ShowMessages", false);
			if (!this.showedMessages)
			{
				this.messagesWindow = new MessagesWindow();
				this.messagesWindow.overlayManager = this;
				this.messagesWindow.Align = this.AlignNotification;
				MessagesVM mvm = this._mvm;
				if (mvm != null)
				{
					mvm.SetTransparent(this.TransparentMessages);
				}
				MessagesVM mvm2 = this._mvm;
				if (mvm2 != null)
				{
					mvm2.SetScale(Engine.UserSettingsService.MessagesWidowScale);
				}
				this.messagesWindow.Show();
				this.showedMessages = true;
			}
		}

		private void ShowRemap(string ID, ConfigData configData, bool isDescriptions)
		{
			Tracer.TraceWrite("OverlayManager ShowRemap", false);
			object lockGamepadWindow = OverlayManager.LockGamepadWindow;
			lock (lockGamepadWindow)
			{
				this.HideRemap();
				if (this.remapWindow == null)
				{
					Application.Current.Dispatcher.Invoke(delegate
					{
						this.remapWindow = RemapWindowFactory.CreateWindow(CreationRemapStyle.NormalCreation, ID, configData, isDescriptions, this.AlignMappings, this.TransparentMappings);
						this.remapWindow.OnWindowSizeChanged += delegate
						{
							OverlayUtils.SetAlign(this.MonitorMappings, this.AlignMappings, 0f, this.remapWindow);
							this.ArrayMessageWindow();
						};
						this.remapWindow.Show();
					});
				}
			}
		}

		private void HideRemap()
		{
			object lockGamepadWindow = OverlayManager.LockGamepadWindow;
			lock (lockGamepadWindow)
			{
				Tracer.TraceWrite("OverlayManager HideRemap", false);
				if (this.remapWindow != null)
				{
					OverlayTracker overlayTracker = this.overlayTracker;
					if (overlayTracker != null)
					{
						overlayTracker.HideOverlayRemap();
					}
					Application.Current.Dispatcher.Invoke(delegate
					{
						RemapWindow remapWindow = this.remapWindow;
						if (remapWindow != null)
						{
							remapWindow.Hide();
						}
						RemapWindow remapWindow2 = this.remapWindow;
						if (remapWindow2 != null)
						{
							remapWindow2.Close();
						}
						this.remapWindow = null;
					});
				}
				this.ArrayMessageWindow();
			}
		}

		public void NextPageOrHideRemap()
		{
			bool shouldHide = true;
			object lockGamepadWindow = OverlayManager.LockGamepadWindow;
			lock (lockGamepadWindow)
			{
				Tracer.TraceWrite("OverlayManager NextPageOrHideRemap", false);
				Application.Current.Dispatcher.Invoke(delegate
				{
					if (this.remapWindowVM.ShouldShowNextItems())
					{
						this.remapWindowVM.ShowNextVisibleItems();
						this.remapWindow.IsUpdating = true;
						shouldHide = false;
					}
				});
			}
			if (shouldHide)
			{
				this.HideRemap();
			}
		}

		private void RecreateGamepad()
		{
			if (OverlayManager.gamepadWindow != null)
			{
				string id = this._gwvm.ID;
				ControllerTypeEnum? controllerType = this._gwvm.ControllerType;
				this.HideGamepad();
				this.ShowGamepad(id, controllerType);
			}
		}

		private void RecreateMappings()
		{
			if (this.remapWindow != null)
			{
				string id = this.remapWindowVM.ID;
				ConfigData configData = this.remapWindowVM.configData;
				bool isDescriptionWindow = this.remapWindow.IsDescriptionWindow;
				this.HideRemap();
				this.ShowRemap(id, configData, isDescriptionWindow);
			}
		}

		private bool IsReplace()
		{
			bool flag = false;
			Rectangle desktopWorkingArea = OverlayUtils.GetDesktopWorkingArea(this.MonitorMessages);
			if (OverlayManager.currentHeight + 165 > desktopWorkingArea.Height)
			{
				flag = true;
			}
			return flag;
		}

		private void OnBatteryState(bool low, string deviceName, ControllerTypeEnum firstControllerType, bool sBatteryLevelPercentPresent, byte BatteryPercents)
		{
			Tracer.TraceWrite("OverlayManager OnBatteryState Call", false);
			if (!Engine.UserSettingsService.IsOverlayEnable || this.showMessagesSettings == 0 || ((this.showBatteryIsLow == 0) & low) || (this.showBatteryIsCritical == 0 && !low))
			{
				return;
			}
			Tracer.TraceWrite("OverlayManager OnBatteryState", false);
			ThreadHelper.ExecuteInMainDispatcher(delegate
			{
				this.ShowMessages();
				MessagesVM mvm = this._mvm;
				if (mvm != null)
				{
					mvm.ShowBatteryLevel(deviceName, low, this.IsReplace(), this.showShortMessagesSettings == 1, OverlayUtils.ConvertToWindowsAligment(this.AlignNotification), firstControllerType, sBatteryLevelPercentPresent, BatteryPercents);
				}
				this.SavePictureMessages();
			}, true);
		}

		private void ShiftHideUI(string ID)
		{
			Tracer.TraceWrite("OverlayManager ShiftcHideUI", false);
			if (this.overlayMenuE.IsShowed)
			{
				this.overlayMenuE.HideMenu();
				return;
			}
			ThreadHelper.ExecuteInMainDispatcher(delegate
			{
				MessagesVM mvm = this._mvm;
				if (mvm == null)
				{
					return;
				}
				mvm.HideShift(ID);
			}, true);
		}

		private void ShiftShowUI(BaseControllerVM controller, GamepadProfile gamepadProfile, ShiftInfo shift, bool toggle, bool alwaysShow)
		{
			Tracer.TraceWrite(string.Concat(new string[]
			{
				"OverlayManager ShiftShowUI shift:",
				shift.Shift.ToString(),
				" gamepadProfile:",
				(gamepadProfile != null) ? gamepadProfile.ToString() : null,
				" toggle:",
				toggle.ToString(),
				" alwaysShow:",
				alwaysShow.ToString()
			}), false);
			if (this.overlayMenuE.IsShowed)
			{
				this.overlayMenuE.HideMenu();
				return;
			}
			string gameName = "";
			string profileName = "";
			if (gamepadProfile != null)
			{
				gameName = gamepadProfile.GameName;
				profileName = gamepadProfile.ProfileName;
			}
			bool flag = false;
			IGameProfilesService gameProfilesService = Engine.GameProfilesService;
			Game game = ((gameProfilesService != null) ? gameProfilesService.GamesCollection.FirstOrDefault((Game g) => g.Name.Equals(gameName)) : null);
			if (game != null)
			{
				Config config = game.ConfigCollection.FirstOrDefault((Config c) => c.Name.Equals(profileName));
				if (config == null)
				{
					return;
				}
				config.ReadConfigFromJsonIfNotLoaded(false);
				ConfigData configData = config.ConfigData;
				if (configData != null)
				{
					configData.GetOverlayBaseXbBindingCollection();
				}
				ConfigData configData2 = config.ConfigData;
				flag = configData2 != null && configData2.IsOverlayShift(shift);
			}
			if (Engine.UserSettingsService.IsOverlayEnable && flag)
			{
				if (this.overlayMenuE.IsShowed)
				{
					this.overlayMenuE.HideMenu();
					return;
				}
				this.overlayMenuE.ShowMenu(controller, gamepadProfile, controller.ID, toggle);
				return;
			}
			else
			{
				if (!Engine.UserSettingsService.IsOverlayEnable || this.showMessagesSettings == 0 || (this.showShiftIsChanged == 0 && this.showShiftIsChangedToggle == 0))
				{
					return;
				}
				Tracer.TraceWrite("OverlayManager ShiftShowUI", false);
				string ID = controller.ID;
				string controllerDisplayName = controller.ControllerDisplayName;
				ThreadHelper.ExecuteInMainDispatcher(delegate
				{
					this.ShowMessages();
					MessagesVM mvm = this._mvm;
					if (mvm != null)
					{
						mvm.ShowShift(ID, controllerDisplayName, shift, this.IsReplace(), this.showShortMessagesSettings == 1, OverlayUtils.ConvertToWindowsAligment(this.AlignNotification), toggle, alwaysShow);
					}
					this.SavePictureMessages();
				}, true);
				return;
			}
		}

		private void SavePictureRemapWindow()
		{
			if (Engine.UserSettingsService.IsOverlayEnable && Engine.UserSettingsService.ShowDirectXOverlay && this.showMappings != 0 && this.overlayTracker != null && this.overlayTracker.IsDirectXAttached())
			{
				Application.Current.Dispatcher.BeginInvoke(new Action(delegate
				{
					this.DoSavePictureRemapWindow();
				}), DispatcherPriority.ContextIdle, null);
			}
		}

		private void SavePictureGamepadWindow()
		{
			if (Engine.UserSettingsService.IsOverlayEnable && Engine.UserSettingsService.ShowDirectXOverlay && this.showGamepadSettings != 0 && this.overlayTracker != null && this.overlayTracker.IsDirectXAttached())
			{
				Application.Current.Dispatcher.BeginInvoke(new Action(delegate
				{
					this.DoSavePictureGamepadWindow();
				}), DispatcherPriority.ContextIdle, null);
			}
		}

		private void SavePictureMessages()
		{
			if (Engine.UserSettingsService.IsOverlayEnable && Engine.UserSettingsService.ShowDirectXOverlay && this.showMessagesSettings != 0 && this.overlayTracker != null && this.overlayTracker.IsDirectXAttached())
			{
				Application.Current.Dispatcher.BeginInvoke(new Action(delegate
				{
					this.DoSavePictureMessages();
				}), DispatcherPriority.ContextIdle, null);
			}
		}

		private void DoSavePictureRemapWindow()
		{
			if (this.remapWindow != null)
			{
				MemoryStream memoryStream = this.remapWindow.CopyAsBitmap().Encode(new PngBitmapEncoder());
				string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Disc-Soft", "reWASD");
				OverlayUtils.ClearOldPng("RemapWindow", OverlayManager.remapFileID);
				string text2 = text + "\\OverlayData\\RemapWindow" + OverlayManager.remapFileID.ToString() + ".png";
				try
				{
					using (FileStream fileStream = new FileStream(text2, FileMode.Create, FileAccess.Write))
					{
						memoryStream.WriteTo(fileStream);
					}
				}
				catch (Exception)
				{
					Tracer.TraceWrite("OverlayManager DoSavePictureMessages. Error write file. " + text2, false);
				}
				this.RemapRendered(OverlayManager.remapFileID);
				OverlayManager.remapFileID++;
				if (OverlayManager.remapFileID > 250)
				{
					OverlayManager.remapFileID = 0;
				}
			}
		}

		private void DoSavePictureGamepadWindow()
		{
			if (OverlayManager.gamepadWindow != null)
			{
				MemoryStream memoryStream = OverlayManager.gamepadWindow.CopyAsBitmap().Encode(new PngBitmapEncoder());
				string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Disc-Soft", "reWASD");
				OverlayUtils.ClearOldPng("GamepadWindow", OverlayManager.gamepadFileID);
				string text2 = text + "\\OverlayData\\GamepadWindow" + OverlayManager.gamepadFileID.ToString() + ".png";
				try
				{
					using (FileStream fileStream = new FileStream(text2, FileMode.Create, FileAccess.Write))
					{
						memoryStream.WriteTo(fileStream);
					}
				}
				catch (Exception)
				{
					Tracer.TraceWrite("OverlayManager DoSavePictureMessages. Error write file. " + text2, false);
				}
				this.GamepadRendered(OverlayManager.gamepadFileID);
				OverlayManager.gamepadFileID++;
				if (OverlayManager.gamepadFileID > 250)
				{
					OverlayManager.gamepadFileID = 0;
				}
			}
		}

		private void DoSavePictureMessages()
		{
			if (this.messagesWindow != null)
			{
				MemoryStream memoryStream = this.messagesWindow.CopyAsBitmap().Encode(new PngBitmapEncoder());
				string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Disc-Soft", "reWASD");
				OverlayUtils.ClearOldPng("MessagesWindow", OverlayManager.messagesFileID);
				string text2 = text + "\\OverlayData\\MessagesWindow" + OverlayManager.messagesFileID.ToString() + ".png";
				try
				{
					using (FileStream fileStream = new FileStream(text2, FileMode.Create, FileAccess.Write))
					{
						memoryStream.WriteTo(fileStream);
					}
				}
				catch (Exception)
				{
					Tracer.TraceWrite("OverlayManager DoSavePictureMessages. Error write file. " + text2, false);
				}
				this.MessageRendered(OverlayManager.messagesFileID);
				OverlayManager.messagesFileID++;
				if (OverlayManager.messagesFileID > 250)
				{
					OverlayManager.messagesFileID = 0;
				}
			}
		}

		private string CheckAndCreateDirectoryForOverlayData()
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Disc-Soft", "reWASD");
			text += "\\OverlayData";
			try
			{
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
			}
			catch (Exception)
			{
				Tracer.TraceWrite("OverlayManager CheckAndCreateDirectoryForOverlayData failed. Overlay in DirectX wil be not available.", false);
			}
			return text;
		}

		private void StartPollerGamepad()
		{
			this.pollingTimerGamepad = new DispatcherTimer();
			this.pollingTimerGamepad.Tick += delegate([Nullable(2)] object o, EventArgs e)
			{
				this.OnTick_GamepadPoller();
			};
			this.pollingTimerGamepad.Interval = new TimeSpan(0, 0, 0, 0, 500);
			this.pollingTimerGamepad.Start();
		}

		private void StopPollerGamepad()
		{
			DispatcherTimer dispatcherTimer = this.pollingTimerGamepad;
			if (dispatcherTimer != null)
			{
				dispatcherTimer.Stop();
			}
			this.pollingTimerGamepad = null;
		}

		private void CreateNewThread()
		{
			Tracer.TraceWrite("OverlayManager CreateNewThread", false);
			this.keyThread = new Thread(async delegate
			{
				while (!this.abortKeyThread)
				{
					int num = 50;
					object obj = OverlayManager.LockPollingRate;
					lock (obj)
					{
						num = this.PollingRate;
					}
					try
					{
					}
					catch (Exception ex)
					{
						if (ex is TaskCanceledException)
						{
						}
					}
					if (this.abortKeyThread)
					{
						this.keyThread = null;
						return;
					}
					bool showedGamepad = false;
					obj = OverlayManager.LockGamepadWindow;
					lock (obj)
					{
						if (OverlayManager.gamepadWindow != null)
						{
							showedGamepad = true;
						}
					}
					bool showedMessagesInThread = false;
					obj = OverlayManager.locakAplaySettings;
					lock (obj)
					{
						if (this.showMessagesSettings != 0)
						{
							showedMessagesInThread = true;
						}
					}
					ArrayList batteryWarnings = new ArrayList();
					using (await new AsyncLock(GamepadService._refreshGamepadCollectionSemaphore).LockAsync())
					{
						if (showedMessagesInThread && (DateTime.Now - this.prevCheckBattery).TotalSeconds >= 5.0)
						{
							this.prevCheckBattery = DateTime.Now;
							foreach (BaseControllerVM baseControllerVM in Engine.GamepadService.GamepadCollection)
							{
								bool flag2 = false;
								ControllerVM controllerVM = baseControllerVM as ControllerVM;
								if (controllerVM != null && ControllerTypeExtensions.IsLedAllowed(controllerVM.ControllerType))
								{
									flag2 = true;
								}
								if (baseControllerVM.ControllerFamily == null && !flag2 && baseControllerVM.IsOnline)
								{
									ControllerVM controllerVM2 = (ControllerVM)baseControllerVM;
									BatteryLevel controllerBatteryLevel = controllerVM2.ControllerBatteryLevel;
									if ((controllerBatteryLevel == 1 || controllerBatteryLevel == null) && controllerVM2.ControllerBatteryChargingState != 1 && controllerVM2.IsControllerBatteryBlockVisible)
									{
										DateTime dateTime2;
										if (controllerBatteryLevel == 1)
										{
											DateTime dateTime;
											if (!this.lastShowLowWarning.TryGetValue(controllerVM2.ID, out dateTime) || (DateTime.Now - dateTime).Minutes >= 5)
											{
												OverlayManager.BatteryWarningType batteryWarningType3 = new OverlayManager.BatteryWarningType();
												batteryWarningType3.deviceName = controllerVM2.ControllerDisplayName;
												batteryWarningType3.low = true;
												batteryWarningType3.firstControllerType = controllerVM2.FirstControllerType;
												batteryWarningType3.IsBatteryLevelPercentPresent = controllerVM2.IsBatteryLevelPercentPresent;
												batteryWarningType3.BatteryPercents = controllerVM2.GetBatteryPercents();
												if (controllerVM2.IsOnline)
												{
													batteryWarnings.Add(batteryWarningType3);
												}
												this.lastShowLowWarning[controllerVM2.ID] = DateTime.Now;
											}
										}
										else if (!this.lastShowCriticalWarning.TryGetValue(controllerVM2.ID, out dateTime2) || (DateTime.Now - dateTime2).Minutes >= 5)
										{
											OverlayManager.BatteryWarningType batteryWarningType2 = new OverlayManager.BatteryWarningType();
											batteryWarningType2.deviceName = controllerVM2.ControllerDisplayName;
											batteryWarningType2.low = false;
											batteryWarningType2.firstControllerType = controllerVM2.FirstControllerType;
											batteryWarningType2.IsBatteryLevelPercentPresent = controllerVM2.IsBatteryLevelPercentPresent;
											batteryWarningType2.BatteryPercents = controllerVM2.GetBatteryPercents();
											if (controllerVM2.IsOnline)
											{
												batteryWarnings.Add(batteryWarningType2);
											}
											this.lastShowCriticalWarning[controllerVM2.ID] = DateTime.Now;
										}
									}
									else
									{
										this.lastShowLowWarning.Remove(controllerVM2.ID);
										this.lastShowCriticalWarning.Remove(controllerVM2.ID);
									}
								}
							}
						}
					}
					AsyncLock.Releaser releaser;
					AsyncLock.Releaser releaser2 = (releaser = await new AsyncLock(GamepadService._refreshServiceProfilesSemaphore).LockAsync());
					try
					{
						if (showedGamepad)
						{
							await this.ReadVirtualControllerStates();
						}
					}
					finally
					{
						((IDisposable)releaser).Dispose();
					}
					releaser = default(AsyncLock.Releaser);
					using (IEnumerator enumerator2 = batteryWarnings.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							OverlayManager.BatteryWarningType batteryWarningType = (OverlayManager.BatteryWarningType)enumerator2.Current;
							Application.Current.Dispatcher.Invoke(delegate
							{
								this.OnBatteryState(batteryWarningType.low, batteryWarningType.deviceName, batteryWarningType.firstControllerType, batteryWarningType.IsBatteryLevelPercentPresent, batteryWarningType.BatteryPercents);
							});
						}
					}
					ArrayList arrayList = new ArrayList();
					foreach (KeyValuePair<string, VirtualGamepadInfo> keyValuePair in this.vControllerStateHash)
					{
						if (!keyValuePair.Value.isPresent)
						{
							arrayList.Add(keyValuePair.Key);
						}
					}
					foreach (object obj2 in arrayList)
					{
						string text = (string)obj2;
						this.vControllerStateHash.Remove(text);
					}
					if (showedGamepad)
					{
						this.HandleVirtualControllerStates();
					}
					batteryWarnings = null;
				}
				Tracer.TraceWrite("OverlayManager Thread Exit", false);
				this.keyThread = null;
			})
			{
				IsBackground = true
			};
		}

		public void HideMessages()
		{
			OverlayTracker overlayTracker = this.overlayTracker;
			if (overlayTracker != null)
			{
				overlayTracker.HideOverlayMessage();
			}
			Tracer.TraceWrite("OverlayManager HideMessages", false);
			MessagesVM mvm = this._mvm;
			if (mvm != null)
			{
				mvm.RemoveAll();
			}
			if (this.messagesWindow != null)
			{
				this.messagesWindow.Hide();
				this.messagesWindow.Close();
				this.messagesWindow = null;
				this.timeCloseMessagesWindow = new DateTime?(DateTime.Now);
				this.idCloseMessage = OverlayManager.messagesFileID;
			}
			this.showedMessages = false;
		}

		public void StartKeyThread()
		{
			if (this.keyThread == null)
			{
				this.abortKeyThread = false;
				this.CreateNewThread();
				this.keyThread.Start();
			}
		}

		public void StopKeyThread()
		{
			if (this.keyThread != null)
			{
				this.abortKeyThread = true;
			}
		}

		private void OverlayGamepadHotkey(string ID, string controllerDisplayName, string gameName, string profileName)
		{
			OverlayManager.<>c__DisplayClass97_0 CS$<>8__locals1 = new OverlayManager.<>c__DisplayClass97_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.ID = ID;
			Tracer.TraceWrite("OverlayManager OverlayGamepadHotkey", false);
			if (OverlayManager.gamepadWindow != null)
			{
				Application.Current.Dispatcher.Invoke(delegate
				{
					CS$<>8__locals1.<>4__this.HideGamepad();
					CS$<>8__locals1.<>4__this.waitReleaseAllkeysAfterHideGamepad = true;
				});
				return;
			}
			if (!Engine.UserSettingsService.IsOverlayEnable || this.showGamepadSettings == 0)
			{
				return;
			}
			if (string.IsNullOrEmpty(gameName) || string.IsNullOrEmpty(profileName))
			{
				Tracer.TraceWrite("OverlayManager OverlayGamepadHotkey: Not showed ((GameName.Empty || profileName.Empty)", false);
				return;
			}
			this.timeFinishLastKeyPress = new DateTime?(DateTime.Now);
			VirtualGamepadInfo virtualGamepadInfo = null;
			using (IEnumerator<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> enumerator = Engine.GamepadService.ServiceProfilesCollection.Where((Wrapper<REWASD_CONTROLLER_PROFILE_EX> x) => x.Value.ProfilesState[x.Value.GetCurrentSlot()].Enabled && x.Value.ProfilesState[x.Value.GetCurrentSlot()].VirtualType > 0U).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					Wrapper<REWASD_CONTROLLER_PROFILE_EX> wrapper = enumerator.Current;
					Slot currentSlot = wrapper.Value.GetCurrentSlot();
					REWASD_GET_PROFILE_STATE_RESPONSE rewasd_GET_PROFILE_STATE_RESPONSE = wrapper.Value.ProfilesState[currentSlot];
					string text = rewasd_GET_PROFILE_STATE_RESPONSE.VirtualId.ToString();
					ControllerTypeEnum controllerTypeEnum = ControllerTypeExtensions.ConvertPhysicalTypeToEnum(0, rewasd_GET_PROFILE_STATE_RESPONSE.VirtualType, 0UL);
					if (!this.vControllerStateHash.TryGetValue(text, out virtualGamepadInfo))
					{
						virtualGamepadInfo = new VirtualGamepadInfo();
					}
					virtualGamepadInfo.controllerType = controllerTypeEnum;
				}
			}
			if (virtualGamepadInfo != null)
			{
				Application.Current.Dispatcher.Invoke(delegate
				{
					CS$<>8__locals1.<>4__this.waitReleaseAllkeysAfterHideGamepad = false;
					CS$<>8__locals1.<>4__this.ShowGamepad(CS$<>8__locals1.ID, new ControllerTypeEnum?(virtualGamepadInfo.controllerType));
					CS$<>8__locals1.<>4__this.waitpollingRate.Cancel();
					CS$<>8__locals1.<>4__this.SavePictureGamepadWindow();
				});
			}
		}

		private void OverlayMappingHotkey(string ID, string controllerDisplayName, string gameName, string profileName, bool isDescriptions)
		{
			Tracer.TraceWrite("OverlayManager OverlayMappingHotkey", false);
			if (!Engine.UserSettingsService.IsOverlayEnable || this.showMappings == 0)
			{
				return;
			}
			if (this.remapWindow == null)
			{
				this.PrepareAndShowRemap(ID, controllerDisplayName, gameName, profileName, isDescriptions);
				return;
			}
			if (this.remapWindow.IsDescriptionWindow == isDescriptions)
			{
				this.NextPageOrHideRemap();
				return;
			}
			this.HideRemap();
			this.PrepareAndShowRemap(ID, controllerDisplayName, gameName, profileName, isDescriptions);
		}

		private void PrepareAndShowRemap(string ID, string controllerDisplayName, string gameName, string profileName, bool isDescriptions)
		{
			ConfigData configData = RemapWindowFactory.CreateConfigData(gameName, profileName);
			if (configData != null)
			{
				this.ShowRemap(ID, configData, isDescriptions);
				this.SavePictureRemapWindow();
			}
		}

		public void ApplySetting()
		{
			object obj = OverlayManager.LockGamepadWindow;
			lock (obj)
			{
				Tracer.TraceWrite("OverlayManager ApplySetting", false);
				this.showGamepadSettings = RegistryHelper.GetValue("Overlay", "ShowGamepad", 1, false);
				this.showControllerOnly = RegistryHelper.GetValue("Overlay", "ShowControllerOnly", 0, false);
				this.showMessagesSettings = RegistryHelper.GetValue("Overlay", "ShowMessages", 1, false);
				this.showShortMessagesSettings = RegistryHelper.GetValue("Overlay", "ShowShortMessages", 0, false);
				this.showBatteryIsLow = RegistryHelper.GetValue("Overlay", "ShowBatteryIsLow", 0, false);
				this.showBatteryIsCritical = RegistryHelper.GetValue("Overlay", "ShowBatteryIsCritical", 1, false);
				this.showSlotIsChanged = RegistryHelper.GetValue("Overlay", "ShowSlotIsChanged", 1, false);
				this.showShiftIsChanged = RegistryHelper.GetValue("Overlay", "ShowShiftIsChanged", 1, false);
				this.showShiftIsChangedToggle = RegistryHelper.GetValue("Overlay", "showShiftIsChangedToggle", 1, false);
				this.showDisconnected = RegistryHelper.GetValue("Overlay", "ShowDisconnected", 1, false);
				this.showRemapIsONOFF = RegistryHelper.GetValue("Overlay", "ShowRemapIsONOFF", 1, false);
				this.autohideGamepad = RegistryHelper.GetValue("Overlay", "AutohideGamepad", 0, false);
				this.showMappings = RegistryHelper.GetValue("Overlay", "ShowMappings", 1, false);
				this.AlignNotification = RegistryHelper.GetValue("Overlay", "AlignNotification", 3, false);
				this.AlignMappings = RegistryHelper.GetValue("Overlay", "AlignMappings", 2, false);
				this.AlignGamepad = RegistryHelper.GetValue("Overlay", "AlignGamepad", 3, false);
				this.TransparentMessages = (float)RegistryHelper.GetValue("Overlay", "TransparentMessages", 80, false) / 100f;
				this.TransparentGamepad = (float)RegistryHelper.GetValue("Overlay", "TransparentGamepad", 80, false) / 100f;
				this.TransparentMappings = (float)RegistryHelper.GetValue("Overlay", "TransparentMappings", 80, false) / 100f;
				this.MonitorMessages = RegistryHelper.GetString("Overlay", "MonitorMessages", "", false);
				this.MonitorGamepad = RegistryHelper.GetString("Overlay", "MonitorGamepad", "", false);
				this.MonitorMappings = RegistryHelper.GetString("Overlay", "MonitorMappings", "", false);
				this.timeMessageSec = RegistryHelper.GetValue("Overlay", "TimeMessages", 5, false) + (OverlayManager.MESSAGES_DEBUG_MODE ? 400 : 0);
				this.notHideShift = RegistryHelper.GetValue("Overlay", "NotHideShift", 0, false);
				Tracer.TraceWrite("OverlayManager OnSettingsChanged IsOverlayEnable=" + Engine.UserSettingsService.IsOverlayEnable.ToString() + " isDirectXOverlayEnable" + Engine.UserSettingsService.ShowDirectXOverlay.ToString(), false);
				if (Engine.UserSettingsService.IsOverlayEnable && Engine.UserSettingsService.ShowDirectXOverlay)
				{
					if (this.overlayTracker == null)
					{
						Tracer.TraceWrite("OverlayManager start OverlayTracker:", false);
						this.overlayTracker = new OverlayTracker();
					}
				}
				else if (this.overlayTracker != null)
				{
					Tracer.TraceWrite("OverlayManager stop OverlayTracker:", false);
					this.overlayTracker.DeactivateALL();
					this.overlayTracker = null;
				}
			}
			obj = OverlayManager.LockPollingRate;
			lock (obj)
			{
				this.PollingRate = RegistryHelper.GetValue("Overlay", "PollingRate", 50, false);
			}
			OverlayTracker overlayTracker = this.overlayTracker;
			if (overlayTracker != null)
			{
				overlayTracker.DeactivateDeletedFromPreferences();
			}
			this.AplayVisualChangest();
		}

		private void AplayVisualChangest()
		{
			if (Engine.UserSettingsService.IsOverlayEnable)
			{
				this.StartKeyThread();
			}
			else
			{
				this.StopKeyThread();
			}
			this.overlayMenuE.HideMenu();
			if (this.remapWindow != null && Engine.UserSettingsService.IsOverlayEnable && this.showMappings != 0)
			{
				this.RecreateMappings();
			}
			else
			{
				this.HideRemap();
			}
			if (OverlayManager.gamepadWindow != null && Engine.UserSettingsService.IsOverlayEnable && this.showGamepadSettings != 0)
			{
				this.RecreateGamepad();
			}
			else
			{
				this.HideGamepad();
			}
			if (this.messagesWindow != null)
			{
				this.messagesWindow.Align = this.AlignNotification;
				MessagesVM mvm = this._mvm;
				if (mvm != null)
				{
					mvm.SetTransparent(this.TransparentMessages);
				}
				MessagesVM mvm2 = this._mvm;
				if (mvm2 != null)
				{
					mvm2.SetScale(Engine.UserSettingsService.MessagesWidowScale);
				}
			}
			this.ArrayMessageWindow();
			this.overlayMenuE.ClearCachedMenu("", "");
		}

		public float GetOffsetSamePosition()
		{
			float num = 0f;
			if (this.messagesWindow != null)
			{
				float num2 = 0f;
				float num3 = 0f;
				if (OverlayManager.gamepadWindow != null && this.MonitorMessages.Equals(this.MonitorGamepad) && OverlayManager.gamepadWindow.Align == this.messagesWindow.Align)
				{
					num2 = (float)OverlayManager.gamepadWindow.ActualHeight;
				}
				if (this.remapWindow != null && this.MonitorMessages.Equals(this.MonitorMappings) && this.remapWindow.Align == this.messagesWindow.Align)
				{
					num3 = (float)this.remapWindow.ActualHeight;
				}
				num += Math.Max(num2, num3);
			}
			return num;
		}

		public void ArrayMessageWindow()
		{
			if (this.messagesWindow != null)
			{
				OverlayUtils.SetAlign(this.MonitorMessages, this.messagesWindow.Align, this.GetOffsetSamePosition(), this.messagesWindow);
			}
		}

		private void OnControllerRemoved(BaseControllerVM controller, string fullID, string nameForDisconnect)
		{
			Tracer.TraceWrite("OverlayManager OnControllerRemoved", false);
			if (controller.IsCompositeDevice)
			{
				return;
			}
			string ID = controller.ID;
			ThreadHelper.ExecuteInMainDispatcher(delegate
			{
				if (Engine.UserSettingsService.IsOverlayEnable && this.showMessagesSettings != 0 && this.showDisconnected != 0)
				{
					this.ShowMessages();
					MessagesVM mvm = this._mvm;
					if (mvm != null)
					{
						mvm.ShowDisconnected(fullID, nameForDisconnect, this.IsReplace(), this.showShortMessagesSettings == 1, OverlayUtils.ConvertToWindowsAligment(this.AlignNotification), controller);
					}
					this.SavePictureMessages();
				}
				if (this.remapWindow != null && OverlayUtils.IsContainsInLongID(this.remapWindow.ViewModel.ID, ID))
				{
					this.HideRemap();
				}
				if (OverlayManager.gamepadWindow != null && OverlayUtils.IsContainsInLongID(OverlayManager.gamepadWindow.ViewModel.ID, ID))
				{
					this.HideGamepad();
				}
			}, true);
		}

		private async void OffileDevice(BaseControllerVM controller)
		{
			string ID = controller.ID;
			bool isAllOffline = true;
			using (await new AsyncLock(GamepadService._refreshGamepadCollectionSemaphore).LockAsync())
			{
				foreach (BaseControllerVM baseControllerVM in Engine.GamepadService.GamepadCollection)
				{
					CompositeControllerVM compositeControllerVM = baseControllerVM as CompositeControllerVM;
					if (compositeControllerVM != null && baseControllerVM.ID.Contains(ID))
					{
						foreach (BaseControllerVM baseControllerVM2 in compositeControllerVM.BaseControllers)
						{
							if (baseControllerVM2 != null && baseControllerVM2.IsOnline)
							{
								isAllOffline = false;
								break;
							}
						}
					}
				}
			}
			if (isAllOffline)
			{
				this.HideAllOverlays(ID);
			}
		}

		private void HideAllOverlays(string ID)
		{
			ThreadHelper.ExecuteInMainDispatcher(delegate
			{
				if (this.remapWindow != null && this.remapWindow.ViewModel.ID.Contains(ID))
				{
					this.HideRemap();
				}
				if (OverlayManager.gamepadWindow != null && OverlayManager.gamepadWindow.ViewModel.ID.Contains(ID))
				{
					this.HideGamepad();
				}
				if (this.overlayMenuE.IsShowed && OverlayUtils.IsContainsInLongID(this.overlayMenuE.ID, ID))
				{
					this.overlayMenuE.HideMenu();
				}
			}, true);
		}

		public void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
		{
			this.AplayVisualChangest();
		}

		private void OnConfigAppliedToSlot(BaseControllerVM controller, GamepadProfile gamepadProfile, Slot iSlot, REWASD_CONTROLLER_PROFILE? profile)
		{
			OverlayManager.<>c__DisplayClass108_0 CS$<>8__locals1 = new OverlayManager.<>c__DisplayClass108_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.controller = controller;
			CS$<>8__locals1.ID = "";
			CS$<>8__locals1.controllerDisplayName = "";
			if (CS$<>8__locals1.controller != null)
			{
				CS$<>8__locals1.ID = CS$<>8__locals1.controller.ID;
				CS$<>8__locals1.controllerDisplayName = CS$<>8__locals1.controller.ControllerDisplayName;
			}
			this.HideAllOverlays(CS$<>8__locals1.ID);
			CS$<>8__locals1.slot = -1;
			switch (iSlot)
			{
			case 0:
				CS$<>8__locals1.slot = 1;
				break;
			case 1:
				CS$<>8__locals1.slot = 2;
				break;
			case 2:
				CS$<>8__locals1.slot = 3;
				break;
			case 3:
				CS$<>8__locals1.slot = 4;
				break;
			}
			if (!Engine.UserSettingsService.IsOverlayEnable || this.showMessagesSettings == 0)
			{
				return;
			}
			if (this.showRemapIsONOFF == 0)
			{
				return;
			}
			CS$<>8__locals1.gameName = "";
			CS$<>8__locals1.profileName = "";
			if (gamepadProfile != null)
			{
				CS$<>8__locals1.gameName = gamepadProfile.GameName;
				CS$<>8__locals1.profileName = gamepadProfile.ProfileName;
			}
			CS$<>8__locals1.empty = true;
			CS$<>8__locals1.isNeedAnyFeature = false;
			if (profile != null)
			{
				CS$<>8__locals1.empty = false;
				CS$<>8__locals1.isNeedAnyFeature = REWASD_CONTROLLER_PROFILE_Extensions.IsNeedAnyFeature(profile.Value);
			}
			Tracer.TraceWrite("OverlayManager OnConfigAppliedToSlot gameName:" + CS$<>8__locals1.gameName + "  profileName:" + CS$<>8__locals1.profileName, false);
			CS$<>8__locals1.gameConfig = string.Format("{0}: {1}", CS$<>8__locals1.gameName, CS$<>8__locals1.profileName);
			CS$<>8__locals1.isVirtual = false;
			IGameProfilesService gameProfilesService = Engine.GameProfilesService;
			Game game = ((gameProfilesService != null) ? gameProfilesService.GamesCollection.FirstOrDefault((Game g) => g.Name.Equals(CS$<>8__locals1.gameName)) : null);
			if (game != null)
			{
				Config config = game.ConfigCollection.FirstOrDefault((Config c) => c.Name.Equals(CS$<>8__locals1.profileName));
				if (config == null)
				{
					return;
				}
				config.ReadConfigFromJsonIfNotLoaded(false);
				OverlayManager.<>c__DisplayClass108_0 CS$<>8__locals2 = CS$<>8__locals1;
				ConfigData configData = config.ConfigData;
				CS$<>8__locals2.isVirtual = configData != null && configData.IsVirtualGamepadMappingPresent();
				this.overlayMenuE.ClearCachedMenu(CS$<>8__locals1.gameName, config.Name);
			}
			ThreadHelper.ExecuteInMainDispatcher(delegate
			{
				CS$<>8__locals1.<>4__this.ShowMessages();
				MessagesVM mvm = CS$<>8__locals1.<>4__this._mvm;
				if (mvm != null)
				{
					mvm.ShowSlot(CS$<>8__locals1.ID, CS$<>8__locals1.controllerDisplayName, CS$<>8__locals1.gameConfig, CS$<>8__locals1.slot, CS$<>8__locals1.empty, CS$<>8__locals1.<>4__this.IsReplace(), CS$<>8__locals1.<>4__this.showGamepadSettings != 0, CS$<>8__locals1.<>4__this.showMappings != 0, CS$<>8__locals1.isVirtual, CS$<>8__locals1.<>4__this.showShortMessagesSettings == 1, OverlayUtils.ConvertToWindowsAligment(CS$<>8__locals1.<>4__this.AlignNotification), CS$<>8__locals1.isNeedAnyFeature, CS$<>8__locals1.controller.IsOnline);
				}
				CS$<>8__locals1.<>4__this.lastShowLowWarning.Remove(CS$<>8__locals1.ID);
				CS$<>8__locals1.<>4__this.lastShowCriticalWarning.Remove(CS$<>8__locals1.ID);
				CS$<>8__locals1.<>4__this.SavePictureMessages();
			}, true);
		}

		private void OverlaySlotChange(BaseControllerVM controller, GamepadProfile gamepadProfile, Slot iSlot, REWASD_CONTROLLER_PROFILE? profile, bool physical)
		{
			OverlayManager.<>c__DisplayClass109_0 CS$<>8__locals1 = new OverlayManager.<>c__DisplayClass109_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.controller = controller;
			CS$<>8__locals1.ID = "";
			CS$<>8__locals1.controllerDisplayName = "";
			if (CS$<>8__locals1.controller != null)
			{
				CS$<>8__locals1.ID = CS$<>8__locals1.controller.ID;
				CS$<>8__locals1.controllerDisplayName = CS$<>8__locals1.controller.ControllerDisplayName;
			}
			Tracer.TraceWrite("OverlayManager OverlaySlotShiftChange ID:" + CS$<>8__locals1.ID, false);
			this.HideAllOverlays(CS$<>8__locals1.ID);
			CS$<>8__locals1.slot = -1;
			switch (iSlot)
			{
			case 0:
				CS$<>8__locals1.slot = 1;
				break;
			case 1:
				CS$<>8__locals1.slot = 2;
				break;
			case 2:
				CS$<>8__locals1.slot = 3;
				break;
			case 3:
				CS$<>8__locals1.slot = 4;
				break;
			}
			if (!Engine.UserSettingsService.IsOverlayEnable || this.showMessagesSettings == 0)
			{
				return;
			}
			if (this.showSlotIsChanged == 0)
			{
				return;
			}
			CS$<>8__locals1.gameName = "";
			CS$<>8__locals1.profileName = "";
			if (gamepadProfile != null)
			{
				CS$<>8__locals1.gameName = gamepadProfile.GameName;
				CS$<>8__locals1.profileName = gamepadProfile.ProfileName;
			}
			CS$<>8__locals1.empty = true;
			CS$<>8__locals1.isNeedAnyFeature = false;
			if (profile == null)
			{
				return;
			}
			if (profile != null)
			{
				Tracer.TraceWrite("OverlayManager OverlaySlotShiftChange gameName:" + CS$<>8__locals1.gameName + "  profileName:" + CS$<>8__locals1.profileName, false);
				CS$<>8__locals1.empty = !REWASD_CONTROLLER_PROFILE_Extensions.IsSlotWithUserConfig(profile.Value);
				if (profile.Value.Id.All((ulong x) => x == 0UL))
				{
					Tracer.TraceWrite("OverlayManager OverlaySlotShiftChange Empty PhysicalSlot. Return", false);
					return;
				}
			}
			else
			{
				Tracer.TraceWrite(string.Concat(new string[] { "OverlayManager OverlaySlotShiftChange gameName:", CS$<>8__locals1.gameName, "  profileName:", CS$<>8__locals1.profileName, "  profile: NULL" }), false);
			}
			CS$<>8__locals1.gameConfig = string.Format("{0}: {1}", CS$<>8__locals1.gameName, CS$<>8__locals1.profileName);
			CS$<>8__locals1.isVirtual = false;
			IGameProfilesService gameProfilesService = Engine.GameProfilesService;
			Game game = ((gameProfilesService != null) ? gameProfilesService.GamesCollection.FirstOrDefault((Game g) => g.Name.Equals(CS$<>8__locals1.gameName)) : null);
			if (game != null)
			{
				Config config = game.ConfigCollection.FirstOrDefault((Config c) => c.Name.Equals(CS$<>8__locals1.profileName));
				if (config == null)
				{
					return;
				}
				config.ReadConfigFromJsonIfNotLoaded(false);
				OverlayManager.<>c__DisplayClass109_0 CS$<>8__locals2 = CS$<>8__locals1;
				ConfigData configData = config.ConfigData;
				CS$<>8__locals2.isVirtual = configData != null && configData.IsVirtualGamepadMappingPresent();
			}
			ThreadHelper.ExecuteInMainDispatcher(delegate
			{
				CS$<>8__locals1.<>4__this.ShowMessages();
				MessagesVM mvm = CS$<>8__locals1.<>4__this._mvm;
				if (mvm != null)
				{
					mvm.ShowSlot(CS$<>8__locals1.ID, CS$<>8__locals1.controllerDisplayName, CS$<>8__locals1.gameConfig, CS$<>8__locals1.slot, CS$<>8__locals1.empty, CS$<>8__locals1.<>4__this.IsReplace(), CS$<>8__locals1.<>4__this.showGamepadSettings != 0, CS$<>8__locals1.<>4__this.showMappings != 0, CS$<>8__locals1.isVirtual, CS$<>8__locals1.<>4__this.showShortMessagesSettings == 1, OverlayUtils.ConvertToWindowsAligment(CS$<>8__locals1.<>4__this.AlignNotification), CS$<>8__locals1.isNeedAnyFeature, CS$<>8__locals1.controller.IsOnline);
				}
				CS$<>8__locals1.<>4__this.SavePictureMessages();
			}, true);
		}

		public void ReceivedOverlayMessage(OverlayMessageType messageType, IntPtr lParam)
		{
			OverlayTracker overlayTracker = this.overlayTracker;
			if (overlayTracker == null)
			{
				return;
			}
			overlayTracker.ReceivedOverlayMessage(messageType, lParam);
		}

		public void RemapRendered(int id)
		{
			OverlayTracker overlayTracker = this.overlayTracker;
			if (overlayTracker == null)
			{
				return;
			}
			overlayTracker.ShowOverlayRemap(id, this.AlignMappings);
		}

		public void MessageRendered(int id)
		{
			OverlayTracker overlayTracker = this.overlayTracker;
			if (overlayTracker == null)
			{
				return;
			}
			overlayTracker.ShowOverlayMessage(id, this.AlignNotification);
		}

		public void GamepadRendered(int id)
		{
			OverlayTracker overlayTracker = this.overlayTracker;
			if (overlayTracker == null)
			{
				return;
			}
			overlayTracker.ShowOverlayGamepad(id, this.AlignGamepad);
		}

		private async void ForegroundApplicationMonitorServiceOnChanged(object sender, EventArgs eventArgs)
		{
			Tracer.TraceWrite("OverlayManager ForegroundApplicationMonitorServiceOnChanged  IsOverlayEnable=" + Engine.UserSettingsService.IsOverlayEnable.ToString() + " isDirectXOverlayEnable" + Engine.UserSettingsService.ShowDirectXOverlay.ToString(), false);
			if (Engine.UserSettingsService.IsOverlayEnable && Engine.UserSettingsService.ShowDirectXOverlay)
			{
				IForegroundApplicationMonitorService _foregroundApplicationMonitorService = IContainerProviderExtensions.Resolve<IForegroundApplicationMonitorService>(Engine.SContainer);
				if (_foregroundApplicationMonitorService.Process != null)
				{
					using (await new AsyncLock(this._foregroundApplicationMonitorSemaphore).LockAsync())
					{
						string text;
						int id;
						try
						{
							text = _foregroundApplicationMonitorService.Process.ProcessName;
							id = _foregroundApplicationMonitorService.Process.Id;
						}
						catch (Exception)
						{
							return;
						}
						text = text.ToLower();
						if (!(this.prevProcName == text))
						{
							this.prevProcName = text;
							if (!(text == this.curRemapedForegroundName))
							{
								this.curRemapedForegroundName = this.prevProcName.ToLower();
								string text2 = Engine.UserSettingsService.DirectX_Apps.ToLower();
								Tracer.TraceWrite("OverlayManager ForegroundApplicationMonitorServiceOnChanged DirectX_Apps " + text2, false);
								Tracer.TraceWrite(string.Concat(new string[]
								{
									"OverlayManager process ",
									this.curRemapedForegroundName,
									" (id:",
									id.ToString(),
									") checking"
								}), false);
								bool flag = false;
								string[] array = text2.Split(';', StringSplitOptions.None);
								for (int i = 0; i < array.Length; i++)
								{
									if (array[i].StartsWith(this.curRemapedForegroundName))
									{
										flag = true;
									}
								}
								if (flag)
								{
									string text3 = this.CheckAndCreateDirectoryForOverlayData() + "\\BlackListDirectXAPP";
									if (File.Exists(text3) && File.ReadAllLines(text3).ToList<string>().FirstOrDefault((string stringToCheck) => stringToCheck.ToLower().Contains(this.curRemapedForegroundName)) != null)
									{
										Tracer.TraceWrite(string.Concat(new string[]
										{
											"OverlayManager process ",
											this.curRemapedForegroundName,
											" (id:",
											id.ToString(),
											") in black list, ignore "
										}), false);
									}
									else
									{
										Tracer.TraceWrite("OverlayManager Trying to inject:" + this.curRemapedForegroundName + "  Id:" + id.ToString(), false);
										OverlayTracker overlayTracker = this.overlayTracker;
										if (!((overlayTracker != null) ? new bool?(overlayTracker.Inject(id, this.curRemapedForegroundName)) : null).Value)
										{
											Tracer.TraceLog("11111 968  Inject Error Message: ");
											this.ShowTestMessages();
										}
									}
								}
								else
								{
									Tracer.TraceWrite(string.Concat(new string[]
									{
										"OverlayManager process ",
										this.curRemapedForegroundName,
										" (id:",
										id.ToString(),
										") not found in inject process list."
									}), false);
								}
							}
						}
					}
				}
			}
		}

		private async void LoadBlackList()
		{
			string @string = RegistryHelper.GetString("Overlay", "LastTimeDownloadBlackList", "", false);
			DateTime minValue = DateTime.MinValue;
			if (@string.Length > 0)
			{
				DateTime.TryParse(@string, out minValue);
			}
			if ((DateTime.Now - minValue).TotalSeconds > 86000.0)
			{
				try
				{
					Tracer.TraceWrite("OverlayManager LoadBlackList:", false);
					string name = Thread.CurrentThread.CurrentCulture.Name;
					Path.GetTempFileName();
					string text = await new HttpClient().GetStringAsync("https://services.rewasd.com/rewasd/games/processNames");
					if (text.Length == 0)
					{
						Tracer.TraceWrite("OverlayManager LoadBlackList error", false);
					}
					else
					{
						RegistryHelper.SetString("Overlay", "LastTimeDownloadBlackList", DateTime.Now.ToString());
						XmlReader xmlReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(text), new XmlDictionaryReaderQuotas());
						string text2 = "";
						XContainer xcontainer = XElement.Load(xmlReader);
						Tracer.TraceWrite("OverlayManager black list loaded:", false);
						foreach (XElement xelement in xcontainer.Element("blocked").Elements())
						{
							Tracer.TraceWrite(xelement.Value, false);
							text2 += xelement.Value;
							text2 += "\n";
						}
						if (text2.Length > 0)
						{
							string text3 = this.CheckAndCreateDirectoryForOverlayData() + "\\BlackListDirectXAPP";
							if (File.Exists(text3))
							{
								File.Delete(text3);
							}
							using (FileStream fileStream = File.Create(text3))
							{
								byte[] bytes = new UTF8Encoding(true).GetBytes(text2);
								fileStream.Write(bytes, 0, bytes.Length);
							}
						}
					}
				}
				catch (Exception ex)
				{
					Tracer.TraceException(ex, "LoadBlackList");
				}
			}
		}

		private void SetBatteryNotification(BaseControllerVM gamepad)
		{
			Tracer.TraceWrite("OverlayManager SetBatteryNotification", false);
			ControllerVM controller = gamepad as ControllerVM;
			if (controller != null)
			{
				Tracer.TraceWrite("OverlayManager SetBatteryNotification + " + controller.ControllerBatteryLevel.ToString(), false);
				BatteryLevel controllerBatteryLevel = controller.ControllerBatteryLevel;
				if (controller.IsOnline && (controllerBatteryLevel == 1 || controllerBatteryLevel == null) && controller.ControllerBatteryChargingState != 1 && controller.IsControllerBatteryBlockVisible)
				{
					bool low = controllerBatteryLevel == 1;
					Tracer.TraceWrite("OverlayManager SetBatteryNotification. low== + " + low.ToString(), false);
					Application.Current.Dispatcher.Invoke(delegate
					{
						this.OnBatteryState(low, controller.ControllerDisplayName, controller.FirstControllerType, controller.IsBatteryLevelPercentPresent, controller.GetBatteryPercents());
					});
				}
			}
		}

		private void ShowGamepad(string ID, ControllerTypeEnum? controllerType = null)
		{
			object lockGamepadWindow = OverlayManager.LockGamepadWindow;
			lock (lockGamepadWindow)
			{
				if (OverlayManager.gamepadWindow == null)
				{
					OverlayManager.gamepadWindow = new GamepadWindow();
					OverlayManager.gamepadWindow.overlayManager = this;
					OverlayManager.gamepadWindow.Align = this.AlignGamepad;
					this._gwvm.AlignmentSettings = OverlayUtils.ConvertToWindowsAligment(this.AlignGamepad);
					this._gwvm.Transparent = this.TransparentGamepad;
					this._gwvm.ID = ID;
					this._gwvm.HotKeyButtons = new HotkeysInfo();
					this._gwvm.HotKeyButtons.FillGroupsForID(ID, OverlayUtils.HotkeysType.Gamepad);
					this._gwvm.FillEnd();
					Tracer.TraceWrite("OverlayManager ShowGamepad", false);
					if (controllerType == null)
					{
						this._gwvm.IsGamepadVisible = false;
						this._gwvm.IsTableVisible = false;
					}
					OverlayManager.gamepadWindow.Show();
					this.StartPollerGamepad();
				}
				if (controllerType != null)
				{
					this._gwvm.SetControllerType(controllerType);
					this._gwvm.IsGamepadVisible = this.showGamepadSettings != 0;
					this._gwvm.IsTableVisible = this.showControllerOnly == 0;
				}
			}
		}

		private void HideGamepad()
		{
			object lockGamepadWindow = OverlayManager.LockGamepadWindow;
			lock (lockGamepadWindow)
			{
				this.timeFinishLastKeyPress = null;
				Tracer.TraceWrite("OverlayManager HideGamepad", false);
				OverlayTracker overlayTracker = this.overlayTracker;
				if (overlayTracker != null)
				{
					overlayTracker.HideOverlayGamepad();
				}
				GamepadWindow gamepadWindow = OverlayManager.gamepadWindow;
				if (gamepadWindow != null)
				{
					gamepadWindow.Hide();
				}
				GamepadWindow gamepadWindow2 = OverlayManager.gamepadWindow;
				if (gamepadWindow2 != null)
				{
					gamepadWindow2.Close();
				}
				OverlayManager.gamepadWindow = null;
				this.StopPollerGamepad();
				this.ArrayMessageWindow();
			}
		}

		private async Task<int> ReadVirtualControllerStates()
		{
			foreach (KeyValuePair<string, VirtualGamepadInfo> keyValuePair in this.vControllerStateHash)
			{
				keyValuePair.Value.isPresent = false;
			}
			foreach (Wrapper<REWASD_CONTROLLER_PROFILE_EX> wrapper in Engine.GamepadService.ServiceProfilesCollection.Where((Wrapper<REWASD_CONTROLLER_PROFILE_EX> x) => x.Value.ProfilesState[x.Value.GetCurrentSlot()].Enabled && x.Value.ProfilesState[x.Value.GetCurrentSlot()].VirtualType > 0U))
			{
				Slot currentSlot = wrapper.Value.GetCurrentSlot();
				REWASD_GET_PROFILE_STATE_RESPONSE rewasd_GET_PROFILE_STATE_RESPONSE = wrapper.Value.ProfilesState[currentSlot];
				ushort num = wrapper.Value.ServiceProfileIds[currentSlot];
				string virtualId = rewasd_GET_PROFILE_STATE_RESPONSE.VirtualId.ToString();
				ControllerTypeEnum controllerType = ControllerTypeExtensions.ConvertPhysicalTypeToEnum(0, rewasd_GET_PROFILE_STATE_RESPONSE.VirtualType, 0UL);
				VirtualGamepadInfo virtualGamepadInfo;
				if (!this.vControllerStateHash.TryGetValue(virtualId, out virtualGamepadInfo))
				{
					virtualGamepadInfo = new VirtualGamepadInfo();
				}
				GamepadState gamepadState = await Engine.XBServiceCommunicator.GetVirtualGamepadPressedButtons(num);
				virtualGamepadInfo.gamepadState = gamepadState;
				virtualGamepadInfo.controllerType = controllerType;
				virtualGamepadInfo.isPresent = true;
				this.vControllerStateHash[virtualId] = virtualGamepadInfo;
				virtualId = null;
				virtualGamepadInfo = null;
			}
			IEnumerator<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> enumerator2 = null;
			return 1;
		}

		private void HandleVirtualControllerStates()
		{
			using (IEnumerator<KeyValuePair<string, VirtualGamepadInfo>> enumerator = this.vControllerStateHash.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, VirtualGamepadInfo> entry = enumerator.Current;
					VirtualGamepadInfo virtualGamepadInfo = entry.Value;
					HashInfo hashInfo = OverlayUtils.CalcGamepadHash(virtualGamepadInfo.gamepadState, virtualGamepadInfo.oldHashGamepad);
					if (virtualGamepadInfo.oldHashGamepad != null && hashInfo.Hash != virtualGamepadInfo.oldHashGamepad.Hash)
					{
						Application.Current.Dispatcher.Invoke(delegate
						{
							object obj = OverlayManager.LockGamepadWindow;
							bool flag2;
							lock (obj)
							{
								flag2 = this.waitReleaseAllkeysAfterHideGamepad;
							}
							if (!flag2)
							{
								this.OverlayKeyPressed(entry.Key, virtualGamepadInfo.gamepadState, virtualGamepadInfo.controllerType);
							}
							obj = OverlayManager.LockGamepadWindow;
							lock (obj)
							{
								if (flag2 && virtualGamepadInfo.gamepadState.PressedButtons.Count == 0)
								{
									this.waitReleaseAllkeysAfterHideGamepad = false;
								}
							}
						});
					}
					virtualGamepadInfo.oldHashGamepad = hashInfo;
					if (virtualGamepadInfo.gamepadState.PressedButtons.Count > 0)
					{
						this.timeFinishLastKeyPress = new DateTime?(DateTime.Now);
					}
				}
			}
		}

		public void ExecuteOverlayMenuCommand(RewasdOverlayMenuServiceCommand rewasdOverlayMenuServiceCommand)
		{
			this.overlayMenuE.ExecuteOverlayMenuCommand(rewasdOverlayMenuServiceCommand);
		}

		private void ShowTestMessages()
		{
			ThreadHelper.ExecuteInMainDispatcher(delegate
			{
				this.ShowMessages();
				MessagesVM mvm = this._mvm;
				if (mvm == null)
				{
					return;
				}
				mvm.ShowError("Inject error", "... to Wither3.exe", OverlayUtils.ConvertToWindowsAligment(this.AlignNotification), true, false);
			}, true);
		}

		public static bool MESSAGES_DEBUG_MODE = false;

		private OverlayTracker _overlayTracker;

		public bool waitReleaseAllkeysAfterHideGamepad;

		public const int C_WINDOWS_MARGINS = 20;

		private static GamepadWindow gamepadWindow = null;

		private RemapWindow remapWindow;

		private MessagesWindow messagesWindow;

		private OverlayMenuE overlayMenuE = new OverlayMenuE();

		private DispatcherTimer pollingTimerGamepad;

		public static IUnityContainer Container;

		private Thread keyThread;

		private IDictionary<string, VirtualGamepadInfo> vControllerStateHash = new Dictionary<string, VirtualGamepadInfo>();

		public static int messagesFileID = 1;

		public static int remapFileID = 1;

		public static int gamepadFileID = 1;

		private DateTime? timeCloseMessagesWindow;

		private int idCloseMessage;

		private DateTime? timeFinishLastKeyPress;

		private DateTime prevCheckBattery = DateTime.Now;

		private IDictionary<string, DateTime> lastShowLowWarning = new Dictionary<string, DateTime>();

		private IDictionary<string, DateTime> lastShowCriticalWarning = new Dictionary<string, DateTime>();

		public float TransparentMessages;

		public float TransparentGamepad;

		public float TransparentMappings;

		public string MonitorMessages;

		public string MonitorGamepad;

		public string MonitorMappings;

		public int PollingRate;

		public AlignType AlignNotification;

		public AlignType AlignGamepad;

		public AlignType AlignMappings;

		public int showMessagesSettings;

		public int showShortMessagesSettings;

		public int showBatteryIsLow;

		public int showBatteryIsCritical;

		public int showSlotIsChanged;

		public int showShiftIsChanged;

		public int notHideShift;

		public int showShiftIsChangedToggle;

		public int showDisconnected;

		public int showRemapIsONOFF;

		public int showGamepadSettings;

		public int showControllerOnly;

		public int autohideGamepad;

		public int showMappings;

		private int timeMessageSec = 5;

		private bool showedMessages;

		public static int currentHeight = 0;

		private const int C_MAX_HEIGHT = 165;

		private DispatcherTimer _pollingTimerMessages;

		private bool abortKeyThread;

		private static object locakAplaySettings = new object();

		private static object LockGamepadWindow = new object();

		private static object LockPollingRate = new object();


		private string curRemapedForegroundName;

		private string prevProcName = "";

		private AsyncSemaphore _foregroundApplicationMonitorSemaphore = new AsyncSemaphore(1);

		public class BatteryWarningType
		{
			public string deviceName;

			public bool low;

			public ControllerTypeEnum firstControllerType;

			public byte BatteryPercents;

			public bool IsBatteryLevelPercentPresent;
		}
	}
}
