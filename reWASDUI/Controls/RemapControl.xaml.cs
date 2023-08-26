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
using reWASDUI.Infrastructure.KeyBindings.XB;
using Xceed.Wpf.Toolkit.Core.Utilities;

namespace reWASDUI.Controls
{
	public partial class RemapControl : UserControl
	{
		public Type BindingFrameViewTypeToReturnBack
		{
			get
			{
				return (Type)base.GetValue(RemapControl.BindingFrameViewTypeToReturnBackProperty);
			}
			set
			{
				base.SetValue(RemapControl.BindingFrameViewTypeToReturnBackProperty, value);
			}
		}

		public XBBinding XBBinding
		{
			get
			{
				return (XBBinding)base.GetValue(RemapControl.XBBindingProperty);
			}
			set
			{
				base.SetValue(RemapControl.XBBindingProperty, value);
			}
		}

		public DelegateCommand ShowRemapCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._showRemap) == null)
				{
					delegateCommand = (this._showRemap = new DelegateCommand(new Action(this.ShowRemap)));
				}
				return delegateCommand;
			}
		}

		public RemapControl()
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
				if (this.XBBinding == null)
				{
					BindingOperations.SetBinding(this, RemapControl.XBBindingProperty, new Binding("GameProfilesService.RealCurrentBeingMappedBindingCollection.CurrentXBBinding"));
				}
			}
		}

		private void ShowRemap()
		{
			NavigationParameters navigationParameters = new NavigationParameters();
			navigationParameters.Add("BindingFrameViewTypeToReturnBack", this.BindingFrameViewTypeToReturnBack);
			navigationParameters.Add("XBBinding", this.XBBinding);
			this._bindingFrameUc.RegionManager.RequestNavigate(typeof(BFGamepadMapping).ToString(), navigationParameters);
		}

		public static readonly DependencyProperty BindingFrameViewTypeToReturnBackProperty = DependencyProperty.Register("BindingFrameViewTypeToReturnBack", typeof(Type), typeof(RemapControl), new PropertyMetadata(null));

		public static readonly DependencyProperty XBBindingProperty = DependencyProperty.Register("XBBinding", typeof(XBBinding), typeof(RemapControl), new PropertyMetadata(null));

		private DelegateCommand _showRemap;

		private BindingFrameUC _bindingFrameUc;

		private bool _isFirstLoadInited;
	}
}
