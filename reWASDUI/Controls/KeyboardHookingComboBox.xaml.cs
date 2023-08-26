using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View.Controls.ComboBoxes;
using Prism.Events;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.XB;
using XBEliteWPF.Infrastructure.reWASDMapping;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;

namespace reWASDUI.Controls
{
	public partial class KeyboardHookingComboBox : BaseServicesResolvingControl
	{
		public XBBinding XBBinding
		{
			get
			{
				return (XBBinding)base.GetValue(KeyboardHookingComboBox.XBBindingProperty);
			}
			set
			{
				base.SetValue(KeyboardHookingComboBox.XBBindingProperty, value);
			}
		}

		public AssociatedControllerButton AssociatedControllerButton
		{
			get
			{
				return (AssociatedControllerButton)base.GetValue(KeyboardHookingComboBox.AssociatedControllerButtonProperty);
			}
			set
			{
				base.SetValue(KeyboardHookingComboBox.AssociatedControllerButtonProperty, value);
			}
		}

		public IEnumerable<BaseRewasdMapping> ItemsSource
		{
			get
			{
				return (IEnumerable<BaseRewasdMapping>)base.GetValue(KeyboardHookingComboBox.ItemsSourceProperty);
			}
			set
			{
				base.SetValue(KeyboardHookingComboBox.ItemsSourceProperty, value);
			}
		}

		public object SelectedItem
		{
			get
			{
				return base.GetValue(KeyboardHookingComboBox.SelectedItemProperty);
			}
			set
			{
				base.SetValue(KeyboardHookingComboBox.SelectedItemProperty, value);
			}
		}

		public Brush HighlightedScrollThumbBackground
		{
			get
			{
				return (Brush)base.GetValue(KeyboardHookingComboBox.HighlightedScrollThumbBackgroundProperty);
			}
			set
			{
				base.SetValue(KeyboardHookingComboBox.HighlightedScrollThumbBackgroundProperty, value);
			}
		}

		public Brush HighlightedBorderBrush
		{
			get
			{
				return (Brush)base.GetValue(KeyboardHookingComboBox.HighlightedBorderBrushProperty);
			}
			set
			{
				base.SetValue(KeyboardHookingComboBox.HighlightedBorderBrushProperty, value);
			}
		}

		public bool HookKeyboardEventsOnlyWhenFocused
		{
			get
			{
				return (bool)base.GetValue(KeyboardHookingComboBox.HookKeyboardEventsOnlyWhenFocusedProperty);
			}
			set
			{
				base.SetValue(KeyboardHookingComboBox.HookKeyboardEventsOnlyWhenFocusedProperty, value);
			}
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
		}

		public KeyboardHookingComboBox()
		{
			this.InitializeComponent();
			base.Loaded += this.OnLoaded;
			base.GotFocus += this.OnGotFocus;
			base.LostFocus += this.OnLostFocus;
		}

		private void OnGotFocus(object sender, RoutedEventArgs e)
		{
			EventAggregator eventAggregator = base.EventAggregator;
			if (eventAggregator == null)
			{
				return;
			}
			eventAggregator.GetEvent<KeyboardKeyPressed>().Subscribe(new Action<Key>(this.OnKeyboardKeyPressed));
		}

		private void OnLostFocus(object sender, RoutedEventArgs e)
		{
			EventAggregator eventAggregator = base.EventAggregator;
			if (eventAggregator == null)
			{
				return;
			}
			eventAggregator.GetEvent<KeyboardKeyPressed>().Unsubscribe(new Action<Key>(this.OnKeyboardKeyPressed));
		}

		private void OnKeyboardKeyPressed(Key key)
		{
			List<ColoredComboBox> list = VisualTreeHelperExt.FindChildren<ColoredComboBox>(this);
			if (!list.Any<ColoredComboBox>())
			{
				return;
			}
			ColoredComboBox desiredControl = null;
			if (this.HookKeyboardEventsOnlyWhenFocused)
			{
				ColoredComboBox coloredComboBox = list.FirstOrDefault((ColoredComboBox uc) => uc.IsFocused || uc.IsKeyboardFocused || uc.IsKeyboardFocusWithin);
				if (coloredComboBox == null)
				{
					return;
				}
				desiredControl = coloredComboBox;
			}
			else if (list.Count == 1)
			{
				desiredControl = list.First<ColoredComboBox>();
			}
			else
			{
				ColoredComboBox coloredComboBox2 = list.FirstOrDefault((ColoredComboBox uc) => uc.IsFocused || uc.IsKeyboardFocused || uc.IsKeyboardFocusWithin);
				if (coloredComboBox2 != null)
				{
					desiredControl = coloredComboBox2;
				}
				else
				{
					desiredControl = list.First<ColoredComboBox>();
				}
			}
			if (desiredControl.IsVisible)
			{
				ThreadHelper.ExecuteInMainDispatcher(delegate
				{
					KeyScanCodeV2 keyScanCodeV = KeyScanCodeV2.FindKeyScanCodeByKey(key);
					if (new ObservableCollection<BaseRewasdMapping>(this.ItemsSource).Contains(keyScanCodeV))
					{
						if (this.XBBinding != null)
						{
							this.XBBinding.KeyScanCode = keyScanCodeV;
						}
						if (this.AssociatedControllerButton != null)
						{
							this.AssociatedControllerButton.KeyScanCode = keyScanCodeV;
						}
						desiredControl.IsDropDownOpen = false;
					}
				}, true);
			}
		}

		public static readonly DependencyProperty XBBindingProperty = DependencyProperty.Register("XBBinding", typeof(XBBinding), typeof(KeyboardHookingComboBox), new PropertyMetadata(null));

		public static readonly DependencyProperty AssociatedControllerButtonProperty = DependencyProperty.Register("AssociatedControllerButton", typeof(AssociatedControllerButton), typeof(KeyboardHookingComboBox), new PropertyMetadata(null));

		public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable<BaseRewasdMapping>), typeof(KeyboardHookingComboBox), new PropertyMetadata(null));

		public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(KeyboardHookingComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public static readonly DependencyProperty HighlightedScrollThumbBackgroundProperty = DependencyProperty.Register("HighlightedScrollThumbBackground", typeof(Brush), typeof(KeyboardHookingComboBox), new PropertyMetadata(null));

		public static readonly DependencyProperty HighlightedBorderBrushProperty = DependencyProperty.Register("HighlightedBorderBrush", typeof(Brush), typeof(KeyboardHookingComboBox), new PropertyMetadata(null));

		public static readonly DependencyProperty HookKeyboardEventsOnlyWhenFocusedProperty = DependencyProperty.Register("HookKeyboardEventsOnlyWhenFocused", typeof(bool), typeof(KeyboardHookingComboBox), new PropertyMetadata(false));
	}
}
