using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using reWASDCommon.Network.HTTP.DataTransferObjects.Events.System;
using reWASDEngine.Services.HttpServer.SystemMonitor;

namespace reWASDEngine.Services.HttpServer
{
	[Route("v1.7/Events/System")]
	public class SystemEventController : Controller
	{
		[HttpGet]
		[Route("")]
		{
			base.Response.Headers.Add("Content-Type", "text/event-stream");
			base.Response.Headers.Add("Cache-Control", "no-cache");
			base.Response.Headers.Add("Connection", "keep-alive");
			for (;;)
			{
				this._systemMonitor = new SystemEventsMonitor();
				this._systemMonitor.OnInputLocaleIdChanged += this.OnInputLocaleIdChanged;
				this._systemMonitor.OnLockedKeysChanged += this.OnLockedKeysChanged;
				this._systemMonitor.Monitor();
				this.SendHeartbeats();
				foreach (SystemEvent @event in consumingEnumerable)
				{
					string text = "";
					if (@event != null)
					{
						text = typeof(SystemEvent).Name;
					}
					await base.Response.Body.FlushAsync();
					@event = null;
				}
				IEnumerator<SystemEvent> enumerator = null;
				this._systemMonitor.Stop();
			}
		}

		private void OnLockedKeysChanged(HashSet<int> keys)
		{
			List<byte> list = this.ConvertWinKeysToScanCodeBytes(keys);
			this._events.Add(new KeyboardLockedKeysChangedEvent(list));
		}

		private List<byte> ConvertWinKeysToScanCodeBytes(HashSet<int> keys)
		{
			List<byte> list = new List<byte>();
			Dictionary<int, byte> dictionary = new Dictionary<int, byte>
			{
				{ 20, 58 },
				{ 145, 70 },
				{ 144, 69 }
			};
			foreach (int num in keys)
			{
				if (dictionary.ContainsKey(num))
				{
					list.Add(dictionary[num]);
				}
			}
			return list;
		}

		private void OnInputLocaleIdChanged(uint lid)
		{
			this._events.Add(new InputLocaleIdChangedEvent(lid));
		}

		private async void SendHeartbeats()
		{
			for (;;)
			{
				try
				{
					Task task = Task.Delay(60000);
					this._events.Add(new SystemHeartbeatEvent());
					await task;
					continue;
				}
				catch (Exception)
				{
				}
				break;
			}
		}

		private BlockingCollection<SystemEvent> _events = new BlockingCollection<SystemEvent>();

		private SystemEventsMonitor _systemMonitor;
	}
}
