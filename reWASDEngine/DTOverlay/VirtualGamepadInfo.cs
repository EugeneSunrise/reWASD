using System;
using reWASDCommon.Infrastructure.Enums;
using XBEliteWPF.Infrastructure;

namespace DTOverlay
{
	public class VirtualGamepadInfo
	{
		public HashInfo oldHashGamepad;

		public long oldHashMouse;

		public GamepadState gamepadState;

		public MouseState mouseState;

		public ControllerTypeEnum controllerType;

		public uint Type;

		public bool isPresent;

		public ulong id;
	}
}
