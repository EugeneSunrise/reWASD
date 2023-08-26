using System;
using System.Collections.ObjectModel;
using System.Linq;
using DiscSoft.NET.Common.Localization;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.KeyBindings.MacroBinding;
using reWASDUI.Infrastructure.KeyBindings.XB;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.XBEliteService;
using XBEliteWPF.Utils.Wrappers;

namespace reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups
{
	public abstract class BaseStickDirectionalGroup : BaseDirectionalAnalogGroup
	{
		public override string ZonesExplanation
		{
			get
			{
				return null;
			}
		}

		public override bool IsSpringModeAllowed
		{
			get
			{
				return false;
			}
		}

		public override bool IsAdvancedSpringDeadzoneVisible
		{
			get
			{
				return false;
			}
		}

		public override bool IsHardwareChangesAllowed
		{
			get
			{
				return true;
			}
		}

		public override bool IsZoneRangeInherited
		{
			get
			{
				return true;
			}
		}

		public override bool IsZonesAllowed
		{
			get
			{
				return true;
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
				return true;
			}
		}

		public override bool IsZoneMedAllowed
		{
			get
			{
				return true;
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
				return (base.IsGamepadWithSonyTouchpad || base.IsNintendoSwitch) && base.IsBoundToFlickStick;
			}
		}

		public override bool IsStickCrossZonesInherited
		{
			get
			{
				return true;
			}
		}

		public override bool IsDiagonalDirectionsAllowed
		{
			get
			{
				return true;
			}
		}

		public override bool IsFlickStickGroupMappingAvailable
		{
			get
			{
				return true;
			}
		}

		public override bool IsArrowsGroupMappingAvailable
		{
			get
			{
				return false;
			}
		}

		public abstract GamepadButton Click { get; }

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
				return 0.0;
			}
		}

		public XBBinding ClickValue
		{
			get
			{
				return base.HostCollection[this.Click];
			}
			set
			{
				base.HostCollection[this.Click] = value;
			}
		}

		public override XBBinding CurrentSelectedDiagonalDirectionXBBinding
		{
			get
			{
				return base.DirectionToXBBinding(base.CurrentSelectedDiagonalDirection);
			}
		}

		public override GamepadButton CurrentSelectedDiagonalDirectionButton
		{
			get
			{
				return base.DirectionToGamepadButton(base.CurrentSelectedDiagonalDirection);
			}
		}

		private bool _IsHardwareChangesPresent(bool print = false)
		{
			if (print)
			{
				return false;
			}
			if (!base.IsHardwareDeadzone && base.XLow == this.DefaultXLow && base.YLow == this.DefaultYLow && base.XMed == this.DefaultXMed && base.YMed == this.DefaultYMed && base.XHigh == this.DefaultXHigh && base.YHigh == this.DefaultYHigh && base.XSensitivity == 13573 && base.YSensitivity == 13573)
			{
				DISC_SOFT_GAMEPAD_STICK_DEFLECTION_NPROPCHANGE sensitivity = base.Sensitivity;
				ThumbstickSensitivity thumbstickSensitivity = base.SensitivitiesCollection.FirstOrDefault<ThumbstickSensitivity>();
				return sensitivity != ((thumbstickSensitivity != null) ? thumbstickSensitivity.Deflection : null);
			}
			return true;
		}

		public virtual bool IsHardwareChangesPresent
		{
			get
			{
				return this._IsHardwareChangesPresent(false);
			}
		}

		public virtual bool IsHardwareChangesPresentForPrint
		{
			get
			{
				return this._IsHardwareChangesPresent(true);
			}
		}

		public override bool IsXInvert
		{
			get
			{
				return (base.LeftDirectionValue.RemapedTo.Button == 43 || base.LeftDirectionValue.RemapedTo.Button == 50 || base.LeftDirectionValue.RemapedTo.Button == 219) && (base.RightDirectionValue.RemapedTo.Button == 42 || base.RightDirectionValue.RemapedTo.Button == 49 || base.RightDirectionValue.RemapedTo.Button == 218);
			}
		}

		public override bool IsYInvert
		{
			get
			{
				return (base.UpDirectionValue.RemapedTo.Button == 41 || base.UpDirectionValue.RemapedTo.Button == 48 || base.UpDirectionValue.RemapedTo.Button == 217) && (base.DownDirectionValue.RemapedTo.Button == 40 || base.DownDirectionValue.RemapedTo.Button == 47 || base.DownDirectionValue.RemapedTo.Button == 216);
			}
		}

		public override string StickResponseDisabledExplanation
		{
			get
			{
				return DTLocalization.GetString(11416);
			}
		}

		public BaseStickDirectionalGroup(BaseXBBindingCollection baseHostCollection)
			: base(baseHostCollection)
		{
			base.CurrentSelectedDiagonalDirection = 7;
		}

		public override void Unmap()
		{
			base.Unmap();
			base.IsHardwareDeadzone = false;
			ObservableCollection<ThumbstickSensitivity> sensitivitiesCollection = base.SensitivitiesCollection;
			base.SensitivitySensitivity = ((sensitivitiesCollection != null) ? sensitivitiesCollection.FirstOrDefault<ThumbstickSensitivity>() : null);
			this.OnPropertyChanged("StickResponseControlsEnabled");
		}

		public override bool ResetToDefault(bool askConfirmation = true)
		{
			bool flag = base.ResetToDefault(askConfirmation);
			if (flag && this.Click != 2000)
			{
				this.ClickValue.RevertRemap();
			}
			return flag;
		}

		public override void RevertRemapToDefault()
		{
			base.RevertRemapToDefault();
			base.IsHardwareDeadzone = false;
			ObservableCollection<ThumbstickSensitivity> sensitivitiesCollection = base.SensitivitiesCollection;
			base.SensitivitySensitivity = ((sensitivitiesCollection != null) ? sensitivitiesCollection.FirstOrDefault<ThumbstickSensitivity>() : null);
			this.OnPropertyChanged("StickResponseControlsEnabled");
		}

		public void SwapStickWith(BaseStickDirectionalGroup anotherStickGroup)
		{
			ActivatorXBBindingDictionary activatorXBBindingDictionary = this.LeftDirectionValue.ActivatorXBBindingDictionary;
			ActivatorXBBindingDictionary activatorXBBindingDictionary2 = this.RightDirectionValue.ActivatorXBBindingDictionary;
			ActivatorXBBindingDictionary activatorXBBindingDictionary3 = this.UpDirectionValue.ActivatorXBBindingDictionary;
			ActivatorXBBindingDictionary activatorXBBindingDictionary4 = this.DownDirectionValue.ActivatorXBBindingDictionary;
			ActivatorXBBindingDictionary activatorXBBindingDictionary5 = this.ClickValue.ActivatorXBBindingDictionary;
			ActivatorXBBindingDictionary activatorXBBindingDictionary6 = this.LowZoneValue.ActivatorXBBindingDictionary;
			ActivatorXBBindingDictionary activatorXBBindingDictionary7 = this.MedZoneValue.ActivatorXBBindingDictionary;
			ActivatorXBBindingDictionary activatorXBBindingDictionary8 = this.HighZoneValue.ActivatorXBBindingDictionary;
			bool flag = false;
			ActivatorXBBindingDictionary activatorXBBindingDictionary9 = null;
			ActivatorXBBindingDictionary activatorXBBindingDictionary10 = null;
			ActivatorXBBindingDictionary activatorXBBindingDictionary11 = null;
			ActivatorXBBindingDictionary activatorXBBindingDictionary12 = null;
			if (this.IsDiagonalDirectionsAllowed)
			{
				flag = this.IsDiagonalDirections;
				activatorXBBindingDictionary9 = this.UpLeftValue.ActivatorXBBindingDictionary;
				activatorXBBindingDictionary10 = this.UpRightValue.ActivatorXBBindingDictionary;
				activatorXBBindingDictionary11 = this.DownLeftValue.ActivatorXBBindingDictionary;
				activatorXBBindingDictionary12 = this.DownRightValue.ActivatorXBBindingDictionary;
			}
			ActivatorXBBindingDictionary activatorXBBindingDictionary13 = null;
			ActivatorXBBindingDictionary activatorXBBindingDictionary14 = null;
			ActivatorXBBindingDictionary activatorXBBindingDictionary15 = null;
			ActivatorXBBindingDictionary activatorXBBindingDictionary16 = null;
			ActivatorXBBindingDictionary activatorXBBindingDictionary17 = null;
			ActivatorXBBindingDictionary activatorXBBindingDictionary18 = null;
			ActivatorXBBindingDictionary activatorXBBindingDictionary19 = null;
			ActivatorXBBindingDictionary activatorXBBindingDictionary20 = null;
			ActivatorXBBindingDictionary activatorXBBindingDictionary21 = null;
			ActivatorXBBindingDictionary activatorXBBindingDictionary22 = null;
			ActivatorXBBindingDictionary activatorXBBindingDictionary23 = null;
			ActivatorXBBindingDictionary activatorXBBindingDictionary24 = null;
			if (this.IsZonesDirectionAllowed && anotherStickGroup.IsZonesDirectionAllowed)
			{
				activatorXBBindingDictionary13 = this.HighZoneLeftValue.ActivatorXBBindingDictionary;
				activatorXBBindingDictionary14 = this.HighZoneRightValue.ActivatorXBBindingDictionary;
				activatorXBBindingDictionary15 = this.HighZoneUpValue.ActivatorXBBindingDictionary;
				activatorXBBindingDictionary16 = this.HighZoneDownValue.ActivatorXBBindingDictionary;
				activatorXBBindingDictionary17 = this.MedZoneLeftValue.ActivatorXBBindingDictionary;
				activatorXBBindingDictionary18 = this.MedZoneRightValue.ActivatorXBBindingDictionary;
				activatorXBBindingDictionary19 = this.MedZoneUpValue.ActivatorXBBindingDictionary;
				activatorXBBindingDictionary20 = this.MedZoneDownValue.ActivatorXBBindingDictionary;
				activatorXBBindingDictionary21 = this.LowZoneLeftValue.ActivatorXBBindingDictionary;
				activatorXBBindingDictionary22 = this.LowZoneRightValue.ActivatorXBBindingDictionary;
				activatorXBBindingDictionary23 = this.LowZoneUpValue.ActivatorXBBindingDictionary;
				activatorXBBindingDictionary24 = this.LowZoneDownValue.ActivatorXBBindingDictionary;
			}
			this.LeftDirectionValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(anotherStickGroup.LeftDirectionValue.ActivatorXBBindingDictionary, this.LeftDirectionValue.ControllerButton.Clone());
			this.UpDirectionValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(anotherStickGroup.UpDirectionValue.ActivatorXBBindingDictionary, this.UpDirectionValue.ControllerButton.Clone());
			this.RightDirectionValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(anotherStickGroup.RightDirectionValue.ActivatorXBBindingDictionary, this.RightDirectionValue.ControllerButton.Clone());
			this.DownDirectionValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(anotherStickGroup.DownDirectionValue.ActivatorXBBindingDictionary, this.DownDirectionValue.ControllerButton.Clone());
			this.ClickValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(anotherStickGroup.ClickValue.ActivatorXBBindingDictionary, this.ClickValue.ControllerButton.Clone());
			this.LowZoneValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(anotherStickGroup.LowZoneValue.ActivatorXBBindingDictionary, this.LowZoneValue.ControllerButton.Clone());
			this.MedZoneValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(anotherStickGroup.MedZoneValue.ActivatorXBBindingDictionary, this.MedZoneValue.ControllerButton.Clone());
			this.HighZoneValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(anotherStickGroup.HighZoneValue.ActivatorXBBindingDictionary, this.HighZoneValue.ControllerButton.Clone());
			if (this.IsZonesDirectionAllowed && anotherStickGroup.IsZonesDirectionAllowed)
			{
				this.HighZoneLeftValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(anotherStickGroup.HighZoneLeftValue.ActivatorXBBindingDictionary, this.HighZoneLeftValue.ControllerButton.Clone());
				this.HighZoneRightValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(anotherStickGroup.HighZoneRightValue.ActivatorXBBindingDictionary, this.HighZoneRightValue.ControllerButton.Clone());
				this.HighZoneUpValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(anotherStickGroup.HighZoneUpValue.ActivatorXBBindingDictionary, this.HighZoneUpValue.ControllerButton.Clone());
				this.HighZoneDownValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(anotherStickGroup.HighZoneDownValue.ActivatorXBBindingDictionary, this.HighZoneDownValue.ControllerButton.Clone());
				this.MedZoneLeftValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(anotherStickGroup.MedZoneLeftValue.ActivatorXBBindingDictionary, this.MedZoneLeftValue.ControllerButton.Clone());
				this.MedZoneRightValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(anotherStickGroup.MedZoneRightValue.ActivatorXBBindingDictionary, this.MedZoneRightValue.ControllerButton.Clone());
				this.MedZoneUpValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(anotherStickGroup.MedZoneUpValue.ActivatorXBBindingDictionary, this.MedZoneUpValue.ControllerButton.Clone());
				this.MedZoneDownValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(anotherStickGroup.MedZoneDownValue.ActivatorXBBindingDictionary, this.MedZoneDownValue.ControllerButton.Clone());
				this.LowZoneLeftValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(anotherStickGroup.LowZoneLeftValue.ActivatorXBBindingDictionary, this.LowZoneLeftValue.ControllerButton.Clone());
				this.LowZoneRightValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(anotherStickGroup.LowZoneRightValue.ActivatorXBBindingDictionary, this.LowZoneRightValue.ControllerButton.Clone());
				this.LowZoneUpValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(anotherStickGroup.LowZoneUpValue.ActivatorXBBindingDictionary, this.LowZoneUpValue.ControllerButton.Clone());
				this.LowZoneDownValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(anotherStickGroup.LowZoneDownValue.ActivatorXBBindingDictionary, this.LowZoneDownValue.ControllerButton.Clone());
			}
			if (this.IsDiagonalDirectionsAllowed && anotherStickGroup.IsDiagonalDirectionsAllowed)
			{
				this.IsDiagonalDirections = anotherStickGroup.IsDiagonalDirections;
				this.UpLeftValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(anotherStickGroup.UpLeftValue.ActivatorXBBindingDictionary, this.UpLeftValue.ControllerButton.Clone());
				this.UpRightValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(anotherStickGroup.UpRightValue.ActivatorXBBindingDictionary, this.UpRightValue.ControllerButton.Clone());
				this.DownLeftValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(anotherStickGroup.DownLeftValue.ActivatorXBBindingDictionary, this.DownLeftValue.ControllerButton.Clone());
				this.DownRightValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(anotherStickGroup.DownRightValue.ActivatorXBBindingDictionary, this.DownRightValue.ControllerButton.Clone());
			}
			anotherStickGroup.LeftDirectionValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary, anotherStickGroup.LeftDirectionValue.ControllerButton.Clone());
			anotherStickGroup.UpDirectionValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary3, anotherStickGroup.UpDirectionValue.ControllerButton.Clone());
			anotherStickGroup.RightDirectionValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary2, anotherStickGroup.RightDirectionValue.ControllerButton.Clone());
			anotherStickGroup.DownDirectionValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary4, anotherStickGroup.DownDirectionValue.ControllerButton.Clone());
			anotherStickGroup.ClickValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary5, anotherStickGroup.ClickValue.ControllerButton.Clone());
			anotherStickGroup.LowZoneValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary6, anotherStickGroup.LowZoneValue.ControllerButton.Clone());
			anotherStickGroup.MedZoneValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary7, anotherStickGroup.MedZoneValue.ControllerButton.Clone());
			anotherStickGroup.HighZoneValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary8, anotherStickGroup.HighZoneValue.ControllerButton.Clone());
			if (this.IsZonesDirectionAllowed && anotherStickGroup.IsZonesDirectionAllowed)
			{
				anotherStickGroup.HighZoneLeftValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary13, anotherStickGroup.HighZoneLeftValue.ControllerButton.Clone());
				anotherStickGroup.HighZoneRightValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary14, anotherStickGroup.HighZoneRightValue.ControllerButton.Clone());
				anotherStickGroup.HighZoneUpValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary15, anotherStickGroup.HighZoneUpValue.ControllerButton.Clone());
				anotherStickGroup.HighZoneDownValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary16, anotherStickGroup.HighZoneDownValue.ControllerButton.Clone());
				anotherStickGroup.MedZoneLeftValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary17, anotherStickGroup.MedZoneLeftValue.ControllerButton.Clone());
				anotherStickGroup.MedZoneRightValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary18, anotherStickGroup.MedZoneRightValue.ControllerButton.Clone());
				anotherStickGroup.MedZoneUpValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary19, anotherStickGroup.MedZoneUpValue.ControllerButton.Clone());
				anotherStickGroup.MedZoneDownValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary20, anotherStickGroup.MedZoneDownValue.ControllerButton.Clone());
				anotherStickGroup.LowZoneLeftValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary21, anotherStickGroup.LowZoneLeftValue.ControllerButton.Clone());
				anotherStickGroup.LowZoneRightValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary22, anotherStickGroup.LowZoneRightValue.ControllerButton.Clone());
				anotherStickGroup.LowZoneUpValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary23, anotherStickGroup.LowZoneUpValue.ControllerButton.Clone());
				anotherStickGroup.LowZoneDownValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary24, anotherStickGroup.LowZoneDownValue.ControllerButton.Clone());
			}
			if (this.IsDiagonalDirectionsAllowed && anotherStickGroup.IsDiagonalDirectionsAllowed)
			{
				anotherStickGroup.IsDiagonalDirections = flag;
				anotherStickGroup.UpLeftValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary9, anotherStickGroup.UpLeftValue.ControllerButton.Clone());
				anotherStickGroup.UpRightValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary10, anotherStickGroup.UpRightValue.ControllerButton.Clone());
				anotherStickGroup.DownLeftValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary11, anotherStickGroup.DownLeftValue.ControllerButton.Clone());
				anotherStickGroup.DownRightValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary12, anotherStickGroup.DownRightValue.ControllerButton.Clone());
			}
			GamepadButtonDescription remapedTo = this.LeftDirectionValue.RemapedTo;
			GamepadButtonDescription remapedTo2 = this.UpDirectionValue.RemapedTo;
			GamepadButtonDescription remapedTo3 = this.RightDirectionValue.RemapedTo;
			GamepadButtonDescription remapedTo4 = this.DownDirectionValue.RemapedTo;
			GamepadButtonDescription remapedTo5 = this.ClickValue.RemapedTo;
			this.LeftDirectionValue.RemapedTo = anotherStickGroup.LeftDirectionValue.RemapedTo;
			this.UpDirectionValue.RemapedTo = anotherStickGroup.UpDirectionValue.RemapedTo;
			this.RightDirectionValue.RemapedTo = anotherStickGroup.RightDirectionValue.RemapedTo;
			this.DownDirectionValue.RemapedTo = anotherStickGroup.DownDirectionValue.RemapedTo;
			this.ClickValue.RemapedTo = anotherStickGroup.ClickValue.RemapedTo;
			anotherStickGroup.LeftDirectionValue.RemapedTo = remapedTo;
			anotherStickGroup.UpDirectionValue.RemapedTo = remapedTo2;
			anotherStickGroup.RightDirectionValue.RemapedTo = remapedTo3;
			anotherStickGroup.DownDirectionValue.RemapedTo = remapedTo4;
			anotherStickGroup.ClickValue.RemapedTo = remapedTo5;
			ushort xlow = base.XLow;
			ushort xmed = base.XMed;
			ushort xhigh = base.XHigh;
			ushort ylow = base.YLow;
			ushort ymed = base.YMed;
			ushort yhigh = base.YHigh;
			bool isRadialZoning = base.IsRadialZoning;
			bool isEllipticZoning = base.IsEllipticZoning;
			bool isHardwareDeadzone = base.IsHardwareDeadzone;
			bool isHardwareDeadzone2 = anotherStickGroup.IsHardwareDeadzone;
			base.IsHardwareDeadzone = false;
			anotherStickGroup.IsHardwareDeadzone = false;
			ObservableCollection<ThumbstickSensitivity> sensitivitiesCollection = base.SensitivitiesCollection;
			DISC_SOFT_GAMEPAD_STICK_DEFLECTION_NPROPCHANGE sensitivity = base.Sensitivity;
			base.SensitivitiesCollection = anotherStickGroup.SensitivitiesCollection;
			base.Sensitivity = anotherStickGroup.Sensitivity;
			anotherStickGroup.SensitivitiesCollection = sensitivitiesCollection;
			anotherStickGroup.Sensitivity = sensitivity;
			ushort xsensitivity = base.XSensitivity;
			ushort ysensitivity = base.YSensitivity;
			base.XSensitivity = anotherStickGroup.XSensitivity;
			base.YSensitivity = anotherStickGroup.YSensitivity;
			anotherStickGroup.XSensitivity = xsensitivity;
			anotherStickGroup.YSensitivity = ysensitivity;
			base.XLow = anotherStickGroup.XLow;
			base.XMed = anotherStickGroup.XMed;
			base.XHigh = anotherStickGroup.XHigh;
			base.YLow = anotherStickGroup.YLow;
			base.YMed = anotherStickGroup.YMed;
			base.YHigh = anotherStickGroup.YHigh;
			base.IsRadialZoning = anotherStickGroup.IsRadialZoning;
			base.IsEllipticZoning = anotherStickGroup.IsEllipticZoning;
			anotherStickGroup.XLow = xlow;
			anotherStickGroup.XMed = xmed;
			anotherStickGroup.XHigh = xhigh;
			anotherStickGroup.YLow = ylow;
			anotherStickGroup.YMed = ymed;
			anotherStickGroup.YHigh = yhigh;
			anotherStickGroup.IsRadialZoning = isRadialZoning;
			anotherStickGroup.IsEllipticZoning = isEllipticZoning;
			base.IsHardwareDeadzone = isHardwareDeadzone2;
			anotherStickGroup.IsHardwareDeadzone = isHardwareDeadzone;
			short rotation = base.Rotation;
			base.Rotation = anotherStickGroup.Rotation;
			anotherStickGroup.Rotation = rotation;
			this.OnPropertyChanged("IsYInvert");
			this.OnPropertyChanged("IsXInvert");
			anotherStickGroup.OnPropertyChanged("IsYInvert");
			anotherStickGroup.OnPropertyChanged("IsXInvert");
		}

		protected override void ResetActivatorXBBindingDictionaries()
		{
			base.ResetActivatorXBBindingDictionaries();
			if (this.Click != 2000)
			{
				this.ClickValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.Click);
			}
		}

		protected override void SetDefaultValues(bool silent = false)
		{
			base.SetDefaultValues(silent);
			if (silent)
			{
				this._xSensitivity = 13573;
				this._ySensitivity = 13573;
				return;
			}
			base.XSensitivity = 13573;
			base.YSensitivity = 13573;
		}

		public override void UpdateProperties()
		{
			base.UpdateProperties();
			this.OnPropertyChanged("IsMouseSmoothingAvailable");
		}
	}
}
