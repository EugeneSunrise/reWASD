using System;
using System.Collections.ObjectModel;
using reWASDUI.DataModels.GamepadSlotHotkeyCollection;
using reWASDUI.Infrastructure.KeyBindings;
using XBEliteWPF.Infrastructure.KeyBindingsModel;

namespace reWASDUI.Utils.Extensions
{
	public static class GamepadsSlotHotkeyDictionaryExtension
	{
		private static ObservableCollection<AssociatedControllerButton> CloneSlotCollection(ObservableCollection<AssociatedControllerButton> collection)
		{
			ObservableCollection<AssociatedControllerButton> observableCollection = new ObservableCollection<AssociatedControllerButton>();
			foreach (AssociatedControllerButton associatedControllerButton in collection)
			{
				AssociatedControllerButton associatedControllerButton2 = new AssociatedControllerButton();
				associatedControllerButton.CopyToModel(associatedControllerButton2);
				observableCollection.Add(associatedControllerButton2);
			}
			return observableCollection;
		}

		public static SlotsHotkeyCollection Convert(this HotkeyCollection slotsHotkeyCollection)
		{
			if (slotsHotkeyCollection == null)
			{
				return null;
			}
			return new SlotsHotkeyCollection
			{
				ControllerTypes = slotsHotkeyCollection.ControllerTypes,
				ControllerFamily = slotsHotkeyCollection.ControllerFamily,
				DisplayName = slotsHotkeyCollection.DisplayName,
				Slot1AssociatedButtonCollection = GamepadsSlotHotkeyDictionaryExtension.CloneSlotCollection(slotsHotkeyCollection.Slot1AssociatedButtonCollection),
				Slot2AssociatedButtonCollection = GamepadsSlotHotkeyDictionaryExtension.CloneSlotCollection(slotsHotkeyCollection.Slot2AssociatedButtonCollection),
				Slot3AssociatedButtonCollection = GamepadsSlotHotkeyDictionaryExtension.CloneSlotCollection(slotsHotkeyCollection.Slot3AssociatedButtonCollection),
				Slot4AssociatedButtonCollection = GamepadsSlotHotkeyDictionaryExtension.CloneSlotCollection(slotsHotkeyCollection.Slot4AssociatedButtonCollection),
				GamepadOverlayAssociatedButtonCollection = GamepadsSlotHotkeyDictionaryExtension.CloneSlotCollection(slotsHotkeyCollection.GamepadOverlayAssociatedButtonCollection),
				MappingOverlayAssociatedButtonCollection = GamepadsSlotHotkeyDictionaryExtension.CloneSlotCollection(slotsHotkeyCollection.MappingOverlayAssociatedButtonCollection),
				MappingDescriptionsOverlayAssociatedButtonCollection = GamepadsSlotHotkeyDictionaryExtension.CloneSlotCollection(slotsHotkeyCollection.MappingDescriptionsOverlayAssociatedButtonCollection),
				IsSlot1Enabled = slotsHotkeyCollection.IsSlot1Enabled,
				IsSlot2Enabled = slotsHotkeyCollection.IsSlot2Enabled,
				IsSlot3Enabled = slotsHotkeyCollection.IsSlot3Enabled,
				IsSlot4Enabled = slotsHotkeyCollection.IsSlot4Enabled,
				IsGamepadOverlayEnabled = slotsHotkeyCollection.IsGamepadOverlayEnabled,
				IsMappingOverlayEnabled = slotsHotkeyCollection.IsMappingOverlayEnabled
			};
		}
	}
}
