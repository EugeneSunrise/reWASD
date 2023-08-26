using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Utils.Converters
{
	[ValueConversion(typeof(LEDSupportedDevice), typeof(bool))]
	public class LEDSupportedDeviceIsMonochromeConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			LEDSupportedDeviceIsMonochromeConverter ledsupportedDeviceIsMonochromeConverter;
			if ((ledsupportedDeviceIsMonochromeConverter = LEDSupportedDeviceIsMonochromeConverter._converter) == null)
			{
				ledsupportedDeviceIsMonochromeConverter = (LEDSupportedDeviceIsMonochromeConverter._converter = new LEDSupportedDeviceIsMonochromeConverter());
			}
			return ledsupportedDeviceIsMonochromeConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is LEDSupportedDevice)
			{
				LEDSupportedDevice ledsupportedDevice = (LEDSupportedDevice)value;
				return LEDSupportedDeviceExtensions.IsMonochromeColor(ledsupportedDevice);
			}
			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static LEDSupportedDeviceIsMonochromeConverter _converter;
	}
}
