using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Utils.XBUtilModel;

namespace reWASDUI.Utils.Converters
{
	[ValueConversion(typeof(ControllerFamily), typeof(Drawing))]
	public class ControllerFamilyIconConverter : MarkupExtension, IValueConverter, IMultiValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			ControllerFamilyIconConverter controllerFamilyIconConverter;
			if ((controllerFamilyIconConverter = ControllerFamilyIconConverter._converter) == null)
			{
				controllerFamilyIconConverter = (ControllerFamilyIconConverter._converter = new ControllerFamilyIconConverter());
			}
			return controllerFamilyIconConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is ControllerFamily)
			{
				ControllerFamily controllerFamily = (ControllerFamily)value;
				return XBUtils.GetControllerFamilyImage(controllerFamily);
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			object obj = values[0];
			if (obj is ControllerFamily)
			{
				ControllerFamily controllerFamily = (ControllerFamily)obj;
				return XBUtils.GetControllerFamilyImage(controllerFamily);
			}
			return null;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static ControllerFamilyIconConverter _converter;
	}
}
