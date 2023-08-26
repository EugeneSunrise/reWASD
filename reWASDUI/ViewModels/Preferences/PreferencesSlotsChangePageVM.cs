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
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using Prism.Commands;
using Prism.Events;
using reWASDUI.DataModels.GamepadSlotHotkeyCollection;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Services;
using reWASDUI.Utils.XBUtil;
using reWASDUI.ViewModels.Preferences.Base;

namespace reWASDUI.ViewModels.Preferences
{
	public class PreferencesSlotsChangePageVM : PreferencesBaseVM
	{
		public KeyBindingService KeyBindingService { get; set; }

		public GamepadService GamepadService { get; set; }

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public GamepadsHotkeyDictionary GamepadsHotkeyCollections
		{
			get
			{
				return this._gamepadsHotkeyCollections;
			}
			set
			{
				GamepadsHotkeyDictionary gamepadsHotkeyCollections = this._gamepadsHotkeyCollections;
				if (this.SetProperty<GamepadsHotkeyDictionary>(ref this._gamepadsHotkeyCollections, value, "GamepadsHotkeyCollections"))
				{
					if (gamepadsHotkeyCollections != null)
					{
						gamepadsHotkeyCollections.CollectionItemPropertyChanged -= this.GamepadsHotkeyCollectionsOnCollectionItemPropertyChanged;
						gamepadsHotkeyCollections.CollectionItemPropertyChangedExtended -= new PropertyChangedExtendedEventHandler(this.GamepadsHotkeyCollectionsOnCollectionItemPropertyChanged);
						gamepadsHotkeyCollections.ControllerButtonChanged -= new PropertyChangedExtendedEventHandler(this.GamepadsHotkeyCollectionsOnCollectionItemPropertyChanged);
						gamepadsHotkeyCollections.UnsubscribeChangeSlotsEvents();
					}
					if (value != null)
					{
						this.GamepadsHotkeyCollections.CollectionItemPropertyChanged += this.GamepadsHotkeyCollectionsOnCollectionItemPropertyChanged;
						this.GamepadsHotkeyCollections.CollectionItemPropertyChangedExtended += new PropertyChangedExtendedEventHandler(this.GamepadsHotkeyCollectionsOnCollectionItemPropertyChanged);
						this.GamepadsHotkeyCollections.ControllerButtonChanged += new PropertyChangedExtendedEventHandler(this.GamepadsHotkeyCollectionsOnCollectionItemPropertyChanged);
						this.GamepadsHotkeyCollections.SubscribeChangeSlotsEvents();
					}
					this.SetCurrentGamepad();
				}
			}
		}

		private void SetCurrentGamepad()
		{
			if (!string.IsNullOrEmpty(this.CurrentKeyValuePair.Key) && this.GamepadsHotkeyCollections.ContainsKey(this.CurrentKeyValuePair.Key))
			{
				this.CurrentKeyValuePair = this.GamepadsHotkeyCollections.FirstOrDefault((KeyValuePair<string, HotkeyCollection> kvp) => kvp.Key == this.CurrentKeyValuePair.Key);
				return;
			}
			KeyValuePair<string, HotkeyCollection> keyValuePair = this.GamepadsHotkeyCollections.FirstOrDefault(delegate(KeyValuePair<string, HotkeyCollection> gs)
			{
				string key = gs.Key;
				BaseControllerVM currentGamepad = this.GamepadService.CurrentGamepad;
				return key == ((currentGamepad != null) ? currentGamepad.ID : null);
			});
			if (keyValuePair.Value == null && this.GamepadsHotkeyCollections.HasItems)
			{
				keyValuePair = this.GamepadsHotkeyCollections.First<KeyValuePair<string, HotkeyCollection>>();
			}
			this.CurrentKeyValuePair = keyValuePair;
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public KeyValuePair<string, HotkeyCollection> CurrentKeyValuePair
		{
			get
			{
				return this._currentKeyValuePair;
			}
			set
			{
				if (object.Equals(this._currentKeyValuePair, value))
				{
					return;
				}
				this._currentKeyValuePair = value;
				HotkeyCollection value2 = this._currentKeyValuePair.Value;
				if (value2 != null)
				{
					value2.RefreshButtonsForSlotHotkey(true);
				}
				this.RemoveCurrentEntryCommand.RaiseCanExecuteChanged();
				this.OnPropertyChanged("CurrentKeyValuePair");
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
			if (DTMessageBox.Show(DTLocalization.GetString(11219), MessageBoxButton.YesNo, MessageBoxImage.Question, null, false, MessageBoxResult.None) == MessageBoxResult.Yes)
			{
				App.GamepadService.GamepadsHotkeyCollection.Remove(this.CurrentKeyValuePair.Key);
				this.SaveSlotCollection(false);
			}
		}

		private bool RemoveCurrentEntryCanExecute()
		{
			return this.CurrentKeyValuePair.Key != null && this.CurrentKeyValuePair.Value != null && this.GamepadService.GamepadCollection.All((BaseControllerVM g) => g.ID != this.CurrentKeyValuePair.Key);
		}

		public PreferencesSlotsChangePageVM()
		{
			this.KeyBindingService = (KeyBindingService)App.KeyBindingService;
			this.GamepadService = (GamepadService)App.GamepadService;
			this.GamepadService.GamepadCollection.CollectionChanged += this.GamepadCollectionOnCollectionChanged;
			this.GamepadService.GamepadsHotkeyCollection.CollectionChanged += this.GamepadCollectionOnCollectionChanged;
			IEventAggregator eventAggregator = App.EventAggregator;
			if (eventAggregator == null)
			{
				return;
			}
			eventAggregator.GetEvent<CurrentGamepadChanged>().Subscribe(delegate(BaseControllerVM gamepad)
			{
				this.CurrentGamepadOnChanged();
			});
		}

		public RelayCommand RestoreToDefault
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._restoreToDefault) == null)
				{
					relayCommand = (this._restoreToDefault = new RelayCommand(new Action(this.FillSlotsCollectionsWithDefault), new Func<bool>(this.RestoreToDefaultCanExecute)));
				}
				return relayCommand;
			}
		}

		private bool RestoreToDefaultCanExecute()
		{
			return !this.CurrentKeyValuePair.Value.IsDefault();
		}

		private void FillSlotsCollectionsWithDefault()
		{
			HotkeyCollection value = this.CurrentKeyValuePair.Value;
			if (value == null)
			{
				return;
			}
			value.RestoreDefaults();
		}

		public override Task Initialize()
		{
			this.GamepadsHotkeyCollections = App.GamepadService.GamepadsHotkeyCollection.CloneSlots();
			this.CurrentKeyValuePair = default(KeyValuePair<string, HotkeyCollection>);
			this.SetCurrentGamepad();
			this.IsDictionaryChanged = false;
			base.ChangedProperties.Clear();
			this.SetDescription();
			return Task.CompletedTask;
		}

		private void GamepadCollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.GamepadsHotkeyCollections = App.GamepadService.GamepadsHotkeyCollection.CloneSlots();
			this.RemoveCurrentEntryCommand.RaiseCanExecuteChanged();
		}

		private void CurrentGamepadOnChanged()
		{
			if (this.GamepadsHotkeyCollections != null)
			{
				KeyValuePair<string, HotkeyCollection> keyValuePair = this.GamepadsHotkeyCollections.FirstOrDefault(delegate(KeyValuePair<string, HotkeyCollection> gs)
				{
					string key = gs.Key;
					BaseControllerVM currentGamepad = this.GamepadService.CurrentGamepad;
					return key == ((currentGamepad != null) ? currentGamepad.ID : null);
				});
				if (keyValuePair.Value == null && this.GamepadsHotkeyCollections.HasItems)
				{
					keyValuePair = this.GamepadsHotkeyCollections.First<KeyValuePair<string, HotkeyCollection>>();
				}
				this.CurrentKeyValuePair = keyValuePair;
			}
		}

		private void GamepadsHotkeyCollectionsOnCollectionItemPropertyChanged(object s, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "ButtonsForSlotHotkey")
			{
				return;
			}
			this.IsDictionaryChanged = true;
		}

		public override Task<bool> ApplyChanges()
		{
			if (!base.IsChanged)
			{
				return Task.FromResult<bool>(true);
			}
			if (!this.CheckHotkeys())
			{
				return Task.FromResult<bool>(false);
			}
			this.SaveSlotCollection(true);
			if (base.IsChanged)
			{
				if (this.GamepadsHotkeyCollections.Any((KeyValuePair<string, HotkeyCollection> kvp) => kvp.Value.IsAnySlotEnabled))
				{
					base.FireRequiredEnableRemap();
				}
			}
			this.IsDictionaryChanged = false;
			base.ChangedProperties.Clear();
			return Task.FromResult<bool>(true);
		}

		private bool CheckHotkeys()
		{
			string text = string.Empty;
			foreach (KeyValuePair<string, HotkeyCollection> keyValuePair in ((IEnumerable<KeyValuePair<string, HotkeyCollection>>)this.GamepadsHotkeyCollections))
			{
				List<ObservableCollection<AssociatedControllerButton>> list = new List<ObservableCollection<AssociatedControllerButton>>();
				if (keyValuePair.Value.IsSlot1Enabled)
				{
					list.Add(keyValuePair.Value.Slot1AssociatedButtonCollection);
				}
				if (keyValuePair.Value.IsSlot2Enabled)
				{
					list.Add(keyValuePair.Value.Slot2AssociatedButtonCollection);
				}
				if (keyValuePair.Value.IsSlot3Enabled)
				{
					list.Add(keyValuePair.Value.Slot3AssociatedButtonCollection);
				}
				if (keyValuePair.Value.IsSlot4Enabled)
				{
					list.Add(keyValuePair.Value.Slot4AssociatedButtonCollection);
				}
				if (!XBUtils.IsSlotHotkeyCollectionsValid(list, true))
				{
					if (text.Length > 0)
					{
						text += ", ";
					}
					text += keyValuePair.Value.DisplayName;
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				DTMessageBox.Show(DTLocalization.GetString(11218).Replace("{0}", text));
				return false;
			}
			return true;
		}

		private async void SaveSlotCollection(bool isPushClonedCollectionToService = true)
		{
			if (isPushClonedCollectionToService)
			{
				App.GamepadService.GamepadsHotkeyCollection.MergeSlots(this.GamepadsHotkeyCollections);
			}
			await App.GamepadService.BinDataSerialize.SaveGamepadsHotkeyCollection();
			this.GamepadsHotkeyCollections = App.GamepadService.GamepadsHotkeyCollection.CloneSlots();
		}

		private void SetDescription()
		{
			base.Description = ((!App.LicensingService.IsSlotFeatureUnlocked) ? new Localizable(12127) : new Localizable());
		}

		private GamepadsHotkeyDictionary _gamepadsHotkeyCollections;

		private KeyValuePair<string, HotkeyCollection> _currentKeyValuePair;

		private bool _isDictionaryChanged;

		private DelegateCommand _RemoveCurrentEntry;

		private RelayCommand _restoreToDefault;
	}
}
