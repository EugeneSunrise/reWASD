using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Clases;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Infrastructure.LED;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using XBEliteWPF.DataModels.CompositeDevicesCollection;
using XBEliteWPF.DataModels.GamepadActiveProfiles;
using XBEliteWPF.DataModels.GamepadSlotHotkeyCollection;
using XBEliteWPF.DataModels.InitializedDevicesCollection;
using XBEliteWPF.DataModels.PeripheralDevicesCollection;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.Controller;
using XBEliteWPF.Infrastructure.ExternalDeviceRelations;
using XBEliteWPF.Infrastructure.ExternalDevices;

namespace XBEliteWPF.Services.Interfaces
{
	public interface IGamepadService : IServiceInitedAwaitable
	{
		BaseControllerVM FindControllerBySingleId(string id);

		SortableObservableCollection<BaseControllerVM> AllPhysicalControllers { get; }

		ObservableCollection<BaseControllerVM> GamepadCollection { get; }

		List<Tuple<ulong, uint>> DuplicateGamepadCollection { get; }

		ObservableCollection<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> ServiceProfilesCollection { get; }

		List<Tuple<ulong, SimpleDeviceInfo>> SimpleDeviceInfoList { get; }

		ObservableCollection<ControllerVM> VirtualGamepadCollection { get; }

		bool IsAnyGamepadConnected { get; }

		bool IsSingleGamepadConnected { get; }

		bool IsMultipleGamepadsConnected { get; }

		bool IsAnyGamepadRemaped { get; }

		bool IsAsyncRemapInProgress { get; set; }

		bool IsExclusiveCaptureControllersPresent { get; }

		bool IsExclusiveCaptureProfilePresent { get; set; }

		GamepadProfilesCollection GamepadProfileRelations { get; }

		AutoGamesDetectionGamepadProfilesCollection AutoGamesDetectionGamepadProfileRelations { get; set; }

		CompositeDevices CompositeDevices { get; set; }

		PeripheralDevices PeripheralDevices { get; set; }

		InitializedDevices InitializedDevices { get; set; }

		GamepadsHotkeyDictionary GamepadsHotkeyCollection { get; set; }

		GamepadsPlayerLedDictionary GamepadsUserLedCollection { get; set; }

		ObservableCollection<SlotInfo> SlotsInfo { get; }

		ObservableCollection<BlackListGamepad> BlacklistGamepads { get; set; }

		ExternalDevicesCollection ExternalDevices { get; set; }

		ObservableCollection<ExternalClient> ExternalClients { get; set; }

		ObservableCollection<GamepadSettings> GamepadsSettings { get; set; }

		ObservableCollection<REWASD_CONTROLLER_PROFILE_CACHE> CachedProfilesCollection { get; }

		ExternalDeviceRelationsHelper ExternalDeviceRelationsHelper { get; }

		BinDataSerialize BinDataSerialize { get; }

		EngineControllersWpapper EngineControllersWpapper { get; }

		event GamepadStateChanged OnGamepadStateChanged;

		event BatteryLevelChangedHandler OnBatteryLevelChanged;

		event BatteryLevelChangedHandler OnBatteryLevelChangedUI;

		event ConfigAppliedToSlotHandler OnConfigAppliedToSlot;

		event RemapStateChangedHandler OnRemapStateChanged;

		event ControllerAddedHandler OnControllerAdded;

		event ControllerChangedHandler OnControllerChanged;

		event ControllerRemovedHandlerForProxy OnControllerRemovedForProxy;

		event AllControllersRemovedHandler OnAllControllersRemoved;

		event ControllerSlotChangedHandler OnPhysicalSlotChanged;

		event SetBatteryNotificationEventHandler SetBatteryNotificationEvent;

		Task<ushort> EnableRemap(bool showGUIMessages = false, string ID = null, bool remapNonToggledFromRelations = false, bool remapNonConnectedGamepad = false, bool changeGamepadSlot = true, int slotNumber = -1, bool force = false, bool reenableRemap = false, bool checkLicense = true, EnableRemapBundle enableRemapBundle = null, EnableRemapResponse enableRemapResponse = null);

		Task DisableRemap(string ID = null, bool changeIsRemapToggled = true);

		Task RefreshGamepadCollection(ulong id = 0UL);

		Task RefreshGamepadBattery(ulong id);

		Task RefreshServiceProfiles();

		void PublishRemapStateChanged(BaseControllerVM controller, RemapState state);

		Task RestoreDefaults(string ID, List<Slot> slots);

		void RemoveRelationsForConfigFile(string configPath);

		uint GetSlotsWrapperIdForGamepadId(string gamepadId);

		Task<bool> SwitchProfileToSlot(BaseControllerVM controller, Slot slot);

		REWASD_CONTROLLER_PROFILE_EX GetProfileEx(ulong controllerID);

		REWASD_CONTROLLER_PROFILE_EX GetProfileEx(string ID);

		Task DeleteAllExclusiveCaptureProfiles();

		Task ProcessExclusiveCaptureProfile(string gamepadID, bool processDelete = true, bool processAdd = true);

		Task ReCompileSteamLizardProfile();

		Task CompileHiddenAppliedConfig(ulong controllerId, uint type, bool enabled = true);

		Task RefreshHiddenAppliedConfig();

		Task RefreshInputDevices();

		bool IsAnyServiceProfileWithExternalBluetooth();

		bool IsAnyServiceProfileWithExternalBluetoothAdapter();

		bool IsAnyServiceProfileWithExternalSerialPort(string externalDeviceTypeId = "");

		bool IsAnyServiceProfileWithExternalDevice(ExternalDeviceType externalDeviceType);

		bool IsAnyServiceProfileWithExternalDevice(string externalDeviceTypeId);

		List<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> GetServiceProfileWithExternalBluetooth();

		List<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> GetServiceProfileWithExternalBluetoothAdapter();

		List<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> GetServiceProfileWithExternalSerialPort();

		List<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> GetServiceProfileWithExternalDevice(ExternalDeviceType externalDeviceType);

		REWASD_CONTROLLER_PROFILE_EX GetProfileExByServiceProfileId(ulong serviceProfileId);

		bool IsGamepadNotPresent(ulong id);

		bool IsGamepadOnline(ulong id);

		bool IsGamepadInNothingAppliedState(BaseControllerVM gamepad);

		bool IsSlotValid(Slot slot, bool isSlotFeatureUnlocked, ControllerTypeEnum[] controllerTypes);

		RemapState GetRemapState(string gamepadId);

		VirtualGamepadType? GetVirtualGamepadType(BaseControllerVM controller);

		void UpdateDeviceFriendlyName(string ID);

		Task ControllerChangeMasterAddress(ulong gamepadID, uint gamepadType, ulong bluetoothAddress);

		void InitializePeripheralDevice(PeripheralVM peripheral, PeripheralPhysicalType physicalType);

		Task ReinitializeDevice(string controllerId);

		void InitializeDevice(BaseControllerVM controller, string deviceType);

		void SendGamepadChanged(BaseControllerVM controller);

		Task<bool> ApplyHoneypotProfile(SpecialProfileInfo honeypotInfo);

		Task<ushort> ApplyHardwareDonglePingProfile(SpecialProfileInfo honeypotInfo);

		Task DeleteSpecialProfiles();

		void SetEngineBatteryState(ControllerVM controller, byte batteryLevel, BatteryChargingState chargingState);
	}
}
