using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Network.HTTP;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using XBEliteWPF.Infrastructure.ExternalDeviceRelationsModel;
using XBEliteWPF.Infrastructure.ExternalDevices;

namespace reWASDUI.Services.HttpClient
{
	public class ExternalDevicesClientService : BaseHttpClientService
	{
		public async Task<ExternalState> GetExternalState(GetExternalStateInfo info)
		{
			ExternalState externalState;
			if (info.ConfigName == null)
			{
				externalState = 0;
			}
			else
			{
				HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(info), "ExternalDevices/GetExternalState"));
				if (!base.CheckResponseError(httpResponseMessage))
				{
					externalState = 0;
				}
				else
				{
					externalState = JsonConvert.DeserializeObject<ExternalState>(await httpResponseMessage.Content.ReadAsStringAsync());
				}
			}
			return externalState;
		}

		public async Task<ExternalDeviceState> GetExternalDeviceState(GetExternalStateInfo info)
		{
			ExternalDeviceState externalDeviceState;
			if (info.ConfigName == null)
			{
				externalDeviceState = 0;
			}
			else
			{
				HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(info), "ExternalDevices/GetExternalDeviceState"));
				if (!base.CheckResponseError(httpResponseMessage))
				{
					externalDeviceState = 0;
				}
				else
				{
					externalDeviceState = JsonConvert.DeserializeObject<ExternalDeviceState>(await httpResponseMessage.Content.ReadAsStringAsync());
				}
			}
			return externalDeviceState;
		}

		public async Task<ExternalDeviceState> GetExternalDeviceStateWithProfiles(GetExternalStateInfo info)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(info), "ExternalDevices/GetExternalDeviceStateWithProfiles"));
			ExternalDeviceState externalDeviceState;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				externalDeviceState = 0;
			}
			else
			{
				externalDeviceState = JsonConvert.DeserializeObject<ExternalDeviceState>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return externalDeviceState;
		}

		public async Task ExternalDeviceBluetoothReconnect(string gamepadID, Slot slot)
		{
			await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(new SelectSlotInfo
			{
				ID = gamepadID,
				Slot = slot
			}), string.Format("ExternalDevices/ExternalDeviceBluetoothReconnect", Array.Empty<object>())));
		}

		public async Task ExternalDeviceDisableRemapForSerialPort(string serialPort)
		{
			await HttpUtils.SendRequest(() => HttpUtils.GetRequest(string.Format("ExternalDevices/ExternalDeviceDisableRemapForSerialPort/{0}", serialPort)));
		}

		public async Task<ExternalDeviceRelationsCollection> GetExternalDeviceRelations()
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("ExternalDevices/ExternalDeviceRelations"));
			ExternalDeviceRelationsCollection externalDeviceRelationsCollection;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				externalDeviceRelationsCollection = null;
			}
			else
			{
				externalDeviceRelationsCollection = JsonConvert.DeserializeObject<ExternalDeviceRelationsCollection>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return externalDeviceRelationsCollection;
		}

		public async Task<bool> SaveExternalDeviceRelations(ExternalDeviceRelationsCollection collection)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(collection), "ExternalDevices/SaveExternalDeviceRelations"));
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

		public async Task<ExternalDevicesCollection> GetExternalDevices()
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("ExternalDevices/ExternalDevices"));
			ExternalDevicesCollection externalDevicesCollection;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				externalDevicesCollection = null;
			}
			else
			{
				externalDevicesCollection = JsonConvert.DeserializeObject<ExternalDevicesCollection>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return externalDevicesCollection;
		}

		public async Task<bool> SaveExternalDevices(ExternalDevicesCollection collection)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(collection), "ExternalDevices/SaveExternalDevices"));
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

		public async Task<ObservableCollection<ExternalClient>> GetExternalClients()
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("ExternalDevices/ExternalClients"));
			ObservableCollection<ExternalClient> observableCollection;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				observableCollection = null;
			}
			else
			{
				observableCollection = JsonConvert.DeserializeObject<ObservableCollection<ExternalClient>>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return observableCollection;
		}

		public async Task<bool> SaveExternalClients(ObservableCollection<ExternalClient> collection)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(collection), "ExternalDevices/SaveExternalClients"));
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
