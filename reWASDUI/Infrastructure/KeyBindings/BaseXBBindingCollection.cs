using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Properties;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Attributes;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using Prism.Commands;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Infrastructure.KeyBindingsModel;
using reWASDCommon.Infrastructure.SupportedControllers;
using reWASDCommon.Infrastructure.SupportedControllers.Base;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings.ActivatorXB;
using reWASDUI.Infrastructure.KeyBindings.ControllerBindings;
using reWASDUI.Infrastructure.KeyBindings.MacroBinding;
using reWASDUI.Infrastructure.KeyBindings.Mask;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups;
using reWASDUI.Services.Interfaces;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Infrastructure.KeyBindingsModel.XB;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtilModel;

namespace reWASDUI.Infrastructure.KeyBindings
{
	public abstract class BaseXBBindingCollection : ObservableDictionary<GamepadButton, XBBinding>, IDisposable
	{
		public event BaseXBBindingCollection.IsChangedModified IsChangedModifiedEvent;

		public bool IsChanged
		{
			get
			{
				return this._isChanged;
			}
			set
			{
				this.OnPropertyChanged("IsCurrentBoundToWASD");
				this.OnPropertyChanged("IsCurrentBoundToKeyboard");
				this.OnPropertyChanged("IsCurrentBoundToFlickStick");
				this.OnPropertyChanged("IsCurrentBoundToMouse");
				this.OnPropertyChanged("IsCurrentHasMouseDirection");
				this.OnPropertyChanged("IsCurrentBoundToOverlayMenuDirections");
				this.OnPropertyChanged("IsCurrentBoundToDS4Touchpad");
				this.OnPropertyChanged("IsCurrentBoundToAnyInvertedMouse");
				this.OnPropertyChanged("IsCurrentUnmapped");
				this.OnPropertyChanged("IsTrackballFrictionAllowed");
				if (this._isChanged == value)
				{
					return;
				}
				this._isChanged = value;
				this.OnPropertyChanged("IsChanged");
				this.RiseIsChangedModifiedEvent();
			}
		}

		private ControllerTypeEnum? CurrentControllerType
		{
			get
			{
				BaseControllerVM currentGamepad = App.GamepadServiceLazy.Value.CurrentGamepad;
				if (currentGamepad == null)
				{
					return null;
				}
				ControllerVM currentController = currentGamepad.CurrentController;
				if (currentController == null)
				{
					return null;
				}
				return new ControllerTypeEnum?(currentController.ControllerType);
			}
		}

		public int ShiftIndex
		{
			get
			{
				return this._shiftIndex;
			}
			set
			{
				this.SetProperty<int>(ref this._shiftIndex, value, "ShiftIndex");
			}
		}

		public void RiseIsChangedModifiedEvent()
		{
			BaseXBBindingCollection.IsChangedModified isChangedModifiedEvent = this.IsChangedModifiedEvent;
			if (isChangedModifiedEvent == null)
			{
				return;
			}
			isChangedModifiedEvent();
		}

		public bool IsAdaptiveLeftTriggerSettingsPresent
		{
			get
			{
				AdaptiveTriggerSettings adaptiveLeftTriggerSettings = this.AdaptiveLeftTriggerSettings;
				return adaptiveLeftTriggerSettings != null && adaptiveLeftTriggerSettings.AdaptiveTriggerSettingsNonDefault();
			}
		}

		public bool IsAdaptiveRightTriggerSettingsPresent
		{
			get
			{
				AdaptiveTriggerSettings adaptiveRightTriggerSettings = this.AdaptiveRightTriggerSettings;
				return adaptiveRightTriggerSettings != null && adaptiveRightTriggerSettings.AdaptiveTriggerSettingsNonDefault();
			}
		}

		public bool IsOverlayCollection
		{
			get
			{
				if (this.IsShiftCollection)
				{
					ShiftXBBindingCollection shiftXBBindingCollection = this as ShiftXBBindingCollection;
					return shiftXBBindingCollection != null && shiftXBBindingCollection.IsOverlayShift;
				}
				return false;
			}
		}

		public bool IsShowSubconfig
		{
			get
			{
				bool flag = true;
				if (this.IsOverlayCollection && this.IsOverlayMenuModeView)
				{
					flag = false;
				}
				return !flag;
			}
		}

		public bool IsCollectionHasMappingsForCopy
		{
			get
			{
				if (!base.AnyValue((XBBinding kvp) => kvp.IsAnyActivatorVirtualMappingPresent) && !this.IsCollectionHasMappings)
				{
					if (!base.AnyValue((XBBinding xbb) => xbb.IsAnyActivatorDescriptionPresent) && !this.IsAdaptiveLeftTriggerSettingsPresent)
					{
						return this.IsAdaptiveRightTriggerSettingsPresent;
					}
				}
				return true;
			}
		}

		public virtual bool IsCollectionHasMappings
		{
			get
			{
				return this.HasVirtualMapping || this.HasVirtualGamepadMapping || this.HasMaskVirtualMapping || this.HasMaskVirtualGamepadMapping || this.HasControllerBindingsVirtualMapping || this.HasControllerBindingsVirtualMapping || this.HasHardwareMapping;
			}
		}

		public bool IsCollectionHasMappingsButNotMasks
		{
			get
			{
				return this.HasVirtualMapping || this.HasVirtualGamepadMapping || this.HasControllerBindingsVirtualMapping || this.HasControllerBindingsVirtualMapping || this.HasHardwareMapping || this.IsAdaptiveLeftTriggerSettingsPresent || this.IsAdaptiveRightTriggerSettingsPresent;
			}
		}

		public bool AnyPaddleIsMapped
		{
			get
			{
				return this.Where((KeyValuePair<GamepadButton, XBBinding> b) => GamepadButtonExtensions.IsAnyPaddle(b.Value.GamepadButton)).Any((KeyValuePair<GamepadButton, XBBinding> kvp) => !kvp.Value.ActivatorXBBindingDictionary.Any((KeyValuePair<ActivatorType, ActivatorXBBinding> kvp1) => kvp1.Value.MappedKey.IsNotMapped));
			}
		}

		public virtual bool HasVirtualMapping
		{
			get
			{
				return base.AnyValue((XBBinding kvp) => kvp.IsAnyActivatorVirtualMappingPresent);
			}
		}

		public bool HasMacroMapping
		{
			get
			{
				return base.AnyValue((XBBinding kvp) => kvp.IsAnyActivatorMacroMappingPresent);
			}
		}

		public bool HasTurboToggleMapping
		{
			get
			{
				return base.AnyValue((XBBinding kvp) => kvp.IsAnyActivatorTurboTogglePresent);
			}
		}

		public virtual bool HasToggleOrCustomOrDelayBeforeJumpShift
		{
			get
			{
				return base.AnyValue(delegate(XBBinding kvp)
				{
					ActivatorXBBindingDictionary activatorXBBindingDictionary = kvp.ActivatorXBBindingDictionary;
					return activatorXBBindingDictionary != null && activatorXBBindingDictionary.AnyValue((ActivatorXBBinding activator) => activator.IsJumpToShift && (activator.IsDelayBerforeJumpChecked || (this.IsMainCollection && !activator.IsShiftHold)));
				});
			}
		}

		public virtual bool HasVirtualGamepadMapping
		{
			get
			{
				return base.AnyValue((XBBinding kvp) => kvp.IsAnyActivatorVirtualGamepadMappingPresent);
			}
		}

		public virtual bool HasJumpToShift
		{
			get
			{
				bool result = false;
				this.EnumAllBindings(true, true, true).ForEach(delegate(XBBinding xbb)
				{
					result = result || xbb.IsContainsJumpToShift;
				});
				return result;
			}
		}

		public virtual bool HasMaskVirtualMapping
		{
			get
			{
				MaskItemCollection maskBindingCollection = this.MaskBindingCollection;
				if (maskBindingCollection == null)
				{
					return false;
				}
				return maskBindingCollection.Any((MaskItem mi) => mi.XBBinding.IsAnyActivatorVirtualMappingPresent);
			}
		}

		public virtual bool HasKeyboardMaskWithVirtualMapping
		{
			get
			{
				MaskItemCollection maskBindingCollection = this.MaskBindingCollection;
				if (maskBindingCollection == null)
				{
					return false;
				}
				return maskBindingCollection.Any((MaskItem mi) => mi.MaskConditions.IsKeyboardPresent() && mi.XBBinding.IsAnyActivatorVirtualMappingPresent);
			}
		}

		public virtual bool HasMouseDigitalMaskWithVirtualMapping
		{
			get
			{
				MaskItemCollection maskBindingCollection = this.MaskBindingCollection;
				if (maskBindingCollection == null)
				{
					return false;
				}
				return maskBindingCollection.Any((MaskItem mi) => mi.MaskConditions.IsMouseDigitalPresent() && mi.XBBinding.IsAnyActivatorVirtualMappingPresent);
			}
		}

		public virtual bool HasMouseDirectionalGroupVirtualMapping
		{
			get
			{
				return this.MouseDirectionalGroup.IsBoundToVirtualLeftStick || this.MouseDirectionalGroup.IsBoundToVirtualRightStick;
			}
		}

		public virtual bool HasMaskVirtualGamepadMapping
		{
			get
			{
				MaskItemCollection maskBindingCollection = this.MaskBindingCollection;
				if (maskBindingCollection == null)
				{
					return false;
				}
				return maskBindingCollection.Any((MaskItem mi) => mi.XBBinding.IsAnyActivatorVirtualGamepadMappingPresent);
			}
		}

		public virtual bool HasControllerBindingsVirtualMapping
		{
			get
			{
				return this.ControllerBindings.Any((ControllerBinding cb) => cb.XBBinding.IsAnyActivatorVirtualMappingPresent);
			}
		}

		public virtual bool HasControllerBindingsVirtualGamepadMapping
		{
			get
			{
				return this.ControllerBindings.Any((ControllerBinding cb) => cb.XBBinding.IsAnyActivatorVirtualGamepadMappingPresent);
			}
		}

		public virtual bool HasRumble
		{
			get
			{
				return this.Any((KeyValuePair<GamepadButton, XBBinding> kvp) => kvp.Value.ActivatorXBBindingDictionary.Any((KeyValuePair<ActivatorType, ActivatorXBBinding> kvp1) => kvp1.Value.IsRumble || kvp1.Value.HasMacroRumble));
			}
		}

		public virtual bool HasMouseToStick
		{
			get
			{
				return this.Any((KeyValuePair<GamepadButton, XBBinding> kvp) => kvp.Value.ActivatorXBBindingDictionary.Any((KeyValuePair<ActivatorType, ActivatorXBBinding> kvp1) => kvp1.Value.IsMouseToStick));
			}
		}

		public virtual bool HasHardwareMapping
		{
			get
			{
				if (!base.AnyValue((XBBinding kvp) => kvp.IsRemapedOrUnmapped))
				{
					return this.ControllerBindings.Any((ControllerBinding item) => item.XBBinding.IsUnmapped);
				}
				return true;
			}
		}

		public bool HasMacroFeatureMapping(ControllerTypeEnum controllerType)
		{
			return this.EnumAllBindingsForController(controllerType).Any((XBBinding xbbinding) => xbbinding.IsAnyActivatorMacroMappingPresent);
		}

		public bool HasRapidFeatureMapping(ControllerTypeEnum controllerType)
		{
			return this.EnumAllBindingsForController(controllerType).Any(delegate(XBBinding xbbinding)
			{
				if (!xbbinding.IsAnyActivatorTurboTogglePresent)
				{
					ActivatorXBBindingDictionary activatorXBBindingDictionary = xbbinding.ActivatorXBBindingDictionary;
					return activatorXBBindingDictionary != null && activatorXBBindingDictionary.AnyValue((ActivatorXBBinding activator) => activator.IsJumpToShift && (activator.IsDelayBerforeJumpChecked || (this.IsMainCollection && !activator.IsShiftHold)));
				}
				return true;
			});
		}

		public bool HasAdvancedFeatureMapping(ControllerTypeEnum controllerType)
		{
			bool isAllowedHardwareMappingWithoutAdvancedFeature = ControllerTypeExtensions.IsAllowedHardwareMappingWithoutAdvancedFeature(controllerType);
			bool flag = ControllerTypeExtensions.IsAnySteam(controllerType);
			bool flag2 = ControllerTypeExtensions.IsAnyAzeron(controllerType);
			bool flag3 = ControllerTypeExtensions.IsAzeronCyro(controllerType);
			bool flag4 = ControllerTypeExtensions.IsXboxEliteOrOne(controllerType);
			bool flag5 = ControllerTypeExtensions.IsRightStickDirectionsAvailiable(controllerType);
			if (this.EnumAllBindingsForController(controllerType).Any((XBBinding xbbinding) => xbbinding.ActivatorXBBindingDictionary.AnyValue((ActivatorXBBinding activator) => activator.IsMacroMapping && activator.MacroSequence.HasComboWithAdvancedFeature) || (!isAllowedHardwareMappingWithoutAdvancedFeature && xbbinding.IsRemapedOrUnmapped)))
			{
				return true;
			}
			if (this.SubConfigData.IsGamepad)
			{
				if (this.LeftStickDirectionalGroup.SensitivitySensitivity.IsCustom || (flag5 && this.RightStickDirectionalGroup.SensitivitySensitivity.IsCustom) || (ControllerTypeExtensions.IsGyroAvailiable(controllerType) && !this.GyroTiltDirectionalGroup.IsSensitivityDefault()) || (ControllerTypeExtensions.IsTouchpadExpected(controllerType) && !this.Touchpad1DirectionalGroup.IsSensitivityDefault()) || (flag && !this.Touchpad2DirectionalGroup.IsSensitivityDefault()) || this.LeftStickDirectionalGroup.IsHardwareDeadzone || (flag5 && this.RightStickDirectionalGroup.IsHardwareDeadzone))
				{
					return true;
				}
				if ((!flag && !flag2 && (this.LeftStickDirectionalGroup.IsUnmapped || (flag5 && this.RightStickDirectionalGroup.IsUnmapped))) || (!flag4 && (this.IsLeftStickSwapped || (flag5 && this.IsRightStickSwapped))))
				{
					return true;
				}
				if (!isAllowedHardwareMappingWithoutAdvancedFeature && (!this.LeftStickDirectionalGroup.IsSensitivityDefault() || (flag5 && !this.RightStickDirectionalGroup.IsSensitivityDefault()) || (this.IsLeftStickXInverted || (flag5 && this.IsRightStickXInverted)) || this.IsLeftStickYInverted || (flag5 && this.IsRightStickYInverted)))
				{
					return true;
				}
			}
			return (this.SubConfigData.IsMouse && (this.MouseDirectionalGroup.IsBoundToVirtualLeftStick || this.MouseDirectionalGroup.IsBoundToVirtualRightStick || this.MouseDirectionalGroup.IsUnmapped)) || (this.SubConfigData.IsGamepad && flag3 && (this.MouseDirectionalGroup.IsBoundToVirtualLeftStick || this.MouseDirectionalGroup.IsBoundToVirtualRightStick));
		}

		public string AppName
		{
			get
			{
				return this._appName;
			}
			set
			{
				if (this._appName == value)
				{
					return;
				}
				this._appName = value;
				this.OnPropertyChanged("AppName");
			}
		}

		public ObservableCollection<string> ProcessNames
		{
			get
			{
				return this._processNames;
			}
			set
			{
				if (this._processNames == value)
				{
					return;
				}
				this._processNames = value;
				this.OnPropertyChanged("ProcessNames");
			}
		}

		public string Comment
		{
			get
			{
				return this._comment;
			}
			set
			{
				if (this._comment == value)
				{
					return;
				}
				this._comment = value;
				this.OnPropertyChanged("Comment");
			}
		}

		public string Author
		{
			get
			{
				return this._author;
			}
			set
			{
				if (this._author == value)
				{
					return;
				}
				this._author = value;
				this.OnPropertyChanged("Author");
			}
		}

		public virtual string DefaultDescription
		{
			get
			{
				return "";
			}
		}

		public string Description
		{
			get
			{
				if (string.IsNullOrEmpty(this._description))
				{
					return this.DefaultDescription;
				}
				return this._description;
			}
			set
			{
				if (this._description == value)
				{
					return;
				}
				this._description = value;
				this.OnPropertyChanged("Description");
			}
		}

		public SolidColorBrush CollectionBrushSemiTransparent
		{
			get
			{
				SolidColorBrush solidColorBrush = this.CollectionBrush.Clone();
				solidColorBrush.Opacity = 0.1;
				return solidColorBrush;
			}
		}

		public SolidColorBrush CollectionBrush20Opacity
		{
			get
			{
				SolidColorBrush solidColorBrush = this.CollectionBrush.Clone();
				solidColorBrush.Opacity = 0.2;
				return solidColorBrush;
			}
		}

		public SolidColorBrush CollectionBrush
		{
			get
			{
				return this._collectionBrush;
			}
			set
			{
				if (this._collectionBrush == value)
				{
					return;
				}
				this._collectionBrush = value;
				this.OnPropertyChanged("CollectionBrush");
				this.OnPropertyChanged("CollectionBrushSemiTransparent");
			}
		}

		public SolidColorBrush CollectionBrushHighlightedSemiTransparent
		{
			get
			{
				SolidColorBrush solidColorBrush = this.CollectionBrushHighlighted.Clone();
				solidColorBrush.Opacity = 0.1;
				return solidColorBrush;
			}
		}

		public SolidColorBrush CollectionBrushHighlighted
		{
			get
			{
				return this._collectionBrushHighlighted;
			}
			set
			{
				if (this._collectionBrushHighlighted == value)
				{
					return;
				}
				this._collectionBrushHighlighted = value;
				this.OnPropertyChanged("CollectionBrushHighlighted");
				this.OnPropertyChanged("CollectionBrushHighlightedSemiTransparent");
			}
		}

		public SolidColorBrush CollectionBrushPressedSemiTransparent
		{
			get
			{
				SolidColorBrush solidColorBrush = this.CollectionBrushPressed.Clone();
				solidColorBrush.Opacity = 0.1;
				return solidColorBrush;
			}
		}

		public SolidColorBrush CollectionBrushPressed
		{
			get
			{
				return this._collectionBrushPressed;
			}
			set
			{
				if (this._collectionBrushPressed == value)
				{
					return;
				}
				this._collectionBrushPressed = value;
				this.OnPropertyChanged("CollectionBrushPressed");
				this.OnPropertyChanged("CollectionBrushPressedSemiTransparent");
			}
		}

		public ControllerBindingFrameAdditionalModes? ControllerBindingFrameMode
		{
			get
			{
				return this._controllerBindingFrameMode;
			}
			set
			{
				if (this.SetProperty<ControllerBindingFrameAdditionalModes?>(ref this._controllerBindingFrameMode, value, "ControllerBindingFrameMode"))
				{
					this.OnPropertyChanged("CurrentControllerButton");
					this.OnPropertyChanged("CurrentXBBinding");
					App.EventAggregator.GetEvent<CurrentButtonMappingChanged>().Publish(null);
				}
			}
		}

		public AssociatedControllerButton CurrentControllerButton
		{
			get
			{
				if (this.ControllerBindingFrameMode != null)
				{
					return new AssociatedControllerButton(this.ControllerBindingFrameMode.Value);
				}
				XBBinding currentXBBinding = this.CurrentXBBinding;
				if (currentXBBinding == null)
				{
					return null;
				}
				return currentXBBinding.ControllerButton;
			}
		}

		public XBBinding CurrentXBBinding
		{
			get
			{
				if (this.ControllerBindingFrameMode != null)
				{
					ControllerBindingFrameAdditionalModes? controllerBindingFrameMode = this.ControllerBindingFrameMode;
					ControllerBindingFrameAdditionalModes controllerBindingFrameAdditionalModes = 1;
					if (!((controllerBindingFrameMode.GetValueOrDefault() == controllerBindingFrameAdditionalModes) & (controllerBindingFrameMode != null)))
					{
						return null;
					}
				}
				XBBinding xbbinding;
				if ((xbbinding = this.CurrentButtonMappingValue) == null)
				{
					MaskItemCollection maskBindingCollection = this.MaskBindingCollection;
					XBBinding xbbinding2;
					if (maskBindingCollection == null)
					{
						xbbinding2 = null;
					}
					else
					{
						MaskItem currentEditItem = maskBindingCollection.CurrentEditItem;
						xbbinding2 = ((currentEditItem != null) ? currentEditItem.XBBinding : null);
					}
					if ((xbbinding = xbbinding2) == null)
					{
						ControllerBinding currentEditItem2 = this.ControllerBindings.CurrentEditItem;
						if (currentEditItem2 == null)
						{
							return null;
						}
						return currentEditItem2.XBBinding;
					}
				}
				return xbbinding;
			}
		}

		public bool IsSameScrollDelta
		{
			get
			{
				return this._isSameScrollDelta;
			}
			set
			{
				if (this._isSameScrollDelta == value)
				{
					return;
				}
				this._isSameScrollDelta = value;
				this.OnPropertyChanged("IsSameScrollDelta");
			}
		}

		public DelegateCommand<XBBinding> SetSameScrollDeltaCommand
		{
			get
			{
				DelegateCommand<XBBinding> delegateCommand;
				if ((delegateCommand = this._setSameScrollDeltaCommand) == null)
				{
					delegateCommand = (this._setSameScrollDeltaCommand = new DelegateCommand<XBBinding>(new Action<XBBinding>(this.SetSameScrollDelta)));
				}
				return delegateCommand;
			}
		}

		private void SetSameScrollDelta(XBBinding xbBinding)
		{
			if (xbBinding != null && GamepadButtonExtensions.IsMouseScroll(xbBinding.GamepadButton))
			{
				GamepadButton gamepadButton = xbBinding.GamepadButton;
				if (gamepadButton != 171)
				{
					if (gamepadButton == 172)
					{
						XBBinding xbbinding = base.TryGetValue(171);
						if (xbbinding != null)
						{
							xbbinding.MouseScrollDelta = xbBinding.MouseScrollDelta;
						}
					}
				}
				else
				{
					XBBinding xbbinding2 = base.TryGetValue(172);
					if (xbbinding2 != null)
					{
						xbbinding2.MouseScrollDelta = xbBinding.MouseScrollDelta;
					}
				}
				this.OnPropertyChanged("IsSameScrollDelta");
				this.IsChanged = true;
			}
		}

		public virtual bool IsHardwareChangesPresent
		{
			get
			{
				return this.LeftStickDirectionalGroup.IsHardwareChangesPresent || this.RightStickDirectionalGroup.IsHardwareChangesPresent || this.AdditionalStickDirectionalGroup.IsHardwareChangesPresent;
			}
		}

		public virtual bool IsHardwareChangesPresentForPrint
		{
			get
			{
				return this.LeftStickDirectionalGroup.IsHardwareChangesPresentForPrint || this.RightStickDirectionalGroup.IsHardwareChangesPresentForPrint || this.AdditionalStickDirectionalGroup.IsHardwareChangesPresentForPrint;
			}
		}

		public Dictionary<GamepadButton, XBBinding> ButtonMappings
		{
			get
			{
				return this.Where((KeyValuePair<GamepadButton, XBBinding> kvp) => GamepadButtonExtensions.IsDigital(kvp.Value.GamepadButton)).ToDictionary((KeyValuePair<GamepadButton, XBBinding> i) => i.Key, (KeyValuePair<GamepadButton, XBBinding> i) => i.Value);
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public KeyValuePair<GamepadButton, XBBinding>? CurrentButtonMapping
		{
			get
			{
				return this._currentButtonMapping;
			}
			private set
			{
				if (this.SetProperty<KeyValuePair<GamepadButton, XBBinding>?>(ref this._currentButtonMapping, value, "CurrentButtonMapping"))
				{
					App.EventAggregator.GetEvent<CurrentButtonMappingChanged>().Publish(null);
					BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
					if (currentGamepad != null)
					{
						currentGamepad.FirePropertyChanged("IsExtendedMappingAvailable");
					}
					BaseControllerVM currentGamepad2 = App.GamepadService.CurrentGamepad;
					if (currentGamepad2 != null)
					{
						currentGamepad2.FirePropertyChanged("IsExtendedMappingAvailableConsideringMappings");
					}
					EventHandler currentButtonMappingChanged = this.CurrentButtonMappingChanged;
					if (currentButtonMappingChanged != null)
					{
						currentButtonMappingChanged(this, EventArgs.Empty);
					}
					this.OnPropertyChanged("IsZoneMappingSettingsAvailiableForCurrentBinding");
					this.OnPropertyChanged("IsZoneMappingSettingsExistForCurrentBinding");
					this.OnPropertyChanged("IsPressureZonesMappingSettingsAvailiableForCurrentBinding");
					this.OnPropertyChanged("IsPressureZoneMappingSettingsExistForCurrentBinding");
					this.OnPropertyChanged("IsAdaptiveTriggerSettingsAvailiableForCurrentBinding");
					this.OnPropertyChanged("IsAdaptiveTriggerSettingsExistForCurrentBinding");
					this.OnPropertyChanged("CurrentAdaptiveTriggerSettings");
					this.OnPropertyChanged("CurrentBindingIsPhysicalTrackPad1");
					this.OnPropertyChanged("CurrentBindingIsPhysicalTrackPad2");
					this.OnPropertyChanged("CurrentBindingIsGyroTilt");
					this.OnPropertyChanged("CurrentBindingIsLeftStick");
					this.OnPropertyChanged("CurrentBindingIsRightStick");
					this.OnPropertyChanged("CurrentBindingIsLeftTrigger");
					this.OnPropertyChanged("CurrentBindingIsRightTrigger");
					this.OnPropertyChanged("CurrentBindingIsMouseDirectionOrZone");
					this.OnPropertyChanged("CurrentBindingIsPhysicalTrackPad1Direction");
					this.OnPropertyChanged("CurrentBindingIsPhysicalTrackPad2Direction");
					this.OnPropertyChanged("CurrentBindingIsGyroTiltDirection");
					this.OnPropertyChanged("CurrentBindingIsGyroTiltLean");
					this.OnPropertyChanged("CurrentBindingIsLeftStickDirection");
					this.OnPropertyChanged("CurrentBindingIsRightStickDirection");
					this.OnPropertyChanged("CurrentBindingIsLeftTriggerPress");
					this.OnPropertyChanged("CurrentBindingIsRightTriggerPress");
					this.OnPropertyChanged("CurrentBindingIsGyroTiltZone");
					this.OnPropertyChanged("CurrentBindingIsLeftStickZone");
					this.OnPropertyChanged("CurrentBindingIsRightStickZone");
					this.OnPropertyChanged("CurrentBindingIsLeftTriggerZone");
					this.OnPropertyChanged("CurrentBindingIsRightTriggerZone");
					this.OnPropertyChanged("CurrentBindingIsLeftDS3AnalogZone");
					this.OnPropertyChanged("CurrentBindingIsRightDS3AnalogZone");
					this.OnPropertyChanged("CurrentBindingIsTrackpad1Zone");
					this.OnPropertyChanged("CurrentBindingIsTrackpad2Zone");
					this.OnPropertyChanged("CurrentBindingIsDS3DPADUpAnalogZone");
					this.OnPropertyChanged("CurrentBindingIsDS3DPADDownAnalogZone");
					this.OnPropertyChanged("CurrentBindingIsDS3DPADLeftAnalogZone");
					this.OnPropertyChanged("CurrentBindingIsDS3DPADRightAnalogZone");
					this.OnPropertyChanged("CurrentBindingIsDS3LeftBumperAnalogZone");
					this.OnPropertyChanged("CurrentBindingIsDS3CrossAnalogZone");
					this.OnPropertyChanged("CurrentBindingIsDS3CircleAnalogZone");
					this.OnPropertyChanged("CurrentBindingIsDS3SquareAnalogZone");
					this.OnPropertyChanged("CurrentBindingIsDS3TriangleAnalogZone");
					this.OnPropertyChanged("CurrentBindingIsDS3RightBumperAnalogZone");
					this._currentBoundGroup = null;
					this.OnPropertyChanged("CurrentBoundGroup");
					this.OnPropertyChanged("IsAdvanceMappingSettingsAvailiableForCurrentBinding");
					this.OnPropertyChanged("IsAdvanceMappingSettingsChanged");
					this.OnPropertyChanged("IsGamepadMappingAvailiableForCurrentBinding");
					this.OnPropertyChanged("CurrentTouchpadDirectionalGroup");
					this.OnPropertyChanged("CurrenDirectionalAnalogGroup");
					this.OnPropertyChanged("IsCurrentBoundToWASD");
					this.OnPropertyChanged("IsCurrentBoundToKeyboard");
					this.OnPropertyChanged("IsCurrentBoundToFlickStick");
					this.OnPropertyChanged("IsCurrentBoundToMouse");
					this.OnPropertyChanged("IsCurrentHasMouseDirection");
					this.OnPropertyChanged("IsCurrentBoundToDS4Touchpad");
					this.OnPropertyChanged("IsCurrentBoundToAnyInvertedMouse");
					this.OnPropertyChanged("IsCurrentBoundToLeftVirtualStick");
					this.OnPropertyChanged("IsCurrentBoundToRightVirtualStick");
					this.OnPropertyChanged("IsCurrentBoundToVirtualDPAD");
					this.OnPropertyChanged("IsCurrentUnmapped");
					this.OnPropertyChanged("CurrentControllerButton");
					this.OnPropertyChanged("CurrentXBBinding");
					this.OnPropertyChanged("CurrentButtonMappingValue");
					this.OnPropertyChanged("CurrentButtonMappingKey");
					this.OnPropertyChanged("IsCurrentButtonMappingDoNotInherit");
					this.OnPropertyChanged("IsTrackballFrictionAllowed");
					this.BindCurrentToWASDCommand.RaiseCanExecuteChanged();
					this.BindCurrentToLeftVirtualStickCommand.RaiseCanExecuteChanged();
					this.BindCurrentToRightVirtualStickCommand.RaiseCanExecuteChanged();
					this.BindCurrentToFlickStickCommand.RaiseCanExecuteChanged();
					this.BindCurrentToMouseCommand.RaiseCanExecuteChanged();
					this.UnmapCurrentCommand.RaiseCanExecuteChanged();
				}
			}
		}

		public XBBinding CurrentButtonMappingValue
		{
			get
			{
				if (this.CurrentButtonMapping == null)
				{
					return null;
				}
				KeyValuePair<GamepadButton, XBBinding>? keyValuePair;
				return keyValuePair.GetValueOrDefault().Value;
			}
		}

		public GamepadButton? CurrentButtonMappingKey
		{
			get
			{
				if (this.CurrentButtonMapping == null)
				{
					return null;
				}
				KeyValuePair<GamepadButton, XBBinding>? keyValuePair;
				return new GamepadButton?(keyValuePair.GetValueOrDefault().Key);
			}
		}

		public event EventHandler CurrentButtonMappingChanged;

		public void SetCurrentButtonMapping(AssociatedControllerButton controllerButton)
		{
			if (controllerButton == null)
			{
				this.SetCurrentButtonMapping(null, true);
				this.ControllerBindings.SetCurrentEditTo(null);
				return;
			}
			if (controllerButton.IsGamepad)
			{
				this.SetCurrentButtonMapping(new GamepadButton?(controllerButton.GamepadButton), true);
				return;
			}
			if (controllerButton.IsKeyScanCode)
			{
				this.ControllerBindings.SetCurrentEditTo(controllerButton);
			}
		}

		public void SetCurrentButtonMapping(ControllerBindingFrameAdditionalModes bindingFrameMode)
		{
			if (bindingFrameMode == null)
			{
				foreach (SubConfigData subConfigData in this.SubConfigData.ConfigData.ConfigVM.ConfigData)
				{
					if (subConfigData.IsGamepad)
					{
						subConfigData.MainXBBindingCollection.ControllerBindingFrameMode = new ControllerBindingFrameAdditionalModes?(bindingFrameMode);
					}
				}
			}
			this.ControllerBindingFrameMode = new ControllerBindingFrameAdditionalModes?(bindingFrameMode);
			this.SetCurrentButtonMapping(null, false);
		}

		public void SetCurrentButtonMapping(GamepadButton? zisCurrentButtonToBind, bool changeControllerBindingFrameMode = true)
		{
			if (changeControllerBindingFrameMode)
			{
				ControllerBindingFrameAdditionalModes? controllerBindingFrameMode = this.ControllerBindingFrameMode;
				ControllerBindingFrameAdditionalModes controllerBindingFrameAdditionalModes = 0;
				if ((controllerBindingFrameMode.GetValueOrDefault() == controllerBindingFrameAdditionalModes) & (controllerBindingFrameMode != null))
				{
					foreach (SubConfigData subConfigData in this.SubConfigData.ConfigData.ConfigVM.ConfigData)
					{
						subConfigData.MainXBBindingCollection.ControllerBindingFrameMode = null;
					}
				}
				this.ControllerBindingFrameMode = null;
			}
			GamepadButton? gamepadButton = this.CurrentButtonMappingKey;
			GamepadButton? gamepadButton2 = zisCurrentButtonToBind;
			if ((gamepadButton.GetValueOrDefault() == gamepadButton2.GetValueOrDefault()) & (gamepadButton != null == (gamepadButton2 != null)))
			{
				return;
			}
			XBBinding currentButtonMappingValue = this.CurrentButtonMappingValue;
			if (currentButtonMappingValue != null)
			{
				currentButtonMappingValue.SetNonEmptyCurrentActivator(false);
			}
			if (zisCurrentButtonToBind == null)
			{
				this.CurrentButtonMapping = null;
				this.CurrentButtonMapping = null;
				return;
			}
			if (this.CurrentButtonMapping != null)
			{
				gamepadButton2 = this.CurrentButtonMappingKey;
				gamepadButton = zisCurrentButtonToBind;
				if ((gamepadButton2.GetValueOrDefault() == gamepadButton.GetValueOrDefault()) & (gamepadButton2 != null == (gamepadButton != null)))
				{
					return;
				}
			}
			this.CurrentButtonMapping = new KeyValuePair<GamepadButton, XBBinding>?(this.FirstOrDefault(delegate(KeyValuePair<GamepadButton, XBBinding> btnmap)
			{
				GamepadButton key = btnmap.Key;
				GamepadButton? zisCurrentButtonToBind2 = zisCurrentButtonToBind;
				return (key == zisCurrentButtonToBind2.GetValueOrDefault()) & (zisCurrentButtonToBind2 != null);
			}));
		}

		public void SetCurrentButtonMapping(MouseButton mb)
		{
			this.SetCurrentButtonMapping(null);
			this.ControllerBindings.CurrentEditItem = this.ControllerBindings.FirstOrDefault((ControllerBinding kvp) => kvp.XBBinding.KeyScanCode == KeyScanCodeV2.FindKeyScanCodeByMouseButton(mb));
			this.OnPropertyChanged("CurrentControllerButton");
			this.OnPropertyChanged("CurrentXBBinding");
			this.OnPropertyChanged("IsZoneMappingSettingsAvailiableForCurrentBinding");
			this.OnPropertyChanged("IsPressureZonesMappingSettingsAvailiableForCurrentBinding");
			this.OnPropertyChanged("IsAdaptiveTriggerSettingsAvailiableForCurrentBinding");
			this.OnPropertyChanged("CurrentAdaptiveTriggerSettings");
			this.OnPropertyChanged("IsCurrentButtonMappingDoNotInherit");
		}

		public bool IsGamepadMappingAvailiableForCurrentBinding
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum = this.CurrentControllerType;
				KeyValuePair<GamepadButton, XBBinding>? keyValuePair;
				if (controllerTypeEnum != null && ControllerTypeExtensions.IsXboxOrRazerWolverine2(controllerTypeEnum.GetValueOrDefault()))
				{
					keyValuePair = this.CurrentButtonMapping;
					if (keyValuePair != null && keyValuePair.GetValueOrDefault().Key == 11)
					{
						return false;
					}
				}
				controllerTypeEnum = this.CurrentControllerType;
				if (controllerTypeEnum != null && ControllerTypeExtensions.IsGameSirG7(controllerTypeEnum.GetValueOrDefault()))
				{
					keyValuePair = this.CurrentButtonMapping;
					if (keyValuePair == null || keyValuePair.GetValueOrDefault().Key != 12)
					{
						keyValuePair = this.CurrentButtonMapping;
						if (keyValuePair == null || keyValuePair.GetValueOrDefault().Key != 13)
						{
							keyValuePair = this.CurrentButtonMapping;
							if (keyValuePair == null || keyValuePair.GetValueOrDefault().Key != 14)
							{
								keyValuePair = this.CurrentButtonMapping;
								if (keyValuePair == null || keyValuePair.GetValueOrDefault().Key != 29)
								{
									keyValuePair = this.CurrentButtonMapping;
									if (keyValuePair == null || keyValuePair.GetValueOrDefault().Key != 30)
									{
										goto IL_13C;
									}
								}
							}
						}
					}
					return false;
				}
				IL_13C:
				controllerTypeEnum = this.CurrentControllerType;
				if (controllerTypeEnum != null && ControllerTypeExtensions.IsSonyDualSenseEdge(controllerTypeEnum.GetValueOrDefault()))
				{
					keyValuePair = this.CurrentButtonMapping;
					if (keyValuePair == null || keyValuePair.GetValueOrDefault().Key != 14)
					{
						keyValuePair = this.CurrentButtonMapping;
						if (keyValuePair == null || keyValuePair.GetValueOrDefault().Key != 15)
						{
							keyValuePair = this.CurrentButtonMapping;
							if (keyValuePair == null || keyValuePair.GetValueOrDefault().Key != 16)
							{
								keyValuePair = this.CurrentButtonMapping;
								if (keyValuePair == null || keyValuePair.GetValueOrDefault().Key != 17)
								{
									goto IL_202;
								}
							}
						}
					}
					return false;
				}
				IL_202:
				controllerTypeEnum = this.CurrentControllerType;
				if (controllerTypeEnum != null && ControllerTypeExtensions.IsGamepadNativeMappingNotAvailiable(controllerTypeEnum.GetValueOrDefault()))
				{
					return false;
				}
				controllerTypeEnum = this.CurrentControllerType;
				if (controllerTypeEnum != null && ControllerTypeExtensions.IsGamepadWithSonyTouchpad(controllerTypeEnum.GetValueOrDefault()))
				{
					keyValuePair = this.CurrentButtonMapping;
					if (keyValuePair != null && GamepadButtonExtensions.IsPhysicalTrackPad1DigitalDirection(keyValuePair.GetValueOrDefault().Key))
					{
						Touchpad1DirectionalGroup touchpad1DirectionalGroup = this.CurrenDirectionalAnalogGroup as Touchpad1DirectionalGroup;
						if (touchpad1DirectionalGroup != null && touchpad1DirectionalGroup.TouchpadAnalogMode)
						{
							return false;
						}
					}
				}
				keyValuePair = this.CurrentButtonMapping;
				return keyValuePair != null && GamepadButtonExtensions.IsGamepadMappingAvailiable(keyValuePair.GetValueOrDefault().Key);
			}
		}

		public bool IsAdvanceMappingSettingsAvailiableForCurrentBinding
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				if (this.CurrentControllerType != null && ControllerTypeExtensions.IsSNES(controllerTypeEnum.GetValueOrDefault()))
				{
					return false;
				}
				BaseDirectionalAnalogGroup baseDirectionalAnalogGroup = this.CurrentBoundGroup as BaseDirectionalAnalogGroup;
				if (baseDirectionalAnalogGroup != null && !baseDirectionalAnalogGroup.IsAdvancedSettingsVisible)
				{
					return false;
				}
				XBBinding currentXBBinding = this.CurrentXBBinding;
				return currentXBBinding != null && currentXBBinding.ControllerButton.IsAdvancedMappingAvailiable;
			}
		}

		public bool IsAdvanceMappingSettingsChanged
		{
			get
			{
				if (this.IsAdvanceMappingSettingsAvailiableForCurrentBinding)
				{
					BaseDirectionalGroup currentBoundGroup = this.CurrentBoundGroup;
					return currentBoundGroup != null && !currentBoundGroup.IsAdvancedDefault();
				}
				return false;
			}
		}

		public bool IsZoneMappingSettingsAvailiableForCurrentBinding
		{
			get
			{
				KeyValuePair<GamepadButton, XBBinding>? keyValuePair;
				return this.CurrentButtonMapping != null && GamepadButtonExtensions.IsZonesMappingAvailiable(keyValuePair.GetValueOrDefault().Key, this.CurrentControllerType);
			}
		}

		public bool IsPressureZonesMappingSettingsAvailiableForCurrentBinding
		{
			get
			{
				KeyValuePair<GamepadButton, XBBinding>? keyValuePair;
				return this.CurrentButtonMapping != null && GamepadButtonExtensions.IsPressureZonesMappingAvailiable(keyValuePair.GetValueOrDefault().Key, this.CurrentControllerType);
			}
		}

		public bool IsCurrentHarwareDeadzoneTriggerSet
		{
			get
			{
				if (!this.IsMainCollection)
				{
					return false;
				}
				KeyValuePair<GamepadButton, XBBinding>? keyValuePair = this.CurrentButtonMapping;
				if (keyValuePair == null || keyValuePair.GetValueOrDefault().Key != 51 || !(this as MainXBBindingCollection).IsHardwareDeadzoneLeftTrigger)
				{
					keyValuePair = this.CurrentButtonMapping;
					return keyValuePair != null && keyValuePair.GetValueOrDefault().Key == 55 && (this as MainXBBindingCollection).IsHardwareDeadzoneRightTrigger;
				}
				return true;
			}
		}

		public bool IsZoneMappingSettingsExistForCurrentBinding
		{
			get
			{
				if (this.CurrentButtonMapping != null && this.IsZoneMappingSettingsAvailiableForCurrentBinding)
				{
					if (!this.IsCurrentHarwareDeadzoneTriggerSet)
					{
						XBBinding xbbindingByGamepadButton = this.GetXBBindingByGamepadButton(GamepadButtonExtensions.GetZone(this.CurrentButtonMapping.Value.Key, 0));
						if (xbbindingByGamepadButton == null || xbbindingByGamepadButton.IsEmpty)
						{
							XBBinding xbbindingByGamepadButton2 = this.GetXBBindingByGamepadButton(GamepadButtonExtensions.GetZone(this.CurrentButtonMapping.Value.Key, 1));
							if (xbbindingByGamepadButton2 == null || xbbindingByGamepadButton2.IsEmpty)
							{
								XBBinding xbbindingByGamepadButton3 = this.GetXBBindingByGamepadButton(GamepadButtonExtensions.GetZone(this.CurrentButtonMapping.Value.Key, 2));
								return xbbindingByGamepadButton3 != null && !xbbindingByGamepadButton3.IsEmpty;
							}
						}
					}
					return true;
				}
				return false;
			}
		}

		public bool IsPressureZoneMappingSettingsExistForCurrentBinding
		{
			get
			{
				if (this.CurrentButtonMapping != null && this.IsPressureZonesMappingSettingsAvailiableForCurrentBinding)
				{
					XBBinding xbbindingByGamepadButton = this.GetXBBindingByGamepadButton(GamepadButtonExtensions.GetZone(this.CurrentButtonMapping.Value.Key, 0));
					if (xbbindingByGamepadButton == null || xbbindingByGamepadButton.IsEmpty)
					{
						XBBinding xbbindingByGamepadButton2 = this.GetXBBindingByGamepadButton(GamepadButtonExtensions.GetZone(this.CurrentButtonMapping.Value.Key, 1));
						if (xbbindingByGamepadButton2 == null || xbbindingByGamepadButton2.IsEmpty)
						{
							XBBinding xbbindingByGamepadButton3 = this.GetXBBindingByGamepadButton(GamepadButtonExtensions.GetZone(this.CurrentButtonMapping.Value.Key, 2));
							return xbbindingByGamepadButton3 != null && !xbbindingByGamepadButton3.IsEmpty;
						}
					}
					return true;
				}
				return false;
			}
		}

		public bool IsAdaptiveTriggerSettingsAvailiableForCurrentBinding
		{
			get
			{
				KeyValuePair<GamepadButton, XBBinding>? keyValuePair;
				return this.CurrentButtonMapping != null && GamepadButtonExtensions.IsAdaptiveTriggerSettingsAvailiable(keyValuePair.GetValueOrDefault().Key, this.CurrentControllerType);
			}
		}

		public bool IsAdaptiveTriggerSettingsExistForCurrentBinding
		{
			get
			{
				return this.IsAdaptiveTriggerSettingsAvailiableForCurrentBinding && (this.CurrentAdaptiveTriggerSettings != null && this.CurrentAdaptiveTriggerSettings.Preset != null) && this.CurrentAdaptiveTriggerSettings.Preset != 2;
			}
		}

		public AdaptiveTriggerSettings CurrentAdaptiveTriggerSettings
		{
			get
			{
				KeyValuePair<GamepadButton, XBBinding>? keyValuePair;
				if (this.CurrentButtonMapping == null || !GamepadButtonExtensions.IsLeftTriggerPress(keyValuePair.GetValueOrDefault().Key))
				{
					return this._adaptiveRightTriggerSettings;
				}
				return this._adaptiveLeftTriggerSettings;
			}
			set
			{
				KeyValuePair<GamepadButton, XBBinding>? keyValuePair;
				if (this.CurrentButtonMapping != null && GamepadButtonExtensions.IsLeftTriggerPress(keyValuePair.GetValueOrDefault().Key))
				{
					this.AdaptiveLeftTriggerSettings = value;
				}
				else
				{
					this.AdaptiveRightTriggerSettings = value;
				}
				this.OnPropertyChanged("CurrentAdaptiveTriggerSettings");
				this.IsChanged = true;
				this.RiseIsChangedModifiedEvent();
			}
		}

		public bool CurrentBindingIsGyroTilt
		{
			get
			{
				XBBinding currentXBBinding = this.CurrentXBBinding;
				return currentXBBinding != null && currentXBBinding.IsGyroTilt;
			}
		}

		public bool CurrentBindingIsLeftStick
		{
			get
			{
				XBBinding currentXBBinding = this.CurrentXBBinding;
				return currentXBBinding != null && currentXBBinding.IsLeftStick;
			}
		}

		public bool CurrentBindingIsRightStick
		{
			get
			{
				XBBinding currentXBBinding = this.CurrentXBBinding;
				return currentXBBinding != null && currentXBBinding.IsRightStick;
			}
		}

		public bool CurrentBindingIsAdditionalStick
		{
			get
			{
				XBBinding currentXBBinding = this.CurrentXBBinding;
				return currentXBBinding != null && currentXBBinding.IsAdditionalStick;
			}
		}

		public bool CurrentBindingIsLeftTrigger
		{
			get
			{
				XBBinding currentXBBinding = this.CurrentXBBinding;
				return currentXBBinding != null && currentXBBinding.IsLeftTrigger;
			}
		}

		public bool CurrentBindingIsRightTrigger
		{
			get
			{
				XBBinding currentXBBinding = this.CurrentXBBinding;
				return currentXBBinding != null && currentXBBinding.IsRightTrigger;
			}
		}

		public bool CurrentBindingIsPhysicalTrackPad1
		{
			get
			{
				XBBinding currentXBBinding = this.CurrentXBBinding;
				return currentXBBinding != null && currentXBBinding.IsPhysicalTrackPad1;
			}
		}

		public bool CurrentBindingIsPhysicalTrackPad2
		{
			get
			{
				XBBinding currentXBBinding = this.CurrentXBBinding;
				return currentXBBinding != null && currentXBBinding.IsPhysicalTrackPad2;
			}
		}

		public bool CurrentBindingIsGyroTiltDirection
		{
			get
			{
				XBBinding currentXBBinding = this.CurrentXBBinding;
				return currentXBBinding != null && currentXBBinding.IsGyroTiltDirection;
			}
		}

		public bool CurrentBindingIsGyroTiltLean
		{
			get
			{
				XBBinding currentXBBinding = this.CurrentXBBinding;
				return currentXBBinding != null && currentXBBinding.IsGyroLean;
			}
		}

		public bool CurrentBindingIsMouseDirectionOrZone
		{
			get
			{
				XBBinding currentXBBinding = this.CurrentXBBinding;
				return currentXBBinding != null && currentXBBinding.IsMouseStick;
			}
		}

		public bool CurrentBindingIsLeftStickDirection
		{
			get
			{
				XBBinding currentXBBinding = this.CurrentXBBinding;
				return currentXBBinding != null && currentXBBinding.IsLeftStickDirection;
			}
		}

		public bool CurrentBindingIsRightStickDirection
		{
			get
			{
				XBBinding currentXBBinding = this.CurrentXBBinding;
				return currentXBBinding != null && currentXBBinding.IsRightStickDirection;
			}
		}

		public bool CurrentBindingIsLeftTriggerPress
		{
			get
			{
				XBBinding currentXBBinding = this.CurrentXBBinding;
				return currentXBBinding != null && currentXBBinding.IsLeftTriggerPress;
			}
		}

		public bool CurrentBindingIsRightTriggerPress
		{
			get
			{
				XBBinding currentXBBinding = this.CurrentXBBinding;
				return currentXBBinding != null && currentXBBinding.IsRightTriggerPress;
			}
		}

		public bool CurrentBindingIsPhysicalTrackPad1Direction
		{
			get
			{
				XBBinding currentXBBinding = this.CurrentXBBinding;
				return currentXBBinding != null && currentXBBinding.IsPhysicalTrackPad1Direction;
			}
		}

		public bool CurrentBindingIsPhysicalTrackPad2Direction
		{
			get
			{
				XBBinding currentXBBinding = this.CurrentXBBinding;
				return currentXBBinding != null && currentXBBinding.IsPhysicalTrackPad2Direction;
			}
		}

		public bool CurrentBindingIsGyroTiltZone
		{
			get
			{
				XBBinding currentXBBinding = this.CurrentXBBinding;
				return currentXBBinding != null && currentXBBinding.IsGyroTiltZone;
			}
		}

		public bool CurrentBindingIsLeftStickZone
		{
			get
			{
				XBBinding currentXBBinding = this.CurrentXBBinding;
				return currentXBBinding != null && currentXBBinding.IsLeftStickZone;
			}
		}

		public bool CurrentBindingIsRightStickZone
		{
			get
			{
				XBBinding currentXBBinding = this.CurrentXBBinding;
				return currentXBBinding != null && currentXBBinding.IsRightStickZone;
			}
		}

		public bool CurrentBindingIsLeftTriggerZone
		{
			get
			{
				XBBinding currentXBBinding = this.CurrentXBBinding;
				return currentXBBinding != null && currentXBBinding.IsLeftTriggerZone;
			}
		}

		public bool CurrentBindingIsRightTriggerZone
		{
			get
			{
				XBBinding currentXBBinding = this.CurrentXBBinding;
				return currentXBBinding != null && currentXBBinding.IsRightTriggerZone;
			}
		}

		public bool CurrentBindingIsTrackpad1Zone
		{
			get
			{
				XBBinding currentXBBinding = this.CurrentXBBinding;
				return currentXBBinding != null && GamepadButtonExtensions.IsPhysicalTrackPad1PressureZone(currentXBBinding.GamepadButton);
			}
		}

		public bool CurrentBindingIsTrackpad2Zone
		{
			get
			{
				XBBinding currentXBBinding = this.CurrentXBBinding;
				return currentXBBinding != null && GamepadButtonExtensions.IsPhysicalTrackPad2PressureZone(currentXBBinding.GamepadButton);
			}
		}

		public bool CurrentBindingIsDS3CrossAnalogZone
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				if (this.CurrentControllerType != null && ControllerTypeExtensions.IsSonyDS3(controllerTypeEnum.GetValueOrDefault()))
				{
					XBBinding currentXBBinding = this.CurrentXBBinding;
					return currentXBBinding != null && GamepadButtonExtensions.IsCrossZone(currentXBBinding.GamepadButton);
				}
				return false;
			}
		}

		public bool CurrentBindingIsDS3CircleAnalogZone
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				if (this.CurrentControllerType != null && ControllerTypeExtensions.IsSonyDS3(controllerTypeEnum.GetValueOrDefault()))
				{
					XBBinding currentXBBinding = this.CurrentXBBinding;
					return currentXBBinding != null && GamepadButtonExtensions.IsCircleZone(currentXBBinding.GamepadButton);
				}
				return false;
			}
		}

		public bool CurrentBindingIsDS3SquareAnalogZone
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				if (this.CurrentControllerType != null && ControllerTypeExtensions.IsSonyDS3(controllerTypeEnum.GetValueOrDefault()))
				{
					XBBinding currentXBBinding = this.CurrentXBBinding;
					return currentXBBinding != null && GamepadButtonExtensions.IsSquareZone(currentXBBinding.GamepadButton);
				}
				return false;
			}
		}

		public bool CurrentBindingIsDS3TriangleAnalogZone
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				if (this.CurrentControllerType != null && ControllerTypeExtensions.IsSonyDS3(controllerTypeEnum.GetValueOrDefault()))
				{
					XBBinding currentXBBinding = this.CurrentXBBinding;
					return currentXBBinding != null && GamepadButtonExtensions.IsTriangleZone(currentXBBinding.GamepadButton);
				}
				return false;
			}
		}

		public bool CurrentBindingIsDS3RightBumperAnalogZone
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				if (this.CurrentControllerType != null && ControllerTypeExtensions.IsSonyDS3(controllerTypeEnum.GetValueOrDefault()))
				{
					XBBinding currentXBBinding = this.CurrentXBBinding;
					return currentXBBinding != null && GamepadButtonExtensions.IsRightBumperZone(currentXBBinding.GamepadButton);
				}
				return false;
			}
		}

		public bool CurrentBindingIsDS3LeftBumperAnalogZone
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				if (this.CurrentControllerType != null && ControllerTypeExtensions.IsSonyDS3(controllerTypeEnum.GetValueOrDefault()))
				{
					XBBinding currentXBBinding = this.CurrentXBBinding;
					return currentXBBinding != null && GamepadButtonExtensions.IsLeftBumperZone(currentXBBinding.GamepadButton);
				}
				return false;
			}
		}

		public bool CurrentBindingIsDS3CrossAnalogButton
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				if (this.CurrentControllerType != null && ControllerTypeExtensions.IsSonyDS3(controllerTypeEnum.GetValueOrDefault()))
				{
					XBBinding currentXBBinding = this.CurrentXBBinding;
					return currentXBBinding != null && currentXBBinding.GamepadButton == 1;
				}
				return false;
			}
		}

		public bool CurrentBindingIsDS3CircleAnalogButton
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				if (this.CurrentControllerType != null && ControllerTypeExtensions.IsSonyDS3(controllerTypeEnum.GetValueOrDefault()))
				{
					XBBinding currentXBBinding = this.CurrentXBBinding;
					return currentXBBinding != null && currentXBBinding.GamepadButton == 2;
				}
				return false;
			}
		}

		public bool CurrentBindingIsDS3SquareAnalogButton
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				if (this.CurrentControllerType != null && ControllerTypeExtensions.IsSonyDS3(controllerTypeEnum.GetValueOrDefault()))
				{
					XBBinding currentXBBinding = this.CurrentXBBinding;
					return currentXBBinding != null && currentXBBinding.GamepadButton == 3;
				}
				return false;
			}
		}

		public bool CurrentBindingIsDS3TriangleAnalogButton
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				if (this.CurrentControllerType != null && ControllerTypeExtensions.IsSonyDS3(controllerTypeEnum.GetValueOrDefault()))
				{
					XBBinding currentXBBinding = this.CurrentXBBinding;
					return currentXBBinding != null && currentXBBinding.GamepadButton == 4;
				}
				return false;
			}
		}

		public bool CurrentBindingIsDS3DPADUpAnalogZone
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				if (this.CurrentControllerType != null && ControllerTypeExtensions.IsSonyDS3(controllerTypeEnum.GetValueOrDefault()))
				{
					XBBinding currentXBBinding = this.CurrentXBBinding;
					return currentXBBinding != null && GamepadButtonExtensions.IsDPADUpZone(currentXBBinding.GamepadButton);
				}
				return false;
			}
		}

		public bool CurrentBindingIsDS3DPADDownAnalogZone
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				if (this.CurrentControllerType != null && ControllerTypeExtensions.IsSonyDS3(controllerTypeEnum.GetValueOrDefault()))
				{
					XBBinding currentXBBinding = this.CurrentXBBinding;
					return currentXBBinding != null && GamepadButtonExtensions.IsDPADDownZone(currentXBBinding.GamepadButton);
				}
				return false;
			}
		}

		public bool CurrentBindingIsDS3DPADLeftAnalogZone
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				if (this.CurrentControllerType != null && ControllerTypeExtensions.IsSonyDS3(controllerTypeEnum.GetValueOrDefault()))
				{
					XBBinding currentXBBinding = this.CurrentXBBinding;
					return currentXBBinding != null && GamepadButtonExtensions.IsDPADLeftZone(currentXBBinding.GamepadButton);
				}
				return false;
			}
		}

		public bool CurrentBindingIsDS3DPADRightAnalogZone
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				if (this.CurrentControllerType != null && ControllerTypeExtensions.IsSonyDS3(controllerTypeEnum.GetValueOrDefault()))
				{
					XBBinding currentXBBinding = this.CurrentXBBinding;
					return currentXBBinding != null && GamepadButtonExtensions.IsDPADRightZone(currentXBBinding.GamepadButton);
				}
				return false;
			}
		}

		public bool CurrentBindingIsDS3DPADUpAnalogButton
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				if (this.CurrentControllerType != null && ControllerTypeExtensions.IsSonyDS3(controllerTypeEnum.GetValueOrDefault()))
				{
					XBBinding currentXBBinding = this.CurrentXBBinding;
					return currentXBBinding != null && currentXBBinding.GamepadButton == 33;
				}
				return false;
			}
		}

		public bool CurrentBindingIsDS3DPADDownAnalogButton
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				if (this.CurrentControllerType != null && ControllerTypeExtensions.IsSonyDS3(controllerTypeEnum.GetValueOrDefault()))
				{
					XBBinding currentXBBinding = this.CurrentXBBinding;
					return currentXBBinding != null && currentXBBinding.GamepadButton == 34;
				}
				return false;
			}
		}

		public bool CurrentBindingIsDS3DPADLeftAnalogButton
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				if (this.CurrentControllerType != null && ControllerTypeExtensions.IsSonyDS3(controllerTypeEnum.GetValueOrDefault()))
				{
					XBBinding currentXBBinding = this.CurrentXBBinding;
					return currentXBBinding != null && currentXBBinding.GamepadButton == 35;
				}
				return false;
			}
		}

		public bool CurrentBindingIsDS3DPADRightAnalogButton
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				if (this.CurrentControllerType != null && ControllerTypeExtensions.IsSonyDS3(controllerTypeEnum.GetValueOrDefault()))
				{
					XBBinding currentXBBinding = this.CurrentXBBinding;
					return currentXBBinding != null && currentXBBinding.GamepadButton == 36;
				}
				return false;
			}
		}

		public bool CurrentBindingIsLeftDS3AnalogZone
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				if (this.CurrentControllerType != null && ControllerTypeExtensions.IsSonyDS3(controllerTypeEnum.GetValueOrDefault()))
				{
					XBBinding currentXBBinding = this.CurrentXBBinding;
					return currentXBBinding != null && GamepadButtonExtensions.IsLeftDS3AnalogZone(currentXBBinding.GamepadButton);
				}
				return false;
			}
		}

		public bool CurrentBindingIsRightDS3AnalogZone
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				if (this.CurrentControllerType != null && ControllerTypeExtensions.IsSonyDS3(controllerTypeEnum.GetValueOrDefault()))
				{
					XBBinding currentXBBinding = this.CurrentXBBinding;
					return currentXBBinding != null && GamepadButtonExtensions.IsRightDS3AnalogZone(currentXBBinding.GamepadButton);
				}
				return false;
			}
		}

		public bool CurrentBindingIsLeftDS3AnalogButton
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				if (this.CurrentControllerType != null && ControllerTypeExtensions.IsSonyDS3(controllerTypeEnum.GetValueOrDefault()))
				{
					XBBinding currentXBBinding = this.CurrentXBBinding;
					return currentXBBinding != null && GamepadButtonExtensions.IsLeftDS3AnalogButton(currentXBBinding.GamepadButton);
				}
				return false;
			}
		}

		public bool CurrentBindingIsRightDS3AnalogButton
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				if (this.CurrentControllerType != null && ControllerTypeExtensions.IsSonyDS3(controllerTypeEnum.GetValueOrDefault()))
				{
					XBBinding currentXBBinding = this.CurrentXBBinding;
					return currentXBBinding != null && GamepadButtonExtensions.IsRightDS3AnalogButton(currentXBBinding.GamepadButton);
				}
				return false;
			}
		}

		public bool IsCurrentButtonMappingDoNotInherit
		{
			get
			{
				XBBinding currentXBBinding = this.CurrentXBBinding;
				if (((currentXBBinding != null) ? currentXBBinding.SingleActivator.MappedKey : null) != KeyScanCodeV2.NoInheritance)
				{
					MaskItemCollection maskBindingCollection = this.MaskBindingCollection;
					KeyScanCodeV2 keyScanCodeV;
					if (maskBindingCollection == null)
					{
						keyScanCodeV = null;
					}
					else
					{
						MaskItem currentEditItem = maskBindingCollection.CurrentEditItem;
						keyScanCodeV = ((currentEditItem != null) ? currentEditItem.XBBinding.SingleActivator.MappedKey : null);
					}
					return keyScanCodeV == KeyScanCodeV2.NoInheritance;
				}
				return true;
			}
		}

		public bool IsShiftCollection
		{
			get
			{
				return this is ShiftXBBindingCollection;
			}
		}

		public bool IsMainCollection
		{
			get
			{
				return this is MainXBBindingCollection;
			}
		}

		public bool IsAdvancedZonesSettingsVisible
		{
			get
			{
				if (!(this is MainXBBindingCollection))
				{
					MouseDirectionalGroup mouseDirectionalGroup = this.CurrentBoundGroup as MouseDirectionalGroup;
					return mouseDirectionalGroup != null && mouseDirectionalGroup.SpringMode;
				}
				return true;
			}
		}

		public ControllerBindingsCollection ControllerBindings { get; set; }

		public bool IsControllerBindingsEmpty
		{
			get
			{
				return this.ControllerBindings.Count == 0;
			}
		}

		public bool IsControllerBindingsEmptyWithoutStandart
		{
			get
			{
				if (this.ControllerBindings.Count == 0)
				{
					return true;
				}
				return !this.ControllerBindings.Any((ControllerBinding x) => !x.XBBinding.IsEmpty);
			}
		}

		public bool IsControllerBindingsPresent
		{
			get
			{
				return this.ControllerBindings.Count > 0;
			}
		}

		public bool IsControllerBindingsKeyboardPresent
		{
			get
			{
				return this.IsControllerBindingsPresent && (this.IsControllerBindingsKeyboardStandartPresent || this.IsControllerBindingsKeyboardConsumerPresent || this.IsControllerBindingsKeyboardSystemPresent);
			}
		}

		public bool IsControllerBindingsKeyboardStandartPresent
		{
			get
			{
				if (this.IsControllerBindingsPresent)
				{
					return this.ControllerBindings.Any((ControllerBinding x) => x.XBBinding.KeyScanCode.IsCategoryKeyboardStandart && (x.XBBinding.IsAnyActivatorVirtualMappingPresent || x.XBBinding.IsUnmapped || x.XBBinding.IsAnyActivatorDescriptionPresent));
				}
				return false;
			}
		}

		public bool IsControllerBindingsKeyboardConsumerPresent
		{
			get
			{
				if (this.IsControllerBindingsPresent)
				{
					return this.ControllerBindings.Any((ControllerBinding x) => x.XBBinding.KeyScanCode.IsCategoryKeyboardConsumer && (x.XBBinding.IsAnyActivatorVirtualMappingPresent || x.XBBinding.IsUnmapped || x.XBBinding.IsAnyActivatorDescriptionPresent));
				}
				return false;
			}
		}

		public bool IsControllerBindingsKeyboardSystemPresent
		{
			get
			{
				if (this.IsControllerBindingsPresent)
				{
					return this.ControllerBindings.Any((ControllerBinding x) => x.XBBinding.KeyScanCode.IsCategoryKeyboardSystem && (x.XBBinding.IsAnyActivatorVirtualMappingPresent || x.XBBinding.IsUnmapped || x.XBBinding.IsAnyActivatorDescriptionPresent));
				}
				return false;
			}
		}

		public bool IsControllerBindingsMousePresent
		{
			get
			{
				return this.IsControllerBindingsPresent && (this.IsControllerBindingsMouseDigitalPresent || this.IsControllerBindingsMouseAnalogPresent);
			}
		}

		public bool IsControllerBindingsMouseDigitalPresent
		{
			get
			{
				if (this.IsControllerBindingsPresent)
				{
					return this.ControllerBindings.Any((ControllerBinding x) => x.XBBinding.KeyScanCode.IsCategoryMouseDigital && (x.XBBinding.IsAnyActivatorVirtualMappingPresent || x.XBBinding.IsUnmapped || x.XBBinding.IsAnyActivatorDescriptionPresent));
				}
				return false;
			}
		}

		public bool IsControllerBindingsMouseAnalogPresent
		{
			get
			{
				if (this.IsControllerBindingsPresent)
				{
					return this.ControllerBindings.Any((ControllerBinding x) => x.XBBinding.KeyScanCode.IsCategoryMouseAnalog && (x.XBBinding.IsAnyActivatorVirtualMappingPresent || x.XBBinding.IsUnmapped || x.XBBinding.IsAnyActivatorDescriptionPresent));
				}
				return false;
			}
		}

		public VirtualDeviceSettings VirtualDeviceSettings
		{
			get
			{
				return this.SubConfigData.ConfigData.GetShiftVirtualSticks(this.ShiftIndex);
			}
		}

		public VirtualStickSettings LeftVirtualStick
		{
			get
			{
				return this.SubConfigData.ConfigData.GetShiftVirtualSticks(this.ShiftIndex).VirtualLeftStick;
			}
		}

		public VirtualStickSettings RightVirtualStick
		{
			get
			{
				return this.SubConfigData.ConfigData.GetShiftVirtualSticks(this.ShiftIndex).VirtualRightStick;
			}
		}

		public VirtualGyroSettings VirtualGyro
		{
			get
			{
				return this.SubConfigData.ConfigData.GetShiftVirtualSticks(this.ShiftIndex).VirtualGyro;
			}
		}

		public ObservableCollection<AdaptiveTriggerSettings> AdaptiveTriggerSettingsCollection
		{
			get
			{
				this.UnriseAdaptiveTriggerSettingsEventHandler();
				KeyValuePair<GamepadButton, XBBinding>? keyValuePair;
				ObservableCollection<AdaptiveTriggerSettings> observableCollection;
				if (this.CurrentButtonMapping == null || !GamepadButtonExtensions.IsLeftTriggerPress(keyValuePair.GetValueOrDefault().Key))
				{
					AdaptiveTriggersSettings adaptiveTriggersSettings = new AdaptiveTriggersSettings();
					bool isShiftCollection = this.IsShiftCollection;
					ShiftXBBindingCollection shiftXBBindingCollection = this as ShiftXBBindingCollection;
					observableCollection = adaptiveTriggersSettings.CreateAdaptiveTriggerSettings(ref this._adaptiveRightTriggerSettings, isShiftCollection, (shiftXBBindingCollection != null) ? shiftXBBindingCollection.ParentBindingCollection.AdaptiveRightTriggerSettings : null);
				}
				else
				{
					AdaptiveTriggersSettings adaptiveTriggersSettings2 = new AdaptiveTriggersSettings();
					bool isShiftCollection2 = this.IsShiftCollection;
					ShiftXBBindingCollection shiftXBBindingCollection2 = this as ShiftXBBindingCollection;
					observableCollection = adaptiveTriggersSettings2.CreateAdaptiveTriggerSettings(ref this._adaptiveLeftTriggerSettings, isShiftCollection2, (shiftXBBindingCollection2 != null) ? shiftXBBindingCollection2.ParentBindingCollection.AdaptiveLeftTriggerSettings : null);
				}
				this.RiseAdaptiveTriggerSettingsEventHandler();
				return observableCollection;
			}
		}

		private void AdaptiveTriggerSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.IsChanged = true;
			this.RiseIsChangedModifiedEvent();
		}

		private void UnriseAdaptiveTriggerSettingsEventHandler()
		{
			if (this._adaptiveLeftTriggerSettings != null)
			{
				this._adaptiveLeftTriggerSettings.PropertyChanged -= this.AdaptiveTriggerSettings_PropertyChanged;
			}
			if (this._adaptiveRightTriggerSettings != null)
			{
				this._adaptiveRightTriggerSettings.PropertyChanged -= this.AdaptiveTriggerSettings_PropertyChanged;
			}
		}

		private void RiseAdaptiveTriggerSettingsEventHandler()
		{
			if (this._adaptiveLeftTriggerSettings != null)
			{
				this._adaptiveLeftTriggerSettings.PropertyChanged += this.AdaptiveTriggerSettings_PropertyChanged;
			}
			if (this._adaptiveRightTriggerSettings != null)
			{
				this._adaptiveRightTriggerSettings.PropertyChanged += this.AdaptiveTriggerSettings_PropertyChanged;
			}
		}

		private void EnsureAdaptiveLeftTriggerSettingsExist()
		{
			if (this.AdaptiveLeftTriggerSettings == null)
			{
				ShiftXBBindingCollection shiftXBBindingCollection = this as ShiftXBBindingCollection;
				if (shiftXBBindingCollection == null || shiftXBBindingCollection.ParentBindingCollection.AdaptiveLeftTriggerSettings.Preset > 0)
				{
					AdaptiveTriggersSettings adaptiveTriggersSettings = new AdaptiveTriggersSettings();
					ShiftXBBindingCollection shiftXBBindingCollection2 = this as ShiftXBBindingCollection;
					this._adaptiveLeftTriggerSettings = adaptiveTriggersSettings.CreateAdaptiveTriggerSettingsInherited((shiftXBBindingCollection2 != null) ? shiftXBBindingCollection2.ParentBindingCollection.AdaptiveLeftTriggerSettings : null);
				}
			}
			if (this.AdaptiveRightTriggerSettings == null)
			{
				ShiftXBBindingCollection shiftXBBindingCollection3 = this as ShiftXBBindingCollection;
				if (shiftXBBindingCollection3 == null || shiftXBBindingCollection3.ParentBindingCollection.AdaptiveRightTriggerSettings.Preset > 0)
				{
					AdaptiveTriggersSettings adaptiveTriggersSettings2 = new AdaptiveTriggersSettings();
					ShiftXBBindingCollection shiftXBBindingCollection4 = this as ShiftXBBindingCollection;
					this._adaptiveRightTriggerSettings = adaptiveTriggersSettings2.CreateAdaptiveTriggerSettingsInherited((shiftXBBindingCollection4 != null) ? shiftXBBindingCollection4.ParentBindingCollection.AdaptiveRightTriggerSettings : null);
				}
			}
		}

		public void UpdateAdapterTriggersMappings()
		{
			if (!base.ContainsKey(51))
			{
				return;
			}
			ActivatorXBBinding activatorXBBinding = base[51].ActivatorXBBindingDictionary[0];
			ActivatorXBBinding activatorXBBinding2 = base[55].ActivatorXBBindingDictionary[0];
			BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
			bool flag;
			if (currentGamepad == null)
			{
				flag = false;
			}
			else
			{
				ControllerVM currentController = currentGamepad.CurrentController;
				bool? flag2 = ((currentController != null) ? new bool?(ControllerTypeExtensions.IsAnySonyDualSense(currentController.ControllerType)) : null);
				bool flag3 = true;
				flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
			}
			if (!flag)
			{
				activatorXBBinding.IsAdaptiveTriggers = false;
				activatorXBBinding2.IsAdaptiveTriggers = false;
				activatorXBBinding.IsAdaptiveTriggersInherited = false;
				activatorXBBinding2.IsAdaptiveTriggersInherited = false;
				return;
			}
			this.EnsureAdaptiveLeftTriggerSettingsExist();
			AdaptiveTriggerPreset[] array = new AdaptiveTriggerPreset[]
			{
				default(AdaptiveTriggerPreset),
				2,
				1
			};
			ActivatorXBBinding activatorXBBinding3 = activatorXBBinding;
			bool flag4;
			if (this.AdaptiveLeftTriggerSettings == null || array.Contains(this.AdaptiveLeftTriggerSettings.Preset))
			{
				if (this.AdaptiveLeftTriggerSettings == null || this.AdaptiveLeftTriggerSettings.IsInherited)
				{
					ShiftXBBindingCollection shiftXBBindingCollection = this as ShiftXBBindingCollection;
					flag4 = shiftXBBindingCollection == null || shiftXBBindingCollection.ParentBindingCollection.AdaptiveLeftTriggerSettings.Preset > 0;
				}
				else
				{
					flag4 = false;
				}
			}
			else
			{
				flag4 = true;
			}
			activatorXBBinding3.IsAdaptiveTriggers = flag4;
			ActivatorXBBinding activatorXBBinding4 = activatorXBBinding2;
			bool flag5;
			if (this.AdaptiveRightTriggerSettings == null || array.Contains(this.AdaptiveRightTriggerSettings.Preset))
			{
				if (this.AdaptiveRightTriggerSettings == null || this.AdaptiveRightTriggerSettings.IsInherited)
				{
					ShiftXBBindingCollection shiftXBBindingCollection2 = this as ShiftXBBindingCollection;
					flag5 = shiftXBBindingCollection2 == null || shiftXBBindingCollection2.ParentBindingCollection.AdaptiveRightTriggerSettings.Preset > 0;
				}
				else
				{
					flag5 = false;
				}
			}
			else
			{
				flag5 = true;
			}
			activatorXBBinding4.IsAdaptiveTriggers = flag5;
			if (this.IsShiftCollection)
			{
				ActivatorXBBinding activatorXBBinding5 = activatorXBBinding;
				bool flag6;
				if (this.AdaptiveLeftTriggerSettings != null)
				{
					AdaptiveTriggerSettings adaptiveLeftTriggerSettings = this.AdaptiveLeftTriggerSettings;
					flag6 = adaptiveLeftTriggerSettings != null && adaptiveLeftTriggerSettings.IsInherited;
				}
				else
				{
					flag6 = true;
				}
				activatorXBBinding5.IsAdaptiveTriggersInherited = flag6;
				ActivatorXBBinding activatorXBBinding6 = activatorXBBinding2;
				bool flag7;
				if (this.AdaptiveRightTriggerSettings != null)
				{
					AdaptiveTriggerSettings adaptiveRightTriggerSettings = this.AdaptiveRightTriggerSettings;
					flag7 = adaptiveRightTriggerSettings != null && adaptiveRightTriggerSettings.IsInherited;
				}
				else
				{
					flag7 = true;
				}
				activatorXBBinding6.IsAdaptiveTriggersInherited = flag7;
				return;
			}
			activatorXBBinding.IsAdaptiveTriggersInherited = false;
			activatorXBBinding2.IsAdaptiveTriggersInherited = false;
		}

		public AdaptiveTriggerSettings AdaptiveLeftTriggerSettings
		{
			get
			{
				if (this._adaptiveLeftTriggerSettings == null && this.IsMainCollection)
				{
					this._adaptiveLeftTriggerSettings = new AdaptiveTriggersSettings().CreateAdaptiveTriggerSettingsDefault();
				}
				return this._adaptiveLeftTriggerSettings;
			}
			set
			{
				if (value != this._adaptiveLeftTriggerSettings)
				{
					this.UnriseAdaptiveTriggerSettingsEventHandler();
					this.SetProperty<AdaptiveTriggerSettings>(ref this._adaptiveLeftTriggerSettings, value, "AdaptiveLeftTriggerSettings");
					this.RiseAdaptiveTriggerSettingsEventHandler();
					this.UpdateAdapterTriggersMappings();
				}
			}
		}

		public AdaptiveTriggerSettings AdaptiveRightTriggerSettings
		{
			get
			{
				if (this._adaptiveRightTriggerSettings == null && this.IsMainCollection)
				{
					this._adaptiveRightTriggerSettings = new AdaptiveTriggersSettings().CreateAdaptiveTriggerSettingsDefault();
				}
				return this._adaptiveRightTriggerSettings;
			}
			set
			{
				if (value != this._adaptiveRightTriggerSettings)
				{
					this.UnriseAdaptiveTriggerSettingsEventHandler();
					this.SetProperty<AdaptiveTriggerSettings>(ref this._adaptiveRightTriggerSettings, value, "AdaptiveRightTriggerSettings");
					this.RiseAdaptiveTriggerSettingsEventHandler();
					this.UpdateAdapterTriggersMappings();
				}
			}
		}

		public int MouseDirectionalGroupMacroCounter
		{
			get
			{
				return this._mouseDirectionalGroupMacroCounter;
			}
			set
			{
				this.SetProperty<int>(ref this._mouseDirectionalGroupMacroCounter, value, "MouseDirectionalGroupMacroCounter");
			}
		}

		public int LeftStickDirectionalGroupMacroCounter
		{
			get
			{
				return this._leftStickDirectionalGroupMacroCounter;
			}
			set
			{
				this.SetProperty<int>(ref this._leftStickDirectionalGroupMacroCounter, value, "LeftStickDirectionalGroupMacroCounter");
			}
		}

		public int RightStickDirectionalGroupMacroCounter
		{
			get
			{
				return this._rightStickDirectionalGroupMacroCounter;
			}
			set
			{
				this.SetProperty<int>(ref this._rightStickDirectionalGroupMacroCounter, value, "RightStickDirectionalGroupMacroCounter");
			}
		}

		public int GyroTiltDirectionalGroupMacroCounter
		{
			get
			{
				return this._gyroTiltDirectionalGroupMacroCounter;
			}
			set
			{
				this.SetProperty<int>(ref this._gyroTiltDirectionalGroupMacroCounter, value, "GyroTiltDirectionalGroupMacroCounter");
			}
		}

		public int LeftTouchpadDirectionalGroupMacroCounter
		{
			get
			{
				return this._leftTouchpadDirectionalGroupMacroCounter;
			}
			set
			{
				this.SetProperty<int>(ref this._leftTouchpadDirectionalGroupMacroCounter, value, "LeftTouchpadDirectionalGroupMacroCounter");
			}
		}

		public int RightTouchpadDirectionalGroupMacroCounter
		{
			get
			{
				return this._rightTouchpadDirectionalGroupMacroCounter;
			}
			set
			{
				this.SetProperty<int>(ref this._rightTouchpadDirectionalGroupMacroCounter, value, "RightTouchpadDirectionalGroupMacroCounter");
			}
		}

		public DPADDirectionalGroup DPADDirectionalGroup
		{
			get
			{
				DPADDirectionalGroup dpaddirectionalGroup;
				if ((dpaddirectionalGroup = this._DPADDirectionalGroup) == null)
				{
					dpaddirectionalGroup = (this._DPADDirectionalGroup = new DPADDirectionalGroup(this));
				}
				return dpaddirectionalGroup;
			}
		}

		public GyroTiltDirectionalGroup GyroTiltDirectionalGroup
		{
			get
			{
				GyroTiltDirectionalGroup gyroTiltDirectionalGroup;
				if ((gyroTiltDirectionalGroup = this._gyroTiltDirectionalGroup) == null)
				{
					gyroTiltDirectionalGroup = (this._gyroTiltDirectionalGroup = new GyroTiltDirectionalGroup(this));
				}
				return gyroTiltDirectionalGroup;
			}
		}

		public LeftStickDirectionalGroup LeftStickDirectionalGroup
		{
			get
			{
				LeftStickDirectionalGroup leftStickDirectionalGroup;
				if ((leftStickDirectionalGroup = this._leftStickDirectionalGroup) == null)
				{
					leftStickDirectionalGroup = (this._leftStickDirectionalGroup = new LeftStickDirectionalGroup(this));
				}
				return leftStickDirectionalGroup;
			}
		}

		public RightStickDirectionalGroup RightStickDirectionalGroup
		{
			get
			{
				RightStickDirectionalGroup rightStickDirectionalGroup;
				if ((rightStickDirectionalGroup = this._rightStickDirectionalGroup) == null)
				{
					rightStickDirectionalGroup = (this._rightStickDirectionalGroup = new RightStickDirectionalGroup(this));
				}
				return rightStickDirectionalGroup;
			}
		}

		public AdditionalStickDirectionalGroup AdditionalStickDirectionalGroup
		{
			get
			{
				AdditionalStickDirectionalGroup additionalStickDirectionalGroup;
				if ((additionalStickDirectionalGroup = this._additionalStickDirectionalGroup) == null)
				{
					additionalStickDirectionalGroup = (this._additionalStickDirectionalGroup = new AdditionalStickDirectionalGroup(this));
				}
				return additionalStickDirectionalGroup;
			}
		}

		public MouseDirectionalGroup MouseDirectionalGroup
		{
			get
			{
				MouseDirectionalGroup mouseDirectionalGroup;
				if ((mouseDirectionalGroup = this._mouseDirectionalGroup) == null)
				{
					mouseDirectionalGroup = (this._mouseDirectionalGroup = new MouseDirectionalGroup(this));
				}
				return mouseDirectionalGroup;
			}
		}

		public Touchpad1DirectionalGroup Touchpad1DirectionalGroup
		{
			get
			{
				Touchpad1DirectionalGroup touchpad1DirectionalGroup;
				if ((touchpad1DirectionalGroup = this._touchpad1DirectionalGroup) == null)
				{
					touchpad1DirectionalGroup = (this._touchpad1DirectionalGroup = new Touchpad1DirectionalGroup(this));
				}
				return touchpad1DirectionalGroup;
			}
		}

		public Touchpad2DirectionalGroup Touchpad2DirectionalGroup
		{
			get
			{
				Touchpad2DirectionalGroup touchpad2DirectionalGroup;
				if ((touchpad2DirectionalGroup = this._touchpad2DirectionalGroup) == null)
				{
					touchpad2DirectionalGroup = (this._touchpad2DirectionalGroup = new Touchpad2DirectionalGroup(this));
				}
				return touchpad2DirectionalGroup;
			}
		}

		public BaseTouchpadDirectionalGroup CurrentTouchpadDirectionalGroup
		{
			get
			{
				if (this.CurrentButtonMapping == null)
				{
					return null;
				}
				GamepadButton key = this.CurrentButtonMapping.Value.Key;
				if (GamepadButtonExtensions.IsPhysicalTrackPad1Direction(key))
				{
					return this.Touchpad1DirectionalGroup;
				}
				if (GamepadButtonExtensions.IsPhysicalTrackPad2Direction(key))
				{
					return this.Touchpad2DirectionalGroup;
				}
				return null;
			}
		}

		public BaseDirectionalGroup CurrentBoundGroup
		{
			get
			{
				if (this.CurrentButtonMapping == null)
				{
					return null;
				}
				GamepadButton key = this.CurrentButtonMapping.Value.Key;
				if (this._currentBoundGroup == null)
				{
					if (GamepadButtonExtensions.IsDPAD(key))
					{
						this._currentBoundGroup = this.DPADDirectionalGroup;
					}
					if (GamepadButtonExtensions.IsGyroTilt(key))
					{
						this._currentBoundGroup = this.GyroTiltDirectionalGroup;
					}
					else if (GamepadButtonExtensions.IsLeftStick(key))
					{
						this._currentBoundGroup = this.LeftStickDirectionalGroup;
					}
					else if (GamepadButtonExtensions.IsRightStick(key))
					{
						this._currentBoundGroup = this.RightStickDirectionalGroup;
					}
					else if (GamepadButtonExtensions.IsAdditionalStick(key))
					{
						this._currentBoundGroup = this.AdditionalStickDirectionalGroup;
					}
					else if (GamepadButtonExtensions.IsMouseStick(key))
					{
						this._currentBoundGroup = this.MouseDirectionalGroup;
					}
					else if (GamepadButtonExtensions.IsPhysicalTrackPad1Direction(key))
					{
						this._currentBoundGroup = this.Touchpad1DirectionalGroup;
					}
					else if (GamepadButtonExtensions.IsPhysicalTrackPad2Direction(key))
					{
						this._currentBoundGroup = this.Touchpad2DirectionalGroup;
					}
				}
				return this._currentBoundGroup;
			}
		}

		public BaseDirectionalAnalogGroup CurrenDirectionalAnalogGroup
		{
			get
			{
				return this.CurrentBoundGroup as BaseDirectionalAnalogGroup;
			}
		}

		public bool IsTrackballFrictionAllowed
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				if (this.CurrentBoundGroup == null || (this.CurrentControllerType == null || !ControllerTypeExtensions.IsTrackballFrictionAvailiable(controllerTypeEnum.GetValueOrDefault())))
				{
					return false;
				}
				if ((this.IsCurrentBoundToAnyInvertedMouse || this.IsCurrentBoundToLeftVirtualStick || this.IsCurrentBoundToRightVirtualStick) && (this.CurrentBoundGroup is Touchpad1DirectionalGroup || this.CurrentBoundGroup is Touchpad2DirectionalGroup))
				{
					return true;
				}
				if (!(this.CurrentBoundGroup is Touchpad2DirectionalGroup) || this.CurrentBoundGroup.IsAnyDirectionVirtualMappingPresent || this.CurrentBoundGroup.IsUnmapped)
				{
					return false;
				}
				SubConfigData subConfigData = this.SubConfigData;
				if (subConfigData == null)
				{
					return false;
				}
				ConfigData configData = subConfigData.ConfigData;
				bool? flag = ((configData != null) ? new bool?(configData.IsVirtualGamepad) : null);
				bool flag2 = true;
				return (flag.GetValueOrDefault() == flag2) & (flag != null);
			}
		}

		public bool IsCurrentBoundToWASD
		{
			get
			{
				return this.CurrentBoundGroup != null && this.CurrentBoundGroup.IsBoundToWASD;
			}
		}

		public bool IsCurrentBoundToFlickStick
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				return this.CurrentBoundGroup != null && this.CurrentBoundGroup.IsBoundToFlickStick && (this.CurrentBoundGroup is LeftStickDirectionalGroup || this.CurrentBoundGroup is RightStickDirectionalGroup || this.CurrentBoundGroup is AdditionalStickDirectionalGroup || this.CurrentBoundGroup is Touchpad2DirectionalGroup || (this.CurrentBoundGroup is Touchpad1DirectionalGroup && this.CurrentControllerType != null && ControllerTypeExtensions.IsAnySteam(controllerTypeEnum.GetValueOrDefault())));
			}
		}

		public bool IsCurrentBoundToOverlayMenuDirections
		{
			get
			{
				return this.CurrentBoundGroup != null && this.CurrentBoundGroup.IsBoundToOverlayMenuDirections;
			}
		}

		public bool IsCurrentBoundToArrows
		{
			get
			{
				return this.CurrentBoundGroup != null && this.CurrentBoundGroup.IsBoundToArrows;
			}
		}

		public bool IsCurrentBoundToMouse
		{
			get
			{
				return this.CurrentBoundGroup != null && this.CurrentBoundGroup.IsBoundToMouse;
			}
		}

		public bool IsCurrentHasMouseDirection
		{
			get
			{
				return this.CurrentBoundGroup != null && this.CurrentBoundGroup.IsHasMouseDirection;
			}
		}

		public bool IsCurrentBoundToDS4Touchpad
		{
			get
			{
				return this.CurrentBoundGroup != null && this.CurrentBoundGroup.IsBoundToDS4Touchpad;
			}
		}

		public bool IsCurrentBoundToAnyInvertedMouse
		{
			get
			{
				return this.CurrentBoundGroup != null && this.CurrentBoundGroup.IsBoundToAnyInvertedMouse;
			}
		}

		public bool IsCurrentBoundToLeftVirtualStick
		{
			get
			{
				return this.CurrentBoundGroup != null && this.CurrentBoundGroup.IsBoundToVirtualLeftStick;
			}
		}

		public bool IsCurrentBoundToRightVirtualStick
		{
			get
			{
				return this.CurrentBoundGroup != null && this.CurrentBoundGroup.IsBoundToVirtualRightStick;
			}
		}

		public bool IsCurrentBoundToVirtualDPAD
		{
			get
			{
				return this.CurrentBoundGroup != null && this.CurrentBoundGroup.IsBoundToVirtualDPAD;
			}
		}

		public bool IsCurrentBoundToKeyboard
		{
			get
			{
				return this.CurrentBoundGroup != null && this.CurrentBoundGroup.IsBoundToKeyboard;
			}
		}

		public bool IsCurrentUnmapped
		{
			get
			{
				if (this.CurrentBoundGroup != null)
				{
					BaseDirectionalAnalogGroup baseDirectionalAnalogGroup = this.CurrentBoundGroup as BaseDirectionalAnalogGroup;
					if (baseDirectionalAnalogGroup == null || !baseDirectionalAnalogGroup.IsDigitalMode)
					{
						return this.CurrentBoundGroup.IsUnmapped;
					}
				}
				if (this.CurrentXBBinding == null)
				{
					return false;
				}
				XBBinding xbbinding = this.CurrentXBBinding;
				if (GamepadButtonExtensions.IsAnalogZoneButton(xbbinding.ControllerButton.GamepadButton))
				{
					xbbinding = base[XBUtils.GetButtonByAnalogZoneButton(xbbinding.ControllerButton.GamepadButton)];
				}
				return GamepadButtonDescription.Unmapped.Equals(xbbinding.RemapedTo);
			}
		}

		public void RefreshGroupBoundProperties()
		{
			this.OnPropertyChanged("IsCurrentBoundToWASD");
			this.OnPropertyChanged("IsCurrentBoundToOverlayMenuDirections");
			this.OnPropertyChanged("IsCurrentBoundToKeyboard");
			this.OnPropertyChanged("IsCurrentBoundToFlickStick");
			this.OnPropertyChanged("IsCurrentBoundToMouse");
			this.OnPropertyChanged("IsCurrentHasMouseDirection");
			this.OnPropertyChanged("IsCurrentBoundToDS4Touchpad");
			this.OnPropertyChanged("IsCurrentBoundToAnyInvertedMouse");
			this.OnPropertyChanged("IsCurrentBoundToLeftVirtualStick");
			this.OnPropertyChanged("IsCurrentBoundToRightVirtualStick");
			this.OnPropertyChanged("IsCurrentBoundToVirtualDPAD");
			this.OnPropertyChanged("IsCurrentUnmapped");
			this.OnPropertyChanged("IsTrackballFrictionAllowed");
		}

		public DelegateCommand BindCurrentToOverlayMenuDirectionsCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._BindCurrentToOverlayMenuDirections) == null)
				{
					delegateCommand = (this._BindCurrentToOverlayMenuDirections = new DelegateCommand(new Action(this.BindCurrentToOverlayMenuDirections), new Func<bool>(this.BindCurrentToOverlayMenuDirectionsCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void BindCurrentToOverlayMenuDirections()
		{
			if (!this.CurrentBoundGroup.IsBoundToOverlayMenuDirections)
			{
				this.CurrentBoundGroup.Unbind();
				this.CurrentBoundGroup.BindToOverlayMenuDirections();
			}
			else
			{
				this.CurrentBoundGroup.Unbind();
			}
			this.RefreshGroupBoundProperties();
		}

		private bool BindCurrentToOverlayMenuDirectionsCanExecute()
		{
			return this.CurrentButtonMapping != null;
		}

		public DelegateCommand BindCurrentToWASDCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._BindCurrentToWASD) == null)
				{
					delegateCommand = (this._BindCurrentToWASD = new DelegateCommand(new Action(this.BindCurrentToWASD), new Func<bool>(this.BindCurrentToWASDCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void BindCurrentToWASD()
		{
			if (!this.CurrentBoundGroup.IsBoundToWASD)
			{
				this.CurrentBoundGroup.Unbind();
				this.CurrentBoundGroup.BindToWASD();
			}
			else
			{
				this.CurrentBoundGroup.Unbind();
			}
			this.RefreshGroupBoundProperties();
		}

		private bool BindCurrentToWASDCanExecute()
		{
			return this.CurrentButtonMapping != null;
		}

		public DelegateCommand BindCurrentToFlickStickCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._BindCurrentToFlickStick) == null)
				{
					delegateCommand = (this._BindCurrentToFlickStick = new DelegateCommand(new Action(this.BindCurrentToFlickStick), new Func<bool>(this.BindCurrentToFlickStickCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void BindCurrentToFlickStick()
		{
			if (!this.CurrentBoundGroup.IsBoundToFlickStick)
			{
				this.CurrentBoundGroup.Unbind();
				this.CurrentBoundGroup.BindToFlickStick();
			}
			else
			{
				this.CurrentBoundGroup.Unbind();
			}
			this.RefreshGroupBoundProperties();
		}

		private bool BindCurrentToFlickStickCanExecute()
		{
			return this.CurrentButtonMapping != null;
		}

		public DelegateCommand BindCurrentToArrowsCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._BindCurrentToArrows) == null)
				{
					delegateCommand = (this._BindCurrentToArrows = new DelegateCommand(new Action(this.BindCurrentToArrows), new Func<bool>(this.BindCurrentToArrowsCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void BindCurrentToArrows()
		{
			if (!this.CurrentBoundGroup.IsBoundToArrows)
			{
				this.CurrentBoundGroup.Unbind();
				this.CurrentBoundGroup.BindToArrows();
			}
			else
			{
				this.CurrentBoundGroup.Unbind();
			}
			this.RefreshGroupBoundProperties();
		}

		private bool BindCurrentToArrowsCanExecute()
		{
			return this.CurrentButtonMapping != null;
		}

		public DelegateCommand BindCurrentToMouseCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._BindCurrentToMouse) == null)
				{
					delegateCommand = (this._BindCurrentToMouse = new DelegateCommand(new Action(this.BindCurrentToMouse), new Func<bool>(this.BindCurrentToMouseCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void BindCurrentToMouse()
		{
			if (!this.CurrentBoundGroup.IsBoundToAnyInvertedMouse)
			{
				this.CurrentBoundGroup.Unbind();
				this.CurrentBoundGroup.BindToMouse();
			}
			else
			{
				this.CurrentBoundGroup.Unbind();
			}
			this.RefreshGroupBoundProperties();
		}

		private bool BindCurrentToMouseCanExecute()
		{
			return this.CurrentButtonMapping != null;
		}

		public DelegateCommand BindCurrentToLeftVirtualStickCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._BindCurrentToLeftVirtualStick) == null)
				{
					delegateCommand = (this._BindCurrentToLeftVirtualStick = new DelegateCommand(new Action(this.BindCurrentToLeftVirtualStick), new Func<bool>(this.BindCurrentToLeftVirtualStickCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void BindCurrentToLeftVirtualStick()
		{
			if (!this.CurrentBoundGroup.IsBoundToVirtualLeftStick)
			{
				this.CurrentBoundGroup.Unbind();
				this.CurrentBoundGroup.BindToLeftVirtualStick();
			}
			else
			{
				this.CurrentBoundGroup.Unbind();
			}
			this.RefreshGroupBoundProperties();
		}

		private bool BindCurrentToLeftVirtualStickCanExecute()
		{
			return this.CurrentButtonMapping != null;
		}

		public DelegateCommand BindCurrentToRightVirtualStickCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._BindCurrentToRightVirtualStick) == null)
				{
					delegateCommand = (this._BindCurrentToRightVirtualStick = new DelegateCommand(new Action(this.BindCurrentToRightVirtualStick), new Func<bool>(this.BindCurrentToRightVirtualStickCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void BindCurrentToRightVirtualStick()
		{
			if (!this.CurrentBoundGroup.IsBoundToVirtualRightStick)
			{
				this.CurrentBoundGroup.Unbind();
				this.CurrentBoundGroup.BindToRightVirtualStick();
			}
			else
			{
				this.CurrentBoundGroup.Unbind();
			}
			this.RefreshGroupBoundProperties();
		}

		private bool BindCurrentToRightVirtualStickCanExecute()
		{
			return this.CurrentButtonMapping != null;
		}

		public DelegateCommand BindCurrentToVirtualDPADCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._BindCurrentToVirtualDPAD) == null)
				{
					delegateCommand = (this._BindCurrentToVirtualDPAD = new DelegateCommand(new Action(this.BindCurrentToVirtualDPAD), new Func<bool>(this.BindCurrentToVirtualDPADCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void BindCurrentToVirtualDPAD()
		{
			if (!this.CurrentBoundGroup.IsBoundToVirtualDPAD)
			{
				this.CurrentBoundGroup.Unbind();
				this.CurrentBoundGroup.BindToVirtualDPAD();
			}
			else
			{
				this.CurrentBoundGroup.Unbind();
			}
			this.RefreshGroupBoundProperties();
		}

		private bool BindCurrentToVirtualDPADCanExecute()
		{
			return this.CurrentButtonMapping != null;
		}

		public DelegateCommand BindCurrentToDS4GamepadCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._BindCurrentToDS4Gamepad) == null)
				{
					delegateCommand = (this._BindCurrentToDS4Gamepad = new DelegateCommand(new Action(this.BindCurrentToDS4Gamepad), new Func<bool>(this.BindCurrentToDS4GamepadCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void BindCurrentToDS4Gamepad()
		{
			if (!this.CurrentBoundGroup.IsBoundToDS4Touchpad)
			{
				this.CurrentBoundGroup.Unbind();
				this.CurrentBoundGroup.BindToTouchpad();
			}
			else
			{
				this.CurrentBoundGroup.Unbind();
			}
			this.RefreshGroupBoundProperties();
		}

		private bool BindCurrentToDS4GamepadCanExecute()
		{
			return this.CurrentButtonMapping != null;
		}

		public DelegateCommand UnmapCurrentCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._UnmapCurrent) == null)
				{
					delegateCommand = (this._UnmapCurrent = new DelegateCommand(new Action(this.UnmapCurrent), new Func<bool>(this.UnmapCurrentCanExecute)));
				}
				return delegateCommand;
			}
		}

		protected void UnmapCurrent()
		{
			if (this.CurrentBoundGroup == null)
			{
				if (this.CurrentXBBinding == null)
				{
					return;
				}
				XBBinding xbbinding = this.CurrentXBBinding;
				if (GamepadButtonExtensions.IsAnalogZoneButton(xbbinding.GamepadButton))
				{
					xbbinding = this.GetXBBindingByGamepadButton(XBUtils.GetButtonByAnalogZoneButton(xbbinding.GamepadButton));
				}
				if (GamepadButtonDescription.Unmapped.Equals(xbbinding.RemapedTo))
				{
					xbbinding.RevertRemap();
				}
				else
				{
					xbbinding.RemapedTo = GamepadButtonDescription.Unmapped;
				}
			}
			else
			{
				this.CurrentBoundGroup.ToggleUnmap();
			}
			this.SubConfigData.ConfigData.CheckFeatures();
			this.RefreshGroupBoundProperties();
		}

		protected virtual bool UnmapCurrentCanExecute()
		{
			return true;
		}

		public bool IsLabelModeView
		{
			get
			{
				return this._isLabelModeView;
			}
			set
			{
				this._isShowAllView = false;
				this._isLabelModeView = false;
				this._isShowMappingsView = false;
				if (this.SetProperty<bool>(ref this._isLabelModeView, value, "IsLabelModeView"))
				{
					this.OnPropertyChanged("IsShowAllView");
					this.OnPropertyChanged("IsShowMappingsView");
					this.OnPropertyChanged("IsLabelModeView");
					this.SendIsShowMappingsViewChanged();
				}
			}
		}

		public void SendIsShowMappingsViewChanged()
		{
			base.ForEachValue(delegate(XBBinding xbBinding)
			{
				xbBinding.ActivatorXBBindingDictionary.ForEachValue(delegate(ActivatorXBBinding item)
				{
					item.OnShowMappingsViewChanged();
				});
			});
			this.ControllerBindings.ForEach(delegate(ControllerBinding controllerBinding)
			{
				controllerBinding.XBBinding.ActivatorXBBindingDictionary.ForEachValue(delegate(ActivatorXBBinding item)
				{
					item.OnShowMappingsViewChanged();
				});
			});
			MaskItemCollection maskBindingCollection = this.MaskBindingCollection;
			if (maskBindingCollection == null)
			{
				return;
			}
			maskBindingCollection.ForEach(delegate(MaskItem maskItem)
			{
				maskItem.XBBinding.ActivatorXBBindingDictionary.ForEachValue(delegate(ActivatorXBBinding item)
				{
					item.OnShowMappingsViewChanged();
				});
			});
		}

		public bool IsShowMappingsView
		{
			get
			{
				return this._isShowMappingsView;
			}
			set
			{
				this._isShowAllView = false;
				this._isLabelModeView = false;
				this._isShowMappingsView = false;
				if (this.SetProperty<bool>(ref this._isShowMappingsView, value, "IsShowMappingsView"))
				{
					this.OnPropertyChanged("IsShowAllView");
					this.OnPropertyChanged("IsShowMappingsView");
					this.OnPropertyChanged("IsLabelModeView");
					this.SendIsShowMappingsViewChanged();
				}
			}
		}

		public bool IsShowAllView
		{
			get
			{
				return this._isShowAllView;
			}
			set
			{
				if (value)
				{
					this._isShowMappingsView = true;
					this._isLabelModeView = true;
				}
				if (this.SetProperty<bool>(ref this._isShowAllView, value, "IsShowAllView"))
				{
					this.OnPropertyChanged("IsShowAllView");
					this.OnPropertyChanged("IsShowMappingsView");
					this.OnPropertyChanged("IsLabelModeView");
					this.SendIsShowMappingsViewChanged();
				}
			}
		}

		public bool IsExpandActivatorsView
		{
			get
			{
				return this._isExpandActivatorsView;
			}
			set
			{
				this.SetProperty<bool>(ref this._isExpandActivatorsView, value, "IsExpandActivatorsView");
			}
		}

		public bool IsMaskModeView
		{
			get
			{
				return this._isMaskModeView;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isMaskModeView, value, "IsMaskModeView") && value)
				{
					this.IsVirtualStickSettingsModeView = false;
					this.IsOverlayMenuModeView = false;
				}
			}
		}

		public bool IsVirtualStickSettingsModeView
		{
			get
			{
				return this._isVirtualStickSettingsModeView;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isVirtualStickSettingsModeView, value, "IsVirtualStickSettingsModeView") && value)
				{
					this.IsMaskModeView = false;
					this.IsOverlayMenuModeView = false;
				}
			}
		}

		public bool IsOverlayMenuModeView
		{
			get
			{
				return this._isOverlayMenuModeView;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isOverlayMenuModeView, value, "IsOverlayMenuModeView"))
				{
					if (value)
					{
						this.IsMaskModeView = false;
						this.IsVirtualStickSettingsModeView = false;
					}
					this.OnPropertyChanged("IsShowSubconfig");
				}
			}
		}

		public DelegateCommand<ActivatorType?> SwitchCurrentActivatorCommand
		{
			get
			{
				DelegateCommand<ActivatorType?> delegateCommand;
				if ((delegateCommand = this._switchCurrentActivator) == null)
				{
					delegateCommand = (this._switchCurrentActivator = new DelegateCommand<ActivatorType?>(new Action<ActivatorType?>(this.SwitchCurrentActivator)));
				}
				return delegateCommand;
			}
		}

		private void SwitchCurrentActivator(ActivatorType? activatorType)
		{
			if (this.CurrentButtonMappingValue == null || activatorType == null)
			{
				return;
			}
			this.CurrentButtonMappingValue.SwitchCurrentActivator(activatorType.Value);
		}

		public DelegateCommand<Stick?> InvertStickXCommand
		{
			get
			{
				DelegateCommand<Stick?> delegateCommand;
				if ((delegateCommand = this._invertStickX) == null)
				{
					delegateCommand = (this._invertStickX = new DelegateCommand<Stick?>(new Action<Stick?>(this.InvertStickXPrivate), new Func<Stick?, bool>(this.InvertStickXCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void InvertStickXPrivate(Stick? stick)
		{
			if (stick == null)
			{
				return;
			}
			this.InvertStickX(stick.Value);
		}

		private bool InvertStickXCanExecute(Stick? stick)
		{
			return true;
		}

		public DelegateCommand<Stick?> InvertStickYCommand
		{
			get
			{
				DelegateCommand<Stick?> delegateCommand;
				if ((delegateCommand = this._invertStickY) == null)
				{
					delegateCommand = (this._invertStickY = new DelegateCommand<Stick?>(new Action<Stick?>(this.InvertStickYPrivate), new Func<Stick?, bool>(this.InvertStickYCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void InvertStickYPrivate(Stick? stick)
		{
			if (stick == null)
			{
				return;
			}
			this.InvertStickY(stick.Value);
		}

		private bool InvertStickYCanExecute(Stick? stick)
		{
			return true;
		}

		public DelegateCommand SwapSticksCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._swapSticks) == null)
				{
					delegateCommand = (this._swapSticks = new DelegateCommand(delegate
					{
						this.SwapSticks();
					}));
				}
				return delegateCommand;
			}
		}

		public DelegateCommand<Stick?> ResetStickToDefaultCommand
		{
			get
			{
				DelegateCommand<Stick?> delegateCommand;
				if ((delegateCommand = this._resetStickToDefault) == null)
				{
					delegateCommand = (this._resetStickToDefault = new DelegateCommand<Stick?>(new Action<Stick?>(this.ResetStickToDefaultPrivate)));
				}
				return delegateCommand;
			}
		}

		private async void ResetStickToDefaultPrivate(Stick? stick)
		{
			if (stick != null)
			{
				this.SetEnablePropertyChanged(false, true);
				if (this.IsLeftStickSwapped || this.IsRightStickSwapped)
				{
					TaskAwaiter<bool> taskAwaiter = this.ResetStickToDefault(0, true).GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						TaskAwaiter<bool> taskAwaiter2;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<bool>);
					}
					if (taskAwaiter.GetResult())
					{
						await this.ResetStickToDefault(1, false);
					}
				}
				else
				{
					await this.ResetStickToDefault(stick.Value, true);
				}
				this.SetEnablePropertyChanged(true, false);
				Stick? stick2 = stick;
				Stick stick3 = 4;
				ControllerTypeEnum? controllerTypeEnum;
				if (((stick2.GetValueOrDefault() == stick3) & (stick2 != null)) && (this.CurrentControllerType != null && ControllerTypeExtensions.IsGamepadWithSonyTouchpad(controllerTypeEnum.GetValueOrDefault())))
				{
					App.EventAggregator.GetEvent<RequestBindingFrameBack>().Publish(null);
				}
				this.SubConfigData.ConfigData.CheckFeatures();
			}
		}

		public DelegateCommand AddControllerBindingCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._addControllerBinding) == null)
				{
					delegateCommand = (this._addControllerBinding = new DelegateCommand(new Action(this.AddControllerBinding)));
				}
				return delegateCommand;
			}
		}

		private void AddControllerBinding()
		{
			ControllerBinding controllerBinding = new ControllerBinding(new XBBinding(this), this.ControllerBindings);
			this.ControllerBindings.Add(controllerBinding);
			if (this.ControllerBindings.ControllerButtonTag != null)
			{
				this.ControllerBindings.ReFilterItems();
			}
			this.ControllerBindings.CurrentEditItem = controllerBinding;
		}

		public DelegateCommand UnmapWholeGamepadCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._UnmapWholeGamepad) == null)
				{
					delegateCommand = (this._UnmapWholeGamepad = new DelegateCommand(new Action(this.UnmapWholeGamepad)));
				}
				return delegateCommand;
			}
		}

		private void UnmapWholeGamepad()
		{
			this.SubConfigData.MainXBBindingCollection.SetEnablePropertyChanged(false, false);
			IKeyBindingService keyBindingService = App.KeyBindingService;
			BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
			Collection<GamepadButtonDescription> collection = keyBindingService.GenerateRemapButtonDescriptionsForController((currentGamepad != null) ? currentGamepad.CurrentController : null, true, null);
			bool isWholeGamepadUnmapped = this.IsWholeGamepadUnmapped;
			foreach (GamepadButtonDescription gamepadButtonDescription in collection)
			{
				XBBinding xbbindingByGamepadButton = this.GetXBBindingByGamepadButton(gamepadButtonDescription);
				if (xbbindingByGamepadButton != null)
				{
					if (isWholeGamepadUnmapped)
					{
						xbbindingByGamepadButton.RevertRemap();
						xbbindingByGamepadButton.ActivatorXBBindingDictionary.ForEachValue(delegate(ActivatorXBBinding activatorXBBinding)
						{
							if (activatorXBBinding.JumpToShift != -1)
							{
								activatorXBBinding.ShiftXBBinding.RevertRemap();
							}
						});
					}
					else
					{
						xbbindingByGamepadButton.RemapedTo = GamepadButtonDescription.Unmapped;
						xbbindingByGamepadButton.ActivatorXBBindingDictionary.ForEachValue(delegate(ActivatorXBBinding activatorXBBinding)
						{
							if (activatorXBBinding.JumpToShift != -1)
							{
								activatorXBBinding.ShiftXBBinding.RemapedTo = GamepadButtonDescription.Unmapped;
							}
						});
					}
				}
			}
			this.SubConfigData.MainXBBindingCollection.SetEnablePropertyChanged(true, false);
			this.SubConfigData.MainXBBindingCollection.IsChanged = true;
			this.SubConfigData.ConfigData.CheckFeatures();
			this.OnPropertyChanged("IsWholeGamepadUnmapped");
			App.EventAggregator.GetEvent<CurrentButtonMappingChanged>().Publish(null);
		}

		public bool IsWholeGamepadUnmapped
		{
			get
			{
				IKeyBindingService keyBindingService = App.KeyBindingService;
				BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
				foreach (GamepadButtonDescription gamepadButtonDescription in keyBindingService.GenerateRemapButtonDescriptionsForController((currentGamepad != null) ? currentGamepad.CurrentController : null, true, null))
				{
					XBBinding xbbindingByGamepadButton = this.GetXBBindingByGamepadButton(gamepadButtonDescription);
					if (xbbindingByGamepadButton != null && !xbbindingByGamepadButton.IsUnmapped)
					{
						return false;
					}
				}
				return true;
			}
		}

		public MaskItemCollection MaskBindingCollection
		{
			get
			{
				SubConfigData subConfigData = this.SubConfigData;
				if (subConfigData == null)
				{
					return null;
				}
				ConfigData configData = subConfigData.ConfigData;
				if (configData == null)
				{
					return null;
				}
				ConfigData.AdditionalDataItem additionalDataItemByIndex = configData.GetAdditionalDataItemByIndex(this.ShiftIndex);
				if (additionalDataItemByIndex == null)
				{
					return null;
				}
				return additionalDataItemByIndex.MaskBindingCollection;
			}
		}

		public abstract SubConfigData SubConfigData { get; set; }

		public BaseXBBindingCollection(ControllerFamily controllerFamily)
		{
			base.CollectionItemPropertyChangedExtended += this.OnCollectionItemPropertyChangedExtended;
			this.ControllerBindings = new ControllerBindingsCollection(this, false);
			this.ControllerBindings.CollectionItemPropertyChangedExtended += delegate(object s, PropertyChangedExtendedEventArgs e)
			{
				this.OnCollectionItemPropertyChangedExtended(s, e);
			};
			this.FillDictionaryWithEmpyXBBindings(controllerFamily);
			this._processNames = new ObservableCollection<string>();
			this.IsChanged = false;
			base.AutoDispose = true;
			TranslationManager.Instance.LanguageChanged += delegate([Nullable(2)] object s, EventArgs e)
			{
				this.OnPropertyChanged("Description");
			};
		}

		protected void InitEventHandler()
		{
			this.Touchpad1DirectionalGroup.PropertyChanged += this.TouchpadDirectionalGroup_PropertyChanged;
			this.Touchpad2DirectionalGroup.PropertyChanged += this.TouchpadDirectionalGroup_PropertyChanged;
			this.AdditionalStickDirectionalGroup.PropertyChanged += this.AnalogStickDirectionalGroup_PropertyChanged;
			this.GyroTiltDirectionalGroup.PropertyChanged += this.AnalogStickDirectionalGroup_PropertyChanged;
		}

		public override void Dispose()
		{
			base.CollectionItemPropertyChangedExtended -= this.OnCollectionItemPropertyChangedExtended;
			this.CurrentButtonMappingChanged = null;
			this.IsChangedModifiedEvent = null;
			this._processNames.Clear();
			this.LeftStickDirectionalGroup.Dispose();
			this.RightStickDirectionalGroup.Dispose();
			this.AdditionalStickDirectionalGroup.Dispose();
			this.MouseDirectionalGroup.Dispose();
			this.ControllerBindings = null;
			this.SubConfigData = null;
			base.Dispose();
		}

		public bool IsLeftStickXInverted
		{
			get
			{
				return this.IsStickXInverted(0);
			}
		}

		public bool IsLeftStickYInverted
		{
			get
			{
				return this.IsStickYInverted(0);
			}
		}

		public bool IsRightStickXInverted
		{
			get
			{
				return this.IsStickXInverted(1);
			}
		}

		public bool IsRightStickYInverted
		{
			get
			{
				return this.IsStickYInverted(1);
			}
		}

		public bool IsAdditionalStickXInverted
		{
			get
			{
				return this.IsStickXInverted(6);
			}
		}

		public bool IsAdditionalStickYInverted
		{
			get
			{
				return this.IsStickYInverted(6);
			}
		}

		public bool IsMouseStickXInverted
		{
			get
			{
				return this.IsStickXInverted(2);
			}
		}

		public bool IsMouseStickYInverted
		{
			get
			{
				return this.IsStickYInverted(2);
			}
		}

		public bool IsGyroTiltStickXInverted
		{
			get
			{
				return this.IsStickXInverted(3);
			}
		}

		public bool IsGyroTiltStickYInverted
		{
			get
			{
				return this.IsStickYInverted(3);
			}
		}

		public bool IsTrackPad1StickXInverted
		{
			get
			{
				return this.IsStickXInverted(4);
			}
		}

		public bool IsTrackPad1StickYInverted
		{
			get
			{
				return this.IsStickYInverted(4);
			}
		}

		public bool IsTrackPad2StickXInverted
		{
			get
			{
				return this.IsStickXInverted(5);
			}
		}

		public bool IsTrackPad2StickYInverted
		{
			get
			{
				return this.IsStickYInverted(5);
			}
		}

		private bool IsStickXInverted(Stick stick)
		{
			if (stick == null)
			{
				return this.LeftStickDirectionalGroup.IsXInvert;
			}
			if (stick == 1)
			{
				return this.RightStickDirectionalGroup.IsXInvert;
			}
			if (stick == 6)
			{
				return this.AdditionalStickDirectionalGroup.IsXInvert;
			}
			if (stick == 2)
			{
				return this.MouseDirectionalGroup.IsXInvert;
			}
			if (stick == 3)
			{
				return this.GyroTiltDirectionalGroup.IsXInvert;
			}
			if (stick == 4)
			{
				return this.Touchpad1DirectionalGroup.IsXInvert;
			}
			if (stick == 5)
			{
				return this.Touchpad2DirectionalGroup.IsXInvert;
			}
			throw new Exception("Unspopprted stick enum");
		}

		private bool IsStickYInverted(Stick stick)
		{
			if (stick == null)
			{
				return this.LeftStickDirectionalGroup.IsYInvert;
			}
			if (stick == 1)
			{
				return this.RightStickDirectionalGroup.IsYInvert;
			}
			if (stick == 6)
			{
				return this.AdditionalStickDirectionalGroup.IsYInvert;
			}
			if (stick == 2)
			{
				return this.MouseDirectionalGroup.IsYInvert;
			}
			if (stick == 3)
			{
				return this.GyroTiltDirectionalGroup.IsYInvert;
			}
			if (stick == 4)
			{
				return this.Touchpad1DirectionalGroup.IsYInvert;
			}
			if (stick == 5)
			{
				return this.Touchpad2DirectionalGroup.IsYInvert;
			}
			throw new Exception("Unspopprted stick enum");
		}

		public virtual void InvertStickX(Stick stick)
		{
			if (stick == null)
			{
				this.LeftStickDirectionalGroup.InvertStickX(true, true);
				this.OnPropertyChanged("IsLeftStickXInverted");
				return;
			}
			if (stick == 1)
			{
				this.RightStickDirectionalGroup.InvertStickX(true, true);
				this.OnPropertyChanged("IsRightStickXInverted");
				return;
			}
			if (stick == 6)
			{
				this.AdditionalStickDirectionalGroup.InvertStickX(true, true);
				this.OnPropertyChanged("IsAdditionalStickXInverted");
				return;
			}
			if (stick == 6)
			{
				this.AdditionalStickDirectionalGroup.InvertStickX(true, true);
				this.OnPropertyChanged("IsRightStickXInverted");
				return;
			}
			if (stick == 2)
			{
				this.MouseDirectionalGroup.InvertStickX(true, false);
				this.OnPropertyChanged("IsMouseStickXInverted");
				return;
			}
			if (stick == 3)
			{
				this.GyroTiltDirectionalGroup.InvertStickX(true, false);
				this.OnPropertyChanged("IsGyroTiltStickXInverted");
				return;
			}
			if (stick == 4)
			{
				this.Touchpad1DirectionalGroup.InvertStickX(true, false);
				this.OnPropertyChanged("IsTrackPad1StickXInverted");
				return;
			}
			if (stick == 5)
			{
				this.Touchpad2DirectionalGroup.InvertStickX(true, false);
				this.OnPropertyChanged("IsTrackPad2StickXInverted");
			}
		}

		public virtual void InvertStickY(Stick stick)
		{
			if (stick == null)
			{
				this.LeftStickDirectionalGroup.InvertStickY(true, true);
				this.OnPropertyChanged("IsLeftStickYInverted");
				return;
			}
			if (stick == 1)
			{
				this.RightStickDirectionalGroup.InvertStickY(true, true);
				this.OnPropertyChanged("IsRightStickYInverted");
				return;
			}
			if (stick == 6)
			{
				this.AdditionalStickDirectionalGroup.InvertStickY(true, true);
				this.OnPropertyChanged("IsAdditionalStickYInverted");
				return;
			}
			if (stick == 2)
			{
				this.MouseDirectionalGroup.InvertStickY(true, false);
				this.OnPropertyChanged("IsMouseStickYInverted");
				return;
			}
			if (stick == 3)
			{
				this.GyroTiltDirectionalGroup.InvertStickY(true, false);
				this.OnPropertyChanged("IsGyroTiltStickYInverted");
				return;
			}
			if (stick == 4)
			{
				this.Touchpad1DirectionalGroup.InvertStickY(true, false);
				this.OnPropertyChanged("IsTrackPad1StickYInverted");
				return;
			}
			if (stick == 5)
			{
				this.Touchpad2DirectionalGroup.InvertStickY(true, false);
				this.OnPropertyChanged("IsTrackPad2StickYInverted");
			}
		}

		public bool IsLeftStickSwapped
		{
			get
			{
				return this.IsStickSwapped(0);
			}
		}

		public bool IsRightStickSwapped
		{
			get
			{
				return this.IsStickSwapped(1);
			}
		}

		private bool IsStickSwapped(Stick stick)
		{
			if (stick == null)
			{
				LeftStickDirectionalGroup leftStickDirectionalGroup = this.LeftStickDirectionalGroup;
				return base.ContainsKey(leftStickDirectionalGroup.LeftDirection) && (GamepadButtonExtensions.IsRightStickDirection(leftStickDirectionalGroup.LeftDirectionValue.RemapedTo.Button) && GamepadButtonExtensions.IsRightStickDirection(leftStickDirectionalGroup.UpDirectionValue.RemapedTo.Button) && GamepadButtonExtensions.IsRightStickDirection(leftStickDirectionalGroup.RightDirectionValue.RemapedTo.Button)) && GamepadButtonExtensions.IsRightStickDirection(leftStickDirectionalGroup.DownDirectionValue.RemapedTo.Button);
			}
			if (stick == 1)
			{
				RightStickDirectionalGroup rightStickDirectionalGroup = this.RightStickDirectionalGroup;
				return base.ContainsKey(rightStickDirectionalGroup.LeftDirection) && (GamepadButtonExtensions.IsLeftStickDirection(rightStickDirectionalGroup.LeftDirectionValue.RemapedTo.Button) && GamepadButtonExtensions.IsLeftStickDirection(rightStickDirectionalGroup.UpDirectionValue.RemapedTo.Button) && GamepadButtonExtensions.IsLeftStickDirection(rightStickDirectionalGroup.RightDirectionValue.RemapedTo.Button)) && GamepadButtonExtensions.IsLeftStickDirection(rightStickDirectionalGroup.DownDirectionValue.RemapedTo.Button);
			}
			return false;
		}

		public virtual void SwapSticks()
		{
			this.LeftStickDirectionalGroup.SwapStickWith(this.RightStickDirectionalGroup);
			this.OnPropertyChanged("IsLeftStickSwapped");
			this.OnPropertyChanged("IsRightStickSwapped");
		}

		public virtual Task<bool> ResetStickToDefault(Stick stick, bool askConfirmation = true)
		{
			BaseDirectionalAnalogGroup baseDirectionalAnalogGroup = null;
			if (stick == null)
			{
				baseDirectionalAnalogGroup = this.LeftStickDirectionalGroup;
			}
			else if (stick == 1)
			{
				baseDirectionalAnalogGroup = this.RightStickDirectionalGroup;
			}
			else if (stick == 6)
			{
				baseDirectionalAnalogGroup = this.AdditionalStickDirectionalGroup;
			}
			else if (stick == 2)
			{
				baseDirectionalAnalogGroup = this.MouseDirectionalGroup;
			}
			else if (stick == 3)
			{
				baseDirectionalAnalogGroup = this.GyroTiltDirectionalGroup;
			}
			else if (stick == 4)
			{
				baseDirectionalAnalogGroup = this.Touchpad1DirectionalGroup;
			}
			else if (stick == 5)
			{
				baseDirectionalAnalogGroup = this.Touchpad2DirectionalGroup;
			}
			if (baseDirectionalAnalogGroup == null)
			{
				return Task.FromResult<bool>(false);
			}
			if (!baseDirectionalAnalogGroup.ResetToDefault(askConfirmation))
			{
				return Task.FromResult<bool>(false);
			}
			baseDirectionalAnalogGroup.RevertRemapToDefault();
			if (stick == null)
			{
				this.OnPropertyChanged("IsLeftStickSwapped");
				this.OnPropertyChanged("IsLeftStickXInverted");
				this.OnPropertyChanged("IsLeftStickYInverted");
			}
			if (stick == 1)
			{
				this.OnPropertyChanged("IsRightStickSwapped");
				this.OnPropertyChanged("IsRightStickXInverted");
				this.OnPropertyChanged("IsRightStickYInverted");
			}
			if (stick == 6)
			{
				this.OnPropertyChanged("IsAdditionalStickXInverted");
				this.OnPropertyChanged("IsAdditionalStickYInverted");
			}
			if (stick == 2)
			{
				this.OnPropertyChanged("IsMouseStickXInverted");
				this.OnPropertyChanged("IsMouseStickYInverted");
				this.OnPropertyChanged("IsCurrentBoundToLeftVirtualStick");
				this.OnPropertyChanged("IsCurrentBoundToRightVirtualStick");
				this.OnPropertyChanged("CurrentXBBinding");
			}
			if (stick == 4)
			{
				this.OnPropertyChanged("IsTrackPad1StickXInverted");
				this.OnPropertyChanged("IsTrackPad1StickYInverted");
			}
			if (stick == 5)
			{
				this.OnPropertyChanged("IsTrackPad2StickXInverted");
				this.OnPropertyChanged("IsTrackPad2StickYInverted");
			}
			this.OnPropertyChanged("IsCurrentUnmapped");
			return Task.FromResult<bool>(true);
		}

		public bool CheckDuplicateButtonMapping(ActivatorXBBinding sender)
		{
			return true;
		}

		public bool HasMaskForButton(GamepadButton button)
		{
			MaskItemCollection maskBindingCollection = this.MaskBindingCollection;
			Func<MaskItemCondition, bool> <>9__1;
			return maskBindingCollection != null && maskBindingCollection.Any(delegate(MaskItem mi)
			{
				IEnumerable<MaskItemCondition> maskConditions = mi.MaskConditions;
				Func<MaskItemCondition, bool> func;
				if ((func = <>9__1) == null)
				{
					func = (<>9__1 = (MaskItemCondition mic) => mic.GamepadButton == button);
				}
				return maskConditions.Any(func);
			});
		}

		protected virtual void OnMaskCollectionChangedExtended(object s, NotifyCollectionChangedEventArgs e)
		{
		}

		private void AnalogStickDirectionalGroup_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsButtonMappingVisible")
			{
				this.OnPropertyChanged("IsAdvanceMappingSettingsAvailiableForCurrentBinding");
			}
		}

		private void TouchpadDirectionalGroup_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "TouchpadAnalogMode")
			{
				this.UpdateAdvanceMappingSettingsIcon();
			}
		}

		public void UpdateAdvanceMappingSettingsIcon()
		{
			if (this.IsAdvanceMappingSettingsAvailiableForCurrentBinding)
			{
				this.OnPropertyChanged("IsAdvanceMappingSettingsChanged");
			}
		}

		protected virtual void OnCollectionItemPropertyChangedExtended(object s, PropertyChangedExtendedEventArgs e)
		{
			if (!(e.PropertyName == "IsInheritedBinding") && !(e.PropertyName == "IsAdaptiveTriggersInherited") && !(e.PropertyName == "IsAdaptiveTriggers") && !(e.PropertyName == "CurrentActivatorXBBinding") && !(e.PropertyName == "MacroSequenceAnnotation"))
			{
				if (e.PropertyName == "IsMacroSequenceChanged")
				{
					bool? flag = e.NewValue as bool?;
					bool flag2 = false;
					if ((flag.GetValueOrDefault() == flag2) & (flag != null))
					{
						return;
					}
				}
				if (e.PropertyName == "MappedKey")
				{
					this.OnPropertyChanged("CurrentBoundGroup");
				}
				if (e.PropertyName == "CurrentEditItem" || e.PropertyName == "CurrentEditItem")
				{
					this.OnPropertyChanged("CurrentControllerButton");
					this.OnPropertyChanged("CurrentXBBinding");
					this.OnPropertyChanged("IsCurrentUnmapped");
					return;
				}
				ActivatorXBBinding activatorXBBinding = s as ActivatorXBBinding;
				if (activatorXBBinding != null && e.PropertyName == "MappedKey")
				{
					if (!this.CheckDuplicateButtonMapping(activatorXBBinding))
					{
						activatorXBBinding.ResetChanges("MappedKey");
					}
					this.OnPropertyChanged("IsCurrentButtonMappingDoNotInherit");
				}
				this.IsChanged = true;
				return;
			}
		}

		public virtual void AttachMaskCollectionPropertyChanged()
		{
			if (this.MaskBindingCollection != null)
			{
				this.MaskBindingCollection.CollectionItemPropertyChangedExtended += delegate(object s, PropertyChangedExtendedEventArgs e)
				{
					this.OnCollectionItemPropertyChangedExtended(s, e);
				};
				this.MaskBindingCollection.CollectionChanged += delegate([Nullable(2)] object s, NotifyCollectionChangedEventArgs e)
				{
					this.OnMaskCollectionChangedExtended(s, e);
				};
			}
		}

		public virtual bool CanSave(bool verbose, ref bool errorIsShown)
		{
			bool errorIsShownTemp = errorIsShown;
			if (this.Any((KeyValuePair<GamepadButton, XBBinding> kvp) => !kvp.Value.CanSave(verbose, ref errorIsShownTemp)))
			{
				errorIsShown = errorIsShownTemp;
				return false;
			}
			if (this.ControllerBindings.Any((ControllerBinding cb) => !cb.XBBinding.CanSave(verbose, ref errorIsShownTemp)))
			{
				errorIsShown = errorIsShownTemp;
				return false;
			}
			return true;
		}

		public virtual bool SaveChanges(bool verbose, ref bool errorIsShown)
		{
			bool rSuccess = true;
			bool localErrorIsShown = errorIsShown;
			base.ForEachValue(delegate(XBBinding v)
			{
				rSuccess = v.SaveChanges(verbose, ref localErrorIsShown) & rSuccess;
			});
			errorIsShown = localErrorIsShown;
			foreach (ControllerBinding controllerBinding in this.ControllerBindings)
			{
				rSuccess = controllerBinding.XBBinding.SaveChanges(verbose, ref errorIsShown) & rSuccess;
			}
			this.IsChanged = false;
			return rSuccess;
		}

		public Dictionary<GamepadButton, XBBinding> GetUnmapedItems()
		{
			return this.Where((KeyValuePair<GamepadButton, XBBinding> b) => b.Value.IsUnmapped).ToDictionary((KeyValuePair<GamepadButton, XBBinding> i) => i.Key, (KeyValuePair<GamepadButton, XBBinding> i) => i.Value);
		}

		public void FillDictionaryWithEmpyXBBindings(ControllerFamily controllerFamily)
		{
			if (controllerFamily == 1)
			{
				return;
			}
			foreach (object obj in Enum.GetValues(typeof(GamepadButton)))
			{
				GamepadButton gamepadButton = (GamepadButton)obj;
				if (gamepadButton <= 140)
				{
					if (gamepadButton - 59 <= 1 || gamepadButton - 133 <= 7)
					{
						continue;
					}
				}
				else if (gamepadButton == 248 || gamepadButton == 2000)
				{
					continue;
				}
				if (controllerFamily != 2 || gamepadButton.ToString().Contains("MOUSE"))
				{
					this.SetEntry(gamepadButton, new XBBinding(this, gamepadButton));
				}
			}
			base.UpdateEntries();
		}

		protected bool CheckDuplicateShiftMappingForButtonMapping(object o, PropertyChangedExtendedEventArgs e, ShiftXBBindingCollection shiftXBBindingCollection)
		{
			return this.CheckDuplicateShiftMappingForButtonMapping(o, e, new List<ShiftXBBindingCollection> { shiftXBBindingCollection });
		}

		public bool CheckDuplicateShiftMappingForButtonMapping(object o, PropertyChangedExtendedEventArgs e, IEnumerable<ShiftXBBindingCollection> shiftXBBindingCollection)
		{
			return true;
		}

		public virtual void ResetCurrentMappingsAndOtherSelectors()
		{
			this.IsMaskModeView = false;
			this.IsVirtualStickSettingsModeView = false;
			this.IsOverlayMenuModeView = false;
			this.SetCurrentButtonMapping(null);
			if (this.MaskBindingCollection != null)
			{
				this.MaskBindingCollection.CurrentEditItem = null;
				this.MaskBindingCollection.AssociatedController = null;
				this.MaskBindingCollection.AssociatedControllerButtonContainer.ControllerButton.SetDefaultButtons(false);
			}
			if (this.ControllerBindings != null)
			{
				this.ControllerBindings.CurrentEditItem = null;
			}
		}

		public XBBinding GetXBBindingByAssociatedControllerButton(AssociatedControllerButton associatedControllerButton)
		{
			if (associatedControllerButton != null && associatedControllerButton.IsGamepad)
			{
				return this.GetXBBindingByGamepadButton(associatedControllerButton.GamepadButton);
			}
			if (associatedControllerButton != null && associatedControllerButton.IsKeyScanCode)
			{
				return this.GetXBBindingByKeyScanCodeButton(associatedControllerButton.KeyScanCode);
			}
			return null;
		}

		public XBBinding GetXBBindingByGamepadButton(GamepadButtonDescription gamepadButtonDescription)
		{
			return base.TryGetValue(gamepadButtonDescription.Button);
		}

		public XBBinding GetXBBindingByGamepadButton(GamepadButton annotatedButton)
		{
			return base.TryGetValue(annotatedButton);
		}

		public XBBinding GetXBBindingByGamepadButtonForGui(GamepadButton annotatedButton)
		{
			if (GamepadButtonExtensions.IsAdditionalStickDirection(annotatedButton) && this.AdditionalStickDirectionalGroup.IsNativeMode)
			{
				annotatedButton = this.AdditionalStickDirectionalGroup.GetNativeDirection(annotatedButton);
			}
			return base.TryGetValue(annotatedButton);
		}

		public XBBinding GetXBBindingByKeyScanCodeButton(KeyScanCodeV2 ksc)
		{
			return this.ControllerBindings[ksc];
		}

		public XBBinding GetXBBindingByMouseButton(MouseButton mouseButton)
		{
			SubConfigData subConfigData = this.SubConfigData;
			if (subConfigData != null && subConfigData.ControllerFamily == 1)
			{
				return null;
			}
			return this.GetXBBindingByKeyScanCodeButton(KeyScanCodeV2.FindKeyScanCodeByMouseButton(mouseButton));
		}

		public BaseDirectionalGroup GetDirectionalGroupByXBBinding(XBBinding xb)
		{
			return this.GetDirectionalGroupByXBBinding(xb.GamepadButton);
		}

		public BaseDirectionalGroup GetDirectionalGroupByXBBinding(GamepadButton gb)
		{
			if (GamepadButtonExtensions.IsLeftStick(gb) || GamepadButtonExtensions.IsLeftStickDiagonalDirection(gb))
			{
				return this.LeftStickDirectionalGroup;
			}
			if (GamepadButtonExtensions.IsRightStick(gb) || GamepadButtonExtensions.IsRightStickDiagonalDirection(gb))
			{
				return this.RightStickDirectionalGroup;
			}
			if (GamepadButtonExtensions.IsAdditionalStick(gb) || GamepadButtonExtensions.IsAdditionalStickDiagonalDirection(gb))
			{
				return this.AdditionalStickDirectionalGroup;
			}
			if (GamepadButtonExtensions.IsDPAD(gb))
			{
				return this.DPADDirectionalGroup;
			}
			if (GamepadButtonExtensions.IsMouseStick(gb))
			{
				return this.MouseDirectionalGroup;
			}
			if (GamepadButtonExtensions.IsGyroTilt(gb))
			{
				return this.GyroTiltDirectionalGroup;
			}
			if (GamepadButtonExtensions.IsPhysicalTrackPad1Direction(gb) || GamepadButtonExtensions.IsPhysicalTrack1DiagonalDirection(gb))
			{
				return this.Touchpad1DirectionalGroup;
			}
			if (GamepadButtonExtensions.IsPhysicalTrackPad2Direction(gb) || GamepadButtonExtensions.IsPhysicalTrack2DiagonalDirection(gb))
			{
				return this.Touchpad2DirectionalGroup;
			}
			return null;
		}

		public Dictionary<GamepadButton, XBBinding> GetDictionary()
		{
			return this.ToDictionary((KeyValuePair<GamepadButton, XBBinding> i) => i.Key, (KeyValuePair<GamepadButton, XBBinding> i) => i.Value);
		}

		public void CopyFromModel(BaseXBBindingCollection model, bool isCopyToNotEmpty = true)
		{
			this.AppName = model.AppName;
			this.ProcessNames = new ObservableCollection<string>(model.ProcessNames);
			this.Comment = model.Comment;
			this.Author = model.Author;
			this.IsSameScrollDelta = model.IsSameScrollDelta;
			if (model.Description != null && model.Description != this.DefaultDescription)
			{
				this.Description = model.Description;
			}
			this.MouseDirectionalGroupMacroCounter = model.MouseDirectionalGroupMacroCounter;
			this.LeftStickDirectionalGroupMacroCounter = model.LeftStickDirectionalGroupMacroCounter;
			this.RightStickDirectionalGroupMacroCounter = model.RightStickDirectionalGroupMacroCounter;
			this.GyroTiltDirectionalGroupMacroCounter = model.GyroTiltDirectionalGroupMacroCounter;
			this.LeftTouchpadDirectionalGroupMacroCounter = model.LeftTouchpadDirectionalGroupMacroCounter;
			this.RightTouchpadDirectionalGroupMacroCounter = model.RightTouchpadDirectionalGroupMacroCounter;
			foreach (XBBinding xbbinding in model)
			{
				GamepadButton gamepadButton = xbbinding.GamepadButton;
				if (base.ContainsKey(xbbinding.GamepadButton))
				{
					XBBinding xbbinding2 = base[xbbinding.GamepadButton];
					if (isCopyToNotEmpty || (!isCopyToNotEmpty && xbbinding2.IsEmpty))
					{
						base[xbbinding.GamepadButton].CopyFromModel(model[xbbinding.GamepadButton]);
					}
				}
			}
			this.ControllerBindings.CopyFromModel(model.ControllerBindings, isCopyToNotEmpty);
			List<AdaptiveTriggerPreset> list = new List<AdaptiveTriggerPreset> { 0, 2 };
			if (this.IsMainCollection)
			{
				list.Add(1);
			}
			if (!isCopyToNotEmpty)
			{
				if (isCopyToNotEmpty)
				{
					goto IL_1C0;
				}
				AdaptiveTriggerSettings adaptiveLeftTriggerSettings = this.AdaptiveLeftTriggerSettings;
				if (adaptiveLeftTriggerSettings == null || adaptiveLeftTriggerSettings.Preset != 0)
				{
					goto IL_1C0;
				}
			}
			if (model.AdaptiveLeftTriggerSettings != null)
			{
				if (list.Contains(model.AdaptiveLeftTriggerSettings.Preset))
				{
					this.AdaptiveLeftTriggerSettings = null;
				}
				else
				{
					this.AdaptiveLeftTriggerSettings = new AdaptiveTriggerSettings(0);
					this._adaptiveLeftTriggerSettings.CopyFromModel(model.AdaptiveLeftTriggerSettings);
				}
			}
			IL_1C0:
			if (!isCopyToNotEmpty)
			{
				if (isCopyToNotEmpty)
				{
					goto IL_21E;
				}
				AdaptiveTriggerSettings adaptiveRightTriggerSettings = this.AdaptiveRightTriggerSettings;
				if (adaptiveRightTriggerSettings == null || adaptiveRightTriggerSettings.Preset != 0)
				{
					goto IL_21E;
				}
			}
			if (model.AdaptiveRightTriggerSettings != null)
			{
				if (list.Contains(model.AdaptiveRightTriggerSettings.Preset))
				{
					this.AdaptiveRightTriggerSettings = null;
				}
				else
				{
					this.AdaptiveRightTriggerSettings = new AdaptiveTriggerSettings(0);
					this._adaptiveRightTriggerSettings.CopyFromModel(model.AdaptiveRightTriggerSettings);
				}
			}
			IL_21E:
			try
			{
				if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.GyroTiltDirectionalGroup.IsAdvancedDefault()))
				{
					this.GyroTiltDirectionalGroup.CopyFromModel(model.GyroTiltDirectionalGroup);
				}
			}
			catch (Exception)
			{
			}
			try
			{
				if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.LeftStickDirectionalGroup.IsAdvancedDefault()))
				{
					this.LeftStickDirectionalGroup.CopyFromModel(model.LeftStickDirectionalGroup);
				}
			}
			catch (Exception)
			{
			}
			try
			{
				if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.RightStickDirectionalGroup.IsAdvancedDefault()))
				{
					this.RightStickDirectionalGroup.CopyFromModel(model.RightStickDirectionalGroup);
				}
			}
			catch (Exception)
			{
			}
			try
			{
				if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.AdditionalStickDirectionalGroup.IsAdvancedDefault()))
				{
					this.AdditionalStickDirectionalGroup.CopyFromModel(model.AdditionalStickDirectionalGroup);
				}
			}
			catch (Exception)
			{
			}
			try
			{
				if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.MouseDirectionalGroup.IsAdvancedDefault()))
				{
					this.MouseDirectionalGroup.CopyFromModel(model.MouseDirectionalGroup);
				}
			}
			catch (Exception)
			{
			}
			try
			{
				if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.Touchpad1DirectionalGroup.IsAdvancedDefault()))
				{
					this.Touchpad1DirectionalGroup.CopyFromModel(model.Touchpad1DirectionalGroup);
				}
			}
			catch (Exception)
			{
			}
			try
			{
				if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.Touchpad2DirectionalGroup.IsAdvancedDefault()))
				{
					this.Touchpad2DirectionalGroup.CopyFromModel(model.Touchpad2DirectionalGroup);
				}
			}
			catch (Exception)
			{
			}
		}

		public void CopyFromModelDirectionalGroups(BaseXBBindingCollection model, bool isCopyToNotEmpty = true)
		{
			this.AppName = model.AppName;
			this.ProcessNames = new ObservableCollection<string>(model.ProcessNames);
			this.Comment = model.Comment;
			this.Author = model.Author;
			this.IsSameScrollDelta = model.IsSameScrollDelta;
			if (model.Description != null && model.Description != this.DefaultDescription)
			{
				this.Description = model.Description;
			}
			this.MouseDirectionalGroupMacroCounter = model.MouseDirectionalGroupMacroCounter;
			this.LeftStickDirectionalGroupMacroCounter = model.LeftStickDirectionalGroupMacroCounter;
			this.RightStickDirectionalGroupMacroCounter = model.RightStickDirectionalGroupMacroCounter;
			this.GyroTiltDirectionalGroupMacroCounter = model.GyroTiltDirectionalGroupMacroCounter;
			this.LeftTouchpadDirectionalGroupMacroCounter = model.LeftTouchpadDirectionalGroupMacroCounter;
			this.RightTouchpadDirectionalGroupMacroCounter = model.RightTouchpadDirectionalGroupMacroCounter;
			try
			{
				if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.GyroTiltDirectionalGroup.IsAdvancedDefault()))
				{
					this.GyroTiltDirectionalGroup.CopyFromModel(model.GyroTiltDirectionalGroup);
				}
			}
			catch (Exception)
			{
			}
			try
			{
				if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.LeftStickDirectionalGroup.IsAdvancedDefault()))
				{
					this.LeftStickDirectionalGroup.CopyFromModel(model.LeftStickDirectionalGroup);
				}
			}
			catch (Exception)
			{
			}
			try
			{
				if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.RightStickDirectionalGroup.IsAdvancedDefault()))
				{
					this.RightStickDirectionalGroup.CopyFromModel(model.RightStickDirectionalGroup);
				}
			}
			catch (Exception)
			{
			}
			try
			{
				if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.AdditionalStickDirectionalGroup.IsAdvancedDefault()))
				{
					this.AdditionalStickDirectionalGroup.CopyFromModel(model.AdditionalStickDirectionalGroup);
				}
			}
			catch (Exception)
			{
			}
			try
			{
				if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.MouseDirectionalGroup.IsAdvancedDefault()))
				{
					this.MouseDirectionalGroup.CopyFromModel(model.MouseDirectionalGroup);
				}
			}
			catch (Exception)
			{
			}
			try
			{
				if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.Touchpad1DirectionalGroup.IsAdvancedDefault()))
				{
					this.Touchpad1DirectionalGroup.CopyFromModel(model.Touchpad1DirectionalGroup);
				}
			}
			catch (Exception)
			{
			}
			try
			{
				if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.Touchpad2DirectionalGroup.IsAdvancedDefault()))
				{
					this.Touchpad2DirectionalGroup.CopyFromModel(model.Touchpad2DirectionalGroup);
				}
			}
			catch (Exception)
			{
			}
		}

		public void CopyToModelDirectionalGroups(BaseXBBindingCollection model)
		{
			model.AppName = this.AppName;
			model.ProcessNames = new List<string>(this.ProcessNames);
			model.Comment = this.Comment;
			model.Author = this.Author;
			model.IsSameScrollDelta = this.IsSameScrollDelta;
			if (this._description != null && model.Description != this.DefaultDescription)
			{
				model.Description = this.Description;
			}
			model.MouseDirectionalGroupMacroCounter = this.MouseDirectionalGroupMacroCounter;
			model.LeftStickDirectionalGroupMacroCounter = this.LeftStickDirectionalGroupMacroCounter;
			model.RightStickDirectionalGroupMacroCounter = this.RightStickDirectionalGroupMacroCounter;
			model.GyroTiltDirectionalGroupMacroCounter = this.GyroTiltDirectionalGroupMacroCounter;
			model.LeftTouchpadDirectionalGroupMacroCounter = this.LeftTouchpadDirectionalGroupMacroCounter;
			model.RightTouchpadDirectionalGroupMacroCounter = this.RightTouchpadDirectionalGroupMacroCounter;
			model.Touchpad1DirectionalGroup.TouchpadAnalogMode = this.Touchpad1DirectionalGroup.TouchpadAnalogMode;
			model.Touchpad2DirectionalGroup.TouchpadAnalogMode = this.Touchpad2DirectionalGroup.TouchpadAnalogMode;
			this.GyroTiltDirectionalGroup.CopyToModel(model.GyroTiltDirectionalGroup);
			this.LeftStickDirectionalGroup.CopyToModel(model.LeftStickDirectionalGroup);
			this.RightStickDirectionalGroup.CopyToModel(model.RightStickDirectionalGroup);
			this.AdditionalStickDirectionalGroup.CopyToModel(model.AdditionalStickDirectionalGroup);
			this.MouseDirectionalGroup.CopyToModel(model.MouseDirectionalGroup);
			this.Touchpad1DirectionalGroup.CopyToModel(model.Touchpad1DirectionalGroup);
			this.Touchpad2DirectionalGroup.CopyToModel(model.Touchpad2DirectionalGroup);
		}

		public void CopyToModel(BaseXBBindingCollection model)
		{
			model.AppName = this.AppName;
			model.ProcessNames = new List<string>(this.ProcessNames);
			model.Comment = this.Comment;
			model.Author = this.Author;
			model.IsSameScrollDelta = this.IsSameScrollDelta;
			if (this._description != null && model.Description != this.DefaultDescription)
			{
				model.Description = this.Description;
			}
			model.MouseDirectionalGroupMacroCounter = this.MouseDirectionalGroupMacroCounter;
			model.LeftStickDirectionalGroupMacroCounter = this.LeftStickDirectionalGroupMacroCounter;
			model.RightStickDirectionalGroupMacroCounter = this.RightStickDirectionalGroupMacroCounter;
			model.GyroTiltDirectionalGroupMacroCounter = this.GyroTiltDirectionalGroupMacroCounter;
			model.LeftTouchpadDirectionalGroupMacroCounter = this.LeftTouchpadDirectionalGroupMacroCounter;
			model.RightTouchpadDirectionalGroupMacroCounter = this.RightTouchpadDirectionalGroupMacroCounter;
			model.Touchpad1DirectionalGroup.TouchpadAnalogMode = this.Touchpad1DirectionalGroup.TouchpadAnalogMode;
			model.Touchpad2DirectionalGroup.TouchpadAnalogMode = this.Touchpad2DirectionalGroup.TouchpadAnalogMode;
			foreach (KeyValuePair<GamepadButton, XBBinding> keyValuePair in ((IEnumerable<KeyValuePair<GamepadButton, XBBinding>>)this))
			{
				if (base[keyValuePair.Key].IsNeedSave)
				{
					base[keyValuePair.Key].CopyToModel(model[keyValuePair.Key]);
				}
			}
			this.ControllerBindings.CopyToModel(model.ControllerBindings);
			this.GyroTiltDirectionalGroup.CopyToModel(model.GyroTiltDirectionalGroup);
			this.LeftStickDirectionalGroup.CopyToModel(model.LeftStickDirectionalGroup);
			this.RightStickDirectionalGroup.CopyToModel(model.RightStickDirectionalGroup);
			this.AdditionalStickDirectionalGroup.CopyToModel(model.AdditionalStickDirectionalGroup);
			this.MouseDirectionalGroup.CopyToModel(model.MouseDirectionalGroup);
			this.Touchpad1DirectionalGroup.CopyToModel(model.Touchpad1DirectionalGroup);
			this.Touchpad2DirectionalGroup.CopyToModel(model.Touchpad2DirectionalGroup);
			if (this._adaptiveLeftTriggerSettings != null)
			{
				model.AdaptiveLeftTriggerSettings = new AdaptiveTriggerSettings(0);
				this.AdaptiveLeftTriggerSettings.CopyTo(model.AdaptiveLeftTriggerSettings);
			}
			if (this._adaptiveRightTriggerSettings != null)
			{
				model.AdaptiveRightTriggerSettings = new AdaptiveTriggerSettings(0);
				this.AdaptiveRightTriggerSettings.CopyTo(model.AdaptiveRightTriggerSettings);
			}
		}

		public void IsGamepadMappingAvailiableForCurrentBindingChanged()
		{
			this.OnPropertyChanged("IsGamepadMappingAvailiableForCurrentBinding");
		}

		public IEnumerable<ActivatorXBBinding> EnumAllActivatorsWithShift
		{
			get
			{
				int num;
				for (int i = 0; i < this.SubConfigData.MainXBBindingCollection.ShiftXBBindingCollections.Count + 1; i = num + 1)
				{
					if (i != this.ShiftIndex)
					{
						IEnumerator<XBBinding> enumerator2;
						foreach (SubConfigData subConfigData in this.SubConfigData.ConfigData)
						{
							BaseXBBindingCollection collectionByLayer = subConfigData.MainXBBindingCollection.GetCollectionByLayer(i);
							foreach (XBBinding xbbinding in collectionByLayer.EnumAllBindings(true, true, false))
							{
								foreach (KeyValuePair<ActivatorType, ActivatorXBBinding> keyValuePair in ((IEnumerable<KeyValuePair<ActivatorType, ActivatorXBBinding>>)xbbinding.ActivatorXBBindingDictionary))
								{
									if (keyValuePair.Value.JumpToShift == this.ShiftIndex)
									{
										yield return keyValuePair.Value;
									}
								}
								IEnumerator<KeyValuePair<ActivatorType, ActivatorXBBinding>> enumerator3 = null;
							}
							enumerator2 = null;
						}
						IEnumerator<SubConfigData> enumerator = null;
						BaseXBBindingCollection collectionByLayer2 = this.SubConfigData.MainXBBindingCollection.GetCollectionByLayer(i);
						foreach (XBBinding xbbinding2 in collectionByLayer2.EnumAllBindings(false, false, true))
						{
							foreach (KeyValuePair<ActivatorType, ActivatorXBBinding> keyValuePair2 in ((IEnumerable<KeyValuePair<ActivatorType, ActivatorXBBinding>>)xbbinding2.ActivatorXBBindingDictionary))
							{
								if (keyValuePair2.Value.JumpToShift == this.ShiftIndex)
								{
									yield return keyValuePair2.Value;
								}
							}
							IEnumerator<KeyValuePair<ActivatorType, ActivatorXBBinding>> enumerator3 = null;
						}
						enumerator2 = null;
					}
					num = i;
				}
				yield break;
				yield break;
			}
		}

		public IEnumerable<XBBinding> EnumAllBindingsForController(ControllerTypeEnum controllerType)
		{
			SupportedControllerInfo supportedControllerInfo;
			ControllersHelper.SupportedControllersDictionary.TryGetValue(controllerType, out supportedControllerInfo);
			SupportedGamepad controller = supportedControllerInfo as SupportedGamepad;
			foreach (KeyValuePair<GamepadButton, XBBinding> keyValuePair in ((IEnumerable<KeyValuePair<GamepadButton, XBBinding>>)this))
			{
				if (controller != null && controller.Buttons.ContainsKey(keyValuePair.Key))
				{
					yield return keyValuePair.Value;
				}
			}
			IEnumerator<KeyValuePair<GamepadButton, XBBinding>> enumerator = null;
			foreach (ControllerBinding controllerBinding in this.ControllerBindings)
			{
				yield return controllerBinding.XBBinding;
			}
			IEnumerator<ControllerBinding> enumerator2 = null;
			if (this.MaskBindingCollection != null)
			{
				foreach (MaskItem maskItem in this.MaskBindingCollection)
				{
					if (this.MaskBindingCollection.Filter(maskItem))
					{
						yield return maskItem.XBBinding;
					}
				}
				IEnumerator<MaskItem> enumerator3 = null;
			}
			yield break;
			yield break;
		}

		public IEnumerable<XBBinding> EnumAllBindings(bool gamepadItems = true, bool controllerItems = true, bool maskItems = true)
		{
			if (gamepadItems)
			{
				foreach (KeyValuePair<GamepadButton, XBBinding> keyValuePair in ((IEnumerable<KeyValuePair<GamepadButton, XBBinding>>)this))
				{
					yield return keyValuePair.Value;
				}
				IEnumerator<KeyValuePair<GamepadButton, XBBinding>> enumerator = null;
			}
			if (controllerItems)
			{
				foreach (ControllerBinding controllerBinding in this.ControllerBindings)
				{
					yield return controllerBinding.XBBinding;
				}
				IEnumerator<ControllerBinding> enumerator2 = null;
			}
			if (maskItems && this.MaskBindingCollection != null)
			{
				foreach (MaskItem maskItem in this.MaskBindingCollection)
				{
					yield return maskItem.XBBinding;
				}
				IEnumerator<MaskItem> enumerator3 = null;
			}
			yield break;
			yield break;
		}

		private SolidColorBrush _collectionBrush;

		private SolidColorBrush _collectionBrushHighlighted;

		private SolidColorBrush _collectionBrushPressed;

		private ControllerBindingFrameAdditionalModes? _controllerBindingFrameMode;

		private bool _isChanged;

		public int _shiftIndex;

		private string _appName;

		private ObservableCollection<string> _processNames;

		private string _comment;

		private string _author;

		private string _description;

		private bool _isSameScrollDelta = true;

		private DelegateCommand<XBBinding> _setSameScrollDeltaCommand;

		private KeyValuePair<GamepadButton, XBBinding>? _currentButtonMapping;

		[CanBeNull]
		private AdaptiveTriggerSettings _adaptiveLeftTriggerSettings;

		[CanBeNull]
		private AdaptiveTriggerSettings _adaptiveRightTriggerSettings;

		private int _mouseDirectionalGroupMacroCounter;

		private int _leftStickDirectionalGroupMacroCounter;

		private int _rightStickDirectionalGroupMacroCounter;

		private int _gyroTiltDirectionalGroupMacroCounter;

		private int _leftTouchpadDirectionalGroupMacroCounter;

		private int _rightTouchpadDirectionalGroupMacroCounter;

		private LeftStickDirectionalGroup _leftStickDirectionalGroup;

		private RightStickDirectionalGroup _rightStickDirectionalGroup;

		private AdditionalStickDirectionalGroup _additionalStickDirectionalGroup;

		private MouseDirectionalGroup _mouseDirectionalGroup;

		private GyroTiltDirectionalGroup _gyroTiltDirectionalGroup;

		private Touchpad1DirectionalGroup _touchpad1DirectionalGroup;

		private Touchpad2DirectionalGroup _touchpad2DirectionalGroup;

		private DPADDirectionalGroup _DPADDirectionalGroup;

		private BaseDirectionalGroup _currentBoundGroup;

		private DelegateCommand _BindCurrentToOverlayMenuDirections;

		private DelegateCommand _BindCurrentToWASD;

		private DelegateCommand _BindCurrentToFlickStick;

		private DelegateCommand _BindCurrentToArrows;

		private DelegateCommand _BindCurrentToMouse;

		private DelegateCommand _BindCurrentToLeftVirtualStick;

		private DelegateCommand _BindCurrentToRightVirtualStick;

		private DelegateCommand _BindCurrentToVirtualDPAD;

		private DelegateCommand _BindCurrentToDS4Gamepad;

		private DelegateCommand _UnmapCurrent;

		private bool _isLabelModeView;

		private bool _isShowMappingsView = true;

		private bool _isShowAllView;

		private bool _isExpandActivatorsView;

		private bool _isMaskModeView;

		private bool _isVirtualStickSettingsModeView;

		private bool _isOverlayMenuModeView;

		private DelegateCommand<ActivatorType?> _switchCurrentActivator;

		private DelegateCommand<Stick?> _invertStickX;

		private DelegateCommand<Stick?> _invertStickY;

		private DelegateCommand _swapSticks;

		private DelegateCommand<Stick?> _resetStickToDefault;

		private DelegateCommand _addControllerBinding;

		private DelegateCommand _UnmapWholeGamepad;

		public delegate void IsChangedModified();
	}
}
