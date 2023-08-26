using System;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace XBEliteWPF.Infrastructure.reWASDMapping.RewasduserCommands.RewasdCommands
{
	[Serializable]
	public class SendComputerToSleepCommand : BaseExecutableRewasdUserCommand
	{
		public SendComputerToSleepCommand(int id, string displayName, int displayNameStrId, string drawingResourcename)
			: base(id, displayName, displayNameStrId, drawingResourcename, 4)
		{
		}

		public override bool Execute(ulong profileID)
		{
			Application.SetSuspendState(PowerState.Suspend, false, false);
			return true;
		}

		protected SendComputerToSleepCommand()
		{
		}

		protected SendComputerToSleepCommand(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
