using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Principal;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using reWASDCommon.Infrastructure;
using reWASDCommon.Infrastructure.LED;
using reWASDUI.DataModels.CompositeDevicesCollection;
using reWASDUI.DataModels.GamepadActiveProfiles;
using reWASDUI.DataModels.GamepadSlotHotkeyCollection;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Services.Interfaces;
using XBEliteWPF.DataModels.PeripheralDevicesCollection;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.ExternalDevices;
using XBEliteWPF.Services;

namespace reWASDUI.Services
{
	public class BinDataSerialize
	{
		public GamepadService GamepadService { get; set; }

		public BinDataSerialize(GamepadService gamepadService, IGameProfilesService gameProfilesService)
		{
			this._gameProfilesService = gameProfilesService;
			this.GamepadService = gamepadService;
			string text = "Anonymous";
			WindowsIdentity current = WindowsIdentity.GetCurrent();
			SecurityIdentifier user = current.User;
			new WindowsPrincipal(current).IsInRole(WindowsBuiltInRole.Administrator);
			if (user != null)
			{
				text = user.ToString().RemoveCharacters(new char[] { '{', '}', '-' });
			}
			BinDataSerialize.USER_IMPERSONATED_GAMEPAD_COMPOSITE_COLLECTION_FULL_SAVE_FILE_PATH = Path.Combine(Constants.PROGRAMM_DATA_DIRECTORY_PATH, "CompositeDevicesCollection_" + text + ".json");
			BinDataSerialize.USER_IMPERSONATED_EXTERNAL_DEVICES_RELATIONS_COLLECTION_FULL_SAVE_FILE_PATH = Path.Combine(Constants.PROGRAMM_DATA_DIRECTORY_PATH, "ExternalDevicesRelations_" + text + ".json");
			this.CheckAndCreateProgramDataDir();
		}

		public async Task LoadAllBins()
		{
			await this.LoadGamepadProfileRelations();
			await this.LoadGamepadsSettings();
			await this.LoadBlacklistDevices();
			await this.LoadExternalDevices();
			await this.LoadExternalClients();
			await this.LoadAutoGamesDetectionGamepadProfileRelations(true);
			await this.LoadGamepadsSlotHotkeyCollection();
			await this.LoadGamepadsUserLedCollection();
			await this.LoadCompositeDevicesCollection(false);
			await this.LoadPeripheralDevicesCollection();
		}

		private void CheckAndCreateProgramDataDir()
		{
			try
			{
				if (!Directory.Exists(Constants.PROGRAMM_DATA_DIRECTORY_PATH))
				{
					Tracer.TraceWrite("Creating " + Constants.PROGRAMM_DATA_DIRECTORY_PATH, false);
					Directory.CreateDirectory(Constants.PROGRAMM_DATA_DIRECTORY_PATH);
				}
			}
			catch (Exception)
			{
			}
		}

		public async Task LoadGamepadProfileRelations()
		{
			Tracer.TraceWrite("BinDataSerialize.LoadGamepadProfileRelations", false);
			GamepadService gamepadService = this.GamepadService;
			GamepadProfilesCollection gamepadProfilesCollection = await App.HttpClientService.Gamepad.GetGamepadProfileRelations();
			gamepadService.GamepadProfileRelations = gamepadProfilesCollection;
			gamepadService = null;
			this.GamepadService.RefreshRemapStateProperties();
		}

		public async Task<bool> SaveAutoGamesDetectionGamepadProfileRelations()
		{
			return await App.HttpClientService.Gamepad.SaveAutoGamesDetectionGamepadProfileRelations(this.GamepadService.AutoGamesDetectionGamepadProfileRelations);
		}

		public async Task LoadAutoGamesDetectionGamepadProfileRelations(bool isRemoveNonExistent = true)
		{
			Tracer.TraceWrite("BinDataSerialize.LoadAutoGamesDetectionGamepadProfileRelations", false);
			GamepadService gamepadService = this.GamepadService;
			AutoGamesDetectionGamepadProfilesCollection autoGamesDetectionGamepadProfilesCollection = await App.HttpClientService.Gamepad.GetAutoGamesDetectionGamepadProfileRelations();
			gamepadService.AutoGamesDetectionGamepadProfileRelations = autoGamesDetectionGamepadProfilesCollection;
			gamepadService = null;
		}

		public async Task LoadExternalDevices()
		{
			ExternalDevicesCollection externalDevicesCollection = await App.HttpClientService.ExternalDevices.GetExternalDevices();
			if (externalDevicesCollection != null)
			{
				this.GamepadService.ExternalDevices = externalDevicesCollection;
			}
		}

		public async Task<bool> SaveExternalDevices()
		{
			return await App.HttpClientService.ExternalDevices.SaveExternalDevices(this.GamepadService.ExternalDevices);
		}

		public async Task LoadExternalClients()
		{
			ObservableCollection<ExternalClient> observableCollection = await App.HttpClientService.ExternalDevices.GetExternalClients();
			if (observableCollection != null)
			{
				this.GamepadService.ExternalClients = observableCollection;
			}
		}

		public async Task<bool> SaveExternalClients()
		{
			return await App.HttpClientService.ExternalDevices.SaveExternalClients(this.GamepadService.ExternalClients);
		}

		public async Task LoadBlacklistDevices()
		{
			ObservableCollection<BlackListGamepad> observableCollection = await App.HttpClientService.Gamepad.GetBlacklistDevices();
			if (observableCollection != null)
			{
				this.GamepadService.BlacklistGamepads = observableCollection;
			}
		}

		public async Task<bool> SaveBlacklistDevices()
		{
			return await App.HttpClientService.Gamepad.SaveBlacklistDevices(this.GamepadService.BlacklistGamepads);
		}

		public async Task LoadGamepadsSettings()
		{
			GamepadService gamepadService = this.GamepadService;
			ObservableCollection<GamepadSettings> observableCollection = await App.HttpClientService.Gamepad.GetGamepadsSettings();
			gamepadService.GamepadsSettings = observableCollection;
			gamepadService = null;
			if (this.GamepadService.GamepadsSettings == null)
			{
				this.GamepadService.GamepadsSettings = new ObservableCollection<GamepadSettings>();
			}
		}

		public async Task<bool> SaveGamepadsSettings()
		{
			return await App.HttpClientService.Gamepad.SaveGamepadsSettings(this.GamepadService.GamepadsSettings);
		}

		public async Task LoadGamepadsSlotHotkeyCollection()
		{
			Tracer.TraceWrite("BinDataSerialize.LoadGamepadsSlotHotkeyCollection", false);
			GamepadService gamepadService = this.GamepadService;
			GamepadsHotkeyDictionary gamepadsHotkeyDictionary = await App.HttpClientService.Gamepad.GetGamepadsHotkeyCollection();
			gamepadService.GamepadsHotkeyCollection = gamepadsHotkeyDictionary;
			gamepadService = null;
		}

		public async Task<bool> SaveGamepadsHotkeyCollection()
		{
			Tracer.TraceWrite("BinDataSerialize.SaveGamepadsHotkeyCollection", false);
			return await App.HttpClientService.Gamepad.SaveGamepadsHotkeyCollection(this.GamepadService.GamepadsHotkeyCollection);
		}

		public async Task LoadGamepadsUserLedCollection()
		{
			Tracer.TraceWrite("BinDataSerialize.LoadGamepadsUserLedCollection", false);
			GamepadService gamepadService = this.GamepadService;
			GamepadsPlayerLedDictionary gamepadsPlayerLedDictionary = await App.HttpClientService.Gamepad.GetGamepadsUserLedCollection();
			gamepadService.GamepadsUserLedCollection = gamepadsPlayerLedDictionary;
			gamepadService = null;
		}

		public async Task<bool> SaveGamepadsUserLedCollection()
		{
			Tracer.TraceWrite("BinDataSerialize.SaveGamepadsUserLedCollection", false);
			return await App.HttpClientService.Gamepad.SaveGamepadsUserLedCollection(this.GamepadService.GamepadsUserLedCollection);
		}

		public async Task<bool> SaveCompositeDevicesCollection()
		{
			Tracer.TraceWrite("BinDataSerialize.SaveCompositeDevicesCollection", false);
			return await App.HttpClientService.Gamepad.SaveCompositeDevicesCollection(this.GamepadService.CompositeDevices);
		}

		public async Task LoadCompositeDevicesCollection(bool refreshControllers = false)
		{
			Tracer.TraceWrite("BinDataSerialize.LoadCompositeDevicesCollection", false);
			GamepadService gamepadService = this.GamepadService;
			CompositeDevices compositeDevices = await App.HttpClientService.Gamepad.GetCompositeDevicesCollection();
			gamepadService.CompositeDevices = compositeDevices;
			gamepadService = null;
			if (refreshControllers)
			{
				BaseControllerVM currentGamepad = this.GamepadService.CurrentGamepad;
				if (currentGamepad != null)
				{
					string id = currentGamepad.ID;
				}
				foreach (BaseControllerVM baseControllerVM in this.GamepadService.GamepadCollection)
				{
					CompositeDevice compositeDevice = null;
					ControllerVM controllerVM = baseControllerVM as ControllerVM;
					if (controllerVM != null)
					{
						compositeDevice = this.GamepadService.CompositeDevices.FindCompositeForSimple(controllerVM);
					}
					else
					{
						PeripheralVM peripheralVM = baseControllerVM as PeripheralVM;
						if (peripheralVM != null)
						{
							compositeDevice = this.GamepadService.CompositeDevices.FindCompositeForSimple(peripheralVM);
						}
					}
					if (compositeDevice != null && !baseControllerVM.IsInsideCompositeDevice)
					{
						baseControllerVM.IsInsideCompositeDevice = true;
					}
				}
			}
		}

		public async Task LoadPeripheralDevicesCollection()
		{
			Tracer.TraceWrite("BinDataSerialize.LoadPeripheralDevicesCollection", false);
			GamepadService gamepadService = this.GamepadService;
			PeripheralDevices peripheralDevices = await App.HttpClientService.Gamepad.GetPeripheralDevices();
			gamepadService.PeripheralDevices = peripheralDevices;
			gamepadService = null;
		}

		public static string USER_IMPERSONATED_GAMEPAD_COMPOSITE_COLLECTION_FULL_SAVE_FILE_PATH;

		public static string USER_IMPERSONATED_EXTERNAL_DEVICES_RELATIONS_COLLECTION_FULL_SAVE_FILE_PATH;

		private static int SentTotalProfilesNumber = RegistryHelper.GetValue("Analytics", "ProfilesAppliedForConnected", 0, false);

		private IGameProfilesService _gameProfilesService;

		private class PeripheralComparer : IEqualityComparer<PeripheralDevice>
		{
			public int GetHashCode(PeripheralDevice dev)
			{
				return dev.ID.GetHashCode();
			}

			public bool Equals(PeripheralDevice dev1, PeripheralDevice dev2)
			{
				return dev1.ID == dev2.ID;
			}
		}
	}
}
