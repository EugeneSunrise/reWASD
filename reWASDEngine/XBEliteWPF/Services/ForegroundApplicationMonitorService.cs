using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using DiscSoft.NET.Common.Utils;
using XBEliteWPF.Services.Interfaces;

namespace XBEliteWPF.Services
{
	public class ForegroundApplicationMonitorService : IForegroundApplicationMonitorService
	{
		[DllImport("user32.dll")]
		private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, ForegroundApplicationMonitorService.WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

		[DllImport("user32.dll")]
		private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int ProcessId);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool EnumChildWindows(IntPtr hwnd, ForegroundApplicationMonitorService.WindowEnumProc callback, IntPtr lParam);

		private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
		{
			if (eventType == 3U || eventType == 8U || eventType == 23U)
			{
				this.update();
			}
		}

		private static int GetWindowProcessId(IntPtr hwnd)
		{
			int num;
			ForegroundApplicationMonitorService.GetWindowThreadProcessId(hwnd, out num);
			return num;
		}

		private Process GetRealProcess(Process foregroundProcess)
		{
			ForegroundApplicationMonitorService.EnumChildWindows(foregroundProcess.MainWindowHandle, new ForegroundApplicationMonitorService.WindowEnumProc(this.ChildWindowCallback), IntPtr.Zero);
			return this._realProcess;
		}

		private bool ChildWindowCallback(IntPtr hwnd, IntPtr lparam)
		{
			Process processById = Process.GetProcessById(ForegroundApplicationMonitorService.GetWindowProcessId(hwnd));
			if (processById.ProcessName != "ApplicationFrameHost")
			{
				this._realProcess = processById;
			}
			return true;
		}

		private void update()
		{
			this.processId = 0;
			this.threadId = 0;
			try
			{
				IntPtr intPtr = IntPtr.Zero;
				intPtr = ForegroundApplicationMonitorService.GetForegroundWindow();
				this.threadId = ForegroundApplicationMonitorService.GetWindowThreadProcessId(intPtr, out this.processId);
				this.process = Process.GetProcessById(this.processId);
				if (this.process.ProcessName == "ApplicationFrameHost")
				{
					this.process = this.GetRealProcess(this.process);
				}
			}
			catch (Exception ex)
			{
				Tracer.TraceException(ex, "update");
			}
			if (this.EnableRaisingEvents)
			{
				EventHandler changed = this.Changed;
				if (changed == null)
				{
					return;
				}
				changed(this, new EventArgs());
			}
		}

		private void hook()
		{
			if (this.dele != null)
			{
				return;
			}
			this.dele = new ForegroundApplicationMonitorService.WinEventDelegate(this.WinEventProc);
			this.m_hhook = ForegroundApplicationMonitorService.SetWinEventHook(3U, 23U, IntPtr.Zero, this.dele, 0U, 0U, 0U);
			if (this.m_hhook == IntPtr.Zero)
			{
				throw new WinEventHookException("Error when setting Windows event hook.");
			}
		}

		private void unhook()
		{
			if (this.dele == null)
			{
				return;
			}
			ForegroundApplicationMonitorService.UnhookWinEvent(this.m_hhook);
			this.dele = null;
		}

		[Category("Read-Only Properties")]
		[DefaultValue(0)]
		[Description("The ID of the process associated with the currently active window.")]
		public int ProcessId
		{
			get
			{
				return this.processId;
			}
		}

		[Category("Read-Only Properties")]
		[DefaultValue(0)]
		[Description("The ID of the thread associated with the currently active window.")]
		public int ThreadId
		{
			get
			{
				return this.threadId;
			}
		}

		[Category("Read-Only Properties")]
		[DefaultValue(null)]
		[Description("An object representing the process associated with the currently active window.")]
		public Process Process
		{
			get
			{
				return this.process;
			}
		}

		[Category("Read-Only Properties")]
		[DefaultValue(null)]
		[Description("The title of currently active window.")]
		public string WindowTitle
		{
			get
			{
				if (this.process == null)
				{
					return "";
				}
				return this.process.MainWindowTitle;
			}
		}

		[Category("Read-Only Properties")]
		[DefaultValue(null)]
		[Description("The name of the module (executable) associated with the currenctly active window.")]
		public string ModuleName
		{
			get
			{
				if (this.process == null)
				{
					return "";
				}
				return this.process.MainModule.ModuleName;
			}
		}

		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Indicates whether the component is enabled or not.")]
		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				if (this.enabled && !value)
				{
					this.Disable();
				}
				if (!this.enabled && value)
				{
					this.Enable();
				}
			}
		}

		[Category("Misc")]
		[DefaultValue(true)]
		[Description("Indicates whether or not the component will trigger events.")]
		public bool EnableRaisingEvents { get; set; }

		[Description("Occurs when the currently active window changes.")]
		public event EventHandler Changed;

		public ForegroundApplicationMonitorService()
		{
			this.enabled = false;
			this.EnableRaisingEvents = true;
			this.processId = 0;
			this.threadId = 0;
			this.process = null;
		}

		public void Enable()
		{
			this.enabled = true;
			this.hook();
			this.update();
		}

		public void Disable()
		{
			this.unhook();
			this.enabled = false;
		}

		private bool enabled;

		private int processId;

		private int threadId;

		private Process process;

		private ForegroundApplicationMonitorService.WinEventDelegate dele;

		private IntPtr m_hhook = IntPtr.Zero;

		private const uint WINEVENT_OUTOFCONTEXT = 0U;

		private const uint EVENT_SYSTEM_FOREGROUND = 3U;

		private const uint EVENT_SYSTEM_CAPTURESTART = 8U;

		private const uint EVENT_SYSTEM_MINIMIZEEND = 23U;

		private Process _realProcess;

		private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

		public delegate bool WindowEnumProc(IntPtr hwnd, IntPtr lparam);
	}
}
