using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace reWASDUI.Utils.Converters
{
	[ValueConversion(typeof(object), typeof(string))]
	public class StringsConcatConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			StringsConcatConverter stringsConcatConverter;
			if ((stringsConcatConverter = StringsConcatConverter._converter) == null)
			{
				stringsConcatConverter = (StringsConcatConverter._converter = new StringsConcatConverter());
			}
			return stringsConcatConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string text = value as string;
			if (text != null)
			{
				string text2 = parameter as string;
				if (text2 != null)
				{
					return text + text2;
				}
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static StringsConcatConverter _converter;
	}
}
