using System;
using DiscSoft.NET.Common.Localization;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class GimxFailsStageVM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.GimxFailsStage;
			}
		}

		public string GimxFailsMessage
		{
			get
			{
				return DTLocalization.GetString(12080);
			}
		}

		public GimxFailsStageVM(WizardVM wizard)
			: base(wizard)
		{
		}

		protected override void NavigatePreviousPage()
		{
			base.GoPage(PageType.GimxStage3);
		}

		protected override void NavigateToNextPage()
		{
			base.GoPage(PageType.GimxStage1);
		}
	}
}
