using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.SecondaryWindows.Base;
using reWASDCommon.Utils;
using XBEliteWPF.Infrastructure.ExternalDevices;

namespace reWASDUI.Views.SecondaryWindows
{
	public partial class AddExternalClient : BaseSecondaryWindow
	{
		public string Alias
		{
			get
			{
				return this._alias;
			}
			set
			{
				this.SetProperty(ref this._alias, value, "Alias");
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
				}
			}
		}

		public ObservableCollection<ExternalClient> ExternalClientsCollection { get; set; } = new ObservableCollection<ExternalClient>();

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

		private void StartDiscovery()
		{
			this.thread = new Thread(delegate
			{
				new List<BluetoothUtils.BLUETOOTH_DEVICE_INFO>();
				try
				{
					for (;;)
					{
						foreach (BluetoothUtils.BLUETOOTH_DEVICE_INFO bluetooth_DEVICE_INFO in BluetoothUtils.BluetoothDiscovery(8))
						{
							ExternalClient device = new ExternalClient();
							device.Alias = bluetooth_DEVICE_INFO.szName;
							device.MacAddress = bluetooth_DEVICE_INFO.Address;
							Func<ExternalClient, bool> <>9__2;
							Func<ExternalClient, bool> <>9__3;
							Func<ExternalClient, bool> <>9__5;
							ThreadHelper.ExecuteInMainDispatcher(delegate
							{
								IEnumerable<ExternalClient> externalClientsCollection = this.ExternalClientsCollection;
								Func<ExternalClient, bool> func;
								if ((func = <>9__2) == null)
								{
									func = (<>9__2 = (ExternalClient x) => x.MacAddress != device.MacAddress);
								}
								if (externalClientsCollection.All(func))
								{
									this.ExternalClientsCollection.Add(device);
									return;
								}
								IEnumerable<ExternalClient> externalClientsCollection2 = this.ExternalClientsCollection;
								Func<ExternalClient, bool> func2;
								if ((func2 = <>9__3) == null)
								{
									func2 = (<>9__3 = (ExternalClient x) => x.MacAddress == device.MacAddress);
								}
								if (externalClientsCollection2.Where(func2).Any((ExternalClient x) => x.Alias == null) && !string.IsNullOrEmpty(device.Alias))
								{
									ObservableCollection<ExternalClient> externalClientsCollection3 = this.ExternalClientsCollection;
									Func<ExternalClient, bool> func3;
									if ((func3 = <>9__5) == null)
									{
										func3 = (<>9__5 = (ExternalClient x) => x.MacAddress == device.MacAddress);
									}
									externalClientsCollection3.Remove(func3);
									this.ExternalClientsCollection.Add(device);
								}
							}, true);
						}
					}
				}
				catch (Exception)
				{
				}
			});
			this.thread.IsBackground = true;
			this.thread.Start();
		}

		public AddExternalClient()
		{
			this.InitializeComponent();
			base.DataContext = this;
			this.Alias = "Device";
			this.MacAddressText = "00:00:00:00:00:00";
			this.StartDiscovery();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (this.thread != null && this.thread.IsAlive)
			{
				this.thread.Interrupt();
			}
		}

		private Thread thread;

		private string _alias;

		private string _macAddressText;

		private ExternalClient _externalClient;
	}
}
