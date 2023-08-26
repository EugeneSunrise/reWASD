using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using DiscSoft.NET.Common.View.SecondaryWindows.WaitDialog;
using Prism.Commands;
using reWASDCommon.Infrastructure;
using reWASDUI.Infrastructure;
using reWASDUI.License;
using reWASDUI.License.Features;
using reWASDUI.Services.Interfaces;
using reWASDUI.Views;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.License.Licensing.ComStructures;
using XBEliteWPF.Services.Interfaces;

namespace reWASDUI.Services
{
	public class LicensingService : LicenseMainVM, ILicensingService
	{
		[DllImport("User32.dll")]
		public static extern uint RegisterWindowMessage(string str);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int BroadcastSystemMessage(uint flags, ref uint lpInfo, uint Msg, uint wParam, uint lParam);

		public event LicenseServiceDelegates.LicenseChangedDelegate OnLicenseChangedCompleted;

		public event LicenseServiceDelegates.LicenseFailedDelegate OnLicenseFailed;

		public bool IsValidLicense
		{
			get
			{
				return this._isValidLicense;
			}
		}

		public bool IsSlotFeatureUnlocked
		{
			get
			{
				return base.Features.First((Feature f) => f.FeatureId == "four-slots").IsFeatureActivated;
			}
		}

		public bool IsMacroFeatureUnlocked
		{
			get
			{
				return base.Features.First((Feature f) => f.FeatureId == "macros").IsFeatureActivated;
			}
		}

		public bool IsAdvancedMappingFeatureUnlocked
		{
			get
			{
				return base.Features.First((Feature f) => f.FeatureId == "advanced-mapping").IsFeatureActivated;
			}
		}

		public bool IsTurboFeatureUnlocked
		{
			get
			{
				return base.Features.First((Feature f) => f.FeatureId == "rapid-fire").IsFeatureActivated;
			}
		}

		public bool IsToggleFeatureUnlocked
		{
			get
			{
				return base.Features.First((Feature f) => f.FeatureId == "rapid-fire").IsFeatureActivated;
			}
		}

		public bool IsMobileControllerFeatureUnlocked
		{
			get
			{
				return base.Features.First((Feature f) => f.FeatureId == "mobile-controller").IsFeatureActivated;
			}
		}

		public LicensingService()
		{
			Tracer.TraceWrite("Constructor for LicensingService", false);
		}

		public async Task WaitForLicenseChecked()
		{
			while (!this._isValidLicense)
			{
				await Task.Delay(100);
			}
		}

		public async Task<LicenseInfo> GetLicenseInfo()
		{
			return await App.HttpClientService.LicenseApi.GetLicenseInfo();
		}

		private async Task<LicenseCheckResult> CheckLicense(bool force = false)
		{
			LicensingService.<>c__DisplayClass30_0 CS$<>8__locals1 = new LicensingService.<>c__DisplayClass30_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.onCancel = delegate
			{
				CS$<>8__locals1.<>4__this.ExitWithTray();
			};
			ThreadHelper.ExecuteInMainDispatcher(delegate
			{
				string @string = DTLocalization.GetString(4622);
				int? num = new int?(2000);
				WaitDialog.ShowDialogStatic(@string, DTLocalization.GetString(5003), CS$<>8__locals1.onCancel, false, false, null, num);
			}, true);
			LicenseCheckResult licenseCheckResult = await App.HttpClientService.LicenseApi.CheckLicense(false);
			CS$<>8__locals1.result = licenseCheckResult;
			if (CS$<>8__locals1.result.Result == 15)
			{
				ActivationLicenseInfo activationLicenseInfo = default(ActivationLicenseInfo);
				activationLicenseInfo.licenseType = 2;
				licenseCheckResult = await base.Activate(activationLicenseInfo);
				CS$<>8__locals1.result = licenseCheckResult;
			}
			ThreadHelper.ExecuteInMainDispatcher(delegate
			{
				LicensingService.<>c__DisplayClass30_0.<<CheckLicense>b__2>d <<CheckLicense>b__2>d;
				<<CheckLicense>b__2>d.<>t__builder = AsyncVoidMethodBuilder.Create();
				<<CheckLicense>b__2>d.<>4__this = CS$<>8__locals1;
				<<CheckLicense>b__2>d.<>1__state = -1;
				<<CheckLicense>b__2>d.<>t__builder.Start<LicensingService.<>c__DisplayClass30_0.<<CheckLicense>b__2>d>(ref <<CheckLicense>b__2>d);
			}, true);
			return CS$<>8__locals1.result;
		}

		public async Task<LicenseCheckResult> CheckLicenseAsync(bool force = false)
		{
			return await this.CheckLicense(force);
		}

		private async Task OnCheckLicenseResult(LicenseCheckResult result)
		{
			WaitDialog.TryCloseWaitDialog();
			TaskAwaiter<bool> taskAwaiter = this.CheckActivationResult(result).GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<bool> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			if (taskAwaiter.GetResult())
			{
				this._isValidLicense = true;
				this.OnPropertyChanged("IsValidLicense");
			}
		}

		private async Task<bool> CheckActivationResult(LicenseCheckResult result)
		{
			if (result.IsSuccessResult == 1)
			{
				TrayAgentCommunicator.SyncAutorunValue();
			}
			bool flag2;
			if (result.Result == null)
			{
				bool flag = await App.HttpClientService.LicenseApi.IsOfferExist();
				base.NewVersionAvailable = result.NewVersionAvailable == 1;
				if (flag)
				{
					HtmlOffer htmlOffer = await App.HttpClientService.LicenseApi.GetOffer();
					IGuiHelperService guiHelperService = App.GuiHelperService;
					await (((guiHelperService != null) ? guiHelperService.ShowLicenseWizard(new Ref<HtmlOffer>(htmlOffer), new Ref<LicenseCheckResult>(result)) : null) ?? Task.FromResult<bool>(true));
				}
				await this.OnLicenseChanged(result, false);
				flag2 = true;
			}
			else
			{
				TaskAwaiter<bool> taskAwaiter = this.ProcessResult_LicenseError(new Ref<LicenseCheckResult>(result)).GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<bool> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				if (taskAwaiter.GetResult())
				{
					base.ExitWithTray();
				}
				else
				{
					await Task.Delay(1000);
					await this.CheckLicenseAsync(false);
				}
				flag2 = false;
			}
			return flag2;
		}

		private async Task OnEndCheckUpdate(LicenseCheckResult result)
		{
			if ((result.Result == null || result.Result == 14) && 16 != result.Result)
			{
				base.NewVersionAvailable = result.NewVersionAvailable == 1;
				if (result.NewVersionAvailable == 1)
				{
					await this.ProcessResult_NewVersion(new Ref<LicenseCheckResult>(result), false);
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 4);
					defaultInterpolatedStringHandler.AppendFormatted("reWASD");
					defaultInterpolatedStringHandler.AppendLiteral(" ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(6);
					defaultInterpolatedStringHandler.AppendLiteral(".");
					defaultInterpolatedStringHandler.AppendFormatted<int>(7);
					defaultInterpolatedStringHandler.AppendLiteral(".");
					defaultInterpolatedStringHandler.AppendFormatted<int>(0);
					defaultInterpolatedStringHandler.AppendLiteral(" ");
					DTMessageBox.Show(defaultInterpolatedStringHandler.ToStringAndClear() + DTLocalization.GetString(4941), MessageBoxButton.OK, MessageBoxImage.Asterisk, null, false, MessageBoxResult.None);
				}
			}
			else if (result.Result == 2)
			{
				TaskAwaiter<bool> taskAwaiter = this.ProcessResult_LicenseError(new Ref<LicenseCheckResult>(result)).GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<bool> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				if (taskAwaiter.GetResult())
				{
					base.ExitWithTray();
				}
			}
			else if (result.Result == 6 || result.IsSuccessResult != 1 || result.Result == 16)
			{
				await this.ProcessResult_LicenseError(new Ref<LicenseCheckResult>(result));
			}
			await this.OnLicenseChanged(result, true);
		}

		public DelegateCommand CheckForUpdateCommand
		{
			get
			{
				if (this._checkForUpdateCommand == null)
				{
					this._checkForUpdateCommand = new DelegateCommand(async delegate
					{
						await this.CheckForUpdate();
					}, new Func<bool>(this.CanCheckForUpdate));
				}
				return this._checkForUpdateCommand;
			}
		}

		private bool CanCheckForUpdate()
		{
			return !this._isCheckingUpdate;
		}

		public Task<LicenseCheckResult> CheckForUpdate()
		{
			LicensingService.<CheckForUpdate>d__39 <CheckForUpdate>d__;
			<CheckForUpdate>d__.<>t__builder = AsyncTaskMethodBuilder<LicenseCheckResult>.Create();
			<CheckForUpdate>d__.<>4__this = this;
			<CheckForUpdate>d__.<>1__state = -1;
			<CheckForUpdate>d__.<>t__builder.Start<LicensingService.<CheckForUpdate>d__39>(ref <CheckForUpdate>d__);
			return <CheckForUpdate>d__.<>t__builder.Task;
		}

		public async void ShowLicenseDialog()
		{
			await App.HttpClientService.LicenseApi.CheckLicense(false);
			reWASDApplicationCommands.NavigateContentCommand.Execute(typeof(LicenseMain));
		}

		private async Task ProcessResult_NewVersion(Ref<LicenseCheckResult> checkingResultInfo, bool bCheckSkipVersion)
		{
			string @string = RegistryHelper.GetString("SkipVersion", "Config", "", false);
			string message = checkingResultInfo.Value.Message;
			if (!bCheckSkipVersion || string.IsNullOrEmpty(@string) || @string != message)
			{
				IGuiHelperService guiHelperService = App.GuiHelperService;
				await (((guiHelperService != null) ? guiHelperService.ShowLicenseWizardUpdate(checkingResultInfo) : null) ?? Task.FromResult<bool>(true));
			}
		}

		protected void ClearSerial()
		{
			RegistryHelper.SetString("Config", "Serial", "");
		}

		protected override async Task<bool> ProcessResult_LicenseError(Ref<LicenseCheckResult> checkingResultInfo)
		{
			if (checkingResultInfo.Value.Result == 29 || checkingResultInfo.Value.Result == 24 || checkingResultInfo.Value.Result == 2 || checkingResultInfo.Value.Result == 18)
			{
				this.ClearSerial();
			}
			TaskAwaiter<bool> taskAwaiter = base.ProcessResult_LicenseError(checkingResultInfo).GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<bool> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			bool flag;
			if (taskAwaiter.GetResult())
			{
				Tracer.TraceWrite("Checking license failed: " + checkingResultInfo.Value.Result.ToString(), false);
				LicenseServiceDelegates.LicenseFailedDelegate onLicenseFailed = this.OnLicenseFailed;
				if (onLicenseFailed != null)
				{
					onLicenseFailed.Invoke();
				}
				flag = true;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override async Task OnLicenseChanged(LicenseCheckResult result, bool onlineActivation = true)
		{
			await base.OnLicenseChanged(result, onlineActivation);
			this.RefreshLicensingProperties();
			LicenseServiceDelegates.LicenseChangedDelegate onLicenseChangedCompleted = this.OnLicenseChangedCompleted;
			if (onLicenseChangedCompleted != null)
			{
				onLicenseChangedCompleted.Invoke(result, onlineActivation);
			}
		}

		public override void RefreshLicensingProperties()
		{
			base.RefreshLicensingProperties();
			this.OnPropertyChanged("IsMacroFeatureUnlocked");
			this.OnPropertyChanged("IsAdvancedMappingFeatureUnlocked");
			this.OnPropertyChanged("IsToggleFeatureUnlocked");
			this.OnPropertyChanged("IsTurboFeatureUnlocked");
			this.OnPropertyChanged("IsSlotFeatureUnlocked");
			this.OnPropertyChanged("IsMobileControllerFeatureUnlocked");
		}

		public Task<bool> TryActivateFeatureOnApplyConfig(bool needMacroFeature, bool needAdvancedMappingFeature, bool needRapidFireFeature, bool needMobileControllerFeature, string needFeatureAdditionalInfo, bool showGuiMessages, EnableRemapBundle enableRemapBundle = null, EnableRemapResponse enableRemapResponse = null)
		{
			return Task.FromResult<bool>(true);
		}

		protected LicensingService.EndCommandDelegate endCommandEventEvent;

		protected LicensingService.ResultUpdateCommandDelegate resultCommand;

		private bool _isValidLicense;

		private DelegateCommand _checkForUpdateCommand;

		private bool _isCheckingUpdate;

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void EndCommandDelegate();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void ResultUpdateCommandDelegate(ref LicenseCheckResult result);
	}
}
