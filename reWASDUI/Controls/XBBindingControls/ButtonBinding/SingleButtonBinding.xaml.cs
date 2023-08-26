using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Converters;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.Controls.CheckBoxes;
using DiscSoft.NET.Common.View.Controls.ComboBoxes;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.ActivatorXB;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups;
using XBEliteWPF.Infrastructure.reWASDMapping;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Infrastructure.reWASDMapping.RewasduserCommands;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Controls.XBBindingControls.ButtonBinding
{
	public partial class SingleButtonBinding : BaseButtonBinding
	{
		public Visibility TitleVisibility
		{
			get
			{
				return (Visibility)base.GetValue(SingleButtonBinding.TitleVisibilityProperty);
			}
			set
			{
				base.SetValue(SingleButtonBinding.TitleVisibilityProperty, value);
			}
		}

		public ObservableCollection<BaseRewasdMapping> RewasdMappings
		{
			get
			{
				return (ObservableCollection<BaseRewasdMapping>)base.GetValue(SingleButtonBinding.RewasdMappingsProperty);
			}
			set
			{
				base.SetValue(SingleButtonBinding.RewasdMappingsProperty, value);
			}
		}

		public VirtualGamepadType CurrentVirtualGamepad
		{
			get
			{
				return (VirtualGamepadType)base.GetValue(SingleButtonBinding.CurrentVirtualGamepadProperty);
			}
			set
			{
				base.SetValue(SingleButtonBinding.CurrentVirtualGamepadProperty, value);
			}
		}

		private static void CurrentVirtualGamepadChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			SingleButtonBinding singleButtonBinding = d as SingleButtonBinding;
			if (singleButtonBinding == null)
			{
				return;
			}
			singleButtonBinding.OnCurrentVirtualGamepadChanged(e);
		}

		private void OnCurrentVirtualGamepadChanged(DependencyPropertyChangedEventArgs e)
		{
			this.ReEvaluateRewasdMappings();
		}

		public bool IsShowKeyboardButtons
		{
			get
			{
				return (bool)base.GetValue(SingleButtonBinding.IsShowKeyboardButtonsProperty);
			}
			set
			{
				base.SetValue(SingleButtonBinding.IsShowKeyboardButtonsProperty, value);
			}
		}

		private static void IsShowKeyboardButtonsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			SingleButtonBinding singleButtonBinding = d as SingleButtonBinding;
			if (singleButtonBinding == null)
			{
				return;
			}
			singleButtonBinding.OnIsShowKeyboardButtonsChanged(e);
		}

		private void OnIsShowKeyboardButtonsChanged(DependencyPropertyChangedEventArgs e)
		{
			this.ReEvaluateRewasdMappings();
		}

		public bool IsShowMouseButtons
		{
			get
			{
				return (bool)base.GetValue(SingleButtonBinding.IsShowMouseButtonsProperty);
			}
			set
			{
				base.SetValue(SingleButtonBinding.IsShowMouseButtonsProperty, value);
			}
		}

		private static void IsShowMouseButtonsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			SingleButtonBinding singleButtonBinding = d as SingleButtonBinding;
			if (singleButtonBinding == null)
			{
				return;
			}
			singleButtonBinding.OnIsShowMouseButtonsChanged(e);
		}

		private void OnIsShowMouseButtonsChanged(DependencyPropertyChangedEventArgs e)
		{
			this.ReEvaluateRewasdMappings();
		}

		public bool IsShowVirtualGamepadButtons
		{
			get
			{
				return (bool)base.GetValue(SingleButtonBinding.IsShowVirtualGamepadButtonsProperty);
			}
			set
			{
				base.SetValue(SingleButtonBinding.IsShowVirtualGamepadButtonsProperty, value);
			}
		}

		private static void IsShowVirtualGamepadButtonsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			SingleButtonBinding singleButtonBinding = d as SingleButtonBinding;
			if (singleButtonBinding == null)
			{
				return;
			}
			singleButtonBinding.OnIsShowVirtualGamepadButtonsChanged(e);
		}

		private void OnIsShowVirtualGamepadButtonsChanged(DependencyPropertyChangedEventArgs e)
		{
			this.ReEvaluateRewasdMappings();
		}

		public bool IsShowOverlayButtons
		{
			get
			{
				return (bool)base.GetValue(SingleButtonBinding.IsShowOverlayButtonsProperty);
			}
			set
			{
				base.SetValue(SingleButtonBinding.IsShowOverlayButtonsProperty, value);
			}
		}

		private static void IsShowOverlayButtonsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			SingleButtonBinding singleButtonBinding = d as SingleButtonBinding;
			if (singleButtonBinding == null)
			{
				return;
			}
			singleButtonBinding.OnIsShowOverlayButtonsChanged(e);
		}

		private void OnIsShowOverlayButtonsChanged(DependencyPropertyChangedEventArgs e)
		{
			this.ReEvaluateRewasdMappings();
		}

		public bool IsShowUserCommands
		{
			get
			{
				return (bool)base.GetValue(SingleButtonBinding.IsShowUserCommandsProperty);
			}
			set
			{
				base.SetValue(SingleButtonBinding.IsShowUserCommandsProperty, value);
			}
		}

		private static void IsShowUserCommandsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			SingleButtonBinding singleButtonBinding = d as SingleButtonBinding;
			if (singleButtonBinding == null)
			{
				return;
			}
			singleButtonBinding.OnIsShowUserCommandsChanged(e);
		}

		private void OnIsShowUserCommandsChanged(DependencyPropertyChangedEventArgs e)
		{
			this.ReEvaluateRewasdMappings();
		}

		public object PropertyMonitoringCrutch
		{
			get
			{
				return base.GetValue(SingleButtonBinding.PropertyMonitoringCrutchProperty);
			}
			set
			{
				base.SetValue(SingleButtonBinding.PropertyMonitoringCrutchProperty, value);
			}
		}

		private static void PropertyMonitoringCrutchChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			SingleButtonBinding singleButtonBinding = d as SingleButtonBinding;
			if (singleButtonBinding == null)
			{
				return;
			}
			singleButtonBinding.OnPropertyMonitoringCrutchChanged(e);
		}

		private void OnPropertyMonitoringCrutchChanged(DependencyPropertyChangedEventArgs e)
		{
			this.ReEvaluateRewasdMappings();
		}

		protected override void ReEvaluateXBBinding()
		{
			XBBinding xbbinding = base.XBBinding;
			if (xbbinding == null)
			{
				return;
			}
			ActivatorXBBinding currentActivatorXBBinding = xbbinding.CurrentActivatorXBBinding;
			if (currentActivatorXBBinding == null)
			{
				return;
			}
			currentActivatorXBBinding.RefreshAnnotations();
		}

		public SingleButtonBinding()
		{
			this.InitializeComponent();
			base.Loaded += this.OnLoaded;
		}

		protected override void OnXBBindingChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnXBBindingChanged(e);
			this.GenerateDefaultCrutchBindingForRefreshingReEvaluateRewasdMappings();
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			this.SetFilterBindings();
		}

		private void SetFilterBindings()
		{
			BindingOperations.SetBinding(this, SingleButtonBinding.IsShowKeyboardButtonsProperty, new Binding("GameProfilesService.CurrentGame.CurrentConfig.IsShowKeboardMappings")
			{
				RelativeSource = RelativeSource.Self,
				Mode = BindingMode.TwoWay
			});
			BindingOperations.SetBinding(this, SingleButtonBinding.IsShowMouseButtonsProperty, new Binding("GameProfilesService.CurrentGame.CurrentConfig.IsShowMouseMappings")
			{
				RelativeSource = RelativeSource.Self,
				Mode = BindingMode.TwoWay
			});
			BindingOperations.SetBinding(this, SingleButtonBinding.IsShowVirtualGamepadButtonsProperty, new Binding("GameProfilesService.CurrentGame.CurrentConfig.IsShowVirtualGamepadMappings")
			{
				RelativeSource = RelativeSource.Self,
				Mode = BindingMode.TwoWay
			});
			BindingOperations.SetBinding(this, SingleButtonBinding.IsShowUserCommandsProperty, new Binding("GameProfilesService.CurrentGame.CurrentConfig.IsShowUserCommands")
			{
				RelativeSource = RelativeSource.Self,
				Mode = BindingMode.TwoWay
			});
		}

		private void GenerateDefaultCrutchBindingForRefreshingReEvaluateRewasdMappings()
		{
			MultiBinding multiBinding = new MultiBinding
			{
				Converter = new HashSumMultiConverter()
			};
			Binding binding = new Binding("CurrentActivatorXBBinding.ActivatorType")
			{
				Source = base.XBBinding
			};
			Binding binding2 = new Binding("HostCollection.IsShiftCollection")
			{
				Source = base.XBBinding
			};
			multiBinding.Bindings.Add(binding);
			multiBinding.Bindings.Add(binding2);
			BindingOperations.SetBinding(this, SingleButtonBinding.PropertyMonitoringCrutchProperty, multiBinding);
		}

		private void ReEvaluateRewasdMappings()
		{
			XBBinding xbbinding = base.XBBinding;
			bool flag;
			if (xbbinding == null)
			{
				flag = false;
			}
			else
			{
				BaseXBBindingCollection hostCollection = xbbinding.HostCollection;
				bool? flag2 = ((hostCollection != null) ? new bool?(hostCollection.IsOverlayCollection) : null);
				bool flag3 = true;
				flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
			}
			XBBinding xbbinding2 = base.XBBinding;
			ActivatorType? activatorType;
			if (xbbinding2 == null)
			{
				activatorType = null;
			}
			else
			{
				ActivatorXBBinding currentActivatorXBBinding = xbbinding2.CurrentActivatorXBBinding;
				activatorType = ((currentActivatorXBBinding != null) ? new ActivatorType?(currentActivatorXBBinding.ActivatorType) : null);
			}
			ActivatorType? activatorType2 = activatorType;
			XBBinding xbbinding3 = base.XBBinding;
			ActivatorType? activatorType3;
			ActivatorType activatorType4;
			bool flag5;
			if (((xbbinding3 != null) ? xbbinding3.HostMaskItem : null) == null)
			{
				XBBinding xbbinding4 = base.XBBinding;
				bool flag4;
				if (xbbinding4 == null)
				{
					flag4 = false;
				}
				else
				{
					BaseXBBindingCollection hostCollection2 = xbbinding4.HostCollection;
					bool? flag2 = ((hostCollection2 != null) ? new bool?(hostCollection2.IsShiftCollection) : null);
					bool flag3 = true;
					flag4 = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
				if (flag4)
				{
					activatorType3 = activatorType2;
					activatorType4 = 0;
					flag5 = (activatorType3.GetValueOrDefault() == activatorType4) & (activatorType3 != null);
					goto IL_F0;
				}
			}
			flag5 = false;
			IL_F0:
			bool flag6 = flag5;
			activatorType3 = activatorType2;
			activatorType4 = 5;
			bool flag7;
			if (!((activatorType3.GetValueOrDefault() == activatorType4) & (activatorType3 != null)))
			{
				XBBinding xbbinding5 = base.XBBinding;
				flag7 = xbbinding5 == null || !GamepadButtonExtensions.IsAnyStickDiagonalDirection(xbbinding5.GamepadButton);
			}
			else
			{
				flag7 = false;
			}
			bool flag8 = flag7;
			activatorType3 = activatorType2;
			activatorType4 = 5;
			bool flag9 = !((activatorType3.GetValueOrDefault() == activatorType4) & (activatorType3 != null));
			bool flag10 = this.CurrentVirtualGamepad == 4;
			bool flag11 = this.CurrentVirtualGamepad == 2;
			bool flag12;
			if (this.CurrentVirtualGamepad == 2)
			{
				activatorType3 = activatorType2;
				activatorType4 = 5;
				flag12 = !((activatorType3.GetValueOrDefault() == activatorType4) & (activatorType3 != null));
			}
			else
			{
				flag12 = false;
			}
			bool flag13 = flag12;
			bool flag14;
			if (this.CurrentVirtualGamepad == 2 || this.CurrentVirtualGamepad == 4)
			{
				activatorType3 = activatorType2;
				activatorType4 = 5;
				flag14 = !((activatorType3.GetValueOrDefault() == activatorType4) & (activatorType3 != null));
			}
			else
			{
				flag14 = false;
			}
			bool flag15 = flag14;
			XBBinding xbbinding6 = base.XBBinding;
			bool flag16 = xbbinding6 != null && xbbinding6.IsOverlaySector;
			bool flag17 = flag && this.IsShowOverlayButtons && !flag16;
			if (base.XBBinding != null && GamepadButtonExtensions.IsMouseScroll(base.XBBinding.GamepadButton))
			{
				flag8 = false;
				flag9 = false;
				flag15 = false;
			}
			if (base.XBBinding != null && GamepadButtonExtensions.IsGyroTilt(base.XBBinding.GamepadButton))
			{
				flag15 = false;
			}
			List<BaseRewasdMapping> list = new List<BaseRewasdMapping>();
			if (flag17)
			{
				list.AddRange(BaseRewasdUserCommand.REWASD_USER_COMMAND_TABLE.Where((BaseRewasdUserCommand x) => x.IsOverlayMenuCommand).ToList<BaseRewasdUserCommand>());
			}
			list.AddRange(from ksc in KeyScanCodeV2.SCAN_CODE_TABLE
				where !string.IsNullOrWhiteSpace(ksc.FriendlyName) && ksc.PCKeyCategory != 1
				orderby (ksc.IsMouse ? 1 : 0) + (ksc.IsGamepadOrGyroTilt ? 2 : 0) descending
				select ksc);
			list.AddRange(BaseRewasdUserCommand.REWASD_USER_COMMAND_TABLE.Where((BaseRewasdUserCommand x) => !x.IsAnyOverlayMenuCommand).ToList<BaseRewasdUserCommand>());
			if (this.IsShowVirtualGamepadButtons && flag15)
			{
				int num = list.FindLastIndex((BaseRewasdMapping item) => item.IsVirtualGyroTilt);
				BaseRewasdMapping baseRewasdMapping = list.Find((BaseRewasdMapping rm) => rm.IsVirtualGyroReset);
				if (num != -1 && baseRewasdMapping != null)
				{
					list.RemoveAll((BaseRewasdMapping rm) => rm.IsVirtualGyroReset);
					list.Insert(num + 1, baseRewasdMapping);
				}
			}
			KeyScanCodeV2 noMap = KeyScanCodeV2.NoMap;
			KeyScanCodeV2 noInheritance = KeyScanCodeV2.NoInheritance;
			list.Remove(noMap);
			list.Remove(noInheritance);
			if (!this.IsShowKeyboardButtons)
			{
				list.RemoveAll(delegate(BaseRewasdMapping rm)
				{
					KeyScanCodeV2 keyScanCodeV = rm as KeyScanCodeV2;
					return keyScanCodeV != null && keyScanCodeV.PCKeyCategory == 0;
				});
			}
			if (!this.IsShowMouseButtons)
			{
				list.RemoveAll(delegate(BaseRewasdMapping rm)
				{
					KeyScanCodeV2 keyScanCodeV2 = rm as KeyScanCodeV2;
					return keyScanCodeV2 != null && (keyScanCodeV2.PCKeyCategory == 2 || keyScanCodeV2.PCKeyCategory == 3);
				});
			}
			if (!flag8)
			{
				list.RemoveAll(delegate(BaseRewasdMapping rm)
				{
					KeyScanCodeV2 keyScanCodeV3 = rm as KeyScanCodeV2;
					return keyScanCodeV3 != null && keyScanCodeV3.PCKeyCategory == 2;
				});
				list.RemoveAll(delegate(BaseRewasdMapping rm)
				{
					KeyScanCodeV2 keyScanCodeV4 = rm as KeyScanCodeV2;
					return keyScanCodeV4 != null && keyScanCodeV4.PCKeyCategory == 9;
				});
			}
			if (!flag13)
			{
				list.RemoveAll(delegate(BaseRewasdMapping rm)
				{
					KeyScanCodeV2 keyScanCodeV5 = rm as KeyScanCodeV2;
					return keyScanCodeV5 != null && keyScanCodeV5.PCKeyCategory == 8;
				});
			}
			if (!flag15)
			{
				list.RemoveAll(delegate(BaseRewasdMapping rm)
				{
					KeyScanCodeV2 keyScanCodeV6 = rm as KeyScanCodeV2;
					return keyScanCodeV6 != null && keyScanCodeV6.PCKeyCategory == 9;
				});
				list.RemoveAll((BaseRewasdMapping rm) => rm.IsVirtualGyroReset);
			}
			if (!this.IsShowVirtualGamepadButtons)
			{
				list.RemoveAll(delegate(BaseRewasdMapping rm)
				{
					KeyScanCodeV2 keyScanCodeV7 = rm as KeyScanCodeV2;
					return keyScanCodeV7 != null && (keyScanCodeV7.PCKeyCategory == 4 || keyScanCodeV7.PCKeyCategory == 5 || keyScanCodeV7.PCKeyCategory == 9 || keyScanCodeV7.PCKeyCategory == 8);
				});
				list.RemoveAll((BaseRewasdMapping rm) => rm.IsVirtualGyroReset);
			}
			if (!flag10)
			{
				list.Remove(KeyScanCodeV2.GamepadButton12);
			}
			if (!flag11)
			{
				list.Remove(KeyScanCodeV2.GamepadTouchClick);
			}
			if (!flag9)
			{
				list.RemoveAll(delegate(BaseRewasdMapping rm)
				{
					KeyScanCodeV2 keyScanCodeV8 = rm as KeyScanCodeV2;
					return keyScanCodeV8 != null && keyScanCodeV8.PCKeyCategory == 5;
				});
			}
			if (!this.IsShowUserCommands)
			{
				list.RemoveAll((BaseRewasdMapping rm) => rm.IsUserCommand && !rm.IsOverlayMenuCommand && !rm.IsVirtualGyroReset);
			}
			if (base.XBBinding != null)
			{
				if (GamepadButtonExtensions.IsAnyXStickDirection(base.XBBinding.GamepadButton))
				{
					goto IL_6AC;
				}
				if (GamepadButtonExtensions.IsPhysicalTrackXDirection(base.XBBinding.GamepadButton))
				{
					BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
					bool flag18;
					if (currentGamepad == null)
					{
						flag18 = false;
					}
					else
					{
						ControllerVM currentController = currentGamepad.CurrentController;
						ControllerTypeEnum? controllerTypeEnum;
						bool? flag2 = ((currentController != null) ? ((currentController.FirstGamepadType != null) ? new bool?(ControllerTypeExtensions.IsAnySteam(controllerTypeEnum.GetValueOrDefault())) : null) : null);
						bool flag3 = true;
						flag18 = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
					}
					if (flag18)
					{
						BaseXBBindingCollection hostCollection3 = base.XBBinding.HostCollection;
						if (((hostCollection3 != null) ? hostCollection3.CurrentBoundGroup : null) != null)
						{
							BaseXBBindingCollection hostCollection4 = base.XBBinding.HostCollection;
							BaseTouchpadDirectionalGroup baseTouchpadDirectionalGroup = ((hostCollection4 != null) ? hostCollection4.CurrentBoundGroup : null) as BaseTouchpadDirectionalGroup;
							if (baseTouchpadDirectionalGroup != null && !baseTouchpadDirectionalGroup.IsDigitalMode)
							{
								goto IL_6AC;
							}
						}
					}
				}
			}
			list.RemoveAll(delegate(BaseRewasdMapping rm)
			{
				KeyScanCodeV2 keyScanCodeV9 = rm as KeyScanCodeV2;
				return keyScanCodeV9 != null && keyScanCodeV9.IsMouseFlickDirection;
			});
			IL_6AC:
			list.Insert(0, noMap);
			if (flag6)
			{
				list.Insert(1, noInheritance);
			}
			if (!this.IsSameMappings(list))
			{
				this.RewasdMappings = new ObservableCollection<BaseRewasdMapping>(list);
			}
		}

		private bool IsSameMappings(List<BaseRewasdMapping> filteredRewasdMappings)
		{
			if (this.RewasdMappings == null || filteredRewasdMappings.Count != this.RewasdMappings.Count)
			{
				return false;
			}
			int count = filteredRewasdMappings.Count;
			for (int i = 0; i < count; i++)
			{
				if (filteredRewasdMappings[i] != this.RewasdMappings[i])
				{
					return false;
				}
			}
			return true;
		}

		public void OnKeyboardKeyPressed(Key key)
		{
			if (!base.AllowKeyboardHooks)
			{
				return;
			}
			ThreadHelper.ExecuteInMainDispatcher(delegate
			{
				KeyScanCodeV2 keyScanCodeV = KeyScanCodeV2.FindKeyScanCodeByKey(key);
				XBBinding xbbinding = this.XBBinding;
				if (((xbbinding != null) ? xbbinding.CurrentActivatorXBBinding : null) != null && !string.IsNullOrWhiteSpace(keyScanCodeV.Description) && this.RewasdMappings.Contains(keyScanCodeV))
				{
					this.XBBinding.CurrentActivatorXBBinding.MappedKey = KeyScanCodeV2.FindKeyScanCodeByKey(key);
					ColoredComboBox coloredComboBox = VisualTreeHelperExt.FindChild<ColoredComboBox>(this, "cmbKeyCode");
					if (coloredComboBox != null)
					{
						coloredComboBox.IsDropDownOpen = false;
					}
				}
			}, true);
		}

		private void BtnRemoveKeyBinding(object sender, RoutedEventArgs e)
		{
			base.XBBinding.RemoveKeyBinding();
			base.XBBinding.HostCollection.SubConfigData.ConfigData.CheckFeatures();
		}

		private void ContainerBorderMouseDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		private void EnableOverlayFilterOnly(object sender, MouseButtonEventArgs e)
		{
			this.IsShowKeyboardButtons = false;
			this.IsShowMouseButtons = false;
			this.IsShowVirtualGamepadButtons = false;
			this.IsShowUserCommands = false;
			this.IsShowOverlayButtons = true;
			e.Handled = true;
		}

		private void EnableGamepadFilterOnly(object sender, MouseButtonEventArgs e)
		{
			this.IsShowKeyboardButtons = false;
			this.IsShowMouseButtons = false;
			this.IsShowVirtualGamepadButtons = true;
			this.IsShowUserCommands = false;
			this.IsShowOverlayButtons = false;
			e.Handled = true;
		}

		private void EnableMouseFilterOnly(object sender, MouseButtonEventArgs e)
		{
			this.IsShowKeyboardButtons = false;
			this.IsShowMouseButtons = true;
			this.IsShowVirtualGamepadButtons = false;
			this.IsShowUserCommands = false;
			this.IsShowOverlayButtons = false;
			e.Handled = true;
		}

		private void EnableKeyboardFilterOnly(object sender, MouseButtonEventArgs e)
		{
			this.IsShowKeyboardButtons = true;
			this.IsShowMouseButtons = false;
			this.IsShowVirtualGamepadButtons = false;
			this.IsShowUserCommands = false;
			this.IsShowOverlayButtons = false;
			e.Handled = true;
		}

		private void EnableUserCommandsFilterOnly(object sender, MouseButtonEventArgs e)
		{
			this.IsShowKeyboardButtons = false;
			this.IsShowMouseButtons = false;
			this.IsShowVirtualGamepadButtons = false;
			this.IsShowUserCommands = true;
			this.IsShowOverlayButtons = false;
			e.Handled = true;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				((Border)target).MouseDown += this.ContainerBorderMouseDown;
				return;
			case 2:
				((SVGButton)target).Click += this.BtnRemoveKeyBinding;
				return;
			case 3:
				((DrawingCheckBox)target).MouseDoubleClick += this.EnableOverlayFilterOnly;
				return;
			case 4:
				((DrawingCheckBox)target).MouseDoubleClick += this.EnableGamepadFilterOnly;
				return;
			case 5:
				((DrawingCheckBox)target).PreviewMouseDoubleClick += this.EnableGamepadFilterOnly;
				return;
			case 6:
				((DrawingCheckBox)target).PreviewMouseDoubleClick += this.EnableGamepadFilterOnly;
				return;
			case 7:
				((DrawingCheckBox)target).PreviewMouseDoubleClick += this.EnableGamepadFilterOnly;
				return;
			case 8:
				((DrawingCheckBox)target).PreviewMouseDoubleClick += this.EnableGamepadFilterOnly;
				return;
			case 9:
				((DrawingCheckBox)target).PreviewMouseDoubleClick += this.EnableMouseFilterOnly;
				return;
			case 10:
				((DrawingCheckBox)target).PreviewMouseDoubleClick += this.EnableKeyboardFilterOnly;
				return;
			case 11:
				((DrawingCheckBox)target).PreviewMouseDoubleClick += this.EnableUserCommandsFilterOnly;
				return;
			default:
				return;
			}
		}

		public static readonly DependencyProperty TitleVisibilityProperty = DependencyProperty.Register("TitleVisibility", typeof(Visibility), typeof(SingleButtonBinding), new PropertyMetadata(Visibility.Visible));

		public static readonly DependencyProperty RewasdMappingsProperty = DependencyProperty.Register("RewasdMappings", typeof(ObservableCollection<BaseRewasdMapping>), typeof(SingleButtonBinding), new PropertyMetadata(null));

		public static readonly DependencyProperty CurrentVirtualGamepadProperty = DependencyProperty.Register("CurrentVirtualGamepad", typeof(VirtualGamepadType), typeof(SingleButtonBinding), new PropertyMetadata(0, new PropertyChangedCallback(SingleButtonBinding.CurrentVirtualGamepadChangedCallback)));

		public static readonly DependencyProperty IsShowKeyboardButtonsProperty = DependencyProperty.Register("IsShowKeyboardButtons", typeof(bool), typeof(SingleButtonBinding), new PropertyMetadata(true, new PropertyChangedCallback(SingleButtonBinding.IsShowKeyboardButtonsChangedCallback)));

		public static readonly DependencyProperty IsShowMouseButtonsProperty = DependencyProperty.Register("IsShowMouseButtons", typeof(bool), typeof(SingleButtonBinding), new PropertyMetadata(true, new PropertyChangedCallback(SingleButtonBinding.IsShowMouseButtonsChangedCallback)));

		public static readonly DependencyProperty IsShowVirtualGamepadButtonsProperty = DependencyProperty.Register("IsShowVirtualGamepadButtons", typeof(bool), typeof(SingleButtonBinding), new PropertyMetadata(true, new PropertyChangedCallback(SingleButtonBinding.IsShowVirtualGamepadButtonsChangedCallback)));

		public static readonly DependencyProperty IsShowOverlayButtonsProperty = DependencyProperty.Register("IsShowOverlayButtons", typeof(bool), typeof(SingleButtonBinding), new PropertyMetadata(true, new PropertyChangedCallback(SingleButtonBinding.IsShowOverlayButtonsChangedCallback)));

		public static readonly DependencyProperty IsShowUserCommandsProperty = DependencyProperty.Register("IsShowUserCommands", typeof(bool), typeof(SingleButtonBinding), new PropertyMetadata(true, new PropertyChangedCallback(SingleButtonBinding.IsShowUserCommandsChangedCallback)));

		public static readonly DependencyProperty PropertyMonitoringCrutchProperty = DependencyProperty.Register("PropertyMonitoringCrutch", typeof(object), typeof(SingleButtonBinding), new PropertyMetadata(null, new PropertyChangedCallback(SingleButtonBinding.PropertyMonitoringCrutchChangedCallback)));
	}
}
