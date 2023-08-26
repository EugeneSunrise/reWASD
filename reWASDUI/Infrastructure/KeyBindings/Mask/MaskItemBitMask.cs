using System;
using System.Collections;
using DiscSoft.NET.Common.Properties;
using reWASDCommon.Infrastructure.Enums;

namespace reWASDUI.Infrastructure.KeyBindings.Mask
{
	public struct MaskItemBitMask
	{
		public MaskItemBitMask(ControllerFamily controllerFamily, byte controllerFamilyIndex, BitArray gamepadBitMaskValue = null, BitArray peripheralBitMaskValue = null)
		{
			this.ControllerFamilyIndex = controllerFamilyIndex;
			this.ControllerFamily = controllerFamily;
			this.GamepadBitMask = ((gamepadBitMaskValue != null) ? new BitArray(gamepadBitMaskValue) : null);
			this.PeripheralBitMask = ((peripheralBitMaskValue != null) ? new BitArray(peripheralBitMaskValue) : null);
		}

		public bool Equals(MaskItemBitMask other)
		{
			if (this.ControllerFamilyIndex != other.ControllerFamilyIndex || this.ControllerFamily != other.ControllerFamily)
			{
				return false;
			}
			BitArray gamepadBitMask = this.GamepadBitMask;
			int? num = ((gamepadBitMask != null) ? new int?(gamepadBitMask.Length) : null);
			BitArray gamepadBitMask2 = other.GamepadBitMask;
			int? num2 = ((gamepadBitMask2 != null) ? new int?(gamepadBitMask2.Length) : null);
			if ((num.GetValueOrDefault() == num2.GetValueOrDefault()) & (num != null == (num2 != null)))
			{
				BitArray peripheralBitMask = this.PeripheralBitMask;
				num2 = ((peripheralBitMask != null) ? new int?(peripheralBitMask.Length) : null);
				BitArray peripheralBitMask2 = other.PeripheralBitMask;
				num = ((peripheralBitMask2 != null) ? new int?(peripheralBitMask2.Length) : null);
				if ((num2.GetValueOrDefault() == num.GetValueOrDefault()) & (num2 != null == (num != null)))
				{
					BitArray gamepadBitMask3 = this.GamepadBitMask;
					num = ((gamepadBitMask3 != null) ? new int?(gamepadBitMask3.Length) : null);
					BitArray gamepadBitMask4 = other.GamepadBitMask;
					num2 = ((gamepadBitMask4 != null) ? new int?(gamepadBitMask4.Length) : null);
					if ((num.GetValueOrDefault() == num2.GetValueOrDefault()) & (num != null == (num2 != null)))
					{
						int num3 = 0;
						for (;;)
						{
							int num4 = num3;
							BitArray gamepadBitMask5 = this.GamepadBitMask;
							num2 = ((gamepadBitMask5 != null) ? new int?(gamepadBitMask5.Length) : null);
							if (!((num4 < num2.GetValueOrDefault()) & (num2 != null)))
							{
								goto IL_1A1;
							}
							if (this.GamepadBitMask[num3] != other.GamepadBitMask[num3])
							{
								break;
							}
							num3++;
						}
						return false;
					}
					IL_1A1:
					BitArray peripheralBitMask3 = this.PeripheralBitMask;
					num2 = ((peripheralBitMask3 != null) ? new int?(peripheralBitMask3.Length) : null);
					BitArray peripheralBitMask4 = other.PeripheralBitMask;
					num = ((peripheralBitMask4 != null) ? new int?(peripheralBitMask4.Length) : null);
					if ((num2.GetValueOrDefault() == num.GetValueOrDefault()) & (num2 != null == (num != null)))
					{
						int num5 = 0;
						for (;;)
						{
							int num6 = num5;
							BitArray peripheralBitMask5 = this.PeripheralBitMask;
							num = ((peripheralBitMask5 != null) ? new int?(peripheralBitMask5.Length) : null);
							if (!((num6 < num.GetValueOrDefault()) & (num != null)))
							{
								return true;
							}
							if (this.PeripheralBitMask[num5] != other.PeripheralBitMask[num5])
							{
								break;
							}
							num5++;
						}
						return false;
					}
					return true;
				}
			}
			return false;
		}

		public byte ControllerFamilyIndex;

		public ControllerFamily ControllerFamily;

		[CanBeNull]
		public BitArray GamepadBitMask;

		[CanBeNull]
		public BitArray PeripheralBitMask;
	}
}
