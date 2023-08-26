using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace reWASDEngine.Infrastructure.RadialMenu.Converters
{
	internal class DoubleSumConverter : MarkupExtension, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			object obj;
			try
			{
				double num = System.Convert.ToDouble(value);
				double num2 = System.Convert.ToDouble(parameter);
				obj = num + num2;
			}
			catch (Exception)
			{
				obj = value;
			}
			return obj;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new InvalidOperationException();
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			DoubleSumConverter doubleSumConverter;
			if ((doubleSumConverter = DoubleSumConverter._converter) == null)
			{
				doubleSumConverter = (DoubleSumConverter._converter = new DoubleSumConverter());
			}
			return doubleSumConverter;
		}

		private static DoubleSumConverter _converter;
	}
}
