using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace reWASDUI.Views.SecondaryWindows
{
	public static class ProcessExtensions
	{
		[DllImport("Kernel32.dll")]
		private static extern uint QueryFullProcessImageName([In] IntPtr hProcess, [In] uint dwFlags, [Out] StringBuilder lpExeName, [In] [Out] ref uint lpdwSize);

		public static string GetMainModuleFileName(this Process process, int buffer = 1024)
		{
			StringBuilder stringBuilder = new StringBuilder(buffer);
			uint num = (uint)(stringBuilder.Capacity + 1);
			if (ProcessExtensions.QueryFullProcessImageName(process.Handle, 0U, stringBuilder, ref num) == 0U)
			{
				return null;
			}
			return stringBuilder.ToString();
		}
	}
}
