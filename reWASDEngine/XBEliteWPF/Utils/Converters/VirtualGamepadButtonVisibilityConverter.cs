using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using XBEliteWPF.Utils.XBUtil;

namespace XBEliteWPF.Utils.Converters
{
	public class VirtualGamepadButtonVisibilityConverter : MarkupExtension, IMultiValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			VirtualGamepadButtonVisibilityConverter virtualGamepadButtonVisibilityConverter;
			if ((virtualGamepadButtonVisibilityConverter = VirtualGamepadButtonVisibilityConverter._converter) == null)
			{
				virtualGamepadButtonVisibilityConverter = (VirtualGamepadButtonVisibilityConverter._converter = new VirtualGamepadButtonVisibilityConverter());
			}
			return virtualGamepadButtonVisibilityConverter;
		}

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			return XBUtils.ShouldVirtualGamepadButtonBeVisible(values) ? Visibility.Visible : Visibility.Collapsed;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static VirtualGamepadButtonVisibilityConverter _converter;
	}
}
