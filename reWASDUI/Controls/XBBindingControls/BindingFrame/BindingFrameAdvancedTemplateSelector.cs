using System;
using System.Windows;
using System.Windows.Controls;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.KeyBindings.XB;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Controls.XBBindingControls.BindingFrame
{
	public class BindingFrameAdvancedTemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			XBBinding xbbinding = item as XBBinding;
			FrameworkElement frameworkElement = container as FrameworkElement;
			if (xbbinding == null || frameworkElement == null)
			{
				return null;
			}
			GamepadButton gamepadButton = xbbinding.GamepadButton;
			if (GamepadButtonExtensions.IsAnyStick(gamepadButton) || GamepadButtonExtensions.IsMouseDirection(gamepadButton))
			{
				return frameworkElement.FindResource("BFStickAdvancedTemplate") as DataTemplate;
			}
			if (GamepadButtonExtensions.IsAnyTrigger(gamepadButton))
			{
				return frameworkElement.FindResource("BFTriggerAdvancedTemplate") as DataTemplate;
			}
			if (GamepadButtonExtensions.IsAnyPhysicalTrackPadPressureZone(gamepadButton))
			{
				return frameworkElement.FindResource("BFTrackpadZonesTemplate") as DataTemplate;
			}
			if (GamepadButtonExtensions.IsLeftDS3AnalogZone(gamepadButton) || GamepadButtonExtensions.IsRightDS3AnalogZone(gamepadButton))
			{
				return frameworkElement.FindResource("BFBumperAdvancedTemplate") as DataTemplate;
			}
			return null;
		}
	}
}
