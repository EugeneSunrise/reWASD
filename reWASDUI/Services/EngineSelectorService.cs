using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using Prism.Ioc;
using reWASDCommon.Services.HttpServer;
using reWASDCommon.Utils;
using reWASDUI.Services.Interfaces;

namespace reWASDUI.Services
{
	public class EngineSelectorService : ZBindableBase, IEngineSelectorService
	{
		public ObservableCollection<EngineInfo> Engines { get; set; } = new ObservableCollection<EngineInfo>();

		public EngineInfo CurrentEngine
		{
			get
			{
				return this._currentEngine;
			}
			set
			{
				if (this.SetProperty<EngineInfo>(ref this._currentEngine, value, "CurrentEngine"))
				{
					this.ChangeEngine(this._currentEngine);
				}
			}
		}

		public async Task ChangeEngine(EngineInfo engine)
		{
			IContainerProviderExtensions.Resolve<ISSEProcessor>(App.Container).StopAndClose();
			HttpServerSettings.CurrentRoute = engine.Address;
			IContainerProviderExtensions.Resolve<ISSEProcessor>(App.Container).InitAndRun();
			App.GameProfilesService.CurrentGame = null;
			await App.LicensingService.CheckLicenseAsync(false);
			await App.GameProfilesService.FillGamesCollection();
			await App.GamepadService.BinDataSerialize.LoadAllBins();
			await App.GamepadService.RefreshGamepadCollection(0UL);
		}

		public EngineSelectorService()
		{
			EngineInfo engineInfo = new EngineInfo
			{
				Address = "127.0.0.1",
				Name = "localhost",
				Port = HttpServerSettings.GetPort()
			};
			this.Engines.Add(engineInfo);
			this._currentEngine = engineInfo;
		}

		~EngineSelectorService()
		{
			CrossPlatformLib.Stop();
		}

		public void DeviceWasFound(string devId, string name, string ip, int port, int devType)
		{
			EngineInfo remote = new EngineInfo
			{
				Address = ip,
				Name = name,
				Port = port
			};
			Func<EngineInfo, bool> <>9__1;
			ThreadHelper.ExecuteInMainDispatcher(delegate
			{
				IEnumerable<EngineInfo> engines = this.Engines;
				Func<EngineInfo, bool> func;
				if ((func = <>9__1) == null)
				{
					func = (<>9__1 = (EngineInfo item) => item.Address == ip);
				}
				if (!engines.Any(func))
				{
					this.Engines.Add(remote);
				}
			}, true);
		}

		private void RunDiscovery()
		{
			this.deviceWasFound = new CrossPlatformLib.HlpLibDeviceWasFound(this.DeviceWasFound);
			CrossPlatformLib.ConnDeviceInfo connDeviceInfo = default(CrossPlatformLib.ConnDeviceInfo);
			connDeviceInfo.ip = "";
			connDeviceInfo.port = HttpServerSettings.GetPort() - 1;
			connDeviceInfo.emulatorPort = HttpServerSettings.GetEmulatorPort();
			connDeviceInfo.devType = 1;
			connDeviceInfo.name = HttpServerSettings.GetDeviceName();
			connDeviceInfo.devId = HttpServerSettings.GetDevId();
			CrossPlatformLib.Initialize(null, this.deviceWasFound, connDeviceInfo, 0);
			CrossPlatformLib.StartDiscoverDevices(HttpServerSettings.GetPort());
		}

		private EngineInfo _currentEngine;

		private CrossPlatformLib.HlpLibDeviceWasFound deviceWasFound;
	}
}
