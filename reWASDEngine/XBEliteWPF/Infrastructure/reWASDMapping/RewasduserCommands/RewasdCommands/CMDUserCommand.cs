using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace XBEliteWPF.Infrastructure.reWASDMapping.RewasduserCommands.RewasdCommands
{
	[Serializable]
	public class CMDUserCommand : BaseExecutableRewasdUserCommand
	{
		private string Arguments { get; set; }

		public CMDUserCommand(int id, string displayName, int displayNameStrId, string drawingResourcename, string arguments)
			: base(id, displayName, displayNameStrId, drawingResourcename, 3)
		{
			this.Arguments = arguments;
		}

		public override bool Execute(ulong profileID)
		{
			try
			{
				new Process
				{
					StartInfo = new ProcessStartInfo
					{
						WindowStyle = ProcessWindowStyle.Hidden,
						FileName = "cmd.exe",
						Arguments = "/c " + this.Arguments,
						UseShellExecute = false,
						CreateNoWindow = true
					}
				}.Start();
			}
			catch (Exception)
			{
			}
			return true;
		}

		protected CMDUserCommand()
		{
		}

		protected CMDUserCommand(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
