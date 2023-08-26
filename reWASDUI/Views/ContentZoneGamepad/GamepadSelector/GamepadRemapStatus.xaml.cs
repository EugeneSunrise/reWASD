using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.Controls.ComboBoxes;
using reWASDUI.ViewModels;

namespace reWASDUI.Views.ContentZoneGamepad.GamepadSelector
{
	public partial class GamepadRemapStatus : UserControl
	{
		private GamepadSelectorVM _datacontext
		{
			get
			{
				return (GamepadSelectorVM)base.DataContext;
			}
		}

		public GamepadRemapStatus()
		{
			this.InitializeComponent();
		}
	}
}
