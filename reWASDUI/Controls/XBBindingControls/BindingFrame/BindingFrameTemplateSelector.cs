using System;
using System.Windows;
using System.Windows.Controls;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.KeyBindings;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Controls.XBBindingControls.BindingFrame
{
	public class BindingFrameTemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			AssociatedControllerButton associatedControllerButton = item as AssociatedControllerButton;
			FrameworkElement frameworkElement = container as FrameworkElement;
			if (associatedControllerButton == null || frameworkElement == null)
			{
				return null;
			}
			if (associatedControllerButton.ControllerBindingFrameMode != null)
			{
				ControllerBindingFrameAdditionalModes? controllerBindingFrameAdditionalModes = associatedControllerButton.ControllerBindingFrameMode;
				ControllerBindingFrameAdditionalModes controllerBindingFrameAdditionalModes2 = 0;
				if ((controllerBindingFrameAdditionalModes.GetValueOrDefault() == controllerBindingFrameAdditionalModes2) & (controllerBindingFrameAdditionalModes != null))
				{
					return frameworkElement.FindResource("BFControllerWizardTemplate") as DataTemplate;
				}
				controllerBindingFrameAdditionalModes = associatedControllerButton.ControllerBindingFrameMode;
				controllerBindingFrameAdditionalModes2 = 1;
				if ((controllerBindingFrameAdditionalModes.GetValueOrDefault() == controllerBindingFrameAdditionalModes2) & (controllerBindingFrameAdditionalModes != null))
				{
					return frameworkElement.FindResource("BFMouseDirectionTemplate") as DataTemplate;
				}
				return null;
			}
			else
			{
				GamepadButton gamepadButton = associatedControllerButton.GamepadButton;
				if (GamepadButtonExtensions.IsDPAD(gamepadButton))
				{
					return frameworkElement.FindResource("BFDPADTemplate") as DataTemplate;
				}
				if (GamepadButtonExtensions.IsAnyStick(gamepadButton))
				{
					return frameworkElement.FindResource("BFStickTemplate") as DataTemplate;
				}
				if (GamepadButtonExtensions.IsMouseDirection(gamepadButton))
				{
					return frameworkElement.FindResource("BFMouseDirectionTemplate") as DataTemplate;
				}
				if (GamepadButtonExtensions.IsGyroTiltDirection(gamepadButton))
				{
					return frameworkElement.FindResource("BFGyroTiltDirectionTemplate") as DataTemplate;
				}
				if (GamepadButtonExtensions.IsAnyPhysicalTrackPadDigitalDirection(gamepadButton))
				{
					return frameworkElement.FindResource("BFTouchpadButtonTemplate") as DataTemplate;
				}
				return frameworkElement.FindResource("BFSingleButtonTemplate") as DataTemplate;
			}
		}
	}
}
