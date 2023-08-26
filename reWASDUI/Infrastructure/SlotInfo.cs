using System;
using System.Collections.ObjectModel;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.DataModels.GamepadSlotHotkeyCollection;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Services.Interfaces;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.ViewModels.Base;

namespace reWASDUI.Infrastructure
{
	public class SlotInfo : ZBindable
	{
		public bool IsAvailable
		{
			get
			{
				return this._propertyName;
			}
			set
			{
				this.SetProperty<bool>(ref this._propertyName, value, "IsAvailable");
			}
		}

		public Slot Slot
		{
			get
			{
				return this._slot;
			}
			set
			{
				this.SetProperty<Slot>(ref this._slot, value, "Slot");
			}
		}

		public ObservableCollection<AssociatedControllerButton> SlotHotkeyCollection
		{
			get
			{
				if (this.GamepadService.CurrentGamepad == null)
				{
					return null;
				}
				switch (this.Slot)
				{
				case 0:
				{
					HotkeyCollection hotkeyCollection = this.GamepadService.GamepadsHotkeyCollection.TryGetValue(this.GamepadService.CurrentGamepad.ID);
					if (hotkeyCollection == null)
					{
						return null;
					}
					return hotkeyCollection.Slot1AssociatedButtonCollection;
				}
				case 1:
				{
					HotkeyCollection hotkeyCollection2 = this.GamepadService.GamepadsHotkeyCollection.TryGetValue(this.GamepadService.CurrentGamepad.ID);
					if (hotkeyCollection2 == null)
					{
						return null;
					}
					return hotkeyCollection2.Slot2AssociatedButtonCollection;
				}
				case 2:
				{
					HotkeyCollection hotkeyCollection3 = this.GamepadService.GamepadsHotkeyCollection.TryGetValue(this.GamepadService.CurrentGamepad.ID);
					if (hotkeyCollection3 == null)
					{
						return null;
					}
					return hotkeyCollection3.Slot3AssociatedButtonCollection;
				}
				case 3:
				{
					HotkeyCollection hotkeyCollection4 = this.GamepadService.GamepadsHotkeyCollection.TryGetValue(this.GamepadService.CurrentGamepad.ID);
					if (hotkeyCollection4 == null)
					{
						return null;
					}
					return hotkeyCollection4.Slot4AssociatedButtonCollection;
				}
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		}

		public bool IsSlotHotkeyAssigned
		{
			get
			{
				if (this.GamepadService.CurrentGamepad == null)
				{
					return false;
				}
				switch (this.Slot)
				{
				case 0:
				{
					HotkeyCollection hotkeyCollection = this.GamepadService.GamepadsHotkeyCollection.TryGetValue(this.GamepadService.CurrentGamepad.ID);
					return hotkeyCollection != null && hotkeyCollection.IsSlot1Enabled;
				}
				case 1:
				{
					HotkeyCollection hotkeyCollection2 = this.GamepadService.GamepadsHotkeyCollection.TryGetValue(this.GamepadService.CurrentGamepad.ID);
					return hotkeyCollection2 != null && hotkeyCollection2.IsSlot2Enabled;
				}
				case 2:
				{
					HotkeyCollection hotkeyCollection3 = this.GamepadService.GamepadsHotkeyCollection.TryGetValue(this.GamepadService.CurrentGamepad.ID);
					return hotkeyCollection3 != null && hotkeyCollection3.IsSlot3Enabled;
				}
				case 3:
				{
					HotkeyCollection hotkeyCollection4 = this.GamepadService.GamepadsHotkeyCollection.TryGetValue(this.GamepadService.CurrentGamepad.ID);
					return hotkeyCollection4 != null && hotkeyCollection4.IsSlot4Enabled;
				}
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		}

		public IGamepadService GamepadService { get; set; }

		public ILicensingService LicensingService { get; set; }

		public SlotInfo(Slot slot, bool isAvailable)
		{
			this.Slot = slot;
			this.IsAvailable = isAvailable;
		}

		public SlotInfo(Slot slot, bool isAvailable, IGamepadService gamepadService, ILicensingService licensingService)
			: this(slot, isAvailable)
		{
			this.GamepadService = gamepadService;
			this.LicensingService = licensingService;
		}

		public override string ToString()
		{
			return this.Slot.TryGetLocalizedDescription();
		}

		private bool _propertyName;

		private Slot _slot;
	}
}
