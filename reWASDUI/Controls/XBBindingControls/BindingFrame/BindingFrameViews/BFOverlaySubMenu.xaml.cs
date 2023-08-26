using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Prism.Commands;
using Prism.Regions;
using XBEliteWPF.Views.OverlayMenu;
using Xceed.Wpf.Toolkit.Core.Utilities;

namespace reWASDUI.Controls.XBBindingControls.BindingFrame.BindingFrameViews
{
	public partial class BFOverlaySubMenu : BaseBFView, INavigationAware
	{
		public Type BindingFrameViewTypeToReturnBack
		{
			get
			{
				return (Type)base.GetValue(BFOverlaySubMenu.BindingFrameViewTypeToReturnBackProperty);
			}
			set
			{
				base.SetValue(BFOverlaySubMenu.BindingFrameViewTypeToReturnBackProperty, value);
			}
		}

		public SectorItem SectorItem
		{
			get
			{
				return (SectorItem)base.GetValue(BFOverlaySubMenu.SectorItemProperty);
			}
			set
			{
				base.SetValue(BFOverlaySubMenu.SectorItemProperty, value);
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

		public BFOverlaySubMenu()
		{
			this.InitializeComponent();
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
				if (this.SectorItem == null)
				{
					BindingOperations.SetBinding(this, BFOverlaySubMenu.SectorItemProperty, new Binding("GameProfilesService.CurrentGame.CurrentConfig.ConfigData.OverlayMenu.Circle.MainOrSubmenu.CurrentSector"));
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

		private void RemoveJumpToShiftCommand(object sender, RoutedEventArgs e)
		{
		}

		public static readonly DependencyProperty BindingFrameViewTypeToReturnBackProperty = DependencyProperty.Register("BindingFrameViewTypeToReturnBack", typeof(Type), typeof(BFOverlaySubMenu), new PropertyMetadata(null));

		public static readonly DependencyProperty SectorItemProperty = DependencyProperty.Register("SectorItem", typeof(SectorItem), typeof(BFOverlaySubMenu), new PropertyMetadata(null));

		private DelegateCommand _ShiftBack;

		private BindingFrameUC _bindingFrameUc;

		private bool _isFirstLoadInited;
	}
}
