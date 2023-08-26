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
	public partial class EditConfigName : BaseSecondaryWindow
	{
		public EditConfigName(object dataContext)
		{
			base.DataContext = dataContext;
			this.InitializeComponent();
		}

		protected override void OkButton_Click(object sender, RoutedEventArgs e)
		{
			if (XBValidators.ValidateConfigName(this.textBox.Text.Trim()))
			{
				this.WindowResult = MessageBoxResult.OK;
				base.Close();
			}
		}
	}
}
