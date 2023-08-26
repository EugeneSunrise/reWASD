using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using DiscSoft.NET.Common.Localization;
using Prism.Commands;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.KeyBindings.ActivatorXB;
using reWASDUI.Infrastructure.KeyBindings.MacroBinding;
using reWASDUI.Infrastructure.KeyBindings.XB;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.KeyBindingsModel.XBBindingDirectionalGroups;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Infrastructure.XBEliteService;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.GenericInheritance;
using XBEliteWPF.Utils.Wrappers;

namespace reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups
{
	public abstract class BaseDirectionalAnalogGroup : BaseDirectionalGroup
	{
		public virtual string UncheckedSpringTooltip
		{
			get
			{
				return null;
			}
		}

		public virtual string CheckedSpringTooltip
		{
			get
			{
				return DTLocalization.GetString(11407);
			}
		}

		private bool IsLeftStick
		{
			get
			{
				return this is LeftStickDirectionalGroup;
			}
		}

		private bool IsRightStick
		{
			get
			{
				return this is RightStickDirectionalGroup;
			}
		}

		private bool IsGyroTilt
		{
			get
			{
				return this is GyroTiltDirectionalGroup;
			}
		}

		public bool IsTrackPad1
		{
			get
			{
				return this is Touchpad1DirectionalGroup;
			}
		}

		public bool IsTrackPad2
		{
			get
			{
				return this is Touchpad2DirectionalGroup;
			}
		}

		public bool IsMouseStick
		{
			get
			{
				return this is MouseDirectionalGroup;
			}
		}

		private bool IsAnyTrackPad
		{
			get
			{
				return this.IsTrackPad1 || this.IsTrackPad2;
			}
		}

		public ushort XLow
		{
			get
			{
				if (this.IsZoneRangeInherited)
				{
					bool flag = this.IsTrackPad2 && (this.IsDigitalMode != this.MainDirectionalAnalogGroup.IsDigitalMode || (base.HostCollection is ShiftXBBindingCollection && base.IsAnyDirectionVirtualMappingPresent && !this.IsDigitalMode));
					if (this._xLow == this.DefaultXLow && !flag)
					{
						return this.MainDirectionalAnalogGroup._xLow;
					}
				}
				if (this.IsSpringModeAllowed && this.SpringMode)
				{
					return 0;
				}
				return this._xLow;
			}
			set
			{
				if (this.SetProperty<ushort>(ref this._xLow, value, "XLow"))
				{
					if (this.IsHardwareDeadzone)
					{
						if (this.IsEllipticZoning)
						{
							this.YLow = value;
						}
						if (this.Sensitivity != null)
						{
							this.SetDeadZoneOnDeflection(this.Sensitivity, value, true);
						}
					}
					if (this.IsRadialZoning || !this.IsRadialDeadzoneAllowed || !this.IsZonesShapeChangeAllowed)
					{
						this.YLow = this.XLow;
					}
					base.IsChanged = true;
				}
			}
		}

		public ushort XMed
		{
			get
			{
				if (this.IsZoneRangeInherited)
				{
					bool flag = this.IsTrackPad2 && (this.IsDigitalMode != this.MainDirectionalAnalogGroup.IsDigitalMode || (base.HostCollection is ShiftXBBindingCollection && base.IsAnyDirectionVirtualMappingPresent && !this.IsDigitalMode));
					if (this._xMed == this.DefaultXMed && !flag)
					{
						return this.MainDirectionalAnalogGroup._xMed;
					}
				}
				return this._xMed;
			}
			set
			{
				if (this.SetProperty<ushort>(ref this._xMed, value, "XMed"))
				{
					base.IsChanged = true;
					if (this.IsRadialZoning || !this.IsZonesShapeChangeAllowed)
					{
						this.YMed = this.XMed;
					}
				}
			}
		}

		public ushort XHigh
		{
			get
			{
				if (this.IsZoneRangeInherited)
				{
					bool flag = this.IsTrackPad2 && (this.IsDigitalMode != this.MainDirectionalAnalogGroup.IsDigitalMode || (base.HostCollection is ShiftXBBindingCollection && base.IsAnyDirectionVirtualMappingPresent && !this.IsDigitalMode));
					if (this._xHigh == this.DefaultXHigh && !flag)
					{
						return this.MainDirectionalAnalogGroup._xHigh;
					}
				}
				return this._xHigh;
			}
			set
			{
				if (this.SetProperty<ushort>(ref this._xHigh, value, "XHigh"))
				{
					base.IsChanged = true;
					if (this.IsRadialZoning || !this.IsZonesShapeChangeAllowed)
					{
						this.YHigh = this.XHigh;
					}
				}
			}
		}

		public ushort YLow
		{
			get
			{
				if (!this.IsZonesShapeChangeAllowed)
				{
					return this.XLow;
				}
				if (this.IsZoneRangeInherited)
				{
					bool flag = this.IsTrackPad2 && (this.IsDigitalMode != this.MainDirectionalAnalogGroup.IsDigitalMode || (base.HostCollection is ShiftXBBindingCollection && base.IsAnyDirectionVirtualMappingPresent && !this.IsDigitalMode));
					if (this._yLow == this.DefaultYLow && !flag)
					{
						return this.MainDirectionalAnalogGroup._yLow;
					}
				}
				return this._yLow;
			}
			set
			{
				if (this.SetProperty<ushort>(ref this._yLow, value, "YLow"))
				{
					if (this.IsHardwareDeadzone && this.IsEllipticZoning)
					{
						this.XLow = value;
					}
					base.IsChanged = true;
				}
			}
		}

		public ushort YMed
		{
			get
			{
				if (!this.IsZonesShapeChangeAllowed)
				{
					return this.XMed;
				}
				if (this.IsZoneRangeInherited)
				{
					bool flag = this.IsTrackPad2 && (this.IsDigitalMode != this.MainDirectionalAnalogGroup.IsDigitalMode || (base.HostCollection is ShiftXBBindingCollection && base.IsAnyDirectionVirtualMappingPresent && !this.IsDigitalMode));
					if (this._yMed == this.DefaultYMed && !flag)
					{
						return this.MainDirectionalAnalogGroup._yMed;
					}
				}
				return this._yMed;
			}
			set
			{
				if (this.SetProperty<ushort>(ref this._yMed, value, "YMed"))
				{
					base.IsChanged = true;
				}
			}
		}

		public ushort YHigh
		{
			get
			{
				if (!this.IsZonesShapeChangeAllowed)
				{
					return this.XHigh;
				}
				if (this.IsZoneRangeInherited)
				{
					bool flag = this.IsTrackPad2 && (this.IsDigitalMode != this.MainDirectionalAnalogGroup.IsDigitalMode || (base.HostCollection is ShiftXBBindingCollection && base.IsAnyDirectionVirtualMappingPresent && !this.IsDigitalMode));
					if (this._yHigh == this.DefaultYHigh && !flag)
					{
						return this.MainDirectionalAnalogGroup._yHigh;
					}
				}
				return this._yHigh;
			}
			set
			{
				if (this.SetProperty<ushort>(ref this._yHigh, value, "YHigh"))
				{
					base.IsChanged = true;
				}
			}
		}

		public bool IsRadialZoning
		{
			get
			{
				return this._isRadialZoning;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isRadialZoning, value, "IsRadialZoning"))
				{
					if (value)
					{
						this.YHigh = this.XHigh;
						this.YMed = this.XMed;
						this.YLow = this.XLow;
						this.ScaleFactorY = this.ScaleFactorX;
					}
					this.IsEllipticZoning = !value;
				}
			}
		}

		public bool IsEllipticZoning
		{
			get
			{
				return this._isEllipticZoning;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isEllipticZoning, value, "IsEllipticZoning"))
				{
					this.IsRadialZoning = !value;
				}
			}
		}

		public bool IsZoneChangesPresent()
		{
			return this.XLow != this.DefaultXLow || this.YLow != this.DefaultYLow || this.XMed != this.DefaultXMed || this.YMed != this.DefaultYMed || this.XHigh != this.DefaultXHigh || this.YHigh != this.DefaultYHigh;
		}

		public ushort XSensitivity
		{
			get
			{
				if (this.IsStickCrossZonesInherited && this._xSensitivity == 13573)
				{
					return this.MainDirectionalAnalogGroup._xSensitivity;
				}
				return this._xSensitivity;
			}
			set
			{
				if (this.SetProperty<ushort>(ref this._xSensitivity, value, "XSensitivity"))
				{
					base.IsChanged = true;
				}
			}
		}

		public ushort YSensitivity
		{
			get
			{
				if (this.IsStickCrossZonesInherited && this._ySensitivity == 13573)
				{
					return this.MainDirectionalAnalogGroup._ySensitivity;
				}
				return this._ySensitivity;
			}
			set
			{
				if (this.SetProperty<ushort>(ref this._ySensitivity, value, "YSensitivity"))
				{
					base.IsChanged = true;
				}
			}
		}

		public bool SpringMode
		{
			get
			{
				return this._springMode;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._springMode, value, "SpringMode"))
				{
					base.IsChanged = true;
					this.OnPropertyChanged("XLow");
					this.OnPropertyChanged("IsZonesAllowed");
					this.OnPropertyChanged("RadialMode");
				}
			}
		}

		public bool RadialMode
		{
			get
			{
				return !this._springMode;
			}
			set
			{
				if (value != this.RadialMode)
				{
					this.SpringMode = !value;
				}
			}
		}

		public byte ReturnTime
		{
			get
			{
				return this._returnTime;
			}
			set
			{
				if (this.SetProperty<byte>(ref this._returnTime, value, "ReturnTime"))
				{
					base.IsChanged = true;
				}
			}
		}

		public uint Smoothing
		{
			get
			{
				return this._smoothing;
			}
			set
			{
				if (this.SetProperty<uint>(ref this._smoothing, value, "Smoothing"))
				{
					base.IsChanged = true;
				}
			}
		}

		public uint NoiseFilter
		{
			get
			{
				return this._noiseFilter;
			}
			set
			{
				if (this.SetProperty<uint>(ref this._noiseFilter, value, "NoiseFilter"))
				{
					base.IsChanged = true;
				}
			}
		}

		public virtual double ScaleFactorMaximum
		{
			get
			{
				return 5000.0;
			}
		}

		public virtual double ScaleFactorMinimum
		{
			get
			{
				return 5.0;
			}
		}

		public uint ScaleFactorX
		{
			get
			{
				return this._scaleFactorX;
			}
			set
			{
				if (this.SetProperty<uint>(ref this._scaleFactorX, value, "ScaleFactorX"))
				{
					base.IsChanged = true;
					if (this.IsRadialZoning)
					{
						this.OnPropertyChanged("ScaleFactorY");
					}
				}
			}
		}

		public uint ScaleFactorY
		{
			get
			{
				if (!this.IsRadialZoning)
				{
					return this._scaleFactorY;
				}
				return this._scaleFactorX;
			}
			set
			{
				if (this.SetProperty<uint>(ref this._scaleFactorY, value, "ScaleFactorY"))
				{
					base.IsChanged = true;
				}
			}
		}

		public short Rotation
		{
			get
			{
				return this.MainDirectionalAnalogGroup._rotation;
			}
			set
			{
				if (this.SetProperty<short>(ref this._rotation, value, "Rotation"))
				{
					base.IsChanged = true;
				}
			}
		}

		public virtual bool IsRotationAvailable
		{
			get
			{
				return !this.IsGyroTilt && !this.IsMouseStick;
			}
		}

		public virtual bool IsMouseYSensitivityAvailable
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsMouseSmoothingAvailable
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsStickCrossZonesChangeAvailable
		{
			get
			{
				return true;
			}
		}

		public virtual bool IsResetToDefaultButtonAllowed
		{
			get
			{
				return true;
			}
		}

		public virtual bool IsDigitalMode
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsButtonMappingVisible
		{
			get
			{
				return true;
			}
		}

		public virtual bool IsAdvancedSettingsVisible
		{
			get
			{
				return this.IsButtonMappingVisible;
			}
		}

		public virtual bool IsDiagonalDirectionsAllowed
		{
			get
			{
				return false;
			}
		}

		public virtual XBBinding CurrentSelectedDiagonalDirectionXBBinding
		{
			get
			{
				return null;
			}
		}

		public virtual GamepadButton CurrentSelectedDiagonalDirectionButton
		{
			get
			{
				return 2000;
			}
		}

		public IEnumerable<Direction> AllowedDirections
		{
			get
			{
				List<Direction> list = new List<Direction>(Enum.GetValues(typeof(Direction)).OfType<Direction>());
				if (!this.IsZoneLeanAllowed)
				{
					list.Remove(5);
					list.Remove(6);
				}
				if (!this.IsDigitalMode)
				{
					list.Remove(7);
					list.Remove(8);
					list.Remove(9);
					list.Remove(10);
				}
				if (this.IsDigitalMode)
				{
					list.Remove(3);
					list.Remove(4);
					list.Remove(2);
					list.Remove(1);
					list.Remove(0);
				}
				return list;
			}
		}

		public IEnumerable<Direction> DiagonalDirections
		{
			get
			{
				List<Direction> list = new List<Direction>(Enum.GetValues(typeof(Direction)).OfType<Direction>());
				list.Remove(3);
				list.Remove(4);
				list.Remove(2);
				list.Remove(1);
				list.Remove(0);
				list.Remove(5);
				list.Remove(6);
				return list;
			}
		}

		public Direction CurrentSelectedDirection
		{
			get
			{
				return this._currentSelectedDirection;
			}
			set
			{
				if (this.SetProperty<Direction>(ref this._currentSelectedDirection, value, "CurrentSelectedDirection"))
				{
					this.OnPropertyChanged("FilteredZoneButtons");
				}
			}
		}

		public Zone CurrentSelectedZone
		{
			get
			{
				return this._currentSelectedZone;
			}
			set
			{
				if (this.SetProperty<Zone>(ref this._currentSelectedZone, value, "CurrentSelectedZone"))
				{
					this.OnPropertyChanged("FilteredZoneButtons");
				}
				this.CurrentSelectedZoneButton = this.GetGampadButtonFromFilteredZoneButtons(this.CurrentSelectedZone);
			}
		}

		public GamepadButton CurrentSelectedZoneButton
		{
			get
			{
				return this._currentSelectedZoneButton;
			}
			set
			{
				this.SetProperty<GamepadButton>(ref this._currentSelectedZoneButton, value, "CurrentSelectedZoneButton");
			}
		}

		public Direction CurrentSelectedDiagonalDirection
		{
			get
			{
				return this._currentSelectedDiagonalDirection;
			}
			set
			{
				if (this.SetProperty<Direction>(ref this._currentSelectedDiagonalDirection, value, "CurrentSelectedDiagonalDirection"))
				{
					this.OnPropertyChanged("CurrentSelectedDiagonalDirectionXBBinding");
					this.OnPropertyChanged("CurrentSelectedDiagonalDirectionButton");
				}
			}
		}

		public GamepadButtonsObservableCollection ZoneButtons
		{
			get
			{
				GamepadButtonsObservableCollection gamepadButtonsObservableCollection = new GamepadButtonsObservableCollection();
				if (this.IsZoneHighAllowed)
				{
					gamepadButtonsObservableCollection.Add(this.HighZone);
					if (this.IsZonesDirectionAllowed)
					{
						gamepadButtonsObservableCollection.Add(this.HighZoneLeft);
						gamepadButtonsObservableCollection.Add(this.HighZoneRight);
						gamepadButtonsObservableCollection.Add(this.HighZoneUp);
						gamepadButtonsObservableCollection.Add(this.HighZoneDown);
						if (this.IsZoneLeanAllowed)
						{
							gamepadButtonsObservableCollection.Add(this.HighZoneLeanLeft);
							gamepadButtonsObservableCollection.Add(this.HighZoneLeanRight);
						}
					}
				}
				if (this.IsZoneMedAllowed)
				{
					gamepadButtonsObservableCollection.Add(this.MedZone);
					if (this.IsZonesDirectionAllowed)
					{
						gamepadButtonsObservableCollection.Add(this.MedZoneLeft);
						gamepadButtonsObservableCollection.Add(this.MedZoneRight);
						gamepadButtonsObservableCollection.Add(this.MedZoneUp);
						gamepadButtonsObservableCollection.Add(this.MedZoneDown);
						if (this.IsZoneLeanAllowed)
						{
							gamepadButtonsObservableCollection.Add(this.MedZoneLeanLeft);
							gamepadButtonsObservableCollection.Add(this.MedZoneLeanRight);
						}
					}
				}
				if (this.IsZoneLowAllowed)
				{
					gamepadButtonsObservableCollection.Add(this.LowZone);
					if (this.IsZonesDirectionAllowed)
					{
						gamepadButtonsObservableCollection.Add(this.LowZoneLeft);
						gamepadButtonsObservableCollection.Add(this.LowZoneRight);
						gamepadButtonsObservableCollection.Add(this.LowZoneUp);
						gamepadButtonsObservableCollection.Add(this.LowZoneDown);
						if (this.IsZoneLeanAllowed)
						{
							gamepadButtonsObservableCollection.Add(this.LowZoneLeanLeft);
							gamepadButtonsObservableCollection.Add(this.LowZoneLeanRight);
						}
					}
				}
				return gamepadButtonsObservableCollection;
			}
		}

		public GamepadButtonsObservableCollection FilteredZoneButtons
		{
			get
			{
				GamepadButtonsObservableCollection gamepadButtonsObservableCollection = new GamepadButtonsObservableCollection();
				switch (this.CurrentSelectedDirection)
				{
				case 0:
					CollectionExtensions.AddRange<GamepadButton>(gamepadButtonsObservableCollection, this.ZoneButtons.Where((GamepadButton gb) => GamepadButtonExtensions.IsAnyNonDirectionalZone(gb)));
					break;
				case 1:
					CollectionExtensions.AddRange<GamepadButton>(gamepadButtonsObservableCollection, this.ZoneButtons.Where((GamepadButton gb) => GamepadButtonExtensions.IsAnyLeftDirectionalZone(gb)));
					break;
				case 2:
					CollectionExtensions.AddRange<GamepadButton>(gamepadButtonsObservableCollection, this.ZoneButtons.Where((GamepadButton gb) => GamepadButtonExtensions.IsAnyRightDirectionalZone(gb)));
					break;
				case 3:
					CollectionExtensions.AddRange<GamepadButton>(gamepadButtonsObservableCollection, this.ZoneButtons.Where((GamepadButton gb) => GamepadButtonExtensions.IsAnyUpDirectionalZone(gb)));
					break;
				case 4:
					CollectionExtensions.AddRange<GamepadButton>(gamepadButtonsObservableCollection, this.ZoneButtons.Where((GamepadButton gb) => GamepadButtonExtensions.IsAnyDownDirectionalZone(gb)));
					break;
				case 5:
					CollectionExtensions.AddRange<GamepadButton>(gamepadButtonsObservableCollection, this.ZoneButtons.Where((GamepadButton gb) => GamepadButtonExtensions.IsAnyLeanLeftDirectionalZone(gb)));
					break;
				case 6:
					CollectionExtensions.AddRange<GamepadButton>(gamepadButtonsObservableCollection, this.ZoneButtons.Where((GamepadButton gb) => GamepadButtonExtensions.IsAnyLeanRightDirectionalZone(gb)));
					break;
				}
				return gamepadButtonsObservableCollection;
			}
		}

		private GamepadButton GetGampadButtonFromFilteredZoneButtons(Zone zone)
		{
			return this.FilteredZoneButtons.FirstOrDefault((GamepadButton item) => GamepadButtonExtensions.ToZone(item) == zone);
		}

		public virtual double DeadZoneMinimum
		{
			get
			{
				return 100.0;
			}
		}

		public abstract GamepadButton UpLeft { get; }

		public abstract GamepadButton UpRight { get; }

		public abstract GamepadButton DownLeft { get; }

		public abstract GamepadButton DownRight { get; }

		public abstract GamepadButton LowZone { get; }

		public abstract GamepadButton MedZone { get; }

		public abstract GamepadButton HighZone { get; }

		public abstract GamepadButton LowZoneUp { get; }

		public abstract GamepadButton LowZoneDown { get; }

		public abstract GamepadButton LowZoneLeft { get; }

		public abstract GamepadButton LowZoneRight { get; }

		public abstract GamepadButton LowZoneLeanLeft { get; }

		public abstract GamepadButton LowZoneLeanRight { get; }

		public abstract GamepadButton MedZoneUp { get; }

		public abstract GamepadButton MedZoneDown { get; }

		public abstract GamepadButton MedZoneLeft { get; }

		public abstract GamepadButton MedZoneRight { get; }

		public abstract GamepadButton MedZoneLeanLeft { get; }

		public abstract GamepadButton MedZoneLeanRight { get; }

		public abstract GamepadButton HighZoneUp { get; }

		public abstract GamepadButton HighZoneDown { get; }

		public abstract GamepadButton HighZoneLeft { get; }

		public abstract GamepadButton HighZoneRight { get; }

		public abstract GamepadButton HighZoneLeanLeft { get; }

		public abstract GamepadButton HighZoneLeanRight { get; }

		public XBBinding UpLeftValue
		{
			get
			{
				return base.HostCollection[this.UpLeft];
			}
			set
			{
				base.HostCollection[this.UpLeft] = value;
			}
		}

		public XBBinding UpRightValue
		{
			get
			{
				return base.HostCollection[this.UpRight];
			}
			set
			{
				base.HostCollection[this.UpRight] = value;
			}
		}

		public XBBinding DownLeftValue
		{
			get
			{
				return base.HostCollection[this.DownLeft];
			}
			set
			{
				base.HostCollection[this.DownLeft] = value;
			}
		}

		public XBBinding DownRightValue
		{
			get
			{
				return base.HostCollection[this.DownRight];
			}
			set
			{
				base.HostCollection[this.DownRight] = value;
			}
		}

		public XBBinding LowZoneValue
		{
			get
			{
				if (this.IsZoneMappingsInherited)
				{
					return this.MainDirectionalAnalogGroup.HostCollection[this.LowZone];
				}
				if (!this.IsZoneLowAllowed)
				{
					return null;
				}
				return base.HostCollection[this.LowZone];
			}
			set
			{
				base.HostCollection[this.LowZone] = value;
			}
		}

		public XBBinding MedZoneValue
		{
			get
			{
				if (this.IsZoneMappingsInherited)
				{
					return this.MainDirectionalAnalogGroup.HostCollection[this.MedZone];
				}
				if (!this.IsZoneMedAllowed)
				{
					return null;
				}
				return base.HostCollection[this.MedZone];
			}
			set
			{
				base.HostCollection[this.MedZone] = value;
			}
		}

		public XBBinding HighZoneValue
		{
			get
			{
				if (this.IsZoneMappingsInherited)
				{
					return this.MainDirectionalAnalogGroup.HostCollection[this.HighZone];
				}
				if (!this.IsZoneHighAllowed)
				{
					return null;
				}
				return base.HostCollection[this.HighZone];
			}
			set
			{
				base.HostCollection[this.HighZone] = value;
			}
		}

		public XBBinding LowZoneLeftValue
		{
			get
			{
				if (this.IsZoneMappingsInherited)
				{
					return this.MainDirectionalAnalogGroup.HostCollection[this.LowZoneLeft];
				}
				if (!this.IsZoneLowAllowed || !this.IsZonesDirectionAllowed)
				{
					return null;
				}
				return base.HostCollection[this.LowZoneLeft];
			}
			set
			{
				base.HostCollection[this.LowZoneLeft] = value;
			}
		}

		public XBBinding MedZoneLeftValue
		{
			get
			{
				if (this.IsZoneMappingsInherited)
				{
					return this.MainDirectionalAnalogGroup.HostCollection[this.MedZoneLeft];
				}
				if (!this.IsZoneMedAllowed || !this.IsZonesDirectionAllowed)
				{
					return null;
				}
				return base.HostCollection[this.MedZoneLeft];
			}
			set
			{
				base.HostCollection[this.MedZoneLeft] = value;
			}
		}

		public XBBinding HighZoneLeftValue
		{
			get
			{
				if (this.IsZoneMappingsInherited)
				{
					return this.MainDirectionalAnalogGroup.HostCollection[this.HighZoneLeft];
				}
				if (!this.IsZoneHighAllowed || !this.IsZonesDirectionAllowed)
				{
					return null;
				}
				return base.HostCollection[this.HighZoneLeft];
			}
			set
			{
				base.HostCollection[this.HighZoneLeft] = value;
			}
		}

		public XBBinding LowZoneUpValue
		{
			get
			{
				if (this.IsZoneMappingsInherited)
				{
					return this.MainDirectionalAnalogGroup.HostCollection[this.LowZoneUp];
				}
				if (!this.IsZoneLowAllowed || !this.IsZonesDirectionAllowed)
				{
					return null;
				}
				return base.HostCollection[this.LowZoneUp];
			}
			set
			{
				base.HostCollection[this.LowZoneUp] = value;
			}
		}

		public XBBinding MedZoneUpValue
		{
			get
			{
				if (this.IsZoneMappingsInherited)
				{
					return this.MainDirectionalAnalogGroup.HostCollection[this.MedZoneUp];
				}
				if (!this.IsZoneMedAllowed || !this.IsZonesDirectionAllowed)
				{
					return null;
				}
				return base.HostCollection[this.MedZoneUp];
			}
			set
			{
				base.HostCollection[this.MedZoneUp] = value;
			}
		}

		public XBBinding HighZoneUpValue
		{
			get
			{
				if (this.IsZoneMappingsInherited)
				{
					return this.MainDirectionalAnalogGroup.HostCollection[this.HighZoneUp];
				}
				if (!this.IsZoneHighAllowed || !this.IsZonesDirectionAllowed)
				{
					return null;
				}
				return base.HostCollection[this.HighZoneUp];
			}
			set
			{
				base.HostCollection[this.HighZoneUp] = value;
			}
		}

		public XBBinding LowZoneRightValue
		{
			get
			{
				if (this.IsZoneMappingsInherited)
				{
					return this.MainDirectionalAnalogGroup.HostCollection[this.LowZoneRight];
				}
				if (!this.IsZoneLowAllowed || !this.IsZonesDirectionAllowed)
				{
					return null;
				}
				return base.HostCollection[this.LowZoneRight];
			}
			set
			{
				base.HostCollection[this.LowZoneRight] = value;
			}
		}

		public XBBinding MedZoneRightValue
		{
			get
			{
				if (this.IsZoneMappingsInherited)
				{
					return this.MainDirectionalAnalogGroup.HostCollection[this.MedZoneRight];
				}
				if (!this.IsZoneMedAllowed || !this.IsZonesDirectionAllowed)
				{
					return null;
				}
				return base.HostCollection[this.MedZoneRight];
			}
			set
			{
				base.HostCollection[this.MedZoneRight] = value;
			}
		}

		public XBBinding HighZoneRightValue
		{
			get
			{
				if (this.IsZoneMappingsInherited)
				{
					return this.MainDirectionalAnalogGroup.HostCollection[this.HighZoneRight];
				}
				if (!this.IsZoneHighAllowed || !this.IsZonesDirectionAllowed)
				{
					return null;
				}
				return base.HostCollection[this.HighZoneRight];
			}
			set
			{
				base.HostCollection[this.HighZoneRight] = value;
			}
		}

		public XBBinding LowZoneDownValue
		{
			get
			{
				if (this.IsZoneMappingsInherited)
				{
					return this.MainDirectionalAnalogGroup.HostCollection[this.LowZoneDown];
				}
				if (!this.IsZoneLowAllowed || !this.IsZonesDirectionAllowed)
				{
					return null;
				}
				return base.HostCollection[this.LowZoneDown];
			}
			set
			{
				base.HostCollection[this.LowZoneDown] = value;
			}
		}

		public XBBinding MedZoneDownValue
		{
			get
			{
				if (this.IsZoneMappingsInherited)
				{
					return this.MainDirectionalAnalogGroup.HostCollection[this.MedZoneDown];
				}
				if (!this.IsZoneMedAllowed || !this.IsZonesDirectionAllowed)
				{
					return null;
				}
				return base.HostCollection[this.MedZoneDown];
			}
			set
			{
				base.HostCollection[this.MedZoneDown] = value;
			}
		}

		public XBBinding HighZoneDownValue
		{
			get
			{
				if (this.IsZoneMappingsInherited)
				{
					return this.MainDirectionalAnalogGroup.HostCollection[this.HighZoneDown];
				}
				if (!this.IsZoneHighAllowed || !this.IsZonesDirectionAllowed)
				{
					return null;
				}
				return base.HostCollection[this.HighZoneDown];
			}
			set
			{
				base.HostCollection[this.HighZoneDown] = value;
			}
		}

		public XBBinding LowZoneLeanLeftValue
		{
			get
			{
				if (this.IsZoneMappingsInherited)
				{
					return this.MainDirectionalAnalogGroup.HostCollection[this.LowZoneLeanLeft];
				}
				if (!this.IsZoneLowAllowed || !this.IsZonesDirectionAllowed || !this.IsZoneLeanAllowed)
				{
					return null;
				}
				return base.HostCollection[this.LowZoneLeanLeft];
			}
			set
			{
				base.HostCollection[this.LowZoneLeanLeft] = value;
			}
		}

		public XBBinding LowZoneLeanRightValue
		{
			get
			{
				if (this.IsZoneMappingsInherited)
				{
					return this.MainDirectionalAnalogGroup.HostCollection[this.LowZoneLeanRight];
				}
				if (!this.IsZoneLowAllowed || !this.IsZonesDirectionAllowed || !this.IsZoneLeanAllowed)
				{
					return null;
				}
				return base.HostCollection[this.LowZoneLeanRight];
			}
			set
			{
				base.HostCollection[this.LowZoneLeanRight] = value;
			}
		}

		public XBBinding MedZoneLeanLeftValue
		{
			get
			{
				if (this.IsZoneMappingsInherited)
				{
					return this.MainDirectionalAnalogGroup.HostCollection[this.MedZoneLeanLeft];
				}
				if (!this.IsZoneMedAllowed || !this.IsZonesDirectionAllowed || !this.IsZoneLeanAllowed)
				{
					return null;
				}
				return base.HostCollection[this.MedZoneLeanLeft];
			}
			set
			{
				base.HostCollection[this.MedZoneLeanLeft] = value;
			}
		}

		public XBBinding MedZoneLeanRightValue
		{
			get
			{
				if (this.IsZoneMappingsInherited)
				{
					return this.MainDirectionalAnalogGroup.HostCollection[this.MedZoneLeanRight];
				}
				if (!this.IsZoneMedAllowed || !this.IsZonesDirectionAllowed || !this.IsZoneLeanAllowed)
				{
					return null;
				}
				return base.HostCollection[this.MedZoneLeanRight];
			}
			set
			{
				base.HostCollection[this.MedZoneLeanRight] = value;
			}
		}

		public XBBinding HighZoneLeanLeftValue
		{
			get
			{
				if (this.IsZoneMappingsInherited)
				{
					return this.MainDirectionalAnalogGroup.HostCollection[this.HighZoneLeanLeft];
				}
				if (!this.IsZoneHighAllowed || !this.IsZonesDirectionAllowed || !this.IsZoneLeanAllowed)
				{
					return null;
				}
				return base.HostCollection[this.HighZoneLeanLeft];
			}
			set
			{
				base.HostCollection[this.HighZoneLeanLeft] = value;
			}
		}

		public XBBinding HighZoneLeanRightValue
		{
			get
			{
				if (this.IsZoneMappingsInherited)
				{
					return this.MainDirectionalAnalogGroup.HostCollection[this.HighZoneLeanRight];
				}
				if (!this.IsZoneHighAllowed || !this.IsZonesDirectionAllowed || !this.IsZoneLeanAllowed)
				{
					return null;
				}
				return base.HostCollection[this.HighZoneLeanRight];
			}
			set
			{
				base.HostCollection[this.HighZoneLeanRight] = value;
			}
		}

		public abstract bool IsHardwareChangesAllowed { get; }

		public abstract bool IsZonesAllowed { get; }

		public abstract bool IsZonesDirectionAllowed { get; }

		public abstract bool IsZoneLowAllowed { get; }

		public abstract bool IsZoneMedAllowed { get; }

		public abstract bool IsZoneHighAllowed { get; }

		public abstract bool IsZoneLeanAllowed { get; }

		public abstract bool IsZonesShapeChangeAllowed { get; }

		public abstract bool IsSpringModeAllowed { get; }

		public abstract bool IsAdvancedSpringDeadzoneVisible { get; }

		public abstract string ZonesExplanation { get; }

		public virtual string ZonesLabelAxisX
		{
			get
			{
				return DTLocalization.GetString(11612);
			}
		}

		public virtual string ZonesLabelAxisY
		{
			get
			{
				return DTLocalization.GetString(11613);
			}
		}

		public abstract bool IsXInvert { get; }

		public abstract bool IsYInvert { get; }

		public virtual bool IsZoneMappingsInherited
		{
			get
			{
				return false;
			}
		}

		public abstract bool IsZoneRangeInherited { get; }

		public virtual bool IsRadialDeadzoneAllowed
		{
			get
			{
				return true;
			}
		}

		public abstract bool IsStickCrossZonesInherited { get; }

		public bool IsHardwareDeadzone
		{
			get
			{
				return this._isHardwareDeadzone;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isHardwareDeadzone, value, "IsHardwareDeadzone"))
				{
					base.IsChanged = true;
					if (value)
					{
						this.YLow = this.XLow;
						ThumbstickSensitivity thumbstickSensitivity = this.SensitivitiesCollection.FirstOrDefault((ThumbstickSensitivity sens) => sens.IsCustom);
						if (thumbstickSensitivity != this.SensitivitySensitivity)
						{
							thumbstickSensitivity.Deflection = this.SensitivitySensitivity.Deflection.Clone();
							this.SetDeadzoneOnGraphic(value, thumbstickSensitivity.Deflection);
							this.SensitivitySensitivity = thumbstickSensitivity;
							return;
						}
						this.SetDeadzoneOnGraphic(value, thumbstickSensitivity.Deflection);
						return;
					}
					else
					{
						ThumbstickSensitivity thumbstickSensitivity2 = this.SensitivitiesCollection.FirstOrDefault((ThumbstickSensitivity sens) => sens.CheckEqualsForDeflection(this.Sensitivity));
						if (thumbstickSensitivity2 != null && thumbstickSensitivity2.Name == "Custom")
						{
							this.Sensitivity.HorizontalPoint[0].TravelDistance = (ushort)Math.Round((double)this.Sensitivity.HorizontalPoint[1].TravelDistance / 2.0);
							this.Sensitivity.HorizontalPoint[0].NewValue = (ushort)Math.Round((double)this.Sensitivity.HorizontalPoint[1].NewValue / 2.0);
							this.Sensitivity.VerticalPoint[0].TravelDistance = (ushort)Math.Round((double)this.Sensitivity.VerticalPoint[1].TravelDistance / 2.0);
							this.Sensitivity.VerticalPoint[0].NewValue = (ushort)Math.Round((double)this.Sensitivity.VerticalPoint[1].NewValue / 2.0);
						}
						this.SetDeadzoneOnGraphic(value, this.Sensitivity);
					}
				}
			}
		}

		public bool IsDiagonalDirections
		{
			get
			{
				return (!this.IsAnyTrackPad || this.IsDigitalMode) && this.IsDiagonalDirectionsAllowed && this._isDiagonalDirections;
			}
			set
			{
				if ((this.IsAnyTrackPad && !this.IsDigitalMode) || !this.IsDiagonalDirectionsAllowed)
				{
					value = false;
				}
				if (this.SetProperty<bool>(ref this._isDiagonalDirections, value, "IsDiagonalDirections"))
				{
					MainXBBindingCollection mainXBBindingCollection = base.HostCollection as MainXBBindingCollection;
					if (mainXBBindingCollection != null)
					{
						foreach (ShiftXBBindingCollection shiftXBBindingCollection in mainXBBindingCollection.ShiftXBBindingCollections)
						{
							BaseDirectionalAnalogGroup baseDirectionalAnalogGroup;
							if (!(this is Touchpad1DirectionalGroup))
							{
								if (!(this is Touchpad2DirectionalGroup))
								{
									if (!(this is GyroTiltDirectionalGroup))
									{
										if (!(this is MouseDirectionalGroup))
										{
											if (!(this is LeftStickDirectionalGroup))
											{
												if (!(this is RightStickDirectionalGroup))
												{
													if (!(this is AdditionalStickDirectionalGroup))
													{
														throw new Exception("Unexpected this type in BaseTouchpadDirectionalGroup.TouchpadAnalogMode setter");
													}
													baseDirectionalAnalogGroup = shiftXBBindingCollection.AdditionalStickDirectionalGroup;
												}
												else
												{
													baseDirectionalAnalogGroup = shiftXBBindingCollection.RightStickDirectionalGroup;
												}
											}
											else
											{
												baseDirectionalAnalogGroup = shiftXBBindingCollection.LeftStickDirectionalGroup;
											}
										}
										else
										{
											baseDirectionalAnalogGroup = shiftXBBindingCollection.MouseDirectionalGroup;
										}
									}
									else
									{
										baseDirectionalAnalogGroup = shiftXBBindingCollection.GyroTiltDirectionalGroup;
									}
								}
								else
								{
									baseDirectionalAnalogGroup = shiftXBBindingCollection.Touchpad2DirectionalGroup;
								}
							}
							else
							{
								baseDirectionalAnalogGroup = shiftXBBindingCollection.Touchpad1DirectionalGroup;
							}
							if (!baseDirectionalAnalogGroup.IsAnyDirectionVirtualMappingPresent && !baseDirectionalAnalogGroup.IsAnyDiagonalDirectionVirtualMappingPresent)
							{
								baseDirectionalAnalogGroup.IsDiagonalDirections = value;
							}
						}
					}
					base.IsChanged = true;
					this.OnPropertyChanged("StickResponseControlsIsVisible");
				}
			}
		}

		public bool IsAnyDiagonalDirectionVirtualMappingPresent
		{
			get
			{
				return this.IsDiagonalDirections && (this.UpLeftValue.IsAnyActivatorVirtualMappingPresent || this.UpRightValue.IsAnyActivatorVirtualMappingPresent || this.DownLeftValue.IsAnyActivatorVirtualMappingPresent || this.DownRightValue.IsAnyActivatorVirtualMappingPresent);
			}
		}

		public bool IsAnyDirectionOverlayMappingPresent
		{
			get
			{
				if (!base.UpDirectionValue.ActivatorXBBindingDictionary.Values.Any((ActivatorXBBinding x) => x.MappedKey.IsAnyOverlayMenuCommand))
				{
					if (!base.RightDirectionValue.ActivatorXBBindingDictionary.Values.Any((ActivatorXBBinding x) => x.MappedKey.IsAnyOverlayMenuCommand))
					{
						if (!base.LeftDirectionValue.ActivatorXBBindingDictionary.Values.Any((ActivatorXBBinding x) => x.MappedKey.IsAnyOverlayMenuCommand))
						{
							if (!base.DownDirectionValue.ActivatorXBBindingDictionary.Values.Any((ActivatorXBBinding x) => x.MappedKey.IsAnyOverlayMenuCommand))
							{
								if (this.IsDiagonalDirections)
								{
									if (!this.UpLeftValue.ActivatorXBBindingDictionary.Values.Any((ActivatorXBBinding x) => x.MappedKey.IsAnyOverlayMenuCommand))
									{
										if (!this.UpRightValue.ActivatorXBBindingDictionary.Values.Any((ActivatorXBBinding x) => x.MappedKey.IsAnyOverlayMenuCommand))
										{
											if (!this.DownLeftValue.ActivatorXBBindingDictionary.Values.Any((ActivatorXBBinding x) => x.MappedKey.IsAnyOverlayMenuCommand))
											{
												if (!this.DownRightValue.ActivatorXBBindingDictionary.Values.Any((ActivatorXBBinding x) => x.MappedKey.IsAnyOverlayMenuCommand))
												{
													return false;
												}
											}
										}
									}
									return true;
								}
								return false;
							}
						}
					}
				}
				return true;
			}
		}

		public BaseDirectionalAnalogGroup MainDirectionalAnalogGroup
		{
			get
			{
				MainXBBindingCollection mainXBBindingCollection = null;
				MainXBBindingCollection mainXBBindingCollection2 = base.HostCollection as MainXBBindingCollection;
				if (mainXBBindingCollection2 != null)
				{
					mainXBBindingCollection = mainXBBindingCollection2;
				}
				else
				{
					ShiftXBBindingCollection shiftXBBindingCollection = base.HostCollection as ShiftXBBindingCollection;
					if (shiftXBBindingCollection != null)
					{
						if (this is MouseDirectionalGroup)
						{
							return shiftXBBindingCollection.MouseDirectionalGroup;
						}
						mainXBBindingCollection = shiftXBBindingCollection.ParentBindingCollection;
					}
				}
				if (mainXBBindingCollection == null)
				{
					return null;
				}
				if (this is LeftStickDirectionalGroup)
				{
					return mainXBBindingCollection.LeftStickDirectionalGroup;
				}
				if (this is RightStickDirectionalGroup)
				{
					return mainXBBindingCollection.RightStickDirectionalGroup;
				}
				if (this is AdditionalStickDirectionalGroup)
				{
					return mainXBBindingCollection.AdditionalStickDirectionalGroup;
				}
				if (this is MouseDirectionalGroup)
				{
					return mainXBBindingCollection.MouseDirectionalGroup;
				}
				if (this is GyroTiltDirectionalGroup)
				{
					return mainXBBindingCollection.GyroTiltDirectionalGroup;
				}
				if (this is Touchpad1DirectionalGroup)
				{
					return mainXBBindingCollection.Touchpad1DirectionalGroup;
				}
				if (this is Touchpad2DirectionalGroup)
				{
					return mainXBBindingCollection.Touchpad2DirectionalGroup;
				}
				return null;
			}
		}

		protected GamepadButton DirectionToGamepadButton(Direction direction)
		{
			switch (direction)
			{
			case 1:
				return this.LeftDirection;
			case 2:
				return this.RightDirection;
			case 3:
				return this.UpDirection;
			case 4:
				return this.DownDirection;
			case 7:
				return this.UpLeft;
			case 8:
				return this.UpRight;
			case 9:
				return this.DownLeft;
			case 10:
				return this.DownRight;
			}
			return 2000;
		}

		protected XBBinding DirectionToXBBinding(Direction direction)
		{
			switch (direction)
			{
			case 1:
				return base.LeftDirectionValue;
			case 2:
				return base.RightDirectionValue;
			case 3:
				return base.UpDirectionValue;
			case 4:
				return base.DownDirectionValue;
			case 7:
				return this.UpLeftValue;
			case 8:
				return this.UpRightValue;
			case 9:
				return this.DownLeftValue;
			case 10:
				return this.DownRightValue;
			}
			return null;
		}

		public void InvertStickX(bool invertVM = true, bool invertHM = true)
		{
			ActivatorXBBindingDictionary activatorXBBindingDictionary = base.LeftDirectionValue.ActivatorXBBindingDictionary;
			ActivatorXBBindingDictionary activatorXBBindingDictionary2 = base.RightDirectionValue.ActivatorXBBindingDictionary;
			if (invertVM)
			{
				base.LeftDirectionValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary2, base.LeftDirectionValue.ControllerButton.Clone());
				base.RightDirectionValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary, base.RightDirectionValue.ControllerButton.Clone());
			}
			if (invertHM)
			{
				GamepadButtonDescription remapedTo = base.LeftDirectionValue.RemapedTo;
				base.LeftDirectionValue.RemapedTo = base.RightDirectionValue.RemapedTo;
				base.RightDirectionValue.RemapedTo = remapedTo;
			}
			base.IsChanged = true;
			this.OnPropertyChanged("IsXInvert");
		}

		public void InvertStickY(bool invertVM = true, bool invertHM = true)
		{
			ActivatorXBBindingDictionary activatorXBBindingDictionary = base.UpDirectionValue.ActivatorXBBindingDictionary;
			ActivatorXBBindingDictionary activatorXBBindingDictionary2 = base.DownDirectionValue.ActivatorXBBindingDictionary;
			if (invertVM)
			{
				base.UpDirectionValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary2, base.UpDirectionValue.ControllerButton.Clone());
				base.DownDirectionValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(activatorXBBindingDictionary, base.DownDirectionValue.ControllerButton.Clone());
			}
			if (invertHM)
			{
				GamepadButtonDescription remapedTo = base.UpDirectionValue.RemapedTo;
				base.UpDirectionValue.RemapedTo = base.DownDirectionValue.RemapedTo;
				base.DownDirectionValue.RemapedTo = remapedTo;
			}
			base.IsChanged = true;
			this.OnPropertyChanged("IsYInvert");
		}

		private void SetDeadzoneOnGraphic(bool isHardwareDeadzone, DISC_SOFT_GAMEPAD_STICK_DEFLECTION_NPROPCHANGE deflection)
		{
			if (deflection == null)
			{
				return;
			}
			if (isHardwareDeadzone)
			{
				this.SetDeadZoneOnDeflectionPointArray(deflection.HorizontalPoint, this.XLow);
				return;
			}
			if (!this.IsCustomResponse)
			{
				ThumbstickSensitivity thumbstickSensitivity = this.SensitivitiesCollection.FirstOrDefault((ThumbstickSensitivity sens) => sens.CheckEqualsForDeflection(deflection));
				if (thumbstickSensitivity != null)
				{
					ThumbstickSensitivity thumbstickSensitivity2 = this.CreateDefaultSensitivity();
					string name = thumbstickSensitivity.Name;
					if (!(name == "Default"))
					{
						if (!(name == "Delay"))
						{
							if (!(name == "Aggressive"))
							{
								if (!(name == "Instant"))
								{
									if (name == "Smooth")
									{
										thumbstickSensitivity2 = this.CreateSmoothSensitivity();
									}
								}
								else
								{
									thumbstickSensitivity2 = this.CreateInstantSensitivity();
								}
							}
							else
							{
								thumbstickSensitivity2 = this.CreateAggressiveSensitivity();
							}
						}
						else
						{
							thumbstickSensitivity2 = this.CreateDelaySensitivity();
						}
					}
					else
					{
						thumbstickSensitivity2 = this.CreateDefaultSensitivity();
					}
					if (!thumbstickSensitivity2.CheckEqualsForDeflection(thumbstickSensitivity.Deflection))
					{
						thumbstickSensitivity.Deflection = thumbstickSensitivity2.Deflection;
					}
					if (!thumbstickSensitivity2.CheckEqualsForDeflection(this.Sensitivity))
					{
						this.Sensitivity = thumbstickSensitivity2.Deflection;
					}
				}
			}
		}

		private void SetDeadZoneOnDeflection(DISC_SOFT_GAMEPAD_STICK_DEFLECTION_NPROPCHANGE deflection, ushort value, bool isHorizontal)
		{
			if (isHorizontal)
			{
				this.SetDeadZoneOnDeflectionPointArray(deflection.HorizontalPoint, value);
				return;
			}
			this.SetDeadZoneOnDeflectionPointArray(deflection.VerticalPoint, value);
		}

		private void SetDeadZoneOnDeflectionPointArray(DISC_SOFT_GAMEPAD_STICK_DEFLECTION_POINT_Wrapper[] deflectionPointArray, ushort value)
		{
			for (int i = 3; i > 0; i--)
			{
				if (deflectionPointArray[i].TravelDistance < value)
				{
					deflectionPointArray[i].TravelDistance = value;
				}
				deflectionPointArray[0].NewValue = 0;
				deflectionPointArray[0].TravelDistance = value;
			}
		}

		private void AttachDetachDeadzoneEventsForCustomResponse()
		{
			if (this.IsCustomResponse)
			{
				this.Sensitivity.HorizontalPoint[0].PropertyChanged += this.OnLeftStick0HorizontalPointNewValueChanged;
				this.Sensitivity.HorizontalPoint[1].PropertyChanged += this.OnLeftStick123HorizontalPointNewValueChanged;
				this.Sensitivity.HorizontalPoint[2].PropertyChanged += this.OnLeftStick123HorizontalPointNewValueChanged;
				this.Sensitivity.HorizontalPoint[3].PropertyChanged += this.OnLeftStick123HorizontalPointNewValueChanged;
				return;
			}
			this.Sensitivity.HorizontalPoint[0].PropertyChanged -= this.OnLeftStick0HorizontalPointNewValueChanged;
			this.Sensitivity.HorizontalPoint[1].PropertyChanged -= this.OnLeftStick123HorizontalPointNewValueChanged;
			this.Sensitivity.HorizontalPoint[2].PropertyChanged -= this.OnLeftStick123HorizontalPointNewValueChanged;
			this.Sensitivity.HorizontalPoint[3].PropertyChanged -= this.OnLeftStick123HorizontalPointNewValueChanged;
		}

		private void OnLeftStick0HorizontalPointNewValueChanged(object sender, PropertyChangedEventArgs e)
		{
			if (this.Sensitivity.HorizontalPoint[0].NewValue == 0 && (!(this is MouseDirectionalGroup) || !this.SpringMode))
			{
				this.XLow = this.Sensitivity.HorizontalPoint[0].TravelDistance;
				this.IsHardwareDeadzone = true;
			}
			else
			{
				this._isHardwareDeadzone = false;
				this.OnPropertyChanged("IsHardwareDeadzone");
			}
			base.IsChanged = true;
		}

		private void OnLeftStick123HorizontalPointNewValueChanged(object sender, PropertyChangedEventArgs e)
		{
			base.IsChanged = true;
		}

		public ThumbstickSensitivity SensitivitySensitivity
		{
			get
			{
				return this._sensitivitySensitivity;
			}
			set
			{
				if (this.SetProperty<ThumbstickSensitivity>(ref this._sensitivitySensitivity, value, "SensitivitySensitivity"))
				{
					this.Sensitivity = this._sensitivitySensitivity.Deflection;
					this.OnPropertyChanged("SensitivitySensitivity");
					this.OnPropertyChanged("IsCustomResponse");
					this.ClearResponseCommand.RaiseCanExecuteChanged();
					this.PasteResponseCommand.RaiseCanExecuteChanged();
					this.CopyResponseCommand.RaiseCanExecuteChanged();
				}
			}
		}

		public bool IsSensitivityDefault()
		{
			if (this.Sensitivity != null)
			{
				if (this.IsGyroTilt && this.Sensitivity.IsGyroDefault())
				{
					return true;
				}
				if (this.IsAnyTrackPad && this.Sensitivity.IsPhysicalTrackPadDefault())
				{
					return true;
				}
				if (this.Sensitivity.IsDefault())
				{
					return true;
				}
			}
			return false;
		}

		public DISC_SOFT_GAMEPAD_STICK_DEFLECTION_NPROPCHANGE Sensitivity
		{
			get
			{
				return this._sensitivity;
			}
			set
			{
				ThumbstickSensitivity thumbstickSensitivity = this.SensitivitiesCollection.FirstOrDefault((ThumbstickSensitivity sens) => sens.CheckEqualsForDeflection(value));
				if (thumbstickSensitivity == null)
				{
					ThumbstickSensitivity thumbstickSensitivity2 = this.SensitivitiesCollection.FirstOrDefault((ThumbstickSensitivity sens) => sens.IsCustom);
					thumbstickSensitivity2.Deflection = value;
					thumbstickSensitivity = thumbstickSensitivity2;
				}
				this._sensitivity = thumbstickSensitivity.Deflection;
				this.SensitivitySensitivity = thumbstickSensitivity;
				this.AttachDetachDeadzoneEventsForCustomResponse();
				base.IsChanged = true;
				this.OnPropertyChanged("Sensitivity");
				if ((base.IsLeftStickGroup || base.IsRightStickGroup) && this._sensitivity.HorizontalPoint[0].NewValue != 0)
				{
					this.IsHardwareDeadzone = false;
				}
			}
		}

		public bool IsSensitivityAllowedForController(ControllerTypeEnum controllerType, bool isAdvancedMappingFeatureUnlocked)
		{
			return isAdvancedMappingFeatureUnlocked || this.IsSensitivityDefault() || ((this.IsLeftStick || this.IsRightStick) && ControllerTypeExtensions.IsAllowedHardwareMappingWithoutAdvancedFeature(controllerType) && !this.Sensitivity.IsCustom());
		}

		public ObservableCollection<ThumbstickSensitivity> SensitivitiesCollection
		{
			get
			{
				ObservableCollection<ThumbstickSensitivity> observableCollection;
				if ((observableCollection = this._sensitivitiesCollection) == null)
				{
					observableCollection = (this._sensitivitiesCollection = this.CreateSensitivities());
				}
				return observableCollection;
			}
			set
			{
				this.SetProperty<ObservableCollection<ThumbstickSensitivity>>(ref this._sensitivitiesCollection, value, "SensitivitiesCollection");
			}
		}

		public bool IsCustomResponse
		{
			get
			{
				return this.SensitivitySensitivity.IsCustom;
			}
		}

		public ushort FlickStickSensitivity
		{
			get
			{
				return this._flickStickSensitivity;
			}
			set
			{
				if (this.SetProperty<ushort>(ref this._flickStickSensitivity, value, "FlickStickSensitivity"))
				{
					base.IsChanged = true;
				}
			}
		}

		public ushort FlickStickThreshold
		{
			get
			{
				return this._flickStickThreshold;
			}
			set
			{
				if (this.SetProperty<ushort>(ref this._flickStickThreshold, value, "FlickStickThreshold"))
				{
					base.IsChanged = true;
				}
			}
		}

		public uint FlickStickDuration
		{
			get
			{
				return this._flickStickDuration;
			}
			set
			{
				if (this.SetProperty<uint>(ref this._flickStickDuration, value, "FlickStickDuration"))
				{
					base.IsChanged = true;
				}
			}
		}

		public bool FlickStickIsInverted
		{
			get
			{
				return base.LeftDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseFlickRight;
			}
			set
			{
				if (value != this.FlickStickIsInverted)
				{
					if (value)
					{
						base.BindToFlickStickInverted();
						return;
					}
					base.BindToFlickStick();
				}
			}
		}

		public bool IsUseGlobalMouseSensitivity
		{
			get
			{
				return this._isUseGlobalMouseSensitivity;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isUseGlobalMouseSensitivity, value, "IsUseGlobalMouseSensitivity"))
				{
					base.IsChanged = true;
					this.OnPropertyChanged("MouseSensitivity");
					this.OnPropertyChanged("MouseSensitivityY");
				}
			}
		}

		public ushort MouseSensitivity
		{
			get
			{
				if (!this._isUseGlobalMouseSensitivity)
				{
					return this._mouseSensitivity;
				}
				return this.GlobalMouseSensitivity;
			}
			set
			{
				if (this.SetProperty<ushort>(ref this._mouseSensitivity, value, "MouseSensitivity"))
				{
					base.IsChanged = true;
				}
			}
		}

		public ushort MouseSensitivityY
		{
			get
			{
				if (!this._isUseGlobalMouseSensitivity)
				{
					return this._mouseSensitivityY;
				}
				return this.GlobalMouseSensitivity;
			}
			set
			{
				if (this.SetProperty<ushort>(ref this._mouseSensitivityY, value, "MouseSensitivityY"))
				{
					base.IsChanged = true;
				}
			}
		}

		public ushort GlobalMouseSensitivity
		{
			get
			{
				MainXBBindingCollection mainXBBindingCollection = base.HostCollection as MainXBBindingCollection;
				if (mainXBBindingCollection != null)
				{
					return mainXBBindingCollection.MouseSensitivity;
				}
				ShiftXBBindingCollection shiftXBBindingCollection = base.HostCollection as ShiftXBBindingCollection;
				if (shiftXBBindingCollection != null)
				{
					return shiftXBBindingCollection.ParentBindingCollection.MouseSensitivity;
				}
				throw new Exception("Unexpected Collection Type");
			}
		}

		public ushort MouseSmoothing
		{
			get
			{
				return this._mouseSmoothing;
			}
			set
			{
				if (this.SetProperty<ushort>(ref this._mouseSmoothing, value, "MouseSmoothing"))
				{
					base.IsChanged = true;
				}
			}
		}

		public ushort MouseFlickSmoothing
		{
			get
			{
				return this._mouseFlickSmoothing;
			}
			set
			{
				if (this.SetProperty<ushort>(ref this._mouseFlickSmoothing, value, "MouseFlickSmoothing"))
				{
					base.IsChanged = true;
				}
			}
		}

		public bool IsTrackballFriction
		{
			get
			{
				return this._isTrackballFriction;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isTrackballFriction, value, "IsTrackballFriction"))
				{
					base.IsChanged = true;
				}
			}
		}

		public ushort TrackballFriction
		{
			get
			{
				if (this._trackballFriction == 0 && (base.IsBoundToVirtualLeftStick || base.IsBoundToVirtualRightStick))
				{
					this._trackballFriction = 1;
				}
				return this._trackballFriction;
			}
			set
			{
				if (this.SetProperty<ushort>(ref this._trackballFriction, value, "TrackballFriction"))
				{
					base.IsChanged = true;
				}
			}
		}

		public abstract ushort DefaultXLow { get; }

		public abstract ushort DefaultYLow { get; }

		public abstract ushort DefaultXMed { get; }

		public abstract ushort DefaultYMed { get; }

		public abstract ushort DefaultXHigh { get; }

		public abstract ushort DefaultYHigh { get; }

		public virtual bool DefaultSpringMode
		{
			get
			{
				return true;
			}
		}

		public virtual ushort DefaultScaleFactor
		{
			get
			{
				return 1500;
			}
		}

		protected BaseDirectionalAnalogGroup(BaseXBBindingCollection baseHostCollection)
			: base(baseHostCollection)
		{
		}

		protected override void ResetActivatorXBBindingDictionaries()
		{
			base.ResetActivatorXBBindingDictionaries();
			if (this.UpLeft != 2000)
			{
				this.UpLeftValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.UpLeft);
			}
			if (this.UpRight != 2000)
			{
				this.UpRightValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.UpRight);
			}
			if (this.DownLeft != 2000)
			{
				this.DownLeftValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.DownLeft);
			}
			if (this.DownRight != 2000)
			{
				this.DownRightValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.DownRight);
			}
			if (this.IsZoneLowAllowed && this.LowZoneValue != null)
			{
				this.LowZoneValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.LowZone);
			}
			if (this.IsZoneMedAllowed && this.MedZoneValue != null)
			{
				this.MedZoneValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.MedZone);
			}
			if (this.IsZoneHighAllowed && this.HighZoneValue != null)
			{
				this.HighZoneValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.HighZone);
			}
			if (this.IsZonesDirectionAllowed)
			{
				if (this.IsZoneHighAllowed)
				{
					if (this.HighZoneLeftValue != null)
					{
						this.HighZoneLeftValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.HighZoneLeft);
					}
					if (this.HighZoneRightValue != null)
					{
						this.HighZoneRightValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.HighZoneRight);
					}
					if (this.HighZoneUpValue != null)
					{
						this.HighZoneUpValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.HighZoneUp);
					}
					if (this.HighZoneDownValue != null)
					{
						this.HighZoneDownValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.HighZoneDown);
					}
					if (this.IsZoneLeanAllowed)
					{
						if (this.HighZoneLeanLeftValue != null)
						{
							this.HighZoneLeanLeftValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.HighZoneLeanLeft);
						}
						if (this.HighZoneLeanRightValue != null)
						{
							this.HighZoneLeanRightValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.HighZoneLeanRight);
						}
					}
				}
				if (this.IsZoneMedAllowed)
				{
					if (this.MedZoneLeftValue != null)
					{
						this.MedZoneLeftValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.MedZoneLeft);
					}
					if (this.MedZoneRightValue != null)
					{
						this.MedZoneRightValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.MedZoneRight);
					}
					if (this.MedZoneUpValue != null)
					{
						this.MedZoneUpValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.MedZoneUp);
					}
					if (this.MedZoneDownValue != null)
					{
						this.MedZoneDownValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.MedZoneDown);
					}
					if (this.IsZoneLeanAllowed)
					{
						if (this.MedZoneLeanLeftValue != null)
						{
							this.MedZoneLeanLeftValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.MedZoneLeanLeft);
						}
						if (this.MedZoneLeanRightValue != null)
						{
							this.MedZoneLeanRightValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.MedZoneLeanRight);
						}
					}
				}
				if (this.IsZoneLowAllowed)
				{
					if (this.LowZoneLeftValue != null)
					{
						this.LowZoneLeftValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.LowZoneLeft);
					}
					if (this.LowZoneRightValue != null)
					{
						this.LowZoneRightValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.LowZoneRight);
					}
					if (this.LowZoneUpValue != null)
					{
						this.LowZoneUpValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.LowZoneUp);
					}
					if (this.LowZoneDownValue != null)
					{
						this.LowZoneDownValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.LowZoneDown);
					}
					if (this.IsZoneLeanAllowed)
					{
						if (this.LowZoneLeanLeftValue != null)
						{
							this.LowZoneLeanLeftValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.LowZoneLeanLeft);
						}
						if (this.LowZoneLeanRightValue != null)
						{
							this.LowZoneLeanRightValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.LowZoneLeanRight);
						}
					}
				}
			}
		}

		protected virtual ObservableCollection<ThumbstickSensitivity> CreateSensitivities()
		{
			return new ObservableCollection<ThumbstickSensitivity>
			{
				this.CreateDefaultSensitivity(),
				this.CreateDelaySensitivity(),
				this.CreateAggressiveSensitivity(),
				this.CreateInstantSensitivity(),
				this.CreateSmoothSensitivity(),
				this.CreateCustomSensitivity()
			};
		}

		protected virtual ThumbstickSensitivity CreateDefaultSensitivity()
		{
			return new ThumbstickSensitivity("Default", new Localizable(11621), 8190, 8195, 24575, 24575, false);
		}

		protected virtual ThumbstickSensitivity CreateDelaySensitivity()
		{
			return new ThumbstickSensitivity("Delay", new Localizable(11625), 8191, 8195, 28000, 19000, false);
		}

		protected virtual ThumbstickSensitivity CreateAggressiveSensitivity()
		{
			return new ThumbstickSensitivity("Aggressive", new Localizable(11622), 8192, 8195, 18000, 26000, false);
		}

		protected virtual ThumbstickSensitivity CreateInstantSensitivity()
		{
			return new ThumbstickSensitivity("Instant", new Localizable(11624), 8193, 8195, 8200, 13435, false);
		}

		protected virtual ThumbstickSensitivity CreateSmoothSensitivity()
		{
			return new ThumbstickSensitivity("Smooth", new Localizable(11623), 17814, 17815, 27852, 20000, false);
		}

		protected virtual ThumbstickSensitivity CreateCustomSensitivity()
		{
			return new ThumbstickSensitivity("Custom", new Localizable(11620), 6400, 6400, 12800, 12800, 19200, 19200, 25600, 25600, true);
		}

		public virtual bool InvertControlsEnabled
		{
			get
			{
				return this.StickResponseControlsEnabled;
			}
		}

		public virtual string InvertControlsDisabledExplanation
		{
			get
			{
				return null;
			}
		}

		public virtual bool StickResponseControlsEnabled
		{
			get
			{
				return !base.IsUnmapped || base.IsBoundToAnyInvertedMouse || base.IsBoundToVirtualLeftStick || base.IsBoundToVirtualRightStick;
			}
		}

		public virtual bool StickResponseControlsIsVisible
		{
			get
			{
				return !this.IsDiagonalDirections;
			}
		}

		public virtual string StickResponseDisabledExplanation
		{
			get
			{
				return null;
			}
		}

		protected override void SetDefaultValues(bool silent = false)
		{
			base.SetDefaultValues(silent);
			this.SensitivitiesCollection = this.CreateSensitivities();
			if (silent)
			{
				ObservableCollection<ThumbstickSensitivity> sensitivitiesCollection = this.SensitivitiesCollection;
				this._sensitivitySensitivity = ((sensitivitiesCollection != null) ? sensitivitiesCollection.FirstOrDefault<ThumbstickSensitivity>() : null);
				ThumbstickSensitivity sensitivitySensitivity = this._sensitivitySensitivity;
				this._sensitivity = ((sensitivitySensitivity != null) ? sensitivitySensitivity.Deflection : null);
				this._isEllipticZoning = false;
				this._isRadialZoning = true;
				this._isHardwareDeadzone = false;
				this._mouseSensitivity = 4;
				this._mouseSensitivityY = 4;
				this._isUseGlobalMouseSensitivity = true;
				this._flickStickSensitivity = 500;
				this._flickStickThreshold = 20000;
				this._flickStickDuration = 100U;
				this._mouseSmoothing = (this.IsGyroTilt ? 10 : 32);
				this._mouseFlickSmoothing = ((this.IsTrackPad1 || this.IsTrackPad2) ? 24 : 10);
				this._isDiagonalDirections = false;
				this._xLow = this.DefaultXLow;
				this._xMed = this.DefaultXMed;
				this._xHigh = this.DefaultXHigh;
				this._yLow = this.DefaultYLow;
				this._yMed = this.DefaultYMed;
				this._yHigh = this.DefaultYHigh;
				this._springMode = true;
				this._returnTime = 30;
				this._smoothing = 3U;
				this._noiseFilter = 3U;
				this._scaleFactorX = 1500U;
				this._scaleFactorY = 1500U;
				this._rotation = 0;
				this._trackballFriction = 50;
				return;
			}
			ObservableCollection<ThumbstickSensitivity> sensitivitiesCollection2 = this.SensitivitiesCollection;
			this.SensitivitySensitivity = ((sensitivitiesCollection2 != null) ? sensitivitiesCollection2.FirstOrDefault<ThumbstickSensitivity>() : null);
			this.IsEllipticZoning = false;
			this.IsRadialZoning = true;
			this.IsHardwareDeadzone = false;
			this.MouseSensitivity = 4;
			this.MouseSensitivityY = 4;
			this.IsUseGlobalMouseSensitivity = true;
			this.FlickStickSensitivity = 500;
			this.FlickStickThreshold = 20000;
			this.FlickStickDuration = 100U;
			this.MouseSmoothing = (this.IsGyroTilt ? 10 : 32);
			this.MouseFlickSmoothing = ((this.IsTrackPad1 || this.IsTrackPad2) ? 24 : 10);
			this.IsDiagonalDirections = false;
			this.XLow = this.DefaultXLow;
			this.XMed = this.DefaultXMed;
			this.XHigh = this.DefaultXHigh;
			this.YLow = this.DefaultYLow;
			this.YMed = this.DefaultYMed;
			this.YHigh = this.DefaultYHigh;
			this.SpringMode = true;
			this.ReturnTime = 30;
			this.Smoothing = 3U;
			this.NoiseFilter = 3U;
			this.ScaleFactorX = 1500U;
			this.ScaleFactorY = 1500U;
			this.Rotation = 0;
			this.TrackballFriction = 50;
		}

		public override bool IsAdvancedDefault()
		{
			bool flag = base.IsAdvancedDefault();
			ThumbstickSensitivity sensitivitySensitivity = this.SensitivitySensitivity;
			ObservableCollection<ThumbstickSensitivity> sensitivitiesCollection = this.SensitivitiesCollection;
			return flag & (sensitivitySensitivity == ((sensitivitiesCollection != null) ? sensitivitiesCollection.FirstOrDefault<ThumbstickSensitivity>() : null)) & (this.XSensitivity == 13573) & (this.YSensitivity == 13573) & !this.IsHardwareDeadzone & (this.SpringMode == this.DefaultSpringMode) & (this.ReturnTime == 30) & (this.Smoothing == 3U) & (this.NoiseFilter == 3U) & (this.ScaleFactorX == (uint)this.DefaultScaleFactor) & (this.ScaleFactorY == (uint)this.DefaultScaleFactor) & (this.Rotation == 0) & !this.IsXInvert & !this.IsYInvert & this.CheckXYZoneDefault() & this.CheckAdvancedBindigsDefault();
		}

		protected virtual bool CheckXYZoneDefault()
		{
			int num = (int)((this.IsSpringModeAllowed && this.SpringMode) ? 0 : this.DefaultXLow);
			int num2 = ((!this.IsZonesShapeChangeAllowed) ? num : ((int)this.DefaultYLow));
			return ((int)this.XLow == num) & (this.XMed == this.DefaultXMed) & (this.XHigh == this.DefaultXHigh) & ((int)this.YLow == num2) & (this.YMed == this.DefaultYMed) & (this.YHigh == this.DefaultYHigh);
		}

		private bool CheckAdvancedBindigsDefault()
		{
			bool flag = true;
			if (this.UpLeft != 2000)
			{
				flag &= this.UpLeftValue.IsEmpty;
			}
			if (this.UpRight != 2000)
			{
				flag &= this.UpRightValue.IsEmpty;
			}
			if (this.DownLeft != 2000)
			{
				flag &= this.DownLeftValue.IsEmpty;
			}
			if (this.DownRight != 2000)
			{
				flag &= this.DownRightValue.IsEmpty;
			}
			if (this.LowZoneValue != null)
			{
				flag &= this.LowZoneValue.IsEmpty;
			}
			if (this.MedZoneValue != null)
			{
				flag &= this.MedZoneValue.IsEmpty;
			}
			if (this.HighZoneValue != null)
			{
				flag &= this.HighZoneValue.IsEmpty;
			}
			if (this.LowZoneUpValue != null)
			{
				flag &= this.LowZoneUpValue.IsEmpty;
			}
			if (this.LowZoneDownValue != null)
			{
				flag &= this.LowZoneDownValue.IsEmpty;
			}
			if (this.LowZoneLeftValue != null)
			{
				flag &= this.LowZoneLeftValue.IsEmpty;
			}
			if (this.LowZoneRightValue != null)
			{
				flag &= this.LowZoneRightValue.IsEmpty;
			}
			if (this.LowZoneLeanLeftValue != null)
			{
				flag &= this.LowZoneLeanLeftValue.IsEmpty;
			}
			if (this.LowZoneLeanRightValue != null)
			{
				flag &= this.LowZoneLeanRightValue.IsEmpty;
			}
			if (this.MedZoneUpValue != null)
			{
				flag &= this.MedZoneUpValue.IsEmpty;
			}
			if (this.MedZoneDownValue != null)
			{
				flag &= this.MedZoneDownValue.IsEmpty;
			}
			if (this.MedZoneLeftValue != null)
			{
				flag &= this.MedZoneLeftValue.IsEmpty;
			}
			if (this.MedZoneRightValue != null)
			{
				flag &= this.MedZoneRightValue.IsEmpty;
			}
			if (this.MedZoneLeanLeftValue != null)
			{
				flag &= this.MedZoneLeanLeftValue.IsEmpty;
			}
			if (this.MedZoneLeanRightValue != null)
			{
				flag &= this.MedZoneLeanRightValue.IsEmpty;
			}
			if (this.HighZoneUpValue != null)
			{
				flag &= this.HighZoneUpValue.IsEmpty;
			}
			if (this.HighZoneDownValue != null)
			{
				flag &= this.HighZoneDownValue.IsEmpty;
			}
			if (this.HighZoneLeftValue != null)
			{
				flag &= this.HighZoneLeftValue.IsEmpty;
			}
			if (this.HighZoneRightValue != null)
			{
				flag &= this.HighZoneRightValue.IsEmpty;
			}
			if (this.HighZoneLeanLeftValue != null)
			{
				flag &= this.HighZoneLeanLeftValue.IsEmpty;
			}
			if (this.HighZoneLeanRightValue != null)
			{
				flag &= this.HighZoneLeanRightValue.IsEmpty;
			}
			return flag;
		}

		public override bool ResetToDefault(bool askConfirmation = true)
		{
			bool flag = base.ResetToDefault(askConfirmation);
			if (flag)
			{
				this.OnPropertyChanged("InvertControlsEnabled");
				this.OnPropertyChanged("StickResponseControlsEnabled");
				this.OnPropertyChanged("IsXInvert");
				this.OnPropertyChanged("IsYInvert");
			}
			return flag;
		}

		public override void ToggleUnmap()
		{
			base.ToggleUnmap();
			this.OnPropertyChanged("InvertControlsEnabled");
			this.OnPropertyChanged("StickResponseControlsEnabled");
			this.OnPropertyChanged("IsXInvert");
			this.OnPropertyChanged("IsYInvert");
		}

		public override void RevertRemapToDefault()
		{
			base.RevertRemapToDefault();
			if (this.UpLeft != 2000)
			{
				this.UpLeftValue.RevertRemap();
			}
			if (this.UpLeft != 2000)
			{
				this.UpRightValue.RevertRemap();
			}
			if (this.UpLeft != 2000)
			{
				this.DownLeftValue.RevertRemap();
			}
			if (this.UpLeft != 2000)
			{
				this.DownRightValue.RevertRemap();
			}
		}

		public override void UpdateProperties()
		{
			base.UpdateProperties();
			this.OnPropertyChanged("ZonesExplanation");
			this.OnPropertyChanged("StickResponseControlsIsVisible");
			this.OnPropertyChanged("InvertControlsEnabled");
			this.OnPropertyChanged("StickResponseControlsEnabled");
			this.OnPropertyChanged("IsDiagonalDirections");
			this.OnPropertyChanged("IsDigitalMode");
			this.OnPropertyChanged("IsXInvert");
			this.OnPropertyChanged("IsYInvert");
			this.OnPropertyChanged("FlickStickIsInverted");
		}

		public bool IsAnyZoneVirtualMappingPresent
		{
			get
			{
				XBBinding lowZoneValue = this.LowZoneValue;
				if (lowZoneValue == null || !lowZoneValue.IsAnyActivatorVirtualMappingPresent)
				{
					XBBinding medZoneValue = this.MedZoneValue;
					if (medZoneValue == null || !medZoneValue.IsAnyActivatorVirtualMappingPresent)
					{
						XBBinding highZoneValue = this.HighZoneValue;
						if (highZoneValue == null || !highZoneValue.IsAnyActivatorVirtualMappingPresent)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		public bool IsAnyZoneAllowedVirtualMappingPresent
		{
			get
			{
				if (this.IsZoneLowAllowed)
				{
					XBBinding lowZoneValue = this.LowZoneValue;
					if (lowZoneValue != null && lowZoneValue.IsAnyActivatorVirtualMappingPresent)
					{
						return true;
					}
				}
				if (this.IsZoneMedAllowed)
				{
					XBBinding medZoneValue = this.MedZoneValue;
					if (medZoneValue != null && medZoneValue.IsAnyActivatorVirtualMappingPresent)
					{
						return true;
					}
				}
				if (this.IsZoneHighAllowed)
				{
					XBBinding highZoneValue = this.HighZoneValue;
					if (highZoneValue != null && highZoneValue.IsAnyActivatorVirtualMappingPresent)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool IsAnyZoneDirectionalVirtualMappingPresent
		{
			get
			{
				if (!this.IsZonesDirectionAllowed)
				{
					return false;
				}
				if (this.IsZoneLowAllowed)
				{
					XBBinding lowZoneUpValue = this.LowZoneUpValue;
					if (lowZoneUpValue == null || !lowZoneUpValue.IsAnyActivatorVirtualMappingPresent)
					{
						XBBinding lowZoneDownValue = this.LowZoneDownValue;
						if (lowZoneDownValue == null || !lowZoneDownValue.IsAnyActivatorVirtualMappingPresent)
						{
							XBBinding lowZoneLeftValue = this.LowZoneLeftValue;
							if (lowZoneLeftValue == null || !lowZoneLeftValue.IsAnyActivatorVirtualMappingPresent)
							{
								XBBinding lowZoneRightValue = this.LowZoneRightValue;
								if (lowZoneRightValue == null || !lowZoneRightValue.IsAnyActivatorVirtualMappingPresent)
								{
									if (this.IsZoneLeanAllowed)
									{
										XBBinding lowZoneLeanLeftValue = this.LowZoneLeanLeftValue;
										if (lowZoneLeanLeftValue == null || !lowZoneLeanLeftValue.IsAnyActivatorVirtualMappingPresent)
										{
											XBBinding lowZoneLeanRightValue = this.LowZoneLeanRightValue;
											if (lowZoneLeanRightValue == null || !lowZoneLeanRightValue.IsAnyActivatorVirtualMappingPresent)
											{
												goto IL_99;
											}
										}
										return true;
									}
									goto IL_99;
								}
							}
						}
					}
					return true;
				}
				IL_99:
				if (this.IsZoneMedAllowed)
				{
					XBBinding medZoneUpValue = this.MedZoneUpValue;
					if (medZoneUpValue == null || !medZoneUpValue.IsAnyActivatorVirtualMappingPresent)
					{
						XBBinding medZoneDownValue = this.MedZoneDownValue;
						if (medZoneDownValue == null || !medZoneDownValue.IsAnyActivatorVirtualMappingPresent)
						{
							XBBinding medZoneLeftValue = this.MedZoneLeftValue;
							if (medZoneLeftValue == null || !medZoneLeftValue.IsAnyActivatorVirtualMappingPresent)
							{
								XBBinding medZoneRightValue = this.MedZoneRightValue;
								if (medZoneRightValue == null || !medZoneRightValue.IsAnyActivatorVirtualMappingPresent)
								{
									if (this.IsZoneLeanAllowed)
									{
										XBBinding medZoneLeanLeftValue = this.MedZoneLeanLeftValue;
										if (medZoneLeanLeftValue == null || !medZoneLeanLeftValue.IsAnyActivatorVirtualMappingPresent)
										{
											XBBinding medZoneLeanRightValue = this.MedZoneLeanRightValue;
											if (medZoneLeanRightValue == null || !medZoneLeanRightValue.IsAnyActivatorVirtualMappingPresent)
											{
												goto IL_128;
											}
										}
										return true;
									}
									goto IL_128;
								}
							}
						}
					}
					return true;
				}
				IL_128:
				if (this.IsZoneHighAllowed)
				{
					XBBinding highZoneUpValue = this.HighZoneUpValue;
					if (highZoneUpValue == null || !highZoneUpValue.IsAnyActivatorVirtualMappingPresent)
					{
						XBBinding highZoneDownValue = this.HighZoneDownValue;
						if (highZoneDownValue == null || !highZoneDownValue.IsAnyActivatorVirtualMappingPresent)
						{
							XBBinding highZoneLeftValue = this.HighZoneLeftValue;
							if (highZoneLeftValue == null || !highZoneLeftValue.IsAnyActivatorVirtualMappingPresent)
							{
								XBBinding highZoneRightValue = this.HighZoneRightValue;
								if (highZoneRightValue == null || !highZoneRightValue.IsAnyActivatorVirtualMappingPresent)
								{
									if (this.IsZoneLeanAllowed)
									{
										XBBinding highZoneLeanLeftValue = this.HighZoneLeanLeftValue;
										if (highZoneLeanLeftValue == null || !highZoneLeanLeftValue.IsAnyActivatorVirtualMappingPresent)
										{
											XBBinding highZoneLeanRightValue = this.HighZoneLeanRightValue;
											if (highZoneLeanRightValue == null || !highZoneLeanRightValue.IsAnyActivatorVirtualMappingPresent)
											{
												return false;
											}
										}
										return true;
									}
									return false;
								}
							}
						}
					}
					return true;
				}
				return false;
			}
		}

		public List<GamepadButton> GetAllowedZoneButtons()
		{
			List<GamepadButton> list = new List<GamepadButton>();
			if (this.IsZoneLowAllowed)
			{
				if (GamepadButtonExtensions.IsRealButton(this.LowZone))
				{
					list.Add(this.LowZone);
				}
				if (this.IsZonesDirectionAllowed)
				{
					if (GamepadButtonExtensions.IsRealButton(this.LowZoneUp))
					{
						list.Add(this.LowZoneUp);
					}
					if (GamepadButtonExtensions.IsRealButton(this.LowZoneDown))
					{
						list.Add(this.LowZoneDown);
					}
					if (GamepadButtonExtensions.IsRealButton(this.LowZoneLeft))
					{
						list.Add(this.LowZoneLeft);
					}
					if (GamepadButtonExtensions.IsRealButton(this.LowZoneRight))
					{
						list.Add(this.LowZoneRight);
					}
					if (this.IsZoneLeanAllowed)
					{
						if (GamepadButtonExtensions.IsRealButton(this.LowZoneLeanLeft))
						{
							list.Add(this.LowZoneLeanLeft);
						}
						if (GamepadButtonExtensions.IsRealButton(this.LowZoneLeanRight))
						{
							list.Add(this.LowZoneLeanRight);
						}
					}
				}
			}
			if (this.IsZoneMedAllowed)
			{
				if (GamepadButtonExtensions.IsRealButton(this.MedZone))
				{
					list.Add(this.MedZone);
				}
				if (this.IsZonesDirectionAllowed)
				{
					if (GamepadButtonExtensions.IsRealButton(this.MedZoneUp))
					{
						list.Add(this.MedZoneUp);
					}
					if (GamepadButtonExtensions.IsRealButton(this.MedZoneDown))
					{
						list.Add(this.MedZoneDown);
					}
					if (GamepadButtonExtensions.IsRealButton(this.MedZoneLeft))
					{
						list.Add(this.MedZoneLeft);
					}
					if (GamepadButtonExtensions.IsRealButton(this.MedZoneRight))
					{
						list.Add(this.MedZoneRight);
					}
					if (this.IsZoneLeanAllowed)
					{
						if (GamepadButtonExtensions.IsRealButton(this.MedZoneLeanLeft))
						{
							list.Add(this.MedZoneLeanLeft);
						}
						if (GamepadButtonExtensions.IsRealButton(this.MedZoneLeanRight))
						{
							list.Add(this.MedZoneLeanRight);
						}
					}
				}
			}
			if (this.IsZoneHighAllowed)
			{
				if (GamepadButtonExtensions.IsRealButton(this.HighZone))
				{
					list.Add(this.HighZone);
				}
				if (this.IsZonesDirectionAllowed)
				{
					if (GamepadButtonExtensions.IsRealButton(this.HighZoneUp))
					{
						list.Add(this.HighZoneUp);
					}
					if (GamepadButtonExtensions.IsRealButton(this.HighZoneDown))
					{
						list.Add(this.HighZoneDown);
					}
					if (GamepadButtonExtensions.IsRealButton(this.HighZoneLeft))
					{
						list.Add(this.HighZoneLeft);
					}
					if (GamepadButtonExtensions.IsRealButton(this.HighZoneRight))
					{
						list.Add(this.HighZoneRight);
					}
					if (this.IsZoneLeanAllowed)
					{
						if (GamepadButtonExtensions.IsRealButton(this.HighZoneLeanLeft))
						{
							list.Add(this.HighZoneLeanLeft);
						}
						if (GamepadButtonExtensions.IsRealButton(this.HighZoneLeanRight))
						{
							list.Add(this.HighZoneLeanRight);
						}
					}
				}
			}
			return list;
		}

		public bool IsAnyDirectionalZoneHasMapping
		{
			get
			{
				XBBinding lowZoneLeftValue = this.LowZoneLeftValue;
				if (lowZoneLeftValue == null || !lowZoneLeftValue.IsAnyActivatorVirtualMappingPresent)
				{
					XBBinding lowZoneUpValue = this.LowZoneUpValue;
					if (lowZoneUpValue == null || !lowZoneUpValue.IsAnyActivatorVirtualMappingPresent)
					{
						XBBinding lowZoneRightValue = this.LowZoneRightValue;
						if (lowZoneRightValue == null || !lowZoneRightValue.IsAnyActivatorVirtualMappingPresent)
						{
							XBBinding lowZoneDownValue = this.LowZoneDownValue;
							if (lowZoneDownValue == null || !lowZoneDownValue.IsAnyActivatorVirtualMappingPresent)
							{
								XBBinding medZoneLeftValue = this.MedZoneLeftValue;
								if (medZoneLeftValue == null || !medZoneLeftValue.IsAnyActivatorVirtualMappingPresent)
								{
									XBBinding medZoneUpValue = this.MedZoneUpValue;
									if (medZoneUpValue == null || !medZoneUpValue.IsAnyActivatorVirtualMappingPresent)
									{
										XBBinding medZoneRightValue = this.MedZoneRightValue;
										if (medZoneRightValue == null || !medZoneRightValue.IsAnyActivatorVirtualMappingPresent)
										{
											XBBinding medZoneDownValue = this.MedZoneDownValue;
											if (medZoneDownValue == null || !medZoneDownValue.IsAnyActivatorVirtualMappingPresent)
											{
												XBBinding highZoneLeftValue = this.HighZoneLeftValue;
												if (highZoneLeftValue == null || !highZoneLeftValue.IsAnyActivatorVirtualMappingPresent)
												{
													XBBinding highZoneUpValue = this.HighZoneUpValue;
													if (highZoneUpValue == null || !highZoneUpValue.IsAnyActivatorVirtualMappingPresent)
													{
														XBBinding highZoneRightValue = this.HighZoneRightValue;
														if (highZoneRightValue == null || !highZoneRightValue.IsAnyActivatorVirtualMappingPresent)
														{
															XBBinding highZoneDownValue = this.HighZoneDownValue;
															if (highZoneDownValue == null || !highZoneDownValue.IsAnyActivatorVirtualMappingPresent)
															{
																if (this.IsZoneLeanAllowed)
																{
																	XBBinding lowZoneLeanLeftValue = this.LowZoneLeanLeftValue;
																	if (lowZoneLeanLeftValue == null || !lowZoneLeanLeftValue.IsAnyActivatorVirtualMappingPresent)
																	{
																		XBBinding lowZoneLeanRightValue = this.LowZoneLeanRightValue;
																		if (lowZoneLeanRightValue == null || !lowZoneLeanRightValue.IsAnyActivatorVirtualMappingPresent)
																		{
																			XBBinding medZoneLeanLeftValue = this.MedZoneLeanLeftValue;
																			if (medZoneLeanLeftValue == null || !medZoneLeanLeftValue.IsAnyActivatorVirtualMappingPresent)
																			{
																				XBBinding medZoneLeanRightValue = this.MedZoneLeanRightValue;
																				if (medZoneLeanRightValue == null || !medZoneLeanRightValue.IsAnyActivatorVirtualMappingPresent)
																				{
																					XBBinding highZoneLeanLeftValue = this.HighZoneLeanLeftValue;
																					if (highZoneLeanLeftValue == null || !highZoneLeanLeftValue.IsAnyActivatorVirtualMappingPresent)
																					{
																						XBBinding highZoneLeanRightValue = this.HighZoneLeanRightValue;
																						if (highZoneLeanRightValue == null || !highZoneLeanRightValue.IsAnyActivatorVirtualMappingPresent)
																						{
																							return false;
																						}
																					}
																				}
																			}
																		}
																	}
																	return true;
																}
																return false;
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				return true;
			}
		}

		public override object GetValueByKey(object key)
		{
			object valueByKey = base.GetValueByKey(key);
			if (valueByKey != null)
			{
				return valueByKey;
			}
			if (key is GamepadButton)
			{
				GamepadButton gamepadButton = (GamepadButton)key;
				if (gamepadButton == this.LowZone)
				{
					return this.LowZoneValue;
				}
				if (gamepadButton == this.LowZoneLeft)
				{
					return this.LowZoneLeftValue;
				}
				if (gamepadButton == this.LowZoneUp)
				{
					return this.LowZoneUpValue;
				}
				if (gamepadButton == this.LowZoneRight)
				{
					return this.LowZoneRightValue;
				}
				if (gamepadButton == this.LowZoneDown)
				{
					return this.LowZoneDownValue;
				}
				if (this.IsZoneLeanAllowed)
				{
					if (gamepadButton == this.LowZoneLeanLeft)
					{
						return this.LowZoneLeanLeftValue;
					}
					if (gamepadButton == this.LowZoneLeanRight)
					{
						return this.LowZoneLeanRightValue;
					}
				}
				if (gamepadButton == this.MedZone)
				{
					return this.MedZoneValue;
				}
				if (gamepadButton == this.MedZoneLeft)
				{
					return this.MedZoneLeftValue;
				}
				if (gamepadButton == this.MedZoneUp)
				{
					return this.MedZoneUpValue;
				}
				if (gamepadButton == this.MedZoneRight)
				{
					return this.MedZoneRightValue;
				}
				if (gamepadButton == this.MedZoneDown)
				{
					return this.MedZoneDownValue;
				}
				if (this.IsZoneLeanAllowed)
				{
					if (gamepadButton == this.MedZoneLeanLeft)
					{
						return this.MedZoneLeanLeftValue;
					}
					if (gamepadButton == this.MedZoneLeanRight)
					{
						return this.MedZoneLeanRightValue;
					}
				}
				if (gamepadButton == this.HighZone)
				{
					return this.HighZoneValue;
				}
				if (gamepadButton == this.HighZoneLeft)
				{
					return this.HighZoneLeftValue;
				}
				if (gamepadButton == this.HighZoneUp)
				{
					return this.HighZoneUpValue;
				}
				if (gamepadButton == this.HighZoneRight)
				{
					return this.HighZoneRightValue;
				}
				if (gamepadButton == this.HighZoneDown)
				{
					return this.HighZoneDownValue;
				}
				if (this.IsZoneLeanAllowed)
				{
					if (gamepadButton == this.HighZoneLeanLeft)
					{
						return this.HighZoneLeanLeftValue;
					}
					if (gamepadButton == this.HighZoneLeanRight)
					{
						return this.HighZoneLeanRightValue;
					}
				}
			}
			return valueByKey;
		}

		public XBBinding GetXBBindingByZoneAndDirection(Zone zone, Direction direction)
		{
			if (zone == null)
			{
				if (direction == null)
				{
					return this.LowZoneValue;
				}
				if (direction == 1)
				{
					return this.LowZoneLeftValue;
				}
				if (direction == 3)
				{
					return this.LowZoneUpValue;
				}
				if (direction == 2)
				{
					return this.LowZoneRightValue;
				}
				if (direction == 4)
				{
					return this.LowZoneDownValue;
				}
				if (this.IsZoneLeanAllowed)
				{
					if (direction == 5)
					{
						return this.LowZoneLeanLeftValue;
					}
					if (direction == 6)
					{
						return this.LowZoneLeanRightValue;
					}
				}
			}
			else if (zone == 1)
			{
				if (direction == null)
				{
					return this.MedZoneValue;
				}
				if (direction == 1)
				{
					return this.MedZoneLeftValue;
				}
				if (direction == 3)
				{
					return this.MedZoneUpValue;
				}
				if (direction == 2)
				{
					return this.MedZoneRightValue;
				}
				if (direction == 4)
				{
					return this.MedZoneDownValue;
				}
				if (this.IsZoneLeanAllowed)
				{
					if (direction == 5)
					{
						return this.MedZoneLeanLeftValue;
					}
					if (direction == 6)
					{
						return this.MedZoneLeanRightValue;
					}
				}
			}
			else if (zone == 2)
			{
				if (direction == null)
				{
					return this.HighZoneValue;
				}
				if (direction == 1)
				{
					return this.HighZoneLeftValue;
				}
				if (direction == 3)
				{
					return this.HighZoneUpValue;
				}
				if (direction == 2)
				{
					return this.HighZoneRightValue;
				}
				if (direction == 4)
				{
					return this.HighZoneDownValue;
				}
				if (this.IsZoneLeanAllowed)
				{
					if (direction == 5)
					{
						return this.HighZoneLeanLeftValue;
					}
					if (direction == 6)
					{
						return this.HighZoneLeanRightValue;
					}
				}
			}
			return null;
		}

		public DelegateCommand CopyResponseCommand
		{
			get
			{
				if (this._copyResponseCommand == null)
				{
					this._copyResponseCommand = new DelegateCommand(new Action(this.CopyResponseCommandExecute), new Func<bool>(this.CopyResponseCommandCanExecute));
				}
				return this._copyResponseCommand;
			}
		}

		private void CopyResponseCommandExecute()
		{
			BaseDirectionalAnalogGroup._copySensitivity = this.SensitivitySensitivity.Clone();
			this.PasteResponseCommand.RaiseCanExecuteChanged();
		}

		private bool CopyResponseCommandCanExecute()
		{
			return this.SensitivitySensitivity.IsCustom;
		}

		public DelegateCommand PasteResponseCommand
		{
			get
			{
				if (this._pasteResponseCommand == null)
				{
					this._pasteResponseCommand = new DelegateCommand(new Action(this.PasteResponseCommandExecute), new Func<bool>(this.PasteResponseCanExecute));
				}
				return this._pasteResponseCommand;
			}
		}

		private void PasteResponseCommandExecute()
		{
			this.Sensitivity = BaseDirectionalAnalogGroup._copySensitivity.Deflection.Clone();
		}

		private bool PasteResponseCanExecute()
		{
			return BaseDirectionalAnalogGroup._copySensitivity != null && this.SensitivitySensitivity.IsCustom;
		}

		public DelegateCommand ClearResponseCommand
		{
			get
			{
				if (this._clearResponseCommand == null)
				{
					this._clearResponseCommand = new DelegateCommand(new Action(this.ClearResponseCommandExecute), new Func<bool>(this.ClearResponseCommandCanExecute));
				}
				return this._clearResponseCommand;
			}
		}

		private void ClearResponseCommandExecute()
		{
			this.SensitivitySensitivity = this.CreateCustomSensitivity();
		}

		private bool ClearResponseCommandCanExecute()
		{
			return this.SensitivitySensitivity.IsCustom;
		}

		public void CopyFrom(BaseDirectionalAnalogGroup model)
		{
			this._xLow = model.XLow;
			this._xMed = model.XMed;
			this._xHigh = model.XHigh;
			this._yLow = model.YLow;
			this._yMed = model.YMed;
			this._yHigh = model.YHigh;
			this._isRadialZoning = model.IsRadialZoning;
			this._isEllipticZoning = model.IsEllipticZoning;
			this._isDiagonalDirections = model.IsDiagonalDirections;
			this._xSensitivity = model.XSensitivity;
			this._ySensitivity = model.YSensitivity;
			this._rotation = model.Rotation;
			this._isUseGlobalMouseSensitivity = model.IsUseGlobalMouseSensitivity;
			this._mouseSensitivity = model.MouseSensitivity;
			this._mouseSensitivityY = model.MouseSensitivityY;
			this._mouseSmoothing = model.MouseSmoothing;
			this._springMode = model.SpringMode;
			this._returnTime = model.ReturnTime;
			this._smoothing = model.Smoothing;
			this._noiseFilter = model.NoiseFilter;
			this._scaleFactorX = model.ScaleFactorX;
			this._scaleFactorY = model.ScaleFactorY;
			this._flickStickSensitivity = model.FlickStickSensitivity;
			this._flickStickThreshold = model.FlickStickThreshold;
			this._flickStickDuration = model.FlickStickDuration;
			this.SensitivitiesCollection = new ObservableCollection<ThumbstickSensitivity>(model.SensitivitiesCollection);
			this.SensitivitySensitivity = model.SensitivitySensitivity;
			this._isHardwareDeadzone = model.IsHardwareDeadzone;
			this._isTrackballFriction = model.IsTrackballFriction;
			this._trackballFriction = model.TrackballFriction;
		}

		public void CopyFromForShiftCollection(BaseDirectionalAnalogGroup model)
		{
			this._isDiagonalDirections = model.IsDiagonalDirections;
			this._xSensitivity = model.XSensitivity;
			this._ySensitivity = model.YSensitivity;
			this._rotation = model.Rotation;
			this._isUseGlobalMouseSensitivity = model.IsUseGlobalMouseSensitivity;
			this._mouseSensitivity = model.MouseSensitivity;
			this._mouseSensitivityY = model.MouseSensitivityY;
			this._mouseSmoothing = model.MouseSmoothing;
			this._springMode = model.SpringMode;
			this._returnTime = model.ReturnTime;
			this._smoothing = model.Smoothing;
			this._noiseFilter = model.NoiseFilter;
			this._flickStickSensitivity = model.FlickStickSensitivity;
			this._flickStickThreshold = model.FlickStickThreshold;
			this._flickStickDuration = model.FlickStickDuration;
			this.SensitivitiesCollection = new ObservableCollection<ThumbstickSensitivity>(model.SensitivitiesCollection);
			this.SensitivitySensitivity = model.SensitivitySensitivity;
			this._isHardwareDeadzone = model.IsHardwareDeadzone;
			this._isTrackballFriction = model.IsTrackballFriction;
			this._trackballFriction = model.TrackballFriction;
		}

		public void CopyTo(BaseDirectionalAnalogGroup model)
		{
			model._xLow = this.XLow;
			model._xMed = this.XMed;
			model._xHigh = this.XHigh;
			model._yLow = this.YLow;
			model._yMed = this.YMed;
			model._yHigh = this.YHigh;
			model._isRadialZoning = this.IsRadialZoning;
			model._isEllipticZoning = this.IsEllipticZoning;
			model._isDiagonalDirections = this.IsDiagonalDirections;
			model._xSensitivity = this.XSensitivity;
			model._ySensitivity = this.YSensitivity;
			model._rotation = this.Rotation;
			model._isUseGlobalMouseSensitivity = this.IsUseGlobalMouseSensitivity;
			model._mouseSensitivity = this.MouseSensitivity;
			model._mouseSensitivityY = this.MouseSensitivityY;
			model._mouseSmoothing = this.MouseSmoothing;
			model._springMode = this.SpringMode;
			model._returnTime = this.ReturnTime;
			model._smoothing = this.Smoothing;
			model._noiseFilter = this.NoiseFilter;
			model._scaleFactorX = this.ScaleFactorX;
			model._scaleFactorY = this.ScaleFactorY;
			model._flickStickSensitivity = this.FlickStickSensitivity;
			model._flickStickThreshold = this.FlickStickThreshold;
			model._flickStickDuration = this.FlickStickDuration;
			bool isChanged = base.HostCollection.IsChanged;
			model.SensitivitiesCollection = new ObservableCollection<ThumbstickSensitivity>(this.SensitivitiesCollection);
			model.SensitivitySensitivity = this.SensitivitySensitivity;
			model._isHardwareDeadzone = this.IsHardwareDeadzone;
			base.HostCollection.IsChanged = isChanged;
			model._isTrackballFriction = this.IsTrackballFriction;
			model._trackballFriction = this.TrackballFriction;
		}

		public void CopyFromModel(BaseDirectionalAnalogGroup model)
		{
			this._xLow = model.XLow;
			this._xMed = model.XMed;
			this._xHigh = model.XHigh;
			this._yLow = model.YLow;
			this._yMed = model.YMed;
			this._yHigh = model.YHigh;
			this._isRadialZoning = model.IsRadialZoning;
			this._isEllipticZoning = model.IsEllipticZoning;
			this._isDiagonalDirections = model.IsDiagonalDirections;
			this._xSensitivity = model.XSensitivity;
			this._ySensitivity = model.YSensitivity;
			this._rotation = model.Rotation;
			this._isUseGlobalMouseSensitivity = model.IsUseGlobalMouseSensitivity;
			this._mouseSensitivity = model.MouseSensitivity;
			this._mouseSensitivityY = model.MouseSensitivityY;
			this._mouseSmoothing = model.MouseSmoothing;
			this._mouseFlickSmoothing = model.MouseFlickSmoothing;
			this._springMode = model.SpringMode;
			this._returnTime = model.ReturnTime;
			this._smoothing = model.Smoothing;
			this._noiseFilter = model.NoiseFilter;
			this._scaleFactorX = model.ScaleFactorX;
			this._scaleFactorY = model.ScaleFactorY;
			this._flickStickSensitivity = model.FlickStickSensitivity;
			this._flickStickThreshold = model.FlickStickThreshold;
			this._flickStickDuration = model.FlickStickDuration;
			this.Sensitivity = model.Sensitivity;
			this._isHardwareDeadzone = model.IsHardwareDeadzone;
			this._isTrackballFriction = model.IsTrackballFriction;
			this._trackballFriction = model.TrackballFriction;
		}

		public void CopyToModel(BaseDirectionalAnalogGroup model)
		{
			model.XLow = this._xLow;
			model.XMed = this._xMed;
			model.XHigh = this._xHigh;
			model.YLow = this._yLow;
			model.YMed = this._yMed;
			model.YHigh = this._yHigh;
			model.IsRadialZoning = this._isRadialZoning;
			model.IsEllipticZoning = this._isEllipticZoning;
			model.IsDiagonalDirections = this._isDiagonalDirections;
			model.XSensitivity = this._xSensitivity;
			model.YSensitivity = this._ySensitivity;
			model.Rotation = this._rotation;
			model.IsUseGlobalMouseSensitivity = this._isUseGlobalMouseSensitivity;
			model.MouseSensitivity = this._mouseSensitivity;
			model.MouseSensitivityY = this._mouseSensitivityY;
			model.MouseSmoothing = this._mouseSmoothing;
			model.MouseFlickSmoothing = this._mouseFlickSmoothing;
			model.SpringMode = this._springMode;
			model.ReturnTime = this._returnTime;
			model.Smoothing = this._smoothing;
			model.NoiseFilter = this._noiseFilter;
			model.ScaleFactorX = this._scaleFactorX;
			model.ScaleFactorY = this._scaleFactorY;
			model.FlickStickSensitivity = this.FlickStickSensitivity;
			model.FlickStickThreshold = this.FlickStickThreshold;
			model.FlickStickDuration = this.FlickStickDuration;
			model.SensitivitiesCollection = new ObservableCollection<ThumbstickSensitivity>(this.SensitivitiesCollection);
			model.SensitivitySensitivity = this.SensitivitySensitivity;
			model.IsHardwareDeadzone = this._isHardwareDeadzone;
			model.IsTrackballFriction = this._isTrackballFriction;
			model.TrackballFriction = this._trackballFriction;
		}

		private Direction _currentSelectedDirection;

		private Direction _currentSelectedDiagonalDirection;

		private Zone _currentSelectedZone;

		private GamepadButton _currentSelectedZoneButton;

		private bool _isDiagonalDirections;

		protected ushort _xLow;

		protected ushort _xMed;

		protected ushort _xHigh;

		protected ushort _yLow;

		protected ushort _yMed;

		protected ushort _yHigh;

		protected bool _isRadialZoning;

		protected bool _isEllipticZoning;

		protected ushort _xSensitivity;

		protected ushort _ySensitivity;

		protected bool _springMode;

		private byte _returnTime;

		protected uint _smoothing;

		protected uint _noiseFilter;

		protected uint _scaleFactorX;

		protected uint _scaleFactorY;

		protected short _rotation;

		private bool _isHardwareDeadzone;

		private ObservableCollection<ThumbstickSensitivity> _sensitivitiesCollection;

		private ThumbstickSensitivity _sensitivitySensitivity;

		private DISC_SOFT_GAMEPAD_STICK_DEFLECTION_NPROPCHANGE _sensitivity;

		private ushort _flickStickSensitivity;

		private ushort _flickStickThreshold;

		private uint _flickStickDuration;

		private bool _isUseGlobalMouseSensitivity;

		private ushort _mouseSensitivity;

		private ushort _mouseSensitivityY;

		private ushort _mouseSmoothing;

		private ushort _mouseFlickSmoothing;

		private bool _isTrackballFriction;

		private ushort _trackballFriction;

		private static ThumbstickSensitivity _copySensitivity;

		private DelegateCommand _copyResponseCommand;

		private DelegateCommand _pasteResponseCommand;

		private DelegateCommand _clearResponseCommand;
	}
}
