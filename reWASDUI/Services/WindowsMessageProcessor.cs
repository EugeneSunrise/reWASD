using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using reWASDUI.DataModels;
using XBEliteWPF.Services.Interfaces;

namespace reWASDUI.Services
{
	public class WindowsMessageProcessor : IWindowsMessageProcessor
	{
		[DllImport("User32.dll")]
		public static extern uint RegisterWindowMessage(string str);

		~WindowsMessageProcessor()
		{
		}

		public void Attach(Window window, bool attachDriverEvents = true)
		{
			HwndSource hwndSource = HwndSource.FromHwnd(new WindowInteropHelper(window).Handle);
			if (hwndSource != null)
			{
				hwndSource.AddHook(new HwndSourceHook(this.WndProc));
			}
		}

		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if ((long)msg == (long)((ulong)this.CloseGUIMessage))
			{
				GameVM currentGame = App.GameProfilesService.CurrentGame;
				if (currentGame != null)
				{
					ConfigVM currentConfig = currentGame.CurrentConfig;
					if (currentConfig != null)
					{
						currentConfig.SaveConfigToJson();
					}
				}
				Application.Current.Shutdown();
			}
			return IntPtr.Zero;
		}

		private uint CloseGUIMessage = WindowsMessageProcessor.RegisterWindowMessage("DiscSoftReWASD_msg_uninstall");
	}
}
