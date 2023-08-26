using System;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;

namespace reWASDUI.Utils.WebUtils
{
	internal class Downloader
	{
		public static async Task DownloadFile(string url, string fileName, AsyncCompletedEventHandler DownloadFileCompleted)
		{
			WebClient webClient2;
			WebClient webClient = (webClient2 = new WebClient());
			try
			{
				webClient.DownloadFileCompleted += DownloadFileCompleted;
				try
				{
					await webClient.DownloadFileTaskAsync(new Uri(url), fileName);
				}
				catch (OperationCanceledException)
				{
				}
				catch (Exception)
				{
				}
			}
			finally
			{
				if (webClient2 != null)
				{
					((IDisposable)webClient2).Dispose();
				}
			}
			webClient2 = null;
			webClient = null;
		}
	}
}
