using System;
using DiscSoft.NET.Common.Localization;

namespace reWASDUI.License.Pages
{
	internal class LicenseInfoPageVM : BaseLicensePage
	{
		public LicenseInfoPageVM(LicenseInfoModel licenseInfo)
			: base(licenseInfo)
		{
			this.RefreshLicenseInfo();
		}

		protected override void RefreshLicenseInfo()
		{
			base.RefreshLicenseInfo();
			if (this._licenseInfo.LicenseType != 3)
			{
				DateTime dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
				this.ExpiresDate = dateTime.AddDays((double)this._licenseInfo.TrialDaysLeft).ToString("d");
				if (this._licenseInfo.TrialDaysLeft == 0)
				{
					this.ExpiresCaption = DTLocalization.GetString(8975);
					return;
				}
				this.ExpiresCaption = DTLocalization.GetString(8974);
			}
		}

		public string ExpiresCaption
		{
			get
			{
				return this._expiresCaption;
			}
			set
			{
				if (value == this._expiresCaption)
				{
					return;
				}
				this._expiresCaption = value;
				this.OnPropertyChanged("ExpiresCaption");
			}
		}

		public string ExpiresDate
		{
			get
			{
				return this._expiresDate;
			}
			set
			{
				if (value == this._expiresDate)
				{
					return;
				}
				this._expiresDate = value;
				this.OnPropertyChanged("ExpiresDate");
			}
		}

		private string _expiresCaption;

		private string _expiresDate;
	}
}
