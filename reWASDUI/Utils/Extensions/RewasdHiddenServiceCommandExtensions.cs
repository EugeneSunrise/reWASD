using System;
using reWASDCommon.Infrastructure.Enums;

namespace reWASDUI.Utils.Extensions
{
	public static class RewasdHiddenServiceCommandExtensions
	{
		public static bool IsLEDReactionRequired(this RewasdHiddenServiceCommand command)
		{
			return (command >= 0 && command <= 21) || (command >= 30 && command <= 50) || (command >= 22 && command <= 25);
		}
	}
}
