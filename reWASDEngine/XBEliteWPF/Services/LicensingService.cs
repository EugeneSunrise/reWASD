using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.License;
using XBEliteWPF.License.Licensing;
using XBEliteWPF.License.Licensing.ComStructures;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.ViewModels;

namespace XBEliteWPF.Services
{
	public class LicensingService : LicenseMainVM, ILicensingService
	{
		public event LicenseServiceDelegates.LicenseChangedDelegate OnLicenseChangedCompleted;

		public event LicenseServiceDelegates.LicenseFailedDelegate OnLicenseFailed;

		public event LicenseServiceDelegates.LicenseActivatedDelegate OnLicenseActivated;

		public bool IsValidLicense
		{
			get
			{
				return this._isValidLicense;
			}
		}

		public bool NewVersionAvailable { get; set; }

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
			return await LicenseApi.GetLicenseInfo();
		}

		public void InitPartner()
		{
			if (!string.IsNullOrWhiteSpace(""))
			{
				LicenseApi.Function12("");
			}
		}

		public Task<LicenseCheckResult> CheckLicenseAsync(bool force = false)
		{
			LicensingService.<CheckLicenseAsync>d__36 <CheckLicenseAsync>d__;
			<CheckLicenseAsync>d__.<>t__builder = AsyncTaskMethodBuilder<LicenseCheckResult>.Create();
			<CheckLicenseAsync>d__.<>4__this = this;
			<CheckLicenseAsync>d__.force = force;
			<CheckLicenseAsync>d__.<>1__state = -1;
			<CheckLicenseAsync>d__.<>t__builder.Start<LicensingService.<CheckLicenseAsync>d__36>(ref <CheckLicenseAsync>d__);
			return <CheckLicenseAsync>d__.<>t__builder.Task;
		}

		private async Task OnCheckLicenseResult(LicenseCheckResult result)
		{
			bool flag = await this.CheckActivationResult(result);
			this._isValidLicense = flag;
		}

		private async Task<bool> CheckActivationResult(LicenseCheckResult result)
		{
			bool flag2;
			if (result.Result == null)
			{
				bool flag = await LicenseApi.IsOfferExist();
				this.NewVersionAvailable = result.NewVersionAvailable == 1;
				if (flag)
				{
					await LicenseApi.GetOffer();
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
				this.NewVersionAvailable = result.NewVersionAvailable == 1;
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

		public Task<LicenseCheckResult> CheckForUpdate()
		{
			LicensingService.<CheckForUpdate>d__40 <CheckForUpdate>d__;
			<CheckForUpdate>d__.<>t__builder = AsyncTaskMethodBuilder<LicenseCheckResult>.Create();
			<CheckForUpdate>d__.<>4__this = this;
			<CheckForUpdate>d__.<>1__state = -1;
			<CheckForUpdate>d__.<>t__builder.Start<LicensingService.<CheckForUpdate>d__40>(ref <CheckForUpdate>d__);
			return <CheckForUpdate>d__.<>t__builder.Task;
		}

		public async void ShowLicenseDialog()
		{
			await LicenseApi.CheckLicense(false);
		}

		protected override Task<bool> ProcessResult_LicenseError(Ref<LicenseCheckResult> checkingResultInfo)
		{
			if (checkingResultInfo.Value.Result == 29 || checkingResultInfo.Value.Result == 24 || checkingResultInfo.Value.Result == 2 || checkingResultInfo.Value.Result == 18)
			{
				base.ClearSerial();
			}
			return Task.FromResult<bool>(checkingResultInfo.Value.IsSuccessResult != 1);
		}

		public override async Task OnLicenseChanged(LicenseCheckResult result, bool onlineActivation = true)
		{
			await base.OnLicenseChanged(result, onlineActivation);
			LicenseServiceDelegates.LicenseChangedDelegate onLicenseChangedCompleted = this.OnLicenseChangedCompleted;
			if (onLicenseChangedCompleted != null)
			{
				onLicenseChangedCompleted.Invoke(result, onlineActivation);
			}
		}

		public async Task<bool> TryActivateFeatureOnApplyConfig(bool needMacroFeature, bool needAdvancedMappingFeature, bool needRapidFireFeature, bool needMobileControllerFeature, string needFeatureAdditionalInfo, bool showGuiMessages, EnableRemapBundle enableRemapBundle = null, EnableRemapResponse enableRemapResponse = null)
		{
			string text = "";
			string text2 = "";
			string text3 = "";
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			if (needMacroFeature && base.FeatureCanBeTrial("macros"))
			{
				text += base.GetFeatureByGUID("macros").BigName.Value;
				list2.Add("macros");
			}
			if (needMacroFeature && !base.FeatureCanBeTrial("macros"))
			{
				text2 += base.GetFeatureByGUID("macros").BigName.Value;
				list.Add("macros");
			}
			if (needAdvancedMappingFeature && base.FeatureCanBeTrial("advanced-mapping"))
			{
				text = text + (string.IsNullOrEmpty(text) ? "" : ", ") + base.GetFeatureByGUID("advanced-mapping").BigName.Value;
				list2.Add("advanced-mapping");
			}
			if (needAdvancedMappingFeature && !base.FeatureCanBeTrial("advanced-mapping"))
			{
				text2 = text2 + (string.IsNullOrEmpty(text2) ? "" : ", ") + base.GetFeatureByGUID("advanced-mapping").BigName.Value;
				list.Add("advanced-mapping");
			}
			if (needRapidFireFeature && base.FeatureCanBeTrial("rapid-fire"))
			{
				text = text + (string.IsNullOrEmpty(text) ? "" : ", ") + base.GetFeatureByGUID("rapid-fire").BigName.Value;
				list2.Add("rapid-fire");
			}
			if (needRapidFireFeature && !base.FeatureCanBeTrial("rapid-fire"))
			{
				text2 = text2 + (string.IsNullOrEmpty(text2) ? "" : ", ") + base.GetFeatureByGUID("rapid-fire").BigName.Value;
				list.Add("rapid-fire");
			}
			if (needMobileControllerFeature && base.FeatureCanBeTrial("mobile-controller"))
			{
				text = text + (string.IsNullOrEmpty(text) ? "" : ", ") + base.GetFeatureByGUID("mobile-controller").BigName.Value;
				list2.Add("mobile-controller");
			}
			if (needMobileControllerFeature && !base.FeatureCanBeTrial("mobile-controller"))
			{
				text2 = text2 + (string.IsNullOrEmpty(text2) ? "" : ", ") + base.GetFeatureByGUID("mobile-controller").BigName.Value;
				list.Add("mobile-controller");
			}
			bool flag = !string.IsNullOrEmpty(text);
			bool flag2 = !string.IsNullOrEmpty(text2);
			if (flag2 && list2.Count > 0)
			{
				using (List<string>.Enumerator enumerator = list2.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string featureCanTry = enumerator.Current;
						if (!list.Exists((string x) => x == featureCanTry))
						{
							list.Add(featureCanTry);
						}
					}
				}
			}
			if (flag2)
			{
				text3 += string.Format(DTLocalization.GetString(11582), text2, Environment.NewLine);
			}
			if (flag)
			{
				text3 += string.Format(DTLocalization.GetString(11581), text, Environment.NewLine);
			}
			if (flag && !flag2)
			{
				text3 += DTLocalization.GetString(11580);
			}
			bool flag3 = enableRemapBundle != null;
			bool flag4 = enableRemapBundle != null && enableRemapBundle.IsUI;
			if (flag3)
			{
				if (enableRemapBundle.UserActions.Exists((EnableRemapButtonAction x) => x == 1))
				{
					return true;
				}
			}
			if (flag && !flag2)
			{
				if (flag3)
				{
					if (RegistryHelper.GetBool(RegistryHelper.CONFIRMATION_REG_PATH, "TryApplyLockedTrial", true))
					{
						if (!flag4)
						{
							this.AddLicenseDialogForMobileClient(enableRemapResponse, needMobileControllerFeature);
							return false;
						}
						if (!enableRemapBundle.UserActions.Exists((EnableRemapButtonAction x) => x == 15))
						{
							if (!enableRemapBundle.UserActions.Exists((EnableRemapButtonAction x) => x == 16))
							{
								this.AddLicenseDialogForUI(enableRemapResponse, text3, 15, DTLocalization.GetString(5000), 16, DTLocalization.GetString(5001));
								return false;
							}
						}
						if (enableRemapBundle.UserActions.Exists((EnableRemapButtonAction x) => x == 15))
						{
							if (needMacroFeature && base.FeatureCanBeTrial("macros"))
							{
								await base.TryFeature("macros");
							}
							if (needAdvancedMappingFeature && base.FeatureCanBeTrial("advanced-mapping"))
							{
								await base.TryFeature("advanced-mapping");
							}
							if (needRapidFireFeature && base.FeatureCanBeTrial("rapid-fire"))
							{
								await base.TryFeature("rapid-fire");
							}
							if (needMobileControllerFeature && base.FeatureCanBeTrial("mobile-controller"))
							{
								await base.TryFeature("mobile-controller");
							}
						}
					}
					else if (RegistryHelper.GetBool("GuiNamespace", "TryApplyLockedTrialResult", false))
					{
						if (needMacroFeature && base.FeatureCanBeTrial("macros"))
						{
							await base.TryFeature("macros");
						}
						if (needAdvancedMappingFeature && base.FeatureCanBeTrial("advanced-mapping"))
						{
							await base.TryFeature("advanced-mapping");
						}
						if (needRapidFireFeature && base.FeatureCanBeTrial("rapid-fire"))
						{
							await base.TryFeature("rapid-fire");
						}
						if (needMobileControllerFeature && base.FeatureCanBeTrial("mobile-controller"))
						{
							await base.TryFeature("mobile-controller");
						}
					}
					return true;
				}
			}
			else if (flag3)
			{
				if (RegistryHelper.GetBool(RegistryHelper.CONFIRMATION_REG_PATH, "TryApplyLockedToNoTrial", true))
				{
					if (!flag4)
					{
						this.AddLicenseDialogForMobileClient(enableRemapResponse, needMobileControllerFeature);
						return false;
					}
					if (!enableRemapBundle.UserActions.Exists((EnableRemapButtonAction x) => x == 1))
					{
						string buyUrlForFeatures = base.GetBuyUrlForFeatures(list);
						EnableRemapResponseDialog enableRemapResponseDialog = this.AddLicenseDialogForUI(enableRemapResponse, text3, 1, DTLocalization.GetString(11824), 0, DTLocalization.GetString(8322));
						enableRemapResponseDialog.AdditionalParameter = buyUrlForFeatures;
						enableRemapResponseDialog.LicenseInfo = needFeatureAdditionalInfo;
						return false;
					}
				}
				return true;
			}
			return true;
		}

		private void AddLicenseDialogForMobileClient(EnableRemapResponse enableRemapResponse, bool needMobileControllerFeature)
		{
			if (enableRemapResponse == null)
			{
				enableRemapResponse = new EnableRemapResponse();
			}
			EnableRemapResponseDialog enableRemapResponseDialog = new EnableRemapResponseDialog
			{
				Message = (needMobileControllerFeature ? DTLocalization.GetString(12547) : DTLocalization.GetString(12208))
			};
			if (needMobileControllerFeature)
			{
				enableRemapResponseDialog.AddButton(new EnableRemapResponseButton
				{
					Text = DTLocalization.GetString(5005),
					ButtonAction = 0
				}, false);
			}
			else
			{
				enableRemapResponseDialog.AddButton(new EnableRemapResponseButton
				{
					Text = DTLocalization.GetString(11824),
					ButtonAction = 1
				}, true);
				enableRemapResponseDialog.AddButton(new EnableRemapResponseButton
				{
					Text = DTLocalization.GetString(5005),
					ButtonAction = 0
				}, false);
			}
			enableRemapResponse.AddDialog(enableRemapResponseDialog);
		}

		private EnableRemapResponseDialog AddLicenseDialogForUI(EnableRemapResponse enableRemapResponse, string message, EnableRemapButtonAction yesButtonAction, string yesText, EnableRemapButtonAction noButtonAction, string noText)
		{
			if (enableRemapResponse == null)
			{
				enableRemapResponse = new EnableRemapResponse();
			}
			EnableRemapResponseDialog enableRemapResponseDialog = new EnableRemapResponseDialog
			{
				Message = message
			};
			enableRemapResponseDialog.AddButton(new EnableRemapResponseButton
			{
				Text = yesText,
				ButtonAction = yesButtonAction
			}, true);
			enableRemapResponseDialog.AddButton(new EnableRemapResponseButton
			{
				Text = noText,
				ButtonAction = noButtonAction
			}, false);
			enableRemapResponse.AddDialog(enableRemapResponseDialog);
			return enableRemapResponseDialog;
		}

		protected LicensingService.EndCommandDelegate endCommandEventEvent;

		protected LicensingService.ResultUpdateCommandDelegate resultCommand;

		private bool _isValidLicense;

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void EndCommandDelegate();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void ResultUpdateCommandDelegate(ref LicenseCheckResult result);
	}
}
