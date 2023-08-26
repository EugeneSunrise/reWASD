using System;
using System.Windows.Input;
using DiscSoft.NET.Common.Localization;
using Prism.Commands;
using XBEliteWPF.Infrastructure.ExternalDevices;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class EspLatencySpeedVM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.EspLatencySpeed;
			}
		}

		public int LatencySpeed
		{
			get
			{
				ExternalDevice result = this._wizard.Result;
				if (result.IsSerialPortFTDI)
				{
					int num;
					result.GetESP32Latency(ref num);
					return num;
				}
				return 0;
			}
		}

		public int SelectedLatencyIndex
		{
			get
			{
				return this._selectedLatencyIndex;
			}
			set
			{
				if (this._selectedLatencyIndex == value)
				{
					return;
				}
				this._selectedLatencyIndex = value;
				this.OnPropertyChanged("SelectedLatencyIndex");
			}
		}

		public string EspDescriptionMessage
		{
			get
			{
				return string.Format(DTLocalization.GetString(12664), this.LatencySpeed);
			}
		}

		public string LatencySpeedDescription
		{
			get
			{
				return string.Format(DTLocalization.GetString(12663), this._wizard.Result.Alias + " (" + this._wizard.Result.SerialPort + ")", "https://help.rewasd.com/external-devices/esp32-bluetooth-adapter.html#latency-timer");
			}
		}

		public PageType PreviousPage { get; set; }

		public EspLatencySpeedVM(WizardVM wizard)
			: base(wizard)
		{
		}

		public override void OnShowPage()
		{
			this.OnPropertyChanged("EspDescriptionMessage");
			this.OnPropertyChanged("LatencySpeedDescription");
		}

		protected override void NavigatePreviousPage()
		{
			base.GoPage(PageType.EspStage3);
		}

		protected override async void NavigateToNextPage()
		{
			ExternalDevice device = this._wizard.Result;
			await App.AdminOperations.ChangeESP32DeviceLatency(device.SerialPort, this.SelectedLatencyIndex + 2);
			if (!device.NeedChangeLatencySpeed)
			{
				base.GoPage(PageType.EspStage4);
			}
		}

		public ICommand SkipCommand
		{
			get
			{
				if (this._skipCommand == null)
				{
					this._skipCommand = new DelegateCommand(new Action(this.Skip));
				}
				return this._skipCommand;
			}
		}

		protected virtual void Skip()
		{
			base.GoPage(PageType.EspStage4);
		}

		private int _selectedLatencyIndex;

		private DelegateCommand _skipCommand;
	}
}
