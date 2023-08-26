using System;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class AddNintendoSwitchConsoleStep1VM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.AddNintendoSwitchConsoleStep1;
			}
		}

		public AddNintendoSwitchConsoleStep1VM(WizardVM wizard)
			: base(wizard)
		{
		}

		protected override void NavigatePreviousPage()
		{
			base.GoPage(PageType.AddExternalClient);
		}

		protected override void NavigateToNextPage()
		{
			base.GoPage(PageType.AddNintendoSwitchConsoleStep2);
		}
	}
}
