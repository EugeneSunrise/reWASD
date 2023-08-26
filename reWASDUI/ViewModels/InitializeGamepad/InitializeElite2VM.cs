using System;
using System.Runtime.CompilerServices;
using System.Windows;
using DiscSoft.NET.Common.Localization;
using Prism.Events;
using Prism.Ioc;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;

namespace reWASDUI.ViewModels.InitializeGamepad
{
	public class InitializeElite2VM : BaseInitializeGamepadVM
	{
		public string BTItemStr
		{
			get
			{
				return string.Format(DTLocalization.GetString(12541), 0);
			}
		}

		public string FirstItemStr
		{
			get
			{
				BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
				if (currentGamepad == null || !currentGamepad.IsBluetoothConnectionFlagPresent)
				{
					string @string = DTLocalization.GetString(12545);
					object obj = 0;
					BaseControllerVM currentGamepad2 = App.GamepadService.CurrentGamepad;
					object obj2;
					if (currentGamepad2 == null)
					{
						obj2 = null;
					}
					else
					{
						ControllerVM currentController = currentGamepad2.CurrentController;
						obj2 = ((currentController != null) ? currentController.FirmwareVersion : null);
					}
					return string.Format(@string, obj, obj2);
				}
				return string.Format(DTLocalization.GetString(12544), 1);
			}
		}

		public string EmptyProfileStr
		{
			get
			{
				string @string = DTLocalization.GetString(12542);
				BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
				return string.Format(@string, (currentGamepad != null && currentGamepad.IsBluetoothConnectionFlagPresent) ? 2 : 1);
			}
		}

		public string CloneProfileStr
		{
			get
			{
				string @string = DTLocalization.GetString(12543);
				BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
				return string.Format(@string, (currentGamepad != null && currentGamepad.IsBluetoothConnectionFlagPresent) ? 3 : 2);
			}
		}

		public Visibility IsModernFirmware
		{
			get
			{
				bool flag = false;
				BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
				ControllerVM controllerVM = ((currentGamepad != null) ? currentGamepad.CurrentController : null);
				if (controllerVM != null)
				{
					flag = controllerVM.FirmwareVersionMajor > 5 || (controllerVM.FirmwareVersionMajor == 5 && controllerVM.FirmwareVersionMinor > 13) || (controllerVM.FirmwareVersionMajor == 5 && controllerVM.FirmwareVersionMinor == 13 && controllerVM.FirmwareVersionBuild >= 3143);
				}
				if (!flag)
				{
					return Visibility.Visible;
				}
				return Visibility.Collapsed;
			}
		}

		public InitializeElite2VM(IContainerProvider uc)
			: base(uc)
		{
			IEventAggregator eventAggregator = App.EventAggregator;
			if (eventAggregator != null)
			{
				eventAggregator.GetEvent<CurrentControllerDataChanged>().Subscribe(delegate(BaseControllerVM gamepad)
				{
					this.UpdateProperies();
				});
			}
			IEventAggregator eventAggregator2 = App.EventAggregator;
			if (eventAggregator2 != null)
			{
				eventAggregator2.GetEvent<CurrentGamepadChanged>().Subscribe(delegate(BaseControllerVM gamepad)
				{
					this.UpdateProperies();
				});
			}
			TranslationManager.Instance.LanguageChanged += delegate([Nullable(2)] object sender, EventArgs args)
			{
				this.UpdateProperies();
			};
		}

		public void UpdateProperies()
		{
			this.OnPropertyChanged("FirstItemStr");
			this.OnPropertyChanged("EmptyProfileStr");
			this.OnPropertyChanged("CloneProfileStr");
			this.OnPropertyChanged("IsModernFirmware");
		}

		private const int ModernMajorFirmware = 5;

		private const int ModernMinorFirmware = 13;

		private const int ModernBuildFirmware = 3143;
	}
}
