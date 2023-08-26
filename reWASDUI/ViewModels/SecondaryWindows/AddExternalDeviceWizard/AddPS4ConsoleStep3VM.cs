using System;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class AddPS4ConsoleStep3VM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.AddPS4ConsoleStep3;
			}
		}

		public AddPS4ConsoleStep3VM(WizardVM wizard)
			: base(wizard)
		{
		}

		protected override void NavigateToNextPage()
		{
			base.GoPage(PageType.AddPS4ConsoleStep4);
		}

		protected override void NavigatePreviousPage()
		{
			base.GoPage(PageType.AddPS4ConsoleStep2);
		}
	}
}
