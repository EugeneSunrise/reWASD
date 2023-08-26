using System;

namespace reWASDUI.Properties
{
	[AttributeUsage(AttributeTargets.Parameter)]
	public sealed class AssertionConditionAttribute : Attribute
	{
		public AssertionConditionAttribute(AssertionConditionType conditionType)
		{
			this.ConditionType = conditionType;
		}

		public AssertionConditionType ConditionType { get; private set; }
	}
}
