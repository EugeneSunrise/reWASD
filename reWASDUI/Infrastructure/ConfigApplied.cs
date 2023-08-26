using System;
using Prism.Events;
using reWASDCommon.Network.HTTP.DataTransferObjects.Events.Desktop;

namespace reWASDUI.Infrastructure
{
	public class ConfigApplied : PubSubEvent<ConfigAppliedEvent>
	{
	}
}
