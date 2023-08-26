using System;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.ViewModels.Base;

namespace reWASDUI.ViewModels
{
	public class InitializePeripheralVM : BaseServicesVM
	{
		public bool IsKeyboard
		{
			get
			{
				return this._isKeyboard;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isKeyboard, value, "IsKeyboard"))
				{
					this.SubmitCommand.RaiseCanExecuteChanged();
				}
			}
		}

		public bool IsMouse
		{
			get
			{
				return this._isMouse;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isMouse, value, "IsMouse"))
				{
					this.SubmitCommand.RaiseCanExecuteChanged();
				}
			}
		}

		public DelegateCommand SetIsKeyboardCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._setIsKeyboard) == null)
				{
					delegateCommand = (this._setIsKeyboard = new DelegateCommand(new Action(this.SetIsKeyboard)));
				}
				return delegateCommand;
			}
		}

		private void SetIsKeyboard()
		{
			this.IsKeyboard = true;
			this.IsMouse = false;
		}

		public DelegateCommand SetIsMouseCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._setIsMouse) == null)
				{
					delegateCommand = (this._setIsMouse = new DelegateCommand(new Action(this.SetIsMouse)));
				}
				return delegateCommand;
			}
		}

		private void SetIsMouse()
		{
			this.IsMouse = true;
			this.IsKeyboard = false;
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
			return this.IsMouse || this.IsKeyboard;
		}

		public InitializePeripheralVM(IContainerProvider uc)
			: base(uc)
		{
		}

		public DelegateCommand StartStopGamepadsDetectionModeCommand
		{
			get
			{
				return IContainerProviderExtensions.Resolve<GamepadSelectorVM>(App.Container).StartStopGamepadsDetectionModeCommand;
			}
		}

		private async void Submit()
		{
			PeripheralVM peripheralVM = base.GamepadService.CurrentGamepad as PeripheralVM;
			if (peripheralVM != null)
			{
				if (this.IsKeyboard)
				{
					peripheralVM.PeripheralPhysicalType = 1;
				}
				else if (this.IsMouse)
				{
					peripheralVM.PeripheralPhysicalType = 2;
				}
				await base.HttpClientService.Gamepad.InitializePeripheralDevice(peripheralVM.ID, peripheralVM.PeripheralPhysicalType);
				await base.GamepadService.BinDataSerialize.LoadPeripheralDevicesCollection();
				IEventAggregator eventAggregator = App.EventAggregator;
				if (eventAggregator != null)
				{
					eventAggregator.GetEvent<CurrentGamepadChanged>().Publish(base.GamepadService.CurrentGamepad);
				}
			}
		}

		private bool _isKeyboard;

		private bool _isMouse;

		private DelegateCommand _setIsKeyboard;

		private DelegateCommand _setIsMouse;

		private DelegateCommand _submit;
	}
}
