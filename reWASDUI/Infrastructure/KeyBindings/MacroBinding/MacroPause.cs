using System;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Infrastructure.KeyBindingsModel.MacroBinding;

namespace reWASDUI.Infrastructure.KeyBindings.MacroBinding
{
	public class MacroPause : BaseDurationMacro
	{
		public override MacroItemType MacroItemType
		{
			get
			{
				return 5;
			}
		}

		public MacroPause(MacroSequence macroSequence, uint duration = 500U)
			: base(macroSequence, duration, 10U, 10U, 2147483000U)
		{
		}

		public void RemoveSelfIfBelowThreshold()
		{
			if (base.Duration < 10U)
			{
				this.Remove();
			}
		}

		public override BaseMacro Clone(MacroSequence hostMacroSequence)
		{
			return new MacroPause(hostMacroSequence, base.Duration);
		}

		public override BaseMacro Clone(MacroSequence hostMacroSequence)
		{
			return new MacroPause(base.Duration);
		}
	}
}
