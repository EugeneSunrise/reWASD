using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils.Clases;

namespace reWASDUI.License.Pages
{
	internal class LicenseEnterSerialPageVM : BaseLicensePage
	{
		public LicenseEnterSerialPageVM(LicenseInfoModel licenseInfo)
			: base(licenseInfo)
		{
			this.SerialNumber = licenseInfo.Serial;
			if (this._licenseInfo.LicenseType == 2)
			{
				this.SerialNumber = licenseInfo.ExistingPaidSerial;
			}
		}

		public ICommand ActivateLicenseCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._activateLicenseCommand) == null)
				{
					relayCommand = (this._activateLicenseCommand = new RelayCommand(new Action(this.OnActivateLicenseCommand), new Func<bool>(this.ActivateLicenseCanExecute)));
				}
				return relayCommand;
			}
		}

		public bool IsAllowedCharForSerialNumber(char c)
		{
			return c != '-' && ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'));
		}

		private bool ActivateLicenseCanExecute()
		{
			return this.SerialNumber != null && this.SerialNumber.Count<char>() == 32 && this.SerialNumber.All(new Func<char, bool>(this.IsAllowedCharForSerialNumber));
		}

		private void OnActivateLicenseCommand()
		{
			LicenseEnterSerialPageVM.<OnActivateLicenseCommand>d__7 <OnActivateLicenseCommand>d__;
			<OnActivateLicenseCommand>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<OnActivateLicenseCommand>d__.<>4__this = this;
			<OnActivateLicenseCommand>d__.<>1__state = -1;
			<OnActivateLicenseCommand>d__.<>t__builder.Start<LicenseEnterSerialPageVM.<OnActivateLicenseCommand>d__7>(ref <OnActivateLicenseCommand>d__);
		}

		protected override void RefreshLicenseInfo()
		{
			base.RefreshLicenseInfo();
			base.Header = DTLocalization.GetString(4458);
		}

		public string SerialNumber
		{
			get
			{
				return this._serial;
			}
			set
			{
				if (value == this._serial)
				{
					return;
				}
				this._serial = value;
				this.OnPropertyChanged("SerialNumber");
				this.OnPropertyChanged("CheckSerialNumber");
			}
		}

		public string CheckSpamDescription
		{
			get
			{
				return string.Format(DTLocalization.GetString(11805), "https://www.daemon-tools.cc/contacts/producttechnicalsupport");
			}
		}

		public string Description
		{
			get
			{
				return string.Format(DTLocalization.GetString(4464), "reWASD", DTLocalization.GetString(8322));
			}
		}

		private string _serial;

		private RelayCommand _activateLicenseCommand;
	}
}
