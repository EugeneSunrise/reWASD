using System;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Services;

namespace reWASDUI.Infrastructure
{
	public interface ISetSlotModel
	{
		Slot SelectedSlot { get; set; }

		GamepadService GamepadService { get; }

		LicensingService LicensingService { get; }
	}
}
