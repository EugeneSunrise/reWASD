using System;

namespace XBEliteWPF.Services
{
	public class WinEventHookException : Exception
	{
		public WinEventHookException()
		{
		}

		public WinEventHookException(string message)
			: base(message)
		{
		}
	}
}
