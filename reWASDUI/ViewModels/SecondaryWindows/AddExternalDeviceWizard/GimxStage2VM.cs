using System;
using DiscSoft.NET.Common.Localization;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class GimxStage2VM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.GimxStage2;
			}
		}

		public GimxStage2VM(WizardVM wizard)
			: base(wizard)
		{
		}

		public string MessageText
		{
			get
			{
				return string.Format(DTLocalization.GetString(12046), this.SerialPort, "GIMX");
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
				if (this._serialPort == value)
				{
					return;
				}
				this._serialPort = value;
				this.OnPropertyChanged("SerialPort");
				this.OnPropertyChanged("MessageText");
			}
		}

		public override void OnShowPage()
		{
			this.SerialPort = this._wizard.Result.SerialPort;
		}

		protected override void NavigateToNextPage()
		{
			base.GoPage(PageType.GimxStage3);
		}

		protected override void NavigatePreviousPage()
		{
			base.GoPage(PageType.GimxStage1);
		}

		private string _serialPort;
	}
}
