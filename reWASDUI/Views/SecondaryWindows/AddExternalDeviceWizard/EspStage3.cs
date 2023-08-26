using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.Controls.ComboBoxes;
using reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard;

namespace reWASDUI.Views.SecondaryWindows.AddExternalDeviceWizard
{
	public class EspStage3 : UserControl, IComponentConnector
	{
		private EspStage3VM _dataContext
		{
			get
			{
				return base.DataContext as EspStage3VM;
			}
		}

		public EspStage3()
		{
			this.InitializeComponent();
			base.Loaded += delegate(object sender, RoutedEventArgs args)
			{
				this._dataContext.PropertyChanged += this.EspStage3_PropertyChanged;
			};
		}

		private void EspStage3_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsFirmwareRewriteInProgress")
			{
				EspStage3VM dataContext = this._dataContext;
				if (dataContext != null && dataContext.IsFirmwareRewriteInProgress)
				{
					if (this.FlashingAnimation.IsVisible)
					{
						this.FlashingAnimation.PlayAnimation();
					}
					if (this.FlashingEsp32S2Animation.IsVisible)
					{
						this.FlashingEsp32S2Animation.PlayAnimation();
						return;
					}
				}
				else
				{
					if (this.FlashingAnimation.IsVisible)
					{
						this.FlashingAnimation.PauseAnimation();
					}
					if (this.FlashingEsp32S2Animation.IsVisible)
					{
						this.FlashingEsp32S2Animation.PauseAnimation();
					}
				}
			}
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
			Uri uri = new Uri("/reWASD;component/views/secondarywindows/addexternaldevicewizard/addesp32device/espstage3.xaml", UriKind.Relative);
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
				this.HeaderTB = (TextBlock)target;
				return;
			case 2:
				this.MessageTB = (TextBlock)target;
				return;
			case 3:
				this.cbESPType = (ColoredComboBox)target;
				return;
			case 4:
				this.tipTB = (TextBlock)target;
				return;
			case 5:
				this.FlashingAnimation = (EspLoadingAnimation)target;
				return;
			case 6:
				this.FlashingEsp32S2Animation = (Esp32S2LoadingAnimation)target;
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

		internal TextBlock HeaderTB;

		internal TextBlock MessageTB;

		internal ColoredComboBox cbESPType;

		internal TextBlock tipTB;

		internal EspLoadingAnimation FlashingAnimation;

		internal Esp32S2LoadingAnimation FlashingEsp32S2Animation;

		internal ColoredButton BackButton;

		internal ColoredButton NextButton;

		internal ColoredButton CancelButton;

		private bool _contentLoaded;
	}
}
