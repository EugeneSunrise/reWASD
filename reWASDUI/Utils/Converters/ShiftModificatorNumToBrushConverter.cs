using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using reWASDUI.Utils.XBUtil;

namespace reWASDUI.Utils.Converters
{
	public class ShiftModificatorNumToBrushConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			ShiftModificatorNumToBrushConverter shiftModificatorNumToBrushConverter;
			if ((shiftModificatorNumToBrushConverter = ShiftModificatorNumToBrushConverter._converter) == null)
			{
				shiftModificatorNumToBrushConverter = (ShiftModificatorNumToBrushConverter._converter = new ShiftModificatorNumToBrushConverter());
			}
			return shiftModificatorNumToBrushConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int num = 0;
			double num2 = 1.0;
			int num3 = -1;
			if (value is int)
			{
				int num4 = (int)value;
				if (num4 == -1)
				{
					num = 0;
				}
				if (num4 > 0)
				{
					num = num4;
				}
				num3 = num4;
			}
			if (parameter is double)
			{
				double num5 = (double)parameter;
				num2 = num5;
			}
			SolidColorBrush solidColorBrush;
			if (num == 0)
			{
				if (num3 == 0 || App.GameProfilesService.RealCurrentBeingMappedBindingCollection == null)
				{
					solidColorBrush = Application.Current.TryFindResource("CreamBrush") as SolidColorBrush;
				}
				else
				{
					solidColorBrush = App.GameProfilesService.RealCurrentBeingMappedBindingCollection.CollectionBrush;
				}
			}
			else
			{
				solidColorBrush = XBUtils.GetBrushForShiftModificatorNum(num);
			}
			if (solidColorBrush != null)
			{
				SolidColorBrush solidColorBrush2 = solidColorBrush.Clone();
				solidColorBrush2.Opacity = num2;
				solidColorBrush = solidColorBrush2;
			}
			return solidColorBrush;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static ShiftModificatorNumToBrushConverter _converter;
	}
}
