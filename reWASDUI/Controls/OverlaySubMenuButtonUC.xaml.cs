using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using Prism.Commands;
using Prism.Regions;
using reWASDUI.Controls.XBBindingControls.BindingFrame;
using reWASDUI.Controls.XBBindingControls.BindingFrame.BindingFrameViews;
using XBEliteWPF.Views.OverlayMenu;
using Xceed.Wpf.Toolkit.Core.Utilities;

namespace reWASDUI.Controls
{
	public partial class OverlaySubMenuButtonUC : UserControl
	{
		public SectorItem SectorItem
		{
			get
			{
				return (SectorItem)base.GetValue(OverlaySubMenuButtonUC.SectorItemProperty);
			}
			set
			{
				base.SetValue(OverlaySubMenuButtonUC.SectorItemProperty, value);
			}
		}

		public Type BindingFrameViewTypeToReturnBack
		{
			get
			{
				return (Type)base.GetValue(OverlaySubMenuButtonUC.BindingFrameViewTypeToReturnBackProperty);
			}
			set
			{
				base.SetValue(OverlaySubMenuButtonUC.BindingFrameViewTypeToReturnBackProperty, value);
			}
		}

		public DelegateCommand ShowShiftSettingsCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._showShiftSettings) == null)
				{
					delegateCommand = (this._showShiftSettings = new DelegateCommand(new Action(this.ShowShiftSettings)));
				}
				return delegateCommand;
			}
		}

		public OverlaySubMenuButtonUC()
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
					BindingOperations.SetBinding(this, OverlaySubMenuButtonUC.SectorItemProperty, new Binding("GameProfilesService.CurrentGame.CurrentConfig.ConfigData.OverlayMenu.Circle.MainOrSubmenu.CurrentSector"));
				}
			}
		}

		private void ShowShiftSettings()
		{
			NavigationParameters navigationParameters = new NavigationParameters();
			navigationParameters.Add("BindingFrameViewTypeToReturnBack", this.BindingFrameViewTypeToReturnBack);
			this._bindingFrameUc.RegionManager.RequestNavigate(typeof(BFOverlaySubMenu).ToString(), navigationParameters);
		}

		public static readonly DependencyProperty SectorItemProperty = DependencyProperty.Register("SectorItem", typeof(SectorItem), typeof(OverlaySubMenuButtonUC), new PropertyMetadata(null));

		public static readonly DependencyProperty BindingFrameViewTypeToReturnBackProperty = DependencyProperty.Register("BindingFrameViewTypeToReturnBack", typeof(Type), typeof(OverlaySubMenuButtonUC), new PropertyMetadata(null));

		private DelegateCommand _showShiftSettings;

		private BindingFrameUC _bindingFrameUc;

		private bool _isFirstLoadInited;
	}
}
