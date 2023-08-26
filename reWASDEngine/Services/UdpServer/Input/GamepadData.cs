using System;
using System.Collections.Generic;
using reWASDCommon.Infrastructure.Enums;

namespace reWASDEngine.Services.UdpServer.Input
{
	internal struct GamepadData
	{
		public List<GamepadButton> PressedButtons { readonly get; set; }

		public ushort LeftTrigger { readonly get; set; }

		public ushort RightTrigger { readonly get; set; }

		public short LeftStickX { readonly get; set; }

		public short LeftStickY { readonly get; set; }

		public short RightStickX { readonly get; set; }

		public short RightStickY { readonly get; set; }

		public ushort LeftFingerX { readonly get; set; }

		public ushort LeftFingerY { readonly get; set; }

		public byte LeftFingerEventId { readonly get; set; }

		public ushort RightFingerX { readonly get; set; }

		public ushort RightFingerY { readonly get; set; }

		public byte RightFingerEventId { readonly get; set; }
	}
}
