using System;
using reWASDUI.Infrastructure.Controller;

namespace reWASDUI.Infrastructure
{
	[Serializable]
	public class Gyroscope
	{
		public ulong DeviceID { get; private set; }

		public uint DeviceType { get; private set; }

		public bool IsGyroscopeAutoCalibrationOn { get; set; }

		public BaseControllerVM CurrentGamepad
		{
			get
			{
				return this._currentGamepad;
			}
			set
			{
				if (this._currentGamepad == value)
				{
					return;
				}
				this._currentGamepad = value;
				if (this._currentGamepad != null)
				{
					this.DeviceID = this._currentGamepad.Ids[0];
					this.DeviceType = this._currentGamepad.FirstType;
				}
			}
		}

		private BaseControllerVM _currentGamepad;
	}
}
