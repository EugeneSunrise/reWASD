using System;
using System.Windows;
using System.Windows.Media;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.DataModels;
using reWASDUI.Infrastructure.KeyBindings;

namespace reWASDUI.ViewModels.SecondaryWindows.AddExternalDeviceWizard
{
	internal class AddOtherEsp32BTClientFinishVM : BasePageVM
	{
		public override PageType PageType
		{
			get
			{
				return PageType.AddOtherEsp32BTClientFinish;
			}
		}

		public AddOtherEsp32BTClientFinishVM(WizardVM wizard)
			: base(wizard)
		{
		}

		public Drawing ControllerImage
		{
			get
			{
				GameVM currentGame = App.GameProfilesService.CurrentGame;
				VirtualGamepadType? virtualGamepadType;
				if (currentGame == null)
				{
					virtualGamepadType = null;
				}
				else
				{
					ConfigVM currentConfig = currentGame.CurrentConfig;
					if (currentConfig == null)
					{
						virtualGamepadType = null;
					}
					else
					{
						ConfigData configData = currentConfig.ConfigData;
						virtualGamepadType = ((configData != null) ? new VirtualGamepadType?(configData.VirtualGamepadType) : null);
					}
				}
				VirtualGamepadType? virtualGamepadType2 = virtualGamepadType;
				string text;
				if (virtualGamepadType2 != null)
				{
					VirtualGamepadType valueOrDefault = virtualGamepadType2.GetValueOrDefault();
					if (valueOrDefault <= 1)
					{
						text = "BTXboxWirelessControllerPaired";
						goto IL_7B;
					}
					if (valueOrDefault == 4)
					{
						text = "BTProControllerPaired";
						goto IL_7B;
					}
				}
				text = "BTWirelessControllerPaired";
				IL_7B:
				Application application = Application.Current;
				object obj;
				if (application == null)
				{
					obj = null;
				}
				else
				{
					Window mainWindow = application.MainWindow;
					obj = ((mainWindow != null) ? mainWindow.TryFindResource(text) : null);
				}
				return obj as Drawing;
			}
		}

		protected override void NavigateToNextPage()
		{
			this._wizard.OnOk();
		}
	}
}
