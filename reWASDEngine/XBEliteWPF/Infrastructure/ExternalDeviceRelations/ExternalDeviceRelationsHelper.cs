using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Properties;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoftReWASDServiceNamespace;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Utils;
using reWASDEngine;
using XBEliteWPF.DataModels;
using XBEliteWPF.DataModels.GamepadActiveProfiles;
using XBEliteWPF.Infrastructure.Controller;
using XBEliteWPF.Infrastructure.ExternalDeviceRelationsModel;
using XBEliteWPF.Infrastructure.ExternalDevices;
using XBEliteWPF.Services;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.ViewModels.Base;

namespace XBEliteWPF.Infrastructure.ExternalDeviceRelations
{
	public class ExternalDeviceRelationsHelper : ZBindable
	{
		public event ExternalDeviceRelationsHelper.StateChangedDelegate CurrentExternalStateChangedEvent;

		public ExternalDeviceRelationsCollection ExternalDeviceRelationsCollection
		{
			get
			{
				return this._externalDeviceRelationsCollection;
			}
		}

		public GamepadService GamepadService { get; }

		public ObservableCollection<GamepadAuth> GamepadsAuth
		{
			get
			{
				return this._gamepadsAuth;
			}
		}

		public SortableObservableCollection<GamepadAuth> CurrentGamepadsAuthCollection
		{
			get
			{
				return this._currentGamepadsAuthCollection;
			}
		}

		[CanBeNull]
		public ExternalDeviceRelation CurrentExternalDeviceRelation
		{
			get
			{
				return this._currentExternalDeviceRelation;
			}
			set
			{
				if (this.SetProperty<ExternalDeviceRelation>(ref this._currentExternalDeviceRelation, value, "CurrentExternalDeviceRelation"))
				{
					this.OnPropertyChanged("IsCurrentExternalDeviceRelationExist");
					if (this._currentExternalDeviceRelation != null)
					{
						this.CurrentExternalDevice = this._currentExternalDeviceRelation.ExternalDevice;
					}
					else
					{
						this._currentExternalDevice = null;
						this.OnPropertyChanged("CurrentExternalDevice");
					}
					this.OnPropertyChanged("CurrentExternalClient");
					this.OnPropertyChanged("CurrentAuthGamepad");
				}
			}
		}

		public bool IsCurrentExternalDeviceRelationExist
		{
			get
			{
				return this.CurrentExternalDeviceRelation != null;
			}
		}

		public ExternalStateAgent CurrentExternalStateAgent
		{
			get
			{
				return this._currentExternalStateAgent;
			}
			set
			{
				if (this.SetProperty<ExternalStateAgent>(ref this._currentExternalStateAgent, value, "CurrentExternalStateAgent"))
				{
					ExternalDeviceRelationsHelper.StateChangedDelegate currentExternalStateChangedEvent = this.CurrentExternalStateChangedEvent;
					if (currentExternalStateChangedEvent == null)
					{
						return;
					}
					currentExternalStateChangedEvent();
				}
			}
		}

		public string CurrentExternalStateAgentTooltip
		{
			get
			{
				return this._currentExternalStateAgentTooltip;
			}
			set
			{
				if (this.SetProperty<string>(ref this._currentExternalStateAgentTooltip, value, "CurrentExternalStateAgentTooltip"))
				{
					ExternalDeviceRelationsHelper.StateChangedDelegate currentExternalStateChangedEvent = this.CurrentExternalStateChangedEvent;
					if (currentExternalStateChangedEvent == null)
					{
						return;
					}
					currentExternalStateChangedEvent();
				}
			}
		}

		public bool IsCurrentExternalDeviceBluetooth
		{
			get
			{
				ExternalDevice currentExternalDevice = this.CurrentExternalDevice;
				return currentExternalDevice != null && currentExternalDevice.DeviceType == 0;
			}
		}

		public bool IsCurrentExternalDeviceGIMX
		{
			get
			{
				ExternalDevice currentExternalDevice = this.CurrentExternalDevice;
				return currentExternalDevice != null && currentExternalDevice.DeviceType == 1;
			}
		}

		public bool IsCurrentExternalDeviceESP32
		{
			get
			{
				ExternalDevice currentExternalDevice = this.CurrentExternalDevice;
				return currentExternalDevice != null && currentExternalDevice.DeviceType == 2;
			}
		}

		public bool IsCurrentExternalDeviceESP32S2
		{
			get
			{
				ExternalDevice currentExternalDevice = this.CurrentExternalDevice;
				return currentExternalDevice != null && currentExternalDevice.DeviceType == 3;
			}
		}

		public bool IsCurrentExternalDeviceBluetoothOrESP32
		{
			get
			{
				ExternalDevice currentExternalDevice = this.CurrentExternalDevice;
				if (currentExternalDevice == null || currentExternalDevice.DeviceType != 0)
				{
					ExternalDevice currentExternalDevice2 = this.CurrentExternalDevice;
					return currentExternalDevice2 != null && currentExternalDevice2.DeviceType == 2;
				}
				return true;
			}
		}

		[CanBeNull]
		public ExternalDevice CurrentExternalDevice
		{
			get
			{
				if (this._currentExternalDevice != null)
				{
					return this._currentExternalDevice;
				}
				if (this.GamepadService.ExternalDevices.Count <= 0)
				{
					return null;
				}
				return this.GamepadService.ExternalDevices[0];
			}
			set
			{
				this.SetProperty<ExternalDevice>(ref this._currentExternalDevice, value, "CurrentExternalDevice");
			}
		}

		[CanBeNull]
		public ExternalClient CurrentExternalClient
		{
			get
			{
				ExternalDeviceRelation currentExternalDeviceRelation = this.CurrentExternalDeviceRelation;
				if (currentExternalDeviceRelation == null)
				{
					return null;
				}
				return currentExternalDeviceRelation.ExternalClient;
			}
		}

		[CanBeNull]
		public GamepadAuth CurrentAuthGamepad
		{
			get
			{
				ExternalDeviceRelation currentExternalDeviceRelation = this.CurrentExternalDeviceRelation;
				if (currentExternalDeviceRelation == null)
				{
					return null;
				}
				return currentExternalDeviceRelation.AuthGamepad;
			}
		}

		public ExternalDeviceRelationsHelper(GamepadService gamepadService, IGameProfilesService gameProfilesService)
		{
			this._gameProfilesService = gameProfilesService;
			this.GamepadService = gamepadService;
			this._externalDeviceRelationsCollection = new ExternalDeviceRelationsCollection();
			this._gamepadsAuth = new ObservableCollection<GamepadAuth>();
			this._currentGamepadsAuthCollection = new SortableObservableCollection<GamepadAuth>();
			this.LoadRelations();
			this.InitVariables();
			this.RefreshComPorts();
		}

		public async void InitVariables()
		{
			bool flag = await BluetoothUtils.IsBluetoothAdapterIsSupportedForNintendoConsole();
			this._isBluetoothAdapterIsSupportedForNintendoConsole = flag;
		}

		public ExternalDeviceState GetExternalDeviceStateWithProfiles(bool isConfigExternal, VirtualGamepadType? configVirtualGamepadType, Config config, BaseControllerVM gamepad, Slot? slot, ExternalDevice externalDevice, ExternalClient externalClient, GamepadAuth gamepadAuth, REWASD_CONTROLLER_PROFILE_EX profileEx = null)
		{
			ExternalDeviceState externalDeviceState = this.GetExternalDeviceState(isConfigExternal, configVirtualGamepadType, config, gamepad, slot, externalDevice, externalClient, gamepadAuth, null);
			if (externalDeviceState == 2)
			{
				REWASD_CONTROLLER_PROFILE_EX rewasd_CONTROLLER_PROFILE_EX = null;
				if (externalDevice != null && this.GamepadService.IsAnyServiceProfileWithExternalDevice(externalDevice.DeviceType))
				{
					rewasd_CONTROLLER_PROFILE_EX = this.GamepadService.GetProfileEx((gamepad != null) ? gamepad.ID : null);
				}
				if (rewasd_CONTROLLER_PROFILE_EX != null)
				{
					ExternalDeviceState externalDeviceState2 = this.GetExternalDeviceState(isConfigExternal, configVirtualGamepadType, config, gamepad, slot, externalDevice, externalClient, gamepadAuth, rewasd_CONTROLLER_PROFILE_EX);
					if (externalDeviceState2 == 5)
					{
						return externalDeviceState2;
					}
				}
			}
			return externalDeviceState;
		}

		public ExternalDeviceState GetExternalDeviceState(bool isConfigExternal, VirtualGamepadType? configVirtualGamepadType, Config config, BaseControllerVM gamepad, Slot? slot, ExternalDevice externalDevice, ExternalClient externalClient, GamepadAuth gamepadAuth, REWASD_CONTROLLER_PROFILE_EX profileEx = null)
		{
			if (externalDevice == null)
			{
				return 1;
			}
			if (externalDevice.IsOffline)
			{
				return 3;
			}
			if (externalDevice.DeviceType == null && BluetoothUtils.IsBluetoothAdapterIsNotSupported())
			{
				return 8;
			}
			if (isConfigExternal)
			{
				if (externalDevice.DeviceType == null || externalDevice.DeviceType == 2)
				{
					VirtualGamepadType? virtualGamepadType = configVirtualGamepadType;
					VirtualGamepadType virtualGamepadType2 = 0;
					if (!((virtualGamepadType.GetValueOrDefault() == virtualGamepadType2) & (virtualGamepadType != null)))
					{
						virtualGamepadType = configVirtualGamepadType;
						virtualGamepadType2 = 3;
						if (!((virtualGamepadType.GetValueOrDefault() == virtualGamepadType2) & (virtualGamepadType != null)))
						{
							goto IL_7A;
						}
					}
					return 9;
				}
				IL_7A:
				if (externalDevice.DeviceType == null)
				{
					VirtualGamepadType? virtualGamepadType = configVirtualGamepadType;
					VirtualGamepadType virtualGamepadType2 = 4;
					if (((virtualGamepadType.GetValueOrDefault() == virtualGamepadType2) & (virtualGamepadType != null)) && !this._isBluetoothAdapterIsSupportedForNintendoConsole)
					{
						return 9;
					}
				}
			}
			if (!externalDevice.IsOffline && !externalDevice.IsOnlineAndCorrect)
			{
				return 4;
			}
			if (externalClient != null && externalClient.IsConsole && externalClient != null && externalClient.IsSonyConsole)
			{
				if (externalDevice.DeviceType == null && BluetoothUtils.IsLmpSecureSimpleParingIsNotPresent())
				{
					return 7;
				}
				if (externalDevice.DeviceType == null || externalDevice.DeviceType == 2)
				{
					if (gamepadAuth == null || string.IsNullOrEmpty(gamepadAuth.ID))
					{
						return 6;
					}
					if (!this.GamepadService.SimpleDeviceInfoList.Exists((Tuple<ulong, SimpleDeviceInfo> x) => x.Item1.ToString() == gamepadAuth.ID))
					{
						return 10;
					}
				}
			}
			if (externalDevice.DeviceType == 1 || externalDevice.DeviceType == 3)
			{
				if (gamepadAuth != null && gamepadAuth.IsValid() && !this.GamepadService.SimpleDeviceInfoList.Exists((Tuple<ulong, SimpleDeviceInfo> x) => x.Item1.ToString() == gamepadAuth.ID))
				{
					return 10;
				}
				VirtualGamepadType? virtualGamepadType = configVirtualGamepadType;
				VirtualGamepadType virtualGamepadType2 = 1;
				if (((virtualGamepadType.GetValueOrDefault() == virtualGamepadType2) & (virtualGamepadType != null)) && (gamepadAuth == null || string.IsNullOrEmpty(gamepadAuth.ID)))
				{
					return 6;
				}
			}
			if (this.GamepadService.IsAnyServiceProfileWithExternalDevice(externalDevice.ExternalDeviceId) && !Convert.ToBoolean(RegistryHelper.GetValue("Config", "ExternalDeviceOverwritePrevConfig", 0, false)))
			{
				if (gamepad != null && this.CheckInUseWithOtherDevices(externalDevice.DeviceType, externalDevice.ExternalDeviceId, gamepad.Ids))
				{
					return 5;
				}
				if (profileEx != null && this._gameProfilesService != null && slot.Value < (ulong)((long)profileEx.Profiles.Length))
				{
					GamepadProfiles gamepadActiveProfiles = this.GamepadService.GetGamepadActiveProfiles(gamepad);
					GamepadProfile gamepadProfile = ((gamepadActiveProfiles != null) ? gamepadActiveProfiles.SlotProfiles.TryGetValue(slot.Value) : null);
					if (gamepadProfile != null && this.GamepadService.GetRemapState(gamepad.ID) == 1 && config != null && config.ConfigPath != null && !string.IsNullOrEmpty((config != null) ? config.ConfigPath : null) && gamepadProfile.ConfigPath != ((config != null) ? config.ConfigPath : null) && REWASD_CONTROLLER_PROFILE_Extensions.GetExternalDeviceId(profileEx.Profiles[slot.Value]) == externalDevice.ExternalDeviceId)
					{
						return 5;
					}
				}
			}
			return 2;
		}

		public ExternalState GetExternalState(bool isConfigExternal, VirtualGamepadType? configVirtualGamepadType, Config config, BaseControllerVM gamepad, Slot? slot, ExternalDevice externalDevice, ExternalClient externalClient, GamepadAuth gamepadAuth)
		{
			REWASD_CONTROLLER_PROFILE_EX rewasd_CONTROLLER_PROFILE_EX = null;
			if (externalDevice != null && this.GamepadService.IsAnyServiceProfileWithExternalDevice(externalDevice.DeviceType))
			{
				rewasd_CONTROLLER_PROFILE_EX = this.GamepadService.GetProfileEx((gamepad != null) ? gamepad.ID : null);
			}
			ExternalDeviceState externalDeviceState = this.GetExternalDeviceState(isConfigExternal, configVirtualGamepadType, config, gamepad, slot, externalDevice, externalClient, gamepadAuth, rewasd_CONTROLLER_PROFILE_EX);
			if (externalDeviceState != 2)
			{
				return this.ConvertExternalDeviceStateToExternalState(externalDeviceState);
			}
			if (gamepadAuth != null && gamepadAuth.IsValid() && !this.GamepadService.SimpleDeviceInfoList.Exists((Tuple<ulong, SimpleDeviceInfo> x) => x.Item1.ToString() == gamepadAuth.ID))
			{
				return 16;
			}
			if (gamepad != null && this.GamepadService.GetRemapState(gamepad.ID) == 1 && rewasd_CONTROLLER_PROFILE_EX != null && config != null && config.ConfigPath != null && !string.IsNullOrEmpty((config != null) ? config.ConfigPath : null))
			{
				GamepadProfiles gamepadActiveProfiles = this.GamepadService.GetGamepadActiveProfiles(gamepad);
				SlotProfilesDictionary slotProfilesDictionary = ((gamepadActiveProfiles != null) ? gamepadActiveProfiles.SlotProfiles : null);
				if (slotProfilesDictionary != null)
				{
					foreach (KeyValuePair<Slot, GamepadProfile> keyValuePair in ((IEnumerable<KeyValuePair<Slot, GamepadProfile>>)slotProfilesDictionary))
					{
						if (keyValuePair.Value.ConfigPath == ((config != null) ? config.ConfigPath : null) && rewasd_CONTROLLER_PROFILE_EX.Enabled[slot.Value])
						{
							REWASD_GET_PROFILE_STATE_RESPONSE rewasd_GET_PROFILE_STATE_RESPONSE = rewasd_CONTROLLER_PROFILE_EX.ProfilesState[slot.Value];
							if (rewasd_GET_PROFILE_STATE_RESPONSE.RemoteConnected)
							{
								return 9;
							}
							if (rewasd_CONTROLLER_PROFILE_EX.ExternalClientDisconnected[slot.Value])
							{
								return 7;
							}
							if (rewasd_GET_PROFILE_STATE_RESPONSE.BluetoothAuthenticationMethod != 0)
							{
								return 8;
							}
							return 6;
						}
					}
				}
			}
			if (externalClient == null || (externalDevice != null && externalDevice.DeviceType == 1) || (externalDevice != null && externalDevice.DeviceType == 3))
			{
				return 5;
			}
			if (!string.IsNullOrEmpty(externalClient.Alias))
			{
				return 3;
			}
			return 4;
		}

		public ExternalStateAgent GetExternalStateAgent()
		{
			if (!this.GamepadService.ServiceProfilesCollection.Any((Wrapper<REWASD_CONTROLLER_PROFILE_EX> x) => x.Value.IsAnyExternalDevicePresent()))
			{
				this.CurrentExternalStateAgentTooltip = null;
				return 0;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			List<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> serviceProfileWithExternalBluetoothAdapter = this.GamepadService.GetServiceProfileWithExternalBluetoothAdapter();
			List<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> serviceProfileWithExternalBluetooth = this.GamepadService.GetServiceProfileWithExternalBluetooth();
			List<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> serviceProfileWithExternalSerialPort = this.GamepadService.GetServiceProfileWithExternalSerialPort();
			if (serviceProfileWithExternalBluetoothAdapter.Count > 0 && !BluetoothUtils.CanUseBluetoothInRewasd)
			{
				this.CurrentExternalStateAgentTooltip = DTLocalization.GetString(11995);
				return 2;
			}
			if (serviceProfileWithExternalBluetooth.Count > 0)
			{
				foreach (Wrapper<REWASD_CONTROLLER_PROFILE_EX> wrapper in serviceProfileWithExternalBluetooth)
				{
					int num = 0;
					REWASD_CONTROLLER_PROFILE[] array = wrapper.Value.Profiles;
					for (int i = 0; i < array.Length; i++)
					{
						REWASD_CONTROLLER_PROFILE bluetoothProfile = array[i];
						if (num >= wrapper.Value.Enabled.Length || num >= wrapper.Value.ProfilesState.Length)
						{
							break;
						}
						if (REWASD_CONTROLLER_PROFILE_Extensions.IsExternalDeviceWithBluetoothPresent(bluetoothProfile) && wrapper.Value.Enabled[num])
						{
							if (bluetoothProfile.AuthControllerId != 0UL && !this.GamepadService.SimpleDeviceInfoList.Exists((Tuple<ulong, SimpleDeviceInfo> x) => x.Item1 == bluetoothProfile.AuthControllerId))
							{
								flag = true;
							}
							if (!wrapper.Value.ProfilesState[num].RemoteConnected)
							{
								flag2 = true;
								flag3 = wrapper.Value.ExternalClientDisconnected[num];
							}
						}
						num++;
					}
				}
			}
			if (serviceProfileWithExternalSerialPort.Count > 0)
			{
				List<string> list = SerialPort.GetPortNames().ToList<string>();
				foreach (Wrapper<REWASD_CONTROLLER_PROFILE_EX> wrapper2 in serviceProfileWithExternalSerialPort)
				{
					int num2 = 0;
					REWASD_CONTROLLER_PROFILE[] array = wrapper2.Value.Profiles;
					for (int i = 0; i < array.Length; i++)
					{
						REWASD_CONTROLLER_PROFILE gimxProfile = array[i];
						if (num2 >= wrapper2.Value.Enabled.Length || num2 >= wrapper2.Value.ProfilesState.Length)
						{
							break;
						}
						if (REWASD_CONTROLLER_PROFILE_Extensions.IsExternalDeviceWithSerialPortPresent(gimxProfile) && wrapper2.Value.Enabled[num2])
						{
							if (!list.Exists((string x) => string.Equals(x, new string(gimxProfile.GimxPortName).Trim('\0').ToUpper(), StringComparison.CurrentCultureIgnoreCase)))
							{
								this.CurrentExternalStateAgentTooltip = DTLocalization.GetString(11995);
								return 2;
							}
							if (gimxProfile.AuthControllerId != 0UL && !this.GamepadService.SimpleDeviceInfoList.Exists((Tuple<ulong, SimpleDeviceInfo> x) => x.Item1 == gimxProfile.AuthControllerId))
							{
								flag = true;
							}
							if (!wrapper2.Value.ProfilesState[num2].RemoteConnected)
							{
								flag2 = true;
								flag3 = wrapper2.Value.ExternalClientDisconnected[num2];
							}
						}
						num2++;
					}
				}
			}
			if (flag)
			{
				this.CurrentExternalStateAgentTooltip = DTLocalization.GetString(11997);
				return 2;
			}
			if (flag2)
			{
				this.CurrentExternalStateAgentTooltip = (flag3 ? DTLocalization.GetString(12084) : DTLocalization.GetString(12001));
				return 2;
			}
			this.CurrentExternalStateAgentTooltip = null;
			return 1;
		}

		public ExternalDeviceRelation GetRelationForCurrentGamepadAndConfig(Config config, BaseControllerVM gamepad)
		{
			if (gamepad == null || gamepad.ID == null || config == null || config.Name == null || config == null || config.Name == null)
			{
				return null;
			}
			if (!string.IsNullOrEmpty(config.Name) && !string.IsNullOrEmpty((config != null) ? config.Name : null) && !string.IsNullOrEmpty(gamepad.ID))
			{
				return this.GetRelation(config.Name, (config != null) ? config.Name : null, gamepad.Ids);
			}
			return null;
		}

		public ExternalDeviceRelation GetRelation(string gameName, string configName, ulong[] controllerIds)
		{
			if (controllerIds == null || this.ExternalDeviceRelationsCollection == null)
			{
				return null;
			}
			if (controllerIds.Length != 0)
			{
				if (!controllerIds.All((ulong x) => x == 0UL))
				{
					ulong[] compareIdsArray = new List<ulong>(controllerIds).Where((ulong x) => x > 0UL).ToArray<ulong>();
					return this.ExternalDeviceRelationsCollection.FirstOrDefault((ExternalDeviceRelation item) => item.GameName == gameName && item.ConfigName == configName && item.ControllerIds != null && item.ControllerIds.Length == compareIdsArray.Length && !item.ControllerIds.Except(compareIdsArray).Any<ulong>() && !compareIdsArray.Except(item.ControllerIds).Any<ulong>());
				}
			}
			return null;
		}

		public bool AddAndSaveRelation(Config config, BaseControllerVM gamepad, ExternalDevice externalDevice, ExternalClient externalClient, GamepadAuth authGamepad)
		{
			if (externalDevice == null || gamepad == null || Engine.GameProfilesService == null || config == null)
			{
				return false;
			}
			string gameName = config.GameName;
			string configName = config.Name;
			string controllerId = gamepad.ID;
			string controllerDisplayName = gamepad.ControllerDisplayName;
			if (string.IsNullOrEmpty(controllerId) || string.IsNullOrEmpty(gameName) || string.IsNullOrEmpty(configName) || string.IsNullOrEmpty(externalDevice.ExternalDeviceId) || externalDevice.IsDummy)
			{
				return false;
			}
			if ((externalDevice.DeviceType == null || externalDevice.DeviceType == 2) && (externalClient == null || externalClient.Dummy))
			{
				return false;
			}
			if (authGamepad != null && !authGamepad.IsValid())
			{
				return false;
			}
			ExternalDeviceRelation externalDeviceRelation = this.ExternalDeviceRelationsCollection.FirstOrDefault((ExternalDeviceRelation x) => x.GameName == gameName && x.ConfigName == configName && x.ControllerId == controllerId);
			if (externalDeviceRelation == null)
			{
				this.ExternalDeviceRelationsCollection.Add(new ExternalDeviceRelation(controllerId, controllerDisplayName, gameName, configName, externalDevice, externalClient, authGamepad));
			}
			else
			{
				externalDeviceRelation.ExternalDevice = externalDevice.Clone();
				ExternalDeviceRelation externalDeviceRelation2 = externalDeviceRelation;
				ExternalClient externalClient2;
				if (externalClient == null)
				{
					(externalClient2 = new ExternalClient()).Dummy = true;
				}
				else
				{
					externalClient2 = externalClient.Clone();
				}
				externalDeviceRelation2.ExternalClient = externalClient2;
				externalDeviceRelation.AuthGamepad = ((authGamepad != null) ? authGamepad.Clone() : new GamepadAuth("", "", 0));
			}
			bool flag = this.SaveAndReloadRelations();
			this.Refresh();
			return flag;
		}

		public bool SaveRelations()
		{
			ExternalDeviceRelationsCollection externalDeviceRelationsCollection = new ExternalDeviceRelationsCollection();
			this.ExternalDeviceRelationsCollection.CopyToModel(externalDeviceRelationsCollection);
			return externalDeviceRelationsCollection.Serialize(BinDataSerialize.USER_IMPERSONATED_EXTERNAL_DEVICES_RELATIONS_COLLECTION_FULL_SAVE_FILE_PATH);
		}

		public bool SaveAndReloadRelations()
		{
			return this.SaveRelations() && this.LoadRelations();
		}

		public bool LoadRelations()
		{
			ExternalDeviceRelationsCollection externalDeviceRelationsCollection = new ExternalDeviceRelationsCollection();
			if (externalDeviceRelationsCollection.Deserialize(BinDataSerialize.USER_IMPERSONATED_EXTERNAL_DEVICES_RELATIONS_COLLECTION_FULL_SAVE_FILE_PATH))
			{
				this.ExternalDeviceRelationsCollection.Clear();
				this.ExternalDeviceRelationsCollection.CopyFromModel(externalDeviceRelationsCollection);
				return true;
			}
			return false;
		}

		public bool RemoveGameRelations(string gameName)
		{
			if (string.IsNullOrEmpty(gameName))
			{
				return false;
			}
			this.ExternalDeviceRelationsCollection.Remove((ExternalDeviceRelation x) => x.GameName == gameName);
			return this.SaveAndReloadRelations();
		}

		public bool RemoveConfigRelations(string gameName, string configName)
		{
			if (string.IsNullOrEmpty(gameName) || string.IsNullOrEmpty(configName))
			{
				return false;
			}
			this.ExternalDeviceRelationsCollection.Remove((ExternalDeviceRelation x) => x.GameName == gameName && x.ConfigName == configName);
			return this.SaveAndReloadRelations();
		}

		public bool RemoveExternalDeviceRelations(string externalDeviceId)
		{
			if (string.IsNullOrEmpty(externalDeviceId))
			{
				return false;
			}
			this.ExternalDeviceRelationsCollection.Remove((ExternalDeviceRelation x) => x.ExternalDeviceId == externalDeviceId);
			return this.SaveAndReloadRelations();
		}

		public bool RemoveExternalClientRelations(ulong macAddress)
		{
			if (macAddress == 0UL)
			{
				return false;
			}
			this.ExternalDeviceRelationsCollection.Remove(delegate(ExternalDeviceRelation x)
			{
				ExternalClient externalClient = x.ExternalClient;
				return externalClient != null && externalClient.MacAddress == macAddress;
			});
			return this.SaveAndReloadRelations();
		}

		public bool RenameGameRelations(string oldGameName, string newGameName)
		{
			if (string.IsNullOrEmpty(oldGameName) || string.IsNullOrEmpty(newGameName))
			{
				return false;
			}
			foreach (ExternalDeviceRelation externalDeviceRelation in this.ExternalDeviceRelationsCollection)
			{
				if (externalDeviceRelation.GameName == oldGameName)
				{
					externalDeviceRelation.GameName = newGameName;
				}
			}
			return this.SaveAndReloadRelations();
		}

		public bool RenameConfigRelations(string gameName, string oldConfigName, string newConfigName)
		{
			if (string.IsNullOrEmpty(gameName) || string.IsNullOrEmpty(oldConfigName) || string.IsNullOrEmpty(newConfigName))
			{
				return false;
			}
			foreach (ExternalDeviceRelation externalDeviceRelation in this.ExternalDeviceRelationsCollection)
			{
				if (externalDeviceRelation.GameName == gameName && externalDeviceRelation.ConfigName == oldConfigName)
				{
					externalDeviceRelation.ConfigName = newConfigName;
				}
			}
			return this.SaveAndReloadRelations();
		}

		public bool IsRelationValidForApply(ExternalDeviceRelation relation)
		{
			if (((relation != null) ? relation.ExternalDevice : null) == null)
			{
				Tracer.TraceWrite("IsRelationValidForApply: Error! ExternalDevice is NULL", false);
				return false;
			}
			if (relation.ExternalDevice.IsDummy)
			{
				Tracer.TraceWrite("IsRelationValidForApply: Error! ExternalDevice invalid", false);
				return false;
			}
			if (relation.ExternalDevice.DeviceType == null || relation.ExternalDevice.DeviceType == 2)
			{
				if (relation.ExternalClient == null)
				{
					Tracer.TraceWrite("IsRelationValidForApply: Bluetooth: Error! ExternalClient is NULL", false);
					return false;
				}
				if (relation.ExternalClient.MacAddress == 0UL)
				{
					Tracer.TraceWrite("IsRelationValidForApply: Bluetooth: Error! ExternalClient.MacAddress is zero", false);
					return false;
				}
			}
			if ((relation.ExternalDevice.DeviceType == 1 || relation.ExternalDevice.DeviceType == 2 || relation.ExternalDevice.DeviceType == 3) && relation.ExternalDevice.SerialPort.Length > 16)
			{
				Tracer.TraceWrite("IsRelationValidForApply: Error! SerialPort lenght is too long", false);
				return false;
			}
			if (!relation.ExternalDevice.IsOnlineAndCorrect)
			{
				Tracer.TraceWrite("IsRelationValidForApply: Error! ExternalDevice is NOT ready for Apply", false);
				if (relation.ExternalDevice.DeviceType == null)
				{
					Tracer.TraceWrite("------", false);
					Tracer.TraceWrite("IsRelationValidForApply: Error! Bluetooth:", false);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 1);
					defaultInterpolatedStringHandler.AppendLiteral("   IsBluetoothExist ");
					defaultInterpolatedStringHandler.AppendFormatted<bool>(BluetoothUtils.IsBluetoothExist());
					Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 1);
					defaultInterpolatedStringHandler.AppendLiteral("   NeedInitForRewasd ");
					defaultInterpolatedStringHandler.AppendFormatted<bool>(BluetoothUtils.NeedInitForRewasd());
					Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 1);
					defaultInterpolatedStringHandler.AppendLiteral("   IsBluetoothLocalRadioInfoInvalidCod ");
					defaultInterpolatedStringHandler.AppendFormatted<bool>(BluetoothUtils.IsBluetoothLocalRadioInfoInvalidCod(false));
					Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 1);
					defaultInterpolatedStringHandler.AppendLiteral("   IsBluetoothAdapterIsNotSupported ");
					defaultInterpolatedStringHandler.AppendFormatted<bool>(BluetoothUtils.IsBluetoothAdapterIsNotSupported());
					Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
					Tracer.TraceWrite("------", false);
				}
				return false;
			}
			return true;
		}

		public ExternalState ConvertExternalDeviceStateToExternalState(ExternalDeviceState externalDeviceState)
		{
			switch (externalDeviceState)
			{
			case 1:
				return 1;
			case 3:
			case 4:
				return 2;
			case 5:
				return 11;
			case 6:
				return 12;
			case 7:
				return 13;
			case 8:
				return 14;
			case 9:
				return 15;
			case 10:
				return 16;
			}
			return 0;
		}

		public string GetExternalText(ExternalState externalState)
		{
			if (externalState == 9)
			{
				ExternalDevice currentExternalDevice = this.CurrentExternalDevice;
				if (currentExternalDevice == null || currentExternalDevice.DeviceType != 0)
				{
					ExternalDevice currentExternalDevice2 = this.CurrentExternalDevice;
					if (currentExternalDevice2 == null || currentExternalDevice2.DeviceType != 2)
					{
						goto IL_68;
					}
				}
				ExternalClient currentExternalClient = this.CurrentExternalClient;
				if (!string.IsNullOrEmpty((currentExternalClient != null) ? currentExternalClient.Alias : null))
				{
					return string.Format(externalState.TryGetLocalizedDescription(), this.CurrentExternalClient.Alias);
				}
				IL_68:
				return 10.TryGetLocalizedDescription();
			}
			if (externalState == 7)
			{
				ExternalDevice currentExternalDevice3 = this.CurrentExternalDevice;
				if (currentExternalDevice3 == null || currentExternalDevice3.DeviceType != 0)
				{
					ExternalDevice currentExternalDevice4 = this.CurrentExternalDevice;
					if (currentExternalDevice4 == null || currentExternalDevice4.DeviceType != 2)
					{
						goto IL_DB;
					}
				}
				ExternalClient currentExternalClient2 = this.CurrentExternalClient;
				if (!string.IsNullOrEmpty((currentExternalClient2 != null) ? currentExternalClient2.Alias : null))
				{
					return string.Format(DTLocalization.GetString(12083), this.CurrentExternalClient.Alias);
				}
				IL_DB:
				return DTLocalization.GetString(12084);
			}
			if (externalState == 3)
			{
				ExternalDevice currentExternalDevice5 = this.CurrentExternalDevice;
				if (currentExternalDevice5 == null || currentExternalDevice5.DeviceType != 0)
				{
					ExternalDevice currentExternalDevice6 = this.CurrentExternalDevice;
					if (currentExternalDevice6 == null || currentExternalDevice6.DeviceType != 2)
					{
						goto IL_14D;
					}
				}
				if (this.CurrentExternalClient != null && !string.IsNullOrEmpty(this.CurrentExternalClient.Alias))
				{
					return string.Format(DTLocalization.GetString(12005), this.CurrentExternalClient.Alias);
				}
			}
			IL_14D:
			return externalState.TryGetLocalizedDescription();
		}

		public string GetExternalStateTooltip(ExternalState externalState)
		{
			switch (externalState)
			{
			case 1:
				return DTLocalization.GetString(12006);
			case 2:
				if (this.CurrentExternalDevice != null)
				{
					ExternalDeviceType deviceType = this.CurrentExternalDevice.DeviceType;
					if (deviceType != null)
					{
						if (deviceType - 1 <= 2)
						{
							if (this.CurrentExternalDevice.IsOffline)
							{
								return DTLocalization.GetString(12011);
							}
						}
					}
					else
					{
						if (this.CurrentExternalDevice.IsOffline)
						{
							return DTLocalization.GetString(12010);
						}
						if (BluetoothUtils.IsBluetoothAdapterIsNotSupported())
						{
							return DTLocalization.GetString(11999);
						}
						if (BluetoothUtils.NeedInitForRewasd())
						{
							return DTLocalization.GetString(12008);
						}
						if (BluetoothUtils.IsRebootRequired())
						{
							return DTLocalization.GetString(12009);
						}
					}
				}
				break;
			case 3:
			case 4:
			case 5:
				return DTLocalization.GetString(12007);
			default:
				if (externalState == 13)
				{
					if (this.CurrentExternalDevice != null && this.CurrentExternalDevice.DeviceType == null && BluetoothUtils.IsLmpSecureSimpleParingIsNotPresent())
					{
						return DTLocalization.GetString(12057);
					}
				}
				break;
			}
			return null;
		}

		public bool CheckInUseWithOtherDevices(ExternalDeviceType externalDeviceType, string externalDeviceId, ulong[] currentControllerIds)
		{
			if (currentControllerIds == null || currentControllerIds.Length == 0)
			{
				return false;
			}
			List<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> serviceProfileWithExternalDevice = this.GamepadService.GetServiceProfileWithExternalDevice(externalDeviceType);
			Func<REWASD_CONTROLLER_PROFILE, bool> <>9__2;
			Func<ulong, bool> <>9__4;
			return serviceProfileWithExternalDevice != null && serviceProfileWithExternalDevice.Where((Wrapper<REWASD_CONTROLLER_PROFILE_EX> profileEx) => profileEx.Value.IsExternalDevicePresent(externalDeviceType)).Any(delegate(Wrapper<REWASD_CONTROLLER_PROFILE_EX> profileEx)
			{
				IEnumerable<REWASD_CONTROLLER_PROFILE> profiles = profileEx.Value.Profiles;
				Func<REWASD_CONTROLLER_PROFILE, bool> func;
				if ((func = <>9__2) == null)
				{
					func = (<>9__2 = (REWASD_CONTROLLER_PROFILE profile) => REWASD_CONTROLLER_PROFILE_Extensions.IsExternalDeviceEqual(profile, externalDeviceId));
				}
				IEnumerable<ulong> enumerable = profiles.Where(func).SelectMany((REWASD_CONTROLLER_PROFILE profile) => profile.Id.Where((ulong x) => x > 0UL).ToList<ulong>());
				Func<ulong, bool> func2;
				if ((func2 = <>9__4) == null)
				{
					func2 = (<>9__4 = (ulong id) => !currentControllerIds.Contains(id));
				}
				return enumerable.Any(func2);
			});
		}

		public async void RefreshDevice(REWASD_CONTROLLER_PROFILE_EX profileEx)
		{
			foreach (object obj in Enum.GetValues(typeof(Slot)))
			{
				Slot slot = (Slot)obj;
				if (profileEx.ServiceProfileIds[slot] != 0 && profileEx.Enabled[slot] && REWASD_CONTROLLER_PROFILE_Extensions.IsExternalDeviceWithBluetoothPresent(profileEx.Profiles[slot]) && profileEx.Profiles[slot].VirtualType == 48U && Convert.ToBoolean((uint)(profileEx.Profiles[slot].VirtualFlags & 64)))
				{
					if (profileEx.ExternalClientDisconnected[slot] && profileEx.ExternalClientReconnectTryCounter[slot] < 5)
					{
						profileEx.ExternalClientReconnectTryCounter[slot]++;
						await this.ExternalDeviceBluetoothReconnect(profileEx, slot);
					}
					if (profileEx.ProfilesState[slot].RemoteConnected)
					{
						profileEx.ExternalClientReconnectTryCounter[slot] = 0;
					}
				}
			}
			IEnumerator enumerator = null;
			this.Refresh();
		}

		public void RefreshComPorts()
		{
			Engine.GamepadService.ExternalDevices.ForEach(delegate(ExternalDevice device)
			{
				device.RefreshComPorts();
			});
		}

		public void Refresh()
		{
			this.InitVariables();
			this.CurrentExternalStateAgent = this.GetExternalStateAgent();
			Engine.EventAggregator.GetEvent<ExternalHelperChanged>().Publish(null);
		}

		public void OnVirtualGamepadTypeChanged()
		{
			this.Refresh();
		}

		public async void AddGamepadToGamepadsAuthCollection(string id, string controllerDisplayName, ControllerTypeEnum controllerType)
		{
			using (await new AsyncLock(ExternalDeviceRelationsHelper._refreshGamepadAuthSemaphore).LockAsync())
			{
				if (this.GamepadsAuth.FirstOrDefault((GamepadAuth item) => item.ID == id) == null)
				{
					this.GamepadsAuth.Add(new GamepadAuth(id, controllerDisplayName, controllerType));
				}
				if (this.CurrentGamepadsAuthCollection.FirstOrDefault((GamepadAuth item) => item.ID == id) == null)
				{
					if (this._latestExternalClientFilter == null || (this._latestExternalClientFilter != null && !this._latestExternalClientFilter.IsConsole))
					{
						this.CurrentGamepadsAuthCollection.Add(new GamepadAuth(id, controllerDisplayName, controllerType));
					}
					else
					{
						ConsoleType consoleType = this._latestExternalClientFilter.ConsoleType;
						if (consoleType != 1)
						{
							if (consoleType == 3)
							{
								if (ControllerTypeExtensions.IsXboxEliteOrOne(controllerType))
								{
									this.CurrentGamepadsAuthCollection.Add(new GamepadAuth(id, controllerDisplayName, controllerType));
								}
							}
						}
						else if (ControllerTypeExtensions.IsSonyDS4Auth(controllerType))
						{
							this.CurrentGamepadsAuthCollection.Add(new GamepadAuth(id, controllerDisplayName, controllerType));
						}
					}
				}
			}
			this.CurrentGamepadsAuthCollectionRefreshOfflineStatus();
		}

		public async void RemoveGamepadFromGamepadsAuthCollection(string id)
		{
			using (await new AsyncLock(ExternalDeviceRelationsHelper._refreshGamepadAuthSemaphore).LockAsync())
			{
				if (this.GamepadsAuth.Any((GamepadAuth item) => item.ID == id))
				{
					GamepadAuth gamepadAuth = this.GamepadsAuth.FirstOrDefault((GamepadAuth x) => x.ID == id);
					if (gamepadAuth != null)
					{
						this.GamepadsAuth.Remove(gamepadAuth);
					}
				}
				if (this.CurrentGamepadsAuthCollection.Any((GamepadAuth item) => item.ID == id))
				{
					GamepadAuth gamepadAuth2 = this.CurrentGamepadsAuthCollection.FirstOrDefault((GamepadAuth x) => x.ID == id);
					if (gamepadAuth2 != null)
					{
						GamepadAuth currentAuthGamepad = this.CurrentAuthGamepad;
						if (((currentAuthGamepad != null) ? currentAuthGamepad.ID : null) != id)
						{
							this.CurrentGamepadsAuthCollection.Remove(gamepadAuth2);
						}
					}
				}
			}
			this.CurrentGamepadsAuthCollectionRefreshOfflineStatus();
		}

		public async void RecreateCurrentGamepadsAuthList(ExternalClient client)
		{
			using (await new AsyncLock(ExternalDeviceRelationsHelper._refreshGamepadAuthSemaphore).LockAsync())
			{
				this._latestExternalClientFilter = client;
			}
			this.CurrentGamepadsAuthCollectionRefreshOfflineStatus();
		}

		public async void CurrentGamepadsAuthCollectionRefreshOfflineStatus()
		{
			using (await new AsyncLock(ExternalDeviceRelationsHelper._refreshGamepadAuthSemaphore).LockAsync())
			{
				using (IEnumerator<GamepadAuth> enumerator = this.CurrentGamepadsAuthCollection.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						GamepadAuth gamepadAuth = enumerator.Current;
						gamepadAuth.IsOffline = !this.GamepadService.SimpleDeviceInfoList.Exists((Tuple<ulong, SimpleDeviceInfo> x) => x.Item1.ToString() == gamepadAuth.ID);
					}
				}
			}
		}

		public async Task ExternalDeviceBluetoothReconnect(REWASD_CONTROLLER_PROFILE_EX profileEx, Slot _slot)
		{
			if (profileEx.ServiceProfileIds[_slot] != 0 && profileEx.Enabled[_slot] && !profileEx.ProfilesState[_slot].RemoteConnected && profileEx.IsExternalBluetoothPresent())
			{
				if (profileEx.ExternalClientDisconnected[_slot])
				{
					profileEx.ExternalClientDisconnected[_slot] = false;
				}
				REWASD_SET_PROFILE_INFO[] array = new REWASD_SET_PROFILE_INFO[] { REWASD_SET_PROFILE_INFO.CreateBlankInstance() };
				array[0].ProfileId = profileEx.ServiceProfileIds[_slot];
				REWASD_SET_PROFILE_INFO[] array2 = array;
				int num = 0;
				array2[num].Flags = array2[num].Flags | 1U;
				REWASD_SET_PROFILE_INFO[] array3 = array;
				int num2 = 0;
				array3[num2].Flags = array3[num2].Flags | 32U;
				REWASD_SET_PROFILE_INFO[] array4 = array;
				int num3 = 0;
				array4[num3].Flags = array4[num3].Flags | 64U;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(46, 1);
				defaultInterpolatedStringHandler.AppendLiteral("ExternalDeviceBluetoothReconnect: ProfileId 0x");
				defaultInterpolatedStringHandler.AppendFormatted<ushort>(array[0].ProfileId, "X");
				Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
				await Engine.XBServiceCommunicator.SetProfileState(array, false);
			}
		}

		public async Task ExternalDeviceBluetoothConnectOrDisconnect(ushort serviceProfileId, bool connectState)
		{
			if (serviceProfileId != 0)
			{
				REWASD_SET_PROFILE_INFO[] array = new REWASD_SET_PROFILE_INFO[] { REWASD_SET_PROFILE_INFO.CreateBlankInstance() };
				array[0].ProfileId = serviceProfileId;
				REWASD_SET_PROFILE_INFO[] array2 = array;
				int num = 0;
				array2[num].Flags = array2[num].Flags | 1U;
				REWASD_SET_PROFILE_INFO[] array3 = array;
				int num2 = 0;
				array3[num2].Flags = array3[num2].Flags | 32U;
				if (connectState)
				{
					REWASD_SET_PROFILE_INFO[] array4 = array;
					int num3 = 0;
					array4[num3].Flags = array4[num3].Flags | 64U;
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(47, 1);
				defaultInterpolatedStringHandler.AppendLiteral("ExternalDeviceBluetoothDisconnect: ProfileId 0x");
				defaultInterpolatedStringHandler.AppendFormatted<ushort>(array[0].ProfileId, "X");
				Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
				await Engine.XBServiceCommunicator.SetProfileState(array, false);
			}
		}

		public async Task ExternalDeviceBluetoothReconnect(string gamepadId, Slot _slot)
		{
			if (this.GamepadService.GetRemapState(gamepadId) == 1)
			{
				uint slotsWrapperId = 0U;
				REWASD_CONTROLLER_PROFILE_EX profileEx = this.GamepadService.GetProfileEx(gamepadId);
				if (profileEx != null)
				{
					slotsWrapperId = profileEx.SlotsWrapperId;
				}
				if (slotsWrapperId != 0U)
				{
					Wrapper<REWASD_CONTROLLER_PROFILE_EX> wrapper = this.GamepadService.ServiceProfilesCollection.FirstOrDefault((Wrapper<REWASD_CONTROLLER_PROFILE_EX> sp) => sp.Value.SlotsWrapperId == slotsWrapperId);
					if (wrapper != null)
					{
						await this.ExternalDeviceBluetoothReconnect(wrapper.Value, _slot);
					}
				}
			}
		}

		public async Task ExternalDeviceDisableRemapForSerialPort(string serialPort)
		{
			if (!string.IsNullOrEmpty(serialPort))
			{
				List<Wrapper<REWASD_CONTROLLER_PROFILE_EX>> serviceProfileWithExternalSerialPort = this.GamepadService.GetServiceProfileWithExternalSerialPort();
				if (serviceProfileWithExternalSerialPort != null && serviceProfileWithExternalSerialPort.Count > 0)
				{
					Func<REWASD_CONTROLLER_PROFILE, bool> <>9__0;
					foreach (Wrapper<REWASD_CONTROLLER_PROFILE_EX> wrapper in serviceProfileWithExternalSerialPort)
					{
						IEnumerable<REWASD_CONTROLLER_PROFILE> profiles = wrapper.Value.Profiles;
						Func<REWASD_CONTROLLER_PROFILE, bool> func;
						if ((func = <>9__0) == null)
						{
							func = (<>9__0 = (REWASD_CONTROLLER_PROFILE x) => REWASD_CONTROLLER_PROFILE_Extensions.IsExternalDeviceWithSerialPortPresent(x) && REWASD_CONTROLLER_PROFILE_Extensions.GetPortName(x) == serialPort.ToUpper());
						}
						if (profiles.Any(func))
						{
							Tracer.TraceWrite("ExternalDeviceDisableRemapForSerialPort: DisableRemap for controllerID: " + wrapper.Value.GetID(null), false);
							await Engine.XBServiceCommunicator.DeleteProfiles(wrapper.Value.GetAllProfileIds());
						}
					}
					List<Wrapper<REWASD_CONTROLLER_PROFILE_EX>>.Enumerator enumerator = default(List<Wrapper<REWASD_CONTROLLER_PROFILE_EX>>.Enumerator);
				}
			}
		}

		private ExternalDeviceRelationsCollection _externalDeviceRelationsCollection;

		private ExternalDeviceRelation _currentExternalDeviceRelation;

		private ExternalDevice _currentExternalDevice;

		private IGameProfilesService _gameProfilesService;

		private ObservableCollection<GamepadAuth> _gamepadsAuth;

		private SortableObservableCollection<GamepadAuth> _currentGamepadsAuthCollection;

		private bool _isBluetoothAdapterIsSupportedForNintendoConsole;

		private static readonly AsyncSemaphore _refreshGamepadAuthSemaphore = new AsyncSemaphore(1);

		private ExternalStateAgent _currentExternalStateAgent;

		private string _currentExternalStateAgentTooltip;

		private ExternalClient _latestExternalClientFilter;

		public delegate void StateChangedDelegate();
	}
}
