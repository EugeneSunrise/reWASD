using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Media;
using DiscSoft.NET.Common.ColorStuff;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using reWASDCommon.Infrastructure;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon._3dPartyManufacturersAPI;
using reWASDEngine;
using XBEliteWPF.DataModels;
using XBEliteWPF.DataModels.GamepadActiveProfiles;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.Controller;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Infrastructure.LED;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Services._3dPartyManufacturersAPI;
using XBEliteWPF.Utils;
using XBEliteWPF.Utils.Extensions;

namespace XBEliteWPF.Services
{
	public class LEDService : ILEDService
	{
		public event LEDStateChangedHandler OnLEDStateChanged;

		private void OnLEDChanged(ulong id, zColor color, ref bool isOnline)
		{
			LEDStateChangedHandler onLEDStateChanged = this.OnLEDStateChanged;
			if (onLEDStateChanged == null)
			{
				return;
			}
			onLEDStateChanged(id, color, ref isOnline);
		}

		public LEDService(IUserSettingsService userSettingsService)
		{
			this._userSettingsService = userSettingsService;
			this._gamepadsColorChanger = new reWASDGamepadsServiceAPI();
			this._gamepadsColorChanger.OnLEDStateChanged += this.OnLEDChanged;
			this._IsLedSettingsEnabled = this._userSettingsService.IsLedSettingsEnabled;
		}

		public bool IsAny3dPartyServiceRunning
		{
			get
			{
				return this.LedOperationsDecider.IsAnyServiceRunning();
			}
		}

		public async Task Stop(bool stopGamepadsOrEngineController = true, bool stopMice = true, bool stopKeyboards = true)
		{
			if (this._IsLedSettingsEnabled)
			{
				if (stopMice || stopKeyboards)
				{
					this.LedOperationsDecider.Stop(true, true, stopMice, stopKeyboards);
				}
				if (stopGamepadsOrEngineController)
				{
					await this._gamepadsColorChanger.Stop(true, true, true, true);
				}
			}
		}

		public async Task ChangeLedSettingsEnabled()
		{
			if (!this._userSettingsService.IsLedSettingsEnabled && this._IsLedSettingsEnabled)
			{
				await this.Stop(true, true, true);
			}
			this._IsLedSettingsEnabled = this._userSettingsService.IsLedSettingsEnabled;
		}

		public void DeinitializeAndClose()
		{
			this.LedOperationsDecider.ExitHelper();
		}

		public void IncrementActiveConfigs()
		{
			this.LedOperationsDecider.IncrementActiveConfigs();
		}

		public void DecrementActiveConfigs()
		{
			this.LedOperationsDecider.DecrementActiveConfigs();
		}

		public async Task ApplyLEDsToControllerAccordingToSettings(BaseControllerVM controller, Slot? slotNullable = null, int shift = 0, bool forceReReadConfig = false, bool forceReReadPreferences = false, bool forceApply = false)
		{
			GamepadProfiles gamepadProfiles = Engine.GamepadService.GamepadProfileRelations.FirstOrDefault((KeyValuePair<string, GamepadProfiles> kvp) => kvp.Value.ID.Contains(controller.ID)).Value;
			GamepadProfile gamepadProfile = null;
			if (gamepadProfiles != null && (gamepadProfiles.SlotProfiles.Count > 0 || forceApply))
			{
				await controller.RefreshCurrentSlot();
				gamepadProfile = gamepadProfiles.SlotProfiles.TryGetValue(controller.CurrentSlot);
			}
			await this.ApplyLEDsToControllerAccordingToSettings(controller, gamepadProfile, slotNullable, shift, forceReReadConfig, forceReReadPreferences);
		}

		public async Task ApplyLEDsToControllerAccordingToSettings(BaseControllerVM controller, GamepadProfile slotProfile, Slot? slotNullable = null, int shiftIndex = 0, bool forceReReadConfig = false, bool forceReReadPreferences = false)
		{
			if (!this._IsLedSettingsEnabled)
			{
				Tracer.TraceWriteTag(" - LEDService", "LED settings disabled", false);
			}
			else if (controller == null)
			{
				Tracer.TraceWriteTag(" - LEDService", "ApplyLEDsToControllerAccordingToSettings Controller not found - unexpected situation", false);
			}
			else if (controller.IsOnline && !controller.IsDebugController)
			{
				if (slotNullable == null)
				{
					await controller.RefreshCurrentSlot();
					slotNullable = new Slot?(controller.CurrentSlot);
				}
				Slot slot = slotNullable.Value;
				bool isControllerRemapped = Engine.GamepadService.ServiceProfilesCollection.ContainsProfileForID(controller.ID);
				bool forEmptySlot = slotProfile == null || !isControllerRemapped;
				string text = " - LEDService";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Id: ");
				defaultInterpolatedStringHandler.AppendFormatted(controller.ID);
				defaultInterpolatedStringHandler.AppendLiteral(", Type: ");
				defaultInterpolatedStringHandler.AppendFormatted<uint>(controller.Types[0]);
				Tracer.TraceWriteTag(text, defaultInterpolatedStringHandler.ToStringAndClear(), false);
				string text2 = " - LEDService";
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Slot:  ");
				defaultInterpolatedStringHandler.AppendFormatted<Slot>(slot);
				Tracer.TraceWriteTag(text2, defaultInterpolatedStringHandler.ToStringAndClear(), false);
				string text3 = " - LEDService";
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Shift: ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(shiftIndex);
				Tracer.TraceWriteTag(text3, defaultInterpolatedStringHandler.ToStringAndClear(), false);
				string text4 = " - LEDService";
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 1);
				defaultInterpolatedStringHandler.AppendLiteral("ForEmptySlot: ");
				defaultInterpolatedStringHandler.AppendFormatted<bool>(forEmptySlot);
				Tracer.TraceWriteTag(text4, defaultInterpolatedStringHandler.ToStringAndClear(), false);
				List<LEDSupportedDevice> ledsupportedDevicesEnum = controller.GetLEDSupportedDevicesEnum();
				Config curConfig = ((slotProfile != null) ? slotProfile.Config : null);
				if (forceReReadConfig)
				{
					Config config = curConfig;
					if (config != null)
					{
						config.ReadConfigFromJson(false, false);
					}
				}
				else
				{
					Config config2 = curConfig;
					if (config2 != null)
					{
						config2.ReadConfigFromJsonIfNotLoaded(false);
					}
				}
				if (forceReReadPreferences)
				{
					this._userSettingsService.Load();
				}
				foreach (LEDSupportedDevice supportedLEDDevice in ledsupportedDevicesEnum)
				{
					string text5 = " - LEDService";
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Device: ");
					defaultInterpolatedStringHandler.AppendFormatted<LEDSupportedDevice>(supportedLEDDevice);
					Tracer.TraceWriteTag(text5, defaultInterpolatedStringHandler.ToStringAndClear(), false);
					ILEDSettingsPerCollectionContainer ledSettingsContainer = null;
					LEDSettingsGlobalPerDevice perDeviceSettings = this._userSettingsService.PerDeviceGlobalLedSettings[supportedLEDDevice];
					LEDService.SetGamepadColorParams setGamepadColorParams = null;
					if (LEDSupportedDeviceExtensions.IsGamepad(supportedLEDDevice))
					{
						if (LEDSupportedDeviceExtensions.IsPlayerLEDIndicationAllowed(supportedLEDDevice) || LEDSupportedDeviceExtensions.IsPlayerLEDCustomIndicationAllowed(supportedLEDDevice))
						{
							this.SetPlayerLedIndication(controller, supportedLEDDevice, slot, shiftIndex, isControllerRemapped, slotProfile);
						}
						if (LEDSupportedDeviceExtensions.IsAllowedOnDeviceConnected(supportedLEDDevice))
						{
							if (perDeviceSettings.IsDeviceConnectedLED)
							{
								setGamepadColorParams = new LEDService.SetGamepadColorParams(controller, supportedLEDDevice, perDeviceSettings.DeviceConnectedLEDSettings, 0, 0);
							}
							else
							{
								await this.ResetGamepadColor(controller, supportedLEDDevice, true, false, false);
							}
						}
					}
					if (forEmptySlot)
					{
						bool flag = LEDSupportedDeviceExtensions.IsPlayerLEDCustomIndicationAllowed(supportedLEDDevice) && perDeviceSettings.PlayerLEDEnableMode > 0;
						Tracer.TraceWriteTag(" - LEDService", "Slot profile not found. No remap for slot reset controller", false);
						if (LEDSupportedDeviceExtensions.IsGamepad(supportedLEDDevice))
						{
							await this.ResetGamepadColor(controller, supportedLEDDevice, true, flag, false);
						}
						else
						{
							await this.Stop(false, true, true);
						}
					}
					Config config3 = curConfig;
					bool flag2;
					if (config3 == null)
					{
						flag2 = false;
					}
					else
					{
						ConfigData configData = config3.ConfigData;
						bool? flag3 = ((configData != null) ? new bool?(configData.IsConfigShouldBeApplied) : null);
						bool flag4 = true;
						flag2 = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
					}
					if (flag2)
					{
						Tracer.TraceWriteTag(" - LEDService", "Using config settings", false);
						ledSettingsContainer = curConfig.ConfigData.GetLedSettingsContainer();
					}
					else if (perDeviceSettings.IsChangeColorOnSlotAndShiftChange)
					{
						Tracer.TraceWriteTag(" - LEDService", "Using preferences settings", false);
						ledSettingsContainer = perDeviceSettings.LedSlotSettings[slot];
					}
					if (forEmptySlot || ledSettingsContainer == null)
					{
						if (setGamepadColorParams != null)
						{
							this.SetGamepadColor(setGamepadColorParams.controller, setGamepadColorParams.ledSupportedDevice, setGamepadColorParams.ledSettingsPerCollection, setGamepadColorParams.slot, setGamepadColorParams.shiftIndex);
						}
						else if (supportedLEDDevice == 1)
						{
							await this.ResetGamepadColor(controller, supportedLEDDevice, true, false, false);
						}
					}
					else
					{
						LEDSettingsPerCollection shiftSettings = ILEDSettingsPerCollectionContainerExtensions.GetShiftSettings(ledSettingsContainer, shiftIndex);
						if (LEDSupportedDeviceExtensions.IsGamepad(supportedLEDDevice))
						{
							if (LEDSupportedDeviceExtensions.IsChangeColorOnSlotAndShiftChangeAllowed(supportedLEDDevice) && perDeviceSettings.IsChangeColorOnSlotAndShiftChange)
							{
								setGamepadColorParams = new LEDService.SetGamepadColorParams(controller, supportedLEDDevice, shiftSettings, slot, shiftIndex);
							}
						}
						else
						{
							this.SetColor(new LEDDeviceInfo(supportedLEDDevice), shiftSettings);
						}
						if (setGamepadColorParams != null)
						{
							this.SetGamepadColor(setGamepadColorParams.controller, setGamepadColorParams.ledSupportedDevice, setGamepadColorParams.ledSettingsPerCollection, setGamepadColorParams.slot, setGamepadColorParams.shiftIndex);
						}
						ledSettingsContainer = null;
						perDeviceSettings = null;
						setGamepadColorParams = null;
					}
				}
				List<LEDSupportedDevice>.Enumerator enumerator = default(List<LEDSupportedDevice>.Enumerator);
			}
		}

		private void SetPlayerLedIndication(BaseControllerVM controller, LEDSupportedDevice ledSupportedDevice, Slot slot, int shift, bool isControllerRemapped, GamepadProfile slotProfile)
		{
			CompositeControllerVM compositeControllerVM = controller as CompositeControllerVM;
			if (compositeControllerVM != null)
			{
				using (List<BaseControllerVM>.Enumerator enumerator = compositeControllerVM.BaseControllers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BaseControllerVM baseControllerVM = enumerator.Current;
						this.SetPlayerLedIndication(baseControllerVM, ledSupportedDevice, slot, shift, isControllerRemapped, slotProfile);
					}
					return;
				}
			}
			ControllerVM controllerVM = controller as ControllerVM;
			if (controllerVM != null)
			{
				LEDSupportedDevice? ledsupportedDevice = ControllerTypeExtensions.ConvertEnumToLEDSupportedType(controllerVM.ControllerType);
				if (((ledsupportedDevice.GetValueOrDefault() == ledSupportedDevice) & (ledsupportedDevice != null)) && controllerVM.IsOnline && !controllerVM.IsDebugController)
				{
					byte playerLedsMask = this.GetPlayerLedsMask(controllerVM, ledSupportedDevice, slot, shift, isControllerRemapped, slotProfile);
					this.SetGamepadPlayerLED(controllerVM, playerLedsMask);
					this.SetMicrophoneLedPulsation(controllerVM, ledSupportedDevice, slot, shift);
				}
			}
		}

		private byte GetPlayerLedsMask(ControllerVM controller, LEDSupportedDevice ledSupportedDevice, Slot slot, int shift, bool isControllerRemapped, GamepadProfile slotProfile)
		{
			byte b = 0;
			LEDSettingsGlobalPerDevice ledsettingsGlobalPerDevice = this._userSettingsService.PerDeviceGlobalLedSettings[ledSupportedDevice];
			if (LEDSupportedDeviceExtensions.IsPlayerLEDIndicationAllowed(ledSupportedDevice))
			{
				b = this.GetUsualPlayerLedsMask(controller.ID, ledSupportedDevice, slot, shift, isControllerRemapped);
			}
			else if (LEDSupportedDeviceExtensions.IsPlayerLEDCustomIndicationAllowed(ledSupportedDevice) && (ledsettingsGlobalPerDevice.PlayerLEDEnableMode == null || (ledsettingsGlobalPerDevice.PlayerLEDEnableMode == 1 && slotProfile != null)))
			{
				b = LEDSupportedDeviceExtensions.GetPlayerLedIndicators(ledSupportedDevice, 0);
			}
			if (LEDSupportedDeviceExtensions.HasMicrophoneLed(ledSupportedDevice) && (slotProfile != null && isControllerRemapped))
			{
				b |= this.GetMicrophoneLedMask(ledSupportedDevice, slot, shift);
			}
			return b;
		}

		private byte GetUsualPlayerLedsMask(string controllerID, LEDSupportedDevice ledSupportedDevice, Slot slot, int shift, bool isControllerRemapped)
		{
			byte b = 0;
			LEDSettingsGlobalPerDevice ledsettingsGlobalPerDevice = this._userSettingsService.PerDeviceGlobalLedSettings[ledSupportedDevice];
			if (ledsettingsGlobalPerDevice.PlayerLedMode == null)
			{
				int num = 1;
				if (Engine.GamepadService.GamepadsUserLedCollection.ContainsKey(controllerID))
				{
					num = Engine.GamepadService.GamepadsUserLedCollection[controllerID].LedNumber;
				}
				if (num > 0)
				{
					b = LEDSupportedDeviceExtensions.GetPlayerLedIndicators(ledSupportedDevice, num - 1);
				}
			}
			else if (ledsettingsGlobalPerDevice.PlayerLedMode == 2 && isControllerRemapped)
			{
				if (shift != 0)
				{
					int num2 = (shift - 1) % 4;
					b = LEDSupportedDeviceExtensions.GetPlayerLedIndicators(ledSupportedDevice, num2);
				}
			}
			else if (ledsettingsGlobalPerDevice.PlayerLedMode == 1 && isControllerRemapped)
			{
				b = LEDSupportedDeviceExtensions.GetPlayerLedIndicators(ledSupportedDevice, slot);
			}
			return b;
		}

		private byte GetMicrophoneLedMask(LEDSupportedDevice ledSupportedDevice, Slot slot, int shift)
		{
			if (!LEDSupportedDeviceExtensions.HasMicrophoneLed(ledSupportedDevice) || !LEDSupportedDeviceExtensions.IsChangeColorOnSlotAndShiftChangeAllowed(ledSupportedDevice))
			{
				return 0;
			}
			LEDSettingsGlobalPerDevice ledsettingsGlobalPerDevice = this._userSettingsService.PerDeviceGlobalLedSettings[ledSupportedDevice];
			if (ledsettingsGlobalPerDevice.IsMicrophoneChangeColorOnSlotAndShiftChange && ILEDSettingsPerCollectionContainerExtensions.GetShiftSettings(ledsettingsGlobalPerDevice.MicrophoneLedSlotSettings[slot], shift).LEDColorMode != null)
			{
				return Constants.LEDMicrophonePlayerBit;
			}
			return 0;
		}

		public void SetColor(zColor color, LEDColorMode ledColorMode = 1, int durationMS = 5000)
		{
			if (ledColorMode == null)
			{
				ledColorMode = 1;
				color.SetColor(Colors.Black, false);
			}
			this.LedOperationsDecider.SetColor1(color, ledColorMode, durationMS);
		}

		public void SetColor(zColor color, LEDDeviceInfo ledDeviceInfo, LEDColorMode ledColorMode = 1, int durationMS = 5000)
		{
			if (ledColorMode == null)
			{
				ledColorMode = 1;
				color.SetColor(Colors.Black, false);
			}
			this.LedOperationsDecider.SetColor3(color, ledDeviceInfo, ledColorMode, durationMS, true);
		}

		public void SetColor(LEDDeviceInfo ledDeviceInfo, LEDSettingsBasic ledSettingsPerCollection)
		{
			this.SetColor(ledSettingsPerCollection.LEDColor, ledDeviceInfo, ledSettingsPerCollection.LEDColorMode, ledSettingsPerCollection.GetDurationMS());
		}

		public async Task ResetGamepadColor(BaseControllerVM controller, LEDSupportedDevice ledSupportedDevice, bool resetColor = true, bool resetPlayerLed = true, bool resetBatteryIndication = true)
		{
			CompositeControllerVM compositeControllerVM = controller as CompositeControllerVM;
			if (compositeControllerVM != null)
			{
				foreach (BaseControllerVM baseControllerVM in compositeControllerVM.BaseControllers)
				{
					await this.ResetGamepadColor(baseControllerVM, ledSupportedDevice, resetColor, resetPlayerLed, resetBatteryIndication);
				}
				List<BaseControllerVM>.Enumerator enumerator = default(List<BaseControllerVM>.Enumerator);
			}
			else
			{
				ControllerVM controllerVM = controller as ControllerVM;
				if (controllerVM != null)
				{
					LEDSupportedDevice? ledsupportedDevice = ControllerTypeExtensions.ConvertEnumToLEDSupportedType(controllerVM.ControllerType);
					if ((ledsupportedDevice.GetValueOrDefault() == ledSupportedDevice) & (ledsupportedDevice != null))
					{
						await this.ResetGamepadColor(new LEDDeviceInfo(controllerVM.ControllerId, controllerVM.Type, 0), resetColor, resetPlayerLed, resetBatteryIndication, false);
					}
				}
			}
		}

		public async Task ResetGamepadColor(LEDDeviceInfo ledDeviceInfo, bool resetColor = true, bool resetPlayerLed = true, bool resetBatteryIndication = true, bool force = false)
		{
			if (resetBatteryIndication)
			{
				await this.RemoveBatteryNotification(ledDeviceInfo);
			}
			await this._gamepadsColorChanger.Stop(ledDeviceInfo, resetColor, resetPlayerLed, force);
		}

		public void SetBatteryNotification(ControllerVM item)
		{
			Tracer.TraceWriteTag(" - LEDService", "SetBatteryNotification", false);
			string text = " - LEDService";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<ControllerTypeEnum>(item.ControllerType);
			Tracer.TraceWriteTag(text, defaultInterpolatedStringHandler.ToStringAndClear(), false);
			LEDSupportedDevice? ledsupportedDevice = ControllerTypeExtensions.ConvertEnumToLEDSupportedType(item.ControllerType);
			if (ledsupportedDevice == null)
			{
				Tracer.TraceWriteTag(" - LEDService", "Controller LED not supported", false);
				return;
			}
			if (item.ControllerBatteryLevel == 1 && !LEDSupportedDeviceExtensions.IsBatteryLowIndicationAllowed(ledsupportedDevice.Value))
			{
				Tracer.TraceWriteTag(" - LEDService", "Controller IsBatteryLowIndication not allowed", false);
				return;
			}
			if (item.ControllerBatteryLevel == null && !LEDSupportedDeviceExtensions.IsBatteryCriticalIndicationAllowed(ledsupportedDevice.Value) && !LEDSupportedDeviceExtensions.IsBatteryLowIndicationAllowed(ledsupportedDevice.Value))
			{
				Tracer.TraceWriteTag(" - LEDService", "Controller IsBatteryCriticalIndication or low not allowed", false);
				return;
			}
			string text2 = " - LEDService";
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<LEDSupportedDevice?>(ledsupportedDevice);
			Tracer.TraceWriteTag(text2, defaultInterpolatedStringHandler.ToStringAndClear(), false);
			LEDSettingsGlobalPerDevice ledsettingsGlobalPerDevice = Engine.UserSettingsService.PerDeviceGlobalLedSettings[ledsupportedDevice.Value];
			if ((item.ControllerBatteryLevel == 1 && ledsettingsGlobalPerDevice.IsBatteryLowIndication) || (item.ControllerBatteryLevel == null && (ledsettingsGlobalPerDevice.IsBatteryCriticalIndication || ledsettingsGlobalPerDevice.IsBatteryLowIndication)))
			{
				LEDService.GamepadStatusCache gamepadStatusCache = this._gamepadsCurrentSlotShift.TryGetValueEx(item.ControllerId, true);
				if (gamepadStatusCache.IsBatteryStatusSet)
				{
					this._gamepadsColorChanger.RemoveBatteryNotification(item, false);
				}
				gamepadStatusCache.IsBatteryStatusSet = true;
				this._gamepadsColorChanger.SetBatteryNotification(item, ledsettingsGlobalPerDevice);
			}
		}

		public async Task RemoveBatteryNotificationAndResume(ControllerVM item)
		{
			Tracer.TraceWriteTag(" - LEDService", "RemoveBatteryNotificationAndResume", false);
			string text = " - LEDService";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<ControllerTypeEnum>(item.ControllerType);
			Tracer.TraceWriteTag(text, defaultInterpolatedStringHandler.ToStringAndClear(), false);
			LEDService.GamepadStatusCache entry = this._gamepadsCurrentSlotShift.TryGetValueEx(item.ControllerId, false);
			LEDService.GamepadStatusCache gamepadStatusCache = entry;
			if (gamepadStatusCache != null && gamepadStatusCache.IsBatteryStatusSet)
			{
				Tracer.TraceWriteTag(" - LEDService", "Battery was set removing.", false);
				entry.IsBatteryStatusSet = false;
				LEDSupportedDevice? ledsupportedDevice = ControllerTypeExtensions.ConvertEnumToLEDSupportedType(item.ControllerType);
				LEDSettingsGlobalPerDevice ledsettingsGlobalPerDevice = Engine.UserSettingsService.PerDeviceGlobalLedSettings[ledsupportedDevice.Value];
				await this._gamepadsColorChanger.RemoveBatteryNotification(item, false);
				await Engine.LEDService.ApplyLEDsToControllerAccordingToSettings(item, new Slot?(entry.CurrentSlot), entry.CurrentShift, false, false, true);
			}
		}

		public async Task RemoveBatteryNotification(LEDDeviceInfo deviceInfo)
		{
			Tracer.TraceWriteTag(" - LEDService", "RemoveBatteryNotification", false);
			string text = " - LEDService";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<uint>(deviceInfo.DeviceType);
			Tracer.TraceWriteTag(text, defaultInterpolatedStringHandler.ToStringAndClear(), false);
			LEDService.GamepadStatusCache gamepadStatusCache = this._gamepadsCurrentSlotShift.TryGetValueEx(deviceInfo.DeviceID, false);
			if (gamepadStatusCache != null && gamepadStatusCache.IsBatteryStatusSet)
			{
				Tracer.TraceWriteTag(" - LEDService", "Battery was set removing.", false);
				gamepadStatusCache.IsBatteryStatusSet = false;
				await this._gamepadsColorChanger.RemoveBatteryNotification(deviceInfo, true);
			}
		}

		public async Task SetGamepadColor(BaseControllerVM controller, LEDSupportedDevice ledSupportedDevice, LEDSettingsBasic ledSettingsPerCollection, Slot slot, int shift)
		{
			await this.SetGamepadColor(controller, ledSupportedDevice, ledSettingsPerCollection.LEDColor, ledSettingsPerCollection.LEDColorMode, ledSettingsPerCollection.GetDurationMS(), slot, shift);
		}

		public async Task SetGamepadColor(LEDSupportedDevice ledSupportedDevice, zColor color, LEDColorMode ledColorMode, int durationMS, Slot slot, int shift)
		{
			List<BaseControllerVM> list = Engine.GamepadService.AllPhysicalControllers.Where(delegate(BaseControllerVM b)
			{
				ControllerVM controllerVM = b as ControllerVM;
				if (controllerVM != null)
				{
					LEDSupportedDevice? ledsupportedDevice = ControllerTypeExtensions.ConvertEnumToLEDSupportedType(controllerVM.ControllerType);
					LEDSupportedDevice ledSupportedDevice2 = ledSupportedDevice;
					return (ledsupportedDevice.GetValueOrDefault() == ledSupportedDevice2) & (ledsupportedDevice != null);
				}
				return false;
			}).ToList<BaseControllerVM>();
			foreach (BaseControllerVM baseControllerVM in list)
			{
				await this.SetGamepadColor(baseControllerVM, ledSupportedDevice, color, ledColorMode, durationMS, slot, shift);
			}
			List<BaseControllerVM>.Enumerator enumerator = default(List<BaseControllerVM>.Enumerator);
		}

		public async Task SetGamepadColor(BaseControllerVM controller, LEDSupportedDevice ledSupportedDevice, zColor color, LEDColorMode ledColorMode, int durationMS, Slot slot, int shift)
		{
			Tracer.TraceWriteTag(" - LEDService", "SetGamepadColor", false);
			if (ledColorMode == null)
			{
				ledColorMode = 1;
				color.SetColor(Colors.Black, false);
			}
			CompositeControllerVM compositeControllerVM = controller as CompositeControllerVM;
			if (compositeControllerVM != null)
			{
				foreach (BaseControllerVM baseControllerVM in compositeControllerVM.BaseControllers)
				{
					PeripheralVM peripheralVM = baseControllerVM as PeripheralVM;
					if (peripheralVM != null && peripheralVM.BaseControllers[0].HasAnyEngineControllers)
					{
						await this.SetGamepadColor(peripheralVM.BaseControllers[0], ledSupportedDevice, color, ledColorMode, durationMS, slot, shift);
					}
					else
					{
						await this.SetGamepadColor(baseControllerVM, ledSupportedDevice, color, ledColorMode, durationMS, slot, shift);
					}
				}
				List<BaseControllerVM>.Enumerator enumerator = default(List<BaseControllerVM>.Enumerator);
			}
			else
			{
				ControllerVM c = controller as ControllerVM;
				if (c != null)
				{
					LEDSupportedDevice? ledsupportedDevice = ControllerTypeExtensions.ConvertEnumToLEDSupportedType(c.ControllerType);
					if (((ledsupportedDevice.GetValueOrDefault() == ledSupportedDevice) & (ledsupportedDevice != null)) && controller.IsOnline && !controller.IsDebugController)
					{
						LEDService.GamepadStatusCache gamepadStatusCache = this._gamepadsCurrentSlotShift.TryGetValueEx(c.ControllerId, true);
						gamepadStatusCache.CurrentSlot = slot;
						gamepadStatusCache.CurrentShift = shift;
						if (c.MaxLedValue == 0)
						{
							Tracer.TraceWriteTag(" - LEDService", "MaxLedValue == 0", false);
						}
						else
						{
							if (c.IsMonochromeLed)
							{
								Tracer.TraceWriteTag(" - LEDService", "Device.IsMonochromeLed", false);
								string text = " - LEDService";
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 4);
								defaultInterpolatedStringHandler.AppendLiteral("Color before adjustment: A-");
								defaultInterpolatedStringHandler.AppendFormatted<byte>(color.A);
								defaultInterpolatedStringHandler.AppendLiteral(" R-");
								defaultInterpolatedStringHandler.AppendFormatted<byte>(color.R);
								defaultInterpolatedStringHandler.AppendLiteral(" G-");
								defaultInterpolatedStringHandler.AppendFormatted<byte>(color.G);
								defaultInterpolatedStringHandler.AppendLiteral(" B-");
								defaultInterpolatedStringHandler.AppendFormatted<byte>(color.B);
								Tracer.TraceWriteTag(text, defaultInterpolatedStringHandler.ToStringAndClear(), false);
								zColor zColor;
								if (!(color.GetColor() == Colors.Black))
								{
									(zColor = new zColor()).R = color.A;
								}
								else
								{
									(zColor = new zColor()).R = 0;
								}
								color = zColor;
								string text2 = " - LEDService";
								defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 4);
								defaultInterpolatedStringHandler.AppendLiteral("Color after adjustment: A-");
								defaultInterpolatedStringHandler.AppendFormatted<byte>(color.A);
								defaultInterpolatedStringHandler.AppendLiteral(" R-");
								defaultInterpolatedStringHandler.AppendFormatted<byte>(color.R);
								defaultInterpolatedStringHandler.AppendLiteral(" G-");
								defaultInterpolatedStringHandler.AppendFormatted<byte>(color.G);
								defaultInterpolatedStringHandler.AppendLiteral(" B-");
								defaultInterpolatedStringHandler.AppendFormatted<byte>(color.B);
								Tracer.TraceWriteTag(text2, defaultInterpolatedStringHandler.ToStringAndClear(), false);
							}
							List<Tuple<ulong, uint>> list = Engine.GamepadService.DuplicateGamepadCollection.FindAll((Tuple<ulong, uint> x) => x.Item1 == c.ControllerId);
							if (list.Count > 0)
							{
								foreach (Tuple<ulong, uint> tuple in list)
								{
									ulong num;
									uint num2;
									tuple.Deconstruct(out num, out num2);
									ulong num3 = num;
									uint num4 = num2;
									if (c.ControllerId != num3)
									{
										this._gamepadsColorChanger.SetColor(color, new LEDDeviceInfo(num3, num4, ledSupportedDevice), ledColorMode, durationMS, true);
									}
								}
							}
							this._gamepadsColorChanger.SetColor(color, new LEDDeviceInfo(c.ControllerId, c.Type, 0), ledColorMode, durationMS, true);
						}
					}
				}
			}
		}

		private void SetGamepadPlayerLED(ControllerVM controller, byte ledsMask)
		{
			string text = " - LEDService";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 1);
			defaultInterpolatedStringHandler.AppendLiteral("SetGamepadPlayerLED ledsMask:");
			defaultInterpolatedStringHandler.AppendFormatted<byte>(ledsMask);
			Tracer.TraceWriteTag(text, defaultInterpolatedStringHandler.ToStringAndClear(), false);
			List<Tuple<ulong, uint>> list = Engine.GamepadService.DuplicateGamepadCollection.FindAll((Tuple<ulong, uint> x) => x.Item1 == controller.ControllerId && x.Item2 != controller.Type);
			if (list.Count > 0)
			{
				using (List<Tuple<ulong, uint>>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Tuple<ulong, uint> tuple = enumerator.Current;
						ulong num;
						uint num2;
						tuple.Deconstruct(out num, out num2);
						ulong num3 = num;
						uint num4 = num2;
						this._gamepadsColorChanger.SetGamepadPlayerLed(num3, num4, ledsMask, false);
					}
					return;
				}
			}
			this._gamepadsColorChanger.SetGamepadPlayerLed(controller.ControllerId, controller.Type, ledsMask, false);
		}

		private void SetMicrophoneLedPulsation(ControllerVM controller, LEDSupportedDevice ledSupportedDevice, Slot slot, int shift)
		{
			if (!LEDSupportedDeviceExtensions.HasMicrophoneLed(ledSupportedDevice) || !LEDSupportedDeviceExtensions.IsChangeColorOnSlotAndShiftChangeAllowed(ledSupportedDevice))
			{
				return;
			}
			bool flag = false;
			LEDSettingsGlobalPerDevice ledsettingsGlobalPerDevice = this._userSettingsService.PerDeviceGlobalLedSettings[ledSupportedDevice];
			if (ledsettingsGlobalPerDevice.IsMicrophoneChangeColorOnSlotAndShiftChange && ILEDSettingsPerCollectionContainerExtensions.GetShiftSettings(ledsettingsGlobalPerDevice.MicrophoneLedSlotSettings[slot], shift).LEDColorMode == 5)
			{
				flag = true;
			}
			string text = " - LEDService";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 2);
			defaultInterpolatedStringHandler.AppendLiteral("SetMicrophoneLedPulsation ");
			defaultInterpolatedStringHandler.AppendFormatted<LEDSupportedDevice>(ledSupportedDevice);
			defaultInterpolatedStringHandler.AppendLiteral(" pulsation:");
			defaultInterpolatedStringHandler.AppendFormatted<bool>(flag);
			Tracer.TraceWriteTag(text, defaultInterpolatedStringHandler.ToStringAndClear(), false);
			List<Tuple<ulong, uint>> list = Engine.GamepadService.DuplicateGamepadCollection.FindAll((Tuple<ulong, uint> x) => x.Item1 == controller.ControllerId && x.Item2 != controller.Type);
			if (list.Count > 0)
			{
				using (List<Tuple<ulong, uint>>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Tuple<ulong, uint> tuple = enumerator.Current;
						ulong num;
						uint num2;
						tuple.Deconstruct(out num, out num2);
						ulong num3 = num;
						uint num4 = num2;
						this._gamepadsColorChanger.SetMicrophoneLedPulsation(num3, num4, flag);
					}
					return;
				}
			}
			this._gamepadsColorChanger.SetMicrophoneLedPulsation(controller.ControllerId, controller.Type, flag);
		}

		private reWASDGamepadsServiceAPI _gamepadsColorChanger;

		private LedOperationsDecider LedOperationsDecider = new LedOperationsDecider();

		private IUserSettingsService _userSettingsService;

		private bool _IsLedSettingsEnabled;

		private Dictionary<ulong, LEDService.GamepadStatusCache> _gamepadsCurrentSlotShift = new Dictionary<ulong, LEDService.GamepadStatusCache>();

		private class SetGamepadColorParams
		{
			public SetGamepadColorParams(BaseControllerVM controller, LEDSupportedDevice ledSupportedDevice, LEDSettingsBasic ledSettingsPerCollection, Slot slot, int shiftIndex)
			{
				this.controller = controller;
				this.ledSupportedDevice = ledSupportedDevice;
				this.ledSettingsPerCollection = ledSettingsPerCollection;
				this.slot = slot;
				this.shiftIndex = shiftIndex;
			}

			public BaseControllerVM controller;

			public LEDSupportedDevice ledSupportedDevice;

			public LEDSettingsBasic ledSettingsPerCollection;

			public Slot slot;

			public int shiftIndex;
		}

		private class GamepadStatusCache
		{
			public bool IsBatteryStatusSet { get; set; }

			public Slot CurrentSlot { get; set; }

			public int CurrentShift { get; set; }
		}
	}
}
