using System;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Infrastructure.KeyBindingsModel.MacroBinding;

namespace reWASDUI.Infrastructure.KeyBindings.MacroBinding
{
	public class MacroRumble : BaseDurationMacro
	{
		public override MacroItemType MacroItemType
		{
			get
			{
				return 6;
			}
		}

		public byte MotorLeft
		{
			get
			{
				return this._motorLeft;
			}
			set
			{
				this.SetProperty<byte>(ref this._motorLeft, value, "MotorLeft");
			}
		}

		public byte MotorRight
		{
			get
			{
				return this._motorRight;
			}
			set
			{
				this.SetProperty<byte>(ref this._motorRight, value, "MotorRight");
			}
		}

		public byte TriggerLeft
		{
			get
			{
				return this._triggerLeft;
			}
			set
			{
				this.SetProperty<byte>(ref this._triggerLeft, value, "TriggerLeft");
			}
		}

		public byte TriggerRight
		{
			get
			{
				return this._triggerRight;
			}
			set
			{
				this.SetProperty<byte>(ref this._triggerRight, value, "TriggerRight");
			}
		}

		public MacroRumble(MacroSequence macroSequence, uint duration = 150U, byte motorLeft = 80, byte motorRight = 80, byte triggerLeft = 60, byte triggerRight = 60)
			: base(macroSequence, duration, 10U, 100U, 2550U)
		{
			this._motorLeft = motorLeft;
			this._motorRight = motorRight;
			this._triggerLeft = triggerLeft;
			this._triggerRight = triggerRight;
		}

		public override BaseMacro Clone(MacroSequence hostMacroSequence)
		{
			return new MacroRumble(hostMacroSequence, base.Duration, this.MotorLeft, this.MotorRight, this.TriggerLeft, this.TriggerRight);
		}

		public override BaseMacro Clone(MacroSequence hostMacroSequence)
		{
			return new MacroRumble(base.Duration, this.MotorLeft, this.MotorRight, this.TriggerLeft, this.TriggerRight);
		}

		private byte _motorLeft;

		private byte _motorRight;

		private byte _triggerLeft;

		private byte _triggerRight;
	}
}
