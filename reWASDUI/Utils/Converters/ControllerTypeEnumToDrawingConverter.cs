using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.DataModels.GamepadSlotHotkeyCollection;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtilModel;

namespace reWASDUI.Utils.Converters
{
	[ValueConversion(typeof(ControllerTypeEnum[]), typeof(IEnumerable<Drawing>))]
	public class ControllerTypeEnumToDrawingConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			ControllerTypeEnumToDrawingConverter controllerTypeEnumToDrawingConverter;
			if ((controllerTypeEnumToDrawingConverter = ControllerTypeEnumToDrawingConverter._converter) == null)
			{
				controllerTypeEnumToDrawingConverter = (ControllerTypeEnumToDrawingConverter._converter = new ControllerTypeEnumToDrawingConverter());
			}
			return controllerTypeEnumToDrawingConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			List<ControllerTypeEnum> list = null;
			if (value is KeyValuePair<string, HotkeyCollection>)
			{
				KeyValuePair<string, HotkeyCollection> keyValuePair = (KeyValuePair<string, HotkeyCollection>)value;
				list = keyValuePair.Value.ControllerTypes;
				if (keyValuePair.Value.IsNintendoSwitchJoyConComposite)
				{
					return Application.Current.TryFindResource("MiniGamepadNJCon") as Drawing;
				}
				if (keyValuePair.Value.ControllerFamily == 3)
				{
					if (list.Count == 2)
					{
						if (list.Any((ControllerTypeEnum t) => t == 503))
						{
							if (list.Any((ControllerTypeEnum x) => x == 504))
							{
								return Application.Current.TryFindResource("MiniEngineControllerMobileMouse+Keyboard") as Drawing;
							}
						}
					}
					return XBUtils.GetDrawingForControllerFamily(3);
				}
				if (keyValuePair.Value.ControllerFamily == 2 && !ControllerTypeExtensions.IsAnyEngineMouse(keyValuePair.Value.ControllerTypes[0]))
				{
					return XBUtils.GetDrawingForControllerFamily(2);
				}
				if (keyValuePair.Value.ControllerFamily == 1 && !ControllerTypeExtensions.IsEngineKeyboard(keyValuePair.Value.ControllerTypes[0]))
				{
					return XBUtils.GetDrawingForControllerFamily(1);
				}
			}
			else
			{
				if (value is ControllerFamily)
				{
					ControllerFamily controllerFamily = (ControllerFamily)value;
					return XBUtils.GetDrawingForControllerFamily(controllerFamily);
				}
				if (value is ControllerTypeEnum)
				{
					ControllerTypeEnum controllerTypeEnum = (ControllerTypeEnum)value;
					list = new List<ControllerTypeEnum> { controllerTypeEnum };
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
			}
			return XBUtils.GetDrawingsForControllerTypeEnums((list != null) ? list.ToList<ControllerTypeEnum>() : null).FirstOrDefault<Drawing>();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static ControllerTypeEnumToDrawingConverter _converter;
	}
}
