using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Utils.XBUtil;

namespace reWASDUI.Utils.Converters
{
	[ValueConversion(typeof(ExternalDeviceType[]), typeof(IEnumerable<Drawing>))]
	public class ExternalDeviceTypeToDrawingConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			ExternalDeviceTypeToDrawingConverter externalDeviceTypeToDrawingConverter;
			if ((externalDeviceTypeToDrawingConverter = ExternalDeviceTypeToDrawingConverter._converter) == null)
			{
				externalDeviceTypeToDrawingConverter = (ExternalDeviceTypeToDrawingConverter._converter = new ExternalDeviceTypeToDrawingConverter());
			}
			return externalDeviceTypeToDrawingConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is ExternalDeviceType)
			{
				ExternalDeviceType externalDeviceType = (ExternalDeviceType)value;
				return XBUtils.GetAdaptersImage(externalDeviceType, true, false);
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static ExternalDeviceTypeToDrawingConverter _converter;
	}
}
