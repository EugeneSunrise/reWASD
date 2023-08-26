using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Attributes;
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.KeyBindings.ActivatorXB;
using reWASDUI.Infrastructure.KeyBindings.XB;
using XBEliteWPF.Infrastructure.KeyBindingsModel.ControllerBindings;
using XBEliteWPF.Infrastructure.KeyBindingsModel.XB;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;

namespace reWASDUI.Infrastructure.KeyBindings.ControllerBindings
{
	public class ControllerBindingsCollection : SortableObservableCollection<ControllerBinding>
	{
		public BaseXBBindingCollection HostCollection { get; set; }

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

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public ControllerBinding CurrentEditItem
		{
			get
			{
				return this._currentEditItem;
			}
			set
			{
				ControllerBinding currentEditItem = this._currentEditItem;
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
					this.RaiseCollectionItemPropertyChangedExtended(this, new PropertyChangedExtendedEventArgs("CurrentEditItem", typeof(ControllerBinding), currentEditItem, value));
				}
			}
		}

		public ObservableCollection<ControllerButtonTag> AvailiableControllerButtonTags
		{
			get
			{
				ObservableCollection<ControllerButtonTag> observableCollection = new ObservableCollection<ControllerButtonTag>(Enum.GetValues(typeof(ControllerButtonTag)).OfType<ControllerButtonTag>());
				if (this.HostCollection.SubConfigData.ControllerFamily == 2)
				{
					observableCollection.Remove(3);
				}
				return observableCollection;
			}
		}

		public ControllerButtonTag ControllerButtonTag
		{
			get
			{
				return this._controllerButtonTag;
			}
			set
			{
				if (this.SetProperty(ref this._controllerButtonTag, value, "ControllerButtonTag"))
				{
					this.ReFilterItems();
					this.FirePropertyChanged("FilteredKeyScanCodesForeKeyboard");
					this.FirePropertyChanged("FilteredKeyScanCodesForMouse");
					this.FirePropertyChanged("FilteredKeyScanCodesForGamepad");
				}
			}
		}

		public ObservableCollection<KeyScanCodeV2> FilteredKeyScanCodesForeKeyboard
		{
			get
			{
				ObservableCollection<KeyScanCodeV2> observableCollection = new ObservableCollection<KeyScanCodeV2>();
				CollectionExtensions.AddRange<KeyScanCodeV2>(observableCollection, App.KeyBindingService.KeyScanCodeCollectionForKeyboard.Where((KeyScanCodeV2 ksc) => this.ControllerButtonTag == null || ksc.ControllerButtonTags.Contains(0) || ksc.ControllerButtonTags.Contains(this.ControllerButtonTag)));
				return observableCollection;
			}
		}

		public ObservableCollection<KeyScanCodeV2> FilteredKeyScanCodesForMouse
		{
			get
			{
				ObservableCollection<KeyScanCodeV2> observableCollection = new ObservableCollection<KeyScanCodeV2>();
				CollectionExtensions.AddRange<KeyScanCodeV2>(observableCollection, App.KeyBindingService.KeyScanCodeCollectionForMouse.Where((KeyScanCodeV2 ksc) => this.ControllerButtonTag == null || ksc.ControllerButtonTags.Contains(0) || ksc.ControllerButtonTags.Contains(this.ControllerButtonTag)));
				return observableCollection;
			}
		}

		public ObservableCollection<KeyScanCodeV2> FilteredKeyScanCodesForGamepad
		{
			get
			{
				ObservableCollection<KeyScanCodeV2> observableCollection = new ObservableCollection<KeyScanCodeV2>();
				CollectionExtensions.AddRange<KeyScanCodeV2>(observableCollection, App.KeyBindingService.KeyScanCodeCollectionForGamepad.Where((KeyScanCodeV2 ksc) => this.ControllerButtonTag == null || ksc.ControllerButtonTags.Contains(0) || ksc.ControllerButtonTags.Contains(this.ControllerButtonTag)));
				return observableCollection;
			}
		}

		public ObservableCollection<ControllerBinding> FilteredMasks { get; } = new ObservableCollection<ControllerBinding>();

		public void ReFilterItems()
		{
			if (this.SuppressNotification)
			{
				return;
			}
			foreach (ControllerBinding controllerBinding in this)
			{
				if (this.Filter(controllerBinding))
				{
					if (!this.FilteredMasks.Contains(controllerBinding))
					{
						this.FilteredMasks.Add(controllerBinding);
					}
				}
				else
				{
					this.FilteredMasks.Remove(controllerBinding);
				}
			}
			this.SortFilteredMasksAccordingShift();
		}

		private void SortFilteredMasksAccordingShift()
		{
			List<ControllerBinding> list = this.FilteredMasks.Where((ControllerBinding item) => item.XBBinding.IsContainsJumpToShift).ToList<ControllerBinding>();
			list.AddRange(this.FilteredMasks.Where((ControllerBinding item) => !item.XBBinding.IsContainsJumpToShift));
			for (int i = 0; i < this.FilteredMasks.Count; i++)
			{
				int num = this.FilteredMasks.IndexOf(list[i]);
				if (num != i)
				{
					this.FilteredMasks.Move(num, i);
				}
			}
		}

		public bool Contains(KeyScanCodeV2 ksc)
		{
			return this.FirstOrDefault((ControllerBinding cb) => cb.XBBinding.KeyScanCode == ksc) != null;
		}

		public event PropertyChangedExtendedEventHandler CollectionItemPropertyChangedExtended;

		public ControllerBindingsCollection(BaseXBBindingCollection hostCollection, bool createOnDemand = false)
			: base(true)
		{
			this.CreateOnDemand = createOnDemand;
			this.HostCollection = hostCollection;
			this.CollectionChanged += this.OnCollectionChanged;
			this.CollectionItemPropertyChangedExtended += this.OnCollectionItemPropertyChanged;
			base.AfterCollectionChanged += this.OnAfterCollectionChanged;
		}

		private void OnCollectionItemPropertyChanged(object sender, PropertyChangedExtendedEventArgs e)
		{
			if (DSUtils.HasAttributeDefined(sender.GetType(), e.PropertyName, typeof(DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent)))
			{
				return;
			}
			ControllerBinding controllerBinding = null;
			ControllerBinding controllerBinding2 = null;
			ActivatorXBBinding axb = sender as ActivatorXBBinding;
			if (axb != null)
			{
				controllerBinding = this.FirstOrDefault((ControllerBinding c) => c.IsInheritedBinding && c.XBBinding.KeyScanCode == axb.KeyScanCode);
				controllerBinding2 = this.FirstOrDefault((ControllerBinding c) => !c.IsInheritedBinding && c.XBBinding.KeyScanCode == axb.KeyScanCode);
			}
			else
			{
				XBBinding xb = sender as XBBinding;
				if (xb != null)
				{
					controllerBinding = this.FirstOrDefault((ControllerBinding c) => c.IsInheritedBinding && c.XBBinding.KeyScanCode == xb.KeyScanCode);
					controllerBinding2 = this.FirstOrDefault((ControllerBinding c) => !c.IsInheritedBinding && c.XBBinding.KeyScanCode == xb.KeyScanCode);
				}
				else
				{
					AssociatedControllerButton acb = sender as AssociatedControllerButton;
					if (acb != null)
					{
						controllerBinding = this.FirstOrDefault((ControllerBinding c) => c.IsInheritedBinding && c.XBBinding.KeyScanCode == acb.KeyScanCode);
						controllerBinding2 = this.FirstOrDefault((ControllerBinding c) => !c.IsInheritedBinding && c.XBBinding.KeyScanCode == acb.KeyScanCode);
					}
				}
			}
			this.Validate();
			if (controllerBinding2 != null && !controllerBinding2.IsNoMapping && controllerBinding != null)
			{
				this.Remove(controllerBinding);
			}
		}

		public XBBinding this[KeyScanCodeV2 key]
		{
			get
			{
				ControllerBinding controllerBinding = this.FirstOrDefault((ControllerBinding cb) => cb.XBBinding.KeyScanCode == key);
				XBBinding xbbinding = ((controllerBinding != null) ? controllerBinding.XBBinding : null);
				if (xbbinding == null && this.CreateOnDemand)
				{
					this._suppressNotification = true;
					this.DoSetEntry(key, new XBBinding(this.HostCollection, key));
					this._suppressNotification = false;
					ControllerBinding controllerBinding2 = this.FirstOrDefault((ControllerBinding cb) => cb.XBBinding.KeyScanCode == key);
					xbbinding = ((controllerBinding2 != null) ? controllerBinding2.XBBinding : null);
				}
				return xbbinding;
			}
			set
			{
				this.DoSetEntry(key, value);
			}
		}

		private void DoSetEntry(KeyScanCodeV2 key, XBBinding value)
		{
			this.Remove((ControllerBinding cb) => cb.XBBinding.KeyScanCode == key);
			this.Add(new ControllerBinding(key, value, this));
		}

		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (this._suppressItemPropChangeNotifications || this.SuppressNotification)
			{
				return;
			}
			this._suppressItemPropChangeNotifications = true;
			this.Validate();
			this._suppressItemPropChangeNotifications = false;
			if (e.Action != NotifyCollectionChangedAction.Reset)
			{
				this.HostCollection.IsChanged = true;
			}
			if (this.HostCollection.IsShiftCollection && e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (ControllerBinding controllerBinding in e.OldItems.OfType<ControllerBinding>())
				{
					this._pendingInheritedKeyScanCodesToAdd.Add(controllerBinding.XBBinding.KeyScanCode);
					this.FilteredMasks.Remove(controllerBinding);
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				bool flag = false;
				foreach (object obj in e.OldItems)
				{
					flag |= ((ControllerBinding)obj).XBBinding.IsAnyActivatorVirtualGamepadMappingPresent;
				}
				if (flag)
				{
					this.HostCollection.SubConfigData.ConfigData.CheckVirtualMappingsExist();
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				if (this.HostCollection.SubConfigData.ConfigData.IsVirtualMappingExist)
				{
					this.HostCollection.SubConfigData.ConfigData.CheckVirtualMappingsExist();
				}
				this.FilteredMasks.Clear();
				this.ReFilterItems();
			}
		}

		private void OnAfterCollectionChanged(object sender, EventArgs e)
		{
			if (this._pendingInheritedKeyScanCodesToAdd.Any<KeyScanCodeV2>())
			{
				this.SuppressNotification = true;
				List<KeyScanCodeV2> list = new List<KeyScanCodeV2>(this._pendingInheritedKeyScanCodesToAdd);
				this._pendingInheritedKeyScanCodesToAdd.Clear();
				foreach (KeyScanCodeV2 keyScanCodeV in list)
				{
					this.AddInheritedItemIfNeededForShiftCollection(keyScanCodeV);
				}
				this.SuppressNotification = false;
			}
		}

		public void RemoveEmptyItems()
		{
			this.FilteredMasks.Remove((ControllerBinding cb) => cb.XBBinding.IsEmpty);
			this.Remove((ControllerBinding cb) => cb.XBBinding.IsEmpty);
		}

		public void Validate()
		{
			this.ValidateIsDuplicateToAnotherForChildren();
		}

		public void ValidateIsDuplicateToAnotherForChildren()
		{
			this.ForEach(delegate(ControllerBinding t)
			{
				t.IsDuplicateToAnother = false;
			});
			foreach (IGrouping<KeyScanCodeV2, ControllerBinding> grouping in from cb in this
				group cb by cb.XBBinding.KeyScanCode into @group
				where @group.Count<ControllerBinding>() > 1
				select @group)
			{
				foreach (ControllerBinding controllerBinding in grouping.Skip(1).ToList<ControllerBinding>())
				{
					controllerBinding.IsDuplicateToAnother = true;
				}
			}
		}

		private bool Filter(object obj)
		{
			if (this.SuppressNotification)
			{
				return false;
			}
			ControllerBinding controllerBinding = obj as ControllerBinding;
			return controllerBinding != null && (controllerBinding.XBBinding.KeyScanCode == KeyScanCodeV2.NoMap || (this.FilterByCombobox(controllerBinding) && this.FilterByDevice(controllerBinding)));
		}

		private bool FilterByCombobox(ControllerBinding cb)
		{
			if (this.ControllerButtonTag == null)
			{
				return true;
			}
			List<ControllerButtonTag> controllerButtonTags = cb.XBBinding.KeyScanCode.ControllerButtonTags;
			return controllerButtonTags != null && controllerButtonTags.Contains(this.ControllerButtonTag);
		}

		private bool FilterByDevice(ControllerBinding cb)
		{
			if (this.HostCollection.SubConfigData.ControllerFamily == 2 && cb.XBBinding.KeyScanCode.IsMouse)
			{
				return false;
			}
			if (this.ControllerButtonTag == null)
			{
				return true;
			}
			List<ControllerButtonTag> controllerButtonTags = cb.XBBinding.KeyScanCode.ControllerButtonTags;
			return controllerButtonTags != null && controllerButtonTags.Contains(this.ControllerButtonTag);
		}

		public bool Remove(XBBinding xbBinding)
		{
			foreach (ControllerBinding controllerBinding in this)
			{
				if (controllerBinding.XBBinding == xbBinding)
				{
					return this.Remove(controllerBinding);
				}
			}
			return false;
		}

		public new bool Remove(ControllerBinding item)
		{
			if (base.Contains(item))
			{
				if (this.FilteredMasks != this)
				{
					this.FilteredMasks.Remove(item);
				}
				return base.Remove(item);
			}
			return false;
		}

		public new void Add(ControllerBinding item)
		{
			base.Add(item);
			this.ReFilterItems();
		}

		public void RaiseCollectionItemPropertyChangedExtended(object s, PropertyChangedExtendedEventArgs e)
		{
			PropertyChangedExtendedEventHandler collectionItemPropertyChangedExtended = this.CollectionItemPropertyChangedExtended;
			if (collectionItemPropertyChangedExtended != null)
			{
				collectionItemPropertyChangedExtended(s, e);
			}
			this.OnCollectionItemPropertyChanged(s, e);
		}

		public void AddInheritedItemIfNeededForShiftCollection(KeyScanCodeV2 ksc)
		{
			ShiftXBBindingCollection shiftXBBindingCollection = this.HostCollection as ShiftXBBindingCollection;
			if (shiftXBBindingCollection == null)
			{
				return;
			}
			if (shiftXBBindingCollection.ControllerBindings.Contains(ksc) || !shiftXBBindingCollection.ParentBindingCollection.ControllerBindings.Contains(ksc))
			{
				return;
			}
			if (this.HostCollection.SubConfigData.ControllerFamily == 2 && ksc.IsMouse)
			{
				return;
			}
			XBBinding xbbinding = shiftXBBindingCollection.ParentBindingCollection.ControllerBindings[ksc];
			if (xbbinding.IsEmptySkipShift)
			{
				return;
			}
			XBBinding xbbinding2 = xbbinding.Clone();
			xbbinding2.HostCollection = shiftXBBindingCollection;
			xbbinding2.NullJumpToShift();
			if (xbbinding.ControllerButton.IsSet)
			{
				shiftXBBindingCollection.ControllerBindings.Add(new ControllerBinding(ksc, xbbinding2, shiftXBBindingCollection.ControllerBindings)
				{
					IsInheritedBinding = true
				});
			}
		}

		public void SetCurrentEditTo(AssociatedControllerButton associatedControllerButton)
		{
			if (associatedControllerButton == null)
			{
				this.CurrentEditItem = null;
				return;
			}
			this.CurrentEditItem = this.FirstOrDefault((ControllerBinding cb) => cb.XBBinding.ControllerButton.IsAssociatedSetToEqualButtons(associatedControllerButton));
		}

		public void CopyFromModel(ControllerBindingsCollection model, bool isCopyToNotEmpty = true)
		{
			this.SuppressNotification = true;
			if (isCopyToNotEmpty)
			{
				base.Clear();
			}
			this._controllerButtonTag = model.ControllerButtonTag;
			foreach (ControllerBinding controllerBinding in model)
			{
				if (isCopyToNotEmpty || (!isCopyToNotEmpty && !this.Contains(controllerBinding.XBBinding.KeyScanCode)))
				{
					XBBinding xbbinding = new XBBinding(this.HostCollection, controllerBinding.XBBinding.KeyScanCode);
					xbbinding.CopyFromModel(controllerBinding.XBBinding);
					ControllerBinding controllerBinding2 = new ControllerBinding(controllerBinding.XBBinding.KeyScanCode, xbbinding, this);
					controllerBinding2.CopyFromModel(controllerBinding);
					this.Add(controllerBinding2);
				}
			}
			this.SuppressNotification = false;
		}

		public void CopyToModel(ControllerBindingsCollection model)
		{
			model.Clear();
			model.ControllerButtonTag = this.ControllerButtonTag;
			using (IEnumerator<ControllerBinding> enumerator = base.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ControllerBinding item = enumerator.Current;
					if (!item.IsInheritedBinding && !model.Exists((ControllerBinding x) => x.XBBinding.KeyScanCode.Equals(item.XBBinding.KeyScanCode)))
					{
						XBBinding xbbinding = new XBBinding(model.HostCollection, item.XBBinding.KeyScanCode);
						item.XBBinding.CopyToModel(xbbinding);
						ControllerBinding controllerBinding = new ControllerBinding(item.XBBinding.KeyScanCode, xbbinding, model);
						item.CopyToModel(controllerBinding);
						model.Add(controllerBinding);
					}
				}
			}
		}

		private ControllerBinding _currentEditItem;

		private ControllerButtonTag _controllerButtonTag;

		private bool _suppressItemPropChangeNotifications;

		public bool CreateOnDemand;

		private readonly List<KeyScanCodeV2> _pendingInheritedKeyScanCodesToAdd = new List<KeyScanCodeV2>();
	}
}
