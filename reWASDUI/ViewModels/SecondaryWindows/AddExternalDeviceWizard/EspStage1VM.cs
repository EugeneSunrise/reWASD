using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using GIMXEngine;
using reWASDCommon.Utils;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class EspStage1VM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.EspStage1;
			}
		}

		public bool FirmwareIsUpToDate { get; set; }

		public bool FirmwareExistAndIsNotUpToDate { get; set; }

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

		public bool IsCheckingNewDevice
		{
			get
			{
				return this._isCheckingNewDevice;
			}
			set
			{
				if (this._isCheckingNewDevice != value)
				{
					this._isCheckingNewDevice = value;
					if (value)
					{
						this._dataProcessingThread = new Thread(new ThreadStart(this.CheckFirmware))
						{
							IsBackground = true
						};
						this._dataProcessingThread.Start();
						this.OnPropertyChanged("IsCheckingNewDevice");
					}
				}
			}
		}

		public SPHandler SpHandler
		{
			get
			{
				return this._spHandler;
			}
			set
			{
				if (this._spHandler == value)
				{
					return;
				}
				this._spHandler = value;
				this.OnPropertyChanged("SpHandler");
			}
		}

		public string WaitingConnectDescription
		{
			get
			{
				return this._waitingConnectDescription;
			}
			set
			{
				if (this._waitingConnectDescription == value)
				{
					return;
				}
				this._waitingConnectDescription = value;
				this.OnPropertyChanged("WaitingConnectDescription");
			}
		}

		public string WaitingConnectHeader
		{
			get
			{
				return this._waitingConnectHeader;
			}
			set
			{
				if (this._waitingConnectHeader == value)
				{
					return;
				}
				this._waitingConnectHeader = value;
				this.OnPropertyChanged("WaitingConnectHeader");
			}
		}

		public EspStage1VM(WizardVM wizard)
			: base(wizard)
		{
			this.espToolDownloader = new EspToolDownloader(wizard);
		}

		public override void OnShowPage()
		{
			this.SetBaseTexts(false);
			this.StartSerialPortPoller();
		}

		private void SetBaseTexts(bool checkingFirmware)
		{
			this.WaitingConnectHeader = DTLocalization.GetString(checkingFirmware ? 12045 : 12043);
			if (checkingFirmware)
			{
				this.WaitingConnectDescription = string.Format(DTLocalization.GetString(12679), this.SerialPort, this._wizard.DeviceTypeStr);
				return;
			}
			this.WaitingConnectDescription = string.Concat(new string[]
			{
				string.Format(DTLocalization.GetString(12620), this._wizard.DeviceTypeStr),
				Environment.NewLine,
				Environment.NewLine,
				string.Format(DTLocalization.GetString(12653), this._wizard.Result.DeviceType.ToString()),
				Environment.NewLine,
				"<a href=\"https://help.rewasd.com/external-devices/esp32-bluetooth-adapter.html#native_drivers\">",
				DTLocalization.GetString(11777),
				"</a>"
			});
		}

		private void SetErrorTexts()
		{
			this.WaitingConnectHeader = DTLocalization.GetString(12657);
			this.WaitingConnectDescription = DTLocalization.GetString(12658) + Environment.NewLine + Environment.NewLine + string.Format(DTLocalization.GetString(12681), this._wizard.Result.DeviceType, "https://help.rewasd.com/external-devices/esp32-bluetooth-adapter.html#adapter-wasn't-detected");
		}

		public override void OnHidePage()
		{
			this.StopSerialPortPoller();
			this.StopProcessingThread();
		}

		private async void StartSerialPortPoller()
		{
			this.IsWaitingForNewDevice = true;
			this._serialPortNames = System.IO.Ports.SerialPort.GetPortNames().ToList<string>();
			int counter = 0;
			for (;;)
			{
				this.CheckNewSerialPort();
				if (!this.IsWaitingForNewDevice)
				{
					break;
				}
				await Task.Delay(1000);
				counter++;
				if (counter == 60)
				{
					this.SetErrorTexts();
				}
			}
		}

		private void StopSerialPortPoller()
		{
			this.IsWaitingForNewDevice = false;
		}

		private void CheckNewSerialPort()
		{
			List<string> list = System.IO.Ports.SerialPort.GetPortNames().ToList<string>();
			List<string> list2 = list.Except(this._serialPortNames).ToList<string>();
			if (list2.Any<string>())
			{
				this.SerialPort = list2.First<string>();
				this.SetBaseTexts(true);
				this.StopSerialPortPoller();
				this.IsCheckingNewDevice = true;
			}
			this._serialPortNames = list;
		}

		protected override void NavigateToNextPage()
		{
			if (this.espToolDownloader.IsExistAndCorrect())
			{
				base.GoPage(PageType.EspStage3);
				return;
			}
			this.espToolDownloader.PrevPage = PageType.EspStage1;
			this.espToolDownloader.NextPage = PageType.EspStage3;
			this.espToolDownloader.Download();
		}

		protected override void NavigatePreviousPage()
		{
			base.GoPage(PageType.DeviceType);
		}

		private async void CheckFirmware()
		{
			await Task.Delay(2000);
			this.FirmwareIsUpToDate = false;
			SPHandler sphandler = new SPHandler(this._wizard.Result.SerialPort, this._wizard.Result.DeviceType);
			this.SpHandler = sphandler;
			try
			{
				if (this.SpHandler.Open())
				{
					ulong num;
					bool flag;
					bool flag2;
					if (this.SpHandler.CheckForRewasdFirmwareVersion(ref num, ref flag, ref flag2))
					{
						if (this._wizard.Result.DeviceType == 2)
						{
							if (num != 0UL)
							{
								this._wizard.Result.HardwareDongleBluetoothMacAddress = UtilsCommon.MacAddressToString(num, ":");
							}
							if (flag)
							{
								this._wizard.Result.HardwareDongleLedPresent = true;
							}
							this._wizard.Result.BaudRate = 500000U;
							this._wizard.Result.RefreshComPorts();
						}
						this.FirmwareIsUpToDate = true;
					}
					this.FirmwareExistAndIsNotUpToDate = flag2;
				}
			}
			finally
			{
				if (sphandler != null)
				{
					sphandler.Dispose();
				}
			}
			ThreadHelper.ExecuteInMainDispatcher(delegate
			{
				this.SetFirmwareIsUpToDate(this.FirmwareIsUpToDate);
				this.StopProcessingThread();
			}, false);
		}

		private void SetFirmwareIsUpToDate(bool firmwareIsUpToDate)
		{
			this._wizard.Result.DefaultLatencySpeed = this._wizard.Result.LatencySpeed;
			this.IsCheckingNewDevice = false;
			if (firmwareIsUpToDate)
			{
				base.GoPage(this._wizard.Result.NeedChangeLatencySpeed ? PageType.EspLatencySpeed : PageType.EspStage4);
				return;
			}
			this.NavigateToNextPage();
		}

		private void StopProcessingThread()
		{
			Thread dataProcessingThread = this._dataProcessingThread;
			if (dataProcessingThread != null)
			{
				dataProcessingThread.Interrupt();
			}
			this._dataProcessingThread = null;
		}

		private const int WAIT_PERIOD = 1000;

		private const int WAIT_COUNT = 60;

		private List<string> _serialPortNames;

		private Thread _dataProcessingThread;

		private SPHandler _spHandler;

		private EspToolDownloader espToolDownloader;

		private string _serialPort;

		private bool _isWaitingForNewDevice;

		private bool _isCheckingNewDevice;

		private string _waitingConnectDescription;

		private string _waitingConnectHeader;
	}
}
