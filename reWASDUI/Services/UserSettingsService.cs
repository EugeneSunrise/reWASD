using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using DTOverlay;
using Microsoft.Win32;
using Prism.Events;
using reWASDCommon.Infrastructure;
using reWASDUI.Infrastructure;
using XBEliteWPF.Infrastructure.LED;
using XBEliteWPF.Services.Interfaces;

namespace reWASDUI.Services
{
	public class UserSettingsService : ZBindableBase, IUserSettingsService
	{
		public Language ActiveLanguage
		{
			get
			{
				return LocalizationManager.Instance.GetActiveLanguage();
			}
		}

		public bool NotHideShift
		{
			get
			{
				return this._notHideShift;
			}
			set
			{
				this.SetProperty<bool>(ref this._notHideShift, value, "NotHideShift");
			}
		}

		public bool ShowShiftIsChangedToggle
		{
			get
			{
				return this._showShiftIsChangedToggle;
			}
			set
			{
				this.SetProperty<bool>(ref this._showShiftIsChangedToggle, value, "ShowShiftIsChangedToggle");
			}
		}

		public bool ShowShiftIsChanged
		{
			get
			{
				return this._showShiftIsChanged;
			}
			set
			{
				this.SetProperty<bool>(ref this._showShiftIsChanged, value, "ShowShiftIsChanged");
			}
		}

		public string DirectX_Apps
		{
			get
			{
				return this._directX_Apps;
			}
			set
			{
				this.SetProperty<string>(ref this._directX_Apps, value, "DirectX_Apps");
			}
		}

		public double MessagesWidowScale
		{
			get
			{
				return this._messagesWidowScale;
			}
			set
			{
				this.SetProperty<double>(ref this._messagesWidowScale, value, "MessagesWidowScale");
			}
		}

		public double GamepadWidowScale
		{
			get
			{
				return this._gamepadWidowScale;
			}
			set
			{
				this.SetProperty<double>(ref this._gamepadWidowScale, value, "GamepadWidowScale");
			}
		}

		public double RemapWidowScale
		{
			get
			{
				return this._remapWidowScale;
			}
			set
			{
				this.SetProperty<double>(ref this._remapWidowScale, value, "RemapWidowScale");
			}
		}

		public bool RestoreXBOXEliteSlot
		{
			get
			{
				return this._restoreXBOXEliteSlot;
			}
			set
			{
				this.SetProperty<bool>(ref this._restoreXBOXEliteSlot, value, "RestoreXBOXEliteSlot");
			}
		}

		public bool ShowDirectXOverlay
		{
			get
			{
				return this._showDirectXOverlay;
			}
			set
			{
				this.SetProperty<bool>(ref this._showDirectXOverlay, value, "ShowDirectXOverlay");
			}
		}

		public bool ShowMenuOverlay
		{
			get
			{
				return this._showMenuOverlay;
			}
			set
			{
				this.SetProperty<bool>(ref this._showMenuOverlay, value, "ShowMenuOverlay");
			}
		}

		public bool IsOverlayEnable
		{
			get
			{
				return this._isOverlayEnable;
			}
			set
			{
				this.SetProperty<bool>(ref this._isOverlayEnable, value, "IsOverlayEnable");
			}
		}

		public bool IsOverlayShowMappingsEnable
		{
			get
			{
				return this._isOverlayShowMappingsEnable;
			}
			set
			{
				this.SetProperty<bool>(ref this._isOverlayShowMappingsEnable, value, "IsOverlayShowMappingsEnable");
			}
		}

		public bool IsOverlayShowGamepadEnable
		{
			get
			{
				return this._isOverlayShowGamepadEnable;
			}
			set
			{
				this.SetProperty<bool>(ref this._isOverlayShowGamepadEnable, value, "IsOverlayShowGamepadEnable");
			}
		}

		public bool IsHidePhysicalController
		{
			get
			{
				return this._isHidePhysicalController;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isHidePhysicalController, value, "IsHidePhysicalController"))
				{
					this.RaiseOnHidePhysicalControllerChanged(null, null);
				}
				if (!value)
				{
					this.IsHidePhysicalXboxOneOrEliteController = false;
				}
			}
		}

		public bool IsHidePhysicalXboxOneOrEliteController
		{
			get
			{
				return this._isHidePhysicalXboxOneOrEliteController;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isHidePhysicalXboxOneOrEliteController, value, "IsHidePhysicalXboxOneOrEliteController"))
				{
					this.RaiseOnHidePhysicalControllerChanged(null, null);
				}
			}
		}

		public bool EmulateLizardConfigForSteam
		{
			get
			{
				return this._emulateLizardConfigForSteam;
			}
			set
			{
				this.SetProperty<bool>(ref this._emulateLizardConfigForSteam, value, "EmulateLizardConfigForSteam");
			}
		}

		public bool EmulateLizardConfigForSteamDeck
		{
			get
			{
				return this._emulateLizardConfigForSteamDeck;
			}
			set
			{
				this.SetProperty<bool>(ref this._emulateLizardConfigForSteamDeck, value, "EmulateLizardConfigForSteamDeck");
			}
		}

		public AlignType OverlayMenuAlign
		{
			get
			{
				return this._overlayAlign;
			}
			set
			{
				this.SetProperty<AlignType>(ref this._overlayAlign, value, "OverlayMenuAlign");
			}
		}

		public bool IsShowOverlayMappings
		{
			get
			{
				return this._isShowOverlayMappings;
			}
			set
			{
				this.SetProperty<bool>(ref this._isShowOverlayMappings, value, "IsShowOverlayMappings");
			}
		}

		public bool IsShowOverlayDescriptions
		{
			get
			{
				return this._isShowOverlayDescriptions;
			}
			set
			{
				this.SetProperty<bool>(ref this._isShowOverlayDescriptions, value, "IsShowOverlayDescriptions");
			}
		}

		public int OverlayScale
		{
			get
			{
				return this._overlayScale;
			}
			set
			{
				this.SetProperty<int>(ref this._overlayScale, value, "OverlayScale");
			}
		}

		public string OverlayMenuSelectedMonitor
		{
			get
			{
				return this._overlaySelectedMonitor;
			}
			set
			{
				this.SetProperty<string>(ref this._overlaySelectedMonitor, value, "OverlayMenuSelectedMonitor");
			}
		}

		public string ConfigsFolderPath
		{
			get
			{
				return this._configsFolderPath;
			}
			set
			{
				this.SetProperty<string>(ref this._configsFolderPath, value, "ConfigsFolderPath");
			}
		}

		public string PresetsFolderPath
		{
			get
			{
				return this._presetsFolderPath;
			}
			set
			{
				this.SetProperty<string>(ref this._presetsFolderPath, value, "PresetsFolderPath");
			}
		}

		public string ScreenshotsFolderPath
		{
			get
			{
				return this._screenshotsFolderPath;
			}
			set
			{
				this.SetProperty<string>(ref this._screenshotsFolderPath, value, "ScreenshotsFolderPath");
			}
		}

		public bool SendRumbleToGamepad
		{
			get
			{
				return this._sendRumbleToGamepad;
			}
			set
			{
				this.SetProperty<bool>(ref this._sendRumbleToGamepad, value, "SendRumbleToGamepad");
			}
		}

		public bool TurnOffControllerOption
		{
			get
			{
				return this._turnOffControllerOption;
			}
			set
			{
				this.SetProperty<bool>(ref this._turnOffControllerOption, value, "TurnOffControllerOption");
			}
		}

		public int TurnOffControllerTimeout
		{
			get
			{
				return this._turnOffControllerTimeout;
			}
			set
			{
				this.SetProperty<int>(ref this._turnOffControllerTimeout, value, "TurnOffControllerTimeout");
			}
		}

		public bool ExternalDeviceOverwritePrevConfig
		{
			get
			{
				return this._externalDeviceOverwritePrevConfig;
			}
			set
			{
				this.SetProperty<bool>(ref this._externalDeviceOverwritePrevConfig, value, "ExternalDeviceOverwritePrevConfig");
			}
		}

		public bool IsLedSettingsEnabled
		{
			get
			{
				return this._isLedSettingsEnabled;
			}
			set
			{
				this.SetProperty<bool>(ref this._isLedSettingsEnabled, value, "IsLedSettingsEnabled");
			}
		}

		public LEDSettingsGlobal PerDeviceGlobalLedSettings
		{
			get
			{
				return this._perDeviceGlobalLedSettings;
			}
			set
			{
				LEDSettingsGlobal perDeviceGlobalLedSettings = this._perDeviceGlobalLedSettings;
				if (this.SetProperty<LEDSettingsGlobal>(ref this._perDeviceGlobalLedSettings, value, "PerDeviceGlobalLedSettings"))
				{
					if (perDeviceGlobalLedSettings != null)
					{
						perDeviceGlobalLedSettings.OnLEDSettingsChanged -= this.RaiseOnLEDSettingsChanged;
					}
					if (this._perDeviceGlobalLedSettings != null)
					{
						this._perDeviceGlobalLedSettings.OnLEDSettingsChanged += this.RaiseOnLEDSettingsChanged;
					}
					this.RaiseOnLEDSettingsChanged(null, null);
				}
			}
		}

		public bool UsePhysicalUSBHubOption
		{
			get
			{
				RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\hidgamemap\\Parameters", false);
				int num = 1;
				if (registryKey != null)
				{
					try
					{
						num = (int)registryKey.GetValue("VirtualUsbHubEnabled", 1);
					}
					catch
					{
					}
					registryKey.Close();
				}
				return num == 1;
			}
		}

		public event EventHandler OnOverlaySettingsChanged;

		public void RaiseOverlayDSettingsChanged(object sender, EventArgs e)
		{
			EventHandler onOverlaySettingsChanged = this.OnOverlaySettingsChanged;
			if (onOverlaySettingsChanged == null)
			{
				return;
			}
			onOverlaySettingsChanged(this, EventArgs.Empty);
		}

		public event EventHandler OnLEDSettingsChanged;

		public void RaiseOnLEDSettingsChanged(object sender, EventArgs e)
		{
			EventHandler onLEDSettingsChanged = this.OnLEDSettingsChanged;
			if (onLEDSettingsChanged == null)
			{
				return;
			}
			onLEDSettingsChanged(this, EventArgs.Empty);
		}

		public event EventHandler OnHidePhysicalController;

		public void RaiseOnHidePhysicalControllerChanged(object sender, EventArgs e)
		{
			EventHandler onHidePhysicalController = this.OnHidePhysicalController;
			if (onHidePhysicalController == null)
			{
				return;
			}
			onHidePhysicalController(this, EventArgs.Empty);
		}

		public UserSettingsService(IEventAggregator ea)
		{
			this.Load();
			TranslationManager.Instance.LanguageChanged += delegate([Nullable(2)] object sender, EventArgs args)
			{
				this.OnPropertyChanged("ActiveLanguage");
			};
			this._eventAggregator = ea;
			this._eventAggregator.GetEvent<PreferencesChanged>().Subscribe(new Action<object>(this.OnPreferencesChanged));
		}

		private void OnPreferencesChanged(object obj)
		{
		}

		public async Task Save()
		{
			RegistryHelper.SetString("Config", "ConfigsFolderPath", this.ConfigsFolderPath);
			RegistryHelper.SetString("Config", "PresetsFolderPath", this.PresetsFolderPath);
			RegistryHelper.SetString("Config", "ScreenshotsFolderPath", this.ScreenshotsFolderPath);
			RegistryHelper.SetValue("Config", "SendRumbleToGamepad", Convert.ToInt32(this.SendRumbleToGamepad));
			RegistryHelper.SetValue("Config", "TurnOffControllerTimeout", this.TurnOffControllerTimeout);
			RegistryHelper.SetValue("Config", "TurnOffControllerOption", Convert.ToInt32(this.TurnOffControllerOption));
			RegistryHelper.SetValue("Config", "HidePhysicalControllerOnVirtualCreation", Convert.ToInt32(this.IsHidePhysicalController));
			RegistryHelper.SetValue("Config", "HidePhysicalXboxOneOrEliteControllerOnVirtualCreation", Convert.ToInt32(this.IsHidePhysicalXboxOneOrEliteController));
			RegistryHelper.SetValue("Config", "RestoreXBOXEliteSlot", Convert.ToInt32(this.RestoreXBOXEliteSlot));
			RegistryHelper.SetValue("Config", "EmulateLizardConfigForSteam", Convert.ToInt32(this.EmulateLizardConfigForSteam));
			RegistryHelper.SetValue("Config", "EmulateLizardConfigForSteamDeck", Convert.ToInt32(this.EmulateLizardConfigForSteamDeck));
			RegistryHelper.SetValue("Preferences", "IsLedSettingsEnabled", this.IsLedSettingsEnabled);
			RegistryHelper.SetValue("Overlay", "ShowOverlay", this.IsOverlayEnable);
			RegistryHelper.SetValue("Overlay", "ShowMappings", this.IsOverlayShowMappingsEnable);
			RegistryHelper.SetValue("Overlay", "ShowDirectXOverlay", Convert.ToInt32(this.ShowDirectXOverlay));
			RegistryHelper.SetValue("Overlay", "ShowMenuOverlay", Convert.ToInt32(this.ShowMenuOverlay));
			RegistryHelper.SetString("Overlay", "DirectX_Apps", this.DirectX_Apps);
			RegistryHelper.SetValue("Overlay", "NotHideShift", Convert.ToInt32(this.NotHideShift));
			RegistryHelper.SetValue("Overlay", "ShowShiftIsChangedToggle", Convert.ToInt32(this.ShowShiftIsChangedToggle));
			RegistryHelper.SetValue("Overlay", "ShowShiftIsChanged", Convert.ToInt32(this.ShowShiftIsChanged));
			RegistryHelper.SetValue("Overlay", "MessagesWidowScale", (long)(this.MessagesWidowScale * 100.0));
			RegistryHelper.SetValue("Overlay", "GamepadWidowScale", (long)(this.GamepadWidowScale * 100.0));
			RegistryHelper.SetValue("Overlay", "RemapWidowScale", (long)(this.RemapWidowScale * 100.0));
			RegistryHelper.SetValue("Overlay\\OverlayCircle", "OverlayAlign", Convert.ToInt32(this.OverlayMenuAlign));
			RegistryHelper.SetValue("Overlay\\OverlayCircle", "ShowOverlayMappings", Convert.ToInt32(this.IsShowOverlayMappings));
			RegistryHelper.SetValue("Overlay\\OverlayCircle", "ShowOverlayDescriptions", Convert.ToInt32(this.IsShowOverlayDescriptions));
			RegistryHelper.SetValue("Overlay\\OverlayCircle", "Scale", Convert.ToInt32(this.OverlayScale));
			RegistryHelper.SetString("Overlay\\OverlayCircle", "OverlayMonitor", this.OverlayMenuSelectedMonitor);
			bool flag = Convert.ToBoolean(RegistryHelper.GetValue("Config", "ExternalDeviceOverwritePrevConfig", 0, false));
			RegistryHelper.SetValue("Config", "ExternalDeviceOverwritePrevConfig", Convert.ToInt32(this.ExternalDeviceOverwritePrevConfig));
			if (flag != this.ExternalDeviceOverwritePrevConfig)
			{
				App.GamepadService.ExternalDeviceRelationsHelper.Refresh();
			}
			await App.HttpClientService.Gamepad.SavePerDeviceGlobalLedSettings(this.PerDeviceGlobalLedSettings);
			await App.HttpClientService.Engine.RequestReloadUserSettings();
			this._eventAggregator.GetEvent<PreferencesChanged>().Publish(null);
		}

		private bool IsPathCorrect(string path)
		{
			path = path.Trim();
			return !string.IsNullOrEmpty(path) && !(path.Substring(0, 1) == ".") && !(path.Substring(0, 1) == "\\");
		}

		public async void Load()
		{
			this.ConfigsFolderPath = RegistryHelper.GetString("Config", "ConfigsFolderPath", Path.Combine(Constants.COMMON_DOCUMENTS_DIRECTORY_PATH, "Profiles"), false);
			if (!this.IsPathCorrect(this.ConfigsFolderPath))
			{
				this.ConfigsFolderPath = Path.Combine(Constants.COMMON_DOCUMENTS_DIRECTORY_PATH, "Profiles");
			}
			this.PresetsFolderPath = RegistryHelper.GetString("Config", "PresetsFolderPath", Path.Combine(Constants.COMMON_DOCUMENTS_DIRECTORY_PATH, "Presets"), false);
			if (!this.IsPathCorrect(this.PresetsFolderPath))
			{
				this.PresetsFolderPath = Path.Combine(Constants.COMMON_DOCUMENTS_DIRECTORY_PATH, "Presets");
			}
			this.ScreenshotsFolderPath = RegistryHelper.GetString("Config", "ScreenshotsFolderPath", Path.Combine(Constants.COMMON_DOCUMENTS_DIRECTORY_PATH, "Screenshots"), false);
			if (!this.IsPathCorrect(this.ScreenshotsFolderPath))
			{
				this.ScreenshotsFolderPath = Path.Combine(Constants.COMMON_DOCUMENTS_DIRECTORY_PATH, "Screenshots");
			}
			this.SendRumbleToGamepad = Convert.ToBoolean(RegistryHelper.GetValue("Config", "SendRumbleToGamepad", 1, false));
			this.ExternalDeviceOverwritePrevConfig = Convert.ToBoolean(RegistryHelper.GetValue("Config", "ExternalDeviceOverwritePrevConfig", 0, false));
			this.TurnOffControllerOption = Convert.ToBoolean(RegistryHelper.GetValue("Config", "TurnOffControllerOption", 0, false));
			this.TurnOffControllerTimeout = RegistryHelper.GetValue("Config", "TurnOffControllerTimeout", 15, false);
			this.IsHidePhysicalController = Convert.ToBoolean(RegistryHelper.GetValue("Config", "HidePhysicalControllerOnVirtualCreation", 1, false));
			this.IsHidePhysicalXboxOneOrEliteController = Convert.ToBoolean(RegistryHelper.GetValue("Config", "HidePhysicalXboxOneOrEliteControllerOnVirtualCreation", 0, false));
			this.RestoreXBOXEliteSlot = Convert.ToBoolean(RegistryHelper.GetValue("Config", "RestoreXBOXEliteSlot", 1, false));
			this.EmulateLizardConfigForSteam = Convert.ToBoolean(RegistryHelper.GetValue("Config", "EmulateLizardConfigForSteam", 0, false));
			this.EmulateLizardConfigForSteamDeck = Convert.ToBoolean(RegistryHelper.GetValue("Config", "EmulateLizardConfigForSteamDeck", 1, false));
			this.OverlayMenuAlign = RegistryHelper.GetValue("Overlay\\OverlayCircle", "OverlayAlign", 4, false);
			this.OverlayScale = RegistryHelper.GetValue("Overlay\\OverlayCircle", "Scale", 200, false);
			this.IsShowOverlayMappings = Convert.ToBoolean(RegistryHelper.GetValue("Overlay\\OverlayCircle", "ShowOverlayMappings", 1, false));
			this.IsShowOverlayDescriptions = Convert.ToBoolean(RegistryHelper.GetValue("Overlay\\OverlayCircle", "ShowOverlayDescriptions", 1, false));
			this.OverlayMenuSelectedMonitor = RegistryHelper.GetString("Overlay\\OverlayCircle", "OverlayMonitor", "", false);
			try
			{
				this.IsLedSettingsEnabled = Convert.ToBoolean(RegistryHelper.GetValue("Preferences", "IsLedSettingsEnabled", 1, false));
				LEDSettingsGlobal ledsettingsGlobal = await App.HttpClientService.Gamepad.GetPerDeviceGlobalLedSettings();
				this.PerDeviceGlobalLedSettings = ledsettingsGlobal;
				LEDSettingsGlobal perDeviceGlobalLedSettings = this.PerDeviceGlobalLedSettings;
				if (perDeviceGlobalLedSettings != null)
				{
					perDeviceGlobalLedSettings.OnAfterLoaded();
				}
			}
			catch (Exception)
			{
			}
			this.ShowDirectXOverlay = Convert.ToBoolean(RegistryHelper.GetValue("Overlay", "ShowDirectXOverlay", 0, false));
			this.ShowMenuOverlay = Convert.ToBoolean(RegistryHelper.GetValue("Overlay", "ShowMenuOverlay", 1, false));
			this.DirectX_Apps = RegistryHelper.GetString("Overlay", "DirectX_Apps", "", false);
			this.IsOverlayEnable = Convert.ToBoolean(RegistryHelper.GetValue("Overlay", "ShowOverlay", 1, false));
			this.IsOverlayShowMappingsEnable = Convert.ToBoolean(RegistryHelper.GetValue("Overlay", "ShowMappings", 1, false));
			this.IsOverlayShowGamepadEnable = Convert.ToBoolean(RegistryHelper.GetValue("Overlay", "ShowGamepad", 1, false));
			this.NotHideShift = Convert.ToBoolean(RegistryHelper.GetValue("Overlay", "NotHideShift", 0, false));
			this.ShowShiftIsChanged = Convert.ToBoolean(RegistryHelper.GetValue("Overlay", "ShowShiftIsChanged", 1, false));
			this.ShowShiftIsChangedToggle = Convert.ToBoolean(RegistryHelper.GetValue("Overlay", "ShowShiftIsChangedToggle", 1, false));
			this.MessagesWidowScale = Convert.ToDouble(RegistryHelper.GetValue("Overlay", "MessagesWidowScale", 100L)) / 100.0;
			this.GamepadWidowScale = Convert.ToDouble(RegistryHelper.GetValue("Overlay", "GamepadWidowScale", 100L)) / 100.0;
			this.RemapWidowScale = Convert.ToDouble(RegistryHelper.GetValue("Overlay", "RemapWidowScale", 100L)) / 100.0;
		}

		private IEventAggregator _eventAggregator;

		private string _screenshotsFolderPath;

		private string _configsFolderPath;

		private string _presetsFolderPath;

		private bool _sendRumbleToGamepad;

		private bool _turnOffControllerOption;

		private int _turnOffControllerTimeout;

		private bool _isLedSettingsEnabled;

		private LEDSettingsGlobal _perDeviceGlobalLedSettings;

		private bool _externalDeviceOverwritePrevConfig;

		private bool _isHidePhysicalController;

		private bool _isHidePhysicalXboxOneOrEliteController;

		private bool _restoreXBOXEliteSlot;

		private bool _isOverlayEnable;

		private bool _showDirectXOverlay;

		private bool _showMenuOverlay;

		private string _directX_Apps;

		private bool _isOverlayShowMappingsEnable;

		private bool _isOverlayShowGamepadEnable;

		private bool _emulateLizardConfigForSteam;

		private bool _emulateLizardConfigForSteamDeck;

		private AlignType _overlayAlign;

		private bool _isShowOverlayMappings;

		private bool _isShowOverlayDescriptions;

		private int _overlayScale;

		private string _overlaySelectedMonitor;

		private bool _notHideShift;

		private bool _showShiftIsChangedToggle;

		private bool _showShiftIsChanged;

		private double _messagesWidowScale;

		private double _gamepadWidowScale;

		private double _remapWidowScale;
	}
}
