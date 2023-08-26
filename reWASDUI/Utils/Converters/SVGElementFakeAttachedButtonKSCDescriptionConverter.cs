using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using reWASDCommon.Infrastructure.Enums;

namespace reWASDUI.Utils.Converters
{
	[ValueConversion(typeof(Direction), typeof(Drawing))]
	public class SVGElementFakeAttachedButtonKSCDescriptionConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			SVGElementFakeAttachedButtonKSCDescriptionConverter svgelementFakeAttachedButtonKSCDescriptionConverter;
			if ((svgelementFakeAttachedButtonKSCDescriptionConverter = SVGElementFakeAttachedButtonKSCDescriptionConverter._converter) == null)
			{
				svgelementFakeAttachedButtonKSCDescriptionConverter = (SVGElementFakeAttachedButtonKSCDescriptionConverter._converter = new SVGElementFakeAttachedButtonKSCDescriptionConverter());
			}
			return svgelementFakeAttachedButtonKSCDescriptionConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return null;
			}
			string text = value as string;
			if (text == "AltFakeButton")
			{
				return "Alt";
			}
			if (text == "CtrlFakeButton")
			{
				return "Ctrl";
			}
			if (!(text == "ShiftFakeButton"))
			{
				return null;
			}
			return "Shift";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static SVGElementFakeAttachedButtonKSCDescriptionConverter _converter;
	}
}
