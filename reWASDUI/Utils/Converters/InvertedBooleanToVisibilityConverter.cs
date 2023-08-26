using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace reWASDUI.Utils.Converters
{
	public class InvertedBooleanToVisibilityConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			InvertedBooleanToVisibilityConverter invertedBooleanToVisibilityConverter;
			if ((invertedBooleanToVisibilityConverter = InvertedBooleanToVisibilityConverter._converter) == null)
			{
				invertedBooleanToVisibilityConverter = (InvertedBooleanToVisibilityConverter._converter = new InvertedBooleanToVisibilityConverter());
			}
			return invertedBooleanToVisibilityConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((parameter == null) ? ((bool)value) : (!(bool)value)) ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static InvertedBooleanToVisibilityConverter _converter;
	}
}
