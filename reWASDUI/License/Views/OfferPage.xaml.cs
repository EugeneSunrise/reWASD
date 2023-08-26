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
using DiscSoft.NET.Common.View.Controls.Buttons;
using Microsoft.CSharp.RuntimeBinder;
using reWASDUI.License.Pages;
using XBEliteWPF.Utils;

namespace reWASDUI.License.Views
{
	public partial class OfferPage : UserControl
	{
		public OfferPage()
		{
			this.InitializeComponent();
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri)
			{
				UseShellExecute = true
			});
			SenderGoogleAnalytics.SendMessageEvent("License", "Buy", "offer", -1L, false);
			e.Handled = true;
			OfferPageVM offerPageVM = base.DataContext as OfferPageVM;
			if (offerPageVM != null && !offerPageVM.UseSavedOffer)
			{
				(base.DataContext as BaseLicensePage).OnActivateCommand();
			}
		}

		private void webBrowser_LoadCompleted(object sender, NavigationEventArgs e)
		{
			object document = this.webBrowser.Document;
			int num = (int)typeof(SystemParameters).GetProperty("DpiX", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null);
			if (num != 96)
			{
				double num2 = (double)num;
				num2 /= 96.0;
				if (OfferPage.<>o__2.<>p__1 == null)
				{
					OfferPage.<>o__2.<>p__1 = CallSite<Action<CallSite, object, string>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "execScript", null, typeof(OfferPage), new CSharpArgumentInfo[]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
					}));
				}
				Action<CallSite, object, string> target = OfferPage.<>o__2.<>p__1.Target;
				CallSite <>p__ = OfferPage.<>o__2.<>p__1;
				if (OfferPage.<>o__2.<>p__0 == null)
				{
					OfferPage.<>o__2.<>p__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "parentWindow", typeof(OfferPage), new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				target(<>p__, OfferPage.<>o__2.<>p__0.Target(OfferPage.<>o__2.<>p__0, document), "document.body.style.zoom=" + num2.ToString().Replace(",", ".") + ";");
			}
		}
	}
}
