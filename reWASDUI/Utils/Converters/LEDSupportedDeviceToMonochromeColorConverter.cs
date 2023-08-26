using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Utils.Converters
{
	[ValueConversion(typeof(LEDSupportedDevice), typeof(Color))]
	public class LEDSupportedDeviceToMonochromeColorConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			LEDSupportedDeviceToMonochromeColorConverter ledsupportedDeviceToMonochromeColorConverter;
			if ((ledsupportedDeviceToMonochromeColorConverter = LEDSupportedDeviceToMonochromeColorConverter._converter) == null)
			{
				ledsupportedDeviceToMonochromeColorConverter = (LEDSupportedDeviceToMonochromeColorConverter._converter = new LEDSupportedDeviceToMonochromeColorConverter());
			}
			return ledsupportedDeviceToMonochromeColorConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is LEDSupportedDevice)
			{
				LEDSupportedDevice ledsupportedDevice = (LEDSupportedDevice)value;
				return LEDSupportedDeviceExtensions.GetMonoChromeBaseColor(ledsupportedDevice);
			}
			return Colors.White;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static LEDSupportedDeviceToMonochromeColorConverter _converter;
	}
}
