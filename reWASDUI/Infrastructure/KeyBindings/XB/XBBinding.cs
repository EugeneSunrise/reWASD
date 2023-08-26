using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Attributes;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.View.SecondaryWindows;
using Prism.Commands;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings.ActivatorXB;
using reWASDUI.Infrastructure.KeyBindings.ControllerBindings;
using reWASDUI.Infrastructure.KeyBindings.MacroBinding;
using reWASDUI.Infrastructure.KeyBindings.Mask;
using reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups;
using reWASDUI.Services.Interfaces;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.KeyBindingsModel.XB;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Infrastructure.KeyBindings.XB
{
	public class XBBinding : AssociatedControllerButtonContainer
	{
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

		public DelegateCommand ClearBindingCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._clearBinding) == null)
				{
					delegateCommand = (this._clearBinding = new DelegateCommand(new Action(this.ClearBinding), new Func<bool>(this.ClearBindingCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void ClearBinding()
		{
			XBBinding xbbinding = null;
			bool flag = false;
			if (this.IsInheritedBinding)
			{
				MainXBBindingCollection mainXBBindingCollection = this.HostCollection as MainXBBindingCollection;
				if (mainXBBindingCollection != null)
				{
					xbbinding = mainXBBindingCollection.CurrentShiftXBBindingCollection.GetXBBindingByAssociatedControllerButton(base.ControllerButton);
				}
				else
				{
					ShiftXBBindingCollection shiftXBBindingCollection = this.HostCollection as ShiftXBBindingCollection;
					if (shiftXBBindingCollection != null)
					{
						xbbinding = shiftXBBindingCollection.GetXBBindingByAssociatedControllerButton(base.ControllerButton);
					}
				}
				flag = true;
			}
			else
			{
				ShiftXBBindingCollection shiftXBBindingCollection2 = this.HostCollection as ShiftXBBindingCollection;
				if (shiftXBBindingCollection2 != null)
				{
					if (!GamepadButtonExtensions.IsMouseDirection(base.GamepadButton))
					{
						XBBinding xbbindingByGamepadButton = shiftXBBindingCollection2.ParentBindingCollection.GetXBBindingByGamepadButton(base.GamepadButton);
						if (xbbindingByGamepadButton != null && xbbindingByGamepadButton.ClearBindingCanExecute())
						{
							goto IL_C8;
						}
					}
					if (base.KeyScanCode == null || base.KeyScanCode == KeyScanCodeV2.NoMap)
					{
						goto IL_E6;
					}
					XBBinding xbbindingByKeyScanCodeButton = shiftXBBindingCollection2.ParentBindingCollection.GetXBBindingByKeyScanCodeButton(base.KeyScanCode);
					if (xbbindingByKeyScanCodeButton == null || !xbbindingByKeyScanCodeButton.ClearBindingCanExecute())
					{
						goto IL_E6;
					}
					IL_C8:
					xbbinding = this;
					if (xbbinding.SingleActivator.MappedKey != KeyScanCodeV2.NoInheritance && !xbbinding.IsContainsJumpToShift)
					{
						flag = true;
					}
				}
			}
			IL_E6:
			if (xbbinding != null)
			{
				xbbinding.ActivatorXBBindingDictionary.RemoveAllVirtualBindings();
				xbbinding.ActivatorXBBindingDictionary.RemoveAllDescriptions();
				xbbinding.RevertRemap();
				if (flag)
				{
					xbbinding.SingleActivator.MappedKey = KeyScanCodeV2.NoInheritance;
				}
				this.IsDisableInheritVirtualMapFromMain = !this.IsDisableInheritVirtualMapFromMain;
				this.IsDisableInheritVirtualMapFromMain = !this.IsDisableInheritVirtualMapFromMain;
			}
			else if (!GamepadButtonExtensions.IsMouseDirection(base.GamepadButton))
			{
				this.ActivatorXBBindingDictionary.RemoveAllVirtualBindings();
				this.ActivatorXBBindingDictionary.RemoveAllDescriptions();
				this.RevertRemap();
			}
			if (base.ControllerButton.IsGamepad)
			{
				if (GamepadButtonExtensions.IsLeftStickDirection(base.ControllerButton.GamepadButton) || GamepadButtonExtensions.IsLeftStickClick(base.ControllerButton.GamepadButton))
				{
					App.EventAggregator.GetEvent<RequestRefreshAnnotationVisibilityForGroup>().Publish(this.HostCollection.LeftStickDirectionalGroup);
				}
				else if (GamepadButtonExtensions.IsRightStickDirection(base.ControllerButton.GamepadButton) || GamepadButtonExtensions.IsRightStickClick(base.ControllerButton.GamepadButton))
				{
					App.EventAggregator.GetEvent<RequestRefreshAnnotationVisibilityForGroup>().Publish(this.HostCollection.RightStickDirectionalGroup);
				}
				else if (GamepadButtonExtensions.IsGyroTiltDirection(base.ControllerButton.GamepadButton))
				{
					App.EventAggregator.GetEvent<RequestRefreshAnnotationVisibilityForGroup>().Publish(this.HostCollection.GyroTiltDirectionalGroup);
				}
				else if (GamepadButtonExtensions.IsDPAD(base.ControllerButton.GamepadButton))
				{
					App.EventAggregator.GetEvent<RequestRefreshAnnotationVisibilityForGroup>().Publish(this.HostCollection.DPADDirectionalGroup);
				}
				else if (GamepadButtonExtensions.IsPhysicalTrackPad1(base.ControllerButton.GamepadButton))
				{
					App.EventAggregator.GetEvent<RequestRefreshAnnotationVisibilityForGroup>().Publish(this.HostCollection.Touchpad1DirectionalGroup);
				}
				else if (GamepadButtonExtensions.IsPhysicalTrackPad2(base.ControllerButton.GamepadButton))
				{
					App.EventAggregator.GetEvent<RequestRefreshAnnotationVisibilityForGroup>().Publish(this.HostCollection.Touchpad2DirectionalGroup);
				}
				else if (GamepadButtonExtensions.IsMouseDirection(base.ControllerButton.GamepadButton))
				{
					this.HostCollection.MouseDirectionalGroup.ResetToDefault(false);
					App.EventAggregator.GetEvent<RequestRefreshAnnotationVisibilityForGroup>().Publish(this.HostCollection.MouseDirectionalGroup);
				}
			}
			App.EventAggregator.GetEvent<CurrentButtonMappingChanged>().Publish(null);
		}

		private bool ClearBindingCanExecute()
		{
			if (this.HostCollection != null)
			{
				BaseTouchpadDirectionalGroup baseTouchpadDirectionalGroup = this.HostCollection.GetDirectionalGroupByXBBinding(this) as BaseTouchpadDirectionalGroup;
				if (baseTouchpadDirectionalGroup == null || !baseTouchpadDirectionalGroup.TouchpadAnalogMode)
				{
					if (!GamepadButtonExtensions.IsMouseDirection(base.GamepadButton) && (this.IsAnyActivatorDescriptionPresent || this.IsContainsJumpToShift || this.IsAnyActivatorVirtualMappingPresent || this.IsAnyActivatorVirtualGamepadMappingPresent || this.IsRemapedOrUnmapped))
					{
						return true;
					}
					if ((GamepadButtonExtensions.IsMouseDirection(base.GamepadButton) && this.IsAnyActivatorDescriptionPresent) || this.IsAnyActivatorVirtualMappingPresent || this.IsAnyActivatorVirtualGamepadMappingPresent || this.IsRemapedOrUnmapped)
					{
						return true;
					}
					if (base.GamepadButton != 2001)
					{
						ShiftXBBindingCollection shiftXBBindingCollection = this.HostCollection as ShiftXBBindingCollection;
						if (shiftXBBindingCollection != null && !GamepadButtonExtensions.IsMouseDirection(base.GamepadButton))
						{
							XBBinding xbbindingByGamepadButton = shiftXBBindingCollection.ParentBindingCollection.GetXBBindingByGamepadButton(base.GamepadButton);
							if (xbbindingByGamepadButton != null && xbbindingByGamepadButton.ClearBindingCanExecute())
							{
								return true;
							}
						}
					}
					else if (base.KeyScanCode != null && base.KeyScanCode != KeyScanCodeV2.NoMap)
					{
						ShiftXBBindingCollection shiftXBBindingCollection2 = this.HostCollection as ShiftXBBindingCollection;
						if (shiftXBBindingCollection2 != null)
						{
							XBBinding xbbindingByKeyScanCodeButton = shiftXBBindingCollection2.ParentBindingCollection.GetXBBindingByKeyScanCodeButton(base.KeyScanCode);
							if (xbbindingByKeyScanCodeButton != null && xbbindingByKeyScanCodeButton.ClearBindingCanExecute())
							{
								return true;
							}
						}
					}
					return false;
				}
			}
			return false;
		}

		public int MouseScrollDelta
		{
			get
			{
				return this._mouseScrollDelta;
			}
			set
			{
				if (value == 0)
				{
					value = 2;
				}
				if (this._mouseScrollDelta == value)
				{
					return;
				}
				this._mouseScrollDelta = value;
				this.HostCollection.IsChanged = true;
				if (this.HostCollection.IsSameScrollDelta && this.HostCollection.IsMainCollection)
				{
					GamepadButton gamepadButton = base.GamepadButton;
					if (gamepadButton != 171)
					{
						if (gamepadButton == 172)
						{
							XBBinding xbbinding = this.HostCollection.TryGetValue(171);
							if (xbbinding != null)
							{
								xbbinding._mouseScrollDelta = value;
							}
						}
					}
					else
					{
						XBBinding xbbinding2 = this.HostCollection.TryGetValue(172);
						if (xbbinding2 != null)
						{
							xbbinding2._mouseScrollDelta = value;
						}
					}
				}
				this.OnPropertyChanged("MouseScrollDelta");
			}
		}

		public bool IsMouseScrollDeltaShouldBeShown
		{
			get
			{
				return GamepadButtonExtensions.IsMouseScroll(base.GamepadButton) && this.HostCollection.IsMainCollection;
			}
		}

		public bool IsDisableInheritVirtualMapFromMain
		{
			get
			{
				return this._isDisableInheritVirtualMapFromMain;
			}
			set
			{
				this.SetProperty<bool>(ref this._isDisableInheritVirtualMapFromMain, value, "IsDisableInheritVirtualMapFromMain");
			}
		}

		public ObservableCollection<AssociatedControllerButton> MaskConditions { get; } = new ObservableCollection<AssociatedControllerButton>();

		public bool MaskConditionsHasZones
		{
			get
			{
				if (this.HostMaskItem == null)
				{
					return false;
				}
				ObservableCollection<AssociatedControllerButton> maskConditions = this.MaskConditions;
				if (maskConditions == null)
				{
					return false;
				}
				return maskConditions.Any((AssociatedControllerButton item) => item != null && GamepadButtonExtensions.IsAnyZoneOrDirections(item.GamepadButton) && item != null && !GamepadButtonExtensions.IsAnyPhysicalTrackPadDigitalDirection(item.GamepadButton));
			}
		}

		public bool CanContainShift
		{
			get
			{
				return (base.ControllerButton.IsGamepad && this.IsJumpToLayerAllowed) || base.ControllerButton.IsKeyScanCode || (this.HostMaskItem != null && !this.MaskConditionsHasZones);
			}
		}

		public ActivatorXBBinding SingleActivator
		{
			get
			{
				ActivatorXBBindingDictionary activatorXBBindingDictionary = this.ActivatorXBBindingDictionary;
				if (activatorXBBindingDictionary == null)
				{
					return null;
				}
				return activatorXBBindingDictionary.TryGetValue(0);
			}
		}

		public ActivatorXBBinding LongActivator
		{
			get
			{
				ActivatorXBBindingDictionary activatorXBBindingDictionary = this.ActivatorXBBindingDictionary;
				if (activatorXBBindingDictionary == null)
				{
					return null;
				}
				return activatorXBBindingDictionary.TryGetValue(1);
			}
		}

		public ActivatorXBBinding DoubleActivator
		{
			get
			{
				ActivatorXBBindingDictionary activatorXBBindingDictionary = this.ActivatorXBBindingDictionary;
				if (activatorXBBindingDictionary == null)
				{
					return null;
				}
				return activatorXBBindingDictionary.TryGetValue(2);
			}
		}

		public ActivatorXBBinding TripleActivator
		{
			get
			{
				ActivatorXBBindingDictionary activatorXBBindingDictionary = this.ActivatorXBBindingDictionary;
				if (activatorXBBindingDictionary == null)
				{
					return null;
				}
				return activatorXBBindingDictionary.TryGetValue(3);
			}
		}

		public ActivatorXBBinding StartActivator
		{
			get
			{
				ActivatorXBBindingDictionary activatorXBBindingDictionary = this.ActivatorXBBindingDictionary;
				if (activatorXBBindingDictionary == null)
				{
					return null;
				}
				return activatorXBBindingDictionary.TryGetValue(4);
			}
		}

		public ActivatorXBBinding ReleaseActivator
		{
			get
			{
				ActivatorXBBindingDictionary activatorXBBindingDictionary = this.ActivatorXBBindingDictionary;
				if (activatorXBBindingDictionary == null)
				{
					return null;
				}
				return activatorXBBindingDictionary.TryGetValue(5);
			}
		}

		public ActivatorXBBindingDictionary ActivatorXBBindingDictionary
		{
			get
			{
				return this._activatorXBBindingDictionary;
			}
			set
			{
				if (this._activatorXBBindingDictionary != null && value == null)
				{
					this.ActivatorXBBindingDictionary.OnValueSet = null;
					this.ActivatorXBBindingDictionary.HostXBBinding = null;
				}
				if (value != null && !object.Equals(this._activatorXBBindingDictionary, value))
				{
					value.HostXBBinding = this;
				}
				if (this.SetProperty<ActivatorXBBindingDictionary>(ref this._activatorXBBindingDictionary, value, "ActivatorXBBindingDictionary"))
				{
					if (this.ActivatorXBBindingDictionary != null)
					{
						this.ActivatorXBBindingDictionary.OnValueSet = new EventHandler<ActivatorType>(this.OnActivatorDictionaryValueSet);
						this.SetNonEmptyCurrentActivator(true);
					}
					this.OnPropertyChanged("SingleActivator");
					this.OnPropertyChanged("LongActivator");
					this.OnPropertyChanged("DoubleActivator");
					this.OnPropertyChanged("TripleActivator");
					this.OnPropertyChanged("StartActivator");
					this.OnPropertyChanged("ReleaseActivator");
				}
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public ActivatorXBBinding CurrentActivatorXBBinding
		{
			get
			{
				return this._currentActivatorXBBinding;
			}
			set
			{
				ActivatorXBBinding currentActivatorXBBinding = this._currentActivatorXBBinding;
				if (this.SetProperty<ActivatorXBBinding>(ref this._currentActivatorXBBinding, value, "CurrentActivatorXBBinding"))
				{
					if (currentActivatorXBBinding != null)
					{
						currentActivatorXBBinding.PropertyChangedExtended -= this.CurrentActivatorXBBindingOnPropertyChangedExtended;
					}
					if (this._currentActivatorXBBinding != null)
					{
						this._currentActivatorXBBinding.PropertyChangedExtended += this.CurrentActivatorXBBindingOnPropertyChangedExtended;
					}
					if (this.CurrentActivatorXBBinding.JumpToShift != -1 && this.HostCollection == this.HostCollection.SubConfigData.MainXBBindingCollection.RealCurrentBeingMappedBindingCollection)
					{
						XBBinding shiftXBBinding = this.CurrentActivatorXBBinding.ShiftXBBinding;
						if (shiftXBBinding == null)
						{
							return;
						}
						shiftXBBinding.SwitchCurrentActivator(this.CurrentActivatorXBBinding.ActivatorType);
					}
				}
			}
		}

		public bool IsGamepadMappingAvailiable
		{
			get
			{
				if (!base.ControllerButton.IsGamepad)
				{
					return false;
				}
				ControllerTypeEnum? controllerTypeEnum = this.CurrentControllerType;
				if (controllerTypeEnum != null && ControllerTypeExtensions.IsXboxOrRazerWolverine2(controllerTypeEnum.GetValueOrDefault()) && base.GamepadButton == 11)
				{
					return false;
				}
				controllerTypeEnum = this.CurrentControllerType;
				if (controllerTypeEnum != null && ControllerTypeExtensions.IsGameSirG7(controllerTypeEnum.GetValueOrDefault()) && (base.GamepadButton == 12 || base.GamepadButton == 13 || base.GamepadButton == 14 || base.GamepadButton == 29 || base.GamepadButton == 30))
				{
					return false;
				}
				controllerTypeEnum = this.CurrentControllerType;
				if (controllerTypeEnum != null && ControllerTypeExtensions.IsSonyDualSenseEdge(controllerTypeEnum.GetValueOrDefault()) && (base.GamepadButton == 14 || base.GamepadButton == 15 || base.GamepadButton == 16 || base.GamepadButton == 17))
				{
					return false;
				}
				controllerTypeEnum = this.CurrentControllerType;
				return (controllerTypeEnum == null || !ControllerTypeExtensions.IsGamepadNativeMappingNotAvailiable(controllerTypeEnum.GetValueOrDefault())) && GamepadButtonExtensions.IsGamepadMappingAvailiable(base.GamepadButton);
			}
		}

		private void CurrentActivatorXBBindingOnPropertyChangedExtended(object s, PropertyChangedExtendedEventArgs e)
		{
			if (e.PropertyName == "MappedKey")
			{
				KeyScanCodeV2 keyScanCodeV = e.NewValue as KeyScanCodeV2;
				if (keyScanCodeV == null || (keyScanCodeV != KeyScanCodeV2.NoInheritance && keyScanCodeV != KeyScanCodeV2.NoMap))
				{
					this.TryShowKeyboardMappingUnmapWarning(s as ActivatorXBBinding, e.OldValue as KeyScanCodeV2);
				}
			}
			this.OnPropertyChangedExtended(s, e);
			if (e.PropertyName == "MacroSequenceAnnotation")
			{
				return;
			}
			this.OnPropertyChanged("IsRemapShouldBeShown");
			this.OnPropertyChanged("IsUnmapShouldBeShown");
			this.OnPropertyChanged("IsMouseScrollDeltaShouldBeShown");
			this.OnPropertyChanged("IsRemapedOrUnmappedShouldBeShown");
			this.OnPropertyChanged("IsAnnotationShouldBeShownForMapping");
			this.OnPropertyChanged("IsAnnotationShouldBeShownForDescription");
			this.OnPropertyChanged("IsAnyActivatorDescriptionPresent");
			this.OnPropertyChanged("IsAnyActivatorVirtualMappingPresent");
			this.OnPropertyChanged("IsAnyNonSingleActivatorVirtualMappingPresent");
			this.OnPropertyChanged("IsAnyActivatorVirtualGamepadMappingPresent");
			this.OnPropertyChanged("IsAnyTurboPresent");
			this.OnPropertyChanged("IsAnyTogglePresent");
			BaseXBBindingCollection realCurrentBeingMappedBindingCollection = App.GameProfilesService.RealCurrentBeingMappedBindingCollection;
			XBBinding xbbinding = ((realCurrentBeingMappedBindingCollection != null) ? realCurrentBeingMappedBindingCollection.CurrentXBBinding : null);
			if (xbbinding != null && ((xbbinding.CurrentActivatorXBBinding.JumpToShift != -1 && xbbinding.CurrentActivatorXBBinding.ShiftXBBinding == this) || xbbinding == this) && e.PropertyName != "MappedKeyBytes")
			{
				this.CopyToAnotherShifts();
			}
			this.ClearBindingCommand.RaiseCanExecuteChanged();
		}

		private List<int> GetJumpToCurrentLayer()
		{
			List<int> list = new List<int>();
			Action<int> <>9__1;
			this.ActivatorXBBindingDictionary.ForEachValue(delegate(ActivatorXBBinding item)
			{
				List<int> jumpToCurrentLayer = item.GetJumpToCurrentLayer();
				Action<int> action;
				if ((action = <>9__1) == null)
				{
					action = (<>9__1 = delegate(int layer)
					{
						if (!list.Contains(layer) && layer != this.HostCollection.ShiftIndex)
						{
							list.Add(layer);
						}
					});
				}
				jumpToCurrentLayer.ForEach(action);
			});
			return list;
		}

		private void FindXBBindingsByShift(XBBinding from, List<XBBinding> destXbBindings)
		{
			from.ActivatorXBBindingDictionary.ForEachValue(delegate(ActivatorXBBinding item)
			{
				if (item.JumpToShift != -1)
				{
					XBBinding xbbindingByLayer = item.GetXBBindingByLayer(item.JumpToShift, false, true);
					if (xbbindingByLayer != null && (!item.IsShiftToggle || !xbbindingByLayer.ActivatorXBBindingDictionary[item.ActivatorType].IsToggle) && ((item.IsDelayBerforeJumpChecked && !item.IsPostponeMapping) || (item.IsShiftToggle && !xbbindingByLayer.ActivatorXBBindingDictionary[item.ActivatorType].IsToggle)) && !destXbBindings.Contains(xbbindingByLayer))
					{
						destXbBindings.Add(xbbindingByLayer);
						this.FindXBBindingsByShift(xbbindingByLayer, destXbBindings);
					}
				}
				item.GetJumpToCurrentLayer().ForEach(delegate(int layer)
				{
					XBBinding xbbindingByLayer2 = item.GetXBBindingByLayer(layer, false, false);
					ActivatorXBBinding activatorXBBinding = xbbindingByLayer2.ActivatorXBBindingDictionary[item.ActivatorType];
					if (xbbindingByLayer2 != null && (!activatorXBBinding.IsShiftToggle || !item.IsToggle) && ((activatorXBBinding.IsShiftToggle && !item.IsToggle) || (activatorXBBinding.IsDelayBerforeJumpChecked && !activatorXBBinding.IsPostponeMapping)) && !destXbBindings.Contains(xbbindingByLayer2))
					{
						destXbBindings.Add(xbbindingByLayer2);
						this.FindXBBindingsByShift(xbbindingByLayer2, destXbBindings);
					}
				});
			});
		}

		public void CopyToAnotherShifts()
		{
			if (XBBinding._copyToAnotherShiftsInProgress)
			{
				return;
			}
			XBBinding._copyToAnotherShiftsInProgress = true;
			List<XBBinding> list = new List<XBBinding>();
			this.FindXBBindingsByShift(this, list);
			list.Remove(this);
			list.ForEach(delegate(XBBinding item)
			{
				this.CopyXBBindingMapping(this, item);
			});
			XBBinding._copyToAnotherShiftsInProgress = false;
		}

		private void CopyRemapToAnotherShifts()
		{
			this.SingleActivator.GetJumpToCurrentLayer().ForEach(delegate(int layer)
			{
				XBBinding xbbindingByLayer = this.SingleActivator.GetXBBindingByLayer(layer, false, false);
				if (xbbindingByLayer != null)
				{
					xbbindingByLayer.RemapedTo = this.RemapedTo;
				}
			});
		}

		private void CopyXBBindingMapping(XBBinding source, XBBinding dest)
		{
			dest.ActivatorXBBindingDictionary.ForEachValue(delegate(ActivatorXBBinding activatorXBBinding)
			{
				activatorXBBinding.CopyMappingFrom(source.ActivatorXBBindingDictionary[activatorXBBinding.ActivatorType]);
			});
			dest.SetButtonsFromAnotherInstance(this);
			dest.RemapedTo = this.RemapedTo;
			dest.FeatureFlags = this.FeatureFlags;
		}

		public IGamepadService GamepadService
		{
			get
			{
				return App.GamepadService;
			}
		}

		public void RemoveKeyBindingInActivator(ActivatorType activatorType)
		{
			ActivatorXBBinding activatorXBBinding = this.ActivatorXBBindingDictionary[activatorType];
			MacroSequence macroSequence = activatorXBBinding.MacroSequence;
			if (macroSequence != null && macroSequence.Count > 0)
			{
				activatorXBBinding.ClearMacroSequence();
				activatorXBBinding.IsTurbo = false;
				activatorXBBinding.IsToggle = false;
			}
			else
			{
				activatorXBBinding.MappedKey = KeyScanCodeV2.NoMap;
			}
			this.HostCollection.SubConfigData.ConfigData.IsChanged = true;
		}

		public void RemoveKeyBinding()
		{
			this.RemoveKeyBindingInActivator(this.CurrentActivatorXBBinding.ActivatorType);
		}

		public GamepadButtonDescription RemapedTo
		{
			get
			{
				return this._remapedTo;
			}
			set
			{
				if (this.SetProperty<GamepadButtonDescription>(ref this._remapedTo, value, "RemapedTo"))
				{
					this.CopyRemapToAnotherShifts();
					this.OnPropertyChanged("IsRemaped");
					this.OnPropertyChanged("IsRemapShouldBeShown");
					this.OnPropertyChanged("IsUnmapped");
					this.OnPropertyChanged("IsRemapedOrUnmapped");
					this.OnPropertyChanged("IsRemapedOrUnmappedShouldBeShown");
					this.OnPropertyChanged("IsAnnotationShouldBeShownForMapping");
					ActivatorXBBinding currentActivatorXBBinding = this.CurrentActivatorXBBinding;
					if (currentActivatorXBBinding != null)
					{
						currentActivatorXBBinding.FirePropertyChanged("IsTurboOrToggleCanBeEnabled");
					}
					ActivatorXBBinding currentActivatorXBBinding2 = this.CurrentActivatorXBBinding;
					if (currentActivatorXBBinding2 != null)
					{
						currentActivatorXBBinding2.FirePropertyChanged("IsTurboOrTogglePeripheralCheck");
					}
					if (base.GamepadButton == 163)
					{
						BaseControllerVM currentGamepad = App.GamepadServiceLazy.Value.CurrentGamepad;
						bool flag;
						if (currentGamepad == null)
						{
							flag = false;
						}
						else
						{
							ControllerVM currentController = currentGamepad.CurrentController;
							bool? flag2 = ((currentController != null) ? new bool?(ControllerTypeExtensions.IsGamepadWithSonyTouchpad(currentController.ControllerType)) : null);
							bool flag3 = true;
							flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
						}
						if (flag)
						{
							BaseXBBindingCollection hostCollection = this.HostCollection;
							if (((hostCollection != null) ? hostCollection.Touchpad1DirectionalGroup : null) != null)
							{
								this.HostCollection.SubConfigData.MainXBBindingCollection.Touchpad1DirectionalGroup.CructhForFirePropertyChanged = !this.HostCollection.SubConfigData.MainXBBindingCollection.Touchpad1DirectionalGroup.CructhForFirePropertyChanged;
							}
						}
					}
				}
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool IsInheritedBinding
		{
			get
			{
				return this._isInheritedBinding;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isInheritedBinding, value, "IsInheritedBinding"))
				{
					this.ActivatorXBBindingDictionary.IsInheritedBinding = value;
				}
				this.OnPropertyChanged("IsAnnotationShouldBeShownForMapping");
			}
		}

		public bool HasMacroRumble
		{
			get
			{
				return this.CurrentActivatorXBBinding.HasMacroRumble;
			}
		}

		public bool IsRemaped
		{
			get
			{
				bool flag = !this.IsUnmapped && !this.RemapedTo.Equals(base.GamepadButtonDescription);
				if (flag)
				{
					BaseControllerVM currentGamepad = App.GamepadServiceLazy.Value.CurrentGamepad;
					bool flag2;
					if (currentGamepad == null)
					{
						flag2 = false;
					}
					else
					{
						ControllerVM currentController = currentGamepad.CurrentController;
						bool? flag3 = ((currentController != null) ? new bool?(ControllerTypeExtensions.IsGamepadNativeMappingNotAvailiable(currentController.ControllerType)) : null);
						bool flag4 = true;
						flag2 = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
					}
					if (flag2 && !GamepadButtonExtensions.IsAnyStickDirection(this.RemapedTo.Button))
					{
						flag = false;
					}
				}
				return flag;
			}
		}

		public bool IsRemapShouldBeShown
		{
			get
			{
				BaseControllerVM currentGamepad = App.GamepadServiceLazy.Value.CurrentGamepad;
				ControllerTypeEnum? controllerTypeEnum;
				if (currentGamepad == null)
				{
					controllerTypeEnum = null;
				}
				else
				{
					ControllerVM currentController = currentGamepad.CurrentController;
					controllerTypeEnum = ((currentController != null) ? new ControllerTypeEnum?(currentController.ControllerType) : null);
				}
				ControllerTypeEnum? controllerTypeEnum2 = controllerTypeEnum;
				if (((controllerTypeEnum2 != null && ControllerTypeExtensions.IsAzeron(controllerTypeEnum2.GetValueOrDefault())) || (controllerTypeEnum2 != null && ControllerTypeExtensions.IsAzeronCyborg(controllerTypeEnum2.GetValueOrDefault())) || (controllerTypeEnum2 != null && ControllerTypeExtensions.IsAnySteam(controllerTypeEnum2.GetValueOrDefault())) || (controllerTypeEnum2 != null && ControllerTypeExtensions.IsFlydigi(controllerTypeEnum2.GetValueOrDefault())) || (controllerTypeEnum2 != null && ControllerTypeExtensions.IsAnyEngineGamepad(controllerTypeEnum2.GetValueOrDefault()))) && !GamepadButtonExtensions.IsAnyStickDirection(this.RemapedTo.Button))
				{
					return false;
				}
				BaseControllerVM currentGamepad2 = App.GamepadServiceLazy.Value.CurrentGamepad;
				bool flag;
				if (currentGamepad2 == null)
				{
					flag = false;
				}
				else
				{
					ControllerVM currentController2 = currentGamepad2.CurrentController;
					bool? flag2 = ((currentController2 != null) ? new bool?(ControllerTypeExtensions.IsXboxOrRazerWolverine2(currentController2.ControllerType)) : null);
					bool flag3 = true;
					flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
				if (flag && base.ControllerButton.IsGamepad && base.ControllerButton.GamepadButton == 11)
				{
					return false;
				}
				BaseControllerVM currentGamepad3 = App.GamepadServiceLazy.Value.CurrentGamepad;
				bool flag4;
				if (currentGamepad3 == null)
				{
					flag4 = false;
				}
				else
				{
					ControllerVM currentController3 = currentGamepad3.CurrentController;
					bool? flag2 = ((currentController3 != null) ? new bool?(ControllerTypeExtensions.IsGameSirG7(currentController3.ControllerType)) : null);
					bool flag3 = true;
					flag4 = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
				if (flag4 && base.ControllerButton.IsGamepad && (base.ControllerButton.GamepadButton == 12 || base.ControllerButton.GamepadButton == 13 || base.ControllerButton.GamepadButton == 14 || base.ControllerButton.GamepadButton == 29 || base.ControllerButton.GamepadButton == 30))
				{
					return false;
				}
				BaseControllerVM currentGamepad4 = App.GamepadServiceLazy.Value.CurrentGamepad;
				bool flag5;
				if (currentGamepad4 == null)
				{
					flag5 = false;
				}
				else
				{
					ControllerVM currentController4 = currentGamepad4.CurrentController;
					bool? flag2 = ((currentController4 != null) ? new bool?(ControllerTypeExtensions.IsSonyDualSenseEdge(currentController4.ControllerType)) : null);
					bool flag3 = true;
					flag5 = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
				return (!flag5 || !base.ControllerButton.IsGamepad || (base.ControllerButton.GamepadButton != 14 && base.ControllerButton.GamepadButton != 15 && base.ControllerButton.GamepadButton != 16 && base.ControllerButton.GamepadButton != 17)) && this.IsUnmapShouldBeShown;
			}
		}

		public bool IsUnmapped
		{
			get
			{
				bool flag = this.RemapedTo == null || this.RemapedTo.Equals(GamepadButtonDescription.Unmapped);
				if (flag)
				{
					BaseControllerVM currentGamepad = App.GamepadServiceLazy.Value.CurrentGamepad;
					bool flag2;
					if (currentGamepad == null)
					{
						flag2 = false;
					}
					else
					{
						ControllerVM currentController = currentGamepad.CurrentController;
						bool? flag3 = ((currentController != null) ? new bool?(ControllerTypeExtensions.IsUnmapAvailable(currentController.ControllerType, base.GamepadButton)) : null);
						bool flag4 = false;
						flag2 = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
					}
					if (flag2)
					{
						flag = false;
					}
				}
				return flag;
			}
		}

		public bool IsUnmapShouldBeShown
		{
			get
			{
				BaseControllerVM currentGamepad = App.GamepadServiceLazy.Value.CurrentGamepad;
				ControllerTypeEnum? controllerTypeEnum;
				if (currentGamepad == null)
				{
					controllerTypeEnum = null;
				}
				else
				{
					ControllerVM currentController = currentGamepad.CurrentController;
					controllerTypeEnum = ((currentController != null) ? new ControllerTypeEnum?(currentController.ControllerType) : null);
				}
				ControllerTypeEnum? controllerTypeEnum2 = controllerTypeEnum;
				if (((controllerTypeEnum2 != null && ControllerTypeExtensions.IsAzeron(controllerTypeEnum2.GetValueOrDefault())) || (controllerTypeEnum2 != null && ControllerTypeExtensions.IsAzeronCyborg(controllerTypeEnum2.GetValueOrDefault())) || (((controllerTypeEnum2 != null && ControllerTypeExtensions.IsAnySteam(controllerTypeEnum2.GetValueOrDefault())) || (controllerTypeEnum2 != null && ControllerTypeExtensions.IsFlydigi(controllerTypeEnum2.GetValueOrDefault())) || (controllerTypeEnum2 != null && ControllerTypeExtensions.IsEngineControllerControlPad(controllerTypeEnum2.GetValueOrDefault()))) && !this.IsUnmapped)) && !GamepadButtonExtensions.IsAnyStickDirection(this.RemapedTo.Button))
				{
					return false;
				}
				BaseControllerVM currentGamepad2 = App.GamepadServiceLazy.Value.CurrentGamepad;
				bool flag;
				if (currentGamepad2 == null)
				{
					flag = false;
				}
				else
				{
					ControllerVM currentController2 = currentGamepad2.CurrentController;
					bool? flag2 = ((currentController2 != null) ? new bool?(ControllerTypeExtensions.IsXboxOrRazerWolverine2(currentController2.ControllerType)) : null);
					bool flag3 = true;
					flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
				if (flag && base.ControllerButton.IsGamepad && base.ControllerButton.GamepadButton == 11 && !this.IsUnmapped)
				{
					return false;
				}
				BaseControllerVM currentGamepad3 = App.GamepadServiceLazy.Value.CurrentGamepad;
				bool flag4;
				if (currentGamepad3 == null)
				{
					flag4 = false;
				}
				else
				{
					ControllerVM currentController3 = currentGamepad3.CurrentController;
					bool? flag2 = ((currentController3 != null) ? new bool?(ControllerTypeExtensions.IsGameSirG7(currentController3.ControllerType)) : null);
					bool flag3 = true;
					flag4 = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
				if (flag4 && base.ControllerButton.IsGamepad && (base.ControllerButton.GamepadButton == 12 || base.ControllerButton.GamepadButton == 13 || base.ControllerButton.GamepadButton == 14 || base.ControllerButton.GamepadButton == 29 || base.ControllerButton.GamepadButton == 30) && !this.IsUnmapped)
				{
					return false;
				}
				BaseControllerVM currentGamepad4 = App.GamepadServiceLazy.Value.CurrentGamepad;
				bool flag5;
				if (currentGamepad4 == null)
				{
					flag5 = false;
				}
				else
				{
					ControllerVM currentController4 = currentGamepad4.CurrentController;
					bool? flag2 = ((currentController4 != null) ? new bool?(ControllerTypeExtensions.IsSonyDualSenseEdge(currentController4.ControllerType)) : null);
					bool flag3 = true;
					flag5 = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
				return (!flag5 || !base.ControllerButton.IsGamepad || (base.ControllerButton.GamepadButton != 14 && base.ControllerButton.GamepadButton != 15 && base.ControllerButton.GamepadButton != 16 && base.ControllerButton.GamepadButton != 17) || this.IsUnmapped) && (GamepadButtonExtensions.IsAnyStickDirection(this.RemapedTo.Button) || App.KeyBindingService.ButtonsToRemap.Contains(this.RemapedTo));
			}
		}

		public bool IsRemapedOrUnmapped
		{
			get
			{
				return this.IsRemaped || this.IsUnmapped;
			}
		}

		public bool IsRemapedOrUnmappedShouldBeShown
		{
			get
			{
				return this.IsRemapedOrUnmapped && (this.IsRemapShouldBeShown || this.IsUnmapShouldBeShown);
			}
		}

		public bool IsAnnotationShouldBeShownForMapping
		{
			get
			{
				ActivatorXBBindingDictionary activatorXBBindingDictionary = this.ActivatorXBBindingDictionary;
				bool flag;
				if (activatorXBBindingDictionary == null)
				{
					flag = false;
				}
				else
				{
					flag = activatorXBBindingDictionary.AnyValue((ActivatorXBBinding kvp) => kvp.IsAnnotationShouldBeShownForMapping);
				}
				return flag || this.IsUnmapped || (this.IsRemaped && this.IsRemapShouldBeShown);
			}
		}

		public bool IsAnnotationShouldBeShownForDescription
		{
			get
			{
				ActivatorXBBindingDictionary activatorXBBindingDictionary = this.ActivatorXBBindingDictionary;
				if (activatorXBBindingDictionary == null)
				{
					return false;
				}
				return activatorXBBindingDictionary.AnyValue((ActivatorXBBinding kvp) => kvp.IsAnnotationShouldBeShownForDescription);
			}
		}

		public bool IsContainsJumpToShift
		{
			get
			{
				ActivatorXBBindingDictionary activatorXBBindingDictionary = this.ActivatorXBBindingDictionary;
				if (activatorXBBindingDictionary == null)
				{
					return false;
				}
				return activatorXBBindingDictionary.AnyValue((ActivatorXBBinding kvp) => kvp.IsJumpToShift);
			}
		}

		public bool IsGroupForCopyClear
		{
			get
			{
				return this.IsGyroTilt || this.IsLeftRightStick;
			}
		}

		public bool IsMouseDirection
		{
			get
			{
				return GamepadButtonExtensions.IsMouseDirection(base.GamepadButton);
			}
		}

		public bool IsTrigger
		{
			get
			{
				return GamepadButtonExtensions.IsAnyTrigger(base.GamepadButton);
			}
		}

		public bool IsTriggerZone
		{
			get
			{
				return GamepadButtonExtensions.IsAnyTriggerZone(base.GamepadButton);
			}
		}

		public bool IsTriggerPress
		{
			get
			{
				return GamepadButtonExtensions.IsAnyTriggerPress(base.GamepadButton);
			}
		}

		public bool IsLeftTrigger
		{
			get
			{
				return GamepadButtonExtensions.IsLeftTrigger(base.GamepadButton);
			}
		}

		public bool IsRightTrigger
		{
			get
			{
				return GamepadButtonExtensions.IsRightTrigger(base.GamepadButton);
			}
		}

		public bool IsLeftTriggerPress
		{
			get
			{
				return GamepadButtonExtensions.IsLeftTriggerPress(base.GamepadButton);
			}
		}

		public bool IsRightTriggerPress
		{
			get
			{
				return GamepadButtonExtensions.IsRightTriggerPress(base.GamepadButton);
			}
		}

		public bool IsLeftTriggerZone
		{
			get
			{
				return GamepadButtonExtensions.IsLeftTriggerZone(base.GamepadButton);
			}
		}

		public bool IsRightTriggerZone
		{
			get
			{
				return GamepadButtonExtensions.IsRightTriggerZone(base.GamepadButton);
			}
		}

		public bool IsPaddle
		{
			get
			{
				return GamepadButtonExtensions.IsAnyPaddle(base.GamepadButton);
			}
		}

		public bool IsDigital
		{
			get
			{
				return GamepadButtonExtensions.IsDigital(base.GamepadButton);
			}
		}

		public bool IsJumpToLayerAllowed
		{
			get
			{
				if (GamepadButtonExtensions.IsDigital(base.GamepadButton) || GamepadButtonExtensions.IsAnyTriggerPress(base.GamepadButton))
				{
					BaseControllerVM currentGamepad = App.GamepadServiceLazy.Value.CurrentGamepad;
					bool flag;
					if (currentGamepad == null)
					{
						flag = false;
					}
					else
					{
						ControllerVM currentController = currentGamepad.CurrentController;
						bool? flag2 = ((currentController != null) ? new bool?(ControllerTypeExtensions.IsAnySteam(currentController.ControllerType)) : null);
						bool flag3 = true;
						flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
					}
					return !flag || !GamepadButtonExtensions.IsAnyPhysicalTrackPadDirection(base.GamepadButton);
				}
				return false;
			}
		}

		public bool IsAnalog
		{
			get
			{
				return GamepadButtonExtensions.IsAnalog(base.GamepadButton);
			}
		}

		public bool IsLeftStick
		{
			get
			{
				return GamepadButtonExtensions.IsLeftStick(base.GamepadButton);
			}
		}

		public bool IsRightStick
		{
			get
			{
				return GamepadButtonExtensions.IsRightStick(base.GamepadButton);
			}
		}

		public bool IsAdditionalStick
		{
			get
			{
				return GamepadButtonExtensions.IsAdditionalStick(base.GamepadButton);
			}
		}

		public bool IsMouseStick
		{
			get
			{
				return GamepadButtonExtensions.IsMouseStick(base.GamepadButton);
			}
		}

		public bool IsMouseScrolls
		{
			get
			{
				return GamepadButtonExtensions.IsMouseScroll(base.GamepadButton);
			}
		}

		public bool IsDPAD
		{
			get
			{
				return GamepadButtonExtensions.IsDPAD(base.GamepadButton);
			}
		}

		public bool IsMouseStickOrScrolls
		{
			get
			{
				return GamepadButtonExtensions.IsMouseStick(base.GamepadButton) || GamepadButtonExtensions.IsMouseScroll(base.GamepadButton);
			}
		}

		public bool IsStick
		{
			get
			{
				return GamepadButtonExtensions.IsAnyStick(base.GamepadButton);
			}
		}

		public bool IsLeftRightStick
		{
			get
			{
				return GamepadButtonExtensions.IsLeftStick(base.GamepadButton) || GamepadButtonExtensions.IsRightStick(base.GamepadButton);
			}
		}

		public bool IsDS3AnalogZone
		{
			get
			{
				return GamepadButtonExtensions.IsDS3AnalogZone(base.GamepadButton);
			}
		}

		public bool IsLeftStickDirection
		{
			get
			{
				return GamepadButtonExtensions.IsLeftStickDirection(base.GamepadButton);
			}
		}

		public bool IsRightStickDirection
		{
			get
			{
				return GamepadButtonExtensions.IsRightStickDirection(base.GamepadButton);
			}
		}

		public bool IsLeftStickZone
		{
			get
			{
				return GamepadButtonExtensions.IsLeftStickZone(base.GamepadButton);
			}
		}

		public bool IsRightStickZone
		{
			get
			{
				return GamepadButtonExtensions.IsRightStickZone(base.GamepadButton);
			}
		}

		public bool IsStickZone
		{
			get
			{
				return GamepadButtonExtensions.IsAnyStickZone(base.GamepadButton);
			}
		}

		public bool IsStickDirection
		{
			get
			{
				return GamepadButtonExtensions.IsAnyStickDirection(base.GamepadButton);
			}
		}

		public bool IsAnyGyroTiltDirection
		{
			get
			{
				return GamepadButtonExtensions.IsAnyGyroTiltDirection(base.GamepadButton);
			}
		}

		public bool IsAnyPhysicalTrackPadDirection
		{
			get
			{
				return GamepadButtonExtensions.IsAnyPhysicalTrackPadDirection(base.GamepadButton);
			}
		}

		public bool IsAnyPhysicalTrackPadDigitalDirection
		{
			get
			{
				return GamepadButtonExtensions.IsAnyPhysicalTrackPadDigitalDirection(base.GamepadButton);
			}
		}

		public bool IsPhysicalTrackPad1Direction
		{
			get
			{
				return GamepadButtonExtensions.IsPhysicalTrackPad1Direction(base.GamepadButton);
			}
		}

		public bool IsPhysicalTrackPad2Direction
		{
			get
			{
				return GamepadButtonExtensions.IsPhysicalTrackPad2Direction(base.GamepadButton);
			}
		}

		public bool IsGyroLean
		{
			get
			{
				return GamepadButtonExtensions.IsGyroLean(base.GamepadButton);
			}
		}

		public bool IsGyroTilt
		{
			get
			{
				return GamepadButtonExtensions.IsGyroTilt(base.GamepadButton);
			}
		}

		public bool IsGyroTiltDirection
		{
			get
			{
				return GamepadButtonExtensions.IsGyroTiltDirection(base.GamepadButton);
			}
		}

		public bool IsTiltDirection
		{
			get
			{
				return GamepadButtonExtensions.IsTiltDirection(base.GamepadButton);
			}
		}

		public bool IsGyroTiltZone
		{
			get
			{
				return GamepadButtonExtensions.IsGyroTiltZone(base.GamepadButton);
			}
		}

		public bool IsPhysicalTrackPad1
		{
			get
			{
				return GamepadButtonExtensions.IsPhysicalTrackPad1(base.GamepadButton);
			}
		}

		public bool IsPhysicalTrackPad2
		{
			get
			{
				return GamepadButtonExtensions.IsPhysicalTrackPad2(base.GamepadButton);
			}
		}

		public Drawing XBButtonImage
		{
			get
			{
				return base.ControllerButton.XBButtonImage;
			}
		}

		public BaseXBBindingCollection HostCollection { get; set; }

		public MaskItem HostMaskItem
		{
			get
			{
				return this._hostMaskItem;
			}
			set
			{
				this._hostMaskItem = value;
				this.ActivatorXBBindingDictionary.HostXBBinding = this;
			}
		}

		public bool IsOverlaySector { get; set; }

		public ControllerBinding HostControllerBinding { get; set; }

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
			if (activatorType == null)
			{
				return;
			}
			this.SwitchCurrentActivator(activatorType.Value);
		}

		public void SwitchCurrentActivator(ActivatorType activatorType)
		{
			this.CurrentActivatorXBBinding = this.ActivatorXBBindingDictionary.TryGetValue(activatorType);
		}

		public XBBinding(BaseXBBindingCollection xbCol)
			: this(xbCol, 2001)
		{
		}

		public XBBinding(BaseXBBindingCollection xbCol, GamepadButton gamepadButton)
			: this(xbCol, GamepadButtonDescription.GamepadButtonDescriptionDictionary[gamepadButton], null)
		{
		}

		public XBBinding(BaseXBBindingCollection xbCol, GamepadButtonDescription gamepadButton, GamepadButtonDescription remapedTo = null)
		{
			this.HostCollection = xbCol;
			base.ControllerButton.GamepadButtonDescription = gamepadButton;
			if (remapedTo == null)
			{
				this._remapedTo = gamepadButton;
			}
			else
			{
				this._remapedTo = remapedTo;
			}
			this.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(base.ControllerButton);
			this.FeatureFlags = 0U;
			this.MaskConditions.CollectionChanged += this.MaskConditions_CollectionChanged;
			base.ApplyChanges();
		}

		private void MaskConditions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				this.ResetJumpToShift();
			}
			this.OnPropertyChanged("MaskConditionsHasZones");
		}

		public XBBinding(BaseXBBindingCollection xbCol, KeyScanCodeV2 keyScanCode)
			: this(xbCol, 2001)
		{
			base.ControllerButton.KeyScanCode = keyScanCode;
			base.ApplyChanges();
		}

		public override void Dispose()
		{
			base.Dispose();
			if (this.MaskConditions != null)
			{
				this.MaskConditions.Clear();
			}
			this._currentActivatorXBBinding = null;
			if (this._activatorXBBindingDictionary != null)
			{
				this.ActivatorXBBindingDictionary.Dispose();
				this.ActivatorXBBindingDictionary = null;
			}
			this.HostCollection = null;
			base.ControllerButton = null;
		}

		protected override void OnBeforeControllerButtonChanged(object s, PropertyChangedExtendedEventArgs e)
		{
			KeyScanCodeV2 keyScanCodeV = e.OldValue as KeyScanCodeV2;
			if (keyScanCodeV != null && keyScanCodeV != KeyScanCodeV2.NoMap)
			{
				this.ResetJumpToShift();
			}
		}

		protected override void OnControllerButtonChanged(object s, PropertyChangedExtendedEventArgs e)
		{
			base.OnControllerButtonChanged(s, e);
			if (base.ControllerButton.IsSet)
			{
				this.MaskConditions.SetItemAt(0, base.ControllerButton);
				if (base.ControllerButton.IsGamepad && GamepadButtonExtensions.IsMouseDirection(base.ControllerButton.GamepadButton))
				{
					this.IsDisableInheritVirtualMapFromMain = true;
				}
			}
			else if (this.MaskConditions.Count > 0)
			{
				this.MaskConditions.Clear();
			}
			this.OnPropertyChanged("IsRemaped");
			this.OnPropertyChanged("XBButtonImage");
			ActivatorXBBinding currentActivatorXBBinding = this.CurrentActivatorXBBinding;
			if (currentActivatorXBBinding != null)
			{
				currentActivatorXBBinding.FirePropertyChanged("IsTurboOrToggleCanBeEnabled");
			}
			ActivatorXBBinding currentActivatorXBBinding2 = this.CurrentActivatorXBBinding;
			if (currentActivatorXBBinding2 != null)
			{
				currentActivatorXBBinding2.FirePropertyChanged("IsToggleCanBeEnabled");
			}
			ActivatorXBBinding currentActivatorXBBinding3 = this.CurrentActivatorXBBinding;
			if (currentActivatorXBBinding3 == null)
			{
				return;
			}
			currentActivatorXBBinding3.FirePropertyChanged("IsTurboOrTogglePeripheralCheck");
		}

		public void ResetJumpToShift()
		{
			ActivatorXBBindingDictionary activatorXBBindingDictionary = this.ActivatorXBBindingDictionary;
			if (activatorXBBindingDictionary == null)
			{
				return;
			}
			activatorXBBindingDictionary.ForEachValue(delegate(ActivatorXBBinding axb)
			{
				axb.ResetJumpToShift();
			});
		}

		public void NullJumpToShift()
		{
			ActivatorXBBindingDictionary activatorXBBindingDictionary = this.ActivatorXBBindingDictionary;
			if (activatorXBBindingDictionary == null)
			{
				return;
			}
			activatorXBBindingDictionary.ForEachValue(delegate(ActivatorXBBinding axb)
			{
				axb.NullJumpToShift();
			});
		}

		public void TryShowKeyboardMappingUnmapWarning(ActivatorXBBinding axb)
		{
			bool flag;
			if (base.GamepadButton == 163 || base.GamepadButton == 99)
			{
				BaseControllerVM currentGamepad = App.GamepadServiceLazy.Value.CurrentGamepad;
				if (currentGamepad == null)
				{
					flag = false;
				}
				else
				{
					ControllerVM currentController = currentGamepad.CurrentController;
					bool? flag2 = ((currentController != null) ? new bool?(ControllerTypeExtensions.IsNVidiaShield2015(currentController.ControllerType)) : null);
					bool flag3 = true;
					flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
			}
			else
			{
				flag = false;
			}
			bool flag4 = flag;
			if ((!base.ControllerButton.IsKeyScanCode && !GamepadButtonExtensions.IsMouseScroll(base.ControllerButton.GamepadButton) && !flag4) || !base.ControllerButton.IsUnmapAvailiable)
			{
				return;
			}
			List<ActivatorXBBinding> list = new List<ActivatorXBBinding>(axb.HostDictionary.Values);
			list.RemoveAll((ActivatorXBBinding ab) => ab.ActivatorType == axb.ActivatorType);
			if (list.Any((ActivatorXBBinding ac) => ac.IsVirtualMappingPresentSkipShift))
			{
				return;
			}
			if (!this.IsUnmapped && this.HostCollection.SubConfigData.ConfigData.ConfigVM.IsEditConfigMode)
			{
				string text = (flag4 ? DTLocalization.GetString(12145) : DTLocalization.GetString(11568));
				if (MessageBoxWithRememberMyChoiceLogic.Show(Application.Current.MainWindow, text, MessageBoxButton.YesNo, MessageBoxImage.Asterisk, flag4 ? "ConfirmUnmapTouchpad" : "ConfirmUnmapForKeyboardMapping", "GuiNamespace", flag4 ? "UnmapTouchpad" : "UnmapForKeyboardMapping", false, 0.0, null, null, null, null, null) == MessageBoxResult.Yes)
				{
					this.RemapedTo = GamepadButtonDescription.Unmapped;
					this.GetJumpToCurrentLayer().ForEach(delegate(int layer)
					{
						this.SingleActivator.GetXBBindingByLayer(layer, true, false).RemapedTo = GamepadButtonDescription.Unmapped;
					});
				}
			}
		}

		public void TryShowKeyboardMappingUnmapWarning(ActivatorXBBinding axb, KeyScanCodeV2 oldMappedKey)
		{
			if (oldMappedKey != KeyScanCodeV2.NoMap && oldMappedKey != KeyScanCodeV2.NoInheritance)
			{
				return;
			}
			this.TryShowKeyboardMappingUnmapWarning(axb);
		}

		private void OnActivatorDictionaryValueSet(object sender, ActivatorType activatorType)
		{
			if (this.CurrentActivatorXBBinding.ActivatorType == activatorType)
			{
				this.CurrentActivatorXBBinding = this.ActivatorXBBindingDictionary.TryGetValue(activatorType);
			}
		}

		public void RevertRemap()
		{
			this.RemapedTo = base.GamepadButtonDescription;
		}

		public bool CanSave(bool verbose, ref bool errorIsShown)
		{
			bool localErrorIsShown = errorIsShown;
			bool success = true;
			this.ActivatorXBBindingDictionary.ForEachValue(delegate(ActivatorXBBinding v)
			{
				success = v.CanSave(verbose, ref localErrorIsShown) & success;
			});
			errorIsShown = localErrorIsShown;
			return success;
		}

		public bool SaveChanges(bool verbose, ref bool errorIsShown)
		{
			bool localErrorIsShown = errorIsShown;
			bool success = true;
			this.ActivatorXBBindingDictionary.ForEachValue(delegate(ActivatorXBBinding v)
			{
				success = v.SaveChanges(verbose, ref localErrorIsShown) & success;
			});
			errorIsShown = localErrorIsShown;
			return success;
		}

		public void PrepareForSave()
		{
			this.ActivatorXBBindingDictionary.ForEachValue(delegate(ActivatorXBBinding v)
			{
				v.MacroSequence.PrepareForSave();
			});
		}

		public void RefreshRemappedTo()
		{
			this.OnPropertyChanged("RemapedTo");
		}

		public bool IsNeedSave
		{
			get
			{
				return !this.IsEmpty || this._mouseScrollDelta != 2;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return !this.IsRemapedOrUnmapped && !this.IsAnyActivatorVirtualMappingPresent && !this.IsAnyActivatorDescriptionPresent && this._mouseScrollDelta == 2;
			}
		}

		public bool IsEmptySkipShift
		{
			get
			{
				return !this.IsRemapedOrUnmapped && !this.IsAnyActivatorVirtualMappingPresentSkipShift && !this.IsAnyActivatorDescriptionPresent && this._mouseScrollDelta == 2;
			}
		}

		public bool IsSingleActivatorVirtualMappingPresent
		{
			get
			{
				ActivatorXBBinding singleActivator = this.SingleActivator;
				return singleActivator != null && singleActivator.IsVirtualMappingPresent;
			}
		}

		public bool IsCurrentActivatorDescriptionPresent
		{
			get
			{
				ActivatorXBBinding currentActivatorXBBinding = this.CurrentActivatorXBBinding;
				return currentActivatorXBBinding != null && currentActivatorXBBinding.IsDescriptionPresent;
			}
		}

		public bool IsCurrentActivatorVirtualMappingPresent
		{
			get
			{
				ActivatorXBBinding currentActivatorXBBinding = this.CurrentActivatorXBBinding;
				return currentActivatorXBBinding != null && currentActivatorXBBinding.IsVirtualMappingPresent;
			}
		}

		public bool IsCurrentActivatorMouseMappingToTrackpadPresent
		{
			get
			{
				ActivatorXBBinding currentActivatorXBBinding = this.CurrentActivatorXBBinding;
				return currentActivatorXBBinding != null && currentActivatorXBBinding.IsMouseToTrackpad;
			}
		}

		public bool IsCurrentActivatorOverlayRadialMenuToTrackpadPresent
		{
			get
			{
				ActivatorXBBinding currentActivatorXBBinding = this.CurrentActivatorXBBinding;
				return currentActivatorXBBinding != null && currentActivatorXBBinding.IsOverlayRadialMenuToTrackpad;
			}
		}

		public bool IsCurrentActivatorFlickStickMappingToTrackpadPresent
		{
			get
			{
				ActivatorXBBinding currentActivatorXBBinding = this.CurrentActivatorXBBinding;
				return currentActivatorXBBinding != null && currentActivatorXBBinding.IsFlickStickToTrackpad;
			}
		}

		public bool IsAnyActivatorDescriptionPresent
		{
			get
			{
				ActivatorXBBindingDictionary activatorXBBindingDictionary = this.ActivatorXBBindingDictionary;
				if (activatorXBBindingDictionary == null)
				{
					return false;
				}
				return activatorXBBindingDictionary.AnyValue((ActivatorXBBinding kvp) => kvp.IsDescriptionPresent);
			}
		}

		public bool IsAnyActivatorVirtualMappingPresent
		{
			get
			{
				ActivatorXBBindingDictionary activatorXBBindingDictionary = this.ActivatorXBBindingDictionary;
				if (activatorXBBindingDictionary == null)
				{
					return false;
				}
				return activatorXBBindingDictionary.AnyValue((ActivatorXBBinding kvp) => kvp.IsVirtualMappingPresent);
			}
		}

		public bool IsAnyActivatorVirtualMappingPresentSkipShift
		{
			get
			{
				ActivatorXBBindingDictionary activatorXBBindingDictionary = this.ActivatorXBBindingDictionary;
				if (activatorXBBindingDictionary == null)
				{
					return false;
				}
				return activatorXBBindingDictionary.AnyValue((ActivatorXBBinding kvp) => kvp.IsVirtualMappingPresentSkipShift);
			}
		}

		public bool IsAnyNonSingleActivatorVirtualMappingPresent
		{
			get
			{
				ActivatorXBBindingDictionary activatorXBBindingDictionary = this.ActivatorXBBindingDictionary;
				if (activatorXBBindingDictionary == null)
				{
					return false;
				}
				return activatorXBBindingDictionary.Any((KeyValuePair<ActivatorType, ActivatorXBBinding> kvp) => kvp.Value.IsVirtualMappingPresent && kvp.Key > 0);
			}
		}

		public bool IsAnyActivatorVirtualGamepadMappingPresent
		{
			get
			{
				ActivatorXBBindingDictionary activatorXBBindingDictionary = this.ActivatorXBBindingDictionary;
				if (activatorXBBindingDictionary == null)
				{
					return false;
				}
				return activatorXBBindingDictionary.AnyValue((ActivatorXBBinding kvp) => kvp.IsGamepadMacroMappingPresent);
			}
		}

		public bool IsAnyActivatorMacroMappingPresent
		{
			get
			{
				ActivatorXBBindingDictionary activatorXBBindingDictionary = this.ActivatorXBBindingDictionary;
				if (activatorXBBindingDictionary == null)
				{
					return false;
				}
				return activatorXBBindingDictionary.AnyValue((ActivatorXBBinding kvp) => kvp.IsMacroMapping);
			}
		}

		public bool IsAnyActivatorTurboTogglePresent
		{
			get
			{
				ActivatorXBBindingDictionary activatorXBBindingDictionary = this.ActivatorXBBindingDictionary;
				if (activatorXBBindingDictionary == null)
				{
					return false;
				}
				return activatorXBBindingDictionary.AnyValue((ActivatorXBBinding kvp) => kvp.IsTurbo || kvp.IsToggle);
			}
		}

		public bool IsAnyTurboPresent
		{
			get
			{
				ActivatorXBBindingDictionary activatorXBBindingDictionary = this.ActivatorXBBindingDictionary;
				if (activatorXBBindingDictionary == null)
				{
					return false;
				}
				return activatorXBBindingDictionary.AnyValue((ActivatorXBBinding kvp) => kvp.IsTurbo);
			}
		}

		public bool IsAnyTogglePresent
		{
			get
			{
				ActivatorXBBindingDictionary activatorXBBindingDictionary = this.ActivatorXBBindingDictionary;
				if (activatorXBBindingDictionary == null)
				{
					return false;
				}
				return activatorXBBindingDictionary.AnyValue((ActivatorXBBinding kvp) => kvp.IsToggle);
			}
		}

		public void SetNonEmptyCurrentActivator(bool forced = false)
		{
			if (forced || this.CurrentActivatorXBBinding == null || !this.CurrentActivatorXBBinding.IsVirtualMappingPresent)
			{
				this.CurrentActivatorXBBinding = this.ActivatorXBBindingDictionary.GetNonEmptyActivator();
			}
		}

		public bool EqualsByValues(XBBinding xb)
		{
			return this.EqualsByVirtualMappings(xb) && this.EqualsByHardwareMappings(xb);
		}

		public bool EqualsByVirtualMappings(XBBinding xb)
		{
			if (!object.Equals(base.GamepadButton, xb.GamepadButton))
			{
				return false;
			}
			foreach (KeyValuePair<ActivatorType, ActivatorXBBinding> keyValuePair in ((IEnumerable<KeyValuePair<ActivatorType, ActivatorXBBinding>>)this.ActivatorXBBindingDictionary))
			{
				if (!object.Equals(keyValuePair.Value.IsToggle, xb.ActivatorXBBindingDictionary[keyValuePair.Key].IsToggle))
				{
					return false;
				}
				if (!object.Equals(keyValuePair.Value.IsTurbo, xb.ActivatorXBBindingDictionary[keyValuePair.Key].IsTurbo))
				{
					return false;
				}
				if (!object.Equals(keyValuePair.Value.IsRumble, xb.ActivatorXBBindingDictionary[keyValuePair.Key].IsRumble))
				{
					return false;
				}
				if (!object.Equals(keyValuePair.Value.MappedKey, xb.ActivatorXBBindingDictionary[keyValuePair.Key].MappedKey))
				{
					return false;
				}
				if (!keyValuePair.Value.MacroSequence.SequenceEqual(xb.ActivatorXBBindingDictionary[keyValuePair.Key].MacroSequence))
				{
					return false;
				}
			}
			return true;
		}

		public bool EqualsByHardwareMappings(XBBinding xb)
		{
			return object.Equals(this.RemapedTo, xb.RemapedTo);
		}

		public DelegateCommand RevertRemapCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._revertRemapCommand) == null)
				{
					delegateCommand = (this._revertRemapCommand = new DelegateCommand(new Action(this.RevertRemap)));
				}
				return delegateCommand;
			}
		}

		public DelegateCommand UnmapCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._Unmap) == null)
				{
					delegateCommand = (this._Unmap = new DelegateCommand(new Action(this.ToggleUnmap)));
				}
				return delegateCommand;
			}
		}

		protected void ToggleUnmap()
		{
			if (GamepadButtonDescription.Unmapped.Equals(this.RemapedTo))
			{
				this.RevertRemap();
				return;
			}
			this.RemapedTo = GamepadButtonDescription.Unmapped;
		}

		public void ClearVirtualMappings()
		{
			this.ActivatorXBBindingDictionary.ForEachValue(delegate(ActivatorXBBinding item)
			{
				item.ClearVirtualMapping();
			});
		}

		public XBBinding Clone()
		{
			XBBinding xbbinding = new XBBinding(this.HostCollection);
			xbbinding.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.ActivatorXBBindingDictionary, null);
			xbbinding.SetButtonsFromAnotherInstance(this);
			xbbinding.RemapedTo = this.RemapedTo;
			xbbinding.FeatureFlags = this.FeatureFlags;
			return xbbinding;
		}

		public void CopyFromModel(XBBinding model)
		{
			if (model == null)
			{
				return;
			}
			this.ActivatorXBBindingDictionary.CopyFromModel(model.ActivatorXBBindingDictionary);
			base.ControllerButton.CopyFromModel(model.ControllerButton);
			this.MouseScrollDelta = model.MouseScrollDelta;
			this.RemapedTo = model.RemapedTo;
			this.FeatureFlags = model.FeatureFlags;
		}

		public void CopyToModel(XBBinding model)
		{
			if (model == null)
			{
				return;
			}
			base.ControllerButton.CopyToModel(model.ControllerButton);
			model.MouseScrollDelta = this.MouseScrollDelta;
			model.RemapedTo = this.RemapedTo;
			model.FeatureFlags = this.FeatureFlags;
			this.ActivatorXBBindingDictionary.CopyToModel(model.ActivatorXBBindingDictionary);
		}

		private DelegateCommand _clearBinding;

		private GamepadButtonDescription _remapedTo;

		private bool _isInheritedBinding;

		private bool _isDisableInheritVirtualMapFromMain;

		private ActivatorXBBindingDictionary _activatorXBBindingDictionary;

		private ActivatorXBBinding _currentActivatorXBBinding;

		public uint FeatureFlags;

		private int _mouseScrollDelta = 2;

		private static bool _copyToAnotherShiftsInProgress;

		private MaskItem _hostMaskItem;

		private DelegateCommand<ActivatorType?> _switchCurrentActivator;

		private DelegateCommand _revertRemapCommand;

		private DelegateCommand _Unmap;
	}
}
