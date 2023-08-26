using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace reWASDUI.Utils.Converters
{
	public class EnumToBooleanToVisibilityConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			EnumToBooleanToVisibilityConverter enumToBooleanToVisibilityConverter;
			if ((enumToBooleanToVisibilityConverter = EnumToBooleanToVisibilityConverter._converter) == null)
			{
				enumToBooleanToVisibilityConverter = (EnumToBooleanToVisibilityConverter._converter = new EnumToBooleanToVisibilityConverter());
			}
			return enumToBooleanToVisibilityConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return false;
			}
			return value.Equals(parameter) ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return false;
			}
			if (!value.Equals(true))
			{
				return Binding.DoNothing;
			}
			return parameter;
		}

		private static EnumToBooleanToVisibilityConverter _converter;
	}
}
