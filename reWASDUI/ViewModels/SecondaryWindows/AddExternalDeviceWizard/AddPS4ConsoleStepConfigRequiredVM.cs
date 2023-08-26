using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using reWASDUI.Infrastructure.Controller;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class AddPS4ConsoleStepConfigRequiredVM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.AddPS4ConsoleStepConfigRequired;
			}
		}

		public override void OnShowPage()
		{
			this._DS4ControllerVM = (this._wizard.FindPage(PageType.AddPS4ConsoleStep2) as AddPS4ConsoleStep2VM).DS4ControllerVM;
			this.StartPoller();
		}

		public override void OnHidePage()
		{
			this.StopPoller();
		}

		public bool IsSaveEnabled
		{
			get
			{
				return this._isSaveEnabled;
			}
			set
			{
				this.SetProperty(ref this._isSaveEnabled, value, "IsSaveEnabled");
			}
		}

		public AddPS4ConsoleStepConfigRequiredVM(WizardVM wizard)
			: base(wizard)
		{
		}

		protected override async void NavigateToNextPage()
		{
			await App.GameProfilesService.ApplyCurrentProfile(true);
			base.GoPage(PageType.AddPS4ConsoleStep3);
		}

		protected override void NavigatePreviousPage()
		{
			base.GoPage(PageType.AddPS4ConsoleStep2);
		}

		private void StartPoller()
		{
			this._pollingTimer = new DispatcherTimer();
			this._pollingTimer.Tick += delegate([Nullable(2)] object o, EventArgs e)
			{
				this.CheckDS4Exist();
			};
			this._pollingTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
			this._pollingTimer.Start();
		}

		private void CheckDS4Exist()
		{
			this.IsSaveEnabled = this._DS4ControllerVM != null && !string.IsNullOrEmpty(this._DS4ControllerVM.ID) && App.GamepadService.GamepadCollection.FirstOrDefault(delegate(BaseControllerVM item)
			{
				if (!(item.ID == this._DS4ControllerVM.ID))
				{
					CompositeControllerVM compositeControllerVM = item as CompositeControllerVM;
					return compositeControllerVM != null && compositeControllerVM.BaseControllers.Any((BaseControllerVM comItem) => comItem != null && comItem.ID == this._DS4ControllerVM.ID);
				}
				return true;
			}) != null;
		}

		private void StopPoller()
		{
			DispatcherTimer pollingTimer = this._pollingTimer;
			if (pollingTimer != null)
			{
				pollingTimer.Stop();
			}
			this._pollingTimer = null;
		}

		public bool _isSaveEnabled;

		private ControllerVM _DS4ControllerVM;

		private DispatcherTimer _pollingTimer;
	}
}
