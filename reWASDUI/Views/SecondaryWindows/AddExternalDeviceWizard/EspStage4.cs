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
	public class EspStage4 : UserControl, IComponentConnector
	{
		public EspStage4()
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
			Uri uri = new Uri("/reWASD;component/views/secondarywindows/addexternaldevicewizard/addesp32device/espstage4.xaml", UriKind.Relative);
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
				this.YouAreAllSetUpTB = (TextBlock)target;
				return;
			case 2:
				this.DescriptionTB = (TextBlock)target;
				return;
			case 3:
				this.SuccessSVG = (RecolorableSVG)target;
				return;
			case 4:
				this.SuccessAnimation = (EspSuccessAnimation)target;
				return;
			case 5:
				this.NextButton = (ColoredButton)target;
				return;
			case 6:
				this.CloseButton = (ColoredButton)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		internal TextBlock YouAreAllSetUpTB;

		internal TextBlock DescriptionTB;

		internal RecolorableSVG SuccessSVG;

		internal EspSuccessAnimation SuccessAnimation;

		internal ColoredButton NextButton;

		internal ColoredButton CloseButton;

		private bool _contentLoaded;
	}
}
