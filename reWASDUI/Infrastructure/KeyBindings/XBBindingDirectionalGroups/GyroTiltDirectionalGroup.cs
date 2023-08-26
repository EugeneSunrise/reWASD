using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings.ActivatorXB;
using reWASDUI.Infrastructure.KeyBindings.MacroBinding;
using reWASDUI.Infrastructure.KeyBindings.Mask;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.Views;
using XBEliteWPF.Infrastructure.KeyBindingsModel.XBBindingDirectionalGroups;
using XBEliteWPF.Infrastructure.reWASDMapping;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Infrastructure.reWASDMapping.RewasduserCommands;
using XBEliteWPF.Infrastructure.XBEliteService;

namespace reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups
{
	public class GyroTiltDirectionalGroup : BaseDirectionalAnalogGroup
	{
		public override bool IsButtonMappingVisible
		{
			get
			{
				if (this.IsTiltMode)
				{
					ControllerVM currentController = base.CurrentController;
					return currentController != null && currentController.HasLean;
				}
				return true;
			}
		}

		public GyroTiltDirectionalGroup MainGyroTiltDirectionalGroup
		{
			get
			{
				return base.MainDirectionalAnalogGroup as GyroTiltDirectionalGroup;
			}
		}

		public bool IsGyroMode
		{
			get
			{
				if (!(base.HostCollection is MainXBBindingCollection))
				{
					return this.MainGyroTiltDirectionalGroup._isGyroMode;
				}
				return this._isGyroMode;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isGyroMode, value, "IsGyroMode"))
				{
					this.IsTiltMode = !value;
					this.OnPropertyChanged("GroupLabel");
					this.OnPropertyChanged("GroupMapToLabel");
					this.OnPropertyChanged("ZonesExplanation");
					this.OnPropertyChanged("StickResponseControlsIsVisible");
					this.OnPropertyChanged("StickResponseControlsEnabled");
					this.OnPropertyChanged("IsMouseSmoothingAvailable");
					this.OnPropertyChanged("IsButtonMappingVisible");
					this.OnPropertyChanged("IsAdvancedSettingsVisible");
					this.SetDefaultZoneValuesOnly();
					if (this._isTiltMode && (base.CurrentSelectedDirection == 5 || base.CurrentSelectedDirection == 6))
					{
						base.CurrentSelectedDirection = 0;
					}
					MainXBBindingCollection mainXBBindingCollection = base.HostCollection as MainXBBindingCollection;
					if (mainXBBindingCollection != null)
					{
						foreach (ShiftXBBindingCollection shiftXBBindingCollection in mainXBBindingCollection.ShiftXBBindingCollections)
						{
							shiftXBBindingCollection.GyroTiltDirectionalGroup.IsGyroMode = value;
						}
					}
				}
			}
		}

		public bool IsTiltMode
		{
			get
			{
				if (!(base.HostCollection is MainXBBindingCollection))
				{
					return this.MainGyroTiltDirectionalGroup._isTiltMode;
				}
				return this._isTiltMode;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isTiltMode, value, "IsTiltMode"))
				{
					this.IsGyroMode = !value;
				}
			}
		}

		public override ushort DefaultScaleFactor
		{
			get
			{
				return 100;
			}
		}

		public string GyroTiltSwitchResetExplanation
		{
			get
			{
				if (this.IsGyroMode)
				{
					if (this.MappedTo.Any<object>())
					{
						return DTLocalization.GetString(11548);
					}
					return DTLocalization.GetString(11549);
				}
				else
				{
					if (this.MappedTo.Any<object>())
					{
						return DTLocalization.GetString(11550);
					}
					return DTLocalization.GetString(11551);
				}
			}
		}

		public List<object> MappedTo
		{
			get
			{
				BaseRewasdMapping mappingToCheckFor = (this.IsGyroMode ? BaseRewasdUserCommand.SwitchGyroAxis : BaseRewasdUserCommand.ResetTilt);
				List<object> list = new List<object>();
				List<BaseXBBindingCollection> list2 = new List<BaseXBBindingCollection>();
				list2.Add(this.MainGyroTiltDirectionalGroup.HostCollection);
				foreach (ShiftXBBindingCollection shiftXBBindingCollection in ((MainXBBindingCollection)this.MainGyroTiltDirectionalGroup.HostCollection).ShiftXBBindingCollections)
				{
					list2.Add(shiftXBBindingCollection);
				}
				IEnumerable<MaskItem> enumerable = list2.SelectMany((BaseXBBindingCollection col) => col.MaskBindingCollection);
				IEnumerable<ActivatorXBBinding> enumerable2 = from ab in (from xbBindings in list2.SelectMany((BaseXBBindingCollection col) => col.Values)
						from activatorDictionaries in xbBindings.ActivatorXBBindingDictionary
						where activatorDictionaries.Value.MappedKey == mappingToCheckFor
						select xbBindings).Distinct<XBBinding>().SelectMany((XBBinding a) => a.ActivatorXBBindingDictionary.Values)
					where ab.MappedKey == mappingToCheckFor
					select ab;
				list.AddRange(enumerable2);
				Func<ActivatorXBBinding, bool> <>9__9;
				list.AddRange(enumerable.Where(delegate(MaskItem mask)
				{
					ObservableDictionary<ActivatorType, ActivatorXBBinding> activatorXBBindingDictionary = mask.XBBinding.ActivatorXBBindingDictionary;
					Func<ActivatorXBBinding, bool> func;
					if ((func = <>9__9) == null)
					{
						func = (<>9__9 = (ActivatorXBBinding axb) => axb.MappedKey == mappingToCheckFor);
					}
					return activatorXBBindingDictionary.AnyValue(func);
				}));
				return list;
			}
		}

		public GyroTiltDirectionalGroup(BaseXBBindingCollection baseHostCollection)
			: base(baseHostCollection)
		{
		}

		public override bool InvertControlsEnabled
		{
			get
			{
				return base.IsBoundToWASD || base.IsBoundToFlickStick || base.IsBoundToArrows || base.IsBoundToVirtualLeftStick || base.IsBoundToVirtualRightStick || base.IsBoundToAnyInvertedMouse || this.IsYInvert || this.IsXInvert;
			}
		}

		public override string InvertControlsDisabledExplanation
		{
			get
			{
				return DTLocalization.GetString(11619);
			}
		}

		public override bool StickResponseControlsEnabled
		{
			get
			{
				return base.IsBoundToVirtualLeftStick || base.IsBoundToVirtualRightStick || base.IsBoundToAnyInvertedMouse || this.IsTiltMode;
			}
		}

		public override string StickResponseDisabledExplanation
		{
			get
			{
				return DTLocalization.GetString(11618);
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
				return this.IsGyroMode;
			}
		}

		public override bool IsStickCrossZonesChangeAvailable
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
				return false;
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
				if (!this.IsGyroMode)
				{
					return false;
				}
				BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
				if (currentGamepad == null)
				{
					return false;
				}
				ControllerVM currentController = currentGamepad.CurrentController;
				bool? flag = ((currentController != null) ? new bool?(currentController.HasLean) : null);
				bool flag2 = true;
				return (flag.GetValueOrDefault() == flag2) & (flag != null);
			}
		}

		public override bool IsZonesShapeChangeAllowed
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

		public override bool IsUnmapAvailable
		{
			get
			{
				return true;
			}
		}

		public override bool IsStickCrossZonesInherited
		{
			get
			{
				return true;
			}
		}

		public override string ZonesLabelAxisX
		{
			get
			{
				return DTLocalization.GetString(11351);
			}
		}

		public override bool IsYInvert
		{
			get
			{
				return (base.UpDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.DikS && base.DownDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.DikW) || (base.UpDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.DikDown && base.DownDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.DikUp) || (base.UpDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonLeftStickDown && base.DownDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonLeftStickUp) || (base.UpDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonRightStickDown && base.DownDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonRightStickUp) || (base.UpDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseYUp && base.DownDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseYDown);
			}
		}

		public override bool IsXInvert
		{
			get
			{
				return (base.LeftDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.DikD && base.RightDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.DikA) || (base.LeftDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.DikRight && base.RightDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.DikLeft) || (base.LeftDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonLeftStickRight && base.RightDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonLeftStickLeft) || (base.LeftDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonRightStickRight && base.RightDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonRightStickLeft) || (base.LeftDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseXRight && base.RightDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseXLeft);
			}
		}

		public override string GroupLabel
		{
			get
			{
				return DTLocalization.GetString(this.IsGyroMode ? 11614 : 11615);
			}
		}

		public override string ZonesExplanation
		{
			get
			{
				return null;
			}
		}

		public override double DeadZoneMinimum
		{
			get
			{
				return 0.0;
			}
		}

		public override GamepadButton LeftDirection
		{
			get
			{
				return 70;
			}
		}

		public override GamepadButton UpDirection
		{
			get
			{
				return 68;
			}
		}

		public override GamepadButton RightDirection
		{
			get
			{
				return 71;
			}
		}

		public override GamepadButton DownDirection
		{
			get
			{
				return 69;
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

		public override GamepadButton LowZone
		{
			get
			{
				return 74;
			}
		}

		public override GamepadButton MedZone
		{
			get
			{
				return 75;
			}
		}

		public override GamepadButton HighZone
		{
			get
			{
				return 76;
			}
		}

		public override GamepadButton LowZoneUp
		{
			get
			{
				return 145;
			}
		}

		public override GamepadButton LowZoneDown
		{
			get
			{
				return 146;
			}
		}

		public override GamepadButton LowZoneLeft
		{
			get
			{
				return 147;
			}
		}

		public override GamepadButton LowZoneRight
		{
			get
			{
				return 148;
			}
		}

		public override GamepadButton MedZoneUp
		{
			get
			{
				return 149;
			}
		}

		public override GamepadButton MedZoneDown
		{
			get
			{
				return 150;
			}
		}

		public override GamepadButton MedZoneLeft
		{
			get
			{
				return 151;
			}
		}

		public override GamepadButton MedZoneRight
		{
			get
			{
				return 152;
			}
		}

		public override GamepadButton HighZoneUp
		{
			get
			{
				return 153;
			}
		}

		public override GamepadButton HighZoneDown
		{
			get
			{
				return 154;
			}
		}

		public override GamepadButton HighZoneLeft
		{
			get
			{
				return 155;
			}
		}

		public override GamepadButton HighZoneRight
		{
			get
			{
				return 156;
			}
		}

		public override GamepadButton LowZoneLeanLeft
		{
			get
			{
				return 157;
			}
		}

		public override GamepadButton LowZoneLeanRight
		{
			get
			{
				return 158;
			}
		}

		public override GamepadButton MedZoneLeanLeft
		{
			get
			{
				return 159;
			}
		}

		public override GamepadButton MedZoneLeanRight
		{
			get
			{
				return 160;
			}
		}

		public override GamepadButton HighZoneLeanLeft
		{
			get
			{
				return 161;
			}
		}

		public override GamepadButton HighZoneLeanRight
		{
			get
			{
				return 162;
			}
		}

		public GamepadButton LeanLeft
		{
			get
			{
				return 72;
			}
		}

		public GamepadButton LeanRight
		{
			get
			{
				return 73;
			}
		}

		public XBBinding LeanLeftValue
		{
			get
			{
				return base.HostCollection[this.LeanLeft];
			}
			set
			{
				base.HostCollection[this.LeanLeft] = value;
			}
		}

		public XBBinding LeanRightValue
		{
			get
			{
				return base.HostCollection[this.LeanRight];
			}
			set
			{
				base.HostCollection[this.LeanRight] = value;
			}
		}

		public override ushort DefaultXLow
		{
			get
			{
				if (!this.IsGyroMode)
				{
					return 500;
				}
				return 50;
			}
		}

		public override ushort DefaultYLow
		{
			get
			{
				if (!this.IsGyroMode)
				{
					return 500;
				}
				return 50;
			}
		}

		public override ushort DefaultXMed
		{
			get
			{
				if (!this.IsGyroMode)
				{
					return 16155;
				}
				return 4000;
			}
		}

		public override ushort DefaultYMed
		{
			get
			{
				if (!this.IsGyroMode)
				{
					return 16155;
				}
				return 4000;
			}
		}

		public override ushort DefaultXHigh
		{
			get
			{
				if (!this.IsGyroMode)
				{
					return 24461;
				}
				return 9000;
			}
		}

		public override ushort DefaultYHigh
		{
			get
			{
				if (!this.IsGyroMode)
				{
					return 24461;
				}
				return 9000;
			}
		}

		protected override ObservableCollection<ThumbstickSensitivity> CreateSensitivities()
		{
			return new ObservableCollection<ThumbstickSensitivity>
			{
				this.CreateDefaultSensitivity(),
				this.CreateAggressiveSensitivity(),
				this.CreateMinimizedAccelerationSensitivity(),
				this.CreateCustomSensitivity()
			};
		}

		protected override ThumbstickSensitivity CreateDefaultSensitivity()
		{
			return new ThumbstickSensitivity("Default", new Localizable(11621), 8000, 16000, 16384, 32768, false);
		}

		protected override ThumbstickSensitivity CreateAggressiveSensitivity()
		{
			return new ThumbstickSensitivity("Aggressive", new Localizable(11622), 2730, 8195, 6000, 26000, true);
		}

		private ThumbstickSensitivity CreateMinimizedAccelerationSensitivity()
		{
			return new ThumbstickSensitivity("Minimized Acceleration", new Localizable(12099), 8000, 17000, 32768, 17000, true);
		}

		protected override ThumbstickSensitivity CreateCustomSensitivity()
		{
			return new ThumbstickSensitivity("Custom", new Localizable(11620), 6000, 12000, 10000, 20000, 14384, 28768, 16384, 32768, true);
		}

		public override bool ResetToDefault(bool askConfirmation = true)
		{
			bool isTiltMode = this.IsTiltMode;
			bool flag = base.ResetToDefault(askConfirmation);
			if (flag && isTiltMode)
			{
				reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(SVGGamepadWithAllAnnotations));
			}
			return flag;
		}

		protected override void ResetActivatorXBBindingDictionaries()
		{
			base.ResetActivatorXBBindingDictionaries();
			this.LeanLeftValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.LeanLeft);
			this.LeanRightValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.LeanRight);
		}

		protected override void SetDefaultValues(bool silent = false)
		{
			base.SetDefaultValues(silent);
			if (silent)
			{
				this._isGyroMode = true;
				this._isTiltMode = false;
				this._ySensitivity = 13573;
				this._xSensitivity = 13573;
				this._smoothing = 3U;
				this._noiseFilter = 3U;
				this._scaleFactorX = (uint)this.DefaultScaleFactor;
				this._scaleFactorY = (uint)this.DefaultScaleFactor;
				return;
			}
			this.IsGyroMode = true;
			this.IsTiltMode = false;
			base.XSensitivity = 13573;
			base.YSensitivity = 13573;
			base.Smoothing = 3U;
			base.NoiseFilter = 3U;
			base.ScaleFactorX = (uint)this.DefaultScaleFactor;
			base.ScaleFactorY = (uint)this.DefaultScaleFactor;
		}

		public override bool IsAdvancedDefault()
		{
			return base.IsAdvancedDefault() & this.LeanLeftValue.IsEmpty & this.LeanRightValue.IsEmpty;
		}

		private void SetDefaultZoneValuesOnly()
		{
			base.XLow = this.DefaultXLow;
			base.XMed = this.DefaultXMed;
			base.XHigh = this.DefaultXHigh;
			base.YLow = this.DefaultYLow;
			base.YMed = this.DefaultYMed;
			base.YHigh = this.DefaultYHigh;
		}

		public bool IsLeanDirectionVirtualMappingPresent
		{
			get
			{
				return this.LeanLeftValue.IsSingleActivatorVirtualMappingPresent || this.LeanRightValue.IsSingleActivatorVirtualMappingPresent;
			}
		}

		public void CopyFromModel(GyroTiltDirectionalGroup model)
		{
			this._isGyroMode = model.IsGyroMode;
			this._isTiltMode = model.IsTiltMode;
			base.CopyFromModel(model);
		}

		public void CopyToModel(GyroTiltDirectionalGroup model)
		{
			model.IsGyroMode = this._isGyroMode;
			model.IsTiltMode = this._isTiltMode;
			base.CopyToModel(model);
		}

		public void CopyFrom(GyroTiltDirectionalGroup model)
		{
			this._isGyroMode = model.IsGyroMode;
			this._isTiltMode = model.IsTiltMode;
			base.CopyFrom(model);
		}

		public void CopyFromForShiftCollection(GyroTiltDirectionalGroup model)
		{
			this._isGyroMode = model.IsGyroMode;
			this._isTiltMode = model.IsTiltMode;
			base.CopyFromForShiftCollection(model);
		}

		public void CopyTo(GyroTiltDirectionalGroup model)
		{
			model.IsGyroMode = this._isGyroMode;
			model.IsTiltMode = this._isTiltMode;
			base.CopyTo(model);
		}

		private bool _isGyroMode = true;

		private bool _isTiltMode;
	}
}
