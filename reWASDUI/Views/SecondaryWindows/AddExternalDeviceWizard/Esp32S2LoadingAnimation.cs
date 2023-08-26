using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace reWASDUI.Views.SecondaryWindows.AddExternalDeviceWizard
{
	public class Esp32S2LoadingAnimation : UserControl, IComponentConnector
	{
		public Esp32S2LoadingAnimation()
		{
			this.InitializeComponent();
		}

		public void PlayAnimation()
		{
			(base.TryFindResource("Storyboard1") as Storyboard).Begin();
		}

		public void PauseAnimation()
		{
			(base.TryFindResource("Storyboard1") as Storyboard).Pause();
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
			Uri uri = new Uri("/reWASD;component/views/secondarywindows/addexternaldevicewizard/animations/esp32s2loadinganimation.xaml", UriKind.Relative);
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
				this.image = (Image)target;
				return;
			case 2:
				this.dot1 = (Ellipse)target;
				return;
			case 3:
				this.dot2 = (Ellipse)target;
				return;
			case 4:
				this.dot3 = (Ellipse)target;
				return;
			case 5:
				this.dot4 = (Ellipse)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		internal Image image;

		internal Ellipse dot1;

		internal Ellipse dot2;

		internal Ellipse dot3;

		internal Ellipse dot4;

		private bool _contentLoaded;
	}
}
