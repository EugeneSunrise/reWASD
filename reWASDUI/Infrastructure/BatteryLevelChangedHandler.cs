using System;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.Controller;

namespace reWASDUI.Infrastructure
{
	public delegate void BatteryLevelChangedHandler(BaseControllerVM controller, BatteryLevel batteryLevel);
}
