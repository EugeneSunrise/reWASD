using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Infrastructure.LED;
using reWASDUI.DataModels.GamepadSlotHotkeyCollection;
using reWASDUI.Infrastructure.Controller;
using XBEliteWPF.Utils.XBUtilModel;

namespace reWASDUI.Utils.Converters
{
	[ValueConversion(typeof(string), typeof(string))]
	public class ControllerIDToFriendlyNameConverter : MarkupExtension, IValueConverter, IMultiValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			ControllerIDToFriendlyNameConverter controllerIDToFriendlyNameConverter;
			if ((controllerIDToFriendlyNameConverter = ControllerIDToFriendlyNameConverter._converter) == null)
			{
				controllerIDToFriendlyNameConverter = (ControllerIDToFriendlyNameConverter._converter = new ControllerIDToFriendlyNameConverter());
			}
			return controllerIDToFriendlyNameConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is KeyValuePair<string, HotkeyCollection>)
			{
				return ((KeyValuePair<string, HotkeyCollection>)value).Value.DisplayName;
			}
			if (value is KeyValuePair<string, PlayerLedSettings>)
			{
				return ((KeyValuePair<string, PlayerLedSettings>)value).Value.DisplayName;
			}
			string text = value as string;
			if (text != null)
			{
				return XBUtils.GetControllerFriendlyNameSavedOrBasedOnControllerTypes(text, null);
			}
			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			string text = string.Empty;
			string ID = string.Empty;
			ControllerTypeEnum[] array = null;
			string text2 = values[0] as string;
			if (text2 != null)
			{
				ID = text2;
			}
			object obj = values[0];
			if (obj is ControllerTypeEnum)
			{
				ControllerTypeEnum controllerTypeEnum = (ControllerTypeEnum)obj;
				array = new ControllerTypeEnum[] { controllerTypeEnum };
			}
			ControllerTypeEnum[] array2 = values[0] as ControllerTypeEnum[];
			if (array2 != null)
			{
				array = array2;
			}
			string text3 = values[1] as string;
			if (text3 != null)
			{
				ID = text3;
			}
			obj = values[1];
			if (obj is ControllerTypeEnum)
			{
				ControllerTypeEnum controllerTypeEnum2 = (ControllerTypeEnum)obj;
				array = new ControllerTypeEnum[] { controllerTypeEnum2 };
			}
			ControllerTypeEnum[] array3 = values[1] as ControllerTypeEnum[];
			if (array3 != null)
			{
				array = array3;
			}
			BaseControllerVM baseControllerVM = App.GamepadService.AllPhysicalControllers.FirstOrDefault((BaseControllerVM g) => g.ID == ID);
			if (baseControllerVM == null)
			{
				text = XBUtils.GetControllerFriendlyNameSavedOrBasedOnControllerTypes(ID, (array != null) ? array.ToList<ControllerTypeEnum>() : null);
			}
			else
			{
				text = baseControllerVM.ControllerDisplayName;
			}
			return text;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static ControllerIDToFriendlyNameConverter _converter;
	}
}
