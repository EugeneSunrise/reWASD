using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.KeyBindingsModel.MacroBinding;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Infrastructure.KeyBindings.MacroBinding
{
	public class MacroGamepadStick : BaseMacroBinding
	{
		public byte GamepadIndex
		{
			get
			{
				return this._gamepadIndex;
			}
			set
			{
				this.SetProperty<byte>(ref this._gamepadIndex, value, "GamepadIndex");
			}
		}

		public Stick Stick
		{
			get
			{
				return this._stick;
			}
			set
			{
				this.SetProperty<Stick>(ref this._stick, value, "Stick");
			}
		}

		public Axis Axis
		{
			get
			{
				return this._axis;
			}
			set
			{
				this.SetProperty<Axis>(ref this._axis, value, "Axis");
			}
		}

		public short DeflectionPercentage
		{
			get
			{
				return this._deflectionPercentage;
			}
			set
			{
				this.SetProperty<short>(ref this._deflectionPercentage, value, "DeflectionPercentage");
			}
		}

		public bool Relative
		{
			get
			{
				return this._relative;
			}
			set
			{
				this.SetProperty<bool>(ref this._relative, value, "Relative");
			}
		}

		public override MacroItemType MacroItemType
		{
			get
			{
				return 2;
			}
		}

		public GamepadButtonDescription GamepadButtonDescription { get; set; }

		public new KeyScanCodeV2 KeyScanCode
		{
			get
			{
				return KeyScanCodeV2.FindKeyScanCodeByGamepadButton(this.GamepadButtonDescription.Button);
			}
		}

		public MacroGamepadStick(MacroSequence macroSequence, GamepadButtonDescription gamepadButtonDescription, MacroKeyType macroType)
			: base(macroSequence, macroType)
		{
			this.GamepadIndex = 0;
			this.GamepadButtonDescription = gamepadButtonDescription;
			GamepadButton button = this.GamepadButtonDescription.Button;
			if (GamepadButtonExtensions.IsLeftStick(button))
			{
				this.Stick = 0;
			}
			else if (GamepadButtonExtensions.IsRightStick(button))
			{
				this.Stick = 1;
			}
			if (GamepadButtonExtensions.IsStickXAxis(button))
			{
				this.Axis = 0;
			}
			else if (GamepadButtonExtensions.IsStickYAxis(button))
			{
				this.Axis = 1;
			}
			base.PropertyChanged += delegate([Nullable(2)] object sender, PropertyChangedEventArgs args)
			{
				if (args.PropertyName == "IsBeingDragged" && base.Twin != null)
				{
					base.Twin.IsHighlighted = base.IsBeingDragged;
				}
			};
		}

		public MacroGamepadStick(MacroSequence macroSequence, int gbEnumIndex, MacroKeyType macroType)
			: this(macroSequence, GamepadButtonDescription.GetGamepadButtonDescriptionByGamepadButtonEnumIndex(gbEnumIndex), macroType)
		{
		}

		public override bool IsBindingEqualTo(BaseMacroBinding bmb)
		{
			MacroGamepadBinding macroGamepadBinding = bmb as MacroGamepadBinding;
			return macroGamepadBinding != null && this.GamepadButtonDescription.Equals(macroGamepadBinding.GamepadButtonDescription);
		}

		public override BaseMacro Clone(MacroSequence hostMacroSequence)
		{
			return new MacroGamepadStick(hostMacroSequence, this.GamepadButtonDescription, base.MacroKeyType)
			{
				DeflectionPercentage = this.DeflectionPercentage,
				Relative = this.Relative,
				GamepadIndex = this.GamepadIndex
			};
		}

		public override BaseMacro Clone(MacroSequence hostMacroSequence)
		{
			return new MacroGamepadStick(this.GamepadButtonDescription, base.MacroKeyType)
			{
				DeflectionPercentage = this.DeflectionPercentage,
				Relative = this.Relative,
				GamepadIndex = this.GamepadIndex
			};
		}

		private byte _gamepadIndex;

		private Stick _stick;

		private Axis _axis;

		private short _deflectionPercentage;

		private bool _relative;
	}
}
