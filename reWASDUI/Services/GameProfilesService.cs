using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Attributes;
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.View.SecondaryWindows;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using Prism.Commands;
using Prism.Events;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Infrastructure.SupportedControllers;
using reWASDCommon.Infrastructure.SupportedControllers.Base;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using reWASDUI.DataModels;
using reWASDUI.DataModels.GamepadActiveProfiles;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Services.Interfaces;
using reWASDUI.Views;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Infrastructure.ExternalDeviceRelations;
using XBEliteWPF.Infrastructure.ExternalDevices;
using XBEliteWPF.License.Licensing.ComStructures;
using XBEliteWPF.Services;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils;
using XBEliteWPF.Utils.Extensions;
using XBEliteWPF.Utils.XBUtilModel;
using XBEliteWPF.ViewModels.Base;

namespace reWASDUI.Services
{
	public class GameProfilesService : ZBindable, IGameProfilesService, IServiceInitedAwaitable
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

		public ObservableCollection<GameVM> GamesCollection
		{
			get
			{
				return this._gamesCollection;
			}
			set
			{
				this.SetProperty<ObservableCollection<GameVM>>(ref this._gamesCollection, value, "GamesCollection");
			}
		}

		public ObservableCollection<ConfigVM> PresetsCollection
		{
			get
			{
				return this._presetsCollection;
			}
			set
			{
				this.SetProperty<ObservableCollection<ConfigVM>>(ref this._presetsCollection, value, "PresetsCollection");
			}
		}

		public async Task SetCurrentGame(GameVM game, bool canCancel = true)
		{
			if (this._currentGame != game)
			{
				if (this._currentGame != null)
				{
					this._currentGame.CurrentConfigChanged -= this.CurrentGameOnCurrentConfigChanged;
				}
				PropertyChangedExtendedEventArgs<GameVM> args = new PropertyChangedExtendedEventArgs<GameVM>("CurrentGame", this._currentGame, game);
				TaskAwaiter<GameChangedResult> taskAwaiter = this.TryAskUserToSaveChanges(canCancel).GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<GameChangedResult> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<GameChangedResult>);
				}
				switch (taskAwaiter.GetResult())
				{
				case 1:
					this.ToggleAutoDetectCommand.RaiseCanExecuteChanged();
					break;
				case 3:
				{
					GameVM oldValue = args.OldValue;
					this._currentGame = oldValue;
					game = oldValue;
					break;
				}
				}
				GameVM currentGame = this._currentGame;
				if (this.SetProperty<GameVM>(ref this._currentGame, game, "CurrentGame"))
				{
					if (game != null && game.CurrentConfig == null)
					{
						ConfigVM configVM = game.ConfigCollection.FirstOrDefault<ConfigVM>();
						if (configVM != null)
						{
							await configVM.ReadConfigFromJsonIfNotLoaded();
						}
					}
					if (this._currentGame != null)
					{
						if (this._currentGame.CurrentConfig == null)
						{
							this._currentGame.CurrentConfig = this._currentGame.ConfigCollection.FirstOrDefault<ConfigVM>();
						}
						ConfigVM currentConfig = this._currentGame.CurrentConfig;
						if (((currentConfig != null) ? currentConfig.ConfigData : null) == null && this._currentGame.CurrentConfig != null)
						{
							ConfigVM currentConfig2 = this._currentGame.CurrentConfig;
							await ((currentConfig2 != null) ? currentConfig2.ReadConfigFromJsonAsync(true) : null);
						}
					}
					GameVM currentGame2 = this._currentGame;
					if (currentGame2 != null)
					{
						ConfigVM currentConfig3 = currentGame2.CurrentConfig;
						if (currentConfig3 != null)
						{
							currentConfig3.ResetChangedState();
						}
					}
					if (this._currentGame != null)
					{
						this._currentGame.CurrentConfigChanged += this.CurrentGameOnCurrentConfigChanged;
					}
					PropertyChangedExtendedEventHandler<GameVM> currentGameChanged = this.CurrentGameChanged;
					if (currentGameChanged != null)
					{
						currentGameChanged(this, args);
					}
					this.ChangeCurrentShiftCollection(new int?(0), false);
					reWASDApplicationCommands.NavigateContentCommand.Execute(typeof(MainContent));
				}
				else if (this._currentGame != null)
				{
					this._currentGame.CurrentConfigChanged += this.CurrentGameOnCurrentConfigChanged;
				}
				GC.Collect();
				GC.WaitForFullGCComplete(-1);
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
		public GameVM CurrentGame
		{
			get
			{
				return this._currentGame;
			}
			set
			{
				this.SetCurrentGame(value, true);
			}
		}

		public event PropertyChangedExtendedEventHandler<GameVM> CurrentGameChanged;

		public event PropertyChangedExtendedEventHandler<ConfigVM> CurrentGameProfileChanged;

		public event PropertyChangedExtendedEventHandler OnAutodetectForAnySlotChanged;

		public BaseXBBindingCollection RealCurrentBeingMappedBindingCollection
		{
			get
			{
				GameVM currentGame = this.CurrentGame;
				if (currentGame == null)
				{
					return null;
				}
				ConfigVM currentConfig = currentGame.CurrentConfig;
				if (currentConfig == null)
				{
					return null;
				}
				MainXBBindingCollection currentBindingCollection = currentConfig.CurrentBindingCollection;
				if (currentBindingCollection == null)
				{
					return null;
				}
				return currentBindingCollection.RealCurrentBeingMappedBindingCollection;
			}
		}

		public BaseXBBindingCollection CurrentKeyboardBindingCollection
		{
			get
			{
				if (this._currentKeyboardBindingCollection == null)
				{
					GameVM currentGame = this.CurrentGame;
					bool flag;
					if (currentGame == null)
					{
						flag = null != null;
					}
					else
					{
						ConfigVM currentConfig = currentGame.CurrentConfig;
						flag = ((currentConfig != null) ? currentConfig.ConfigData : null) != null;
					}
					if (flag)
					{
						GameVM currentGame2 = this.CurrentGame;
						bool flag2;
						if (currentGame2 == null)
						{
							flag2 = false;
						}
						else
						{
							ConfigVM currentConfig2 = currentGame2.CurrentConfig;
							int? num = ((currentConfig2 != null) ? new int?(currentConfig2.ConfigData.Count) : null);
							int num2 = 0;
							flag2 = (num.GetValueOrDefault() == num2) & (num != null);
						}
						if (!flag2)
						{
							GameVM currentGame3 = this.CurrentGame;
							if (currentGame3 == null)
							{
								return null;
							}
							ConfigVM currentConfig3 = currentGame3.CurrentConfig;
							if (currentConfig3 == null)
							{
								return null;
							}
							return currentConfig3.ConfigData[1].MainXBBindingCollection.RealCurrentBeingMappedBindingCollection;
						}
					}
					return null;
				}
				return this._currentKeyboardBindingCollection;
			}
		}

		public BaseXBBindingCollection CurrentGamepadBindingCollection
		{
			get
			{
				if (this._currentGamepadBindingCollection == null)
				{
					GameVM currentGame = this.CurrentGame;
					bool flag;
					if (currentGame == null)
					{
						flag = null != null;
					}
					else
					{
						ConfigVM currentConfig = currentGame.CurrentConfig;
						flag = ((currentConfig != null) ? currentConfig.ConfigData : null) != null;
					}
					if (flag)
					{
						GameVM currentGame2 = this.CurrentGame;
						bool flag2;
						if (currentGame2 == null)
						{
							flag2 = false;
						}
						else
						{
							ConfigVM currentConfig2 = currentGame2.CurrentConfig;
							int? num = ((currentConfig2 != null) ? new int?(currentConfig2.ConfigData.Count) : null);
							int num2 = 0;
							flag2 = (num.GetValueOrDefault() == num2) & (num != null);
						}
						if (!flag2)
						{
							GameVM currentGame3 = this.CurrentGame;
							if (currentGame3 == null)
							{
								return null;
							}
							ConfigVM currentConfig3 = currentGame3.CurrentConfig;
							if (currentConfig3 == null)
							{
								return null;
							}
							return currentConfig3.ConfigData[0].MainXBBindingCollection.RealCurrentBeingMappedBindingCollection;
						}
					}
					return null;
				}
				return this._currentGamepadBindingCollection;
			}
		}

		public BaseXBBindingCollection CurrentMouseBindingCollection
		{
			get
			{
				if (this._currentMouseBindingCollection == null)
				{
					GameVM currentGame = this.CurrentGame;
					bool flag;
					if (currentGame == null)
					{
						flag = null != null;
					}
					else
					{
						ConfigVM currentConfig = currentGame.CurrentConfig;
						flag = ((currentConfig != null) ? currentConfig.ConfigData : null) != null;
					}
					if (flag)
					{
						GameVM currentGame2 = this.CurrentGame;
						bool flag2;
						if (currentGame2 == null)
						{
							flag2 = false;
						}
						else
						{
							ConfigVM currentConfig2 = currentGame2.CurrentConfig;
							int? num = ((currentConfig2 != null) ? new int?(currentConfig2.ConfigData.Count) : null);
							int num2 = 0;
							flag2 = (num.GetValueOrDefault() == num2) & (num != null);
						}
						if (!flag2)
						{
							GameVM currentGame3 = this.CurrentGame;
							if (currentGame3 == null)
							{
								return null;
							}
							ConfigVM currentConfig3 = currentGame3.CurrentConfig;
							if (currentConfig3 == null)
							{
								return null;
							}
							return currentConfig3.ConfigData[2].MainXBBindingCollection.RealCurrentBeingMappedBindingCollection;
						}
					}
					return null;
				}
				return this._currentMouseBindingCollection;
			}
		}

		public BaseXBBindingCollection OverlayCircleBindingCollection
		{
			get
			{
				GameVM currentGame = this.CurrentGame;
				if (currentGame == null)
				{
					return null;
				}
				ConfigVM currentConfig = currentGame.CurrentConfig;
				if (currentConfig == null)
				{
					return null;
				}
				return currentConfig.ConfigData.GetOverlayBaseXbBindingCollection();
			}
		}

		public MainXBBindingCollection CurrentMainBindingCollection
		{
			get
			{
				GameVM currentGame = this.CurrentGame;
				if (currentGame == null)
				{
					return null;
				}
				ConfigVM currentConfig = currentGame.CurrentConfig;
				if (currentConfig == null)
				{
					return null;
				}
				return currentConfig.CurrentBindingCollection;
			}
		}

		public int CurrentShiftModificator
		{
			get
			{
				return this._currentShiftModificator;
			}
		}

		public async Task<GameChangedResult> TryAskUserToSaveChanges(bool canCancel = true)
		{
			GameVM currentGame = this.CurrentGame;
			GameChangedResult gameChangedResult;
			if (((currentGame != null) ? currentGame.CurrentConfig : null) == null)
			{
				gameChangedResult = 0;
			}
			else if (this.CurrentGame.CurrentConfig.IsChangedIncludingShiftCollections)
			{
				ConfigVM currentConfig = this.CurrentGame.CurrentConfig;
				GameChangedResult ret = currentConfig.TryAskUserToSaveChanges(canCancel);
				if (ret == 1)
				{
					await currentConfig.SaveConfig(true);
				}
				else if (ret == 2 && currentConfig != null)
				{
					int index = currentConfig.ConfigData.IndexOf(currentConfig.CurrentBindingCollection.SubConfigData);
					await currentConfig.ReadConfigFromJsonAsync(false);
					GameVM currentGame2 = this.CurrentGame;
					if (((currentGame2 != null) ? currentGame2.CurrentConfig : null) == currentConfig)
					{
						if (index == -1 || index >= currentConfig.ConfigData.Count)
						{
							index = 0;
						}
						currentConfig.CurrentSubConfigData = currentConfig.ConfigData[index];
						PropertyChangedExtendedEventHandler<ConfigVM> currentGameProfileChanged = this.CurrentGameProfileChanged;
						if (currentGameProfileChanged != null)
						{
							currentGameProfileChanged(this, new PropertyChangedExtendedEventArgs<ConfigVM>("currentConfig", currentConfig, currentConfig));
						}
					}
				}
				gameChangedResult = ret;
			}
			else
			{
				gameChangedResult = 0;
			}
			return gameChangedResult;
		}

		public async Task ReloadCurrentConfig()
		{
			GameVM currentGame = this.CurrentGame;
			ConfigVM config = ((currentGame != null) ? currentGame.CurrentConfig : null);
			if (config != null)
			{
				int index = config.ConfigData.IndexOf(config.CurrentBindingCollection.SubConfigData);
				if (index != -1)
				{
					await config.ReadConfigFromJsonAsync(false);
					if (index >= config.ConfigData.Count)
					{
						index = 0;
					}
					config.CurrentSubConfigData = config.ConfigData[index];
					PropertyChangedExtendedEventHandler<ConfigVM> currentGameProfileChanged = this.CurrentGameProfileChanged;
					if (currentGameProfileChanged != null)
					{
						currentGameProfileChanged(this, new PropertyChangedExtendedEventArgs<ConfigVM>("config", config, config));
					}
					config.SaveConfigCommand.RaiseCanExecuteChanged();
				}
			}
		}

		public DelegateCommand<string> SetCurrentGameAndProfileByProfilePathCommand
		{
			get
			{
				DelegateCommand<string> delegateCommand;
				if ((delegateCommand = this._setCurrentGameAndProfileByProfilePath) == null)
				{
					delegateCommand = (this._setCurrentGameAndProfileByProfilePath = new DelegateCommand<string>(async delegate(string path)
					{
						await this.SetCurrentGameAndConfig(path);
					}));
				}
				return delegateCommand;
			}
		}

		public DelegateCommand ShowControllerWizardCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._ShowControllerWizard) == null)
				{
					delegateCommand = (this._ShowControllerWizard = new DelegateCommand(new Action(this.ShowControllerWizard)));
				}
				return delegateCommand;
			}
		}

		private void ShowControllerWizard()
		{
			if (this.RealCurrentBeingMappedBindingCollection == null)
			{
				return;
			}
			this.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(0);
		}

		public DelegateCommand<GamepadButton?> ChangeCurrentBindingCommand
		{
			get
			{
				DelegateCommand<GamepadButton?> delegateCommand;
				if ((delegateCommand = this._ChangeCurrentBinding) == null)
				{
					delegateCommand = (this._ChangeCurrentBinding = new DelegateCommand<GamepadButton?>(new Action<GamepadButton?>(this.ChangeCurrentBinding)));
				}
				return delegateCommand;
			}
		}

		private void ChangeCurrentBinding(GamepadButton? button)
		{
			if (this.RealCurrentBeingMappedBindingCollection == null || button == null)
			{
				return;
			}
			BaseControllerVM currentGamepad = this._gamepadServiceLazy.Value.CurrentGamepad;
			ControllerVM controllerVM = ((currentGamepad != null) ? currentGamepad.CurrentController : null);
			if (controllerVM != null && ControllerTypeExtensions.IsGamepad(controllerVM.ControllerType))
			{
				IGamepadService value = this._gamepadServiceLazy.Value;
				bool flag;
				if (controllerVM == null)
				{
					flag = false;
				}
				else
				{
					SupportedGamepad supportedControllerInfo = controllerVM.SupportedControllerInfo;
					bool? flag2 = ((supportedControllerInfo != null) ? new bool?(supportedControllerInfo.IsBackSideButton(button.Value)) : null);
					bool flag3 = true;
					flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
				value.IsCurrentGamepadBackShown = flag;
			}
			else
			{
				SupportedControllerInfo supportedControllerInfo2;
				ControllersHelper.SupportedControllersDictionary.TryGetValue(3, out supportedControllerInfo2);
				IGamepadService value2 = this._gamepadServiceLazy.Value;
				SupportedGamepad supportedGamepad = supportedControllerInfo2 as SupportedGamepad;
				value2.IsCurrentGamepadBackShown = supportedGamepad != null && supportedGamepad.IsBackSideButton(button.Value);
			}
			this.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(button, true);
			App.EventAggregator.GetEvent<CloseAllPopups>().Publish(null);
		}

		public DelegateCommand<MouseButton?> ChangeCurrentMouseBindingCommand
		{
			get
			{
				DelegateCommand<MouseButton?> delegateCommand;
				if ((delegateCommand = this._ChangeCurrentMouseBinding) == null)
				{
					delegateCommand = (this._ChangeCurrentMouseBinding = new DelegateCommand<MouseButton?>(new Action<MouseButton?>(this.ChangeCurrentMouseBinding)));
				}
				return delegateCommand;
			}
		}

		private void ChangeCurrentMouseBinding(MouseButton? button)
		{
			if (this.RealCurrentBeingMappedBindingCollection == null || button == null)
			{
				return;
			}
			this.RealCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(button.Value);
		}

		public DelegateCommand<int?> ChangeCurrentShiftCollectionCommand
		{
			get
			{
				DelegateCommand<int?> delegateCommand;
				if ((delegateCommand = this._changeCurrentShiftCollection) == null)
				{
					delegateCommand = (this._changeCurrentShiftCollection = new DelegateCommand<int?>(new Action<int?>(this.ChangeCurrentShiftCollection)));
				}
				return delegateCommand;
			}
		}

		public void ChangeCurrentShiftCollection(int? shift, bool force)
		{
			int num = ((shift == null) ? 0 : shift.Value);
			if (!force && this._currentShiftModificator == num)
			{
				return;
			}
			GameVM currentGame = this.CurrentGame;
			if (((currentGame != null) ? currentGame.CurrentConfig : null) != null)
			{
				BaseXBBindingCollection realCurrentBeingMappedBindingCollection = this.RealCurrentBeingMappedBindingCollection;
				if (realCurrentBeingMappedBindingCollection != null)
				{
					realCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(null);
				}
				this._currentShiftModificator = num;
				MainXBBindingCollection currentBindingCollection = this.CurrentGame.CurrentConfig.CurrentBindingCollection;
				if (currentBindingCollection != null)
				{
					if (num == 0 || currentBindingCollection.ShiftXBBindingCollections.Count <= num - 1)
					{
						currentBindingCollection.SetCurrentShiftXBBindingCollection(null);
					}
					else
					{
						currentBindingCollection.SetCurrentShiftXBBindingCollection(currentBindingCollection.ShiftXBBindingCollections[num - 1]);
					}
					if (!force)
					{
						App.EventAggregator.GetEvent<CurrentShiftBindingCollectionChanged>().Publish(currentBindingCollection.CurrentShiftXBBindingCollection);
					}
				}
				this.CurrentGame.CurrentConfig.NavigateGamepadZoneAccordingToConfigOrSubconfigState();
			}
		}

		private void ChangeCurrentShiftCollection(int? shiftIndex)
		{
			if (shiftIndex == null)
			{
				return;
			}
			this.ChangeCurrentShiftCollection(shiftIndex, false);
		}

		public DelegateCommand RadialMenuIconSelectorCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._radialMenuIconSelectorCommand) == null)
				{
					delegateCommand = (this._radialMenuIconSelectorCommand = new DelegateCommand(new Action(this.RadialMenuIconSelector)));
				}
				return delegateCommand;
			}
		}

		private void RadialMenuIconSelector()
		{
			App.GuiHelperService.ShowRadialMenuIconSelector(this.CurrentGame.CurrentConfig);
		}

		public DelegateCommand ToggleAutoDetectCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._toggleAutoDetectCommand) == null)
				{
					delegateCommand = (this._toggleAutoDetectCommand = new DelegateCommand(new Action(this.ToggleAutoDetect), new Func<bool>(this.ToggleAutoDetectCanExecute)));
				}
				return delegateCommand;
			}
		}

		private async void ToggleAutoDetect()
		{
			if (!this.IsCurrentGamepadGameSlotAutodetect)
			{
				TaskAwaiter<bool> taskAwaiter = App.GuiHelperService.EditGameAppsExecute(this.CurrentGame).GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<bool> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				if (!taskAwaiter.GetResult())
				{
					return;
				}
				this.CurrentGame.IsAutodetect = true;
			}
			IGamepadService value = this._gamepadServiceLazy.Value;
			AutoGamesDetectionGamepadProfilesCollection autoGamesDetectionGamepadProfileRelations = value.AutoGamesDetectionGamepadProfileRelations;
			Slot slot = this.CurrentSlotInfo.Slot;
			if (this.IsCurrentGamepadGameSlotAutodetect)
			{
				autoGamesDetectionGamepadProfileRelations.SetSlotProfile(this.CurrentGame.Name, value.CurrentGamepad, slot, null);
			}
			else
			{
				autoGamesDetectionGamepadProfileRelations.SetSlotProfile(this.CurrentGame.Name, value.CurrentGamepad, slot, new GamepadProfile(this.CurrentGame.CurrentConfig));
			}
			bool flag = false;
			foreach (ConfigVM configVM in this.CurrentGame.ConfigCollection)
			{
				configVM.AutodetectForConfigChanged();
				flag |= configVM.IsAutodetectEnabledForAnySlot;
			}
			this.OnPropertyChanged("IsCurrentGamepadGameSlotAutodetect");
			if (!flag)
			{
				this.CurrentGame.IsAutodetect = false;
			}
			value.BinDataSerialize.SaveAutoGamesDetectionGamepadProfileRelations();
			this.ToggleAutoDetectCommand.RaiseCanExecuteChanged();
			PropertyChangedExtendedEventHandler onAutodetectForAnySlotChanged = this.OnAutodetectForAnySlotChanged;
			if (onAutodetectForAnySlotChanged != null)
			{
				onAutodetectForAnySlotChanged(this, null);
			}
		}

		public bool CanCurrentConfigBeAppliedNow
		{
			get
			{
				GameVM currentGame = this.CurrentGame;
				bool flag;
				if (currentGame == null)
				{
					flag = true;
				}
				else
				{
					ConfigVM currentConfig = currentGame.CurrentConfig;
					bool? flag2 = ((currentConfig != null) ? new bool?(currentConfig.IsEmptyForApply) : null);
					bool flag3 = false;
					flag = !((flag2.GetValueOrDefault() == flag3) & (flag2 != null));
				}
				return !flag && this._gamepadServiceLazy.Value.CurrentGamepad != null && !this._gamepadServiceLazy.Value.CurrentGamepad.IsUnknownControllerType && !this._gamepadServiceLazy.Value.CurrentGamepad.IsInvalidControllerType && this._gamepadServiceLazy.Value.CurrentGamepad.IsInitializedController;
			}
		}

		private bool ToggleAutoDetectCanExecute()
		{
			GameVM currentGame = this.CurrentGame;
			return ((currentGame != null) ? currentGame.CurrentConfig : null) != null && !string.IsNullOrEmpty(this.CurrentGame.Name) && this.CanCurrentConfigBeAppliedNow;
		}

		public bool IsCurrentGamepadGameSlotAutodetect
		{
			get
			{
				bool flag;
				try
				{
					if (this.CurrentSlotInfo != null)
					{
						GameVM currentGame = this.CurrentGame;
						if (((currentGame != null) ? currentGame.CurrentConfig : null) != null && !string.IsNullOrEmpty(this.CurrentGame.Name))
						{
							IGamepadService value = this._gamepadServiceLazy.Value;
							AutoGamesDetectionGamepadProfilesCollection autoGamesDetectionGamepadProfileRelations = value.AutoGamesDetectionGamepadProfileRelations;
							Slot slot = this.CurrentSlotInfo.Slot;
							return value.CurrentGamepad != null && autoGamesDetectionGamepadProfileRelations[this.CurrentGame.Name][value.CurrentGamepad.ID].SlotProfiles[slot].Config == this.CurrentGame.CurrentConfig;
						}
					}
					flag = false;
				}
				catch (Exception)
				{
					flag = false;
				}
				return flag;
			}
		}

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
				if (this.SetProperty<bool>(ref this._isHidingIrrelevantSubConfigs, value, "IsHidingIrrelevantSubConfigs"))
				{
					RegistryHelper.SetValue("Config", "HideIrrelevantSubConfigs", this._isHidingIrrelevantSubConfigs ? 1 : 0);
					if (value)
					{
						GameVM currentGame = this.CurrentGame;
						bool flag;
						if (currentGame == null)
						{
							flag = false;
						}
						else
						{
							ConfigVM currentConfig = currentGame.CurrentConfig;
							bool? flag2;
							if (currentConfig == null)
							{
								flag2 = null;
							}
							else
							{
								SubConfigData currentSubConfigData = currentConfig.CurrentSubConfigData;
								flag2 = ((currentSubConfigData != null) ? new bool?(currentSubConfigData.IsCurrentSubConfigRelevantForCurrentDevice) : null);
							}
							bool? flag3 = flag2;
							bool flag4 = false;
							flag = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
						}
						if (flag)
						{
							foreach (SubConfigData subConfigData in this.CurrentGame.CurrentConfig.ConfigData)
							{
								if (subConfigData.IsCurrentSubConfigRelevantForCurrentDevice)
								{
									this.CurrentGame.CurrentConfig.CurrentSubConfigData = subConfigData;
									break;
								}
							}
						}
					}
					GameVM currentGame2 = this.CurrentGame;
					if (currentGame2 == null)
					{
						return;
					}
					ConfigVM currentConfig2 = currentGame2.CurrentConfig;
					if (currentConfig2 == null)
					{
						return;
					}
					currentConfig2.UpdateCreateSubConfigsCommandsCanExecute();
				}
			}
		}

		public bool CanAddAnySubConfig
		{
			get
			{
				if (!this.IsHidingIrrelevantSubConfigs)
				{
					return true;
				}
				BaseControllerVM currentGamepad = this._gamepadServiceLazy.Value.CurrentGamepad;
				if (currentGamepad == null || currentGamepad.IsUnsupportedControllerType)
				{
					return true;
				}
				GameVM currentGame = this.CurrentGame;
				bool flag;
				if (currentGame == null)
				{
					flag = false;
				}
				else
				{
					ConfigVM currentConfig = currentGame.CurrentConfig;
					bool? flag2 = ((currentConfig != null) ? new bool?(currentConfig.IsFutureConfigRelevantForCurrentGamepad(0)) : null);
					bool flag3 = true;
					flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
				if (flag)
				{
					return true;
				}
				GameVM currentGame2 = this.CurrentGame;
				bool flag4;
				if (currentGame2 == null)
				{
					flag4 = false;
				}
				else
				{
					ConfigVM currentConfig2 = currentGame2.CurrentConfig;
					bool? flag2 = ((currentConfig2 != null) ? new bool?(currentConfig2.IsFutureConfigRelevantForCurrentGamepad(1)) : null);
					bool flag3 = true;
					flag4 = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
				if (flag4)
				{
					return true;
				}
				GameVM currentGame3 = this.CurrentGame;
				bool flag5;
				if (currentGame3 == null)
				{
					flag5 = false;
				}
				else
				{
					ConfigVM currentConfig3 = currentGame3.CurrentConfig;
					bool? flag2 = ((currentConfig3 != null) ? new bool?(currentConfig3.IsFutureConfigRelevantForCurrentGamepad(2)) : null);
					bool flag3 = true;
					flag5 = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
				return flag5;
			}
		}

		public SlotInfo CurrentSlotInfo
		{
			get
			{
				return this._currentSlotInfo;
			}
			set
			{
				if (this.SetProperty<SlotInfo>(ref this._currentSlotInfo, value, "CurrentSlotInfo"))
				{
					this.OnPropertyChanged("IsCurrentGamepadGameSlotAutodetect");
					Lazy<IGamepadService> gamepadServiceLazy = this._gamepadServiceLazy;
					if (gamepadServiceLazy == null)
					{
						return;
					}
					IGamepadService value2 = gamepadServiceLazy.Value;
					if (value2 == null)
					{
						return;
					}
					ExternalDeviceRelationsHelper externalDeviceRelationsHelper = value2.ExternalDeviceRelationsHelper;
					if (externalDeviceRelationsHelper == null)
					{
						return;
					}
					externalDeviceRelationsHelper.Refresh();
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

		public GameProfilesService(IConfigFileService ifs, IEventAggregator ea, Lazy<IGamepadService> gsLazy, ILicensingService ls, IHttpClientService hcs)
		{
			Tracer.TraceWrite("Constructor for GameProfilesService", false);
			this._configFileService = ifs;
			this._gamepadServiceLazy = gsLazy;
			this._eventAggregator = ea;
			this._licensingService = ls;
			this._httpClientService = hcs;
			this.CurrentGameChanged += this.OnCurrentGameChanged;
			this._eventAggregator.GetEvent<CurrentShiftBindingCollectionChanged>().Subscribe(new Action<ShiftXBBindingCollection>(this.OnCurrentShiftBindingCollectionChanged));
			this._eventAggregator.GetEvent<CurrentGamepadChanged>().Subscribe(new Action<BaseControllerVM>(this.OnCurrentGamepadChanged));
			this._eventAggregator.GetEvent<RequestReloadConfig>().Subscribe(new Action<string>(this.ReLoadConfigIfLoadedByPath));
			this._licensingService.OnLicenseChangedCompleted += delegate(LicenseCheckResult result, bool onlineActivation)
			{
				GameVM currentGame = this.CurrentGame;
				if (currentGame == null)
				{
					return;
				}
				ConfigVM currentConfig = currentGame.CurrentConfig;
				if (currentConfig == null)
				{
					return;
				}
				ConfigData configData = currentConfig.ConfigData;
				if (configData == null)
				{
					return;
				}
				configData.CheckFeatures();
			};
			this._eventAggregator.GetEvent<CurrentBindingCollectionWrapperChanged>().Subscribe(delegate(SubConfigData sb)
			{
				this.OnPropertyChanged("CurrentMainBindingCollection");
			});
			this._eventAggregator.GetEvent<AllBinsLoaded>().Subscribe(delegate(object sb)
			{
				this.OnPropertyChanged("IsCurrentGamepadGameSlotAutodetect");
			});
			this._eventAggregator.GetEvent<ProfilesChanged>().Subscribe(delegate(WindowMessageEvent o)
			{
				this.ToggleAutoDetectCommand.RaiseCanExecuteChanged();
			});
			this._eventAggregator.GetEvent<CurrentGamepadSlotChanged>().Subscribe(delegate(Slot slot)
			{
				if (this._gamepadServiceLazy.Value.CurrentGamepad != null)
				{
					this.CurrentSlotInfo = this._gamepadServiceLazy.Value.SlotsInfo.FirstOrDefault((SlotInfo si) => si.Slot == slot);
				}
				else
				{
					this.CurrentSlotInfo = this._gamepadServiceLazy.Value.SlotsInfo.FirstOrDefault<SlotInfo>();
				}
				this.ToggleAutoDetectCommand.RaiseCanExecuteChanged();
			});
			this._isHidingIrrelevantSubConfigs = RegistryHelper.GetValue("Config", "HideIrrelevantSubConfigs", 1, false) == 1;
			this.InitService();
		}

		private async void ReLoadConfigIfLoadedByPath(string path)
		{
			ConfigVM configVM = this.FindConfigByPath(path, false);
			if (configVM != null)
			{
				int index = -1;
				GameVM currentGame = this.CurrentGame;
				if (((currentGame != null) ? currentGame.CurrentConfig : null) == configVM && this.CurrentGame != null)
				{
					index = this.CurrentGame.CurrentConfig.ConfigData.IndexOf(this.CurrentGame.CurrentConfig.CurrentSubConfigData);
				}
				await ((configVM != null) ? configVM.ReadConfigFromJsonAsync(false) : null);
				if (index != -1)
				{
					this.CurrentGame.CurrentConfig.CurrentSubConfigData = this.CurrentGame.CurrentConfig.ConfigData[index];
				}
			}
		}

		private void OnCurrentGamepadChanged(BaseControllerVM obj)
		{
			BaseXBBindingCollection realCurrentBeingMappedBindingCollection = this.RealCurrentBeingMappedBindingCollection;
			if (realCurrentBeingMappedBindingCollection != null)
			{
				realCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(null);
			}
			SlotInfo slotInfo = null;
			if (this._gamepadServiceLazy.Value.CurrentGamepad != null)
			{
				slotInfo = this._gamepadServiceLazy.Value.SlotsInfo.FirstOrDefault(delegate(SlotInfo si)
				{
					Slot slot = si.Slot;
					BaseControllerVM currentGamepad = this._gamepadServiceLazy.Value.CurrentGamepad;
					Slot? slot2 = ((currentGamepad != null) ? new Slot?(currentGamepad.CurrentSlot) : null);
					return (slot == slot2.GetValueOrDefault()) & (slot2 != null);
				});
				if (!slotInfo.IsAvailable)
				{
					slotInfo = null;
				}
			}
			this.CurrentSlotInfo = ((slotInfo != null) ? slotInfo : this._gamepadServiceLazy.Value.SlotsInfo.FirstOrDefault<SlotInfo>());
			this.ToggleAutoDetectCommand.RaiseCanExecuteChanged();
			if (this.CurrentGame != null)
			{
				foreach (ConfigVM configVM in this.CurrentGame.ConfigCollection)
				{
					configVM.AutodetectForConfigChanged();
				}
			}
			this.OnPropertyChanged("IsCurrentGamepadGameSlotAutodetect");
			GameVM currentGame = this.CurrentGame;
			bool flag;
			if (currentGame == null)
			{
				flag = false;
			}
			else
			{
				ConfigVM currentConfig = currentGame.CurrentConfig;
				bool? flag2;
				if (currentConfig == null)
				{
					flag2 = null;
				}
				else
				{
					MainXBBindingCollection currentBindingCollection = currentConfig.CurrentBindingCollection;
					flag2 = ((currentBindingCollection != null) ? new bool?(currentBindingCollection.IsMaskModeView) : null);
				}
				bool? flag3 = flag2;
				bool flag4 = true;
				flag = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
			}
			if (flag)
			{
				this.CurrentGame.CurrentConfig.CurrentBindingCollection.IsMaskModeView = false;
			}
			GameVM currentGame2 = this.CurrentGame;
			if (currentGame2 != null)
			{
				ConfigVM currentConfig2 = currentGame2.CurrentConfig;
				if (currentConfig2 != null)
				{
					ConfigData configData = currentConfig2.ConfigData;
					if (configData != null)
					{
						configData.CheckFeatures();
					}
				}
			}
			this.RaiseCanAddAnySubConfigChanged();
			IGuiHelperService guiHelperService = App.GuiHelperService;
			if (guiHelperService == null)
			{
				return;
			}
			guiHelperService.OpenLEDSettingsViewModeCommand.RaiseCanExecuteChanged();
		}

		public void SetCurrentBindingsToKeyboard()
		{
			this._currentKeyboardBindingCollection = this.RealCurrentBeingMappedBindingCollection;
			this.OnPropertyChanged("CurrentKeyboardBindingCollection");
		}

		private void OnCurrentShiftBindingCollectionChanged(object obj)
		{
			if (this.RealCurrentBeingMappedBindingCollection == null)
			{
				this._currentKeyboardBindingCollection = null;
				this._currentGamepadBindingCollection = null;
				this._currentMouseBindingCollection = null;
				this.OnPropertyChanged("CurrentKeyboardBindingCollection");
				this.OnPropertyChanged("CurrentGamepadBindingCollection");
				this.OnPropertyChanged("CurrentMouseBindingCollection");
			}
			else if (this.RealCurrentBeingMappedBindingCollection.SubConfigData.IsKeyboard)
			{
				this._currentKeyboardBindingCollection = this.RealCurrentBeingMappedBindingCollection;
				this.OnPropertyChanged("CurrentKeyboardBindingCollection");
			}
			else if (this.RealCurrentBeingMappedBindingCollection.SubConfigData.IsGamepad)
			{
				this._currentGamepadBindingCollection = this.RealCurrentBeingMappedBindingCollection;
				this.OnPropertyChanged("CurrentGamepadBindingCollection");
			}
			else if (this.RealCurrentBeingMappedBindingCollection.SubConfigData.IsMouse)
			{
				this._currentMouseBindingCollection = this.RealCurrentBeingMappedBindingCollection;
				this.OnPropertyChanged("CurrentMouseBindingCollection");
			}
			this.OnPropertyChanged("RealCurrentBeingMappedBindingCollection");
			this.RaiseCanAddAnySubConfigChanged();
			if (this.RealCurrentBeingMappedBindingCollection != null && this.RealCurrentBeingMappedBindingCollection.IsMaskModeView)
			{
				this.RealCurrentBeingMappedBindingCollection.MaskBindingCollection.CurrentEditItem = null;
			}
			this.OnPropertyChanged("OverlayCircleBindingCollection");
		}

		public void RaiseCanAddAnySubConfigChanged()
		{
			this.OnPropertyChanged("CanAddAnySubConfig");
		}

		private async void OnCurrentGameChanged(object sender, PropertyChangedExtendedEventArgs<GameVM> e)
		{
			if (this._gamepadServiceLazy.Value.IsExclusiveCaptureControllersPresent && this.CurrentGame == null)
			{
				await this._httpClientService.Gamepad.DeleteAllExclusiveCaptureProfiles();
			}
			GameVM currentGame = this.CurrentGame;
			if (currentGame != null)
			{
				ConfigVM currentConfig = currentGame.CurrentConfig;
				if (currentConfig != null)
				{
					currentConfig.ChangeCurrentMainWrapperAccordingToControllerVM(this._gamepadServiceLazy.Value.CurrentGamepad, true);
				}
			}
			if (e != null)
			{
				GameVM oldValue = e.OldValue;
				if (oldValue != null)
				{
					ConfigVM currentConfig2 = oldValue.CurrentConfig;
					if (currentConfig2 != null)
					{
						MainXBBindingCollection currentBindingCollection = currentConfig2.CurrentBindingCollection;
						if (currentBindingCollection != null)
						{
							currentBindingCollection.ResetCurrentMappingsAndOtherSelectors();
						}
					}
				}
			}
			this.OnCurrentShiftBindingCollectionChanged(null);
			this.OnPropertyChanged("IsCurrentGamepadGameSlotAutodetect");
			this.ToggleAutoDetectCommand.RaiseCanExecuteChanged();
			this.RaiseCanAddAnySubConfigChanged();
			if (e.NewValue != null)
			{
				e.NewValue.IsAutoDetectChanged += delegate(object o, PropertyChangedExtendedEventArgs<bool> args)
				{
					this.ToggleAutoDetectCommand.RaiseCanExecuteChanged();
				};
				foreach (ConfigVM configVM in e.NewValue.ConfigCollection)
				{
					configVM.AutodetectForConfigChanged();
				}
			}
			Lazy<IGamepadService> gamepadServiceLazy = this._gamepadServiceLazy;
			if (gamepadServiceLazy != null)
			{
				IGamepadService value = gamepadServiceLazy.Value;
				if (value != null)
				{
					ExternalDeviceRelationsHelper externalDeviceRelationsHelper = value.ExternalDeviceRelationsHelper;
					if (externalDeviceRelationsHelper != null)
					{
						externalDeviceRelationsHelper.Refresh();
					}
				}
			}
			GameVM currentGame2 = this.CurrentGame;
			if (currentGame2 != null)
			{
				ConfigVM currentConfig3 = currentGame2.CurrentConfig;
				if (currentConfig3 != null)
				{
					ConfigData configData = currentConfig3.ConfigData;
					if (configData != null)
					{
						configData.CheckFeatures();
					}
				}
			}
		}

		private void CurrentGameOnCurrentConfigChanged(object sender, PropertyChangedExtendedEventArgs<ConfigVM> e)
		{
			GameVM currentGame = this.CurrentGame;
			if (currentGame != null)
			{
				ConfigVM currentConfig = currentGame.CurrentConfig;
				if (currentConfig != null)
				{
					currentConfig.ChangeCurrentMainWrapperAccordingToControllerVM(this._gamepadServiceLazy.Value.CurrentGamepad, true);
				}
			}
			PropertyChangedExtendedEventHandler<ConfigVM> currentGameProfileChanged = this.CurrentGameProfileChanged;
			if (currentGameProfileChanged != null)
			{
				currentGameProfileChanged(sender, e);
			}
			ConfigVM oldValue = e.OldValue;
			if (oldValue != null)
			{
				MainXBBindingCollection currentBindingCollection = oldValue.CurrentBindingCollection;
				if (currentBindingCollection != null)
				{
					currentBindingCollection.ResetCurrentMappingsAndOtherSelectors();
				}
			}
			this.OnPropertyChanged("CurrentMainBindingCollection");
			this.OnPropertyChanged("RealCurrentBeingMappedBindingCollection");
			this.OnPropertyChanged("OverlayCircleBindingCollection");
			this.OnPropertyChanged("IsCurrentGamepadGameSlotAutodetect");
			this.ToggleAutoDetectCommand.RaiseCanExecuteChanged();
			CommandManager.InvalidateRequerySuggested();
			this.RaiseCanAddAnySubConfigChanged();
			GameVM currentGame2 = this.CurrentGame;
			if (currentGame2 == null)
			{
				return;
			}
			ConfigVM currentConfig2 = currentGame2.CurrentConfig;
			if (currentConfig2 == null)
			{
				return;
			}
			ConfigData configData = currentConfig2.ConfigData;
			if (configData == null)
			{
				return;
			}
			configData.CheckFeatures();
		}

		public async Task CreateNewGame(string name, string imageSourcePath, ICollection<string> applicationNames, bool createDefaultProfile, string defaultProfileName = "")
		{
			ResponseWithError responseWithError = await this._httpClientService.GameProfiles.CreateNewGame(new NewGameParams
			{
				Name = name,
				ImageSourcePath = imageSourcePath,
				ApplicationNames = applicationNames,
				CreateDefaultProfile = createDefaultProfile,
				DefaultProfileName = defaultProfileName
			});
			if (responseWithError.Result)
			{
				await this.AddGameAndProfiles(name);
			}
			else if (!string.IsNullOrEmpty(responseWithError.ErrorText))
			{
				DTMessageBox.Show(responseWithError.ErrorText, MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
			}
		}

		public async Task SetCurrentGameAndConfig(string configPath)
		{
			await this.SetCurrentGameAndConfig(this.FindConfigByPath(configPath, false), true);
		}

		public async Task<bool> SetCurrentGameAndConfig(ConfigVM config, bool showErrorIfNotFound = false)
		{
			bool flag;
			if (config == null)
			{
				if (showErrorIfNotFound)
				{
					if (this._messageSetCurrentGameAndConfigShowed)
					{
						return false;
					}
					this._messageSetCurrentGameAndConfigShowed = true;
					DTMessageBox.Show(DTLocalization.GetString(11853), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
					this._messageSetCurrentGameAndConfigShowed = false;
				}
				flag = false;
			}
			else
			{
				if (this.CurrentGame == config.ParentGame)
				{
					GameVM currentGame = this.CurrentGame;
					if (((currentGame != null) ? currentGame.CurrentConfig : null) == config)
					{
						goto IL_164;
					}
				}
				await config.ParentGame.SetCurrentConfig(config);
				await this.SetCurrentGame(config.ParentGame, false);
				IL_164:
				flag = true;
			}
			return flag;
		}

		public async void AddGameProfile()
		{
			TaskAwaiter<GameChangedResult> taskAwaiter = this.TryAskUserToSaveChanges(true).GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<GameChangedResult> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<GameChangedResult>);
			}
			if (taskAwaiter.GetResult() != 3)
			{
				string sName = "";
				string sImageSourcePath = "";
				ObservableCollection<string> observableCollection = new ObservableCollection<string>();
				IGuiHelperService guiHelperService = App.GuiHelperService;
				if (guiHelperService != null && guiHelperService.ShowDialogAddEditGame(ref sName, ref sImageSourcePath, ref observableCollection, 1))
				{
					if (sName.Length > 0)
					{
						if (this.GamesCollection.FirstOrDefault((GameVM vm) => vm.Name.ToLower().Equals(sName.ToLower())) == null)
						{
							await this.CreateNewGame(sName, sImageSourcePath, observableCollection, true, "");
							GameVM gameVM = this.GamesCollection.FirstOrDefault((GameVM vm) => vm.Name.Equals(sName));
							if (gameVM != null)
							{
								if (sImageSourcePath.Length > 0)
								{
									GameVM.SaveGameThumbnail(sImageSourcePath, gameVM.GetImageSourcePath());
									gameVM.SetImageSource(false);
								}
								await this.SetCurrentGame(gameVM, true);
								SenderGoogleAnalytics.SendMessageEvent("GUI", "ProfileNumber", this.GamesCollection.Count.ToString(), -1L, false);
							}
						}
						else
						{
							DTMessageBox.Show(DTLocalization.GetString(11115), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
						}
					}
					else
					{
						DTMessageBox.Show(DTLocalization.GetString(11029), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
					}
				}
			}
		}

		public string GamesFolderPath
		{
			get
			{
				return App.UserSettingsService.ConfigsFolderPath;
			}
		}

		public async Task FillPresetsCollection(bool forceRefresh)
		{
			ObservableCollection<ConfigVM> observableCollection = await this._httpClientService.GameProfiles.GetPresetsCollection(forceRefresh);
			this.PresetsCollection = observableCollection ?? new ObservableCollection<ConfigVM>();
		}

		public async Task FillGamesCollection()
		{
			this.IsServiceInited = false;
			ObservableCollection<GameVM> observableCollection = await this._httpClientService.GameProfiles.GetGamesCollection();
			if (observableCollection != null)
			{
				observableCollection.ForEach(delegate(GameVM game)
				{
					game.ConfigCollection.ForEach(delegate(ConfigVM config)
					{
						config.ParentGame = game;
					});
				});
				this.GamesCollection = observableCollection;
			}
			else
			{
				this.GamesCollection = new ObservableCollection<GameVM>();
			}
			await this.FillPresetsCollection(false);
			this.IsServiceInited = true;
		}

		public async Task AddGameAndProfiles(string name)
		{
			GameVM gameVM = new GameVM(name);
			await gameVM.FillConfigsCollection();
			if (gameVM.ConfigCollection.Count > 0)
			{
				await gameVM.SetCurrentConfig(gameVM.ConfigCollection[0]);
			}
			this.GamesCollection.Add(gameVM);
			XBUtils.SortObservableCollection<GameVM>(this.GamesCollection);
		}

		public ConfigVM FindConfig(string gameName, string configName)
		{
			GameVM gameVM = this.GamesCollection.FirstOrDefault((GameVM g) => g.Name == gameName);
			if (gameVM == null)
			{
				return null;
			}
			return gameVM.ConfigCollection.FirstOrDefault((ConfigVM c) => c.Name == configName);
		}

		public ConfigVM FindConfigByPath(string configPath, bool newlyAddedConfig = false)
		{
			if (string.IsNullOrWhiteSpace(configPath) || !File.Exists(configPath))
			{
				return null;
			}
			if (newlyAddedConfig)
			{
				string gamePath = configPath.Remove(configPath.LastIndexOf('\\'));
				gamePath = gamePath.Remove(gamePath.LastIndexOf('\\'));
				GameVM gameVM = this.GamesCollection.FirstOrDefault((GameVM g) => g.GetGameFolderPath() == gamePath);
				if (gameVM != null)
				{
					gameVM.FillSingleConfig(configPath);
				}
			}
			return (from g in this.GamesCollection
				from p in g.ConfigCollection
				where p.ConfigPath.ToLower() == configPath.ToLower()
				select p).FirstOrDefault<ConfigVM>();
		}

		private bool TryPromptUserToMergePeripheralDevices()
		{
			bool flag;
			if (!this._gamepadServiceLazy.Value.AllPhysicalControllers.Any((BaseControllerVM c) => c.ControllerFamily == 2))
			{
				flag = this._gamepadServiceLazy.Value.BlacklistGamepads.Any((BlackListGamepad b) => b.ControllerFamily == 2);
			}
			else
			{
				flag = true;
			}
			bool flag2;
			if (!this._gamepadServiceLazy.Value.AllPhysicalControllers.Any((BaseControllerVM c) => c.ControllerFamily == 1))
			{
				flag2 = this._gamepadServiceLazy.Value.BlacklistGamepads.Any((BlackListGamepad b) => b.ControllerFamily == 1);
			}
			else
			{
				flag2 = true;
			}
			bool flag3 = flag2;
			bool isPeripheralDevice = this._gamepadServiceLazy.Value.CurrentGamepad.IsPeripheralDevice;
			bool flag4 = this.CurrentGame.CurrentConfig.ConfigData.Any((SubConfigData bc) => bc.IsMouse && (bc.MainXBBindingCollection.IsControllerBindingsHasVirtualGamepadMappingsIncludingShift || bc.MainXBBindingCollection.IsMouseDigitalMaskHasVirtualGamepadMappingsIncludingShift || bc.MainXBBindingCollection.IsMouseDirectionalGroupHasVirtualGamepadMappingsIncludingShift));
			bool flag5 = this.CurrentGame.CurrentConfig.ConfigData.Any((SubConfigData bc) => bc.IsKeyboard && (bc.MainXBBindingCollection.IsControllerBindingsHasVirtualGamepadMappingsIncludingShift || bc.MainXBBindingCollection.IsKeyboardMaskHasVirtualGamepadMappingsIncludingShift));
			return !flag || !flag3 || !isPeripheralDevice || !flag4 || !flag5 || MessageBoxWithDoNotShowLogic.Show(Application.Current.MainWindow, DTLocalization.GetString(11448), MessageBoxButton.OKCancel, MessageBoxImage.Question, "ConfirmMergeKeyboardAndMouse", MessageBoxResult.OK, false, 0.0, DTLocalization.GetString(11583), DTLocalization.GetString(11584), null, null, null, null) != MessageBoxResult.Cancel;
		}

		public async Task<bool> ApplyCurrentProfile(bool silent)
		{
			Tracer.TraceWrite("GameConfigselectorVM.ApplyProfile", false);
			GameVM currentGame = this.CurrentGame;
			bool flag;
			if (currentGame == null)
			{
				flag = false;
			}
			else
			{
				ConfigVM currentConfig = currentGame.CurrentConfig;
				bool? flag2 = ((currentConfig != null) ? new bool?(currentConfig.IsChangedIncludingShiftCollections) : null);
				bool flag3 = true;
				flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
			}
			TaskAwaiter<bool> taskAwaiter2;
			if (flag)
			{
				GameVM currentGame2 = this.CurrentGame;
				Task<bool> task;
				if (currentGame2 == null)
				{
					task = null;
				}
				else
				{
					ConfigVM currentConfig2 = currentGame2.CurrentConfig;
					task = ((currentConfig2 != null) ? currentConfig2.SaveConfigToJson() : null);
				}
				TaskAwaiter<bool> taskAwaiter = task.GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				if (!taskAwaiter.GetResult())
				{
					return false;
				}
			}
			Tracer.TraceWrite("GameConfigselectorVM.ApplyProfile after saving config", false);
			GameVM currentGame3 = this.CurrentGame;
			bool flag4;
			if (currentGame3 == null)
			{
				flag4 = null != null;
			}
			else
			{
				ConfigVM currentConfig3 = currentGame3.CurrentConfig;
				flag4 = ((currentConfig3 != null) ? currentConfig3.ConfigData : null) != null;
			}
			if (flag4)
			{
				this.CurrentGame.CurrentConfig.ConfigData.IsChanged = false;
				if (this.CurrentGame.CurrentConfig.ConfigData.IsVirtualUsbHubPresent && !App.UserSettingsService.UsePhysicalUSBHubOption && DTMessageBox.Show(Application.Current.MainWindow, DTLocalization.GetString(12705), null, MessageBoxButton.OKCancel, MessageBoxImage.Exclamation, DTLocalization.GetString(11157), false, 0.0, MessageBoxResult.None, DTLocalization.GetString(11576), null, null) == MessageBoxResult.OK)
				{
					await App.AdminOperations.SetUsePhysicalUSBHub(true);
					if (!App.UserSettingsService.UsePhysicalUSBHubOption)
					{
						return false;
					}
					DSUtils.RebootNow();
				}
			}
			bool flag5;
			if (!silent && !this.TryPromptUserToMergePeripheralDevices())
			{
				flag5 = false;
			}
			else
			{
				TaskAwaiter<bool> taskAwaiter = this.CheckExternalDeviceDependencies().GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				if (!taskAwaiter.GetResult())
				{
					flag5 = false;
				}
				else
				{
					bool flag6 = RegistryHelper.GetString("InputDevices", "MouseInputDeviceID", "", false) == "VirtualID";
					Regex regex = new Regex("^val[orante\\.\\d\\D\\s]{0,}$", RegexOptions.IgnoreCase);
					if ((this.CurrentGame.Name.ToLower().Contains("valorant") || regex.IsMatch(this.CurrentGame.Name.Trim().ToLower())) && flag6 && DTMessageBox.Show(Application.Current.MainWindow, DTLocalization.GetString(12425), null, MessageBoxButton.OKCancel, MessageBoxImage.Exclamation, DTLocalization.GetString(11583), false, 0.0, MessageBoxResult.None, DTLocalization.GetString(11584), null, null) == MessageBoxResult.Cancel)
					{
						flag5 = false;
					}
					else
					{
						ConfigVM currentConfig4 = this.CurrentGame.CurrentConfig;
						bool flag3;
						bool flag7;
						if (currentConfig4 == null)
						{
							flag7 = false;
						}
						else
						{
							ConfigData configData = currentConfig4.ConfigData;
							bool? flag2 = ((configData != null) ? new bool?(configData.IsExternal) : null);
							flag3 = true;
							flag7 = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
						}
						if (flag7)
						{
							ExternalDeviceRelation relationForCurrentGamepadAndConfig = this._gamepadServiceLazy.Value.ExternalDeviceRelationsHelper.GetRelationForCurrentGamepadAndConfig();
							if (this._gamepadServiceLazy.Value.CurrentGamepad != null && ((relationForCurrentGamepadAndConfig != null) ? relationForCurrentGamepadAndConfig.AuthGamepad : null) != null)
							{
								ExternalDevice externalDevice = relationForCurrentGamepadAndConfig.ExternalDevice;
								if (externalDevice != null && externalDevice.DeviceType == 0 && relationForCurrentGamepadAndConfig.AuthGamepad.IsValid() && this._gamepadServiceLazy.Value.CurrentGamepad.ID.Contains(relationForCurrentGamepadAndConfig.AuthGamepad.ID))
								{
									taskAwaiter = this._httpClientService.Gamepad.IsOnlyBluetoothConnection(relationForCurrentGamepadAndConfig.AuthGamepad.ID).GetAwaiter();
									if (!taskAwaiter.IsCompleted)
									{
										await taskAwaiter;
										taskAwaiter = taskAwaiter2;
										taskAwaiter2 = default(TaskAwaiter<bool>);
									}
									if (taskAwaiter.GetResult() && MessageBoxWithDoNotShowLogic.Show(Application.Current.MainWindow, DTLocalization.GetString(12090), MessageBoxButton.OKCancel, MessageBoxImage.Asterisk, "RemindAboutPossibleLagsWithExternalController", MessageBoxResult.OK, false, 0.0, DTLocalization.GetString(11583), DTLocalization.GetString(11584), null, null, null, null) == MessageBoxResult.Cancel)
									{
										return false;
									}
								}
							}
						}
						ConfigApplyInfo configApplyInfo = new ConfigApplyInfo();
						GameVM currentGame4 = this.CurrentGame;
						configApplyInfo.Path = ((((currentGame4 != null) ? currentGame4.CurrentConfig : null) != null) ? this.CurrentGame.CurrentConfig.ConfigPath : "");
						configApplyInfo.Bundle = null;
						configApplyInfo.GamepadId = ((this._gamepadServiceLazy.Value.CurrentGamepad != null) ? this._gamepadServiceLazy.Value.CurrentGamepad.ID : "");
						configApplyInfo.Slot = this.CurrentSlotInfo.Slot;
						ConfigApplyInfo configApplyInfo2 = configApplyInfo;
						bool isAlreadyRemapped = this._gamepadServiceLazy.Value.IsCurrentGamepadRemaped;
						flag3 = await this._httpClientService.Gamepad.ConfigApply(configApplyInfo2);
						bool res = flag3;
						if (res && isAlreadyRemapped)
						{
							this.CheckExternalDeviceForBluetoothReconnect();
						}
						await this._gamepadServiceLazy.Value.RefreshCurrentRemapState();
						flag5 = res;
					}
				}
			}
			return flag5;
		}

		private async void CheckExternalDeviceForBluetoothReconnect()
		{
			GameVM currentGame = this.CurrentGame;
			bool flag;
			if (currentGame == null)
			{
				flag = false;
			}
			else
			{
				ConfigVM currentConfig = currentGame.CurrentConfig;
				bool? flag2;
				if (currentConfig == null)
				{
					flag2 = null;
				}
				else
				{
					ConfigData configData = currentConfig.ConfigData;
					flag2 = ((configData != null) ? new bool?(configData.IsExternal) : null);
				}
				bool? flag3 = flag2;
				bool flag4 = true;
				flag = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
			}
			if (flag && this.CurrentSlotInfo != null && this._gamepadServiceLazy.Value.CurrentGamepad != null && this._gamepadServiceLazy.Value.ExternalDeviceRelationsHelper.CurrentExternalStateReconnectVisible)
			{
				await App.HttpClientService.ExternalDevices.ExternalDeviceBluetoothReconnect(this._gamepadServiceLazy.Value.CurrentGamepad.ID, this.CurrentSlotInfo.Slot);
			}
		}

		private async Task<bool> CheckExternalDeviceDependencies()
		{
			ConfigData configData = this.CurrentGame.CurrentConfig.ConfigData;
			bool flag;
			if (configData != null && !configData.IsExternal)
			{
				flag = true;
			}
			else
			{
				ExternalDeviceRelation externalRelation = this._gamepadServiceLazy.Value.ExternalDeviceRelationsHelper.GetRelationForCurrentGamepadAndConfig();
				if (externalRelation == null && this._gamepadServiceLazy.Value.ExternalDevices.Count == 1 && (((this._gamepadServiceLazy.Value.ExternalDevices[0].DeviceType == null || this._gamepadServiceLazy.Value.ExternalDevices[0].DeviceType == 2) && this._gamepadServiceLazy.Value.ExternalClients.Count == 1) || this._gamepadServiceLazy.Value.ExternalDevices[0].DeviceType == 1 || this._gamepadServiceLazy.Value.ExternalDevices[0].DeviceType == 3))
				{
					if (this._gamepadServiceLazy.Value.ExternalDevices[0].DeviceType == null && this._gamepadServiceLazy.Value.ExternalClients.Count == 1)
					{
						Tracer.TraceWrite("GameConfigselectorVM.ApplyProfile: CheckExternalDeviceDependencies: can't find existing relation. Found one Bluetooth Adapter + one Target -> autosave this relation and Apply", false);
					}
					if (this._gamepadServiceLazy.Value.ExternalDevices[0].DeviceType == 2 && this._gamepadServiceLazy.Value.ExternalClients.Count == 1)
					{
						Tracer.TraceWrite("GameConfigselectorVM.ApplyProfile: CheckExternalDeviceDependencies: can't find existing relation. Found one ESP32 Adapter + one Target -> autosave this relation and Apply", false);
					}
					if (this._gamepadServiceLazy.Value.ExternalDevices[0].DeviceType == 1)
					{
						Tracer.TraceWrite("GameConfigselectorVM.ApplyProfile: CheckExternalDeviceDependencies: can't find existing relation. Found one GIMX Adapter -> autosave this relation and Apply", false);
					}
					if (this._gamepadServiceLazy.Value.ExternalDevices[0].DeviceType == 3)
					{
						Tracer.TraceWrite("GameConfigselectorVM.ApplyProfile: CheckExternalDeviceDependencies: can't find existing relation. Found one ESP32S2 Adapter -> autosave this relation and Apply", false);
					}
					ExternalDeviceRelationsHelper externalDeviceRelationsHelper = this._gamepadServiceLazy.Value.ExternalDeviceRelationsHelper;
					ExternalDevice externalDevice = this._gamepadServiceLazy.Value.ExternalDevices[0];
					ExternalClient externalClient = (((this._gamepadServiceLazy.Value.ExternalDevices[0].DeviceType == null || this._gamepadServiceLazy.Value.ExternalDevices[0].DeviceType == 2) && this._gamepadServiceLazy.Value.ExternalClients.Count > 0) ? this._gamepadServiceLazy.Value.ExternalClients[0] : null);
					if ((this._gamepadServiceLazy.Value.ExternalDevices[0].DeviceType != null && this._gamepadServiceLazy.Value.ExternalDevices[0].DeviceType != 2) || this._gamepadServiceLazy.Value.ExternalClients.Count <= 0 || !this._gamepadServiceLazy.Value.ExternalClients[0].IsConsoleAuthRequired)
					{
						if (this._gamepadServiceLazy.Value.ExternalDevices[0].DeviceType != 1 && this._gamepadServiceLazy.Value.ExternalDevices[0].DeviceType != 3)
						{
							goto IL_36E;
						}
						ConfigData configData2 = this.CurrentGame.CurrentConfig.ConfigData;
						if (configData2 == null || configData2.VirtualGamepadType != 1)
						{
							goto IL_36E;
						}
					}
					GamepadAuth gamepadAuth;
					if (this._gamepadServiceLazy.Value.ExternalDeviceRelationsHelper.CurrentGamepadsAuthCollection.Count > 0)
					{
						gamepadAuth = this._gamepadServiceLazy.Value.ExternalDeviceRelationsHelper.CurrentGamepadsAuthCollection[0];
						goto IL_38C;
					}
					IL_36E:
					gamepadAuth = null;
					IL_38C:
					await externalDeviceRelationsHelper.AddAndSaveRelation(externalDevice, externalClient, gamepadAuth);
					externalRelation = this._gamepadServiceLazy.Value.ExternalDeviceRelationsHelper.GetRelationForCurrentGamepadAndConfig();
				}
				if (externalRelation != null && this._gamepadServiceLazy.Value.ExternalDeviceRelationsHelper.IsRelationValidForApply(externalRelation))
				{
					ExternalDeviceState externalDeviceState = await this._gamepadServiceLazy.Value.ExternalDeviceRelationsHelper.GetExternalDeviceState(externalRelation.ExternalDevice, externalRelation.ExternalClient, externalRelation.AuthGamepad);
					ExternalState externalState = await this._gamepadServiceLazy.Value.ExternalDeviceRelationsHelper.GetExternalState(externalRelation.ExternalDevice, externalRelation.ExternalClient, externalRelation.AuthGamepad);
					bool flag2 = false;
					ExternalClient externalClient2 = externalRelation.ExternalClient;
					if (externalClient2 != null && externalClient2.IsNintendoSwitchConsole)
					{
						ConfigData configData3 = this.CurrentGame.CurrentConfig.ConfigData;
						if (configData3 == null || configData3.VirtualGamepadType != 4)
						{
							flag2 = true;
						}
					}
					if (!flag2 && externalState != 11 && externalState != 12 && externalState != 16 && (externalDeviceState == 2 || externalDeviceState == null))
					{
						return true;
					}
					Tracer.TraceWrite("GameConfigselectorVM.ApplyProfile: CheckExternalDeviceDependencies: ExternalDeviceState \"" + externalDeviceState.TryGetDescription() + "\"", false);
				}
				IGuiHelperService guiHelperService = App.GuiHelperService;
				MessageBoxResult? dialogResult = ((guiHelperService != null) ? new MessageBoxResult?(guiHelperService.AddExternalDeviceWizard()) : null);
				bool flag3 = externalRelation != null;
				if (flag3)
				{
					TaskAwaiter<ExternalState> taskAwaiter = this._gamepadServiceLazy.Value.ExternalDeviceRelationsHelper.GetExternalState(externalRelation.ExternalDevice, externalRelation.ExternalClient, externalRelation.AuthGamepad).GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						TaskAwaiter<ExternalState> taskAwaiter2;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<ExternalState>);
					}
					flag3 = taskAwaiter.GetResult() == 11;
				}
				if (flag3)
				{
					flag = false;
				}
				else
				{
					MessageBoxResult? messageBoxResult = dialogResult;
					MessageBoxResult messageBoxResult2 = MessageBoxResult.OK;
					flag = (messageBoxResult.GetValueOrDefault() == messageBoxResult2) & (messageBoxResult != null);
				}
			}
			return flag;
		}

		public const ushort MINIMUM_STICK_ZONE_LOW = 100;

		public const ushort MAXIMUM_STICK_ZONE_HIGH = 100;

		public const ushort MINIMUM_TRIGGER_ZONE_LOW = 100;

		public const ushort MAXIMUM_TRIGGER_ZONE_HIGH = 100;

		private ObservableCollection<GameVM> _gamesCollection = new ObservableCollection<GameVM>();

		private ObservableCollection<ConfigVM> _presetsCollection = new ObservableCollection<ConfigVM>();

		private GameVM _currentGame;

		private IConfigFileService _configFileService;

		private IEventAggregator _eventAggregator;

		private Lazy<IGamepadService> _gamepadServiceLazy;

		private ILicensingService _licensingService;

		private IHttpClientService _httpClientService;

		private int _currentShiftModificator;

		private BaseXBBindingCollection _currentKeyboardBindingCollection;

		private BaseXBBindingCollection _currentGamepadBindingCollection;

		private BaseXBBindingCollection _currentMouseBindingCollection;

		private DelegateCommand<string> _setCurrentGameAndProfileByProfilePath;

		private DelegateCommand _ShowControllerWizard;

		private DelegateCommand<GamepadButton?> _ChangeCurrentBinding;

		private DelegateCommand<MouseButton?> _ChangeCurrentMouseBinding;

		private DelegateCommand<int?> _changeCurrentShiftCollection;

		private DelegateCommand _radialMenuIconSelectorCommand;

		private DelegateCommand _toggleAutoDetectCommand;

		private bool _isHidingIrrelevantSubConfigs;

		private DelegateCommand _toggleUseIrrelevantSubConfigs;

		private SlotInfo _currentSlotInfo;

		private const uint INIT_WAIT_TIMEOUT = 600000U;

		private bool _messageSetCurrentGameAndConfigShowed;
	}
}
