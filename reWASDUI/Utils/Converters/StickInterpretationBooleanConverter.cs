using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace reWASDUI.Utils.Converters
{
	public class StickInterpretationBooleanConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			StickInterpretationBooleanConverter stickInterpretationBooleanConverter;
			if ((stickInterpretationBooleanConverter = StickInterpretationBooleanConverter._converter) == null)
			{
				stickInterpretationBooleanConverter = (StickInterpretationBooleanConverter._converter = new StickInterpretationBooleanConverter());
			}
			return stickInterpretationBooleanConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (value != null && (bool)value) ? 1 : 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value != null && value.Equals(1);
		}

		private static StickInterpretationBooleanConverter _converter;
	}
}
