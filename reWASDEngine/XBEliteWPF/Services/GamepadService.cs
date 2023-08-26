using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DiscSoft.NET.Common.AdminRightsFeatures;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using DiscSoftReWASDServiceNamespace;
using Microsoft.Win32.SafeHandles;
using Prism.Events;
using reWASDCommon;
using reWASDCommon.Infrastructure;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Infrastructure.LED;
using reWASDCommon.Interfaces;
using reWASDCommon.MacroCompilers;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using reWASDCommon.Utils;
using reWASDCommon._3dPartyManufacturersAPI;
using reWASDEngine;
using reWASDEngine.Utils.Extensions;
using XBEliteWPF.DataModels;
using XBEliteWPF.DataModels.CompositeDevicesCollection;
using XBEliteWPF.DataModels.ControllerProfileInfo;
using XBEliteWPF.DataModels.GamepadActiveProfiles;
using XBEliteWPF.DataModels.GamepadSlotHotkeyCollection;
using XBEliteWPF.DataModels.InitializedDevicesCollection;
using XBEliteWPF.DataModels.PeripheralDevicesCollection;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.Controller;
using XBEliteWPF.Infrastructure.ExternalDeviceRelations;
using XBEliteWPF.Infrastructure.ExternalDevices;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Infrastructure.LED;
using XBEliteWPF.Infrastructure.reWASDMapping.RewasduserCommands;
using XBEliteWPF.License.Licensing.ComStructures;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtilModel;

namespace XBEliteWPF.Services
{
	public class GamepadService : IGamepadService, IServiceInitedAwaitable, IAdminOperationsDeciderContainer
	{
		public SortableObservableCollection<BaseControllerVM> AllPhysicalControllers { get; } = new SortableObservableCollection<BaseControllerVM>(true);

		public ObservableCollection<BaseControllerVM> GamepadCollection { get; set; } = new ObservableCollection<BaseControllerVM>();

		public List<Tuple<ulong, uint>> DuplicateGamepadCollection { get; } = new List<Tuple<ulong, uint>>();

		public List<Tuple<ulong, SimpleDeviceInfo>> SimpleDeviceInfoList { get; } = new List<Tuple<ulong, SimpleDeviceInfo>>();

		public ObservableCollection<ControllerVM> VirtualGamepadCollection { get; } = new ObservableCollection<ControllerVM>();

		public ObservableCollection<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> ServiceProfilesCollection { get; } = new ObservableCollection<Wrapper<REWASD_CONTROLLER_PROFILE_EX>>();

		public ObservableCollection<REWASD_CONTROLLER_PROFILE_CACHE> CachedProfilesCollection { get; } = new ObservableCollection<REWASD_CONTROLLER_PROFILE_CACHE>();

		private ObservableCollection<REWASD_STEAM_CONTROLLER_PROFILE_CACHE> CachedSteamProfilesCollection { get; } = new ObservableCollection<REWASD_STEAM_CONTROLLER_PROFILE_CACHE>();

		private ObservableCollection<REWASD_HIDDEN_APPLIED_PROFILE_CACHE> CachedHiddenAppliedProfilesCollection { get; } = new ObservableCollection<REWASD_HIDDEN_APPLIED_PROFILE_CACHE>();

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

		public EngineControllersWpapper EngineControllersWpapper
		{
			get
			{
				return this._engineControllersWpapper;
			}
		}

		public AdminOperationsDecider AdminOperationsDecider { get; set; }

		public bool IsExclusiveCaptureControllersPresent { get; set; }

		public bool IsExclusiveCaptureProfilePresent { get; set; }

		public bool IsAnyGamepadConnected
		{
			get
			{
				return this.GamepadCollection.Count > 0;
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

		public bool IsAnyVirtualGamepadConnected
		{
			get
			{
				return this.VirtualGamepadCollection.Count > 0;
			}
		}

		public bool IsSingleVirtualGamepadConnected
		{
			get
			{
				return this.VirtualGamepadCollection.Count == 1;
			}
		}

		public bool IsMultipleVirtualGamepadsConnected
		{
			get
			{
				return this.VirtualGamepadCollection.Count > 1;
			}
		}

		public event GamepadStateChanged OnGamepadStateChanged;

		public event BatteryLevelChangedHandler OnBatteryLevelChanged;

		public event BatteryLevelChangedHandler OnBatteryLevelChangedUI;

		public event ConfigAppliedToSlotHandler OnConfigAppliedToSlot;

		public event RemapStateChangedHandler OnRemapStateChanged;

		public event ControllerAddedHandler OnControllerAdded;

		public event ControllerChangedHandler OnControllerChanged;

		public event ControllerRemovedHandlerForProxy OnControllerRemovedForProxy;

		public event AllControllersRemovedHandler OnAllControllersRemoved;

		public event ControllerSlotChangedHandler OnPhysicalSlotChanged;

		public event SetBatteryNotificationEventHandler SetBatteryNotificationEvent;

		public REWASD_CONTROLLER_PROFILE_EX GetProfileEx(ulong controllerID)
		{
			ObservableCollection<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> serviceProfilesCollection = this.ServiceProfilesCollection;
			Wrapper<REWASD_CONTROLLER_PROFILE_EX> wrapper = ((serviceProfilesCollection != null) ? serviceProfilesCollection.FindWrapperByControllerID(controllerID) : null);
			if (wrapper == null)
			{
				return null;
			}
			return wrapper.Value;
		}

		public REWASD_CONTROLLER_PROFILE_EX GetProfileEx(string ID)
		{
			ObservableCollection<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> serviceProfilesCollection = this.ServiceProfilesCollection;
			Wrapper<REWASD_CONTROLLER_PROFILE_EX> wrapper = ((serviceProfilesCollection != null) ? serviceProfilesCollection.FindWrapperByID(ID) : null);
			if (wrapper == null)
			{
				return null;
			}
			return wrapper.Value;
		}

		public REWASD_CONTROLLER_PROFILE_EX GetProfileExByServiceProfileId(ulong serviceProfileId)
		{
			ObservableCollection<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> serviceProfilesCollection = this.ServiceProfilesCollection;
			Wrapper<REWASD_CONTROLLER_PROFILE_EX> wrapper = ((serviceProfilesCollection != null) ? serviceProfilesCollection.FindWrapperByServiceProfileId(serviceProfileId) : null);
			if (wrapper == null)
			{
				return null;
			}
			return wrapper.Value;
		}

		public bool IsGamepadNotPresent(ulong id)
		{
			return !this.SimpleDeviceInfoList.Exists((Tuple<ulong, SimpleDeviceInfo> x) => x.Item1 == id);
		}

		public bool IsGamepadOnline(ulong id)
		{
			return !this.IsGamepadNotPresent(id);
		}

		public bool IsGamepadInNothingAppliedState(BaseControllerVM gamepad)
		{
			if (gamepad == null)
			{
				return true;
			}
			bool flag2;
			if (!this.IsAsyncRemapInProgress)
			{
				if (!gamepad.IsUnknownControllerType)
				{
					GamepadProfiles gamepadActiveProfiles = this.GetGamepadActiveProfiles(gamepad);
					bool flag;
					if (gamepadActiveProfiles == null)
					{
						flag = false;
					}
					else
					{
						SlotProfilesDictionary slotProfiles = gamepadActiveProfiles.SlotProfiles;
						int? num = ((slotProfiles != null) ? new int?(slotProfiles.Count) : null);
						int num2 = 0;
						flag = (num.GetValueOrDefault() > num2) & (num != null);
					}
					if (flag)
					{
						flag2 = true;
						goto IL_65;
					}
				}
				flag2 = this.IsGamepadRemaped(gamepad);
			}
			else
			{
				flag2 = false;
			}
			IL_65:
			bool flag3 = this.IsGamepadRemaped(gamepad);
			return !flag2 && !flag3;
		}

		public bool IsAnyServiceProfileWithExternalBluetooth()
		{
			if (this.ServiceProfilesCollection != null)
			{
				return this.ServiceProfilesCollection.Any((Wrapper<REWASD_CONTROLLER_PROFILE_EX> x) => x.Value.IsExternalBluetoothPresent());
			}
			return false;
		}

		public bool IsAnyServiceProfileWithExternalBluetoothAdapter()
		{
			if (this.ServiceProfilesCollection != null)
			{
				return this.ServiceProfilesCollection.Any((Wrapper<REWASD_CONTROLLER_PROFILE_EX> x) => x.Value.IsExternalBluetoothAdapterPresent());
			}
			return false;
		}

		public bool IsAnyServiceProfileWithExternalSerialPort(string externalDeviceTypeId = "")
		{
			if (!string.IsNullOrEmpty(externalDeviceTypeId))
			{
				return this.ServiceProfilesCollection != null && this.ServiceProfilesCollection.Any((Wrapper<REWASD_CONTROLLER_PROFILE_EX> x) => x.Value.IsExternalDeviceEqual(externalDeviceTypeId));
			}
			if (this.ServiceProfilesCollection != null)
			{
				return this.ServiceProfilesCollection.Any((Wrapper<REWASD_CONTROLLER_PROFILE_EX> x) => x.Value.IsExternalDeviceWithSerialPortPresent());
			}
			return false;
		}

		public bool IsAnyServiceProfileWithExternalDevice(ExternalDeviceType externalDeviceType)
		{
			if (externalDeviceType != null)
			{
				return externalDeviceType - 1 <= 3 && this.IsAnyServiceProfileWithExternalSerialPort("");
			}
			return this.IsAnyServiceProfileWithExternalBluetoothAdapter();
		}

		public bool IsAnyServiceProfileWithExternalDevice(string externalDeviceTypeId)
		{
			return this.IsAnyServiceProfileWithExternalBluetoothAdapter() || this.IsAnyServiceProfileWithExternalSerialPort(externalDeviceTypeId);
		}

		public List<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> GetServiceProfileWithExternalBluetoothAdapter()
		{
			ObservableCollection<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> serviceProfilesCollection = this.ServiceProfilesCollection;
			if (serviceProfilesCollection == null)
			{
				return null;
			}
			return serviceProfilesCollection.Where((Wrapper<REWASD_CONTROLLER_PROFILE_EX> x) => x.Value.IsExternalBluetoothPresent() && !x.Value.IsExternalDeviceWithSerialPortPresent()).ToList<Wrapper<REWASD_CONTROLLER_PROFILE_EX>>();
		}

		public List<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> GetServiceProfileWithExternalBluetooth()
		{
			ObservableCollection<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> serviceProfilesCollection = this.ServiceProfilesCollection;
			if (serviceProfilesCollection == null)
			{
				return null;
			}
			return serviceProfilesCollection.Where((Wrapper<REWASD_CONTROLLER_PROFILE_EX> x) => x.Value.IsExternalBluetoothPresent()).ToList<Wrapper<REWASD_CONTROLLER_PROFILE_EX>>();
		}

		public List<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> GetServiceProfileWithExternalSerialPort()
		{
			ObservableCollection<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> serviceProfilesCollection = this.ServiceProfilesCollection;
			if (serviceProfilesCollection == null)
			{
				return null;
			}
			return serviceProfilesCollection.Where((Wrapper<REWASD_CONTROLLER_PROFILE_EX> x) => x.Value.IsExternalDeviceWithSerialPortPresent()).ToList<Wrapper<REWASD_CONTROLLER_PROFILE_EX>>();
		}

		public List<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> GetServiceProfileWithExternalDevice(ExternalDeviceType externalDeviceType)
		{
			if (externalDeviceType == null)
			{
				return this.GetServiceProfileWithExternalBluetoothAdapter();
			}
			if (externalDeviceType - 1 > 3)
			{
				return null;
			}
			return this.GetServiceProfileWithExternalSerialPort();
		}

		public GamepadProfilesCollection GamepadProfileRelations { get; set; } = new GamepadProfilesCollection();

		public AutoGamesDetectionGamepadProfilesCollection AutoGamesDetectionGamepadProfileRelations { get; set; } = new AutoGamesDetectionGamepadProfilesCollection();

		public ExternalDevicesCollection ExternalDevices { get; set; } = new ExternalDevicesCollection();

		public ObservableCollection<ExternalClient> ExternalClients { get; set; } = new ObservableCollection<ExternalClient>();

		public ObservableCollection<BlackListGamepad> BlacklistGamepads { get; set; } = new ObservableCollection<BlackListGamepad>();

		public ObservableCollection<GamepadSettings> GamepadsSettings { get; set; } = new ObservableCollection<GamepadSettings>();

		public GamepadProfiles GetGamepadActiveProfiles(BaseControllerVM gamepad)
		{
			if (gamepad == null)
			{
				return null;
			}
			if (this.GamepadProfileRelations.ContainsKey(gamepad.ID))
			{
				this.GamepadProfileRelations[gamepad.ID].UpdateControllerInfosChainIfRequired(gamepad);
			}
			return this.GamepadProfileRelations.TryGetValue(gamepad.ID);
		}

		public RemapState GetRemapState(string gamepadId)
		{
			RemapState remapState = 0;
			GamepadProfiles gamepadProfiles = this.GamepadProfileRelations.TryGetValue(gamepadId);
			bool flag;
			if (gamepadProfiles == null)
			{
				flag = false;
			}
			else
			{
				SlotProfilesDictionary slotProfiles = gamepadProfiles.SlotProfiles;
				int? num = ((slotProfiles != null) ? new int?(slotProfiles.Count) : null);
				int num2 = 0;
				flag = (num.GetValueOrDefault() > num2) & (num != null);
			}
			if (flag)
			{
				if (this.ServiceProfilesCollection.ContainsProfileForID(gamepadId))
				{
					remapState = 1;
				}
				else
				{
					remapState = 2;
				}
			}
			return remapState;
		}

		public VirtualGamepadType? GetVirtualGamepadType(BaseControllerVM controller)
		{
			Wrapper<REWASD_CONTROLLER_PROFILE_EX> wrapper = this.ServiceProfilesCollection.FindWrapperByID(controller.ID);
			if (wrapper == null)
			{
				return null;
			}
			uint virtualType = wrapper.Value.Profiles[controller.CurrentSlot].VirtualType;
			if (virtualType == 0U)
			{
				return null;
			}
			return ControllerTypeExtensions.ConvertEnumToVirtualType(ControllerTypeExtensions.ConvertPhysicalTypeToEnum(0, virtualType, 0UL));
		}

		public ObservableCollection<SlotInfo> SlotsInfo { get; private set; }

		public GamepadsHotkeyDictionary GamepadsHotkeyCollection { get; set; } = new GamepadsHotkeyDictionary();

		public GamepadsPlayerLedDictionary GamepadsUserLedCollection { get; set; } = new GamepadsPlayerLedDictionary();

		public CompositeDevices CompositeDevices { get; set; } = new CompositeDevices();

		public PeripheralDevices PeripheralDevices { get; set; } = new PeripheralDevices();

		public InitializedDevices InitializedDevices { get; set; } = new InitializedDevices();

		public bool IgnoreServiceSlotChangedCallbacks { get; set; }

		public GamepadService(IXBServiceCommunicator sc, IGameProfilesService gps, IEventAggregator ea, IAdminOperations ao, ILicensingService ls, IConfigFileService cfs, IUserSettingsService uss)
		{
			Tracer.TraceWrite("Constructor for GamepadService", false);
			this._eventAggregator = ea;
			this._gameProfilesService = gps;
			this.AdminOperationsDecider = (AdminOperationsDecider)ao;
			this._configFileService = cfs;
			this._licensingService = ls;
			this._serviceVersionMajor = byte.MaxValue;
			this._serviceVersionMinor = byte.MaxValue;
			this._xbServiceCommunicator = sc;
			this._userSettingsService = uss;
			Engine.GamepadService = this;
			BaseRewasdUserCommandInitializer.Init();
			this._binDataSerialize = new BinDataSerialize(this, gps, sc);
			this._binDataSerialize.LoadAllBins();
			this.GamepadCollection.ForEach(delegate(BaseControllerVM item)
			{
				if (!(item is CompositeControllerVM))
				{
					this.AddGamepadToPhysicalCollection(item);
				}
			});
			this._externalDeviceRelationsHelper = new ExternalDeviceRelationsHelper(this, gps);
			this._engineControllersWpapper = new EngineControllersWpapper();
			this.ReinitService();
			this._eventAggregator.GetEvent<GamepadChanged>().Subscribe(new Action<WindowMessageEvent>(this.ProcessGamepadChanged));
			this._eventAggregator.GetEvent<ServiceProfilesChanged>().Subscribe(new Action<WindowMessageEvent>(this.ProcessProfilesChanged));
			this._eventAggregator.GetEvent<ServiceProfileStateChanged>().Subscribe(new Action<WindowMessageEvent>(this.ProcessProfileStateChanged));
			this._eventAggregator.GetEvent<GyroCalibrationFinished>().Subscribe(new Action<WindowMessageEvent>(this.ProcessGyroCalibrationFinished));
			this._eventAggregator.GetEvent<RemapOff>().Subscribe(new Action<ulong?>(this.OnRemapOffExternalEvent));
			this._eventAggregator.GetEvent<PhysSlot1Selected>().Subscribe(delegate(WindowMessageEvent payload)
			{
				this.ProcessPhysicalSlot(payload, 0);
			});
			this._eventAggregator.GetEvent<PhysSlot2Selected>().Subscribe(delegate(WindowMessageEvent payload)
			{
				this.ProcessPhysicalSlot(payload, 1);
			});
			this._eventAggregator.GetEvent<PhysSlot3Selected>().Subscribe(delegate(WindowMessageEvent payload)
			{
				this.ProcessPhysicalSlot(payload, 2);
			});
			this._eventAggregator.GetEvent<PhysSlot4Selected>().Subscribe(delegate(WindowMessageEvent payload)
			{
				this.ProcessPhysicalSlot(payload, 3);
			});
			this._eventAggregator.GetEvent<MacroCommandBroadcast>().Subscribe(new Action<WindowMessageEvent>(this.ProcessMacroCommand));
			this._eventAggregator.GetEvent<CompositeDevicesChanged>().Subscribe(delegate(WindowMessageEvent payload)
			{
				this.BinDataSerialize.LoadCompositeDevicesCollection(true);
			});
			this._eventAggregator.GetEvent<InitializedPeripheralDevicesChanged>().Subscribe(delegate(WindowMessageEvent payload)
			{
				this.BinDataSerialize.LoadPeripheralDevicesCollection(true);
			});
			this._eventAggregator.GetEvent<GamepadInitializationChanged>().Subscribe(new Action<BaseControllerVM>(this.OnInitializedController));
			this._eventAggregator.GetEvent<AmiiboUnloadByUidBroadcast>().Subscribe(new Action<WindowMessageEvent>(this.ProcessAmiiboUnloadByUid));
			this.InitSlots();
			this.PrepareSlots();
			this._licensingService.OnLicenseChangedCompleted += async delegate(LicenseCheckResult result, bool onlineActivation)
			{
				this.PrepareSlots();
				if (onlineActivation)
				{
					await this.EnableRemap(false, null, false, false, true, -1, false, false, true, null, null);
					this.GamepadCollection.ForEach(async delegate(BaseControllerVM controller)
					{
						if (!this.SlotsInfo[controller.CurrentSlot].IsAvailable)
						{
							await this.SwitchProfileToSlot(controller, 0);
						}
					});
				}
			};
			uss.OnLEDSettingsChanged += async delegate([Nullable(2)] object s, EventArgs o)
			{
				await Engine.LEDService.ChangeLedSettingsEnabled();
				foreach (BaseControllerVM controller in this.GamepadCollection)
				{
					ILEDService ledservice = Engine.LEDService;
					BaseControllerVM baseControllerVM = controller;
					GamepadProfiles gamepadProfiles = this.GamepadProfileRelations.TryGetValue(controller.ID);
					await ledservice.ApplyLEDsToControllerAccordingToSettings(baseControllerVM, (gamepadProfiles != null) ? gamepadProfiles.SlotProfiles.TryGetValue(controller.CurrentSlot) : null, new Slot?(controller.CurrentSlot), 0, false, false);
					await this.RefreshGamepadStatesAndReactOnBattery(controller, true);
					controller = null;
				}
				IEnumerator<BaseControllerVM> enumerator = null;
			};
			this._licensingService.OnLicenseFailed += delegate
			{
				this.DisableRemap(null, true);
			};
			this.GamepadCollection.CollectionChanged += this.GamepadCollectionOnCollectionChanged;
			this.ServiceProfilesCollection.CollectionChanged += this.OnServiceProfilesCollectionCollectionChanged;
			this.StartBatteryTimer();
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

		private async void OnRemapOffExternalEvent(ulong? profileID)
		{
			if (Engine.UserSettingsService.IsLedSettingsEnabled)
			{
				await this._gameProfilesService.WaitForServiceInited();
				if (profileID != null)
				{
					Func<ushort, bool> <>9__1;
					BaseControllerVM baseControllerVM = this.GamepadCollection.FirstOrDefault(delegate(BaseControllerVM item)
					{
						IEnumerable<ushort> serviceProfileIDs = item.ServiceProfileIDs;
						Func<ushort, bool> func;
						if ((func = <>9__1) == null)
						{
							func = (<>9__1 = delegate(ushort pIDs)
							{
								ulong num = (ulong)pIDs;
								ulong? profileID2 = profileID;
								return (num == profileID2.GetValueOrDefault()) & (profileID2 != null);
							});
						}
						return serviceProfileIDs.Any(func);
					});
					if (baseControllerVM != null)
					{
						foreach (ControllerVM controllerVM in baseControllerVM.GetLEDSupportedGamepads())
						{
							Engine.LEDService.ResetGamepadColor(new LEDDeviceInfo(controllerVM.ControllerId, controllerVM.Type, 0), true, true, true, false);
						}
					}
				}
			}
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
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (this._licensingService.IsSlotFeatureUnlocked)
			{
				flag = true;
				flag2 = true;
				flag3 = true;
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
						slotInfo.IsAvailable = true;
						break;
					case 1:
						slotInfo.IsAvailable = flag;
						break;
					case 2:
						slotInfo.IsAvailable = flag2;
						break;
					case 3:
						slotInfo.IsAvailable = flag3;
						break;
					default:
						throw new ArgumentOutOfRangeException();
					}
				}
			}
		}

		public uint GetSlotsWrapperIdForGamepadId(string gamepadId)
		{
			REWASD_CONTROLLER_PROFILE_EX profileEx = this.GetProfileEx(gamepadId);
			if (profileEx != null)
			{
				return profileEx.SlotsWrapperId;
			}
			return 0U;
		}

		private void SaveSlot(string shortID, Slot slot)
		{
			try
			{
				RegistryHelper.SetValue("Controllers\\" + shortID, "CurrentSlot", slot);
			}
			catch (Exception)
			{
			}
		}

		public async Task<bool> SwitchProfileToSlot(BaseControllerVM controller, Slot slot)
		{
			uint slotsWrapperId = Engine.GamepadService.GetSlotsWrapperIdForGamepadId(controller.ID);
			bool flag;
			if (slotsWrapperId == 0U)
			{
				this.SaveSlot(controller.ShortID, slot);
				controller.CurrentSlot = slot;
				this.SendPhysicalSlotChanged(controller, slot);
				flag = true;
			}
			else
			{
				bool isSlotFeatureUnlocked = this._licensingService.IsSlotFeatureUnlocked;
				Wrapper<REWASD_CONTROLLER_PROFILE_EX> wrapper = this.ServiceProfilesCollection.FirstOrDefault((Wrapper<REWASD_CONTROLLER_PROFILE_EX> sp) => sp.Value.SlotsWrapperId == slotsWrapperId);
				if (wrapper == null)
				{
					flag = false;
				}
				else if (wrapper.Value.ServiceProfileIds[slot] == 0)
				{
					flag = false;
				}
				else if (!isSlotFeatureUnlocked && !this.IsSlotValid(slot, false, wrapper.Value.ControllerPhysicalTypes))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(86, 1);
					defaultInterpolatedStringHandler.AppendLiteral("SwitchProfileToSlot: switch to slot ");
					defaultInterpolatedStringHandler.AppendFormatted<Slot>(slot);
					defaultInterpolatedStringHandler.AppendLiteral(" FAILED. Slot is not valid due to license feature.");
					Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
					flag = false;
				}
				else
				{
					ushort[] array = new ushort[4];
					bool[] array2 = new bool[4];
					int num = 0;
					for (int i = 0; i < 4; i++)
					{
						if (i != slot && num < 4)
						{
							array[num] = wrapper.Value.ServiceProfileIds[i];
							array2[num] = false;
							num++;
						}
					}
					array[3] = wrapper.Value.ServiceProfileIds[slot];
					array2[3] = true;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 1);
					defaultInterpolatedStringHandler.AppendLiteral("SwitchProfileToSlot: switch to Slot: ");
					defaultInterpolatedStringHandler.AppendFormatted<Slot>(slot);
					Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
					bool flag2 = await this._xbServiceCommunicator.SetProfilesActiveState(array, array2, false, null, null);
					if (flag2)
					{
						this.SaveSlot(controller.ShortID, slot);
						if (!controller.IsOnline)
						{
							this.SendPhysicalSlotChanged(controller, slot);
						}
					}
					flag = flag2;
				}
			}
			return flag;
		}

		public bool IsSlotValid(Slot slot, bool isSlotFeatureUnlocked, uint[] controllerPhysicalTypes)
		{
			if (slot == 0 || isSlotFeatureUnlocked)
			{
				return true;
			}
			if (slot == 1)
			{
				if (controllerPhysicalTypes.Any((uint x) => x == 3U || x == 4U))
				{
					return true;
				}
			}
			if (slot > 1)
			{
				if (controllerPhysicalTypes.Any((uint x) => x == 4U))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsSlotValid(Slot slot, bool isSlotFeatureUnlocked, ControllerTypeEnum[] controllerTypes)
		{
			uint[] array = ControllerTypeExtensions.ConvertEnumsToPhysicalTypes(controllerTypes);
			return this.IsSlotValid(slot, isSlotFeatureUnlocked, array);
		}

		private void GamepadCollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				using (IEnumerator enumerator = e.NewItems.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						BaseControllerVM baseControllerVM = (BaseControllerVM)obj;
						ControllerAddedHandler onControllerAdded = this.OnControllerAdded;
						if (onControllerAdded != null)
						{
							onControllerAdded(baseControllerVM);
						}
					}
					return;
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				Tracer.TraceWrite("GamepadCollectionOnCollectionChanged removed item", false);
				using (IEnumerator enumerator = e.OldItems.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj2 = enumerator.Current;
						BaseControllerVM baseControllerVM2 = (BaseControllerVM)obj2;
						CompositeDevice compositeDevice = this.CompositeDevices.FindCompositeForSimple(baseControllerVM2.ID);
						bool flag = baseControllerVM2.IsCompositeDevice || (!baseControllerVM2.IsCompositeDevice && compositeDevice == null);
						ControllerRemovedHandlerForProxy onControllerRemovedForProxy = this.OnControllerRemovedForProxy;
						if (onControllerRemovedForProxy != null)
						{
							onControllerRemovedForProxy(baseControllerVM2, flag);
						}
					}
					return;
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				AllControllersRemovedHandler onAllControllersRemoved = this.OnAllControllersRemoved;
				if (onAllControllersRemoved == null)
				{
					return;
				}
				onAllControllersRemoved();
			}
		}

		private async void ProcessGamepadChanged(WindowMessageEvent payload)
		{
			Tracer.TraceWrite("ProcessGamepadChanged", false);
			if (!UACHelper.IsWindows10OrHigher(-1))
			{
				await Task.Delay(2000);
			}
			else if (this.DuplicateGamepadCollection.Count > 0)
			{
				await Task.Delay(1000);
			}
			else
			{
				await Task.Delay(500);
			}
			await this.RefreshGamepadCollection(null);
			if (this.IsExclusiveCaptureProfilePresent)
			{
				this.CheckExclusiveCaptureProfilesOnGamepadChanged();
			}
		}

		private async Task<Slot?> RefreshAndGetSlot(BaseControllerVM controller)
		{
			Slot? slot;
			if (controller == null)
			{
				slot = null;
			}
			else
			{
				Slot currentSlot = controller.CurrentSlot;
				Slot slot2 = currentSlot;
				Slot slot3 = await controller.GetCurrentSlot();
				if (slot2 != slot3)
				{
					this.SendPhysicalSlotChanged(controller, controller.CurrentSlot);
				}
				slot = new Slot?(controller.CurrentSlot);
			}
			return slot;
		}

		private async void ProcessProfilesChanged(WindowMessageEvent payload)
		{
			Tracer.TraceWrite("ProcessProfilesChanged", false);
			ObservableCollection<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> oldServiceProfilesCollection = new ObservableCollection<Wrapper<REWASD_CONTROLLER_PROFILE_EX>>(this.ServiceProfilesCollection);
			await this.RefreshServiceProfiles();
			List<REWASD_CONTROLLER_PROFILE_EX> list = await this._xbServiceCommunicator.GetExclusiveCaptureProfilesList();
			this.IsExclusiveCaptureProfilePresent = list.Count > 0;
			List<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> newProfileWrappers = this.ServiceProfilesCollection.Except(oldServiceProfilesCollection).ToList<Wrapper<REWASD_CONTROLLER_PROFILE_EX>>();
			List<REWASD_CONTROLLER_INFO> list2 = await this._xbServiceCommunicator.GetControllersList(true, false, false);
			List<REWASD_CONTROLLER_INFO> hiddenControlers = UtilsCommon.GetHiddenControllers(list2);
			using (List<Wrapper<REWASD_CONTROLLER_PROFILE_EX>>.Enumerator enumerator = oldServiceProfilesCollection.Except(this.ServiceProfilesCollection).ToList<Wrapper<REWASD_CONTROLLER_PROFILE_EX>>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Wrapper<REWASD_CONTROLLER_PROFILE_EX> wrapper2 = enumerator.Current;
					if (wrapper2.Value.Profiles.Any((REWASD_CONTROLLER_PROFILE p) => REWASD_CONTROLLER_PROFILE_Extensions.IsSlotWithUserConfig(p)))
					{
						BaseControllerVM baseControllerVM = this.GamepadCollection.FirstOrDefault((BaseControllerVM global) => global.ID == wrapper2.Value.GetID(hiddenControlers));
						if (baseControllerVM != null)
						{
							GamepadProfiles gamepadProfiles = this.GamepadProfileRelations.TryGetValue(wrapper2.Value.GetID(hiddenControlers));
							bool flag;
							if (gamepadProfiles == null)
							{
								flag = false;
							}
							else
							{
								SlotProfilesDictionary slotProfiles = gamepadProfiles.SlotProfiles;
								int? num = ((slotProfiles != null) ? new int?(slotProfiles.Count) : null);
								int num2 = 0;
								flag = (num.GetValueOrDefault() > num2) & (num != null);
							}
							if (flag)
							{
								RemapStateChangedHandler onRemapStateChanged = this.OnRemapStateChanged;
								if (onRemapStateChanged != null)
								{
									onRemapStateChanged(baseControllerVM, 2);
								}
							}
							else
							{
								RemapStateChangedHandler onRemapStateChanged2 = this.OnRemapStateChanged;
								if (onRemapStateChanged2 != null)
								{
									onRemapStateChanged2(baseControllerVM, 0);
								}
							}
							this.RefreshAndGetSlot(baseControllerVM);
						}
					}
				}
			}
			using (List<Wrapper<REWASD_CONTROLLER_PROFILE_EX>>.Enumerator enumerator = newProfileWrappers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Wrapper<REWASD_CONTROLLER_PROFILE_EX> wrapper = enumerator.Current;
					if (wrapper.Value.Profiles.Any((REWASD_CONTROLLER_PROFILE p) => REWASD_CONTROLLER_PROFILE_Extensions.IsSlotWithUserConfig(p)))
					{
						BaseControllerVM baseControllerVM2 = this.GamepadCollection.FirstOrDefault((BaseControllerVM global) => global.ID == wrapper.Value.GetID(hiddenControlers));
						if (baseControllerVM2 != null)
						{
							RemapStateChangedHandler onRemapStateChanged3 = this.OnRemapStateChanged;
							if (onRemapStateChanged3 != null)
							{
								onRemapStateChanged3(baseControllerVM2, 1);
							}
						}
					}
				}
			}
			bool isExclusiveCaptureControllersPresent = this.IsExclusiveCaptureControllersPresent;
			this.ExternalDeviceRelationsHelper.Refresh();
		}

		public void PublishRemapStateChanged(BaseControllerVM controller, RemapState state)
		{
			RemapStateChangedHandler onRemapStateChanged = this.OnRemapStateChanged;
			if (onRemapStateChanged == null)
			{
				return;
			}
			onRemapStateChanged(controller, state);
		}

		private async void ProcessProfileStateChanged(WindowMessageEvent payload)
		{
			if (payload != null)
			{
				ulong ulongFromWMPayload = DSUtils.GetUlongFromWMPayload(payload);
				ulong num = (ulongFromWMPayload >> 32) & 65535UL;
				int num2 = (int)((ulongFromWMPayload >> 48) & 65535UL);
				bool flag = Convert.ToBoolean((long)num2 & 1L);
				bool flag2 = Convert.ToBoolean((long)num2 & 2L);
				bool flag3 = Convert.ToBoolean((long)num2 & 4L);
				bool flag4 = Convert.ToBoolean((long)num2 & 16L);
				bool flag5 = Convert.ToBoolean((long)num2 & 8L);
				Tracer.TraceWrite("---", false);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 1);
				defaultInterpolatedStringHandler.AppendLiteral("ProcessProfileStateChanged: ProfileID 0x");
				defaultInterpolatedStringHandler.AppendFormatted<ulong>(num, "X");
				Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(46, 1);
				defaultInterpolatedStringHandler.AppendLiteral("ProcessProfileStateChanged: ConnectionChanged ");
				defaultInterpolatedStringHandler.AppendFormatted<bool>(flag);
				Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 1);
				defaultInterpolatedStringHandler.AppendLiteral("ProcessProfileStateChanged: PairingChanged ");
				defaultInterpolatedStringHandler.AppendFormatted<bool>(flag2);
				Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
				if (flag3)
				{
					Tracer.TraceWrite("ProcessProfileStateChanged: VirtualPortConflict = TRUE", false);
				}
				if (flag4)
				{
					Tracer.TraceWrite("ProcessProfileStateChanged: VirtualRemoteConflict = TRUE", false);
				}
				if (flag5)
				{
					Tracer.TraceWrite("ProcessProfileStateChanged: FirmwareVersionMismatch = TRUE", false);
				}
				string externalDeviceId = "";
				if (num != 0UL)
				{
					REWASD_CONTROLLER_PROFILE_EX profileEx = this.GetProfileExByServiceProfileId(num);
					if (profileEx != null)
					{
						REWASD_CONTROLLER_PROFILE? rewasd_CONTROLLER_PROFILE = profileEx.FindProfileById((ushort)num);
						if (rewasd_CONTROLLER_PROFILE != null && REWASD_CONTROLLER_PROFILE_Extensions.IsExternalDevicePresent(rewasd_CONTROLLER_PROFILE.Value))
						{
							externalDeviceId = REWASD_CONTROLLER_PROFILE_Extensions.GetExternalDeviceId(rewasd_CONTROLLER_PROFILE.Value);
						}
					}
					if (flag5)
					{
						await this.RefreshServiceProfiles();
						if (profileEx != null)
						{
							await this._xbServiceCommunicator.DeleteProfiles(profileEx.GetAllProfileIds());
							if (!string.IsNullOrEmpty(externalDeviceId))
							{
								bool flag6 = false;
								ExternalDevice externalDevice = null;
								foreach (ExternalDevice externalDevice2 in new ExternalDevicesCollection(this.ExternalDevices))
								{
									if (externalDevice2.ExternalDeviceId == externalDeviceId)
									{
										Tracer.TraceWrite("ProcessProfileStateChanged: remove ExternalDevice with id " + externalDeviceId, false);
										externalDevice = externalDevice2;
										this.ExternalDevices.Remove(externalDevice2);
										flag6 = true;
									}
								}
								if (flag6)
								{
									this.BinDataSerialize.SaveExternalDevices();
									this.ExternalDeviceRelationsHelper.RemoveExternalDeviceRelations(externalDeviceId);
									if (externalDevice != null)
									{
										Engine.EventAggregator.GetEvent<ExternalDeviceOutdated>().Publish(externalDevice);
									}
								}
							}
						}
					}
					else
					{
						await this.RefreshServiceProfileState(num, num2);
					}
					if (!string.IsNullOrEmpty(externalDeviceId))
					{
						this.ExternalDeviceRelationsHelper.RefreshDevice(profileEx);
					}
					profileEx = null;
				}
				externalDeviceId = null;
			}
		}

		private async void ProcessGyroCalibrationFinished(WindowMessageEvent payload)
		{
			Tracer.TraceWrite("ProcessGyroCalibrationFinished", false);
			if (payload != null)
			{
				ulong id = DSUtils.GetUlongFromWMPayload(payload);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(50, 2);
				defaultInterpolatedStringHandler.AppendLiteral("ProcessGyroCalibrationFinished: ControllerId ");
				defaultInterpolatedStringHandler.AppendFormatted<ulong>(id);
				defaultInterpolatedStringHandler.AppendLiteral(" / 0x");
				defaultInterpolatedStringHandler.AppendFormatted<ulong>(id, "X");
				Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
				List<REWASD_CONTROLLER_INFO> list = await this._xbServiceCommunicator.GetControllersList(false, false, false);
				if (list.Exists((REWASD_CONTROLLER_INFO x) => x.Id == id))
				{
					REWASD_CONTROLLER_INFO rewasd_CONTROLLER_INFO = list.Find((REWASD_CONTROLLER_INFO x) => x.Id == id);
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
					defaultInterpolatedStringHandler.AppendFormatted("Calibration");
					defaultInterpolatedStringHandler.AppendLiteral("\\");
					defaultInterpolatedStringHandler.AppendFormatted<ulong>(id);
					RegistryHelper.SetValue(defaultInterpolatedStringHandler.ToStringAndClear(), "GyroXDelta", rewasd_CONTROLLER_INFO.GyroXCalibrationDelta);
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
					defaultInterpolatedStringHandler.AppendFormatted("Calibration");
					defaultInterpolatedStringHandler.AppendLiteral("\\");
					defaultInterpolatedStringHandler.AppendFormatted<ulong>(id);
					RegistryHelper.SetValue(defaultInterpolatedStringHandler.ToStringAndClear(), "GyroYDelta", rewasd_CONTROLLER_INFO.GyroYCalibrationDelta);
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
					defaultInterpolatedStringHandler.AppendFormatted("Calibration");
					defaultInterpolatedStringHandler.AppendLiteral("\\");
					defaultInterpolatedStringHandler.AppendFormatted<ulong>(id);
					RegistryHelper.SetValue(defaultInterpolatedStringHandler.ToStringAndClear(), "GyroZDelta", rewasd_CONTROLLER_INFO.GyroZCalibrationDelta);
					string text = "ProcessGyroCalibrationFinished: save to registry ";
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 1);
					defaultInterpolatedStringHandler.AppendLiteral("GyroXDelta ");
					defaultInterpolatedStringHandler.AppendFormatted<long>(rewasd_CONTROLLER_INFO.GyroXCalibrationDelta);
					defaultInterpolatedStringHandler.AppendLiteral(" / ");
					string text2 = defaultInterpolatedStringHandler.ToStringAndClear();
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 1);
					defaultInterpolatedStringHandler.AppendLiteral("GyroYDelta ");
					defaultInterpolatedStringHandler.AppendFormatted<long>(rewasd_CONTROLLER_INFO.GyroYCalibrationDelta);
					defaultInterpolatedStringHandler.AppendLiteral(" / ");
					string text3 = defaultInterpolatedStringHandler.ToStringAndClear();
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 1);
					defaultInterpolatedStringHandler.AppendLiteral("GyroZDelta ");
					defaultInterpolatedStringHandler.AppendFormatted<long>(rewasd_CONTROLLER_INFO.GyroZCalibrationDelta);
					Tracer.TraceWrite(text + text2 + text3 + defaultInterpolatedStringHandler.ToStringAndClear(), false);
				}
			}
		}

		private void SendPhysicalSlotChanged(BaseControllerVM controller, Slot slot)
		{
			GamepadProfiles value = this.GamepadProfileRelations.FirstOrDefault((KeyValuePair<string, GamepadProfiles> kvp) => controller.ID == kvp.Key).Value;
			REWASD_CONTROLLER_PROFILE_EX rewasd_CONTROLLER_PROFILE_EX = this.ServiceProfilesCollection.FindByID(controller.ID);
			this.SaveSlot(controller.ShortID, slot);
			ControllerSlotChangedHandler onPhysicalSlotChanged = this.OnPhysicalSlotChanged;
			if (onPhysicalSlotChanged == null)
			{
				return;
			}
			BaseControllerVM controller2 = controller;
			GamepadProfile gamepadProfile;
			if (value == null)
			{
				gamepadProfile = null;
			}
			else
			{
				SlotProfilesDictionary slotProfiles = value.SlotProfiles;
				gamepadProfile = ((slotProfiles != null) ? slotProfiles.TryGetValue(slot) : null);
			}
			onPhysicalSlotChanged(controller2, gamepadProfile, slot, (rewasd_CONTROLLER_PROFILE_EX != null) ? new REWASD_CONTROLLER_PROFILE?(rewasd_CONTROLLER_PROFILE_EX.Profiles[slot]) : null, true);
		}

		private void ProcessPhysicalSlot(WindowMessageEvent payload, Slot slot)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 1);
			defaultInterpolatedStringHandler.AppendLiteral("ProcessPhysicalSlot");
			defaultInterpolatedStringHandler.AppendFormatted<Slot>(slot);
			defaultInterpolatedStringHandler.AppendLiteral("Selected");
			Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
			if (payload != null)
			{
				ulong controllerID = DSUtils.GetUlongFromWMPayload(payload);
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Controller Id ");
				defaultInterpolatedStringHandler.AppendFormatted<ulong>(controllerID);
				Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
				using (IEnumerator<BaseControllerVM> enumerator = this.GamepadCollection.GetEnumerator())
				{
					Func<BaseControllerVM, bool> <>9__0;
					while (enumerator.MoveNext())
					{
						BaseControllerVM item = enumerator.Current;
						ControllerVM controllerVM = item as ControllerVM;
						if (controllerVM == null || controllerVM.ControllerId != controllerID)
						{
							CompositeControllerVM compositeControllerVM = item as CompositeControllerVM;
							if (compositeControllerVM == null)
							{
								continue;
							}
							IEnumerable<BaseControllerVM> baseControllers = compositeControllerVM.BaseControllers;
							Func<BaseControllerVM, bool> func;
							if ((func = <>9__0) == null)
							{
								func = (<>9__0 = delegate(BaseControllerVM baseController)
								{
									ControllerVM controllerVM2 = baseController as ControllerVM;
									return controllerVM2 != null && controllerVM2.ControllerId == controllerID;
								});
							}
							if (!baseControllers.Any(func))
							{
								continue;
							}
						}
						item.SetCurrentSlot(slot);
						this.SendPhysicalSlotChanged(item, slot);
						if (!Engine.UserSettingsService.IsLedSettingsEnabled)
						{
							break;
						}
						Application.Current.Dispatcher.InvokeAsync(delegate
						{
							GamepadProfiles value = this.GamepadProfileRelations.FirstOrDefault((KeyValuePair<string, GamepadProfiles> kvp) => item.ID == kvp.Key).Value;
							ControllerVM currentController = item.CurrentController;
							ILEDService ledservice = Engine.LEDService;
							BaseControllerVM baseControllerVM = currentController;
							GamepadProfile gamepadProfile;
							if (value == null)
							{
								gamepadProfile = null;
							}
							else
							{
								SlotProfilesDictionary slotProfiles = value.SlotProfiles;
								gamepadProfile = ((slotProfiles != null) ? slotProfiles.TryGetValue(slot) : null);
							}
							ledservice.ApplyLEDsToControllerAccordingToSettings(baseControllerVM, gamepadProfile, new Slot?(slot), 0, false, false);
						});
					}
				}
			}
		}

		private void ProcessMacroCommand(WindowMessageEvent payload)
		{
			Tracer.TraceWrite("ProcessMacroCommand", false);
			if (payload != null)
			{
				ulong ulongFromWMPayload = DSUtils.GetUlongFromWMPayload(payload);
				ulong num = (ulongFromWMPayload >> 32) & 65535UL;
				int commandID = (int)((ulongFromWMPayload >> 48) & 65535UL);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(51, 2);
				defaultInterpolatedStringHandler.AppendLiteral("ProcessMacroCommand: ProfileId 0x");
				defaultInterpolatedStringHandler.AppendFormatted<ulong>(num, "X");
				defaultInterpolatedStringHandler.AppendLiteral(" / UserCommand Id ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(commandID);
				Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
				BaseExecutableRewasdUserCommand baseExecutableRewasdUserCommand = (BaseExecutableRewasdUserCommand)BaseRewasdUserCommand.REWASD_USER_COMMAND_TABLE.FirstOrDefault(delegate(BaseRewasdUserCommand ruc)
				{
					BaseExecutableRewasdUserCommand baseExecutableRewasdUserCommand2 = ruc as BaseExecutableRewasdUserCommand;
					return baseExecutableRewasdUserCommand2 != null && baseExecutableRewasdUserCommand2.CommandId == commandID;
				});
				if (baseExecutableRewasdUserCommand == null)
				{
					return;
				}
				baseExecutableRewasdUserCommand.Execute(num);
				Tracer.TraceWrite("ProcessMacroCommand: UserCommand Description " + baseExecutableRewasdUserCommand.Description, false);
			}
		}

		private void ProcessAmiiboUnloadByUid(WindowMessageEvent payload)
		{
			Tracer.TraceWrite("ProcessAmiiboUnloadByUid", false);
			if (payload != null)
			{
				ulong ulongFromWMPayload = DSUtils.GetUlongFromWMPayload(payload);
				byte[] array = new byte[]
				{
					(byte)((ulongFromWMPayload >> 48) & 255UL),
					(byte)((ulongFromWMPayload >> 40) & 255UL),
					(byte)((ulongFromWMPayload >> 32) & 255UL),
					(byte)((ulongFromWMPayload >> 24) & 255UL),
					(byte)((ulongFromWMPayload >> 16) & 255UL),
					(byte)((ulongFromWMPayload >> 8) & 255UL),
					(byte)(ulongFromWMPayload & 255UL)
				};
				string text = "ProcessAmiiboUnloadByUid: Amiibo UID ";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 7);
				defaultInterpolatedStringHandler.AppendFormatted<byte>(array[0], "X2");
				defaultInterpolatedStringHandler.AppendLiteral("-");
				defaultInterpolatedStringHandler.AppendFormatted<byte>(array[1], "X2");
				defaultInterpolatedStringHandler.AppendLiteral("-");
				defaultInterpolatedStringHandler.AppendFormatted<byte>(array[2], "X2");
				defaultInterpolatedStringHandler.AppendLiteral("-");
				defaultInterpolatedStringHandler.AppendFormatted<byte>(array[3], "X2");
				defaultInterpolatedStringHandler.AppendLiteral("-");
				defaultInterpolatedStringHandler.AppendFormatted<byte>(array[4], "X2");
				defaultInterpolatedStringHandler.AppendLiteral("-");
				defaultInterpolatedStringHandler.AppendFormatted<byte>(array[5], "X2");
				defaultInterpolatedStringHandler.AppendLiteral("-");
				defaultInterpolatedStringHandler.AppendFormatted<byte>(array[6], "X2");
				Tracer.TraceWrite(text + defaultInterpolatedStringHandler.ToStringAndClear(), false);
				REWASD_CONTROLLER_PROFILE_EX profile = this.ServiceProfilesCollection.FindByAmiiboUid(array);
				if (profile != null)
				{
					foreach (REWASD_CONTROLLER_PROFILE_EX.AmiiboWrapper amiiboWrapper in profile.Amiibo)
					{
						if (amiiboWrapper.IsUidEqual(array))
						{
							amiiboWrapper.UnLoad();
							Tracer.TraceWrite("ProcessAmiiboUnloadByUid: Amiibo Unloaded", false);
						}
					}
					BaseControllerVM baseControllerVM = Engine.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.ID == profile.GetID(null));
					if (baseControllerVM != null)
					{
						ControllerChangedHandler onControllerChanged = this.OnControllerChanged;
						if (onControllerChanged == null)
						{
							return;
						}
						onControllerChanged(baseControllerVM);
					}
				}
			}
		}

		private async Task ReinitService()
		{
			await this._gameProfilesService.WaitForServiceInited();
			await this.InitService();
		}

		public bool IsServiceInited { get; set; }

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
				ServiceResponseWrapper<REWASD_GET_VERSION_RESPONSE> serviceResponseWrapper = await this._xbServiceCommunicator.GetVersion();
				if (serviceResponseWrapper.IsResponseValid)
				{
					if (serviceResponseWrapper.ServiceResponse.Header.Status == 0U && serviceResponseWrapper.ServiceResponse.Header.Size > 0U)
					{
						bool flag = false;
						this._serviceVersionMajor = serviceResponseWrapper.ServiceResponse.ServiceMajorVersion;
						this._serviceVersionMinor = serviceResponseWrapper.ServiceResponse.ServiceMinorVersion;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 2);
						defaultInterpolatedStringHandler.AppendLiteral("Service version service response: ");
						defaultInterpolatedStringHandler.AppendFormatted<byte>(this._serviceVersionMajor);
						defaultInterpolatedStringHandler.AppendLiteral(".");
						defaultInterpolatedStringHandler.AppendFormatted<byte>(this._serviceVersionMinor);
						Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
						if (this._serviceVersionMajor != 5 || this._serviceVersionMinor != 12)
						{
							defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(62, 4);
							defaultInterpolatedStringHandler.AppendLiteral("Check version: Service version ");
							defaultInterpolatedStringHandler.AppendFormatted<byte>(this._serviceVersionMajor);
							defaultInterpolatedStringHandler.AppendLiteral(".");
							defaultInterpolatedStringHandler.AppendFormatted<byte>(this._serviceVersionMinor);
							defaultInterpolatedStringHandler.AppendLiteral(" is not equal to GUI version ");
							defaultInterpolatedStringHandler.AppendFormatted<int>(5);
							defaultInterpolatedStringHandler.AppendLiteral(".");
							defaultInterpolatedStringHandler.AppendFormatted<int>(12);
							Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
							flag = true;
						}
						this._driverVersionMajor = serviceResponseWrapper.ServiceResponse.DriverMajorVersion;
						this._driverVersionMinor = serviceResponseWrapper.ServiceResponse.DriverMinorVersion;
						defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 2);
						defaultInterpolatedStringHandler.AppendLiteral("Driver version service response: ");
						defaultInterpolatedStringHandler.AppendFormatted<byte>(this._driverVersionMajor);
						defaultInterpolatedStringHandler.AppendLiteral(".");
						defaultInterpolatedStringHandler.AppendFormatted<byte>(this._driverVersionMinor);
						Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
						if (this._driverVersionMajor == 0 && this._driverVersionMinor == 0)
						{
							Tracer.TraceWrite("Driver is not found", false);
							if (!AdminOperationsDeciderExtensions.TryInstallDriver(this, false))
							{
								return;
							}
							flag = true;
						}
						else if (this._driverVersionMajor != 3 || this._driverVersionMinor != 29)
						{
							defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(61, 4);
							defaultInterpolatedStringHandler.AppendLiteral("Check version: Driver version ");
							defaultInterpolatedStringHandler.AppendFormatted<byte>(this._driverVersionMajor);
							defaultInterpolatedStringHandler.AppendLiteral(".");
							defaultInterpolatedStringHandler.AppendFormatted<byte>(this._driverVersionMinor);
							defaultInterpolatedStringHandler.AppendLiteral(" is not equal to GUI version ");
							defaultInterpolatedStringHandler.AppendFormatted<int>(3);
							defaultInterpolatedStringHandler.AppendLiteral(".");
							defaultInterpolatedStringHandler.AppendFormatted<int>(29);
							Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
							if (!AdminOperationsDeciderExtensions.TryInstallDriver(this, true))
							{
								return;
							}
							flag = true;
						}
						if (this._driverVersionMajor != 0)
						{
							bool flag2 = Convert.ToBoolean((uint)(serviceResponseWrapper.ServiceResponse.DriverFlags & 2));
							Tracer.TraceWrite(flag2 ? "Driver flags: NDIS filter registered" : "Driver flags: NDIS filter NOT registered", false);
							RegistryHelper.SetValue("Config", "IsNDISFilterRegistered", flag2 ? 1 : 0);
							Tracer.TraceWrite(Convert.ToBoolean((uint)(serviceResponseWrapper.ServiceResponse.DriverFlags & 64)) ? "Driver flags: process notify registered" : "Driver flags: process notify not registered", false);
							Tracer.TraceWrite(Convert.ToBoolean((uint)(serviceResponseWrapper.ServiceResponse.DriverFlags & 128)) ? "Driver flags: object callbacks registered" : "Driver flags: object callbacks not registered", false);
							Tracer.TraceWrite(Convert.ToBoolean((uint)(serviceResponseWrapper.ServiceResponse.DriverFlags & 256)) ? "Driver flags: virtual USB HUB present" : "Driver flags: virtual USB HUB not present", false);
							SafeFileHandle safeFileHandle = UtilsNative.OpenHidgamemapDriver();
							if (UtilsNative.IsHandleValid(safeFileHandle))
							{
								Tracer.TraceWrite("HidgamemapDriver opened successfully", false);
								safeFileHandle.Close();
							}
							else
							{
								Tracer.TraceWrite("Failed to open HidgamemapDriver", false);
							}
						}
						string text = Path.Combine(Constants.PROGRAMM_DATA_DIRECTORY_PATH, "Logs");
						string text2 = "Anonymous";
						WindowsIdentity current = WindowsIdentity.GetCurrent();
						SecurityIdentifier user = current.User;
						bool flag3 = new WindowsPrincipal(current).IsInRole(WindowsBuiltInRole.Administrator);
						if (user != null)
						{
							text2 = user.ToString().RemoveCharacters(new char[] { '{', '}', '-' });
							if (flag3)
							{
								text2 += "_admin";
							}
						}
						Path.Combine(text, "Version_" + text2 + ".log");
						if (!string.IsNullOrEmpty(text))
						{
							if (!Directory.Exists(text))
							{
								try
								{
									Directory.CreateDirectory(text);
								}
								catch (Exception)
								{
								}
							}
						}
						if (flag)
						{
							DTMessageBox.Show("Service and app version mismatch. Please install the latest reWASD version to continue.", MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
							Environment.Exit(0);
						}
					}
					await this.RefreshServiceProfiles();
					await this.RefreshGamepadCollection(null);
					List<Tuple<ulong, SimpleDeviceInfo>> simpleDeviceInfoList = this.SimpleDeviceInfoList;
					bool flag4;
					if (simpleDeviceInfoList == null)
					{
						flag4 = false;
					}
					else
					{
						flag4 = simpleDeviceInfoList.Exists((Tuple<ulong, SimpleDeviceInfo> x) => x.Item2.Type == 141U);
					}
					if (flag4)
					{
						Tuple<ulong, SimpleDeviceInfo> tuple = this.SimpleDeviceInfoList.FirstOrDefault((Tuple<ulong, SimpleDeviceInfo> x) => x.Item2.Type == 141U);
						byte b = (byte)RegistryHelper.GetValue("Config", "SteamDeckLeftIntensity", 7, false);
						int value = RegistryHelper.GetValue("Config", "SteamDeckLeftIsStrongIntensity", 1, false);
						byte b2 = (byte)RegistryHelper.GetValue("Config", "SteamDeckRightIntensity", 7, false);
						int value2 = RegistryHelper.GetValue("Config", "SteamDeckRightIsStrongIntensity", 1, false);
						if (tuple != null && (b != 7 || value != 1 || b2 != 7 || value2 != 1))
						{
							await this._xbServiceCommunicator.SteamDeckSetMotorIntensitySettings(tuple.Item2.Id, tuple.Item2.Type, Convert.ToBoolean(value), b, Convert.ToBoolean(value2), b2);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Tracer.TraceException(ex, "InitService");
			}
			this.IsServiceInited = true;
			this._eventAggregator.GetEvent<GamepadServiceInited>().Publish(null);
			if (BluetoothUtils.IsBluetoothExist() && Tracer.IsTextFileTraceEnabled)
			{
				BluetoothUtils.TraceBluetoothInfo(" - Bluetooth");
				List<REWASD_RADIO_INFO> list = await this._xbServiceCommunicator.GetRadioInfoList();
				if (list != null && list.Count > 0)
				{
					BluetoothUtils.TraceRadioInfo(list, " - Bluetooth");
				}
			}
		}

		public async Task RefreshGamepadCollection(ulong controllerID)
		{
			await this.RefreshGamepadCollection((controllerID == 0UL) ? "" : controllerID.ToString());
		}

		private async Task RefreshStatesAndReactOnBatteryOfGamepadCollection()
		{
			BaseControllerVM[] array = new BaseControllerVM[this.GamepadCollection.Count];
			this.GamepadCollection.CopyTo(array, 0);
			foreach (BaseControllerVM baseControllerVM in array)
			{
				await this.RefreshGamepadStatesAndReactOnBattery(baseControllerVM, true);
			}
			BaseControllerVM[] array2 = null;
		}

		public async Task RefreshGamepadBattery(ulong id)
		{
			BaseControllerVM baseControllerVM = this.GamepadCollection.FindControllerByControllerId(id);
			await this.RefreshGamepadStatesAndReactOnBattery(baseControllerVM, false);
		}

		private async Task RefreshGamepadStatesAndReactOnBattery(BaseControllerVM controller, bool isUpdateControllerStateFromService = true)
		{
			try
			{
				CompositeControllerVM compositeControllerVM = controller as CompositeControllerVM;
				if (compositeControllerVM != null)
				{
					foreach (BaseControllerVM baseControllerVM in compositeControllerVM.BaseControllers)
					{
						await this.RefreshGamepadStatesAndReactOnBattery(baseControllerVM, isUpdateControllerStateFromService);
					}
					List<BaseControllerVM>.Enumerator enumerator = default(List<BaseControllerVM>.Enumerator);
				}
				else if (!(controller is OfflineDeviceVM))
				{
					ControllerVM controllerVM = controller as ControllerVM;
					if (controllerVM != null && controller.IsOnline)
					{
						await this.RefreshGamepadStatesAndReactOnBattery(controllerVM, isUpdateControllerStateFromService);
					}
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(string.Format("private RefreshGamepadStatesAndReactOnBattery - {0}", ex.Message));
			}
		}

		private async Task RefreshGamepadStatesAndReactOnBattery(ControllerVM gamepad, bool isUpdateControllerStateFromService)
		{
			try
			{
				if (ControllerTypeExtensions.IsGamepad(gamepad.ControllerType) && gamepad.IsOnline && !gamepad.IsDebugController && gamepad.ControllerBatteryKind != 3)
				{
					if (isUpdateControllerStateFromService)
					{
						ServiceResponseWrapper<REWASD_GET_CONTROLLER_STATE_RESPONSE> serviceResponseWrapper = await this._xbServiceCommunicator.GetControllerState(gamepad.ControllerId, gamepad.Type, false);
						if (serviceResponseWrapper.IsResponseValid && serviceResponseWrapper.ServiceResponse.Header.Status == 0U && serviceResponseWrapper.ServiceResponse.Header.Size > 0U)
						{
							gamepad.UpdateControllerState(serviceResponseWrapper.ServiceResponse);
						}
					}
					if (gamepad.IsLedPresent && ControllerTypeExtensions.IsLedAllowed(gamepad.ControllerType) && Engine.UserSettingsService.IsLedSettingsEnabled)
					{
						if (!gamepad.IsCharging && gamepad.ConnectionType != null && (gamepad.ControllerBatteryLevel == null || gamepad.ControllerBatteryLevel == 1))
						{
							Engine.LEDService.SetBatteryNotification(gamepad);
							SetBatteryNotificationEventHandler setBatteryNotificationEvent = this.SetBatteryNotificationEvent;
							if (setBatteryNotificationEvent != null)
							{
								setBatteryNotificationEvent(gamepad);
							}
						}
						else
						{
							await Engine.LEDService.RemoveBatteryNotificationAndResume(gamepad);
						}
					}
					BatteryLevelChangedHandler onBatteryLevelChanged = this.OnBatteryLevelChanged;
					if (onBatteryLevelChanged != null)
					{
						onBatteryLevelChanged(gamepad);
					}
					if (!gamepad.IsCharging)
					{
						BatteryLevelChangedHandler onBatteryLevelChangedUI = this.OnBatteryLevelChangedUI;
						if (onBatteryLevelChangedUI != null)
						{
							onBatteryLevelChangedUI(gamepad);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(string.Format("private RefreshGamepadStatesAndReactOnBattery2 - {0}", ex.Message));
			}
		}

		private void OnInitializedController(BaseControllerVM baseController)
		{
			this.CheckControllerSettings(baseController);
			this.BinDataSerialize.SaveGamepadCollection();
		}

		public void RemoveControllersByCompositeDevices()
		{
			BaseControllerVM baseControllerVM;
			do
			{
				baseControllerVM = this.GamepadCollection.FirstOrDefault((BaseControllerVM item) => (item is PeripheralVM || item is ControllerVM) && this.CompositeDevices.FindCompositeForSimple(item.ID) != null);
				if (baseControllerVM != null)
				{
					this.GamepadCollection.Remove(baseControllerVM);
				}
			}
			while (baseControllerVM != null);
		}

		private void CheckControllerSettings(BaseControllerVM controller)
		{
			string text = controller.ControllerDisplayName;
			ControllerVM controllerVM = controller as ControllerVM;
			if (controllerVM != null)
			{
				text = (controllerVM.IsUnknownControllerType ? controllerVM.UnknownControllerInfoString : controllerVM.ControllerDisplayName);
			}
			if (controller.IsInitializedController)
			{
				PeripheralVM peripheralVM = controller as PeripheralVM;
				if (((peripheralVM != null && peripheralVM.PeripheralPhysicalType != null) || !(controller is PeripheralVM)) && !this.IsBlacklistedDevice(controller.ID))
				{
					if (!(controller is CompositeControllerVM))
					{
						this.GamepadsUserLedCollection.AddGamepad(controller.ID, controller.Ids, controller.Types, text);
					}
					this.GamepadsHotkeyCollection.AddDefaultEntryIfNotPresent(controller.ID, controller.Types, controller.ControllerFamily, text, controller.Ids);
				}
			}
		}

		private void SetOnlineState(BaseControllerVM controller, bool onlineState)
		{
			bool flag = false;
			RemapState remapState = this.GetRemapState(controller.ID);
			REWASD_CONTROLLER_PROFILE_EX profileEx = this.GetProfileEx(controller.ID);
			if (profileEx != null)
			{
				flag = profileEx.IsExclusiveControllersMaskPresent();
			}
			if (!Convert.ToBoolean(RegistryHelper.GetValue("Config", "LeaveVirtualControllerForOfflineController", 0, false)) && controller.IsOnline && !onlineState && !flag)
			{
				bool flag2 = remapState == 1;
				this.DisableRemap(controller.ID, true);
				RegistryHelper.SetValue("Controllers\\" + controller.ShortID, "IsRemapped", flag2);
			}
			bool flag3 = false;
			if (!controller.IsOnline && onlineState && ((remapState != null && RegistryHelper.GetValue("Controllers\\" + controller.ShortID, "IsRemapped", 0, false) == 1) || remapState == 1))
			{
				controller.IsOnline = onlineState;
				this.CachedProfilesCollection.Clear();
				if (!flag)
				{
					this.EnableRemap(false, controller.ID, false, false, true, -1, false, false, true, null, null);
				}
				flag3 = true;
			}
			else
			{
				controller.IsOnline = onlineState;
			}
			if (!flag3)
			{
				IEnumerable<uint> types = controller.Types;
				Func<uint, bool> func;
				if ((func = GamepadService.<>O.<0>__IsHiddenAppliedConfigControllerPhysicalType) == null)
				{
					func = (GamepadService.<>O.<0>__IsHiddenAppliedConfigControllerPhysicalType = new Func<uint, bool>(UtilsCommon.IsHiddenAppliedConfigControllerPhysicalType));
				}
				if (types.Any(func))
				{
					for (int i = 0; i < controller.Types.Length; i++)
					{
						if (UtilsCommon.IsHiddenAppliedConfigControllerPhysicalType(controller.Types[i]) && i < controller.Ids.Length)
						{
							this.CompileHiddenAppliedConfig(controller.Ids[i], controller.Types[i], true);
						}
					}
				}
			}
		}

		private async Task UpdateExistingControllers(List<REWASD_CONTROLLER_INFO> realControllersList, List<REWASD_CONTROLLER_INFO> controllersPeripheralForAdd, List<REWASD_CONTROLLER_INFO> controllersForAdd, HashSet<CompositeControllerVM> changedComposites)
		{
			using (List<REWASD_CONTROLLER_INFO>.Enumerator enumerator = realControllersList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					GamepadService.<>c__DisplayClass216_0 CS$<>8__locals1 = new GamepadService.<>c__DisplayClass216_0();
					CS$<>8__locals1.newItem = enumerator.Current;
					if (realControllersList.Count((REWASD_CONTROLLER_INFO x) => x.Id == CS$<>8__locals1.newItem.Id && ControllerTypeExtensions.IsSonyDS4OrDualSense(ControllerTypeExtensions.ConvertPhysicalTypeToEnum(0, x.Type, x.Id))) > 1)
					{
						this.DuplicateGamepadCollection.Add(new Tuple<ulong, uint>(CS$<>8__locals1.newItem.Id, CS$<>8__locals1.newItem.Type));
					}
					if (REWASD_CONTROLLER_INFO_Extensions.IsPeripheral(CS$<>8__locals1.newItem))
					{
						controllersPeripheralForAdd.Add(CS$<>8__locals1.newItem);
					}
					else if (controllersForAdd.All((REWASD_CONTROLLER_INFO item) => item.Id != CS$<>8__locals1.newItem.Id))
					{
						controllersForAdd.Add(CS$<>8__locals1.newItem);
					}
					foreach (BaseControllerVM existItem in this.GamepadCollection)
					{
						bool flag;
						if (existItem.ControllerFamily != null)
						{
							if (existItem.ControllerFamily == 4)
							{
								flag = existItem.ControllerTypeEnums.All((ControllerTypeEnum x) => x == null || x == 2000);
							}
							else
							{
								flag = false;
							}
						}
						else
						{
							flag = true;
						}
						bool flag2 = flag;
						if (existItem.IsConsideredTheSameControllerByID(CS$<>8__locals1.newItem.Id.ToString()) || (!flag2 && existItem.ContainerId == CS$<>8__locals1.newItem.ContainerId))
						{
							bool skipRemoveController = false;
							if (CS$<>8__locals1.newItem.Type != 24U)
							{
								goto IL_229;
							}
							List<Tuple<ulong, uint>> duplicateGamepadCollection = this.DuplicateGamepadCollection;
							Predicate<Tuple<ulong, uint>> predicate;
							if ((predicate = CS$<>8__locals1.<>9__3) == null)
							{
								predicate = (CS$<>8__locals1.<>9__3 = (Tuple<ulong, uint> x) => x.Item1 == CS$<>8__locals1.newItem.Id && x.Item2 == 23U);
							}
							if (!duplicateGamepadCollection.Exists(predicate))
							{
								goto IL_229;
							}
							bool flag3 = true;
							IL_27E:
							ConnectionType prevConnectionType = 4;
							string prevFirmware = "";
							bool prevGyroFlag = false;
							bool prevAccelerometerFlag = false;
							ControllerVM controllerVM = existItem as ControllerVM;
							if (controllerVM != null)
							{
								prevConnectionType = controllerVM.ConnectionType;
								prevFirmware = controllerVM.FirmwareVersion;
								prevGyroFlag = controllerVM.IsGyroscopePresent;
								prevAccelerometerFlag = controllerVM.IsAccelerometerPresent;
							}
							bool isControllerChanged = false;
							if (!flag3)
							{
								isControllerChanged = existItem.UpdateControllerInfo(CS$<>8__locals1.newItem);
							}
							ServiceResponseWrapper<REWASD_GET_CONTROLLER_STATE_RESPONSE> serviceResponseWrapper = await this._xbServiceCommunicator.GetControllerState(CS$<>8__locals1.newItem.Id, CS$<>8__locals1.newItem.Type, false);
							if (serviceResponseWrapper.IsResponseValid && serviceResponseWrapper.ServiceResponse.Header.Status == 0U && serviceResponseWrapper.ServiceResponse.Header.Size > 0U)
							{
								BaseCompositeControllerVM baseCompositeControllerVM = existItem as BaseCompositeControllerVM;
								if (baseCompositeControllerVM != null)
								{
									BaseControllerVM controller = baseCompositeControllerVM.FindController(CS$<>8__locals1.newItem.Id);
									if (controller != null && !(controller is OfflineDeviceVM))
									{
										controller.UpdateControllerState(serviceResponseWrapper.ServiceResponse);
										if (existItem is CompositeControllerVM)
										{
											ControllerVM controllerVM2 = controller as ControllerVM;
											if (controllerVM2 != null && controller.IsOnline)
											{
												BatteryLevelChangedHandler onBatteryLevelChanged = this.OnBatteryLevelChanged;
												if (onBatteryLevelChanged != null)
												{
													onBatteryLevelChanged(controllerVM2);
												}
											}
											if (!controller.IsOnline)
											{
												controller.IsOnline = true;
												CompositeControllerVM compositeControllerVM = existItem as CompositeControllerVM;
												if (compositeControllerVM != null)
												{
													changedComposites.Add(compositeControllerVM);
												}
												this.RefreshLedForController(controller);
												await this.RefreshGamepadStatesAndReactOnBattery(controller, true);
												GamepadStateChanged onGamepadStateChanged = this.OnGamepadStateChanged;
												if (onGamepadStateChanged != null)
												{
													onGamepadStateChanged(controller);
												}
											}
										}
									}
									else
									{
										skipRemoveController = true;
									}
									controller = null;
								}
								else
								{
									existItem.UpdateControllerState(serviceResponseWrapper.ServiceResponse);
									ControllerVM controllerVM3 = existItem as ControllerVM;
									if (controllerVM3 != null)
									{
										if (controllerVM3.IsOnline)
										{
											BatteryLevelChangedHandler onBatteryLevelChanged2 = this.OnBatteryLevelChanged;
											if (onBatteryLevelChanged2 != null)
											{
												onBatteryLevelChanged2(controllerVM3);
											}
											if (prevConnectionType != controllerVM3.ConnectionType)
											{
												this.RefreshLedForController(controllerVM3);
											}
										}
										if (isControllerChanged || prevFirmware != controllerVM3.FirmwareVersion || prevGyroFlag != controllerVM3.IsGyroscopePresent || prevAccelerometerFlag != controllerVM3.IsAccelerometerPresent)
										{
											ControllerChangedHandler onControllerChanged = this.OnControllerChanged;
											if (onControllerChanged != null)
											{
												onControllerChanged(existItem);
											}
											isControllerChanged = false;
										}
									}
								}
							}
							if (!existItem.IsOnline)
							{
								if (!(existItem is CompositeControllerVM))
								{
									this.AddGamepadToGamepadsAuthCollection(existItem);
									this.CheckControllerSettings(existItem);
								}
								this.SetOnlineState(existItem, true);
								this.RefreshLedForController(existItem);
								if (isControllerChanged)
								{
									ControllerChangedHandler onControllerChanged2 = this.OnControllerChanged;
									if (onControllerChanged2 != null)
									{
										onControllerChanged2(existItem);
									}
								}
								else
								{
									GamepadStateChanged onGamepadStateChanged2 = this.OnGamepadStateChanged;
									if (onGamepadStateChanged2 != null)
									{
										onGamepadStateChanged2(existItem);
									}
								}
							}
							if (!skipRemoveController)
							{
								controllersForAdd.Remove(CS$<>8__locals1.newItem);
								controllersPeripheralForAdd.Remove(CS$<>8__locals1.newItem);
							}
							prevFirmware = null;
							goto IL_6AC;
							IL_229:
							if (CS$<>8__locals1.newItem.Type == 26U)
							{
								List<Tuple<ulong, uint>> duplicateGamepadCollection2 = this.DuplicateGamepadCollection;
								Predicate<Tuple<ulong, uint>> predicate2;
								if ((predicate2 = CS$<>8__locals1.<>9__4) == null)
								{
									predicate2 = (CS$<>8__locals1.<>9__4 = (Tuple<ulong, uint> x) => x.Item1 == CS$<>8__locals1.newItem.Id && x.Item2 == 25U);
								}
								flag3 = duplicateGamepadCollection2.Exists(predicate2);
								goto IL_27E;
							}
							flag3 = false;
							goto IL_27E;
						}
						IL_6AC:
						existItem = null;
					}
					IEnumerator<BaseControllerVM> enumerator2 = null;
					CS$<>8__locals1 = null;
				}
			}
			List<REWASD_CONTROLLER_INFO>.Enumerator enumerator = default(List<REWASD_CONTROLLER_INFO>.Enumerator);
		}

		private async Task AddPeripheralDevices(List<REWASD_CONTROLLER_INFO> controllersPeripheralForAdd, HashSet<CompositeControllerVM> changedComposites)
		{
			using (IEnumerator<IGrouping<Guid, REWASD_CONTROLLER_INFO>> enumerator = (from c in controllersPeripheralForAdd
				group c by c.ContainerId).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IGrouping<Guid, REWASD_CONTROLLER_INFO> group = enumerator.Current;
					GamepadService.<>c__DisplayClass217_1 CS$<>8__locals2 = new GamepadService.<>c__DisplayClass217_1();
					string text = " - Controllers";
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(47, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Processing \"peripheral\" device for \"");
					defaultInterpolatedStringHandler.AppendFormatted<Guid>(group.Key);
					defaultInterpolatedStringHandler.AppendLiteral("\" container");
					Tracer.TraceWriteTag(text, defaultInterpolatedStringHandler.ToStringAndClear(), false);
					CS$<>8__locals2.newPeripheral = this.AllPhysicalControllers.FirstOrDefault((BaseControllerVM item) => item is PeripheralVM && item.ContainerId == group.Key) as PeripheralVM;
					bool isNewDevice = false;
					if (CS$<>8__locals2.newPeripheral == null)
					{
						CS$<>8__locals2.newPeripheral = new PeripheralVM(group.Key, false);
						if (this.BlacklistGamepads.FirstOrDefault((BlackListGamepad item) => item.ID == CS$<>8__locals2.newPeripheral.ID) != null)
						{
							continue;
						}
						isNewDevice = true;
					}
					bool flag = !isNewDevice && !CS$<>8__locals2.newPeripheral.IsOnline;
					using (IEnumerator<REWASD_CONTROLLER_INFO> enumerator2 = group.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							REWASD_CONTROLLER_INFO newItem = enumerator2.Current;
							if (!CS$<>8__locals2.newPeripheral.BaseControllers.Exists((BaseControllerVM item) => (item as ControllerVM).ControllerId == newItem.Id))
							{
								ControllerVM controllerVM = new ControllerVM(newItem, false);
								controllerVM.IsOnline = true;
								CS$<>8__locals2.newPeripheral.AddController(controllerVM);
							}
						}
					}
					if (CS$<>8__locals2.newPeripheral.HasKeyboardControllers || CS$<>8__locals2.newPeripheral.HasMouseControllers)
					{
						PeripheralDevice peripheralDevice = this.PeripheralDevices.FindCompositeForSimple(CS$<>8__locals2.newPeripheral.CurrentController.ControllerId);
						if (peripheralDevice != null)
						{
							CS$<>8__locals2.newPeripheral.PeripheralPhysicalType = peripheralDevice.PeripheralPhysicalType;
						}
						else
						{
							CS$<>8__locals2.newPeripheral.InitPeripheralPhysicalType();
						}
						if (!CS$<>8__locals2.newPeripheral.IsInitializedController && this.InitializedDevices.FindCompositeForSimple(CS$<>8__locals2.newPeripheral.ID) != null)
						{
							CS$<>8__locals2.newPeripheral.SetIsInitialized(true);
						}
						CompositeDevice compositeDevice = this.CompositeDevices.FindCompositeForSimple(CS$<>8__locals2.newPeripheral);
						if (compositeDevice == null)
						{
							Tracer.TraceWrite("Add simple peripheral controller to collection", false);
							this.SetOnlineState(CS$<>8__locals2.newPeripheral, true);
							if (isNewDevice)
							{
								this.AddGamepadToCollection(CS$<>8__locals2.newPeripheral);
								await CS$<>8__locals2.newPeripheral.RefreshCurrentSlot();
								ControllerChangedHandler onControllerChanged = this.OnControllerChanged;
								if (onControllerChanged != null)
								{
									onControllerChanged(CS$<>8__locals2.newPeripheral);
								}
							}
						}
						else
						{
							CS$<>8__locals2.newPeripheral.IsOnline = true;
							Tracer.TraceWrite("Process peripheral controller part of CompositeDevice", false);
							this.AddNewDeviceToComposite(compositeDevice, CS$<>8__locals2.newPeripheral, changedComposites, flag);
						}
						if (isNewDevice)
						{
							this.AddGamepadToPhysicalCollection(CS$<>8__locals2.newPeripheral);
						}
						this.CheckControllerSettings(CS$<>8__locals2.newPeripheral);
						CS$<>8__locals2 = null;
					}
				}
			}
			IEnumerator<IGrouping<Guid, REWASD_CONTROLLER_INFO>> enumerator = null;
		}

		private async Task AddGamepadControllers(List<REWASD_CONTROLLER_INFO> controllersForAdd, HashSet<CompositeControllerVM> changedComposites)
		{
			using (List<REWASD_CONTROLLER_INFO>.Enumerator enumerator = controllersForAdd.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					REWASD_CONTROLLER_INFO newItem = enumerator.Current;
					if (this.BlacklistGamepads.FirstOrDefault((BlackListGamepad item) => item.ID == newItem.Id.ToString()) == null)
					{
						Tracer.TraceWriteTag(" - Controllers", "Processing \"gamepad\" device", false);
						ControllerVM controller = new ControllerVM(newItem, false);
						if (!controller.IsInitializedController)
						{
							InitializedDevice initializedDevice = this.InitializedDevices.FindCompositeForSimple(controller.ControllerId);
							if (initializedDevice != null)
							{
								controller.InitializedDeviceType = initializedDevice.DeviceType;
								controller.SetIsInitialized(true);
							}
						}
						CompositeDevice compositeDevice = this.CompositeDevices.FindCompositeForSimple(controller);
						if (compositeDevice == null)
						{
							Tracer.TraceWrite("Add simple controller to collection", false);
							this.SetOnlineState(controller, true);
							this.AddGamepadToCollection(controller);
						}
						else
						{
							Tracer.TraceWrite("Process peripheral controller part of CompositeDevice", false);
							controller.IsOnline = true;
							this.AddNewDeviceToComposite(compositeDevice, controller, changedComposites, false);
							this.RefreshLedForController(controller);
						}
						this.CheckControllerSettings(controller);
						this.AddGamepadToPhysicalCollection(controller);
						this.AddGamepadToGamepadsAuthCollection(controller);
						SenderGoogleAnalytics.SendControllerInfoEvent(controller.ContainerId.ToString(), controller.ControllerInfoString);
						ServiceResponseWrapper<REWASD_GET_CONTROLLER_STATE_RESPONSE> serviceResponseWrapper = await this._xbServiceCommunicator.GetControllerState(newItem.Id, newItem.Type, false);
						if (serviceResponseWrapper.IsResponseValid && serviceResponseWrapper.ServiceResponse.Header.Status == 0U && serviceResponseWrapper.ServiceResponse.Header.Size > 0U)
						{
							controller.UpdateControllerState(serviceResponseWrapper.ServiceResponse);
						}
						await controller.RefreshCurrentSlot();
						controller = null;
					}
				}
			}
			List<REWASD_CONTROLLER_INFO>.Enumerator enumerator = default(List<REWASD_CONTROLLER_INFO>.Enumerator);
		}

		private async Task RemoveAddVirtualControllers(List<REWASD_CONTROLLER_INFO> virtualControllersList)
		{
			foreach (ControllerVM controllerVM in new List<ControllerVM>(this.VirtualGamepadCollection))
			{
				bool flag = false;
				if (virtualControllersList.Count > 0)
				{
					foreach (REWASD_CONTROLLER_INFO rewasd_CONTROLLER_INFO in virtualControllersList)
					{
						if (controllerVM.ControllerId == rewasd_CONTROLLER_INFO.Id)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					this.VirtualGamepadCollection.Remove(controllerVM);
				}
			}
			if (virtualControllersList.Count > 0)
			{
				foreach (REWASD_CONTROLLER_INFO newVirtualItem in virtualControllersList)
				{
					bool flag2 = false;
					foreach (ControllerVM existVirtualItem in this.VirtualGamepadCollection)
					{
						ControllerVM existVirtualItem;
						if (existVirtualItem.ControllerId == newVirtualItem.Id)
						{
							existVirtualItem.UpdateControllerInfo(newVirtualItem);
							ServiceResponseWrapper<REWASD_GET_CONTROLLER_STATE_RESPONSE> serviceResponseWrapper = await this._xbServiceCommunicator.GetControllerState(newVirtualItem.Id, newVirtualItem.Type, false);
							if (serviceResponseWrapper.IsResponseValid && serviceResponseWrapper.ServiceResponse.Header.Status == 0U && serviceResponseWrapper.ServiceResponse.Header.Size > 0U)
							{
								existVirtualItem.UpdateControllerState(serviceResponseWrapper.ServiceResponse);
							}
							flag2 = true;
							break;
						}
						existVirtualItem = null;
					}
					IEnumerator<ControllerVM> enumerator4 = null;
					if (!flag2)
					{
						Tracer.TraceWriteTag(" - Controllers", "Processing \"virtual\" device", false);
						ControllerVM existVirtualItem = new ControllerVM(newVirtualItem, false);
						this.VirtualGamepadCollection.Add(existVirtualItem);
						ServiceResponseWrapper<REWASD_GET_CONTROLLER_STATE_RESPONSE> serviceResponseWrapper2 = await this._xbServiceCommunicator.GetControllerState(newVirtualItem.Id, newVirtualItem.Type, false);
						if (serviceResponseWrapper2.IsResponseValid && serviceResponseWrapper2.ServiceResponse.Header.Status == 0U && serviceResponseWrapper2.ServiceResponse.Header.Size > 0U)
						{
							existVirtualItem.UpdateControllerState(serviceResponseWrapper2.ServiceResponse);
						}
						await existVirtualItem.RefreshCurrentSlot();
						existVirtualItem = null;
					}
					newVirtualItem = default(REWASD_CONTROLLER_INFO);
				}
				List<REWASD_CONTROLLER_INFO>.Enumerator enumerator3 = default(List<REWASD_CONTROLLER_INFO>.Enumerator);
			}
		}

		private void CreateNewCompositeDevicesFromCompositeDevicesList(HashSet<CompositeControllerVM> changedComposites)
		{
			this.CompositeDevices.ForEach(delegate(CompositeDevice composite)
			{
				CompositeControllerVM compositeVM = this.GamepadCollection.OfType<CompositeControllerVM>().FirstOrDefault((CompositeControllerVM cc) => cc.ID == composite.ID);
				if (compositeVM == null)
				{
					compositeVM = new CompositeControllerVM(composite, false);
					compositeVM.BaseControllers.ForEach(delegate(BaseControllerVM controller)
					{
						if (controller != null)
						{
							BaseControllerVM baseControllerVM = this.AllPhysicalControllers.FirstOrDefault((BaseControllerVM item) => item.ID == controller.ID);
							if (baseControllerVM != null)
							{
								compositeVM.AddController(baseControllerVM);
							}
						}
					});
					compositeVM.FillFriendlyName();
					this.AddGamepadToCollection(compositeVM);
					this.CheckControllerSettings(compositeVM);
					changedComposites.Add(compositeVM);
				}
			});
		}

		public async Task RefreshGamepadCollection(string ID = null)
		{
			List<Tuple<ulong, uint>> duplicateGamepadCollectionPrev = new List<Tuple<ulong, uint>>(this.DuplicateGamepadCollection);
			AsyncLock.Releaser releaser2 = await new AsyncLock(GamepadService._refreshGamepadCollectionSemaphore).LockAsync();
			using (releaser2)
			{
				Tracer.TraceWriteTag(" - Controllers", ">>>>>>>>>>>>>>>>>>>>>>>>", false);
				Tracer.TraceWriteTag(" - Controllers", "RefreshGamepadCollection", false);
				List<REWASD_CONTROLLER_INFO> list2;
				List<REWASD_CONTROLLER_INFO> list = (list2 = await this._xbServiceCommunicator.GetControllersList(false, true, false));
				this.IsExclusiveCaptureControllersPresent = list2.Any((REWASD_CONTROLLER_INFO x) => UtilsCommon.IsExclusiveCaptureControllerPhysicalType(x.Type));
				List<REWASD_CONTROLLER_INFO> list3 = list.Where((REWASD_CONTROLLER_INFO x) => x.ConnectionType != 3).ToList<REWASD_CONTROLLER_INFO>();
				List<REWASD_CONTROLLER_INFO> virtualControllersList = list.Where((REWASD_CONTROLLER_INFO x) => x.ConnectionType == 3).ToList<REWASD_CONTROLLER_INFO>();
				List<REWASD_CONTROLLER_INFO> controllersForAdd = new List<REWASD_CONTROLLER_INFO>();
				List<REWASD_CONTROLLER_INFO> controllersPeripheralForAdd = new List<REWASD_CONTROLLER_INFO>();
				this.DuplicateGamepadCollection.Clear();
				GamepadService.UpdateSimpleDeviceInfoList(this.SimpleDeviceInfoList, ref list);
				HashSet<CompositeControllerVM> changedComposites = new HashSet<CompositeControllerVM>();
				this.CreateNewCompositeDevicesFromCompositeDevicesList(changedComposites);
				this.RemoveControllersWhichShouldBeInCompositeController();
				this.SetOfflineNonExistControllers(list3);
				if (list3.Count > 0)
				{
					await this.UpdateExistingControllers(list3, controllersPeripheralForAdd, controllersForAdd, changedComposites);
					await this.AddPeripheralDevices(controllersPeripheralForAdd, changedComposites);
					await this.AddGamepadControllers(controllersForAdd, changedComposites);
					foreach (CompositeControllerVM compositeControllerVM in changedComposites)
					{
						this.CheckControllerSettings(compositeControllerVM);
						ControllerChangedHandler onControllerChanged = this.OnControllerChanged;
						if (onControllerChanged != null)
						{
							onControllerChanged(compositeControllerVM);
						}
					}
					if (!string.IsNullOrEmpty(ID))
					{
						this.GamepadCollection.FirstOrDefault((BaseControllerVM item) => item.IsConsideredTheSameControllerByID(ID));
					}
					else
					{
						string prevGamepadID = RegistryHelper.GetString("Config", "CurrentGamepad", "", false);
						if (this.GamepadCollection.FirstOrDefault((BaseControllerVM item) => item.ID == prevGamepadID) == null)
						{
							this.GamepadCollection.FirstOrDefault((BaseControllerVM item) => ControllerTypeExtensions.IsGamepad(item.FirstControllerType));
						}
					}
					if (this.GamepadCollection.Count > 1 && GamepadService.SentGamepadNumber < this.GamepadCollection.Count)
					{
						GamepadService.SentGamepadNumber = this.GamepadCollection.Count;
						SenderGoogleAnalytics.SendMessageEvent("GUI", "GamepadNumber", this.GamepadCollection.Count.ToString(), -1L, false);
					}
				}
				await this.RemoveAddVirtualControllers(virtualControllersList);
				virtualControllersList = null;
				controllersForAdd = null;
				controllersPeripheralForAdd = null;
				changedComposites = null;
			}
			AsyncLock.Releaser releaser = default(AsyncLock.Releaser);
			this.IsServiceInited = true;
			if (duplicateGamepadCollectionPrev.Count != this.DuplicateGamepadCollection.Count)
			{
				GamepadService.<>c__DisplayClass221_2 CS$<>8__locals3 = new GamepadService.<>c__DisplayClass221_2();
				List<ulong> list4 = (from dupNew in this.DuplicateGamepadCollection
					where !duplicateGamepadCollectionPrev.Exists((Tuple<ulong, uint> x) => x.Item1 == dupNew.Item1 && x.Item2 == dupNew.Item2)
					select dupNew.Item1).ToList<ulong>();
				list4.AddRange(from dupOld in duplicateGamepadCollectionPrev
					where !this.DuplicateGamepadCollection.Exists((Tuple<ulong, uint> x) => x.Item1 == dupOld.Item1 && x.Item2 == dupOld.Item2)
					select dupOld.Item1);
				foreach (ulong id in list4.Distinct<ulong>().ToList<ulong>())
				{
					if (!this.ServiceProfilesCollection.ContainsProfileForID(id.ToString()))
					{
						Tracer.TraceWrite("GamepadService no profile found. refreshing", false);
						await this.RefreshServiceProfiles();
					}
					if (this.ServiceProfilesCollection.ContainsProfileForID(id.ToString()))
					{
						this.CachedProfilesCollection.Clear();
						Tracer.TraceWrite("GamepadService profile founded", false);
						await this.EnableRemap(false, id.ToString(), false, false, true, -1, true, false, true, null, null);
					}
				}
				List<ulong>.Enumerator enumerator2 = default(List<ulong>.Enumerator);
				CS$<>8__locals3.dubID = (duplicateGamepadCollectionPrev.Any<Tuple<ulong, uint>>() ? duplicateGamepadCollectionPrev : this.DuplicateGamepadCollection)[0].Item1;
				BaseControllerVM baseControllerVM = this.GamepadCollection.FirstOrDefault((BaseControllerVM item) => item.ID == CS$<>8__locals3.dubID.ToString());
				if (baseControllerVM != null)
				{
					ControllerChangedHandler onControllerChanged2 = this.OnControllerChanged;
					if (onControllerChanged2 != null)
					{
						onControllerChanged2(baseControllerVM);
					}
					this.RefreshGamepadStatesAndReactOnBattery(baseControllerVM, false);
				}
				CS$<>8__locals3 = null;
			}
			this.ExternalDeviceRelationsHelper.Refresh();
			await this.RefreshInputDevices();
			this.BinDataSerialize.SaveGamepadCollection();
		}

		private async Task RefreshLedForController(BaseControllerVM controller)
		{
			await controller.RefreshCurrentSlot();
			GamepadProfiles gamepadProfiles = this.GamepadProfileRelations.FindProfileByControllerId(controller.ID);
			GamepadProfile gamepadProfile = ((gamepadProfiles != null) ? gamepadProfiles.SlotProfiles.TryGetValue(controller.CurrentSlot) : null);
			await Engine.LEDService.ApplyLEDsToControllerAccordingToSettings(controller, gamepadProfile, new Slot?(controller.CurrentSlot), 0, false, false);
		}

		private void AddOfflineDevice(BaseControllerVM controller, bool useState)
		{
			if (!controller.IsDebugController)
			{
				if (useState)
				{
					this.SetOnlineState(controller, false);
				}
				else
				{
					controller.IsOnline = false;
				}
				GamepadStateChanged onGamepadStateChanged = this.OnGamepadStateChanged;
				if (onGamepadStateChanged == null)
				{
					return;
				}
				onGamepadStateChanged(controller);
			}
		}

		private void RemoveControllersWhichShouldBeInCompositeController()
		{
			List<BaseControllerVM> list = new List<BaseControllerVM>();
			foreach (BaseControllerVM baseControllerVM in this.GamepadCollection)
			{
				if (baseControllerVM.IsInsideCompositeDevice)
				{
					ControllerVM controllerVM2 = baseControllerVM as ControllerVM;
					PeripheralVM peripheralVM = baseControllerVM as PeripheralVM;
					if (this.CompositeDevices.FindCompositeForSimple(controllerVM2) != null || this.CompositeDevices.FindCompositeForSimple(peripheralVM) != null)
					{
						list.Add(baseControllerVM);
					}
				}
				else
				{
					CompositeControllerVM compositeController = baseControllerVM as CompositeControllerVM;
					if (compositeController != null && this.CompositeDevices.All((CompositeDevice cd) => cd.ID != compositeController.ID))
					{
						list.Add(baseControllerVM);
					}
				}
			}
			foreach (BaseControllerVM baseControllerVM2 in list)
			{
				CompositeControllerVM compositeControllerVM = baseControllerVM2 as CompositeControllerVM;
				if (compositeControllerVM != null)
				{
					using (List<BaseControllerVM>.Enumerator enumerator3 = compositeControllerVM.BaseControllers.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							BaseControllerVM controllerVM = enumerator3.Current;
							if (controllerVM != null)
							{
								this.AllPhysicalControllers.Remove((BaseControllerVM it) => it.ID == controllerVM.ID);
								this.RemoveGamepadFromGamepadsAuthCollection(controllerVM);
							}
						}
					}
					this.GamepadCollection.Remove(baseControllerVM2);
				}
			}
		}

		private void SetOfflineNonExistControllers(List<REWASD_CONTROLLER_INFO> realControllersList)
		{
			List<BaseControllerVM> list = new List<BaseControllerVM>();
			using (IEnumerator<BaseControllerVM> enumerator = this.GamepadCollection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					BaseControllerVM existItem = enumerator.Current;
					bool flag = false;
					ControllerVM controllerVM = existItem as ControllerVM;
					if (controllerVM == null)
					{
						BaseCompositeControllerVM baseCompositeControllerVM = existItem as BaseCompositeControllerVM;
						if (baseCompositeControllerVM != null)
						{
							foreach (BaseControllerVM baseControllerVM in new List<BaseControllerVM>(baseCompositeControllerVM.BaseControllers))
							{
								if (baseControllerVM != null && !realControllersList.ContainsControllerInfoForControllerId(baseControllerVM.CurrentController.ControllerId))
								{
									if (!baseCompositeControllerVM.IsCompositeDevice)
									{
										list.Add(baseCompositeControllerVM);
										flag = true;
										break;
									}
									if (baseControllerVM.IsOnline)
									{
										this.AddOfflineDevice(baseControllerVM, false);
									}
									this.GamepadsHotkeyCollection.RefreshEntry(baseCompositeControllerVM.ID, string.IsNullOrEmpty(baseCompositeControllerVM.ControllerFriendlyName) ? baseCompositeControllerVM.ControllerDisplayName : baseCompositeControllerVM.ControllerFriendlyName);
									ControllerChangedHandler onControllerChanged = this.OnControllerChanged;
									if (onControllerChanged != null)
									{
										onControllerChanged(baseCompositeControllerVM);
									}
								}
							}
							if (baseCompositeControllerVM.IsCompositeDevice && !baseCompositeControllerVM.HasOnlineDevice)
							{
								list.Add(baseCompositeControllerVM);
								flag = true;
							}
						}
					}
					else if (!realControllersList.ContainsControllerInfoForControllerId(controllerVM.ControllerId) && !controllerVM.IsDebugController)
					{
						list.Add(controllerVM);
						flag = true;
					}
					if (!flag && this.BlacklistGamepads.FirstOrDefault((BlackListGamepad item) => item.ID == existItem.ID) != null)
					{
						list.Add(existItem);
					}
				}
			}
			foreach (BaseControllerVM baseControllerVM2 in list)
			{
				ControllerVM controllerVM2 = baseControllerVM2 as ControllerVM;
				if (controllerVM2 != null)
				{
					this.RemoveGamepadFromGamepadsAuthCollection(controllerVM2);
				}
				if (baseControllerVM2.IsOnline)
				{
					this.AddOfflineDevice(baseControllerVM2, true);
				}
				if (this.IsBlacklistedDevice(baseControllerVM2.ID))
				{
					this.GamepadCollection.Remove(baseControllerVM2);
					this.AllPhysicalControllers.Remove(baseControllerVM2);
				}
			}
		}

		private async void AddNewDeviceToComposite(CompositeDevice composite, BaseControllerVM newDevice, HashSet<CompositeControllerVM> changedComposites, bool becomeOnline = false)
		{
			CompositeControllerVM compositeVM = this.GamepadCollection.OfType<CompositeControllerVM>().FirstOrDefault((CompositeControllerVM cc) => cc.ID == composite.ID);
			if (compositeVM == null)
			{
				compositeVM = new CompositeControllerVM(composite, false);
				this.AddGamepadToCollection(compositeVM);
			}
			if (compositeVM.FindController(newDevice.ID) is OfflineDeviceVM || becomeOnline)
			{
				changedComposites.Add(compositeVM);
			}
			compositeVM.AddController(newDevice);
			compositeVM.FillFriendlyName();
			this.SetOnlineState(compositeVM, true);
			await compositeVM.RefreshCurrentSlot();
			if (!this.IsBlacklistedDevice(compositeVM.ID))
			{
				this.GamepadsHotkeyCollection.AddDefaultEntryIfNotPresent(compositeVM.ID, compositeVM.Types, compositeVM.ControllerFamily, compositeVM.ControllerDisplayName, compositeVM.Ids);
			}
		}

		public BaseControllerVM FindControllerBySingleId(string id)
		{
			return this.GamepadCollection.FirstOrDefault(delegate(BaseControllerVM c)
			{
				if (!(c.ID == id))
				{
					BaseCompositeControllerVM baseCompositeControllerVM = c as BaseCompositeControllerVM;
					return baseCompositeControllerVM != null && baseCompositeControllerVM.FindController(id) != null;
				}
				return true;
			});
		}

		public async Task RefreshServiceProfileState(ulong serviceProfileId, int parameter)
		{
			bool connectionChanged = Convert.ToBoolean((long)parameter & 1L);
			Convert.ToBoolean((long)parameter & 2L);
			Convert.ToBoolean((long)parameter & 4L);
			Convert.ToBoolean((long)parameter & 16L);
			AsyncLock.Releaser releaser2 = await new AsyncLock(GamepadService._refreshServiceProfilesSemaphore).LockAsync();
			using (releaser2)
			{
				if (this.ServiceProfilesCollection != null)
				{
					REWASD_CONTROLLER_PROFILE_EX profileEx = this.GetProfileExByServiceProfileId(serviceProfileId);
					if (profileEx != null)
					{
						ServiceResponseWrapper<REWASD_GET_PROFILE_STATE_RESPONSE> serviceResponseWrapper = await this._xbServiceCommunicator.GetProfileState((ushort)serviceProfileId, false);
						if (serviceResponseWrapper.IsResponseValid)
						{
							int num = 0;
							ushort[] serviceProfileIds = profileEx.ServiceProfileIds;
							for (int i = 0; i < serviceProfileIds.Length; i++)
							{
								if ((ulong)serviceProfileIds[i] == serviceProfileId && num < profileEx.ProfilesState.Length)
								{
									bool flag = connectionChanged && profileEx.ProfilesState[num].RemoteConnected && !serviceResponseWrapper.ServiceResponse.RemoteConnected;
									if (flag)
									{
										Tracer.TraceWrite("ProfileState: Disconnected", false);
									}
									if (serviceResponseWrapper.ServiceResponse.VirtualType == 48U && Convert.ToBoolean((uint)(profileEx.Profiles[num].VirtualFlags & 64)))
									{
										profileEx.ExternalClientDisconnected[num] = flag && !serviceResponseWrapper.ServiceResponse.RemoteSleep;
									}
									else
									{
										profileEx.ExternalClientDisconnected[num] = flag;
									}
									profileEx.ProfilesState[num] = serviceResponseWrapper.ServiceResponse;
									this.TraceProfileState(serviceProfileId, ref profileEx.ProfilesState[num]);
									break;
								}
								num++;
							}
						}
					}
					profileEx = null;
				}
			}
			AsyncLock.Releaser releaser = default(AsyncLock.Releaser);
		}

		public async Task RefreshServiceProfiles()
		{
			AsyncLock.Releaser releaser2 = await new AsyncLock(GamepadService._refreshServiceProfilesSemaphore).LockAsync();
			using (releaser2)
			{
				List<REWASD_CONTROLLER_PROFILE_EX> list = await this._xbServiceCommunicator.GetProfilesList();
				foreach (Wrapper<REWASD_CONTROLLER_PROFILE_EX> wrapper in new List<Wrapper<REWASD_CONTROLLER_PROFILE_EX>>(this.ServiceProfilesCollection))
				{
					bool flag = false;
					if (list.Count > 0)
					{
						foreach (REWASD_CONTROLLER_PROFILE_EX rewasd_CONTROLLER_PROFILE_EX in list)
						{
							if (wrapper.Value.SlotsWrapperId == rewasd_CONTROLLER_PROFILE_EX.SlotsWrapperId)
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						this.ServiceProfilesCollection.Remove(wrapper);
					}
				}
				if (list.Count > 0)
				{
					List<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> list2 = new List<Wrapper<REWASD_CONTROLLER_PROFILE_EX>>();
					using (List<REWASD_CONTROLLER_PROFILE_EX>.Enumerator enumerator2 = list.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							REWASD_CONTROLLER_PROFILE_EX newItem = enumerator2.Current;
							bool flag2 = false;
							using (IEnumerator<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> enumerator3 = this.ServiceProfilesCollection.GetEnumerator())
							{
								Func<Wrapper<REWASD_CONTROLLER_PROFILE_EX>, bool> <>9__0;
								while (enumerator3.MoveNext())
								{
									if (enumerator3.Current.Value.SlotsWrapperId == newItem.SlotsWrapperId)
									{
										IEnumerable<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> serviceProfilesCollection = this.ServiceProfilesCollection;
										Func<Wrapper<REWASD_CONTROLLER_PROFILE_EX>, bool> func;
										if ((func = <>9__0) == null)
										{
											Func<Wrapper<REWASD_CONTROLLER_PROFILE_EX>, bool> func2 = (Wrapper<REWASD_CONTROLLER_PROFILE_EX> item) => item.Value.SlotsWrapperId == newItem.SlotsWrapperId;
											<>9__0 = func2;
											func = func2;
										}
										Wrapper<REWASD_CONTROLLER_PROFILE_EX> wrapper2 = serviceProfilesCollection.First(func);
										wrapper2.Value = newItem;
										flag2 = true;
										list2.Add(wrapper2);
										break;
									}
								}
							}
							if (!flag2)
							{
								this.ServiceProfilesCollection.Add(new Wrapper<REWASD_CONTROLLER_PROFILE_EX>(newItem));
							}
						}
					}
					if (list2.Any<Wrapper<REWASD_CONTROLLER_PROFILE_EX>>())
					{
						this.RefreshServiceStuffAfterServiceProfilesCollectionChanged();
					}
				}
				if (Engine.GamepadUdpServer != null)
				{
					List<ushort> udpIdsList = Engine.GamepadUdpServer.GetControllersServiceIds();
					List<ushort> currentIdsList = new List<ushort>();
					foreach (REWASD_CONTROLLER_PROFILE_EX rewasd_CONTROLLER_PROFILE_EX2 in list)
					{
						if (rewasd_CONTROLLER_PROFILE_EX2.ProfilesState.Any((REWASD_GET_PROFILE_STATE_RESPONSE x) => x.Enabled && x.VirtualType != 0U && REWASD_GET_PROFILE_STATE_RESPONSE_Extension.IsUdpPresent(x)))
						{
							for (int i = 0; i < rewasd_CONTROLLER_PROFILE_EX2.ProfilesState.Length; i++)
							{
								if (rewasd_CONTROLLER_PROFILE_EX2.ProfilesState[i].Enabled && rewasd_CONTROLLER_PROFILE_EX2.ProfilesState[i].VirtualType == 23U && REWASD_GET_PROFILE_STATE_RESPONSE_Extension.IsUdpPresent(rewasd_CONTROLLER_PROFILE_EX2.ProfilesState[i]))
								{
									currentIdsList.Add(rewasd_CONTROLLER_PROFILE_EX2.ServiceProfileIds[i]);
								}
							}
						}
					}
					IEnumerable<ushort> udpIdsList2 = udpIdsList;
					Func<ushort, bool> func3;
					Func<ushort, bool> <>9__2;
					if ((func3 = <>9__2) == null)
					{
						Func<ushort, bool> func4 = (ushort udpId) => !currentIdsList.Exists((ushort x) => x == udpId);
						<>9__2 = func4;
						func3 = func4;
					}
					foreach (ushort num in udpIdsList2.Where(func3))
					{
						Tracer.TraceWrite("GamepadService.RefreshServiceProfiles: DeleteUDPController", false);
						if (!Engine.GamepadUdpServer.DeleteUDPController(num))
						{
							Tracer.TraceWrite("GamepadService.RefreshServiceProfiles: DeleteUDPController return FALSE", false);
						}
					}
					IEnumerable<ushort> currentIdsList2 = currentIdsList;
					Func<ushort, bool> func5;
					Func<ushort, bool> <>9__4;
					if ((func5 = <>9__4) == null)
					{
						Func<ushort, bool> func6 = (ushort currentId) => !udpIdsList.Exists((ushort x) => x == currentId);
						<>9__4 = func6;
						func5 = func6;
					}
					foreach (ushort num2 in currentIdsList2.Where(func5))
					{
						Tracer.TraceWrite("GamepadService.RefreshServiceProfiles: AddUDPController", false);
						if (!Engine.GamepadUdpServer.AddUDPController(num2))
						{
							Tracer.TraceWrite("GamepadService.RefreshServiceProfiles: AddUDPController return FALSE", false);
						}
					}
				}
			}
			AsyncLock.Releaser releaser = default(AsyncLock.Releaser);
			if (Engine.GamepadUdpServer != null && Engine.GamepadUdpServer.IsUdpEnabledInPreferences && Engine.GamepadUdpServer.PadList.Count > 0 && Engine.GamepadUdpServer.IsUdpServerHasException && !Engine.GamepadUdpServer.IsRunning && Engine.GamepadUdpServer.IsPortNotLockedByOtherApplication())
			{
				Tracer.TraceWrite("GamepadService.RefreshServiceProfiles: GamepadUdpServer.AsyncStart", false);
				Engine.GamepadUdpServer.AsyncStart();
			}
		}

		public async Task RestoreDefaults(string ID, List<Slot> slots)
		{
			Tracer.TraceWrite("GamepadService.RestoreDefaults", false);
			GamepadProfiles gamepadProfiles = this.GamepadProfileRelations.TryGetValue(ID);
			if (gamepadProfiles != null)
			{
				Func<BaseControllerVM, bool> <>9__1;
				foreach (Slot slot in slots)
				{
					if (gamepadProfiles.SlotProfiles.ContainsKey(slot))
					{
						IEnumerable<BaseControllerVM> gamepadCollection = this.GamepadCollection;
						Func<BaseControllerVM, bool> func;
						if ((func = <>9__1) == null)
						{
							func = (<>9__1 = (BaseControllerVM g) => g.ID == ID);
						}
						BaseControllerVM baseControllerVM = gamepadCollection.FirstOrDefault(func);
						bool flag = baseControllerVM != null;
						if (flag)
						{
							flag = await this.CheckLedDeviceAffected(ID, slot, baseControllerVM.HasLEDMouse, baseControllerVM.HasLEDKeyboard);
						}
						if (flag)
						{
							Engine.LEDService.DecrementActiveConfigs();
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(53, 1);
						defaultInterpolatedStringHandler.AppendLiteral("RestoreDefaults: GamepadProfiles.SlotProfiles.Remove ");
						defaultInterpolatedStringHandler.AppendFormatted<Slot>(slot);
						Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
						gamepadProfiles.SlotProfiles.Remove(slot);
					}
				}
				List<Slot>.Enumerator enumerator = default(List<Slot>.Enumerator);
				bool isRemapOff = false;
				if (gamepadProfiles.SlotProfiles.Count == 0)
				{
					this.GamepadProfileRelations.Remove(ID);
					isRemapOff = true;
				}
				this.BinDataSerialize.SaveGamepadProfileRelations();
				if (!this.ServiceProfilesCollection.ContainsProfileForID(ID))
				{
					Tracer.TraceWrite("GamepadService no profile found. refreshing", false);
					await this.RefreshServiceProfiles();
				}
				List<Tuple<ulong, uint>> hiddenAppliedControllers = new List<Tuple<ulong, uint>>();
				if (this.ServiceProfilesCollection.ContainsProfileForID(ID))
				{
					Tracer.TraceWrite("GamepadService profile founded", false);
					TaskAwaiter<ushort> taskAwaiter = this.EnableRemap(false, ID, false, false, true, -1, false, false, false, null, null).GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						TaskAwaiter<ushort> taskAwaiter2;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<ushort>);
					}
					if (taskAwaiter.GetResult() == 0)
					{
						REWASD_CONTROLLER_PROFILE_EX profileEx = this.ServiceProfilesCollection.FindByID(ID);
						if (profileEx != null)
						{
							await this._xbServiceCommunicator.DeleteProfiles(profileEx.GetAllProfileIds());
							if (profileEx.Profiles.Length != 0)
							{
								for (int i = 0; i < profileEx.Profiles[0].Type.Length; i++)
								{
									if (UtilsCommon.IsHiddenAppliedConfigControllerPhysicalType(profileEx.Profiles[0].Type[i]))
									{
										hiddenAppliedControllers.Add(new Tuple<ulong, uint>(profileEx.Profiles[0].Id[i], profileEx.Profiles[0].Type[i]));
									}
								}
							}
						}
						profileEx = null;
					}
				}
				else
				{
					Tracer.TraceWrite("GamepadService no profile found.", false);
				}
				BaseControllerVM baseControllerVM2 = this.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.ID == ID);
				if (baseControllerVM2 != null && isRemapOff)
				{
					RemapStateChangedHandler onRemapStateChanged = this.OnRemapStateChanged;
					if (onRemapStateChanged != null)
					{
						onRemapStateChanged(baseControllerVM2, 0);
					}
				}
				if (hiddenAppliedControllers.Count > 0)
				{
					foreach (Tuple<ulong, uint> tuple in hiddenAppliedControllers)
					{
						await this.CompileHiddenAppliedConfig(tuple.Item1, tuple.Item2, true);
					}
					List<Tuple<ulong, uint>>.Enumerator enumerator2 = default(List<Tuple<ulong, uint>>.Enumerator);
				}
			}
		}

		public bool IsAsyncRemapInProgress { get; set; }

		public bool IsAnyGamepadRemaped
		{
			get
			{
				return this.ServiceProfilesCollection.Any<Wrapper<REWASD_CONTROLLER_PROFILE_EX>>();
			}
		}

		private bool IsGamepadRemaped(BaseControllerVM gamepad)
		{
			return gamepad != null && this.ServiceProfilesCollection.ContainsProfileForID(gamepad.ID);
		}

		private BaseControllerVM GetAssociatedController(REWASD_CONTROLLER_PROFILE_EX controllerProfile)
		{
			REWASD_CONTROLLER_PROFILE nonEmptyprofile = controllerProfile.Profiles.FirstOrDefault((REWASD_CONTROLLER_PROFILE p) => REWASD_CONTROLLER_PROFILE_Extensions.IsSlotWithUserConfig(p));
			if (nonEmptyprofile.Id == null)
			{
				return null;
			}
			BaseControllerVM baseControllerVM = this.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.IsConsideredTheSameControllerByID(nonEmptyprofile.Id));
			if (baseControllerVM == null)
			{
				Func<BaseControllerVM, bool> <>9__2;
				foreach (BaseControllerVM baseControllerVM2 in this.GamepadCollection)
				{
					CompositeControllerVM compositeControllerVM = baseControllerVM2 as CompositeControllerVM;
					if (compositeControllerVM != null)
					{
						IEnumerable<BaseControllerVM> baseControllers = compositeControllerVM.BaseControllers;
						Func<BaseControllerVM, bool> func;
						if ((func = <>9__2) == null)
						{
							func = (<>9__2 = (BaseControllerVM g) => g != null && !(g is OfflineDeviceVM) && g.IsConsideredTheSameControllerByID(nonEmptyprofile.Id));
						}
						baseControllerVM = baseControllers.FirstOrDefault(func);
						if (baseControllerVM != null)
						{
							baseControllerVM = compositeControllerVM;
							break;
						}
					}
				}
			}
			return baseControllerVM;
		}

		private async void OnServiceProfilesCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.RefreshServiceStuffAfterServiceProfilesCollectionChanged();
			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				if (!Engine.UserSettingsService.IsLedSettingsEnabled)
				{
					return;
				}
				foreach (object obj in e.OldItems)
				{
					Wrapper<REWASD_CONTROLLER_PROFILE_EX> wrapper = (Wrapper<REWASD_CONTROLLER_PROFILE_EX>)obj;
					GamepadService.<>c__DisplayClass239_0 CS$<>8__locals1 = new GamepadService.<>c__DisplayClass239_0();
					List<Tuple<ulong, uint>> gamepadIdTypes = wrapper.Value.GetGamepadIdTypes(true);
					CS$<>8__locals1.id = wrapper.Value.GetID(null);
					Slot slot = wrapper.Value.GetCurrentSlot();
					bool hasMouse = wrapper.Value.HasMouse();
					bool hasKeyboard = wrapper.Value.HasKeyboard();
					BaseControllerVM controller = this.GamepadCollection.FirstOrDefault((BaseControllerVM item) => item.ID == CS$<>8__locals1.id);
					if (controller == null)
					{
						controller = this.GetAssociatedController(wrapper.Value);
					}
					if (controller != null)
					{
						CS$<>8__locals1.id = controller.ID;
						slot = controller.CurrentSlot;
						hasMouse = controller.HasLEDMouse;
						hasKeyboard = controller.HasLEDKeyboard;
					}
					if (!string.IsNullOrEmpty(CS$<>8__locals1.id))
					{
						if (controller != null && controller.IsOnline && !controller.IsDebugController)
						{
							foreach (Tuple<ulong, uint> tuple in gamepadIdTypes)
							{
								await Engine.LEDService.ResetGamepadColor(new LEDDeviceInfo(tuple.Item1, tuple.Item2, 0), true, true, false, false);
							}
							List<Tuple<ulong, uint>>.Enumerator enumerator2 = default(List<Tuple<ulong, uint>>.Enumerator);
							await Engine.LEDService.Stop(controller.HasAnyEngineControllers, hasMouse, hasKeyboard);
						}
						TaskAwaiter<bool> taskAwaiter = this.CheckLedDeviceAffected(CS$<>8__locals1.id, slot, hasMouse, hasKeyboard).GetAwaiter();
						if (!taskAwaiter.IsCompleted)
						{
							await taskAwaiter;
							TaskAwaiter<bool> taskAwaiter2;
							taskAwaiter = taskAwaiter2;
							taskAwaiter2 = default(TaskAwaiter<bool>);
						}
						if (taskAwaiter.GetResult())
						{
							Engine.LEDService.DecrementActiveConfigs();
						}
						if (controller != null)
						{
							ILEDService ledservice = Engine.LEDService;
							BaseControllerVM baseControllerVM = controller;
							GamepadProfiles gamepadProfiles2 = this.GamepadProfileRelations.TryGetValue(controller.ID);
							await ledservice.ApplyLEDsToControllerAccordingToSettings(baseControllerVM, (gamepadProfiles2 != null) ? gamepadProfiles2.SlotProfiles.TryGetValue(slot) : null, new Slot?(slot), 0, false, false);
						}
						CS$<>8__locals1 = null;
						controller = null;
					}
				}
				IEnumerator enumerator = null;
			}
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (object obj2 in e.NewItems)
				{
					Wrapper<REWASD_CONTROLLER_PROFILE_EX> wrapper2 = (Wrapper<REWASD_CONTROLLER_PROFILE_EX>)obj2;
					GamepadService.<>c__DisplayClass239_1 CS$<>8__locals2 = new GamepadService.<>c__DisplayClass239_1();
					CS$<>8__locals2.<>4__this = this;
					REWASD_CONTROLLER_PROFILE? profile = wrapper2.Value.GetCurrentSlotProfile();
					if (profile == null)
					{
						Tracer.TraceWrite("ServiceProfilesCollectionOnCollectionChanged.Add profile is null", false);
					}
					else
					{
						GamepadProfiles gamepadProfiles = null;
						try
						{
							GamepadService.<>c__DisplayClass239_2 CS$<>8__locals3 = new GamepadService.<>c__DisplayClass239_2();
							ulong[] ids = profile.Value.Id.ToArray<ulong>();
							uint[] types = profile.Value.Type.ToArray<uint>();
							if (this.AllPhysicalControllers.Any((BaseControllerVM x) => x.ControllerTypeEnums.Contains(36) || x.ControllerTypeEnums.Contains(26) || x.ControllerTypeEnums.Contains(54)) && ids.Length == types.Length)
							{
								GamepadService.<>c__DisplayClass239_3 CS$<>8__locals4 = new GamepadService.<>c__DisplayClass239_3();
								CS$<>8__locals4.fullControllersList = await this._xbServiceCommunicator.GetControllersList(true, false, false);
								using (List<BaseControllerVM>.Enumerator enumerator3 = this.AllPhysicalControllers.Where((BaseControllerVM x) => x.ControllerTypeEnums.Any((ControllerTypeEnum y) => ControllerTypeExtensions.IsGamepadWithBuiltInAnyKeypad(y)) && CS$<>8__locals4.fullControllersList.Exists((REWASD_CONTROLLER_INFO y) => y.ContainerId.Equals(x.ContainerId) && y.Type == 2147483650U)).ToList<BaseControllerVM>().GetEnumerator())
								{
									while (enumerator3.MoveNext())
									{
										BaseControllerVM gamepadWithBuiltInAnyKeypad = enumerator3.Current;
										foreach (REWASD_CONTROLLER_INFO rewasd_CONTROLLER_INFO in CS$<>8__locals4.fullControllersList.Where((REWASD_CONTROLLER_INFO x) => x.ContainerId.Equals(gamepadWithBuiltInAnyKeypad.ContainerId) && x.Type == 2147483650U).ToList<REWASD_CONTROLLER_INFO>())
										{
											for (int i = 0; i < ids.Length; i++)
											{
												if (ids[i] == rewasd_CONTROLLER_INFO.Id && types[i] == 2147483650U)
												{
													ids[i] = 0UL;
												}
											}
										}
									}
								}
								using (List<BaseControllerVM>.Enumerator enumerator3 = this.AllPhysicalControllers.Where((BaseControllerVM x) => x.ControllerTypeEnums.Any((ControllerTypeEnum y) => ControllerTypeExtensions.IsGamepadWithBuiltInMouse(y)) && CS$<>8__locals4.fullControllersList.Exists((REWASD_CONTROLLER_INFO y) => y.ContainerId.Equals(x.ContainerId) && y.Type == 2147483649U)).ToList<BaseControllerVM>().GetEnumerator())
								{
									while (enumerator3.MoveNext())
									{
										BaseControllerVM gamepadWithBuiltInMouse = enumerator3.Current;
										foreach (REWASD_CONTROLLER_INFO rewasd_CONTROLLER_INFO2 in CS$<>8__locals4.fullControllersList.Where((REWASD_CONTROLLER_INFO x) => x.ContainerId.Equals(gamepadWithBuiltInMouse.ContainerId) && x.Type == 2147483649U).ToList<REWASD_CONTROLLER_INFO>())
										{
											for (int j = 0; j < ids.Length; j++)
											{
												if (ids[j] == rewasd_CONTROLLER_INFO2.Id && types[j] == 2147483649U)
												{
													ids[j] = 0UL;
												}
											}
										}
									}
								}
								CS$<>8__locals4 = null;
							}
							CS$<>8__locals3.Id = XBUtils.CalculateID(ids);
							gamepadProfiles = this.GamepadProfileRelations.FirstOrDefault((KeyValuePair<string, GamepadProfiles> kvp) => CS$<>8__locals3.Id == kvp.Key).Value;
							CS$<>8__locals3 = null;
							ids = null;
							types = null;
						}
						catch
						{
						}
						if (gamepadProfiles == null)
						{
							Tracer.TraceWrite("ServiceProfilesCollectionOnCollectionChanged.Add gamepadProfiles is null", false);
						}
						else
						{
							CS$<>8__locals2.controller = gamepadProfiles.GetAssociatedController();
							if (CS$<>8__locals2.controller == null)
							{
								Tracer.TraceWrite("ServiceProfilesCollectionOnCollectionChanged.Add controller is null", false);
							}
							else
							{
								await CS$<>8__locals2.controller.RefreshCurrentSlot();
								CS$<>8__locals2.slot = CS$<>8__locals2.controller.CurrentSlot;
								CS$<>8__locals2.gamepadProfile = gamepadProfiles.SlotProfiles.TryGetValue(CS$<>8__locals2.slot);
								if (CS$<>8__locals2.gamepadProfile != null)
								{
									CS$<>8__locals2.controller.FillFriendlyName();
									ConfigAppliedToSlotHandler onConfigAppliedToSlot = this.OnConfigAppliedToSlot;
									if (onConfigAppliedToSlot != null)
									{
										onConfigAppliedToSlot(CS$<>8__locals2.controller, CS$<>8__locals2.gamepadProfile, CS$<>8__locals2.slot, profile);
									}
									this.SaveSlot(CS$<>8__locals2.controller.ShortID, CS$<>8__locals2.slot);
								}
								await CS$<>8__locals2.controller.RefreshCurrentSlot();
								if (!Engine.UserSettingsService.IsLedSettingsEnabled)
								{
									return;
								}
								CS$<>8__locals2.affected = await this.CheckLedDeviceAffected(CS$<>8__locals2.controller);
								new Thread(delegate
								{
									if (CS$<>8__locals2.affected)
									{
										Engine.LEDService.IncrementActiveConfigs();
									}
									Engine.LEDService.ApplyLEDsToControllerAccordingToSettings(CS$<>8__locals2.controller, CS$<>8__locals2.gamepadProfile, new Slot?(CS$<>8__locals2.slot), 0, false, false);
									if (CS$<>8__locals2.controller.HasGamepadControllers)
									{
										CS$<>8__locals2.<>4__this.RefreshGamepadStatesAndReactOnBattery(CS$<>8__locals2.controller, false);
									}
								}).Start();
								CS$<>8__locals2 = null;
								profile = null;
								gamepadProfiles = null;
							}
						}
					}
				}
				IEnumerator enumerator = null;
			}
		}

		protected async Task<bool> CheckLedDeviceAffected(BaseControllerVM controller)
		{
			return await this.CheckLedDeviceAffected(controller.ID, controller.CurrentSlot, controller.HasLEDKeyboard, controller.HasLEDMouse);
		}

		protected Task<bool> CheckLedDeviceAffected(string ID, Slot slot, bool hasLEDKeyboard, bool hasLEDMouse)
		{
			if (!hasLEDKeyboard && !hasLEDMouse)
			{
				return Task.FromResult<bool>(false);
			}
			GamepadProfiles value = this.GamepadProfileRelations.FirstOrDefault((KeyValuePair<string, GamepadProfiles> kvp) => kvp.Value.ID.Contains(ID)).Value;
			if (value != null && value.SlotProfiles.Count > 0)
			{
				GamepadProfile gamepadProfile = value.SlotProfiles.TryGetValue(slot);
				Config config = ((gamepadProfile != null) ? gamepadProfile.Config : null);
				if (config != null)
				{
					config.ReadConfigFromJsonIfNotLoaded(false);
				}
				foreach (LEDSupportedDevice ledsupportedDevice in new List<LEDSupportedDevice> { 9, 8 })
				{
					LEDSettingsGlobalPerDevice ledsettingsGlobalPerDevice = this._userSettingsService.PerDeviceGlobalLedSettings[ledsupportedDevice];
					if (config != null)
					{
						ConfigData configData = config.ConfigData;
						bool? flag = ((configData != null) ? new bool?(configData.IsConfigShouldBeApplied) : null);
						bool flag2 = true;
						if ((flag.GetValueOrDefault() == flag2) & (flag != null))
						{
							goto IL_F6;
						}
					}
					if (!ledsettingsGlobalPerDevice.IsChangeColorOnSlotAndShiftChange)
					{
						continue;
					}
					IL_F6:
					return Task.FromResult<bool>(true);
				}
			}
			return Task.FromResult<bool>(false);
		}

		private void RefreshServiceStuffAfterServiceProfilesCollectionChanged()
		{
			IEventAggregator eventAggregator = Engine.EventAggregator;
			if (eventAggregator == null)
			{
				return;
			}
			eventAggregator.GetEvent<ServiceProfilesCollectionChanged>().Publish(null);
		}

		private void SetCheckLicenseError(ref EnableRemapResponse enableRemapResponse)
		{
			if (enableRemapResponse == null)
			{
				enableRemapResponse = new EnableRemapResponse();
			}
			EnableRemapResponseDialog enableRemapResponseDialog = new EnableRemapResponseDialog
			{
				Message = DTLocalization.GetString(4580)
			};
			enableRemapResponseDialog.AddButton(new EnableRemapResponseButton
			{
				Text = DTLocalization.GetString(5004),
				ButtonAction = 0
			}, false);
			enableRemapResponse.AddDialog(enableRemapResponseDialog);
			enableRemapResponse.IsSucceded = false;
			enableRemapResponse.DontReCallEnableRemap = true;
		}

		public async Task<ushort> EnableRemap(bool showGUIMessages = false, string ID = null, bool remapNonToggledFromRelations = false, bool remapNonConnectedGamepad = false, bool changeGamepadSlot = true, int slotNumber = -1, bool force = false, bool reenableRemap = false, bool checkLicense = true, EnableRemapBundle enableRemapBundle = null, EnableRemapResponse enableRemapResponse = null)
		{
			Tracer.TraceWrite("GamepadService.EnableRemap: gamepadId: " + ID, false);
			Tracer.TraceWrite("GamepadService.EnableRemap inside async lock", false);
			if (!this._gameProfilesService.IsServiceInited)
			{
				Tracer.TraceWrite("GamepadService.EnableRemap: GameProfilesService.IsServiceInited is FALSE", false);
				await this._gameProfilesService.WaitForServiceInited();
			}
			Tracer.TraceWrite("GamepadService.EnableRemap: remapNonToggledFromRelations: " + remapNonToggledFromRelations.ToString(), false);
			Tracer.TraceWrite("GamepadService.EnableRemap: remapNonConnectedGamepad: " + remapNonConnectedGamepad.ToString(), false);
			if (checkLicense)
			{
				await this._licensingService.CheckLicenseAsync(false);
				if (!this._licensingService.IsValidLicense)
				{
					this.SetCheckLicenseError(ref enableRemapResponse);
					if (!string.IsNullOrEmpty(ID))
					{
						this.GamepadProfileRelations.Remove(ID);
					}
					return 0;
				}
			}
			this.IsAsyncRemapInProgress = true;
			if (!this.IsServiceInited)
			{
				Tracer.TraceWrite("GamepadService.EnableRemap: IsServiceInited is FALSE", false);
				await this.WaitForServiceInited();
			}
			ushort num;
			using (await new AsyncLock(GamepadService._gamepadServiceEnableDisableRemapSemaphore).LockAsync())
			{
				bool remapSuccessfullyEnabled = false;
				ushort profileID = 0;
				bool isMobileClientRequest = enableRemapBundle != null;
				if (string.IsNullOrEmpty(ID))
				{
					Tracer.TraceWrite("GamepadService.EnableRemap: Enable remap is global, iterating through relations collection", false);
					using (IEnumerator<KeyValuePair<string, GamepadProfiles>> enumerator = ((IEnumerable<KeyValuePair<string, GamepadProfiles>>)this.GamepadProfileRelations).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							GamepadService.<>c__DisplayClass244_1 CS$<>8__locals2 = new GamepadService.<>c__DisplayClass244_1();
							CS$<>8__locals2.kvp = enumerator.Current;
							Tracer.TraceWrite("GamepadService.EnableRemap: GamepadId: " + CS$<>8__locals2.kvp.Key, false);
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 1);
							defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemap: IsRemapToggled: ");
							defaultInterpolatedStringHandler.AppendFormatted<bool>(CS$<>8__locals2.kvp.Value.IsRemapToggled);
							Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
							if (!CS$<>8__locals2.kvp.Value.IsRemapToggled && !remapNonToggledFromRelations)
							{
								goto IL_5D8;
							}
							if (CS$<>8__locals2.kvp.Value.SlotProfiles.Any((DictionaryEntry sp) => sp.Value != null) && (remapNonConnectedGamepad || this.GamepadCollection.Any((BaseControllerVM con) => con.ID == CS$<>8__locals2.kvp.Key)))
							{
								try
								{
									BaseControllerVM baseControllerVM = this.GamepadCollection.FirstOrDefault((BaseControllerVM item) => item.ID == CS$<>8__locals2.kvp.Key);
									slotNumber = -1;
									Slot? slot = await this.RefreshAndGetSlot(baseControllerVM);
									if (slot != null)
									{
										slotNumber = slot.Value;
									}
									profileID = await this.EnableRemap(CS$<>8__locals2.kvp.Value, showGUIMessages, changeGamepadSlot, slotNumber, false, reenableRemap, enableRemapBundle, enableRemapResponse, true);
									remapSuccessfullyEnabled = remapSuccessfullyEnabled || profileID > 0;
									defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
									defaultInterpolatedStringHandler.AppendLiteral("profileID: 0x");
									defaultInterpolatedStringHandler.AppendFormatted<ushort>(profileID, "X");
									Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
									goto IL_5E3;
								}
								catch (Exception ex)
								{
									Tracer.TraceException(ex, "EnableRemap");
									goto IL_5E3;
								}
								goto IL_5D8;
							}
							goto IL_5D8;
							IL_5E3:
							CS$<>8__locals2 = null;
							continue;
							IL_5D8:
							Tracer.TraceWrite("GamepadService.EnableRemap: Error 1", false);
							goto IL_5E3;
						}
					}
					IEnumerator<KeyValuePair<string, GamepadProfiles>> enumerator = null;
				}
				else if (this.GamepadProfileRelations.ContainsKey(ID))
				{
					Tracer.TraceWrite("GamepadService.EnableRemap: GamepadId found in GamepadProfilesCollection", false);
					BaseControllerVM gamepad = this.GamepadCollection.FirstOrDefault((BaseControllerVM item) => item.ID == ID);
					if (gamepad != null)
					{
						if (isMobileClientRequest && gamepad.IsNonInitialized8BitDo)
						{
							Tracer.TraceWrite("GamepadService.EnableRemap: IsNonInitialized8BitDo", false);
							if (enableRemapResponse == null)
							{
								enableRemapResponse = new EnableRemapResponse();
							}
							EnableRemapResponseDialog enableRemapResponseDialog = new EnableRemapResponseDialog();
							enableRemapResponseDialog.Message = DTLocalization.GetString(12148);
							enableRemapResponseDialog.AddButton(new EnableRemapResponseButton
							{
								Text = DTLocalization.GetString(5004),
								ButtonAction = 0
							}, enableRemapBundle.IsUI);
							enableRemapResponse.AddDialog(enableRemapResponseDialog);
							enableRemapResponse.DontReCallEnableRemap = true;
							return 0;
						}
						gamepad.IsRemapInProgress = true;
					}
					try
					{
						if (remapNonConnectedGamepad || this.GamepadCollection.Any((BaseControllerVM con) => con.ID == ID))
						{
							profileID = await this.EnableRemap(this.GamepadProfileRelations[ID], showGUIMessages, changeGamepadSlot, slotNumber, force, false, enableRemapBundle, enableRemapResponse, true);
							remapSuccessfullyEnabled = profileID > 0;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
							defaultInterpolatedStringHandler.AppendLiteral("profileID: 0x");
							defaultInterpolatedStringHandler.AppendFormatted<ushort>(profileID, "X");
							Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
						}
						else
						{
							Tracer.TraceWrite("GamepadService.EnableRemap: Error 2", false);
						}
					}
					catch (Exception ex2)
					{
						Tracer.TraceException(ex2, "EnableRemap");
						if (ex2 is UnauthorizedAccessException && showGUIMessages)
						{
							DTMessageBox.Show(string.Format(DTLocalization.GetString(11134), DTLocalization.GetString(4276), ex2.Message), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
						}
						if (isMobileClientRequest)
						{
							if (enableRemapResponse == null)
							{
								enableRemapResponse = new EnableRemapResponse();
							}
							if (enableRemapResponse.Dialogs != null)
							{
								List<EnableRemapResponseDialog> dialogs = enableRemapResponse.Dialogs;
								if (dialogs == null || dialogs.Count != 0)
								{
									goto IL_9C7;
								}
							}
							if (ex2 is UnauthorizedAccessException)
							{
								string text = ex2.Message.Split('\'', StringSplitOptions.None).FirstOrDefault((string x) => x.Contains(".rewasd"));
								if (!enableRemapBundle.UserActions.Exists((EnableRemapButtonAction x) => x == 10) && !string.IsNullOrEmpty(text))
								{
									EnableRemapResponseDialog enableRemapResponseDialog2 = new EnableRemapResponseDialog();
									enableRemapResponseDialog2.Message = string.Format(DTLocalization.GetString(11134), text, ex2.Message);
									enableRemapResponseDialog2.AddButton(new EnableRemapResponseButton
									{
										Text = DTLocalization.GetString(5004),
										ButtonAction = 10
									}, enableRemapBundle.IsUI);
									enableRemapResponse.AddDialog(enableRemapResponseDialog2);
								}
								enableRemapResponse.DontReCallEnableRemap = true;
							}
						}
						IL_9C7:;
					}
					if (gamepad != null)
					{
						gamepad.IsRemapInProgress = false;
					}
					gamepad = null;
				}
				else
				{
					Tracer.TraceWrite("GamepadService.EnableRemap: Error 3. GamepadId NOT found in GamepadProfilesCollection", false);
				}
				this.BinDataSerialize.SaveGamepadProfileRelations();
				if (remapSuccessfullyEnabled)
				{
					this.GamepadProfileRelations.IsGlobalRemapToggled = true;
				}
				this.IsAsyncRemapInProgress = false;
				Tracer.TraceWrite("Exiting EnableRemap", false);
				num = profileID;
			}
			return num;
		}

		public async Task DisableRemap(string ID = null, bool changeIsRemapToggled = true)
		{
			if (!string.IsNullOrEmpty(ID))
			{
				Tracer.TraceWrite("GamepadService.DisableRemap for controllerID: " + ID, false);
			}
			else
			{
				Tracer.TraceWrite("GamepadService.DisableRemap: controllerID IsNullOrEmpty", false);
			}
			List<Tuple<ulong, uint>> hiddenAppliedControllers = new List<Tuple<ulong, uint>>();
			AsyncLock.Releaser releaser2 = await new AsyncLock(GamepadService._gamepadServiceEnableDisableRemapSemaphore).LockAsync();
			using (releaser2)
			{
				Tracer.TraceWrite("GamepadService.DisableRemap inside async lock", false);
				this.IsAsyncRemapInProgress = true;
				if (this.ServiceProfilesCollection.Count > 0)
				{
					if (string.IsNullOrEmpty(ID))
					{
						await this._xbServiceCommunicator.DeleteAllProfiles();
						if (changeIsRemapToggled)
						{
							foreach (KeyValuePair<string, GamepadProfiles> keyValuePair in ((IEnumerable<KeyValuePair<string, GamepadProfiles>>)this.GamepadProfileRelations))
							{
								keyValuePair.Value.IsRemapToggled = false;
							}
						}
						this.GamepadProfileRelations.IsGlobalRemapToggled = false;
						hiddenAppliedControllers.AddRange(from controllerInfo in await this._xbServiceCommunicator.GetControllersList(false, false, false)
							where UtilsCommon.IsHiddenAppliedConfigControllerPhysicalType(controllerInfo.Type)
							select new Tuple<ulong, uint>(controllerInfo.Id, controllerInfo.Type));
					}
					else
					{
						if (changeIsRemapToggled && this.GamepadProfileRelations.ContainsKey(ID))
						{
							this.GamepadProfileRelations[ID].IsRemapToggled = false;
						}
						REWASD_CONTROLLER_PROFILE_EX profileEx = this.GetProfileEx(ID);
						if (profileEx != null)
						{
							REWASD_CONTROLLER_PROFILE_EX.AmiiboWrapper[] amiibo = profileEx.Amiibo;
							bool flag;
							if (amiibo == null)
							{
								flag = false;
							}
							else
							{
								flag = amiibo.Any((REWASD_CONTROLLER_PROFILE_EX.AmiiboWrapper x) => x.AmiiboLoaded);
							}
							if (flag)
							{
								foreach (REWASD_CONTROLLER_PROFILE_EX.AmiiboWrapper amiiboWrapper in profileEx.Amiibo)
								{
									if (amiiboWrapper.AmiiboLoaded)
									{
										amiiboWrapper.UnLoad();
									}
								}
							}
							ushort[] allProfileIds = profileEx.GetAllProfileIds();
							await this._xbServiceCommunicator.DeleteProfiles(allProfileIds);
							if (profileEx.Profiles.Length != 0)
							{
								for (int j = 0; j < profileEx.Profiles[0].Type.Length; j++)
								{
									if (UtilsCommon.IsHiddenAppliedConfigControllerPhysicalType(profileEx.Profiles[0].Type[j]))
									{
										hiddenAppliedControllers.Add(new Tuple<ulong, uint>(profileEx.Profiles[0].Id[j], profileEx.Profiles[0].Type[j]));
									}
								}
							}
						}
						profileEx = null;
					}
					this.BinDataSerialize.SaveGamepadProfileRelations();
				}
				BaseControllerVM baseControllerVM = this.GamepadCollection.FirstOrDefault((BaseControllerVM item) => item.ID == ID);
				if (baseControllerVM != null)
				{
					ControllerChangedHandler onControllerChanged = this.OnControllerChanged;
					if (onControllerChanged != null)
					{
						onControllerChanged(baseControllerVM);
					}
				}
				this.IsAsyncRemapInProgress = false;
			}
			AsyncLock.Releaser releaser = default(AsyncLock.Releaser);
			if (hiddenAppliedControllers.Count > 0)
			{
				foreach (Tuple<ulong, uint> tuple in hiddenAppliedControllers)
				{
					await this.CompileHiddenAppliedConfig(tuple.Item1, tuple.Item2, true);
				}
				List<Tuple<ulong, uint>>.Enumerator enumerator2 = default(List<Tuple<ulong, uint>>.Enumerator);
			}
		}

		public async Task RefreshHiddenAppliedConfig()
		{
			GamepadService.<>c__DisplayClass246_0 CS$<>8__locals1 = new GamepadService.<>c__DisplayClass246_0();
			this.CachedHiddenAppliedProfilesCollection.Clear();
			await this._xbServiceCommunicator.DeleteAllHiddenAppliedConfigs();
			if (this._licensingService.IsMobileControllerFeatureUnlocked)
			{
				List<REWASD_CONTROLLER_INFO> allControllersList = await this._xbServiceCommunicator.GetControllersList(false, false, false);
				CS$<>8__locals1.allProfilesList = await this._xbServiceCommunicator.GetProfilesList();
				IEnumerable<REWASD_CONTROLLER_INFO> enumerable = allControllersList.Where((REWASD_CONTROLLER_INFO controller) => UtilsCommon.IsHiddenAppliedConfigControllerPhysicalType(controller.Type));
				Func<REWASD_CONTROLLER_INFO, bool> func;
				if ((func = CS$<>8__locals1.<>9__1) == null)
				{
					GamepadService.<>c__DisplayClass246_0 CS$<>8__locals2 = CS$<>8__locals1;
					Func<REWASD_CONTROLLER_INFO, bool> func2 = (REWASD_CONTROLLER_INFO controller) => !CS$<>8__locals1.allProfilesList.Exists(delegate(REWASD_CONTROLLER_PROFILE_EX x)
					{
						REWASD_CONTROLLER_PROFILE[] profiles = x.Profiles;
						return profiles != null && profiles.Length != 0 && REWASD_CONTROLLER_PROFILE_Extensions.IsControllerPresent(x.Profiles[0], controller.Id, (ulong)controller.Type);
					});
					CS$<>8__locals2.<>9__1 = func2;
					func = func2;
				}
				foreach (REWASD_CONTROLLER_INFO rewasd_CONTROLLER_INFO in enumerable.Where(func))
				{
					await this.CompileHiddenAppliedConfig(rewasd_CONTROLLER_INFO.Id, rewasd_CONTROLLER_INFO.Type, true);
				}
				IEnumerator<REWASD_CONTROLLER_INFO> enumerator = null;
			}
		}

		private async void AddGamepadToPhysicalCollection(BaseControllerVM controller)
		{
			BlackListGamepad blackListGamepad = this.BlacklistGamepads.FirstOrDefault((BlackListGamepad item) => item.ID.Contains(controller.ID));
			BaseControllerVM findedController = this.AllPhysicalControllers.FirstOrDefault((BaseControllerVM item) => item.ID == controller.ID);
			if (blackListGamepad != null)
			{
				if (findedController != null)
				{
					this.AllPhysicalControllers.Remove(findedController);
				}
				blackListGamepad.ControllerFamily = controller.ControllerFamily;
				blackListGamepad.ControllerTypeEnums = controller.ControllerTypeEnums;
			}
			else
			{
				if (findedController == null && !controller.IsUnknownControllerType)
				{
					this.AllPhysicalControllers.Add(controller);
					this.AddGamepadToGamepadsSettingsCollection(controller);
					await this.RefreshGamepadStatesAndReactOnBattery(controller, false);
				}
				if (findedController != null)
				{
					findedController.IsInsideCompositeDevice = controller.IsInsideCompositeDevice;
				}
			}
		}

		private void AddGamepadToGamepadsSettingsCollection(BaseControllerVM device)
		{
			ControllerVM controller = device as ControllerVM;
			if (controller != null && controller.HasGamepadControllers && controller.IsControllerBatteryBlockVisible && this.GamepadsSettings.FirstOrDefault((GamepadSettings item) => item.ID == controller.ID) == null)
			{
				this.GamepadsSettings.Add(new GamepadSettings(controller.ID, controller.ControllerDisplayName, controller.FirstControllerType));
			}
		}

		private void AddGamepadToGamepadsAuthCollection(BaseControllerVM device)
		{
			ControllerVM controllerVM = device as ControllerVM;
			if (controllerVM != null && controllerVM.HasGamepadControllers && controllerVM.IsControllerAuthAllowed)
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

		private bool IsBlacklistedDevice(string deviceID)
		{
			return this.BlacklistGamepads.FirstOrDefault((BlackListGamepad item) => item.ID.Contains(deviceID)) != null;
		}

		private async void AddGamepadToCollection(BaseControllerVM controller)
		{
			if (this.BlacklistGamepads.FirstOrDefault((BlackListGamepad item) => item.ID == controller.ID) == null)
			{
				try
				{
					this.GamepadCollection.Add(controller);
				}
				catch (Exception)
				{
				}
				await this.RefreshLedForController(controller);
				this.BinDataSerialize.SaveGamepadsSettings();
			}
		}

		public async Task DeleteAllExclusiveCaptureProfiles()
		{
			if (this.IsExclusiveCaptureControllersPresent && this.IsExclusiveCaptureProfilePresent)
			{
				this.IsExclusiveCaptureProfilePresent = false;
				await this._xbServiceCommunicator.DeleteExclusiveCaptureProfiles();
			}
		}

		public async Task ProcessExclusiveCaptureProfile(string gamepadID, bool processDelete = true, bool processAdd = true)
		{
			BaseControllerVM _currentGamepad = this.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.ID == gamepadID);
			if (this.IsExclusiveCaptureControllersPresent && _currentGamepad != null)
			{
				if (!_currentGamepad.HasExclusiveCaptureControllers)
				{
					await this.DeleteAllExclusiveCaptureProfiles();
				}
				else
				{
					List<REWASD_CONTROLLER_PROFILE_EX> captureProfiles = await this._xbServiceCommunicator.GetExclusiveCaptureProfilesList();
					bool flag = false;
					if (processDelete)
					{
						using (IEnumerator<Tuple<ulong, SimpleDeviceInfo>> enumerator = this.SimpleDeviceInfoList.Where((Tuple<ulong, SimpleDeviceInfo> x) => UtilsCommon.IsExclusiveCaptureControllerPhysicalType(x.Item2.Type)).GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								GamepadService.<>c__DisplayClass254_1 CS$<>8__locals2 = new GamepadService.<>c__DisplayClass254_1();
								CS$<>8__locals2.exclusiveSimpleDeviceInfo = enumerator.Current;
								if (_currentGamepad.Ids.All((ulong x) => x != CS$<>8__locals2.exclusiveSimpleDeviceInfo.Item1))
								{
									IEnumerable<REWASD_CONTROLLER_PROFILE_EX> enumerable = captureProfiles;
									Func<REWASD_CONTROLLER_PROFILE_EX, bool> func;
									if ((func = CS$<>8__locals2.<>9__3) == null)
									{
										GamepadService.<>c__DisplayClass254_1 CS$<>8__locals3 = CS$<>8__locals2;
										Func<REWASD_CONTROLLER_PROFILE_EX, bool> func2 = (REWASD_CONTROLLER_PROFILE_EX x) => x.Profiles[0].Id[0] == CS$<>8__locals2.exclusiveSimpleDeviceInfo.Item1 && UtilsCommon.IsExclusiveCaptureControllerPhysicalType(x.Profiles[0].Type[0]);
										CS$<>8__locals3.<>9__3 = func2;
										func = func2;
									}
									foreach (REWASD_CONTROLLER_PROFILE_EX rewasd_CONTROLLER_PROFILE_EX in enumerable.Where(func))
									{
										string text = "ProcessExclusiveCaptureProfile: delete CaptureProfile for Id: ";
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 2);
										defaultInterpolatedStringHandler.AppendLiteral("0x");
										defaultInterpolatedStringHandler.AppendFormatted<ulong>(CS$<>8__locals2.exclusiveSimpleDeviceInfo.Item1, "X");
										defaultInterpolatedStringHandler.AppendLiteral(" / ");
										defaultInterpolatedStringHandler.AppendFormatted<ulong>(CS$<>8__locals2.exclusiveSimpleDeviceInfo.Item1);
										Tracer.TraceWrite(text + defaultInterpolatedStringHandler.ToStringAndClear(), false);
										await this._xbServiceCommunicator.DeleteProfile(rewasd_CONTROLLER_PROFILE_EX.ServiceProfileIds[0]);
										flag = true;
									}
									IEnumerator<REWASD_CONTROLLER_PROFILE_EX> enumerator2 = null;
								}
								CS$<>8__locals2 = null;
							}
						}
						IEnumerator<Tuple<ulong, SimpleDeviceInfo>> enumerator = null;
					}
					if (flag)
					{
						captureProfiles = await this._xbServiceCommunicator.GetExclusiveCaptureProfilesList();
					}
					this.IsExclusiveCaptureProfilePresent = captureProfiles.Count > 0;
					if (processAdd && _currentGamepad.HasExclusiveCaptureControllers && !this.ServiceProfilesCollection.ContainsProfileForID(_currentGamepad.ID))
					{
						Dictionary<ulong, uint> dictionary = new Dictionary<ulong, uint>();
						for (int i = 0; i < _currentGamepad.Types.Length; i++)
						{
							if (UtilsCommon.IsExclusiveCaptureControllerPhysicalType(_currentGamepad.Types[i]) && !dictionary.ContainsKey(_currentGamepad.Ids[i]))
							{
								dictionary.Add(_currentGamepad.Ids[i], _currentGamepad.Types[i]);
							}
						}
						using (Dictionary<ulong, uint>.Enumerator enumerator3 = dictionary.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								KeyValuePair<ulong, uint> exclusiveItem = enumerator3.Current;
								bool flag2 = false;
								if (captureProfiles.Count > 0)
								{
									Func<ulong, bool> <>9__6;
									Func<REWASD_CONTROLLER_PROFILE, bool> <>9__5;
									flag2 = captureProfiles.Exists(delegate(REWASD_CONTROLLER_PROFILE_EX x)
									{
										IEnumerable<REWASD_CONTROLLER_PROFILE> profiles = x.Profiles;
										Func<REWASD_CONTROLLER_PROFILE, bool> func3;
										if ((func3 = <>9__5) == null)
										{
											func3 = (<>9__5 = delegate(REWASD_CONTROLLER_PROFILE profile)
											{
												IEnumerable<ulong> id2 = profile.Id;
												Func<ulong, bool> func4;
												if ((func4 = <>9__6) == null)
												{
													func4 = (<>9__6 = (ulong id) => id == exclusiveItem.Key);
												}
												return id2.Any(func4);
											});
										}
										return profiles.Any(func3);
									});
								}
								if (!flag2)
								{
									this.CompileExclusiveCaptureProfile(exclusiveItem.Key, exclusiveItem.Value, true);
								}
							}
						}
					}
					captureProfiles = null;
				}
			}
		}

		private async void CheckExclusiveCaptureProfilesOnGamepadChanged()
		{
			bool refreshCaptureProfiles = false;
			List<REWASD_CONTROLLER_PROFILE_EX> list = await this._xbServiceCommunicator.GetExclusiveCaptureProfilesList();
			List<REWASD_CONTROLLER_PROFILE_EX> captureProfiles = list;
			if (captureProfiles.Count > 0)
			{
				List<REWASD_CONTROLLER_INFO> allControllersList = await this._xbServiceCommunicator.GetControllersList(false, false, false);
				foreach (REWASD_CONTROLLER_PROFILE_EX rewasd_CONTROLLER_PROFILE_EX in captureProfiles.Where((REWASD_CONTROLLER_PROFILE_EX profile) => UtilsCommon.IsExclusiveCaptureControllerPhysicalType(profile.Profiles[0].Type[0])))
				{
					ulong id = rewasd_CONTROLLER_PROFILE_EX.Profiles[0].Id[0];
					if (!allControllersList.Any((REWASD_CONTROLLER_INFO x) => x.Id == id))
					{
						string text = "CheckExclusiveCaptureProfilesOnGamepadChanged: delete CaptureProfile for Id: ";
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 2);
						defaultInterpolatedStringHandler.AppendLiteral("0x");
						defaultInterpolatedStringHandler.AppendFormatted<ulong>(id, "X");
						defaultInterpolatedStringHandler.AppendLiteral(" / ");
						defaultInterpolatedStringHandler.AppendFormatted<ulong>(id);
						Tracer.TraceWrite(text + defaultInterpolatedStringHandler.ToStringAndClear(), false);
						await this._xbServiceCommunicator.DeleteProfile(rewasd_CONTROLLER_PROFILE_EX.ServiceProfileIds[0]);
						refreshCaptureProfiles = true;
					}
				}
				IEnumerator<REWASD_CONTROLLER_PROFILE_EX> enumerator = null;
				allControllersList = null;
			}
			if (refreshCaptureProfiles)
			{
				list = await this._xbServiceCommunicator.GetExclusiveCaptureProfilesList();
				captureProfiles = list;
			}
			this.IsExclusiveCaptureProfilePresent = captureProfiles.Count > 0;
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

		private void StartBatteryTimer()
		{
			this._refreshStatesTimer = new DispatcherTimer();
			this._refreshStatesTimer.Tick += async delegate([Nullable(2)] object o, EventArgs e)
			{
				await this.RefreshStatesAndReactOnBatteryOfGamepadCollection();
			};
			this._refreshStatesTimer.Interval = new TimeSpan(0, 5, 0);
			this._refreshStatesTimer.Start();
		}

		public async Task ReinitializeDevice(string controllerId)
		{
			PeripheralDevice curpd = this.PeripheralDevices.FirstOrDefault((PeripheralDevice pd) => pd.ID == controllerId);
			InitializedDevice curid = this.InitializedDevices.FirstOrDefault((InitializedDevice id) => id.ID == controllerId);
			await this.DisableRemap(controllerId, true);
			if (curpd != null)
			{
				this.PeripheralDevices.Remove(curpd);
				this.BinDataSerialize.SavePeripheralDevicesCollection();
				PeripheralVM peripheralVM = this.GamepadCollection.FirstOrDefault((BaseControllerVM item) => item.ID == controllerId) as PeripheralVM;
				if (peripheralVM != null)
				{
					peripheralVM.PeripheralPhysicalType = 0;
					ControllerChangedHandler onControllerChanged = this.OnControllerChanged;
					if (onControllerChanged != null)
					{
						onControllerChanged(peripheralVM);
					}
				}
			}
			if (curid != null)
			{
				this.InitializedDevices.Remove(curid);
				this.BinDataSerialize.SaveInitializedDevicesCollection("");
				BaseControllerVM baseControllerVM = this.GamepadCollection.FirstOrDefault((BaseControllerVM item) => item.ID == controllerId);
				if (baseControllerVM != null)
				{
					baseControllerVM.SetIsInitialized(false);
					ControllerChangedHandler onControllerChanged2 = this.OnControllerChanged;
					if (onControllerChanged2 != null)
					{
						onControllerChanged2(baseControllerVM);
					}
				}
			}
			this.BinDataSerialize.SaveGamepadCollection();
		}

		public void InitializePeripheralDevice(PeripheralVM peripheral, PeripheralPhysicalType physicalType)
		{
			PeripheralDevice peripheralDevice = new PeripheralDevice(peripheral.ID, physicalType);
			this.PeripheralDevices.Add(peripheralDevice);
			this.BinDataSerialize.SavePeripheralDevicesCollection();
			peripheral.PeripheralPhysicalType = physicalType;
			this.CheckControllerSettings(peripheral);
			ControllerChangedHandler onControllerChanged = this.OnControllerChanged;
			if (onControllerChanged != null)
			{
				onControllerChanged(peripheral);
			}
			this.BinDataSerialize.SaveGamepadCollection();
		}

		public void InitializeDevice(BaseControllerVM controller, string deviceType)
		{
			if (this.InitializedDevices.FirstOrDefault((InitializedDevice item) => item.ID == controller.ID) == null)
			{
				InitializedDevice initializedDevice = new InitializedDevice(controller.ID, deviceType);
				this.InitializedDevices.Add(initializedDevice);
				this.BinDataSerialize.SaveInitializedDevicesCollection("");
			}
			controller.InitializedDeviceType = deviceType;
			controller.SetIsInitialized(true);
			ControllerChangedHandler onControllerChanged = this.OnControllerChanged;
			if (onControllerChanged == null)
			{
				return;
			}
			onControllerChanged(controller);
		}

		public void RemoveRelationsForConfigFile(string configPath)
		{
			List<Tuple<string, Slot>> list = new List<Tuple<string, Slot>>();
			foreach (KeyValuePair<string, GamepadProfiles> keyValuePair in ((IEnumerable<KeyValuePair<string, GamepadProfiles>>)this.GamepadProfileRelations))
			{
				foreach (KeyValuePair<Slot, GamepadProfile> keyValuePair2 in ((IEnumerable<KeyValuePair<Slot, GamepadProfile>>)keyValuePair.Value.SlotProfiles))
				{
					GamepadProfile value = keyValuePair2.Value;
					if (((value != null) ? value.ConfigPath : null) == configPath)
					{
						list.Add(new Tuple<string, Slot>(keyValuePair.Key, keyValuePair2.Key));
					}
				}
			}
			list.ForEach(async delegate(Tuple<string, Slot> item)
			{
				await this.RestoreDefaults(item.Item1, new List<Slot> { item.Item2 });
			});
			bool flag = this.GamepadProfileRelations.RemoveConfig(configPath);
			bool flag2 = this.AutoGamesDetectionGamepadProfileRelations.RemoveConfig(configPath);
			if (flag)
			{
				this.BinDataSerialize.SaveGamepadProfileRelations();
			}
			if (flag2)
			{
				this.BinDataSerialize.SaveAutoGamesDetectionGamepadProfileRelations();
			}
		}

		public void SendGamepadChanged(BaseControllerVM controller)
		{
			ControllerChangedHandler onControllerChanged = this.OnControllerChanged;
			if (onControllerChanged == null)
			{
				return;
			}
			onControllerChanged(controller);
		}

		public void UpdateDeviceFriendlyName(string ID)
		{
			BaseControllerVM baseControllerVM = this.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.ID == ID);
			if (baseControllerVM != null)
			{
				ControllerChangedHandler onControllerChanged = this.OnControllerChanged;
				if (onControllerChanged == null)
				{
					return;
				}
				onControllerChanged(baseControllerVM);
			}
		}

		public async Task ControllerChangeMasterAddress(ulong gamepadID, uint gamepadType, ulong bluetoothAddress)
		{
			await Engine.XBServiceCommunicator.ControllerChangeMasterAddress(gamepadID, gamepadType, bluetoothAddress);
			await Engine.GamepadService.RefreshGamepadCollection(0UL);
			BaseControllerVM baseControllerVM = this.GamepadCollection.FirstOrDefault(delegate(BaseControllerVM g)
			{
				ControllerVM controllerVM = g as ControllerVM;
				return controllerVM != null && controllerVM.ControllerId == gamepadID;
			});
			if (baseControllerVM != null)
			{
				ControllerChangedHandler onControllerChanged = this.OnControllerChanged;
				if (onControllerChanged != null)
				{
					onControllerChanged(baseControllerVM);
				}
			}
		}

		public async void SetEngineBatteryState(ControllerVM controller, byte batteryLevel, BatteryChargingState chatrgingState)
		{
			if (controller.IsEngineController)
			{
				controller.SetEngineBatteryState(batteryLevel, chatrgingState);
				await this.RefreshGamepadBattery(controller.ControllerId);
				BatteryLevelChangedHandler onBatteryLevelChanged = this.OnBatteryLevelChanged;
				if (onBatteryLevelChanged != null)
				{
					onBatteryLevelChanged(controller);
				}
			}
		}

		private async Task<ushort> EnableRemap(GamepadProfiles gamepadProfiles, bool showGuiMessages, bool changeGamepadSlot, int slotNumber = -1, bool force = false, bool reenableRemap = false, EnableRemapBundle enableRemapBundle = null, EnableRemapResponse enableRemapResponse = null, bool isCacheAllowed = true)
		{
			GamepadService.<>c__DisplayClass267_0 CS$<>8__locals1 = new GamepadService.<>c__DisplayClass267_0();
			CS$<>8__locals1.gamepadProfiles = gamepadProfiles;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(94, 2);
			defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: Inside async EnableRemap. SlotNumber = ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(slotNumber);
			defaultInterpolatedStringHandler.AppendLiteral(". ChangeGamepadSlot = ");
			defaultInterpolatedStringHandler.AppendFormatted<bool>(changeGamepadSlot);
			Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
			CS$<>8__locals1.gamepadProfiles.UpdateControllerInfosChainIfRequired(this, force);
			ushort profileId = 0;
			bool anyProfileFound = false;
			bool currentSlotResetedDueToLicenseFeature = false;
			bool needMacroFeature = false;
			bool needAdvancedMappingFeature = false;
			bool needRapidFireFeature = false;
			bool needMobileControllerFeature = false;
			string needFeatureAdditionalInfo = "";
			bool isControllerCheatsExist = false;
			bool configFileIsNotExistWarning = false;
			bool adaptiveLeftTriggerPresent = false;
			bool adaptiveRightTriggerPresent = false;
			bool isSlotFeatureUnlocked = this._licensingService.IsSlotFeatureUnlocked;
			ControllerProfileInfoCollection[] controllerInfos = CS$<>8__locals1.gamepadProfiles.ControllerProfileInfoCollections;
			ServiceResponseWrapper<REWASD_GET_VERSION_RESPONSE> serviceResponseWrapper = await this._xbServiceCommunicator.GetVersion();
			bool virtualUsbHubPresent = true;
			if (serviceResponseWrapper.IsResponseValid && serviceResponseWrapper.ServiceResponse.Header.Status == 0U)
			{
				virtualUsbHubPresent = Convert.ToBoolean((uint)(serviceResponseWrapper.ServiceResponse.DriverFlags & 256));
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(70, 1);
				defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: Driver flags: VirtualUsbHubPresent = ");
				defaultInterpolatedStringHandler.AppendFormatted<bool>(virtualUsbHubPresent);
				Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
			}
			GamepadService.UpdateControllerProfileInfoCollection(controllerInfos, this.SimpleDeviceInfoList);
			List<REWASD_CONTROLLER_INFO> fullControllersList = null;
			if (IControllerProfileInfoCollectionContainerExtensions.GetControllerTypes(controllerInfos, false, null).Any((ControllerTypeEnum x) => ControllerTypeExtensions.IsNVidiaShield2015(x) || ControllerTypeExtensions.IsGamepadWithBuiltInAnyKeypad(x) || ControllerTypeExtensions.IsGamepadWithBuiltInMouse(x)))
			{
				fullControllersList = await this._xbServiceCommunicator.GetControllersList(true, false, false);
			}
			CS$<>8__locals1.controllerIds = IControllerProfileInfoCollectionContainerExtensions.GetControllerIds(controllerInfos, false, fullControllersList);
			ControllerTypeEnum[] controllerTypes = IControllerProfileInfoCollectionContainerExtensions.GetControllerTypes(controllerInfos, false, fullControllersList);
			uint[] controllerPhysicalTypes = ControllerTypeExtensions.ConvertEnumsToPhysicalTypes(controllerTypes);
			bool isAllControllersOffline = false;
			foreach (ulong num in CS$<>8__locals1.controllerIds)
			{
				if (num != 0UL)
				{
					BaseControllerVM baseControllerVM = this.GamepadCollection.FindControllerByControllerId(num);
					if (baseControllerVM != null)
					{
						if (baseControllerVM.IsOnline)
						{
							isAllControllersOffline = false;
							break;
						}
						isAllControllersOffline = true;
					}
				}
			}
			if (isAllControllersOffline)
			{
				Tracer.TraceWrite("GamepadService.EnableRemapAsync: IsAllControllersOffline = TRUE", false);
			}
			bool externalDeviceOverwritePrevConfig = Convert.ToBoolean(RegistryHelper.GetValue("Config", "ExternalDeviceOverwritePrevConfig", 0, false));
			LicenseData licenseInfo = new LicenseData(this._licensingService.IsMacroFeatureUnlocked, this._licensingService.IsTurboFeatureUnlocked, this._licensingService.IsToggleFeatureUnlocked, this._licensingService.IsAdvancedMappingFeatureUnlocked, isSlotFeatureUnlocked, this._licensingService.IsMobileControllerFeatureUnlocked);
			bool isMobileClientRequest = enableRemapBundle != null;
			if (isMobileClientRequest)
			{
				if (enableRemapResponse == null)
				{
					enableRemapResponse = new EnableRemapResponse();
				}
				if (enableRemapResponse.Dialogs != null && enableRemapResponse.Dialogs.Count > 0)
				{
					enableRemapResponse.Dialogs = null;
				}
				enableRemapResponse.PostAction = 0;
				if (enableRemapBundle != null && enableRemapBundle.IsUI)
				{
					isCacheAllowed = false;
				}
			}
			bool flag = true;
			REWASD_CONTROLLER_PROFILE_CACHE? rewasd_CONTROLLER_PROFILE_CACHE = null;
			if (isCacheAllowed)
			{
				if (this.CachedLicenseData.IsMacroFeatureUnlocked != this._licensingService.IsMacroFeatureUnlocked || this.CachedLicenseData.IsTurboFeatureUnlocked != this._licensingService.IsTurboFeatureUnlocked || this.CachedLicenseData.IsToggleFeatureUnlocked != this._licensingService.IsToggleFeatureUnlocked || this.CachedLicenseData.IsAdvancedMappingFeatureUnlocked != this._licensingService.IsAdvancedMappingFeatureUnlocked || this.CachedLicenseData.IsSlotFeatureUnlocked != isSlotFeatureUnlocked || this.CachedLicenseData.IsMobileControllerFeatureUnlocked != this._licensingService.IsMobileControllerFeatureUnlocked)
				{
					this.CachedProfilesCollection.Clear();
					this.CachedLicenseData.IsMacroFeatureUnlocked = this._licensingService.IsMacroFeatureUnlocked;
					this.CachedLicenseData.IsTurboFeatureUnlocked = this._licensingService.IsTurboFeatureUnlocked;
					this.CachedLicenseData.IsToggleFeatureUnlocked = this._licensingService.IsToggleFeatureUnlocked;
					this.CachedLicenseData.IsAdvancedMappingFeatureUnlocked = this._licensingService.IsAdvancedMappingFeatureUnlocked;
					this.CachedLicenseData.IsSlotFeatureUnlocked = isSlotFeatureUnlocked;
					this.CachedLicenseData.IsMobileControllerFeatureUnlocked = this._licensingService.IsMobileControllerFeatureUnlocked;
				}
				if (this.CachedProfilesCollection.Count > 0)
				{
					rewasd_CONTROLLER_PROFILE_CACHE = this.TryGetCachedProfile(CS$<>8__locals1.gamepadProfiles.SlotProfiles, CS$<>8__locals1.controllerIds, controllerTypes, isCacheAllowed);
					if (rewasd_CONTROLLER_PROFILE_CACHE != null)
					{
						flag = false;
					}
					else
					{
						Tracer.TraceWrite("GamepadService.EnableRemapAsync: TryGetCachedProfile return null", false);
					}
				}
			}
			if (isMobileClientRequest)
			{
				flag = true;
			}
			REWASD_CONTROLLER_PROFILE_EX controllerProfileEx = ((!flag) ? rewasd_CONTROLLER_PROFILE_CACHE.Value.ControllerProfileEx : REWASD_CONTROLLER_PROFILE_EX.CreateBlankInstance(0U));
			Slot? firstSlotWithConfig = ((!flag) ? rewasd_CONTROLLER_PROFILE_CACHE.Value.FirstSlotWithConfig : null);
			List<uint> virtualGamepadList = ((!flag) ? rewasd_CONTROLLER_PROFILE_CACHE.Value.VirtualGamepadList : new List<uint>());
			if (controllerProfileEx.SlotsWrapperId == 0U)
			{
				Tracer.TraceWrite("GamepadService.EnableRemapAsync: SlotsWrapperId is 0!", false);
			}
			ulong slotWrapperIdBitMask = ((ulong)controllerProfileEx.SlotsWrapperId << 32) & 18446744069414584320UL;
			bool localVirtualGamepadPresent = false;
			bool skipSteamExtendedWarning = false;
			bool skipSteamExtendedBsodWarning = false;
			List<Slot> restoreSlotsToDefaults = new List<Slot>();
			if (flag)
			{
				List<CacheSlotInfo> slotInfoList = new List<CacheSlotInfo>();
				controllerProfileEx.ControllerPhysicalTypes = controllerPhysicalTypes;
				Tracer.TraceWrite(Engine.UserSettingsService.TurnOffControllerOption ? "GamepadService.EnableRemapAsync: TurnOffControllerOption TRUE" : "GamepadService.EnableRemapAsync: TurnOffControllerOption FALSE", false);
				if (Engine.UserSettingsService.TurnOffControllerOption)
				{
					string text = "GamepadService.EnableRemapAsync: TurnOffControllerTimeout ";
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
					defaultInterpolatedStringHandler.AppendFormatted<int>(Engine.UserSettingsService.TurnOffControllerTimeout);
					Tracer.TraceWrite(text + defaultInterpolatedStringHandler.ToStringAndClear(), false);
				}
				if (controllerTypes.Any((ControllerTypeEnum x) => ControllerTypeExtensions.IsAnySonyDualSense(x)))
				{
					foreach (object obj in Enum.GetValues(typeof(Slot)))
					{
						Slot slot2 = (Slot)obj;
						if (CS$<>8__locals1.gamepadProfiles.SlotProfiles.ContainsKey(slot2))
						{
							GamepadProfile gamepadProfile = CS$<>8__locals1.gamepadProfiles.SlotProfiles[slot2];
							if (File.Exists((gamepadProfile != null) ? gamepadProfile.ConfigPath : null))
							{
								GamepadProfile gamepadProfile2 = CS$<>8__locals1.gamepadProfiles.SlotProfiles[slot2];
								bool flag2;
								if (gamepadProfile2 == null)
								{
									flag2 = null != null;
								}
								else
								{
									Config config = gamepadProfile2.Config;
									flag2 = ((config != null) ? config.ConfigData : null) != null;
								}
								if (flag2)
								{
									if (CS$<>8__locals1.gamepadProfiles.SlotProfiles[slot2].Config.ConfigData != null)
									{
										if (CS$<>8__locals1.gamepadProfiles.SlotProfiles[slot2].Config.ConfigData.Any((SubConfigData x) => x.MainXBBindingCollection != null && x.MainXBBindingCollection.ShiftXBBindingCollections == null))
										{
											goto IL_89B;
										}
									}
									adaptiveLeftTriggerPresent |= CS$<>8__locals1.gamepadProfiles.SlotProfiles[slot2].Config.ConfigData.IsAdaptiveLeftTriggerSettingsPresent();
									adaptiveRightTriggerPresent |= CS$<>8__locals1.gamepadProfiles.SlotProfiles[slot2].Config.ConfigData.IsAdaptiveRightTriggerSettingsPresent();
									continue;
								}
								IL_89B:
								ConfigData configData = XBUtils.CreateConfigData(false);
								GamepadProfile gamepadProfile3 = CS$<>8__locals1.gamepadProfiles.SlotProfiles[slot2];
								bool flag3;
								if (MacroCompiler.ParseConfigFile((gamepadProfile3 != null) ? gamepadProfile3.ConfigPath : null, configData, ref flag3, null, false))
								{
									adaptiveLeftTriggerPresent |= configData.IsAdaptiveLeftTriggerSettingsPresent();
									adaptiveRightTriggerPresent |= configData.IsAdaptiveRightTriggerSettingsPresent();
								}
							}
						}
					}
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(60, 1);
					defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: AdaptiveLeftTriggerPresent ");
					defaultInterpolatedStringHandler.AppendFormatted<bool>(adaptiveLeftTriggerPresent);
					Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(61, 1);
					defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: AdaptiveRightTriggerPresent ");
					defaultInterpolatedStringHandler.AppendFormatted<bool>(adaptiveRightTriggerPresent);
					Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
				}
				foreach (object obj2 in Enum.GetValues(typeof(Slot)))
				{
					Slot slot = (Slot)obj2;
					REWASD_CONTROLLER_PROFILE controllerProfile = REWASD_CONTROLLER_PROFILE.CreateBlankInstance();
					controllerProfile.GuiContext = slotWrapperIdBitMask;
					controllerProfile.GuiContext |= (slot << 8) & 65280;
					controllerProfile.BroadcastFlags |= 4;
					if (slot != null)
					{
						controllerProfile.FeatureFlags |= 64U;
					}
					Buffer.BlockCopy(MacroCompilerVirtualMouse.CalculateVirtualMouseDelta(), 0, controllerProfile.Macros, (int)MacroCompiler.CalculateMouseDeltaOffset, (int)MacroCompiler.MouseDeltaSize);
					Buffer.BlockCopy(MacroCompiler.VirtualGamepadLeftStickUpdate(), 0, controllerProfile.Macros, (int)MacroCompiler.VirtualGamepadLeftStickUpdateOffset, (int)MacroCompiler.VirtualGamepadStickUpdateSize);
					Buffer.BlockCopy(MacroCompiler.VirtualGamepadRightStickUpdate(), 0, controllerProfile.Macros, (int)MacroCompiler.VirtualGamepadRightStickUpdateOffset, (int)MacroCompiler.VirtualGamepadStickUpdateSize);
					uint num2 = 55U;
					controllerProfile.Macros[0] = 0;
					controllerProfile.Macros[1] = 1;
					controllerProfile.Macros[3] = 2;
					controllerProfile.Macros[5] = 3;
					controllerProfile.Macros[7] = 4;
					controllerProfile.Macros[8] = 0;
					uint num3 = 0U;
					Tracer.TraceWrite("GamepadService.EnableRemapAsync: Before iterating for slots and filling profiles", false);
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(77, 2);
					defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: GamepadProfiles.SlotProfiles.ContainsKey(");
					defaultInterpolatedStringHandler.AppendFormatted(slot.ToString());
					defaultInterpolatedStringHandler.AppendLiteral("): ");
					defaultInterpolatedStringHandler.AppendFormatted<bool>(CS$<>8__locals1.gamepadProfiles.SlotProfiles.ContainsKey(slot));
					Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
					bool flag4 = isSlotFeatureUnlocked || slot == 0;
					if (!isSlotFeatureUnlocked)
					{
						if (slot == 1)
						{
							if (controllerTypes.Any((ControllerTypeEnum x) => x == 3 || x == 12))
							{
								flag4 = true;
							}
						}
						if (slot > 1)
						{
							if (controllerTypes.Any((ControllerTypeEnum x) => x == 12))
							{
								flag4 = true;
							}
						}
					}
					if (!flag4 && CS$<>8__locals1.gamepadProfiles.SlotProfiles.ContainsKey(slot))
					{
						GamepadProfile gamepadProfile4 = CS$<>8__locals1.gamepadProfiles.SlotProfiles[slot];
						if (((gamepadProfile4 != null) ? gamepadProfile4.Config : null) != null)
						{
							restoreSlotsToDefaults.Add(slot);
							defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(73, 1);
							defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: GamepadProfiles.SlotProfiles.Remove ");
							defaultInterpolatedStringHandler.AppendFormatted<Slot>(slot);
							defaultInterpolatedStringHandler.AppendLiteral(" (0)");
							Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
							CS$<>8__locals1.gamepadProfiles.SlotProfiles.Remove(slot);
							if (slot == slotNumber)
							{
								currentSlotResetedDueToLicenseFeature = true;
							}
						}
					}
					if (isMobileClientRequest && CS$<>8__locals1.gamepadProfiles.SlotProfiles.ContainsKey(slot) && flag4)
					{
						GamepadProfile gamepadProfile5 = CS$<>8__locals1.gamepadProfiles.SlotProfiles[slot];
						if (!string.IsNullOrEmpty((gamepadProfile5 != null) ? gamepadProfile5.ConfigPath : null))
						{
							GamepadProfile gamepadProfile6 = CS$<>8__locals1.gamepadProfiles.SlotProfiles[slot];
							if (!File.Exists((gamepadProfile6 != null) ? gamepadProfile6.ConfigPath : null))
							{
								if (!enableRemapBundle.UserActions.Exists((EnableRemapButtonAction x) => x == 9))
								{
									EnableRemapResponseDialog enableRemapResponseDialog = new EnableRemapResponseDialog();
									enableRemapResponseDialog.Message = DTLocalization.GetString(11853);
									enableRemapResponseDialog.AddButton(new EnableRemapResponseButton
									{
										Text = DTLocalization.GetString(5004),
										ButtonAction = 9
									}, enableRemapBundle.IsUI);
									enableRemapResponse.AddDialog(enableRemapResponseDialog);
								}
								enableRemapResponse.DontReCallEnableRemap = true;
								enableRemapResponse.PostAction = 1;
								defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(73, 1);
								defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: GamepadProfiles.SlotProfiles.Remove ");
								defaultInterpolatedStringHandler.AppendFormatted<Slot>(slot);
								defaultInterpolatedStringHandler.AppendLiteral(" (1)");
								Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
								CS$<>8__locals1.gamepadProfiles.SlotProfiles.Remove(slot);
								return 0;
							}
						}
					}
					bool flag5;
					if (CS$<>8__locals1.gamepadProfiles.SlotProfiles.ContainsKey(slot))
					{
						GamepadProfile gamepadProfile7 = CS$<>8__locals1.gamepadProfiles.SlotProfiles[slot];
						flag5 = ((gamepadProfile7 != null) ? gamepadProfile7.Config : null) != null;
					}
					else
					{
						flag5 = false;
					}
					if (flag5 && flag4)
					{
						Tracer.TraceWrite("GamepadService.EnableRemapAsync: GamepadProfiles.SlotProfiles.ConfigPath: " + CS$<>8__locals1.gamepadProfiles.SlotProfiles[slot].ConfigPath, false);
						Config config2 = CS$<>8__locals1.gamepadProfiles.SlotProfiles[slot].Config;
						if (!string.IsNullOrEmpty(config2.ConfigPath) && !File.Exists(config2.ConfigPath))
						{
							if (showGuiMessages && !configFileIsNotExistWarning)
							{
								DTMessageBox.Show(DTLocalization.GetString(11853), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
								configFileIsNotExistWarning = true;
							}
							defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(73, 1);
							defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: GamepadProfiles.SlotProfiles.Remove ");
							defaultInterpolatedStringHandler.AppendFormatted<Slot>(slot);
							defaultInterpolatedStringHandler.AppendLiteral(" (2)");
							Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
							CS$<>8__locals1.gamepadProfiles.SlotProfiles.Remove(slot);
							continue;
						}
						CS$<>8__locals1.gamepadProfiles.SlotProfiles[slot].Config.ReadConfigFromJsonIfNotLoaded(true);
						if (firstSlotWithConfig == null)
						{
							firstSlotWithConfig = new Slot?(slot);
						}
						Tracer.TraceWrite("GamepadService.EnableRemapAsync: Before FillControllerProfileSlotWithConfig", false);
						uint num4;
						List<byte[]> list;
						bool flag7;
						bool flag8;
						ApplyResultStatus errorStatus;
						bool flag6 = GamepadService.FillControllerProfileSlotWithConfig(ref controllerProfile, ref num3, slot, CS$<>8__locals1.gamepadProfiles.SlotProfiles[slot].Config.ConfigPath, out needMacroFeature, out needAdvancedMappingFeature, out needRapidFireFeature, out needMobileControllerFeature, ref needFeatureAdditionalInfo, out num4, out list, num2, licenseInfo, out flag7, out flag8, out errorStatus, ref skipSteamExtendedWarning, ref skipSteamExtendedBsodWarning, true, controllerInfos, this.ExternalDeviceRelationsHelper.ExternalDeviceRelationsCollection, this.SimpleDeviceInfoList, this.DuplicateGamepadCollection, fullControllersList, showGuiMessages, CS$<>8__locals1.gamepadProfiles.SlotProfiles[slot].GameName, CS$<>8__locals1.gamepadProfiles.SlotProfiles[slot].Config.Name, enableRemapBundle, enableRemapResponse, adaptiveLeftTriggerPresent, adaptiveRightTriggerPresent, virtualUsbHubPresent);
						Tracer.TraceWrite("GamepadService.EnableRemapAsync: After FillControllerProfileSlotWithConfig", false);
						if (flag7)
						{
							if (errorStatus == 3)
							{
								Tracer.TraceWrite("GamepadService.EnableRemapAsync: ExternalDeviceInvalidParameter", false);
							}
							Tracer.TraceWrite("GamepadService.EnableRemapAsync: ShouldCancelApply", false);
							await this._xbServiceCommunicator.DeleteProfiles(controllerProfileEx.GetAllProfileIds());
							if (errorStatus != null)
							{
								SlotProfilesDictionary slotProfiles = CS$<>8__locals1.gamepadProfiles.SlotProfiles;
								if (slotProfiles != null && slotProfiles.ContainsKey(slot))
								{
									defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(73, 1);
									defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: GamepadProfiles.SlotProfiles.Remove ");
									defaultInterpolatedStringHandler.AppendFormatted<Slot>(slot);
									defaultInterpolatedStringHandler.AppendLiteral(" (3)");
									Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
									CS$<>8__locals1.gamepadProfiles.SlotProfiles.Remove(slot);
									if (CS$<>8__locals1.gamepadProfiles.SlotProfiles == null || CS$<>8__locals1.gamepadProfiles.SlotProfiles.Count == 0)
									{
										IEnumerable<BaseControllerVM> gamepadCollection = this.GamepadCollection;
										Func<BaseControllerVM, bool> func;
										if ((func = CS$<>8__locals1.<>9__13) == null)
										{
											GamepadService.<>c__DisplayClass267_0 CS$<>8__locals2 = CS$<>8__locals1;
											Func<BaseControllerVM, bool> func2 = (BaseControllerVM g) => g.ID == CS$<>8__locals1.gamepadProfiles.ID;
											CS$<>8__locals2.<>9__13 = func2;
											func = func2;
										}
										BaseControllerVM baseControllerVM2 = gamepadCollection.FirstOrDefault(func);
										if (baseControllerVM2 != null)
										{
											Engine.GamepadService.PublishRemapStateChanged(baseControllerVM2, 0);
										}
									}
								}
							}
							Tracer.TraceWrite("GamepadService.EnableRemapAsync: shouldCancelApply -> return 0", false);
							return 0;
						}
						if (flag8)
						{
							Engine.EventAggregator.GetEvent<RequestReloadConfig>().Publish(CS$<>8__locals1.gamepadProfiles.SlotProfiles[slot].Config.ConfigPath);
						}
						if (!flag6)
						{
							Tracer.TraceWrite("GamepadService.EnableRemapAsync: !resultSlotCompiling", false);
							bool flag9 = false;
							if ((ulong)num4 >= (ulong)MacroCompiler.MaxRewasdMacroTableSize)
							{
								flag9 = true;
								SenderGoogleAnalytics.SendComboEditorError("SlotSizeError", (long)((ulong)num4));
								string text2 = "GamepadService.EnableRemapAsync: Error! ";
								defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 2);
								defaultInterpolatedStringHandler.AppendLiteral("Slot macro size ");
								defaultInterpolatedStringHandler.AppendFormatted<uint>(num4);
								defaultInterpolatedStringHandler.AppendLiteral(" >= ");
								defaultInterpolatedStringHandler.AppendFormatted<long>(MacroCompiler.MaxRewasdMacroTableSize);
								Tracer.TraceWrite(text2 + defaultInterpolatedStringHandler.ToStringAndClear(), false);
							}
							if (num3 >= 2048U)
							{
								SenderGoogleAnalytics.SendComboEditorError("TotalMacroAmount", (long)((ulong)num3));
								string text3 = "GamepadService.EnableRemapAsync: Error! ";
								defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 2);
								defaultInterpolatedStringHandler.AppendLiteral("MacroCounter ");
								defaultInterpolatedStringHandler.AppendFormatted<uint>(num3);
								defaultInterpolatedStringHandler.AppendLiteral(" >= REWASD_MAX_MACROS ");
								defaultInterpolatedStringHandler.AppendFormatted<int>(2048);
								Tracer.TraceWrite(text3 + defaultInterpolatedStringHandler.ToStringAndClear(), false);
							}
							byte[] array = list.SelectMany((byte[] bytes) => bytes).ToArray<byte>();
							if (array != null && array.Length != 0 && (ulong)num2 + (ulong)((long)array.Length) >= (ulong)MacroCompiler.MaxRewasdMacroTableSize)
							{
								Tracer.TraceWrite("GamepadService.EnableRemapAsync: Error! MACRO_TABLE_SIZE " + MacroCompiler.MaxRewasdMacroTableSize.ToString() + " < TotalMacroSize " + ((long)((ulong)num2 + (ulong)((long)array.Length))).ToString(), false);
								if (!flag9)
								{
									SenderGoogleAnalytics.SendComboEditorError("SlotSizeError", (long)((ulong)Convert.ToUInt32((long)((ulong)num2 + (ulong)((long)array.Length)))));
								}
							}
							if (isMobileClientRequest)
							{
								if (!enableRemapBundle.UserActions.Exists((EnableRemapButtonAction x) => x == 11))
								{
									EnableRemapResponseDialog enableRemapResponseDialog2 = new EnableRemapResponseDialog();
									enableRemapResponseDialog2.Message = DTLocalization.GetString(11450);
									enableRemapResponseDialog2.AddButton(new EnableRemapResponseButton
									{
										Text = DTLocalization.GetString(5004),
										ButtonAction = 11
									}, enableRemapBundle.IsUI);
									enableRemapResponse.AddDialog(enableRemapResponseDialog2);
								}
								enableRemapResponse.DontReCallEnableRemap = true;
							}
							SlotProfilesDictionary slotProfiles2 = CS$<>8__locals1.gamepadProfiles.SlotProfiles;
							if (slotProfiles2 != null && slotProfiles2.ContainsKey(slot))
							{
								defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(73, 1);
								defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: GamepadProfiles.SlotProfiles.Remove ");
								defaultInterpolatedStringHandler.AppendFormatted<Slot>(slot);
								defaultInterpolatedStringHandler.AppendLiteral(" (4)");
								Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
								CS$<>8__locals1.gamepadProfiles.SlotProfiles.Remove(slot);
								if (CS$<>8__locals1.gamepadProfiles.SlotProfiles == null || CS$<>8__locals1.gamepadProfiles.SlotProfiles.Count == 0)
								{
									IEnumerable<BaseControllerVM> gamepadCollection2 = this.GamepadCollection;
									Func<BaseControllerVM, bool> func3;
									if ((func3 = CS$<>8__locals1.<>9__16) == null)
									{
										GamepadService.<>c__DisplayClass267_0 CS$<>8__locals3 = CS$<>8__locals1;
										Func<BaseControllerVM, bool> func4 = (BaseControllerVM g) => g.ID == CS$<>8__locals1.gamepadProfiles.ID;
										CS$<>8__locals3.<>9__16 = func4;
										func3 = func4;
									}
									BaseControllerVM baseControllerVM3 = gamepadCollection2.FirstOrDefault(func3);
									if (baseControllerVM3 != null)
									{
										Engine.GamepadService.PublishRemapStateChanged(baseControllerVM3, 0);
									}
								}
							}
							Tracer.TraceWrite("GamepadService.EnableRemapAsync: !resultSlotCompiling -> return 0", false);
							return 0;
						}
						if (flag6)
						{
							if (REWASD_CONTROLLER_PROFILE_Extensions.IsUdpPresent(controllerProfile) && Engine.GamepadUdpServer != null)
							{
								if (Engine.GamepadUdpServer.IsUdpEnabledInPreferences && Engine.GamepadUdpServer.IsUdpServerHasException && !Engine.GamepadUdpServer.IsRunning && Engine.GamepadUdpServer.IsPortNotLockedByOtherApplication())
								{
									Tracer.TraceWrite("GamepadService.RefreshServiceProfiles: GamepadUdpServer.Start", false);
									Engine.GamepadUdpServer.Start();
								}
								if (isMobileClientRequest && (!Engine.GamepadUdpServer.IsUdpEnabledInPreferences || (Engine.GamepadUdpServer.IsUdpEnabledInPreferences && (Engine.GamepadUdpServer.IsUdpServerHasException || !Engine.GamepadUdpServer.IsPortNotLockedByOtherApplication()))))
								{
									if (!enableRemapBundle.UserActions.Exists((EnableRemapButtonAction x) => x == 14))
									{
										EnableRemapResponseDialog enableRemapResponseDialog3 = new EnableRemapResponseDialog();
										enableRemapResponseDialog3.Message = DTLocalization.GetString(12371);
										enableRemapResponseDialog3.AddButton(new EnableRemapResponseButton
										{
											Text = DTLocalization.GetString(11824),
											ButtonAction = 14
										}, true);
										enableRemapResponseDialog3.AddButton(new EnableRemapResponseButton
										{
											Text = DTLocalization.GetString(5005),
											ButtonAction = 0
										}, false);
										enableRemapResponse.AddDialog(enableRemapResponseDialog3);
										Tracer.TraceWrite("GamepadService.EnableRemapAsync: ask for mobile user action -> return 0", false);
										return 0;
									}
								}
							}
							controllerProfile.GuiContext |= 4UL;
							if (needMacroFeature || needAdvancedMappingFeature || needRapidFireFeature || needMobileControllerFeature)
							{
								if (needAdvancedMappingFeature)
								{
									controllerProfile.GuiContext |= 8UL;
								}
								if (needMacroFeature)
								{
									controllerProfile.GuiContext |= 16UL;
								}
								if (needRapidFireFeature)
								{
									controllerProfile.GuiContext |= 32UL;
								}
							}
							if (controllerProfile.CheatsEnabled)
							{
								isControllerCheatsExist = true;
							}
							anyProfileFound = true;
							if (isCacheAllowed)
							{
								string fileMD5Hash = XBUtils.GetFileMD5Hash(CS$<>8__locals1.gamepadProfiles.SlotProfiles[slot].Config.ConfigPath);
								if (!string.IsNullOrEmpty(fileMD5Hash))
								{
									defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(48, 3);
									defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync Slot ");
									defaultInterpolatedStringHandler.AppendFormatted<Slot>(slot);
									defaultInterpolatedStringHandler.AppendLiteral(" MD5 ");
									defaultInterpolatedStringHandler.AppendFormatted(fileMD5Hash);
									defaultInterpolatedStringHandler.AppendLiteral(" JSON ");
									defaultInterpolatedStringHandler.AppendFormatted(CS$<>8__locals1.gamepadProfiles.SlotProfiles[slot].Config.ConfigPath);
									Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
									slotInfoList.Add(new CacheSlotInfo(slot, CS$<>8__locals1.gamepadProfiles.SlotProfiles[slot].Config.ConfigPath, fileMD5Hash));
								}
							}
						}
						else
						{
							defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(73, 1);
							defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: GamepadProfiles.SlotProfiles.Remove ");
							defaultInterpolatedStringHandler.AppendFormatted<Slot>(slot);
							defaultInterpolatedStringHandler.AppendLiteral(" (5)");
							Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
							CS$<>8__locals1.gamepadProfiles.SlotProfiles.Remove(slot);
						}
					}
					else
					{
						controllerProfile = REWASD_CONTROLLER_PROFILE_Extensions.FillControllerInfos(controllerProfile, controllerInfos, Engine.UserSettingsService, this.DuplicateGamepadCollection, false, fullControllersList, false, false, null, this.SimpleDeviceInfoList);
						uint num5 = num2;
						uint num6 = 0U;
						List<byte[]> list2 = new List<byte[]>();
						if (!MacroCompiler.CompileSlotBroadcast(ref controllerProfile, slot, ref num3, ref num6, ref list2, ref num2))
						{
							defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(67, 1);
							defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: CompileSlotBroadcast failed. Slot ");
							defaultInterpolatedStringHandler.AppendFormatted<Slot>(slot);
							Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
						}
						if (ControllerTypeExtensions.ConvertPhysicalTypesToEnums(0, controllerProfile.Type, controllerProfile.Id).Any((ControllerTypeEnum x) => ControllerTypeExtensions.IsAnySonyDualSense(x)) && (adaptiveLeftTriggerPresent || adaptiveRightTriggerPresent))
						{
							defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(99, 1);
							defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: CompileAllSonyDualSenseAdaptiveTriggerResetEffects for empty Slot ");
							defaultInterpolatedStringHandler.AppendFormatted<Slot>(slot);
							Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
							if (!MacroCompiler.CompileAllSonyDualSenseAdaptiveTriggerResetEffects(ref controllerProfile, ControllerTypeExtensions.ConvertPhysicalTypesToEnums(0, controllerProfile.Type, controllerProfile.Id), adaptiveLeftTriggerPresent, adaptiveRightTriggerPresent, ref num3, ref num6, ref list2, ref num2))
							{
								defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(97, 1);
								defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: CompileAllSonyDualSenseAdaptiveTriggerResetEffects failed. Slot ");
								defaultInterpolatedStringHandler.AppendFormatted<Slot>(slot);
								Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
							}
						}
						byte[] array2 = list2.SelectMany((byte[] bytes) => bytes).ToArray<byte>();
						if (array2 != null && array2.Length != 0)
						{
							defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 1);
							defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: Slot macro size ");
							defaultInterpolatedStringHandler.AppendFormatted<long>((long)((ulong)num5 + (ulong)((long)array2.Length)));
							Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
							if ((ulong)num5 + (ulong)((long)array2.Length) < (ulong)MacroCompiler.MaxRewasdMacroTableSize)
							{
								Buffer.BlockCopy(array2, 0, controllerProfile.Macros, (int)num5, array2.Length);
							}
						}
						if (controllerTypes.Any((ControllerTypeEnum ct) => ControllerTypeExtensions.IsAllowedHardwareMappingWithoutAdvancedFeature(ct)))
						{
							for (int k = 0; k < 15; k++)
							{
								DISC_SOFT_KERNEL_REMAP disc_SOFT_KERNEL_REMAP = DISC_SOFT_KERNEL_REMAP.CreateBlankInstance();
								if (controllerTypes.Length > k && controllerProfile.Remap.Length > k)
								{
									MacroCompiler.FillGamepadDefaultKernelRemap(ref disc_SOFT_KERNEL_REMAP, controllerTypes[k], Engine.UserSettingsService, false);
									controllerProfile.Remap[k] = DISC_SOFT_HID_REMAP_Extensions.DeepClone(disc_SOFT_KERNEL_REMAP, controllerTypes[k]);
									MacroCompiler.ResetRemapFlagMotors(ref controllerProfile.Remap[k].Gamepad.Flags);
									MacroCompiler.SetRemapFlagButtons(ref controllerProfile.Remap[k].Gamepad.Flags);
									MacroCompiler.SetRemapFlagTriggers(ref controllerProfile.Remap[k].Gamepad.Flags);
									MacroCompiler.SetRemapFlagSticks(ref controllerProfile.Remap[k].Gamepad.Flags);
								}
							}
						}
					}
					virtualGamepadList.Add(controllerProfile.VirtualType);
					if (controllerProfile.VirtualType != 0U && !REWASD_CONTROLLER_PROFILE_Extensions.IsExternalDevicePresent(controllerProfile))
					{
						localVirtualGamepadPresent = true;
					}
					if (num3 > 1024U)
					{
						SenderGoogleAnalytics.SendComboEditorError("TotalMacroAmount", (long)((ulong)num3));
					}
					Tracer.TraceWrite("GamepadService.EnableRemapAsync: After iterating for slots and filling profiles", false);
					if ((this._licensingService.IsMacroFeatureUnlocked || this._licensingService.IsTurboFeatureUnlocked || this._licensingService.IsToggleFeatureUnlocked) && Convert.ToBoolean(RegistryHelper.GetValue("Config", "StopMacroHotkey", 1, false)))
					{
						controllerProfile.StopMacrosMask[0].ButtonAndFlags = 5;
						controllerProfile.StopMacrosMask[0].ControllerMask = 32767;
						controllerProfile.StopMacrosMask[1].ButtonAndFlags = 6;
						controllerProfile.StopMacrosMask[1].ControllerMask = 32767;
						controllerProfile.StopMacrosMask[2].ButtonAndFlags = 7;
						controllerProfile.StopMacrosMask[2].ControllerMask = 32767;
						controllerProfile.StopMacrosMask[3].ButtonAndFlags = 8;
						controllerProfile.StopMacrosMask[3].ControllerMask = 32767;
						if (controllerProfile.Type.Any((uint x) => x == 100U || x == 101U || x == 102U || x == 268435455U))
						{
							for (int l = 0; l < controllerProfile.Type.Length; l++)
							{
								if (controllerProfile.Type[l] == 100U || controllerProfile.Type[l] == 101U || controllerProfile.Type[l] == 102U || controllerProfile.Type[l] == 268435455U)
								{
									if (controllerProfile.Type[l] == 268435455U && controllerProfile.Id[l] != 0UL)
									{
										try
										{
											if (!ControllerTypeExtensions.IsEngineControllerControlPad(EnineControllerTypeExtensions.ToControllerTypeEnum(RegistryHelper.GetValue("Controllers\\" + controllerProfile.Id[l].ToString(), "EngineControllerType", 0, false))))
											{
												goto IL_2031;
											}
										}
										catch
										{
										}
									}
									ushort num7 = (ushort)(1 << (int)((byte)l));
									if (Convert.ToBoolean((int)(controllerProfile.StopMacrosMask[0].ControllerMask & num7)))
									{
										REWASD_BUTTON_MASK[] stopMacrosMask = controllerProfile.StopMacrosMask;
										int num8 = 0;
										stopMacrosMask[num8].ControllerMask = stopMacrosMask[num8].ControllerMask & ~num7;
									}
									if (Convert.ToBoolean((int)(controllerProfile.StopMacrosMask[1].ControllerMask & num7)))
									{
										REWASD_BUTTON_MASK[] stopMacrosMask2 = controllerProfile.StopMacrosMask;
										int num9 = 1;
										stopMacrosMask2[num9].ControllerMask = stopMacrosMask2[num9].ControllerMask & ~num7;
									}
									if (Convert.ToBoolean((int)(controllerProfile.StopMacrosMask[2].ControllerMask & num7)))
									{
										REWASD_BUTTON_MASK[] stopMacrosMask3 = controllerProfile.StopMacrosMask;
										int num10 = 2;
										stopMacrosMask3[num10].ControllerMask = stopMacrosMask3[num10].ControllerMask & ~num7;
									}
									if (Convert.ToBoolean((int)(controllerProfile.StopMacrosMask[3].ControllerMask & num7)))
									{
										REWASD_BUTTON_MASK[] stopMacrosMask4 = controllerProfile.StopMacrosMask;
										int num11 = 3;
										stopMacrosMask4[num11].ControllerMask = stopMacrosMask4[num11].ControllerMask & ~num7;
									}
								}
								IL_2031:;
							}
						}
					}
					if (Convert.ToBoolean(RegistryHelper.GetValue("Config", "PreventSleepIfComboIsBeingEmulated", 0, false)))
					{
						controllerProfile.SleepFlags |= 1;
					}
					if (REWASD_CONTROLLER_PROFILE_Extensions.IsExternalDevicePresent(controllerProfile))
					{
						ExternalDeviceType type = 1000;
						if (REWASD_CONTROLLER_PROFILE_Extensions.IsExternalBluetoothAdapterPresent(controllerProfile))
						{
							type = 0;
						}
						if (REWASD_CONTROLLER_PROFILE_Extensions.IsExternalDeviceWithSerialPortPresent(controllerProfile))
						{
							type = 1;
						}
						if (REWASD_CONTROLLER_PROFILE_Extensions.IsExternalDeviceWithBluetoothPresent(controllerProfile) && REWASD_CONTROLLER_PROFILE_Extensions.IsExternalDeviceWithSerialPortPresent(controllerProfile))
						{
							type = 2;
						}
						List<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> serviceProfileWithExternalDevice = this.GetServiceProfileWithExternalDevice(type);
						if (serviceProfileWithExternalDevice != null)
						{
							foreach (Wrapper<REWASD_CONTROLLER_PROFILE_EX> wrapper in serviceProfileWithExternalDevice)
							{
								bool flag10 = false;
								if (wrapper.Value.IsExternalDevicePresent(type))
								{
									foreach (REWASD_CONTROLLER_PROFILE rewasd_CONTROLLER_PROFILE in wrapper.Value.Profiles)
									{
										if (REWASD_CONTROLLER_PROFILE_Extensions.IsExternalDeviceEqual(rewasd_CONTROLLER_PROFILE, REWASD_CONTROLLER_PROFILE_Extensions.GetExternalDeviceId(controllerProfile)))
										{
											foreach (ulong num12 in rewasd_CONTROLLER_PROFILE.Id.Where((ulong x) => x > 0UL).ToList<ulong>())
											{
												if (!CS$<>8__locals1.controllerIds.Contains(num12))
												{
													if (!externalDeviceOverwritePrevConfig)
													{
														Tracer.TraceWrite("GamepadService.EnableRemapAsync: ExternalDeviceOverwritePrevConfig: Error! External device is already in use", false);
														Tracer.TraceWrite("GamepadService.EnableRemapAsync: CancelApply", false);
														await this._xbServiceCommunicator.DeleteProfiles(controllerProfileEx.GetAllProfileIds());
														Tracer.TraceWrite("GamepadService.EnableRemapAsync: !externalDeviceOverwritePrevConfig -> return 0", false);
														return 0;
													}
													flag10 = true;
													break;
												}
											}
											List<ulong>.Enumerator enumerator4 = default(List<ulong>.Enumerator);
										}
									}
								}
								if (flag10)
								{
									string[] array4 = new string[5];
									array4[0] = "GamepadService.EnableRemapAsync: ExternalDeviceOverwritePrevConfig: overwrite profiles ";
									int num13 = 1;
									defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 1);
									defaultInterpolatedStringHandler.AppendLiteral("0x");
									defaultInterpolatedStringHandler.AppendFormatted<ushort>(wrapper.Value.ServiceProfileIds[0], "X");
									defaultInterpolatedStringHandler.AppendLiteral(" ");
									array4[num13] = defaultInterpolatedStringHandler.ToStringAndClear();
									int num14 = 2;
									defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 1);
									defaultInterpolatedStringHandler.AppendLiteral("0x");
									defaultInterpolatedStringHandler.AppendFormatted<ushort>(wrapper.Value.ServiceProfileIds[1], "X");
									defaultInterpolatedStringHandler.AppendLiteral(" ");
									array4[num14] = defaultInterpolatedStringHandler.ToStringAndClear();
									int num15 = 3;
									defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 1);
									defaultInterpolatedStringHandler.AppendLiteral("0x");
									defaultInterpolatedStringHandler.AppendFormatted<ushort>(wrapper.Value.ServiceProfileIds[2], "X");
									defaultInterpolatedStringHandler.AppendLiteral(" ");
									array4[num15] = defaultInterpolatedStringHandler.ToStringAndClear();
									int num16 = 4;
									defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 1);
									defaultInterpolatedStringHandler.AppendLiteral("0x");
									defaultInterpolatedStringHandler.AppendFormatted<ushort>(wrapper.Value.ServiceProfileIds[3], "X");
									array4[num16] = defaultInterpolatedStringHandler.ToStringAndClear();
									Tracer.TraceWrite(string.Concat(array4), false);
									await this._xbServiceCommunicator.DeleteProfiles(wrapper.Value.GetAllProfileIds());
									await Task.Delay(100);
								}
							}
							List<Wrapper<REWASD_CONTROLLER_PROFILE_EX>>.Enumerator enumerator3 = default(List<Wrapper<REWASD_CONTROLLER_PROFILE_EX>>.Enumerator);
						}
					}
					if (controllerTypes.Any((ControllerTypeEnum x) => ControllerTypeExtensions.IsNVidiaShield2015(x)))
					{
						for (int m = 0; m < 15; m++)
						{
							if (controllerPhysicalTypes[m] == 0U && controllerProfile.Type[m] != 0U)
							{
								CS$<>8__locals1.controllerIds[m] = controllerProfile.Id[m];
								controllerPhysicalTypes[m] = controllerProfile.Type[m];
							}
						}
						controllerTypes = ControllerTypeExtensions.ConvertPhysicalTypesToEnums(0, controllerPhysicalTypes, CS$<>8__locals1.controllerIds);
					}
					if (controllerProfile.Type.Any((uint x) => x == 268435455U || x == 268435453U || x == 268435454U))
					{
						controllerProfile.FeatureFlags |= 128U;
					}
					controllerProfileEx.Profiles[slot] = controllerProfile;
					controllerProfile = default(REWASD_CONTROLLER_PROFILE);
				}
				IEnumerator enumerator2 = null;
				if (!anyProfileFound)
				{
					BaseControllerVM gamepad = this.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.ID == CS$<>8__locals1.gamepadProfiles.ID);
					if (restoreSlotsToDefaults.Count > 0 && gamepad != null)
					{
						REWASD_CONTROLLER_PROFILE_EX rewasd_CONTROLLER_PROFILE_EX = this.ServiceProfilesCollection.FindByID(gamepad.ID);
						if (rewasd_CONTROLLER_PROFILE_EX != null)
						{
							if (rewasd_CONTROLLER_PROFILE_EX.ServiceProfileIds.Any((ushort x) => x > 0))
							{
								await this._xbServiceCommunicator.DeleteProfiles(rewasd_CONTROLLER_PROFILE_EX.GetAllProfileIds());
							}
						}
					}
					if (isMobileClientRequest && currentSlotResetedDueToLicenseFeature)
					{
						enableRemapResponse.AddDialog(this.Create4SlotsRequiredDlg());
					}
					SlotProfilesDictionary slotProfiles3 = CS$<>8__locals1.gamepadProfiles.SlotProfiles;
					if (slotProfiles3 != null && slotProfiles3.ContainsKey(slotNumber))
					{
						defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(73, 1);
						defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: GamepadProfiles.SlotProfiles.Remove ");
						defaultInterpolatedStringHandler.AppendFormatted<Slot>(slotNumber);
						defaultInterpolatedStringHandler.AppendLiteral(" (7)");
						Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
						CS$<>8__locals1.gamepadProfiles.SlotProfiles.Remove(slotNumber);
						if ((CS$<>8__locals1.gamepadProfiles.SlotProfiles == null || CS$<>8__locals1.gamepadProfiles.SlotProfiles.Count == 0) && gamepad != null)
						{
							Engine.GamepadService.PublishRemapStateChanged(gamepad, 0);
						}
					}
					Tracer.TraceWrite("GamepadService.EnableRemapAsync: No profiles found. Leaving", false);
					this.IsAsyncRemapInProgress = false;
					Tracer.TraceWrite("GamepadService.EnableRemapAsync: !anyProfileFound -> return 0", false);
					return 0;
				}
				if (showGuiMessages || isMobileClientRequest)
				{
					if (needMacroFeature || needAdvancedMappingFeature || needRapidFireFeature || needMobileControllerFeature)
					{
						TaskAwaiter<bool> taskAwaiter = this._licensingService.TryActivateFeatureOnApplyConfig(needMacroFeature, needAdvancedMappingFeature, needRapidFireFeature, needMobileControllerFeature, needFeatureAdditionalInfo, showGuiMessages, enableRemapBundle, enableRemapResponse).GetAwaiter();
						if (!taskAwaiter.IsCompleted)
						{
							await taskAwaiter;
							TaskAwaiter<bool> taskAwaiter2;
							taskAwaiter = taskAwaiter2;
							taskAwaiter2 = default(TaskAwaiter<bool>);
						}
						if (!taskAwaiter.GetResult())
						{
							Tracer.TraceWrite("GamepadService.EnableRemapAsync: Need Feature -> return 0", false);
							return 0;
						}
						bool flag11;
						this.TryShowWarningForAdditionalGamepads(showGuiMessages, localVirtualGamepadPresent, CS$<>8__locals1.controllerIds, out flag11, enableRemapBundle, enableRemapResponse);
						if (flag11)
						{
							Tracer.TraceWrite("GamepadService.EnableRemapAsync: breakApplyOnReApply -> return 0", false);
							return 0;
						}
						ushort num17 = await this.EnableRemap(CS$<>8__locals1.gamepadProfiles, false, changeGamepadSlot, slotNumber, false, false, null, null, true);
						if (isMobileClientRequest)
						{
							enableRemapResponse.IsSucceded = num17 > 0;
						}
						if (num17 == 0)
						{
							SlotProfilesDictionary slotProfiles4 = CS$<>8__locals1.gamepadProfiles.SlotProfiles;
							if (slotProfiles4 != null && slotProfiles4.ContainsKey(slotNumber))
							{
								defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(73, 1);
								defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: GamepadProfiles.SlotProfiles.Remove ");
								defaultInterpolatedStringHandler.AppendFormatted<Slot>(slotNumber);
								defaultInterpolatedStringHandler.AppendLiteral(" (9)");
								Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
								CS$<>8__locals1.gamepadProfiles.SlotProfiles.Remove(slotNumber);
							}
							Tracer.TraceWrite("GamepadService.EnableRemapAsync: reAppliedProfileId == 0 -> return 0", false);
						}
						return num17;
					}
					else if (isControllerCheatsExist && isMobileClientRequest && RegistryHelper.GetBool(RegistryHelper.CONFIRMATION_REG_PATH, "CheatWarning", true) && RegistryHelper.GetBool(RegistryHelper.CONFIRMATION_REG_PATH, "CheatWarning", true))
					{
						if (!enableRemapBundle.UserActions.Exists((EnableRemapButtonAction x) => x == 12))
						{
							EnableRemapResponseDialog enableRemapResponseDialog4 = new EnableRemapResponseDialog();
							enableRemapResponseDialog4.Message = DTLocalization.GetString(11449);
							enableRemapResponseDialog4.AddButton(new EnableRemapResponseButton
							{
								Text = DTLocalization.GetString(5004),
								ButtonAction = 12
							}, true);
							enableRemapResponse.AddDialog(enableRemapResponseDialog4);
						}
					}
				}
				if (isCacheAllowed)
				{
					Tracer.TraceWrite("GamepadService.EnableRemapAsync: Add compiled ControllerProfile to CachedProfilesCollection", false);
					this.CachedProfilesCollection.Add(new REWASD_CONTROLLER_PROFILE_CACHE(CS$<>8__locals1.controllerIds, controllerTypes, slotInfoList, firstSlotWithConfig, virtualGamepadList, controllerProfileEx));
				}
				slotInfoList = null;
			}
			BaseControllerVM existingGamepad = null;
			Slot slotToSwitch = 0;
			REWASD_CONTROLLER_PROFILE_EX oldProfileEx = null;
			bool forceFailRemapResponse = false;
			if (changeGamepadSlot)
			{
				Slot slot3 = 0;
				existingGamepad = this.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.ID == CS$<>8__locals1.gamepadProfiles.ID);
				if (existingGamepad != null)
				{
					Slot currentSlot = existingGamepad.CurrentSlot;
					if (Enum.IsDefined(typeof(Slot), slotNumber))
					{
						slotToSwitch = slotNumber;
					}
					else if (firstSlotWithConfig != null)
					{
						if (CS$<>8__locals1.gamepadProfiles.SlotProfiles.ContainsKey(currentSlot))
						{
							GamepadProfile gamepadProfile8 = CS$<>8__locals1.gamepadProfiles.SlotProfiles[currentSlot];
							if (((gamepadProfile8 != null) ? gamepadProfile8.Config : null) != null)
							{
								slotToSwitch = currentSlot;
								slot3 = currentSlot;
								goto IL_2CC4;
							}
						}
						slotToSwitch = firstSlotWithConfig.Value;
						slot3 = firstSlotWithConfig.Value;
					}
					else
					{
						slotToSwitch = existingGamepad.CurrentSlot;
						slot3 = existingGamepad.CurrentSlot;
					}
					IL_2CC4:
					oldProfileEx = this.ServiceProfilesCollection.FindByID(existingGamepad.ID);
				}
				bool flag12 = isSlotFeatureUnlocked || slotToSwitch == 0;
				bool flag13 = isSlotFeatureUnlocked || slot3 == 0;
				if (!isSlotFeatureUnlocked)
				{
					if (slotToSwitch == 1)
					{
						if (controllerTypes.Any((ControllerTypeEnum x) => x == 3 || x == 12))
						{
							flag12 = true;
						}
					}
					if (slot3 == 1)
					{
						if (controllerTypes.Any((ControllerTypeEnum x) => x == 3 || x == 12))
						{
							flag13 = true;
						}
					}
					if (slotToSwitch > 1)
					{
						if (controllerTypes.Any((ControllerTypeEnum x) => x == 12))
						{
							flag12 = true;
						}
					}
					if (slot3 > 1)
					{
						if (controllerTypes.Any((ControllerTypeEnum x) => x == 12))
						{
							flag13 = true;
						}
					}
					if (!flag12)
					{
						Slot slot4 = slotToSwitch;
						slotToSwitch = (flag13 ? slot3 : 0);
						if (slot4 != slotToSwitch)
						{
							if (isMobileClientRequest)
							{
								enableRemapResponse.AddDialog(this.Create4SlotsRequiredDlg());
								forceFailRemapResponse = true;
							}
							if (oldProfileEx == null)
							{
								SlotProfilesDictionary slotProfiles5 = CS$<>8__locals1.gamepadProfiles.SlotProfiles;
								if (slotProfiles5 != null && slotProfiles5.ContainsKey(slot4))
								{
									defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(73, 1);
									defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: GamepadProfiles.SlotProfiles.Remove ");
									defaultInterpolatedStringHandler.AppendFormatted<Slot>(slot4);
									defaultInterpolatedStringHandler.AppendLiteral(" (6)");
									Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
									CS$<>8__locals1.gamepadProfiles.SlotProfiles.Remove(slot4);
								}
								Tracer.TraceWrite("GamepadService.EnableRemapAsync: oldProfileEx == null -> return 0", false);
								return 0;
							}
						}
					}
				}
			}
			bool flag14;
			this.TryShowWarningForAdditionalGamepads(showGuiMessages, localVirtualGamepadPresent, CS$<>8__locals1.controllerIds, out flag14, enableRemapBundle, enableRemapResponse);
			ushort num18;
			if (flag14)
			{
				Tracer.TraceWrite("GamepadService.EnableRemapAsync: breakApply -> return 0", false);
				num18 = 0;
			}
			else
			{
				if (controllerTypes.Any((ControllerTypeEnum x) => ControllerTypeExtensions.IsAnySteam(x) || ControllerTypeExtensions.IsAnyAzeron(x) || ControllerTypeExtensions.IsFlydigi(x)))
				{
					List<REWASD_CONTROLLER_PROFILE_EX> captureProfiles = await this._xbServiceCommunicator.GetExclusiveCaptureProfilesList();
					GamepadService.<>c__DisplayClass267_1 CS$<>8__locals4 = new GamepadService.<>c__DisplayClass267_1();
					CS$<>8__locals4.CS$<>8__locals1 = CS$<>8__locals1;
					CS$<>8__locals4.i = 0;
					while (CS$<>8__locals4.i < controllerTypes.Length)
					{
						if (ControllerTypeExtensions.IsAnySteam(controllerTypes[CS$<>8__locals4.i]) || ControllerTypeExtensions.IsAnyAzeron(controllerTypes[CS$<>8__locals4.i]) || ControllerTypeExtensions.IsFlydigi(controllerTypes[CS$<>8__locals4.i]))
						{
							IEnumerable<REWASD_CONTROLLER_PROFILE_EX> enumerable = captureProfiles;
							Func<REWASD_CONTROLLER_PROFILE_EX, bool> func5;
							if ((func5 = CS$<>8__locals4.<>9__28) == null)
							{
								GamepadService.<>c__DisplayClass267_1 CS$<>8__locals5 = CS$<>8__locals4;
								Func<REWASD_CONTROLLER_PROFILE_EX, bool> func6 = (REWASD_CONTROLLER_PROFILE_EX x) => x.Profiles[0].Id[0] == CS$<>8__locals4.CS$<>8__locals1.controllerIds[CS$<>8__locals4.i] && UtilsCommon.IsExclusiveCaptureControllerPhysicalType(x.Profiles[0].Type[0]);
								CS$<>8__locals5.<>9__28 = func6;
								func5 = func6;
							}
							foreach (REWASD_CONTROLLER_PROFILE_EX rewasd_CONTROLLER_PROFILE_EX2 in enumerable.Where(func5))
							{
								string text4 = "GamepadService.EnableRemapAsync: delete CaptureProfile for Id: ";
								defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 2);
								defaultInterpolatedStringHandler.AppendLiteral("0x");
								defaultInterpolatedStringHandler.AppendFormatted<ulong>(CS$<>8__locals4.CS$<>8__locals1.controllerIds[CS$<>8__locals4.i], "X");
								defaultInterpolatedStringHandler.AppendLiteral(" / ");
								defaultInterpolatedStringHandler.AppendFormatted<ulong>(CS$<>8__locals4.CS$<>8__locals1.controllerIds[CS$<>8__locals4.i]);
								Tracer.TraceWrite(text4 + defaultInterpolatedStringHandler.ToStringAndClear(), false);
								await this._xbServiceCommunicator.DeleteProfile(rewasd_CONTROLLER_PROFILE_EX2.ServiceProfileIds[0]);
							}
							IEnumerator<REWASD_CONTROLLER_PROFILE_EX> enumerator5 = null;
						}
						int j = CS$<>8__locals4.i;
						CS$<>8__locals4.i = j + 1;
					}
					CS$<>8__locals4 = null;
					captureProfiles = null;
				}
				if (controllerTypes.Any((ControllerTypeEnum x) => ControllerTypeExtensions.IsAllowedHiddenAppliedConfig(x)))
				{
					List<REWASD_CONTROLLER_PROFILE_EX> captureProfiles = await this._xbServiceCommunicator.GetHiddenAppliedConfigsList();
					GamepadService.<>c__DisplayClass267_2 CS$<>8__locals6 = new GamepadService.<>c__DisplayClass267_2();
					CS$<>8__locals6.CS$<>8__locals2 = CS$<>8__locals1;
					CS$<>8__locals6.i = 0;
					while (CS$<>8__locals6.i < controllerTypes.Length)
					{
						if (ControllerTypeExtensions.IsAllowedHiddenAppliedConfig(controllerTypes[CS$<>8__locals6.i]))
						{
							IEnumerable<REWASD_CONTROLLER_PROFILE_EX> enumerable2 = captureProfiles;
							Func<REWASD_CONTROLLER_PROFILE_EX, bool> func7;
							if ((func7 = CS$<>8__locals6.<>9__29) == null)
							{
								GamepadService.<>c__DisplayClass267_2 CS$<>8__locals7 = CS$<>8__locals6;
								Func<REWASD_CONTROLLER_PROFILE_EX, bool> func8 = (REWASD_CONTROLLER_PROFILE_EX x) => x.Profiles[0].Id[0] == CS$<>8__locals6.CS$<>8__locals2.controllerIds[CS$<>8__locals6.i] && UtilsCommon.IsHiddenAppliedConfigControllerPhysicalType(x.Profiles[0].Type[0]);
								CS$<>8__locals7.<>9__29 = func8;
								func7 = func8;
							}
							foreach (REWASD_CONTROLLER_PROFILE_EX rewasd_CONTROLLER_PROFILE_EX3 in enumerable2.Where(func7))
							{
								string text5 = "GamepadService.EnableRemapAsync: delete HiddenAppliedConfig for Id: ";
								defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 2);
								defaultInterpolatedStringHandler.AppendLiteral("0x");
								defaultInterpolatedStringHandler.AppendFormatted<ulong>(CS$<>8__locals6.CS$<>8__locals2.controllerIds[CS$<>8__locals6.i], "X");
								defaultInterpolatedStringHandler.AppendLiteral(" / ");
								defaultInterpolatedStringHandler.AppendFormatted<ulong>(CS$<>8__locals6.CS$<>8__locals2.controllerIds[CS$<>8__locals6.i]);
								Tracer.TraceWrite(text5 + defaultInterpolatedStringHandler.ToStringAndClear(), false);
								await this._xbServiceCommunicator.DeleteProfile(rewasd_CONTROLLER_PROFILE_EX3.ServiceProfileIds[0]);
							}
							IEnumerator<REWASD_CONTROLLER_PROFILE_EX> enumerator5 = null;
						}
						int j = CS$<>8__locals6.i;
						CS$<>8__locals6.i = j + 1;
					}
					CS$<>8__locals6 = null;
					captureProfiles = null;
				}
				bool disconnectFromNintendoConsole = false;
				if (oldProfileEx != null)
				{
					for (int n = 0; n < 4; n++)
					{
						controllerProfileEx.Profiles[n].Enabled = oldProfileEx.Enabled[n];
					}
				}
				ushort exclusiveControllersMask = 0;
				int i = 0;
				while (i < 4)
				{
					ushort oldProfileId = 0;
					ushort flags = 0;
					int j;
					if (oldProfileEx != null)
					{
						string text6 = "GamepadService.EnableRemapAsync: ";
						defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 1);
						defaultInterpolatedStringHandler.AppendLiteral("replace existing ProfileId 0x");
						defaultInterpolatedStringHandler.AppendFormatted<ushort>(oldProfileEx.ServiceProfileIds[i], "X");
						Tracer.TraceWrite(text6 + defaultInterpolatedStringHandler.ToStringAndClear(), false);
						oldProfileId = oldProfileEx.ServiceProfileIds[i];
						flags |= 1;
						if (i == slotToSwitch)
						{
							disconnectFromNintendoConsole = controllerProfileEx.Profiles[i].Enabled && REWASD_CONTROLLER_PROFILE_Extensions.IsExternalDeviceWithBluetoothPresent(controllerProfileEx.Profiles[i]) && REWASD_CONTROLLER_PROFILE_Extensions.IsExternalConsoleFlagPresent(controllerProfileEx.Profiles[i]) && REWASD_CONTROLLER_PROFILE_Extensions.IsExternalDeviceWithBluetoothPresent(oldProfileEx.Profiles[i]) && REWASD_CONTROLLER_PROFILE_Extensions.IsExternalConsoleFlagPresent(oldProfileEx.Profiles[i]) && controllerProfileEx.Profiles[i].VirtualType == 48U && (controllerProfileEx.Profiles[i].SwitchProLeftStickDeadzone != oldProfileEx.Profiles[i].SwitchProLeftStickDeadzone || controllerProfileEx.Profiles[i].SwitchProRightStickDeadzone != oldProfileEx.Profiles[i].SwitchProRightStickDeadzone);
							if (ControllerTypeExtensions.ConvertPhysicalTypesToEnums(0, controllerProfileEx.ControllerPhysicalTypes, null).Any((ControllerTypeEnum x) => ControllerTypeExtensions.IsAnySonyDualSense(x)))
							{
								bool resetAdaptiveLeftTrigger = false;
								bool resetAdaptiveRightTrigger = false;
								byte[] array5 = new byte[11];
								array5[0] = 5;
								byte[] resetTriggerPacket = array5;
								if (REWASD_CONTROLLER_PROFILE_Extensions.IsSlotWithLeftAdaptiveTriggerSettings(oldProfileEx.Profiles[i]) && !REWASD_CONTROLLER_PROFILE_Extensions.IsSlotWithLeftAdaptiveTriggerSettings(controllerProfileEx.Profiles[i]))
								{
									resetAdaptiveLeftTrigger = true;
								}
								if (REWASD_CONTROLLER_PROFILE_Extensions.IsSlotWithRightAdaptiveTriggerSettings(oldProfileEx.Profiles[i]) && !REWASD_CONTROLLER_PROFILE_Extensions.IsSlotWithRightAdaptiveTriggerSettings(controllerProfileEx.Profiles[i]))
								{
									resetAdaptiveRightTrigger = true;
								}
								if (!resetAdaptiveLeftTrigger)
								{
									if (controllerProfileEx.Profiles.All((REWASD_CONTROLLER_PROFILE x) => !REWASD_CONTROLLER_PROFILE_Extensions.IsSlotWithLeftAdaptiveTriggerSettings(x)))
									{
										if (oldProfileEx.Profiles.Any((REWASD_CONTROLLER_PROFILE x) => REWASD_CONTROLLER_PROFILE_Extensions.IsSlotWithLeftAdaptiveTriggerSettings(x)))
										{
											resetAdaptiveLeftTrigger = true;
										}
									}
								}
								if (!resetAdaptiveRightTrigger)
								{
									if (controllerProfileEx.Profiles.All((REWASD_CONTROLLER_PROFILE x) => !REWASD_CONTROLLER_PROFILE_Extensions.IsSlotWithRightAdaptiveTriggerSettings(x)))
									{
										if (oldProfileEx.Profiles.Any((REWASD_CONTROLLER_PROFILE x) => REWASD_CONTROLLER_PROFILE_Extensions.IsSlotWithRightAdaptiveTriggerSettings(x)))
										{
											resetAdaptiveRightTrigger = true;
										}
									}
								}
								if (resetAdaptiveLeftTrigger || resetAdaptiveRightTrigger)
								{
									int indexDualSense = 0;
									while (indexDualSense < ControllerTypeExtensions.ConvertPhysicalTypesToEnums(0, oldProfileEx.ControllerPhysicalTypes, null).Length)
									{
										if (ControllerTypeExtensions.IsAnySonyDualSense(ControllerTypeExtensions.ConvertPhysicalTypesToEnums(0, oldProfileEx.ControllerPhysicalTypes, null)[indexDualSense]))
										{
											TaskAwaiter<ServiceResponseWrapper<REWASD_SET_ADAPTIVE_TRIGGERS_RESPONSE>> taskAwaiter3 = this._xbServiceCommunicator.SetAdaptiveTrigger(CS$<>8__locals1.controllerIds[indexDualSense], oldProfileEx.ControllerPhysicalTypes[indexDualSense], resetAdaptiveLeftTrigger, resetAdaptiveRightTrigger, resetAdaptiveLeftTrigger ? resetTriggerPacket : null, resetAdaptiveRightTrigger ? resetTriggerPacket : null, 0).GetAwaiter();
											if (!taskAwaiter3.IsCompleted)
											{
												await taskAwaiter3;
												TaskAwaiter<ServiceResponseWrapper<REWASD_SET_ADAPTIVE_TRIGGERS_RESPONSE>> taskAwaiter4;
												taskAwaiter3 = taskAwaiter4;
												taskAwaiter4 = default(TaskAwaiter<ServiceResponseWrapper<REWASD_SET_ADAPTIVE_TRIGGERS_RESPONSE>>);
											}
											if (taskAwaiter3.GetResult().ServiceResponse.Header.Status != 0U)
											{
												Tracer.TraceWrite("GamepadService.EnableRemapAsync: SetAdaptiveTrigger return invalid response", false);
											}
										}
										j = indexDualSense++;
									}
								}
								resetTriggerPacket = null;
							}
						}
					}
					string text7 = "GamepadService.EnableRemapAsync: ";
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 2);
					defaultInterpolatedStringHandler.AppendLiteral("AddProfile Slot ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(i);
					defaultInterpolatedStringHandler.AppendLiteral(" Id ");
					defaultInterpolatedStringHandler.AppendFormatted(CS$<>8__locals1.gamepadProfiles.ID);
					defaultInterpolatedStringHandler.AppendLiteral(" ");
					Tracer.TraceWrite(text7 + defaultInterpolatedStringHandler.ToStringAndClear() + "State " + (controllerProfileEx.Profiles[i].Enabled ? "Enabled" : "Disabled"), false);
					if (isAllControllersOffline)
					{
						controllerProfileEx.Profiles[i].VirtualType = 0U;
						controllerProfileEx.Profiles[i].VirtualFlags = 0;
						controllerProfileEx.Profiles[i].AuthControllerId = 0UL;
						controllerProfileEx.Profiles[i].GimxBaudRate = 0U;
						for (int num19 = 0; num19 < controllerProfileEx.Profiles[i].GimxPortName.Length; num19++)
						{
							controllerProfileEx.Profiles[i].GimxPortName[num19] = '\0';
						}
					}
					ushort savedExclusiveControllersMask = 0;
					if (controllerProfileEx.Profiles[i].ExclusiveControllersMask != 0)
					{
						exclusiveControllersMask |= controllerProfileEx.Profiles[i].ExclusiveControllersMask;
						savedExclusiveControllersMask = controllerProfileEx.Profiles[i].ExclusiveControllersMask;
						controllerProfileEx.Profiles[i].ExclusiveControllersMask = 0;
					}
					ServiceResponseWrapper<REWASD_ADD_PROFILE_RESPONSE> serviceResponseWrapper2 = await this._xbServiceCommunicator.AddProfile(controllerProfileEx.Profiles[i], controllerTypes, oldProfileId, flags, showGuiMessages, enableRemapBundle, enableRemapResponse, false);
					if (savedExclusiveControllersMask != 0)
					{
						controllerProfileEx.Profiles[i].ExclusiveControllersMask = savedExclusiveControllersMask;
					}
					if (serviceResponseWrapper2.ServiceResponse.Header.Status != 0U)
					{
						string text8 = "GamepadService.EnableRemapAsync: AddProfile return invalid response Status ";
						defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
						defaultInterpolatedStringHandler.AppendFormatted<uint>(serviceResponseWrapper2.ServiceResponse.Header.Status);
						Tracer.TraceWrite(text8 + defaultInterpolatedStringHandler.ToStringAndClear(), false);
						if (serviceResponseWrapper2.ServiceResponse.Header.Status != 1365U)
						{
							string text9 = "";
							if (Convert.ToBoolean((uint)(controllerProfileEx.Profiles[i].VirtualFlags & 1)))
							{
								if (serviceResponseWrapper2.ServiceResponse.Header.Status == 1244U)
								{
									text9 = "unable to send authentication response because the user has not been authenticated.";
								}
								if (serviceResponseWrapper2.ServiceResponse.Header.Status == 1167U)
								{
									text9 = "unable to send authentication response because device is not connected.";
								}
								if (serviceResponseWrapper2.ServiceResponse.Header.Status == 1223U)
								{
									text9 = "device denied response or some radio communication problem.";
								}
								if (serviceResponseWrapper2.ServiceResponse.Header.Status == 2147500037U)
								{
									text9 = "device returned a failure code during authentication.";
								}
								if (serviceResponseWrapper2.ServiceResponse.Header.Status == 5023U)
								{
									text9 = "class of device is invalid.";
								}
								if (serviceResponseWrapper2.ServiceResponse.Header.Status == 350U)
								{
									text9 = "unable to restart device, changes will take effect after reboot.";
								}
								if (serviceResponseWrapper2.ServiceResponse.Header.Status == 87U)
								{
									text9 = "service return ERROR_INVALID_PARAMETER.";
								}
							}
							if (string.IsNullOrEmpty(text9))
							{
								defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
								defaultInterpolatedStringHandler.AppendFormatted<uint>(serviceResponseWrapper2.ServiceResponse.Header.Status);
								text9 = defaultInterpolatedStringHandler.ToStringAndClear();
							}
							Tracer.TraceWrite("Failed to Apply config. Error: " + text9, false);
						}
						await this._xbServiceCommunicator.DeleteProfiles(controllerProfileEx.GetAllProfileIds());
						SlotProfilesDictionary slotProfiles6 = CS$<>8__locals1.gamepadProfiles.SlotProfiles;
						if (slotProfiles6 != null && slotProfiles6.ContainsKey(i))
						{
							defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(74, 1);
							defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: GamepadProfiles.SlotProfiles.Remove ");
							defaultInterpolatedStringHandler.AppendFormatted<Slot>(i);
							defaultInterpolatedStringHandler.AppendLiteral(" (12)");
							Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
							CS$<>8__locals1.gamepadProfiles.SlotProfiles.Remove(i);
						}
						Tracer.TraceWrite("GamepadService.EnableRemapAsync: response.ServiceResponse.Header.Status != 0 -> return 0", false);
						return 0;
					}
					if (serviceResponseWrapper2.IsResponseValid)
					{
						if (serviceResponseWrapper2.ServiceResponse.Header.Status == 0U)
						{
							controllerProfileEx.ServiceSessionIds[i] = serviceResponseWrapper2.ServiceResponse.SessionId;
							controllerProfileEx.ServiceProfileIds[i] = serviceResponseWrapper2.ServiceResponse.ProfileId;
							if (slotToSwitch == i)
							{
								profileId = serviceResponseWrapper2.ServiceResponse.ProfileId;
							}
						}
						string text10;
						if (serviceResponseWrapper2.ServiceResponse.Header.Status != 0U)
						{
							defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(63, 1);
							defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: AddProfile failed with status ");
							defaultInterpolatedStringHandler.AppendFormatted<uint>(serviceResponseWrapper2.ServiceResponse.Header.Status);
							text10 = defaultInterpolatedStringHandler.ToStringAndClear();
						}
						else
						{
							defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(95, 3);
							defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: AddProfile success Slot ");
							defaultInterpolatedStringHandler.AppendFormatted<int>(i);
							defaultInterpolatedStringHandler.AppendLiteral(" SlotsWrapperId 0x");
							defaultInterpolatedStringHandler.AppendFormatted<uint>(controllerProfileEx.SlotsWrapperId, "X");
							defaultInterpolatedStringHandler.AppendLiteral(" ServiceProfileId 0x");
							defaultInterpolatedStringHandler.AppendFormatted<ushort>(serviceResponseWrapper2.ServiceResponse.ProfileId, "X");
							text10 = defaultInterpolatedStringHandler.ToStringAndClear();
						}
						Tracer.TraceWrite(text10, false);
					}
					else
					{
						Tracer.TraceWrite("GamepadService.EnableRemapAsync: AddProfile return invalid response", false);
					}
					j = i++;
				}
				Dictionary<string, List<AssociatedControllerButton>> dictionary = new Dictionary<string, List<AssociatedControllerButton>>();
				Dictionary<string, List<AssociatedControllerButton>> dictionary2 = new Dictionary<string, List<AssociatedControllerButton>>();
				Dictionary<string, List<AssociatedControllerButton>> dictionary3 = new Dictionary<string, List<AssociatedControllerButton>>();
				if (Engine.UserSettingsService.IsOverlayEnable)
				{
					foreach (ControllerProfileInfoCollection controllerProfileInfoCollection in controllerInfos)
					{
						if (controllerProfileInfoCollection.ControllerFamily != 4)
						{
							string text11 = IControllerProfileInfoCollectionContainerExtensions.CalculateID(controllerProfileInfoCollection);
							SlotsHotkeyCollection slotsHotkeyCollection = this.GamepadsHotkeyCollection.TryGetValue(text11).Convert();
							if (slotsHotkeyCollection != null)
							{
								if (Engine.UserSettingsService.IsOverlayShowGamepadEnable && slotsHotkeyCollection.GamepadOverlayAssociatedButtonCollection != null)
								{
									dictionary.Add(text11, KeyBindingExtensions.Convert(slotsHotkeyCollection.GamepadOverlayAssociatedButtonCollection));
								}
								if (Engine.UserSettingsService.IsOverlayShowMappingsEnable && slotsHotkeyCollection.MappingOverlayAssociatedButtonCollection != null)
								{
									dictionary2.Add(text11, KeyBindingExtensions.Convert(slotsHotkeyCollection.MappingOverlayAssociatedButtonCollection));
								}
								if (Engine.UserSettingsService.IsOverlayShowMappingsEnable && slotsHotkeyCollection.MappingDescriptionsOverlayAssociatedButtonCollection != null)
								{
									dictionary3.Add(text11, KeyBindingExtensions.Convert(slotsHotkeyCollection.MappingDescriptionsOverlayAssociatedButtonCollection));
								}
							}
						}
					}
				}
				SlotsHotkeyCollection slotsHotkeyCollection2 = null;
				if (this.GamepadsHotkeyCollection.TryGetValue(CS$<>8__locals1.gamepadProfiles.ID) == null)
				{
					Tracer.TraceWrite("GamepadService.EnableRemapAsync: GamepadsHotkeyCollection return NULL for ID " + CS$<>8__locals1.gamepadProfiles.ID, false);
				}
				else
				{
					slotsHotkeyCollection2 = this.GamepadsHotkeyCollection.TryGetValue(CS$<>8__locals1.gamepadProfiles.ID).Convert();
				}
				if (!MacroCompiler.CompileSlotManagerProfile(ref controllerProfileEx, licenseInfo, controllerInfos, this.DuplicateGamepadCollection, slotsHotkeyCollection2, dictionary, dictionary2, dictionary3, Engine.UserSettingsService, fullControllersList, this.SimpleDeviceInfoList))
				{
					Tracer.TraceWrite("GamepadService.EnableRemapAsync: CompileSlotManagerProfile failed", false);
					await this._xbServiceCommunicator.DeleteProfiles(controllerProfileEx.GetAllProfileIds());
					num18 = 0;
				}
				else
				{
					ushort oldProfileId = 0;
					ushort flags = 0;
					if (oldProfileEx != null)
					{
						defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(73, 1);
						defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: replace existing SlotManagerProfileId 0x");
						defaultInterpolatedStringHandler.AppendFormatted<ushort>(oldProfileEx.SlotManagerProfileId, "X");
						Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
						oldProfileId = oldProfileEx.SlotManagerProfileId;
						flags |= 1;
					}
					if (exclusiveControllersMask != 0)
					{
						controllerProfileEx.SlotManagerProfile.ExclusiveControllersMask = exclusiveControllersMask;
					}
					REWASD_CONTROLLER_PROFILE[] array3 = controllerProfileEx.Profiles;
					for (int j = 0; j < array3.Length; j++)
					{
						REWASD_CONTROLLER_PROFILE profile = array3[j];
						if (profile.VirtualType == 2U && profile.AuthControllerId != 0UL)
						{
							List<Tuple<ulong, SimpleDeviceInfo>> simpleDeviceInfoList = this.SimpleDeviceInfoList;
							if (simpleDeviceInfoList != null && simpleDeviceInfoList.Exists((Tuple<ulong, SimpleDeviceInfo> x) => x.Item1 == profile.AuthControllerId) && REWASD_CONTROLLER_PROFILE_Extensions.IsExternalDeviceWithSerialPortPresent(profile) && !REWASD_CONTROLLER_PROFILE_Extensions.IsExternalDeviceWithBluetoothPresent(profile) && controllerProfileEx.SlotManagerProfile.Id.All((ulong x) => x != profile.AuthControllerId))
							{
								int num20 = 0;
								Predicate<Tuple<ulong, SimpleDeviceInfo>> <>9__37;
								while (num20 < controllerProfileEx.SlotManagerProfile.Id.Length)
								{
									if (controllerProfileEx.SlotManagerProfile.Id[num20] == 0UL && controllerProfileEx.SlotManagerProfile.Id[num20] == 0UL)
									{
										List<Tuple<ulong, SimpleDeviceInfo>> simpleDeviceInfoList2 = this.SimpleDeviceInfoList;
										Predicate<Tuple<ulong, SimpleDeviceInfo>> predicate;
										if ((predicate = <>9__37) == null)
										{
											Predicate<Tuple<ulong, SimpleDeviceInfo>> predicate2 = (Tuple<ulong, SimpleDeviceInfo> x) => x.Item1 == profile.AuthControllerId;
											<>9__37 = predicate2;
											predicate = predicate2;
										}
										Tuple<ulong, SimpleDeviceInfo> tuple = simpleDeviceInfoList2.Find(predicate);
										if (tuple != null)
										{
											SimpleDeviceInfo item = tuple.Item2;
											controllerProfileEx.SlotManagerProfile.Id[num20] = tuple.Item2.Id;
											controllerProfileEx.SlotManagerProfile.Type[num20] = tuple.Item2.Type;
											REWASD_CONTROLLER_PROFILE_EX rewasd_CONTROLLER_PROFILE_EX4 = controllerProfileEx;
											rewasd_CONTROLLER_PROFILE_EX4.SlotManagerProfile.ExclusiveControllersMask = rewasd_CONTROLLER_PROFILE_EX4.SlotManagerProfile.ExclusiveControllersMask | (ushort)(1 << num20);
											string text12 = "GamepadService.EnableRemapAsync: add to SlotManagerProfile exclusive controller: ";
											defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 3);
											defaultInterpolatedStringHandler.AppendLiteral("Id 0x");
											defaultInterpolatedStringHandler.AppendFormatted<ulong>(tuple.Item2.Id, "X");
											defaultInterpolatedStringHandler.AppendLiteral(", Type ");
											defaultInterpolatedStringHandler.AppendFormatted<uint>(tuple.Item2.Type);
											defaultInterpolatedStringHandler.AppendLiteral(", Bit index ");
											defaultInterpolatedStringHandler.AppendFormatted<int>(num20);
											Tracer.TraceWrite(text12 + defaultInterpolatedStringHandler.ToStringAndClear(), false);
											break;
										}
										break;
									}
									else
									{
										num20++;
									}
								}
							}
						}
					}
					if (controllerProfileEx.SlotManagerProfile.ExclusiveControllersMask != 0)
					{
						string[] array7 = new string[5];
						array7[0] = "GamepadService.EnableRemapAsync: SlotManagerProfile ExclusiveControllersMask: ";
						int num21 = 1;
						defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 1);
						defaultInterpolatedStringHandler.AppendLiteral("0x");
						defaultInterpolatedStringHandler.AppendFormatted<ushort>(controllerProfileEx.SlotManagerProfile.ExclusiveControllersMask, "X");
						defaultInterpolatedStringHandler.AppendLiteral(" ");
						array7[num21] = defaultInterpolatedStringHandler.ToStringAndClear();
						array7[2] = "[bits: ";
						array7[3] = Convert.ToString((int)controllerProfileEx.SlotManagerProfile.ExclusiveControllersMask, 2);
						array7[4] = "]";
						Tracer.TraceWrite(string.Concat(array7), false);
					}
					ServiceResponseWrapper<REWASD_ADD_PROFILE_RESPONSE> serviceResponseWrapper3 = await this._xbServiceCommunicator.AddProfile(controllerProfileEx.SlotManagerProfile, controllerTypes, oldProfileId, flags, showGuiMessages, enableRemapBundle, enableRemapResponse, false);
					if (serviceResponseWrapper3.IsResponseValid)
					{
						if (serviceResponseWrapper3.ServiceResponse.Header.Status == 0U)
						{
							controllerProfileEx.SlotManagerSessionId = serviceResponseWrapper3.ServiceResponse.SessionId;
							controllerProfileEx.SlotManagerProfileId = serviceResponseWrapper3.ServiceResponse.ProfileId;
						}
						string text13;
						if (serviceResponseWrapper3.ServiceResponse.Header.Status != 0U)
						{
							defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(84, 1);
							defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: AddProfile failed with status ");
							defaultInterpolatedStringHandler.AppendFormatted<uint>(serviceResponseWrapper3.ServiceResponse.Header.Status);
							defaultInterpolatedStringHandler.AppendLiteral(" (SlotManagerProfile)");
							text13 = defaultInterpolatedStringHandler.ToStringAndClear();
						}
						else
						{
							defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(110, 2);
							defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: AddProfile success SlotsWrapperId 0x");
							defaultInterpolatedStringHandler.AppendFormatted<uint>(controllerProfileEx.SlotsWrapperId, "X");
							defaultInterpolatedStringHandler.AppendLiteral(" ServiceProfileId 0x");
							defaultInterpolatedStringHandler.AppendFormatted<ushort>(serviceResponseWrapper3.ServiceResponse.ProfileId, "X");
							defaultInterpolatedStringHandler.AppendLiteral(" (SlotManagerProfile)");
							text13 = defaultInterpolatedStringHandler.ToStringAndClear();
						}
						Tracer.TraceWrite(text13, false);
						if (slotToSwitch < controllerProfileEx.ServiceProfileIds.Length && controllerProfileEx.ServiceProfileIds[slotToSwitch] != 0)
						{
							ushort[] array8 = new ushort[4];
							bool[] array9 = new bool[4];
							int num22 = 0;
							for (int num23 = 0; num23 < 4; num23++)
							{
								if (num23 != slotToSwitch && num22 < 4)
								{
									array8[num22] = controllerProfileEx.ServiceProfileIds[num23];
									array9[num22] = false;
									num22++;
								}
							}
							array8[3] = controllerProfileEx.ServiceProfileIds[slotToSwitch];
							array9[3] = true;
							defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 1);
							defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: switch to Slot: ");
							defaultInterpolatedStringHandler.AppendFormatted<Slot>(slotToSwitch);
							Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
							await this._xbServiceCommunicator.SetProfilesActiveState(array8, array9, showGuiMessages, enableRemapBundle, enableRemapResponse);
						}
						if (disconnectFromNintendoConsole && profileId != 0)
						{
							defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(109, 2);
							defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: try to Disconnect from External Nintendo Console! Slot: ");
							defaultInterpolatedStringHandler.AppendFormatted<Slot>(slotToSwitch);
							defaultInterpolatedStringHandler.AppendLiteral(" ServiceProfileId 0x");
							defaultInterpolatedStringHandler.AppendFormatted<ushort>(profileId, "X");
							Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
							await this.ExternalDeviceRelationsHelper.ExternalDeviceBluetoothConnectOrDisconnect(profileId, false);
							Tracer.TraceWrite("GamepadService.EnableRemapAsync: ExternalDeviceBluetoothConnectOrDisconnect exit", false);
							await Task.Delay(500);
							Tracer.TraceWrite("GamepadService.EnableRemapAsync: try to Connect to External Nintendo Console!", false);
							await this.ExternalDeviceRelationsHelper.ExternalDeviceBluetoothConnectOrDisconnect(profileId, true);
							Tracer.TraceWrite("GamepadService.EnableRemapAsync: ExternalDeviceBluetoothConnectOrDisconnect exit", false);
						}
						if (Engine.UserSettingsService.IsLedSettingsEnabled)
						{
							i = 0;
							while (i < controllerTypes.Length)
							{
								if (ControllerTypeExtensions.IsGamepad(controllerTypes[i]))
								{
									Tracer.TraceWrite("GamepadService.EnableRemapAsync: check if required to reset gamepad color", false);
									LEDSupportedDevice? ledsupportedDevice = ControllerTypeExtensions.ConvertEnumToLEDSupportedType(controllerTypes[i]);
									if (ledsupportedDevice != null)
									{
										LEDSettingsGlobalPerDevice ledsettingsGlobalPerDevice = Engine.UserSettingsService.PerDeviceGlobalLedSettings[ledsupportedDevice.Value];
										bool flag15 = ledsettingsGlobalPerDevice.IsPlayerLEDCustomIndicationAllowed && ledsettingsGlobalPerDevice.PlayerLEDEnableMode != 2;
										bool flag16 = ledsettingsGlobalPerDevice.IsPlayerLEDIndicationAllowed && ledsettingsGlobalPerDevice.PlayerLedMode != 3;
										bool isDeviceConnectedLED = ledsettingsGlobalPerDevice.IsDeviceConnectedLED;
										bool isChangeColorOnSlotAndShiftChange = ledsettingsGlobalPerDevice.IsChangeColorOnSlotAndShiftChange;
										bool isMicrophoneChangeColorOnSlotAndShiftChange = ledsettingsGlobalPerDevice.IsMicrophoneChangeColorOnSlotAndShiftChange;
										bool flag17 = flag15 || flag16 || isChangeColorOnSlotAndShiftChange || isDeviceConnectedLED || isMicrophoneChangeColorOnSlotAndShiftChange;
										bool isExclusiveAccess = ledsettingsGlobalPerDevice.IsExclusiveAccess;
										defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(50, 1);
										defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: ledSupportedType ");
										defaultInterpolatedStringHandler.AppendFormatted<LEDSupportedDevice?>(ledsupportedDevice);
										Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
										defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(51, 1);
										defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: isExclusiveAccess ");
										defaultInterpolatedStringHandler.AppendFormatted<bool>(isExclusiveAccess);
										Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
										defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(52, 1);
										defaultInterpolatedStringHandler.AppendLiteral("GamepadService.EnableRemapAsync: isAnyLedSettingsOn ");
										defaultInterpolatedStringHandler.AppendFormatted<bool>(flag17);
										Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
										if (isExclusiveAccess && !flag17)
										{
											string text14 = "GamepadService.EnableRemapAsync: gamepad reset LED request. ";
											defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 2);
											defaultInterpolatedStringHandler.AppendLiteral("Id 0x");
											defaultInterpolatedStringHandler.AppendFormatted<ulong>(CS$<>8__locals1.controllerIds[i], "X");
											defaultInterpolatedStringHandler.AppendLiteral(" / ");
											defaultInterpolatedStringHandler.AppendFormatted<ulong>(CS$<>8__locals1.controllerIds[i]);
											Tracer.TraceWrite(text14 + defaultInterpolatedStringHandler.ToStringAndClear(), false);
											await Engine.LEDService.ResetGamepadColor(new LEDDeviceInfo(CS$<>8__locals1.controllerIds[i], ControllerTypeExtensions.ConvertEnumToPhysicalType(controllerTypes[i]), 0), true, true, true, true);
										}
									}
								}
								int j = i++;
							}
						}
						if (changeGamepadSlot && existingGamepad != null)
						{
							Tracer.TraceWrite("GamepadService.EnableRemapAsync: Gamepad is found. Active slot is " + slotToSwitch.ToString(), false);
							if (existingGamepad.HasXboxElite)
							{
								await Task.Delay(1000);
							}
						}
						Tracer.TraceWrite("GamepadService.EnableRemapAsync: Remap is successfully enabled, set IsRemapToggled=true for Id: " + CS$<>8__locals1.gamepadProfiles.ID, false);
						if (profileId != 0)
						{
							if (restoreSlotsToDefaults.Count > 0)
							{
								this.BinDataSerialize.SaveGamepadProfileRelations();
							}
							if (!reenableRemap)
							{
								await this.RefreshServiceProfiles();
							}
						}
						if (this.GamepadProfileRelations.ContainsKey(CS$<>8__locals1.gamepadProfiles.ID))
						{
							this.GamepadProfileRelations[CS$<>8__locals1.gamepadProfiles.ID].IsRemapToggled = true;
						}
						if (enableRemapResponse != null)
						{
							enableRemapResponse.IsSucceded = profileId != 0 && !forceFailRemapResponse;
						}
						num18 = profileId;
					}
					else
					{
						Tracer.TraceWrite("GamepadService.EnableRemapAsync: AddProfile return invalid response (SlotManagerProfile)", false);
						await this._xbServiceCommunicator.DeleteProfiles(controllerProfileEx.GetAllProfileIds());
						num18 = 0;
					}
				}
			}
			return num18;
		}

		private EnableRemapResponseDialog Create4SlotsRequiredDlg()
		{
			EnableRemapResponseDialog enableRemapResponseDialog = new EnableRemapResponseDialog();
			enableRemapResponseDialog.Message = DTLocalization.GetString(12227);
			enableRemapResponseDialog.AddButton(new EnableRemapResponseButton
			{
				Text = DTLocalization.GetString(5004),
				ButtonAction = 0
			}, false);
			return enableRemapResponseDialog;
		}

		public async Task ReCompileSteamLizardProfile()
		{
			this.CachedSteamProfilesCollection.Clear();
			if (this.IsExclusiveCaptureProfilePresent)
			{
				List<REWASD_CONTROLLER_PROFILE_EX> list = await this._xbServiceCommunicator.GetExclusiveCaptureProfilesList();
				List<REWASD_CONTROLLER_PROFILE_EX> captureProfiles = list;
				await this.DeleteAllExclusiveCaptureProfiles();
				if (captureProfiles.Count > 0)
				{
					foreach (REWASD_CONTROLLER_PROFILE_EX rewasd_CONTROLLER_PROFILE_EX in captureProfiles.Where((REWASD_CONTROLLER_PROFILE_EX profile) => profile.Profiles[0].Type[0] == 140U || profile.Profiles[0].Type[0] == 141U))
					{
						await this.CompileExclusiveCaptureProfile(rewasd_CONTROLLER_PROFILE_EX.Profiles[0].Id[0], rewasd_CONTROLLER_PROFILE_EX.Profiles[0].Type[0], true);
					}
					IEnumerator<REWASD_CONTROLLER_PROFILE_EX> enumerator = null;
				}
			}
		}

		private Task CompileExclusiveCaptureProfile(ulong controllerId, uint type, bool enabled = true)
		{
			GamepadService.<CompileExclusiveCaptureProfile>d__270 <CompileExclusiveCaptureProfile>d__;
			<CompileExclusiveCaptureProfile>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<CompileExclusiveCaptureProfile>d__.<>4__this = this;
			<CompileExclusiveCaptureProfile>d__.controllerId = controllerId;
			<CompileExclusiveCaptureProfile>d__.type = type;
			<CompileExclusiveCaptureProfile>d__.enabled = enabled;
			<CompileExclusiveCaptureProfile>d__.<>1__state = -1;
			<CompileExclusiveCaptureProfile>d__.<>t__builder.Start<GamepadService.<CompileExclusiveCaptureProfile>d__270>(ref <CompileExclusiveCaptureProfile>d__);
			return <CompileExclusiveCaptureProfile>d__.<>t__builder.Task;
		}

		public async Task CompileHiddenAppliedConfig(ulong controllerId, uint type, bool enabled = true)
		{
			ControllerTypeEnum controllerTypeEnum = ControllerTypeExtensions.ConvertPhysicalTypeToEnum(0, type, controllerId);
			if (!ControllerTypeExtensions.IsAnyEngineController(controllerTypeEnum) || this._licensingService.IsMobileControllerFeatureUnlocked)
			{
				if (UtilsCommon.IsHiddenAppliedConfigControllerPhysicalType(type) && ControllerTypeExtensions.IsAllowedHiddenAppliedConfig(controllerTypeEnum))
				{
					REWASD_HIDDEN_APPLIED_PROFILE_CACHE? rewasd_HIDDEN_APPLIED_PROFILE_CACHE = null;
					ControllerTypeEnum[] array = new ControllerTypeEnum[15];
					array[0] = controllerTypeEnum;
					bool flag = true;
					if (this.CachedHiddenAppliedProfilesCollection.Count > 0)
					{
						rewasd_HIDDEN_APPLIED_PROFILE_CACHE = this.TryGetCachedHiddenAppliedProfile(controllerId);
						if (rewasd_HIDDEN_APPLIED_PROFILE_CACHE != null)
						{
							flag = false;
						}
						else
						{
							Tracer.TraceWrite("CompileHiddenAppliedConfig: TryGetCachedHiddenAppliedProfile return null", false);
						}
					}
					REWASD_CONTROLLER_PROFILE rewasd_CONTROLLER_PROFILE = ((!flag) ? rewasd_HIDDEN_APPLIED_PROFILE_CACHE.Value.ControllerProfile : REWASD_CONTROLLER_PROFILE.CreateBlankInstance());
					if (flag)
					{
						uint num = 0U;
						bool isSlotFeatureUnlocked = this._licensingService.IsSlotFeatureUnlocked;
						LicenseData licenseData;
						licenseData..ctor(this._licensingService.IsMacroFeatureUnlocked, this._licensingService.IsTurboFeatureUnlocked, this._licensingService.IsToggleFeatureUnlocked, this._licensingService.IsAdvancedMappingFeatureUnlocked, isSlotFeatureUnlocked, this._licensingService.IsMobileControllerFeatureUnlocked);
						Buffer.BlockCopy(MacroCompilerVirtualMouse.CalculateVirtualMouseDelta(), 0, rewasd_CONTROLLER_PROFILE.Macros, (int)MacroCompiler.CalculateMouseDeltaOffset, (int)MacroCompiler.MouseDeltaSize);
						Buffer.BlockCopy(MacroCompiler.VirtualGamepadLeftStickUpdate(), 0, rewasd_CONTROLLER_PROFILE.Macros, (int)MacroCompiler.VirtualGamepadLeftStickUpdateOffset, (int)MacroCompiler.VirtualGamepadStickUpdateSize);
						Buffer.BlockCopy(MacroCompiler.VirtualGamepadRightStickUpdate(), 0, rewasd_CONTROLLER_PROFILE.Macros, (int)MacroCompiler.VirtualGamepadRightStickUpdateOffset, (int)MacroCompiler.VirtualGamepadStickUpdateSize);
						rewasd_CONTROLLER_PROFILE.Macros[0] = 0;
						rewasd_CONTROLLER_PROFILE.Macros[1] = 1;
						rewasd_CONTROLLER_PROFILE.Macros[3] = 2;
						rewasd_CONTROLLER_PROFILE.Macros[5] = 3;
						rewasd_CONTROLLER_PROFILE.Macros[7] = 4;
						rewasd_CONTROLLER_PROFILE.Macros[8] = 0;
						Tracer.TraceWrite("CompileHiddenAppliedConfig: Before FillControllerProfileSlotWithConfig", false);
						ControllerProfileInfoCollection[] array2 = new ControllerProfileInfoCollection[1];
						ControllerFamily controllerFamily = 4;
						if (controllerTypeEnum != 503)
						{
							if (controllerTypeEnum - 504 <= 1)
							{
								controllerFamily = 2;
							}
						}
						else
						{
							controllerFamily = 1;
						}
						array2[0] = new ControllerProfileInfoCollection
						{
							ControllerFamily = controllerFamily,
							ControllerProfileInfos = new ControllerProfileInfo[1]
						};
						array2[0].ControllerProfileInfos[0] = new ControllerProfileInfo
						{
							Id = controllerId,
							ControllerType = controllerTypeEnum,
							IsPS2 = true
						};
						bool flag2 = true;
						bool flag3 = true;
						string text = "";
						bool flag5;
						bool flag6;
						bool flag7;
						bool flag8;
						uint num2;
						List<byte[]> list;
						bool flag9;
						bool flag10;
						ApplyResultStatus applyResultStatus;
						bool flag4 = GamepadService.FillControllerProfileSlotWithConfig(ref rewasd_CONTROLLER_PROFILE, ref num, 0, "Default.rewasd", out flag5, out flag6, out flag7, out flag8, ref text, out num2, out list, 55U, licenseData, out flag9, out flag10, out applyResultStatus, ref flag2, ref flag3, false, array2, this.ExternalDeviceRelationsHelper.ExternalDeviceRelationsCollection, this.SimpleDeviceInfoList, this.DuplicateGamepadCollection, null, false, "", "", null, null, false, false, true);
						Tracer.TraceWrite("CompileHiddenAppliedConfig: After FillControllerProfileSlotWithConfig", false);
						if (flag9)
						{
							Tracer.TraceWrite("CompileHiddenAppliedConfig: ShouldCancelApply", false);
							return;
						}
						if (!flag4)
						{
							if ((ulong)num2 >= (ulong)MacroCompiler.MaxRewasdMacroTableSize)
							{
								string text2 = "CompileHiddenAppliedConfig: Error! ";
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 2);
								defaultInterpolatedStringHandler.AppendLiteral("Slot macro size ");
								defaultInterpolatedStringHandler.AppendFormatted<uint>(num2);
								defaultInterpolatedStringHandler.AppendLiteral(" >= ");
								defaultInterpolatedStringHandler.AppendFormatted<long>(MacroCompiler.MaxRewasdMacroTableSize);
								Tracer.TraceWrite(text2 + defaultInterpolatedStringHandler.ToStringAndClear(), false);
							}
							if (num >= 2048U)
							{
								string text3 = "CompileHiddenAppliedConfig: Error! ";
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 2);
								defaultInterpolatedStringHandler.AppendLiteral("MacroCounter ");
								defaultInterpolatedStringHandler.AppendFormatted<uint>(num);
								defaultInterpolatedStringHandler.AppendLiteral(" >= REWASD_MAX_MACROS ");
								defaultInterpolatedStringHandler.AppendFormatted<int>(2048);
								Tracer.TraceWrite(text3 + defaultInterpolatedStringHandler.ToStringAndClear(), false);
							}
							byte[] array3 = list.SelectMany((byte[] bytes) => bytes).ToArray<byte>();
							if (array3.Length != 0 && 55L + (long)array3.Length >= MacroCompiler.MaxRewasdMacroTableSize)
							{
								Tracer.TraceWrite("CompileHiddenAppliedConfig: Error! MACRO_TABLE_SIZE " + MacroCompiler.MaxRewasdMacroTableSize.ToString() + " < TotalMacroSize " + (55L + (long)array3.Length).ToString(), false);
							}
							return;
						}
						Tracer.TraceWrite("CompileHiddenAppliedConfig: Add compiled ControllerProfile to CachedHiddenAppliedProfilesCollection", false);
						this.CachedHiddenAppliedProfilesCollection.Add(new REWASD_HIDDEN_APPLIED_PROFILE_CACHE(controllerId, controllerTypeEnum, rewasd_CONTROLLER_PROFILE));
					}
					rewasd_CONTROLLER_PROFILE.Id[0] = controllerId;
					rewasd_CONTROLLER_PROFILE.Type[0] = type;
					rewasd_CONTROLLER_PROFILE.BroadcastFlags = 0;
					rewasd_CONTROLLER_PROFILE.DeleteAfterPipeDisconnect = true;
					rewasd_CONTROLLER_PROFILE.HiddenControllersMask = 0;
					rewasd_CONTROLLER_PROFILE.Enabled = enabled;
					rewasd_CONTROLLER_PROFILE.GuiContext = 262144UL;
					await this._xbServiceCommunicator.AddProfile(rewasd_CONTROLLER_PROFILE, array, 0, 0, false, null, null, false);
					Tracer.TraceWrite("CompileHiddenAppliedConfig: config applied to service", false);
				}
			}
		}

		public async Task<bool> ApplyHoneypotProfile(SpecialProfileInfo specialProfileInfo)
		{
			Tracer.TraceWrite("ApplyHoneypotProfile", false);
			ExternalDeviceType externalDeviceType = specialProfileInfo.ExternalDeviceType;
			VirtualGamepadType virtualGamepadType = specialProfileInfo.VirtualGamepadType;
			string text = specialProfileInfo.ComPort;
			byte b = 0;
			switch (externalDeviceType)
			{
			case 0:
				b = 75;
				break;
			case 1:
				Tracer.TraceWrite("ApplyHoneypotProfile: Error! GIMX is not supported", false);
				return false;
			case 2:
				if (string.IsNullOrEmpty(text) || (!string.IsNullOrEmpty(text) && !text.ToUpper().StartsWith("COM")))
				{
					Tracer.TraceWrite("ApplyHoneypotProfile: Error! Invalid COM port for ESP32", false);
					return false;
				}
				b = 65;
				break;
			case 3:
				Tracer.TraceWrite("ApplyHoneypotProfile: Error! ESP32-S2 is not supported", false);
				return false;
			}
			bool flag;
			if (virtualGamepadType == null || virtualGamepadType == 3)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(56, 1);
				defaultInterpolatedStringHandler.AppendLiteral("ApplyHoneypotProfile: Error! Invalid VirtualGamepadType ");
				defaultInterpolatedStringHandler.AppendFormatted<VirtualGamepadType>(virtualGamepadType);
				Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
				flag = false;
			}
			else
			{
				uint num;
				switch (virtualGamepadType)
				{
				case 1:
					num = 2U;
					goto IL_167;
				case 2:
					num = 24U;
					goto IL_167;
				case 4:
					num = 48U;
					goto IL_167;
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(56, 1);
				defaultInterpolatedStringHandler.AppendLiteral("ApplyHoneypotProfile: Error! Invalid VirtualGamepadType ");
				defaultInterpolatedStringHandler.AppendFormatted<VirtualGamepadType>(virtualGamepadType);
				Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
				return false;
				IL_167:
				ControllerTypeEnum[] array = new ControllerTypeEnum[15];
				REWASD_CONTROLLER_PROFILE rewasd_CONTROLLER_PROFILE = REWASD_CONTROLLER_PROFILE.CreateBlankInstance();
				rewasd_CONTROLLER_PROFILE.VirtualType = num;
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 1);
				defaultInterpolatedStringHandler.AppendLiteral("ApplyHoneypotProfile: VirtualGamepadType = ");
				defaultInterpolatedStringHandler.AppendFormatted<VirtualGamepadType>(virtualGamepadType);
				Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 1);
				defaultInterpolatedStringHandler.AppendLiteral("ApplyHoneypotProfile: VirtualType = ");
				defaultInterpolatedStringHandler.AppendFormatted<uint>(num);
				Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
				rewasd_CONTROLLER_PROFILE.RemoteBthAddr = 279280282174469UL;
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 1);
				defaultInterpolatedStringHandler.AppendLiteral("ApplyHoneypotProfile: RemoteBthAddr = 0x");
				defaultInterpolatedStringHandler.AppendFormatted<ulong>(rewasd_CONTROLLER_PROFILE.RemoteBthAddr, "X");
				Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
				rewasd_CONTROLLER_PROFILE.VirtualFlags = b;
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 1);
				defaultInterpolatedStringHandler.AppendLiteral("ApplyHoneypotProfile: VirtualFlags = ");
				defaultInterpolatedStringHandler.AppendFormatted<byte>(rewasd_CONTROLLER_PROFILE.VirtualFlags);
				Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
				rewasd_CONTROLLER_PROFILE.GuiContext = 131072UL;
				rewasd_CONTROLLER_PROFILE.BroadcastFlags = 0;
				rewasd_CONTROLLER_PROFILE.DeleteAfterPipeDisconnect = true;
				rewasd_CONTROLLER_PROFILE.Enabled = true;
				if (externalDeviceType == 2)
				{
					text = text.ToUpper();
					char[] array2 = text.ToCharArray();
					rewasd_CONTROLLER_PROFILE.GimxBaudRate = 500000U;
					for (int i = 0; i < text.Length; i++)
					{
						rewasd_CONTROLLER_PROFILE.GimxPortName[i] = array2[i];
					}
					Tracer.TraceWrite("ApplyHoneypotProfile: ESP32 COM port = " + text, false);
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 1);
					defaultInterpolatedStringHandler.AppendLiteral("ApplyHoneypotProfile: ESP32 BaudRate = ");
					defaultInterpolatedStringHandler.AppendFormatted<uint>(rewasd_CONTROLLER_PROFILE.GimxBaudRate);
					Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
				}
				ServiceResponseWrapper<REWASD_ADD_PROFILE_RESPONSE> serviceResponseWrapper = await this._xbServiceCommunicator.AddProfile(rewasd_CONTROLLER_PROFILE, array, 0, 0, false, null, null, true);
				if (serviceResponseWrapper.IsResponseValid)
				{
					string text2;
					if (serviceResponseWrapper.ServiceResponse.Header.Status != 0U)
					{
						defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(52, 1);
						defaultInterpolatedStringHandler.AppendLiteral("ApplyHoneypotProfile: AddProfile failed with status ");
						defaultInterpolatedStringHandler.AppendFormatted<uint>(serviceResponseWrapper.ServiceResponse.Header.Status);
						text2 = defaultInterpolatedStringHandler.ToStringAndClear();
					}
					else
					{
						defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(60, 1);
						defaultInterpolatedStringHandler.AppendLiteral("ApplyHoneypotProfile: AddProfile success ServiceProfileId 0x");
						defaultInterpolatedStringHandler.AppendFormatted<ushort>(serviceResponseWrapper.ServiceResponse.ProfileId, "X");
						text2 = defaultInterpolatedStringHandler.ToStringAndClear();
					}
					Tracer.TraceWrite(text2, false);
					if (serviceResponseWrapper.ServiceResponse.Header.Status == 0U)
					{
						return true;
					}
				}
				else
				{
					Tracer.TraceWrite("ApplyHoneypotProfile: AddProfile return invalid response", false);
				}
				await this._xbServiceCommunicator.DeleteSpecialProfiles();
				flag = false;
			}
			return flag;
		}

		public Task<ushort> ApplyHardwareDonglePingProfile(SpecialProfileInfo specialProfileInfo)
		{
			GamepadService.<ApplyHardwareDonglePingProfile>d__273 <ApplyHardwareDonglePingProfile>d__;
			<ApplyHardwareDonglePingProfile>d__.<>t__builder = AsyncTaskMethodBuilder<ushort>.Create();
			<ApplyHardwareDonglePingProfile>d__.<>4__this = this;
			<ApplyHardwareDonglePingProfile>d__.specialProfileInfo = specialProfileInfo;
			<ApplyHardwareDonglePingProfile>d__.<>1__state = -1;
			<ApplyHardwareDonglePingProfile>d__.<>t__builder.Start<GamepadService.<ApplyHardwareDonglePingProfile>d__273>(ref <ApplyHardwareDonglePingProfile>d__);
			return <ApplyHardwareDonglePingProfile>d__.<>t__builder.Task;
		}

		public async Task DeleteSpecialProfiles()
		{
			Tracer.TraceWrite("DeleteSpecialProfiles", false);
			await this._xbServiceCommunicator.DeleteSpecialProfiles();
		}

		private static bool FillControllerProfileSlotWithConfig(ref REWASD_CONTROLLER_PROFILE controllerProfile, ref uint macroCounter, Slot slot, string configPath, out bool needMacroFeature, out bool needAdvancedMappingFeature, out bool needRapidFireFeature, out bool needMobileControllerFeature, ref string needFeatureAdditionalInfo, out uint totalMacroSizeError, out List<byte[]> slotMacroList, uint slotMacroIndex, LicenseData licenseData, out bool shouldCancelApply, out bool shouldReloadConfig, out ApplyResultStatus errorStatus, ref bool skipSteamExtendedWarning, ref bool skipSteamExtendedBsodWarning, bool sendStatistics = false, ControllerProfileInfoCollection[] controllerInfos = null, ObservableCollection<ExternalDeviceRelation> externalDeviceRelationsCollection = null, List<Tuple<ulong, SimpleDeviceInfo>> simpleDeviceInfoList = null, List<Tuple<ulong, uint>> duplicateGamepadCollection = null, List<REWASD_CONTROLLER_INFO> fullControllersList = null, bool showGuiMessages = true, string gameName = "", string configName = "", EnableRemapBundle enableRemapBundle = null, EnableRemapResponse enableRemapResponse = null, bool adaptiveLeftTriggerPresent = false, bool adaptiveRightTriggerPresent = false, bool virtualUsbHubPresent = true)
		{
			try
			{
				return MacroCompiler.CompileConfigToControllerProfileSlot(ref controllerProfile, ref macroCounter, slot, configPath, ref needMacroFeature, ref needAdvancedMappingFeature, ref needRapidFireFeature, ref needMobileControllerFeature, ref needFeatureAdditionalInfo, ref totalMacroSizeError, ref slotMacroList, slotMacroIndex, licenseData, ref shouldCancelApply, ref shouldReloadConfig, ref errorStatus, Engine.UserSettingsService, ref skipSteamExtendedWarning, ref skipSteamExtendedBsodWarning, sendStatistics, controllerInfos, externalDeviceRelationsCollection, simpleDeviceInfoList, duplicateGamepadCollection, fullControllersList, showGuiMessages, gameName, configName, enableRemapBundle, enableRemapResponse, adaptiveLeftTriggerPresent, adaptiveRightTriggerPresent, virtualUsbHubPresent);
			}
			catch (Exception ex)
			{
				errorStatus = 1;
				shouldCancelApply = true;
				shouldReloadConfig = false;
				needMacroFeature = false;
				needAdvancedMappingFeature = false;
				needRapidFireFeature = false;
				needMobileControllerFeature = false;
				totalMacroSizeError = 0U;
				slotMacroList = new List<byte[]>();
				if (enableRemapBundle != null)
				{
					if (enableRemapResponse == null)
					{
						enableRemapResponse = new EnableRemapResponse();
					}
					if (!enableRemapBundle.UserActions.Exists((EnableRemapButtonAction x) => x == 10))
					{
						EnableRemapResponseDialog enableRemapResponseDialog = new EnableRemapResponseDialog
						{
							Message = string.Format(DTLocalization.GetString(11134), configPath, ex.Message)
						};
						enableRemapResponseDialog.AddButton(new EnableRemapResponseButton
						{
							Text = DTLocalization.GetString(5004),
							ButtonAction = 10
						}, enableRemapBundle.IsUI);
						enableRemapResponse.AddDialog(enableRemapResponseDialog);
					}
					enableRemapResponse.DontReCallEnableRemap = true;
				}
				Tracer.TraceException(ex, "FillControllerProfileSlotWithConfig");
			}
			return false;
		}

		private REWASD_CONTROLLER_PROFILE_CACHE? TryGetCachedProfile(SlotProfilesDictionary slotProfilesDictionary, IReadOnlyList<ulong> controllerIds, IReadOnlyList<ControllerTypeEnum> controllerTypes, bool isCacheAllowed)
		{
			if (!isCacheAllowed)
			{
				REWASD_CONTROLLER_PROFILE_CACHE? rewasd_CONTROLLER_PROFILE_CACHE = null;
				return rewasd_CONTROLLER_PROFILE_CACHE;
			}
			if (this.CachedProfilesCollection.Count == 0)
			{
				REWASD_CONTROLLER_PROFILE_CACHE? rewasd_CONTROLLER_PROFILE_CACHE = null;
				return rewasd_CONTROLLER_PROFILE_CACHE;
			}
			List<CacheSlotInfo> list = new List<CacheSlotInfo>();
			foreach (object obj in Enum.GetValues(typeof(Slot)))
			{
				Slot slot = (Slot)obj;
				if (slotProfilesDictionary.ContainsKey(slot))
				{
					GamepadProfile gamepadProfile = slotProfilesDictionary[slot];
					if (((gamepadProfile != null) ? gamepadProfile.Config : null) != null)
					{
						string fileMD5Hash = XBUtils.GetFileMD5Hash(slotProfilesDictionary[slot].Config.ConfigPath);
						if (!string.IsNullOrEmpty(fileMD5Hash))
						{
							string text = "TryGetCachedProfile: ";
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 3);
							defaultInterpolatedStringHandler.AppendLiteral("Slot ");
							defaultInterpolatedStringHandler.AppendFormatted<Slot>(slot);
							defaultInterpolatedStringHandler.AppendLiteral(" MD5 ");
							defaultInterpolatedStringHandler.AppendFormatted(fileMD5Hash);
							defaultInterpolatedStringHandler.AppendLiteral(" JSON ");
							defaultInterpolatedStringHandler.AppendFormatted(slotProfilesDictionary[slot].Config.ConfigPath);
							Tracer.TraceWrite(text + defaultInterpolatedStringHandler.ToStringAndClear(), false);
							list.Add(new CacheSlotInfo(slot, slotProfilesDictionary[slot].Config.ConfigPath, fileMD5Hash));
						}
					}
				}
			}
			Func<ulong, int, bool> <>9__0;
			Func<ControllerTypeEnum, int, bool> <>9__1;
			foreach (REWASD_CONTROLLER_PROFILE_CACHE rewasd_CONTROLLER_PROFILE_CACHE2 in this.CachedProfilesCollection)
			{
				if (rewasd_CONTROLLER_PROFILE_CACHE2.ControllerIds.Length == controllerIds.Count && rewasd_CONTROLLER_PROFILE_CACHE2.ControllerTypes.Length == controllerTypes.Count && rewasd_CONTROLLER_PROFILE_CACHE2.SlotInfoList.Count == list.Count)
				{
					IEnumerable<ulong> controllerIds2 = rewasd_CONTROLLER_PROFILE_CACHE2.ControllerIds;
					Func<ulong, int, bool> func;
					if ((func = <>9__0) == null)
					{
						func = (<>9__0 = (ulong t, int i) => t != controllerIds[i]);
					}
					bool flag = controllerIds2.Where(func).Any<ulong>();
					if (!flag)
					{
						IEnumerable<ControllerTypeEnum> controllerTypes2 = rewasd_CONTROLLER_PROFILE_CACHE2.ControllerTypes;
						Func<ControllerTypeEnum, int, bool> func2;
						if ((func2 = <>9__1) == null)
						{
							func2 = (<>9__1 = (ControllerTypeEnum t, int i) => t != controllerTypes[i]);
						}
						if (controllerTypes2.Where(func2).Any<ControllerTypeEnum>())
						{
							flag = true;
						}
						if (!flag)
						{
							for (int l = 0; l < rewasd_CONTROLLER_PROFILE_CACHE2.SlotInfoList.Count; l++)
							{
								if (rewasd_CONTROLLER_PROFILE_CACHE2.SlotInfoList[l].Slot != list[l].Slot || rewasd_CONTROLLER_PROFILE_CACHE2.SlotInfoList[l].ConfigPath != list[l].ConfigPath || rewasd_CONTROLLER_PROFILE_CACHE2.SlotInfoList[l].ConfigPathMd5 != list[l].ConfigPathMd5)
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								Tracer.TraceWrite("TryGetCachedProfile: found cached profile", false);
								REWASD_CONTROLLER_PROFILE_EX controllerProfileEx = rewasd_CONTROLLER_PROFILE_CACHE2.ControllerProfileEx;
								if (((controllerProfileEx != null) ? controllerProfileEx.Profiles : null) != null)
								{
									for (int j = 0; j < rewasd_CONTROLLER_PROFILE_CACHE2.ControllerProfileEx.Profiles.Length; j++)
									{
										rewasd_CONTROLLER_PROFILE_CACHE2.ControllerProfileEx.Profiles[j].Enabled = false;
									}
								}
								REWASD_CONTROLLER_PROFILE_EX controllerProfileEx2 = rewasd_CONTROLLER_PROFILE_CACHE2.ControllerProfileEx;
								if (((controllerProfileEx2 != null) ? controllerProfileEx2.Enabled : null) != null)
								{
									for (int k = 0; k < rewasd_CONTROLLER_PROFILE_CACHE2.ControllerProfileEx.Enabled.Length; k++)
									{
										rewasd_CONTROLLER_PROFILE_CACHE2.ControllerProfileEx.Enabled[k] = false;
									}
								}
								return new REWASD_CONTROLLER_PROFILE_CACHE?(rewasd_CONTROLLER_PROFILE_CACHE2);
							}
						}
					}
				}
			}
			return null;
		}

		private REWASD_STEAM_CONTROLLER_PROFILE_CACHE? TryGetCachedSteamProfile(ulong controllerId)
		{
			if (this.CachedSteamProfilesCollection.Count == 0)
			{
				return null;
			}
			foreach (REWASD_STEAM_CONTROLLER_PROFILE_CACHE rewasd_STEAM_CONTROLLER_PROFILE_CACHE in this.CachedSteamProfilesCollection)
			{
				if (rewasd_STEAM_CONTROLLER_PROFILE_CACHE.ControllerId == controllerId && ControllerTypeExtensions.IsAnySteam(rewasd_STEAM_CONTROLLER_PROFILE_CACHE.ControllerType))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(72, 1);
					defaultInterpolatedStringHandler.AppendLiteral("TryGetCachedSteamProfile: found cached Steam profile for ControllerId 0x");
					defaultInterpolatedStringHandler.AppendFormatted<ulong>(controllerId, "X");
					Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
					return new REWASD_STEAM_CONTROLLER_PROFILE_CACHE?(rewasd_STEAM_CONTROLLER_PROFILE_CACHE);
				}
			}
			return null;
		}

		private REWASD_HIDDEN_APPLIED_PROFILE_CACHE? TryGetCachedHiddenAppliedProfile(ulong controllerId)
		{
			if (this.CachedHiddenAppliedProfilesCollection.Count == 0)
			{
				return null;
			}
			foreach (REWASD_HIDDEN_APPLIED_PROFILE_CACHE rewasd_HIDDEN_APPLIED_PROFILE_CACHE in this.CachedHiddenAppliedProfilesCollection)
			{
				if (rewasd_HIDDEN_APPLIED_PROFILE_CACHE.ControllerId == controllerId && ControllerTypeExtensions.IsAllowedHiddenAppliedConfig(rewasd_HIDDEN_APPLIED_PROFILE_CACHE.ControllerType))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(88, 1);
					defaultInterpolatedStringHandler.AppendLiteral("TryGetCachedHiddenAppliedProfile: found cached HiddenApplied profile for ControllerId 0x");
					defaultInterpolatedStringHandler.AppendFormatted<ulong>(controllerId, "X");
					Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
					return new REWASD_HIDDEN_APPLIED_PROFILE_CACHE?(rewasd_HIDDEN_APPLIED_PROFILE_CACHE);
				}
			}
			return null;
		}

		private static void UpdateSimpleDeviceInfoList(List<Tuple<ulong, SimpleDeviceInfo>> simpleDeviceInfoList, ref List<REWASD_CONTROLLER_INFO> controllerList)
		{
			if (controllerList == null)
			{
				return;
			}
			if (simpleDeviceInfoList == null)
			{
				simpleDeviceInfoList = new List<Tuple<ulong, SimpleDeviceInfo>>();
			}
			List<REWASD_CONTROLLER_INFO> list = controllerList.Where((REWASD_CONTROLLER_INFO x) => x.ConnectionType != 3).ToList<REWASD_CONTROLLER_INFO>();
			int i;
			Predicate<REWASD_CONTROLLER_INFO> <>9__2;
			int j;
			for (i = 0; i < simpleDeviceInfoList.Count; i = j + 1)
			{
				List<REWASD_CONTROLLER_INFO> list2 = list;
				Predicate<REWASD_CONTROLLER_INFO> predicate;
				if ((predicate = <>9__2) == null)
				{
					predicate = (<>9__2 = (REWASD_CONTROLLER_INFO x) => x.Id == simpleDeviceInfoList[i].Item1 && x.Type == simpleDeviceInfoList[i].Item2.Type);
				}
				if (!list2.Exists(predicate))
				{
					SimpleDeviceInfo item = simpleDeviceInfoList[i].Item2;
					item.InvalidDevice = true;
					simpleDeviceInfoList[i] = new Tuple<ulong, SimpleDeviceInfo>(simpleDeviceInfoList[i].Item1, item);
				}
				j = i;
			}
			simpleDeviceInfoList.RemoveAll((Tuple<ulong, SimpleDeviceInfo> x) => x.Item2.InvalidDevice);
			using (List<REWASD_CONTROLLER_INFO>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					REWASD_CONTROLLER_INFO controller = enumerator.Current;
					if (!simpleDeviceInfoList.Exists((Tuple<ulong, SimpleDeviceInfo> x) => x.Item1 == controller.Id && x.Item2.Type == controller.Type))
					{
						simpleDeviceInfoList.Add(new Tuple<ulong, SimpleDeviceInfo>(controller.Id, new SimpleDeviceInfo(controller.Id, controller.VendorId, controller.ProductId, controller.Type, controller.Properties, controller.ContainerId)));
					}
				}
			}
		}

		private static void UpdateControllerProfileInfoCollection(ControllerProfileInfoCollection[] controllerInfos, List<Tuple<ulong, SimpleDeviceInfo>> simpleDeviceInfoList)
		{
			if (controllerInfos == null || simpleDeviceInfoList == null)
			{
				return;
			}
			if (simpleDeviceInfoList.Count == 0)
			{
				return;
			}
			foreach (ControllerProfileInfoCollection controllerProfileInfoCollection in controllerInfos)
			{
				if (controllerProfileInfoCollection.ControllerFamily != 4)
				{
					ControllerProfileInfo[] controllerProfileInfos = controllerProfileInfoCollection.ControllerProfileInfos;
					for (int j = 0; j < controllerProfileInfos.Length; j++)
					{
						ControllerProfileInfo controllerProfileInfo = controllerProfileInfos[j];
						if (controllerProfileInfo.Id != 0UL)
						{
							Tuple<ulong, SimpleDeviceInfo> tuple = simpleDeviceInfoList.Find((Tuple<ulong, SimpleDeviceInfo> x) => x.Item1 == controllerProfileInfo.Id);
							bool flag;
							if (tuple == null)
							{
								flag = true;
							}
							else
							{
								SimpleDeviceInfo item = tuple.Item2;
								flag = false;
							}
							if (!flag && tuple.Item2.Type != 0U)
							{
								ControllerTypeEnum controllerTypeEnum = ControllerTypeExtensions.ConvertPhysicalTypeToEnum(0, tuple.Item2.Type, tuple.Item2.Id);
								if (controllerProfileInfo.ControllerType != controllerTypeEnum)
								{
									string text = "UpdateControllerProfileInfoCollection: update Type from ";
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 2);
									defaultInterpolatedStringHandler.AppendLiteral("\"");
									defaultInterpolatedStringHandler.AppendFormatted<ControllerTypeEnum>(controllerProfileInfo.ControllerType);
									defaultInterpolatedStringHandler.AppendLiteral("\" to \"");
									defaultInterpolatedStringHandler.AppendFormatted<ControllerTypeEnum>(controllerTypeEnum);
									defaultInterpolatedStringHandler.AppendLiteral("\"");
									Tracer.TraceWrite(text + defaultInterpolatedStringHandler.ToStringAndClear(), false);
								}
								controllerProfileInfo.ControllerType = controllerTypeEnum;
								controllerProfileInfo.VendorId = tuple.Item2.VendorId;
								controllerProfileInfo.ProductId = tuple.Item2.ProductId;
								controllerProfileInfo.ContainerId = tuple.Item2.ContainerId;
								controllerProfileInfo.AnalogTriggersPresent = tuple.Item2.AnalogTriggersPresent;
								controllerProfileInfo.TriggerRumbleMotorPresent = tuple.Item2.TriggerRumbleMotorPresent;
								controllerProfileInfo.MotorRumbleMotorPresent = tuple.Item2.MotorRumbleMotorPresent;
								controllerProfileInfo.AdaptiveTriggersPresent = tuple.Item2.AdaptiveTriggersPresent;
								controllerProfileInfo.GyroscopePresent = tuple.Item2.GyroscopePresent;
								controllerProfileInfo.AccelerometerPresent = tuple.Item2.AccelerometerPresent;
								controllerProfileInfo.TouchpadPresent = tuple.Item2.TouchpadPresent;
								controllerProfileInfo.RightHandDevice = tuple.Item2.RightHandDevice;
								break;
							}
						}
					}
				}
			}
		}

		public Task RefreshInputDevices()
		{
			GamepadService.<RefreshInputDevices>d__281 <RefreshInputDevices>d__;
			<RefreshInputDevices>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<RefreshInputDevices>d__.<>4__this = this;
			<RefreshInputDevices>d__.<>1__state = -1;
			<RefreshInputDevices>d__.<>t__builder.Start<GamepadService.<RefreshInputDevices>d__281>(ref <RefreshInputDevices>d__);
			return <RefreshInputDevices>d__.<>t__builder.Task;
		}

		private void TryShowWarningForAdditionalGamepads(bool showGuiMessages, bool localVirtualGamepadPresent, ulong[] controllerIds, out bool breakApply, EnableRemapBundle enableRemapBundle = null, EnableRemapResponse enableRemapResponse = null)
		{
			breakApply = false;
			bool flag = enableRemapBundle != null;
			if ((showGuiMessages || flag) && localVirtualGamepadPresent)
			{
				List<ulong> list = (from simpleDeviceInfo in this.SimpleDeviceInfoList
					where ControllerTypeExtensions.IsGamepad(ControllerTypeExtensions.ConvertPhysicalTypeToEnum(0, simpleDeviceInfo.Item2.Type, simpleDeviceInfo.Item2.Id))
					select simpleDeviceInfo.Item1).ToList<ulong>();
				list = list.Distinct<ulong>().ToList<ulong>();
				foreach (ulong num in controllerIds)
				{
					if (list.Contains(num))
					{
						list.Remove(num);
					}
				}
				if (list.Any((ulong gamepadId) => !this.ServiceProfilesCollection.ContainsProfileForID(gamepadId.ToString())) && flag && RegistryHelper.GetBool(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmUnplugPhysicalControler", true))
				{
					if (enableRemapResponse == null)
					{
						enableRemapResponse = new EnableRemapResponse();
					}
					if (!enableRemapBundle.UserActions.Exists((EnableRemapButtonAction x) => x == 2))
					{
						EnableRemapResponseDialog enableRemapResponseDialog = new EnableRemapResponseDialog
						{
							Message = DTLocalization.GetString(11865)
						};
						enableRemapResponseDialog.AddButton(new EnableRemapResponseButton
						{
							Text = DTLocalization.GetString(11824),
							ButtonAction = 2
						}, true);
						enableRemapResponseDialog.AddButton(new EnableRemapResponseButton
						{
							Text = DTLocalization.GetString(5005),
							ButtonAction = 0
						}, false);
						enableRemapResponse.AddDialog(enableRemapResponseDialog);
						breakApply = true;
					}
				}
			}
		}

		private static readonly AsyncSemaphore _gamepadServiceEnableDisableRemapSemaphore = new AsyncSemaphore(1);

		private byte _serviceVersionMajor;

		private byte _serviceVersionMinor;

		private byte _driverVersionMajor;

		private byte _driverVersionMinor;

		private ExternalDeviceRelationsHelper _externalDeviceRelationsHelper;

		private BinDataSerialize _binDataSerialize;

		private EngineControllersWpapper _engineControllersWpapper;

		private LicenseData CachedLicenseData;

		private IXBServiceCommunicator _xbServiceCommunicator;

		private IGameProfilesService _gameProfilesService;

		private IUserSettingsService _userSettingsService;

		private IEventAggregator _eventAggregator;

		private ILicensingService _licensingService;

		private IConfigFileService _configFileService;

		public static readonly AsyncSemaphore _refreshGamepadCollectionSemaphore = new AsyncSemaphore(1);

		public static readonly AsyncSemaphore _refreshServiceProfilesSemaphore = new AsyncSemaphore(1);

		private static int SentGamepadNumber = 0;

		private const ushort INIT_WAIT_TIMEOUT = 60000;

		private DispatcherTimer _refreshStatesTimer;

		[CompilerGenerated]
		private static class <>O
		{
			public static Func<uint, bool> <0>__IsHiddenAppliedConfigControllerPhysicalType;
		}
	}
}
