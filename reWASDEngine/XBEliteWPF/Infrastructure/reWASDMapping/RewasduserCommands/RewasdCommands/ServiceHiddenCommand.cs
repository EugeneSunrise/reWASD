using System;
using System.Runtime.Serialization;
using reWASDCommon.Infrastructure.Enums;
using reWASDEngine;

namespace XBEliteWPF.Infrastructure.reWASDMapping.RewasduserCommands.RewasdCommands
{
	[Serializable]
	public class ServiceHiddenCommand : BaseServiceHiddenCommand
	{
		public ServiceHiddenCommand(int id, RewasdHiddenServiceCommand rewasdHiddenServiceCommand)
			: base(id, rewasdHiddenServiceCommand, 9)
		{
		}

		public override bool Execute(ulong profileID)
		{
			Engine.EventProcessor.ProcessHiddenServiceCommand((ushort)profileID, base.RewasdHiddenServiceCommand);
			return true;
		}

		protected ServiceHiddenCommand()
		{
		}

		protected ServiceHiddenCommand(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
