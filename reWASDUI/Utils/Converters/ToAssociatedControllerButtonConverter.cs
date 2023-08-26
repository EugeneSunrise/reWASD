using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.KeyBindings;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;

namespace reWASDUI.Utils.Converters
{
	public class ToAssociatedControllerButtonConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			ToAssociatedControllerButtonConverter toAssociatedControllerButtonConverter;
			if ((toAssociatedControllerButtonConverter = ToAssociatedControllerButtonConverter._converter) == null)
			{
				toAssociatedControllerButtonConverter = (ToAssociatedControllerButtonConverter._converter = new ToAssociatedControllerButtonConverter());
			}
			return toAssociatedControllerButtonConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is GamepadButton)
			{
				GamepadButton gamepadButton = (GamepadButton)value;
				return new AssociatedControllerButton(gamepadButton);
			}
			GamepadButtonDescription gamepadButtonDescription = value as GamepadButtonDescription;
			if (gamepadButtonDescription != null)
			{
				return new AssociatedControllerButton(gamepadButtonDescription.Button);
			}
			KeyScanCodeV2 keyScanCodeV = value as KeyScanCodeV2;
			if (keyScanCodeV != null)
			{
				return new AssociatedControllerButton(keyScanCodeV);
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static ToAssociatedControllerButtonConverter _converter;
	}
}
