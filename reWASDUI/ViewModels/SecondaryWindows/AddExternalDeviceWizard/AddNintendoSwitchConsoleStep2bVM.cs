using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using XBEliteWPF.Infrastructure.ExternalDevices;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class AddNintendoSwitchConsoleStep2bVM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.AddNintendoSwitchConsoleStep2b;
			}
		}

		public AddNintendoSwitchConsoleStep2bVM(WizardVM wizard)
			: base(wizard)
		{
		}

		public override async void OnShowPage()
		{
			base.IsProcessing = true;
			ExternalDevice currentExternalDevice = (this._wizard.FindPage(PageType.AdaptersSettings) as AdaptersSettingsVM).CurrentExternalDevice;
			ExternalClient externalClient = (this._wizard.FindPage(PageType.AddNintendoSwitchConsoleStep2) as AddNintendoSwitchConsoleStep2VM).ExternalClient;
			await App.GamepadService.ExternalDeviceRelationsHelper.AddAndSaveRelation(currentExternalDevice, externalClient, null);
			await Task.Delay(500);
			TaskAwaiter<bool> taskAwaiter = App.GameProfilesService.ApplyCurrentProfile(false).GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<bool> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			if (!taskAwaiter.GetResult())
			{
				base.IsProcessing = false;
				base.GoPage(PageType.AddNintendoSwitchConsoleStepConfigRequired);
			}
			else
			{
				if (this._wizard.Result.DeviceType == null || this._wizard.Result.DeviceType == 2)
				{
					for (int i = 0; i < 3; i++)
					{
						await Task.Delay(4000);
						if ((App.GamepadService.ExternalDeviceRelationsHelper.CurrentExternalState == 6 || App.GamepadService.ExternalDeviceRelationsHelper.CurrentExternalState == 3) && App.GamepadService.CurrentGamepad != null)
						{
							await App.HttpClientService.ExternalDevices.ExternalDeviceBluetoothReconnect(App.GamepadService.CurrentGamepad.ID, App.GamepadService.CurrentGamepad.CurrentSlot);
						}
						if (App.GamepadService.ExternalDeviceRelationsHelper.CurrentExternalState == 9)
						{
							break;
						}
					}
				}
				base.IsProcessing = false;
				base.GoPage(PageType.AddNintendoSwitchConsoleStep3);
			}
		}
	}
}
