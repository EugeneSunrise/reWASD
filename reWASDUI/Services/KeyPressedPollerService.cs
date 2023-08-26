using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using DiscSoft.NET.Common.Utils;
using LaunchDarkly.EventSource;
using Newtonsoft.Json;
using Prism.Events;
using reWASDCommon.Utils;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Services.Interfaces;

namespace reWASDUI.Services
{
	public class KeyPressedPollerService : IKeyPressedPollerService
	{
		public bool IsPollingAllowedViaSettings
		{
			get
			{
				return this._isPollingAllowedViaSettings;
			}
			set
			{
				this._isPollingAllowedViaSettings = value;
				if (!value)
				{
					this.StopPolling();
					return;
				}
				this.StartPolling(true);
			}
		}

		private bool IsWindowActive
		{
			get
			{
				return Application.Current.Windows.OfType<Window>().SingleOrDefault((Window w) => w.IsActive) != null;
			}
		}

		public KeyPressedPollerService(IEventAggregator ea, IDeviceDetectionService dds)
		{
			Tracer.TraceWrite("Constructor for KeyPressedPollerService", false);
			this.IsPollingAllowedViaSettings = Convert.ToBoolean(RegistryHelper.GetValue("Config", "HookControllerButtons", 1, false));
			this._eventAggregator = ea;
			this._mainPollThread = new KeyPressedPollerService.KeyPollThread(() => dds.IsEnabled);
			this._onlyInCurrentApp = true;
		}

		public void Subscribe(IKeyPressedEventHandler listener, bool exclusive = false)
		{
			if (exclusive)
			{
				this._mainPollThread.ExclusiveListener = listener;
			}
			if (!this._mainPollThread.Listeners.Contains(listener))
			{
				this._mainPollThread.Listeners.Add(listener);
				if (this._mainPollThread.Listeners.Count == 1)
				{
					this._eventAggregator.GetEvent<CurrentGamepadChanged>().Subscribe(new Action<BaseControllerVM>(this.OnCurrentGamepadChanged));
					this.OnCurrentGamepadChanged(App.GamepadService.CurrentGamepad);
				}
			}
		}

		public void Unsubscribe(IKeyPressedEventHandler listener)
		{
			if (this._mainPollThread.ExclusiveListener == listener)
			{
				this._mainPollThread.ExclusiveListener = null;
			}
			if (this._mainPollThread.Listeners.Contains(listener))
			{
				this._mainPollThread.Listeners.Remove(listener);
			}
		}

		public void StartPolling(bool onlyInCurrentApp = true)
		{
			if (!this.IsPollingAllowedViaSettings)
			{
				return;
			}
			if (this._currentPolledGamepad != null)
			{
				this._currentPolledGamepad.OnRemapStateChanged -= this.CurrentPolledGamepad_OnRemapStateChanged;
				this._currentPolledGamepad.OnRemapStateChanged += this.CurrentPolledGamepad_OnRemapStateChanged;
				if (this._currentPolledGamepad.RemapState != 1)
				{
					this._onlyInCurrentApp = onlyInCurrentApp;
					this._isPollingPaused = false;
					this._mainPollThread.StartThread(this._currentPolledGamepad, false);
				}
			}
		}

		public void StopPolling()
		{
			if (this._currentPolledGamepad != null)
			{
				this._currentPolledGamepad.OnRemapStateChanged -= this.CurrentPolledGamepad_OnRemapStateChanged;
				this._mainPollThread.StopThread();
			}
		}

		private void CurrentPolledGamepad_OnRemapStateChanged()
		{
			if (this._currentPolledGamepad.RemapState != 1)
			{
				this.StartPolling(true);
				return;
			}
			this.StopPolling();
			this._currentPolledGamepad.OnRemapStateChanged += this.CurrentPolledGamepad_OnRemapStateChanged;
		}

		public async void StartAllGamepadsPolling()
		{
			if (this.IsPollingAllowedViaSettings)
			{
				this._onlyInCurrentApp = false;
				this._isPollingPaused = false;
				this._mainPollThread.StopThread();
				await Task.Delay(100);
				this._mainPollThread.StartThread(null, true);
			}
		}

		public async void StopAllGamepadsPolling()
		{
			this._mainPollThread.StopThread();
			await Task.Delay(100);
			this.StartPolling(true);
		}

		private void TryResumePolling()
		{
			if (!this._isPollingPaused)
			{
				this.StartPolling(true);
			}
		}

		public bool SuspendPollingUntilStarted()
		{
			if (this._isPollingPaused)
			{
				return false;
			}
			this._isPollingPaused = true;
			this.StopPolling();
			return true;
		}

		private void OnCurrentGamepadChanged(BaseControllerVM newGamepad)
		{
			this.StopPolling();
			this._currentPolledGamepad = newGamepad;
			if (this.IsWindowActive || !this._onlyInCurrentApp)
			{
				this.StartPolling(true);
			}
		}

		private const int POLL_INTERVAL = 75;

		private IEventAggregator _eventAggregator;

		private KeyPressedPollerService.KeyPollThread _mainPollThread;

		private BaseControllerVM _currentPolledGamepad;

		private bool _isPollingPaused;

		private bool _isPollingAllowedViaSettings;

		private bool _onlyInCurrentApp;

		private class KeyPollThread
		{
			public List<IKeyPressedEventHandler> Listeners { get; set; } = new List<IKeyPressedEventHandler>();

			public IKeyPressedEventHandler ExclusiveListener { get; set; }

			private bool IsRunning
			{
				get
				{
					return this._eventSource != null;
				}
			}

			public KeyPollThread(KeyPressedPollerService.KeyPollThread.LockSendEvents lockSend)
			{
				this._lockSend = lockSend;
			}

			public void OnMessageReceived(object sender, MessageReceivedEventArgs args)
			{
				if (args.Message.Name == "UIEvent" || args.Message.Name == "DataEvent")
				{
					return;
				}
				if (args.Message.Name != "GamepadState")
				{
					return;
				}
				if (this._lockSend())
				{
					return;
				}
				string data = args.Message.Data;
				try
				{
					GamepadState gamepadState = JsonConvert.DeserializeObject<GamepadState>(data);
					List<GamepadButtonDescription> list = this.keyPressedList.Where((GamepadButtonDescription item) => !gamepadState.PressedButtons.Any((GamepadButtonDescription kp) => kp.Button == item.Button)).ToList<GamepadButtonDescription>();
					List<GamepadButtonDescription> list2 = gamepadState.PressedButtons.Where((GamepadButtonDescription item) => !this.keyPressedList.Any((GamepadButtonDescription keyPressed) => keyPressed.Button == item.Button)).ToList<GamepadButtonDescription>();
					bool flag = list.Any<GamepadButtonDescription>();
					bool flag2 = list2.Any<GamepadButtonDescription>();
					if (this.ExclusiveListener != null)
					{
						if (flag)
						{
							this.ExclusiveListener.OnControllerKeyUp(list);
						}
						if (flag2)
						{
							this.ExclusiveListener.OnControllerKeyDown(list2);
						}
						this.ExclusiveListener.OnControllerPollState(gamepadState);
					}
					else
					{
						for (int i = this.Listeners.Count - 1; i >= 0; i--)
						{
							if (flag)
							{
								this.Listeners[i].OnControllerKeyUp(list);
							}
							if (flag2)
							{
								this.Listeners[i].OnControllerKeyDown(list2);
							}
							this.Listeners[i].OnControllerPollState(gamepadState);
						}
					}
					this.keyPressedList = gamepadState.PressedButtons;
				}
				catch (Exception)
				{
				}
			}

			public void StartThread(BaseControllerVM currentGamepad, bool allGamepads = false)
			{
				if (!this.IsRunning)
				{
					if (this.Listeners.Count == 0)
					{
						return;
					}
					this._forAllGamepads = allGamepads;
					this.CreateEventSource((currentGamepad != null) ? currentGamepad.ID : null, allGamepads);
				}
			}

			private void CreateEventSource(string controllerId, bool allGamepads)
			{
				try
				{
					int port = HttpServerSettings.GetPort();
					string actualLocalRoute = HttpServerSettings.GetActualLocalRoute();
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 3);
					defaultInterpolatedStringHandler.AppendLiteral("http://");
					defaultInterpolatedStringHandler.AppendFormatted(actualLocalRoute);
					defaultInterpolatedStringHandler.AppendLiteral(":");
					defaultInterpolatedStringHandler.AppendFormatted<int>(port);
					defaultInterpolatedStringHandler.AppendLiteral("/");
					defaultInterpolatedStringHandler.AppendFormatted("v1.7");
					defaultInterpolatedStringHandler.AppendLiteral("/KeyPressedPoller/");
					Configuration configuration = Configuration.Builder(new Uri(defaultInterpolatedStringHandler.ToStringAndClear() + (allGamepads ? "all" : controllerId))).Build();
					this._eventSource = new EventSource(configuration);
					this._eventSource.MessageReceived += this.OnMessageReceived;
					this._eventSource.StartAsync();
				}
				catch (Exception ex)
				{
					Tracer.TraceWrite("Failed to run SSE", false);
					Tracer.TraceException(ex, "CreateEventSource");
				}
			}

			public void StopThread()
			{
				EventSource eventSource = this._eventSource;
				if (eventSource != null)
				{
					eventSource.Close();
				}
				this._eventSource = null;
			}

			private KeyPressedPollerService.KeyPollThread.LockSendEvents _lockSend;

			private EventSource _eventSource;

			private bool _forAllGamepads;

			private List<GamepadButtonDescription> keyPressedList = new List<GamepadButtonDescription>();

			public delegate bool LockSendEvents();
		}
	}
}
