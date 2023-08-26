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
	public class AddPS4ConsoleErrorPage : UserControl, IComponentConnector
	{
		public AddPS4ConsoleErrorPage()
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
			Uri uri = new Uri("/reWASD;component/views/secondarywindows/addexternaldevicewizard/addps4/addps4consoleerrorpage.xaml", UriKind.Relative);
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
				this.SomethingWentWrongTB = (TextBlock)target;
				return;
			case 2:
				this.CouldNotGetMacTB = (TextBlock)target;
				return;
			case 3:
				this.BackButton = (ColoredButton)target;
				return;
			case 4:
				this.TryAgainButton = (ColoredButton)target;
				return;
			case 5:
				this.CancelButton = (ColoredButton)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		internal TextBlock SomethingWentWrongTB;

		internal TextBlock CouldNotGetMacTB;

		internal ColoredButton BackButton;

		internal ColoredButton TryAgainButton;

		internal ColoredButton CancelButton;

		private bool _contentLoaded;
	}
}
