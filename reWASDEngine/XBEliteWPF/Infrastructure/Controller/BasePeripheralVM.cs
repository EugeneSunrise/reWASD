using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using reWASDCommon.Infrastructure.Enums;

namespace XBEliteWPF.Infrastructure.Controller
{
	public abstract class BasePeripheralVM : BaseCompositeControllerVM
	{
		[JsonProperty("PeripheralPhysicalType")]
		[JsonConverter(typeof(StringEnumConverter))]
		public PeripheralPhysicalType PeripheralPhysicalType
		{
			get
			{
				return this._peripheralPhysicalType;
			}
			set
			{
				if (this._peripheralPhysicalType != value)
				{
					this._peripheralPhysicalType = value;
					if (value == 1)
					{
						List<ControllerVM> controllers = base.Controllers;
						ControllerVM controllerVM;
						if (controllers == null)
						{
							controllerVM = null;
						}
						else
						{
							controllerVM = controllers.FirstOrDefault((ControllerVM c) => c.IsKeyboard);
						}
						this.CurrentController = controllerVM;
						return;
					}
					if (value != 2)
					{
						return;
					}
					List<ControllerVM> controllers2 = base.Controllers;
					ControllerVM controllerVM2;
					if (controllers2 == null)
					{
						controllerVM2 = null;
					}
					else
					{
						controllerVM2 = controllers2.FirstOrDefault((ControllerVM c) => c.IsMouse);
					}
					this.CurrentController = controllerVM2;
				}
			}
		}

		[JsonProperty("PeripheralType")]
		[JsonConverter(typeof(StringEnumConverter))]
		public PeripheralType PeripheralType { get; set; }

		protected BasePeripheralVM(bool isDebugController = false)
			: base(isDebugController)
		{
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

		public override bool IsRightHandDevice
		{
			get
			{
				return false;
			}
		}

		[JsonProperty("IsNonInitializedPeripheralController")]
		public override bool IsNonInitializedPeripheralController
		{
			get
			{
				return this.PeripheralPhysicalType == 0;
			}
		}

		protected override void Vibrate()
		{
		}

		public override Task<GamepadState> GetControllerPressedButtons()
		{
			return Task.FromResult<GamepadState>(new GamepadState());
		}

		private PeripheralPhysicalType _peripheralPhysicalType;
	}
}
