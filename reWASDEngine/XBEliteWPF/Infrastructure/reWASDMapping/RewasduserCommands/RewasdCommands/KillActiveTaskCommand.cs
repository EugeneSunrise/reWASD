using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace XBEliteWPF.Infrastructure.reWASDMapping.RewasduserCommands.RewasdCommands
{
	[Serializable]
	public class KillActiveTaskCommand : BaseExecutableRewasdUserCommand
	{
		public KillActiveTaskCommand(int id, string displayName, int displayNameStrId, string drawingResourcename)
			: base(id, displayName, displayNameStrId, drawingResourcename, 8)
		{
		}

		public override bool Execute(ulong profileID)
		{
			Process processById = Process.GetProcessById(WindowFinder.GetWindowProcessId(IntPtr.Zero));
			if (processById.ProcessName == "searchui" || processById.ProcessName == "explorer")
			{
				return false;
			}
			if (!processById.CloseMainWindow())
			{
				processById.Kill();
			}
			return true;
		}

		protected KillActiveTaskCommand()
		{
		}

		protected KillActiveTaskCommand(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
