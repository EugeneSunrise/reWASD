using System;
using DiscSoft.NET.Common.Utils.Clases;
using Prism.Events;

namespace XBEliteWPF.Infrastructure
{
	public class ServiceProfileStateChanged : PubSubEvent<WindowMessageEvent>
	{
	}
}
