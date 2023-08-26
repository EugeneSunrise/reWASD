using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace reWASDEngine.Infrastructure.RadialMenu.Converters
{
	internal class ParameterToStringConverter : MarkupExtension, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (parameter != null)
			{
				string text = parameter as string;
				if (text != null)
				{
					return text;
				}
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new InvalidOperationException("RadialMenuItemToArrowPosition is a One-Way converter only !");
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			ParameterToStringConverter parameterToStringConverter;
			if ((parameterToStringConverter = ParameterToStringConverter._converter) == null)
			{
				parameterToStringConverter = (ParameterToStringConverter._converter = new ParameterToStringConverter());
			}
			return parameterToStringConverter;
		}

		private static ParameterToStringConverter _converter;
	}
}
