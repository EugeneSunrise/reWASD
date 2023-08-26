using System;
using Prism.Events;
using XBEliteWPF.Infrastructure.Controller;

namespace XBEliteWPF.Infrastructure
{
	public class GamepadInitializationChanged : PubSubEvent<BaseControllerVM>
	{
	}
}
