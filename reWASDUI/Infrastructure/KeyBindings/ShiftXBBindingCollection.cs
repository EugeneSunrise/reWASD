using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DiscSoft.NET.Common.Utils;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Utils;
using reWASDUI.Infrastructure.KeyBindings.ControllerBindings;
using reWASDUI.Infrastructure.KeyBindings.MacroBinding;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups;
using reWASDUI.Services.Interfaces;
using reWASDUI.Utils.XBUtil;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Infrastructure.KeyBindings
{
	public class ShiftXBBindingCollection : BaseXBBindingCollection
	{
		public override string DefaultDescription
		{
			get
			{
				return UtilsCommon.GetDefaultShiftDescription(this.ShiftType, base.ShiftIndex);
			}
		}

		public IGameProfilesService GameProfilesService
		{
			get
			{
				return App.GameProfilesService;
			}
		}

		public Drawing ShiftIcon
		{
			get
			{
				return this._shiftIcon;
			}
			set
			{
				if (this._shiftIcon == value)
				{
					return;
				}
				this._shiftIcon = value;
				this.OnPropertyChanged("ShiftIcon");
			}
		}

		public ShiftType ShiftType
		{
			get
			{
				return this._shiftType;
			}
			set
			{
				if (this._shiftType == value)
				{
					return;
				}
				this._shiftType = value;
				this.OnPropertyChanged("ShiftType");
				this.OnPropertyChanged("IsOverlayShift");
			}
		}

		public bool IsOverlayShift
		{
			get
			{
				return this.ShiftType == 2;
			}
		}

		public override bool HasVirtualMapping
		{
			get
			{
				foreach (KeyValuePair<GamepadButton, XBBinding> keyValuePair in ((IEnumerable<KeyValuePair<GamepadButton, XBBinding>>)this))
				{
					if (GamepadButtonExtensions.IsAnyStick(keyValuePair.Key) || GamepadButtonExtensions.IsAnyStickClick(keyValuePair.Key))
					{
						if (keyValuePair.Value.IsAnyActivatorVirtualMappingPresent && !keyValuePair.Value.EqualsByVirtualMappings(this.ParentBindingCollection[keyValuePair.Key]))
						{
							return true;
						}
					}
					else if (keyValuePair.Value.IsAnyActivatorVirtualMappingPresent)
					{
						return true;
					}
				}
				return false;
			}
		}

		public override bool HasHardwareMapping
		{
			get
			{
				foreach (KeyValuePair<GamepadButton, XBBinding> keyValuePair in ((IEnumerable<KeyValuePair<GamepadButton, XBBinding>>)this))
				{
					if (GamepadButtonExtensions.IsAnyStick(keyValuePair.Key) || GamepadButtonExtensions.IsAnyStickClick(keyValuePair.Key))
					{
						if (keyValuePair.Value.IsRemapedOrUnmapped && !keyValuePair.Value.EqualsByHardwareMappings(this.ParentBindingCollection[keyValuePair.Key]))
						{
							return true;
						}
					}
					else if (keyValuePair.Value.IsRemapedOrUnmapped)
					{
						return true;
					}
				}
				return base.ControllerBindings.Any((ControllerBinding item) => item.XBBinding.IsUnmapped);
			}
		}

		public MainXBBindingCollection ParentBindingCollection
		{
			get
			{
				return this._parentBindingCollection;
			}
		}

		public override SubConfigData SubConfigData
		{
			get
			{
				MainXBBindingCollection parentBindingCollection = this.ParentBindingCollection;
				if (parentBindingCollection == null)
				{
					return null;
				}
				return parentBindingCollection.SubConfigData;
			}
			set
			{
				this.ParentBindingCollection.SubConfigData = value;
			}
		}

		public override void Dispose()
		{
			base.Dispose();
			this._parentBindingCollection = null;
			base.CollectionBrush = null;
			base.CollectionBrushHighlighted = null;
			base.CollectionBrushPressed = null;
			this.ShiftIcon = null;
		}

		public void CopyInheritedValuesFromMainBeforeSwapSticksIfNeeded()
		{
			this.CopyInheritedValuesFromMainLeftStick();
			this.CopyInheritedValuesFromMainRightStick();
		}

		public void CopyInheritedValuesFromMainLeftStick()
		{
			this.CopyInheritedValuesFromMainStick(base.LeftStickDirectionalGroup, this.ParentBindingCollection.LeftStickDirectionalGroup);
		}

		public void CopyInheritedValuesFromMainRightStick()
		{
			this.CopyInheritedValuesFromMainStick(base.RightStickDirectionalGroup, this.ParentBindingCollection.RightStickDirectionalGroup);
		}

		public void CopyInheritedValuesFromMainStick(BaseStickDirectionalGroup stick, BaseStickDirectionalGroup parentStick)
		{
			this.CopyXBBindingFromMainIfNeeded(stick.LeftDirectionValue, parentStick.LeftDirectionValue, false);
			this.CopyXBBindingFromMainIfNeeded(stick.RightDirectionValue, parentStick.RightDirectionValue, false);
			this.CopyXBBindingFromMainIfNeeded(stick.UpDirectionValue, parentStick.UpDirectionValue, false);
			this.CopyXBBindingFromMainIfNeeded(stick.DownDirectionValue, parentStick.DownDirectionValue, false);
			this.CopyXBBindingFromMainIfNeeded(stick.ClickValue, parentStick.ClickValue, true);
			this.CopyXBBindingFromMainIfNeeded(stick.LowZoneValue, parentStick.LowZoneValue, false);
			this.CopyXBBindingFromMainIfNeeded(stick.MedZoneValue, parentStick.MedZoneValue, false);
			this.CopyXBBindingFromMainIfNeeded(stick.HighZoneValue, parentStick.HighZoneValue, false);
		}

		private void CopyXBBindingFromMainIfNeeded(XBBinding curXB, XBBinding parXB, bool copyHM = false)
		{
			if (curXB.IsAnyActivatorVirtualMappingPresent || curXB.IsUnmapped)
			{
				return;
			}
			curXB.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(parXB.ActivatorXBBindingDictionary, null);
			if (copyHM && !curXB.IsRemapedOrUnmapped)
			{
				curXB.RemapedTo = parXB.RemapedTo;
			}
		}

		public void CopyInheritedValuesFromMainBeforeInvertIfNeeded(Stick stick, Orientation orientation)
		{
			BaseDirectionalGroup baseDirectionalGroup = null;
			BaseDirectionalGroup baseDirectionalGroup2 = null;
			if (stick == null)
			{
				baseDirectionalGroup = base.LeftStickDirectionalGroup;
				baseDirectionalGroup2 = this.ParentBindingCollection.LeftStickDirectionalGroup;
			}
			if (stick == 1)
			{
				baseDirectionalGroup = base.RightStickDirectionalGroup;
				baseDirectionalGroup2 = this.ParentBindingCollection.RightStickDirectionalGroup;
			}
			if (stick == 6)
			{
				baseDirectionalGroup = base.AdditionalStickDirectionalGroup;
				baseDirectionalGroup2 = this.ParentBindingCollection.AdditionalStickDirectionalGroup;
			}
			if (baseDirectionalGroup == null || baseDirectionalGroup2 == null)
			{
				return;
			}
			if (orientation == Orientation.Horizontal)
			{
				if (!baseDirectionalGroup.LeftDirectionValue.IsUnmapped && !baseDirectionalGroup.LeftDirectionValue.IsAnyActivatorVirtualMappingPresent)
				{
					baseDirectionalGroup.LeftDirectionValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(baseDirectionalGroup2.LeftDirectionValue.ActivatorXBBindingDictionary, null);
				}
				if (!baseDirectionalGroup.RightDirectionValue.IsUnmapped && !baseDirectionalGroup.RightDirectionValue.IsAnyActivatorVirtualMappingPresent)
				{
					baseDirectionalGroup.RightDirectionValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(baseDirectionalGroup2.RightDirectionValue.ActivatorXBBindingDictionary, null);
				}
			}
			if (orientation == Orientation.Vertical)
			{
				if (!baseDirectionalGroup.UpDirectionValue.IsUnmapped && !baseDirectionalGroup.UpDirectionValue.IsAnyActivatorVirtualMappingPresent)
				{
					baseDirectionalGroup.UpDirectionValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(baseDirectionalGroup2.UpDirectionValue.ActivatorXBBindingDictionary, null);
				}
				if (!baseDirectionalGroup.DownDirectionValue.IsUnmapped && !baseDirectionalGroup.DownDirectionValue.IsAnyActivatorVirtualMappingPresent)
				{
					baseDirectionalGroup.DownDirectionValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(baseDirectionalGroup2.DownDirectionValue.ActivatorXBBindingDictionary, null);
				}
			}
		}

		public ShiftXBBindingCollection(int shiftIndex, MainXBBindingCollection parentBindingCollection, ControllerFamily controllerFamily, ShiftType shiftType = 0)
			: base(controllerFamily)
		{
			base.ShiftIndex = shiftIndex;
			this.ShiftType = shiftType;
			this._parentBindingCollection = parentBindingCollection;
			this.InitIcons();
			base.CurrentButtonMappingChanged += delegate([Nullable(2)] object sender, EventArgs args)
			{
				base.UnmapCurrentCommand.RaiseCanExecuteChanged();
				this.OnPropertyChanged("IsCurrentButtonMappingDoNotInherit");
			};
			base.InitEventHandler();
			base.IsChanged = false;
		}

		public void InitIcons()
		{
			Application application = Application.Current;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Shift");
			defaultInterpolatedStringHandler.AppendFormatted<int>(base.ShiftIndex);
			defaultInterpolatedStringHandler.AppendLiteral("Brush");
			base.CollectionBrush = application.TryFindResource(defaultInterpolatedStringHandler.ToStringAndClear()) as SolidColorBrush;
			Application application2 = Application.Current;
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Shift");
			defaultInterpolatedStringHandler.AppendFormatted<int>(base.ShiftIndex);
			defaultInterpolatedStringHandler.AppendLiteral("BrushHighlighted");
			base.CollectionBrushHighlighted = application2.TryFindResource(defaultInterpolatedStringHandler.ToStringAndClear()) as SolidColorBrush;
			Application application3 = Application.Current;
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Shift");
			defaultInterpolatedStringHandler.AppendFormatted<int>(base.ShiftIndex);
			defaultInterpolatedStringHandler.AppendLiteral("BrushPressed");
			base.CollectionBrushPressed = application3.TryFindResource(defaultInterpolatedStringHandler.ToStringAndClear()) as SolidColorBrush;
			this.ShiftIcon = XBUtils.GetDrawingShiftModificatorNum(base.ShiftIndex);
			this.OnPropertyChanged("Description");
		}

		protected override void OnCollectionItemPropertyChangedExtended(object s, PropertyChangedExtendedEventArgs e)
		{
			if (e.PropertyName == "MacroSequenceAnnotation")
			{
				return;
			}
			base.OnCollectionItemPropertyChangedExtended(s, e);
			base.CheckDuplicateShiftMappingForButtonMapping(s, e, this);
			this.ParentBindingCollection.RaiseOnMainOrShiftCollectionItemPropertyChangedExtended(s, e);
		}

		protected override void OnMaskCollectionChangedExtended(object s, NotifyCollectionChangedEventArgs e)
		{
			this.ParentBindingCollection.RaiseOnMainOrShiftCollectionItemPropertyChangedExtended(s, null);
		}

		public override string ToString()
		{
			return base.Description;
		}

		public void CopyFromModel(ShiftXBBindingCollection model)
		{
			base.ShiftIndex = model.ShiftModificatorNum;
			this.ShiftType = model.ShiftType;
			base.CopyFromModel(model, true);
		}

		public void CopyToModel(ShiftXBBindingCollection model)
		{
			model.ShiftModificatorNum = base.ShiftIndex;
			model.ShiftType = this.ShiftType;
			base.CopyToModel(model);
		}

		private Drawing _shiftIcon;

		private MainXBBindingCollection _parentBindingCollection;

		private ShiftType _shiftType;
	}
}
