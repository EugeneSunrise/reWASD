using System;

namespace DTOverlay
{
	public enum OverlayMessageType
	{
		AttachDll,
		DetachDll,
		ThreadInitialized,
		ThreadTerminating,
		NeedStopInject,
		DirectX11_Init,
		FullscreenOverlay_Show,
		ShowOverlayMessage0,
		ShowOverlayMessage1,
		ShowOverlayMessage2,
		ShowOverlayMessage3,
		HideOverlayMessage,
		ShowOverlayRemap0,
		ShowOverlayRemap1,
		ShowOverlayRemap2,
		ShowOverlayRemap3,
		HideOverlayRemap,
		ShowOverlayGamepad0,
		ShowOverlayGamepad1,
		ShowOverlayGamepad2,
		ShowOverlayGamepad3,
		HideOverlayGamepad,
		ShowOverlayMenu0,
		ShowOverlayMenu1,
		ShowOverlayMenu2,
		ShowOverlayMenu3,
		ShowOverlayMenu4,
		HideOverlayMenu
	}
}
