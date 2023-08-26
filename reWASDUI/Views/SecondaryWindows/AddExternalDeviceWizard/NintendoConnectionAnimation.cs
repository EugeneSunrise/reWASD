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
	public class NintendoConnectionAnimation : UserControl, IComponentConnector
	{
		public NintendoConnectionAnimation()
		{
			this.InitializeComponent();
			base.IsVisibleChanged += this.EspConnectionAnimation_IsVisibleChanged;
		}

		private void EspConnectionAnimation_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue)
			{
				this.PlayAnimation();
			}
		}

		public void PlayAnimation()
		{
			(base.TryFindResource("Storyboard1") as Storyboard).Begin();
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
			Uri uri = new Uri("/reWASD;component/views/secondarywindows/addexternaldevicewizard/animations/nintendoconnectionanimation.xaml", UriKind.Relative);
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
				this.Rectangle1 = (Rectangle)target;
				return;
			case 2:
				this.Rectangle2 = (Rectangle)target;
				return;
			case 3:
				this.Rectangle3 = (Rectangle)target;
				return;
			case 4:
				this.Rectangle4 = (Rectangle)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		internal Rectangle Rectangle1;

		internal Rectangle Rectangle2;

		internal Rectangle Rectangle3;

		internal Rectangle Rectangle4;

		private bool _contentLoaded;
	}
}
