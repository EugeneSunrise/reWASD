using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DiscSoft.NET.Common.DSBaseApp;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.AttachedBehaviours;
using DiscSoft.NET.Common.Utils.Clases;
using Hardcodet.Wpf.TaskbarNotification;
using HL.IconPro.Lib.Wpf;
using Microsoft.Win32;
using reWASDCommon.Infrastructure;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.Controller;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils.Converters;
using XBEliteWPF.Utils.XBUtilModel;

namespace reWASDEngine
{
	public class MainWindow : Window, IComponentConnector
	{
		private bool UseAgent { get; set; }

		private bool ShowTrayIcons { get; set; }

		public MainWindow()
		{
			base.Loaded += this.OnLoaded;
			this.InitializeComponent();
		}

		private void OnClosing(object sender, CancelEventArgs e)
		{
			ILEDService ledservice = Engine.LEDService;
			if (ledservice != null)
			{
				ledservice.Stop(true, true, true);
			}
			ILEDService ledservice2 = Engine.LEDService;
			if (ledservice2 != null)
			{
				ledservice2.DeinitializeAndClose();
			}
			IHttpServer httpServer = Engine.HttpServer;
			if (httpServer != null)
			{
				httpServer.StopAndClose();
			}
			IUdpServer udpServer = Engine.UdpServer;
			if (udpServer == null)
			{
				return;
			}
			udpServer.StopAndClose();
		}

		public void TrayIconsVisibility(bool isVisible)
		{
			this.notifyIcon.Visibility = (isVisible ? Visibility.Visible : Visibility.Collapsed);
		}

		private void OnPreferencesChanged(object obj)
		{
			this.ShowTrayIcons = Convert.ToBoolean(RegistryHelper.GetValue("Config", "ShowTrayIcons", 1, false));
			this.UseAgent = Convert.ToBoolean(RegistryHelper.GetValue("Config", "Autorun", 1, false));
			this.TrayIconsVisibility(this.ShowTrayIcons && this.UseAgent);
			this.UpdateTrays();
			Constants.InitAppSettingsFromRegistry();
		}

		private void OnGamepadsBlacklistChanged(WindowMessageEvent obj)
		{
			MainWindowViewModel mainWindowViewModel = base.DataContext as MainWindowViewModel;
			if (mainWindowViewModel == null)
			{
				return;
			}
			mainWindowViewModel.RefreshTrayAndGamepadsCollection(null);
		}

		~MainWindow()
		{
			XBUtils.UnLoadPolicyDll(MainWindow._policyDll);
		}

		private void OnLoaded(object s, RoutedEventArgs e)
		{
			this.ShowTrayIcons = Convert.ToBoolean(RegistryHelper.GetValue("Config", "ShowTrayIcons", 1, false));
			this.UseAgent = Convert.ToBoolean(RegistryHelper.GetValue("Config", "Autorun", 1, false));
			this.notifyIcon.Visibility = ((this.UseAgent && this.ShowTrayIcons) ? Visibility.Visible : Visibility.Collapsed);
			base.DataContext = new MainWindowViewModel();
			base.Closing += this.OnClosing;
			Engine.GamepadService.AllPhysicalControllers.CollectionChanged += this.AllPhysicalControllers_CollectionChanged;
			Engine.EventAggregator.GetEvent<ControllerStateChanged>().Subscribe(new Action<BaseControllerVM>(this.OnControllerStateChanged));
			Engine.EventAggregator.GetEvent<GamepadsSettingsChanged>().Subscribe(new Action<WindowMessageEvent>(this.OnGamepadsSettingsChanged));
			Engine.EventAggregator.GetEvent<GamepadsBlacklistChanged>().Subscribe(new Action<WindowMessageEvent>(this.OnGamepadsBlacklistChanged));
			Engine.EventAggregator.GetEvent<PreferencesChanged>().Subscribe(new Action<object>(this.OnPreferencesChanged));
			Engine.EventAggregator.GetEvent<DeviceRenamed>().Subscribe(new Action<string>(this.OnDeviceRenamed));
			Engine.GamepadService.OnBatteryLevelChanged += delegate(ControllerVM controller)
			{
				ThreadHelper.ExecuteInMainDispatcher(delegate
				{
					this.CheckAddController(controller);
				}, true);
			};
			SystemEvents.DisplaySettingsChanged += this.SystemEvents_DisplaySettingsChanged;
			Constants.InitAppSettingsFromRegistry();
			Engine.HttpServer.InitAndRun();
			Engine.UdpServer.InitAndRun();
			MainWindow._policyDll = XBUtils.LoadPolicyDll();
			base.Visibility = Visibility.Hidden;
			Engine.WindowsMessageProcessor.Attach(this, true);
			DSBaseApplication dsbaseApplication = Application.Current as DSBaseApplication;
			if (dsbaseApplication != null)
			{
				dsbaseApplication.RaiseOnMainWindowContentRendered(s, e);
			}
		}

		private async void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
		{
			await Task.Delay(1000);
			((MainWindowViewModel)base.DataContext).RefreshTray(true);
			this.UpdateTrays();
		}

		private void OnGamepadsSettingsChanged(WindowMessageEvent obj)
		{
			ThreadHelper.ExecuteInMainDispatcher(delegate
			{
				this.UpdateTrays();
			}, true);
		}

		private void OnDeviceRenamed(string devID)
		{
			ThreadHelper.ExecuteInMainDispatcher(delegate
			{
				this.UpdateDeviceName(devID);
			}, true);
		}

		private void UpdateTrayData(TaskbarIcon taskbarIcon, ControllerVM controller)
		{
			taskbarIcon.ToolTipText = controller.ControllerDisplayName;
			if (controller.IsBatteryLevelPercentPresent)
			{
				string text = DTLocalization.GetString(12352).Replace("{0}", controller.BatteryLevelPercent.ToString());
				taskbarIcon.ToolTipText = taskbarIcon.ToolTipText + "\n" + text;
			}
			taskbarIcon.IconSource = this.GetBatteryIconFromController(controller);
			Tracer.TraceWrite(Tracer.PRODUCT_SHORT_NAME + " UpdateControllerInfo " + controller.ControllerDisplayName, false);
		}

		private void OnControllerStateChanged(BaseControllerVM controller)
		{
			if (controller.IsOnline)
			{
				ThreadHelper.ExecuteInMainDispatcher(delegate
				{
					this.CheckAddController(controller);
				}, true);
				return;
			}
			ThreadHelper.ExecuteInMainDispatcher(delegate
			{
				this.CheckRemoveController(controller);
			}, true);
		}

		private void AllPhysicalControllers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				Tracer.TraceWrite(Tracer.PRODUCT_SHORT_NAME + " PhysicalControllers_CollectionChanged Reset", false);
				List<ControllerVM> list = new List<ControllerVM>();
				foreach (KeyValuePair<ControllerVM, TaskbarIcon> keyValuePair in ((IEnumerable<KeyValuePair<ControllerVM, TaskbarIcon>>)this.TrayForControllers))
				{
					list.Add(keyValuePair.Key);
				}
				foreach (ControllerVM controllerVM in list)
				{
					this.CheckRemoveController(controllerVM);
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				Tracer.TraceWrite(Tracer.PRODUCT_SHORT_NAME + " PhysicalControllers_CollectionChanged Add", false);
				foreach (object obj in e.NewItems)
				{
					BaseControllerVM baseControllerVM = (BaseControllerVM)obj;
					this.CheckAddController(baseControllerVM);
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				Tracer.TraceWrite(Tracer.PRODUCT_SHORT_NAME + " PhysicalControllers_CollectionChanged Remove", false);
				foreach (object obj2 in e.OldItems)
				{
					BaseControllerVM baseControllerVM2 = (BaseControllerVM)obj2;
					this.CheckRemoveController(baseControllerVM2);
				}
			}
		}

		private Color GetControllerColorFromSettings(ControllerVM controller)
		{
			GamepadSettings gamepadSettings = Engine.GamepadService.GamepadsSettings.SingleOrDefault((GamepadSettings item) => item.ID == controller.ID);
			try
			{
				if (gamepadSettings != null)
				{
					return (new GamepadColorToBrushConverter().Convert(gamepadSettings.Color, typeof(GamepadColor), null, null) as SolidColorBrush).Color;
				}
			}
			catch (Exception)
			{
			}
			return (new GamepadColorToBrushConverter().Convert(0, typeof(GamepadColor), null, null) as SolidColorBrush).Color;
		}

		private ImageSource GetBatteryIconFromController(ControllerVM controller)
		{
			string text = "";
			switch (controller.ControllerBatteryLevel)
			{
			case 0:
				text = "critical";
				break;
			case 1:
				text = "low";
				break;
			case 2:
				text = "medium";
				break;
			case 3:
				text = "high";
				break;
			case 4:
				text = "unknown";
				break;
			}
			if (controller.ControllerBatteryChargingState == 1)
			{
				text = "battery_agent_charging_" + text;
			}
			else
			{
				text = "battery_agent_" + text;
			}
			if (controller.IsConnectionWired && controller.ControllerBatteryChargingState != 1)
			{
				text = "battery_agent_wired_n";
			}
			Color controllerColorFromSettings = this.GetControllerColorFromSettings(controller);
			string text2 = Constants.PROGRAMM_DATA_DIRECTORY_PATH + "\\icons";
			if (!Directory.Exists(text2))
			{
				Directory.CreateDirectory(text2);
			}
			string text3 = string.Concat(new string[]
			{
				text2,
				"\\",
				text,
				"_",
				string.Format("{0:X2}{1:X2}{2:X2}", controllerColorFromSettings.R, controllerColorFromSettings.G, controllerColorFromSettings.B),
				".ico"
			});
			if (!File.Exists(text3))
			{
				Drawing drawing = Application.Current.TryFindResource(text) as Drawing;
				try
				{
					this.ConvertDrawingToIconFile(drawing, controllerColorFromSettings, text3);
				}
				catch (Exception)
				{
					return null;
				}
			}
			return new ImageSourceConverter().ConvertFromString(text3) as ImageSource;
		}

		private void ConvertDrawingToIconFile(Drawing drawing, Color color, string iconFileName)
		{
			Image image = new Image();
			image.Source = new DrawingImage(drawing.Clone());
			ImageRecolor.ChangeSVGColor(image, new SolidColorBrush(color), true, false, false);
			IconBitmapEncoder iconBitmapEncoder = new IconBitmapEncoder();
			iconBitmapEncoder.UsePngCompression = false;
			foreach (double num in new double[] { 16.0, 24.0, 32.0, 40.0, 48.0 })
			{
				image.Arrange(new Rect(0.0, 0.0, num, num));
				RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)num, (int)num, 96.0, 96.0, PixelFormats.Pbgra32);
				renderTargetBitmap.Render(image);
				iconBitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
			}
			using (FileStream fileStream = new FileStream(iconFileName, FileMode.Create))
			{
				iconBitmapEncoder.Save(fileStream);
				fileStream.Close();
			}
		}

		private void AddOrUpdateTrayForController(ControllerVM controller)
		{
			try
			{
				KeyValuePair<ControllerVM, TaskbarIcon> keyValuePair = this.TrayForControllers.First((KeyValuePair<ControllerVM, TaskbarIcon> item) => item.Key.ID == controller.ID);
				if (keyValuePair.Key == controller)
				{
					this.UpdateTrayData(keyValuePair.Value, controller);
					return;
				}
				this.RemoveTrayForController(keyValuePair.Key);
			}
			catch (Exception)
			{
			}
			Tracer.TraceWrite(Tracer.PRODUCT_SHORT_NAME + " AddOrUpdateTrayForController " + controller.ControllerDisplayName, false);
			TaskbarIcon taskbarIcon = new TaskbarIcon();
			ContextMenu contextMenu = new ContextMenu();
			MenuItem item1 = new MenuItem();
			MenuItem item2 = new MenuItem();
			item1.Name = "AdjustAgentSettings";
			item1.Header = DTLocalization.GetString(11808);
			item1.Command = ((MainWindowViewModel)base.DataContext).AdjustAgentSettingsCommand;
			item1.CommandParameter = controller;
			contextMenu.Items.Add(item1);
			item2.Name = "HideBatteryIcon";
			item2.Header = DTLocalization.GetString(11843);
			item2.Click += delegate(object o, RoutedEventArgs e)
			{
				this.RemoveTrayForController(controller);
			};
			contextMenu.Items.Add(item2);
			TranslationManager.Instance.LanguageChanged += delegate([Nullable(2)] object o, EventArgs e)
			{
				try
				{
					item1.Header = DTLocalization.GetString(11808);
					item2.Header = DTLocalization.GetString(11843);
				}
				catch (Exception)
				{
				}
			};
			taskbarIcon.ContextMenu = contextMenu;
			this.UpdateTrayData(taskbarIcon, controller);
			this.mainGrid.Children.Add(taskbarIcon);
			this.TrayForControllers[controller] = taskbarIcon;
		}

		private void Controller_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (sender is ControllerVM && e.PropertyName == "ControllerBatteryLevel")
			{
				Tracer.TraceWrite(Tracer.PRODUCT_SHORT_NAME + " ControllerBatteryLevel changed", false);
				ControllerVM controller = sender as ControllerVM;
				try
				{
					this.UpdateTrayData(this.TrayForControllers.First((KeyValuePair<ControllerVM, TaskbarIcon> item) => item.Key.ID == controller.ID).Value, controller);
				}
				catch (Exception)
				{
					Tracer.TraceWrite(Tracer.PRODUCT_SHORT_NAME + " Controller_PropertyChanged controller not exist", false);
				}
			}
		}

		private void CheckAddController(BaseControllerVM item)
		{
			Tracer.TraceWrite(Tracer.PRODUCT_SHORT_NAME + " CheckAddController " + item.ControllerDisplayName, false);
			ControllerVM controllerVM = item as ControllerVM;
			if (controllerVM != null && controllerVM.HasGamepadControllers && controllerVM.IsControllerBatteryBlockVisible && controllerVM.IsOnline)
			{
				GamepadSettings gamepadSettings = Engine.GamepadService.GamepadsSettings.FirstOrDefault((GamepadSettings settings) => settings.ID == item.ID);
				if (this.ShowTrayIcons && (gamepadSettings == null || gamepadSettings.ShowBatteryInTaskbar))
				{
					this.AddOrUpdateTrayForController(controllerVM);
				}
			}
		}

		private void CheckRemoveController(BaseControllerVM item)
		{
			Tracer.TraceWrite(Tracer.PRODUCT_SHORT_NAME + " CheckRemoveController " + item.ControllerDisplayName, false);
			if (item is ControllerVM)
			{
				this.RemoveTrayForController(item as ControllerVM);
			}
		}

		private void RemoveTrayForController(ControllerVM controller)
		{
			try
			{
				Tracer.TraceWrite(Tracer.PRODUCT_SHORT_NAME + " RemoveTrayForController " + controller.ControllerDisplayName, false);
				KeyValuePair<ControllerVM, TaskbarIcon> keyValuePair = this.TrayForControllers.First((KeyValuePair<ControllerVM, TaskbarIcon> item) => item.Key.ID == controller.ID);
				if (keyValuePair.Value.TrayToolTipResolved != null)
				{
					keyValuePair.Value.TrayToolTipResolved.IsOpen = false;
				}
				keyValuePair.Value.Visibility = Visibility.Collapsed;
				keyValuePair.Value.Dispose();
				this.mainGrid.Children.Remove(keyValuePair.Value);
				this.TrayForControllers.Remove(keyValuePair.Key);
			}
			catch (Exception)
			{
				Tracer.TraceWrite(Tracer.PRODUCT_SHORT_NAME + " RemoveTrayForController controller not exist", false);
			}
		}

		private void UpdateTrays()
		{
			foreach (KeyValuePair<ControllerVM, TaskbarIcon> keyValuePair in ((IEnumerable<KeyValuePair<ControllerVM, TaskbarIcon>>)this.TrayForControllers))
			{
				this.UpdateTrayData(keyValuePair.Value, keyValuePair.Key);
			}
			using (IEnumerator<BaseControllerVM> enumerator2 = Engine.GamepadService.AllPhysicalControllers.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					BaseControllerVM item = enumerator2.Current;
					GamepadSettings gamepadSettings = Engine.GamepadService.GamepadsSettings.FirstOrDefault((GamepadSettings settings) => settings.ID == item.ID);
					if (gamepadSettings != null)
					{
						if (gamepadSettings.ShowBatteryInTaskbar && this.ShowTrayIcons)
						{
							this.CheckAddController(item);
						}
						else
						{
							this.CheckRemoveController(item);
						}
					}
				}
			}
		}

		private void UpdateDeviceName(string devID)
		{
			foreach (KeyValuePair<ControllerVM, TaskbarIcon> keyValuePair in ((IEnumerable<KeyValuePair<ControllerVM, TaskbarIcon>>)this.TrayForControllers))
			{
				if (keyValuePair.Key.ID == devID)
				{
					this.UpdateTrayData(keyValuePair.Value, keyValuePair.Key);
				}
			}
		}

		private void HideTrayMenuItem_Click(object sender, RoutedEventArgs e)
		{
			this.TrayIconsVisibility(false);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/reWASDEngine;component/views/mainwindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				this.mainGrid = (Grid)target;
				return;
			case 2:
				this.notifyIcon = (TaskbarIcon)target;
				return;
			case 3:
				this.RemapOnnOff = (MenuItem)target;
				return;
			case 4:
				this.OpenGui = (MenuItem)target;
				return;
			case 5:
				this.VisitCommunity = (MenuItem)target;
				return;
			case 6:
				this.HideTrayAgent = (MenuItem)target;
				this.HideTrayAgent.Click += this.HideTrayMenuItem_Click;
				return;
			case 7:
				this.CloseCommand = (MenuItem)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		protected static IntPtr _policyDll;

		private ObservableDictionary<ControllerVM, TaskbarIcon> TrayForControllers = new ObservableDictionary<ControllerVM, TaskbarIcon>();

		internal Grid mainGrid;

		internal TaskbarIcon notifyIcon;

		internal MenuItem RemapOnnOff;

		internal MenuItem OpenGui;

		internal MenuItem VisitCommunity;

		internal MenuItem HideTrayAgent;

		internal MenuItem CloseCommand;

		private bool _contentLoaded;
	}
}
