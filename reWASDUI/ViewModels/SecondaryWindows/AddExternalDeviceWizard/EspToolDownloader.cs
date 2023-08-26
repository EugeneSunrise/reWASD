using System;
using System.IO;
using System.Net;
using System.Text;
using DiscSoft.NET.Common.Localization;
using GIMXEngine;
using reWASDCommon.Infrastructure;
using reWASDDownloader;
using reWASDUI.Utils.WebUtils;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class EspToolDownloader
	{
		public PageType PrevPage { get; set; } = PageType.None;

		public PageType NextPage { get; set; } = PageType.None;

		public EspToolDownloader(WizardVM wizard)
		{
			this._wizard = wizard;
		}

		public bool IsExistAndCorrect()
		{
			return FirmwareLoader.IsEsptoolExistAndCorrect();
		}

		private string GetUrl()
		{
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://disc-tools.com/downloader?p=" + Uri.EscapeDataString(AESCrypter.EncryptString("esptool", AESCrypter.StringToByteArray("828d7a5c304cf43e379de0208015c928"))));
				httpWebRequest.Proxy = Program.DefaultWebProxy;
				httpWebRequest.UserAgent = Program.PRODUCT_USERAGENT;
				httpWebRequest.Timeout = DSWebClient.DEFAULT_TIMEOUT;
				using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
				{
					using (Stream responseStream = httpWebResponse.GetResponseStream())
					{
						using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
						{
							return AESCrypter.DecryptString(streamReader.ReadToEnd(), AESCrypter.StringToByteArray("9c30891ec516722d206265bcf26d79cb"));
						}
					}
				}
			}
			catch (Exception)
			{
			}
			return "";
		}

		public void Download()
		{
			ToolDownloaderVM toolDownloaderVM = this._wizard.FindPage(PageType.ToolDownloader) as ToolDownloaderVM;
			if (toolDownloaderVM != null)
			{
				toolDownloaderVM.NextPage = this.NextPage;
				toolDownloaderVM.PreviousPage = this.PrevPage;
				string text = Path.Combine(new string[] { Path.Combine(Constants.PROGRAMM_DATA_DIRECTORY_PATH, "esptool") });
				try
				{
					Directory.CreateDirectory(text);
				}
				catch (Exception)
				{
				}
				toolDownloaderVM.FileName = Path.Combine(text, "esptool.exe");
				toolDownloaderVM.Url = this.GetUrl();
				toolDownloaderVM.Header = DTLocalization.GetString(12682);
				toolDownloaderVM.Message = string.Format(DTLocalization.GetString(12683), "https://help.rewasd.com/external-devices/esp32-bluetooth-adapter.html#esptool");
				toolDownloaderVM.DownloadingMessage = DTLocalization.GetString(12686);
				toolDownloaderVM.FailedDescription = string.Format(DTLocalization.GetString(12685), "https://help.rewasd.com/external-devices/esp32-bluetooth-adapter.html#esptool-download");
				toolDownloaderVM.CheckCertThumbrint = FirmwareLoader.EsptoolThumbprint;
				this._wizard.GoPage(PageType.ToolDownloader);
			}
		}

		private WizardVM _wizard;
	}
}
