using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using reWASDUI.Infrastructure.KeyBindings.ControllerBindings;
using reWASDUI.Infrastructure.KeyBindings.XB;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;

namespace reWASDUI.Utils.Converters
{
	public class BindToXBBindingFromControllerBindingCollectionConverter : MarkupExtension, IMultiValueConverter, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			BindToXBBindingFromControllerBindingCollectionConverter bindToXBBindingFromControllerBindingCollectionConverter;
			if ((bindToXBBindingFromControllerBindingCollectionConverter = BindToXBBindingFromControllerBindingCollectionConverter._converter) == null)
			{
				bindToXBBindingFromControllerBindingCollectionConverter = (BindToXBBindingFromControllerBindingCollectionConverter._converter = new BindToXBBindingFromControllerBindingCollectionConverter());
			}
			return bindToXBBindingFromControllerBindingCollectionConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			object obj = null;
			ControllerBindingsCollection controllerBindingsCollection = value as ControllerBindingsCollection;
			if (controllerBindingsCollection != null)
			{
				if (parameter is MouseButton)
				{
					MouseButton mb = (MouseButton)parameter;
					obj = controllerBindingsCollection.FirstOrDefault((ControllerBinding cb) => cb.XBBinding.ControllerButton.KeyScanCode == KeyScanCodeV2.FindKeyScanCodeByMouseButton(mb));
				}
				else
				{
					KeyScanCodeV2 ksc = parameter as KeyScanCodeV2;
					if (ksc != null)
					{
						obj = controllerBindingsCollection.FirstOrDefault((ControllerBinding cb) => cb.XBBinding.ControllerButton.KeyScanCode == ksc);
					}
				}
			}
			return obj;
		}

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			XBBinding xbbinding = null;
			ControllerBindingsCollection controllerBindingsCollection = values[0] as ControllerBindingsCollection;
			if (controllerBindingsCollection != null)
			{
				object obj = values[1];
				if (obj is MouseButton)
				{
					MouseButton mb = (MouseButton)obj;
					ControllerBinding controllerBinding = controllerBindingsCollection.FirstOrDefault((ControllerBinding cb) => cb.XBBinding.ControllerButton.KeyScanCode == KeyScanCodeV2.FindKeyScanCodeByMouseButton(mb));
					xbbinding = ((controllerBinding != null) ? controllerBinding.XBBinding : null);
				}
				else
				{
					obj = values[1];
					KeyScanCodeV2 ksc = obj as KeyScanCodeV2;
					if (ksc != null)
					{
						ControllerBinding controllerBinding2 = controllerBindingsCollection.FirstOrDefault((ControllerBinding cb) => cb.XBBinding.ControllerButton.KeyScanCode == ksc);
						xbbinding = ((controllerBinding2 != null) ? controllerBinding2.XBBinding : null);
					}
				}
			}
			return xbbinding;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static BindToXBBindingFromControllerBindingCollectionConverter _converter;
	}
}
