using System;
using System.Windows;
using System.Windows.Data;
using DiscSoft.NET.Common.Utils.Converters;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.Services;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Controls.XBBindingControls
{
	public abstract class BaseXBBindingUserControl : BaseServicesResolvingControl
	{
		public XBBinding XBBinding
		{
			get
			{
				return (XBBinding)base.GetValue(BaseXBBindingUserControl.XBBindingProperty);
			}
			set
			{
				base.SetValue(BaseXBBindingUserControl.XBBindingProperty, value);
			}
		}

		private static void XBBindingChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			BaseXBBindingUserControl baseXBBindingUserControl = d as BaseXBBindingUserControl;
			if (baseXBBindingUserControl == null)
			{
				return;
			}
			baseXBBindingUserControl.OnXBBindingChanged(e);
		}

		public XBBinding CurrentXBBinding
		{
			get
			{
				return (XBBinding)base.GetValue(BaseXBBindingUserControl.CurrentXBBindingProperty);
			}
			set
			{
				base.SetValue(BaseXBBindingUserControl.CurrentXBBindingProperty, value);
			}
		}

		private static void CurrentXBBindingChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			BaseXBBindingUserControl baseXBBindingUserControl = d as BaseXBBindingUserControl;
			if (baseXBBindingUserControl == null)
			{
				return;
			}
			baseXBBindingUserControl.OnCurrentXBBindingChanged(e);
		}

		protected virtual void OnCurrentXBBindingChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		public AssociatedControllerButton CurrentControllerButton
		{
			get
			{
				return (AssociatedControllerButton)base.GetValue(BaseXBBindingUserControl.CurrentControllerButtonProperty);
			}
			set
			{
				base.SetValue(BaseXBBindingUserControl.CurrentControllerButtonProperty, value);
			}
		}

		private static void CurrentControllerButtonChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			BaseXBBindingUserControl baseXBBindingUserControl = d as BaseXBBindingUserControl;
			if (baseXBBindingUserControl == null)
			{
				return;
			}
			baseXBBindingUserControl.OnCurrentControllerButtonChanged(e);
		}

		protected virtual void OnCurrentControllerButtonChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static void IsShiftModeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			BaseXBBindingUserControl baseXBBindingUserControl = d as BaseXBBindingUserControl;
			if (baseXBBindingUserControl == null)
			{
				return;
			}
			baseXBBindingUserControl.OnIsShiftModeChanged(e);
		}

		protected virtual void OnIsShiftModeChanged(DependencyPropertyChangedEventArgs e)
		{
			this.ReEvaluateXBBinding();
		}

		public bool IsShiftMode
		{
			get
			{
				return (bool)base.GetValue(BaseXBBindingUserControl.IsShiftModeModeProperty);
			}
			set
			{
				base.SetValue(BaseXBBindingUserControl.IsShiftModeModeProperty, value);
			}
		}

		public bool IsHovered
		{
			get
			{
				return (bool)base.GetValue(BaseXBBindingUserControl.IsHoveredProperty);
			}
			set
			{
				base.SetValue(BaseXBBindingUserControl.IsHoveredProperty, value);
			}
		}

		public bool IsSameCurrentSubConfig()
		{
			if (this.XBBinding != null)
			{
				XBBinding xbbinding = this.XBBinding;
				ControllerFamily? controllerFamily = ((xbbinding != null) ? new ControllerFamily?(xbbinding.HostCollection.SubConfigData.ControllerFamily) : null);
				GameProfilesService gameProfilesService = base.GameProfilesService;
				ControllerFamily? controllerFamily2;
				if (gameProfilesService == null)
				{
					controllerFamily2 = null;
				}
				else
				{
					BaseXBBindingCollection realCurrentBeingMappedBindingCollection = gameProfilesService.RealCurrentBeingMappedBindingCollection;
					controllerFamily2 = ((realCurrentBeingMappedBindingCollection != null) ? new ControllerFamily?(realCurrentBeingMappedBindingCollection.SubConfigData.ControllerFamily) : null);
				}
				ControllerFamily? controllerFamily3 = controllerFamily2;
				if (!((controllerFamily.GetValueOrDefault() == controllerFamily3.GetValueOrDefault()) & (controllerFamily != null == (controllerFamily3 != null))))
				{
					XBBinding xbbinding2 = this.XBBinding;
					ConfigData configData = ((xbbinding2 != null) ? xbbinding2.HostCollection.SubConfigData.ConfigData : null);
					GameProfilesService gameProfilesService2 = base.GameProfilesService;
					object obj;
					if (gameProfilesService2 == null)
					{
						obj = null;
					}
					else
					{
						BaseXBBindingCollection realCurrentBeingMappedBindingCollection2 = gameProfilesService2.RealCurrentBeingMappedBindingCollection;
						if (realCurrentBeingMappedBindingCollection2 == null)
						{
							obj = null;
						}
						else
						{
							SubConfigData subConfigData = realCurrentBeingMappedBindingCollection2.SubConfigData;
							obj = ((subConfigData != null) ? subConfigData.ConfigData : null);
						}
					}
					if (configData == obj)
					{
						XBBinding xbbinding3 = this.XBBinding;
						bool flag;
						if (xbbinding3 == null)
						{
							flag = true;
						}
						else
						{
							AssociatedControllerButton controllerButton = xbbinding3.ControllerButton;
							bool? flag2 = ((controllerButton != null) ? new bool?(GamepadButtonExtensions.IsMouseDirection(controllerButton.GamepadButton)) : null);
							bool flag3 = true;
							flag = !((flag2.GetValueOrDefault() == flag3) & (flag2 != null));
						}
						if (flag)
						{
							XBBinding xbbinding4 = this.XBBinding;
							bool flag4;
							if (xbbinding4 == null)
							{
								flag4 = true;
							}
							else
							{
								AssociatedControllerButton controllerButton2 = xbbinding4.ControllerButton;
								bool? flag2 = ((controllerButton2 != null) ? new bool?(GamepadButtonExtensions.IsMouseScroll(controllerButton2.GamepadButton)) : null);
								bool flag3 = true;
								flag4 = !((flag2.GetValueOrDefault() == flag3) & (flag2 != null));
							}
							if (flag4)
							{
								return false;
							}
						}
					}
				}
			}
			return true;
		}

		public BaseXBBindingUserControl()
		{
			Binding binding = new Binding("KeyBindingService.GameProfilesService.CurrentGame.CurrentConfig.CurrentBindingCollection.CurrentShiftXBBindingCollection");
			binding.Converter = new ValueIsNotNull();
			binding.RelativeSource = RelativeSource.Self;
			BindingOperations.SetBinding(this, BaseXBBindingUserControl.IsShiftModeModeProperty, binding);
			binding = new Binding("GameProfilesService.RealCurrentBeingMappedBindingCollection.CurrentXBBinding");
			binding.RelativeSource = RelativeSource.Self;
			BindingOperations.SetBinding(this, BaseXBBindingUserControl.CurrentXBBindingProperty, binding);
			binding = new Binding("GameProfilesService.RealCurrentBeingMappedBindingCollection.CurrentControllerButton");
			binding.RelativeSource = RelativeSource.Self;
			BindingOperations.SetBinding(this, BaseXBBindingUserControl.CurrentControllerButtonProperty, binding);
		}

		protected virtual void OnXBBindingChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		protected override void OnServicesReady()
		{
			this.ReEvaluateXBBinding();
		}

		protected abstract void ReEvaluateXBBinding();

		public static readonly DependencyProperty XBBindingProperty = DependencyProperty.Register("XBBinding", typeof(XBBinding), typeof(BaseXBBindingUserControl), new PropertyMetadata(null, new PropertyChangedCallback(BaseXBBindingUserControl.XBBindingChangedCallback)));

		public static readonly DependencyProperty CurrentXBBindingProperty = DependencyProperty.Register("CurrentXBBinding", typeof(XBBinding), typeof(BaseXBBindingUserControl), new PropertyMetadata(null, new PropertyChangedCallback(BaseXBBindingUserControl.CurrentXBBindingChangedCallback)));

		public static readonly DependencyProperty CurrentControllerButtonProperty = DependencyProperty.Register("CurrentControllerButton", typeof(AssociatedControllerButton), typeof(BaseXBBindingUserControl), new PropertyMetadata(null, new PropertyChangedCallback(BaseXBBindingUserControl.CurrentControllerButtonChangedCallback)));

		public static readonly DependencyProperty IsShiftModeModeProperty = DependencyProperty.Register("IsShiftMode", typeof(bool), typeof(BaseXBBindingUserControl), new PropertyMetadata(false, new PropertyChangedCallback(BaseXBBindingUserControl.IsShiftModeChangedCallback)));

		public static readonly DependencyProperty IsHoveredProperty = DependencyProperty.Register("IsHovered", typeof(bool), typeof(BaseXBBindingUserControl), new PropertyMetadata(false));
	}
}
