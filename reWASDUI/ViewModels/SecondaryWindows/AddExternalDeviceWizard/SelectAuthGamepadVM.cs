using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using reWASDUI.Services.Interfaces;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class SelectAuthGamepadVM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.SelectAuthGamepad;
			}
		}

		public IGamepadService GamepadService
		{
			get
			{
				return App.GamepadService;
			}
		}

		public AdaptersSettingsVM AdapterSettingsPage
		{
			get
			{
				return (AdaptersSettingsVM)this._wizard.FindPage(PageType.AdaptersSettings);
			}
		}

		public string Header
		{
			get
			{
				return DTLocalization.GetString(12780);
			}
		}

		public string Message
		{
			get
			{
				return string.Format(DTLocalization.GetString(12781), this.AdapterSettingsPage.USBTargetConsoleType.TryGetDescription());
			}
		}

		public Drawing GamepadImage
		{
			get
			{
				if (this.AdapterSettingsPage.USBTargetConsoleType == 3)
				{
					return Application.Current.TryFindResource("Connect_Xbox") as Drawing;
				}
				if (this.AdapterSettingsPage.USBTargetConsoleType == 1)
				{
					return Application.Current.TryFindResource("Connect_PlayStation_4") as Drawing;
				}
				return null;
			}
		}

		public SelectAuthGamepadVM(WizardVM wizard)
			: base(wizard)
		{
		}

		protected override void NavigatePreviousPage()
		{
			base.GoPage(PageType.AdaptersSettings);
		}

		protected override async void NavigateToNextPage()
		{
			TaskAwaiter<bool> taskAwaiter = this.AdapterSettingsPage.Save().GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<bool> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			if (taskAwaiter.GetResult())
			{
				base.GoPage(PageType.FinishAndWaitUSBClient);
			}
		}
	}
}
