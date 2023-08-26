using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using GIMXEngine;
using Prism.Commands;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Infrastructure.ExternalDevices;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class GimxStage3VM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.GimxStage3;
			}
		}

		public GimxStage3VM(WizardVM wizard)
			: base(wizard)
		{
			this.ExternalDeviceCollection = new ObservableCollection<ExternalDevice>(App.GamepadService.ExternalDevices);
			this._dispatcher = Application.Current.Dispatcher;
		}

		protected override void NavigatePreviousPage()
		{
			this.OnPageLeave();
			base.GoPage(PageType.GimxStage2);
		}

		protected override void NavigateToNextPage()
		{
			this.OnPageLeave();
			this.SaveExternalDevices();
			base.GoPage(PageType.GimxStage4);
		}

		public override void OnShowPage()
		{
			this.IsFirmwareVersionChecked = false;
			this.FirmwareIsUpToDate = false;
			this.IsFirmwareRewriteInProgress = false;
			this._dataProcessingThread = new Thread(new ThreadStart(this.CheckFirmware))
			{
				IsBackground = true
			};
			this._dataProcessingThread.Start();
		}

		public override void OnHidePage()
		{
			this.OnPageLeave();
		}

		public new ICommand CancelCommand
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
			this.OnPageLeave();
			this._wizard.OnCancel();
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

		private void OnPageLeave()
		{
			this.StopProcessingThread();
			this.StopBootLoaderPortPooler();
		}

		public string HeaderText
		{
			get
			{
				if (!this.IsFirmwareVersionChecked)
				{
					return DTLocalization.GetString(12045);
				}
				if (this.FirmwareIsUpToDate && this.IsFirmwareVersionChecked && !this.IsFirmwareRewriteInProgress)
				{
					return DTLocalization.GetString(12051);
				}
				if (!this.FirmwareIsUpToDate && this.IsFirmwareVersionChecked && !this.IsFirmwareRewriteInProgress)
				{
					return DTLocalization.GetString(12053);
				}
				if (this.IsFirmwareRewriteInProgress)
				{
					return DTLocalization.GetString(12054);
				}
				return string.Empty;
			}
		}

		public string MessageText
		{
			get
			{
				if (!this.IsFirmwareVersionChecked)
				{
					return DTLocalization.GetString(12049);
				}
				if (this.FirmwareIsUpToDate && this.IsFirmwareVersionChecked && !this.IsFirmwareRewriteInProgress && this.FirmwareIsFlashedSuccessfully)
				{
					return DTLocalization.GetString(12445);
				}
				if (this.FirmwareIsUpToDate && this.IsFirmwareVersionChecked && !this.IsFirmwareRewriteInProgress)
				{
					return DTLocalization.GetString(12049);
				}
				if (!this.FirmwareIsUpToDate && this.IsFirmwareVersionChecked && !this.IsFirmwareRewriteInProgress)
				{
					return DTLocalization.GetString(12052);
				}
				if (this.IsFirmwareRewriteInProgress)
				{
					return DTLocalization.GetString(12055);
				}
				return string.Empty;
			}
		}

		public string ToolTipText
		{
			get
			{
				if (!this.IsFirmwareVersionChecked)
				{
					return DTLocalization.GetString(12045);
				}
				if (!this.FirmwareIsUpToDate && this.IsFirmwareVersionChecked && !this.IsFirmwareRewriteInProgress)
				{
					return DTLocalization.GetString(12053);
				}
				if (this.IsFirmwareRewriteInProgress)
				{
					return DTLocalization.GetString(12054);
				}
				return string.Empty;
			}
		}

		public bool FirmwareIsUpToDate
		{
			get
			{
				return this._firmwareIsUpToDate;
			}
			set
			{
				if (this._firmwareIsUpToDate != value)
				{
					this._firmwareIsUpToDate = value;
					this.OnPropertyChanged("FirmwareIsUpToDate");
					this.OnPropertyChanged("HeaderText");
					this.OnPropertyChanged("MessageText");
					this.OnPropertyChanged("ToolTipText");
				}
			}
		}

		public bool IsFirmwareVersionChecked
		{
			get
			{
				return this._isFirmwareVersionChecked;
			}
			set
			{
				if (this._isFirmwareVersionChecked == value)
				{
					return;
				}
				this._isFirmwareVersionChecked = value;
				this.OnPropertyChanged("IsFirmwareVersionChecked");
				this.OnPropertyChanged("HeaderText");
				this.OnPropertyChanged("MessageText");
				this.OnPropertyChanged("ToolTipText");
			}
		}

		public bool IsFirmwareRewriteInProgress
		{
			get
			{
				return this._isFirmwareRewriteInProgress;
			}
			set
			{
				if (this._isFirmwareRewriteInProgress == value)
				{
					return;
				}
				this._isFirmwareRewriteInProgress = value;
				this.OnPropertyChanged("IsFirmwareRewriteInProgress");
				this.OnPropertyChanged("HeaderText");
				this.OnPropertyChanged("MessageText");
				this.OnPropertyChanged("ToolTipText");
			}
		}

		public bool FirmwareIsFlashedSuccessfully
		{
			get
			{
				return this._firmwareIsFlashedSuccessfully;
			}
			set
			{
				if (this._firmwareIsFlashedSuccessfully == value)
				{
					return;
				}
				this._firmwareIsFlashedSuccessfully = value;
				this.OnPropertyChanged("FirmwareIsFlashedSuccessfully");
				this.OnPropertyChanged("HeaderText");
				this.OnPropertyChanged("MessageText");
				this.OnPropertyChanged("ToolTipText");
			}
		}

		public string BootLoaderSerialPort
		{
			get
			{
				return this._bootLoaderSerialPort;
			}
			set
			{
				if (this._bootLoaderSerialPort != value)
				{
					this._bootLoaderSerialPort = value;
					this.OnPropertyChanged("BootLoaderSerialPort");
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

		private void CheckFirmware()
		{
			using (this.SpHandler = new SPHandler(this._wizard.Result.SerialPort, this._externalDeviceType))
			{
				ulong num;
				bool flag;
				bool flag2;
				if (this.SpHandler.Open() && this.SpHandler.CheckForRewasdFirmwareVersion(ref num, ref flag, ref flag2))
				{
					this.FirmwareIsUpToDate = true;
					this.IsFirmwareVersionChecked = true;
					return;
				}
			}
			this.FirmwareIsUpToDate = false;
			this.IsFirmwareVersionChecked = true;
			if (this._wizard.CurrentPage == this)
			{
				ThreadHelper.ExecuteInMainDispatcher(delegate
				{
					this.StopProcessingThread();
					this.StartBootLoaderPortPooler();
				}, false);
			}
		}

		private void StartBootLoaderPortPooler()
		{
			this._serialPortNames = SerialPort.GetPortNames().ToList<string>();
			this._pollingTimer = new DispatcherTimer();
			this._pollingTimer.Tick += delegate([Nullable(2)] object o, EventArgs e)
			{
				this.CheckFirmwareWasOverWritten();
			};
			this._pollingTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
			this._pollingTimer.Start();
		}

		private void StopBootLoaderPortPooler()
		{
			DispatcherTimer pollingTimer = this._pollingTimer;
			if (pollingTimer == null)
			{
				return;
			}
			pollingTimer.Stop();
		}

		private void CheckFirmwareWasOverWritten()
		{
			List<string> list = SerialPort.GetPortNames().ToList<string>();
			List<string> newPortList = list.Except(this._serialPortNames).ToList<string>();
			if (newPortList.Any<string>())
			{
				this.IsFirmwareRewriteInProgress = true;
				this._dataProcessingThread = new Thread(delegate
				{
					this.BootLoaderSerialPort = newPortList.First<string>();
					bool flag = this.RewriteFirmware();
					using (SPHandler sphandler = new SPHandler(this._wizard.Result.SerialPort, this._externalDeviceType))
					{
						if (sphandler.Open())
						{
							ulong num;
							bool flag3;
							bool flag4;
							bool flag2 = sphandler.CheckForRewasdFirmwareVersion(ref num, ref flag3, ref flag4);
							this.FirmwareIsUpToDate = flag && flag2;
							this.FirmwareIsFlashedSuccessfully = flag && flag2;
						}
						sphandler.Close();
					}
					this.IsFirmwareRewriteInProgress = false;
					if (!this.FirmwareIsUpToDate && !this.IsFirmwareRewriteInProgress)
					{
						this.GoPage(PageType.GimxFailsStage);
					}
				})
				{
					IsBackground = true
				};
				this._dataProcessingThread.Start();
			}
			this._serialPortNames = list;
		}

		private bool RewriteFirmware()
		{
			bool flag;
			try
			{
				flag = new FirmwareLoader(this.BootLoaderSerialPort).WriteGIMX();
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				flag = false;
			}
			return flag;
		}

		private async void SaveExternalDevices()
		{
			if (this.ExternalDeviceCollection.All((ExternalDevice x) => x.ExternalDeviceId != this._wizard.Result.ExternalDeviceId))
			{
				this.ExternalDeviceCollection.Add(this._wizard.Result.Clone());
				ObservableCollection<ExternalDevice> observableCollection = new ObservableCollection<ExternalDevice>(this.ExternalDeviceCollection.Where((ExternalDevice x) => !x.IsDummy).ToList<ExternalDevice>());
				App.GamepadService.ExternalDevices = new ExternalDevicesCollection(observableCollection);
				await App.GamepadService.BinDataSerialize.SaveExternalDevices();
				App.GamepadService.ExternalDeviceRelationsHelper.Refresh();
				this.OnPropertyChanged("ExternalDevices");
			}
		}

		private Thread _dataProcessingThread;

		private SPHandler _spHandler;

		private List<string> _serialPortNames;

		private DispatcherTimer _pollingTimer;

		private Dispatcher _dispatcher;

		private ExternalDeviceType _externalDeviceType = 1;

		private DelegateCommand _cancelCommand;

		private bool _firmwareIsUpToDate;

		private bool _isFirmwareVersionChecked;

		private bool _isFirmwareRewriteInProgress;

		private bool _firmwareIsFlashedSuccessfully;

		private string _bootLoaderSerialPort;

		private ObservableCollection<ExternalDevice> _externalDeviceCollection;
	}
}
