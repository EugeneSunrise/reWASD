using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.ViewModels.Preferences.Base;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.ViewModels.Preferences
{
	public class ConfirmationPageVM : PreferencesBaseVM
	{
		public bool ExitTrayAgent
		{
			get
			{
				return this._exitTrayAgent;
			}
			set
			{
				if (this._exitTrayAgent == value)
				{
					return;
				}
				this._exitTrayAgent = value;
				this.OnPropertyChanged("ExitTrayAgent");
			}
		}

		public bool ConfirmTryApplyLockedTrial
		{
			get
			{
				return this._confirmTryApplyLockedTrial;
			}
			set
			{
				if (this._confirmTryApplyLockedTrial == value)
				{
					return;
				}
				this._confirmTryApplyLockedTrial = value;
				this.OnPropertyChanged("ConfirmTryApplyLockedTrial");
			}
		}

		public bool ConfirmTryApplyLockedNoTrial
		{
			get
			{
				return this._confirmTryApplyLockedNoTrial;
			}
			set
			{
				if (this._confirmTryApplyLockedNoTrial == value)
				{
					return;
				}
				this._confirmTryApplyLockedNoTrial = value;
				this.OnPropertyChanged("ConfirmTryApplyLockedNoTrial");
			}
		}

		public bool ConfirmClearShiftConfig
		{
			get
			{
				return this._confirmClearShiftConfig;
			}
			set
			{
				if (this._confirmClearShiftConfig == value)
				{
					return;
				}
				this._confirmClearShiftConfig = value;
				this.OnPropertyChanged("ConfirmClearShiftConfig");
			}
		}

		public bool ConfirmAddOrDeleteShift
		{
			get
			{
				return this._confirmAddOrDeleteShift;
			}
			set
			{
				if (this._confirmAddOrDeleteShift == value)
				{
					return;
				}
				this._confirmAddOrDeleteShift = value;
				this.OnPropertyChanged("ConfirmAddOrDeleteShift");
			}
		}

		public bool ConfirmSavePresetShift
		{
			get
			{
				return this._confirmSavePresetShift;
			}
			set
			{
				if (this._confirmSavePresetShift == value)
				{
					return;
				}
				this._confirmSavePresetShift = value;
				this.OnPropertyChanged("ConfirmSavePresetShift");
			}
		}

		public bool ConfirmClearConfig
		{
			get
			{
				return this._confirmClearConfig;
			}
			set
			{
				if (this._confirmClearConfig == value)
				{
					return;
				}
				this._confirmClearConfig = value;
				this.OnPropertyChanged("ConfirmClearConfig");
			}
		}

		public bool ConfirmDuplicateMappings
		{
			get
			{
				return this._confirmDuplicateMappings;
			}
			set
			{
				if (this._confirmDuplicateMappings == value)
				{
					return;
				}
				this._confirmDuplicateMappings = value;
				this.OnPropertyChanged("ConfirmDuplicateMappings");
			}
		}

		public bool ConfirmRemoveGameOrConfig
		{
			get
			{
				return this._confirmRemoveGameOrConfig;
			}
			set
			{
				if (this._confirmRemoveGameOrConfig == value)
				{
					return;
				}
				this._confirmRemoveGameOrConfig = value;
				this.OnPropertyChanged("ConfirmRemoveGameOrConfig");
			}
		}

		public bool ConfirmResetSticksDefault
		{
			get
			{
				return this._confirmResetSticksDefault;
			}
			set
			{
				if (this._confirmResetSticksDefault == value)
				{
					return;
				}
				this._confirmResetSticksDefault = value;
				this.OnPropertyChanged("ConfirmResetSticksDefault");
			}
		}

		public bool WorkWithMacroEditor
		{
			get
			{
				return this._workWithMacroEditor;
			}
			set
			{
				if (this._workWithMacroEditor == value)
				{
					return;
				}
				this._workWithMacroEditor = value;
				this.OnPropertyChanged("WorkWithMacroEditor");
			}
		}

		public bool ConfirmCheatWarning
		{
			get
			{
				return this._confirmCheatWarning;
			}
			set
			{
				if (this._confirmCheatWarning == value)
				{
					return;
				}
				this._confirmCheatWarning = value;
				this.OnPropertyChanged("ConfirmCheatWarning");
			}
		}

		public bool RemindSteamLimitation
		{
			get
			{
				return this._remindSteamLimitation;
			}
			set
			{
				if (this._remindSteamLimitation == value)
				{
					return;
				}
				this._remindSteamLimitation = value;
				this.OnPropertyChanged("RemindSteamLimitation");
			}
		}

		public bool RemindAboutPossibleLagsWithExternalController
		{
			get
			{
				return this._remindAboutPossibleLagsWithExternalController;
			}
			set
			{
				if (this._remindAboutPossibleLagsWithExternalController == value)
				{
					return;
				}
				this._remindAboutPossibleLagsWithExternalController = value;
				this.OnPropertyChanged("RemindAboutPossibleLagsWithExternalController");
			}
		}

		public bool ConfirmClearCombo
		{
			get
			{
				return this._confirmClearCombo;
			}
			set
			{
				if (this._confirmClearCombo == value)
				{
					return;
				}
				this._confirmClearCombo = value;
				this.OnPropertyChanged("ConfirmClearCombo");
			}
		}

		public bool ConfirmZeroDelayBetweenKeys
		{
			get
			{
				return this._confirmZeroDelayBetweenKeys;
			}
			set
			{
				if (this._confirmZeroDelayBetweenKeys == value)
				{
					return;
				}
				this._confirmZeroDelayBetweenKeys = value;
				this.OnPropertyChanged("ConfirmZeroDelayBetweenKeys");
			}
		}

		public bool ConfirmUnmapForKeyboardMapping
		{
			get
			{
				return this._confirmUnmapForKeyboardMapping;
			}
			set
			{
				if (this._confirmUnmapForKeyboardMapping == value)
				{
					return;
				}
				this._confirmUnmapForKeyboardMapping = value;
				this.OnPropertyChanged("ConfirmUnmapForKeyboardMapping");
			}
		}

		public bool ConfirmSwitchToXBoxOne
		{
			get
			{
				return this._confirmSwitchToXBoxOne;
			}
			set
			{
				if (this._confirmSwitchToXBoxOne == value)
				{
					return;
				}
				this._confirmSwitchToXBoxOne = value;
				this.OnPropertyChanged("ConfirmSwitchToXBoxOne");
			}
		}

		public bool ConfirmSyncMappings
		{
			get
			{
				return this._confirmSyncMappings;
			}
			set
			{
				if (this._confirmSyncMappings == value)
				{
					return;
				}
				this._confirmSyncMappings = value;
				this.OnPropertyChanged("ConfirmSyncMappings");
			}
		}

		public bool ConfirmUnmapTouchpad
		{
			get
			{
				return this._confirmUnmapTouchpad;
			}
			set
			{
				if (this._confirmUnmapTouchpad == value)
				{
					return;
				}
				this._confirmUnmapTouchpad = value;
				this.OnPropertyChanged("ConfirmUnmapTouchpad");
			}
		}

		public bool ConfirmAutoCorrectStickDeflections
		{
			get
			{
				return this._confirmAutoCorrectStickDeflections;
			}
			set
			{
				if (this._confirmAutoCorrectStickDeflections == value)
				{
					return;
				}
				this._confirmAutoCorrectStickDeflections = value;
				this.OnPropertyChanged("ConfirmAutoCorrectStickDeflections");
			}
		}

		public bool ConfirmMergeKeyboardAndMouse
		{
			get
			{
				return this._confirmMergeKeyboardAndMouse;
			}
			set
			{
				if (this._confirmMergeKeyboardAndMouse == value)
				{
					return;
				}
				this._confirmMergeKeyboardAndMouse = value;
				this.OnPropertyChanged("ConfirmMergeKeyboardAndMouse");
			}
		}

		public bool RemindAboutUnmappedMouse
		{
			get
			{
				return this._remindAboutUnmappedMouse;
			}
			set
			{
				if (this._remindAboutUnmappedMouse == value)
				{
					return;
				}
				this._remindAboutUnmappedMouse = value;
				this.OnPropertyChanged("RemindAboutUnmappedMouse");
			}
		}

		public bool AskwhetherToFixConfigWithUnmappedMouse
		{
			get
			{
				return this._askwhetherToFixConfigWithUnmappedMouse;
			}
			set
			{
				if (this._askwhetherToFixConfigWithUnmappedMouse == value)
				{
					return;
				}
				this._askwhetherToFixConfigWithUnmappedMouse = value;
				this.OnPropertyChanged("AskwhetherToFixConfigWithUnmappedMouse");
			}
		}

		public bool ConfirmDetectActiveDevice
		{
			get
			{
				return this._confirmDetectActiveDevice;
			}
			set
			{
				if (this._confirmDetectActiveDevice == value)
				{
					return;
				}
				this._confirmDetectActiveDevice = value;
				this.OnPropertyChanged("ConfirmDetectActiveDevice");
			}
		}

		public bool ConfirmUnplugPhysController
		{
			get
			{
				return this._confirmUnplugPhysController;
			}
			set
			{
				if (this._confirmUnplugPhysController == value)
				{
					return;
				}
				this._confirmUnplugPhysController = value;
				this.OnPropertyChanged("ConfirmUnplugPhysController");
			}
		}

		public bool ConfirmVirtualUsbHub
		{
			get
			{
				return this._confirmVirtualUsbHub;
			}
			set
			{
				if (this._confirmVirtualUsbHub == value)
				{
					return;
				}
				this._confirmVirtualUsbHub = value;
				this.OnPropertyChanged("ConfirmVirtualUsbHub");
			}
		}

		public bool ConfirmSteamExclusiveMode
		{
			get
			{
				return this._confirmSteamExclusiveMode;
			}
			set
			{
				if (this._confirmSteamExclusiveMode == value)
				{
					return;
				}
				this._confirmSteamExclusiveMode = value;
				this.OnPropertyChanged("ConfirmSteamExclusiveMode");
			}
		}

		public bool ConfirmEngineController
		{
			get
			{
				return this._confirmEngineController;
			}
			set
			{
				if (this._confirmEngineController == value)
				{
					return;
				}
				this._confirmEngineController = value;
				this.OnPropertyChanged("ConfirmEngineController");
			}
		}

		public bool ConfirmAzeronExclusiveMode
		{
			get
			{
				return this._confirmAzeronExclusiveMode;
			}
			set
			{
				if (this._confirmAzeronExclusiveMode == value)
				{
					return;
				}
				this._confirmAzeronExclusiveMode = value;
				this.OnPropertyChanged("ConfirmAzeronExclusiveMode");
			}
		}

		public bool ConfirmFlydigiExclusiveMode
		{
			get
			{
				return this._confirmFlydigiExclusiveMode;
			}
			set
			{
				if (this._confirmFlydigiExclusiveMode == value)
				{
					return;
				}
				this._confirmFlydigiExclusiveMode = value;
				this.OnPropertyChanged("ConfirmFlydigiExclusiveMode");
			}
		}

		public bool ConfirmSwitchToGameSirG7
		{
			get
			{
				return this._confirmSwitchToGameSirG7;
			}
			set
			{
				if (this._confirmSwitchToGameSirG7 == value)
				{
					return;
				}
				this._confirmSwitchToGameSirG7 = value;
				this.OnPropertyChanged("ConfirmSwitchToGameSirG7");
			}
		}

		public string InformSteamExclusiveMode
		{
			get
			{
				return string.Format(DTLocalization.GetString(11898), "Steam");
			}
		}

		public string InformAzeronExclusiveMode
		{
			get
			{
				return string.Format(DTLocalization.GetString(11898), "Azeron");
			}
		}

		public string InformFlydigiExclusiveMode
		{
			get
			{
				return string.Format(DTLocalization.GetString(11898), "Flydigi");
			}
		}

		public string InformEngineController
		{
			get
			{
				return DTLocalization.GetString(12522);
			}
		}

		public ConfirmationPageVM()
		{
			TranslationManager.Instance.LanguageChanged += delegate([Nullable(2)] object o, EventArgs e)
			{
				this.OnPropertyChanged("InformSteamExclusiveMode");
				this.OnPropertyChanged("InformAzeronExclusiveMode");
				this.OnPropertyChanged("InformFlydigiExclusiveMode");
				this.OnPropertyChanged("InformEngineController");
			};
		}

		public override Task Initialize()
		{
			this.ConfirmTryApplyLockedTrial = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "TryApplyLockedTrial", 1, false) == 1;
			this.ConfirmTryApplyLockedNoTrial = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "TryApplyLockedToNoTrial", 1, false) == 1;
			this.ConfirmClearShiftConfig = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmClearShiftConfig", 1, false) == 1;
			this.ConfirmClearConfig = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmClearConfig", 1, false) == 1;
			this.ConfirmAddOrDeleteShift = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmAddOrDeleteShift", 1, false) == 1;
			this.ConfirmSavePresetShift = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmAddOrDeletePresetShift", 1, false) == 1;
			this.ConfirmDuplicateMappings = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "DuplicateMappings", 1, false) == 1;
			this.ConfirmRemoveGameOrConfig = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "RemoveGameOrConfig", 1, false) == 1;
			this.ConfirmResetSticksDefault = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmResetSticksDefault", 1, false) == 1;
			this.WorkWithMacroEditor = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "WorkWithMacroEditor", 1, false) == 1;
			this.ConfirmCheatWarning = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "CheatWarning", 1, false) == 1;
			this.ConfirmClearCombo = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmClearCombo", 1, false) == 1;
			this.ConfirmZeroDelayBetweenKeys = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmZeroDelayBetweenKeys", 1, false) == 1;
			this.ConfirmUnmapForKeyboardMapping = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmUnmapForKeyboardMapping", 1, false) == 1;
			this.ConfirmAutoCorrectStickDeflections = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmAutoCorrectStickDeflections", 1, false) == 1;
			this.ConfirmMergeKeyboardAndMouse = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmMergeKeyboardAndMouse", 1, false) == 1;
			this.RemindAboutUnmappedMouse = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "RemindAboutUnmappedMouse", 1, false) == 1;
			this.AskwhetherToFixConfigWithUnmappedMouse = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmAskwhetherToFixConfigWithUnmappedMouse", 1, false) == 1;
			this.ConfirmDetectActiveDevice = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmDetectActiveDevice", 1, false) == 1;
			this.ConfirmUnplugPhysController = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmUnplugPhysicalControler", 1, false) == 1;
			this.ConfirmSteamExclusiveMode = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmSteamExclusiveMode", 1, false) == 1;
			this.ConfirmEngineController = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmEngineController", 1, false) == 1;
			this.ConfirmAzeronExclusiveMode = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmAzeronExclusiveMode", 1, false) == 1;
			this.ConfirmFlydigiExclusiveMode = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmFlydigiExclusiveMode", 1, false) == 1;
			this.ConfirmSwitchToGameSirG7 = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmGameSirG7Mode", 1, false) == 1;
			this.RemindAboutPossibleLagsWithExternalController = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "RemindAboutPossibleLagsWithExternalController", 1, false) == 1;
			this.RemindSteamLimitation = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "RemindSteamIgnoreRewasd", 1, false) == 1;
			this.ConfirmUnmapTouchpad = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmUnmapTouchpad", 1, false) == 1;
			this.ConfirmSwitchToXBoxOne = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmSwitchToXBOXOne", 1, false) == 1;
			this.ConfirmSyncMappings = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmSyncMappings", 1, false) == 1;
			this.ConfirmVirtualUsbHub = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmVirtualUsbHub", 1, false) == 1;
			this.ExitTrayAgent = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ExitTrayAgentModified", 1, false) == 1;
			return Task.CompletedTask;
		}

		private void ResetInitializationForController(Func<ControllerVM, bool> action, bool value)
		{
			App.GamepadService.GamepadCollection.ForEach(delegate(BaseControllerVM gamepad)
			{
				ControllerVM controllerVM = gamepad as ControllerVM;
				if (controllerVM != null && action(controllerVM))
				{
					controllerVM.SetIsInitialized(value);
				}
			});
		}

		public override Task<bool> ApplyChanges()
		{
			bool flag = false;
			if (RegistryHelper.GetBool(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmEngineController", true) != this.ConfirmEngineController && this.ConfirmEngineController)
			{
				flag = true;
			}
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "TryApplyLockedTrial", Convert.ToInt32(this.ConfirmTryApplyLockedTrial));
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "TryApplyLockedToNoTrial", Convert.ToInt32(this.ConfirmTryApplyLockedNoTrial));
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "DuplicateMappings", Convert.ToInt32(this.ConfirmDuplicateMappings));
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "RemoveGameOrConfig", Convert.ToInt32(this.ConfirmRemoveGameOrConfig));
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmResetSticksDefault", Convert.ToInt32(this.ConfirmResetSticksDefault));
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "WorkWithMacroEditor", Convert.ToInt32(this.WorkWithMacroEditor));
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "CheatWarning", Convert.ToInt32(this.ConfirmCheatWarning));
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmClearCombo", Convert.ToInt32(this.ConfirmClearCombo));
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmZeroDelayBetweenKeys", Convert.ToInt32(this.ConfirmZeroDelayBetweenKeys));
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmUnmapForKeyboardMapping", Convert.ToInt32(this.ConfirmUnmapForKeyboardMapping));
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmAutoCorrectStickDeflections", Convert.ToInt32(this.ConfirmAutoCorrectStickDeflections));
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmMergeKeyboardAndMouse", Convert.ToInt32(this.ConfirmMergeKeyboardAndMouse));
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "RemindAboutUnmappedMouse", Convert.ToInt32(this.RemindAboutUnmappedMouse));
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmAskwhetherToFixConfigWithUnmappedMouse", Convert.ToInt32(this.AskwhetherToFixConfigWithUnmappedMouse));
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmDetectActiveDevice", Convert.ToInt32(this.ConfirmDetectActiveDevice));
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmUnplugPhysicalControler", Convert.ToInt32(this.ConfirmUnplugPhysController));
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmVirtualUsbHub", Convert.ToInt32(this.ConfirmVirtualUsbHub));
			bool flag2 = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmSteamExclusiveMode", 1, false) == 1;
			bool flag3 = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmAzeronExclusiveMode", 1, false) == 1;
			bool flag4 = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmFlydigiExclusiveMode", 1, false) == 1;
			bool flag5 = RegistryHelper.GetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmGameSirG7Mode", 1, false) == 1;
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmSteamExclusiveMode", Convert.ToInt32(this.ConfirmSteamExclusiveMode));
			if (flag2 != this.ConfirmSteamExclusiveMode)
			{
				this.ResetInitializationForController((ControllerVM controller) => controller.IsAnySteam, !this.ConfirmSteamExclusiveMode);
			}
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmAzeronExclusiveMode", Convert.ToInt32(this.ConfirmAzeronExclusiveMode));
			if (flag3 != this.ConfirmAzeronExclusiveMode)
			{
				this.ResetInitializationForController((ControllerVM controller) => controller.IsAnyAzeron, !this.ConfirmAzeronExclusiveMode);
			}
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmFlydigiExclusiveMode", Convert.ToInt32(this.ConfirmFlydigiExclusiveMode));
			if (flag4 != this.ConfirmFlydigiExclusiveMode)
			{
				this.ResetInitializationForController((ControllerVM controller) => ControllerTypeExtensions.IsFlydigi(controller.ControllerType), this.ConfirmFlydigiExclusiveMode);
			}
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmGameSirG7Mode", Convert.ToInt32(this.ConfirmSwitchToGameSirG7));
			if (this.ConfirmSwitchToGameSirG7)
			{
				RegistryHelper.SetValue("Config", "ChangeGameSirG7Mode", 0);
			}
			if (flag5 != this.ConfirmSwitchToGameSirG7)
			{
				this.ResetInitializationForController((ControllerVM controller) => controller.IsXboxGameSirG7, !this.ConfirmSwitchToGameSirG7);
			}
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmEngineController", Convert.ToInt32(this.ConfirmEngineController));
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "RemindAboutPossibleLagsWithExternalController", Convert.ToInt32(this.RemindAboutPossibleLagsWithExternalController));
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "RemindSteamIgnoreRewasd", Convert.ToInt32(this.RemindSteamLimitation));
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmUnmapTouchpad", Convert.ToInt32(this.ConfirmUnmapTouchpad));
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmClearShiftConfig", Convert.ToInt32(this.ConfirmClearShiftConfig));
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmClearConfig", Convert.ToInt32(this.ConfirmClearConfig));
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmAddOrDeleteShift", Convert.ToInt32(this.ConfirmAddOrDeleteShift));
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmAddOrDeletePresetShift", Convert.ToInt32(this.ConfirmSavePresetShift));
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmSwitchToXBOXOne", Convert.ToInt32(this.ConfirmSwitchToXBoxOne));
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ConfirmSyncMappings", Convert.ToInt32(this.ConfirmSyncMappings));
			RegistryHelper.SetValue(RegistryHelper.CONFIRMATION_REG_PATH, "ExitTrayAgentModified", Convert.ToInt32(this.ExitTrayAgent));
			if (flag)
			{
				App.GamepadService.RefreshPromoController();
			}
			return Task.FromResult<bool>(true);
		}

		private bool _confirmTryApplyLockedTrial;

		private bool _confirmTryApplyLockedNoTrial;

		private bool _confirmClearShiftConfig;

		private bool _confirmAddOrDeleteShift;

		private bool _confirmSavePresetShift;

		private bool _confirmClearConfig;

		private bool _confirmDuplicateMappings;

		private bool _confirmRemoveGameOrConfig;

		private bool _confirmResetSticksDefault;

		private bool _workWithMacroEditor;

		private bool _confirmCheatWarning;

		private bool _confirmClearCombo;

		private bool _confirmZeroDelayBetweenKeys;

		private bool _confirmUnmapForKeyboardMapping;

		private bool _confirmAutoCorrectStickDeflections;

		private bool _confirmMergeKeyboardAndMouse;

		private bool _remindAboutUnmappedMouse;

		private bool _askwhetherToFixConfigWithUnmappedMouse;

		private bool _confirmDetectActiveDevice;

		private bool _confirmUnplugPhysController;

		private bool _confirmSteamExclusiveMode;

		private bool _confirmEngineController;

		private bool _confirmAzeronExclusiveMode;

		private bool _confirmFlydigiExclusiveMode;

		private bool _confirmSwitchToGameSirG7;

		private bool _remindAboutPossibleLagsWithExternalController;

		private bool _remindSteamLimitation;

		private bool _confirmUnmapTouchpad;

		private bool _confirmSwitchToXBoxOne;

		private bool _confirmSyncMappings;

		private bool _confirmVirtualUsbHub;

		private bool _exitTrayAgent;
	}
}
