using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.Controls.CheckBoxes;
using DiscSoft.NET.Common.View.Controls.ComboBoxes;
using reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard;
using XBEliteWPF.Infrastructure.ExternalDevices;

namespace reWASDUI.Views.SecondaryWindows.AddExternalDeviceWizard
{
	public partial class AdaptersSettings : UserControl
	{
		public AdaptersSettings()
		{
			this.InitializeComponent();
		}

		private void OnDeviceSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			AdaptersSettingsVM adaptersSettingsVM = (AdaptersSettingsVM)base.DataContext;
			ComboBox comboBox = sender as ComboBox;
			ExternalDevice externalDevice = ((comboBox != null) ? comboBox.SelectedItem : null) as ExternalDevice;
			if (externalDevice != null)
			{
				this.Esp32TooltipVisibilityCheck();
				if (externalDevice.HasBluetoothTransport)
				{
					UIElement uielement = this.spAuth;
					Visibility visibility;
					if (this.cbxClients.Items.Count != 0)
					{
						if (!this.cbxClients.Items.Cast<ExternalClient>().All((ExternalClient x) => x.IsDummy))
						{
							visibility = Visibility.Visible;
							goto IL_87;
						}
					}
					visibility = Visibility.Collapsed;
					IL_87:
					uielement.Visibility = visibility;
					return;
				}
				this.spAuth.Visibility = ((externalDevice.DeviceType == 1) ? Visibility.Visible : Visibility.Collapsed);
			}
		}

		private void Esp32TooltipVisibilityCheck()
		{
			AdaptersSettingsVM adaptersSettingsVM = (AdaptersSettingsVM)base.DataContext;
			this.tbEsp32lToolTip.Visibility = ((adaptersSettingsVM.CurrentExternalDevice.DeviceType == 2 && adaptersSettingsVM.CurrentExternalClient.IsDummy) ? Visibility.Visible : Visibility.Collapsed);
		}

		public void OnClientSelectionChanged(object sender, RoutedEventArgs e)
		{
			AdaptersSettingsVM adaptersSettingsVM = (AdaptersSettingsVM)base.DataContext;
			ComboBox comboBox = sender as ComboBox;
			ExternalClient externalClient = ((comboBox != null) ? comboBox.SelectedItem : null) as ExternalClient;
			if (externalClient != null)
			{
				this.Esp32TooltipVisibilityCheck();
				if (adaptersSettingsVM != null && adaptersSettingsVM.CurrentExternalDevice.HasBluetoothTransport)
				{
					this.spAuth.Visibility = (externalClient.IsDummy ? Visibility.Collapsed : Visibility.Visible);
					if (externalClient.IsConsoleAuthRequired)
					{
						this.spAuth.chkBxAuth.IsChecked = new bool?(true);
					}
				}
				else
				{
					this.spAuth.Visibility = ((adaptersSettingsVM != null && adaptersSettingsVM.CurrentExternalDevice.DeviceType == 1) ? Visibility.Visible : Visibility.Collapsed);
				}
				if (!externalClient.IsDummy && this.spAuth.cbxAuth.Items.Count > 0)
				{
					this.spAuth.cbxAuth.SelectedItem = this.spAuth.cbxAuth.Items[0];
				}
			}
		}
	}
}
