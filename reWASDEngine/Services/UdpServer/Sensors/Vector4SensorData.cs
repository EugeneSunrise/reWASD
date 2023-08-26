using System;

namespace reWASDEngine.Services.UdpServer.Sensors
{
	internal struct Vector4SensorData
	{
		public float X { readonly get; private set; }

		public float Y { readonly get; private set; }

		public float Z { readonly get; private set; }

		public float W { readonly get; private set; }

		public Vector4SensorData(float x, float y, float z, float w)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
			this.W = w;
		}
	}
}
