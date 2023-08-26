using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.Controls.Buttons;

namespace reWASDUI.License.Views
{
	public partial class LicensePaidUncheckedPage : UserControl
	{
		public LicensePaidUncheckedPage()
		{
			this.InitializeComponent();
			base.Loaded += delegate(object sender, RoutedEventArgs args)
			{
				this.CloseBtn.Focus();
			};
		}
	}
}
