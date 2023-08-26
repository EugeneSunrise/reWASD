using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using reWASDCommon.Infrastructure.Enums;

namespace reWASDUI.Infrastructure.KeyBindings.MacroBinding
{
	public abstract class BaseDurationMultiGamepadButtonMacro : BaseDurationGamepadButtonMacro
	{
		public abstract ObservableCollection<GamepadButton> PossibleButtons { get; }

		public abstract Drawing DurationDrawing { get; }

		protected BaseDurationMultiGamepadButtonMacro(MacroSequence macroSequence, uint duration, GamepadButton gamepadButton, uint step = 10U, uint minDuration = 10U, uint maxDuration = 4294967295U)
			: base(macroSequence, duration, gamepadButton, step, minDuration, maxDuration)
		{
		}
	}
}
