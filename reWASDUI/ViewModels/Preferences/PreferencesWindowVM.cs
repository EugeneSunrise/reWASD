using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Localization;
using reWASDUI.Services.Interfaces;
using reWASDUI.ViewModels.Preferences.Base;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.ViewModels.Base;

namespace reWASDUI.ViewModels.Preferences
{
	public class PreferencesWindowVM : ZBindable
	{
		private bool IsInitializing { get; set; }

		public PreferencesWindowVM(IUserSettingsService usss, ISSEProcessor sse)
		{
			this._userSettingsService = usss;
			this._pagesVMs = new List<PreferencesBaseVM>();
			this._prefGeneral = new PreferencesGeneralVM();
			this._prefGamepads = new PreferencesGamepadsVM();
			this._prefInputDevices = new PreferencesInputDevicesVM();
			this._prefConfirmation = new ConfirmationPageVM();
			this._prefActivators = new ActivatorsPageVM();
			this._prefSlots = new PreferencesSlotsChangePageVM();
			this._prefConfigs = new PreferencesConfigsVM();
			this._prefBlacklist = new PreferencesBlacklistVM();
			this._prefTrayAgent = new PreferencesTrayAgentVM();
			this._prefOverlay = new PreferencesOverlayVM();
			this._prefLEDSettings = new PreferencesLEDSettingsVM();
			this._prefExternalDevices = new PreferencesExternalDevicesVM();
			this._prefHttp = new PreferencesHttpVM(sse);
			this._pagesVMs.Add(this._prefGeneral);
			this._pagesVMs.Add(this._prefTrayAgent);
			this._pagesVMs.Add(this._prefGamepads);
			this._pagesVMs.Add(this._prefInputDevices);
			this._pagesVMs.Add(this._prefConfirmation);
			this._pagesVMs.Add(this._prefActivators);
			this._pagesVMs.Add(this._prefSlots);
			this._pagesVMs.Add(this._prefConfigs);
			this._pagesVMs.Add(this._prefBlacklist);
			this._pagesVMs.Add(this._prefLEDSettings);
			this._pagesVMs.Add(this._prefExternalDevices);
			this._pagesVMs.Add(this._prefHttp);
			this._pagesVMs.Add(this._prefOverlay);
			foreach (PreferencesBaseVM preferencesBaseVM in this._pagesVMs)
			{
				preferencesBaseVM.OptionChanged += delegate
				{
					if (!this.IsInitializing)
					{
						this.IsOptionChanged = true;
						this.OnPropertyChanged("Description");
					}
				};
				preferencesBaseVM.DescriptionChanged += delegate
				{
					this.OnPropertyChanged("Description");
				};
				preferencesBaseVM.RequiredEnableRemap += delegate
				{
					this._isRequiredEnableRemap = true;
				};
				preferencesBaseVM.RequiredSteamLizardReApply += delegate
				{
					this._isRequiredSteamLizardReApply = true;
				};
				preferencesBaseVM.RequiredInputDeviceRefresh += delegate
				{
					this._isRequiredInputDeviceRefresh = true;
				};
			}
		}

		public async void Initialize()
		{
			this.IsInitializing = true;
			foreach (PreferencesBaseVM preferencesBaseVM in this._pagesVMs)
			{
				await preferencesBaseVM.Initialize();
			}
			List<PreferencesBaseVM>.Enumerator enumerator = default(List<PreferencesBaseVM>.Enumerator);
			this.IsInitializing = false;
			this.IsOptionChanged = false;
			this._isRequiredEnableRemap = false;
			this._isRequiredSteamLizardReApply = false;
			this._isRequiredInputDeviceRefresh = false;
		}

		public void Refresh()
		{
			foreach (PreferencesBaseVM preferencesBaseVM in this._pagesVMs)
			{
				preferencesBaseVM.Refresh();
			}
		}

		public void SelectPage(Type selectPage)
		{
			int num = 0;
			using (List<PreferencesBaseVM>.Enumerator enumerator = this._pagesVMs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.GetType() == selectPage)
					{
						this.SelectedPage = num;
						break;
					}
					num++;
				}
			}
		}

		public PreferencesGeneralVM PreferencesGeneralVM
		{
			get
			{
				return this._prefGeneral;
			}
		}

		public PreferencesGamepadsVM PreferencesGamepadsVM
		{
			get
			{
				return this._prefGamepads;
			}
		}

		public PreferencesInputDevicesVM PreferencesInputDevicesVM
		{
			get
			{
				return this._prefInputDevices;
			}
		}

		public ConfirmationPageVM ConfirmationPageVM
		{
			get
			{
				return this._prefConfirmation;
			}
		}

		public ActivatorsPageVM ActivatorsPageVM
		{
			get
			{
				return this._prefActivators;
			}
		}

		public PreferencesSlotsChangePageVM PreferencesSlotsChangePageVM
		{
			get
			{
				return this._prefSlots;
			}
		}

		public PreferencesConfigsVM PreferencesConfigsVM
		{
			get
			{
				return this._prefConfigs;
			}
		}

		public PreferencesBlacklistVM PreferencesBlacklistVM
		{
			get
			{
				return this._prefBlacklist;
			}
		}

		public PreferencesLEDSettingsVM PreferencesLEDSettingsVM
		{
			get
			{
				return this._prefLEDSettings;
			}
		}

		public PreferencesTrayAgentVM PreferencesTrayAgentVM
		{
			get
			{
				return this._prefTrayAgent;
			}
		}

		public PreferencesOverlayVM PreferencesOverlayVM
		{
			get
			{
				return this._prefOverlay;
			}
		}

		public PreferencesExternalDevicesVM PreferencesExternalDevicesVM
		{
			get
			{
				return this._prefExternalDevices;
			}
		}

		public PreferencesHttpVM PreferencesHttpVM
		{
			get
			{
				return this._prefHttp;
			}
		}

		public ILicensingService LicensingService
		{
			get
			{
				return App.LicensingService;
			}
		}

		public object MainContentVM
		{
			get
			{
				return App.MainContentVM;
			}
		}

		public int SelectedPage
		{
			get
			{
				return this._selectedPage;
			}
			set
			{
				if (this.SetProperty<int>(ref this._selectedPage, value, "SelectedPage"))
				{
					this.OnPropertyChanged("Description");
					this.OnPropertyChanged("CurrentPage");
				}
			}
		}

		public PreferencesBaseVM CurrentPage
		{
			get
			{
				return this._pagesVMs[this.SelectedPage];
			}
		}

		public bool IsOptionChanged
		{
			get
			{
				return this._isOptionChanged;
			}
			set
			{
				this.SetProperty<bool>(ref this._isOptionChanged, value, "IsOptionChanged");
			}
		}

		public Localizable Description
		{
			get
			{
				return this._pagesVMs[this.SelectedPage].Description;
			}
		}

		public void ResetChanges()
		{
			this.PreferencesLEDSettingsVM.UserSettingsService.Load();
		}

		public async Task ApplyChanges()
		{
			bool success = true;
			foreach (PreferencesBaseVM preferencesBaseVM in this._pagesVMs)
			{
				bool flag = await preferencesBaseVM.ApplyChanges();
				success = flag && success;
			}
			List<PreferencesBaseVM>.Enumerator enumerator = default(List<PreferencesBaseVM>.Enumerator);
			await this._userSettingsService.Save();
			if (success)
			{
				this.IsOptionChanged = false;
			}
			if (this._isRequiredInputDeviceRefresh)
			{
				await App.GamepadService.RefreshInputDevices();
				this._isRequiredInputDeviceRefresh = false;
			}
			if (this._isRequiredSteamLizardReApply)
			{
				await App.GamepadService.ReCompileSteamLizardProfile();
				this._isRequiredSteamLizardReApply = false;
			}
			if (this._isRequiredEnableRemap)
			{
				await App.HttpClientService.Gamepad.ReapplayRemap();
				this._isRequiredEnableRemap = false;
			}
			this.OnPropertyChanged("Description");
		}

		private List<PreferencesBaseVM> _pagesVMs;

		private PreferencesGeneralVM _prefGeneral;

		private PreferencesGamepadsVM _prefGamepads;

		private PreferencesInputDevicesVM _prefInputDevices;

		private ConfirmationPageVM _prefConfirmation;

		private ActivatorsPageVM _prefActivators;

		private PreferencesSlotsChangePageVM _prefSlots;

		private PreferencesConfigsVM _prefConfigs;

		private PreferencesBlacklistVM _prefBlacklist;

		private PreferencesLEDSettingsVM _prefLEDSettings;

		private PreferencesTrayAgentVM _prefTrayAgent;

		private PreferencesOverlayVM _prefOverlay;

		private PreferencesExternalDevicesVM _prefExternalDevices;

		private PreferencesHttpVM _prefHttp;

		private IUserSettingsService _userSettingsService;

		private int _selectedPage;

		private bool _isOptionChanged;

		private bool _isRequiredEnableRemap;

		private bool _isRequiredSteamLizardReApply;

		private bool _isRequiredInputDeviceRefresh;
	}
}
