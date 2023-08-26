using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using DiscSoft.NET.Common.Localization;

namespace reWASDUI.Utils.Converters
{
	[ValueConversion(typeof(object), typeof(string))]
	public class StringDeadzoneDisableHintConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			StringDeadzoneDisableHintConverter stringDeadzoneDisableHintConverter;
			if ((stringDeadzoneDisableHintConverter = StringDeadzoneDisableHintConverter._converter) == null)
			{
				stringDeadzoneDisableHintConverter = (StringDeadzoneDisableHintConverter._converter = new StringDeadzoneDisableHintConverter());
			}
			return stringDeadzoneDisableHintConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return string.Format(DTLocalization.GetString(12479), string.IsNullOrEmpty((string)value) ? DTLocalization.GetString(11195) : value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static StringDeadzoneDisableHintConverter _converter;
	}
}
