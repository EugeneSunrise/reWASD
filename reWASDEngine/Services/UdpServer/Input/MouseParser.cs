using System;
using reWASDEngine.Services.UdpServer.Utils;

namespace reWASDEngine.Services.UdpServer.Input
{
	internal class MouseParser
	{
		public static MouseData Parse(byte[] data)
		{
			MouseData mouseData = default(MouseData);
			int num = 0;
			mouseData.Buttons = data[num];
			num++;
			mouseData.MouseXDelta = ByteUtils.GetLong(ByteUtils.GetPart(data, num, 8));
			num += 8;
			mouseData.MouseYDelta = ByteUtils.GetLong(ByteUtils.GetPart(data, num, 8));
			num += 8;
			mouseData.WheelXDelta = ByteUtils.GetLong(ByteUtils.GetPart(data, num, 8));
			num += 8;
			mouseData.WheelYDelta = ByteUtils.GetLong(ByteUtils.GetPart(data, num, 8));
			num += 8;
			return mouseData;
		}
	}
}
