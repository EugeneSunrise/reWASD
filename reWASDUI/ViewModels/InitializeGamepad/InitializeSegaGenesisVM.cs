using System;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.ViewModels.Base;

namespace reWASDUI.ViewModels.InitializeGamepad
{
	public class InitializeSegaGenesisVM : BaseServicesVM
	{
		public bool IsJapanese
		{
			get
			{
				return this._isJapanese;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isJapanese, value, "IsJapanese"))
				{
					this.SubmitCommand.RaiseCanExecuteChanged();
				}
			}
		}

		public bool IsInternational
		{
			get
			{
				return this._isInternational;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isInternational, value, "IsInternational"))
				{
					this.SubmitCommand.RaiseCanExecuteChanged();
				}
			}
		}

		public DelegateCommand SetIsJapaneseCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._setIsJapanese) == null)
				{
					delegateCommand = (this._setIsJapanese = new DelegateCommand(new Action(this.SetIsJapanese)));
				}
				return delegateCommand;
			}
		}

		private void SetIsJapanese()
		{
			this.IsJapanese = true;
			this.IsInternational = false;
		}

		public DelegateCommand SetIsInternationalCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._setIsInternational) == null)
				{
					delegateCommand = (this._setIsInternational = new DelegateCommand(new Action(this.SetIsInternational)));
				}
				return delegateCommand;
			}
		}

		private void SetIsInternational()
		{
			this.IsInternational = true;
			this.IsJapanese = false;
		}

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
			return this.IsInternational || this.IsJapanese;
		}

		public InitializeSegaGenesisVM(IContainerProvider uc)
			: base(uc)
		{
		}

		private async void Submit()
		{
			BaseControllerVM currentGamepad = base.GamepadService.CurrentGamepad;
			if (currentGamepad != null)
			{
				await base.HttpClientService.Gamepad.InitializeDevice(currentGamepad.ID, this.IsJapanese ? "Japanese" : "International");
				IEventAggregator eventAggregator = App.EventAggregator;
				if (eventAggregator != null)
				{
					eventAggregator.GetEvent<CurrentGamepadChanged>().Publish(base.GamepadService.CurrentGamepad);
				}
			}
		}

		private bool _isJapanese;

		private bool _isInternational;

		private DelegateCommand _setIsJapanese;

		private DelegateCommand _setIsInternational;

		private DelegateCommand _submit;
	}
}
