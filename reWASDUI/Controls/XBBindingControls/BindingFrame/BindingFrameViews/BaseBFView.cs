using System;
using System.Windows;
using System.Windows.Controls;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View;

namespace reWASDUI.Controls.XBBindingControls.BindingFrame.BindingFrameViews
{
	public abstract class BaseBFView : UserControl
	{
		public BaseBFView()
		{
			base.Loaded += this.OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			if (this.isLoaded)
			{
				return;
			}
			this.isLoaded = true;
			SVGAnchorContainer svganchorContainer = VisualTreeHelperExt.FindParent<SVGAnchorContainer>(this, null);
			if (svganchorContainer == null)
			{
				return;
			}
			svganchorContainer.AttachToSVGElement();
		}

		private bool isLoaded;
	}
}
