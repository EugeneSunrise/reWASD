using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace reWASDUI.Views.SecondaryWindows.CalibrateGyroscope
{
	public partial class CalibrateGyroStart : UserControl
	{
		public CalibrateGyroStart()
		{
			this.InitializeComponent();
		}

		private void OnNextButtonClick(object sender, RoutedEventArgs e)
		{
			(sender as Button).IsEnabled = false;
		}
	}
}
