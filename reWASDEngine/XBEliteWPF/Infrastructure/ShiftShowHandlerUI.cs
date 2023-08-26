using System;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using XBEliteWPF.DataModels.GamepadActiveProfiles;
using XBEliteWPF.Infrastructure.Controller;

namespace XBEliteWPF.Infrastructure
{
	public delegate void ShiftShowHandlerUI(BaseControllerVM controller, GamepadProfile gamepadProfile, ShiftInfo shift, bool toggle, bool alwaysShow);
}
