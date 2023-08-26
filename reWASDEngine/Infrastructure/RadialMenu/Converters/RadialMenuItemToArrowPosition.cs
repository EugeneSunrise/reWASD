using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace reWASDEngine.Infrastructure.RadialMenu.Converters
{
	internal class RadialMenuItemToArrowPosition : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			string text = (string)parameter;
			if (text != "X" && text != "Y")
			{
				throw new ArgumentException("RadialMenuItemToArrowPosition parameter needs to be 'X' or 'Y' !", "parameter");
			}
			double num = (double)values[0];
			double num2 = (double)values[1];
			double num3 = (double)values[2];
			double num4 = (double)values[3];
			double num5 = double.Parse(values[4].ToString());
			if (text == "X")
			{
				return num - num3 / 2.0;
			}
			return num2 - num5 - num4 / 2.0;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new InvalidOperationException("RadialMenuItemToArrowPosition is a One-Way converter only !");
		}

		private static Point ComputeCartesianCoordinate(Point center, double angle, double radius)
		{
			double num = 0.017453292519943295 * (angle - 90.0);
			double num2 = radius * Math.Cos(num);
			double num3 = radius * Math.Sin(num);
			return new Point(num2 + center.X, num3 + center.Y);
		}
	}
}
