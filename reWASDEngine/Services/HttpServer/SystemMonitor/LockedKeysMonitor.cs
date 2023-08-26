using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace reWASDEngine.Services.HttpServer.SystemMonitor
{
	public class LockedKeysMonitor : AsyncLoopMonitor
	{
		public event LockedKeysChanged OnLockedKeysChanged;

		protected override int DELAY
		{
			get
			{
				return 100;
			}
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern short GetKeyState(int keyCode);

		protected override void Check()
		{
			HashSet<int> hashSet = new HashSet<int>();
			if (this.IsLocked(20))
			{
				hashSet.Add(20);
			}
			if (this.IsLocked(145))
			{
				hashSet.Add(145);
			}
			if (this.IsLocked(144))
			{
				hashSet.Add(144);
			}
			if (!hashSet.SetEquals(this.lastKeys))
			{
				LockedKeysChanged onLockedKeysChanged = this.OnLockedKeysChanged;
				if (onLockedKeysChanged != null)
				{
					onLockedKeysChanged(hashSet);
				}
			}
			this.lastKeys = hashSet;
		}

		private bool IsLocked(int key)
		{
			return ((ushort)LockedKeysMonitor.GetKeyState(key) & ushort.MaxValue) > 0;
		}

		private const int STATE_LOCKED = 65535;

		private const int CAPS_LOCK = 20;

		private const int SCROLL_LOCK = 145;

		private const int NUM_LOCK = 144;

		private HashSet<int> lastKeys = new HashSet<int>();
	}
}
