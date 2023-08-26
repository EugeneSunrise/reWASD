using System;
using System.Collections.ObjectModel;
using System.Linq;
using DiscSoft.NET.Common.AdminRightsFeatures;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Utils;
using reWASDUI.DataModels;
using reWASDUI.Infrastructure.KeyBindings;
using XBEliteWPF.Infrastructure.ExternalDevices;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class DeviceTypeVM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.DeviceType;
			}
		}

		private VirtualGamepadType? CurrentVirtualGamepadType
		{
			get
			{
				GameVM currentGame = App.GameProfilesService.CurrentGame;
				if (currentGame == null)
				{
					return null;
				}
				ConfigVM currentConfig = currentGame.CurrentConfig;
				if (currentConfig == null)
				{
					return null;
				}
				ConfigData configData = currentConfig.ConfigData;
				if (configData == null)
				{
					return null;
				}
				return new VirtualGamepadType?(configData.VirtualGamepadType);
			}
		}

		public bool IsBluetoothCouldBeAdded
		{
			get
			{
				if (!App.GamepadService.ExternalDevices.IsBluetoothExist && !BluetoothUtils.IsRebootRequired() && !BluetoothUtils.IsBluetoothAdapterIsNotSupported() && UACHelper.IsWindows10OrHigher(-1))
				{
					VirtualGamepadType? currentVirtualGamepadType = this.CurrentVirtualGamepadType;
					VirtualGamepadType virtualGamepadType = 3;
					return !((currentVirtualGamepadType.GetValueOrDefault() == virtualGamepadType) & (currentVirtualGamepadType != null));
				}
				return false;
			}
		}

		public string BluetoothDisabledToolTip
		{
			get
			{
				if (!UACHelper.IsWindows10OrHigher(-1))
				{
					return DTLocalization.GetString(12097);
				}
				if (BluetoothUtils.IsRebootRequired())
				{
					return DTLocalization.GetString(12009);
				}
				if (BluetoothUtils.IsBluetoothAdapterIsNotSupported())
				{
					return DTLocalization.GetString(11999);
				}
				VirtualGamepadType? currentVirtualGamepadType = this.CurrentVirtualGamepadType;
				VirtualGamepadType virtualGamepadType = 3;
				if ((currentVirtualGamepadType.GetValueOrDefault() == virtualGamepadType) & (currentVirtualGamepadType != null))
				{
					return DTLocalization.GetString(12032);
				}
				return DTLocalization.GetString(12092);
			}
		}

		public ExternalDeviceType DeviceType
		{
			get
			{
				return this._externalDeviceType;
			}
			set
			{
				if (this._externalDeviceType != value)
				{
					this._externalDeviceType = value;
					this.OnPropertyChanged("DeviceType");
				}
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
					this.OnPropertyChanged("IsAliasValid");
				}
			}
		}

		public bool IsAliasValid
		{
			get
			{
				return this.Alias.Length != 0 && !string.IsNullOrWhiteSpace(this.Alias);
			}
		}

		public bool IsBluetooth
		{
			get
			{
				return this.DeviceType == 0;
			}
			set
			{
				if (value != this.IsBluetooth)
				{
					this.DeviceType = (value ? 0 : 1);
					this.UpdateProperties();
				}
			}
		}

		public bool IsGIMX
		{
			get
			{
				return this.DeviceType == 1;
			}
			set
			{
				if (value != this.IsGIMX)
				{
					this.DeviceType = (value ? 1 : 0);
					this.UpdateProperties();
				}
			}
		}

		public bool IsESP32
		{
			get
			{
				return this.DeviceType == 2;
			}
			set
			{
				if (value != this.IsESP32)
				{
					this.DeviceType = (value ? 2 : 0);
					this.UpdateProperties();
				}
			}
		}

		public bool IsESP32S2
		{
			get
			{
				return this.DeviceType == 3;
			}
			set
			{
				if (value != this.IsESP32S2)
				{
					this.DeviceType = (value ? 3 : 0);
					this.UpdateProperties();
				}
			}
		}

		private void UpdateProperties()
		{
			this.ChangeAlias();
			this.OnPropertyChanged("IsBluetooth");
			this.OnPropertyChanged("IsGIMX");
			this.OnPropertyChanged("IsESP32");
			this.OnPropertyChanged("IsESP32S2");
		}

		public DeviceTypeVM(WizardVM wizard)
			: base(wizard)
		{
			this.ExternalDeviceCollection = new ObservableCollection<ExternalDevice>(App.GamepadService.ExternalDevices);
			this.ChangeAlias();
		}

		private void ChangeAlias()
		{
			switch (this._externalDeviceType)
			{
			case 0:
				this.Alias = DTLocalization.GetString(11986);
				return;
			case 1:
				this.Alias = "GIMX";
				return;
			case 2:
				this.Alias = DTLocalization.GetString(12673);
				return;
			case 3:
				this.Alias = DTLocalization.GetString(12707);
				return;
			default:
				return;
			}
		}

		public override void OnShowPage()
		{
			if (!this.IsBluetoothCouldBeAdded)
			{
				this.IsGIMX = true;
			}
		}

		protected override void NavigatePreviousPage()
		{
			base.GoPage(PageType.AdaptersSettings);
		}

		protected override void NavigateToNextPage()
		{
			if (!this.IsAliasValid)
			{
				return;
			}
			this._wizard.Result.DeviceType = this.DeviceType;
			this._wizard.Result.Alias = this.Alias;
			if (this.IsBluetooth)
			{
				base.GoPage(PageType.BluetoothSettings);
			}
			if (this.IsGIMX)
			{
				base.GoPage(PageType.GimxStage1);
			}
			if (this.IsESP32 || this.IsESP32S2)
			{
				base.GoPage(PageType.EspStage1);
			}
		}

		public ObservableCollection<ExternalDevice> ExternalDeviceCollection
		{
			get
			{
				ObservableCollection<ExternalDevice> externalDeviceCollection = this._externalDeviceCollection;
				if (externalDeviceCollection != null && externalDeviceCollection.Count > 1)
				{
					for (int i = 1; i < this._externalDeviceCollection.Count; i++)
					{
						if (this._externalDeviceCollection[i].SortIndex < this._externalDeviceCollection[i - 1].SortIndex)
						{
							this._externalDeviceCollection = new ObservableCollection<ExternalDevice>(this._externalDeviceCollection.OrderBy((ExternalDevice x) => x.SortIndex));
							break;
						}
					}
				}
				return this._externalDeviceCollection;
			}
			set
			{
				if (this._externalDeviceCollection == value)
				{
					return;
				}
				this.SetProperty(ref this._externalDeviceCollection, value, "ExternalDeviceCollection");
			}
		}

		private ExternalDeviceType _externalDeviceType;

		private string _alias;

		private ObservableCollection<ExternalDevice> _externalDeviceCollection;
	}
}
