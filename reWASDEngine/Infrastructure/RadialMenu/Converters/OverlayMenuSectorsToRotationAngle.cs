using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace reWASDEngine.Infrastructure.RadialMenu.Converters
{
	internal class OverlayMenuSectorsToRotationAngle : MarkupExtension, IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			int count = ((IList)values[0]).Count;
			int num = (int)values[1];
			return 360.0 / (double)count * (double)num;
		}

		public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
		{
			throw new InvalidOperationException("RadialMenuItemToArrowPosition is a One-Way converter only !");
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			OverlayMenuSectorsToRotationAngle overlayMenuSectorsToRotationAngle;
			if ((overlayMenuSectorsToRotationAngle = OverlayMenuSectorsToRotationAngle._converter) == null)
			{
				overlayMenuSectorsToRotationAngle = (OverlayMenuSectorsToRotationAngle._converter = new OverlayMenuSectorsToRotationAngle());
			}
			return overlayMenuSectorsToRotationAngle;
		}

		private static OverlayMenuSectorsToRotationAngle _converter;
	}
}
