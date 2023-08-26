using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoftReWASDServiceNamespace;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Prism.Events;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Infrastructure.SupportedControllers;
using reWASDCommon.Infrastructure.SupportedControllers.Base;
using reWASDCommon.Utils;
using reWASDEngine;
using XBEliteWPF.DataModels.InitializedDevicesCollection;
using XBEliteWPF.Infrastructure.XBEliteService;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtilModel;

namespace XBEliteWPF.Infrastructure.Controller
{
	public class ControllerVM : BaseControllerVM
	{
		[JsonProperty("ControllerInfoString")]
		public string ControllerInfoString
		{
			get
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 3);
				defaultInterpolatedStringHandler.AppendLiteral("Type: ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(this.ControllerType);
				defaultInterpolatedStringHandler.AppendLiteral(" VendorID: 0x");
				defaultInterpolatedStringHandler.AppendFormatted<ushort>(this._controllerInfo.VendorId, "X");
				defaultInterpolatedStringHandler.AppendLiteral(" ProductID: 0x");
				defaultInterpolatedStringHandler.AppendFormatted<ushort>(this._controllerInfo.ProductId, "X");
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}
		}

		[JsonProperty("UnknownControllerInfoString")]
		public string UnknownControllerInfoString
		{
			get
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Unsupported device VID: 0x");
				defaultInterpolatedStringHandler.AppendFormatted<ushort>(this._controllerInfo.VendorId, "X");
				defaultInterpolatedStringHandler.AppendLiteral(" PID: 0x");
				defaultInterpolatedStringHandler.AppendFormatted<ushort>(this._controllerInfo.ProductId, "X");
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}
		}

		public override Drawing MiniGamepadSVGIco
		{
			get
			{
				return this.GetDrawingForController();
			}
		}

		public bool IsUnmapAvailable
		{
			get
			{
				return this.IsXboxElite || this.IsXboxOne || this.IsAnySteam || this.IsXboxOneX;
			}
		}

		public bool IsExtendedMappingAvailable
		{
			get
			{
				return this.IsXboxElite || this.IsXboxOne || this.IsAnySteam || this.IsXboxOneX;
			}
		}

		public ControllerVM(REWASD_CONTROLLER_INFO controllerInfo, bool isDebugController = false)
			: base(isDebugController)
		{
			this._controllerInfo = REWASD_CONTROLLER_INFO.CreateBlankInstance();
			this._controllerState = default(REWASD_GET_CONTROLLER_STATE);
			if (controllerInfo.Id != 0UL)
			{
				this.UpdateControllerInfo(controllerInfo);
				base.FillFriendlyName();
			}
		}

		protected override async void OnOnlineChanged()
		{
			if (base.IsOnline && this.IsXboxGameSirG7)
			{
				bool? isInitialized = this.IsInitialized;
				bool flag = true;
				if (((isInitialized.GetValueOrDefault() == flag) & (isInitialized != null)) && this.IsChangeGamesirG7Mode)
				{
					await Task.Delay(1200);
					await Engine.XBServiceCommunicator.SwitchControllerToHidMode(this.ControllerId, this.Type);
				}
			}
		}

		protected override void Vibrate()
		{
			if (!base.IsDebugController && Engine.UserSettingsService.SendRumbleToGamepad && base.IsOnline && (this.IsMotorRumbleMotorPresent || this.IsTriggerRumbleMotorPresent))
			{
				Engine.XBServiceCommunicator.SendControllerVibration(this.ControllerId, this.Type, 300, 50, 0);
			}
		}

		public override bool UpdateControllerInfo(REWASD_CONTROLLER_INFO controllerInfo)
		{
			bool flag = this._controllerInfo.Properties != controllerInfo.Properties || this.MasterBthAddr != controllerInfo.MasterBthAddr;
			if (!this.IsEngineController)
			{
				this._controllerInfo = controllerInfo;
			}
			else
			{
				byte batteryLevel = this._controllerInfo.BatteryLevel;
				byte batteryChargingState = this._controllerInfo.BatteryChargingState;
				this._controllerInfo = controllerInfo;
				this._controllerInfo.BatteryChargingState = batteryChargingState;
				this._controllerInfo.BatteryLevel = batteryLevel;
			}
			this._description = DSUtils.ConvertCharArrayToStringUntilNullSymbol(this._controllerInfo.Description);
			this.ContainerDescription = DSUtils.ConvertCharArrayToStringUntilNullSymbol(this._controllerInfo.ContainerDescription);
			this.ManufacturerName = DSUtils.ConvertCharArrayToStringUntilNullSymbol(this._controllerInfo.ManufacturerName);
			this.ProductName = DSUtils.ConvertCharArrayToStringUntilNullSymbol(this._controllerInfo.ProductName);
			if (this.IsChatpad)
			{
				this.ContainerDescription = this._description;
			}
			if (this._isInitialized == null)
			{
				if (ControllerTypeExtensions.IsAnySteam(this.ControllerType) || ControllerTypeExtensions.IsAnyAzeron(this.ControllerType) || ControllerTypeExtensions.IsFlydigi(this.ControllerType) || this.IsXboxGameSirG7)
				{
					this._isInitialized = new bool?(true);
				}
				if (this.ControllerType == 12 || this.ControllerType == 15 || this.IsCanBluetoothPaired || this.IsFlydigiXbox360 || this.ControllerType == 50 || this.ControllerType == 18 || this.ControllerType == 25 || this.ControllerType == 26)
				{
					this._isInitialized = new bool?(false);
				}
			}
			base.RefreshCurrentSlot();
			if (this.IsGyroscopePresent)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
				defaultInterpolatedStringHandler.AppendFormatted("Calibration");
				defaultInterpolatedStringHandler.AppendLiteral("\\");
				defaultInterpolatedStringHandler.AppendFormatted<ulong>(this.ControllerId);
				bool @bool = RegistryHelper.GetBool(defaultInterpolatedStringHandler.ToStringAndClear(), "IsAutoCalibration", true);
				if (this.IsControllerGyroAutomaticCalibrationDisabled != !@bool)
				{
					string text = (@bool ? "\"Auto\"" : "\"Manual\"");
					string text2 = ((!this.IsControllerGyroAutomaticCalibrationDisabled) ? "\"Auto\"" : "\"Manual\"");
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(77, 3);
					defaultInterpolatedStringHandler.AppendLiteral("Gyroscope calibration mode: current service status is ");
					defaultInterpolatedStringHandler.AppendFormatted(text2);
					defaultInterpolatedStringHandler.AppendLiteral(" for ControllerId ");
					defaultInterpolatedStringHandler.AppendFormatted<ulong>(this.ControllerId);
					defaultInterpolatedStringHandler.AppendLiteral(" / 0x");
					defaultInterpolatedStringHandler.AppendFormatted<ulong>(this.ControllerId, "X");
					Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(73, 3);
					defaultInterpolatedStringHandler.AppendLiteral("Gyroscope calibration mode: set service status to ");
					defaultInterpolatedStringHandler.AppendFormatted(text);
					defaultInterpolatedStringHandler.AppendLiteral(" for ControllerId ");
					defaultInterpolatedStringHandler.AppendFormatted<ulong>(this.ControllerId);
					defaultInterpolatedStringHandler.AppendLiteral(" / 0x");
					defaultInterpolatedStringHandler.AppendFormatted<ulong>(this.ControllerId, "X");
					Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
					Engine.XBServiceCommunicator.ControllerUpdateGyroCalibrationMode(this.ControllerId, this.Type);
				}
			}
			if (!this.IsXimNexus && (this.ControllerType == 1 || (this.ControllerType == 2 && this.ProductId == 736 && this.IsBluetoothConnectionFlagPresent)))
			{
				this.CheckXimNexusController(this.VendorId, this.ProductId, this.ControllerId, this.ContainerDescription, this.IsBluetoothConnectionFlagPresent);
			}
			this.TraceControllerInfo(" - Controllers");
			return flag;
		}

		public override void UpdateControllerState(REWASD_GET_CONTROLLER_STATE_RESPONSE controllerState)
		{
			this._controllerState.Buttons = controllerState.State.Gamepad.Buttons[0];
			this._controllerState.LeftTrigger = controllerState.State.Gamepad.LeftTrigger;
			this._controllerState.RightTrigger = controllerState.State.Gamepad.RightTrigger;
			this._controllerState.LeftThumbstickX = controllerState.State.Gamepad.LeftStickX;
			this._controllerState.LeftThumbstickY = controllerState.State.Gamepad.LeftStickY;
			this._controllerState.RightThumbstickX = controllerState.State.Gamepad.RightStickX;
			this._controllerState.RightThumbstickY = controllerState.State.Gamepad.RightStickY;
			if (!base.IsDebugController && !this.IsEngineController)
			{
				this._controllerInfo.BatteryChargingState = controllerState.BatteryChargingState;
				this._controllerInfo.BatteryLevel = controllerState.BatteryLevel;
			}
			this._controllerInfo.GyroXCalibrationDelta = controllerState.GyroXCalibrationDelta;
			this._controllerInfo.GyroYCalibrationDelta = controllerState.GyroYCalibrationDelta;
			this._controllerInfo.GyroZCalibrationDelta = controllerState.GyroZCalibrationDelta;
		}

		public override ControllerVM CurrentController
		{
			get
			{
				return this;
			}
			set
			{
			}
		}

		public override string ID
		{
			get
			{
				return this.ControllerId.ToString();
			}
			set
			{
			}
		}

		public override string ShortID
		{
			get
			{
				return this.ID;
			}
		}

		public override string HintID
		{
			get
			{
				return "ID: " + this.ID;
			}
		}

		[JsonProperty("ControllerId")]
		public ulong ControllerId
		{
			get
			{
				return this._controllerInfo.Id;
			}
			set
			{
				this._controllerInfo.Id = value;
			}
		}

		public override uint[] Types
		{
			get
			{
				uint[] array = new uint[15];
				array[0] = this._controllerInfo.Type;
				return array;
			}
			set
			{
				this._controllerInfo.Type = value[0];
			}
		}

		public override ulong[] Ids
		{
			get
			{
				ulong[] array = new ulong[15];
				array[0] = this._controllerInfo.Id;
				return array;
			}
			set
			{
				this._controllerInfo.Id = value[0];
			}
		}

		[JsonProperty("Type")]
		public uint Type
		{
			get
			{
				return this._controllerInfo.Type;
			}
		}

		[JsonProperty("Description")]
		public string Description { get; set; }

		[JsonProperty("ContainerDescription")]
		public string ContainerDescription { get; set; }

		[JsonProperty("ManufacturerName")]
		public string ManufacturerName { get; set; }

		[JsonProperty("ProductName")]
		public string ProductName { get; set; }

		public override Guid ContainerId
		{
			get
			{
				return this._controllerInfo.ContainerId;
			}
			set
			{
				this._controllerInfo.ContainerId = value;
			}
		}

		[JsonProperty("ControllerConnectionType")]
		private byte ControllerConnectionType
		{
			get
			{
				return this._controllerInfo.ConnectionType;
			}
			set
			{
				this._controllerInfo.ConnectionType = value;
			}
		}

		[JsonProperty("VendorId")]
		public ushort VendorId
		{
			get
			{
				return this._controllerInfo.VendorId;
			}
			set
			{
				this._controllerInfo.VendorId = value;
			}
		}

		public bool IsVendorIdPS2
		{
			get
			{
				return this.VendorId == 65534;
			}
		}

		[JsonProperty("ProductId")]
		public ushort ProductId
		{
			get
			{
				return this._controllerInfo.ProductId;
			}
			set
			{
				this._controllerInfo.ProductId = value;
			}
		}

		[JsonProperty("Version")]
		public ushort Version
		{
			get
			{
				return this._controllerInfo.Version;
			}
			set
			{
				this._controllerInfo.Version = value;
			}
		}

		public string FirmwareVersion
		{
			get
			{
				return string.Format("{0}.{1}.{2}.{3}", new object[] { this.FirmwareVersionMajor, this.FirmwareVersionMinor, this.FirmwareVersionBuild, this.FirmwareVersionRevision });
			}
		}

		[JsonProperty("FirmwareVersionMajor")]
		public ushort FirmwareVersionMajor
		{
			get
			{
				return this._controllerInfo.FirmwareVersionMajor;
			}
			set
			{
				this._controllerInfo.FirmwareVersionMajor = value;
			}
		}

		[JsonProperty("FirmwareVersionMinor")]
		public ushort FirmwareVersionMinor
		{
			get
			{
				return this._controllerInfo.FirmwareVersionMinor;
			}
			set
			{
				this._controllerInfo.FirmwareVersionMinor = value;
			}
		}

		[JsonProperty("FirmwareVersionBuild")]
		public ushort FirmwareVersionBuild
		{
			get
			{
				return this._controllerInfo.FirmwareVersionBuild;
			}
			set
			{
				this._controllerInfo.FirmwareVersionBuild = value;
			}
		}

		[JsonProperty("FirmwareVersionRevision")]
		public ushort FirmwareVersionRevision
		{
			get
			{
				return this._controllerInfo.FirmwareVersionRevision;
			}
			set
			{
				this._controllerInfo.FirmwareVersionRevision = value;
			}
		}

		[JsonProperty("Properties")]
		public uint Properties
		{
			get
			{
				return this._controllerInfo.Properties;
			}
			set
			{
				this._controllerInfo.Properties = value;
			}
		}

		public override bool IsBluetoothConnectionFlagPresent
		{
			get
			{
				return REWASD_CONTROLLER_INFO_Extensions.IsBluetoothConnection(this._controllerInfo);
			}
		}

		public override bool IsAnalogTriggersPresent
		{
			get
			{
				return REWASD_CONTROLLER_INFO_Extensions.IsAnalogTriggersPresent(this._controllerInfo);
			}
		}

		public override bool IsTriggersFullPullPresent
		{
			get
			{
				return this.IsAnySteam || this.IsGameCube;
			}
		}

		public override bool IsMotorRumbleMotorPresent
		{
			get
			{
				return REWASD_CONTROLLER_INFO_Extensions.IsMotorRumbleMotorPresent(this._controllerInfo);
			}
		}

		public override bool IsTriggerRumbleMotorPresent
		{
			get
			{
				return REWASD_CONTROLLER_INFO_Extensions.IsTriggerRumbleMotorPresent(this._controllerInfo) && !ControllerTypeExtensions.IsRazerWolverine2(this.ControllerType);
			}
		}

		public override bool IsAdaptiveTriggersPresent
		{
			get
			{
				return REWASD_CONTROLLER_INFO_Extensions.IsAdaptiveTriggersPresent(this._controllerInfo);
			}
		}

		public override bool IsSteamExtendedPresent
		{
			get
			{
				return REWASD_CONTROLLER_INFO_Extensions.IsSteamExtendedPresent(this._controllerInfo);
			}
		}

		public override bool IsAccelerometerPresent
		{
			get
			{
				return REWASD_CONTROLLER_INFO_Extensions.IsAccelerometerPresent(this._controllerInfo);
			}
		}

		public override bool IsGyroscopePresent
		{
			get
			{
				return REWASD_CONTROLLER_INFO_Extensions.IsGyroscopePresent(this._controllerInfo);
			}
		}

		public override bool IsTouchpadPresent
		{
			get
			{
				return REWASD_CONTROLLER_INFO_Extensions.IsTouchpadPresent(this._controllerInfo);
			}
		}

		public override bool IsTiltMode2
		{
			get
			{
				return REWASD_CONTROLLER_INFO_Extensions.IsTiltMode2(this._controllerInfo);
			}
		}

		public override bool IsControllerExclusiveAccessSupported
		{
			get
			{
				return REWASD_CONTROLLER_INFO_Extensions.IsControllerExclusiveAccessSupported(this._controllerInfo);
			}
		}

		[JsonProperty("IsRightHandDevice")]
		public override bool IsRightHandDevice
		{
			get
			{
				return REWASD_CONTROLLER_INFO_Extensions.IsRightHandDevice(this._controllerInfo);
			}
		}

		public override bool IsPowerManagementPresent
		{
			get
			{
				return this.IsPowerManagementEnabled;
			}
		}

		[JsonProperty("IsPowerManagementEnabled")]
		public bool IsPowerManagementEnabled
		{
			get
			{
				return REWASD_CONTROLLER_INFO_Extensions.IsPowerManagementEnabled(this._controllerInfo);
			}
		}

		[JsonProperty("IsPowerDownSupported")]
		public bool IsPowerDownSupported
		{
			get
			{
				return REWASD_CONTROLLER_INFO_Extensions.IsPowerDownSupported(this._controllerInfo);
			}
		}

		[JsonProperty("IsControllerPropertyChatpad")]
		public bool IsControllerPropertyChatpad
		{
			get
			{
				return REWASD_CONTROLLER_INFO_Extensions.IsControllerPropertyChatpad(this._controllerInfo);
			}
		}

		[JsonProperty("IsControllerPhysicalOutputAndDebug")]
		public bool IsControllerPhysicalOutputAndDebug
		{
			get
			{
				return REWASD_CONTROLLER_INFO_Extensions.IsControllerPhysicalOutputAndDebug(this._controllerInfo);
			}
		}

		[JsonProperty("IsControllerSupportHardwareGyroCalibration")]
		public bool IsControllerSupportHardwareGyroCalibration
		{
			get
			{
				return REWASD_CONTROLLER_INFO_Extensions.IsControllerSupportHardwareGyroCalibration(this._controllerInfo);
			}
		}

		[JsonProperty("IsControllerGyroAutomaticCalibrationDisabled")]
		public bool IsControllerGyroAutomaticCalibrationDisabled
		{
			get
			{
				return REWASD_CONTROLLER_INFO_Extensions.IsControllerGyroAutomaticCalibrationDisabled(this._controllerInfo);
			}
		}

		public bool IsCharging
		{
			get
			{
				return this.ControllerBatteryChargingState == 1;
			}
		}

		public bool IsConnectionWired
		{
			get
			{
				return !this.IsEngineGamepad && this.ControllerConnectionType == 1;
			}
		}

		public bool IsConnectionWireless
		{
			get
			{
				return this.IsEngineGamepad || this.ControllerConnectionType == 2;
			}
		}

		public bool IsConnectionVirtual
		{
			get
			{
				return this.ControllerConnectionType == 3;
			}
		}

		[JsonProperty("IsBatteryLevelPercentPresent")]
		public bool IsBatteryLevelPercentPresent
		{
			get
			{
				return Convert.ToBoolean((int)(this._controllerInfo.BatteryLevel & 128));
			}
		}

		[JsonProperty("BatteryLevelPercent")]
		public byte BatteryLevelPercent
		{
			get
			{
				if (!this.IsBatteryLevelPercentPresent)
				{
					return 0;
				}
				return this.GetBatteryPercents();
			}
		}

		[JsonProperty("IsMonochromeLed")]
		public bool IsMonochromeLed
		{
			get
			{
				return !this.HasAnyEngineControllers && this.IsLedPresent && REWASD_CONTROLLER_INFO_Extensions.IsMonochromeLed(this._controllerInfo);
			}
		}

		[JsonProperty("IsMasterAddressPresent")]
		public bool IsMasterAddressPresent
		{
			get
			{
				return REWASD_CONTROLLER_INFO_Extensions.IsMasterAddressPresent(this._controllerInfo);
			}
		}

		[JsonProperty("MasterBthAddr")]
		public ulong MasterBthAddr
		{
			get
			{
				return this._controllerInfo.MasterBthAddr;
			}
		}

		public bool IsLedPresent
		{
			get
			{
				return this.MaxLedValue > 0;
			}
		}

		[JsonProperty("MaxLedValue")]
		public byte MaxLedValue
		{
			get
			{
				if (!ControllerTypeExtensions.IsAnyEngineController(this.ControllerType))
				{
					return this._controllerInfo.MaxLedValue;
				}
				return byte.MaxValue;
			}
		}

		public bool IsUserLedsPresent
		{
			get
			{
				return this.NumUserLeds > 0;
			}
		}

		[JsonProperty("NumUserLeds")]
		public byte NumUserLeds
		{
			get
			{
				return this._controllerInfo.NumUserLeds;
			}
		}

		public override bool HasLEDGamepad
		{
			get
			{
				LEDSupportedDevice? ledsupportedDevice;
				return ControllerTypeExtensions.IsGamepad(this.ControllerType) && ControllerTypeExtensions.ConvertEnumToLEDSupportedType(this.ControllerType) != null && LEDSupportedDeviceExtensions.IsChangeColorOnSlotAndShiftChangeAllowed(ledsupportedDevice.GetValueOrDefault());
			}
		}

		public override bool HasLEDKeyboard
		{
			get
			{
				return this.IsKeyboard && this.IsLedPresent;
			}
		}

		public override bool HasLEDMouse
		{
			get
			{
				return this.IsMouse && this.IsLedPresent;
			}
		}

		public override bool HasXboxElite
		{
			get
			{
				return this.IsXboxElite || this.IsXboxElite2;
			}
		}

		public override bool HasXboxElite2
		{
			get
			{
				return this.IsXboxElite2;
			}
		}

		public override bool HasExclusiveCaptureControllers
		{
			get
			{
				return this.IsAnySteam || this.IsAnyAzeron || ControllerTypeExtensions.IsFlydigi(this.ControllerType);
			}
		}

		public override bool HasTouchpad
		{
			get
			{
				return this.IsAnySteam || this.IsSonyDualshock4 || this.IsAnySonyDualSense;
			}
		}

		public override bool HasAnyEngineControllers
		{
			get
			{
				return ControllerTypeExtensions.IsAnyEngineController(this.ControllerType);
			}
		}

		public override bool HasGamepadControllers
		{
			get
			{
				return ControllerTypeExtensions.IsGamepad(this.ControllerType);
			}
		}

		public override bool HasGamepadControllersWithFictiveButtons
		{
			get
			{
				return ControllerTypeExtensions.IsGamepad(this.ControllerType) && !ControllerTypeExtensions.IsFictiveButtonsNotAllowed(this.ControllerType);
			}
		}

		public override bool HasGamepadVibrateControllers
		{
			get
			{
				return ControllerTypeExtensions.IsGamepad(this.ControllerType) && !ControllerTypeExtensions.IsAnyAzeron(this.ControllerType);
			}
		}

		public override bool HasOnlineGamepadVibrateControllers
		{
			get
			{
				return !this.HasExclusiveCaptureControllers && this.IsMotorRumbleMotorPresent && base.IsOnline;
			}
		}

		public override bool HasGamepadControllersWithBuiltInAnyKeypad
		{
			get
			{
				return ControllerTypeExtensions.IsGamepadWithBuiltInAnyKeypad(this.ControllerType);
			}
		}

		public bool IsAnyKeyboard
		{
			get
			{
				return this.ControllerType == 1000 || this.ControllerType == 1002 || this.ControllerType == 1003 || this.ControllerType == 503;
			}
		}

		public bool IsKeyboard
		{
			get
			{
				return this.ControllerType == 1000 || this.ControllerType == 503;
			}
		}

		public bool IsEngineKeyboard
		{
			get
			{
				return this.ControllerType == 503;
			}
		}

		public bool IsChatpad
		{
			get
			{
				return this.ControllerType == 1000 && this.IsControllerPropertyChatpad;
			}
		}

		public bool IsMouse
		{
			get
			{
				return this.ControllerType == 1001 || this.ControllerType == 504 || this.ControllerType == 505;
			}
		}

		public bool IsAnyEngineMouse
		{
			get
			{
				return this.ControllerType == 504 || this.ControllerType == 505;
			}
		}

		public bool IsEngineMouse
		{
			get
			{
				return this.ControllerType == 504;
			}
		}

		public bool IsEngineMouseTouchpad
		{
			get
			{
				return this.ControllerType == 505;
			}
		}

		public bool IsKeyboardPS2
		{
			get
			{
				return this.IsKeyboard && (this.IsVendorIdPS2 || this.IsEngineKeyboard) && !this.IsConnectionVirtual;
			}
		}

		public bool IsMousePS2
		{
			get
			{
				return this.IsMouse && (this.IsVendorIdPS2 || this.IsAnyEngineMouse) && !this.IsConnectionVirtual;
			}
		}

		public bool IsPeripheralPS2
		{
			get
			{
				return (this.IsMouse || this.IsKeyboard) && (this.IsVendorIdPS2 || this.IsEngineKeyboard || this.IsAnyEngineMouse) && !this.IsConnectionVirtual;
			}
		}

		public bool IsConsumer
		{
			get
			{
				return this.ControllerType == 1002;
			}
		}

		public bool IsSystem
		{
			get
			{
				return this.ControllerType == 1003;
			}
		}

		public bool IsXboxAuth
		{
			get
			{
				return this.IsControllerExclusiveAccessSupported && (this.IsXboxOne || this.IsXboxOneX || this.IsXboxElite);
			}
		}

		public bool IsXboxElite
		{
			get
			{
				return this.ControllerType == 3 || this.ControllerType == 12;
			}
		}

		public bool IsXboxElite2
		{
			get
			{
				return this.ControllerType == 12;
			}
		}

		public bool IsXboxOne
		{
			get
			{
				return this.ControllerType == 2;
			}
		}

		public bool IsXboxOneX
		{
			get
			{
				return this.ControllerType == 22;
			}
		}

		public bool IsXbox360
		{
			get
			{
				return this.ControllerType == 1;
			}
		}

		public bool IsFlydigiXbox360
		{
			get
			{
				return this.IsXbox360 && this.ContainerDescription != null && this.ContainerDescription.ToLower().Contains("flydigi");
			}
		}

		public bool IsFlydigiApex3
		{
			get
			{
				return this.ControllerType == 62;
			}
		}

		public bool IsGameSirG7
		{
			get
			{
				return this.ControllerType == 55;
			}
		}

		public bool IsXboxGameSirG7
		{
			get
			{
				if (!this.IsGameSirG7)
				{
					ControllerVM currentController = this.CurrentController;
					ushort? num = ((currentController != null) ? new ushort?(currentController.VendorId) : null);
					int? num2 = ((num != null) ? new int?((int)num.GetValueOrDefault()) : null);
					int num3 = 13623;
					if ((num2.GetValueOrDefault() == num3) & (num2 != null))
					{
						ControllerVM currentController2 = this.CurrentController;
						num = ((currentController2 != null) ? new ushort?(currentController2.ProductId) : null);
						num2 = ((num != null) ? new int?((int)num.GetValueOrDefault()) : null);
						num3 = 4097;
						return (num2.GetValueOrDefault() == num3) & (num2 != null);
					}
				}
				return false;
			}
		}

		public bool IsSonyDualshock4
		{
			get
			{
				return this.IsSonyDualshock4USB || this.IsSonyDualshock4BT;
			}
		}

		public bool IsAnySonyDualSense
		{
			get
			{
				return this.IsSonyDualSense || this.IsSonyDualSenseEdge;
			}
		}

		public bool IsSonyDualSense
		{
			get
			{
				return this.IsSonyDualSenseUSB || this.IsSonyDualSenseBT;
			}
		}

		public bool IsSonyDualSenseEdge
		{
			get
			{
				return this.IsSonyDualSenseEdgeUSB || this.IsSonyDualSenseEdgeBT;
			}
		}

		public bool IsSonyDualshock4USB
		{
			get
			{
				return this.ControllerType == 4;
			}
		}

		public bool IsSonyDualshock4BT
		{
			get
			{
				return this.ControllerType == 14;
			}
		}

		public bool IsSonyDualSenseUSB
		{
			get
			{
				return this.ControllerType == 20;
			}
		}

		public bool IsSonyDualSenseBT
		{
			get
			{
				return this.ControllerType == 21;
			}
		}

		public bool IsSonyDualSenseEdgeUSB
		{
			get
			{
				return this.ControllerType == 56;
			}
		}

		public bool IsSonyDualSenseEdgeBT
		{
			get
			{
				return this.ControllerType == 57;
			}
		}

		public bool IsNVidiaShield
		{
			get
			{
				return this.ControllerType == 23;
			}
		}

		public bool IsNVidiaShield2
		{
			get
			{
				return this.ControllerType == 24;
			}
		}

		public bool IsAnySteam
		{
			get
			{
				return this.IsSteam || this.IsSteamDeck;
			}
		}

		public bool IsSteam
		{
			get
			{
				return this.ControllerType == 16;
			}
		}

		public bool IsSteamDeck
		{
			get
			{
				return this.ControllerType == 63;
			}
		}

		public bool IsGameCube
		{
			get
			{
				return this.ControllerType == 17;
			}
		}

		public bool IsGameCubeMayFlash
		{
			get
			{
				return this.ControllerType == 18;
			}
		}

		public bool Is8BitDo
		{
			get
			{
				return this.ControllerType == 25;
			}
		}

		public bool IsAzeron
		{
			get
			{
				return ControllerTypeExtensions.IsAzeron(this.ControllerType);
			}
		}

		public bool IsAzeronCyborg
		{
			get
			{
				return ControllerTypeExtensions.IsAzeronCyborg(this.ControllerType);
			}
		}

		public bool IsAzeronCyro
		{
			get
			{
				return ControllerTypeExtensions.IsAzeronCyro(this.ControllerType);
			}
		}

		public bool IsAnyAzeron
		{
			get
			{
				return this.IsAzeron || this.IsAzeronCyborg || this.IsAzeronCyro;
			}
		}

		public bool IsAzeronLefty
		{
			get
			{
				return RegistryHelper.GetValue("Config\\AzeronLefty", this.ID, 0, false) == 1;
			}
		}

		public bool IsChangeGamesirG7Mode
		{
			get
			{
				return RegistryHelper.GetValue("Config", "ChangeGameSirG7Mode", 0, false) == 1;
			}
		}

		public bool IsIpega
		{
			get
			{
				return ControllerTypeExtensions.IsIpega(this.ControllerType);
			}
		}

		public bool IsEngineGamepad
		{
			get
			{
				return ControllerTypeExtensions.IsAnyEngineGamepad(this.ControllerType);
			}
		}

		public bool IsEngineController
		{
			get
			{
				return ControllerTypeExtensions.IsAnyEngineController(this.ControllerType);
			}
		}

		public override bool IsNintendoSwitchJoyConL
		{
			get
			{
				return this.ControllerType == 9;
			}
		}

		public override bool IsNintendoSwitchJoyConR
		{
			get
			{
				return this.ControllerType == 10;
			}
		}

		public override bool IsNintendoSwitchJoyConComposite
		{
			get
			{
				return false;
			}
		}

		public bool IsRazerWolverine2
		{
			get
			{
				return ControllerTypeExtensions.IsRazerWolverine2(this.ControllerType);
			}
		}

		public bool IsGamepadLeftStickIsAllowed
		{
			get
			{
				return ControllerTypeExtensions.IsLeftStickAvailiable(this.ControllerType);
			}
		}

		public bool IsGamepadRightStickDirectionsIsAllowed
		{
			get
			{
				return ControllerTypeExtensions.IsRightStickDirectionsAvailiable(this.ControllerType);
			}
		}

		[JsonProperty("IsXimNexus")]
		public bool IsXimNexus { get; set; }

		public override bool IsUnknownControllerType
		{
			get
			{
				return this.ControllerType == 0;
			}
		}

		public override bool IsUnsupportedControllerType
		{
			get
			{
				return this.IsUnknownControllerType;
			}
		}

		[JsonProperty("IsInitialized")]
		public bool? IsInitialized
		{
			get
			{
				return this._isInitialized;
			}
			set
			{
				this._isInitialized = value;
			}
		}

		public override bool IsNonInitializedElite2
		{
			get
			{
				bool? isInitialized = this._isInitialized;
				bool flag = true;
				return !((isInitialized.GetValueOrDefault() == flag) & (isInitialized != null)) && this.IsXboxElite2;
			}
		}

		public override bool IsNonInitializedSegaGenesis
		{
			get
			{
				bool? isInitialized = this._isInitialized;
				bool flag = true;
				return !((isInitialized.GetValueOrDefault() == flag) & (isInitialized != null)) && this.ControllerType == 50;
			}
		}

		public override bool IsNonInitializedLogitech
		{
			get
			{
				bool? isInitialized = this._isInitialized;
				bool flag = true;
				return !((isInitialized.GetValueOrDefault() == flag) & (isInitialized != null)) && UtilsCommon.IsVidPidLogitechDirectInput(this.VendorId, this.ProductId);
			}
		}

		public override bool IsNonInitializedBluetoothPairing
		{
			get
			{
				bool? isInitialized = this._isInitialized;
				bool flag = true;
				return !((isInitialized.GetValueOrDefault() == flag) & (isInitialized != null)) && this.IsCanBluetoothPaired;
			}
		}

		public override bool IsNonInitializedGameCube
		{
			get
			{
				bool? isInitialized = this._isInitialized;
				bool flag = true;
				return !((isInitialized.GetValueOrDefault() == flag) & (isInitialized != null)) && this.IsGameCubeMayFlash;
			}
		}

		public override bool IsNonInitialized8BitDo
		{
			get
			{
				bool? isInitialized = this._isInitialized;
				bool flag = true;
				return !((isInitialized.GetValueOrDefault() == flag) & (isInitialized != null)) && this.Is8BitDo;
			}
		}

		public override bool IsNonInitializedFlydigiXbox360
		{
			get
			{
				bool? isInitialized = this._isInitialized;
				bool flag = true;
				return !((isInitialized.GetValueOrDefault() == flag) & (isInitialized != null)) && this.IsFlydigiXbox360;
			}
		}

		public override bool IsNonInitializedIpega
		{
			get
			{
				bool? isInitialized = this._isInitialized;
				bool flag = true;
				return !((isInitialized.GetValueOrDefault() == flag) & (isInitialized != null)) && this.IsIpega;
			}
		}

		public override bool IsNonInitializedGameSirG7
		{
			get
			{
				bool? isInitialized = this._isInitialized;
				bool flag = true;
				return !((isInitialized.GetValueOrDefault() == flag) & (isInitialized != null)) && this.IsXboxGameSirG7 && ControllerVM.IsInformGameSirG7 && !Engine.GamepadService.ServiceProfilesCollection.ContainsProfileForID(this.ID);
			}
		}

		public override bool CanReinitializeController
		{
			get
			{
				return Engine.GamepadService.InitializedDevices.Any((InitializedDevice p) => p.ID == this.ID);
			}
		}

		public static bool IsInformSteam
		{
			get
			{
				return RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmSteamExclusiveMode", 1, false) == 1;
			}
		}

		public static bool IsInformEngineController
		{
			get
			{
				return RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmEngineController", 1, false) == 1;
			}
		}

		public static bool IsInformAzeron
		{
			get
			{
				return RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmAzeronExclusiveMode", 1, false) == 1;
			}
		}

		public static bool IsInformFlydigi
		{
			get
			{
				return RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmFlydigiExclusiveMode", 1, false) == 1;
			}
		}

		public static bool IsInformGameSirG7
		{
			get
			{
				return RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmGameSirG7Mode", 1, false) == 1;
			}
		}

		public override bool IsNonInitializedExclusiveDevice
		{
			get
			{
				bool? isInitialized = this._isInitialized;
				bool flag = true;
				return !((isInitialized.GetValueOrDefault() == flag) & (isInitialized != null)) && ((ControllerVM.IsInformSteam && this.IsAnySteam) || (ControllerVM.IsInformAzeron && this.IsAnyAzeron) || (ControllerVM.IsInformFlydigi && ControllerTypeExtensions.IsFlydigi(this.ControllerType))) && !Engine.GamepadService.ServiceProfilesCollection.ContainsProfileForID(this.ID);
			}
		}

		public override bool IsNonInitializedEngineController
		{
			get
			{
				bool? isInitialized = this._isInitialized;
				bool flag = true;
				return !((isInitialized.GetValueOrDefault() == flag) & (isInitialized != null)) && this.IsEngineController && base.IsPromoController && !Engine.GamepadService.ServiceProfilesCollection.ContainsProfileForID(this.ID);
			}
		}

		public virtual bool IsRazer
		{
			get
			{
				return UtilsCommon.IsVidRazer(this.VendorId);
			}
		}

		public override bool IsInvalidControllerType
		{
			get
			{
				return this.IsUnknownControllerType && (UtilsCommon.IsVidPidXboxElite(this.VendorId, this.ProductId) || UtilsCommon.IsVidPidXboxElite2Bluetooth(this.VendorId, this.ProductId) || UtilsCommon.IsVidPidXboxAdaptiveBluetooth(this.VendorId, this.ProductId) || UtilsCommon.IsVidPidXboxOne(this.VendorId, this.ProductId) || UtilsCommon.IsVidPidXbox360(this.VendorId, this.ProductId) || UtilsCommon.IsVidPidSonyDualshock3(this.VendorId, this.ProductId) || UtilsCommon.IsVidPidSonyDualshock4(this.VendorId, this.ProductId) || UtilsCommon.IsVidPidSonyDualSense(this.VendorId, this.ProductId) || UtilsCommon.IsVidPidSonyDualSenseEdge(this.VendorId, this.ProductId) || UtilsCommon.IsVidPidSonyDualshock4WirelessAdapter(this.VendorId, this.ProductId) || UtilsCommon.IsVidPidSonyDualshock3Adapter(this.VendorId, this.ProductId) || UtilsCommon.IsVidPidSonyPs3Navigation(this.VendorId, this.ProductId) || UtilsCommon.IsVidPidNintendoSwitchPro(this.VendorId, this.ProductId) || UtilsCommon.IsVidPidNintendoSwitchWired(this.VendorId, this.ProductId) || UtilsCommon.IsVidPidNintendoSwitchJoyConChargingGrip(this.VendorId, this.ProductId) || UtilsCommon.IsVidPidNintendoSwitchJoyConL(this.VendorId, this.ProductId) || UtilsCommon.IsVidPidNintendoSwitchJoyConR(this.VendorId, this.ProductId) || UtilsCommon.IsVidPidGoogleStadia(this.VendorId, this.ProductId) || UtilsCommon.IsVidPidLogitechDirectInput(this.VendorId, this.ProductId) || UtilsCommon.IsVidPidLogitechXinput(this.VendorId, this.ProductId) || UtilsCommon.IsVidPidSteam(this.VendorId, this.ProductId) || UtilsCommon.IsVidPidNvidiaShield2015(this.VendorId, this.ProductId) || UtilsCommon.IsVidPidNvidiaShield2017(this.VendorId, this.ProductId) || UtilsCommon.IsVidPidPowerAMOGAXP5AP(this.VendorId, this.ProductId) || UtilsCommon.IsVidPidRazerWolverine2(this.VendorId, this.ProductId));
			}
		}

		public override List<BaseControllerVM> GetLEDSupportedControllers()
		{
			List<BaseControllerVM> list = new List<BaseControllerVM>();
			if (ControllerTypeExtensions.ConvertEnumToLEDSupportedType(this.ControllerType) != null)
			{
				list.Add(this);
			}
			return list;
		}

		public bool IsCanBluetoothPaired
		{
			get
			{
				ulong num;
				return (ControllerTypeExtensions.IsSonyDS3(this.ControllerType) || ControllerTypeExtensions.IsPS3Navigation(this.ControllerType)) && this.IsMasterAddressPresent && BluetoothUtils.BluetoothGetLocalRadioAddress(ref num) && num != this.MasterBthAddr;
			}
		}

		public override string ControllerTypeFriendlyName
		{
			get
			{
				return this.ControllerType.TryGetDescription();
			}
			set
			{
			}
		}

		public async void CheckXimNexusController(ushort vendorId, ushort productId, ulong controllerId, string containerDescription, bool isBluetoothConnectionFlagPresent)
		{
			if (!this.IsXimNexus)
			{
				if (containerDescription.ToLower().Contains("xim nexus"))
				{
					this.IsXimNexus = true;
					Engine.GamepadService.SendGamepadChanged(this);
					Engine.GamepadService.BinDataSerialize.SaveGamepadCollection();
				}
				else if (vendorId == 1118 && productId == 736 && isBluetoothConnectionFlagPresent)
				{
					List<BluetoothDeviceInfo> list = await Engine.XBServiceCommunicator.GetBluetoothDeviceInfoList(0U);
					if (list.Count > 0 && list.Exists((BluetoothDeviceInfo x) => x != null && x.Address == (controllerId & 281474976710655UL) && !string.IsNullOrEmpty(x.Name) && x.Name.ToLower().Contains("xim nexus")))
					{
						this.IsXimNexus = true;
						Engine.GamepadService.SendGamepadChanged(this);
						Engine.GamepadService.BinDataSerialize.SaveGamepadCollection();
					}
				}
			}
		}

		[JsonProperty("ControllerType")]
		[JsonConverter(typeof(StringEnumConverter))]
		public virtual ControllerTypeEnum ControllerType
		{
			get
			{
				return XBUtils.ControllerTypeEnumFromServiceControllerType(this.Type, this.VendorId, this.ProductId, this.ControllerId, this.IsXimNexus);
			}
		}

		public bool IsUnknownConnectionType
		{
			get
			{
				return !this.IsConnectionWired && !this.IsConnectionWireless && !this.IsConnectionVirtual;
			}
		}

		[JsonProperty("ConnectionType")]
		[JsonConverter(typeof(StringEnumConverter))]
		public ConnectionType ConnectionType
		{
			get
			{
				if (this.IsConnectionWired)
				{
					return 0;
				}
				if (this.IsConnectionWireless)
				{
					return 1;
				}
				if (this.IsConnectionVirtual)
				{
					return 2;
				}
				return 4;
			}
		}

		[JsonProperty("IsControllerBatteryBlockVisible")]
		public bool IsControllerBatteryBlockVisible
		{
			get
			{
				return (!this.IsFlydigiApex3 && !this.IsXimNexus && !this.IsNVidiaShield2 && !this.IsNVidiaShield && !this.IsGameCube && !this.IsGameCubeMayFlash && !this.IsAnySteam && !this.IsAnyAzeron && this.ControllerBatteryKind != null) || this.IsXboxOne || this.IsXboxElite || this.IsXboxOneX || this.IsRazerWolverine2 || (this.IsXbox360 && this.ConnectionType == null) || (this.Type == 1U && UtilsCommon.IsVidPidPowerAMOGAXP5AP(this.VendorId, this.ProductId));
			}
		}

		public bool IsControllerAuthAllowed
		{
			get
			{
				return this.IsXboxAuth || ControllerTypeExtensions.IsSonyDS4Auth(this.ControllerType);
			}
		}

		[JsonProperty("ControllerBatteryKind")]
		[JsonConverter(typeof(StringEnumConverter))]
		public BatteryKind ControllerBatteryKind
		{
			get
			{
				if (this.IsEngineController)
				{
					if (this._controllerBatteryKind == null)
					{
						this._controllerBatteryKind = 2;
					}
					return this._controllerBatteryKind;
				}
				if (this.Type == 4294967295U)
				{
					return 3;
				}
				switch (this._controllerInfo.BatteryKind)
				{
				case 0:
					return 0;
				case 1:
					return 1;
				case 2:
					return 2;
				default:
					return 3;
				}
			}
		}

		[JsonProperty("ControllerBatteryLevel")]
		[JsonConverter(typeof(StringEnumConverter))]
		public BatteryLevel ControllerBatteryLevel
		{
			get
			{
				if (this.ConnectionType == null && ControllerTypeExtensions.IsXboxOrRazerWolverine2(this.ControllerType) && !ControllerTypeExtensions.IsXboxElite2(this.ControllerType))
				{
					return 4;
				}
				switch (Convert.ToBoolean((int)(this._controllerInfo.BatteryLevel & 128)) ? this.GetBatteryLevelFromPercents() : this._controllerInfo.BatteryLevel)
				{
				case 0:
					return 0;
				case 1:
					return 1;
				case 2:
					return 2;
				case 3:
					return 3;
				default:
					return 3;
				}
			}
		}

		public byte GetBatteryLevelFromPercents()
		{
			bool flag = this._controllerInfo.Type == 268435455U;
			uint num = (flag ? 10U : 20U);
			uint num2 = (flag ? 20U : 40U);
			byte batteryPercents = this.GetBatteryPercents();
			if ((uint)batteryPercents < num)
			{
				return 0;
			}
			if ((uint)batteryPercents < num2)
			{
				return 1;
			}
			if (batteryPercents < 80)
			{
				return 2;
			}
			return 3;
		}

		public byte GetBatteryPercents()
		{
			if (!Convert.ToBoolean((int)(this._controllerInfo.BatteryLevel & 128)))
			{
				return 0;
			}
			byte b = this._controllerInfo.BatteryLevel & 127;
			if (b > 100)
			{
				b = 100;
			}
			return b;
		}

		[JsonProperty("ControllerBatteryChargingState")]
		[JsonConverter(typeof(StringEnumConverter))]
		public BatteryChargingState ControllerBatteryChargingState
		{
			get
			{
				if (this.Type == 4294967295U || this.ControllerBatteryKind != 2)
				{
					return 3;
				}
				switch (this._controllerInfo.BatteryChargingState)
				{
				case 0:
					return 0;
				case 1:
					return 1;
				case 2:
					return 2;
				default:
					return 3;
				}
			}
		}

		public void TraceControllerInfo(string traceWriteTag = " - Controllers")
		{
			if (!Tracer.IsTextFileTraceEnabled)
			{
				return;
			}
			ControllerTypeEnum controllerTypeEnum = ControllerTypeExtensions.ConvertPhysicalTypeToEnum(0, this.Type, this.ControllerId);
			string text = "";
			if (this.IsConnectionVirtual)
			{
				text = " virtual";
			}
			Tracer.TraceWriteTag(traceWriteTag, "---", false);
			if (ControllerTypeExtensions.IsGamepad(controllerTypeEnum))
			{
				Tracer.TraceWriteTag(traceWriteTag, "Gamepad", false);
			}
			if (ControllerTypeExtensions.IsKeyboardStandart(controllerTypeEnum))
			{
				Tracer.TraceWriteTag(traceWriteTag, "Keyboard" + text, false);
			}
			if (ControllerTypeExtensions.IsConsumer(controllerTypeEnum))
			{
				Tracer.TraceWriteTag(traceWriteTag, "Consumer", false);
			}
			if (ControllerTypeExtensions.IsSystem(controllerTypeEnum))
			{
				Tracer.TraceWriteTag(traceWriteTag, "System", false);
			}
			if (ControllerTypeExtensions.IsMouse(controllerTypeEnum))
			{
				Tracer.TraceWriteTag(traceWriteTag, "Mouse" + text, false);
			}
			if (this.IsChatpad)
			{
				Tracer.TraceWriteTag(traceWriteTag, "Chatpad", false);
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 2);
			defaultInterpolatedStringHandler.AppendLiteral("Id: 0x");
			defaultInterpolatedStringHandler.AppendFormatted<ulong>(this._controllerInfo.Id, "X");
			defaultInterpolatedStringHandler.AppendLiteral(" / ");
			defaultInterpolatedStringHandler.AppendFormatted<ulong>(this._controllerInfo.Id);
			Tracer.TraceWriteTag(traceWriteTag, defaultInterpolatedStringHandler.ToStringAndClear(), false);
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(38, 3);
			defaultInterpolatedStringHandler.AppendLiteral("VendorID: 0x");
			defaultInterpolatedStringHandler.AppendFormatted<ushort>(this._controllerInfo.VendorId, "X");
			defaultInterpolatedStringHandler.AppendLiteral(" ProductID: 0x");
			defaultInterpolatedStringHandler.AppendFormatted<ushort>(this._controllerInfo.ProductId, "X");
			defaultInterpolatedStringHandler.AppendLiteral(" Version: 0x");
			defaultInterpolatedStringHandler.AppendFormatted<ushort>(this._controllerInfo.Version, "X");
			Tracer.TraceWriteTag(traceWriteTag, defaultInterpolatedStringHandler.ToStringAndClear(), false);
			if (UtilsCommon.IsVidPidXboxElite(this.VendorId, this.ProductId))
			{
				Tracer.TraceWriteTag(traceWriteTag, "VID PID: Xbox Elite", false);
			}
			else if (UtilsCommon.IsVidPidXboxElite2Bluetooth(this.VendorId, this.ProductId))
			{
				Tracer.TraceWriteTag(traceWriteTag, "VID PID: Xbox Elite Series 2 on bluetooth", false);
			}
			else if (UtilsCommon.IsVidPidXboxAdaptiveBluetooth(this.VendorId, this.ProductId))
			{
				Tracer.TraceWriteTag(traceWriteTag, "VID PID: Xbox Adaptive Controller on bluetooth", false);
			}
			else if (UtilsCommon.IsVidPidXboxOne(this.VendorId, this.ProductId))
			{
				Tracer.TraceWriteTag(traceWriteTag, "VID PID: Xbox One", false);
			}
			else if (UtilsCommon.IsVidPidXbox360(this.VendorId, this.ProductId))
			{
				Tracer.TraceWriteTag(traceWriteTag, "VID PID: Xbox 360", false);
			}
			else if (UtilsCommon.IsVidPidSonyDualshock4(this.VendorId, this.ProductId))
			{
				Tracer.TraceWriteTag(traceWriteTag, "VID PID: Sony Dualshock4", false);
			}
			else if (UtilsCommon.IsVidPidSonyDualSense(this.VendorId, this.ProductId))
			{
				Tracer.TraceWriteTag(traceWriteTag, "VID PID: Sony DualSense", false);
			}
			else if (UtilsCommon.IsVidPidSonyDualSenseEdge(this.VendorId, this.ProductId))
			{
				Tracer.TraceWriteTag(traceWriteTag, "VID PID: Sony DualSense Edge", false);
			}
			else if (UtilsCommon.IsVidPidSonyDualshock4WirelessAdapter(this.VendorId, this.ProductId))
			{
				Tracer.TraceWriteTag(traceWriteTag, "VID PID: Sony Dualshock4 Wireless Adapter", false);
			}
			else if (UtilsCommon.IsVidPidSonyDualshock3(this.VendorId, this.ProductId))
			{
				Tracer.TraceWriteTag(traceWriteTag, "VID PID: Sony Dualshock3", false);
			}
			else if (UtilsCommon.IsVidPidSonyDualshock3Adapter(this.VendorId, this.ProductId))
			{
				Tracer.TraceWriteTag(traceWriteTag, "VID PID: SonyDualshock3 Adapter", false);
			}
			else if (UtilsCommon.IsVidPidSonyPs3Navigation(this.VendorId, this.ProductId))
			{
				Tracer.TraceWriteTag(traceWriteTag, "VID PID: Playstation Navigation", false);
			}
			else if (UtilsCommon.IsVidPidNintendoSwitchPro(this.VendorId, this.ProductId))
			{
				Tracer.TraceWriteTag(traceWriteTag, "VID PID: Nintendo Switch Pro", false);
			}
			else if (UtilsCommon.IsVidPidNintendoSwitchWired(this.VendorId, this.ProductId))
			{
				Tracer.TraceWriteTag(traceWriteTag, "VID PID: Nintendo Switch Wired", false);
			}
			else if (UtilsCommon.IsVidPidNintendoSwitchJoyConChargingGrip(this.VendorId, this.ProductId))
			{
				Tracer.TraceWriteTag(traceWriteTag, "VID PID: Nintendo Switch Joy-Con in Charging Grip", false);
			}
			else if (UtilsCommon.IsVidPidNintendoSwitchJoyConL(this.VendorId, this.ProductId))
			{
				Tracer.TraceWriteTag(traceWriteTag, "VID PID: Nintendo Switch Joy-Con L", false);
			}
			else if (UtilsCommon.IsVidPidNintendoSwitchJoyConR(this.VendorId, this.ProductId))
			{
				Tracer.TraceWriteTag(traceWriteTag, "VID PID: Nintendo Switch Joy-Con R", false);
			}
			else if (UtilsCommon.IsVidPidGoogleStadia(this.VendorId, this.ProductId))
			{
				Tracer.TraceWriteTag(traceWriteTag, "VID PID: Google Stadia", false);
			}
			else if (UtilsCommon.IsVidPidSteam(this.VendorId, this.ProductId))
			{
				Tracer.TraceWriteTag(traceWriteTag, "VID PID: Valve Steam", false);
			}
			else if (UtilsCommon.IsVidPidLogitechDirectInput(this.VendorId, this.ProductId))
			{
				Tracer.TraceWriteTag(traceWriteTag, "VID PID: Logitech (DirectInput)", false);
			}
			else if (UtilsCommon.IsVidPidLogitechXinput(this.VendorId, this.ProductId))
			{
				Tracer.TraceWriteTag(traceWriteTag, "VID PID: Logitech (Xinput)", false);
			}
			else if (UtilsCommon.IsVidPidNvidiaShield2015(this.VendorId, this.ProductId))
			{
				Tracer.TraceWriteTag(traceWriteTag, "VID PID: Nvidia Shield 2015", false);
			}
			else if (UtilsCommon.IsVidPidNvidiaShield2017(this.VendorId, this.ProductId))
			{
				Tracer.TraceWriteTag(traceWriteTag, "VID PID: Nvidia Shield 2017", false);
			}
			else if (UtilsCommon.IsVidPidPowerAMOGAXP5AP(this.VendorId, this.ProductId))
			{
				Tracer.TraceWriteTag(traceWriteTag, "VID PID: PowerA MOGA XP5-X Plus", false);
			}
			else if (UtilsCommon.IsVidPidRazerWolverine2(this.VendorId, this.ProductId))
			{
				Tracer.TraceWriteTag(traceWriteTag, "VID PID: Razer Wolverine V2", false);
			}
			else if (UtilsCommon.IsVidPidGameSirG7(this.VendorId, this.ProductId))
			{
				Tracer.TraceWriteTag(traceWriteTag, "VID PID: GameSir G7", false);
			}
			if (this.Type == 268435453U)
			{
				Tracer.TraceWriteTag(traceWriteTag, "ControllerType: Engine keyboard", false);
			}
			else if (this.Type == 268435454U)
			{
				Tracer.TraceWriteTag(traceWriteTag, "ControllerType: Engine mouse", false);
			}
			else if (this.Type == 2147483648U)
			{
				if (this.IsConnectionVirtual)
				{
					Tracer.TraceWriteTag(traceWriteTag, "ControllerType: Virtual keyboard", false);
				}
				else if (this.VendorId == 65535)
				{
					Tracer.TraceWriteTag(traceWriteTag, "ControllerType: Remote Desktop keyboard", false);
				}
				else if (this.VendorId == 65534)
				{
					Tracer.TraceWriteTag(traceWriteTag, "ControllerType: PS/2 keyboard", false);
				}
				else
				{
					Tracer.TraceWriteTag(traceWriteTag, "ControllerType: HID compatible keyboard", false);
				}
			}
			else if (this.Type == 2147483649U)
			{
				if (this.IsConnectionVirtual)
				{
					Tracer.TraceWriteTag(traceWriteTag, "ControllerType: Virtual mouse", false);
				}
				else if (this.VendorId == 65535)
				{
					Tracer.TraceWriteTag(traceWriteTag, "ControllerType: Remote Desktop mouse", false);
				}
				else if (this.VendorId == 65534)
				{
					Tracer.TraceWriteTag(traceWriteTag, "ControllerType: PS/2 mouse", false);
				}
				else if (this.VendorId == 65533)
				{
					Tracer.TraceWriteTag(traceWriteTag, "ControllerType: Serial mouse", false);
				}
				else
				{
					Tracer.TraceWriteTag(traceWriteTag, "ControllerType: HID compatible mouse", false);
				}
			}
			else if (this.Type == 4294967295U)
			{
				Tracer.TraceWriteTag(traceWriteTag, "ControllerType: Type is unsupported 'HID compliant game controller'.\n", false);
			}
			else if (this.Type == 2147483650U)
			{
				Tracer.TraceWriteTag(traceWriteTag, "ControllerType: consumer", false);
			}
			else if (this.Type == 2147483651U)
			{
				Tracer.TraceWriteTag(traceWriteTag, "ControllerType: system", false);
			}
			else
			{
				foreach (SupportedControllerInfo supportedControllerInfo in ControllersHelper.SupportedControllers)
				{
					SupportedGamepad supportedGamepad = supportedControllerInfo as SupportedGamepad;
					if (supportedGamepad != null && supportedGamepad.ServiceControllerType == this.Type)
					{
						string text2 = "ControllerType: ";
						DescriptionAttribute descriptionAttribute = typeof(ControllerTypeEnum).GetField(supportedGamepad.ControllerType.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false)[0] as DescriptionAttribute;
						Tracer.TraceWriteTag(traceWriteTag, text2 + ((descriptionAttribute != null) ? descriptionAttribute.Description : null), false);
						break;
					}
				}
			}
			if (ControllerTypeExtensions.IsXboxEliteOrOne(controllerTypeEnum) && (this._controllerInfo.FirmwareVersionMajor != 0 || this._controllerInfo.FirmwareVersionMinor != 0 || this._controllerInfo.FirmwareVersionBuild != 0 || this._controllerInfo.FirmwareVersionRevision != 0))
			{
				string[] array = new string[5];
				array[0] = "FirmwareVersion: ";
				int num = 1;
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 1);
				defaultInterpolatedStringHandler.AppendFormatted<ushort>(this._controllerInfo.FirmwareVersionMajor);
				defaultInterpolatedStringHandler.AppendLiteral(".");
				array[num] = defaultInterpolatedStringHandler.ToStringAndClear();
				int num2 = 2;
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 1);
				defaultInterpolatedStringHandler.AppendFormatted<ushort>(this._controllerInfo.FirmwareVersionMinor);
				defaultInterpolatedStringHandler.AppendLiteral(".");
				array[num2] = defaultInterpolatedStringHandler.ToStringAndClear();
				int num3 = 3;
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 1);
				defaultInterpolatedStringHandler.AppendFormatted<ushort>(this._controllerInfo.FirmwareVersionBuild);
				defaultInterpolatedStringHandler.AppendLiteral(".");
				array[num3] = defaultInterpolatedStringHandler.ToStringAndClear();
				int num4 = 4;
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
				defaultInterpolatedStringHandler.AppendFormatted<ushort>(this._controllerInfo.FirmwareVersionRevision);
				array[num4] = defaultInterpolatedStringHandler.ToStringAndClear();
				Tracer.TraceWriteTag(traceWriteTag, string.Concat(array), false);
			}
			Tracer.TraceWriteTag(traceWriteTag, "Description: " + this.Description, false);
			Tracer.TraceWriteTag(traceWriteTag, "Container description: " + this.ContainerDescription, false);
			if (!string.IsNullOrEmpty(this.ManufacturerName))
			{
				Tracer.TraceWriteTag(traceWriteTag, "ManufacturerName: " + this.ManufacturerName, false);
			}
			if (!string.IsNullOrEmpty(this.ProductName))
			{
				Tracer.TraceWriteTag(traceWriteTag, "ProductName: " + this.ProductName, false);
			}
			Tracer.TraceWriteTag(traceWriteTag, "ContainerId: " + base.ContainerIdString, false);
			if (this.IsConnectionWired)
			{
				Tracer.TraceWriteTag(traceWriteTag, "ConnectionType: Wired", false);
			}
			else if (this.IsConnectionWireless)
			{
				Tracer.TraceWriteTag(traceWriteTag, "ConnectionType: Wireless", false);
			}
			else if (this.IsConnectionVirtual)
			{
				Tracer.TraceWriteTag(traceWriteTag, "ConnectionType: Virtual", false);
			}
			else
			{
				Tracer.TraceWriteTag(traceWriteTag, "ConnectionType: Unknown", false);
			}
			if (this.Type != 4294967295U)
			{
				switch (this._controllerInfo.BatteryKind)
				{
				case 0:
					Tracer.TraceWriteTag(traceWriteTag, "BatteryKind: None", false);
					break;
				case 1:
					Tracer.TraceWriteTag(traceWriteTag, "BatteryKind: Standard", false);
					break;
				case 2:
					Tracer.TraceWriteTag(traceWriteTag, "BatteryKind: Rechargeable", false);
					break;
				case 3:
					Tracer.TraceWriteTag(traceWriteTag, "BatteryKind: Unknown", false);
					break;
				}
				if (this._controllerInfo.BatteryKind != 0)
				{
					bool flag = Convert.ToBoolean((int)(this._controllerInfo.BatteryLevel & 128));
					switch (flag ? this.GetBatteryLevelFromPercents() : this._controllerInfo.BatteryLevel)
					{
					case 0:
						Tracer.TraceWriteTag(traceWriteTag, "BatteryLevel: Critical", false);
						break;
					case 1:
						Tracer.TraceWriteTag(traceWriteTag, "BatteryLevel: Low", false);
						break;
					case 2:
						Tracer.TraceWriteTag(traceWriteTag, "BatteryLevel: Medium", false);
						break;
					case 3:
						Tracer.TraceWriteTag(traceWriteTag, "BatteryLevel: High", false);
						break;
					}
					if (flag)
					{
						Tracer.TraceWriteTag(traceWriteTag, "BatteryPercents: " + this.GetBatteryPercents().ToString() + "%", false);
					}
				}
				if (this._controllerInfo.BatteryKind == 2)
				{
					switch (this._controllerInfo.BatteryChargingState)
					{
					case 0:
						Tracer.TraceWriteTag(traceWriteTag, "BatteryChargingState: Inactive", false);
						break;
					case 1:
						Tracer.TraceWriteTag(traceWriteTag, "BatteryChargingState: Active", false);
						break;
					case 2:
						Tracer.TraceWriteTag(traceWriteTag, "BatteryChargingState: Error", false);
						break;
					case 3:
						Tracer.TraceWriteTag(traceWriteTag, "BatteryChargingState: Unknown", false);
						break;
					}
				}
			}
			else
			{
				Tracer.TraceWriteTag(traceWriteTag, "BatteryKind: Unknown", false);
				Tracer.TraceWriteTag(traceWriteTag, "BatteryLevel: Unknown", false);
				Tracer.TraceWriteTag(traceWriteTag, "BatteryChargingState: Unknown", false);
			}
			if (this.IsSteamExtendedPresent)
			{
				Tracer.TraceWriteTag(traceWriteTag, "Steam Extended Feature Support is active", false);
			}
			if (this.Type != 2147483648U && this.Type != 2147483649U && this.Type != 2147483650U && this.Type != 2147483651U)
			{
				Tracer.TraceWriteTag(traceWriteTag, this.IsBluetoothConnectionFlagPresent ? "Bluetooth connection flag: true" : "Bluetooth connection flag: false", false);
				Tracer.TraceWriteTag(traceWriteTag, this.IsAnalogTriggersPresent ? "Analog triggers are present" : "Analog triggers are not present", false);
				Tracer.TraceWriteTag(traceWriteTag, this.IsMotorRumbleMotorPresent ? "Motors are present" : "Motors are not present", false);
				Tracer.TraceWriteTag(traceWriteTag, this.IsTriggerRumbleMotorPresent ? "Trigger motors are present" : "Trigger motors are not present", false);
				Tracer.TraceWriteTag(traceWriteTag, this.IsGyroscopePresent ? "3-axis gyroscope is present" : "3-axis gyroscope is not present", false);
				Tracer.TraceWriteTag(traceWriteTag, this.IsAccelerometerPresent ? "3-axis accelerometer is present" : "3-axis accelerometer is not present", false);
				if (this.IsAdaptiveTriggersPresent)
				{
					Tracer.TraceWriteTag(traceWriteTag, "Adaptive triggers is present", false);
				}
				if (this.IsTouchpadPresent)
				{
					Tracer.TraceWriteTag(traceWriteTag, "Touchpad is present", false);
				}
				if (this.IsTiltMode2)
				{
					Tracer.TraceWriteTag(traceWriteTag, "TiltMode2: Gamepad is in tile mode 2 (XZ plane used for tilts instead of XY)", false);
				}
				if (this.IsPowerManagementEnabled)
				{
					Tracer.TraceWriteTag(traceWriteTag, "Dongle power management is enabled!", false);
				}
				if (this.IsPowerDownSupported)
				{
					Tracer.TraceWriteTag(traceWriteTag, "Power down is supported", false);
				}
				if (this.IsControllerExclusiveAccessSupported)
				{
					Tracer.TraceWriteTag(traceWriteTag, "Controller property: exclusive access is supported", false);
				}
				if (this.IsLedPresent)
				{
					string text3 = (this.IsMonochromeLed ? "monochrome" : "RGB");
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(54, 1);
					defaultInterpolatedStringHandler.AppendLiteral("LED present. MaxLedValue: ");
					defaultInterpolatedStringHandler.AppendFormatted<byte>(this.MaxLedValue);
					defaultInterpolatedStringHandler.AppendLiteral(". Controller property flag: ");
					Tracer.TraceWriteTag(traceWriteTag, defaultInterpolatedStringHandler.ToStringAndClear() + text3, false);
				}
				if (this.IsUserLedsPresent)
				{
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 1);
					defaultInterpolatedStringHandler.AppendLiteral("User LED present. NumUserLeds: ");
					defaultInterpolatedStringHandler.AppendFormatted<byte>(this.NumUserLeds);
					Tracer.TraceWriteTag(traceWriteTag, defaultInterpolatedStringHandler.ToStringAndClear(), false);
				}
				if (this.Type == 20U || this.Type == 21U)
				{
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 1);
					defaultInterpolatedStringHandler.AppendLiteral("MasterAddress property: ");
					defaultInterpolatedStringHandler.AppendFormatted<bool>(this.IsMasterAddressPresent);
					Tracer.TraceWriteTag(traceWriteTag, defaultInterpolatedStringHandler.ToStringAndClear(), false);
					if (this.IsMasterAddressPresent)
					{
						if (this.MasterBthAddr == 0UL)
						{
							Tracer.TraceWriteTag(traceWriteTag, "MasterBthAddr: no bluetooth pairing exists", false);
						}
						else
						{
							Tracer.TraceWriteTag(traceWriteTag, string.Concat(new string[]
							{
								"MasterBthAddr: bluetooth pairing with 0x",
								((byte)(this.MasterBthAddr >> 40)).ToString("X2"),
								":0x",
								((byte)(this.MasterBthAddr >> 32)).ToString("X2"),
								":0x",
								((byte)(this.MasterBthAddr >> 24)).ToString("X2"),
								":0x",
								((byte)(this.MasterBthAddr >> 16)).ToString("X2"),
								":0x",
								((byte)(this.MasterBthAddr >> 8)).ToString("X2"),
								":0x",
								((byte)this.MasterBthAddr).ToString("X2"),
								" exists"
							}), false);
						}
					}
				}
			}
			if (this.Type == 23U || this.Type == 24U || this.Type == 25U || this.Type == 26U)
			{
				Tracer.TraceWriteTag(traceWriteTag, string.Concat(new string[]
				{
					"MasterBthAddr: 0x",
					((byte)(this.MasterBthAddr >> 40)).ToString("X2"),
					":0x",
					((byte)(this.MasterBthAddr >> 32)).ToString("X2"),
					":0x",
					((byte)(this.MasterBthAddr >> 24)).ToString("X2"),
					":0x",
					((byte)(this.MasterBthAddr >> 16)).ToString("X2"),
					":0x",
					((byte)(this.MasterBthAddr >> 8)).ToString("X2"),
					":0x",
					((byte)this.MasterBthAddr).ToString("X2")
				}), false);
			}
			if ((this.Type == 2147483648U || this.Type == 2147483649U) && REWASD_CONTROLLER_INFO_Extensions.IsKeyboardMouseInput(this._controllerInfo))
			{
				Tracer.TraceWriteTag(traceWriteTag, (this.Type == 2147483648U) ? "Keyboard input emulation" : "Mouse input emulation", false);
			}
			Tracer.TraceWriteTag(traceWriteTag, "---", false);
			Tracer.TraceWriteTag(traceWriteTag, "", false);
		}

		public override async Task<GamepadState> GetControllerPressedButtons()
		{
			GamepadState gamepadState;
			if (!base.IsOnline || base.IsDebugController)
			{
				gamepadState = new GamepadState();
			}
			else
			{
				gamepadState = await Engine.XBServiceCommunicator.GetGamepadPressedButtons(this.ControllerId, this.Type, this.ControllerType, false);
			}
			return gamepadState;
		}

		public override async Task<Slot?> GetActivePhyscialSlot()
		{
			return await Engine.XBServiceCommunicator.GetActivePhysicalSlot(this.ControllerId, this.Type, this.ControllerType);
		}

		public override async Task<bool> GetIsControllerPressedButton()
		{
			bool flag;
			if (!base.IsOnline || base.IsDebugController)
			{
				flag = false;
			}
			else
			{
				flag = await Engine.XBServiceCommunicator.IsControllerPressedButton(this.ControllerId, this.Type, this.ControllerType);
			}
			return flag;
		}

		public void ToggleAzeronLefy()
		{
			RegistryHelper.SetValue("Config\\AzeronLefty", this.ID, this.IsAzeronLefty ? 0 : 1);
		}

		private Drawing GetDrawingForController()
		{
			if (this.IsAnyAzeron && this.IsAzeronLefty)
			{
				return Application.Current.TryFindResource("MiniGamepadAzeronLefty") as Drawing;
			}
			return XBUtils.GetDrawingForControllerTypeEnum(this.ControllerType, this.IsBluetoothConnectionFlagPresent);
		}

		public override void SetIsInitialized(bool value)
		{
			this._isInitialized = new bool?(value);
			IEventAggregator eventAggregator = Engine.EventAggregator;
			if (eventAggregator == null)
			{
				return;
			}
			eventAggregator.GetEvent<GamepadInitializationChanged>().Publish(this);
		}

		public void SetEngineBatteryState(byte batteryLevel, BatteryChargingState chargingState)
		{
			if (!this.IsEngineController)
			{
				return;
			}
			this._controllerInfo.BatteryLevel = batteryLevel;
			this._controllerInfo.BatteryLevel = this._controllerInfo.BatteryLevel | 128;
			this._controllerBatteryKind = 2;
			switch (chargingState)
			{
			case 0:
				this._controllerInfo.BatteryChargingState = 0;
				return;
			case 1:
				this._controllerInfo.BatteryChargingState = 1;
				return;
			case 2:
				this._controllerInfo.BatteryChargingState = 2;
				return;
			case 3:
				this._controllerInfo.BatteryChargingState = 3;
				return;
			default:
				return;
			}
		}

		private REWASD_CONTROLLER_INFO _controllerInfo;

		private REWASD_GET_CONTROLLER_STATE _controllerState;

		private string _description;

		private BatteryKind _controllerBatteryKind;

		private bool? _isInitialized;
	}
}
