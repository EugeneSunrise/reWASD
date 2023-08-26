using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using reWASDCommon.Infrastructure.Enums;

namespace reWASDUI.Utils.Converters
{
	[ValueConversion(typeof(ActivatorType), typeof(string))]
	public class DoubleSumConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			DoubleSumConverter doubleSumConverter;
			if ((doubleSumConverter = DoubleSumConverter._converter) == null)
			{
				doubleSumConverter = (DoubleSumConverter._converter = new DoubleSumConverter());
			}
			return doubleSumConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || parameter == null)
			{
				return null;
			}
			double value2 = (value as double?).Value;
			double num = 0.0;
			if (!double.TryParse(parameter as string, out num))
			{
				return null;
			}
			return value2 + num;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static DoubleSumConverter _converter;
	}
}
