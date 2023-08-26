using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using DiscSoft.NET.Common.View.Controls.MultiRangeSlider;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Services;
using reWASDUI.Services.Interfaces;
using reWASDUI.Utils.XBUtil;
using reWASDUI.Views;
using reWASDUI.Views.ContentZoneGamepad;
using reWASDUI.Views.OverlayMenu;
using reWASDUI.Views.VirtualSticks;
using XBEliteWPF.Services.Interfaces;

namespace reWASDUI.ViewModels
{
	internal class MainWindowViewModel : BindableBase, ISetSlotModel, IBaseServicesContainer
	{
		public GamepadService GamepadService { get; set; }

		public KeyBindingService KeyBindingService { get; set; }

		public LicensingService LicensingService { get; set; }

		public GuiScaleService GuiScaleService { get; set; }

		public GuiHelperService GuiHelperService { get; set; }

		public EventAggregator EventAggregator { get; set; }

		public GameProfilesService GameProfilesService { get; set; }

		public DeviceDetectionService DeviceDetectionService { get; set; }

		public EngineSelectorService EngineSelectorService { get; set; }

		public DelegateCommand<object> NavigateContentCommand { get; private set; }

		public DelegateCommand<object> NavigateBindingFrameCommand { get; private set; }

		public DelegateCommand<object> NavigateGamepadCommand { get; private set; }

		public DelegateCommand<object> NavigateSideBarCommand { get; private set; }

		public IRegionManager RegionManager
		{
			get
			{
				return this._regionManager;
			}
		}

		public DelegateCommand CheckForUpdatesCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._checkForUpdates) == null)
				{
					delegateCommand = (this._checkForUpdates = new DelegateCommand(new Action(this.CheckForUpdates)));
				}
				return delegateCommand;
			}
		}

		private async void CheckForUpdates()
		{
			await this.LicensingService.CheckForUpdate();
		}

		public DelegateCommand AddGameCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._addGame) == null)
				{
					delegateCommand = (this._addGame = new DelegateCommand(new Action(this.AddGame)));
				}
				return delegateCommand;
			}
		}

		private void AddGame()
		{
			IContainerProviderExtensions.Resolve<IGameProfilesService>(this._container).AddGameProfile();
		}

		public Slot SelectedSlot
		{
			get
			{
				return this._selectedSlot;
			}
			set
			{
				this.SetProperty<Slot>(ref this._selectedSlot, value, "SelectedSlot");
			}
		}

		public DelegateCommand RestoreDefaultsCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._restoreDefaults) == null)
				{
					delegateCommand = (this._restoreDefaults = new DelegateCommand(new Action(this.RestoreDefaults)));
				}
				return delegateCommand;
			}
		}

		private void RestoreDefaults()
		{
			IContainerProviderExtensions.Resolve<IGamepadService>(this._container).RestoreDefaults(new List<Slot> { this.SelectedSlot });
		}

		public MainWindowViewModel(IContainerProvider container, IEventAggregator ea, IRegionManager rm, IGamepadService gps, IGameProfilesService gprs, ILicensingService fls, IKeyBindingService kbs, IGuiScaleService gsc, IDeviceDetectionService dds, IGuiHelperService ghs, IEngineSelectorService ess)
		{
			this._container = container;
			this.EventAggregator = (EventAggregator)ea;
			this.GamepadService = (GamepadService)gps;
			this.GameProfilesService = (GameProfilesService)gprs;
			this.LicensingService = (LicensingService)fls;
			this.KeyBindingService = (KeyBindingService)kbs;
			this.GuiScaleService = (GuiScaleService)gsc;
			this.GuiHelperService = (GuiHelperService)ghs;
			this.DeviceDetectionService = (DeviceDetectionService)dds;
			this.EngineSelectorService = (EngineSelectorService)ess;
			this._regionManager = rm;
			this._regionManager.RegisterViewWithRegion(RegionNames.Content, typeof(MainContent));
			IRegionManager regionManager = this._regionManager;
			string gamepad = RegionNames.Gamepad;
			BaseControllerVM currentGamepad = this.GamepadService.CurrentGamepad;
			regionManager.RegisterViewWithRegion(gamepad, XBUtils.GetZoneTypeByControllerFamily((currentGamepad != null) ? new ControllerFamily?(currentGamepad.ControllerFamily) : null));
			this.NavigateContentCommand = new DelegateCommand<object>(new Action<object>(this.NavigateContentRegion));
			this.NavigateBindingFrameCommand = new DelegateCommand<object>(new Action<object>(this.NavigateBindingFrameRegion));
			this.NavigateGamepadCommand = new DelegateCommand<object>(new Action<object>(this.NavigateGamepadRegion));
			reWASDApplicationCommands.NavigateContentCommand.RegisterCommand(this.NavigateContentCommand);
			reWASDApplicationCommands.NavigateBindingFrameCommand.RegisterCommand(this.NavigateBindingFrameCommand);
			reWASDApplicationCommands.NavigateGamepadCommand.RegisterCommand(this.NavigateGamepadCommand);
			this.EventAggregator.GetEvent<CloseAllPopups>().Subscribe(delegate(object action)
			{
				MultiRangeSlider.FireCloseAllPopups();
			});
		}

		public bool IsMainContent()
		{
			return this.RegionManager.Regions[RegionNames.Content].ActiveViews.Any((object item) => item is MainContent);
		}

		private void NavigateContentRegion(object navigatePath)
		{
			this.Navigate(navigatePath, RegionNames.Content);
		}

		private void NavigateBindingFrameRegion(object navigatePath)
		{
			this.NavigateBindingframe(navigatePath);
		}

		private void NavigateGamepadRegion(object navigatePath)
		{
			this.Navigate(navigatePath, RegionNames.Gamepad);
		}

		private void NavigateSideBar(object navigatePath)
		{
			this.Navigate(navigatePath, RegionNames.Sidebar);
		}

		private void Navigate(object navigatePath, string regionName)
		{
			Dictionary<object, object> dictionary = navigatePath as Dictionary<object, object>;
			if (dictionary != null)
			{
				object obj = dictionary["navigatePath"];
				NavigationParameters navigationParameters = dictionary["NavigationParameters"] as NavigationParameters;
				this._regionManager.RequestNavigate(regionName, obj.ToString(), navigationParameters);
				this.EventAggregator.GetEvent<OnNavigateRequest>().Publish(obj);
			}
			else if (navigatePath != null)
			{
				this._regionManager.RequestNavigate(regionName, navigatePath.ToString());
				this.EventAggregator.GetEvent<OnNavigateRequest>().Publish(navigatePath);
			}
			if (regionName == RegionNames.Gamepad)
			{
				IContainerProviderExtensions.Resolve<GameConfigSelectorVM>(App.Container).GameConfigSelectorShouldBeShown = navigatePath as Type == typeof(KeyboardMappingView) || navigatePath as Type == typeof(MouseMappingView) || navigatePath as Type == typeof(OverlayMenuView) || navigatePath as Type == typeof(MouseSettings) || navigatePath as Type == typeof(VirtualSticksSettingsView) || navigatePath as Type == typeof(MaskView) || navigatePath as Type == typeof(SVGGamepadWithAllAnnotations);
			}
			this.OnFinishNavigate();
		}

		private void OnFinishNavigate()
		{
			this.EventAggregator.GetEvent<CloseAllPopups>().Publish(null);
		}

		private void NavigateBindingframe(object navObject)
		{
			Dictionary<object, object> dictionary = navObject as Dictionary<object, object>;
			bool flag = false;
			object obj = null;
			NavigationParameters navigationParameters = null;
			if (dictionary != null)
			{
				obj = dictionary["navigatePath"];
				navigationParameters = dictionary["NavigationParameters"] as NavigationParameters;
			}
			else
			{
				obj = navObject;
			}
			foreach (SingleRegionManager singleRegionManager in App.BindingFrameRegionManagers)
			{
				if (navObject == null)
				{
					obj = singleRegionManager.DefaultView;
				}
				if (obj != null)
				{
					if (navigationParameters != null)
					{
						singleRegionManager.RequestNavigate(singleRegionManager.RegionName, obj.ToString(), navigationParameters);
						flag = true;
					}
					else
					{
						singleRegionManager.RequestNavigate(singleRegionManager.RegionName, obj.ToString());
						flag = true;
					}
				}
			}
			if (flag)
			{
				this.EventAggregator.GetEvent<OnNavigateRequest>().Publish(obj);
			}
		}

		public void OnMainWindowKeyDown(object sender, KeyEventArgs e)
		{
			TextBox textBox = e.OriginalSource as TextBox;
		}

		private IContainerProvider _container;

		private IRegionManager _regionManager;

		private DelegateCommand _checkForUpdates;

		private DelegateCommand _addGame;

		private Slot _selectedSlot;

		private DelegateCommand _restoreDefaults;
	}
}
