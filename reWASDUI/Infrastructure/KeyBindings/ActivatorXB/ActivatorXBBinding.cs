using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Attributes;
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoft.NET.Common.View.SecondaryWindows;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Regions;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.MacroCompilers;
using reWASDUI.Infrastructure.KeyBindings.MacroBinding;
using reWASDUI.Infrastructure.KeyBindings.Mask;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups;
using reWASDUI.Views.ContentZoneGamepad.Macro;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.KeyBindingsModel.ActivatorXB;
using XBEliteWPF.Infrastructure.KeyBindingsModel.MacroBinding;
using XBEliteWPF.Infrastructure.reWASDMapping;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Infrastructure.reWASDMapping.RewasduserCommands;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.ViewModels.Base;

namespace reWASDUI.Infrastructure.KeyBindings.ActivatorXB
{
	public class ActivatorXBBinding : ZBindable
	{
		public DelegateCommand OpenMacroEditorCommandCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._openMacroEditorCommand) == null)
				{
					delegateCommand = (this._openMacroEditorCommand = new DelegateCommand(new Action(this.OpenMacroEditorCommand)));
				}
				return delegateCommand;
			}
		}

		private void OpenMacroEditorCommand()
		{
			if (App.GameProfilesService.RealCurrentBeingMappedBindingCollection != null)
			{
				Dictionary<object, object> dictionary = new Dictionary<object, object>();
				dictionary.Add("navigatePath", typeof(MacroSettings));
				NavigationParameters navigationParameters = new NavigationParameters();
				navigationParameters.Add("XBBinding", this.ShiftXBBinding);
				dictionary.Add("NavigationParameters", navigationParameters);
				reWASDApplicationCommands.NavigateGamepadCommand.Execute(dictionary);
			}
		}

		private void CheckAndInitMacroSequence()
		{
			if (this._macroSequence == null && this._macroSequenceInit)
			{
				this._macroSequenceInit = false;
				this._macroSequence = new MacroSequence(this._controllerButton, 1, this._activatorType);
				this._macroSequence.SupressOnCollectionChanged = false;
				this._macroSequence.CollectionChanged += this.MacroSequenceOnCollectionChanged;
				this._macroSequence.MacroSequencePropertyChanged += this.MacroSequencePropertyChanged;
			}
		}

		public ushort MappedKeyBytes
		{
			get
			{
				return this._mappedKeyBytes;
			}
			set
			{
				this.SetProperty<ushort>(ref this._mappedKeyBytes, value, "MappedKeyBytes");
			}
		}

		public BaseRewasdMapping MappedKey
		{
			get
			{
				if (this._mappedKey == null)
				{
					this._mappedKey = KeyScanCodeV2.NoMap;
				}
				return this._mappedKey;
			}
			set
			{
				BaseRewasdMapping mappedKey = this._mappedKey;
				if (this.SetProperty<BaseRewasdMapping>(ref this._mappedKey, value, "MappedKey"))
				{
					if (value.PCKeyCategory == 2 || value.PCKeyCategory == 9)
					{
						this.IsToggle = false;
						this.IsTurbo = false;
					}
					if (value == KeyScanCodeV2.NoMap && !this.IsRumble)
					{
						this.IsToggle = false;
						this.IsTurbo = false;
					}
					KeyScanCodeV2 keyScanCodeV = value as KeyScanCodeV2;
					if (keyScanCodeV != null)
					{
						this.MappedKeyBytes = (ushort)KeyScanCodeV2.GetIndex(keyScanCodeV);
					}
					else
					{
						this.MappedKeyBytes = 0;
					}
					BaseXBBindingCollection hostCollection = this.HostCollection;
					bool flag;
					if (hostCollection == null)
					{
						flag = null != null;
					}
					else
					{
						SubConfigData subConfigData = hostCollection.SubConfigData;
						flag = ((subConfigData != null) ? subConfigData.ConfigData : null) != null;
					}
					if (flag)
					{
						if (value != null && value.IsGamepadOrGyroTilt)
						{
							this.HostCollection.SubConfigData.ConfigData.IsVirtualMappingExist = true;
						}
						else if (mappedKey != null && mappedKey.IsGamepadOrGyroTilt)
						{
							this.HostCollection.SubConfigData.ConfigData.CheckVirtualMappingsExist();
						}
					}
					this.OnPropertyChanged("IsMappedToNumKey");
					this.OnPropertyChanged("IsActivatorVisible");
					this.OnPropertyChanged("IsAnnotationShouldBeShownForMapping");
					this.OnPropertyChanged("IsAnnotationShouldBeShownForMappingWithoutShift");
					this.OnPropertyChanged("IsVirtualMappingPresent");
					this.OnPropertyChanged("IsTurboOrToggleCanBeEnabled");
					this.OnPropertyChanged("IsToggleCanBeEnabled");
					this.OnPropertyChanged("IsTurboOrToggleCanBeEnabledWithoutMapping");
					this.OnPropertyChanged("IsTurboOrTogglePeripheralCheck");
					this.OnPropertyChanged("IsTurbo");
					this.OnPropertyChanged("IsToggle");
					this.OnPropertyChanged("IsMappedKeyAnalog");
					this.OnPropertyChanged("IsMacroMapping");
					BaseXBBindingCollection hostCollection2 = this.HostCollection;
					bool flag2;
					if (hostCollection2 == null)
					{
						flag2 = null != null;
					}
					else
					{
						SubConfigData subConfigData2 = hostCollection2.SubConfigData;
						flag2 = ((subConfigData2 != null) ? subConfigData2.ConfigData : null) != null;
					}
					if (flag2)
					{
						this.HostCollection.SubConfigData.ConfigData.IsChanged = true;
					}
				}
			}
		}

		public bool IsActivatorVisible
		{
			get
			{
				BaseXBBindingCollection realCurrentBeingMappedBindingCollection = App.GameProfilesService.RealCurrentBeingMappedBindingCollection;
				bool flag = realCurrentBeingMappedBindingCollection != null && realCurrentBeingMappedBindingCollection.IsLabelModeView;
				BaseXBBindingCollection realCurrentBeingMappedBindingCollection2 = App.GameProfilesService.RealCurrentBeingMappedBindingCollection;
				return (realCurrentBeingMappedBindingCollection2 != null && realCurrentBeingMappedBindingCollection2.IsShowMappingsView && (this.IsVirtualMappingPresent || this.IsAdaptiveTriggers)) || (flag && this.IsDescriptionPresent);
			}
		}

		public ActivatorXBBindingDictionary HostDictionary { get; set; }

		public BaseXBBindingCollection HostCollection
		{
			get
			{
				return this.HostDictionary.HostXBBinding.HostCollection;
			}
		}

		public ActivatorType ActivatorType
		{
			get
			{
				return this._activatorType;
			}
		}

		public GamepadButton GamepadButton
		{
			get
			{
				return this._controllerButton.GamepadButton;
			}
		}

		public KeyScanCodeV2 KeyScanCode
		{
			get
			{
				return this._controllerButton.KeyScanCode;
			}
		}

		public BaseXBBindingCollection ShiftModificatorCollection
		{
			get
			{
				if (this.JumpToShift != -1)
				{
					return this.HostCollection.SubConfigData.MainXBBindingCollection.GetCollectionByLayer(this.JumpToShift);
				}
				return null;
			}
		}

		public XBBinding ShiftXBBinding
		{
			get
			{
				if (this.JumpToShift != -1)
				{
					return this.GetXBBindingByLayer(this.JumpToShift, this.IsCurrentCollection, true);
				}
				return this.HostDictionary.HostXBBinding;
			}
		}

		private bool IsCurrentCollection
		{
			get
			{
				return this.HostCollection == App.GameProfilesService.RealCurrentBeingMappedBindingCollection;
			}
		}

		public ActivatorXBBinding ShiftActivatorXBBinding
		{
			get
			{
				XBBinding xbbinding = ((this.JumpToShift == -1) ? this.HostDictionary.HostXBBinding : this.GetXBBindingByLayer(this.JumpToShift, this.IsCurrentCollection, false));
				if (xbbinding == null)
				{
					return null;
				}
				return xbbinding.ActivatorXBBindingDictionary[this.ActivatorType];
			}
		}

		public void SetButtonsFromAnotherInstance(AssociatedControllerButton acb)
		{
			if (acb.IsKeyScanCode)
			{
				this._controllerButton.KeyScanCode = acb.KeyScanCode;
				this._controllerButton.GamepadButton = 2001;
				return;
			}
			this._controllerButton.GamepadButton = acb.GamepadButton;
			this._controllerButton.KeyScanCode = KeyScanCodeV2.NoMap;
		}

		public bool IsSingleActivator
		{
			get
			{
				return this.ActivatorType == 0;
			}
		}

		public bool IsLongActivator
		{
			get
			{
				return this.ActivatorType == 1;
			}
		}

		public bool IsDoubleActivator
		{
			get
			{
				return this.ActivatorType == 2;
			}
		}

		public bool IsTripleActivator
		{
			get
			{
				return this.ActivatorType == 3;
			}
		}

		public bool IsStartActivator
		{
			get
			{
				return this.ActivatorType == 4;
			}
		}

		public bool IsReleaseActivator
		{
			get
			{
				return this.ActivatorType == 5;
			}
		}

		public bool IsJumpToShift
		{
			get
			{
				return this.JumpToShift != -1;
			}
		}

		public bool IsReturnToMainShift
		{
			get
			{
				return this.JumpToShift == 0;
			}
		}

		public bool IsShowAnnotationAcordingShift
		{
			get
			{
				if (this.IsJumpToShift)
				{
					BaseXBBindingCollection hostCollection = this.HostCollection;
					if ((hostCollection == null || !hostCollection.IsLabelModeView) && !this.IsInheritedBinding)
					{
						return (!this.IsShiftToggle || this.ShiftActivatorXBBinding.IsToggle) && (!this.IsDelayBerforeJumpChecked || this.IsPostponeMapping);
					}
				}
				return true;
			}
		}

		public bool JumpToCurrentLayerExistFromSameActivator
		{
			get
			{
				return this.IsAnyJumpToCurrentLayerExistFromSameActivator();
			}
		}

		public bool IsAlwaysEnabledJumpToShift
		{
			get
			{
				return this.HostCollection.IsMainCollection && !this.IsStartActivator && !this.IsReleaseActivator;
			}
		}

		public bool IsHideMappings
		{
			get
			{
				return this.ActivatorType != 5 && (this.IsJumpToShift || this.JumpToCurrentLayerExistFromSameActivator);
			}
		}

		public bool IsAnyDirectionHasJumpToShift
		{
			get
			{
				BaseDirectionalGroup currentBoundGroup = this.HostCollection.CurrentBoundGroup;
				return currentBoundGroup != null && currentBoundGroup.IsAnyDirectionHasJumpToShift;
			}
		}

		public int ShiftOfJumpToCurrentLayer
		{
			get
			{
				return this.GetAnyJumpToCurrentLayer();
			}
		}

		public void ResetJumpToShift()
		{
			this.JumpToShift = -1;
			this.ResetDelayBeforeJump();
		}

		public void NullJumpToShift()
		{
			this.SetProperty<int>(ref this._jumpToShift, -1, "JumpToShift");
			this.OnPropertyChanged("IsJumpToShift");
			this.ResetDelayBeforeJump();
		}

		public void MoveJumpToShift(int delta)
		{
			this._jumpToShift += delta;
			this.OnPropertyChanged("JumpToShift");
		}

		public void ResetShiftType()
		{
			if (this.HostCollection.IsShiftCollection)
			{
				return;
			}
			if (this.IsStartActivator || this.IsReleaseActivator)
			{
				this.SetShiftType(1);
				return;
			}
			this.SetShiftType(0);
		}

		public void ResetDelayBeforeJump()
		{
			this.IsDelayBerforeJumpChecked = false;
			this.DelayBerforeJump = 200;
			this.IsPostponeMapping = false;
		}

		public void ClearJumpToShift()
		{
			this.ResetJumpToShift();
			this.ResetShiftType();
			this.ResetDelayBeforeJump();
			this.UpdateJumpToShiftProperties();
		}

		public void UpdateJumpToShiftProperties()
		{
			this.OnPropertyChanged("IsActivatorVisible");
			this.OnPropertyChanged("IsAnnotationShouldBeShownForMapping");
			this.OnPropertyChanged("IsAnnotationShouldBeShownForMappingWithoutShift");
			this.OnPropertyChanged("IsJumpToShift");
			this.OnPropertyChanged("IsShiftHold");
			this.OnPropertyChanged("IsShiftToggle");
			this.OnPropertyChanged("IsShiftCustom");
			this.OnPropertyChanged("JumpToCurrentLayerExistFromSameActivator");
			this.OnPropertyChanged("ShiftOfJumpToCurrentLayer");
			this.OnPropertyChanged("IsHideMappings");
			this.OnPropertyChanged("ShiftModificatorCollection");
			this.OnPropertyChanged("IsAnyDirectionHasJumpToShift");
			this.OnPropertyChanged("ShiftXBBinding");
			this.OnPropertyChanged("ShiftActivatorXBBinding");
			this.OnPropertyChanged("IsVirtualMappingPresent");
			this.OnPropertyChanged("IsShowAnnotationAcordingShift");
		}

		public int JumpToShift
		{
			get
			{
				return this._jumpToShift;
			}
			set
			{
				int num = value;
				int jumpToShift = this._jumpToShift;
				if (num == this.HostCollection.ShiftIndex)
				{
					this.ResetDelayBeforeJump();
					if (num == 0)
					{
						this.SetProperty<int>(ref this._jumpToShift, -1, "JumpToShift");
						this.UpdateJumpToShiftProperties();
						return;
					}
					num = -1;
				}
				if (jumpToShift == num)
				{
					return;
				}
				ActivatorShiftType currentShiftType = this.GetCurrentShiftType();
				this.SetProperty<int>(ref this._jumpToShift, value, "JumpToShift");
				if (num == -1)
				{
					XBBinding xbbindingByLayer = this.GetXBBindingByLayer(jumpToShift, false, true);
					if (xbbindingByLayer != null)
					{
						xbbindingByLayer.RemoveKeyBindingInActivator(this.ActivatorType);
					}
				}
				if (num != -1)
				{
					XBBinding shiftXBBinding = this.ShiftXBBinding;
					if (shiftXBBinding != null && shiftXBBinding.IsEmpty)
					{
						shiftXBBinding.RemapedTo = this.HostDictionary.HostXBBinding.RemapedTo;
					}
				}
				if (jumpToShift == -1)
				{
					this.SyncMappingWithShift(num);
				}
				if (currentShiftType != 2)
				{
					this.SetReturnShift(jumpToShift, currentShiftType, -1, true);
					this.SetReturnShift(num, currentShiftType, 0, true);
				}
				else
				{
					if (jumpToShift != -1)
					{
						this.RemoveXBBindingIfEmpty(this.GetXBBindingByLayer(jumpToShift, false, true));
					}
					if (num != -1)
					{
						this.GetXBBindingByLayer(num, true, true);
					}
				}
				this.UpdateJumpToShiftProperties();
			}
		}

		private void SyncMappingOnSettingsChanged()
		{
			if (this.IsDelayBerforeJumpChecked && !this.IsPostponeMapping)
			{
				this.HostDictionary.HostXBBinding.CopyToAnotherShifts();
			}
			else if (!this.IsShiftToggle)
			{
				this.HostDictionary.HostXBBinding.RemoveKeyBindingInActivator(this.ActivatorType);
			}
			this.OnPropertyChanged("IsShowAnnotationAcordingShift");
		}

		private void SyncMappingWithShift(int shift)
		{
			ActivatorXBBinding activatorXBBinding = this.GetXBBindingByLayer(shift, true, true).ActivatorXBBindingDictionary[this.ActivatorType];
			if (this.IsVirtualMappingPresentSkipShift)
			{
				ActivatorXBBinding activatorXBBinding2 = new ActivatorXBBinding(this._controllerButton, this._activatorType);
				activatorXBBinding2.HostDictionary = this.HostDictionary;
				activatorXBBinding2.CopyMappingFrom(this);
				if (!this.JumpToCurrentLayerExistFromSameActivator)
				{
					this.HostDictionary.HostXBBinding.RemoveKeyBinding();
				}
				activatorXBBinding.CopyMappingFrom(activatorXBBinding2);
			}
			else if (this.IsShiftToggle && activatorXBBinding.IsVirtualMappingPresentSkipShift)
			{
				this.CopyMappingFrom(activatorXBBinding);
			}
			if (this.IsShiftToggle)
			{
				ActivatorXBBinding shiftActivatorXBBinding = this.ShiftActivatorXBBinding;
				if (shiftActivatorXBBinding == null || !shiftActivatorXBBinding.IsToggle)
				{
					return;
				}
			}
			this.HostDictionary.HostXBBinding.RemoveKeyBindingInActivator(this.ActivatorType);
		}

		public XBBinding GetXBBindingByLayer(int shift, bool createIfNeeded = false, bool switchActivator = true)
		{
			if (this.HostCollection == null)
			{
				return null;
			}
			BaseXBBindingCollection collectionByLayer = this.HostCollection.SubConfigData.MainXBBindingCollection.GetCollectionByLayer(shift);
			if (collectionByLayer == null)
			{
				return null;
			}
			MaskItem hostMaskItem = this.HostDictionary.HostXBBinding.HostMaskItem;
			XBBinding xbbinding;
			if (hostMaskItem != null)
			{
				MaskItem maskItem = collectionByLayer.MaskBindingCollection.FindSimilarMaskItem(hostMaskItem);
				xbbinding = ((maskItem != null) ? maskItem.XBBinding : null);
				if (createIfNeeded && xbbinding == null)
				{
					MaskItem maskItem2 = collectionByLayer.MaskBindingCollection.AddMaskItemWithMaskCondition(hostMaskItem.MaskConditions);
					xbbinding = ((maskItem2 != null) ? maskItem2.XBBinding : null);
				}
			}
			else
			{
				AssociatedControllerButton controllerButton = this.HostDictionary.HostXBBinding.ControllerButton;
				bool createOnDemand = collectionByLayer.ControllerBindings.CreateOnDemand;
				collectionByLayer.ControllerBindings.CreateOnDemand = createIfNeeded;
				xbbinding = collectionByLayer.GetXBBindingByAssociatedControllerButton(controllerButton);
				collectionByLayer.ControllerBindings.CreateOnDemand = createOnDemand;
			}
			if (switchActivator && createIfNeeded && xbbinding != null)
			{
				xbbinding.SwitchCurrentActivator(this._activatorType);
			}
			return xbbinding;
		}

		private bool IsAnyJumpToCurrentLayerExistFromSameActivator()
		{
			return this.GetJumpToCurrentLayer().Count != 0;
		}

		public int GetAnyJumpToCurrentLayer()
		{
			List<int> jumpToCurrentLayer = this.GetJumpToCurrentLayer();
			if (jumpToCurrentLayer.Count == 0)
			{
				return -1;
			}
			return jumpToCurrentLayer.First<int>();
		}

		public List<int> GetJumpToCurrentLayer()
		{
			List<int> list = new List<int>();
			foreach (int num in this.JumpToShiftItems)
			{
				if (num != -1)
				{
					XBBinding xbbindingByLayer = this.GetXBBindingByLayer(num, false, true);
					if (xbbindingByLayer != null && xbbindingByLayer.ActivatorXBBindingDictionary[this.ActivatorType].JumpToShift == this.HostCollection.ShiftIndex)
					{
						list.Add(num);
					}
				}
			}
			return list;
		}

		private void RemoveXBBindingIfEmpty(XBBinding xbBinding)
		{
			if (xbBinding == null)
			{
				return;
			}
			if (xbBinding.IsEmpty)
			{
				if (xbBinding.HostControllerBinding != null)
				{
					xbBinding.HostCollection.ControllerBindings.Remove(xbBinding);
				}
				if (xbBinding.HostMaskItem != null)
				{
					xbBinding.HostCollection.MaskBindingCollection.Remove(xbBinding.HostMaskItem);
				}
			}
		}

		private void SetReturnShift(int shift, ActivatorShiftType shiftType, int layer, bool removeEmptyBinding = true)
		{
			if (shift == -1)
			{
				return;
			}
			XBBinding xbbindingByLayer = this.GetXBBindingByLayer(shift, true, true);
			if (shiftType == 2)
			{
				return;
			}
			if (xbbindingByLayer != null)
			{
				ActivatorXBBinding activatorXBBinding = xbbindingByLayer.ActivatorXBBindingDictionary[(shiftType == null) ? 5 : this.ActivatorType];
				activatorXBBinding._jumpToShift = layer;
				activatorXBBinding.UpdateJumpToShiftProperties();
				if (xbbindingByLayer.HostMaskItem != null)
				{
					xbbindingByLayer.HostMaskItem.ValidateInternalConditions();
				}
				if (removeEmptyBinding && layer == -1 && xbbindingByLayer.IsEmpty)
				{
					this.RemoveXBBindingIfEmpty(xbbindingByLayer);
				}
				this.UpdateJumpToShiftProperties();
			}
		}

		private void SetShiftType(ActivatorShiftType shiftType)
		{
			ActivatorShiftType currentShiftType = this.GetCurrentShiftType();
			if (currentShiftType == shiftType || this.HostCollection.IsShiftCollection)
			{
				return;
			}
			if (shiftType == 1)
			{
				if (this._jumpToShift != -1)
				{
					this.ShowWarningAboutCopyMappings();
				}
				this.IsPostponeMapping = false;
			}
			if (this._jumpToShift != -1)
			{
				this.SetReturnShift(this._jumpToShift, currentShiftType, -1, false);
				this.SetReturnShift(this._jumpToShift, shiftType, 0, true);
				this.SyncMappingWithShift(this._jumpToShift);
			}
			this.HostDictionary.HostXBBinding.CopyToAnotherShifts();
			this._shiftTypeForUnsetShift = shiftType;
			this.OnPropertyChanged("IsShiftHold");
			this.OnPropertyChanged("IsShiftToggle");
			this.OnPropertyChanged("IsShiftCustom");
			this.OnPropertyChanged("ShiftXBBinding");
			this.OnPropertyChanged("ShiftActivatorXBBinding");
			BaseXBBindingCollection hostCollection = this.HostCollection;
			bool flag;
			if (hostCollection == null)
			{
				flag = null != null;
			}
			else
			{
				SubConfigData subConfigData = hostCollection.SubConfigData;
				flag = ((subConfigData != null) ? subConfigData.ConfigData : null) != null;
			}
			if (flag)
			{
				this.HostCollection.SubConfigData.ConfigData.IsChanged = true;
			}
		}

		private ActivatorShiftType GetCurrentShiftType()
		{
			if (this.HostCollection == null || this.HostCollection.IsShiftCollection)
			{
				return 2;
			}
			if (this.JumpToShift == -1)
			{
				return this._shiftTypeForUnsetShift;
			}
			XBBinding xbbindingByLayer = this.GetXBBindingByLayer(this.JumpToShift, false, true);
			if (xbbindingByLayer != null && xbbindingByLayer.ActivatorXBBindingDictionary[this.ActivatorType].JumpToShift == 0)
			{
				return 1;
			}
			if (xbbindingByLayer != null && xbbindingByLayer.ActivatorXBBindingDictionary[5].JumpToShift == 0)
			{
				return 0;
			}
			return 2;
		}

		public bool IsShiftHold
		{
			get
			{
				return this.GetCurrentShiftType() == 0;
			}
			set
			{
				if (value)
				{
					this.SetShiftType(0);
				}
			}
		}

		public bool IsShiftToggle
		{
			get
			{
				return this.GetCurrentShiftType() == 1;
			}
			set
			{
				if (value)
				{
					this.SetShiftType(1);
				}
			}
		}

		public bool IsShiftCustom
		{
			get
			{
				return this.GetCurrentShiftType() == 2;
			}
			set
			{
				if (value)
				{
					this.SetShiftType(2);
				}
			}
		}

		public int DelayBerforeJump
		{
			get
			{
				return this._delayBerforeJump;
			}
			set
			{
				this.SetProperty<int>(ref this._delayBerforeJump, value, "DelayBerforeJump");
			}
		}

		public bool IsDelayBerforeJumpChecked
		{
			get
			{
				return this._isDelayBerforeJumpChecked;
			}
			set
			{
				if (this._isDelayBerforeJumpChecked != value && value && !this.IsPostponeMapping)
				{
					this.ShowWarningAboutCopyMappings();
				}
				if (this.SetProperty<bool>(ref this._isDelayBerforeJumpChecked, value, "IsDelayBerforeJumpChecked"))
				{
					if (!value && this.IsPostponeMapping)
					{
						this.IsPostponeMapping = false;
					}
					this.SyncMappingOnSettingsChanged();
				}
			}
		}

		public bool IsPostponeMapping
		{
			get
			{
				return this._isPostponeMapping;
			}
			set
			{
				if (this.IsDelayBerforeJumpChecked && !value && value != this._isPostponeMapping)
				{
					this.ShowWarningAboutCopyMappings();
				}
				if (this.SetProperty<bool>(ref this._isPostponeMapping, value, "IsPostponeMapping"))
				{
					this.SyncMappingOnSettingsChanged();
				}
			}
		}

		public List<int> JumpToShiftItemsFull
		{
			get
			{
				List<int> list = new List<int> { -1, 0 };
				BaseXBBindingCollection hostCollection = this.HostCollection;
				int? num = ((hostCollection != null) ? new int?(hostCollection.SubConfigData.MainXBBindingCollection.ShiftXBBindingCollections.Count) : null);
				if (num != null)
				{
					list.AddRange(Enumerable.Range(1, num.Value));
				}
				return list;
			}
		}

		public List<int> JumpToShiftItems
		{
			get
			{
				return this.JumpToShiftItemsFull.Where(delegate(int item)
				{
					BaseXBBindingCollection hostCollection = this.HostCollection;
					int? num = ((hostCollection != null) ? new int?(hostCollection.ShiftIndex) : null);
					return !((item == num.GetValueOrDefault()) & (num != null));
				}).ToList<int>();
			}
		}

		public string Description
		{
			get
			{
				return this._description;
			}
			set
			{
				if (this.SetProperty<string>(ref this._description, value, "Description"))
				{
					this.OnPropertyChanged("IsDescriptionPresent");
					this.OnPropertyChanged("IsActivatorVisible");
					this.OnPropertyChanged("IsAnnotationShouldBeShownForMapping");
					this.OnPropertyChanged("IsAnnotationShouldBeShownForMappingWithoutShift");
					BaseXBBindingCollection hostCollection = this.HostCollection;
					bool flag;
					if (hostCollection == null)
					{
						flag = null != null;
					}
					else
					{
						SubConfigData subConfigData = hostCollection.SubConfigData;
						flag = ((subConfigData != null) ? subConfigData.ConfigData : null) != null;
					}
					if (flag)
					{
						this.HostCollection.SubConfigData.ConfigData.IsChanged = true;
					}
				}
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool IsAdaptiveTriggersInherited
		{
			get
			{
				return this._isAdaptiveTriggersInherited;
			}
			set
			{
				this.SetProperty<bool>(ref this._isAdaptiveTriggersInherited, value, "IsAdaptiveTriggersInherited");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool IsAdaptiveTriggers
		{
			get
			{
				return this._isAdaptiveTriggers;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isAdaptiveTriggers, value, "IsAdaptiveTriggers"))
				{
					this.OnPropertyChanged("IsActivatorVisible");
					this.OnPropertyChanged("IsAnnotationShouldBeShownForMapping");
					this.OnPropertyChanged("IsAnnotationShouldBeShownForMappingWithoutShift");
				}
			}
		}

		public bool IsNotEmpty
		{
			get
			{
				return this.IsVirtualMappingPresent;
			}
		}

		public bool IsDescriptionPresent
		{
			get
			{
				return !string.IsNullOrWhiteSpace(this.Description);
			}
		}

		public bool IsOneKeyVirtualMappingPresent
		{
			get
			{
				BaseRewasdMapping mappedKey = this._mappedKey;
				return mappedKey != null && !mappedKey.IsNotMapped;
			}
		}

		public bool IsVirtualMappingPresent
		{
			get
			{
				return this.IsOneKeyVirtualMappingPresent || this.IsMacroMapping || this.IsRumble || this.IsJumpToShift;
			}
		}

		public bool IsVirtualMappingPresentSkipShift
		{
			get
			{
				return this.IsOneKeyVirtualMappingPresent || this.IsMacroMapping || this.IsRumble;
			}
		}

		public bool IsVirtualMappingPresentSkipRumble
		{
			get
			{
				return this.IsOneKeyVirtualMappingPresent || this.IsMacroMapping;
			}
		}

		public bool IsMacroMapping
		{
			get
			{
				if (this._macroSequence != null)
				{
					MacroSequence macroSequence = this.MacroSequence;
					return macroSequence != null && macroSequence.Count > 0;
				}
				return false;
			}
		}

		public bool HasMacroRumble
		{
			get
			{
				return this._macroSequence != null && this.MacroSequence.HasRumble;
			}
		}

		public bool IsOneKeyVirtualTouchpadTapMappingPresent
		{
			get
			{
				return this.IsOneKeyVirtualMappingPresent && this.MappedKey.IsVirtualTouchpad && !this.MappedKey.IsVirtualTouchpadSwipeOrZoom;
			}
		}

		public bool IsOneKeySwipeOrZoomMappingPresent
		{
			get
			{
				return this.IsOneKeyVirtualMappingPresent && this.MappedKey.IsVirtualTouchpadSwipeOrZoom;
			}
		}

		public bool IsGamepadMacroMappingPresent
		{
			get
			{
				if (!this.IsOneKeyVirtualMappingPresent || !this.MappedKey.IsGamepadOrGyroTilt)
				{
					if (this._macroSequence != null)
					{
						MacroSequence macroSequence = this.MacroSequence;
						if (macroSequence != null && macroSequence.Count > 0)
						{
							return this.MacroSequence.IsGamepadBindingPresent;
						}
					}
					return false;
				}
				return true;
			}
		}

		public bool IsGamepadStickMacroMappingPresent
		{
			get
			{
				if (!this.IsOneKeyVirtualMappingPresent || !this.MappedKey.IsGamepadStick)
				{
					if (this._macroSequence != null)
					{
						MacroSequence macroSequence = this.MacroSequence;
						if (macroSequence != null && macroSequence.Count > 0)
						{
							return this.MacroSequence.IsGamepadStickBindingPresent;
						}
					}
					return false;
				}
				return true;
			}
		}

		public bool IsVirtualTouchpadMacroMappingPresent
		{
			get
			{
				if (!this.IsOneKeyVirtualMappingPresent || !this.MappedKey.IsVirtualTouchpad)
				{
					if (this._macroSequence != null)
					{
						MacroSequence macroSequence = this.MacroSequence;
						if (macroSequence != null && macroSequence.Count > 0)
						{
							return this.MacroSequence.IsVirtualTouchpadBindingPresent;
						}
					}
					return false;
				}
				return true;
			}
		}

		public bool IsAnnotationShouldBeShownForMappingWithoutShift
		{
			get
			{
				return (this.MappedKey != null && !this.MappedKey.IsNotMapped) || this.IsMacroMapping || this.IsRumble || this.IsAdaptiveTriggers;
			}
		}

		public bool IsAnnotationShouldBeShownForMapping
		{
			get
			{
				return (this.MappedKey != null && !this.MappedKey.IsNotMapped) || this.IsMacroMapping || this.IsRumble || this.IsAdaptiveTriggers || (this.JumpToShift != -1 && !this.IsInheritedBinding);
			}
		}

		public bool IsAnnotationShouldBeShownForDescription
		{
			get
			{
				return !string.IsNullOrEmpty(this.Description);
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
				this.OnPropertyChanged("IsAnnotationShouldBeShownForMapping");
				this.OnPropertyChanged("IsVirtualMappingPresent");
				this.OnPropertyChanged("IsShowAnnotationAcordingShift");
				this.OnPropertyChanged("IsActivatorVisible");
				this.OnPropertyChanged("IsAnnotationShouldBeShownForMappingWithoutShift");
				this.OnPropertyChanged("IsNotEmpty");
			}
		}

		public bool IsToggle
		{
			get
			{
				if (!this.IsTurboOrToggleCanBeEnabled)
				{
					this._isToggle = false;
				}
				return this._isToggle;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isToggle, value, "IsToggle"))
				{
					this.MacroSequence.IsParentBindingToggle = value;
					if (value)
					{
						this.MacroSequence.MacroSequenceType = 1;
						this.MacroSequence.RepeatCount = 1;
						this.MacroSequenceAnnotation = this.GenerateAnnotationsFromMacroSequence();
						if (this.HostCollection.ShiftIndex != 0 && this.GetJumpToCurrentLayer().Contains(0))
						{
							XBBinding xbbindingByLayer = this.GetXBBindingByLayer(0, false, false);
							ActivatorXBBinding activatorXBBinding = ((xbbindingByLayer != null) ? xbbindingByLayer.ActivatorXBBindingDictionary[this.ActivatorType] : null);
							if (activatorXBBinding != null && activatorXBBinding.IsShiftToggle)
							{
								xbbindingByLayer.RemoveKeyBindingInActivator(this.ActivatorType);
							}
						}
					}
					this.HandleTurboToggleSwitchLogic();
					BaseXBBindingCollection hostCollection = this.HostCollection;
					bool flag;
					if (hostCollection == null)
					{
						flag = null != null;
					}
					else
					{
						SubConfigData subConfigData = hostCollection.SubConfigData;
						flag = ((subConfigData != null) ? subConfigData.ConfigData : null) != null;
					}
					if (flag)
					{
						this.HostCollection.SubConfigData.ConfigData.IsChanged = true;
					}
				}
			}
		}

		public uint TurboDelay
		{
			get
			{
				return this._turboDelay;
			}
			set
			{
				if (this.SetProperty<uint>(ref this._turboDelay, value, "TurboDelay"))
				{
					BaseXBBindingCollection hostCollection = this.HostCollection;
					bool flag;
					if (hostCollection == null)
					{
						flag = null != null;
					}
					else
					{
						SubConfigData subConfigData = hostCollection.SubConfigData;
						flag = ((subConfigData != null) ? subConfigData.ConfigData : null) != null;
					}
					if (flag)
					{
						this.HostCollection.SubConfigData.ConfigData.IsChanged = true;
					}
				}
			}
		}

		public bool IsTurbo
		{
			get
			{
				if (!this.IsTurboOrToggleCanBeEnabled)
				{
					this._isTurbo = false;
				}
				return this._isTurbo;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isTurbo, value, "IsTurbo"))
				{
					this.MacroSequence.IsParentBindingTurbo = value;
					if (value)
					{
						this.MacroSequence.MacroSequenceType = 1;
						this.MacroSequence.RepeatCount = 1;
						this.MacroSequenceAnnotation = this.GenerateAnnotationsFromMacroSequence();
					}
					this.HandleTurboToggleSwitchLogic();
					BaseXBBindingCollection hostCollection = this.HostCollection;
					bool flag;
					if (hostCollection == null)
					{
						flag = null != null;
					}
					else
					{
						SubConfigData subConfigData = hostCollection.SubConfigData;
						flag = ((subConfigData != null) ? subConfigData.ConfigData : null) != null;
					}
					if (flag)
					{
						this.HostCollection.SubConfigData.ConfigData.IsChanged = true;
					}
				}
			}
		}

		private void HandleTurboToggleSwitchLogic()
		{
			if (this.IsRumble)
			{
				return;
			}
			if (this._macroSequence != null && this.MacroSequence.Count > 0)
			{
				return;
			}
			if (this.IsTurbo || this.IsToggle)
			{
				if (this.MappedKey == KeyScanCodeV2.NoMap)
				{
					if (this.HostDictionary.HostXBBinding.ControllerButton.IsGamepad)
					{
						this.MappedKey = KeyScanCodeV2.FindKeyScanCodeByGamepadButton(this.GamepadButton);
						return;
					}
					if (this.HostDictionary.HostXBBinding.ControllerButton.IsKeyScanCode)
					{
						this.MappedKey = this.KeyScanCode;
					}
					return;
				}
			}
			else
			{
				KeyScanCodeV2 keyScanCodeV = this.MappedKey as KeyScanCodeV2;
				if (keyScanCodeV != null && this.GamepadButton == KeyScanCodeV2.FindGamepadButtonByKeyScanCode(keyScanCodeV))
				{
					this.MappedKey = KeyScanCodeV2.NoMap;
					return;
				}
			}
		}

		public bool IsTurboOrTogglePeripheralCheck
		{
			get
			{
				if (!App.LicensingService.IsTurboFeatureUnlocked && !App.LicensingService.IsToggleFeatureUnlocked)
				{
					return true;
				}
				BaseRewasdMapping mappedKey = this.MappedKey;
				if (mappedKey != null && mappedKey.IsNotMapped && !this.IsRumble)
				{
					if (this._macroSequence != null)
					{
						MacroSequence macroSequence = this.MacroSequence;
						if (macroSequence == null || macroSequence.Count != 0)
						{
							return true;
						}
					}
					if (!this.HostDictionary.HostXBBinding.ControllerButton.IsGamepad)
					{
						if (this.KeyScanCode == KeyScanCodeV2.NoMap && this.MappedKey == KeyScanCodeV2.NoMap)
						{
							return false;
						}
						KeyScanCodeV2 keyScanCode = this.KeyScanCode;
						if (keyScanCode != null && keyScanCode.IsBlacklist && this.MappedKey == KeyScanCodeV2.NoMap)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		public bool IsTurboOrToggleCanBeEnabledWithoutMapping
		{
			get
			{
				return (App.LicensingService.IsTurboFeatureUnlocked || App.LicensingService.IsToggleFeatureUnlocked) && (this.HostDictionary.HostXBBinding.ControllerButton.IsGamepad && !this.IsVirtualMappingPresentSkipRumble) && (GamepadButtonExtensions.IsAnyPaddle(this.GamepadButton) || GamepadButtonExtensions.IsAnyPhysicalTrackPad(this.GamepadButton) || GamepadButtonExtensions.IsAnyZone(this.GamepadButton) || GamepadButtonExtensions.IsAnyTriggerFullPullPress(this.GamepadButton));
			}
		}

		public bool IsToggleCanBeEnabled
		{
			get
			{
				return (!App.LicensingService.IsTurboFeatureUnlocked && !App.LicensingService.IsToggleFeatureUnlocked) || ((!this.IsShiftToggle || !this.IsJumpToShift) && this.IsTurboOrToggleCanBeEnabled);
			}
		}

		public bool IsTurboOrToggleCanBeEnabled
		{
			get
			{
				if (!App.LicensingService.IsTurboFeatureUnlocked && !App.LicensingService.IsToggleFeatureUnlocked)
				{
					return true;
				}
				BaseRewasdMapping mappedKey = this.MappedKey;
				if (mappedKey == null || !mappedKey.IsNotMapped)
				{
					if (!this.IsMappedKeyAnalog)
					{
						BaseRewasdMapping mappedKey2 = this.MappedKey;
						if (mappedKey2 == null || !mappedKey2.IsDoNotInherit)
						{
							BaseRewasdMapping mappedKey3 = this.MappedKey;
							if (mappedKey3 == null || !mappedKey3.IsUserCommand)
							{
								if (this._macroSequence != null && this.MacroSequence.Count > 0)
								{
									if (this.MacroSequence.FirstOrDefault((BaseMacro m) => m.IsBreak) != null)
									{
										return false;
									}
								}
								return true;
							}
						}
					}
					return false;
				}
				if (this._macroSequence != null && this.MacroSequence.Count > 0)
				{
					if (this.MacroSequence.FirstOrDefault((BaseMacro m) => m.IsBreak) != null)
					{
						return false;
					}
				}
				if (this._macroSequence != null && this.MacroSequence.Count > 0 && this.MacroSequence.IsHoldUntilRelease)
				{
					return false;
				}
				if (this.IsRumble)
				{
					return true;
				}
				if (this._macroSequence != null && this.MacroSequence.Count > 0 && this.MacroSequence.IsOnetime)
				{
					return true;
				}
				AssociatedControllerButton controllerButton = this.HostDictionary.HostXBBinding.ControllerButton;
				if (controllerButton != null && controllerButton.IsGamepad)
				{
					return !GamepadButtonExtensions.IsAnyPaddle(this.GamepadButton) && !GamepadButtonExtensions.IsAnyPhysicalTrackPad(this.GamepadButton) && MacroCompiler.IsVirtualGamepadDigitalButtonValid(this.GamepadButton, 1U);
				}
				if (this.KeyScanCode == KeyScanCodeV2.NoMap && this.MappedKey == KeyScanCodeV2.NoMap)
				{
					return false;
				}
				ActivatorXBBindingDictionary hostDictionary = this.HostDictionary;
				if (hostDictionary != null && hostDictionary.HostXBBinding.HostCollection.SubConfigData.IsPeripheral)
				{
					KeyScanCodeV2 keyScanCode = this.KeyScanCode;
					return keyScanCode == null || !keyScanCode.IsBlacklist || (!this.MappedKey.IsBlacklist && this.MappedKey != KeyScanCodeV2.NoMap);
				}
				return this.MappedKey != KeyScanCodeV2.NoMap;
			}
		}

		public bool IsMappedKeyAnalog
		{
			get
			{
				BaseRewasdMapping mappedKey = this.MappedKey;
				if (mappedKey == null || mappedKey.PCKeyCategory != 2)
				{
					BaseRewasdMapping mappedKey2 = this.MappedKey;
					return mappedKey2 != null && mappedKey2.PCKeyCategory == 9;
				}
				return true;
			}
		}

		public bool IsMouseToStick
		{
			get
			{
				return this.MappedKey.PCKeyCategory == 2 && !this.MappedKey.IsMouseFlickDirection && (GamepadButtonExtensions.GetGamepadButtonCategory(this.GamepadButton) == 3 || GamepadButtonExtensions.GetGamepadButtonCategory(this.GamepadButton) == 6);
			}
		}

		public bool IsMouseToTrackpad
		{
			get
			{
				return this.MappedKey.PCKeyCategory == 2 && !this.MappedKey.IsMouseFlickDirection && GamepadButtonExtensions.IsAnyPhysicalTrackPad(this.GamepadButton);
			}
		}

		public bool IsOverlayRadialMenuToTrackpad
		{
			get
			{
				return this.MappedKey.IsOverlayMenuCommand && GamepadButtonExtensions.IsAnyPhysicalTrackPad(this.GamepadButton);
			}
		}

		public bool IsMappedToNumKey
		{
			get
			{
				if (this.MappedKey == null)
				{
					return false;
				}
				string description = this.MappedKey.Description;
				if (description == null || !description.StartsWith("DIK_NUMPAD"))
				{
					string altDescription = this.MappedKey.AltDescription;
					return altDescription != null && altDescription.StartsWith("DIK_NUMPAD");
				}
				return true;
			}
		}

		public bool IsFlickStickToTrackpad
		{
			get
			{
				return this.MappedKey != null && this.MappedKey.IsMouseFlickDirection && GamepadButtonExtensions.IsAnyPhysicalTrackPad(this.GamepadButton);
			}
		}

		public ActivatorXBBinding(GamepadButton gb, ActivatorType at)
			: this(new AssociatedControllerButton
			{
				GamepadButton = gb
			}, at)
		{
		}

		public ActivatorXBBinding(KeyScanCodeV2 ksc, ActivatorType at)
			: this(new AssociatedControllerButton
			{
				KeyScanCode = ksc
			}, at)
		{
		}

		public ActivatorXBBinding(AssociatedControllerButton controllerButton, ActivatorType activatorType)
		{
			this._controllerButton = controllerButton;
			this._activatorType = activatorType;
			this._macroSequenceInit = true;
			this.InitShiftType();
		}

		public ActivatorXBBinding(GamepadButton gb, ActivatorType at, MacroSequence ms, BaseRewasdMapping mappedKey = null, string d = "")
			: this(new AssociatedControllerButton
			{
				GamepadButton = gb
			}, at, ms, mappedKey, d)
		{
		}

		public ActivatorXBBinding(GamepadButtonDescription gbd, ActivatorType at, MacroSequence ms, BaseRewasdMapping mappedKey = null, string d = "")
			: this(new AssociatedControllerButton
			{
				GamepadButtonDescription = gbd
			}, at, ms, mappedKey, d)
		{
		}

		public ActivatorXBBinding(KeyScanCodeV2 ksc, ActivatorType at, MacroSequence ms, BaseRewasdMapping mappedKey = null, string d = "")
			: this(new AssociatedControllerButton
			{
				KeyScanCode = ksc
			}, at, ms, mappedKey, d)
		{
		}

		public ActivatorXBBinding(AssociatedControllerButton controllerButton, ActivatorType activatorType, MacroSequence macroSequence, BaseRewasdMapping mappedKey = null, string description = "")
			: this(controllerButton, activatorType)
		{
			this.MappedKeyBytes = 0;
			if (macroSequence != null && macroSequence.Count > 0)
			{
				this.MacroSequence = macroSequence;
				this.MacroSequence.SupressOnCollectionChanged = false;
			}
			if (mappedKey != null)
			{
				KeyScanCodeV2 keyScanCodeV = mappedKey as KeyScanCodeV2;
				if (keyScanCodeV != null)
				{
					this.MappedKeyBytes = (ushort)KeyScanCodeV2.GetIndex(keyScanCodeV);
					this.MappedKey = KeyScanCodeV2.SCAN_CODE_TABLE[(int)this.MappedKeyBytes];
				}
				else
				{
					BaseRewasdUserCommand baseRewasdUserCommand = mappedKey as BaseRewasdUserCommand;
					if (baseRewasdUserCommand != null)
					{
						this.MappedKey = baseRewasdUserCommand;
					}
					else
					{
						this.MappedKey = KeyScanCodeV2.NoMap;
					}
				}
			}
			else
			{
				this.MappedKey = KeyScanCodeV2.NoMap;
			}
			this.Description = description;
		}

		public override void Dispose()
		{
			base.Dispose();
			this._mappedKey = null;
			this._controllerButton = null;
			if (this._macroSequence != null)
			{
				this.MacroSequence.Dispose();
				this.MacroSequence = null;
			}
			this.HostDictionary = null;
			if (this._macroSequenceAnnotation != null)
			{
				this.MacroSequenceAnnotation.Clear();
				this.MacroSequenceAnnotation = null;
			}
		}

		public void ClearVirtualMapping()
		{
			if (this.IsGamepadMacroMappingPresent)
			{
				this.MappedKey = KeyScanCodeV2.NoMap;
				this.MacroSequence.Clear();
				this.IsToggle = false;
				this.IsTurbo = false;
			}
		}

		public void InitShiftType()
		{
			if (this.ActivatorType == 4 || this.ActivatorType == 5)
			{
				this._shiftTypeForUnsetShift = 1;
			}
		}

		public void RefreshAnnotations()
		{
			if (this.MacroSequenceAnnotation.Count == 0 && this._macroSequence != null && this.MacroSequence.Count == 0)
			{
				return;
			}
			SortableObservableCollection<BaseMacroAnnotation> sortableObservableCollection = this.GenerateAnnotationsFromMacroSequence();
			if (!sortableObservableCollection.SequenceEqual(this.MacroSequenceAnnotation))
			{
				this.MacroSequenceAnnotation = sortableObservableCollection;
			}
		}

		public void OnShowMappingsViewChanged()
		{
			this.OnPropertyChanged("IsAnnotationShouldBeShownForMapping");
			this.OnPropertyChanged("IsAnnotationShouldBeShownForMappingWithoutShift");
			this.OnPropertyChanged("IsActivatorVisible");
			this.OnPropertyChanged("IsShowAnnotationAcordingShift");
		}

		public void ClearMacroSequence(bool force = false)
		{
			if (this.MacroSequence.Count == 0)
			{
				return;
			}
			if (force)
			{
				this.MacroSequence.Clear();
			}
			else
			{
				this.MacroSequence.ClearCommand.Execute();
			}
			if (this.MappedKey == KeyScanCodeV2.NoMap && !this.IsRumble)
			{
				this.IsToggle = false;
				this.IsTurbo = false;
			}
			if (this.MacroSequence.Count == 0)
			{
				this.MacroSequenceAnnotation.Clear();
				this.OnPropertyChanged("MacroSequenceAnnotation");
				this.OnPropertyChanged("IsMacroSequenceChanged");
				this.OnPropertyChanged("IsMacroMapping");
				this.OnPropertyChanged("IsTurbo");
				this.OnPropertyChanged("IsToggle");
				this.OnPropertyChanged("IsActivatorVisible");
				this.OnPropertyChanged("IsAnnotationShouldBeShownForMapping");
				this.OnPropertyChanged("IsAnnotationShouldBeShownForMappingWithoutShift");
				this.OnPropertyChanged("IsTurboOrToggleCanBeEnabled");
				this.OnPropertyChanged("IsToggleCanBeEnabled");
				this.OnPropertyChanged("IsTurboOrToggleCanBeEnabledWithoutMapping");
				this.OnPropertyChanged("IsTurboOrTogglePeripheralCheck");
				this.OnPropertyChanged("IsVirtualMappingPresent");
				this.OnPropertyChanged("IsVirtualMappingPresentSkipRumble");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public SortableObservableCollection<BaseMacroAnnotation> MacroSequenceAnnotation
		{
			get
			{
				if (this._macroSequenceAnnotation == null)
				{
					this.MacroSequenceAnnotation = this.GenerateAnnotationsFromMacroSequence();
				}
				return this._macroSequenceAnnotation;
			}
			set
			{
				this.SetProperty<SortableObservableCollection<BaseMacroAnnotation>>(ref this._macroSequenceAnnotation, value, "MacroSequenceAnnotation");
			}
		}

		public void ClearMacroSequence()
		{
			if (this._macroSequence == null)
			{
				return;
			}
			this.MacroSequence.Clear();
			this.IsMacroSequenceChanged = true;
		}

		public MacroSequence MacroSequence
		{
			get
			{
				this.CheckAndInitMacroSequence();
				return this._macroSequence;
			}
			private set
			{
				this._macroSequenceInit = false;
				if (this._macroSequence != null)
				{
					this._macroSequence.CollectionChanged -= this.MacroSequenceOnCollectionChanged;
					this._macroSequence.MacroSequencePropertyChanged -= this.MacroSequencePropertyChanged;
				}
				if (this.SetProperty<MacroSequence>(ref this._macroSequence, value, "MacroSequence"))
				{
					if (this._macroSequence != null)
					{
						this._macroSequence.CollectionChanged += this.MacroSequenceOnCollectionChanged;
						this._macroSequence.MacroSequencePropertyChanged += this.MacroSequencePropertyChanged;
					}
					this.OnPropertyChanged("IsMacroMapping");
					this.OnPropertyChanged("IsVirtualMappingPresent");
					this.OnPropertyChanged("IsVirtualMappingPresentSkipRumble");
				}
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool IsMacroSequenceChanged
		{
			get
			{
				return this._isMacroSequenceChanged;
			}
			set
			{
				this.SetProperty<bool>(ref this._isMacroSequenceChanged, value, "IsMacroSequenceChanged");
				if (this._isMacroSequenceChanged)
				{
					this.OnPropertyChangedExtended(this, new PropertyChangedExtendedEventArgs("MacroSequence", typeof(MacroSequence), -1, -1));
				}
			}
		}

		public void SetIsRumbleLocal(bool leftMotor, bool rightMotor, bool leftTrigger, bool rightTrigger)
		{
			this._isRumbleLeftTrigger = leftTrigger;
			this._isRumbleRightTrigger = rightTrigger;
			this._isRumbleLeftMotor = leftMotor;
			this._isRumbleRightMotor = rightMotor;
			this.OnPropertyChanged("IsRumbleLeftTrigger");
			this.OnPropertyChanged("IsRumbleRightTrigger");
			this.OnPropertyChanged("IsRumbleLeftMotor");
			this.OnPropertyChanged("IsRumbleRightMotor");
			this.OnPropertyChanged("IsRumble");
		}

		public void SetIsTurboLocal(bool value)
		{
			this._isTurbo = value;
			this.OnPropertyChanged("IsTurbo");
		}

		public void SetIsToggleLocal(bool value)
		{
			this._isToggle = value;
			this.OnPropertyChanged("IsToggle");
		}

		private void MacroSequenceOnCollectionChanged(object s, NotifyCollectionChangedEventArgs e)
		{
			if (((MacroSequence)s).SupressOnCollectionChanged)
			{
				return;
			}
			if (e.Action != NotifyCollectionChangedAction.Reset)
			{
				this.IsMacroSequenceChanged = true;
			}
			this.OnPropertyChanged("IsMacroMapping");
			this.MappedKey = KeyScanCodeV2.NoMap;
			if (e.Action != NotifyCollectionChangedAction.Reset)
			{
				this.OnPropertyChangedExtended(s, new PropertyChangedExtendedEventArgs("MacroSequence", typeof(MacroSequence), e.NewStartingIndex, e.OldStartingIndex));
			}
			else if (this.HostCollection.SubConfigData.ConfigData.IsVirtualMappingExist)
			{
				this.HostCollection.SubConfigData.ConfigData.CheckVirtualMappingsExist();
			}
			if (e.Action == NotifyCollectionChangedAction.Add && ((BaseMacro)e.NewItems[0]).IsVirtualGamepadBinding)
			{
				this.HostCollection.SubConfigData.ConfigData.IsVirtualMappingExist = true;
			}
			if (e.Action == NotifyCollectionChangedAction.Remove && this.HostCollection.SubConfigData.ConfigData.IsVirtualMappingExist)
			{
				bool flag = false;
				foreach (object obj in e.OldItems)
				{
					flag |= ((BaseMacro)obj).IsVirtualGamepadBinding;
				}
				if (flag)
				{
					this.HostCollection.SubConfigData.ConfigData.CheckVirtualMappingsExist();
				}
			}
			this.OnPropertyChanged("IsTurboOrToggleCanBeEnabled");
			this.OnPropertyChanged("IsToggleCanBeEnabled");
			this.OnPropertyChanged("IsTurboOrToggleCanBeEnabledWithoutMapping");
			this.OnPropertyChanged("IsTurboOrTogglePeripheralCheck");
			this.OnPropertyChanged("IsActivatorVisible");
			this.OnPropertyChanged("IsAnnotationShouldBeShownForMapping");
			this.OnPropertyChanged("IsAnnotationShouldBeShownForMappingWithoutShift");
			this.OnPropertyChanged("IsVirtualMappingPresent");
			BaseXBBindingCollection hostCollection = this.HostCollection;
			bool flag2;
			if (hostCollection == null)
			{
				flag2 = null != null;
			}
			else
			{
				SubConfigData subConfigData = hostCollection.SubConfigData;
				flag2 = ((subConfigData != null) ? subConfigData.ConfigData : null) != null;
			}
			if (flag2)
			{
				this.HostCollection.SubConfigData.ConfigData.IsChanged = true;
			}
			this.RefreshAnnotations();
		}

		private void MacroSequencePropertyChanged(object s, PropertyChangedEventArgs e)
		{
			this.IsMacroSequenceChanged = this.MacroSequence.IsChanged;
		}

		[JsonProperty("RumbleLeftTriggerSpeed")]
		public ushort RumbleLeftTriggerSpeed
		{
			get
			{
				return this._rumbleLeftTriggerSpeed;
			}
			set
			{
				this.SetProperty<ushort>(ref this._rumbleLeftTriggerSpeed, value, "RumbleLeftTriggerSpeed");
			}
		}

		[JsonProperty("RumbleRightTriggerSpeed")]
		public ushort RumbleRightTriggerSpeed
		{
			get
			{
				return this._rumbleRightTriggerSpeed;
			}
			set
			{
				this.SetProperty<ushort>(ref this._rumbleRightTriggerSpeed, value, "RumbleRightTriggerSpeed");
			}
		}

		[JsonProperty("RumbleLeftMotorSpeed")]
		public ushort RumbleLeftMotorSpeed
		{
			get
			{
				return this._rumbleLeftMotorSpeed;
			}
			set
			{
				this.SetProperty<ushort>(ref this._rumbleLeftMotorSpeed, value, "RumbleLeftMotorSpeed");
			}
		}

		public ushort RumbleRightMotorSpeed
		{
			get
			{
				return this._rumbleRightMotorSpeed;
			}
			set
			{
				this.SetProperty<ushort>(ref this._rumbleRightMotorSpeed, value, "RumbleRightMotorSpeed");
			}
		}

		[JsonProperty("RumbleDuration")]
		public uint RumbleDuration
		{
			get
			{
				return this._rumbleDuration;
			}
			set
			{
				this.SetProperty<uint>(ref this._rumbleDuration, value, "RumbleDuration");
			}
		}

		public bool IsRumble
		{
			get
			{
				return this.IsRumbleLeftTrigger || this.IsRumbleRightTrigger || this.IsRumbleLeftMotor || this.IsRumbleRightMotor;
			}
			set
			{
				this.IsRumbleLeftTrigger = value;
				this.IsRumbleRightTrigger = value;
				this.IsRumbleLeftMotor = value;
				this.IsRumbleRightMotor = value;
			}
		}

		[JsonProperty("IsRumbleLeftTrigger")]
		public bool IsRumbleLeftTrigger
		{
			get
			{
				return this._isRumbleLeftTrigger;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isRumbleLeftTrigger, value, "IsRumbleLeftTrigger"))
				{
					this.RaiseOnPropertyChangedIndividualRumbleDependantProperties();
					if (!this.IsRumble && this.MappedKey == KeyScanCodeV2.NoMap)
					{
						if (this._macroSequence != null)
						{
							MacroSequence macroSequence = this.MacroSequence;
							if (macroSequence == null || macroSequence.Count != 0)
							{
								return;
							}
						}
						this.IsTurbo = false;
						this.IsToggle = false;
					}
				}
			}
		}

		[JsonProperty("IsRumbleRightTrigger")]
		public bool IsRumbleRightTrigger
		{
			get
			{
				return this._isRumbleRightTrigger;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isRumbleRightTrigger, value, "IsRumbleRightTrigger"))
				{
					this.RaiseOnPropertyChangedIndividualRumbleDependantProperties();
					if (!this.IsRumble && this.MappedKey == KeyScanCodeV2.NoMap)
					{
						if (this._macroSequence != null)
						{
							MacroSequence macroSequence = this.MacroSequence;
							if (macroSequence == null || macroSequence.Count != 0)
							{
								return;
							}
						}
						this.IsTurbo = false;
						this.IsToggle = false;
					}
				}
			}
		}

		[JsonProperty("IsRumbleLeftMotor")]
		public bool IsRumbleLeftMotor
		{
			get
			{
				return this._isRumbleLeftMotor;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isRumbleLeftMotor, value, "IsRumbleLeftMotor"))
				{
					this.RaiseOnPropertyChangedIndividualRumbleDependantProperties();
					if (!this.IsRumble && this.MappedKey == KeyScanCodeV2.NoMap)
					{
						if (this._macroSequence != null)
						{
							MacroSequence macroSequence = this.MacroSequence;
							if (macroSequence == null || macroSequence.Count != 0)
							{
								return;
							}
						}
						this.IsTurbo = false;
						this.IsToggle = false;
					}
				}
			}
		}

		[JsonProperty("IsRumbleRightMotor")]
		public bool IsRumbleRightMotor
		{
			get
			{
				return this._isRumbleRightMotor;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isRumbleRightMotor, value, "IsRumbleRightMotor"))
				{
					this.RaiseOnPropertyChangedIndividualRumbleDependantProperties();
					if (!this.IsRumble && this.MappedKey == KeyScanCodeV2.NoMap)
					{
						if (this._macroSequence != null)
						{
							MacroSequence macroSequence = this.MacroSequence;
							if (macroSequence == null || macroSequence.Count != 0)
							{
								return;
							}
						}
						this.IsTurbo = false;
						this.IsToggle = false;
					}
				}
			}
		}

		private void RaiseOnPropertyChangedIndividualRumbleDependantProperties()
		{
			this.OnPropertyChanged("IsVirtualMappingPresent");
			this.OnPropertyChanged("IsActivatorVisible");
			this.OnPropertyChanged("IsAnnotationShouldBeShownForMapping");
			this.OnPropertyChanged("IsAnnotationShouldBeShownForMappingWithoutShift");
			this.OnPropertyChanged("IsTurboOrToggleCanBeEnabled");
			this.OnPropertyChanged("IsToggleCanBeEnabled");
			this.OnPropertyChanged("IsTurboOrToggleCanBeEnabledWithoutMapping");
			this.OnPropertyChanged("IsTurboOrTogglePeripheralCheck");
			this.OnPropertyChanged("IsTurbo");
			this.OnPropertyChanged("IsToggle");
			this.OnPropertyChanged("IsRumble");
		}

		public bool CanSave(bool verbose, ref bool errorIsShown)
		{
			return this._macroSequence == null || this.MacroSequence.CanSaveChanges(verbose, ref errorIsShown);
		}

		public bool SaveChanges(bool verbose, ref bool errorIsShown)
		{
			return this._macroSequence == null || this.MacroSequence.SaveChanges(verbose, ref errorIsShown);
		}

		private void ShowWarningAboutCopyMappings()
		{
			XBBinding shiftXBBinding = this.ShiftXBBinding;
			if (shiftXBBinding != null && shiftXBBinding.IsAnyActivatorVirtualMappingPresentSkipShift)
			{
				MessageBoxWithDoNotShowLogic.Show(Application.Current.MainWindow, DTLocalization.GetString(12415), MessageBoxButton.OK, MessageBoxImage.Asterisk, "ConfirmSyncMappings", MessageBoxResult.OK, false, 0.0, null, null, null, null, null, null);
			}
		}

		public void CopyMappingFrom(ActivatorXBBinding from)
		{
			new ActivatorXBBinding(from._controllerButton, from._activatorType);
			if (from._macroSequence != null && from._macroSequence.Count > 0)
			{
				this.MacroSequence = from._macroSequence.Clone(null);
			}
			else
			{
				this.MacroSequence.Clear();
			}
			this.IsRumbleLeftTrigger = from.IsRumbleLeftTrigger;
			this.IsRumbleRightTrigger = from.IsRumbleRightTrigger;
			this.IsRumbleLeftMotor = from.IsRumbleLeftMotor;
			this.IsRumbleRightMotor = from.IsRumbleRightMotor;
			this.RumbleDuration = from.RumbleDuration;
			this.RumbleLeftMotorSpeed = from.RumbleLeftMotorSpeed;
			this.RumbleLeftTriggerSpeed = from.RumbleLeftTriggerSpeed;
			this.RumbleRightMotorSpeed = from.RumbleRightMotorSpeed;
			this.RumbleRightTriggerSpeed = from.RumbleRightTriggerSpeed;
			this.IsToggle = from.IsToggle;
			this.IsTurbo = from.IsTurbo;
			this.TurboDelay = from.TurboDelay;
			this.Description = from.Description;
			this.MappedKey = from.MappedKey;
			this.MappedKeyBytes = from.MappedKeyBytes;
			this.MacroSequenceAnnotation = null;
		}

		public ActivatorXBBinding Clone(AssociatedControllerButton controllerButton = null)
		{
			ActivatorXBBinding activatorXBBinding = ((controllerButton != null) ? new ActivatorXBBinding(controllerButton, this._activatorType) : new ActivatorXBBinding(this._controllerButton, this._activatorType));
			if (this._macroSequence != null)
			{
				activatorXBBinding.MacroSequence = ((controllerButton != null) ? this._macroSequence.Clone(controllerButton) : this._macroSequence.Clone(null));
			}
			activatorXBBinding._isRumbleLeftTrigger = this._isRumbleLeftTrigger;
			activatorXBBinding._isRumbleRightTrigger = this._isRumbleRightTrigger;
			activatorXBBinding._isRumbleLeftMotor = this._isRumbleLeftMotor;
			activatorXBBinding._isRumbleRightMotor = this._isRumbleRightMotor;
			activatorXBBinding._rumbleDuration = this._rumbleDuration;
			activatorXBBinding._rumbleLeftMotorSpeed = this._rumbleLeftMotorSpeed;
			activatorXBBinding._rumbleLeftTriggerSpeed = this._rumbleLeftTriggerSpeed;
			activatorXBBinding._rumbleRightMotorSpeed = this._rumbleRightMotorSpeed;
			activatorXBBinding._rumbleRightTriggerSpeed = this._rumbleRightTriggerSpeed;
			activatorXBBinding._isToggle = this._isToggle;
			activatorXBBinding._isTurbo = this._isTurbo;
			activatorXBBinding._turboDelay = this._turboDelay;
			activatorXBBinding._description = this._description;
			activatorXBBinding._jumpToShift = this._jumpToShift;
			activatorXBBinding._delayBerforeJump = this._delayBerforeJump;
			activatorXBBinding._isDelayBerforeJumpChecked = this._isDelayBerforeJumpChecked;
			activatorXBBinding._mappedKey = this._mappedKey;
			activatorXBBinding._mappedKeyBytes = this._mappedKeyBytes;
			activatorXBBinding._isPostponeMapping = this._isPostponeMapping;
			return activatorXBBinding;
		}

		public void CopyFromModel(ActivatorXBBinding model)
		{
			if (model == null)
			{
				return;
			}
			this._activatorType = model.ActivatorType;
			this._controllerButton.CopyFromModel(model.ControllerButton);
			this.IsRumbleLeftTrigger = model.IsRumbleLeftTrigger;
			this.IsRumbleRightTrigger = model.IsRumbleRightTrigger;
			this.IsRumbleLeftMotor = model.IsRumbleLeftMotor;
			this.IsRumbleRightMotor = model.IsRumbleRightMotor;
			this.RumbleDuration = model.RumbleDuration;
			this.RumbleLeftMotorSpeed = model.RumbleLeftMotorSpeed;
			this.RumbleLeftTriggerSpeed = model.RumbleLeftTriggerSpeed;
			this.RumbleRightMotorSpeed = model.RumbleRightMotorSpeed;
			this.RumbleRightTriggerSpeed = model.RumbleRightTriggerSpeed;
			this.TurboDelay = model.TurboDelay;
			this.Description = model.Description;
			if (!model.MappedKey.IsDoNotInherit || this.HostCollection.IsShiftCollection)
			{
				this.MappedKey = model.MappedKey;
				this.MappedKeyBytes = model.MappedKeyBytes;
			}
			this._jumpToShift = model.JumpToShift;
			this._delayBerforeJump = model.DelayBerforeJump;
			this._isDelayBerforeJumpChecked = model.IsDelayBerforeJumpChecked;
			this._isPostponeMapping = model.IsPostponeMapping;
			if (this._jumpToShift == this.HostCollection.ShiftIndex)
			{
				this._jumpToShift = -1;
				this.ResetDelayBeforeJump();
			}
			if (model.MacroSequence.Any<BaseMacro>())
			{
				this.MacroSequence.CopyFromModel(model.MacroSequence);
			}
			this.IsToggle = model.IsToggle;
			this.IsTurbo = model.IsTurbo;
		}

		public void CopyToModel(ActivatorXBBinding model)
		{
			model.ActivatorType = this.ActivatorType;
			this._controllerButton.CopyToModel(model.ControllerButton);
			model.IsRumbleLeftTrigger = this._isRumbleLeftTrigger;
			model.IsRumbleRightTrigger = this._isRumbleRightTrigger;
			model.IsRumbleLeftMotor = this._isRumbleLeftMotor;
			model.IsRumbleRightMotor = this._isRumbleRightMotor;
			model.RumbleDuration = this._rumbleDuration;
			model.RumbleLeftMotorSpeed = this._rumbleLeftMotorSpeed;
			model.RumbleLeftTriggerSpeed = this._rumbleLeftTriggerSpeed;
			model.RumbleRightMotorSpeed = this._rumbleRightMotorSpeed;
			model.RumbleRightTriggerSpeed = this._rumbleRightTriggerSpeed;
			model.TurboDelay = this._turboDelay;
			model.Description = this._description;
			model.JumpToShift = this._jumpToShift;
			model.DelayBerforeJump = this.DelayBerforeJump;
			model.IsDelayBerforeJumpChecked = this.IsDelayBerforeJumpChecked;
			model.IsPostponeMapping = this.IsPostponeMapping;
			if (this._mappedKey != null)
			{
				model.MappedKey = this._mappedKey;
			}
			model.MappedKeyBytes = this._mappedKeyBytes;
			if (this.MacroSequence.Any<BaseMacro>())
			{
				this.MacroSequence.CopyToModel(model.MacroSequence);
			}
			model.IsToggle = this._isToggle;
			model.IsTurbo = this._isTurbo;
		}

		private bool IsSeparatorAllowed(BaseMacroAnnotation annotation)
		{
			return !annotation.IsSeparatorAnnotation && !annotation.IsPlusAnnotation && !annotation.IsBreakAnnotation && !annotation.IsParenthesesOpenAnnotation && !annotation.IsPauseAnnotation && !annotation.IsRepeatAnnotation;
		}

		private bool IsSeparator(BaseMacroAnnotation annotation)
		{
			return annotation.IsSeparatorAnnotation || annotation.IsPlusAnnotation;
		}

		private int KeyAnnotationGetLastIndex(ref SortableObservableCollection<BaseMacroAnnotation> annotations, KeyScanCodeV2 keyScanCode)
		{
			BaseMacroAnnotation baseMacroAnnotation = annotations.LastOrDefault((BaseMacroAnnotation b) => b.IsKeyAnnotation && ((MacroKeyAnnotation)b)._key == keyScanCode);
			if (baseMacroAnnotation != null)
			{
				return annotations.IndexOf(baseMacroAnnotation);
			}
			BaseMacroAnnotation baseMacroAnnotation2 = annotations.LastOrDefault((BaseMacroAnnotation b) => b.IsGamepadAnnotation && ((MacroGamepadAnnotation)b).Key == keyScanCode);
			if (baseMacroAnnotation2 != null)
			{
				return annotations.IndexOf(baseMacroAnnotation2);
			}
			return -1;
		}

		private void ArchivePause(ref SortableObservableCollection<BaseMacroAnnotation> annotations, ref ObservableCollection<BaseMacroBinding> downKeysCollection, ref ObservableCollection<BaseMacroBinding> downKeysCollectionForPause, ref ObservableCollection<BaseMacroBinding> upKeysCollectionForPause, bool rumble = false)
		{
			if (downKeysCollectionForPause.Count >= 1 && downKeysCollection.Count > 0 && downKeysCollection.Count < 2)
			{
				BaseMacroBinding baseMacroBinding = downKeysCollectionForPause[0];
				int num = this.KeyAnnotationGetLastIndex(ref annotations, baseMacroBinding.KeyScanCode);
				if (num >= 0)
				{
					annotations.Insert(num, new MacroParenthesesOpenAnnotation());
				}
			}
			if (downKeysCollection.Count > 0)
			{
				if (annotations.Count > 0 && annotations[annotations.Count - 1].IsKeyAnnotation)
				{
					annotations.Add(new MacroPlusAnnotation());
				}
				if (downKeysCollection.Count > 1)
				{
					annotations.Add(new MacroParenthesesOpenAnnotation());
				}
				else if (annotations.Count > 0 && this.IsSeparatorAllowed(annotations[annotations.Count - 1]))
				{
					annotations.Add(new MacroSeparatorAnnotation());
				}
				if (downKeysCollection[0].IsGamepadBinding)
				{
					annotations.Add(new MacroGamepadAnnotation(downKeysCollection[0].KeyScanCode));
				}
				else
				{
					annotations.Add(new MacroKeyAnnotation(downKeysCollection[0].KeyScanCode));
				}
				if (downKeysCollection.Count > 1)
				{
					for (int i = 1; i < downKeysCollection.Count; i++)
					{
						annotations.Add(new MacroPlusAnnotation());
						if (downKeysCollection[i].IsGamepadBinding)
						{
							annotations.Add(new MacroGamepadAnnotation(downKeysCollection[i].KeyScanCode));
						}
						else
						{
							annotations.Add(new MacroKeyAnnotation(downKeysCollection[i].KeyScanCode));
						}
					}
					annotations.Add(new MacroParenthesesCloseAnnotation());
				}
				if (downKeysCollectionForPause.Count >= 1 && downKeysCollection.Count > 0 && downKeysCollection.Count < 2)
				{
					annotations.Add(new MacroParenthesesCloseAnnotation());
				}
				annotations.Add(new MacroTimerAnnotation());
				if (rumble)
				{
					annotations.Add(new MacroRumbleAnnotation());
				}
				if (upKeysCollectionForPause.Count > 0)
				{
					int num2 = -1;
					bool flag = false;
					if (downKeysCollectionForPause.Count >= 1)
					{
						BaseMacroBinding baseMacroBinding2 = downKeysCollectionForPause[0];
						num2 = this.KeyAnnotationGetLastIndex(ref annotations, baseMacroBinding2.KeyScanCode);
						if (num2 >= 0 && num2 > 0 && annotations[num2 - 1].IsParenthesesOpenAnnotation)
						{
							num2--;
						}
					}
					if (num2 >= 0)
					{
						flag = true;
					}
					if (annotations.Count > 0 && !this.IsSeparator(annotations[annotations.Count - 1]))
					{
						if (!flag)
						{
							annotations.Add(new MacroPlusAnnotation());
						}
						else
						{
							annotations.Insert(num2, new MacroPlusAnnotation());
						}
					}
					if (upKeysCollectionForPause.Count > 1)
					{
						if (!flag)
						{
							annotations.Add(new MacroParenthesesOpenAnnotation());
						}
						else
						{
							annotations.Insert(num2, new MacroParenthesesOpenAnnotation());
							num2++;
						}
					}
					int num3 = 0;
					if (upKeysCollectionForPause[0] != null)
					{
						num3 = this.KeyAnnotationGetLastIndex(ref annotations, upKeysCollectionForPause[0].KeyScanCode);
					}
					if (num3 >= 0)
					{
						annotations.RemoveAt(num3);
						if (num3 < annotations.Count - 1 && this.IsSeparator(annotations[num3]))
						{
							annotations.RemoveAt(num3);
						}
					}
					if (upKeysCollectionForPause[0] != null)
					{
						if (!flag)
						{
							if (upKeysCollectionForPause[0].IsGamepadBinding)
							{
								annotations.Add(new MacroGamepadAnnotation(upKeysCollectionForPause[0].KeyScanCode));
							}
							else
							{
								annotations.Add(new MacroKeyAnnotation(upKeysCollectionForPause[0].KeyScanCode));
							}
						}
						else
						{
							if (upKeysCollectionForPause[0].IsGamepadBinding)
							{
								annotations.Insert(num2, new MacroGamepadAnnotation(upKeysCollectionForPause[0].KeyScanCode));
							}
							else
							{
								annotations.Insert(num2, new MacroKeyAnnotation(upKeysCollectionForPause[0].KeyScanCode));
							}
							num2++;
						}
					}
					if (upKeysCollectionForPause.Count > 1)
					{
						for (int j = 1; j < upKeysCollectionForPause.Count; j++)
						{
							num3 = 0;
							if (upKeysCollectionForPause[j] != null)
							{
								num3 = this.KeyAnnotationGetLastIndex(ref annotations, upKeysCollectionForPause[j].KeyScanCode);
							}
							if (num3 >= 0)
							{
								annotations.RemoveAt(num3);
								if (num3 < annotations.Count - 1 && this.IsSeparator(annotations[num3]))
								{
									annotations.RemoveAt(num3);
								}
							}
							if (upKeysCollectionForPause[j] != null)
							{
								if (!flag)
								{
									annotations.Add(new MacroSeparatorAnnotation());
									if (upKeysCollectionForPause[j].IsGamepadBinding)
									{
										annotations.Add(new MacroGamepadAnnotation(upKeysCollectionForPause[j].KeyScanCode));
									}
									else
									{
										annotations.Add(new MacroKeyAnnotation(upKeysCollectionForPause[j].KeyScanCode));
									}
								}
								else
								{
									annotations.Insert(num2, new MacroSeparatorAnnotation());
									num2++;
									if (upKeysCollectionForPause[j].IsGamepadBinding)
									{
										annotations.Insert(num2, new MacroGamepadAnnotation(upKeysCollectionForPause[j].KeyScanCode));
									}
									else
									{
										annotations.Insert(num2, new MacroKeyAnnotation(upKeysCollectionForPause[j].KeyScanCode));
									}
									num2++;
								}
							}
						}
						if (!flag)
						{
							annotations.Add(new MacroParenthesesCloseAnnotation());
							return;
						}
						annotations.Insert(num2, new MacroParenthesesCloseAnnotation());
						return;
					}
				}
			}
			else
			{
				if (rumble)
				{
					if (annotations.Count > 0 && this.IsSeparatorAllowed(annotations[annotations.Count - 1]))
					{
						annotations.Add(new MacroSeparatorAnnotation());
					}
					annotations.Add(new MacroRumbleAnnotation());
					return;
				}
				if (annotations.Count > 0 && this.IsSeparator(annotations[annotations.Count - 1]))
				{
					annotations.RemoveAt(annotations.Count - 1);
				}
				if (downKeysCollectionForPause.Count == 0)
				{
					annotations.Add(new MacroPauseAnnotation());
					return;
				}
				if (annotations.Count > 0 && !this.IsSeparator(annotations[annotations.Count - 1]))
				{
					annotations.Add(new MacroPlusAnnotation());
				}
				if (downKeysCollectionForPause.Count > 1)
				{
					annotations.Add(new MacroParenthesesOpenAnnotation());
				}
				int num4 = 0;
				if (downKeysCollectionForPause[0] != null)
				{
					num4 = this.KeyAnnotationGetLastIndex(ref annotations, downKeysCollectionForPause[0].KeyScanCode);
				}
				if (num4 >= 0)
				{
					annotations.RemoveAt(num4);
					if (num4 < annotations.Count - 1 && this.IsSeparator(annotations[num4]))
					{
						annotations.RemoveAt(num4);
					}
				}
				if (downKeysCollectionForPause[0] != null)
				{
					if (downKeysCollectionForPause[0].IsGamepadBinding)
					{
						annotations.Add(new MacroGamepadAnnotation(downKeysCollectionForPause[0].KeyScanCode));
					}
					else
					{
						annotations.Add(new MacroKeyAnnotation(downKeysCollectionForPause[0].KeyScanCode));
					}
				}
				if (downKeysCollectionForPause.Count > 1)
				{
					for (int k = 1; k < downKeysCollectionForPause.Count; k++)
					{
						num4 = 0;
						if (downKeysCollectionForPause[k] != null)
						{
							num4 = this.KeyAnnotationGetLastIndex(ref annotations, downKeysCollectionForPause[k].KeyScanCode);
						}
						if (num4 >= 0)
						{
							annotations.RemoveAt(num4);
							if (num4 < annotations.Count - 1 && this.IsSeparator(annotations[num4]))
							{
								annotations.RemoveAt(num4);
							}
						}
						if (downKeysCollectionForPause[k] != null)
						{
							annotations.Add(new MacroPlusAnnotation());
							if (downKeysCollectionForPause[k].IsGamepadBinding)
							{
								annotations.Add(new MacroGamepadAnnotation(downKeysCollectionForPause[k].KeyScanCode));
							}
							else
							{
								annotations.Add(new MacroKeyAnnotation(downKeysCollectionForPause[k].KeyScanCode));
							}
						}
					}
					annotations.Add(new MacroParenthesesCloseAnnotation());
				}
				annotations.Add(new MacroTimerAnnotation());
			}
		}

		private bool TryArchiveMacros(ref SortableObservableCollection<BaseMacroAnnotation> annotations)
		{
			bool isTurbo = this.IsTurbo;
			bool isToggle = this.IsToggle;
			int num = this.MacroSequence.RepeatCount;
			int num2 = this.MacroSequence.Count((BaseMacro m) => m.IsBreak);
			int num3 = this.MacroSequence.Count((BaseMacro m) => m.IsPause);
			int num4 = this.MacroSequence.Count((BaseMacro m) => m.IsKeyBinding);
			int num5 = this.MacroSequence.Count((BaseMacro m) => m.IsGamepadBinding);
			int num6 = this.MacroSequence.Count((BaseMacro m) => m.IsGamepadStick);
			int num7 = this.MacroSequence.Count((BaseMacro m) => m.IsTouchSwipe || m.IsTouchZoom);
			int num8 = this.MacroSequence.Count((BaseMacro m) => m.IsRumble);
			ObservableCollection<BaseMacroBinding> observableCollection = new ObservableCollection<BaseMacroBinding>();
			ObservableCollection<BaseMacroBinding> observableCollection2 = new ObservableCollection<BaseMacroBinding>();
			ObservableCollection<BaseMacroBinding> observableCollection3 = new ObservableCollection<BaseMacroBinding>();
			ObservableCollection<BaseMacroBinding> observableCollection4 = new ObservableCollection<BaseMacroBinding>();
			ActivatorXBBindingDictionary hostDictionary = this.HostDictionary;
			bool flag;
			if (hostDictionary == null)
			{
				flag = null != null;
			}
			else
			{
				XBBinding hostXBBinding = hostDictionary.HostXBBinding;
				if (hostXBBinding == null)
				{
					flag = null != null;
				}
				else
				{
					BaseXBBindingCollection hostCollection = hostXBBinding.HostCollection;
					if (hostCollection == null)
					{
						flag = null != null;
					}
					else
					{
						SubConfigData subConfigData = hostCollection.SubConfigData;
						flag = ((subConfigData != null) ? subConfigData.ConfigData : null) != null;
					}
				}
			}
			ControllerTypeEnum controllerType = VirtualControllerTypeExtensions.GetControllerType((!flag) ? 0 : this.HostDictionary.HostXBBinding.HostCollection.SubConfigData.ConfigData.VirtualGamepadType);
			if (this.MacroSequence.IsOnetime)
			{
				if ((num4 + num5) / 2 + num6 + num7 > 5)
				{
					return false;
				}
				if ((num8 + num3 + num4 + num5) / 2 + num6 + num7 > 5)
				{
					return false;
				}
			}
			if (this.MacroSequence.IsHoldUntilRelease)
			{
				if (num4 + num5 + num6 + num7 > 5)
				{
					return false;
				}
				if (num8 + num3 + num4 + num5 + num6 + num7 > 5)
				{
					return false;
				}
			}
			if (num3 > 5 || num2 > 5)
			{
				return false;
			}
			if ((isTurbo || isToggle) && num2 > 0)
			{
				return false;
			}
			if (isTurbo || isToggle)
			{
				num = 1;
			}
			if (num > 1 && this.MacroSequence.IsOnetime)
			{
				annotations.Add(new MacroRepeatAnnotation());
			}
			for (int i = 0; i < this.MacroSequence.Count; i++)
			{
				if (this.MacroSequence[i].IsGamepadStick)
				{
					if (annotations.Count > 0 && annotations[annotations.Count - 1].IsPlusAnnotation)
					{
						annotations.RemoveAt(annotations.Count - 1);
					}
					if (annotations.Count > 0 && !this.IsSeparator(annotations[annotations.Count - 1]))
					{
						annotations.Add(new MacroSeparatorAnnotation());
					}
					MacroGamepadStick macroGamepadStick = (MacroGamepadStick)this.MacroSequence[i];
					GamepadButton gamepadButton = macroGamepadStick.GamepadButtonDescription.Button;
					short num9 = macroGamepadStick.DeflectionPercentage;
					if (num9 < 0)
					{
						gamepadButton = GamepadButtonExtensions.RevertStickAxis(gamepadButton);
						num9 = Math.Abs(num9);
					}
					KeyScanCodeV2 keyScanCodeV = KeyScanCodeV2.FindKeyScanCodeByGamepadButton(gamepadButton);
					annotations.Add(new MacroGamepadStickAnnotation(keyScanCodeV, num9));
				}
				if (this.MacroSequence[i].IsTouchSwipe || this.MacroSequence[i].IsTouchZoom)
				{
					if (annotations.Count > 0 && annotations[annotations.Count - 1].IsPlusAnnotation)
					{
						annotations.RemoveAt(annotations.Count - 1);
					}
					if (annotations.Count > 0 && !this.IsSeparator(annotations[annotations.Count - 1]))
					{
						annotations.Add(new MacroSeparatorAnnotation());
					}
					KeyScanCodeV2 keyScanCodeV2 = KeyScanCodeV2.FindKeyScanCodeByGamepadButton(this.MacroSequence[i].IsTouchSwipe ? ((MacroTouchSwipe)this.MacroSequence[i]).GamepadButton : ((MacroTouchZoom)this.MacroSequence[i]).GamepadButton);
					annotations.Add(new MacroGamepadSwipeOrZoomAnnotation(keyScanCodeV2, controllerType));
				}
				if (this.MacroSequence[i].IsGamepadBinding)
				{
					BaseMacroBinding macro2 = (BaseMacroBinding)this.MacroSequence[i];
					if ((macro2.MacroKeyType == null || macro2.MacroKeyType == 2) && observableCollection.FirstOrDefault((BaseMacroBinding b) => b.KeyScanCode == macro2.KeyScanCode && b.MacroKeyType == macro2.MacroKeyType) == null)
					{
						if (i + 1 < this.MacroSequence.Count && this.MacroSequence[i + 1].IsGamepadBinding && ((BaseMacroBinding)this.MacroSequence[i + 1]).MacroKeyType == null)
						{
							if (annotations.Count > 0 && this.IsSeparatorAllowed(annotations[annotations.Count - 1]) && this.MacroSequence.IsOnetime && observableCollection.Count == 0 && observableCollection4.Count == 0)
							{
								annotations.Add(new MacroSeparatorAnnotation());
							}
							annotations.Add(new MacroGamepadAnnotation(macro2.KeyScanCode));
							annotations.Add(new MacroPlusAnnotation());
							observableCollection2.Add(macro2);
						}
						else
						{
							observableCollection.Add(macro2);
						}
					}
					if (macro2.MacroKeyType == 1 || this.MacroSequence.IsHoldUntilRelease)
					{
						BaseMacroBinding baseMacroBinding = observableCollection.FirstOrDefault((BaseMacroBinding b) => b.KeyScanCode == macro2.KeyScanCode && (b.MacroKeyType == null || b.MacroKeyType == 2));
						if (baseMacroBinding != null)
						{
							observableCollection.Remove(baseMacroBinding);
							if (annotations.Count > 0)
							{
								if (this.MacroSequence.IsOnetime && (annotations[annotations.Count - 1].IsGamepadAnnotation || observableCollection.Count == 0) && this.IsSeparatorAllowed(annotations[annotations.Count - 1]))
								{
									if (observableCollection4.Count == 0)
									{
										annotations.Add(new MacroSeparatorAnnotation());
									}
									else
									{
										annotations.Add(new MacroPlusAnnotation());
									}
								}
								else if ((annotations[annotations.Count - 1].IsGamepadAnnotation || this.MacroSequence.IsHoldUntilRelease) && this.IsSeparatorAllowed(annotations[annotations.Count - 1]))
								{
									annotations.Add(new MacroPlusAnnotation());
								}
							}
							annotations.Add(new MacroGamepadAnnotation(macro2.KeyScanCode));
							if (this.MacroSequence.IsOnetime && observableCollection.Count == 0 && observableCollection4.Count == 0)
							{
								annotations.Add(new MacroSeparatorAnnotation());
							}
						}
						BaseMacroBinding baseMacroBinding2 = observableCollection4.FirstOrDefault((BaseMacroBinding b) => b.KeyScanCode == macro2.KeyScanCode && (b.MacroKeyType == null || b.MacroKeyType == 2));
						if (baseMacroBinding2 != null)
						{
							observableCollection4.Remove(baseMacroBinding2);
						}
						BaseMacroBinding baseMacroBinding3 = observableCollection2.FirstOrDefault((BaseMacroBinding b) => b.KeyScanCode == macro2.KeyScanCode && (b.MacroKeyType == null || b.MacroKeyType == 2));
						if (baseMacroBinding3 != null)
						{
							observableCollection2.Remove(baseMacroBinding3);
						}
						if (observableCollection2.Count > 0)
						{
							observableCollection3.Add(baseMacroBinding);
						}
					}
				}
				if (this.MacroSequence[i].IsKeyBinding)
				{
					BaseMacroBinding macro = (BaseMacroBinding)this.MacroSequence[i];
					if ((macro.MacroKeyType == null || macro.MacroKeyType == 2) && observableCollection.FirstOrDefault((BaseMacroBinding b) => b.KeyScanCode == macro.KeyScanCode && b.MacroKeyType == macro.MacroKeyType) == null)
					{
						if (i + 1 < this.MacroSequence.Count && this.MacroSequence[i + 1].IsKeyBinding && ((MacroKeyBinding)this.MacroSequence[i + 1]).MacroKeyType == null)
						{
							if (annotations.Count > 0 && this.IsSeparatorAllowed(annotations[annotations.Count - 1]) && this.MacroSequence.IsOnetime && observableCollection.Count == 0 && observableCollection4.Count == 0)
							{
								annotations.Add(new MacroSeparatorAnnotation());
							}
							annotations.Add(new MacroKeyAnnotation(macro.KeyScanCode));
							annotations.Add(new MacroPlusAnnotation());
							observableCollection2.Add(macro);
						}
						else
						{
							observableCollection.Add(macro);
						}
					}
					if (macro.MacroKeyType == 1 || this.MacroSequence.IsHoldUntilRelease)
					{
						BaseMacroBinding baseMacroBinding4 = observableCollection.FirstOrDefault((BaseMacroBinding b) => b.KeyScanCode == macro.KeyScanCode && (b.MacroKeyType == null || b.MacroKeyType == 2));
						if (baseMacroBinding4 != null)
						{
							observableCollection.Remove(baseMacroBinding4);
							if (annotations.Count > 0)
							{
								if (this.MacroSequence.IsOnetime && (annotations[annotations.Count - 1].IsKeyAnnotation || observableCollection.Count == 0) && this.IsSeparatorAllowed(annotations[annotations.Count - 1]))
								{
									if (observableCollection4.Count == 0)
									{
										annotations.Add(new MacroSeparatorAnnotation());
									}
									else
									{
										annotations.Add(new MacroPlusAnnotation());
									}
								}
								else if ((annotations[annotations.Count - 1].IsKeyAnnotation || this.MacroSequence.IsHoldUntilRelease) && this.IsSeparatorAllowed(annotations[annotations.Count - 1]))
								{
									annotations.Add(new MacroPlusAnnotation());
								}
							}
							annotations.Add(new MacroKeyAnnotation(macro.KeyScanCode));
							if (this.MacroSequence.IsOnetime && observableCollection.Count == 0 && observableCollection4.Count == 0)
							{
								annotations.Add(new MacroSeparatorAnnotation());
							}
						}
						BaseMacroBinding baseMacroBinding5 = observableCollection4.FirstOrDefault((BaseMacroBinding b) => b.KeyScanCode == macro.KeyScanCode && (b.MacroKeyType == null || b.MacroKeyType == 2));
						if (baseMacroBinding5 != null)
						{
							observableCollection4.Remove(baseMacroBinding5);
						}
						BaseMacroBinding baseMacroBinding6 = observableCollection2.FirstOrDefault((BaseMacroBinding b) => b.KeyScanCode == macro.KeyScanCode && (b.MacroKeyType == null || b.MacroKeyType == 2));
						if (baseMacroBinding6 != null)
						{
							observableCollection2.Remove(baseMacroBinding6);
						}
						if (observableCollection2.Count > 0)
						{
							observableCollection3.Add(baseMacroBinding4);
						}
					}
				}
				if (this.MacroSequence[i].IsPause)
				{
					if (observableCollection4.Count > 0)
					{
						annotations.Clear();
						return false;
					}
					this.ArchivePause(ref annotations, ref observableCollection, ref observableCollection2, ref observableCollection3, false);
					foreach (BaseMacroBinding baseMacroBinding7 in observableCollection)
					{
						observableCollection4.Add(baseMacroBinding7);
					}
					foreach (BaseMacroBinding baseMacroBinding8 in observableCollection2)
					{
						observableCollection4.Add(baseMacroBinding8);
					}
					observableCollection.Clear();
					observableCollection2.Clear();
					observableCollection3.Clear();
				}
				if (this.MacroSequence[i].IsRumble)
				{
					if (observableCollection4.Count > 0)
					{
						annotations.Clear();
						return false;
					}
					this.ArchivePause(ref annotations, ref observableCollection, ref observableCollection2, ref observableCollection3, true);
					foreach (BaseMacroBinding baseMacroBinding9 in observableCollection)
					{
						observableCollection4.Add(baseMacroBinding9);
					}
					foreach (BaseMacroBinding baseMacroBinding10 in observableCollection2)
					{
						observableCollection4.Add(baseMacroBinding10);
					}
					observableCollection.Clear();
					observableCollection2.Clear();
					observableCollection3.Clear();
				}
				if (this.MacroSequence[i].IsBreak)
				{
					if (annotations.Count > 0 && this.IsSeparator(annotations[annotations.Count - 1]))
					{
						annotations.RemoveAt(annotations.Count - 1);
					}
					annotations.Add(new MacroBreakAnnotation());
				}
			}
			if (annotations.Count > 0 && this.IsSeparator(annotations[annotations.Count - 1]))
			{
				annotations.RemoveAt(annotations.Count - 1);
			}
			return true;
		}

		public SortableObservableCollection<BaseMacroAnnotation> GenerateAnnotationsFromMacroSequence()
		{
			SortableObservableCollection<BaseMacroAnnotation> sortableObservableCollection = new SortableObservableCollection<BaseMacroAnnotation>();
			if (this._macroSequence != null && this.MacroSequence.Count > 0)
			{
				if (!this.TryArchiveMacros(ref sortableObservableCollection))
				{
					sortableObservableCollection.Clear();
					sortableObservableCollection.Add(new LongMacroAnnotation());
				}
				if (sortableObservableCollection.Count == 0)
				{
					sortableObservableCollection.Add(new LongMacroAnnotation());
				}
			}
			return sortableObservableCollection;
		}

		private MacroSequence _macroSequence;

		private bool _isToggle;

		private bool _isTurbo;

		private uint _turboDelay = 200U;

		private bool _isMacroSequenceChanged;

		private string _description;

		private BaseRewasdMapping _mappedKey;

		private ushort _mappedKeyBytes;

		private ActivatorType _activatorType;

		private AssociatedControllerButton _controllerButton;

		private int _jumpToShift = -1;

		private bool _isPostponeMapping;

		private ActivatorShiftType _shiftTypeForUnsetShift;

		private bool _macroSequenceInit;

		private bool _isAdaptiveTriggers;

		private bool _isAdaptiveTriggersInherited;

		private DelegateCommand _openMacroEditorCommand;

		private int _delayBerforeJump = 200;

		private bool _isDelayBerforeJumpChecked;

		private bool _isInheritedBinding;

		private SortableObservableCollection<BaseMacroAnnotation> _macroSequenceAnnotation;

		private uint _rumbleDuration = 150U;

		private bool _isRumbleLeftTrigger;

		private bool _isRumbleRightTrigger;

		private bool _isRumbleLeftMotor;

		private bool _isRumbleRightMotor;

		private ushort _rumbleLeftTriggerSpeed = 60;

		private ushort _rumbleRightTriggerSpeed = 60;

		private ushort _rumbleLeftMotorSpeed = 80;

		private ushort _rumbleRightMotorSpeed = 80;
	}
}
