using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Infrastructure.SupportedControllers;
using reWASDCommon.Infrastructure.SupportedControllers.Base;

namespace reWASDUI.Utils.Converters
{
	public class SupportedGamepadToVisibilityGamepadBackConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			SupportedGamepadToVisibilityGamepadBackConverter supportedGamepadToVisibilityGamepadBackConverter;
			if ((supportedGamepadToVisibilityGamepadBackConverter = SupportedGamepadToVisibilityGamepadBackConverter._converter) == null)
			{
				supportedGamepadToVisibilityGamepadBackConverter = (SupportedGamepadToVisibilityGamepadBackConverter._converter = new SupportedGamepadToVisibilityGamepadBackConverter());
			}
			return supportedGamepadToVisibilityGamepadBackConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is ControllerTypeEnum))
			{
				return Visibility.Visible;
			}
			ControllerTypeEnum controllerType = (ControllerTypeEnum)value;
			SupportedGamepad supportedGamepad = ControllersHelper.SupportedControllers.Find((SupportedControllerInfo x) => x.ControllerType == controllerType) as SupportedGamepad;
			if (supportedGamepad == null)
			{
				return Visibility.Visible;
			}
			return (supportedGamepad != null && supportedGamepad.IsBackVisible) ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static SupportedGamepadToVisibilityGamepadBackConverter _converter;
	}
}
