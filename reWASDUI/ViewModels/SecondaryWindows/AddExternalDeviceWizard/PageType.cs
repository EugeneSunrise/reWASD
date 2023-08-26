using System;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	public enum PageType
	{
		AdaptersSettings,
		DeviceType,
		BluetoothSettings,
		GimxStage1,
		GimxStage2,
		GimxStage3,
		GimxStage4,
		GimxFailsStage,
		EspStage1,
		EspStage3,
		EspStage4,
		EspFailsStage,
		EspLatencySpeed,
		ToolDownloader,
		AddExternalClient,
		AddOtherBTClient,
		AddPS4ConsoleStep1,
		AddPS4ConsoleStep2,
		AddPS4ConsoleStepConfigRequired,
		AddPS4ConsoleStep3,
		AddPS4ConsoleStep4,
		AddPS4ConsoleErrorPage,
		AddNintendoSwitchConsoleStep1,
		AddNintendoSwitchConsoleStep2,
		AddNintendoSwitchConsoleStep2a,
		AddNintendoSwitchConsoleStep2b,
		AddNintendoSwitchConsoleStep3,
		AddNintendoSwitchConsoleStepConfigRequired,
		AddOtherEsp32BTClient,
		AddOtherEsp32BTClientFinish,
		SelectAuthGamepad,
		FinishAndWaitUSBClient,
		None
	}
}
