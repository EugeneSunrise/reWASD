using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View.Controls.ComboBoxes;
using Prism.Events;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Services.Interfaces;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Controls
{
	public partial class ControllerComboBox : UserControl, IKeyPressedEventHandler
	{
		public BaseControllerVM ControllerToGetItemsSourceFrom
		{
			get
			{
				return (BaseControllerVM)base.GetValue(ControllerComboBox.ControllerToGetItemsSourceFromProperty);
			}
			set
			{
				base.SetValue(ControllerComboBox.ControllerToGetItemsSourceFromProperty, value);
			}
		}

		private static void ControllerToGetItemsSourceFromChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ControllerComboBox controllerComboBox = d as ControllerComboBox;
			if (controllerComboBox == null)
			{
				return;
			}
			controllerComboBox.OnControllerToGetItemsSourceFromChanged(e);
		}

		private void OnControllerToGetItemsSourceFromChanged(DependencyPropertyChangedEventArgs e)
		{
			this.ReEvaluateItemsSourceFromController();
		}

		public AssociatedControllerButton AssociatedControllerButton
		{
			get
			{
				return (AssociatedControllerButton)base.GetValue(ControllerComboBox.AssociatedControllerButtonProperty);
			}
			set
			{
				base.SetValue(ControllerComboBox.AssociatedControllerButtonProperty, value);
			}
		}

		private static void AssociatedControllerButtonChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ControllerComboBox controllerComboBox = d as ControllerComboBox;
			if (controllerComboBox == null)
			{
				return;
			}
			controllerComboBox.OnAssociatedControllerButtonChanged(e);
		}

		private void OnAssociatedControllerButtonChanged(DependencyPropertyChangedEventArgs e)
		{
			this.ReEvaluateItemsSourceFromController();
		}

		public bool IsGetItemsSourceFromController
		{
			get
			{
				return (bool)base.GetValue(ControllerComboBox.IsGetItemsSourceFromControllerProperty);
			}
			set
			{
				base.SetValue(ControllerComboBox.IsGetItemsSourceFromControllerProperty, value);
			}
		}

		private static void IsGetItemsSourceFromControllerChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ControllerComboBox controllerComboBox = d as ControllerComboBox;
			if (controllerComboBox == null)
			{
				return;
			}
			controllerComboBox.OnIsGetItemsSourceFromControllerChanged(e);
		}

		private void OnIsGetItemsSourceFromControllerChanged(DependencyPropertyChangedEventArgs e)
		{
			this.ReEvaluateItemsSourceFromController();
		}

		public bool IsGamepadMaskMode
		{
			get
			{
				return (bool)base.GetValue(ControllerComboBox.IsGamepadMaskModeProperty);
			}
			set
			{
				base.SetValue(ControllerComboBox.IsGamepadMaskModeProperty, value);
			}
		}

		private static void IsGamepadModeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ControllerComboBox controllerComboBox = d as ControllerComboBox;
			if (controllerComboBox == null)
			{
				return;
			}
			controllerComboBox.OnIsGamepadModeChanged(e);
		}

		private void OnIsGamepadModeChanged(DependencyPropertyChangedEventArgs e)
		{
			this.ReEvaluateItemsSourceFromController();
		}

		public bool IsGamepadRemapMode
		{
			get
			{
				return (bool)base.GetValue(ControllerComboBox.IsGamepadRemapModeProperty);
			}
			set
			{
				base.SetValue(ControllerComboBox.IsGamepadRemapModeProperty, value);
			}
		}

		private static void IsGamepadRemapModeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ControllerComboBox controllerComboBox = d as ControllerComboBox;
			if (controllerComboBox == null)
			{
				return;
			}
			controllerComboBox.OnIsGamepadRemapModeChanged(e);
		}

		private void OnIsGamepadRemapModeChanged(DependencyPropertyChangedEventArgs e)
		{
			this.ReEvaluateItemsSourceFromController();
		}

		public bool IsKeyScanCodeMode
		{
			get
			{
				return (bool)base.GetValue(ControllerComboBox.IsKeyScanCodeModeProperty);
			}
			set
			{
				base.SetValue(ControllerComboBox.IsKeyScanCodeModeProperty, value);
			}
		}

		private static void IsKeyScanCodeModeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ControllerComboBox controllerComboBox = d as ControllerComboBox;
			if (controllerComboBox == null)
			{
				return;
			}
			controllerComboBox.OnIsKeyScanCodeModeChanged(e);
		}

		private void OnIsKeyScanCodeModeChanged(DependencyPropertyChangedEventArgs e)
		{
			this.ReEvaluateItemsSourceFromController();
		}

		public bool IsKeyScanCodeMaskMode
		{
			get
			{
				return (bool)base.GetValue(ControllerComboBox.IsKeyScanCodeMaskModeProperty);
			}
			set
			{
				base.SetValue(ControllerComboBox.IsKeyScanCodeMaskModeProperty, value);
			}
		}

		private static void IsKeyScanCodeMaskModeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ControllerComboBox controllerComboBox = d as ControllerComboBox;
			if (controllerComboBox == null)
			{
				return;
			}
			controllerComboBox.OnIsKeyScanCodeMaskModeChanged(e);
		}

		private void OnIsKeyScanCodeMaskModeChanged(DependencyPropertyChangedEventArgs e)
		{
			this.ReEvaluateItemsSourceFromController();
		}

		public ControllerTypeEnum? ControllerType
		{
			get
			{
				return (ControllerTypeEnum?)base.GetValue(ControllerComboBox.ControllerTypeProperty);
			}
			set
			{
				base.SetValue(ControllerComboBox.ControllerTypeProperty, value);
			}
		}

		public IEnumerable ItemsSource
		{
			get
			{
				return (IEnumerable)base.GetValue(ControllerComboBox.ItemsSourceProperty);
			}
			set
			{
				base.SetValue(ControllerComboBox.ItemsSourceProperty, value);
			}
		}

		public IEnumerable FallbackItemsSource
		{
			get
			{
				return (IEnumerable)base.GetValue(ControllerComboBox.FallbackItemsSourceProperty);
			}
			set
			{
				base.SetValue(ControllerComboBox.FallbackItemsSourceProperty, value);
			}
		}

		public object SelectedItem
		{
			get
			{
				return base.GetValue(ControllerComboBox.SelectedItemProperty);
			}
			set
			{
				base.SetValue(ControllerComboBox.SelectedItemProperty, value);
			}
		}

		public Brush HighlightedScrollThumbBackground
		{
			get
			{
				return (Brush)base.GetValue(ControllerComboBox.HighlightedScrollThumbBackgroundProperty);
			}
			set
			{
				base.SetValue(ControllerComboBox.HighlightedScrollThumbBackgroundProperty, value);
			}
		}

		public Brush HighlightedBorderBrush
		{
			get
			{
				return (Brush)base.GetValue(ControllerComboBox.HighlightedBorderBrushProperty);
			}
			set
			{
				base.SetValue(ControllerComboBox.HighlightedBorderBrushProperty, value);
			}
		}

		public ControllerComboBox()
		{
			base.Loaded += this.OnLoaded;
			this.InitializeComponent();
			base.GotFocus += this.OnGotFocus;
			base.LostFocus += this.OnLostFocus;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
		}

		private void OnGotFocus(object sender, RoutedEventArgs e)
		{
			IEventAggregator eventAggregator = App.EventAggregator;
			if (eventAggregator != null)
			{
				eventAggregator.GetEvent<KeyboardKeyPressed>().Subscribe(new Action<Key>(this.OnKeyboardKeyPressed));
			}
			App.KeyPressedPollerService.Subscribe(this, false);
		}

		private void OnLostFocus(object sender, RoutedEventArgs e)
		{
			IEventAggregator eventAggregator = App.EventAggregator;
			if (eventAggregator != null)
			{
				eventAggregator.GetEvent<KeyboardKeyPressed>().Unsubscribe(new Action<Key>(this.OnKeyboardKeyPressed));
			}
			App.KeyPressedPollerService.Unsubscribe(this);
		}

		public void OnControllerPollState(GamepadState gamepadState)
		{
		}

		public void OnControllerKeyDown(List<GamepadButtonDescription> buttons)
		{
			GamepadButton gamepadButton = buttons[0].Button;
			ThreadHelper.ExecuteInMainDispatcher(delegate
			{
				ColoredComboBox comboboxIfVisible = this.GetComboboxIfVisible();
				if (comboboxIfVisible != null)
				{
					foreach (object obj in this.ItemsSource)
					{
						if (obj is GamepadButton)
						{
							GamepadButton gamepadButton2 = (GamepadButton)obj;
							if (gamepadButton2 == gamepadButton)
							{
								comboboxIfVisible.SelectedItem = gamepadButton2;
								comboboxIfVisible.IsDropDownOpen = false;
							}
						}
					}
				}
			}, true);
		}

		public void OnControllerKeyUp(List<GamepadButtonDescription> buttons)
		{
		}

		private ColoredComboBox GetComboboxIfVisible()
		{
			List<ColoredComboBox> list = VisualTreeHelperExt.FindChildren<ColoredComboBox>(this);
			if (!list.Any<ColoredComboBox>())
			{
				return null;
			}
			ColoredComboBox coloredComboBox = list.FirstOrDefault((ColoredComboBox uc) => uc.IsFocused || uc.IsKeyboardFocused || uc.IsKeyboardFocusWithin);
			if (coloredComboBox == null)
			{
				return null;
			}
			ColoredComboBox coloredComboBox2 = coloredComboBox;
			if (coloredComboBox2.IsVisible)
			{
				return coloredComboBox2;
			}
			return null;
		}

		private void OnKeyboardKeyPressed(Key key)
		{
			ThreadHelper.ExecuteInMainDispatcher(delegate
			{
				ColoredComboBox comboboxIfVisible = this.GetComboboxIfVisible();
				if (comboboxIfVisible != null)
				{
					KeyScanCodeV2 keyScanCodeV = KeyScanCodeV2.FindKeyScanCodeByKey(key);
					foreach (object obj in this.ItemsSource)
					{
						KeyScanCodeV2 keyScanCodeV2 = obj as KeyScanCodeV2;
						if (keyScanCodeV2 != null && keyScanCodeV2 == keyScanCodeV)
						{
							comboboxIfVisible.SelectedItem = keyScanCodeV2;
							comboboxIfVisible.IsDropDownOpen = false;
						}
					}
				}
			}, true);
		}

		private void ReEvaluateItemsSourceFromController()
		{
			if (!this.IsGetItemsSourceFromController)
			{
				return;
			}
			if (this.ControllerToGetItemsSourceFrom != null)
			{
				ControllerVM controllerVM = this.ControllerToGetItemsSourceFrom as ControllerVM;
				if (controllerVM != null)
				{
					this.ControllerType = new ControllerTypeEnum?(controllerVM.ControllerType);
				}
				else
				{
					CompositeControllerVM compositeControllerVM = this.ControllerToGetItemsSourceFrom as CompositeControllerVM;
					if (compositeControllerVM != null && compositeControllerVM.CurrentController != null)
					{
						this.ControllerType = new ControllerTypeEnum?(compositeControllerVM.CurrentController.ControllerType);
					}
					else
					{
						this.ControllerType = new ControllerTypeEnum?(this.ControllerToGetItemsSourceFrom.FirstControllerType);
					}
				}
				if (this.IsGamepadMaskMode)
				{
					this.ItemsSource = this.ControllerToGetItemsSourceFrom.GamepadMaskButtons;
				}
				else if (this.IsGamepadRemapMode)
				{
					IKeyBindingService keyBindingService = App.KeyBindingService;
					BaseControllerVM controllerToGetItemsSourceFrom = this.ControllerToGetItemsSourceFrom;
					bool flag = false;
					AssociatedControllerButton associatedControllerButton = this.AssociatedControllerButton;
					this.ItemsSource = keyBindingService.GenerateRemapButtonDescriptionsForController(controllerToGetItemsSourceFrom, flag, (associatedControllerButton != null) ? associatedControllerButton.GamepadButtonDescription : null);
				}
				else if (this.IsKeyScanCodeMaskMode)
				{
					this.ItemsSource = this.ControllerToGetItemsSourceFrom.KeyScanCodesForMask;
				}
				else if (this.IsKeyScanCodeMode)
				{
					this.ItemsSource = this.ControllerToGetItemsSourceFrom.KeyScanCodes;
				}
				ControllerTypeEnum? controllerTypeEnum;
				if ((this.IsGamepadRemapMode || this.IsGamepadMaskMode) && (this.ControllerType != null && ControllerTypeExtensions.IsAnyKeyboardTypesOrMouse(controllerTypeEnum.GetValueOrDefault())))
				{
					this.ControllerType = new ControllerTypeEnum?(3);
					return;
				}
			}
			else if (this.FallbackItemsSource == null)
			{
				if (this.IsGamepadMaskMode)
				{
					this.ItemsSource = App.KeyBindingService.ButtonsForMask;
					this.ControllerType = new ControllerTypeEnum?(3);
					return;
				}
				if (this.IsGamepadRemapMode)
				{
					this.ItemsSource = App.KeyBindingService.ButtonsToRemap;
					this.ControllerType = new ControllerTypeEnum?(3);
					return;
				}
				if (this.IsKeyScanCodeMaskMode)
				{
					this.ItemsSource = App.KeyBindingService.KeyScanCodeCollectionForKeyboard;
					return;
				}
				if (this.IsKeyScanCodeMode)
				{
					this.ItemsSource = App.KeyBindingService.KeyScanCodeCollectionForKeyboard;
					return;
				}
			}
			else
			{
				if (this.IsGamepadMaskMode || this.IsGamepadRemapMode)
				{
					this.ControllerType = new ControllerTypeEnum?(3);
				}
				this.ItemsSource = this.FallbackItemsSource;
			}
		}

		public static readonly DependencyProperty ControllerToGetItemsSourceFromProperty = DependencyProperty.Register("ControllerToGetItemsSourceFrom", typeof(BaseControllerVM), typeof(ControllerComboBox), new PropertyMetadata(null, new PropertyChangedCallback(ControllerComboBox.ControllerToGetItemsSourceFromChangedCallback)));

		public static readonly DependencyProperty AssociatedControllerButtonProperty = DependencyProperty.Register("AssociatedControllerButton", typeof(AssociatedControllerButton), typeof(ControllerComboBox), new PropertyMetadata(null, new PropertyChangedCallback(ControllerComboBox.AssociatedControllerButtonChangedCallback)));

		public static readonly DependencyProperty IsGetItemsSourceFromControllerProperty = DependencyProperty.Register("IsGetItemsSourceFromController", typeof(bool), typeof(ControllerComboBox), new PropertyMetadata(false, new PropertyChangedCallback(ControllerComboBox.IsGetItemsSourceFromControllerChangedCallback)));

		public static readonly DependencyProperty IsGamepadMaskModeProperty = DependencyProperty.Register("IsGamepadMaskMode", typeof(bool), typeof(ControllerComboBox), new PropertyMetadata(false, new PropertyChangedCallback(ControllerComboBox.IsGamepadModeChangedCallback)));

		public static readonly DependencyProperty IsGamepadRemapModeProperty = DependencyProperty.Register("IsGamepadRemapMode", typeof(bool), typeof(ControllerComboBox), new PropertyMetadata(false, new PropertyChangedCallback(ControllerComboBox.IsGamepadRemapModeChangedCallback)));

		public static readonly DependencyProperty IsKeyScanCodeModeProperty = DependencyProperty.Register("IsKeyScanCodeMode", typeof(bool), typeof(ControllerComboBox), new PropertyMetadata(false, new PropertyChangedCallback(ControllerComboBox.IsKeyScanCodeModeChangedCallback)));

		public static readonly DependencyProperty IsKeyScanCodeMaskModeProperty = DependencyProperty.Register("IsKeyScanCodeMaskMode", typeof(bool), typeof(ControllerComboBox), new PropertyMetadata(false, new PropertyChangedCallback(ControllerComboBox.IsKeyScanCodeMaskModeChangedCallback)));

		public static readonly DependencyProperty ControllerTypeProperty = DependencyProperty.Register("ControllerType", typeof(ControllerTypeEnum?), typeof(ControllerComboBox), new PropertyMetadata(3));

		public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ControllerComboBox), new PropertyMetadata(null));

		public static readonly DependencyProperty FallbackItemsSourceProperty = DependencyProperty.Register("FallbackItemsSource", typeof(IEnumerable), typeof(ControllerComboBox), new PropertyMetadata(null));

		public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(ControllerComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public static readonly DependencyProperty HighlightedScrollThumbBackgroundProperty = DependencyProperty.Register("HighlightedScrollThumbBackground", typeof(Brush), typeof(ControllerComboBox), new PropertyMetadata(null));

		public static readonly DependencyProperty HighlightedBorderBrushProperty = DependencyProperty.Register("HighlightedBorderBrush", typeof(Brush), typeof(ControllerComboBox), new PropertyMetadata(null));
	}
}
