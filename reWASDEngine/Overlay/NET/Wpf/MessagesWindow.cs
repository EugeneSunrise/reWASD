using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DTOverlay;
using reWASDEngine.Services.OverlayAPI;

namespace Overlay.NET.Wpf
{
	public class MessagesWindow : Window, IComponentConnector
	{
		public object RootWindow { get; private set; }

		public MessagesVM ViewModel
		{
			get
			{
				return this._dataContext;
			}
		}

		public MessagesWindow()
		{
			this.InitializeComponent();
			this._dataContext = new MessagesVM();
			base.DataContext = this._dataContext;
		}

		private void UpdateSize(object sender, SizeChangedEventArgs e)
		{
			OverlayUtils.SetAlign(this.overlayManager.MonitorMessages, this.Align, this.overlayManager.GetOffsetSamePosition(), this);
			OverlayManager.currentHeight = (int)this.OverlayGrid.ActualHeight;
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.SourceInitialized += this.OnSourceInitialized;
			base.OnInitialized(e);
		}

		private void OnSourceInitialized(object sender, EventArgs e)
		{
			OverlayUtils.SetExtStyle(this);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/reWASDEngine;component/overlayapi/messageswindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				((MessagesWindow)target).SizeChanged += this.UpdateSize;
				return;
			}
			if (connectionId != 2)
			{
				this._contentLoaded = true;
				return;
			}
			this.OverlayGrid = (Grid)target;
		}

		private MessagesVM _dataContext;

		public AlignType Align;

		public OverlayManager overlayManager;

		internal Grid OverlayGrid;

		private bool _contentLoaded;
	}
}
