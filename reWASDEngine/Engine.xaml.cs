using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using DiscSoft.NET.Common.DSBaseApp;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using Prism.Events;
using Prism.Ioc;
using reWASDCommon;
using reWASDCommon.Interfaces;
using reWASDCommon.Utils;
using reWASDEngine.Services;
using reWASDEngine.Services.HttpServer;
using reWASDEngine.Services.Interfaces;
using reWASDEngine.Services.UdpServer;
using XBEliteWPF.Infrastructure.ExternalDevices;
using XBEliteWPF.License.Licensing;
using XBEliteWPF.Services;
using XBEliteWPF.Services.HttpServer;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils;
using XBEliteWPF.Utils.XBUtilModel;

namespace reWASDEngine
{
	public partial class Engine : DSBaseApplication
	{
		public static IGameProfilesService GameProfilesService
		{
			get
			{
				IGameProfilesService gameProfilesService;
				if ((gameProfilesService = Engine._gameProfilesService) == null)
				{
					gameProfilesService = (Engine._gameProfilesService = IContainerProviderExtensions.Resolve<IGameProfilesService>(Engine.SContainer));
				}
				return gameProfilesService;
			}
		}

		public static IEventAggregator EventAggregator
		{
			get
			{
				IEventAggregator eventAggregator;
				if ((eventAggregator = Engine._eventAggregator) == null)
				{
					eventAggregator = (Engine._eventAggregator = IContainerProviderExtensions.Resolve<IEventAggregator>(Engine.SContainer));
				}
				return eventAggregator;
			}
		}

		public static IConfigFileService ConfigFileService
		{
			get
			{
				IConfigFileService configFileService;
				if ((configFileService = Engine._configFileService) == null)
				{
					configFileService = (Engine._configFileService = IContainerProviderExtensions.Resolve<IConfigFileService>(Engine.SContainer));
				}
				return configFileService;
			}
		}

		public static IForegroundApplicationMonitorService ForegroundApplicationMonitorService
		{
			get
			{
				IForegroundApplicationMonitorService foregroundApplicationMonitorService;
				if ((foregroundApplicationMonitorService = Engine._foregroundApplicationMonitorService) == null)
				{
					foregroundApplicationMonitorService = (Engine._foregroundApplicationMonitorService = IContainerProviderExtensions.Resolve<IForegroundApplicationMonitorService>(Engine.SContainer));
				}
				return foregroundApplicationMonitorService;
			}
		}

		public static IGamepadService GamepadService { get; set; }

		public static IOverlayManagerService OverlayManagerService
		{
			get
			{
				IOverlayManagerService overlayManagerService;
				if ((overlayManagerService = Engine._overlayManagerService) == null)
				{
					overlayManagerService = (Engine._overlayManagerService = IContainerProviderExtensions.Resolve<IOverlayManagerService>(Engine.SContainer));
				}
				return overlayManagerService;
			}
		}

		public static Lazy<IGamepadService> GamepadServiceLazy
		{
			get
			{
				Lazy<IGamepadService> lazy;
				if ((lazy = Engine._gamepadServiceLazy) == null)
				{
					lazy = (Engine._gamepadServiceLazy = IContainerProviderExtensions.Resolve<Lazy<IGamepadService>>(Engine.SContainer));
				}
				return lazy;
			}
		}

		public static ILicensingService LicensingService
		{
			get
			{
				ILicensingService licensingService;
				if ((licensingService = Engine._licensingService) == null)
				{
					licensingService = (Engine._licensingService = IContainerProviderExtensions.Resolve<ILicensingService>(Engine.SContainer));
				}
				return licensingService;
			}
		}

		public static IXBServiceCommunicator XBServiceCommunicator
		{
			get
			{
				IXBServiceCommunicator ixbserviceCommunicator;
				if ((ixbserviceCommunicator = Engine._xbServiceCommunicator) == null)
				{
					ixbserviceCommunicator = (Engine._xbServiceCommunicator = IContainerProviderExtensions.Resolve<IXBServiceCommunicator>(Engine.SContainer));
				}
				return ixbserviceCommunicator;
			}
		}

		public static IWindowsMessageProcessor WindowsMessageProcessor
		{
			get
			{
				IWindowsMessageProcessor windowsMessageProcessor;
				if ((windowsMessageProcessor = Engine._windowsMessageProcessor) == null)
				{
					windowsMessageProcessor = (Engine._windowsMessageProcessor = IContainerProviderExtensions.Resolve<IWindowsMessageProcessor>(Engine.SContainer));
				}
				return windowsMessageProcessor;
			}
		}

		public static IUserSettingsService UserSettingsService
		{
			get
			{
				IUserSettingsService userSettingsService;
				if ((userSettingsService = Engine._userSettingsService) == null)
				{
					userSettingsService = (Engine._userSettingsService = IContainerProviderExtensions.Resolve<IUserSettingsService>(Engine.SContainer));
				}
				return userSettingsService;
			}
		}

		public static ILEDService LEDService
		{
			get
			{
				ILEDService iledservice;
				if ((iledservice = Engine._ledService) == null)
				{
					iledservice = (Engine._ledService = IContainerProviderExtensions.Resolve<ILEDService>(Engine.SContainer));
				}
				return iledservice;
			}
		}

		public static IEventProcessor EventProcessor
		{
			get
			{
				IEventProcessor eventProcessor;
				if ((eventProcessor = Engine._eventProcessor) == null)
				{
					eventProcessor = (Engine._eventProcessor = IContainerProviderExtensions.Resolve<IEventProcessor>(Engine.SContainer));
				}
				return eventProcessor;
			}
		}

		public static IHttpServer HttpServer
		{
			get
			{
				IHttpServer httpServer;
				if ((httpServer = Engine._httpServer) == null)
				{
					httpServer = (Engine._httpServer = IContainerProviderExtensions.Resolve<IHttpServer>(Engine.SContainer));
				}
				return httpServer;
			}
		}

		public static IUdpServer UdpServer
		{
			get
			{
				IUdpServer udpServer;
				if ((udpServer = Engine._udpServer) == null)
				{
					udpServer = (Engine._udpServer = IContainerProviderExtensions.Resolve<IUdpServer>(Engine.SContainer));
				}
				return udpServer;
			}
		}

		public static GamepadUdpServer GamepadUdpServer
		{
			get
			{
				GamepadUdpServer gamepadUdpServer;
				if ((gamepadUdpServer = Engine._gamepadUdpServer) == null)
				{
					gamepadUdpServer = (Engine._gamepadUdpServer = new GamepadUdpServer());
				}
				return gamepadUdpServer;
			}
		}

		public static IEventsProxy EventsProxy
		{
			get
			{
				IEventsProxy eventsProxy;
				if ((eventsProxy = Engine._eventProxy) == null)
				{
					eventsProxy = (Engine._eventProxy = IContainerProviderExtensions.Resolve<IEventsProxy>(Engine.SContainer));
				}
				return eventsProxy;
			}
		}

		public static IEngineControllerMonitor EngineControllerMonitor
		{
			get
			{
				IEngineControllerMonitor engineControllerMonitor;
				if ((engineControllerMonitor = Engine._engineControllerMonitor) == null)
				{
					engineControllerMonitor = (Engine._engineControllerMonitor = IContainerProviderExtensions.Resolve<IEngineControllerMonitor>(Engine.SContainer));
				}
				return engineControllerMonitor;
			}
		}

		public static IGuiHelperService GuiHelperService
		{
			get
			{
				if (Engine._guiHelperInited)
				{
					return Engine._guiHelperService;
				}
				Engine._guiHelperInited = true;
				try
				{
					Engine._guiHelperService = IContainerProviderExtensions.Resolve<IGuiHelperService>(Engine.SContainer);
				}
				catch (Exception)
				{
				}
				return Engine._guiHelperService;
			}
		}

		protected override Window CreateShell()
		{
			return IContainerProviderExtensions.Resolve<MainWindow>(base.Container);
		}

		protected override async void RegisterTypes(IContainerRegistry containerRegistry)
		{
			Engine.SContainer = base.Container;
			IContainerRegistryExtensions.RegisterSingleton<MainWindow>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<ILicensingCommunicator, LicensingCommunicator>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IXBServiceCommunicator, XBServiceCommunicator>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IConfigFileService, ConfigFileService>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IGameProfilesService, GameProfilesService>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IGamepadService, GamepadService>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<ILicensingService, LicensingService>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IAdminOperations, AdminOperationsDecider>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IThirdPartyOperations, LedOperationsDecider>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IForegroundApplicationMonitorService, ForegroundApplicationMonitorService>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IWindowsMessageProcessor, WindowsMessageProcessor>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IUserSettingsService, UserSettingsService>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<ILEDService, LEDService>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IEventProcessor, EventProcessor>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IHttpServer, HttpServer>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IUdpServer, UdpServer>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IOverlayManagerService, OverlayManagerService>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IEventsProxy, EventsProxy>(containerRegistry);
			IContainerRegistryExtensions.RegisterSingleton<IEngineControllerMonitor, EngineControllerMonitor>(containerRegistry);
			IContainerProviderExtensions.Resolve<IXBServiceCommunicator>(Engine.SContainer);
			IContainerProviderExtensions.Resolve<ILicensingCommunicator>(Engine.SContainer);
			await LicenseApi.Init();
			await IContainerProviderExtensions.Resolve<ILicensingService>(Engine.SContainer).CheckLicenseAsync(false);
			IContainerProviderExtensions.Resolve<IGamepadService>(Engine.SContainer);
			IContainerProviderExtensions.Resolve<IHttpServer>(Engine.SContainer);
			IContainerProviderExtensions.Resolve<IUdpServer>(Engine.SContainer);
			IContainerProviderExtensions.Resolve<IOverlayManagerService>(Engine.SContainer);
			IContainerProviderExtensions.Resolve<IEventsProxy>(Engine.SContainer);
		}

		protected override async void OnStartup(StartupEventArgs e)
		{
			this.InitializeComponent();
			Tracer.PRODUCT_SHORT_NAME = "reWASD engine";
			await this.ExecCommandLineFeatures(e);
			try
			{
				Engine.mutex = new Mutex(true, "Local\\{87C5FA79-B4CB-429F-836B-1515F4C93438}", out Engine.bMutexInitiallyOwned);
			}
			catch (Exception)
			{
				Environment.Exit(0);
				return;
			}
			if (!Engine.bMutexInitiallyOwned)
			{
				Environment.Exit(0);
			}
			else
			{
				XBUtils.LoadGlobalLibsDll();
				TranslationManager.InitLocalization(LocalizationManager.Instance);
				if (!TranslationManager.Instance.IsFullyInitialized)
				{
					Environment.Exit(0);
				}
				if (!XBUtils.CheckCertDll("CrossPlatformLib.dll") || !XBUtils.CheckCertDll("DiscSoft.NET.Common.dll"))
				{
					Environment.Exit(0);
				}
				if (UtilsNative.IsWindows10OrHigher() && !XBUtils.CheckCertDll("reWASDPolicy.dll"))
				{
					Environment.Exit(0);
				}
				base.OnStartup(e);
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
				this.ParseCommandLineParameters(e);
			}
		}

		private async Task ExecCommandLineFeatures(StartupEventArgs e)
		{
			if (e.Args.Contains("-ClearSettings"))
			{
				ObservableCollection<ExternalDevice> observableCollection = BinDataSerialize.Load<ObservableCollection<ExternalDevice>>(BinDataSerialize.USER_IMPERSONATED_EXTERNAL_DEVICES_COLLECTION_FULL_SAVE_FILE_PATH, null);
				if (observableCollection != null)
				{
					AdminOperationsService adminOperations = new AdminOperationsService();
					foreach (ExternalDevice device in observableCollection)
					{
						if (device.DeviceType == 2)
						{
							bool flag = !string.IsNullOrEmpty(device.HardwareDongleBluetoothMacAddress);
							TaskAwaiter<bool> taskAwaiter2;
							if (flag)
							{
								TaskAwaiter<bool> taskAwaiter = adminOperations.DeleteESP32DeviceRegKeys(device.HardwareDongleBluetoothMacAddress.Replace(":", "-").ToLower()).GetAwaiter();
								if (!taskAwaiter.IsCompleted)
								{
									await taskAwaiter;
									taskAwaiter = taskAwaiter2;
									taskAwaiter2 = default(TaskAwaiter<bool>);
								}
								flag = !taskAwaiter.GetResult();
							}
							if (flag)
							{
								return;
							}
							if (device.IsSerialPortFTDI && device.DefaultLatencySpeed != 0 && device.DefaultLatencySpeed != device.LatencySpeed)
							{
								TaskAwaiter<bool> taskAwaiter = adminOperations.ChangeESP32DeviceLatency(device.SerialPort, device.DefaultLatencySpeed).GetAwaiter();
								if (!taskAwaiter.IsCompleted)
								{
									await taskAwaiter;
									taskAwaiter = taskAwaiter2;
									taskAwaiter2 = default(TaskAwaiter<bool>);
								}
								if (!taskAwaiter.GetResult())
								{
									return;
								}
							}
						}
						device = null;
					}
					IEnumerator<ExternalDevice> enumerator = null;
					adminOperations = null;
				}
				Environment.Exit(0);
			}
		}

		private void ParseCommandLineParameters(StartupEventArgs e)
		{
			Engine.commandLineParam = Engine.CommandLineParam.clpNone;
			if (e.Args.Contains("-autoremap"))
			{
				Engine.commandLineParam = Engine.CommandLineParam.clpAutoremap;
			}
		}

		public static Mutex mutex;

		public static bool bMutexInitiallyOwned;

		private static IGameProfilesService _gameProfilesService;

		private static IEventAggregator _eventAggregator;

		private static IConfigFileService _configFileService;

		private static IForegroundApplicationMonitorService _foregroundApplicationMonitorService;

		private static IOverlayManagerService _overlayManagerService;

		private static Lazy<IGamepadService> _gamepadServiceLazy;

		private static ILicensingService _licensingService;

		private static IXBServiceCommunicator _xbServiceCommunicator;

		private static IWindowsMessageProcessor _windowsMessageProcessor;

		private static IUserSettingsService _userSettingsService;

		private static ILEDService _ledService;

		private static IEventProcessor _eventProcessor;

		private static IHttpServer _httpServer;

		private static IUdpServer _udpServer;

		private static GamepadUdpServer _gamepadUdpServer;

		private static IEventsProxy _eventProxy;

		private static IEngineControllerMonitor _engineControllerMonitor;

		private static IGuiHelperService _guiHelperService;

		private static bool _guiHelperInited;

		public static IContainerProvider SContainer;

		public static Engine.CommandLineParam commandLineParam;

		public enum CommandLineParam
		{
			clpNone,
			clpAutoremap
		}
	}
}
