using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View;
using DiscSoft.NET.Common.View.Controls.ComboBoxes;
using Prism.Events;
using Prism.Regions;
using reWASDUI.Controls.XBBindingControls.BindingFrame.BindingFrameViews;
using reWASDUI.Controls.XBBindingControls.ButtonBinding;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.KeyBindings.ActivatorXB;
using reWASDUI.Infrastructure.KeyBindings.XB;

namespace reWASDUI.Controls.XBBindingControls.BindingFrame
{
	public partial class BindingFrameUC : BaseServicesResolvingControl
	{
		public SingleRegionManager RegionManager
		{
			get
			{
				return (SingleRegionManager)base.GetValue(BindingFrameUC.RegionManagerProperty);
			}
			set
			{
				base.SetValue(BindingFrameUC.RegionManagerProperty, value);
			}
		}

		public Type DefaultViewType
		{
			get
			{
				return (Type)base.GetValue(BindingFrameUC.DefaultViewTypeProperty);
			}
			set
			{
				base.SetValue(BindingFrameUC.DefaultViewTypeProperty, value);
			}
		}

		private static void BindingRegioneDefaultViewChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			BindingFrameUC bindingFrameUC = d as BindingFrameUC;
			if (bindingFrameUC == null)
			{
				return;
			}
			bindingFrameUC.OnDefaultViewChanged(e);
		}

		private void OnDefaultViewChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this.RegionManager != null && this.DefaultViewType != null)
			{
				this.RegionManager.RegisterDefaultView(this.DefaultViewType, true);
			}
		}

		public XBBinding XBBinding
		{
			get
			{
				return (XBBinding)base.GetValue(BindingFrameUC.XBBindingProperty);
			}
			set
			{
				base.SetValue(BindingFrameUC.XBBindingProperty, value);
			}
		}

		private static void XBBindingChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			BindingFrameUC bindingFrameUC = d as BindingFrameUC;
			if (bindingFrameUC == null)
			{
				return;
			}
			bindingFrameUC.OnXBBindingChanged();
		}

		private void OnXBBindingChanged()
		{
			ColoredComboBox coloredComboBox = VisualTreeHelperExt.FindChild<ColoredComboBox>(this, "cmbKeyCode");
			if (coloredComboBox != null)
			{
				coloredComboBox.IsDropDownOpen = false;
			}
		}

		public bool IsFixedWidth
		{
			get
			{
				return (bool)base.GetValue(BindingFrameUC.IsFixedWidthProperty);
			}
			set
			{
				base.SetValue(BindingFrameUC.IsFixedWidthProperty, value);
			}
		}

		public bool HookKeyboardEventsOnlyWhenFocused
		{
			get
			{
				return (bool)base.GetValue(BindingFrameUC.HookKeyboardEventsOnlyWhenFocusedProperty);
			}
			set
			{
				base.SetValue(BindingFrameUC.HookKeyboardEventsOnlyWhenFocusedProperty, value);
			}
		}

		public ContentControl RegionContentControl { get; set; }

		public BindingFrameUC()
		{
			this.InitializeComponent();
			this.RegionManager = new SingleRegionManager();
			this.RegionManager.RegionName = RegionNames.BindingFrame + BindingFrameUC._bindingFrameRegionsCounter.ToString();
			BindingFrameUC._bindingFrameRegionsCounter += 1;
			if (this.DefaultViewType != null)
			{
				this.RegionManager.DefaultViewType = this.DefaultViewType;
			}
			App.BindingFrameRegionManagers.Add(this.RegionManager);
			base.Loaded += this.OnLoaded;
			base.Unloaded += this.OnUnloaded;
			base.IsVisibleChanged += delegate(object s, DependencyPropertyChangedEventArgs e)
			{
				this.ResolveInitRegionContentControl();
			};
			EventAggregator eventAggregator = base.EventAggregator;
			if (eventAggregator == null)
			{
				return;
			}
			eventAggregator.GetEvent<KeyboardKeyPressed>().Subscribe(new Action<Key>(this.OnKeyboardKeyPressed));
		}

		private void ResolveInitRegionContentControl()
		{
			if (this.RegionContentControl == null)
			{
				ContentControl contentControl = base.Content as ContentControl;
				if (contentControl != null && contentControl.Name == "RegionContentControl")
				{
					this.RegionContentControl = contentControl;
					if (this.RegionContentControl != null)
					{
						Prism.Regions.RegionManager.SetRegionManager(this.RegionContentControl, this.RegionManager);
						Prism.Regions.RegionManager.SetRegionName(this.RegionContentControl, this.RegionManager.RegionName);
						this.RegionManager.NavigateToDefaultView();
					}
				}
			}
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			if (!this._isSubscribedToKeyPressed)
			{
				EventAggregator eventAggregator = base.EventAggregator;
				if (eventAggregator != null)
				{
					eventAggregator.GetEvent<KeyboardKeyPressed>().Subscribe(new Action<Key>(this.OnKeyboardKeyPressed));
				}
				this._isSubscribedToKeyPressed = true;
			}
			if (this.isLoaded)
			{
				return;
			}
			this.isLoaded = true;
			if (!this._isParendEventEttached)
			{
				SVGAnchorContainer svganchorContainer = base.Parent as SVGAnchorContainer;
				if (base.Parent is ScrollViewer)
				{
					svganchorContainer = (base.Parent as ScrollViewer).Parent as SVGAnchorContainer;
				}
				if (svganchorContainer != null)
				{
					this._isParendEventEttached = true;
					svganchorContainer.OnSVGElementNameChanged += this.AnchorParentOnOnSVGElementNameChanged;
				}
				Grid grid = base.Parent as Grid;
				if (grid != null)
				{
					ListBox listBox = grid.Children[0] as ListBox;
					if (listBox != null)
					{
						this._isParendEventEttached = true;
						listBox.SelectionChanged += this.Lb_SelectionChanged;
					}
				}
			}
			this.ResolveInitRegionContentControl();
		}

		private void Lb_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this.RegionContentControl != null && e.RemovedItems.Count > 0)
			{
				SingleRegionManager regionManager = this.RegionManager;
				if (regionManager == null)
				{
					return;
				}
				regionManager.NavigateToDefaultView();
			}
		}

		private void OnKeyboardKeyPressed(Key key)
		{
			List<SingleButtonBinding> list = VisualTreeHelperExt.FindChildren<SingleButtonBinding>(this);
			if (!list.Any<SingleButtonBinding>())
			{
				return;
			}
			SingleButtonBinding singleButtonBinding2;
			if (this.HookKeyboardEventsOnlyWhenFocused)
			{
				SingleButtonBinding singleButtonBinding = list.FirstOrDefault((SingleButtonBinding uc) => uc.IsFocused || uc.IsKeyboardFocused || uc.IsKeyboardFocusWithin);
				if (singleButtonBinding == null)
				{
					return;
				}
				singleButtonBinding2 = singleButtonBinding;
			}
			else if (list.Count == 1)
			{
				singleButtonBinding2 = list.First<SingleButtonBinding>();
			}
			else
			{
				SingleButtonBinding singleButtonBinding3 = list.FirstOrDefault((SingleButtonBinding uc) => uc.IsFocused || uc.IsKeyboardFocused || uc.IsKeyboardFocusWithin);
				if (singleButtonBinding3 != null)
				{
					singleButtonBinding2 = singleButtonBinding3;
				}
				else
				{
					singleButtonBinding2 = list.First<SingleButtonBinding>();
				}
			}
			if (singleButtonBinding2.IsVisible)
			{
				XBBinding xbbinding = singleButtonBinding2.XBBinding;
				bool flag;
				if (xbbinding == null)
				{
					flag = false;
				}
				else
				{
					ActivatorXBBinding currentActivatorXBBinding = xbbinding.CurrentActivatorXBBinding;
					bool? flag2 = ((currentActivatorXBBinding != null) ? new bool?(currentActivatorXBBinding.IsMacroMapping) : null);
					bool flag3 = false;
					flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
				if (flag)
				{
					singleButtonBinding2.OnKeyboardKeyPressed(key);
				}
			}
		}

		private void AnchorParentOnOnSVGElementNameChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			if (this.RegionContentControl != null)
			{
				this.RegionManager.NavigateToDefaultView();
			}
			ColoredComboBox child = VisualTreeHelperExt.FindChild<ColoredComboBox>(this, "cmbKeyCode");
			if (child != null && !child.Focus())
			{
				base.Dispatcher.BeginInvoke(new Action(delegate
				{
					child.Focus();
				}), Array.Empty<object>());
			}
		}

		private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
		{
			if (this._isSubscribedToKeyPressed)
			{
				EventAggregator eventAggregator = base.EventAggregator;
				if (eventAggregator != null)
				{
					eventAggregator.GetEvent<KeyboardKeyPressed>().Unsubscribe(new Action<Key>(this.OnKeyboardKeyPressed));
				}
				this._isSubscribedToKeyPressed = false;
			}
		}

		public static readonly DependencyProperty RegionManagerProperty = DependencyProperty.Register("RegionManager", typeof(SingleRegionManager), typeof(BindingFrameUC), new PropertyMetadata(null));

		public static readonly DependencyProperty DefaultViewTypeProperty = DependencyProperty.Register("DefaultViewType", typeof(Type), typeof(BindingFrameUC), new PropertyMetadata(typeof(BFMain), new PropertyChangedCallback(BindingFrameUC.BindingRegioneDefaultViewChangedCallback)));

		public static readonly DependencyProperty XBBindingProperty = DependencyProperty.Register("XBBinding", typeof(XBBinding), typeof(BindingFrameUC), new PropertyMetadata(null, new PropertyChangedCallback(BindingFrameUC.XBBindingChangedCallback)));

		public static readonly DependencyProperty IsFixedWidthProperty = DependencyProperty.Register("IsFixedWidth", typeof(bool), typeof(BindingFrameUC), new PropertyMetadata(true));

		public static readonly DependencyProperty HookKeyboardEventsOnlyWhenFocusedProperty = DependencyProperty.Register("HookKeyboardEventsOnlyWhenFocused", typeof(bool), typeof(BindingFrameUC), new PropertyMetadata(false));

		private bool _isSubscribedToKeyPressed;

		private bool _isParendEventEttached;

		private bool isLoaded;

		private static byte _bindingFrameRegionsCounter;
	}
}
