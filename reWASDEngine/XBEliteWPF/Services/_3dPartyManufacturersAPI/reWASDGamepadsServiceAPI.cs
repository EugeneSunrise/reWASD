using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Media;
using DiscSoft.NET.Common.ColorStuff;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Utils;
using reWASDCommon._3dPartyManufacturersAPI;
using reWASDEngine;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.Controller;
using XBEliteWPF.Infrastructure.LED;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils.Extensions;

namespace XBEliteWPF.Services._3dPartyManufacturersAPI
{
	public class reWASDGamepadsServiceAPI : BasicDeviceColorChanger
	{
		public event LEDStateChangedHandler OnLEDStateChanged;

		public override async Task Stop(bool resetColor = true, bool resetPlayerLed = true, bool stopMice = true, bool stopKeyboards = true)
		{
			foreach (ColorOrchestrator colorOrchestrator in this._colorOrchestratorsDictionary.Values)
			{
				colorOrchestrator.Stop();
			}
			if (resetColor)
			{
				zColor blackColor = new zColor(Colors.Black);
				foreach (KeyValuePair<ulong, uint> keyValuePair in this._devicesThatHaveColorSet)
				{
					await this.SetGamepadRGBLed(keyValuePair.Key, keyValuePair.Value, blackColor, false);
				}
				Dictionary<ulong, uint>.Enumerator enumerator2 = default(Dictionary<ulong, uint>.Enumerator);
				this._devicesThatHaveColorSet.Clear();
				blackColor = null;
			}
			if (resetPlayerLed)
			{
				IXBServiceCommunicator xbc = Engine.XBServiceCommunicator;
				foreach (KeyValuePair<ulong, uint> keyValuePair2 in this._devicesThatHavePlayerLEDSet)
				{
					await xbc.SetGamepadPlayerLed(keyValuePair2.Key, keyValuePair2.Value, 0, 0, 0U, false, false);
				}
				Dictionary<ulong, uint>.Enumerator enumerator2 = default(Dictionary<ulong, uint>.Enumerator);
				this._devicesThatHavePlayerLEDSet.Clear();
				xbc = null;
			}
		}

		public override async Task Stop(LEDDeviceInfo ledDeviceInfo, bool resetColor = true, bool resetPlayerLed = true, bool force = false)
		{
			ColorOrchestrator colorOrchestrator = this._colorOrchestratorsDictionary.TryGetValueEx(ledDeviceInfo.DeviceID, false);
			if (colorOrchestrator != null)
			{
				colorOrchestrator.Stop();
			}
			if (resetColor)
			{
				await this.SetGamepadRGBLed(ledDeviceInfo.DeviceID, ledDeviceInfo.DeviceType, new zColor(Colors.Black), force);
			}
			if (resetPlayerLed)
			{
				await Engine.XBServiceCommunicator.SetGamepadPlayerLed(ledDeviceInfo.DeviceID, ledDeviceInfo.DeviceType, 0, 0, 0U, false, force);
			}
		}

		public override async Task<bool> SetColor(Color color, LEDDeviceInfo ledDeviceInfo, LEDColorMode ledColorMode = 1, int durationMS = 5000, bool isAllowColorSolidOrchestratorRedirect = true)
		{
			return await this.SetColor(new zColor(color), ledDeviceInfo, ledColorMode, durationMS, isAllowColorSolidOrchestratorRedirect);
		}

		public override async Task<bool> SetColor(zColor color, LEDDeviceInfo ledDeviceInfo, LEDColorMode ledColorMode = 1, int durationMS = 5000, bool isAllowColorSolidOrchestratorRedirect = true)
		{
			bool flag;
			if (ledColorMode == 1 && !isAllowColorSolidOrchestratorRedirect)
			{
				flag = await this.SetColorInternal(ledDeviceInfo.DeviceID, ledDeviceInfo.DeviceType, color);
			}
			else
			{
				flag = await this.GetColorOrchestratorForDevice(ledDeviceInfo).SetColor(color.GetColor(), ledDeviceInfo, ledColorMode, durationMS, 10, false, null);
			}
			return flag;
		}

		public override async Task<bool> SetPlayerLed(LEDDeviceInfo ledDeviceInfo, byte leds)
		{
			return await this.SetGamepadPlayerLed(ledDeviceInfo.DeviceID, ledDeviceInfo.DeviceType, leds, false);
		}

		public async Task<bool> SetBatteryNotification(ControllerVM item, LEDSettingsGlobalPerDevice settings)
		{
			reWASDGamepadsServiceAPI.<>c__DisplayClass11_0 CS$<>8__locals1 = new reWASDGamepadsServiceAPI.<>c__DisplayClass11_0();
			CS$<>8__locals1.item = item;
			LEDSettingsBasic ledsettingsBasic = settings.BatteryLowLEDSettings;
			if (CS$<>8__locals1.item.ControllerBatteryLevel == null && settings.IsBatteryCriticalIndication)
			{
				ledsettingsBasic = settings.BatteryCriticalLEDSettings;
			}
			zColor zColor = ledsettingsBasic.LEDColor;
			if (CS$<>8__locals1.item.IsMonochromeLed)
			{
				zColor = new zColor
				{
					R = zColor.A
				};
			}
			bool flag;
			if (settings.IsBatteryColorIndicationAllowed)
			{
				flag = await this.GetColorOrchestratorForDevice(CS$<>8__locals1.item.ControllerId).SetColor(zColor.GetColor(), new LEDDeviceInfo(CS$<>8__locals1.item.ControllerId, CS$<>8__locals1.item.Type, 0), ledsettingsBasic.LEDColorMode, ledsettingsBasic.GetDurationMS(), 10, true, delegate
				{
					reWASDGamepadsServiceAPI.<>c__DisplayClass11_0.<<SetBatteryNotification>b__1>d <<SetBatteryNotification>b__1>d;
					<<SetBatteryNotification>b__1>d.<>t__builder = AsyncVoidMethodBuilder.Create();
					<<SetBatteryNotification>b__1>d.<>4__this = CS$<>8__locals1;
					<<SetBatteryNotification>b__1>d.<>1__state = -1;
					<<SetBatteryNotification>b__1>d.<>t__builder.Start<reWASDGamepadsServiceAPI.<>c__DisplayClass11_0.<<SetBatteryNotification>b__1>d>(ref <<SetBatteryNotification>b__1>d);
				});
			}
			else
			{
				await this.GetColorOrchestratorForDevice(CS$<>8__locals1.item.ControllerId).SetPlayerLedBlinking(new LEDDeviceInfo(CS$<>8__locals1.item.ControllerId, CS$<>8__locals1.item.Type, 0), (CS$<>8__locals1.item.ControllerBatteryLevel == null) ? 40 : 100, ledsettingsBasic.GetDurationMS(), LEDSupportedDeviceExtensions.GetBlinkMode(ledsettingsBasic.Device), 10, true, delegate
				{
					reWASDGamepadsServiceAPI.<>c__DisplayClass11_0.<<SetBatteryNotification>b__0>d <<SetBatteryNotification>b__0>d;
					<<SetBatteryNotification>b__0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
					<<SetBatteryNotification>b__0>d.<>4__this = CS$<>8__locals1;
					<<SetBatteryNotification>b__0>d.<>1__state = -1;
					<<SetBatteryNotification>b__0>d.<>t__builder.Start<reWASDGamepadsServiceAPI.<>c__DisplayClass11_0.<<SetBatteryNotification>b__0>d>(ref <<SetBatteryNotification>b__0>d);
				});
				flag = true;
			}
			return flag;
		}

		public async Task RemoveBatteryNotification(ControllerVM item, bool stopDeviceColorChanger)
		{
			await this.RemoveBatteryNotification(new LEDDeviceInfo(item.ControllerId, item.Type, 0), stopDeviceColorChanger);
		}

		public async Task RemoveBatteryNotification(LEDDeviceInfo deviceInfo, bool stopDeviceColorChanger)
		{
			await this.Stop(deviceInfo, stopDeviceColorChanger, false, false);
		}

		private async Task<bool> SetColorInternal(ulong deviceId, uint deviceType, zColor color)
		{
			if (!this._devicesThatHaveColorSet.ContainsKey(deviceId))
			{
				this._devicesThatHaveColorSet.Add(deviceId, deviceType);
			}
			bool flag = color.GetColor() == Colors.Black;
			return await this.SetGamepadRGBLed(deviceId, deviceType, color, flag);
		}

		private async Task<bool> SetGamepadRGBLed(ulong deviceId, uint deviceType, zColor color, bool force = false)
		{
			bool flag2;
			if (deviceType == 268435455U)
			{
				bool flag = true;
				LEDStateChangedHandler onLEDStateChanged = this.OnLEDStateChanged;
				if (onLEDStateChanged != null)
				{
					onLEDStateChanged(deviceId, color, ref flag);
				}
				flag2 = flag;
			}
			else if (deviceType == 268435453U || deviceType == 268435454U)
			{
				bool flag3 = true;
				ulong engineControllerIdByInternalId = UtilsCommon.GetEngineControllerIdByInternalId(deviceType, deviceId);
				if (engineControllerIdByInternalId != 0UL)
				{
					LEDStateChangedHandler onLEDStateChanged2 = this.OnLEDStateChanged;
					if (onLEDStateChanged2 != null)
					{
						onLEDStateChanged2(engineControllerIdByInternalId, color, ref flag3);
					}
				}
				flag2 = flag3;
			}
			else
			{
				flag2 = await Engine.XBServiceCommunicator.SetGamepadRGBLed(deviceId, deviceType, color.R, color.G, color.B, 0, 0U, false, force);
			}
			return flag2;
		}

		public async Task SetMicrophoneLedPulsation(ulong deviceId, uint deviceType, bool pulsation)
		{
			await Engine.XBServiceCommunicator.SetDualSenseMicLedOptions(deviceId, deviceType, pulsation);
		}

		public async Task<bool> SetGamepadPlayerLed(ulong deviceId, uint deviceType, byte leds, bool force = false)
		{
			IXBServiceCommunicator xbserviceCommunicator = Engine.XBServiceCommunicator;
			if (!this._devicesThatHavePlayerLEDSet.ContainsKey(deviceId))
			{
				this._devicesThatHavePlayerLEDSet.Add(deviceId, deviceType);
			}
			return await xbserviceCommunicator.SetGamepadPlayerLed(deviceId, deviceType, leds, 0, 0U, false, force);
		}

		private ColorOrchestrator GetColorOrchestratorForDevice(LEDDeviceInfo ledDeviceInfo)
		{
			return this.GetColorOrchestratorForDevice(ledDeviceInfo.DeviceID);
		}

		private ColorOrchestrator GetColorOrchestratorForDevice(ulong deviceID)
		{
			if (!this._colorOrchestratorsDictionary.ContainsKey(deviceID))
			{
				this._colorOrchestratorsDictionary.Add(deviceID, new ColorOrchestrator(this));
			}
			return this._colorOrchestratorsDictionary[deviceID];
		}

		protected Dictionary<ulong, ColorOrchestrator> _colorOrchestratorsDictionary = new Dictionary<ulong, ColorOrchestrator>();

		protected Dictionary<ulong, uint> _devicesThatHaveColorSet = new Dictionary<ulong, uint>();

		protected Dictionary<ulong, uint> _devicesThatHavePlayerLEDSet = new Dictionary<ulong, uint>();
	}
}
