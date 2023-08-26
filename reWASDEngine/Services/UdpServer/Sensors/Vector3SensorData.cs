using System;

namespace reWASDEngine.Services.UdpServer.Sensors
{
	internal struct Vector3SensorData
	{
		public float X { readonly get; private set; }

		public float Y { readonly get; private set; }

		public float Z { readonly get; private set; }

		public Vector3SensorData(float x, float y, float z)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}
	}
}
