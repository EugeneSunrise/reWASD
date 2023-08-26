using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Utils;
using Prism.Commands;
using reWASDCommon.Infrastructure;
using reWASDUI.ViewModels.Preferences.Base;

namespace reWASDUI.ViewModels.Preferences
{
	public class ActivatorsPageVM : PreferencesBaseVM
	{
		public ushort SingleWaitTimeInMs
		{
			get
			{
				return this._singleWaitTimeInMs;
			}
			set
			{
				this.SetProperty<ushort>(ref this._singleWaitTimeInMs, value, "SingleWaitTimeInMs");
			}
		}

		public ushort DoubleWaitTimeInMs
		{
			get
			{
				return this._doubleWaitTimeInMs;
			}
			set
			{
				this.SetProperty<ushort>(ref this._doubleWaitTimeInMs, value, "DoubleWaitTimeInMs");
			}
		}

		public ushort LongWaitTimeInMs
		{
			get
			{
				return this._longWaitTimeInMs;
			}
			set
			{
				this.SetProperty<ushort>(ref this._longWaitTimeInMs, value, "LongWaitTimeInMs");
			}
		}

		public ushort ShortcutPressTimeInMs
		{
			get
			{
				return this._shortcutPressTimeInMs;
			}
			set
			{
				this.SetProperty<ushort>(ref this._shortcutPressTimeInMs, value, "ShortcutPressTimeInMs");
			}
		}

		public ushort SlotHotkeyPressTimeInMs
		{
			get
			{
				return this._slotHotkeyPressTimeInMs;
			}
			set
			{
				this.SetProperty<ushort>(ref this._slotHotkeyPressTimeInMs, value, "SlotHotkeyPressTimeInMs");
			}
		}

		public DelegateCommand ResetToDefaultsCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._resetToDefaults) == null)
				{
					delegateCommand = (this._resetToDefaults = new DelegateCommand(new Action(this.ResetToDefaults), new Func<bool>(this.ResetToDefaultsCanExecute)));
				}
				return delegateCommand;
			}
		}

		private void ResetToDefaults()
		{
			this.SingleWaitTimeInMs = 150;
			this.DoubleWaitTimeInMs = 300;
			this.LongWaitTimeInMs = 300;
			this.ShortcutPressTimeInMs = 50;
			this.SlotHotkeyPressTimeInMs = 500;
		}
		private bool ResetToDefaultsCanExecute()
		{
			return this.SingleWaitTimeInMs != 150 || this.DoubleWaitTimeInMs != 300 || this.LongWaitTimeInMs != 300 || this.ShortcutPressTimeInMs != 50 || this.SlotHotkeyPressTimeInMs != 500;
		}

		public ActivatorsPageVM()
		{
			this.Initialize();
			this.ResetToDefaultsCommand.ObservesProperty<ushort>(Expression.Lambda<Func<ushort>>(Expression.Property(Expression.Constant(this, typeof(ActivatorsPageVM)), methodof(ActivatorsPageVM.get_SingleWaitTimeInMs())), Array.Empty<ParameterExpression>()));
			this.ResetToDefaultsCommand.ObservesProperty<ushort>(Expression.Lambda<Func<ushort>>(Expression.Property(Expression.Constant(this, typeof(ActivatorsPageVM)), methodof(ActivatorsPageVM.get_DoubleWaitTimeInMs())), Array.Empty<ParameterExpression>()));
			this.ResetToDefaultsCommand.ObservesProperty<ushort>(Expression.Lambda<Func<ushort>>(Expression.Property(Expression.Constant(this, typeof(ActivatorsPageVM)), methodof(ActivatorsPageVM.get_LongWaitTimeInMs())), Array.Empty<ParameterExpression>()));
			this.ResetToDefaultsCommand.ObservesProperty<ushort>(Expression.Lambda<Func<ushort>>(Expression.Property(Expression.Constant(this, typeof(ActivatorsPageVM)), methodof(ActivatorsPageVM.get_ShortcutPressTimeInMs())), Array.Empty<ParameterExpression>()));
			this.ResetToDefaultsCommand.ObservesProperty<ushort>(Expression.Lambda<Func<ushort>>(Expression.Property(Expression.Constant(this, typeof(ActivatorsPageVM)), methodof(ActivatorsPageVM.get_SlotHotkeyPressTimeInMs())), Array.Empty<ParameterExpression>()));
		}

		public override Task Initialize()
		{
			this.SingleWaitTimeInMs = (ushort)RegistryHelper.GetValue("Config\\Activators", "ActivatorsSingle", 150, false);
			this.DoubleWaitTimeInMs = (ushort)RegistryHelper.GetValue("Config\\Activators", "ActivatorsDouble", 300, false);
			this.LongWaitTimeInMs = (ushort)RegistryHelper.GetValue("Config\\Activators", "ActivatorsLong", 300, false);
			this.ShortcutPressTimeInMs = (ushort)RegistryHelper.GetValue("Config\\Activators", "ShortcutPressTime", 50, false);
			this.SlotHotkeyPressTimeInMs = (ushort)RegistryHelper.GetValue("Config\\Activators", "SlotHotkeyPressTime", 500, false);
			base.ChangedProperties.Clear();
			return Task.CompletedTask;
		}

		public override Task<bool> ApplyChanges()
		{
			if (!base.IsChanged)
			{
				return Task.FromResult<bool>(true);
			}
			RegistryHelper.SetValue("Config\\Activators", "ActivatorsSingle", Convert.ToInt32(this.SingleWaitTimeInMs));
			RegistryHelper.SetValue("Config\\Activators", "ActivatorsDouble", Convert.ToInt32(this.DoubleWaitTimeInMs));
			RegistryHelper.SetValue("Config\\Activators", "ActivatorsLong", Convert.ToInt32(this.LongWaitTimeInMs));
			RegistryHelper.SetValue("Config\\Activators", "ShortcutPressTime", Convert.ToInt32(this.ShortcutPressTimeInMs));
			RegistryHelper.SetValue("Config\\Activators", "SlotHotkeyPressTime", Convert.ToInt32(this.SlotHotkeyPressTimeInMs));
			Constants.SingleWaitTimeInMs = this.SingleWaitTimeInMs;
			Constants.DoubleWaitTimeInMs = this.DoubleWaitTimeInMs;
			Constants.LongWaitTimeInMs = this.LongWaitTimeInMs;
			Constants.ShortcutPressTimeInMs = this.ShortcutPressTimeInMs;
			Constants.SlotHotkeyPressTimeInMs = this.SlotHotkeyPressTimeInMs;
			base.FireRequiredEnableRemap();
			base.ChangedProperties.Clear();
			return Task.FromResult<bool>(true);
		}

		private ushort _singleWaitTimeInMs;

		private ushort _doubleWaitTimeInMs;

		private ushort _longWaitTimeInMs;

		private ushort _shortcutPressTimeInMs;

		private ushort _slotHotkeyPressTimeInMs;

		private DelegateCommand _resetToDefaults;
	}
}
