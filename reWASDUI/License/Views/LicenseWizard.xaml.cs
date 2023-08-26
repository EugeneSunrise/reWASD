using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.Utils;
using XBEliteWPF.License.Licensing.ComStructures;

namespace reWASDUI.License.Views
{
	public partial class LicenseWizard : Window
	{
		public LicenseWizard()
		{
			base.Owner = Application.Current.MainWindow;
			this.InitializeComponent();
			base.Title = "reWASD";
		}

		public async Task Init(Ref<LicenseCheckResult> checkingResultInfo)
		{
			this._viewModel = new LicenseWizardVM();
			await this._viewModel.Init(checkingResultInfo);
			this._viewModel.OnClose += this.CloseApp;
			base.DataContext = this._viewModel;
		}

		public async Task Init(Ref<HtmlOffer> offer, Ref<LicenseCheckResult> checkingResultInfo)
		{
			this._viewModel = new LicenseWizardVM();
			await this._viewModel.Init(offer, checkingResultInfo);
			this._viewModel.OnClose += base.Close;
			base.DataContext = this._viewModel;
		}

		public async Task<bool> IsTerminate()
		{
			return await this._viewModel.IsTerminate();
		}

		public async Task<bool> DoModalUpdateAvailable()
		{
			this._viewModel.ShowWizardPageAction(11);
			base.ShowDialog();
			return await this._viewModel.IsTerminate();
		}

		public async Task<bool> DoModalLicenseError()
		{
			if (this._viewModel.ProcessLicenseError())
			{
				try
				{
					base.ShowDialog();
				}
				catch (Exception)
				{
				}
			}
			return await this._viewModel.IsTerminate();
		}

		public async Task<bool> DoModalOffer()
		{
			this._viewModel.OfferOnly = true;
			base.ShowDialog();
			return await this._viewModel.IsTerminate();
		}

		public async Task<bool> DoModalLicenseInfo()
		{
			this._viewModel.ShowWizardPageAction(0);
			base.ShowDialog();
			return await this._viewModel.IsTerminate();
		}

		private async void CloseApp()
		{
			TaskAwaiter<bool> taskAwaiter = this._viewModel.IsTerminate().GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<bool> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			if (!taskAwaiter.GetResult())
			{
				base.Close();
			}
			else
			{
				Application.Current.Shutdown();
			}
		}

		private LicenseWizardVM _viewModel;
	}
}
