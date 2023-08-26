using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.KeyBindings.ActivatorXB;
using reWASDUI.Infrastructure.KeyBindings.XB;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.KeyBindingsModel.ActivatorXB;
using XBEliteWPF.Infrastructure.KeyBindingsModel.MacroBinding;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;

namespace reWASDUI.Infrastructure.KeyBindings.MacroBinding
{
	public class ActivatorXBBindingDictionary : ObservableDictionary<ActivatorType, ActivatorXBBinding>, IDisposable
	{
		private void FireValueSet(ActivatorType key)
		{
			EventHandler<ActivatorType> onValueSet = this.OnValueSet;
			if (onValueSet != null)
			{
				onValueSet(this, key);
			}
			this.OnPropertyChanged("IsAnyNonSingleActivatorHasMapping");
		}

		public ActivatorXBBinding SingleActivator
		{
			get
			{
				return base.TryGetValue(0);
			}
		}

		public bool IsAnyNonSingleActivatorHasMapping
		{
			get
			{
				return base.AnyValue((ActivatorXBBinding axb) => !axb.IsSingleActivator && (axb.IsAnnotationShouldBeShownForMapping || axb.IsDescriptionPresent));
			}
		}

		public XBBinding HostXBBinding
		{
			get
			{
				return this._hostXBBinding;
			}
			set
			{
				this._hostXBBinding = value;
				base.ForEachValue(delegate(ActivatorXBBinding item)
				{
					item.InitShiftType();
				});
			}
		}

		public bool IsInheritedBinding
		{
			get
			{
				return this._isInheritedBinding;
			}
			set
			{
				this._isInheritedBinding = value;
				base.ForEachValue(delegate(ActivatorXBBinding item)
				{
					item.IsInheritedBinding = value;
				});
				this.OnPropertyChanged("IsAnyNonSingleActivatorHasMapping");
			}
		}

		public ActivatorXBBindingDictionary(GamepadButton gb)
			: this(new AssociatedControllerButton
			{
				GamepadButton = gb
			})
		{
		}

		public ActivatorXBBindingDictionary(GamepadButtonDescription gbd)
			: this(new AssociatedControllerButton
			{
				GamepadButtonDescription = gbd
			})
		{
		}

		public ActivatorXBBindingDictionary(KeyScanCodeV2 ksc)
			: this(new AssociatedControllerButton
			{
				KeyScanCode = ksc
			})
		{
		}

		public ActivatorXBBindingDictionary(AssociatedControllerButton cb)
		{
			this.CollectionChanged += this.OnCollectionChanged;
			this.FillSelfWithDefault(cb);
			base.CollectionItemPropertyChanged += this.OnCollectionItemPropertyChanged;
		}

		public override void Dispose()
		{
			base.Dispose();
			this.HostXBBinding = null;
		}

		public ActivatorXBBindingDictionary(ActivatorXBBindingDictionary dictionary, AssociatedControllerButton controllerButton = null)
		{
			foreach (KeyValuePair<ActivatorType, ActivatorXBBinding> keyValuePair in ((IEnumerable<KeyValuePair<ActivatorType, ActivatorXBBinding>>)dictionary))
			{
				ActivatorXBBinding activatorXBBinding = ((controllerButton != null) ? keyValuePair.Value.Clone(controllerButton) : keyValuePair.Value.Clone(null));
				activatorXBBinding.HostDictionary = this;
				base.Add(keyValuePair.Key, activatorXBBinding);
			}
			this.CollectionChanged += this.OnCollectionChanged;
			base.CollectionItemPropertyChanged += this.OnCollectionItemPropertyChanged;
		}

		public void CheckJumpToShift()
		{
			if (this.HostXBBinding != null)
			{
				bool canContainShift = this.HostXBBinding.CanContainShift;
				base.ForEachValue(delegate(ActivatorXBBinding item)
				{
					item.JumpToShift = (canContainShift ? item.JumpToShift : (-1));
					if (item.JumpToShift == this.HostXBBinding.HostCollection.ShiftIndex)
					{
						item.JumpToShift = -1;
					}
				});
			}
		}

		public void CheckOverlayCommands()
		{
			if (this.HostXBBinding != null)
			{
				bool isOverlayShift = this.HostXBBinding.HostCollection.IsOverlayCollection;
				base.ForEachValue(delegate(ActivatorXBBinding item)
				{
					if (item.MappedKey.IsAnyOverlayMenuCommand && !isOverlayShift)
					{
						item.ClearVirtualMapping();
					}
				});
			}
		}

		private void OnCollectionItemPropertyChanged(object s, PropertyChangedEventArgs e)
		{
			this.OnPropertyChanged("IsAnyNonSingleActivatorHasMapping");
		}

		private void OnCollectionChanged(object o, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				foreach (object obj in e.OldItems)
				{
					((KeyValuePair<ActivatorType, ActivatorXBBinding>)obj).Value.HostDictionary = null;
				}
			}
			if (e.NewItems != null)
			{
				foreach (object obj2 in e.NewItems)
				{
					((KeyValuePair<ActivatorType, ActivatorXBBinding>)obj2).Value.HostDictionary = this;
				}
			}
		}

		public ActivatorXBBinding this[ActivatorType key]
		{
			get
			{
				return base[key];
			}
			set
			{
				base[key] = value;
				this.FireValueSet(key);
			}
		}

		public ActivatorXBBinding GetNonEmptyActivator()
		{
			foreach (object obj in Enum.GetValues(typeof(ActivatorType)))
			{
				ActivatorType activatorType = (ActivatorType)obj;
				ActivatorXBBinding activatorXBBinding = base.TryGetValue(activatorType);
				if (activatorXBBinding.IsVirtualMappingPresent)
				{
					return activatorXBBinding;
				}
			}
			return base.TryGetValue(0);
		}

		private void FillSelfWithDefault(AssociatedControllerButton cb)
		{
			foreach (object obj in Enum.GetValues(typeof(ActivatorType)))
			{
				ActivatorType activatorType = (ActivatorType)obj;
				base.Add(activatorType, new ActivatorXBBinding(cb, activatorType));
			}
		}

		public void RemoveAllVirtualBindings()
		{
			base.ForEachValue(delegate(ActivatorXBBinding v)
			{
				v.IsRumble = false;
				v.IsTurbo = false;
				v.IsToggle = false;
			});
			this.ClearMappings();
			this.ClearMacroSequences();
			this.ClearShift();
		}

		public void RemoveAllDescriptions()
		{
			this.ForEach(delegate(KeyValuePair<ActivatorType, ActivatorXBBinding> kvp)
			{
				kvp.Value.Description = null;
			});
		}

		public void ClearMappings()
		{
			base.ForEachValue(delegate(ActivatorXBBinding v)
			{
				v.MappedKey = KeyScanCodeV2.NoMap;
			});
		}

		public void ClearShift()
		{
			base.ForEachValue(delegate(ActivatorXBBinding v)
			{
				v.ClearJumpToShift();
			});
		}

		public void PrepareForSave()
		{
			base.ForEachValue(delegate(ActivatorXBBinding kvp)
			{
				kvp.MacroSequence.PrepareForSave();
			});
		}

		public void ClearMacroSequences()
		{
			base.ForEachValue(delegate(ActivatorXBBinding v)
			{
				v.ClearMacroSequence();
			});
		}

		public void CopyFromModel(ActivatorXBBindingDictionary model)
		{
			this.ForEach(delegate(KeyValuePair<ActivatorType, ActivatorXBBinding> kvp)
			{
				ActivatorXBBinding activatorXBBinding = model[kvp.Key];
				if (activatorXBBinding != null)
				{
					kvp.Value.CopyFromModel(activatorXBBinding);
				}
			});
		}

		public void CopyToModel(ActivatorXBBindingDictionary model)
		{
			this.ForEach(delegate(KeyValuePair<ActivatorType, ActivatorXBBinding> kvp)
			{
				if (kvp.Value.IsVirtualMappingPresent || kvp.Value.IsDescriptionPresent)
				{
					model.FillEmpty(kvp.Key);
					kvp.Value.CopyToModel(model[kvp.Key]);
				}
			});
		}

		public EventHandler<ActivatorType> OnValueSet;

		public XBBinding _hostXBBinding;

		private bool _isInheritedBinding;
	}
}
