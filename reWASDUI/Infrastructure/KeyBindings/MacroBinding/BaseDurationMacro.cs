using System;
using Prism.Commands;

namespace reWASDUI.Infrastructure.KeyBindings.MacroBinding
{
	public abstract class BaseDurationMacro : BaseMacro
	{
		public uint Duration
		{
			get
			{
				return this._duration;
			}
			set
			{
				this.SetProperty<uint>(ref this._duration, value, "Duration");
			}
		}

		public uint MinDuration
		{
			get
			{
				return this._minDuration;
			}
			set
			{
				this.SetProperty<uint>(ref this._minDuration, value, "MinDuration");
			}
		}

		public uint MaxDuration
		{
			get
			{
				return this._maxDuration;
			}
			set
			{
				this.SetProperty<uint>(ref this._maxDuration, value, "MaxDuration");
			}
		}

		public uint Step
		{
			get
			{
				return this._step;
			}
			set
			{
				this.SetProperty<uint>(ref this._step, value, "Step");
			}
		}

		public DelegateCommand IncreaseDurationCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._increaseDuration) == null)
				{
					delegateCommand = (this._increaseDuration = new DelegateCommand(new Action(this.IncreaseDuration)));
				}
				return delegateCommand;
			}
		}

		private void IncreaseDuration()
		{
			if (this.Duration + this.Step > this.MaxDuration)
			{
				return;
			}
			this.Duration += this.Step;
		}

		public DelegateCommand DecreaseDurationCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._decreaseDuration) == null)
				{
					delegateCommand = (this._decreaseDuration = new DelegateCommand(new Action(this.DecreaseDuration)));
				}
				return delegateCommand;
			}
		}

		private void DecreaseDuration()
		{
			if (this.Duration <= this.MinDuration)
			{
				this.Remove();
				return;
			}
			this.Duration -= this.Step;
		}

		public BaseDurationMacro(MacroSequence macroSequence, uint duration, uint step = 10U, uint minDuration = 10U, uint maxDuration = 4294967295U)
			: base(macroSequence)
		{
			this._duration = duration;
			this._minDuration = minDuration;
			this._maxDuration = maxDuration;
			this._step = step;
		}

		private uint _duration;

		private uint _minDuration;

		private uint _maxDuration;

		private uint _step;

		private DelegateCommand _increaseDuration;

		private DelegateCommand _decreaseDuration;
	}
}
