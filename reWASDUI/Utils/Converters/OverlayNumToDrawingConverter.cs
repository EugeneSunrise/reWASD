using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace reWASDUI.Utils.Converters
{
	public class OverlayNumToDrawingConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			OverlayNumToDrawingConverter overlayNumToDrawingConverter;
			if ((overlayNumToDrawingConverter = OverlayNumToDrawingConverter._converter) == null)
			{
				overlayNumToDrawingConverter = (OverlayNumToDrawingConverter._converter = new OverlayNumToDrawingConverter());
			}
			return overlayNumToDrawingConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool flag = parameter as string == "notnull";
			if (!(value is int))
			{
				return null;
			}
			int num = (int)value;
			if (num != -1)
			{
				return Application.Current.TryFindResource("OverlayMenuMode") as Drawing;
			}
			if (!flag)
			{
				return null;
			}
			return Application.Current.TryFindResource("UriKeyScanCodeEmpty") as Drawing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static OverlayNumToDrawingConverter _converter;
	}
}
