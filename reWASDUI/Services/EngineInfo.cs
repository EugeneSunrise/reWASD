using System;

namespace reWASDUI.Services
{
	public class EngineInfo
	{
		public string Name { get; set; }

		public string Address { get; set; }

		public int Port { get; set; }

		public override string ToString()
		{
			return this.Name;
		}
	}
}
