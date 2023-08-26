using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils.Attributes;
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.View.SecondaryWindows;
using DiscSoft.NET.Common.View.SecondaryWindows.WaitDialog;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings.ControllerBindings;
using reWASDUI.Infrastructure.KeyBindings.Mask;
using reWASDUI.Infrastructure.KeyBindings.XB;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Infrastructure.KeyBindings
{
	public class SubConfigData : ZBindableBase, IDisposable
	{
		public MainXBBindingCollection MainXBBindingCollection { get; private set; }

		public ConfigData ConfigData { get; private set; }

		public ControllerFamily ControllerFamily
		{
			get
			{
				return this._controllerFamily;
			}
			set
			{
				this.SetProperty<ControllerFamily>(ref this._controllerFamily, value, "ControllerFamily");
				this.OnPropertyChanged("ControllerFamilySafe");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public ControllerFamily ControllerFamilySafe
		{
			get
			{
				return this.ControllerFamily;
			}
		}

		public bool IsGamepad
		{
			get
			{
				return this.ControllerFamily == 0;
			}
		}

		public bool IsKeyboard
		{
			get
			{
				return this.ControllerFamily == 1;
			}
		}

		public bool IsMouse
		{
			get
			{
				return this.ControllerFamily == 2;
			}
		}

		public bool IsPeripheral
		{
			get
			{
				return !this.IsGamepad;
			}
		}

		public bool IsUnmapAvailable
		{
			get
			{
				BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
				ControllerTypeEnum? controllerTypeEnum;
				if (currentGamepad == null)
				{
					controllerTypeEnum = null;
				}
				else
				{
					ControllerVM currentController = currentGamepad.CurrentController;
					controllerTypeEnum = ((currentController != null) ? new ControllerTypeEnum?(currentController.ControllerType) : null);
				}
				ControllerTypeEnum? controllerTypeEnum2 = controllerTypeEnum;
				if ((controllerTypeEnum2 == null || !ControllerTypeExtensions.IsAzeron(controllerTypeEnum2.GetValueOrDefault())) && (controllerTypeEnum2 == null || !ControllerTypeExtensions.IsAzeronCyborg(controllerTypeEnum2.GetValueOrDefault())))
				{
					ControllerTypeEnum? controllerTypeEnum3 = controllerTypeEnum2;
					ControllerTypeEnum controllerTypeEnum4 = 502;
					if (!((controllerTypeEnum3.GetValueOrDefault() == controllerTypeEnum4) & (controllerTypeEnum3 != null)))
					{
						return true;
					}
				}
				return !this.IsGamepad;
			}
		}

		private bool IsEmptyMappings()
		{
			if (!this.MainXBBindingCollection.AnyValue((XBBinding item) => item.IsRemapedOrUnmapped || item.IsAnyActivatorVirtualMappingPresent))
			{
				if (!this.MainXBBindingCollection.ControllerBindings.Any((ControllerBinding cb) => cb.XBBinding.IsRemapedOrUnmapped || (cb.XBBinding.IsAnyActivatorVirtualMappingPresent && cb.IsValid)))
				{
					return !this.MainXBBindingCollection.ShiftXBBindingCollections.Any(delegate(ShiftXBBindingCollection col)
					{
						if (!col.Any((KeyValuePair<GamepadButton, XBBinding> item) => item.Value.IsRemapedOrUnmapped || item.Value.IsAnyActivatorVirtualMappingPresent))
						{
							return col.ControllerBindings.Any((ControllerBinding cb) => cb.XBBinding.IsRemapedOrUnmapped || (cb.XBBinding.IsAnyActivatorVirtualMappingPresent && cb.IsValid));
						}
						return true;
					});
				}
			}
			return false;
		}

		private bool IsEmptyHardwareChanges(bool forPrint)
		{
			return !((!forPrint) ? this.MainXBBindingCollection.IsHardwareChangesPresent : this.MainXBBindingCollection.IsHardwareChangesPresentForPrint) && !this.MainXBBindingCollection.ShiftXBBindingCollections.Any(delegate(ShiftXBBindingCollection col)
			{
				if (forPrint)
				{
					return col.IsHardwareChangesPresentForPrint;
				}
				return col.IsHardwareChangesPresent;
			});
		}

		private bool IsEmptyMask()
		{
			if (this.MainXBBindingCollection.MaskBindingCollection.Any((MaskItem item) => item.XBBinding.IsAnyActivatorVirtualMappingPresent && !item.IsNotAtLeastTwoButtons && !item.IsDuplicateInside))
			{
				return false;
			}
			return !this.MainXBBindingCollection.ShiftXBBindingCollections.Any((ShiftXBBindingCollection col) => col.MaskBindingCollection.Any((MaskItem item) => item.XBBinding.IsAnyActivatorVirtualMappingPresent && !item.IsNotAtLeastTwoButtons && !item.IsDuplicateInside));
		}

		public bool IsEmptyPrint()
		{
			return this.IsEmptyMappings() && this.IsEmptyHardwareChanges(true) && this.IsEmptyMask();
		}

		public bool IsEmptyForApply()
		{
			return this.MainXBBindingCollection.GamepadVibrationMainLeft.IsDefault() && this.MainXBBindingCollection.GamepadVibrationMainRight.IsDefault() && this.MainXBBindingCollection.GamepadVibrationTriggerLeft.IsDefault() && this.MainXBBindingCollection.GamepadVibrationTriggerRight.IsDefault() && !this.MainXBBindingCollection.IsAnyAdaptiveTriggerSettingsPresent() && this.MainXBBindingCollection.LeftStickDirectionalGroup.Rotation == 0 && this.MainXBBindingCollection.RightStickDirectionalGroup.Rotation == 0 && this.MainXBBindingCollection.AdditionalStickDirectionalGroup.Rotation == 0 && this.MainXBBindingCollection.Touchpad1DirectionalGroup.Rotation == 0 && this.MainXBBindingCollection.Touchpad2DirectionalGroup.Rotation == 0 && this.IsEmptyMappings() && this.IsEmptyHardwareChanges(false) && this.IsEmptyMask();
		}

		public bool IsEmpty()
		{
			if (!this.MainXBBindingCollection.GamepadVibrationMainLeft.IsDefault() || !this.MainXBBindingCollection.GamepadVibrationMainRight.IsDefault() || !this.MainXBBindingCollection.GamepadVibrationTriggerLeft.IsDefault() || !this.MainXBBindingCollection.GamepadVibrationTriggerRight.IsDefault())
			{
				return false;
			}
			if (!this.IsEmptyMappings() || !this.IsEmptyHardwareChanges(false))
			{
				return false;
			}
			if (this.MainXBBindingCollection.MaskBindingCollection.Any((MaskItem item) => item.XBBinding.IsAnyActivatorVirtualMappingPresent))
			{
				return false;
			}
			return !this.MainXBBindingCollection.ShiftXBBindingCollections.Any(delegate(ShiftXBBindingCollection col)
			{
				MaskItemCollection maskBindingCollection = col.MaskBindingCollection;
				if (maskBindingCollection == null)
				{
					return false;
				}
				return maskBindingCollection.Any((MaskItem item) => item.XBBinding.IsAnyActivatorVirtualMappingPresent);
			}) && this.MainXBBindingCollection.LeftStickDirectionalGroup.Rotation == 0 && this.MainXBBindingCollection.RightStickDirectionalGroup.Rotation == 0 && this.MainXBBindingCollection.AdditionalStickDirectionalGroup.Rotation == 0 && this.MainXBBindingCollection.Touchpad1DirectionalGroup.Rotation == 0 && this.MainXBBindingCollection.Touchpad2DirectionalGroup.Rotation == 0 && !this.MainXBBindingCollection.IsAnyAdaptiveTriggerSettingsPresent();
		}

		private bool IsExistButtonDescriptions(BaseXBBindingCollection mainXBBindingCollection)
		{
			if (mainXBBindingCollection.AnyValue((XBBinding item) => item.SingleActivator.IsDescriptionPresent || item.DoubleActivator.IsDescriptionPresent || item.LongActivator.IsDescriptionPresent || item.TripleActivator.IsDescriptionPresent || item.StartActivator.IsDescriptionPresent || item.ReleaseActivator.IsDescriptionPresent))
			{
				return true;
			}
			return mainXBBindingCollection.ControllerBindings.Any((ControllerBinding item) => item.XBBinding.SingleActivator.IsDescriptionPresent || item.XBBinding.DoubleActivator.IsDescriptionPresent || item.XBBinding.LongActivator.IsDescriptionPresent || item.XBBinding.TripleActivator.IsDescriptionPresent || item.XBBinding.StartActivator.IsDescriptionPresent || item.XBBinding.ReleaseActivator.IsDescriptionPresent);
		}

		public bool IsDescriptionEmpty()
		{
			if (!this.IsExistButtonDescriptions(this.MainXBBindingCollection))
			{
				if (!this.MainXBBindingCollection.MaskBindingCollection.Any((MaskItem x) => x.XBBinding.IsAnyActivatorDescriptionPresent))
				{
					return this.MainXBBindingCollection.ShiftXBBindingCollections.All(delegate(ShiftXBBindingCollection shiftCol)
					{
						if (!this.IsExistButtonDescriptions(shiftCol))
						{
							return !shiftCol.MaskBindingCollection.Any((MaskItem x) => x.XBBinding.IsAnyActivatorDescriptionPresent);
						}
						return false;
					});
				}
			}
			return false;
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public bool IsCurrentSubConfigRelevantForCurrentDevice
		{
			get
			{
				BaseControllerVM baseControllerVM;
				return this.GetIsSubConfigRelevantForDevice(App.GamepadService.CurrentGamepad, out baseControllerVM);
			}
		}

		public int Index
		{
			get
			{
				return this._index;
			}
			set
			{
				if (this.SetProperty<int>(ref this._index, value, "Index"))
				{
					this.OnPropertyChanged("IndexText");
					this.OnPropertyChanged("IsMainSubConfig");
					this.OnPropertyChanged("IsCurrentSubConfigRelevantForCurrentDevice");
				}
			}
		}

		public string IndexText
		{
			get
			{
				if (this.Index <= 0)
				{
					return "";
				}
				return (this.Index + 1).ToString();
			}
		}

		public int IndexByControllerFamily
		{
			get
			{
				return this.ConfigData.Count((SubConfigData item) => item.Index < this.Index && item.ControllerFamily == this.ControllerFamily);
			}
		}

		public bool GetIsSubConfigRelevantForDevice(BaseControllerVM controllerVM, out BaseControllerVM relevantController)
		{
			relevantController = null;
			if (controllerVM == null || controllerVM.IsUnsupportedControllerType)
			{
				return true;
			}
			if (controllerVM.IsNintendoSwitchJoyConComposite)
			{
				if (this.ControllerFamily == null)
				{
					relevantController = controllerVM;
				}
				return this.ControllerFamily == 0;
			}
			if (controllerVM.IsCompositeDevice)
			{
				CompositeControllerVM compositeControllerVM = controllerVM as CompositeControllerVM;
				if (compositeControllerVM == null)
				{
					return false;
				}
				if (this.Index > compositeControllerVM.BaseControllers.Count - 1)
				{
					return false;
				}
				int num = -1;
				using (List<BaseControllerVM>.Enumerator enumerator = compositeControllerVM.BaseControllers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BaseControllerVM baseControllerVM = enumerator.Current;
						if (baseControllerVM != null && baseControllerVM.ControllerFamily == this.ControllerFamily)
						{
							num++;
							if (num == this.Index)
							{
								relevantController = baseControllerVM;
								return true;
							}
						}
					}
					return false;
				}
			}
			if (this.Index == 0)
			{
				if (this.ControllerFamily == controllerVM.ControllerFamily)
				{
					relevantController = controllerVM;
				}
				return this.ControllerFamily == controllerVM.ControllerFamily;
			}
			return false;
		}

		private void UpdateIsCurrentSubConfigRelevantForCurrentDevice(BaseControllerVM baseControllerVM)
		{
			this.OnPropertyChanged("IsCurrentSubConfigRelevantForCurrentDevice");
		}

		public SubConfigData(ConfigData configData, MainXBBindingCollection col, ControllerFamily controllerFamily, int index)
		{
			this.ConfigData = configData;
			this.MainXBBindingCollection = col;
			this.MainXBBindingCollection.SubConfigData = this;
			if (controllerFamily == 2)
			{
				col.ControllerBindings.CreateOnDemand = true;
				if (col.ShiftXBBindingCollections != null)
				{
					col.ShiftXBBindingCollections.ForEach(delegate(ShiftXBBindingCollection sc)
					{
						sc.ControllerBindings.CreateOnDemand = true;
					});
				}
			}
			this.MainXBBindingCollection.AttachMaskCollectionPropertyChanged();
			App.EventAggregator.GetEvent<CurrentGamepadChanged>().Subscribe(new Action<BaseControllerVM>(this.UpdateIsCurrentSubConfigRelevantForCurrentDevice));
			TranslationManager.Instance.LanguageChanged += delegate([Nullable(2)] object s, EventArgs e)
			{
				this.OnPropertyChanged("ControllerFamilySafe");
			};
			this._controllerFamily = controllerFamily;
			this.Index = index;
		}

		public new void Dispose()
		{
			this.MainXBBindingCollection.Dispose();
			this.MainXBBindingCollection = null;
			this.ConfigData = null;
		}

		public ICommand RemoveFromHostCollectionCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._removeFromHostCollection) == null)
				{
					relayCommand = (this._removeFromHostCollection = new RelayCommand(new Action(this.RemoveFromHostCollection), new Func<bool>(this.RemoveFromHostCollectionCanExecute)));
				}
				return relayCommand;
			}
		}

		private void RemoveFromHostCollection()
		{
			if (MessageBoxWithDoNotShowLogic.Show(Application.Current.MainWindow, DTLocalization.GetString(11451), MessageBoxButton.YesNo, MessageBoxImage.Question, "RemoveGameOrConfig", MessageBoxResult.Yes, false, 0.0, null, null, null, null, null, null) == MessageBoxResult.Yes)
			{
				SubConfigData subConfigData = null;
				if (this.ConfigData.ConfigVM.CurrentSubConfigData == this && this.Index > 0)
				{
					subConfigData = this.ConfigData.FirstOrDefault((SubConfigData w) => w.Index == this.Index - 1 && w.ControllerFamily == this.ControllerFamily);
				}
				if (subConfigData != null)
				{
					this.ConfigData.ConfigVM.CurrentSubConfigData = subConfigData;
				}
				this.ConfigData.Remove(this);
				this.ConfigData.Where((SubConfigData s) => s.ControllerFamily == this.ControllerFamily && s.Index > this.Index).ForEach(delegate(SubConfigData s)
				{
					s.Index--;
				});
				this.ConfigData.IsChanged = true;
				this.ConfigData.ConfigVM.UpdateCreateSubConfigsCommandsCanExecute();
			}
		}

		private bool RemoveFromHostCollectionCanExecute()
		{
			return !this.IsMainSubConfig;
		}

		public bool IsMainSubConfig
		{
			get
			{
				return this.Index == 0;
			}
		}

		public bool HasSeveralSubConfigsByControllerFamily
		{
			get
			{
				return this.ConfigData.Count((SubConfigData item) => item.ControllerFamily == this.ControllerFamily) > 1;
			}
		}

		public ICommand ClearSubConfigLayerCommand
		{
			get
			{
				ICommand command;
				if ((command = this._clearSubConfigLayer) == null)
				{
					command = (this._clearSubConfigLayer = new RelayCommand(new Action(this.ClearSubConfigLayer), new Func<bool>(this.ClearSubConfigLayerCanExecute)));
				}
				return command;
			}
		}

		private void ClearSubConfigLayer()
		{
			if (MessageBoxWithDoNotShowLogic.Show(Application.Current.MainWindow, DTLocalization.GetString(12473), MessageBoxButton.YesNo, MessageBoxImage.Question, "ConfirmClearConfig", MessageBoxResult.Yes, false, 0.0, null, null, null, null, null, null) == MessageBoxResult.Yes)
			{
				WaitDialog.ShowDialogStatic(DTLocalization.GetString(5238), null, null, false, false, null, null);
				reWASDApplicationCommands.ClearSubConfig(this.ConfigData.ConfigVM, this, App.GameProfilesService.CurrentShiftModificator, true);
				WaitDialog.TryCloseWaitDialog();
			}
		}

		public ICommand ClearSubConfigCommand
		{
			get
			{
				ICommand command;
				if ((command = this._clearSubConfig) == null)
				{
					command = (this._clearSubConfig = new RelayCommand(new Action(this.ClearSubConfig), new Func<bool>(this.ClearSubConfigCanExecute)));
				}
				return command;
			}
		}

		private bool ClearSubConfigLayerCanExecute()
		{
			int currentShiftModificator = App.GameProfilesService.CurrentShiftModificator;
			BaseXBBindingCollection collectionByLayer = this.MainXBBindingCollection.GetCollectionByLayer(currentShiftModificator);
			return collectionByLayer != null && (collectionByLayer.IsCollectionHasMappings || !collectionByLayer.IsControllerBindingsEmptyWithoutStandart || collectionByLayer.IsHardwareChangesPresent || ((collectionByLayer.IsAdaptiveLeftTriggerSettingsPresent && !collectionByLayer.AdaptiveLeftTriggerSettings.IsInherited) || (collectionByLayer.IsAdaptiveRightTriggerSettingsPresent && !collectionByLayer.AdaptiveRightTriggerSettings.IsInherited)) || (currentShiftModificator == 0 && (this.MainXBBindingCollection.MouseSensitivity != 4 || this.MainXBBindingCollection.MouseDeflection != 8 || this.MainXBBindingCollection.WheelDeflection != 13 || this.MainXBBindingCollection.MouseAcceleration != 8 || this.MainXBBindingCollection.VirtualKeyboardRepeatRate != null)));
		}

		private bool ClearSubConfigCanExecute()
		{
			if (this.MainXBBindingCollection.IsCollectionHasMappings)
			{
				return true;
			}
			if (!this.MainXBBindingCollection.IsControllerBindingsEmptyWithoutStandart)
			{
				return true;
			}
			if (this.MainXBBindingCollection.IsHardwareChangesPresent)
			{
				return true;
			}
			if (this.MainXBBindingCollection.IsAdaptiveLeftTriggerSettingsPresent || this.MainXBBindingCollection.IsAdaptiveRightTriggerSettingsPresent)
			{
				return true;
			}
			if (!this.MainXBBindingCollection.GamepadVibrationMainLeft.IsDefault() || !this.MainXBBindingCollection.GamepadVibrationMainRight.IsDefault() || !this.MainXBBindingCollection.GamepadVibrationTriggerLeft.IsDefault() || !this.MainXBBindingCollection.GamepadVibrationTriggerRight.IsDefault())
			{
				return true;
			}
			foreach (ShiftXBBindingCollection shiftXBBindingCollection in this.MainXBBindingCollection.ShiftXBBindingCollections)
			{
				if (shiftXBBindingCollection.IsCollectionHasMappings || !shiftXBBindingCollection.IsControllerBindingsEmptyWithoutStandart || shiftXBBindingCollection.IsHardwareChangesPresent)
				{
					return true;
				}
			}
			return this.MainXBBindingCollection.MouseSensitivity != 4 || this.MainXBBindingCollection.MouseDeflection != 8 || this.MainXBBindingCollection.WheelDeflection != 13 || this.MainXBBindingCollection.MouseAcceleration != 8 || this.MainXBBindingCollection.VirtualKeyboardRepeatRate != null;
		}

		private void ClearSubConfig()
		{
			if (MessageBoxWithDoNotShowLogic.Show(Application.Current.MainWindow, DTLocalization.GetString(12460), MessageBoxButton.YesNo, MessageBoxImage.Question, "ConfirmClearConfig", MessageBoxResult.Yes, false, 0.0, null, null, null, null, null, null) == MessageBoxResult.Yes)
			{
				WaitDialog.ShowDialogStatic(DTLocalization.GetString(5238), null, null, false, false, null, null);
				reWASDApplicationCommands.ClearSubConfig(this.ConfigData.ConfigVM, this, App.GameProfilesService.CurrentShiftModificator, false);
				WaitDialog.TryCloseWaitDialog();
			}
		}

		public void CopyFromModel(SubConfigData model)
		{
			this.MainXBBindingCollection.CopyFromModel(model.MainXBBindingCollection);
		}

		public void CopyToModel(SubConfigData model)
		{
			this.MainXBBindingCollection.CopyToModel(model.MainXBBindingCollection);
		}

		private int _index;

		private ControllerFamily _controllerFamily;

		private RelayCommand _removeFromHostCollection;

		private ICommand _clearSubConfigLayer;

		private ICommand _clearSubConfig;
	}
}
