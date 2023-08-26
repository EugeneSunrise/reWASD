using System;
using System.IO;
using System.Runtime.CompilerServices;
using DiscSoft.NET.Common.Utils;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Interfaces;
using reWASDCommon.Network.HTTP.DataTransferObjects.Events.Desktop;
using reWASDEngine;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.ExternalDeviceRelations;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils.XBUtilModel;

namespace XBEliteWPF.DataModels
{
	public class Config : IComparable
	{
		public Lazy<IGamepadService> GamepadServiceLazy { get; set; }

		public IGameProfilesService GameProfilesService { get; set; }

		public ILicensingService LicensingService { get; set; }

		public ConfigData ConfigData
		{
			get
			{
				return this._configData;
			}
		}

		public string ConfigPath { get; set; }

		public Game ParentGame { get; set; }

		public string Name { get; set; }

		public string GameName { get; set; }

		public bool IsAutodetectEnabledForAnySlot { get; set; }

		public bool IsFullDataLoaded { get; set; }

		public bool? IsLoadedSuccessfully { get; set; }

		public bool IsEmpty
		{
			get
			{
				ConfigData configData = this.ConfigData;
				return configData != null && configData.IsEmpty;
			}
		}

		public bool IsFutureConfigRelevantForCurrentGamepad(ControllerFamily family)
		{
			return true;
		}

		public Config(string configPath, IConfigFileService ifs, IGameProfilesService gps, Lazy<IGamepadService> gsLazy, ILicensingService ls, Game parentGame)
		{
			this.GameProfilesService = gps;
			this.GamepadServiceLazy = gsLazy;
			this.LicensingService = ls;
			this._configFileService = ifs;
			this.ParentGame = parentGame;
			this.ConfigPath = configPath;
			this.Name = Path.GetFileNameWithoutExtension(this.ConfigPath);
		}

		public void InitConfig()
		{
			this._configData = XBUtils.CreateConfigData(false);
		}

		public bool ReadConfigFromJson(bool verbose = true, bool forceLoadFullData = false)
		{
			bool flag = false;
			this.IsLoadedSuccessfully = new bool?(false);
			object readConfigFromJsonLock = this._readConfigFromJsonLock;
			lock (readConfigFromJsonLock)
			{
				try
				{
					if (!this._configFileService.ParseConfigFile(this.ConfigPath, ref this._configData, ref flag, verbose, null, forceLoadFullData))
					{
						return false;
					}
				}
				catch (UnauthorizedAccessException)
				{
				}
			}
			this.IsFullDataLoaded = forceLoadFullData;
			this.IsLoadedSuccessfully = new bool?(true);
			return true;
		}

		public bool SaveConfig(ConfigData configData, string clientID)
		{
			this._configData = configData;
			return this.SaveConfigToJson(clientID, true);
		}

		public bool SaveConfigToJson(string clientID = null, bool reload = false)
		{
			bool flag = false;
			try
			{
				if (this._configData != null && !this._configData.IsExternal)
				{
					string text = ((this.ParentGame != null) ? this.ParentGame.Name : this.GameName);
					Lazy<IGamepadService> gamepadServiceLazy = this.GamepadServiceLazy;
					if (gamepadServiceLazy != null)
					{
						IGamepadService value = gamepadServiceLazy.Value;
						if (value != null)
						{
							ExternalDeviceRelationsHelper externalDeviceRelationsHelper = value.ExternalDeviceRelationsHelper;
							if (externalDeviceRelationsHelper != null)
							{
								externalDeviceRelationsHelper.RemoveConfigRelations(text, this.Name);
							}
						}
					}
				}
				RegistryHelper.SetValue("Config", "VirtualGamepadType", this.ConfigData.VirtualGamepadType);
				Exception ex;
				flag = this._configFileService.SaveConfigFile(this.ConfigPath, this._configData, ref ex);
				if (clientID != null && flag)
				{
					if (reload)
					{
						ConfigData configData = this._configData;
						if (configData != null)
						{
							configData.Dispose();
						}
						this._configData = null;
						this.ReadConfigFromJson(false, true);
					}
					Engine.EventAggregator.GetEvent<ConfigSaved>().Publish(new ConfigSavedEvent(clientID, this.GameName, this.Name));
				}
			}
			catch (Exception ex2)
			{
				Tracer.TraceException(ex2, "SaveConfigToJson");
			}
			return flag;
		}

		public void SetToDefault()
		{
			File.WriteAllText(this.ConfigPath, this._configFileService.GetDefaultConfig());
			this.ReadConfigFromJson(true, false);
		}

		private void OnAfterConfigRead()
		{
		}

		public int CompareTo(object obj)
		{
			Config config = (Config)obj;
			return string.Compare(this.Name, config.Name, StringComparison.CurrentCulture);
		}

		public bool ReadConfigFromJsonIfNotLoaded(bool forceLoadFullData = false)
		{
			Tracer.TraceWrite("ReadConfigFromJSONIfNotLoaded", false);
			bool? flag;
			bool flag2;
			if (!forceLoadFullData || this.IsFullDataLoaded)
			{
				flag = this.IsLoadedSuccessfully;
				flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					goto IL_79;
				}
			}
			Tracer.TraceWrite("Config is not loaded try to load", false);
			this.IsLoadedSuccessfully = new bool?(this.ReadConfigFromJson(false, true));
			flag = this.IsLoadedSuccessfully;
			flag2 = true;
			if (!((flag.GetValueOrDefault() == flag2) & (flag != null)))
			{
				Tracer.TraceWrite("Config is not loaded after try to load", false);
			}
			IL_79:
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Config load success ");
			defaultInterpolatedStringHandler.AppendFormatted<bool?>(this.IsLoadedSuccessfully);
			Tracer.TraceWrite(defaultInterpolatedStringHandler.ToStringAndClear(), false);
			flag = this.IsLoadedSuccessfully;
			flag2 = true;
			return (flag.GetValueOrDefault() == flag2) & (flag != null);
		}

		private IConfigFileService _configFileService;

		private ConfigData _configData;

		private readonly object _readConfigFromJsonLock = new object();
	}
}
