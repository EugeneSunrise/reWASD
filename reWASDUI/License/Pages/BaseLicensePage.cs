using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using XBEliteWPF.License.Licensing.Enums;
using XBEliteWPF.Utils;

namespace reWASDUI.License.Pages
{
	internal class BaseLicensePage : ZBindableBase
	{
		public bool IsChecking
		{
			get
			{
				return this._isChecking;
			}
			set
			{
				this.SetProperty<bool>(ref this._isChecking, value, "IsChecking");
				Mouse.OverrideCursor = (this._isChecking ? System.Windows.Input.Cursors.Wait : System.Windows.Input.Cursors.Arrow);
			}
		}

		public string UserId
		{
			get
			{
				return this._userId;
			}
			set
			{
				this.SetProperty<string>(ref this._userId, value, "UserId");
			}
		}

		public string Header
		{
			get
			{
				return this._header;
			}
			set
			{
				if (value == this._header)
				{
					return;
				}
				this._header = value;
				this.OnPropertyChanged("Header");
			}
		}

		public bool IsPaidLicense
		{
			get
			{
				return this._isPaidLicense;
			}
			set
			{
				if (value == this._isPaidLicense)
				{
					return;
				}
				this._isPaidLicense = value;
				this.OnPropertyChanged("IsPaidLicense");
			}
		}

		public bool IsAdditionalLinkExist
		{
			get
			{
				return !string.IsNullOrEmpty(this._licenseInfo.AdditionalLinkText);
			}
		}

		public string AdditionalLinkUrl
		{
			get
			{
				return this._licenseInfo.AdditionalLinkUrl;
			}
		}

		public string AdditionalLinkText
		{
			get
			{
				return this._licenseInfo.AdditionalLinkText;
			}
		}

		public string Serial
		{
			get
			{
				return this._licenseInfo.Serial;
			}
		}

		public string HardwareId
		{
			get
			{
				return this._licenseInfo.HardwareId;
			}
		}

		public string BuyUrl
		{
			get
			{
				return this.GetBuyUrl();
			}
		}

		public string SupportUrl
		{
			get
			{
				return this.GetSupportUrl();
			}
		}

		public bool UseSavedOffer
		{
			get
			{
				return this._licenseInfo.UseSavedOffer;
			}
		}

		public bool IsTrialExpired
		{
			get
			{
				return this._licenseInfo.CheckLicenseResult == 5;
			}
		}

		public ICommand ActivateCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._activateCommand) == null)
				{
					relayCommand = (this._activateCommand = new RelayCommand(new Action(this.OnActivateCommand)));
				}
				return relayCommand;
			}
		}

		public ICommand BuyCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._buyCommand) == null)
				{
					relayCommand = (this._buyCommand = new RelayCommand(new Action(this.OnBuyAndActivateCommand)));
				}
				return relayCommand;
			}
		}

		public ICommand BuyNowCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._buyNowCommand) == null)
				{
					relayCommand = (this._buyNowCommand = new RelayCommand(new Action(this.OnBuyCommand)));
				}
				return relayCommand;
			}
		}

		public ICommand CloseCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._closeCommand) == null)
				{
					relayCommand = (this._closeCommand = new RelayCommand(new Action(this.OnCloseCommand)));
				}
				return relayCommand;
			}
		}

		public ICommand BackCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._backCommand) == null)
				{
					relayCommand = (this._backCommand = new RelayCommand(new Action(this.OnBackCommand)));
				}
				return relayCommand;
			}
		}

		public ICommand CheckActivationCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._checkActivationCommand) == null)
				{
					relayCommand = (this._checkActivationCommand = new RelayCommand(new Action(this.OnCheckActivationCommand), () => !this.IsChecking));
				}
				return relayCommand;
			}
		}

		public event BaseLicensePage.ShowWizardPageAction showWizardPageAction;

		public BaseLicensePage(LicenseInfoModel licenseInfo)
		{
			if (licenseInfo != null)
			{
				this._licenseInfo = licenseInfo;
				this.RefreshLicenseInfo();
				this.IsPaidLicense = this._licenseInfo.LicenseType == 3;
			}
		}

		public virtual void OnShowPage()
		{
			this.RefreshLicenseInfo();
		}

		protected virtual void RefreshLicenseInfo()
		{
			if (this._licenseInfo == null)
			{
				return;
			}
			if (this._licenseInfo.LicenseType == 3)
			{
				this.Header = DTLocalization.GetString(4458);
				this.IsPaidLicense = true;
				return;
			}
			this.IsPaidLicense = false;
			this.Header = DTLocalization.GetString(4459);
		}

		protected void OnCheckActivationCommand()
		{
			BaseLicensePage.<OnCheckActivationCommand>d__60 <OnCheckActivationCommand>d__;
			<OnCheckActivationCommand>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<OnCheckActivationCommand>d__.<>4__this = this;
			<OnCheckActivationCommand>d__.<>1__state = -1;
			<OnCheckActivationCommand>d__.<>t__builder.Start<BaseLicensePage.<OnCheckActivationCommand>d__60>(ref <OnCheckActivationCommand>d__);
		}

		public void OnActivateCommand()
		{
			this.showWizardPageAction(9);
		}

		private void OnBuyAndActivateCommand()
		{
			this.OnBuyCommand();
			this.OnActivateCommand();
		}

		public virtual string GetBuyUrl()
		{
			return "https://www.daemon-tools.cc/cart/buy_check?abbr=rewasd&utm_campaign=rewasd2&utm_source=app&utm_medium=license";
		}

		public string GetSupportUrl()
		{
			return "https://www.daemon-tools.cc/contacts/producttechnicalsupport";
		}

		private void OnBuyCommand()
		{
			DSUtils.GoUrl("https://www.daemon-tools.cc/cart/buy_check?abbr=rewasd&utm_campaign=rewasd2&utm_source=app&utm_medium=license");
			SenderGoogleAnalytics.SendMessageEvent("License", "Buy", "license", -1L, false);
		}

		private void OnCloseCommand()
		{
			this.showWizardPageAction(13);
		}

		private void OnBackCommand()
		{
			this.showWizardPageAction(14);
		}

		public Task OnActivateWithFacebook(string email)
		{
			BaseLicensePage.<OnActivateWithFacebook>d__68 <OnActivateWithFacebook>d__;
			<OnActivateWithFacebook>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<OnActivateWithFacebook>d__.<>4__this = this;
			<OnActivateWithFacebook>d__.email = email;
			<OnActivateWithFacebook>d__.<>1__state = -1;
			<OnActivateWithFacebook>d__.<>t__builder.Start<BaseLicensePage.<OnActivateWithFacebook>d__68>(ref <OnActivateWithFacebook>d__);
			return <OnActivateWithFacebook>d__.<>t__builder.Task;
		}

		protected void OnShowWizardPageAction(LicenseWizardActions wizardActions)
		{
			this.showWizardPageAction(wizardActions);
		}

		public bool CheckLicenseError()
		{
			if (!string.IsNullOrEmpty(this._licenseInfo.OfferHtml))
			{
				this.OnShowWizardPageAction(3);
				return true;
			}
			if (this._licenseInfo.CheckLicenseResult == 15)
			{
				this.OnShowWizardPageAction(7);
				return true;
			}
			if (this._licenseInfo.CheckLicenseResult == 5)
			{
				this.OnShowWizardPageAction(5);
				return true;
			}
			if (this._licenseInfo.CheckLicenseResult == 8)
			{
				this.OnShowWizardPageAction(7);
				return true;
			}
			if (this._licenseInfo.CheckLicenseResult == 7)
			{
				this.OnShowWizardPageAction(1);
				return true;
			}
			if (this._licenseInfo.CheckLicenseResult == 14)
			{
				return false;
			}
			if (this._licenseInfo.CheckLicenseResult == 6)
			{
				if (!this._licenseInfo.VerifiedLicense)
				{
					this.OnShowWizardPageAction(7);
					return true;
				}
				DTMessageBox.Show(DTLocalization.GetString(4234), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
				return false;
			}
			else
			{
				if (!string.IsNullOrEmpty(this._licenseInfo.Message))
				{
					DTMessageBox.Show(this._licenseInfo.Message, MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
				}
				if (this._licenseInfo.CheckLicenseResult == 10 || this._licenseInfo.CheckLicenseResult == 2 || this._licenseInfo.CheckLicenseResult == 29)
				{
					this.OnShowWizardPageAction(9);
					return true;
				}
				if (this._licenseInfo.CheckLicenseResult == 31)
				{
					if (DTMessageBox.Show(null, DTLocalization.GetString(12735), "", MessageBoxButton.OKCancel, MessageBoxImage.Hand, DTLocalization.GetString(12743), false, 0.0, MessageBoxResult.Cancel, DTLocalization.GetString(5003), null, null) == MessageBoxResult.OK && App.AdminOperations.RestartServices() == 0)
					{
						new Process
						{
							StartInfo = 
							{
								WorkingDirectory = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath),
								FileName = "Rewasd.exe"
							}
						}.Start();
						Environment.Exit(0);
					}
					return false;
				}
				return false;
			}
		}

		protected LicenseInfoModel _licenseInfo;

		private string _header;

		private bool _isPaidLicense;

		private string _userId;

		private bool _isChecking;

		private RelayCommand _activateCommand;

		private RelayCommand _buyCommand;

		private RelayCommand _buyNowCommand;

		private RelayCommand _closeCommand;

		private RelayCommand _backCommand;

		private RelayCommand _checkActivationCommand;

		public delegate void ShowWizardPageAction(LicenseWizardActions action);
	}
}
