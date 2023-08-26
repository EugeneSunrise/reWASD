using System;
using System.Windows;
using System.Windows.Controls;

namespace reWASDUI.Controls.XBBindingControls.BindingAnnotation
{
	public abstract class BaseGamepadBindingAnnotation : BaseXBBindingAnnotation
	{
		public Orientation Orientation
		{
			get
			{
				return (Orientation)base.GetValue(BaseGamepadBindingAnnotation.OrientationProperty);
			}
			set
			{
				base.SetValue(BaseGamepadBindingAnnotation.OrientationProperty, value);
			}
		}

		public bool TextIsHiddenIfNotMapped
		{
			get
			{
				return (bool)base.GetValue(BaseGamepadBindingAnnotation.TextIsHiddenIfNotMappedProperty);
			}
			set
			{
				base.SetValue(BaseGamepadBindingAnnotation.TextIsHiddenIfNotMappedProperty, value);
			}
		}

		public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(BaseGamepadBindingAnnotation), new PropertyMetadata(Orientation.Horizontal));

		public static readonly DependencyProperty TextIsHiddenIfNotMappedProperty = DependencyProperty.Register("TextIsHiddenIfNotMapped", typeof(bool), typeof(BaseGamepadBindingAnnotation), new PropertyMetadata(true));
	}
}
