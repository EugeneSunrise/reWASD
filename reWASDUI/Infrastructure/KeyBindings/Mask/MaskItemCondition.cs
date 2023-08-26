using System;
using System.Linq;
using DiscSoft.NET.Common.Utils;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.Controller;
using XBEliteWPF.Infrastructure.KeyBindingsModel.Mask;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Infrastructure.KeyBindings.Mask
{
	public class MaskItemCondition : AssociatedControllerButtonContainer, IEquatable<MaskItemCondition>
	{
		public byte ControllerFamilyIndex
		{
			get
			{
				return this._controllerFamilyIndex;
			}
			set
			{
				this.SetProperty<byte>(ref this._controllerFamilyIndex, value, "ControllerFamilyIndex");
			}
		}

		public ControllerFamily ControllerFamily
		{
			get
			{
				return this._controllerFamily;
			}
			set
			{
				this.SetProperty<ControllerFamily>(ref this._controllerFamily, value, "ControllerFamily");
			}
		}

		public bool IsTiltMode
		{
			get
			{
				return this._isTiltMode;
			}
			set
			{
				this.SetProperty<bool>(ref this._isTiltMode, value, "IsTiltMode");
			}
		}

		public BaseControllerVM AssociatedController
		{
			get
			{
				if (App.GamepadService.CurrentGamepad == null)
				{
					return null;
				}
				if (App.GamepadService.CurrentGamepad.IsTreatAsSingleDevice)
				{
					return App.GamepadService.CurrentGamepad;
				}
				CompositeControllerVM compositeControllerVM = App.GamepadService.CurrentGamepad as CompositeControllerVM;
				if (compositeControllerVM == null)
				{
					return App.GamepadService.CurrentGamepad;
				}
				if (this.ControllerFamily == 4)
				{
					return compositeControllerVM.NonNullBaseControllers.FirstOrDefault<BaseControllerVM>();
				}
				return compositeControllerVM.NonNullBaseControllers.Where((BaseControllerVM c) => c.ControllerFamily == this.ControllerFamily).Skip((int)this.ControllerFamilyIndex).FirstOrDefault<BaseControllerVM>();
			}
			set
			{
				CompositeControllerVM compositeControllerVM = App.GamepadService.CurrentGamepad as CompositeControllerVM;
				if (compositeControllerVM != null)
				{
					this.ControllerFamily = value.ControllerFamily;
					this.ControllerFamilyIndex = (byte)Array.IndexOf<BaseControllerVM>(compositeControllerVM.NonNullBaseControllers.Where((BaseControllerVM c) => c.ControllerFamily == value.ControllerFamily).ToArray<BaseControllerVM>(), value);
				}
				else
				{
					BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
					this.ControllerFamily = ((currentGamepad != null) ? currentGamepad.ControllerFamily : 0);
					this.ControllerFamilyIndex = 0;
					this.OnPropertyChanged("AssociatedController");
				}
				this.OnPropertyChanged("AssociatedController");
			}
		}

		public bool IsNotSelected
		{
			get
			{
				return base.GamepadButton == 2001 && base.KeyScanCode == KeyScanCodeV2.NoMap;
			}
		}

		public MaskItemCondition()
		{
		}

		public MaskItemCondition(byte controllerFamilyIndex, GamepadButton gamepadButton)
		{
			this.ControllerFamily = 0;
			this.ControllerFamilyIndex = controllerFamilyIndex;
			base.GamepadButton = gamepadButton;
		}

		public MaskItemCondition(GamepadButton gamepadButton)
			: this(0, gamepadButton)
		{
		}

		public MaskItemCondition(byte controllerFamilyIndex, KeyScanCodeV2 keyScanCode, ControllerFamily controllerFamily)
		{
			this.ControllerFamily = controllerFamily;
			this.ControllerFamilyIndex = controllerFamilyIndex;
			base.KeyScanCode = keyScanCode;
		}

		public MaskItemCondition(KeyScanCodeV2 keyScanCode, ControllerFamily controllerFamily)
			: this(0, keyScanCode, controllerFamily)
		{
		}

		protected override void OnBeforeControllerButtonChanged(object s, PropertyChangedExtendedEventArgs e)
		{
			this.OnPropertyChanged(e.PropertyName);
		}

		protected override void OnControllerButtonChanged(object s, PropertyChangedExtendedEventArgs e)
		{
			base.OnControllerButtonChanged(s, e);
			this.OnPropertyChanged(e.PropertyName);
			if (base.ControllerButton.IsGamepad)
			{
				this.ControllerFamily = 0;
				return;
			}
			if (base.ControllerButton.IsKeyScanCode && this.AssociatedController != null && !this.AssociatedController.IsCompositeDevice && ControllerFamilyExtensions.IsKeyboardOrMouse(this.AssociatedController.TreatAsControllerFamily))
			{
				this.ControllerFamily = this.AssociatedController.TreatAsControllerFamily;
			}
		}

		public bool Equals(MaskItemCondition other)
		{
			return other != null && (this == other || (this.ControllerFamilyIndex == other.ControllerFamilyIndex && base.GamepadButton == other.GamepadButton && base.KeyScanCode == other.KeyScanCode));
		}

		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((MaskItemCondition)obj)));
		}

		public override int GetHashCode()
		{
			return (17 * 23 + this.ControllerFamilyIndex.GetHashCode()) * 23 + base.GamepadButton.GetHashCode();
		}

		public void Clone(MaskItemCondition source)
		{
			base.ControllerButton = source.ControllerButton;
			this.ControllerFamilyIndex = source.ControllerFamilyIndex;
			this.ControllerFamily = source.ControllerFamily;
			this.IsTiltMode = source.IsTiltMode;
		}

		public void CopyFromModel(MaskItemCondition model)
		{
			base.ControllerButton.CopyFromModel(model.ControllerButton);
			this.ControllerFamilyIndex = model.ControllerFamilyIndex;
			this.ControllerFamily = model.ControllerFamily;
			this.IsTiltMode = model.IsTiltMode;
		}

		public void CopyToModel(MaskItemCondition model)
		{
			base.ControllerButton.CopyToModel(model.ControllerButton);
			model.ControllerFamilyIndex = this.ControllerFamilyIndex;
			model.ControllerFamily = this.ControllerFamily;
			model.IsTiltMode = this.IsTiltMode;
		}

		private byte _controllerFamilyIndex;

		private ControllerFamily _controllerFamily;

		private bool _isTiltMode;
	}
}
