using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using DiscSoft.NET.Common.DSBaseApp;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Tracing.LogmanTracing;
using DiscSoft.NET.Common.View.SecondaryWindows;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using DiscSoft.NET.Common.View.SecondaryWindows.WaitDialog;
using Microsoft.Win32;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using reWASDCommon;
using reWASDCommon.Infrastructure;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Utils;
using reWASDUI.Controls.XBBindingControls.BindingFrame.BindingFrameViews;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Services;
using reWASDUI.Services.Interfaces;
using reWASDUI.Utils;
using reWASDUI.ViewModels;
using reWASDUI.ViewModels.Preferences;
using reWASDUI.Views;
using reWASDUI.Views.ContentZoneGamepad;
using reWASDUI.Views.ContentZoneGamepad.AdvancedStick;
using reWASDUI.Views.ContentZoneGamepad.Macro;
using reWASDUI.Views.OverlayMenu;
using reWASDUI.Views.Preferences;
using reWASDUI.Views.VirtualSticks;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtilModel;
using XBEliteWPF.ViewModels.Base;

namespace reWASDUI
{
	public partial class App : DSBaseApplication
	{
		public static IGameProfilesService GameProfilesService
		{
			get
			{
				IGameProfilesService gameProfilesService;
				if ((gameProfilesService = App._gameProfilesService) == null)
				{
					gameProfilesService = (App._gameProfilesService = IContainerProviderExtensions.Resolve<IGameProfilesService>(App.Container));
				}
				return gameProfilesService;
			}
		}

		public static IEventAggregator EventAggregator
		{
			get
			{
				IEventAggregator eventAggregator;
				if ((eventAggregator = App._eventAggregator) == null)
				{
					eventAggregator = (App._eventAggregator = IContainerProviderExtensions.Resolve<IEventAggregator>(App.Container));
				}
				return eventAggregator;
			}
		}

		public static IConfigFileService ConfigFileService
		{
			get
			{
				IConfigFileService configFileService;
				if ((configFileService = App._configFileService) == null)
				{
					configFileService = (App._configFileService = IContainerProviderExtensions.Resolve<IConfigFileService>(App.Container));
				}
				return configFileService;
			}
		}

		public static IKeyBindingService KeyBindingService
		{
			get
			{
				IKeyBindingService keyBindingService;
				if ((keyBindingService = App._keyBindingService) == null)
				{
					keyBindingService = (App._keyBindingService = IContainerProviderExtensions.Resolve<IKeyBindingService>(App.Container));
				}
				return keyBindingService;
			}
		}

		public static IGamepadService GamepadService
		{
			get
			{
				IGamepadService gamepadService;
				if ((gamepadService = App._gamepadService) == null)
				{
					gamepadService = (App._gamepadService = IContainerProviderExtensions.Resolve<IGamepadService>(App.Container));
				}
				return gamepadService;
			}
		}

		public static IAdminOperations AdminOperations
		{
			get
			{
				IAdminOperations adminOperations;
				if ((adminOperations = App._adminOperations) == null)
				{
					adminOperations = (App._adminOperations = IContainerProviderExtensions.Resolve<IAdminOperations>(App.Container));
				}
				return adminOperations;
			}
		}

		public static Lazy<IGamepadService> GamepadServiceLazy
		{
			get
			{
				Lazy<IGamepadService> lazy;
				if ((lazy = App._gamepadServiceLazy) == null)
				{
					lazy = (App._gamepadServiceLazy = IContainerProviderExtensions.Resolve<Lazy<IGamepadService>>(App.Container));
				}
				return lazy;
			}
		}

		public static ILicensingService LicensingService
		{
			get
			{
				ILicensingService licensingService;
				if ((licensingService = App._licensingService) == null)
				{
					licensingService = (App._licensingService = IContainerProviderExtensions.Resolve<ILicensingService>(App.Container));
				}
				return licensingService;
			}
		}

		public static IDeviceDetectionService DeviceDetectionService
		{
			get
			{
				IDeviceDetectionService deviceDetectionService;
				if ((deviceDetectionService = App._deviceDetectionService) == null)
				{
					deviceDetectionService = (App._deviceDetectionService = IContainerProviderExtensions.Resolve<IDeviceDetectionService>(App.Container));
				}
				return deviceDetectionService;
			}
		}

		public static IKeyPressedPollerService KeyPressedPollerService
		{
			get
			{
				IKeyPressedPollerService keyPressedPollerService;
				if ((keyPressedPollerService = App._keyPressedPollerService) == null)
				{
					keyPressedPollerService = (App._keyPressedPollerService = IContainerProviderExtensions.Resolve<IKeyPressedPollerService>(App.Container));
				}
				return keyPressedPollerService;
			}
		}

		public static IWindowsMessageProcessor WindowsMessageProcessor
		{
			get
			{
				IWindowsMessageProcessor windowsMessageProcessor;
				if ((windowsMessageProcessor = App._windowsMessageProcessor) == null)
				{
					windowsMessageProcessor = (App._windowsMessageProcessor = IContainerProviderExtensions.Resolve<IWindowsMessageProcessor>(App.Container));
				}
				return windowsMessageProcessor;
			}
		}

		public static IUserSettingsService UserSettingsService
		{
			get
			{
				IUserSettingsService userSettingsService;
				if ((userSettingsService = App._userSettingsService) == null)
				{
					userSettingsService = (App._userSettingsService = IContainerProviderExtensions.Resolve<IUserSettingsService>(App.Container));
				}
				return userSettingsService;
			}
		}

		public static IHttpClientService HttpClientService
		{
			get
			{
				IHttpClientService httpClientService;
				if ((httpClientService = App._httpClientService) == null)
				{
					httpClientService = (App._httpClientService = IContainerProviderExtensions.Resolve<IHttpClientService>(App.Container));
				}
				return httpClientService;
			}
		}

		public static MainContentVM MainContentVM
		{
			get
			{
				MainContentVM mainContentVM;
				if ((mainContentVM = App._mainContentVM) == null)
				{
					mainContentVM = (App._mainContentVM = IContainerProviderExtensions.Resolve<MainContentVM>(App.Container));
				}
				return mainContentVM;
			}
		}

		public static IGuiHelperService GuiHelperService
		{
			get
			{
				if (App._guiHelperInited)
				{
					return App._guiHelperService;
				}
				App._guiHelperInited = true;
				try
				{
					App._guiHelperService = IContainerProviderExtensions.Resolve<IGuiHelperService>(App.Container);
				}
				catch (Exception)
				{
				}
				return App._guiHelperService;
			}
		}

		[DllImport("User32.dll")]
		private static extern IntPtr PostMessage(int hWnd, uint Msg, UIntPtr wParam, UIntPtr lParam);

		[DllImport("User32.dll")]
		public static extern uint RegisterWindowMessage(string str);

		public static IRegionManager RegionManager
		{
			get
			{
				IRegionManager regionManager;
				if ((regionManager = App._regionManager) == null)
				{
					regionManager = (App._regionManager = IContainerProviderExtensions.Resolve<IRegionManager>(App.Container));
				}
				return regionManager;
			}
		}

		public static MainWindow m_MainWnd { get; set; }

		[DllImport("user32.dll")]
		private static extern bool AllowSetForegroundWindow(int procID);

		protected override async void OnStartup(StartupEventArgs e)
		{
			this._args = e;
			Tracer.PRODUCT_SHORT_NAME = "reWASD";
			this.InitializeComponent();
			this.InitLocalization();
			new AdminOperationsService().DeleteNonVolatileRebootKeys();
			if (DSBaseApplication.IsRebootRequired)
			{
				new AdminOperationsDecider().DeleteNonVolatileRebootKeys();
				if (DSBaseApplication.IsRebootRequired)
				{
					DTMessageBox.Show(DTLocalization.GetString(64063), MessageBoxButton.OK, MessageBoxImage.Exclamation, null, false, MessageBoxResult.None);
					Environment.Exit(3010);
				}
			}
			try
			{
				App.mutex = new Mutex(true, "Local\\{DBCA738F-A4CB-4da8-59D2-7BC90CB671EA}", out App.bMutexInitiallyOwned);
			}
			catch (Exception)
			{
				DTMessageBox.Show(DTLocalization.GetString(11001), DTLocalization.GetString(11000), MessageBoxButton.OK, MessageBoxImage.Hand);
				base.Shutdown();
				return;
			}
			if (!App.bMutexInitiallyOwned)
			{
				App.AllowSetForegroundWindow(-1);
				int num = 65535;
				uint num2 = App.RegisterWindowMessage("XBOXEliteWPFWPFRestoreWindow");
				App.PostMessage(num, num2, (UIntPtr)((IntPtr)this.GetCommandLineParam(e.Args)), UIntPtr.Zero);
				base.Shutdown();
			}
			else
			{
				await this.CheckEngineRunning();
				XBUtils.LoadGlobalLibsDll();
				base.OnStartup(e);
				ThemeManager.Instance.SetCurrentSVGTheme();
				App.InitInfrastructure();
				this.CreateRecreateMainWindow(false);
			}
		}

		protected override Window CreateShell()
		{
			return IContainerProviderExtensions.Resolve<MainWindow>(App.Container);
		}

		protected override void RegisterTypes(IContainerRegistry containerRegistry)
		{
			App.Container = base.Container;
			IContainerRegistryExtensions.RegisterForNavigation<MainContent>(containerRegistry, typeof(MainContent).FullName);
			IContainerRegistryExtensions.RegisterForNavigation<LicenseMain>(containerRegistry, typeof(LicenseMain).FullName);
			IContainerRegistryExtensions.RegisterForNavigation<CommunityConfigsView>(containerRegistry, typeof(CommunityConfigsView).FullName);
			IContainerRegistryExtensions.RegisterForNavigation<PreferencesWindow>(containerRegistry, typeof(PreferencesWindow).FullName);
			IContainerRegistryExtensions.RegisterForNavigation<MaskView>(containerRegistry, typeof(MaskView).FullName);
			IContainerRegistryExtensions.RegisterForNavigation<KeyboardMappingView>(containerRegistry, typeof(KeyboardMappingView).FullName);
			IContainerRegistryExtensions.RegisterForNavigation<MouseMappingView>(containerRegistry, typeof(MouseMappingView).FullName);
			IContainerRegistryExtensions.RegisterForNavigation<CompositeDeviceBlankView>(containerRegistry, typeof(CompositeDeviceBlankView).FullName);
			IContainerRegistryExtensions.RegisterForNavigation<SVGGamepadWithAllAnnotations>(containerRegistry, typeof(SVGGamepadWithAllAnnotations).FullName);
			IContainerRegistryExtensions.RegisterForNavigation<AdvancedStickSettings>(containerRegistry, typeof(AdvancedStickSettings).FullName);
			IContainerRegistryExtensions.RegisterForNavigation<VibrationSettings>(containerRegistry, typeof(VibrationSettings).FullName);
			IContainerRegistryExtensions.RegisterForNavigation<MouseSettings>(containerRegistry, typeof(MouseSettings).FullName);
			IContainerRegistryExtensions.RegisterForNavigation<VirtualSticksSettingsView>(containerRegistry, typeof(VirtualSticksSettingsView).FullName);
			IContainerRegistryExtensions.RegisterForNavigation<OverlayMenuView>(containerRegistry, typeof(OverlayMenuView).FullName);
			IContainerRegistryExtensions.RegisterForNavigation<MacroSettings>(containerRegistry, typeof(MacroSettings).FullName);
			IContainerRegistryExtensions.RegisterForNavigation<LedSettingsView>(containerRegistry, typeof(LedSettingsView).FullName);
			IContainerRegistryExtensions.RegisterForNavigation<BFAdvanced>(containerRegistry, typeof(BFAdvanced).FullName);
			IContainerRegistryExtensions.RegisterForNavigation<BFAdvancedZonesSettings>(containerRegistry, typeof(BFAdvancedZonesSettings).FullName);
			IContainerRegistryExtensions.RegisterForNavigation<BFGamepadMapping>(containerRegistry, typeof(BFGamepadMapping).FullName);
			IContainerRegistryExtensions.RegisterForNavigation<BFMain>(containerRegistry, typeof(BFMain).FullName);
			IContainerRegistryExtensions.RegisterForNavigation<BFMask>(containerRegistry, typeof(BFMask).FullName);
			IContainerRegistryExtensions.RegisterForNavigation<BFRumble>(containerRegistry, typeof(BFRumble).FullName);
			IContainerRegistryExtensions.RegisterForNavigation<BFShift>(containerRegistry, typeof(BFShift).FullName);
			IContainerRegistryExtensions.RegisterForNavigation<BFOverlayMenu>(containerRegistry, typeof(BFOverlayMenu).FullName);
			IContainerRegistryExtensions.RegisterForNavigation<BFOverlaySubMenu>(containerRegistry, typeof(BFOverlaySubMenu).FullName);
			IContainerRegistryExtensions.RegisterForNavigation<BFAdaptiveTriggerSettings>(containerRegistry, typeof(BFAdaptiveTriggerSettings).FullName);
			IContainerRegistryExtensions.RegisterSingleton<ILicensingService, LicensingService>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IGuiScaleService, GuiScaleService>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IAdminOperations, AdminOperationsDecider>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IConfigFileService, ConfigFileService>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IEventAggregator, EventAggregator>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IGameProfilesService, GameProfilesService>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IKeyBindingService, KeyBindingService>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IGamepadService, GamepadService>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IKeyPressedPollerService, KeyPressedPollerService>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IWindowsMessageProcessor, WindowsMessageProcessor>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IUserSettingsService, UserSettingsService>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IDeviceDetectionService, DeviceDetectionService>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IGuiHelperService, GuiHelperService>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<ISSEProcessor, SSEClient>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IHttpClientService, HttpClientService>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IEngineSelectorService, EngineSelectorService>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<GamesSelectorVM>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<GamepadSelectorVM>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<GameConfigSelectorVM>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<MainContentVM>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<MainWindowViewModel>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<MainWindow>(containerRegistry);
		}

		private async Task InitTypesForPrism()
		{
			IContainerProviderExtensions.Resolve<IHttpClientService>(App.Container);
			ILicensingService licensingService = IContainerProviderExtensions.Resolve<ILicensingService>(App.Container);
			if (!reWASDUI.Services.HttpClientService.InitializationIsFailedStatic)
			{
				await licensingService.CheckLicenseAsync(false);
			}
			IContainerProviderExtensions.Resolve<IGuiScaleService>(App.Container);
			IContainerProviderExtensions.Resolve<IAdminOperations>(App.Container);
			IContainerProviderExtensions.Resolve<IConfigFileService>(App.Container);
			IContainerProviderExtensions.Resolve<IEventAggregator>(App.Container);
			IContainerProviderExtensions.Resolve<IGameProfilesService>(App.Container);
			IContainerProviderExtensions.Resolve<IGamepadService>(App.Container);
			IContainerProviderExtensions.Resolve<IKeyBindingService>(App.Container);
			IContainerProviderExtensions.Resolve<IKeyPressedPollerService>(App.Container);
			IContainerProviderExtensions.Resolve<IWindowsMessageProcessor>(App.Container);
			IContainerProviderExtensions.Resolve<IEngineSelectorService>(App.Container);
			IContainerProviderExtensions.Resolve<ISSEProcessor>(App.Container).InitAndRun();
		}

		protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
		{
			base.ConfigureRegionAdapterMappings(regionAdapterMappings);
			try
			{
				regionAdapterMappings.RegisterMapping(typeof(Grid), IContainerProviderExtensions.Resolve<CachingGridRegionAdapter>(App.Container));
			}
			catch (Exception)
			{
			}
		}

		protected void OnClose()
		{
			this.SaveLocation();
		}

		public static void SetupLocation(bool isOnStartup = false)
		{
			System.Windows.Application application = System.Windows.Application.Current;
			Window window = ((application != null) ? application.MainWindow : null);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			PresentationSource presentationSource = PresentationSource.FromVisual(window);
			Matrix? matrix;
			if (presentationSource == null)
			{
				matrix = null;
			}
			else
			{
				CompositionTarget compositionTarget = presentationSource.CompositionTarget;
				matrix = ((compositionTarget != null) ? new Matrix?(compositionTarget.TransformToDevice) : null);
			}
			Matrix? matrix2 = matrix;
			double num5 = ((matrix2 != null) ? matrix2.GetValueOrDefault().M11 : 1.0);
			IntPtr handle = new WindowInteropHelper(window).Handle;
			try
			{
				num4 = RegistryHelper.GetValue("View\\MainWindowPosition", "Width", int.MaxValue, false);
				if (num4 == 2147483647)
				{
					num4 = (int)window.Width;
				}
			}
			catch (Exception)
			{
				RegistryHelper.DeleteValue("View\\MainWindowPosition", "Width");
			}
			try
			{
				num3 = RegistryHelper.GetValue("View\\MainWindowPosition", "Height", int.MaxValue, false);
				if (num3 == 2147483647)
				{
					num3 = (int)window.Height;
				}
			}
			catch (Exception)
			{
				RegistryHelper.DeleteValue("View\\MainWindowPosition", "Height");
			}
			try
			{
				num = RegistryHelper.GetValue("View\\MainWindowPosition", "Left", int.MaxValue, false);
			}
			catch (Exception)
			{
				RegistryHelper.DeleteValue("View\\MainWindowPosition", "Left");
			}
			try
			{
				num2 = RegistryHelper.GetValue("View\\MainWindowPosition", "Top", int.MaxValue, false);
			}
			catch (Exception)
			{
				RegistryHelper.DeleteValue("View\\MainWindowPosition", "Top");
			}
			bool flag = true;
			bool flag2 = false;
			Vector vector = new Vector((double)num, (double)num2);
			Vector vector2 = new Vector((double)(num + num4), (double)num2);
			Screen[] allScreens = Screen.AllScreens;
			for (int i = 0; i < allScreens.Length; i++)
			{
				Rectangle workingArea = allScreens[i].WorkingArea;
				if (App.IsInWA(vector, workingArea) || App.IsInWAR(vector2, workingArea))
				{
					flag = false;
				}
				if (num == workingArea.Left && num2 == workingArea.Top && num4 == workingArea.Width && num3 == workingArea.Height)
				{
					flag2 = true;
				}
			}
			if (flag)
			{
				Rectangle bounds = Screen.FromHandle(handle).Bounds;
				if ((double)bounds.Width / num5 < 1280.0 || (double)bounds.Height / num5 < 800.0)
				{
					window.WindowState = WindowState.Maximized;
				}
				else
				{
					num = (int)((double)(bounds.Width / 2) / num5 - System.Windows.Application.Current.MainWindow.Width / 2.0 / num5);
					num2 = (int)((double)(bounds.Height / 2) / num5 - System.Windows.Application.Current.MainWindow.Height / 2.0 / num5);
					num4 = (int)(window.Width / num5);
					num3 = (int)(window.Height / num5);
				}
			}
			App.MoveWindow(handle, num, num2, num4, num3, true);
			if (flag2)
			{
				window.WindowState = WindowState.Maximized;
				System.Windows.Application.Current.MainWindow.Width = System.Windows.Application.Current.MainWindow.ActualWidth;
			}
		}

		private static bool IsInWA(Vector v, Rectangle WorkingArea)
		{
			return v.X > (double)WorkingArea.Left && v.X < (double)WorkingArea.Right && v.Y > (double)WorkingArea.Top && v.Y < (double)WorkingArea.Bottom;
		}

		private static bool IsInWAR(Vector v, Rectangle WorkingArea)
		{
			return v.X >= (double)WorkingArea.Left && v.X <= (double)WorkingArea.Right && v.Y >= (double)WorkingArea.Top && v.Y <= (double)WorkingArea.Bottom;
		}

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

		private void SaveLocation()
		{
			System.Windows.Application application = System.Windows.Application.Current;
			Window window = ((application != null) ? application.MainWindow : null);
			if (window != null)
			{
				Screen screen = Screen.FromHandle(new WindowInteropHelper(window).Handle);
				Rectangle bounds = screen.Bounds;
				PresentationSource presentationSource = PresentationSource.FromVisual(window);
				Matrix? matrix;
				if (presentationSource == null)
				{
					matrix = null;
				}
				else
				{
					CompositionTarget compositionTarget = presentationSource.CompositionTarget;
					matrix = ((compositionTarget != null) ? new Matrix?(compositionTarget.TransformToDevice) : null);
				}
				Matrix? matrix2 = matrix;
				double num = ((matrix2 != null) ? matrix2.GetValueOrDefault().M11 : 1.0);
				int num2 = (int)(window.Left * num);
				int num3 = (int)(window.Top * num);
				int num4 = (int)(window.Width * num);
				int num5 = (int)(window.Height * num);
				if (window.WindowState == WindowState.Maximized)
				{
					num2 = screen.WorkingArea.Left;
					num3 = screen.WorkingArea.Top;
					num4 = screen.WorkingArea.Width;
					num5 = screen.WorkingArea.Height;
				}
				RegistryHelper.SetValue("View\\MainWindowPosition", "Left", num2);
				RegistryHelper.SetValue("View\\MainWindowPosition", "Top", num3);
				RegistryHelper.SetValue("View\\MainWindowPosition", "Width", num4);
				RegistryHelper.SetValue("View\\MainWindowPosition", "Height", num5);
			}
		}

		private async Task CheckEngineRunning()
		{
			if (HttpServerSettings.GetActualLocalRoute() == "127.0.0.1" || HttpServerSettings.GetActualLocalRoute() == "localhost")
			{
				await UtilsCommon.TryRunEngine();
			}
		}

		private static void InitInfrastructure()
		{
			GamepadButtonDescription.CustomFriendlyName = delegate(GamepadButtonDescription button)
			{
				BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
				ControllerTypeEnum? controllerTypeEnum;
				if (currentGamepad == null)
				{
					controllerTypeEnum = null;
				}
				else
				{
					ControllerVM currentController = currentGamepad.CurrentController;
					controllerTypeEnum = ((currentController != null) ? new ControllerTypeEnum?(currentController.ControllerType) : null);
				}
				ControllerTypeEnum? controllerTypeEnum2 = controllerTypeEnum;
				return XBUtils.ConvertGamepadButtonToDescription(button.Button, (controllerTypeEnum2 != null && ControllerTypeExtensions.IsGamepad(controllerTypeEnum2.GetValueOrDefault())) ? controllerTypeEnum2 : new ControllerTypeEnum?(2));
			};
		}

		public static void ReleaseMutex()
		{
			App.mutex.Close();
		}

		public void CreateRecreateMainWindow(bool requiresUserPermission = false)
		{
			App.m_MainWnd = this.CreateShell() as MainWindow;
			System.Windows.Application.Current.MainWindow = App.m_MainWnd;
		}

		public async void WaitForConnection()
		{
			HttpClientService httpClient = App.HttpClientService as HttpClientService;
			TaskAwaiter<HttpResponseMessage> taskAwaiter = httpClient.Engine.WaitForInited().GetAwaiter();
			TaskAwaiter<HttpResponseMessage> taskAwaiter2;
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<HttpResponseMessage>);
			}
			bool flag = taskAwaiter.GetResult() != null;
			if (!flag)
			{
				taskAwaiter = httpClient.Engine.WaitForInited().GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<HttpResponseMessage>);
				}
				flag = taskAwaiter.GetResult() != null;
			}
			bool flag2 = flag;
			if (!flag2)
			{
				taskAwaiter = httpClient.Engine.WaitForInited().GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<HttpResponseMessage>);
				}
				flag2 = taskAwaiter.GetResult() != null;
			}
			bool flag3 = flag2;
			if (!flag3)
			{
				taskAwaiter = httpClient.Engine.WaitForInited().GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<HttpResponseMessage>);
				}
				flag3 = taskAwaiter.GetResult() != null;
			}
			if (flag3)
			{
				httpClient.InitializationIsFailed = false;
				EngineSelectorService engineSelectorService = IContainerProviderExtensions.Resolve<IEngineSelectorService>(App.Container) as EngineSelectorService;
				await engineSelectorService.ChangeEngine(engineSelectorService.CurrentEngine);
			}
		}

		public async Task InitAfterShowWindow()
		{
			System.Windows.Application.Current.MainWindow.Closing += delegate([Nullable(2)] object s, CancelEventArgs ex)
			{
				this.OnClose();
			};
			System.Windows.Application.Current.MainWindow.SizeChanged += this.MainWindow_SizeChanged;
			System.Windows.Application.Current.MainWindow.LocationChanged += this.MainWindow_LocationChanged;
			WaitDialog.ShowDialogStatic(DTLocalization.GetString(12271), null, null, false, false, null, null);
			HttpClientService httpClient = new HttpClientService();
			bool needToWaitForConnection = false;
			TaskAwaiter<HttpResponseMessage> taskAwaiter = httpClient.Engine.WaitForInited().GetAwaiter();
			TaskAwaiter<HttpResponseMessage> taskAwaiter2;
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<HttpResponseMessage>);
			}
			bool flag = taskAwaiter.GetResult() == null;
			if (flag)
			{
				taskAwaiter = httpClient.Engine.WaitForInited().GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<HttpResponseMessage>);
				}
				flag = taskAwaiter.GetResult() == null;
			}
			if (flag)
			{
				httpClient.InitializationIsFailed = true;
				needToWaitForConnection = true;
			}
			WaitDialog.TryCloseWaitDialog();
			this.InitViewModelLocator();
			await this.InitTypesForPrism();
			Action onAppFullyInitialized = App.OnAppFullyInitialized;
			if (onAppFullyInitialized != null)
			{
				onAppFullyInitialized();
			}
			if (needToWaitForConnection)
			{
				this.WaitForConnection();
			}
			MainWindow._policyDll = IntPtr.Zero;
			Constants.InitAppSettingsFromRegistry();
			if (RegistryHelper.GetValue("Config", "IsNDISFilterRegistered", 1, false) == 0)
			{
				MessageBoxWithDoNotShowLogic.Show(base.MainWindow, DTLocalization.GetString(12565), MessageBoxButton.OK, MessageBoxImage.Exclamation, "RemindAboutNDISFilterDriver", MessageBoxResult.OK, false, 0.0, null, null, null, null, null, null);
			}
			this.UpdateDefaultStateOfRegistry();
			App.ShowWarningOnUpdates();
			if (RegistryHelper.GetValue("Config", "ShowReleaseNotes", 0, false) == 1)
			{
				RegistryHelper.SetValue("Config", "ShowReleaseNotes", false);
				if (RegistryHelper.GetValue("Config", "ShowReleasePageOnStartup", 1, false) == 1)
				{
					try
					{
						Process.Start(new ProcessStartInfo(string.Format("https://www.rewasd.com/releases/release-{0}.{1}.{2}", 6, 7, 0))
						{
							UseShellExecute = true
						});
					}
					catch
					{
					}
				}
			}
			string text = 5.ToString() + "." + 12.ToString();
			string text2 = 3.ToString() + "." + 29.ToString();
			Tracer.TraceWrite(Tracer.PRODUCT_SHORT_NAME + " launched", false);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(47, 3);
			defaultInterpolatedStringHandler.AppendLiteral("GUI version ");
			defaultInterpolatedStringHandler.AppendFormatted("6.7.0.8034");
			defaultInterpolatedStringHandler.AppendLiteral(", Service version ");
			defaultInterpolatedStringHandler.AppendFormatted(text);
			defaultInterpolatedStringHandler.AppendLiteral(", Driver version ");
			defaultInterpolatedStringHandler.AppendFormatted(text2);
			Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
		}

		private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.SaveLocation();
		}

		private void MainWindow_LocationChanged(object sender, EventArgs e)
		{
			this.SaveLocation();
		}

		public void InitAfterContentRendered()
		{
			this.ParseCommandLine(this._args.Args);
		}

		protected override Uri GetX300ResourcesUri()
		{
			return new Uri("/reWASDResources;component/Images/_ImageCatalog_X3.xaml", UriKind.Relative);
		}

		private App.CommandLineParam GetCommandLineParam(string[] args)
		{
			if (args.Length > 1 && args[0] == "/import")
			{
				RegistryHelper.SetString("ImportParam", "GuiNamespace", args[1]);
				return App.CommandLineParam.clpImport;
			}
			if (args.Length != 0 && args[0] == "/OpenAgentSettings")
			{
				return App.CommandLineParam.clpOpenAgentSettings;
			}
			return App.CommandLineParam.clpNone;
		}

		private async void ParseCommandLine(string[] args)
		{
			if (args.Length > 1 && args[0] == "/import")
			{
				await App.GuiHelperService.ImportGameConfig(args[1], false);
			}
			if (args.Length != 0 && args[0] == "/OpenAgentSettings")
			{
				await App.GamepadService.WaitForServiceInited();
				PreferencesWindow.ShowPreferences(typeof(PreferencesTrayAgentVM));
			}
		}

		public override ILogmanTracer GetETWTracerForCurrentApplication()
		{
			return new RewasdETWTracerWrapper();
		}

		private void InitViewModelLocator()
		{
			this._viewModelTypes = this.GetTypesInNamespace(Assembly.GetExecutingAssembly(), "reWASDUI.ViewModels");
			ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(delegate(Type viewType)
			{
				string viewName = viewType.Name;
				Type type = this._viewModelTypes.FirstOrDefault((Type vt) => vt.Name == viewName + "ViewModel" || vt.Name == viewName + "VM");
				if (type == null)
				{
					return null;
				}
				return Type.GetType(type.FullName);
			});
		}

		private IEnumerable<Type> GetTypesInNamespace(Assembly assembly, string nameSpace)
		{
			return from t in assembly.GetTypes()
				where !t.IsAbstract && (t.IsSubclassOf(typeof(BindableBase)) || t.IsSubclassOf(typeof(ZBindable))) && t.Namespace.StartsWith(nameSpace)
				select t;
		}

		public static void OpenCommunityLink()
		{
			Process.Start(new ProcessStartInfo("https://www.rewasd.com/community/")
			{
				UseShellExecute = true
			});
		}

		private void UpdateDefaultStateOfRegistry()
		{
			int num = 100;
			if (RegistryHelper.GetValue("", "AllowAnonymousReport", num, false) == num)
			{
				RegistryHelper.SetValue("", "AllowAnonymousReport", 1);
			}
			if (RegistryHelper.GetValue("Config", "Autorun", num, false) == num)
			{
				RegistryHelper.SetValue("Config", "Autorun", 1);
				Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Run", "reWASD Engine", "\"" + Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\reWASDEngine.exe\"");
			}
			if (RegistryHelper.GetValue("Config", "AutoRemap", num, false) == num)
			{
				RegistryHelper.SetValue("Config", "AutoRemap", 1);
			}
		}

		public static void ShowWarningOnUpdates()
		{
			if (Convert.ToBoolean(RegistryHelper.GetValue("", "UpdateFromOldVersion", 0, false)))
			{
				RegistryHelper.SetValue("", "UpdateFromOldVersion", 0);
				DTMessageBox.Show(DTLocalization.GetString(11211), MessageBoxButton.OK, MessageBoxImage.Asterisk, null, false, MessageBoxResult.None);
			}
		}

		[STAThread]
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		public static void Main()
		{
			App app = new App();
			app.InitializeComponent();
			app.Run();
		}

		public const double MAIN_WINDOW_DEFAULT_WIDTH = 1280.0;

		public const double MAIN_WINDOW_DEFAULT_HEIGHT = 800.0;

		public const string ANDROID_JUNIOR_URL = "https://play.google.com/store/apps/details?id=com.discsoft.rewasd";

		public const string APP_STORE_JUNIOR_URL = "https://apps.apple.com/app/id1586976147";

		public const string ADVANCED_MAPPING_FEATURE_GUID = "advanced-mapping";

		public const string MACROS_FEATURE_GUID = "macros";

		public const string FOUR_SLOTS_FEATURE_GUID = "four-slots";

		public const string RAPID_FIRE_FEATURE_GUID = "rapid-fire";

		public const string MOBILE_CONTROLLER_FEATURE_GUID = "mobile-controller";

		public const string FULL_PACK_GUID = "bundle";

		private static IGameProfilesService _gameProfilesService;

		private static IEventAggregator _eventAggregator;

		private static IConfigFileService _configFileService;

		private static IKeyBindingService _keyBindingService;

		private static IGamepadService _gamepadService;

		private static IAdminOperations _adminOperations;

		private static Lazy<IGamepadService> _gamepadServiceLazy;

		private static ILicensingService _licensingService;

		private static IDeviceDetectionService _deviceDetectionService;

		private static IKeyPressedPollerService _keyPressedPollerService;

		private static IWindowsMessageProcessor _windowsMessageProcessor;

		private static IUserSettingsService _userSettingsService;

		private static IHttpClientService _httpClientService;

		private static MainContentVM _mainContentVM;

		private static IGuiHelperService _guiHelperService = null;

		private static bool _guiHelperInited = false;

		public static Mutex mutex;

		public static bool bMutexInitiallyOwned;

		public const string sMutexName = "Local\\{DBCA738F-A4CB-4da8-59D2-7BC90CB671EA}";

		public static IContainerProvider Container;

		public static List<SingleRegionManager> BindingFrameRegionManagers = new List<SingleRegionManager>();

		private static IRegionManager _regionManager;

		public static Action OnAppFullyInitialized;

		private StartupEventArgs _args;

		private IEnumerable<Type> _viewModelTypes;

		public enum CommandLineParam
		{
			clpNone,
			clpImport,
			clpOpenAgentSettings
		}
	}
}
