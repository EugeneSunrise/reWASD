using System;

namespace reWASDUI.Properties
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class RazorImportNamespaceAttribute : Attribute
	{
		public RazorImportNamespaceAttribute([NotNull] string name)
		{
			this.Name = name;
		}

		[NotNull]
		public string Name { get; private set; }
	}
}
