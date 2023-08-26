using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using reWASDCommon.Infrastructure.Enums;

namespace reWASDUI.Utils.Converters
{
	public class IsHighZoneConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			IsHighZoneConverter isHighZoneConverter;
			if ((isHighZoneConverter = IsHighZoneConverter._converter) == null)
			{
				isHighZoneConverter = (IsHighZoneConverter._converter = new IsHighZoneConverter());
			}
			return isHighZoneConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is GamepadButton)
			{
				GamepadButton gamepadButton = (GamepadButton)value;
				return gamepadButton == 39 || gamepadButton == 46 || gamepadButton == 54 || gamepadButton == 58 || gamepadButton == 63 || gamepadButton == 185 || gamepadButton == 188 || gamepadButton == 191 || gamepadButton == 197 || gamepadButton == 200 || gamepadButton == 194 || gamepadButton == 203 || gamepadButton == 206 || gamepadButton == 209 || gamepadButton == 212;
			}
			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static IsHighZoneConverter _converter;
	}
}
