using System;

namespace reWASDUI.Properties
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Delegate)]
	public sealed class ItemNotNullAttribute : Attribute
	{
	}
}
