using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.Controls.ComboBoxes;
using DiscSoft.NET.Common.View.RecolorableImages;
using DiscSoft.NET.Common.View.SecondaryWindows.Base;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using GIMXEngine;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using reWASDUI.Infrastructure;
using XBEliteWPF.Infrastructure.ExternalDevices;

namespace reWASDUI.Views.SecondaryWindows
{
	public partial class ESP32Preferences : BaseSecondaryWindow
	{
		public List<ESP32Preferences.BaudRate> BaudRates { get; } = new List<ESP32Preferences.BaudRate>();

		public string LatencySpeedMessage
		{
			get
			{
				return string.Format(DTLocalization.GetString(12664), this._device.LatencySpeed);
			}
		}

		public ESP32Preferences.BaudRate SelectedBaudRate
		{
			get
			{
				return this._selectedBaudRate;
			}
			set
			{
				if (this._selectedBaudRate != value)
				{
					this._selectedBaudRate = value;
					this.OnPropertyChanged("SelectedBaudRate");
				}
			}
		}

		public bool PingInProgress
		{
			get
			{
				return this._pingInProgress;
			}
			set
			{
				if (this._pingInProgress != value)
				{
					this._pingInProgress = value;
					this.OnPropertyChanged("PingInProgress");
				}
			}
		}

		public string MinPingTime
		{
			get
			{
				return this._minPingTime;
			}
			set
			{
				if (value != this._minPingTime)
				{
					this._minPingTime = value;
					this.OnPropertyChanged("MinPingTime");
				}
			}
		}

		public string MaxPingTime
		{
			get
			{
				return this._maxPingTime;
			}
			set
			{
				if (value != this._maxPingTime)
				{
					this._maxPingTime = value;
					this.OnPropertyChanged("MaxPingTime");
				}
			}
		}

		public string AvgPingTime
		{
			get
			{
				return this._avgPingTime;
			}
			set
			{
				if (value != this._avgPingTime)
				{
					this._avgPingTime = value;
					this.OnPropertyChanged("AvgPingTime");
				}
			}
		}

		public string NumPingPacketsSent
		{
			get
			{
				return this._numPingPacketsSent;
			}
			set
			{
				if (value != this._numPingPacketsSent)
				{
					this._numPingPacketsSent = value;
					this.OnPropertyChanged("NumPingPacketsSent");
				}
			}
		}

		public bool CanChangeLatencyTimer
		{
			get
			{
				return this._device.IsSerialPortFTDI;
			}
		}

		public bool NeedChangeLatencyTimer
		{
			get
			{
				return this._needChangeLatencyTimer;
			}
			set
			{
				if (value != this._needChangeLatencyTimer)
				{
					this._needChangeLatencyTimer = value;
					this.OnPropertyChanged("NeedChangeLatencyTimer");
				}
			}
		}

		public ESP32Preferences(ExternalDevice device)
		{
			this._device = device;
			this.BaudRates.Add(new ESP32Preferences.BaudRate
			{
				Name = "0.5M",
				baudRate = 500000U
			});
			this.BaudRates.Add(new ESP32Preferences.BaudRate
			{
				Name = "0.9M",
				baudRate = 921600U
			});
			this.BaudRates.Add(new ESP32Preferences.BaudRate
			{
				Name = "1M",
				baudRate = 1000000U
			});
			this.BaudRates.Add(new ESP32Preferences.BaudRate
			{
				Name = "1.5M",
				baudRate = 1500000U
			});
			this.BaudRates.Add(new ESP32Preferences.BaudRate
			{
				Name = "2M",
				baudRate = 2000000U
			});
			this.BaudRates.Add(new ESP32Preferences.BaudRate
			{
				Name = "3M",
				baudRate = 3000000U
			});
			this.BaudRates.Add(new ESP32Preferences.BaudRate
			{
				Name = "5M",
				baudRate = 5000000U
			});
			this._device.GetSerialPortInformation();
			this.SelectedBaudRate = this.BaudRates.FirstOrDefault((ESP32Preferences.BaudRate item) => item.baudRate == this._device.BaudRate);
			if (this.SelectedBaudRate == null)
			{
				this.SelectedBaudRate = this.BaudRates.First<ESP32Preferences.BaudRate>();
			}
			this.CheckNeedChangeLatencyTimer();
			this.InitializeComponent();
			App.EventAggregator.GetEvent<ExternalHelperChanged>().Subscribe(new Action<object>(this.OnExternalDeviceStatusChanged));
			base.Closed += this.ESP32Preferences_Closed;
		}

		private void ESP32Preferences_Closed(object sender, EventArgs e)
		{
			App.EventAggregator.GetEvent<ExternalHelperChanged>().Unsubscribe(new Action<object>(this.OnExternalDeviceStatusChanged));
		}

		private void OnExternalDeviceStatusChanged(object o)
		{
			this._device.GetSerialPortInformation();
			this.OnPropertyChanged("CanChangeLatencyTimer");
			this.OnPropertyChanged("LatencySpeedMessage");
		}

		private void CheckNeedChangeLatencyTimer()
		{
			this.NeedChangeLatencyTimer = this._device.NeedChangeLatencySpeed;
			this.OnPropertyChanged("LatencySpeedMessage");
		}

		protected override void OkButton_Click(object sender, RoutedEventArgs e)
		{
			this._device.BaudRate = this.SelectedBaudRate.baudRate;
			this.WindowResult = MessageBoxResult.OK;
			base.Close();
		}

		private void StartPingPooler()
		{
			this._getTimeoutsCounter = 0;
			this.PingInProgress = true;
			this._pollingTimer = new DispatcherTimer();
			this._pollingTimer.Tick += delegate([Nullable(2)] object o, EventArgs e)
			{
				this.GetTimeouts();
			};
			this._pollingTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
			this._pollingTimer.Start();
		}

		private async void StopPingPooler()
		{
			this._pollingTimer.Stop();
			this.PingInProgress = false;
			await App.HttpClientService.Gamepad.DeleteSpecialProfiles();
		}

		private async void GetTimeouts()
		{
			this._getTimeoutsCounter++;
			PingInfo pingInfo = await App.HttpClientService.Gamepad.GetProfilePingInfo(this._profileId);
			if (this._getTimeoutsCounter >= 50 || pingInfo == null || (pingInfo.NumPingPacketsSent == 0U && this._getTimeoutsCounter > 10))
			{
				this.StopPingPooler();
				if (pingInfo.NumPingPacketsSent == 0U)
				{
					DTMessageBox.Show(DTLocalization.GetString(12626), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
				}
			}
			else
			{
				string @string = DTLocalization.GetString(12117);
				this.MinPingTime = (pingInfo.MinPingTime / 1000UL).ToString() + @string;
				this.MaxPingTime = (pingInfo.MaxPingTime / 1000UL).ToString() + @string;
				this.AvgPingTime = (pingInfo.AvgPingTime / 1000UL).ToString() + @string;
				this.NumPingPacketsSent = pingInfo.NumPingPacketsSent.ToString();
			}
		}

		private async void Ping_Click(object sender, RoutedEventArgs e)
		{
			if (!this._device.IsOnlineAndCorrect)
			{
				DTMessageBox.Show(DTLocalization.GetString(12627), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
			}
			else
			{
				if (!string.IsNullOrEmpty(this._device.SerialPort))
				{
					await App.HttpClientService.ExternalDevices.ExternalDeviceDisableRemapForSerialPort(this._device.SerialPort);
				}
				using (SPHandler sphandler = new SPHandler(this._device.SerialPort, 2))
				{
					if (!sphandler.Open())
					{
						DTMessageBox.Show(DTLocalization.GetString(12629), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
						return;
					}
					ulong num;
					bool flag2;
					bool flag3;
					bool flag = sphandler.CheckForRewasdFirmwareVersion(ref num, ref flag2, ref flag3);
					sphandler.Close();
					if (!flag)
					{
						DTMessageBox.Show(DTLocalization.GetString(12628), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
						return;
					}
				}
				ushort num2 = await App.HttpClientService.Gamepad.ApplyHardwareDonglePingProfile(new SpecialProfileInfo
				{
					ExternalDeviceType = this._device.DeviceType,
					BaudRate = this.SelectedBaudRate.baudRate,
					ComPort = this._device.SerialPort
				});
				if (num2 != 0)
				{
					this._profileId = num2;
					this.StartPingPooler();
				}
			}
		}

		private async void ChangeLatencyTimer_Click(object sender, RoutedEventArgs e)
		{
			await App.AdminOperations.ChangeESP32DeviceLatency(this._device.SerialPort, this.cbLatency.SelectedIndex + 2);
			this.CheckNeedChangeLatencyTimer();
		}

		private async void RestoreLatency_Click(object sender, RoutedEventArgs e)
		{
			await App.AdminOperations.ChangeESP32DeviceLatency(this._device.SerialPort, this._device.DefaultLatencySpeed);
			this.CheckNeedChangeLatencyTimer();
		}

		private DispatcherTimer _pollingTimer;

		private ushort _profileId;

		private ExternalDevice _device;

		private ESP32Preferences.BaudRate _selectedBaudRate;

		private bool _pingInProgress;

		private string _minPingTime = "...";

		private string _maxPingTime = "...";

		public string _avgPingTime = "...";

		public string _numPingPacketsSent = "...";

		public bool _needChangeLatencyTimer;

		private int _getTimeoutsCounter;

		public class BaudRate
		{
			public override string ToString()
			{
				return this.Name;
			}

			public string Name;

			public uint baudRate;
		}
	}
}
