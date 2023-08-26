using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View.Controls.Buttons;
using QRCoder;
using reWASDCommon.Utils;

namespace reWASDUI.Views.ContentZoneGamepad.GamepadSelector
{
	public class DummyEngineController : UserControl, INotifyPropertyChanged, IComponentConnector, IStyleConnector
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
			}
		}

		public DummyEngineController()
		{
			this.InitializeComponent();
			Bitmap bitmap = null;
			try
			{
				QRCodeGenerator qrcodeGenerator = new QRCodeGenerator();
				string networkIpList = DummyEngineController.GetNetworkIpList(1000L);
				if (string.IsNullOrWhiteSpace(networkIpList))
				{
					Tracer.TraceWrite("Failed to get interface ip. QR code is invalid", false);
					return;
				}
				bitmap = new QRCode(qrcodeGenerator.CreateQrCode(networkIpList, 2, false, false, 0, -1)).GetGraphic(20);
			}
			catch (Exception)
			{
			}
			if (bitmap == null)
			{
				return;
			}
			using (MemoryStream memoryStream = new MemoryStream())
			{
				bitmap.Save(memoryStream, ImageFormat.Bmp);
				memoryStream.Position = 0L;
				BitmapImage bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.StreamSource = memoryStream;
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.EndInit();
				this.BitmapImage = bitmapImage;
			}
		}

		public BitmapImage BitmapImage
		{
			get
			{
				return this.bitmapImage;
			}
			set
			{
				if (this.bitmapImage != value)
				{
					this.bitmapImage = value;
					this.OnPropertyChanged("BitmapImage");
				}
			}
		}

		private void GooglePlay_OnClick(object sender, RoutedEventArgs e)
		{
			Process.Start(new ProcessStartInfo("https://apps.apple.com/app/id1586976147")
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

		private static string GetNetworkIpList(long minimumSpeed)
		{
			if (!NetworkInterface.GetIsNetworkAvailable())
			{
				return null;
			}
			int port = HttpServerSettings.GetPort();
			string text = "";
			foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (networkInterface.OperationalStatus == OperationalStatus.Up && networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback && networkInterface.NetworkInterfaceType != NetworkInterfaceType.Tunnel && networkInterface.Speed >= minimumSpeed && networkInterface.Description.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) < 0 && networkInterface.Name.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) < 0 && !networkInterface.Description.Equals("Microsoft Loopback Adapter", StringComparison.OrdinalIgnoreCase) && (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet))
				{
					foreach (UnicastIPAddressInformation unicastIPAddressInformation in networkInterface.GetIPProperties().UnicastAddresses)
					{
						if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork && !unicastIPAddressInformation.Address.ToString().StartsWith("169.254."))
						{
							string text2 = text;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 2);
							defaultInterpolatedStringHandler.AppendLiteral("http://");
							defaultInterpolatedStringHandler.AppendFormatted<IPAddress>(unicastIPAddressInformation.Address);
							defaultInterpolatedStringHandler.AppendLiteral(":");
							defaultInterpolatedStringHandler.AppendFormatted<int>(port);
							text = text2 + defaultInterpolatedStringHandler.ToStringAndClear() + "\n";
						}
					}
				}
			}
			return text;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/reWASD;component/views/contentzonegamepad/gamepadselector/enginecontroller/dummyenginecontroller.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			this._contentLoaded = true;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				((SVGButton)target).Click += this.GooglePlay_OnClick;
				return;
			}
			if (connectionId != 2)
			{
				return;
			}
			((SVGButton)target).Click += this.AppStore_OnClick;
		}

		private BitmapImage bitmapImage;

		private bool _contentLoaded;
	}
}
