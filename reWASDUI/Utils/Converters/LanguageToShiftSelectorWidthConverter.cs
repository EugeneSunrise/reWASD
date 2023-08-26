using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using DiscSoft.NET.Common.Localization;

namespace reWASDUI.Utils.Converters
{
	[ValueConversion(typeof(Language), typeof(double))]
	public class LanguageToShiftSelectorWidthConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			LanguageToShiftSelectorWidthConverter languageToShiftSelectorWidthConverter;
			if ((languageToShiftSelectorWidthConverter = LanguageToShiftSelectorWidthConverter._converter) == null)
			{
				languageToShiftSelectorWidthConverter = (LanguageToShiftSelectorWidthConverter._converter = new LanguageToShiftSelectorWidthConverter());
			}
			return languageToShiftSelectorWidthConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return null;
			}
			Language language = value as Language;
			if (language != null)
			{
				string name = language.Name;
				if (name != null)
				{
					int length = name.Length;
					switch (length)
					{
					case 6:
					{
						char c = name[0];
						if (c != 'F')
						{
							if (c == 'G')
							{
								if (name == "German")
								{
									return 200;
								}
							}
						}
						else if (name == "French")
						{
							return 200;
						}
						break;
					}
					case 7:
					{
						char c = name[0];
						if (c != 'I')
						{
							if (c != 'R')
							{
								if (c == 'S')
								{
									if (name == "Spanish")
									{
										return 200;
									}
								}
							}
							else if (name == "Russian")
							{
								return 200;
							}
						}
						else if (name == "Italian")
						{
							return 200;
						}
						break;
					}
					case 8:
						if (name == "Japanese")
						{
							return 200;
						}
						break;
					default:
						if (length != 18)
						{
							if (length == 19)
							{
								if (name == "Chinese Traditional")
								{
									return 150;
								}
							}
						}
						else if (name == "Chinese Simplified")
						{
							return 150;
						}
						break;
					}
				}
			}
			return 150;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static LanguageToShiftSelectorWidthConverter _converter;
	}
}
