using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.Controls;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.SecondaryWindows.Base;
using reWASDUI.ViewModels;

namespace reWASDUI.Views.SecondaryWindows
{
	public partial class ShareWindow : BaseSecondaryWindow
	{
		public ShareWindow(object dataContext)
		{
			base.DataContext = dataContext;
			this.InitializeComponent();
		}

		protected override void OkButton_Click(object sender, RoutedEventArgs e)
		{
			(base.DataContext as ShareVM).ConfigDescription = this.ShareComunnityTextBox.Text;
			this.ShareButton.Command.Execute(this);
		}
	}
}
