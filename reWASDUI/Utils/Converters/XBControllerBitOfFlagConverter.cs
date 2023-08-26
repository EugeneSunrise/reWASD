using System;
using System.Globalization;
using System.Windows.Data;
using reWASDCommon.Infrastructure.Enums;

namespace reWASDUI.Utils.Converters
{
	public class XBControllerBitOfFlagConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			XBControllers xbcontrollers = (XBControllers)parameter;
			this._target = (XBControllers)value;
			return (xbcontrollers & this._target) > 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			this._target ^= (XBControllers)parameter;
			return this._target;
		}

		private XBControllers _target;
	}
}
