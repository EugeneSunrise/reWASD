using System;
using Prism.Events;
using reWASDUI.Infrastructure.Controller;

namespace reWASDUI.Infrastructure
{
	public class CurrentGamepadChanged : PubSubEvent<BaseControllerVM>
	{
	}
}
