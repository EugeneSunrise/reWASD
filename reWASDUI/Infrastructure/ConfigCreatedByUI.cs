using System;
using Prism.Events;
using reWASDUI.DataModels;

namespace reWASDUI.Infrastructure
{
	public class ConfigCreatedByUI : PubSubEvent<ConfigVM>
	{
	}
}
