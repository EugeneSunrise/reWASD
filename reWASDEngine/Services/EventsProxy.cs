using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using DiscSoftReWASDServiceNamespace;
using DTOverlay;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using reWASDEngine.Services.Interfaces;
using XBEliteWPF.DataModels.GamepadActiveProfiles;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.Controller;
using XBEliteWPF.Services;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils.Extensions;

namespace reWASDEngine.Services
{
	public class EventsProxy : IEventsProxy
	{
		public event ControllerAddedHandler OnControllerAdded;

		public event ControllerChangedHandler OnControllerChanged;

		public event ControllerRemovedHandler OnControllerRemoved;

		public event AllControllersRemovedHandler OnAllControllersRemoved;

		public event RemapStateChangedHandler OnRemapStateChanged;

		public event ConfigAppliedToSlotHandler OnConfigAppliedToSlot;

		public event ControllerSlotChangedHandler OnSlotChanged;

		public event ShiftChangedHandler OnShiftChanged;

		public event BatteryLevelChangedHandler OnBatteryLevelChanged;

		public event GamepadStateChanged OnGamepadStateChanged;

		public event ControllerAddedHandler OnControllerAddedUI;

		public event ControllerRemovedHandlerUI OnControllerRemovedUI;

		public event AllControllersRemovedHandler OnAllControllersRemovedUI;

		public event RemapOffUIHandler OnRemapOffUI;

		public event ConfigAppliedToSlotHandler OnConfigAppliedToSlotUI;

		public event ControllerSlotChangedHandler OnSlotChangedUI;

		public event ShiftShowHandlerUI ShiftShowUI;

		public event ShiftHideHandlerUI ShiftHideUI;

		public event OffileDeviceHandle OffileDevice;

		public event ShiftHideHandlerUI ShiftHideOverlayUI;

		public event BatteryLevelChangedHandler OnBatteryLevelChangedUI;

		public event OverlayMenuHandler OverlayMenu;

		private void StartPollerMessages()
		{
			this._pollingTimerMessages = new DispatcherTimer();
			this._pollingTimerMessages.Tick += delegate([Nullable(2)] object o, EventArgs e)
			{
				this.OnTickTimerMessages();
			};
			this._pollingTimerMessages.Interval = new TimeSpan(0, 0, 0, 0, 100);
			this._pollingTimerMessages.Start();
		}

		private void StopPollerMessages()
		{
			DispatcherTimer pollingTimerMessages = this._pollingTimerMessages;
			if (pollingTimerMessages != null)
			{
				pollingTimerMessages.Stop();
			}
			this._pollingTimerMessages = null;
		}

		private void OnTickTimerMessages()
		{
			object obj = EventsProxy.lockRemapWait;
			List<EventsProxy.RemapOffWaitType> list;
			lock (obj)
			{
				list = this.remapOffWait.Values.ToList<EventsProxy.RemapOffWaitType>();
			}
			List<string> list2 = new List<string>();
			List<string> list3 = new List<string>();
			foreach (EventsProxy.RemapOffWaitType remapOffWaitType in list)
			{
				if ((DateTime.Now - remapOffWaitType.timeRecive).TotalMilliseconds > 500.0)
				{
					if (remapOffWaitType.waitShowRemap && remapOffWaitType.ID != null)
					{
						if (!Engine.GamepadService.ServiceProfilesCollection.ContainsProfileForID(remapOffWaitType.ID))
						{
							RemapOffUIHandler onRemapOffUI = this.OnRemapOffUI;
							if (onRemapOffUI != null)
							{
								onRemapOffUI(remapOffWaitType.ID, remapOffWaitType.deviceName);
							}
						}
						list3.Add(remapOffWaitType.ID);
						this.CheckAndCloseShifts(remapOffWaitType.ID);
					}
					list2.Add(remapOffWaitType.ID);
				}
			}
			obj = EventsProxy.lockRemapWait;
			lock (obj)
			{
				foreach (string text in list3)
				{
					if (text != null)
					{
						this.remapON.Remove(text);
					}
				}
				foreach (string text2 in list2)
				{
					if (text2 != null)
					{
						this.remapOffWait.Remove(text2);
					}
				}
			}
			if (!Engine.UserSettingsService.NotHideShift)
			{
				List<string> list4 = new List<string>();
				obj = EventsProxy.lockShift;
				lock (obj)
				{
					foreach (KeyValuePair<string, bool> keyValuePair in this.idShift)
					{
						if (keyValuePair.Value)
						{
							list4.Add(keyValuePair.Key);
						}
					}
				}
				foreach (string text3 in list4)
				{
					this.CheckAndCloseShifts(text3);
				}
			}
		}

		public EventsProxy(IGamepadService i_gamepadService, IEventProcessor eventProcessor)
		{
			this.gamepadService = i_gamepadService;
			this.gamepadService.OnGamepadStateChanged += this.HandleGamepadStateChanged;
			this.gamepadService.OnBatteryLevelChanged += this.HandleBatteryChanged;
			this.gamepadService.OnBatteryLevelChangedUI += this.HandleBatteryChangedUI;
			this.gamepadService.OnConfigAppliedToSlot += this.HandleConfigAppliedToSlot;
			this.gamepadService.OnRemapStateChanged += this.HandleOnRemapStateChanged;
			this.gamepadService.OnControllerAdded += this.HandleControllerAdded;
			this.gamepadService.OnControllerChanged += this.HandleControllerChanged;
			this.gamepadService.OnControllerRemovedForProxy += this.OnControllerRemovedForProxy;
			this.gamepadService.OnAllControllersRemoved += this.HandleAllControllersRemoved;
			this.gamepadService.OnPhysicalSlotChanged += this.HandleSlotChanged;
			EventProcessor eventProcessor2 = (EventProcessor)eventProcessor;
			eventProcessor2.OverlaySlotChange += this.HandleSlotChanged;
			eventProcessor2.OverlayShiftChange += this.HandleShiftChanged;
			this.StartPollerMessages();
		}

		~EventsProxy()
		{
			this.StopPollerMessages();
		}

		private void HandleAllControllersRemoved()
		{
			AllControllersRemovedHandler onAllControllersRemoved = this.OnAllControllersRemoved;
			if (onAllControllersRemoved != null)
			{
				onAllControllersRemoved();
			}
			AllControllersRemovedHandler onAllControllersRemovedUI = this.OnAllControllersRemovedUI;
			if (onAllControllersRemovedUI == null)
			{
				return;
			}
			onAllControllersRemovedUI();
		}

		private void HandleControllerAdded(BaseControllerVM controller)
		{
			ControllerAddedHandler onControllerAdded = this.OnControllerAdded;
			if (onControllerAdded != null)
			{
				onControllerAdded(controller);
			}
			ControllerAddedHandler onControllerAddedUI = this.OnControllerAddedUI;
			if (onControllerAddedUI == null)
			{
				return;
			}
			onControllerAddedUI(controller);
		}

		private void HandleControllerChanged(BaseControllerVM controller)
		{
			ControllerChangedHandler onControllerChanged = this.OnControllerChanged;
			if (onControllerChanged == null)
			{
				return;
			}
			onControllerChanged(controller);
		}

		private void OnControllerRemovedForProxy(BaseControllerVM controller, bool showUI)
		{
			ControllerRemovedHandler onControllerRemoved = this.OnControllerRemoved;
			if (onControllerRemoved == null)
			{
				return;
			}
			onControllerRemoved(controller);
		}

		private void OnControllerRemovedForProxyUI(BaseControllerVM controller, bool showUI)
		{
			if (showUI)
			{
				object obj = EventsProxy.lockRemapWait;
				lock (obj)
				{
					this.CheckAndCloseShifts(controller.ID);
				}
				ControllerRemovedHandlerUI onControllerRemovedUI = this.OnControllerRemovedUI;
				if (onControllerRemovedUI == null)
				{
					return;
				}
				onControllerRemovedUI(controller, controller.ID, controller.ControllerDisplayName);
			}
		}

		private void CheckAndCloseShifts(string id)
		{
			string text = null;
			object obj = EventsProxy.lockShift;
			List<string> list;
			lock (obj)
			{
				list = this.idShift.Keys.ToList<string>();
			}
			foreach (string text2 in list)
			{
				if (OverlayUtils.IsContainsInLongID(text2, id))
				{
					text = text2;
					break;
				}
			}
			if (text != null)
			{
				obj = EventsProxy.lockShift;
				lock (obj)
				{
					this.idShift.Remove(text);
				}
				ShiftHideHandlerUI shiftHideOverlayUI = this.ShiftHideOverlayUI;
				if (shiftHideOverlayUI != null)
				{
					shiftHideOverlayUI(text);
				}
			}
			text = null;
			obj = EventsProxy.lockShift;
			lock (obj)
			{
				list = this.idShiftAndroid.Keys.ToList<string>();
			}
			foreach (string text3 in this.idShiftAndroid.Keys)
			{
				if (OverlayUtils.IsContainsInLongID(text3, id))
				{
					text = text3;
					break;
				}
			}
			if (text != null)
			{
				obj = EventsProxy.lockShift;
				lock (obj)
				{
					this.idShiftAndroid.Remove(text);
				}
				ShiftHideHandlerUI shiftHideUI = this.ShiftHideUI;
				if (shiftHideUI == null)
				{
					return;
				}
				shiftHideUI(text);
			}
		}

		private void HandleOnRemapStateChanged(BaseControllerVM controller, RemapState newState)
		{
			RemapStateChangedHandler onRemapStateChanged = this.OnRemapStateChanged;
			if (onRemapStateChanged != null)
			{
				onRemapStateChanged(controller, newState);
			}
			if (controller.IsOnline && (newState == 2 || newState == null))
			{
				EventsProxy.RemapOffWaitType remapOffWaitType = new EventsProxy.RemapOffWaitType();
				remapOffWaitType.timeRecive = DateTime.Now;
				remapOffWaitType.deviceName = controller.ControllerDisplayName;
				remapOffWaitType.ID = controller.ID;
				remapOffWaitType.waitShowRemap = true;
				object obj = EventsProxy.lockRemapWait;
				lock (obj)
				{
					this.remapOffWait[controller.ID] = remapOffWaitType;
				}
			}
		}

		private void HandleConfigAppliedToSlot(BaseControllerVM controller, GamepadProfile gamepadProfile, Slot slot, REWASD_CONTROLLER_PROFILE? profile)
		{
			ConfigAppliedToSlotHandler onConfigAppliedToSlot = this.OnConfigAppliedToSlot;
			if (onConfigAppliedToSlot != null)
			{
				onConfigAppliedToSlot(controller, gamepadProfile, slot, profile);
			}
			EventsProxy.RemapOffWaitType remapOffWaitType = new EventsProxy.RemapOffWaitType();
			remapOffWaitType.timeRecive = DateTime.Now;
			remapOffWaitType.deviceName = controller.ControllerDisplayName;
			remapOffWaitType.ID = controller.ID;
			remapOffWaitType.waitShowRemap = false;
			object obj = EventsProxy.lockRemapWait;
			lock (obj)
			{
				this.remapOffWait[controller.ID] = remapOffWaitType;
				if (this.GetIDContains(controller.ID) == null)
				{
					this.remapON.Add(controller.ID);
				}
			}
			this.CheckAndCloseShifts(controller.ID);
			ConfigAppliedToSlotHandler onConfigAppliedToSlotUI = this.OnConfigAppliedToSlotUI;
			if (onConfigAppliedToSlotUI == null)
			{
				return;
			}
			onConfigAppliedToSlotUI(controller, gamepadProfile, slot, profile);
		}

		private void HandleSlotChanged(BaseControllerVM controller, GamepadProfile gamepadProfile, Slot slot, REWASD_CONTROLLER_PROFILE? profile, bool physical)
		{
			ControllerSlotChangedHandler onSlotChanged = this.OnSlotChanged;
			if (onSlotChanged != null)
			{
				onSlotChanged(controller, gamepadProfile, slot, profile, physical);
			}
			ControllerSlotChangedHandler onSlotChangedUI = this.OnSlotChangedUI;
			if (onSlotChangedUI == null)
			{
				return;
			}
			onSlotChangedUI(controller, gamepadProfile, slot, profile, physical);
		}

		private void HandleShiftChanged(RewasdHiddenServiceCommand command, GamepadProfile gamepadProfile, string name, BaseControllerVM controller, bool toggle)
		{
			ShiftInfo shiftInfo = new ShiftInfo();
			shiftInfo.Shift = 0;
			shiftInfo.Name = name;
			if (command == null)
			{
				shiftInfo.Shift = 0;
			}
			else
			{
				if (command >= 1 && command <= 21)
				{
					shiftInfo.Shift = (int)command;
				}
				if (command >= 30 && command <= 50)
				{
					shiftInfo.Shift = (int)(command - 30 + 1);
				}
			}
			ShiftChangedHandler onShiftChanged = this.OnShiftChanged;
			if (onShiftChanged != null)
			{
				onShiftChanged(controller, shiftInfo, toggle);
			}
			this.ShiftForOverlay(shiftInfo, gamepadProfile, controller, toggle);
		}

		private void ShiftForOverlay(ShiftInfo shift, GamepadProfile gamepadProfile, BaseControllerVM controller, bool toggle)
		{
			bool flag = false;
			bool flag2 = false;
			if (shift.Shift == 0)
			{
				object obj = EventsProxy.lockShift;
				bool flag5;
				bool flag4;
				lock (obj)
				{
					flag4 = this.idShift.TryGetValue(controller.ID, out flag5);
				}
				if (flag4)
				{
					if (!flag5)
					{
						flag2 = true;
					}
					obj = EventsProxy.lockShift;
					lock (obj)
					{
						this.idShift.Remove(controller.ID);
					}
					if (!flag2)
					{
						ShiftHideHandlerUI shiftHideOverlayUI = this.ShiftHideOverlayUI;
						if (shiftHideOverlayUI != null)
						{
							shiftHideOverlayUI(controller.ID);
						}
					}
				}
			}
			else if ((toggle && Engine.UserSettingsService.ShowShiftIsChangedToggle) || (!toggle && Engine.UserSettingsService.ShowShiftIsChanged))
			{
				flag = true;
				object obj = EventsProxy.lockShift;
				lock (obj)
				{
					this.idShift[controller.ID] = Engine.UserSettingsService.NotHideShift;
				}
			}
			if (flag || flag2)
			{
				ShiftShowHandlerUI shiftShowUI = this.ShiftShowUI;
				if (shiftShowUI == null)
				{
					return;
				}
				shiftShowUI(controller, gamepadProfile, shift, toggle, flag && Engine.UserSettingsService.NotHideShift);
			}
		}

		private void HandleGamepadStateChanged(BaseControllerVM controller)
		{
			GamepadStateChanged onGamepadStateChanged = this.OnGamepadStateChanged;
			if (onGamepadStateChanged != null)
			{
				onGamepadStateChanged(controller);
			}
			if (!controller.IsOnline)
			{
				OffileDeviceHandle offileDevice = this.OffileDevice;
				if (offileDevice != null)
				{
					offileDevice(controller);
				}
				this.OnControllerRemovedForProxyUI(controller, true);
			}
		}

		private void HandleBatteryChanged(ControllerVM controller)
		{
			BatteryLevelChangedHandler onBatteryLevelChanged = this.OnBatteryLevelChanged;
			if (onBatteryLevelChanged == null)
			{
				return;
			}
			onBatteryLevelChanged(controller);
		}

		private void HandleBatteryChangedUI(ControllerVM controller)
		{
			BatteryLevelChangedHandler onBatteryLevelChangedUI = this.OnBatteryLevelChangedUI;
			if (onBatteryLevelChangedUI == null)
			{
				return;
			}
			onBatteryLevelChangedUI(controller);
		}

		private string GetIDContains(string ID)
		{
			string text = null;
			object obj = EventsProxy.lockRemapWait;
			List<string> list;
			lock (obj)
			{
				list = this.remapON.ToList<string>();
			}
			foreach (string text2 in list)
			{
				if (OverlayUtils.IsContainsInLongID(text2, ID))
				{
					text = text2;
					break;
				}
			}
			return text;
		}

		private IGamepadService gamepadService;

		private readonly IDictionary<string, EventsProxy.RemapOffWaitType> remapOffWait = new Dictionary<string, EventsProxy.RemapOffWaitType>();

		private static object lockRemapWait = new object();

		private DispatcherTimer _pollingTimerMessages;

		private List<string> remapON = new List<string>();

		private static object lockShift = new object();

		private IDictionary<string, bool> idShift = new Dictionary<string, bool>();

		private IDictionary<string, bool> idShiftAndroid = new Dictionary<string, bool>();

		private class RemapOffWaitType
		{
			public DateTime timeRecive;

			public string deviceName;

			public string ID;

			public bool waitShowRemap;
		}
	}
}
