using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.KeyBindings;

namespace reWASDUI.Utils.Converters
{
	[ValueConversion(typeof(GamepadButton), typeof(AssociatedControllerButton))]
	public class GamepadButtonToAssociatedControllerButton : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			GamepadButtonToAssociatedControllerButton gamepadButtonToAssociatedControllerButton;
			if ((gamepadButtonToAssociatedControllerButton = GamepadButtonToAssociatedControllerButton._converter) == null)
			{
				gamepadButtonToAssociatedControllerButton = (GamepadButtonToAssociatedControllerButton._converter = new GamepadButtonToAssociatedControllerButton());
			}
			return gamepadButtonToAssociatedControllerButton;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is GamepadButton)
			{
				GamepadButton gamepadButton = (GamepadButton)value;
				return new AssociatedControllerButton(gamepadButton);
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static GamepadButtonToAssociatedControllerButton _converter;
	}
}
