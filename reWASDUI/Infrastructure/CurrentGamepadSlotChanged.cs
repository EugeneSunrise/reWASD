using System;
using Prism.Events;
using reWASDCommon.Infrastructure.Enums;

namespace reWASDUI.Infrastructure
{
	public class CurrentGamepadSlotChanged : PubSubEvent<Slot>
	{
	}
}
