using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.ViewModels;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Utils.Converters
{
	[ValueConversion(typeof(Slot), typeof(ImageSource))]
	public class SlotToGamepadButtonIconConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			SlotToGamepadButtonIconConverter slotToGamepadButtonIconConverter;
			if ((slotToGamepadButtonIconConverter = SlotToGamepadButtonIconConverter._converter) == null)
			{
				slotToGamepadButtonIconConverter = (SlotToGamepadButtonIconConverter._converter = new SlotToGamepadButtonIconConverter());
			}
			return slotToGamepadButtonIconConverter;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			BaseControllerVM currentGamepad = ((MainWindowViewModel)Application.Current.MainWindow.DataContext).GamepadService.CurrentGamepad;
			ControllerTypeEnum? controllerTypeEnum;
			if (currentGamepad == null)
			{
				controllerTypeEnum = null;
			}
			else
			{
				ControllerVM currentController = currentGamepad.CurrentController;
				controllerTypeEnum = ((currentController != null) ? new ControllerTypeEnum?(currentController.FirstControllerType) : null);
			}
			ControllerTypeEnum controllerTypeEnum2 = controllerTypeEnum ?? 3;
			Drawing drawing = null;
			switch ((Slot)value)
			{
			case 0:
				drawing = (ControllerTypeExtensions.IsSony(controllerTypeEnum2) ? (Application.Current.TryFindResource("IcoDSBtnSquare") as Drawing) : (Application.Current.TryFindResource("IcoXbBtnX") as Drawing));
				break;
			case 1:
				drawing = (ControllerTypeExtensions.IsSony(controllerTypeEnum2) ? (Application.Current.TryFindResource("IcoDSBtnTriangle") as Drawing) : (Application.Current.TryFindResource("IcoXbBtnY") as Drawing));
				break;
			case 2:
				drawing = (ControllerTypeExtensions.IsSony(controllerTypeEnum2) ? (Application.Current.TryFindResource("IcoDSBtnCircle") as Drawing) : (Application.Current.TryFindResource("IcoXbBtnB") as Drawing));
				break;
			case 3:
				drawing = (ControllerTypeExtensions.IsSony(controllerTypeEnum2) ? (Application.Current.TryFindResource("IcoDSBtnCross") as Drawing) : (Application.Current.TryFindResource("IcoXbBtnA") as Drawing));
				break;
			}
			return drawing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static SlotToGamepadButtonIconConverter _converter;
	}
}
