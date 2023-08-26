using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using reWASDEngine.Services.Interfaces;
using XBEliteWPF.Services.Interfaces;

namespace reWASDEngine.Services.UdpServer
{
	public class EngineControllerMonitor : IEngineControllerMonitor
	{
		private long Now
		{
			get
			{
				return DateTimeOffset.Now.ToUnixTimeMilliseconds();
			}
		}

		public void AddController(ulong id)
		{
			Dictionary<ulong, long> dictionary = this.checkups;
			lock (dictionary)
			{
				this.checkups.ContainsKey(id);
				this.checkups[id] = this.Now;
				if (!this.isMonotoring)
				{
					this.StartMonitoring();
				}
			}
		}

		public void RemoveController(ulong id)
		{
			Dictionary<ulong, long> dictionary = this.checkups;
			lock (dictionary)
			{
				this.checkups.Remove(id);
				if (this.checkups.Count == 0)
				{
					this.StopMonitoring();
				}
			}
		}

		public void ResetTimer(ulong id)
		{
			Dictionary<ulong, long> dictionary = this.checkups;
			lock (dictionary)
			{
				this.checkups[id] = this.Now;
			}
		}

		private async void StartMonitoring()
		{
			this.isMonotoring = true;
			while (this.isMonotoring)
			{
				try
				{
					long now = this.Now;
					Dictionary<ulong, long> dictionary = this.checkups;
					Dictionary<ulong, long> dictionary2;
					lock (dictionary)
					{
						dictionary2 = new Dictionary<ulong, long>(this.checkups);
					}
					foreach (KeyValuePair<ulong, long> r in dictionary2)
					{
						if (r.Value + (long)EngineControllerMonitor.CONTROLLER_TIMEOUT <= now)
						{
							this.RemoveController(r.Key);
							Lazy<IGamepadService> gamepadServiceLazy = Engine.GamepadServiceLazy;
							bool flag2;
							if (gamepadServiceLazy == null)
							{
								flag2 = null != null;
							}
							else
							{
								IGamepadService value = gamepadServiceLazy.Value;
								flag2 = ((value != null) ? value.EngineControllersWpapper : null) != null;
							}
							if (flag2)
							{
								List<uint> list = await Engine.GamepadServiceLazy.Value.EngineControllersWpapper.FindAllEngineTypesById(r.Key);
								if (list.Count > 0)
								{
									foreach (uint num in list)
									{
										await Engine.GamepadServiceLazy.Value.EngineControllersWpapper.RemoveEngineController(r.Key, num);
									}
									List<uint>.Enumerator enumerator2 = default(List<uint>.Enumerator);
								}
							}
						}
						r = default(KeyValuePair<ulong, long>);
					}
					Dictionary<ulong, long>.Enumerator enumerator = default(Dictionary<ulong, long>.Enumerator);
				}
				catch (Exception)
				{
				}
			}
		}

		private void StopMonitoring()
		{
			this.isMonotoring = false;
			{
				return;
			}
		}

		private static int CONTROLLER_TIMEOUT = 10000;

		private bool isMonotoring;

		private Dictionary<ulong, long> checkups = new Dictionary<ulong, long>();

	}
}
