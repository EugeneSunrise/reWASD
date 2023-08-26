using System;

namespace reWASDUI.Properties
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
	public sealed class HtmlAttributeValueAttribute : Attribute
	{
		public HtmlAttributeValueAttribute([NotNull] string name)
		{
			this.Name = name;
		}

		[NotNull]
		public string Name { get; private set; }
	}
}
