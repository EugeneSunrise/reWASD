using System;
using System.Linq;

namespace reWASDEngine.Services.UdpServer.Utils
{
	public class ByteUtils
	{
		public static bool IsBitSet(byte b, int bitVal)
		{
			return ((int)b & bitVal) != 0;
		}

		public static byte[] GetPart(byte[] whole, int offset, int bytesCount)
		{
			return whole.Skip(offset).Take(bytesCount).ToArray<byte>();
		}

		public static int GetInt(byte[] val)
		{
			if (val.Length == 1)
			{
				return (int)val[0];
			}
			if (val.Length == 2)
			{
				return (int)ByteUtils.GetShort(val);
			}
			if (val.Length != 4)
			{
				return 0;
			}
			return BitConverter.ToInt32(ByteUtils.ReverseIfNeeded(val), 0);
		}

		public static short GetShort(byte[] val)
		{
			if (val.Length != 2)
			{
				return 0;
			}
			return BitConverter.ToInt16(ByteUtils.ReverseIfNeeded(val), 0);
		}

		public static ushort GetUShort(byte[] val)
		{
			if (val.Length != 2)
			{
				return 0;
			}
			return BitConverter.ToUInt16(ByteUtils.ReverseIfNeeded(val), 0);
		}

		public static long GetLong(byte[] val)
		{
			if (val.Length != 8)
			{
				return 0L;
			}
			return BitConverter.ToInt64(ByteUtils.ReverseIfNeeded(val), 0);
		}

		public static ulong GetULong(byte[] val)
		{
			if (val.Length != 8)
			{
				return 0UL;
			}
			return BitConverter.ToUInt64(ByteUtils.ReverseIfNeeded(val), 0);
		}

		public static float GetFloat(byte[] val)
		{
			if (val.Length != 4)
			{
				return 0f;
			}
			return BitConverter.ToSingle(ByteUtils.ReverseIfNeeded(val), 0);
		}

		public static byte[] ReverseIfNeeded(byte[] arr)
		{
			if (!BitConverter.IsLittleEndian)
			{
				return arr.AsEnumerable<byte>().Reverse<byte>().ToArray<byte>();
			}
			return arr;
		}
	}
}
