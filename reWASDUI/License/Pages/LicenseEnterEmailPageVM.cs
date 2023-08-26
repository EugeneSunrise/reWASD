using System;
using System.Net.Mail;
using System.Windows.Input;
using DiscSoft.NET.Common.Utils.Clases;

namespace reWASDUI.License.Pages
{
	internal class LicenseEnterEmailPageVM : BaseLicensePage
	{
		public LicenseEnterEmailPageVM(LicenseInfoModel licenseInfo)
			: base(licenseInfo)
		{
		}

		public ICommand AssignLicenseCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._assignLicenseCommand) == null)
				{
					relayCommand = (this._assignLicenseCommand = new RelayCommand(new Action(this.OnAssignLicenseCommand), new Func<bool>(this.AssignLicenseCanExecute)));
				}
				return relayCommand;
			}
		}

		public bool CheckEmail(string email)
		{
			bool flag;
			try
			{
				flag = new MailAddress(email).Address == email;
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		private bool AssignLicenseCanExecute()
		{
			return this.Email != null && this.CheckEmail(this.Email);
		}

		private async void OnAssignLicenseCommand()
		{
			await base.OnActivateWithFacebook(this.Email);
		}

		public string Email
		{
			get
			{
				return this._email;
			}
			set
			{
				this.SetProperty<string>(ref this._email, value, "Email");
			}
		}

		private RelayCommand _assignLicenseCommand;

		private string _email;
	}
}
