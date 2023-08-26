using System;
using reWASDUI.Services.HttpClient;

namespace reWASDUI.Services.Interfaces
{
	public interface IHttpClientService
	{
		EngineClientService Engine { get; }

		GamepadClientService Gamepad { get; }

		GameProfilesClientService GameProfiles { get; }

		ExternalDevicesClientService ExternalDevices { get; }

		LicenseApi LicenseApi { get; }
	}
}
