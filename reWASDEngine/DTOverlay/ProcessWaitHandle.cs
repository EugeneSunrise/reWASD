using System;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace DTOverlay
{
	internal class ProcessWaitHandle : WaitHandle
	{
		public ProcessWaitHandle(IntPtr processHandle)
		{
			base.SafeWaitHandle = new SafeWaitHandle(processHandle, false);
		}
	}
}
