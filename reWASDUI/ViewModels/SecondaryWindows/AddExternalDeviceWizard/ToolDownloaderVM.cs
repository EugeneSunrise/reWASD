using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using DiscSoft.NET.Common.Localization;
using XBEliteWPF.Utils.XBUtilModel;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class ToolDownloaderVM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.ToolDownloader;
			}
		}

		public string Message
		{
			get
			{
				return this._message;
			}
			set
			{
				if (this._message == value)
				{
					return;
				}
				this._message = value;
				this.OnPropertyChanged("Message");
			}
		}

		public string Header
		{
			get
			{
				return this._header;
			}
			set
			{
				if (this._header == value)
				{
					return;
				}
				this._header = value;
				this.OnPropertyChanged("Header");
			}
		}

		public double Progress
		{
			get
			{
				return this._progress;
			}
			set
			{
				if (this._progress == value)
				{
					return;
				}
				this._progress = value;
				this.OnPropertyChanged("Progress");
			}
		}

		public bool IsFailed
		{
			get
			{
				return this._isFailed;
			}
			set
			{
				if (this._isFailed != value)
				{
					this._isFailed = value;
					this.OnPropertyChanged("IsFailed");
					if (value)
					{
						this.prevHeader = this.Header;
						this.Header = DTLocalization.GetString(12684);
						this.Message = this.FailedDescription;
						return;
					}
					if (!string.IsNullOrEmpty(this.prevHeader))
					{
						this.Header = this.prevHeader;
					}
				}
			}
		}

		public string FailedDescription { get; set; }

		public string DownloadingMessage { get; set; }

		public string CheckCertThumbrint { get; set; }

		public string FileName { get; set; }

		public PageType PreviousPage { get; set; }

		public PageType NextPage { get; set; }

		public string Url
		{
			get
			{
				return this._url;
			}
			set
			{
				this._url = value;
				this.IsFailed = string.IsNullOrEmpty(this._url);
			}
		}

		private async void DownloadFile(string url, string fileName)
		{
			using (this.client = new WebClient())
			{
				this.IsFailed = false;
				base.IsProcessing = true;
				this.Message = this.DownloadingMessage;
				this.client.DownloadProgressChanged += delegate(object sender, DownloadProgressChangedEventArgs e)
				{
					double num = (double)e.BytesReceived / (double)e.TotalBytesToReceive;
					this.Progress = num * 100.0;
				};
				this.client.DownloadFileCompleted += delegate([Nullable(2)] object sender, AsyncCompletedEventArgs e)
				{
					base.IsProcessing = false;
					if (e.Error != null)
					{
						this.IsFailed = true;
						return;
					}
					if (!e.Cancelled)
					{
						if (!this.CheckDownloadedFile())
						{
							this.IsFailed = true;
							return;
						}
						base.GoPage(this.NextPage);
					}
				};
				try
				{
					await this.client.DownloadFileTaskAsync(new Uri(url), fileName);
				}
				catch (OperationCanceledException)
				{
				}
				catch (Exception)
				{
					this.IsFailed = true;
				}
			}
			WebClient webClient = null;
			this.client = null;
		}

		public ToolDownloaderVM(WizardVM wizard)
			: base(wizard)
		{
		}

		public override void OnShowPage()
		{
			if (string.IsNullOrEmpty(this.Url))
			{
				this.IsFailed = true;
			}
		}

		protected override void NavigatePreviousPage()
		{
			base.GoPage(this.PreviousPage);
		}

		protected override void NavigateToNextPage()
		{
			this.DownloadFile(this.Url, this.FileName);
		}

		protected override bool CanCancel()
		{
			return true;
		}

		protected override void OnCancel()
		{
			WebClient webClient = this.client;
			if (webClient != null)
			{
				webClient.CancelAsync();
			}
			this._wizard.OnCancel();
		}

		private bool CheckDownloadedFile()
		{
			return string.IsNullOrEmpty(this.CheckCertThumbrint) || XBUtils.GetCertThumbprint(this.FileName).ToLower() == this.CheckCertThumbrint;
		}

		private string _message;

		private string _header;

		private double _progress;

		private string prevHeader;

		private bool _isFailed;

		private string _url;

		private WebClient client;
	}
}
