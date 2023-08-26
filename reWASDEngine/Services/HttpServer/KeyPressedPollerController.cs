using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.Controller;

namespace reWASDEngine.Services.HttpServer
{
	[Route("v1.7/KeyPressedPoller")]
	public class KeyPressedPollerController : Controller
	{
		[HttpGet]
		[Route("{controllerId}")]
		{
			bool allGamepads = controllerId == "all";
			BaseControllerVM currentGamepad = null;
			if (!allGamepads)
			{
				currentGamepad = Engine.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.ID == controllerId);
				if (currentGamepad == null)
				{
					return;
				}
			}
			base.Response.Headers.Add("Content-Type", "text/event-stream");
			base.Response.Headers.Add("Cache-Control", "no-cache");
			base.Response.Headers.Add("Connection", "keep-alive");
			{
				GamepadState gamepadState = new GamepadState();
				bool hasState = false;
				if (allGamepads)
				{
					foreach (BaseControllerVM baseControllerVM in Engine.GamepadService.GamepadCollection)
					{
						if (baseControllerVM.IsOnline)
						{
							GamepadState gamepadState2 = gamepadState;
							GamepadState gamepadState3 = await baseControllerVM.GetControllerPressedButtons();
							gamepadState2.Add(gamepadState3);
							gamepadState2 = null;
							hasState = true;
						}
					}
					IEnumerator<BaseControllerVM> enumerator = null;
				}
				else if (currentGamepad.IsOnline)
				{
					GamepadState gamepadState3 = await currentGamepad.GetControllerPressedButtons();
					gamepadState = gamepadState3;
					hasState = true;
				}
				string text = "GamepadState";
				await base.Response.Body.FlushAsync();
				Thread.Sleep(hasState ? 75 : 1000);
				gamepadState = null;
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		private const int POLL_INTERVAL = 75;
	}
}
