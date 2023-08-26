using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Infrastructure.KeyBindingsModel.MacroBinding;

namespace reWASDUI.Infrastructure.KeyBindings.MacroBinding
{
	public class MacroTouchZoom : BaseDurationMultiGamepadButtonMacro
	{
		public override MacroItemType MacroItemType
		{
			get
			{
				return 4;
			}
		}

		public override Drawing DurationDrawing
		{
			get
			{
				return Application.Current.TryFindResource("IcoZoom") as Drawing;
			}
		}

		public override ObservableCollection<GamepadButton> PossibleButtons { get; } = new ObservableCollection<GamepadButton> { 81, 82 };

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

		public MacroTouchZoom(MacroSequence macroSequence, uint duration = 250U, GamepadButton gamepadButton = 81)
			: base(macroSequence, duration, gamepadButton, 10U, 10U, 10000U)
		{
			this.GamepadIndex = 0;
		}

		public override BaseMacro Clone(MacroSequence hostMacroSequence)
		{
			return new MacroTouchZoom(hostMacroSequence, base.Duration, base.GamepadButton);
		}

		public override BaseMacro Clone(MacroSequence hostMacroSequence)
		{
			return new MacroTouchZoom(base.Duration, base.GamepadButton);
		}

		private byte _gamepadIndex;
	}
}
