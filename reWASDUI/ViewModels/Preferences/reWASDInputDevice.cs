using System;

namespace reWASDUI.ViewModels.Preferences
{
	public class reWASDInputDevice
	{
		public string DeviceID { get; set; }

		public string DeviceName { get; set; }

		public bool IsVirtual { get; set; }

		public override string ToString()
		{
			return this.DeviceName;
		}
	}
}
