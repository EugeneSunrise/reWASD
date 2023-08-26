using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.View.SecondaryWindows;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using DiscSoft.NET.Common.View.SecondaryWindows.WaitDialog;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using Prism.Commands;
using Prism.Events;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using reWASDEngine.Services.HttpServer.Data;
using reWASDUI.Controls.XBBindingControls.BindingFrame.BindingFrameViews;
using reWASDUI.DataModels;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.ActivatorXB;
using reWASDUI.Infrastructure.KeyBindings.ControllerBindings;
using reWASDUI.Infrastructure.KeyBindings.MacroBinding;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.License.Views;
using reWASDUI.Services.HttpClient;
using reWASDUI.Services.Interfaces;
using reWASDUI.Utils.XBUtil;
using reWASDUI.ViewModels;
using reWASDUI.Views;
using reWASDUI.Views.ContentZoneGamepad;
using reWASDUI.Views.ContentZoneGamepad.AdvancedStick;
using reWASDUI.Views.ContentZoneGamepad.Macro;
using reWASDUI.Views.OverlayMenu;
using reWASDUI.Views.SecondaryWindows;
using reWASDUI.Views.SecondaryWindows.AddExternalDeviceWizard;
using reWASDUI.Views.VirtualSticks;
using XBEliteWPF.License.Licensing.ComStructures;
using XBEliteWPF.Utils;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtilModel;

namespace reWASDUI.Services
{
	public class GuiHelperService : ZBindableBase, IGuiHelperService
	{
		public ImportConfig ImportConfigDialog
		{
			get
			{
				ImportConfig importConfig;
				if ((importConfig = this._importConfigDialog) == null)
				{
					importConfig = (this._importConfigDialog = new ImportConfig());
				}
				return importConfig;
			}
		}

		public ExportConfig ExportGameDialog
		{
			get
			{
				ExportConfig exportConfig;
				if ((exportConfig = this._exportGameDialog) == null)
				{
					exportConfig = (this._exportGameDialog = new ExportConfig());
				}
				return exportConfig;
			}
		}

		public GuiHelperService(IEventAggregator ea)
		{
			ea.GetEvent<RequestBindingFrameBack>().Subscribe(new Action<string>(this.OnRequestBindingFrameBack));
		}

		public async Task<bool> ImportGameConfig(string configPath = "", bool isCloning = false)
		{
			GameVM importToGame = ((configPath != "") ? null : App.GameProfilesService.CurrentGame);
			if (isCloning)
			{
				importToGame = App.GameProfilesService.CurrentGame;
			}
			try
			{
				if (ImportConfig.IsShown)
				{
					return false;
				}
				if (!App.GameProfilesService.IsServiceInited)
				{
					Tracer.TraceWrite("ImportGameConfig: GameProfilesService not inited", false);
					await App.GameProfilesService.WaitForServiceInited();
				}
				this.ImportConfigDialog.PrepareDialog(configPath, importToGame, App.ConfigFileService, isCloning);
				await this.ImportConfigDialog.ShowDialogAsync();
				MessageBoxResult windowResult = this.ImportConfigDialog.WindowResult;
				if (windowResult != MessageBoxResult.OK && windowResult != MessageBoxResult.Yes)
				{
					return false;
				}
				TaskAwaiter<GameChangedResult> taskAwaiter = App.GameProfilesService.TryAskUserToSaveChanges(true).GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<GameChangedResult> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<GameChangedResult>);
				}
				if (taskAwaiter.GetResult() == 3)
				{
					return false;
				}
				WaitDialog.ShowDialogStatic(DTLocalization.GetString(5238), null, null, false, false, null, null);
				GameVM gameVM = this.ImportConfigDialog.Data.SelectedGame;
				if (this.ImportConfigDialog.Data.IsCreateNew)
				{
					await App.GameProfilesService.CreateNewGame(this.ImportConfigDialog.Data.NewProfileName, this.ImportConfigDialog.Data.BoxArtPath, null, false, "");
					gameVM = App.GameProfilesService.GamesCollection.FirstOrDefault((GameVM vm) => vm.Name.Equals(this.ImportConfigDialog.Data.NewProfileName));
				}
				if (gameVM == null)
				{
					WaitDialog.TryCloseWaitDialog();
					return false;
				}
				ConfigVM configVM = await gameVM.ImportConfigFile(this.ImportConfigDialog.Data.ConfigName, this.ImportConfigDialog.Data.ConfigSourcePath, isCloning);
				WaitDialog.TryCloseWaitDialog();
				await App.GameProfilesService.SetCurrentGameAndConfig(configVM, false);
			}
			finally
			{
				this._importConfigDialog = null;
			}
			return true;
		}

		public async Task SaveAsGame(string gamePath = "", bool isCloning = false)
		{
			GameVM gameVM = ((gamePath != "") ? null : App.GameProfilesService.CurrentGame);
			try
			{
				if (!ExportConfig.IsShown)
				{
					this.ExportGameDialog.PrepareDialog(gamePath, gameVM, App.ConfigFileService);
					this.ExportGameDialog.ShowDialog();
					MessageBoxResult windowResult = this.ExportGameDialog.WindowResult;
					if (windowResult == MessageBoxResult.OK || windowResult == MessageBoxResult.Yes)
					{
						TaskAwaiter<GameChangedResult> taskAwaiter = App.GameProfilesService.TryAskUserToSaveChanges(true).GetAwaiter();
						if (!taskAwaiter.IsCompleted)
						{
							await taskAwaiter;
							TaskAwaiter<GameChangedResult> taskAwaiter2;
							taskAwaiter = taskAwaiter2;
							taskAwaiter2 = default(TaskAwaiter<GameChangedResult>);
						}
						if (taskAwaiter.GetResult() == 3)
						{
						}
					}
				}
			}
			catch (Exception)
			{
			}
			finally
			{
				this._exportGameDialog = null;
			}
		}

		public async Task<bool> EditGameAppsExecute(GameVM game)
		{
			GuiHelperService.<>c__DisplayClass9_0 CS$<>8__locals1 = new GuiHelperService.<>c__DisplayClass9_0();
			GuiHelperService.<>c__DisplayClass9_0 CS$<>8__locals2 = CS$<>8__locals1;
			ConfigVM configVM2 = game.ConfigCollection.FirstOrDefault<ConfigVM>();
			ObservableCollection<string> observableCollection;
			if (configVM2 == null)
			{
				observableCollection = null;
			}
			else
			{
				ConfigData configData = configVM2.ConfigData;
				if (configData == null)
				{
					observableCollection = null;
				}
				else
				{
					SubConfigData subConfigData = configData.FirstOrDefault((SubConfigData c) => c.MainXBBindingCollection.ProcessNames.Count > 0);
					observableCollection = ((subConfigData != null) ? subConfigData.MainXBBindingCollection.ProcessNames : null);
				}
			}
			CS$<>8__locals2.collection = observableCollection;
			if (CS$<>8__locals1.collection == null)
			{
				GuiHelperService.<>c__DisplayClass9_0 CS$<>8__locals3 = CS$<>8__locals1;
				ConfigVM configVM3 = game.ConfigCollection.FirstOrDefault<ConfigVM>();
				ObservableCollection<string> observableCollection2;
				if (configVM3 == null)
				{
					observableCollection2 = null;
				}
				else
				{
					ConfigData configData2 = configVM3.ConfigData;
					if (configData2 == null)
					{
						observableCollection2 = null;
					}
					else
					{
						SubConfigData subConfigData2 = configData2.FindGamepadCollection(0, false);
						observableCollection2 = ((subConfigData2 != null) ? subConfigData2.MainXBBindingCollection.ProcessNames : null);
					}
				}
				CS$<>8__locals3.collection = observableCollection2;
			}
			bool flag;
			if (AddEditGame.ShowDlgEditAppsInGame(game.Name, ref CS$<>8__locals1.collection, game.ConfigCollection.Count))
			{
				foreach (ConfigVM configVM in game.ConfigCollection)
				{
					IEnumerable<SubConfigData> enumerable = await configVM.GetConfigData();
					Action<SubConfigData> action;
					if ((action = CS$<>8__locals1.<>9__1) == null)
					{
						GuiHelperService.<>c__DisplayClass9_0 CS$<>8__locals4 = CS$<>8__locals1;
						Action<SubConfigData> action2 = delegate(SubConfigData bc)
						{
							bc.MainXBBindingCollection.ProcessNames = CS$<>8__locals1.collection;
						};
						CS$<>8__locals4.<>9__1 = action2;
						action = action2;
					}
					enumerable.ForEach(action);
					await configVM.SaveConfigToJson();
					configVM = null;
				}
				IEnumerator<ConfigVM> enumerator = null;
				flag = CS$<>8__locals1.collection.Count > 0;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public async void EditGameExecute(GameVM game)
		{
			GuiHelperService.<>c__DisplayClass10_0 CS$<>8__locals1 = new GuiHelperService.<>c__DisplayClass10_0();
			TaskAwaiter<GameChangedResult> taskAwaiter = App.GameProfilesService.TryAskUserToSaveChanges(true).GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<GameChangedResult> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<GameChangedResult>);
			}
			if (taskAwaiter.GetResult() != 3)
			{
				string sOldName = game.Name;
				string sNewName = game.Name;
				string sImageSourcePath = "";
				GuiHelperService.<>c__DisplayClass10_0 CS$<>8__locals2 = CS$<>8__locals1;
				ConfigVM configVM2 = game.ConfigCollection.FirstOrDefault<ConfigVM>();
				ObservableCollection<string> observableCollection;
				if (configVM2 == null)
				{
					observableCollection = null;
				}
				else
				{
					ConfigData configData = configVM2.ConfigData;
					if (configData == null)
					{
						observableCollection = null;
					}
					else
					{
						SubConfigData subConfigData = configData.FirstOrDefault((SubConfigData c) => c.MainXBBindingCollection.ProcessNames.Count > 0);
						observableCollection = ((subConfigData != null) ? subConfigData.MainXBBindingCollection.ProcessNames : null);
					}
				}
				CS$<>8__locals2.collection = observableCollection;
				if (CS$<>8__locals1.collection == null)
				{
					GuiHelperService.<>c__DisplayClass10_0 CS$<>8__locals3 = CS$<>8__locals1;
					ConfigVM configVM3 = game.ConfigCollection.FirstOrDefault<ConfigVM>();
					ObservableCollection<string> observableCollection2;
					if (configVM3 == null)
					{
						observableCollection2 = null;
					}
					else
					{
						ConfigData configData2 = configVM3.ConfigData;
						if (configData2 == null)
						{
							observableCollection2 = null;
						}
						else
						{
							SubConfigData subConfigData2 = configData2.FindGamepadCollection(0, false);
							observableCollection2 = ((subConfigData2 != null) ? subConfigData2.MainXBBindingCollection.ProcessNames : null);
						}
					}
					CS$<>8__locals3.collection = observableCollection2;
				}
				if (AddEditGame.ShowDlgAddEditGame(ref sNewName, ref sImageSourcePath, ref CS$<>8__locals1.collection, game.ConfigCollection.Count))
				{
					foreach (ConfigVM configVM in game.ConfigCollection)
					{
						IEnumerable<SubConfigData> enumerable = await configVM.GetConfigData();
						Action<SubConfigData> action;
						if ((action = CS$<>8__locals1.<>9__1) == null)
						{
							GuiHelperService.<>c__DisplayClass10_0 CS$<>8__locals4 = CS$<>8__locals1;
							Action<SubConfigData> action2 = delegate(SubConfigData bc)
							{
								bc.MainXBBindingCollection.ProcessNames = CS$<>8__locals1.collection;
							};
							CS$<>8__locals4.<>9__1 = action2;
							action = action2;
						}
						enumerable.ForEach(action);
						await configVM.SaveConfigToJson();
						configVM = null;
					}
					IEnumerator<ConfigVM> enumerator = null;
					if (sOldName != sNewName)
					{
						ResponseWithError responseWithError = await App.HttpClientService.GameProfiles.RenameGame(new RenameGameParams
						{
							GameName = sOldName,
							NewGameName = sNewName
						});
						if (!responseWithError.Result && !string.IsNullOrEmpty(responseWithError.ErrorText))
						{
							DTMessageBox.Show(responseWithError.ErrorText, MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
						}
						await App.GameProfilesService.FillGamesCollection();
					}
					if (sImageSourcePath.Length > 0)
					{
						GameVM.SaveGameThumbnail(sImageSourcePath, game.GetImageSourcePath());
						game.SetImageSource(false);
					}
				}
			}
		}

		public async void RemoveGameExecute(GameVM game)
		{
			if (MessageBoxWithDoNotShowLogic.Show(Application.Current.MainWindow, DTLocalization.GetString(11016), MessageBoxButton.YesNo, MessageBoxImage.Question, "RemoveGameOrConfig", MessageBoxResult.Yes, false, 0.0, null, null, null, null, null, null) == MessageBoxResult.Yes)
			{
				try
				{
					ConfigVM currentConfig = game.CurrentConfig;
					if (currentConfig != null)
					{
						ConfigData configData = currentConfig.ConfigData;
						if (configData != null)
						{
							configData.ResetIsChanged();
						}
					}
					ResponseWithError responseWithError = await App.HttpClientService.GameProfiles.DeleteGame(game.Name);
					if (!responseWithError.Result && !string.IsNullOrEmpty(responseWithError.ErrorText))
					{
						DTMessageBox.Show(responseWithError.ErrorText, MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
					}
				}
				catch (Exception ex)
				{
					Tracer.TraceException(ex, "RemoveGameExecute");
					DTMessageBox.Show(DTLocalization.GetString(11017), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
				}
			}
		}

		public async void CloneGameExecute(GameVM game)
		{
			GuiHelperService.<>c__DisplayClass12_0 CS$<>8__locals1 = new GuiHelperService.<>c__DisplayClass12_0();
			CS$<>8__locals1.game = game;
			await App.GameProfilesService.TryAskUserToSaveChanges(true);
			try
			{
				if (CS$<>8__locals1.game != null)
				{
					string text = await this.ExportGameDialog.CopyDirectory(CS$<>8__locals1.game.GetGameFolderPath(), CS$<>8__locals1.game.GetGameFolderPath() + " - Copy", true);
					if (string.IsNullOrEmpty(text))
					{
						throw new ArgumentNullException();
					}
					CS$<>8__locals1.suffix = "";
					int num = 1;
					for (;;)
					{
						IEnumerable<GameVM> enumerable = App.GameProfilesService.GamesCollection.ToList<GameVM>();
						Func<GameVM, bool> func;
						if ((func = CS$<>8__locals1.<>9__0) == null)
						{
							GuiHelperService.<>c__DisplayClass12_0 CS$<>8__locals2 = CS$<>8__locals1;
							Func<GameVM, bool> func2 = (GameVM x) => x.Name == CS$<>8__locals1.game.Name + " - Copy" + CS$<>8__locals1.suffix;
							CS$<>8__locals2.<>9__0 = func2;
							func = func2;
						}
						if (!enumerable.Any(func))
						{
							break;
						}
						GuiHelperService.<>c__DisplayClass12_0 CS$<>8__locals3 = CS$<>8__locals1;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 1);
						defaultInterpolatedStringHandler.AppendLiteral(" (");
						defaultInterpolatedStringHandler.AppendFormatted<int>(num);
						defaultInterpolatedStringHandler.AppendLiteral(")");
						CS$<>8__locals3.suffix = defaultInterpolatedStringHandler.ToStringAndClear();
						num++;
					}
					await App.GameProfilesService.CreateNewGame(CS$<>8__locals1.game.Name + " - Copy" + CS$<>8__locals1.suffix, text + "\\IcoGame.png", null, false, "");
					await App.GameProfilesService.FillGamesCollection();
					await App.GameProfilesService.SetCurrentGame(App.GameProfilesService.GamesCollection.ToList<GameVM>().Find((GameVM x) => x.Name == CS$<>8__locals1.game.Name + " - Copy" + CS$<>8__locals1.suffix), true);
				}
			}
			catch (ArgumentNullException)
			{
			}
		}

		public async void CreateConfigExecute(GameVM game)
		{
			ConfigVM configVM = new ConfigVM("", null, game);
			configVM.InitConfig();
			string gameFolderPath = Path.Combine(App.UserSettingsService.ConfigsFolderPath, game.Name + "\\Controller\\");
			configVM.Name = Path.GetFileNameWithoutExtension(DSUtils.GetUniqueFileName(gameFolderPath, "Config.rewasd"));
			configVM.GameName = game.Name;
			TaskAwaiter<GameChangedResult> taskAwaiter = App.GameProfilesService.TryAskUserToSaveChanges(true).GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<GameChangedResult> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<GameChangedResult>);
			}
			if (taskAwaiter.GetResult() != 3)
			{
				await this.CreateNewConfig(game, configVM, gameFolderPath);
				ConfigVM configVM2 = game.ConfigCollection.FirstOrDefault((ConfigVM x) => x.Name == configVM.Name);
				App.EventAggregator.GetEvent<ConfigCreatedByUI>().Publish(configVM2);
			}
		}

		private async Task CreateNewConfig(GameVM game, ConfigVM configVM, string gameFolderPath)
		{
			configVM.Name = configVM.Name.Trim();
			if (!string.IsNullOrEmpty(configVM.Name))
			{
				if (!Directory.Exists(gameFolderPath))
				{
					try
					{
						Directory.CreateDirectory(gameFolderPath);
					}
					catch (Exception)
					{
						DSUtils.CheckWinDefenderAndShowMessage(DTLocalization.GetString(4276));
						return;
					}
				}
				configVM.ConfigPath = gameFolderPath + XBUtils.NormalizeToMaxPathTrimFilename(gameFolderPath, configVM.Name.Trim(), ".rewasd", null) + ".rewasd";
				if (!File.Exists(configVM.ConfigPath) || DTMessageBox.Show(Application.Current.MainWindow, DTLocalization.GetString(11018), MessageBoxButton.YesNo, MessageBoxImage.Question, null) == MessageBoxResult.Yes)
				{
					try
					{
						ResponseWithError responseWithError = await App.HttpClientService.GameProfiles.CreateNewConfig(new ConfigParams
						{
							GameName = game.Name,
							ConfigName = configVM.Name
						});
						ResponseWithError response = responseWithError;
						if (response.Result)
						{
							await game.FillConfigsCollection();
							await game.SetCurrentConfig(game.ConfigCollection.FirstOrDefault((ConfigVM c) => c.Name == configVM.Name));
						}
						if (!response.Result && !string.IsNullOrEmpty(response.ErrorText))
						{
							DTMessageBox.Show(response.ErrorText, MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
						}
						response = null;
					}
					catch (Exception)
					{
						DSUtils.CheckWinDefenderAndShowMessage(DTLocalization.GetString(11076));
					}
				}
			}
		}

		private async void ImportExistingProfile(GameVM game, string configName, string configFilePath)
		{
			ConfigVM configVM = await game.ImportConfigFile(configName, configFilePath, false);
			if (configVM != null)
			{
				App.GameProfilesService.SetCurrentGameAndProfileByProfilePathCommand.Execute(configVM.ConfigPath);
			}
		}

		public DelegateCommand<object> BindingFrameBackCommand
		{
			get
			{
				DelegateCommand<object> delegateCommand;
				if ((delegateCommand = this._bindingFrameBack) == null)
				{
					delegateCommand = (this._bindingFrameBack = new DelegateCommand<object>(new Action<object>(this.BindingFrameBack)));
				}
				return delegateCommand;
			}
		}

		private void SetButtonMappingByCurrentMapZone(GamepadButton button)
		{
			if (GamepadButtonExtensions.IsPhysicalTrackPad1PressureZone(button))
			{
				App.GameProfilesService.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(93), true);
			}
			if (GamepadButtonExtensions.IsPhysicalTrackPad2PressureZone(button))
			{
				App.GameProfilesService.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(103), true);
			}
			if (GamepadButtonExtensions.IsLeftTriggerZone(button))
			{
				App.GameProfilesService.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(51), true);
			}
			if (GamepadButtonExtensions.IsRightTriggerZone(button))
			{
				App.GameProfilesService.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(55), true);
			}
			if (GamepadButtonExtensions.IsLeftBumperZone(button))
			{
				App.GameProfilesService.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(5), true);
			}
			if (GamepadButtonExtensions.IsRightBumperZone(button))
			{
				App.GameProfilesService.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(6), true);
			}
			if (GamepadButtonExtensions.IsCrossZone(button))
			{
				App.GameProfilesService.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(1), true);
			}
			if (GamepadButtonExtensions.IsTriangleZone(button))
			{
				App.GameProfilesService.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(4), true);
			}
			if (GamepadButtonExtensions.IsCircleZone(button))
			{
				App.GameProfilesService.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(2), true);
			}
			if (GamepadButtonExtensions.IsSquareZone(button))
			{
				App.GameProfilesService.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(3), true);
			}
			if (GamepadButtonExtensions.IsDPADUpZone(button))
			{
				App.GameProfilesService.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(33), true);
			}
			if (GamepadButtonExtensions.IsDPADDownZone(button))
			{
				App.GameProfilesService.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(34), true);
			}
			if (GamepadButtonExtensions.IsDPADLeftZone(button))
			{
				App.GameProfilesService.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(35), true);
			}
			if (GamepadButtonExtensions.IsDPADRightZone(button))
			{
				App.GameProfilesService.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(new GamepadButton?(36), true);
			}
		}

		private void BindingFrameBack(object param)
		{
			bool flag = false;
			BaseXBBindingCollection realCurrentBeingMappedBindingCollection = App.GameProfilesService.RealCurrentBeingMappedBindingCollection;
			if (realCurrentBeingMappedBindingCollection != null)
			{
				XBBinding currentXBBinding = realCurrentBeingMappedBindingCollection.CurrentXBBinding;
				if (currentXBBinding != null)
				{
					ActivatorXBBinding currentActivatorXBBinding = currentXBBinding.CurrentActivatorXBBinding;
					if (currentActivatorXBBinding != null)
					{
						MacroSequence macroSequence = currentActivatorXBBinding.MacroSequence;
						if (macroSequence != null)
						{
							macroSequence.TryPromptToCorrectStickDirections();
						}
					}
				}
			}
			BaseXBBindingCollection realCurrentBeingMappedBindingCollection2 = App.GameProfilesService.RealCurrentBeingMappedBindingCollection;
			bool flag2;
			if (realCurrentBeingMappedBindingCollection2 == null)
			{
				flag2 = false;
			}
			else
			{
				XBBinding currentXBBinding2 = realCurrentBeingMappedBindingCollection2.CurrentXBBinding;
				bool? flag3 = ((currentXBBinding2 != null) ? new bool?(currentXBBinding2.CanSave(true, ref flag)) : null);
				bool flag4 = false;
				flag2 = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
			}
			if (flag2)
			{
				return;
			}
			BaseXBBindingCollection realCurrentBeingMappedBindingCollection3 = App.GameProfilesService.RealCurrentBeingMappedBindingCollection;
			if (realCurrentBeingMappedBindingCollection3 != null && realCurrentBeingMappedBindingCollection3.IsOverlayMenuModeView)
			{
				reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(OverlayMenuView));
				reWASDApplicationCommands.NavigateBindingFrameCommand.Execute(null);
			}
			else
			{
				BaseXBBindingCollection realCurrentBeingMappedBindingCollection4 = App.GameProfilesService.RealCurrentBeingMappedBindingCollection;
				if (((realCurrentBeingMappedBindingCollection4 != null) ? realCurrentBeingMappedBindingCollection4.MaskBindingCollection.CurrentEditItem : null) != null)
				{
					reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(MaskView));
					reWASDApplicationCommands.NavigateBindingFrameCommand.Execute(null);
				}
				else
				{
					BaseXBBindingCollection realCurrentBeingMappedBindingCollection5 = App.GameProfilesService.RealCurrentBeingMappedBindingCollection;
					GamepadButton? gamepadButton = ((realCurrentBeingMappedBindingCollection5 != null) ? realCurrentBeingMappedBindingCollection5.CurrentButtonMappingKey : null);
					if (gamepadButton != null)
					{
						if (GamepadButtonExtensions.IsAnyTriggerZone(gamepadButton.Value) || GamepadButtonExtensions.IsAnyPhysicalTrackPadPressureZone(gamepadButton.Value) || GamepadButtonExtensions.IsDS3AnalogZone(gamepadButton.Value))
						{
							if (App.RegionManager.Regions[RegionNames.Gamepad].ActiveViews.FirstOrDefault<object>() is MacroSettings)
							{
								reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(SVGGamepadWithAllAnnotations));
								reWASDApplicationCommands.NavigateBindingFrameCommand.Execute(typeof(BFAdvanced));
								return;
							}
							this.SetButtonMappingByCurrentMapZone(gamepadButton.Value);
						}
						if (GamepadButtonExtensions.IsAnyStickDiagonalDirection(gamepadButton.Value) || GamepadButtonExtensions.IsAnyPhysicalTrackDiagonalDirection(gamepadButton.Value) || GamepadButtonExtensions.IsAnyStickZone(gamepadButton.Value) || GamepadButtonExtensions.IsGyroTiltZone(gamepadButton.Value) || GamepadButtonExtensions.IsMouseZone(gamepadButton.Value) || GamepadButtonExtensions.IsGyroLean(gamepadButton.Value))
						{
							if (App.RegionManager.Regions[RegionNames.Gamepad].ActiveViews.FirstOrDefault<object>() is MacroSettings)
							{
								reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(AdvancedStickSettings));
								return;
							}
							App.GameProfilesService.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(null);
						}
					}
					BaseXBBindingCollection realCurrentBeingMappedBindingCollection6 = App.GameProfilesService.RealCurrentBeingMappedBindingCollection;
					if (realCurrentBeingMappedBindingCollection6 != null)
					{
						realCurrentBeingMappedBindingCollection6.UpdateAdvanceMappingSettingsIcon();
					}
					BaseXBBindingCollection realCurrentBeingMappedBindingCollection7 = App.GameProfilesService.RealCurrentBeingMappedBindingCollection;
					bool flag5;
					if (realCurrentBeingMappedBindingCollection7 == null)
					{
						flag5 = false;
					}
					else
					{
						ControllerBindingsCollection controllerBindings = realCurrentBeingMappedBindingCollection7.ControllerBindings;
						bool? flag6;
						if (controllerBindings == null)
						{
							flag6 = null;
						}
						else
						{
							ControllerBinding currentEditItem = controllerBindings.CurrentEditItem;
							flag6 = ((currentEditItem != null) ? new bool?(currentEditItem.XBBinding.KeyScanCode.IsKeyboard) : null);
						}
						bool? flag3 = flag6;
						bool flag4 = true;
						flag5 = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
					}
					if (flag5)
					{
						BaseXBBindingCollection realCurrentBeingMappedBindingCollection8 = App.GameProfilesService.RealCurrentBeingMappedBindingCollection;
						if (realCurrentBeingMappedBindingCollection8 == null || realCurrentBeingMappedBindingCollection8.SubConfigData.ControllerFamily != 1)
						{
							reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(KeyboardMappingView));
							return;
						}
					}
					if (App.GameProfilesService.CurrentGame != null)
					{
						GameVM currentGame = App.GameProfilesService.CurrentGame;
						ControllerFamily? controllerFamily;
						if (currentGame == null)
						{
							controllerFamily = null;
						}
						else
						{
							ConfigVM currentConfig = currentGame.CurrentConfig;
							if (currentConfig == null)
							{
								controllerFamily = null;
							}
							else
							{
								SubConfigData currentSubConfigData = currentConfig.CurrentSubConfigData;
								controllerFamily = ((currentSubConfigData != null) ? new ControllerFamily?(currentSubConfigData.ControllerFamily) : null);
							}
						}
						XBUtils.NavigateGamepadZoneForControllerFamily(controllerFamily);
					}
					reWASDApplicationCommands.NavigateBindingFrameCommand.Execute(param);
				}
			}
			GC.Collect();
			GC.WaitForFullGCComplete(-1);
		}

		private void OnRequestBindingFrameBack(string obj)
		{
			if (this.BindingFrameBackCommand.CanExecute(null))
			{
				this.BindingFrameBackCommand.Execute(null);
			}
		}

		public async Task RemovePresetExecute(ConfigVM config)
		{
			ResponseWithError responseWithError = await App.HttpClientService.GameProfiles.RemovePreset(new DeleteConfigParams
			{
				ClientID = SSEClient.ClientID,
				GameName = config.GameName,
				ConfigName = config.Name
			});
			if (!responseWithError.Result && !string.IsNullOrEmpty(responseWithError.ErrorText))
			{
				DTMessageBox.Show(responseWithError.ErrorText, MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
			}
			if (responseWithError.Result)
			{
				await App.GameProfilesService.FillPresetsCollection(false);
				SenderGoogleAnalytics.SendMessageEvent("Preset", "Remove", config.Name, -1L, false);
				SenderGoogleAnalytics.SendMessageEvent("Preset", "Amount", App.GameProfilesService.PresetsCollection.Count.ToString(), -1L, false);
			}
		}

		public async Task DeleteConfigExecute(ConfigVM config)
		{
			if (MessageBoxWithDoNotShowLogic.Show(Application.Current.MainWindow, DTLocalization.GetString(11074), MessageBoxButton.YesNo, MessageBoxImage.Question, "RemoveGameOrConfig", MessageBoxResult.Yes, false, 0.0, null, null, null, null, null, null) == MessageBoxResult.Yes)
			{
				if (config.ConfigData != null)
				{
					foreach (SubConfigData subConfigData in config.ConfigData)
					{
						if (subConfigData != null)
						{
							subConfigData.MainXBBindingCollection.IsChanged = false;
							subConfigData.MainXBBindingCollection.ShiftXBBindingCollections.ForEach(delegate(ShiftXBBindingCollection col)
							{
								col.IsChanged = false;
							});
						}
					}
				}
				ResponseWithError responseWithError = await App.HttpClientService.GameProfiles.DeleteConfig(new DeleteConfigParams
				{
					ClientID = SSEClient.ClientID,
					GameName = config.GameName,
					ConfigName = config.Name
				});
				if (!responseWithError.Result && !string.IsNullOrEmpty(responseWithError.ErrorText))
				{
					DTMessageBox.Show(responseWithError.ErrorText, MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
				}
				if (responseWithError.Result)
				{
					config.ParentGame.DeleteConfig(config);
				}
			}
		}

		public void CloneConfigExecute(ConfigVM config)
		{
			if (config.IsChangedIncludingShiftCollections)
			{
				config.SaveConfigToJson();
			}
			this.ImportGameConfig(config.ConfigPath, true);
		}

		public void SaveAsGameExecute(GameVM game)
		{
			this.SaveAsGame(game.GetGameFolderPath(), true);
		}

		public void OpenConfigExecute(ConfigVM config)
		{
			if (config.IsChangedIncludingShiftCollections)
			{
				config.SaveConfigToJson();
			}
			if (File.Exists(config.ConfigPath))
			{
				Process.Start("explorer.exe", "/select, \"" + config.ConfigPath + "\"");
			}
		}

		public void ClearConfigExecute(ConfigVM config)
		{
			if (MessageBoxWithDoNotShowLogic.Show(Application.Current.MainWindow, DTLocalization.GetString(12459), MessageBoxButton.YesNo, MessageBoxImage.Question, "ConfirmClearConfig", MessageBoxResult.Yes, false, 0.0, null, null, null, null, null, null) == MessageBoxResult.Yes)
			{
				WaitDialog.ShowDialogStatic(DTLocalization.GetString(5238), null, null, false, false, null, null);
				reWASDApplicationCommands.ClearConfig(config);
				WaitDialog.TryCloseWaitDialog();
			}
		}

		public void ShareConfigExecute(ConfigVM config)
		{
			if (config.IsChangedIncludingShiftCollections)
			{
				config.SaveConfigToJson();
			}
			ShareVM shareVM = new ShareVM(config, App.ConfigFileService, App.GameProfilesService, App.LicensingService);
			ShareWindow shareWindow = new ShareWindow(shareVM);
			shareVM.Window = shareWindow;
			shareWindow.ShowDialog();
		}

		public async void RenameConfigExecute(ConfigVM config)
		{
			string oldName = config.Name;
			string newName = string.Empty;
			GameVM game = config.ParentGame;
			TaskAwaiter<GameChangedResult> taskAwaiter = App.GameProfilesService.TryAskUserToSaveChanges(true).GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<GameChangedResult> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<GameChangedResult>);
			}
			if (taskAwaiter.GetResult() != 3 && this.ShowEditWindow(config, out newName))
			{
				string directoryName = Path.GetDirectoryName(config.ConfigPath);
				newName = XBUtils.NormalizeToMaxPathTrimFilename(directoryName, newName.Trim(), ".rewasd.bak", null);
				if (oldName != newName && !string.IsNullOrEmpty(newName))
				{
					if (game.ConfigCollection.FirstOrDefault((ConfigVM c) => c.Name == newName) != null)
					{
						DTMessageBox.Show(DTLocalization.GetString(11093), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
					}
					else
					{
						string text = directoryName + "\\" + XBUtils.NormalizeToMaxPathTrimFilename(directoryName + "\\", newName.Trim(), ".rewasd", null) + ".rewasd";
						ResponseWithError responseWithError = await App.HttpClientService.GameProfiles.RenameConfig(new RenameConfigParams
						{
							ClientID = SSEClient.ClientID,
							GameName = config.GameName,
							Name = oldName,
							NewName = newName,
							NewPath = text
						});
						if (!responseWithError.Result && !string.IsNullOrEmpty(responseWithError.ErrorText))
						{
							DTMessageBox.Show(responseWithError.ErrorText, MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
						}
					}
				}
			}
		}

		private bool ShowEditWindow(ConfigVM config, out string newName)
		{
			EditConfigName editConfigName = new EditConfigName(this);
			editConfigName.textBox.Text = config.Name;
			editConfigName.ShowDialog();
			newName = editConfigName.textBox.Text;
			return editConfigName.WindowResult == MessageBoxResult.OK || editConfigName.WindowResult == MessageBoxResult.Yes;
		}

		public async void RenamePresetExecute(ConfigVM config)
		{
			string name = config.Name;
			string newName = string.Empty;
			if (this.ShowEditPresetWindow(config, out newName))
			{
				string directoryName = Path.GetDirectoryName(config.ConfigPath);
				newName = XBUtils.NormalizeToMaxPathTrimFilename(directoryName, newName.Trim(), ".rewasd.bak", null);
				if (name != newName && !string.IsNullOrEmpty(newName))
				{
					if (App.GameProfilesService.PresetsCollection.FirstOrDefault((ConfigVM c) => c.Name == newName) != null)
					{
						DTMessageBox.Show(DTLocalization.GetString(11093), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
					}
					else
					{
						ResponseWithError responseWithError = await App.HttpClientService.GameProfiles.RenamePreset(new RenameConfigParams
						{
							GameName = config.GameName,
							Name = name,
							NewName = newName
						});
						if (!responseWithError.Result && !string.IsNullOrEmpty(responseWithError.ErrorText))
						{
							DTMessageBox.Show(responseWithError.ErrorText, MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
						}
						await App.GameProfilesService.FillPresetsCollection(false);
					}
				}
			}
		}

		private bool ShowEditPresetWindow(ConfigVM config, out string newName)
		{
			RenamePreset renamePreset = new RenamePreset();
			renamePreset.textBox.Text = config.Name;
			renamePreset.ShowDialog();
			newName = renamePreset.textBox.Text.Trim();
			return !string.IsNullOrEmpty(newName) && (renamePreset.WindowResult == MessageBoxResult.OK || renamePreset.WindowResult == MessageBoxResult.Yes);
		}

		public void ShowPrintPreview(FixedDocument fixedDoc, Window parent = null)
		{
			new PrintPreviewDialog
			{
				Owner = parent,
				viewer = 
				{
					Document = fixedDoc
				}
			}.ShowDialog();
		}

		public async void PrintConfigExecute(ConfigVM config)
		{
			BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
			GameVM game = App.GameProfilesService.CurrentGame;
			PrintBindingsWindow printBindingsWindow = new PrintBindingsWindow(game.CurrentConfig.ExistAnyBindingsForController(currentGamepad, true), game.CurrentConfig.ExistAnyDescriptionsForController(currentGamepad));
			printBindingsWindow.ShowDialog();
			if (printBindingsWindow.WindowResult == MessageBoxResult.OK)
			{
				WaitDialog.ShowDialogStatic(DTLocalization.GetString(12271), null, null, false, false, null, null);
				EngineClientService engine = App.HttpClientService.Engine;
				GenerateXPSFromConfigInfo generateXPSFromConfigInfo = new GenerateXPSFromConfigInfo();
				generateXPSFromConfigInfo.ConfigName = game.CurrentConfig.Name;
				generateXPSFromConfigInfo.ConfigPath = game.CurrentConfig.ConfigPath;
				BaseControllerVM currentGamepad2 = App.GamepadService.CurrentGamepad;
				generateXPSFromConfigInfo.GamepadID = ((currentGamepad2 != null) ? currentGamepad2.ID : null);
				generateXPSFromConfigInfo.IsBlack = printBindingsWindow.IsBlackPrint;
				generateXPSFromConfigInfo.IsMappings = printBindingsWindow.IsMappings;
				ResponseWithError responseWithError = await engine.GenerateXPSFromConfig(generateXPSFromConfigInfo);
				if (!responseWithError.Result)
				{
					WaitDialog.TryCloseWaitDialog();
					if (!string.IsNullOrEmpty(responseWithError.ErrorText))
					{
						DTMessageBox.Show(responseWithError.ErrorText, MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
					}
				}
				else
				{
					string text = game.CurrentConfig.ConfigPath + ".xps";
					try
					{
						XpsDocument xpsDocument = new XpsDocument(text, FileAccess.Read);
						DocumentReference documentReference = xpsDocument.GetFixedDocumentSequence().References.First<DocumentReference>();
						WaitDialog.TryCloseWaitDialog();
						this.ShowPrintPreview(documentReference.GetDocument(false), Application.Current.MainWindow);
						try
						{
							xpsDocument.Close();
							File.Delete(text);
						}
						catch (Exception)
						{
						}
					}
					catch (Exception)
					{
						WaitDialog.TryCloseWaitDialog();
					}
				}
			}
		}

		public DelegateCommand SwitchBindingLabelModeCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._switchBindingLabelMode) == null)
				{
					delegateCommand = (this._switchBindingLabelMode = new DelegateCommand(new Action(this.SwitchBindingLabelMode)));
				}
				return delegateCommand;
			}
		}

		private void SwitchBindingLabelMode()
		{
			GameVM currentGame = App.GameProfilesService.CurrentGame;
			if (((currentGame != null) ? currentGame.CurrentConfig : null) == null)
			{
				return;
			}
			if (App.GameProfilesService.CurrentGame.CurrentConfig.CurrentBindingCollection != null)
			{
				App.GameProfilesService.RealCurrentBeingMappedBindingCollection.IsLabelModeView = true;
				App.GameProfilesService.CurrentGame.CurrentConfig.CurrentBindingCollection.ShiftXBBindingCollections.ForEach(delegate(ShiftXBBindingCollection sc)
				{
					sc.IsLabelModeView = true;
				});
				App.GameProfilesService.CurrentGame.CurrentConfig.CurrentBindingCollection.SubConfigData.MainXBBindingCollection.IsLabelModeView = true;
			}
		}

		public DelegateCommand SwitchExpandActivatorsModeCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._switchExpandActivatorsMode) == null)
				{
					delegateCommand = (this._switchExpandActivatorsMode = new DelegateCommand(new Action(this.SwitchExpandActivatorsMode)));
				}
				return delegateCommand;
			}
		}

		private void SwitchExpandActivatorsMode()
		{
			GameVM currentGame = App.GameProfilesService.CurrentGame;
			if (((currentGame != null) ? currentGame.CurrentConfig : null) == null)
			{
				return;
			}
			if (App.GameProfilesService.CurrentGame.CurrentConfig.CurrentBindingCollection != null)
			{
				bool currentExpandActivatorsViewMode = App.GameProfilesService.CurrentGame.CurrentConfig.CurrentBindingCollection.IsExpandActivatorsView;
				App.GameProfilesService.RealCurrentBeingMappedBindingCollection.IsExpandActivatorsView = !currentExpandActivatorsViewMode;
				App.GameProfilesService.CurrentGame.CurrentConfig.CurrentBindingCollection.ShiftXBBindingCollections.ForEach(delegate(ShiftXBBindingCollection sc)
				{
					sc.IsExpandActivatorsView = !currentExpandActivatorsViewMode;
				});
				App.GameProfilesService.CurrentGame.CurrentConfig.CurrentBindingCollection.SubConfigData.MainXBBindingCollection.IsExpandActivatorsView = !currentExpandActivatorsViewMode;
			}
		}

		public DelegateCommand SwitchShowMappingsModeCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._switchShowMappingsMode) == null)
				{
					delegateCommand = (this._switchShowMappingsMode = new DelegateCommand(new Action(this.SwitchShowMappingsMode)));
				}
				return delegateCommand;
			}
		}

		private void SwitchShowMappingsMode()
		{
			GameVM currentGame = App.GameProfilesService.CurrentGame;
			if (((currentGame != null) ? currentGame.CurrentConfig : null) == null)
			{
				return;
			}
			if (App.GameProfilesService.CurrentGame.CurrentConfig.CurrentBindingCollection != null)
			{
				App.GameProfilesService.RealCurrentBeingMappedBindingCollection.IsShowMappingsView = true;
				App.GameProfilesService.CurrentGame.CurrentConfig.CurrentBindingCollection.ShiftXBBindingCollections.ForEach(delegate(ShiftXBBindingCollection sc)
				{
					sc.IsShowMappingsView = true;
				});
				App.GameProfilesService.CurrentGame.CurrentConfig.CurrentBindingCollection.SubConfigData.MainXBBindingCollection.IsShowMappingsView = true;
			}
		}

		public DelegateCommand SwitchShowAllModeCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._switchShowAllMode) == null)
				{
					delegateCommand = (this._switchShowAllMode = new DelegateCommand(new Action(this.SwitchShowAllMode)));
				}
				return delegateCommand;
			}
		}

		private void SwitchShowAllMode()
		{
			GameVM currentGame = App.GameProfilesService.CurrentGame;
			if (((currentGame != null) ? currentGame.CurrentConfig : null) == null)
			{
				return;
			}
			if (App.GameProfilesService.CurrentGame.CurrentConfig.CurrentBindingCollection != null)
			{
				App.GameProfilesService.RealCurrentBeingMappedBindingCollection.IsShowAllView = true;
				App.GameProfilesService.CurrentGame.CurrentConfig.CurrentBindingCollection.ShiftXBBindingCollections.ForEach(delegate(ShiftXBBindingCollection sc)
				{
					sc.IsShowAllView = true;
				});
				App.GameProfilesService.CurrentGame.CurrentConfig.CurrentBindingCollection.SubConfigData.MainXBBindingCollection.IsShowAllView = true;
			}
		}

		public DelegateCommand SwitchMaskViewModeCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._switchMaskViewMode) == null)
				{
					delegateCommand = (this._switchMaskViewMode = new DelegateCommand(new Action(this.SwitchMaskViewMode)));
				}
				return delegateCommand;
			}
		}

		private void SwitchMaskViewMode()
		{
			if (App.GameProfilesService.RealCurrentBeingMappedBindingCollection == null)
			{
				return;
			}
			bool currentMaskViewMode = App.GameProfilesService.CurrentGame.CurrentConfig.CurrentBindingCollection.IsMaskModeView;
			App.GameProfilesService.CurrentGame.CurrentConfig.CurrentBindingCollection.IsMaskModeView = !currentMaskViewMode;
			App.GameProfilesService.CurrentGame.CurrentConfig.CurrentBindingCollection.ShiftXBBindingCollections.ForEach(delegate(ShiftXBBindingCollection sc)
			{
				sc.IsMaskModeView = !currentMaskViewMode;
			});
			if (App.GameProfilesService.RealCurrentBeingMappedBindingCollection.IsMaskModeView)
			{
				App.GameProfilesService.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(null);
				reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(MaskView));
			}
			else
			{
				App.GameProfilesService.RealCurrentBeingMappedBindingCollection.MaskBindingCollection.CurrentEditItem = null;
				GameVM currentGame = App.GameProfilesService.CurrentGame;
				if (((currentGame != null) ? currentGame.CurrentConfig : null) != null)
				{
					App.GameProfilesService.CurrentGame.CurrentConfig.NavigateGamepadZoneAccordingToConfigOrSubconfigState();
				}
				else
				{
					reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(SVGGamepadWithAllAnnotations));
				}
			}
			App.BindingFrameRegionManagers.ForEach(delegate(SingleRegionManager bfrm)
			{
				bfrm.NavigateToDefaultView();
			});
			App.EventAggregator.GetEvent<MaskViewChanged>().Publish(null);
		}

		public DelegateCommand SwitchOverlayMenuViewModeCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._SwitchOverlayMenuViewMode) == null)
				{
					delegateCommand = (this._SwitchOverlayMenuViewMode = new DelegateCommand(new Action(this.SwitchOverlayMenuViewMode)));
				}
				return delegateCommand;
			}
		}

		private void SwitchOverlayMenuViewMode()
		{
			if (App.GameProfilesService.RealCurrentBeingMappedBindingCollection == null)
			{
				return;
			}
			if (!App.GameProfilesService.RealCurrentBeingMappedBindingCollection.IsOverlayCollection)
			{
				IGameProfilesService gameProfilesService = App.GameProfilesService;
				IGameProfilesService gameProfilesService2 = App.GameProfilesService;
				int? num;
				if (gameProfilesService2 == null)
				{
					num = null;
				}
				else
				{
					BaseXBBindingCollection overlayCircleBindingCollection = gameProfilesService2.OverlayCircleBindingCollection;
					num = ((overlayCircleBindingCollection != null) ? new int?(overlayCircleBindingCollection.ShiftIndex) : null);
				}
				gameProfilesService.ChangeCurrentShiftCollection(num, false);
			}
			OverlayMenuVM overlayMenu = App.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.OverlayMenu;
			if (overlayMenu != null)
			{
				OverlayMenuCircle circle = overlayMenu.Circle;
				if (circle != null)
				{
					circle.ClearActive();
				}
			}
			bool currentOverlayMenuModeViewMode = App.GameProfilesService.CurrentGame.CurrentConfig.CurrentBindingCollection.IsOverlayMenuModeView;
			App.GameProfilesService.CurrentGame.CurrentConfig.CurrentBindingCollection.IsOverlayMenuModeView = !currentOverlayMenuModeViewMode;
			App.GameProfilesService.CurrentGame.CurrentConfig.CurrentBindingCollection.ShiftXBBindingCollections.ForEach(delegate(ShiftXBBindingCollection sc)
			{
				sc.IsOverlayMenuModeView = !currentOverlayMenuModeViewMode;
			});
			App.GameProfilesService.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(null);
			App.GameProfilesService.RealCurrentBeingMappedBindingCollection.MaskBindingCollection.CurrentEditItem = null;
			if (App.GameProfilesService.RealCurrentBeingMappedBindingCollection.IsOverlayMenuModeView)
			{
				reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(OverlayMenuView));
			}
			else
			{
				GameVM currentGame = App.GameProfilesService.CurrentGame;
				if (((currentGame != null) ? currentGame.CurrentConfig : null) != null)
				{
					App.GameProfilesService.CurrentGame.CurrentConfig.NavigateGamepadZoneAccordingToConfigOrSubconfigState();
				}
				else
				{
					reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(OverlayMenuView));
				}
			}
			App.EventAggregator.GetEvent<OverlayViewChanged>().Publish(null);
		}

		public DelegateCommand SwitchVirtualStickSettingsViewModeCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._SwitchVirtualStickSettingsViewMode) == null)
				{
					delegateCommand = (this._SwitchVirtualStickSettingsViewMode = new DelegateCommand(new Action(this.SwitchVirtualStickSettingsViewMode)));
				}
				return delegateCommand;
			}
		}

		private void SwitchVirtualStickSettingsViewMode()
		{
			if (App.GameProfilesService.RealCurrentBeingMappedBindingCollection == null)
			{
				return;
			}
			bool currentVirtualStickViewMode = App.GameProfilesService.CurrentGame.CurrentConfig.CurrentBindingCollection.IsVirtualStickSettingsModeView;
			App.GameProfilesService.CurrentGame.CurrentConfig.CurrentBindingCollection.IsVirtualStickSettingsModeView = !currentVirtualStickViewMode;
			App.GameProfilesService.CurrentGame.CurrentConfig.CurrentBindingCollection.ShiftXBBindingCollections.ForEach(delegate(ShiftXBBindingCollection sc)
			{
				sc.IsVirtualStickSettingsModeView = !currentVirtualStickViewMode;
			});
			App.GameProfilesService.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(null);
			App.GameProfilesService.RealCurrentBeingMappedBindingCollection.MaskBindingCollection.CurrentEditItem = null;
			if (App.GameProfilesService.RealCurrentBeingMappedBindingCollection.IsVirtualStickSettingsModeView)
			{
				reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(VirtualSticksSettingsView));
			}
			else
			{
				GameVM currentGame = App.GameProfilesService.CurrentGame;
				if (((currentGame != null) ? currentGame.CurrentConfig : null) != null)
				{
					App.GameProfilesService.CurrentGame.CurrentConfig.NavigateGamepadZoneAccordingToConfigOrSubconfigState();
				}
				else
				{
					reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(SVGGamepadWithAllAnnotations));
				}
			}
			App.EventAggregator.GetEvent<MaskViewChanged>().Publish(null);
		}

		public DelegateCommand SwitchLEDSettingsViewModeCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._SwitchLEDSettingsViewMode) == null)
				{
					delegateCommand = (this._SwitchLEDSettingsViewMode = new DelegateCommand(new Action(this.SwitchLEDSettingsViewMode)));
				}
				return delegateCommand;
			}
		}

		private void SwitchLEDSettingsViewMode()
		{
			if (App.GameProfilesService.CurrentMainBindingCollection == null)
			{
				return;
			}
			bool isLEDSettingsView = App.GameProfilesService.CurrentMainBindingCollection.IsLEDSettingsView;
			App.GameProfilesService.CurrentMainBindingCollection.IsLEDSettingsView = !isLEDSettingsView;
			if (App.GameProfilesService.CurrentMainBindingCollection.IsLEDSettingsView)
			{
				reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(LedSettingsView));
				return;
			}
			GameVM currentGame = App.GameProfilesService.CurrentGame;
			if (((currentGame != null) ? currentGame.CurrentConfig : null) != null)
			{
				App.GameProfilesService.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(null);
				App.GameProfilesService.CurrentGame.CurrentConfig.NavigateGamepadZoneAccordingToConfigOrSubconfigState();
				return;
			}
			reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(SVGGamepadWithAllAnnotations));
		}

		public DelegateCommand OpenLEDSettingsViewModeCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._openLEDSettingsViewMode) == null)
				{
					delegateCommand = (this._openLEDSettingsViewMode = new DelegateCommand(new Action(this.OpenLEDSettingsViewMode), delegate
					{
						BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
						return currentGamepad != null && currentGamepad.HasLED;
					}));
				}
				return delegateCommand;
			}
		}

		private void OpenLEDSettingsViewMode()
		{
			if (App.GameProfilesService.CurrentMainBindingCollection == null)
			{
				return;
			}
			App.GameProfilesService.CurrentMainBindingCollection.IsLEDSettingsView = true;
			reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(LedSettingsView));
		}

		public void ShowRadialMenuIconSelector(ConfigVM config)
		{
			new OverlaySectorIcoSelector().ShowDialog();
		}

		public async Task<bool> ShowLicenseWizard(Ref<LicenseCheckResult> checkingResultInfo)
		{
			LicenseWizard licenseWizard = new LicenseWizard();
			await licenseWizard.Init(checkingResultInfo);
			return await licenseWizard.DoModalLicenseError();
		}

		public async Task<bool> ShowLicenseWizardUpdate(Ref<LicenseCheckResult> checkingResultInfo)
		{
			LicenseWizard licenseWizard = new LicenseWizard();
			await licenseWizard.Init(checkingResultInfo);
			return await licenseWizard.DoModalUpdateAvailable();
		}

		public async Task<bool> ShowLicenseWizard(Ref<HtmlOffer> offer, Ref<LicenseCheckResult> checkingResultInfo)
		{
			LicenseWizard licenseWizard = new LicenseWizard();
			await licenseWizard.Init(offer, checkingResultInfo);
			return await licenseWizard.DoModalOffer();
		}

		public MessageBoxResult AddExternalDeviceWizard()
		{
			Wizard wizard = new Wizard(false);
			wizard.ShowDialog();
			return wizard.WindowResult;
		}

		public MessageBoxResult AddSubConfigDialog(out ControllerFamily controllerFamily)
		{
			AddSubConfig addSubConfig = new AddSubConfig();
			addSubConfig.ShowDialog();
			controllerFamily = addSubConfig.ControllerFamily;
			return addSubConfig.WindowResult;
		}

		public bool ShowDialogAddEditGame(ref string sName, ref string sImageSourcePath, ref ObservableCollection<string> applicationNamesCollection, int configCount = -1)
		{
			return AddEditGame.ShowDlgAddEditGame(ref sName, ref sImageSourcePath, ref applicationNamesCollection, configCount);
		}

		private ImportConfig _importConfigDialog;

		private ExportConfig _exportGameDialog;

		private DelegateCommand<object> _bindingFrameBack;

		private DelegateCommand _switchBindingLabelMode;

		private DelegateCommand _switchExpandActivatorsMode;

		private DelegateCommand _switchShowMappingsMode;

		private DelegateCommand _switchShowAllMode;

		private DelegateCommand _switchMaskViewMode;

		private DelegateCommand _SwitchOverlayMenuViewMode;

		private DelegateCommand _SwitchVirtualStickSettingsViewMode;

		private DelegateCommand _SwitchLEDSettingsViewMode;

		private DelegateCommand _openLEDSettingsViewMode;
	}
}
