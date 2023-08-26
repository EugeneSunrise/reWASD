using System;
using System.Collections.Generic;

namespace reWASDEngine.Services.HttpServer.SystemMonitor
{
	public class SystemEventsMonitor
	{
		public event InputLocaleIdChanged OnInputLocaleIdChanged;

		public event LockedKeysChanged OnLockedKeysChanged;

		public void Monitor()
		{
			this.MonitorLockedKeyboardKeys();
			this.MonitorInputLanguage();
		}

		public void Stop()
		{
			this.lockedKeysMonitor.Stop();
			this.inputLocaleIdMonitor.Stop();
		}

		private void MonitorLockedKeyboardKeys()
		{
			this.lockedKeysMonitor.OnLockedKeysChanged += delegate(HashSet<int> keys)
			{
				LockedKeysChanged onLockedKeysChanged = this.OnLockedKeysChanged;
				if (onLockedKeysChanged == null)
				{
					return;
				}
				onLockedKeysChanged(keys);
			};
			this.lockedKeysMonitor.Monitor();
		}

		private void MonitorInputLanguage()
		{
			this.inputLocaleIdMonitor.OnInputLocaleIdChanged += delegate(uint newLang)
			{
				InputLocaleIdChanged onInputLocaleIdChanged = this.OnInputLocaleIdChanged;
				if (onInputLocaleIdChanged == null)
				{
					return;
				}
				onInputLocaleIdChanged(newLang);
			};
			this.inputLocaleIdMonitor.Monitor();
		}

		private InputLocaleIdMonitor inputLocaleIdMonitor = new InputLocaleIdMonitor();

		private LockedKeysMonitor lockedKeysMonitor = new LockedKeysMonitor();
	}
}
