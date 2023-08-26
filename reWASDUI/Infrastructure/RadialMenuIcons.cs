using System;
using System.Collections.Generic;
using reWASDCommon.Infrastructure.Enums;

namespace reWASDUI.Infrastructure
{
	public class RadialMenuIcons
	{
		public static List<StandardIcon> StandardIcons { get; } = new List<StandardIcon>
		{
			new StandardIcon("", new List<RadialMenuIconCategory> { 1 }, null),
			new StandardIcon("Acceleration", new List<RadialMenuIconCategory> { 2, 4, 3 }, null),
			new StandardIcon("Ammunition", new List<RadialMenuIconCategory> { 2 }, null),
			new StandardIcon("Bomb", new List<RadialMenuIconCategory> { 2 }, null),
			new StandardIcon("Bulletproof", new List<RadialMenuIconCategory> { 2 }, null),
			new StandardIcon("Cartridge", new List<RadialMenuIconCategory> { 2 }, null),
			new StandardIcon("Case", new List<RadialMenuIconCategory> { 4 }, null),
			new StandardIcon("Dynamite", new List<RadialMenuIconCategory> { 2 }, null),
			new StandardIcon("Firework", new List<RadialMenuIconCategory> { 3 }, null),
			new StandardIcon("FirstAidKit", new List<RadialMenuIconCategory> { 2, 4 }, null),
			new StandardIcon("Flashbang", new List<RadialMenuIconCategory> { 2 }, null),
			new StandardIcon("Gift", new List<RadialMenuIconCategory> { 3 }, null),
			new StandardIcon("Grenade", new List<RadialMenuIconCategory> { 2 }, null),
			new StandardIcon("Hardware", new List<RadialMenuIconCategory> { 2, 3 }, null),
			new StandardIcon("HEGrenade", new List<RadialMenuIconCategory> { 2 }, null),
			new StandardIcon("Knife", new List<RadialMenuIconCategory> { 2 }, null),
			new StandardIcon("Pistol", new List<RadialMenuIconCategory> { 2 }, null),
			new StandardIcon("Rocket", new List<RadialMenuIconCategory> { 2 }, null),
			new StandardIcon("Sniper", new List<RadialMenuIconCategory> { 2 }, null),
			new StandardIcon("TwoCartridges", new List<RadialMenuIconCategory> { 2 }, null)
		};

		public static List<RadialMenuIcon> GetAllIcons()
		{
			List<RadialMenuIcon> list = new List<RadialMenuIcon>();
			list.AddRange(RadialMenuIcons.StandardIcons);
			list.AddRange(RadialMenuIcons.UserIcons);
			return list;
		}

		public static List<UserIcon> UserIcons = new List<UserIcon>();
	}
}
