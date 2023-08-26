using System;
using XBEliteWPF.DataModels.GamepadSlotHotkeyCollection;
using XBEliteWPF.Infrastructure.KeyBindingsModel;

namespace XBEliteWPF.Utils.Extensions
{
	public static class GamepadsSlotHotkeyDictionaryExtension
	{
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
				Slot1AssociatedButtonCollection = slotsHotkeyCollection.Slot1AssociatedButtonCollection,
				Slot2AssociatedButtonCollection = slotsHotkeyCollection.Slot2AssociatedButtonCollection,
				Slot3AssociatedButtonCollection = slotsHotkeyCollection.Slot3AssociatedButtonCollection,
				Slot4AssociatedButtonCollection = slotsHotkeyCollection.Slot4AssociatedButtonCollection,
				GamepadOverlayAssociatedButtonCollection = slotsHotkeyCollection.GamepadOverlayAssociatedButtonCollection,
				MappingOverlayAssociatedButtonCollection = slotsHotkeyCollection.MappingOverlayAssociatedButtonCollection,
				MappingDescriptionsOverlayAssociatedButtonCollection = slotsHotkeyCollection.MappingDescriptionsOverlayAssociatedButtonCollection,
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
