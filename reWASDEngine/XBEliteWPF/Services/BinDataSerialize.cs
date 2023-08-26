using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using Newtonsoft.Json;
using Prism.Events;
using reWASDCommon.Infrastructure;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Infrastructure.LED;
using reWASDCommon.Utils;
using reWASDEngine;
using XBEliteWPF.DataModels;
using XBEliteWPF.DataModels.CompositeDevicesCollection;
using XBEliteWPF.DataModels.GamepadActiveProfiles;
using XBEliteWPF.DataModels.GamepadSlotHotkeyCollection;
using XBEliteWPF.DataModels.InitializedDevicesCollection;
using XBEliteWPF.DataModels.PeripheralDevicesCollection;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.Controller;
using XBEliteWPF.Infrastructure.ExternalDevices;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils;

namespace XBEliteWPF.Services
{
	public class BinDataSerialize
	{
		private static int SentTotalProfilesNumber
		{
			get
			{
				return RegistryHelper.GetValue("Analytics", "ProfilesAppliedForConnected", 0, false);
			}
			set
			{
				RegistryHelper.SetValue("Analytics", "ProfilesAppliedForConnected", value);
			}
		}

		public GamepadService GamepadService { get; set; }

		public BinDataSerialize(GamepadService gamepadService, IGameProfilesService gameProfilesService, IXBServiceCommunicator xbServiceCommunicator)
		{
			this._gameProfilesService = gameProfilesService;
			this._xbServiceCommunicator = xbServiceCommunicator;
			this.GamepadService = gamepadService;
			string currentUserStringId = UtilsCommon.GetCurrentUserStringId();
			BinDataSerialize.USER_IMPERSONATED_GAMEPAD_PROFILE_RELATION_FULL_SAVE_FILE_PATH = Path.Combine(Constants.PROGRAMM_DATA_DIRECTORY_PATH, "GamepadProfileUserRelations_" + currentUserStringId + ".json");
			BinDataSerialize.USER_IMPERSONATED_AUTOREMAP_GAMEPAD_PROFILE_RELATION_FULL_SAVE_FILE_PATH = Path.Combine(Constants.PROGRAMM_DATA_DIRECTORY_PATH, "AutoRemapGamepadProfileUserRelations_" + currentUserStringId + ".json");
			BinDataSerialize.USER_IMPERSONATED_GAMEPAD_HOTKEY_SLOT_COLLECTION_FULL_SAVE_FILE_PATH = Path.Combine(Constants.PROGRAMM_DATA_DIRECTORY_PATH, "GamepadHotkeySlotCollection_" + currentUserStringId + ".json");
			BinDataSerialize.USER_IMPERSONATED_GAMEPAD_PLAYER_LED_COLLECTION_FULL_SAVE_FILE_PATH = Path.Combine(Constants.PROGRAMM_DATA_DIRECTORY_PATH, "GamepadPlayerLedCollection_" + currentUserStringId + ".json");
			BinDataSerialize.USER_IMPERSONATED_GAMEPAD_COMPOSITE_COLLECTION_FULL_SAVE_FILE_PATH = Path.Combine(Constants.PROGRAMM_DATA_DIRECTORY_PATH, "CompositeDevicesCollection_" + currentUserStringId + ".json");
			BinDataSerialize.USER_IMPERSONATED_PERIPHERAL_DEVICES_COLLECTION_FULL_SAVE_FILE_PATH = Path.Combine(Constants.PROGRAMM_DATA_DIRECTORY_PATH, "PeripheralDevicesCollection_" + currentUserStringId + ".json");
			BinDataSerialize.USER_IMPERSONATED_INITIALIZED_DEVICES_COLLECTION_FULL_SAVE_FILE_PATH = Path.Combine(Constants.PROGRAMM_DATA_DIRECTORY_PATH, "InitializedDevicesCollection_" + currentUserStringId + ".json");
			BinDataSerialize.USER_IMPERSONATED_EXTERNAL_DEVICES_RELATIONS_COLLECTION_FULL_SAVE_FILE_PATH = Path.Combine(Constants.PROGRAMM_DATA_DIRECTORY_PATH, "ExternalDevicesRelations_" + currentUserStringId + ".json");
			try
			{
				if (!Directory.Exists(Constants.PROGRAMM_DATA_DIRECTORY_PATH))
				{
					Tracer.TraceWrite("Creating " + Constants.PROGRAMM_DATA_DIRECTORY_PATH, false);
					Directory.CreateDirectory(Constants.PROGRAMM_DATA_DIRECTORY_PATH);
				}
			}
			catch (Exception ex)
			{
				Tracer.TraceException(ex, ".ctor");
			}
		}

		public void LoadAllBins()
		{
			this.LoadBlacklistDevices();
			this.LoadExternalDevices();
			this.LoadExternalClients();
			this.LoadGamepadsSettings();
			this.LoadGamepadsSlotHotkeyCollection();
			this.LoadGamepadCollection();
			this.LoadGamepadProfileRelations(true);
			this.LoadAutoGamesDetectionGamepadProfileRelations(true);
			this.LoadGamepadsUserLedCollection();
			this.LoadCompositeDevicesCollection(false);
			this.LoadPeripheralDevicesCollection(false);
			this.LoadInitializedDevicesCollection(false);
		}

		public void SaveGamepadProfileRelations()
		{
			Tracer.TraceWrite("GamepadService.SaveGamepadProfileRelations", false);
			int num = 0;
			using (IEnumerator<KeyValuePair<string, GamepadProfiles>> enumerator = ((IEnumerable<KeyValuePair<string, GamepadProfiles>>)this.GamepadService.GamepadProfileRelations).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, GamepadProfiles> gpr = enumerator.Current;
					if (this.GamepadService.GamepadCollection.FirstOrDefault((BaseControllerVM g) => g.ID == gpr.Key) != null)
					{
						foreach (KeyValuePair<Slot, GamepadProfile> keyValuePair in ((IEnumerable<KeyValuePair<Slot, GamepadProfile>>)gpr.Value.SlotProfiles))
						{
							GamepadProfile value = keyValuePair.Value;
							if (((value != null) ? value.Config : null) != null)
							{
								num++;
							}
						}
					}
				}
			}
			if (BinDataSerialize.SentTotalProfilesNumber != num && num > 0)
			{
				BinDataSerialize.SentTotalProfilesNumber = num;
				SenderGoogleAnalytics.SendMessageEvent("GUI", "AppliedNumber", num.ToString(), -1L, false);
			}
			BinDataSerialize.Save(this.GamepadService.GamepadProfileRelations, BinDataSerialize.USER_IMPERSONATED_GAMEPAD_PROFILE_RELATION_FULL_SAVE_FILE_PATH);
			IEventAggregator eventAggregator = Engine.EventAggregator;
			if (eventAggregator == null)
			{
				return;
			}
			eventAggregator.GetEvent<ProfileRelationsChangedByEngine>().Publish(null);
		}

		public async Task LoadGamepadProfileRelations(bool isRemoveNonExistent = true)
		{
			Tracer.TraceWrite("GamepadService.LoadGamepadProfileRelations", false);
			string profileRelationPathToRead = null;
			if (File.Exists(BinDataSerialize.USER_IMPERSONATED_GAMEPAD_PROFILE_RELATION_FULL_SAVE_FILE_PATH))
			{
				profileRelationPathToRead = BinDataSerialize.USER_IMPERSONATED_GAMEPAD_PROFILE_RELATION_FULL_SAVE_FILE_PATH;
			}
			if (profileRelationPathToRead != null)
			{
				bool successfullyRead = false;
				try
				{
					FileStream fileStream = await DSUtils.WaitForFileAsync(profileRelationPathToRead, FileMode.Open, FileAccess.Read, FileShare.Read, 50, 100);
					if (fileStream == null)
					{
						return;
					}
					using (fileStream)
					{
						StreamReader streamReader = new StreamReader(fileStream);
						string text = streamReader.ReadToEnd();
						streamReader.Close();
						Dictionary<string, GamepadProfiles> dictionary = (from kvp in JsonConvert.DeserializeObject<GamepadProfilesCollection>(text)
							where kvp.Value.ControllerProfileInfoCollections != null
							select kvp).ToDictionary((KeyValuePair<string, GamepadProfiles> pair) => pair.Key, (KeyValuePair<string, GamepadProfiles> pair) => pair.Value);
						this.GamepadService.GamepadProfileRelations = new GamepadProfilesCollection(dictionary);
						fileStream.Close();
					}
					await this._xbServiceCommunicator.SetFileUsersFullAccess(profileRelationPathToRead);
					successfullyRead = true;
				}
				catch (Exception ex)
				{
					Tracer.TraceException(ex, "LoadGamepadProfileRelations");
				}
				if (successfullyRead)
				{
					if (isRemoveNonExistent)
					{
						foreach (KeyValuePair<string, GamepadProfiles> gamepadProfileRelation in ((IEnumerable<KeyValuePair<string, GamepadProfiles>>)this.GamepadService.GamepadProfileRelations))
						{
							List<Slot> list = new List<Slot>();
							foreach (KeyValuePair<Slot, GamepadProfile> keyValuePair in ((IEnumerable<KeyValuePair<Slot, GamepadProfile>>)gamepadProfileRelation.Value.SlotProfiles))
							{
								if (keyValuePair.Value != null && !File.Exists(keyValuePair.Value.ConfigPath))
								{
									list.Add(keyValuePair.Key);
								}
							}
							foreach (Slot slot in list)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(78, 1);
								defaultInterpolatedStringHandler.AppendLiteral("LoadGamepadProfileRelations: GamepadProfileRelation.Value.SlotProfiles.Remove ");
								defaultInterpolatedStringHandler.AppendFormatted<Slot>(slot);
								Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
								gamepadProfileRelation.Value.SlotProfiles.Remove(slot);
							}
							if (list.Count > 0)
							{
								if (this.GamepadService.ServiceProfilesCollection.Count == 0)
								{
									await this.GamepadService.RefreshServiceProfiles();
								}
								await this.GamepadService.DisableRemap(gamepadProfileRelation.Key, true);
							}
							gamepadProfileRelation = default(KeyValuePair<string, GamepadProfiles>);
						}
						IEnumerator<KeyValuePair<string, GamepadProfiles>> enumerator = null;
					}
				}
			}
		}

		public bool SaveAutoGamesDetectionGamepadProfileRelations()
		{
			Tracer.TraceWrite("GamepadService.SaveAutoGamesDetectionGamepadProfileRelations", false);
			List<string> list = new List<string>();
			List<Slot> list2 = new List<Slot>();
			IEnumerator crutchEnumerator = this.GamepadService.AutoGamesDetectionGamepadProfileRelations.GetCrutchEnumerator();
			while (crutchEnumerator.MoveNext())
			{
				KeyValuePair<string, GamepadProfilesCollection> gamepadProfileRelations = (KeyValuePair<string, GamepadProfilesCollection>)crutchEnumerator.Current;
				if (this._gameProfilesService.GamesCollection.FirstOrDefault((Game g) => g.Name == gamepadProfileRelations.Key) == null)
				{
					list.Add(gamepadProfileRelations.Key);
				}
				else
				{
					IEnumerator crutchEnumerator2 = gamepadProfileRelations.Value.GetCrutchEnumerator();
					while (crutchEnumerator2.MoveNext())
					{
						object obj = crutchEnumerator2.Current;
						KeyValuePair<string, GamepadProfiles> keyValuePair = (KeyValuePair<string, GamepadProfiles>)obj;
						list2.Clear();
					}
				}
			}
			foreach (string text in list)
			{
				this.GamepadService.AutoGamesDetectionGamepadProfileRelations.Remove(text);
			}
			if (!BinDataSerialize.Save(this.GamepadService.AutoGamesDetectionGamepadProfileRelations, BinDataSerialize.USER_IMPERSONATED_AUTOREMAP_GAMEPAD_PROFILE_RELATION_FULL_SAVE_FILE_PATH))
			{
				return false;
			}
			IEventAggregator eventAggregator = Engine.EventAggregator;
			if (eventAggregator != null)
			{
				eventAggregator.GetEvent<AutoProfileRelationsChanged>().Publish(null);
			}
			return true;
		}

		public async Task LoadAutoGamesDetectionGamepadProfileRelations(bool isRemoveNonExistent = true)
		{
			Tracer.TraceWrite("GamepadService.LoadAutoGamesDetectionGamepadProfileRelations", false);
			bool successfullyRead = false;
			try
			{
				if (File.Exists(BinDataSerialize.USER_IMPERSONATED_AUTOREMAP_GAMEPAD_PROFILE_RELATION_FULL_SAVE_FILE_PATH))
				{
					FileStream fileStream = await DSUtils.WaitForFileAsync(BinDataSerialize.USER_IMPERSONATED_AUTOREMAP_GAMEPAD_PROFILE_RELATION_FULL_SAVE_FILE_PATH, FileMode.Open, FileAccess.Read, FileShare.Read, 50, 100);
					if (fileStream == null)
					{
						return;
					}
					using (fileStream)
					{
						StreamReader streamReader = new StreamReader(fileStream);
						string text = streamReader.ReadToEnd();
						streamReader.Close();
						AutoGamesDetectionGamepadProfilesCollection autoGamesDetectionGamepadProfilesCollection = JsonConvert.DeserializeObject<AutoGamesDetectionGamepadProfilesCollection>(text);
						this.GamepadService.AutoGamesDetectionGamepadProfileRelations = ((autoGamesDetectionGamepadProfilesCollection != null) ? autoGamesDetectionGamepadProfilesCollection : new AutoGamesDetectionGamepadProfilesCollection());
						fileStream.Close();
					}
					await this._xbServiceCommunicator.SetFileUsersFullAccess(BinDataSerialize.USER_IMPERSONATED_AUTOREMAP_GAMEPAD_PROFILE_RELATION_FULL_SAVE_FILE_PATH);
					successfullyRead = true;
				}
			}
			catch (Exception ex)
			{
				Tracer.TraceException(ex, "LoadAutoGamesDetectionGamepadProfileRelations");
			}
			if (successfullyRead)
			{
				if (isRemoveNonExistent)
				{
					foreach (KeyValuePair<string, GamepadProfilesCollection> keyValuePair in ((IEnumerable<KeyValuePair<string, GamepadProfilesCollection>>)this.GamepadService.AutoGamesDetectionGamepadProfileRelations))
					{
						foreach (KeyValuePair<string, GamepadProfiles> keyValuePair2 in ((IEnumerable<KeyValuePair<string, GamepadProfiles>>)keyValuePair.Value))
						{
							List<Slot> list = new List<Slot>();
							foreach (KeyValuePair<Slot, GamepadProfile> keyValuePair3 in ((IEnumerable<KeyValuePair<Slot, GamepadProfile>>)keyValuePair2.Value.SlotProfiles))
							{
								if (keyValuePair3.Value != null && !File.Exists(keyValuePair3.Value.ConfigPath))
								{
									list.Add(keyValuePair3.Key);
								}
							}
							foreach (Slot slot in list)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(92, 1);
								defaultInterpolatedStringHandler.AppendLiteral("LoadAutoGamesDetectionGamepadProfileRelations: AutoGamepadProfile.Value.SlotProfiles.Remove ");
								defaultInterpolatedStringHandler.AppendFormatted<Slot>(slot);
								Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
								keyValuePair2.Value.SlotProfiles.Remove(slot);
							}
						}
					}
				}
			}
		}

		public bool SaveGamepadsHotkeyCollection()
		{
			return BinDataSerialize.Save(this.GamepadService.GamepadsHotkeyCollection, BinDataSerialize.USER_IMPERSONATED_GAMEPAD_HOTKEY_SLOT_COLLECTION_FULL_SAVE_FILE_PATH);
		}

		public void LoadExternalDevices()
		{
			ExternalDevicesCollection externalDevicesCollection = BinDataSerialize.Load<ExternalDevicesCollection>(BinDataSerialize.USER_IMPERSONATED_EXTERNAL_DEVICES_COLLECTION_FULL_SAVE_FILE_PATH, null);
			if (externalDevicesCollection != null)
			{
				this.GamepadService.ExternalDevices = externalDevicesCollection;
			}
		}

		public bool SaveExternalDevices()
		{
			return BinDataSerialize.Save(this.GamepadService.ExternalDevices, BinDataSerialize.USER_IMPERSONATED_EXTERNAL_DEVICES_COLLECTION_FULL_SAVE_FILE_PATH);
		}

		public void LoadExternalClients()
		{
			ObservableCollection<ExternalClient> observableCollection = BinDataSerialize.Load<ObservableCollection<ExternalClient>>(BinDataSerialize.USER_IMPERSONATED_EXTERNAL_CLIENTS_COLLECTION_FULL_SAVE_FILE_PATH, null);
			if (observableCollection != null)
			{
				this.GamepadService.ExternalClients = observableCollection;
			}
		}

		public bool SaveExternalClients()
		{
			return BinDataSerialize.Save(this.GamepadService.ExternalClients, BinDataSerialize.USER_IMPERSONATED_EXTERNAL_CLIENTS_COLLECTION_FULL_SAVE_FILE_PATH);
		}

		public void LoadBlacklistDevices()
		{
			ObservableCollection<BlackListGamepad> observableCollection = BinDataSerialize.Load<ObservableCollection<BlackListGamepad>>(BinDataSerialize.USER_IMPERSONATED_GAMEPAD_BLACKLIST_COLLECTION_FULL_SAVE_FILE_PATH, null);
			if (observableCollection != null)
			{
				this.GamepadService.BlacklistGamepads = observableCollection;
			}
		}

		public bool SaveBlacklistDevices()
		{
			return BinDataSerialize.Save(this.GamepadService.BlacklistGamepads, BinDataSerialize.USER_IMPERSONATED_GAMEPAD_BLACKLIST_COLLECTION_FULL_SAVE_FILE_PATH);
		}

		public void LoadGamepadCollection()
		{
			ObservableCollection<BaseControllerVM> observableCollection = BinDataSerialize.Load<ObservableCollection<BaseControllerVM>>(BinDataSerialize.USER_IMPERSONATED_GAMEPAD_COLLECTION_FULL_SAVE_FILE_PATH, new ControllersJsonConverter());
			if (observableCollection != null)
			{
				this.GamepadService.GamepadCollection = observableCollection;
				this.GamepadService.GamepadCollection.ForEach(delegate(BaseControllerVM item)
				{
					if (!item.IsDebugController)
					{
						item.IsOnline = false;
					}
				});
			}
		}

		public bool SaveGamepadCollection()
		{
			return BinDataSerialize.Save(this.GamepadService.GamepadCollection, BinDataSerialize.USER_IMPERSONATED_GAMEPAD_COLLECTION_FULL_SAVE_FILE_PATH);
		}

		public void LoadGamepadsSettings()
		{
			ObservableCollection<GamepadSettings> observableCollection = BinDataSerialize.Load<ObservableCollection<GamepadSettings>>(BinDataSerialize.USER_IMPERSONATED_GAMEPAD_SETTINGS_COLLECTION_FULL_SAVE_FILE_PATH, null);
			if (observableCollection != null)
			{
				this.GamepadService.GamepadsSettings = observableCollection;
			}
		}

		public bool SaveGamepadsSettings()
		{
			bool flag = BinDataSerialize.Save(this.GamepadService.GamepadsSettings, BinDataSerialize.USER_IMPERSONATED_GAMEPAD_SETTINGS_COLLECTION_FULL_SAVE_FILE_PATH);
			Engine.EventAggregator.GetEvent<GamepadsSettingsChanged>().Publish(null);
			return flag;
		}

		public void LoadGamepadsSlotHotkeyCollection()
		{
			GamepadsHotkeyDictionary gamepadsHotkeyDictionary = BinDataSerialize.Load<GamepadsHotkeyDictionary>(BinDataSerialize.USER_IMPERSONATED_GAMEPAD_HOTKEY_SLOT_COLLECTION_FULL_SAVE_FILE_PATH, null);
			if (gamepadsHotkeyDictionary == null)
			{
				gamepadsHotkeyDictionary = new GamepadsHotkeyDictionary();
			}
			this.GamepadService.GamepadsHotkeyCollection = gamepadsHotkeyDictionary;
		}

		public async Task<bool> SaveCompositeDevicesCollection()
		{
			bool success = BinDataSerialize.Save(this.GamepadService.CompositeDevices, BinDataSerialize.USER_IMPERSONATED_GAMEPAD_COMPOSITE_COLLECTION_FULL_SAVE_FILE_PATH);
			if (success)
			{
				this.GamepadService.RemoveControllersByCompositeDevices();
				await this.GamepadService.RefreshGamepadCollection(null);
			}
			return success;
		}

		public void LoadCompositeDevicesCollection(bool refreshControllers = false)
		{
			CompositeDevices compositeDevices = BinDataSerialize.Load<CompositeDevices>(BinDataSerialize.USER_IMPERSONATED_GAMEPAD_COMPOSITE_COLLECTION_FULL_SAVE_FILE_PATH, null);
			if (compositeDevices == null)
			{
				compositeDevices = new CompositeDevices();
			}
			this.GamepadService.CompositeDevices = compositeDevices;
			if (refreshControllers)
			{
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
				this.GamepadService.RefreshGamepadCollection(null);
			}
		}

		public bool SavePeripheralDevicesCollection()
		{
			return BinDataSerialize.Save(this.GamepadService.PeripheralDevices, BinDataSerialize.USER_IMPERSONATED_PERIPHERAL_DEVICES_COLLECTION_FULL_SAVE_FILE_PATH);
		}

		private string GetIDUpdatedPeripheralDevice(PeripheralDevices loaded)
		{
			PeripheralDevices peripheralDevices = this.GamepadService.PeripheralDevices;
			IEnumerable<PeripheralDevice> enumerable = loaded.Except(peripheralDevices, new BinDataSerialize.PeripheralComparer());
			IEnumerable<PeripheralDevice> enumerable2 = peripheralDevices.Except(loaded, new BinDataSerialize.PeripheralComparer());
			PeripheralDevice peripheralDevice = enumerable.Concat(enumerable2).FirstOrDefault<PeripheralDevice>();
			string text = ((peripheralDevice != null) ? peripheralDevice.ID : null);
			foreach (BaseControllerVM baseControllerVM in this.GamepadService.GamepadCollection)
			{
				PeripheralVM peripheralVM = baseControllerVM as PeripheralVM;
				if (peripheralVM != null && peripheralVM.IsConsideredTheSameControllerByID(text))
				{
					text = baseControllerVM.ID;
					break;
				}
			}
			return text;
		}

		public void LoadPeripheralDevicesCollection(bool refreshControllers = false)
		{
			PeripheralDevices peripheralDevices = BinDataSerialize.Load<PeripheralDevices>(BinDataSerialize.USER_IMPERSONATED_PERIPHERAL_DEVICES_COLLECTION_FULL_SAVE_FILE_PATH, null);
			if (peripheralDevices == null)
			{
				peripheralDevices = new PeripheralDevices();
			}
			string curID = this.GetIDUpdatedPeripheralDevice(peripheralDevices);
			this.GamepadService.PeripheralDevices = peripheralDevices;
			if (refreshControllers)
			{
				this.GamepadService.GamepadCollection.Remove((BaseControllerVM it) => it.ID == curID);
				this.GamepadService.AllPhysicalControllers.Remove((BaseControllerVM it) => it.ID == curID);
				this.GamepadService.RefreshGamepadCollection(curID);
			}
		}

		public bool SaveInitializedDevicesCollection(string refreshControllerId = "")
		{
			return BinDataSerialize.Save(this.GamepadService.InitializedDevices, BinDataSerialize.USER_IMPERSONATED_INITIALIZED_DEVICES_COLLECTION_FULL_SAVE_FILE_PATH);
		}

		private string GetIDUpdatedInitilizedDevice(InitializedDevices loaded)
		{
			InitializedDevices current = this.GamepadService.InitializedDevices;
			InitializedDevice initializedDevice = loaded.FirstOrDefault((InitializedDevice it) => current.All((InitializedDevice el) => el.ID != it.ID));
			if (initializedDevice == null)
			{
				initializedDevice = current.FirstOrDefault((InitializedDevice it) => loaded.All((InitializedDevice el) => el.ID == it.ID));
			}
			if (initializedDevice != null)
			{
				string id = initializedDevice.ID;
			}
			if (initializedDevice == null)
			{
				return null;
			}
			return initializedDevice.ID;
		}

		public void LoadInitializedDevicesCollection(bool refreshControllers = false)
		{
			InitializedDevices initializedDevices = BinDataSerialize.Load<InitializedDevices>(BinDataSerialize.USER_IMPERSONATED_INITIALIZED_DEVICES_COLLECTION_FULL_SAVE_FILE_PATH, null);
			if (initializedDevices == null)
			{
				initializedDevices = new InitializedDevices();
			}
			string curID = this.GetIDUpdatedInitilizedDevice(initializedDevices);
			this.GamepadService.InitializedDevices = initializedDevices;
			if (refreshControllers)
			{
				this.GamepadService.GamepadCollection.Remove((BaseControllerVM it) => it.ID == curID);
				this.GamepadService.AllPhysicalControllers.Remove((BaseControllerVM it) => it.ID == curID);
				this.GamepadService.RefreshGamepadCollection(curID);
			}
		}

		public void LoadGamepadsUserLedCollection()
		{
			GamepadsPlayerLedDictionary gamepadsPlayerLedDictionary = BinDataSerialize.Load<GamepadsPlayerLedDictionary>(BinDataSerialize.USER_IMPERSONATED_GAMEPAD_PLAYER_LED_COLLECTION_FULL_SAVE_FILE_PATH, null);
			if (gamepadsPlayerLedDictionary == null)
			{
				gamepadsPlayerLedDictionary = new GamepadsPlayerLedDictionary();
			}
			this.FixPlayerLedDictionaryForDualSense(gamepadsPlayerLedDictionary);
			this.GamepadService.GamepadsUserLedCollection = gamepadsPlayerLedDictionary;
		}

		private void FixPlayerLedDictionaryForDualSense(GamepadsPlayerLedDictionary dic)
		{
			bool flag = false;
			foreach (KeyValuePair<string, PlayerLedSettings> keyValuePair in dic)
			{
				if (keyValuePair.Value.SupportedDeviceType == 10 && keyValuePair.Value.LedNumber > 4)
				{
					keyValuePair.Value.LedNumber = 1;
					flag = true;
				}
			}
			if (flag)
			{
				this.SaveGamepadsUserLedCollection();
			}
		}

		public bool SaveGamepadsUserLedCollection()
		{
			return BinDataSerialize.Save(this.GamepadService.GamepadsUserLedCollection, BinDataSerialize.USER_IMPERSONATED_GAMEPAD_PLAYER_LED_COLLECTION_FULL_SAVE_FILE_PATH);
		}

		public static T Load<T>(string binFilePath, JsonConverter jsonConverter = null)
		{
			Tracer.TraceWrite("Load bin file " + binFilePath, false);
			T t = default(T);
			try
			{
				if (File.Exists(binFilePath))
				{
					using (FileStream fileStream = DSUtils.WaitForFile(binFilePath, FileMode.Open, FileAccess.Read, FileShare.None, 50, 100))
					{
						using (StreamReader streamReader = new StreamReader(fileStream))
						{
							string text = streamReader.ReadToEnd();
							if (jsonConverter == null)
							{
								t = JsonConvert.DeserializeObject<T>(text);
							}
							else
							{
								t = JsonConvert.DeserializeObject<T>(text, new JsonConverter[] { jsonConverter });
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Tracer.TraceException(ex, "Load");
			}
			return t;
		}

		public static bool Save(object obj, string binFilePath)
		{
			Tracer.TraceWrite("Save bin file " + binFilePath, false);
			try
			{
				if (!Directory.Exists(Constants.PROGRAMM_DATA_DIRECTORY_PATH))
				{
					Tracer.TraceWrite("Creating " + Constants.PROGRAMM_DATA_DIRECTORY_PATH, false);
					Directory.CreateDirectory(Constants.PROGRAMM_DATA_DIRECTORY_PATH);
				}
				using (FileStream fileStream = DSUtils.WaitForFile(binFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 50, 100))
				{
					using (StreamWriter streamWriter = new StreamWriter(fileStream))
					{
						Tracer.TraceWrite("Opened " + binFilePath + " for writing", false);
						streamWriter.Write(JsonConvert.SerializeObject(obj, 1));
					}
				}
				Engine.XBServiceCommunicator.SetFileUsersFullAccess(binFilePath);
			}
			catch (Exception ex)
			{
				Tracer.TraceException(ex, "Save");
				return false;
			}
			return true;
		}

		public static readonly string USER_IMPERSONATED_GAMEPAD_BLACKLIST_COLLECTION_FULL_SAVE_FILE_PATH = Path.Combine(Constants.PROGRAMM_DATA_DIRECTORY_PATH, "GamepadBlacklist.json");

		public static readonly string USER_IMPERSONATED_GAMEPAD_COLLECTION_FULL_SAVE_FILE_PATH = Path.Combine(Constants.PROGRAMM_DATA_DIRECTORY_PATH, "GamepadCollection.json");

		public static readonly string USER_IMPERSONATED_EXTERNAL_DEVICES_COLLECTION_FULL_SAVE_FILE_PATH = Path.Combine(Constants.PROGRAMM_DATA_DIRECTORY_PATH, "ExternalDevices.json");

		public static readonly string USER_IMPERSONATED_EXTERNAL_CLIENTS_COLLECTION_FULL_SAVE_FILE_PATH = Path.Combine(Constants.PROGRAMM_DATA_DIRECTORY_PATH, "ExternalClients.json");

		public static readonly string USER_IMPERSONATED_GAMEPAD_SETTINGS_COLLECTION_FULL_SAVE_FILE_PATH = Path.Combine(Constants.PROGRAMM_DATA_DIRECTORY_PATH, "GamepadsSettings.json");

		public static string USER_IMPERSONATED_GAMEPAD_PROFILE_RELATION_FULL_SAVE_FILE_PATH;

		public static string USER_IMPERSONATED_AUTOREMAP_GAMEPAD_PROFILE_RELATION_FULL_SAVE_FILE_PATH;

		public static string USER_IMPERSONATED_GAMEPAD_HOTKEY_SLOT_COLLECTION_FULL_SAVE_FILE_PATH;

		public static string USER_IMPERSONATED_GAMEPAD_PLAYER_LED_COLLECTION_FULL_SAVE_FILE_PATH;

		public static string USER_IMPERSONATED_GAMEPAD_COMPOSITE_COLLECTION_FULL_SAVE_FILE_PATH;

		public static string USER_IMPERSONATED_PERIPHERAL_DEVICES_COLLECTION_FULL_SAVE_FILE_PATH;

		public static string USER_IMPERSONATED_INITIALIZED_DEVICES_COLLECTION_FULL_SAVE_FILE_PATH;

		public static string USER_IMPERSONATED_EXTERNAL_DEVICES_RELATIONS_COLLECTION_FULL_SAVE_FILE_PATH;

		private IGameProfilesService _gameProfilesService;

		private IXBServiceCommunicator _xbServiceCommunicator;

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
