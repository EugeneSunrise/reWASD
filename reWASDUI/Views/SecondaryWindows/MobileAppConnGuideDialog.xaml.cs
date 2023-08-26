using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.SecondaryWindows.Base;

namespace reWASDUI.Views.SecondaryWindows
{
	public partial class MobileAppConnGuideDialog : BaseSecondaryWindow
	{
		public MobileAppConnGuideDialog(Bitmap qr)
		{
			this.InitializeComponent();
			if (qr == null)
			{
				return;
			}
			using (MemoryStream memoryStream = new MemoryStream())
			{
				qr.Save(memoryStream, ImageFormat.Bmp);
				memoryStream.Position = 0L;
				BitmapImage bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.StreamSource = memoryStream;
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.EndInit();
				this.QRView.Source = bitmapImage;
			}
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			this.WindowResult = MessageBoxResult.Cancel;
			base.Close();
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
	}
}
