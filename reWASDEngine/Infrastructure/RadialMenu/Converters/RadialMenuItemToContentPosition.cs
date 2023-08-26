using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using RadialMenu.Controls;

namespace reWASDEngine.Infrastructure.RadialMenu.Converters
{
	internal class RadialMenuItemToContentPosition : MarkupExtension, IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (parameter == null)
			{
				throw new ArgumentNullException("parameter", "RadialMenuItemToContentPosition converter needs the parameter (string axis) !");
			}
			string text = (string)parameter;
			if (text != "X" && text != "Y")
			{
				throw new ArgumentException("RadialMenuItemToContentPosition parameter needs to be 'X' or 'Y' !", "parameter");
			}
			double num = (double)values[0];
			double num2 = (double)values[1];
			double num3 = (double)values[2];
			double num4 = (double)values[4];
			RadialMenuItem radialMenuItem = (RadialMenuItem)values[5];
			if (radialMenuItem.RadialMenu == null || radialMenuItem.RadialMenu.Items == null)
			{
				return null;
			}
			double innerRadius = radialMenuItem.InnerRadius;
			double outerRadius = radialMenuItem.OuterRadius;
			double num5 = innerRadius + (outerRadius - innerRadius) / 2.0;
			Point point = RadialMenuItemToContentPosition.ComputeCartesianCoordinate(new Point(num2, num3), num, num5);
			if (text == "X")
			{
				return point.X - num4 / 2.0;
			}
			return point.Y - num4 / 2.0;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new InvalidOperationException("RadialMenuItemToContentPosition is a One-Way converter only !");
		}

		private static Point ComputeCartesianCoordinate(Point center, double angle, double radius)
		{
			double num = 0.017453292519943295 * (angle - 90.0);
			double num2 = radius * Math.Cos(num);
			double num3 = radius * Math.Sin(num);
			return new Point(num2 + center.X, num3 + center.Y);
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			RadialMenuItemToContentPosition radialMenuItemToContentPosition;
			if ((radialMenuItemToContentPosition = RadialMenuItemToContentPosition._converter) == null)
			{
				radialMenuItemToContentPosition = (RadialMenuItemToContentPosition._converter = new RadialMenuItemToContentPosition());
			}
			return radialMenuItemToContentPosition;
		}

		private static RadialMenuItemToContentPosition _converter;
	}
}
