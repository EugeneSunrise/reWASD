using System;
using Prism.Events;
using reWASDUI.DataModels;

namespace reWASDUI.Infrastructure
{
	public class CurrentConfigChanged : PubSubEvent<ConfigVM>
	{
	}
}
