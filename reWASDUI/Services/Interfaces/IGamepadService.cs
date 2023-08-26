using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Media;
using DiscSoft.NET.Common.Utils.Clases;
using Prism.Commands;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Infrastructure.LED;
using reWASDUI.DataModels.CompositeDevicesCollection;
using reWASDUI.DataModels.GamepadActiveProfiles;
using reWASDUI.DataModels.GamepadSlotHotkeyCollection;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using XBEliteWPF.DataModels.PeripheralDevicesCollection;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.ExternalDevices;
using XBEliteWPF.Services;
using XBEliteWPF.Services.Interfaces;

namespace reWASDUI.Services.Interfaces
{
	public interface IGamepadService : IServiceInitedAwaitable
	{
		BaseControllerVM FindControllerBySingleId(string id);

		ObservableCollection<BaseControllerVM> ControllersAvailiableForComposition { get; }

		SortableObservableCollection<BaseControllerVM> AllPhysicalControllers { get; }

		SortableObservableCollection<BaseControllerVM> GamepadCollection { get; }

		bool IsAnyGamepadConnected { get; }

		bool IsSingleGamepadConnected { get; }

		bool IsMultipleGamepadsConnected { get; }

		bool IsCurrentGamepadRemaped { get; set; }

		bool IsAsyncRemapInProgress { get; set; }

		bool IsCurrentGamepadBackShown { get; set; }

		bool IsExclusiveCaptureControllersPresent { get; }

		bool IsExclusiveCaptureProfilePresent { get; set; }

		Drawing CurrentGamepadLeftStickSVGUri { get; }

		Drawing CurrentGamepadRightStickSVGUri { get; }

		Drawing CurrentGamepadLeftTriggerSVGUri { get; }

		Drawing CurrentGamepadRightTriggerSVGUri { get; }

		Drawing CurrentGamepadSVGUri { get; }

		Drawing CurrentGamepadFlipStateButtonSVGUri { get; }

		Drawing CurrentGamepadFlipToFaceButtonSVGUri { get; }

		Drawing CurrentGamepadFlipToBackButtonSVGUri { get; }

		DelegateCommand<bool?> CurrentGamepadFlipCommand { get; }

		BaseControllerVM CurrentGamepad { get; set; }

		GamepadProfilesCollection GamepadProfileRelations { get; }

		AutoGamesDetectionGamepadProfilesCollection AutoGamesDetectionGamepadProfileRelations { get; }

		GamepadProfiles CurrentGamepadActiveProfiles { get; }

		CompositeDevices CompositeDevices { get; set; }

		PeripheralDevices PeripheralDevices { get; set; }

		GamepadsHotkeyDictionary GamepadsHotkeyCollection { get; set; }

		GamepadsPlayerLedDictionary GamepadsUserLedCollection { get; set; }

		ObservableCollection<SlotInfo> SlotsInfo { get; }

		ObservableCollection<BlackListGamepad> BlacklistGamepads { get; set; }

		ExternalDevicesCollection ExternalDevices { get; set; }

		ObservableCollection<ExternalClient> ExternalClients { get; set; }

		ObservableCollection<GamepadSettings> GamepadsSettings { get; set; }

		ExternalDeviceRelationsHelper ExternalDeviceRelationsHelper { get; }

		BinDataSerialize BinDataSerialize { get; }

		Task<ushort> EnableRemap(bool showGUIMessages = false, string ID = null, bool remapNonToggledFromRelations = false, bool remapNonConnectedGamepad = false, bool changeGamepadSlot = true, int slotNumber = -1, bool force = false, EnableRemapBundle enableRemapBundle = null, EnableRemapResponse enableRemapResponse = null);

		Task DisableRemap(string ID = null, bool changeIsRemapToggled = true);

		Task SetCurrentGamepad(BaseControllerVM gamepad);

		Task RefreshGamepadCollection(ulong id = 0UL);

		Task RefreshGamepadCollection(string ID = null, bool refreshPromoControllers = true);

		Task RestoreDefaults(List<Slot> slots);

		Task<bool> SwitchProfileToSlot(Slot slot);

		Task ReCompileSteamLizardProfile();

		void RefreshRemapStateProperties();

		Task RefreshInputDevices();

		void RefreshPromoController();

		void OnControllerChanged(BaseControllerVM controller);

		void OnControllerDisconnected(string ControllerID, string ContainerID);

		void OnControllerConnected(BaseControllerVM controller);

		void SelectNextConnectedControllerById(string controllerId);

		Task RefreshExclusiveDeviceInfo();

		Task SelectDefaultGamepad();

		Task RefreshCurrentRemapState();
	}
}
