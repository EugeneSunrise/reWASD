using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Utils.XBUtilModel;

namespace XBEliteWPF.Utils.Converters
{
	[ValueConversion(typeof(ActivatorType), typeof(Drawing))]
	public class ActivatorTypeToDrawingConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			ActivatorTypeToDrawingConverter activatorTypeToDrawingConverter;
			if ((activatorTypeToDrawingConverter = ActivatorTypeToDrawingConverter._converter) == null)
			{
				activatorTypeToDrawingConverter = (ActivatorTypeToDrawingConverter._converter = new ActivatorTypeToDrawingConverter());
			}
			return activatorTypeToDrawingConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return null;
			}
			return XBUtils.GetDrawingForActivatorType((ActivatorType)value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static ActivatorTypeToDrawingConverter _converter;
	}
}
