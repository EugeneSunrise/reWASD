using System;
using DiscSoft.NET.Common.ColorStuff;

namespace XBEliteWPF.Infrastructure
{
	public delegate void LEDStateChangedHandler(ulong id, zColor color, ref bool isOnline);
}
