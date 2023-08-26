using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.Controls.Buttons;

namespace reWASDUI.Views
{
	public partial class ButtonsZoneView : UserControl
	{
		public ButtonsZoneView()
		{
			base.DataContext = App.MainContentVM;
			this.InitializeComponent();
		}
	}
}
