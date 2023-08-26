using System;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class AddNintendoSwitchConsoleStepConfigRequiredVM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.AddNintendoSwitchConsoleStepConfigRequired;
			}
		}

		public AddNintendoSwitchConsoleStepConfigRequiredVM(WizardVM wizard)
			: base(wizard)
		{
		}

		protected override void NavigateToNextPage()
		{
			base.GoPage(PageType.AddNintendoSwitchConsoleStep2a);
		}

		protected override void NavigatePreviousPage()
		{
			base.GoPage(PageType.AddNintendoSwitchConsoleStep2a);
		}
	}
}
