using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace reWASDUI.ViewModels.CommunityConfigs
{
	[JsonConverter(typeof(SupportedGamepadsConverter))]
	public class SupportedGamepads
	{
		public List<Gamepad> Gamepads { get; set; } = new List<Gamepad>();
	}
}
