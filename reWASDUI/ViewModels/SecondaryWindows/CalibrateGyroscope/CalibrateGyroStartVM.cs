using System;

namespace reWASDUI.ViewModels.SecondaryWindows.CalibrateGyroscope
{
	public class CalibrateGyroStartVM : BaseGyroPageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.CalibrateGyroStart;
			}
		}

		public CalibrateGyroStartVM(GyroWizardVM wizard)
			: base(wizard)
		{
		}

		protected override void NavigateToNextPage()
		{
			base.GoPage(PageType.CalibrateGyroProcessing);
		}
	}
}
