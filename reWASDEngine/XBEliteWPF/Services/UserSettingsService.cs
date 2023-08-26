using System;
using System.IO;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Utils;
using DTOverlay;
using Microsoft.Win32;
using Prism.Events;
using reWASDCommon.Infrastructure;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Infrastructure.RadialMenu.Utils;
using reWASDEngine;
using XBEliteWPF.DataModels.GamepadActiveProfiles;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.LED;
using XBEliteWPF.Services.Interfaces;

namespace XBEliteWPF.Services
{
	public class UserSettingsService : IUserSettingsService
	{
		public bool NotHideShift { get; set; }

		public bool ShowShiftIsChangedToggle { get; set; }

		public bool ShowShiftIsChanged { get; set; }

		public bool ShowDirectXOverlay { get; set; }

		public bool ShowMenuOverlay { get; set; }

		public bool RestoreXBOXEliteSlot { get; set; }

		public AlignType OverlayMenuAlign { get; set; }

		public bool IsShowOverlayMappings { get; set; }

		public bool IsShowOverlayDescriptions { get; set; }

		public int OverlayScale { get; set; }

		public string OverlayMenuSelectedMonitor { get; set; }

		public double MessagesWidowScale { get; set; }

		public double GamepadWidowScale { get; set; }

		public double RemapWidowScale { get; set; }

		public string DirectX_Apps { get; set; }

		public bool IsOverlayEnable { get; set; }

		public bool IsOverlayShowMappingsEnable { get; set; }

		public bool IsOverlayShowGamepadEnable { get; set; }

		public bool IsHidePhysicalController { get; set; }

		public bool IsHidePhysicalXboxOneOrEliteController { get; set; }

		public string ScreenshotsFolderPath { get; set; }

		public bool SendRumbleToGamepad { get; set; }

		public bool TurnOffControllerOption { get; set; }

		public int TurnOffControllerTimeout { get; set; }

		public bool ExternalDeviceOverwritePrevConfig { get; set; }

		public bool EmulateLizardConfigForSteam { get; set; }

		public bool EmulateLizardConfigForSteamDeck { get; set; }

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

		public string ConfigsFolderPath
		{
			get
			{
				return this._configsFolderPath;
			}
			set
			{
				if (!string.IsNullOrEmpty(this._configsFolderPath) && this._configsFolderPath != value)
				{
					Engine.EventAggregator.GetEvent<ConfigsFolderChanged>().Publish(value);
				}
				this._configsFolderPath = value;
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
				if (!string.IsNullOrEmpty(this._presetsFolderPath) && this._presetsFolderPath != value)
				{
					Engine.EventAggregator.GetEvent<PresetsFolderChanged>().Publish(value);
				}
				this._presetsFolderPath = value;
				RadialMenuPresetHelper.SetPresetPath(this._presetsFolderPath);
			}
		}

		public bool IsLedSettingsEnabled { get; set; }

		public LEDSettingsGlobal PerDeviceGlobalLedSettings
		{
			get
			{
				return this._perDeviceGlobalLedSettings;
			}
			set
			{
				LEDSettingsGlobal perDeviceGlobalLedSettings = this._perDeviceGlobalLedSettings;
				if (this._perDeviceGlobalLedSettings != value)
				{
					this._perDeviceGlobalLedSettings = value;
					if (perDeviceGlobalLedSettings != null)
					{
						perDeviceGlobalLedSettings.OnLEDSettingsChanged -= this.RaiseOnLEDSettingsChanged;
					}
					if (this._perDeviceGlobalLedSettings != null)
					{
						this._perDeviceGlobalLedSettings.OnLEDSettingsChanged += this.RaiseOnLEDSettingsChanged;
					}
				}
			}
		}

		public event EventHandler OnLEDSettingsChanged;

		public event EventHandler OnHidePhysicalController;

		public void RaiseOnLEDSettingsChanged(object sender, EventArgs e)
		{
			EventHandler onLEDSettingsChanged = this.OnLEDSettingsChanged;
			if (onLEDSettingsChanged == null)
			{
				return;
			}
			onLEDSettingsChanged(this, EventArgs.Empty);
		}

		public UserSettingsService(IEventAggregator ea)
		{
			this.Load();
			this.RaiseOnLEDSettingsChanged(null, null);
			this._eventAggregator = ea;
			this._eventAggregator.GetEvent<PreferencesChanged>().Subscribe(new Action<object>(this.OnPreferencesChanged));
		}

		private void OnPreferencesChanged(object obj)
		{
			this.RaiseOnLEDSettingsChanged(null, null);
		}

		public Task Save()
		{
			return null;
		}

		private bool IsPathCorrect(string path)
		{
			path = path.Trim();
			return !string.IsNullOrEmpty(path) && !(path.Substring(0, 1) == ".") && !(path.Substring(0, 1) == "\\");
		}

		public void Load()
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
			this.OverlayMenuAlign = RegistryHelper.GetValue("Overlay\\OverlayCircle", "OverlayAlign", 4, false);
			this.OverlayScale = RegistryHelper.GetValue("Overlay\\OverlayCircle", "Scale", 200, false);
			this.IsShowOverlayMappings = Convert.ToBoolean(RegistryHelper.GetValue("Overlay\\OverlayCircle", "ShowOverlayMappings", 1, false));
			this.IsShowOverlayDescriptions = Convert.ToBoolean(RegistryHelper.GetValue("Overlay\\OverlayCircle", "ShowOverlayDescriptions", 1, false));
			this.OverlayMenuSelectedMonitor = RegistryHelper.GetString("Overlay\\OverlayCircle", "OverlayMonitor", "", false);
			this.EmulateLizardConfigForSteam = Convert.ToBoolean(RegistryHelper.GetValue("Config", "EmulateLizardConfigForSteam", 0, false));
			this.EmulateLizardConfigForSteamDeck = Convert.ToBoolean(RegistryHelper.GetValue("Config", "EmulateLizardConfigForSteamDeck", 1, false));
			try
			{
				this.IsLedSettingsEnabled = Convert.ToBoolean(RegistryHelper.GetValue("Preferences", "IsLedSettingsEnabled", 1, false));
				this.PerDeviceGlobalLedSettings = AppDataUserImpersonalizedSerializable<LEDSettingsGlobal>.Load();
				this.FixOldVersionData();
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

		private void FixOldVersionData()
		{
			this.PerDeviceGlobalLedSettings.LEDSettingsGlobalDictionary.ForEachValue(delegate(LEDSettingsGlobalPerDevice value)
			{
				foreach (object obj in Enum.GetValues(typeof(Slot)))
				{
					Slot slot = (Slot)obj;
					ILEDSettingsPerCollectionContainerExtensions.AddDefaultLEDSettingsPerCollectionsIfNeeded(value.LedSlotSettings[slot], value.Device, false);
					if (value.HasMicrophoneLed)
					{
						ILEDSettingsPerCollectionContainerExtensions.AddDefaultLEDSettingsPerCollectionsIfNeeded(value.MicrophoneLedSlotSettings[slot], value.Device, true);
					}
				}
			});
		}

		private IEventAggregator _eventAggregator;

		private LEDSettingsGlobal _perDeviceGlobalLedSettings;

		private string _configsFolderPath;

		private string _presetsFolderPath;
	}
}
