using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using reWASDUI.Infrastructure;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.ExternalDevices;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class AddNintendoSwitchConsoleStep2VM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.AddNintendoSwitchConsoleStep2;
			}
		}

		public ExternalClient ExternalClient { get; set; }

		public AddNintendoSwitchConsoleStep2VM(WizardVM wizard)
			: base(wizard)
		{
		}

		public override void OnShowPage()
		{
			this.StartDiscovery();
		}

		protected override async void NavigatePreviousPage()
		{
			base.IsProcessing = false;
			await this.StopDiscavery();
			base.GoPage(PageType.AddNintendoSwitchConsoleStep1);
		}

		protected override bool CanNavigatePreviousPage()
		{
			return true;
		}

		protected override bool CanCancel()
		{
			return true;
		}

		protected override async void OnCancel()
		{
			base.IsProcessing = false;
			await this.StopDiscavery();
			base.OnCancel();
		}

		private async Task StopDiscavery()
		{
			await App.HttpClientService.Gamepad.DeleteSpecialProfiles();
		}

		private async Task SaveExternalClients(ulong macAddress)
		{
			this.ExternalClient = new ExternalClient
			{
				Alias = this._wizard.Result.Alias,
				MacAddress = macAddress,
				ConsoleType = 4
			};
			if (App.GamepadService.ExternalClients.All((ExternalClient x) => x.MacAddress != this.ExternalClient.MacAddress) && this.ExternalClient.MacAddress != 0UL)
			{
				App.GamepadService.ExternalClients.Add(this.ExternalClient);
				await App.GamepadService.BinDataSerialize.SaveExternalClients();
			}
			await this.StopDiscavery();
			base.IsProcessing = false;
			base.GoPage(PageType.AddNintendoSwitchConsoleStep2b);
		}

		private async void StartDiscovery()
		{
			AddNintendoSwitchConsoleStep2VM.<>c__DisplayClass14_0 CS$<>8__locals1 = new AddNintendoSwitchConsoleStep2VM.<>c__DisplayClass14_0();
			base.IsProcessing = true;
			CS$<>8__locals1.devices = null;
			if (this._wizard.Result.DeviceType == null)
			{
				List<BluetoothDeviceInfo> list = await App.HttpClientService.Engine.GetServiceBluetoothDeviceInfo(0U);
				CS$<>8__locals1.devices = list;
			}
			if (this._wizard.Result.DeviceType == 2)
			{
				App.EventAggregator.GetEvent<HoneypotPairingRejected>().Subscribe(new Action<ulong>(this.OnHoneypotPairingRejected));
			}
			try
			{
				TaskAwaiter<bool> taskAwaiter = App.HttpClientService.Gamepad.ApplyHoneypotProfile(this._wizard.Result.DeviceType, 4, this._wizard.Result.SerialPort, this._wizard.Result.BaudRate).GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<bool> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				if (!taskAwaiter.GetResult())
				{
					base.IsProcessing = false;
					base.GoPage(PageType.AddNintendoSwitchConsoleStep2b);
					return;
				}
				if (!base.IsProcessing)
				{
					await this.StopDiscavery();
					return;
				}
				if (this._wizard.Result.DeviceType == null)
				{
					BluetoothDeviceInfo bluetoothDeviceInfo;
					for (int i = 0; i < 30; i++)
					{
						await Task.Delay(1000);
						IEnumerable<BluetoothDeviceInfo> enumerable = await App.HttpClientService.Engine.GetServiceBluetoothDeviceInfo(1U);
						Func<BluetoothDeviceInfo, bool> func;
						if ((func = CS$<>8__locals1.<>9__1) == null)
						{
							AddNintendoSwitchConsoleStep2VM.<>c__DisplayClass14_0 CS$<>8__locals2 = CS$<>8__locals1;
							Func<BluetoothDeviceInfo, bool> func2 = (BluetoothDeviceInfo item) => !CS$<>8__locals1.devices.Any((BluetoothDeviceInfo devItem) => devItem.Address == item.Address);
							CS$<>8__locals2.<>9__1 = func2;
							func = func2;
						}
						bluetoothDeviceInfo = enumerable.Where(func).FirstOrDefault((BluetoothDeviceInfo item) => item.Name.Contains("Nintendo Switch"));
						if (bluetoothDeviceInfo != null && bluetoothDeviceInfo.Address != 0UL)
						{
							await this.SaveExternalClients(bluetoothDeviceInfo.Address);
							return;
						}
						if (!base.IsProcessing)
						{
							return;
						}
					}
					bluetoothDeviceInfo = CS$<>8__locals1.devices.FirstOrDefault((BluetoothDeviceInfo item) => item.Name.Contains("Nintendo Switch"));
					if (bluetoothDeviceInfo != null && bluetoothDeviceInfo.Address != 0UL)
					{
						await this.SaveExternalClients(bluetoothDeviceInfo.Address);
						return;
					}
				}
				if (this._wizard.Result.DeviceType == 2)
				{
					for (int i = 0; i < 30; i++)
					{
						await Task.Delay(1000);
						if (!base.IsProcessing)
						{
							break;
						}
					}
					if (!base.IsProcessing)
					{
						return;
					}
				}
			}
			finally
			{
				if (this._wizard.Result.DeviceType == 2)
				{
					App.EventAggregator.GetEvent<HoneypotPairingRejected>().Unsubscribe(new Action<ulong>(this.OnHoneypotPairingRejected));
				}
			}
			base.IsProcessing = false;
			await this.StopDiscavery();
			base.GoPage(PageType.AddNintendoSwitchConsoleStep2a);
		}

		private async void OnHoneypotPairingRejected(ulong macAddress)
		{
			await this.SaveExternalClients(macAddress);
		}
	}
}
