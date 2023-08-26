using System;
using Prism.Events;
using reWASDCommon.Network.HTTP.DataTransferObjects.Events.Desktop;

namespace XBEliteWPF.Infrastructure
{
	public class ConfigSaved : PubSubEvent<ConfigSavedEvent>
	{
	}
}
