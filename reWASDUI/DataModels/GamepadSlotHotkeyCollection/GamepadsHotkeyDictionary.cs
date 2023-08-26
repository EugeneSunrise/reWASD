using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using DiscSoft.NET.Common.Utils;
using reWASDUI.Infrastructure.KeyBindings;

namespace reWASDUI.DataModels.GamepadSlotHotkeyCollection
{
	[Serializable]
	public class GamepadsHotkeyDictionary : ObservableDictionary<string, HotkeyCollection>
	{
		public bool IsLoadSucess { get; private set; }

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

		public event PropertyChangedExtendedEventHandler ControllerButtonChanged;

		private void OnControllerButtonChanged(object sender, PropertyChangedExtendedEventArgs e)
		{
			if (this.ControllerButtonChanged != null)
			{
				this.ControllerButtonChanged(sender, e);
			}
		}

		public void SubscribeChangeSlotsEvents()
		{
			foreach (KeyValuePair<string, HotkeyCollection> keyValuePair in ((IEnumerable<KeyValuePair<string, HotkeyCollection>>)this))
			{
				this.SubscribeEvents(keyValuePair.Value.Slot1AssociatedButtonCollection);
				this.SubscribeEvents(keyValuePair.Value.Slot2AssociatedButtonCollection);
				this.SubscribeEvents(keyValuePair.Value.Slot3AssociatedButtonCollection);
				this.SubscribeEvents(keyValuePair.Value.Slot4AssociatedButtonCollection);
			}
		}

		public void SubscribeChangeOverlayEvents()
		{
			foreach (KeyValuePair<string, HotkeyCollection> keyValuePair in ((IEnumerable<KeyValuePair<string, HotkeyCollection>>)this))
			{
				this.SubscribeEvents(keyValuePair.Value.GamepadOverlayAssociatedButtonCollection);
				this.SubscribeEvents(keyValuePair.Value.MappingOverlayAssociatedButtonCollection);
				this.SubscribeEvents(keyValuePair.Value.MappingDescriptionsOverlayAssociatedButtonCollection);
			}
		}

		private void SubscribeEvents(ObservableCollection<AssociatedControllerButton> collection)
		{
			foreach (AssociatedControllerButton associatedControllerButton in collection)
			{
				associatedControllerButton.ControllerButtonChanged += this.OnControllerButtonChanged;
			}
		}

		public void UnsubscribeChangeSlotsEvents()
		{
			foreach (KeyValuePair<string, HotkeyCollection> keyValuePair in ((IEnumerable<KeyValuePair<string, HotkeyCollection>>)this))
			{
				this.UnsubscribeEvents(keyValuePair.Value.Slot1AssociatedButtonCollection);
				this.UnsubscribeEvents(keyValuePair.Value.Slot2AssociatedButtonCollection);
				this.UnsubscribeEvents(keyValuePair.Value.Slot3AssociatedButtonCollection);
				this.UnsubscribeEvents(keyValuePair.Value.Slot4AssociatedButtonCollection);
			}
		}

		public void UnsubscribeChangeOverlayEvents()
		{
			foreach (KeyValuePair<string, HotkeyCollection> keyValuePair in ((IEnumerable<KeyValuePair<string, HotkeyCollection>>)this))
			{
				this.UnsubscribeEvents(keyValuePair.Value.GamepadOverlayAssociatedButtonCollection);
				this.UnsubscribeEvents(keyValuePair.Value.MappingOverlayAssociatedButtonCollection);
				this.UnsubscribeEvents(keyValuePair.Value.MappingDescriptionsOverlayAssociatedButtonCollection);
			}
		}

		private void UnsubscribeEvents(ObservableCollection<AssociatedControllerButton> collection)
		{
			foreach (AssociatedControllerButton associatedControllerButton in collection)
			{
				associatedControllerButton.ControllerButtonChanged -= this.OnControllerButtonChanged;
			}
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
