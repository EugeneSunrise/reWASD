using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Utils.XBUtilModel;

namespace XBEliteWPF.Utils.Converters
{
	[ValueConversion(typeof(ControllerFamily), typeof(Drawing))]
	public class ControllerFamilyOrMaskIconConverter : MarkupExtension, IValueConverter, IMultiValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			ControllerFamilyOrMaskIconConverter controllerFamilyOrMaskIconConverter;
			if ((controllerFamilyOrMaskIconConverter = ControllerFamilyOrMaskIconConverter._converter) == null)
			{
				controllerFamilyOrMaskIconConverter = (ControllerFamilyOrMaskIconConverter._converter = new ControllerFamilyOrMaskIconConverter());
			}
			return controllerFamilyOrMaskIconConverter;
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
			if (values[1] != null && (bool)values[1])
			{
				return Application.Current.TryFindResource("Mask");
			}
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

		private static ControllerFamilyOrMaskIconConverter _converter;
	}
}
