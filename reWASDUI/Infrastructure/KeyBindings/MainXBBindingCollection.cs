using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using Newtonsoft.Json;
using Prism.Commands;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.DataModels;
using reWASDUI.Infrastructure.KeyBindings.ControllerBindings;
using reWASDUI.Infrastructure.KeyBindings.XB;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Views.OverlayMenu;

namespace reWASDUI.Infrastructure.KeyBindings
{
	public class MainXBBindingCollection : BaseXBBindingCollection
	{
		[JsonProperty("GamepadVibrationMainLeft")]
		public VibrationSettings GamepadVibrationMainLeft { get; set; } = new VibrationSettings();

		[JsonProperty("GamepadVibrationMainRight")]
		public VibrationSettings GamepadVibrationMainRight { get; set; } = new VibrationSettings();

		[JsonProperty("GamepadVibrationTriggerLeft")]
		public VibrationSettings GamepadVibrationTriggerLeft { get; set; } = new VibrationSettings();

		[JsonProperty("GamepadVibrationTriggerRight")]
		public VibrationSettings GamepadVibrationTriggerRight { get; set; } = new VibrationSettings();

		public void CheckVibrationSettings(VirtualGamepadType virtualGamepadType)
		{
			if (virtualGamepadType != 1)
			{
				this.GamepadVibrationMainLeft.ClearTriggers();
				this.GamepadVibrationMainRight.ClearTriggers();
				this.GamepadVibrationTriggerLeft.ClearTriggers();
				this.GamepadVibrationTriggerRight.ClearTriggers();
			}
		}

		public DelegateCommand ResetVibrationSettingsCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._resetVibrationSettingsCommand) == null)
				{
					delegateCommand = (this._resetVibrationSettingsCommand = new DelegateCommand(new Action(this.ResetVibrationSettings)));
				}
				return delegateCommand;
			}
		}

		private void ResetVibrationSettings()
		{
			this.GamepadVibrationMainLeft.ResetToDefaults();
			this.GamepadVibrationMainRight.ResetToDefaults();
			this.GamepadVibrationTriggerLeft.ResetToDefaults();
			this.GamepadVibrationTriggerRight.ResetToDefaults();
			this.SubConfigData.ConfigData.CheckFeatures();
		}

		[JsonProperty("AnalogButtons")]
		private Dictionary<GamepadButton, ZoneValues> AnalogButtons { get; set; } = new Dictionary<GamepadButton, ZoneValues>();

		private ZoneValues GetAnalogButton(GamepadButton button)
		{
			ZoneValues zoneValues;
			if (this.AnalogButtons.TryGetValue(button, out zoneValues))
			{
				return zoneValues;
			}
			if (GamepadButtonExtensions.IsAnyTriggerPress(button))
			{
				zoneValues = new ZoneValues(32768, 960);
			}
			else
			{
				zoneValues = new ZoneValues(255, 10);
			}
			zoneValues.PropertyChanged += delegate([Nullable(2)] object sender, PropertyChangedEventArgs args)
			{
				base.IsChanged = true;
			};
			this.AnalogButtons[button] = zoneValues;
			return zoneValues;
		}

		public ZoneValues LeftTrigger
		{
			get
			{
				return this.GetAnalogButton(51);
			}
		}

		public ZoneValues RightTrigger
		{
			get
			{
				return this.GetAnalogButton(55);
			}
		}

		public ZoneValues LeftBumper
		{
			get
			{
				return this.GetAnalogButton(5);
			}
		}

		public ZoneValues RightBumper
		{
			get
			{
				return this.GetAnalogButton(6);
			}
		}

		public ZoneValues DS3CrossAnalog
		{
			get
			{
				return this.GetAnalogButton(1);
			}
		}

		public ZoneValues DS3CircleAnalog
		{
			get
			{
				return this.GetAnalogButton(2);
			}
		}

		public ZoneValues DS3SquareAnalog
		{
			get
			{
				return this.GetAnalogButton(3);
			}
		}

		public ZoneValues DS3TriangleAnalog
		{
			get
			{
				return this.GetAnalogButton(4);
			}
		}

		public ZoneValues DS3DPADUpAnalog
		{
			get
			{
				return this.GetAnalogButton(33);
			}
		}

		public ZoneValues DS3DPADDownAnalog
		{
			get
			{
				return this.GetAnalogButton(34);
			}
		}

		public ZoneValues DS3DPADRightAnalog
		{
			get
			{
				return this.GetAnalogButton(36);
			}
		}

		public ZoneValues DS3DPADLeftAnalog
		{
			get
			{
				return this.GetAnalogButton(35);
			}
		}

		public ZoneValues SteamDeckTrackpad1Pressure
		{
			get
			{
				return this.GetAnalogButton(242);
			}
		}

		public ZoneValues SteamDeckTrackpad2Pressure
		{
			get
			{
				return this.GetAnalogButton(243);
			}
		}

		[JsonProperty("IsHardwareDeadzoneLeftTrigger")]
		public bool IsHardwareDeadzoneLeftTrigger
		{
			get
			{
				return this._isHardwareDeadzoneLeftTrigger;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isHardwareDeadzoneLeftTrigger, value, "IsHardwareDeadzoneLeftTrigger"))
				{
					base.IsChanged = true;
				}
			}
		}

		[JsonProperty("IsHardwareDeadzoneRightTrigger")]
		public bool IsHardwareDeadzoneRightTrigger
		{
			get
			{
				return this._isHardwareDeadzoneRightTrigger;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isHardwareDeadzoneRightTrigger, value, "IsHardwareDeadzoneRightTrigger"))
				{
					base.IsChanged = true;
				}
			}
		}

		public bool IsLEDSettingsView
		{
			get
			{
				return this._isLEDSettingsView;
			}
			set
			{
				this.SetProperty<bool>(ref this._isLEDSettingsView, value, "IsLEDSettingsView");
			}
		}

		public ObservableCollection<KeyboardRepeatType> KeyboardRepeatTypeCollection { get; private set; }

		public KeyboardRepeatType VirtualKeyboardRepeatRate
		{
			get
			{
				return this._virtualKeyboardRepeatRate;
			}
			set
			{
				if (this._virtualKeyboardRepeatRate == value)
				{
					return;
				}
				this._virtualKeyboardRepeatRate = value;
				base.IsChanged = true;
				ConfigVM configVM = this.ConfigVM;
				if (configVM != null)
				{
					configVM.TryApplyMouseSettingsToAllSubConfigs();
				}
				this.OnPropertyChanged("VirtualKeyboardRepeatRate");
				this.OnPropertyChanged("VirtualKeyboardRepeatRateInfo");
			}
		}

		public string VirtualKeyboardRepeatRateInfo
		{
			get
			{
				switch (this.VirtualKeyboardRepeatRate)
				{
				case 0:
					return DTLocalization.GetString(11645);
				case 1:
					return DTLocalization.GetString(11644);
				case 2:
					return DTLocalization.GetString(11646);
				case 3:
					return DTLocalization.GetString(11647);
				default:
					return string.Empty;
				}
			}
		}

		public ushort VirtualKeyboardRepeatValue
		{
			get
			{
				return this._virtualKeyboardRepeatValue;
			}
			set
			{
				if (this._virtualKeyboardRepeatValue == value)
				{
					return;
				}
				this._virtualKeyboardRepeatValue = value;
				base.IsChanged = true;
				this.OnPropertyChanged("VirtualKeyboardRepeatValue");
			}
		}

		public ushort MouseSensitivity
		{
			get
			{
				return this._mouseSensitivity;
			}
			set
			{
				if (this._mouseSensitivity == value)
				{
					return;
				}
				this._mouseSensitivity = value;
				base.IsChanged = true;
				ConfigVM configVM = this.ConfigVM;
				if (configVM != null)
				{
					configVM.TryApplyMouseSettingsToAllSubConfigs();
				}
				this.OnPropertyChanged("MouseSensitivity");
			}
		}

		public ushort MouseDeflection
		{
			get
			{
				return this._mouseDeflection;
			}
			set
			{
				if (this._mouseDeflection == value)
				{
					return;
				}
				this._mouseDeflection = value;
				base.IsChanged = true;
				ConfigVM configVM = this.ConfigVM;
				if (configVM != null)
				{
					configVM.TryApplyMouseSettingsToAllSubConfigs();
				}
				this.OnPropertyChanged("MouseDeflection");
			}
		}

		public ushort WheelDeflection
		{
			get
			{
				return this._wheelDeflection;
			}
			set
			{
				if (this._wheelDeflection == value)
				{
					return;
				}
				this._wheelDeflection = value;
				base.IsChanged = true;
				ConfigVM configVM = this.ConfigVM;
				if (configVM != null)
				{
					configVM.TryApplyMouseSettingsToAllSubConfigs();
				}
				this.OnPropertyChanged("WheelDeflection");
			}
		}

		public ushort MouseAcceleration
		{
			get
			{
				return this._mouseAcceleration;
			}
			set
			{
				if (this._mouseAcceleration == value)
				{
					return;
				}
				this._mouseAcceleration = value;
				base.IsChanged = true;
				ConfigVM configVM = this.ConfigVM;
				if (configVM != null)
				{
					configVM.TryApplyMouseSettingsToAllSubConfigs();
				}
				this.OnPropertyChanged("MouseAcceleration");
			}
		}

		public bool IsChangedIncludingShiftCollections
		{
			get
			{
				if (!base.IsChanged)
				{
					return this.ShiftXBBindingCollections.Any((ShiftXBBindingCollection sc) => sc.IsChanged);
				}
				return true;
			}
		}

		public bool IsCollectionHasMappingsIncludingShift
		{
			get
			{
				if (!this.IsCollectionHasMappings)
				{
					return this.ShiftXBBindingCollections.Any((ShiftXBBindingCollection sc) => sc.IsCollectionHasMappings);
				}
				return true;
			}
		}

		public bool IsCollectionHasMappingsButNotMasksIncludingShift
		{
			get
			{
				if (!base.IsCollectionHasMappingsButNotMasks)
				{
					return this.ShiftXBBindingCollections.Any((ShiftXBBindingCollection sc) => sc.IsCollectionHasMappingsButNotMasks);
				}
				return true;
			}
		}

		public bool IsOverlayCollectionHasVirtualGamepadMappings
		{
			get
			{
				if (this.ShiftXBBindingCollections.Any((ShiftXBBindingCollection sc) => sc.IsOverlayShift && sc.HasVirtualGamepadMapping))
				{
					return true;
				}
				OverlayMenuVM overlayMenu = this.ConfigVM.ConfigData.OverlayMenu;
				if (overlayMenu == null)
				{
					return false;
				}
				OverlayMenuCircle circle = overlayMenu.Circle;
				bool? flag;
				if (circle == null)
				{
					flag = null;
				}
				else
				{
					ObservableCollection<SectorItem> sectors = circle.Sectors;
					if (sectors == null)
					{
						flag = null;
					}
					else
					{
						flag = new bool?(sectors.Any(delegate(SectorItem sc)
						{
							if (!sc.IsSubmenuOn)
							{
								XBBinding xbbinding = sc.XBBinding;
								return xbbinding != null && xbbinding.IsAnyActivatorVirtualGamepadMappingPresent;
							}
							OverlayMenuCircle submenu = sc.Submenu;
							if (submenu == null)
							{
								return false;
							}
							ObservableCollection<SectorItem> sectors2 = submenu.Sectors;
							bool? flag4;
							if (sectors2 == null)
							{
								flag4 = null;
							}
							else
							{
								flag4 = new bool?(sectors2.Any(delegate(SectorItem ssc)
								{
									XBBinding xbbinding2 = ssc.XBBinding;
									return xbbinding2 != null && xbbinding2.IsAnyActivatorVirtualGamepadMappingPresent;
								}));
							}
							bool? flag5 = flag4;
							bool flag6 = true;
							return (flag5.GetValueOrDefault() == flag6) & (flag5 != null);
						}));
					}
				}
				bool? flag2 = flag;
				bool flag3 = true;
				return (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
			}
		}

		public bool IsCollectionHasVirtualGamepadMappingsIncludingShift
		{
			get
			{
				if (!this.HasVirtualGamepadMapping)
				{
					return this.ShiftXBBindingCollections.Any((ShiftXBBindingCollection sc) => sc.HasVirtualGamepadMapping);
				}
				return true;
			}
		}

		public bool IsControllerBindingsHasVirtualGamepadMappingsIncludingShift
		{
			get
			{
				if (!this.HasControllerBindingsVirtualGamepadMapping)
				{
					return this.ShiftXBBindingCollections.Any((ShiftXBBindingCollection sc) => sc.HasControllerBindingsVirtualGamepadMapping);
				}
				return true;
			}
		}

		public bool IsMaskHasVirtualGamepadMappingsIncludingShift
		{
			get
			{
				if (!this.HasMaskVirtualGamepadMapping)
				{
					return this.ShiftXBBindingCollections.Any((ShiftXBBindingCollection sc) => sc.HasMaskVirtualGamepadMapping);
				}
				return true;
			}
		}

		public bool IsKeyboardMaskHasVirtualGamepadMappingsIncludingShift
		{
			get
			{
				if (!this.HasKeyboardMaskWithVirtualMapping)
				{
					return this.ShiftXBBindingCollections.Any((ShiftXBBindingCollection sc) => sc.HasKeyboardMaskWithVirtualMapping);
				}
				return true;
			}
		}

		public bool IsMouseDigitalMaskHasVirtualGamepadMappingsIncludingShift
		{
			get
			{
				if (!this.HasMouseDigitalMaskWithVirtualMapping)
				{
					return this.ShiftXBBindingCollections.Any((ShiftXBBindingCollection sc) => sc.HasMouseDigitalMaskWithVirtualMapping);
				}
				return true;
			}
		}

		public bool IsMouseDirectionalGroupHasVirtualGamepadMappingsIncludingShift
		{
			get
			{
				if (!this.HasMouseDirectionalGroupVirtualMapping)
				{
					return this.ShiftXBBindingCollections.Any((ShiftXBBindingCollection sc) => sc.HasMouseDirectionalGroupVirtualMapping);
				}
				return true;
			}
		}

		public override bool IsHardwareChangesPresent
		{
			get
			{
				return this.SubConfigData.ControllerFamily == null && (base.IsHardwareChangesPresent || base.IsRightStickXInverted || base.IsLeftStickXInverted || base.IsRightStickYInverted || base.IsLeftStickYInverted || base.IsLeftStickSwapped || base.IsRightStickSwapped || base.LeftStickDirectionalGroup.XSensitivity != 13573 || base.LeftStickDirectionalGroup.YSensitivity != 13573 || base.RightStickDirectionalGroup.XSensitivity != 13573 || base.RightStickDirectionalGroup.YSensitivity != 13573 || this.IsHardwareDeadzoneLeftTrigger || this.IsHardwareDeadzoneRightTrigger || this.LeftTrigger.Low != 960 || this.LeftTrigger.Med != 11562 || this.LeftTrigger.High != 22164 || this.LeftTrigger.RightDeadZone != 32768 || this.RightTrigger.Low != 960 || this.RightTrigger.Med != 11562 || this.RightTrigger.High != 22164 || this.RightTrigger.RightDeadZone != 32768);
			}
		}

		public ObservableCollection<ShiftXBBindingCollection> ShiftXBBindingCollections { get; private set; }

		public ShiftXBBindingCollection CurrentShiftXBBindingCollection
		{
			get
			{
				return this._currentShiftXBBindingCollection;
			}
			set
			{
				if (this._currentShiftXBBindingCollection == value)
				{
					return;
				}
				this._currentShiftXBBindingCollection = value;
				this.RefreshCurrentActivators();
				this.OnPropertyChanged("CurrentShiftXBBindingCollection");
				this.OnPropertyChanged("RealCurrentBeingMappedBindingCollection");
			}
		}

		public void SetCurrentShiftXBBindingCollection(ShiftXBBindingCollection sc)
		{
			if (this.RealCurrentBeingMappedBindingCollection != null && sc != null)
			{
				BaseXBBindingCollection realCurrentBeingMappedBindingCollection = this.RealCurrentBeingMappedBindingCollection;
				bool isChanged = this.IsChangedIncludingShiftCollections;
				realCurrentBeingMappedBindingCollection.ControllerBindings.RemoveEmptyItems();
				realCurrentBeingMappedBindingCollection.MaskBindingCollection.RemoveEmpty();
				base.IsChanged = isChanged;
				this.ShiftXBBindingCollections.ForEach(delegate(ShiftXBBindingCollection sh)
				{
					sh.IsChanged = isChanged;
				});
			}
			if (sc != null)
			{
				sc.ControllerBindings.SuppressNotification = true;
				sc.ControllerBindings.Remove((ControllerBinding cb) => cb.IsInheritedBinding);
				foreach (ControllerBinding controllerBinding in base.ControllerBindings)
				{
					sc.ControllerBindings.AddInheritedItemIfNeededForShiftCollection(controllerBinding.XBBinding.KeyScanCode);
				}
				sc.ControllerBindings.SuppressNotification = false;
			}
			if (this.CurrentShiftXBBindingCollection != null)
			{
				this.CurrentShiftXBBindingCollection.ControllerBindings.SuppressNotification = true;
				this.CurrentShiftXBBindingCollection.ControllerBindings.Remove((ControllerBinding cb) => cb.IsInheritedBinding);
				this.CurrentShiftXBBindingCollection.ControllerBindings.SuppressNotification = false;
			}
			ShiftXBBindingCollection currentShiftXBBindingCollection = this.CurrentShiftXBBindingCollection;
			if (currentShiftXBBindingCollection != null)
			{
				currentShiftXBBindingCollection.SetCurrentButtonMapping(null);
			}
			base.SetCurrentButtonMapping(null);
			this.CurrentShiftXBBindingCollection = sc;
		}

		public int GetLayersCount()
		{
			return this.ShiftXBBindingCollections.Count + 1;
		}

		public BaseXBBindingCollection GetCollectionByLayer(int layer)
		{
			if (layer == -1 || layer > this.ShiftXBBindingCollections.Count)
			{
				return null;
			}
			if (layer == 0)
			{
				return this;
			}
			return this.ShiftXBBindingCollections[layer - 1];
		}

		public void CorrectShiftIndexes()
		{
			for (int i = 0; i < this.ShiftXBBindingCollections.Count; i++)
			{
				this.ShiftXBBindingCollections[i].ShiftIndex = i + 1;
				this.ShiftXBBindingCollections[i].InitIcons();
			}
		}

		public BaseXBBindingCollection RealCurrentBeingMappedBindingCollection
		{
			get
			{
				if (this.CurrentShiftXBBindingCollection != null)
				{
					return this.CurrentShiftXBBindingCollection;
				}
				return this;
			}
		}

		public ConfigVM ConfigVM { get; set; }

		public override SubConfigData SubConfigData { get; set; }

		public Drawing ShiftIcon
		{
			get
			{
				return Application.Current.TryFindResource("Shift0White") as Drawing;
			}
		}

		public override string DefaultDescription
		{
			get
			{
				return DTLocalization.GetString(11195);
			}
		}

		public MainXBBindingCollection(ConfigVM configVM, ControllerFamily controllerFamily)
			: base(controllerFamily)
		{
			this.ConfigVM = configVM;
			this.CreateShiftCollections(controllerFamily);
			this.OnMainOrShiftCollectionItemPropertyChangedExtended += delegate(object o, PropertyChangedExtendedEventArgs e)
			{
				base.CheckDuplicateShiftMappingForButtonMapping(o, e, this.ShiftXBBindingCollections);
				this.OnPropertyChanged("IsChangedIncludingShiftCollections");
				this.OnPropertyChanged("IsCollectionHasMappingsIncludingShift");
				this.OnPropertyChanged("IsCollectionHasMappingsButNotMasksIncludingShift");
			};
			base.InitEventHandler();
			this.LeftTrigger.Low = 960;
			this.LeftTrigger.Med = 11562;
			this.LeftTrigger.High = 22164;
			this.LeftTrigger.RightDeadZone = 32768;
			this.RightTrigger.Low = 960;
			this.RightTrigger.Med = 11562;
			this.RightTrigger.High = 22164;
			this.RightTrigger.RightDeadZone = 32768;
			this._mouseSensitivity = 4;
			this._mouseDeflection = 8;
			this._wheelDeflection = 13;
			this._mouseAcceleration = 8;
			this._virtualKeyboardRepeatRate = 0;
			this._virtualKeyboardRepeatValue = 10;
			this.GamepadVibrationMainLeft.OnChanged += delegate(VibrationSettings s)
			{
				this.OnVibrationSettingsChanged(s);
			};
			this.GamepadVibrationMainRight.OnChanged += delegate(VibrationSettings s)
			{
				this.OnVibrationSettingsChanged(s);
			};
			this.GamepadVibrationTriggerLeft.OnChanged += delegate(VibrationSettings s)
			{
				this.OnVibrationSettingsChanged(s);
			};
			this.GamepadVibrationTriggerRight.OnChanged += delegate(VibrationSettings s)
			{
				this.OnVibrationSettingsChanged(s);
			};
			base.CollectionBrush = Application.Current.TryFindResource("CreamBrush") as SolidColorBrush;
			base.CollectionBrushHighlighted = Application.Current.TryFindResource("CreamBrushHighlighted") as SolidColorBrush;
			base.CollectionBrushPressed = Application.Current.TryFindResource("CreamBrushPressed") as SolidColorBrush;
			this.KeyboardRepeatTypeCollection = new ObservableCollection<KeyboardRepeatType>(Enum.GetValues(typeof(KeyboardRepeatType)).Cast<KeyboardRepeatType>());
			this.OnPropertyChanged("KeyboardRepeatTypeCollection");
			base.IsChanged = false;
		}

		private void OnVibrationSettingsChanged(VibrationSettings settings)
		{
			if (settings.InsteadOfMainLeft || settings.InsteadOfMainRight || settings.InsteadOfTriggerLeft || settings.InsteadOfTriggerRight)
			{
				this.ConfigVM.ConfigData.IsVirtualMappingExist = true;
			}
			else
			{
				this.ConfigVM.ConfigData.CheckVirtualMappingsExist();
			}
			base.IsChanged = true;
			ConfigVM configVM = this.ConfigVM;
			if (configVM != null)
			{
				ConfigData configData = configVM.ConfigData;
				if (configData != null)
				{
					configData.RaiseRefreshBoundToWholeControllerProperties();
				}
			}
			PropertyChangedExtendedEventHandler onMainOrShiftCollectionItemPropertyChangedExtended = this.OnMainOrShiftCollectionItemPropertyChangedExtended;
			if (onMainOrShiftCollectionItemPropertyChangedExtended == null)
			{
				return;
			}
			onMainOrShiftCollectionItemPropertyChangedExtended(this, null);
		}

		public override void Dispose()
		{
			base.Dispose();
			foreach (ShiftXBBindingCollection shiftXBBindingCollection in this.ShiftXBBindingCollections)
			{
				shiftXBBindingCollection.Dispose();
			}
			this.ShiftXBBindingCollections.Clear();
			this.ShiftXBBindingCollections = null;
			this._currentShiftXBBindingCollection = null;
			this.ConfigVM = null;
			this.SubConfigData = null;
			this.KeyboardRepeatTypeCollection = null;
			this.OnMainOrShiftCollectionItemPropertyChangedExtended = null;
		}

		public override void InvertStickX(Stick stick)
		{
			this.SetEnablePropertyChanged(false, false);
			if (stick != 2)
			{
				foreach (ShiftXBBindingCollection shiftXBBindingCollection in this.ShiftXBBindingCollections)
				{
					shiftXBBindingCollection.CopyInheritedValuesFromMainBeforeInvertIfNeeded(stick, Orientation.Horizontal);
				}
			}
			base.InvertStickX(stick);
			if (stick != 2)
			{
				foreach (ShiftXBBindingCollection shiftXBBindingCollection2 in this.ShiftXBBindingCollections)
				{
					shiftXBBindingCollection2.InvertStickX(stick);
				}
			}
			this.SetEnablePropertyChanged(true, false);
		}

		public override void InvertStickY(Stick stick)
		{
			this.SetEnablePropertyChanged(false, false);
			if (stick != 2)
			{
				foreach (ShiftXBBindingCollection shiftXBBindingCollection in this.ShiftXBBindingCollections)
				{
					shiftXBBindingCollection.CopyInheritedValuesFromMainBeforeInvertIfNeeded(stick, Orientation.Vertical);
				}
			}
			base.InvertStickY(stick);
			if (stick != 2)
			{
				foreach (ShiftXBBindingCollection shiftXBBindingCollection2 in this.ShiftXBBindingCollections)
				{
					shiftXBBindingCollection2.InvertStickY(stick);
				}
			}
			this.SetEnablePropertyChanged(true, false);
		}

		public bool IsFirePropertyChanged
		{
			get
			{
				return this._fireBaseXBBindingPropertyChanged;
			}
		}

		public override void SetEnablePropertyChanged(bool bEnabled, bool sendAllPropertyChanged = false)
		{
			if (this._fireBaseXBBindingPropertyChanged == bEnabled)
			{
				return;
			}
			base.SetEnablePropertyChanged(bEnabled, sendAllPropertyChanged);
			if (this.ShiftXBBindingCollections != null)
			{
				foreach (ShiftXBBindingCollection shiftXBBindingCollection in this.ShiftXBBindingCollections)
				{
					shiftXBBindingCollection.SetEnablePropertyChanged(bEnabled, sendAllPropertyChanged);
				}
			}
			this._fireBaseXBBindingPropertyChanged = bEnabled;
			if (this.EnableProperyChanged != null)
			{
				this.EnableProperyChanged(this, null);
			}
			if (bEnabled)
			{
				PropertyChangedExtendedEventHandler onMainOrShiftCollectionItemPropertyChangedExtended = this.OnMainOrShiftCollectionItemPropertyChangedExtended;
				if (onMainOrShiftCollectionItemPropertyChangedExtended == null)
				{
					return;
				}
				onMainOrShiftCollectionItemPropertyChangedExtended(this, null);
			}
		}

		public override void SwapSticks()
		{
			this.SetEnablePropertyChanged(false, false);
			foreach (ShiftXBBindingCollection shiftXBBindingCollection in this.ShiftXBBindingCollections)
			{
				shiftXBBindingCollection.CopyInheritedValuesFromMainBeforeSwapSticksIfNeeded();
			}
			base.SwapSticks();
			foreach (ShiftXBBindingCollection shiftXBBindingCollection2 in this.ShiftXBBindingCollections)
			{
				shiftXBBindingCollection2.SwapSticks();
			}
			this.SetEnablePropertyChanged(true, false);
		}

		public override async Task<bool> ResetStickToDefault(Stick stick, bool askConfirmation = true)
		{
			MainXBBindingCollection.<>c__DisplayClass155_0 CS$<>8__locals1 = new MainXBBindingCollection.<>c__DisplayClass155_0();
			CS$<>8__locals1.stick = stick;
			bool flag = await base.ResetStickToDefault(CS$<>8__locals1.stick, askConfirmation);
			CS$<>8__locals1.swapped = flag;
			bool flag2;
			if (!CS$<>8__locals1.swapped)
			{
				flag2 = false;
			}
			else
			{
				this.ShiftXBBindingCollections.ForEach(delegate(ShiftXBBindingCollection col)
				{
					MainXBBindingCollection.<>c__DisplayClass155_0.<<ResetStickToDefault>b__0>d <<ResetStickToDefault>b__0>d;
					<<ResetStickToDefault>b__0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
					<<ResetStickToDefault>b__0>d.<>4__this = CS$<>8__locals1;
					<<ResetStickToDefault>b__0>d.col = col;
					<<ResetStickToDefault>b__0>d.<>1__state = -1;
					<<ResetStickToDefault>b__0>d.<>t__builder.Start<MainXBBindingCollection.<>c__DisplayClass155_0.<<ResetStickToDefault>b__0>d>(ref <<ResetStickToDefault>b__0>d);
				});
				flag2 = CS$<>8__locals1.swapped;
			}
			return flag2;
		}

		public override bool HasMouseToStick
		{
			get
			{
				if (!base.HasMouseToStick)
				{
					return this.ShiftXBBindingCollections.Any((ShiftXBBindingCollection col) => col.HasMouseToStick);
				}
				return true;
			}
		}

		public override bool HasRumble
		{
			get
			{
				if (!base.HasRumble)
				{
					return this.ShiftXBBindingCollections.Any((ShiftXBBindingCollection col2) => col2.HasRumble);
				}
				return true;
			}
		}

		public event PropertyChangedExtendedEventHandler OnMainOrShiftCollectionItemPropertyChangedExtended;

		public void RaiseOnMainOrShiftCollectionItemPropertyChangedExtended(object s, PropertyChangedExtendedEventArgs e)
		{
			PropertyChangedExtendedEventHandler onMainOrShiftCollectionItemPropertyChangedExtended = this.OnMainOrShiftCollectionItemPropertyChangedExtended;
			if (onMainOrShiftCollectionItemPropertyChangedExtended == null)
			{
				return;
			}
			onMainOrShiftCollectionItemPropertyChangedExtended(s, e);
		}

		protected override void OnMaskCollectionChangedExtended(object s, NotifyCollectionChangedEventArgs e)
		{
			PropertyChangedExtendedEventHandler onMainOrShiftCollectionItemPropertyChangedExtended = this.OnMainOrShiftCollectionItemPropertyChangedExtended;
			if (onMainOrShiftCollectionItemPropertyChangedExtended == null)
			{
				return;
			}
			onMainOrShiftCollectionItemPropertyChangedExtended(s, null);
		}

		protected override void OnCollectionItemPropertyChangedExtended(object s, PropertyChangedExtendedEventArgs e)
		{
			if (e.PropertyName == "MacroSequenceAnnotation" || e.PropertyName == "IsInheritedBinding" || e.PropertyName == "CurrentActivatorXBBinding")
			{
				return;
			}
			base.OnCollectionItemPropertyChangedExtended(s, e);
			this.RaiseOnMainOrShiftCollectionItemPropertyChangedExtended(s, e);
		}

		public ShiftXBBindingCollection AddShift(ShiftType shiftType = 0, int index = -1)
		{
			ShiftXBBindingCollection shiftXBBindingCollection = new ShiftXBBindingCollection((index == -1) ? (this.ShiftXBBindingCollections.Count + 1) : index, this, this.SubConfigData.ControllerFamily, shiftType);
			if (index == -1)
			{
				this.ShiftXBBindingCollections.Add(shiftXBBindingCollection);
			}
			else
			{
				this.ShiftXBBindingCollections.Insert(index - 1, shiftXBBindingCollection);
			}
			shiftXBBindingCollection.AttachMaskCollectionPropertyChanged();
			shiftXBBindingCollection.IsChangedModifiedEvent += base.RiseIsChangedModifiedEvent;
			shiftXBBindingCollection.ControllerBindings.CreateOnDemand = true;
			this.CorrectShiftIndexes();
			return shiftXBBindingCollection;
		}

		public override bool CanSave(bool verbose, ref bool errorIsShown)
		{
			bool flag = base.CanSave(verbose, ref errorIsShown);
			bool flag2 = true;
			foreach (ShiftXBBindingCollection shiftXBBindingCollection in this.ShiftXBBindingCollections)
			{
				foreach (KeyValuePair<GamepadButton, XBBinding> keyValuePair in ((IEnumerable<KeyValuePair<GamepadButton, XBBinding>>)shiftXBBindingCollection))
				{
					flag2 = flag2 && keyValuePair.Value.CanSave(verbose, ref errorIsShown);
				}
			}
			return flag && flag2;
		}

		public override bool SaveChanges(bool verbose, ref bool errorIsShown)
		{
			bool flag = base.SaveChanges(verbose, ref errorIsShown);
			foreach (ShiftXBBindingCollection shiftXBBindingCollection in this.ShiftXBBindingCollections)
			{
				flag = shiftXBBindingCollection.SaveChanges(verbose, ref errorIsShown) && flag;
			}
			base.IsChanged = false;
			return flag;
		}

		public void PrepareForSave()
		{
			foreach (XBBinding xbbinding in base.Values)
			{
				xbbinding.ActivatorXBBindingDictionary.PrepareForSave();
			}
			foreach (ControllerBinding controllerBinding in base.ControllerBindings)
			{
				controllerBinding.XBBinding.PrepareForSave();
			}
			foreach (ShiftXBBindingCollection shiftXBBindingCollection in this.ShiftXBBindingCollections)
			{
				foreach (XBBinding xbbinding2 in shiftXBBindingCollection.Values)
				{
					xbbinding2.ActivatorXBBindingDictionary.PrepareForSave();
				}
			}
		}

		public void OnAfterCollectionRead()
		{
			base.ForEachValue(delegate(XBBinding xb)
			{
				xb.SetNonEmptyCurrentActivator(false);
			});
			foreach (ControllerBinding controllerBinding in base.ControllerBindings)
			{
				if (controllerBinding != null)
				{
					controllerBinding.XBBinding.SetNonEmptyCurrentActivator(false);
				}
			}
			foreach (ShiftXBBindingCollection shiftXBBindingCollection in this.ShiftXBBindingCollections)
			{
				shiftXBBindingCollection.ForEachValue(delegate(XBBinding xb)
				{
					xb.SetNonEmptyCurrentActivator(false);
				});
				foreach (ControllerBinding controllerBinding2 in shiftXBBindingCollection.ControllerBindings)
				{
					if (controllerBinding2 != null)
					{
						controllerBinding2.XBBinding.SetNonEmptyCurrentActivator(false);
					}
				}
			}
		}

		public override void ResetCurrentMappingsAndOtherSelectors()
		{
			reWASDApplicationCommands.NavigateBindingFrameCommand.Execute(null);
			base.ResetCurrentMappingsAndOtherSelectors();
			this.IsLEDSettingsView = false;
			ObservableCollection<ShiftXBBindingCollection> shiftXBBindingCollections = this.ShiftXBBindingCollections;
			if (shiftXBBindingCollections == null)
			{
				return;
			}
			shiftXBBindingCollections.ForEach(delegate(ShiftXBBindingCollection sc)
			{
				sc.ResetCurrentMappingsAndOtherSelectors();
			});
		}

		private void CreateShiftCollections(ControllerFamily controllerFamily)
		{
			this.ShiftXBBindingCollections = new ObservableCollection<ShiftXBBindingCollection>();
		}

		public override void AttachMaskCollectionPropertyChanged()
		{
			base.AttachMaskCollectionPropertyChanged();
			if (this.ShiftXBBindingCollections != null)
			{
				this.ShiftXBBindingCollections.ForEach(delegate(ShiftXBBindingCollection sc)
				{
					sc.AttachMaskCollectionPropertyChanged();
				});
			}
		}

		private void RefreshCurrentActivators()
		{
			this.RealCurrentBeingMappedBindingCollection.EnumAllBindings(true, true, true).ForEach(delegate(XBBinding item)
			{
				item.CurrentActivatorXBBinding = item.ActivatorXBBindingDictionary.GetNonEmptyActivator();
			});
		}

		public void ClearVirtualMappings()
		{
			this.GamepadVibrationMainLeft.ResetToDefaults();
			this.GamepadVibrationMainRight.ResetToDefaults();
			this.GamepadVibrationTriggerLeft.ResetToDefaults();
			this.GamepadVibrationTriggerRight.ResetToDefaults();
			base.EnumAllBindings(true, true, true).ForEach(delegate(XBBinding xbBinding)
			{
				xbBinding.ClearVirtualMappings();
			});
			if (this.ShiftXBBindingCollections != null)
			{
				this.ShiftXBBindingCollections.ForEach(delegate(ShiftXBBindingCollection sc)
				{
					sc.EnumAllBindings(true, true, true).ForEach(delegate(XBBinding xbBinding)
					{
						xbBinding.ClearVirtualMappings();
					});
				});
			}
		}

		public bool IsAnyAdaptiveTriggerSettingsPresent()
		{
			return this.IsMainAdaptiveTriggerSettingsPresent() || this.IsAnyShiftAdaptiveTriggerSettingsPresent();
		}

		public bool IsMainAdaptiveTriggerSettingsPresent()
		{
			return base.IsAdaptiveLeftTriggerSettingsPresent || base.IsAdaptiveRightTriggerSettingsPresent;
		}

		public bool IsAnyAdaptiveLeftTriggerSettingsPresent()
		{
			return base.IsAdaptiveLeftTriggerSettingsPresent || this.IsAnyShiftAdaptiveLeftTriggerSettingsPresent();
		}

		public bool IsAnyAdaptiveRightTriggerSettingsPresent()
		{
			return base.IsAdaptiveRightTriggerSettingsPresent || this.IsAnyShiftAdaptiveRightTriggerSettingsPresent();
		}

		public bool IsAnyShiftAdaptiveTriggerSettingsPresent()
		{
			return this.IsAnyShiftAdaptiveLeftTriggerSettingsPresent() || this.IsAnyShiftAdaptiveRightTriggerSettingsPresent();
		}

		public bool IsAnyShiftAdaptiveLeftTriggerSettingsPresent()
		{
			if (this.ShiftXBBindingCollections == null)
			{
				return false;
			}
			return this.ShiftXBBindingCollections.Any((ShiftXBBindingCollection shiftCol) => shiftCol != null && shiftCol.IsAdaptiveLeftTriggerSettingsPresent);
		}

		public bool IsAnyShiftAdaptiveRightTriggerSettingsPresent()
		{
			if (this.ShiftXBBindingCollections == null)
			{
				return false;
			}
			return this.ShiftXBBindingCollections.Any((ShiftXBBindingCollection shiftCol) => shiftCol != null && shiftCol.IsAdaptiveRightTriggerSettingsPresent);
		}

		public void CopyFromModel(MainXBBindingCollection model)
		{
			this.LeftTrigger.Low = model.LeftTrigger.Low;
			this.LeftTrigger.Med = model.LeftTrigger.Med;
			this.LeftTrigger.High = model.LeftTrigger.High;
			this.LeftTrigger.RightDeadZone = model.LeftTrigger.RightDeadZone;
			this.IsHardwareDeadzoneLeftTrigger = model.IsHardwareDeadzoneLeftTrigger;
			this.RightTrigger.Low = model.RightTrigger.Low;
			this.RightTrigger.Med = model.RightTrigger.Med;
			this.RightTrigger.High = model.RightTrigger.High;
			this.RightTrigger.RightDeadZone = model.RightTrigger.RightDeadZone;
			this.IsHardwareDeadzoneRightTrigger = model.IsHardwareDeadzoneRightTrigger;
			this.GamepadVibrationMainLeft.CopyFromModel(model.GamepadVibrationMainLeft);
			this.GamepadVibrationMainRight.CopyFromModel(model.GamepadVibrationMainRight);
			this.GamepadVibrationTriggerLeft.CopyFromModel(model.GamepadVibrationTriggerLeft);
			this.GamepadVibrationTriggerRight.CopyFromModel(model.GamepadVibrationTriggerRight);
			this.LeftBumper.CopyFromModel(model.LeftBumper);
			this.RightBumper.CopyFromModel(model.RightBumper);
			this.DS3CrossAnalog.CopyFromModel(model.DS3CrossAnalog);
			this.DS3CircleAnalog.CopyFromModel(model.DS3CircleAnalog);
			this.DS3SquareAnalog.CopyFromModel(model.DS3SquareAnalog);
			this.DS3TriangleAnalog.CopyFromModel(model.DS3TriangleAnalog);
			this.DS3DPADUpAnalog.CopyFromModel(model.DS3DPADUpAnalog);
			this.DS3DPADDownAnalog.CopyFromModel(model.DS3DPADDownAnalog);
			this.DS3DPADLeftAnalog.CopyFromModel(model.DS3DPADLeftAnalog);
			this.DS3DPADRightAnalog.CopyFromModel(model.DS3DPADRightAnalog);
			this.SteamDeckTrackpad1Pressure.CopyFromModel(model.SteamDeckTrackpad1Pressure);
			this.SteamDeckTrackpad2Pressure.CopyFromModel(model.SteamDeckTrackpad2Pressure);
			this.MouseSensitivity = model.MouseSensitivity;
			this.MouseDeflection = model.MouseDeflection;
			this.WheelDeflection = model.WheelDeflection;
			this.MouseAcceleration = model.MouseAcceleration;
			this.VirtualKeyboardRepeatRate = model.VirtualKeyboardRepeatRate;
			this.VirtualKeyboardRepeatValue = model.VirtualKeyboardRepeatValue;
			base.CopyFromModel(model, true);
			for (int i = 0; i < this.ShiftXBBindingCollections.Count; i++)
			{
				this.ShiftXBBindingCollections[i].CopyFromModel(model.ShiftXBBindingCollections[i]);
			}
		}

		public void CopyFromModelForPresets(MainXBBindingCollection model, bool isCopyToNotEmpty = true)
		{
			if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.LeftTrigger == new ZoneValues(this.LeftTrigger.RightDeadZone, this.LeftTrigger.Low)))
			{
				this.LeftTrigger.Low = model.LeftTrigger.Low;
				this.LeftTrigger.Med = model.LeftTrigger.Med;
				this.LeftTrigger.High = model.LeftTrigger.High;
				this.LeftTrigger.RightDeadZone = model.LeftTrigger.RightDeadZone;
				this.IsHardwareDeadzoneLeftTrigger = model.IsHardwareDeadzoneLeftTrigger;
			}
			if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.LeftBumper == new ZoneValues(this.LeftBumper.RightDeadZone, this.LeftBumper.Low)))
			{
				this.RightTrigger.Low = model.RightTrigger.Low;
				this.RightTrigger.Med = model.RightTrigger.Med;
				this.RightTrigger.High = model.RightTrigger.High;
				this.RightTrigger.RightDeadZone = model.RightTrigger.RightDeadZone;
				this.IsHardwareDeadzoneRightTrigger = model.IsHardwareDeadzoneRightTrigger;
			}
			bool flag = false;
			try
			{
				if (isCopyToNotEmpty || (!isCopyToNotEmpty && base.LeftStickDirectionalGroup.IsAdvancedDefault()))
				{
					if (!flag && model.IsLeftStickSwapped && !base.IsLeftStickSwapped)
					{
						this.SwapSticks();
						flag = true;
					}
					base.LeftStickDirectionalGroup.CopyFromModel(model.LeftStickDirectionalGroup);
					if (model.LeftStickDirectionalGroup.IsXInvert && !base.LeftStickDirectionalGroup.IsXInvert)
					{
						base.LeftStickDirectionalGroup.InvertStickX(true, true);
					}
					if (model.LeftStickDirectionalGroup.IsYInvert && !base.LeftStickDirectionalGroup.IsYInvert)
					{
						base.LeftStickDirectionalGroup.InvertStickY(true, true);
					}
				}
			}
			catch (Exception)
			{
			}
			try
			{
				if (isCopyToNotEmpty || (!isCopyToNotEmpty && base.RightStickDirectionalGroup.IsAdvancedDefault()))
				{
					if (!flag && model.IsRightStickSwapped && !base.IsRightStickSwapped)
					{
						this.SwapSticks();
					}
					base.RightStickDirectionalGroup.CopyFromModel(model.RightStickDirectionalGroup);
					if (model.RightStickDirectionalGroup.IsXInvert && !base.RightStickDirectionalGroup.IsXInvert)
					{
						base.RightStickDirectionalGroup.InvertStickX(true, true);
					}
					if (model.RightStickDirectionalGroup.IsYInvert && !base.RightStickDirectionalGroup.IsYInvert)
					{
						base.RightStickDirectionalGroup.InvertStickY(true, true);
					}
				}
			}
			catch (Exception)
			{
			}
			if (base.ContainsKey(171))
			{
				XBBinding xbbinding = base[171];
				if (isCopyToNotEmpty || (!isCopyToNotEmpty && xbbinding.IsEmpty))
				{
					base.IsSameScrollDelta = model.IsSameScrollDelta;
					base[171].CopyFromModel(model[171]);
				}
			}
			if (base.ContainsKey(172))
			{
				XBBinding xbbinding2 = base[172];
				if (isCopyToNotEmpty || (!isCopyToNotEmpty && xbbinding2.IsEmpty))
				{
					base.IsSameScrollDelta = model.IsSameScrollDelta;
					base[172].CopyFromModel(model[172]);
				}
			}
			if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.GamepadVibrationMainLeft.IsDefault() && this.GamepadVibrationMainRight.IsDefault() && this.GamepadVibrationTriggerLeft.IsDefault() && this.GamepadVibrationTriggerRight.IsDefault()))
			{
				this.GamepadVibrationMainLeft.CopyFromModel(model.GamepadVibrationMainLeft);
				this.GamepadVibrationMainRight.CopyFromModel(model.GamepadVibrationMainRight);
				this.GamepadVibrationTriggerLeft.CopyFromModel(model.GamepadVibrationTriggerLeft);
				this.GamepadVibrationTriggerRight.CopyFromModel(model.GamepadVibrationTriggerRight);
			}
			if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.LeftBumper == new ZoneValues(this.LeftBumper.RightDeadZone, this.LeftBumper.Low)))
			{
				this.LeftBumper.CopyFromModel(model.LeftBumper);
			}
			if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.RightBumper == new ZoneValues(this.RightBumper.RightDeadZone, this.RightBumper.Low)))
			{
				this.RightBumper.CopyFromModel(model.RightBumper);
			}
			if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.DS3CrossAnalog == new ZoneValues(this.DS3CrossAnalog.RightDeadZone, this.DS3CrossAnalog.Low)))
			{
				this.DS3CrossAnalog.CopyFromModel(model.DS3CrossAnalog);
			}
			if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.DS3CircleAnalog == new ZoneValues(this.DS3CircleAnalog.RightDeadZone, this.DS3CircleAnalog.Low)))
			{
				this.DS3CircleAnalog.CopyFromModel(model.DS3CircleAnalog);
			}
			if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.DS3SquareAnalog == new ZoneValues(this.DS3SquareAnalog.RightDeadZone, this.DS3SquareAnalog.Low)))
			{
				this.DS3SquareAnalog.CopyFromModel(model.DS3SquareAnalog);
			}
			if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.DS3TriangleAnalog == new ZoneValues(this.DS3TriangleAnalog.RightDeadZone, this.DS3TriangleAnalog.Low)))
			{
				this.DS3TriangleAnalog.CopyFromModel(model.DS3TriangleAnalog);
			}
			if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.DS3DPADUpAnalog == new ZoneValues(this.DS3DPADUpAnalog.RightDeadZone, this.DS3DPADUpAnalog.Low)))
			{
				this.DS3DPADUpAnalog.CopyFromModel(model.DS3DPADUpAnalog);
			}
			if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.DS3DPADDownAnalog == new ZoneValues(this.DS3DPADDownAnalog.RightDeadZone, this.DS3DPADDownAnalog.Low)))
			{
				this.DS3DPADDownAnalog.CopyFromModel(model.DS3DPADDownAnalog);
			}
			if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.DS3DPADLeftAnalog == new ZoneValues(this.DS3DPADLeftAnalog.RightDeadZone, this.DS3DPADLeftAnalog.Low)))
			{
				this.DS3DPADLeftAnalog.CopyFromModel(model.DS3DPADLeftAnalog);
			}
			if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.DS3DPADRightAnalog == new ZoneValues(this.DS3DPADRightAnalog.RightDeadZone, this.DS3DPADRightAnalog.Low)))
			{
				this.DS3DPADRightAnalog.CopyFromModel(model.DS3DPADRightAnalog);
			}
			if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.SteamDeckTrackpad1Pressure == new ZoneValues(this.SteamDeckTrackpad1Pressure.RightDeadZone, this.SteamDeckTrackpad1Pressure.Low)))
			{
				this.SteamDeckTrackpad1Pressure.CopyFromModel(model.SteamDeckTrackpad1Pressure);
			}
			if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.SteamDeckTrackpad2Pressure == new ZoneValues(this.SteamDeckTrackpad2Pressure.RightDeadZone, this.SteamDeckTrackpad2Pressure.Low)))
			{
				this.SteamDeckTrackpad2Pressure.CopyFromModel(model.SteamDeckTrackpad2Pressure);
			}
			if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.MouseSensitivity == 4 && this.MouseDeflection == 8 && this.WheelDeflection == 13 && this.MouseAcceleration == 8))
			{
				this.MouseSensitivity = model.MouseSensitivity;
				this.MouseDeflection = model.MouseDeflection;
				this.WheelDeflection = model.WheelDeflection;
				this.MouseAcceleration = model.MouseAcceleration;
			}
			if (isCopyToNotEmpty || (!isCopyToNotEmpty && this.VirtualKeyboardRepeatRate == null))
			{
				this.VirtualKeyboardRepeatRate = model.VirtualKeyboardRepeatRate;
				this.VirtualKeyboardRepeatValue = model.VirtualKeyboardRepeatValue;
			}
		}

		public void CopyToModel(MainXBBindingCollection model)
		{
			model.LeftTrigger.Low = this.LeftTrigger.Low;
			model.LeftTrigger.Med = this.LeftTrigger.Med;
			model.LeftTrigger.High = this.LeftTrigger.High;
			model.LeftTrigger.RightDeadZone = this.LeftTrigger.RightDeadZone;
			model.IsHardwareDeadzoneLeftTrigger = this.IsHardwareDeadzoneLeftTrigger;
			model.RightTrigger.Low = this.RightTrigger.Low;
			model.RightTrigger.Med = this.RightTrigger.Med;
			model.RightTrigger.High = this.RightTrigger.High;
			model.RightTrigger.RightDeadZone = this.RightTrigger.RightDeadZone;
			model.IsHardwareDeadzoneRightTrigger = this.IsHardwareDeadzoneRightTrigger;
			this.GamepadVibrationMainLeft.CopyToModel(model.GamepadVibrationMainLeft);
			this.GamepadVibrationMainRight.CopyToModel(model.GamepadVibrationMainRight);
			this.GamepadVibrationTriggerLeft.CopyToModel(model.GamepadVibrationTriggerLeft);
			this.GamepadVibrationTriggerRight.CopyToModel(model.GamepadVibrationTriggerRight);
			this.LeftBumper.CopyToModel(model.LeftBumper);
			this.RightBumper.CopyToModel(model.RightBumper);
			this.DS3CrossAnalog.CopyToModel(model.DS3CrossAnalog);
			this.DS3CircleAnalog.CopyToModel(model.DS3CircleAnalog);
			this.DS3SquareAnalog.CopyToModel(model.DS3SquareAnalog);
			this.DS3TriangleAnalog.CopyToModel(model.DS3TriangleAnalog);
			this.DS3DPADUpAnalog.CopyToModel(model.DS3DPADUpAnalog);
			this.DS3DPADDownAnalog.CopyToModel(model.DS3DPADDownAnalog);
			this.DS3DPADLeftAnalog.CopyToModel(model.DS3DPADLeftAnalog);
			this.DS3DPADRightAnalog.CopyToModel(model.DS3DPADRightAnalog);
			this.SteamDeckTrackpad1Pressure.CopyToModel(model.SteamDeckTrackpad1Pressure);
			this.SteamDeckTrackpad2Pressure.CopyToModel(model.SteamDeckTrackpad2Pressure);
			model.MouseSensitivity = this.MouseSensitivity;
			model.MouseDeflection = this.MouseDeflection;
			model.WheelDeflection = this.WheelDeflection;
			model.MouseAcceleration = this.MouseAcceleration;
			model.VirtualKeyboardRepeatRate = this.VirtualKeyboardRepeatRate;
			model.VirtualKeyboardRepeatValue = this.VirtualKeyboardRepeatValue;
			base.CopyToModel(model);
			for (int i = 0; i < this.ShiftXBBindingCollections.Count; i++)
			{
				this.ShiftXBBindingCollections[i].CopyToModel(model.ShiftXBBindingCollections[i]);
			}
		}

		private DelegateCommand _resetVibrationSettingsCommand;

		private bool _isHardwareDeadzoneLeftTrigger;

		private bool _isHardwareDeadzoneRightTrigger;

		private bool _isLEDSettingsView;

		private KeyboardRepeatType _virtualKeyboardRepeatRate;

		private ushort _virtualKeyboardRepeatValue;

		private ushort _mouseSensitivity;

		private ushort _mouseDeflection;

		private ushort _wheelDeflection;

		private ushort _mouseAcceleration;

		private ShiftXBBindingCollection _currentShiftXBBindingCollection;

		public EventHandler EnableProperyChanged;

		private bool _fireBaseXBBindingPropertyChanged = true;
	}
}
