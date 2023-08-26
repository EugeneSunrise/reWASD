using System;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Infrastructure.KeyBindingsModel.MacroBinding;

namespace reWASDUI.Infrastructure.KeyBindings.MacroBinding
{
	public class MacroGamepadStickCompensation : BaseMacroBinding
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

		public int LeftStickX
		{
			get
			{
				return this._leftStickX;
			}
			set
			{
				this.SetProperty<int>(ref this._leftStickX, value, "LeftStickX");
			}
		}

		public int LeftStickY
		{
			get
			{
				return this._leftStickY;
			}
			set
			{
				this.SetProperty<int>(ref this._leftStickY, value, "LeftStickY");
			}
		}

		public int RightStickX
		{
			get
			{
				return this._rightStickX;
			}
			set
			{
				this.SetProperty<int>(ref this._rightStickX, value, "RightStickX");
			}
		}

		public int RightStickY
		{
			get
			{
				return this._rightStickY;
			}
			set
			{
				this.SetProperty<int>(ref this._rightStickY, value, "RightStickY");
			}
		}

		public MacroGamepadStickCompensation(MacroSequence macroSequence, MacroKeyType macroType)
			: base(macroSequence, macroType)
		{
			this.GamepadIndex = 0;
		}

		public override MacroItemType MacroItemType
		{
			get
			{
				return 2;
			}
		}

		public override bool IsBindingEqualTo(BaseMacroBinding bmb)
		{
			return false;
		}

		public override BaseMacro Clone(MacroSequence hostMacroSequence)
		{
			return new MacroGamepadStickCompensation(hostMacroSequence, base.MacroKeyType)
			{
				GamepadIndex = this.GamepadIndex,
				LeftStickX = this.LeftStickX,
				LeftStickY = this.LeftStickY,
				RightStickX = this.RightStickX,
				RightStickY = this.RightStickY
			};
		}

		public override BaseMacro Clone(MacroSequence hostMacroSequence)
		{
			return new MacroGamepadStickCompensation(base.MacroKeyType)
			{
				GamepadIndex = this.GamepadIndex,
				LeftStickX = this.LeftStickX,
				LeftStickY = this.LeftStickY,
				RightStickX = this.RightStickX,
				RightStickY = this.RightStickY
			};
		}

		private byte _gamepadIndex;

		private int _leftStickX;

		private int _leftStickY;

		private int _rightStickX;

		private int _rightStickY;
	}
}
