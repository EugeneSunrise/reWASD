using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.SecondaryWindows.Base;

namespace reWASDUI.Views.SecondaryWindows
{
	public partial class PrintBindingsWindow : BaseSecondaryWindow
	{
		public PrintBindingsWindow(bool existBindings, bool existDescription)
		{
			this.InitializeComponent();
			base.DataContext = this;
			this.ExistDescription = existDescription;
			this.ExistBindings = existBindings;
			if (!this.ExistBindings && this.ExistDescription)
			{
				this.rbDescriptions.IsChecked = new bool?(true);
			}
		}

		public bool ExistDescription { get; set; }

		public bool ExistBindings { get; set; }

		public bool IsBlackPrint
		{
			get
			{
				bool? isChecked = this.rbBlackWhite.IsChecked;
				bool flag = true;
				return (isChecked.GetValueOrDefault() == flag) & (isChecked != null);
			}
		}

		public bool IsMappings
		{
			get
			{
				bool? isChecked = this.rbMappings.IsChecked;
				bool flag = true;
				return (isChecked.GetValueOrDefault() == flag) & (isChecked != null);
			}
		}
	}
}
