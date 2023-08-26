using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.KeyBindings;

namespace reWASDUI.Controls
{
	public class UserControlWithDropDown : UserControl
	{
		public bool IsDropDownOpen
		{
			get
			{
				return (bool)base.GetValue(UserControlWithDropDown.IsDropDownOpenProperty);
			}
			set
			{
				base.SetValue(UserControlWithDropDown.IsDropDownOpenProperty, value);
			}
		}

		public UserControlWithDropDown()
		{
			App.EventAggregator.GetEvent<CurrentShiftBindingCollectionChanged>().Subscribe(delegate(ShiftXBBindingCollection shift)
			{
				this.IsDropDownOpen = false;
			});
			base.MouseDown += this.OnMouseDown;
		}

		private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			UserControlWithDropDown userControlWithDropDown = (UserControlWithDropDown)d;
			if ((bool)e.NewValue)
			{
				Mouse.Capture(userControlWithDropDown, CaptureMode.SubTree);
				return;
			}
			Mouse.Capture(null);
		}

		private void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (this.IsDropDownOpen && e.ChangedButton == MouseButton.Left)
			{
				base.SetCurrentValue(UserControlWithDropDown.IsDropDownOpenProperty, false);
			}
		}

		public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(UserControlWithDropDown), new PropertyMetadata(false, new PropertyChangedCallback(UserControlWithDropDown.OnIsDropDownOpenChanged)));
	}
}
