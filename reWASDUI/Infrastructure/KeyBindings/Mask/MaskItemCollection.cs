using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using Newtonsoft.Json;
using Prism.Commands;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.Controller;
using XBEliteWPF.Infrastructure.KeyBindingsModel.Mask;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Infrastructure.KeyBindings.Mask
{
	[JsonObject(1)]
	public class MaskItemCollection : SortableObservableCollection<MaskItem>, IDisposable
	{
		public ConfigData ConfigData { get; set; }

		public int ShiftIndex { get; set; }

		public new bool SuppressNotification
		{
			get
			{
				return base.SuppressNotification;
			}
			set
			{
				base.SuppressNotification = value;
			}
		}

		public MaskItem CurrentEditItem
		{
			get
			{
				return this._currentEditItem;
			}
			set
			{
				MaskItem currentEditItem = this._currentEditItem;
				if (this.SetProperty(ref this._currentEditItem, value, "CurrentEditItem"))
				{
					if (currentEditItem != null)
					{
						currentEditItem.IsEditMode = false;
					}
					if (this._currentEditItem != null)
					{
						this._currentEditItem.IsEditMode = true;
					}
					this.RaiseCollectionItemPropertyChangedExtended(this, new PropertyChangedExtendedEventArgs("CurrentEditItem", typeof(MaskItem), currentEditItem, value));
				}
			}
		}

		public ObservableCollection<MaskItem> FilteredMasks
		{
			get
			{
				return this._filteredMasks;
			}
		}

		public event PropertyChangedExtendedEventHandler CollectionItemPropertyChangedExtended;

		public BaseControllerVM AssociatedController
		{
			get
			{
				BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
				if (currentGamepad != null && currentGamepad.IsTreatAsSingleDevice)
				{
					return currentGamepad;
				}
				CompositeControllerVM compositeControllerVM = currentGamepad as CompositeControllerVM;
				if (compositeControllerVM != null)
				{
					return this._associatedController ?? compositeControllerVM;
				}
				return this._associatedController;
			}
			set
			{
				if (this.SetProperty(ref this._associatedController, value, "AssociatedController"))
				{
					this.AssociatedControllerButtonContainer.ControllerButton.SetDefaultButtons(false);
					this.FilterChanged();
				}
			}
		}

		public AssociatedControllerButtonContainer AssociatedControllerButtonContainer { get; set; } = new AssociatedControllerButtonContainer();

		public DelegateCommand AddMaskCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._addMask) == null)
				{
					delegateCommand = (this._addMask = new DelegateCommand(new Action(this.AddMask)));
				}
				return delegateCommand;
			}
		}

		private void AddMask()
		{
			BaseControllerVM baseControllerVM = App.GamepadService.CurrentGamepad;
			if (this.AssociatedController != null)
			{
				baseControllerVM = this.AssociatedController;
			}
			CompositeControllerVM compositeControllerVM = baseControllerVM as CompositeControllerVM;
			if (compositeControllerVM != null)
			{
				baseControllerVM = compositeControllerVM.NonNullBaseControllers.FirstOrDefault<BaseControllerVM>();
			}
			if (baseControllerVM == null)
			{
				return;
			}
			MaskItem maskItem = new MaskItem(this, baseControllerVM);
			this.Add(maskItem);
			AssociatedControllerButton controllerButton = this.AssociatedControllerButtonContainer.ControllerButton;
			if (controllerButton.IsGamepad && (baseControllerVM.ControllerFamily == null || baseControllerVM.IsNintendoSwitchJoyConComposite))
			{
				maskItem.MaskConditions[0].GamepadButton = controllerButton.GamepadButton;
				this.FilterChanged();
			}
			else if (controllerButton.IsKeyScanCode && baseControllerVM.ControllerFamily != null)
			{
				maskItem.MaskConditions[0].ControllerFamily = baseControllerVM.ControllerFamily;
				maskItem.MaskConditions[0].KeyScanCode = controllerButton.KeyScanCode;
				this.FilterChanged();
			}
			this.CurrentEditItem = maskItem;
		}

		public MaskItemCollection(ConfigData configData, int shiftIndex)
			: base(true)
		{
			this.ConfigData = configData;
			this.ShiftIndex = shiftIndex;
			this.CollectionChanged += this.OnCollectionChanged;
			this.AssociatedControllerButtonContainer.PropertyChanged += this.AssociatedControllerButtonContainerOnPropertyChanged;
			App.EventAggregator.GetEvent<CurrentGamepadChanged>().Subscribe(delegate(BaseControllerVM item)
			{
				this.FilteredMasks.Clear();
				this.FilterChanged();
			});
		}

		public void Dispose()
		{
			this.ConfigData = null;
			this.CollectionChanged -= this.OnCollectionChanged;
			this.AssociatedControllerButtonContainer.PropertyChanged -= this.AssociatedControllerButtonContainerOnPropertyChanged;
			this.AssociatedController = null;
			this.CollectionItemPropertyChangedExtended = null;
			foreach (MaskItem maskItem in base.Items)
			{
				maskItem.Dispose();
			}
		}

		public void ClearAll()
		{
			this.CurrentEditItem = null;
			this.FilteredMasks.Clear();
			base.Clear();
		}

		public void RemoveEmpty()
		{
			this.CurrentEditItem = null;
			this.SuppressNotification = true;
			this.Remove((MaskItem cb) => cb.XBBinding.IsEmpty);
			this.SuppressNotification = false;
			this.FilteredMasks.Clear();
			this.FilterChanged();
		}

		private void AssociatedControllerButtonContainerOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.FilterChanged();
		}

		public void FilterChanged()
		{
			foreach (MaskItem maskItem in this)
			{
				if (this.Filter(maskItem))
				{
					if (!this.FilteredMasks.Contains(maskItem))
					{
						this.FilteredMasks.Add(maskItem);
					}
				}
				else
				{
					this.FilteredMasks.Remove(maskItem);
				}
			}
			this.SortFilteredMasksAccordingShift();
		}

		private void SortFilteredMasksAccordingShift()
		{
			List<MaskItem> list = this.FilteredMasks.Where((MaskItem item) => item.XBBinding.IsContainsJumpToShift).ToList<MaskItem>();
			list.AddRange(this.FilteredMasks.Where((MaskItem item) => !item.XBBinding.IsContainsJumpToShift));
			for (int i = 0; i < this.FilteredMasks.Count; i++)
			{
				int num = this.FilteredMasks.IndexOf(list[i]);
				if (num != i)
				{
					this.FilteredMasks.Move(num, i);
				}
			}
		}

		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (this._suppressItemPropChangeNotifications || this.SuppressNotification)
			{
				return;
			}
			this._suppressItemPropChangeNotifications = true;
			this.ValidateIsDuplicateToAnotherForChildren();
			this._suppressItemPropChangeNotifications = false;
			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				bool flag = false;
				foreach (object obj in e.OldItems)
				{
					flag |= ((MaskItem)obj).XBBinding.IsAnyActivatorVirtualGamepadMappingPresent;
				}
				if (flag)
				{
					this.ConfigData.CheckVirtualMappingsExist();
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Reset && this.ConfigData.IsVirtualMappingExist)
			{
				this.ConfigData.CheckVirtualMappingsExist();
			}
			if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove)
			{
				this.ConfigData.IsChanged = true;
			}
		}

		public void ValidateIsDuplicateToAnotherForChildren()
		{
			this.ForEach(delegate(MaskItem t)
			{
				t.IsDuplicateToAnother = false;
			});
			List<MaskItemBitMaskWrapper> list = new List<MaskItemBitMaskWrapper>();
			using (IEnumerator<MaskItem> enumerator = this.Select((MaskItem i) => i).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MaskItem maskItem = enumerator.Current;
					if (maskItem.MaskConditions.MaskItemBitMaskWrapper != null)
					{
						if (!list.Exists((MaskItemBitMaskWrapper x) => x.Equals(maskItem.MaskConditions.MaskItemBitMaskWrapper)))
						{
							list.Add(maskItem.MaskConditions.MaskItemBitMaskWrapper);
						}
						else
						{
							maskItem.IsDuplicateToAnother = true;
						}
					}
				}
			}
		}

		public bool Filter(object obj)
		{
			if (this.SuppressNotification)
			{
				return false;
			}
			MaskItem maskItem = obj as MaskItem;
			return maskItem != null && this.FilterByCombobox(maskItem) && this.FilterByControllerType(maskItem);
		}

		private bool FilterByCombobox(MaskItem mi)
		{
			return this.FilterByAssociatedControllerButton(mi, this.AssociatedControllerButtonContainer.ControllerButton, -1);
		}

		private bool FilterByAssociatedControllerButton(MaskItem mi, AssociatedControllerButton acb, int conrtollerFamilyIndex)
		{
			if (!acb.IsSet)
			{
				return true;
			}
			if (acb.IsGamepad)
			{
				return mi.MaskConditions.Any((MaskItemCondition mic) => mic.GamepadButton == acb.GamepadButton && (conrtollerFamilyIndex == -1 || (int)mic.ControllerFamilyIndex == conrtollerFamilyIndex));
			}
			return !acb.IsKeyScanCode || mi.MaskConditions.Any((MaskItemCondition mic) => mic.KeyScanCode == acb.KeyScanCode && (conrtollerFamilyIndex == -1 || (int)mic.ControllerFamilyIndex == conrtollerFamilyIndex));
		}

		private bool FilterByControllerType(MaskItem mi)
		{
			BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
			CompositeControllerVM cc = currentGamepad as CompositeControllerVM;
			bool flag;
			if (cc != null)
			{
				if (cc.IsTreatAsSingleDevice)
				{
					flag = mi.MaskConditions.All(delegate(MaskItemCondition mic)
					{
						ControllerFamily controllerFamily = mic.ControllerFamily;
						BaseControllerVM associatedController3 = this.AssociatedController;
						ControllerFamily? controllerFamily2 = ((associatedController3 != null) ? new ControllerFamily?(associatedController3.TreatAsControllerFamily) : null);
						return ((controllerFamily == controllerFamily2.GetValueOrDefault()) & (controllerFamily2 != null)) || mic.ControllerFamily == 4;
					});
				}
				else
				{
					flag = mi.MaskConditions.All((MaskItemCondition mic) => mic.AssociatedController == null || cc.ControllersForMaskFilter.Contains(mic.AssociatedController));
				}
			}
			else if (this.AssociatedController != null && ControllerFamilyExtensions.IsKeyboardOrMouse(this.AssociatedController.TreatAsControllerFamily))
			{
				bool flag2;
				if (mi.MaskConditions.All((MaskItemCondition mic) => mic.AssociatedController.KeyScanCodesForMask.Contains(mic.KeyScanCode)))
				{
					flag2 = mi.MaskConditions.All((MaskItemCondition mic) => mic.GamepadButton == 2001);
				}
				else
				{
					flag2 = false;
				}
				flag = flag2;
			}
			else
			{
				flag = mi.MaskConditions.All(delegate(MaskItemCondition mic)
				{
					ControllerFamily controllerFamily3 = mic.ControllerFamily;
					BaseControllerVM associatedController4 = this.AssociatedController;
					ControllerFamily? controllerFamily4 = ((associatedController4 != null) ? new ControllerFamily?(associatedController4.TreatAsControllerFamily) : null);
					return (((controllerFamily3 == controllerFamily4.GetValueOrDefault()) & (controllerFamily4 != null)) || mic.ControllerFamily == 4) && mic.ControllerFamilyIndex == 0;
				});
			}
			bool flag3;
			if (!mi.MaskConditions.Any((MaskItemCondition mic) => mic.AssociatedController == this.AssociatedController && !mic.IsNotSelected))
			{
				BaseControllerVM associatedController = this.AssociatedController;
				flag3 = associatedController != null && associatedController.IsCompositeDevice;
			}
			else
			{
				flag3 = true;
			}
			bool flag4 = flag3;
			bool flag5;
			if (!mi.MaskConditions.All((MaskItemCondition mic) => mic.AssociatedController == this.AssociatedController && mic.IsNotSelected))
			{
				BaseControllerVM associatedController2 = this.AssociatedController;
				flag5 = associatedController2 != null && associatedController2.IsCompositeDevice;
			}
			else
			{
				flag5 = true;
			}
			bool flag6 = flag5;
			return flag && (flag4 || flag6);
		}

		public bool IsShouldBeShownForControllerButton(AssociatedControllerButton acb, int ControllerFamilyIndex)
		{
			return acb.IsSet && this.Any((MaskItem mi) => this.FilterByAssociatedControllerButton(mi, acb, ControllerFamilyIndex));
		}

		public new bool Remove(MaskItem item)
		{
			if (base.Contains(item))
			{
				for (int i = item.MaskId; i < base.Count; i++)
				{
					base[i].MaskId--;
				}
				if (this._filteredMasks != this)
				{
					this._filteredMasks.Remove(item);
				}
				return base.Remove(item);
			}
			return false;
		}

		public new void Add(MaskItem item)
		{
			if (item.MaskId == -1)
			{
				item.MaskId = base.Count + 1;
			}
			base.Add(item);
			this.FilterChanged();
		}

		public void RemoveAllWithGamepadButton(GamepadButton gb)
		{
			List<MaskItem> list = new List<MaskItem>();
			Func<MaskItemCondition, bool> <>9__0;
			foreach (MaskItem maskItem in this)
			{
				IEnumerable<MaskItemCondition> maskConditions = maskItem.MaskConditions;
				Func<MaskItemCondition, bool> func;
				if ((func = <>9__0) == null)
				{
					func = (<>9__0 = (MaskItemCondition mc) => mc.GamepadButton == gb);
				}
				if (maskConditions.Any(func))
				{
					list.Add(maskItem);
				}
			}
			foreach (MaskItem maskItem2 in list)
			{
				this.Remove(maskItem2);
			}
			this.FilterChanged();
		}

		public void RaiseCollectionItemPropertyChangedExtended(object s, PropertyChangedExtendedEventArgs e)
		{
			PropertyChangedExtendedEventHandler collectionItemPropertyChangedExtended = this.CollectionItemPropertyChangedExtended;
			if (collectionItemPropertyChangedExtended == null)
			{
				return;
			}
			collectionItemPropertyChangedExtended(s, e);
		}

		public List<KeyScanCodeV2> GetAllUniqueKeyboardKeyScanCodes()
		{
			List<KeyScanCodeV2> keyScanCodeList = new List<KeyScanCodeV2>();
			Func<MaskItemCondition, bool> <>9__0;
			foreach (MaskItem maskItem in this)
			{
				List<MaskItemCondition> list = maskItem.MaskConditions.GetRelevantKeyboardItemsCollection().ToList<MaskItemCondition>();
				if (list.Count != 0)
				{
					IEnumerable<MaskItemCondition> enumerable = list;
					Func<MaskItemCondition, bool> func;
					if ((func = <>9__0) == null)
					{
						func = (<>9__0 = (MaskItemCondition miKeyScanCode) => !keyScanCodeList.Contains(miKeyScanCode.KeyScanCode));
					}
					foreach (MaskItemCondition maskItemCondition in enumerable.Where(func))
					{
						keyScanCodeList.Add(maskItemCondition.KeyScanCode);
					}
				}
			}
			return keyScanCodeList;
		}

		public List<KeyScanCodeV2> GetAllUniqueMouseDigitalKeyScanCodes()
		{
			List<KeyScanCodeV2> keyScanCodeList = new List<KeyScanCodeV2>();
			Func<MaskItemCondition, bool> <>9__0;
			foreach (MaskItem maskItem in this)
			{
				List<MaskItemCondition> list = maskItem.MaskConditions.GetRelevantMouseDigitalItemsCollection().ToList<MaskItemCondition>();
				if (list.Count != 0)
				{
					IEnumerable<MaskItemCondition> enumerable = list;
					Func<MaskItemCondition, bool> func;
					if ((func = <>9__0) == null)
					{
						func = (<>9__0 = (MaskItemCondition miKeyScanCode) => !keyScanCodeList.Contains(miKeyScanCode.KeyScanCode));
					}
					foreach (MaskItemCondition maskItemCondition in enumerable.Where(func))
					{
						keyScanCodeList.Add(maskItemCondition.KeyScanCode);
					}
				}
			}
			return keyScanCodeList;
		}

		public List<KeyScanCodeV2> GetAllUniqueKeyboardKeyScanCodes(byte controllerFamilyIndex, KeyScanCodeCategory keyScanCodeCategory)
		{
			List<KeyScanCodeV2> keyScanCodeList = new List<KeyScanCodeV2>();
			Func<MaskItemCondition, bool> <>9__0;
			foreach (MaskItem maskItem in this)
			{
				List<MaskItemCondition> list = maskItem.MaskConditions.GetRelevantKeyboardItemsCollection(controllerFamilyIndex, keyScanCodeCategory).ToList<MaskItemCondition>();
				if (list.Count != 0)
				{
					IEnumerable<MaskItemCondition> enumerable = list;
					Func<MaskItemCondition, bool> func;
					if ((func = <>9__0) == null)
					{
						func = (<>9__0 = (MaskItemCondition miKeyScanCode) => !keyScanCodeList.Contains(miKeyScanCode.KeyScanCode));
					}
					foreach (MaskItemCondition maskItemCondition in enumerable.Where(func))
					{
						keyScanCodeList.Add(maskItemCondition.KeyScanCode);
					}
				}
			}
			return keyScanCodeList;
		}

		public List<GamepadButton> GetAllUniqueGamepadButtons()
		{
			List<GamepadButton> gamepadButtonList = new List<GamepadButton>();
			Func<MaskItemCondition, bool> <>9__0;
			foreach (MaskItem maskItem in this)
			{
				List<MaskItemCondition> list = maskItem.MaskConditions.GetRelevantGamepadItemsCollection().ToList<MaskItemCondition>();
				if (list.Count != 0)
				{
					IEnumerable<MaskItemCondition> enumerable = list;
					Func<MaskItemCondition, bool> func;
					if ((func = <>9__0) == null)
					{
						func = (<>9__0 = (MaskItemCondition miGamepadButton) => !gamepadButtonList.Contains(miGamepadButton.GamepadButton));
					}
					foreach (MaskItemCondition maskItemCondition in enumerable.Where(func))
					{
						gamepadButtonList.Add(maskItemCondition.GamepadButton);
					}
				}
			}
			return gamepadButtonList;
		}

		public List<GamepadButton> GetAllUniqueGamepadButtons(byte controllerFamilyIndex)
		{
			List<GamepadButton> gamepadButtonList = new List<GamepadButton>();
			Func<MaskItemCondition, bool> <>9__0;
			foreach (MaskItem maskItem in this)
			{
				List<MaskItemCondition> list = maskItem.MaskConditions.GetRelevantGamepadItemsCollection(controllerFamilyIndex).ToList<MaskItemCondition>();
				if (list.Count != 0)
				{
					IEnumerable<MaskItemCondition> enumerable = list;
					Func<MaskItemCondition, bool> func;
					if ((func = <>9__0) == null)
					{
						func = (<>9__0 = (MaskItemCondition miGamepadButton) => !gamepadButtonList.Contains(miGamepadButton.GamepadButton));
					}
					foreach (MaskItemCondition maskItemCondition in enumerable.Where(func))
					{
						gamepadButtonList.Add(maskItemCondition.GamepadButton);
					}
				}
			}
			return gamepadButtonList;
		}

		public bool IsStickDirectionExist(Stick stick)
		{
			if (stick == 1)
			{
				return this.Any((MaskItem mi) => mi.MaskConditions.IsRightStickDirectionsPresent());
			}
			if (stick == null)
			{
				return this.Any((MaskItem mi) => mi.MaskConditions.IsLeftStickDirectionsPresent());
			}
			return false;
		}

		public MaskItem FindSimilarMaskItem(MaskItem maskItem)
		{
			return this.FirstOrDefault((MaskItem item) => item.MaskConditions.Matches(maskItem.MaskConditions));
		}

		public MaskItem AddMaskItemWithMaskCondition(MaskItemConditionCollection maskItemConditionCollection)
		{
			MaskItem maskItem = new MaskItem(this, null);
			maskItem.CloneMaskConditionCollection(maskItemConditionCollection);
			this.Add(maskItem);
			this.FilterChanged();
			return maskItem;
		}

		public void CopyFromModel(MaskItemCollection model, bool isCopyToNotEmpty = true)
		{
			this.SuppressNotification = true;
			if (isCopyToNotEmpty)
			{
				base.Clear();
			}
			foreach (MaskItem maskItem in model)
			{
				MaskItem mi = new MaskItem(this, null);
				mi.CopyFromModel(maskItem);
				if ((isCopyToNotEmpty || (!isCopyToNotEmpty && !this.Any((MaskItem x) => x.MaskConditions.Matches(mi.MaskConditions)))) && !mi.XBBinding.IsEmpty)
				{
					this.Add(mi);
				}
			}
			this.SuppressNotification = false;
			this.FilteredMasks.Clear();
			this.FilterChanged();
		}

		public void CopyToModel(MaskItemCollection model)
		{
			model.Clear();
			model.ShiftModificatorNum = this.ShiftIndex;
			foreach (MaskItem maskItem in this)
			{
				MaskItem maskItem2 = new MaskItem(model, null);
				model.Add(maskItem2);
				maskItem.CopyToModel(maskItem2);
			}
		}

		private MaskItem _currentEditItem;

		private BaseControllerVM _associatedController;

		private bool _suppressItemPropChangeNotifications;

		private ObservableCollection<MaskItem> _filteredMasks = new ObservableCollection<MaskItem>();

		private DelegateCommand _addMask;
	}
}
