using System;
using System.Runtime.Serialization;

namespace reWASDUI.ViewModels.CommunityConfigs
{
	[DataContract]
	public class Gamepads
	{
		[DataMember(Name = "perfect")]
		public SupportedGamepads Perfect { get; set; }

		[DataMember(Name = "other")]
		public SupportedGamepads Others { get; set; }
	}
}
