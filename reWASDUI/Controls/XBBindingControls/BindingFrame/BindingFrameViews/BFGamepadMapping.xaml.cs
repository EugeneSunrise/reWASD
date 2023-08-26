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
	public partial class BFGamepadMapping : BaseBFView, INavigationAware
	{
		public Type BindingFrameViewTypeToReturnBack
		{
			get
			{
				return (Type)base.GetValue(BFGamepadMapping.BindingFrameViewTypeToReturnBackProperty);
			}
			set
			{
				base.SetValue(BFGamepadMapping.BindingFrameViewTypeToReturnBackProperty, value);
			}
		}

		public XBBinding XBBinding
		{
			get
			{
				return (XBBinding)base.GetValue(BFGamepadMapping.XBBindingProperty);
			}
			set
			{
				base.SetValue(BFGamepadMapping.XBBindingProperty, value);
			}
		}

		public DelegateCommand GamepadMappingBackCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._GamepadMappingBack) == null)
				{
					delegateCommand = (this._GamepadMappingBack = new DelegateCommand(new Action(this.GamepadMappingBack)));
				}
				return delegateCommand;
			}
		}

		public BFGamepadMapping()
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

		private void GamepadMappingBack()
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

		public static readonly DependencyProperty BindingFrameViewTypeToReturnBackProperty = DependencyProperty.Register("BindingFrameViewTypeToReturnBack", typeof(Type), typeof(BFGamepadMapping), new PropertyMetadata(null));

		public static readonly DependencyProperty XBBindingProperty = DependencyProperty.Register("XBBinding", typeof(XBBinding), typeof(BFGamepadMapping), new PropertyMetadata(null));

		private DelegateCommand _GamepadMappingBack;

		private BindingFrameUC _bindingFrameUc;
	}
}
