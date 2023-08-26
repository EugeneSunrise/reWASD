using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoftReWASDServiceNamespace;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.DataModels.ControllerProfileInfo;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtilModel;

namespace XBEliteWPF.Infrastructure.Controller
{
	public class OfflineDeviceVM : ControllerVM
	{
		public ControllerProfileInfoCollection ControllerProfileInfoCollection { get; set; }

		public OfflineDeviceVM(ControllerProfileInfoCollection controllerProfileInfoCollection)
			: base(default(REWASD_CONTROLLER_INFO), false)
		{
			this.ControllerProfileInfoCollection = controllerProfileInfoCollection;
		}

		public override string ID
		{
			get
			{
				if (this.ControllerProfileInfoCollection != null)
				{
					return IControllerProfileInfoCollectionContainerExtensions.CalculateID(this.ControllerProfileInfoCollection);
				}
				return "";
			}
		}

		public override Drawing MiniGamepadSVGIco
		{
			get
			{
				return null;
			}
		}

		public override bool IsBluetoothConnectionFlagPresent
		{
			get
			{
				return false;
			}
		}

		public override bool IsAnalogTriggersPresent
		{
			get
			{
				return false;
			}
		}

		public override bool IsTriggersFullPullPresent
		{
			get
			{
				return false;
			}
		}

		public override bool IsMotorRumbleMotorPresent
		{
			get
			{
				return false;
			}
		}

		public override bool IsTriggerRumbleMotorPresent
		{
			get
			{
				return false;
			}
		}

		public override bool IsAdaptiveTriggersPresent
		{
			get
			{
				return false;
			}
		}

		public override bool IsSteamExtendedPresent
		{
			get
			{
				return false;
			}
		}

		public override bool IsPowerManagementPresent
		{
			get
			{
				return false;
			}
		}

		public override bool IsAccelerometerPresent
		{
			get
			{
				return false;
			}
		}

		public override bool IsGyroscopePresent
		{
			get
			{
				return false;
			}
		}

		public override bool IsTouchpadPresent
		{
			get
			{
				return false;
			}
		}

		public override bool IsTiltMode2
		{
			get
			{
				return false;
			}
		}

		public override bool IsControllerExclusiveAccessSupported
		{
			get
			{
				return false;
			}
		}

		public bool IsOfflineDevice
		{
			get
			{
				return true;
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
				return "";
			}
		}

		public override uint[] Types
		{
			get
			{
				return this.ControllerProfileInfoCollection.ControllerProfileInfos.Select((ControllerProfileInfo item) => ControllerTypeExtensions.ConvertEnumToPhysicalType(item.ControllerType)).ToArray<uint>();
			}
		}

		public override ulong[] Ids
		{
			get
			{
				ulong[] array = new ulong[15];
				array[0] = this.ControllerProfileInfoCollection.ControllerProfileInfos[0].Id;
				return array;
			}
		}

		public override Guid ContainerId
		{
			get
			{
				return Guid.Empty;
			}
			set
			{
			}
		}

		public override bool IsNintendoSwitchJoyConL
		{
			get
			{
				return false;
			}
		}

		public override bool IsNintendoSwitchJoyConR
		{
			get
			{
				return false;
			}
		}

		public override bool IsNintendoSwitchJoyConComposite
		{
			get
			{
				return false;
			}
		}

		public override bool IsUnknownControllerType
		{
			get
			{
				return false;
			}
		}

		public override bool IsInvalidControllerType
		{
			get
			{
				return false;
			}
		}

		public override bool IsUnsupportedControllerType
		{
			get
			{
				return false;
			}
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

		public override Task<Slot?> GetActivePhyscialSlot()
		{
			return Task.FromResult<Slot?>(null);
		}

		public override string ControllerTypeFriendlyName
		{
			get
			{
				string controllerSavedFriendlyName = XBUtils.GetControllerSavedFriendlyName(this.ShortID);
				if (string.IsNullOrEmpty(controllerSavedFriendlyName))
				{
					return this.ControllerProfileInfoCollection.ControllerProfileInfos[0].ControllerType.TryGetDescription();
				}
				return controllerSavedFriendlyName;
			}
		}

		public override ControllerTypeEnum ControllerType
		{
			get
			{
				return this.ControllerProfileInfoCollection.ControllerProfileInfos[0].ControllerType;
			}
		}

		public override Task<GamepadState> GetControllerPressedButtons()
		{
			return Task.FromResult<GamepadState>(new GamepadState());
		}

		public override Task<bool> GetIsControllerPressedButton()
		{
			return Task.FromResult<bool>(false);
		}

		public override List<BaseControllerVM> GetLEDSupportedControllers()
		{
			throw new NotImplementedException();
		}

		public override bool UpdateControllerInfo(REWASD_CONTROLLER_INFO controllerInfo)
		{
			return false;
		}

		public override void UpdateControllerState(REWASD_GET_CONTROLLER_STATE_RESPONSE controllerState)
		{
		}

		protected override void Vibrate()
		{
		}
	}
}
