using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using GIMXEngine;
using Prism.Commands;
using reWASDCommon.Utils;
using XBEliteWPF.Infrastructure.ExternalDevices;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class EspStage3VM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.EspStage3;
			}
		}

		public EspStage3VM(WizardVM wizard)
			: base(wizard)
		{
			this.ExternalDeviceCollection = new ObservableCollection<ExternalDevice>(App.GamepadService.ExternalDevices);
			this._firmwareConfigs = new List<EspStage3VM.FirmwareConfig>();
			this._firmwareConfigs.Add(new EspStage3VM.FirmwareConfig
			{
				FileName = "generic_dongle.bin",
				Name = DTLocalization.GetString(12625)
			});
			this._firmwareConfigs.Add(new EspStage3VM.FirmwareConfig
			{
				FileName = "atom_matrix_dongle.bin",
				Name = "M5Stack ATOM Matrix"
			});
			this._firmwareConfigs.Add(new EspStage3VM.FirmwareConfig
			{
				FileName = "atom_lite_dongle.bin",
				Name = "M5Stack ATOM Lite"
			});
			this._firmwareConfigs.Add(new EspStage3VM.FirmwareConfig
			{
				FileName = "atomu_dongle.bin",
				Name = "M5Stack AtomU"
			});
			this._firmwareConfigs.Add(new EspStage3VM.FirmwareConfig
			{
				FileName = "ttgo_display_dongle.bin",
				Name = "LILYGO TTGO T-Display"
			});
			this._currentFirmwareConfig = this._firmwareConfigs.First<EspStage3VM.FirmwareConfig>();
		}

		protected override void NavigatePreviousPage()
		{
			this.OnPageLeave();
			base.GoPage(PageType.EspStage1);
		}

		protected override void NavigateToNextPage()
		{
			this.OnPageLeave();
			this.SaveExternalDevices();
			base.GoPage(this._wizard.Result.NeedChangeLatencySpeed ? PageType.EspLatencySpeed : PageType.EspStage4);
		}

		public override void OnShowPage()
		{
			this.FirmwareExistAndIsNotUpToDate = (this._wizard.FindPage(PageType.EspStage1) as EspStage1VM).FirmwareExistAndIsNotUpToDate;
			this.FirmwareIsUpToDate = false;
			this.IsFirmwareRewriteInProgress = false;
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
		}

		public List<EspStage3VM.FirmwareConfig> FirmwareConfigs
		{
			get
			{
				return this._firmwareConfigs;
			}
		}

		public EspStage3VM.FirmwareConfig CurrentFirmwareConfig
		{
			get
			{
				return this._currentFirmwareConfig;
			}
			set
			{
				if (value != this._currentFirmwareConfig)
				{
					this._currentFirmwareConfig = value;
					this.OnPropertyChanged("CurrentFirmwareConfig");
				}
			}
		}

		public string HeaderText
		{
			get
			{
				if (!this.FirmwareExistAndIsNotUpToDate && !this.FirmwareIsUpToDate && !this.IsFirmwareRewriteInProgress)
				{
					return DTLocalization.GetString(12655);
				}
				if (this.FirmwareIsUpToDate && !this.IsFirmwareRewriteInProgress)
				{
					return DTLocalization.GetString(12051);
				}
				if (this.FirmwareExistAndIsNotUpToDate && !this.FirmwareIsUpToDate && !this.IsFirmwareRewriteInProgress)
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
				if (!this.FirmwareExistAndIsNotUpToDate && !this.FirmwareIsUpToDate && !this.IsFirmwareRewriteInProgress)
				{
					if (this._wizard.Result.DeviceType == 2)
					{
						return DTLocalization.GetString(12623);
					}
					return DTLocalization.GetString(12770);
				}
				else
				{
					if (this.FirmwareExistAndIsNotUpToDate && !this.FirmwareIsUpToDate && !this.IsFirmwareRewriteInProgress)
					{
						return string.Format(DTLocalization.GetString(12656), this._wizard.Result.SerialPort, this._wizard.DeviceTypeStr);
					}
					if (this.FirmwareIsUpToDate && !this.IsFirmwareRewriteInProgress)
					{
						return DTLocalization.GetString(12624);
					}
					if (this.IsFirmwareRewriteInProgress)
					{
						return string.Format(DTLocalization.GetString(12621), this._wizard.Result.DeviceType.ToString());
					}
					return string.Empty;
				}
			}
		}

		public string ToolTipText
		{
			get
			{
				if (!this.FirmwareIsUpToDate && !this.IsFirmwareRewriteInProgress)
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

		public bool FirmwareExistAndIsNotUpToDate
		{
			get
			{
				return this._firmwareExistAndIsNotUpToDate;
			}
			set
			{
				if (this._firmwareExistAndIsNotUpToDate != value)
				{
					this._firmwareExistAndIsNotUpToDate = value;
					this.OnPropertyChanged("FirmwareExistAndIsNotUpToDate");
					this.OnPropertyChanged("HeaderText");
					this.OnPropertyChanged("MessageText");
					this.OnPropertyChanged("ToolTipText");
				}
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
				if (value)
				{
					this.NavigateToNextPage();
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

		private void StartBootLoaderPortPooler()
		{
			this.IsFirmwareRewriteInProgress = true;
			this._dataProcessingThread = new Thread(delegate
			{
				FirmwareLoader.FirwareUpdateResult EsptoolResult = this.RewriteFirmware(this.FirmwareExistAndIsNotUpToDate);
				bool isEsptoolSuccess = EsptoolResult == 0;
				bool isUpToDate = false;
				using (SPHandler sphandler = new SPHandler(this._wizard.Result.SerialPort, this._wizard.Result.DeviceType))
				{
					if (sphandler.Open())
					{
						ulong num;
						bool flag;
						bool flag2;
						isUpToDate = sphandler.CheckForRewasdFirmwareVersion(ref num, ref flag, ref flag2);
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
						}
					}
					sphandler.Close();
				}
				ThreadHelper.ExecuteInMainDispatcher(delegate
				{
					this.FirmwareIsUpToDate = isEsptoolSuccess & isUpToDate;
					this.FirmwareIsFlashedSuccessfully = isEsptoolSuccess & isUpToDate;
					if (!this.FirmwareIsUpToDate)
					{
						EspFailsStageVM espFailsStageVM = this._wizard.FindPage(PageType.EspFailsStage) as EspFailsStageVM;
						espFailsStageVM.EspFailsHeader = DTLocalization.GetString(12079);
						if (EsptoolResult == 2)
						{
							espFailsStageVM.EspFailsMessage = DTLocalization.GetString(12659);
						}
						else
						{
							espFailsStageVM.EspFailsMessage = DTLocalization.GetString(12680) + Environment.NewLine + Environment.NewLine + string.Format(DTLocalization.GetString(12681), this._wizard.Result.DeviceType, "https://help.rewasd.com/external-devices/esp32-bluetooth-adapter.html#native_drivers");
						}
						espFailsStageVM.PreviousPage = PageType.EspStage3;
						this.GoPage(PageType.EspFailsStage);
						return;
					}
					this.NavigateToNextPage();
				}, false);
			})
			{
				IsBackground = true
			};
			this._dataProcessingThread.Start();
		}

		private FirmwareLoader.FirwareUpdateResult RewriteFirmware(bool onlyUpdate)
		{
			FirmwareLoader.FirwareUpdateResult firwareUpdateResult;
			try
			{
				FirmwareLoader firmwareLoader = new FirmwareLoader(this._wizard.Result.SerialPort);
				if (this._wizard.Result.DeviceType == 2)
				{
					firwareUpdateResult = firmwareLoader.WriteESP32(this._currentFirmwareConfig.FileName, onlyUpdate);
				}
				else
				{
					firwareUpdateResult = firmwareLoader.WriteESP32S2(onlyUpdate);
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				firwareUpdateResult = 1;
			}
			return firwareUpdateResult;
		}

		public ICommand FlashCommand
		{
			get
			{
				if (this._flashCommand == null)
				{
					this._flashCommand = new DelegateCommand(new Action(this.StartBootLoaderPortPooler));
				}
				return this._flashCommand;
			}
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

		private SPHandler _spHandler;

		private Thread _dataProcessingThread;

		private EspStage3VM.FirmwareConfig _currentFirmwareConfig;

		private List<EspStage3VM.FirmwareConfig> _firmwareConfigs;

		private DelegateCommand _cancelCommand;

		private bool _firmwareExistAndIsNotUpToDate;

		private bool _firmwareIsUpToDate;

		private bool _isFirmwareRewriteInProgress;

		private bool _firmwareIsFlashedSuccessfully;

		private ObservableCollection<ExternalDevice> _externalDeviceCollection;

		private DelegateCommand _flashCommand;

		public class FirmwareConfig
		{
			public string Name { get; set; }

			public string FileName { get; set; }
		}
	}
}
