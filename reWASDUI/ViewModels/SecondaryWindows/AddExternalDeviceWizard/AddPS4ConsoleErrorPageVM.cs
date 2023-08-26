using System;
using Prism.Commands;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class AddPS4ConsoleErrorPageVM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.AddPS4ConsoleErrorPage;
			}
		}

		public AddPS4ConsoleErrorPageVM(WizardVM wizard)
			: base(wizard)
		{
		}

		protected override void NavigatePreviousPage()
		{
			base.GoPage(PageType.AddPS4ConsoleStep2);
		}

		public DelegateCommand TryAgainCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._tryAgainCommand) == null)
				{
					delegateCommand = (this._tryAgainCommand = new DelegateCommand(delegate
					{
						base.GoPage(PageType.AddPS4ConsoleStep1);
					}));
				}
				return delegateCommand;
			}
		}

		private DelegateCommand _tryAgainCommand;
	}
}
