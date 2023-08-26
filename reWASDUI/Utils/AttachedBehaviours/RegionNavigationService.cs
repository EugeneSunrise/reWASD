using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using DiscSoft.NET.Common.Utils;
using Prism.Commands;
using Prism.Regions;
using reWASDUI.Controls;
using reWASDUI.Infrastructure;

namespace reWASDUI.Utils.AttachedBehaviours
{
	public class RegionNavigationService
	{
		public static string GetText(DependencyObject d)
		{
			return d.GetValue(RegionNavigationService.TextProperty) as string;
		}

		public static void SetText(DependencyObject d, string value)
		{
			d.SetValue(RegionNavigationService.TextProperty, value);
		}

		private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DispatcherTimer timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(30.0)
			};
			timer.Start();
			timer.Tick += delegate([Nullable(2)] object sender, EventArgs args)
			{
				TextBlock textBlock = d as TextBlock;
				if (textBlock == null)
				{
					return;
				}
				string text = (string)e.NewValue;
				string text2 = (string)e.NewValue;
				if (string.IsNullOrEmpty(text))
				{
					return;
				}
				int num = text.IndexOf("<cmd val=\"");
				bool flag = false;
				while (num != -1)
				{
					textBlock.Inlines.Clear();
					if (num != 0)
					{
						flag = true;
						textBlock.Inlines.Add(new Run(text.Substring(0, num)));
						text = text.Substring(num + 10, text.Length - num - 10);
						int num2 = text.IndexOf("\"");
						string text3 = text.Substring(0, num2);
						text = text.Substring(num2 + 2, text.Length - num2 - 2);
						num2 = text.IndexOf("</cmd>");
						string text4 = text.Substring(0, num2);
						text = text.Substring(num2 + 6, text.Length - num2 - 6);
						RegionHyperlink regionHyperlink = new RegionHyperlink(text3, text4);
						Hyperlink hyperlink = regionHyperlink;
						RoutedEventHandler routedEventHandler;
						if ((routedEventHandler = RegionNavigationService.<>O.<0>__OnUrlClick) == null)
						{
							routedEventHandler = (RegionNavigationService.<>O.<0>__OnUrlClick = new RoutedEventHandler(RegionNavigationService.OnUrlClick));
						}
						hyperlink.Click += routedEventHandler;
						textBlock.Inlines.Add(regionHyperlink);
					}
					num = text.IndexOf("<cmd val");
				}
				if (flag)
				{
					textBlock.Inlines.Add(new Run(text));
				}
				timer.Stop();
			};
		}

		private static void OnUrlClick(object sender, RoutedEventArgs e)
		{
			RegionHyperlink regionHyperlink = (RegionHyperlink)sender;
			string text = regionHyperlink.Text;
			NavigationInfo navigationInfo = new NavigationInfo();
			Window window = Window.GetWindow(regionHyperlink);
			if (window != null)
			{
				window.Close();
			}
			RegionNavigationService.ParseHrefInfo(ref navigationInfo, text);
			string text2 = "XBEliteWPF.Views." + navigationInfo.ViewName;
			try
			{
				Type type = Type.GetType(text2);
				CompositeCommand compositeCommand = reWASDApplicationCommands.NavigateContentCommand;
				if (navigationInfo.RegionName.Equals(RegionNames.Content))
				{
					compositeCommand = reWASDApplicationCommands.NavigateContentCommand;
				}
				else if (navigationInfo.RegionName.Equals(RegionNames.Sidebar))
				{
					compositeCommand = reWASDApplicationCommands.NavigateSideBarRegionCommand;
				}
				else if (navigationInfo.RegionName.Equals(RegionNames.Gamepad))
				{
					compositeCommand = reWASDApplicationCommands.NavigateGamepadCommand;
				}
				else if (navigationInfo.RegionName.Equals(RegionNames.BindingFrame))
				{
					compositeCommand = reWASDApplicationCommands.NavigateBindingFrameCommand;
				}
				if (type != null)
				{
					compositeCommand.Execute(new Dictionary<object, object>
					{
						{
							"navigatePath",
							type.ToString()
						},
						{ "NavigationParameters", navigationInfo.NavParams }
					});
				}
			}
			catch (Exception ex)
			{
				Tracer.TraceWrite(ex.Message + "\n" + ex.StackTrace, false);
			}
		}

		private static void ParseHrefInfo(ref NavigationInfo navInfo, string wholeText)
		{
			int num = wholeText.IndexOf("region=") + 7;
			int num2 = wholeText.IndexOf(",", num);
			navInfo.RegionName = wholeText.Substring(num, num2 - num);
			int num3 = wholeText.IndexOf("view=") + 5;
			int num4 = wholeText.IndexOf(",", num3);
			if (num4 < 0)
			{
				num4 = wholeText.Length;
			}
			navInfo.ViewName = wholeText.Substring(num3, num4 - num3);
			NavigationParameters navigationParameters = new NavigationParameters();
			int num5 = wholeText.IndexOf("params=");
			if (num5 >= 0)
			{
				int length = wholeText.Length;
				string text = wholeText.Substring(num5 + 7, length - num5 - 7);
				string text2 = text.Split(':', StringSplitOptions.None)[0];
				string text3 = text.Split(':', StringSplitOptions.None)[1];
				navigationParameters.Add(text2, text3);
			}
			navInfo.NavParams = navigationParameters;
		}

		public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached("Text", typeof(string), typeof(RegionNavigationService), new PropertyMetadata(null, new PropertyChangedCallback(RegionNavigationService.OnTextChanged)));

		[CompilerGenerated]
		private static class <>O
		{
			public static RoutedEventHandler <0>__OnUrlClick;
		}
	}
}
