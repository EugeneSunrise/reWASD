using System;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.KeyBindingsModel.MacroBinding;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;

namespace reWASDUI.Infrastructure.KeyBindings.MacroBinding
{
	public class MacroCrutchHoldUntillRelease : BaseMacro
	{
		public GamepadButtonDescription GamepadButtonDescription
		{
			get
			{
				return this._gamepadButtonDescription;
			}
			set
			{
				this.SetProperty<GamepadButtonDescription>(ref this._gamepadButtonDescription, value, "GamepadButtonDescription");
			}
		}

		public KeyScanCodeV2 KeyScanCode
		{
			get
			{
				return this._keyScanCode;
			}
			set
			{
				this.SetProperty<KeyScanCodeV2>(ref this._keyScanCode, value, "KeyScanCode");
			}
		}

		public MacroCrutchHoldUntillRelease(MacroSequence macroSequence, GamepadButtonDescription gbd, KeyScanCodeV2 ksc)
			: base(macroSequence)
		{
			this.GamepadButtonDescription = gbd;
			this.KeyScanCode = ksc;
		}

		public override MacroItemType MacroItemType
		{
			get
			{
				return 8;
			}
		}

		public override BaseMacro Clone(MacroSequence hostMacroSequence)
		{
			return new MacroCrutchHoldUntillRelease(hostMacroSequence, this.GamepadButtonDescription, this.KeyScanCode);
		}

		public override BaseMacro Clone(MacroSequence hostMacroSequence)
		{
			return new MacroCrutchHoldUntillRelease(this.GamepadButtonDescription, this.KeyScanCode);
		}

		private GamepadButtonDescription _gamepadButtonDescription;

		private KeyScanCodeV2 _keyScanCode;
	}
}
