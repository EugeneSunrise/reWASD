using System;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.ViewModels.Base;

namespace XBEliteWPF.Infrastructure
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
