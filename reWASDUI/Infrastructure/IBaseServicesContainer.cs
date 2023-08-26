using System;
using Prism.Events;
using reWASDUI.Services;

namespace reWASDUI.Infrastructure
{
	public interface IBaseServicesContainer
	{
		GameProfilesService GameProfilesService { get; }

		GamepadService GamepadService { get; }

		GuiHelperService GuiHelperService { get; }

		KeyBindingService KeyBindingService { get; }

		LicensingService LicensingService { get; }

		EventAggregator EventAggregator { get; }
	}
}
