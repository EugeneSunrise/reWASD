using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.Controls;
using DiscSoft.NET.Common.View.Controls.Buttons;

namespace reWASDUI.Views.SecondaryWindows.AddExternalDeviceWizard
{
	public partial class AddOtherBTClient : UserControl
	{
		public AddOtherBTClient()
		{
			this.InitializeComponent();
		}

		protected void OnRefreshClick(object sender, RoutedEventArgs e)
		{
			(sender as Button).IsEnabled = false;
		}
	}
}
