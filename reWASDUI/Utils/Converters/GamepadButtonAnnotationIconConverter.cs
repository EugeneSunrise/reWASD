using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Infrastructure.SupportedControllers;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.Services.Interfaces;
using reWASDUI.Utils.XBUtil;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtilModel;

namespace reWASDUI.Utils.Converters
{
	[ValueConversion(typeof(GamepadButton), typeof(Drawing))]
	[ValueConversion(typeof(GamepadButtonDescription), typeof(Drawing))]
	public class GamepadButtonAnnotationIconConverter : MarkupExtension, IValueConverter, IMultiValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			GamepadButtonAnnotationIconConverter gamepadButtonAnnotationIconConverter;
			if ((gamepadButtonAnnotationIconConverter = GamepadButtonAnnotationIconConverter._converter) == null)
			{
				gamepadButtonAnnotationIconConverter = (GamepadButtonAnnotationIconConverter._converter = new GamepadButtonAnnotationIconConverter());
			}
			return gamepadButtonAnnotationIconConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
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
			bool flag = false;
			GamepadButton? gamepadButton = null;
			KeyScanCodeV2 keyScanCodeV = null;
			ControllerTypeEnum[] array = null;
			XBUtils.ExtractGamepadButton(value, ref gamepadButton);
			XBUtils.ExtractGamepadButtonAndKeyScanCode(value, ref gamepadButton, ref keyScanCodeV);
			XBUtils.ExtractControllerType(value, ref controllerTypeEnum2);
			XBUtils.ExtractControllerTypes(value, ref array);
			XBUtils.ExtractGamepadButton(parameter, ref gamepadButton);
			XBUtils.ExtractGamepadButtonAndKeyScanCode(parameter, ref gamepadButton, ref keyScanCodeV);
			XBUtils.ExtractControllerType(parameter, ref controllerTypeEnum2);
			XBUtils.ExtractControllerTypes(value, ref array);
			string text = parameter as string;
			if (text != null && text == "Extended")
			{
				flag = true;
			}
			if (gamepadButton != null)
			{
				controllerTypeEnum2 = XBUtils.CorrectNintendoSwitchJoy(controllerTypeEnum2);
				if (array != null)
				{
					return XBUtils.GetAnnotationDrawingForGamepadButton(gamepadButton.Value, array, flag);
				}
				return XBUtils.GetAnnotationDrawingForGamepadButton(gamepadButton.Value, controllerTypeEnum2, flag);
			}
			else
			{
				if (keyScanCodeV != null)
				{
					return XBUtils.GetAnnotationDrawingForBaseRewasdMapping(keyScanCodeV, true);
				}
				return null;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			GamepadButton? gamepadButton = null;
			KeyScanCodeV2 keyScanCodeV = null;
			ControllerTypeEnum? controllerTypeEnum = null;
			ControllerTypeEnum[] array = null;
			XBBinding xbbinding = null;
			XBUtils.ExtractGamepadButton(values[0], ref gamepadButton);
			XBUtils.ExtractGamepadButtonAndKeyScanCode(values[0], ref gamepadButton, ref keyScanCodeV);
			XBUtils.ExtractControllerType(values[0], ref controllerTypeEnum);
			XBUtils.ExtractControllerTypes(values[0], ref array);
			XBUtils.ExtractXBBinding(values[0], ref xbbinding);
			XBUtils.ExtractGamepadButton(values[1], ref gamepadButton);
			XBUtils.ExtractGamepadButtonAndKeyScanCode(values[1], ref gamepadButton, ref keyScanCodeV);
			XBUtils.ExtractControllerType(values[1], ref controllerTypeEnum);
			XBUtils.ExtractControllerTypes(values[1], ref array);
			XBUtils.ExtractXBBinding(values[1], ref xbbinding);
			if (controllerTypeEnum == null)
			{
				controllerTypeEnum = new ControllerTypeEnum?(XBUtils.GetDefaultGamepadControllerType(xbbinding));
			}
			string text = parameter as string;
			bool flag = text != null && text == "Extended";
			if (gamepadButton != null)
			{
				if (array != null)
				{
					return XBUtils.GetAnnotationDrawingForGamepadButton(gamepadButton.Value, array, flag);
				}
				GamepadButton? gamepadButton2 = gamepadButton;
				GamepadButton gamepadButton3 = 2001;
				if ((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null))
				{
					return Application.Current.TryFindResource("UriKeyScanCodeEmpty") as Drawing;
				}
				gamepadButton2 = gamepadButton;
				gamepadButton3 = 2000;
				if ((gamepadButton2.GetValueOrDefault() == gamepadButton3) & (gamepadButton2 != null))
				{
					return Application.Current.TryFindResource("UriBtnUnmapped") as Drawing;
				}
				string text2 = ControllersHelper.GetAnnotationDrawingResourceNameForGamepadButton(gamepadButton.Value, controllerTypeEnum.Value, flag);
				if (string.IsNullOrEmpty(text2) || text2 == "UriBtnUnknown")
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
					text2 = ControllersHelper.GetAnnotationDrawingResourceNameForGamepadButton(gamepadButton.Value, controllerTypeEnum ?? 3, flag);
					if (string.IsNullOrEmpty(text2) || (text2 == "UriBtnUnknown" && ControllerTypeExtensions.IsNintendoSwitchJoyCon(controllerTypeEnum.Value)))
					{
						text2 = ControllersHelper.GetAnnotationDrawingResourceNameForGamepadButton(gamepadButton.Value, XBUtils.OppositeNintendoSwitchJoyCon(controllerTypeEnum.Value), flag);
					}
				}
				return Application.Current.TryFindResource(text2) as Drawing;
			}
			else
			{
				if (keyScanCodeV != null)
				{
					return XBUtils.GetAnnotationDrawingForBaseRewasdMapping(keyScanCodeV, true);
				}
				return null;
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static GamepadButtonAnnotationIconConverter _converter;
	}
}
