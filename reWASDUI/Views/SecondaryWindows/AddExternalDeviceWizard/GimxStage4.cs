using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.Controls.Buttons;

namespace reWASDUI.Views.SecondaryWindows.AddExternalDeviceWizard
{
	public class GimxStage4 : UserControl, IComponentConnector
	{
		public GimxStage4()
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
			Uri uri = new Uri("/reWASD;component/views/secondarywindows/addexternaldevicewizard/addgimx/gimxstage4.xaml", UriKind.Relative);
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
				this.YouAreAllSetUpTB = (TextBlock)target;
				return;
			case 2:
				this.CanConnectCabbleToDeviceTB = (TextBlock)target;
				return;
			case 3:
				this.FinishButton = (ColoredButton)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		internal TextBlock YouAreAllSetUpTB;

		internal TextBlock CanConnectCabbleToDeviceTB;

		internal ColoredButton FinishButton;

		private bool _contentLoaded;
	}
}
