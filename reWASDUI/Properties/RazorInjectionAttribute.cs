using System;

namespace reWASDUI.Properties
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class RazorInjectionAttribute : Attribute
	{
		public RazorInjectionAttribute([NotNull] string type, [NotNull] string fieldName)
		{
			this.Type = type;
			this.FieldName = fieldName;
		}

		[NotNull]
		public string Type { get; private set; }

		[NotNull]
		public string FieldName { get; private set; }
	}
}
