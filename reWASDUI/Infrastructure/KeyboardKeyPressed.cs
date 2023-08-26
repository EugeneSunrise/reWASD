using System;
using System.Windows.Input;
using Prism.Events;

namespace reWASDUI.Infrastructure
{
	public class KeyboardKeyPressed : PubSubEvent<Key>
	{
	}
}
