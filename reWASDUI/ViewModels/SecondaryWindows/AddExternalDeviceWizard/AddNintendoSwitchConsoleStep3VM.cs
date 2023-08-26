using System;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class AddNintendoSwitchConsoleStep3VM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.AddNintendoSwitchConsoleStep3;
			}
		}

		public AddNintendoSwitchConsoleStep3VM(WizardVM wizard)
			: base(wizard)
		{
		}

		protected override void NavigateToNextPage()
		{
			this._wizard.OnOk();
		}
	}
}
