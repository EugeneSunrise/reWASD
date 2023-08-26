using System;
using System.Windows;
using System.Windows.Controls;
using reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups;

namespace reWASDUI.Views.ContentZoneGamepad.AdvancedStick
{
	public class BaseDirectionalAnalogGroupUserControl : UserControl
	{
		public BaseDirectionalAnalogGroup DirectionalGroup
		{
			get
			{
				return (BaseDirectionalAnalogGroup)base.GetValue(BaseDirectionalAnalogGroupUserControl.DirectionalGroupProperty);
			}
			set
			{
				base.SetValue(BaseDirectionalAnalogGroupUserControl.DirectionalGroupProperty, value);
			}
		}

		private static void DirectionalGroupChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			BaseDirectionalAnalogGroupUserControl baseDirectionalAnalogGroupUserControl = d as BaseDirectionalAnalogGroupUserControl;
			if (baseDirectionalAnalogGroupUserControl == null)
			{
				return;
			}
			baseDirectionalAnalogGroupUserControl.OnDirectionalGroupChanged(e);
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

		public static readonly DependencyProperty DirectionalGroupProperty = DependencyProperty.Register("DirectionalGroup", typeof(BaseDirectionalAnalogGroup), typeof(BaseDirectionalAnalogGroupUserControl), new PropertyMetadata(null, new PropertyChangedCallback(BaseDirectionalAnalogGroupUserControl.DirectionalGroupChangedCallback)));
	}
}
