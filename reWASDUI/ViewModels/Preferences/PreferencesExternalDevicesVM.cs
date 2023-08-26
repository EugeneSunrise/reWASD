using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Attributes;
using DiscSoft.NET.Common.View.SecondaryWindows;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using GIMXEngine;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Ioc;
using reWASDCommon;
using reWASDCommon.Utils;
using reWASDUI.Infrastructure;
using reWASDUI.ViewModels.Preferences.Base;
using reWASDUI.Views.SecondaryWindows;
using reWASDUI.Views.SecondaryWindows.AddExternalDeviceWizard;
using XBEliteWPF.Infrastructure.ExternalDevices;
using XBEliteWPF.Utils;

namespace reWASDUI.ViewModels.Preferences
{
	public class PreferencesExternalDevicesVM : PreferencesBaseVM
	{
		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public ExternalDevicesCollection ExternalDevicesCollection
		{
			get
			{
				return this._externalDevicesCollection;
			}
			set
			{
				this.SetProperty<ExternalDevicesCollection>(ref this._externalDevicesCollection, value, "ExternalDevicesCollection");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public ExternalDevice SelectedExternalDevice
		{
			get
			{
				return this._selectedExternalDevice;
			}
			set
			{
				if (this.SetProperty<ExternalDevice>(ref this._selectedExternalDevice, value, "SelectedExternalDevice"))
				{
					this.RefreshCommands();
				}
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public ObservableCollection<ExternalClient> ExternalClientsCollection
		{
			get
			{
				return this._externalClientsCollection;
			}
			set
			{
				this.SetProperty<ObservableCollection<ExternalClient>>(ref this._externalClientsCollection, value, "ExternalClientsCollection");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public ExternalClient SelectedExternalClient
		{
			get
			{
				return this._selectedExternalClient;
			}
			set
			{
				if (this.SetProperty<ExternalClient>(ref this._selectedExternalClient, value, "SelectedExternalClient"))
				{
					this.RefreshCommands();
				}
			}
		}

		public bool ExternalDeviceOverwritePrevConfig
		{
			get
			{
				return this._externalDeviceOverwritePrevConfig;
			}
			set
			{
				if (this._externalDeviceOverwritePrevConfig != value)
				{
					this._externalDeviceOverwritePrevConfig = value;
					this.OnPropertyChanged("ExternalDeviceOverwritePrevConfig");
				}
			}
		}

		public PreferencesExternalDevicesVM()
		{
			App.EventAggregator.GetEvent<ExternalHelperChanged>().Subscribe(delegate(object o)
			{
				this.ExternalDevicesCollection = null;
				this.ExternalDevicesCollection = App.GamepadService.ExternalDevices;
			});
		}

		public override Task Initialize()
		{
			this.ExternalDevicesCollection = App.GamepadService.ExternalDevices;
			this.ExternalClientsCollection = App.GamepadService.ExternalClients;
			this.ExternalDeviceOverwritePrevConfig = base.UserSettingsService.ExternalDeviceOverwritePrevConfig;
			this.SetDescription();
			return Task.CompletedTask;
		}

		public override void Refresh()
		{
			this.ExternalDeviceOverwritePrevConfig = base.UserSettingsService.ExternalDeviceOverwritePrevConfig;
		}

		private async void SaveExternalDevices()
		{
			await App.GamepadService.BinDataSerialize.SaveExternalDevices();
		}

		private async void SaveExternalClients()
		{
			await App.GamepadService.BinDataSerialize.SaveExternalClients();
		}

		public override Task<bool> ApplyChanges()
		{
			this.SaveExternalDevices();
			this.SaveExternalClients();
			base.UserSettingsService.ExternalDeviceOverwritePrevConfig = this.ExternalDeviceOverwritePrevConfig;
			return Task.FromResult<bool>(true);
		}

		private void RefreshCommands()
		{
			this.RemoveExternalDeviceCommand.RaiseCanExecuteChanged();
			this.RefreshExternalDeviceCommand.RaiseCanExecuteChanged();
			this.RollUpExternalDeviceCommand.RaiseCanExecuteChanged();
			this.RollDownExternalDeviceCommand.RaiseCanExecuteChanged();
			this.EditExternalDeviceCommand.RaiseCanExecuteChanged();
			this.SetupExternalDeviceCommand.RaiseCanExecuteChanged();
			this.RemoveExternalClientCommand.RaiseCanExecuteChanged();
			this.RollUpExternalClientCommand.RaiseCanExecuteChanged();
			this.RollDownExternalClientCommand.RaiseCanExecuteChanged();
			this.EditExternalClientCommand.RaiseCanExecuteChanged();
			this.CopyExternalClientCommand.RaiseCanExecuteChanged();
		}

		public DelegateCommand<ExternalDevice> SetupExternalDeviceCommand
		{
			get
			{
				DelegateCommand<ExternalDevice> delegateCommand;
				if ((delegateCommand = this._setupExternalDevice) == null)
				{
					delegateCommand = (this._setupExternalDevice = new DelegateCommand<ExternalDevice>(new Action<ExternalDevice>(this.SetupExternalDevice), (ExternalDevice device) => device != null && device.DeviceType == 2));
				}
				return delegateCommand;
			}
		}

		private void SetupExternalDevice(ExternalDevice device)
		{
			ESP32Preferences esp32Preferences = new ESP32Preferences(device);
			esp32Preferences.ShowDialog();
			if (esp32Preferences.WindowResult == MessageBoxResult.OK)
			{
				device.BaudRate = esp32Preferences.SelectedBaudRate.baudRate;
				this.SaveExternalDevices();
				App.GamepadService.ExternalDeviceRelationsHelper.ChangeExternalDeviceRelationsBaudRate(device.ExternalDeviceId, device.BaudRate);
			}
		}

		public DelegateCommand<ExternalDevice> RefreshExternalDeviceCommand
		{
			get
			{
				DelegateCommand<ExternalDevice> delegateCommand;
				if ((delegateCommand = this._refreshExternalDevice) == null)
				{
					delegateCommand = (this._refreshExternalDevice = new DelegateCommand<ExternalDevice>(new Action<ExternalDevice>(this.RefreshExternalDevice), delegate(ExternalDevice device)
					{
						ExternalDevicesCollection externalDevicesCollection = this.ExternalDevicesCollection;
						return externalDevicesCollection == null || externalDevicesCollection.Count != 0;
					}));
				}
				return delegateCommand;
			}
		}

		private void RefreshExternalDevice(ExternalDevice device)
		{
			this.ExternalDevicesCollection = null;
			this.ExternalDevicesCollection = App.GamepadService.ExternalDevices;
			this.SelectedExternalDevice = device;
		}

		public DelegateCommand<ExternalDevice> RemoveExternalDeviceCommand
		{
			get
			{
				DelegateCommand<ExternalDevice> delegateCommand;
				if ((delegateCommand = this._removeExternalDevice) == null)
				{
					delegateCommand = (this._removeExternalDevice = new DelegateCommand<ExternalDevice>(new Action<ExternalDevice>(this.RemoveExternalDevice), (ExternalDevice device) => device != null));
				}
				return delegateCommand;
			}
		}

		private async void RemoveExternalDevice(ExternalDevice device)
		{
			if (device != null)
			{
				bool eraseFirmware = false;
				if ((device.DeviceType == 2 || device.DeviceType == 3) && device.IsOnlineAndCorrect)
				{
					if (MessageBoxWithCheckBox.Show(App.m_MainWnd, DTLocalization.GetString(12056), MessageBoxButton.YesNo, MessageBoxImage.Question, ref eraseFirmware, MessageBoxResult.Yes, false, 0.0, null, null, null, null, null, DTLocalization.GetString(12668)) != MessageBoxResult.Yes)
					{
						return;
					}
				}
				else if (DTMessageBox.Show(DTLocalization.GetString(12056), MessageBoxButton.YesNo, MessageBoxImage.Question, null, false, MessageBoxResult.None) != MessageBoxResult.Yes)
				{
					return;
				}
				TaskAwaiter<bool> taskAwaiter2;
				if (device.DeviceType == null)
				{
					TaskAwaiter<bool> taskAwaiter = this.CleanBluetoothSettings().GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<bool>);
					}
					if (!taskAwaiter.GetResult())
					{
						return;
					}
				}
				await App.GamepadService.ExternalDeviceRelationsHelper.RemoveExternalDeviceRelations(device.ExternalDeviceId);
				if (device.DeviceType == 2 || device.DeviceType == 3)
				{
					if (device.DeviceType == 2)
					{
						bool flag = !string.IsNullOrEmpty(device.HardwareDongleBluetoothMacAddress);
						if (flag)
						{
							TaskAwaiter<bool> taskAwaiter = App.AdminOperations.DeleteESP32DeviceRegKeys(device.HardwareDongleBluetoothMacAddress.Replace(":", "-").ToLower()).GetAwaiter();
							if (!taskAwaiter.IsCompleted)
							{
								await taskAwaiter;
								taskAwaiter = taskAwaiter2;
								taskAwaiter2 = default(TaskAwaiter<bool>);
							}
							flag = !taskAwaiter.GetResult();
						}
						if (flag)
						{
							return;
						}
						device.RefreshComPorts();
						if (device.IsSerialPortFTDI && device.DefaultLatencySpeed != 0 && device.DefaultLatencySpeed != device.LatencySpeed)
						{
							await App.AdminOperations.ChangeESP32DeviceLatency(device.SerialPort, device.DefaultLatencySpeed);
						}
					}
					if (device.IsOnlineAndCorrect && eraseFirmware)
					{
						TaskAwaiter<FirmwareLoader.FirwareUpdateResult> taskAwaiter3 = Wizard.ClearESP32(device).GetAwaiter();
						if (!taskAwaiter3.IsCompleted)
						{
							await taskAwaiter3;
							TaskAwaiter<FirmwareLoader.FirwareUpdateResult> taskAwaiter4;
							taskAwaiter3 = taskAwaiter4;
							taskAwaiter4 = default(TaskAwaiter<FirmwareLoader.FirwareUpdateResult>);
						}
						if (taskAwaiter3.GetResult() != null)
						{
							return;
						}
					}
				}
				this.ExternalDevicesCollection.Remove(device);
				this.SaveExternalDevices();
				this.OnPropertyChanged("ExternalDevicesCollection");
				this.RefreshCommands();
				App.GamepadService.ExternalDeviceRelationsHelper.Refresh();
			}
		}

		public DelegateCommand<ExternalDevice> RollUpExternalDeviceCommand
		{
			get
			{
				DelegateCommand<ExternalDevice> delegateCommand;
				if ((delegateCommand = this._rollUpExternalDevice) == null)
				{
					delegateCommand = (this._rollUpExternalDevice = new DelegateCommand<ExternalDevice>(new Action<ExternalDevice>(this.RollUpExternalDevice), new Func<ExternalDevice, bool>(this.CanExecuteRollUpExternalDevice)));
				}
				return delegateCommand;
			}
		}

		private bool CanExecuteRollUpExternalDevice(ExternalDevice device)
		{
			return device != null && this.ExternalDevicesCollection.IndexOf(device) > 0;
		}

		private void RollUpExternalDevice(ExternalDevice device)
		{
			if (device == null)
			{
				return;
			}
			int num = this.ExternalDevicesCollection.IndexOf(device);
			this.ExternalDevicesCollection.Move(num, num - 1);
			this.SaveExternalDevices();
			this.RefreshCommands();
		}

		public DelegateCommand<ExternalDevice> RollDownExternalDeviceCommand
		{
			get
			{
				DelegateCommand<ExternalDevice> delegateCommand;
				if ((delegateCommand = this._rollDownExternalDevice) == null)
				{
					delegateCommand = (this._rollDownExternalDevice = new DelegateCommand<ExternalDevice>(new Action<ExternalDevice>(this.RollDownExternalDevice), new Func<ExternalDevice, bool>(this.CanExecuteRollDownExternalDevice)));
				}
				return delegateCommand;
			}
		}

		private bool CanExecuteRollDownExternalDevice(ExternalDevice device)
		{
			return device != null && this.ExternalDevicesCollection.IndexOf(device) + 1 < this.ExternalDevicesCollection.Count;
		}

		private void RollDownExternalDevice(ExternalDevice device)
		{
			if (device == null)
			{
				return;
			}
			int num = this.ExternalDevicesCollection.IndexOf(device);
			this.ExternalDevicesCollection.Move(num, num + 1);
			this.SaveExternalDevices();
			this.RefreshCommands();
		}

		public DelegateCommand<ExternalDevice> EditExternalDeviceCommand
		{
			get
			{
				DelegateCommand<ExternalDevice> delegateCommand;
				if ((delegateCommand = this._editExternalDevice) == null)
				{
					delegateCommand = (this._editExternalDevice = new DelegateCommand<ExternalDevice>(new Action<ExternalDevice>(this.EditExternalDevice), (ExternalDevice device) => device != null));
				}
				return delegateCommand;
			}
		}

		private void EditExternalDevice(ExternalDevice device)
		{
			if (device == null)
			{
				return;
			}
			this._renamableDeviceName = device.Alias;
			this._renamableDeviceId = device.ExternalDeviceId;
			device.IsEditing = true;
		}

		public bool IsExistESP32TargetRegValues(string bluetoothMacAddress)
		{
			RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
			RegistryKey registryKey2 = ((registryKey != null) ? registryKey.OpenSubKey("SOFTWARE\\Disc Soft\\reWASD\\Bluetooth\\Keys\\", false) : null);
			string[] array = ((registryKey2 != null) ? registryKey2.GetSubKeyNames() : null);
			if (registryKey2 != null && array != null)
			{
				foreach (string text in array)
				{
					try
					{
						RegistryKey registryKey3 = registryKey2.OpenSubKey(text, false);
						if (((registryKey3 != null) ? registryKey3.GetValue(bluetoothMacAddress) : null) != null)
						{
							return true;
						}
					}
					catch (Exception)
					{
					}
				}
			}
			return false;
		}

		public DelegateCommand<ExternalClient> RemoveExternalClientCommand
		{
			get
			{
				DelegateCommand<ExternalClient> delegateCommand;
				if ((delegateCommand = this._removeExternalClient) == null)
				{
					delegateCommand = (this._removeExternalClient = new DelegateCommand<ExternalClient>(new Action<ExternalClient>(this.RemoveExternalClient), (ExternalClient client) => client != null));
				}
				return delegateCommand;
			}
		}

		private async void RemoveExternalClient(ExternalClient Client)
		{
			if (Client != null)
			{
				if (DTMessageBox.Show(DTLocalization.GetString(12056), MessageBoxButton.YesNo, MessageBoxImage.Question, null, false, MessageBoxResult.None) == MessageBoxResult.Yes)
				{
					string text = UtilsCommon.MacAddressToString(Client.MacAddress, "-").ToLower();
					bool flag = this.IsExistESP32TargetRegValues(text);
					if (flag)
					{
						TaskAwaiter<bool> taskAwaiter = App.AdminOperations.DeleteESP32TargetRegValues(text).GetAwaiter();
						if (!taskAwaiter.IsCompleted)
						{
							await taskAwaiter;
							TaskAwaiter<bool> taskAwaiter2;
							taskAwaiter = taskAwaiter2;
							taskAwaiter2 = default(TaskAwaiter<bool>);
						}
						flag = !taskAwaiter.GetResult();
					}
					if (!flag)
					{
						await App.HttpClientService.Gamepad.DisableRemapByExternalClient(Client.MacAddress);
						await App.HttpClientService.Engine.UnpairController(Client.MacAddress);
						this.ExternalClientsCollection.Remove(Client);
						this.SaveExternalClients();
						this.OnPropertyChanged("ExternalClientsCollection");
						await App.GamepadService.ExternalDeviceRelationsHelper.RemoveExternalClientRelations(Client.MacAddress);
						App.GamepadService.ExternalDeviceRelationsHelper.Refresh();
						this.RefreshCommands();
					}
				}
			}
		}

		public DelegateCommand<ExternalClient> EditExternalClientCommand
		{
			get
			{
				DelegateCommand<ExternalClient> delegateCommand;
				if ((delegateCommand = this._editExternalClient) == null)
				{
					delegateCommand = (this._editExternalClient = new DelegateCommand<ExternalClient>(new Action<ExternalClient>(this.EditExternalClient), (ExternalClient client) => client != null));
				}
				return delegateCommand;
			}
		}

		private void EditExternalClient(ExternalClient Client)
		{
			if (Client == null)
			{
				return;
			}
			this._renamableClientName = Client.Alias;
			this._renamableClient = Client;
			Client.IsEditing = true;
		}

		public DelegateCommand<ExternalClient> CopyExternalClientCommand
		{
			get
			{
				DelegateCommand<ExternalClient> delegateCommand;
				if ((delegateCommand = this._copyExternalClient) == null)
				{
					delegateCommand = (this._copyExternalClient = new DelegateCommand<ExternalClient>(new Action<ExternalClient>(this.CopyExternalClient), (ExternalClient client) => client != null));
				}
				return delegateCommand;
			}
		}

		private void CopyExternalClient(ExternalClient Client)
		{
			if (Client == null)
			{
				return;
			}
			string text = Client.Alias + " " + Client.MacAddressText;
			try
			{
				Clipboard.SetText(text);
			}
			catch (Exception ex)
			{
				Tracer.TraceException(ex, "CopyExternalClient");
			}
		}

		public DelegateCommand<ExternalClient> RollUpExternalClientCommand
		{
			get
			{
				DelegateCommand<ExternalClient> delegateCommand;
				if ((delegateCommand = this._rollUpExternalClient) == null)
				{
					delegateCommand = (this._rollUpExternalClient = new DelegateCommand<ExternalClient>(new Action<ExternalClient>(this.RollUpExternalClient), new Func<ExternalClient, bool>(this.CanExecuteRollUpExternalClient)));
				}
				return delegateCommand;
			}
		}

		private bool CanExecuteRollUpExternalClient(ExternalClient Client)
		{
			return Client != null && this.ExternalClientsCollection.IndexOf(Client) > 0;
		}

		private void RollUpExternalClient(ExternalClient Client)
		{
			if (Client == null)
			{
				return;
			}
			int num = this.ExternalClientsCollection.IndexOf(Client);
			this.ExternalClientsCollection.Move(num, num - 1);
			this.SaveExternalClients();
			this.RefreshCommands();
		}

		public DelegateCommand<ExternalClient> RollDownExternalClientCommand
		{
			get
			{
				DelegateCommand<ExternalClient> delegateCommand;
				if ((delegateCommand = this._rollDownExternalClient) == null)
				{
					delegateCommand = (this._rollDownExternalClient = new DelegateCommand<ExternalClient>(new Action<ExternalClient>(this.RollDownExternalClient), new Func<ExternalClient, bool>(this.CanExecuteRollDownExternalClient)));
				}
				return delegateCommand;
			}
		}

		private bool CanExecuteRollDownExternalClient(ExternalClient Client)
		{
			return Client != null && this.ExternalClientsCollection.IndexOf(Client) + 1 < this.ExternalClientsCollection.Count;
		}

		private void RollDownExternalClient(ExternalClient Client)
		{
			if (Client == null)
			{
				return;
			}
			int num = this.ExternalClientsCollection.IndexOf(Client);
			this.ExternalClientsCollection.Move(num, num + 1);
			this.SaveExternalClients();
			this.RefreshCommands();
		}

		private void SetDescription()
		{
			base.Description = new Localizable(12033);
		}

		private async Task<bool> CleanBluetoothSettings()
		{
			AdminOperationsDecider adminOperations = (AdminOperationsDecider)IContainerProviderExtensions.Resolve<IAdminOperations>(App.Container);
			if (adminOperations != null)
			{
				TaskAwaiter<int> taskAwaiter = adminOperations.RemoveBluetoothSettingsForRewasd().GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<int> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<int>);
				}
				bool flag = taskAwaiter.GetResult() == 1;
				if (!adminOperations.IsServiceLaunched)
				{
					return false;
				}
				if (flag)
				{
					DTMessageBox.Show(DTLocalization.GetString(12036), MessageBoxButton.OK, MessageBoxImage.Exclamation, null, false, MessageBoxResult.None);
				}
			}
			return true;
		}

		public void OnClientEditModeChanged(string newClientName)
		{
			this.ValidateNewClientName(newClientName);
			this.SaveExternalClients();
		}

		private void ValidateNewClientName(string newClientName)
		{
			PreferencesExternalDevicesVM.<>c__DisplayClass83_0 CS$<>8__locals1 = new PreferencesExternalDevicesVM.<>c__DisplayClass83_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.newClientName = newClientName;
			if (CS$<>8__locals1.newClientName == this._renamableClientName)
			{
				return;
			}
			ExternalClient externalClient;
			if (this._renamableClient != null)
			{
				externalClient = this.ExternalClientsCollection.FirstOrDefault((ExternalClient x) => x == CS$<>8__locals1.<>4__this._renamableClient);
			}
			else
			{
				externalClient = this.ExternalClientsCollection.FirstOrDefault((ExternalClient x) => x.Alias == CS$<>8__locals1.newClientName);
			}
			if (externalClient == null)
			{
				return;
			}
			if (string.IsNullOrWhiteSpace(CS$<>8__locals1.newClientName))
			{
				DTMessageBox.Show(string.Format(DTLocalization.GetString(11026), CS$<>8__locals1.newClientName), DTLocalization.GetString(5010), MessageBoxButton.OK, MessageBoxImage.Exclamation);
				externalClient.Alias = this._renamableClientName;
				return;
			}
			if (this.ExternalClientsCollection.Count((ExternalClient x) => x.Alias == CS$<>8__locals1.newClientName) > 1)
			{
				int num = this.FormatDeviceName(ref CS$<>8__locals1.newClientName) + 1;
				PreferencesExternalDevicesVM.<>c__DisplayClass83_0 CS$<>8__locals2 = CS$<>8__locals1;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
				defaultInterpolatedStringHandler.AppendFormatted(CS$<>8__locals1.newClientName);
				defaultInterpolatedStringHandler.AppendLiteral(" (");
				defaultInterpolatedStringHandler.AppendFormatted<int>(num);
				defaultInterpolatedStringHandler.AppendLiteral(")");
				CS$<>8__locals2.newClientName = defaultInterpolatedStringHandler.ToStringAndClear();
				externalClient.Alias = CS$<>8__locals1.newClientName;
			}
		}

		public void OnDeviceEditModeChanged(string newDeviceName)
		{
			this.ValidateNewDeviceName(newDeviceName);
			this.SaveExternalDevices();
		}

		private void ValidateNewDeviceName(string newDeviceName)
		{
			PreferencesExternalDevicesVM.<>c__DisplayClass85_0 CS$<>8__locals1 = new PreferencesExternalDevicesVM.<>c__DisplayClass85_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.newDeviceName = newDeviceName;
			if (CS$<>8__locals1.newDeviceName == this._renamableDeviceName)
			{
				return;
			}
			ExternalDevice externalDevice;
			if (!string.IsNullOrEmpty(this._renamableDeviceId))
			{
				externalDevice = this.ExternalDevicesCollection.FirstOrDefault((ExternalDevice x) => x.ExternalDeviceId == CS$<>8__locals1.<>4__this._renamableDeviceId);
			}
			else
			{
				externalDevice = this.ExternalDevicesCollection.FirstOrDefault((ExternalDevice x) => x.Alias == CS$<>8__locals1.newDeviceName);
			}
			if (externalDevice == null)
			{
				return;
			}
			if (this.ExternalDevicesCollection.Count((ExternalDevice x) => x.Alias == CS$<>8__locals1.newDeviceName) > 1)
			{
				int num = this.FormatDeviceName(ref CS$<>8__locals1.newDeviceName) + 1;
				PreferencesExternalDevicesVM.<>c__DisplayClass85_0 CS$<>8__locals2 = CS$<>8__locals1;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
				defaultInterpolatedStringHandler.AppendFormatted(CS$<>8__locals1.newDeviceName);
				defaultInterpolatedStringHandler.AppendLiteral(" (");
				defaultInterpolatedStringHandler.AppendFormatted<int>(num);
				defaultInterpolatedStringHandler.AppendLiteral(")");
				CS$<>8__locals2.newDeviceName = defaultInterpolatedStringHandler.ToStringAndClear();
				if (DTMessageBox.Show(string.Format(DTLocalization.GetString(12073), CS$<>8__locals1.newDeviceName), DTLocalization.GetString(5012) ?? "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					externalDevice.Alias = CS$<>8__locals1.newDeviceName;
					return;
				}
				externalDevice.Alias = ((!string.IsNullOrEmpty(this._renamableDeviceName)) ? this._renamableDeviceName : externalDevice.DeviceType.ToString());
			}
			if (string.IsNullOrWhiteSpace(CS$<>8__locals1.newDeviceName))
			{
				DTMessageBox.Show(string.Format(DTLocalization.GetString(11026), CS$<>8__locals1.newDeviceName), DTLocalization.GetString(5010), MessageBoxButton.OK, MessageBoxImage.Exclamation);
				externalDevice.Alias = this._renamableDeviceName;
			}
		}

		private int FormatDeviceName(ref string name)
		{
			int num = 0;
			int num2 = 0;
			try
			{
				if (name[name.Length - 1] == ')' && name[name.Length - 3] == '(')
				{
					num = name.Length - 3;
					num2 = Convert.ToInt32(name[name.Length - 2].ToString());
				}
				if (num != 0)
				{
					name = name.Substring(0, num - 1);
				}
			}
			catch (Exception)
			{
			}
			return num2;
		}

		private ExternalDevicesCollection _externalDevicesCollection;

		private ExternalDevice _selectedExternalDevice;

		private ObservableCollection<ExternalClient> _externalClientsCollection = new ObservableCollection<ExternalClient>();

		private ExternalClient _selectedExternalClient;

		private bool _externalDeviceOverwritePrevConfig = true;

		private string _renamableDeviceName;

		private string _renamableDeviceId;

		private DelegateCommand<ExternalDevice> _setupExternalDevice;

		private DelegateCommand<ExternalDevice> _refreshExternalDevice;

		private DelegateCommand<ExternalDevice> _removeExternalDevice;

		private DelegateCommand<ExternalDevice> _rollUpExternalDevice;

		private DelegateCommand<ExternalDevice> _rollDownExternalDevice;

		private DelegateCommand<ExternalDevice> _editExternalDevice;

		private DelegateCommand<ExternalClient> _removeExternalClient;

		private string _renamableClientName;

		private ExternalClient _renamableClient;

		private DelegateCommand<ExternalClient> _editExternalClient;

		private DelegateCommand<ExternalClient> _copyExternalClient;

		private DelegateCommand<ExternalClient> _rollUpExternalClient;

		private DelegateCommand<ExternalClient> _rollDownExternalClient;
	}
}
