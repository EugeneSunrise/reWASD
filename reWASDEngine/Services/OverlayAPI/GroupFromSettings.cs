using System;
using System.Collections.Generic;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Infrastructure.KeyBindingsModel;

namespace reWASDEngine.Services.OverlayAPI
{
	public class GroupFromSettings
	{
		public List<AssociatedControllerButton> groupButtons;

		public ControllerTypeEnum? CurrentGamepadType;
	}
}
