using System;
using DiscSoft.NET.Common.Localization;
using Prism.Ioc;
using reWASDUI.Infrastructure.Controller;

namespace reWASDUI.ViewModels.InitializeGamepad
{
	public class Initialize8BitDoVM : BaseInitializeGamepadVM
	{
		public string Header
		{
			get
			{
				return DTLocalization.GetString(12147);
			}
		}

		public string Description
		{
			get
			{
				return DTLocalization.GetString(12148);
			}
		}

		public Initialize8BitDoVM(IContainerProvider uc)
			: base(uc)
		{
		}

		protected override void Submit()
		{
			ControllerVM controllerVM = base.GamepadService.CurrentGamepad as ControllerVM;
			if (controllerVM != null)
			{
				controllerVM.SetIsInitialized(true);
			}
		}
	}
}
