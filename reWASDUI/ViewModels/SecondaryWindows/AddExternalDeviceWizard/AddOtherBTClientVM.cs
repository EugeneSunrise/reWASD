using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using Prism.Commands;
using reWASDCommon.Utils;
using reWASDUI.Infrastructure;
using XBEliteWPF.Infrastructure.ExternalDevices;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class AddOtherBTClientVM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.AddOtherBTClient;
			}
		}

		public AddOtherBTClientVM(WizardVM wizard)
			: base(wizard)
		{
			this.ExternalClientsCollection = new ObservableCollection<ExternalClient>(App.GamepadService.ExternalClients);
			this.Alias = DTLocalization.GetString(12060);
			this.MacAddressText = "00:00:00:00:00:00";
		}

		public override void OnShowPage()
		{
			this.StartDiscovery();
		}

		public override async void OnHidePage()
		{
			if (this._wizard.Result.DeviceType == 2)
			{
				await App.HttpClientService.Gamepad.DeleteSpecialProfiles();
				App.EventAggregator.GetEvent<HoneypotPairingRejected>().Unsubscribe(new Action<ulong>(this.OnHoneypotPairingRejected));
			}
			if (this.PoolerThread != null && this.PoolerThread.IsAlive)
			{
				this.PoolerThread.Interrupt();
			}
		}

		protected override void NavigatePreviousPage()
		{
			base.GoPage(PageType.AddExternalClient);
		}

		public new DelegateCommand OkCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._okCommand) == null)
				{
					delegateCommand = (this._okCommand = new DelegateCommand(new Action(this.OnOk), new Func<bool>(this.CanOk)));
				}
				return delegateCommand;
			}
		}

		protected new void OnOk()
		{
			if (this.IsSaveEnabled)
			{
				this.SaveExternalClients();
			}
			base.GoPage(PageType.AdaptersSettings);
		}

		protected override bool CanOk()
		{
			return this.MacAddress > 0UL;
		}

		public new DelegateCommand CancelCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._cancelCommand) == null)
				{
					delegateCommand = (this._cancelCommand = new DelegateCommand(new Action(this.OnCancel), new Func<bool>(this.CanCancel)));
				}
				return delegateCommand;
			}
		}

		protected new void OnCancel()
		{
			this._wizard.OnCancel();
		}

		public Thread PoolerThread
		{
			get
			{
				return this._poolerThread;
			}
			set
			{
				this.SetProperty(ref this._poolerThread, value, "PoolerThread");
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
				if (string.IsNullOrWhiteSpace(value))
				{
					return;
				}
				this.SetProperty(ref this._alias, value, "Alias");
			}
		}

		public bool IsSaveEnabled
		{
			get
			{
				return UtilsCommon.IsValidMacAddress(this.MacAddressText);
			}
		}

		public ulong MacAddress
		{
			get
			{
				return UtilsCommon.MacAddressToUlong(this.MacAddressText);
			}
		}

		public string MacAddressText
		{
			get
			{
				return this._macAddressText;
			}
			set
			{
				if (this.SetProperty(ref this._macAddressText, value, "MacAddressText"))
				{
					this.OnPropertyChanged("IsSaveEnabled");
					this.OkCommand.RaiseCanExecuteChanged();
				}
			}
		}

		public ObservableCollection<ExternalClient> ExternalClientsCollection
		{
			get
			{
				return this._externalClientsCollection;
			}
			set
			{
				this.SetProperty(ref this._externalClientsCollection, value, "ExternalClientsCollection");
			}
		}

		public ExternalClient ExternalClient
		{
			get
			{
				return this._externalClient;
			}
			set
			{
				if (this.SetProperty(ref this._externalClient, value, "ExternalClient"))
				{
					this.Alias = this._externalClient.Alias;
					this.MacAddressText = this._externalClient.MacAddressText;
				}
			}
		}

		private void SaveExternalClients()
		{
			this.ExternalClient = new ExternalClient
			{
				Alias = this.Alias,
				MacAddress = this.MacAddress
			};
			if (App.GamepadService.ExternalClients.All((ExternalClient x) => x.MacAddress != this.ExternalClient.MacAddress) && this.ExternalClient.MacAddress != 0UL)
			{
				App.GamepadService.ExternalClients.Add(this.ExternalClient.Clone());
				App.GamepadService.BinDataSerialize.SaveExternalClients();
				this.OnPropertyChanged("ExternalClientsCollection");
			}
		}

		private void OnHoneypotPairingRejected(ulong macAddress)
		{
			ExternalClient client = new ExternalClient
			{
				Alias = DTLocalization.GetString(12060),
				MacAddress = macAddress
			};
			if (this.ExternalClientsCollection.All((ExternalClient x) => x.MacAddress != client.MacAddress))
			{
				this.ExternalClientsCollection.Add(client);
			}
		}

		private async void StartDiscovery()
		{
			this.ExternalClientsCollection.Clear();
			if (this._wizard.Result.DeviceType == 2)
			{
				App.EventAggregator.GetEvent<HoneypotPairingRejected>().Subscribe(new Action<ulong>(this.OnHoneypotPairingRejected));
				await App.HttpClientService.Gamepad.ApplyHoneypotProfile(this._wizard.Result.DeviceType, App.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.VirtualGamepadType, this._wizard.Result.SerialPort, this._wizard.Result.BaudRate);
			}
			else
			{
				this.PoolerThread = new Thread(delegate
				{
					List<BluetoothUtils.BLUETOOTH_DEVICE_INFO> devices = new List<BluetoothUtils.BLUETOOTH_DEVICE_INFO>();
					try
					{
						Func<ExternalClient, bool> <>9__2;
						for (;;)
						{
							devices = BluetoothUtils.BluetoothDiscovery(8);
							ThreadHelper.ExecuteInMainDispatcher(delegate
							{
								foreach (BluetoothUtils.BLUETOOTH_DEVICE_INFO bluetooth_DEVICE_INFO in devices)
								{
									ExternalClient client = new ExternalClient
									{
										Alias = bluetooth_DEVICE_INFO.szName,
										MacAddress = bluetooth_DEVICE_INFO.Address
									};
									if (this.ExternalClientsCollection.All((ExternalClient x) => x.MacAddress != client.MacAddress))
									{
										this.ExternalClientsCollection.Add(client);
									}
									else if (this.ExternalClientsCollection.Where((ExternalClient x) => x.MacAddress == client.MacAddress).Any((ExternalClient x) => x.Alias == null) && !string.IsNullOrEmpty(client.Alias))
									{
										this.ExternalClientsCollection.Remove((ExternalClient x) => x.MacAddress == client.MacAddress);
										this.ExternalClientsCollection.Add(client);
									}
								}
								ObservableCollection<ExternalClient> externalClientsCollection = this.ExternalClientsCollection;
								Func<ExternalClient, bool> func;
								if ((func = <>9__2) == null)
								{
									func = (<>9__2 = (ExternalClient item) => !devices.Any((BluetoothUtils.BLUETOOTH_DEVICE_INFO deviceItem) => deviceItem.Address == item.MacAddress));
								}
								externalClientsCollection.Remove(func);
							}, true);
						}
					}
					catch (Exception)
					{
					}
				})
				{
					IsBackground = true
				};
				this.PoolerThread.Start();
			}
		}

		private DelegateCommand _okCommand;

		private DelegateCommand _cancelCommand;

		private Thread _poolerThread;

		private string _alias;

		private string _macAddressText;

		private ObservableCollection<ExternalClient> _externalClientsCollection;

		private ExternalClient _externalClient;
	}
}
