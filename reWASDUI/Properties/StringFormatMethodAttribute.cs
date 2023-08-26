using System;

namespace reWASDUI.Properties
{
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Delegate)]
	public sealed class StringFormatMethodAttribute : Attribute
	{
		public StringFormatMethodAttribute([NotNull] string formatParameterName)
		{
			this.FormatParameterName = formatParameterName;
		}

		[NotNull]
		public string FormatParameterName { get; private set; }
	}
}
