using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Attributes;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using Prism.Commands;
using Prism.Events;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Infrastructure.LED;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.ViewModels.Preferences.Base;
using XBEliteWPF.Infrastructure.LED;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.ViewModels.Preferences
{
	public class PreferencesLEDSettingsVM : PreferencesBaseVM
	{
		public PreferencesLEDSettingsVM()
		{
			this.FillExpands();
			foreach (object obj in Enum.GetValues(typeof(PlayerLEDMode)))
			{
				PlayerLEDMode playerLEDMode = (PlayerLEDMode)obj;
				this._playerLedCollection.Add(playerLEDMode);
			}
			App.GamepadService.GamepadsUserLedCollection.CollectionChanged += this.GamepadsCollectionChanged;
			App.GamepadService.GamepadCollection.CollectionChanged += this.GamepadsCollectionChanged;
			IEventAggregator eventAggregator = App.EventAggregator;
			if (eventAggregator == null)
			{
				return;
			}
			eventAggregator.GetEvent<CurrentGamepadChanged>().Subscribe(delegate(BaseControllerVM gamepad)
			{
				this.SetCurrentGamepad();
			});
		}

		private void FillExpands()
		{
			ObservableDictionary<LEDSupportedDevice, LEDSettingsGlobalPerDevice> ledsettingsGlobalDictionary = base.UserSettingsService.PerDeviceGlobalLedSettings.LEDSettingsGlobalDictionary;
			foreach (LEDSupportedDevice ledsupportedDevice in LEDSupportedDeviceHelper.GetUserAvailableItems())
			{
				this._expandHomeLeds[ledsupportedDevice] = false;
				this._expandRemapLeds[ledsupportedDevice] = false;
				this._expandPlayerLeds[ledsupportedDevice] = false;
				this._expandMicrophoneRemapLeds[ledsupportedDevice] = false;
				if (ledsettingsGlobalDictionary[ledsupportedDevice].IsAnyHomeLedSettings)
				{
					this._expandHomeLeds[ledsupportedDevice] = true;
				}
				else if (ledsettingsGlobalDictionary[ledsupportedDevice].IsChangeColorOnSlotAndShiftChangeAllowed)
				{
					this._expandRemapLeds[ledsupportedDevice] = true;
				}
				else if (ledsettingsGlobalDictionary[ledsupportedDevice].HasMicrophoneLed)
				{
					this._expandMicrophoneRemapLeds[ledsupportedDevice] = true;
				}
				else
				{
					this._expandPlayerLeds[ledsupportedDevice] = true;
				}
			}
		}

		public override Task Initialize()
		{
			PreferencesLEDSettingsVM.<>c__DisplayClass6_0 CS$<>8__locals1 = new PreferencesLEDSettingsVM.<>c__DisplayClass6_0();
			this.IsDictionaryChanged = false;
			this.IsLedSettingsEnabled = App.UserSettingsService.IsLedSettingsEnabled;
			base.ChangedProperties.Clear();
			base.UserSettingsService.OnLEDSettingsChanged += this.OnLEDSettingsChanged;
			this.GamepadsUserLedCollection = App.GamepadService.GamepadsUserLedCollection;
			PreferencesLEDSettingsVM.<>c__DisplayClass6_0 CS$<>8__locals2 = CS$<>8__locals1;
			BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
			LEDSupportedDevice? ledsupportedDevice;
			if (currentGamepad == null)
			{
				ledsupportedDevice = null;
			}
			else
			{
				ControllerVM currentController = currentGamepad.CurrentController;
				ledsupportedDevice = ((currentController != null) ? ControllerTypeExtensions.ConvertEnumToLEDSupportedType(currentController.ControllerType) : null);
			}
			CS$<>8__locals2.supportedLEDType = ledsupportedDevice;
			if (CS$<>8__locals1.supportedLEDType != null)
			{
				this.SelectedLedDevice = this.LEDSupportedDevices.FirstOrDefault(delegate(LEDSupportedDevice item)
				{
					LEDSupportedDevice? supportedLEDType = CS$<>8__locals1.supportedLEDType;
					return (item == supportedLEDType.GetValueOrDefault()) & (supportedLEDType != null);
				});
			}
			return Task.CompletedTask;
		}

		private void OnLEDSettingsChanged(object sender, EventArgs e)
		{
			base.FireOptionChanged();
			base.FireRequiredEnableRemap();
		}

		private void GamepadsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.SupportedGamepadsUserLedCollection = this.GamepadsUserLedCollection.Where((KeyValuePair<string, PlayerLedSettings> item) => item.Value.SupportedDeviceType == this.SelectedLedDevice);
			this.RemoveDeviceEntryCommand.RaiseCanExecuteChanged();
		}

		private void GamepadsUserLedCollection_CollectionItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.IsDictionaryChanged = true;
		}

		private void SetCurrentGamepad()
		{
			KeyValuePair<string, PlayerLedSettings> keyValuePair = this.SupportedGamepadsUserLedCollection.FirstOrDefault(delegate(KeyValuePair<string, PlayerLedSettings> gs)
			{
				string key = gs.Key;
				BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
				return key == ((currentGamepad != null) ? currentGamepad.ID : null);
			});
			if (keyValuePair.Value == null && this.SupportedGamepadsUserLedCollection.Count<KeyValuePair<string, PlayerLedSettings>>() > 0)
			{
				keyValuePair = this.SupportedGamepadsUserLedCollection.First<KeyValuePair<string, PlayerLedSettings>>();
			}
			this.CurrentPlayerLedPair = keyValuePair;
		}

		public override async Task<bool> ApplyChanges()
		{
			App.UserSettingsService.IsLedSettingsEnabled = this.IsLedSettingsEnabled;
			await App.GamepadService.BinDataSerialize.SaveGamepadsUserLedCollection();
			await App.HttpClientService.Gamepad.SavePerDeviceGlobalLedSettings(base.UserSettingsService.PerDeviceGlobalLedSettings);
			base.UserSettingsService.PerDeviceGlobalLedSettings.OnLEDSettingsChanged += this.OnLEDSettingsChanged;
			this.IsDictionaryChanged = false;
			base.ChangedProperties.Clear();
			return true;
		}

		public ObservableCollection<LEDSupportedDevice> LEDSupportedDevices
		{
			get
			{
				return new ObservableCollection<LEDSupportedDevice>(LEDSupportedDeviceHelper.GetUserAvailableItems());
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public ObservableCollection<int> PlayerLEDNumbers
		{
			get
			{
				return this._playerLEDNumbers;
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public GamepadsPlayerLedDictionary GamepadsUserLedCollection
		{
			get
			{
				return this._gamepadsUserLedCollection;
			}
			set
			{
				GamepadsPlayerLedDictionary gamepadsUserLedCollection = this._gamepadsUserLedCollection;
				if (this.SetProperty<GamepadsPlayerLedDictionary>(ref this._gamepadsUserLedCollection, value, "GamepadsUserLedCollection"))
				{
					if (gamepadsUserLedCollection != null)
					{
						gamepadsUserLedCollection.CollectionItemPropertyChanged -= this.GamepadsUserLedCollection_CollectionItemPropertyChanged;
					}
					if (value != null)
					{
						this._gamepadsUserLedCollection.CollectionItemPropertyChanged += this.GamepadsUserLedCollection_CollectionItemPropertyChanged;
					}
					this.SupportedGamepadsUserLedCollection = this.GamepadsUserLedCollection.Where((KeyValuePair<string, PlayerLedSettings> item) => item.Value.SupportedDeviceType == this.SelectedLedDevice);
				}
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public IEnumerable<KeyValuePair<string, PlayerLedSettings>> SupportedGamepadsUserLedCollection
		{
			get
			{
				return this._supportedGamepadsUserLedCollection;
			}
			set
			{
				if (this.SetProperty<IEnumerable<KeyValuePair<string, PlayerLedSettings>>>(ref this._supportedGamepadsUserLedCollection, value, "SupportedGamepadsUserLedCollection"))
				{
					this.SetCurrentPair();
				}
			}
		}

		private void SetCurrentPair()
		{
			if (!string.IsNullOrEmpty(this.CurrentPlayerLedPair.Key) && this.SupportedGamepadsUserLedCollection.Any((KeyValuePair<string, PlayerLedSettings> el) => el.Key == this.CurrentPlayerLedPair.Key))
			{
				this.CurrentPlayerLedPair = this.SupportedGamepadsUserLedCollection.FirstOrDefault((KeyValuePair<string, PlayerLedSettings> kvp) => kvp.Key == this.CurrentPlayerLedPair.Key);
				return;
			}
			this.SetCurrentGamepad();
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public KeyValuePair<string, PlayerLedSettings> CurrentPlayerLedPair
		{
			get
			{
				return this._currentPlayerLedPair;
			}
			set
			{
				if (this.SetProperty<KeyValuePair<string, PlayerLedSettings>>(ref this._currentPlayerLedPair, value, "CurrentPlayerLedPair"))
				{
					this.RemoveDeviceEntryCommand.RaiseCanExecuteChanged();
					this.OnPropertyChanged("SelectedPlayerLed");
				}
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public int SelectedPlayerLed
		{
			get
			{
				if (this._currentPlayerLedPair.Value == null)
				{
					return 1;
				}
				return this._currentPlayerLedPair.Value.LedNumber;
			}
			set
			{
				if (this.SetProperty<int>(ref this._currentPlayerLedPair.Value.LedNumber, value, "SelectedPlayerLed"))
				{
					base.FireOptionChanged();
					this.OnPropertyChanged("SelectedPlayerLed");
				}
			}
		}

		public bool IsDictionaryChanged
		{
			get
			{
				return this._isDictionaryChanged;
			}
			set
			{
				this.SetProperty<bool>(ref this._isDictionaryChanged, value, "IsDictionaryChanged");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public LEDSupportedDevice SelectedLedDevice
		{
			get
			{
				return this._selectedLedDevice;
			}
			set
			{
				if (this.SetProperty<LEDSupportedDevice>(ref this._selectedLedDevice, value, "SelectedLedDevice"))
				{
					this.SupportedGamepadsUserLedCollection = this.GamepadsUserLedCollection.Where((KeyValuePair<string, PlayerLedSettings> item) => item.Value.SupportedDeviceType == this.SelectedLedDevice);
					this.OnPropertyChanged("ExpandHomeLeds");
					this.OnPropertyChanged("ExpandPlayerLeds");
					this.OnPropertyChanged("ExpandRemapLeds");
					this.OnPropertyChanged("PlayerLedCollection");
				}
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public ObservableCollection<PlayerLEDMode> PlayerLedCollection
		{
			get
			{
				if (this.SelectedLedDevice == 10)
				{
					if (!this._playerLedCollection.Contains(2))
					{
						this._playerLedCollection.Insert(2, 2);
					}
				}
				else
				{
					this._playerLedCollection.Remove(2);
				}
				return this._playerLedCollection;
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

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool ExpandPlayerLeds
		{
			get
			{
				return this._expandPlayerLeds[this.SelectedLedDevice];
			}
			set
			{
				this.UpdateExpanders(this._expandPlayerLeds, value);
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool ExpandHomeLeds
		{
			get
			{
				return this._expandHomeLeds[this.SelectedLedDevice];
			}
			set
			{
				this.UpdateExpanders(this._expandHomeLeds, value);
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool ExpandRemapLeds
		{
			get
			{
				return this._expandRemapLeds[this.SelectedLedDevice];
			}
			set
			{
				this.UpdateExpanders(this._expandRemapLeds, value);
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool ExpandMicrophoneRemapLeds
		{
			get
			{
				return this._expandMicrophoneRemapLeds[this.SelectedLedDevice];
			}
			set
			{
				this.UpdateExpanders(this._expandMicrophoneRemapLeds, value);
			}
		}

		private void UpdateExpanders(Dictionary<LEDSupportedDevice, bool> dic, bool value)
		{
			if (!object.Equals(dic[this.SelectedLedDevice], value))
			{
				dic[this.SelectedLedDevice] = value;
				if (dic != this._expandPlayerLeds)
				{
					this._expandPlayerLeds[this.SelectedLedDevice] = false;
				}
				if (dic != this._expandHomeLeds)
				{
					this._expandHomeLeds[this.SelectedLedDevice] = false;
				}
				if (dic != this._expandMicrophoneRemapLeds)
				{
					this._expandMicrophoneRemapLeds[this.SelectedLedDevice] = false;
				}
				if (dic != this._expandRemapLeds)
				{
					this._expandRemapLeds[this.SelectedLedDevice] = false;
				}
				this.OnPropertyChanged("ExpandPlayerLeds");
				this.OnPropertyChanged("ExpandHomeLeds");
				this.OnPropertyChanged("ExpandMicrophoneRemapLeds");
				this.OnPropertyChanged("ExpandRemapLeds");
			}
		}

		public DelegateCommand RemoveDeviceEntryCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._RemoveDeviceEntry) == null)
				{
					delegateCommand = (this._RemoveDeviceEntry = new DelegateCommand(new Action(this.RemoveDeviceEntry), new Func<bool>(this.RemoveDeviceEntryCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void RemoveDeviceEntry()
		{
			if (DTMessageBox.Show(DTLocalization.GetString(12434), MessageBoxButton.YesNo, MessageBoxImage.Question, null, false, MessageBoxResult.None) == MessageBoxResult.Yes)
			{
				App.GamepadService.GamepadsUserLedCollection.Remove(this.CurrentPlayerLedPair.Key);
				App.GamepadService.BinDataSerialize.SaveGamepadsUserLedCollection();
				this.SetCurrentPair();
			}
		}

		private bool RemoveDeviceEntryCanExecute()
		{
			if (this.CurrentPlayerLedPair.Key == null || this.CurrentPlayerLedPair.Value == null)
			{
				return false;
			}
			foreach (BaseControllerVM baseControllerVM in App.GamepadService.GamepadCollection)
			{
				if (baseControllerVM.ID == this.CurrentPlayerLedPair.Key)
				{
					return false;
				}
				if (baseControllerVM.IsCompositeDevice)
				{
					CompositeControllerVM compositeControllerVM = baseControllerVM as CompositeControllerVM;
					BaseControllerVM baseControllerVM2 = compositeControllerVM.BaseControllers[0];
					if (!(((baseControllerVM2 != null) ? baseControllerVM2.ID : null) == this.CurrentPlayerLedPair.Key))
					{
						BaseControllerVM baseControllerVM3 = compositeControllerVM.BaseControllers[1];
						if (!(((baseControllerVM3 != null) ? baseControllerVM3.ID : null) == this.CurrentPlayerLedPair.Key))
						{
							BaseControllerVM baseControllerVM4 = compositeControllerVM.BaseControllers[2];
							if (!(((baseControllerVM4 != null) ? baseControllerVM4.ID : null) == this.CurrentPlayerLedPair.Key))
							{
								BaseControllerVM baseControllerVM5 = compositeControllerVM.BaseControllers[3];
								if (!(((baseControllerVM5 != null) ? baseControllerVM5.ID : null) == this.CurrentPlayerLedPair.Key))
								{
									continue;
								}
							}
						}
					}
					return false;
				}
			}
			return true;
		}

		private Dictionary<LEDSupportedDevice, bool> _expandPlayerLeds = new Dictionary<LEDSupportedDevice, bool>();

		private Dictionary<LEDSupportedDevice, bool> _expandHomeLeds = new Dictionary<LEDSupportedDevice, bool>();

		private Dictionary<LEDSupportedDevice, bool> _expandRemapLeds = new Dictionary<LEDSupportedDevice, bool>();

		private Dictionary<LEDSupportedDevice, bool> _expandMicrophoneRemapLeds = new Dictionary<LEDSupportedDevice, bool>();

		private ObservableCollection<int> _playerLEDNumbers = new ObservableCollection<int>(new int[] { 0, 1, 2, 3, 4 });

		private GamepadsPlayerLedDictionary _gamepadsUserLedCollection = new GamepadsPlayerLedDictionary();

		private IEnumerable<KeyValuePair<string, PlayerLedSettings>> _supportedGamepadsUserLedCollection;

		private KeyValuePair<string, PlayerLedSettings> _currentPlayerLedPair;

		private bool _isDictionaryChanged;

		private LEDSupportedDevice _selectedLedDevice = 2;

		private ObservableCollection<PlayerLEDMode> _playerLedCollection = new ObservableCollection<PlayerLEDMode>();

		private bool _isLedSettingsEnabled;

		private DelegateCommand _RemoveDeviceEntry;
	}
}
