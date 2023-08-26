using System;
using DTOverlay;
using reWASDCommon.Infrastructure.Enums;

namespace XBEliteWPF.Services.Interfaces
{
	public interface IOverlayManagerService
	{
		OverlayManager OverlayManager { get; }

		void ReceivedOverlayMessage(OverlayMessageType messageType, IntPtr lParam);

		void ExecuteOverlayMenuCommand(RewasdOverlayMenuServiceCommand rewasdOverlayMenuServiceCommand);
	}
}
