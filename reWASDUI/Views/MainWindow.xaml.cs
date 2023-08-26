using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using DiscSoft.NET.Common.DSBaseApp;
using DiscSoft.NET.Common.DSBaseWindow;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View.Controls;
using DiscSoft.NET.Common.View.Controls.ComboBoxes;
using Prism.Ioc;
using reWASDCommon.Infrastructure;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Controls.XBBindingControls.BindingFrame;
using reWASDUI.Infrastructure;
using reWASDUI.Services;
using reWASDUI.ViewModels;
using reWASDUI.ViewModels.Preferences;
using reWASDUI.Views.Preferences;
using XBEliteWPF.Infrastructure.reWASDMapping;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;

namespace reWASDUI.Views
{
	public partial class MainWindow : PositionStateRememberingWindow
	{
		private MainWindowViewModel _dataContext
		{
			get
			{
				return base.DataContext as MainWindowViewModel;
			}
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern uint RegisterWindowMessage(string lpString);

		public MainWindow()
		{
			this.InitializeComponent();
			base.ContentRendered += this.OnContentRendered;
			this._previousScreen = Screen.FromHandle(new WindowInteropHelper(this).Handle);
			EventManager.RegisterClassHandler(typeof(Window), FrameworkElement.LoadedEvent, new RoutedEventHandler(MainWindow.OnAllWindowLoaded), true);
			EventManager.RegisterClassHandler(typeof(Window), FrameworkElement.UnloadedEvent, new RoutedEventHandler(MainWindow.OnAllWindowUnloaded), true);
			base.SizeChanged += async delegate(object s, SizeChangedEventArgs e)
			{
				await Task.Delay(50);
				this.OnLocationChanged(EventArgs.Empty);
			};
			base.Loaded += delegate(object o, RoutedEventArgs s)
			{
				HwndSource.FromHwnd(new WindowInteropHelper(this).Handle).AddHook(new HwndSourceHook(this.WndProc));
			};
			base.SizeChanged += this.MainWindow_SizeChanged;
			base.DpiChanged += new System.Windows.DpiChangedEventHandler(this.DpiChangedChangedHandler);
			base.SourceInitialized += this.OnSourceInitialized;
		}

		private void OnSourceInitialized(object sender, EventArgs e)
		{
			App.SetupLocation(false);
		}

		private void DpiChangedChangedHandler(object sender, EventArgs e)
		{
			Screen screen = Screen.FromHandle(new WindowInteropHelper(this).Handle);
			if (screen == null)
			{
				return;
			}
			if (screen.DeviceName != this._previousScreen.DeviceName)
			{
				this.SetSizeOnCurrentScreen();
				this._previousScreen = screen;
			}
		}

		private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.SetSizeOnCurrentScreen();
		}

		private void SetSizeOnCurrentScreen()
		{
			Screen screen = Screen.FromHandle(new WindowInteropHelper(this).Handle);
			if (screen == null)
			{
				return;
			}
			PresentationSource presentationSource = PresentationSource.FromVisual(this);
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
			double num2 = (double)screen.Bounds.Width / num;
			double num3 = (double)screen.Bounds.Height / num;
		}

		~MainWindow()
		{
		}

		private static void OnAllWindowLoaded(object sender, RoutedEventArgs e)
		{
			Window window = sender as Window;
			if (window != null)
			{
				Window owner = window.Owner;
				System.Windows.Application application = System.Windows.Application.Current;
				if (owner == ((application != null) ? application.MainWindow : null))
				{
					MainWindow._modalCounter++;
					if (App.m_MainWnd != null)
					{
						DoubleAnimation doubleAnimation = new DoubleAnimation(0.8, TimeSpan.FromSeconds(0.2));
						App.m_MainWnd.MainContentGrid.BeginAnimation(UIElement.OpacityProperty, doubleAnimation);
						App.m_MainWnd.MainContentGridBack.Opacity = 1.0;
						doubleAnimation.Completed += delegate([Nullable(2)] object o, EventArgs e1)
						{
							App.m_MainWnd.MainContentGridBack.Opacity = 1.0;
						};
					}
				}
			}
		}

		private static void OnAllWindowUnloaded(object sender, RoutedEventArgs e)
		{
			Window window = sender as Window;
			if (window != null)
			{
				Window owner = window.Owner;
				System.Windows.Application application = System.Windows.Application.Current;
				if (owner == ((application != null) ? application.MainWindow : null) && MainWindow._modalCounter > 0)
				{
					MainWindow._modalCounter--;
					if (MainWindow._modalCounter == 0 && App.m_MainWnd != null)
					{
						DoubleAnimation doubleAnimation = new DoubleAnimation(1.0, TimeSpan.FromSeconds(0.2));
						doubleAnimation.Completed += delegate([Nullable(2)] object o, EventArgs e1)
						{
							if (MainWindow._modalCounter == 0)
							{
								App.m_MainWnd.MainContentGridBack.Opacity = 0.0;
							}
						};
						App.m_MainWnd.MainContentGrid.BeginAnimation(UIElement.OpacityProperty, doubleAnimation);
					}
				}
			}
		}

		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			MainWindow window;
			try
			{
				window = (MainWindow)HwndSource.FromHwnd(hwnd).RootVisual;
			}
			catch (Exception)
			{
				return IntPtr.Zero;
			}
			if ((long)msg == (long)((ulong)this.RestoreWindow))
			{
				System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
				{
					if (window.WindowState == WindowState.Minimized)
					{
						window.WindowState = WindowState.Normal;
					}
					window.Activate();
					if (wParam == (IntPtr)1)
					{
						App.GuiHelperService.ImportGameConfig(RegistryHelper.GetString("ImportParam", "GuiNamespace", "", false), false);
					}
					if (wParam == (IntPtr)2)
					{
						PreferencesWindow.ShowPreferences(typeof(PreferencesTrayAgentVM));
					}
				}));
			}
			return IntPtr.Zero;
		}

		private async void Window_Closing(object sender, CancelEventArgs e)
		{
			if (base.WindowState == WindowState.Minimized)
			{
				base.WindowState = WindowState.Normal;
			}
			if (this._dataContext != null && !HttpClientService.InitializationIsFailedStatic)
			{
				e.Cancel = true;
				TaskAwaiter<GameChangedResult> taskAwaiter = this._dataContext.GameProfilesService.TryAskUserToSaveChanges(true).GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<GameChangedResult> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<GameChangedResult>);
				}
				if (taskAwaiter.GetResult() != 3)
				{
					await App.HttpClientService.Gamepad.DeleteAllExclusiveCaptureProfiles();
					Environment.Exit(0);
				}
			}
		}

		private async void OnContentRendered(object s, EventArgs e)
		{
			await ((App)System.Windows.Application.Current).InitAfterShowWindow();
			await App.GamepadService.WaitForServiceInited();
			this.content.Content = base.Resources["FullWindowTemplate"];
			this.content.Visibility = Visibility.Visible;
			this.initializeContent.Visibility = Visibility.Collapsed;
			base.DataContext = IContainerProviderExtensions.Resolve<MainWindowViewModel>(App.Container);
			base.PreviewKeyDown += this._dataContext.OnMainWindowKeyDown;
			base.PreviewKeyDown += this.MainWindow_OnKeyDown;
			base.PreviewKeyUp += this.MainWindow_OnKeyUp;
			App.WindowsMessageProcessor.Attach(this, true);
			App.LicensingService.OnLicenseFailed += delegate
			{
				TrayAgentCommunicator.ShutdownAgentAndEngine();
			};
			App.LicensingService.OnLicenseActivated += delegate
			{
				TrayAgentCommunicator.SyncAutorunValue();
			};
			DSBaseApplication dsbaseApplication = System.Windows.Application.Current as DSBaseApplication;
			if (dsbaseApplication != null)
			{
				dsbaseApplication.RaiseOnMainWindowContentRendered(s, e);
			}
			((App)System.Windows.Application.Current).InitAfterContentRendered();
		}

		private void MainWindow_OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (App.DeviceDetectionService.IsEnabled)
			{
				e.Handled = true;
				return;
			}
			bool flag = false;
			object originalSource = e.OriginalSource;
			ComboBoxItem comboBoxItem = originalSource as ComboBoxItem;
			if (comboBoxItem == null)
			{
				ColoredComboBox coloredComboBox = originalSource as ColoredComboBox;
				if (coloredComboBox == null)
				{
					if (!(originalSource is WaterMarkTextBox))
					{
						DependencyObject dependencyObject = originalSource as DependencyObject;
						if (dependencyObject != null)
						{
							flag = VisualTreeHelperExt.FindParent<BindingFrameUC>(dependencyObject, null) != null;
						}
					}
				}
				else
				{
					ComboBoxAdaptor comboBoxAdaptor = VisualTreeHelperExt.FindParent<ComboBoxAdaptor>(coloredComboBox, new ushort?((ushort)2));
					if (comboBoxAdaptor != null)
					{
						IEnumerable itemsSource = comboBoxAdaptor.ItemsSource;
					}
					flag = coloredComboBox.ItemsSource is ObservableCollection<BaseRewasdMapping> || coloredComboBox.ItemsSource is ObservableCollection<KeyScanCodeV2>;
					if (!flag)
					{
						ComboBoxAdaptor comboBoxAdaptor2 = VisualTreeHelperExt.FindParent<ComboBoxAdaptor>(coloredComboBox, new ushort?((ushort)2));
						object obj = ((comboBoxAdaptor2 != null) ? comboBoxAdaptor2.ItemsSource : null);
						flag = obj is ObservableCollection<BaseRewasdMapping> || obj is ObservableCollection<KeyScanCodeV2>;
					}
				}
			}
			else
			{
				flag = comboBoxItem.DataContext is KeyScanCodeV2;
			}
			if (flag)
			{
				Key key = ((e.Key == Key.System) ? e.SystemKey : e.Key);
				App.EventAggregator.GetEvent<KeyboardKeyPressed>().Publish(key);
			}
		}

		private void MainWindow_OnKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.OriginalSource is System.Windows.Controls.TextBox)
			{
				return;
			}
			ComboBoxItem comboBoxItem = e.OriginalSource as ComboBoxItem;
			if (comboBoxItem != null && !(comboBoxItem.DataContext is KeyScanCodeV2))
			{
				return;
			}
			ColoredComboBox coloredComboBox = e.OriginalSource as ColoredComboBox;
			if (coloredComboBox != null && !(coloredComboBox.ItemsSource is ObservableCollection<KeyScanCodeV2>))
			{
				return;
			}
			Key key = ((e.Key == Key.System) ? e.SystemKey : e.Key);
			if (key == Key.Snapshot)
			{
				App.EventAggregator.GetEvent<KeyboardKeyPressed>().Publish(key);
			}
			App.EventAggregator.GetEvent<KeyboardKeyReleased>().Publish(key);
		}

		private void MainWindow_OnManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
		{
			e.Handled = true;
		}

		public static IntPtr _policyDll;

		private uint RestoreWindow = MainWindow.RegisterWindowMessage("XBOXEliteWPFWPFRestoreWindow");

		private Screen _previousScreen;

		private static int _modalCounter;
	}
}
