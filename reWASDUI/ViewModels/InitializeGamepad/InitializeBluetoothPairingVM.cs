using System;
using DiscSoft.NET.Common.Localization;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using reWASDCommon.Utils;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;

namespace reWASDUI.ViewModels.InitializeGamepad
{
	public class InitializeBluetoothPairingVM : BaseInitializeGamepadVM
	{
		public InitializeBluetoothPairingVM(IContainerProvider uc)
			: base(uc)
		{
			IEventAggregator eventAggregator = App.EventAggregator;
			if (eventAggregator == null)
			{
				return;
			}
			eventAggregator.GetEvent<CurrentGamepadChanged>().Subscribe(delegate(BaseControllerVM gamepad)
			{
				this.UpdateProperies();
			});
		}

		public string Description
		{
			get
			{
				if (base.GamepadService.CurrentGamepad == null)
				{
					return "";
				}
				return string.Format(DTLocalization.GetString(11867), base.GamepadService.CurrentGamepad.ControllerTypeFriendlyName);
			}
		}

		public void UpdateProperies()
		{
			this.OnPropertyChanged("Description");
		}

		public DelegateCommand PairCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._Pair) == null)
				{
					delegateCommand = (this._Pair = new DelegateCommand(new Action(this.Pair), new Func<bool>(this.PairCanExecute)));
				}
				return delegateCommand;
			}
		}

		private bool PairCanExecute()
		{
			return true;
		}

		private async void Pair()
		{
			ulong num;
			BluetoothUtils.BluetoothGetLocalRadioAddress(ref num);
			ControllerVM controllerVM = (ControllerVM)base.GamepadService.CurrentGamepad;
			await App.HttpClientService.Gamepad.ControllerChangeMasterAddress(controllerVM.ControllerId, controllerVM.Type, num);
		}

		private DelegateCommand _Pair;
	}
}
