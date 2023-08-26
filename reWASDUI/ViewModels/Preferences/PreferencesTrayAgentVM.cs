using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using DiscSoft.NET.Common.ColorStuff;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Attributes;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using Microsoft.Win32;
using Prism.Commands;
using reWASDCommon.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.ViewModels.Preferences.Base;
using XBEliteWPF.Infrastructure;

namespace reWASDUI.ViewModels.Preferences
{
	public class PreferencesTrayAgentVM : PreferencesBaseVM
	{
		public ObservableCollection<GamepadSettings> GamepadsSettings
		{
			get
			{
				return this._gamepadsSettings;
			}
			set
			{
				this.SetProperty<ObservableCollection<GamepadSettings>>(ref this._gamepadsSettings, value, "GamepadsSettings");
			}
		}

		public zColor CustomTrayColor
		{
			get
			{
				return this._customTrayColor;
			}
			set
			{
				this.SetProperty<zColor>(ref this._customTrayColor, value, "CustomTrayColor");
			}
		}

		private void OnColorChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			base.FireOptionChanged();
		}

		public bool AutorunService
		{
			get
			{
				return this._autorunService;
			}
			set
			{
				if (value == this._autorunService)
				{
					return;
				}
				this._autorunService = value;
				this.SetDescription();
				this.OnPropertyChanged("AutorunService");
				this.OnPropertyChanged("AutoRemap");
			}
		}

		public bool ShowTrayIcons
		{
			get
			{
				return this._showTrayIcons;
			}
			set
			{
				if (value == this._showTrayIcons)
				{
					return;
				}
				this._showTrayIcons = value;
				this.OnPropertyChanged("ShowTrayIcons");
			}
		}

		public bool UseAgent
		{
			get
			{
				return this._useAgent;
			}
			set
			{
				if (value == this._useAgent)
				{
					return;
				}
				this._useAgent = value;
				this.OnPropertyChanged("UseAgent");
			}
		}

		public bool AutoRemap
		{
			get
			{
				return this._autoremap && this.AutorunService;
			}
			set
			{
				if (value == this._autoremap)
				{
					return;
				}
				this._autoremap = value;
				this.OnPropertyChanged("AutoRemap");
			}
		}

		public bool RestoreXBOXEliteSlot
		{
			get
			{
				return this._restoreXBOXEliteSlot && this.AutorunService;
			}
			set
			{
				if (value == this._restoreXBOXEliteSlot)
				{
					return;
				}
				this._restoreXBOXEliteSlot = value;
				this.OnPropertyChanged("RestoreXBOXEliteSlot");
			}
		}

		public bool TurnRemapOffOnLostFocus
		{
			get
			{
				return this._turnRemapOffOnLostFocus;
			}
			set
			{
				if (value == this._turnRemapOffOnLostFocus)
				{
					return;
				}
				this._turnRemapOffOnLostFocus = value;
				this.OnPropertyChanged("TurnRemapOffOnLostFocus");
			}
		}

		public ObservableCollection<GamepadColor> GamepadColors
		{
			get
			{
				return new ObservableCollection<GamepadColor>(Enum.GetValues(typeof(GamepadColor)).OfType<GamepadColor>());
			}
		}

		public PreferencesTrayAgentVM()
		{
			App.GamepadService.AllPhysicalControllers.CollectionChanged += this.AllPhysicalControllers_CollectionChanged;
			this.CustomTrayColor = new zColor((Color)ColorConverter.ConvertFromString(RegistryHelper.GetString("TrayAgent", "CustomTrayColor", Constants.LEDColorCollection[0].ToString(), false)));
			this.CustomTrayColor.PropertyChanged += this.OnColorChanged;
		}

		public override Task Initialize()
		{
			this.ShowTrayIcons = Convert.ToBoolean(RegistryHelper.GetValue("Config", "ShowTrayIcons", 1, false));
			this.UseAgent = Convert.ToBoolean(RegistryHelper.GetValue("Config", "Autorun", 1, false));
			this.AutorunService = Convert.ToBoolean(RegistryHelper.GetValue("Config", "AutorunService", 1, false));
			this.AutoRemap = Convert.ToBoolean(RegistryHelper.GetValue("Config", "AutoRemap", 1, false));
			this.RestoreXBOXEliteSlot = base.UserSettingsService.RestoreXBOXEliteSlot;
			this.TurnRemapOffOnLostFocus = Convert.ToBoolean(RegistryHelper.GetValue("Config", "TurnRemapOffOnLostFocus", 1, false));
			this.GamepadsSettings = new ObservableCollection<GamepadSettings>(App.GamepadService.GamepadsSettings);
			if (this.GamepadsSettings == null)
			{
				this.GamepadsSettings = new ObservableCollection<GamepadSettings>();
			}
			if (this.GamepadsSettings.Count != 0)
			{
				this.CurrentGamepadSettings = this.GamepadsSettings[0];
			}
			string selectedControllerId = RegistryHelper.GetString("GuiNamespace", "SelectedController", "", false);
			if (!string.IsNullOrEmpty(selectedControllerId))
			{
				try
				{
					this.CurrentGamepadSettings = this.GamepadsSettings.First((GamepadSettings item) => item.ID == selectedControllerId);
				}
				catch (Exception)
				{
				}
			}
			this.SetDescription();
			RegistryHelper.SetString("GuiNamespace", "SelectedController", "");
			this.OnPropertyChanged("IsSelectedCustomColor");
			return Task.CompletedTask;
		}

		private void AllPhysicalControllers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (BaseControllerVM baseControllerVM in e.NewItems.OfType<BaseControllerVM>())
				{
					ControllerVM controller = baseControllerVM as ControllerVM;
					if (controller != null && controller.HasGamepadControllers && controller.IsControllerBatteryBlockVisible && this.GamepadsSettings.FirstOrDefault((GamepadSettings item) => item.ID == controller.ID) == null)
					{
						this.GamepadsSettings.Add(new GamepadSettings(controller.ID, controller.ControllerDisplayName, controller.FirstControllerType));
					}
				}
				if (this.CurrentGamepadSettings == null && this.GamepadsSettings != null && this.GamepadsSettings.Count != 0)
				{
					this.CurrentGamepadSettings = this.GamepadsSettings[0];
				}
			}
			this.RemoveCurrentEntryCommand.RaiseCanExecuteChanged();
		}

		public override Task<bool> ApplyChanges()
		{
			RegistryHelper.SetValue("Config", "ShowTrayIcons", Convert.ToInt32(this.ShowTrayIcons));
			RegistryHelper.SetValue("Config", "Autorun", Convert.ToInt32(this.UseAgent));
			RegistryHelper.SetValue("Config", "AutorunService", Convert.ToInt32(this.AutorunService));
			RegistryHelper.SetValue("Config", "AutoRemap", Convert.ToInt32(this.AutoRemap));
			base.UserSettingsService.RestoreXBOXEliteSlot = this.RestoreXBOXEliteSlot;
			RegistryHelper.SetValue("Config", "TurnRemapOffOnLostFocus", Convert.ToInt32(this.TurnRemapOffOnLostFocus));
			if (this.IsSelectedCustomColor)
			{
				RegistryHelper.SetString("TrayAgent", "CustomTrayColor", this.CustomTrayColor.ToString());
			}
			if (this.AutorunService)
			{
				Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Run", "reWASD Engine", "\"" + Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\reWASDEngine.exe\"");
			}
			else
			{
				TrayAgentCommunicator.DeleteAutorunValue();
			}
			App.GamepadService.GamepadsSettings = new ObservableCollection<GamepadSettings>(this.GamepadsSettings);
			App.GamepadService.BinDataSerialize.SaveGamepadsSettings();
			return Task.FromResult<bool>(true);
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public GamepadColor SelectedColor
		{
			get
			{
				if (this._currentGamepadSettings == null)
				{
					return 0;
				}
				return this._currentGamepadSettings.Color;
			}
			set
			{
				if (this.SetProperty<GamepadColor>(ref this._currentGamepadSettings.Color, value, "SelectedColor"))
				{
					base.FireOptionChanged();
				}
				this.OnPropertyChanged("IsSelectedCustomColor");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool IsSelectedCustomColor
		{
			get
			{
				return this.SelectedColor == 15;
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public GamepadSettings CurrentGamepadSettings
		{
			get
			{
				return this._currentGamepadSettings;
			}
			set
			{
				if (this.SetProperty<GamepadSettings>(ref this._currentGamepadSettings, value, "CurrentGamepadSettings"))
				{
					if (this._currentGamepadSettings != null)
					{
						this._showIconForGamepad = this._currentGamepadSettings.ShowBatteryInTaskbar;
					}
					this.RemoveCurrentEntryCommand.RaiseCanExecuteChanged();
					this.OnPropertyChanged("ShowIconForGamepad");
					this.OnPropertyChanged("SelectedColor");
					this.OnPropertyChanged("IsSelectedCustomColor");
				}
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool ShowIconForGamepad
		{
			get
			{
				return this._showIconForGamepad;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._showIconForGamepad, value, "ShowIconForGamepad"))
				{
					base.FireOptionChanged();
					this._currentGamepadSettings.ShowBatteryInTaskbar = this._showIconForGamepad;
					this.OnPropertyChanged("ShowIconForGamepad");
				}
			}
		}

		private void SetDescription()
		{
			base.Description = ((!this.AutorunService) ? new Localizable(11383) : new Localizable());
		}

		private void CurrentGamepadSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.FireOptionChanged();
		}

		public DelegateCommand RemoveCurrentEntryCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._RemoveCurrentEntry) == null)
				{
					delegateCommand = (this._RemoveCurrentEntry = new DelegateCommand(new Action(this.RemoveCurrentEntry), new Func<bool>(this.RemoveCurrentEntryCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void RemoveCurrentEntry()
		{
			if (DTMessageBox.Show(DTLocalization.GetString(11446), MessageBoxButton.YesNo, MessageBoxImage.Question, null, false, MessageBoxResult.None) == MessageBoxResult.Yes)
			{
				this.GamepadsSettings.Remove(this.CurrentGamepadSettings);
				if (this.GamepadsSettings.Count != 0)
				{
					this.CurrentGamepadSettings = this.GamepadsSettings[0];
				}
				else
				{
					this.CurrentGamepadSettings = null;
				}
				this.RemoveCurrentEntryCommand.RaiseCanExecuteChanged();
				base.FireOptionChanged();
			}
		}

		private bool RemoveCurrentEntryCanExecute()
		{
			return this.CurrentGamepadSettings != null && App.GamepadService.AllPhysicalControllers.All((BaseControllerVM g) => g.ID != this.CurrentGamepadSettings.ID);
		}

		private bool _autorunService;

		private bool _useAgent;

		private bool _showTrayIcons;

		private bool _autoremap;

		private bool _restoreXBOXEliteSlot;

		private bool _turnRemapOffOnLostFocus;

		private LocalizationManager _localizationManager = LocalizationManager.Instance;

		private ObservableCollection<GamepadSettings> _gamepadsSettings;

		private zColor _customTrayColor;

		private GamepadSettings _currentGamepadSettings;

		private bool _showIconForGamepad;

		private DelegateCommand _RemoveCurrentEntry;
	}
}
