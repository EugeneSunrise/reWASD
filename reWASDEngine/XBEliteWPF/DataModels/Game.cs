using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoftReWASDServiceNamespace;
using Prism.Events;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Infrastructure.JSONModel;
using reWASDCommon.Interfaces;
using reWASDCommon.MacroCompilers;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using reWASDEngine;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.ExternalDeviceRelations;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils;
using XBEliteWPF.Utils.XBUtilModel;

namespace XBEliteWPF.DataModels
{
	public class Game : IComparable
	{
		public string Name { get; set; }

		public bool IsAutodetect { get; set; }

		public bool HasAnyAutodetectProcesses
		{
			get
			{
				Config config = this.ConfigCollection.FirstOrDefault<Config>();
				object obj;
				if (config == null)
				{
					obj = null;
				}
				else
				{
					ConfigData configData = config.ConfigData;
					if (configData == null)
					{
						obj = null;
					}
					else
					{
						obj = configData.FirstOrDefault((SubConfigData c) => c.MainXBBindingCollection.ProcessNames.Count > 0);
					}
				}
				return obj != null;
			}
		}

		public ObservableCollection<Config> ConfigCollection
		{
			get
			{
				ObservableCollection<Config> observableCollection;
				if ((observableCollection = this._configCollection) == null)
				{
					observableCollection = (this._configCollection = new ObservableCollection<Config>());
				}
				return observableCollection;
			}
		}

		public string GetGameFolderPath()
		{
			return this._gameProfilesService.GamesFolderPath + "\\" + this.Name;
		}

		public string GetImageSourcePath()
		{
			return this.GetGameFolderPath() + "\\IcoGame.png";
		}

		public Game(string name, IGameProfilesService gps, IConfigFileService ifs, IEventAggregator ea, Lazy<IGamepadService> gsLazy, ILicensingService ls)
		{
			this._gameProfilesService = gps;
			this._configFileService = ifs;
			this._gamepadServiceLazy = gsLazy;
			this._licensingService = ls;
			this.Name = name;
			this.ResolveAutoDetect();
		}

		public void SetAutoDetect(bool value)
		{
			if (value)
			{
				this.ConfigCollection.ForEach(delegate(Config p)
				{
					p.ReadConfigFromJsonIfNotLoaded(false);
				});
			}
			this.IsAutodetect = value;
		}

		public void ResolveAutoDetect()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(this.GetGameFolderPath());
			bool flag = File.Exists(((directoryInfo != null) ? directoryInfo.ToString() : null) + "\\IsAutodetect");
			this.SetAutoDetect(flag);
		}

		public bool DeleteConfig(Config config)
		{
			try
			{
				File.Delete(config.ConfigPath);
			}
			catch (Exception)
			{
				return false;
			}
			this.ConfigCollection.Remove(config);
			this._gamepadServiceLazy.Value.RemoveRelationsForConfigFile(config.ConfigPath);
			Lazy<IGamepadService> gamepadServiceLazy = this._gamepadServiceLazy;
			if (gamepadServiceLazy != null)
			{
				IGamepadService value = gamepadServiceLazy.Value;
				if (value != null)
				{
					ExternalDeviceRelationsHelper externalDeviceRelationsHelper = value.ExternalDeviceRelationsHelper;
					if (externalDeviceRelationsHelper != null)
					{
						externalDeviceRelationsHelper.RemoveConfigRelations(this.Name, config.Name);
					}
				}
			}
			return true;
		}

		public void FillConfigsCollection()
		{
			this.ConfigCollection.Clear();
			string text = Path.Combine(Engine.UserSettingsService.ConfigsFolderPath, this.Name + "\\Controller");
			if (!Directory.Exists(text))
			{
				return;
			}
			using (IEnumerator<string> enumerator = Directory.EnumerateFiles(text).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string config = enumerator.Current;
					if (config.EndsWith(".rewasd"))
					{
						Config config2 = new Config(config, this._configFileService, this._gameProfilesService, this._gamepadServiceLazy, this._licensingService, this);
						config2.GameName = this.Name;
						try
						{
							if (this.IsAutodetect)
							{
								config2.ReadConfigFromJson(false, false);
							}
							if (this.ConfigCollection.Where((Config item) => item.ConfigPath == config).FirstOrDefault<Config>() == null)
							{
								this.ConfigCollection.Add(config2);
							}
						}
						catch (Exception ex)
						{
							Tracer.TraceException(ex, "FillConfigsCollection");
						}
					}
				}
			}
			XBUtils.SortObservableCollection<Config>(this.ConfigCollection);
		}

		public void FillSingleConfig(string configSourcePath)
		{
			if (configSourcePath.EndsWith(".rewasd"))
			{
				Config config = new Config(configSourcePath, this._configFileService, this._gameProfilesService, this._gamepadServiceLazy, this._licensingService, this);
				config.GameName = this.Name;
				if (config.ReadConfigFromJson(false, false))
				{
					this.ConfigCollection.Remove((Config item) => item.ConfigPath == configSourcePath);
					this.ConfigCollection.Add(config);
					SenderGoogleAnalytics.SendNewProfileAddedEvent(this.ConfigCollection.Count.ToString());
					XBUtils.SortObservableCollection<Config>(this.ConfigCollection);
				}
			}
		}

		public void SpreadProcessNames(Config configToSpreadFrom = null, bool spreadTo = false)
		{
			if (!this.ConfigCollection.Any<Config>())
			{
				return;
			}
			if (configToSpreadFrom == null)
			{
				configToSpreadFrom = this.ConfigCollection.First<Config>();
			}
			else if (spreadTo)
			{
				Config config = this.ConfigCollection.FirstOrDefault((Config p) => p != configToSpreadFrom);
				if (config != null)
				{
					try
					{
						config.ReadConfigFromJsonIfNotLoaded(false);
						SubConfigData subConfigData = config.ConfigData.FirstOrDefault((SubConfigData c) => c.MainXBBindingCollection.ProcessNames.Count > 0);
						if (subConfigData != null)
						{
							configToSpreadFrom.ConfigData[0].MainXBBindingCollection.ProcessNames = new List<string>(subConfigData.MainXBBindingCollection.ProcessNames.Union(configToSpreadFrom.ConfigData[0].MainXBBindingCollection.ProcessNames).ToList<string>());
						}
						configToSpreadFrom.SaveConfigToJson(null, false);
					}
					catch (Exception ex)
					{
						Tracer.TraceException(ex, "SpreadProcessNames");
					}
				}
			}
			foreach (Config config2 in this.ConfigCollection)
			{
				if (config2 != configToSpreadFrom)
				{
					try
					{
						config2.ReadConfigFromJsonIfNotLoaded(false);
						SubConfigData subConfigData2 = config2.ConfigData.FirstOrDefault((SubConfigData c) => c.MainXBBindingCollection.ProcessNames.Count > 0);
						if (subConfigData2 == null)
						{
							subConfigData2 = config2.ConfigData.FirstOrDefault<SubConfigData>();
						}
						if (subConfigData2 != null)
						{
							config2.ConfigData[0].MainXBBindingCollection.ProcessNames = new List<string>(subConfigData2.MainXBBindingCollection.ProcessNames.Union(configToSpreadFrom.ConfigData[0].MainXBBindingCollection.ProcessNames).ToList<string>());
						}
						config2.SaveConfigToJson(null, false);
					}
					catch (Exception ex2)
					{
						Tracer.TraceException(ex2, "SpreadProcessNames");
					}
				}
			}
		}

		public bool CreateNewConfig(string configName)
		{
			string text = Path.Combine(Engine.UserSettingsService.ConfigsFolderPath, this.Name + "\\Controller\\");
			Config config = new Config(text + XBUtils.NormalizeToMaxPathTrimFilename(text, configName.Trim(), ".rewasd", null) + ".rewasd", this._configFileService, this._gameProfilesService, this._gamepadServiceLazy, this._licensingService, this);
			config.Name = configName;
			config.GameName = this.Name;
			try
			{
				File.WriteAllText(config.ConfigPath, Engine.ConfigFileService.GetDefaultConfig());
				if (config.ReadConfigFromJson(true, true))
				{
					Config config2 = this.ConfigCollection.FirstOrDefault((Config item) => item.Name.ToLower() == configName.ToLower());
					if (config2 != null)
					{
						this.ConfigCollection.Remove(config2);
					}
					this.ConfigCollection.Add(config);
					XBUtils.SortObservableCollection<Config>(this.ConfigCollection);
					this.SpreadProcessNames(config, true);
					config.ConfigData.VirtualGamepadType = ConfigData.ConfigGamepadType;
					Exception ex;
					new ConfigJSON_3_0().WriteBindingsToFile(config.ConfigPath, config.ConfigData, ref ex);
					DSUtils.GrantAccess(config.ConfigPath);
					SenderGoogleAnalytics.SendNewProfileAddedEvent(this.ConfigCollection.Count.ToString());
					return true;
				}
			}
			catch (Exception)
			{
			}
			return false;
		}

		public ImportConfigResult ImportConfig(string configName, string configContent, bool isCloning, bool isRewrite)
		{
			ImportConfigResult importConfigResult = new ImportConfigResult();
			string text = Path.Combine(Engine.UserSettingsService.ConfigsFolderPath, this.Name + "\\Controller\\");
			string configPath = string.Empty;
			if (isCloning)
			{
				configPath = text + XBUtils.NormalizeToMaxPathTrimFilename(text, configName.Trim(), ".rewasd", null) + ".rewasd";
				int num = 0;
				string text2 = string.Empty;
				while (File.Exists(configPath))
				{
					string text3;
					if (num != 0)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
						defaultInterpolatedStringHandler.AppendLiteral(" - Copy (");
						defaultInterpolatedStringHandler.AppendFormatted<int>(num);
						defaultInterpolatedStringHandler.AppendLiteral(")");
						text3 = defaultInterpolatedStringHandler.ToStringAndClear();
					}
					else
					{
						text3 = " - Copy";
					}
					string text4 = text3;
					text2 = configName + text4;
					configPath = text + XBUtils.NormalizeToMaxPathTrimFilename(text, text2.Trim(), ".rewasd", text4) + ".rewasd";
					num++;
				}
				if (!string.IsNullOrEmpty(text2))
				{
					configName = text2;
				}
			}
			else
			{
				configPath = text + XBUtils.NormalizeToMaxPathTrimFilename(text, configName.Trim(), ".rewasd", null) + ".rewasd";
			}
			if (configPath.Length > 260)
			{
				importConfigResult.Status = 1;
				return importConfigResult;
			}
			if (File.Exists(configPath) && !isRewrite)
			{
				if (!isCloning)
				{
					importConfigResult.Status = 2;
					return importConfigResult;
				}
				string directoryName = Path.GetDirectoryName(configPath);
				if (directoryName == null)
				{
					importConfigResult.Status = 3;
					return importConfigResult;
				}
				if (new HashSet<string>(Directory.GetFiles(directoryName)).Contains(configPath))
				{
					configPath = DSUtils.GetUniqueFileNameWithParentheses(directoryName, configName + ".rewasd");
				}
			}
			try
			{
				File.WriteAllText(configPath, configContent);
				File.SetAttributes(configPath, FileAttributes.Normal);
				uint num2 = 0U;
				REWASD_CONTROLLER_PROFILE rewasd_CONTROLLER_PROFILE = REWASD_CONTROLLER_PROFILE.CreateBlankInstance();
				bool flag = true;
				bool flag2 = true;
				string text5 = "";
				bool flag3;
				bool flag4;
				bool flag5;
				bool flag6;
				uint num3;
				List<byte[]> list;
				bool flag7;
				bool flag8;
				ApplyResultStatus applyResultStatus;
				MacroCompiler.CompileConfigToControllerProfileSlot(ref rewasd_CONTROLLER_PROFILE, ref num2, 0, configPath, ref flag3, ref flag4, ref flag5, ref flag6, ref text5, ref num3, ref list, 55U, default(LicenseData), ref flag7, ref flag8, ref applyResultStatus, Engine.UserSettingsService, ref flag, ref flag2, false, null, null, null, null, null, false, this.Name, configName, null, null, false, false, true);
			}
			catch (Exception ex)
			{
				Tracer.TraceException(ex, "ImportConfig");
				try
				{
					File.Delete(configPath);
				}
				catch (Exception)
				{
				}
				importConfigResult.Status = 3;
				importConfigResult.ErrorText = (RegistryHelper.GetWinDefenderControlledForlder() ? DTLocalization.GetString(11852) : DTLocalization.GetString(12469));
				return importConfigResult;
			}
			this.FillSingleConfig(configPath);
			Config config = this.ConfigCollection.FirstOrDefault((Config p) => p.ConfigPath == configPath);
			if (config != null)
			{
				importConfigResult.ImportedConfigName = config.Name;
			}
			importConfigResult.Status = 0;
			return importConfigResult;
		}

		public void Dispose()
		{
			foreach (Config config in this.ConfigCollection)
			{
				config.IsFullDataLoaded = false;
				config.IsLoadedSuccessfully = null;
				if (config.ConfigData != null)
				{
					config.ConfigData.Dispose();
					GC.SuppressFinalize(config.ConfigData);
				}
			}
			GC.Collect();
		}

		public int CompareTo(object obj)
		{
			Game game = (Game)obj;
			return string.Compare(this.Name, game.Name, StringComparison.CurrentCulture);
		}

		private IGameProfilesService _gameProfilesService;

		private IConfigFileService _configFileService;

		private Lazy<IGamepadService> _gamepadServiceLazy;

		private ILicensingService _licensingService;

		private ObservableCollection<Config> _configCollection;
	}
}
