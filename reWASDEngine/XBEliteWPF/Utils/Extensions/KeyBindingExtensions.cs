using System;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Infrastructure.KeyBindingsModel.MacroBinding;

namespace XBEliteWPF.Utils.Extensions
{
	public static class KeyBindingExtensions
	{
		public static AssociatedControllerButton Convert(this AssociatedControllerButton acb)
		{
			return new AssociatedControllerButton
			{
				GamepadButtonDescription = acb.GamepadButtonDescription,
				KeyScanCode = acb.KeyScanCode,
				ControllerBindingFrameMode = acb.ControllerBindingFrameMode
			};
		}

		public static MacroKeyBinding Convert(this MacroKeyBinding mkb)
		{
			return new MacroKeyBinding(mkb.KeyScanCode, mkb.MacroKeyType);
		}
	}
}
