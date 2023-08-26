using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using DiscSoft.NET.Common.Localization;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure;

namespace reWASDUI.Utils.Converters
{
	[ValueConversion(typeof(ActivatorType), typeof(string))]
	public class ApplyToSlotConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			ApplyToSlotConverter applyToSlotConverter;
			if ((applyToSlotConverter = ApplyToSlotConverter._converter) == null)
			{
				applyToSlotConverter = (ApplyToSlotConverter._converter = new ApplyToSlotConverter());
			}
			return applyToSlotConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return null;
			}
			SlotInfo slotInfo = (SlotInfo)value;
			return string.Format(DTLocalization.GetString(12538), slotInfo);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static ApplyToSlotConverter _converter;
	}
}
