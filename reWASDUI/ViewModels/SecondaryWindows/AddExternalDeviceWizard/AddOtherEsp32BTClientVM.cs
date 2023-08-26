using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using Prism.Commands;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.DataModels;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.KeyBindings;
using XBEliteWPF.Infrastructure.ExternalDevices;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class AddOtherEsp32BTClientVM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.AddOtherEsp32BTClient;
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
				if (value == this._alias)
				{
					return;
				}
				this._alias = value;
				this.OnPropertyChanged("Alias");
			}
		}

		public AddOtherEsp32BTClientVM(WizardVM wizard)
			: base(wizard)
		{
		}

		public override void OnShowPage()
		{
			this.Alias = "";
			this.StartDiscovery();
		}

		public override async void OnHidePage()
		{
			if (this._wizard.Result.DeviceType == 2)
			{
				await this.StopDiscavery();
			}
			if (this.PoolerThread != null && this.PoolerThread.IsAlive)
			{
				this.PoolerThread.Interrupt();
			}
		}

		protected override void NavigatePreviousPage()
		{
			this.isInProgress = false;
			base.GoPage(PageType.AddExternalClient);
		}

		protected override bool CanOk()
		{
			return this._macAddress > 0UL;
		}

		public Drawing ControllerImage
		{
			get
			{
				GameVM currentGame = App.GameProfilesService.CurrentGame;
				VirtualGamepadType? virtualGamepadType;
				if (currentGame == null)
				{
					virtualGamepadType = null;
				}
				else
				{
					ConfigVM currentConfig = currentGame.CurrentConfig;
					if (currentConfig == null)
					{
						virtualGamepadType = null;
					}
					else
					{
						ConfigData configData = currentConfig.ConfigData;
						virtualGamepadType = ((configData != null) ? new VirtualGamepadType?(configData.VirtualGamepadType) : null);
					}
				}
				VirtualGamepadType? virtualGamepadType2 = virtualGamepadType;
				string text;
				if (virtualGamepadType2 != null)
				{
					VirtualGamepadType valueOrDefault = virtualGamepadType2.GetValueOrDefault();
					if (valueOrDefault <= 1)
					{
						text = "BTXboxWirelessController";
						goto IL_7B;
					}
					if (valueOrDefault == 4)
					{
						text = "BTProController";
						goto IL_7B;
					}
				}
				text = "BTWirelessController";
				IL_7B:
				Application application = Application.Current;
				object obj;
				if (application == null)
				{
					obj = null;
				}
				else
				{
					Window mainWindow = application.MainWindow;
					obj = ((mainWindow != null) ? mainWindow.TryFindResource(text) : null);
				}
				return obj as Drawing;
			}
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

		protected new async void OnCancel()
		{
			this.isInProgress = false;
			await this.StopDiscavery();
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

		public async Task Connecting()
		{
			this.isInProgress = true;
			ExternalDevice currentExternalDevice = (this._wizard.FindPage(PageType.AdaptersSettings) as AdaptersSettingsVM).CurrentExternalDevice;
			ExternalClient externalClient = new ExternalClient
			{
				Alias = this._wizard.Result.Alias,
				MacAddress = this._macAddress
			};
			await App.GamepadService.ExternalDeviceRelationsHelper.AddAndSaveRelation(currentExternalDevice, externalClient, null);
			await Task.Delay(500);
			TaskAwaiter<bool> taskAwaiter = App.GameProfilesService.ApplyCurrentProfile(false).GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<bool> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			if (!taskAwaiter.GetResult())
			{
				this.NavigatePreviousPage();
			}
			else
			{
				for (int i = 0; i < 3; i++)
				{
					await Task.Delay(4000);
					if (!this.isInProgress)
					{
						return;
					}
					if ((App.GamepadService.ExternalDeviceRelationsHelper.CurrentExternalState == 6 || App.GamepadService.ExternalDeviceRelationsHelper.CurrentExternalState == 3) && App.GamepadService.CurrentGamepad != null)
					{
						await App.HttpClientService.ExternalDevices.ExternalDeviceBluetoothReconnect(App.GamepadService.CurrentGamepad.ID, App.GamepadService.CurrentGamepad.CurrentSlot);
					}
					if (App.GamepadService.ExternalDeviceRelationsHelper.CurrentExternalState == 9)
					{
						break;
					}
				}
				await this.SaveExternalClients();
				this.isInProgress = false;
				base.GoPage(PageType.AddOtherEsp32BTClientFinish);
			}
		}

		private async Task SaveExternalClients()
		{
			ExternalClient externalClient = new ExternalClient
			{
				Alias = this._wizard.Result.Alias,
				MacAddress = this._macAddress
			};
			await this._wizard.SaveExternalClient(externalClient);
		}

		private async void OnHoneypotPairingRejected(ulong macAddress)
		{
			this._macAddress = macAddress;
			this.Alias = this._wizard.Result.Alias;
			await this.StopDiscavery();
			await this.Connecting();
		}

		private async void StartDiscovery()
		{
			this.isInProgress = true;
			App.EventAggregator.GetEvent<HoneypotPairingRejected>().Subscribe(new Action<ulong>(this.OnHoneypotPairingRejected));
			await App.HttpClientService.Gamepad.ApplyHoneypotProfile(this._wizard.Result.DeviceType, App.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.VirtualGamepadType, this._wizard.Result.SerialPort, this._wizard.Result.BaudRate);
		}

		private async Task StopDiscavery()
		{
			App.EventAggregator.GetEvent<HoneypotPairingRejected>().Unsubscribe(new Action<ulong>(this.OnHoneypotPairingRejected));
			await App.HttpClientService.Gamepad.DeleteSpecialProfiles();
		}

		private bool isInProgress;

		private ulong _macAddress;

		private string _alias;

		private DelegateCommand _cancelCommand;

		private Thread _poolerThread;
	}
}
