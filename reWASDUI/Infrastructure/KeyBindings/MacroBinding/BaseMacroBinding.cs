using System;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;

namespace reWASDUI.Infrastructure.KeyBindings.MacroBinding
{
	public abstract class BaseMacroBinding : BaseMacro
	{
		public MacroGamepadBinding GamepadBinding
		{
			get
			{
				return this as MacroGamepadBinding;
			}
		}

		public MacroKeyBinding KeyBinding
		{
			get
			{
				return this as MacroKeyBinding;
			}
		}

		public KeyScanCodeV2 KeyScanCode
		{
			get
			{
				if (!base.IsGamepadBinding)
				{
					return this.KeyBinding.KeyScanCode;
				}
				return this.GamepadBinding.KeyScanCode;
			}
		}

		public MacroKeyType MacroKeyType
		{
			get
			{
				return this._macroKeyType;
			}
			set
			{
				this.SetProperty<MacroKeyType>(ref this._macroKeyType, value, "MacroKeyType");
			}
		}

		public BaseMacroBinding Twin { get; set; }

		public BaseMacroBinding(MacroSequence macroSequence, MacroKeyType macroType)
			: base(macroSequence)
		{
			this._macroKeyType = macroType;
		}

		public abstract bool IsBindingEqualTo(BaseMacroBinding bmb);

		public void SetTwin(BaseMacroBinding twin)
		{
			twin.Twin = this;
			this.Twin = twin;
		}

		public new string DisabledToolTip
		{
			get
			{
				if ((base.MacroSequence.IsParentBindingToggle || base.MacroSequence.IsParentBindingTurbo) && this.MacroKeyType == 1)
				{
					return "Turbo and Toggle set limits on Combo. To use full Combo power, go Back to uncheck them.";
				}
				return null;
			}
		}

		protected override bool AddBreakAfterCanExecute()
		{
			return base.AddBreakAfterCanExecute() && (this is MacroGamepadStick || this.MacroKeyType == 1) && !base.MacroSequence.IsParentBindingToggle && !base.MacroSequence.IsParentBindingTurbo;
		}

		protected override void Remove()
		{
			if (this.MacroKeyType == 1 && this.Twin != null)
			{
				this.Twin.Remove();
				return;
			}
			base.Remove();
		}

		protected MacroKeyType _macroKeyType;
	}
}
