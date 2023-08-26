using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using DiscSoft.NET.Common.Utils;
using XBEliteWPF.Utils;
using XBEliteWPF.Utils.XBUtilModel;

namespace DTOverlay
{
	internal class Injector
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool GetExitCodeProcess(IntPtr hProcess, out uint ExitCode);

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, ref Injector.SECURITY_ATTRIBUTES lpProcessAttributes, ref Injector.SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, [In] ref Injector.STARTUPINFOEX lpStartupInfo, out Injector.PROCESS_INFORMATION lpProcessInformation);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool CloseHandle(IntPtr hObject);

		[DllImport("user32.dll")]
		private static extern bool PostThreadMessage(int idThread, uint Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("kernel32.dll")]
		private static extern int GetLastError();

		[DllImport("kernel32.dll")]
		public static extern bool IsWow64Process(IntPtr hProcess, out bool lpSystemInfo);

		public static bool InjectorRun(int id, string logAppName, bool free)
		{
			if (logAppName == null)
			{
				logAppName = "";
			}
			bool flag = true;
			string text = "x??";
			string text2 = "Not call inject";
			try
			{
				Injector.PROCESS_INFORMATION process_INFORMATION = default(Injector.PROCESS_INFORMATION);
				Tracer.TraceWrite("Inject: Starting inject in " + logAppName, false);
				string startupPath = Application.StartupPath;
				bool flag2 = false;
				Injector.IsWow64Process(Process.GetProcessById(id).Handle, out flag2);
				if (flag2)
				{
					text = "x32";
				}
				else
				{
					text = "x64";
				}
				string text3 = (flag2 ? "InGameOverlay32.dll" : "InGameOverlay64.dll");
				string text4 = (flag2 ? "StartDXOverlay32.exe" : "StartDXOverlay64.exe");
				string text5 = Application.StartupPath + "\\" + text3;
				string text6 = Application.StartupPath + "\\" + text4;
				if (!XBUtils.CheckCertDll(text3) || !XBUtils.CheckCertDll(text4))
				{
					text2 = "CheckCertDll error";
				}
				else
				{
					string text7 = string.Concat(new string[]
					{
						free ? "-free " : "",
						"-l \"",
						text5,
						"\" -p ",
						id.ToString(),
						" ",
						logAppName
					});
					text7 = text7.Replace("\\", "/");
					text2 = Injector.StartInject(ref process_INFORMATION, text7, text6);
					if (free)
					{
						Thread.Sleep(50);
						text2 = Injector.StartInject(ref process_INFORMATION, text7, text6);
					}
					Tracer.TraceWrite("Inject: StartInject in " + logAppName + "result: " + text2, false);
					if (text2 != "OK")
					{
						flag = false;
					}
				}
			}
			catch (Exception ex)
			{
				Tracer.TraceWrite("Inject: Catch exception:" + ex.Message, false);
				flag = false;
			}
			SenderGoogleAnalytics.SendMessageEvent("Inject", logAppName + " " + text, text2, -1L, false);
			Tracer.TraceWrite(string.Concat(new string[] { "StatInjectDX11 Inject: ", logAppName, " ", text, " ", text2 }), false);
			return flag;
		}

		private static void SendMessageEvent(string v1, object p1, string v2, object p2, string v3, object p3)
		{
			throw new NotImplementedException();
		}

		private static string StartInject(ref Injector.PROCESS_INFORMATION procInfo, string commandLine, string exe)
		{
			string text = "OK";
			Injector.STARTUPINFO startupinfo = default(Injector.STARTUPINFO);
			startupinfo.cb = Marshal.SizeOf<Injector.STARTUPINFO>(startupinfo);
			Injector.STARTUPINFOEX startupinfoex = default(Injector.STARTUPINFOEX);
			startupinfoex.StartupInfo.cb = Marshal.SizeOf<Injector.STARTUPINFOEX>(startupinfoex);
			Injector.SECURITY_ATTRIBUTES security_ATTRIBUTES = default(Injector.SECURITY_ATTRIBUTES);
			Injector.SECURITY_ATTRIBUTES security_ATTRIBUTES2 = default(Injector.SECURITY_ATTRIBUTES);
			security_ATTRIBUTES.nLength = Marshal.SizeOf<Injector.SECURITY_ATTRIBUTES>(security_ATTRIBUTES);
			security_ATTRIBUTES2.nLength = Marshal.SizeOf<Injector.SECURITY_ATTRIBUTES>(security_ATTRIBUTES2);
			Tracer.TraceWrite("StartInject: " + exe, false);
			if (Injector.CreateProcess(exe, commandLine, ref security_ATTRIBUTES, ref security_ATTRIBUTES2, false, 134742016U, IntPtr.Zero, null, ref startupinfoex, out procInfo))
			{
				new ProcessWaitHandle(procInfo.hProcess).WaitOne();
				uint num = 0U;
				if (Injector.GetExitCodeProcess(procInfo.hProcess, out num) && num != 0U)
				{
					switch (num)
					{
					case 1U:
						text = "Empty dll path";
						break;
					case 2U:
						text = "Unable to open process";
						break;
					case 3U:
						text = "Unable to allocate memory for dll";
						break;
					case 4U:
						text = "WriteProcessMemory failed in dll ";
						break;
					case 5U:
						text = "Free:Unable to open process";
						break;
					case 6U:
						text = "Free:Module not found";
						break;
					case 7U:
						text = "Free:ExecuteFreeLibrary error";
						break;
					}
				}
			}
			return text;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct STARTUPINFOEX
		{
			public Injector.STARTUPINFO StartupInfo;

			public IntPtr lpAttributeList;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct STARTUPINFO
		{
			public int cb;

			public string lpReserved;

			public string lpDesktop;

			public string lpTitle;

			public int dwX;

			public int dwY;

			public int dwXSize;

			public int dwYSize;

			public int dwXCountChars;

			public int dwYCountChars;

			public int dwFillAttribute;

			public int dwFlags;

			public short wShowWindow;

			public short cbReserved2;

			public IntPtr lpReserved2;

			public IntPtr hStdInput;

			public IntPtr hStdOutput;

			public IntPtr hStdError;
		}

		internal struct PROCESS_INFORMATION
		{
			public IntPtr hProcess;

			public IntPtr hThread;

			public int dwProcessId;

			public int dwThreadId;
		}

		public struct SECURITY_ATTRIBUTES
		{
			public int nLength;

			public IntPtr lpSecurityDescriptor;

			public int bInheritHandle;
		}
	}
}
