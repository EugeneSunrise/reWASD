using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using Newtonsoft.Json;
using Prism.Commands;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings.XB;
using XBEliteWPF.Infrastructure.KeyBindingsModel.Mask;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.ViewModels.Base;

namespace reWASDUI.Infrastructure.KeyBindings.Mask
{
	[JsonObject(1)]
	public class MaskItem : ZBindable
	{
		public XBBinding XBBinding
		{
			get
			{
				return this._xBBinding;
			}
			set
			{
				this.SetProperty<XBBinding>(ref this._xBBinding, value, "XBBinding");
			}
		}

		public int MaskId
		{
			get
			{
				return this._maskId;
			}
			set
			{
				this.SetProperty<int>(ref this._maskId, value, "MaskId");
			}
		}

		public MaskItemConditionCollection MaskConditions { get; }

		public MaskItemCollection HostCollection { get; set; }

		public bool IsEditMode
		{
			get
			{
				return this._isEditMode;
			}
			set
			{
				this.SetProperty<bool>(ref this._isEditMode, value, "IsEditMode");
			}
		}

		public bool IsMaskConditionsValid
		{
			get
			{
				return this.MaskConditions.IsValid();
			}
		}

		public DelegateCommand RemoveCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._remove) == null)
				{
					delegateCommand = (this._remove = new DelegateCommand(new Action(this.Remove)));
				}
				return delegateCommand;
			}
		}

		private void Remove()
		{
			this.XBBinding.ResetJumpToShift();
			this.HostCollection.Remove(this);
			if (this.HostCollection.CurrentEditItem != null && this.HostCollection.CurrentEditItem.Equals(this))
			{
				this.HostCollection.CurrentEditItem = null;
			}
		}

		public DelegateCommand EditCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._edit) == null)
				{
					delegateCommand = (this._edit = new DelegateCommand(new Action(this.Edit)));
				}
				return delegateCommand;
			}
		}

		private void Edit()
		{
			this.HostCollection.CurrentEditItem = this;
		}

		public bool IsNotAtLeastTwoButtons
		{
			get
			{
				return this._isNotAtLeastTwoButtons;
			}
			set
			{
				this.SetProperty<bool>(ref this._isNotAtLeastTwoButtons, value, "IsNotAtLeastTwoButtons");
			}
		}

		public bool IsDuplicateToAnother
		{
			get
			{
				return this._isDuplicateToAnother;
			}
			set
			{
				this.SetProperty<bool>(ref this._isDuplicateToAnother, value, "IsDuplicateToAnother");
			}
		}

		public bool IsDuplicateInside
		{
			get
			{
				return this._isDuplicateInside;
			}
			set
			{
				this.SetProperty<bool>(ref this._isDuplicateInside, value, "IsDuplicateInside");
			}
		}

		public bool IsNoMapping
		{
			get
			{
				return this._isNoMapping;
			}
			set
			{
				this.SetProperty<bool>(ref this._isNoMapping, value, "IsNoMapping");
			}
		}

		public bool IsEqualToAnySlotHotkey
		{
			get
			{
				return this._isEqualToAnySlotHotkey;
			}
			set
			{
				this.SetProperty<bool>(ref this._isEqualToAnySlotHotkey, value, "IsEqualToAnySlotHotkey");
			}
		}

		public bool IsEqualToStopMacroHotkey
		{
			get
			{
				return this._isEqualToStopMacroHotkey;
			}
			set
			{
				this.SetProperty<bool>(ref this._isEqualToStopMacroHotkey, value, "IsEqualToStopMacroHotkey");
			}
		}

		public bool IsContainsShiftModifier
		{
			get
			{
				return this._isContainsShiftModifier;
			}
			set
			{
				this.SetProperty<bool>(ref this._isContainsShiftModifier, value, "IsContainsShiftModifier");
			}
		}

		public MaskItem(MaskItemCollection hostCollection, int maskId, BaseControllerVM controller = null)
			: this(hostCollection, controller)
		{
			this.MaskId = maskId;
		}

		public MaskItem(MaskItemCollection hostCollection, BaseControllerVM controller = null)
		{
			this.HostCollection = hostCollection;
			this.MaskConditions = new MaskItemConditionCollection(this, controller);
			this.MaskConditions.CollectionChanged += this.MaskConditionsOnCollectionChanged;
			BaseXBBindingCollection baseCollection = this.GetBaseCollection();
			this.XBBinding = new XBBinding(baseCollection, 2000);
			this.XBBinding.PropertyChangedExtended += this.XBBindingOnPropertyChangedExtended;
			this.XBBinding.HostMaskItem = this;
			this.ValidateInternalConditions();
		}

		public override void Dispose()
		{
			base.Dispose();
			if (this.XBBinding != null)
			{
				this.XBBinding.PropertyChangedExtended -= this.XBBindingOnPropertyChangedExtended;
				this.XBBinding.Dispose();
				this.XBBinding = null;
			}
			if (this.MaskConditions != null)
			{
				this.MaskConditions.CollectionChanged -= this.MaskConditionsOnCollectionChanged;
				this.MaskConditions.Dispose();
			}
			this.HostCollection = null;
		}

		private BaseXBBindingCollection GetBaseCollection()
		{
			BaseXBBindingCollection baseXBBindingCollection = this.HostCollection.ConfigData.FindGamepadCollection(0, false).MainXBBindingCollection;
			MainXBBindingCollection mainXBBindingCollection = baseXBBindingCollection as MainXBBindingCollection;
			if (mainXBBindingCollection != null && this.HostCollection.ShiftIndex != 0)
			{
				baseXBBindingCollection = mainXBBindingCollection.ShiftXBBindingCollections[this.HostCollection.ShiftIndex - 1];
			}
			return baseXBBindingCollection;
		}

		private void Validate()
		{
			this.ValidateExternalConditions();
			this.ValidateInternalConditions();
		}

		private void ValidateExternalConditions()
		{
			this.ValidateShiftModifier();
			this.ValidateSlotHotkeys();
			this.ValidateStopMacro();
		}

		public void ValidateInternalConditions()
		{
			this.IsNotAtLeastTwoButtons = this.MaskConditions.RelevantItemsCount < 2;
			this.IsDuplicateInside = this.MaskConditions.IsDuplicateExist;
			this.IsNoMapping = !this.XBBinding.IsAnyActivatorVirtualMappingPresent;
		}

		private void ValidateShiftModifier()
		{
		}

		private void ValidateSlotHotkeys()
		{
		}

		private void ValidateStopMacro()
		{
			if (this.MaskConditions.MaskItemBitMaskWrapper != null)
			{
				this.IsEqualToStopMacroHotkey = this.MaskConditions.MaskItemBitMaskWrapper.IsGamepadBitMaskExist(this.GenerateBitMaskForGamepadButtons(new List<GamepadButton> { 5, 6, 7, 8 }));
			}
		}

		private void CopyMaskConditionToXBBinding()
		{
			for (int i = 0; i < this.MaskConditions.Count; i++)
			{
				MaskItemCondition maskItemCondition = this.MaskConditions[i];
				if (GamepadButtonExtensions.IsRealButton(maskItemCondition.GamepadButton))
				{
					this.XBBinding.MaskConditions.SetItemAt(i, maskItemCondition.ControllerButton);
				}
				if (!maskItemCondition.KeyScanCode.IsNotMapped)
				{
					this.XBBinding.MaskConditions.SetItemAt(i, maskItemCondition.ControllerButton);
				}
			}
			if (this.XBBinding.MaskConditions.All((AssociatedControllerButton item) => item == null || (item != null && !item.IsSet)))
			{
				this.XBBinding.MaskConditions.Clear();
			}
		}

		private void MaskConditionsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Replace && e.NewStartingIndex == e.OldStartingIndex)
			{
				this.CopyMaskConditionToXBBinding();
				this.XBBinding.HostCollection.SubConfigData.ConfigData.IsChanged = true;
				this.XBBinding.ResetJumpToShift();
			}
			this.Validate();
			this.MaskConditions.RecalculateBitMask();
			this.OnPropertyChanged("MaskConditions");
		}

		private void XBBindingOnPropertyChangedExtended(object sender, PropertyChangedExtendedEventArgs e)
		{
			if (e.PropertyName == "MacroSequenceAnnotation")
			{
				return;
			}
			this.IsNoMapping = !this.XBBinding.IsAnyActivatorVirtualMappingPresent;
			if (e.PropertyName != "CurrentActivatorXBBinding")
			{
				this.XBBinding.HostCollection.IsChanged = true;
			}
			if (e.PropertyName == "JumpToShift")
			{
				this.HostCollection.FilterChanged();
			}
			this.HostCollection.RaiseCollectionItemPropertyChangedExtended(sender, e);
		}

		private BitArray GenerateBitMaskForGamepadButtons(IEnumerable<GamepadButton> buttons)
		{
			BitArray bitArray = new BitArray(35);
			foreach (GamepadButton gamepadButton in buttons)
			{
				if (gamepadButton != null && gamepadButton < 35)
				{
					bitArray.Set(gamepadButton, true);
				}
			}
			return bitArray;
		}

		public void CloneMaskConditionCollection(MaskItemConditionCollection maskConditions)
		{
			this.MaskConditions.Clone(maskConditions);
			this.CopyMaskConditionToXBBinding();
			this.ValidateInternalConditions();
		}

		public void CopyFromModel(MaskItem model)
		{
			this.IsNotAtLeastTwoButtons = model.IsNotAtLeastTwoButtons;
			this.IsDuplicateToAnother = model.IsDuplicateToAnother;
			this.IsDuplicateInside = model.IsDuplicateInside;
			this.IsNoMapping = model.IsNoMapping;
			this.IsEqualToStopMacroHotkey = model.IsEqualToStopMacroHotkey;
			this.MaskConditions.CopyFromModel(model.MaskConditions);
			this.XBBinding.CopyFromModel(model.XBBinding);
			this.CopyMaskConditionToXBBinding();
			this.ValidateInternalConditions();
		}

		public void CopyToModel(MaskItem model)
		{
			model.IsNotAtLeastTwoButtons = this.IsNotAtLeastTwoButtons;
			model.IsDuplicateToAnother = this.IsDuplicateToAnother;
			model.IsDuplicateInside = this.IsDuplicateInside;
			model.IsNoMapping = this.IsNoMapping;
			model.IsEqualToStopMacroHotkey = this.IsEqualToStopMacroHotkey;
			model.MaskId = this.MaskId;
			this.MaskConditions.CopyToModel(model.MaskConditions);
			this.XBBinding.CopyToModel(model.XBBinding);
		}

		private XBBinding _xBBinding;

		private int _maskId = -1;

		private bool _isEditMode;

		private DelegateCommand _remove;

		private DelegateCommand _edit;

		private bool _isNotAtLeastTwoButtons;

		private bool _isDuplicateToAnother;

		private bool _isDuplicateInside;

		private bool _isNoMapping;

		private bool _isEqualToStopMacroHotkey;

		private bool _isEqualToAnySlotHotkey;

		private bool _isContainsShiftModifier;
	}
}
