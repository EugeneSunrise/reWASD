using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.Controls;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.Controls.CharacterRestrictingTextBoxes;
using reWASDUI.License;

namespace reWASDUI.Views
{
	public partial class LicenseMain : UserControl
	{
		public LicenseMain()
		{
			this.InitializeComponent();
			base.Loaded += this.OnLoaded;
			base.DataContext = App.LicensingService;
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			if (base.DataContext is LicenseMainVM)
			{
				(base.DataContext as LicenseMainVM).ChangeLicenseIsInProgress = false;
				if (!this._isLoaded)
				{
					this._isLoaded = true;
					(base.DataContext as LicenseMainVM).RefreshFeaturePrices();
				}
			}
		}

		private void ChangedVisiblity(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (this.SerialNumberTextBox.IsVisible)
			{
				this.SerialNumberTextBox.Focus();
			}
		}

		private void Hyperlink_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(new ProcessStartInfo("https://www.daemon-tools.cc/contacts/producttechnicalsupport")
			{
				UseShellExecute = true
			});
		}

		private void GooglePlay_OnClick(object sender, RoutedEventArgs e)
		{
			Process.Start(new ProcessStartInfo("https://play.google.com/store/apps/details?id=com.discsoft.rewasd")
			{
				UseShellExecute = true
			});
		}

		private void AppStore_OnClick(object sender, RoutedEventArgs e)
		{
			Process.Start(new ProcessStartInfo("https://apps.apple.com/app/id1586976147")
			{
				UseShellExecute = true
			});
		}

		private bool _isLoaded;
	}
}
