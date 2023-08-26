using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Infrastructure.SupportedControllers;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Utils.XBUtil;
using reWASDUI.ViewModels.Base;
using reWASDUI.Views.Preferences;
using reWASDUI.Views.SecondaryWindows;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.License.Licensing.ComStructures;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.ViewModels
{
	public class MainContentVM : BaseControllerKeyPressedPollerVM, ISetSlotModel
	{
		public GamepadSelectorVM GamepadSelectorVM
		{
			get
			{
				return IContainerProviderExtensions.Resolve<GamepadSelectorVM>(App.Container);
			}
		}

		public GameConfigSelectorVM GameConfigSelectorVM
		{
			get
			{
				return IContainerProviderExtensions.Resolve<GameConfigSelectorVM>(App.Container);
			}
		}

		public Slot SelectedSlot
		{
			get
			{
				return this._selectedSlot;
			}
			set
			{
				this.SetProperty<Slot>(ref this._selectedSlot, value, "SelectedSlot");
			}
		}

		public string LicenseDescription
		{
			get
			{
				return this._licenseDescription;
			}
			set
			{
				this.SetProperty<string>(ref this._licenseDescription, value, "LicenseDescription");
			}
		}

		public bool IsLicenseLockEngineController
		{
			get
			{
				BaseControllerVM currentGamepad = base.GamepadService.CurrentGamepad;
				return currentGamepad != null && currentGamepad.IsLicenseLockEngineController;
			}
		}

		public MainContentVM(IContainerProvider uc)
			: base(uc)
		{
			App.LicensingService.OnLicenseChangedCompleted += delegate(LicenseCheckResult result, bool activation)
			{
				this.LicenseChanged();
			};
			TranslationManager.Instance.LanguageChanged += delegate([Nullable(2)] object s, EventArgs e)
			{
				this.UpdateLicenseDescription();
			};
			this.UpdateLicenseDescription();
			App.EventAggregator.GetEvent<CurrentGamepadChanged>().Subscribe(delegate(BaseControllerVM item)
			{
				this.OnPropertyChanged("IsLicenseLockEngineController");
			});
		}

		~MainContentVM()
		{
		}

		private void LicenseChanged()
		{
			this.UpdateLicenseDescription();
			this.OnPropertyChanged("IsLicenseLockEngineController");
		}

		private async void UpdateLicenseDescription()
		{
			LicenseInfo licenseInfo2 = await App.LicensingService.GetLicenseInfo();
			LicenseInfo licenseInfo = licenseInfo2;
			ThreadHelper.ExecuteInMainDispatcher(delegate
			{
				string text = "";
				if (licenseInfo.License == 2)
				{
					if (licenseInfo.TrialDaysLeft >= 0)
					{
						text += string.Format(DTLocalization.GetString(11800).Replace("%d", "{0}"), licenseInfo.TrialDaysLeft);
					}
					else if (licenseInfo.VerifiedLicense != 0)
					{
						text += DTLocalization.GetString(11799);
					}
				}
				if (licenseInfo.VerifiedLicense == 0)
				{
					text = text + " (" + DTLocalization.GetString(11798) + ") ";
				}
				this.LicenseDescription = text;
			}, true);
		}

		public override bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return true;
		}

		public override void OnControllerPollState(GamepadState gamepadState)
		{
		}

		private bool IsFilteredForHookSelection(GamepadButton gb, ControllerTypeEnum? controllerType)
		{
			ControllerTypeEnum? controllerTypeEnum = controllerType;
			ControllerTypeEnum controllerTypeEnum2 = 63;
			return ((controllerTypeEnum.GetValueOrDefault() == controllerTypeEnum2) & (controllerTypeEnum != null)) && base.UserSettingsService.EmulateLizardConfigForSteamDeck && (GamepadButtonExtensions.IsAnyPhysicalTrackPad(gb) || GamepadButtonExtensions.IsAnyTrigger(gb) || GamepadButtonExtensions.IsDPAD(gb) || gb == 1 || gb == 2);
		}

		public override void OnControllerKeyDown(List<GamepadButtonDescription> buttons)
		{
			MainContentVM.<>c__DisplayClass21_0 CS$<>8__locals1 = new MainContentVM.<>c__DisplayClass21_0();
			CS$<>8__locals1.<>4__this = this;
			if (base.GameProfilesService.RealCurrentBeingMappedBindingCollection == null || !buttons.Any<GamepadButtonDescription>())
			{
				return;
			}
			CS$<>8__locals1.pressedButton = buttons.First<GamepadButtonDescription>().Button;
			if (GamepadButtonExtensions.IsLeftTrigger(CS$<>8__locals1.pressedButton))
			{
				CS$<>8__locals1.pressedButton = 51;
			}
			else if (GamepadButtonExtensions.IsRightTrigger(CS$<>8__locals1.pressedButton))
			{
				CS$<>8__locals1.pressedButton = 55;
			}
			MainContentVM.<>c__DisplayClass21_0 CS$<>8__locals2 = CS$<>8__locals1;
			BaseControllerVM currentGamepad = base.GamepadService.CurrentGamepad;
			ControllerTypeEnum? controllerTypeEnum;
			if (currentGamepad == null)
			{
				controllerTypeEnum = null;
			}
			else
			{
				ControllerVM currentController = currentGamepad.CurrentController;
				controllerTypeEnum = ((currentController != null) ? new ControllerTypeEnum?(currentController.ControllerType) : null);
			}
			CS$<>8__locals2.currentControllerType = controllerTypeEnum;
			if (this.IsFilteredForHookSelection(CS$<>8__locals1.pressedButton, CS$<>8__locals1.currentControllerType))
			{
				return;
			}
			ThreadHelper.ExecuteInMainDispatcher(delegate
			{
				BaseXBBindingCollection realCurrentBeingMappedBindingCollection = CS$<>8__locals1.<>4__this.GameProfilesService.RealCurrentBeingMappedBindingCollection;
				if ((realCurrentBeingMappedBindingCollection == null || !realCurrentBeingMappedBindingCollection.IsMaskModeView) && CS$<>8__locals1.currentControllerType != null)
				{
					ControllerTypeEnum value = XBUtils.CorrectNintendoSwitchJoy(new ControllerTypeEnum?(CS$<>8__locals1.currentControllerType.Value)).Value;
					if (ControllersHelper.IsGamepadButtonExist(CS$<>8__locals1.pressedButton, value) || (CS$<>8__locals1.currentControllerType.Value != value && ControllersHelper.IsGamepadButtonExist(CS$<>8__locals1.pressedButton, CS$<>8__locals1.currentControllerType.Value)))
					{
						CS$<>8__locals1.<>4__this.GameProfilesService.ChangeCurrentBindingCommand.Execute(new GamepadButton?(CS$<>8__locals1.pressedButton));
					}
				}
			}, true);
		}

		public override void OnControllerKeyUp(List<GamepadButtonDescription> buttons)
		{
		}

		public void StartKeyPoller()
		{
			this.KeyPressedPollerService.StartPolling(true);
		}

		public void SuspendKeyPoller()
		{
			this.KeyPressedPollerService.SuspendPollingUntilStarted();
		}

		public DelegateCommand ShowSettingsCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._showSettings) == null)
				{
					delegateCommand = (this._showSettings = new DelegateCommand(new Action(this.ShowSettings)));
				}
				return delegateCommand;
			}
		}

		private void ShowSettings()
		{
			PreferencesWindow.ShowPreferences(null);
		}

		public DelegateCommand OpenOnlineHelpSupportCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._openOnlineHelpSupport) == null)
				{
					delegateCommand = (this._openOnlineHelpSupport = new DelegateCommand(new Action(this.OpenOnlineHelpSupport)));
				}
				return delegateCommand;
			}
		}

		private void OpenOnlineHelpSupport()
		{
			new ContactSupportWindow(base.LicensingService.Serial, base.LicensingService.HardWareID).ShowDialog();
		}

		public DelegateCommand OpenOnlineHelpCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._openOnlineHelp) == null)
				{
					delegateCommand = (this._openOnlineHelp = new DelegateCommand(new Action(this.OpenOnlineHelp)));
				}
				return delegateCommand;
			}
		}

		private void OpenOnlineHelp()
		{
			DSUtils.GoUrl("http://help.rewasd.com/");
		}

		public DelegateCommand OpenDiscordCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._openDiscord) == null)
				{
					delegateCommand = (this._openDiscord = new DelegateCommand(new Action(this.OpenDiscord)));
				}
				return delegateCommand;
			}
		}

		private void OpenDiscord()
		{
			DSUtils.GoUrl("https://discord.gg/vT3udBf");
		}

		public DelegateCommand OpenFacebookCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._openFacebook) == null)
				{
					delegateCommand = (this._openFacebook = new DelegateCommand(new Action(this.OpenFacebook)));
				}
				return delegateCommand;
			}
		}

		private void OpenFacebook()
		{
			DSUtils.GoUrl("https://www.facebook.com/reWASDapp/");
		}

		public DelegateCommand OpenForumCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._openForum) == null)
				{
					delegateCommand = (this._openForum = new DelegateCommand(new Action(this.OpenForum)));
				}
				return delegateCommand;
			}
		}

		private void OpenForum()
		{
			DSUtils.GoUrl("https://forum.rewasd.com/");
		}

		public DelegateCommand ShowLicenseCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._showLicense) == null)
				{
					delegateCommand = (this._showLicense = new DelegateCommand(new Action(this.ShowLicense)));
				}
				return delegateCommand;
			}
		}

		private void ShowLicense()
		{
			base.LicensingService.ShowLicenseDialog();
		}

		private Slot _selectedSlot;

		private string _licenseDescription;

		private DelegateCommand _showSettings;

		private DelegateCommand _openOnlineHelpSupport;

		private DelegateCommand _openOnlineHelp;

		private DelegateCommand _openDiscord;

		private DelegateCommand _openFacebook;

		private DelegateCommand _openForum;

		private DelegateCommand _showLicense;
	}
}
