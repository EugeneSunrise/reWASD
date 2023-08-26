using System;
using reWASDEngine.Services.UdpServer.Utils;

namespace reWASDEngine.Services.UdpServer.Sensors
{
	internal class Vector4SensorParser
	{
		public static Vector4SensorData Parse(byte[] data)
		{
			int num = 4;
			return new Vector4SensorData(ByteUtils.GetFloat(ByteUtils.GetPart(data, 0, num)), ByteUtils.GetFloat(ByteUtils.GetPart(data, num, num)), ByteUtils.GetFloat(ByteUtils.GetPart(data, num * 2, num)), ByteUtils.GetFloat(ByteUtils.GetPart(data, num * 3, num)));
		}

		public const int AXIS_COUNT = 4;

		public const int AXIS_SIZE = 4;
	}
}
