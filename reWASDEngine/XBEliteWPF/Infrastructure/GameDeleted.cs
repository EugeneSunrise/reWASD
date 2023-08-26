using System;
using Prism.Events;

namespace XBEliteWPF.Infrastructure
{
	public class GameDeleted : PubSubEvent<Tuple<string, string>>
	{
	}
}
