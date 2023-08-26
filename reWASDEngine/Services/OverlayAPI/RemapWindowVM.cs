using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Infrastructure.KeyBindingsModel;
using reWASDEngine.OverlayAPI.RemapWindow;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Infrastructure.KeyBindingsModel.ControllerBindings;
using XBEliteWPF.Infrastructure.KeyBindingsModel.Mask;
using XBEliteWPF.Infrastructure.KeyBindingsModel.XB;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtil;
using XBEliteWPF.Utils.XBUtilModel;

namespace reWASDEngine.Services.OverlayAPI
{
	public class RemapWindowVM : ZBindableBase
	{
		public bool IsLabelMode { get; set; }

		public int ButtonTableMaxHeight { get; set; }

		public double ButtonTableMaxWidth { get; set; }

		private int MaxVisibleButtons
		{
			get
			{
				return this.ButtonTableMaxHeight / 22;
			}
		}

		public HorizontalAlignment AlignmentSettings
		{
			get
			{
				return this._alignment;
			}
			set
			{
				this._alignment = value;
				this.OnPropertyChanged("Alignment");
			}
		}

		public Orientation TablesOrientation
		{
			get
			{
				return this._tablesOrientation;
			}
			set
			{
				this.SetProperty<Orientation>(ref this._tablesOrientation, value, "TablesOrientation");
			}
		}

		public Visibility HeaderVisibility
		{
			get
			{
				return this._headerVisibility;
			}
			set
			{
				this.SetProperty<Visibility>(ref this._headerVisibility, value, "HeaderVisibility");
			}
		}

		public string ToCloseString
		{
			get
			{
				this.OnPropertyChanged("HintAutomationId");
				if (!this.ShouldShowNextItems())
				{
					return DTLocalization.GetString(12164);
				}
				if (!this.IsLabelMode)
				{
					return DTLocalization.GetString(12167);
				}
				return DTLocalization.GetString(12202);
			}
			set
			{
			}
		}

		public List<SubConfigDataVM> SubConfigs
		{
			get
			{
				List<SubConfigDataVM> list;
				if ((list = this._subConfigs) == null)
				{
					list = (this._subConfigs = new List<SubConfigDataVM>());
				}
				return list;
			}
		}

		public float Transparent
		{
			get
			{
				return this._transparent;
			}
			set
			{
				this.SetProperty<float>(ref this._transparent, value, "Transparent");
			}
		}

		public double Scale
		{
			get
			{
				if (this.Style != CreationRemapStyle.NormalCreation)
				{
					return 1.0;
				}
				return Engine.UserSettingsService.RemapWidowScale;
			}
		}

		public HotkeysInfo HotKeyButtons
		{
			get
			{
				return this._gamepadHotkeysInfo;
			}
			set
			{
				this.SetProperty<HotkeysInfo>(ref this._gamepadHotkeysInfo, value, "HotKeyButtons");
			}
		}

		public bool NewLine
		{
			get
			{
				return this.HotKeyButtons.IsOnlyOneGroup();
			}
		}

		public void FillEnd()
		{
			this.OnPropertyChanged("NewLine");
		}

		private void FillMainSlot(SubConfigDataVM subConfigDataVM, MaskItem maskItem)
		{
			RemapDataItem remapDataItem = new RemapDataItem();
			remapDataItem.HostObject = subConfigDataVM;
			remapDataItem.Btn = this.GetButton(maskItem.XBBinding);
			remapDataItem.IsLabelMode = this.IsLabelMode;
			remapDataItem.XbBindingMain = maskItem.XBBinding;
			remapDataItem.Msk = maskItem;
			if (!this.IsLabelMode || maskItem.XBBinding.HasDescription)
			{
				subConfigDataVM.Buttons.Add(remapDataItem);
			}
		}

		private void FillShift(SubConfigDataVM subConfigDataVM, MaskItem maskItem, int shift)
		{
			AssociatedControllerButton button = this.GetButton(maskItem.XBBinding);
			bool flag = false;
			IEnumerable<RemapDataItem> buttons = subConfigDataVM.Buttons;
			Func<RemapDataItem, bool> <>9__0;
			Func<RemapDataItem, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (RemapDataItem item) => RemapWindowVM.IsEqualsMasks(maskItem, item.Msk));
			}
			using (IEnumerator<RemapDataItem> enumerator = buttons.Where(func).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					RemapDataItem remapDataItem = enumerator.Current;
					this.SetShiftBinding(remapDataItem, maskItem.XBBinding, shift);
					flag = true;
				}
			}
			if (flag)
			{
				return;
			}
			RemapDataItem remapDataItem2 = new RemapDataItem();
			remapDataItem2.HostObject = subConfigDataVM;
			remapDataItem2.IsLabelMode = this.IsLabelMode;
			remapDataItem2.Btn = button;
			remapDataItem2.Msk = maskItem;
			this.SetShiftBinding(remapDataItem2, maskItem.XBBinding, shift);
			if (!this.IsLabelMode || maskItem.XBBinding.HasDescription)
			{
				subConfigDataVM.Buttons.Add(remapDataItem2);
			}
		}

		private void AddXBBindingItem(SubConfigDataVM subConfigDataVM, MaskItem maskItem, int shift)
		{
			if (shift == 0)
			{
				this.FillMainSlot(subConfigDataVM, maskItem);
				return;
			}
			this.FillShift(subConfigDataVM, maskItem, shift);
		}

		private static bool IsEqualsMasks(MaskItem firstItem, MaskItem secondItem)
		{
			if (firstItem.MaskConditions.RelevantItemsCount != secondItem.MaskConditions.RelevantItemsCount)
			{
				return false;
			}
			int num = 0;
			foreach (MaskItemCondition maskItemCondition in firstItem.MaskConditions)
			{
				foreach (MaskItemCondition maskItemCondition2 in secondItem.MaskConditions)
				{
					if (maskItemCondition.ControllerButton.IsKeyScanCode && maskItemCondition2.ControllerButton.IsKeyScanCode)
					{
						if (maskItemCondition.ControllerButton.KeyScanCode.Equals(maskItemCondition2.ControllerButton.KeyScanCode))
						{
							num++;
							break;
						}
					}
					else if (maskItemCondition.ControllerButton.IsGamepad && maskItemCondition2.ControllerButton.IsGamepad && maskItemCondition.ControllerButton.GamepadButton.Equals(maskItemCondition2.ControllerButton.GamepadButton))
					{
						num++;
						break;
					}
				}
			}
			return num == firstItem.MaskConditions.RelevantItemsCount;
		}

		private static bool IsCurrentSubConfig(MaskItem maskItem, SubConfigDataVM subConfigDataVM)
		{
			return (int)maskItem.MaskConditions[0].ControllerFamilyIndex == subConfigDataVM.ControllerFamilyIndex && maskItem.MaskConditions[0].ControllerFamily == subConfigDataVM.ControllerFamily;
		}

		private void FillCollection(SubConfigDataVM subConfigDataVM, BaseXBBindingCollection collection, int shift)
		{
			if (collection.MaskBindingCollection.Count((MaskItem item) => RemapWindowVM.IsCurrentSubConfig(item, subConfigDataVM)) == 0)
			{
				return;
			}
			IEnumerable<MaskItem> maskBindingCollection = collection.MaskBindingCollection;
			Func<MaskItem, bool> <>9__1;
			Func<MaskItem, bool> func;
			if ((func = <>9__1) == null)
			{
				func = (<>9__1 = (MaskItem item) => RemapWindowVM.IsCurrentSubConfig(item, subConfigDataVM));
			}
			foreach (MaskItem maskItem in maskBindingCollection.Where(func))
			{
				this.AddXBBindingItem(subConfigDataVM, maskItem, shift);
			}
		}

		public void AddMaskInfo(SubConfigData subConfig, ControllerTypeEnum? controllerTypeEnum, int controllerFamilyIndex)
		{
			SubConfigDataVM subConfigDataVM = new SubConfigDataVM();
			subConfigDataVM.IsMaskItems = true;
			subConfigDataVM.ControllerFamilyIndex = controllerFamilyIndex;
			subConfigDataVM.ControllerFamily = subConfig.ControllerFamily;
			subConfigDataVM.ControllerType = controllerTypeEnum;
			this.FillCollection(subConfigDataVM, subConfig.MainXBBindingCollection, 0);
			int num = 1;
			foreach (ShiftXBBindingCollection shiftXBBindingCollection in subConfig.MainXBBindingCollection.ShiftXBBindingCollections)
			{
				this.FillCollection(subConfigDataVM, shiftXBBindingCollection, num);
				num++;
			}
			subConfigDataVM.CalcMaskColumnVisible();
			if (subConfigDataVM.Buttons.Count != 0)
			{
				this.SubConfigs.Add(subConfigDataVM);
			}
		}

		private void FillFlydigiApex2NativeAdditionalStick(SubConfigDataVM subConfigDataVM, BaseXBBindingCollection collection, ControllerTypeEnum? controllerTypeEnum, int shift)
		{
			ControllerTypeEnum? controllerType = subConfigDataVM.ControllerType;
			ControllerTypeEnum controllerTypeEnum2 = 38;
			if (((controllerType.GetValueOrDefault() == controllerTypeEnum2) & (controllerType != null)) && collection.SubConfigData.MainXBBindingCollection.AdditionalStickDirectionalGroup.IsNativeMode)
			{
				foreach (XBBinding xbbinding in collection)
				{
					if (GamepadButtonExtensions.IsRightStickDirection(xbbinding.ControllerButton.GamepadButton) && ((xbbinding.IsAnnotationShouldBeShownForMapping(controllerTypeEnum) && !this.IsLabelMode) || (xbbinding.HasDescription && this.IsLabelMode)))
					{
						XBBinding xbbinding2 = xbbinding.Clone();
						bool isRemaped = xbbinding2.IsRemaped;
						xbbinding2.ControllerButton = new AssociatedControllerButton(GamepadButtonExtensions.ConvertRightStickDirectionToAdditionalStick(xbbinding.ControllerButton.GamepadButton));
						if (!isRemaped)
						{
							xbbinding2.RemapedTo = xbbinding2.ControllerButton.GamepadButtonDescription;
						}
						this.AddXBBindingItem(subConfigDataVM, xbbinding2, shift);
					}
				}
			}
		}

		private void FillCollection(SubConfigDataVM subConfigDataVM, List<GamepadButton> gamepadButtons, BaseXBBindingCollection collection, ControllerTypeEnum? controllerTypeEnum, int shift)
		{
			using (List<XBBinding>.Enumerator enumerator = collection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					XBBinding item2 = enumerator.Current;
					if (((item2.IsAnnotationShouldBeShownForMapping(controllerTypeEnum) && !this.IsLabelMode) || (item2.HasDescription && this.IsLabelMode)) && gamepadButtons.Any((GamepadButton button) => button == item2.GamepadButton))
					{
						this.AddXBBindingItem(subConfigDataVM, item2, shift);
					}
				}
			}
			this.FillFlydigiApex2NativeAdditionalStick(subConfigDataVM, collection, controllerTypeEnum, shift);
			this.FillAdaptiveTrigger(subConfigDataVM, 51, collection.AdaptiveLeftTriggerSettings, shift);
			this.FillAdaptiveTrigger(subConfigDataVM, 55, collection.AdaptiveRightTriggerSettings, shift);
			foreach (ControllerBinding controllerBinding in collection.ControllerBindings)
			{
				if ((controllerBinding.XBBinding.HasDescription && this.IsLabelMode) || (controllerBinding.XBBinding.IsAnnotationShouldBeShownForMapping(controllerTypeEnum) && !this.IsLabelMode))
				{
					this.AddXBBindingItem(subConfigDataVM, controllerBinding.XBBinding, shift);
				}
			}
			ControllerTypeEnum controllerTypeEnum2 = ((subConfigDataVM.ControllerType != null) ? subConfigDataVM.ControllerType.Value : 3);
			if (collection.SubConfigData.ConfigData.IsVirtualGamepadMappingPresent() && shift == 0)
			{
				List<XBBinding> fictiveButtons = XBUtils.GetFictiveButtons(collection.SubConfigData, controllerTypeEnum2);
				if (fictiveButtons != null)
				{
					using (List<XBBinding>.Enumerator enumerator = fictiveButtons.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							XBBinding fictiveButton = enumerator.Current;
							if (!subConfigDataVM.Buttons.Any((RemapDataItem item) => item.IsBindingExist(shift) && item.Btn.IsGamepad && item.Btn.GamepadButton == fictiveButton.GamepadButton))
							{
								this.AddXBBindingItem(subConfigDataVM, fictiveButton, shift);
							}
						}
					}
				}
			}
		}

		public void AddXBBindingItem(SubConfigDataVM subConfigDataVM, XBBinding binding, int shift)
		{
			if (shift == 0)
			{
				this.FillMainSlot(subConfigDataVM, binding);
				return;
			}
			this.FillShift(subConfigDataVM, binding, shift);
		}

		public void AddDeviceInfo(SubConfigData subConfig, ControllerTypeEnum? controllerTypeEnum, int controllerFamilyIndex)
		{
			SubConfigDataVM subConfigDataVM = new SubConfigDataVM();
			List<GamepadButton> list = XBUtils.CreatePosibleButtonsCollectionForController(controllerTypeEnum);
			subConfigDataVM.ControllerFamilyIndex = controllerFamilyIndex;
			subConfigDataVM.ControllerFamily = subConfig.ControllerFamily;
			subConfigDataVM.ControllerType = controllerTypeEnum;
			this.FillCollection(subConfigDataVM, list, subConfig.MainXBBindingCollection, controllerTypeEnum, 0);
			int num = 1;
			foreach (ShiftXBBindingCollection shiftXBBindingCollection in subConfig.MainXBBindingCollection.ShiftXBBindingCollections)
			{
				this.FillCollection(subConfigDataVM, list, shiftXBBindingCollection, controllerTypeEnum, num);
				num++;
			}
			subConfigDataVM.CalcColumnVisible();
			if (!this.IsLabelMode)
			{
				subConfigDataVM.FillInheritedItems();
			}
			subConfigDataVM.MaxVisibleButtons = this.MaxVisibleButtons;
			subConfigDataVM.RestrictVisibleButtons();
			if (subConfigDataVM.Buttons.Count != 0)
			{
				this.SubConfigs.Add(subConfigDataVM);
			}
		}

		public void FillShift(SubConfigDataVM subConfigDataVM, XBBinding binding, int shift)
		{
			AssociatedControllerButton button = this.GetButton(binding);
			bool flag = false;
			foreach (RemapDataItem remapDataItem in subConfigDataVM.Buttons)
			{
				if (button.IsAssociatedSetToEqualButtons(remapDataItem.Btn))
				{
					this.SetShiftBinding(remapDataItem, binding, shift);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				RemapDataItem remapDataItem2 = new RemapDataItem();
				remapDataItem2.HostObject = subConfigDataVM;
				remapDataItem2.IsLabelMode = this.IsLabelMode;
				remapDataItem2.Btn = button;
				this.SetShiftBinding(remapDataItem2, binding, shift);
				if (!this.IsLabelMode || binding.HasDescription)
				{
					subConfigDataVM.Buttons.Add(remapDataItem2);
				}
			}
		}

		public void FillMainSlot(SubConfigDataVM subConfigDataVM, XBBinding binding)
		{
			RemapDataItem remapDataItem = new RemapDataItem();
			remapDataItem.HostObject = subConfigDataVM;
			remapDataItem.Btn = this.GetButton(binding);
			remapDataItem.IsLabelMode = this.IsLabelMode;
			remapDataItem.XbBindingMain = binding;
			if (!this.IsLabelMode || binding.HasDescription)
			{
				subConfigDataVM.Buttons.Add(remapDataItem);
			}
		}

		public void FillAdaptiveTrigger(SubConfigDataVM subConfigDataVM, GamepadButton gamepadButton, AdaptiveTriggerSettings triggerSettings, int shift)
		{
			ControllerTypeEnum? controllerTypeEnum;
			if (triggerSettings == null || (subConfigDataVM.ControllerType != null && !ControllerTypeExtensions.IsAnySonyDualSense(controllerTypeEnum.GetValueOrDefault())) || triggerSettings.Preset == null || triggerSettings.Preset == 2)
			{
				return;
			}
			RemapDataItem remapDataItem = subConfigDataVM.Buttons.FirstOrDefault((RemapDataItem ri) => ri.Btn.GamepadButton == gamepadButton);
			if (remapDataItem == null)
			{
				remapDataItem = new RemapDataItem();
				remapDataItem.HostObject = subConfigDataVM;
				remapDataItem.Btn = new AssociatedControllerButton(gamepadButton);
				remapDataItem.IsLabelMode = this.IsLabelMode;
				if (!this.IsLabelMode)
				{
					this.AddAdaptiveTrigger(subConfigDataVM, remapDataItem);
				}
			}
			remapDataItem.AdaptiveTrigger[shift] = triggerSettings.Preset;
		}

		private void AddAdaptiveTrigger(SubConfigDataVM subConfigDataVM, RemapDataItem item)
		{
			GamepadButton otherTrigger = ((item.Btn.GamepadButton == 51) ? 55 : 51);
			int num = subConfigDataVM.Buttons.FindIndex((RemapDataItem ri) => ri.Btn.GamepadButton == otherTrigger);
			if (num != -1)
			{
				subConfigDataVM.Buttons.Insert(num, item);
				return;
			}
			subConfigDataVM.Buttons.Add(item);
		}

		public void SetShiftBinding(RemapDataItem item, XBBinding binding, int shift)
		{
			switch (shift)
			{
			case 1:
				item.XbBindingShift1 = binding;
				return;
			case 2:
				item.XbBindingShift2 = binding;
				return;
			case 3:
				item.XbBindingShift3 = binding;
				return;
			case 4:
				item.XbBindingShift4 = binding;
				return;
			case 5:
				item.XbBindingShift5 = binding;
				return;
			case 6:
				item.XbBindingShift6 = binding;
				return;
			case 7:
				item.XbBindingShift7 = binding;
				return;
			case 8:
				item.XbBindingShift8 = binding;
				return;
			case 9:
				item.XbBindingShift9 = binding;
				return;
			case 10:
				item.XbBindingShift10 = binding;
				return;
			case 11:
				item.XbBindingShift11 = binding;
				return;
			case 12:
				item.XbBindingShift12 = binding;
				return;
			default:
				return;
			}
		}

		public AssociatedControllerButton GetButton(XBBinding binding)
		{
			AssociatedControllerButton associatedControllerButton;
			if (binding.GamepadButton != 2001)
			{
				associatedControllerButton = new AssociatedControllerButton(binding.GamepadButton);
			}
			else
			{
				associatedControllerButton = new AssociatedControllerButton(binding.KeyScanCode);
			}
			return associatedControllerButton;
		}

		public bool ShouldShowNextItems()
		{
			return this.SubConfigs.Any((SubConfigDataVM conf) => conf.HiddenButtons.Count > 0);
		}

		public string HintAutomationId
		{
			get
			{
				if (!this.ShouldShowNextItems())
				{
					return "CloseGamepadWindow";
				}
				return "ShowNextGamepadWindow";
			}
		}

		public void ShowNextVisibleItems()
		{
			this.SubConfigs.ForEach(delegate(SubConfigDataVM conf)
			{
				conf.NextVisibleButtons();
			});
			this.OnPropertyChanged("ToCloseString");
		}

		public string ID;

		public ConfigData configData;

		public List<SubConfigDataVM> _subConfigs;

		private const int BUTTON_ROW_HEIGHT = 22;

		private HorizontalAlignment _alignment;

		private Orientation _tablesOrientation;

		private Visibility _headerVisibility;

		private float _transparent;

		public CreationRemapStyle Style;

		private HotkeysInfo _gamepadHotkeysInfo;
	}
}
