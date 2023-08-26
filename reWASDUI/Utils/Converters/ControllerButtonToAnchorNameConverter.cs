using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.Utils.XBUtil;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;

namespace reWASDUI.Utils.Converters
{
	public class ControllerButtonToAnchorNameConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			ControllerButtonToAnchorNameConverter controllerButtonToAnchorNameConverter;
			if ((controllerButtonToAnchorNameConverter = ControllerButtonToAnchorNameConverter._converter) == null)
			{
				controllerButtonToAnchorNameConverter = (ControllerButtonToAnchorNameConverter._converter = new ControllerButtonToAnchorNameConverter());
			}
			return controllerButtonToAnchorNameConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			AssociatedControllerButton associatedControllerButton = new AssociatedControllerButton();
			XBBinding xbbinding = value as XBBinding;
			if (xbbinding != null)
			{
				associatedControllerButton = xbbinding.ControllerButton;
			}
			else
			{
				AssociatedControllerButton associatedControllerButton2 = value as AssociatedControllerButton;
				if (associatedControllerButton2 != null)
				{
					associatedControllerButton = associatedControllerButton2;
				}
				else if (value is GamepadButton)
				{
					GamepadButton gamepadButton = (GamepadButton)value;
					associatedControllerButton.GamepadButton = gamepadButton;
				}
				else
				{
					KeyScanCodeV2 keyScanCodeV = value as KeyScanCodeV2;
					if (keyScanCodeV != null)
					{
						associatedControllerButton.KeyScanCode = keyScanCodeV;
					}
					else if (value is ControllerBindingFrameAdditionalModes)
					{
						ControllerBindingFrameAdditionalModes controllerBindingFrameAdditionalModes = (ControllerBindingFrameAdditionalModes)value;
						associatedControllerButton.ControllerBindingFrameMode = new ControllerBindingFrameAdditionalModes?(controllerBindingFrameAdditionalModes);
					}
					else
					{
						XBBinding xbbinding2 = parameter as XBBinding;
						if (xbbinding2 != null)
						{
							associatedControllerButton = xbbinding2.ControllerButton;
						}
						else
						{
							AssociatedControllerButton associatedControllerButton3 = parameter as AssociatedControllerButton;
							if (associatedControllerButton3 != null)
							{
								associatedControllerButton = associatedControllerButton3;
							}
							else if (parameter is GamepadButton)
							{
								GamepadButton gamepadButton2 = (GamepadButton)parameter;
								associatedControllerButton.GamepadButton = gamepadButton2;
							}
							else
							{
								KeyScanCodeV2 keyScanCodeV2 = parameter as KeyScanCodeV2;
								if (keyScanCodeV2 != null)
								{
									associatedControllerButton.KeyScanCode = keyScanCodeV2;
								}
								else
								{
									if (!(parameter is ControllerBindingFrameAdditionalModes))
									{
										return null;
									}
									ControllerBindingFrameAdditionalModes controllerBindingFrameAdditionalModes2 = (ControllerBindingFrameAdditionalModes)parameter;
									associatedControllerButton.ControllerBindingFrameMode = new ControllerBindingFrameAdditionalModes?(controllerBindingFrameAdditionalModes2);
								}
							}
						}
					}
				}
			}
			return XBUtils.ConvertControllerButtonToAnchorString(associatedControllerButton);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string)
			{
				return XBUtils.ConvertAnchorStringToGamepadButton((string)value);
			}
			return null;
		}

		private static ControllerButtonToAnchorNameConverter _converter;
	}
}
