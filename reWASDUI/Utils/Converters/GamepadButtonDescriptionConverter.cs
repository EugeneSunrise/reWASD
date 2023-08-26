using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.Services.Interfaces;
using reWASDUI.Utils.XBUtil;
using reWASDUI.ViewModels;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtilModel;

namespace reWASDUI.Utils.Converters
{
	public class GamepadButtonDescriptionConverter : MarkupExtension, IValueConverter, IMultiValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			GamepadButtonDescriptionConverter gamepadButtonDescriptionConverter;
			if ((gamepadButtonDescriptionConverter = GamepadButtonDescriptionConverter._converter) == null)
			{
				gamepadButtonDescriptionConverter = (GamepadButtonDescriptionConverter._converter = new GamepadButtonDescriptionConverter());
			}
			return gamepadButtonDescriptionConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			GamepadButton gamepadButton = (GamepadButton)value;
			BaseControllerVM currentGamepad = ((MainWindowViewModel)Application.Current.MainWindow.DataContext).GamepadService.CurrentGamepad;
			ControllerTypeEnum? controllerTypeEnum;
			if (currentGamepad == null)
			{
				controllerTypeEnum = null;
			}
			else
			{
				ControllerVM currentController = currentGamepad.CurrentController;
				controllerTypeEnum = ((currentController != null) ? currentController.FirstGamepadType : null);
			}
			ControllerTypeEnum? controllerTypeEnum2 = controllerTypeEnum;
			ControllerTypeEnum[] array = null;
			XBUtils.ExtractControllerType(parameter, ref controllerTypeEnum2);
			XBUtils.ExtractControllerTypes(parameter, ref array);
			if (controllerTypeEnum2 == null || !ControllerTypeExtensions.IsGamepad(controllerTypeEnum2.Value))
			{
				controllerTypeEnum2 = new ControllerTypeEnum?(3);
			}
			controllerTypeEnum2 = XBUtils.CorrectNintendoSwitchJoy(controllerTypeEnum2);
			if (array != null)
			{
				return XBUtils.ConvertGamepadButtonToDescription(gamepadButton, array);
			}
			return XBUtils.ConvertGamepadButtonToDescription(gamepadButton, controllerTypeEnum2);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			GamepadButton? gamepadButton = null;
			ControllerTypeEnum? controllerTypeEnum = null;
			ControllerTypeEnum[] array = null;
			XBBinding xbbinding = null;
			XBUtils.ExtractGamepadButton(values[0], ref gamepadButton);
			XBUtils.ExtractControllerType(values[0], ref controllerTypeEnum);
			XBUtils.ExtractControllerTypes(values[0], ref array);
			XBUtils.ExtractXBBinding(values[0], ref xbbinding);
			XBUtils.ExtractGamepadButton(values[1], ref gamepadButton);
			XBUtils.ExtractControllerType(values[1], ref controllerTypeEnum);
			XBUtils.ExtractControllerTypes(values[1], ref array);
			XBUtils.ExtractXBBinding(values[1], ref xbbinding);
			if (gamepadButton == null)
			{
				return null;
			}
			if (array != null)
			{
				return XBUtils.ConvertGamepadButtonToDescription(gamepadButton.Value, array);
			}
			if (controllerTypeEnum == null)
			{
				controllerTypeEnum = new ControllerTypeEnum?(XBUtils.GetDefaultGamepadControllerType(xbbinding));
			}
			string text = XBUtils.ConvertGamepadButtonToDescription(gamepadButton.Value, controllerTypeEnum);
			if (string.IsNullOrEmpty(text))
			{
				IGamepadService gamepadService = App.GamepadService;
				ControllerTypeEnum? controllerTypeEnum2;
				if (gamepadService == null)
				{
					controllerTypeEnum2 = null;
				}
				else
				{
					BaseControllerVM currentGamepad = gamepadService.CurrentGamepad;
					if (currentGamepad == null)
					{
						controllerTypeEnum2 = null;
					}
					else
					{
						ControllerVM currentController = currentGamepad.CurrentController;
						controllerTypeEnum2 = ((currentController != null) ? new ControllerTypeEnum?(currentController.ControllerType) : null);
					}
				}
				controllerTypeEnum = new ControllerTypeEnum?(controllerTypeEnum2 ?? 3);
				text = XBUtils.ConvertGamepadButtonToDescription(gamepadButton.Value, controllerTypeEnum);
				if (string.IsNullOrEmpty(text) && ControllerTypeExtensions.IsNintendoSwitchJoyCon(controllerTypeEnum.Value))
				{
					text = XBUtils.ConvertGamepadButtonToDescription(gamepadButton.Value, new ControllerTypeEnum?(XBUtils.OppositeNintendoSwitchJoyCon(controllerTypeEnum.Value)));
				}
			}
			return text;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static GamepadButtonDescriptionConverter _converter;
	}
}
