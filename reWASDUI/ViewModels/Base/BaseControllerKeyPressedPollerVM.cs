using System;
using System.Collections.Generic;
using Prism.Ioc;
using Prism.Regions;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Services.Interfaces;

namespace reWASDUI.ViewModels.Base
{
	public abstract class BaseControllerKeyPressedPollerVM : BaseServicesVM, INavigationAware, IKeyPressedEventHandler
	{
		public BaseControllerKeyPressedPollerVM(IContainerProvider uc)
			: base(uc)
		{
			this.KeyPressedPollerService = IContainerProviderExtensions.Resolve<IKeyPressedPollerService>(uc);
		}

		public virtual void OnNavigatedTo(NavigationContext navigationContext)
		{
			this.KeyPressedPollerService.Subscribe(this, false);
		}

		public virtual bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return false;
		}

		public virtual void OnNavigatedFrom(NavigationContext navigationContext)
		{
			this.KeyPressedPollerService.Unsubscribe(this);
		}

		public abstract void OnControllerPollState(GamepadState gamepadState);

		public abstract void OnControllerKeyDown(List<GamepadButtonDescription> buttons);

		public abstract void OnControllerKeyUp(List<GamepadButtonDescription> buttons);

		public IKeyPressedPollerService KeyPressedPollerService;
	}
}
