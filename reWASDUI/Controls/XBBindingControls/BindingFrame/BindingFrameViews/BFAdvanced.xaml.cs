using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using Prism.Regions;

namespace reWASDUI.Controls.XBBindingControls.BindingFrame.BindingFrameViews
{
	public partial class BFAdvanced : BaseBFView, INavigationAware
	{
		public BFAdvanced()
		{
			this.InitializeComponent();
		}

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
		}

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return false;
		}

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{
		}
	}
}
