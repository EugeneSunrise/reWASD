using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using DiscSoft.NET.Common.AdminRightsFeatures;
using DiscSoft.NET.Common.DSEnums;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using DiscSoft.NET.Common.View.SecondaryWindows.WaitDialog;
using reWASDDownloader;
using XBEliteWPF.License.Licensing.ComStructures;
using XBEliteWPF.License.Licensing.Enums;
using XBEliteWPF.Utils;

namespace reWASDUI.License.Pages
{
	internal class UpdateAvailablePageVM : OfferPageVM
	{
		public UpdateAvailablePageVM(LicenseInfoModel licenseInfo)
			: base(licenseInfo)
		{
			if (!licenseInfo.UseSavedOffer)
			{
				base.Header = licenseInfo.Message;
			}
			this.Init();
		}

		public void Init()
		{
			this.webClient = new DSWebClient();
			this.webClient.DownloadFileCompleted += this.Completed;
			this.webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(this.ProgressChanged);
			this.fileTransferTimer = new Timer(10000.0);
			this.fileTransferTimer.Elapsed += this.OnNoDataTransfered;
			this.fileTransferTimer.AutoReset = false;
		}

		private void OnNoDataTransfered(object source, ElapsedEventArgs e)
		{
			this.CancelWork(false);
			ThreadHelper.ExecuteInMainDispatcher(delegate
			{
				DTMessageBox.Show(DTLocalization.GetString(4234), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
			}, true);
		}

		public bool IsOfferExist
		{
			get
			{
				return !string.IsNullOrEmpty(base.HtmlToDisplay);
			}
		}

		public void TempCleanup()
		{
			try
			{
				File.Delete(this._tmpFilePath);
			}
			catch (Exception)
			{
				UnsafeNativeMethods.MoveFileEx(this._tmpFilePath, null, UnsafeNativeMethods.MoveFileFlags.DelayUntilReboot);
			}
		}

		public void Cleanup()
		{
		}

		protected override void RefreshLicenseInfo()
		{
		}

		private void ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			bool enabled = this.fileTransferTimer.Enabled;
			this.fileTransferTimer.Stop();
			if (enabled)
			{
				this.fileTransferTimer.Start();
			}
		}

		private async void Completed(object sender, AsyncCompletedEventArgs e)
		{
			this.fileTransferTimer.Stop();
			try
			{
				if (!File.Exists(this._tmpFilePath))
				{
					SenderGoogleAnalytics.SendDebugEvent(5, -1);
				}
				if (new FileInfo(this._tmpFilePath).Length <= 0L)
				{
					SenderGoogleAnalytics.SendDebugEvent(6, -1);
				}
			}
			catch (Exception ex)
			{
				SenderGoogleAnalytics.SendDebugEvent(7, ex.HResult);
			}
			this._isStarted = false;
			WaitDialog.TryCloseWaitDialog();
			if (!this._isCanceled)
			{
				try
				{
					string arguments = "";
					LicenseType licenseType;
					if (this._licenseInfo.UseSavedOffer)
					{
						TaskAwaiter<LicenseInfo> taskAwaiter = App.HttpClientService.LicenseApi.GetLicenseInfo().GetAwaiter();
						if (!taskAwaiter.IsCompleted)
						{
							await taskAwaiter;
							TaskAwaiter<LicenseInfo> taskAwaiter2;
							taskAwaiter = taskAwaiter2;
							taskAwaiter2 = default(TaskAwaiter<LicenseInfo>);
						}
						licenseType = taskAwaiter.GetResult().License;
					}
					else
					{
						licenseType = this._licenseInfo.LicenseType;
					}
					if (licenseType == 3)
					{
						arguments = "/S /update /showgui";
					}
					Process process = new Process();
					process.StartInfo = new ProcessStartInfo
					{
						FileName = this._tmpFilePath,
						Arguments = arguments,
						Verb = "runas",
						UseShellExecute = true
					};
					process.EnableRaisingEvents = true;
					process.Exited += this.ProcessExited;
					process.Start();
					arguments = null;
				}
				catch (Exception ex2)
				{
					Win32Exception ex3 = ex2 as Win32Exception;
					if (ex3 != null && !ex3.ErrorCode.Equals(Win32ErrorCodes.ERROR_CANCELLED) && !ex3.ErrorCode.Equals(Win32ErrorCodes.ERROR_UNSPECIFIED_FAILURE))
					{
						ThreadHelper.ExecuteInMainDispatcher(delegate
						{
							DTMessageBox.Show(DTLocalization.GetString(4234), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
						}, true);
						SenderGoogleAnalytics.SendDebugEvent(4, ex2.HResult);
					}
				}
				this.Cleanup();
				this.TempCleanup();
				base.OnShowWizardPageAction(13);
			}
		}

		internal void ProcessExited(object sender, EventArgs e)
		{
			this.TempCleanup();
		}

		private void OnCancelDownloading(object sender, RoutedEventArgs e)
		{
			this.CancelWork(true);
		}

		private void CancelWork(bool bForceClose = true)
		{
			WaitDialog.TryCloseWaitDialog();
			this.Cleanup();
			this.TempCleanup();
			this._isCanceled = true;
			if (bForceClose)
			{
				base.OnShowWizardPageAction(13);
				return;
			}
			this._isStarted = false;
		}

		private void OnDownloadCommand()
		{
			if (!UACHelper.IsAdminRights() && !UACHelper.IsUACTurnedOn())
			{
				DTMessageBox.Show(DTLocalization.GetString(1505), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
				return;
			}
			this._isStarted = true;
			string text;
			if (this._licenseInfo.UseSavedOffer)
			{
				text = this._licenseInfo.SavedOffer.OfferLink;
				if (string.IsNullOrWhiteSpace(text))
				{
					SenderGoogleAnalytics.SendDebugEvent(1, -1);
				}
			}
			else
			{
				text = this._licenseInfo.UpdateURL;
				if (string.IsNullOrWhiteSpace(text))
				{
					SenderGoogleAnalytics.SendDebugEvent(2, -1);
				}
			}
			this._tmpFilePath = DSUtils.CreateFileWithUniqueName(Path.GetTempPath(), "reWASD.exe", true);
			if (!string.IsNullOrWhiteSpace(this._tmpFilePath))
			{
				WaitDialog.ShowDialogStatic(DTLocalization.GetString(4478), null, delegate
				{
					this.OnCancelDownloading(null, null);
				}, true, false, null, null);
				this.fileTransferTimer.Start();
				this.webClient.DownloadFileAsync(new Uri(text), this._tmpFilePath);
				base.OnShowWizardPageAction(13);
				return;
			}
			SenderGoogleAnalytics.SendDebugEvent(3, -1);
		}

		public ICommand DownloadCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._downloadCommand) == null)
				{
					relayCommand = (this._downloadCommand = new RelayCommand(new Action(this.OnDownloadCommand), new Func<bool>(this.CanExecuteDownload)));
				}
				return relayCommand;
			}
		}

		private bool CanExecuteDownload()
		{
			return !this._isStarted;
		}

		private void OnSkipCommand()
		{
			RegistryHelper.SetString("SkipVersion", "Config", this._licenseInfo.Message);
			base.OnShowWizardPageAction(13);
		}

		public ICommand SkipCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._skipCommand) == null)
				{
					relayCommand = (this._skipCommand = new RelayCommand(new Action(this.OnSkipCommand)));
				}
				return relayCommand;
			}
		}

		private void OnRemindLaterCommand()
		{
			base.OnShowWizardPageAction(13);
		}

		public string Description
		{
			get
			{
				return this._licenseInfo.UpdateNotes;
			}
		}

		public ICommand RemindLaterCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._remindLaterCommand) == null)
				{
					relayCommand = (this._remindLaterCommand = new RelayCommand(new Action(this.OnRemindLaterCommand)));
				}
				return relayCommand;
			}
		}

		private DSWebClient webClient;

		private Timer fileTransferTimer;

		private string _tmpFilePath;

		private bool _isStarted;

		private bool _isCanceled;

		private RelayCommand _downloadCommand;

		private RelayCommand _skipCommand;

		private RelayCommand _remindLaterCommand;
	}
}
