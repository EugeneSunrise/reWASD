using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Navigation;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.Controls.CheckBoxes;
using DiscSoft.NET.Common.View.Controls.ComboBoxes;
using reWASDUI.ViewModels.Preferences;

namespace reWASDUI.Views.Preferences
{
	public partial class PreferencesGeneral : UserControl
	{
		public PreferencesGeneral()
		{
			this.InitializeComponent();
		}

		private void btnCheckForUpdate_Click(object sender, RoutedEventArgs e)
		{
			(base.DataContext as PreferencesGeneralVM).CheckForUpdate();
		}

		private void BtnClearData_OnClick(object sender, RoutedEventArgs e)
		{
			(base.DataContext as PreferencesGeneralVM).ClearData();
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			DSUtils.GoUrl(e.Uri);
		}
	}
}
