using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace reWASDUI.Utils.Converters
{
	public class ImageSourceToDrawingImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value as DrawingImage;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
