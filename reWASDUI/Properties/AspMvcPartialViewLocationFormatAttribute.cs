using System;

namespace reWASDUI.Properties
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class AspMvcPartialViewLocationFormatAttribute : Attribute
	{
		public AspMvcPartialViewLocationFormatAttribute([NotNull] string format)
		{
			this.Format = format;
		}

		[NotNull]
		public string Format { get; private set; }
	}
}
