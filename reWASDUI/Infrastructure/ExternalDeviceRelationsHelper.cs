using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Properties;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using reWASDCommon.Utils;
using reWASDUI.DataModels;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Services;
using reWASDUI.Services.Interfaces;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.ExternalDeviceRelations;
using XBEliteWPF.Infrastructure.ExternalDeviceRelationsModel;
using XBEliteWPF.Infrastructure.ExternalDevices;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.ViewModels.Base;

namespace reWASDUI.Infrastructure
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

		public ExternalDeviceState CurrentExternalDeviceState { get; set; }

		public ExternalState CurrentExternalState { get; set; }

		public bool CurrentExternalStateActive
		{
			get
			{
				return this.CurrentExternalState == 6 || this.CurrentExternalState == 10 || this.CurrentExternalState == 9 || this.CurrentExternalState == 8 || this.CurrentExternalState == 7;
			}
		}

		public bool CurrentExternalStateReconnectVisible
		{
			get
			{
				ExternalDevice currentExternalDevice = this.CurrentExternalDevice;
				if (currentExternalDevice == null || currentExternalDevice.DeviceType != 0)
				{
					ExternalDevice currentExternalDevice2 = this.CurrentExternalDevice;
					if (currentExternalDevice2 == null || currentExternalDevice2.DeviceType != 2)
					{
						return false;
					}
				}
				return this.CurrentExternalState == 6 || this.CurrentExternalState == 7 || this.CurrentExternalState == 8;
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

		public string CurrentExternalText
		{
			get
			{
				return this.GetExternalText(this.CurrentExternalState);
			}
		}

		public string CurrentExternalStateTooltip
		{
			get
			{
				return this.GetExternalStateTooltip(this.CurrentExternalState);
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

		public bool IsCurrentExternalDeviceGIMXOrESP32S2
		{
			get
			{
				return this.IsCurrentExternalDeviceGIMX || this.IsCurrentExternalDeviceESP32S2;
			}
		}

		public async Task RefreshProperties()
		{
			ExternalState externalState = await this.GetExternalState(this.CurrentExternalDevice, this.CurrentExternalClient, this.CurrentAuthGamepad);
			this.CurrentExternalState = externalState;
			this.CurrentExternalDeviceState = await this.GetExternalDeviceState(this.CurrentExternalDevice, this.CurrentExternalClient, this.CurrentAuthGamepad);
			this.OnPropertyChanged("CurrentExternalDevice");
			this.OnPropertyChanged("CurrentExternalClient");
			this.OnPropertyChanged("CurrentAuthGamepad");
			this.OnPropertyChanged("IsCurrentExternalDeviceBluetooth");
			this.OnPropertyChanged("IsCurrentExternalDeviceGIMX");
			this.OnPropertyChanged("IsCurrentExternalDeviceESP32");
			this.OnPropertyChanged("IsCurrentExternalDeviceESP32S2");
			this.OnPropertyChanged("IsCurrentExternalDeviceBluetoothOrESP32");
			this.OnPropertyChanged("IsCurrentExternalDeviceGIMXOrESP32S2");
			this.OnPropertyChanged("CurrentExternalDeviceState");
			this.OnPropertyChanged("CurrentExternalState");
			this.OnPropertyChanged("CurrentExternalStateActive");
			this.OnPropertyChanged("CurrentExternalText");
			this.OnPropertyChanged("CurrentExternalStateTooltip");
			this.OnPropertyChanged("CurrentExternalStateReconnectVisible");
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
				if (this.SetProperty<ExternalDevice>(ref this._currentExternalDevice, value, "CurrentExternalDevice"))
				{
					this.RefreshProperties();
				}
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
			App.EventAggregator.GetEvent<VirtualControllerTypeChanged>().Subscribe(delegate(ConfigData config)
			{
				if (config.IsExternal)
				{
					ConfigVM configVM = config.ConfigVM;
					if (configVM == null)
					{
						return;
					}
					Lazy<IGamepadService> gamepadServiceLazy = configVM.GamepadServiceLazy;
					if (gamepadServiceLazy == null)
					{
						return;
					}
					IGamepadService value = gamepadServiceLazy.Value;
					if (value == null)
					{
						return;
					}
					ExternalDeviceRelationsHelper externalDeviceRelationsHelper = value.ExternalDeviceRelationsHelper;
					if (externalDeviceRelationsHelper == null)
					{
						return;
					}
					externalDeviceRelationsHelper.OnVirtualGamepadTypeChanged();
				}
			});
			App.EventAggregator.GetEvent<ExternalHelperChanged>().Subscribe(delegate(object obj)
			{
				this.Refresh();
			});
			this.LoadRelations();
		}

		private GetExternalStateInfo CreateGetExternalStateInfo(ExternalDevice externalDevice, ExternalClient externalClient, GamepadAuth gamepadAuth)
		{
			GetExternalStateInfo getExternalStateInfo = new GetExternalStateInfo();
			GameVM currentGame = this._gameProfilesService.CurrentGame;
			getExternalStateInfo.GameName = ((currentGame != null) ? currentGame.Name : null);
			GameVM currentGame2 = this._gameProfilesService.CurrentGame;
			string text;
			if (currentGame2 == null)
			{
				text = null;
			}
			else
			{
				ConfigVM currentConfig = currentGame2.CurrentConfig;
				text = ((currentConfig != null) ? currentConfig.Name : null);
			}
			getExternalStateInfo.ConfigName = text;
			GameVM currentGame3 = this._gameProfilesService.CurrentGame;
			bool flag;
			if (currentGame3 == null)
			{
				flag = false;
			}
			else
			{
				ConfigVM currentConfig2 = currentGame3.CurrentConfig;
				bool? flag2;
				if (currentConfig2 == null)
				{
					flag2 = null;
				}
				else
				{
					ConfigData configData = currentConfig2.ConfigData;
					flag2 = ((configData != null) ? new bool?(configData.IsExternal) : null);
				}
				bool? flag3 = flag2;
				bool flag4 = true;
				flag = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
			}
			getExternalStateInfo.IsConfigExternal = flag;
			GameVM currentGame4 = this._gameProfilesService.CurrentGame;
			VirtualGamepadType? virtualGamepadType;
			if (currentGame4 == null)
			{
				virtualGamepadType = null;
			}
			else
			{
				ConfigVM currentConfig3 = currentGame4.CurrentConfig;
				if (currentConfig3 == null)
				{
					virtualGamepadType = null;
				}
				else
				{
					ConfigData configData2 = currentConfig3.ConfigData;
					virtualGamepadType = ((configData2 != null) ? new VirtualGamepadType?(configData2.VirtualGamepadType) : null);
				}
			}
			getExternalStateInfo.ConfigVirtualGamepadType = virtualGamepadType;
			BaseControllerVM currentGamepad = this.GamepadService.CurrentGamepad;
			getExternalStateInfo.ControllerId = ((currentGamepad != null) ? currentGamepad.ID : null);
			SlotInfo currentSlotInfo = this._gameProfilesService.CurrentSlotInfo;
			getExternalStateInfo.Slot = ((currentSlotInfo != null) ? new Slot?(currentSlotInfo.Slot) : null);
			getExternalStateInfo.ExternalClient = externalClient;
			getExternalStateInfo.ExternalDevice = externalDevice;
			getExternalStateInfo.GamepadAuth = gamepadAuth;
			return getExternalStateInfo;
		}

		public async Task<ExternalDeviceState> GetExternalDeviceStateWithProfiles(ExternalDevice externalDevice, ExternalClient externalClient, GamepadAuth gamepadAuth)
		{
			return await App.HttpClientService.ExternalDevices.GetExternalDeviceStateWithProfiles(this.CreateGetExternalStateInfo(externalDevice, externalClient, gamepadAuth));
		}

		public async Task<ExternalDeviceState> GetExternalDeviceState(ExternalDevice externalDevice, ExternalClient externalClient, GamepadAuth gamepadAuth)
		{
			return await App.HttpClientService.ExternalDevices.GetExternalDeviceState(this.CreateGetExternalStateInfo(externalDevice, externalClient, gamepadAuth));
		}

		public async Task<ExternalState> GetExternalState(ExternalDevice externalDevice, ExternalClient externalClient, GamepadAuth gamepadAuth)
		{
			return await App.HttpClientService.ExternalDevices.GetExternalState(this.CreateGetExternalStateInfo(externalDevice, externalClient, gamepadAuth));
		}

		public ExternalDeviceRelation GetRelationForCurrentGamepadAndConfig()
		{
			GamepadService gamepadService = this.GamepadService;
			bool flag;
			if (gamepadService == null)
			{
				flag = null != null;
			}
			else
			{
				BaseControllerVM currentGamepad = gamepadService.CurrentGamepad;
				flag = ((currentGamepad != null) ? currentGamepad.ID : null) != null;
			}
			if (flag)
			{
				IGameProfilesService gameProfilesService = this._gameProfilesService;
				bool flag2;
				if (gameProfilesService == null)
				{
					flag2 = null != null;
				}
				else
				{
					GameVM currentGame = gameProfilesService.CurrentGame;
					flag2 = ((currentGame != null) ? currentGame.Name : null) != null;
				}
				if (flag2)
				{
					IGameProfilesService gameProfilesService2 = this._gameProfilesService;
					bool flag3;
					if (gameProfilesService2 == null)
					{
						flag3 = null != null;
					}
					else
					{
						GameVM currentGame2 = gameProfilesService2.CurrentGame;
						if (currentGame2 == null)
						{
							flag3 = null != null;
						}
						else
						{
							ConfigVM currentConfig = currentGame2.CurrentConfig;
							flag3 = ((currentConfig != null) ? currentConfig.Name : null) != null;
						}
					}
					if (flag3)
					{
						if (!string.IsNullOrEmpty(this._gameProfilesService.CurrentGame.Name) && !string.IsNullOrEmpty(this._gameProfilesService.CurrentGame.CurrentConfig.Name) && !string.IsNullOrEmpty(this.GamepadService.CurrentGamepad.ID))
						{
							return this.GetRelation(this._gameProfilesService.CurrentGame.Name, this._gameProfilesService.CurrentGame.CurrentConfig.Name, this.GamepadService.CurrentGamepad.Ids);
						}
						return null;
					}
				}
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

		public async Task<bool> AddAndSaveRelation(ExternalDevice externalDevice, ExternalClient externalClient, GamepadAuth authGamepad)
		{
			if (externalDevice != null && App.GamepadService.CurrentGamepad != null && App.GameProfilesService != null)
			{
				IGameProfilesService gameProfilesService = App.GameProfilesService;
				if (((gameProfilesService != null) ? gameProfilesService.CurrentGame : null) != null)
				{
					IGameProfilesService gameProfilesService2 = App.GameProfilesService;
					bool flag;
					if (gameProfilesService2 == null)
					{
						flag = null != null;
					}
					else
					{
						GameVM currentGame = gameProfilesService2.CurrentGame;
						flag = ((currentGame != null) ? currentGame.CurrentConfig : null) != null;
					}
					if (flag)
					{
						string gameName = App.GameProfilesService.CurrentGame.Name;
						string configName = App.GameProfilesService.CurrentGame.CurrentConfig.Name;
						string controllerId = App.GamepadService.CurrentGamepad.ID;
						string controllerDisplayName = App.GamepadService.CurrentGamepad.ControllerDisplayName;
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
						bool flag2 = await this.SaveAndReloadRelations();
						this.Refresh();
						return flag2;
					}
				}
			}
			return false;
		}

		public async Task<bool> SaveRelations()
		{
			ExternalDeviceRelationsCollection externalDeviceRelationsCollection = new ExternalDeviceRelationsCollection();
			this.ExternalDeviceRelationsCollection.CopyToModel(externalDeviceRelationsCollection);
			return await App.HttpClientService.ExternalDevices.SaveExternalDeviceRelations(externalDeviceRelationsCollection);
		}

		public async Task<bool> SaveAndReloadRelations()
		{
			TaskAwaiter<bool> taskAwaiter = this.SaveRelations().GetAwaiter();
			TaskAwaiter<bool> taskAwaiter2;
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			bool flag = !taskAwaiter.GetResult();
			if (!flag)
			{
				taskAwaiter = this.LoadRelations().GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				flag = !taskAwaiter.GetResult();
			}
			bool error = flag;
			await App.HttpClientService.Engine.RequestReloadExternalDevicesData();
			App.EventAggregator.GetEvent<ExternalHelperChanged>().Publish(null);
			return !error;
		}

		public async Task<bool> LoadRelations()
		{
			ExternalDeviceRelationsCollection externalDeviceRelationsCollection = await App.HttpClientService.ExternalDevices.GetExternalDeviceRelations();
			bool flag;
			if (externalDeviceRelationsCollection != null)
			{
				this.ExternalDeviceRelationsCollection.Clear();
				this.ExternalDeviceRelationsCollection.CopyFromModel(externalDeviceRelationsCollection);
				this.CurrentExternalDeviceRelation = this.GetRelationForCurrentGamepadAndConfig();
				flag = true;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public async Task<bool> ChangeExternalDeviceRelationsBaudRate(string externalDeviceId, uint newBaudRate)
		{
			bool flag;
			if (string.IsNullOrEmpty(externalDeviceId) || newBaudRate == 0U)
			{
				flag = false;
			}
			else
			{
				foreach (ExternalDeviceRelation externalDeviceRelation in this.ExternalDeviceRelationsCollection)
				{
					if (externalDeviceRelation.ExternalDevice.ExternalDeviceId == externalDeviceId && externalDeviceRelation.ExternalDevice.BaudRate != newBaudRate)
					{
						externalDeviceRelation.ExternalDevice.BaudRate = newBaudRate;
					}
				}
				flag = await this.SaveAndReloadRelations();
			}
			return flag;
		}

		public async Task<bool> RemoveGameRelations(string gameName)
		{
			bool flag;
			if (string.IsNullOrEmpty(gameName))
			{
				flag = false;
			}
			else
			{
				this.ExternalDeviceRelationsCollection.Remove((ExternalDeviceRelation x) => x.GameName == gameName);
				flag = await this.SaveAndReloadRelations();
			}
			return flag;
		}

		public async Task<bool> RemoveConfigRelations(string gameName, string configName)
		{
			bool flag;
			if (string.IsNullOrEmpty(gameName) || string.IsNullOrEmpty(configName))
			{
				flag = false;
			}
			else
			{
				this.ExternalDeviceRelationsCollection.Remove((ExternalDeviceRelation x) => x.GameName == gameName && x.ConfigName == configName);
				flag = await this.SaveAndReloadRelations();
			}
			return flag;
		}

		public async Task<bool> RemoveExternalDeviceRelations(string externalDeviceId)
		{
			bool flag;
			if (string.IsNullOrEmpty(externalDeviceId))
			{
				flag = false;
			}
			else
			{
				ExternalDeviceRelation relation = null;
				do
				{
					relation = this.ExternalDeviceRelationsCollection.FirstOrDefault((ExternalDeviceRelation x) => x.ExternalDeviceId == externalDeviceId);
					if (relation != null)
					{
						await App.HttpClientService.Gamepad.DisableRemap(relation.ControllerId);
						this.ExternalDeviceRelationsCollection.Remove((ExternalDeviceRelation x) => x.ExternalDeviceId == externalDeviceId);
					}
				}
				while (relation != null);
				flag = await this.SaveAndReloadRelations();
			}
			return flag;
		}

		public async Task<bool> RemoveExternalClientRelations(ulong macAddress)
		{
			bool flag;
			if (macAddress == 0UL)
			{
				flag = false;
			}
			else
			{
				this.ExternalDeviceRelationsCollection.Remove(delegate(ExternalDeviceRelation x)
				{
					ExternalClient externalClient = x.ExternalClient;
					return externalClient != null && externalClient.MacAddress == macAddress;
				});
				flag = await this.SaveAndReloadRelations();
			}
			return flag;
		}

		public async Task<bool> RenameGameRelations(string oldGameName, string newGameName)
		{
			bool flag;
			if (string.IsNullOrEmpty(oldGameName) || string.IsNullOrEmpty(newGameName))
			{
				flag = false;
			}
			else
			{
				foreach (ExternalDeviceRelation externalDeviceRelation in this.ExternalDeviceRelationsCollection)
				{
					if (externalDeviceRelation.GameName == oldGameName)
					{
						externalDeviceRelation.GameName = newGameName;
					}
				}
				flag = await this.SaveAndReloadRelations();
			}
			return flag;
		}

		public async Task<bool> RenameConfigRelations(string gameName, string oldConfigName, string newConfigName)
		{
			bool flag;
			if (string.IsNullOrEmpty(gameName) || string.IsNullOrEmpty(oldConfigName) || string.IsNullOrEmpty(newConfigName))
			{
				flag = false;
			}
			else
			{
				foreach (ExternalDeviceRelation externalDeviceRelation in this.ExternalDeviceRelationsCollection)
				{
					if (externalDeviceRelation.GameName == gameName && externalDeviceRelation.ConfigName == oldConfigName)
					{
						externalDeviceRelation.ConfigName = newConfigName;
					}
				}
				flag = await this.SaveAndReloadRelations();
			}
			return flag;
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

		public async void Refresh()
		{
			this.CurrentExternalDeviceRelation = this.GetRelationForCurrentGamepadAndConfig();
			this.RecreateCurrentGamepadsAuthList();
			if (this.CurrentAuthGamepad != null)
			{
				this.CurrentAuthGamepad.IsOffline = !this.GamepadService.AllPhysicalControllers.Any((BaseControllerVM x) => x.ID == this.CurrentAuthGamepad.ID && x.IsOnline);
			}
			this.CurrentGamepadsAuthCollectionRefreshOfflineStatus();
			await this.RefreshProperties();
		}

		public void OnVirtualGamepadTypeChanged()
		{
			this.Refresh();
			if (this.GamepadService.ExternalDevices.IsBluetoothExist)
			{
				DTMessageBox.Show(DTLocalization.GetString(12096), MessageBoxButton.OK, MessageBoxImage.Asterisk, null, false, MessageBoxResult.None);
			}
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
						GameVM currentGame = this._gameProfilesService.CurrentGame;
						bool flag;
						if (currentGame == null)
						{
							flag = false;
						}
						else
						{
							ConfigVM currentConfig = currentGame.CurrentConfig;
							VirtualGamepadType? virtualGamepadType;
							if (currentConfig == null)
							{
								virtualGamepadType = null;
							}
							else
							{
								ConfigData configData = currentConfig.ConfigData;
								virtualGamepadType = ((configData != null) ? new VirtualGamepadType?(configData.VirtualGamepadType) : null);
							}
							VirtualGamepadType? virtualGamepadType2 = virtualGamepadType;
							VirtualGamepadType virtualGamepadType3 = 1;
							flag = (virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null);
						}
						if (!flag || ControllerTypeExtensions.IsXboxEliteOrOne(controllerType))
						{
							GameVM currentGame2 = this._gameProfilesService.CurrentGame;
							bool flag2;
							if (currentGame2 == null)
							{
								flag2 = false;
							}
							else
							{
								ConfigVM currentConfig2 = currentGame2.CurrentConfig;
								VirtualGamepadType? virtualGamepadType4;
								if (currentConfig2 == null)
								{
									virtualGamepadType4 = null;
								}
								else
								{
									ConfigData configData2 = currentConfig2.ConfigData;
									virtualGamepadType4 = ((configData2 != null) ? new VirtualGamepadType?(configData2.VirtualGamepadType) : null);
								}
								VirtualGamepadType? virtualGamepadType2 = virtualGamepadType4;
								VirtualGamepadType virtualGamepadType3 = 2;
								flag2 = (virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null);
							}
							if (!flag2 || ControllerTypeExtensions.IsSonyDS4Auth(controllerType))
							{
								this.CurrentGamepadsAuthCollection.Add(new GamepadAuth(id, controllerDisplayName, controllerType));
							}
						}
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
				if (client == null || !client.IsConsole || (client.ConsoleType != 1 && client.ConsoleType != 3))
				{
					GameVM currentGame = this._gameProfilesService.CurrentGame;
					bool flag;
					if (currentGame == null)
					{
						flag = false;
					}
					else
					{
						ConfigVM currentConfig = currentGame.CurrentConfig;
						VirtualGamepadType? virtualGamepadType;
						if (currentConfig == null)
						{
							virtualGamepadType = null;
						}
						else
						{
							ConfigData configData = currentConfig.ConfigData;
							virtualGamepadType = ((configData != null) ? new VirtualGamepadType?(configData.VirtualGamepadType) : null);
						}
						VirtualGamepadType? virtualGamepadType2 = virtualGamepadType;
						VirtualGamepadType virtualGamepadType3 = 2;
						flag = (virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null);
					}
					if (!flag)
					{
						GameVM currentGame2 = this._gameProfilesService.CurrentGame;
						bool flag2;
						if (currentGame2 == null)
						{
							flag2 = false;
						}
						else
						{
							ConfigVM currentConfig2 = currentGame2.CurrentConfig;
							VirtualGamepadType? virtualGamepadType4;
							if (currentConfig2 == null)
							{
								virtualGamepadType4 = null;
							}
							else
							{
								ConfigData configData2 = currentConfig2.ConfigData;
								virtualGamepadType4 = ((configData2 != null) ? new VirtualGamepadType?(configData2.VirtualGamepadType) : null);
							}
							VirtualGamepadType? virtualGamepadType2 = virtualGamepadType4;
							VirtualGamepadType virtualGamepadType3 = 1;
							flag2 = (virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null);
						}
						if (!flag2)
						{
							goto IL_56F;
						}
					}
				}
				GamepadAuth currentAuthGamepad = this.CurrentAuthGamepad;
				if (currentAuthGamepad == null || !ControllerTypeExtensions.IsSonyDS4Auth(currentAuthGamepad.ControllerType))
				{
					GamepadAuth currentAuthGamepad2 = this.CurrentAuthGamepad;
					if (currentAuthGamepad2 == null || !ControllerTypeExtensions.IsXBoxAuth(currentAuthGamepad2.ControllerType))
					{
						goto IL_2C5;
					}
				}
				if (this.CurrentGamepadsAuthCollection.All((GamepadAuth x) => x.ID != this.CurrentAuthGamepad.ID))
				{
					GameVM currentGame3 = this._gameProfilesService.CurrentGame;
					bool flag3;
					if (currentGame3 == null)
					{
						flag3 = false;
					}
					else
					{
						ConfigVM currentConfig3 = currentGame3.CurrentConfig;
						VirtualGamepadType? virtualGamepadType5;
						if (currentConfig3 == null)
						{
							virtualGamepadType5 = null;
						}
						else
						{
							ConfigData configData3 = currentConfig3.ConfigData;
							virtualGamepadType5 = ((configData3 != null) ? new VirtualGamepadType?(configData3.VirtualGamepadType) : null);
						}
						VirtualGamepadType? virtualGamepadType2 = virtualGamepadType5;
						VirtualGamepadType virtualGamepadType3 = 1;
						flag3 = (virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null);
					}
					if (!flag3 || ControllerTypeExtensions.IsXboxEliteOrOne(this.CurrentAuthGamepad.ControllerType))
					{
						GameVM currentGame4 = this._gameProfilesService.CurrentGame;
						bool flag4;
						if (currentGame4 == null)
						{
							flag4 = false;
						}
						else
						{
							ConfigVM currentConfig4 = currentGame4.CurrentConfig;
							VirtualGamepadType? virtualGamepadType6;
							if (currentConfig4 == null)
							{
								virtualGamepadType6 = null;
							}
							else
							{
								ConfigData configData4 = currentConfig4.ConfigData;
								virtualGamepadType6 = ((configData4 != null) ? new VirtualGamepadType?(configData4.VirtualGamepadType) : null);
							}
							VirtualGamepadType? virtualGamepadType2 = virtualGamepadType6;
							VirtualGamepadType virtualGamepadType3 = 2;
							flag4 = (virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null);
						}
						if (!flag4 || ControllerTypeExtensions.IsSonyDS4Auth(this.CurrentAuthGamepad.ControllerType))
						{
							this.CurrentGamepadsAuthCollection.Insert(0, this.CurrentAuthGamepad);
						}
					}
				}
				IL_2C5:
				using (IEnumerator<GamepadAuth> enumerator = this.GamepadsAuth.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						GamepadAuth gamepadAuth = enumerator.Current;
						if (!ControllerTypeExtensions.IsSonyDS4Auth(gamepadAuth.ControllerType) && !ControllerTypeExtensions.IsXBoxAuth(gamepadAuth.ControllerType))
						{
							goto IL_3FE;
						}
						if (ControllerTypeExtensions.IsSonyDS4Auth(gamepadAuth.ControllerType))
						{
							GameVM currentGame5 = this._gameProfilesService.CurrentGame;
							bool flag5;
							if (currentGame5 == null)
							{
								flag5 = false;
							}
							else
							{
								ConfigVM currentConfig5 = currentGame5.CurrentConfig;
								VirtualGamepadType? virtualGamepadType7;
								if (currentConfig5 == null)
								{
									virtualGamepadType7 = null;
								}
								else
								{
									ConfigData configData5 = currentConfig5.ConfigData;
									virtualGamepadType7 = ((configData5 != null) ? new VirtualGamepadType?(configData5.VirtualGamepadType) : null);
								}
								VirtualGamepadType? virtualGamepadType2 = virtualGamepadType7;
								VirtualGamepadType virtualGamepadType3 = 1;
								flag5 = (virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null);
							}
							if (flag5)
							{
								goto IL_3FE;
							}
						}
						if (ControllerTypeExtensions.IsXBoxAuth(gamepadAuth.ControllerType))
						{
							GameVM currentGame6 = this._gameProfilesService.CurrentGame;
							bool flag6;
							if (currentGame6 == null)
							{
								flag6 = false;
							}
							else
							{
								ConfigVM currentConfig6 = currentGame6.CurrentConfig;
								VirtualGamepadType? virtualGamepadType8;
								if (currentConfig6 == null)
								{
									virtualGamepadType8 = null;
								}
								else
								{
									ConfigData configData6 = currentConfig6.ConfigData;
									virtualGamepadType8 = ((configData6 != null) ? new VirtualGamepadType?(configData6.VirtualGamepadType) : null);
								}
								VirtualGamepadType? virtualGamepadType2 = virtualGamepadType8;
								VirtualGamepadType virtualGamepadType3 = 2;
								flag6 = (virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null);
							}
							if (flag6)
							{
								goto IL_3FE;
							}
						}
						IL_43C:
						if (ControllerTypeExtensions.IsSonyDS4Auth(gamepadAuth.ControllerType))
						{
							GameVM currentGame7 = this._gameProfilesService.CurrentGame;
							bool flag7;
							if (currentGame7 == null)
							{
								flag7 = false;
							}
							else
							{
								ConfigVM currentConfig7 = currentGame7.CurrentConfig;
								VirtualGamepadType? virtualGamepadType9;
								if (currentConfig7 == null)
								{
									virtualGamepadType9 = null;
								}
								else
								{
									ConfigData configData7 = currentConfig7.ConfigData;
									virtualGamepadType9 = ((configData7 != null) ? new VirtualGamepadType?(configData7.VirtualGamepadType) : null);
								}
								VirtualGamepadType? virtualGamepadType2 = virtualGamepadType9;
								VirtualGamepadType virtualGamepadType3 = 2;
								flag7 = (virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null);
							}
							if (flag7)
							{
								goto IL_525;
							}
						}
						if (!ControllerTypeExtensions.IsXBoxAuth(gamepadAuth.ControllerType))
						{
							continue;
						}
						GameVM currentGame8 = this._gameProfilesService.CurrentGame;
						bool flag8;
						if (currentGame8 == null)
						{
							flag8 = false;
						}
						else
						{
							ConfigVM currentConfig8 = currentGame8.CurrentConfig;
							VirtualGamepadType? virtualGamepadType10;
							if (currentConfig8 == null)
							{
								virtualGamepadType10 = null;
							}
							else
							{
								ConfigData configData8 = currentConfig8.ConfigData;
								virtualGamepadType10 = ((configData8 != null) ? new VirtualGamepadType?(configData8.VirtualGamepadType) : null);
							}
							VirtualGamepadType? virtualGamepadType2 = virtualGamepadType10;
							VirtualGamepadType virtualGamepadType3 = 1;
							flag8 = (virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null);
						}
						if (!flag8)
						{
							continue;
						}
						IL_525:
						if (this.CurrentGamepadsAuthCollection.All((GamepadAuth x) => x.ID != gamepadAuth.ID))
						{
							this.CurrentGamepadsAuthCollection.Add(gamepadAuth);
							continue;
						}
						continue;
						IL_3FE:
						if (this.CurrentGamepadsAuthCollection.Any((GamepadAuth x) => x.ID == gamepadAuth.ID))
						{
							this.CurrentGamepadsAuthCollection.Remove(this.CurrentGamepadsAuthCollection.FirstOrDefault((GamepadAuth x) => x.ID == gamepadAuth.ID));
							goto IL_43C;
						}
						goto IL_43C;
					}
				}
				IL_56F:;
			}
			this.CurrentGamepadsAuthCollectionRefreshOfflineStatus();
		}

		public async void RecreateCurrentGamepadsAuthList()
		{
			using (await new AsyncLock(ExternalDeviceRelationsHelper._refreshGamepadAuthSemaphore).LockAsync())
			{
				GameVM currentGame = this._gameProfilesService.CurrentGame;
				bool flag;
				if (currentGame == null)
				{
					flag = false;
				}
				else
				{
					ConfigVM currentConfig = currentGame.CurrentConfig;
					VirtualGamepadType? virtualGamepadType;
					if (currentConfig == null)
					{
						virtualGamepadType = null;
					}
					else
					{
						ConfigData configData = currentConfig.ConfigData;
						virtualGamepadType = ((configData != null) ? new VirtualGamepadType?(configData.VirtualGamepadType) : null);
					}
					VirtualGamepadType? virtualGamepadType2 = virtualGamepadType;
					VirtualGamepadType virtualGamepadType3 = 2;
					flag = (virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null);
				}
				if (!flag)
				{
					GameVM currentGame2 = this._gameProfilesService.CurrentGame;
					bool flag2;
					if (currentGame2 == null)
					{
						flag2 = false;
					}
					else
					{
						ConfigVM currentConfig2 = currentGame2.CurrentConfig;
						VirtualGamepadType? virtualGamepadType4;
						if (currentConfig2 == null)
						{
							virtualGamepadType4 = null;
						}
						else
						{
							ConfigData configData2 = currentConfig2.ConfigData;
							virtualGamepadType4 = ((configData2 != null) ? new VirtualGamepadType?(configData2.VirtualGamepadType) : null);
						}
						VirtualGamepadType? virtualGamepadType2 = virtualGamepadType4;
						VirtualGamepadType virtualGamepadType3 = 1;
						flag2 = (virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null);
					}
					if (!flag2)
					{
						goto IL_500;
					}
				}
				GamepadAuth currentAuthGamepad = this.CurrentAuthGamepad;
				if (currentAuthGamepad != null && ControllerTypeExtensions.IsSonyDS4Auth(currentAuthGamepad.ControllerType))
				{
					GameVM currentGame3 = this._gameProfilesService.CurrentGame;
					bool flag3;
					if (currentGame3 == null)
					{
						flag3 = false;
					}
					else
					{
						ConfigVM currentConfig3 = currentGame3.CurrentConfig;
						VirtualGamepadType? virtualGamepadType5;
						if (currentConfig3 == null)
						{
							virtualGamepadType5 = null;
						}
						else
						{
							ConfigData configData3 = currentConfig3.ConfigData;
							virtualGamepadType5 = ((configData3 != null) ? new VirtualGamepadType?(configData3.VirtualGamepadType) : null);
						}
						VirtualGamepadType? virtualGamepadType2 = virtualGamepadType5;
						VirtualGamepadType virtualGamepadType3 = 2;
						flag3 = (virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null);
					}
					if (flag3)
					{
						goto IL_22B;
					}
				}
				GameVM currentGame4 = this._gameProfilesService.CurrentGame;
				bool flag4;
				if (currentGame4 == null)
				{
					flag4 = false;
				}
				else
				{
					ConfigVM currentConfig4 = currentGame4.CurrentConfig;
					VirtualGamepadType? virtualGamepadType6;
					if (currentConfig4 == null)
					{
						virtualGamepadType6 = null;
					}
					else
					{
						ConfigData configData4 = currentConfig4.ConfigData;
						virtualGamepadType6 = ((configData4 != null) ? new VirtualGamepadType?(configData4.VirtualGamepadType) : null);
					}
					VirtualGamepadType? virtualGamepadType2 = virtualGamepadType6;
					VirtualGamepadType virtualGamepadType3 = 1;
					flag4 = (virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null);
				}
				if (!flag4)
				{
					goto IL_256;
				}
				GamepadAuth currentAuthGamepad2 = this.CurrentAuthGamepad;
				if (currentAuthGamepad2 == null || !ControllerTypeExtensions.IsXBoxAuth(currentAuthGamepad2.ControllerType))
				{
					goto IL_256;
				}
				IL_22B:
				if (this.CurrentGamepadsAuthCollection.All((GamepadAuth x) => x.ID != this.CurrentAuthGamepad.ID))
				{
					this.CurrentGamepadsAuthCollection.Insert(0, this.CurrentAuthGamepad);
				}
				IL_256:
				using (IEnumerator<GamepadAuth> enumerator = this.GamepadsAuth.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						GamepadAuth gamepadAuth = enumerator.Current;
						if (!ControllerTypeExtensions.IsSonyDS4Auth(gamepadAuth.ControllerType) && !ControllerTypeExtensions.IsXBoxAuth(gamepadAuth.ControllerType))
						{
							goto IL_38F;
						}
						if (ControllerTypeExtensions.IsSonyDS4Auth(gamepadAuth.ControllerType))
						{
							GameVM currentGame5 = this._gameProfilesService.CurrentGame;
							bool flag5;
							if (currentGame5 == null)
							{
								flag5 = false;
							}
							else
							{
								ConfigVM currentConfig5 = currentGame5.CurrentConfig;
								VirtualGamepadType? virtualGamepadType7;
								if (currentConfig5 == null)
								{
									virtualGamepadType7 = null;
								}
								else
								{
									ConfigData configData5 = currentConfig5.ConfigData;
									virtualGamepadType7 = ((configData5 != null) ? new VirtualGamepadType?(configData5.VirtualGamepadType) : null);
								}
								VirtualGamepadType? virtualGamepadType2 = virtualGamepadType7;
								VirtualGamepadType virtualGamepadType3 = 1;
								flag5 = (virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null);
							}
							if (flag5)
							{
								goto IL_38F;
							}
						}
						if (ControllerTypeExtensions.IsXBoxAuth(gamepadAuth.ControllerType))
						{
							GameVM currentGame6 = this._gameProfilesService.CurrentGame;
							bool flag6;
							if (currentGame6 == null)
							{
								flag6 = false;
							}
							else
							{
								ConfigVM currentConfig6 = currentGame6.CurrentConfig;
								VirtualGamepadType? virtualGamepadType8;
								if (currentConfig6 == null)
								{
									virtualGamepadType8 = null;
								}
								else
								{
									ConfigData configData6 = currentConfig6.ConfigData;
									virtualGamepadType8 = ((configData6 != null) ? new VirtualGamepadType?(configData6.VirtualGamepadType) : null);
								}
								VirtualGamepadType? virtualGamepadType2 = virtualGamepadType8;
								VirtualGamepadType virtualGamepadType3 = 2;
								flag6 = (virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null);
							}
							if (flag6)
							{
								goto IL_38F;
							}
						}
						IL_3CD:
						if (ControllerTypeExtensions.IsSonyDS4Auth(gamepadAuth.ControllerType))
						{
							GameVM currentGame7 = this._gameProfilesService.CurrentGame;
							bool flag7;
							if (currentGame7 == null)
							{
								flag7 = false;
							}
							else
							{
								ConfigVM currentConfig7 = currentGame7.CurrentConfig;
								VirtualGamepadType? virtualGamepadType9;
								if (currentConfig7 == null)
								{
									virtualGamepadType9 = null;
								}
								else
								{
									ConfigData configData7 = currentConfig7.ConfigData;
									virtualGamepadType9 = ((configData7 != null) ? new VirtualGamepadType?(configData7.VirtualGamepadType) : null);
								}
								VirtualGamepadType? virtualGamepadType2 = virtualGamepadType9;
								VirtualGamepadType virtualGamepadType3 = 2;
								flag7 = (virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null);
							}
							if (flag7)
							{
								goto IL_4B6;
							}
						}
						if (!ControllerTypeExtensions.IsXBoxAuth(gamepadAuth.ControllerType))
						{
							continue;
						}
						GameVM currentGame8 = this._gameProfilesService.CurrentGame;
						bool flag8;
						if (currentGame8 == null)
						{
							flag8 = false;
						}
						else
						{
							ConfigVM currentConfig8 = currentGame8.CurrentConfig;
							VirtualGamepadType? virtualGamepadType10;
							if (currentConfig8 == null)
							{
								virtualGamepadType10 = null;
							}
							else
							{
								ConfigData configData8 = currentConfig8.ConfigData;
								virtualGamepadType10 = ((configData8 != null) ? new VirtualGamepadType?(configData8.VirtualGamepadType) : null);
							}
							VirtualGamepadType? virtualGamepadType2 = virtualGamepadType10;
							VirtualGamepadType virtualGamepadType3 = 1;
							flag8 = (virtualGamepadType2.GetValueOrDefault() == virtualGamepadType3) & (virtualGamepadType2 != null);
						}
						if (!flag8)
						{
							continue;
						}
						IL_4B6:
						if (this.CurrentGamepadsAuthCollection.All((GamepadAuth x) => x.ID != gamepadAuth.ID))
						{
							this.CurrentGamepadsAuthCollection.Add(gamepadAuth);
							continue;
						}
						continue;
						IL_38F:
						if (this.CurrentGamepadsAuthCollection.Any((GamepadAuth x) => x.ID == gamepadAuth.ID))
						{
							this.CurrentGamepadsAuthCollection.Remove(this.CurrentGamepadsAuthCollection.FirstOrDefault((GamepadAuth x) => x.ID == gamepadAuth.ID));
							goto IL_3CD;
						}
						goto IL_3CD;
					}
				}
				IL_500:;
			}
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
						gamepadAuth.IsOffline = !this.GamepadService.AllPhysicalControllers.Any((BaseControllerVM x) => x.ID == gamepadAuth.ID && x.IsOnline);
					}
				}
			}
		}

		private ExternalDeviceRelationsCollection _externalDeviceRelationsCollection;

		private ExternalDeviceRelation _currentExternalDeviceRelation;

		private ExternalDevice _currentExternalDevice;

		private IGameProfilesService _gameProfilesService;

		private ObservableCollection<GamepadAuth> _gamepadsAuth;

		private SortableObservableCollection<GamepadAuth> _currentGamepadsAuthCollection;

		private static readonly AsyncSemaphore _refreshGamepadAuthSemaphore = new AsyncSemaphore(1);

		private ExternalStateAgent _currentExternalStateAgent;

		private string _currentExternalStateAgentTooltip;

		private ExternalClient _latestExternalClientFilter;

		public delegate void StateChangedDelegate();
	}
}
