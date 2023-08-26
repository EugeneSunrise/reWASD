using System;
using System.Collections.Generic;
using System.Linq;
using reWASDCommon.Infrastructure;
using XBEliteWPF.Infrastructure.reWASDMapping.RewasduserCommands;

namespace reWASDUI.Infrastructure.reWASDMapping.RewasduserCommands
{
	public class BaseRewasdUserCommandInitializer
	{
		public static void Init()
		{
			if (BaseRewasdUserCommand.REWASD_USER_COMMAND_TABLE == null)
			{
				List<BaseRewasdUserCommand[]> list = new List<BaseRewasdUserCommand[]> { new BaseRewasdUserCommand[]
				{
					new BaseRewasdUserCommand(1, "COMMAND_TURN_REMAP_OFF", 11658, "UriBtnUnmapped", 0, 6, 1),
					new BaseRewasdUserCommand(2, "COMMAND_TURN_WIRELESS_JOY_OFF", 11659, "UriUserCommandTurnGamepadOff", 11826, 6, 2),
					new BaseRewasdUserCommand(3, "COMMAND_CLOSE_REWASD_GUI", 11660, "Remove", 0, 6, 3),
					new BaseRewasdUserCommand(4, "COMMAND_SWITCH_GYRO_AXIS", 11661, "UriUserCommandGyroSwitch", 0, 6, 2),
					new BaseRewasdUserCommand(5, "COMMAND_RESET_TILT", 11662, "UriUserCommandTiltReset", 0, 6, 2),
					new BaseRewasdUserCommand(7, "COMMAND_LOG_OUT", 11664, "UriUserCommandLogout", 0, 6, 3),
					new BaseRewasdUserCommand(8, "COMMAND_SHUTDOWN", 11665, "UriKeyScanCodePower", 0, 6, 3),
					new BaseRewasdUserCommand(17, "COMMAND_RESTART_PC", 12546, "UriUserCommandRestartPC", 0, 6, 3),
					new BaseRewasdUserCommand(9, "COMMAND_SLEEP", 11666, "UriKeyScanCodeSleep", 0, 6, 4),
					new BaseRewasdUserCommand(13, "COMMAND_HIBERNATE", 11667, "UriUserCommandHibernate", 0, 6, 5),
					new BaseRewasdUserCommand(10, "COMMAND_TURN_ANTIBOSS_MODE", 11668, "UriUserCommandAntibossMode", 0, 6, 6),
					new BaseRewasdUserCommand(11, "COMMAND_TAKE_SCREENSHOT", 11669, "UriUserCommandScreenshot", 0, 6, 7),
					new BaseRewasdUserCommand(12, "COMMAND_CLOSE_ACTIVE_TASK", 11670, "UriUserCommandCloseTask", 0, 6, 8),
					new BaseRewasdUserCommand(14, "COMMAND_RECONNECT_TO_EXTERNAL_TARGET", 12069, "UriUserCommandReconnectToTargetBT", 0, 6, 2),
					new BaseRewasdUserCommand(6, "COMMAND_TOGGLE_GYRO_ON_OFF", 11663, "UriUserCommandToggleGyro", 0, 6, 2),
					new BaseRewasdUserCommand(15, "COMMAND_TURN_GYRO_ON", 12344, "UriUserCommandToggleGyroOn", 0, 6, 2),
					new BaseRewasdUserCommand(16, "COMMAND_TURN_GYRO_OFF", 12345, "UriUserCommandToggleGyroOff", 0, 6, 2),
					new BaseRewasdUserCommand(18, "COMMAND_RESET_VIRTUAL_GYRO", 12794, "VGyroReset", 0, 6, 2)
				} };
				if (Constants.CreateOverlayShift)
				{
					list.Add(new BaseRewasdUserCommand[]
					{
						new BaseRewasdUserCommand(130, "OVERLAY_MENU_PREV_SECTOR", 12531, "OverlayMappingPrevSector", 0, 6, 101)
					});
					list.Add(new BaseRewasdUserCommand[]
					{
						new BaseRewasdUserCommand(131, "OVERLAY_MENU_NEXT_SECTOR", 12532, "OverlayMappingNextSector", 0, 6, 101)
					});
					list.Add(new BaseRewasdUserCommand[]
					{
						new BaseRewasdUserCommand(132, "OVERLAY_MENU_APPLY", 11099, "OverlayMappingApply", 0, 6, 101)
					});
					list.Add(new BaseRewasdUserCommand[]
					{
						new BaseRewasdUserCommand(133, "OVERLAY_MENU_CANCEL", 12718, "OverlayMappingCancel", 0, 6, 101)
					});
					list.Add(new BaseRewasdUserCommand[]
					{
						new BaseRewasdUserCommand(134, "OVERLAY_MENU_UP", 11047, "IcoSwipeUp", 0, 6, 101)
					});
					list.Add(new BaseRewasdUserCommand[]
					{
						new BaseRewasdUserCommand(135, "OVERLAY_MENU_DOWN", 11048, "IcoSwipeDown", 0, 6, 101)
					});
					list.Add(new BaseRewasdUserCommand[]
					{
						new BaseRewasdUserCommand(137, "OVERLAY_MENU_LEFT", 11049, "IcoSwipeLeft", 0, 6, 101)
					});
					list.Add(new BaseRewasdUserCommand[]
					{
						new BaseRewasdUserCommand(136, "OVERLAY_MENU_RIGHT", 11050, "IcoSwipeRight", 0, 6, 101)
					});
				}
				BaseRewasdUserCommand.REWASD_USER_COMMAND_TABLE = list.SelectMany((BaseRewasdUserCommand[] x) => x).ToArray<BaseRewasdUserCommand>();
			}
		}
	}
}
