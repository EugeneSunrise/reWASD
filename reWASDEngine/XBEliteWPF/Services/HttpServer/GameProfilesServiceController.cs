using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using Microsoft.AspNetCore.Mvc;
using reWASDCommon.Infrastructure.RadialMenu.Utils;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using reWASDEngine;
using XBEliteWPF.DataModels;
using XBEliteWPF.DataModels.GamepadActiveProfiles;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.ExternalDeviceRelations;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils.XBUtilModel;

namespace XBEliteWPF.Services.HttpServer
{
	[Route("v1.7/GameProfilesService")]
	public class GameProfilesServiceController : ControllerBase
	{
		[HttpGet]
		[Route("GamesCollection")]
		public IActionResult GetGamesCollection()
		{
			List<GameInfo> list = new List<GameInfo>();
			IGameProfilesService gameProfilesService = Engine.GameProfilesService;
			foreach (Game game in ((gameProfilesService != null) ? gameProfilesService.GamesCollection : null))
			{
				List<ConfigInfo> list2 = new List<ConfigInfo>();
				foreach (Config config in game.ConfigCollection)
				{
					list2.Add(new ConfigInfo(config.GameName, config.Name, config.ConfigPath, config.IsAutodetectEnabledForAnySlot));
				}
				list.Add(new GameInfo(game.Name, game.GetImageSourcePath(), list2));
			}
			return this.Ok(list);
		}

		[HttpGet]
		[Route("PresetsCollection/{forceRefresh}")]
		public IActionResult GetPresetsCollection(bool forceRefresh)
		{
			if (forceRefresh)
			{
				IGameProfilesService gameProfilesService = Engine.GameProfilesService;
				if (gameProfilesService != null)
				{
					gameProfilesService.FillPresets();
				}
			}
			List<ConfigInfo> list = new List<ConfigInfo>();
			IGameProfilesService gameProfilesService2 = Engine.GameProfilesService;
			foreach (Config config in ((gameProfilesService2 != null) ? gameProfilesService2.PresetsCollection : null))
			{
				list.Add(new ConfigInfo(config.GameName, config.Name, config.ConfigPath, false));
			}
			return this.Ok(list);
		}

		[HttpPost]
		[Route("ConfigsCollection")]
		public IActionResult GetConfigsCollection([FromBody] string gameName)
		{
			IGameProfilesService gameProfilesService = Engine.GameProfilesService;
			Game game = ((gameProfilesService != null) ? gameProfilesService.GamesCollection.FirstOrDefault((Game g) => g.Name.Equals(gameName)) : null);
			if (game == null)
			{
				return this.NotFound();
			}
			List<ConfigInfo> list = new List<ConfigInfo>();
			foreach (Config config in game.ConfigCollection)
			{
				list.Add(new ConfigInfo(config.GameName, config.Name, config.ConfigPath, config.IsAutodetectEnabledForAnySlot));
			}
			return this.Ok(list);
		}

		[HttpPost]
		[Route("CreateNewGame")]
		public IActionResult CreateNewGame([FromBody] NewGameParams gameParams)
		{
			string text;
			bool flag = Engine.GameProfilesService.CreateNewGame(gameParams.Name, gameParams.ImageSourcePath, gameParams.ApplicationNames, gameParams.CreateDefaultProfile, out text, gameParams.DefaultProfileName);
			return this.Ok(new ResponseWithError
			{
				Result = flag,
				ErrorText = text
			});
		}

		[HttpPost]
		[Route("CreateNewConfig")]
		public IActionResult CreateNewConfig([FromBody] ConfigParams configParams)
		{
			IGameProfilesService gameProfilesService = Engine.GameProfilesService;
			Game game = ((gameProfilesService != null) ? gameProfilesService.GamesCollection.FirstOrDefault((Game g) => g.Name.Equals(configParams.GameName)) : null);
			if (game == null)
			{
				return this.NotFound();
			}
			bool flag = game.CreateNewConfig(configParams.ConfigName);
			string text = "";
			if (!flag)
			{
				text = (RegistryHelper.GetWinDefenderControlledForlder() ? DTLocalization.GetString(11852) : DTLocalization.GetString(12465));
			}
			return this.Ok(new ResponseWithError
			{
				Result = flag,
				ErrorText = text
			});
		}

		[HttpPost]
		[Route("RemovePreset")]
		public IActionResult RemovePreset([FromBody] DeleteConfigParams deleteParams)
		{
			IGameProfilesService gameProfilesService = Engine.GameProfilesService;
			Config config = ((gameProfilesService != null) ? gameProfilesService.PresetsCollection.FirstOrDefault((Config g) => g.Name.Equals(deleteParams.ConfigName)) : null);
			if (config == null)
			{
				return this.NotFound();
			}
			bool flag = true;
			try
			{
				System.IO.File.Delete(config.ConfigPath);
			}
			catch (Exception)
			{
				flag = false;
			}
			if (flag)
			{
				Engine.GameProfilesService.PresetsCollection.Remove(config);
			}
			return this.Ok(new ResponseWithError
			{
				Result = flag,
				ErrorText = ""
			});
		}

		[HttpPost]
		[Route("DeleteConfig")]
		public IActionResult DeleteConfig([FromBody] DeleteConfigParams deleteParams)
		{
			IGameProfilesService gameProfilesService = Engine.GameProfilesService;
			Game game = ((gameProfilesService != null) ? gameProfilesService.GamesCollection.FirstOrDefault((Game g) => g.Name.Equals(deleteParams.GameName)) : null);
			if (game == null)
			{
				return this.NotFound();
			}
			Config config = game.ConfigCollection.FirstOrDefault((Config c) => c.Name.Equals(deleteParams.ConfigName));
			if (config == null)
			{
				return this.NotFound();
			}
			bool flag = game.DeleteConfig(config);
			string text = "";
			if (flag)
			{
				Engine.EventAggregator.GetEvent<ConfigDeleted>().Publish(deleteParams);
			}
			else
			{
				text = (RegistryHelper.GetWinDefenderControlledForlder() ? DTLocalization.GetString(11852) : DTLocalization.GetString(11017));
			}
			return this.Ok(new ResponseWithError
			{
				Result = flag,
				ErrorText = text
			});
		}

		[HttpPost]
		[Route("CopyDirectory")]
		public IActionResult CopyDirectory([FromBody] CopyDirectoryParams copyParams)
		{
			try
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(copyParams.SourceDir);
				if (!directoryInfo.Exists)
				{
					throw new DirectoryNotFoundException("Source directory not found: " + directoryInfo.FullName);
				}
				DirectoryInfo[] directories = directoryInfo.GetDirectories();
				string text = "";
				int num = 1;
				while (new DirectoryInfo(copyParams.DestinationDir + text).Exists)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 1);
					defaultInterpolatedStringHandler.AppendLiteral(" (");
					defaultInterpolatedStringHandler.AppendFormatted<int>(num);
					defaultInterpolatedStringHandler.AppendLiteral(")");
					text = defaultInterpolatedStringHandler.ToStringAndClear();
					num++;
				}
				copyParams.DestinationDir += text;
				Directory.CreateDirectory(copyParams.DestinationDir);
				foreach (FileInfo fileInfo in directoryInfo.GetFiles())
				{
					string text2 = XBUtils.NormalizeToMaxPathTrimFilename(copyParams.DestinationDir, Path.GetFileNameWithoutExtension(fileInfo.Name).Trim(), ".rewasd", null) + ".rewasd";
					string text3 = Path.Combine(copyParams.DestinationDir, text2);
					fileInfo.CopyTo(text3);
				}
				if (copyParams.Recursive)
				{
					foreach (DirectoryInfo directoryInfo2 in directories)
					{
						copyParams.DestinationDir = Path.Combine(copyParams.DestinationDir, directoryInfo2.Name);
						copyParams.SourceDir = directoryInfo2.FullName;
						this.CopyDirectory(copyParams);
					}
				}
			}
			catch (Exception)
			{
				copyParams.ErrorText = (RegistryHelper.GetWinDefenderControlledForlder() ? DTLocalization.GetString(11852) : DTLocalization.GetString(12467));
			}
			return this.Ok(copyParams);
		}

		[HttpPost]
		[Route("RenameGame")]
		public IActionResult RenameGame([FromBody] RenameGameParams configParams)
		{
			IGameProfilesService gameProfilesService = Engine.GameProfilesService;
			Game game = ((gameProfilesService != null) ? gameProfilesService.GamesCollection.FirstOrDefault((Game g) => g.Name.Equals(configParams.GameName)) : null);
			if (game == null)
			{
				return this.NotFound();
			}
			if (!game.Name.Equals(configParams.NewGameName))
			{
				try
				{
					Directory.Move(Path.Combine(Engine.GameProfilesService.GamesFolderPath, game.Name), Engine.GameProfilesService.GamesFolderPath + "\\" + configParams.NewGameName);
					game.Name = configParams.NewGameName;
					foreach (Config config in game.ConfigCollection)
					{
						config.GameName = game.Name;
						config.ConfigPath = Path.Combine(Engine.UserSettingsService.ConfigsFolderPath, game.Name + "\\Controller\\" + Path.GetFileName(config.ConfigPath));
					}
					Engine.EventAggregator.GetEvent<GameRenamed>().Publish(configParams);
				}
				catch (Exception ex)
				{
					game.Name = configParams.GameName;
					Tracer.TraceException(ex, "RenameGame");
					string text = (RegistryHelper.GetWinDefenderControlledForlder() ? DTLocalization.GetString(11852) : DTLocalization.GetString(11014));
					return this.Ok(new ResponseWithError
					{
						Result = false,
						ErrorText = text
					});
				}
				XBUtils.SortObservableCollection<Game>(Engine.GameProfilesService.GamesCollection);
				GamepadProfilesCollection gamepadProfilesCollection = Engine.GamepadServiceLazy.Value.AutoGamesDetectionGamepadProfileRelations.TryGetValue(configParams.GameName);
				if (gamepadProfilesCollection != null)
				{
					gamepadProfilesCollection.SetGameName(configParams.NewGameName);
					Engine.GamepadServiceLazy.Value.AutoGamesDetectionGamepadProfileRelations[configParams.NewGameName] = gamepadProfilesCollection;
				}
				Lazy<IGamepadService> gamepadServiceLazy = Engine.GamepadServiceLazy;
				if (gamepadServiceLazy == null)
				{
					goto IL_21A;
				}
				IGamepadService value = gamepadServiceLazy.Value;
				if (value == null)
				{
					goto IL_21A;
				}
				ExternalDeviceRelationsHelper externalDeviceRelationsHelper = value.ExternalDeviceRelationsHelper;
				if (externalDeviceRelationsHelper == null)
				{
					goto IL_21A;
				}
				externalDeviceRelationsHelper.RenameGameRelations(configParams.GameName, configParams.NewGameName);
			}
			IL_21A:
			Engine.GamepadServiceLazy.Value.BinDataSerialize.SaveAutoGamesDetectionGamepadProfileRelations();
			return this.Ok(new ResponseWithError
			{
				Result = true
			});
		}

		[HttpPost]
		[Route("RenameConfig")]
		public IActionResult RenameConfig([FromBody] RenameConfigParams configParams)
		{
			IGameProfilesService gameProfilesService = Engine.GameProfilesService;
			Game game = ((gameProfilesService != null) ? gameProfilesService.GamesCollection.FirstOrDefault((Game g) => g.Name.Equals(configParams.GameName)) : null);
			if (game == null)
			{
				return this.NotFound();
			}
			Config config = game.ConfigCollection.FirstOrDefault((Config c) => c.Name.Equals(configParams.Name));
			if (config == null)
			{
				return this.NotFound();
			}
			try
			{
				config.Name = configParams.NewName;
				string directoryName = Path.GetDirectoryName(config.ConfigPath);
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(config.ConfigPath);
				string text = directoryName + "\\" + fileNameWithoutExtension + ".rewasd.bak";
				string text2 = directoryName + "\\" + fileNameWithoutExtension + ".rewasd";
				List<string> list = new List<string>();
				list.Add(".rewasd");
				list.Add(".rewasd.bak");
				string text3 = list.OrderByDescending((string x) => x.Length).First<string>();
				config.Name = XBUtils.NormalizeToMaxPathTrimFilename(directoryName, config.Name.Trim(), text3, null);
				if (System.IO.File.Exists(text))
				{
					System.IO.File.Move(text, directoryName + "\\" + config.Name + ".rewasd.bak");
					System.IO.File.SetAttributes(directoryName + "\\" + config.Name + ".rewasd.bak", FileAttributes.Normal);
				}
				if (System.IO.File.Exists(text2))
				{
					string text4 = directoryName + "\\" + config.Name + ".rewasd";
					System.IO.File.Move(text2, text4);
					System.IO.File.SetAttributes(text4, FileAttributes.Normal);
					config.ConfigPath = text4;
				}
				XBUtils.SortObservableCollection<Config>(game.ConfigCollection);
				string text5 = ((config.ParentGame != null) ? config.ParentGame.Name : config.GameName);
				Lazy<IGamepadService> gamepadServiceLazy = Engine.GamepadServiceLazy;
				if (gamepadServiceLazy != null)
				{
					IGamepadService value = gamepadServiceLazy.Value;
					if (value != null)
					{
						ExternalDeviceRelationsHelper externalDeviceRelationsHelper = value.ExternalDeviceRelationsHelper;
						if (externalDeviceRelationsHelper != null)
						{
							externalDeviceRelationsHelper.RenameConfigRelations(text5, configParams.Name, config.Name);
						}
					}
				}
				configParams.NewName = config.Name;
				Engine.EventAggregator.GetEvent<ConfigRenamed>().Publish(configParams);
				return this.Ok(new ResponseWithError
				{
					Result = true
				});
			}
			catch
			{
			}
			config.Name = configParams.Name;
			string text6 = (RegistryHelper.GetWinDefenderControlledForlder() ? DTLocalization.GetString(11852) : DTLocalization.GetString(12466));
			return this.Ok(new ResponseWithError
			{
				Result = false,
				ErrorText = text6
			});
		}

		[HttpPost]
		[Route("RenamePreset")]
		public IActionResult RenamePreset([FromBody] RenameConfigParams configParams)
		{
			IGameProfilesService gameProfilesService = Engine.GameProfilesService;
			Config config = ((gameProfilesService != null) ? gameProfilesService.PresetsCollection.FirstOrDefault((Config c) => c.Name.Equals(configParams.Name)) : null);
			if (config == null)
			{
				return this.NotFound();
			}
			try
			{
				config.Name = configParams.NewName;
				string directoryName = Path.GetDirectoryName(config.ConfigPath);
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(config.ConfigPath);
				string text = directoryName + "\\" + fileNameWithoutExtension + ".rewasd.bak";
				string text2 = directoryName + "\\" + fileNameWithoutExtension + ".rewasd";
				List<string> list = new List<string>();
				list.Add(".rewasd");
				list.Add(".rewasd.bak");
				string text3 = list.OrderByDescending((string x) => x.Length).First<string>();
				config.Name = XBUtils.NormalizeToMaxPathTrimFilename(directoryName, config.Name.Trim(), text3, null);
				if (System.IO.File.Exists(text))
				{
					System.IO.File.Move(text, directoryName + "\\" + config.Name + ".rewasd.bak");
					System.IO.File.SetAttributes(directoryName + "\\" + config.Name + ".rewasd.bak", FileAttributes.Normal);
				}
				if (System.IO.File.Exists(text2))
				{
					string text4 = directoryName + "\\" + config.Name + ".rewasd";
					System.IO.File.Move(text2, text4);
					System.IO.File.SetAttributes(text4, FileAttributes.Normal);
					config.ConfigPath = text4;
				}
				Lazy<IGamepadService> gamepadServiceLazy = Engine.GamepadServiceLazy;
				if (gamepadServiceLazy != null)
				{
					IGamepadService value = gamepadServiceLazy.Value;
					if (value != null)
					{
						ExternalDeviceRelationsHelper externalDeviceRelationsHelper = value.ExternalDeviceRelationsHelper;
						if (externalDeviceRelationsHelper != null)
						{
							externalDeviceRelationsHelper.RenameConfigRelations("Preset", configParams.Name, config.Name);
						}
					}
				}
				configParams.NewName = config.Name;
				Engine.EventAggregator.GetEvent<PresetRenamed>().Publish(configParams);
				Engine.GameProfilesService.FillPresets();
				return this.Ok(new ResponseWithError
				{
					Result = true
				});
			}
			catch
			{
			}
			config.Name = configParams.Name;
			string text5 = (RegistryHelper.GetWinDefenderControlledForlder() ? DTLocalization.GetString(11852) : DTLocalization.GetString(12466));
			return this.Ok(new ResponseWithError
			{
				Result = false,
				ErrorText = text5
			});
		}

		[HttpPost]
		[Route("DeleteGame")]
		public IActionResult DeleteGame([FromBody] Tuple<string, string> parameters)
		{
			string gameName = parameters.Item2;
			IGameProfilesService gameProfilesService = Engine.GameProfilesService;
			Game game = ((gameProfilesService != null) ? gameProfilesService.GamesCollection.FirstOrDefault((Game g) => g.Name.Equals(gameName)) : null);
			if (game == null)
			{
				return this.NotFound();
			}
			IGameProfilesService gameProfilesService2 = Engine.GameProfilesService;
			bool flag = gameProfilesService2 != null && gameProfilesService2.DeleteGame(game);
			string text = "";
			if (flag)
			{
				Engine.EventAggregator.GetEvent<GameDeleted>().Publish(parameters);
			}
			else
			{
				text = (RegistryHelper.GetWinDefenderControlledForlder() ? DTLocalization.GetString(11852) : DTLocalization.GetString(11017));
			}
			return this.Ok(new ResponseWithError
			{
				Result = flag,
				ErrorText = text
			});
		}

		[HttpPost]
		[Route("SavePreset")]
		public IActionResult SavePreset([FromBody] SaveConfigParams<ConfigData> configParams)
		{
			IGameProfilesService gameProfilesService = Engine.GameProfilesService;
			if (((gameProfilesService != null) ? gameProfilesService.PresetsCollection.FirstOrDefault((Config g) => g.Name.Equals(configParams.ConfigName)) : null) != null)
			{
				return this.NotFound();
			}
			string presetsFolderPath = Engine.UserSettingsService.PresetsFolderPath;
			if (!Directory.Exists(presetsFolderPath))
			{
				Directory.CreateDirectory(presetsFolderPath);
			}
			List<string> list = new List<string>();
			list.Add(".rewasd");
			list.Add(".rewasd.bak");
			string text = list.OrderByDescending((string x) => x.Length).First<string>();
			string text2 = XBUtils.NormalizeToMaxPathTrimFilename(presetsFolderPath, configParams.ConfigName.Trim(), text, null);
			new Config(Path.Combine(presetsFolderPath, text2) + ".rewasd", Engine.ConfigFileService, Engine.GameProfilesService, Engine.GamepadServiceLazy, Engine.LicensingService, null).SaveConfig(configParams.ConfigData, configParams.ClientID);
			IGameProfilesService gameProfilesService2 = Engine.GameProfilesService;
			if (gameProfilesService2 != null)
			{
				gameProfilesService2.FillPresets();
			}
			return this.Ok(new BaseResponse
			{
				Result = true
			});
		}

		[HttpPost]
		[Route("SaveOverlayPreset")]
		public IActionResult SaveOverlayPreset([FromBody] SaveConfigParams<ConfigData> configParams)
		{
			RadialMenuPresetHelper.Save(configParams.ConfigData);
			return this.Ok(new BaseResponse
			{
				Result = true
			});
		}

		[HttpPost]
		[Route("SaveConfig")]
		public IActionResult SaveConfig([FromBody] SaveConfigParams<ConfigData> configParams)
		{
			IGameProfilesService gameProfilesService = Engine.GameProfilesService;
			Game game = ((gameProfilesService != null) ? gameProfilesService.GamesCollection.FirstOrDefault((Game g) => g.Name.Equals(configParams.GameName)) : null);
			if (game == null)
			{
				return this.NotFound();
			}
			Config config = game.ConfigCollection.FirstOrDefault((Config c) => c.Name.Equals(configParams.ConfigName));
			if (config == null)
			{
				return this.NotFound();
			}
			config.SaveConfig(configParams.ConfigData, configParams.ClientID);
			return this.Ok(new BaseResponse
			{
				Result = true
			});
		}

		[HttpPost]
		[Route("ImportConfig")]
		public IActionResult ImportConfig([FromBody] ImportConfigInfo configParams)
		{
			IGameProfilesService gameProfilesService = Engine.GameProfilesService;
			Game game = ((gameProfilesService != null) ? gameProfilesService.GamesCollection.FirstOrDefault((Game g) => g.Name.Equals(configParams.GameName)) : null);
			if (game == null)
			{
				return this.NotFound();
			}
			return this.Ok(game.ImportConfig(configParams.Name, configParams.Content, configParams.IsCloning, configParams.RewriteExisting));
		}

		[HttpPost]
		[Route("Config")]
		public IActionResult GetConfig([FromBody] ConfigParams configParams)
		{
			if (configParams == null)
			{
				return this.BadRequest();
			}
			IGameProfilesService gameProfilesService = Engine.GameProfilesService;
			Game game = ((gameProfilesService != null) ? gameProfilesService.GamesCollection.FirstOrDefault((Game g) => g.Name.Equals(configParams.GameName)) : null);
			if (game == null)
			{
				return this.NotFound();
			}
			Config config = game.ConfigCollection.FirstOrDefault((Config c) => c.Name.Equals(configParams.ConfigName));
			if (config == null)
			{
				return this.NotFound();
			}
			bool flag = config.ReadConfigFromJsonIfNotLoaded(true);
			ConfigData configData = config.ConfigData;
			if (flag)
			{
				return this.Ok(config.ConfigData);
			}
			return this.NotFound();
		}

		[HttpPost]
		[Route("CreateOverlayCirclePreview")]
		public async Task<IActionResult> CreateOverlayCirclePreview([FromBody] OverlayCirclePreviewInfo previewParams)
		{
			return this.Ok(new OverlayCirclePreviewResult
			{
				PreviewWindow = null
			});
		}

		[HttpGet]
		[Route("Background/{path}")]
		public IActionResult GetBackground(string path)
		{
			if (!System.IO.File.Exists(path))
			{
				return this.NotFound();
			}
			new HttpResponseMessage(HttpStatusCode.OK);
			string text = "application/....";
			return new FileStreamResult(new FileStream(path, FileMode.Open, FileAccess.Read), text)
			{
				FileDownloadName = path
			};
		}
	}
}
