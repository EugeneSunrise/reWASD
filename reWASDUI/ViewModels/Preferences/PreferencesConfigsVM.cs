using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using Prism.Commands;
using reWASDCommon.Infrastructure;
using reWASDUI.DataModels;
using reWASDUI.ViewModels.Preferences.Base;

namespace reWASDUI.ViewModels.Preferences
{
	public class PreferencesConfigsVM : PreferencesBaseVM
	{
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

		public DelegateCommand OpenConfigsFolderCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._openConfigsFolder) == null)
				{
					delegateCommand = (this._openConfigsFolder = new DelegateCommand(new Action(this.OpenConfigsFolder)));
				}
				return delegateCommand;
			}
		}

		public DelegateCommand OpenPresetsFolderCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._openPresetsFolder) == null)
				{
					delegateCommand = (this._openPresetsFolder = new DelegateCommand(new Action(this.OpenPresetsFolder)));
				}
				return delegateCommand;
			}
		}

		public DelegateCommand OpenScreenshotsFolderCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._openScreenshotsFolder) == null)
				{
					delegateCommand = (this._openScreenshotsFolder = new DelegateCommand(new Action(this.OpenScreenshotsFolder)));
				}
				return delegateCommand;
			}
		}

		public DelegateCommand ChangeScreenshotsFolderCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._changeScreenshotsFolder) == null)
				{
					delegateCommand = (this._changeScreenshotsFolder = new DelegateCommand(new Action(this.ChangeScreenshotsFolder)));
				}
				return delegateCommand;
			}
		}

		public DelegateCommand ChangeConfigsFolderCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._changeConfigsFolder) == null)
				{
					delegateCommand = (this._changeConfigsFolder = new DelegateCommand(new Action(this.ChangeConfigsFolder)));
				}
				return delegateCommand;
			}
		}

		public RelayCommand RestoreConfigsFolderCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._restoreConfigsFolder) == null)
				{
					relayCommand = (this._restoreConfigsFolder = new RelayCommand(new Action(this.RestoreConfigsFolder), new Func<bool>(this.RestoreConfigsFolderCanExecute)));
				}
				return relayCommand;
			}
		}

		private bool RestoreConfigsFolderCanExecute()
		{
			return this.ConfigsFolderPath != Path.Combine(Constants.COMMON_DOCUMENTS_DIRECTORY_PATH, "Profiles");
		}

		public RelayCommand RestoreScreenshotsFolderCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._restoreScreenshotsFolder) == null)
				{
					relayCommand = (this._restoreScreenshotsFolder = new RelayCommand(new Action(this.RestoreScreenshotsFolder), new Func<bool>(this.RestoreScreenshotsFolderCanExecute)));
				}
				return relayCommand;
			}
		}

		private bool RestoreScreenshotsFolderCanExecute()
		{
			return this.ScreenshotsFolderPath != Path.Combine(Constants.COMMON_DOCUMENTS_DIRECTORY_PATH, "Screenshots");
		}

		public RelayCommand RestorePresetsFolderCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._restorePresetsFolder) == null)
				{
					relayCommand = (this._restorePresetsFolder = new RelayCommand(new Action(this.RestorePresetsFolder), new Func<bool>(this.RestorePresetsFolderCanExecute)));
				}
				return relayCommand;
			}
		}

		private bool RestorePresetsFolderCanExecute()
		{
			return this.PresetsFolderPath != Path.Combine(Constants.COMMON_DOCUMENTS_DIRECTORY_PATH, "Presets");
		}

		public DelegateCommand ChangePresetsFolderCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._changePresetsFolder) == null)
				{
					delegateCommand = (this._changePresetsFolder = new DelegateCommand(new Action(this.ChangePresetsFolder)));
				}
				return delegateCommand;
			}
		}

		public override Task Initialize()
		{
			this.ConfigsFolderPath = base.UserSettingsService.ConfigsFolderPath;
			this.PresetsFolderPath = base.UserSettingsService.PresetsFolderPath;
			this.ScreenshotsFolderPath = base.UserSettingsService.ScreenshotsFolderPath;
			base.Description = new Localizable(11380);
			return Task.CompletedTask;
		}

		public override async Task<bool> ApplyChanges()
		{
			if (base.UserSettingsService.ScreenshotsFolderPath != this.ScreenshotsFolderPath && this.MoveScreenShots())
			{
				base.UserSettingsService.ScreenshotsFolderPath = this.ScreenshotsFolderPath;
				await App.UserSettingsService.Save();
			}
			this.ScreenshotsFolderPath = base.UserSettingsService.ScreenshotsFolderPath;
			if (base.UserSettingsService.PresetsFolderPath != this.PresetsFolderPath)
			{
				if (this.MovePresets())
				{
					base.UserSettingsService.PresetsFolderPath = this.PresetsFolderPath;
					await App.UserSettingsService.Save();
				}
				await App.GameProfilesService.FillPresetsCollection(true);
			}
			this.PresetsFolderPath = base.UserSettingsService.PresetsFolderPath;
			if (base.UserSettingsService.ConfigsFolderPath != this.ConfigsFolderPath)
			{
				if (App.GamepadService.CurrentGamepad != null)
				{
					GameVM currentGame = App.GameProfilesService.CurrentGame;
					if (currentGame != null)
					{
						ConfigVM currentConfig = currentGame.CurrentConfig;
						if (currentConfig != null)
						{
							currentConfig.ChangeCurrentMainWrapperAccordingToControllerVM(App.GamepadService.CurrentGamepad, true);
						}
					}
				}
				await App.GameProfilesService.SetCurrentGame(null, true);
				if (App.GameProfilesService.CurrentGame != null)
				{
					return false;
				}
				await App.GamepadService.DisableRemap(null, true);
				if (this.MoveConfigs())
				{
					base.UserSettingsService.ConfigsFolderPath = this.ConfigsFolderPath;
					await App.UserSettingsService.Save();
				}
				await App.GameProfilesService.FillGamesCollection();
			}
			this.ConfigsFolderPath = base.UserSettingsService.ConfigsFolderPath;
			return true;
		}

		private void OpenConfigsFolder()
		{
			DSUtils.GoUrl(this.GetExistingConfigsFolder());
		}

		private void OpenPresetsFolder()
		{
			DSUtils.GoUrl(this.GetExistingPresetsFolder());
		}

		private void OpenScreenshotsFolder()
		{
			DSUtils.GoUrl(this.GetExistingScreenshotsFolder());
		}

		private void ChangeScreenshotsFolder()
		{
			string text = DSUtils.BrowseFolder(this.GetExistingScreenshotsFolder());
			if (text != this.ScreenshotsFolderPath && text != base.UserSettingsService.ScreenshotsFolderPath)
			{
				if (!text.Contains(base.UserSettingsService.ScreenshotsFolderPath))
				{
					this.ScreenshotsFolderPath = Path.Combine(text, "Screenshots");
					return;
				}
				DTMessageBox.Show(null, DTLocalization.GetString(12648), "", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
		}

		private string GetExistingConfigsFolder()
		{
			string text = this.ConfigsFolderPath;
			if (!Directory.Exists(text))
			{
				try
				{
					text = text.Substring(0, text.Length - "Profiles".Length - 1);
				}
				catch (Exception)
				{
					text = Constants.COMMON_DOCUMENTS_DIRECTORY_PATH;
				}
			}
			return text;
		}

		private string GetExistingPresetsFolder()
		{
			string text = this.PresetsFolderPath;
			if (!Directory.Exists(text))
			{
				try
				{
					text = text.Substring(0, text.Length - "Presets".Length - 1);
				}
				catch (Exception)
				{
					text = Constants.COMMON_DOCUMENTS_DIRECTORY_PATH;
				}
			}
			return text;
		}

		private string GetExistingScreenshotsFolder()
		{
			string text = this.ScreenshotsFolderPath;
			if (!Directory.Exists(text))
			{
				try
				{
					text = text.Substring(0, text.Length - "Screenshots".Length - 1);
				}
				catch (Exception)
				{
					text = Constants.COMMON_DOCUMENTS_DIRECTORY_PATH;
				}
			}
			return text;
		}

		private void ChangeConfigsFolder()
		{
			string text = DSUtils.BrowseFolder(this.GetExistingConfigsFolder());
			if (text != this.ConfigsFolderPath && text != base.UserSettingsService.ConfigsFolderPath)
			{
				if (!text.Contains(base.UserSettingsService.ConfigsFolderPath))
				{
					this.ConfigsFolderPath = Path.Combine(text, "Profiles");
					return;
				}
				DTMessageBox.Show(null, DTLocalization.GetString(12648), "", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
		}

		private void RestoreConfigsFolder()
		{
			if (DTMessageBox.Show(DTLocalization.GetString(12672), DTLocalization.GetString(5113), MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
			{
				this.ConfigsFolderPath = Path.Combine(Constants.COMMON_DOCUMENTS_DIRECTORY_PATH, "Profiles");
			}
		}

		private void RestoreScreenshotsFolder()
		{
			if (DTMessageBox.Show(DTLocalization.GetString(12672), DTLocalization.GetString(5113), MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
			{
				this.ScreenshotsFolderPath = Path.Combine(Constants.COMMON_DOCUMENTS_DIRECTORY_PATH, "Screenshots");
			}
		}

		private void RestorePresetsFolder()
		{
			if (DTMessageBox.Show(DTLocalization.GetString(12672), DTLocalization.GetString(5113), MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
			{
				this.PresetsFolderPath = Path.Combine(Constants.COMMON_DOCUMENTS_DIRECTORY_PATH, "Presets");
			}
		}

		private void ChangePresetsFolder()
		{
			string text = DSUtils.BrowseFolder(this.GetExistingPresetsFolder());
			if (text != this.PresetsFolderPath && text != base.UserSettingsService.PresetsFolderPath)
			{
				if (!text.Contains(base.UserSettingsService.PresetsFolderPath))
				{
					this.PresetsFolderPath = Path.Combine(text, "Presets");
					return;
				}
				DTMessageBox.Show(null, DTLocalization.GetString(12648), "", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
		}

		private bool MoveConfigs()
		{
			if (Directory.Exists(this.ConfigsFolderPath) && !DSUtils.IsDirectoryEmpty(this.ConfigsFolderPath))
			{
				DTMessageBox.Show(null, DTLocalization.GetString(11988) + " " + string.Format(DTLocalization.GetString(11989), this.ConfigsFolderPath), "", MessageBoxButton.OK, MessageBoxImage.Hand);
				return false;
			}
			if (Directory.Exists(this.ConfigsFolderPath))
			{
				Directory.Delete(this.ConfigsFolderPath);
			}
			try
			{
				if (!string.IsNullOrEmpty(base.UserSettingsService.ConfigsFolderPath))
				{
					if (Directory.Exists(base.UserSettingsService.ConfigsFolderPath))
					{
						DSUtils.DirectoryCopy(base.UserSettingsService.ConfigsFolderPath, this.ConfigsFolderPath, true);
					}
					else
					{
						Directory.CreateDirectory(this.ConfigsFolderPath);
					}
				}
			}
			catch (Exception)
			{
				DTMessageBox.Show(null, DTLocalization.GetString(11988), "", MessageBoxButton.OK, MessageBoxImage.Hand);
				return false;
			}
			try
			{
				Directory.Delete(base.UserSettingsService.ConfigsFolderPath, true);
			}
			catch (Exception)
			{
			}
			return true;
		}

		private bool MovePresets()
		{
			if (Directory.Exists(this.PresetsFolderPath) && !DSUtils.IsDirectoryEmpty(this.PresetsFolderPath))
			{
				DTMessageBox.Show(null, DTLocalization.GetString(11988) + " " + string.Format(DTLocalization.GetString(11989), this.PresetsFolderPath), "", MessageBoxButton.OK, MessageBoxImage.Hand);
				return false;
			}
			if (Directory.Exists(this.PresetsFolderPath))
			{
				Directory.Delete(this.PresetsFolderPath);
			}
			try
			{
				if (!string.IsNullOrEmpty(base.UserSettingsService.PresetsFolderPath))
				{
					if (Directory.Exists(base.UserSettingsService.PresetsFolderPath))
					{
						DSUtils.DirectoryCopy(base.UserSettingsService.PresetsFolderPath, this.PresetsFolderPath, true);
					}
					else
					{
						Directory.CreateDirectory(this.PresetsFolderPath);
					}
				}
			}
			catch (Exception)
			{
				DTMessageBox.Show(null, DTLocalization.GetString(11988), "", MessageBoxButton.OK, MessageBoxImage.Hand);
				return false;
			}
			try
			{
				Directory.Delete(base.UserSettingsService.PresetsFolderPath, true);
			}
			catch (Exception)
			{
			}
			return true;
		}

		private bool MoveScreenShots()
		{
			if (Directory.Exists(this.ScreenshotsFolderPath) && !DSUtils.IsDirectoryEmpty(this.ScreenshotsFolderPath))
			{
				DTMessageBox.Show(null, DTLocalization.GetString(11988) + " " + string.Format(DTLocalization.GetString(11989), this.ScreenshotsFolderPath), "", MessageBoxButton.OK, MessageBoxImage.Hand);
				return false;
			}
			if (Directory.Exists(this.ScreenshotsFolderPath))
			{
				Directory.Delete(this.ScreenshotsFolderPath);
			}
			try
			{
				if (!string.IsNullOrEmpty(base.UserSettingsService.ScreenshotsFolderPath))
				{
					if (Directory.Exists(base.UserSettingsService.ScreenshotsFolderPath))
					{
						DSUtils.DirectoryCopy(base.UserSettingsService.ScreenshotsFolderPath, this.ScreenshotsFolderPath, true);
					}
					else
					{
						Directory.CreateDirectory(this.ScreenshotsFolderPath);
					}
				}
			}
			catch (Exception)
			{
				DTMessageBox.Show(null, DTLocalization.GetString(11988), "", MessageBoxButton.OK, MessageBoxImage.Hand);
				return false;
			}
			try
			{
				Directory.Delete(base.UserSettingsService.ScreenshotsFolderPath, true);
			}
			catch (Exception)
			{
			}
			return true;
		}

		private string _screenshotsFolderPath;

		private string _presetsFolderPath;

		private string _configsFolderPath;

		private DelegateCommand _openConfigsFolder;

		private DelegateCommand _openPresetsFolder;

		private DelegateCommand _openScreenshotsFolder;

		private DelegateCommand _changeScreenshotsFolder;

		private DelegateCommand _changeConfigsFolder;

		private RelayCommand _restoreConfigsFolder;

		private RelayCommand _restoreScreenshotsFolder;

		private RelayCommand _restorePresetsFolder;

		private DelegateCommand _changePresetsFolder;
	}
}
