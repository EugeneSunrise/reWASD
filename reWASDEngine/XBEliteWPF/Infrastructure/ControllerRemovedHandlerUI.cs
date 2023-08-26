using System;
using XBEliteWPF.Infrastructure.Controller;

namespace XBEliteWPF.Infrastructure
{
	public delegate void ControllerRemovedHandlerUI(BaseControllerVM controller, string fullID, string nameForDisconnect);
}
