using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Attributes;
using Prism.Commands;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Utils;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.ViewModels.Preferences.Base;

namespace reWASDUI.ViewModels.Preferences
{
	public class PreferencesGamepadsVM : PreferencesBaseVM
	{
		public ObservableCollection<Language> Languages { get; set; }

		public bool IsWindows10OrHigher
		{
			get
			{
				return UtilsNative.IsWindows10OrHigher();
			}
		}

		public bool SteamDeckIsPresent
		{
			get
			{
				return App.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM item) => item.ControllerTypeEnums.Contains(63)) != null;
			}
		}

		public bool DisablePaddlesOnLocked
		{
			get
			{
				return this._disablePaddlesOnLocked;
			}
			set
			{
				if (value == this._disablePaddlesOnLocked)
				{
					return;
				}
				this._disablePaddlesOnLocked = value;
				this.OnPropertyChanged("DisablePaddlesOnLocked");
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
				if (value == this._emulateLizardConfigForSteam)
				{
					return;
				}
				this._emulateLizardConfigForSteam = value;
				this.OnPropertyChanged("EmulateLizardConfigForSteam");
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
				if (value == this._emulateLizardConfigForSteamDeck)
				{
					return;
				}
				this._emulateLizardConfigForSteamDeck = value;
				this.OnPropertyChanged("EmulateLizardConfigForSteamDeck");
			}
		}

		public bool StopMacroHotkey
		{
			get
			{
				return this._stopMacroHotkey;
			}
			set
			{
				if (value == this._stopMacroHotkey)
				{
					return;
				}
				this._stopMacroHotkey = value;
				this.OnPropertyChanged("StopMacroHotkey");
			}
		}

		public bool HidePhysicalControllerOnVirtualCreation
		{
			get
			{
				return this._hidePhysicalControllerOnVirtualCreation;
			}
			set
			{
				if (value == this._hidePhysicalControllerOnVirtualCreation)
				{
					return;
				}
				this._hidePhysicalControllerOnVirtualCreation = value;
				this.SetDescription();
				this.OnPropertyChanged("HidePhysicalControllerOnVirtualCreation");
				if (!value)
				{
					this.HidePhysicalXboxOneOrEliteControllerOnVirtualCreation = false;
				}
			}
		}

		public bool HidePhysicalXboxOneOrEliteControllerOnVirtualCreation
		{
			get
			{
				return this._hidePhysicalXboxOneOrEliteControllerOnVirtualCreation;
			}
			set
			{
				if (value == this._hidePhysicalXboxOneOrEliteControllerOnVirtualCreation)
				{
					return;
				}
				this._hidePhysicalXboxOneOrEliteControllerOnVirtualCreation = value;
				this.OnPropertyChanged("HidePhysicalXboxOneOrEliteControllerOnVirtualCreation");
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
				if (value == this._turnOffControllerOption)
				{
					return;
				}
				this._turnOffControllerOption = value;
				this.OnPropertyChanged("TurnOffControllerOption");
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
				if (value == this._turnOffControllerTimeout)
				{
					return;
				}
				this._turnOffControllerTimeout = value;
				this.OnPropertyChanged("TurnOffControllerTimeout");
			}
		}

		public bool IsGyroRollDefault
		{
			get
			{
				return this._isGyroRollDefault;
			}
			set
			{
				if (value == this._isGyroRollDefault)
				{
					return;
				}
				this._isGyroRollDefault = value;
				this.OnPropertyChanged("IsGyroRollDefault");
			}
		}

		public bool IsGyroYawDefault
		{
			get
			{
				return this._isGyroYawDefault;
			}
			set
			{
				if (value == this._isGyroYawDefault)
				{
					return;
				}
				this._isGyroYawDefault = value;
				this.OnPropertyChanged("IsGyroYawDefault");
			}
		}

		public bool IsGyroInitialStateOnDefault
		{
			get
			{
				return this._isGyroInitialStateOnDefault;
			}
			set
			{
				if (value == this._isGyroInitialStateOnDefault)
				{
					return;
				}
				this._isGyroInitialStateOnDefault = value;
				this.OnPropertyChanged("IsGyroInitialStateOnDefault");
			}
		}

		public bool IsGyroInitialStateOffDefault
		{
			get
			{
				return this._isGyroInitialStateOffDefault;
			}
			set
			{
				if (value == this._isGyroInitialStateOffDefault)
				{
					return;
				}
				this._isGyroInitialStateOffDefault = value;
				this.OnPropertyChanged("IsGyroInitialStateOffDefault");
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
				if (value == this._sendRumbleToGamepad)
				{
					return;
				}
				this._sendRumbleToGamepad = value;
				this.OnPropertyChanged("SendRumbleToGamepad");
			}
		}

		public byte SteamDeckLeftIntensity
		{
			get
			{
				return this._steamDeckLeftIntensity;
			}
			set
			{
				if (value == this._steamDeckLeftIntensity)
				{
					return;
				}
				this._steamDeckLeftIntensity = value;
				this.OnPropertyChanged("SteamDeckLeftIntensity");
			}
		}

		public int SteamDeckLeftIsStrongIntensity
		{
			get
			{
				return this._steamDeckLeftIsStrongIntensity;
			}
			set
			{
				if (value == this._steamDeckLeftIsStrongIntensity)
				{
					return;
				}
				this._steamDeckLeftIsStrongIntensity = value;
				this.OnPropertyChanged("SteamDeckLeftIsStrongIntensity");
			}
		}

		public byte SteamDeckRightIntensity
		{
			get
			{
				return this._steamDeckRightIntensity;
			}
			set
			{
				if (value == this._steamDeckRightIntensity)
				{
					return;
				}
				this._steamDeckRightIntensity = value;
				this.OnPropertyChanged("SteamDeckRightIntensity");
			}
		}

		public int SteamDeckRightIsStrongIntensity
		{
			get
			{
				return this._steamDeckRightIsStrongIntensity;
			}
			set
			{
				if (value == this._steamDeckRightIsStrongIntensity)
				{
					return;
				}
				this._steamDeckRightIsStrongIntensity = value;
				this.OnPropertyChanged("SteamDeckRightIsStrongIntensity");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public string CheckForUpdateText
		{
			get
			{
				return DTLocalization.GetString(App.LicensingService.NewVersionAvailable ? 11840 : 11130);
			}
		}

		public void CheckForUpdate()
		{
			App.LicensingService.CheckForUpdate();
			this.OnPropertyChanged("CheckForUpdateText");
		}

		public PreferencesGamepadsVM()
		{
			this.ResetSteamDeckToDefaultsCommand.ObservesProperty<byte>(Expression.Lambda<Func<byte>>(Expression.Property(Expression.Constant(this, typeof(PreferencesGamepadsVM)), methodof(PreferencesGamepadsVM.get_SteamDeckLeftIntensity())), Array.Empty<ParameterExpression>()));
			this.ResetSteamDeckToDefaultsCommand.ObservesProperty<byte>(Expression.Lambda<Func<byte>>(Expression.Property(Expression.Constant(this, typeof(PreferencesGamepadsVM)), methodof(PreferencesGamepadsVM.get_SteamDeckRightIntensity())), Array.Empty<ParameterExpression>()));
			this.ResetSteamDeckToDefaultsCommand.ObservesProperty<int>(Expression.Lambda<Func<int>>(Expression.Property(Expression.Constant(this, typeof(PreferencesGamepadsVM)), methodof(PreferencesGamepadsVM.get_SteamDeckLeftIsStrongIntensity())), Array.Empty<ParameterExpression>()));
			this.ResetSteamDeckToDefaultsCommand.ObservesProperty<int>(Expression.Lambda<Func<int>>(Expression.Property(Expression.Constant(this, typeof(PreferencesGamepadsVM)), methodof(PreferencesGamepadsVM.get_SteamDeckRightIsStrongIntensity())), Array.Empty<ParameterExpression>()));
		}

		public override Task Initialize()
		{
			this.SteamDeckLeftIntensity = (byte)RegistryHelper.GetValue("Config", "SteamDeckLeftIntensity", 7, false);
			this.SteamDeckLeftIsStrongIntensity = RegistryHelper.GetValue("Config", "SteamDeckLeftIsStrongIntensity", 1, false);
			this.SteamDeckRightIntensity = (byte)RegistryHelper.GetValue("Config", "SteamDeckRightIntensity", 7, false);
			this.SteamDeckRightIsStrongIntensity = RegistryHelper.GetValue("Config", "SteamDeckRightIsStrongIntensity", 1, false);
			this.DisablePaddlesOnLocked = Convert.ToBoolean(RegistryHelper.GetValue("Config", "DisablePaddlesOnLocked", 1, false));
			this.EmulateLizardConfigForSteam = base.UserSettingsService.EmulateLizardConfigForSteam;
			this.EmulateLizardConfigForSteamDeck = base.UserSettingsService.EmulateLizardConfigForSteamDeck;
			this.StopMacroHotkey = Convert.ToBoolean(RegistryHelper.GetValue("Config", "StopMacroHotkey", 1, false));
			this.HidePhysicalControllerOnVirtualCreation = base.UserSettingsService.IsHidePhysicalController;
			this.HidePhysicalXboxOneOrEliteControllerOnVirtualCreation = base.UserSettingsService.IsHidePhysicalXboxOneOrEliteController;
			this.IsGyroRollDefault = Convert.ToBoolean(RegistryHelper.GetValue("Config", "IsGyroRollDefault", 1, false));
			this.IsGyroYawDefault = Convert.ToBoolean(RegistryHelper.GetValue("Config", "IsGyroYawDefault", 0, false));
			this.IsGyroInitialStateOnDefault = Convert.ToBoolean(RegistryHelper.GetValue("Config", "IsGyroInitialStateOnDefault", 1, false));
			this.IsGyroInitialStateOffDefault = Convert.ToBoolean(RegistryHelper.GetValue("Config", "IsGyroInitialStateOffDefault", 0, false));
			this.SendRumbleToGamepad = base.UserSettingsService.SendRumbleToGamepad;
			this.TurnOffControllerOption = base.UserSettingsService.TurnOffControllerOption;
			this.TurnOffControllerTimeout = base.UserSettingsService.TurnOffControllerTimeout;
			this.SetDescription();
			this.OnPropertyChanged("SteamDeckIsPresent");
			return Task.CompletedTask;
		}

		public override async Task<bool> ApplyChanges()
		{
			bool oldValue_HidePhysicalControllerOnVirtualCreation = base.UserSettingsService.IsHidePhysicalController;
			bool oldValue_HidePhysicalXboxOneOrEliteControllerOnVirtualCreation = base.UserSettingsService.IsHidePhysicalXboxOneOrEliteController;
			bool oldValue_GyroRoll = Convert.ToBoolean(RegistryHelper.GetValue("Config", "IsGyroRollDefault", 1, false));
			bool oldValue_GyroInitialStateOn = Convert.ToBoolean(RegistryHelper.GetValue("Config", "IsGyroInitialStateOnDefault", 1, false));
			bool oldValue_StopMacroHotkey = Convert.ToBoolean(RegistryHelper.GetValue("Config", "StopMacroHotkey", 1, false));
			bool oldValue_EmulateLizardConfigForSteam = Convert.ToBoolean(RegistryHelper.GetValue("Config", "EmulateLizardConfigForSteam", 0, false));
			bool oldValue_EmulateLizardConfigForSteamDeck = Convert.ToBoolean(RegistryHelper.GetValue("Config", "EmulateLizardConfigForSteamDeck", 1, false));
			RegistryHelper.SetValue("Config", "DisablePaddlesOnLocked", Convert.ToInt32(this.DisablePaddlesOnLocked));
			RegistryHelper.SetValue("Config", "StopMacroHotkey", Convert.ToInt32(this.StopMacroHotkey));
			RegistryHelper.SetValue("Config", "IsGyroRollDefault", Convert.ToInt32(this.IsGyroRollDefault));
			RegistryHelper.SetValue("Config", "IsGyroYawDefault", Convert.ToInt32(this.IsGyroYawDefault));
			RegistryHelper.SetValue("Config", "IsGyroInitialStateOnDefault", Convert.ToInt32(this.IsGyroInitialStateOnDefault));
			RegistryHelper.SetValue("Config", "IsGyroInitialStateOffDefault", Convert.ToInt32(this.IsGyroInitialStateOffDefault));
			base.UserSettingsService.SendRumbleToGamepad = this.SendRumbleToGamepad;
			base.UserSettingsService.IsHidePhysicalController = this.HidePhysicalControllerOnVirtualCreation;
			base.UserSettingsService.IsHidePhysicalXboxOneOrEliteController = this.HidePhysicalXboxOneOrEliteControllerOnVirtualCreation;
			base.UserSettingsService.EmulateLizardConfigForSteam = this.EmulateLizardConfigForSteam;
			base.UserSettingsService.EmulateLizardConfigForSteamDeck = this.EmulateLizardConfigForSteamDeck;
			if (this.SteamDeckIsPresent && (this.SteamDeckLeftIntensity != (byte)RegistryHelper.GetValue("Config", "SteamDeckLeftIntensity", 7, false) || this.SteamDeckLeftIsStrongIntensity != RegistryHelper.GetValue("Config", "SteamDeckLeftIsStrongIntensity", 1, false) || this.SteamDeckRightIntensity != (byte)RegistryHelper.GetValue("Config", "SteamDeckRightIntensity", 7, false) || this.SteamDeckRightIsStrongIntensity != RegistryHelper.GetValue("Config", "SteamDeckRightIsStrongIntensity", 1, false)))
			{
				RegistryHelper.SetValue("Config", "SteamDeckLeftIntensity", (int)this.SteamDeckLeftIntensity);
				RegistryHelper.SetValue("Config", "SteamDeckRightIntensity", (int)this.SteamDeckRightIntensity);
				RegistryHelper.SetValue("Config", "SteamDeckLeftIsStrongIntensity", this.SteamDeckLeftIsStrongIntensity);
				RegistryHelper.SetValue("Config", "SteamDeckRightIsStrongIntensity", this.SteamDeckRightIsStrongIntensity);
				await App.HttpClientService.Gamepad.SteamDeckSetMotorIntensitySettings(Convert.ToBoolean(this.SteamDeckLeftIsStrongIntensity), this.SteamDeckLeftIntensity, Convert.ToBoolean(this.SteamDeckRightIsStrongIntensity), this.SteamDeckRightIntensity);
			}
			if (this.TurnOffControllerOption != base.UserSettingsService.TurnOffControllerOption || this.TurnOffControllerTimeout != base.UserSettingsService.TurnOffControllerTimeout)
			{
				base.UserSettingsService.TurnOffControllerOption = this.TurnOffControllerOption;
				base.UserSettingsService.TurnOffControllerTimeout = this.TurnOffControllerTimeout;
				base.FireRequiredEnableRemap();
			}
			if (oldValue_HidePhysicalControllerOnVirtualCreation != this.HidePhysicalControllerOnVirtualCreation)
			{
				base.FireRequiredEnableRemap();
			}
			if (oldValue_HidePhysicalXboxOneOrEliteControllerOnVirtualCreation != this.HidePhysicalXboxOneOrEliteControllerOnVirtualCreation)
			{
				base.FireRequiredEnableRemap();
			}
			if (oldValue_GyroRoll != this.IsGyroRollDefault)
			{
				base.FireRequiredEnableRemap();
			}
			if (oldValue_GyroInitialStateOn != this.IsGyroInitialStateOnDefault)
			{
				base.FireRequiredEnableRemap();
			}
			if (oldValue_StopMacroHotkey != this.StopMacroHotkey)
			{
				base.FireRequiredEnableRemap();
			}
			if (oldValue_EmulateLizardConfigForSteam != this.EmulateLizardConfigForSteam || oldValue_EmulateLizardConfigForSteamDeck != this.EmulateLizardConfigForSteamDeck)
			{
				base.FireRequiredSteamLizardReApply();
			}
			return true;
		}

		private void SetDescription()
		{
			base.Description = ((!this.HidePhysicalControllerOnVirtualCreation) ? new Localizable(11314) : new Localizable());
		}

		public DelegateCommand ResetSteamDeckToDefaultsCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._resetSteamDeckToDefaults) == null)
				{
					delegateCommand = (this._resetSteamDeckToDefaults = new DelegateCommand(new Action(this.ResetSteamDeckToDefaults), new Func<bool>(this.ResetSteamDeckToDefaultsCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void ResetSteamDeckToDefaults()
		{
			this.SteamDeckLeftIntensity = 7;
			this.SteamDeckLeftIsStrongIntensity = 1;
			this.SteamDeckRightIntensity = 7;
			this.SteamDeckRightIsStrongIntensity = 1;
		}

		private bool ResetSteamDeckToDefaultsCanExecute()
		{
			return this.SteamDeckLeftIntensity != 7 || this.SteamDeckLeftIsStrongIntensity != 1 || this.SteamDeckRightIntensity != 7 || this.SteamDeckRightIsStrongIntensity != 1;
		}

		private bool _disablePaddlesOnLocked;

		private bool _emulateLizardConfigForSteam;

		private bool _emulateLizardConfigForSteamDeck;

		private bool _stopMacroHotkey;

		private bool _hidePhysicalControllerOnVirtualCreation;

		private bool _hidePhysicalXboxOneOrEliteControllerOnVirtualCreation;

		private bool _isGyroRollDefault;

		private bool _isGyroYawDefault;

		private bool _isGyroInitialStateOnDefault;

		private bool _isGyroInitialStateOffDefault;

		private bool _turnOffControllerOption;

		private int _turnOffControllerTimeout;

		private bool _sendRumbleToGamepad;

		private byte _steamDeckLeftIntensity;

		private int _steamDeckLeftIsStrongIntensity;

		private byte _steamDeckRightIntensity;

		private int _steamDeckRightIsStrongIntensity;

		private LocalizationManager _localizationManager = LocalizationManager.Instance;

		private DelegateCommand _resetSteamDeckToDefaults;
	}
}
