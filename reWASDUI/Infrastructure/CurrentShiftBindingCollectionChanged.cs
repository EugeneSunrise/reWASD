using System;
using Prism.Events;
using reWASDUI.Infrastructure.KeyBindings;

namespace reWASDUI.Infrastructure
{
	public class CurrentShiftBindingCollectionChanged : PubSubEvent<ShiftXBBindingCollection>
	{
	}
}
