using System;
using DiscSoft.NET.Common.Utils;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.ViewModels.Base;

namespace reWASDUI.Infrastructure.KeyBindings
{
	public class AssociatedControllerButtonContainer : ZBindable
	{
		public GamepadButton GamepadButton
		{
			get
			{
				return this.ControllerButton.GamepadButton;
			}
			set
			{
				if (this.ControllerButton.GamepadButton != value)
				{
					this.ControllerButton.RaiseBeforeControllerButtonChanged(new PropertyChangedExtendedEventArgs("KeyScanCode", typeof(GamepadButton), this.ControllerButton.GamepadButton, value));
				}
				this.ControllerButton.GamepadButton = value;
			}
		}

		public GamepadButtonDescription GamepadButtonDescription
		{
			get
			{
				return this.ControllerButton.GamepadButtonDescription;
			}
			set
			{
				this.ControllerButton.GamepadButtonDescription = value;
			}
		}

		public KeyScanCodeV2 KeyScanCode
		{
			get
			{
				return this.ControllerButton.KeyScanCode;
			}
			set
			{
				this.ControllerButton.KeyScanCode = value;
			}
		}

		public AssociatedControllerButton ControllerButton
		{
			get
			{
				return this._controllerButton;
			}
			protected set
			{
				AssociatedControllerButton controllerButton = this._controllerButton;
				if (this.SetProperty<AssociatedControllerButton>(ref this._controllerButton, value, "ControllerButton"))
				{
					if (controllerButton != null)
					{
						controllerButton.ControllerButtonChanged -= this.OnBeforeControllerButtonChanged;
						controllerButton.BeforeControllerButtonChanged -= this.OnControllerButtonChanged;
					}
					if (value != null)
					{
						value.BeforeControllerButtonChanged += this.OnBeforeControllerButtonChanged;
						value.ControllerButtonChanged += this.OnControllerButtonChanged;
					}
				}
			}
		}

		public AssociatedControllerButtonContainer()
		{
			this.ControllerButton = new AssociatedControllerButton();
		}

		public void SetButtonsFromAnotherInstance(AssociatedControllerButtonContainer acbc)
		{
			this.SetButtonsFromAnotherInstance(acbc.ControllerButton);
		}

		public void SetButtonsFromAnotherInstance(AssociatedControllerButton acb)
		{
			if (acb.IsKeyScanCode)
			{
				this.KeyScanCode = acb.KeyScanCode;
				return;
			}
			this.GamepadButton = acb.GamepadButton;
		}

		protected virtual void OnBeforeControllerButtonChanged(object s, PropertyChangedExtendedEventArgs e)
		{
		}

		protected virtual void OnControllerButtonChanged(object s, PropertyChangedExtendedEventArgs e)
		{
			this.OnPropertyChanged("ControllerButton");
			this.OnPropertyChanged("GamepadButton");
			this.OnPropertyChanged("GamepadButtonDescription");
			this.OnPropertyChanged("KeyScanCode");
		}

		public override void Dispose()
		{
			base.Dispose();
			if (this.ControllerButton != null)
			{
				this.ControllerButton = null;
			}
		}

		private AssociatedControllerButton _controllerButton;
	}
}
