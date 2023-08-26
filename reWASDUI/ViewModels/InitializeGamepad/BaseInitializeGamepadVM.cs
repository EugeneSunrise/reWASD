using System;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.ViewModels.Base;

namespace reWASDUI.ViewModels.InitializeGamepad
{
	public class BaseInitializeGamepadVM : BaseServicesVM
	{
		public DelegateCommand SubmitCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._submit) == null)
				{
					delegateCommand = (this._submit = new DelegateCommand(new Action(this.Submit), new Func<bool>(this.SubmitCanExecute)));
				}
				return delegateCommand;
			}
		}

		private bool SubmitCanExecute()
		{
			return true;
		}

		public BaseInitializeGamepadVM(IContainerProvider uc)
			: base(uc)
		{
		}

		protected virtual async void Submit()
		{
			BaseControllerVM currentGamepad = base.GamepadService.CurrentGamepad;
			if (currentGamepad != null)
			{
				await base.HttpClientService.Gamepad.InitializeDevice(currentGamepad.ID, "");
				IEventAggregator eventAggregator = App.EventAggregator;
				if (eventAggregator != null)
				{
					eventAggregator.GetEvent<CurrentGamepadChanged>().Publish(base.GamepadService.CurrentGamepad);
				}
			}
		}

		private DelegateCommand _submit;
	}
}
