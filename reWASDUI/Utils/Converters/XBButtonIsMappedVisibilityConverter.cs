using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using reWASDUI.Infrastructure.KeyBindings.XB;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;

namespace reWASDUI.Utils.Converters
{
	public class XBButtonIsMappedVisibilityConverter : MarkupExtension, IValueConverter, IMultiValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			XBButtonIsMappedVisibilityConverter xbbuttonIsMappedVisibilityConverter;
			if ((xbbuttonIsMappedVisibilityConverter = XBButtonIsMappedVisibilityConverter._converter) == null)
			{
				xbbuttonIsMappedVisibilityConverter = (XBButtonIsMappedVisibilityConverter._converter = new XBButtonIsMappedVisibilityConverter());
			}
			return xbbuttonIsMappedVisibilityConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool flag = ((XBBinding)value).CurrentActivatorXBBinding.MappedKey != KeyScanCodeV2.NoMap;
			return ((parameter == null) ? flag : (!flag)) ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			List<bool> list = new List<bool>();
			foreach (XBBinding xbbinding in values)
			{
				list.Add(xbbinding.CurrentActivatorXBBinding.MappedKey != KeyScanCodeV2.NoMap);
			}
			string text = parameter as string;
			bool flag;
			if (((text != null) ? text.ToLower() : null) == "or")
			{
				flag = list.Any((bool value) => value);
			}
			else
			{
				flag = list.All((bool value) => value);
			}
			return flag ? Visibility.Visible : Visibility.Collapsed;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static XBButtonIsMappedVisibilityConverter _converter;
	}
}
