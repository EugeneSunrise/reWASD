using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using reWASDCommon.Infrastructure.Enums;

namespace reWASDUI.Utils.Converters
{
	[ValueConversion(typeof(object), typeof(string))]
	public class OverlayMenuCategoryConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			OverlayMenuCategoryConverter overlayMenuCategoryConverter;
			if ((overlayMenuCategoryConverter = OverlayMenuCategoryConverter._converter) == null)
			{
				overlayMenuCategoryConverter = (OverlayMenuCategoryConverter._converter = new OverlayMenuCategoryConverter());
			}
			return overlayMenuCategoryConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is RadialMenuIconCategory)
			{
				RadialMenuIconCategory radialMenuIconCategory = (RadialMenuIconCategory)value;
				return radialMenuIconCategory.TryGetLocalizedDescription();
			}
			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static OverlayMenuCategoryConverter _converter;
	}
}
