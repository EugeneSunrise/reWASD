using System;
using XBEliteWPF.Utils;

namespace XBEliteWPF.Infrastructure
{
	public interface ILedOperationsDeciderContainer
	{
		LedOperationsDecider LedOperationsDecider { get; set; }
	}
}
