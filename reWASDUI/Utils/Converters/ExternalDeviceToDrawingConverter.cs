using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using reWASDUI.Utils.XBUtil;
using XBEliteWPF.Infrastructure.ExternalDevices;

namespace reWASDUI.Utils.Converters
{
	[ValueConversion(typeof(ExternalDevice[]), typeof(IEnumerable<Drawing>))]
	public class ExternalDeviceToDrawingConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			ExternalDeviceToDrawingConverter externalDeviceToDrawingConverter;
			if ((externalDeviceToDrawingConverter = ExternalDeviceToDrawingConverter._converter) == null)
			{
				externalDeviceToDrawingConverter = (ExternalDeviceToDrawingConverter._converter = new ExternalDeviceToDrawingConverter());
			}
			return externalDeviceToDrawingConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			ExternalDevice externalDevice = value as ExternalDevice;
			if (externalDevice != null)
			{
				return XBUtils.GetAdaptersImage(externalDevice.DeviceType, externalDevice.IsOnlineAndCorrect, externalDevice.IsBluetoothSecureSimpleParingIsNotPresent);
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static ExternalDeviceToDrawingConverter _converter;
	}
}
