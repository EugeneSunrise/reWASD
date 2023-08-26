using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Clases;
using reWASDUI.License.Pages;
using XBEliteWPF.License.Licensing.ComStructures;
using XBEliteWPF.License.Licensing.Enums;

namespace reWASDUI.License
{
	internal class LicenseWizardVM : NotifyPropertyChangedObject
	{
		public event LicenseWizardVM.CloseDelegate OnClose;

		public async Task Init(Ref<LicenseCheckResult> checkingResultInfo)
		{
			this._licenseInfo = new LicenseInfoModel();
			await this._licenseInfo.GetLicenseInfo(checkingResultInfo);
			this.InitWizard();
		}

		public async Task Init(Ref<HtmlOffer> offer, Ref<LicenseCheckResult> checkingResultInfo)
		{
			this._licenseInfo = new LicenseInfoModel();
			await this._licenseInfo.GetLicenseInfo(checkingResultInfo);
			this._licenseInfo.SavedOffer = offer;
			this._licenseInfo.UseSavedOffer = true;
			this.InitWizard();
			if (offer.Value.OfferType == 0)
			{
				this.CurrentPage = this._licenseErrorWithOffer;
			}
			else
			{
				this.CurrentPage = this._updateAvailable;
			}
			await App.HttpClientService.LicenseApi.ClearOffer();
		}

		private void InitWizard()
		{
			this._baseLicenseInfoPage = new LicenseInfoPageVM(this._licenseInfo);
			this._baseLicenseInfoPage.showWizardPageAction += this.ShowWizardPageAction;
			this._enterSerialPage = new LicenseEnterSerialPageVM(this._licenseInfo);
			this._enterSerialPage.showWizardPageAction += this.ShowWizardPageAction;
			this._welcomeNotActivatedPage = new LicenseWelcomeNotActivatedPageVM(this._licenseInfo);
			this._welcomeNotActivatedPage.showWizardPageAction += this.ShowWizardPageAction;
			this._updateAvailable = new UpdateAvailablePageVM(this._licenseInfo);
			this._updateAvailable.showWizardPageAction += this.ShowWizardPageAction;
			this._licensePaidUncheckedPage = new LicensePaidUncheckedPageVM(this._licenseInfo);
			this._licensePaidUncheckedPage.showWizardPageAction += this.ShowWizardPageAction;
			this._licenseErrorWithOffer = new OfferPageVM(this._licenseInfo);
			this._licenseErrorWithOffer.showWizardPageAction += this.ShowWizardPageAction;
			this._licenseTrialExpiredPage = new LicenseTrialExpiredPageVM(this._licenseInfo);
			this._licenseTrialExpiredPage.showWizardPageAction += this.ShowWizardPageAction;
			this._emailPage = new LicenseEnterEmailPageVM(this._licenseInfo);
			this._history = new List<BaseLicensePage>();
		}

		public BaseLicensePage CurrentPage
		{
			get
			{
				return this._currentPage;
			}
			set
			{
				if (value == this._currentPage)
				{
					return;
				}
				this._currentPage = value;
				this._currentPage.OnShowPage();
				this.OnPropertyChanged("CurrentPage");
			}
		}

		public bool ProcessLicenseError()
		{
			BaseLicensePage baseLicensePage = new BaseLicensePage(this._licenseInfo);
			baseLicensePage.showWizardPageAction += this.ShowWizardPageAction;
			return baseLicensePage.CheckLicenseError();
		}

		public void ShowWelcomePage()
		{
			if (this._licenseInfo.LicenseType == 3)
			{
				this.ShowWizardPageAction(10);
				return;
			}
			this.ShowWizardPageAction(8);
		}

		public void ShowWizardPageAction(LicenseWizardActions action)
		{
			switch (action)
			{
			case 0:
				this._history.Add(this.CurrentPage);
				this.CurrentPage = this._baseLicenseInfoPage;
				return;
			case 1:
				this.CurrentPage = this._licensePaidUncheckedPage;
				return;
			case 2:
				this.CurrentPage = this._licenseTrialUncheckedPage;
				return;
			case 3:
				this.CurrentPage = this._licenseErrorWithOffer;
				App.HttpClientService.LicenseApi.ClearOffer();
				return;
			case 4:
				this.CurrentPage = this._licenseTrialWarningPage;
				break;
			case 5:
				this.CurrentPage = this._licenseTrialExpiredPage;
				return;
			case 6:
				this.CurrentPage = this._licenseTryTrialExpiredPage;
				return;
			case 7:
				this._history.Add(this.CurrentPage);
				this.CurrentPage = this._welcomeNotActivatedPage;
				return;
			case 8:
				this._history.Add(this.CurrentPage);
				this.CurrentPage = this._welcomeTrialPage;
				return;
			case 9:
				if (this.CurrentPage != this._enterSerialPage)
				{
					if (this.CurrentPage != null)
					{
						this._history.Add(this.CurrentPage);
					}
					this.CurrentPage = this._enterSerialPage;
					return;
				}
				break;
			case 10:
				this._history.Add(this.CurrentPage);
				this.CurrentPage = this._finishPage;
				return;
			case 11:
				this.CurrentPage = this._updateAvailable;
				App.HttpClientService.LicenseApi.ClearOffer();
				return;
			case 12:
				this.CurrentPage = this._emailPage;
				return;
			case 13:
				this.OnClose();
				return;
			case 14:
				if (this._history.Count > 0)
				{
					this.CurrentPage = this._history[this._history.Count - 1];
					this._history.RemoveAt(this._history.Count - 1);
					return;
				}
				break;
			default:
				return;
			}
		}

		public async Task<bool> IsTerminate()
		{
			TaskAwaiter<bool> taskAwaiter = this._licenseInfo.IsSuccessLicense().GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<bool> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			return !taskAwaiter.GetResult();
		}

		private LicenseInfoModel _licenseInfo;

		private BaseLicensePage _currentPage;

		private LicenseInfoPageVM _baseLicenseInfoPage;

		private LicenseEnterSerialPageVM _enterSerialPage;

		private LicenseTryTrialExpiredPageVM _licenseTryTrialExpiredPage;

		private LicenseTrialExpiredPageVM _licenseTrialExpiredPage;

		private LicenseTrialWarningPageVM _licenseTrialWarningPage;

		private LicenseFinishPageVM _finishPage;

		private LicenseTrialUncheckedPageVM _licenseTrialUncheckedPage;

		private LicensePaidUncheckedPageVM _licensePaidUncheckedPage;

		private LicenseWelcomeTrialPageVM _welcomeTrialPage;

		private LicenseWelcomeNotActivatedPageVM _welcomeNotActivatedPage;

		private UpdateAvailablePageVM _updateAvailable;

		private OfferPageVM _licenseErrorWithOffer;

		private LicenseEnterEmailPageVM _emailPage;

		private List<BaseLicensePage> _history;

		public bool OfferOnly;

		public delegate void CloseDelegate();
	}
}
