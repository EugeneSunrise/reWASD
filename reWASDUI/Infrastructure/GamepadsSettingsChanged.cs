using System;
using DiscSoft.NET.Common.Utils.Clases;
using Prism.Events;

namespace reWASDUI.Infrastructure
{
	public class GamepadsSettingsChanged : PubSubEvent<WindowMessageEvent>
	{
	}
}
