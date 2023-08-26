using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.RecolorableImages;

namespace reWASDUI.Views.SecondaryWindows.AddExternalDeviceWizard
{
	public class AddNintendoSwitchConsoleStep2a : UserControl, IComponentConnector
	{
		public AddNintendoSwitchConsoleStep2a()
		{
			this.InitializeComponent();
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
			Uri uri = new Uri("/reWASD;component/views/secondarywindows/addexternaldevicewizard/addnintendoswitch/addnintendoswitchconsolestep2a.xaml", UriKind.Relative);
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
				this.Step2aTB = (TextBlock)target;
				return;
			case 2:
				this.NintendoStep2aTitleTB = (TextBlock)target;
				return;
			case 3:
				this.TB1 = (TextBlock)target;
				return;
			case 4:
				this.Img1 = (RecolorableSVG)target;
				return;
			case 5:
				this.TB2 = (TextBlock)target;
				return;
			case 6:
				this.Img2 = (RecolorableSVG)target;
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

		internal TextBlock Step2aTB;

		internal TextBlock NintendoStep2aTitleTB;

		internal TextBlock TB1;

		internal RecolorableSVG Img1;

		internal TextBlock TB2;

		internal RecolorableSVG Img2;

		internal ColoredButton BackButton;

		internal ColoredButton NextButton;

		internal ColoredButton CancelButton;

		private bool _contentLoaded;
	}
}
