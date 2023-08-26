using System;
using System.Windows.Input;
using Prism.Events;

namespace XBEliteWPF.Infrastructure
{
	public class KeyboardKeyPressed : PubSubEvent<Key>
	{
	}
}
