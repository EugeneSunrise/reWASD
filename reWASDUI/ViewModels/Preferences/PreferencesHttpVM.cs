using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Attributes;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using Prism.Ioc;
using reWASDCommon;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using reWASDCommon.Utils;
using reWASDUI.Services.HttpClient;
using reWASDUI.Services.Interfaces;
using reWASDUI.ViewModels.Preferences.Base;
using XBEliteWPF.Utils;

namespace reWASDUI.ViewModels.Preferences
{
	public class PreferencesHttpVM : PreferencesBaseVM, IDataErrorInfo
	{
		public ISSEProcessor SSEClient { get; set; }

		public bool IsEnabledOverLocalNetwork
		{
			get
			{
				return this._isEnabledOverLocalNetwork;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isEnabledOverLocalNetwork, value, "IsEnabledOverLocalNetwork"))
				{
					this.OnPropertyChanged("IsEnabledOverLocalNetwork");
					this.SetDescription();
				}
			}
		}

		public bool IsUdpSelected
		{
			get
			{
				return this._isUdpSelected;
			}
			set
			{
				this.SetProperty<bool>(ref this._isUdpSelected, value, "IsUdpSelected");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool IsUdpRunning
		{
			get
			{
				return this._isUdpRunning;
			}
			set
			{
				this.SetProperty<bool>(ref this._isUdpRunning, value, "IsUdpRunning");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool IsUdpServerHasException
		{
			get
			{
				return this._isUdpServerHasException;
			}
			set
			{
				this.SetProperty<bool>(ref this._isUdpServerHasException, value, "IsUdpServerHasException");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool IsUdpReserved
		{
			get
			{
				return this._isUdpReserved;
			}
			set
			{
				this.SetProperty<bool>(ref this._isUdpReserved, value, "IsUdpReserved");
			}
		}

		private async Task UdpServerState()
		{
			GamepadUdpServerState gamepadUdpServerState = await App.HttpClientService.Engine.GetGamepadUdpServerState();
			if (gamepadUdpServerState == null)
			{
				this.IsUdpRunning = false;
			}
			if (gamepadUdpServerState != null)
			{
				this.IsUdpRunning = gamepadUdpServerState.IsUdpRunning;
				this.IsUdpSelected = gamepadUdpServerState.IsUdpEnabledInPreferences;
				this.IsUdpServerHasException = gamepadUdpServerState.IsUdpServerHasException;
				this.IsUdpReserved = gamepadUdpServerState.IsUdpReserved;
			}
			if (this.IsUdpServerHasException)
			{
				this.UdpStateString = DTLocalization.GetString(12328);
				this.IsUdpRunning = false;
			}
			else if (this.IsUdpRunning)
			{
				this.UdpStateString = string.Format(DTLocalization.GetString(12327), this.CurrentUdpInterface.Value.Ip, this.UdpPort);
			}
			else if (this.IsUdpReserved)
			{
				this.UdpStateString = DTLocalization.GetString(12370);
			}
			else
			{
				this.UdpStateString = DTLocalization.GetString(12326);
			}
		}

		public ViewModelItem<PreferencesHttpVM.InterfaceItem> CurrentInterface
		{
			get
			{
				return this._currentInterfaceItem;
			}
			set
			{
				if (this._currentInterfaceItem == value)
				{
					return;
				}
				this._currentInterfaceItem = value;
				this.OnPropertyChanged("CurrentInterface");
			}
		}

		public ObservableCollection<ViewModelItem<PreferencesHttpVM.InterfaceItem>> UdpInterfacesCollection
		{
			get
			{
				return this._UdpInterfacesCollection;
			}
			set
			{
				if (this._UdpInterfacesCollection == value)
				{
					return;
				}
				this._UdpInterfacesCollection = value;
				this.OnPropertyChanged("UdpInterfacesCollection");
			}
		}

		public ObservableCollection<ViewModelItem<PreferencesHttpVM.InterfaceItem>> InterfacesCollection
		{
			get
			{
				return this._InterfacesCollection;
			}
			set
			{
				if (this._InterfacesCollection == value)
				{
					return;
				}
				this._InterfacesCollection = value;
				this.OnPropertyChanged("InterfacesCollection");
			}
		}

		public ViewModelItem<PreferencesHttpVM.InterfaceItem> CurrentUdpInterface
		{
			get
			{
				return this._currentUdpInterfaceItem;
			}
			set
			{
				if (this._currentUdpInterfaceItem == value)
				{
					return;
				}
				this._currentUdpInterfaceItem = value;
				this.OnPropertyChanged("CurrentUdpInterface");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public string UdpStateString
		{
			get
			{
				return this._udpAddressString;
			}
			set
			{
				this.SetProperty<string>(ref this._udpAddressString, value, "UdpStateString");
			}
		}

		public int DefaultUdpPort
		{
			get
			{
				return GamepadUdpServerSettings.DEFAULT_PORTS[0];
			}
		}

		public int DefaultPort
		{
			get
			{
				return HttpServerSettings.DEFAULT_PORTS[0];
			}
		}

		public int DefaultUDPEmulatorPort
		{
			get
			{
				return HttpServerSettings.DEFAULT_EMULATOR_PORT;
			}
		}

		public string PortToolTip
		{
			get
			{
				return DTLocalization.GetString(12103) + " " + this.DefaultPort.ToString();
			}
		}

		public string UDPEmulatorPortToolTip
		{
			get
			{
				return DTLocalization.GetString(12103) + " " + this.DefaultUDPEmulatorPort.ToString();
			}
		}

		public string UdpPortToolTip
		{
			get
			{
				return DTLocalization.GetString(12103) + " " + this.DefaultUdpPort.ToString();
			}
		}

		public string PortString
		{
			get
			{
				return this._portString;
			}
			set
			{
				this.SetProperty<string>(ref this._portString, value, "PortString");
			}
		}

		public int? Port
		{
			get
			{
				int num;
				if (int.TryParse(this.PortString, out num))
				{
					return new int?(num);
				}
				return null;
			}
			set
			{
				this.PortString = value.ToString();
			}
		}

		public string UDPEmulatorPortString
		{
			get
			{
				return this._udpEmulatorPortString;
			}
			set
			{
				this.SetProperty<string>(ref this._udpEmulatorPortString, value, "UDPEmulatorPortString");
			}
		}

		public int? UDPEmulatorPort
		{
			get
			{
				int num;
				if (int.TryParse(this.UDPEmulatorPortString, out num))
				{
					return new int?(num);
				}
				return null;
			}
			set
			{
				RegistryHelper.SetValue("Preferences\\Servers", "EmulatorPort", value.Value);
				this.UDPEmulatorPortString = value.ToString();
			}
		}

		public string UdpPortString
		{
			get
			{
				return this._udpPortString;
			}
			set
			{
				this.SetProperty<string>(ref this._udpPortString, value, "UdpPortString");
			}
		}

		public int? UdpPort
		{
			get
			{
				int num;
				if (int.TryParse(this.UdpPortString, out num))
				{
					return new int?(num);
				}
				return null;
			}
			set
			{
				RegistryHelper.SetValue("Preferences\\Servers", "UdpPort", value.Value);
				this.UdpPortString = value.ToString();
			}
		}

		public string this[string columnName]
		{
			get
			{
				string text = string.Empty;
				if (!(columnName == "PortString"))
				{
					if (!(columnName == "UDPEmulatorPortString"))
					{
						if (columnName == "UdpPortString")
						{
							if (string.IsNullOrEmpty(this.UdpPortString))
							{
								text = " ";
							}
						}
					}
					else if (string.IsNullOrEmpty(this.UDPEmulatorPortString))
					{
						text = " ";
					}
				}
				else if (string.IsNullOrEmpty(this.PortString))
				{
					text = " ";
				}
				return text;
			}
		}

		public string Error
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string DeviceName
		{
			get
			{
				return this._deviceName;
			}
			set
			{
				if (this.SetProperty<string>(ref this._deviceName, value, "DeviceName"))
				{
					this.OnPropertyChanged("DeviceName");
				}
			}
		}

		public PreferencesHttpVM(ISSEProcessor sse)
		{
			this.SSEClient = sse;
			this.InterfacesCollection = new ObservableCollection<ViewModelItem<PreferencesHttpVM.InterfaceItem>>();
			string text = "(";
			foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
				{
					Console.WriteLine(networkInterface.Name);
					foreach (UnicastIPAddressInformation unicastIPAddressInformation in networkInterface.GetIPProperties().UnicastAddresses)
					{
						if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork && !unicastIPAddressInformation.Address.ToString().StartsWith("169.254.") && !string.IsNullOrWhiteSpace(unicastIPAddressInformation.Address.ToString()))
						{
							PreferencesHttpVM.InterfaceItem interfaceItem = default(PreferencesHttpVM.InterfaceItem);
							interfaceItem.Ip = unicastIPAddressInformation.Address.ToString();
							text = text + interfaceItem.Ip + "; ";
							this.InterfacesCollection.Add(new ViewModelItem<PreferencesHttpVM.InterfaceItem>(unicastIPAddressInformation.Address.ToString(), interfaceItem));
						}
					}
				}
			}
			if (this.InterfacesCollection.Count >= 1)
			{
				if (text.EndsWith("; "))
				{
					text = text.Remove(text.LastIndexOf("; "));
				}
				text += ")";
				this.InterfacesCollection.Insert(0, new ViewModelItem<PreferencesHttpVM.InterfaceItem>("ANY " + text, new PreferencesHttpVM.InterfaceItem
				{
					Ip = ""
				}));
			}
			if (this.InterfacesCollection.Count == 0)
			{
				this.InterfacesCollection.Add(new ViewModelItem<PreferencesHttpVM.InterfaceItem>("N/A", default(PreferencesHttpVM.InterfaceItem)));
			}
			this.CurrentUdpInterface = this.UdpInterfacesCollection.FirstOrDefault<ViewModelItem<PreferencesHttpVM.InterfaceItem>>();
		}

		public override async Task Initialize()
		{
			this.IsEnabledOverLocalNetwork = (this._isEnabledOverLocalNetworkOld = HttpServerSettings.IsEnabledOverLocalNetwork());
			string interf = HttpServerSettings.GetInterface();
			if (string.IsNullOrEmpty(interf))
			{
				this.CurrentInterface = (this._currentInterfaceItemOld = this.InterfacesCollection.FirstOrDefault<ViewModelItem<PreferencesHttpVM.InterfaceItem>>());
			}
			else
			{
				this.CurrentInterface = (this._currentInterfaceItemOld = this.InterfacesCollection.FirstOrDefault((ViewModelItem<PreferencesHttpVM.InterfaceItem> item) => item.Value.Ip == interf));
				if (this.CurrentInterface == null)
				{
					this.CurrentInterface = (this._currentInterfaceItemOld = this.InterfacesCollection.FirstOrDefault<ViewModelItem<PreferencesHttpVM.InterfaceItem>>());
				}
			}
			this.Port = new int?(this._portOld = HttpServerSettings.GetPort());
			this.UdpPort = new int?(GamepadUdpServerSettings.Port);
			this.UDPEmulatorPort = new int?(this._emulatorPortOld = HttpServerSettings.GetEmulatorPort());
			this.DeviceName = (this._deviceNameOld = HttpServerSettings.GetDeviceName());
			TranslationManager.Instance.LanguageChanged += delegate([Nullable(2)] object s, EventArgs e)
			{
				this.OnPropertyChanged("UdpPortToolTip");
				this.OnPropertyChanged("UDPEmulatorPortToolTip");
			};
			this.SetDescription();
			await this.UdpServerState();
		}

		public override async Task<bool> ApplyChanges()
		{
			int? num = this.Port;
			int num2 = 0;
			if (!((num.GetValueOrDefault() <= num2) & (num != null)))
			{
				num = this.Port;
				num2 = 65536;
				if (!((num.GetValueOrDefault() >= num2) & (num != null)) && this.Port != null)
				{
					num = this.UDPEmulatorPort;
					num2 = 0;
					if (!((num.GetValueOrDefault() <= num2) & (num != null)))
					{
						num = this.UDPEmulatorPort;
						num2 = 65536;
						if (!((num.GetValueOrDefault() >= num2) & (num != null)) && this.UDPEmulatorPort != null)
						{
							num = this.UdpPort;
							num2 = 0;
							if (!((num.GetValueOrDefault() <= num2) & (num != null)))
							{
								num = this.UdpPort;
								num2 = 65536;
								if (!((num.GetValueOrDefault() >= num2) & (num != null)) && this.UdpPort != null)
								{
									goto IL_1E2;
								}
							}
							if (this.IsUdpSelected)
							{
								DTMessageBox.Show(DTLocalization.GetString(12267), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
								this.UdpPort = new int?(this.DefaultUdpPort);
								return false;
							}
							IL_1E2:
							int portOld = this._portOld;
							num = this.Port;
							if (!((portOld == num.GetValueOrDefault()) & (num != null)))
							{
								ViewModelItem<PreferencesHttpVM.InterfaceItem> currentInterface = this.CurrentInterface;
								if (!this.CheckPortAvailability((currentInterface != null) ? currentInterface.DisplayName : null, null))
								{
									DTMessageBox.Show(DTLocalization.GetString(12279), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
									this.Initialize();
									return false;
								}
							}
							int emulatorPortOld = this._emulatorPortOld;
							num = this.UDPEmulatorPort;
							if (!((emulatorPortOld == num.GetValueOrDefault()) & (num != null)))
							{
								ViewModelItem<PreferencesHttpVM.InterfaceItem> currentInterface2 = this.CurrentInterface;
								if (!this.CheckPortAvailability((currentInterface2 != null) ? currentInterface2.DisplayName : null, new int?(this.UDPEmulatorPort.Value)))
								{
									DTMessageBox.Show(DTLocalization.GetString(12279), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
									this.Initialize();
									return false;
								}
							}
							await App.HttpClientService.Engine.SetIsUdpEnabledInPreferences(this.IsUdpSelected);
							if (this.IsUdpSelected)
							{
								GamepadUdpServerSettings.Port = this.UdpPort.Value;
								this.UdpPort = new int?(GamepadUdpServerSettings.Port);
								EngineClientService engine = App.HttpClientService.Engine;
								ViewModelItem<PreferencesHttpVM.InterfaceItem> currentUdpInterface = this.CurrentUdpInterface;
								await engine.RequestUDPStart(((currentUdpInterface != null) ? currentUdpInterface.Value.Ip : null) ?? this.UdpInterfacesCollection.FirstOrDefault<ViewModelItem<PreferencesHttpVM.InterfaceItem>>().Value.Ip, this.UdpPort.Value);
							}
							if (!this.IsUdpSelected && this.IsUdpRunning)
							{
								await App.HttpClientService.Engine.RequestUDPStopping();
							}
							this.UdpServerState();
							this.DeviceName = this.DeviceName.Trim();
							if (this.CurrentInterface == null || this.CurrentInterface == this._currentInterfaceItemOld)
							{
								int emulatorPortOld2 = this._emulatorPortOld;
								num = this.UDPEmulatorPort;
								if ((emulatorPortOld2 == num.GetValueOrDefault()) & (num != null))
								{
									int portOld2 = this._portOld;
									num = this.Port;
									if (((portOld2 == num.GetValueOrDefault()) & (num != null)) && this._isEnabledOverLocalNetworkOld == this.IsEnabledOverLocalNetwork && !(this._deviceNameOld != this.DeviceName))
									{
										goto IL_DF9;
									}
								}
							}
							ViewModelItem<PreferencesHttpVM.InterfaceItem> currentInterfaceItemOld = this._currentInterfaceItemOld;
							string prevInterf = ((currentInterfaceItemOld != null) ? currentInterfaceItemOld.Value.Ip : null);
							int prevPort = this._portOld;
							int prevUdpPort = this._emulatorPortOld;
							bool prevLocalSharing = this._isEnabledOverLocalNetworkOld;
							string prevDevName = this._deviceNameOld;
							string actualLocalRoute = HttpServerSettings.GetActualLocalRoute();
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 3);
							defaultInterpolatedStringHandler.AppendLiteral("http://");
							defaultInterpolatedStringHandler.AppendFormatted(actualLocalRoute);
							defaultInterpolatedStringHandler.AppendLiteral(":");
							defaultInterpolatedStringHandler.AppendFormatted<int>(this._portOld);
							defaultInterpolatedStringHandler.AppendLiteral("/");
							defaultInterpolatedStringHandler.AppendFormatted("v1.7");
							defaultInterpolatedStringHandler.AppendLiteral("/");
							string addr = defaultInterpolatedStringHandler.ToStringAndClear();
							int num3 = 0;
							try
							{
								AdminOperationsDecider adminOperations = (AdminOperationsDecider)IContainerProviderExtensions.Resolve<IAdminOperations>(App.Container);
								if (adminOperations == null)
								{
									throw new PreferencesHttpVM.HttpSavingException("Failed to resolve AdminOperations");
								}
								if (!adminOperations.RunWCFIfNeeded())
								{
									throw new PreferencesHttpVM.HttpSavingException("Failed to run UACHelper (or it's cancelled)");
								}
								bool flag = !HttpServerSettings.DEFAULT_PORTS.Contains(prevPort);
								TaskAwaiter<bool> taskAwaiter2;
								if (flag)
								{
									TaskAwaiter<bool> taskAwaiter = adminOperations.RemoveHttpUrlReservation(prevLocalSharing, prevPort).GetAwaiter();
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
									throw new PreferencesHttpVM.HttpSavingException("Failed to save Http settings")
									{
										NeedToRollbackReserveUrl = true,
										NeedToRoolbackHttpSettings = false
									};
								}
								flag = !HttpServerSettings.DEFAULT_PORTS.Contains(this.Port.Value);
								if (flag)
								{
									TaskAwaiter<bool> taskAwaiter = adminOperations.ReserveHttpUrl(this.IsEnabledOverLocalNetwork, this.Port.Value).GetAwaiter();
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
									throw new PreferencesHttpVM.HttpSavingException("Failed to save Http settings")
									{
										NeedToRollbackReserveUrl = true,
										NeedToRoolbackHttpSettings = false
									};
								}
								if (prevUdpPort != this.UDPEmulatorPort.Value)
								{
									await adminOperations.RemoveUDPEmulatorPort(prevUdpPort);
									TaskAwaiter<bool> taskAwaiter = adminOperations.ReserveUDPEmulatorPort(this.UDPEmulatorPort.Value).GetAwaiter();
									if (!taskAwaiter.IsCompleted)
									{
										await taskAwaiter;
										taskAwaiter = taskAwaiter2;
										taskAwaiter2 = default(TaskAwaiter<bool>);
									}
									if (!taskAwaiter.GetResult())
									{
										throw new PreferencesHttpVM.HttpSavingException("Failed to save UDP Emulator settings")
										{
											NeedToRollbackReserveUrl = true,
											NeedToRoolbackHttpSettings = false
										};
									}
								}
								string newName = (string.IsNullOrWhiteSpace(this.DeviceName) ? HttpServerSettings.GetDefaultMachineName() : this.DeviceName);
								ViewModelItem<PreferencesHttpVM.InterfaceItem> currentInterface3 = this.CurrentInterface;
								if (!HttpServerSettings.SetInterface((currentInterface3 != null) ? currentInterface3.Value.Ip : null) || !HttpServerSettings.SetPort(this.Port.Value) || !HttpServerSettings.SetEmulatorPort(this.UDPEmulatorPort.Value) || !HttpServerSettings.SetEnabledOverLocalNetwork(this.IsEnabledOverLocalNetwork) || !HttpServerSettings.SetDeviceName(newName))
								{
									throw new PreferencesHttpVM.HttpSavingException("Failed to save Http settings")
									{
										NeedToRollbackReserveUrl = true,
										NeedToRoolbackHttpSettings = true
									};
								}
								int num4 = prevPort;
								num = this.Port;
								if (!((num4 == num.GetValueOrDefault()) & (num != null)))
								{
									App.HttpClientService.Engine.RequestHttpRestart(addr);
								}
								int num5 = prevUdpPort;
								num = this.UDPEmulatorPort;
								if (!((num5 == num.GetValueOrDefault()) & (num != null)))
								{
									await App.HttpClientService.Engine.RequestUdpRestart();
								}
								await Task.Delay(1000);
								this.SSEClient.Restart();
								if (this.IsEnabledOverLocalNetwork != prevLocalSharing)
								{
									App.HttpClientService.Engine.RequestHttpRestart(addr);
								}
								this._currentInterfaceItemOld = this.CurrentInterface;
								this._portOld = this.Port.Value;
								this._emulatorPortOld = this.UDPEmulatorPort.Value;
								this._isEnabledOverLocalNetworkOld = this.IsEnabledOverLocalNetwork;
								this._deviceNameOld = newName;
								adminOperations = null;
								newName = null;
							}
							catch (PreferencesHttpVM.HttpSavingException obj)
							{
								num3 = 1;
							}
							num2 = num3;
							object obj;
							if (num2 == 1)
							{
								PreferencesHttpVM.HttpSavingException e = (PreferencesHttpVM.HttpSavingException)obj;
								if (e.NeedToRollbackReserveUrl)
								{
									AdminOperationsDecider adminOperations = (AdminOperationsDecider)IContainerProviderExtensions.Resolve<IAdminOperations>(App.Container);
									if (!HttpServerSettings.DEFAULT_PORTS.Contains(this.Port.Value))
									{
										AdminOperationsDecider adminOperationsDecider = adminOperations;
										await ((adminOperationsDecider != null) ? adminOperationsDecider.RemoveHttpUrlReservation(this.IsEnabledOverLocalNetwork, this.Port.Value) : null);
									}
									if (!HttpServerSettings.DEFAULT_PORTS.Contains(prevPort))
									{
										AdminOperationsDecider adminOperationsDecider2 = adminOperations;
										await ((adminOperationsDecider2 != null) ? adminOperationsDecider2.ReserveHttpUrl(prevLocalSharing, prevPort) : null);
									}
									adminOperations = null;
								}
								if (e.NeedToRoolbackHttpSettings)
								{
									HttpServerSettings.SetInterface(prevInterf);
									HttpServerSettings.SetPort(prevPort);
									HttpServerSettings.SetEmulatorPort(prevUdpPort);
									HttpServerSettings.SetEnabledOverLocalNetwork(prevLocalSharing);
									HttpServerSettings.SetDeviceName(prevDevName);
									this.Initialize();
								}
								App.HttpClientService.Engine.RequestHttpRestart(addr);
								await App.HttpClientService.Engine.RequestUdpRestart();
								await Task.Delay(1000);
								this.SSEClient.Restart();
								Tracer.TraceWrite(e.Message, false);
								DTMessageBox.Show(e.localizedMsg, MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
								return false;
							}
							obj = null;
							prevInterf = null;
							prevDevName = null;
							addr = null;
							IL_DF9:
							return true;
						}
					}
					DTMessageBox.Show(DTLocalization.GetString(12267), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
					this.UDPEmulatorPort = new int?(this.DefaultUDPEmulatorPort);
					return false;
				}
			}
			DTMessageBox.Show(DTLocalization.GetString(12267), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
			this.Port = new int?(this.DefaultPort);
			return false;
		}

		private bool CheckPortAvailability(string address, int? port = null)
		{
			if (port == null)
			{
				port = this.Port;
			}
			if (address.Contains("ANY") && !this.PortIsAvailable(IPAddress.Any, port.Value))
			{
				return false;
			}
			string[] array = address.Split(')', StringSplitOptions.None);
			if (array.Count<string>() > 0)
			{
				array = array[0].Split('(', StringSplitOptions.None);
				if (array.Count<string>() == 2)
				{
					array = array[1].Split(';', StringSplitOptions.None);
				}
				foreach (string text in array)
				{
					if (!this.PortIsAvailable(IPAddress.Parse(text.Trim()), port.Value))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		private bool PortIsAvailable(IPAddress addr, int port)
		{
			int? num = this.Port;
			int num2 = 0;
			if (!((num.GetValueOrDefault() <= num2) & (num != null)))
			{
				num = this.Port;
				num2 = 65536;
				if (!((num.GetValueOrDefault() >= num2) & (num != null)) && this.Port != null)
				{
					TcpListener tcpListener = new TcpListener(addr, port);
					try
					{
						tcpListener.Start();
						tcpListener.Stop();
					}
					catch (Exception)
					{
						return false;
					}
					return true;
				}
			}
			return false;
		}

		private void SetDescription()
		{
			base.Description = (this.IsEnabledOverLocalNetwork ? new Localizable() : new Localizable(12278));
		}

		private bool _isEnabledOverLocalNetwork;

		private bool _isEnabledOverLocalNetworkOld;

		private bool _isUdpSelected;

		private bool _isUdpRunning;

		private bool _isUdpServerHasException;

		private bool _isUdpReserved;

		private ViewModelItem<PreferencesHttpVM.InterfaceItem> _currentInterfaceItemOld;

		private ViewModelItem<PreferencesHttpVM.InterfaceItem> _currentInterfaceItem;

		public ObservableCollection<ViewModelItem<PreferencesHttpVM.InterfaceItem>> _UdpInterfacesCollection = new ObservableCollection<ViewModelItem<PreferencesHttpVM.InterfaceItem>>
		{
			new ViewModelItem<PreferencesHttpVM.InterfaceItem>(DTLocalization.GetString(12329), new PreferencesHttpVM.InterfaceItem
			{
				Ip = "127.0.0.1"
			})
			{
				IsSelected = true
			},
			new ViewModelItem<PreferencesHttpVM.InterfaceItem>(DTLocalization.GetString(12330), new PreferencesHttpVM.InterfaceItem
			{
				Ip = "0.0.0.0"
			})
		};

		private ObservableCollection<ViewModelItem<PreferencesHttpVM.InterfaceItem>> _InterfacesCollection;

		private ViewModelItem<PreferencesHttpVM.InterfaceItem> _currentUdpInterfaceItem;

		private string _udpAddressString;

		private string _portString;

		private int _portOld;

		private int _emulatorPortOld;

		private string _udpEmulatorPortString;

		private string _udpPortString;

		private string _deviceName;

		private string _deviceNameOld;

		private class HttpSavingException : Exception
		{
			public HttpSavingException(string msg)
				: base(msg)
			{
			}

			public string localizedMsg { get; set; } = DTLocalization.GetString(12270);

			public bool NeedToRollbackReserveUrl { get; set; }

			public bool NeedToRoolbackHttpSettings { get; set; }
		}

		public struct InterfaceItem
		{
			public string Ip { readonly get; set; }
		}
	}
}
