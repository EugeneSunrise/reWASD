using System;
using System.Runtime.CompilerServices;
using DiscSoft.NET.Common.Utils;

namespace reWASDUI.ViewModels.SecondaryWindows.CalibrateGyroscope
{
	public class CalibrateGyroAutoIsOnVM : BaseGyroPageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.CalibrateGyroAutoIsOn;
			}
		}

		private ulong _gamepadID
		{
			get
			{
				return this._wizard.Gyro.DeviceID;
			}
		}

		private uint _gamepadType
		{
			get
			{
				return this._wizard.Gyro.DeviceType;
			}
		}

		public CalibrateGyroAutoIsOnVM(GyroWizardVM wizard)
			: base(wizard)
		{
			this.IsGyroscopeAutoCalibrationOn = this.ReadAutoCalibrateProperty();
		}

		public override void OnShowPage()
		{
		}

		protected override void NavigateToNextPage()
		{
			base.GoPage(PageType.CalibrateGyroStart);
		}

		public bool IsGyroscopeAutoCalibrationOn
		{
			get
			{
				return this._isGyroscopeAutoCalibrationOn;
			}
			set
			{
				if (this._isGyroscopeAutoCalibrationOn == value)
				{
					return;
				}
				this._isGyroscopeAutoCalibrationOn = value;
				this.WriteAutoCalibrateProperty();
				this.OnPropertyChanged("IsGyroscopeAutoCalibrationOn");
			}
		}

		private bool ReadAutoCalibrateProperty()
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
			defaultInterpolatedStringHandler.AppendFormatted("Calibration");
			defaultInterpolatedStringHandler.AppendLiteral("\\");
			defaultInterpolatedStringHandler.AppendFormatted<ulong>(this._gamepadID);
			return RegistryHelper.GetBool(defaultInterpolatedStringHandler.ToStringAndClear(), "IsAutoCalibration", true);
		}

		private void WriteAutoCalibrateProperty()
		{
			string text = (this.IsGyroscopeAutoCalibrationOn ? "\"Auto\"" : "\"Manual\"");
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(69, 3);
			defaultInterpolatedStringHandler.AppendLiteral("Gyroscope calibration mode: set in registry to ");
			defaultInterpolatedStringHandler.AppendFormatted(text);
			defaultInterpolatedStringHandler.AppendLiteral(" for ControllerId ");
			defaultInterpolatedStringHandler.AppendFormatted<ulong>(this._gamepadID);
			defaultInterpolatedStringHandler.AppendLiteral(" /0x");
			defaultInterpolatedStringHandler.AppendFormatted<ulong>(this._gamepadID, "X");
			Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
			defaultInterpolatedStringHandler.AppendFormatted("Calibration");
			defaultInterpolatedStringHandler.AppendLiteral("\\");
			defaultInterpolatedStringHandler.AppendFormatted<ulong>(this._gamepadID);
			RegistryHelper.SetValue(defaultInterpolatedStringHandler.ToStringAndClear(), "IsAutoCalibration", this.IsGyroscopeAutoCalibrationOn);
			App.HttpClientService.Gamepad.ControllerUpdateGyroCalibrationMode(this._gamepadID, this._gamepadType);
		}

		private bool _isGyroscopeAutoCalibrationOn;
	}
}
