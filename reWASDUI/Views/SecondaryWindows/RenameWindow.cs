using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.Controls;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.SecondaryWindows.Base;
using reWASDUI.Utils;

namespace reWASDUI.Views.SecondaryWindows
{
	public class RenameWindow : BaseSecondaryWindow, IComponentConnector
	{
		public RenameWindow()
		{
			this.InitializeComponent();
		}

		protected override void OkButton_Click(object sender, RoutedEventArgs e)
		{
			if (XBValidators.ValidateShiftName(this.textBox.Text.Trim()))
			{
				this.WindowResult = MessageBoxResult.OK;
				base.Close();
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
			Uri uri = new Uri("/reWASD;component/views/secondarywindows/renameshift.xaml", UriKind.Relative);
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
				this.textBox = (WaterMarkTextBox)target;
				return;
			case 2:
				((ColoredButton)target).Click += this.OkButton_Click;
				return;
			case 3:
				((ColoredButton)target).Click += this.CancelButton_Click;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		internal WaterMarkTextBox textBox;

		private bool _contentLoaded;
	}
}
