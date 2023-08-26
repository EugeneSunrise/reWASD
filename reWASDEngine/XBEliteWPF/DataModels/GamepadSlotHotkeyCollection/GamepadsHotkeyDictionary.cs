using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Input;
using DiscSoft.NET.Common.Utils;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Services;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtil;

namespace XBEliteWPF.DataModels.GamepadSlotHotkeyCollection
{
	[Serializable]
	public class GamepadsHotkeyDictionary : ObservableDictionary<string, HotkeyCollection>
	{
		public GamepadsHotkeyDictionary()
		{
		}

		public GamepadsHotkeyDictionary(ObservableDictionary<string, HotkeyCollection> dictionary)
		{
			foreach (KeyValuePair<string, HotkeyCollection> keyValuePair in ((IEnumerable<KeyValuePair<string, HotkeyCollection>>)dictionary))
			{
				base.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}

		public void RefreshEntry(string ID, string displayName)
		{
			if (displayName != null && base.ContainsKey(ID))
			{
				base[ID].DisplayName = displayName;
			}
		}

		public void AddDefaultEntryIfNotPresent(string ID, uint[] controllerTypes, ControllerFamily controllerFamily, string displayName, ulong[] ids = null)
		{
			this.AddDefaultEntryIfNotPresent(ID, ControllerTypeExtensions.ConvertPhysicalTypesToEnums(0, controllerTypes, ids).ToList<ControllerTypeEnum>(), controllerFamily, displayName);
		}

		public void AddDefaultEntryIfNotPresent(string ID, List<ControllerTypeEnum> controllerTypes, ControllerFamily controllerFamily, string displayName)
		{
			if (controllerFamily == 4)
			{
				return;
			}
			if (base.ContainsKey(ID))
			{
				if (base[ID].DisplayName != displayName || base[ID].ControllerFamily != controllerFamily)
				{
					base[ID].DisplayName = displayName;
					base[ID].ControllerFamily = controllerFamily;
					BinDataSerialize.Save(this, BinDataSerialize.USER_IMPERSONATED_GAMEPAD_HOTKEY_SLOT_COLLECTION_FULL_SAVE_FILE_PATH);
				}
				return;
			}
			controllerTypes.RemoveAll((ControllerTypeEnum ct) => ct == 0);
			if (controllerTypes.Count == 1 && ControllerTypeExtensions.IsEngineMouseTouchpad(controllerTypes.First<ControllerTypeEnum>()))
			{
				return;
			}
			HotkeyCollection hotkeyCollection = new HotkeyCollection(controllerTypes, controllerFamily, displayName);
			hotkeyCollection.IsGamepadOverlayEnabled = true;
			hotkeyCollection.IsMappingOverlayEnabled = true;
			if (RegistryHelper.GetValue("Config", "UseHotkeyToSwitchSlots", -1, false) != 0)
			{
				hotkeyCollection.IsSlot1Enabled = true;
				hotkeyCollection.IsSlot2Enabled = true;
				hotkeyCollection.IsSlot3Enabled = true;
				hotkeyCollection.IsSlot4Enabled = true;
			}
			hotkeyCollection.IsGamepadOverlayEnabled = true;
			hotkeyCollection.IsMappingOverlayEnabled = true;
			this.SetGamePadAssociations(hotkeyCollection, controllerTypes);
			if (controllerFamily == 1)
			{
				this.SetKeyboardAssociations(hotkeyCollection);
			}
			if (controllerFamily == 2 || controllerFamily == 4 || (controllerFamily == 3 && (!controllerTypes.Contains(9) || !controllerTypes.Contains(10))))
			{
				this.SetNotSelectedAssociations(hotkeyCollection);
			}
			base.Add(ID, hotkeyCollection);
			BinDataSerialize.Save(this, BinDataSerialize.USER_IMPERSONATED_GAMEPAD_HOTKEY_SLOT_COLLECTION_FULL_SAVE_FILE_PATH);
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
			if (controllerTypes.All((ControllerTypeEnum ct) => ControllerTypeExtensions.IsSNES(ct)))
			{
				this.SetSNESAssociations(entry);
				return;
			}
			if (controllerTypes.All((ControllerTypeEnum ct) => ControllerTypeExtensions.IsEngineControllerTouchpad(ct)))
			{
				entry.IsSlot1Enabled = false;
				entry.IsSlot2Enabled = false;
				entry.IsSlot3Enabled = false;
				entry.IsSlot4Enabled = false;
				return;
			}
			if (RegistryHelper.GetValue("Config", "UseHotkeyToSwitchSlots", -1, false) != -1)
			{
				ObservableCollection<AssociatedControllerButton> slotHotkeyCollectionFromRegistry = XBUtils.GetSlotHotkeyCollectionFromRegistry(0);
				ObservableCollection<AssociatedControllerButton> slotHotkeyCollectionFromRegistry2 = XBUtils.GetSlotHotkeyCollectionFromRegistry(1);
				ObservableCollection<AssociatedControllerButton> slotHotkeyCollectionFromRegistry3 = XBUtils.GetSlotHotkeyCollectionFromRegistry(2);
				ObservableCollection<AssociatedControllerButton> slotHotkeyCollectionFromRegistry4 = XBUtils.GetSlotHotkeyCollectionFromRegistry(3);
				entry.Slot1AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>(slotHotkeyCollectionFromRegistry);
				entry.Slot2AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>(slotHotkeyCollectionFromRegistry2);
				entry.Slot3AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>(slotHotkeyCollectionFromRegistry3);
				entry.Slot4AssociatedButtonCollection = new ObservableCollection<AssociatedControllerButton>(slotHotkeyCollectionFromRegistry4);
				return;
			}
			this.SetDefaultAssociations(entry);
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

		private void SetSNESAssociations(HotkeyCollection entry)
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
					GamepadButton = 7
				},
				new AssociatedControllerButton
				{
					GamepadButton = 6
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
					GamepadButton = 7
				},
				new AssociatedControllerButton
				{
					GamepadButton = 5
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

		protected GamepadsHotkeyDictionary(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			foreach (SerializationEntry serializationEntry in info)
			{
				if (serializationEntry.Name == "AdditionalParameters")
				{
					this.AdditionalParameters = (Dictionary<string, string>)info.GetValue("AdditionalParameters", typeof(Dictionary<string, string>));
				}
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("AdditionalParameters", this.AdditionalParameters);
		}

		public GamepadsHotkeyDictionary CloneSlots()
		{
			GamepadsHotkeyDictionary gamepadsHotkeyDictionary = new GamepadsHotkeyDictionary();
			foreach (KeyValuePair<string, HotkeyCollection> keyValuePair in ((IEnumerable<KeyValuePair<string, HotkeyCollection>>)this))
			{
				gamepadsHotkeyDictionary.Add(keyValuePair.Key, keyValuePair.Value.CloneSlots());
			}
			return gamepadsHotkeyDictionary;
		}

		public GamepadsHotkeyDictionary CloneOverlays()
		{
			GamepadsHotkeyDictionary gamepadsHotkeyDictionary = new GamepadsHotkeyDictionary();
			foreach (KeyValuePair<string, HotkeyCollection> keyValuePair in ((IEnumerable<KeyValuePair<string, HotkeyCollection>>)this))
			{
				gamepadsHotkeyDictionary.Add(keyValuePair.Key, keyValuePair.Value.CloneOverlays());
			}
			return gamepadsHotkeyDictionary;
		}

		public bool MergeSlots(GamepadsHotkeyDictionary gampadSlots)
		{
			foreach (KeyValuePair<string, HotkeyCollection> keyValuePair in ((IEnumerable<KeyValuePair<string, HotkeyCollection>>)this))
			{
				if (!keyValuePair.Value.MergeSlots(gampadSlots[keyValuePair.Key]))
				{
					return false;
				}
			}
			return true;
		}

		public bool MergeOverlay(GamepadsHotkeyDictionary gampadSlots)
		{
			foreach (KeyValuePair<string, HotkeyCollection> keyValuePair in ((IEnumerable<KeyValuePair<string, HotkeyCollection>>)this))
			{
				if (!keyValuePair.Value.MergeOverlay(gampadSlots[keyValuePair.Key]))
				{
					return false;
				}
			}
			return true;
		}

		public readonly Dictionary<string, string> AdditionalParameters = new Dictionary<string, string>();
	}
}
