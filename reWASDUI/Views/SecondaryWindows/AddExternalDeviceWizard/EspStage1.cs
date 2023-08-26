using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Navigation;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.RecolorableImages;

namespace reWASDUI.Views.SecondaryWindows.AddExternalDeviceWizard
{
	public class EspStage1 : UserControl, IComponentConnector
	{
		public EspStage1()
		{
			this.InitializeComponent();
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			DSUtils.GoUrl(e.Uri);
			e.Handled = true;
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
			Uri uri = new Uri("/reWASD;component/views/secondarywindows/addexternaldevicewizard/addesp32device/espstage1.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				this.WaitingForAdaptorTB = (TextBlock)target;
				return;
			case 2:
				this.WaitingConnectDescrTB = (TextBlock)target;
				return;
			case 3:
				this.Waiting = (RecolorableSVG)target;
				return;
			case 4:
				this.CheckingAnimationEsp32S2 = (Esp32S2InitializationAnimation)target;
				return;
			case 5:
				this.WaitingAnimation = (EspConnectionAnimation)target;
				return;
			case 6:
				this.CheckingAnimation = (EspInitializationAnimation)target;
				return;
			case 7:
				this.BackButton = (ColoredButton)target;
				return;
			case 8:
				this.NextButton = (ColoredButton)target;
				return;
			case 9:
				this.CancelButton = (ColoredButton)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		internal TextBlock WaitingForAdaptorTB;

		internal TextBlock WaitingConnectDescrTB;

		internal RecolorableSVG Waiting;

		internal Esp32S2InitializationAnimation CheckingAnimationEsp32S2;

		internal EspConnectionAnimation WaitingAnimation;

		internal EspInitializationAnimation CheckingAnimation;

		internal ColoredButton BackButton;

		internal ColoredButton NextButton;

		internal ColoredButton CancelButton;

		private bool _contentLoaded;
	}
}
