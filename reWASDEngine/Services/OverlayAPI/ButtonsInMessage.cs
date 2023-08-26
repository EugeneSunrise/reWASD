using System;
using reWASDCommon.Infrastructure.Enums;

namespace reWASDEngine.Services.OverlayAPI
{
	public class ButtonsInMessage
	{
		public ControllerTypeEnum? CurrentGamepadType { get; set; }

		public MessageButtonInfo Button { get; set; }
	}
}
