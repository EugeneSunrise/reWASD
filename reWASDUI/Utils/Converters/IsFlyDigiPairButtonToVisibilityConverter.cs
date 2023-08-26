using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Services.Interfaces;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Utils.Converters
{
	public class IsFlyDigiPairButtonToVisibilityConverter : MarkupExtension, IMultiValueConverter, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			IsFlyDigiPairButtonToVisibilityConverter isFlyDigiPairButtonToVisibilityConverter;
			if ((isFlyDigiPairButtonToVisibilityConverter = IsFlyDigiPairButtonToVisibilityConverter._converter) == null)
			{
				isFlyDigiPairButtonToVisibilityConverter = (IsFlyDigiPairButtonToVisibilityConverter._converter = new IsFlyDigiPairButtonToVisibilityConverter());
			}
			return isFlyDigiPairButtonToVisibilityConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is GamepadButton)
			{
				GamepadButton gamepadButton = (GamepadButton)value;
				if (gamepadButton == 12)
				{
					IGamepadService gamepadService = App.GamepadService;
					bool flag;
					if (gamepadService == null)
					{
						flag = false;
					}
					else
					{
						BaseControllerVM currentGamepad = gamepadService.CurrentGamepad;
						bool? flag2;
						if (currentGamepad == null)
						{
							flag2 = null;
						}
						else
						{
							ControllerVM currentController = currentGamepad.CurrentController;
							flag2 = ((currentController != null) ? new bool?(ControllerTypeExtensions.IsFlydigi(currentController.ControllerType)) : null);
						}
						bool? flag3 = flag2;
						bool flag4 = true;
						flag = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
					}
					if (flag)
					{
						return Visibility.Visible;
					}
				}
			}
			return Visibility.Collapsed;
		}

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Length == 2)
			{
				object obj = values[0];
				if (obj is GamepadButton)
				{
					GamepadButton gamepadButton = (GamepadButton)obj;
					obj = values[1];
					if (obj is ControllerTypeEnum)
					{
						ControllerTypeEnum controllerTypeEnum = (ControllerTypeEnum)obj;
						if (gamepadButton == 12 && ControllerTypeExtensions.IsFlydigi(controllerTypeEnum))
						{
							return Visibility.Visible;
						}
					}
				}
			}
			return Visibility.Collapsed;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static IsFlyDigiPairButtonToVisibilityConverter _converter;
	}
}
