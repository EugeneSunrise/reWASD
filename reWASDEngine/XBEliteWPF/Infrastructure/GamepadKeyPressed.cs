using System;
using System.Collections.Generic;
using Prism.Events;

namespace XBEliteWPF.Infrastructure
{
	public class GamepadKeyPressed : PubSubEvent<List<GamepadButtonDescription>>
	{
	}
}
