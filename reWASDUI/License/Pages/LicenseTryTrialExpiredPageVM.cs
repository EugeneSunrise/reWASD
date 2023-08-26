using System;
using System.Windows.Input;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Clases;

namespace reWASDUI.License.Pages
{
	internal class LicenseTryTrialExpiredPageVM : BaseLicensePage
	{
		public LicenseTryTrialExpiredPageVM(LicenseInfoModel licenseInfo)
			: base(licenseInfo)
		{
		}

		public string UpgradeDiscount
		{
			get
			{
				return "";
			}
		}

		public override string GetBuyUrl()
		{
			return string.Format("https://www.daemon-tools.cc/cart/set_upgrade?config[type]=update&config[serial]={0}", this._licenseInfo.GetExistingSerial());
		}

		private void OnDownloadPrevVersionCommand()
		{
			DSUtils.GoUrl("https://www.daemon-tools.cc/account/serials");
		}

		public ICommand DownloadPrevVersionCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._downloadPrevVersionCommand) == null)
				{
					relayCommand = (this._downloadPrevVersionCommand = new RelayCommand(new Action(this.OnDownloadPrevVersionCommand)));
				}
				return relayCommand;
			}
		}

		private RelayCommand _downloadPrevVersionCommand;
	}
}
