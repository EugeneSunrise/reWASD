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
	public class AddNintendoSwitchConsoleStep3 : UserControl, IComponentConnector
	{
		public AddNintendoSwitchConsoleStep3()
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
			Uri uri = new Uri("/reWASD;component/views/secondarywindows/addexternaldevicewizard/addnintendoswitch/addnintendoswitchconsolestep3.xaml", UriKind.Relative);
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
				this.Step3TB = (TextBlock)target;
				return;
			case 2:
				this.NintendoStep3TitleTB = (TextBlock)target;
				return;
			case 3:
				this.NintendoStep3FinderImg = (RecolorableSVG)target;
				return;
			case 4:
				this.FinishButton = (ColoredButton)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		internal TextBlock Step3TB;

		internal TextBlock NintendoStep3TitleTB;

		internal RecolorableSVG NintendoStep3FinderImg;

		internal ColoredButton FinishButton;

		private bool _contentLoaded;
	}
}
