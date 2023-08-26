using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using DiscSoft.NET.Common.Utils;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.DataModels;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups;
using reWASDUI.Services;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Controls.XBBindingControls.BindingAnnotation
{
	public abstract class BaseXBBindingAnnotation : BaseXBBindingUserControl
	{
		public bool IsHiddenIfNotMapped
		{
			get
			{
				return (bool)base.GetValue(BaseXBBindingAnnotation.IsHiddenIfNotMappedProperty);
			}
			set
			{
				base.SetValue(BaseXBBindingAnnotation.IsHiddenIfNotMappedProperty, value);
			}
		}

		private static void IsHiddenIfNotMappedOnChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			BaseXBBindingAnnotation baseXBBindingAnnotation = dependencyObject as BaseXBBindingAnnotation;
			if (baseXBBindingAnnotation == null)
			{
				return;
			}
			baseXBBindingAnnotation.ReEvaluateVisibility();
		}

		public bool IsCurrentBinding
		{
			get
			{
				return (bool)base.GetValue(BaseXBBindingAnnotation.IsCurrentBindingProperty);
			}
			set
			{
				base.SetValue(BaseXBBindingAnnotation.IsCurrentBindingProperty, value);
			}
		}

		private static void IsLabelModeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			BaseXBBindingAnnotation baseXBBindingAnnotation = d as BaseXBBindingAnnotation;
			if (baseXBBindingAnnotation == null)
			{
				return;
			}
			baseXBBindingAnnotation.OnIsLabelModeChanged(e);
		}

		protected virtual void OnIsLabelModeChanged(DependencyPropertyChangedEventArgs e)
		{
			this.ReEvaluateVisibility();
		}

		public bool IsLabelMode
		{
			get
			{
				return (bool)base.GetValue(BaseXBBindingAnnotation.IsLabelModeProperty);
			}
			set
			{
				base.SetValue(BaseXBBindingAnnotation.IsLabelModeProperty, value);
			}
		}

		private static void IsShowMappingsViewChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			BaseXBBindingAnnotation baseXBBindingAnnotation = d as BaseXBBindingAnnotation;
			if (baseXBBindingAnnotation == null)
			{
				return;
			}
			baseXBBindingAnnotation.OnIsShowMappingsViewChanged(e);
		}

		protected virtual void OnIsShowMappingsViewChanged(DependencyPropertyChangedEventArgs e)
		{
			this.ReEvaluateVisibility();
		}

		public bool IsShowMappingsView
		{
			get
			{
				return (bool)base.GetValue(BaseXBBindingAnnotation.IsShowMappingsViewProperty);
			}
			set
			{
				base.SetValue(BaseXBBindingAnnotation.IsShowMappingsViewProperty, value);
			}
		}

		public Brush AnnotationRecolorBrush
		{
			get
			{
				return (Brush)base.GetValue(BaseXBBindingAnnotation.AnnotationRecolorBrushProperty);
			}
			set
			{
				base.SetValue(BaseXBBindingAnnotation.AnnotationRecolorBrushProperty, value);
			}
		}

		private static void AnnotationRecolorBrushChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			BaseXBBindingAnnotation baseXBBindingAnnotation = d as BaseXBBindingAnnotation;
			if (baseXBBindingAnnotation == null)
			{
				return;
			}
			baseXBBindingAnnotation.OnAnnotationRecolorBrushChanged();
		}

		protected virtual void OnAnnotationRecolorBrushChanged()
		{
		}

		protected BaseXBBindingCollection CurrentAnnotatedXBBindingCollection
		{
			get
			{
				if (base.GameProfilesService == null)
				{
					return null;
				}
				return base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
			}
		}

		protected MainXBBindingCollection CurrentMainXBBindingCollection
		{
			get
			{
				GameProfilesService gameProfilesService = base.GameProfilesService;
				if (((gameProfilesService != null) ? gameProfilesService.RealCurrentBeingMappedBindingCollection : null) == null)
				{
					return null;
				}
				return base.GameProfilesService.CurrentGame.CurrentConfig.CurrentBindingCollection;
			}
		}

		protected BaseXBBindingAnnotation()
		{
			base.Loaded += this.OnLoaded;
			base.DataContextChanged += delegate(object sender, DependencyPropertyChangedEventArgs args)
			{
				this.RefreshVisibilityBindingAndIsCurrentBinding();
			};
		}

		protected virtual void ReEvaluateVisibilityForGroup(BaseDirectionalGroup directionalGroup)
		{
			if (base.XBBinding == null || this.CurrentAnnotatedXBBindingCollection == null)
			{
				return;
			}
			if ((GamepadButtonExtensions.IsLeftStick(base.XBBinding.GamepadButton) && directionalGroup is LeftStickDirectionalGroup) || (GamepadButtonExtensions.IsAdditionalStick(base.XBBinding.GamepadButton) && directionalGroup is AdditionalStickDirectionalGroup) || (GamepadButtonExtensions.IsRightStick(base.XBBinding.GamepadButton) && directionalGroup is AdditionalStickDirectionalGroup) || (GamepadButtonExtensions.IsRightStick(base.XBBinding.GamepadButton) && directionalGroup is RightStickDirectionalGroup) || (GamepadButtonExtensions.IsDPAD(base.XBBinding.GamepadButton) && directionalGroup is DPADDirectionalGroup) || (GamepadButtonExtensions.IsGyroTiltDirection(base.XBBinding.GamepadButton) && directionalGroup is GyroTiltDirectionalGroup) || (GamepadButtonExtensions.IsPhysicalTrackPad1(base.XBBinding.GamepadButton) && directionalGroup is Touchpad1DirectionalGroup) || (GamepadButtonExtensions.IsPhysicalTrackPad2(base.XBBinding.GamepadButton) && directionalGroup is Touchpad2DirectionalGroup) || (GamepadButtonExtensions.IsMouseDirection(base.XBBinding.GamepadButton) && directionalGroup is MouseDirectionalGroup))
			{
				this.RefreshVisibilityBindingAndIsCurrentBinding();
			}
		}

		protected static void XBButtonToBindChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			BaseGamepadBindingAnnotation baseGamepadBindingAnnotation = d as BaseGamepadBindingAnnotation;
			if (baseGamepadBindingAnnotation == null)
			{
				return;
			}
			baseGamepadBindingAnnotation.ReEvaluateXBBinding();
		}

		protected override void OnXBBindingChanged(DependencyPropertyChangedEventArgs e)
		{
			XBBinding xbbinding = e.OldValue as XBBinding;
			if (xbbinding != null)
			{
				xbbinding.PropertyChangedExtended -= new PropertyChangedExtendedEventHandler(this.OnXBBindingValueChanged);
			}
			XBBinding xbbinding2 = e.NewValue as XBBinding;
			if (xbbinding2 != null)
			{
				xbbinding2.PropertyChangedExtended -= new PropertyChangedExtendedEventHandler(this.OnXBBindingValueChanged);
				xbbinding2.PropertyChangedExtended += new PropertyChangedExtendedEventHandler(this.OnXBBindingValueChanged);
			}
			base.OnXBBindingChanged(e);
		}

		protected void OnXBBindingValueChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			if (propertyChangedEventArgs.PropertyName == "ActivatorXBBindingDictionary" || propertyChangedEventArgs.PropertyName == "IsDisableInheritVirtualMapFromMain" || propertyChangedEventArgs.PropertyName == "RemapedTo" || propertyChangedEventArgs.PropertyName == "MappedKey" || propertyChangedEventArgs.PropertyName == "JumpToShift" || propertyChangedEventArgs.PropertyName == "IsAdaptiveTriggers" || propertyChangedEventArgs.PropertyName == "IsAdaptiveTriggersInherited" || propertyChangedEventArgs.PropertyName == "IsRumble" || propertyChangedEventArgs.PropertyName == "IsRumbleLeftMotor" || propertyChangedEventArgs.PropertyName == "IsRumbleRightMotor" || propertyChangedEventArgs.PropertyName == "IsRumbleLeftTrigger" || propertyChangedEventArgs.PropertyName == "IsRumbleRightTrigger" || propertyChangedEventArgs.PropertyName == "Description" || propertyChangedEventArgs.PropertyName == "MacroSequence")
			{
				this.RefreshVisibilityBindingAndIsCurrentBinding();
			}
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			if (this.isLoaded)
			{
				return;
			}
			this.isLoaded = true;
			if (base.EventAggregator != null)
			{
				base.EventAggregator.GetEvent<CurrentGamepadChanged>().Subscribe(delegate(BaseControllerVM o)
				{
					XBBinding xbbinding = base.XBBinding;
					if (xbbinding != null)
					{
						xbbinding.RefreshRemappedTo();
					}
					this.RefreshVisibilityBindingAndIsCurrentBinding();
				});
				base.EventAggregator.GetEvent<CurrentGamepadSlotChanged>().Subscribe(delegate(Slot o)
				{
					this.RefreshVisibilityBindingAndIsCurrentBinding();
				});
				base.EventAggregator.GetEvent<MaskViewChanged>().Subscribe(delegate(object o)
				{
					this.RefreshVisibilityBindingAndIsCurrentBinding();
				});
				base.EventAggregator.GetEvent<CurrentButtonMappingChanged>().Subscribe(new Action<object>(this.OnCurrentButtonmappingChanged));
				base.EventAggregator.GetEvent<CurrentShiftBindingCollectionChanged>().Subscribe(new Action<ShiftXBBindingCollection>(this.OnCurrentShiftBindingCollectionChanged));
				base.EventAggregator.GetEvent<ShiftBindingCollectionRemoved>().Subscribe(new Action<object>(this.OnShiftBindingCollectionRemoved));
				base.EventAggregator.GetEvent<VirtualControllerTypeChanged>().Subscribe(delegate(ConfigData o)
				{
					this.ReEvaluateXBBinding();
				});
				base.EventAggregator.GetEvent<RequestRefreshAnnotationVisibilityForGroup>().Subscribe(delegate(BaseDirectionalGroup dg)
				{
					this.ReEvaluateVisibilityForGroup(dg);
				});
			}
			if (base.GamepadService != null)
			{
				base.GamepadService.IsRemapChanged += delegate([Nullable(2)] object o, EventArgs args)
				{
					this.RefreshVisibilityBindingAndIsCurrentBinding();
				};
			}
			if (base.GameProfilesService != null)
			{
				base.GameProfilesService.CurrentGameChanged += delegate(object o, PropertyChangedExtendedEventArgs<GameVM> args)
				{
					this.RefreshVisibilityBindingAndIsCurrentBinding();
				};
				base.GameProfilesService.CurrentGameProfileChanged += delegate(object o, PropertyChangedExtendedEventArgs<ConfigVM> args)
				{
					this.RefreshVisibilityBindingAndIsCurrentBinding();
				};
			}
		}

		private void OnCurrentButtonmappingChanged(object o)
		{
			this.ReEvaluateIsCurrentBinding();
		}

		private void OnCurrentShiftBindingCollectionChanged(object o)
		{
			if (base.XBBinding != null)
			{
				XBBinding xbbinding = base.XBBinding;
				ControllerFamily? controllerFamily = ((xbbinding != null) ? new ControllerFamily?(xbbinding.HostCollection.SubConfigData.ControllerFamily) : null);
				BaseXBBindingCollection realCurrentBeingMappedBindingCollection = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
				ControllerFamily? controllerFamily2 = ((realCurrentBeingMappedBindingCollection != null) ? new ControllerFamily?(realCurrentBeingMappedBindingCollection.SubConfigData.ControllerFamily) : null);
				if (!((controllerFamily.GetValueOrDefault() == controllerFamily2.GetValueOrDefault()) & (controllerFamily != null == (controllerFamily2 != null))))
				{
					XBBinding xbbinding2 = base.XBBinding;
					bool flag;
					if (xbbinding2 == null)
					{
						flag = false;
					}
					else
					{
						AssociatedControllerButton controllerButton = xbbinding2.ControllerButton;
						bool? flag2 = ((controllerButton != null) ? new bool?(GamepadButtonExtensions.IsMouseDirection(controllerButton.GamepadButton)) : null);
						bool flag3 = true;
						flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
					}
					if (!flag)
					{
						XBBinding xbbinding3 = base.XBBinding;
						bool flag4;
						if (xbbinding3 == null)
						{
							flag4 = false;
						}
						else
						{
							AssociatedControllerButton controllerButton2 = xbbinding3.ControllerButton;
							bool? flag2 = ((controllerButton2 != null) ? new bool?(GamepadButtonExtensions.IsMouseScroll(controllerButton2.GamepadButton)) : null);
							bool flag3 = true;
							flag4 = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
						}
						if (!flag4)
						{
							return;
						}
					}
				}
			}
			this.RefreshVisibilityBindingAndIsCurrentBinding();
		}

		private void OnShiftBindingCollectionRemoved(object o)
		{
			this.RefreshVisibilityBindingAndIsCurrentBinding();
		}

		protected abstract void ReEvaluateVisibility();

		protected virtual void RefreshVisibilityBindingAndIsCurrentBinding()
		{
			this.ReEvaluateXBBinding();
			this.ReEvaluateVisibility();
			this.ReEvaluateIsCurrentBinding();
		}

		protected override void OnServicesReady()
		{
			this.RefreshVisibilityBindingAndIsCurrentBinding();
		}

		protected override void OnCurrentXBBindingChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnCurrentXBBindingChanged(e);
			this.ReEvaluateIsCurrentBinding();
		}

		protected abstract void ReEvaluateIsCurrentBinding();

		public static readonly DependencyProperty IsHiddenIfNotMappedProperty = DependencyProperty.Register("IsHiddenIfNotMapped", typeof(bool), typeof(BaseXBBindingAnnotation), new PropertyMetadata(true, new PropertyChangedCallback(BaseXBBindingAnnotation.IsHiddenIfNotMappedOnChanged)));

		public static readonly DependencyProperty IsCurrentBindingProperty = DependencyProperty.Register("IsCurrentBinding", typeof(bool), typeof(BaseXBBindingAnnotation), new PropertyMetadata(false));

		public static readonly DependencyProperty IsLabelModeProperty = DependencyProperty.Register("IsLabelMode", typeof(bool), typeof(BaseXBBindingAnnotation), new PropertyMetadata(false, new PropertyChangedCallback(BaseXBBindingAnnotation.IsLabelModeChangedCallback)));

		public static readonly DependencyProperty IsShowMappingsViewProperty = DependencyProperty.Register("IsShowMappingsView", typeof(bool), typeof(BaseXBBindingAnnotation), new PropertyMetadata(false, new PropertyChangedCallback(BaseXBBindingAnnotation.IsShowMappingsViewChangedCallback)));

		public static readonly DependencyProperty AnnotationRecolorBrushProperty = DependencyProperty.Register("AnnotationRecolorBrush", typeof(Brush), typeof(BaseXBBindingAnnotation), new PropertyMetadata(null, new PropertyChangedCallback(BaseXBBindingAnnotation.AnnotationRecolorBrushChangedCallback)));

		public static readonly DependencyProperty ShiftXBBindingCollectionProperty = DependencyProperty.Register("ShiftXBBindingCollection", typeof(ShiftXBBindingCollection), typeof(BaseXBBindingAnnotation), new PropertyMetadata(null));

		private bool isLoaded;
	}
}
