using System;
using System.Collections.Generic;
using reWASDCommon.Infrastructure.Enums;

namespace reWASDEngine.Services.OverlayAPI
{
	public class ButtonsInMessageAllDevices
	{
		public int Index { get; set; }

		public List<ButtonsInMessage> Buttons { get; set; }

		public ControllerTypeEnum? CurrentGamepadType;
	}
}
