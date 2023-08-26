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
	public class EspFailsStage : UserControl, IComponentConnector
	{
		public EspFailsStage()
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
			Uri uri = new Uri("/reWASD;component/views/secondarywindows/addexternaldevicewizard/addesp32device/espfailsstage.xaml", UriKind.Relative);
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
				this.FirmwareUpdateFailsTB = (TextBlock)target;
				return;
			case 2:
				this.EspFailsMessageTB = (TextBlock)target;
				return;
			case 3:
				this.ErrorSVG = (RecolorableSVG)target;
				return;
			case 4:
				this.ErrorAnimation = (EspErrorAnimation)target;
				return;
			case 5:
				this.TryAgainButton = (ColoredButton)target;
				return;
			case 6:
				this.CancelButton = (ColoredButton)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		internal TextBlock FirmwareUpdateFailsTB;

		internal TextBlock EspFailsMessageTB;

		internal RecolorableSVG ErrorSVG;

		internal EspErrorAnimation ErrorAnimation;

		internal ColoredButton TryAgainButton;

		internal ColoredButton CancelButton;

		private bool _contentLoaded;
	}
}
