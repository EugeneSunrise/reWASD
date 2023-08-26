using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Infrastructure.KeyBindingsModel.MacroBinding;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;

namespace reWASDUI.Infrastructure.KeyBindings.MacroBinding
{
	public class MacroKeyBinding : BaseMacroBinding
	{
		public new KeyScanCodeV2 KeyScanCode { get; set; }

		public override MacroItemType MacroItemType
		{
			get
			{
				return 0;
			}
		}

		public MacroKeyBinding(MacroSequence macroSequence, KeyScanCodeV2 keyScanCode, MacroKeyType macroType)
			: base(macroSequence, macroType)
		{
			this.KeyScanCode = keyScanCode;
			base.PropertyChanged += delegate([Nullable(2)] object sender, PropertyChangedEventArgs args)
			{
				if (args.PropertyName == "IsBeingDragged" && base.Twin != null)
				{
					base.Twin.IsHighlighted = base.IsBeingDragged;
				}
			};
		}

		public MacroKeyBinding(MacroSequence macroSequence, byte keyScanCode, MacroKeyType macroType)
			: this(macroSequence, KeyScanCodeV2.SCAN_CODE_TABLE[(int)keyScanCode], macroType)
		{
		}

		public override bool IsBindingEqualTo(BaseMacroBinding bmb)
		{
			MacroKeyBinding macroKeyBinding = bmb as MacroKeyBinding;
			return macroKeyBinding != null && this.KeyScanCode.Equals(macroKeyBinding.KeyScanCode);
		}

		public override BaseMacro Clone(MacroSequence hostMacroSequence)
		{
			return new MacroKeyBinding(hostMacroSequence, this.KeyScanCode, base.MacroKeyType);
		}

		public override BaseMacro Clone(MacroSequence hostMacroSequence)
		{
			return new MacroKeyBinding(this.KeyScanCode, base.MacroKeyType);
		}
	}
}
