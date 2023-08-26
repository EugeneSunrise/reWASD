using System;
using DiscSoft.NET.Common.Localization;
using reWASDCommon.Infrastructure.Enums;

namespace reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups
{
	public class Touchpad2DirectionalGroup : BaseTouchpadDirectionalGroup
	{
		public Touchpad2DirectionalGroup(BaseXBBindingCollection baseHostCollection)
			: base(baseHostCollection)
		{
		}

		public override string GroupLabel
		{
			get
			{
				return DTLocalization.GetString(11942);
			}
		}

		public override bool IsFlickStickGroupMappingAvailable
		{
			get
			{
				return true;
			}
		}

		public override GamepadButton LeftDirection
		{
			get
			{
				return 102;
			}
		}

		public override GamepadButton UpDirection
		{
			get
			{
				return 100;
			}
		}

		public override GamepadButton RightDirection
		{
			get
			{
				return 103;
			}
		}

		public override GamepadButton DownDirection
		{
			get
			{
				return 101;
			}
		}

		public override GamepadButton Click
		{
			get
			{
				return 108;
			}
		}

		public override GamepadButton UpLeft
		{
			get
			{
				return 104;
			}
		}

		public override GamepadButton UpRight
		{
			get
			{
				return 105;
			}
		}

		public override GamepadButton DownLeft
		{
			get
			{
				return 106;
			}
		}

		public override GamepadButton DownRight
		{
			get
			{
				return 107;
			}
		}

		public override GamepadButton Tap
		{
			get
			{
				return 164;
			}
		}

		public override ushort DefaultXLow
		{
			get
			{
				if (!base.TouchpadDigitalMode)
				{
					return 100;
				}
				return 8689;
			}
		}

		public override ushort DefaultYLow
		{
			get
			{
				if (!base.TouchpadDigitalMode)
				{
					return 100;
				}
				return 8689;
			}
		}

		public override ushort DefaultXMed
		{
			get
			{
				return 16715;
			}
		}

		public override ushort DefaultYMed
		{
			get
			{
				return 16715;
			}
		}

		public override ushort DefaultXHigh
		{
			get
			{
				return 24741;
			}
		}

		public override ushort DefaultYHigh
		{
			get
			{
				return 24741;
			}
		}
	}
}
