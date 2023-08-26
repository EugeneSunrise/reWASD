using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using DiscSoft.NET.Common.Interfaces;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.AttachedBehaviours;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.View.Controls;
using DiscSoft.NET.Common.View.Controls.Buttons;
using Prism.Ioc;
using Prism.Regions;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.MacroCompilers;
using reWASDUI.Controls;
using reWASDUI.DataModels;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.ActivatorXB;
using reWASDUI.Infrastructure.KeyBindings.MacroBinding;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.ViewModels;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.KeyBindingsModel.MacroBinding;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Views.ContentZoneGamepad.Macro
{
	public partial class MacroSettings : BaseServicesDataContextControl, IKeyPressedEventHandler, ISelectableContainer, INavigationAware, INotifyPropertyChanged
	{
		public int RecordingCounter
		{
			get
			{
				return (int)base.GetValue(MacroSettings.RecordingCounterProperty);
			}
			set
			{
				base.SetValue(MacroSettings.RecordingCounterProperty, value);
			}
		}

		public bool IsRecording
		{
			get
			{
				return (bool)base.GetValue(MacroSettings.IsRecordingProperty);
			}
			set
			{
				base.SetValue(MacroSettings.IsRecordingProperty, value);
			}
		}

		public bool IsFinalizingRecording
		{
			get
			{
				return (bool)base.GetValue(MacroSettings.IsFinalizingRecordingProperty);
			}
			set
			{
				base.SetValue(MacroSettings.IsFinalizingRecordingProperty, value);
			}
		}

		private static void XBBindingPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MacroSettings macroSettings = d as MacroSettings;
			if (macroSettings != null)
			{
				macroSettings.MacroListView_OnSelectionChanged(null, null);
			}
		}

		public XBBinding XBBinding
		{
			get
			{
				return (XBBinding)base.GetValue(MacroSettings.XBBindingProperty);
			}
			set
			{
				base.SetValue(MacroSettings.XBBindingProperty, value);
			}
		}

		public Type BindingFrameViewTypeToReturnBack
		{
			get
			{
				return (Type)base.GetValue(MacroSettings.BindingFrameViewTypeToReturnBackProperty);
			}
			set
			{
				base.SetValue(MacroSettings.BindingFrameViewTypeToReturnBackProperty, value);
			}
		}

		public MacroSettings()
		{
			this.InitializeComponent();
			base.Loaded += this.OnLoaded;
			base.SizeChanged += delegate(object s, SizeChangedEventArgs e)
			{
				this.OnPropertyChanged("MacroDurationWidth");
				this.OnPropertyChanged("MacroDurationIsWrapped");
			};
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(name));
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			this.ItemsControl = base.Template.FindName("MacroListView", this) as ListBox;
			Grid grid = base.Template.FindName("MacroListViewGrid", this) as Grid;
			if (grid != null)
			{
				DragSelection.AttachDragSelection(grid, this.ItemsControl, this, new Func<Rect, List<object>>(this.GetICItemByMouseRect), true);
			}
			this.MacroListView_OnSelectionChanged(null, null);
			this.OnPropertyChanged("MacroDurationWidth");
			this.OnPropertyChanged("MacroDurationIsWrapped");
		}

		private List<object> GetICItemByMouseRect(Rect rectToSearch)
		{
			List<object> list = new List<object>();
			foreach (object obj in ((IEnumerable)this.ItemsControl.Items))
			{
				ListBoxItem listBoxItem = this.ItemsControl.ItemContainerGenerator.ContainerFromItem(obj) as ListBoxItem;
				if (listBoxItem != null)
				{
					Rect rect = listBoxItem.TransformToAncestor(this.ItemsControl).TransformBounds(new Rect(0.0, 0.0, listBoxItem.ActualWidth, listBoxItem.ActualHeight));
					if (rectToSearch.IntersectsWith(rect))
					{
						list.Add(listBoxItem);
					}
				}
			}
			return list;
		}

		public double MacroDurationWidth
		{
			get
			{
				return (base.Template.FindName("durationBorder", this) as Border).ActualWidth - (base.Template.FindName("durationTagContainer", this) as StackPanel).ActualWidth - 20.0;
			}
		}

		public bool MacroDurationIsWrapped
		{
			get
			{
				return this.IsTextTrimmed();
			}
		}

		private bool IsTextTrimmed()
		{
			Typeface typeface = new Typeface(base.FontFamily, base.FontStyle, base.FontWeight, base.FontStretch);
			return (int)new FormattedText(this.XBBinding.CurrentActivatorXBBinding.MacroSequence.TotalComboTime, CultureInfo.CurrentCulture, base.FlowDirection, typeface, base.FontSize, base.Foreground, VisualTreeHelper.GetDpi(this).PixelsPerDip).Width > (int)(base.Template.FindName("TotalComboTime", this) as TextBlock).ActualWidth;
		}

		private void BtnStartRecord_OnClick(object sender, RoutedEventArgs e)
		{
			this.StartKeyboardRecorder();
			this.StartGamepadRecorder();
		}

		private void BtnStopRecord_OnClick(object sender, RoutedEventArgs e)
		{
			this.StopGamepadRecorder();
			this.StopKeyboardRecorder();
		}

		private void OnMouseDown(object o, MouseButtonEventArgs e)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 1);
			defaultInterpolatedStringHandler.AppendLiteral("MacroRecorder.OnMouseDown e: ");
			defaultInterpolatedStringHandler.AppendFormatted<MouseButton>(e.ChangedButton);
			Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
			this.ProcessKey(e.ChangedButton, false);
		}

		private void OnMouseUp(object o, MouseButtonEventArgs e)
		{
			if (this._macroSequence.IsHoldUntilRelease)
			{
				return;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 1);
			defaultInterpolatedStringHandler.AppendLiteral("MacroRecorder.OnMouseUp e: ");
			defaultInterpolatedStringHandler.AppendFormatted<MouseButton>(e.ChangedButton);
			Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
			this.ProcessKey(e.ChangedButton, true);
		}

		private void StartKeyboardRecorder()
		{
			Tracer.TraceWrite("StartKeyboardRecorder", false);
			this.CleanAnalogStates();
			this._globalKeyboardHook = new GlobalKeyboardHook();
			this._globalKeyboardHook.KeyboardPressed += this.OnKeyPressed;
			base.Focus();
			this.IsRecording = true;
			this.RecordingCounter = 0;
			Application.Current.MainWindow.IsEnabled = false;
			if (this._guiMacroSequence.ControllerButton.IsKeyScanCode)
			{
				this._macroSequence = new MacroSequence(this._guiMacroSequence.ControllerButton.KeyScanCode, this._guiMacroSequence.MacroSequenceType, this._guiMacroSequence.ActivatorType);
			}
			else
			{
				this._macroSequence = new MacroSequence(this._guiMacroSequence.ControllerButton.GamepadButton, this._guiMacroSequence.MacroSequenceType, this._guiMacroSequence.ActivatorType);
			}
			this._macroSequence.AddCrutchHoldItem(false);
			this.ItemsControl.Focus();
		}

		[DllImport("kernel32.dll")]
		private static extern ulong GetTickCount64();

		private async void StopKeyboardRecorder()
		{
			Tracer.TraceWrite("StopKeyboardRecorder", false);
			if (this._globalKeyboardHook != null)
			{
				this._globalKeyboardHook.KeyboardPressed -= this.OnKeyPressed;
			}
			GlobalKeyboardHook globalKeyboardHook = this._globalKeyboardHook;
			if (globalKeyboardHook != null)
			{
				globalKeyboardHook.Dispose();
			}
			if (this._currentPause != null)
			{
				MacroSequence macroSequence = this._macroSequence;
				if (macroSequence != null)
				{
					macroSequence.Remove(this._currentPause);
				}
				this._currentPause = null;
			}
			if (this._macroSequence != null)
			{
				this.IsFinalizingRecording = true;
				this._guiMacroSequence.SuppressNotification = true;
				await this._guiMacroSequence.AsyncAddMacrosFromModel(this._macroSequence);
				this._guiMacroSequence.SuppressNotification = false;
				MacroSequence macroSequence2 = this._macroSequence;
				if (macroSequence2 != null && macroSequence2.Count > 0)
				{
					ThreadHelper.ExecuteInMainDispatcher(delegate
					{
						this.XBBinding.HostCollection.SubConfigData.ConfigData.IsChanged = true;
						this.XBBinding.HostCollection.SubConfigData.ConfigData.CheckVirtualMappingsExist();
						this.XBBinding.CurrentActivatorXBBinding.IsMacroSequenceChanged = true;
					}, true);
				}
				this.IsFinalizingRecording = false;
				this._guiMacroSequence.KeyDelaySilentChange = 0;
			}
			this.IsRecording = false;
			this.RecordingCounter = 0;
			this._macroSequence = null;
			Application.Current.MainWindow.IsEnabled = true;
		}

		private void OnKeyPressed(object sender, GlobalKeyboardHookEventArgs e)
		{
			bool flag = e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyUp || e.KeyboardState == GlobalKeyboardHook.KeyboardState.SysKeyUp;
			Key key = KeyInterop.KeyFromVirtualKey(e.KeyboardData.VirtualCode);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
			if (!flag)
			{
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 1);
				defaultInterpolatedStringHandler.AppendLiteral("MacroRecorder.OnKeyDown e: ");
				defaultInterpolatedStringHandler.AppendFormatted<Key>(key);
				Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
				this.ProcessKey(key, false);
				return;
			}
			if (this._macroSequence.IsHoldUntilRelease)
			{
				return;
			}
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 1);
			defaultInterpolatedStringHandler.AppendLiteral("MacroRecorder.OnKeyUp e: ");
			defaultInterpolatedStringHandler.AppendFormatted<Key>(key);
			Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
			if (key == Key.Snapshot)
			{
				this.ProcessKey(key, false);
			}
			this.ProcessKey(key, true);
		}

		private void StartGamepadRecorder()
		{
			IKeyPressedPollerService keyPressedPollerService = IContainerProviderExtensions.Resolve<IKeyPressedPollerService>(App.Container);
			if (keyPressedPollerService != null)
			{
				keyPressedPollerService.Subscribe(this, true);
			}
			if (keyPressedPollerService == null)
			{
				return;
			}
			keyPressedPollerService.StartAllGamepadsPolling();
		}

		private void StopGamepadRecorder()
		{
			IKeyPressedPollerService keyPressedPollerService = IContainerProviderExtensions.Resolve<IKeyPressedPollerService>(App.Container);
			if (keyPressedPollerService != null)
			{
				keyPressedPollerService.StopAllGamepadsPolling();
			}
			if (keyPressedPollerService == null)
			{
				return;
			}
			keyPressedPollerService.Unsubscribe(this);
		}

		private void ProcessKey(Key key, bool isUp)
		{
			if (this._curProcessingKey == key && !isUp)
			{
				return;
			}
			this._curProcessingKey = (isUp ? Key.None : key);
			KeyScanCodeV2 keyScanCodeV = KeyScanCodeV2.FindKeyScanCodeByKey(key);
			if (keyScanCodeV == KeyScanCodeV2.NoMap || (string.IsNullOrWhiteSpace(keyScanCodeV.Description) && string.IsNullOrWhiteSpace(keyScanCodeV.AltDescription)))
			{
				return;
			}
			this.ProcessKey(keyScanCodeV, isUp);
		}

		private void ProcessKey(MouseButton mouseButton, bool isUp)
		{
			this.ProcessKey(KeyScanCodeV2.FindKeyScanCodeByMouseButton(mouseButton), isUp);
		}

		private void ProcessKey(KeyScanCodeV2 ksc, bool isUp)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 2);
			defaultInterpolatedStringHandler.AppendLiteral("ProcessKey ksc:");
			defaultInterpolatedStringHandler.AppendFormatted<KeyScanCodeV2>(ksc);
			defaultInterpolatedStringHandler.AppendLiteral(" isUp:");
			defaultInterpolatedStringHandler.AppendFormatted<bool>(isUp);
			Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
			MacroSequence macroSequence = this._macroSequence;
			if (macroSequence != null)
			{
				macroSequence.InsertOrAdd(new MacroKeyBinding(ksc, isUp ? 1 : 0), int.MaxValue, false);
			}
			int recordingCounter = this.RecordingCounter;
			this.RecordingCounter = recordingCounter + 1;
			this.ResolvePause(isUp);
		}

		private void ResolvePause(bool isUp)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 1);
			defaultInterpolatedStringHandler.AppendLiteral("ResolvePause isUp:");
			defaultInterpolatedStringHandler.AppendFormatted<bool>(isUp);
			Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
			if (this._macroSequence == null)
			{
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 1);
				defaultInterpolatedStringHandler.AppendLiteral("ResolvePause isUp:");
				defaultInterpolatedStringHandler.AppendFormatted<bool>(isUp);
				defaultInterpolatedStringHandler.AppendLiteral(" MacroSequence is null");
				Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
				return;
			}
			if (this._currentPause != null)
			{
				this._currentPause.Duration = (uint)(MacroSettings.GetTickCount64() - this._startPauseTickTime);
				if (this._currentPause.Duration < 10U)
				{
					MacroSequence macroSequence = this._macroSequence;
					if (macroSequence != null)
					{
						macroSequence.Remove(this._currentPause);
					}
					MacroSequence macroSequence2 = this._macroSequence;
					if (macroSequence2 != null)
					{
						macroSequence2.RemoveCrutchHoldItem();
					}
					MacroSequence macroSequence3 = this._macroSequence;
					if (macroSequence3 != null)
					{
						macroSequence3.AddCrutchHoldItem(false);
					}
				}
			}
			this._startPauseTickTime = MacroSettings.GetTickCount64();
			this._currentPause = new MacroPause(0U);
			MacroSequence macroSequence4 = this._macroSequence;
			if (macroSequence4 == null)
			{
				return;
			}
			macroSequence4.InsertOrAdd(this._currentPause, int.MaxValue, false);
		}

		private void CleanAnalogStates()
		{
			this._prevLeftStickX = 0;
			this._prevLeftStickY = 0;
			this._prevRightStickX = 0;
			this._prevRightStickY = 0;
			this._prevLeftTrigger = 0;
			this._prevRightTrigger = 0;
			this._rightTriggerAdded = false;
			this._leftTriggerAdded = false;
			foreach (KeyValuePair<GamepadButton, MacroSettings.ButtonState> keyValuePair in this.AnalogButtons)
			{
				keyValuePair.Value.PrevValue = 0;
				keyValuePair.Value.IsAdded = false;
			}
		}

		private bool AddStickMacro(short StickValue, ref short prevStickValue, GamepadButton gpd)
		{
			bool flag = false;
			if (Math.Abs((long)StickValue) < 3280L)
			{
				StickValue = 0;
			}
			if (Math.Abs((long)prevStickValue - (long)StickValue) > 327L)
			{
				flag = true;
				prevStickValue = StickValue;
				MacroSequence macroSequence = this._macroSequence;
				if (macroSequence != null)
				{
					macroSequence.InsertOrAdd(new MacroGamepadStick(GamepadButtonDescription.GamepadButtonDescriptionDictionary[gpd], 0)
					{
						DeflectionPercentage = StickValue / 327
					}, int.MaxValue, false);
				}
				ThreadHelper.ExecuteInMainDispatcher(delegate
				{
					int recordingCounter = this.RecordingCounter;
					this.RecordingCounter = recordingCounter + 1;
				}, false);
			}
			return flag;
		}

		private bool AddTriggerMacro(ushort Value, ref ushort prevalue, GamepadButton gpd)
		{
			bool flag = false;
			if (Math.Abs((long)((ulong)Value)) < 3280L)
			{
				Value = 0;
			}
			if (Math.Abs((long)((ulong)prevalue - (ulong)Value)) > 327L)
			{
				flag = true;
				prevalue = Value;
				MacroSequence macroSequence = this._macroSequence;
				if (macroSequence != null)
				{
					macroSequence.InsertOrAdd(new MacroGamepadBinding(GamepadButtonDescription.GamepadButtonDescriptionDictionary[gpd], 0)
					{
						DeflectionPercentage = (short)(Value / 327)
					}, int.MaxValue, false);
				}
				ThreadHelper.ExecuteInMainDispatcher(delegate
				{
					int recordingCounter = this.RecordingCounter;
					this.RecordingCounter = recordingCounter + 1;
				}, false);
			}
			return flag;
		}

		private bool AddAnalogMacro(byte Value, ref byte prevalue, GamepadButton gpd)
		{
			bool flag = false;
			if (Math.Abs((short)Value) < 26)
			{
				Value = 0;
			}
			if (Math.Abs((int)(prevalue - Value)) > 3)
			{
				flag = true;
				prevalue = Value;
				MacroSequence macroSequence = this._macroSequence;
				if (macroSequence != null)
				{
					macroSequence.InsertOrAdd(new MacroGamepadBinding(GamepadButtonDescription.GamepadButtonDescriptionDictionary[gpd], 0)
					{
						DeflectionPercentage = (short)((double)Value / 2.55)
					}, int.MaxValue, false);
				}
				ThreadHelper.ExecuteInMainDispatcher(delegate
				{
					int recordingCounter = this.RecordingCounter;
					this.RecordingCounter = recordingCounter + 1;
				}, false);
			}
			return flag;
		}

		public void OnControllerPollState(GamepadState gamepadState)
		{
			bool flag = false;
			flag |= this.AddStickMacro(gamepadState.LeftStickX, ref this._prevLeftStickX, 43);
			flag |= this.AddStickMacro(gamepadState.LeftStickY, ref this._prevLeftStickY, 40);
			flag |= this.AddStickMacro(gamepadState.RightStickX, ref this._prevRightStickX, 50);
			flag |= this.AddStickMacro(gamepadState.RightStickY, ref this._prevRightStickY, 47);
			if (this._leftTriggerPressed)
			{
				bool flag2 = this.AddTriggerMacro(gamepadState.LeftTrigger, ref this._prevLeftTrigger, 51);
				this._leftTriggerAdded = this._leftTriggerAdded || flag2;
				flag = flag || flag2;
			}
			if (this._rightTriggerPressed)
			{
				bool flag3 = this.AddTriggerMacro(gamepadState.RightTrigger, ref this._prevRightTrigger, 55);
				this._rightTriggerAdded = this._rightTriggerAdded || flag3;
				flag = flag || flag3;
			}
			GameVM currentGame = App.GameProfilesService.CurrentGame;
			bool flag4;
			if (currentGame == null)
			{
				flag4 = false;
			}
			else
			{
				ConfigVM currentConfig = currentGame.CurrentConfig;
				VirtualGamepadType? virtualGamepadType = ((currentConfig != null) ? new VirtualGamepadType?(currentConfig.ConfigData.VirtualGamepadType) : null);
				VirtualGamepadType virtualGamepadType2 = 3;
				flag4 = (virtualGamepadType.GetValueOrDefault() == virtualGamepadType2) & (virtualGamepadType != null);
			}
			if (flag4)
			{
				BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
				bool flag5;
				if (currentGamepad == null)
				{
					flag5 = false;
				}
				else
				{
					ControllerVM currentController = currentGamepad.CurrentController;
					ControllerTypeEnum? controllerTypeEnum = ((currentController != null) ? currentController.FirstGamepadType : null);
					ControllerTypeEnum controllerTypeEnum2 = 5;
					flag5 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
				}
				if (flag5)
				{
					foreach (KeyValuePair<GamepadButton, MacroSettings.ButtonState> keyValuePair in this.AnalogButtons)
					{
						if (keyValuePair.Value.IsPressed)
						{
							byte ds3AnalogPressureFromGamepadButton = gamepadState.GetDS3AnalogPressureFromGamepadButton(keyValuePair.Key);
							bool flag6 = this.AddAnalogMacro(ds3AnalogPressureFromGamepadButton, ref keyValuePair.Value.PrevValue, keyValuePair.Key);
							keyValuePair.Value.IsAdded = flag6;
							flag = flag || flag6;
						}
					}
				}
			}
			if (flag)
			{
				this.ResolvePause(false);
			}
		}

		private void AddKeyUpOrDown(GamepadButtonDescription button, bool isUp)
		{
			ThreadHelper.ExecuteInMainDispatcher(delegate
			{
				MacroSequence macroSequence = this._macroSequence;
				if (macroSequence != null)
				{
					macroSequence.InsertOrAdd(new MacroGamepadBinding(button, isUp ? 1 : 0), int.MaxValue, false);
				}
				int recordingCounter = this.RecordingCounter;
				this.RecordingCounter = recordingCounter + 1;
				this.ResolvePause(false);
			}, false);
		}

		public void OnControllerKeyDown(List<GamepadButtonDescription> buttons)
		{
			if (buttons == null)
			{
				return;
			}
			GameVM currentGame = App.GameProfilesService.CurrentGame;
			bool flag;
			if (currentGame == null)
			{
				flag = false;
			}
			else
			{
				ConfigVM currentConfig = currentGame.CurrentConfig;
				VirtualGamepadType? virtualGamepadType = ((currentConfig != null) ? new VirtualGamepadType?(currentConfig.ConfigData.VirtualGamepadType) : null);
				VirtualGamepadType virtualGamepadType2 = 3;
				flag = (virtualGamepadType.GetValueOrDefault() == virtualGamepadType2) & (virtualGamepadType != null);
			}
			bool flag2 = flag;
			BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
			bool flag3;
			if (currentGamepad == null)
			{
				flag3 = false;
			}
			else
			{
				ControllerVM currentController = currentGamepad.CurrentController;
				ControllerTypeEnum? controllerTypeEnum = ((currentController != null) ? currentController.FirstGamepadType : null);
				ControllerTypeEnum controllerTypeEnum2 = 5;
				flag3 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
			}
			bool flag4 = flag3;
			foreach (GamepadButtonDescription gamepadButtonDescription in buttons)
			{
				if (MacroCompiler.IsVirtualGamepadDigitalButtonValid(gamepadButtonDescription.Button, 1U) && !GamepadButtonExtensions.IsAnyStick(gamepadButtonDescription.Button) && !GamepadButtonExtensions.IsAnyTrigger(gamepadButtonDescription.Button) && !GamepadButtonExtensions.IsAnyPaddle(gamepadButtonDescription.Button) && (!GamepadButtonExtensions.IsDS3AnalogButton(gamepadButtonDescription.Button) || !flag2 || !flag4))
				{
					this.AddKeyUpOrDown(gamepadButtonDescription, false);
				}
				if (GamepadButtonExtensions.IsLeftTrigger(gamepadButtonDescription.Button))
				{
					this._leftTriggerPressed = true;
					this._prevLeftTrigger = 0;
				}
				if (GamepadButtonExtensions.IsRightTrigger(gamepadButtonDescription.Button))
				{
					this._rightTriggerPressed = true;
					this._prevRightTrigger = 0;
				}
				if (GamepadButtonExtensions.IsDS3AnalogButton(gamepadButtonDescription.Button) && flag2 && flag4)
				{
					foreach (KeyValuePair<GamepadButton, MacroSettings.ButtonState> keyValuePair in this.AnalogButtons)
					{
						if (gamepadButtonDescription.Button == keyValuePair.Key)
						{
							keyValuePair.Value.PrevValue = 0;
							keyValuePair.Value.IsPressed = true;
							break;
						}
					}
				}
			}
		}

		public void OnControllerKeyUp(List<GamepadButtonDescription> buttons)
		{
			if (buttons == null)
			{
				return;
			}
			GameVM currentGame = App.GameProfilesService.CurrentGame;
			bool flag;
			if (currentGame == null)
			{
				flag = false;
			}
			else
			{
				ConfigVM currentConfig = currentGame.CurrentConfig;
				VirtualGamepadType? virtualGamepadType = ((currentConfig != null) ? new VirtualGamepadType?(currentConfig.ConfigData.VirtualGamepadType) : null);
				VirtualGamepadType virtualGamepadType2 = 3;
				flag = (virtualGamepadType.GetValueOrDefault() == virtualGamepadType2) & (virtualGamepadType != null);
			}
			bool flag2 = flag;
			BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
			bool flag3;
			if (currentGamepad == null)
			{
				flag3 = false;
			}
			else
			{
				ControllerVM currentController = currentGamepad.CurrentController;
				ControllerTypeEnum? controllerTypeEnum = ((currentController != null) ? new ControllerTypeEnum?(currentController.FirstControllerType) : null);
				ControllerTypeEnum controllerTypeEnum2 = 5;
				flag3 = (controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null);
			}
			bool flag4 = flag3;
			foreach (GamepadButtonDescription gamepadButtonDescription in buttons)
			{
				if (MacroCompiler.IsVirtualGamepadDigitalButtonValid(gamepadButtonDescription.Button, 1U) && !GamepadButtonExtensions.IsAnyStick(gamepadButtonDescription.Button) && !GamepadButtonExtensions.IsAnyTrigger(gamepadButtonDescription.Button) && !GamepadButtonExtensions.IsAnyPaddle(gamepadButtonDescription.Button) && (!GamepadButtonExtensions.IsDS3AnalogButton(gamepadButtonDescription.Button) || !flag2 || !flag4))
				{
					this.AddKeyUpOrDown(gamepadButtonDescription, true);
				}
				if (GamepadButtonExtensions.IsLeftTrigger(gamepadButtonDescription.Button))
				{
					if (this._leftTriggerAdded)
					{
						this.AddKeyUpOrDown(gamepadButtonDescription, true);
					}
					this._leftTriggerAdded = false;
					this._leftTriggerPressed = false;
				}
				if (GamepadButtonExtensions.IsRightTrigger(gamepadButtonDescription.Button))
				{
					if (this._rightTriggerAdded)
					{
						this.AddKeyUpOrDown(gamepadButtonDescription, true);
					}
					this._rightTriggerAdded = false;
					this._rightTriggerPressed = false;
				}
				if (GamepadButtonExtensions.IsDS3AnalogButton(gamepadButtonDescription.Button) && flag2 && flag4)
				{
					foreach (KeyValuePair<GamepadButton, MacroSettings.ButtonState> keyValuePair in this.AnalogButtons)
					{
						if (gamepadButtonDescription.Button == keyValuePair.Key)
						{
							if (keyValuePair.Value.IsAdded)
							{
								this.AddKeyUpOrDown(gamepadButtonDescription, true);
							}
							keyValuePair.Value.IsAdded = false;
							keyValuePair.Value.IsPressed = false;
							break;
						}
					}
				}
			}
		}

		private void DisposeDragProcess()
		{
			this.DisposeDragInit();
			this.DisposeDragWindow();
			this._guiMacroSequence.ForEach(delegate(BaseMacro m)
			{
				m.IsBeingDragged = false;
			});
			this._guiMacroSequence.ForEach(delegate(BaseMacro m)
			{
				m.IsBeingDraggedOverFromRight = false;
			});
			this._guiMacroSequence.ForEach(delegate(BaseMacro m)
			{
				m.IsBeingDraggedOverFromLeft = false;
			});
		}

		private void DisposeDragInit()
		{
			this.dragStartEvalutionPoint = null;
			this._draggedItem = null;
		}

		private void DisposeDragWindow()
		{
			if (this._dragdropWindow != null)
			{
				Mouse.SetCursor(Cursors.Arrow);
				this._dragdropWindow.Close();
				this._dragdropWindow = null;
			}
		}

		private void MacroListView_DragEnter(object sender, DragEventArgs e)
		{
			int num = this.GetCurrentIndex((ExtendedListBox)sender, new MacroSettings.GetPositionDelegate(e.GetPosition), 0);
			if (num == -1)
			{
				num = this.GetCurrentIndex((ExtendedListBox)sender, new MacroSettings.GetPositionDelegate(e.GetPosition), -10);
			}
			if (num == -1)
			{
				return;
			}
			BaseMacro baseMacro = (BaseMacro)((ExtendedListBox)sender).Items[num];
			int num2 = this._guiMacroSequence.IndexOf(this._draggedItem);
			if (num == num2)
			{
				baseMacro.IsBeingDraggedOverFromRight = false;
				baseMacro.IsBeingDraggedOverFromLeft = false;
				return;
			}
			this._guiMacroSequence.ForEach(delegate(BaseMacro m)
			{
				m.IsBeingDraggedOverFromRight = false;
			});
			this._guiMacroSequence.ForEach(delegate(BaseMacro m)
			{
				m.IsBeingDraggedOverFromLeft = false;
			});
			if (!baseMacro.IsSelected && (this.isMultiSelectDragItems || this.IsValidPositionForDrop(this._draggedItem, baseMacro)))
			{
				bool flag = num > num2;
				baseMacro.IsBeingDraggedOverFromRight = !flag;
				baseMacro.IsBeingDraggedOverFromLeft = flag;
				Mouse.SetCursor(Cursors.Hand);
				return;
			}
			Mouse.SetCursor(Cursors.No);
		}

		private void MacroListView_PreviewDrop(object sender, DragEventArgs e)
		{
			BaseMacro baseMacro = e.Data.GetData("Source") as BaseMacro;
			BaseMacro baseMacro2 = this._guiMacroSequence.FirstOrDefault((BaseMacro m) => m.IsBeingDraggedOverFromRight || m.IsBeingDraggedOverFromLeft);
			if (baseMacro2 == null)
			{
				return;
			}
			if (baseMacro != null && !baseMacro2.IsSelected && (this.isMultiSelectDragItems || this.IsValidPositionForDrop(this._draggedItem, baseMacro2)))
			{
				baseMacro2.IsBeingDragged = false;
				((MacroSequence)this.ItemsControl.ItemsSource).MoveSelectedItems(baseMacro2);
			}
			this._guiMacroSequence.ForEach(delegate(BaseMacro m)
			{
				m.IsBeingDraggedOverFromRight = false;
			});
			this._guiMacroSequence.ForEach(delegate(BaseMacro m)
			{
				m.IsBeingDraggedOverFromLeft = false;
			});
			this._guiMacroSequence.ForEach(delegate(BaseMacro m)
			{
				m.IsHighlighted = false;
			});
			Mouse.SetCursor(Cursors.Arrow);
		}

		private void MacroItemPreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			this.dragStartEvalutionPoint = new Point?(e.GetPosition(this.ItemsControl));
			this._draggedItem = ((ListBoxItem)sender).Content as BaseMacro;
			this.isMultiSelectDragItems = this.ItemsControl.SelectedItems.Count > 1;
		}

		private void MacroItemPreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			this.DisposeDragInit();
		}

		private void MacroItemPreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton != MouseButtonState.Pressed || this.dragStartEvalutionPoint == null || this._draggedItem == null)
			{
				return;
			}
			if (this._draggedItem is MacroCrutchHoldUntillRelease)
			{
				return;
			}
			Point position = e.GetPosition(this.ItemsControl);
			Vector vector = this.dragStartEvalutionPoint.Value - position;
			if (Math.Abs(vector.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(vector.Y) > SystemParameters.MinimumVerticalDragDistance)
			{
				DataObject dataObject = new DataObject();
				if (this._draggedItem != null)
				{
					this._draggedItem.IsBeingDragged = true;
					dataObject.SetData("Source", this._draggedItem);
					this.CreateDragDropWindow((ListBoxItem)sender);
					DragDrop.DoDragDrop(sender as DependencyObject, dataObject, DragDropEffects.Move);
				}
				e.Handled = true;
				this.DisposeDragProcess();
			}
		}

		private bool IsValidPositionForDrop(BaseMacro dragged, BaseMacro current)
		{
			if (dragged == null || current == null)
			{
				return false;
			}
			int num = this._guiMacroSequence.IndexOf(current);
			int num2 = this._guiMacroSequence.IndexOf(dragged);
			if (current is MacroCrutchHoldUntillRelease)
			{
				return false;
			}
			MacroSequence macroSequence = new MacroSequence(this._guiMacroSequence.ControllerButton, this._guiMacroSequence.MacroSequenceType, 0);
			macroSequence.AddRange(this._guiMacroSequence);
			macroSequence.Move(num2, num);
			return macroSequence.IsMacroSequenceValid(false, false);
		}

		private void MacroItemDragLeave(object sender, DragEventArgs e)
		{
			this._guiMacroSequence.ForEach(delegate(BaseMacro m)
			{
				m.IsBeingDraggedOverFromRight = false;
			});
			this._guiMacroSequence.ForEach(delegate(BaseMacro m)
			{
				m.IsBeingDraggedOverFromLeft = false;
			});
		}

		private void MacroItemDragGiveFeedback(object sender, GiveFeedbackEventArgs e)
		{
			if (this._dragdropWindow == null)
			{
				return;
			}
			e.UseDefaultCursors = false;
			e.Handled = true;
			double dpicoefficient = DSUtils.GetDPICoefficient();
			MacroSettings.Win32Point win32Point = default(MacroSettings.Win32Point);
			MacroSettings.GetCursorPos(ref win32Point);
			this._dragdropWindow.Left = (double)win32Point.X / dpicoefficient + 2.0;
			this._dragdropWindow.Top = (double)win32Point.Y / dpicoefficient + 2.0;
		}

		private void CreateDragDropWindow(Visual dragElement)
		{
			this.DisposeDragWindow();
			this._dragdropWindow = new Window();
			this._dragdropWindow.WindowStyle = WindowStyle.None;
			this._dragdropWindow.AllowsTransparency = true;
			this._dragdropWindow.AllowDrop = false;
			this._dragdropWindow.Background = null;
			this._dragdropWindow.IsHitTestVisible = false;
			this._dragdropWindow.SizeToContent = SizeToContent.WidthAndHeight;
			this._dragdropWindow.Topmost = true;
			this._dragdropWindow.ShowInTaskbar = false;
			this._dragdropWindow.Opacity = 0.7;
			double dpicoefficient = DSUtils.GetDPICoefficient();
			Rectangle rectangle = new Rectangle();
			rectangle.Width = ((FrameworkElement)dragElement).ActualWidth;
			rectangle.Height = ((FrameworkElement)dragElement).ActualHeight;
			rectangle.Fill = new VisualBrush(dragElement);
			this._dragdropWindow.Content = rectangle;
			MacroSettings.Win32Point win32Point = default(MacroSettings.Win32Point);
			MacroSettings.GetCursorPos(ref win32Point);
			this._dragdropWindow.Left = (double)win32Point.X / dpicoefficient + 2.0;
			this._dragdropWindow.Top = (double)win32Point.Y / dpicoefficient + 2.0;
			this._dragdropWindow.Show();
		}

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetCursorPos(ref MacroSettings.Win32Point pt);

		private void MacroItemMouseEnter(object sender, MouseEventArgs e)
		{
			BaseMacro baseMacro = (BaseMacro)((ListBoxItem)sender).Content;
			baseMacro.IsHighlighted = true;
			BaseMacroBinding baseMacroBinding = baseMacro as BaseMacroBinding;
			if (baseMacroBinding != null)
			{
				BaseMacroBinding twin = baseMacroBinding.Twin;
				if (twin != null)
				{
					twin.IsHighlighted = true;
				}
			}
		}

		private void MacroItemMouseLeave(object sender, MouseEventArgs e)
		{
			BaseMacro baseMacro = ((ListBoxItem)sender).Content as BaseMacro;
			if (baseMacro == null)
			{
				return;
			}
			baseMacro.IsHighlighted = false;
			BaseMacroBinding baseMacroBinding = baseMacro as BaseMacroBinding;
			if (baseMacroBinding != null && !baseMacro.IsBeingDragged)
			{
				BaseMacroBinding twin = baseMacroBinding.Twin;
				if (twin != null)
				{
					twin.IsHighlighted = false;
				}
			}
		}

		public IEnumerable<object> ISelectedItems
		{
			get
			{
				List<object> list = new List<object>();
				foreach (object obj in this.ItemsControl.SelectedItems)
				{
					ListBoxItem listBoxItem = this.ItemsControl.ItemContainerGenerator.ContainerFromItem(obj) as ListBoxItem;
					list.Add(listBoxItem);
				}
				return list;
			}
		}

		public void SelectItem(object item, bool isSingleSelection = false)
		{
			if (item is ListBoxItem)
			{
				(item as ListBoxItem).IsSelected = true;
			}
		}

		public void DeSelectItem(object item)
		{
			if (item is ListBoxItem)
			{
				(item as ListBoxItem).IsSelected = false;
			}
		}

		public void DeSelectAll()
		{
			if (this.ItemsControl != null)
			{
				this.ItemsControl.SelectedItems.Clear();
			}
		}

		private void MacroListView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			int currentIndex = this.GetCurrentIndex((ExtendedListBox)sender, new MacroSettings.GetPositionDelegate(e.GetPosition), -10);
			if (currentIndex != -1)
			{
				for (int i = 0; i < this._guiMacroSequence.Count; i++)
				{
					this._guiMacroSequence[i].IsRightClicked = i == currentIndex;
				}
				return;
			}
			this.DeSelectAll();
		}

		private int GetCurrentIndex(ExtendedListBox listbox, MacroSettings.GetPositionDelegate getPosition, int offset)
		{
			int num = -1;
			for (int i = 0; i < listbox.Items.Count; i++)
			{
				ListBoxItem listBoxItem = listbox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
				Point point = getPosition(listBoxItem);
				Rect descendantBounds = VisualTreeHelper.GetDescendantBounds(listBoxItem);
				descendantBounds.X += (double)offset;
				if (descendantBounds.Contains(point))
				{
					num = i;
				}
			}
			return num;
		}

		private void MacroListView_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
		}

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			XBBinding xbbinding = (XBBinding)navigationContext.Parameters["XBBinding"];
			this.BindingFrameViewTypeToReturnBack = (Type)navigationContext.Parameters["BindingFrameViewTypeToReturnBack"];
			if (xbbinding == null)
			{
				MainContentVM mainContentVM = this._mainContentVM;
				XBBinding xbbinding2;
				if (mainContentVM == null)
				{
					xbbinding2 = null;
				}
				else
				{
					BaseXBBindingCollection realCurrentBeingMappedBindingCollection = mainContentVM.GameProfilesService.RealCurrentBeingMappedBindingCollection;
					xbbinding2 = ((realCurrentBeingMappedBindingCollection != null) ? realCurrentBeingMappedBindingCollection.CurrentXBBinding : null);
				}
				xbbinding = xbbinding2;
			}
			this._mainContentVM = App.MainContentVM;
			MainContentVM mainContentVM2 = this._mainContentVM;
			if (mainContentVM2 != null)
			{
				mainContentVM2.SuspendKeyPoller();
			}
			this.XBBinding = xbbinding;
			XBBinding xbbinding3 = this.XBBinding;
			this._guiMacroSequence = ((xbbinding3 != null) ? xbbinding3.CurrentActivatorXBBinding.MacroSequence : null);
			MacroSequence guiMacroSequence = this._guiMacroSequence;
			if (guiMacroSequence != null)
			{
				guiMacroSequence.AddCrutchHoldItem(false);
			}
			this.DeSelectAll();
		}

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return true;
		}

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{
			MainContentVM mainContentVM = this._mainContentVM;
			if (mainContentVM != null)
			{
				mainContentVM.StartKeyPoller();
			}
			this.StopKeyboardRecorder();
			this.XBBinding = null;
			this._macroSequence = null;
			this.DeSelectAll();
		}

		private void MacroListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ContextMenu contextMenu = new ContextMenu();
			int num = -1;
			if (e != null && e.AddedItems.Count > 0)
			{
				try
				{
					num = this._guiMacroSequence.IndexOf((BaseMacro)e.AddedItems[0]);
				}
				catch (Exception)
				{
				}
			}
			if (num != -1)
			{
				BaseMacro baseMacro = this._guiMacroSequence[num];
				if (baseMacro.MacroItemType == 8 || baseMacro.IsGamepadStickCompensation)
				{
					((FrameworkElement)sender).ContextMenu = null;
					return;
				}
				MenuItem menuItem = new MenuItem();
				menuItem.Command = baseMacro.RemoveCommand;
				menuItem.SetValue(AutomationProperties.AutomationIdProperty, "RemoveBtn");
				menuItem.Header = DTLocalization.GetString(11588);
				contextMenu.Items.Add(menuItem);
				if (baseMacro.AddPauseAfterCommand.CanExecute())
				{
					menuItem = new MenuItem();
					menuItem.Command = baseMacro.AddPauseAfterCommand;
					menuItem.SetValue(AutomationProperties.AutomationIdProperty, "AddPauseBtn");
					menuItem.Header = DTLocalization.GetString(11589);
					contextMenu.Items.Add(menuItem);
				}
				if (baseMacro.AddBreakAfterCommand.CanExecute())
				{
					menuItem = new MenuItem();
					menuItem.Command = baseMacro.AddBreakAfterCommand;
					menuItem.SetValue(AutomationProperties.AutomationIdProperty, "AddBreakBtn");
					menuItem.Header = DTLocalization.GetString(11586);
					menuItem.SetValue(ToolTipHelper.DisabledToolTipProperty, baseMacro.DisabledToolTip);
					contextMenu.Items.Add(menuItem);
				}
				if (baseMacro.AddRumbleAfterCommand.CanExecute())
				{
					menuItem = new MenuItem();
					menuItem.Command = baseMacro.AddRumbleAfterCommand;
					menuItem.SetValue(AutomationProperties.AutomationIdProperty, "AddRumbleBtn");
					menuItem.Header = DTLocalization.GetString(11587);
					contextMenu.Items.Add(menuItem);
				}
				if (baseMacro.CopyCommand.CanExecute())
				{
					menuItem = new MenuItem();
					menuItem.Command = baseMacro.CopyCommand;
					menuItem.SetValue(AutomationProperties.AutomationIdProperty, "CopyBtn");
					menuItem.Header = DTLocalization.GetString(11598);
					contextMenu.Items.Add(menuItem);
				}
				menuItem = new MenuItem();
				menuItem.Command = baseMacro.ReplaceCommand;
				menuItem.SetValue(AutomationProperties.AutomationIdProperty, "ReplaceBtn");
				menuItem.Header = DTLocalization.GetString(12188);
				contextMenu.Items.Add(menuItem);
			}
			else
			{
				MenuItem menuItem2 = new MenuItem();
				MenuItem menuItem3 = menuItem2;
				XBBinding xbbinding = this.XBBinding;
				ICommand command;
				if (xbbinding == null)
				{
					command = null;
				}
				else
				{
					ActivatorXBBinding currentActivatorXBBinding = xbbinding.CurrentActivatorXBBinding;
					if (currentActivatorXBBinding == null)
					{
						command = null;
					}
					else
					{
						MacroSequence macroSequence = currentActivatorXBBinding.MacroSequence;
						command = ((macroSequence != null) ? macroSequence.PasteCommand : null);
					}
				}
				menuItem3.Command = command;
				menuItem2.SetValue(AutomationProperties.AutomationIdProperty, "PasteBtn");
				menuItem2.Header = DTLocalization.GetString(11599);
				contextMenu.Items.Add(menuItem2);
			}
			contextMenu.SetValue(AutomationProperties.AutomationIdProperty, "RightButtonContextMenu");
			if (this.ItemsControl != null)
			{
				this.ItemsControl.ContextMenu = contextMenu;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				((SVGButton)target).Click += this.BtnStartRecord_OnClick;
				return;
			case 2:
				((SVGButton)target).Click += this.BtnStopRecord_OnClick;
				return;
			case 3:
			{
				EventSetter eventSetter = new EventSetter();
				eventSetter.Event = UIElement.PreviewMouseDownEvent;
				eventSetter.Handler = new MouseButtonEventHandler(this.MacroItemPreviewMouseDown);
				((Style)target).Setters.Add(eventSetter);
				eventSetter = new EventSetter();
				eventSetter.Event = UIElement.PreviewMouseUpEvent;
				eventSetter.Handler = new MouseButtonEventHandler(this.MacroItemPreviewMouseUp);
				((Style)target).Setters.Add(eventSetter);
				eventSetter = new EventSetter();
				eventSetter.Event = UIElement.MouseMoveEvent;
				eventSetter.Handler = new MouseEventHandler(this.MacroItemPreviewMouseMove);
				((Style)target).Setters.Add(eventSetter);
				eventSetter = new EventSetter();
				eventSetter.Event = UIElement.MouseEnterEvent;
				eventSetter.Handler = new MouseEventHandler(this.MacroItemMouseEnter);
				((Style)target).Setters.Add(eventSetter);
				eventSetter = new EventSetter();
				eventSetter.Event = UIElement.MouseLeaveEvent;
				eventSetter.Handler = new MouseEventHandler(this.MacroItemMouseLeave);
				((Style)target).Setters.Add(eventSetter);
				eventSetter = new EventSetter();
				eventSetter.Event = UIElement.PreviewDragLeaveEvent;
				eventSetter.Handler = new DragEventHandler(this.MacroItemDragLeave);
				((Style)target).Setters.Add(eventSetter);
				eventSetter = new EventSetter();
				eventSetter.Event = UIElement.GiveFeedbackEvent;
				eventSetter.Handler = new GiveFeedbackEventHandler(this.MacroItemDragGiveFeedback);
				((Style)target).Setters.Add(eventSetter);
				return;
			}
			default:
				return;
			}
		}

		public static readonly DependencyProperty RecordingCounterProperty = DependencyProperty.Register("RecordingCounter", typeof(int), typeof(MacroSettings), new PropertyMetadata(0));

		public static readonly DependencyProperty IsRecordingProperty = DependencyProperty.Register("IsRecording", typeof(bool), typeof(MacroSettings), new PropertyMetadata(false));

		public static readonly DependencyProperty IsFinalizingRecordingProperty = DependencyProperty.Register("IsFinalizingRecording", typeof(bool), typeof(MacroSettings), new PropertyMetadata(false));

		public static readonly DependencyProperty XBBindingProperty = DependencyProperty.Register("XBBinding", typeof(XBBinding), typeof(MacroSettings), new PropertyMetadata(null, new PropertyChangedCallback(MacroSettings.XBBindingPropertyChangedCallback)));

		public static readonly DependencyProperty BindingFrameViewTypeToReturnBackProperty = DependencyProperty.Register("BindingFrameViewTypeToReturnBack", typeof(Type), typeof(MacroSettings), new PropertyMetadata(null));

		private MacroSequence _macroSequence;

		private MacroSequence _guiMacroSequence;

		private MainContentVM _mainContentVM;

		private ListBox ItemsControl;

		private GlobalKeyboardHook _globalKeyboardHook;

		private Key _curProcessingKey;

		private short _prevLeftStickX;

		private short _prevLeftStickY;

		private short _prevRightStickX;

		private short _prevRightStickY;

		private bool _leftTriggerPressed;

		private bool _leftTriggerAdded;

		private ushort _prevLeftTrigger;

		private bool _rightTriggerPressed;

		private bool _rightTriggerAdded;

		private ushort _prevRightTrigger;

		private Dictionary<GamepadButton, MacroSettings.ButtonState> AnalogButtons = new Dictionary<GamepadButton, MacroSettings.ButtonState>
		{
			{
				33,
				new MacroSettings.ButtonState()
			},
			{
				34,
				new MacroSettings.ButtonState()
			},
			{
				35,
				new MacroSettings.ButtonState()
			},
			{
				36,
				new MacroSettings.ButtonState()
			},
			{
				1,
				new MacroSettings.ButtonState()
			},
			{
				2,
				new MacroSettings.ButtonState()
			},
			{
				3,
				new MacroSettings.ButtonState()
			},
			{
				4,
				new MacroSettings.ButtonState()
			},
			{
				5,
				new MacroSettings.ButtonState()
			},
			{
				6,
				new MacroSettings.ButtonState()
			}
		};

		private ulong _startPauseTickTime;

		private MacroPause _currentPause;

		private Window _dragdropWindow;

		private Point? dragStartEvalutionPoint;

		private BaseMacro _draggedItem;

		private bool isMultiSelectDragItems;

		public class ButtonState
		{
			public ButtonState()
			{
				this.PrevValue = 0;
				this.IsPressed = false;
				this.IsAdded = false;
			}

			public byte PrevValue;

			public bool IsPressed;

			public bool IsAdded;
		}

		internal struct Win32Point
		{
			public int X;

			public int Y;
		}

		private delegate Point GetPositionDelegate(IInputElement element);
	}
}
