using System;

namespace reWASDUI.Properties
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class AspTypePropertyAttribute : Attribute
	{
		public bool CreateConstructorReferences { get; private set; }

		public AspTypePropertyAttribute(bool createConstructorReferences)
		{
			this.CreateConstructorReferences = createConstructorReferences;
		}
	}
}
