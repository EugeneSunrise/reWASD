using System;
using System.Runtime.InteropServices;

namespace DTOverlay
{
	internal class Hook
	{
		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr SetWindowsHookEx(int idHook, IntPtr lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool UnhookWindowsHookEx(IntPtr hook);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr LoadLibrary(string lpFileName);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool FreeLibrary(IntPtr hModule);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr GetProcAddress(IntPtr hModule, int procName);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

		~Hook()
		{
			this.Deactivate();
		}

		public bool Activate(HookInfo info)
		{
			if (this._hook != IntPtr.Zero)
			{
				return true;
			}
			IntPtr intPtr = Hook.LoadLibrary(info.libName);
			if (intPtr == IntPtr.Zero)
			{
				return false;
			}
			IntPtr procAddress = Hook.GetProcAddress(intPtr, info.hookFunction);
			if (procAddress == IntPtr.Zero)
			{
				return false;
			}
			this._hook = Hook.SetWindowsHookEx(info.hookID, procAddress, intPtr, info.threadID);
			return this._hook != IntPtr.Zero;
		}

		public void Deactivate()
		{
			if (this._hook != IntPtr.Zero)
			{
				Hook.UnhookWindowsHookEx(this._hook);
				this._hook = IntPtr.Zero;
			}
		}

		private IntPtr _hook = IntPtr.Zero;
	}
}
