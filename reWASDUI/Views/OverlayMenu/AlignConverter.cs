using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace reWASDUI.Views.OverlayMenu
{
	public class AlignConverter : MarkupExtension, IValueConverter, IMultiValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			AlignConverter alignConverter;
			if ((alignConverter = AlignConverter._converter) == null)
			{
				alignConverter = (AlignConverter._converter = new AlignConverter());
			}
			return alignConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (double)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			double num = 0.0;
			if (values.Length == 4)
			{
				string text = parameter as string;
				if (text != null)
				{
					object obj = values[0];
					if (obj is double)
					{
						double num2 = (double)obj;
						obj = values[1];
						if (obj is double)
						{
							double num3 = (double)obj;
							obj = values[2];
							if (obj is bool)
							{
								bool flag = (bool)obj;
								obj = values[3];
								if (obj is double)
								{
									double num4 = (double)obj;
									if (text.Equals("Top"))
									{
										num = num2 - num3 / 2.0;
									}
									else if (flag)
									{
										num = num2 - num3;
									}
									else
									{
										num = num2;
									}
									if (num < 0.0)
									{
										num = 0.0;
									}
									if (num + num3 > num4)
									{
										num = num4 - num3;
									}
								}
							}
						}
					}
				}
			}
			return num;
		}

		private static AlignConverter _converter;
	}
}
