using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Navigation;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View.Controls.Buttons;
using Microsoft.CSharp.RuntimeBinder;
using XBEliteWPF.Utils;

namespace reWASDUI.License.Views
{
	public partial class UpdateAvailablePage : UserControl
	{
		public UpdateAvailablePage()
		{
			this.InitializeComponent();
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			DSUtils.GoUrl(e.Uri);
			SenderGoogleAnalytics.SendMessageEvent("License", "Buy", "offer", -1L, false);
			e.Handled = true;
		}

		private void webBrowser_LoadCompleted(object sender, NavigationEventArgs e)
		{
			object document = this.webBrowser.Document;
			int num = (int)typeof(SystemParameters).GetProperty("DpiX", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null);
			if (num != 96)
			{
				double num2 = (double)num;
				num2 /= 96.0;
				if (UpdateAvailablePage.<>o__2.<>p__1 == null)
				{
					UpdateAvailablePage.<>o__2.<>p__1 = CallSite<Action<CallSite, object, string>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "execScript", null, typeof(UpdateAvailablePage), new CSharpArgumentInfo[]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
					}));
				}
				Action<CallSite, object, string> target = UpdateAvailablePage.<>o__2.<>p__1.Target;
				CallSite <>p__ = UpdateAvailablePage.<>o__2.<>p__1;
				if (UpdateAvailablePage.<>o__2.<>p__0 == null)
				{
					UpdateAvailablePage.<>o__2.<>p__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "parentWindow", typeof(UpdateAvailablePage), new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				target(<>p__, UpdateAvailablePage.<>o__2.<>p__0.Target(UpdateAvailablePage.<>o__2.<>p__0, document), "document.body.style.zoom=" + num2.ToString().Replace(",", ".") + ";");
			}
		}
	}
}
