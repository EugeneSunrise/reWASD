using System;

namespace reWASDUI.Properties
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class AspMvcMasterLocationFormatAttribute : Attribute
	{
		public AspMvcMasterLocationFormatAttribute(string format)
		{
			this.Format = format;
		}

		public string Format { get; private set; }
	}
}
