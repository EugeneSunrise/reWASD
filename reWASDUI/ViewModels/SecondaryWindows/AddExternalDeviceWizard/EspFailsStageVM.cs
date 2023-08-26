using System;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class EspFailsStageVM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.EspFailsStage;
			}
		}

		public string EspFailsMessage
		{
			get
			{
				return this._espFailsMessage;
			}
			set
			{
				if (this._espFailsMessage == value)
				{
					return;
				}
				this._espFailsMessage = value;
				this.OnPropertyChanged("EspFailsMessage");
			}
		}

		public string EspFailsHeader
		{
			get
			{
				return this._espFailsHeader;
			}
			set
			{
				if (this._espFailsHeader == value)
				{
					return;
				}
				this._espFailsHeader = value;
				this.OnPropertyChanged("EspFailsHeader");
			}
		}

		public PageType PreviousPage { get; set; } = PageType.None;

		public EspFailsStageVM(WizardVM wizard)
			: base(wizard)
		{
		}

		protected override void NavigatePreviousPage()
		{
			base.GoPage(PageType.EspStage3);
		}

		protected override void NavigateToNextPage()
		{
			if (this.PreviousPage == PageType.None)
			{
				this._wizard.OnOk();
				return;
			}
			base.GoPage(this.PreviousPage);
		}

		private string _espFailsMessage;

		private string _espFailsHeader;
	}
}
