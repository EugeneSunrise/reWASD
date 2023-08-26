using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.DataModels.GamepadSlotHotkeyCollection;
using XBEliteWPF.Utils.XBUtilModel;

namespace reWASDUI.Utils.Converters
{
	[ValueConversion(typeof(ControllerTypeEnum[]), typeof(IEnumerable<Drawing>))]
	public class ControllerTypeEnumToDrawingsConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			ControllerTypeEnumToDrawingsConverter controllerTypeEnumToDrawingsConverter;
			if ((controllerTypeEnumToDrawingsConverter = ControllerTypeEnumToDrawingsConverter._converter) == null)
			{
				controllerTypeEnumToDrawingsConverter = (ControllerTypeEnumToDrawingsConverter._converter = new ControllerTypeEnumToDrawingsConverter());
			}
			return controllerTypeEnumToDrawingsConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			List<ControllerTypeEnum> list = null;
			if (value is KeyValuePair<string, HotkeyCollection>)
			{
				list = ((KeyValuePair<string, HotkeyCollection>)value).Value.ControllerTypes;
			}
			else
			{
				List<ControllerTypeEnum> list2 = value as List<ControllerTypeEnum>;
				if (list2 != null)
				{
					list = list2;
				}
				else
				{
					ControllerTypeEnum[] array = value as ControllerTypeEnum[];
					if (array != null)
					{
						list = array.ToList<ControllerTypeEnum>();
					}
				}
			}
			return XBUtils.GetDrawingsForControllerTypeEnums(list);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static ControllerTypeEnumToDrawingsConverter _converter;
	}
}
