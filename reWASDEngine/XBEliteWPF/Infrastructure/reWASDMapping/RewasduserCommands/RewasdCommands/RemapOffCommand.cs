using System;
using System.Runtime.Serialization;
using reWASDEngine;

namespace XBEliteWPF.Infrastructure.reWASDMapping.RewasduserCommands.RewasdCommands
{
	[Serializable]
	public class RemapOffCommand : BaseExecutableRewasdUserCommand
	{
		public RemapOffCommand(int id, string displayName, int displayNameStrId, string drawingResourcename)
			: base(id, displayName, displayNameStrId, drawingResourcename, 1)
		{
		}

		public override bool Execute(ulong profileID)
		{
			string text = "";
			if (profileID != 0UL)
			{
				REWASD_CONTROLLER_PROFILE_EX profileExByServiceProfileId = Engine.GamepadService.GetProfileExByServiceProfileId(profileID);
				if (profileExByServiceProfileId != null)
				{
					text = profileExByServiceProfileId.GetID(null);
				}
			}
			Engine.GamepadService.DisableRemap((!string.IsNullOrEmpty(text)) ? text : null, true);
			return true;
		}

		protected RemapOffCommand()
		{
		}

		protected RemapOffCommand(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
