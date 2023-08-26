using System;
using DiscSoftReWASDServiceNamespace;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.DataModels.GamepadActiveProfiles;
using XBEliteWPF.Infrastructure.Controller;

namespace XBEliteWPF.Infrastructure
{
	public delegate void ControllerSlotChangedHandler(BaseControllerVM controller, GamepadProfile gamepadProfile, Slot slot, REWASD_CONTROLLER_PROFILE? profile, bool physical);
}
