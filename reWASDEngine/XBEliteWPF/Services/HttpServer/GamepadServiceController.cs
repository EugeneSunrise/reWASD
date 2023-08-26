using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoftReWASDServiceNamespace;
using Microsoft.AspNetCore.Mvc;
using reWASDCommon.Infrastructure;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Infrastructure.LED;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using reWASDCommon.Utils;
using reWASDEngine;
using reWASDEngine.Services.HttpServer.Data;
using XBEliteWPF.DataModels;
using XBEliteWPF.DataModels.CompositeDevicesCollection;
using XBEliteWPF.DataModels.ControllerProfileInfo;
using XBEliteWPF.DataModels.GamepadActiveProfiles;
using XBEliteWPF.DataModels.GamepadSlotHotkeyCollection;
using XBEliteWPF.DataModels.PeripheralDevicesCollection;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.Controller;
using XBEliteWPF.Infrastructure.LED;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtilModel;

namespace XBEliteWPF.Services.HttpServer
{
	[Route("v1.7/GamepadService")]
	public class GamepadServiceController : ControllerBase
	{
		[HttpGet]
		[Route("GamepadCollection")]
		public IActionResult GetGamepadCollection()
		{
			IGamepadService gamepadService = Engine.GamepadService;
			return this.Ok((gamepadService != null) ? gamepadService.GamepadCollection : null);
		}

		[HttpGet]
		[Route("GamepadProfileRelations")]
		public IActionResult GamepadProfileRelations()
		{
			return this.Ok(Engine.GamepadService.GamepadProfileRelations);
		}

		[HttpGet]
		[Route("AutoGamesDetectionGamepadProfileRelations")]
		public IActionResult AutoGamesDetectionGamepadProfileRelations()
		{
			return this.Ok(Engine.GamepadService.AutoGamesDetectionGamepadProfileRelations);
		}

		[HttpGet]
		[Route("CompositeDevicesCollection")]
		public IActionResult GetCompositeDevicesCollection()
		{
			return this.Ok(Engine.GamepadService.CompositeDevices);
		}

		[HttpGet]
		[Route("BlacklistDevices")]
		public IActionResult BlacklistDevices()
		{
			return this.Ok(Engine.GamepadService.BlacklistGamepads);
		}

		[HttpGet]
		[Route("PerDeviceGlobalLedSettings")]
		public IActionResult PerDeviceGlobalLedSettings()
		{
			return this.Ok(Engine.UserSettingsService.PerDeviceGlobalLedSettings);
		}

		[HttpGet]
		[Route("GamepadsSettings")]
		public IActionResult GamepadsSettings()
		{
			return this.Ok(Engine.GamepadService.GamepadsSettings);
		}

		[HttpGet]
		[Route("GamepadsHotkeyCollection")]
		public IActionResult GamepadsHotkeyCollection()
		{
			return this.Ok(Engine.GamepadService.GamepadsHotkeyCollection);
		}

		[HttpGet]
		[Route("GamepadsUserLedCollection")]
		public IActionResult GamepadsUserLedCollection()
		{
			return this.Ok(Engine.GamepadService.GamepadsUserLedCollection);
		}

		[HttpGet]
		[Route("PeripheralDevices")]
		public IActionResult PeripheralDevices()
		{
			return this.Ok(Engine.GamepadService.PeripheralDevices);
		}

		[HttpGet]
		[Route("GetProfilePingInfo/{profileId}")]
		public async Task<IActionResult> GetProfilePingInfo(ushort profileId)
		{
			ServiceResponseWrapper<REWASD_GET_PROFILE_STATE_RESPONSE> serviceResponseWrapper = await Engine.XBServiceCommunicator.GetProfileState(profileId, false);
			IActionResult actionResult;
			if (serviceResponseWrapper == null || !serviceResponseWrapper.IsResponseValid)
			{
				actionResult = this.BadRequest(DTLocalization.GetString(12223));
			}
			else
			{
				actionResult = this.Ok(new PingInfo(serviceResponseWrapper.ServiceResponse.MinPingTime, serviceResponseWrapper.ServiceResponse.MaxPingTime, serviceResponseWrapper.ServiceResponse.AvgPingTime, serviceResponseWrapper.ServiceResponse.NumPingPacketsSent));
			}
			return actionResult;
		}

		[HttpGet]
		[Route("ControllerStartManualHardwareGyroReCalibration/{gamepadId}/{gamepadType}")]
		public async Task<IActionResult> ControllerStartManualHardwareGyroReCalibration(ulong gamepadID, uint gamepadType)
		{
			await Engine.XBServiceCommunicator.ControllerStartManualHardwareGyroReCalibration(gamepadID, gamepadType);
			return this.Ok();
		}

		[HttpGet]
		[Route("ControllerStartManualSoftwareGyroReCalibration/{gamepadId}/{gamepadType}")]
		public async Task<IActionResult> ControllerStartManualSoftwareGyroReCalibration(ulong gamepadID, uint gamepadType)
		{
			await Engine.XBServiceCommunicator.ControllerStartManualSoftwareGyroReCalibration(gamepadID, gamepadType);
			return this.Ok();
		}

		[HttpGet]
		[Route("ControllerUpdateGyroCalibrationMode/{gamepadId}/{gamepadType}")]
		public async Task<IActionResult> ControllerUpdateGyroCalibrationMode(ulong gamepadID, uint gamepadType)
		{
			await Engine.XBServiceCommunicator.ControllerUpdateGyroCalibrationMode(gamepadID, gamepadType);
			return this.Ok();
		}

		[HttpGet]
		[Route("ProcessExclusiveCaptureProfile/{gamepadId}/{add}/{delete}")]
		public async Task<IActionResult> ProcessExclusiveCaptureProfile(string gamepadID, bool add, bool delete)
		{
			await Engine.GamepadService.ProcessExclusiveCaptureProfile(gamepadID, add, delete);
			return this.Ok();
		}

		[HttpGet]
		[Route("DeleteSpecialProfiles")]
		public async Task<IActionResult> DeleteSpecialProfiles()
		{
			await Engine.GamepadService.DeleteSpecialProfiles();
			return this.Ok();
		}

		[HttpGet]
		[Route("SendControllerVibration/{gamepadId}/{gamepadType}")]
		public IActionResult SendControllerVibration(ulong gamepadID, uint gamepadType)
		{
			Engine.XBServiceCommunicator.SendControllerVibration(gamepadID, gamepadType, 300, 50, 0);
			return this.Ok();
		}

		[HttpGet]
		[Route("SwitchControllerToHidMode/{gamepadId}/{gamepadType}")]
		public IActionResult SwitchControllerToHidMode(ulong gamepadID, uint gamepadType)
		{
			Engine.XBServiceCommunicator.SwitchControllerToHidMode(gamepadID, gamepadType);
			return this.Ok();
		}

		[HttpGet]
		[Route("DeleteAllExclusiveCaptureProfiles")]
		public IActionResult DeleteAllExclusiveCaptureProfiles()
		{
			Engine.GamepadService.DeleteAllExclusiveCaptureProfiles();
			return this.Ok();
		}

		[HttpGet]
		[Route("ControllerChangeMasterAddress/{gamepadId}/{gamepadType}/{bluetoothAddress}")]
		public async Task<IActionResult> ControllerChangeMasterAddress(ulong gamepadID, uint gamepadType, ulong bluetoothAddress)
		{
			await Engine.GamepadService.ControllerChangeMasterAddress(gamepadID, gamepadType, bluetoothAddress);
			return this.Ok();
		}

		[HttpGet]
		[Route("GetExclusiveCaptureControllersInfo")]
		public IActionResult GetExclusiveCaptureControllersInfo()
		{
			return this.Ok(new ExclusiveCaptureControllersInfo
			{
				IsExclusiveCaptureControllersPresent = Engine.GamepadService.IsExclusiveCaptureControllersPresent,
				IsExclusiveCaptureProfilePresent = Engine.GamepadService.IsExclusiveCaptureProfilePresent
			});
		}

		[HttpGet]
		[Route("ReCompileSteamLizardProfile")]
		public async Task<IActionResult> ReCompileSteamLizardProfile()
		{
			await Engine.GamepadService.ReCompileSteamLizardProfile();
			return this.Ok();
		}

		[HttpGet]
		[Route("RefreshInputDevices")]
		public async Task<IActionResult> RefreshInputDevices()
		{
			await Engine.GamepadService.RefreshInputDevices();
			return this.Ok();
		}

		[HttpGet]
		[Route("ReapplayRemap")]
		public IActionResult ReapplayRemap()
		{
			ThreadHelper.ExecuteInMainDispatcher(async delegate
			{
				Engine.GamepadService.CachedProfilesCollection.Clear();
				await Engine.GamepadService.EnableRemap(false, null, false, false, true, -1, false, true, true, null, null);
				await Engine.GamepadService.RefreshHiddenAppliedConfig();
			}, false);
			return this.Ok();
		}

		[HttpGet]
		[Route("DisableRemapByExternalClient/{macAddress}")]
		public async Task<IActionResult> DisableRemapByExternalClient(ulong macAddress)
		{
			List<REWASD_CONTROLLER_PROFILE> list = Engine.GamepadService.ServiceProfilesCollection.FindServiceProfilesByExternalMacAddress(macAddress);
			foreach (REWASD_CONTROLLER_PROFILE rewasd_CONTROLLER_PROFILE in list)
			{
				string text = XBUtils.CalculateID(rewasd_CONTROLLER_PROFILE.Id);
				await Engine.GamepadService.RestoreDefaults(text, new List<Slot> { REWASD_CONTROLLER_PROFILE_Extensions.GetSlot(rewasd_CONTROLLER_PROFILE) });
			}
			List<REWASD_CONTROLLER_PROFILE>.Enumerator enumerator = default(List<REWASD_CONTROLLER_PROFILE>.Enumerator);
			return this.Ok();
		}

		[HttpPost]
		[Route("SaveBlacklistDevices")]
		public async Task<IActionResult> SaveBlacklistDevices([FromBody] ObservableCollection<BlackListGamepad> gamepads)
		{
			Engine.GamepadService.BlacklistGamepads = gamepads;
			bool result = Engine.GamepadService.BinDataSerialize.SaveBlacklistDevices();
			await Engine.GamepadService.RefreshGamepadCollection(0UL);
			Engine.EventAggregator.GetEvent<GamepadsBlacklistChanged>().Publish(null);
			return this.Ok(result);
		}

		[HttpPost]
		[Route("SaveGamepadsSettings")]
		public IActionResult SaveGamepadsSettings([FromBody] ObservableCollection<GamepadSettings> gamepadsSettings)
		{
			Engine.GamepadService.GamepadsSettings = gamepadsSettings;
			return this.Ok(Engine.GamepadService.BinDataSerialize.SaveGamepadsSettings());
		}

		[HttpPost]
		[Route("SaveGamepadsHotkeyCollection")]
		public IActionResult SaveGamepadsHotkeyCollection([FromBody] GamepadsHotkeyDictionary gamepadsHotkeyDictionary)
		{
			Engine.GamepadService.GamepadsHotkeyCollection = gamepadsHotkeyDictionary;
			return this.Ok(Engine.GamepadService.BinDataSerialize.SaveGamepadsHotkeyCollection());
		}

		[HttpPost]
		[Route("ApplyHoneypotProfile")]
		public async Task<IActionResult> ApplyHoneypotProfile([FromBody] SpecialProfileInfo specialProfileInfo)
		{
			bool flag = await Engine.GamepadService.ApplyHoneypotProfile(specialProfileInfo);
			return this.Ok(flag);
		}

		[HttpPost]
		[Route("ApplyHardwareDonglePingProfile")]
		public async Task<IActionResult> ApplyHardwareDonglePingProfile([FromBody] SpecialProfileInfo specialProfileInfo)
		{
			ushort num = await Engine.GamepadService.ApplyHardwareDonglePingProfile(specialProfileInfo);
			return this.Ok(num);
		}

		[HttpPost]
		[Route("GetAppliedConfig")]
		public IActionResult GetAppliedConfig([FromBody] SelectSlotInfo slotInfo)
		{
			GamepadProfiles gamepadProfiles = Engine.GamepadService.GamepadProfileRelations.TryGetValue(slotInfo.ID);
			if (gamepadProfiles != null)
			{
				SlotProfilesDictionary slotProfiles = gamepadProfiles.SlotProfiles;
				GamepadProfile gamepadProfile = ((slotProfiles != null) ? slotProfiles.TryGetValue(slotInfo.Slot) : null);
				if (gamepadProfile != null)
				{
					if (gamepadProfile.Config != null)
					{
						return this.Ok(new ConfigInfo(gamepadProfile.Config.GameName, gamepadProfile.Config.Name, gamepadProfile.Config.ConfigPath, gamepadProfile.Config.IsAutodetectEnabledForAnySlot));
					}
				}
				else
				{
					REWASD_CONTROLLER_PROFILE_EX rewasd_CONTROLLER_PROFILE_EX = Engine.GamepadService.ServiceProfilesCollection.FindByID(slotInfo.ID);
					if (rewasd_CONTROLLER_PROFILE_EX != null && REWASD_CONTROLLER_PROFILE_Extensions.IsSlotWithUserConfig(rewasd_CONTROLLER_PROFILE_EX.Profiles[slotInfo.Slot]))
					{
						return this.NotFound(null);
					}
				}
			}
			return this.Ok();
		}

		[HttpPost]
		[Route("CurrentSlot")]
		public async Task<IActionResult> GetCurrentSlot([FromBody] ControllerIDInfo controllerID)
		{
			BaseControllerVM baseControllerVM = Engine.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.ID.Contains(controllerID.ID));
			IActionResult actionResult;
			if (baseControllerVM == null)
			{
				actionResult = this.BadRequest(DTLocalization.GetString(12223));
			}
			else
			{
				Slot slot = await baseControllerVM.GetCurrentSlot();
				actionResult = this.Ok(slot);
			}
			return actionResult;
		}

		[HttpPost]
		[Route("RemoveOfflineGamepad")]
		public async Task<IActionResult> RemoveOfflineGamepad([FromBody] ControllerIDInfo controllerID)
		{
			GamepadServiceController.<>c__DisplayClass32_0 CS$<>8__locals1 = new GamepadServiceController.<>c__DisplayClass32_0();
			CS$<>8__locals1.controllerID = controllerID;
			CS$<>8__locals1.<>4__this = this;
			BaseControllerVM gamepad = Engine.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.ID.Contains(CS$<>8__locals1.controllerID.ID));
			IActionResult actionResult;
			if (gamepad == null)
			{
				List<string> internalIdsForEngineController = UtilsCommon.GetInternalIdsForEngineController(CS$<>8__locals1.controllerID.ID);
				if (internalIdsForEngineController.Count >= 1)
				{
					if (internalIdsForEngineController.Count > 1)
					{
						foreach (CompositeDevice compositeDevice in from internalId in internalIdsForEngineController
							select Engine.GamepadService.CompositeDevices.FindCompositeForSimple(internalId) into composite
							where composite != null
							select composite)
						{
							Engine.GamepadService.CompositeDevices.Remove(compositeDevice);
						}
					}
					internalIdsForEngineController.ForEach(delegate(string id)
					{
						GamepadServiceController.<>c__DisplayClass32_0.<<RemoveOfflineGamepad>b__1>d <<RemoveOfflineGamepad>b__1>d;
						<<RemoveOfflineGamepad>b__1>d.<>t__builder = AsyncVoidMethodBuilder.Create();
						<<RemoveOfflineGamepad>b__1>d.<>4__this = CS$<>8__locals1;
						<<RemoveOfflineGamepad>b__1>d.id = id;
						<<RemoveOfflineGamepad>b__1>d.<>1__state = -1;
						<<RemoveOfflineGamepad>b__1>d.<>t__builder.Start<GamepadServiceController.<>c__DisplayClass32_0.<<RemoveOfflineGamepad>b__1>d>(ref <<RemoveOfflineGamepad>b__1>d);
					});
					actionResult = this.Ok();
				}
				else
				{
					actionResult = this.BadRequest(DTLocalization.GetString(12223));
				}
			}
			else
			{
				await Engine.GamepadService.RestoreDefaults(gamepad.ID, new List<Slot> { 0, 1, 2, 3 });
				if (!(gamepad is CompositeControllerVM))
				{
					Engine.GamepadService.AllPhysicalControllers.Remove(gamepad);
				}
				Engine.GamepadService.GamepadCollection.Remove(gamepad);
				Engine.GamepadService.BinDataSerialize.SaveGamepadCollection();
				actionResult = this.Ok();
			}
			return actionResult;
		}

		[HttpPost]
		[Route("SaveCompositeDevicesCollection")]
		public async Task<IActionResult> SaveCompositeDevicesCollection([FromBody] CompositeDevices compositeDevices)
		{
			Engine.GamepadService.CompositeDevices = compositeDevices;
			bool flag = await Engine.GamepadService.BinDataSerialize.SaveCompositeDevicesCollection();
			return this.Ok(flag);
		}

		[HttpPost]
		[Route("SaveAutoGamesDetectionGamepadProfileRelations")]
		public IActionResult SaveAutoGamesDetectionGamepadProfileRelations([FromBody] AutoGamesDetectionGamepadProfilesCollection autoDetectProfiles)
		{
			Engine.GamepadService.AutoGamesDetectionGamepadProfileRelations = autoDetectProfiles;
			return this.Ok(Engine.GamepadService.BinDataSerialize.SaveAutoGamesDetectionGamepadProfileRelations());
		}

		[HttpPost]
		[Route("SavePerDeviceGlobalLedSettings")]
		public async Task<IActionResult> SavePerDeviceGlobalLedSettings([FromBody] LEDSettingsGlobal ledSettings)
		{
			Engine.UserSettingsService.PerDeviceGlobalLedSettings = ledSettings;
			bool flag = await Engine.UserSettingsService.PerDeviceGlobalLedSettings.Save();
			return this.Ok(flag);
		}

		[HttpPost]
		[Route("SaveGamepadsUserLedCollection")]
		public IActionResult SaveGamepadsUserLedCollection([FromBody] GamepadsPlayerLedDictionary dic)
		{
			Engine.GamepadService.GamepadsUserLedCollection = dic;
			return this.Ok(Engine.GamepadService.BinDataSerialize.SaveGamepadsUserLedCollection());
		}

		[HttpPost]
		[Route("Reinitialize")]
		public async Task<IActionResult> RemovePeripheralDevices([FromBody] ControllerIDInfo controllerID)
		{
			await Engine.GamepadService.ReinitializeDevice(controllerID.ID);
			return this.Ok();
		}

		[HttpPost]
		[Route("InitializePeripheralDevice")]
		public IActionResult InitializePeripheralDevice([FromBody] InitializeDeviceInfo deviceInfo)
		{
			BaseControllerVM baseControllerVM = Engine.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.ID == deviceInfo.ID);
			if (baseControllerVM == null)
			{
				return this.BadRequest(DTLocalization.GetString(12223));
			}
			if (Engine.GamepadService.PeripheralDevices.FirstOrDefault((PeripheralDevice fpd) => fpd.ID == deviceInfo.ID) != null)
			{
				return this.BadRequest();
			}
			PeripheralVM peripheralVM = baseControllerVM as PeripheralVM;
			if (peripheralVM == null)
			{
				return this.BadRequest(DTLocalization.GetString(12223));
			}
			PeripheralPhysicalType peripheralPhysicalType = (PeripheralPhysicalType)Enum.Parse(typeof(PeripheralPhysicalType), deviceInfo.DeviceType);
			Engine.GamepadService.InitializePeripheralDevice(peripheralVM, peripheralPhysicalType);
			return this.Ok();
		}

		[HttpPost]
		[Route("InitializeDevice")]
		public IActionResult InitializeDevice([FromBody] InitializeDeviceInfo deviceInfo)
		{
			BaseControllerVM baseControllerVM = Engine.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.ID == deviceInfo.ID);
			if (baseControllerVM == null)
			{
				return this.BadRequest(DTLocalization.GetString(12223));
			}
			if (Engine.GamepadService.PeripheralDevices.FirstOrDefault((PeripheralDevice fpd) => fpd.ID == deviceInfo.ID) != null)
			{
				return this.BadRequest();
			}
			Engine.GamepadService.InitializeDevice(baseControllerVM, deviceInfo.DeviceType);
			return this.Ok();
		}

		[HttpPost]
		[Route("SteamDeckSetMotorIntensitySettings")]
		public async Task<IActionResult> SteamDeckSetMotorIntensitySettings([FromBody] SteamDeckMotorIntensitySettings intensitySettings)
		{
			ControllerVM controllerVM = Engine.GamepadService.AllPhysicalControllers.FirstOrDefault(delegate(BaseControllerVM item)
			{
				ControllerVM controllerVM2 = item as ControllerVM;
				return controllerVM2 != null && controllerVM2.ControllerType == 63;
			}) as ControllerVM;
			IActionResult actionResult;
			if (controllerVM == null)
			{
				actionResult = this.BadRequest(DTLocalization.GetString(12223));
			}
			else
			{
				bool flag = await Engine.XBServiceCommunicator.SteamDeckSetMotorIntensitySettings(controllerVM.ControllerId, controllerVM.Type, intensitySettings.LeftIsStrongIntensity, intensitySettings.LeftIntensity, intensitySettings.RightIsStrongIntensity, intensitySettings.RightIntensity);
				actionResult = this.Ok(new BaseResponse
				{
					Result = flag
				});
			}
			return actionResult;
		}

		[HttpPost]
		[Route("RemapState")]
		public IActionResult GetRemapState([FromBody] ControllerIDInfo controllerID)
		{
			return this.Ok(new GamepadRemapState(Engine.GamepadService.GetRemapState(controllerID.ID)));
		}

		[HttpPost]
		[Route("VirtualGamepadType")]
		public IActionResult GetVirtualGamepadType([FromBody] ControllerIDInfo controllerID)
		{
			BaseControllerVM baseControllerVM = Engine.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.ID == controllerID.ID);
			if (baseControllerVM == null)
			{
				return this.BadRequest(DTLocalization.GetString(12223));
			}
			return this.Ok(Engine.GamepadService.GetVirtualGamepadType(baseControllerVM));
		}

		[HttpPost]
		[Route("IsOnlyBluetoothConnection")]
		public IActionResult IsOnlyBluetoothConnection([FromBody] ControllerIDInfo controllerID)
		{
			bool flag = Engine.GamepadService.SimpleDeviceInfoList.Exists((Tuple<ulong, SimpleDeviceInfo> x) => x.Item2.Id.ToString() == controllerID.ID && x.Item2.BluetoothConnection) && !Engine.GamepadService.SimpleDeviceInfoList.Exists((Tuple<ulong, SimpleDeviceInfo> x) => x.Item2.Id.ToString() == controllerID.ID && !x.Item2.BluetoothConnection);
			return this.Ok(flag);
		}

		[HttpPost]
		[Route("IsControllerPressedButton")]
		public async Task<IActionResult> IsControllerPressedButton([FromBody] ControllerIDInfo controllerID)
		{
			BaseControllerVM baseControllerVM = Engine.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.ID == controllerID.ID);
			IActionResult actionResult;
			if (baseControllerVM == null)
			{
				actionResult = this.BadRequest(DTLocalization.GetString(12223));
			}
			else if (!baseControllerVM.IsOnline)
			{
				actionResult = this.Ok(false);
			}
			else
			{
				bool flag = await baseControllerVM.GetIsControllerPressedButton();
				actionResult = this.Ok(flag);
			}
			return actionResult;
		}

		[HttpPost]
		[Route("GetControllerPressedButtons")]
		public async Task<IActionResult> GetControllerPressedButtons([FromBody] ControllerIDInfo controllerID)
		{
			BaseControllerVM baseControllerVM = Engine.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.ID == controllerID.ID);
			IActionResult actionResult;
			if (baseControllerVM == null)
			{
				actionResult = this.BadRequest(DTLocalization.GetString(12223));
			}
			else
			{
				GamepadState gamepadState = await baseControllerVM.GetControllerPressedButtons();
				actionResult = this.Ok(gamepadState);
			}
			return actionResult;
		}

		[HttpPost]
		[Route("ApplyAmiibo")]
		public async Task<IActionResult> ApplyAmiibo([FromBody] ApplyAmiiboInfo amiiboInfo)
		{
			BaseControllerVM gamepad = Engine.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.ID == amiiboInfo.GamepadId);
			IActionResult actionResult;
			if (gamepad == null)
			{
				actionResult = this.BadRequest(DTLocalization.GetString(12223));
			}
			else
			{
				ApplyAmiiboInfo amiiboInfo2 = amiiboInfo;
				bool flag;
				if (amiiboInfo2 == null)
				{
					flag = false;
				}
				else
				{
					byte[] amiiboData = amiiboInfo2.AmiiboData;
					int? num = ((amiiboData != null) ? new int?(amiiboData.Length) : null);
					int num2 = 0;
					flag = (num.GetValueOrDefault() > num2) & (num != null);
				}
				if (flag && Engine.GamepadService.ServiceProfilesCollection.ContainsProfileForID(amiiboInfo.GamepadId))
				{
					REWASD_CONTROLLER_PROFILE_EX pEx = Engine.GamepadService.ServiceProfilesCollection.FindByID(amiiboInfo.GamepadId);
					if (pEx.Profiles[amiiboInfo.Slot].VirtualType == 48U && pEx.Enabled[amiiboInfo.Slot])
					{
						REWASD_SET_PROFILE_INFO[] array = new REWASD_SET_PROFILE_INFO[] { REWASD_SET_PROFILE_INFO.CreateBlankInstance() };
						array[0].ProfileId = pEx.ServiceProfileIds[amiiboInfo.Slot];
						REWASD_SET_PROFILE_INFO[] array2 = array;
						int num3 = 0;
						array2[num3].Flags = array2[num3].Flags | 1U;
						REWASD_SET_PROFILE_INFO[] array3 = array;
						int num4 = 0;
						array3[num4].Flags = array3[num4].Flags | 256U;
						if (amiiboInfo.AmiiboData.Length != 0 && amiiboInfo.AmiiboData.Length <= array[0].Amiibo.Length)
						{
							Buffer.BlockCopy(amiiboInfo.AmiiboData, 0, array[0].Amiibo, 0, amiiboInfo.AmiiboData.Length);
						}
						string text = "ApplyAmiibo: ";
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(51, 3);
						defaultInterpolatedStringHandler.AppendLiteral("ProfileId 0x");
						defaultInterpolatedStringHandler.AppendFormatted<ushort>(array[0].ProfileId, "X");
						defaultInterpolatedStringHandler.AppendLiteral(". GamepadId ");
						defaultInterpolatedStringHandler.AppendFormatted(amiiboInfo.GamepadId);
						defaultInterpolatedStringHandler.AppendLiteral(". AmiiboData Length ");
						defaultInterpolatedStringHandler.AppendFormatted<int>(amiiboInfo.AmiiboData.Length);
						defaultInterpolatedStringHandler.AppendLiteral(" bytes.");
						Tracer.TraceWrite(text + defaultInterpolatedStringHandler.ToStringAndClear(), false);
						if (amiiboInfo.AmiiboData.Length >= 8)
						{
							string text2 = "ApplyAmiibo: Amiibo UID ";
							defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 3);
							defaultInterpolatedStringHandler.AppendFormatted<byte>(amiiboInfo.AmiiboData[0], "X2");
							defaultInterpolatedStringHandler.AppendLiteral("-");
							defaultInterpolatedStringHandler.AppendFormatted<byte>(amiiboInfo.AmiiboData[1], "X2");
							defaultInterpolatedStringHandler.AppendLiteral("-");
							defaultInterpolatedStringHandler.AppendFormatted<byte>(amiiboInfo.AmiiboData[2], "X2");
							defaultInterpolatedStringHandler.AppendLiteral("-");
							string text3 = defaultInterpolatedStringHandler.ToStringAndClear();
							defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 4);
							defaultInterpolatedStringHandler.AppendFormatted<byte>(amiiboInfo.AmiiboData[4], "X2");
							defaultInterpolatedStringHandler.AppendLiteral("-");
							defaultInterpolatedStringHandler.AppendFormatted<byte>(amiiboInfo.AmiiboData[5], "X2");
							defaultInterpolatedStringHandler.AppendLiteral("-");
							defaultInterpolatedStringHandler.AppendFormatted<byte>(amiiboInfo.AmiiboData[6], "X2");
							defaultInterpolatedStringHandler.AppendLiteral("-");
							defaultInterpolatedStringHandler.AppendFormatted<byte>(amiiboInfo.AmiiboData[7], "X2");
							Tracer.TraceWrite(text2 + text3 + defaultInterpolatedStringHandler.ToStringAndClear(), false);
						}
						ServiceResponseWrapper<REWASD_SET_PROFILE_STATE_RESPONSE> serviceResponseWrapper = await Engine.XBServiceCommunicator.SetProfileState(array, false);
						if (!serviceResponseWrapper.IsResponseValid)
						{
							return this.BadRequest();
						}
						if (serviceResponseWrapper.ServiceResponse.Header.Status == 87U)
						{
							Tracer.TraceWrite("ApplyAmiibo: Error! This file is a not a valid backup. Please try another one.", false);
							return this.BadRequest(DTLocalization.GetString(12481));
						}
						if (serviceResponseWrapper.ServiceResponse.Header.Status == 170U)
						{
							Tracer.TraceWrite("ApplyAmiibo: Error! The console is reading the previous Amiibo. Please wait a bit and try again.", false);
							return this.BadRequest(DTLocalization.GetString(12495));
						}
						pEx.Amiibo[amiiboInfo.Slot].Load(amiiboInfo.AmiiboData);
						Engine.GamepadService.SendGamepadChanged(gamepad);
						Tracer.TraceWrite("ApplyAmiibo: Amiibo Loaded", false);
					}
					pEx = null;
				}
				actionResult = this.Ok();
			}
			return actionResult;
		}

		[HttpPost]
		[Route("AppliedConfig")]
		public async Task<IActionResult> ApplyConfig([FromBody] ConfigApplyInfo configInfo)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(configInfo.Path) || !System.IO.File.Exists(configInfo.Path))
				{
					return this.BadRequest(DTLocalization.GetString(11853));
				}
			}
			catch (Exception)
			{
			}
			GamepadProfile profile = new GamepadProfile(configInfo.Path);
			IActionResult actionResult;
			if (!profile.FindAndSetProfile())
			{
				actionResult = this.BadRequest(DTLocalization.GetString(11853));
			}
			else
			{
				profile.Config.ReadConfigFromJsonIfNotLoaded(true);
				if (profile.Config.IsEmpty)
				{
					actionResult = this.BadRequest(DTLocalization.GetString(12209));
				}
				else
				{
					EnableRemapResponse response = new EnableRemapResponse();
					bool isSlotFeatureUnlocked = Engine.LicensingService.IsSlotFeatureUnlocked;
					if (string.IsNullOrWhiteSpace(configInfo.GamepadId))
					{
						IGamepadService gamepadService = Engine.GamepadService;
						if (((gamepadService != null) ? gamepadService.GamepadCollection : null) != null)
						{
							bool someSlotAvailable = false;
							bool anyGamepadConnected = false;
							foreach (BaseControllerVM baseControllerVM in Engine.GamepadService.GamepadCollection)
							{
								if (baseControllerVM.HasGamepadControllers)
								{
									anyGamepadConnected = true;
									IGamepadService gamepadService2 = Engine.GamepadService;
									if (gamepadService2 != null && gamepadService2.IsSlotValid(configInfo.Slot, isSlotFeatureUnlocked, baseControllerVM.ControllerTypeEnums))
									{
										someSlotAvailable = true;
										await this.ApplyConfig(baseControllerVM, profile, configInfo.Slot, configInfo.Bundle, response);
										Thread.Sleep(1000);
									}
								}
							}
							IEnumerator<BaseControllerVM> enumerator = null;
							if (anyGamepadConnected && !someSlotAvailable)
							{
								return this.BadRequest(DTLocalization.GetString(12227));
							}
						}
					}
					else
					{
						BaseControllerVM baseControllerVM2 = Engine.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.ID == configInfo.GamepadId);
						if (baseControllerVM2 == null)
						{
							return this.BadRequest(DTLocalization.GetString(12223));
						}
						if (!Engine.GamepadService.IsSlotValid(configInfo.Slot, isSlotFeatureUnlocked, baseControllerVM2.ControllerTypeEnums))
						{
							return this.BadRequest(DTLocalization.GetString(12227));
						}
						await this.ApplyConfig(baseControllerVM2, profile, configInfo.Slot, configInfo.Bundle, response);
						GamepadProfile gamepadProfile = profile;
						bool flag;
						if (gamepadProfile == null)
						{
							flag = null != null;
						}
						else
						{
							Config config = gamepadProfile.Config;
							flag = ((config != null) ? config.ConfigData : null) != null;
						}
						if (flag)
						{
							SenderGoogleAnalytics.SendMessageEvent("GUI", "ApplyShiftLayer", (profile.Config.ConfigData.LayersCount - 1).ToString(), -1L, false);
						}
					}
					actionResult = this.Ok(response);
				}
			}
			return actionResult;
		}

		[HttpPost]
		[Route("SelectSlot")]
		public async Task<IActionResult> SelectSlot([FromBody] SelectSlotInfo slotInfo)
		{
			BaseControllerVM baseControllerVM = Engine.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.ID == slotInfo.ID);
			IActionResult actionResult;
			if (baseControllerVM == null)
			{
				actionResult = this.BadRequest(DTLocalization.GetString(12223));
			}
			else
			{
				bool isSlotFeatureUnlocked = Engine.LicensingService.IsSlotFeatureUnlocked;
				if (!Engine.GamepadService.IsSlotValid(slotInfo.Slot, isSlotFeatureUnlocked, baseControllerVM.ControllerTypeEnums))
				{
					actionResult = this.BadRequest(DTLocalization.GetString(12227));
				}
				else
				{
					TaskAwaiter<bool> taskAwaiter = Engine.GamepadService.SwitchProfileToSlot(baseControllerVM, slotInfo.Slot).GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						TaskAwaiter<bool> taskAwaiter2;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<bool>);
					}
					if (taskAwaiter.GetResult())
					{
						actionResult = this.Ok();
					}
					else
					{
						actionResult = this.BadRequest();
					}
				}
			}
			return actionResult;
		}

		[HttpPost]
		[Route("ClearSlot")]
		public async Task<IActionResult> ClearSlot([FromBody] ClearSlotInfo slotInfo)
		{
			if (string.IsNullOrWhiteSpace(slotInfo.ID))
			{
				IGamepadService gamepadService = Engine.GamepadService;
				if (((gamepadService != null) ? gamepadService.GamepadCollection : null) != null)
				{
					foreach (BaseControllerVM baseControllerVM in Engine.GamepadService.GamepadCollection)
					{
						await Engine.GamepadService.RestoreDefaults(baseControllerVM.ID, slotInfo.Slots);
						Thread.Sleep(1000);
					}
					IEnumerator<BaseControllerVM> enumerator = null;
				}
			}
			else
			{
				IGamepadService gamepadService2 = Engine.GamepadService;
				bool flag;
				if (gamepadService2 == null)
				{
					flag = null != null;
				}
				else
				{
					ObservableCollection<BaseControllerVM> gamepadCollection = gamepadService2.GamepadCollection;
					flag = ((gamepadCollection != null) ? gamepadCollection.FirstOrDefault((BaseControllerVM g) => g.ID == slotInfo.ID) : null) != null;
				}
				if (!flag)
				{
					return this.BadRequest(DTLocalization.GetString(12223));
				}
				await Engine.GamepadService.RestoreDefaults(slotInfo.ID, slotInfo.Slots);
			}
			return this.Ok();
		}

		[HttpPost]
		[Route("EnableRemap")]
		public async Task<IActionResult> EnableRemap([FromBody] EnableRemapInfo remapInfo)
		{
			IActionResult actionResult;
			if (remapInfo == null)
			{
				actionResult = this.BadRequest();
			}
			else
			{
				bool flag = false;
				bool flag2 = false;
				this.CheckRemapAvailability(remapInfo.ID, ref flag, ref flag2);
				if (!flag)
				{
					actionResult = this.BadRequest(DTLocalization.GetString(12223));
				}
				else if (!flag2)
				{
					actionResult = this.BadRequest(DTLocalization.GetString(11604));
				}
				else
				{
					bool remapNoToggled = remapInfo.RemapNoToggled;
					EnableRemapResponse response = new EnableRemapResponse();
					EnableRemapBundle enableRemapBundle = remapInfo.Bundle;
					if (enableRemapBundle == null)
					{
						enableRemapBundle = new EnableRemapBundle();
					}
					BaseControllerVM baseControllerVM = null;
					if (remapInfo.ID != null)
					{
						baseControllerVM = Engine.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.ID.Contains(remapInfo.ID));
					}
					int num = ((baseControllerVM != null) ? baseControllerVM.CurrentSlot : (-1));
					await Engine.GamepadService.EnableRemap(false, remapInfo.ID, remapNoToggled, false, true, num, false, false, true, enableRemapBundle, response);
					actionResult = this.Ok(response);
				}
			}
			return actionResult;
		}

		[HttpPost]
		[Route("DisableRemap")]
		public async Task<IActionResult> DisableRemap([FromBody] DisableRemapInfo remapInfo)
		{
			IActionResult actionResult;
			if (remapInfo == null)
			{
				actionResult = this.BadRequest();
			}
			else
			{
				bool flag = false;
				bool flag2 = false;
				this.CheckRemapAvailability(remapInfo.ID, ref flag, ref flag2);
				if (!flag)
				{
					actionResult = this.BadRequest(DTLocalization.GetString(12223));
				}
				else if (!flag2)
				{
					actionResult = this.BadRequest(DTLocalization.GetString(11604));
				}
				else
				{
					await Engine.GamepadService.DisableRemap(remapInfo.ID, true);
					actionResult = this.Ok();
				}
			}
			return actionResult;
		}

		[HttpPost]
		[Route("SetControllerFriendlyName")]
		public IActionResult SetControllerFriendlyName([FromBody] ControllerFriendlyNameInfo info)
		{
			BaseControllerVM baseControllerVM = Engine.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.ID == info.ControllerId);
			if (baseControllerVM == null)
			{
				return this.BadRequest(DTLocalization.GetString(12223));
			}
			if (baseControllerVM.ControllerFriendlyName != info.FriendlyName)
			{
				baseControllerVM.ControllerFriendlyName = info.FriendlyName;
				if (string.IsNullOrWhiteSpace(info.FriendlyName))
				{
					baseControllerVM.IsCustomControllerFriendlyName = false;
				}
				else
				{
					baseControllerVM.IsCustomControllerFriendlyName = true;
				}
				Engine.GamepadService.UpdateDeviceFriendlyName(info.ControllerId);
				Engine.EventAggregator.GetEvent<DeviceRenamed>().Publish(info.ControllerId);
				Engine.GamepadService.BinDataSerialize.SaveGamepadCollection();
			}
			return this.Ok();
		}

		private void CheckRemapAvailability(string id, ref bool deviceFound, ref bool enableRemapAvailable)
		{
			deviceFound = false;
			enableRemapAvailable = false;
			List<BaseControllerVM> list = new List<BaseControllerVM>();
			if (!string.IsNullOrWhiteSpace(id))
			{
				BaseControllerVM baseControllerVM = Engine.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.ID == id);
				if (baseControllerVM == null)
				{
					return;
				}
				list.Add(baseControllerVM);
			}
			else
			{
				if (Engine.GamepadService.GamepadCollection.Count == 0)
				{
					return;
				}
				list = Engine.GamepadService.GamepadCollection.ToList<BaseControllerVM>();
			}
			deviceFound = true;
			foreach (BaseControllerVM baseControllerVM2 in list)
			{
				if (!Engine.GamepadService.IsGamepadInNothingAppliedState(baseControllerVM2))
				{
					enableRemapAvailable = true;
					break;
				}
			}
		}

		private async Task<ushort> ApplyConfig(BaseControllerVM gamepad, GamepadProfile profile, Slot slot, EnableRemapBundle bundle, EnableRemapResponse response)
		{
			if (Engine.GamepadService.GamepadProfileRelations.ContainsKey(gamepad.ID))
			{
				Engine.GamepadService.GamepadProfileRelations[gamepad.ID].SlotProfiles[slot] = profile;
			}
			else
			{
				GamepadProfiles gamepadProfiles = new GamepadProfiles(gamepad);
				gamepadProfiles.SlotProfiles[slot] = profile;
				gamepadProfiles.SlotProfiles[slot].FindAndSetProfile();
				Engine.GamepadService.GamepadProfileRelations.Add(gamepad.ID, gamepadProfiles);
			}
			if (bundle == null)
			{
				bundle = new EnableRemapBundle();
			}
			Engine.GamepadService.CachedProfilesCollection.Clear();
			return await Engine.GamepadService.EnableRemap(false, gamepad.ID, false, false, true, slot, false, false, true, bundle, response);
		}

		[HttpPost]
		[Route("AddEngineController")]
		public async Task<IActionResult> AddEngineController([FromBody] AddEngineControllerInfo info)
		{
			GamepadServiceController.<>c__DisplayClass55_0 CS$<>8__locals1 = new GamepadServiceController.<>c__DisplayClass55_0();
			CS$<>8__locals1.info = info;
			try
			{
				List<Tuple<ulong, ControllerTypeEnum>> list;
				if (UtilsCommon.IsEnginePeripheralsShouldBeComposited(CS$<>8__locals1.info.Id, CS$<>8__locals1.info.EngineControllerTypes, ref list))
				{
					if (list.Count > 1)
					{
						CompositeDevice compositeDevice = new CompositeDevice();
						Lazy<IGamepadService> gamepadServiceLazy = Engine.GamepadServiceLazy;
						if (gamepadServiceLazy != null)
						{
							IGamepadService value = gamepadServiceLazy.Value;
							if (value != null)
							{
								value.CompositeDevices.Add(compositeDevice);
							}
						}
						int num = 0;
						foreach (Tuple<ulong, ControllerTypeEnum> tuple in list)
						{
							Guid guid = Guid.Empty;
							PeripheralPhysicalType peripheralPhysicalType = 0;
							bool flag = ControllerTypeExtensions.IsEngineKeyboard(tuple.Item2);
							bool flag2 = ControllerTypeExtensions.IsAnyEngineMouse(tuple.Item2);
							if (flag)
							{
								peripheralPhysicalType = 1;
								if (!RegistryHelper.ValueExists("Controllers\\" + CS$<>8__locals1.info.Id.ToString(), "EngineKeyboardGuid"))
								{
									guid = Guid.NewGuid();
									RegistryHelper.SetString("Controllers\\" + CS$<>8__locals1.info.Id.ToString(), "EngineKeyboardGuid", guid.ToString());
								}
								else
								{
									string @string = RegistryHelper.GetString("Controllers\\" + CS$<>8__locals1.info.Id.ToString(), "EngineKeyboardGuid", Guid.Empty.ToString(), false);
									if (!string.IsNullOrEmpty(@string))
									{
										guid = Guid.Parse(@string);
									}
									else
									{
										guid = Guid.NewGuid();
										RegistryHelper.SetString("Controllers\\" + CS$<>8__locals1.info.Id.ToString(), "EngineKeyboardGuid", guid.ToString());
									}
								}
							}
							if (flag2)
							{
								peripheralPhysicalType = 2;
								if (!RegistryHelper.ValueExists("Controllers\\" + CS$<>8__locals1.info.Id.ToString(), "EngineMouseGuid"))
								{
									guid = Guid.NewGuid();
									RegistryHelper.SetString("Controllers\\" + CS$<>8__locals1.info.Id.ToString(), "EngineMouseGuid", guid.ToString());
								}
								else
								{
									string string2 = RegistryHelper.GetString("Controllers\\" + CS$<>8__locals1.info.Id.ToString(), "EngineMouseGuid", Guid.Empty.ToString(), false);
									if (!string.IsNullOrEmpty(string2))
									{
										guid = Guid.Parse(string2);
									}
									else
									{
										guid = Guid.NewGuid();
										RegistryHelper.SetString("Controllers\\" + CS$<>8__locals1.info.Id.ToString(), "EngineMouseGuid", guid.ToString());
									}
								}
							}
							PeripheralVM peripheralVM = new PeripheralVM(guid, false)
							{
								PeripheralPhysicalType = peripheralPhysicalType
							};
							REWASD_CONTROLLER_INFO rewasd_CONTROLLER_INFO = REWASD_CONTROLLER_INFO.CreateBlankInstance();
							rewasd_CONTROLLER_INFO.Id = tuple.Item1;
							rewasd_CONTROLLER_INFO.Type = ControllerTypeExtensions.ConvertEnumToPhysicalType(tuple.Item2);
							rewasd_CONTROLLER_INFO.ContainerId = guid;
							ControllerVM controllerVM = new ControllerVM(rewasd_CONTROLLER_INFO, false);
							if (flag)
							{
								controllerVM.ControllerFriendlyName = ControllerTypeExtensions.GetFriendlyName(503);
							}
							if (ControllerTypeExtensions.IsEngineMouse(tuple.Item2))
							{
								controllerVM.ControllerFriendlyName = ControllerTypeExtensions.GetFriendlyName(504);
							}
							if (ControllerTypeExtensions.IsEngineMouseTouchpad(tuple.Item2))
							{
								controllerVM.ControllerFriendlyName = ControllerTypeExtensions.GetFriendlyName(505);
							}
							peripheralVM.AddController(controllerVM);
							compositeDevice.PutControllerInfoIntoChain(peripheralVM, num);
							num++;
						}
						compositeDevice.ID = IControllerProfileInfoCollectionContainerExtensions.CalculateID(compositeDevice);
						XBUtils.SetControllerSavedFriendlyName(XBUtils.CalcShortID(compositeDevice.ID), CS$<>8__locals1.info.Name);
					}
					await Engine.GamepadService.BinDataSerialize.SaveCompositeDevicesCollection();
					Engine.EventAggregator.GetEvent<CompositeSettingsChanged>().Publish(null);
				}
				UtilsCommon.SaveEngineControllerToRegistry(CS$<>8__locals1.info.Id, CS$<>8__locals1.info.EngineControllerTypes);
				XBUtils.SetControllerSavedFriendlyName(CS$<>8__locals1.info.Id.ToString(), CS$<>8__locals1.info.Name);
			}
			catch
			{
			}
			Lazy<IGamepadService> gamepadServiceLazy2 = Engine.GamepadServiceLazy;
			bool flag3;
			if (gamepadServiceLazy2 == null)
			{
				flag3 = null != null;
			}
			else
			{
				IGamepadService value2 = gamepadServiceLazy2.Value;
				flag3 = ((value2 != null) ? value2.EngineControllersWpapper : null) != null;
			}
			if (flag3)
			{
				bool error = false;
				TaskAwaiter<bool> taskAwaiter2;
				if (CS$<>8__locals1.info.EngineControllerTypes.Any((EngineControllerType x) => EngineControllerTypeExtensions.IsGamepad(x)))
				{
					TaskAwaiter<bool> taskAwaiter = Engine.GamepadServiceLazy.Value.EngineControllersWpapper.AddEngineController(CS$<>8__locals1.info.Id, 268435455U, CS$<>8__locals1.info).GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<bool>);
					}
					if (!taskAwaiter.GetResult())
					{
						error = true;
					}
				}
				if (CS$<>8__locals1.info.EngineControllerTypes.Any((EngineControllerType x) => EngineControllerTypeExtensions.IsKeyboard(x)))
				{
					TaskAwaiter<bool> taskAwaiter = Engine.GamepadServiceLazy.Value.EngineControllersWpapper.AddEngineController(CS$<>8__locals1.info.Id, 268435453U, CS$<>8__locals1.info).GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<bool>);
					}
					if (!taskAwaiter.GetResult())
					{
						error = true;
					}
				}
				if (CS$<>8__locals1.info.EngineControllerTypes.Any((EngineControllerType x) => EngineControllerTypeExtensions.IsAnyMouse(x)))
				{
					TaskAwaiter<bool> taskAwaiter = Engine.GamepadServiceLazy.Value.EngineControllersWpapper.AddEngineController(CS$<>8__locals1.info.Id, 268435454U, CS$<>8__locals1.info).GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<bool>);
					}
					if (!taskAwaiter.GetResult())
					{
						error = true;
					}
				}
				if (!error)
				{
					GamepadServiceController.<>c__DisplayClass55_1 CS$<>8__locals2 = new GamepadServiceController.<>c__DisplayClass55_1();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					Engine.EngineControllerMonitor.AddController(CS$<>8__locals2.CS$<>8__locals1.info.Id);
					if (CS$<>8__locals2.CS$<>8__locals1.info.EngineControllerTypes.Count != 0)
					{
						SenderGoogleAnalytics.SendMessageEvent("MobileControllers", "Create", string.Join<EngineControllerType>(", ", CS$<>8__locals2.CS$<>8__locals1.info.EngineControllerTypes.ToArray()), -1L, false);
					}
					BaseControllerVM baseControllerVM = Engine.GamepadServiceLazy.Value.GamepadCollection.FindControllerByControllerId(CS$<>8__locals2.CS$<>8__locals1.info.Id);
					CS$<>8__locals2.batteryInfo = CS$<>8__locals2.CS$<>8__locals1.info.BatteryInfo ?? EngineBatteryInfo.Default;
					if (baseControllerVM != null)
					{
						if (!baseControllerVM.IsCustomControllerFriendlyName && baseControllerVM.ControllerFriendlyName != CS$<>8__locals2.CS$<>8__locals1.info.Name)
						{
							baseControllerVM.ControllerFriendlyName = CS$<>8__locals2.CS$<>8__locals1.info.Name;
							Engine.GamepadService.UpdateDeviceFriendlyName(CS$<>8__locals2.CS$<>8__locals1.info.Id.ToString());
							Engine.EventAggregator.GetEvent<DeviceRenamed>().Publish(CS$<>8__locals2.CS$<>8__locals1.info.Id.ToString());
						}
						ControllerVM controllerVM2 = ((!(baseControllerVM is CompositeControllerVM)) ? (baseControllerVM as ControllerVM) : ((baseControllerVM as CompositeControllerVM).BaseControllers.FirstOrDefault((BaseControllerVM c) => c.Ids.Contains(CS$<>8__locals2.CS$<>8__locals1.info.Id)) as ControllerVM));
						if (controllerVM2 != null)
						{
							Engine.GamepadService.SetEngineBatteryState(controllerVM2, CS$<>8__locals2.batteryInfo.BatteryLevel, CS$<>8__locals2.batteryInfo.ChargingState);
						}
					}
					else
					{
						Task.Run(delegate
						{
							GamepadServiceController.<>c__DisplayClass55_1.<<AddEngineController>b__4>d <<AddEngineController>b__4>d;
							<<AddEngineController>b__4>d.<>t__builder = AsyncTaskMethodBuilder.Create();
							<<AddEngineController>b__4>d.<>4__this = CS$<>8__locals2;
							<<AddEngineController>b__4>d.<>1__state = -1;
							<<AddEngineController>b__4>d.<>t__builder.Start<GamepadServiceController.<>c__DisplayClass55_1.<<AddEngineController>b__4>d>(ref <<AddEngineController>b__4>d);
							return <<AddEngineController>b__4>d.<>t__builder.Task;
						});
					}
				}
			}
			return this.Ok();
		}

		[HttpPost]
		[Route("RemoveEngineController")]
		public async Task<IActionResult> RemoveEngineController([FromBody] RemoveEngineControllerInfo info)
		{
			Engine.EngineControllerMonitor.RemoveController(info.Id);
			Lazy<IGamepadService> gamepadServiceLazy = Engine.GamepadServiceLazy;
			bool flag;
			if (gamepadServiceLazy == null)
			{
				flag = null != null;
			}
			else
			{
				IGamepadService value = gamepadServiceLazy.Value;
				flag = ((value != null) ? value.EngineControllersWpapper : null) != null;
			}
			if (flag)
			{
				List<uint> list = await Engine.GamepadServiceLazy.Value.EngineControllersWpapper.FindAllEngineTypesById(info.Id);
				if (list.Count > 0)
				{
					foreach (uint num in list)
					{
						await Engine.GamepadServiceLazy.Value.EngineControllersWpapper.RemoveEngineController(info.Id, num);
					}
					List<uint>.Enumerator enumerator = default(List<uint>.Enumerator);
				}
			}
			return this.Ok();
		}

		[HttpPost]
		[Route("SetEngineBatteryState")]
		public IActionResult SetEngineBatteryState([FromBody] EngineBatteryInfo batteryInfo)
		{
			ControllerVM controllerVM = Engine.GamepadService.FindControllerBySingleId(batteryInfo.Id) as ControllerVM;
			if (controllerVM != null)
			{
				Engine.GamepadService.SetEngineBatteryState(controllerVM, batteryInfo.BatteryLevel, batteryInfo.ChargingState);
			}
			return this.Ok();
		}
	}
}
