using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using Overlay.NET.Wpf;
using Prism.Ioc;
using reWASDCommon.Infrastructure.Enums;
using reWASDEngine;
using reWASDEngine.OverlayAPI;
using reWASDEngine.Services.OverlayAPI;
using XBEliteWPF.DataModels;
using XBEliteWPF.DataModels.GamepadActiveProfiles;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.Controller;
using XBEliteWPF.Services;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils.Extensions;

namespace DTOverlay
{
	public class OverlayMenuE
	{
		public OverlayMenuE()
		{
			((EventProcessor)IContainerProviderExtensions.Resolve<IEventProcessor>(Engine.SContainer)).OverlayHideRadialMenu += this.OverlayHideRadialMenu;
		}

		public bool IsShowed
		{
			get
			{
				return this.menuWindow != null && this.menuWindow.Visibility == Visibility.Visible;
			}
		}

		private void StartPollerMenu()
		{
			this.pollingTimerMenu = new DispatcherTimer();
			this.pollingTimerMenu.Tick += delegate([Nullable(2)] object o, EventArgs e)
			{
				this.OnTick_MenuPoller();
			};
			this.pollingTimerMenu.Interval = new TimeSpan(0, 0, 0, 0, 30);
			this.pollingTimerMenu.Start();
		}

		public void ShowMenu(BaseControllerVM controller, GamepadProfile gamepadProfile, string iID, bool toggle)
		{
			Tracer.TraceWrite("OverlayMenuE ShowMenu", false);
			this.fControllerStateHashLock.WaitOne();
			this.fControllerStateHash.Clear();
			this.fControllerStateHashLock.ReleaseMutex();
			CompositeControllerVM compositeControllerVM = controller as CompositeControllerVM;
			if (compositeControllerVM != null && (compositeControllerVM.HasGamepadControllers || compositeControllerVM.HasMouseControllers))
			{
				foreach (BaseControllerVM baseControllerVM in compositeControllerVM.BaseControllers)
				{
					ControllerVM controllerVM = baseControllerVM as ControllerVM;
					if (controllerVM != null)
					{
						this.ProcessControllerVM(controllerVM);
					}
					PeripheralVM peripheralVM = baseControllerVM as PeripheralVM;
					if (peripheralVM != null)
					{
						this.ProcessPeripheralVM(peripheralVM);
					}
				}
			}
			PeripheralVM peripheralVM2 = controller as PeripheralVM;
			if (peripheralVM2 != null)
			{
				this.ProcessPeripheralVM(peripheralVM2);
			}
			ControllerVM controllerVM2 = controller as ControllerVM;
			if (controllerVM2 != null)
			{
				this.ProcessControllerVM(controllerVM2);
			}
			string gameName = "";
			string profileName = "";
			if (gamepadProfile == null)
			{
				return;
			}
			gameName = gamepadProfile.GameName;
			profileName = gamepadProfile.ProfileName;
			Tracer.TraceWrite("OverlayMenuE ShowMenu gameName:" + gameName + "  profileName:" + profileName, false);
			string.Format("{0}: {1}", gameName, profileName);
			IGameProfilesService gameProfilesService = Engine.GameProfilesService;
			Game game = ((gameProfilesService != null) ? gameProfilesService.GamesCollection.FirstOrDefault((Game g) => g.Name.Equals(gameName)) : null);
			if (game == null)
			{
				Tracer.TraceWrite("OverlayMenuE ShowMenu game == null", false);
				return;
			}
			Config config = game.ConfigCollection.FirstOrDefault((Config c) => c.Name.Equals(profileName));
			if (config == null)
			{
				Tracer.TraceWrite("OverlayMenuE ShowMenu config == null", false);
				return;
			}
			config.ReadConfigFromJsonIfNotLoaded(true);
			REWASD_CONTROLLER_PROFILE_EX rewasd_CONTROLLER_PROFILE_EX = Engine.GamepadService.ServiceProfilesCollection.FindByID(iID);
			if (rewasd_CONTROLLER_PROFILE_EX == null)
			{
				Tracer.TraceWrite("OverlayMenuE ShowMenu profileEx == null", false);
				return;
			}
			Slot currentSlot = rewasd_CONTROLLER_PROFILE_EX.GetCurrentSlot();
			ushort num = rewasd_CONTROLLER_PROFILE_EX.ServiceProfileIds[currentSlot];
			if (this.menuWindow == null || !this.menuID.Equals(gameName + config.Name))
			{
				this.menuWindow = new OverlayMenuViewE(this, controller, iID, num, config, toggle);
				if (this.blackWindow == null)
				{
					this.blackWindow = new OverlayMenuBlackViewE((double)this._menuvm.Circle.TintBackground / 100.0);
					if (this._menuvm.Circle.IsTintedBackground)
					{
						this.blackWindow.Show();
					}
				}
				this.blackWindow.Topmost = true;
				this.menuID = gameName + config.Name;
				this.menuWindow.Show();
				this.menuWindow.Topmost = true;
			}
			else
			{
				if (this.blackWindow != null)
				{
					if (this._menuvm.Circle.IsTintedBackground)
					{
						this.blackWindow.Visibility = Visibility.Visible;
					}
					else
					{
						this.blackWindow.Visibility = Visibility.Hidden;
					}
				}
				this.menuWindow.Visibility = Visibility.Visible;
				this.menuWindow.UpdateButtons(controller, config);
			}
			if (this.menuWindow.IsOkIsLoaded)
			{
				this.needInitMousePOsitionForOverlayMenu = true;
				this.ID = iID;
				this._menuvm.StartPoller();
				this.StartPollerMenu();
				this.SavePictureMenuWindow();
				return;
			}
			this.menuWindow = null;
		}

		public void NeedInitMousePos()
		{
			this.needInitMousePOsitionForOverlayMenu = true;
		}

		public void ClearCachedMenu(string gameName, string configName)
		{
			if ((this.menuID.Equals(gameName + configName) || gameName.Equals("")) && this.menuWindow != null)
			{
				Application.Current.Dispatcher.Invoke(delegate
				{
					this.HideMenu();
					OverlayMenuViewE overlayMenuViewE = this.menuWindow;
					if (overlayMenuViewE != null)
					{
						overlayMenuViewE.Hide();
					}
					OverlayMenuViewE overlayMenuViewE2 = this.menuWindow;
					if (overlayMenuViewE2 != null)
					{
						overlayMenuViewE2.Close();
					}
					this.menuWindow = null;
					OverlayMenuBlackViewE overlayMenuBlackViewE = this.blackWindow;
					if (overlayMenuBlackViewE != null)
					{
						overlayMenuBlackViewE.Hide();
					}
					OverlayMenuBlackViewE overlayMenuBlackViewE2 = this.blackWindow;
					if (overlayMenuBlackViewE2 != null)
					{
						overlayMenuBlackViewE2.Close();
					}
					this.blackWindow = null;
				});
			}
		}

		public void HideMenu()
		{
			if (this.menuWindow != null)
			{
				OverlayMenuVME menuvm = this._menuvm;
				if (menuvm != null)
				{
					menuvm.StopPoller();
				}
				OverlayMenuVME menuvm2 = this._menuvm;
				if (menuvm2 != null)
				{
					menuvm2.HideSubmenu();
				}
				this.StopPollerMenu();
				OverlayTracker overlayTracker = this.overlayTracker;
				if (overlayTracker != null)
				{
					overlayTracker.HideMenu();
				}
				Application.Current.Dispatcher.Invoke(delegate
				{
					this.menuWindow.Visibility = Visibility.Collapsed;
					if (this.blackWindow != null)
					{
						this.blackWindow.Visibility = Visibility.Collapsed;
					}
				});
			}
		}

		public void ExecuteOverlayMenuCommand(RewasdOverlayMenuServiceCommand rewasdOverlayMenuServiceCommand)
		{
			if (this.menuWindow != null)
			{
				OverlayMenuVME menuvm = this._menuvm;
				if (menuvm == null)
				{
					return;
				}
				menuvm.ExecuteOverlayMenuCommand(rewasdOverlayMenuServiceCommand);
			}
		}

		private OverlayMenuVME _menuvm
		{
			get
			{
				if (this.menuWindow == null)
				{
					return null;
				}
				return this.menuWindow.ViewModel;
			}
		}

		private void KeyForMenu(ControllerTypeEnum controllerType, GamepadState gamepadState, ulong id)
		{
			if (this._menuvm != null && this._menuvm.OverlayKeyPressed(controllerType, gamepadState, id))
			{
				this.SavePictureMenuWindow();
			}
		}

		private void MouseForMenu(MouseState mouseState, ulong id)
		{
			if (this._menuvm != null)
			{
				OverlayMenuVME menuvm = this._menuvm;
				if (((menuvm != null) ? new bool?(menuvm.OverlayMouseMove(mouseState, id)) : null).Value)
				{
					this.SavePictureMenuWindow();
				}
			}
		}

		private void StopPollerMenu()
		{
			DispatcherTimer dispatcherTimer = this.pollingTimerMenu;
			if (dispatcherTimer != null)
			{
				dispatcherTimer.Stop();
			}
			this.pollingTimerMenu = null;
		}

		private async void OnTick_MenuPoller()
		{
			if (this.menuWindow != null && this.menuWindow.Visibility == Visibility.Visible)
			{
				await this.ReadPhysicalControllerStates();
			}
			if (this.menuWindow != null)
			{
				this.HandleFizControllerStates();
			}
		}

		private void OverlayHideRadialMenu()
		{
			this.HideMenu();
		}

		private async Task<int> ReadPhysicalControllerStates()
		{
			this.fControllerStateHashLock.WaitOne();
			List<VirtualGamepadInfo> list = this.fControllerStateHash.ToList<VirtualGamepadInfo>();
			this.fControllerStateHashLock.ReleaseMutex();
			foreach (VirtualGamepadInfo val in list)
			{
				if (Engine.GamepadService.IsGamepadOnline(val.id))
				{
					if (ControllerTypeExtensions.IsGamepad(val.controllerType))
					{
						AsyncLock.Releaser releaser2 = await new AsyncLock(GamepadService._refreshServiceProfilesSemaphore).LockAsync();
						using (releaser2)
						{
							GamepadState gamepadState = await Engine.XBServiceCommunicator.GetGamepadPressedButtons(val.id, val.Type, val.controllerType, false);
							this.fControllerStateHashLock.WaitOne();
							val.gamepadState = gamepadState;
							this.fControllerStateHashLock.ReleaseMutex();
						}
						AsyncLock.Releaser releaser = default(AsyncLock.Releaser);
					}
					if (ControllerTypeExtensions.IsMouse(val.controllerType))
					{
						AsyncLock.Releaser releaser2 = await new AsyncLock(GamepadService._refreshServiceProfilesSemaphore).LockAsync();
						MouseState mouseState;
						using (releaser2)
						{
							mouseState = await Engine.XBServiceCommunicator.GetMousePressedButtons(val.id, val.Type, val.controllerType);
						}
						AsyncLock.Releaser releaser = default(AsyncLock.Releaser);
						this.fControllerStateHashLock.WaitOne();
						val.mouseState = mouseState;
						this.fControllerStateHashLock.ReleaseMutex();
						if (this.needInitMousePOsitionForOverlayMenu)
						{
							OverlayMenuVME menuvm = this._menuvm;
							if (menuvm != null)
							{
								menuvm.SetMouseInitPosition(mouseState.MouseXDelta, mouseState.MouseYDelta);
							}
							this.needInitMousePOsitionForOverlayMenu = false;
						}
					}
				}
				val = null;
			}
			List<VirtualGamepadInfo>.Enumerator enumerator = default(List<VirtualGamepadInfo>.Enumerator);
			return 1;
		}

		private void HandleFizControllerStates()
		{
			this.fControllerStateHashLock.WaitOne();
			List<VirtualGamepadInfo> list = this.fControllerStateHash.ToList<VirtualGamepadInfo>();
			this.fControllerStateHashLock.ReleaseMutex();
			using (List<VirtualGamepadInfo>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					VirtualGamepadInfo virtualGamepadInfo = enumerator.Current;
					if (ControllerTypeExtensions.IsGamepad(virtualGamepadInfo.controllerType) && virtualGamepadInfo.gamepadState != null)
					{
						HashInfo hashInfo = OverlayUtils.CalcGamepadHash(virtualGamepadInfo.gamepadState, virtualGamepadInfo.oldHashGamepad);
						if (virtualGamepadInfo.oldHashGamepad != null && hashInfo.Hash != virtualGamepadInfo.oldHashGamepad.Hash)
						{
							Application.Current.Dispatcher.Invoke(delegate
							{
								this.KeyForMenu(virtualGamepadInfo.controllerType, virtualGamepadInfo.gamepadState, virtualGamepadInfo.id);
							});
						}
						this.fControllerStateHashLock.WaitOne();
						virtualGamepadInfo.oldHashGamepad = hashInfo;
						this.fControllerStateHashLock.ReleaseMutex();
					}
					if (ControllerTypeExtensions.IsMouse(virtualGamepadInfo.controllerType) && virtualGamepadInfo.mouseState != null)
					{
						long num = OverlayUtils.CalcMousedHash(virtualGamepadInfo.mouseState, virtualGamepadInfo.oldHashMouse);
						if (num != virtualGamepadInfo.oldHashMouse)
						{
							Application.Current.Dispatcher.Invoke(delegate
							{
								this.MouseForMenu(virtualGamepadInfo.mouseState, virtualGamepadInfo.id);
							});
						}
						this.fControllerStateHashLock.WaitOne();
						virtualGamepadInfo.oldHashMouse = num;
						this.fControllerStateHashLock.ReleaseMutex();
					}
				}
			}
		}

		private void ProcessPeripheralVM(PeripheralVM peripheralVM)
		{
			if (peripheralVM.HasMouseControllers)
			{
				foreach (ControllerVM controllerVM in peripheralVM.Controllers.Where((ControllerVM X) => X.IsMouse))
				{
					if (controllerVM.ControllerId != 0UL && ControllerTypeExtensions.IsMouse(controllerVM.ControllerType))
					{
						this.fControllerStateHashLock.WaitOne();
						this.fControllerStateHash.Add(new VirtualGamepadInfo
						{
							gamepadState = null,
							mouseState = null,
							isPresent = true,
							controllerType = controllerVM.ControllerType,
							Type = controllerVM.Type,
							id = controllerVM.ControllerId
						});
						this.fControllerStateHashLock.ReleaseMutex();
					}
				}
			}
		}

		private void ProcessControllerVM(ControllerVM controllerVM)
		{
			if (controllerVM.ControllerId != 0UL)
			{
				this.fControllerStateHashLock.WaitOne();
				this.fControllerStateHash.Add(new VirtualGamepadInfo
				{
					gamepadState = null,
					mouseState = null,
					isPresent = true,
					controllerType = controllerVM.ControllerType,
					Type = controllerVM.Type,
					id = controllerVM.ControllerId
				});
				this.fControllerStateHashLock.ReleaseMutex();
			}
		}

		private void SavePictureMenuWindow()
		{
			Application.Current.Dispatcher.Invoke(delegate
			{
				if (Engine.UserSettingsService.IsOverlayEnable && Engine.UserSettingsService.ShowDirectXOverlay && this.overlayTracker != null && this.overlayTracker.IsDirectXAttached())
				{
					Application.Current.Dispatcher.BeginInvoke(new Action(delegate
					{
						this.DoSavePictureMenuWindow();
					}), DispatcherPriority.ContextIdle, null);
				}
			});
		}

		public void DoSavePictureMenuWindow()
		{
			if (this.menuWindow != null)
			{
				MemoryStream memoryStream = this.menuWindow.CopyAsBitmap().Encode(new PngBitmapEncoder());
				string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Disc-Soft", "reWASD");
				OverlayUtils.ClearOldPng("MenuWindow", this.menuFileID);
				string text2 = text + "\\OverlayData\\MenuWindow" + this.menuFileID.ToString() + ".png";
				try
				{
					using (FileStream fileStream = new FileStream(text2, FileMode.Create, FileAccess.Write))
					{
						memoryStream.WriteTo(fileStream);
					}
				}
				catch (Exception)
				{
					Tracer.TraceWrite("OverlayMenuE DoSavePictureMessages. Error write file. " + text2, false);
				}
				this.MenuRendered(this.menuFileID);
				this.menuFileID++;
				if (this.menuFileID > 250)
				{
					this.menuFileID = 0;
				}
			}
		}

		private void MenuRendered(int id)
		{
			int num = (this._menuvm.Circle.IsTintedBackground ? this._menuvm.Circle.TintBackground : 0);
			OverlayTracker overlayTracker = this.overlayTracker;
			if (overlayTracker == null)
			{
				return;
			}
			overlayTracker.ShowMenu(id * 256 + num * 256 / 100, Engine.UserSettingsService.OverlayMenuAlign);
		}

		public OverlayTracker overlayTracker;

		public string ID;

		private Mutex fControllerStateHashLock = new Mutex();

		private List<VirtualGamepadInfo> fControllerStateHash = new List<VirtualGamepadInfo>();

		private OverlayMenuBlackViewE blackWindow;

		private OverlayMenuViewE menuWindow;

		private DispatcherTimer pollingTimerMenu;

		private bool needInitMousePOsitionForOverlayMenu;

		private int menuFileID = 1;

		private string menuID = "";
	}
}
