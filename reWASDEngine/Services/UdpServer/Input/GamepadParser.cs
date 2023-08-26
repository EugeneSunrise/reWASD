using System;
using System.Collections.Generic;
using reWASDCommon.Infrastructure.Enums;
using reWASDEngine.Services.UdpServer.Utils;

namespace reWASDEngine.Services.UdpServer.Input
{
	internal class GamepadParser
	{
		public static GamepadData Parse(byte[] data)
		{
			GamepadData gamepadData = default(GamepadData);
			int num = (int)data[0];
			byte[] part = ByteUtils.GetPart(data, 1, num);
			int num2 = 0;
			if (num2 < num)
			{
				gamepadData.LeftTrigger = ByteUtils.GetUShort(ByteUtils.GetPart(part, num2, 2));
			}
			num2 += 2;
			if (num2 < num)
			{
				gamepadData.RightTrigger = ByteUtils.GetUShort(ByteUtils.GetPart(part, num2, 2));
			}
			num2 += 2;
			if (num2 < num)
			{
				gamepadData.LeftStickX = ByteUtils.GetShort(ByteUtils.GetPart(part, num2, 2));
			}
			num2 += 2;
			if (num2 < num)
			{
				gamepadData.LeftStickY = ByteUtils.GetShort(ByteUtils.GetPart(part, num2, 2));
			}
			num2 += 2;
			if (num2 < num)
			{
				gamepadData.RightStickX = ByteUtils.GetShort(ByteUtils.GetPart(part, num2, 2));
			}
			num2 += 2;
			if (num2 < num)
			{
				gamepadData.RightStickY = ByteUtils.GetShort(ByteUtils.GetPart(part, num2, 2));
			}
			num2 += 2;
			if (num2 < num)
			{
				gamepadData.LeftFingerX = ByteUtils.GetUShort(ByteUtils.GetPart(part, num2, 2));
			}
			num2 += 2;
			if (num2 < num)
			{
				gamepadData.LeftFingerY = ByteUtils.GetUShort(ByteUtils.GetPart(part, num2, 2));
			}
			num2 += 2;
			if (num2 < num)
			{
				gamepadData.LeftFingerEventId = part[num2];
			}
			num2++;
			if (num2 < num)
			{
				gamepadData.RightFingerX = ByteUtils.GetUShort(ByteUtils.GetPart(part, num2, 2));
			}
			num2 += 2;
			if (num2 < num)
			{
				gamepadData.RightFingerY = ByteUtils.GetUShort(ByteUtils.GetPart(part, num2, 2));
			}
			num2 += 2;
			if (num2 < num)
			{
				gamepadData.RightFingerEventId = part[num2];
			}
			num2++;
			gamepadData.PressedButtons = new List<GamepadButton>();
			int @int = ByteUtils.GetInt(ByteUtils.GetPart(data, num + 1, 1));
			byte[] part2 = ByteUtils.GetPart(data, num + 2, @int);
			for (int i = 0; i < @int; i++)
			{
				gamepadData.PressedButtons.Add(part2[i]);
			}
			return gamepadData;
		}
	}
}
