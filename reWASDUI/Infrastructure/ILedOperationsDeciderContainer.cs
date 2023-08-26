using System;
using reWASDUI.Utils;

namespace reWASDUI.Infrastructure
{
	public interface ILedOperationsDeciderContainer
	{
		LedOperationsDecider LedOperationsDecider { get; set; }
	}
}
