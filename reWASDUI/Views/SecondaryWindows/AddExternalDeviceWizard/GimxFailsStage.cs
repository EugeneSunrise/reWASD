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
	public class GimxFailsStage : UserControl, IComponentConnector
	{
		public GimxFailsStage()
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
			Uri uri = new Uri("/reWASD;component/views/secondarywindows/addexternaldevicewizard/addgimx/gimxfailsstage.xaml", UriKind.Relative);
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
				this.FirmwareUpdateFailsTB = (TextBlock)target;
				return;
			case 2:
				this.GimxFailsMessageTB = (TextBlock)target;
				return;
			case 3:
				this.TryAgainButton = (ColoredButton)target;
				return;
			case 4:
				this.CancelButton = (ColoredButton)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		internal TextBlock FirmwareUpdateFailsTB;

		internal TextBlock GimxFailsMessageTB;

		internal ColoredButton TryAgainButton;

		internal ColoredButton CancelButton;

		private bool _contentLoaded;
	}
}
