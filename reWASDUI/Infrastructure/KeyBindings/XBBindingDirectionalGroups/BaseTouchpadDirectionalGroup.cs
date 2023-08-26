using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DiscSoft.NET.Common.Localization;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.KeyBindings.ActivatorXB;
using reWASDUI.Infrastructure.KeyBindings.MacroBinding;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.Views;
using XBEliteWPF.Infrastructure.KeyBindingsModel.XBBindingDirectionalGroups;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Infrastructure.XBEliteService;

namespace reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups
{
	public abstract class BaseTouchpadDirectionalGroup : BaseDirectionalAnalogGroup
	{
		public BaseTouchpadDirectionalGroup MainTouchpadDirectionalGroup
		{
			get
			{
				return base.MainDirectionalAnalogGroup as BaseTouchpadDirectionalGroup;
			}
		}

		public override string UncheckedSpringTooltip
		{
			get
			{
				return DTLocalization.GetString(11956);
			}
		}

		public override string CheckedSpringTooltip
		{
			get
			{
				return DTLocalization.GetString(11959);
			}
		}

		public override string ZonesExplanation
		{
			get
			{
				return null;
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
				return false;
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
				return false;
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

		public override bool IsSpringModeAllowed
		{
			get
			{
				return this.TouchpadAnalogMode && (base.IsBoundToVirtualLeftStick || base.IsBoundToVirtualRightStick);
			}
		}

		public override bool IsAdvancedSpringDeadzoneVisible
		{
			get
			{
				return true;
			}
		}

		public override bool IsRadialDeadzoneAllowed
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
				return true;
			}
		}

		public override bool IsDigitalMode
		{
			get
			{
				return this.TouchpadDigitalMode;
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
				return base.IsAnySteam;
			}
		}

		public override bool IsDiagonalDirectionsAllowed
		{
			get
			{
				return true;
			}
		}

		public override bool IsRotationAvailable
		{
			get
			{
				return base.IsAnySteam;
			}
		}

		public virtual bool DefaultTouchpadAnalogMode
		{
			get
			{
				return true;
			}
		}

		public virtual bool DefaultTouchpadZoneClickRequired
		{
			get
			{
				return false;
			}
		}

		public abstract GamepadButton Click { get; }

		public abstract GamepadButton Tap { get; }

		public override GamepadButton LowZone
		{
			get
			{
				return 2000;
			}
		}

		public override GamepadButton MedZone
		{
			get
			{
				return 2000;
			}
		}

		public override GamepadButton HighZone
		{
			get
			{
				return 2000;
			}
		}

		public override GamepadButton LowZoneUp
		{
			get
			{
				return 2000;
			}
		}

		public override GamepadButton LowZoneDown
		{
			get
			{
				return 2000;
			}
		}

		public override GamepadButton LowZoneLeft
		{
			get
			{
				return 2000;
			}
		}

		public override GamepadButton LowZoneRight
		{
			get
			{
				return 2000;
			}
		}

		public override GamepadButton MedZoneUp
		{
			get
			{
				return 2000;
			}
		}

		public override GamepadButton MedZoneDown
		{
			get
			{
				return 2000;
			}
		}

		public override GamepadButton MedZoneLeft
		{
			get
			{
				return 2000;
			}
		}

		public override GamepadButton MedZoneRight
		{
			get
			{
				return 2000;
			}
		}

		public override GamepadButton HighZoneUp
		{
			get
			{
				return 2000;
			}
		}

		public override GamepadButton HighZoneDown
		{
			get
			{
				return 2000;
			}
		}

		public override GamepadButton HighZoneLeft
		{
			get
			{
				return 2000;
			}
		}

		public override GamepadButton HighZoneRight
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

		public override double ScaleFactorMaximum
		{
			get
			{
				return 200.0;
			}
		}

		public override double ScaleFactorMinimum
		{
			get
			{
				return 0.0;
			}
		}

		public override ushort DefaultScaleFactor
		{
			get
			{
				return 100;
			}
		}

		public override bool DefaultSpringMode
		{
			get
			{
				return false;
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
				return true;
			}
		}

		public override bool IsDS4TouchpadGroupMappingAvailable
		{
			get
			{
				return true;
			}
		}

		public override bool IsButtonMappingVisible
		{
			get
			{
				return this.TouchpadDigitalMode;
			}
		}

		public override bool IsAdvancedSettingsVisible
		{
			get
			{
				return true;
			}
		}

		public virtual bool AdvancedDigitalMappingSettingsExist
		{
			get
			{
				return this.TouchpadDigitalMode;
			}
		}

		public virtual string AdvancedDigitalMappingSettingsDisabledTooltip
		{
			get
			{
				return DTLocalization.GetString(11926);
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

		public XBBinding TapValue
		{
			get
			{
				return base.HostCollection[this.Tap];
			}
			set
			{
				base.HostCollection[this.Tap] = value;
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

		public BaseTouchpadDirectionalGroup(BaseXBBindingCollection baseHostCollection)
			: base(baseHostCollection)
		{
			base.CurrentSelectedDiagonalDirection = 7;
		}

		public bool TouchpadAnalogMode
		{
			get
			{
				return this._touchpadAnalogMode;
			}
			set
			{
				if (this._touchpadAnalogMode == value)
				{
					return;
				}
				MainXBBindingCollection mainXBBindingCollection = base.HostCollection.SubConfigData.MainXBBindingCollection;
				if (mainXBBindingCollection.IsFirePropertyChanged)
				{
					mainXBBindingCollection.SetEnablePropertyChanged(false, false);
					this.SetTouchpadAnalogMode(value);
					mainXBBindingCollection.SetEnablePropertyChanged(true, false);
					return;
				}
				this.SetTouchpadAnalogMode(value);
			}
		}

		public bool TouchpadDigitalMode
		{
			get
			{
				return this._touchpadDigitalMode;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._touchpadDigitalMode, value, "TouchpadDigitalMode"))
				{
					this.TouchpadAnalogMode = !value;
					this.OnPropertyChanged("IsAnyDirectionHasJumpToShift");
					this.OnPropertyChanged("IsDigitalMode");
					this.OnPropertyChanged("IsButtonMappingVisible");
				}
			}
		}

		public bool? TouchpadZoneClickRequired
		{
			get
			{
				if (this.TouchpadAnalogMode)
				{
					return new bool?(false);
				}
				if (this._touchpadZoneClickRequired != null)
				{
					return this._touchpadZoneClickRequired;
				}
				return new bool?(this.DefaultTouchpadZoneClickRequired);
			}
			set
			{
				if (this.TouchpadAnalogMode)
				{
					value = new bool?(false);
				}
				if (this.SetProperty<bool?>(ref this._touchpadZoneClickRequired, value, "TouchpadZoneClickRequired"))
				{
					MainXBBindingCollection mainXBBindingCollection = base.HostCollection as MainXBBindingCollection;
					if (mainXBBindingCollection != null)
					{
						foreach (ShiftXBBindingCollection shiftXBBindingCollection in mainXBBindingCollection.ShiftXBBindingCollections)
						{
							BaseTouchpadDirectionalGroup baseTouchpadDirectionalGroup;
							if (!(this is Touchpad1DirectionalGroup))
							{
								if (!(this is Touchpad2DirectionalGroup))
								{
									throw new Exception("Unexpected this type in BaseTouchpadDirectionalGroup.TouchpadAnalogMode setter");
								}
								baseTouchpadDirectionalGroup = shiftXBBindingCollection.Touchpad2DirectionalGroup;
							}
							else
							{
								baseTouchpadDirectionalGroup = shiftXBBindingCollection.Touchpad1DirectionalGroup;
							}
							if (baseTouchpadDirectionalGroup.IsTouchpadDirectionsAnyVirtualMappingPresent || baseTouchpadDirectionalGroup.LeftDirectionValue.IsRemaped || baseTouchpadDirectionalGroup.UpDirectionValue.IsRemaped || baseTouchpadDirectionalGroup.RightDirectionValue.IsRemaped || baseTouchpadDirectionalGroup.DownDirectionValue.IsRemaped)
							{
								if (baseTouchpadDirectionalGroup.TouchpadZoneClickRequired == null)
								{
									baseTouchpadDirectionalGroup.TouchpadZoneClickRequired = new bool?(baseTouchpadDirectionalGroup.DefaultTouchpadZoneClickRequired);
								}
								else
								{
									baseTouchpadDirectionalGroup.ForceTouchpadZoneClickRequiredInternal(value);
								}
							}
							else
							{
								baseTouchpadDirectionalGroup.TouchpadZoneClickRequired = value;
							}
						}
					}
					this.GetJumpToShiftList().ForEach(delegate(int item)
					{
						BaseXBBindingCollection collectionByLayer = this.HostCollection.SubConfigData.MainXBBindingCollection.GetCollectionByLayer(item);
						BaseTouchpadDirectionalGroup <>4__this = this;
						BaseTouchpadDirectionalGroup baseTouchpadDirectionalGroup2;
						if (!(<>4__this is Touchpad1DirectionalGroup))
						{
							if (!(<>4__this is Touchpad2DirectionalGroup))
							{
								throw new Exception("Unexpected this type in BaseTouchpadDirectionalGroup.TouchpadAnalogMode setter");
							}
							baseTouchpadDirectionalGroup2 = collectionByLayer.Touchpad2DirectionalGroup;
						}
						else
						{
							baseTouchpadDirectionalGroup2 = collectionByLayer.Touchpad1DirectionalGroup;
						}
						baseTouchpadDirectionalGroup2.TouchpadZoneClickRequired = value;
					});
					base.IsChanged = true;
				}
			}
		}

		public void ForceTouchpadZoneClickRequiredInternal(bool? value)
		{
			this._touchpadZoneClickRequired = value;
		}

		public override bool IsYInvert
		{
			get
			{
				return (base.UpDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonLeftStickDown && base.DownDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonLeftStickUp) || (base.UpDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonRightStickDown && base.DownDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonRightStickUp) || (base.UpDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseYUp && base.DownDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseYDown);
			}
		}

		public override bool IsXInvert
		{
			get
			{
				return (base.LeftDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonLeftStickRight && base.RightDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonLeftStickLeft) || (base.LeftDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonRightStickRight && base.RightDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonRightStickLeft) || (base.LeftDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseXRight && base.RightDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseXLeft);
			}
		}

		public override bool InvertControlsEnabled
		{
			get
			{
				return base.IsBoundToVirtualLeftStick || base.IsBoundToVirtualRightStick || base.IsBoundToAnyInvertedMouse || this.IsYInvert || this.IsXInvert;
			}
		}

		public override bool StickResponseControlsEnabled
		{
			get
			{
				return base.IsBoundToVirtualLeftStick || base.IsBoundToVirtualRightStick || base.IsBoundToAnyInvertedMouse;
			}
		}

		public override bool ResetToDefault(bool askConfirmation = true)
		{
			bool flag = base.ResetToDefault(askConfirmation);
			if (flag)
			{
				if (this.TouchpadAnalogMode)
				{
					reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(SVGGamepadWithAllAnnotations));
				}
				this.ClickValue.RevertRemap();
				this.TapValue.RevertRemap();
			}
			return flag;
		}

		protected override bool CheckXYZoneDefault()
		{
			return !this.IsDigitalMode || this._xLow == this.DefaultXLow;
		}

		public override void Unmap()
		{
			base.Unmap();
			base.IsHardwareDeadzone = false;
			ObservableCollection<ThumbstickSensitivity> sensitivitiesCollection = base.SensitivitiesCollection;
			base.SensitivitySensitivity = ((sensitivitiesCollection != null) ? sensitivitiesCollection.FirstOrDefault<ThumbstickSensitivity>() : null);
			this.OnPropertyChanged("StickResponseControlsEnabled");
		}

		public override void RevertRemapToDefault()
		{
			base.RevertRemapToDefault();
			base.IsHardwareDeadzone = false;
			ObservableCollection<ThumbstickSensitivity> sensitivitiesCollection = base.SensitivitiesCollection;
			base.SensitivitySensitivity = ((sensitivitiesCollection != null) ? sensitivitiesCollection.FirstOrDefault<ThumbstickSensitivity>() : null);
			this.OnPropertyChanged("StickResponseControlsEnabled");
		}

		protected override void ResetActivatorXBBindingDictionaries()
		{
			base.ResetActivatorXBBindingDictionaries();
			this.ClickValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.Click);
			this.TapValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.Tap);
		}

		protected override void SetDefaultValues(bool silent = false)
		{
			base.SetDefaultValues(silent);
			if (silent)
			{
				this._touchpadAnalogMode = this.DefaultTouchpadAnalogMode;
				this._touchpadDigitalMode = !this.DefaultTouchpadAnalogMode;
				this._touchpadZoneClickRequired = null;
			}
			else
			{
				this.TouchpadAnalogMode = this.DefaultTouchpadAnalogMode;
				this.TouchpadZoneClickRequired = null;
			}
			this.SetDefaultValuesExceptMode(silent);
		}

		private void SetDefaultValuesExceptMode(bool silent)
		{
			if (silent)
			{
				this._springMode = false;
				this._touchpadZoneClickRequired = null;
				this._xSensitivity = 13573;
				this._ySensitivity = 13573;
				this._scaleFactorX = (uint)this.DefaultScaleFactor;
				this._scaleFactorY = (uint)this.DefaultScaleFactor;
				return;
			}
			base.SpringMode = false;
			this.TouchpadZoneClickRequired = null;
			base.XSensitivity = 13573;
			base.YSensitivity = 13573;
			base.ScaleFactorX = (uint)this.DefaultScaleFactor;
			base.ScaleFactorY = (uint)this.DefaultScaleFactor;
		}

		public override void UpdateProperties()
		{
			base.UpdateProperties();
			this.OnPropertyChanged("IsAnyDirectionHasJumpToShift");
			this.OnPropertyChanged("IsTouchpadAnyVirtualMappingPresent");
			this.OnPropertyChanged("IsTouchpadDirectionsAnyVirtualMappingPresent");
			this.OnPropertyChanged("TouchpadDigitalMode");
			this.OnPropertyChanged("TouchpadAnalogMode");
			this.OnPropertyChanged("IsButtonMappingVisible");
			this.OnPropertyChanged("IsAdvancedSettingsVisible");
			this.OnPropertyChanged("IsDigitalMode");
			this.OnPropertyChanged("TouchpadZoneClickRequired");
			this.OnPropertyChanged("IsSpringModeAllowed");
			this.OnPropertyChanged("AdvancedDigitalMappingSettingsExist");
			this.OnPropertyChanged("AdvancedDigitalMappingSettingsDisabledTooltip");
			this.OnPropertyChanged("IsFlickStickGroupMappingAvailable");
			base.HostCollection.IsGamepadMappingAvailiableForCurrentBindingChanged();
		}

		private void FillJumpToShiftList(List<int> list, XBBinding xbBinding)
		{
			xbBinding.ActivatorXBBindingDictionary.ForEachValue(delegate(ActivatorXBBinding item)
			{
				if (item.JumpToShift != -1 && !list.Contains(item.JumpToShift))
				{
					list.Add(item.JumpToShift);
				}
			});
		}

		private List<int> GetJumpToShiftList()
		{
			List<int> list = new List<int>();
			this.FillJumpToShiftList(list, base.UpDirectionValue);
			this.FillJumpToShiftList(list, base.DownDirectionValue);
			this.FillJumpToShiftList(list, base.LeftDirectionValue);
			this.FillJumpToShiftList(list, base.RightDirectionValue);
			return list;
		}

		public bool IsTouchpadAnyVirtualMappingPresent
		{
			get
			{
				return this.IsTouchpadDirectionsAnyVirtualMappingPresent || this.ClickValue.IsAnyActivatorVirtualMappingPresent || this.TapValue.IsAnyActivatorVirtualMappingPresent;
			}
		}

		public bool IsTouchpadDirectionsAnyVirtualMappingPresent
		{
			get
			{
				return base.IsAnyDirectionVirtualMappingPresent || base.IsAnyDiagonalDirectionVirtualMappingPresent;
			}
		}

		protected override ObservableCollection<ThumbstickSensitivity> CreateSensitivities()
		{
			return new ObservableCollection<ThumbstickSensitivity>
			{
				this.CreateDefaultSensitivity(),
				this.CreateCustomSensitivity()
			};
		}

		protected override ThumbstickSensitivity CreateDefaultSensitivity()
		{
			return new ThumbstickSensitivity("Default", new Localizable(11621), 8190, 9584, 28000, 32768, false);
		}

		protected override ThumbstickSensitivity CreateCustomSensitivity()
		{
			return new ThumbstickSensitivity("Custom", new Localizable(11620), 6143, 7188, 10237, 11980, 25953, 30372, 28000, 32768, true);
		}

		protected void SetTouchpadAnalogMode(bool value)
		{
			if (this._touchpadAnalogMode == value)
			{
				return;
			}
			if (base.IsAnyDirectionHasJumpToShift)
			{
				base.ClearDirectionAllJumpToShift();
			}
			this._touchpadDigitalMode = !value;
			this._touchpadAnalogMode = value;
			ushort num = this.DefaultXLow;
			ushort num2 = this.DefaultYLow;
			if (this._touchpadDigitalMode)
			{
				if (base.IsTrackPad1)
				{
					num = 7849;
					num2 = 7849;
				}
				if (base.IsTrackPad2)
				{
					num = 8689;
					num2 = 8689;
				}
			}
			base.XLow = (this._touchpadAnalogMode ? ((ushort)this.DeadZoneMinimum) : num);
			base.YLow = (this._touchpadAnalogMode ? ((ushort)this.DeadZoneMinimum) : num2);
			base.ResetActivatorXBBindingDictionaries();
			base.RevertRemapToDefault();
			this.SetDefaultValuesExceptMode(true);
			base.CructhForFirePropertyChanged = !base.CructhForFirePropertyChanged;
			MainXBBindingCollection mainXBBindingCollection = base.HostCollection as MainXBBindingCollection;
			if (mainXBBindingCollection != null)
			{
				foreach (ShiftXBBindingCollection shiftXBBindingCollection in mainXBBindingCollection.ShiftXBBindingCollections)
				{
					BaseTouchpadDirectionalGroup baseTouchpadDirectionalGroup;
					if (!(this is Touchpad1DirectionalGroup))
					{
						if (!(this is Touchpad2DirectionalGroup))
						{
							throw new Exception("Unexpected this type in BaseTouchpadDirectionalGroup.TouchpadAnalogMode setter");
						}
						baseTouchpadDirectionalGroup = shiftXBBindingCollection.Touchpad2DirectionalGroup;
					}
					else
					{
						baseTouchpadDirectionalGroup = shiftXBBindingCollection.Touchpad1DirectionalGroup;
					}
					if (!baseTouchpadDirectionalGroup.IsTouchpadDirectionsAnyVirtualMappingPresent)
					{
						baseTouchpadDirectionalGroup.SetTouchpadAnalogMode(value);
					}
				}
			}
			base.IsChanged = true;
			this.UpdateProperties();
		}

		public void CopyFromModel(BaseTouchpadDirectionalGroup model)
		{
			this._touchpadAnalogMode = model.TouchpadAnalogMode;
			this._touchpadDigitalMode = model.TouchpadDigitalMode;
			this._touchpadZoneClickRequired = model.TouchpadZoneClickRequired;
			base.CopyFromModel(model);
		}

		public void CopyToModel(BaseTouchpadDirectionalGroup model)
		{
			model.TouchpadAnalogMode = this._touchpadAnalogMode;
			model.TouchpadDigitalMode = this._touchpadDigitalMode;
			model.TouchpadZoneClickRequired = this._touchpadZoneClickRequired;
			base.CopyToModel(model);
		}

		private bool _touchpadAnalogMode;

		private bool _touchpadDigitalMode;

		private bool? _touchpadZoneClickRequired;
	}
}
