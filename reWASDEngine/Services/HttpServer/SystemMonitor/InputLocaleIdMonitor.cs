using System;
using System.Runtime.InteropServices;
using System.Text;

namespace reWASDEngine.Services.HttpServer.SystemMonitor
{
	public class InputLocaleIdMonitor : AsyncLoopMonitor
	{
		protected override int DELAY
		{
			get
			{
				return 100;
			}
		}

		[DllImport("user32.dll")]
		private static extern IntPtr GetKeyboardLayout(uint idThread);

		[DllImport("user32.dll")]
		private static extern long GetKeyboardLayoutName(StringBuilder pwszKLID);

		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr processId);

		public event InputLocaleIdChanged OnInputLocaleIdChanged;

		protected override void Check()
		{
			try
			{
				IntPtr keyboardLayout = InputLocaleIdMonitor.GetKeyboardLayout(InputLocaleIdMonitor.GetWindowThreadProcessId(InputLocaleIdMonitor.GetForegroundWindow(), IntPtr.Zero));
				if (keyboardLayout != IntPtr.Zero)
				{
					if (this.prevLid != keyboardLayout)
					{
						this.prevLid = keyboardLayout;
						Convert.ToString((long)((ulong)((uint)keyboardLayout)), 2);
						InputLocaleIdChanged onInputLocaleIdChanged = this.OnInputLocaleIdChanged;
						if (onInputLocaleIdChanged != null)
						{
							onInputLocaleIdChanged((uint)keyboardLayout);
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private IntPtr prevLid = IntPtr.Zero;
	}
}
