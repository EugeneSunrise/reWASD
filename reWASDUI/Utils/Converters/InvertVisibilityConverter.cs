using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace reWASDUI.Utils.Converters
{
	public class InvertVisibilityConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			InvertVisibilityConverter invertVisibilityConverter;
			if ((invertVisibilityConverter = InvertVisibilityConverter._converter) == null)
			{
				invertVisibilityConverter = (InvertVisibilityConverter._converter = new InvertVisibilityConverter());
			}
			return invertVisibilityConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType == typeof(Visibility))
			{
				return ((Visibility)value == Visibility.Collapsed) ? Visibility.Visible : Visibility.Collapsed;
			}
			throw new InvalidOperationException("Converter can only convert to value of type Visibility.");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new Exception("Invalid call - one way only");
		}

		private static InvertVisibilityConverter _converter;
	}
}
