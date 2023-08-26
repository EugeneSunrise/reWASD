using System;
using System.Windows.Input;
using DiscSoft.NET.Common.Utils.Clases;
using XBEliteWPF.License.Licensing.ComStructures;

namespace reWASDUI.License.Pages
{
	internal class LicenseWelcomeNotActivatedPageVM : BaseLicensePage
	{
		public LicenseWelcomeNotActivatedPageVM(LicenseInfoModel licenseInfo)
			: base(licenseInfo)
		{
		}

		private async void OnActivateTrialCommand()
		{
			if (this._licenseInfo.LicenseType == null)
			{
				ActivationLicenseInfo activationLicenseInfo = default(ActivationLicenseInfo);
				activationLicenseInfo.licenseType = 2;
				LicenseCheckResult licenseCheckResult = await App.LicensingService.Activate(activationLicenseInfo);
				await this._licenseInfo.GetLicenseInfo(licenseCheckResult);
				if (this._licenseInfo.IsCheckActivationClean())
				{
					if (!string.IsNullOrEmpty(this._licenseInfo.OfferHtml))
					{
						base.OnShowWizardPageAction(3);
					}
					else
					{
						base.OnShowWizardPageAction(13);
					}
				}
				else
				{
					base.CheckLicenseError();
				}
			}
		}

		public ICommand ActivateTrialCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._ActivateTrialCommand) == null)
				{
					relayCommand = (this._ActivateTrialCommand = new RelayCommand(new Action(this.OnActivateTrialCommand)));
				}
				return relayCommand;
			}
		}

		public bool IsTryAgainInVisible
		{
			get
			{
				return this._licenseInfo.CheckLicenseResult == 7 && this._licenseInfo.LicenseType == 3;
			}
		}

		private RelayCommand _ActivateTrialCommand;
	}
}
