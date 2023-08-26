using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Markup;
using DTOverlay;

namespace reWASDEngine.OverlayAPI
{
	public partial class OverlayMenuBlackViewE : Window, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
			}
		}

		public OverlayMenuBlackViewE(double i_tintBackground)
		{
			this.InitializeComponent();
			if (base.DataContext == null)
			{
				base.DataContext = new OverlayMenuBlackViewEVM();
			}
			this._viewModel = (OverlayMenuBlackViewEVM)base.DataContext;
			this._viewModel.TintBackground = i_tintBackground;
		}

		private void UpdateSize(object sender, SizeChangedEventArgs e)
		{
			Rectangle desktopWorkingArea = OverlayUtils.GetDesktopWorkingArea(Engine.UserSettingsService.OverlayMenuSelectedMonitor);
			int num = (int)typeof(SystemParameters).GetProperty("DpiX", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null);
			double num2 = 96.0 / (double)num;
			base.Width = (double)desktopWorkingArea.Width * num2;
			base.Height = (double)desktopWorkingArea.Height * num2;
			base.Left = (double)desktopWorkingArea.Left * num2;
			base.Top = (double)desktopWorkingArea.Top * num2;
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

		private OverlayMenuBlackViewEVM _viewModel;
	}
}
