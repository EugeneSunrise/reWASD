using System;

namespace DTOverlay
{
	public struct HookInfo
	{
		public string libName;

		public string hookFunction;

		public uint threadID;

		public int hookID;
	}
}
