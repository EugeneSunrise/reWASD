using System;
using XBEliteWPF.Infrastructure;

namespace reWASDEngine.Services.Interfaces
{
	public interface IEventsProxy
	{
		event ControllerAddedHandler OnControllerAdded;

		event ControllerChangedHandler OnControllerChanged;

		event ControllerRemovedHandler OnControllerRemoved;

		event AllControllersRemovedHandler OnAllControllersRemoved;

		event RemapStateChangedHandler OnRemapStateChanged;

		event ConfigAppliedToSlotHandler OnConfigAppliedToSlot;

		event ControllerSlotChangedHandler OnSlotChanged;

		event ShiftChangedHandler OnShiftChanged;

		event BatteryLevelChangedHandler OnBatteryLevelChanged;

		event GamepadStateChanged OnGamepadStateChanged;

		event ControllerAddedHandler OnControllerAddedUI;

		event ControllerRemovedHandlerUI OnControllerRemovedUI;

		event AllControllersRemovedHandler OnAllControllersRemovedUI;

		event RemapOffUIHandler OnRemapOffUI;

		event ConfigAppliedToSlotHandler OnConfigAppliedToSlotUI;

		event ControllerSlotChangedHandler OnSlotChangedUI;

		event ShiftShowHandlerUI ShiftShowUI;

		event ShiftHideHandlerUI ShiftHideUI;

		event OffileDeviceHandle OffileDevice;

		event ShiftHideHandlerUI ShiftHideOverlayUI;

		event BatteryLevelChangedHandler OnBatteryLevelChangedUI;

		event OverlayMenuHandler OverlayMenu;
	}
}
