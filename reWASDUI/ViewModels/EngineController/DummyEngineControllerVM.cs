using System;
using System.Linq;
using DiscSoft.NET.Common.Utils;
using Prism.Commands;
using Prism.Ioc;
using reWASDUI.Infrastructure.Controller;

namespace reWASDUI.ViewModels.EngineController
{
	public class DummyEngineControllerVM : BaseEngineControllerVM
	{
		public DummyEngineControllerVM(IContainerProvider uc)
			: base(uc)
		{
		}

		protected override void PreferenceChanged()
		{
			base.PreferenceChanged();
			if (!this.GetFlagState)
			{
				this.Submit();
			}
		}

		protected override void ActivateMobileFeature()
		{
			base.ActivateMobileFeature();
			if (base.IsLicenseNotActivated || (base.IsLicenseTrial && !base.IsFeatureHasTrialDays))
			{
				this.Submit();
			}
		}

		public bool GetFlagState
		{
			get
			{
				return RegistryHelper.GetBool(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmEngineController", true);
			}
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

		public DelegateCommand SubmitCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._submit) == null)
				{
					delegateCommand = (this._submit = new DelegateCommand(new Action(this.Submit)));
				}
				return delegateCommand;
			}
		}

		private void Submit()
		{
			if (this.GetFlagState)
			{
				RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmEngineController", !this.IsDoNotRemind);
				this.IsDoNotRemind = false;
			}
			else
			{
				this.IsDoNotRemind = true;
			}
			BaseControllerVM baseControllerVM = App.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM item) => item is PromoDeviceVM);
			if (baseControllerVM != null)
			{
				App.GamepadService.GamepadCollection.Remove(baseControllerVM);
				if (App.GamepadService.CurrentGamepad == null)
				{
					App.GamepadService.SelectDefaultGamepad();
				}
			}
		}

		private bool _isDoNotRemind;

		private DelegateCommand _submit;
	}
}
