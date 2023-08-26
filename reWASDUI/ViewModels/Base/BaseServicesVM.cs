using System;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using reWASDUI.Infrastructure;
using reWASDUI.Services;
using reWASDUI.Services.Interfaces;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.ViewModels.Base;

namespace reWASDUI.ViewModels.Base
{
	public class BaseServicesVM : ZBindable, IRegionManagerAware, IBaseServicesContainer
	{
		public GameProfilesService GameProfilesService { get; set; }

		public GuiHelperService GuiHelperService { get; set; }

		public GamepadService GamepadService { get; set; }

		public KeyBindingService KeyBindingService { get; set; }

		public LicensingService LicensingService { get; set; }

		public GuiScaleService GuiScaleService { get; set; }

		public HttpClientService HttpClientService { get; set; }

		public DeviceDetectionService DeviceDetectionService { get; set; }

		public EventAggregator EventAggregator { get; set; }

		public UserSettingsService UserSettingsService { get; set; }

		public IRegionManager RegionManager { get; set; }

		public BaseServicesVM(IContainerProvider container)
		{
			this.GameProfilesService = IContainerProviderExtensions.Resolve<IGameProfilesService>(container) as GameProfilesService;
			this.UserSettingsService = IContainerProviderExtensions.Resolve<IUserSettingsService>(container) as UserSettingsService;
			this.GamepadService = IContainerProviderExtensions.Resolve<IGamepadService>(container) as GamepadService;
			this.KeyBindingService = IContainerProviderExtensions.Resolve<IKeyBindingService>(container) as KeyBindingService;
			this.LicensingService = IContainerProviderExtensions.Resolve<ILicensingService>(container) as LicensingService;
			this.EventAggregator = IContainerProviderExtensions.Resolve<IEventAggregator>(container) as EventAggregator;
			this.GuiScaleService = IContainerProviderExtensions.Resolve<IGuiScaleService>(container) as GuiScaleService;
			this.DeviceDetectionService = IContainerProviderExtensions.Resolve<IDeviceDetectionService>(container) as DeviceDetectionService;
			this.GuiHelperService = IContainerProviderExtensions.Resolve<IGuiHelperService>(container) as GuiHelperService;
			this.HttpClientService = IContainerProviderExtensions.Resolve<IHttpClientService>(container) as HttpClientService;
			this.RegionManager = IContainerProviderExtensions.Resolve<IRegionManager>(container);
		}
	}
}
