using System;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using Prism.Events;
using Prism.Ioc;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.ViewModels.InitializeGamepad
{
	public class InitializeExclusiveDeviceVM : BaseInitializeGamepadVM
	{
		public string Header
		{
			get
			{
				return string.Format(DTLocalization.GetString(11895), ControllerTypeExtensions.GetDescription(this._controllerType));
			}
		}

		public string Description
		{
			get
			{
				return string.Format(DTLocalization.GetString(11896), ControllerTypeExtensions.GetDescription(this._controllerType));
			}
		}

		public bool IsAnyAzeron
		{
			get
			{
				return ControllerTypeExtensions.IsAnyAzeron(this._controllerType);
			}
		}

		public InitializeExclusiveDeviceVM(IContainerProvider uc)
			: base(uc)
		{
			this.UpdateProperies();
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

		public bool IsRighty
		{
			get
			{
				return this._isRighty;
			}
			set
			{
				this.SetProperty<bool>(ref this._isRighty, value, "IsRighty");
			}
		}

		public void UpdateProperies()
		{
			if (base.GamepadService.CurrentGamepad != null)
			{
				this._controllerType = base.GamepadService.CurrentGamepad.FirstControllerType;
				if (ControllerTypeExtensions.IsAnyAzeron(this._controllerType))
				{
					this.IsRighty = !base.GamepadService.CurrentGamepad.CurrentController.IsAzeronLefty;
				}
				this.OnPropertyChanged("Header");
				this.OnPropertyChanged("IsAnyAzeron");
				this.OnPropertyChanged("Description");
				this.OnPropertyChanged("IsRighty");
			}
		}

		private string GetStrOptionFromGamepadType(ControllerTypeEnum controller)
		{
			if (ControllerTypeExtensions.IsAnyAzeron(controller))
			{
				return "ConfirmAzeronExclusiveMode";
			}
			if (ControllerTypeExtensions.IsFlydigi(controller))
			{
				return "ConfirmFlydigiExclusiveMode";
			}
			return "ConfirmSteamExclusiveMode";
		}

		protected override void Submit()
		{
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, this.GetStrOptionFromGamepadType(this._controllerType), !this.IsDoNotRemind);
			if (ControllerTypeExtensions.IsAnyAzeron(this._controllerType))
			{
				bool isRighty = this.IsRighty;
				BaseControllerVM currentGamepad = base.GamepadService.CurrentGamepad;
				bool? flag;
				if (currentGamepad == null)
				{
					flag = null;
				}
				else
				{
					ControllerVM currentController = currentGamepad.CurrentController;
					flag = ((currentController != null) ? new bool?(currentController.IsAzeronLefty) : null);
				}
				bool? flag2 = flag;
				if ((isRighty == flag2.GetValueOrDefault()) & (flag2 != null))
				{
					base.GamepadService.CurrentGamepad.CurrentController.ToggleAzeronLefy();
				}
			}
			IEventAggregator eventAggregator = App.EventAggregator;
			if (eventAggregator != null)
			{
				eventAggregator.GetEvent<CurrentGamepadChanged>().Publish(base.GamepadService.CurrentGamepad);
			}
			ControllerVM controllerVM = base.GamepadService.CurrentGamepad as ControllerVM;
			if (controllerVM != null)
			{
				controllerVM.SetIsInitialized(true);
			}
			this.IsDoNotRemind = false;
		}

		private ControllerTypeEnum _controllerType = 16;

		private bool _isDoNotRemind;

		private bool _isRighty;
	}
}
