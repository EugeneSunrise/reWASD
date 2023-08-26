using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.View.SecondaryWindows.Base;
using DiscSoft.NET.Common.View.SecondaryWindows.WaitDialog;
using GIMXEngine;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard;
using XBEliteWPF.Infrastructure.ExternalDevices;

namespace reWASDUI.Views.SecondaryWindows.AddExternalDeviceWizard
{
	public partial class Wizard : BaseSecondaryWindow
	{
		public ExternalDevice Result
		{
			get
			{
				return this._wizardVM.Result;
			}
		}

		public Wizard(bool skipFirstPage = false)
		{
			this.InitializeComponent();
			this._wizardVM = new WizardVM(skipFirstPage);
			this.Init();
		}

		public Wizard(PageType page)
		{
			this.InitializeComponent();
			this._wizardVM = new WizardVM(page);
			this.Init();
		}

		private void Init()
		{
			this._wizardVM.CancelEvent += delegate
			{
				this.WindowResult = MessageBoxResult.Cancel;
				base.Close();
			};
			this._wizardVM.OkEvent += delegate
			{
				this.WindowResult = MessageBoxResult.OK;
				base.Close();
			};
			base.DataContext = this._wizardVM;
		}

		protected override void OkButton_Click(object sender, RoutedEventArgs e)
		{
			this._wizardVM.MoveNext();
		}

		private static bool DownloadEsptool()
		{
			Wizard wizard = new Wizard(PageType.ToolDownloader);
			new EspToolDownloader(wizard.DataContext as WizardVM).Download();
			wizard.ShowDialog();
			return FirmwareLoader.IsEsptoolExistAndCorrect();
		}

		private static bool ShowEspError(ExternalDeviceType deviceType, FirmwareLoader.FirwareUpdateResult esptoolResult)
		{
			Wizard wizard = new Wizard(PageType.EspFailsStage);
			wizard.Result.DeviceType = deviceType;
			EspFailsStageVM espFailsStageVM = (wizard.DataContext as WizardVM).CurrentPage as EspFailsStageVM;
			espFailsStageVM.EspFailsHeader = DTLocalization.GetString(12669);
			if (esptoolResult == 2)
			{
				espFailsStageVM.EspFailsMessage = DTLocalization.GetString(12659);
			}
			else
			{
				espFailsStageVM.EspFailsMessage = DTLocalization.GetString(12680) + Environment.NewLine + Environment.NewLine + string.Format(DTLocalization.GetString(12681), deviceType, "https://help.rewasd.com/external-devices/esp32-bluetooth-adapter.html#native_drivers");
			}
			wizard.ShowDialog();
			return wizard.WindowResult == MessageBoxResult.OK;
		}

		public static async Task<FirmwareLoader.FirwareUpdateResult> ClearESP32(ExternalDevice device)
		{
			Wizard.<>c__DisplayClass9_0 CS$<>8__locals1 = new Wizard.<>c__DisplayClass9_0();
			CS$<>8__locals1.device = device;
			CS$<>8__locals1.result = 1;
			for (;;)
			{
				Wizard.<>c__DisplayClass9_1 CS$<>8__locals2 = new Wizard.<>c__DisplayClass9_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				if (!FirmwareLoader.IsEsptoolExistAndCorrect() && !Wizard.DownloadEsptool())
				{
					break;
				}
				WaitDialog.ShowDialogStatic(DTLocalization.GetString(12670), null, null, false, false, null, null);
				CS$<>8__locals2.loader = new FirmwareLoader(CS$<>8__locals2.CS$<>8__locals1.device.SerialPort);
				await Task.Run<FirmwareLoader.FirwareUpdateResult>(() => CS$<>8__locals2.CS$<>8__locals1.result = CS$<>8__locals2.loader.ClearESP32(CS$<>8__locals2.CS$<>8__locals1.device.DeviceType == 3));
				WaitDialog.TryCloseWaitDialog();
				bool flag = false;
				if (CS$<>8__locals2.CS$<>8__locals1.result != null)
				{
					flag = Wizard.ShowEspError(CS$<>8__locals2.CS$<>8__locals1.device.DeviceType, CS$<>8__locals2.CS$<>8__locals1.result);
				}
				CS$<>8__locals2 = null;
				if (!flag)
				{
					goto Block_4;
				}
			}
			return 1;
			Block_4:
			return CS$<>8__locals1.result;
		}

		private WizardVM _wizardVM;
	}
}
