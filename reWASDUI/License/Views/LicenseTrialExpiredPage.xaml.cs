using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Navigation;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View.Controls.Buttons;
using XBEliteWPF.Utils;

namespace reWASDUI.License.Views
{
	public partial class LicenseTrialExpiredPage : UserControl
	{
		public LicenseTrialExpiredPage()
		{
			this.InitializeComponent();
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			DSUtils.GoUrl(e.Uri);
			SenderGoogleAnalytics.SendMessageEvent("License", "Buy", "offer", -1L, false);
			e.Handled = true;
		}
	}
}
