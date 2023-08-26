using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View;

namespace reWASDUI.Views
{
	public partial class MouseMappingView : UserControl
	{
		public MouseMappingView()
		{
			this.InitializeComponent();
			base.Loaded += this.OnLoaded;
			base.IsVisibleChanged += this.MouseMappingView_IsVisibleChanged;
		}

		private async void OnLoaded(object sender, RoutedEventArgs e)
		{
			await this.InitContainerGrid();
			base.UpdateLayout();
		}

		private async Task WaitForContainer()
		{
			if (this._svgContainerGrid == null)
			{
				this._svgContainerGrid = VisualTreeHelperExt.FindChild<SVGContainerGrid>(this, null);
			}
			if (this._svgContainerGrid == null)
			{
				await Task.Delay(50);
				this._svgContainerGrid = base.Template.FindName("svgContainerGrid", this) as SVGContainerGrid;
			}
			if (this._svgContainerGrid == null)
			{
				await Task.Delay(200);
				this._svgContainerGrid = base.Template.FindName("svgContainerGrid", this) as SVGContainerGrid;
			}
		}

		private async void MouseMappingView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue)
			{
				await this.InitContainerGrid();
				this.UpdateAttachedButtonsPos();
			}
		}

		private void UpdateAttachedButtonsPos()
		{
			if (this._svgContainerGrid == null)
			{
				return;
			}
			foreach (object obj in this._svgContainerGrid.Children)
			{
				SVGElementAttachedButton svgelementAttachedButton = obj as SVGElementAttachedButton;
				if (svgelementAttachedButton != null)
				{
					svgelementAttachedButton.AttachToSVGElement();
				}
			}
		}

		private async Task InitContainerGrid()
		{
			if (!this._firstLoadInited)
			{
				this._firstLoadInited = true;
				await this.WaitForContainer();
				SVGContainerGrid svgContainerGrid = this._svgContainerGrid;
			}
		}

		private SVGContainerGrid _svgContainerGrid;

		private bool _firstLoadInited;
	}
}
