using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using Prism.Commands;
using Prism.Regions;
using reWASDUI.Infrastructure.KeyBindings.XB;
using Xceed.Wpf.Toolkit.Core.Utilities;

namespace reWASDUI.Controls.XBBindingControls.BindingFrame.BindingFrameViews
{
	public partial class BFRumble : BaseBFView, INavigationAware
	{
		public Type BindingFrameViewTypeToReturnBack
		{
			get
			{
				return (Type)base.GetValue(BFRumble.BindingFrameViewTypeToReturnBackProperty);
			}
			set
			{
				base.SetValue(BFRumble.BindingFrameViewTypeToReturnBackProperty, value);
			}
		}

		public XBBinding XBBinding
		{
			get
			{
				return (XBBinding)base.GetValue(BFRumble.XBBindingProperty);
			}
			set
			{
				base.SetValue(BFRumble.XBBindingProperty, value);
			}
		}

		public DelegateCommand RumbleBackCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._RumbleBack) == null)
				{
					delegateCommand = (this._RumbleBack = new DelegateCommand(new Action(this.RumbleBack)));
				}
				return delegateCommand;
			}
		}

		public BFRumble()
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
		}

		private void RumbleBack()
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
			this.XBBinding = navigationContext.Parameters["XBBinding"] as XBBinding;
			if (this.XBBinding == null)
			{
				this.XBBinding = App.GameProfilesService.RealCurrentBeingMappedBindingCollection.CurrentXBBinding;
			}
		}

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return true;
		}

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{
			this.BindingFrameViewTypeToReturnBack = null;
		}

		public static readonly DependencyProperty BindingFrameViewTypeToReturnBackProperty = DependencyProperty.Register("BindingFrameViewTypeToReturnBack", typeof(Type), typeof(BFRumble), new PropertyMetadata(null));

		public static readonly DependencyProperty XBBindingProperty = DependencyProperty.Register("XBBinding", typeof(XBBinding), typeof(BFRumble), new PropertyMetadata(null));

		private DelegateCommand _RumbleBack;

		private BindingFrameUC _bindingFrameUc;
	}
}
