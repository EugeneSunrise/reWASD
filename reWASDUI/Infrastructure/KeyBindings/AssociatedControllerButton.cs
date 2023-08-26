using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Media;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using Newtonsoft.Json;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Utils;
using reWASDUI.Infrastructure.Controller;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtilModel;

namespace reWASDUI.Infrastructure.KeyBindings
{
	[JsonObject(1)]
	[Serializable]
	public class AssociatedControllerButton : ZBindableBase, IEquatable<AssociatedControllerButton>, ISerializable
	{
		public bool IsSingleSimpleButtonEdit
		{
			get
			{
				return this.ControllerBindingFrameMode == null && !GamepadButtonExtensions.IsMouseDirection(this.GamepadButton);
			}
		}

		public AssociatedControllerButton Clone()
		{
			return new AssociatedControllerButton
			{
				_keyScanCode = this._keyScanCode,
				_gamepadButtonDescription = this._gamepadButtonDescription
			};
		}

		public void Merge(AssociatedControllerButton button)
		{
			this.KeyScanCode = button.KeyScanCode;
			this.GamepadButtonDescription = button.GamepadButtonDescription;
		}

		[JsonProperty("KeyScanCode")]
		public KeyScanCodeV2 KeyScanCode
		{
			get
			{
				return this._keyScanCode;
			}
			set
			{
				KeyScanCodeV2 keyScanCode = this._keyScanCode;
				if (this._gamepadButtonDescription != null && GamepadButtonExtensions.IsRealButton(this._gamepadButtonDescription.Button))
				{
					this.GamepadButton = 2001;
				}
				if (value != null && this._keyScanCode != value)
				{
					this.RaiseBeforeControllerButtonChanged(new PropertyChangedExtendedEventArgs("KeyScanCode", typeof(KeyScanCodeV2), this._keyScanCode, value));
					this.SetProperty<KeyScanCodeV2>(ref this._keyScanCode, value, "KeyScanCode");
					this.OnKeyScanCodeChanged(keyScanCode, this._keyScanCode);
				}
			}
		}

		[JsonProperty("GamepadButton")]
		public GamepadButton GamepadButton
		{
			get
			{
				return this.GamepadButtonDescription.Button;
			}
			set
			{
				this.GamepadButtonDescription = GamepadButtonDescription.FindGamepadButtonDescriptionByGamepadButton(value);
			}
		}

		public GamepadButtonDescription GamepadButtonDescription
		{
			get
			{
				return this._gamepadButtonDescription;
			}
			set
			{
				GamepadButtonDescription gamepadButtonDescription = this._gamepadButtonDescription;
				if (this.SetProperty<GamepadButtonDescription>(ref this._gamepadButtonDescription, value, "GamepadButtonDescription"))
				{
					this.OnPropertyChanged("GamepadButton");
					if (gamepadButtonDescription != null)
					{
						this.OnGamepadButtonChanged(gamepadButtonDescription.Button, this._gamepadButtonDescription.Button);
					}
				}
			}
		}

		public ControllerBindingFrameAdditionalModes? ControllerBindingFrameMode
		{
			get
			{
				return this._controllerBindingFrameMode;
			}
			set
			{
				this.SetProperty<ControllerBindingFrameAdditionalModes?>(ref this._controllerBindingFrameMode, value, "ControllerBindingFrameMode");
			}
		}

		public object ControllerBindingButtonSelectionObject
		{
			get
			{
				if (this.IsGamepad)
				{
					return this.GamepadButton;
				}
				if (this.IsKeyScanCode)
				{
					return this.KeyScanCode;
				}
				return 2001;
			}
			set
			{
				if (value is GamepadButton)
				{
					GamepadButton gamepadButton = (GamepadButton)value;
					this.GamepadButton = gamepadButton;
					if (gamepadButton == 2001)
					{
						this.KeyScanCode = KeyScanCodeV2.NoMap;
					}
				}
				else
				{
					GamepadButtonDescription gamepadButtonDescription = value as GamepadButtonDescription;
					if (gamepadButtonDescription != null)
					{
						this.GamepadButtonDescription = gamepadButtonDescription;
						if (gamepadButtonDescription == GamepadButtonDescription.NotSelected)
						{
							this.KeyScanCode = KeyScanCodeV2.NoMap;
						}
					}
					else
					{
						KeyScanCodeV2 keyScanCodeV = value as KeyScanCodeV2;
						if (keyScanCodeV != null)
						{
							this.KeyScanCode = keyScanCodeV;
						}
					}
				}
				this.OnPropertyChanged("ControllerBindingButtonSelectionObject");
			}
		}

		public bool IsKeyComboMappingAvailiable
		{
			get
			{
				if (this.ControllerBindingFrameMode != null)
				{
					return false;
				}
				if (this.IsGamepad)
				{
					return GamepadButtonExtensions.IsKeyComboMappingAvailiable(this.GamepadButton);
				}
				bool isKeyScanCode = this.IsKeyScanCode;
				return true;
			}
		}

		public bool IsAdvancedMappingAvailiable
		{
			get
			{
				if (this.IsGamepad)
				{
					return GamepadButtonExtensions.IsAdvancedMappingAvailiable(this.GamepadButton);
				}
				return this.IsKeyScanCode && !this.IsMouseButton;
			}
		}

		public bool IsGamepadRemappingAvailiable
		{
			get
			{
				return this.IsGamepad && GamepadButtonExtensions.IsGamepadMappingAvailiable(this.GamepadButton);
			}
		}

		public bool IsUnmapAvailiable
		{
			get
			{
				if (!this.IsGamepad)
				{
					return this.IsKeyScanCode && (this.KeyScanCode.IsMouseDigital || UtilsCommon.IsKeyboardUnmapValid(KeyScanCodeV2.GetIndex(this.KeyScanCode)));
				}
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
				if (controllerTypeEnum2 != null && ControllerTypeExtensions.IsAzeronCyro(controllerTypeEnum2.GetValueOrDefault()) && !GamepadButtonExtensions.IsMouseDirection(this.GamepadButton) && !GamepadButtonExtensions.IsMouseScroll(this.GamepadButton))
				{
					return false;
				}
				if (GamepadButtonExtensions.IsAnyPhysicalTrackPadDigitalDirection(this.GamepadButton) && (controllerTypeEnum2 != null && ControllerTypeExtensions.IsGamepadWithSonyTouchpad(controllerTypeEnum2.GetValueOrDefault())))
				{
					return false;
				}
				BaseControllerVM currentGamepad2 = App.GamepadService.CurrentGamepad;
				bool flag;
				if (currentGamepad2 == null)
				{
					flag = false;
				}
				else
				{
					ControllerVM currentController2 = currentGamepad2.CurrentController;
					bool? flag2 = ((currentController2 != null) ? new bool?(ControllerTypeExtensions.IsGameSirG7(currentController2.ControllerType)) : null);
					bool flag3 = true;
					flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
				if (flag && (this.GamepadButton == 12 || this.GamepadButton == 13 || this.GamepadButton == 14 || this.GamepadButton == 29 || this.GamepadButton == 30))
				{
					return false;
				}
				BaseControllerVM currentGamepad3 = App.GamepadService.CurrentGamepad;
				bool flag4;
				if (currentGamepad3 == null)
				{
					flag4 = false;
				}
				else
				{
					ControllerVM currentController3 = currentGamepad3.CurrentController;
					bool? flag2 = ((currentController3 != null) ? new bool?(ControllerTypeExtensions.IsSonyDualSenseEdge(currentController3.ControllerType)) : null);
					bool flag3 = true;
					flag4 = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
				return !flag4 || (this.GamepadButton != 14 && this.GamepadButton != 15 && this.GamepadButton != 16 && this.GamepadButton != 17);
			}
		}

		public bool IsGamepad
		{
			get
			{
				return this.GamepadButton != 2001 && this.GamepadButton != 2000 && this.GamepadButton > 0;
			}
		}

		public bool IsMouseButton
		{
			get
			{
				return GamepadButtonExtensions.IsMouseStick(this.GamepadButton) || GamepadButtonExtensions.IsMouseScroll(this.GamepadButton) || this.KeyScanCode.IsMouse;
			}
		}

		public bool IsMouseScroll
		{
			get
			{
				return GamepadButtonExtensions.IsMouseScroll(this.GamepadButton);
			}
		}

		public bool IsKeyScanCode
		{
			get
			{
				return this.KeyScanCode != null && this.KeyScanCode != KeyScanCodeV2.NoMap;
			}
		}

		public bool IsControllerWizard
		{
			get
			{
				ControllerBindingFrameAdditionalModes? controllerBindingFrameMode = this.ControllerBindingFrameMode;
				ControllerBindingFrameAdditionalModes controllerBindingFrameAdditionalModes = 0;
				return (controllerBindingFrameMode.GetValueOrDefault() == controllerBindingFrameAdditionalModes) & (controllerBindingFrameMode != null);
			}
		}

		public bool IsMouseDirectionsOrScrolls
		{
			get
			{
				ControllerBindingFrameAdditionalModes? controllerBindingFrameMode = this.ControllerBindingFrameMode;
				ControllerBindingFrameAdditionalModes controllerBindingFrameAdditionalModes = 1;
				return ((controllerBindingFrameMode.GetValueOrDefault() == controllerBindingFrameAdditionalModes) & (controllerBindingFrameMode != null)) || GamepadButtonExtensions.IsMouseStick(this.GamepadButton) || GamepadButtonExtensions.IsMouseScroll(this.GamepadButton);
			}
		}

		public bool IsSet
		{
			get
			{
				return this.IsGamepad || this.IsKeyScanCode;
			}
		}

		public bool IsVirtualGamepadMappingAllowed
		{
			get
			{
				return this.IsGamepad && !GamepadButtonExtensions.IsMouseDirection(this.GamepadButton) && !GamepadButtonExtensions.IsMouseScroll(this.GamepadButton) && !GamepadButtonExtensions.IsAnyPaddle(this.GamepadButton);
			}
		}

		public Drawing XBButtonImage
		{
			get
			{
				if (this.IsGamepad)
				{
					GamepadButton gamepadButton = this.GamepadButton;
					BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
					bool flag;
					if (currentGamepad == null)
					{
						flag = false;
					}
					else
					{
						ControllerVM currentController = currentGamepad.CurrentController;
						if (currentController == null)
						{
							flag = false;
						}
						else
						{
							ControllerTypeEnum controllerType = currentController.ControllerType;
							flag = true;
						}
					}
					return XBUtils.GetAnnotationDrawingForGamepadButton(gamepadButton, new ControllerTypeEnum?((flag && ControllerTypeExtensions.IsGamepad(App.GamepadService.CurrentGamepad.CurrentController.ControllerType)) ? App.GamepadService.CurrentGamepad.CurrentController.ControllerType : 3), false);
				}
				bool isKeyScanCode = this.IsKeyScanCode;
				return null;
			}
		}

		public event PropertyChangedExtendedEventHandler ControllerButtonChanged;

		public event PropertyChangedExtendedEventHandler BeforeControllerButtonChanged;

		public AssociatedControllerButton()
		{
			this.SetDefaultButtons(true);
		}

		public AssociatedControllerButton(GamepadButton gb)
			: this()
		{
			this.GamepadButton = gb;
		}

		public AssociatedControllerButton(KeyScanCodeV2 ksc)
			: this()
		{
			this.KeyScanCode = ksc;
		}

		public AssociatedControllerButton(ControllerBindingFrameAdditionalModes controllerBindingFrameMode)
		{
			this.SetDefaultButtons(true);
			this.ControllerBindingFrameMode = new ControllerBindingFrameAdditionalModes?(controllerBindingFrameMode);
		}

		protected AssociatedControllerButton(SerializationInfo info, StreamingContext context)
		{
			this.GamepadButton = 2001;
			SerializationInfoEnumerator enumerator = info.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SerializationEntry serializationEntry = enumerator.Current;
				string name = serializationEntry.Name;
				if (!(name == "KeyScanCode"))
				{
					if (!(name == "GamepadButton"))
					{
						if (name == "GamepadButtonDescription")
						{
							GamepadButtonDescription gamepadButtonDescription = (GamepadButtonDescription)info.GetValue(serializationEntry.Name, typeof(GamepadButtonDescription));
							this.GamepadButton = gamepadButtonDescription.Button;
						}
					}
					else
					{
						this.GamepadButton = (GamepadButton)info.GetValue(serializationEntry.Name, typeof(GamepadButton));
					}
				}
				else
				{
					KeyScanCodeV2 keyScanCode = (KeyScanCodeV2)info.GetValue(serializationEntry.Name, typeof(KeyScanCodeV2));
					if (keyScanCode.Description != "")
					{
						this.KeyScanCode = KeyScanCodeV2.SCAN_CODE_TABLE.FirstOrDefault((KeyScanCodeV2 ksc) => ksc.Description == keyScanCode.Description);
					}
				}
			}
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (this.IsKeyScanCode)
			{
				info.AddValue("KeyScanCode", this.KeyScanCode);
				return;
			}
			info.AddValue("GamepadButton", this.GamepadButton);
		}

		public bool IsAssociatedSetToEqualButtons(AssociatedControllerButton anotherButton)
		{
			if (anotherButton == null)
			{
				return false;
			}
			if (this.IsGamepad && anotherButton.IsGamepad)
			{
				return this.GamepadButton == anotherButton.GamepadButton;
			}
			if (this.IsKeyScanCode && anotherButton.IsKeyScanCode)
			{
				return this.KeyScanCode == anotherButton.KeyScanCode;
			}
			return this.IsKeyScanCode == anotherButton.IsKeyScanCode && this.IsGamepad == anotherButton.IsGamepad;
		}

		public void SetDefaultButtons(bool silent = true)
		{
			if (silent)
			{
				this._gamepadButtonDescription = GamepadButtonDescription.NotSelected;
				this._keyScanCode = KeyScanCodeV2.NoMap;
				return;
			}
			this.GamepadButtonDescription = GamepadButtonDescription.NotSelected;
			this.KeyScanCode = KeyScanCodeV2.NoMap;
		}

		public void SetRefButtons(ref GamepadButton? gamepadButton, ref KeyScanCodeV2 keyScanCode)
		{
			if (this.IsGamepad)
			{
				gamepadButton = new GamepadButton?(this.GamepadButton);
			}
			if (this.IsKeyScanCode)
			{
				keyScanCode = this.KeyScanCode;
			}
		}

		protected virtual void OnKeyScanCodeChanged(KeyScanCodeV2 prevValue, KeyScanCodeV2 newValue)
		{
			this._gamepadButtonDescription = GamepadButtonDescription.NotSelected;
			this.FireNotifyPropertyChangedForProperties();
			this.RaiseControllerButtonChanged(new PropertyChangedExtendedEventArgs("KeyScanCode", typeof(KeyScanCodeV2), prevValue, newValue));
		}

		protected virtual void OnGamepadButtonChanged(GamepadButton prevValue, GamepadButton newValue)
		{
			this._keyScanCode = KeyScanCodeV2.NoMap;
			this.FireNotifyPropertyChangedForProperties();
			this.RaiseControllerButtonChanged(new PropertyChangedExtendedEventArgs("GamepadButton", typeof(GamepadButton), prevValue, newValue));
		}

		public void RaiseBeforeControllerButtonChanged(PropertyChangedExtendedEventArgs e)
		{
			PropertyChangedExtendedEventHandler beforeControllerButtonChanged = this.BeforeControllerButtonChanged;
			if (beforeControllerButtonChanged == null)
			{
				return;
			}
			beforeControllerButtonChanged(this, e);
		}

		protected void RaiseControllerButtonChanged(PropertyChangedExtendedEventArgs e)
		{
			PropertyChangedExtendedEventHandler controllerButtonChanged = this.ControllerButtonChanged;
			if (controllerButtonChanged == null)
			{
				return;
			}
			controllerButtonChanged(this, e);
		}

		private void FireNotifyPropertyChangedForProperties()
		{
			this.OnPropertyChanged("GamepadButton");
			this.OnPropertyChanged("GamepadButtonDescription");
			this.OnPropertyChanged("KeyScanCode");
			this.OnPropertyChanged("IsGamepad");
			this.OnPropertyChanged("IsKeyScanCode");
			this.OnPropertyChanged("IsSet");
			this.OnPropertyChanged("XBButtonImage");
			this.OnPropertyChanged("IsKeyComboMappingAvailiable");
			this.OnPropertyChanged("IsAdvancedMappingAvailiable");
			this.OnPropertyChanged("IsUnmapAvailiable");
			this.OnPropertyChanged("IsVirtualGamepadMappingAllowed");
			this.OnPropertyChanged("IsControllerWizard");
			this.OnPropertyChanged("IsMouseDirectionsOrScrolls");
		}

		public long GetButtonId()
		{
			if (this.IsGamepad)
			{
				return this.GamepadButton;
			}
			if (this.IsKeyScanCode)
			{
				return (long)KeyScanCodeV2.GetIndex(this.KeyScanCode);
			}
			return 0L;
		}

		public bool IsShiftModifierable()
		{
			if (this.IsGamepad)
			{
				return GamepadButtonExtensions.IsShiftModifierable(this.GamepadButton);
			}
			return this.IsKeyScanCode && KeyScanCodeExtensions.IsShiftModifierable(this.KeyScanCode);
		}

		public bool Equals(AssociatedControllerButton other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (this.GamepadButton == other.GamepadButton && this.KeyScanCode == other.KeyScanCode)
			{
				ControllerBindingFrameAdditionalModes? controllerBindingFrameMode = this.ControllerBindingFrameMode;
				ControllerBindingFrameAdditionalModes? controllerBindingFrameMode2 = other.ControllerBindingFrameMode;
				return (controllerBindingFrameMode.GetValueOrDefault() == controllerBindingFrameMode2.GetValueOrDefault()) & (controllerBindingFrameMode != null == (controllerBindingFrameMode2 != null));
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((AssociatedControllerButton)obj)));
		}

		public override int GetHashCode()
		{
			int num = 17;
			if (this.KeyScanCode != null)
			{
				num = num * 23 + this.KeyScanCode.GetHashCode();
			}
			num = num * 23 + this.GamepadButton.GetHashCode();
			if (this.ControllerBindingFrameMode != null)
			{
				num = num * 23 + this.ControllerBindingFrameMode.GetHashCode();
			}
			return num;
		}

		public void CopyFromModel(AssociatedControllerButton model)
		{
			this._gamepadButtonDescription = model.GamepadButtonDescription;
			this._keyScanCode = model.KeyScanCode;
			this._controllerBindingFrameMode = model.ControllerBindingFrameMode;
		}

		public void CopyToModel(AssociatedControllerButton model)
		{
			if (this.IsGamepad)
			{
				model.GamepadButtonDescription = this.GamepadButtonDescription;
			}
			else
			{
				model.KeyScanCode = this.KeyScanCode;
			}
			model.ControllerBindingFrameMode = this.ControllerBindingFrameMode;
		}

		public static AssociatedControllerButton ControllerWizardAssociatedControllerButton = new AssociatedControllerButton(0);

		public static AssociatedControllerButton MouseDirectionAssociatedControllerButton = new AssociatedControllerButton(1);

		private GamepadButtonDescription _gamepadButtonDescription;

		private KeyScanCodeV2 _keyScanCode;

		private ControllerBindingFrameAdditionalModes? _controllerBindingFrameMode;
	}
}
