using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using reWASDCommon.Network.HTTP;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using reWASDUI.DataModels;
using XBEliteWPF.Infrastructure.KeyBindingsModel;

namespace reWASDUI.Services.HttpClient
{
	public class GameProfilesClientService : BaseHttpClientService
	{
		public async Task<ObservableCollection<ConfigVM>> GetPresetsCollection(bool forceRefresh)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest(string.Format("GameProfilesService/PresetsCollection/{0}", forceRefresh)));
			ObservableCollection<ConfigVM> observableCollection;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				observableCollection = null;
			}
			else
			{
				observableCollection = JsonConvert.DeserializeObject<ObservableCollection<ConfigVM>>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return observableCollection;
		}

		public async Task<ObservableCollection<ConfigVM>> GetConfigsCollection(string gameName)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(gameName), "GameProfilesService/ConfigsCollection"));
			ObservableCollection<ConfigVM> observableCollection;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				observableCollection = null;
			}
			else
			{
				observableCollection = JsonConvert.DeserializeObject<ObservableCollection<ConfigVM>>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return observableCollection;
		}

		public async Task<ObservableCollection<GameVM>> GetGamesCollection()
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetRequest("GameProfilesService/GamesCollection"));
			ObservableCollection<GameVM> observableCollection;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				observableCollection = null;
			}
			else
			{
				observableCollection = JsonConvert.DeserializeObject<ObservableCollection<GameVM>>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return observableCollection;
		}

		public async Task<BaseResponse> SaveOverlayPreset(SaveConfigParams<ConfigData> configParams)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(configParams), "GameProfilesService/SaveOverlayPreset"));
			BaseResponse baseResponse;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				baseResponse = BaseResponse.False;
			}
			else
			{
				baseResponse = JsonConvert.DeserializeObject<BaseResponse>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return baseResponse;
		}

		public async Task<BaseResponse> SavePreset(SaveConfigParams<ConfigData> configParams)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(configParams), "GameProfilesService/SavePreset"));
			BaseResponse baseResponse;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				baseResponse = BaseResponse.False;
			}
			else
			{
				baseResponse = JsonConvert.DeserializeObject<BaseResponse>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return baseResponse;
		}

		public async Task<BaseResponse> SaveConfig(SaveConfigParams<ConfigData> configParams)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(configParams), "GameProfilesService/SaveConfig"));
			BaseResponse baseResponse;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				baseResponse = BaseResponse.False;
			}
			else
			{
				baseResponse = JsonConvert.DeserializeObject<BaseResponse>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return baseResponse;
		}

		public async Task<ConfigData> GetConfig(ConfigParams configParams)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(configParams), "GameProfilesService/Config"));
			ConfigData configData;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				configData = null;
			}
			else
			{
				configData = JsonConvert.DeserializeObject<ConfigData>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return configData;
		}

		public async Task<ResponseWithError> CreateNewGame(NewGameParams newGameParams)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(newGameParams), "GameProfilesService/CreateNewGame"));
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

		public async Task<ResponseWithError> CreateNewConfig(ConfigParams newConfigParams)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(newConfigParams), "GameProfilesService/CreateNewConfig"));
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

		public async Task<CopyDirectoryParams> CopyDirectory(CopyDirectoryParams copyParams)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(copyParams), "GameProfilesService/CopyDirectory"));
			CopyDirectoryParams copyDirectoryParams;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				copyDirectoryParams = new CopyDirectoryParams();
			}
			else
			{
				copyDirectoryParams = JsonConvert.DeserializeObject<CopyDirectoryParams>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return copyDirectoryParams;
		}

		public async Task<ResponseWithError> RenameConfig(RenameConfigParams configParams)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(configParams), "GameProfilesService/RenameConfig"));
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

		public async Task<ResponseWithError> RenamePreset(RenameConfigParams configParams)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(configParams), "GameProfilesService/RenamePreset"));
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

		public async Task<ResponseWithError> RenameGame(RenameGameParams gameParams)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(gameParams), "GameProfilesService/RenameGame"));
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

		public async Task<ResponseWithError> RemovePreset(DeleteConfigParams deleteParams)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(deleteParams), "GameProfilesService/RemovePreset"));
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

		public async Task<ResponseWithError> DeleteConfig(DeleteConfigParams deleteParams)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(deleteParams), "GameProfilesService/DeleteConfig"));
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

		public async Task<ResponseWithError> DeleteGame(string gameName)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(new Tuple<string, string>(SSEClient.ClientID, gameName)), "GameProfilesService/DeleteGame"));
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

		public async Task<ImportConfigResult> ImportConfig(ImportConfigInfo importParams)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(importParams), "GameProfilesService/ImportConfig"));
			ImportConfigResult importConfigResult;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				importConfigResult = new ImportConfigResult
				{
					Status = 3
				};
			}
			else
			{
				importConfigResult = JsonConvert.DeserializeObject<ImportConfigResult>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return importConfigResult;
		}

		public async Task<OverlayCirclePreviewResult> CreateOverlayCirclePreview(OverlayCirclePreviewInfo previewParams)
		{
			HttpResponseMessage httpResponseMessage = await HttpUtils.SendRequest(() => HttpUtils.GetPostRequest(JsonConvert.SerializeObject(previewParams), "GameProfilesService/CreateOverlayCirclePreview"));
			OverlayCirclePreviewResult overlayCirclePreviewResult;
			if (!base.CheckResponseError(httpResponseMessage))
			{
				overlayCirclePreviewResult = new OverlayCirclePreviewResult
				{
					ErrorText = httpResponseMessage.ReasonPhrase
				};
			}
			else
			{
				overlayCirclePreviewResult = JsonConvert.DeserializeObject<OverlayCirclePreviewResult>(await httpResponseMessage.Content.ReadAsStringAsync());
			}
			return overlayCirclePreviewResult;
		}
	}
}
