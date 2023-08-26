using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Utils.XBUtil;
using XBEliteWPF.Utils.XBUtilModel;

namespace XBEliteWPF.Utils.Converters
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
			ControllerTypeEnum? controllerTypeEnum = null;
			bool flag = false;
			GamepadButton? gamepadButton = null;
			KeyScanCodeV2 keyScanCodeV = null;
			XBUtils.ExtractGamepadButton(value, ref gamepadButton);
			XBUtils.ExtractGamepadButtonAndKeyScanCode(value, ref gamepadButton, ref keyScanCodeV);
			XBUtils.ExtractControllerType(value, ref controllerTypeEnum);
			XBUtils.ExtractGamepadButton(parameter, ref gamepadButton);
			XBUtils.ExtractGamepadButtonAndKeyScanCode(parameter, ref gamepadButton, ref keyScanCodeV);
			XBUtils.ExtractControllerType(parameter, ref controllerTypeEnum);
			string text = parameter as string;
			if (text != null && text == "Extended")
			{
				flag = true;
			}
			if (gamepadButton != null)
			{
				return XBUtils.GetAnnotationDrawingForGamepadButton(gamepadButton.Value, controllerTypeEnum, flag);
			}
			if (keyScanCodeV != null)
			{
				return XBUtils.GetAnnotationDrawingForBaseRewasdMapping(keyScanCodeV, true);
			}
			return null;
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
			XBUtils.ExtractGamepadButton(values[0], ref gamepadButton);
			XBUtils.ExtractGamepadButtonAndKeyScanCode(values[0], ref gamepadButton, ref keyScanCodeV);
			XBUtils.ExtractControllerType(values[0], ref controllerTypeEnum);
			XBUtils.ExtractGamepadButton(values[1], ref gamepadButton);
			XBUtils.ExtractGamepadButtonAndKeyScanCode(values[1], ref gamepadButton, ref keyScanCodeV);
			XBUtils.ExtractControllerType(values[1], ref controllerTypeEnum);
			if (controllerTypeEnum == null)
			{
				controllerTypeEnum = new ControllerTypeEnum?(XBUtils.GetDefaultGamepadControllerType());
			}
			string text = parameter as string;
			bool flag = text != null && text == "Extended";
			if (gamepadButton != null)
			{
				return XBUtils.GetAnnotationDrawingForGamepadButton(gamepadButton.Value, controllerTypeEnum, flag);
			}
			if (keyScanCodeV != null)
			{
				return XBUtils.GetAnnotationDrawingForBaseRewasdMapping(keyScanCodeV, true);
			}
			return null;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static GamepadButtonAnnotationIconConverter _converter;
	}
}
