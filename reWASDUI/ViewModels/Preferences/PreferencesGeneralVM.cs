using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using DiscSoft.NET.Common.AdminRightsFeatures;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Logging;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Attributes;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using DiscSoft.NET.Common.View.SecondaryWindows.WaitDialog;
using Prism.Commands;
using Prism.Ioc;
using reWASDCommon;
using reWASDCommon.Infrastructure;
using reWASDCommon.Utils;
using reWASDUI.Services;
using reWASDUI.ViewModels.Preferences.Base;
using XBEliteWPF.Infrastructure.ExternalDevices;
using XBEliteWPF.Infrastructure.XBEliteService;
using XBEliteWPF.Utils;

namespace reWASDUI.ViewModels.Preferences
{
	public class PreferencesGeneralVM : PreferencesBaseVM
	{
		public bool IsWindows10OrHigher
		{
			get
			{
				return UtilsNative.IsWindows10OrHigher();
			}
		}

		public ObservableCollection<Language> Languages
		{
			get
			{
				return this._languages;
			}
			set
			{
				if (value == this._languages)
				{
					return;
				}
				this._languages = value;
				this.OnPropertyChanged("Languages");
			}
		}

		public Language ActiveLanguage
		{
			get
			{
				return this._activeLanguage;
			}
			set
			{
				if (value == this._activeLanguage)
				{
					return;
				}
				this._activeLanguage = value;
				this.OnPropertyChanged("ActiveLanguage");
			}
		}

		public bool SendAnonymousStatistic
		{
			get
			{
				return this._sendAnonymousStatistic;
			}
			set
			{
				if (value == this._sendAnonymousStatistic)
				{
					return;
				}
				this._sendAnonymousStatistic = value;
				this.OnPropertyChanged("SendAnonymousStatistic");
			}
		}

		public bool HideLockedFeatures
		{
			get
			{
				return this._hideLockedFeatures;
			}
			set
			{
				if (value == this._hideLockedFeatures)
				{
					return;
				}
				this._hideLockedFeatures = value;
				this.OnPropertyChanged("HideLockedFeatures");
			}
		}

		public bool ShowReleasePageOnStartup
		{
			get
			{
				return this._showReleasePageOnStartup;
			}
			set
			{
				if (value == this._showReleasePageOnStartup)
				{
					return;
				}
				this._showReleasePageOnStartup = value;
				this.OnPropertyChanged("ShowReleasePageOnStartup");
			}
		}

		public bool GetRecommendedConfigsWithUpdates
		{
			get
			{
				return this._getRecommendedConfigsWithUpdates;
			}
			set
			{
				if (value == this._getRecommendedConfigsWithUpdates)
				{
					return;
				}
				this._getRecommendedConfigsWithUpdates = value;
				this.OnPropertyChanged("GetRecommendedConfigsWithUpdates");
			}
		}

		public bool PreventSleepIfComboIsBeingEmulated
		{
			get
			{
				return this._preventSleepIfComboIsBeingEmulated;
			}
			set
			{
				if (value == this._preventSleepIfComboIsBeingEmulated)
				{
					return;
				}
				this._preventSleepIfComboIsBeingEmulated = value;
				this.OnPropertyChanged("PreventSleepIfComboIsBeingEmulated");
			}
		}

		public bool IsFilledSVGTheme
		{
			get
			{
				return this._isFilledSVGTheme;
			}
			set
			{
				if (value == this._isFilledSVGTheme)
				{
					return;
				}
				this._isFilledSVGTheme = value;
				this.OnPropertyChanged("IsFilledSVGTheme");
			}
		}

		public bool IsTransparentSVGTheme
		{
			get
			{
				return this._isTransparentSVGTheme;
			}
			set
			{
				if (value == this._isTransparentSVGTheme)
				{
					return;
				}
				this._isTransparentSVGTheme = value;
				this.OnPropertyChanged("IsTransparentSVGTheme");
			}
		}

		public bool HookControllerButtons
		{
			get
			{
				return this._hookControllerButtons;
			}
			set
			{
				if (value == this._hookControllerButtons)
				{
					return;
				}
				this._hookControllerButtons = value;
				this.OnPropertyChanged("HookControllerButtons");
				this.SetDescription();
			}
		}

		public bool RepressButtonForShift
		{
			get
			{
				return this._repressButtonForShift;
			}
			set
			{
				if (value == this._repressButtonForShift)
				{
					return;
				}
				this._repressButtonForShift = value;
				this.OnPropertyChanged("RepressButtonForShift");
			}
		}

		public bool LeaveVirtualControllerForOfflineController
		{
			get
			{
				return this._leaveVirtualControllerForOfflineController;
			}
			set
			{
				if (value == this._leaveVirtualControllerForOfflineController)
				{
					return;
				}
				this._leaveVirtualControllerForOfflineController = value;
				this.OnPropertyChanged("LeaveVirtualControllerForOfflineController");
			}
		}

		public bool TurnOffRemapByAltCtrlDel
		{
			get
			{
				return this._turnOffRemapByAltCtrlDel;
			}
			set
			{
				if (value == this._turnOffRemapByAltCtrlDel)
				{
					return;
				}
				this._turnOffRemapByAltCtrlDel = value;
				this.OnPropertyChanged("TurnOffRemapByAltCtrlDel");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool IsUpdateChecking
		{
			get
			{
				return this._isUpdateChecking;
			}
			set
			{
				if (value == this._isUpdateChecking)
				{
					return;
				}
				this._isUpdateChecking = value;
				this.OnPropertyChanged("IsUpdateChecking");
			}
		}

		public string PrivacyUrl
		{
			get
			{
				return "https://www.rewasd.com/privacy";
			}
		}

		public string PrivacyPolicyString
		{
			get
			{
				return string.Format("{0} {1}", DTLocalization.GetString(12346), "© 2016-2023 Disc Soft FZE LLC.");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool IsLoggerEnabled
		{
			get
			{
				this.OnPropertyChanged("IsStandartLoggingEnabled");
				this.OnPropertyChanged("IsBootLoggingEnabled");
				if (this.IsStandartLogging)
				{
					return this.IsStandartLoggingEnabled;
				}
				return this.IsBootLoggingEnabled;
			}
		}

		public bool IsGUILogger
		{
			get
			{
				return Convert.ToBoolean(RegistryHelper.GetValue("Preferences\\General\\Logger", "IsGUILogger", 0, false));
			}
			set
			{
				RegistryHelper.SetValue("Preferences\\General\\Logger", "IsGUILogger", Convert.ToInt32(value));
				this.StartStopCommand.RaiseCanExecuteChanged();
			}
		}

		public bool IsServiceLoggingEnabled { get; set; }

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool IsServiceLogging
		{
			get
			{
				return this._isServiceLogging;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isServiceLogging, value, "IsServiceLogging"))
				{
					this.StartStopCommand.RaiseCanExecuteChanged();
				}
			}
		}

		public bool IsGuiLoggingEnabled
		{
			get
			{
				return this.IsGUILogger;
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool IsGuiLogging
		{
			get
			{
				return this._isGuiLogging;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isGuiLogging, value, "IsGuiLogging"))
				{
					this.StartStopCommand.RaiseCanExecuteChanged();
				}
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool IsBootLoggingEnabled { get; set; }

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool IsBootLogging
		{
			get
			{
				return this._isBootLogging;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isBootLogging, value, "IsBootLogging"))
				{
					this.OnPropertyChanged("IsLoggerEnabled");
					this.StartStopCommand.RaiseCanExecuteChanged();
				}
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool IsStandartLoggingEnabled
		{
			get
			{
				return this.IsServiceLoggingEnabled || this.IsGuiLoggingEnabled;
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool IsStandartLogging
		{
			get
			{
				return !this.IsBootLogging;
			}
		}

		public static bool IsLoggerShow
		{
			get
			{
				return UACHelper.IsAdminRights();
			}
		}

		private bool RestartWithAdminRights { get; set; }

		private bool CanExecuteStartStop()
		{
			return !this.IsStandartLogging || this.IsServiceLogging || this.IsGuiLogging;
		}

		private void StartStop()
		{
			if (this.IsStandartLogging)
			{
				bool flag = true;
				if (this.IsGuiLogging)
				{
					flag = this.EnableGuiLogging();
				}
				if (this.IsServiceLogging && flag)
				{
					new DiscSoftReWASDLogman().StartStop(this.IsServiceLogging, false, !this.IsGuiLogging);
				}
				if (this.IsGuiLogging && flag)
				{
					this.RestartApp();
				}
			}
			else
			{
				new DiscSoftReWASDLogman().StartStop(false, this.IsBootLogging, true);
			}
			this.UpdateLoggerProperties();
			this.OnPropertyChanged("IsServiceLogging");
			this.OnPropertyChanged("IsGuiLogging");
			this.OnPropertyChanged("IsLoggerEnabled");
		}

		public DelegateCommand StartStopCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._startStopCommand) == null)
				{
					delegateCommand = (this._startStopCommand = new DelegateCommand(new Action(this.StartStop), new Func<bool>(this.CanExecuteStartStop)));
				}
				return delegateCommand;
			}
		}

		private static async Task ShutdownAgentAndEngine(bool isLogs = false)
		{
			await App.GamepadService.DisableRemap(null, true);
			TrayAgentCommunicator.ShutdownAgentAndEngine();
			await Task.Delay(200);
			App.ReleaseMutex();
		}

		private void StartProcess()
		{
			try
			{
				if (this.IsGUILogger || this.RestartWithAdminRights)
				{
					new Process
					{
						StartInfo = 
						{
							WorkingDirectory = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath),
							FileName = "Rewasd.exe"
						}
					}.Start();
				}
				else
				{
					Process.Start(new ProcessStartInfo("cmd.exe")
					{
						Arguments = "/C explorer.exe " + Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Rewasd.exe",
						WindowStyle = ProcessWindowStyle.Hidden
					});
				}
			}
			catch (Exception)
			{
			}
		}

		private bool EnableGuiLogging()
		{
			if (!this.IsGUILogger && DTMessageBox.Show(string.Format(DTLocalization.GetString(this.IsGUILogger ? 12368 : 12367), "reWASD"), MessageBoxButton.YesNo, MessageBoxImage.Question, null, false, MessageBoxResult.None) != MessageBoxResult.Yes)
			{
				return false;
			}
			if (this.IsGUILogger)
			{
				string text = string.Format(DTLocalization.GetString(12368), "reWASD");
				string text2 = string.Format(DTLocalization.GetString(11814), "reWASD");
				if (DTMessageBox.Show(System.Windows.Application.Current.MainWindow, text, text2, MessageBoxButton.OKCancel, MessageBoxImage.Asterisk, DTLocalization.GetString(12195), false, 0.0, MessageBoxResult.None, DTLocalization.GetString(12374), null, null) == MessageBoxResult.OK)
				{
					Process.Start(new ProcessStartInfo(Constants.LOG_ROOT_PATH)
					{
						UseShellExecute = true
					});
				}
				else
				{
					this.RestartWithAdminRights = true;
				}
			}
			this.IsGUILogger = !this.IsGUILogger;
			return true;
		}

		private async void RestartApp()
		{
			WaitDialog.ShowDialogStatic(DTLocalization.GetString(12271), null, null, false, false, null, null);
			await PreferencesGeneralVM.ShutdownAgentAndEngine(false);
			this.StartProcess();
			WaitDialog.TryCloseWaitDialog();
			Environment.Exit(0);
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public string CheckForUpdateText
		{
			get
			{
				return DTLocalization.GetString(App.LicensingService.NewVersionAvailable ? 11840 : 11130);
			}
		}

		public async void CheckForUpdate()
		{
			this.IsUpdateChecking = true;
			await App.LicensingService.CheckForUpdate();
			this.IsUpdateChecking = false;
			this.OnPropertyChanged("CheckForUpdateText");
		}

		public override void Refresh()
		{
			this.OnPropertyChanged("CheckForUpdateText");
		}

		private void UpdateLoggerProperties()
		{
			this.IsServiceLoggingEnabled = new DiscSoftReWASDLogman().LogmanTracers.Where((LogmanListener x) => !x.IsBootLogging).Any((LogmanListener x) => x.GetIsLogging());
			this.IsBootLoggingEnabled = new DiscSoftReWASDLogman().LogmanTracers.Where((LogmanListener x) => x.IsBootLogging).Any((LogmanListener x) => x.GetIsLogging());
		}

		public override Task Initialize()
		{
			this.SendAnonymousStatistic = Convert.ToBoolean(RegistryHelper.GetValue("", "AllowAnonymousReport", 1, false));
			this.HookControllerButtons = Convert.ToBoolean(RegistryHelper.GetValue("Config", "HookControllerButtons", 1, false));
			this.RepressButtonForShift = Convert.ToBoolean(RegistryHelper.GetValue("Config", "RepressButtonForShift", 1, false));
			this.LeaveVirtualControllerForOfflineController = Convert.ToBoolean(RegistryHelper.GetValue("Config", "LeaveVirtualControllerForOfflineController", 0, false));
			this.ShowReleasePageOnStartup = Convert.ToBoolean(RegistryHelper.GetValue("Config", "ShowReleasePageOnStartup", 1, false));
			this.GetRecommendedConfigsWithUpdates = Convert.ToBoolean(RegistryHelper.GetValue("Config", "GetRecommendedConfigsWithUpdates", 1, false));
			this.HideLockedFeatures = Convert.ToBoolean(RegistryHelper.GetValue("Config", "HideLockedFeatures", 0, false));
			this.PreventSleepIfComboIsBeingEmulated = Convert.ToBoolean(RegistryHelper.GetValue("Config", "PreventSleepIfComboIsBeingEmulated", 0, false));
			this.TurnOffRemapByAltCtrlDel = Convert.ToBoolean(RegistryHelper.GetValue("Options", "DisableProfilesOnCAD", 1, true));
			TranslationManager.Instance.LanguageChanged += delegate([Nullable(2)] object s, EventArgs e)
			{
				this.OnPropertyChanged("CheckForUpdateText");
				this.OnPropertyChanged("PrivacyPolicyString");
			};
			this.IsFilledSVGTheme = ThemeManager.Instance.IsFilledSVGTheme;
			this.IsTransparentSVGTheme = !ThemeManager.Instance.IsFilledSVGTheme;
			if (PreferencesGeneralVM.IsLoggerShow)
			{
				this.UpdateLoggerProperties();
			}
			if (this.IsStandartLoggingEnabled)
			{
				this.IsServiceLogging = this.IsServiceLoggingEnabled;
				this.IsGuiLogging = this.IsGuiLoggingEnabled;
			}
			else if (this.IsBootLoggingEnabled)
			{
				this.IsBootLogging = true;
			}
			else
			{
				this.IsServiceLogging = true;
				this.IsGuiLogging = true;
			}
			this.SetDescription();
			this.InitLanguages();
			return Task.CompletedTask;
		}

		public override Task<bool> ApplyChanges()
		{
			bool flag = Convert.ToBoolean(RegistryHelper.GetValue("Config", "HookControllerButtons", 1, false));
			if (Convert.ToBoolean(RegistryHelper.GetValue("Config", "RepressButtonForShift", 1, false)) != this.RepressButtonForShift)
			{
				base.FireRequiredEnableRemap();
			}
			RegistryHelper.SetValue("Config", "LeaveVirtualControllerForOfflineController", Convert.ToInt32(this.LeaveVirtualControllerForOfflineController));
			RegistryHelper.SetValue("Config", "HookControllerButtons", Convert.ToInt32(this.HookControllerButtons));
			RegistryHelper.SetValue("Config", "RepressButtonForShift", Convert.ToInt32(this.RepressButtonForShift));
			RegistryHelper.SetValue("Config", "HideLockedFeatures", Convert.ToInt32(this.HideLockedFeatures));
			RegistryHelper.SetValue("Config", "ShowReleasePageOnStartup", Convert.ToInt32(this.ShowReleasePageOnStartup));
			RegistryHelper.SetValue("Config", "GetRecommendedConfigsWithUpdates", Convert.ToInt32(this.GetRecommendedConfigsWithUpdates));
			RegistryHelper.SetValue("Config", "PreventSleepIfComboIsBeingEmulated", Convert.ToInt32(this.PreventSleepIfComboIsBeingEmulated));
			RegistryHelper.SetValue("", "AllowAnonymousReport", Convert.ToInt32(this.SendAnonymousStatistic));
			if (this.TurnOffRemapByAltCtrlDel != Convert.ToBoolean(RegistryHelper.GetValue("Options", "DisableProfilesOnCAD", 1, true)))
			{
				AdminOperationsDecider adminOperationsDecider = (AdminOperationsDecider)IContainerProviderExtensions.Resolve<IAdminOperations>(App.Container);
				if (adminOperationsDecider != null && adminOperationsDecider.RunWCFIfNeeded())
				{
					adminOperationsDecider.SetDisableOnCadValue(this.TurnOffRemapByAltCtrlDel);
				}
			}
			this.TurnOffRemapByAltCtrlDel = Convert.ToBoolean(RegistryHelper.GetValue("Options", "DisableProfilesOnCAD", 1, true));
			if (flag != this.HookControllerButtons)
			{
				App.KeyPressedPollerService.IsPollingAllowedViaSettings = this.HookControllerButtons;
			}
			if (this.IsFilledSVGTheme)
			{
				ThemeManager.Instance.SetFilledSVGTheme();
			}
			else
			{
				ThemeManager.Instance.SetTransparentSVGTheme();
			}
			this.SaveLanguage();
			return Task.FromResult<bool>(true);
		}

		private void SaveLanguage()
		{
			this._localizationManager.SetActiveLanguage(this.ActiveLanguage);
			TranslationManager.Instance.OnLanguageChanged();
		}

		private void InitLanguages()
		{
			this.Languages = new ObservableCollection<Language>();
			int languagesCount = this._localizationManager.GetLanguagesCount();
			for (int i = 0; i < languagesCount; i++)
			{
				this.Languages.Add(this._localizationManager.GetLanguage(i));
			}
			this.ActiveLanguage = this._localizationManager.GetActiveLanguage();
		}

		private void SetDescription()
		{
			base.Description = ((!this.HookControllerButtons) ? new Localizable(11967) : new Localizable());
		}

		private async Task<bool> ClearESP32Data()
		{
			foreach (ExternalDevice device in App.GamepadService.ExternalDevices)
			{
				if (device.DeviceType == 2)
				{
					bool flag = !string.IsNullOrEmpty(device.HardwareDongleBluetoothMacAddress);
					TaskAwaiter<bool> taskAwaiter2;
					if (flag)
					{
						TaskAwaiter<bool> taskAwaiter = App.AdminOperations.DeleteESP32DeviceRegKeys(device.HardwareDongleBluetoothMacAddress.Replace(":", "-").ToLower()).GetAwaiter();
						if (!taskAwaiter.IsCompleted)
						{
							await taskAwaiter;
							taskAwaiter = taskAwaiter2;
							taskAwaiter2 = default(TaskAwaiter<bool>);
						}
						flag = !taskAwaiter.GetResult();
					}
					if (flag)
					{
						return false;
					}
					if (device.IsSerialPortFTDI && device.DefaultLatencySpeed != 0 && device.DefaultLatencySpeed != device.LatencySpeed)
					{
						TaskAwaiter<bool> taskAwaiter = App.AdminOperations.ChangeESP32DeviceLatency(device.SerialPort, device.DefaultLatencySpeed).GetAwaiter();
						if (!taskAwaiter.IsCompleted)
						{
							await taskAwaiter;
							taskAwaiter = taskAwaiter2;
							taskAwaiter2 = default(TaskAwaiter<bool>);
						}
						if (!taskAwaiter.GetResult())
						{
							return false;
						}
					}
				}
				device = null;
			}
			IEnumerator<ExternalDevice> enumerator = null;
			return true;
		}

		public async void ClearData()
		{
			if (DTMessageBox.Show(DTLocalization.GetString(11913), MessageBoxButton.YesNo, MessageBoxImage.Question, null, false, MessageBoxResult.None) == MessageBoxResult.Yes)
			{
				TaskAwaiter<bool> taskAwaiter = this.ClearESP32Data().GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<bool> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				if (taskAwaiter.GetResult())
				{
					WaitDialog.ShowDialogStatic(DTLocalization.GetString(12271), null, null, false, false, null, null);
					await PreferencesGeneralVM.ShutdownAgentAndEngine(false);
					RegistryHelper.DeleteValue("TrayAgent", "CustomTrayColor");
					List<string> list = Directory.GetFiles(Constants.PROGRAMM_DATA_DIRECTORY_PATH, "*.binv2").ToList<string>();
					Action<string> action;
					if ((action = PreferencesGeneralVM.<>O.<0>__Delete) == null)
					{
						action = (PreferencesGeneralVM.<>O.<0>__Delete = new Action<string>(File.Delete));
					}
					list.ForEach(action);
					Directory.GetFiles(Constants.PROGRAMM_DATA_DIRECTORY_PATH, "*.binv1").ToList<string>().ForEach(delegate(string file)
					{
						File.Delete(file);
					});
					Directory.GetFiles(Constants.PROGRAMM_DATA_DIRECTORY_PATH, "*.json").ToList<string>().ForEach(delegate(string file)
					{
						File.Delete(file);
					});
					this.StartProcess();
					WaitDialog.TryCloseWaitDialog();
					Environment.Exit(0);
				}
			}
		}

		private bool _sendAnonymousStatistic;

		private bool _hideLockedFeatures;

		private bool _showReleasePageOnStartup;

		private bool _getRecommendedConfigsWithUpdates;

		private bool _hookControllerButtons;

		private bool _repressButtonForShift;

		private bool _leaveVirtualControllerForOfflineController;

		private bool _preventSleepIfComboIsBeingEmulated;

		private bool _isFilledSVGTheme;

		private bool _isTransparentSVGTheme;

		private bool _turnOffRemapByAltCtrlDel;

		private Language _activeLanguage;

		private bool _isUpdateChecking;

		private ObservableCollection<Language> _languages;

		private LocalizationManager _localizationManager = LocalizationManager.Instance;

		private bool _isServiceLogging;

		private bool _isGuiLogging;

		private bool _isBootLogging;

		private DelegateCommand _startStopCommand;

		[CompilerGenerated]
		private static class <>O
		{
			public static Action<string> <0>__Delete;
		}
	}
}
