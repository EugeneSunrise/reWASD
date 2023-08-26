using System;
using DiscSoft.NET.Common.Localization;
using reWASDCommon.Infrastructure.Enums;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class AddExternalClientVM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.AddExternalClient;
			}
		}

		public bool IsPS4Enabled
		{
			get
			{
				return App.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.VirtualGamepadType == 2;
			}
		}

		public bool IsNintendoSwitchProSelected
		{
			get
			{
				return App.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.VirtualGamepadType == 4;
			}
		}

		public bool IsNintendoSwitchEnabled
		{
			get
			{
				return this.IsNintendoSwitchProSelected && ((this._wizard.Result.DeviceType == null && this.IsBluetoothAdapterIsSupportedForNintendoConsole) || this._wizard.Result.DeviceType == 2);
			}
		}

		public AddExternalClientVM(WizardVM wizard)
			: base(wizard)
		{
			if (!this.IsPS4Enabled)
			{
				this.ClientType = (this.IsNintendoSwitchProSelected ? 4 : 0);
			}
			this.ChangeAlias();
			this.CheckNintendoSwitchSupport();
		}

		public string NintendoSwitchDisabledToolTip
		{
			get
			{
				if (this.IsNintendoSwitchProSelected && !this.IsBluetoothAdapterIsSupportedForNintendoConsole)
				{
					return DTLocalization.GetString(12032);
				}
				return DTLocalization.GetString(12451);
			}
		}

		private async void CheckNintendoSwitchSupport()
		{
			bool flag = await App.HttpClientService.Engine.IsBluetoothAdapterIsSupportedForNintendoConsole();
			this.IsBluetoothAdapterIsSupportedForNintendoConsole = flag;
			if (!this.IsBluetoothAdapterIsSupportedForNintendoConsole && this.ClientType == 4)
			{
				this.ClientType = 0;
				this.ChangeAlias();
			}
		}

		protected override async void NavigateToNextPage()
		{
			this._wizard.Result.Alias = this.Alias;
			if (this._wizard.Result.DeviceType == 2)
			{
				await App.HttpClientService.ExternalDevices.ExternalDeviceDisableRemapForSerialPort(this._wizard.Result.SerialPort);
			}
			if (this.ClientType == 1)
			{
				base.GoPage(PageType.AddPS4ConsoleStep1);
			}
			else if (this.ClientType == 4)
			{
				base.GoPage(PageType.AddNintendoSwitchConsoleStep1);
			}
			else if (this._wizard.Result.DeviceType == 2)
			{
				base.GoPage(PageType.AddOtherEsp32BTClient);
			}
			else
			{
				base.GoPage(PageType.AddOtherBTClient);
			}
		}

		public string Alias
		{
			get
			{
				return this._alias;
			}
			set
			{
				if (this._alias != value)
				{
					this._alias = value;
					this.OnPropertyChanged("Alias");
				}
			}
		}

		private void ChangeAlias()
		{
			switch (this.ClientType)
			{
			case 1:
				this.Alias = "PS4 console";
				return;
			case 2:
				this.Alias = "PS5 console";
				return;
			case 3:
				this.Alias = "Xbox console";
				return;
			case 4:
				this.Alias = "Switch console";
				return;
			default:
				this.Alias = "Other device";
				return;
			}
		}

		public ConsoleType ClientType
		{
			get
			{
				return this._clientType;
			}
			set
			{
				if (this._clientType != value)
				{
					this._clientType = value;
					this.OnPropertyChanged("ClientType");
				}
			}
		}

		public bool IsBluetoothAdapterIsSupportedForNintendoConsole
		{
			get
			{
				return this._isBluetoothAdapterIsSupportedForNintendoConsole;
			}
			set
			{
				if (this._isBluetoothAdapterIsSupportedForNintendoConsole != value)
				{
					this._isBluetoothAdapterIsSupportedForNintendoConsole = value;
					this.OnPropertyChanged("IsBluetoothAdapterIsSupportedForNintendoConsole");
				}
			}
		}

		private void UpdateProperties()
		{
			this.OnPropertyChanged("IsPs4");
			this.OnPropertyChanged("IsNintendoSwitch");
			this.OnPropertyChanged("IsCustom");
			this.OnPropertyChanged("IsAliasVisible");
		}

		public bool IsAliasVisible
		{
			get
			{
				return !this.IsCustom || this._wizard.Result.DeviceType == 2;
			}
		}

		public bool IsPs4
		{
			get
			{
				return this.ClientType == 1;
			}
			set
			{
				if (value)
				{
					this.ClientType = 1;
					this.ChangeAlias();
					this.UpdateProperties();
				}
			}
		}

		public bool IsNintendoSwitch
		{
			get
			{
				return this.ClientType == 4;
			}
			set
			{
				if (value)
				{
					this.ClientType = 4;
					this.ChangeAlias();
					this.UpdateProperties();
				}
			}
		}

		public bool IsCustom
		{
			get
			{
				return !this.IsPs4 && !this.IsNintendoSwitch;
			}
			set
			{
				if (value)
				{
					this.ClientType = 0;
					this.ChangeAlias();
					this.UpdateProperties();
				}
			}
		}

		private string _alias;

		private ConsoleType _clientType = 1;

		private bool _isBluetoothAdapterIsSupportedForNintendoConsole;
	}
}
