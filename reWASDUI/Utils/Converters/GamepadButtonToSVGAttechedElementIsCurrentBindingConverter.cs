using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using reWASDUI.Infrastructure.KeyBindings;

namespace reWASDUI.Utils.Converters
{
	public class GamepadButtonToSVGAttechedElementIsCurrentBindingConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Any((object v) => object.Equals(v, DependencyProperty.UnsetValue) || v == null))
			{
				return false;
			}
			AssociatedControllerButton associatedControllerButton = (AssociatedControllerButton)values[0];
			AssociatedControllerButton associatedControllerButton2 = (AssociatedControllerButton)values[1];
			if (associatedControllerButton2.ControllerBindingFrameMode != null && associatedControllerButton2.ControllerBindingFrameMode.Value == null && associatedControllerButton.IsVirtualGamepadMappingAllowed)
			{
				return true;
			}
			if (!associatedControllerButton.IsSet && !associatedControllerButton2.IsSet)
			{
				return false;
			}
			if (object.Equals(values[0], values[1]))
			{
				return true;
			}
			return false;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
