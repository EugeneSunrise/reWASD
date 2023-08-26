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
	public class GamepadWindow : Window, IComponentConnector
	{
		public object RootWindow { get; private set; }

		public GamepadWindowVM ViewModel
		{
			get
			{
				return this._dataContext;
			}
		}

		public GamepadWindow()
		{
			this.InitializeComponent();
			this._dataContext = new GamepadWindowVM();
			base.DataContext = this._dataContext;
			this._dataContext.IsGamepadVisible = false;
		}

		private void UpdateSize(object sender, SizeChangedEventArgs e)
		{
			OverlayUtils.SetAlign(this.overlayManager.MonitorGamepad, this.Align, 0f, this);
			this.overlayManager.ArrayMessageWindow();
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
			Uri uri = new Uri("/reWASDEngine;component/overlayapi/gamepadwindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				((GamepadWindow)target).SizeChanged += this.UpdateSize;
				return;
			case 2:
				this.OverlayGrid = (Grid)target;
				return;
			case 3:
				this.GamepadGrid = (Grid)target;
				return;
			case 4:
				this.GamepadDrawing = (Image)target;
				return;
			case 5:
				this.tbGyro = (TextBlock)target;
				return;
			case 6:
				this.tbYaw = (TextBlock)target;
				return;
			case 7:
				this.SliderYaw = (Slider)target;
				return;
			case 8:
				this.tbPitch = (TextBlock)target;
				return;
			case 9:
				this.SliderPitch = (Slider)target;
				return;
			case 10:
				this.tbRoll = (TextBlock)target;
				return;
			case 11:
				this.SliderRoll = (Slider)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		private GamepadWindowVM _dataContext;

		public AlignType Align;

		public OverlayManager overlayManager;

		internal Grid OverlayGrid;

		internal Grid GamepadGrid;

		internal Image GamepadDrawing;

		internal TextBlock tbGyro;

		internal TextBlock tbYaw;

		internal Slider SliderYaw;

		internal TextBlock tbPitch;

		internal Slider SliderPitch;

		internal TextBlock tbRoll;

		internal Slider SliderRoll;

		private bool _contentLoaded;
	}
}
