using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using XBEliteWPF.Infrastructure.Controller;
using XBEliteWPF.License.Licensing;
using XBEliteWPF.License.Licensing.ComStructures;

namespace reWASDEngine.Services.HttpServer
{
	[Route("v1.7/License")]
	public class LicenseController : ControllerBase
	{
		[HttpGet]
		[Route("")]
		public IActionResult Get()
		{
			return this.Ok(new LicenseInfo(Engine.LicensingService));
		}

		[HttpGet]
		[Route("Slots/{gamepadId}")]
		public IActionResult GetSlotsForDevice(string gamepadId)
		{
			BaseControllerVM baseControllerVM = Engine.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.ID == gamepadId);
			List<Slot> list = new List<Slot>();
			bool isSlotFeatureUnlocked = Engine.LicensingService.IsSlotFeatureUnlocked;
			if (baseControllerVM == null)
			{
				if (isSlotFeatureUnlocked)
				{
					list = new List<Slot> { 0, 1, 2, 3 };
				}
				else
				{
					list.Add(0);
				}
			}
			else
			{
				foreach (object obj in Enum.GetValues(typeof(Slot)))
				{
					Slot slot = (Slot)obj;
					if (Engine.GamepadService.IsSlotValid(slot, isSlotFeatureUnlocked, baseControllerVM.ControllerTypeEnums))
					{
						list.Add(slot);
					}
				}
			}
			return this.Ok(new LicenseSlotsInfo
			{
				Slots = list
			});
		}

		[HttpGet]
		[Route("LicenseInfo")]
		public async Task<IActionResult> GetLicenseInfo()
		{
			LicenseInfo licenseInfo = await LicenseApi.GetLicenseInfo();
			return this.Ok(licenseInfo);
		}

		[HttpGet]
		[Route("CheckLicense/{forceOnlineCheck}")]
		public async Task<IActionResult> CheckLicense(bool forceOnlineCheck)
		{
			LicenseCheckResult licenseCheckResult = await Engine.LicensingService.CheckLicenseAsync(forceOnlineCheck);
			return this.Ok(licenseCheckResult);
		}

		[HttpGet]
		[Route("CheckForUpdate")]
		public async Task<IActionResult> CheckForUpdate()
		{
			LicenseCheckResult licenseCheckResult = await Engine.LicensingService.CheckForUpdate();
			return this.Ok(licenseCheckResult);
		}

		[HttpGet]
		[Route("ActivateTrialFeature/{featureId}")]
		public async Task<IActionResult> ActivateTrialFeature(string featureId)
		{
			LicenseCheckResult licenseCheckResult = await Engine.LicensingService.ActivateFeatureAsync(featureId);
			return this.Ok(licenseCheckResult);
		}

		[HttpGet]
		[Route("IsOfferExist")]
		public async Task<IActionResult> IsOfferExist()
		{
			bool flag = await LicenseApi.IsOfferExist();
			return this.Ok(flag);
		}

		[HttpGet]
		[Route("GetOffer")]
		public async Task<IActionResult> GetOffer()
		{
			HtmlOffer htmlOffer = await LicenseApi.GetOffer();
			return this.Ok(htmlOffer);
		}

		[HttpGet]
		[Route("ClearOffer")]
		public async Task<IActionResult> ClearOffer()
		{
			await LicenseApi.ClearOffer();
			return this.Ok();
		}

		[HttpPost]
		[Route("ActivateLicense")]
		public async Task<IActionResult> ActivateLicense([FromBody] ActivationLicenseInfo licenseInfo)
		{
			LicenseCheckResult licenseCheckResult = await Engine.LicensingService.Activate(licenseInfo);
			return this.Ok(licenseCheckResult);
		}
	}
}
