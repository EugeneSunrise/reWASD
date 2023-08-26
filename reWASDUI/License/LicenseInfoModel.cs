using System;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Utils;
using XBEliteWPF.License.Licensing.ComStructures;
using XBEliteWPF.License.Licensing.Enums;

namespace reWASDUI.License
{
	public class LicenseInfoModel
	{
		public async Task GetLicenseInfo(Ref<LicenseCheckResult> licenseResult)
		{
			this._trialDaysLeft = licenseResult.Value.TrialDaysLeft;
			this._updateURL = licenseResult.Value.UpdateURL;
			this._updateNotes = licenseResult.Value.UpdateNotes;
			this._message = licenseResult.Value.Message;
			this._offerHtml = licenseResult.Value.OfferText;
			this._additionalLinkUrl = licenseResult.Value.AdditionalURL;
			this._additionalLinkText = licenseResult.Value.AdditionalURLText;
			this._paidSerial = licenseResult.Value.PaidSerial;
			this._paidMajorVersion = licenseResult.Value.PaidMajorVersion;
			this._isTryUpdateVersion = !string.IsNullOrEmpty(this._paidSerial) && this._paidMajorVersion < 6;
			this._buyButtonUrl = licenseResult.Value.OfferLink;
			this._buyButtonText = licenseResult.Value.OfferButtonText;
			this._checkLicenseResult = licenseResult.Value.Result;
			LicenseInfo licenseInfo = await App.HttpClientService.LicenseApi.GetLicenseInfo();
			this._hwId = licenseInfo.HardwareId;
			this._serial = licenseInfo.Serial;
			this._licenseType = licenseInfo.License;
			this._verifiedLicense = licenseInfo.VerifiedLicense == 1;
		}

		public bool IsCheckActivationClean()
		{
			return this.LicenseType != null && this.CheckLicenseResult == null;
		}

		public async Task<bool> IsSuccessLicense()
		{
			bool flag;
			if (this.LicenseType == null)
			{
				flag = false;
			}
			else
			{
				if (this.CheckLicenseResult == 6 || this.CheckLicenseResult == 10)
				{
					LicenseCheckResult licenseCheckResult = await App.HttpClientService.LicenseApi.CheckLicense(false);
					await this.GetLicenseInfo(licenseCheckResult);
					if (this.IsCheckActivationClean())
					{
						return true;
					}
				}
				if (this.CheckLicenseResult == null || this.CheckLicenseResult == 14 || this.CheckLicenseResult == 16 || this.CheckLicenseResult == 7)
				{
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
			return flag;
		}

		public int TrialDaysLeft
		{
			get
			{
				return this._trialDaysLeft;
			}
		}

		public int PaidMajorVersion
		{
			get
			{
				return this._paidMajorVersion;
			}
		}

		public string ExistingPaidSerial
		{
			get
			{
				return this._paidSerial;
			}
		}

		public string GetExistingSerial()
		{
			if (this._serial == "")
			{
				return this._paidSerial;
			}
			return this._serial;
		}

		public string HardwareId
		{
			get
			{
				return this._hwId;
			}
		}

		public string Serial
		{
			get
			{
				return this._serial;
			}
		}

		public LicenseType LicenseType
		{
			get
			{
				return this._licenseType;
			}
		}

		public bool VerifiedLicense
		{
			get
			{
				return this._verifiedLicense;
			}
		}

		public string UpdateURL
		{
			get
			{
				return this._updateURL;
			}
		}

		public string UpdateNotes
		{
			get
			{
				return this._updateNotes;
			}
		}

		public string OfferHtml
		{
			get
			{
				return this._offerHtml;
			}
		}

		public LicenseCheckingResultEnum CheckLicenseResult
		{
			get
			{
				return this._checkLicenseResult;
			}
		}

		public string Message
		{
			get
			{
				return this._message;
			}
		}

		public string AdditionalLinkUrl
		{
			get
			{
				return this._additionalLinkUrl;
			}
		}

		public string AdditionalLinkText
		{
			get
			{
				return this._additionalLinkText;
			}
		}

		public string BuyButtonUrl
		{
			get
			{
				return this._buyButtonUrl;
			}
		}

		public string BuyButtonText
		{
			get
			{
				return this._buyButtonText;
			}
		}

		public bool IsTryUpdateVersion
		{
			get
			{
				return this._isTryUpdateVersion;
			}
		}

		public bool UseSavedOffer { get; set; }

		public HtmlOffer SavedOffer { get; set; }

		private int _trialDaysLeft;

		private int _paidMajorVersion;

		private string _hwId;

		private string _paidSerial;

		private string _serial;

		private LicenseType _licenseType;

		private bool _verifiedLicense;

		private string _updateURL;

		private string _updateNotes;

		private string _offerHtml;

		private string _message;

		private string _additionalLinkUrl;

		private string _additionalLinkText;

		private string _buyButtonUrl;

		private string _buyButtonText;

		private bool _isTryUpdateVersion;

		private LicenseCheckingResultEnum _checkLicenseResult;
	}
}
