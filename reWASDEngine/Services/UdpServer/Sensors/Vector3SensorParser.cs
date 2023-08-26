using System;
using reWASDEngine.Services.UdpServer.Utils;

namespace reWASDEngine.Services.UdpServer.Sensors
{
	internal class Vector3SensorParser
	{
		public static Vector3SensorData Parse(byte[] data)
		{
			int num = 4;
			return new Vector3SensorData(ByteUtils.GetFloat(ByteUtils.GetPart(data, 0, num)), ByteUtils.GetFloat(ByteUtils.GetPart(data, num, num)), ByteUtils.GetFloat(ByteUtils.GetPart(data, num * 2, num)));
		}

		public const int AXIS_COUNT = 3;

		public const int AXIS_SIZE = 4;
	}
}
