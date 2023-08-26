using System;
using System.Threading.Tasks;
using DiscSoft.NET.Common.ColorStuff;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon._3dPartyManufacturersAPI;
using XBEliteWPF.DataModels.GamepadActiveProfiles;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.Controller;
using XBEliteWPF.Infrastructure.LED;

namespace XBEliteWPF.Services.Interfaces
{
	public interface ILEDService
	{
		event LEDStateChangedHandler OnLEDStateChanged;

		bool IsAny3dPartyServiceRunning { get; }

		Task Stop(bool stopGamepadsOrEngineController = true, bool stopMice = true, bool stopKeyboards = true);

		void IncrementActiveConfigs();

		void DecrementActiveConfigs();

		void DeinitializeAndClose();

		Task ApplyLEDsToControllerAccordingToSettings(BaseControllerVM controller, Slot? slot = null, int shift = 0, bool forceReReadConfig = false, bool forceReReadPreferences = false, bool forceApply = false);

		Task ApplyLEDsToControllerAccordingToSettings(BaseControllerVM controller, GamepadProfile slotProfile, Slot? slot = null, int shift = 0, bool forceReReadConfig = false, bool forceReReadPreferences = false);

		void SetColor(zColor color, LEDColorMode ledColorMode = 1, int durationMS = 5000);

		void SetColor(zColor color, LEDDeviceInfo ledDeviceInfo, LEDColorMode ledColorMode = 1, int durationMS = 5000);

		void SetColor(LEDDeviceInfo ledDeviceInfo, LEDSettingsBasic ledSettingsPerCollection);

		Task ResetGamepadColor(LEDDeviceInfo ledDeviceInfo, bool resetColor = true, bool resetPlayerLed = true, bool resetBatteryIndication = true, bool force = false);

		Task ResetGamepadColor(BaseControllerVM controller, LEDSupportedDevice ledSupportedDevice, bool resetColor = true, bool resetPlayerLed = true, bool resetBatteryIndication = true);

		Task SetGamepadColor(BaseControllerVM controller, LEDSupportedDevice ledSupportedDevice, LEDSettingsBasic ledSettingsPerCollection, Slot slot, int shift);

		Task SetGamepadColor(BaseControllerVM controller, LEDSupportedDevice ledSupportedDevice, zColor color, LEDColorMode ledColorMode, int durationMS, Slot slot, int shift);

		Task SetGamepadColor(LEDSupportedDevice ledSupportedDevice, zColor color, LEDColorMode ledColorMode, int durationMS, Slot slot, int shift);

		void SetBatteryNotification(ControllerVM item);

		Task RemoveBatteryNotificationAndResume(ControllerVM item);

		Task RemoveBatteryNotification(LEDDeviceInfo deviceInfo);

		Task ChangeLedSettingsEnabled();
	}
}
