using System;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.DataModels.GamepadActiveProfiles;
using XBEliteWPF.Infrastructure.Controller;

namespace XBEliteWPF.Infrastructure
{
	public delegate void HiddenCommandEventHandler(RewasdHiddenServiceCommand rewasdHiddenServiceCommand, GamepadProfile gamepadProfile, string name, BaseControllerVM controller, bool toggle);
}
