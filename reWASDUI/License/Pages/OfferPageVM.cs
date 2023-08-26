using System;
using System.Windows.Input;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Clases;
using XBEliteWPF.Utils;

namespace reWASDUI.License.Pages
{
	internal class OfferPageVM : BaseLicensePage
	{
		public OfferPageVM(LicenseInfoModel licenseInfo)
			: base(licenseInfo)
		{
		}

		public string HtmlToDisplay
		{
			get
			{
				if (this._licenseInfo.UseSavedOffer)
				{
					return this._licenseInfo.SavedOffer.OfferText;
				}
				if (!string.IsNullOrEmpty(this._licenseInfo.OfferHtml))
				{
					return this._licenseInfo.OfferHtml;
				}
				return "";
			}
		}

		public string BuyNowText
		{
			get
			{
				if (this._licenseInfo.UseSavedOffer)
				{
					return this._licenseInfo.SavedOffer.OfferButtonText;
				}
				if (!string.IsNullOrEmpty(this._licenseInfo.BuyButtonText))
				{
					return this._licenseInfo.BuyButtonText;
				}
				return DTLocalization.GetString(8322);
			}
		}

		public new bool IsAdditionalLinkExist
		{
			get
			{
				if (this._licenseInfo.UseSavedOffer)
				{
					return !string.IsNullOrEmpty(this._licenseInfo.SavedOffer.AdditionalURLText);
				}
				return !string.IsNullOrEmpty(this._licenseInfo.AdditionalLinkText);
			}
		}

		public new string AdditionalLinkUrl
		{
			get
			{
				if (this._licenseInfo.UseSavedOffer)
				{
					return this._licenseInfo.SavedOffer.AdditionalURL;
				}
				return this._licenseInfo.AdditionalLinkUrl;
			}
		}

		public new string AdditionalLinkText
		{
			get
			{
				if (this._licenseInfo.UseSavedOffer)
				{
					return this._licenseInfo.SavedOffer.AdditionalURLText;
				}
				return this._licenseInfo.AdditionalLinkText;
			}
		}

		public ICommand CustomBuyCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._customBuyCommand) == null)
				{
					relayCommand = (this._customBuyCommand = new RelayCommand(new Action(this.OnCustomBuyCommand)));
				}
				return relayCommand;
			}
		}

		public void OnCustomBuyCommand()
		{
			string text;
			if (this._licenseInfo.UseSavedOffer)
			{
				text = this._licenseInfo.SavedOffer.OfferLink;
			}
			else
			{
				text = this._licenseInfo.BuyButtonUrl;
			}
			if (string.IsNullOrEmpty(text))
			{
				DSUtils.GoUrl("https://www.daemon-tools.cc/cart/buy_check?abbr=rewasd&utm_campaign=rewasd2&utm_source=app&utm_medium=license");
			}
			else
			{
				DSUtils.GoUrl(text);
			}
			this._licenseInfo.UseSavedOffer = false;
			SenderGoogleAnalytics.SendMessageEvent("License", "Buy", "offer", -1L, false);
			base.OnShowWizardPageAction(9);
		}

		private RelayCommand _customBuyCommand;
	}
}
