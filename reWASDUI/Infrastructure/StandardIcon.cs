using System;
using System.Collections.Generic;
using reWASDCommon.Infrastructure.Enums;

namespace reWASDUI.Infrastructure
{
	public class StandardIcon : RadialMenuIcon
	{
		public StandardIcon(string resource, List<RadialMenuIconCategory> categories, string description = null)
			: base(resource, categories, description, 0)
		{
		}

		public override RadialMenuIcon Clone()
		{
			return new StandardIcon(base.Resource, base.Categories, base.Description);
		}
	}
}
