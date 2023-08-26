using System;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace XBEliteWPF.Infrastructure.reWASDMapping.RewasduserCommands.RewasdCommands
{
	[Serializable]
	public class SendComputerToHibernationCommand : BaseExecutableRewasdUserCommand
	{
		public SendComputerToHibernationCommand(int id, string displayName, int displayNameStrId, string drawingResourcename)
			: base(id, displayName, displayNameStrId, drawingResourcename, 5)
		{
		}

		public override bool Execute(ulong profileID)
		{
			Application.SetSuspendState(PowerState.Hibernate, false, false);
			return true;
		}

		protected SendComputerToHibernationCommand()
		{
		}

		protected SendComputerToHibernationCommand(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
