using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.ViewModels;
using Xceed.Wpf.Toolkit.Core.Utilities;

namespace reWASDUI.Controls.XBBindingControls.BindingFrame.BindingFrameViews
{
	public partial class BFOverlayMenu : BaseBFView, INavigationAware
	{
		public Type BindingFrameViewTypeToReturnBack
		{
			get
			{
				return (Type)base.GetValue(BFOverlayMenu.BindingFrameViewTypeToReturnBackProperty);
			}
			set
			{
				base.SetValue(BFOverlayMenu.BindingFrameViewTypeToReturnBackProperty, value);
			}
		}

		public XBBinding XBBinding
		{
			get
			{
				return (XBBinding)base.GetValue(BFOverlayMenu.XBBindingProperty);
			}
			set
			{
				base.SetValue(BFOverlayMenu.XBBindingProperty, value);
			}
		}

		private static void XBBindingChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			BFOverlayMenu bfoverlayMenu = d as BFOverlayMenu;
			if (bfoverlayMenu == null)
			{
				return;
			}
			bfoverlayMenu.OnXBBindingChanged(e);
		}

		protected virtual void OnXBBindingChanged(DependencyPropertyChangedEventArgs e)
		{
			XBBinding xbbinding = e.OldValue as XBBinding;
			if (xbbinding != null)
			{
				xbbinding.MaskConditions.CollectionChanged -= this.MaskConditions_CollectionChanged;
			}
			XBBinding xbbinding2 = e.NewValue as XBBinding;
			if (xbbinding2 != null)
			{
				xbbinding2.MaskConditions.CollectionChanged += this.MaskConditions_CollectionChanged;
			}
		}

		private void MaskConditions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (base.IsVisible)
			{
				if (e.Action == NotifyCollectionChangedAction.Reset)
				{
					XBBinding xbbinding = this.XBBinding;
					bool flag;
					if (xbbinding == null)
					{
						flag = false;
					}
					else
					{
						ObservableCollection<AssociatedControllerButton> maskConditions = xbbinding.MaskConditions;
						int? num = ((maskConditions != null) ? new int?(maskConditions.Count) : null);
						int num2 = 0;
						flag = (num.GetValueOrDefault() == num2) & (num != null);
					}
					if (flag)
					{
						goto IL_67;
					}
				}
				XBBinding xbbinding2 = this.XBBinding;
				if (xbbinding2 == null || !xbbinding2.MaskConditionsHasZones)
				{
					return;
				}
				IL_67:
				this.ShiftBack();
			}
		}

		public DelegateCommand ShiftBackCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._ShiftBack) == null)
				{
					delegateCommand = (this._ShiftBack = new DelegateCommand(new Action(this.ShiftBack)));
				}
				return delegateCommand;
			}
		}

		public BFOverlayMenu()
		{
			this.InitializeComponent();
			base.DataContext = IContainerProviderExtensions.Resolve<GamepadSelectorVM>(App.Container);
			base.Loaded += this.OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			if (this._bindingFrameUc == null)
			{
				this._bindingFrameUc = VisualTreeHelperEx.FindAncestorByType<BindingFrameUC>(this);
			}
			if (!this._isFirstLoadInited)
			{
				this._isFirstLoadInited = true;
				if (this.XBBinding == null)
				{
					BindingOperations.SetBinding(this, BFOverlayMenu.XBBindingProperty, new Binding("GameProfilesService.CurrentGame.CurrentConfig.ConfigData.OverlayMenu.Circle.MainOrSubmenu.CurrentSector.XBBinding"));
				}
			}
		}

		private void ShiftBack()
		{
			if (this.BindingFrameViewTypeToReturnBack != null)
			{
				this._bindingFrameUc.RegionManager.RequestNavigate(this.BindingFrameViewTypeToReturnBack.ToString(), null);
				return;
			}
			this._bindingFrameUc.RegionManager.NavigateToDefaultView();
		}

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			this.BindingFrameViewTypeToReturnBack = navigationContext.Parameters["BindingFrameViewTypeToReturnBack"] as Type;
		}

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return true;
		}

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{
			this.BindingFrameViewTypeToReturnBack = null;
		}

		public static readonly DependencyProperty BindingFrameViewTypeToReturnBackProperty = DependencyProperty.Register("BindingFrameViewTypeToReturnBack", typeof(Type), typeof(BFOverlayMenu), new PropertyMetadata(null));

		public static readonly DependencyProperty XBBindingProperty = DependencyProperty.Register("XBBinding", typeof(XBBinding), typeof(BFOverlayMenu), new PropertyMetadata(null, new PropertyChangedCallback(BFOverlayMenu.XBBindingChangedCallback)));

		private DelegateCommand _ShiftBack;

		private BindingFrameUC _bindingFrameUc;

		private bool _isFirstLoadInited;
	}
}
