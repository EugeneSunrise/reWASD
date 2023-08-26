using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using Prism.Commands;
using Prism.Events;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;

namespace reWASDUI.ViewModels.SecondaryWindows.CalibrateGyroscope
{
	public class CalibrateGyroProcessingVM : BaseGyroPageVM
	{
		public bool IsCalibrated
		{
			get
			{
				return this._isCalibrated;
			}
			private set
			{
				if (this._isCalibrated == value)
				{
					return;
				}
				this._isCalibrated = value;
				this.OnPropertyChanged("IsCalibrated");
			}
		}

		public bool IsFinished
		{
			get
			{
				return this._isFinished;
			}
			private set
			{
				if (this._isFinished == value)
				{
					return;
				}
				this._isFinished = value;
				this.OnPropertyChanged("IsFinished");
			}
		}

		public override PageType PageType
		{
			get
			{
				return PageType.CalibrateGyroProcessing;
			}
		}

		public CalibrateGyroProcessingVM(GyroWizardVM wizard)
			: base(wizard)
		{
			this._currentGamepad = wizard.Gyro.CurrentGamepad;
			this._gamepadID = wizard.Gyro.DeviceID;
			this._gamepadType = wizard.Gyro.DeviceType;
		}

		public override async void OnShowPage()
		{
			this.IsCalibrated = false;
			this.IsFinished = false;
			await Task.Delay(100);
			this.RunManualCalibration();
		}

		public override void OnHidePage()
		{
		}

		private void ProcessGyroCalibrationFinished(string controllerId)
		{
			this.IsCalibrated = true;
			this.IsFinished = true;
		}

		public async void RunManualCalibration()
		{
			ControllerVM controllerVM = this._currentGamepad as ControllerVM;
			if (controllerVM != null && controllerVM.IsAnySteam)
			{
				await App.HttpClientService.Gamepad.ControllerStartManualHardwareGyroReCalibration(this._gamepadID, this._gamepadType);
				await Task.Delay(3000);
				this.IsCalibrated = true;
			}
			else
			{
				await App.HttpClientService.Gamepad.ControllerStartManualSoftwareGyroReCalibration(this._gamepadID, this._gamepadType);
				await Task.Delay(10000);
			}
			this.IsFinished = true;
		}

		protected override void OnCancel()
		{
			if (!this.IsFinished)
			{
				if (DTMessageBox.Show(DTLocalization.GetString(11985), string.Empty, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
				{
					this._wizard.OnCancel();
					return;
				}
			}
			else
			{
				this._wizard.OnCancel();
			}
		}

		public ICommand TryAgainCommand
		{
			get
			{
				if (this._tryAgainCommand == null)
				{
					this._tryAgainCommand = new DelegateCommand(new Action(this.OnTryAgain));
				}
				return this._tryAgainCommand;
			}
		}

		protected void OnTryAgain()
		{
			this._wizard.GoPage(PageType.CalibrateGyroAutoIsOn);
		}

		private bool _isCalibrated;

		private bool _isFinished;

		private ulong _gamepadID;

		private uint _gamepadType;

		private BaseControllerVM _currentGamepad;


		private DelegateCommand _tryAgainCommand;
	}
}
