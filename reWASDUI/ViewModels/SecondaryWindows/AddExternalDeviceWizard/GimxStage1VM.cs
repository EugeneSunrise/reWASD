using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using DiscSoft.NET.Common.Localization;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class GimxStage1VM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.GimxStage1;
			}
		}

		public string SerialPort
		{
			get
			{
				return this._serialPort;
			}
			set
			{
				if (this._serialPort != value)
				{
					this._wizard.Result.SerialPort = value;
					this._serialPort = value;
					this.OnPropertyChanged("SerialPort");
				}
			}
		}

		public bool IsWaitingForNewDevice
		{
			get
			{
				return this._isWaitingForNewDevice;
			}
			set
			{
				if (this._isWaitingForNewDevice != value)
				{
					this._isWaitingForNewDevice = value;
					this.OnPropertyChanged("IsWaitingForNewDevice");
				}
			}
		}

		public string WaitingConnectDescr
		{
			get
			{
				return DTLocalization.GetString(12041);
			}
		}

		public GimxStage1VM(WizardVM wizard)
			: base(wizard)
		{
		}

		public override void OnShowPage()
		{
			this.StartSerialPortPoller();
		}

		public override void OnHidePage()
		{
			this.StopSerialPortPoller();
		}

		private void StartSerialPortPoller()
		{
			this.IsWaitingForNewDevice = true;
			this._serialPortNames = System.IO.Ports.SerialPort.GetPortNames().ToList<string>();
			this._pollingTimer = new DispatcherTimer();
			this._pollingTimer.Tick += delegate([Nullable(2)] object o, EventArgs e)
			{
				this.CheckNewSerialPort();
			};
			this._pollingTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
			this._pollingTimer.Start();
		}

		private void StopSerialPortPoller()
		{
			DispatcherTimer pollingTimer = this._pollingTimer;
			if (pollingTimer != null)
			{
				pollingTimer.Stop();
			}
			this.IsWaitingForNewDevice = false;
		}

		private void CheckNewSerialPort()
		{
			List<string> list = System.IO.Ports.SerialPort.GetPortNames().ToList<string>();
			List<string> list2 = list.Except(this._serialPortNames).ToList<string>();
			if (list2.Any<string>())
			{
				this.SerialPort = list2.First<string>();
				this.StopSerialPortPoller();
			}
			this._serialPortNames = list;
		}

		protected override void NavigateToNextPage()
		{
			base.GoPage(PageType.GimxStage2);
		}

		protected override void NavigatePreviousPage()
		{
			base.GoPage(PageType.DeviceType);
		}

		private List<string> _serialPortNames;

		private DispatcherTimer _pollingTimer;

		private string _serialPort;

		private bool _isWaitingForNewDevice;
	}
}
