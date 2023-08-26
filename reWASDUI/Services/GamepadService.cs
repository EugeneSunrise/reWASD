using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using DiscSoft.NET.Common.AdminRightsFeatures;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoftReWASDServiceNamespace;
using Prism.Commands;
using Prism.Events;
using reWASDCommon;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Infrastructure.LED;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using reWASDCommon.Network.HTTP.DataTransferObjects.Events.Desktop;
using reWASDCommon.Utils;
using reWASDUI.DataModels.CompositeDevicesCollection;
using reWASDUI.DataModels.GamepadActiveProfiles;
using reWASDUI.DataModels.GamepadSlotHotkeyCollection;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.reWASDMapping.RewasduserCommands;
using reWASDUI.Services.Interfaces;
using XBEliteWPF.DataModels.PeripheralDevicesCollection;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.ExternalDevices;
using XBEliteWPF.License.Licensing.ComStructures;
using XBEliteWPF.Services;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.ViewModels.Base;

namespace reWASDUI.Services
{
	public class GamepadService : ZBindable, IGamepadService, IServiceInitedAwaitable, IAdminOperationsDeciderContainer
	{
		public ObservableCollection<BaseControllerVM> ControllersAvailiableForComposition { get; } = new ObservableCollection<BaseControllerVM>();

		public SortableObservableCollection<BaseControllerVM> AllPhysicalControllers { get; } = new SortableObservableCollection<BaseControllerVM>(true);

		public SortableObservableCollection<BaseControllerVM> GamepadCollection { get; } = new SortableObservableCollection<BaseControllerVM>(new Dictionary<string, ListSortDirection>
		{
			{
				"IsOnline",
				ListSortDirection.Descending
			},
			{
				"ControllerFamily",
				ListSortDirection.Ascending
			},
			{
				"ID",
				ListSortDirection.Ascending
			}
		}, false);

		public ExternalDeviceRelationsHelper ExternalDeviceRelationsHelper
		{
			get
			{
				return this._externalDeviceRelationsHelper;
			}
		}

		public BinDataSerialize BinDataSerialize
		{
			get
			{
				return this._binDataSerialize;
			}
		}

		public AdminOperationsDecider AdminOperationsDecider { get; set; }

		public bool IsExclusiveCaptureControllersPresent { get; set; }

		public bool IsExclusiveCaptureProfilePresent { get; set; }

		public bool IsAnyGamepadConnected
		{
			get
			{
				return this.GamepadCollection.Count > 0 && (this.GamepadCollection.Count != 1 || !this.GamepadCollection[0].IsPromoController);
			}
		}

		public bool IsSingleGamepadConnected
		{
			get
			{
				return this.GamepadCollection.Count == 1;
			}
		}

		public bool IsMultipleGamepadsConnected
		{
			get
			{
				return this.GamepadCollection.Count > 1;
			}
		}

		public async Task SetCurrentGamepad(BaseControllerVM gamepad)
		{
			if (this._currentGamepad != gamepad && gamepad != null)
			{
				await this.SetCurrentGamepad(null);
			}
			using (await new AsyncLock(GamepadService._setCurrentGamepadSemaphore).LockAsync())
			{
				BaseControllerVM prevGamepad = this._currentGamepad;
				if (this.SetProperty<BaseControllerVM>(ref this._currentGamepad, gamepad, "CurrentGamepad"))
				{
					if (this.IsExclusiveCaptureControllersPresent && this._currentGamepad != null && !this._currentGamepad.HasExclusiveCaptureControllers && this.IsExclusiveCaptureProfilePresent)
					{
						await this._httpClientService.Gamepad.DeleteAllExclusiveCaptureProfiles();
					}
					this.OnPropertyChanged("CurrentGamepadActiveProfiles");
					this.RefreshCurrentGamepadSVGs();
					this.IsCurrentGamepadBackShown = false;
					IEventAggregator eventAggregator = App.EventAggregator;
					if (eventAggregator != null)
					{
						eventAggregator.GetEvent<CurrentGamepadChanged>().Publish(this.CurrentGamepad);
					}
					this.RefreshRemapStateProperties();
					if (this._currentGamepad != null)
					{
						RegistryHelper.SetString("Config", "CurrentGamepad", this._currentGamepad.ID);
					}
					CompositeControllerVM compositeControllerVM = this._currentGamepad as CompositeControllerVM;
					if (compositeControllerVM != null)
					{
						compositeControllerVM.CurrentControllerChanged += this.CcOnCurrentControllerChanged;
					}
					CompositeControllerVM compositeControllerVM2 = prevGamepad as CompositeControllerVM;
					if (compositeControllerVM2 != null)
					{
						compositeControllerVM2.CurrentControllerChanged -= this.CcOnCurrentControllerChanged;
					}
					if (this._currentGamepad != null)
					{
						this.ExternalDeviceRelationsHelper.Refresh();
					}
				}
				prevGamepad = null;
			}
			AsyncLock.Releaser releaser = default(AsyncLock.Releaser);
		}

		public BaseControllerVM CurrentGamepad
		{
			get
			{
				return this._currentGamepad;
			}
			set
			{
				this.SetCurrentGamepad(value);
			}
		}

		private void CcOnCurrentControllerChanged(object sender, ControllerVM e)
		{
			this.RefreshCurrentGamepadSVGs();
			this._eventAggregator.GetEvent<CurrentGamepadCurrentChanged>().Publish(e);
		}

		public ControllerVM CurrentVirtualGamepad
		{
			get
			{
				return this._currentVirtualGamepad;
			}
			set
			{
				this.SetProperty<ControllerVM>(ref this._currentVirtualGamepad, value, "CurrentVirtualGamepad");
			}
		}

		public GamepadProfilesCollection GamepadProfileRelations
		{
			get
			{
				return this._gamepadProfileRelations;
			}
			set
			{
				this.SetProperty<GamepadProfilesCollection>(ref this._gamepadProfileRelations, value, "GamepadProfileRelations");
			}
		}

		public AutoGamesDetectionGamepadProfilesCollection AutoGamesDetectionGamepadProfileRelations
		{
			get
			{
				return this._autoGamesDetectionGamepadProfilesCollection;
			}
			set
			{
				this.SetProperty<AutoGamesDetectionGamepadProfilesCollection>(ref this._autoGamesDetectionGamepadProfilesCollection, value, "AutoGamesDetectionGamepadProfileRelations");
			}
		}

		public ExternalDevicesCollection ExternalDevices
		{
			get
			{
				return this._externalDevices;
			}
			set
			{
				this.SetProperty<ExternalDevicesCollection>(ref this._externalDevices, value, "ExternalDevices");
			}
		}

		public ObservableCollection<ExternalClient> ExternalClients
		{
			get
			{
				return this._externalClients;
			}
			set
			{
				this.SetProperty<ObservableCollection<ExternalClient>>(ref this._externalClients, value, "ExternalClients");
			}
		}

		public ObservableCollection<BlackListGamepad> BlacklistGamepads { get; set; } = new ObservableCollection<BlackListGamepad>();

		public ObservableCollection<GamepadSettings> GamepadsSettings { get; set; } = new ObservableCollection<GamepadSettings>();

		public GamepadProfiles CurrentGamepadActiveProfiles
		{
			get
			{
				return this.GetGamepadActiveProfiles(this.CurrentGamepad);
			}
		}

		private GamepadProfiles GetGamepadActiveProfiles(BaseControllerVM gamepad)
		{
			if (gamepad == null || this.GamepadProfileRelations == null)
			{
				return null;
			}
			return this.GamepadProfileRelations.TryGetValue(gamepad.ID);
		}

		public ObservableCollection<SlotInfo> SlotsInfo { get; private set; }

		public GamepadsHotkeyDictionary GamepadsHotkeyCollection
		{
			get
			{
				return this._gamepadsSlotHotkeyCollection;
			}
			set
			{
				this.SetProperty<GamepadsHotkeyDictionary>(ref this._gamepadsSlotHotkeyCollection, value, "GamepadsHotkeyCollection");
			}
		}

		public GamepadsPlayerLedDictionary GamepadsUserLedCollection
		{
			get
			{
				return this._gamepadsUserLedCollection;
			}
			set
			{
				this.SetProperty<GamepadsPlayerLedDictionary>(ref this._gamepadsUserLedCollection, value, "GamepadsUserLedCollection");
			}
		}

		public CompositeDevices CompositeDevices
		{
			get
			{
				return this._compositeDevices;
			}
			set
			{
				this.SetProperty<CompositeDevices>(ref this._compositeDevices, value, "CompositeDevices");
			}
		}

		public PeripheralDevices PeripheralDevices
		{
			get
			{
				return this._peripheralDevices;
			}
			set
			{
				this.SetProperty<PeripheralDevices>(ref this._peripheralDevices, value, "PeripheralDevices");
			}
		}

		public event EventHandler IsRemapChanged;

		public bool IgnoreServiceSlotChangedCallbacks { get; set; }

		public bool IsCurrentGamepadBackShown
		{
			get
			{
				return this._isCurrentGamepadBackShown;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isCurrentGamepadBackShown, value, "IsCurrentGamepadBackShown"))
				{
					this.OnPropertyChanged("CurrentGamepadSVGUri");
					this.OnPropertyChanged("CurrentGamepadFlipStateButtonSVGUri");
					this.OnPropertyChanged("CurrentGamepadFlipToFaceButtonSVGUri");
					this.OnPropertyChanged("CurrentGamepadFlipToBackButtonSVGUri");
				}
			}
		}

		public string GamepadTypeForResource
		{
			get
			{
				string text = "GamepadXBElite";
				BaseControllerVM baseControllerVM = this.CurrentGamepad;
				CompositeControllerVM compositeControllerVM = this.CurrentGamepad as CompositeControllerVM;
				if (compositeControllerVM != null)
				{
					if (compositeControllerVM.IsNintendoSwitchJoyConComposite)
					{
						return "GamepadNJCon";
					}
					baseControllerVM = compositeControllerVM.CurrentController;
				}
				ControllerVM controllerVM = baseControllerVM as ControllerVM;
				if (controllerVM != null)
				{
					if (controllerVM.SupportedControllerInfo != null)
					{
						text = controllerVM.SupportedControllerInfo.Image;
					}
					if (controllerVM.IsAnyAzeron && controllerVM.IsAzeronLefty)
					{
						text += "Lefty";
					}
					if (controllerVM.ControllerType == 50 && controllerVM.InitializedDeviceType == "Japanese")
					{
						text += "JapaneseEdition";
					}
				}
				return text;
			}
		}

		public string GamepadTypeForTriggerResource
		{
			get
			{
				string text = "GamepadXB";
				ControllerVM controllerVM = this.CurrentGamepad as ControllerVM;
				if (controllerVM != null)
				{
					if (controllerVM.IsSonyDualshock3)
					{
						text = "GamepadDS3";
					}
					if (controllerVM.IsSonyDualshock3Adapter || controllerVM.IsSonyPs3Navigation)
					{
						text = "GamepadDS3";
					}
					if (controllerVM.IsSonyDualshock4)
					{
						text = "GamepadDS4";
					}
					if (controllerVM.IsAnySonyDualSense)
					{
						text = "GamepadDS4";
					}
				}
				return text;
			}
		}

		public Drawing CurrentGamepadLeftStickSVGUri
		{
			get
			{
				return Application.Current.TryFindResource(this.GamepadTypeForResource + "LeftStick") as Drawing;
			}
		}

		public Drawing CurrentGamepadRightStickSVGUri
		{
			get
			{
				return Application.Current.TryFindResource(this.GamepadTypeForResource + "RightStick") as Drawing;
			}
		}

		public Drawing CurrentGamepadLeftTriggerSVGUri
		{
			get
			{
				return Application.Current.TryFindResource(this.GamepadTypeForTriggerResource + "LeftTrigger") as Drawing;
			}
		}

		public Drawing CurrentGamepadRightTriggerSVGUri
		{
			get
			{
				return Application.Current.TryFindResource(this.GamepadTypeForTriggerResource + "RightTrigger") as Drawing;
			}
		}

		public Drawing CurrentGamepadTrackpad1ZoneSVGUri
		{
			get
			{
				return Application.Current.TryFindResource("GamepadValveSteamDeckLeftTrackpad") as Drawing;
			}
		}

		public Drawing CurrentGamepadTrackpad2ZoneSVGUri
		{
			get
			{
				return Application.Current.TryFindResource("GamepadValveSteamDeckRightTrackpad") as Drawing;
			}
		}

		public Drawing CurrentGamepadSVGUri
		{
			get
			{
				return Application.Current.TryFindResource(this.IsCurrentGamepadBackShown ? (this.GamepadTypeForResource + "Back") : this.GamepadTypeForResource) as Drawing;
			}
		}

		public Drawing CurrentMouseFlipSVGUri
		{
			get
			{
				BaseControllerVM currentGamepad = this.CurrentGamepad;
				bool flag;
				if (currentGamepad == null)
				{
					flag = false;
				}
				else
				{
					ControllerVM currentController = currentGamepad.CurrentController;
					bool? flag2 = ((currentController != null) ? new bool?(currentController.IsEngineMouse) : null);
					bool flag3 = true;
					flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
				if (flag)
				{
					return Application.Current.TryFindResource("EngineControllerMobileMouseFlipBack") as Drawing;
				}
				BaseControllerVM currentGamepad2 = this.CurrentGamepad;
				bool flag4;
				if (currentGamepad2 == null)
				{
					flag4 = false;
				}
				else
				{
					ControllerVM currentController2 = currentGamepad2.CurrentController;
					bool? flag2 = ((currentController2 != null) ? new bool?(currentController2.IsEngineMouseTouchpad) : null);
					bool flag3 = true;
					flag4 = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
				if (flag4)
				{
					return Application.Current.TryFindResource("EngineControllerMouseTouchpadFlipBack") as Drawing;
				}
				return Application.Current.TryFindResource("SwitchToMouse") as Drawing;
			}
		}

		public Drawing CurrentGamepadSVGUriForStartPage
		{
			get
			{
				Application application = Application.Current;
				BaseControllerVM currentGamepad = this.CurrentGamepad;
				return application.TryFindResource((currentGamepad != null && currentGamepad.IsAnyOfForbidden) ? "GamepadUnsupportedDevice" : this.GamepadTypeForResource) as Drawing;
			}
		}

		public Drawing CurrentGamepadFlipStateButtonSVGUri
		{
			get
			{
				if (!this.IsCurrentGamepadBackShown)
				{
					return this.CurrentGamepadFlipToBackButtonSVGUri;
				}
				return this.CurrentGamepadFlipToFaceButtonSVGUri;
			}
		}

		public Drawing CurrentGamepadFlipToFaceButtonSVGUri
		{
			get
			{
				return Application.Current.TryFindResource(this.GamepadTypeForResource + "FlipBack") as Drawing;
			}
		}

		public Drawing CurrentGamepadFlipToBackButtonSVGUri
		{
			get
			{
				return Application.Current.TryFindResource(this.GamepadTypeForResource + "Flip") as Drawing;
			}
		}

		public void RefreshCurrentGamepadSVGs()
		{
			this.OnPropertyChanged("CurrentGamepadSVGUri");
			this.OnPropertyChanged("CurrentGamepadSVGUriForStartPage");
			this.OnPropertyChanged("CurrentMouseFlipSVGUri");
			this.OnPropertyChanged("CurrentGamepadLeftStickSVGUri");
			this.OnPropertyChanged("CurrentGamepadRightStickSVGUri");
			this.OnPropertyChanged("CurrentGamepadLeftTriggerSVGUri");
			this.OnPropertyChanged("CurrentGamepadRightTriggerSVGUri");
			this.OnPropertyChanged("CurrentGamepadFlipStateButtonSVGUri");
			this.OnPropertyChanged("CurrentGamepadFlipToFaceButtonSVGUri");
			this.OnPropertyChanged("CurrentGamepadFlipToBackButtonSVGUri");
		}

		public DelegateCommand<bool?> CurrentGamepadFlipCommand
		{
			get
			{
				DelegateCommand<bool?> delegateCommand;
				if ((delegateCommand = this._FlipGamepad) == null)
				{
					delegateCommand = (this._FlipGamepad = new DelegateCommand<bool?>(new Action<bool?>(this.FlipGamepad)));
				}
				return delegateCommand;
			}
		}

		private void FlipGamepad(bool? showBack)
		{
			if (showBack != null)
			{
				this.IsCurrentGamepadBackShown = showBack.Value;
				return;
			}
			this.IsCurrentGamepadBackShown = !this.IsCurrentGamepadBackShown;
		}

		public GamepadService(IGameProfilesService gps, IEventAggregator ea, IAdminOperations ao, ILicensingService ls, IConfigFileService cfs, IDeviceDetectionService dds, IUserSettingsService uss, IHttpClientService hcs)
		{
			Tracer.TraceWrite("Constructor for GamepadService", false);
			this._eventAggregator = ea;
			this._gameProfilesService = gps;
			this.AdminOperationsDecider = (AdminOperationsDecider)ao;
			this._configFileService = cfs;
			this._licensingService = ls;
			this._httpClientService = hcs;
			this._deviceDetectionService = dds;
			this._userSettingsService = uss;
			BaseRewasdUserCommandInitializer.Init();
			this._binDataSerialize = new BinDataSerialize(this, gps);
			this.LoadAllBins();
			this._externalDeviceRelationsHelper = new ExternalDeviceRelationsHelper(this, gps);
			this.ReinitService();
			this._eventAggregator.GetEvent<CurrentGamepadChanged>().Subscribe(new Action<BaseControllerVM>(this.ProcessCurrentGamepadChanged));
			this._eventAggregator.GetEvent<SlotChanged>().Subscribe(delegate(SlotChangedEvent slotChangedData)
			{
				this.ProcessPhysicalSlot(slotChangedData);
			});
			this._eventAggregator.GetEvent<CurrentGamepadSlotChanged>().Subscribe(delegate(Slot slot)
			{
				BaseControllerVM currentGamepad = this.CurrentGamepad;
				this.SetCurrentSlotForGamepad((currentGamepad != null) ? currentGamepad.ID : null, slot);
			});
			this._eventAggregator.GetEvent<CompositeDevicesChanged>().Subscribe(delegate(WindowMessageEvent payload)
			{
				this.BinDataSerialize.LoadCompositeDevicesCollection(true);
			});
			this._eventAggregator.GetEvent<SlotChanged>().Subscribe(async delegate(SlotChangedEvent slotChangedData)
			{
				BaseControllerVM baseControllerVM = this.GamepadCollection.FirstOrDefault((BaseControllerVM item) => slotChangedData.ControllerId == item.ID);
				if (baseControllerVM != null)
				{
					BaseControllerVM baseControllerVM2 = baseControllerVM;
					VirtualGamepadType? virtualGamepadType = await this._httpClientService.Gamepad.GetVirtualGamepadType(baseControllerVM.ID);
					baseControllerVM2.VirtualGamepadType = virtualGamepadType;
					baseControllerVM2 = null;
				}
			});
			this._eventAggregator.GetEvent<ConfigApplied>().Subscribe(async delegate(ConfigAppliedEvent configAppliedData)
			{
				BaseControllerVM baseControllerVM3 = this.GamepadCollection.FirstOrDefault((BaseControllerVM item) => configAppliedData.ControllerId == item.ID);
				if (baseControllerVM3 != null)
				{
					BaseControllerVM baseControllerVM4 = baseControllerVM3;
					VirtualGamepadType? virtualGamepadType2 = await this._httpClientService.Gamepad.GetVirtualGamepadType(baseControllerVM3.ID);
					baseControllerVM4.VirtualGamepadType = virtualGamepadType2;
					baseControllerVM4 = null;
				}
			});
			this._eventAggregator.GetEvent<ControllerStateChanged>().Subscribe(delegate(BaseControllerVM controller)
			{
				this.GamepadCollection.Sort();
			});
			this._eventAggregator.GetEvent<RemapStateChanged>().Subscribe(async delegate(RemapStateChangedEvent remapStateChanged)
			{
				BaseControllerVM baseControllerVM5 = this.GamepadCollection.FirstOrDefault((BaseControllerVM item) => remapStateChanged.ControllerId == item.ID);
				if (baseControllerVM5 != null)
				{
					baseControllerVM5.RemapState = remapStateChanged.RemapState;
					BaseControllerVM baseControllerVM6 = baseControllerVM5;
					VirtualGamepadType? virtualGamepadType3 = await this._httpClientService.Gamepad.GetVirtualGamepadType(baseControllerVM5.ID);
					baseControllerVM6.VirtualGamepadType = virtualGamepadType3;
					baseControllerVM6 = null;
					string controllerId = remapStateChanged.ControllerId;
					BaseControllerVM currentGamepad2 = this.CurrentGamepad;
					if (controllerId == ((currentGamepad2 != null) ? currentGamepad2.ID : null))
					{
						this.OnPropertyChanged("IsCurrentGamepadRemaped");
						this.OnPropertyChanged("IsCurrentGamepadHasProfiles");
						this.OnPropertyChanged("AutoIdRemapState");
					}
				}
			});
			this.InitSlots();
			this.PrepareSlots();
			this._licensingService.OnLicenseChangedCompleted += delegate(LicenseCheckResult result, bool onlineActivation)
			{
				this.PrepareSlots();
				if (onlineActivation)
				{
					IGameProfilesService gameProfilesService = this._gameProfilesService;
					bool flag;
					if (gameProfilesService == null)
					{
						flag = false;
					}
					else
					{
						SlotInfo currentSlotInfo = gameProfilesService.CurrentSlotInfo;
						bool? flag2 = ((currentSlotInfo != null) ? new bool?(currentSlotInfo.IsAvailable) : null);
						bool flag3 = false;
						flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
					}
					if (flag)
					{
						this._gameProfilesService.CurrentSlotInfo = this.SlotsInfo[0];
					}
				}
			};
			this.GamepadCollection.CollectionChanged += this.GamepadCollectionOnCollectionChanged;
			this.AllPhysicalControllers.CollectionChanged += this.AllPhysicalControllersOnCollectionChanged;
			this.AllPhysicalControllers.CollectionItemPropertyChanged += this.AllPhysicalControllersOnCollectionItemPropertyChanged;
			ThemeManager.Instance.OnThemeChanged += delegate
			{
				this.OnPropertyChanged("CurrentGamepadSVGUriForStartPage");
				this.OnPropertyChanged("CurrentGamepadSVGUri");
			};
			if (this.ExternalDevices.IsBluetoothExist)
			{
				Tracer.TraceWrite("------", false);
				Tracer.TraceWrite("BluetoothUtils:", false);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 1);
				defaultInterpolatedStringHandler.AppendLiteral("   CanUseBluetoothInRewasd ");
				defaultInterpolatedStringHandler.AppendFormatted<bool>(BluetoothUtils.CanUseBluetoothInRewasd);
				Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 1);
				defaultInterpolatedStringHandler.AppendLiteral("   IsBluetoothExist ");
				defaultInterpolatedStringHandler.AppendFormatted<bool>(BluetoothUtils.IsBluetoothExist());
				Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 1);
				defaultInterpolatedStringHandler.AppendLiteral("   NeedInitForRewasd ");
				defaultInterpolatedStringHandler.AppendFormatted<bool>(BluetoothUtils.NeedInitForRewasd());
				Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 1);
				defaultInterpolatedStringHandler.AppendLiteral("   IsBluetoothLocalRadioInfoInvalidCod ");
				defaultInterpolatedStringHandler.AppendFormatted<bool>(BluetoothUtils.IsBluetoothLocalRadioInfoInvalidCod(false));
				Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 1);
				defaultInterpolatedStringHandler.AppendLiteral("   IsBluetoothAdapterIsNotSupported ");
				defaultInterpolatedStringHandler.AppendFormatted<bool>(BluetoothUtils.IsBluetoothAdapterIsNotSupported());
				Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 1);
				defaultInterpolatedStringHandler.AppendLiteral("   IsRebootRequired ");
				defaultInterpolatedStringHandler.AppendFormatted<bool>(BluetoothUtils.IsRebootRequired());
				Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 1);
				defaultInterpolatedStringHandler.AppendLiteral("   IsWindows10OrHigher ");
				defaultInterpolatedStringHandler.AppendFormatted<bool>(UACHelper.IsWindows10OrHigher(-1));
				Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
				Tracer.TraceWrite("------", false);
			}
		}

		private async void LoadAllBins()
		{
			await this._binDataSerialize.LoadAllBins();
			this._eventAggregator.GetEvent<AllBinsLoaded>().Publish(null);
		}

		private void InitSlots()
		{
			this.SlotsInfo = new ObservableCollection<SlotInfo>();
			this.SlotsInfo.Add(new SlotInfo(0, true, this, this._licensingService));
			this.SlotsInfo.Add(new SlotInfo(1, false, this, this._licensingService));
			this.SlotsInfo.Add(new SlotInfo(2, false, this, this._licensingService));
			this.SlotsInfo.Add(new SlotInfo(3, false, this, this._licensingService));
		}

		private void PrepareSlots()
		{
			bool flag = true;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			if (this._licensingService.IsSlotFeatureUnlocked)
			{
				flag2 = true;
				flag3 = true;
				flag4 = true;
			}
			else if (this.IsAnyGamepadConnected)
			{
				ControllerVM controllerVM = this.CurrentGamepad as ControllerVM;
				if (controllerVM != null)
				{
					flag2 = controllerVM.IsXboxElite;
					flag3 = controllerVM.IsXboxElite2;
					flag4 = controllerVM.IsXboxElite2;
				}
			}
			if (this.SlotsInfo == null)
			{
				this.InitSlots();
			}
			if (this.SlotsInfo != null)
			{
				foreach (SlotInfo slotInfo in this.SlotsInfo)
				{
					switch (slotInfo.Slot)
					{
					case 0:
						slotInfo.IsAvailable = flag;
						break;
					case 1:
						slotInfo.IsAvailable = flag2;
						break;
					case 2:
						slotInfo.IsAvailable = flag3;
						break;
					case 3:
						slotInfo.IsAvailable = flag4;
						break;
					default:
						throw new ArgumentOutOfRangeException();
					}
				}
			}
		}

		public async Task<bool> SwitchProfileToSlot(Slot slot)
		{
			return await this._httpClientService.Gamepad.SelectSlot(new SelectSlotInfo
			{
				ID = this.CurrentGamepad.ID,
				Slot = slot
			});
		}

		private void GamepadCollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.OnPropertyChanged("IsAnyGamepadConnected");
			this.OnPropertyChanged("IsSingleGamepadConnected");
			this.OnPropertyChanged("IsMultipleGamepadsConnected");
		}

		private void RemoveControllerFromAllPhysicalDevices(string ControllerID, string ContainerID)
		{
			CompositeControllerVM compositeControllerVM = this.GamepadCollection.FirstOrDefault((BaseControllerVM item) => item.ContainerIdString == ContainerID || item.ID == ControllerID) as CompositeControllerVM;
			if (compositeControllerVM != null)
			{
				compositeControllerVM.BaseControllers.ForEach(delegate(BaseControllerVM baseController)
				{
					if (baseController != null)
					{
						this.AllPhysicalControllers.Remove((BaseControllerVM item) => item.ID == baseController.ID);
					}
				});
				return;
			}
			this.AllPhysicalControllers.Remove((BaseControllerVM item) => item.ContainerIdString == ContainerID || item.ID == ControllerID);
		}

		private void AddControllerToAllPhysicalDevices(BaseControllerVM device)
		{
			CompositeControllerVM compositeControllerVM = device as CompositeControllerVM;
			if (compositeControllerVM != null)
			{
				compositeControllerVM.BaseControllers.ForEach(delegate(BaseControllerVM baseController)
				{
					if (baseController != null)
					{
						this.AllPhysicalControllers.Add(baseController);
					}
				});
				return;
			}
			this.AllPhysicalControllers.Remove((BaseControllerVM item) => item.ID == device.ID);
			this.AllPhysicalControllers.Add(device);
		}

		public async void OnControllerDisconnected(string ControllerID, string ContainerID)
		{
			AsyncLock.Releaser releaser2 = await new AsyncLock(GamepadService._refreshGamepadCollectionSemaphore).LockAsync();
			using (releaser2)
			{
				string containerID = ContainerID;
				BaseControllerVM currentGamepad = this.CurrentGamepad;
				bool flag;
				if (!(containerID == ((currentGamepad != null) ? currentGamepad.ContainerIdString : null)))
				{
					string controllerID = ControllerID;
					BaseControllerVM currentGamepad2 = this.CurrentGamepad;
					flag = controllerID == ((currentGamepad2 != null) ? currentGamepad2.ID : null);
				}
				else
				{
					flag = true;
				}
				bool changeCurrentGamepad = flag;
				this.RemoveControllerFromAllPhysicalDevices(ControllerID, ContainerID);
				if (changeCurrentGamepad)
				{
					await this.SetCurrentGamepad(null);
				}
				BaseControllerVM baseControllerVM = this.GamepadCollection.FirstOrDefault((BaseControllerVM item) => item.ContainerIdString == ContainerID || item.ID == ControllerID);
				this.GamepadCollection.Remove((BaseControllerVM item) => item.ContainerIdString == ContainerID || item.ID == ControllerID);
				if (baseControllerVM != null)
				{
					baseControllerVM.ID = "removed";
				}
				if (changeCurrentGamepad)
				{
					await this.SelectDefaultGamepad();
					if (this.CurrentGamepad == null)
					{
						await App.GameProfilesService.SetCurrentGame(null, false);
					}
				}
			}
			AsyncLock.Releaser releaser = default(AsyncLock.Releaser);
		}

		public void RefreshPromoController()
		{
			if (RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmEngineController", 1, false) == 1)
			{
				if (!this.GamepadCollection.Any((BaseControllerVM x) => x.IsPromoController))
				{
					this.GamepadCollection.Add(new PromoDeviceVM(500, "Mobile Controller"));
				}
			}
		}

		public async void OnControllerConnected(BaseControllerVM device)
		{
			AsyncLock.Releaser releaser2 = await new AsyncLock(GamepadService._refreshGamepadCollectionSemaphore).LockAsync();
			using (releaser2)
			{
				if (this.GamepadCollection.FirstOrDefault((BaseControllerVM item) => item.ID == device.ID) == null)
				{
					this.GamepadCollection.Add(device);
					if (device.ID == this._nextController)
					{
						this._nextController = null;
						this.CurrentGamepad = device;
					}
					if (this.CurrentGamepad == null)
					{
						this.CurrentGamepad = device;
					}
					this.AddControllerToAllPhysicalDevices(device);
				}
				await this.CheckSettingsForDevice(device);
			}
			AsyncLock.Releaser releaser = default(AsyncLock.Releaser);
		}

		public async void OnControllerChanged(BaseControllerVM controller)
		{
			this.GamepadCollection.Sort();
			using (await new AsyncLock(GamepadService._refreshGamepadCollectionSemaphore).LockAsync())
			{
				CompositeControllerVM compositeControllerVM = controller as CompositeControllerVM;
				if (compositeControllerVM != null)
				{
					compositeControllerVM.BaseControllers.ForEach(delegate(BaseControllerVM item)
					{
						if (item != null)
						{
							this.AllPhysicalControllers.Remove((BaseControllerVM physController) => physController.ID == item.ID);
							this.AllPhysicalControllers.Add(item);
						}
					});
				}
				this.BinDataSerialize.LoadGamepadsSlotHotkeyCollection();
				if (controller == this.CurrentGamepad)
				{
					IEventAggregator eventAggregator = App.EventAggregator;
					if (eventAggregator != null)
					{
						eventAggregator.GetEvent<CurrentControllerDataChanged>().Publish(this.CurrentGamepad);
					}
					this.RefreshCurrentGamepadSVGs();
				}
			}
			await this.CheckSettingsForDevice(controller);
		}

		private async Task CheckSettingsForDevice(BaseControllerVM device)
		{
			ControllerVM controller = device as ControllerVM;
			if (controller != null && controller.HasGamepadControllers && controller.IsControllerBatteryBlockVisible && this.GamepadsSettings.FirstOrDefault((GamepadSettings item) => item.ID == controller.ID) == null)
			{
				this.GamepadsSettings.Add(new GamepadSettings(controller.ID, controller.ControllerDisplayName, controller.FirstControllerType));
			}
			if (this.GamepadsHotkeyCollection != null)
			{
				if (!this.GamepadsHotkeyCollection.ContainsKey(device.ID))
				{
					GamepadsHotkeyDictionary gamepadsHotkeyDictionary = await App.HttpClientService.Gamepad.GetGamepadsHotkeyCollection();
					if (gamepadsHotkeyDictionary != null && gamepadsHotkeyDictionary.ContainsKey(device.ID))
					{
						this.GamepadsHotkeyCollection[device.ID] = gamepadsHotkeyDictionary[device.ID];
					}
				}
				foreach (string devID in device.ID.Split(';', StringSplitOptions.None))
				{
					GamepadsPlayerLedDictionary gamepadsUserLedCollection = this.GamepadsUserLedCollection;
					if (gamepadsUserLedCollection != null && !gamepadsUserLedCollection.ContainsKey(devID))
					{
						GamepadsPlayerLedDictionary gamepadsPlayerLedDictionary = await App.HttpClientService.Gamepad.GetGamepadsUserLedCollection();
						if (gamepadsPlayerLedDictionary != null && gamepadsPlayerLedDictionary.ContainsKey(devID))
						{
							this.GamepadsUserLedCollection[devID] = gamepadsPlayerLedDictionary[devID];
						}
					}
					devID = null;
				}
				string[] array = null;
			}
		}

		public void SelectNextConnectedControllerById(string controllerId)
		{
			this._nextController = controllerId;
		}

		private void ProcessCurrentGamepadChanged(object o)
		{
			this.PrepareSlots();
		}

		private void SetCurrentSlotForGamepad(string controllerId, Slot slot)
		{
			foreach (BaseControllerVM baseControllerVM in this.GamepadCollection)
			{
				if (baseControllerVM.ID == controllerId)
				{
					baseControllerVM.SetCurrentSlot(slot);
					break;
				}
			}
		}

		private void ProcessPhysicalSlot(SlotChangedEvent slotInfo)
		{
			this.SetCurrentSlotForGamepad(slotInfo.ControllerId, slotInfo.Slot);
			BaseControllerVM currentGamepad = this.CurrentGamepad;
			if (currentGamepad != null && currentGamepad.IsConsideredTheSameControllerByID(slotInfo.ControllerId))
			{
				IEventAggregator eventAggregator = App.EventAggregator;
				if (eventAggregator == null)
				{
					return;
				}
				eventAggregator.GetEvent<CurrentGamepadSlotChanged>().Publish(slotInfo.Slot);
			}
		}

		private async Task ReinitService()
		{
			await this._gameProfilesService.WaitForServiceInited();
			await this.InitService();
		}

		public bool IsServiceInited
		{
			get
			{
				return this._isServiceInited;
			}
			set
			{
				this.SetProperty<bool>(ref this._isServiceInited, value, "IsServiceInited");
			}
		}

		public async Task<bool> WaitForServiceInited()
		{
			Tracer.TraceWrite("GamepadService.WaitForServiceInited", false);
			ushort curWaitTime = 0;
			while (!this.IsServiceInited && curWaitTime < 60000)
			{
				await Task.Delay(50);
				curWaitTime += 50;
			}
			bool flag;
			if (curWaitTime >= 60000)
			{
				Tracer.TraceWrite("GamepadService.WaitForServiceInited wait timeout exceeded", false);
				flag = false;
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		private async Task InitService()
		{
			try
			{
				await this.RefreshGamepadCollection(null, true);
			}
			catch (Exception ex)
			{
				Tracer.TraceException(ex, "InitService");
			}
			this.IsServiceInited = true;
			this._eventAggregator.GetEvent<GamepadServiceInited>().Publish(null);
			try
			{
				if (BluetoothUtils.IsBluetoothExist() && Tracer.IsTextFileTraceEnabled)
				{
					BluetoothUtils.TraceBluetoothInfo(" - Bluetooth");
				}
			}
			catch (Exception)
			{
			}
		}

		public async Task RefreshGamepadCollection(ulong controllerID)
		{
			await this.RefreshGamepadCollection((controllerID == 0UL) ? "" : controllerID.ToString(), false);
		}

		public async Task SelectDefaultGamepad()
		{
			string prevGamepadID = RegistryHelper.GetString("Config", "CurrentGamepad", "", false);
			BaseControllerVM baseControllerVM = this.GamepadCollection.Where((BaseControllerVM x) => x.IsOnline).FirstOrDefault((BaseControllerVM item) => item.ID == prevGamepadID);
			if (baseControllerVM == null)
			{
				baseControllerVM = this.GamepadCollection.FirstOrDefault((BaseControllerVM item) => item.RemapState == 1 && item.IsOnline);
			}
			if (baseControllerVM == null)
			{
				baseControllerVM = this.GamepadCollection.FirstOrDefault((BaseControllerVM item) => ControllerTypeExtensions.IsGamepad(item.FirstControllerType) && !item.IsPromoController && item.IsOnline);
			}
			if (baseControllerVM == null)
			{
				baseControllerVM = this.GamepadCollection.FirstOrDefault((BaseControllerVM item) => ControllerTypeExtensions.IsAnyKeyboardTypesOrMouse(item.FirstControllerType) && item.IsOnline);
			}
			if (baseControllerVM == null)
			{
				baseControllerVM = this.GamepadCollection.FirstOrDefault((BaseControllerVM item) => ControllerTypeExtensions.IsGamepad(item.FirstControllerType) && item.RemapState == 1 && !item.IsPromoController && !item.IsOnline);
			}
			if (baseControllerVM == null)
			{
				baseControllerVM = this.GamepadCollection.FirstOrDefault((BaseControllerVM item) => ControllerTypeExtensions.IsAnyKeyboardTypesOrMouse(item.FirstControllerType) && item.RemapState == 1 && !item.IsOnline);
			}
			if (baseControllerVM == null)
			{
				baseControllerVM = this.GamepadCollection.FirstOrDefault((BaseControllerVM item) => ControllerTypeExtensions.IsGamepad(item.FirstControllerType) && !item.IsPromoController && !item.IsOnline);
			}
			if (baseControllerVM == null)
			{
				baseControllerVM = this.GamepadCollection.FirstOrDefault((BaseControllerVM item) => ControllerTypeExtensions.IsAnyKeyboardTypesOrMouse(item.FirstControllerType) && !item.IsOnline);
			}
			if (baseControllerVM == null)
			{
				baseControllerVM = this.GamepadCollection.FirstOrDefault((BaseControllerVM item) => ControllerTypeExtensions.IsGamepad(item.FirstControllerType));
			}
			if (baseControllerVM == null)
			{
				baseControllerVM = this.GamepadCollection.FirstOrDefault((BaseControllerVM item) => ControllerTypeExtensions.IsAnyKeyboardTypesOrMouse(item.FirstControllerType));
			}
			await this.SetCurrentGamepad((baseControllerVM != null) ? baseControllerVM : this.GamepadCollection.FirstOrDefault<BaseControllerVM>());
		}

		public async Task RefreshGamepadCollection(string ID = null, bool refreshPromoControllers = true)
		{
			AsyncLock.Releaser releaser2 = await new AsyncLock(GamepadService._refreshGamepadCollectionSemaphore).LockAsync();
			using (releaser2)
			{
				ObservableCollection<BaseControllerVM> observableCollection = await this._httpClientService.Gamepad.GetGamepadCollection();
				this.GamepadCollection.Clear();
				if (observableCollection != null)
				{
					this.GamepadCollection.AddRange(observableCollection);
				}
			}
			AsyncLock.Releaser releaser = default(AsyncLock.Releaser);
			if (!string.IsNullOrEmpty(ID))
			{
				await this.SetCurrentGamepad(this.GamepadCollection.FirstOrDefault((BaseControllerVM item) => item.IsConsideredTheSameControllerByID(ID)) ?? this.GamepadCollection.FirstOrDefault<BaseControllerVM>());
			}
			else
			{
				await this.SelectDefaultGamepad();
			}
			this.IsServiceInited = true;
			this.GetAllPhysicalControllers();
			await this.RefreshExclusiveDeviceInfo();
			if (refreshPromoControllers)
			{
				this.RefreshPromoController();
			}
		}

		public void GetAllPhysicalControllers()
		{
			this.AllPhysicalControllers.Clear();
			foreach (BaseControllerVM baseControllerVM in this.GamepadCollection)
			{
				this.AddControllerToAllPhysicalDevices(baseControllerVM);
			}
		}

		public async Task RefreshExclusiveDeviceInfo()
		{
			ExclusiveCaptureControllersInfo exclusiveCaptureControllersInfo = await this._httpClientService.Gamepad.GetExclusiveCaptureControllersInfo();
			if (exclusiveCaptureControllersInfo != null)
			{
				this.IsExclusiveCaptureControllersPresent = exclusiveCaptureControllersInfo.IsExclusiveCaptureControllersPresent;
				this.IsExclusiveCaptureProfilePresent = exclusiveCaptureControllersInfo.IsExclusiveCaptureProfilePresent;
			}
		}

		public BaseControllerVM FindControllerBySingleId(string id)
		{
			return this.AllPhysicalControllers.FirstOrDefault((BaseControllerVM c) => c.ID == id);
		}

		public async Task RestoreDefaults(List<Slot> slots)
		{
			if (this.CurrentGamepad != null)
			{
				await this._httpClientService.Gamepad.ClearSlot(new ClearSlotInfo
				{
					ID = this.CurrentGamepad.ID,
					Slots = slots
				});
			}
		}

		public bool IsAsyncRemapInProgress
		{
			get
			{
				return this._isAsyncRemapInProgress;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isAsyncRemapInProgress, value, "IsAsyncRemapInProgress"))
				{
					Tracer.TraceWrite("Setting IsAsyncRemapInProgress: " + value.ToString(), false);
					this.OnPropertyChanged("IsRemapEnabled");
				}
			}
		}

		public async Task EnableDisableRemap(bool value)
		{
			if (value)
			{
				await this.EnableRemap(true, this.CurrentGamepad.ID, false, false, true, -1, false, null, null);
				await this.RefreshCurrentRemapState();
			}
			else
			{
				BaseControllerVM currentGamepad = this.CurrentGamepad;
				await this.DisableRemap((currentGamepad != null) ? currentGamepad.ID : null, true);
			}
			this.RefreshRemapStateProperties();
			EventHandler isRemapChanged = this.IsRemapChanged;
			if (isRemapChanged != null)
			{
				isRemapChanged(this, EventArgs.Empty);
			}
		}

		public bool IsCurrentGamepadRemaped
		{
			get
			{
				return this.CurrentGamepadRemappedState == 1;
			}
			set
			{
				this.EnableDisableRemap(value);
			}
		}

		public bool IsCurrentGamepadHasProfiles
		{
			get
			{
				return this.CurrentGamepadRemappedState > 0;
			}
		}

		public string AutoIdRemapState
		{
			get
			{
				return "ToggleRemapState_" + (this.IsCurrentGamepadHasProfiles ? (this.IsCurrentGamepadRemaped ? "On" : "Off") : "NothingApplied");
			}
		}

		public RemapState CurrentGamepadRemappedState
		{
			get
			{
				if (this.CurrentGamepad != null)
				{
					return this.CurrentGamepad.RemapState;
				}
				return 0;
			}
		}

		public bool IsRemapEnabled
		{
			get
			{
				return this.IsCurrentGamepadRemaped || (this.CurrentGamepad != null && !this.IsAsyncRemapInProgress);
			}
		}

		public async Task<ushort> EnableRemap(bool showGUIMessages = false, string ID = null, bool remapNonToggledFromRelations = false, bool remapNonConnectedGamepad = false, bool changeGamepadSlot = true, int slotNumber = -1, bool force = false, EnableRemapBundle enableRemapBundle = null, EnableRemapResponse enableRemapResponse = null)
		{
			Tracer.TraceWrite("GamepadService.EnableRemap: gamepadId: " + ID, false);
			TaskAwaiter<int> taskAwaiter = this._httpClientService.Gamepad.EnableRemap(ID, remapNonToggledFromRelations).GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<int> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<int>);
			}
			if (taskAwaiter.GetResult() == 1)
			{
				await this.BinDataSerialize.LoadGamepadProfileRelations();
			}
			return 0;
		}

		public void RefreshRemapStateProperties()
		{
			this.OnPropertyChanged("CurrentGamepadActiveProfiles");
			this.OnPropertyChanged("IsCurrentGamepadRemaped");
			this.OnPropertyChanged("AutoIdRemapState");
			this.OnPropertyChanged("IsCurrentGamepadHasProfiles");
		}

		public async Task DisableRemap(string ID = null, bool changeIsRemapToggled = true)
		{
			Tracer.TraceWrite("GamepadService.DisableRemap for controllerID: " + ID, false);
			await this._httpClientService.Gamepad.DisableRemap(ID);
		}

		private void AllPhysicalControllersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
			case NotifyCollectionChangedAction.Add:
			{
				using (IEnumerator<BaseControllerVM> enumerator = e.NewItems.OfType<BaseControllerVM>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BaseControllerVM baseControllerVM = enumerator.Current;
						if (!(baseControllerVM is OfflineDeviceVM) && baseControllerVM.IsAvailiableForComposition && !this.ControllersAvailiableForComposition.Contains(baseControllerVM))
						{
							this.ControllersAvailiableForComposition.Add(baseControllerVM);
						}
						this.AddGamepadToGamepadsAuthCollection(baseControllerVM);
					}
					return;
				}
				break;
			}
			case NotifyCollectionChangedAction.Remove:
				break;
			case NotifyCollectionChangedAction.Replace:
			case NotifyCollectionChangedAction.Move:
				return;
			case NotifyCollectionChangedAction.Reset:
				goto IL_C3;
			default:
				return;
			}
			using (IEnumerator<BaseControllerVM> enumerator = e.OldItems.OfType<BaseControllerVM>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					BaseControllerVM baseControllerVM2 = enumerator.Current;
					this.ControllersAvailiableForComposition.Remove(baseControllerVM2);
					this.RemoveGamepadFromGamepadsAuthCollection(baseControllerVM2);
				}
				return;
			}
			IL_C3:
			this.ControllersAvailiableForComposition.Clear();
		}

		private void AllPhysicalControllersOnCollectionItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsInsideCompositeDevice" || e.PropertyName == "IsAvailiableForComposition")
			{
				BaseControllerVM baseControllerVM = sender as BaseControllerVM;
				if (baseControllerVM != null)
				{
					if (baseControllerVM.IsInsideCompositeDevice || !baseControllerVM.IsAvailiableForComposition)
					{
						this.ControllersAvailiableForComposition.Remove(baseControllerVM);
						return;
					}
					if (baseControllerVM.IsAvailiableForComposition && !this.ControllersAvailiableForComposition.Contains(baseControllerVM))
					{
						this.ControllersAvailiableForComposition.Add(baseControllerVM);
					}
				}
			}
		}

		public async Task AddGamepadToBlacklist(BaseControllerVM controller)
		{
			if (controller != null)
			{
				string text = controller.ControllerFamily.ToString();
				if (controller is PeripheralVM && controller.ControllerFamily == 4)
				{
					text = "NotInited";
				}
				BlackListGamepad blackListGamepad;
				if (controller is ControllerVM && controller.IsUnknownControllerType)
				{
					blackListGamepad = new BlackListGamepad(controller.ID, ((ControllerVM)controller).UnknownControllerInfoString, text);
				}
				else
				{
					blackListGamepad = new BlackListGamepad(controller.ID, controller.ControllerDisplayName, text);
				}
				blackListGamepad.ControllerFamily = controller.ControllerFamily;
				blackListGamepad.ControllerTypeEnums = controller.ControllerTypeEnums;
				this.BlacklistGamepads.Add(blackListGamepad);
				SenderGoogleAnalytics.SendMessageEvent("Blacklist", "Blacklisted", text, -1L, false);
				await this.BinDataSerialize.SaveBlacklistDevices();
			}
		}

		private void AddGamepadToGamepadsAuthCollection(BaseControllerVM device)
		{
			ControllerVM controllerVM = device as ControllerVM;
			if (controllerVM != null && controllerVM.HasGamepadControllers && controllerVM.IsControllerAuthAllowed && controllerVM.VendorId != 9649)
			{
				this.ExternalDeviceRelationsHelper.AddGamepadToGamepadsAuthCollection(controllerVM.ID, controllerVM.ControllerDisplayName, controllerVM.FirstControllerType);
			}
		}

		private void RemoveGamepadFromGamepadsAuthCollection(BaseControllerVM device)
		{
			ControllerVM controllerVM = device as ControllerVM;
			if (controllerVM != null && controllerVM.HasGamepadControllers && controllerVM.IsControllerAuthAllowed)
			{
				this.ExternalDeviceRelationsHelper.RemoveGamepadFromGamepadsAuthCollection(controllerVM.ID);
			}
		}

		public async Task RefreshCurrentRemapState()
		{
			if (this.CurrentGamepad != null)
			{
				BaseControllerVM baseControllerVM = this.CurrentGamepad;
				GamepadRemapState gamepadRemapState = await this._httpClientService.Gamepad.GetGamepadRemapState(this.CurrentGamepad.ID);
				baseControllerVM.RemapState = gamepadRemapState.RemapState;
				baseControllerVM = null;
			}
		}

		public void TraceProfileState(ulong serviceProfileId, ref REWASD_GET_PROFILE_STATE_RESPONSE profileState)
		{
			Tracer.TraceWrite("ProfileState: ---", false);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 1);
			defaultInterpolatedStringHandler.AppendLiteral("ProfileState: ProfileId 0x");
			defaultInterpolatedStringHandler.AppendFormatted<ulong>(serviceProfileId, "X");
			Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 1);
			defaultInterpolatedStringHandler.AppendLiteral("ProfileState: Enabled ");
			defaultInterpolatedStringHandler.AppendFormatted<bool>(profileState.Enabled);
			Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 1);
			defaultInterpolatedStringHandler.AppendLiteral("ProfileState: VirtualType ");
			defaultInterpolatedStringHandler.AppendFormatted<uint>(profileState.VirtualType);
			Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 1);
			defaultInterpolatedStringHandler.AppendLiteral("ProfileState: RemoteConnected ");
			defaultInterpolatedStringHandler.AppendFormatted<bool>(profileState.RemoteConnected);
			Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(46, 1);
			defaultInterpolatedStringHandler.AppendLiteral("ProfileState: BluetoothAuthenticationMethod 0x");
			defaultInterpolatedStringHandler.AppendFormatted<byte>(profileState.BluetoothAuthenticationMethod, "X");
			Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
			switch (profileState.BluetoothAuthenticationMethod)
			{
			case 1:
				Tracer.TraceWrite("ProfileState:     Legacy", false);
				break;
			case 2:
				Tracer.TraceWrite("ProfileState:     OOB", false);
				break;
			case 3:
				Tracer.TraceWrite("ProfileState:     Numeric comparision", false);
				break;
			case 4:
				Tracer.TraceWrite("ProfileState:     Passkey notification", false);
				break;
			case 5:
				Tracer.TraceWrite("ProfileState:     Passkey", false);
				break;
			default:
				Tracer.TraceWrite("ProfileState:     Unknown", false);
				break;
			}
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(52, 1);
			defaultInterpolatedStringHandler.AppendLiteral("ProfileState: BluetoothAuthenticationIoCapability 0x");
			defaultInterpolatedStringHandler.AppendFormatted<byte>(profileState.BluetoothAuthenticationIoCapability, "X");
			Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
			BluetoothUtils.BLUETOOTH_IO_CAPABILITY bluetoothAuthenticationIoCapability = profileState.BluetoothAuthenticationIoCapability;
			switch (bluetoothAuthenticationIoCapability)
			{
			case 0:
				Tracer.TraceWrite("ProfileState:     Display only", false);
				break;
			case 1:
				Tracer.TraceWrite("ProfileState:     Display Yes/No", false);
				break;
			case 2:
				Tracer.TraceWrite("ProfileState:     Keyboard only", false);
				break;
			case 3:
				Tracer.TraceWrite("ProfileState:     No input, No output", false);
				break;
			case 4:
				Tracer.TraceWrite("ProfileState:     Display and keyboard", false);
				break;
			default:
				if (bluetoothAuthenticationIoCapability != 255)
				{
					Tracer.TraceWrite("ProfileState:     Unknown", false);
				}
				else
				{
					Tracer.TraceWrite("ProfileState:     Undefined", false);
				}
				break;
			}
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(52, 1);
			defaultInterpolatedStringHandler.AppendLiteral("ProfileState: BluetoothAuthenticationRequirements 0x");
			defaultInterpolatedStringHandler.AppendFormatted<byte>(profileState.BluetoothAuthenticationRequirements, "X");
			Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
			BluetoothUtils.BLUETOOTH_AUTHENTICATION_REQUIREMENTS bluetoothAuthenticationRequirements = profileState.BluetoothAuthenticationRequirements;
			switch (bluetoothAuthenticationRequirements)
			{
			case 0:
				Tracer.TraceWrite("ProfileState:     BLUETOOTH_MITM_ProtectionNotRequired", false);
				break;
			case 1:
				Tracer.TraceWrite("ProfileState:     BLUETOOTH_MITM_ProtectionRequired", false);
				break;
			case 2:
				Tracer.TraceWrite("ProfileState:     BLUETOOTH_MITM_ProtectionNotRequiredBonding", false);
				break;
			case 3:
				Tracer.TraceWrite("ProfileState:     BLUETOOTH_MITM_ProtectionRequiredBonding", false);
				break;
			case 4:
				Tracer.TraceWrite("ProfileState:     BLUETOOTH_MITM_ProtectionNotRequiredGeneralBonding", false);
				break;
			case 5:
				Tracer.TraceWrite("ProfileState:     BLUETOOTH_MITM_ProtectionRequiredGeneralBonding", false);
				break;
			default:
				if (bluetoothAuthenticationRequirements != 255)
				{
					Tracer.TraceWrite("ProfileState:     Unknown", false);
				}
				else
				{
					Tracer.TraceWrite("ProfileState:     BLUETOOTH_MITM_ProtectionNotDefined", false);
				}
				break;
			}
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(47, 1);
			defaultInterpolatedStringHandler.AppendLiteral("ProfileState: BluetoothAuthenticationPasskey 0x");
			defaultInterpolatedStringHandler.AppendFormatted<uint>(profileState.BluetoothAuthenticationPasskey, "X");
			Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(46, 1);
			defaultInterpolatedStringHandler.AppendLiteral("ProfileState: BluetoothAuthenticationStatus 0x");
			defaultInterpolatedStringHandler.AppendFormatted<uint>(profileState.BluetoothAuthenticationStatus, "X");
			Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
			Tracer.TraceWrite("ProfileState: ---", false);
		}

		public async Task ReCompileSteamLizardProfile()
		{
			await this._httpClientService.Gamepad.ReCompileSteamLizardProfile();
			await this.RefreshExclusiveDeviceInfo();
		}

		public async Task RefreshInputDevices()
		{
			await this._httpClientService.Gamepad.RefreshInputDevices();
		}

		private GamepadProfilesCollection _gamepadProfileRelations = new GamepadProfilesCollection();

		private AutoGamesDetectionGamepadProfilesCollection _autoGamesDetectionGamepadProfilesCollection = new AutoGamesDetectionGamepadProfilesCollection();

		private ExternalDevicesCollection _externalDevices = new ExternalDevicesCollection();

		private ObservableCollection<ExternalClient> _externalClients = new ObservableCollection<ExternalClient>();

		private ExternalDeviceRelationsHelper _externalDeviceRelationsHelper;

		private BinDataSerialize _binDataSerialize;

		private BaseControllerVM _currentGamepad;

		private ControllerVM _currentVirtualGamepad;

		private IGameProfilesService _gameProfilesService;

		private IDeviceDetectionService _deviceDetectionService;

		private IUserSettingsService _userSettingsService;

		private IEventAggregator _eventAggregator;

		private ILicensingService _licensingService;

		private IHttpClientService _httpClientService;

		private IConfigFileService _configFileService;

		public static readonly AsyncSemaphore _refreshGamepadCollectionSemaphore = new AsyncSemaphore(1);

		public static readonly AsyncSemaphore _setCurrentGamepadSemaphore = new AsyncSemaphore(1);

		private GamepadsHotkeyDictionary _gamepadsSlotHotkeyCollection = new GamepadsHotkeyDictionary();

		private GamepadsPlayerLedDictionary _gamepadsUserLedCollection = new GamepadsPlayerLedDictionary();

		private CompositeDevices _compositeDevices = new CompositeDevices();

		private PeripheralDevices _peripheralDevices = new PeripheralDevices();

		private bool _isCurrentGamepadBackShown;

		private DelegateCommand<bool?> _FlipGamepad;

		private string _nextController;

		private bool _isServiceInited;

		private const ushort INIT_WAIT_TIMEOUT = 60000;

		private bool _isAsyncRemapInProgress;
	}
}
