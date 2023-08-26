using System;
using DiscSoft.NET.Common.Utils;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;

namespace reWASDUI.ViewModels.InitializeGamepad
{
	public class InitializeGameSirG7VM : BaseInitializeGamepadVM
	{
		public bool IsDoNotRemind
		{
			get
			{
				return this._isDoNotRemind;
			}
			set
			{
				this.SetProperty<bool>(ref this._isDoNotRemind, value, "IsDoNotRemind");
			}
		}

		public InitializeGameSirG7VM(IContainerProvider uc)
			: base(uc)
		{
		}

		private void Submit(bool switchToG7)
		{
			if (this.IsDoNotRemind)
			{
				RegistryHelper.SetValue("Config", "ChangeGameSirG7Mode", switchToG7);
			}
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmGameSirG7Mode", !this.IsDoNotRemind);
			IEventAggregator eventAggregator = App.EventAggregator;
			if (eventAggregator != null)
			{
				eventAggregator.GetEvent<CurrentGamepadChanged>().Publish(base.GamepadService.CurrentGamepad);
			}
			ControllerVM controllerVM = base.GamepadService.CurrentGamepad as ControllerVM;
			if (controllerVM != null)
			{
				if (switchToG7)
				{
					controllerVM.SwitchControllerToHidModeCommand.Execute();
				}
				controllerVM.SetIsInitialized(true);
			}
			this.IsDoNotRemind = false;
		}

		protected override void Submit()
		{
			this.Submit(false);
		}

		protected void ChangeAndSubmit()
		{
			this.Submit(true);
		}

		public DelegateCommand ChangeAndSubmitCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._changeAndSubmitCommand) == null)
				{
					delegateCommand = (this._changeAndSubmitCommand = new DelegateCommand(new Action(this.ChangeAndSubmit)));
				}
				return delegateCommand;
			}
		}

		private bool _isDoNotRemind;

		private DelegateCommand _changeAndSubmitCommand;
	}
}
