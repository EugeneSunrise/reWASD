using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.Controls.Buttons;
using Prism.Commands;
using reWASDUI.License.Features;

namespace reWASDUI.Controls
{
	public class MissingFeatureControl : UserControlWithDropDown, IComponentConnector, IStyleConnector
	{
		public MissingFeatureControl()
		{
			this.InitializeComponent();
		}

		private void BtnShowFeatures_OnClick(object sender, RoutedEventArgs e)
		{
			base.SetCurrentValue(UserControlWithDropDown.IsDropDownOpenProperty, !base.IsDropDownOpen);
		}

		private async void ConfigsList_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Right)
			{
				e.Handled = true;
			}
		}

		public DelegateCommand<Feature> GoToFeatureCommand
		{
			get
			{
				DelegateCommand<Feature> delegateCommand;
				if ((delegateCommand = this._goToFeatureCommand) == null)
				{
					delegateCommand = (this._goToFeatureCommand = new DelegateCommand<Feature>(new Action<Feature>(this.GoToFeature)));
				}
				return delegateCommand;
			}
		}

		private async void GoToFeature(Feature feature)
		{
			base.SetCurrentValue(UserControlWithDropDown.IsDropDownOpenProperty, false);
			feature.GoToFeatureCommand.Execute(null);
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
			Uri uri = new Uri("/reWASD;component/controls/dropdowncontrols/missingfeaturecontrol.xaml", UriKind.Relative);
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
			this._contentLoaded = true;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				((SVGButton)target).Click += this.BtnShowFeatures_OnClick;
			}
		}

		private DelegateCommand<Feature> _goToFeatureCommand;

		private bool _contentLoaded;
	}
}
