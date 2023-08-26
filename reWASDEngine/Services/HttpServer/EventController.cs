using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoftReWASDServiceNamespace;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using reWASDCommon.Network.HTTP.DataTransferObjects.Events;
using reWASDCommon.Network.HTTP.DataTransferObjects.Events.Desktop;
using reWASDCommon.Utils;
using XBEliteWPF.DataModels;
using XBEliteWPF.DataModels.GamepadActiveProfiles;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.Controller;
using XBEliteWPF.Infrastructure.ExternalDevices;
using XBEliteWPF.License.Licensing.ComStructures;
using XBEliteWPF.Services.Interfaces;

namespace reWASDEngine.Services.HttpServer
{
	[Route("v1.7/Events")]
	public class EventController : Controller
	{
		private void OnServiceProfilesChanged(WindowMessageEvent obj)
		{
			this.AddEvent(new ServiceProfilesChangedEvent());
		}

		private void OnRemapStateChanged(BaseControllerVM controller, RemapState newState)
		{
			this.AddEvent(new RemapStateChangedEvent(controller.ID, controller.ControllerDisplayName, newState));
		}

		private void OnSlotChanged(BaseControllerVM controller, GamepadProfile gamepadProfile, Slot slot, REWASD_CONTROLLER_PROFILE? profile, bool physical)
		{
			if (!controller.IsRemapInProgress || physical)
			{
				this.AddEvent(new SlotChangedEvent(controller.ID, controller.ControllerDisplayName, gamepadProfile, slot, profile));
			}
		}

		private void OnGamepadStateChanged(BaseControllerVM controller)
		{
			this.AddEvent(new GamepadStateChangedEvent(controller.ID, controller.ControllerDisplayName, controller.IsOnline));
		}

		private void OnBatteryLevelChanged(ControllerVM controller)
		{
			this.AddEvent(new BatteryLevelChangedEvent(controller.ID, controller.ControllerDisplayName, controller.ControllerBatteryLevel, controller.IsBatteryLevelPercentPresent, controller.BatteryLevelPercent, controller.ControllerBatteryChargingState));
		}

		private void OnConfigSaved(ConfigSavedEvent savedEvent)
		{
			this.AddEvent(savedEvent);
		}

		private void OnConfigRenamed(RenameConfigParams parameters)
		{
			this.AddEvent(new ConfigRenamedEvent(parameters));
		}

		private void OnConfigDeleted(DeleteConfigParams parameters)
		{
			this.AddEvent(new ConfigDeletedEvent(parameters.ClientID, parameters.GameName, parameters.ConfigName));
		}

		private void OnGameRenamed(RenameGameParams parameters)
		{
			this.AddEvent(new GameRenamedEvent(parameters));
		}

		private void OnCompositeSettingsChanged(object obj)
		{
			this.AddEvent(new CompositeSettingsChangedEvent());
		}

		private void OnGameDeleted(Tuple<string, string> parameters)
		{
			this.AddEvent(new GameDeletedEvent(parameters.Item1, parameters.Item2));
		}

		private void OnExternalDevicesChanged(object obj)
		{
			this.AddEvent(new ExternalDevicesChangedEvent());
		}

		private void OnExternalDeviceOutdated(ExternalDevice externalDevice)
		{
			this.AddEvent(new ExternalDeviceOutdatedEvent(externalDevice));
		}

		private void OnControllerAdded(BaseControllerVM controller)
		{
			this.AddEvent(new ControllerConnectedEvent(controller));
		}

		private void OnControllerChanged(BaseControllerVM controller)
		{
			this.AddEvent(new ControllerChangedEvent(controller));
		}

		private void OnControllerRemoved(BaseControllerVM controller)
		{
			this.AddEvent(new ControllerDisconnectedEvent(controller.ID, controller.ControllerDisplayName, controller.ContainerIdString));
		}

		private void OnAllControllersRemoved()
		{
			this.AddEvent(new AllControllersDisconnectedEvent());
		}

		private void OnShiftChanged(BaseControllerVM controller, ShiftInfo shift, bool toggle)
		{
			int shift2 = shift.Shift;
			this.AddEvent(new ShiftChangedEvent(controller.ID, controller.ControllerDisplayName, shift2));
		}

		private void OnConfigApplied(BaseControllerVM controller, GamepadProfile gamepadProfile, Slot slot, REWASD_CONTROLLER_PROFILE? profile)
		{
			this.AddEvent(new ConfigAppliedEvent(controller.ID, controller.ControllerDisplayName, gamepadProfile.GameName, gamepadProfile.Config.Name, slot));
		}

		private void OnGyroCalibrationFinished(WindowMessageEvent payload)
		{
			this.AddEvent(new GyroCalibrationFinishedEvent(DSUtils.GetUlongFromWMPayload(payload).ToString()));
		}

		private void OnRequestReloadConfig(string configPath)
		{
			Config config = Engine.GameProfilesService.FindConfigByPath(configPath, false);
			if (config != null)
			{
				this.AddEvent(new ConfigSavedEvent("engine", config.GameName, config.Name));
			}
		}

		private void OnLicenseChanged(LicenseCheckResult result, bool onlineActivation)
		{
			this.AddEvent(new LicenseChangedEvent(result, onlineActivation));
		}

		private void OnControllerAddedUI(BaseControllerVM controller)
		{
			this.AddEvent(new ControllerConnectedUIEvent(controller));
		}

		private void OnControllerRemovedUI(BaseControllerVM controller, string fullID, string nameForDisconnect)
		{
			this.AddEvent(new ControllerDisconnectedUIEvent(controller.ID, nameForDisconnect));
		}

		private void OnAllControllersRemovedUI()
		{
			this.AddEvent(new AllControllersDisconnectedUIEvent());
		}

		private void OnRemapOffUI(string id, string deviceName)
		{
			this.AddEvent(new RemapOffUIEvent(id, deviceName));
		}

		private void OnConfigAppliedToSlotUI(BaseControllerVM controller, GamepadProfile gamepadProfile, Slot slot, REWASD_CONTROLLER_PROFILE? profile)
		{
			this.AddEvent(new ConfigAppliedUIEvent(controller.ID, controller.ControllerDisplayName, gamepadProfile.GameName, gamepadProfile.ProfileName, slot));
		}

		private void OnSlotChangedUI(BaseControllerVM controller, GamepadProfile gamepadProfile, Slot slot, REWASD_CONTROLLER_PROFILE? profile, bool physical)
		{
			this.AddEvent(new SlotChangedUIEvent(controller.ID, controller.ControllerDisplayName, gamepadProfile, slot, profile));
		}

		private void ShiftShowUI(BaseControllerVM controller, GamepadProfile gamepadProfile, ShiftInfo shift, bool toggle, bool alwaysShow)
		{
			this.AddEvent(new ShiftShowUIEvent(controller.ID, controller.ControllerDisplayName, shift, toggle, alwaysShow));
		}

		private void ShiftHideUI(string id)
		{
			this.AddEvent(new ShiftHideUIEvent(id));
		}

		private void OnBatteryLevelChangedUI(ControllerVM controller)
		{
			this.AddEvent(new BatteryLevelChangedUIEvent(controller.ID, controller.ControllerDisplayName, controller.ControllerBatteryLevel, controller.IsBatteryLevelPercentPresent, controller.BatteryLevelPercent));
		}

		private void OnHoneypotPairingRejected(WindowMessageEvent payload)
		{
			Tracer.TraceWrite("HoneypotPairingRejected", false);
			if (payload != null)
			{
				ulong num = DSUtils.GetUlongFromWMPayload(payload) & 281474976710655UL;
				string text = UtilsCommon.MacAddressToString(num, ":");
				if (!string.IsNullOrEmpty(text))
				{
					Tracer.TraceWrite("HoneypotPairingRejected: MAC address = " + text, false);
				}
				this.AddEvent(new HoneypotPairingRejectedEvent(num));
			}
		}

		private void OnProfileRelationsChangedByEngine(object _)
		{
			this.AddEvent(new ProfileRelationsChangedByEngineEvent());
		}

		private void AddEvent(BaseEvent @event)
		{
			try
			{
				if (!this._events.IsAddingCompleted)
				{
					this._events.Add(@event);
				}
			}
			catch (ObjectDisposedException)
			{
			}
		}

		[HttpGet]
		[Route("")]
		{
			base.Response.Headers.Add("Content-Type", "text/event-stream");
			base.Response.Headers.Add("Cache-Control", "no-cache");
			base.Response.Headers.Add("Connection", "keep-alive");
			for (;;)
			{
				this.SubscribeToEvents();
				this.SendHeartbeats();
				foreach (BaseEvent @event in consumingEnumerable)
				{
					string text = @event.Type.ToString();
					await base.Response.Body.FlushAsync();
					@event = null;
				}
				IEnumerator<BaseEvent> enumerator = null;
				this.UnsubscribeFromEvents();
			}
		}

		private async void SendHeartbeats()
		{
			for (;;)
			{
				try
				{
					Task task = Task.Delay(60000);
					this._events.Add(new HeartbeatEvent());
					await task;
					continue;
				}
				catch (Exception)
				{
				}
				break;
			}
		}

		private void SubscribeToEvents()
		{
			Engine.EventsProxy.OnControllerAdded += this.OnControllerAdded;
			Engine.EventsProxy.OnControllerChanged += this.OnControllerChanged;
			Engine.EventsProxy.OnControllerRemoved += this.OnControllerRemoved;
			Engine.EventsProxy.OnAllControllersRemoved += this.OnAllControllersRemoved;
			Engine.EventsProxy.OnRemapStateChanged += this.OnRemapStateChanged;
			Engine.EventsProxy.OnConfigAppliedToSlot += this.OnConfigApplied;
			Engine.EventsProxy.OnSlotChanged += this.OnSlotChanged;
			Engine.EventsProxy.OnShiftChanged += this.OnShiftChanged;
			Engine.EventsProxy.OnBatteryLevelChanged += this.OnBatteryLevelChanged;
			Engine.EventsProxy.OnGamepadStateChanged += this.OnGamepadStateChanged;
			Engine.EventsProxy.OnControllerAddedUI += this.OnControllerAddedUI;
			Engine.EventsProxy.OnControllerRemovedUI += this.OnControllerRemovedUI;
			Engine.EventsProxy.OnAllControllersRemovedUI += this.OnAllControllersRemovedUI;
			Engine.EventsProxy.OnRemapOffUI += this.OnRemapOffUI;
			Engine.EventsProxy.OnConfigAppliedToSlotUI += this.OnConfigAppliedToSlotUI;
			Engine.EventsProxy.OnSlotChangedUI += this.OnSlotChangedUI;
			Engine.EventsProxy.ShiftShowUI += this.ShiftShowUI;
			Engine.EventsProxy.ShiftHideUI += this.ShiftHideUI;
			Engine.EventsProxy.OnBatteryLevelChangedUI += this.OnBatteryLevelChangedUI;
			Engine.EventAggregator.GetEvent<RequestReloadConfig>().Subscribe(new Action<string>(this.OnRequestReloadConfig));
			Engine.EventAggregator.GetEvent<ProfileRelationsChangedByEngine>().Subscribe(new Action<object>(this.OnProfileRelationsChangedByEngine));
			Engine.EventAggregator.GetEvent<GyroCalibrationFinished>().Subscribe(new Action<WindowMessageEvent>(this.OnGyroCalibrationFinished));
			Engine.EventAggregator.GetEvent<ConfigSaved>().Subscribe(new Action<ConfigSavedEvent>(this.OnConfigSaved));
			Engine.EventAggregator.GetEvent<ConfigRenamed>().Subscribe(new Action<RenameConfigParams>(this.OnConfigRenamed));
			Engine.EventAggregator.GetEvent<GameRenamed>().Subscribe(new Action<RenameGameParams>(this.OnGameRenamed));
			Engine.EventAggregator.GetEvent<ConfigDeleted>().Subscribe(new Action<DeleteConfigParams>(this.OnConfigDeleted));
			Engine.EventAggregator.GetEvent<GameDeleted>().Subscribe(new Action<Tuple<string, string>>(this.OnGameDeleted));
			Engine.EventAggregator.GetEvent<ExternalHelperChanged>().Subscribe(new Action<object>(this.OnExternalDevicesChanged));
			Engine.EventAggregator.GetEvent<ExternalDeviceOutdated>().Subscribe(new Action<ExternalDevice>(this.OnExternalDeviceOutdated));
			Engine.EventAggregator.GetEvent<CompositeSettingsChanged>().Subscribe(new Action<object>(this.OnCompositeSettingsChanged));
			Engine.EventAggregator.GetEvent<HoneypotPairingRejectedBroadcast>().Subscribe(new Action<WindowMessageEvent>(this.OnHoneypotPairingRejected));
			Engine.LicensingService.OnLicenseChangedCompleted += new LicenseServiceDelegates.LicenseChangedDelegate(this.OnLicenseChanged);
		}

		private void UnsubscribeFromEvents()
		{
			Engine.EventsProxy.OnControllerAdded -= this.OnControllerAdded;
			Engine.EventsProxy.OnControllerChanged -= this.OnControllerChanged;
			Engine.EventsProxy.OnControllerRemoved -= this.OnControllerRemoved;
			Engine.EventsProxy.OnAllControllersRemoved -= this.OnAllControllersRemoved;
			Engine.EventsProxy.OnRemapStateChanged -= this.OnRemapStateChanged;
			Engine.EventsProxy.OnConfigAppliedToSlot -= this.OnConfigApplied;
			Engine.EventsProxy.OnSlotChanged -= this.OnSlotChanged;
			Engine.EventsProxy.OnShiftChanged -= this.OnShiftChanged;
			Engine.EventsProxy.OnBatteryLevelChanged -= this.OnBatteryLevelChanged;
			Engine.EventsProxy.OnGamepadStateChanged -= this.OnGamepadStateChanged;
			Engine.EventsProxy.OnControllerAddedUI -= this.OnControllerAddedUI;
			Engine.EventsProxy.OnControllerRemovedUI -= this.OnControllerRemovedUI;
			Engine.EventsProxy.OnAllControllersRemovedUI -= this.OnAllControllersRemovedUI;
			Engine.EventsProxy.OnRemapOffUI -= this.OnRemapOffUI;
			Engine.EventsProxy.OnConfigAppliedToSlotUI -= this.OnConfigAppliedToSlotUI;
			Engine.EventsProxy.OnSlotChangedUI -= this.OnSlotChangedUI;
			Engine.EventsProxy.ShiftShowUI -= this.ShiftShowUI;
			Engine.EventsProxy.ShiftHideUI -= this.ShiftHideUI;
			Engine.EventsProxy.OnBatteryLevelChangedUI -= this.OnBatteryLevelChangedUI;
			Engine.EventAggregator.GetEvent<RequestReloadConfig>().Unsubscribe(new Action<string>(this.OnRequestReloadConfig));
			Engine.EventAggregator.GetEvent<ProfileRelationsChangedByEngine>().Unsubscribe(new Action<object>(this.OnProfileRelationsChangedByEngine));
			Engine.EventAggregator.GetEvent<GyroCalibrationFinished>().Unsubscribe(new Action<WindowMessageEvent>(this.OnGyroCalibrationFinished));
			Engine.EventAggregator.GetEvent<ConfigSaved>().Unsubscribe(new Action<ConfigSavedEvent>(this.OnConfigSaved));
			Engine.EventAggregator.GetEvent<ConfigRenamed>().Unsubscribe(new Action<RenameConfigParams>(this.OnConfigRenamed));
			Engine.EventAggregator.GetEvent<GameRenamed>().Unsubscribe(new Action<RenameGameParams>(this.OnGameRenamed));
			Engine.EventAggregator.GetEvent<ConfigDeleted>().Unsubscribe(new Action<DeleteConfigParams>(this.OnConfigDeleted));
			Engine.EventAggregator.GetEvent<GameDeleted>().Unsubscribe(new Action<Tuple<string, string>>(this.OnGameDeleted));
			Engine.EventAggregator.GetEvent<HoneypotPairingRejectedBroadcast>().Unsubscribe(new Action<WindowMessageEvent>(this.OnHoneypotPairingRejected));
			Engine.LicensingService.OnLicenseChangedCompleted -= new LicenseServiceDelegates.LicenseChangedDelegate(this.OnLicenseChanged);
		}

		protected override void Dispose(bool disposing)
		{
			this.UnsubscribeFromEvents();
			this._events.CompleteAdding();
			base.Dispose(disposing);
		}

		private BlockingCollection<BaseEvent> _events = new BlockingCollection<BaseEvent>();
	}
}
