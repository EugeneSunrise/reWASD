using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups;

namespace reWASDUI.Utils.Converters
{
	public class GetXBBindingFromDirectionalGroupByZoneAndDirectionConvertor : MarkupExtension, IMultiValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			GetXBBindingFromDirectionalGroupByZoneAndDirectionConvertor getXBBindingFromDirectionalGroupByZoneAndDirectionConvertor;
			if ((getXBBindingFromDirectionalGroupByZoneAndDirectionConvertor = GetXBBindingFromDirectionalGroupByZoneAndDirectionConvertor._converter) == null)
			{
				getXBBindingFromDirectionalGroupByZoneAndDirectionConvertor = (GetXBBindingFromDirectionalGroupByZoneAndDirectionConvertor._converter = new GetXBBindingFromDirectionalGroupByZoneAndDirectionConvertor());
			}
			return getXBBindingFromDirectionalGroupByZoneAndDirectionConvertor;
		}

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			BaseDirectionalAnalogGroup baseDirectionalAnalogGroup = null;
			Zone? zone = null;
			Direction? direction = null;
			object obj = values[0];
			if (obj is Zone)
			{
				Zone zone2 = (Zone)obj;
				zone = new Zone?(zone2);
			}
			else
			{
				obj = values[0];
				if (obj is Direction)
				{
					Direction direction2 = (Direction)obj;
					direction = new Direction?(direction2);
				}
				else
				{
					BaseDirectionalAnalogGroup baseDirectionalAnalogGroup2 = values[0] as BaseDirectionalAnalogGroup;
					if (baseDirectionalAnalogGroup2 != null)
					{
						baseDirectionalAnalogGroup = baseDirectionalAnalogGroup2;
					}
				}
			}
			obj = values[1];
			if (obj is Zone)
			{
				Zone zone3 = (Zone)obj;
				zone = new Zone?(zone3);
			}
			else
			{
				obj = values[1];
				if (obj is Direction)
				{
					Direction direction3 = (Direction)obj;
					direction = new Direction?(direction3);
				}
				else
				{
					BaseDirectionalAnalogGroup baseDirectionalAnalogGroup3 = values[1] as BaseDirectionalAnalogGroup;
					if (baseDirectionalAnalogGroup3 != null)
					{
						baseDirectionalAnalogGroup = baseDirectionalAnalogGroup3;
					}
				}
			}
			obj = values[2];
			if (obj is Zone)
			{
				Zone zone4 = (Zone)obj;
				zone = new Zone?(zone4);
			}
			else
			{
				obj = values[2];
				if (obj is Direction)
				{
					Direction direction4 = (Direction)obj;
					direction = new Direction?(direction4);
				}
				else
				{
					BaseDirectionalAnalogGroup baseDirectionalAnalogGroup4 = values[2] as BaseDirectionalAnalogGroup;
					if (baseDirectionalAnalogGroup4 != null)
					{
						baseDirectionalAnalogGroup = baseDirectionalAnalogGroup4;
					}
				}
			}
			if (zone == null || direction == null)
			{
				return null;
			}
			if (baseDirectionalAnalogGroup == null)
			{
				return null;
			}
			return baseDirectionalAnalogGroup.GetXBBindingByZoneAndDirection(zone.Value, direction.Value);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static GetXBBindingFromDirectionalGroupByZoneAndDirectionConvertor _converter;
	}
}
