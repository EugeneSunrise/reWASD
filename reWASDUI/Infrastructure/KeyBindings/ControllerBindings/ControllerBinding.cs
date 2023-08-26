using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Attributes;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using Prism.Commands;
using reWASDUI.Infrastructure.KeyBindings.XB;
using XBEliteWPF.Infrastructure.KeyBindingsModel.ControllerBindings;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;

namespace reWASDUI.Infrastructure.KeyBindings.ControllerBindings
{
	public class ControllerBinding : ZBindableBase
	{
		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool IsNoSourceSelected
		{
			get
			{
				return this._isNoSourceSelected;
			}
			set
			{
				this.SetProperty<bool>(ref this._isNoSourceSelected, value, "IsNoSourceSelected");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
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

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
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

		public bool IsValid
		{
			get
			{
				return !this.IsNoSourceSelected && !this.IsNoMapping && !this.IsDuplicateToAnother;
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
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

		public XBBinding XBBinding { get; set; }

		public ControllerBindingsCollection HostCollection { get; set; }

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool IsInheritedBinding
		{
			get
			{
				return this._isInheritedBinding;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isInheritedBinding, value, "IsInheritedBinding"))
				{
					this.XBBinding.IsInheritedBinding = value;
				}
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
			if (this.IsInheritedBinding)
			{
				this.XBBinding.RevertRemap();
				this.XBBinding.ActivatorXBBindingDictionary.RemoveAllVirtualBindings();
				this.XBBinding.SingleActivator.MappedKey = KeyScanCodeV2.NoInheritance;
				return;
			}
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

		public ControllerBinding(XBBinding binding, ControllerBindingsCollection hostCollection)
		{
			this.HostCollection = hostCollection;
			this.XBBinding = binding;
			this.XBBinding.PropertyChangedExtended += this.XBBindingOnPropertyChangedExtended;
			this.HostCollection.CollectionChanged += delegate([Nullable(2)] object sender, NotifyCollectionChangedEventArgs args)
			{
				this.Validate();
			};
			this.XBBinding.ControllerButton.ControllerButtonChanged += this.ControllerButtonOnControllerButtonChanged;
			this.XBBinding.HostControllerBinding = this;
		}

		private void OnShiftCollectionItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "KeyScanCode")
			{
				this.Validate();
			}
		}

		public ControllerBinding(KeyScanCodeV2 ksc, XBBinding binding, ControllerBindingsCollection hostCollection)
			: this(binding, hostCollection)
		{
			this.XBBinding.KeyScanCode = ksc;
			this.XBBinding.ControllerButton.KeyScanCode.PropertyChanged += delegate([Nullable(2)] object sender, PropertyChangedEventArgs args)
			{
				this.Validate();
			};
		}

		public void Validate()
		{
			this.ValidateNoSourceSelected();
			this.ValidateShiftModifier();
			this.ValidateIsNoMapping();
			this.HostCollection.ValidateIsDuplicateToAnotherForChildren();
		}

		private void ValidateNoSourceSelected()
		{
			this.IsNoSourceSelected = this.XBBinding.ControllerButton.GamepadButton == 2001 && this.XBBinding.ControllerButton.KeyScanCode == KeyScanCodeV2.NoMap;
		}

		private void ValidateShiftModifier()
		{
		}

		private void ValidateIsNoMapping()
		{
			this.IsNoMapping = !this.XBBinding.IsAnyActivatorVirtualMappingPresent && !this.XBBinding.IsUnmapped && !this.XBBinding.IsContainsJumpToShift;
		}

		private void XBBindingOnPropertyChangedExtended(object sender, PropertyChangedExtendedEventArgs e)
		{
			if (DSUtils.HasAttributeDefined(sender.GetType(), e.PropertyName, typeof(DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent)))
			{
				return;
			}
			if (e.PropertyName == "IsInheritedBinding" || e.PropertyName == "MacroSequenceAnnotation")
			{
				return;
			}
			if (e.PropertyName == "JumpToShift")
			{
				this.HostCollection.ReFilterItems();
			}
			this.XBBinding.HostCollection.IsChanged = true;
			this.Validate();
			this.HostCollection.RaiseCollectionItemPropertyChangedExtended(sender, e);
			this.IsInheritedBinding = false;
		}

		private void ControllerButtonOnControllerButtonChanged(object sender, PropertyChangedExtendedEventArgs e)
		{
			this.Validate();
			this.HostCollection.RaiseCollectionItemPropertyChangedExtended(sender, e);
			this.IsInheritedBinding = false;
		}

		public void CopyFromModel(ControllerBinding model)
		{
			this.IsNoSourceSelected = model.IsInheritedBinding;
			this.IsDuplicateToAnother = model.IsDuplicateToAnother;
			this.IsNoMapping = model.IsNoMapping;
			this.IsInheritedBinding = model.IsInheritedBinding;
		}

		public void CopyToModel(ControllerBinding model)
		{
			model.IsNoSourceSelected = this.IsInheritedBinding;
			model.IsDuplicateToAnother = this.IsDuplicateToAnother;
			model.IsNoMapping = this.IsNoMapping;
			model.IsInheritedBinding = this.IsInheritedBinding;
		}

		private bool _isNoSourceSelected;

		private bool _isDuplicateToAnother;

		private bool _isNoMapping;

		private bool _isEditMode;

		private bool _isInheritedBinding;

		private DelegateCommand _remove;

		private DelegateCommand _edit;
	}
}
