using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.Utils.XBUtil;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;

namespace reWASDUI.Utils.Converters
{
	public class AnchorNameToControllerButtonConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			AnchorNameToControllerButtonConverter anchorNameToControllerButtonConverter;
			if ((anchorNameToControllerButtonConverter = AnchorNameToControllerButtonConverter._converter) == null)
			{
				anchorNameToControllerButtonConverter = (AnchorNameToControllerButtonConverter._converter = new AnchorNameToControllerButtonConverter());
			}
			return anchorNameToControllerButtonConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string)
			{
				return XBUtils.ConvertAnchorStringToGamepadButton((string)value);
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			XBBinding xbbinding = value as XBBinding;
			if (xbbinding != null)
			{
				if (xbbinding.ControllerButton.IsGamepad)
				{
					return XBUtils.ConvertGamepadButtonToAnchorString(xbbinding.ControllerButton.GamepadButton);
				}
				if (xbbinding.ControllerButton.IsKeyScanCode && xbbinding.ControllerButton.KeyScanCode.IsMouse)
				{
					return XBUtils.ConvertMouseButtonToAnchorString(KeyScanCodeV2.FindMouseButtonByKeyScanCode(xbbinding.ControllerButton.KeyScanCode));
				}
			}
			else
			{
				if (value is GamepadButton)
				{
					GamepadButton gamepadButton = (GamepadButton)value;
					return XBUtils.ConvertGamepadButtonToAnchorString(gamepadButton);
				}
				if (value is MouseButton)
				{
					MouseButton mouseButton = (MouseButton)value;
					return XBUtils.ConvertMouseButtonToAnchorString(mouseButton);
				}
			}
			return null;
		}

		private static AnchorNameToControllerButtonConverter _converter;
	}
}
