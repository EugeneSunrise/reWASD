using System;
using DiscSoft.NET.Common.Localization;
using reWASDCommon.Infrastructure.Enums;

namespace reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups
{
	public class RightStickDirectionalGroup : BaseStickDirectionalGroup
	{
		public RightStickDirectionalGroup(BaseXBBindingCollection baseHostCollection)
			: base(baseHostCollection)
		{
		}

		public override string GroupLabel
		{
			get
			{
				return DTLocalization.GetString(11269);
			}
		}

		public override GamepadButton LeftDirection
		{
			get
			{
				return 49;
			}
		}

		public override GamepadButton UpDirection
		{
			get
			{
				return 47;
			}
		}

		public override GamepadButton RightDirection
		{
			get
			{
				return 50;
			}
		}

		public override GamepadButton DownDirection
		{
			get
			{
				return 48;
			}
		}

		public override GamepadButton UpLeft
		{
			get
			{
				return 179;
			}
		}

		public override GamepadButton UpRight
		{
			get
			{
				return 180;
			}
		}

		public override GamepadButton DownLeft
		{
			get
			{
				return 181;
			}
		}

		public override GamepadButton DownRight
		{
			get
			{
				return 182;
			}
		}

		public override GamepadButton LowZone
		{
			get
			{
				return 44;
			}
		}

		public override GamepadButton MedZone
		{
			get
			{
				return 45;
			}
		}

		public override GamepadButton HighZone
		{
			get
			{
				return 46;
			}
		}

		public override GamepadButton LowZoneUp
		{
			get
			{
				return 121;
			}
		}

		public override GamepadButton LowZoneDown
		{
			get
			{
				return 122;
			}
		}

		public override GamepadButton LowZoneLeft
		{
			get
			{
				return 123;
			}
		}

		public override GamepadButton LowZoneRight
		{
			get
			{
				return 124;
			}
		}

		public override GamepadButton MedZoneUp
		{
			get
			{
				return 125;
			}
		}

		public override GamepadButton MedZoneDown
		{
			get
			{
				return 126;
			}
		}

		public override GamepadButton MedZoneLeft
		{
			get
			{
				return 127;
			}
		}

		public override GamepadButton MedZoneRight
		{
			get
			{
				return 128;
			}
		}

		public override GamepadButton HighZoneUp
		{
			get
			{
				return 129;
			}
		}

		public override GamepadButton HighZoneDown
		{
			get
			{
				return 130;
			}
		}

		public override GamepadButton HighZoneLeft
		{
			get
			{
				return 131;
			}
		}

		public override GamepadButton HighZoneRight
		{
			get
			{
				return 132;
			}
		}

		public override GamepadButton Click
		{
			get
			{
				return 10;
			}
		}

		public override ushort DefaultXLow
		{
			get
			{
				return 8689;
			}
		}

		public override ushort DefaultYLow
		{
			get
			{
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
