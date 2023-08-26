using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Windows;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.ScreenMonitorStuff;
using reWASDEngine;

namespace XBEliteWPF.Infrastructure.reWASDMapping.RewasduserCommands.RewasdCommands
{
	[Serializable]
	public class TakeScreenshotCommand : BaseExecutableRewasdUserCommand
	{
		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

		public TakeScreenshotCommand(int id, string displayName, int displayNameStrId, string drawingResourcename)
			: base(id, displayName, displayNameStrId, drawingResourcename, 7)
		{
		}

		public override bool Execute(ulong profileID)
		{
			string text = Path.Combine(Engine.UserSettingsService.ScreenshotsFolderPath, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
			DSUtils.TryCreateFolderAndFile(text, false);
			ScreenCapturer.CaptureAndSave(text, CaptureMode.Window, null, true);
			return true;
		}

		protected TakeScreenshotCommand()
		{
		}

		protected TakeScreenshotCommand(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
