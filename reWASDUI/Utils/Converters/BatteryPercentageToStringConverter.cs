using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using DiscSoft.NET.Common.Localization;

namespace reWASDUI.Utils.Converters
{
	[ValueConversion(typeof(object), typeof(string))]
	public class BatteryPercentageToStringConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			BatteryPercentageToStringConverter batteryPercentageToStringConverter;
			if ((batteryPercentageToStringConverter = BatteryPercentageToStringConverter._converter) == null)
			{
				batteryPercentageToStringConverter = (BatteryPercentageToStringConverter._converter = new BatteryPercentageToStringConverter());
			}
			return batteryPercentageToStringConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return DTLocalization.GetString(12352).Replace("{0}", ((byte)value).ToString());
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static BatteryPercentageToStringConverter _converter;
	}
}
