using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Infrastructure.KeyBindingsModel.XBBindingDirectionalGroups;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtilModel;

namespace reWASDUI.Utils.Converters
{
	[ValueConversion(typeof(GamepadButton), typeof(Drawing))]
	[ValueConversion(typeof(GamepadButtonDescription), typeof(Drawing))]
	public class GamepadVirtualButtonAnnotationIconConverter : MarkupExtension, IValueConverter, IMultiValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			GamepadVirtualButtonAnnotationIconConverter gamepadVirtualButtonAnnotationIconConverter;
			if ((gamepadVirtualButtonAnnotationIconConverter = GamepadVirtualButtonAnnotationIconConverter._converter) == null)
			{
				gamepadVirtualButtonAnnotationIconConverter = (GamepadVirtualButtonAnnotationIconConverter._converter = new GamepadVirtualButtonAnnotationIconConverter());
			}
			return gamepadVirtualButtonAnnotationIconConverter;
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
			GamepadButton? gamepadButton = null;
			KeyScanCodeV2 keyScanCodeV = null;
			if (value is GamepadButton)
			{
				GamepadButton gamepadButton2 = (GamepadButton)value;
				gamepadButton = new GamepadButton?(gamepadButton2);
			}
			GamepadButtonDescription gamepadButtonDescription = value as GamepadButtonDescription;
			if (gamepadButtonDescription != null)
			{
				gamepadButton = new GamepadButton?(gamepadButtonDescription.Button);
			}
			KeyScanCodeV2 keyScanCodeV2 = value as KeyScanCodeV2;
			if (keyScanCodeV2 != null)
			{
				keyScanCodeV = keyScanCodeV2;
			}
			AssociatedControllerButton associatedControllerButton = value as AssociatedControllerButton;
			if (associatedControllerButton != null)
			{
				associatedControllerButton.SetRefButtons(ref gamepadButton, ref keyScanCodeV);
			}
			if (value is VirtualGamepadType)
			{
				VirtualGamepadType virtualGamepadType = (VirtualGamepadType)value;
				controllerTypeEnum2 = new ControllerTypeEnum?(VirtualControllerTypeExtensions.GetControllerType(virtualGamepadType));
			}
			if (value is ControllerTypeEnum)
			{
				ControllerTypeEnum controllerTypeEnum3 = (ControllerTypeEnum)value;
				controllerTypeEnum2 = new ControllerTypeEnum?(controllerTypeEnum3);
			}
			ControllerTypeEnum[] array = value as ControllerTypeEnum[];
			if (array != null)
			{
				controllerTypeEnum2 = new ControllerTypeEnum?(ControllerTypeExtensions.GetFirstOfFamily(array, 0));
			}
			if (value is GamepadButton)
			{
				GamepadButton gamepadButton3 = (GamepadButton)value;
				gamepadButton = new GamepadButton?(gamepadButton3);
			}
			GamepadButtonDescription gamepadButtonDescription2 = parameter as GamepadButtonDescription;
			if (gamepadButtonDescription2 != null)
			{
				gamepadButton = new GamepadButton?(gamepadButtonDescription2.Button);
			}
			KeyScanCodeV2 keyScanCodeV3 = parameter as KeyScanCodeV2;
			if (keyScanCodeV3 != null)
			{
				keyScanCodeV = keyScanCodeV3;
			}
			AssociatedControllerButton associatedControllerButton2 = parameter as AssociatedControllerButton;
			if (associatedControllerButton2 != null)
			{
				associatedControllerButton2.SetRefButtons(ref gamepadButton, ref keyScanCodeV);
			}
			if (parameter is VirtualGamepadType)
			{
				VirtualGamepadType virtualGamepadType2 = (VirtualGamepadType)parameter;
				controllerTypeEnum2 = new ControllerTypeEnum?(VirtualControllerTypeExtensions.GetControllerType(virtualGamepadType2));
			}
			if (parameter is ControllerTypeEnum)
			{
				ControllerTypeEnum controllerTypeEnum4 = (ControllerTypeEnum)parameter;
				controllerTypeEnum2 = new ControllerTypeEnum?(controllerTypeEnum4);
			}
			ControllerTypeEnum[] array2 = parameter as ControllerTypeEnum[];
			if (array2 != null)
			{
				controllerTypeEnum2 = new ControllerTypeEnum?(ControllerTypeExtensions.GetFirstOfFamily(array2, 0));
			}
			if (keyScanCodeV != null)
			{
				gamepadButton = keyScanCodeV.VirtualGamepadButton;
			}
			if (gamepadButton != null)
			{
				return XBUtils.GetAnnotationDrawingForVirtualGamepadButton(gamepadButton.Value, null, null, controllerTypeEnum2, null);
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			VirtualGamepadType virtualGamepadType = 0;
			ControllerTypeEnum controllerTypeEnum = 3;
			GamepadButton? gamepadButton = null;
			KeyScanCodeV2 keyScanCodeV = null;
			BaseXBBindingCollection baseXBBindingCollection = null;
			BaseXBBindingCollection baseXBBindingCollection2 = null;
			this.ProcessArrayItem(values[0], ref gamepadButton, ref keyScanCodeV, ref virtualGamepadType, ref controllerTypeEnum, ref baseXBBindingCollection, ref baseXBBindingCollection2);
			if (values.Length > 1)
			{
				this.ProcessArrayItem(values[1], ref gamepadButton, ref keyScanCodeV, ref virtualGamepadType, ref controllerTypeEnum, ref baseXBBindingCollection, ref baseXBBindingCollection2);
			}
			if (values.Length > 2)
			{
				this.ProcessArrayItem(values[2], ref gamepadButton, ref keyScanCodeV, ref virtualGamepadType, ref controllerTypeEnum, ref baseXBBindingCollection, ref baseXBBindingCollection2);
			}
			if (values.Length > 3)
			{
				this.ProcessArrayItem(values[3], ref gamepadButton, ref keyScanCodeV, ref virtualGamepadType, ref controllerTypeEnum, ref baseXBBindingCollection, ref baseXBBindingCollection2);
			}
			if (keyScanCodeV != null)
			{
				gamepadButton = keyScanCodeV.VirtualGamepadButton;
			}
			bool? flag = null;
			bool? flag2 = null;
			if (baseXBBindingCollection != null)
			{
				BaseTouchpadDirectionalGroup baseTouchpadDirectionalGroup = ((baseXBBindingCollection != null) ? baseXBBindingCollection.GetDirectionalGroupByXBBinding(gamepadButton.Value) : null) as BaseTouchpadDirectionalGroup;
				flag = ((baseTouchpadDirectionalGroup != null) ? new bool?(baseTouchpadDirectionalGroup.TouchpadDigitalMode) : null);
				flag2 = ((baseTouchpadDirectionalGroup != null) ? new bool?(baseTouchpadDirectionalGroup.TapValue.IsUnmapped) : null);
			}
			if (baseXBBindingCollection2 != null)
			{
				BaseTouchpadDirectionalGroup baseTouchpadDirectionalGroup2 = ((baseXBBindingCollection2 != null) ? baseXBBindingCollection2.GetDirectionalGroupByXBBinding(gamepadButton.Value) : null) as BaseTouchpadDirectionalGroup;
				flag = ((baseTouchpadDirectionalGroup2 != null) ? new bool?(baseTouchpadDirectionalGroup2.TouchpadDigitalMode) : null);
				flag2 = ((baseTouchpadDirectionalGroup2 != null) ? new bool?(baseTouchpadDirectionalGroup2.TapValue.IsUnmapped) : null);
			}
			if (gamepadButton != null)
			{
				return XBUtils.GetAnnotationDrawingForVirtualGamepadButton(gamepadButton.Value, flag, flag2, new ControllerTypeEnum?(VirtualControllerTypeExtensions.GetControllerType(virtualGamepadType)), new ControllerTypeEnum?(controllerTypeEnum));
			}
			return null;
		}

		private void ProcessArrayItem(object item, ref GamepadButton? gamepadButton, ref KeyScanCodeV2 keyScanCode, ref VirtualGamepadType virtualControllerType, ref ControllerTypeEnum realControllerType, ref BaseXBBindingCollection hostCollection, ref BaseXBBindingCollection hostCollectionModel)
		{
			if (item is GamepadButton)
			{
				GamepadButton gamepadButton2 = (GamepadButton)item;
				gamepadButton = new GamepadButton?(gamepadButton2);
			}
			GamepadButtonDescription gamepadButtonDescription = item as GamepadButtonDescription;
			if (gamepadButtonDescription != null)
			{
				gamepadButton = new GamepadButton?(gamepadButtonDescription.Button);
			}
			KeyScanCodeV2 keyScanCodeV = item as KeyScanCodeV2;
			if (keyScanCodeV != null)
			{
				keyScanCode = keyScanCodeV;
			}
			AssociatedControllerButton associatedControllerButton = item as AssociatedControllerButton;
			if (associatedControllerButton != null)
			{
				associatedControllerButton.SetRefButtons(ref gamepadButton, ref keyScanCode);
			}
			if (item is VirtualGamepadType)
			{
				VirtualGamepadType virtualGamepadType = (VirtualGamepadType)item;
				virtualControllerType = virtualGamepadType;
			}
			if (item is ControllerTypeEnum)
			{
				ControllerTypeEnum controllerTypeEnum = (ControllerTypeEnum)item;
				realControllerType = controllerTypeEnum;
			}
			ControllerTypeEnum[] array = item as ControllerTypeEnum[];
			if (array != null)
			{
				realControllerType = ControllerTypeExtensions.GetFirstOfFamily(array, 0);
			}
			IEnumerable<ControllerTypeEnum> enumerable = item as IEnumerable<ControllerTypeEnum>;
			if (enumerable != null)
			{
				realControllerType = ControllerTypeExtensions.GetFirstOfFamily(enumerable, 0);
			}
			BaseXBBindingCollection baseXBBindingCollection = item as BaseXBBindingCollection;
			if (baseXBBindingCollection != null)
			{
				hostCollection = baseXBBindingCollection;
			}
			BaseXBBindingCollection baseXBBindingCollection2 = item as BaseXBBindingCollection;
			if (baseXBBindingCollection2 != null)
			{
				hostCollectionModel = baseXBBindingCollection2;
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static GamepadVirtualButtonAnnotationIconConverter _converter;
	}
}
