using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.Controls.CheckBoxes;
using DiscSoft.NET.Common.View.Controls.ComboBoxes;
using reWASDUI.ViewModels.Preferences;

namespace reWASDUI.Views.Preferences
{
	public partial class PreferencesOverlayOverlaySettings : UserControl
	{
		private PreferencesOverlayOverlaySettingsVM OverlayVM
		{
			get
			{
				return (PreferencesOverlayOverlaySettingsVM)base.DataContext;
			}
		}

		public PreferencesOverlayOverlaySettings()
		{
			this.InitializeComponent();
		}
	}
}
