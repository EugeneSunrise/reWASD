using System;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class AddNintendoSwitchConsoleStep2aVM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.AddNintendoSwitchConsoleStep2a;
			}
		}

		public AddNintendoSwitchConsoleStep2aVM(WizardVM wizard)
			: base(wizard)
		{
		}

		protected override void NavigatePreviousPage()
		{
			base.GoPage(PageType.AddNintendoSwitchConsoleStep1);
		}

		protected override void NavigateToNextPage()
		{
			base.GoPage(PageType.AddNintendoSwitchConsoleStep2);
		}
	}
}
