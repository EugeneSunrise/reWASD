using System;
using reWASDCommon.Infrastructure.Enums;

namespace reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups
{
	public class DPADDirectionalGroup : BaseDirectionalGroup
	{
		public DPADDirectionalGroup(BaseXBBindingCollection baseHostCollection)
			: base(baseHostCollection)
		{
		}

		public override string GroupLabel
		{
			get
			{
				return "DPAD";
			}
		}

		public override GamepadButton LeftDirection
		{
			get
			{
				return 35;
			}
		}

		public override GamepadButton UpDirection
		{
			get
			{
				return 33;
			}
		}

		public override GamepadButton RightDirection
		{
			get
			{
				return 36;
			}
		}

		public override GamepadButton DownDirection
		{
			get
			{
				return 34;
			}
		}

		public void CopyTo(DPADDirectionalGroup model)
		{
			model.LeftDirectionValue = base.LeftDirectionValue;
			model.UpDirectionValue = base.UpDirectionValue;
			model.RightDirectionValue = base.RightDirectionValue;
			model.DownDirectionValue = base.DownDirectionValue;
		}

		public void CopyFrom(DPADDirectionalGroup model)
		{
			base.LeftDirectionValue = model.LeftDirectionValue;
			base.UpDirectionValue = model.UpDirectionValue;
			base.RightDirectionValue = model.RightDirectionValue;
			base.DownDirectionValue = model.DownDirectionValue;
		}
	}
}
