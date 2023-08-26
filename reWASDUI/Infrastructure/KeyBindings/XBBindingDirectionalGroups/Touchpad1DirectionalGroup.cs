using System;
using DiscSoft.NET.Common.Localization;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups
{
	public class Touchpad1DirectionalGroup : BaseTouchpadDirectionalGroup
	{
		public Touchpad1DirectionalGroup(BaseXBBindingCollection baseHostCollection)
			: base(baseHostCollection)
		{
		}

		private bool IsDS4OrDualSense
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum = base.CurrentControllerType;
				if (controllerTypeEnum == null || !ControllerTypeExtensions.IsGamepadWithSonyTouchpad(controllerTypeEnum.GetValueOrDefault()))
				{
					controllerTypeEnum = base.CurrentControllerType;
					return controllerTypeEnum != null && ControllerTypeExtensions.IsAnyEngineGamepad(controllerTypeEnum.GetValueOrDefault());
				}
				return true;
			}
		}

		public override bool IsVirtualLeftStickGroupMappingAvaliable
		{
			get
			{
				return !this.IsDS4OrDualSense;
			}
		}

		public override bool IsVirtualRightStickGroupMappingAvaliable
		{
			get
			{
				return !this.IsDS4OrDualSense;
			}
		}

		public override bool IsDS4TouchpadGroupMappingAvailable
		{
			get
			{
				return !this.IsDS4OrDualSense;
			}
		}

		public override bool IsUnmapAvailable
		{
			get
			{
				return !this.IsDS4OrDualSense;
			}
		}

		public override bool IsFlickStickGroupMappingAvailable
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				return base.CurrentControllerType != null && ControllerTypeExtensions.IsAnySteam(controllerTypeEnum.GetValueOrDefault());
			}
		}

		public override string GroupLabel
		{
			get
			{
				if (!this.IsDS4OrDualSense)
				{
					return DTLocalization.GetString(11941);
				}
				return DTLocalization.GetString(11686);
			}
		}

		public override bool AdvancedDigitalMappingSettingsExist
		{
			get
			{
				return !this.IsDS4OrDualSense && base.TouchpadDigitalMode;
			}
		}

		public override string AdvancedDigitalMappingSettingsDisabledTooltip
		{
			get
			{
				if (!this.IsDS4OrDualSense)
				{
					return base.AdvancedDigitalMappingSettingsDisabledTooltip;
				}
				return DTLocalization.GetString(11979);
			}
		}

		public override GamepadButton LeftDirection
		{
			get
			{
				return 93;
			}
		}

		public override GamepadButton UpDirection
		{
			get
			{
				return 91;
			}
		}

		public override GamepadButton RightDirection
		{
			get
			{
				return 94;
			}
		}

		public override GamepadButton DownDirection
		{
			get
			{
				return 92;
			}
		}

		public override GamepadButton Click
		{
			get
			{
				return 99;
			}
		}

		public override GamepadButton UpLeft
		{
			get
			{
				return 95;
			}
		}

		public override GamepadButton UpRight
		{
			get
			{
				return 96;
			}
		}

		public override GamepadButton DownLeft
		{
			get
			{
				return 97;
			}
		}

		public override GamepadButton DownRight
		{
			get
			{
				return 98;
			}
		}

		public override GamepadButton Tap
		{
			get
			{
				return 163;
			}
		}

		public override ushort DefaultXLow
		{
			get
			{
				return 7849;
			}
		}

		public override ushort DefaultYLow
		{
			get
			{
				return 7849;
			}
		}

		public override ushort DefaultXMed
		{
			get
			{
				return 16155;
			}
		}

		public override ushort DefaultYMed
		{
			get
			{
				return 16155;
			}
		}

		public override ushort DefaultXHigh
		{
			get
			{
				return 24461;
			}
		}

		public override ushort DefaultYHigh
		{
			get
			{
				return 24461;
			}
		}

		public override bool DefaultTouchpadAnalogMode
		{
			get
			{
				return false;
			}
		}

		public override bool DefaultTouchpadZoneClickRequired
		{
			get
			{
				return !this.IsDS4OrDualSense;
			}
		}
	}
}
