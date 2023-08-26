using System;

namespace reWASDEngine.Services.UdpServer.Input
{
	internal class KeyboardParser
	{
		public static KeyboardData Parse(byte[] data)
		{
			KeyboardData keyboardData = default(KeyboardData);
			keyboardData.Keys = new byte[32];
			foreach (byte b in data)
			{
				int num = (int)((b - 1) / 8);
				int num2 = (int)((b - 1) % 8);
				byte[] keys = keyboardData.Keys;
				int num3 = num;
				keys[num3] |= (byte)(1 << num2);
			}
			return keyboardData;
		}
	}
}
