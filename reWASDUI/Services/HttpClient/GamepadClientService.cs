using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Utils;
using Newtonsoft.Json;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Infrastructure.LED;
using reWASDCommon.Network.HTTP;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using reWASDEngine.Services.HttpServer.Data;
using reWASDUI.DataModels.CompositeDevicesCollection;
using reWASDUI.DataModels.GamepadActiveProfiles;
using reWASDUI.DataModels.GamepadSlotHotkeyCollection;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Properties;
using XBEliteWPF.DataModels.PeripheralDevicesCollection;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.LED;
using XBEliteWPF.Services;

namespace reWASDUI.Services.HttpClient
{
	public class GamepadClientService : BaseHttpClientService
	{
		private async Task<EnableRemapResponse> SendApplyConfigRequest(ConfigApplyInfo info)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequestWithTimeout(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(info), "GamepadService/AppliedConfig"), 15);
			EnableRemapResponse enableRemapResponse;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				enableRemapResponse = null;
			}
			else
			{
				enableRemapResponse = JsonConvert.DeserializeObject<EnableRemapResponse>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return enableRemapResponse;
		}

		public async Task<ObservableCollection<BaseControllerVM>> GetGamepadCollection()
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("GamepadService/GamepadCollection"));
			ObservableCollection<BaseControllerVM> observableCollection;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				observableCollection = null;
			}
			else
			{
				observableCollection = JsonConvert.DeserializeObject<ObservableCollection<BaseControllerVM>>(await httpResponseMessage.Content.ReadAsStringAsync(), new JsonConverter[]
				{
					new ControllersJsonConverter()
				});
			}
			return observableCollection;
		}

		public async Task<GamepadState> GetControllerPressedButtons(string id)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(new ControllerIDInfo
			{
				ID = id
			}), "GamepadService/GetControllerPressedButtons"));
			GamepadState gamepadState;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				gamepadState = null;
			}
			else
			{
				gamepadState = JsonConvert.DeserializeObject<GamepadState>(await httpResponseMessage.Content.ReadAsStringAsync(), new JsonConverter[]
				{
					new ControllersJsonConverter()
				});
			}
			return gamepadState;
		}

		public async Task<bool> IsControllerPressedButton(string id)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(new ControllerIDInfo
			{
				ID = id
			}), "GamepadService/IsControllerPressedButton"));
			bool flag;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				flag = false;
			}
			else
			{
				flag = JsonConvert.DeserializeObject<bool>(await httpResponseMessage.Content.ReadAsStringAsync(), new JsonConverter[]
				{
					new ControllersJsonConverter()
				});
			}
			return flag;
		}

		public async Task<ushort> ApplyHardwareDonglePingProfile(SpecialProfileInfo profileInfo)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(profileInfo), "GamepadService/ApplyHardwareDonglePingProfile"));
			ushort num;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				num = 0;
			}
			else
			{
				num = JsonConvert.DeserializeObject<ushort>(await httpResponseMessage.Content.ReadAsStringAsync(), new JsonConverter[]
				{
					new ControllersJsonConverter()
				});
			}
			return num;
		}

		public async Task<PingInfo> GetProfilePingInfo(ushort profileId)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("GamepadService/GetProfilePingInfo/" + profileId.ToString()));
			PingInfo pingInfo;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				pingInfo = null;
			}
			else
			{
				pingInfo = JsonConvert.DeserializeObject<PingInfo>(await httpResponseMessage.Content.ReadAsStringAsync(), new JsonConverter[]
				{
					new ControllersJsonConverter()
				});
			}
			return pingInfo;
		}

		public async Task<GamepadRemapState> GetGamepadRemapState(string id)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(new ControllerIDInfo
			{
				ID = id
			}), "GamepadService/RemapState"));
			GamepadRemapState gamepadRemapState;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				gamepadRemapState = null;
			}
			else
			{
				gamepadRemapState = JsonConvert.DeserializeObject<GamepadRemapState>(await httpResponseMessage.Content.ReadAsStringAsync(), new JsonConverter[]
				{
					new ControllersJsonConverter()
				});
			}
			return gamepadRemapState;
		}

		public async Task<VirtualGamepadType?> GetVirtualGamepadType(string id)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(new ControllerIDInfo
			{
				ID = id
			}), "GamepadService/VirtualGamepadType"));
			VirtualGamepadType? virtualGamepadType;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				virtualGamepadType = null;
			}
			else
			{
				virtualGamepadType = JsonConvert.DeserializeObject<VirtualGamepadType?>(await httpResponseMessage.Content.ReadAsStringAsync(), new JsonConverter[]
				{
					new ControllersJsonConverter()
				});
			}
			return virtualGamepadType;
		}

		public async Task DeleteAllExclusiveCaptureProfiles()
		{
			await HttpUtils.SendRequest(() => HttpUtils.GetRequest("GamepadService/DeleteAllExclusiveCaptureProfiles"));
		}

		public async Task<bool> ApplyHoneypotProfile(ExternalDeviceType externalDeviceType, VirtualGamepadType virtualGamepadType, string comPort = "", uint baudRate = 0U)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(new SpecialProfileInfo
			{
				ExternalDeviceType = externalDeviceType,
				VirtualGamepadType = virtualGamepadType,
				ComPort = comPort,
				BaudRate = baudRate
			}), "GamepadService/ApplyHoneypotProfile"));
			bool flag;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				flag = false;
			}
			else
			{
				flag = JsonConvert.DeserializeObject<bool>(await httpResponseMessage.Content.ReadAsStringAsync(), new JsonConverter[]
				{
					new ControllersJsonConverter()
				});
			}
			return flag;
		}

		public async Task DeleteSpecialProfiles()
		{
			await HttpUtils.SendRequest(() => HttpUtils.GetRequest("GamepadService/DeleteSpecialProfiles"));
		}

		public async Task ControllerStartManualHardwareGyroReCalibration(ulong gamepadID, uint gamepadType)
		{
			await HttpUtils.SendRequest(() => HttpUtils.GetRequest(string.Format("GamepadService/ControllerStartManualHardwareGyroReCalibration/{0}/{1}", gamepadID, gamepadType)));
		}

		public async Task ControllerStartManualSoftwareGyroReCalibration(ulong gamepadID, uint gamepadType)
		{
			await HttpUtils.SendRequest(() => HttpUtils.GetRequest(string.Format("GamepadService/ControllerStartManualSoftwareGyroReCalibration/{0}/{1}", gamepadID, gamepadType)));
		}

		public async Task ControllerUpdateGyroCalibrationMode(ulong gamepadID, uint gamepadType)
		{
			await HttpUtils.SendRequest(() => HttpUtils.GetRequest(string.Format("GamepadService/ControllerUpdateGyroCalibrationMode/{0}/{1}", gamepadID, gamepadType)));
		}

		public async Task SendControllerVibration(ulong gamepadID, uint gamepadType)
		{
			await HttpUtils.SendRequest(() => HttpUtils.GetRequest(string.Format("GamepadService/SendControllerVibration/{0}/{1}", gamepadID, gamepadType)));
		}

		public async Task SwitchControllerToHidMode(ulong gamepadID, uint gamepadType)
		{
			await HttpUtils.SendRequest(() => HttpUtils.GetRequest(string.Format("GamepadService/SwitchControllerToHidMode/{0}/{1}", gamepadID, gamepadType)));
		}

		public async Task ControllerChangeMasterAddress(ulong gamepadID, uint gamepadType, ulong bluetoothAddress)
		{
			await HttpUtils.SendRequest(() => HttpUtils.GetRequest(string.Format("GamepadService/ControllerChangeMasterAddress/{0}/{1}/{2}", gamepadID, gamepadType, bluetoothAddress)));
		}

		public async Task ProcessExclusiveCaptureProfile([CanBeNull] string gamepadID, bool processDelete = true, bool processAdd = true)
		{
			await HttpUtils.SendRequest(() => HttpUtils.GetRequest(string.Format("GamepadService/ProcessExclusiveCaptureProfile/{0}/{1}/{2}", gamepadID, processDelete, processAdd)));
		}

		public async Task SetControllerFriendlyName(string gamepadID, string friendlyName)
		{
			await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(new ControllerFriendlyNameInfo
			{
				ControllerId = gamepadID,
				FriendlyName = friendlyName
			}), "GamepadService/SetControllerFriendlyName"));
		}

		public async Task<ConfigInfo> GetAplliedConfig(string gamepadId, Slot slot)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(new SelectSlotInfo
			{
				ID = gamepadId,
				Slot = slot
			}), "GamepadService/GetAppliedConfig"));
			ConfigInfo configInfo;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				configInfo = null;
			}
			else
			{
				configInfo = JsonConvert.DeserializeObject<ConfigInfo>(await httpResponseMessage.Content.ReadAsStringAsync(), new JsonConverter[]
				{
					new ControllersJsonConverter()
				});
			}
			return configInfo;
		}

		public async Task<Slot> GetCurrentSlot(string gamepadId)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(new ControllerIDInfo
			{
				ID = gamepadId
			}), "GamepadService/CurrentSlot"));
			Slot slot;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				slot = 0;
			}
			else
			{
				slot = JsonConvert.DeserializeObject<Slot>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return slot;
		}

		public async Task<ExclusiveCaptureControllersInfo> GetExclusiveCaptureControllersInfo()
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("GamepadService/GetExclusiveCaptureControllersInfo"));
			ExclusiveCaptureControllersInfo exclusiveCaptureControllersInfo;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				exclusiveCaptureControllersInfo = null;
			}
			else
			{
				exclusiveCaptureControllersInfo = JsonConvert.DeserializeObject<ExclusiveCaptureControllersInfo>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return exclusiveCaptureControllersInfo;
		}

		public async Task ReCompileSteamLizardProfile()
		{
			await HttpUtils.SendRequest(() => HttpUtils.GetRequest("GamepadService/ReCompileSteamLizardProfile"));
		}

		public async Task RefreshInputDevices()
		{
			await HttpUtils.SendRequest(() => HttpUtils.GetRequest("GamepadService/RefreshInputDevices"));
		}

		public async Task<bool> SelectSlot(SelectSlotInfo slotInfo)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(slotInfo), "GamepadService/SelectSlot"));
			bool flag;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				flag = false;
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		public async Task<bool> ClearSlot(ClearSlotInfo slotInfo)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(slotInfo), "GamepadService/ClearSlot"));
			bool flag;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				flag = false;
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		public async Task<int> DisableRemap(string id)
		{
			string json = JsonConvert.SerializeObject(new DisableRemapInfo
			{
				ID = id
			});
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(json, "GamepadService/DisableRemap"));
			return base.CheckResponseError(httpResponseMessage) ? 0 : 1;
		}

		public async Task DisableRemapByExternalClient(ulong id)
		{
			await HttpUtils.SendRequest(() => HttpUtils.GetRequest(string.Format("GamepadService/DisableRemapByExternalClient/{0}", id)));
		}

		public async Task RemoveOfflineGamepad(string id)
		{
			string json = JsonConvert.SerializeObject(new ControllerIDInfo
			{
				ID = id
			});
			await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(json, "GamepadService/RemoveOfflineGamepad"));
		}

		public async Task<GamepadProfilesCollection> GetGamepadProfileRelations()
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("GamepadService/GamepadProfileRelations"));
			GamepadProfilesCollection gamepadProfilesCollection;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				gamepadProfilesCollection = null;
			}
			else
			{
				gamepadProfilesCollection = JsonConvert.DeserializeObject<GamepadProfilesCollection>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return gamepadProfilesCollection;
		}

		public async Task<AutoGamesDetectionGamepadProfilesCollection> GetAutoGamesDetectionGamepadProfileRelations()
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("GamepadService/AutoGamesDetectionGamepadProfileRelations"));
			AutoGamesDetectionGamepadProfilesCollection autoGamesDetectionGamepadProfilesCollection;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				autoGamesDetectionGamepadProfilesCollection = null;
			}
			else
			{
				autoGamesDetectionGamepadProfilesCollection = JsonConvert.DeserializeObject<AutoGamesDetectionGamepadProfilesCollection>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return autoGamesDetectionGamepadProfilesCollection;
		}

		public async Task<bool> SaveAutoGamesDetectionGamepadProfileRelations(AutoGamesDetectionGamepadProfilesCollection collection)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(collection), "GamepadService/SaveAutoGamesDetectionGamepadProfileRelations"));
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

		public async Task<ObservableCollection<BlackListGamepad>> GetBlacklistDevices()
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("GamepadService/BlacklistDevices"));
			ObservableCollection<BlackListGamepad> observableCollection;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				observableCollection = null;
			}
			else
			{
				observableCollection = JsonConvert.DeserializeObject<ObservableCollection<BlackListGamepad>>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return observableCollection;
		}

		public async Task<bool> SaveBlacklistDevices(ObservableCollection<BlackListGamepad> collection)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(collection), "GamepadService/SaveBlacklistDevices"));
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

		public async Task<LEDSettingsGlobal> GetPerDeviceGlobalLedSettings()
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("GamepadService/PerDeviceGlobalLedSettings"));
			LEDSettingsGlobal ledsettingsGlobal;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				ledsettingsGlobal = null;
			}
			else
			{
				ledsettingsGlobal = JsonConvert.DeserializeObject<LEDSettingsGlobal>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return ledsettingsGlobal;
		}

		public async Task<bool> SavePerDeviceGlobalLedSettings(LEDSettingsGlobal collection)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(collection), "GamepadService/SavePerDeviceGlobalLedSettings"));
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

		public async Task<PeripheralDevices> GetPeripheralDevices()
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("GamepadService/PeripheralDevices"));
			PeripheralDevices peripheralDevices;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				peripheralDevices = null;
			}
			else
			{
				peripheralDevices = JsonConvert.DeserializeObject<PeripheralDevices>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return peripheralDevices;
		}

		public async Task<GamepadsHotkeyDictionary> GetGamepadsHotkeyCollection()
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("GamepadService/GamepadsHotkeyCollection"));
			GamepadsHotkeyDictionary gamepadsHotkeyDictionary;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				gamepadsHotkeyDictionary = null;
			}
			else
			{
				gamepadsHotkeyDictionary = JsonConvert.DeserializeObject<GamepadsHotkeyDictionary>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return gamepadsHotkeyDictionary;
		}

		public async Task<bool> SaveGamepadsHotkeyCollection(GamepadsHotkeyDictionary collection)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(collection), "GamepadService/SaveGamepadsHotkeyCollection"));
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

		public async Task<GamepadsPlayerLedDictionary> GetGamepadsUserLedCollection()
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("GamepadService/GamepadsUserLedCollection"));
			GamepadsPlayerLedDictionary gamepadsPlayerLedDictionary;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				gamepadsPlayerLedDictionary = null;
			}
			else
			{
				gamepadsPlayerLedDictionary = JsonConvert.DeserializeObject<GamepadsPlayerLedDictionary>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return gamepadsPlayerLedDictionary;
		}

		public async Task<bool> SaveGamepadsUserLedCollection(GamepadsPlayerLedDictionary collection)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(collection), "GamepadService/SaveGamepadsUserLedCollection"));
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

		public async Task<CompositeDevices> GetCompositeDevicesCollection()
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("GamepadService/CompositeDevicesCollection"));
			CompositeDevices compositeDevices;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				compositeDevices = null;
			}
			else
			{
				compositeDevices = JsonConvert.DeserializeObject<CompositeDevices>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return compositeDevices;
		}

		public async Task<bool> SaveCompositeDevicesCollection(CompositeDevices collection)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(collection), "GamepadService/SaveCompositeDevicesCollection"));
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

		public async Task InitializePeripheralDevice(string controllerId, PeripheralPhysicalType type)
		{
			await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(new InitializeDeviceInfo
			{
				ID = controllerId,
				DeviceType = type.ToString()
			}), "GamepadService/InitializePeripheralDevice"));
		}

		public async Task InitializeDevice(string controllerId, string deviceType = "")
		{
			await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(new InitializeDeviceInfo
			{
				ID = controllerId,
				DeviceType = deviceType
			}), "GamepadService/InitializeDevice"));
		}

		public async Task Reinitialize(string controllerId)
		{
			await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(new ControllerIDInfo
			{
				ID = controllerId
			}), "GamepadService/Reinitialize"));
		}

		public async Task<ObservableCollection<GamepadSettings>> GetGamepadsSettings()
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("GamepadService/GamepadsSettings"));
			ObservableCollection<GamepadSettings> observableCollection;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				observableCollection = null;
			}
			else
			{
				observableCollection = JsonConvert.DeserializeObject<ObservableCollection<GamepadSettings>>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return observableCollection;
		}

		public async Task<bool> SaveGamepadsSettings(ObservableCollection<GamepadSettings> collection)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(collection), "GamepadService/SaveGamepadsSettings"));
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

		public async Task<bool> IsOnlyBluetoothConnection(string gamepadID)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(new ControllerIDInfo
			{
				ID = gamepadID
			}), "GamepadService/IsOnlyBluetoothConnection"));
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

		public async Task ReapplayRemap()
		{
			await HttpUtils.SendRequest(() => HttpUtils.GetRequest("GamepadService/ReapplayRemap"));
		}

		public async Task<BaseResponse> ApplyAmiibo(ApplyAmiiboInfo info)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(info), "GamepadService/ApplyAmiibo"));
			BaseResponse baseResponse;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				baseResponse = null;
			}
			else
			{
				baseResponse = JsonConvert.DeserializeObject<BaseResponse>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return baseResponse;
		}

		private async Task<EnableRemapResponse> SendEnableRemapRequest(EnableRemapInfo info)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequestWithTimeout(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(info), "GamepadService/EnableRemap"), 15);
			EnableRemapResponse enableRemapResponse;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				enableRemapResponse = null;
			}
			else
			{
				enableRemapResponse = JsonConvert.DeserializeObject<EnableRemapResponse>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return enableRemapResponse;
		}

		public async Task<BaseResponse> SteamDeckSetMotorIntensitySettings(bool leftIsStrongIntensity, byte leftIntensity, bool rightIsStrongIntensity, byte rightIntensity)
		{
			SteamDeckMotorIntensitySettings intensitySettings = new SteamDeckMotorIntensitySettings
			{
				LeftIsStrongIntensity = leftIsStrongIntensity,
				LeftIntensity = leftIntensity,
				RightIsStrongIntensity = rightIsStrongIntensity,
				RightIntensity = rightIntensity
			};
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequestWithTimeout(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(intensitySettings), "GamepadService/SteamDeckSetMotorIntensitySettings"), 15);
			BaseResponse baseResponse;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				baseResponse = null;
			}
			else
			{
				baseResponse = JsonConvert.DeserializeObject<BaseResponse>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return baseResponse;
		}

		public async Task<bool> ConfigApply(ConfigApplyInfo data)
		{
			EnableRemapBundle bundle = new EnableRemapBundle
			{
				IsUI = true
			};
			EnableRemapResponse enableRemapResponse;
			for (;;)
			{
				if (data.Bundle == null)
				{
					data.Bundle = new EnableRemapBundle
					{
						IsUI = true
					};
				}
				enableRemapResponse = await this.SendApplyConfigRequest(data);
				if (enableRemapResponse == null)
				{
					break;
				}
				if (enableRemapResponse.IsSucceded)
				{
					goto Block_3;
				}
				if (enableRemapResponse.Dialogs == null || enableRemapResponse.Dialogs.Count == 0)
				{
					goto IL_13E;
				}
				bool flag;
				base.ShowDialogs(enableRemapResponse.Dialogs, ref bundle, out flag, false);
				if (flag)
				{
					goto IL_198;
				}
				data.Bundle = bundle;
				if (bundle.UserActions.Count == 0 || enableRemapResponse.DontReCallEnableRemap)
				{
					goto IL_194;
				}
			}
			Tracer.TraceWrite("Can't apply config", false);
			return false;
			Block_3:
			List<EnableRemapResponseDialog> dialogs = enableRemapResponse.Dialogs;
			if (dialogs == null || dialogs.Count <= 0)
			{
				goto IL_198;
			}
			if (enableRemapResponse.Dialogs.Any(delegate(EnableRemapResponseDialog x)
			{
				if (x != null)
				{
					return x.Buttons.Exists((EnableRemapResponseButton y) => y != null && (y.ButtonAction == 12 || y.ButtonAction == 7));
				}
				return false;
			}))
			{
				bool flag2;
				base.ShowDialogs(enableRemapResponse.Dialogs, ref bundle, out flag2, false);
				goto IL_198;
			}
			goto IL_198;
			IL_13E:
			Tracer.TraceWrite("Can't apply config", false);
			return false;
			IL_194:
			return false;
			IL_198:
			return true;
		}

		public async Task<int> EnableRemap(string id, bool remapNonToggledFromRelations)
		{
			EnableRemapInfo req = new EnableRemapInfo
			{
				ID = id,
				RemapNoToggled = remapNonToggledFromRelations,
				Bundle = null
			};
			EnableRemapBundle bundle = new EnableRemapBundle
			{
				IsUI = true
			};
			EnableRemapResponse enableRemapResponse;
			for (;;)
			{
				if (req.Bundle == null)
				{
					req.Bundle = new EnableRemapBundle
					{
						IsUI = true
					};
				}
				enableRemapResponse = await this.SendEnableRemapRequest(req);
				if (enableRemapResponse == null)
				{
					break;
				}
				if (enableRemapResponse.IsSucceded)
				{
					goto Block_3;
				}
				if (enableRemapResponse.Dialogs == null || enableRemapResponse.Dialogs.Count == 0)
				{
					goto IL_16B;
				}
				bool flag;
				base.ShowDialogs(enableRemapResponse.Dialogs, ref bundle, out flag, false);
				if (flag)
				{
					goto IL_1C5;
				}
				req.Bundle = bundle;
				if (bundle.UserActions.Count == 0 || enableRemapResponse.DontReCallEnableRemap)
				{
					goto IL_1C1;
				}
			}
			Tracer.TraceWrite("Can't enable remap", false);
			return 1;
			Block_3:
			List<EnableRemapResponseDialog> dialogs = enableRemapResponse.Dialogs;
			if (dialogs == null || dialogs.Count <= 0)
			{
				goto IL_1C5;
			}
			if (enableRemapResponse.Dialogs.Any(delegate(EnableRemapResponseDialog x)
			{
				if (x != null)
				{
					return x.Buttons.Exists((EnableRemapResponseButton y) => y != null && (y.ButtonAction == 12 || y.ButtonAction == 7));
				}
				return false;
			}))
			{
				bool flag2;
				base.ShowDialogs(enableRemapResponse.Dialogs, ref bundle, out flag2, false);
				goto IL_1C5;
			}
			goto IL_1C5;
			IL_16B:
			Tracer.TraceWrite("Can't enable remap", false);
			return 1;
			IL_1C1:
			return 1;
			IL_1C5:
			return 0;
		}
	}
}
