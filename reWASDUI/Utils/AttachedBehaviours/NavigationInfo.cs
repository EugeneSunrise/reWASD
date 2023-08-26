using System;
using Prism.Regions;

namespace reWASDUI.Utils.AttachedBehaviours
{
	public class NavigationInfo
	{
		public string RegionName { get; set; }

		public string ViewName { get; set; }

		public NavigationParameters NavParams { get; set; }
	}
}
