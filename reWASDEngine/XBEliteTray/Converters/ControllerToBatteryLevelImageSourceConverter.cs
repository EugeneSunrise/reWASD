using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using XBEliteWPF.Infrastructure.Controller;

namespace XBEliteTray.Converters
{
	[MarkupExtensionReturnType(typeof(IValueConverter))]
	[ValueConversion(typeof(ControllerVM), typeof(ImageSource))]
	public class ControllerToBatteryLevelImageSourceConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			ControllerToBatteryLevelImageSourceConverter controllerToBatteryLevelImageSourceConverter;
			if ((controllerToBatteryLevelImageSourceConverter = ControllerToBatteryLevelImageSourceConverter._converter) == null)
			{
				controllerToBatteryLevelImageSourceConverter = (ControllerToBatteryLevelImageSourceConverter._converter = new ControllerToBatteryLevelImageSourceConverter());
			}
			return controllerToBatteryLevelImageSourceConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is ControllerVM)
			{
				ControllerVM controllerVM = value as ControllerVM;
				new DrawingImage();
				string text = "";
				switch (controllerVM.ControllerBatteryLevel)
				{
				case 0:
					text = "Battery_critical";
					break;
				case 1:
					text = "Battery_low";
					break;
				case 2:
					text = "Battery_medium";
					break;
				case 3:
					text = "Battery_high";
					break;
				case 4:
					text = "Battery_unknown";
					break;
				}
				return Application.Current.TryFindResource(text) as ImageSource;
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static ControllerToBatteryLevelImageSourceConverter _converter;
	}
}
