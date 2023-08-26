using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using DiscSoftReWASDServiceNamespace;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Prism.Commands;
using Prism.Ioc;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Network.HTTP.Interfaces.Controllers;
using reWASDUI.DataModels;
using reWASDUI.DataModels.CompositeDevicesCollection;
using reWASDUI.Services;
using reWASDUI.Services.Interfaces;
using reWASDUI.Utils.XBUtil;
using reWASDUI.ViewModels;
using reWASDUI.Views.SecondaryWindows;
using reWASDUI.Views.SecondaryWindows.CalibrateGyroscope;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.License.Licensing.ComStructures;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Infrastructure.Controller
{
	public abstract class BaseControllerVM : ZBindableBase, IBaseController, IEquatable<BaseControllerVM>
	{
		public IGameProfilesService GameProfilesService { get; set; }

		public IHttpClientService HttpClientService { get; set; }

		public IGamepadService GamepadService { get; set; }

		public IDeviceDetectionService DeviceDetectionService { get; set; }

		public IUserSettingsService UserSettingsService { get; set; }

		public async Task<Slot> GetCurrentSlot()
		{
			await this.RefreshCurrentSlot();
			return this._currentSlot;
		}

		public event BaseControllerVM.RemapStateChangedDelegate OnRemapStateChanged;

		[JsonProperty("RemapState")]
		[JsonConverter(typeof(StringEnumConverter))]
		public RemapState RemapState
		{
			get
			{
				return this._remapState;
			}
			set
			{
				if (this.SetProperty<RemapState>(ref this._remapState, value, "RemapState"))
				{
					BaseControllerVM.RemapStateChangedDelegate onRemapStateChanged = this.OnRemapStateChanged;
					if (onRemapStateChanged == null)
					{
						return;
					}
					onRemapStateChanged();
				}
			}
		}

		[JsonProperty("VirtualGamepadType")]
		[JsonConverter(typeof(StringEnumConverter))]
		public VirtualGamepadType? VirtualGamepadType
		{
			get
			{
				return this._virtualGamepadType;
			}
			set
			{
				this.SetProperty<VirtualGamepadType?>(ref this._virtualGamepadType, value, "VirtualGamepadType");
			}
		}

		[JsonProperty("DataType")]
		public string DataType
		{
			get
			{
				return base.GetType().Name;
			}
		}

		[JsonProperty("CurrentSlot")]
		[JsonConverter(typeof(StringEnumConverter))]
		public Slot CurrentSlot
		{
			get
			{
				return this._currentSlot;
			}
			set
			{
				if (this.SetProperty<Slot>(ref this._currentSlot, value, "CurrentSlot"))
				{
					this.OnPropertyChanged("IsSlot1");
					this.OnPropertyChanged("IsSlot2");
					this.OnPropertyChanged("IsSlot3");
					this.OnPropertyChanged("IsSlot4");
				}
			}
		}

		public async Task RefreshCurrentSlot()
		{
			Slot slot = await App.HttpClientService.Gamepad.GetCurrentSlot(this.ID);
			this._currentSlot = slot;
		}

		public bool IsSlot1
		{
			get
			{
				return this._currentSlot == 0;
			}
		}

		public bool IsSlot2
		{
			get
			{
				return this._currentSlot == 1;
			}
		}

		public bool IsSlot3
		{
			get
			{
				return this._currentSlot == 2;
			}
		}

		public bool IsSlot4
		{
			get
			{
				return this._currentSlot == 3;
			}
		}

		public abstract Drawing MiniGamepadSVGIco { get; }

		public bool IsAmiiboApplied
		{
			get
			{
				return this._isAmiiboApplied;
			}
			set
			{
				this.SetProperty<bool>(ref this._isAmiiboApplied, value, "IsAmiiboApplied");
			}
		}

		public bool IsBluetoothConnectionFlagPresent
		{
			get
			{
				return this._isBluetoothConnectionFlagPresent;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isBluetoothConnectionFlagPresent, value, "IsBluetoothConnectionFlagPresent"))
				{
					this.OnPropertyChanged("MiniGamepadSVGIco");
					this.OnPropertyChanged("IsUnpairAllowed");
					this.UnpairCommand.RaiseCanExecuteChanged();
				}
			}
		}

		public bool IsAnalogTriggersPresent
		{
			get
			{
				return this._isAnalogTriggersPresent;
			}
			set
			{
				this.SetProperty<bool>(ref this._isAnalogTriggersPresent, value, "IsAnalogTriggersPresent");
			}
		}

		public bool IsTriggerRumbleMotorPresent
		{
			get
			{
				return this._isTriggerRumbleMotorPresent;
			}
			set
			{
				this.SetProperty<bool>(ref this._isTriggerRumbleMotorPresent, value, "IsTriggerRumbleMotorPresent");
			}
		}

		public bool IsAdaptiveTriggersPresent
		{
			get
			{
				return this._isAdaptiveTriggersPresent;
			}
			set
			{
				this.SetProperty<bool>(ref this._isAdaptiveTriggersPresent, value, "IsAdaptiveTriggersPresent");
			}
		}

		public bool IsAccelerometerPresent
		{
			get
			{
				return this._isAccelerometerPresent;
			}
			set
			{
				this.SetProperty<bool>(ref this._isAccelerometerPresent, value, "IsAccelerometerPresent");
			}
		}

		public virtual bool IsGyroscopePresent
		{
			get
			{
				return this._isGyroscopePresent;
			}
			set
			{
				this.SetProperty<bool>(ref this._isGyroscopePresent, value, "IsGyroscopePresent");
			}
		}

		public bool IsTouchpadPresent
		{
			get
			{
				return this._isTouchpadPresent;
			}
			set
			{
				this.SetProperty<bool>(ref this._isTouchpadPresent, value, "IsTouchpadPresent");
			}
		}

		public bool IsTriggersFullPullPresent
		{
			get
			{
				return this._isTriggersFullPullPresent;
			}
			set
			{
				this.SetProperty<bool>(ref this._isTriggersFullPullPresent, value, "IsTriggersFullPullPresent");
			}
		}

		public bool IsMotorRumbleMotorPresent
		{
			get
			{
				return this._isMotorRumbleMotorPresent;
			}
			set
			{
				this.SetProperty<bool>(ref this._isMotorRumbleMotorPresent, value, "IsMotorRumbleMotorPresent");
			}
		}

		public bool IsSteamExtendedPresent
		{
			get
			{
				return this._isSteamExtendedPresent;
			}
			set
			{
				this.SetProperty<bool>(ref this._isSteamExtendedPresent, value, "IsSteamExtendedPresent");
			}
		}

		public bool IsPowerManagementPresent
		{
			get
			{
				return this._isPowerManagementPresent;
			}
			set
			{
				this.SetProperty<bool>(ref this._isPowerManagementPresent, value, "IsPowerManagementPresent");
			}
		}

		public bool IsTiltMode2
		{
			get
			{
				return this._isTiltMode2;
			}
			set
			{
				this.SetProperty<bool>(ref this._isTiltMode2, value, "IsTiltMode2");
			}
		}

		public bool IsControllerExclusiveAccessSupported
		{
			get
			{
				return this._isControllerExclusiveAccessSupported;
			}
			set
			{
				this.SetProperty<bool>(ref this._isControllerExclusiveAccessSupported, value, "IsControllerExclusiveAccessSupported");
			}
		}

		public bool IsRightHandDevice
		{
			get
			{
				return this._isRightHandDevice;
			}
			set
			{
				this.SetProperty<bool>(ref this._isRightHandDevice, value, "IsRightHandDevice");
			}
		}

		protected virtual void OnOnlineChanged()
		{
		}

		public bool IsOnline
		{
			get
			{
				return this._isOnline;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isOnline, value, "IsOnline"))
				{
					this.OnOnlineChanged();
				}
			}
		}

		[JsonProperty("ID")]
		public virtual string ID { get; set; }

		[JsonProperty("ShortID")]
		public virtual string ShortID { get; set; }

		[JsonProperty("HintID")]
		public virtual string HintID { get; set; }

		[JsonProperty("IsDebugController")]
		public bool IsDebugController { get; set; }

		[JsonProperty("IsPromoController")]
		public virtual bool IsPromoController { get; set; }

		public string HintVIdPId
		{
			get
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 2);
				defaultInterpolatedStringHandler.AppendLiteral("VID:PID: ");
				ControllerVM currentController = this.CurrentController;
				defaultInterpolatedStringHandler.AppendFormatted<ushort?>((currentController != null) ? new ushort?(currentController.VendorId) : null, "X4");
				defaultInterpolatedStringHandler.AppendLiteral(":");
				ControllerVM currentController2 = this.CurrentController;
				defaultInterpolatedStringHandler.AppendFormatted<ushort?>((currentController2 != null) ? new ushort?(currentController2.ProductId) : null, "X4");
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}
		}

		public string FirmwareVersion
		{
			get
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 4);
				ControllerVM currentController = this.CurrentController;
				defaultInterpolatedStringHandler.AppendFormatted<ushort?>((currentController != null) ? new ushort?(currentController.FirmwareVersionMajor) : null);
				defaultInterpolatedStringHandler.AppendLiteral(".");
				ControllerVM currentController2 = this.CurrentController;
				defaultInterpolatedStringHandler.AppendFormatted<ushort?>((currentController2 != null) ? new ushort?(currentController2.FirmwareVersionMinor) : null);
				defaultInterpolatedStringHandler.AppendLiteral(".");
				ControllerVM currentController3 = this.CurrentController;
				defaultInterpolatedStringHandler.AppendFormatted<ushort?>((currentController3 != null) ? new ushort?(currentController3.FirmwareVersionBuild) : null);
				defaultInterpolatedStringHandler.AppendLiteral(".");
				ControllerVM currentController4 = this.CurrentController;
				defaultInterpolatedStringHandler.AppendFormatted<ushort?>((currentController4 != null) ? new ushort?(currentController4.FirmwareVersionRevision) : null);
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}
		}

		public virtual string BluetoothMasterAddressDescription
		{
			get
			{
				return "";
			}
		}

		public bool FirmwareVersionVisible
		{
			get
			{
				if (!this.IsCompositeDevice)
				{
					ControllerVM currentController = this.CurrentController;
					if (currentController != null && currentController.IsOnline)
					{
						ControllerVM currentController2 = this.CurrentController;
						bool flag;
						if (currentController2 == null)
						{
							flag = false;
						}
						else
						{
							ControllerTypeEnum? controllerTypeEnum;
							bool? flag2 = ((currentController2.FirstGamepadType != null) ? new bool?(ControllerTypeExtensions.IsXboxEliteOrOne(controllerTypeEnum.GetValueOrDefault())) : null);
							bool flag3 = true;
							flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
						}
						if (flag)
						{
							ControllerVM currentController3 = this.CurrentController;
							ushort? num = ((currentController3 != null) ? new ushort?(currentController3.FirmwareVersionMajor) : null);
							int? num2 = ((num != null) ? new int?((int)num.GetValueOrDefault()) : null);
							int num3 = 0;
							if ((num2.GetValueOrDefault() == num3) & (num2 != null))
							{
								ControllerVM currentController4 = this.CurrentController;
								num = ((currentController4 != null) ? new ushort?(currentController4.FirmwareVersionMinor) : null);
								num2 = ((num != null) ? new int?((int)num.GetValueOrDefault()) : null);
								num3 = 0;
								if ((num2.GetValueOrDefault() == num3) & (num2 != null))
								{
									ControllerVM currentController5 = this.CurrentController;
									num = ((currentController5 != null) ? new ushort?(currentController5.FirmwareVersionBuild) : null);
									num2 = ((num != null) ? new int?((int)num.GetValueOrDefault()) : null);
									num3 = 0;
									if ((num2.GetValueOrDefault() == num3) & (num2 != null))
									{
										ControllerVM currentController6 = this.CurrentController;
										num = ((currentController6 != null) ? new ushort?(currentController6.FirmwareVersionRevision) : null);
										num2 = ((num != null) ? new int?((int)num.GetValueOrDefault()) : null);
										num3 = 0;
										return !((num2.GetValueOrDefault() == num3) & (num2 != null));
									}
								}
							}
							return true;
						}
					}
				}
				return false;
			}
		}

		public bool IsUnpairAllowed
		{
			get
			{
				ControllerVM controllerVM = this as ControllerVM;
				return controllerVM != null && !this.IsBluetoothConnectionFlagPresent && controllerVM.IsSonyBluetoothPaired;
			}
		}

		[JsonProperty("Types")]
		public uint[] Types { get; set; }

		[JsonProperty("Ids")]
		public ulong[] Ids { get; set; }

		[JsonProperty("ControllerTypeEnums")]
		public ControllerTypeEnum[] ControllerTypeEnums
		{
			get
			{
				return ControllerTypeExtensions.ConvertPhysicalTypesToEnums(0, this.Types, this.Ids);
			}
		}

		public uint FirstType
		{
			get
			{
				if (this.Types.Length == 0)
				{
					return 0U;
				}
				return this.Types[0];
			}
		}

		public ulong FirstId
		{
			get
			{
				if (this.Ids.Length == 0)
				{
					return 0UL;
				}
				return this.Ids[0];
			}
		}

		[JsonProperty("FirstControllerType")]
		[JsonConverter(typeof(StringEnumConverter))]
		public ControllerTypeEnum FirstControllerType
		{
			get
			{
				return ControllerTypeExtensions.ConvertPhysicalTypeToEnum(0, this.FirstType, this.FirstId);
			}
		}

		[JsonProperty("FirstGamepadType")]
		[JsonConverter(typeof(StringEnumConverter))]
		public ControllerTypeEnum? FirstGamepadType
		{
			get
			{
				foreach (ControllerTypeEnum controllerTypeEnum in this.ControllerTypeEnums)
				{
					if (ControllerTypeExtensions.IsGamepad(controllerTypeEnum))
					{
						return new ControllerTypeEnum?(controllerTypeEnum);
					}
				}
				return null;
			}
		}

		[JsonProperty("ControllerFamily")]
		public virtual ControllerFamily ControllerFamily
		{
			get
			{
				return ControllerTypeExtensions.GetControllerFamily(this.FirstControllerType);
			}
		}

		public virtual ControllerFamily TreatAsControllerFamily
		{
			get
			{
				return this.ControllerFamily;
			}
		}

		public virtual List<BaseControllerVM> ControllersForMaskFilter
		{
			get
			{
				return new List<BaseControllerVM> { this };
			}
		}

		public virtual bool IsAvailiableForComposition
		{
			get
			{
				return !this.IsInsideCompositeDevice && !this.IsPromoController && (this.IsInitializedController || this.HasExclusiveCaptureControllers);
			}
		}

		public Guid ContainerId { get; set; }

		public string ContainerIdString { get; set; }

		public bool IsNintendoSwitchJoyConL
		{
			get
			{
				return this._isNintendoSwitchJoyConL;
			}
			set
			{
				this.SetProperty<bool>(ref this._isNintendoSwitchJoyConL, value, "IsNintendoSwitchJoyConL");
			}
		}

		public bool IsNintendoSwitchJoyConR
		{
			get
			{
				return this._isNintendoSwitchJoyConR;
			}
			set
			{
				this.SetProperty<bool>(ref this._isNintendoSwitchJoyConR, value, "IsNintendoSwitchJoyConR");
			}
		}

		public bool IsNintendoSwitchJoyConComposite
		{
			get
			{
				return this._isNintendoSwitchJoyConComposite;
			}
			set
			{
				this.SetProperty<bool>(ref this._isNintendoSwitchJoyConComposite, value, "IsNintendoSwitchJoyConComposite");
			}
		}

		public bool IsUnknownControllerType
		{
			get
			{
				return this._isUnknownControllerType;
			}
			set
			{
				this.SetProperty<bool>(ref this._isUnknownControllerType, value, "IsUnknownControllerType");
			}
		}

		public bool IsInvalidControllerType
		{
			get
			{
				return this._isInvalidControllerType;
			}
			set
			{
				this.SetProperty<bool>(ref this._isInvalidControllerType, value, "IsInvalidControllerType");
			}
		}

		public bool IsUnsupportedControllerType
		{
			get
			{
				return this._isUnsupportedControllerType;
			}
			set
			{
				this.SetProperty<bool>(ref this._isUnsupportedControllerType, value, "IsUnsupportedControllerType");
			}
		}

		[JsonProperty("CanReinitializeController")]
		public virtual bool CanReinitializeController
		{
			get
			{
				return this._canReinitializeController;
			}
			set
			{
				this._canReinitializeController = value;
				this.ReinitializeCommand.RaiseCanExecuteChanged();
			}
		}

		[JsonProperty("InitializedDeviceType")]
		public string InitializedDeviceType { get; set; }

		[JsonProperty("IsInitializedController")]
		public bool IsInitializedController
		{
			get
			{
				if (this.IsNonInitializedExclusiveDevice || this.IsNonInitializedGameSirG7)
				{
					return false;
				}
				bool? isInitializedController = this._isInitializedController;
				bool flag = true;
				return (isInitializedController.GetValueOrDefault() == flag) & (isInitializedController != null);
			}
			set
			{
				this._isInitializedController = new bool?(value);
				this.SetIsInitialized(value);
			}
		}

		public virtual bool IsNonInitializedPeripheralController
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsNonInitializedElite2
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsNonInitializedLogitech
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsNonInitialized8BitDo
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsNonInitializedEngineController
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsNonInitializedGameCube
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsNonInitializedSegaGenesis
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsNonInitializedExclusiveDevice
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsNonInitializedChatpad
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsNonInitializedFlydigiXbox360
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsNonInitializedBluetoothPairing
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsNonInitializedIpega
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsNonInitializedGameSirG7
		{
			get
			{
				return false;
			}
		}

		[JsonProperty("IsApplyForbidden")]
		public bool IsApplyForbidden
		{
			get
			{
				return this.IsUnknownControllerType || this.IsInvalidControllerType || this.IsUnsupportedControllerType || this.IsNonInitializedPeripheralController || this.IsNonInitialized8BitDo;
			}
		}

		public bool IsAnyOfForbidden
		{
			get
			{
				return this.IsUnknownControllerType || this.IsInvalidControllerType || this.IsUnsupportedControllerType;
			}
		}

		public abstract ControllerVM CurrentController { get; set; }

		public abstract void UpdateControllerInfo();

		public abstract void UpdateControllerState(REWASD_GET_CONTROLLER_STATE_RESPONSE controllerState);

		public abstract Task<GamepadState> GetControllerPressedButtons();

		public bool IsControllerPressedButton
		{
			get
			{
				return this._isControllerPressedButton;
			}
			set
			{
				this.SetProperty<bool>(ref this._isControllerPressedButton, value, "IsControllerPressedButton");
			}
		}

		public void CheckControllerPressedButton()
		{
			if (!this.IsOnline || this.IsDebugController)
			{
				return;
			}
			Application application = Application.Current;
			if (application == null)
			{
				return;
			}
			application.Dispatcher.InvokeAsync<Task<bool>>(async delegate
			{
				bool flag = await this.HttpClientService.Gamepad.IsControllerPressedButton(this.ID);
				return this.IsControllerPressedButton = flag;
			});
		}

		public bool IsSimpleDevice
		{
			get
			{
				return this is ControllerVM;
			}
		}

		public bool IsCompositeDevice
		{
			get
			{
				return this is CompositeControllerVM;
			}
		}

		public bool IsPeripheralDevice
		{
			get
			{
				return this is PeripheralVM;
			}
		}

		public bool IsTreatAsSingleDevice
		{
			get
			{
				return this.IsSimpleDevice || this.IsPeripheralDevice || this.IsNintendoSwitchJoyConComposite;
			}
		}

		[JsonProperty("HasAnyEngineControllers")]
		public virtual bool HasAnyEngineControllers { get; set; }

		[JsonProperty("HasGamepadControllers")]
		public virtual bool HasGamepadControllers { get; set; }

		[JsonProperty("HasGamepadControllersWithFictiveButtons")]
		public virtual bool HasGamepadControllersWithFictiveButtons { get; set; }

		[JsonProperty("HasGamepadVibrateControllers")]
		public virtual bool HasGamepadVibrateControllers { get; set; }

		[JsonProperty("HasOnlineGamepadVibrateControllers")]
		public virtual bool HasOnlineGamepadVibrateControllers { get; set; }

		[JsonProperty("HasKeyboardControllers")]
		public virtual bool HasKeyboardControllers { get; set; }

		[JsonProperty("HasAnyKeyboardControllers")]
		public virtual bool HasAnyKeyboardControllers { get; set; }

		[JsonProperty("HasChatpadControllers")]
		public virtual bool HasChatpadControllers { get; set; }

		[JsonProperty("HasMouseControllers")]
		public virtual bool HasMouseControllers { get; set; }

		[JsonProperty("HasMouseControllersWithKeypad")]
		public virtual bool HasMouseControllersWithKeypad { get; set; }

		[JsonProperty("HasMouseControllersWithAnyKeypad")]
		public virtual bool HasMouseControllersWithAnyKeypad { get; set; }

		[JsonProperty("HasConsumerControllers")]
		public virtual bool HasConsumerControllers { get; set; }

		[JsonProperty("HasGamepadControllersWithBuiltInAnyKeypad")]
		public virtual bool HasGamepadControllersWithBuiltInAnyKeypad { get; set; }

		[JsonProperty("HasSystemControllers")]
		public virtual bool HasSystemControllers { get; set; }

		public virtual bool HasEngineMouseControllers
		{
			get
			{
				return false;
			}
		}

		public virtual bool HasEngineMouseTouchpadControllers
		{
			get
			{
				return false;
			}
		}

		public virtual bool HasEngineControllers
		{
			get
			{
				return false;
			}
		}

		public virtual bool HasXboxElite
		{
			get
			{
				return false;
			}
		}

		public virtual bool HasXboxElite2
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsEngineMouseKeyboardComposite
		{
			get
			{
				return false;
			}
		}

		public virtual bool HasExclusiveCaptureControllers
		{
			get
			{
				return false;
			}
		}

		public virtual bool HasTouchpad
		{
			get
			{
				return false;
			}
		}

		public bool HasLED
		{
			get
			{
				return this.HasLEDGamepad || this.HasLEDKeyboard || this.HasLEDMouse || this.HasAnyEngineControllers;
			}
		}

		public virtual bool HasLEDGamepad
		{
			get
			{
				return false;
			}
		}

		public virtual bool HasLEDKeyboard
		{
			get
			{
				return false;
			}
		}

		public virtual bool HasLEDMouse
		{
			get
			{
				return false;
			}
		}

		public virtual int ConsistsOfControllersNumber
		{
			get
			{
				return 1;
			}
		}

		public bool IsLicenseLockEngineController
		{
			get
			{
				BaseControllerVM currentGamepad = this.GamepadService.CurrentGamepad;
				if (((currentGamepad != null) ? currentGamepad.CurrentController : null) != null && !App.LicensingService.IsMobileControllerFeatureUnlocked)
				{
					BaseControllerVM currentGamepad2 = this.GamepadService.CurrentGamepad;
					return currentGamepad2 != null && currentGamepad2.HasEngineControllers;
				}
				return false;
			}
		}

		[JsonProperty("IsInsideCompositeDevice")]
		public bool IsInsideCompositeDevice
		{
			get
			{
				return this._isInsideCompositeDevice;
			}
			set
			{
				this.SetProperty<bool>(ref this._isInsideCompositeDevice, value, "IsInsideCompositeDevice");
			}
		}

		public DelegateCommand VibrateCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._VibrateCommand) == null)
				{
					delegateCommand = (this._VibrateCommand = new DelegateCommand(new Action(this.Vibrate)));
				}
				return delegateCommand;
			}
		}

		public virtual void VibrateForce()
		{
		}

		protected abstract void Vibrate();

		public ObservableCollection<GamepadButton> GamepadMaskButtons
		{
			get
			{
				return App.KeyBindingService.GenerateButtonsForController(this);
			}
		}

		public ObservableCollection<GamepadButtonDescription> GamepadUnmapableButtonDescriptions
		{
			get
			{
				return App.KeyBindingService.GenerateRemapButtonDescriptionsForController(this, true, null);
			}
		}

		public ObservableCollection<GamepadButtonDescription> GamepadRemapButtonDescriptions
		{
			get
			{
				return App.KeyBindingService.GenerateRemapButtonDescriptionsForController(this, false, null);
			}
		}

		public ObservableCollection<KeyScanCodeV2> KeyScanCodes
		{
			get
			{
				return App.KeyBindingService.GenerateKeysForController(this, false, false, false, false);
			}
		}

		public ObservableCollection<KeyScanCodeV2> KeyScanCodesForMask
		{
			get
			{
				return App.KeyBindingService.GenerateKeysForController(this, false, false, true, false);
			}
		}

		public BaseControllerVM()
		{
			this.GameProfilesService = App.GameProfilesService;
			this.GamepadService = App.GamepadService;
			this.DeviceDetectionService = App.DeviceDetectionService;
			this.UserSettingsService = App.UserSettingsService;
			this.HttpClientService = App.HttpClientService;
			this._currentSlot = 0;
			TranslationManager.Instance.LanguageChanged += delegate([Nullable(2)] object sender, EventArgs args)
			{
				this.OnPropertyChanged("ControllerDisplayName");
			};
			App.EventAggregator.GetEvent<ControllerStateChanged>().Subscribe(delegate(BaseControllerVM item)
			{
				this.OnPropertyChanged("HasOnlineGamepadVibrateControllers");
			});
			App.EventAggregator.GetEvent<CurrentGamepadChanged>().Subscribe(delegate(BaseControllerVM item)
			{
				this.OnPropertyChanged("IsLicenseLockEngineController");
			});
			App.LicensingService.OnLicenseChangedCompleted += delegate(LicenseCheckResult result, bool activation)
			{
				this.LicenseChanged();
			};
		}

		private void LicenseChanged()
		{
			this.OnPropertyChanged("IsLicenseLockEngineController");
		}

		public virtual void Init()
		{
		}

		public virtual void SetIsInitialized(bool value)
		{
		}

		public virtual int GetControllerFamilyCount(ControllerFamily controllerFamily)
		{
			if (this.ControllerFamily != controllerFamily)
			{
				return 0;
			}
			return 1;
		}

		public virtual ControllerTypeEnum? GetGamepadTypeByIndex(int index)
		{
			return this.FirstGamepadType;
		}

		public virtual List<LEDSupportedDevice> GetLEDSupportedDevicesEnum()
		{
			List<LEDSupportedDevice> list = new List<LEDSupportedDevice>();
			ControllerTypeEnum[] controllerTypeEnums = this.ControllerTypeEnums;
			for (int i = 0; i < controllerTypeEnums.Length; i++)
			{
				LEDSupportedDevice? ledsupportedDevice = ControllerTypeExtensions.ConvertEnumToLEDSupportedType(controllerTypeEnums[i]);
				if (ledsupportedDevice != null)
				{
					list.AddIfNotContains(ledsupportedDevice.Value);
				}
			}
			return list;
		}

		public abstract List<BaseControllerVM> GetLEDSupportedControllers();

		public List<ControllerVM> GetLEDSupportedGamepads()
		{
			List<ControllerVM> list = new List<ControllerVM>();
			foreach (BaseControllerVM baseControllerVM in this.GetLEDSupportedControllers())
			{
				ControllerTypeEnum[] controllerTypeEnums = baseControllerVM.ControllerTypeEnums;
				for (int i = 0; i < controllerTypeEnums.Length; i++)
				{
					LEDSupportedDevice? ledsupportedDevice = ControllerTypeExtensions.ConvertEnumToLEDSupportedType(controllerTypeEnums[i]);
					if (ledsupportedDevice != null && LEDSupportedDeviceExtensions.IsGamepad(ledsupportedDevice.GetValueOrDefault()))
					{
						list.AddIfNotContains(baseControllerVM.CurrentController);
						break;
					}
				}
			}
			return list;
		}

		public virtual bool IsConsideredTheSameControllerByID(string id)
		{
			return !string.IsNullOrEmpty(id) && this.ID.Contains(id);
		}

		public virtual bool IsConsideredTheSameControllerByID(ulong[] IDs)
		{
			return IDs.Any((ulong id) => id != 0UL && this.ID.Contains(id.ToString()));
		}

		public virtual void SetCurrentSlot(Slot newSlot)
		{
			this.CurrentSlot = newSlot;
		}

		[JsonProperty("ControllerFriendlyName")]
		public string JSONControllerFriendlyName
		{
			get
			{
				return this._controllerFriendlyName;
			}
			set
			{
				this.SetProperty<string>(ref this._controllerFriendlyName, value, "ControllerFriendlyName");
			}
		}

		[JsonProperty("IsCustomControllerFriendlyName")]
		public bool IsCustomControllerFriendlyName { get; set; }

		[JsonProperty("GuiControllerFriendlyName")]
		public string ControllerFriendlyName
		{
			get
			{
				if (this.CurrentController != null)
				{
					if (this.CurrentController.IsInvalidControllerType)
					{
						return string.Format(DTLocalization.GetString(11802), "https://www.rewasd.com/#contacts");
					}
					if (this.CurrentController.IsUnknownControllerType)
					{
						return string.Format(DTLocalization.GetString(11803), XBUtils.GetControllerRequestLink(this.CurrentController));
					}
				}
				return this._controllerFriendlyName;
			}
			set
			{
				if (this.SetProperty<string>(ref this._controllerFriendlyName, (value == null) ? "" : value.Trim(), "ControllerFriendlyName"))
				{
					this.UpdateFriendlyName();
				}
			}
		}

		public void FillFriendlyName()
		{
			this.OnPropertyChanged("ControllerFriendlyName");
		}

		public async void UpdateFriendlyName()
		{
			await App.HttpClientService.Gamepad.SetControllerFriendlyName(this.ID, this._controllerFriendlyName);
			await App.GamepadService.BinDataSerialize.LoadGamepadsSlotHotkeyCollection();
		}

		public string ControllerTypeFriendlyName { get; set; }

		public string ControllerDisplayName
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(this.ControllerFriendlyName))
				{
					return this.ControllerFriendlyName;
				}
				return this.ControllerTypeFriendlyName;
			}
		}

		public void UpdateGuiProperties()
		{
			this.OnPropertyChanged("MiniGamepadSVGIco");
			this.OnPropertyChanged("HasLED");
			this.OnPropertyChanged("IsNonInitializedPeripheralController");
			this.OnPropertyChanged("IsInsideCompositeDevice");
			this.OnPropertyChanged("ControllersForMaskFilter");
			this.OnPropertyChanged("ControllerDisplayName");
			this.OnPropertyChanged("HasOnlineGamepadVibrateControllers");
			this.OnPropertyChanged("IsUnpairAllowed");
			this.UnpairCommand.RaiseCanExecuteChanged();
			this.Init();
		}

		public override bool Equals(object obj)
		{
			BaseControllerVM baseControllerVM = obj as BaseControllerVM;
			return (obj == null || !(obj.GetType() != base.GetType())) && this.Equals(baseControllerVM);
		}

		public bool Equals(BaseControllerVM other)
		{
			return other != null && (this == other || this.ID == other.ID);
		}

		public override int GetHashCode()
		{
			return this.ID.GetHashCode();
		}

		public GamepadSelectorVM GamepadSelectorVM
		{
			get
			{
				return IContainerProviderExtensions.Resolve<GamepadSelectorVM>(App.Container);
			}
		}

		public DelegateCommand ShowCompositeDevicesWindowCommand
		{
			get
			{
				if (this._ShowCompositeDevicesWindow == null)
				{
					this._ShowCompositeDevicesWindow = new DelegateCommand(new Action(this.ShowCompositeDevicesWindow), new Func<bool>(this.ShowCompositeDevicesCanExecute));
					this.GamepadService.GamepadCollection.CollectionChanged += delegate([Nullable(2)] object sender, NotifyCollectionChangedEventArgs args)
					{
						this.ShowCompositeDevicesWindowCommand.RaiseCanExecuteChanged();
					};
					App.EventAggregator.GetEvent<GamepadInitializationChanged>().Subscribe(delegate(BaseControllerVM item)
					{
						this.ShowCompositeDevicesWindowCommand.RaiseCanExecuteChanged();
					});
				}
				return this._ShowCompositeDevicesWindow;
			}
		}

		private void ShowCompositeDevicesWindow()
		{
			new CompositeDevicesWindow(this.GamepadService as GamepadService, this).ShowDialog();
		}

		private bool ShowCompositeDevicesCanExecute()
		{
			return this.IsInitializedController && !this.IsUnknownControllerType && !this.IsUnsupportedControllerType && (this.GamepadService.ControllersAvailiableForComposition.Count > 1 || this.IsCompositeDevice);
		}

		public DelegateCommand CalibrateGyroCommand
		{
			get
			{
				if (this._CalibrateGyro == null)
				{
					this._CalibrateGyro = new DelegateCommand(new Action(this.CalibrateGyro), new Func<bool>(this.CalibrateDeviceGyroCanExecute));
					App.EventAggregator.GetEvent<ControllerStateChanged>().Subscribe(delegate(BaseControllerVM o)
					{
						this._CalibrateGyro.RaiseCanExecuteChanged();
					});
					this.GameProfilesService.CurrentGameChanged += async delegate(object o, PropertyChangedExtendedEventArgs<GameVM> e)
					{
						await Task.Delay(300);
						this._CalibrateGyro.RaiseCanExecuteChanged();
					};
					this.GameProfilesService.CurrentGameProfileChanged += async delegate(object o, PropertyChangedExtendedEventArgs<ConfigVM> e)
					{
						await Task.Delay(300);
						this._CalibrateGyro.RaiseCanExecuteChanged();
					};
				}
				return this._CalibrateGyro;
			}
		}

		private bool CalibrateDeviceGyroCanExecute()
		{
			ControllerTypeEnum? controllerTypeEnum;
			if (this.FirstGamepadType != null && ControllerTypeExtensions.IsAnySteam(controllerTypeEnum.GetValueOrDefault()))
			{
				IGameProfilesService gameProfilesService = this.GameProfilesService;
				bool flag;
				if (gameProfilesService == null)
				{
					flag = null != null;
				}
				else
				{
					GameVM currentGame = gameProfilesService.CurrentGame;
					flag = ((currentGame != null) ? currentGame.CurrentConfig : null) != null;
				}
				return flag && this.GameProfilesService.CurrentGame.CurrentConfig.IsLoaded && this.IsGyroscopePresent;
			}
			return this.IsGyroscopePresent && this.IsOnline;
		}

		private void CalibrateGyro()
		{
			GyroWizard gyroWizard = new GyroWizard(this);
			gyroWizard.ShowDialog();
			if (gyroWizard.WindowResult == MessageBoxResult.OK)
			{
				this.OnPropertyChanged("CalibrateGyro");
			}
		}

		public DelegateCommand UnpairCommand
		{
			get
			{
				if (this._Unpair == null)
				{
					this._Unpair = new DelegateCommand(new Action(this.Unpair), new Func<bool>(this.UnpairCanExecute));
				}
				return this._Unpair;
			}
		}

		private bool UnpairCanExecute()
		{
			return this.IsUnpairAllowed;
		}

		private async void Unpair()
		{
			if (DTMessageBox.Show(DTLocalization.GetString(12643), "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				uint firstType = this.FirstType;
				await App.HttpClientService.Gamepad.ControllerChangeMasterAddress(this.GamepadService.CurrentGamepad.Ids[0], firstType, 0UL);
				this.Reinitialize();
				this.UnpairCommand.RaiseCanExecuteChanged();
				this.OnPropertyChanged("IsUnpairAllowed");
				this.OnPropertyChanged("IsNonInitializedBluetoothPairing");
			}
		}

		public DelegateCommand AzeronLeftyCommand
		{
			get
			{
				if (this._azeronLeftyCommand == null)
				{
					this._azeronLeftyCommand = new DelegateCommand(new Action(this.AzeronLefty));
				}
				return this._azeronLeftyCommand;
			}
		}

		private void AzeronLefty()
		{
			ControllerVM controllerVM = this as ControllerVM;
			if (controllerVM != null)
			{
				controllerVM.ToggleAzeronLefy();
			}
		}

		public DelegateCommand ReinitializeCommand
		{
			get
			{
				if (this._reinitializeCommand == null)
				{
					this._reinitializeCommand = new DelegateCommand(new Action(this.Reinitialize), new Func<bool>(this.ReinitializeCommandCanExecute));
				}
				return this._reinitializeCommand;
			}
		}

		private async void Reinitialize()
		{
			await this.HttpClientService.Gamepad.Reinitialize(this.ID);
			await this.GamepadService.BinDataSerialize.LoadPeripheralDevicesCollection();
			await this.GamepadService.SetCurrentGamepad(this);
			this.ReinitializeCommand.RaiseCanExecuteChanged();
		}

		private bool ReinitializeCommandCanExecute()
		{
			return this.CanReinitializeController;
		}

		public DelegateCommand CopyDeviceIdCommand
		{
			get
			{
				if (this._copyDeviceIdCommand == null)
				{
					this._copyDeviceIdCommand = new DelegateCommand(new Action(this.CopyDeviceId), new Func<bool>(this.CopyDeviceIdCommandCanExecute));
					App.EventAggregator.GetEvent<GamepadInitializationChanged>().Subscribe(delegate(BaseControllerVM item)
					{
						this.CopyDeviceIdCommand.RaiseCanExecuteChanged();
					});
				}
				return this._copyDeviceIdCommand;
			}
		}

		private void CopyDeviceId()
		{
			Clipboard.SetText(this.ID);
		}

		private bool CopyDeviceIdCommandCanExecute()
		{
			return !this.IsApplyForbidden;
		}

		public DelegateCommand RemoveUnitlReconnectCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._removeUnitlReconnect) == null)
				{
					delegateCommand = (this._removeUnitlReconnect = new DelegateCommand(new Action(this.RemoveUnitlReconnect)));
				}
				return delegateCommand;
			}
		}

		private async void RemoveUnitlReconnect()
		{
			CompositeControllerVM compositeVM = this as CompositeControllerVM;
			if (compositeVM != null)
			{
				CompositeDevice compositeDevice = this.GamepadService.CompositeDevices.FirstOrDefault((CompositeDevice item) => item.ID == compositeVM.ID);
				if (compositeDevice != null)
				{
					compositeDevice.Remove();
				}
			}
			else
			{
				await this.HttpClientService.Gamepad.RemoveOfflineGamepad(this.ID);
			}
		}

		public DelegateCommand RemoveCompositeCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._removeComposite) == null)
				{
					delegateCommand = (this._removeComposite = new DelegateCommand(new Action(this.RemoveComposite)));
				}
				return delegateCommand;
			}
		}

		private void RemoveComposite()
		{
			if (this.IsCompositeDevice)
			{
				CompositeDevice compositeDevice = App.GamepadService.CompositeDevices.FirstOrDefault((CompositeDevice g) => g.ID.Contains(this.ID));
				if (compositeDevice == null)
				{
					return;
				}
				compositeDevice.RemoveCommand.Execute();
			}
		}

		public DelegateCommand DoNotShowInRewasdCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._doNotShowInRewasd) == null)
				{
					delegateCommand = (this._doNotShowInRewasd = new DelegateCommand(new Action(this.DoNotShowInRewasd)));
				}
				return delegateCommand;
			}
		}

		private void DoNotShowInRewasd()
		{
			if (DTMessageBox.Show(DTLocalization.GetString(11212), MessageBoxButton.YesNo, MessageBoxImage.Question, null, false, MessageBoxResult.None) == MessageBoxResult.Yes)
			{
				(this.GamepadService as GamepadService).AddGamepadToBlacklist(this);
			}
		}

		public DelegateCommand EditControllerNameCommand
		{
			get
			{
				if (this._editControllerName == null)
				{
					this._editControllerName = new DelegateCommand(new Action(this.EditControllerName), new Func<bool>(this.EditControllerNameCanExecute));
					App.EventAggregator.GetEvent<CurrentGamepadChanged>().Subscribe(delegate(BaseControllerVM item)
					{
						this.EditControllerNameCommand.RaiseCanExecuteChanged();
					});
				}
				return this._editControllerName;
			}
		}

		private void EditControllerName()
		{
			this.GamepadSelectorVM.EditControllerNameCommand.Execute();
		}

		private bool EditControllerNameCanExecute()
		{
			return this == this.GamepadService.CurrentGamepad;
		}

		private Slot _currentSlot;

		private bool _isInsideCompositeDevice;

		private RemapState _remapState;

		private VirtualGamepadType? _virtualGamepadType;

		private bool _isAmiiboApplied;

		private bool _isBluetoothConnectionFlagPresent;

		private bool _isAnalogTriggersPresent;

		private bool _isTriggerRumbleMotorPresent;

		private bool _isAdaptiveTriggersPresent;

		private bool _isAccelerometerPresent;

		private bool _isGyroscopePresent;

		private bool _isTouchpadPresent;

		private bool _isTriggersFullPullPresent;

		private bool _isMotorRumbleMotorPresent;

		private bool _isSteamExtendedPresent;

		private bool _isPowerManagementPresent;

		private bool _isTiltMode2;

		private bool _isControllerExclusiveAccessSupported;

		private bool _isRightHandDevice;

		private bool _isOnline;

		private bool _isNintendoSwitchJoyConL;

		private bool _isNintendoSwitchJoyConR;

		private bool _isNintendoSwitchJoyConComposite;

		private bool _isUnknownControllerType;

		private bool _isInvalidControllerType;

		private bool _isUnsupportedControllerType;

		private bool _canReinitializeController;

		protected bool? _isInitializedController = new bool?(false);

		private bool _isControllerPressedButton;

		private DelegateCommand _VibrateCommand;

		private string _controllerFriendlyName;

		private DelegateCommand _ShowCompositeDevicesWindow;

		private DelegateCommand _CalibrateGyro;

		private DelegateCommand _Unpair;

		private DelegateCommand _azeronLeftyCommand;

		private DelegateCommand _reinitializeCommand;

		private DelegateCommand _copyDeviceIdCommand;

		private DelegateCommand _removeUnitlReconnect;

		private DelegateCommand _removeComposite;

		private DelegateCommand _doNotShowInRewasd;

		private DelegateCommand _editControllerName;

		public delegate void RemapStateChangedDelegate();
	}
}
