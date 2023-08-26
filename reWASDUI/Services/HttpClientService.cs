using System;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using reWASDUI.Services.HttpClient;
using reWASDUI.Services.Interfaces;

namespace reWASDUI.Services
{
	public class HttpClientService : ZBindableBase, IHttpClientService
	{
		public bool InitializationIsFailed
		{
			get
			{
				return HttpClientService.InitializationIsFailedStatic;
			}
			set
			{
				this.SetProperty<bool>(ref HttpClientService.InitializationIsFailedStatic, value, "InitializationIsFailed");
			}
		}

		public EngineClientService Engine { get; } = new EngineClientService();

		public GamepadClientService Gamepad { get; } = new GamepadClientService();

		public GameProfilesClientService GameProfiles { get; } = new GameProfilesClientService();

		public ExternalDevicesClientService ExternalDevices { get; } = new ExternalDevicesClientService();

		public LicenseApi LicenseApi { get; } = new LicenseApi();

		public static bool InitializationIsFailedStatic;
	}
}
