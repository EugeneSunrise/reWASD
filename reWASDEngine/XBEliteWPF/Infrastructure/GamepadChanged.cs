using System;
using DiscSoft.NET.Common.Utils.Clases;
using Prism.Events;

namespace XBEliteWPF.Infrastructure
{
	public class GamepadChanged : PubSubEvent<WindowMessageEvent>
	{
	}
}
