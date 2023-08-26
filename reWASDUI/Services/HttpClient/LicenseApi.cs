using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using reWASDCommon.Network.HTTP;
using XBEliteWPF.License.Licensing.ComStructures;

namespace reWASDUI.Services.HttpClient
{
	public class LicenseApi : BaseHttpClientService
	{
		public async Task<LicenseInfo> GetLicenseInfo()
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("License/LicenseInfo"));
			LicenseInfo licenseInfo;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				licenseInfo = default(LicenseInfo);
			}
			else
			{
				licenseInfo = JsonConvert.DeserializeObject<LicenseInfo>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return licenseInfo;
		}

		public async Task<LicenseCheckResult> CheckLicense(bool forceOnlineCheck)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("License/CheckLicense/" + forceOnlineCheck.ToString()));
			LicenseCheckResult licenseCheckResult;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				licenseCheckResult = default(LicenseCheckResult);
			}
			else
			{
				licenseCheckResult = JsonConvert.DeserializeObject<LicenseCheckResult>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return licenseCheckResult;
		}

		public async Task<LicenseCheckResult> ActivateLicense(ActivationLicenseInfo licenseInfo)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(licenseInfo), "License/ActivateLicense"));
			LicenseCheckResult licenseCheckResult;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				licenseCheckResult = default(LicenseCheckResult);
			}
			else
			{
				licenseCheckResult = JsonConvert.DeserializeObject<LicenseCheckResult>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return licenseCheckResult;
		}

		public async Task<LicenseCheckResult> CheckForUpdate()
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("License/CheckForUpdate"));
			LicenseCheckResult licenseCheckResult;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				licenseCheckResult = default(LicenseCheckResult);
			}
			else
			{
				licenseCheckResult = JsonConvert.DeserializeObject<LicenseCheckResult>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return licenseCheckResult;
		}

		public async Task<LicenseCheckResult> ActivateTrialFeature(string featureId)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("License/ActivateTrialFeature/" + featureId));
			LicenseCheckResult licenseCheckResult;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				licenseCheckResult = default(LicenseCheckResult);
			}
			else
			{
				licenseCheckResult = JsonConvert.DeserializeObject<LicenseCheckResult>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return licenseCheckResult;
		}

		public async Task<bool> IsOfferExist()
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("License/IsOfferExist"));
			bool flag;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				flag = false;
			}
			else
			{
				flag = JsonConvert.DeserializeObject<bool>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return flag;
		}

		public async Task<HtmlOffer> GetOffer()
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("License/GetOffer"));
			HtmlOffer htmlOffer;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				htmlOffer = default(HtmlOffer);
			}
			else
			{
				htmlOffer = JsonConvert.DeserializeObject<HtmlOffer>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return htmlOffer;
		}

		public async Task ClearOffer()
		{
			await HttpUtils.SendRequest(() => HttpUtils.GetRequest("License/ClearOffer"));
		}
	}
}
