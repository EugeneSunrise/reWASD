using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using reWASDCommon.Infrastructure;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Infrastructure.reWASDMapping.RewasduserCommands.RewasdCommands;

namespace XBEliteWPF.Infrastructure.reWASDMapping.RewasduserCommands
{
	public class BaseRewasdUserCommandInitializer
	{
		public static void Init()
		{
			if (BaseRewasdUserCommand.REWASD_USER_COMMAND_TABLE == null)
			{
				List<BaseRewasdUserCommand[]> list = new List<BaseRewasdUserCommand[]> { new BaseRewasdUserCommand[]
				{
					new RemapOffCommand(1, "COMMAND_TURN_REMAP_OFF", 11658, "UriBtnUnmapped"),
					new SimpleServiceUserCommand(2, "COMMAND_TURN_WIRELESS_JOY_OFF", 11659, "UriUserCommandTurnGamepadOff", 0, 11826),
					new CMDUserCommand(3, "COMMAND_CLOSE_REWASD_GUI", 11660, "Remove", "taskkill /im reWASD.exe"),
					new SimpleServiceUserCommand(4, "COMMAND_SWITCH_GYRO_AXIS", 11661, "UriUserCommandGyroSwitch", 2, 0),
					new SimpleServiceUserCommand(5, "COMMAND_RESET_TILT", 11662, "UriUserCommandTiltReset", 1, 0),
					new CMDUserCommand(7, "COMMAND_LOG_OUT", 11664, "UriUserCommandLogout", "shutdown -l"),
					new CMDUserCommand(8, "COMMAND_SHUTDOWN", 11665, "UriKeyScanCodePower", "shutdown -s"),
					new CMDUserCommand(17, "COMMAND_RESTART_PC", 12546, "UriUserCommandRestartPC", "shutdown -r"),
					new SendComputerToSleepCommand(9, "COMMAND_SLEEP", 11666, "UriKeyScanCodeSleep"),
					new SendComputerToHibernationCommand(13, "COMMAND_HIBERNATE", 11667, "UriUserCommandHibernate"),
					new MacroUserCommand(10, "COMMAND_TURN_ANTIBOSS_MODE", 11668, "UriUserCommandAntibossMode", new List<BaseRewasdMapping>
					{
						KeyScanCodeV2.FindKeyScanCodeByKey(Key.LWin),
						KeyScanCodeV2.FindKeyScanCodeByKey(Key.D),
						KeyScanCodeV2.FindKeyScanCodeByKey(Key.VolumeMute)
					}, 10),
					new TakeScreenshotCommand(11, "COMMAND_TAKE_SCREENSHOT", 11669, "UriUserCommandScreenshot"),
					new KillActiveTaskCommand(12, "COMMAND_CLOSE_ACTIVE_TASK", 11670, "UriUserCommandCloseTask"),
					new SimpleServiceUserCommand(14, "COMMAND_RECONNECT_TO_EXTERNAL_TARGET", 12069, "UriUserCommandReconnectToTargetBT", 4, 0),
					new SimpleServiceUserCommand(6, "COMMAND_TOGGLE_GYRO_ON_OFF", 11663, "UriUserCommandToggleGyro", 3, 0),
					new SimpleServiceUserCommand(15, "COMMAND_TURN_GYRO_ON", 12344, "UriUserCommandToggleGyroOn", 5, 0),
					new SimpleServiceUserCommand(16, "COMMAND_TURN_GYRO_OFF", 12345, "UriUserCommandToggleGyroOff", 6, 0),
					new SimpleServiceUserCommand(18, "COMMAND_RESET_VIRTUAL_GYRO", 12794, "VGyroReset", 7, 0)
				} };
				if (Constants.CreateOverlayShift)
				{
					list.Add(new BaseRewasdUserCommand[]
					{
						new OverlayMenuCommand(130, "OVERLAY_MENU_PREV_SECTOR", 12531, "UriUserCommandOverlayMenu", 2, 101),
						new OverlayMenuCommand(131, "OVERLAY_MENU_NEXT_SECTOR", 12532, "UriUserCommandOverlayMenu", 3, 101),
						new OverlayMenuCommand(132, "OVERLAY_MENU_APPLY", 11099, "UriUserCommandOverlayMenu", 4, 101),
						new OverlayMenuCommand(133, "OVERLAY_MENU_CANCEL", 12718, "UriUserCommandOverlayMenu", 5, 101),
						new OverlayMenuCommand(134, "OVERLAY_MENU_UP", 11047, "UriUserCommandOverlayMenu", 6, 101),
						new OverlayMenuCommand(135, "OVERLAY_MENU_DOWN", 11048, "UriUserCommandOverlayMenu", 7, 101),
						new OverlayMenuCommand(136, "OVERLAY_MENU_RIGHT", 11050, "UriUserCommandOverlayMenu", 8, 101),
						new OverlayMenuCommand(137, "OVERLAY_MENU_LEFT", 11049, "UriUserCommandOverlayMenu", 9, 101)
					});
				}
				list.Add(new BaseRewasdUserCommand[]
				{
					new ServiceHiddenCommand(10000, 22),
					new ServiceHiddenCommand(10001, 23),
					new ServiceHiddenCommand(10002, 24),
					new ServiceHiddenCommand(10003, 25),
					new ServiceHiddenCommand(10020, 26),
					new ServiceHiddenCommand(10021, 27),
					new ServiceHiddenCommand(10022, 28),
					new ServiceHiddenCommand(10023, 29),
					new ServiceHiddenCommand(10100, 0)
				});
				int num = 10101;
				for (byte b = 1; b <= 21; b += 1)
				{
					list.Add(new BaseRewasdUserCommand[]
					{
						new ServiceHiddenCommand(num, b)
					});
					num++;
				}
				for (byte b2 = 30; b2 <= 50; b2 += 1)
				{
					list.Add(new BaseRewasdUserCommand[]
					{
						new ServiceHiddenCommand(num, b2)
					});
					num++;
				}
				BaseRewasdUserCommand.REWASD_USER_COMMAND_TABLE = list.SelectMany((BaseRewasdUserCommand[] x) => x).ToArray<BaseRewasdUserCommand>();
			}
		}
	}
}
