using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.XB;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtilModel;

namespace reWASDUI.Utils.Converters
{
	[ValueConversion(typeof(Direction), typeof(Drawing))]
	public class DirectionToDrawingConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			DirectionToDrawingConverter directionToDrawingConverter;
			if ((directionToDrawingConverter = DirectionToDrawingConverter._converter) == null)
			{
				directionToDrawingConverter = (DirectionToDrawingConverter._converter = new DirectionToDrawingConverter());
			}
			return directionToDrawingConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return null;
			}
			Direction? direction = new Direction?((Direction)value);
			BaseXBBindingCollection realCurrentBeingMappedBindingCollection = App.GameProfilesService.RealCurrentBeingMappedBindingCollection;
			bool flag;
			if (realCurrentBeingMappedBindingCollection == null)
			{
				flag = false;
			}
			else
			{
				bool? flag2;
				if (realCurrentBeingMappedBindingCollection.CurrentButtonMapping == null)
				{
					flag2 = null;
				}
				else
				{
					KeyValuePair<GamepadButton, XBBinding>? keyValuePair;
					XBBinding value2 = keyValuePair.GetValueOrDefault().Value;
					flag2 = ((value2 != null) ? new bool?(GamepadButtonExtensions.IsAnyPhysicalTrackPad(value2.GamepadButton)) : null);
				}
				bool? flag3 = flag2;
				bool flag4 = true;
				flag = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
			}
			return XBUtils.GetDrawingForDirection(direction, flag);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static DirectionToDrawingConverter _converter;
	}
}
