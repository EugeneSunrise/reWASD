using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using reWASDCommon.Network.HTTP;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using reWASDEngine.Services.HttpServer.Data;
using XBEliteWPF.Infrastructure;

namespace reWASDUI.Services.HttpClient
{
	public class EngineClientService : BaseHttpClientService
	{
		public async Task<HttpResponseMessage> WaitForInited()
		{
			return await HttpUtils.SendRequestWithTimeout(() => HttpUtils.GetRequest("EngineService/WaitForInited"), 10);
		}

		public async Task RequestReloadUserSettings()
		{
			await HttpUtils.SendRequest(() => HttpUtils.GetRequest("EngineService/RequestReloadUserSettings"));
		}

		public void RequestHttpRestart(string addr)
		{
			HttpUtils.SendRequestAsync(() => new HttpRequestMessage(HttpMethod.Get, addr + "EngineService/RequestHttpRestart"));
		}

		public async Task RequestUdpRestart()
		{
			await HttpUtils.SendRequest(() => HttpUtils.GetRequest("EngineService/RequestUdpRestart"));
		}

		public async Task RequestReloadExternalDevicesData()
		{
			await HttpUtils.SendRequest(() => HttpUtils.GetRequest("EngineService/RequestReloadExternalDevicesData"));
		}

		public async Task<ResponseWithError> GenerateXPSFromConfig(GenerateXPSFromConfigInfo info)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequestWithTimeout(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(info), "EngineService/GenerateXPSFromConfig"), 30);
			ResponseWithError responseWithError;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				responseWithError = ResponseWithError.False;
			}
			else
			{
				responseWithError = JsonConvert.DeserializeObject<ResponseWithError>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return responseWithError;
		}

		public async Task UpdateDeviceFriendlyName(string ID)
		{
			await HttpUtils.SendRequest(() => HttpUtils.GetRequest("EngineService/UpdateDeviceFriendlyName/" + ID));
		}

		public async Task DeletePromoController(string id)
		{
			await HttpUtils.SendRequest(() => HttpUtils.GetRequest("EngineService/DeletePromoController/" + id));
		}

		public async Task<bool> RequestUDPStart(string IP, int PORT)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("EngineService/RequestUDPStart/" + IP + ":" + PORT.ToString()));
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

		public async Task<List<Tuple<ulong, uint>>> GetDuplicateGamepadCollection()
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("EngineService/GetDuplicateGamepadCollection"));
			List<Tuple<ulong, uint>> list;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				list = null;
			}
			else
			{
				list = JsonConvert.DeserializeObject<List<Tuple<ulong, uint>>>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return list;
		}

		public async Task<bool> RequestUDPStart()
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("EngineService/RequestUDPStart"));
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

		public async Task RequestUDPStopping()
		{
			await HttpUtils.SendRequest(() => HttpUtils.GetRequest("EngineService/RequestUDPStopping"));
		}

		public async Task<bool> IsUdpRunning()
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("EngineService/IsUdpRunning"));
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

		public async Task<bool> IsBluetoothAdapterIsSupportedForNintendoConsole()
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("EngineService/IsBluetoothAdapterIsSupportedForNintendoConsole"));
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

		public async Task<List<BluetoothDeviceInfo>> GetServiceBluetoothDeviceInfo(uint flags)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest(string.Format("EngineService/GetServiceBluetoothDeviceInfo/{0}", flags)));
			List<BluetoothDeviceInfo> list;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				list = new List<BluetoothDeviceInfo>();
			}
			else
			{
				list = JsonConvert.DeserializeObject<List<BluetoothDeviceInfo>>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return list;
		}

		public async Task<bool> UnpairController(ulong macAddress)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest(string.Format("EngineService/UnpairController/{0}", macAddress)));
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

		public async Task<GamepadUdpServerState> GetGamepadUdpServerState()
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("EngineService/GetGamepadUdpServerState"));
			GamepadUdpServerState gamepadUdpServerState;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				gamepadUdpServerState = null;
			}
			else
			{
				gamepadUdpServerState = JsonConvert.DeserializeObject<GamepadUdpServerState>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return gamepadUdpServerState;
		}

		public async Task<bool> GetIsUdpEnabledInPreferences()
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("EngineService/GetIsUdpEnabledInPreferences"));
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

		public async Task SetIsUdpEnabledInPreferences(bool value)
		{
			await HttpUtils.SendRequest(() => HttpUtils.GetRequest("EngineService/SetIsUdpEnabledInPreferences/" + value.ToString()));
		}

		public async Task<bool> IsUdpReserved()
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("EngineService/IsUdpReserved"));
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

		public async Task<bool> IsUdpServerHasException()
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("EngineService/IsUdpServerHasException"));
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
	}
}
