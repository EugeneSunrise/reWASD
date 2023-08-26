using System;
using reWASDCommon.Infrastructure.Enums;

namespace reWASDUI.Infrastructure.KeyBindings.MacroBinding
{
	public abstract class BaseDurationGamepadButtonMacro : BaseDurationMacro
	{
		public GamepadButton GamepadButton
		{
			get
			{
				return this._gamepadButton;
			}
			set
			{
				this.SetProperty<GamepadButton>(ref this._gamepadButton, value, "GamepadButton");
			}
		}

		protected BaseDurationGamepadButtonMacro(MacroSequence macroSequence, uint duration, GamepadButton gamepadButton, uint step = 10U, uint minDuration = 10U, uint maxDuration = 4294967295U)
			: base(macroSequence, duration, step, minDuration, maxDuration)
		{
			this.GamepadButton = gamepadButton;
		}

		private GamepadButton _gamepadButton;
	}
}
