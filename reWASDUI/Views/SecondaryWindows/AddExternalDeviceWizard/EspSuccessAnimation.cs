using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace reWASDUI.Views.SecondaryWindows.AddExternalDeviceWizard
{
	public class EspSuccessAnimation : UserControl, IComponentConnector
	{
		public EspSuccessAnimation()
		{
			this.InitializeComponent();
			base.IsVisibleChanged += this.EspSuccessAnimation_IsVisibleChanged;
		}

		private void EspSuccessAnimation_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
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
			Uri uri = new Uri("/reWASD;component/views/secondarywindows/addexternaldevicewizard/animations/espsuccessanimation.xaml", UriKind.Relative);
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
				this.Success = (Image)target;
				return;
			case 2:
				this.mask = (Image)target;
				return;
			case 3:
				this.esp = (Image)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		internal Image Success;

		internal Image mask;

		internal Image esp;

		private bool _contentLoaded;
	}
}
