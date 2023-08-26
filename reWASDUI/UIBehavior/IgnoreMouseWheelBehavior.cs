using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace reWASDUI.UIBehavior
{
	public sealed class IgnoreMouseWheelBehavior : Behavior<UIElement>
	{
		protected override void OnAttached()
		{
			base.OnAttached();
			base.AssociatedObject.PreviewMouseWheel += this.AssociatedObject_PreviewMouseWheel;
		}

		protected override void OnDetaching()
		{
			base.AssociatedObject.PreviewMouseWheel -= this.AssociatedObject_PreviewMouseWheel;
			base.OnDetaching();
		}

		private void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			e.Handled = true;
			MouseWheelEventArgs mouseWheelEventArgs = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
			mouseWheelEventArgs.RoutedEvent = UIElement.MouseWheelEvent;
			base.AssociatedObject.RaiseEvent(mouseWheelEventArgs);
		}
	}
}
