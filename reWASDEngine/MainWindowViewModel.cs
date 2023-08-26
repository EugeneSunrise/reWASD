using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.View.SecondaryWindows;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Utils;
using XBEliteWPF.DataModels;
using XBEliteWPF.DataModels.ControllerProfileInfo;
using XBEliteWPF.DataModels.GamepadActiveProfiles;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.Controller;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.License;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtilModel;
using XBEliteWPF.ViewModels.Base;

namespace reWASDEngine
{
	internal class MainWindowViewModel : ZBindable
	{
		public static IGamepadService GamepadService
		{
			get
			{
				IGamepadService gamepadService;
				if ((gamepadService = MainWindowViewModel._gamepadService) == null)
				{
					gamepadService = (MainWindowViewModel._gamepadService = IContainerProviderExtensions.Resolve<IGamepadService>(Engine.SContainer));
				}
				return gamepadService;
			}
		}

		public static IGameProfilesService GameProfilesService
		{
			get
			{
				IGameProfilesService gameProfilesService;
				if ((gameProfilesService = MainWindowViewModel._gameProfilesService) == null)
				{
					gameProfilesService = (MainWindowViewModel._gameProfilesService = IContainerProviderExtensions.Resolve<IGameProfilesService>(Engine.SContainer));
				}
				return gameProfilesService;
			}
		}

		public static IForegroundApplicationMonitorService ForegroundApplicationMonitorService
		{
			get
			{
				IForegroundApplicationMonitorService foregroundApplicationMonitorService;
				if ((foregroundApplicationMonitorService = MainWindowViewModel._foregroundApplicationMonitorService) == null)
				{
					foregroundApplicationMonitorService = (MainWindowViewModel._foregroundApplicationMonitorService = IContainerProviderExtensions.Resolve<IForegroundApplicationMonitorService>(Engine.SContainer));
				}
				return foregroundApplicationMonitorService;
			}
		}

		public MainWindowViewModel()
		{
			MainWindowViewModel.GamepadService.GamepadCollection.CollectionChanged += this.GamepadCollectionOnCollectionChanged;
			IEventAggregator eventAggregator = IContainerProviderExtensions.Resolve<IEventAggregator>(Engine.SContainer);
			eventAggregator.GetEvent<ProfilesChanged>().Subscribe(new Action<WindowMessageEvent>(this.RefreshTrayAndGamesCollection));
			eventAggregator.GetEvent<ServiceProfilesChanged>().Subscribe(new Action<WindowMessageEvent>(this.OnServiceProfilesChanged));
			eventAggregator.GetEvent<ServiceProfileStateChanged>().Subscribe(new Action<WindowMessageEvent>(this.OnServiceProfileStateChanged));
			eventAggregator.GetEvent<AutoProfileRelationsChanged>().Subscribe(new Action<WindowMessageEvent>(this.AutoProfileRelationsChanged));
			eventAggregator.GetEvent<PreferencesChanged>().Subscribe(new Action<object>(this.OnPreferencesChanged));
			TranslationManager.Instance.LanguageChanged += delegate([Nullable(2)] object o, EventArgs e)
			{
				this.OnPropertyChanged("RemapOnOffCaption");
			};
			MainWindowViewModel.GamepadService.ServiceProfilesCollection.CollectionChanged += this.ServiceProfilesCollection_CollectionChanged;
			this.OnPreferencesChanged(null);
			this._licensingService = IContainerProviderExtensions.Resolve<ILicensingService>(Engine.SContainer);
			this.autoRemap = Convert.ToBoolean(RegistryHelper.GetValue("Config", "AutoRemap", 1, false));
			MainWindowViewModel.ForegroundApplicationMonitorService.Changed += this.ForegroundApplicationMonitorServiceOnChanged;
			this.AutoProfileRelationsChanged(null);
			MainWindowViewModel.GamepadService.ExternalDeviceRelationsHelper.CurrentExternalStateChangedEvent += delegate
			{
				this.RefreshTray(false);
				this.OnPropertyChanged("TrayToolTip");
			};
			MainWindowViewModel.GamepadService.ServiceProfilesCollection.CollectionChanged += delegate([Nullable(2)] object o, NotifyCollectionChangedEventArgs args)
			{
				this.RefreshTray(null);
			};
			eventAggregator.GetEvent<GamepadChanged>().Subscribe(new Action<WindowMessageEvent>(this.RefreshTray));
			eventAggregator.GetEvent<ProfileRelationsChangedByEngine>().Subscribe(new Action<object>(this.RefreshTray));
			if (this.autoRemap)
			{
				ObservableCollection<BaseControllerVM> gamepadCollection = MainWindowViewModel.GamepadService.GamepadCollection;
				if (gamepadCollection != null)
				{
					gamepadCollection.ToList<BaseControllerVM>().ForEach(async delegate(BaseControllerVM controller)
					{
						await this.OnControllerAdded(controller);
					});
				}
			}
			this.RefreshTray(false);
		}

		private void ServiceProfilesCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
		}

		private void OnServiceProfilesChanged(WindowMessageEvent obj)
		{
		}

		private void OnServiceProfileStateChanged(WindowMessageEvent obj)
		{
			this.RefreshTray(false);
		}

		private async Task OnControllerAdded(BaseControllerVM controller)
		{
			bool flag = false;
			GamepadProfiles gamepadProfiles = MainWindowViewModel.GamepadService.GamepadProfileRelations.TryGetValue(controller.ID);
			if (gamepadProfiles != null && gamepadProfiles.IsRemapToggled && !MainWindowViewModel.GamepadService.ServiceProfilesCollection.ContainsProfileForID(controller.ID))
			{
				if (gamepadProfiles.ControllerProfileInfoCollections == null)
				{
					gamepadProfiles.GenerateControllerInfosChain(controller, false);
				}
				Tracer.TraceWrite("GamepadCollectionOnCollectionChanged EnableRemap", false);
				Slot slot = await controller.GetCurrentSlot();
				MainWindowViewModel.GamepadService.EnableRemap(false, controller.ID, false, false, true, slot, false, false, true, null, null);
				flag = true;
			}
			if (!flag)
			{
				IEnumerable<uint> types = controller.Types;
				Func<uint, bool> func;
				if ((func = MainWindowViewModel.<>O.<0>__IsHiddenAppliedConfigControllerPhysicalType) == null)
				{
					func = (MainWindowViewModel.<>O.<0>__IsHiddenAppliedConfigControllerPhysicalType = new Func<uint, bool>(UtilsCommon.IsHiddenAppliedConfigControllerPhysicalType));
				}
				if (types.Any(func))
				{
					for (int i = 0; i < controller.Types.Length; i++)
					{
						if (UtilsCommon.IsHiddenAppliedConfigControllerPhysicalType(controller.Types[i]) && i < controller.Ids.Length)
						{
							MainWindowViewModel.GamepadService.CompileHiddenAppliedConfig(controller.Ids[i], controller.Types[i], true);
						}
					}
				}
			}
		}

		private async void GamepadCollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			Tracer.TraceWrite("GamepadCollectionOnCollectionChanged", false);
			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				Tracer.TraceWrite("GamepadCollectionOnCollectionChanged removed item", false);
				using (IEnumerator enumerator = e.OldItems.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						BaseControllerVM baseControllerVM = (BaseControllerVM)obj;
						if (MainWindowViewModel.GamepadService.GamepadProfileRelations.TryGetValue(baseControllerVM.ID) != null && MainWindowViewModel.GamepadService.ServiceProfilesCollection.ContainsProfileForID(baseControllerVM.ID))
						{
							Tracer.TraceWrite("GamepadCollectionOnCollectionChanged DisableRemap", false);
							MainWindowViewModel.GamepadService.DisableRemap(baseControllerVM.ID, false);
						}
					}
					goto IL_1B1;
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Add && this.autoRemap)
			{
				Tracer.TraceWrite("GamepadCollectionOnCollectionChanged added item", false);
				foreach (object obj2 in e.NewItems)
				{
					BaseControllerVM baseControllerVM2 = (BaseControllerVM)obj2;
					await this.OnControllerAdded(baseControllerVM2);
				}
				IEnumerator enumerator2 = null;
			}
			IL_1B1:
			this.RefreshTray(false);
			this.RemapOnOffCommand.RaiseCanExecuteChanged();
		}

		private void OnPreferencesChanged(object obj)
		{
			this.turnRemapOffOnUnsupportedApplication = Convert.ToBoolean(RegistryHelper.GetValue("Config", "TurnRemapOffOnLostFocus", 1, false));
			this.autoRemap = Convert.ToBoolean(RegistryHelper.GetValue("Config", "AutoRemap", 1, false));
			LocalizationManager.Instance.ReInit();
			TranslationManager.Instance.OnLanguageChanged();
			this.AutoProfileRelationsChanged(null);
		}

		public async void AutoProfileRelationsChanged(object payload)
		{
			await MainWindowViewModel.GameProfilesService.WaitForServiceInited();
			ThreadHelper.ExecuteInMainDispatcher(delegate
			{
				MainWindowViewModel.GameProfilesService.GamesCollection.ForEach(delegate(Game g)
				{
					g.ResolveAutoDetect();
				});
				if (MainWindowViewModel.GameProfilesService.GamesCollection.Any((Game game) => game.IsAutodetect) || (Engine.UserSettingsService.ShowDirectXOverlay && Engine.UserSettingsService.IsOverlayEnable))
				{
					MainWindowViewModel.ForegroundApplicationMonitorService.Enabled = true;
					return;
				}
				MainWindowViewModel.ForegroundApplicationMonitorService.Enabled = false;
			}, true);
		}

		private async void ForegroundApplicationMonitorServiceOnChanged(object sender, EventArgs eventArgs)
		{
			if (MainWindowViewModel.ForegroundApplicationMonitorService.Process != null)
			{
				AsyncLock.Releaser releaser = await new AsyncLock(this._foregroundApplicationMonitorSemaphore).LockAsync();
				using (releaser)
				{
					string procName;
					try
					{
						procName = MainWindowViewModel._foregroundApplicationMonitorService.Process.ProcessName;
					}
					catch (Exception)
					{
						return;
					}
					procName = procName.ToLower();
					if (this.prevProcName == procName)
					{
						return;
					}
					this.prevProcName = procName;
					if (procName == this.curRemapedForegroundName)
					{
						return;
					}
					Tracer.TraceWrite("ForegroundApplicationMonitorServiceOnChanged Detected foreground process change ProcessName: " + procName, false);
					GamepadProfilesCollection gpra = this.GetAutoGamepadProfilesCollection(procName);
					await this.DisableCurrentAppRemapOnFocusLostIfNeeded();
					if (gpra == null)
					{
						return;
					}
					using (IEnumerator<KeyValuePair<string, GamepadProfiles>> enumerator = ((IEnumerable<KeyValuePair<string, GamepadProfiles>>)gpra).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<string, GamepadProfiles> gamepadProfiles = enumerator.Current;
							BaseControllerVM baseControllerVM = MainWindowViewModel.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM item) => item.ID == gamepadProfiles.Key);
							GamepadProfiles gpr = MainWindowViewModel.GamepadService.GamepadProfileRelations.TryGetValue(gamepadProfiles.Key);
							if (gpr == null)
							{
								MainWindowViewModel.GamepadService.GamepadProfileRelations[gamepadProfiles.Key] = gpra.TryGetValue(gamepadProfiles.Key).Clone();
								gpr = MainWindowViewModel.GamepadService.GamepadProfileRelations[gamepadProfiles.Key];
								if (gpr.ControllerProfileInfoCollections == null)
								{
									gpr.ID = gamepadProfiles.Key;
									if (baseControllerVM != null)
									{
										gpr.GenerateControllerInfosChain(baseControllerVM, false);
									}
								}
							}
							Slot? slot = null;
							bool flag = false;
							foreach (KeyValuePair<Slot, GamepadProfile> keyValuePair in ((IEnumerable<KeyValuePair<Slot, GamepadProfile>>)gamepadProfiles.Value.SlotProfiles))
							{
								if (keyValuePair.Value != null)
								{
									gpr.SlotProfiles[keyValuePair.Key] = keyValuePair.Value;
									if (slot == null)
									{
										slot = new Slot?(keyValuePair.Key);
									}
									flag = true;
								}
							}
							ushort num = 0;
							if (flag)
							{
								if (baseControllerVM != null)
								{
									num = await MainWindowViewModel.GamepadService.EnableRemap(true, gamepadProfiles.Key, true, false, true, (slot == null) ? (-1) : slot.Value, false, false, true, null, null);
								}
								gpr.IsRemapToggled = true;
								this.curRemapedForegroundName = procName;
							}
							this.curRemapedProfileId = num;
							gpr = null;
						}
					}
					IEnumerator<KeyValuePair<string, GamepadProfiles>> enumerator = null;
					procName = null;
					gpra = null;
				}
				AsyncLock.Releaser releaser2 = default(AsyncLock.Releaser);
				Tracer.TraceWrite("ForegroundApplicationMonitorServiceOnChanged ended", false);
			}
		}

		private GamepadProfilesCollection GetAutoGamepadProfilesCollection(string processName)
		{
			Func<string, bool> <>9__4;
			Func<SubConfigData, bool> <>9__3;
			Func<Config, bool> <>9__2;
			Game game = MainWindowViewModel.GameProfilesService.GamesCollection.Where((Game g) => g.IsAutodetect).ToList<Game>().FirstOrDefault(delegate(Game g)
			{
				if (g.ConfigCollection.Any<Config>())
				{
					IEnumerable<Config> configCollection = g.ConfigCollection;
					Func<Config, bool> func;
					if ((func = <>9__2) == null)
					{
						func = (<>9__2 = delegate(Config c)
						{
							IEnumerable<SubConfigData> configData = c.ConfigData;
							Func<SubConfigData, bool> func2;
							if ((func2 = <>9__3) == null)
							{
								func2 = (<>9__3 = delegate(SubConfigData bc)
								{
									IEnumerable<string> processNames = bc.MainXBBindingCollection.ProcessNames;
									Func<string, bool> func3;
									if ((func3 = <>9__4) == null)
									{
										func3 = (<>9__4 = (string p) => p.ToLower().StartsWith(processName));
									}
									return processNames.Any(func3);
								});
							}
							return configData.Any(func2);
						});
					}
					return configCollection.Any(func);
				}
				return false;
			});
			if (game == null)
			{
				Tracer.TraceWrite("GetAutoGamepadProfilesCollection no game founded", false);
				return null;
			}
			return MainWindowViewModel.GamepadService.AutoGamesDetectionGamepadProfileRelations.TryGetValue(game.Name);
		}

		private async Task DisableCurrentAppRemapOnFocusLostIfNeeded()
		{
			Tracer.TraceWrite("DisableCurrentAppRemapOnFocusLostIfNeeded", false);
			if (this.curRemapedForegroundName != null)
			{
				Tracer.TraceWrite("curRemapedForegroundName: " + this.curRemapedForegroundName, false);
				GamepadProfilesCollection autoGamepadProfilesCollection = this.GetAutoGamepadProfilesCollection(this.curRemapedForegroundName);
				if (autoGamepadProfilesCollection == null)
				{
					Tracer.TraceWrite("DisableCurrentAppRemapOnFocusLostIfNeeded no game founded", false);
					return;
				}
				Tracer.TraceWrite("curRemapGame found iteraiting for slot collection", false);
				foreach (KeyValuePair<string, GamepadProfiles> keyValuePair in ((IEnumerable<KeyValuePair<string, GamepadProfiles>>)autoGamepadProfilesCollection))
				{
					if (MainWindowViewModel.GamepadService.GamepadProfileRelations.TryGetValue(keyValuePair.Key) == null)
					{
						this.curRemapedForegroundName = null;
						this.curRemapedProfileId = 0;
						return;
					}
					if (this.turnRemapOffOnUnsupportedApplication)
					{
						List<Slot> list = new List<Slot>();
						foreach (KeyValuePair<Slot, GamepadProfile> keyValuePair2 in ((IEnumerable<KeyValuePair<Slot, GamepadProfile>>)keyValuePair.Value.SlotProfiles))
						{
							list.Add(keyValuePair2.Key);
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 1);
						defaultInterpolatedStringHandler.AppendLiteral("slotsToReset count: ");
						defaultInterpolatedStringHandler.AppendFormatted<int>(list.Count);
						Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
						await MainWindowViewModel.GamepadService.RestoreDefaults(keyValuePair.Key, list);
					}
				}
				IEnumerator<KeyValuePair<string, GamepadProfiles>> enumerator = null;
				this.curRemapedForegroundName = null;
				this.curRemapedProfileId = 0;
			}
			Tracer.TraceWrite("DisableCurrentAppRemapOnFocusLostIfNeeded ended", false);
		}

		public void UpdateLicense(bool force)
		{
			this._licensingService.CheckLicenseAsync(force);
		}

		public void RefreshTray(object payload)
		{
			this.OnPropertyChanged("RemapOnOffCaption");
			this.RefreshTray(false);
			this.RemapOnOffCommand.RaiseCanExecuteChanged();
		}

		public void RefreshTrayAndGamesCollection(object payload)
		{
			this.OnPropertyChanged("RemapOnOffCaption");
			this.RefreshTray(false);
			MainWindowViewModel.GameProfilesService.FillGamesCollection();
		}

		public void RefreshTrayAndGamepadsCollection(object payload)
		{
			this.OnPropertyChanged("RemapOnOffCaption");
			this.RefreshTray(false);
			this.RemapOnOffCommand.RaiseCanExecuteChanged();
		}

		public void RefreshTray(bool force = false)
		{
			if (force)
			{
				this.TrayIcon = null;
			}
			ThreadHelper.ExecuteInMainDispatcher(delegate
			{
				this.TrayIcon = this.DefaultTrayIcon;
			}, true);
		}

		public ImageSource TrayIcon
		{
			get
			{
				return this._trayIcon;
			}
			set
			{
				this.SetProperty<ImageSource>(ref this._trayIcon, value, "TrayIcon");
			}
		}

		public string TrayToolTip
		{
			get
			{
				string currentExternalStateAgentTooltip = MainWindowViewModel.GamepadService.ExternalDeviceRelationsHelper.CurrentExternalStateAgentTooltip;
				if (string.IsNullOrEmpty(currentExternalStateAgentTooltip))
				{
					return "reWASD";
				}
				return currentExternalStateAgentTooltip;
			}
		}

		public ImageSource DefaultTrayIcon
		{
			get
			{
				if (!MainWindowViewModel.GamepadService.IsAnyGamepadConnected)
				{
					return Application.Current.TryFindResource("IcoControllerEmpty") as ImageSource;
				}
				if (MainWindowViewModel.GamepadService.IsAnyGamepadRemaped)
				{
					ExternalStateAgent currentExternalStateAgent = MainWindowViewModel.GamepadService.ExternalDeviceRelationsHelper.CurrentExternalStateAgent;
					string text;
					if (currentExternalStateAgent != 1)
					{
						if (currentExternalStateAgent != 2)
						{
							text = "IcoRemapIsOn";
						}
						else
						{
							text = "IcoControllerError";
						}
					}
					else
					{
						text = "IcoControllerExternal";
					}
					return Application.Current.TryFindResource(text) as ImageSource;
				}
				return Application.Current.TryFindResource("IcoRemapIsOff") as ImageSource;
			}
		}

		public string RemapOnOffCaption
		{
			get
			{
				if (MainWindowViewModel.GamepadService.IsAnyGamepadRemaped)
				{
					return DTLocalization.GetString(11562);
				}
				return DTLocalization.GetString(11563);
			}
		}

		private void Autoremap()
		{
			MainWindowViewModel.GamepadService.BinDataSerialize.LoadGamepadProfileRelations(true);
			if (MainWindowViewModel.GamepadService.GamepadProfileRelations.IsGlobalRemapToggled)
			{
				MainWindowViewModel.GamepadService.EnableRemap(false, null, false, true, true, -1, false, false, true, null, null);
			}
		}

		public ICommand VisitCommunityCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._visitCommunityCommand) == null)
				{
					relayCommand = (this._visitCommunityCommand = new RelayCommand(new Action(this.VisitCommunity)));
				}
				return relayCommand;
			}
		}

		public void VisitCommunity()
		{
			Process.Start(new ProcessStartInfo("https://www.rewasd.com/community/")
			{
				UseShellExecute = true
			});
		}

		public DelegateCommand<ControllerVM> AdjustAgentSettingsCommand
		{
			get
			{
				DelegateCommand<ControllerVM> delegateCommand;
				if ((delegateCommand = this._adjustAgentSettingsCommand) == null)
				{
					delegateCommand = (this._adjustAgentSettingsCommand = new DelegateCommand<ControllerVM>(new Action<ControllerVM>(this.AdjustAgentSettings)));
				}
				return delegateCommand;
			}
		}

		private async Task<bool> CheckLicense()
		{
			TaskAwaiter<bool> taskAwaiter = LicenseMainVM.IsTrialExpired().GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<bool> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			bool flag;
			if (taskAwaiter.GetResult())
			{
				this.OpenGui();
				flag = false;
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		public async void AdjustAgentSettings(ControllerVM controller)
		{
			TaskAwaiter<bool> taskAwaiter = this.CheckLicense().GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<bool> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			if (taskAwaiter.GetResult())
			{
				RegistryHelper.SetString("GuiNamespace", "SelectedController", controller.ID);
				XBUtils.RunReWASDGui("/OpenAgentSettings");
			}
		}

		public DelegateCommand RemapOnOffCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._remapOnOffCommand) == null)
				{
					delegateCommand = (this._remapOnOffCommand = new DelegateCommand(new Action(this.RemapOnOff), new Func<bool>(this.CanExecuteRemapOnOff)));
				}
				return delegateCommand;
			}
		}

		public bool CanExecuteRemapOnOff()
		{
			if (!MainWindowViewModel.GamepadService.IsAnyGamepadConnected)
			{
				return false;
			}
			if (MainWindowViewModel.GamepadService.IsAnyGamepadRemaped)
			{
				return true;
			}
			List<BaseControllerVM> gamepadCollection = MainWindowViewModel.GamepadService.GamepadCollection.ToList<BaseControllerVM>();
			return MainWindowViewModel.GamepadService.GamepadProfileRelations.Any((KeyValuePair<string, GamepadProfiles> gpr) => gamepadCollection.FirstOrDefault((BaseControllerVM g) => object.Equals(gpr.Key, g.ID)) != null && gpr.Value.SlotProfiles.Any<KeyValuePair<Slot, GamepadProfile>>());
		}

		public async void RemapOnOff()
		{
			if (!MainWindowViewModel.GamepadService.IsAnyGamepadRemaped)
			{
				if (!MainWindowViewModel.GamepadService.GamepadProfileRelations.Any((KeyValuePair<string, GamepadProfiles> item) => item.Value.SlotProfiles.Any<KeyValuePair<Slot, GamepadProfile>>()))
				{
					this.OpenGui();
					return;
				}
			}
			if (!MainWindowViewModel.GamepadService.IsAnyGamepadRemaped)
			{
				TaskAwaiter<bool> taskAwaiter = this.CheckLicense().GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<bool> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				if (taskAwaiter.GetResult())
				{
					await MainWindowViewModel.GamepadService.BinDataSerialize.LoadGamepadProfileRelations(true);
					await MainWindowViewModel.GamepadService.EnableRemap(false, null, true, false, true, -1, false, false, true, null, null);
				}
			}
			else
			{
				await MainWindowViewModel.GamepadService.DisableRemap(null, true);
			}
		}

		public ICommand OpenGuiCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._openGui) == null)
				{
					relayCommand = (this._openGui = new RelayCommand(new Action(this.OpenGui)));
				}
				return relayCommand;
			}
		}

		private void OpenGui()
		{
			XBUtils.RunReWASDGui("");
		}

		public DelegateCommand<bool?> CloseCommand
		{
			get
			{
				DelegateCommand<bool?> delegateCommand;
				if ((delegateCommand = this._close) == null)
				{
					delegateCommand = (this._close = new DelegateCommand<bool?>(new Action<bool?>(this.Close)));
				}
				return delegateCommand;
			}
		}

		public void Close(bool? verbose = true)
		{
			bool? flag = verbose;
			bool flag2 = false;
			MessageBoxResult messageBoxResult = (((flag.GetValueOrDefault() == flag2) & (flag != null)) ? MessageBoxResult.Yes : MessageBoxWithDoNotShowLogic.Show(Application.Current.MainWindow, DTLocalization.GetString(11460), MessageBoxButton.YesNo, MessageBoxImage.Exclamation, "ExitTrayAgentModified", MessageBoxResult.Yes, false, 0.0, null, null, null, null, null, null));
			if ((messageBoxResult == MessageBoxResult.Yes || messageBoxResult == MessageBoxResult.OK) && MainWindowViewModel.TryCloseGUI())
			{
				MainWindowViewModel.ForegroundApplicationMonitorService.Changed -= this.ForegroundApplicationMonitorServiceOnChanged;
				Application.Current.Shutdown();
			}
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int BroadcastSystemMessage(uint flags, ref uint lpInfo, uint Msg, uint wParam, uint lParam);

		[DllImport("User32.dll")]
		public static extern uint RegisterWindowMessage(string str);

		public static void SendCloseGUI(string msgName)
		{
			uint num = MainWindowViewModel.RegisterWindowMessage(msgName);
			uint num2 = 24U;
			MainWindowViewModel.BroadcastSystemMessage(50U, ref num2, num, 0U, 0U);
		}

		public static bool GUIIsNotRunning(string mutexName)
		{
			bool flag = true;
			bool flag2 = false;
			while (flag)
			{
				try
				{
					WaitHandle waitHandle = new Mutex(true, mutexName, out flag2);
					flag = false;
					waitHandle.Close();
				}
				catch (Exception)
				{
				}
				if (!flag2)
				{
					flag = DTMessageBox.Show(string.Format(DTLocalization.GetString(4488), "reWASD"), "reWASD", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
				}
			}
			return flag2;
		}

		public static bool TryCloseGUI()
		{
			MainWindowViewModel.SendCloseGUI("DiscSoftReWASD_msg_uninstall");
			Thread.Sleep(1000);
			return MainWindowViewModel.GUIIsNotRunning("Local\\{DBCA738F-A4CB-4da8-59D2-7BC90CB671EA}");
		}

		private static IGamepadService _gamepadService;

		public static IGameProfilesService _gameProfilesService;

		private static IForegroundApplicationMonitorService _foregroundApplicationMonitorService;

		private ILicensingService _licensingService;

		private bool turnRemapOffOnUnsupportedApplication;

		private bool autoRemap;

		private AsyncSemaphore _foregroundApplicationMonitorSemaphore = new AsyncSemaphore(1);

		private string curRemapedForegroundName;

		private ushort curRemapedProfileId;

		private string prevProcName = "";

		private ImageSource _trayIcon;

		private RelayCommand _visitCommunityCommand;

		private DelegateCommand<ControllerVM> _adjustAgentSettingsCommand;

		private DelegateCommand _remapOnOffCommand;

		private RelayCommand _openGui;

		private DelegateCommand<bool?> _close;

		[CompilerGenerated]
		private static class <>O
		{
			public static Func<uint, bool> <0>__IsHiddenAppliedConfigControllerPhysicalType;
		}
	}
}
