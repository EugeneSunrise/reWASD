using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoftReWASDServiceNamespace;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Prism.Commands;
using reWASDCommon.Infrastructure;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Network.HTTP.Interfaces.Controllers;
using reWASDEngine;
using reWASDEngine.Utils.Extensions;
using XBEliteWPF.Infrastructure.ExternalDeviceRelations;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtilModel;

namespace XBEliteWPF.Infrastructure.Controller
{
	[JsonObject(1)]
	[JsonConverter(typeof(ControllersJsonConverter))]
	public abstract class BaseControllerVM : IBaseController
	{
		public ushort[] ServiceProfileIDs
		{
			get
			{
				Wrapper<REWASD_CONTROLLER_PROFILE_EX> wrapper = Engine.GamepadService.ServiceProfilesCollection.FindWrapperByID(this.ID);
				if (wrapper == null)
				{
					return new ushort[4];
				}
				return wrapper.Value.ServiceProfileIds;
			}
		}

		protected virtual void OnOnlineChanged()
		{
		}

		[JsonProperty("IsOnline")]
		public bool IsOnline
		{
			get
			{
				return this._isOnline;
			}
			set
			{
				if (value == this._isOnline)
				{
					return;
				}
				this._isOnline = value;
				this.OnOnlineChanged();
				Engine.EventAggregator.GetEvent<ControllerStateChanged>().Publish(this);
			}
		}

		public async Task<Slot> GetCurrentSlot()
		{
			await this.RefreshCurrentSlot();
			return this.CurrentSlot;
		}

		[JsonProperty("RemapState")]
		[JsonConverter(typeof(StringEnumConverter))]
		public RemapState RemapState
		{
			get
			{
				return Engine.GamepadService.GetRemapState(this.ID);
			}
		}

		[JsonProperty("VirtualGamepadType")]
		[JsonConverter(typeof(StringEnumConverter))]
		public VirtualGamepadType? VirtualGamepadType
		{
			get
			{
				return Engine.GamepadService.GetVirtualGamepadType(this);
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
		public Slot CurrentSlot { get; set; }

		public async Task RefreshCurrentSlot()
		{
			REWASD_CONTROLLER_PROFILE_EX profileEx = Engine.GamepadService.GetProfileEx(this.ID);
			if (profileEx != null)
			{
				for (int i = 0; i < 4; i++)
				{
					ServiceResponseWrapper<REWASD_GET_PROFILE_STATE_RESPONSE> serviceResponseWrapper = await Engine.XBServiceCommunicator.GetProfileState(profileEx.ServiceProfileIds[i], false);
					if (serviceResponseWrapper.ServiceResponse.Enabled && !REWASD_GET_PROFILE_STATE_RESPONSE_Extension.IsSlotManagerProfile(serviceResponseWrapper.ServiceResponse))
					{
						this.CurrentSlot = REWASD_GET_PROFILE_STATE_RESPONSE_Extension.GetSlot(serviceResponseWrapper.ServiceResponse);
						break;
					}
				}
			}
			else
			{
				if (Engine.UserSettingsService.RestoreXBOXEliteSlot)
				{
					Slot? slot = await this.GetActivePhyscialSlot();
					if (slot != null)
					{
						this.CurrentSlot = slot.Value;
						return;
					}
				}
				try
				{
					this.CurrentSlot = RegistryHelper.GetValue("Controllers\\" + this.ShortID, "CurrentSlot", 0, false);
				}
				catch (Exception)
				{
				}
			}
		}

		[JsonProperty("IsPromoController")]
		public bool IsPromoController { get; set; }

		public bool IsSlot1
		{
			get
			{
				return this.CurrentSlot == 0;
			}
		}

		public bool IsSlot2
		{
			get
			{
				return this.CurrentSlot == 1;
			}
		}

		public bool IsSlot3
		{
			get
			{
				return this.CurrentSlot == 2;
			}
		}

		public bool IsSlot4
		{
			get
			{
				return this.CurrentSlot == 3;
			}
		}

		public abstract Drawing MiniGamepadSVGIco { get; }

		[JsonProperty("IsAmiiboApplied")]
		public bool IsAmiiboApplied
		{
			get
			{
				REWASD_CONTROLLER_PROFILE_EX profileEx = Engine.GamepadService.GetProfileEx(this.ID);
				if (profileEx == null)
				{
					return false;
				}
				REWASD_CONTROLLER_PROFILE_EX.AmiiboWrapper[] amiibo = profileEx.Amiibo;
				bool? flag;
				if (amiibo == null)
				{
					flag = null;
				}
				else
				{
					flag = new bool?(amiibo.Any((REWASD_CONTROLLER_PROFILE_EX.AmiiboWrapper item) => item.AmiiboLoaded));
				}
				bool? flag2 = flag;
				bool flag3 = true;
				return (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
			}
		}

		[JsonProperty("IsBluetoothConnectionFlagPresent")]
		public abstract bool IsBluetoothConnectionFlagPresent { get; }

		[JsonProperty("IsAnalogTriggersPresent")]
		public abstract bool IsAnalogTriggersPresent { get; }

		[JsonProperty("IsTriggersFullPullPresent")]
		public abstract bool IsTriggersFullPullPresent { get; }

		[JsonProperty("IsMotorRumbleMotorPresent")]
		public abstract bool IsMotorRumbleMotorPresent { get; }

		[JsonProperty("IsTriggerRumbleMotorPresent")]
		public abstract bool IsTriggerRumbleMotorPresent { get; }

		[JsonProperty("IsAdaptiveTriggersPresent")]
		public abstract bool IsAdaptiveTriggersPresent { get; }

		[JsonProperty("IsSteamExtendedPresent")]
		public abstract bool IsSteamExtendedPresent { get; }

		[JsonProperty("IsPowerManagementPresent")]
		public abstract bool IsPowerManagementPresent { get; }

		[JsonProperty("IsAccelerometerPresent")]
		public abstract bool IsAccelerometerPresent { get; }

		[JsonProperty("IsGyroscopePresent")]
		public abstract bool IsGyroscopePresent { get; }

		[JsonProperty("IsTouchpadPresent")]
		public abstract bool IsTouchpadPresent { get; }

		[JsonProperty("IsTiltMode2")]
		public abstract bool IsTiltMode2 { get; }

		[JsonProperty("IsControllerExclusiveAccessSupported")]
		public abstract bool IsControllerExclusiveAccessSupported { get; }

		[JsonProperty("IsRightHandDevice")]
		public abstract bool IsRightHandDevice { get; }

		[JsonProperty("ID")]
		public abstract string ID { get; set; }

		[JsonProperty("ShortID")]
		public abstract string ShortID { get; }

		[JsonProperty("HintID")]
		public abstract string HintID { get; }

		[JsonProperty("IsDebugController")]
		public bool IsDebugController { get; set; }

		public bool IsUnpairAllowed
		{
			get
			{
				return false;
			}
		}

		[JsonProperty("Types")]
		public abstract uint[] Types { get; set; }

		[JsonProperty("Ids")]
		public abstract ulong[] Ids { get; set; }

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
				return !this.IsInsideCompositeDevice && this.IsInitializedController;
			}
		}

		public abstract Guid ContainerId { get; set; }

		[JsonProperty("ContainerIdString")]
		public string ContainerIdString
		{
			get
			{
				return this.ContainerId.ToString();
			}
			set
			{
				this.ContainerId = Guid.Parse(value);
			}
		}

		[JsonProperty("IsNintendoSwitchJoyConL")]
		public abstract bool IsNintendoSwitchJoyConL { get; }

		[JsonProperty("IsNintendoSwitchJoyConR")]
		public abstract bool IsNintendoSwitchJoyConR { get; }

		[JsonProperty("IsNintendoSwitchJoyConComposite")]
		public abstract bool IsNintendoSwitchJoyConComposite { get; }

		[JsonProperty("IsUnknownControllerType")]
		public abstract bool IsUnknownControllerType { get; }

		[JsonProperty("IsInvalidControllerType")]
		public abstract bool IsInvalidControllerType { get; }

		[JsonProperty("IsUnsupportedControllerType")]
		public abstract bool IsUnsupportedControllerType { get; }

		[JsonProperty("CanReinitializeController")]
		public virtual bool CanReinitializeController
		{
			get
			{
				return false;
			}
		}

		[JsonProperty("InitializedDeviceType")]
		public string InitializedDeviceType { get; set; }

		[JsonProperty("IsInitializedController")]
		public virtual bool IsInitializedController
		{
			get
			{
				return !this.IsNonInitializedPeripheralController && !this.IsNonInitializedElite2 && !this.IsNonInitializedSegaGenesis && !this.IsNonInitializedLogitech && !this.IsNonInitializedBluetoothPairing && !this.IsNonInitializedGameCube && !this.IsNonInitializedFlydigiXbox360 && !this.IsNonInitialized8BitDo && !this.IsNonInitializedEngineController && !this.IsNonInitializedExclusiveDevice && !this.IsNonInitializedChatpad && !this.IsNonInitializedGameSirG7;
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

		public virtual bool IsNonInitializedSegaGenesis
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

		public virtual bool IsNonInitializedFlydigiXbox360
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

		public virtual bool IsNonInitializedExclusiveDevice
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

		public virtual bool IsNonInitializedChatpad
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

		public abstract ControllerVM CurrentController { get; set; }

		public abstract bool UpdateControllerInfo(REWASD_CONTROLLER_INFO controllerInfo);

		public abstract void UpdateControllerState(REWASD_GET_CONTROLLER_STATE_RESPONSE controllerState);

		public abstract Task<GamepadState> GetControllerPressedButtons();

		public abstract Task<bool> GetIsControllerPressedButton();

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
		public virtual bool HasAnyEngineControllers
		{
			get
			{
				return false;
			}
		}

		[JsonProperty("HasGamepadControllers")]
		public virtual bool HasGamepadControllers
		{
			get
			{
				return false;
			}
		}

		[JsonProperty("HasGamepadControllersWithFictiveButtons")]
		public virtual bool HasGamepadControllersWithFictiveButtons
		{
			get
			{
				return false;
			}
		}

		[JsonProperty("HasGamepadVibrateControllers")]
		public virtual bool HasGamepadVibrateControllers
		{
			get
			{
				return false;
			}
		}

		[JsonProperty("HasOnlineGamepadVibrateControllers")]
		public virtual bool HasOnlineGamepadVibrateControllers
		{
			get
			{
				return false;
			}
		}

		[JsonProperty("HasKeyboardControllers")]
		public virtual bool HasKeyboardControllers
		{
			get
			{
				return false;
			}
		}

		[JsonProperty("HasAnyKeyboardControllers")]
		public virtual bool HasAnyKeyboardControllers
		{
			get
			{
				return false;
			}
		}

		[JsonProperty("HasChatpadControllers")]
		public virtual bool HasChatpadControllers
		{
			get
			{
				return false;
			}
		}

		[JsonProperty("HasMouseControllers")]
		public virtual bool HasMouseControllers
		{
			get
			{
				return false;
			}
		}

		[JsonProperty("HasMouseControllersWithKeypad")]
		public virtual bool HasMouseControllersWithKeypad
		{
			get
			{
				return false;
			}
		}

		[JsonProperty("HasMouseControllersWithAnyKeypad")]
		public virtual bool HasMouseControllersWithAnyKeypad
		{
			get
			{
				return false;
			}
		}

		[JsonProperty("HasConsumerControllers")]
		public virtual bool HasConsumerControllers
		{
			get
			{
				return false;
			}
		}

		public virtual bool HasGamepadControllersWithBuiltInAnyKeypad
		{
			get
			{
				return false;
			}
		}

		[JsonProperty("HasSystemControllers")]
		public virtual bool HasSystemControllers
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

		[JsonProperty("HasLED")]
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

		[JsonProperty("IsInsideCompositeDevice")]
		public bool IsInsideCompositeDevice { get; set; }

		public bool IsRemapInProgress { get; set; }

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

		protected abstract void Vibrate();

		public BaseControllerVM(bool isDebugController = false)
		{
			this.CurrentSlot = 0;
			this.IsDebugController = isDebugController;
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

		public virtual void SetIsInitialized(bool value)
		{
		}

		[JsonProperty("IsCustomControllerFriendlyName")]
		public bool IsCustomControllerFriendlyName { get; set; }

		[JsonProperty("ControllerFriendlyName")]
		public string ControllerFriendlyName
		{
			get
			{
				return this._controllerFriendlyName;
			}
			set
			{
				if (this._controllerFriendlyName != value)
				{
					this._controllerFriendlyName = value;
					this.UpdateFriendlyName();
				}
			}
		}

		public void FillFriendlyName()
		{
			this._controllerFriendlyName = XBUtils.GetControllerSavedFriendlyName(this.ShortID);
		}

		public void UpdateFriendlyName()
		{
			if (string.IsNullOrEmpty(this.ID))
			{
				return;
			}
			XBUtils.SetControllerSavedFriendlyName(this.ShortID, this._controllerFriendlyName);
			string newName = (string.IsNullOrEmpty(this._controllerFriendlyName) ? this.ControllerDisplayName : this._controllerFriendlyName);
			if (!(this is CompositeControllerVM))
			{
				Engine.GamepadService.GamepadsUserLedCollection.AddGamepad(this.ID, this.Ids, this.Types, newName);
			}
			if (!(this is ControllerVM) || (this.ControllerFamily != 1 && this.ControllerFamily != 2))
			{
				Engine.GamepadService.GamepadsHotkeyCollection.AddDefaultEntryIfNotPresent(this.ID, this.Types, this.ControllerFamily, newName, this.Ids);
			}
			Engine.GamepadService.GamepadsSettings.ForEach(delegate(GamepadSettings item)
			{
				if (item.ID == this.ID)
				{
					item.DisplayName = newName;
				}
			});
			ExternalDeviceRelationsHelper externalDeviceRelationsHelper = Engine.GamepadService.ExternalDeviceRelationsHelper;
			if (externalDeviceRelationsHelper == null)
			{
				return;
			}
			externalDeviceRelationsHelper.CurrentGamepadsAuthCollection.ForEach(delegate(GamepadAuth item)
			{
				if (item.ID == this.ID)
				{
					item.DisplayName = newName;
				}
			});
		}

		[JsonProperty("ControllerTypeFriendlyName")]
		public abstract string ControllerTypeFriendlyName { get; set; }

		[JsonProperty("ControllerDisplayName")]
		public virtual string ControllerDisplayName
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

		public virtual Task<Slot?> GetActivePhyscialSlot()
		{
			return Task.FromResult<Slot?>(null);
		}

		private bool _isOnline;

		private DelegateCommand _VibrateCommand;

		protected string _controllerFriendlyName;
	}
}
