using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoftReWASDServiceNamespace;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Prism.Commands;
using Prism.Events;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Infrastructure.SupportedControllers;
using reWASDCommon.Infrastructure.SupportedControllers.Base;
using reWASDCommon.Utils;
using reWASDUI.Services;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtilModel;

namespace reWASDUI.Infrastructure.Controller
{
	public class ControllerVM : BaseControllerVM
	{
		public string ControllerInfoString { get; set; }

		public string UnknownControllerInfoString { get; set; }

		public override Drawing MiniGamepadSVGIco
		{
			get
			{
				return this.GetDrawingForController();
			}
		}

		public SupportedGamepad SupportedControllerInfo
		{
			get
			{
				if (this._supportedControllerInfo == null || this._supportedControllerInfo.ControllerType != this.ControllerType)
				{
					SupportedControllerInfo supportedControllerInfo;
					ControllersHelper.SupportedControllersDictionary.TryGetValue(this.ControllerType, out supportedControllerInfo);
					this._supportedControllerInfo = supportedControllerInfo as SupportedGamepad;
				}
				return this._supportedControllerInfo;
			}
		}

		public override bool IsAvailiableForComposition
		{
			get
			{
				return base.IsAvailiableForComposition && this.ControllerType > 0;
			}
		}

		public bool IsUnmapAvailable
		{
			get
			{
				return this.IsXboxElite || this.IsXboxOne || this.IsAnySteam || this.IsXboxOneX;
			}
		}

		public bool IsUnmapAvailableConsideringMappings
		{
			get
			{
				return base.GameProfilesService.RealCurrentBeingMappedBindingCollection != null && (this.IsAnySteam || this.IsFlydigi || (this.IsAzeronCyro && base.GameProfilesService.RealCurrentBeingMappedBindingCollection.SubConfigData.ControllerFamily == this.ControllerFamily) || (this.IsUnmapAvailable && !base.GameProfilesService.RealCurrentBeingMappedBindingCollection.CurrentBindingIsLeftStick && !base.GameProfilesService.RealCurrentBeingMappedBindingCollection.CurrentBindingIsRightStick));
			}
		}

		public bool IsRumbleAvailable
		{
			get
			{
				bool flag = ControllerTypeExtensions.IsAnyAzeron(this.ControllerType);
				bool flag2 = ControllerTypeExtensions.IsAnyEngineGamepad(this.ControllerType);
				return !flag && !flag2;
			}
		}

		public bool IsExtendedMappingAvailable
		{
			get
			{
				return this.IsXboxElite || this.IsXboxOne || this.IsAnySteam || this.IsXboxOneX || this.IsAnyAzeron || this.IsFlydigi;
			}
		}

		public bool IsExtendedMappingAvailableConsideringMappings
		{
			get
			{
				return base.GameProfilesService.RealCurrentBeingMappedBindingCollection != null && this.IsExtendedMappingAvailable && !base.GameProfilesService.RealCurrentBeingMappedBindingCollection.CurrentBindingIsMouseDirectionOrZone && !base.GameProfilesService.RealCurrentBeingMappedBindingCollection.CurrentBindingIsGyroTilt;
			}
		}

		public ControllerVM()
		{
			IEventAggregator eventAggregator = App.EventAggregator;
			if (eventAggregator != null)
			{
				eventAggregator.GetEvent<CurrentGamepadChanged>().Subscribe(delegate(BaseControllerVM c)
				{
					if (this.IsExclusiveDevice || (this.IsXboxGameSirG7 && ControllerVM.IsInformGameSirG7))
					{
						this._isInitializedController = new bool?(false);
					}
					this.OnPropertyChanged("IsNonInitializedExclusiveDevice");
					this.OnPropertyChanged("IsInitializedController");
				});
			}
			TranslationManager.Instance.LanguageChanged += delegate([Nullable(2)] object s, EventArgs e)
			{
				this.OnPropertyChanged("BatteryLevelPercent");
			};
		}

		public override void Init()
		{
			this.UpdateControllerInfo();
			base.FillFriendlyName();
		}

		public override void VibrateForce()
		{
			if ((base.IsMotorRumbleMotorPresent || base.IsTriggerRumbleMotorPresent) && base.IsOnline)
			{
				base.HttpClientService.Gamepad.SendControllerVibration(this.ControllerId, this.Type);
			}
		}

		protected override void Vibrate()
		{
			if ((base.IsMotorRumbleMotorPresent || base.IsTriggerRumbleMotorPresent) && base.IsOnline && base.UserSettingsService.SendRumbleToGamepad)
			{
				base.HttpClientService.Gamepad.SendControllerVibration(this.ControllerId, this.Type);
			}
		}

		public override void UpdateControllerInfo()
		{
			if (this._isInitializedController == null && (this.ControllerType == 12 || this.ControllerType == 15 || this.IsFlydigiXbox360 || this.IsSega || this.IsCanBluetoothPaired || this.IsXboxGameSirG7 || ControllerTypeExtensions.IsAnySteam(this.ControllerType) || ControllerTypeExtensions.IsAnyAzeron(this.ControllerType) || ControllerTypeExtensions.IsFlydigi(this.ControllerType) || this.ControllerType == 18 || this.ControllerType == 25 || this.ControllerType == 500 || this.ControllerType == 26))
			{
				this._isInitializedController = new bool?(false);
			}
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
					base.HttpClientService.Gamepad.ControllerUpdateGyroCalibrationMode(this.ControllerId, this.Type);
				}
			}
			this.OnPropertyChanged("UpdateControllerInfo");
			this.OnPropertyChanged("IsControllerBatteryBlockVisible");
			this.OnPropertyChanged("ControllerBatteryLevel");
			this.OnPropertyChanged("IsConnectionWired");
			this.OnPropertyChanged("MiniGamepadSVGIco");
			this.OnPropertyChanged("HasOnlineGamepadVibrateControllers");
			this.OnPropertyChanged("IsCharging");
		}

		public override void UpdateControllerState(REWASD_GET_CONTROLLER_STATE_RESPONSE controllerState)
		{
			this.OnPropertyChanged("ControllerBatteryLevel");
			this.OnPropertyChanged("ControllerBatteryChargingState");
			this.OnPropertyChanged("IsCharging");
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
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 1);
				defaultInterpolatedStringHandler.AppendLiteral("ID: ");
				defaultInterpolatedStringHandler.AppendFormatted<ulong>(this.ControllerId, "X16");
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}
		}

		[JsonProperty("ControllerId")]
		public ulong ControllerId { get; set; }

		public uint Type { get; set; }

		public string Description { get; set; }

		public string ContainerDescription { get; set; }

		public string ManufacturerName { get; set; }

		public string ProductName { get; set; }

		public ushort VendorId { get; set; }

		public bool IsVendorIdPS2
		{
			get
			{
				return this.VendorId == 65534;
			}
		}

		public ushort ProductId { get; set; }

		public ushort Version { get; set; }

		public ushort FirmwareVersionMajor { get; set; }

		public ushort FirmwareVersionMinor { get; set; }

		public ushort FirmwareVersionBuild { get; set; }

		public ushort FirmwareVersionRevision { get; set; }

		public bool IsPowerManagementEnabled { get; set; }

		public bool IsPowerDownSupported { get; set; }

		public bool IsControllerPropertyChatpad { get; set; }

		public bool IsControllerPhysicalOutputAndDebug { get; set; }

		public bool IsControllerSupportHardwareGyroCalibration { get; set; }

		public bool IsControllerGyroAutomaticCalibrationDisabled { get; set; }

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
				return this.ConnectionType == 0;
			}
		}

		public bool IsConnectionWireless
		{
			get
			{
				return this.ConnectionType == 1;
			}
		}

		public bool IsConnectionVirtual
		{
			get
			{
				return this.ConnectionType == 2;
			}
		}

		public bool IsMonochromeLed { get; set; }

		public bool IsMasterAddressPresent
		{
			get
			{
				return this._isMasterAddressPresent;
			}
			set
			{
				if (this._isMasterAddressPresent == value)
				{
					return;
				}
				this._isMasterAddressPresent = value;
				this.OnPropertyChanged("IsMasterAddressPresent");
			}
		}

		public ulong MasterBthAddr
		{
			get
			{
				return this._masterAddress;
			}
			set
			{
				if (this._masterAddress != value)
				{
					this._masterAddress = value;
					this.OnPropertyChanged("MasterBthAddr");
					this.OnPropertyChanged("IsNonInitializedBluetoothPairing");
					this.OnPropertyChanged("BluetoothMasterAddressDescription");
				}
			}
		}

		public bool IsLedPresent
		{
			get
			{
				return this.MaxLedValue > 0;
			}
		}

		public byte MaxLedValue { get; set; }

		public bool IsUserLedsPresent
		{
			get
			{
				return this.NumUserLeds > 0;
			}
		}

		public byte NumUserLeds { get; set; }

		public bool NoBackButtons
		{
			get
			{
				SupportedControllerInfo supportedControllerInfo;
				if (ControllersHelper.SupportedControllersDictionary.TryGetValue(this.ControllerType, out supportedControllerInfo))
				{
					SupportedGamepad supportedGamepad = supportedControllerInfo as SupportedGamepad;
					return supportedGamepad == null || !supportedGamepad.IsBackVisible;
				}
				return false;
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

		public override bool HasEngineMouseTouchpadControllers
		{
			get
			{
				return this.IsEngineMouseTouchpad;
			}
		}

		public override bool HasEngineMouseControllers
		{
			get
			{
				return this.IsEngineMouse;
			}
		}

		public override bool HasEngineControllers
		{
			get
			{
				return this.IsEngineController;
			}
		}

		public bool HasLean
		{
			get
			{
				return !ControllerTypeExtensions.IsFlydigi(this.ControllerType) || (ControllerTypeExtensions.IsFlydigi(this.ControllerType) && base.IsAccelerometerPresent);
			}
		}

		public bool IsCalibrateAllowed
		{
			get
			{
				return !ControllerTypeExtensions.IsAnyEngineGamepad(this.ControllerType);
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
				return !this.HasExclusiveCaptureControllers && base.IsMotorRumbleMotorPresent && base.IsOnline;
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
				return base.IsControllerExclusiveAccessSupported && (this.IsXboxOne || this.IsXboxOneX || this.IsXboxElite);
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
				return this.IsXbox360 && this.ContainerDescription.ToLower().Contains("flydigi");
			}
		}

		public bool IsSonyDualshock3
		{
			get
			{
				return this.ControllerType == 5;
			}
		}

		public bool IsSonyDualshock3Adapter
		{
			get
			{
				return this.ControllerType == 6;
			}
		}

		public bool IsSonyDualshock4
		{
			get
			{
				return this.IsSonyDualshock4USB || this.IsSonyDualshock4BT;
			}
		}

		public bool IsThrustmasterDA4
		{
			get
			{
				return this.ControllerType == 29;
			}
		}

		public bool IsSNES
		{
			get
			{
				return this.ControllerType == 30 || this.ControllerType == 44;
			}
		}

		public bool IsNES
		{
			get
			{
				return this.ControllerType == 47 || this.ControllerType == 48 || this.ControllerType == 45 || this.ControllerType == 46;
			}
		}

		public bool IsRaiju
		{
			get
			{
				return this.ControllerType == 36;
			}
		}

		public bool IsGeneric12bDualTrigger
		{
			get
			{
				return this.ControllerType == 43;
			}
		}

		public bool IsGeneric12b
		{
			get
			{
				return this.ControllerType == 35;
			}
		}

		public bool IsGeneric13b
		{
			get
			{
				return this.ControllerType == 41;
			}
		}

		public bool IsGeneric14b
		{
			get
			{
				return this.ControllerType == 42;
			}
		}

		public bool IsWIINunchuk
		{
			get
			{
				return this.ControllerType == 33;
			}
		}

		public bool IsWIIClassic
		{
			get
			{
				return this.ControllerType == 34;
			}
		}

		public bool IsWIIUPro
		{
			get
			{
				return this.ControllerType == 32;
			}
		}

		public bool IsWIIRemote
		{
			get
			{
				return this.ControllerType == 31;
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

		public bool IsNintendoSwitchPro
		{
			get
			{
				return this.ControllerType == 7;
			}
		}

		public bool IsNintendoSwitchWired
		{
			get
			{
				return this.ControllerType == 8;
			}
		}

		public bool IsGoogleStadia
		{
			get
			{
				return this.ControllerType == 13;
			}
		}

		public bool IsLogitech
		{
			get
			{
				return this.ControllerType == 15;
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

		public bool IsSega
		{
			get
			{
				return this.ControllerType == 50;
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

		public bool IsIpega
		{
			get
			{
				return ControllerTypeExtensions.IsIpega(this.ControllerType);
			}
		}

		public bool IsFlydigiApex1
		{
			get
			{
				return ControllerTypeExtensions.IsFlydigiApex1(this.ControllerType);
			}
		}

		public bool IsFlydigiApex2
		{
			get
			{
				return ControllerTypeExtensions.IsFlydigiApex2(this.ControllerType);
			}
		}

		public bool IsFlydigiVader2
		{
			get
			{
				return ControllerTypeExtensions.IsFlydigiVader2(this.ControllerType);
			}
		}

		public bool IsFlydigiVader2Pro
		{
			get
			{
				return ControllerTypeExtensions.IsFlydigiVader2Pro(this.ControllerType);
			}
		}

		public bool IsFlydigiVader
		{
			get
			{
				return this.IsFlydigiVader2 || this.IsFlydigiVader2Pro;
			}
		}

		public bool IsFlydigiApex3
		{
			get
			{
				return ControllerTypeExtensions.IsFlydigiApex3(this.ControllerType);
			}
		}

		public bool IsFlydigi
		{
			get
			{
				return ControllerTypeExtensions.IsFlydigi(this.ControllerType);
			}
		}

		public bool IsGameSirG7
		{
			get
			{
				return this.ControllerType == 55;
			}
		}

		public bool IsPowerAMOGAXP5AP
		{
			get
			{
				return ControllerTypeExtensions.IsPowerAMOGAXP5AP(this.ControllerType);
			}
		}

		public bool IsSonyPs3Navigation
		{
			get
			{
				return this.ControllerType == 11;
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

		public bool IsEngineControllerControlPad
		{
			get
			{
				return this.ControllerType == 502;
			}
		}

		public bool IsRazerWolverine2
		{
			get
			{
				return ControllerTypeExtensions.IsRazerWolverine2(this.ControllerType);
			}
		}

		public bool IsXboxGameSirG7
		{
			get
			{
				return this.IsCanSwitchControllerToHidMode;
			}
		}

		public bool IsHoriMiniDualshock4
		{
			get
			{
				return ControllerTypeExtensions.IsHoriMiniDualshock4(this.ControllerType);
			}
		}

		public bool IsHoriFightingCommanderOctaAnySonySvg
		{
			get
			{
				return ControllerTypeExtensions.IsHoriFightingCommanderOctaAnySonySvg(this.ControllerType);
			}
		}

		public bool IsNoRumble
		{
			get
			{
				if (base.IsDebugController)
				{
					return this.IsAnyAzeron || this.IsSNES || this.IsThrustmasterDA4 || this.IsRaiju || this.IsPowerAMOGAXP5AP || this.IsIpega || this.IsFlydigiApex1 || this.IsSega || this.IsNES || this.IsEngineGamepad || this.IsHoriMiniDualshock4;
				}
				return !base.IsMotorRumbleMotorPresent || this.IsEngineGamepad;
			}
		}

		public override bool IsNonInitializedElite2
		{
			get
			{
				bool? isInitializedController = this._isInitializedController;
				bool flag = true;
				return !((isInitializedController.GetValueOrDefault() == flag) & (isInitializedController != null)) && this.IsXboxElite2;
			}
		}

		private bool IsVidPidLogitechDirectInput
		{
			get
			{
				return UtilsCommon.IsVidPidLogitechDirectInput(this.VendorId, this.ProductId);
			}
		}

		public override bool IsNonInitializedLogitech
		{
			get
			{
				bool? isInitializedController = this._isInitializedController;
				bool flag = true;
				return !((isInitializedController.GetValueOrDefault() == flag) & (isInitializedController != null)) && this.IsVidPidLogitechDirectInput;
			}
		}

		public override bool IsNonInitializedBluetoothPairing
		{
			get
			{
				bool? isInitializedController = this._isInitializedController;
				bool flag = true;
				return !((isInitializedController.GetValueOrDefault() == flag) & (isInitializedController != null)) && this.IsCanBluetoothPaired;
			}
		}

		public override bool IsNonInitializedFlydigiXbox360
		{
			get
			{
				bool? isInitializedController = this._isInitializedController;
				bool flag = true;
				return !((isInitializedController.GetValueOrDefault() == flag) & (isInitializedController != null)) && this.IsFlydigiXbox360;
			}
		}

		public override bool IsNonInitializedGameCube
		{
			get
			{
				bool? isInitializedController = this._isInitializedController;
				bool flag = true;
				return !((isInitializedController.GetValueOrDefault() == flag) & (isInitializedController != null)) && this.IsGameCubeMayFlash;
			}
		}

		public override bool IsNonInitializedSegaGenesis
		{
			get
			{
				bool? isInitializedController = this._isInitializedController;
				bool flag = true;
				return !((isInitializedController.GetValueOrDefault() == flag) & (isInitializedController != null)) && this.IsSega;
			}
		}

		public override bool IsNonInitialized8BitDo
		{
			get
			{
				bool? isInitializedController = this._isInitializedController;
				bool flag = true;
				return !((isInitializedController.GetValueOrDefault() == flag) & (isInitializedController != null)) && this.Is8BitDo;
			}
		}

		public override bool IsNonInitializedEngineController
		{
			get
			{
				bool? isInitializedController = this._isInitializedController;
				bool flag = true;
				return !((isInitializedController.GetValueOrDefault() == flag) & (isInitializedController != null)) && this.IsPromoController && this.IsEngineController;
			}
		}

		public override bool IsNonInitializedIpega
		{
			get
			{
				bool? isInitializedController = this._isInitializedController;
				bool flag = true;
				return !((isInitializedController.GetValueOrDefault() == flag) & (isInitializedController != null)) && this.IsIpega;
			}
		}

		public override bool IsNonInitializedGameSirG7
		{
			get
			{
				bool? isInitializedController = this._isInitializedController;
				bool flag = true;
				return !((isInitializedController.GetValueOrDefault() == flag) & (isInitializedController != null)) && this.IsXboxGameSirG7 && ControllerVM.IsInformGameSirG7;
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

		public static bool IsInformGameSirG7
		{
			get
			{
				return RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmGameSirG7Mode", 1, false) == 1;
			}
		}

		public static bool IsInformFlidigi
		{
			get
			{
				return RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmFlydigiExclusiveMode", 1, false) == 1;
			}
		}

		public bool IsExclusiveDevice
		{
			get
			{
				return (ControllerVM.IsInformSteam && this.IsAnySteam) || (ControllerVM.IsInformAzeron && this.IsAnyAzeron) || (ControllerVM.IsInformFlidigi && ControllerTypeExtensions.IsFlydigi(this.ControllerType));
			}
		}

		public override bool IsNonInitializedExclusiveDevice
		{
			get
			{
				bool? isInitializedController = this._isInitializedController;
				bool flag = true;
				return !((isInitializedController.GetValueOrDefault() == flag) & (isInitializedController != null)) && this.IsExclusiveDevice;
			}
		}

		private bool IsVidRazer
		{
			get
			{
				return this.VendorId == 5426;
			}
		}

		public virtual bool IsRazer
		{
			get
			{
				return this.IsVidRazer;
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

		public override string BluetoothMasterAddressDescription
		{
			get
			{
				if (this.MasterBthAddr == 0UL)
				{
					return DTLocalization.GetString(12647);
				}
				ulong num;
				return string.Format(DTLocalization.GetString((BluetoothUtils.BluetoothGetLocalRadioAddress(ref num) && num == this.MasterBthAddr) ? 12645 : 12644), UtilsCommon.MacAddressToString(this.MasterBthAddr, "-"));
			}
		}

		public bool IsCanBluetoothPaired
		{
			get
			{
				ulong num;
				return (ControllerTypeExtensions.IsSonyDS3(this.ControllerType) || ControllerTypeExtensions.IsPS3Navigation(this.ControllerType)) && this.IsMasterAddressPresent && BluetoothUtils.BluetoothGetLocalRadioAddress(ref num) && num != this.MasterBthAddr;
			}
		}

		public bool IsSonyBluetoothPaired
		{
			get
			{
				ulong num;
				return (ControllerTypeExtensions.IsSonyDS3(this.ControllerType) || ControllerTypeExtensions.IsPS3Navigation(this.ControllerType)) && this.IsMasterAddressPresent && BluetoothUtils.BluetoothGetLocalRadioAddress(ref num) && num == this.MasterBthAddr;
			}
		}

		[JsonProperty("ControllerType")]
		[JsonConverter(typeof(StringEnumConverter))]
		public ControllerTypeEnum ControllerType { get; set; }

		public bool IsUnknownConnectionType
		{
			get
			{
				return !this.IsConnectionWired && !this.IsConnectionWireless && !this.IsConnectionVirtual;
			}
		}

		public ConnectionType ConnectionType
		{
			get
			{
				return this._connectionType;
			}
			set
			{
				if (this.SetProperty<ConnectionType>(ref this._connectionType, value, "ConnectionType"))
				{
					this.OnPropertyChanged("IsConnectionWired");
					this.OnPropertyChanged("IsConnectionWireless");
					this.OnPropertyChanged("IsConnectionVirtual");
				}
			}
		}

		public bool IsControllerBatteryBlockVisible { get; set; }

		public bool IsControllerAuthAllowed
		{
			get
			{
				return this.IsXboxAuth || ControllerTypeExtensions.IsSonyDS4Auth(this.ControllerType);
			}
		}

		[JsonProperty("ControllerBatteryKind")]
		[JsonConverter(typeof(StringEnumConverter))]
		public BatteryKind ControllerBatteryKind { get; set; }

		[JsonProperty("ControllerBatteryLevel")]
		[JsonConverter(typeof(StringEnumConverter))]
		public BatteryLevel ControllerBatteryLevel
		{
			get
			{
				return this._controllerBatteryLevel;
			}
			set
			{
				this.SetProperty<BatteryLevel>(ref this._controllerBatteryLevel, value, "ControllerBatteryLevel");
			}
		}

		[JsonProperty("IsBatteryLevelPercentPresent")]
		public bool IsBatteryLevelPercentPresent
		{
			get
			{
				return this._isBatteryLevelPercentPresent;
			}
			set
			{
				this.SetProperty<bool>(ref this._isBatteryLevelPercentPresent, value, "IsBatteryLevelPercentPresent");
			}
		}

		[JsonProperty("BatteryLevelPercent")]
		public byte BatteryLevelPercent
		{
			get
			{
				return this._batteryLevelPercent;
			}
			set
			{
				this.SetProperty<byte>(ref this._batteryLevelPercent, value, "BatteryLevelPercent");
			}
		}

		[JsonProperty("ControllerBatteryChargingState")]
		[JsonConverter(typeof(StringEnumConverter))]
		public BatteryChargingState ControllerBatteryChargingState
		{
			get
			{
				return this._controllerBatteryChargingState;
			}
			set
			{
				if (this.SetProperty<BatteryChargingState>(ref this._controllerBatteryChargingState, value, "ControllerBatteryChargingState"))
				{
					this.OnPropertyChanged("IsCharging");
				}
			}
		}

		public override async Task<GamepadState> GetControllerPressedButtons()
		{
			return await base.HttpClientService.Gamepad.GetControllerPressedButtons(this.ID);
		}

		public void ToggleAzeronLefy()
		{
			RegistryHelper.SetValue("Config\\AzeronLefty", this.ID, this.IsAzeronLefty ? 0 : 1);
			this.OnPropertyChanged("IsAzeronLefty");
			this.OnPropertyChanged("MiniGamepadSVGIco");
			((GamepadService)base.GamepadService).RefreshCurrentGamepadSVGs();
		}

		private Drawing GetDrawingForController()
		{
			if (this.IsAzeron && this.IsAzeronLefty)
			{
				return Application.Current.TryFindResource("MiniGamepadAzeronLefty") as Drawing;
			}
			if (this.IsAzeronCyborg && this.IsAzeronLefty)
			{
				return Application.Current.TryFindResource("MiniGamepadAzeronCyborgLefty") as Drawing;
			}
			if (this.IsAzeronCyro && this.IsAzeronLefty)
			{
				return Application.Current.TryFindResource("MiniGamepadAzeronCyroLefty") as Drawing;
			}
			if (this.IsPromoController && this.IsEngineGamepad)
			{
				return Application.Current.TryFindResource("MiniEngineControllerGamepadInvert") as Drawing;
			}
			return XBUtils.GetDrawingForControllerTypeEnum(this.ControllerType, base.IsBluetoothConnectionFlagPresent) ?? (Application.Current.TryFindResource("MiniGamepadUnknown") as Drawing);
		}

		public override void SetIsInitialized(bool value)
		{
			this._isInitializedController = new bool?(value);
			this.OnPropertyChanged("IsNonInitializedElite2");
			this.OnPropertyChanged("IsNonInitializedLogitech");
			this.OnPropertyChanged("IsNonInitializedGameCube");
			this.OnPropertyChanged("IsNonInitializedSegaGenesis");
			this.OnPropertyChanged("IsNonInitialized8BitDo");
			this.OnPropertyChanged("IsNonInitializedEngineController");
			this.OnPropertyChanged("IsNonInitializedBluetoothPairing");
			this.OnPropertyChanged("IsNonInitializedFlydigiXbox360");
			this.OnPropertyChanged("IsNonInitializedExclusiveDevice");
			this.OnPropertyChanged("IsNonInitializedGameSirG7");
			this.OnPropertyChanged("IsInitializedController");
			this.OnPropertyChanged("IsGyroscopePresent");
			this.OnPropertyChanged("IsControllerBatteryBlockVisible");
			IEventAggregator eventAggregator = App.EventAggregator;
			if (eventAggregator == null)
			{
				return;
			}
			eventAggregator.GetEvent<GamepadInitializationChanged>().Publish(this);
		}

		public bool IsCanSwitchControllerToHidMode
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

		public DelegateCommand SwitchControllerToHidModeCommand
		{
			get
			{
				if (this._switchControllerToHidModeCommand == null)
				{
					this._switchControllerToHidModeCommand = new DelegateCommand(new Action(this.SwitchControllerToHidMode), new Func<bool>(this.SwitchControllerToHidModeCommandCanExecute));
				}
				return this._switchControllerToHidModeCommand;
			}
		}

		private bool SwitchControllerToHidModeCommandCanExecute()
		{
			return this.IsCanSwitchControllerToHidMode;
		}

		private async void SwitchControllerToHidMode()
		{
			await base.HttpClientService.Gamepad.SwitchControllerToHidMode(this.ControllerId, this.Type);
		}

		private SupportedGamepad _supportedControllerInfo;

		private bool _isMasterAddressPresent;

		private ulong _masterAddress;

		private ConnectionType _connectionType;

		private BatteryLevel _controllerBatteryLevel;

		private bool _isBatteryLevelPercentPresent;

		private byte _batteryLevelPercent;

		private BatteryChargingState _controllerBatteryChargingState;

		private DelegateCommand _switchControllerToHidModeCommand;
	}
}
