using System;
using System.Windows;
using DiscSoft.NET.Common.View.Controls;
using reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups;

namespace reWASDUI.Views.ContentZoneGamepad.AdvancedStick
{
	public class BaseDirectionalAnalogGroupSquarePositioningControl : SquarePositioningControl
	{
		public BaseDirectionalAnalogGroup DirectionalGroup
		{
			get
			{
				return (BaseDirectionalAnalogGroup)base.GetValue(BaseDirectionalAnalogGroupSquarePositioningControl.DirectionalGroupProperty);
			}
			set
			{
				base.SetValue(BaseDirectionalAnalogGroupSquarePositioningControl.DirectionalGroupProperty, value);
			}
		}

		private static void DirectionalGroupChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			BaseDirectionalAnalogGroupSquarePositioningControl baseDirectionalAnalogGroupSquarePositioningControl = d as BaseDirectionalAnalogGroupSquarePositioningControl;
			if (baseDirectionalAnalogGroupSquarePositioningControl == null)
			{
				return;
			}
			baseDirectionalAnalogGroupSquarePositioningControl.OnDirectionalGroupChanged(e);
		}

		protected virtual void OnDirectionalGroupChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		public bool IsLeftStick
		{
			get
			{
				return this.DirectionalGroup is LeftStickDirectionalGroup;
			}
		}

		public bool IsRightStick
		{
			get
			{
				return this.DirectionalGroup is RightStickDirectionalGroup;
			}
		}

		public static readonly DependencyProperty DirectionalGroupProperty = DependencyProperty.Register("DirectionalGroup", typeof(BaseDirectionalAnalogGroup), typeof(BaseDirectionalAnalogGroupSquarePositioningControl), new PropertyMetadata(null, new PropertyChangedCallback(BaseDirectionalAnalogGroupSquarePositioningControl.DirectionalGroupChangedCallback)));
	}
}
