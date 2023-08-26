using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace reWASDUI.Utils.Converters
{
	[ValueConversion(typeof(object), typeof(bool))]
	public class IsShowActivatorContentConverter : MarkupExtension, IMultiValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			IsShowActivatorContentConverter isShowActivatorContentConverter;
			if ((isShowActivatorContentConverter = IsShowActivatorContentConverter._converter) == null)
			{
				isShowActivatorContentConverter = (IsShowActivatorContentConverter._converter = new IsShowActivatorContentConverter());
			}
			return isShowActivatorContentConverter;
		}

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Any((object v) => v == DependencyProperty.UnsetValue))
			{
				return false;
			}
			object obj = values[0];
			object obj2 = values[1];
			bool flag = (bool)values[2];
			bool flag2 = object.Equals(obj, obj2) || flag;
			return (parameter is string && ((string)parameter).ToLower() == "invert") ? (!flag2) : flag2;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static IsShowActivatorContentConverter _converter;
	}
}
