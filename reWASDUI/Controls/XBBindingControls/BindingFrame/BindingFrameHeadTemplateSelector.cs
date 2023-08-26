using System;
using System.Windows;
using System.Windows.Controls;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.KeyBindings;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Controls.XBBindingControls.BindingFrame
{
	public class BindingFrameHeadTemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			AssociatedControllerButton associatedControllerButton = item as AssociatedControllerButton;
			FrameworkElement frameworkElement = container as FrameworkElement;
			if (associatedControllerButton == null || frameworkElement == null)
			{
				return frameworkElement.FindResource("BFEmptyTemplateHead") as DataTemplate;
			}
			if (associatedControllerButton.ControllerBindingFrameMode != null)
			{
				return frameworkElement.FindResource("BFEmptyTemplateHead") as DataTemplate;
			}
			GamepadButton gamepadButton = associatedControllerButton.GamepadButton;
			if (GamepadButtonExtensions.IsAdditionalStick(gamepadButton))
			{
				return frameworkElement.FindResource("BFAdditionalStickTemplateHead") as DataTemplate;
			}
			if (GamepadButtonExtensions.IsGyroTiltDirection(gamepadButton))
			{
				return frameworkElement.FindResource("BFGyroTiltDirectionTemplateHead") as DataTemplate;
			}
			if (GamepadButtonExtensions.IsAnyPhysicalTrackPadDigitalDirection(gamepadButton))
			{
				return frameworkElement.FindResource("BFTouchpadButtonTemplateHead") as DataTemplate;
			}
			return frameworkElement.FindResource("BFEmptyTemplateHead") as DataTemplate;
		}
	}
}
