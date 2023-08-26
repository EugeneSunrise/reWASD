using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoftReWASDServiceNamespace;
using reWASDCommon.Infrastructure;
using XBEliteWPF.License.Licensing.ComStructures;
using XBEliteWPF.Services;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils;

namespace XBEliteWPF.License.Licensing
{
	public static class LicenseApi
	{
		private static ILicensingCommunicator LicensingCommunicator
		{
			get
			{
				if (LicenseApi._licensingCommunicator == null)
				{
					LicenseApi._licensingCommunicator = new LicensingCommunicator(new AdminOperationsDecider());
				}
				return LicenseApi._licensingCommunicator;
			}
		}

		private static async Task<bool> WaitForLicenseApiInited()
		{
			while (!LicenseApi._isLicenseApiInited)
			{
				ushort curWaitTime = 0;
				while (!LicenseApi._isLicenseApiInited && curWaitTime < 60000)
				{
					await Task.Delay(50);
					curWaitTime += 50;
				}
			}
			return true;
		}

		public static async Task Init()
		{
			await LicenseApi.LicensingCommunicator.ReinitService(true);
			await LicenseApi.InitLicenseParams();
			LicenseApi._licensingCommunicator.LicensingCommunicatorInited += async delegate
			{
				await LicenseApi.InitLicenseParams();
			};
		}

		private static async Task InitLicenseParams()
		{
			await LicenseApi.Function12("");
			await LicenseApi.GetLicenseInfo5(DTLocalization.GetString(7000).Substring(4));
			LicenseApi._isLicenseApiInited = true;
		}

		private static async Task<bool> CheckServiceVersion()
		{
			ServiceResponseWrapper<REWASD_GET_VERSION_RESPONSE> serviceResponseWrapper = await LicenseApi.LicensingCommunicator.GetVersion();
			LicenseApi._isServiceVersionChecked = true;
			if (serviceResponseWrapper.IsResponseValid)
			{
				if (serviceResponseWrapper.ServiceResponse.Header.Status == 0U && serviceResponseWrapper.ServiceResponse.Header.Size > 0U)
				{
					byte serviceMajorVersion = serviceResponseWrapper.ServiceResponse.ServiceMajorVersion;
					byte serviceMinorVersion = serviceResponseWrapper.ServiceResponse.ServiceMinorVersion;
					if (serviceMajorVersion != 5 || serviceMinorVersion != 12)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(74, 4);
						defaultInterpolatedStringHandler.AppendLiteral("LicenseApi: Check version: Service version ");
						defaultInterpolatedStringHandler.AppendFormatted<byte>(serviceMajorVersion);
						defaultInterpolatedStringHandler.AppendLiteral(".");
						defaultInterpolatedStringHandler.AppendFormatted<byte>(serviceMinorVersion);
						defaultInterpolatedStringHandler.AppendLiteral(" is not equal to GUI version ");
						defaultInterpolatedStringHandler.AppendFormatted<int>(5);
						defaultInterpolatedStringHandler.AppendLiteral(".");
						defaultInterpolatedStringHandler.AppendFormatted<int>(12);
						Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
						return false;
					}
					LicenseApi._serviceVersionEqual = true;
					return true;
				}
			}
			else
			{
				Tracer.TraceWrite("LicenseApi: CheckServiceVersion failed", false);
			}
			return false;
		}

		public static Task<LicenseInfo> GetLicenseInfo()
		{
			LicenseApi.<GetLicenseInfo>d__11 <GetLicenseInfo>d__;
			<GetLicenseInfo>d__.<>t__builder = AsyncTaskMethodBuilder<LicenseInfo>.Create();
			<GetLicenseInfo>d__.<>1__state = -1;
			<GetLicenseInfo>d__.<>t__builder.Start<LicenseApi.<GetLicenseInfo>d__11>(ref <GetLicenseInfo>d__);
			return <GetLicenseInfo>d__.<>t__builder.Task;
		}

		public static Task<LicenseCheckResult> CheckLicense(bool forceOnlineCheck)
		{
			LicenseApi.<CheckLicense>d__12 <CheckLicense>d__;
			<CheckLicense>d__.<>t__builder = AsyncTaskMethodBuilder<LicenseCheckResult>.Create();
			<CheckLicense>d__.forceOnlineCheck = forceOnlineCheck;
			<CheckLicense>d__.<>1__state = -1;
			<CheckLicense>d__.<>t__builder.Start<LicenseApi.<CheckLicense>d__12>(ref <CheckLicense>d__);
			return <CheckLicense>d__.<>t__builder.Task;
		}

		public static Task<LicenseCheckResult> ActivateLicense(ActivationLicenseInfo licenseInfo)
		{
			LicenseApi.<ActivateLicense>d__13 <ActivateLicense>d__;
			<ActivateLicense>d__.<>t__builder = AsyncTaskMethodBuilder<LicenseCheckResult>.Create();
			<ActivateLicense>d__.licenseInfo = licenseInfo;
			<ActivateLicense>d__.<>1__state = -1;
			<ActivateLicense>d__.<>t__builder.Start<LicenseApi.<ActivateLicense>d__13>(ref <ActivateLicense>d__);
			return <ActivateLicense>d__.<>t__builder.Task;
		}

		public static Task<LicenseCheckResult> CheckForUpdate()
		{
			LicenseApi.<CheckForUpdate>d__14 <CheckForUpdate>d__;
			<CheckForUpdate>d__.<>t__builder = AsyncTaskMethodBuilder<LicenseCheckResult>.Create();
			<CheckForUpdate>d__.<>1__state = -1;
			<CheckForUpdate>d__.<>t__builder.Start<LicenseApi.<CheckForUpdate>d__14>(ref <CheckForUpdate>d__);
			return <CheckForUpdate>d__.<>t__builder.Task;
		}

		public static Task<LicenseCheckResult> ActivateTrialFeature(string feautreId)
		{
			LicenseApi.<ActivateTrialFeature>d__15 <ActivateTrialFeature>d__;
			<ActivateTrialFeature>d__.<>t__builder = AsyncTaskMethodBuilder<LicenseCheckResult>.Create();
			<ActivateTrialFeature>d__.feautreId = feautreId;
			<ActivateTrialFeature>d__.<>1__state = -1;
			<ActivateTrialFeature>d__.<>t__builder.Start<LicenseApi.<ActivateTrialFeature>d__15>(ref <ActivateTrialFeature>d__);
			return <ActivateTrialFeature>d__.<>t__builder.Task;
		}

		public static async Task<bool> IsOfferExist()
		{
			await LicenseApi.WaitForLicenseApiInited();
			bool exists = false;
			try
			{
				if (!LicenseApi._isServiceVersionChecked)
				{
					await LicenseApi.CheckServiceVersion();
				}
				if (!LicenseApi._serviceVersionEqual)
				{
					return exists;
				}
				byte[] array = new byte[] { 6 };
				uint num = (uint)array.Length;
				ServiceLicensingResponseWrapper<REWASD_LICENSE_CONTROL_RESPONSE> serviceLicensingResponseWrapper = await LicenseApi.LicensingCommunicator.LicenseControl(array, num, 2U);
				if (serviceLicensingResponseWrapper.IsResponseValid && (ulong)serviceLicensingResponseWrapper.ServiceResponse.Header.Size == (ulong)(2L + (long)Marshal.SizeOf<REWASD_RESPONSE_HEADER>()))
				{
					Convert.ToBoolean(serviceLicensingResponseWrapper.ServiceResponse.Buffer[0]);
					exists = Convert.ToBoolean(serviceLicensingResponseWrapper.ServiceResponse.Buffer[1]);
					return exists;
				}
				Tracer.TraceWrite("IsOfferExist failed", false);
			}
			catch (Exception ex)
			{
				Tracer.TraceException(ex, "IsOfferExist");
			}
			return exists;
		}

		public static Task<HtmlOffer> GetOffer()
		{
			LicenseApi.<GetOffer>d__17 <GetOffer>d__;
			<GetOffer>d__.<>t__builder = AsyncTaskMethodBuilder<HtmlOffer>.Create();
			<GetOffer>d__.<>1__state = -1;
			<GetOffer>d__.<>t__builder.Start<LicenseApi.<GetOffer>d__17>(ref <GetOffer>d__);
			return <GetOffer>d__.<>t__builder.Task;
		}

		public static async Task ClearOffer()
		{
			try
			{
				if (!LicenseApi._isServiceVersionChecked)
				{
					await LicenseApi.CheckServiceVersion();
				}
				if (LicenseApi._serviceVersionEqual)
				{
					byte[] array = new byte[] { 8 };
					uint num = (uint)array.Length;
					ServiceLicensingResponseWrapper<REWASD_LICENSE_CONTROL_RESPONSE> serviceLicensingResponseWrapper = await LicenseApi.LicensingCommunicator.LicenseControl(array, num, 1U);
					if (serviceLicensingResponseWrapper.IsResponseValid && serviceLicensingResponseWrapper.ServiceResponse.Header.Size > 0U)
					{
						Convert.ToBoolean(serviceLicensingResponseWrapper.ServiceResponse.Buffer[0]);
					}
					else
					{
						Tracer.TraceWrite("ClearOffer failed", false);
					}
				}
			}
			catch (Exception ex)
			{
				Tracer.TraceException(ex, "ClearOffer");
			}
		}

		public static async Task<bool> GetLicenseInfo0(string fileName)
		{
			try
			{
				if (!LicenseApi._isServiceVersionChecked)
				{
					await LicenseApi.CheckServiceVersion();
				}
				if (!LicenseApi._serviceVersionEqual)
				{
					return false;
				}
				byte[] array = new byte[] { 10 };
				List<byte[]> list = new List<byte[]>();
				list.Add(array);
				list.Add(Encoding.Unicode.GetBytes(fileName));
				list.Add(new byte[2]);
				byte[] array2 = list.SelectMany((byte[] x) => x).ToArray<byte>();
				uint num = (uint)array2.Length;
				uint outputBufferSize = 0U;
				ServiceLicensingResponseWrapper<REWASD_LICENSE_CONTROL_RESPONSE> serviceLicensingResponseWrapper = await LicenseApi.LicensingCommunicator.LicenseControl(array2, num, outputBufferSize);
				if (serviceLicensingResponseWrapper.IsResponseValid && (ulong)serviceLicensingResponseWrapper.ServiceResponse.Header.Size == (ulong)outputBufferSize + (ulong)((long)Marshal.SizeOf<REWASD_RESPONSE_HEADER>()))
				{
					return true;
				}
				Tracer.TraceWrite("GetLicenseInfo0 failed", false);
			}
			catch (Exception ex)
			{
				Tracer.TraceException(ex, "GetLicenseInfo0");
			}
			return false;
		}

		public static async Task<bool> Function12(string partnerId)
		{
			try
			{
				if (!LicenseApi._isServiceVersionChecked)
				{
					await LicenseApi.CheckServiceVersion();
				}
				if (!LicenseApi._serviceVersionEqual)
				{
					return false;
				}
				Encoding unicode = Encoding.Unicode;
				List<byte[]> list = new List<byte[]>();
				list.Add(new byte[] { 12 });
				list.Add(unicode.GetBytes(partnerId + "\0"));
				byte[] array = list.SelectMany((byte[] x) => x).ToArray<byte>();
				uint num = (uint)array.Length;
				ServiceLicensingResponseWrapper<REWASD_LICENSE_CONTROL_RESPONSE> serviceLicensingResponseWrapper = await LicenseApi.LicensingCommunicator.LicenseControl(array, num, 0U);
				if (serviceLicensingResponseWrapper.IsResponseValid && (ulong)serviceLicensingResponseWrapper.ServiceResponse.Header.Size == (ulong)((long)Marshal.SizeOf<REWASD_RESPONSE_HEADER>()))
				{
					return true;
				}
				Tracer.TraceWrite("Function12 failed", false);
			}
			catch (Exception ex)
			{
				Tracer.TraceException(ex, "Function12");
			}
			return false;
		}

		public static async Task<bool> GetLicenseInfo4(string appVersion)
		{
			try
			{
				if (!LicenseApi._isServiceVersionChecked)
				{
					await LicenseApi.CheckServiceVersion();
				}
				if (!LicenseApi._serviceVersionEqual)
				{
					return false;
				}
				Encoding unicode = Encoding.Unicode;
				List<byte[]> list = new List<byte[]>();
				list.Add(new byte[] { 14 });
				list.Add(unicode.GetBytes(appVersion + "\0"));
				byte[] array = list.SelectMany((byte[] x) => x).ToArray<byte>();
				uint num = (uint)array.Length;
				ServiceLicensingResponseWrapper<REWASD_LICENSE_CONTROL_RESPONSE> serviceLicensingResponseWrapper = await LicenseApi.LicensingCommunicator.LicenseControl(array, num, 0U);
				if (serviceLicensingResponseWrapper.IsResponseValid && (ulong)serviceLicensingResponseWrapper.ServiceResponse.Header.Size == (ulong)((long)Marshal.SizeOf<REWASD_RESPONSE_HEADER>()))
				{
					return true;
				}
				Tracer.TraceWrite("GetLicenseInfo4 failed", false);
			}
			catch (Exception ex)
			{
				Tracer.TraceException(ex, "GetLicenseInfo4");
			}
			return false;
		}

		public static async Task<bool> GetLicenseInfo5(string languageId)
		{
			try
			{
				if (!LicenseApi._isServiceVersionChecked)
				{
					await LicenseApi.CheckServiceVersion();
				}
				if (!LicenseApi._serviceVersionEqual)
				{
					return false;
				}
				Encoding unicode = Encoding.Unicode;
				List<byte[]> list = new List<byte[]>();
				list.Add(new byte[] { 15 });
				list.Add(unicode.GetBytes(languageId + "\0"));
				byte[] array = list.SelectMany((byte[] x) => x).ToArray<byte>();
				uint num = (uint)array.Length;
				ServiceLicensingResponseWrapper<REWASD_LICENSE_CONTROL_RESPONSE> serviceLicensingResponseWrapper = await LicenseApi.LicensingCommunicator.LicenseControl(array, num, 0U);
				if (serviceLicensingResponseWrapper.IsResponseValid && (ulong)serviceLicensingResponseWrapper.ServiceResponse.Header.Size == (ulong)((long)Marshal.SizeOf<REWASD_RESPONSE_HEADER>()))
				{
					return true;
				}
				Tracer.TraceWrite("GetLicenseInfo5 failed", false);
			}
			catch (Exception ex)
			{
				Tracer.TraceException(ex, "GetLicenseInfo5");
			}
			return false;
		}

		public static async Task Reconnect()
		{
			await LicenseApi.LicensingCommunicator.ReinitService(true);
			await LicenseApi.InitLicenseParams();
		}

		private static bool _isServiceVersionChecked;

		private static bool _serviceVersionEqual;

		private static ILicensingCommunicator _licensingCommunicator;

		private const ushort INIT_WAIT_TIMEOUT = 60000;

		private static bool _isLicenseApiInited;
	}
}
