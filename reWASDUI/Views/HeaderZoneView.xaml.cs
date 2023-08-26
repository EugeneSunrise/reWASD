using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.Controls;
using DiscSoft.NET.Common.View.Controls.Buttons;
using reWASDUI.ViewModels;

namespace reWASDUI.Views
{
	public partial class HeaderZoneView : UserControl
	{
		public GamesSelectorVM GamesSelectorVM
		{
			get
			{
				return (GamesSelectorVM)this.gamesSelector.DataContext;
			}
		}

		public Visibility GamesSelectorVisibility
		{
			get
			{
				return (Visibility)base.GetValue(HeaderZoneView.GamesSelectorVisibilityProperty);
			}
			set
			{
				base.SetValue(HeaderZoneView.GamesSelectorVisibilityProperty, value);
			}
		}

		public HeaderZoneView()
		{
			this.InitializeComponent();
		}

		public static readonly DependencyProperty GamesSelectorVisibilityProperty = DependencyProperty.Register("GamesSelectorVisibility", typeof(Visibility), typeof(HeaderZoneView), new PropertyMetadata(Visibility.Visible));
	}
}
