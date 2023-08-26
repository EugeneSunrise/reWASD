using System;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class AddPS4ConsoleStep1VM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.AddPS4ConsoleStep1;
			}
		}

		public AddPS4ConsoleStep1VM(WizardVM wizard)
			: base(wizard)
		{
		}

		protected override void NavigatePreviousPage()
		{
			base.GoPage(PageType.AddExternalClient);
		}

		protected override void NavigateToNextPage()
		{
			base.GoPage(PageType.AddPS4ConsoleStep2);
		}
	}
}
