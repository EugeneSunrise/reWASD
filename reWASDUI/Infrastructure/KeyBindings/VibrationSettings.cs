using System;
using System.Windows;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.View.SecondaryWindows;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using reWASDCommon.Infrastructure.KeyBindingsModel;

namespace reWASDUI.Infrastructure.KeyBindings
{
	public class VibrationSettings : ZBindableBase
	{
		public event VibrationSettings.ChangedEvent OnChanged;

		public ushort Intensivity
		{
			get
			{
				return this._intensivity;
			}
			set
			{
				if (this.SetProperty<ushort>(ref this._intensivity, value, "Intensivity"))
				{
					VibrationSettings.ChangedEvent onChanged = this.OnChanged;
					if (onChanged == null)
					{
						return;
					}
					onChanged(this);
				}
			}
		}

		public bool TurnedOn
		{
			get
			{
				return this._turnedOn;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._turnedOn, value, "TurnedOn"))
				{
					VibrationSettings.ChangedEvent onChanged = this.OnChanged;
					if (onChanged == null)
					{
						return;
					}
					onChanged(this);
				}
			}
		}

		public bool InsteadOfMainLeft
		{
			get
			{
				return this._insteadOfMainLeft;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._insteadOfMainLeft, value, "InsteadOfMainLeft"))
				{
					VibrationSettings.ChangedEvent onChanged = this.OnChanged;
					if (onChanged == null)
					{
						return;
					}
					onChanged(this);
				}
			}
		}

		public bool InsteadOfMainRight
		{
			get
			{
				return this._insteadOfMainRight;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._insteadOfMainRight, value, "InsteadOfMainRight"))
				{
					VibrationSettings.ChangedEvent onChanged = this.OnChanged;
					if (onChanged == null)
					{
						return;
					}
					onChanged(this);
				}
			}
		}

		public bool InsteadOfTriggerLeft
		{
			get
			{
				return this._insteadOfTriggerLeft;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._insteadOfTriggerLeft, value, "InsteadOfTriggerLeft"))
				{
					VibrationSettings.ChangedEvent onChanged = this.OnChanged;
					if (onChanged != null)
					{
						onChanged(this);
					}
					this.OnPropertyChanged("InsteadOfTriggerLeftGui");
				}
			}
		}

		public bool InsteadOfTriggerLeftGui
		{
			get
			{
				return this._insteadOfTriggerLeft;
			}
			set
			{
				if (value && !this.CheckSwitchToXBoxOne())
				{
					return;
				}
				if (this.SetProperty<bool>(ref this._insteadOfTriggerLeft, value, "InsteadOfTriggerLeftGui"))
				{
					VibrationSettings.ChangedEvent onChanged = this.OnChanged;
					if (onChanged == null)
					{
						return;
					}
					onChanged(this);
				}
			}
		}

		public bool InsteadOfTriggerRight
		{
			get
			{
				return this._insteadOfTriggerRight;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._insteadOfTriggerRight, value, "InsteadOfTriggerRight"))
				{
					this.OnPropertyChanged("InsteadOfTriggerRightGui");
					VibrationSettings.ChangedEvent onChanged = this.OnChanged;
					if (onChanged == null)
					{
						return;
					}
					onChanged(this);
				}
			}
		}

		public bool InsteadOfTriggerRightGui
		{
			get
			{
				return this._insteadOfTriggerRight;
			}
			set
			{
				if (value && !this.CheckSwitchToXBoxOne())
				{
					return;
				}
				if (this.SetProperty<bool>(ref this._insteadOfTriggerRight, value, "InsteadOfTriggerRightGui"))
				{
					VibrationSettings.ChangedEvent onChanged = this.OnChanged;
					if (onChanged == null)
					{
						return;
					}
					onChanged(this);
				}
			}
		}

		public void ClearTriggers()
		{
			this.InsteadOfTriggerLeft = false;
			this.InsteadOfTriggerRight = false;
		}

		private bool CheckSwitchToXBoxOne()
		{
			if (App.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.VirtualGamepadType == 1)
			{
				return true;
			}
			if (MessageBoxWithDoNotShowLogic.Show(Application.Current.MainWindow, DTLocalization.GetString(12268), MessageBoxButton.OKCancel, MessageBoxImage.Exclamation, "ConfirmSwitchToXBOXOne", MessageBoxResult.OK, false, 0.0, null, null, null, null, null, null) == MessageBoxResult.OK)
			{
				App.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.VirtualGamepadType = 1;
				return true;
			}
			return false;
		}

		public bool IsDefault()
		{
			return this.Intensivity == 100 && this.TurnedOn && !this.InsteadOfMainLeft && !this.InsteadOfMainRight && !this.InsteadOfTriggerLeft && !this.InsteadOfTriggerRight;
		}

		public void ResetToDefaults()
		{
			this.TurnedOn = true;
			this.Intensivity = 100;
			this.InsteadOfMainLeft = false;
			this.InsteadOfMainRight = false;
			this.InsteadOfTriggerLeft = false;
			this.InsteadOfTriggerRight = false;
		}

		public void CopyToModel(VibrationSettings model)
		{
			model.Intensivity = this.Intensivity;
			model.InsteadOfMainLeft = this.InsteadOfMainLeft;
			model.InsteadOfMainRight = this.InsteadOfMainRight;
			model.InsteadOfTriggerLeft = this.InsteadOfTriggerLeft;
			model.InsteadOfTriggerRight = this.InsteadOfTriggerRight;
			model.TurnedOn = this.TurnedOn;
		}

		public void CopyFromModel(VibrationSettings model)
		{
			this.Intensivity = model.Intensivity;
			this.InsteadOfMainLeft = model.InsteadOfMainLeft;
			this.InsteadOfMainRight = model.InsteadOfMainRight;
			this.InsteadOfTriggerLeft = model.InsteadOfTriggerLeft;
			this.InsteadOfTriggerRight = model.InsteadOfTriggerRight;
			this.TurnedOn = model.TurnedOn;
		}

		private ushort _intensivity = 100;

		private bool _turnedOn = true;

		private bool _insteadOfMainLeft;

		private bool _insteadOfMainRight;

		private bool _insteadOfTriggerLeft;

		private bool _insteadOfTriggerRight;

		public delegate void ChangedEvent(VibrationSettings settings);
	}
}
