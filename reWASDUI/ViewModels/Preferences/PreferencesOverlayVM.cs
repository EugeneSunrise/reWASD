using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Attributes;
using DiscSoft.NET.Common.View.Controls.MultiRangeSlider;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using Prism.Events;
using reWASDCommon.Infrastructure;
using reWASDUI.DataModels.GamepadSlotHotkeyCollection;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Services;
using reWASDUI.Utils.XBUtil;
using reWASDUI.ViewModels.Preferences.Base;
using UtilsDisplayName;
using XBEliteWPF.Infrastructure;

namespace reWASDUI.ViewModels.Preferences
{
	public class PreferencesOverlayVM : PreferencesBaseVM
	{
		public PreferencesOverlayNotificationsVM NotificationsVM
		{
			get
			{
				return this._notificationsVM;
			}
		}

		public PreferencesOverlayMappingsVM MappingsVM
		{
			get
			{
				return this._mappingsVM;
			}
		}

		public PreferencesOverlayGamepadVM GamepadVM
		{
			get
			{
				return this._gamepadVM;
			}
		}

		public PreferencesOverlayDirectXVM DirectXVM
		{
			get
			{
				return this._directXVM;
			}
		}

		public PreferencesOverlayOverlaySettingsVM OverlayVM
		{
			get
			{
				return this._overlayVM;
			}
		}

		public bool IsRadialMenuEnabled
		{
			get
			{
				return Constants.CreateOverlayShift;
			}
		}

		public bool IsX86
		{
			get
			{
				return RuntimeInformation.ProcessArchitecture != Architecture.Arm64;
			}
		}

		public bool ShowOverlay
		{
			get
			{
				return this._showOverlay;
			}
			set
			{
				if (value == this._showOverlay)
				{
					return;
				}
				this._showOverlay = value;
				this.OnPropertyChanged("ShowOverlay");
				this.NotificationsVM.ShowOverlayChanged();
				this.MappingsVM.ShowOverlayChanged();
				this.GamepadVM.ShowOverlayChanged();
				this.DirectXVM.ShowOverlayChanged();
				this.OverlayVM.ShowOverlayChanged();
				MultiRangeSlider.FireCloseAllPopups();
				this.IsDictionaryChanged = true;
			}
		}

		public KeyBindingService KeyBindingService { get; set; }

		public GamepadService GamepadService { get; set; }

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public IEnumerable<KeyValuePair<string, HotkeyCollection>> FilteredGamepadsSlotHotkeyCollections
		{
			get
			{
				return this._filteredGamepadsSlotHotkeyCollections;
			}
			set
			{
				this.SetProperty<IEnumerable<KeyValuePair<string, HotkeyCollection>>>(ref this._filteredGamepadsSlotHotkeyCollections, value, "FilteredGamepadsSlotHotkeyCollections");
			}
		}

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
						gamepadsHotkeyCollections.UnsubscribeChangeOverlayEvents();
					}
					if (value != null)
					{
						this.GamepadsHotkeyCollections.CollectionItemPropertyChanged += this.GamepadsHotkeyCollectionsOnCollectionItemPropertyChanged;
						this.GamepadsHotkeyCollections.CollectionItemPropertyChangedExtended += new PropertyChangedExtendedEventHandler(this.GamepadsHotkeyCollectionsOnCollectionItemPropertyChanged);
						this.GamepadsHotkeyCollections.ControllerButtonChanged += new PropertyChangedExtendedEventHandler(this.GamepadsHotkeyCollectionsOnCollectionItemPropertyChanged);
						this.GamepadsHotkeyCollections.SubscribeChangeOverlayEvents();
					}
					this.FilteredGamepadsSlotHotkeyCollections = this.GamepadsHotkeyCollections.Where((KeyValuePair<string, HotkeyCollection> item) => item.Value.ControllerFamily != 3);
					this.GamepadVM.CurrentKeyValuePairGamepad = this.SetCurrentPair(this.GamepadVM.CurrentKeyValuePairGamepad);
					this.MappingsVM.CurrentKeyValuePairMapping = this.SetCurrentPair(this.MappingsVM.CurrentKeyValuePairMapping);
					this.MappingsVM.CurrentKeyValuePairMappingDescriptions = this.SetCurrentPair(this.MappingsVM.CurrentKeyValuePairMappingDescriptions);
				}
			}
		}

		private KeyValuePair<string, HotkeyCollection> SetCurrentPair(KeyValuePair<string, HotkeyCollection> currPair)
		{
			if (!string.IsNullOrEmpty(currPair.Key) && this.FilteredGamepadsSlotHotkeyCollections.Any((KeyValuePair<string, HotkeyCollection> item) => item.Key == currPair.Key))
			{
				currPair = this.FilteredGamepadsSlotHotkeyCollections.FirstOrDefault((KeyValuePair<string, HotkeyCollection> kvp) => kvp.Key == currPair.Key);
			}
			else
			{
				currPair = this.FilteredGamepadsSlotHotkeyCollections.FirstOrDefault(delegate(KeyValuePair<string, HotkeyCollection> gs)
				{
					string key = gs.Key;
					BaseControllerVM currentGamepad = this.GamepadService.CurrentGamepad;
					return key == ((currentGamepad != null) ? currentGamepad.ID : null);
				});
				if (currPair.Value == null)
				{
					currPair = this.FilteredGamepadsSlotHotkeyCollections.FirstOrDefault<KeyValuePair<string, HotkeyCollection>>();
				}
			}
			return currPair;
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

		public ObservableCollection<GamepadColor> GamepadColors
		{
			get
			{
				return new ObservableCollection<GamepadColor>(Enum.GetValues(typeof(GamepadColor)).OfType<GamepadColor>());
			}
		}

		public PreferencesOverlayVM()
		{
			this.TabSelectionChanged = 0;
			this.KeyBindingService = (KeyBindingService)App.KeyBindingService;
			this.GamepadService = (GamepadService)App.GamepadService;
			this.GamepadService.GamepadCollection.CollectionChanged += this.GamepadCollectionOnCollectionChanged;
			this.GamepadService.GamepadsHotkeyCollection.CollectionChanged += this.GamepadCollectionOnCollectionChanged;
			IEventAggregator eventAggregator = App.EventAggregator;
			if (eventAggregator != null)
			{
				eventAggregator.GetEvent<CurrentGamepadChanged>().Subscribe(delegate(BaseControllerVM gamepad)
				{
					this.CurrentGamepadOnChanged();
				});
			}
			this._notificationsVM = new PreferencesOverlayNotificationsVM(this);
			this._mappingsVM = new PreferencesOverlayMappingsVM(this);
			this._gamepadVM = new PreferencesOverlayGamepadVM(this);
			this._directXVM = new PreferencesOverlayDirectXVM(this);
			this._overlayVM = new PreferencesOverlayOverlaySettingsVM(this);
			this._notificationsVM.OptionChanged += this.ChildOptionChanged;
			this._mappingsVM.OptionChanged += this.ChildOptionChanged;
			this._gamepadVM.OptionChanged += this.ChildOptionChanged;
			this._directXVM.OptionChanged += this.ChildOptionChanged;
			this._overlayVM.OptionChanged += this.ChildOptionChanged;
		}

		private void ChildOptionChanged()
		{
			base.FireOptionChanged();
		}

		public override void Refresh()
		{
			this._directXVM.Refresh();
		}

		public override async Task Initialize()
		{
			this.Monitors = new ObservableCollection<string>();
			Tracer.TraceWrite("PreferencesOverlayVM System.Windows.Forms.Screen.AllScreens.Count(): " + Screen.AllScreens.Count<Screen>().ToString(), false);
			Screen screen = Screen.FromHandle(new WindowInteropHelper(System.Windows.Application.Current.MainWindow).Handle);
			if (screen != null)
			{
				this.OverlayVM.SelectedMonitor = ScreenInterrogatory.DeviceFriendlyName(screen);
				this.OverlayVM.SelectedScreen = screen;
			}
			foreach (Screen screen2 in Screen.AllScreens)
			{
				string text = ScreenInterrogatory.DeviceFriendlyName(screen2);
				Tracer.TraceWrite("PreferencesOverlayVM DeviceFriendlyName: " + text, false);
				this.Monitors.Add(text);
				if (screen2.Primary)
				{
					if (screen == null)
					{
						this.OverlayVM.SelectedMonitor = text;
						this.OverlayVM.SelectedScreen = screen2;
					}
					this.NotificationsVM.SelectedMonitor = text;
					this.MappingsVM.SelectedMonitor = text;
					this.GamepadVM.SelectedMonitor = text;
				}
			}
			this.OnPropertyChanged("IsManyMonitors");
			this.OnPropertyChanged("Monitors");
			this.ShowOverlay = Convert.ToBoolean(RegistryHelper.GetValue("Overlay", "ShowOverlay", 1, false));
			await this.NotificationsVM.Initialize();
			await this.MappingsVM.Initialize();
			await this.GamepadVM.Initialize();
			await this.DirectXVM.Initialize();
			await this.OverlayVM.Initialize();
			this.GamepadsHotkeyCollections = App.GamepadService.GamepadsHotkeyCollection.CloneOverlays();
			this.IsDictionaryChanged = false;
			base.ChangedProperties.Clear();
			if (this.TabSelectionChanged == 3)
			{
				base.Description = new Localizable(12332);
			}
			else
			{
				base.Description = new Localizable();
			}
		}

		public string GetCheckedMonitorName(string primapyMonitor, string savedMonitor)
		{
			string text;
			if (savedMonitor.Equals(""))
			{
				text = primapyMonitor;
			}
			else
			{
				bool flag = false;
				foreach (string text2 in this.Monitors)
				{
					if (savedMonitor.Equals(text2))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					text = savedMonitor;
				}
				else
				{
					text = primapyMonitor;
				}
			}
			return text;
		}

		private void GamepadCollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.GamepadsHotkeyCollections = App.GamepadService.GamepadsHotkeyCollection.CloneOverlays();
			this.GamepadVM.RemoveVirtualEntryCommand.RaiseCanExecuteChanged();
			this.MappingsVM.RemoveMappingEntryCommand.RaiseCanExecuteChanged();
			this.MappingsVM.RemoveMappingEntryCommandDescriptions.RaiseCanExecuteChanged();
		}

		private void CurrentGamepadOnChanged()
		{
			try
			{
				KeyValuePair<string, HotkeyCollection> keyValuePair = this.FilteredGamepadsSlotHotkeyCollections.FirstOrDefault(delegate(KeyValuePair<string, HotkeyCollection> gs)
				{
					string key = gs.Key;
					BaseControllerVM currentGamepad = this.GamepadService.CurrentGamepad;
					return key == ((currentGamepad != null) ? currentGamepad.ID : null);
				});
				if (keyValuePair.Value == null && this.FilteredGamepadsSlotHotkeyCollections.Count<KeyValuePair<string, HotkeyCollection>>() > 0)
				{
					keyValuePair = this.FilteredGamepadsSlotHotkeyCollections.First<KeyValuePair<string, HotkeyCollection>>();
				}
				this.GamepadVM.CurrentKeyValuePairGamepad = keyValuePair;
				this.MappingsVM.CurrentKeyValuePairMapping = keyValuePair;
				this.MappingsVM.CurrentKeyValuePairMappingDescriptions = keyValuePair;
			}
			catch (Exception)
			{
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
			this.NotificationsVM.ApplyChanges();
			this.MappingsVM.ApplyChanges();
			this.GamepadVM.ApplyChanges();
			this.DirectXVM.ApplyChanges();
			this.OverlayVM.ApplyChanges();
			App.UserSettingsService.IsOverlayEnable = this.ShowOverlay;
			App.UserSettingsService.IsOverlayShowGamepadEnable = Convert.ToBoolean(RegistryHelper.GetValue("Overlay", "ShowGamepad", 1, false));
			if (!base.IsChanged)
			{
				return Task.FromResult<bool>(true);
			}
			if (!this.CheckHotkeys())
			{
				return Task.FromResult<bool>(false);
			}
			this.SaveHotkeys(true);
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

		private bool IsOnlyNoneKeys(ObservableCollection<AssociatedControllerButton> buttons)
		{
			bool flag = true;
			foreach (AssociatedControllerButton associatedControllerButton in buttons)
			{
				if (associatedControllerButton.GamepadButton != 2001 || associatedControllerButton.IsKeyScanCode)
				{
					flag = false;
					break;
				}
			}
			return flag;
		}

		private bool CheckHotkeys()
		{
			string text = string.Empty;
			string text2 = string.Empty;
			foreach (KeyValuePair<string, HotkeyCollection> keyValuePair in ((IEnumerable<KeyValuePair<string, HotkeyCollection>>)this.GamepadsHotkeyCollections))
			{
				List<ObservableCollection<AssociatedControllerButton>> list = new List<ObservableCollection<AssociatedControllerButton>>();
				if (keyValuePair.Value.IsGamepadOverlayEnabled && !this.IsOnlyNoneKeys(keyValuePair.Value.GamepadOverlayAssociatedButtonCollection))
				{
					list.Add(keyValuePair.Value.GamepadOverlayAssociatedButtonCollection);
				}
				if (keyValuePair.Value.IsMappingOverlayEnabled && !this.IsOnlyNoneKeys(keyValuePair.Value.MappingOverlayAssociatedButtonCollection))
				{
					list.Add(keyValuePair.Value.MappingOverlayAssociatedButtonCollection);
				}
				if (keyValuePair.Value.IsMappingOverlayEnabled && !this.IsOnlyNoneKeys(keyValuePair.Value.MappingDescriptionsOverlayAssociatedButtonCollection))
				{
					list.Add(keyValuePair.Value.MappingDescriptionsOverlayAssociatedButtonCollection);
				}
				if (!XBUtils.IsSlotHotkeyCollectionsValid(list, false))
				{
					if (text.Length > 0)
					{
						text += ", ";
					}
					text += keyValuePair.Value.DisplayName;
				}
				if (XBUtils.IsHotkeysSame(keyValuePair.Value.MappingOverlayAssociatedButtonCollection, keyValuePair.Value.MappingDescriptionsOverlayAssociatedButtonCollection))
				{
					if (text2.Length > 0)
					{
						text2 += ", ";
					}
					text2 += keyValuePair.Value.DisplayName;
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				DTMessageBox.Show(DTLocalization.GetString(12158).Replace("{0}", text));
				return false;
			}
			if (!string.IsNullOrEmpty(text2))
			{
				DTMessageBox.Show(DTLocalization.GetString(12199).Replace("{0}", text2));
				return false;
			}
			return true;
		}

		public async void SaveHotkeys(bool isPushClonedCollectionToService = true)
		{
			if (isPushClonedCollectionToService)
			{
				App.GamepadService.GamepadsHotkeyCollection.MergeOverlay(this.GamepadsHotkeyCollections);
			}
			await App.GamepadService.BinDataSerialize.SaveGamepadsHotkeyCollection();
			this.GamepadsHotkeyCollections = App.GamepadService.GamepadsHotkeyCollection.CloneOverlays();
		}

		public bool GamepadIsInCollectionOrInCompositeController(KeyValuePair<string, HotkeyCollection> gamepadPair)
		{
			if (gamepadPair.Key == null || gamepadPair.Value == null)
			{
				return false;
			}
			foreach (BaseControllerVM baseControllerVM in this.GamepadService.GamepadCollection)
			{
				if (baseControllerVM.ID == gamepadPair.Key)
				{
					return false;
				}
				if (baseControllerVM.IsCompositeDevice)
				{
					CompositeControllerVM compositeControllerVM = baseControllerVM as CompositeControllerVM;
					BaseControllerVM baseControllerVM2 = compositeControllerVM.BaseControllers[0];
					if (!(((baseControllerVM2 != null) ? baseControllerVM2.ID : null) == gamepadPair.Key))
					{
						BaseControllerVM baseControllerVM3 = compositeControllerVM.BaseControllers[1];
						if (!(((baseControllerVM3 != null) ? baseControllerVM3.ID : null) == gamepadPair.Key))
						{
							BaseControllerVM baseControllerVM4 = compositeControllerVM.BaseControllers[2];
							if (!(((baseControllerVM4 != null) ? baseControllerVM4.ID : null) == gamepadPair.Key))
							{
								BaseControllerVM baseControllerVM5 = compositeControllerVM.BaseControllers[3];
								if (!(((baseControllerVM5 != null) ? baseControllerVM5.ID : null) == gamepadPair.Key))
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

		public ObservableCollection<string> Monitors { get; private set; }

		public bool IsManyMonitors
		{
			get
			{
				ObservableCollection<string> monitors = this.Monitors;
				return monitors != null && monitors.Count > 1;
			}
		}

		public int TabSelectionChanged
		{
			get
			{
				return this.tabSelectionChanged;
			}
			set
			{
				this.tabSelectionChanged = value;
				if (this.tabSelectionChanged == 3)
				{
					base.Description = new Localizable(12332);
					return;
				}
				base.Description = new Localizable();
			}
		}

		private PreferencesOverlayNotificationsVM _notificationsVM;

		private PreferencesOverlayMappingsVM _mappingsVM;

		private PreferencesOverlayGamepadVM _gamepadVM;

		private PreferencesOverlayDirectXVM _directXVM;

		private PreferencesOverlayOverlaySettingsVM _overlayVM;

		private bool _showOverlay;

		private GamepadsHotkeyDictionary _gamepadsHotkeyCollections;

		private IEnumerable<KeyValuePair<string, HotkeyCollection>> _filteredGamepadsSlotHotkeyCollections;

		private bool _isDictionaryChanged;

		private int tabSelectionChanged;
	}
}
