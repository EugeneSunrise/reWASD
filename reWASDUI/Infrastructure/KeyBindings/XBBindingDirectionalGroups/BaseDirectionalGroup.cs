using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using DiscSoft.NET.Common.Interfaces;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.View.SecondaryWindows;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings.MacroBinding;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.Services.Interfaces;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Infrastructure.reWASDMapping.RewasduserCommands;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups
{
	public abstract class BaseDirectionalGroup : ZBindableBase, IIndexable
	{
		protected ControllerTypeEnum? CurrentControllerType
		{
			get
			{
				IGamepadService gamepadService = App.GamepadService;
				if (gamepadService == null)
				{
					return null;
				}
				BaseControllerVM currentGamepad = gamepadService.CurrentGamepad;
				if (currentGamepad == null)
				{
					return null;
				}
				return currentGamepad.GetGamepadTypeByIndex(this.HostCollection.SubConfigData.IndexByControllerFamily + 1);
			}
		}

		protected ControllerVM CurrentController
		{
			get
			{
				IGamepadService gamepadService = App.GamepadService;
				if (gamepadService == null)
				{
					return null;
				}
				BaseControllerVM currentGamepad = gamepadService.CurrentGamepad;
				if (currentGamepad == null)
				{
					return null;
				}
				return currentGamepad.CurrentController;
			}
		}

		public abstract string GroupLabel { get; }

		public virtual string GroupMapToLabel
		{
			get
			{
				return string.Format(DTLocalization.GetString(11611), this.GroupLabel);
			}
		}

		public abstract GamepadButton LeftDirection { get; }

		public abstract GamepadButton UpDirection { get; }

		public abstract GamepadButton RightDirection { get; }

		public abstract GamepadButton DownDirection { get; }

		public XBBinding LeftDirectionValue
		{
			get
			{
				return this.HostCollection[this.LeftDirection];
			}
			set
			{
				this.HostCollection[this.LeftDirection] = value;
			}
		}

		public XBBinding UpDirectionValue
		{
			get
			{
				return this.HostCollection[this.UpDirection];
			}
			set
			{
				this.HostCollection[this.UpDirection] = value;
			}
		}

		public XBBinding RightDirectionValue
		{
			get
			{
				return this.HostCollection[this.RightDirection];
			}
			set
			{
				this.HostCollection[this.RightDirection] = value;
			}
		}

		public XBBinding DownDirectionValue
		{
			get
			{
				return this.HostCollection[this.DownDirection];
			}
			set
			{
				this.HostCollection[this.DownDirection] = value;
			}
		}

		public virtual bool IsWASDGroupMappingAvailable
		{
			get
			{
				return true;
			}
		}

		public bool IsBoundToWASD
		{
			get
			{
				return this.LeftDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.DikA && this.UpDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.DikW && this.RightDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.DikD && this.DownDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.DikS;
			}
		}

		public virtual bool IsFlickStickGroupMappingAvailable
		{
			get
			{
				return false;
			}
		}

		public bool IsBoundToFlickStick
		{
			get
			{
				return (this.LeftDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseFlickLeft && this.RightDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseFlickRight) || (this.RightDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseFlickLeft && this.LeftDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseFlickRight);
			}
		}

		public virtual bool IsArrowsGroupMappingAvailable
		{
			get
			{
				return true;
			}
		}

		public bool IsBoundToArrows
		{
			get
			{
				return this.LeftDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.DikLeft && this.UpDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.DikUp && this.RightDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.DikRight && this.DownDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.DikDown;
			}
		}

		public virtual bool IsMouseGroupMappingAvailable
		{
			get
			{
				return true;
			}
		}

		public virtual bool IsDS4TouchpadGroupMappingAvailable
		{
			get
			{
				return false;
			}
		}

		public bool IsBoundToMouse
		{
			get
			{
				return this.LeftDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseXLeft && this.UpDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseYDown && this.RightDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseXRight && this.DownDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseYUp;
			}
		}

		public bool IsHasMouseDirection
		{
			get
			{
				return this.LeftDirectionValue.SingleActivator.MappedKey.IsMouseDirection || this.UpDirectionValue.SingleActivator.MappedKey.IsMouseDirection || this.RightDirectionValue.SingleActivator.MappedKey.IsMouseDirection || this.DownDirectionValue.SingleActivator.MappedKey.IsMouseDirection;
			}
		}

		public bool IsBoundToOverlayMenuDirections
		{
			get
			{
				return this.UpDirectionValue.SingleActivator.MappedKey.IsUserCommand && ((BaseRewasdUserCommand)this.UpDirectionValue.SingleActivator.MappedKey).CommandId == 134 && this.DownDirectionValue.SingleActivator.MappedKey.IsUserCommand && ((BaseRewasdUserCommand)this.DownDirectionValue.SingleActivator.MappedKey).CommandId == 135 && this.LeftDirectionValue.SingleActivator.MappedKey.IsUserCommand && ((BaseRewasdUserCommand)this.LeftDirectionValue.SingleActivator.MappedKey).CommandId == 137 && this.RightDirectionValue.SingleActivator.MappedKey.IsUserCommand && ((BaseRewasdUserCommand)this.RightDirectionValue.SingleActivator.MappedKey).CommandId == 136;
			}
		}

		public bool IsBoundToDS4Touchpad
		{
			get
			{
				return this.LeftDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadTouchpadLeft && this.UpDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadTouchpadUp && this.RightDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadTouchpadRight && this.DownDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadTouchpadDown;
			}
		}

		public bool IsBoundToYInvertedMouse
		{
			get
			{
				return this.LeftDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseXLeft && this.UpDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseYUp && this.RightDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseXRight && this.DownDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseYDown;
			}
		}

		public bool IsBoundToAnyInvertedMouse
		{
			get
			{
				return ((this.UpDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseYUp && this.DownDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseYDown) || (this.UpDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseYDown && this.DownDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseYUp)) && ((this.LeftDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseXLeft && this.RightDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseXRight) || (this.LeftDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseXRight && this.RightDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.MouseXLeft));
			}
		}

		public bool IsBoundToKeyboard
		{
			get
			{
				return this.LeftDirectionValue.SingleActivator.MappedKey.IsKeyboard && this.UpDirectionValue.SingleActivator.MappedKey.IsKeyboard && this.RightDirectionValue.SingleActivator.MappedKey.IsKeyboard && this.DownDirectionValue.SingleActivator.MappedKey.IsKeyboard;
			}
		}

		public virtual bool IsVirtualLeftStickGroupMappingAvaliable
		{
			get
			{
				return true;
			}
		}

		public bool IsBoundToVirtualLeftStick
		{
			get
			{
				bool flag = (this.LeftDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonLeftStickLeft && this.RightDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonLeftStickRight) || (this.LeftDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonLeftStickRight && this.RightDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonLeftStickLeft);
				bool flag2 = (this.UpDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonLeftStickUp && this.DownDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonLeftStickDown) || (this.UpDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonLeftStickDown && this.DownDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonLeftStickUp);
				return flag && flag2;
			}
		}

		public virtual bool IsVirtualRightStickGroupMappingAvaliable
		{
			get
			{
				return true;
			}
		}

		public bool IsBoundToVirtualRightStick
		{
			get
			{
				bool flag = (this.LeftDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonRightStickLeft && this.RightDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonRightStickRight) || (this.LeftDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonRightStickRight && this.RightDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonRightStickLeft);
				bool flag2 = (this.UpDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonRightStickUp && this.DownDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonRightStickDown) || (this.UpDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonRightStickDown && this.DownDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonRightStickUp);
				return flag && flag2;
			}
		}

		public bool IsVirtualDPADGroupMappingAvailable
		{
			get
			{
				return this.IsAnyAzeron;
			}
		}

		public bool IsBoundToVirtualDPAD
		{
			get
			{
				return this.LeftDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonDpadLeft && this.RightDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonDpadRight && this.UpDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonDpadUp && this.DownDirectionValue.SingleActivator.MappedKey == KeyScanCodeV2.GamepadButtonDpadDown;
			}
		}

		private ControllerTypeEnum? _currentControllerType
		{
			get
			{
				IGamepadService gamepadService = App.GamepadService;
				BaseControllerVM baseControllerVM = ((gamepadService != null) ? gamepadService.CurrentGamepad : null);
				if (baseControllerVM == null)
				{
					return null;
				}
				ControllerVM currentController = baseControllerVM.CurrentController;
				if (currentController == null)
				{
					return null;
				}
				return new ControllerTypeEnum?(currentController.ControllerType);
			}
		}

		protected bool IsAnyAzeron
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				return this._currentControllerType != null && ControllerTypeExtensions.IsAnyAzeron(controllerTypeEnum.GetValueOrDefault());
			}
		}

		protected bool IsAnySteam
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				return this._currentControllerType != null && ControllerTypeExtensions.IsAnySteam(controllerTypeEnum.GetValueOrDefault());
			}
		}

		protected bool IsGamepadWithSonyTouchpad
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				return this._currentControllerType != null && ControllerTypeExtensions.IsGamepadWithSonyTouchpad(controllerTypeEnum.GetValueOrDefault());
			}
		}

		protected bool IsNintendoSwitch
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				return this._currentControllerType != null && ControllerTypeExtensions.IsNintendoSwitch(controllerTypeEnum.GetValueOrDefault());
			}
		}

		protected bool IsFlydigi
		{
			get
			{
				ControllerTypeEnum? controllerTypeEnum;
				return this._currentControllerType != null && ControllerTypeExtensions.IsFlydigi(controllerTypeEnum.GetValueOrDefault());
			}
		}

		public virtual bool IsUnmapAvailable
		{
			get
			{
				return !this.IsAnyAzeron;
			}
		}

		public bool IsUnmapped
		{
			get
			{
				GamepadButton? gamepadButton = null;
				return (GamepadButtonDescription.Unmapped.Equals(this.LeftDirectionValue.RemapedTo) || (gamepadButton != null && this.LeftDirection == gamepadButton.Value)) && (GamepadButtonDescription.Unmapped.Equals(this.UpDirectionValue.RemapedTo) || (gamepadButton != null && this.UpDirection == gamepadButton.Value)) && (GamepadButtonDescription.Unmapped.Equals(this.RightDirectionValue.RemapedTo) || (gamepadButton != null && this.RightDirection == gamepadButton.Value)) && (GamepadButtonDescription.Unmapped.Equals(this.DownDirectionValue.RemapedTo) || (gamepadButton != null && this.DownDirection == gamepadButton.Value));
			}
		}

		public bool IsAnyDirectionRemapedOrUnmapped
		{
			get
			{
				return this.LeftDirectionValue.IsRemapedOrUnmapped || this.UpDirectionValue.IsRemapedOrUnmapped || this.RightDirectionValue.IsRemapedOrUnmapped || this.DownDirectionValue.IsRemapedOrUnmapped;
			}
		}

		public bool IsAnyDirectionRemapedOrUnmappedShouldBeShown
		{
			get
			{
				return this.LeftDirectionValue.IsRemapedOrUnmappedShouldBeShown || this.UpDirectionValue.IsRemapedOrUnmappedShouldBeShown || this.RightDirectionValue.IsRemapedOrUnmappedShouldBeShown || this.DownDirectionValue.IsRemapedOrUnmappedShouldBeShown;
			}
		}

		public bool IsAnyDirectionVirtualMappingPresent
		{
			get
			{
				return this.LeftDirectionValue.IsAnyActivatorVirtualMappingPresent || this.UpDirectionValue.IsAnyActivatorVirtualMappingPresent || this.RightDirectionValue.IsAnyActivatorVirtualMappingPresent || this.DownDirectionValue.IsAnyActivatorVirtualMappingPresent;
			}
		}

		public bool IsAnyDescriptionPresent
		{
			get
			{
				return this.LeftDirectionValue.IsAnyActivatorDescriptionPresent || this.UpDirectionValue.IsAnyActivatorDescriptionPresent || this.RightDirectionValue.IsAnyActivatorDescriptionPresent || this.DownDirectionValue.IsAnyActivatorDescriptionPresent;
			}
		}

		public bool IsAnyDirectionHasJumpToShift
		{
			get
			{
				return this.LeftDirectionValue.CurrentActivatorXBBinding.IsJumpToShift || this.UpDirectionValue.CurrentActivatorXBBinding.IsJumpToShift || this.RightDirectionValue.CurrentActivatorXBBinding.IsJumpToShift || this.DownDirectionValue.CurrentActivatorXBBinding.IsJumpToShift;
			}
		}

		public void ClearDirectionAllJumpToShift()
		{
			this.LeftDirectionValue.CurrentActivatorXBBinding.ClearJumpToShift();
			this.UpDirectionValue.CurrentActivatorXBBinding.ClearJumpToShift();
			this.RightDirectionValue.CurrentActivatorXBBinding.ClearJumpToShift();
			this.DownDirectionValue.CurrentActivatorXBBinding.ClearJumpToShift();
		}

		public bool IsDefaultValues
		{
			get
			{
				return !this.IsAnyDirectionVirtualMappingPresent && !this.IsAnyDescriptionPresent && !this.IsAnyDirectionRemapedOrUnmapped;
			}
		}

		public bool IsChanged
		{
			get
			{
				return this.HostCollection.IsChanged;
			}
			set
			{
				this.HostCollection.IsChanged = value;
			}
		}

		public bool CructhForFirePropertyChanged
		{
			get
			{
				return this._cructhForFirePropertyChanged;
			}
			set
			{
				this.SetProperty<bool>(ref this._cructhForFirePropertyChanged, value, "CructhForFirePropertyChanged");
			}
		}

		public BaseXBBindingCollection HostCollection { get; set; }

		public bool IsGyroTiltGroup
		{
			get
			{
				return this is GyroTiltDirectionalGroup;
			}
		}

		public bool IsDPADGroup
		{
			get
			{
				return this is DPADDirectionalGroup;
			}
		}

		public bool IsLeftStickGroup
		{
			get
			{
				return this is LeftStickDirectionalGroup;
			}
		}

		public bool IsRightStickGroup
		{
			get
			{
				return this is RightStickDirectionalGroup;
			}
		}

		public bool IsMouseGroup
		{
			get
			{
				return this is MouseDirectionalGroup;
			}
		}

		public bool IsTrackPad1Group
		{
			get
			{
				return this is Touchpad1DirectionalGroup;
			}
		}

		public bool IsTrackPad2Group
		{
			get
			{
				return this is Touchpad2DirectionalGroup;
			}
		}

		public bool IsAnyTrackPadGroup
		{
			get
			{
				return this.IsTrackPad1Group || this.IsTrackPad2Group;
			}
		}

		public BaseDirectionalGroup(BaseXBBindingCollection baseHostCollection)
		{
			this.HostCollection = baseHostCollection;
			this.SetDefaultValues(true);
			TranslationManager.Instance.LanguageChanged += delegate([Nullable(2)] object s, EventArgs e)
			{
				this.OnPropertyChanged("GroupMapToLabel");
			};
		}

		public virtual void Unbind()
		{
			this.UnbindMacroOnly();
			this.LeftDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.NoMap;
			this.UpDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.NoMap;
			this.RightDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.NoMap;
			this.DownDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.NoMap;
			this.UpdateProperties();
		}

		public virtual void UnbindMacroOnly()
		{
			this.LeftDirectionValue.SingleActivator.ClearMacroSequence(true);
			this.UpDirectionValue.SingleActivator.ClearMacroSequence(true);
			this.RightDirectionValue.SingleActivator.ClearMacroSequence(true);
			this.DownDirectionValue.SingleActivator.ClearMacroSequence(true);
		}

		public virtual void Unmap()
		{
			GamepadButton? gamepadButton = null;
			if (gamepadButton == null || gamepadButton.Value != this.LeftDirection)
			{
				this.LeftDirectionValue.RemapedTo = GamepadButtonDescription.Unmapped;
			}
			if (gamepadButton == null || gamepadButton.Value != this.UpDirection)
			{
				this.UpDirectionValue.RemapedTo = GamepadButtonDescription.Unmapped;
			}
			if (gamepadButton == null || gamepadButton.Value != this.RightDirection)
			{
				this.RightDirectionValue.RemapedTo = GamepadButtonDescription.Unmapped;
			}
			if (gamepadButton == null || gamepadButton.Value != this.DownDirection)
			{
				this.DownDirectionValue.RemapedTo = GamepadButtonDescription.Unmapped;
			}
			this.UpdateProperties();
		}

		public virtual void RevertRemapToDefault()
		{
			this.LeftDirectionValue.RevertRemap();
			this.UpDirectionValue.RevertRemap();
			this.RightDirectionValue.RevertRemap();
			this.DownDirectionValue.RevertRemap();
			this.UpdateProperties();
		}

		public virtual void ToggleUnmap()
		{
			if (this.IsUnmapped)
			{
				this.RevertRemapToDefault();
			}
			else
			{
				this.Unmap();
			}
			App.EventAggregator.GetEvent<RequestRefreshAnnotationVisibilityForGroup>().Publish(this);
		}

		public void BindToWASD()
		{
			this.LeftDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.DikA;
			this.UpDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.DikW;
			this.RightDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.DikD;
			this.DownDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.DikS;
			this.UpdateProperties();
		}

		public void BindToFlickStick()
		{
			this.LeftDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.MouseFlickLeft;
			this.RightDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.MouseFlickRight;
			this.UpdateProperties();
		}

		public void BindToFlickStickInverted()
		{
			this.LeftDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.MouseFlickRight;
			this.RightDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.MouseFlickLeft;
			this.UpdateProperties();
		}

		public void BindToArrows()
		{
			this.LeftDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.DikLeft;
			this.UpDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.DikUp;
			this.RightDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.DikRight;
			this.DownDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.DikDown;
			this.UpdateProperties();
		}

		public void BindToMouse()
		{
			this.LeftDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.MouseXLeft;
			this.UpDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.MouseYDown;
			this.RightDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.MouseXRight;
			this.DownDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.MouseYUp;
			this.UpdateProperties();
		}

		public void BindToTouchpad()
		{
			this.LeftDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.GamepadTouchpadLeft;
			this.UpDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.GamepadTouchpadUp;
			this.RightDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.GamepadTouchpadRight;
			this.DownDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.GamepadTouchpadDown;
			this.UpdateProperties();
		}

		public void BindToLeftVirtualStick()
		{
			this.LeftDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.GamepadButtonLeftStickLeft;
			this.UpDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.GamepadButtonLeftStickUp;
			this.RightDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.GamepadButtonLeftStickRight;
			this.DownDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.GamepadButtonLeftStickDown;
			this.UpdateProperties();
		}

		public void BindToRightVirtualStick()
		{
			this.LeftDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.GamepadButtonRightStickLeft;
			this.UpDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.GamepadButtonRightStickUp;
			this.RightDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.GamepadButtonRightStickRight;
			this.DownDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.GamepadButtonRightStickDown;
			this.UpdateProperties();
		}

		public void BindToVirtualDPAD()
		{
			this.LeftDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.GamepadButtonDpadLeft;
			this.UpDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.GamepadButtonDpadUp;
			this.RightDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.GamepadButtonDpadRight;
			this.DownDirectionValue.SingleActivator.MappedKey = KeyScanCodeV2.GamepadButtonDpadDown;
			this.UpdateProperties();
		}

		public void BindToOverlayMenuDirections()
		{
			this.LeftDirectionValue.SingleActivator.MappedKey = BaseRewasdUserCommand.REWASD_USER_COMMAND_TABLE.First((BaseRewasdUserCommand x) => x.CommandId == 137);
			this.RightDirectionValue.SingleActivator.MappedKey = BaseRewasdUserCommand.REWASD_USER_COMMAND_TABLE.First((BaseRewasdUserCommand x) => x.CommandId == 136);
			this.UpDirectionValue.SingleActivator.MappedKey = BaseRewasdUserCommand.REWASD_USER_COMMAND_TABLE.First((BaseRewasdUserCommand x) => x.CommandId == 134);
			this.DownDirectionValue.SingleActivator.MappedKey = BaseRewasdUserCommand.REWASD_USER_COMMAND_TABLE.First((BaseRewasdUserCommand x) => x.CommandId == 135);
			this.UpdateProperties();
		}

		public virtual bool ResetToDefault(bool askConfirmation = true)
		{
			MessageBoxResult messageBoxResult = MessageBoxResult.Yes;
			if (askConfirmation)
			{
				string text = string.Format(DTLocalization.GetString(11569), this.GroupLabel);
				messageBoxResult = MessageBoxWithDoNotShowLogic.Show(Application.Current.MainWindow, text, MessageBoxButton.YesNo, MessageBoxImage.Question, "ConfirmResetSticksDefault", MessageBoxResult.Yes, false, 0.0, null, null, null, null, null, null);
			}
			if (messageBoxResult != MessageBoxResult.Yes)
			{
				return false;
			}
			this.ResetActivatorXBBindingDictionaries();
			this.SetDefaultValues(false);
			this.RevertRemapToDefault();
			return true;
		}

		protected virtual void ResetActivatorXBBindingDictionaries()
		{
			if (this.LeftDirectionValue != null)
			{
				this.LeftDirectionValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.LeftDirection);
			}
			if (this.UpDirectionValue != null)
			{
				this.UpDirectionValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.UpDirection);
			}
			if (this.RightDirectionValue != null)
			{
				this.RightDirectionValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.RightDirection);
			}
			if (this.DownDirectionValue != null)
			{
				this.DownDirectionValue.ActivatorXBBindingDictionary = new ActivatorXBBindingDictionary(this.DownDirection);
			}
		}

		protected virtual void SetDefaultValues(bool silent = false)
		{
		}

		public virtual bool IsAdvancedDefault()
		{
			return true;
		}

		public virtual void UpdateProperties()
		{
			this.OnPropertyChanged("IsBoundToWASD");
			this.OnPropertyChanged("IsBoundToKeyboard");
			this.OnPropertyChanged("IsBoundToMouse");
			this.OnPropertyChanged("IsBoundToAnyInvertedMouse");
			this.OnPropertyChanged("IsBoundToFlickStick");
			this.OnPropertyChanged("IsBoundToArrows");
			this.OnPropertyChanged("IsBoundToVirtualLeftStick");
			this.OnPropertyChanged("IsBoundToVirtualRightStick");
			this.OnPropertyChanged("IsBoundToVirtualDPAD");
			this.OnPropertyChanged("IsBoundToDS4Touchpad");
			this.OnPropertyChanged("IsBoundToOverlayMenuDirections");
			this.OnPropertyChanged("IsUnmapped");
			this.OnPropertyChanged("GroupLabel");
			this.OnPropertyChanged("GroupMapToLabel");
		}

		public override void Dispose()
		{
			base.Dispose();
			this.HostCollection = null;
		}

		public virtual object GetValueByKey(object key)
		{
			if (key is GamepadButton)
			{
				GamepadButton gamepadButton = (GamepadButton)key;
				if (gamepadButton == this.LeftDirection)
				{
					return this.LeftDirectionValue;
				}
				if (gamepadButton == this.UpDirection)
				{
					return this.UpDirectionValue;
				}
				if (gamepadButton == this.RightDirection)
				{
					return this.RightDirectionValue;
				}
				if (gamepadButton == this.DownDirection)
				{
					return this.DownDirectionValue;
				}
			}
			return null;
		}

		private bool _cructhForFirePropertyChanged;
	}
}
