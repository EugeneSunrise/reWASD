using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using DiscSoft.NET.Common.Utils;
using reWASDEngine;
using XBEliteWPF.Utils;

namespace DTOverlay
{
	public class OverlayTracker
	{
		[DllImport("user32.dll")]
		private static extern bool PostThreadMessage(int idThread, uint Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("kernel32.dll")]
		private static extern int GetLastError();

		[DllImport("User32.dll")]
		public static extern uint RegisterWindowMessage(string str);

		~OverlayTracker()
		{
			this.DeactivateALL();
		}

		public bool Inject(int id, string curRemapedForegroundName)
		{
			bool flag = true;
			this.CheckKilledProcess();
			if (this.injectedProcesses.Any((OverlayTracker.TYPE_ProcessInfo p) => p.id == id))
			{
				Tracer.TraceWrite(string.Concat(new string[]
				{
					"OverlayTracker process ",
					curRemapedForegroundName,
					"  Id:",
					id.ToString(),
					" alredy injected. Skip"
				}), false);
				return false;
			}
			if (this.failedProcesses.Contains(id))
			{
				Tracer.TraceWrite(string.Concat(new string[]
				{
					"OverlayTracker process ",
					curRemapedForegroundName,
					"  Id:",
					id.ToString(),
					" skip - there were attempts at unsuccessful injections"
				}), false);
				return false;
			}
			if (Injector.InjectorRun(id, curRemapedForegroundName, false))
			{
				OverlayTracker.TYPE_ProcessInfo type_ProcessInfo;
				type_ProcessInfo.id = id;
				type_ProcessInfo.name = curRemapedForegroundName;
				object lockData = OverlayTracker.LockData;
				lock (lockData)
				{
					this.injectedProcesses.Add(type_ProcessInfo);
					return flag;
				}
			}
			this.failedProcesses.Add(id);
			return false;
		}

		public void DeactivateALL()
		{
			this.UnInject(0);
			Thread.Sleep(50);
			object lockData = OverlayTracker.LockData;
			lock (lockData)
			{
				this.overlayThreads.Clear();
			}
		}

		public void DeactivateDeletedFromPreferences()
		{
			object lockData = OverlayTracker.LockData;
			lock (lockData)
			{
				List<int> list = new List<int>();
				string[] array = Engine.UserSettingsService.DirectX_Apps.ToLower().Split(';', StringSplitOptions.None);
				foreach (OverlayTracker.TYPE_ThreadInfo type_ThreadInfo in this.overlayThreads)
				{
					bool flag2 = false;
					string[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						if (array2[i].StartsWith(type_ThreadInfo.name))
						{
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						this.UnInject(type_ThreadInfo.id_process);
						Thread.Sleep(50);
						list.Add(type_ThreadInfo.id_process);
					}
				}
				using (List<int>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						int threadIDdel = enumerator2.Current;
						this.overlayThreads.RemoveAll((OverlayTracker.TYPE_ThreadInfo p) => p.id_thread == threadIDdel);
					}
				}
			}
			this.CheckKilledProcess();
		}

		public bool IsDirectXAttached()
		{
			object lockData = OverlayTracker.LockData;
			bool flag2;
			lock (lockData)
			{
				flag2 = this.overlayThreads.Count<OverlayTracker.TYPE_ThreadInfo>() > 0;
			}
			return flag2;
		}

		private void SendMessageToOverlay(OverlayMessageType message, int param, int iThreadID = 0)
		{
			List<int> list = new List<int>();
			if (this.nMessage == 0U)
			{
				this.nMessage = OverlayTracker.RegisterWindowMessage("{0DC46C90-6125-4B3C-9A7F-0823930C0E58}");
			}
			object obj;
			if (iThreadID != 0)
			{
				if (!OverlayTracker.PostThreadMessage(iThreadID, this.nMessage, (IntPtr)message, (IntPtr)param))
				{
					Tracer.TraceWrite("OverlayTracker Send message to overlay failed " + OverlayTracker.GetLastError().ToString() + " removing thread " + iThreadID.ToString(), false);
					list.Add(iThreadID);
				}
			}
			else
			{
				obj = OverlayTracker.LockData;
				lock (obj)
				{
					foreach (OverlayTracker.TYPE_ThreadInfo type_ThreadInfo in this.overlayThreads)
					{
						if (!OverlayTracker.PostThreadMessage(type_ThreadInfo.id_thread, this.nMessage, (IntPtr)message, (IntPtr)param))
						{
							Tracer.TraceWrite("OverlayTracker Send message to overlay failed " + OverlayTracker.GetLastError().ToString() + " removing thread " + type_ThreadInfo.ToString(), false);
							list.Add(type_ThreadInfo.id_thread);
						}
					}
				}
			}
			obj = OverlayTracker.LockData;
			lock (obj)
			{
				using (List<int>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						int threadIDdel = enumerator2.Current;
						this.overlayThreads.RemoveAll((OverlayTracker.TYPE_ThreadInfo p) => p.id_thread == threadIDdel);
					}
				}
			}
			this.CheckKilledProcess();
		}

		public void ReceivedOverlayMessage(OverlayMessageType messageType, IntPtr lParam)
		{
			Tracer.TraceWrite("OverlayTracker " + messageType.ToString() + " " + lParam.ToString(), false);
			int lParamValue = lParam.ToInt32();
			object obj;
			switch (messageType)
			{
			case OverlayMessageType.AttachDll:
				break;
			case OverlayMessageType.DetachDll:
				obj = OverlayTracker.LockData;
				lock (obj)
				{
					this.injectedProcesses.RemoveAll((OverlayTracker.TYPE_ProcessInfo p) => p.id == lParamValue);
					return;
				}
				goto IL_16F;
			case OverlayMessageType.ThreadInitialized:
				goto IL_16F;
			case OverlayMessageType.ThreadTerminating:
				goto IL_20F;
			case OverlayMessageType.NeedStopInject:
				this.UnInject(lParamValue);
				Thread.Sleep(50);
				obj = OverlayTracker.LockData;
				lock (obj)
				{
					this.overlayThreads.RemoveAll((OverlayTracker.TYPE_ThreadInfo p) => p.id_process == lParamValue);
					return;
				}
				break;
			case OverlayMessageType.DirectX11_Init:
				goto IL_246;
			case OverlayMessageType.FullscreenOverlay_Show:
				goto IL_2AA;
			default:
				return;
			}
			obj = OverlayTracker.LockData;
			string name;
			lock (obj)
			{
				name = this.injectedProcesses.FirstOrDefault((OverlayTracker.TYPE_ProcessInfo p) => p.id == lParamValue).name;
				OverlayTracker.TYPE_ThreadInfo type_ThreadInfo;
				type_ThreadInfo.id_process = lParamValue;
				type_ThreadInfo.id_thread = 0;
				type_ThreadInfo.name = name;
				this.overlayThreads.Add(type_ThreadInfo);
			}
			if (name != null)
			{
				SenderGoogleAnalytics.SendMessageEvent("DLL_STARTED", name, "", -1L, false);
				Tracer.TraceWrite("OverlayTracker StatInjectDX11 DllStarted " + name, false);
				return;
			}
			return;
			IL_16F:
			obj = OverlayTracker.LockData;
			lock (obj)
			{
				for (int i = 0; i < this.overlayThreads.Count<OverlayTracker.TYPE_ThreadInfo>(); i++)
				{
					if (this.overlayThreads[i].id_thread == 0)
					{
						OverlayTracker.TYPE_ThreadInfo type_ThreadInfo2;
						type_ThreadInfo2.id_process = this.overlayThreads[i].id_process;
						type_ThreadInfo2.id_thread = lParamValue;
						type_ThreadInfo2.name = this.overlayThreads[i].name;
						this.overlayThreads[i] = type_ThreadInfo2;
						break;
					}
				}
				return;
			}
			IL_20F:
			obj = OverlayTracker.LockData;
			lock (obj)
			{
				this.overlayThreads.RemoveAll((OverlayTracker.TYPE_ThreadInfo p) => p.id_thread == lParamValue);
				return;
			}
			IL_246:
			obj = OverlayTracker.LockData;
			lock (obj)
			{
				string name2 = this.injectedProcesses.FirstOrDefault((OverlayTracker.TYPE_ProcessInfo p) => p.id == lParamValue).name;
				if (name2 != null)
				{
					SenderGoogleAnalytics.SendMessageEvent("DirectX11", name2, "", -1L, false);
					Tracer.TraceWrite("OverlayTracker StatInjectDX11 DirectX11 " + name2, false);
				}
				return;
			}
			IL_2AA:
			obj = OverlayTracker.LockData;
			lock (obj)
			{
				string name3 = this.injectedProcesses.FirstOrDefault((OverlayTracker.TYPE_ProcessInfo p) => p.id == lParamValue).name;
				if (name3 != null)
				{
					SenderGoogleAnalytics.SendMessageEvent("OverlayShowed", name3, "", -1L, false);
					Tracer.TraceWrite("OverlayTracker StatInjectDX11 OverlayShowed " + name3, false);
				}
			}
		}

		public void ShowOverlayMessage(int id, int align)
		{
			switch (align)
			{
			case 0:
				this.SendMessageToOverlay(OverlayMessageType.ShowOverlayMessage0, id, 0);
				return;
			case 1:
				this.SendMessageToOverlay(OverlayMessageType.ShowOverlayMessage1, id, 0);
				return;
			case 2:
				this.SendMessageToOverlay(OverlayMessageType.ShowOverlayMessage2, id, 0);
				return;
			case 3:
				this.SendMessageToOverlay(OverlayMessageType.ShowOverlayMessage3, id, 0);
				return;
			default:
				return;
			}
		}

		public void ShowOverlayRemap(int id, int align)
		{
			switch (align)
			{
			case 0:
				this.SendMessageToOverlay(OverlayMessageType.ShowOverlayRemap0, id, 0);
				return;
			case 1:
				this.SendMessageToOverlay(OverlayMessageType.ShowOverlayRemap1, id, 0);
				return;
			case 2:
				this.SendMessageToOverlay(OverlayMessageType.ShowOverlayRemap2, id, 0);
				return;
			case 3:
				this.SendMessageToOverlay(OverlayMessageType.ShowOverlayRemap3, id, 0);
				return;
			default:
				return;
			}
		}

		public void ShowOverlayGamepad(int id, int align)
		{
			switch (align)
			{
			case 0:
				this.SendMessageToOverlay(OverlayMessageType.ShowOverlayGamepad0, id, 0);
				return;
			case 1:
				this.SendMessageToOverlay(OverlayMessageType.ShowOverlayGamepad1, id, 0);
				return;
			case 2:
				this.SendMessageToOverlay(OverlayMessageType.ShowOverlayGamepad2, id, 0);
				return;
			case 3:
				this.SendMessageToOverlay(OverlayMessageType.ShowOverlayGamepad3, id, 0);
				return;
			default:
				return;
			}
		}

		public void ShowMenu(int id, int align)
		{
			switch (align)
			{
			case 0:
				this.SendMessageToOverlay(OverlayMessageType.ShowOverlayMenu0, id, 0);
				return;
			case 1:
				this.SendMessageToOverlay(OverlayMessageType.ShowOverlayMenu1, id, 0);
				return;
			case 2:
				this.SendMessageToOverlay(OverlayMessageType.ShowOverlayMenu2, id, 0);
				return;
			case 3:
				this.SendMessageToOverlay(OverlayMessageType.ShowOverlayMenu3, id, 0);
				return;
			case 4:
				this.SendMessageToOverlay(OverlayMessageType.ShowOverlayMenu4, id, 0);
				return;
			default:
				return;
			}
		}

		public void HideOverlayRemap()
		{
			this.SendMessageToOverlay(OverlayMessageType.HideOverlayRemap, 0, 0);
		}

		public void HideOverlayMessage()
		{
			this.SendMessageToOverlay(OverlayMessageType.HideOverlayMessage, 0, 0);
		}

		public void HideOverlayGamepad()
		{
			this.SendMessageToOverlay(OverlayMessageType.HideOverlayGamepad, 0, 0);
		}

		public void HideMenu()
		{
			this.SendMessageToOverlay(OverlayMessageType.HideOverlayMenu, 0, 0);
		}

		public void CheckKilledProcess()
		{
			object lockData = OverlayTracker.LockData;
			lock (lockData)
			{
				Process[] processes = Process.GetProcesses();
				List<int> list = new List<int>();
				using (List<OverlayTracker.TYPE_ProcessInfo>.Enumerator enumerator = this.injectedProcesses.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						OverlayTracker.TYPE_ProcessInfo process = enumerator.Current;
						if (processes.FirstOrDefault((Process pr) => pr.Id == process.id) == null)
						{
							list.Add(process.id);
						}
					}
				}
				using (List<int>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						int id = enumerator2.Current;
						this.injectedProcesses.RemoveAll((OverlayTracker.TYPE_ProcessInfo p) => p.id == id);
					}
				}
			}
		}

		public bool UnInject(int iThreadID = 0)
		{
			bool flag = true;
			Tracer.TraceWrite("OverlayTracker UnInject iThreadID = " + iThreadID.ToString(), false);
			if (iThreadID != 0)
			{
				if (Injector.InjectorRun(iThreadID, this.overlayThreads.FirstOrDefault((OverlayTracker.TYPE_ThreadInfo p) => p.id_thread == iThreadID).name, true))
				{
					Tracer.TraceWrite("OverlayTracker UnInject iThreadID = " + iThreadID.ToString() + " success", false);
				}
				else
				{
					flag = false;
				}
			}
			else
			{
				object lockData = OverlayTracker.LockData;
				lock (lockData)
				{
					foreach (OverlayTracker.TYPE_ThreadInfo type_ThreadInfo in this.overlayThreads)
					{
						if (Injector.InjectorRun(type_ThreadInfo.id_process, type_ThreadInfo.name, true))
						{
							Tracer.TraceWrite(string.Concat(new string[]
							{
								"OverlayTracker UnInject iThreadID = ",
								iThreadID.ToString(),
								" ",
								type_ThreadInfo.name,
								" success"
							}), false);
						}
						else
						{
							flag = false;
						}
					}
				}
			}
			this.CheckKilledProcess();
			return flag;
		}

		private uint nMessage;

		private static object LockData = new object();

		private List<OverlayTracker.TYPE_ThreadInfo> overlayThreads = new List<OverlayTracker.TYPE_ThreadInfo>();

		private List<OverlayTracker.TYPE_ProcessInfo> injectedProcesses = new List<OverlayTracker.TYPE_ProcessInfo>();

		private List<int> failedProcesses = new List<int>();

		private List<int> hookedProcesses = new List<int>();

		private struct TYPE_ProcessInfo
		{
			public int id;

			public string name;
		}

		private struct TYPE_ThreadInfo
		{
			public int id_thread;

			public int id_process;

			public string name;
		}
	}
}
