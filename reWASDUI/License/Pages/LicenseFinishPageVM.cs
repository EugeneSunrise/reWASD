using System;

namespace reWASDUI.License.Pages
{
	internal class LicenseFinishPageVM : BaseLicensePage
	{
		public LicenseFinishPageVM(LicenseInfoModel licenseInfo)
			: base(licenseInfo)
		{
		}

		public string ThankYou
		{
			get
			{
				return "Thank you";
			}
		}

		public string ThankYouDescription
		{
			get
			{
				return "";
			}
		}
	}
}
