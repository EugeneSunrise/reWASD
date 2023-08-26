using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils.ExtensionMethods;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class FinishAndWaitUSBClientVM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.FinishAndWaitUSBClient;
			}
		}

		public AdaptersSettingsVM AdapterSettingsPage
		{
			get
			{
				return (AdaptersSettingsVM)this._wizard.FindPage(PageType.AdaptersSettings);
			}
		}

		public bool IsWaitingForConnection
		{
			get
			{
				return this.isMonitoring;
			}
		}

		public Drawing StatusImage
		{
			get
			{
				if (this.IsWaitingForConnection)
				{
					if (this.AdapterSettingsPage.USBTargetConsoleType == 3)
					{
						return Application.Current.TryFindResource("Connect_to_Xbox") as Drawing;
					}
					if (this.AdapterSettingsPage.USBTargetConsoleType == 1)
					{
						return Application.Current.TryFindResource("Connect_to_PlayStation_4") as Drawing;
					}
					return Application.Current.TryFindResource("Connect_to_target_device") as Drawing;
				}
				else
				{
					if (this.AdapterSettingsPage.USBTargetConsoleType == 3)
					{
						return Application.Current.TryFindResource("Connected_Xbox") as Drawing;
					}
					if (this.AdapterSettingsPage.USBTargetConsoleType == 1)
					{
						return Application.Current.TryFindResource("Connected_PlayStation") as Drawing;
					}
					return Application.Current.TryFindResource("Connected_target_device") as Drawing;
				}
			}
		}

		public string Header
		{
			get
			{
				if (this.IsWaitingForConnection)
				{
					return DTLocalization.GetString(12782);
				}
				return DTLocalization.GetString(12699);
			}
		}

		public string Message
		{
			get
			{
				string text = this.AdapterSettingsPage.USBTargetConsoleType.TryGetDescription();
				if (this.AdapterSettingsPage.USBTargetConsoleType == null)
				{
					text = DTLocalization.GetString(12020);
				}
				if (this.IsWaitingForConnection)
				{
					return string.Format(DTLocalization.GetString(12783), text);
				}
				return string.Format(DTLocalization.GetString(12817), text);
			}
		}

		public FinishAndWaitUSBClientVM(WizardVM wizard)
			: base(wizard)
		{
		}

		public override async void OnShowPage()
		{
			base.OnShowPage();
			this.isMonitoring = true;
			this.RefreshProperties();
			TaskAwaiter<bool> taskAwaiter = App.GameProfilesService.ApplyCurrentProfile(false).GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<bool> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			if (!taskAwaiter.GetResult())
			{
				this.NavigatePreviousPage();
			}
			else
			{
				while (App.GamepadService.ExternalDeviceRelationsHelper.CurrentExternalState != 9)
				{
					await Task.Delay(500);
					if (!this.isMonitoring)
					{
						return;
					}
				}
				this.isMonitoring = false;
				this.RefreshProperties();
			}
		}

		private void RefreshProperties()
		{
			this.OnPropertyChanged("IsWaitingForConnection");
			this.OnPropertyChanged("Message");
			this.OnPropertyChanged("Header");
			this.OnPropertyChanged("StatusImage");
		}

		public override void OnHidePage()
		{
			base.OnHidePage();
			this.isMonitoring = false;
		}

		protected override void NavigatePreviousPage()
		{
			if (this.AdapterSettingsPage.USBTargetConsoleType == null)
			{
				base.GoPage(PageType.AdaptersSettings);
				return;
			}
			base.GoPage(PageType.SelectAuthGamepad);
		}

		private bool isMonitoring = true;
	}
}
