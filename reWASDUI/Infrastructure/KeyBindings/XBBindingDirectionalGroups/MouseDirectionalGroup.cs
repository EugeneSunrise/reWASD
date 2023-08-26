using System;
using DiscSoft.NET.Common.Localization;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;

namespace reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups
{
	public class MouseDirectionalGroup : BaseDirectionalAnalogGroup
	{
		public MouseDirectionalGroup(BaseXBBindingCollection baseHostCollection)
			: base(baseHostCollection)
		{
		}

		public override string GroupLabel
		{
			get
			{
				return DTLocalization.GetString(11324);
			}
		}

		public override string GroupMapToLabel
		{
			get
			{
				return DTLocalization.GetString(11616);
			}
		}

		public override string ZonesLabelAxisX
		{
			get
			{
				return DTLocalization.GetString(11351);
			}
		}

		public override bool IsHardwareChangesAllowed
		{
			get
			{
				return false;
			}
		}

		public override bool IsZoneRangeInherited
		{
			get
			{
				return false;
			}
		}

		public override bool IsZonesAllowed
		{
			get
			{
				return base.SpringMode;
			}
		}

		public override bool IsZonesDirectionAllowed
		{
			get
			{
				return true;
			}
		}

		public override bool IsZoneLowAllowed
		{
			get
			{
				return false;
			}
		}

		public override bool IsZoneMedAllowed
		{
			get
			{
				return false;
			}
		}

		public override bool IsZoneHighAllowed
		{
			get
			{
				return true;
			}
		}

		public override bool IsZoneLeanAllowed
		{
			get
			{
				return false;
			}
		}

		public override bool IsZonesShapeChangeAllowed
		{
			get
			{
				return false;
			}
		}

		public override bool IsRadialDeadzoneAllowed
		{
			get
			{
				return false;
			}
		}

		public override bool IsSpringModeAllowed
		{
			get
			{
				return true;
			}
		}

		public override bool IsAdvancedSpringDeadzoneVisible
		{
			get
			{
				return true;
			}
		}

		public override bool IsMouseYSensitivityAvailable
		{
			get
			{
				return true;
			}
		}

		public override bool IsMouseSmoothingAvailable
		{
			get
			{
				return false;
			}
		}

		public override bool IsStickCrossZonesChangeAvailable
		{
			get
			{
				return false;
			}
		}

		public override bool IsStickCrossZonesInherited
		{
			get
			{
				return false;
			}
		}

		public override string ZonesExplanation
		{
			get
			{
				return DTLocalization.GetString(11617);
			}
		}

		public override bool IsXInvert
		{
			get
			{
				return (base.LeftDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonLeftStickRight || base.LeftDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonRightStickRight) && (base.RightDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonLeftStickLeft || base.RightDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonRightStickLeft);
			}
		}

		public override bool IsYInvert
		{
			get
			{
				return (base.UpDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonLeftStickDown || base.UpDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonRightStickDown) && (base.DownDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonLeftStickUp || base.DownDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonRightStickUp);
			}
		}

		public override GamepadButton LeftDirection
		{
			get
			{
				return 66;
			}
		}

		public override GamepadButton UpDirection
		{
			get
			{
				return 64;
			}
		}

		public override GamepadButton RightDirection
		{
			get
			{
				return 67;
			}
		}

		public override GamepadButton DownDirection
		{
			get
			{
				return 65;
			}
		}

		public override GamepadButton LowZone
		{
			get
			{
				return 61;
			}
		}

		public override GamepadButton MedZone
		{
			get
			{
				return 62;
			}
		}

		public override GamepadButton HighZone
		{
			get
			{
				return 63;
			}
		}

		public override GamepadButton LowZoneUp
		{
			get
			{
				return 133;
			}
		}

		public override GamepadButton LowZoneDown
		{
			get
			{
				return 134;
			}
		}

		public override GamepadButton LowZoneLeft
		{
			get
			{
				return 135;
			}
		}

		public override GamepadButton LowZoneRight
		{
			get
			{
				return 136;
			}
		}

		public override GamepadButton MedZoneUp
		{
			get
			{
				return 137;
			}
		}

		public override GamepadButton MedZoneDown
		{
			get
			{
				return 138;
			}
		}

		public override GamepadButton MedZoneLeft
		{
			get
			{
				return 139;
			}
		}

		public override GamepadButton MedZoneRight
		{
			get
			{
				return 140;
			}
		}

		public override GamepadButton HighZoneUp
		{
			get
			{
				return 141;
			}
		}

		public override GamepadButton HighZoneDown
		{
			get
			{
				return 142;
			}
		}

		public override GamepadButton HighZoneLeft
		{
			get
			{
				return 143;
			}
		}

		public override GamepadButton HighZoneRight
		{
			get
			{
				return 144;
			}
		}

		public override GamepadButton UpLeft
		{
			get
			{
				return 2000;
			}
		}

		public override GamepadButton UpRight
		{
			get
			{
				return 2000;
			}
		}

		public override GamepadButton DownLeft
		{
			get
			{
				return 2000;
			}
		}

		public override GamepadButton DownRight
		{
			get
			{
				return 2000;
			}
		}

		public override GamepadButton LowZoneLeanLeft
		{
			get
			{
				return 2000;
			}
		}

		public override GamepadButton LowZoneLeanRight
		{
			get
			{
				return 2000;
			}
		}

		public override GamepadButton MedZoneLeanLeft
		{
			get
			{
				return 2000;
			}
		}

		public override GamepadButton MedZoneLeanRight
		{
			get
			{
				return 2000;
			}
		}

		public override GamepadButton HighZoneLeanLeft
		{
			get
			{
				return 2000;
			}
		}

		public override GamepadButton HighZoneLeanRight
		{
			get
			{
				return 2000;
			}
		}

		public override double DeadZoneMinimum
		{
			get
			{
				return 100.0;
			}
		}

		public override ushort DefaultXLow
		{
			get
			{
				return 100;
			}
		}

		public override ushort DefaultYLow
		{
			get
			{
				return 100;
			}
		}

		public override ushort DefaultXMed
		{
			get
			{
				return 100;
			}
		}

		public override ushort DefaultYMed
		{
			get
			{
				return 100;
			}
		}

		public override ushort DefaultXHigh
		{
			get
			{
				return 16000;
			}
		}

		public override ushort DefaultYHigh
		{
			get
			{
				return 16000;
			}
		}

		public override bool IsWASDGroupMappingAvailable
		{
			get
			{
				return false;
			}
		}

		public override bool IsFlickStickGroupMappingAvailable
		{
			get
			{
				return false;
			}
		}

		public override bool IsArrowsGroupMappingAvailable
		{
			get
			{
				return false;
			}
		}

		public override bool IsMouseGroupMappingAvailable
		{
			get
			{
				return false;
			}
		}

		public override bool ResetToDefault(bool askConfirmation = true)
		{
			bool flag = base.ResetToDefault(askConfirmation);
			if (flag)
			{
				App.EventAggregator.GetEvent<RequestBindingFrameBack>().Publish(null);
			}
			return flag;
		}

		protected override void SetDefaultValues(bool silent = false)
		{
			base.SetDefaultValues(silent);
			if (silent)
			{
				this._ySensitivity = 13573;
				this._xSensitivity = 13573;
				return;
			}
			base.XSensitivity = 13573;
			base.YSensitivity = 13573;
		}

		public override void Unbind()
		{
			base.Unbind();
			this.SetDefaultValues(false);
		}

		public override void RevertRemapToDefault()
		{
			base.RevertRemapToDefault();
		}

		public override void Unmap()
		{
			base.Unmap();
		}
	}
}
