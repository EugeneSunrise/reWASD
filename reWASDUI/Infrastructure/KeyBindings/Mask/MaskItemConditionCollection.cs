using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DiscSoft.NET.Common.Utils.Clases;
using Newtonsoft.Json;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.Controller;
using XBEliteWPF.Infrastructure.KeyBindingsModel.Mask;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtilModel;

namespace reWASDUI.Infrastructure.KeyBindings.Mask
{
	[JsonObject(1)]
	public class MaskItemConditionCollection : SortableObservableCollection<MaskItemCondition>, IDisposable
	{
		private MaskItem HostItem { get; set; }

		public MaskItemBitMaskWrapper MaskItemBitMaskWrapper
		{
			get
			{
				return this._maskItemBitMaskWrapper;
			}
		}

		public void RecalculateBitMask()
		{
			this._maskItemBitMaskWrapper = this.GenerateBitMaskForMaskCollection(this.GetRelevantItemsCollection());
		}

		public int RelevantItemsCount
		{
			get
			{
				return this.GetRelevantItemsCollection().Count<MaskItemCondition>();
			}
		}

		public bool IsDuplicateExist
		{
			get
			{
				return (from x in this.GetRelevantItemsCollection()
					group x by x into @group
					where @group.Count<MaskItemCondition>() > 1
					select @group.Key).ToList<MaskItemCondition>().Count > 0;
			}
		}

		public MaskItemConditionCollection(MaskItem hostItem)
			: base(true)
		{
			if (hostItem != null)
			{
				this.HostItem = hostItem;
			}
			this.FillDefault(4, 0);
		}

		public MaskItemConditionCollection(MaskItem hostItem, BaseControllerVM controller)
			: base(true)
		{
			if (hostItem != null)
			{
				this.HostItem = hostItem;
			}
			CompositeControllerVM compositeControllerVM = controller as CompositeControllerVM;
			if (compositeControllerVM != null)
			{
				controller = compositeControllerVM.CurrentController;
			}
			if (controller != null)
			{
				this.FillDefault(controller.ControllerFamily, 0);
				return;
			}
			this.FillDefault(4, 0);
		}

		public void Dispose()
		{
			this.HostItem = null;
			foreach (MaskItemCondition maskItemCondition in base.Items)
			{
				maskItemCondition.Dispose();
			}
		}

		private void FillDefault(ControllerFamily controllerFamily = 4, byte controllerFamilyIndex = 0)
		{
			if (ControllerFamilyExtensions.IsGamepad(controllerFamily))
			{
				base.Add(new MaskItemCondition(controllerFamilyIndex, 2001));
				base.Add(new MaskItemCondition(controllerFamilyIndex, 2001));
				base.Add(new MaskItemCondition(controllerFamilyIndex, 2001));
				base.Add(new MaskItemCondition(controllerFamilyIndex, 2001));
				return;
			}
			if (ControllerFamilyExtensions.IsKeyboardOrMouse(controllerFamily))
			{
				base.Add(new MaskItemCondition(controllerFamilyIndex, KeyScanCodeV2.NoMap, controllerFamily));
				base.Add(new MaskItemCondition(controllerFamilyIndex, KeyScanCodeV2.NoMap, controllerFamily));
				base.Add(new MaskItemCondition(controllerFamilyIndex, KeyScanCodeV2.NoMap, controllerFamily));
				base.Add(new MaskItemCondition(controllerFamilyIndex, KeyScanCodeV2.NoMap, controllerFamily));
				return;
			}
			base.Add(new MaskItemCondition());
			base.Add(new MaskItemCondition());
			base.Add(new MaskItemCondition());
			base.Add(new MaskItemCondition());
		}

		public bool Matches(IEnumerable<MaskItemCondition> collectionToMatch)
		{
			HashSet<MaskItemCondition> hashSet = new HashSet<MaskItemCondition>(this.GetRelevantItemsCollection());
			HashSet<MaskItemCondition> hashSet2 = new HashSet<MaskItemCondition>(collectionToMatch);
			return hashSet.SetEquals(hashSet2);
		}

		public bool Matches(MaskItemConditionCollection collectionToMatch)
		{
			HashSet<MaskItemCondition> hashSet = new HashSet<MaskItemCondition>(this.GetRelevantItemsCollection());
			HashSet<MaskItemCondition> hashSet2 = new HashSet<MaskItemCondition>(collectionToMatch.GetRelevantItemsCollection());
			return hashSet.SetEquals(hashSet2);
		}

		public IEnumerable<MaskItemCondition> GetRelevantItemsCollection()
		{
			return this.Where((MaskItemCondition c) => (c.GamepadButton != 2000 && c.GamepadButton != 2001 && c.GamepadButton != null) || (c.KeyScanCode != KeyScanCodeV2.NoMap && c.KeyScanCode.IsCategoryAllKeyboardTypesOrMouseDigital));
		}

		public IEnumerable<MaskItemCondition> GetRelevantKeyboardItemsCollection()
		{
			return this.Where((MaskItemCondition c) => c.KeyScanCode != KeyScanCodeV2.NoMap && c.KeyScanCode.IsCategoryAllKeyboardTypes);
		}

		public bool IsKeyboardPresent()
		{
			return this.GetRelevantItemsCollection().Any((MaskItemCondition item) => item.KeyScanCode.IsCategoryAllKeyboardTypes);
		}

		public bool IsMouseDigitalPresent()
		{
			return this.GetRelevantItemsCollection().Any((MaskItemCondition item) => item.KeyScanCode.IsCategoryMouseDigital);
		}

		public IEnumerable<MaskItemCondition> GetRelevantMouseDigitalItemsCollection()
		{
			return this.Where((MaskItemCondition c) => c.KeyScanCode != KeyScanCodeV2.NoMap && c.KeyScanCode.IsCategoryMouseDigital);
		}

		public IEnumerable<MaskItemCondition> GetRelevantKeyboardItemsCollection(byte controllerFamilyIndex, KeyScanCodeCategory keyScanCodeCategory)
		{
			return this.Where((MaskItemCondition c) => c.KeyScanCode != KeyScanCodeV2.NoMap && c.KeyScanCode.KeyScanCodeCategory == keyScanCodeCategory && c.ControllerFamilyIndex == controllerFamilyIndex);
		}

		public IEnumerable<MaskItemCondition> GetRelevantGamepadItemsCollection()
		{
			return this.Where((MaskItemCondition c) => c.GamepadButton != 2000 && c.GamepadButton != 2001 && c.GamepadButton > 0);
		}

		public IEnumerable<MaskItemCondition> GetRelevantGamepadItemsCollection(byte controllerFamilyIndex)
		{
			return this.Where((MaskItemCondition c) => c.GamepadButton != 2000 && c.GamepadButton != 2001 && c.GamepadButton != null && c.ControllerFamilyIndex == controllerFamilyIndex);
		}

		public bool IsGamepadButtonPresent(GamepadButton button)
		{
			return this.GetRelevantItemsCollection().Any((MaskItemCondition item) => item.GamepadButton == button);
		}

		public bool IsKeyScanCodePresent(KeyScanCodeV2 button)
		{
			return this.GetRelevantItemsCollection().Any((MaskItemCondition item) => item.KeyScanCode == button);
		}

		public bool IsZoneOrDirectionsPresent()
		{
			return this.GetRelevantItemsCollection().Any((MaskItemCondition item) => GamepadButtonExtensions.IsAnyZoneOrDirections(item.GamepadButton));
		}

		public bool IsLeftStickDirectionsPresent()
		{
			return this.GetRelevantItemsCollection().Any((MaskItemCondition item) => GamepadButtonExtensions.IsLeftStickDirection(item.GamepadButton));
		}

		public bool IsRightStickDirectionsPresent()
		{
			return this.GetRelevantItemsCollection().Any((MaskItemCondition item) => GamepadButtonExtensions.IsRightStickDirection(item.GamepadButton));
		}

		public bool IsValid()
		{
			return this.RelevantItemsCount > 1 && !this.IsDuplicateExist;
		}

		public override string ToString()
		{
			string text = string.Empty;
			for (int i = 0; i < base.Count; i++)
			{
				MaskItemCondition maskItemCondition = base[i];
				if (maskItemCondition.GamepadButton != 2001)
				{
					if (i > 0)
					{
						text += " + ";
					}
					string text2 = text;
					GamepadButton gamepadButton = maskItemCondition.GamepadButton;
					BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
					ControllerTypeEnum? controllerTypeEnum;
					if (currentGamepad == null)
					{
						controllerTypeEnum = null;
					}
					else
					{
						ControllerVM currentController = currentGamepad.CurrentController;
						controllerTypeEnum = ((currentController != null) ? new ControllerTypeEnum?(currentController.FirstControllerType) : null);
					}
					text = text2 + XBUtils.ConvertGamepadButtonToDescription(gamepadButton, controllerTypeEnum);
				}
				if (maskItemCondition.KeyScanCode != KeyScanCodeV2.NoMap)
				{
					if (i > 0)
					{
						text += " + ";
					}
					text += maskItemCondition.KeyScanCode.FriendlyName;
				}
			}
			return text;
		}

		private MaskItemBitMaskWrapper GenerateBitMaskForMaskCollection(IEnumerable<MaskItemCondition> buttons)
		{
			if (buttons == null)
			{
				return null;
			}
			MaskItemBitMaskWrapper maskItemBitMaskWrapper = new MaskItemBitMaskWrapper();
			foreach (MaskItemCondition maskItemCondition in buttons.Where((MaskItemCondition b) => ControllerFamilyExtensions.IsGamepad(b.ControllerFamily) || ControllerFamilyExtensions.IsKeyboardOrMouse(b.ControllerFamily)))
			{
				if (maskItemCondition.GamepadButton != null && maskItemCondition.GamepadButton != 2001 && maskItemCondition.GamepadButton != 2000 && maskItemCondition.GamepadButton < 248)
				{
					MaskItemBitMask? maskItemData = maskItemBitMaskWrapper.GetMaskItemData(maskItemCondition.ControllerFamily, maskItemCondition.ControllerFamilyIndex, true);
					if (maskItemData != null)
					{
						BitArray gamepadBitMask = maskItemData.GetValueOrDefault().GamepadBitMask;
						if (gamepadBitMask != null)
						{
							gamepadBitMask.Set(maskItemCondition.GamepadButton, true);
						}
					}
				}
				if (maskItemCondition.KeyScanCode != KeyScanCodeV2.NoMap && KeyScanCodeV2.GetIndex(maskItemCondition.KeyScanCode) < 256)
				{
					MaskItemBitMask? maskItemData2 = maskItemBitMaskWrapper.GetMaskItemData(maskItemCondition.ControllerFamily, maskItemCondition.ControllerFamilyIndex, true);
					if (maskItemData2 != null)
					{
						BitArray peripheralBitMask = maskItemData2.GetValueOrDefault().PeripheralBitMask;
						if (peripheralBitMask != null)
						{
							peripheralBitMask.Set(KeyScanCodeV2.GetIndex(maskItemCondition.KeyScanCode), true);
						}
					}
				}
			}
			return maskItemBitMaskWrapper;
		}

		public void Clone(MaskItemConditionCollection source)
		{
			base.Clear();
			foreach (MaskItemCondition maskItemCondition in source)
			{
				MaskItemCondition maskItemCondition2 = new MaskItemCondition(maskItemCondition.ControllerFamilyIndex, maskItemCondition.GamepadButton);
				maskItemCondition2.Clone(maskItemCondition);
				base.Add(maskItemCondition2);
			}
		}

		public void CopyFromModel(MaskItemConditionCollection model)
		{
			base.Clear();
			foreach (MaskItemCondition maskItemCondition in model)
			{
				MaskItemCondition maskItemCondition2 = new MaskItemCondition(maskItemCondition.ControllerFamilyIndex, maskItemCondition.GamepadButton);
				maskItemCondition2.CopyFromModel(maskItemCondition);
				base.Add(maskItemCondition2);
			}
		}

		public void CopyToModel(MaskItemConditionCollection model)
		{
			model.Clear();
			foreach (MaskItemCondition maskItemCondition in this)
			{
				MaskItemCondition maskItemCondition2 = new MaskItemCondition(maskItemCondition.ControllerFamilyIndex, maskItemCondition.GamepadButton);
				maskItemCondition.CopyToModel(maskItemCondition2);
				model.Add(maskItemCondition2);
			}
		}

		private MaskItemBitMaskWrapper _maskItemBitMaskWrapper;
	}
}
