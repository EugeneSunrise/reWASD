using System;

namespace reWASDUI.Properties
{
	[AttributeUsage(AttributeTargets.Parameter)]
	public sealed class AspMvcAreaAttribute : Attribute
	{
		public AspMvcAreaAttribute()
		{
		}

		public AspMvcAreaAttribute([NotNull] string anonymousProperty)
		{
			this.AnonymousProperty = anonymousProperty;
		}

		[CanBeNull]
		public string AnonymousProperty { get; private set; }
	}
}
