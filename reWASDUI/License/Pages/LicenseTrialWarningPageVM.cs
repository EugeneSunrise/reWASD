using System;

namespace reWASDUI.License.Pages
{
	internal class LicenseTrialWarningPageVM : BaseLicensePage
	{
		public LicenseTrialWarningPageVM(LicenseInfoModel licenseInfo)
			: base(licenseInfo)
		{
		}

		public string LikeProduct
		{
			get
			{
				return "";
			}
		}

		public override string GetBuyUrl()
		{
			return string.Format("https://www.daemon-tools.cc/cart/buy_check?abbr={0}&coupon_code=30offultra&system_key={1}", "rewasd", this._licenseInfo.HardwareId);
		}
	}
}
