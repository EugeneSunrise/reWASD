using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using DiscSoft.NET.Common.Localization;

namespace reWASDUI.Utils.Converters
{
	public class ShiftNumToTextConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			ShiftNumToTextConverter shiftNumToTextConverter;
			if ((shiftNumToTextConverter = ShiftNumToTextConverter._converter) == null)
			{
				shiftNumToTextConverter = (ShiftNumToTextConverter._converter = new ShiftNumToTextConverter());
			}
			return shiftNumToTextConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is int))
			{
				return "";
			}
			int num = (int)value;
			if ((string)parameter == "btn")
			{
				return "btnToggleShift" + num.ToString();
			}
			if (num == -1)
			{
				return DTLocalization.GetString(4403);
			}
			return App.GameProfilesService.CurrentGame.CurrentConfig.ConfigData[0].MainXBBindingCollection.GetCollectionByLayer(num).Description;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static ShiftNumToTextConverter _converter;
	}
}
