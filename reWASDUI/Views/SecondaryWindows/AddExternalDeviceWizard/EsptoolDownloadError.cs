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
	public class EsptoolDownloadError : UserControl, IComponentConnector
	{
		public EsptoolDownloadError()
		{
			this.InitializeComponent();
			base.IsVisibleChanged += this.EspErrorAnimation_IsVisibleChanged;
		}

		private void EspErrorAnimation_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue)
			{
				this.PlayAnimation();
			}
		}

		private void EspErrorAnimation_Loaded(object sender, RoutedEventArgs e)
		{
			this.PlayAnimation();
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
			Uri uri = new Uri("/reWASD;component/views/secondarywindows/addexternaldevicewizard/animations/esptooldownloaderror.xaml", UriKind.Relative);
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
				this.Error = (Image)target;
				return;
			case 2:
				this.mask = (Image)target;
				return;
			case 3:
				this.Espressif = (Image)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		internal Image Error;

		internal Image mask;

		internal Image Espressif;

		private bool _contentLoaded;
	}
}
