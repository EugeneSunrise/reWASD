using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DiscSoft.NET.Common.ColorStuff;
using DiscSoft.NET.Common.Utils;
using DiscSoftReWASDServiceNamespace;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Network.HTTP.DataTransferObjects.Events.Emulator;
using reWASDCommon.Utils;
using XBEliteWPF.DataModels.GamepadActiveProfiles;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.Controller;
using XBEliteWPF.Utils.Extensions;

namespace reWASDEngine.Services.HttpServer
{
	[Route("v1.7/Events/MobileController")]
	public class EmulatorEventController : ControllerBase
	{
		[HttpGet]
		[Route("{controllerId}")]
		{
			this._controllerId = controllerId;
			base.Response.Headers.Add("Content-Type", "text/event-stream");
			base.Response.Headers.Add("Cache-Control", "no-cache");
			base.Response.Headers.Add("Connection", "keep-alive");
			for (;;)
			{
				this.SubscribeToEvents();
				this.MonitorVibration(controllerId);
				this.SendHeartbeats();
				this.ReportVirtualGamepadType();
				foreach (EmulatorEvent @event in consumingEnumerable)
				{
					string text = "";
					if (@event != null)
					{
						text = typeof(EmulatorEvent).Name;
					}
					await base.Response.Body.FlushAsync();
					@event = null;
				}
				IEnumerator<EmulatorEvent> enumerator = null;
				this.UnsubscribeFromEvents();
			}
		}

		private void SubscribeToEvents()
		{
			Engine.LEDService.OnLEDStateChanged += this.OnLEDStateChanged;
			Engine.EventsProxy.OnRemapStateChanged += this.OnRemapStateChanged;
			Engine.EventsProxy.OnConfigAppliedToSlot += this.OnConfigApplied;
			Engine.EventsProxy.OnSlotChanged += this.OnSlotChanged;
		}

		private void UnsubscribeFromEvents()
		{
			Engine.LEDService.OnLEDStateChanged -= this.OnLEDStateChanged;
			Engine.EventsProxy.OnRemapStateChanged -= this.OnRemapStateChanged;
			Engine.EventsProxy.OnConfigAppliedToSlot -= this.OnConfigApplied;
			Engine.EventsProxy.OnSlotChanged -= this.OnSlotChanged;
		}

		private BaseControllerVM FindControllerByEngineControllerId(ulong id)
		{
			string text = id.ToString();
			if (Engine.GamepadService.FindControllerBySingleId(text) == null)
			{
				List<string> internalIdsForEngineController = UtilsCommon.GetInternalIdsForEngineController(text);
				if (internalIdsForEngineController.Count > 0)
				{
					return Engine.GamepadService.FindControllerBySingleId(internalIdsForEngineController[0]);
				}
			}
			return null;
		}

		private void OnLEDStateChanged(ulong id, zColor color, ref bool isOnline)
		{
			if (this._controllerId != id)
			{
				return;
			}
			this._events.Add(new LEDStateChangedEvent(color));
			BaseControllerVM baseControllerVM = this.FindControllerByEngineControllerId(id);
			if (baseControllerVM != null)
			{
				isOnline = baseControllerVM.IsOnline;
			}
		}

		private void OnRemapStateChanged(BaseControllerVM controller, RemapState newState)
		{
			if (!this.IsThisEngineController(controller))
			{
				return;
			}
			this.ReportVirtualGamepadType();
		}

		private bool IsThisEngineController(BaseControllerVM controller)
		{
			if (controller is ControllerVM && this._controllerId == controller.FirstId)
			{
				return true;
			}
			if (controller.Types.Length == controller.Ids.Length)
			{
				for (int i = 0; i < controller.Types.Length; i++)
				{
					if ((controller.Types[i] == 268435453U || controller.Types[i] == 268435454U) && UtilsCommon.GetEngineControllerIdByInternalId(controller.Types[i], controller.Ids[i]) == this._controllerId)
					{
						return true;
					}
				}
			}
			return false;
		}

		private void OnSlotChanged(BaseControllerVM controller, GamepadProfile gamepadProfile, Slot slot, REWASD_CONTROLLER_PROFILE? profile, bool physical)
		{
			if (!this.IsThisEngineController(controller))
			{
				return;
			}
			this.ReportVirtualGamepadType();
		}

		private void OnConfigApplied(BaseControllerVM controller, GamepadProfile gamepadProfile, Slot slot, REWASD_CONTROLLER_PROFILE? profile)
		{
			if (!this.IsThisEngineController(controller))
			{
				return;
			}
			this.ReportVirtualGamepadType();
		}

		private void ReportVirtualGamepadType()
		{
			VirtualGamepadType? virtualGamepadType = this.GetVirtualGamepadType();
			this._events.Add(new VirtualGamepadChangedEvent(virtualGamepadType));
		}

		private VirtualGamepadType? GetVirtualGamepadType()
		{
			string text = this._controllerId.ToString();
			BaseControllerVM baseControllerVM = Engine.GamepadService.FindControllerBySingleId(text);
			if (baseControllerVM == null)
			{
				List<string> internalIdsForEngineController = UtilsCommon.GetInternalIdsForEngineController(text);
				if (internalIdsForEngineController.Count != 0)
				{
					baseControllerVM = Engine.GamepadService.FindControllerBySingleId(internalIdsForEngineController[0]);
					if (baseControllerVM == null)
					{
						return null;
					}
				}
			}
			return Engine.GamepadService.GetVirtualGamepadType(baseControllerVM);
		}

		private async void MonitorVibration(ulong controllerId)
		{
			bool isVibrating = false;
			for (;;)
			{
				try
				{
					await Task.Delay(50);
					REWASD_CONTROLLER_PROFILE_EX rewasd_CONTROLLER_PROFILE_EX = Engine.GamepadService.ServiceProfilesCollection.FindByControllerID(controllerId);
					if (rewasd_CONTROLLER_PROFILE_EX == null)
					{
						continue;
					}
					Slot currentSlot = rewasd_CONTROLLER_PROFILE_EX.GetCurrentSlot();
					ushort num = rewasd_CONTROLLER_PROFILE_EX.ServiceProfileIds[currentSlot];
					GamepadState gamepadState = await Engine.XBServiceCommunicator.GetVirtualGamepadPressedButtons(num);
					if (gamepadState == null)
					{
						continue;
					}
					if (!isVibrating && gamepadState.LeftMotor == 0 && gamepadState.RightMotor == 0 && gamepadState.LeftTriggerMotor == 0 && gamepadState.RightTriggerMotor == 0)
					{
						continue;
					}
					isVibrating = gamepadState.LeftMotor != 0 || gamepadState.RightMotor != 0 || gamepadState.LeftTriggerMotor != 0 || gamepadState.RightTriggerMotor > 0;
					this._events.Add(new VibrateEvent(gamepadState.LeftMotor, gamepadState.RightMotor, gamepadState.LeftTriggerMotor, gamepadState.RightTriggerMotor));
					continue;
				}
				catch (Exception ex)
				{
					Tracer.TraceWriteTag("EmulatorEventController", "MonitorVibration stopped", false);
					Tracer.TraceException(ex, "MonitorVibration");
				}
				break;
			}
		}

		private async void SendHeartbeats()
		{
			for (;;)
			{
				try
				{
					Task task = Task.Delay(60000);
					this._events.Add(new EmulatorHeartbeatEvent());
					await task;
					continue;
				}
				catch (Exception)
				{
				}
				break;
			}
		}

		private BlockingCollection<EmulatorEvent> _events = new BlockingCollection<EmulatorEvent>();

		private ulong _controllerId;
	}
}
