using System;
using System.Collections.Generic;
using Prism.Events;
using XBEliteWPF.Infrastructure;

namespace reWASDUI.Infrastructure
{
	public class GamepadKeyPressed : PubSubEvent<List<GamepadButtonDescription>>
	{
	}
}
