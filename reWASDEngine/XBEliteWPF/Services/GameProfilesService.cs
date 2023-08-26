using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using Prism.Commands;
using Prism.Events;
using reWASDCommon.Infrastructure.JSONModel;
using reWASDCommon.Interfaces;
using reWASDEngine;
using XBEliteWPF.DataModels;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.ExternalDeviceRelations;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils;
using XBEliteWPF.Utils.XBUtilModel;

namespace XBEliteWPF.Services
{
	public class GameProfilesService : IGameProfilesService, IServiceInitedAwaitable
	{
		public int MinimumStickZoneLow
		{
			get
			{
				return 100;
			}
		}

		public int MaximumStickZoneHight
		{
			get
			{
				return 100;
			}
		}

		public int MinimumTriggerZoneLow
		{
			get
			{
				return 100;
			}
		}

		public int MaximumTriggerZoneHigh
		{
			get
			{
				return 100;
			}
		}

		public ObservableCollection<Game> GamesCollection { get; } = new ObservableCollection<Game>();

		public ObservableCollection<Config> PresetsCollection { get; } = new ObservableCollection<Config>();

		public DelegateCommand ToggleHideIrrelevantSubConfigsCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._toggleUseIrrelevantSubConfigs) == null)
				{
					delegateCommand = (this._toggleUseIrrelevantSubConfigs = new DelegateCommand(new Action(this.ToggleUseIrrelevantSubConfigs)));
				}
				return delegateCommand;
			}
		}

		private void ToggleUseIrrelevantSubConfigs()
		{
			this.IsHidingIrrelevantSubConfigs = !this.IsHidingIrrelevantSubConfigs;
		}

		public bool IsHidingIrrelevantSubConfigs
		{
			get
			{
				return this._isHidingIrrelevantSubConfigs;
			}
			set
			{
				if (this._isHidingIrrelevantSubConfigs != value)
				{
					this._isHidingIrrelevantSubConfigs = value;
					RegistryHelper.SetValue("Config", "HideIrrelevantSubConfigs", this._isHidingIrrelevantSubConfigs ? 1 : 0);
				}
			}
		}

		public bool IsServiceInited { get; set; }

		public async Task<bool> WaitForServiceInited()
		{
			Tracer.TraceWrite("GameProfilesService.WaitForServiceInited", false);
			uint curWaitTime = 0U;
			while (!this.IsServiceInited && curWaitTime < 600000U)
			{
				await Task.Delay(50);
				curWaitTime += 50U;
			}
			bool flag;
			if (curWaitTime >= 600000U)
			{
				Tracer.TraceWrite("GameProfilesService.WaitForServiceInited wait timeout exceeded", false);
				flag = false;
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		private async Task InitService()
		{
			try
			{
				await this.FillGamesCollection();
			}
			catch (Exception ex)
			{
				Tracer.TraceException(ex, "InitService");
			}
		}

		public GameProfilesService(IConfigFileService ifs, IEventAggregator ea, Lazy<IGamepadService> gsLazy, ILicensingService ls)
		{
			Tracer.TraceWrite("Constructor for GameProfilesService", false);
			this._configFileService = ifs;
			this._gamepadServiceLazy = gsLazy;
			this._eventAggregator = ea;
			this._licensingService = ls;
			this._eventAggregator.GetEvent<RequestReloadConfig>().Subscribe(new Action<string>(this.ReLoadConfigIfLoadedByPath));
			this._eventAggregator.GetEvent<ConfigsFolderChanged>().Subscribe(new Action<string>(this.ConfigsFolderChanged));
			this._isHidingIrrelevantSubConfigs = RegistryHelper.GetValue("Config", "HideIrrelevantSubConfigs", 1, false) == 1;
			Task.Factory.StartNew<Task>(async delegate
			{
				await Task.Delay(100);
				await this.InitService();
			});
		}

		private void ConfigsFolderChanged(string newFolder)
		{
			foreach (Game game in this.GamesCollection)
			{
				foreach (Config config in game.ConfigCollection)
				{
					config.ConfigPath = config.ConfigPath.Replace(this.GamesFolderPath, newFolder);
				}
			}
		}

		private void ReLoadConfigIfLoadedByPath(string path)
		{
			Config config = this.FindConfigByPath(path, false);
			if (config == null)
			{
				return;
			}
			config.ReadConfigFromJson(false, true);
		}

		public bool DeleteGame(Game game)
		{
			string gameFolderPath = game.GetGameFolderPath();
			try
			{
				DSUtils.RemoveReadOnlyFlag(new DirectoryInfo(gameFolderPath));
				Directory.Delete(gameFolderPath, true);
			}
			catch (Exception)
			{
				return false;
			}
			foreach (Config config in game.ConfigCollection)
			{
				Lazy<IGamepadService> gamepadServiceLazy = Engine.GamepadServiceLazy;
				if (gamepadServiceLazy != null)
				{
					IGamepadService value = gamepadServiceLazy.Value;
					if (value != null)
					{
						value.RemoveRelationsForConfigFile(config.ConfigPath);
					}
				}
			}
			this.GamesCollection.Remove(game);
			if (Engine.GamepadServiceLazy.Value.AutoGamesDetectionGamepadProfileRelations.Remove(game.Name))
			{
				Engine.GamepadServiceLazy.Value.BinDataSerialize.SaveAutoGamesDetectionGamepadProfileRelations();
			}
			SenderGoogleAnalytics.SendMessageEvent("GUI", "ProfileRemoved", game.Name, -1L, false);
			Lazy<IGamepadService> gamepadServiceLazy2 = Engine.GamepadServiceLazy;
			if (gamepadServiceLazy2 != null)
			{
				IGamepadService value2 = gamepadServiceLazy2.Value;
				if (value2 != null)
				{
					ExternalDeviceRelationsHelper externalDeviceRelationsHelper = value2.ExternalDeviceRelationsHelper;
					if (externalDeviceRelationsHelper != null)
					{
						externalDeviceRelationsHelper.RemoveGameRelations(game.Name);
					}
				}
			}
			return true;
		}

		public bool CreateNewGame(string name, string imageSourcePath, ICollection<string> applicationNames, bool createDefaultProfile, out string errorText, string defaultProfileName = "")
		{
			string text = Path.Combine(this.GamesFolderPath, name);
			string text2 = text + "\\Controller";
			errorText = "";
			try
			{
				DirectorySecurity directorySecurity = new DirectorySecurity();
				directorySecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
				Directory.CreateDirectory(text);
				new DirectoryInfo(text).SetAccessControl(directorySecurity);
				Directory.CreateDirectory(text2);
				new DirectoryInfo(text2).SetAccessControl(directorySecurity);
				new DirectoryInfo(text).Attributes = FileAttributes.Normal;
				new DirectoryInfo(text2).Attributes = FileAttributes.Normal;
				if (createDefaultProfile)
				{
					string text3 = text2 + "\\Config 1.rewasd";
					if (!string.IsNullOrEmpty(defaultProfileName))
					{
						text3 = text2 + "\\" + defaultProfileName + ".rewasd";
					}
					using (FileStream fileStream = File.Create(text3))
					{
						string defaultConfig = this._configFileService.GetDefaultConfig();
						byte[] bytes = new UTF8Encoding(true).GetBytes(defaultConfig);
						fileStream.Write(bytes, 0, bytes.Length);
					}
					ConfigData configData = XBUtils.CreateConfigData(false);
					string text4 = text2 + "\\Config 1.rewasd";
					ConfigJSON_3_0 configJSON_3_ = new ConfigJSON_3_0();
					try
					{
						configJSON_3_.ReadBindingsFromFile(text4, configData, false, null);
						configData.ForEach(delegate(SubConfigData bc)
						{
							bc.MainXBBindingCollection.AppName = name;
						});
						configData.ForEach(delegate(SubConfigData bc)
						{
							bc.MainXBBindingCollection.ProcessNames = new List<string>(applicationNames);
						});
						configData.VirtualGamepadType = ConfigData.ConfigGamepadType;
						Exception ex;
						configJSON_3_.WriteBindingsToFile(text4, configData, ref ex);
						DSUtils.GrantAccess(text4);
						DSUtils.GrantAccessFolder(text2);
						DSUtils.GrantAccessFolder(text);
					}
					catch (Exception ex2)
					{
						Tracer.TraceException(ex2, "CreateNewGame");
					}
				}
				SenderGoogleAnalytics.SendMessageEvent("GUI", "ProfileCreated", name, -1L, false);
			}
			catch (Exception ex3)
			{
				Tracer.TraceException(ex3, "CreateNewGame");
				errorText = (RegistryHelper.GetWinDefenderControlledForlder() ? DTLocalization.GetString(11852) : DTLocalization.GetString(12467));
				return false;
			}
			this.AddGameAndProfiles(name);
			return true;
		}

		public void AddGameProfile()
		{
			string sName = "";
			string text = "";
			ObservableCollection<string> observableCollection = new ObservableCollection<string>();
			IGuiHelperService guiHelperService = Engine.GuiHelperService;
			if (guiHelperService != null && guiHelperService.ShowDialogAddEditGame(ref sName, ref text, ref observableCollection, -1) && sName.Length > 0 && this.GamesCollection.FirstOrDefault((Game vm) => vm.Name.ToLower().Equals(sName.ToLower())) == null)
			{
				string text2;
				this.CreateNewGame(sName, text, observableCollection, true, out text2, "");
				if (this.GamesCollection.FirstOrDefault((Game vm) => vm.Name.Equals(sName)) != null)
				{
					SenderGoogleAnalytics.SendMessageEvent("GUI", "ProfileNumber", this.GamesCollection.Count.ToString(), -1L, false);
				}
			}
		}

		public string GamesFolderPath
		{
			get
			{
				return Engine.UserSettingsService.ConfigsFolderPath;
			}
		}

		public void FillPresets()
		{
			this.PresetsCollection.Clear();
			string presetsFolderPath = Engine.UserSettingsService.PresetsFolderPath;
			if (!Directory.Exists(presetsFolderPath))
			{
				return;
			}
			using (IEnumerator<string> enumerator = Directory.EnumerateFiles(presetsFolderPath).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string config = enumerator.Current;
					if (config.EndsWith(".rewasd"))
					{
						Config config2 = new Config(config, this._configFileService, this, this._gamepadServiceLazy, this._licensingService, null);
						config2.GameName = "Preset";
						try
						{
							if (this.PresetsCollection.Where((Config item) => item.ConfigPath == config).FirstOrDefault<Config>() == null)
							{
								this.PresetsCollection.Add(config2);
							}
						}
						catch (Exception ex)
						{
							Tracer.TraceException(ex, "FillPresets");
						}
					}
				}
			}
			XBUtils.SortObservableCollection<Config>(this.PresetsCollection);
		}

		public async Task FillGamesCollection()
		{
			this.IsServiceInited = false;
			this.GamesCollection.Clear();
			string gamesFolderPath = this.GamesFolderPath;
			if (!Directory.Exists(gamesFolderPath))
			{
				try
				{
					Directory.CreateDirectory(gamesFolderPath);
				}
				catch (Exception ex)
				{
					Tracer.TraceException(ex, "FillGamesCollection");
				}
				this.IsServiceInited = true;
			}
			else
			{
				ObservableCollection<Game> gamesCollection = new ObservableCollection<Game>();
				foreach (string text in Directory.EnumerateDirectories(gamesFolderPath))
				{
					Game game = new Game(new DirectoryInfo(text).Name, this, this._configFileService, this._eventAggregator, this._gamepadServiceLazy, this._licensingService);
					gamesCollection.Add(game);
				}
				List<Task> list = new List<Task>();
				using (IEnumerator<Game> enumerator2 = gamesCollection.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Game gameVM = enumerator2.Current;
						list.Add(Task.Factory.StartNew(delegate
						{
							gameVM.FillConfigsCollection();
						}));
					}
				}
				list.Add(Task.Factory.StartNew(delegate
				{
					this.FillPresets();
				}));
				await Task.WhenAll(list.ToArray());
				CollectionExtensions.AddRange<Game>(this.GamesCollection, gamesCollection);
				this.IsServiceInited = true;
			}
		}

		public void AddGameAndProfiles(string name)
		{
			Game game = new Game(name, this, this._configFileService, this._eventAggregator, this._gamepadServiceLazy, this._licensingService);
			game.FillConfigsCollection();
			this.GamesCollection.Add(game);
			XBUtils.SortObservableCollection<Game>(this.GamesCollection);
		}

		public Config FindConfigByPath(string configPath, bool newlyAddedConfig = false)
		{
			if (string.IsNullOrWhiteSpace(configPath) || !File.Exists(configPath))
			{
				return null;
			}
			if (newlyAddedConfig)
			{
				string gamePath = configPath.Remove(configPath.LastIndexOf('\\'));
				gamePath = gamePath.Remove(gamePath.LastIndexOf('\\'));
				Game game = this.GamesCollection.FirstOrDefault((Game g) => g.GetGameFolderPath() == gamePath);
				if (game != null)
				{
					game.FillSingleConfig(configPath);
				}
			}
			return (from g in this.GamesCollection
				from p in g.ConfigCollection
				where p.ConfigPath.ToLower() == configPath.ToLower()
				select p).FirstOrDefault<Config>();
		}

		public const ushort MINIMUM_STICK_ZONE_LOW = 100;

		public const ushort MAXIMUM_STICK_ZONE_HIGH = 100;

		public const ushort MINIMUM_TRIGGER_ZONE_LOW = 100;

		public const ushort MAXIMUM_TRIGGER_ZONE_HIGH = 100;

		private IConfigFileService _configFileService;

		private IEventAggregator _eventAggregator;

		private Lazy<IGamepadService> _gamepadServiceLazy;

		private ILicensingService _licensingService;

		private bool _isHidingIrrelevantSubConfigs;

		private DelegateCommand _toggleUseIrrelevantSubConfigs;

		private const uint INIT_WAIT_TIMEOUT = 600000U;
	}
}
