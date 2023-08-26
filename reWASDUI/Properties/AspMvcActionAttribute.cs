using System;

namespace reWASDUI.Properties
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
	public sealed class AspMvcActionAttribute : Attribute
	{
		public AspMvcActionAttribute()
		{
		}

		public AspMvcActionAttribute([NotNull] string anonymousProperty)
		{
			this.AnonymousProperty = anonymousProperty;
		}

		[CanBeNull]
		public string AnonymousProperty { get; private set; }
	}
}
