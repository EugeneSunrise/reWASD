using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Input;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.KeyBindings;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtilModel;
using XBEliteWPF.ViewModels.Base;

namespace reWASDUI.DataModels.GamepadSlotHotkeyCollection
{
	[Serializable]
	public class HotkeyCollection : BaseSerializable
	{
		public bool IsNintendoSwitchJoyConLExpected
		{
			get
			{
				return this.ControllerTypes.Any((ControllerTypeEnum t) => t == 9);
			}
		}

		public bool IsNintendoSwitchJoyConRExpected
		{
			get
			{
				return this.ControllerTypes.Any((ControllerTypeEnum t) => t == 10);
			}
		}

		public bool IsNintendoSwitchJoyConComposite
		{
			get
			{
				if (this.IsNintendoSwitchJoyConLExpected && this.IsNintendoSwitchJoyConRExpected)
				{
					return this.ControllerTypes.Count((ControllerTypeEnum t) => t > 0) == 2;
				}
				return false;
			}
		}

		public ControllerTypeEnum FirstGamepadType
		{
			get
			{
				return this.ControllerTypes.FirstOrDefault((ControllerTypeEnum ct) => ControllerTypeExtensions.IsGamepad(ct));
			}
		}

		public List<ControllerTypeEnum> ControllerTypes { get; set; }

		public ControllerFamily ControllerFamily { get; set; }

		public string DisplayName { get; set; }

		public ObservableCollection<AssociatedControllerButton> Slot1AssociatedButtonCollection
		{
			get
			{
				return this._slot1AssociatedButtonCollection;
			}
			set
			{
				ObservableCollection<AssociatedControllerButton> slot1AssociatedButtonCollection = this._slot1AssociatedButtonCollection;
				if (this.SetProperty<ObservableCollection<AssociatedControllerButton>>(ref this._slot1AssociatedButtonCollection, value, "Slot1AssociatedButtonCollection"))
				{
					if (slot1AssociatedButtonCollection != null)
					{
						slot1AssociatedButtonCollection.CollectionChanged -= this.Slot1CollectionChanged;
					}
					if (value != null)
					{
						value.CollectionChanged += this.Slot1CollectionChanged;
					}
				}
			}
		}

		public ObservableCollection<AssociatedControllerButton> Slot2AssociatedButtonCollection
		{
			get
			{
				return this._slot2AssociatedButtonCollection;
			}
			set
			{
				ObservableCollection<AssociatedControllerButton> slot2AssociatedButtonCollection = this._slot2AssociatedButtonCollection;
				if (this.SetProperty<ObservableCollection<AssociatedControllerButton>>(ref this._slot2AssociatedButtonCollection, value, "Slot2AssociatedButtonCollection"))
				{
					if (slot2AssociatedButtonCollection != null)
					{
						slot2AssociatedButtonCollection.CollectionChanged -= this.Slot2CollectionChanged;
					}
					if (value != null)
					{
						value.CollectionChanged += this.Slot2CollectionChanged;
					}
				}
			}
		}

		public ObservableCollection<AssociatedControllerButton> Slot3AssociatedButtonCollection
		{
			get
			{
				return this._slot3AssociatedButtonCollection;
			}
			set
			{
				ObservableCollection<AssociatedControllerButton> slot3AssociatedButtonCollection = this._slot3AssociatedButtonCollection;
				if (this.SetProperty<ObservableCollection<AssociatedControllerButton>>(ref this._slot3AssociatedButtonCollection, value, "Slot3AssociatedButtonCollection"))
				{
					if (slot3AssociatedButtonCollection != null)
					{
						slot3AssociatedButtonCollection.CollectionChanged -= this.Slot3CollectionChanged;
					}
					if (value != null)
					{
						value.CollectionChanged += this.Slot3CollectionChanged;
					}
				}
			}
		}

		public ObservableCollection<AssociatedControllerButton> GamepadOverlayAssociatedButtonCollection
		{
			get
			{
				return this._gamepadOverlayAssociatedButtonCollection;
			}
			set
			{
				ObservableCollection<AssociatedControllerButton> gamepadOverlayAssociatedButtonCollection = this._gamepadOverlayAssociatedButtonCollection;
				if (this.SetProperty<ObservableCollection<AssociatedControllerButton>>(ref this._gamepadOverlayAssociatedButtonCollection, value, "GamepadOverlayAssociatedButtonCollection"))
				{
					if (gamepadOverlayAssociatedButtonCollection != null)
					{
						gamepadOverlayAssociatedButtonCollection.CollectionChanged -= this.GamepadOverlayAssociatedButtonCollectionChanged;
					}
					if (value != null)
					{
						value.CollectionChanged += this.GamepadOverlayAssociatedButtonCollectionChanged;
					}
				}
			}
		}

		public ObservableCollection<AssociatedControllerButton> MappingOverlayAssociatedButtonCollection
		{
			get
			{
				return this._mappingOverlayAssociatedButtonCollection;
			}
			set
			{
				ObservableCollection<AssociatedControllerButton> mappingOverlayAssociatedButtonCollection = this._mappingOverlayAssociatedButtonCollection;
				if (this.SetProperty<ObservableCollection<AssociatedControllerButton>>(ref this._mappingOverlayAssociatedButtonCollection, value, "MappingOverlayAssociatedButtonCollection"))
				{
					if (mappingOverlayAssociatedButtonCollection != null)
					{
						mappingOverlayAssociatedButtonCollection.CollectionChanged -= this.GamepadMappingyAssociatedButtonCollectionChanged;
					}
					if (value != null)
					{
						value.CollectionChanged += this.GamepadMappingyAssociatedButtonCollectionChanged;
					}
				}
			}
		}

		public ObservableCollection<AssociatedControllerButton> MappingDescriptionsOverlayAssociatedButtonCollection
		{
			get
			{
				return this._mappingDescriptionsOverlayAssociatedButtonCollection;
			}
			set
			{
				ObservableCollection<AssociatedControllerButton> mappingDescriptionsOverlayAssociatedButtonCollection = this._mappingDescriptionsOverlayAssociatedButtonCollection;
				if (this.SetProperty<ObservableCollection<AssociatedControllerButton>>(ref this._mappingDescriptionsOverlayAssociatedButtonCollection, value, "MappingDescriptionsOverlayAssociatedButtonCollection"))
				{
					if (mappingDescriptionsOverlayAssociatedButtonCollection != null)
					{
						mappingDescriptionsOverlayAssociatedButtonCollection.CollectionChanged -= this.GamepadMappingyDescriptionsAssociatedButtonCollectionChanged;
					}
					if (value != null)
					{
						value.CollectionChanged += this.GamepadMappingyDescriptionsAssociatedButtonCollectionChanged;
					}
				}
			}
		}

		public ObservableCollection<AssociatedControllerButton> Slot4AssociatedButtonCollection
		{
			get
			{
				return this._slot4AssociatedButtonCollection;
			}
			set
			{
				ObservableCollection<AssociatedControllerButton> slot4AssociatedButtonCollection = this._slot4AssociatedButtonCollection;
				if (this.SetProperty<ObservableCollection<AssociatedControllerButton>>(ref this._slot4AssociatedButtonCollection, value, "Slot4AssociatedButtonCollection"))
				{
					if (slot4AssociatedButtonCollection != null)
					{
						slot4AssociatedButtonCollection.CollectionChanged -= this.Slot4CollectionChanged;
					}
					if (value != null)
					{
						value.CollectionChanged += this.Slot4CollectionChanged;
					}
				}
			}
		}

		public bool IsAnySlotEnabled
		{
			get
			{
				return this.IsSlot1Enabled || this.IsSlot2Enabled || this.IsSlot3Enabled || this.IsSlot4Enabled || this.IsMappingOverlayEnabled || this.IsGamepadOverlayEnabled;
			}
		}

		public bool IsSlot1Enabled
		{
			get
			{
				return this._isSlot1Enabled;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isSlot1Enabled, value, "IsSlot1Enabled"))
				{
					this.OnPropertyChanged("IsAnySlotEnabled");
				}
			}
		}

		public bool IsSlot2Enabled
		{
			get
			{
				return this._isSlot2Enabled;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isSlot2Enabled, value, "IsSlot2Enabled"))
				{
					this.OnPropertyChanged("IsAnySlotEnabled");
				}
			}
		}

		public bool IsSlot3Enabled
		{
			get
			{
				return this._isSlot3Enabled;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isSlot3Enabled, value, "IsSlot3Enabled"))
				{
					this.OnPropertyChanged("IsAnySlotEnabled");
				}
			}
		}

		public bool IsGamepadOverlayEnabled
		{
			get
			{
				return this._isGamepadOverlayEnabled;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isGamepadOverlayEnabled, value, "IsGamepadOverlayEnabled"))
				{
					this.OnPropertyChanged("IsGamepadOverlayEnabled");
				}
			}
		}

		public bool IsMappingOverlayEnabled
		{
			get
			{
				return this._isMappingOverlayEnabled;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isMappingOverlayEnabled, value, "IsMappingOverlayEnabled"))
				{
					this.OnPropertyChanged("IsMappingOverlayEnabled");
				}
			}
		}

		public bool IsSlot4Enabled
		{
			get
			{
				return this._isSlot4Enabled;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isSlot4Enabled, value, "IsSlot4Enabled"))
				{
					this.OnPropertyChanged("IsAnySlotEnabled");
				}
			}
		}

		public ObservableCollection<object> ButtonsForSlotHotkey
		{
			get
			{
				if (this._buttonsForSlotHotkey == null)
				{
					this.RefreshButtonsForSlotHotkey(false);
				}
				return this._buttonsForSlotHotkey;
			}
		}

		public HotkeyCollection()
		{
			this.FillSlotsCollectionsWithDefault();
		}

		public HotkeyCollection(List<ControllerTypeEnum> controllerTypes, ControllerFamily controllerFamily, string displayName)
			: this()
		{
			this.ControllerTypes = controllerTypes;
			this.ControllerFamily = controllerFamily;
			this.DisplayName = displayName;
		}

		public HotkeyCollection(uint[] controllerTypes, ControllerFamily controllerFamily, string displayName)
			: this(ControllerTypeExtensions.ConvertPhysicalTypesToEnums(0, controllerTypes, null).ToList<ControllerTypeEnum>(), controllerFamily, displayName)
		{
		}

		public void RefreshButtonsForSlotHotkey(bool firePropertyChange = false)
		{
			this._buttonsForSlotHotkey = XBUtils.CreateSlotHotkeyCollection(this.ControllerTypes, false);
			if (firePropertyChange)
			{
				this.OnPropertyChanged("ButtonsForSlotHotkey");
			}
		}

		private void Slot1CollectionChanged(object s, NotifyCollectionChangedEventArgs e)
		{
			this.OnPropertyChanged("Slot1AssociatedButtonCollection");
		}

		private void Slot2CollectionChanged(object s, NotifyCollectionChangedEventArgs e)
		{
			this.OnPropertyChanged("Slot2AssociatedButtonCollection");
		}

		private void Slot3CollectionChanged(object s, NotifyCollectionChangedEventArgs e)
		{
			this.OnPropertyChanged("Slot3AssociatedButtonCollection");
		}

		private void Slot4CollectionChanged(object s, NotifyCollectionChangedEventArgs e)
		{
			this.OnPropertyChanged("Slot4AssociatedButtonCollection");
		}

		private void GamepadOverlayAssociatedButtonCollectionChanged(object s, NotifyCollectionChangedEventArgs e)
		{
			this.OnPropertyChanged("GamepadOverlayAssociatedButtonCollection");
		}

		private void GamepadMappingyAssociatedButtonCollectionChanged(object s, NotifyCollectionChangedEventArgs e)
		{
			this.OnPropertyChanged("MappingOverlayAssociatedButtonCollection");
		}

		private void GamepadMappingyDescriptionsAssociatedButtonCollectionChanged(object s, NotifyCollectionChangedEventArgs e)
		{
			this.OnPropertyChanged("MappingDescriptionsOverlayAssociatedButtonCollection");
		}

		public void RestoreDefaults()
		{
			HotkeyCollection hotkeyCollection = new HotkeyCollection();
			this.SetGamePadAssociations(hotkeyCollection, this.ControllerTypes);
			this.IsSlot1Enabled = hotkeyCollection.IsSlot1Enabled;
			this.IsSlot2Enabled = hotkeyCollection.IsSlot2Enabled;
			this.IsSlot3Enabled = hotkeyCollection.IsSlot3Enabled;
			this.IsSlot4Enabled = hotkeyCollection.IsSlot4Enabled;
			this.Slot1AssociatedButtonCollection = hotkeyCollection.Slot1AssociatedButtonCollection;
			this.Slot2AssociatedButtonCollection = hotkeyCollection.Slot2AssociatedButtonCollection;
			this.Slot3AssociatedButtonCollection = hotkeyCollection.Slot3AssociatedButtonCollection;
			this.Slot4AssociatedButtonCollection = hotkeyCollection.Slot4AssociatedButtonCollection;
		}

		public bool IsDefault()
		{
			bool flag = true;
			HotkeyCollection hotkeyCollection = new HotkeyCollection();
			this.SetGamePadAssociations(hotkeyCollection, this.ControllerTypes);
			return this.IsSlot1Enabled == hotkeyCollection.IsSlot1Enabled && this.IsSlot2Enabled == hotkeyCollection.IsSlot2Enabled && this.IsSlot3Enabled == hotkeyCollection.IsSlot3Enabled && this.IsSlot4Enabled == hotkeyCollection.IsSlot4Enabled && this.CollectionsIsEquals(this.Slot1AssociatedButtonCollection, hotkeyCollection.Slot1AssociatedButtonCollection) && this.CollectionsIsEquals(this.Slot2AssociatedButtonCollection, hotkeyCollection.Slot2AssociatedButtonCollection) && this.CollectionsIsEquals(this.Slot3AssociatedButtonCollection, hotkeyCollection.Slot3AssociatedButtonCollection) && this.CollectionsIsEquals(this.Slot4AssociatedButtonCollection, hotkeyCollection.Slot4AssociatedButtonCollection) && flag;
		}

		private bool CollectionsIsEquals(ObservableCollection<AssociatedControllerButton> slotCollection, ObservableCollection<AssociatedControllerButton> slotCollection2)
		{
			for (int i = 0; i < 3; i++)
			{
				if (!slotCollection[i].IsAssociatedSetToEqualButtons(slotCollection2[i]))
				{
					return false;
				}
			}
			return true;
		}

		private void FillSlotsCollectionsWithDefault()
		{
			this.FillCollectionsWithDefault(ref this._slot1AssociatedButtonCollection);
			this.FillCollectionsWithDefault(ref this._slot2AssociatedButtonCollection);
			this.FillCollectionsWithDefault(ref this._slot3AssociatedButtonCollection);
			this.FillCollectionsWithDefault(ref this._slot4AssociatedButtonCollection);
			this.FillCollectionsWithDefault(ref this._gamepadOverlayAssociatedButtonCollection);
			this.FillCollectionsWithDefault(ref this._mappingOverlayAssociatedButtonCollection);
			this.FillCollectionsWithDefault(ref this._mappingDescriptionsOverlayAssociatedButtonCollection);
		}

		private void FillCollectionsWithDefault(ref ObservableCollection<AssociatedControllerButton> slotCollection)
		{
			if (slotCollection == null)
			{
				slotCollection = new ObservableCollection<AssociatedControllerButton>();
			}
			while (slotCollection.Count < 3)
			{
				slotCollection.Add(new AssociatedControllerButton(2001));
			}
		}

		protected HotkeyCollection(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.FillSlotsCollectionsWithDefault();
		}

		private ObservableCollection<AssociatedControllerButton> FillFromOldCollection(ObservableCollection<GamepadButton> oldCollection)
		{
			ObservableCollection<AssociatedControllerButton> observableCollection = new ObservableCollection<AssociatedControllerButton>();
			foreach (GamepadButton gamepadButton in oldCollection)
			{
				observableCollection.Add(new AssociatedControllerButton(gamepadButton));
			}
			return observableCollection;
		}

		private ObservableCollection<AssociatedControllerButton> CloneCollection(ObservableCollection<AssociatedControllerButton> collection)
		{
			ObservableCollection<AssociatedControllerButton> observableCollection = new ObservableCollection<AssociatedControllerButton>();
			foreach (AssociatedControllerButton associatedControllerButton in collection)
			{
				observableCollection.Add(associatedControllerButton.Clone());
			}
			return observableCollection;
		}

		private HotkeyCollection Clone()
		{
			return new HotkeyCollection
			{
				ControllerTypes = this.ControllerTypes,
				ControllerFamily = this.ControllerFamily,
				DisplayName = this.DisplayName
			};
		}

		public HotkeyCollection CloneSlots()
		{
			HotkeyCollection hotkeyCollection = this.Clone();
			hotkeyCollection.Slot1AssociatedButtonCollection = this.CloneCollection(this.Slot1AssociatedButtonCollection);
			hotkeyCollection.Slot2AssociatedButtonCollection = this.CloneCollection(this.Slot2AssociatedButtonCollection);
			hotkeyCollection.Slot3AssociatedButtonCollection = this.CloneCollection(this.Slot3AssociatedButtonCollection);
			hotkeyCollection.Slot4AssociatedButtonCollection = this.CloneCollection(this.Slot4AssociatedButtonCollection);
			hotkeyCollection.IsSlot1Enabled = this.IsSlot1Enabled;
			hotkeyCollection.IsSlot2Enabled = this.IsSlot2Enabled;
			hotkeyCollection.IsSlot3Enabled = this.IsSlot3Enabled;
			hotkeyCollection.IsSlot4Enabled = this.IsSlot4Enabled;
			return hotkeyCollection;
		}

		public HotkeyCollection CloneOverlays()
		{
			HotkeyCollection hotkeyCollection = this.Clone();
			hotkeyCollection.GamepadOverlayAssociatedButtonCollection = this.CloneCollection(this.GamepadOverlayAssociatedButtonCollection);
			hotkeyCollection.MappingOverlayAssociatedButtonCollection = this.CloneCollection(this.MappingOverlayAssociatedButtonCollection);
			hotkeyCollection.MappingDescriptionsOverlayAssociatedButtonCollection = this.CloneCollection(this.MappingDescriptionsOverlayAssociatedButtonCollection);
			hotkeyCollection.IsGamepadOverlayEnabled = this.IsGamepadOverlayEnabled;
			hotkeyCollection.IsMappingOverlayEnabled = this.IsMappingOverlayEnabled;
			return hotkeyCollection;
		}

		private bool MergeCollection(ObservableCollection<AssociatedControllerButton> oldCollection, ObservableCollection<AssociatedControllerButton> newCollection)
		{
			if (oldCollection.Count != newCollection.Count)
			{
				return false;
			}
			for (int i = 0; i < oldCollection.Count; i++)
			{
				oldCollection[i].Merge(newCollection[i]);
			}
			return true;
		}

		public bool MergeSlots(HotkeyCollection collection)
		{
			if (collection == null)
			{
				return false;
			}
			this.MergeCollection(this.Slot1AssociatedButtonCollection, collection.Slot1AssociatedButtonCollection);
			this.MergeCollection(this.Slot2AssociatedButtonCollection, collection.Slot2AssociatedButtonCollection);
			this.MergeCollection(this.Slot3AssociatedButtonCollection, collection.Slot3AssociatedButtonCollection);
			this.MergeCollection(this.Slot4AssociatedButtonCollection, collection.Slot4AssociatedButtonCollection);
			this.IsSlot1Enabled = collection.IsSlot1Enabled;
			this.IsSlot2Enabled = collection.IsSlot2Enabled;
			this.IsSlot3Enabled = collection.IsSlot3Enabled;
			this.IsSlot4Enabled = collection.IsSlot4Enabled;
			return true;
		}

		public bool MergeOverlay(HotkeyCollection collection)
		{
			if (collection == null)
			{
				return false;
			}
			this.MergeCollection(this.GamepadOverlayAssociatedButtonCollection, collection.GamepadOverlayAssociatedButtonCollection);
			this.MergeCollection(this.MappingOverlayAssociatedButtonCollection, collection.MappingOverlayAssociatedButtonCollection);
			this.MergeCollection(this.MappingDescriptionsOverlayAssociatedButtonCollection, collection.MappingDescriptionsOverlayAssociatedButtonCollection);
			this.IsGamepadOverlayEnabled = collection.IsGamepadOverlayEnabled;
			this.IsMappingOverlayEnabled = collection.IsMappingOverlayEnabled;
			return true;
		}

		private void SetDefaultAssociations(HotkeyCollection entry)
		{
			entry.Slot1AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 5
				},
				new AssociatedControllerButton
				{
					GamepadButton = 7
				},
				new AssociatedControllerButton
				{
					GamepadButton = 1
				}
			};
			entry.Slot2AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 5
				},
				new AssociatedControllerButton
				{
					GamepadButton = 7
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2
				}
			};
			entry.Slot3AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 5
				},
				new AssociatedControllerButton
				{
					GamepadButton = 7
				},
				new AssociatedControllerButton
				{
					GamepadButton = 3
				}
			};
			entry.Slot4AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 5
				},
				new AssociatedControllerButton
				{
					GamepadButton = 7
				},
				new AssociatedControllerButton
				{
					GamepadButton = 4
				}
			};
			entry.GamepadOverlayAssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 8
				},
				new AssociatedControllerButton
				{
					GamepadButton = 34
				},
				new AssociatedControllerButton
				{
					GamepadButton = 36
				}
			};
			entry.MappingOverlayAssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 8
				},
				new AssociatedControllerButton
				{
					GamepadButton = 33
				},
				new AssociatedControllerButton
				{
					GamepadButton = 35
				}
			};
			entry.MappingDescriptionsOverlayAssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				}
			};
		}

		private void SetKeyboardAssociations(HotkeyCollection entry)
		{
			entry.Slot1AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					KeyScanCode = KeyScanCodeV2.FindKeyScanCodeByKey(Key.LeftCtrl)
				},
				new AssociatedControllerButton
				{
					KeyScanCode = KeyScanCodeV2.FindKeyScanCodeByKey(Key.LeftShift)
				},
				new AssociatedControllerButton
				{
					KeyScanCode = KeyScanCodeV2.FindKeyScanCodeByKey(Key.F1)
				}
			};
			entry.Slot2AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					KeyScanCode = KeyScanCodeV2.FindKeyScanCodeByKey(Key.LeftCtrl)
				},
				new AssociatedControllerButton
				{
					KeyScanCode = KeyScanCodeV2.FindKeyScanCodeByKey(Key.LeftShift)
				},
				new AssociatedControllerButton
				{
					KeyScanCode = KeyScanCodeV2.FindKeyScanCodeByKey(Key.F2)
				}
			};
			entry.Slot3AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					KeyScanCode = KeyScanCodeV2.FindKeyScanCodeByKey(Key.LeftCtrl)
				},
				new AssociatedControllerButton
				{
					KeyScanCode = KeyScanCodeV2.FindKeyScanCodeByKey(Key.LeftShift)
				},
				new AssociatedControllerButton
				{
					KeyScanCode = KeyScanCodeV2.FindKeyScanCodeByKey(Key.F3)
				}
			};
			entry.Slot4AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					KeyScanCode = KeyScanCodeV2.FindKeyScanCodeByKey(Key.LeftCtrl)
				},
				new AssociatedControllerButton
				{
					KeyScanCode = KeyScanCodeV2.FindKeyScanCodeByKey(Key.LeftShift)
				},
				new AssociatedControllerButton
				{
					KeyScanCode = KeyScanCodeV2.FindKeyScanCodeByKey(Key.F4)
				}
			};
			entry.GamepadOverlayAssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					KeyScanCode = KeyScanCodeV2.FindKeyScanCodeByKey(Key.LeftCtrl)
				},
				new AssociatedControllerButton
				{
					KeyScanCode = KeyScanCodeV2.FindKeyScanCodeByKey(Key.LeftShift)
				},
				new AssociatedControllerButton
				{
					KeyScanCode = KeyScanCodeV2.FindKeyScanCodeByKey(Key.F12)
				}
			};
			entry.MappingOverlayAssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					KeyScanCode = KeyScanCodeV2.FindKeyScanCodeByKey(Key.LeftCtrl)
				},
				new AssociatedControllerButton
				{
					KeyScanCode = KeyScanCodeV2.FindKeyScanCodeByKey(Key.LeftShift)
				},
				new AssociatedControllerButton
				{
					KeyScanCode = KeyScanCodeV2.FindKeyScanCodeByKey(Key.F11)
				}
			};
		}

		private void SetNotSelectedAssociations(HotkeyCollection entry)
		{
			entry.IsSlot1Enabled = false;
			entry.IsSlot2Enabled = false;
			entry.IsSlot3Enabled = false;
			entry.IsSlot4Enabled = false;
			entry.Slot1AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				}
			};
			entry.Slot2AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				}
			};
			entry.Slot3AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				}
			};
			entry.Slot4AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				}
			};
			entry.GamepadOverlayAssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				}
			};
			entry.MappingOverlayAssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				}
			};
		}

		private void SetGamePadAssociations(HotkeyCollection entry, List<ControllerTypeEnum> controllerTypes)
		{
			entry.IsSlot1Enabled = true;
			entry.IsSlot2Enabled = true;
			entry.IsSlot3Enabled = true;
			entry.IsSlot4Enabled = true;
			if (controllerTypes.Contains(9) && !controllerTypes.Contains(10))
			{
				this.SetJoyConLeftAssociations(entry);
				return;
			}
			if (controllerTypes.Contains(10) && !controllerTypes.Contains(9))
			{
				this.SetJoyConRightAssociations(entry);
				return;
			}
			if (controllerTypes.All((ControllerTypeEnum ct) => ct == 50))
			{
				this.SetSegaGenesisAssociations(entry);
				return;
			}
			if (controllerTypes.All((ControllerTypeEnum ct) => ct == 49))
			{
				this.SetSwitchOnlineN64Associations(entry);
				return;
			}
			if (controllerTypes.All((ControllerTypeEnum ct) => ControllerTypeExtensions.IsNES(ct)))
			{
				this.SetNESAssociations(entry);
				return;
			}
			if (controllerTypes.All((ControllerTypeEnum ct) => ct == 11))
			{
				this.SetSonyPs3NavigationAssociations(entry);
				return;
			}
			if (controllerTypes.All((ControllerTypeEnum ct) => ControllerTypeExtensions.IsGameCube(ct)))
			{
				this.SetGameCubeAssociations(entry);
				return;
			}
			if (controllerTypes.All((ControllerTypeEnum ct) => ControllerTypeExtensions.IsAnySteam(ct)))
			{
				this.SetSteamAssociations(entry);
				return;
			}
			if (controllerTypes.All((ControllerTypeEnum ct) => ControllerTypeExtensions.IsAzeron(ct)))
			{
				this.SetAzeronAssociations(entry);
				return;
			}
			if (controllerTypes.All((ControllerTypeEnum ct) => ControllerTypeExtensions.IsAzeronCyborg(ct)))
			{
				this.SetAzeronCyborgAssociations(entry);
				return;
			}
			if (controllerTypes.All((ControllerTypeEnum ct) => ControllerTypeExtensions.IsAzeronCyro(ct)))
			{
				this.SetAzeronCyroAssociations(entry);
				return;
			}
			if (ControllerTypeExtensions.IsGamepad(controllerTypes[0]))
			{
				this.SetDefaultAssociations(entry);
				return;
			}
			if (this.ControllerFamily == 1)
			{
				this.SetKeyboardAssociations(entry);
				return;
			}
			if (this.ControllerFamily != 2)
			{
				if (!controllerTypes.All((ControllerTypeEnum ct) => ControllerTypeExtensions.IsEngineControllerTouchpad(ct)))
				{
					return;
				}
			}
			this.SetNotSelectedAssociations(entry);
		}

		private void SetJoyConLeftAssociations(HotkeyCollection entry)
		{
			entry.Slot1AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 51
				},
				new AssociatedControllerButton
				{
					GamepadButton = 5
				},
				new AssociatedControllerButton
				{
					GamepadButton = 34
				}
			};
			entry.Slot2AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 51
				},
				new AssociatedControllerButton
				{
					GamepadButton = 5
				},
				new AssociatedControllerButton
				{
					GamepadButton = 36
				}
			};
			entry.Slot3AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 51
				},
				new AssociatedControllerButton
				{
					GamepadButton = 5
				},
				new AssociatedControllerButton
				{
					GamepadButton = 35
				}
			};
			entry.Slot4AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 51
				},
				new AssociatedControllerButton
				{
					GamepadButton = 5
				},
				new AssociatedControllerButton
				{
					GamepadButton = 33
				}
			};
			entry.GamepadOverlayAssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 51
				},
				new AssociatedControllerButton
				{
					GamepadButton = 7
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				}
			};
			entry.MappingOverlayAssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 5
				},
				new AssociatedControllerButton
				{
					GamepadButton = 7
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				}
			};
		}

		private void SetJoyConRightAssociations(HotkeyCollection entry)
		{
			entry.Slot1AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 55
				},
				new AssociatedControllerButton
				{
					GamepadButton = 6
				},
				new AssociatedControllerButton
				{
					GamepadButton = 1
				}
			};
			entry.Slot2AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 55
				},
				new AssociatedControllerButton
				{
					GamepadButton = 6
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2
				}
			};
			entry.Slot3AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 55
				},
				new AssociatedControllerButton
				{
					GamepadButton = 6
				},
				new AssociatedControllerButton
				{
					GamepadButton = 3
				}
			};
			entry.Slot4AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 55
				},
				new AssociatedControllerButton
				{
					GamepadButton = 6
				},
				new AssociatedControllerButton
				{
					GamepadButton = 4
				}
			};
			entry.GamepadOverlayAssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 55
				},
				new AssociatedControllerButton
				{
					GamepadButton = 8
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				}
			};
			entry.MappingOverlayAssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 6
				},
				new AssociatedControllerButton
				{
					GamepadButton = 8
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				}
			};
		}

		private void SetSonyPs3NavigationAssociations(HotkeyCollection entry)
		{
			entry.Slot1AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 5
				},
				new AssociatedControllerButton
				{
					GamepadButton = 51
				},
				new AssociatedControllerButton
				{
					GamepadButton = 34
				}
			};
			entry.Slot2AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 5
				},
				new AssociatedControllerButton
				{
					GamepadButton = 51
				},
				new AssociatedControllerButton
				{
					GamepadButton = 36
				}
			};
			entry.Slot3AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 5
				},
				new AssociatedControllerButton
				{
					GamepadButton = 51
				},
				new AssociatedControllerButton
				{
					GamepadButton = 35
				}
			};
			entry.Slot4AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 5
				},
				new AssociatedControllerButton
				{
					GamepadButton = 51
				},
				new AssociatedControllerButton
				{
					GamepadButton = 33
				}
			};
			entry.GamepadOverlayAssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 34
				},
				new AssociatedControllerButton
				{
					GamepadButton = 36
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				}
			};
			entry.MappingOverlayAssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 33
				},
				new AssociatedControllerButton
				{
					GamepadButton = 35
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				}
			};
		}

		private void SetSegaGenesisAssociations(HotkeyCollection entry)
		{
			entry.Slot1AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 12
				},
				new AssociatedControllerButton
				{
					GamepadButton = 11
				},
				new AssociatedControllerButton
				{
					GamepadButton = 33
				}
			};
			entry.Slot2AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 12
				},
				new AssociatedControllerButton
				{
					GamepadButton = 11
				},
				new AssociatedControllerButton
				{
					GamepadButton = 36
				}
			};
			entry.Slot3AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 12
				},
				new AssociatedControllerButton
				{
					GamepadButton = 11
				},
				new AssociatedControllerButton
				{
					GamepadButton = 34
				}
			};
			entry.Slot4AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 12
				},
				new AssociatedControllerButton
				{
					GamepadButton = 11
				},
				new AssociatedControllerButton
				{
					GamepadButton = 35
				}
			};
		}

		private void SetNESAssociations(HotkeyCollection entry)
		{
			entry.Slot1AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 6
				},
				new AssociatedControllerButton
				{
					GamepadButton = 5
				},
				new AssociatedControllerButton
				{
					GamepadButton = 33
				}
			};
			entry.Slot2AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 6
				},
				new AssociatedControllerButton
				{
					GamepadButton = 5
				},
				new AssociatedControllerButton
				{
					GamepadButton = 36
				}
			};
			entry.Slot3AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 6
				},
				new AssociatedControllerButton
				{
					GamepadButton = 5
				},
				new AssociatedControllerButton
				{
					GamepadButton = 34
				}
			};
			entry.Slot4AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 6
				},
				new AssociatedControllerButton
				{
					GamepadButton = 5
				},
				new AssociatedControllerButton
				{
					GamepadButton = 35
				}
			};
			entry.GamepadOverlayAssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 6
				},
				new AssociatedControllerButton
				{
					GamepadButton = 34
				},
				new AssociatedControllerButton
				{
					GamepadButton = 36
				}
			};
			entry.MappingOverlayAssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 6
				},
				new AssociatedControllerButton
				{
					GamepadButton = 33
				},
				new AssociatedControllerButton
				{
					GamepadButton = 35
				}
			};
			entry.MappingDescriptionsOverlayAssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				}
			};
		}

		private void SetSwitchOnlineN64Associations(HotkeyCollection entry)
		{
			entry.Slot1AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 5
				},
				new AssociatedControllerButton
				{
					GamepadButton = 8
				},
				new AssociatedControllerButton
				{
					GamepadButton = 1
				}
			};
			entry.Slot2AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 5
				},
				new AssociatedControllerButton
				{
					GamepadButton = 8
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2
				}
			};
			entry.Slot3AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 5
				},
				new AssociatedControllerButton
				{
					GamepadButton = 8
				},
				new AssociatedControllerButton
				{
					GamepadButton = 3
				}
			};
			entry.Slot4AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 5
				},
				new AssociatedControllerButton
				{
					GamepadButton = 8
				},
				new AssociatedControllerButton
				{
					GamepadButton = 4
				}
			};
		}

		private void SetGameCubeAssociations(HotkeyCollection entry)
		{
			entry.Slot1AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 6
				},
				new AssociatedControllerButton
				{
					GamepadButton = 8
				},
				new AssociatedControllerButton
				{
					GamepadButton = 1
				}
			};
			entry.Slot2AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 6
				},
				new AssociatedControllerButton
				{
					GamepadButton = 8
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2
				}
			};
			entry.Slot3AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 6
				},
				new AssociatedControllerButton
				{
					GamepadButton = 8
				},
				new AssociatedControllerButton
				{
					GamepadButton = 3
				}
			};
			entry.Slot4AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 6
				},
				new AssociatedControllerButton
				{
					GamepadButton = 8
				},
				new AssociatedControllerButton
				{
					GamepadButton = 4
				}
			};
		}

		private void SetSteamAssociations(HotkeyCollection entry)
		{
			this.SetDefaultAssociations(entry);
			entry.GamepadOverlayAssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 9
				},
				new AssociatedControllerButton
				{
					GamepadButton = 32
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				}
			};
			entry.MappingOverlayAssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 9
				},
				new AssociatedControllerButton
				{
					GamepadButton = 31
				},
				new AssociatedControllerButton
				{
					GamepadButton = 2001
				}
			};
		}

		private void SetAzeronAssociations(HotkeyCollection entry)
		{
			entry.Slot1AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 21
				},
				new AssociatedControllerButton
				{
					GamepadButton = 22
				},
				new AssociatedControllerButton
				{
					GamepadButton = 33
				}
			};
			entry.Slot2AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 21
				},
				new AssociatedControllerButton
				{
					GamepadButton = 22
				},
				new AssociatedControllerButton
				{
					GamepadButton = 36
				}
			};
			entry.Slot3AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 21
				},
				new AssociatedControllerButton
				{
					GamepadButton = 22
				},
				new AssociatedControllerButton
				{
					GamepadButton = 34
				}
			};
			entry.Slot4AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 21
				},
				new AssociatedControllerButton
				{
					GamepadButton = 22
				},
				new AssociatedControllerButton
				{
					GamepadButton = 35
				}
			};
			entry.GamepadOverlayAssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 21
				},
				new AssociatedControllerButton
				{
					GamepadButton = 22
				},
				new AssociatedControllerButton
				{
					GamepadButton = 18
				}
			};
			entry.MappingOverlayAssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 21
				},
				new AssociatedControllerButton
				{
					GamepadButton = 22
				},
				new AssociatedControllerButton
				{
					GamepadButton = 17
				}
			};
		}

		private void SetAzeronCyborgAssociations(HotkeyCollection entry)
		{
			entry.Slot1AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 19
				},
				new AssociatedControllerButton
				{
					GamepadButton = 20
				},
				new AssociatedControllerButton
				{
					GamepadButton = 33
				}
			};
			entry.Slot2AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 19
				},
				new AssociatedControllerButton
				{
					GamepadButton = 20
				},
				new AssociatedControllerButton
				{
					GamepadButton = 36
				}
			};
			entry.Slot3AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 19
				},
				new AssociatedControllerButton
				{
					GamepadButton = 20
				},
				new AssociatedControllerButton
				{
					GamepadButton = 34
				}
			};
			entry.Slot4AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 19
				},
				new AssociatedControllerButton
				{
					GamepadButton = 20
				},
				new AssociatedControllerButton
				{
					GamepadButton = 35
				}
			};
			entry.GamepadOverlayAssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 19
				},
				new AssociatedControllerButton
				{
					GamepadButton = 20
				},
				new AssociatedControllerButton
				{
					GamepadButton = 18
				}
			};
			entry.MappingOverlayAssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 19
				},
				new AssociatedControllerButton
				{
					GamepadButton = 20
				},
				new AssociatedControllerButton
				{
					GamepadButton = 17
				}
			};
		}

		private void SetAzeronCyroAssociations(HotkeyCollection entry)
		{
			entry.Slot1AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 3
				},
				new AssociatedControllerButton
				{
					GamepadButton = 4
				},
				new AssociatedControllerButton
				{
					GamepadButton = 33
				}
			};
			entry.Slot2AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 3
				},
				new AssociatedControllerButton
				{
					GamepadButton = 4
				},
				new AssociatedControllerButton
				{
					GamepadButton = 36
				}
			};
			entry.Slot3AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 3
				},
				new AssociatedControllerButton
				{
					GamepadButton = 4
				},
				new AssociatedControllerButton
				{
					GamepadButton = 34
				}
			};
			entry.Slot4AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 3
				},
				new AssociatedControllerButton
				{
					GamepadButton = 4
				},
				new AssociatedControllerButton
				{
					GamepadButton = 35
				}
			};
			entry.GamepadOverlayAssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 3
				},
				new AssociatedControllerButton
				{
					GamepadButton = 4
				},
				new AssociatedControllerButton
				{
					GamepadButton = 15
				}
			};
			entry.MappingOverlayAssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>
			{
				new AssociatedControllerButton
				{
					GamepadButton = 3
				},
				new AssociatedControllerButton
				{
					GamepadButton = 4
				},
				new AssociatedControllerButton
				{
					GamepadButton = 16
				}
			};
		}

		private ObservableCollection<AssociatedControllerButton> _slot1AssociatedButtonCollection;

		private ObservableCollection<AssociatedControllerButton> _slot2AssociatedButtonCollection;

		private ObservableCollection<AssociatedControllerButton> _slot3AssociatedButtonCollection;

		private ObservableCollection<AssociatedControllerButton> _slot4AssociatedButtonCollection;

		private ObservableCollection<AssociatedControllerButton> _gamepadOverlayAssociatedButtonCollection;

		private ObservableCollection<AssociatedControllerButton> _mappingOverlayAssociatedButtonCollection;

		private ObservableCollection<AssociatedControllerButton> _mappingDescriptionsOverlayAssociatedButtonCollection;

		private bool _isSlot1Enabled;

		private bool _isSlot2Enabled;

		private bool _isSlot3Enabled;

		private bool _isSlot4Enabled;

		private bool _isGamepadOverlayEnabled;

		private bool _isMappingOverlayEnabled;

		private ObservableCollection<object> _buttonsForSlotHotkey;
	}
}
