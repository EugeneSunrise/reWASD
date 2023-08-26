using System;

namespace reWASDUI.Properties
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class LocalizationRequiredAttribute : Attribute
	{
		public LocalizationRequiredAttribute()
			: this(true)
		{
		}

		public LocalizationRequiredAttribute(bool required)
		{
			this.Required = required;
		}

		public bool Required { get; private set; }
	}
}
