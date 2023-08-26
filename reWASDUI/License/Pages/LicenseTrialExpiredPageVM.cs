using System;
using DiscSoft.NET.Common.Localization;

namespace reWASDUI.License.Pages
{
	internal class LicenseTrialExpiredPageVM : BaseLicensePage
	{
		public LicenseTrialExpiredPageVM(LicenseInfoModel licenseInfo)
			: base(licenseInfo)
		{
		}

		public string EnjoyedTrial
		{
			get
			{
				return string.Format(DTLocalization.GetString(9157), "reWASD");
			}
		}

		public string LinkName
		{
			get
			{
				return "Visit the official store to get the full version";
			}
		}

		public bool IsLongTrialExpired
		{
			get
			{
				return this._licenseInfo.TrialDaysLeft < -3;
			}
		}
	}
}
