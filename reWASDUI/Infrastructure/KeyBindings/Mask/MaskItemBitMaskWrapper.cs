using System;
using System.Collections;
using System.Collections.Generic;
using DiscSoft.NET.Common.Properties;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Infrastructure.KeyBindings.Mask
{
	public class MaskItemBitMaskWrapper
	{
		public List<MaskItemBitMask> MaskItemBitMaskList
		{
			get
			{
				return this._maskItemBitMaskList;
			}
		}

		public MaskItemBitMaskWrapper()
		{
			this._maskItemBitMaskList = new List<MaskItemBitMask>();
		}

		public MaskItemBitMask? GetMaskItemData(ControllerFamily controllerFamily, byte controllerFamilyIndex, bool createOnDemand = true)
		{
			if (this._maskItemBitMaskList.Exists((MaskItemBitMask x) => x.ControllerFamily == controllerFamily && x.ControllerFamilyIndex == controllerFamilyIndex))
			{
				return new MaskItemBitMask?(this._maskItemBitMaskList.Find((MaskItemBitMask x) => x.ControllerFamily == controllerFamily && x.ControllerFamilyIndex == controllerFamilyIndex));
			}
			if (createOnDemand)
			{
				if (ControllerFamilyExtensions.IsGamepad(controllerFamily))
				{
					MaskItemBitMask maskItemBitMask = new MaskItemBitMask(controllerFamily, controllerFamilyIndex, new BitArray(248), null);
					this._maskItemBitMaskList.Add(maskItemBitMask);
					return new MaskItemBitMask?(maskItemBitMask);
				}
				if (ControllerFamilyExtensions.IsKeyboardOrMouse(controllerFamily))
				{
					MaskItemBitMask maskItemBitMask2 = new MaskItemBitMask(controllerFamily, controllerFamilyIndex, null, new BitArray(256));
					this._maskItemBitMaskList.Add(maskItemBitMask2);
					return new MaskItemBitMask?(maskItemBitMask2);
				}
			}
			return null;
		}

		public bool IsGamepadBitMaskExist(BitArray gamepadBitMask)
		{
			return gamepadBitMask != null && this.MaskItemBitMaskList.Exists((MaskItemBitMask x) => x.ControllerFamily == null && x.GamepadBitMask != null && MaskItemBitMaskWrapper.IsBitMaskEquals(x.GamepadBitMask, gamepadBitMask));
		}

		public bool IsPeripheralBitMaskExist(BitArray peripheralBitMask)
		{
			return peripheralBitMask != null && this.MaskItemBitMaskList.Exists((MaskItemBitMask x) => ControllerFamilyExtensions.IsKeyboardOrMouse(x.ControllerFamily) && x.PeripheralBitMask != null && MaskItemBitMaskWrapper.IsBitMaskEquals(x.PeripheralBitMask, peripheralBitMask));
		}

		private static bool IsBitMaskEquals([CanBeNull] BitArray bitMask1, [CanBeNull] BitArray bitMask2)
		{
			if (bitMask1 == null && bitMask2 == null)
			{
				return false;
			}
			if ((bitMask1 == null && bitMask2 != null) || (bitMask1 != null && bitMask2 == null))
			{
				return false;
			}
			if (bitMask1.Length != bitMask2.Length)
			{
				return false;
			}
			for (int i = 0; i < bitMask1.Length; i++)
			{
				if (bitMask1[i] != bitMask2[i])
				{
					return false;
				}
			}
			return true;
		}

		public bool Equals(MaskItemBitMaskWrapper other)
		{
			List<MaskItemBitMask> list = new List<MaskItemBitMask>(other.MaskItemBitMaskList);
			using (List<MaskItemBitMask>.Enumerator enumerator = this.MaskItemBitMaskList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MaskItemBitMask maskItemBitMaskListItem = enumerator.Current;
					if (!list.Exists((MaskItemBitMask x) => x.Equals(maskItemBitMaskListItem)))
					{
						return false;
					}
					list.Remove(list.Find((MaskItemBitMask x) => x.Equals(maskItemBitMaskListItem)));
				}
			}
			return list.Count <= 0;
		}

		private List<MaskItemBitMask> _maskItemBitMaskList;
	}
}
