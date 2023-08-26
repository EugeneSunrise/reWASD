using System;
using DiscSoft.NET.Common.Localization;
using reWASDCommon.Infrastructure.Enums;

namespace reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups
{
	public class LeftStickDirectionalGroup : BaseStickDirectionalGroup
	{
		public LeftStickDirectionalGroup(BaseXBBindingCollection baseHostCollection)
			: base(baseHostCollection)
		{
		}

		public override string GroupLabel
		{
			get
			{
				return DTLocalization.GetString(11268);
			}
		}

		public override GamepadButton LeftDirection
		{
			get
			{
				return 42;
			}
		}

		public override GamepadButton UpDirection
		{
			get
			{
				return 40;
			}
		}

		public override GamepadButton RightDirection
		{
			get
			{
				return 43;
			}
		}

		public override GamepadButton DownDirection
		{
			get
			{
				return 41;
			}
		}

		public override GamepadButton UpLeft
		{
			get
			{
				return 175;
			}
		}

		public override GamepadButton UpRight
		{
			get
			{
				return 176;
			}
		}

		public override GamepadButton DownLeft
		{
			get
			{
				return 177;
			}
		}

		public override GamepadButton DownRight
		{
			get
			{
				return 178;
			}
		}

		public override GamepadButton LowZone
		{
			get
			{
				return 37;
			}
		}

		public override GamepadButton MedZone
		{
			get
			{
				return 38;
			}
		}

		public override GamepadButton HighZone
		{
			get
			{
				return 39;
			}
		}

		public override GamepadButton LowZoneUp
		{
			get
			{
				return 109;
			}
		}

		public override GamepadButton LowZoneDown
		{
			get
			{
				return 110;
			}
		}

		public override GamepadButton LowZoneLeft
		{
			get
			{
				return 111;
			}
		}

		public override GamepadButton LowZoneRight
		{
			get
			{
				return 112;
			}
		}

		public override GamepadButton MedZoneUp
		{
			get
			{
				return 113;
			}
		}

		public override GamepadButton MedZoneDown
		{
			get
			{
				return 114;
			}
		}

		public override GamepadButton MedZoneLeft
		{
			get
			{
				return 115;
			}
		}

		public override GamepadButton MedZoneRight
		{
			get
			{
				return 116;
			}
		}

		public override GamepadButton HighZoneUp
		{
			get
			{
				return 117;
			}
		}

		public override GamepadButton HighZoneDown
		{
			get
			{
				return 118;
			}
		}

		public override GamepadButton HighZoneLeft
		{
			get
			{
				return 119;
			}
		}

		public override GamepadButton HighZoneRight
		{
			get
			{
				return 120;
			}
		}

		public override GamepadButton Click
		{
			get
			{
				return 9;
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
	}
}
