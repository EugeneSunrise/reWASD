using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using reWASDCommon.Infrastructure.Enums;

namespace reWASDUI.Utils.Converters
{
	public class VirtualGamepadTypeToDrawingConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			VirtualGamepadTypeToDrawingConverter virtualGamepadTypeToDrawingConverter;
			if ((virtualGamepadTypeToDrawingConverter = VirtualGamepadTypeToDrawingConverter._converter) == null)
			{
				virtualGamepadTypeToDrawingConverter = (VirtualGamepadTypeToDrawingConverter._converter = new VirtualGamepadTypeToDrawingConverter());
			}
			return virtualGamepadTypeToDrawingConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is VirtualGamepadType)
			{
				VirtualGamepadType virtualGamepadType = (VirtualGamepadType)value;
				string text = "";
				switch (virtualGamepadType)
				{
				case 0:
					text = "VirtualXBOX360Mode";
					break;
				case 1:
					text = "VirtualXBOXOneMode";
					break;
				case 2:
					text = "VirtualDS4Mode";
					break;
				case 3:
					text = "VirtualDS3Mode";
					break;
				case 4:
					text = "VirtualNSProMode";
					break;
				}
				return Application.Current.TryFindResource(text);
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static VirtualGamepadTypeToDrawingConverter _converter;
	}
}
