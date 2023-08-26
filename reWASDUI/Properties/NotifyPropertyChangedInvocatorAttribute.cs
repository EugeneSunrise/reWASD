using System;

namespace reWASDUI.Properties
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class NotifyPropertyChangedInvocatorAttribute : Attribute
	{
		public NotifyPropertyChangedInvocatorAttribute()
		{
		}

		public NotifyPropertyChangedInvocatorAttribute([NotNull] string parameterName)
		{
			this.ParameterName = parameterName;
		}

		[CanBeNull]
		public string ParameterName { get; private set; }
	}
}
