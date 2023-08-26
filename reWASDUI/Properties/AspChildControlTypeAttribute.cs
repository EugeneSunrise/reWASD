using System;

namespace reWASDUI.Properties
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class AspChildControlTypeAttribute : Attribute
	{
		public AspChildControlTypeAttribute([NotNull] string tagName, [NotNull] Type controlType)
		{
			this.TagName = tagName;
			this.ControlType = controlType;
		}

		[NotNull]
		public string TagName { get; private set; }

		[NotNull]
		public Type ControlType { get; private set; }
	}
}
