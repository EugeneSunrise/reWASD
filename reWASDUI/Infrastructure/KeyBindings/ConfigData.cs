using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.Utils.ObservableContentCollection;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using Prism.Commands;
using Prism.Events;
using reWASDCommon.Infrastructure;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Infrastructure.SupportedControllers;
using reWASDCommon.Utils;
using reWASDUI.DataModels;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings.ActivatorXB;
using reWASDUI.Infrastructure.KeyBindings.MacroBinding;
using reWASDUI.Infrastructure.KeyBindings.Mask;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.License.Features;
using reWASDUI.Services;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Infrastructure.LED;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Views.OverlayMenu;

namespace reWASDUI.Infrastructure.KeyBindings
{
	public class ConfigData : SortableObservableCollection<SubConfigData>, IDisposable
	{
		public event EventHandler IsChangedModifiedEvent;

		public bool IsChanged
		{
			get
			{
				return this._isChanged;
			}
			set
			{
				this.FirePropertyChanged("IsCanBeVirtualUsbHubPresent");
				this.FirePropertyChanged("IsCanBeUdpPresent");
				this.FirePropertyChanged("IsCanBeExternalPresent");
				this.FirePropertyChanged("IsBoundToDS4");
				if (this._isVirtualUsbHubPresent && !this.IsCanBeVirtualUsbHubPresent)
				{
					this.IsVirtualUsbHubPresent = false;
				}
				if (this._isUdpPresent && !this.IsCanBeUdpPresent)
				{
					this.IsUdpPresent = false;
				}
				if (this._isExternal && !this.IsCanBeExternalPresent)
				{
					this.IsExternal = false;
				}
				if (this._isChanged == value)
				{
					return;
				}
				ConfigVM configVM = this.ConfigVM;
				if (configVM != null)
				{
					configVM.AutodetectForConfigChanged();
				}
				this._isChanged = value;
				this.FirePropertyChanged("IsChanged");
				this.RaiseIsChangedModifiedEvent();
			}
		}

		public void RaiseIsChangedModifiedEvent()
		{
			EventHandler isChangedModifiedEvent = this.IsChangedModifiedEvent;
			if (isChangedModifiedEvent == null)
			{
				return;
			}
			isChangedModifiedEvent(this, EventArgs.Empty);
		}

		public bool IsHidePhysicalController
		{
			get
			{
				return App.UserSettingsService.IsHidePhysicalController;
			}
		}

		public bool IsHidePhysicalXboxOneOrEliteController
		{
			get
			{
				return App.UserSettingsService.IsHidePhysicalXboxOneOrEliteController;
			}
		}

		private bool IsVirtualMappingsExistCheck
		{
			get
			{
				return this.Any((SubConfigData mw) => mw.MainXBBindingCollection.IsCollectionHasVirtualGamepadMappingsIncludingShift || mw.MainXBBindingCollection.IsOverlayCollectionHasVirtualGamepadMappings || mw.MainXBBindingCollection.IsControllerBindingsHasVirtualGamepadMappingsIncludingShift || mw.MainXBBindingCollection.IsMaskHasVirtualGamepadMappingsIncludingShift || mw.MainXBBindingCollection.GamepadVibrationMainLeft.InsteadOfMainRight || mw.MainXBBindingCollection.GamepadVibrationMainLeft.InsteadOfTriggerLeft || mw.MainXBBindingCollection.GamepadVibrationMainLeft.InsteadOfTriggerRight || mw.MainXBBindingCollection.GamepadVibrationMainRight.InsteadOfMainLeft || mw.MainXBBindingCollection.GamepadVibrationMainRight.InsteadOfTriggerLeft || mw.MainXBBindingCollection.GamepadVibrationMainRight.InsteadOfTriggerRight || mw.MainXBBindingCollection.GamepadVibrationTriggerLeft.InsteadOfMainLeft || mw.MainXBBindingCollection.GamepadVibrationTriggerLeft.InsteadOfMainRight || mw.MainXBBindingCollection.GamepadVibrationTriggerLeft.InsteadOfTriggerRight || mw.MainXBBindingCollection.GamepadVibrationTriggerRight.InsteadOfMainLeft || mw.MainXBBindingCollection.GamepadVibrationTriggerRight.InsteadOfMainRight || mw.MainXBBindingCollection.GamepadVibrationTriggerRight.InsteadOfTriggerLeft);
			}
		}

		public void NeedAddFeatureForDevice(BaseControllerVM controller, out bool macro, out bool rapidFire, out bool advanced)
		{
			macro = false;
			rapidFire = false;
			advanced = false;
			bool isMacroFeatureUnlocked = App.LicensingService.IsMacroFeatureUnlocked;
			bool isAdvancedMappingFeatureUnlocked = App.LicensingService.IsAdvancedMappingFeatureUnlocked;
			bool isToggleFeatureUnlocked = App.LicensingService.IsToggleFeatureUnlocked;
			this.MissingFeatures.Clear();
			if (isMacroFeatureUnlocked && isAdvancedMappingFeatureUnlocked && isToggleFeatureUnlocked)
			{
				return;
			}
			foreach (SubConfigData subConfigData in this)
			{
				BaseControllerVM baseControllerVM;
				if (subConfigData.GetIsSubConfigRelevantForDevice(controller, out baseControllerVM))
				{
					bool flag = false;
					bool flag2 = false;
					bool flag3 = false;
					bool flag4 = false;
					ControllerTypeEnum controllerTypeEnum = 2;
					ControllerVM controllerVM = baseControllerVM as ControllerVM;
					if (controllerVM != null)
					{
						controllerTypeEnum = controllerVM.ControllerType;
						flag = !controllerVM.IsNoRumble;
						flag2 = ControllerTypeExtensions.IsTriggerMotorExpected(controllerTypeEnum);
						flag3 = ControllersHelper.IsGamepadButtonExist(51, controllerTypeEnum);
						flag4 = ControllersHelper.IsGamepadButtonExist(55, controllerTypeEnum);
					}
					MainXBBindingCollection mainXBBindingCollection = subConfigData.MainXBBindingCollection;
					if (!isAdvancedMappingFeatureUnlocked && !advanced)
					{
						advanced |= !isAdvancedMappingFeatureUnlocked && ((flag3 && mainXBBindingCollection.IsHardwareDeadzoneLeftTrigger) || (flag4 && mainXBBindingCollection.IsHardwareDeadzoneRightTrigger) || (flag && (!mainXBBindingCollection.GamepadVibrationMainLeft.IsDefault() || !mainXBBindingCollection.GamepadVibrationMainRight.IsDefault())) || (flag2 && (!mainXBBindingCollection.GamepadVibrationTriggerLeft.IsDefault() || !mainXBBindingCollection.GamepadVibrationTriggerRight.IsDefault())));
					}
					for (int i = 0; i < mainXBBindingCollection.GetLayersCount(); i++)
					{
						BaseXBBindingCollection collectionByLayer = mainXBBindingCollection.GetCollectionByLayer(i);
						if (!isMacroFeatureUnlocked && !macro)
						{
							macro |= !isMacroFeatureUnlocked && collectionByLayer.HasMacroFeatureMapping(controllerTypeEnum);
						}
						if (!isAdvancedMappingFeatureUnlocked && !advanced)
						{
							advanced |= !isAdvancedMappingFeatureUnlocked && collectionByLayer.HasAdvancedFeatureMapping(controllerTypeEnum);
						}
						if (!isToggleFeatureUnlocked && !rapidFire)
						{
							rapidFire |= !isToggleFeatureUnlocked && collectionByLayer.HasRapidFeatureMapping(controllerTypeEnum);
						}
					}
				}
				if (macro & advanced & rapidFire)
				{
					break;
				}
			}
			LicensingService licensingService = App.LicensingService as LicensingService;
			if (licensingService != null)
			{
				if (macro)
				{
					this.MissingFeatures.Add(licensingService.Features.First((Feature f) => f.FeatureId == "macros"));
				}
				if (advanced)
				{
					this.MissingFeatures.Add(licensingService.Features.First((Feature f) => f.FeatureId == "advanced-mapping"));
				}
				if (rapidFire)
				{
					this.MissingFeatures.Add(licensingService.Features.First((Feature f) => f.FeatureId == "rapid-fire"));
				}
			}
		}

		public void CheckFeatures()
		{
			bool flag;
			bool flag2;
			bool flag3;
			this.NeedAddFeatureForDevice(App.GamepadService.CurrentGamepad, out flag, out flag2, out flag3);
			this.IsMissingFeatureExist = flag || flag2 || flag3;
		}

		public void CheckVirtualMappingsExist()
		{
			this.IsVirtualMappingExist = this.IsVirtualMappingsExistCheck;
		}

		private void ClearVirtualMappings()
		{
			this.ForEach(delegate(SubConfigData mw)
			{
				mw.MainXBBindingCollection.ClearVirtualMappings();
			});
			if (Constants.CreateOverlayShift)
			{
				OverlayMenuVM overlayMenu = this.OverlayMenu;
				bool flag;
				if (overlayMenu == null)
				{
					flag = false;
				}
				else
				{
					OverlayMenuCircle circle = overlayMenu.Circle;
					int? num;
					if (circle == null)
					{
						num = null;
					}
					else
					{
						ObservableCollection<SectorItem> sectors = circle.Sectors;
						num = ((sectors != null) ? new int?(sectors.Count) : null);
					}
					int? num2 = num;
					int num3 = 0;
					flag = (num2.GetValueOrDefault() > num3) & (num2 != null);
				}
				if (flag)
				{
					foreach (SectorItem sectorItem in this.OverlayMenu.Circle.Sectors)
					{
						if (sectorItem.IsSubmenuOn)
						{
							OverlayMenuCircle submenu = sectorItem.Submenu;
							if (submenu != null)
							{
								ObservableCollection<SectorItem> sectors2 = submenu.Sectors;
								if (sectors2 != null)
								{
									sectors2.ForEach(delegate(SectorItem ss)
									{
										XBBinding xbbinding2 = ss.XBBinding;
										if (xbbinding2 == null)
										{
											return;
										}
										xbbinding2.ClearVirtualMappings();
									});
								}
							}
						}
						else
						{
							XBBinding xbbinding = sectorItem.XBBinding;
							if (xbbinding != null)
							{
								xbbinding.ClearVirtualMappings();
							}
						}
					}
				}
			}
		}

		private void BindWholeConfigToVirtualController(VirtualGamepadType virtualGamepadType)
		{
			if ((!this.IsBoundToGamepad && !this.IsVirtualMappingExist) || ((this.IsBoundToGamepad || this.IsVirtualMappingExist) && this.VirtualGamepadType != virtualGamepadType))
			{
				this.VirtualGamepadType = virtualGamepadType;
				this.IsBoundToGamepad = true;
			}
			else
			{
				if (this.IsVirtualMappingsExistCheck || this.IsExternal || this.IsUdpPresent || this.IsVirtualUsbHubPresent)
				{
					if (DTMessageBox.Show(DTLocalization.GetString(12499), MessageBoxButton.YesNo, MessageBoxImage.Question, null, false, MessageBoxResult.None) == MessageBoxResult.No)
					{
						return;
					}
					this.ClearVirtualMappings();
				}
				this.IsVirtualMappingExist = false;
				this.IsBoundToGamepad = false;
			}
			if (this.IsVirtualUsbHubPresent && (!this.IsCanBeVirtualUsbHubPresent || !this.IsBoundToGamepad))
			{
				this.IsVirtualUsbHubPresent = false;
			}
			if (this.IsUdpPresent && (!this.IsCanBeUdpPresent || !this.IsBoundToGamepad))
			{
				this.IsUdpPresent = false;
			}
			if (!this.IsBoundToGamepad && this.IsExternal)
			{
				this.IsExternal = false;
			}
			this.IsChanged = true;
			this.RaiseIsChangedModifiedEvent();
		}

		public DelegateCommand BindWholeConfigToXBOX360Command
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._bindWholeConfigToXBOX360) == null)
				{
					delegateCommand = (this._bindWholeConfigToXBOX360 = new DelegateCommand(delegate
					{
						this.BindWholeConfigToVirtualController(0);
					}, () => this.BindWholeConfigToVirtualGamepadCanExecute(0)));
				}
				return delegateCommand;
			}
		}

		public bool IsBoundToXBOX360Triggered
		{
			get
			{
				return this.VirtualGamepadType == null && (this.IsBoundToGamepad || this.IsVirtualMappingExist);
			}
		}

		public DelegateCommand BindWholeConfigToXBOXOneCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._bindWholeConfigToXBOXOne) == null)
				{
					delegateCommand = (this._bindWholeConfigToXBOXOne = new DelegateCommand(delegate
					{
						this.BindWholeConfigToVirtualController(1);
					}, () => this.BindWholeConfigToVirtualGamepadCanExecute(1)));
				}
				return delegateCommand;
			}
		}

		public bool CanBoundToXBOXOne
		{
			get
			{
				return UtilsNative.IsWindows10OrHigher();
			}
		}

		public bool IsBoundToXBOXOneTriggered
		{
			get
			{
				return this.VirtualGamepadType == 1 && (this.IsBoundToGamepad || this.IsVirtualMappingExist);
			}
		}

		public DelegateCommand BindWholeConfigToDS4Command
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._bindWholeConfigToDS4) == null)
				{
					delegateCommand = (this._bindWholeConfigToDS4 = new DelegateCommand(delegate
					{
						this.BindWholeConfigToVirtualController(2);
					}, () => this.BindWholeConfigToVirtualGamepadCanExecute(2)));
				}
				return delegateCommand;
			}
		}

		public bool IsBoundToDS4Triggered
		{
			get
			{
				return this.VirtualGamepadType == 2 && (this.IsBoundToGamepad || this.IsVirtualMappingExist);
			}
		}

		public DelegateCommand BindWholeConfigToDS3Command
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._bindWholeConfigToDS3) == null)
				{
					delegateCommand = (this._bindWholeConfigToDS3 = new DelegateCommand(delegate
					{
						this.BindWholeConfigToVirtualController(3);
					}, () => this.BindWholeConfigToVirtualGamepadCanExecute(3)));
				}
				return delegateCommand;
			}
		}

		public bool CanBoundToDS3
		{
			get
			{
				return true;
			}
		}

		public bool IsBoundToDS3Triggered
		{
			get
			{
				return this.VirtualGamepadType == 3 && (this.IsBoundToGamepad || this.IsVirtualMappingExist);
			}
		}

		public DelegateCommand BindWholeConfigToNintendoSwitchProCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._bindWholeConfigToNintendoSwitchPro) == null)
				{
					delegateCommand = (this._bindWholeConfigToNintendoSwitchPro = new DelegateCommand(delegate
					{
						this.BindWholeConfigToVirtualController(4);
					}, () => this.BindWholeConfigToVirtualGamepadCanExecute(4)));
				}
				return delegateCommand;
			}
		}

		public bool CanBoundToNintendoSwitchPro
		{
			get
			{
				return UtilsNative.IsWindows10OrHigher();
			}
		}

		public bool IsBoundToNintendoSwitchPro
		{
			get
			{
				return this.VirtualGamepadType == 4 && (this.IsBoundToGamepad || this.IsVirtualMappingExist);
			}
		}

		public bool IsBoundToNintendoSwitchProTriggered
		{
			get
			{
				return this.VirtualGamepadType == 4 && (this.IsBoundToGamepad || this.IsVirtualMappingExist);
			}
		}

		public bool IsVirtualGamepad
		{
			get
			{
				return this.IsBoundToGamepad || this.IsVirtualMappingExist;
			}
		}

		public bool IsBoundToGamepad
		{
			get
			{
				return this._isBoundToGamepad;
			}
			set
			{
				bool isVirtualGamepad = this.IsVirtualGamepad;
				if (this.SetProperty(ref this._isBoundToGamepad, value, "IsBoundToGamepad"))
				{
					this.IsChanged = true;
					this.RaiseRefreshBoundToWholeControllerProperties();
					if (this.IsVirtualGamepad && !isVirtualGamepad && !this._isLoading)
					{
						App.EventAggregator.GetEvent<VirtualGamepadTurnedOn>().Publish(null);
					}
					App.EventAggregator.GetEvent<IsBoundToGamepadChanged>().Publish(this);
				}
			}
		}

		public string MissingFeatureText
		{
			get
			{
				return string.Format(DTLocalization.GetString(12807), this.ConfigVM.Name);
			}
		}

		public ObservableCollection<Feature> MissingFeatures { get; } = new ObservableContentCollection<Feature>();

		public bool IsMissingFeatureExist
		{
			get
			{
				return this._isMissingFeatureExist;
			}
			set
			{
				this.SetProperty(ref this._isMissingFeatureExist, value, "IsMissingFeatureExist");
			}
		}

		public bool IsVirtualMappingExist
		{
			get
			{
				return this._isVirtualMappingExist;
			}
			set
			{
				bool isVirtualGamepad = this.IsVirtualGamepad;
				if (this.SetProperty(ref this._isVirtualMappingExist, value, "IsVirtualMappingExist"))
				{
					this.RaiseRefreshBoundToWholeControllerProperties();
					if (this.IsVirtualGamepad && !isVirtualGamepad && !this._isLoading)
					{
						App.EventAggregator.GetEvent<VirtualGamepadTurnedOn>().Publish(null);
					}
					App.EventAggregator.GetEvent<IsBoundToGamepadChanged>().Publish(this);
				}
			}
		}

		private bool BindWholeConfigToVirtualGamepadCanExecute(VirtualGamepadType vgt)
		{
			return this.IsHidePhysicalController || (vgt == this.VirtualGamepadType && this.IsBoundToGamepad);
		}

		public event EventHandler OnLEDSettingsChanged;

		public void RaiseOnLEDSettingsChanged()
		{
			EventHandler onLEDSettingsChanged = this.OnLEDSettingsChanged;
			if (onLEDSettingsChanged != null)
			{
				onLEDSettingsChanged(this, EventArgs.Empty);
			}
			this.FirePropertyChanged("IsConfigShouldBeApplied");
		}

		public bool IsEmpty(bool apply = false)
		{
			return !this.IsBoundToGamepad && !this.Any(delegate(SubConfigData bc)
			{
				if (!apply)
				{
					return !bc.IsEmpty();
				}
				return !bc.IsEmptyForApply();
			});
		}

		public bool IsExternal
		{
			get
			{
				return this._isExternal;
			}
			set
			{
				if (this.SetProperty(ref this._isExternal, value, "IsExternal"))
				{
					if (this._isVirtualUsbHubPresent && !this.IsCanBeVirtualUsbHubPresent)
					{
						this.IsVirtualUsbHubPresent = false;
					}
					if (this._isUdpPresent && !this.IsCanBeUdpPresent)
					{
						this.IsUdpPresent = false;
					}
					if (this._isExternal)
					{
						this.IsBoundToGamepad = true;
					}
					this.IsChanged = true;
				}
			}
		}

		public bool IsHighVirtualDevicePerformance
		{
			get
			{
				return this.VirtualDevicePerformance == 1;
			}
			set
			{
				this.VirtualDevicePerformance = (value ? 1 : 0);
			}
		}

		public Performance VirtualDevicePerformance
		{
			get
			{
				return this._virtualDevicePerformance;
			}
			set
			{
				if (this.SetProperty(ref this._virtualDevicePerformance, value, "VirtualDevicePerformance"))
				{
					this.FirePropertyChanged("IsHighVirtualDevicePerformance");
					this.IsChanged = true;
				}
			}
		}

		public bool IsUdpPresent
		{
			get
			{
				return this._isUdpPresent;
			}
			set
			{
				if (this.SetProperty(ref this._isUdpPresent, value, "IsUdpPresent"))
				{
					if (value)
					{
						this.IsBoundToGamepad = true;
					}
					this.IsChanged = true;
				}
			}
		}

		public bool IsVirtualUsbHubPresent
		{
			get
			{
				return this._isVirtualUsbHubPresent;
			}
			set
			{
				if (this.SetProperty(ref this._isVirtualUsbHubPresent, value, "IsVirtualUsbHubPresent"))
				{
					if (value)
					{
						this.IsBoundToGamepad = true;
					}
					this.IsChanged = true;
				}
			}
		}

		public bool IsCanBeExternalPresent
		{
			get
			{
				return !this.IsUdpPresent && !this.IsVirtualUsbHubPresent;
			}
		}

		public bool IsCanBeUdpPresent
		{
			get
			{
				return !this.IsExternal && this.VirtualGamepadType == 2;
			}
		}

		public bool IsCanBeVirtualUsbHubPresent
		{
			get
			{
				return !this.IsExternal && this.VirtualGamepadType == 2;
			}
		}

		public bool IsBoundToDS4
		{
			get
			{
				return this.VirtualGamepadType == 2;
			}
		}

		public bool VirtualGamepadCanUseAuth
		{
			get
			{
				return this.VirtualGamepadType == 2 || this.VirtualGamepadType == 1;
			}
		}

		public VirtualGamepadType VirtualGamepadType
		{
			get
			{
				return this._virtualGamepadType;
			}
			set
			{
				VirtualGamepadType virtualGamepadType = this._virtualGamepadType;
				VirtualGamepadType virtualGamepadType2 = value;
				if (!UtilsNative.IsWindows10OrHigher() && (virtualGamepadType2 == 1 || virtualGamepadType2 == 4))
				{
					virtualGamepadType2 = 0;
				}
				if (this.SetProperty(ref this._virtualGamepadType, virtualGamepadType2, "VirtualGamepadType"))
				{
					this.IsChanged = true;
					this.RaiseRefreshBoundToWholeControllerProperties();
					this.FirePropertyChanged("IsBoundToDS4");
					this.FirePropertyChanged("IsCanBeUdpPresent");
					this.FirePropertyChanged("IsCanBeExternalPresent");
					this.FirePropertyChanged("VirtualGamepadCanUseAuth");
					this.ForEach(delegate(SubConfigData item)
					{
						item.MainXBBindingCollection.CheckVibrationSettings(this._virtualGamepadType);
					});
					if (!this._isLoading)
					{
						App.EventAggregator.GetEvent<VirtualControllerTypeChanged>().Publish(this);
						if (this.IsVirtualGamepad)
						{
							App.EventAggregator.GetEvent<VirtualGamepadTurnedOn>().Publish(null);
						}
					}
					if (virtualGamepadType2 != 2)
					{
						this._isUdpPresent = false;
					}
					if (virtualGamepadType2 == 3)
					{
						this._isExternal = false;
					}
					if (virtualGamepadType == 4 && this._isExternal)
					{
						this.IsExternal = false;
					}
				}
			}
		}

		public ConfigVM ConfigVM { get; private set; }

		public OverlayMenuVM OverlayMenu { get; set; }

		public ObservableCollection<ConfigData.AdditionalDataItem> AdditionalData { get; set; }

		public ConfigData.AdditionalDataItem GetAdditionalDataItemByIndex(int index)
		{
			if (index < 0 || index >= this.AdditionalData.Count)
			{
				return null;
			}
			return this.AdditionalData[index];
		}

		public void RaiseAdditionalDataPropertyChanged()
		{
			this.FirePropertyChanged("AdditionalData");
		}

		public bool IsConfigShouldBeApplied
		{
			get
			{
				return this.AdditionalData.Any((ConfigData.AdditionalDataItem item) => item.LEDSettings.IsSettingsShouldBeApplied);
			}
		}

		public DelegateCommand ResetLedSettingsCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._resetLedSettingsCommand) == null)
				{
					delegateCommand = (this._resetLedSettingsCommand = new DelegateCommand(new Action(this.ResetLedSettings)));
				}
				return delegateCommand;
			}
		}

		public void ResetLedSettings()
		{
			this.AdditionalData.ForEach(delegate(ConfigData.AdditionalDataItem item)
			{
				item.LEDSettings.ResetToDefaults();
			});
		}

		public ConfigData()
			: this(null, ListSortDirection.Ascending, "ControllerFamily", "Index")
		{
		}

		public ConfigData(ConfigVM configVM, ListSortDirection direction, string name1, string name2)
			: base(direction, name1, name2)
		{
			IEventAggregator eventAggregator = App.EventAggregator;
			if (eventAggregator != null)
			{
				PreferencesChanged @event = eventAggregator.GetEvent<PreferencesChanged>();
				if (@event != null)
				{
					@event.Subscribe(new Action<object>(this.OnPreferencesChanged));
				}
			}
			this.ConfigVM = configVM;
			this.AdditionalData = new ObservableCollection<ConfigData.AdditionalDataItem>();
			this.AdditionalData.CollectionChanged += this.AdditionalData_CollectionChanged;
			this.AddAdditionalDataItem(-1);
			this.OnLEDSettingsChanged += delegate([Nullable(2)] object s, EventArgs e)
			{
				this.IsChanged = true;
			};
			App.UserSettingsService.OnHidePhysicalController += delegate([Nullable(2)] object s, EventArgs e)
			{
				this.FirePropertyChanged("IsBoundToXBOX360Triggered");
				this.BindWholeConfigToXBOX360Command.RaiseCanExecuteChanged();
				this.FirePropertyChanged("IsBoundToXBOXOneTriggered");
				this.BindWholeConfigToXBOXOneCommand.RaiseCanExecuteChanged();
				this.FirePropertyChanged("IsBoundToDS4Triggered");
				this.BindWholeConfigToDS4Command.RaiseCanExecuteChanged();
				this.FirePropertyChanged("IsBoundToDS3Triggered");
				this.BindWholeConfigToDS3Command.RaiseCanExecuteChanged();
				this.FirePropertyChanged("IsBoundToNintendoSwitchProTriggered");
				this.BindWholeConfigToNintendoSwitchProCommand.RaiseCanExecuteChanged();
			};
			TranslationManager.Instance.LanguageChanged += delegate([Nullable(2)] object s, EventArgs e)
			{
				this.FirePropertyChanged("VirtualGamepadType");
			};
			VirtualGamepadType virtualGamepadType;
			Enum.TryParse<VirtualGamepadType>(RegistryHelper.GetValue("Config", "VirtualGamepadType", 0, false).ToString(), out virtualGamepadType);
			this.VirtualGamepadType = virtualGamepadType;
		}

		private void AdditionalData_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.IsChanged = true;
		}

		private void AddAdditionalDataItem(int index = -1)
		{
			ConfigData.AdditionalDataItem additionalDataItem = new ConfigData.AdditionalDataItem();
			int num = ((index == -1) ? this.AdditionalData.Count : index);
			additionalDataItem.MaskBindingCollection = new MaskItemCollection(this, num);
			additionalDataItem.VirtualDeviceSettings = ((this.AdditionalData.Count == 0) ? new VirtualDeviceSettings(0, null) : new VirtualDeviceSettings(num, this.AdditionalData.FirstOrDefault<ConfigData.AdditionalDataItem>().VirtualDeviceSettings));
			additionalDataItem.VirtualDeviceSettings.OnSettingsChanged += delegate([Nullable(2)] object s, EventArgs e)
			{
				this.IsChanged = true;
			};
			LEDSettingsPerCollection ledsettingsPerCollection = new LEDSettingsPerCollection(num, ILEDSettingsPerCollectionContainerExtensions.GetDefaultColor(num, 0), 0, false);
			ledsettingsPerCollection.OnSettingsChanged += delegate([Nullable(2)] object sender, EventArgs args)
			{
				this.RaiseOnLEDSettingsChanged();
			};
			additionalDataItem.LEDSettings = ledsettingsPerCollection;
			if (index == -1)
			{
				this.AdditionalData.Add(additionalDataItem);
				return;
			}
			this.AdditionalData.Insert(index, additionalDataItem);
			for (int i = 0; i < this.AdditionalData.Count; i++)
			{
				this.AdditionalData[i].LEDSettings.ShiftIndex = i;
				this.AdditionalData[i].LEDSettings.ResetShiftDrawing();
			}
		}

		public void RaiseRefreshBoundToWholeControllerProperties()
		{
			this.FirePropertyChanged("IsBoundToXBOX360Triggered");
			this.FirePropertyChanged("IsBoundToDS4Triggered");
			this.FirePropertyChanged("IsBoundToDS3Triggered");
			this.FirePropertyChanged("IsBoundToXBOXOneTriggered");
			this.FirePropertyChanged("IsBoundToNintendoSwitchProTriggered");
			this.FirePropertyChanged("IsVirtualGamepad");
			this.BindWholeConfigToDS4Command.RaiseCanExecuteChanged();
			this.BindWholeConfigToDS3Command.RaiseCanExecuteChanged();
			this.BindWholeConfigToXBOX360Command.RaiseCanExecuteChanged();
			this.BindWholeConfigToXBOXOneCommand.RaiseCanExecuteChanged();
			this.BindWholeConfigToNintendoSwitchProCommand.RaiseCanExecuteChanged();
		}

		private void OnPreferencesChanged(object obj)
		{
			this.RaiseRefreshBoundToWholeControllerProperties();
		}

		public void ResetIsChanged()
		{
			this.IsChanged = false;
			foreach (SubConfigData subConfigData in this)
			{
				subConfigData.MainXBBindingCollection.IsChanged = false;
				subConfigData.MainXBBindingCollection.ShiftXBBindingCollections.ForEach(delegate(ShiftXBBindingCollection sc)
				{
					sc.IsChanged = false;
				});
			}
		}

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

		public void Dispose()
		{
			this.AdditionalData.ForEach(delegate(ConfigData.AdditionalDataItem item)
			{
				item.VirtualDeviceSettings.Dispose();
				item.VirtualDeviceSettings = null;
				item.MaskBindingCollection.Dispose();
				item.MaskBindingCollection = null;
			});
			this.ConfigVM = null;
			this.IsChangedModifiedEvent = null;
			foreach (SubConfigData subConfigData in base.Items)
			{
				subConfigData.Dispose();
			}
			base.Clear();
		}

		public VirtualDeviceSettings GetShiftVirtualSticks(int shiftIndex)
		{
			return this.AdditionalData[shiftIndex].VirtualDeviceSettings;
		}

		public SubConfigData FindControllerCollection(ControllerFamily family, int index, bool createOnDemand)
		{
			SubConfigData subConfigData = this.FirstOrDefault((SubConfigData w) => w.ControllerFamily == family && w.Index == index);
			if (subConfigData == null && createOnDemand)
			{
				subConfigData = new SubConfigData(this, new MainXBBindingCollection(this.ConfigVM, family), family, index);
				for (int i = 0; i < this.AdditionalData.Count - 1; i++)
				{
					subConfigData.MainXBBindingCollection.AddShift(0, -1);
				}
				base.Add(subConfigData);
			}
			return subConfigData;
		}

		public SubConfigData FindGamepadCollection(int index, bool createOnDemand = false)
		{
			return this.FindControllerCollection(0, index, createOnDemand);
		}

		public SubConfigData FindKeyboardCollection(int index, bool createOnDemand = false)
		{
			return this.FindControllerCollection(1, index, createOnDemand);
		}

		public SubConfigData FindMouseCollection(int index, bool createOnDemand = false)
		{
			return this.FindControllerCollection(2, index, createOnDemand);
		}

		public static void FreeBindingsCollection(ref ConfigData collection)
		{
			if (collection == null)
			{
				return;
			}
			collection.Dispose();
			GC.SuppressFinalize(collection);
			collection = null;
			GC.Collect();
		}

		public int LayersCount
		{
			get
			{
				return this.AdditionalData.Count;
			}
		}

		public int AddShift(ShiftType shiftType = 0)
		{
			int lastCommonShiftIndex = this.GetLastCommonShiftIndex();
			this.AddAdditionalDataItem(lastCommonShiftIndex);
			foreach (SubConfigData subConfigData in this)
			{
				subConfigData.MainXBBindingCollection.AddShift(shiftType, lastCommonShiftIndex);
			}
			if (lastCommonShiftIndex != -1)
			{
				this.MoveJumpToShifts(lastCommonShiftIndex, 1);
			}
			if (lastCommonShiftIndex == -1)
			{
				return this.AdditionalData.Count - 1;
			}
			return lastCommonShiftIndex;
		}

		public void SetLayersCount(int layersCount)
		{
			if (!Constants.DynamicShifts)
			{
				int num = (Constants.CreateOverlayShift ? 6 : 5);
				if (layersCount > num)
				{
					layersCount = num;
				}
			}
			if (layersCount > 11)
			{
				layersCount = 11;
			}
			while (this.AdditionalData.Count < layersCount)
			{
				this.AddShift(0);
			}
		}

		public int GetLastCommonShiftIndex()
		{
			int num = -1;
			foreach (ShiftXBBindingCollection shiftXBBindingCollection in base[0].MainXBBindingCollection.ShiftXBBindingCollections)
			{
				if (shiftXBBindingCollection.ShiftType != null)
				{
					num = shiftXBBindingCollection.ShiftIndex;
					break;
				}
			}
			return num;
		}

		public void MoveJumpToShifts(int shiftIndex, int delta)
		{
			bool flag = true;
			Action<ActivatorXBBinding> <>9__1;
			Action<XBBinding> <>9__0;
			foreach (SubConfigData subConfigData in this)
			{
				for (int i = 0; i < this.LayersCount; i++)
				{
					if (i != shiftIndex)
					{
						IEnumerable<XBBinding> enumerable = subConfigData.MainXBBindingCollection.GetCollectionByLayer(i).EnumAllBindings(true, true, flag);
						Action<XBBinding> action;
						if ((action = <>9__0) == null)
						{
							action = (<>9__0 = delegate(XBBinding xbBinding)
							{
								ObservableDictionary<ActivatorType, ActivatorXBBinding> activatorXBBindingDictionary = xbBinding.ActivatorXBBindingDictionary;
								Action<ActivatorXBBinding> action2;
								if ((action2 = <>9__1) == null)
								{
									action2 = (<>9__1 = delegate(ActivatorXBBinding axb)
									{
										if (axb.JumpToShift == shiftIndex && delta < 0)
										{
											MacroSequence macroSequence = axb.MacroSequence;
											if (macroSequence != null)
											{
												macroSequence.Clear();
											}
											axb.MappedKey = KeyScanCodeV2.NoMap;
											axb.NullJumpToShift();
										}
										if (axb.JumpToShift > shiftIndex || (axb.JumpToShift == shiftIndex && delta > 0))
										{
											axb.MoveJumpToShift(delta);
										}
									});
								}
								activatorXBBindingDictionary.ForEachValue(action2);
							});
						}
						enumerable.ForEach(action);
					}
				}
				flag = false;
			}
		}

		public bool IsOverlayBaseXbBindingCollectionPresent()
		{
			return base[0].MainXBBindingCollection.ShiftXBBindingCollections.Any((ShiftXBBindingCollection x) => x.IsOverlayShift);
		}

		public BaseXBBindingCollection GetOverlayBaseXbBindingCollection()
		{
			return base[0].MainXBBindingCollection.ShiftXBBindingCollections.FirstOrDefault((ShiftXBBindingCollection x) => x.IsOverlayShift);
		}

		public void CopyFromModel(ConfigData model)
		{
			this._isLoading = true;
			this.VirtualGamepadType = model.VirtualGamepadType;
			this.IsExternal = model.IsExternal;
			this.IsUdpPresent = model.IsUdpPresent;
			this.IsVirtualUsbHubPresent = model.IsVirtualUsbHubPresent;
			this.IsBoundToGamepad = model.IsBoundToGamepad;
			this.VirtualDevicePerformance = model.VirtualDevicePerformance;
			base.Clear();
			while (this.AdditionalData.Count > 1)
			{
				this.AdditionalData.RemoveAt(1);
			}
			List<SubConfigData> list = new List<SubConfigData>();
			using (List<SubConfigData>.Enumerator enumerator = model.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SubConfigData subConfigData = enumerator.Current;
					SubConfigData subConfigData2 = new SubConfigData(this, new MainXBBindingCollection(this.ConfigVM, subConfigData.ControllerFamily), subConfigData.ControllerFamily, subConfigData.Index);
					base.Add(subConfigData2);
					list.Add(subConfigData2);
				}
				goto IL_DB;
			}
			IL_D3:
			this.AddShift(0);
			IL_DB:
			if (model.AdditionalData.Count <= this.AdditionalData.Count)
			{
				for (int i = 0; i < model.AdditionalData.Count; i++)
				{
					model.AdditionalData[i].VirtualDeviceSettings.CopyTo(this.AdditionalData[i].VirtualDeviceSettings);
					model.AdditionalData[i].LEDSettings.CopyTo(this.AdditionalData[i].LEDSettings);
				}
				for (int j = 0; j < base.Count; j++)
				{
					list[j].CopyFromModel(model[j]);
				}
				for (int k = 0; k < model.AdditionalData.Count; k++)
				{
					this.AdditionalData[k].MaskBindingCollection.CopyFromModel(model.AdditionalData[k].MaskBindingCollection, true);
				}
				for (int l = 1; l < base.Count; l++)
				{
					for (int m = 0; m < this.LayersCount; m++)
					{
						string text = base[0].MainXBBindingCollection.GetCollectionByLayer(m).Description;
						if (text == base[0].MainXBBindingCollection.GetCollectionByLayer(m).DefaultDescription)
						{
							text = null;
						}
						base[l].MainXBBindingCollection.GetCollectionByLayer(m).Description = text;
					}
				}
				if (Constants.CreateOverlayShift && this.IsOverlayBaseXbBindingCollectionPresent())
				{
					this.OverlayMenu = new OverlayMenuVM(this.GetOverlayBaseXbBindingCollection());
					this.OverlayMenu.CopyFromModel(model.OverlayMenu);
				}
				this._isLoading = false;
				return;
			}
			goto IL_D3;
		}

		public void CopyToModel(ConfigData model)
		{
			model.VirtualGamepadType = this.VirtualGamepadType;
			model.IsExternal = this.IsExternal;
			model.IsUdpPresent = this.IsUdpPresent;
			model.IsVirtualUsbHubPresent = this.IsVirtualUsbHubPresent;
			model.IsBoundToGamepad = this.IsBoundToGamepad;
			model.VirtualDevicePerformance = this.VirtualDevicePerformance;
			model.Clear();
			while (model.AdditionalData.Count > 1)
			{
				model.AdditionalData.RemoveAt(1);
			}
			using (IEnumerator<SubConfigData> enumerator = base.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SubConfigData subConfigData = enumerator.Current;
					model.Add(new SubConfigData(model, new MainXBBindingCollection(), subConfigData.ControllerFamily, subConfigData.Index));
				}
				goto IL_B2;
			}
			IL_AB:
			model.AddShift(0);
			IL_B2:
			if (this.AdditionalData.Count <= model.AdditionalData.Count)
			{
				for (int i = 0; i < this.AdditionalData.Count; i++)
				{
					this.AdditionalData[i].VirtualDeviceSettings.CopyTo(model.AdditionalData[i].VirtualDeviceSettings);
					this.AdditionalData[i].LEDSettings.CopyTo(model.AdditionalData[i].LEDSettings);
				}
				for (int j = 0; j < base.Count; j++)
				{
					base[j].CopyToModel(model[j]);
				}
				for (int k = 0; k < this.AdditionalData.Count; k++)
				{
					this.AdditionalData[k].MaskBindingCollection.CopyToModel(model.AdditionalData[k].MaskBindingCollection);
				}
				if (Constants.CreateOverlayShift && model.OverlayMenu != null && model.IsOverlayBaseXbBindingCollectionPresent())
				{
					this.OverlayMenu.CopyToModel(model.OverlayMenu, model.GetOverlayBaseXbBindingCollection());
				}
				return;
			}
			goto IL_AB;
		}

		private VirtualGamepadType _virtualGamepadType;

		private bool _isExternal;

		private bool _isUdpPresent;

		private bool _isVirtualUsbHubPresent;

		private bool _isLoading;

		private Performance _virtualDevicePerformance;

		private bool _isChanged;

		private DelegateCommand _bindWholeConfigToXBOX360;

		private DelegateCommand _bindWholeConfigToXBOXOne;

		private DelegateCommand _bindWholeConfigToDS4;

		private DelegateCommand _bindWholeConfigToDS3;

		private DelegateCommand _bindWholeConfigToNintendoSwitchPro;

		private bool _isBoundToGamepad;

		private bool _isMissingFeatureExist;

		private bool _isVirtualMappingExist;

		private DelegateCommand _resetLedSettingsCommand;

		public class AdditionalDataItem
		{
			public MaskItemCollection MaskBindingCollection { get; set; }

			public VirtualDeviceSettings VirtualDeviceSettings { get; set; }

			public LEDSettingsPerCollection LEDSettings { get; set; }
		}
	}
}
