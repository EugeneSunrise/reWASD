using System;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class AddPS4ConsoleStep4VM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.AddPS4ConsoleStep4;
			}
		}

		public AddPS4ConsoleStep4VM(WizardVM wizard)
			: base(wizard)
		{
		}

		protected override void NavigateToNextPage()
		{
			this._wizard.OnOk();
		}
	}
}
