using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using reWASDUI.Infrastructure.KeyBindings;

namespace reWASDUI.Utils.Converters
{
	public class IsShiftModificatorSetConverter : MarkupExtension, IMultiValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			IsShiftModificatorSetConverter isShiftModificatorSetConverter;
			if ((isShiftModificatorSetConverter = IsShiftModificatorSetConverter._converter) == null)
			{
				isShiftModificatorSetConverter = (IsShiftModificatorSetConverter._converter = new IsShiftModificatorSetConverter());
			}
			return isShiftModificatorSetConverter;
		}

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Length < 2)
			{
				return false;
			}
			AssociatedControllerButton associatedControllerButton = values[0] as AssociatedControllerButton;
			if (associatedControllerButton != null)
			{
				AssociatedControllerButton associatedControllerButton2 = values[1] as AssociatedControllerButton;
				if (associatedControllerButton2 != null)
				{
					return associatedControllerButton.IsAssociatedSetToEqualButtons(associatedControllerButton2);
				}
			}
			return false;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static IsShiftModificatorSetConverter _converter;
	}
}
