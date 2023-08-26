using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.Controls.ComboBoxes;

namespace reWASDUI.Views.SecondaryWindows.AddExternalDeviceWizard
{
	public class EspLatencySpeed : UserControl, IComponentConnector
	{
		public EspLatencySpeed()
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
			Uri uri = new Uri("/reWASD;component/views/secondarywindows/addexternaldevicewizard/addesp32device/esplatencyspeed.xaml", UriKind.Relative);
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
				this.LatencySpeedTB = (TextBlock)target;
				return;
			case 2:
				this.EspDescriptionTB = (TextBlock)target;
				return;
			case 3:
				this.EspDescription1TB = (TextBlock)target;
				return;
			case 4:
				this.cbLatency = (ColoredComboBox)target;
				return;
			case 5:
				this.EspPressNextTB = (TextBlock)target;
				return;
			case 6:
				this.EspDescription2TB = (TextBlock)target;
				return;
			case 7:
				this.SuccessAnimation = (EspSuccessAnimation)target;
				return;
			case 8:
				this.SkipButton = (ColoredButton)target;
				return;
			case 9:
				this.NextButton = (ColoredButton)target;
				return;
			case 10:
				this.CancelButton = (ColoredButton)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		internal TextBlock LatencySpeedTB;

		internal TextBlock EspDescriptionTB;

		internal TextBlock EspDescription1TB;

		internal ColoredComboBox cbLatency;

		internal TextBlock EspPressNextTB;

		internal TextBlock EspDescription2TB;

		internal EspSuccessAnimation SuccessAnimation;

		internal ColoredButton SkipButton;

		internal ColoredButton NextButton;

		internal ColoredButton CancelButton;

		private bool _contentLoaded;
	}
}
