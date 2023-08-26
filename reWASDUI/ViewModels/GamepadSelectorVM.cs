using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.View.SecondaryWindows;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using QRCoder;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Infrastructure.UDP;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using reWASDCommon.Network.HTTP.DataTransferObjects.Events.Desktop;
using reWASDCommon.Utils;
using reWASDUI.Controls.XBBindingControls.BindingFrame.BindingFrameViews;
using reWASDUI.DataModels;
using reWASDUI.DataModels.GamepadActiveProfiles;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups;
using reWASDUI.Services;
using reWASDUI.Services.Interfaces;
using reWASDUI.Utils.XBUtil;
using reWASDUI.ViewModels.Base;
using reWASDUI.ViewModels.Preferences;
using reWASDUI.Views;
using reWASDUI.Views.ContentZoneGamepad;
using reWASDUI.Views.ContentZoneGamepad.AdvancedStick;
using reWASDUI.Views.Preferences;
using reWASDUI.Views.SecondaryWindows;
using reWASDUI.Views.SecondaryWindows.AddExternalDeviceWizard;
using XBEliteWPF.Infrastructure.ExternalDevices;
using XBEliteWPF.License.Licensing.ComStructures;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.ViewModels
{
	public class GamepadSelectorVM : BaseServicesVM, INavigationAware, IRegionMemberLifetime
	{
		public bool IsFriendlyNameEditVisible
		{
			get
			{
				return this._isFriendlyNameEditVisible;
			}
			set
			{
				this.SetProperty<bool>(ref this._isFriendlyNameEditVisible, value, "IsFriendlyNameEditVisible");
			}
		}

		public string VirtualControllerStateHint
		{
			get
			{
				BaseControllerVM currentGamepad = base.GamepadService.CurrentGamepad;
				if (currentGamepad == null || currentGamepad.RemapState != 1)
				{
					string @string = DTLocalization.GetString(12506);
					GameVM currentGame = base.GameProfilesService.CurrentGame;
					object obj;
					if (currentGame == null)
					{
						obj = null;
					}
					else
					{
						ConfigVM currentConfig = currentGame.CurrentConfig;
						if (currentConfig == null)
						{
							obj = null;
						}
						else
						{
							ConfigData configData = currentConfig.ConfigData;
							obj = ((configData != null) ? VirtualControllerTypeExtensions.GetVirtualGamepadTypeLocalizationId(configData.VirtualGamepadType) : null);
						}
					}
					return string.Format(@string, obj ?? DTLocalization.GetString(11396));
				}
				string string2 = DTLocalization.GetString(12505);
				BaseControllerVM currentGamepad2 = base.GamepadService.CurrentGamepad;
				return string.Format(string2, (currentGamepad2 != null) ? VirtualControllerTypeExtensions.GetVirtualGamepadTypeLocalizationId(currentGamepad2.VirtualGamepadType) : null);
			}
		}

		public bool IsMultipleControllers
		{
			get
			{
				return base.GamepadService.GamepadCollection.Count > 1;
			}
		}

		public bool KeepAlive
		{
			get
			{
				return true;
			}
		}

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
		}

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return true;
		}

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{
		}

		public string WillBeAppliedToSlotText
		{
			get
			{
				List<Slot> list = new List<Slot>();
				if (base.GameProfilesService.CurrentSlotInfo != null)
				{
					GameVM currentGame = base.GameProfilesService.CurrentGame;
					if (((currentGame != null) ? currentGame.CurrentConfig : null) != null && !string.IsNullOrEmpty(base.GameProfilesService.CurrentGame.Name) && base.GamepadService.CurrentGamepad != null)
					{
						if (base.GamepadService.AutoGamesDetectionGamepadProfileRelations.ContainsKey(base.GameProfilesService.CurrentGame.Name) && base.GamepadService.AutoGamesDetectionGamepadProfileRelations.ContainsKey(base.GameProfilesService.CurrentGame.Name) && base.GamepadService.AutoGamesDetectionGamepadProfileRelations[base.GameProfilesService.CurrentGame.Name].ContainsKey(base.GamepadService.CurrentGamepad.ID))
						{
							GamepadProfiles gamepadProfiles = base.GamepadService.AutoGamesDetectionGamepadProfileRelations[base.GameProfilesService.CurrentGame.Name][base.GamepadService.CurrentGamepad.ID];
							SlotProfilesDictionary slotProfilesDictionary = ((gamepadProfiles != null) ? gamepadProfiles.SlotProfiles : null);
							if (slotProfilesDictionary == null)
							{
								return "";
							}
							using (IEnumerator<KeyValuePair<Slot, GamepadProfile>> enumerator = slotProfilesDictionary.OrderBy((KeyValuePair<Slot, GamepadProfile> e) => e.Key.ToString()).GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									KeyValuePair<Slot, GamepadProfile> slot = enumerator.Current;
									if (slot.Value != null && slot.Value.Config == base.GameProfilesService.CurrentGame.CurrentConfig)
									{
										SlotInfo slotInfo = base.GamepadService.SlotsInfo.FirstOrDefault((SlotInfo si) => si.Slot == slot.Key);
										if (slotInfo != null && slotInfo.IsAvailable)
										{
											list.Add(slot.Key);
										}
									}
								}
							}
						}
						if (list.Count > 0)
						{
							string text = "";
							foreach (Slot slot2 in list)
							{
								text += slot2.TryGetLocalizedDescription();
								if (list.IndexOf(slot2) != list.Count - 1 && list.Count != 1)
								{
									text += ", ";
								}
							}
							return string.Format(DTLocalization.GetString(12087), text);
						}
						return "";
					}
				}
				return "";
			}
		}

		public DelegateCommand TriggerRemapCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._TriggerRemap) == null)
				{
					delegateCommand = (this._TriggerRemap = new DelegateCommand(new Action(this.TriggerRemap)));
				}
				return delegateCommand;
			}
		}

		private async void TriggerRemap()
		{
			if (!this._isApplyingProfile)
			{
				this._isApplyingProfile = true;
				try
				{
					await base.GamepadService.EnableDisableRemap(!base.GamepadService.IsCurrentGamepadRemaped);
				}
				catch (Exception)
				{
				}
				this._isApplyingProfile = false;
				this.ApplyProfileCommand.RaiseCanExecuteChanged();
			}
		}

		public DelegateCommand ApplyProfileCommand
		{
			get
			{
				if (this._applyProfile == null)
				{
					this._applyProfile = new DelegateCommand(async delegate
					{
						if (!this._isApplyingProfile)
						{
							this._isApplyingProfile = true;
							this.ApplyProfileCommand.RaiseCanExecuteChanged();
							await Task.Delay(10);
							await base.GameProfilesService.ApplyCurrentProfile(false);
							this._isApplyingProfile = false;
							this.ApplyProfileCommand.RaiseCanExecuteChanged();
						}
					}, new Func<bool>(this.ApplyProfileCanExecute));
				}
				return this._applyProfile;
			}
		}

		private bool ApplyProfileCanExecute()
		{
			return !this._isApplyingProfile && base.GameProfilesService.CanCurrentConfigBeAppliedNow && (!base.GameProfilesService.IsCurrentGamepadGameSlotAutodetect || !base.GameProfilesService.CurrentGame.IsAutodetect);
		}

		public bool IsUdpPresent
		{
			get
			{
				return this._isUdpPresent;
			}
			set
			{
				this.SetProperty<bool>(ref this._isUdpPresent, value, "IsUdpPresent");
			}
		}

		public bool IsUdpEnabledInPreferences
		{
			get
			{
				return this._isUdpEnabledInPreferences;
			}
			set
			{
				this.SetProperty<bool>(ref this._isUdpEnabledInPreferences, value, "IsUdpEnabledInPreferences");
			}
		}

		public bool IsUdpRunning
		{
			get
			{
				return this._isUdpRunning;
			}
			set
			{
				this.SetProperty<bool>(ref this._isUdpRunning, value, "IsUdpRunning");
			}
		}

		public bool IsUdpServerHasException
		{
			get
			{
				return this._isUdpServerHasException;
			}
			set
			{
				this.SetProperty<bool>(ref this._isUdpServerHasException, value, "IsUdpServerHasException");
			}
		}

		public string UdpRunningText
		{
			get
			{
				return this._udpRunningText;
			}
			set
			{
				this.SetProperty<string>(ref this._udpRunningText, value, "UdpRunningText");
			}
		}

		public bool IsUdpFull
		{
			get
			{
				return this._isUdpFull;
			}
			set
			{
				this.SetProperty<bool>(ref this._isUdpFull, value, "IsUdpFull");
			}
		}

		private async Task RefreshUdpState()
		{
			GamepadSelectorVM.<>c__DisplayClass49_0 CS$<>8__locals1 = new GamepadSelectorVM.<>c__DisplayClass49_0();
			await Task.Delay(200);
			GamepadSelectorVM.<>c__DisplayClass49_0 CS$<>8__locals2 = CS$<>8__locals1;
			BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
			CS$<>8__locals2.currentGamepadIds = ((currentGamepad != null) ? currentGamepad.Ids : null);
			GamepadUdpServerState gamepadUdpServerState = await App.HttpClientService.Engine.GetGamepadUdpServerState();
			if (gamepadUdpServerState != null)
			{
				List<PadMeta> list = gamepadUdpServerState.PadList ?? null;
				if (CS$<>8__locals1.currentGamepadIds != null)
				{
					this.IsUdpPresent = list.Any(delegate(PadMeta x)
					{
						IEnumerable<ulong> enumerable = x.PadIds.Where((ulong y) => y > 0UL);
						Func<ulong, bool> func;
						if ((func = CS$<>8__locals1.<>9__2) == null)
						{
							func = (CS$<>8__locals1.<>9__2 = (ulong id) => CS$<>8__locals1.currentGamepadIds.Where((ulong z) => z > 0UL).Contains(id));
						}
						return enumerable.All(func);
					});
				}
				else
				{
					this.IsUdpPresent = false;
				}
				this.IsUdpRunning = gamepadUdpServerState.IsUdpRunning;
				this.IsUdpEnabledInPreferences = gamepadUdpServerState.IsUdpEnabledInPreferences;
				this.IsUdpServerHasException = gamepadUdpServerState.IsUdpServerHasException;
				this.UdpRunningText = string.Format(DTLocalization.GetString(12327), "127.0.0.1", gamepadUdpServerState.Port);
				this.IsUdpFull = list.Count == 4;
			}
		}

		public DelegateCommand OpenUdpPreferencesCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._openUdpPreferencesCommand) == null)
				{
					delegateCommand = (this._openUdpPreferencesCommand = new DelegateCommand(new Action(this.OpenUdpPreferences)));
				}
				return delegateCommand;
			}
		}

		private void OpenUdpPreferences()
		{
			PreferencesWindow.ShowPreferences(typeof(PreferencesHttpVM));
		}

		public bool IsAdvancedMappingFeatureNotRequired
		{
			get
			{
				return this._isAdvancedMappingFeatureNotRequired;
			}
			set
			{
				this.SetProperty<bool>(ref this._isAdvancedMappingFeatureNotRequired, value, "IsAdvancedMappingFeatureNotRequired");
			}
		}

		public bool IsAdvancedMappingFeatureNotRequiredForUnmap
		{
			get
			{
				return this._isAdvancedMappingFeatureNotRequiredForUnmap;
			}
			set
			{
				this.SetProperty<bool>(ref this._isAdvancedMappingFeatureNotRequiredForUnmap, value, "IsAdvancedMappingFeatureNotRequiredForUnmap");
			}
		}

		public bool IsAdvancedMappingFeatureUnlocked
		{
			get
			{
				return this._isAdvancedMappingFeatureUnlocked;
			}
			set
			{
				this.SetProperty<bool>(ref this._isAdvancedMappingFeatureUnlocked, value, "IsAdvancedMappingFeatureUnlocked");
			}
		}

		public bool AdvancedMappingFeatureNotRequired()
		{
			BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
			bool flag;
			if (currentGamepad == null)
			{
				flag = false;
			}
			else
			{
				ControllerVM currentController = currentGamepad.CurrentController;
				ControllerTypeEnum? controllerTypeEnum = ((currentController != null) ? new ControllerTypeEnum?(currentController.ControllerType) : null);
				ControllerTypeEnum controllerTypeEnum2 = 16;
				flag = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
			}
			if (!flag)
			{
				BaseControllerVM currentGamepad2 = App.GamepadService.CurrentGamepad;
				bool flag2;
				if (currentGamepad2 == null)
				{
					flag2 = false;
				}
				else
				{
					ControllerVM currentController2 = currentGamepad2.CurrentController;
					ControllerTypeEnum? controllerTypeEnum = ((currentController2 != null) ? new ControllerTypeEnum?(currentController2.ControllerType) : null);
					ControllerTypeEnum controllerTypeEnum2 = 63;
					flag2 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
				}
				if (!flag2)
				{
					BaseControllerVM currentGamepad3 = App.GamepadService.CurrentGamepad;
					bool flag3;
					if (currentGamepad3 == null)
					{
						flag3 = false;
					}
					else
					{
						ControllerVM currentController3 = currentGamepad3.CurrentController;
						ControllerTypeEnum? controllerTypeEnum = ((currentController3 != null) ? new ControllerTypeEnum?(currentController3.ControllerType) : null);
						ControllerTypeEnum controllerTypeEnum2 = 2;
						flag3 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
					}
					if (!flag3)
					{
						BaseControllerVM currentGamepad4 = App.GamepadService.CurrentGamepad;
						bool flag4;
						if (currentGamepad4 == null)
						{
							flag4 = false;
						}
						else
						{
							ControllerVM currentController4 = currentGamepad4.CurrentController;
							ControllerTypeEnum? controllerTypeEnum = ((currentController4 != null) ? new ControllerTypeEnum?(currentController4.ControllerType) : null);
							ControllerTypeEnum controllerTypeEnum2 = 22;
							flag4 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
						}
						if (!flag4)
						{
							BaseControllerVM currentGamepad5 = App.GamepadService.CurrentGamepad;
							bool flag5;
							if (currentGamepad5 == null)
							{
								flag5 = false;
							}
							else
							{
								ControllerVM currentController5 = currentGamepad5.CurrentController;
								ControllerTypeEnum? controllerTypeEnum = ((currentController5 != null) ? new ControllerTypeEnum?(currentController5.ControllerType) : null);
								ControllerTypeEnum controllerTypeEnum2 = 3;
								flag5 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
							}
							if (!flag5)
							{
								BaseControllerVM currentGamepad6 = App.GamepadService.CurrentGamepad;
								if (currentGamepad6 == null)
								{
									return false;
								}
								ControllerVM currentController6 = currentGamepad6.CurrentController;
								ControllerTypeEnum? controllerTypeEnum = ((currentController6 != null) ? new ControllerTypeEnum?(currentController6.ControllerType) : null);
								ControllerTypeEnum controllerTypeEnum2 = 12;
								return (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
							}
						}
					}
				}
			}
			return true;
		}

		public bool AdvancedMappingFeatureNotRequiredForUnmap()
		{
			BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
			bool flag;
			if (currentGamepad == null)
			{
				flag = false;
			}
			else
			{
				ControllerVM currentController = currentGamepad.CurrentController;
				ControllerTypeEnum? controllerTypeEnum = ((currentController != null) ? new ControllerTypeEnum?(currentController.ControllerType) : null);
				ControllerTypeEnum controllerTypeEnum2 = 16;
				flag = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
			}
			if (!flag)
			{
				BaseControllerVM currentGamepad2 = App.GamepadService.CurrentGamepad;
				bool flag2;
				if (currentGamepad2 == null)
				{
					flag2 = false;
				}
				else
				{
					ControllerVM currentController2 = currentGamepad2.CurrentController;
					ControllerTypeEnum? controllerTypeEnum = ((currentController2 != null) ? new ControllerTypeEnum?(currentController2.ControllerType) : null);
					ControllerTypeEnum controllerTypeEnum2 = 63;
					flag2 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
				}
				if (!flag2)
				{
					BaseControllerVM currentGamepad3 = App.GamepadService.CurrentGamepad;
					bool flag3;
					if (currentGamepad3 == null)
					{
						flag3 = false;
					}
					else
					{
						ControllerVM currentController3 = currentGamepad3.CurrentController;
						ControllerTypeEnum? controllerTypeEnum = ((currentController3 != null) ? new ControllerTypeEnum?(currentController3.ControllerType) : null);
						ControllerTypeEnum controllerTypeEnum2 = 19;
						flag3 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
					}
					if (!flag3)
					{
						BaseControllerVM currentGamepad4 = App.GamepadService.CurrentGamepad;
						bool flag4;
						if (currentGamepad4 == null)
						{
							flag4 = false;
						}
						else
						{
							ControllerVM currentController4 = currentGamepad4.CurrentController;
							ControllerTypeEnum? controllerTypeEnum = ((currentController4 != null) ? new ControllerTypeEnum?(currentController4.ControllerType) : null);
							ControllerTypeEnum controllerTypeEnum2 = 28;
							flag4 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
						}
						if (!flag4)
						{
							BaseControllerVM currentGamepad5 = App.GamepadService.CurrentGamepad;
							bool flag5;
							if (currentGamepad5 == null)
							{
								flag5 = false;
							}
							else
							{
								ControllerVM currentController5 = currentGamepad5.CurrentController;
								ControllerTypeEnum? controllerTypeEnum = ((currentController5 != null) ? new ControllerTypeEnum?(currentController5.ControllerType) : null);
								ControllerTypeEnum controllerTypeEnum2 = 54;
								flag5 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
							}
							if (!flag5)
							{
								BaseControllerVM currentGamepad6 = App.GamepadService.CurrentGamepad;
								bool flag6;
								if (currentGamepad6 == null)
								{
									flag6 = false;
								}
								else
								{
									ControllerVM currentController6 = currentGamepad6.CurrentController;
									ControllerTypeEnum? controllerTypeEnum = ((currentController6 != null) ? new ControllerTypeEnum?(currentController6.ControllerType) : null);
									ControllerTypeEnum controllerTypeEnum2 = 37;
									flag6 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
								}
								if (!flag6)
								{
									BaseControllerVM currentGamepad7 = App.GamepadService.CurrentGamepad;
									bool flag7;
									if (currentGamepad7 == null)
									{
										flag7 = false;
									}
									else
									{
										ControllerVM currentController7 = currentGamepad7.CurrentController;
										ControllerTypeEnum? controllerTypeEnum = ((currentController7 != null) ? new ControllerTypeEnum?(currentController7.ControllerType) : null);
										ControllerTypeEnum controllerTypeEnum2 = 38;
										flag7 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
									}
									if (!flag7)
									{
										BaseControllerVM currentGamepad8 = App.GamepadService.CurrentGamepad;
										bool flag8;
										if (currentGamepad8 == null)
										{
											flag8 = false;
										}
										else
										{
											ControllerVM currentController8 = currentGamepad8.CurrentController;
											ControllerTypeEnum? controllerTypeEnum = ((currentController8 != null) ? new ControllerTypeEnum?(currentController8.ControllerType) : null);
											ControllerTypeEnum controllerTypeEnum2 = 39;
											flag8 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
										}
										if (!flag8)
										{
											BaseControllerVM currentGamepad9 = App.GamepadService.CurrentGamepad;
											bool flag9;
											if (currentGamepad9 == null)
											{
												flag9 = false;
											}
											else
											{
												ControllerVM currentController9 = currentGamepad9.CurrentController;
												ControllerTypeEnum? controllerTypeEnum = ((currentController9 != null) ? new ControllerTypeEnum?(currentController9.ControllerType) : null);
												ControllerTypeEnum controllerTypeEnum2 = 40;
												flag9 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
											}
											if (!flag9)
											{
												BaseControllerVM currentGamepad10 = App.GamepadService.CurrentGamepad;
												if (currentGamepad10 == null)
												{
													return false;
												}
												ControllerVM currentController10 = currentGamepad10.CurrentController;
												ControllerTypeEnum? controllerTypeEnum = ((currentController10 != null) ? new ControllerTypeEnum?(currentController10.ControllerType) : null);
												ControllerTypeEnum controllerTypeEnum2 = 62;
												return (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return true;
		}

		public DelegateCommand EditControllerNameCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._editControllerName) == null)
				{
					delegateCommand = (this._editControllerName = new DelegateCommand(new Action(this.EditControllerName)));
				}
				return delegateCommand;
			}
		}

		private void EditControllerName()
		{
			this._controllerForEditing = base.GamepadService.CurrentGamepad;
			this._controllerNameBeforeEdit = this._controllerForEditing.ControllerFriendlyName;
			this.IsFriendlyNameEditVisible = true;
		}

		public DelegateCommand RequestSupportCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._RequestSupportCommand) == null)
				{
					delegateCommand = (this._RequestSupportCommand = new DelegateCommand(new Action(this.RequestSupport)));
				}
				return delegateCommand;
			}
		}

		private void RequestSupport()
		{
			BaseControllerVM currentGamepad = base.GamepadService.CurrentGamepad;
			DSUtils.GoUrl(XBUtils.GetControllerRequestLink((currentGamepad != null) ? currentGamepad.CurrentController : null));
		}

		public DelegateCommand ShowMouseSettingsCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._showMouseSettings) == null)
				{
					delegateCommand = (this._showMouseSettings = new DelegateCommand(new Action(this.ShowMouseSettings)));
				}
				return delegateCommand;
			}
		}

		private void ShowMouseSettings()
		{
			BaseXBBindingCollection realCurrentBeingMappedBindingCollection = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
			if (realCurrentBeingMappedBindingCollection != null)
			{
				realCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(null);
			}
			reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(MouseSettings));
		}

		public DelegateCommand VibrateCommand
		{
			get
			{
				if (this._VibrateCommand == null)
				{
					this._VibrateCommand = new DelegateCommand(new Action(this.Vibrate), new Func<bool>(this.VibrateCommandCanExecute));
					App.EventAggregator.GetEvent<CurrentGamepadChanged>().Subscribe(delegate(BaseControllerVM i)
					{
						this._VibrateCommand.RaiseCanExecuteChanged();
					});
				}
				return this._VibrateCommand;
			}
		}

		private bool VibrateCommandCanExecute()
		{
			BaseControllerVM currentGamepad = base.GamepadService.CurrentGamepad;
			if (currentGamepad != null && !currentGamepad.HasExclusiveCaptureControllers)
			{
				BaseControllerVM currentGamepad2 = base.GamepadService.CurrentGamepad;
				return currentGamepad2 != null && currentGamepad2.IsMotorRumbleMotorPresent;
			}
			return false;
		}

		private void Vibrate()
		{
			BaseControllerVM currentGamepad = base.GamepadService.CurrentGamepad;
			if (currentGamepad == null)
			{
				return;
			}
			currentGamepad.VibrateForce();
		}

		public DelegateCommand<bool?> ShowVibrationSettingsCommand
		{
			get
			{
				if (this._showVibrationSettings == null)
				{
					this._showVibrationSettings = new DelegateCommand<bool?>(new Action<bool?>(this.ShowVibrationSettings), new Func<bool?, bool>(this.ShowGamepadCanExecute));
					base.GameProfilesService.CurrentGameChanged += delegate(object o, PropertyChangedExtendedEventArgs<GameVM> args)
					{
						this._showVibrationSettings.RaiseCanExecuteChanged();
					};
					base.GameProfilesService.CurrentGameProfileChanged += delegate(object o, PropertyChangedExtendedEventArgs<ConfigVM> args)
					{
						this._showVibrationSettings.RaiseCanExecuteChanged();
					};
					App.EventAggregator.GetEvent<CurrentShiftBindingCollectionChanged>().Subscribe(delegate(ShiftXBBindingCollection i)
					{
						this._showVibrationSettings.RaiseCanExecuteChanged();
					});
				}
				return this._showVibrationSettings;
			}
		}

		private void ShowVibrationSettings(bool? val)
		{
			BaseXBBindingCollection realCurrentBeingMappedBindingCollection = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
			if (realCurrentBeingMappedBindingCollection != null)
			{
				realCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(null);
			}
			reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(reWASDUI.Views.ContentZoneGamepad.VibrationSettings));
		}

		public DelegateCommand<bool?> ShowGamepadCommand
		{
			get
			{
				if (this._showGamepad == null)
				{
					this._showGamepad = new DelegateCommand<bool?>(new Action<bool?>(this.ShowGamepad), new Func<bool?, bool>(this.ShowGamepadCanExecute));
					base.GameProfilesService.CurrentGameChanged += delegate(object o, PropertyChangedExtendedEventArgs<GameVM> args)
					{
						this._showGamepad.RaiseCanExecuteChanged();
					};
					base.GameProfilesService.CurrentGameProfileChanged += delegate(object o, PropertyChangedExtendedEventArgs<ConfigVM> args)
					{
						this._showGamepad.RaiseCanExecuteChanged();
					};
					App.EventAggregator.GetEvent<CurrentShiftBindingCollectionChanged>().Subscribe(delegate(ShiftXBBindingCollection i)
					{
						this._showGamepad.RaiseCanExecuteChanged();
						this.OnPropertyChanged("VirtualControllerStateHint");
					});
					App.EventAggregator.GetEvent<PubSubEvent<LicenseChangedEvent>>().Subscribe(delegate(LicenseChangedEvent i)
					{
						this._showGamepad.RaiseCanExecuteChanged();
					});
				}
				return this._showGamepad;
			}
		}

		private void ShowGamepad(bool? showBack = null)
		{
			base.GuiHelperService.BindingFrameBackCommand.Execute(null);
			if (showBack != null)
			{
				base.GamepadService.CurrentGamepadFlipCommand.Execute(new bool?(showBack.Value));
			}
			reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(SVGGamepadWithAllAnnotations));
		}

		private bool ShowGamepadCanExecute(bool? val)
		{
			this.IsAdvancedMappingFeatureNotRequired = this.AdvancedMappingFeatureNotRequired();
			this.IsAdvancedMappingFeatureNotRequiredForUnmap = this.AdvancedMappingFeatureNotRequiredForUnmap();
			this.IsAdvancedMappingFeatureUnlocked = App.LicensingService.IsAdvancedMappingFeatureUnlocked;
			return base.GameProfilesService.RealCurrentBeingMappedBindingCollection != null && base.GameProfilesService.RealCurrentBeingMappedBindingCollection.SubConfigData.IsGamepad;
		}

		public ICommand ShowKeyboardCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._showKeyboard) == null)
				{
					relayCommand = (this._showKeyboard = new RelayCommand(new Action(this.ShowKeyboard), new Func<bool>(this.ShowKeyboardCanExecute)));
				}
				return relayCommand;
			}
		}

		private void ShowKeyboard()
		{
			reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(KeyboardMappingView));
		}

		private bool ShowKeyboardCanExecute()
		{
			BaseXBBindingCollection realCurrentBeingMappedBindingCollection = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
			return realCurrentBeingMappedBindingCollection != null && realCurrentBeingMappedBindingCollection.SubConfigData.IsKeyboard;
		}

		public ICommand ShowMouseCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._showMouse) == null)
				{
					relayCommand = (this._showMouse = new RelayCommand(new Action(this.ShowMouse), new Func<bool>(this.ShowMouseCanExecute)));
				}
				return relayCommand;
			}
		}

		private void ShowMouse()
		{
			reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(MouseMappingView));
		}

		private bool ShowMouseCanExecute()
		{
			BaseXBBindingCollection realCurrentBeingMappedBindingCollection = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
			return realCurrentBeingMappedBindingCollection != null && realCurrentBeingMappedBindingCollection.SubConfigData.IsMouse;
		}

		public ICommand ShowKeypadCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._showKeypad) == null)
				{
					relayCommand = (this._showKeypad = new RelayCommand(new Action(this.ShowKeypad), new Func<bool>(this.ShowKeypadCanExecute)));
				}
				return relayCommand;
			}
		}

		private void ShowKeypad()
		{
			BaseXBBindingCollection realCurrentBeingMappedBindingCollection = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
			if (realCurrentBeingMappedBindingCollection != null)
			{
				realCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(null);
			}
			base.GameProfilesService.SetCurrentBindingsToKeyboard();
			reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(KeyboardMappingView));
		}

		private bool ShowKeypadCanExecute()
		{
			if (base.GamepadService.CurrentGamepad != null && (base.GamepadService.CurrentGamepad.ControllerFamily == 2 || base.GamepadService.CurrentGamepad.ControllerFamily == 3))
			{
				if (base.GamepadService.CurrentGamepad.IsCompositeDevice)
				{
					BaseCompositeControllerVM baseCompositeControllerVM = base.GamepadService.CurrentGamepad as CompositeControllerVM;
					int num = 0;
					int num2 = 0;
					int num3 = 0;
					if (base.GameProfilesService.RealCurrentBeingMappedBindingCollection != null)
					{
						num3 = base.GameProfilesService.RealCurrentBeingMappedBindingCollection.SubConfigData.Index;
					}
					using (List<BaseControllerVM>.Enumerator enumerator = baseCompositeControllerVM.BaseControllers.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							BaseControllerVM baseControllerVM = enumerator.Current;
							if (baseControllerVM != null && ControllerFamilyExtensions.IsMouse(baseControllerVM.ControllerFamily))
							{
								if (num < num3)
								{
									num++;
									continue;
								}
								if (num3 == num)
								{
									BaseXBBindingCollection realCurrentBeingMappedBindingCollection = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
									if (realCurrentBeingMappedBindingCollection != null && realCurrentBeingMappedBindingCollection.SubConfigData.IsMouse)
									{
										return baseControllerVM.HasMouseControllersWithAnyKeypad;
									}
								}
							}
							if (baseControllerVM != null && ControllerFamilyExtensions.IsGamepad(baseControllerVM.ControllerFamily))
							{
								if (num2 < num3)
								{
									num2++;
								}
								else if (num3 == num2)
								{
									BaseXBBindingCollection realCurrentBeingMappedBindingCollection2 = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
									if (realCurrentBeingMappedBindingCollection2 != null && realCurrentBeingMappedBindingCollection2.SubConfigData.IsGamepad)
									{
										return baseControllerVM.HasGamepadControllersWithBuiltInAnyKeypad;
									}
								}
							}
						}
						return false;
					}
				}
				if (base.GameProfilesService.RealCurrentBeingMappedBindingCollection != null)
				{
					BaseXBBindingCollection realCurrentBeingMappedBindingCollection3 = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
					if (realCurrentBeingMappedBindingCollection3 == null || !realCurrentBeingMappedBindingCollection3.SubConfigData.IsMouse)
					{
						if (base.GameProfilesService.RealCurrentBeingMappedBindingCollection != null)
						{
							BaseXBBindingCollection realCurrentBeingMappedBindingCollection4 = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
							if (realCurrentBeingMappedBindingCollection4 == null || !realCurrentBeingMappedBindingCollection4.SubConfigData.IsGamepad)
							{
								return false;
							}
						}
						return base.GamepadService.CurrentGamepad.HasGamepadControllersWithBuiltInAnyKeypad;
					}
				}
				return base.GamepadService.CurrentGamepad.HasMouseControllersWithAnyKeypad;
			}
			BaseXBBindingCollection realCurrentBeingMappedBindingCollection5 = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
			if (realCurrentBeingMappedBindingCollection5 != null && realCurrentBeingMappedBindingCollection5.SubConfigData.IsMouse)
			{
				return true;
			}
			if (base.GamepadService.CurrentGamepad != null && base.GamepadService.CurrentGamepad.ControllerFamily == null)
			{
				BaseXBBindingCollection realCurrentBeingMappedBindingCollection6 = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
				if (realCurrentBeingMappedBindingCollection6 != null && realCurrentBeingMappedBindingCollection6.SubConfigData.IsGamepad)
				{
					return base.GamepadService.CurrentGamepad.HasGamepadControllersWithBuiltInAnyKeypad;
				}
			}
			return false;
		}

		public ICommand ShowGamepadKeypadCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._showGamepadKeypad) == null)
				{
					relayCommand = (this._showGamepadKeypad = new RelayCommand(new Action(this.ShowGamepadKeypad), new Func<bool>(this.ShowGamepadKeypadCanExecute)));
				}
				return relayCommand;
			}
		}

		private void ShowGamepadKeypad()
		{
			BaseXBBindingCollection realCurrentBeingMappedBindingCollection = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
			if (realCurrentBeingMappedBindingCollection != null)
			{
				realCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(null);
			}
			base.GameProfilesService.SetCurrentBindingsToKeyboard();
			reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(KeyboardMappingView));
		}

		private bool ShowGamepadKeypadCanExecute()
		{
			if (base.GamepadService.CurrentGamepad == null || (base.GamepadService.CurrentGamepad.ControllerFamily != 2 && base.GamepadService.CurrentGamepad.ControllerFamily != 3))
			{
				if (base.GamepadService.CurrentGamepad != null && base.GamepadService.CurrentGamepad.ControllerFamily == null)
				{
					BaseXBBindingCollection realCurrentBeingMappedBindingCollection = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
					if (realCurrentBeingMappedBindingCollection != null && realCurrentBeingMappedBindingCollection.SubConfigData.IsGamepad)
					{
						return base.GamepadService.CurrentGamepad.HasGamepadControllersWithBuiltInAnyKeypad;
					}
				}
				return false;
			}
			if (base.GamepadService.CurrentGamepad.IsCompositeDevice)
			{
				BaseCompositeControllerVM baseCompositeControllerVM = base.GamepadService.CurrentGamepad as CompositeControllerVM;
				int num = 0;
				int num2 = 0;
				if (base.GameProfilesService.RealCurrentBeingMappedBindingCollection != null)
				{
					num2 = base.GameProfilesService.RealCurrentBeingMappedBindingCollection.SubConfigData.Index;
				}
				using (List<BaseControllerVM>.Enumerator enumerator = baseCompositeControllerVM.BaseControllers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BaseControllerVM baseControllerVM = enumerator.Current;
						if (baseControllerVM != null && ControllerFamilyExtensions.IsGamepad(baseControllerVM.ControllerFamily))
						{
							if (num < num2)
							{
								num++;
							}
							else if (num2 == num)
							{
								BaseXBBindingCollection realCurrentBeingMappedBindingCollection2 = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
								if (realCurrentBeingMappedBindingCollection2 != null && realCurrentBeingMappedBindingCollection2.SubConfigData.IsGamepad)
								{
									return baseControllerVM.HasGamepadControllersWithBuiltInAnyKeypad;
								}
							}
						}
					}
					return false;
				}
			}
			if (base.GameProfilesService.RealCurrentBeingMappedBindingCollection != null)
			{
				BaseXBBindingCollection realCurrentBeingMappedBindingCollection3 = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
				if (realCurrentBeingMappedBindingCollection3 == null || !realCurrentBeingMappedBindingCollection3.SubConfigData.IsGamepad)
				{
					return false;
				}
			}
			return base.GamepadService.CurrentGamepad.HasGamepadControllersWithBuiltInAnyKeypad;
		}

		public DelegateCommand ShowAdvancedStickSettingsCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._showAdvancedSettings) == null)
				{
					delegateCommand = (this._showAdvancedSettings = new DelegateCommand(new Action(this.ShowAdvancedSettings)));
				}
				return delegateCommand;
			}
		}

		private void ShowAdvancedSettings()
		{
			BaseXBBindingCollection realCurrentBeingMappedBindingCollection = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
			if (realCurrentBeingMappedBindingCollection != null && realCurrentBeingMappedBindingCollection.CurrentBindingIsLeftStick)
			{
				base.GameProfilesService.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(37), true);
			}
			else
			{
				BaseXBBindingCollection realCurrentBeingMappedBindingCollection2 = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
				if (realCurrentBeingMappedBindingCollection2 != null && realCurrentBeingMappedBindingCollection2.CurrentBindingIsRightStick)
				{
					base.GameProfilesService.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(44), true);
				}
				else
				{
					BaseXBBindingCollection realCurrentBeingMappedBindingCollection3 = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
					if (realCurrentBeingMappedBindingCollection3 != null && realCurrentBeingMappedBindingCollection3.CurrentBindingIsGyroTilt)
					{
						base.GameProfilesService.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(74), true);
					}
				}
			}
			reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(AdvancedStickSettings));
			App.BindingFrameRegionManagers.ForEach(delegate(SingleRegionManager bfrm)
			{
				bfrm.NavigateToDefaultView();
			});
			BaseXBBindingCollection realCurrentBeingMappedBindingCollection4 = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
			if (realCurrentBeingMappedBindingCollection4 == null)
			{
				return;
			}
			BaseDirectionalGroup currentBoundGroup = realCurrentBeingMappedBindingCollection4.CurrentBoundGroup;
			if (currentBoundGroup == null)
			{
				return;
			}
			currentBoundGroup.UpdateProperties();
		}

		public DelegateCommand<XBBinding> ShowMacroSettingsCommand
		{
			get
			{
				DelegateCommand<XBBinding> delegateCommand;
				if ((delegateCommand = this._showMacroSettings) == null)
				{
					delegateCommand = (this._showMacroSettings = new DelegateCommand<XBBinding>(new Action<XBBinding>(this.ShowMacroSettings)));
				}
				return delegateCommand;
			}
		}

		private void ShowMacroSettings(XBBinding xbBinding)
		{
			reWASDApplicationCommands.ShowMacroSettings(xbBinding);
		}

		public DelegateCommand ShowZonesCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._showZones) == null)
				{
					delegateCommand = (this._showZones = new DelegateCommand(new Action(this.ShowZones)));
				}
				return delegateCommand;
			}
		}

		private void ShowZones()
		{
			BaseXBBindingCollection realCurrentBeingMappedBindingCollection = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
			if (realCurrentBeingMappedBindingCollection == null)
			{
				return;
			}
			if (realCurrentBeingMappedBindingCollection.CurrentBindingIsPhysicalTrackPad1)
			{
				realCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(236), true);
			}
			if (realCurrentBeingMappedBindingCollection.CurrentBindingIsPhysicalTrackPad2)
			{
				realCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(239), true);
			}
			if (realCurrentBeingMappedBindingCollection.CurrentBindingIsLeftTrigger)
			{
				realCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(52), true);
			}
			if (realCurrentBeingMappedBindingCollection.CurrentBindingIsRightTrigger)
			{
				realCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(56), true);
			}
			if (realCurrentBeingMappedBindingCollection.CurrentBindingIsLeftDS3AnalogButton)
			{
				if (realCurrentBeingMappedBindingCollection.CurrentBindingIsDS3DPADUpAnalogButton)
				{
					realCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(201), true);
				}
				else if (realCurrentBeingMappedBindingCollection.CurrentBindingIsDS3DPADDownAnalogButton)
				{
					realCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(204), true);
				}
				else if (realCurrentBeingMappedBindingCollection.CurrentBindingIsDS3DPADLeftAnalogButton)
				{
					realCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(207), true);
				}
				else if (realCurrentBeingMappedBindingCollection.CurrentBindingIsDS3DPADRightAnalogButton)
				{
					realCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(210), true);
				}
				else
				{
					realCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(183), true);
				}
			}
			if (realCurrentBeingMappedBindingCollection.CurrentBindingIsRightDS3AnalogButton)
			{
				if (realCurrentBeingMappedBindingCollection.CurrentBindingIsDS3CrossAnalogButton)
				{
					realCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(189), true);
				}
				else if (realCurrentBeingMappedBindingCollection.CurrentBindingIsDS3CircleAnalogButton)
				{
					realCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(195), true);
				}
				else if (realCurrentBeingMappedBindingCollection.CurrentBindingIsDS3SquareAnalogButton)
				{
					realCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(198), true);
				}
				else if (realCurrentBeingMappedBindingCollection.CurrentBindingIsDS3TriangleAnalogButton)
				{
					realCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(192), true);
				}
				else
				{
					realCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(186), true);
				}
			}
			reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(SVGGamepadWithAllAnnotations));
			reWASDApplicationCommands.NavigateBindingFrameCommand.Execute(typeof(BFAdvanced));
		}

		public DelegateCommand ShowBlacklistPreferencesCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._showBlacklistPreferencesCommand) == null)
				{
					delegateCommand = (this._showBlacklistPreferencesCommand = new DelegateCommand(new Action(this.ShowBlacklistPreferences)));
				}
				return delegateCommand;
			}
		}

		private void ShowBlacklistPreferences()
		{
			PreferencesWindow.ShowPreferences(typeof(PreferencesBlacklistVM));
		}

		public DelegateCommand OpenMobileConnectionGuideCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._openMobileConnectionGuideCommand) == null)
				{
					delegateCommand = (this._openMobileConnectionGuideCommand = new DelegateCommand(new Action(this.OpenMobileConnectionGuide)));
				}
				return delegateCommand;
			}
		}

		private static string GetNetworkIpList(long minimumSpeed)
		{
			if (!NetworkInterface.GetIsNetworkAvailable())
			{
				return null;
			}
			int port = HttpServerSettings.GetPort();
			string text = "";
			foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (networkInterface.OperationalStatus == OperationalStatus.Up && networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback && networkInterface.NetworkInterfaceType != NetworkInterfaceType.Tunnel && networkInterface.Speed >= minimumSpeed && networkInterface.Description.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) < 0 && networkInterface.Name.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) < 0 && !networkInterface.Description.Equals("Microsoft Loopback Adapter", StringComparison.OrdinalIgnoreCase) && (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet))
				{
					foreach (UnicastIPAddressInformation unicastIPAddressInformation in networkInterface.GetIPProperties().UnicastAddresses)
					{
						if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork && !unicastIPAddressInformation.Address.ToString().StartsWith("169.254."))
						{
							string text2 = text;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 2);
							defaultInterpolatedStringHandler.AppendLiteral("http://");
							defaultInterpolatedStringHandler.AppendFormatted<IPAddress>(unicastIPAddressInformation.Address);
							defaultInterpolatedStringHandler.AppendLiteral(":");
							defaultInterpolatedStringHandler.AppendFormatted<int>(port);
							text = text2 + defaultInterpolatedStringHandler.ToStringAndClear() + "\n";
						}
					}
				}
			}
			return text;
		}

		public void OpenMobileConnectionGuide()
		{
			Bitmap bitmap = null;
			try
			{
				QRCodeGenerator qrcodeGenerator = new QRCodeGenerator();
				string networkIpList = GamepadSelectorVM.GetNetworkIpList(1000L);
				if (string.IsNullOrWhiteSpace(networkIpList))
				{
					Tracer.TraceWrite("Failed to get interface ip. QR code is invalid", false);
				}
				bitmap = new QRCode(qrcodeGenerator.CreateQrCode(networkIpList, 2, false, false, 0, -1)).GetGraphic(20);
			}
			catch (Exception)
			{
			}
			new MobileAppConnGuideDialog(bitmap)
			{
				Owner = App.m_MainWnd
			}.ShowDialog();
		}

		public DelegateCommand ShowLEDPreferencesCommand
		{
			get
			{
				if (this._showLEDPreferencesCommand == null)
				{
					this._showLEDPreferencesCommand = new DelegateCommand(new Action(this.ShowLEDPreferences), new Func<bool>(this.ShowLEDCanExecute));
					base.EventAggregator.GetEvent<CurrentGamepadChanged>().Subscribe(delegate(BaseControllerVM o)
					{
						this._showLEDPreferencesCommand.RaiseCanExecuteChanged();
					});
				}
				return this._showLEDPreferencesCommand;
			}
		}

		private void ShowLEDPreferences()
		{
			PreferencesWindow.ShowPreferences(typeof(PreferencesLEDSettingsVM));
		}

		private bool ShowLEDCanExecute()
		{
			ControllerVM controllerVM = App.GamepadService.CurrentGamepad as ControllerVM;
			if (controllerVM != null)
			{
				return ControllerTypeExtensions.ConvertEnumToLEDSupportedType(controllerVM.ControllerType) != null && ControllerTypeExtensions.IsGamepad(controllerVM.ControllerType);
			}
			CompositeControllerVM compositeControllerVM = App.GamepadService.CurrentGamepad as CompositeControllerVM;
			return compositeControllerVM != null && compositeControllerVM.IsNintendoSwitchJoyConComposite;
		}

		public DelegateCommand ShowSlotHotkeysPreferencesCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._showSlotHotkeysPreferencesCommand) == null)
				{
					delegateCommand = (this._showSlotHotkeysPreferencesCommand = new DelegateCommand(new Action(this.ShowSlotHotkeysPreferences)));
				}
				return delegateCommand;
			}
		}

		private void ShowSlotHotkeysPreferences()
		{
			PreferencesWindow.ShowPreferences(typeof(PreferencesSlotsChangePageVM));
		}

		public DelegateCommand ShowExternalDeviceSettingsCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._showExternalDeviceSettingsCommand) == null)
				{
					delegateCommand = (this._showExternalDeviceSettingsCommand = new DelegateCommand(new Action(this.ShowExternalDeviceSettings)));
				}
				return delegateCommand;
			}
		}

		private void ShowExternalDeviceSettings()
		{
			PreferencesWindow.ShowPreferences(typeof(PreferencesExternalDevicesVM));
		}

		public DelegateCommand ShowMacroHelpCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._ShowMacroHelp) == null)
				{
					delegateCommand = (this._ShowMacroHelp = new DelegateCommand(new Action(this.ShowMacroHelp)));
				}
				return delegateCommand;
			}
		}

		private void ShowMacroHelp()
		{
			DSUtils.GoUrl("https://www.rewasd.com/macro-controller/#howto");
		}

		public DelegateCommand StartStopGamepadsDetectionModeCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._startStopGamepadsDetectionModeCommand) == null)
				{
					delegateCommand = (this._startStopGamepadsDetectionModeCommand = new DelegateCommand(new Action(this.StartStopGamepadsDetectionMode)));
				}
				return delegateCommand;
			}
		}

		public void StartGamepadsDetectionMode()
		{
			if (MessageBoxWithDoNotShowLogic.Show(System.Windows.Application.Current.MainWindow, DTLocalization.GetString(11447), MessageBoxButton.OKCancel, MessageBoxImage.Asterisk, "ConfirmDetectActiveDevice", MessageBoxResult.OK, false, 0.0, null, null, null, null, null, null) != MessageBoxResult.OK)
			{
				return;
			}
			base.DeviceDetectionService.IsEnabled = true;
		}

		private void StartStopGamepadsDetectionMode()
		{
			if (base.DeviceDetectionService.IsEnabled)
			{
				base.DeviceDetectionService.IsEnabled = false;
				return;
			}
			this.StartGamepadsDetectionMode();
		}

		public GamepadSelectorVM(IEventAggregator ea, IContainerProvider uc, IGamepadService gs)
			: base(uc)
		{
			this._eventAggregator = ea;
			this._eventAggregator.GetEvent<CurrentGamepadChanged>().Subscribe(new Action<BaseControllerVM>(this.GamepadServiceOnCurrentGamepadChanged));
			this._eventAggregator.GetEvent<CurrentGamepadSlotChanged>().Subscribe(new Action<Slot>(this.GamepadServiceOnCurrentGamepadSlotChanged));
			this._eventAggregator.GetEvent<ConfigApplied>().Subscribe(new Action<ConfigAppliedEvent>(this.OnConfigApplied));
			this._eventAggregator.GetEvent<RemapStateChanged>().Subscribe(delegate(RemapStateChangedEvent remapStateChanged)
			{
				this.RefreshUdpState();
				this.OnPropertyChanged("VirtualControllerStateHint");
			});
			this._eventAggregator.GetEvent<ControllerStateChanged>().Subscribe(new Action<BaseControllerVM>(this.OnControllerStateChanged));
			PreferencesChanged @event = this._eventAggregator.GetEvent<PreferencesChanged>();
			if (@event != null)
			{
				@event.Subscribe(delegate(object remapStateChanged)
				{
					this.RefreshUdpState();
				});
			}
			CurrentBindingCollectionWrapperChanged event2 = this._eventAggregator.GetEvent<CurrentBindingCollectionWrapperChanged>();
			if (event2 != null)
			{
				event2.Subscribe(delegate(SubConfigData o)
				{
					CommandManager.InvalidateRequerySuggested();
				});
			}
			VirtualControllerTypeChanged event3 = this._eventAggregator.GetEvent<VirtualControllerTypeChanged>();
			if (event3 != null)
			{
				event3.Subscribe(delegate(ConfigData o)
				{
					this.OnPropertyChanged("VirtualControllerStateHint");
				});
			}
			base.GameProfilesService.CurrentGameChanged += delegate(object sender, PropertyChangedExtendedEventArgs<GameVM> args)
			{
				this.OnPropertyChanged("WillBeAppliedToSlotText");
			};
			base.GameProfilesService.CurrentGameProfileChanged += delegate(object sender, PropertyChangedExtendedEventArgs<ConfigVM> args)
			{
				this.OnPropertyChanged("WillBeAppliedToSlotText");
			};
			base.EventAggregator.GetEvent<CurrentGamepadChanged>().Subscribe(delegate(BaseControllerVM sender)
			{
				this.OnPropertyChanged("WillBeAppliedToSlotText");
			});
			base.GameProfilesService.OnAutodetectForAnySlotChanged += delegate(object s, PropertyChangedExtendedEventArgs a)
			{
				this.OnPropertyChanged("WillBeAppliedToSlotText");
			};
			base.GamepadService.GamepadCollection.CollectionChanged += delegate([Nullable(2)] object sender, NotifyCollectionChangedEventArgs args)
			{
				this.OnPropertyChanged("IsMultipleControllers");
			};
			App.LicensingService.OnLicenseChangedCompleted += delegate(LicenseCheckResult s, bool e)
			{
				this.IsAdvancedMappingFeatureNotRequired = this.AdvancedMappingFeatureNotRequired();
				this.IsAdvancedMappingFeatureNotRequiredForUnmap = this.AdvancedMappingFeatureNotRequiredForUnmap();
				this.IsAdvancedMappingFeatureUnlocked = App.LicensingService.IsAdvancedMappingFeatureUnlocked;
			};
			TranslationManager.Instance.LanguageChanged += delegate([Nullable(2)] object s, EventArgs e)
			{
				this.OnPropertyChanged("VirtualControllerStateHint");
			};
			this.ExternalDeviceCollection = new ExternalDevicesCollection(App.GamepadService.ExternalDevices);
			if (base.GamepadService.CurrentGamepad != null)
			{
				this.GamepadServiceOnCurrentGamepadChanged(null);
			}
		}

		public RelayCommand<Slot> RestoreDefaultsCommand
		{
			get
			{
				RelayCommand<Slot> relayCommand;
				if ((relayCommand = this._restoreDefaults) == null)
				{
					relayCommand = (this._restoreDefaults = new RelayCommand<Slot>(new Action<Slot>(this.RestoreDefaults)));
				}
				return relayCommand;
			}
		}

		private async void RestoreDefaults(Slot slot)
		{
			await base.GamepadService.RestoreDefaults(new List<Slot> { slot });
			await base.GamepadService.BinDataSerialize.LoadGamepadProfileRelations();
			await this.SetCurrentGameAndProfileByCurentGamepadActiveSlot();
		}

		public RelayCommand<Slot> ChangeGamepadSlotCommand
		{
			get
			{
				RelayCommand<Slot> relayCommand;
				if ((relayCommand = this._changeGamepadSlot) == null)
				{
					relayCommand = (this._changeGamepadSlot = new RelayCommand<Slot>(new Action<Slot>(this.ChangeGamepadSlot)));
				}
				return relayCommand;
			}
		}

		private void ChangeGamepadSlot(Slot slot)
		{
			base.GamepadService.SwitchProfileToSlot(slot);
		}

		public ICommand SelectSlotCommand
		{
			get
			{
				RelayCommand<Slot> relayCommand;
				if ((relayCommand = this._selectSlot) == null)
				{
					relayCommand = (this._selectSlot = new RelayCommand<Slot>(new Action<Slot>(this.SelectSlot)));
				}
				return relayCommand;
			}
		}

		private void SelectSlot(Slot slotInfo)
		{
			foreach (SlotInfo slotInfo2 in base.GamepadService.SlotsInfo)
			{
				if (slotInfo2.Slot == slotInfo)
				{
					this.CurrentSlotInfo = slotInfo2;
				}
			}
		}

		public SlotInfo CurrentSlotInfo
		{
			get
			{
				return this._currentSlotInfo;
			}
			set
			{
				if (!value.IsAvailable)
				{
					Dictionary<object, object> dictionary = new Dictionary<object, object>();
					dictionary.Add("navigatePath", typeof(LicenseMain).ToString());
					NavigationParameters navigationParameters = new NavigationParameters();
					navigationParameters.Add("tab", "four-slots");
					dictionary.Add("NavigationParameters", navigationParameters);
					reWASDApplicationCommands.NavigateContentCommand.Execute(dictionary);
					return;
				}
				if (this.SetProperty<SlotInfo>(ref this._currentSlotInfo, value, "CurrentSlotInfo"))
				{
					this.OnCurrentSlotInfoChanged();
				}
			}
		}

		private async void OnCurrentSlotInfoChanged()
		{
			await base.GamepadService.SwitchProfileToSlot(this.CurrentSlotInfo.Slot);
			await this.SetCurrentGameAndProfileByCurentGamepadActiveSlot();
			IEventAggregator eventAggregator = App.EventAggregator;
			if (eventAggregator != null)
			{
				eventAggregator.GetEvent<CurrentGamepadSlotChanged>().Publish(this.CurrentSlotInfo.Slot);
			}
		}

		private void SetCurrentSlot(SlotInfo slot)
		{
			if (slot != null)
			{
				this._currentSlotInfo = slot;
				this.OnPropertyChanged("CurrentSlotInfo");
				if (this.CurrentSlotInfo != null)
				{
					IEventAggregator eventAggregator = App.EventAggregator;
					if (eventAggregator == null)
					{
						return;
					}
					eventAggregator.GetEvent<CurrentGamepadSlotChanged>().Publish(this.CurrentSlotInfo.Slot);
				}
			}
		}

		private void OnControllerStateChanged(BaseControllerVM controller)
		{
			if (controller.IsOnline && controller.RemapState != null && base.GameProfilesService.CurrentGame == null)
			{
				base.GamepadService.CurrentGamepad = controller;
			}
		}

		private async void OnConfigApplied(ConfigAppliedEvent configInfo)
		{
			if (base.GamepadService.GamepadCollection != null)
			{
				await base.GamepadService.BinDataSerialize.LoadGamepadProfileRelations();
				BaseControllerVM controller = base.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM item) => item.ID == configInfo.ControllerId);
				if (controller != null && controller.IsOnline && base.GameProfilesService.CurrentGame == null)
				{
					await base.GamepadService.SetCurrentGamepad(controller);
				}
				Slot currentSlot = 0;
				if (base.GamepadService.CurrentGamepad != null)
				{
					currentSlot = await base.GamepadService.CurrentGamepad.GetCurrentSlot();
				}
				this.SetCurrentSlot(base.GamepadService.SlotsInfo.FirstOrDefault((SlotInfo si) => si.Slot == currentSlot));
				ConfigVM config = base.GameProfilesService.FindConfig(configInfo.GameName, configInfo.ConfigName);
				if (config != null && controller == base.GamepadService.CurrentGamepad)
				{
					await config.ReadConfigFromJsonIfNotLoaded();
					await base.GameProfilesService.SetCurrentGameAndConfig(config, false);
				}
				await this.RefreshUdpState();
				this.OnPropertyChanged("VirtualControllerStateHint");
			}
		}

		public async Task SetCurrentGameAndProfileByCurentGamepadActiveSlot()
		{
			if (!XBUtils.BlockNavigateContent)
			{
				GamepadProfiles currentGamepadActiveProfiles = base.GamepadService.CurrentGamepadActiveProfiles;
				GamepadProfile gamepadProfile = ((currentGamepadActiveProfiles != null) ? currentGamepadActiveProfiles.SlotProfiles.TryGetValue(this.CurrentSlotInfo.Slot) : null);
				base.GamepadService.RefreshRemapStateProperties();
				if (gamepadProfile != null && base.GamepadService.CurrentGamepadRemappedState != null)
				{
					await base.GameProfilesService.SetCurrentGameAndConfig(gamepadProfile.ConfigPath);
					if (base.GamepadService.CurrentGamepad != null)
					{
						GameVM currentGame = base.GameProfilesService.CurrentGame;
						ControllerFamily? controllerFamily;
						if (currentGame == null)
						{
							controllerFamily = null;
						}
						else
						{
							ConfigVM currentConfig2 = currentGame.CurrentConfig;
							if (currentConfig2 == null)
							{
								controllerFamily = null;
							}
							else
							{
								SubConfigData currentSubConfigData = currentConfig2.CurrentSubConfigData;
								controllerFamily = ((currentSubConfigData != null) ? new ControllerFamily?(currentSubConfigData.ControllerFamily) : null);
							}
						}
						ControllerFamily? controllerFamily2 = controllerFamily;
						ControllerVM currentController = base.GamepadService.CurrentGamepad.CurrentController;
						ControllerFamily? controllerFamily3 = ((currentController != null) ? new ControllerFamily?(currentController.ControllerFamily) : null);
						if (!((controllerFamily2.GetValueOrDefault() == controllerFamily3.GetValueOrDefault()) & (controllerFamily2 != null == (controllerFamily3 != null))))
						{
							GameVM currentGame2 = base.GameProfilesService.CurrentGame;
							if (currentGame2 != null)
							{
								ConfigVM currentConfig3 = currentGame2.CurrentConfig;
								if (currentConfig3 != null)
								{
									currentConfig3.ChangeCurrentMainWrapperAccordingToControllerVM(base.GamepadService.CurrentGamepad, true);
								}
							}
						}
					}
				}
				else
				{
					GameVM currentGame3 = base.GameProfilesService.CurrentGame;
					ConfigVM currentConfig = ((currentGame3 != null) ? currentGame3.CurrentConfig : null);
					await base.GameProfilesService.SetCurrentGame(null, false);
					if (base.GamepadService.CurrentGamepad != null)
					{
						ConfigVM configVM = currentConfig;
						if (configVM != null)
						{
							configVM.ChangeCurrentMainWrapperAccordingToControllerVM(base.GamepadService.CurrentGamepad, true);
						}
					}
					BaseControllerVM currentGamepad = base.GamepadService.CurrentGamepad;
					XBUtils.NavigateGamepadZoneForControllerFamily((currentGamepad != null) ? new ControllerFamily?(currentGamepad.TreatAsControllerFamily) : null);
					currentConfig = null;
				}
			}
		}

		private void GamepadServiceOnCurrentGamepadSlotChanged(Slot slot)
		{
			GamepadSelectorVM.<>c__DisplayClass181_0 CS$<>8__locals1 = new GamepadSelectorVM.<>c__DisplayClass181_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.slot = slot;
			ThreadHelper.ExecuteInMainDispatcher(delegate
			{
				GamepadSelectorVM.<>c__DisplayClass181_0.<<GamepadServiceOnCurrentGamepadSlotChanged>b__0>d <<GamepadServiceOnCurrentGamepadSlotChanged>b__0>d;
				<<GamepadServiceOnCurrentGamepadSlotChanged>b__0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
				<<GamepadServiceOnCurrentGamepadSlotChanged>b__0>d.<>4__this = CS$<>8__locals1;
				<<GamepadServiceOnCurrentGamepadSlotChanged>b__0>d.<>1__state = -1;
				<<GamepadServiceOnCurrentGamepadSlotChanged>b__0>d.<>t__builder.Start<GamepadSelectorVM.<>c__DisplayClass181_0.<<GamepadServiceOnCurrentGamepadSlotChanged>b__0>d>(ref <<GamepadServiceOnCurrentGamepadSlotChanged>b__0>d);
			}, true);
		}

		private async void GamepadServiceOnCurrentGamepadChanged(object payload)
		{
			this.ResetGamadEditingUIState();
			if (base.GamepadService.CurrentGamepad != null)
			{
				GamepadSelectorVM.<>c__DisplayClass182_0 CS$<>8__locals1 = new GamepadSelectorVM.<>c__DisplayClass182_0();
				BaseControllerVM currentGamepad = base.GamepadService.CurrentGamepad;
				XBUtils.NavigateGamepadZoneForControllerFamily((currentGamepad != null) ? new ControllerFamily?(currentGamepad.TreatAsControllerFamily) : null);
				Slot slot = await base.GamepadService.CurrentGamepad.GetCurrentSlot();
				CS$<>8__locals1.currentSlot = slot;
				SlotInfo slotInfo = base.GamepadService.SlotsInfo.FirstOrDefault((SlotInfo si) => si.Slot == CS$<>8__locals1.currentSlot);
				if (slotInfo != null && !slotInfo.IsAvailable)
				{
					slotInfo = base.GamepadService.SlotsInfo[0];
				}
				this.SetCurrentSlot(slotInfo);
				await this.SetCurrentGameAndProfileByCurentGamepadActiveSlot();
				GameVM currentGame = base.GameProfilesService.CurrentGame;
				if (currentGame != null)
				{
					ConfigVM currentConfig = currentGame.CurrentConfig;
					if (currentConfig != null)
					{
						currentConfig.NavigateGamepadZoneAccordingToConfigOrSubconfigState();
					}
				}
				this.IsAdvancedMappingFeatureNotRequired = this.AdvancedMappingFeatureNotRequired();
				this.IsAdvancedMappingFeatureNotRequiredForUnmap = this.AdvancedMappingFeatureNotRequiredForUnmap();
				this.IsAdvancedMappingFeatureUnlocked = App.LicensingService.IsAdvancedMappingFeatureUnlocked;
				CS$<>8__locals1 = null;
			}
			else
			{
				XBUtils.NavigateGamepadZoneForControllerFamily(new ControllerFamily?(3));
			}
			BaseControllerVM currentGamepad2 = base.GamepadService.CurrentGamepad;
			if (currentGamepad2 != null)
			{
				currentGamepad2.ShowCompositeDevicesWindowCommand.RaiseCanExecuteChanged();
			}
			CommandManager.InvalidateRequerySuggested();
			await this.RefreshUdpState();
			this.OnPropertyChanged("VirtualControllerStateHint");
		}

		public void TBControllerNameApplyChanges()
		{
			GamepadService gamepadService = base.GamepadService;
			if (((gamepadService != null) ? gamepadService.CurrentGamepad : null) != null && this._controllerNameBeforeEdit != base.GamepadService.CurrentGamepad.ControllerFriendlyName)
			{
				BaseControllerVM controllerForEditing = this._controllerForEditing;
				GamepadService gamepadService2 = base.GamepadService;
				if (controllerForEditing == ((gamepadService2 != null) ? gamepadService2.CurrentGamepad : null))
				{
					base.GamepadService.CurrentGamepad.UpdateFriendlyName();
				}
			}
			this.ResetGamadEditingUIState();
		}

		public void TBControllerNameRevertChanges()
		{
			GamepadService gamepadService = base.GamepadService;
			if (((gamepadService != null) ? gamepadService.CurrentGamepad : null) != null)
			{
				base.GamepadService.CurrentGamepad.ControllerFriendlyName = this._controllerNameBeforeEdit;
			}
			this.ResetGamadEditingUIState();
		}

		private void ResetGamadEditingUIState()
		{
			this._controllerNameBeforeEdit = null;
			this._controllerForEditing = null;
			this.IsFriendlyNameEditVisible = false;
		}

		public ObservableCollection<ExternalDevice> ExternalDeviceCollection
		{
			get
			{
				ObservableCollection<ExternalDevice> externalDeviceCollection = this._externalDeviceCollection;
				if (externalDeviceCollection != null && externalDeviceCollection.Count > 1)
				{
					for (int i = 1; i < this._externalDeviceCollection.Count; i++)
					{
						if (this._externalDeviceCollection[i].SortIndex < this._externalDeviceCollection[i - 1].SortIndex)
						{
							this._externalDeviceCollection = new ObservableCollection<ExternalDevice>(this._externalDeviceCollection.OrderBy((ExternalDevice x) => x.SortIndex));
							break;
						}
					}
				}
				return this._externalDeviceCollection;
			}
			set
			{
				if (this._externalDeviceCollection == value)
				{
					return;
				}
				this.SetProperty<ObservableCollection<ExternalDevice>>(ref this._externalDeviceCollection, value, "ExternalDeviceCollection");
			}
		}

		public DelegateCommand ManageExternalDevicesCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._addExternalDeviceCommand) == null)
				{
					delegateCommand = (this._addExternalDeviceCommand = new DelegateCommand(new Action(this.ManageExternalDevices)));
				}
				return delegateCommand;
			}
		}

		private void ManageExternalDevices()
		{
			new Wizard(false).ShowDialog();
		}

		public DelegateCommand ExternalDeviceBluetoothReconnectCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._externalDeviceBluetoothReconnectCommand) == null)
				{
					delegateCommand = (this._externalDeviceBluetoothReconnectCommand = new DelegateCommand(new Action(this.ExternalDeviceBluetoothReconnect)));
				}
				return delegateCommand;
			}
		}

		private void ExternalDeviceBluetoothReconnect()
		{
			App.HttpClientService.ExternalDevices.ExternalDeviceBluetoothReconnect(base.GamepadService.CurrentGamepad.ID, this.CurrentSlotInfo.Slot);
		}

		public DelegateCommand ApplyAmiiboCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._applyAmiiboCommand) == null)
				{
					delegateCommand = (this._applyAmiiboCommand = new DelegateCommand(new Action(this.ApplyAmiibo)));
				}
				return delegateCommand;
			}
		}

		private void ApplyAmiibo()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = DTLocalization.GetString(11027) + " (*.bin) | *.bin";
			openFileDialog.CheckFileExists = true;
			openFileDialog.Multiselect = false;
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				try
				{
					FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
					if (!UtilsCommon.IsAmiiboValidSize(fileInfo.Length))
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(47, 1);
						defaultInterpolatedStringHandler.AppendLiteral("ApplyAmiibo: Error! Invalid Amiibo size: ");
						defaultInterpolatedStringHandler.AppendFormatted<long>(fileInfo.Length);
						defaultInterpolatedStringHandler.AppendLiteral(" bytes");
						Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
						DTMessageBox.Show(DTLocalization.GetString(12481), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
					}
					else
					{
						byte[] array = File.ReadAllBytes(openFileDialog.FileName);
						ApplyAmiiboInfo applyAmiiboInfo = new ApplyAmiiboInfo();
						GameVM currentGame = App.GameProfilesService.CurrentGame;
						applyAmiiboInfo.Path = ((((currentGame != null) ? currentGame.CurrentConfig : null) != null) ? App.GameProfilesService.CurrentGame.CurrentConfig.ConfigPath : "");
						applyAmiiboInfo.GamepadId = ((base.GamepadService.CurrentGamepad != null) ? base.GamepadService.CurrentGamepad.ID : "");
						applyAmiiboInfo.Slot = this.CurrentSlotInfo.Slot;
						applyAmiiboInfo.AmiiboData = array;
						ApplyAmiiboInfo applyAmiiboInfo2 = applyAmiiboInfo;
						App.HttpClientService.Gamepad.ApplyAmiibo(applyAmiiboInfo2);
					}
				}
				catch (Exception)
				{
				}
			}
		}

		private IEventAggregator _eventAggregator;

		private bool _isFriendlyNameEditVisible;

		private DelegateCommand _TriggerRemap;

		private DelegateCommand _applyProfile;

		private bool _isApplyingProfile;

		private bool _isUdpPresent;

		private bool _isUdpEnabledInPreferences;

		private bool _isUdpRunning;

		private bool _isUdpServerHasException;

		private string _udpRunningText;

		private bool _isUdpFull;

		private DelegateCommand _openUdpPreferencesCommand;

		private bool _isAdvancedMappingFeatureNotRequired;

		private bool _isAdvancedMappingFeatureNotRequiredForUnmap;

		private bool _isAdvancedMappingFeatureUnlocked;

		private string _controllerNameBeforeEdit;

		private BaseControllerVM _controllerForEditing;

		private DelegateCommand _editControllerName;

		private DelegateCommand _RequestSupportCommand;

		private DelegateCommand _showMouseSettings;

		private DelegateCommand _VibrateCommand;

		private DelegateCommand<bool?> _showVibrationSettings;

		private DelegateCommand<bool?> _showGamepad;

		private RelayCommand _showKeyboard;

		private RelayCommand _showMouse;

		private RelayCommand _showKeypad;

		private RelayCommand _showGamepadKeypad;

		private DelegateCommand _showAdvancedSettings;

		private DelegateCommand<XBBinding> _showMacroSettings;

		private DelegateCommand _showZones;

		private DelegateCommand _showBlacklistPreferencesCommand;

		private DelegateCommand _openMobileConnectionGuideCommand;

		private DelegateCommand _showLEDPreferencesCommand;

		private DelegateCommand _showSlotHotkeysPreferencesCommand;

		private DelegateCommand _showExternalDeviceSettingsCommand;

		private DelegateCommand _ShowMacroHelp;

		private DelegateCommand _startStopGamepadsDetectionModeCommand;

		private RelayCommand<Slot> _restoreDefaults;

		private RelayCommand<Slot> _changeGamepadSlot;

		private RelayCommand<Slot> _selectSlot;

		private SlotInfo _currentSlotInfo;

		private ObservableCollection<ExternalDevice> _externalDeviceCollection;

		private DelegateCommand _addExternalDeviceCommand;

		private DelegateCommand _externalDeviceBluetoothReconnectCommand;

		private DelegateCommand _applyAmiiboCommand;
	}
}
