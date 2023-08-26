using System;
using Prism.Events;
using reWASDCommon.Network.HTTP.DataTransferObjects;

namespace XBEliteWPF.Infrastructure
{
	public class GameRenamed : PubSubEvent<RenameGameParams>
	{
	}
}
