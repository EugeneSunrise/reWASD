using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using reWASDUI.Utils.XBUtil;

namespace reWASDUI.Utils.Converters
{
	public class ShiftNumToDrawingConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			ShiftNumToDrawingConverter shiftNumToDrawingConverter;
			if ((shiftNumToDrawingConverter = ShiftNumToDrawingConverter._converter) == null)
			{
				shiftNumToDrawingConverter = (ShiftNumToDrawingConverter._converter = new ShiftNumToDrawingConverter());
			}
			return shiftNumToDrawingConverter;
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
				return XBUtils.GetDrawingShiftNum(num);
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

		private static ShiftNumToDrawingConverter _converter;
	}
}
