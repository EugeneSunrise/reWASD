using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.Controls.Buttons;
using reWASDUI.License;

namespace reWASDUI.Controls
{
	public partial class LicenseInfo : UserControl
	{
		public bool IsLicenseOpen
		{
			get
			{
				return (bool)base.GetValue(LicenseInfo.IsLicenseOpenProperty);
			}
			set
			{
				base.SetValue(LicenseInfo.IsLicenseOpenProperty, value);
			}
		}

		public LicenseInfo()
		{
			this.InitializeComponent();
		}

		private void BtnShowHideLicenseInfo_OnClick(object sender, RoutedEventArgs e)
		{
			this.IsLicenseOpen = !this.IsLicenseOpen;
		}

		private void CopyHWId_OnClick(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText((base.DataContext as LicenseMainVM).HardWareID);
		}

		private void CopySerial_OnClick(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText((base.DataContext as LicenseMainVM).Serial);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				((SVGButton)target).Click += this.BtnShowHideLicenseInfo_OnClick;
				return;
			}
			if (connectionId != 2)
			{
				return;
			}
			((SVGButton)target).Click += this.CopySerial_OnClick;
		}

		public static readonly DependencyProperty IsLicenseOpenProperty = DependencyProperty.Register("IsLicenseOpen", typeof(bool), typeof(LicenseInfo), new PropertyMetadata(false));
	}
}
