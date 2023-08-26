using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using XBEliteWPF.Infrastructure.reWASDMapping;
using XBEliteWPF.Utils.XBUtilModel;

namespace reWASDUI.Utils.Converters
{
	public class BaseRewasdMappingAnnotationIconConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			BaseRewasdMappingAnnotationIconConverter baseRewasdMappingAnnotationIconConverter;
			if ((baseRewasdMappingAnnotationIconConverter = BaseRewasdMappingAnnotationIconConverter._converter) == null)
			{
				baseRewasdMappingAnnotationIconConverter = (BaseRewasdMappingAnnotationIconConverter._converter = new BaseRewasdMappingAnnotationIconConverter());
			}
			return baseRewasdMappingAnnotationIconConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is BaseRewasdMapping))
			{
				return null;
			}
			return XBUtils.GetAnnotationDrawingForBaseRewasdMapping((BaseRewasdMapping)value, parameter == null);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static BaseRewasdMappingAnnotationIconConverter _converter;
	}
}
