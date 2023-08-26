using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Clases;
using DiscSoft.NET.Common.View.SecondaryWindows;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using DiscSoft.NET.Common.View.SecondaryWindows.WaitDialog;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using reWASDCommon.Infrastructure;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using reWASDUI.DataModels;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Services;
using reWASDUI.Services.HttpClient;
using reWASDUI.Services.Interfaces;
using reWASDUI.Utils.XBUtil;
using reWASDUI.Views.SecondaryWindows;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils;
using XBEliteWPF.Utils.XBUtilModel;
using XBEliteWPF.ViewModels.Base;

namespace reWASDUI.ViewModels
{
	public class GameConfigSelectorVM : ZBindable
	{
		public IGameProfilesService GameProfilesService { get; set; }

		public IGuiHelperService GuiHelperService { get; set; }

		public ILicensingService LicensingService { get; set; }

		public IGamepadService GamepadService { get; set; }

		public IEventAggregator EventAggregator { get; set; }

		public IGuiScaleService GuiScaleService { get; set; }

		public IUserSettingsService UserSettingsService { get; set; }

		public bool GameConfigSelectorShouldBeShown
		{
			get
			{
				return this._gameConfigSelectorShouldBeShown;
			}
			set
			{
				this.SetProperty<bool>(ref this._gameConfigSelectorShouldBeShown, value, "GameConfigSelectorShouldBeShown");
			}
		}

		public ConfigVM CurrentConfig
		{
			get
			{
				return this._currentConfig;
			}
			set
			{
				this.SetCurrentConfig(value);
			}
		}

		private void CurrentConfigOnCurrentBindingCollectionChanged(object sender, PropertyChangedExtendedEventArgs<MainXBBindingCollection> e)
		{
			if (e.OldValue != null)
			{
				e.OldValue.OnMainOrShiftCollectionItemPropertyChangedExtended -= this.RaiseApplyProfileAndToggleAutoRemapCanExecute;
				e.OldValue.IsChangedModifiedEvent -= this.RaiseApplyProfileAndToggleAutoRemapCanExecute;
			}
			if (e.NewValue != null)
			{
				e.NewValue.OnMainOrShiftCollectionItemPropertyChangedExtended += this.RaiseApplyProfileAndToggleAutoRemapCanExecute;
				e.NewValue.IsChangedModifiedEvent += this.RaiseApplyProfileAndToggleAutoRemapCanExecute;
			}
		}

		public bool IsMultipleConfigs
		{
			get
			{
				ObservableCollection<ConfigVM> configCollection = this.GameProfilesService.CurrentGame.ConfigCollection;
				return configCollection != null && configCollection.Count > 1;
			}
		}

		public bool IsDynamicShifts
		{
			get
			{
				return Constants.DynamicShifts;
			}
		}

		public bool IsOverlayVisible
		{
			get
			{
				return Constants.CreateOverlayShift;
			}
		}

		public RelayCommand<object> CopyShiftCommand
		{
			get
			{
				if (this._copyShiftCommand == null)
				{
					this._copyShiftCommand = new RelayCommand<object>(new Action<object>(this.CopyShiftCommandExecute), new Predicate<object>(this.CopyShiftCanExecute));
				}
				return this._copyShiftCommand;
			}
		}

		private void CopyShiftCommandExecute(object shiftIndex)
		{
			reWASDApplicationCommands.CopyShiftConfig(this.GameProfilesService.CurrentGame.CurrentConfig, (int)shiftIndex);
			this.PasteShiftCommand.RaiseCanExecuteChanged();
			this.GameProfilesService.ToggleAutoDetectCommand.RaiseCanExecuteChanged();
		}

		private bool CopyShiftCanExecute(object shiftIndex)
		{
			if (shiftIndex != null)
			{
				GameVM currentGame = this.GameProfilesService.CurrentGame;
				if (((currentGame != null) ? currentGame.CurrentConfig : null) != null)
				{
					GameVM currentGame2 = this.GameProfilesService.CurrentGame;
					bool flag;
					if (currentGame2 == null)
					{
						flag = false;
					}
					else
					{
						ConfigVM currentConfig = currentGame2.CurrentConfig;
						bool? flag2;
						if (currentConfig == null)
						{
							flag2 = null;
						}
						else
						{
							ConfigData configData = currentConfig.ConfigData;
							flag2 = ((configData != null) ? new bool?(configData.IsOverlayBaseXbBindingCollectionPresent()) : null);
						}
						bool? flag3 = flag2;
						bool flag4 = true;
						flag = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
					}
					return (!flag || this.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.GetOverlayBaseXbBindingCollection().ShiftIndex != (int)shiftIndex) && reWASDApplicationCommands.CanCopyShiftConfig(this.GameProfilesService.CurrentGame.CurrentConfig, (int)shiftIndex, true);
				}
			}
			return false;
		}

		public DelegateCommand<object> PasteShiftCommand
		{
			get
			{
				if (this._pasteShiftCommand == null)
				{
					this._pasteShiftCommand = new DelegateCommand<object>(new Action<object>(this.PasteShiftCommandExecute), new Func<object, bool>(this.PasteShiftCanExecute));
				}
				return this._pasteShiftCommand;
			}
		}

		private void PasteShiftCommandExecute(object shiftIndex)
		{
			GameVM currentGame = this.GameProfilesService.CurrentGame;
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
					flag2 = ((configData != null) ? new bool?(configData.IsOverlayBaseXbBindingCollectionPresent()) : null);
				}
				bool? flag3 = flag2;
				bool flag4 = true;
				flag = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
			}
			if (flag && this.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.GetOverlayBaseXbBindingCollection().ShiftIndex == (int)shiftIndex)
			{
				return;
			}
			reWASDApplicationCommands.PasteShiftConfig(this.GameProfilesService.CurrentGame.CurrentConfig, (int)shiftIndex);
		}

		private bool PasteShiftCanExecute(object shiftIndex)
		{
			if (shiftIndex == null)
			{
				return false;
			}
			GameVM currentGame = this.GameProfilesService.CurrentGame;
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
					flag2 = ((configData != null) ? new bool?(configData.IsOverlayBaseXbBindingCollectionPresent()) : null);
				}
				bool? flag3 = flag2;
				bool flag4 = true;
				flag = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
			}
			return (!flag || this.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.GetOverlayBaseXbBindingCollection().ShiftIndex != (int)shiftIndex) && reWASDApplicationCommands.CanPasteShiftConfig();
		}

		public RelayCommand<object> ClearShiftCommand
		{
			get
			{
				if (this._clearShiftCommand == null)
				{
					this._clearShiftCommand = new RelayCommand<object>(new Action<object>(this.ClearShiftCommandExecute), new Predicate<object>(this.ClearShiftCommandCanExecute));
				}
				return this._clearShiftCommand;
			}
		}

		private void ClearShiftCommandExecute(object shiftIndex)
		{
			if (MessageBoxWithDoNotShowLogic.Show(Application.Current.MainWindow, DTLocalization.GetString(12203), MessageBoxButton.YesNo, MessageBoxImage.Question, "ConfirmClearShiftConfig", MessageBoxResult.Yes, false, 0.0, null, null, null, null, null, null) == MessageBoxResult.Yes)
			{
				reWASDApplicationCommands.ClearShiftConfig(this.GameProfilesService.CurrentGame.CurrentConfig, (int)shiftIndex);
			}
		}

		private bool ClearShiftCommandCanExecute(object shiftIndex)
		{
			if (shiftIndex != null)
			{
				GameVM currentGame = this.GameProfilesService.CurrentGame;
				if (((currentGame != null) ? currentGame.CurrentConfig : null) != null)
				{
					return reWASDApplicationCommands.CanCopyShiftConfig(this.GameProfilesService.CurrentGame.CurrentConfig, (int)shiftIndex, true);
				}
			}
			return false;
		}

		public RelayCommand<object> RemoveShiftCommand
		{
			get
			{
				if (this._RemoveShiftCommand == null)
				{
					this._RemoveShiftCommand = new RelayCommand<object>(new Action<object>(this.RemoveShiftCommandExecute), new Predicate<object>(this.RemoveShiftCommandCanExecute));
				}
				return this._RemoveShiftCommand;
			}
		}

		private void RemoveShiftCommandExecute(object shiftIndex)
		{
			if (MessageBoxWithDoNotShowLogic.Show(Application.Current.MainWindow, DTLocalization.GetString(12203), MessageBoxButton.YesNo, MessageBoxImage.Question, "ConfirmAddOrDeleteShift", MessageBoxResult.Yes, false, 0.0, null, null, null, null, null, null) == MessageBoxResult.Yes)
			{
				reWASDApplicationCommands.RemoveShiftConfig(this.GameProfilesService.CurrentGame.CurrentConfig, (int)shiftIndex);
				GameVM currentGame = this.GameProfilesService.CurrentGame;
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
					SenderGoogleAnalytics.SendMessageEvent("GUI", "CreatedShifts", (this.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.LayersCount - 1).ToString(), -1L, false);
				}
				IGameProfilesService gameProfilesService = this.GameProfilesService;
				ConfigVM currentConfig2 = this.GameProfilesService.CurrentGame.CurrentConfig;
				int? num;
				if (currentConfig2 == null)
				{
					num = null;
				}
				else
				{
					MainXBBindingCollection currentBindingCollection = currentConfig2.CurrentBindingCollection;
					if (currentBindingCollection == null)
					{
						num = null;
					}
					else
					{
						ShiftXBBindingCollection currentShiftXBBindingCollection = currentBindingCollection.CurrentShiftXBBindingCollection;
						num = ((currentShiftXBBindingCollection != null) ? new int?(currentShiftXBBindingCollection.ShiftIndex) : null);
					}
				}
				gameProfilesService.ChangeCurrentShiftCollection(num, false);
				this.GameProfilesService.CurrentGame.CurrentConfig.AddShiftCommand.RaiseCanExecuteChanged();
			}
		}

		private bool RemoveShiftCommandCanExecute(object shiftIndex)
		{
			if (!Constants.DynamicShifts)
			{
				return false;
			}
			if (shiftIndex != null)
			{
				GameVM currentGame = this.GameProfilesService.CurrentGame;
				if (((currentGame != null) ? currentGame.CurrentConfig : null) != null)
				{
					GameVM currentGame2 = this.GameProfilesService.CurrentGame;
					bool flag;
					if (currentGame2 == null)
					{
						flag = false;
					}
					else
					{
						ConfigVM currentConfig = currentGame2.CurrentConfig;
						bool? flag2;
						if (currentConfig == null)
						{
							flag2 = null;
						}
						else
						{
							ConfigData configData = currentConfig.ConfigData;
							flag2 = ((configData != null) ? new bool?(configData.IsOverlayBaseXbBindingCollectionPresent()) : null);
						}
						bool? flag3 = flag2;
						bool flag4 = true;
						flag = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
					}
					if (flag && this.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.GetOverlayBaseXbBindingCollection().ShiftIndex == (int)shiftIndex)
					{
						return false;
					}
					if ((int)shiftIndex > 0)
					{
						GameVM currentGame3 = this.GameProfilesService.CurrentGame;
						int? num;
						if (currentGame3 == null)
						{
							num = null;
						}
						else
						{
							ConfigVM currentConfig2 = currentGame3.CurrentConfig;
							if (currentConfig2 == null)
							{
								num = null;
							}
							else
							{
								ConfigData configData2 = currentConfig2.ConfigData;
								num = ((configData2 != null) ? new int?(configData2.LayersCount) : null);
							}
						}
						int? num2 = num;
						int num3 = (Constants.CreateOverlayShift ? 3 : 2);
						return (num2.GetValueOrDefault() > num3) & (num2 != null);
					}
					return false;
				}
			}
			return false;
		}

		public RelayCommand<object> RenameShiftCommand
		{
			get
			{
				if (this._renameShiftCommand == null)
				{
					this._renameShiftCommand = new RelayCommand<object>(new Action<object>(this.RenameShiftCommandExecute), new Predicate<object>(this.RenameShiftCommandCanExecute));
				}
				return this._renameShiftCommand;
			}
		}

		private void RenameShiftCommandExecute(object shiftIndex)
		{
			string text = "";
			string text2 = "";
			using (IEnumerator<SubConfigData> enumerator = this.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					BaseXBBindingCollection collectionByLayer = enumerator.Current.MainXBBindingCollection.GetCollectionByLayer((int)shiftIndex);
					text = collectionByLayer.Description;
					text2 = collectionByLayer.DefaultDescription;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				text = text2;
			}
			RenameWindow renameWindow = new RenameWindow();
			renameWindow.textBox.Text = text;
			renameWindow.ShowDialog();
			if (renameWindow.WindowResult == MessageBoxResult.OK || renameWindow.WindowResult == MessageBoxResult.Yes)
			{
				string text3 = renameWindow.textBox.Text.Trim();
				if (string.IsNullOrEmpty(text3))
				{
					reWASDApplicationCommands.RenameShiftConfig(this.GameProfilesService.CurrentGame.CurrentConfig, (int)shiftIndex, text2);
					return;
				}
				reWASDApplicationCommands.RenameShiftConfig(this.GameProfilesService.CurrentGame.CurrentConfig, (int)shiftIndex, text3);
			}
		}

		private bool RenameShiftCommandCanExecute(object shiftIndex)
		{
			if (shiftIndex != null)
			{
				GameVM currentGame = this.GameProfilesService.CurrentGame;
				if (((currentGame != null) ? currentGame.CurrentConfig : null) != null)
				{
					return true;
				}
			}
			return false;
		}

		public RelayCommand<object> SaveOverlayAsPresetCommand
		{
			get
			{
				if (this._saveOverlayAsPresetCommand == null)
				{
					this._saveOverlayAsPresetCommand = new RelayCommand<object>(new Action<object>(this.SaveOverlayAsPresetCommandExecute), new Predicate<object>(this.SaveOverlayAsPresetCommandCanExecute));
				}
				return this._saveOverlayAsPresetCommand;
			}
		}

		private async void SaveOverlayAsPresetCommandExecute(object shiftIndex)
		{
			MessageBoxResult messageBoxResult = DTMessageBox.Show(Application.Current.MainWindow, DTLocalization.GetString(12764), MessageBoxButton.OKCancel, MessageBoxImage.Exclamation, DTLocalization.GetString(9219));
			if (messageBoxResult == MessageBoxResult.Yes || messageBoxResult == MessageBoxResult.OK)
			{
				WaitDialog.ShowDialogStatic(DTLocalization.GetString(5238), null, null, false, false, null, null);
				await this.SaveOverlayPreset(shiftIndex);
				WaitDialog.TryCloseWaitDialog();
			}
		}

		private async Task SaveOverlayPreset(object shiftIndex)
		{
			string text = "DefaultRadialMenu.rewasd";
			ConfigData configData = XBUtils.CreateConfigData(true);
			this.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.CopyToModel(configData);
			ConfigVM configVM = new ConfigVM("", text, null);
			configVM.CreateBindingCollection(false);
			reWASDApplicationCommands.FillShift(configData, (int)shiftIndex, configVM, (int)shiftIndex, true, true);
			ConfigData configData2 = new ConfigData();
			configVM.ConfigData.CopyToModel(configData2);
			await App.HttpClientService.GameProfiles.SaveOverlayPreset(new SaveConfigParams<ConfigData>
			{
				ClientID = SSEClient.ClientID,
				ConfigName = text,
				GameName = "",
				ConfigData = configData2
			});
		}

		private bool SaveOverlayAsPresetCommandCanExecute(object shiftIndex)
		{
			if (shiftIndex != null)
			{
				GameVM currentGame = this.GameProfilesService.CurrentGame;
				if (((currentGame != null) ? currentGame.CurrentConfig : null) != null)
				{
					GameVM currentGame2 = this.GameProfilesService.CurrentGame;
					bool flag;
					if (currentGame2 == null)
					{
						flag = false;
					}
					else
					{
						ConfigVM currentConfig = currentGame2.CurrentConfig;
						bool? flag2;
						if (currentConfig == null)
						{
							flag2 = null;
						}
						else
						{
							ConfigData configData = currentConfig.ConfigData;
							flag2 = ((configData != null) ? new bool?(configData.IsOverlayBaseXbBindingCollectionPresent()) : null);
						}
						bool? flag3 = flag2;
						bool flag4 = true;
						flag = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
					}
					return flag && this.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.GetOverlayBaseXbBindingCollection().ShiftIndex == (int)shiftIndex;
				}
			}
			return false;
		}

		public RelayCommand<object> SaveShiftAsPresetCommand
		{
			get
			{
				if (this._saveShiftAsPresetCommand == null)
				{
					this._saveShiftAsPresetCommand = new RelayCommand<object>(new Action<object>(this.SaveShiftAsPresetCommandExecute), new Predicate<object>(this.SaveShiftAsPresetCommandCanExecute));
				}
				return this._saveShiftAsPresetCommand;
			}
		}

		private async void SaveShiftAsPresetCommandExecute(object shiftIndex)
		{
			if (MessageBoxWithDoNotShowLogic.Show(Application.Current.MainWindow, DTLocalization.GetString(12571), MessageBoxButton.YesNo, MessageBoxImage.Question, "ConfirmAddOrDeletePresetShift", MessageBoxResult.Yes, false, 0.0, null, null, null, null, null, null) == MessageBoxResult.Yes)
			{
				WaitDialog.ShowDialogStatic(DTLocalization.GetString(5238), null, null, false, false, null, null);
				await this.SaveShiftAsPreset(shiftIndex);
				WaitDialog.TryCloseWaitDialog();
			}
		}

		private async Task SaveShiftAsPreset(object shiftIndex)
		{
			string description = this.GameProfilesService.CurrentGame.CurrentConfig.ConfigData[0].MainXBBindingCollection.GetCollectionByLayer((int)shiftIndex).Description;
			RenamePreset renamePreset = new RenamePreset();
			renamePreset.textBox.Text = description;
			renamePreset.ShowDialog();
			if (renamePreset.WindowResult == MessageBoxResult.OK || renamePreset.WindowResult == MessageBoxResult.Yes)
			{
				string newName = renamePreset.textBox.Text.Trim();
				ConfigData configData = XBUtils.CreateConfigData(true);
				this.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.CopyToModel(configData);
				ConfigVM configVM = new ConfigVM("", newName, null);
				configVM.CreateBindingCollection(false);
				reWASDApplicationCommands.FillShift(configData, (int)shiftIndex, configVM, 1, true, true);
				reWASDApplicationCommands.FillPresetDirections(configData, (int)shiftIndex, configVM, 0, true);
				ConfigData configData2 = new ConfigData();
				configVM.ConfigData.CopyToModel(configData2);
				await App.HttpClientService.GameProfiles.SavePreset(new SaveConfigParams<ConfigData>
				{
					ClientID = SSEClient.ClientID,
					ConfigName = newName,
					GameName = "",
					ConfigData = configData2
				});
				await App.GameProfilesService.FillPresetsCollection(false);
				SenderGoogleAnalytics.SendMessageEvent("Preset", "Save", newName, (long)App.GameProfilesService.PresetsCollection.Count, false);
				SenderGoogleAnalytics.SendMessageEvent("Preset", "Amount", App.GameProfilesService.PresetsCollection.Count.ToString(), -1L, false);
				newName = null;
			}
		}

		private bool SaveShiftAsPresetCommandCanExecute(object shiftIndex)
		{
			if (shiftIndex != null)
			{
				GameVM currentGame = this.GameProfilesService.CurrentGame;
				if (((currentGame != null) ? currentGame.CurrentConfig : null) != null)
				{
					GameVM currentGame2 = this.GameProfilesService.CurrentGame;
					bool flag;
					if (currentGame2 == null)
					{
						flag = false;
					}
					else
					{
						ConfigVM currentConfig = currentGame2.CurrentConfig;
						bool? flag2;
						if (currentConfig == null)
						{
							flag2 = null;
						}
						else
						{
							ConfigData configData = currentConfig.ConfigData;
							flag2 = ((configData != null) ? new bool?(configData.IsOverlayBaseXbBindingCollectionPresent()) : null);
						}
						bool? flag3 = flag2;
						bool flag4 = true;
						flag = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
					}
					return (!flag || this.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.GetOverlayBaseXbBindingCollection().ShiftIndex != (int)shiftIndex) && reWASDApplicationCommands.CanCopyShiftConfig(this.GameProfilesService.CurrentGame.CurrentConfig, (int)shiftIndex, false);
				}
			}
			return false;
		}

		public DelegateCommand ApplyProfileCommand
		{
			get
			{
				return IContainerProviderExtensions.Resolve<GamepadSelectorVM>(App.Container).ApplyProfileCommand;
			}
		}

		private void RaiseApplyProfileAndToggleAutoRemapCanExecute(object sender, PropertyChangedEventArgs e)
		{
			this.RaiseApplyProfileAndToggleAutoRemapCanExecute();
		}

		private void RaiseApplyProfileAndToggleAutoRemapCanExecute(object sender, PropertyChangedExtendedEventArgs e)
		{
			this.RaiseApplyProfileAndToggleAutoRemapCanExecute();
		}

		private void RaiseApplyProfileAndToggleAutoRemapCanExecute()
		{
			this.ApplyProfileCommand.RaiseCanExecuteChanged();
			this.GameProfilesService.ToggleAutoDetectCommand.RaiseCanExecuteChanged();
		}

		private void OnAutodetectForAnySlotChanged(object sender, PropertyChangedExtendedEventArgs args)
		{
			this.RaiseApplyProfileAndToggleAutoRemapCanExecute();
		}

		public ICommand OpenCommunityCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._openCommunity) == null)
				{
					relayCommand = (this._openCommunity = new RelayCommand(new Action(this.OpenCommunity)));
				}
				return relayCommand;
			}
		}

		private void OpenCommunity()
		{
			App.OpenCommunityLink();
		}

		private void UnsubscribeConfigEvents()
		{
			if (this._currentConfig != null)
			{
				this._currentConfig.CurrentBindingCollectionChanged -= this.CurrentConfigOnCurrentBindingCollectionChanged;
				if (this._currentConfig.CurrentBindingCollection != null)
				{
					this._currentConfig.CurrentBindingCollection.OnMainOrShiftCollectionItemPropertyChangedExtended -= this.RaiseApplyProfileAndToggleAutoRemapCanExecute;
					this._currentConfig.CurrentBindingCollection.IsChangedModifiedEvent -= this.RaiseApplyProfileAndToggleAutoRemapCanExecute;
				}
			}
		}

		private void SubscribeConfigEvents()
		{
			if (this._currentConfig != null && this._currentConfig.CurrentBindingCollection != null)
			{
				this._currentConfig.CurrentBindingCollectionChanged += this.CurrentConfigOnCurrentBindingCollectionChanged;
				this._currentConfig.CurrentBindingCollection.OnMainOrShiftCollectionItemPropertyChangedExtended += this.RaiseApplyProfileAndToggleAutoRemapCanExecute;
				this._currentConfig.CurrentBindingCollection.IsChangedModifiedEvent += this.RaiseApplyProfileAndToggleAutoRemapCanExecute;
			}
		}

		public async Task SetCurrentConfig(ConfigVM config)
		{
			ConfigVM _oldValue = this._currentConfig;
			this.UnsubscribeConfigEvents();
			if (this.SetProperty<ConfigVM>(ref this._currentConfig, config, "CurrentConfig"))
			{
				if (config != null && this.GameProfilesService.CurrentGame != null)
				{
					await this.GameProfilesService.CurrentGame.SetCurrentConfig(config);
					GameVM currentGame = this.GameProfilesService.CurrentGame;
					if (((currentGame != null) ? currentGame.CurrentConfig : null) != config)
					{
						this.SetProperty<ConfigVM>(ref this._currentConfig, _oldValue, "CurrentConfig");
					}
				}
				if (config == null)
				{
					BaseControllerVM currentGamepad = this.GamepadService.CurrentGamepad;
					XBUtils.NavigateGamepadZoneForControllerFamily((currentGamepad != null) ? new ControllerFamily?(currentGamepad.TreatAsControllerFamily) : null);
				}
				this.RaiseApplyProfileAndToggleAutoRemapCanExecute();
			}
			this.SubscribeConfigEvents();
			this.CheckExclusiveCaptureProfile();
			ExternalDeviceRelationsHelper externalDeviceRelationsHelper = this.GamepadService.ExternalDeviceRelationsHelper;
			if (externalDeviceRelationsHelper != null)
			{
				externalDeviceRelationsHelper.Refresh();
			}
		}

		private async void CheckExclusiveCaptureProfile()
		{
			await this.GamepadService.RefreshExclusiveDeviceInfo();
			if (this.GamepadService.IsExclusiveCaptureControllersPresent)
			{
				if (this._currentConfig != null && this.GamepadService.CurrentGamepad != null && this.GamepadService.CurrentGamepad.HasExclusiveCaptureControllers && !this.GamepadService.IsExclusiveCaptureProfilePresent)
				{
					GamepadClientService gamepad = App.HttpClientService.Gamepad;
					BaseControllerVM currentGamepad = this.GamepadService.CurrentGamepad;
					await gamepad.ProcessExclusiveCaptureProfile((currentGamepad != null) ? currentGamepad.ID : null, true, true);
				}
				if (this._currentConfig == null && this.GamepadService.IsExclusiveCaptureProfilePresent)
				{
					await App.HttpClientService.Gamepad.DeleteAllExclusiveCaptureProfiles();
				}
			}
		}

		public GameConfigSelectorVM(IGameProfilesService gps, ILicensingService ls, IGamepadService gs, IEventAggregator ea, IGuiScaleService gss, IGuiHelperService ghs, IUserSettingsService uss)
		{
			this.GameProfilesService = gps;
			this.GuiHelperService = ghs;
			this.LicensingService = ls;
			this.GamepadService = gs;
			this.EventAggregator = ea;
			this.GuiScaleService = gss;
			this.UserSettingsService = uss;
			this.GameProfilesService.CurrentGameChanged += async delegate(object sender, PropertyChangedExtendedEventArgs<GameVM> args)
			{
				if (this.GameProfilesService.CurrentGame != null)
				{
					if (this.GameProfilesService.CurrentGame.CurrentConfig == null)
					{
						await this.SetCurrentConfig(this.GameProfilesService.CurrentGame.ConfigCollection.FirstOrDefault<ConfigVM>());
					}
					else
					{
						this.UnsubscribeConfigEvents();
						this._currentConfig = this.GameProfilesService.CurrentGame.CurrentConfig;
						this.SubscribeConfigEvents();
						this.OnPropertyChanged("CurrentConfig");
						this.CheckExclusiveCaptureProfile();
					}
					if (this.GameProfilesService.CurrentGame != null)
					{
						this.GameProfilesService.CurrentGame.IsAutoDetectChanged += delegate(object o, PropertyChangedExtendedEventArgs<bool> args2)
						{
							this.ApplyProfileCommand.RaiseCanExecuteChanged();
						};
					}
					this.RaiseApplyProfileAndToggleAutoRemapCanExecute();
					this.OnPropertyChanged("IsMultipleConfigs");
				}
			};
			this.GameProfilesService.CurrentGameProfileChanged += async delegate(object sender, PropertyChangedExtendedEventArgs<ConfigVM> args)
			{
				if (args.NewValue != this.CurrentConfig)
				{
					await this.SetCurrentConfig(args.NewValue);
				}
				this.RaiseApplyProfileAndToggleAutoRemapCanExecute();
				this.OnPropertyChanged("IsMultipleConfigs");
			};
			this.EventAggregator.GetEvent<CurrentGamepadChanged>().Subscribe(delegate(BaseControllerVM sender)
			{
				this.ApplyProfileCommand.RaiseCanExecuteChanged();
				this.GameProfilesService.ToggleAutoDetectCommand.RaiseCanExecuteChanged();
			});
			this.EventAggregator.GetEvent<CurrentGamepadSlotChanged>().Subscribe(delegate(Slot sender)
			{
				this.RaiseApplyProfileAndToggleAutoRemapCanExecute();
			});
			App.EventAggregator.GetEvent<IsBoundToGamepadChanged>().Subscribe(delegate(ConfigData configData)
			{
				ConfigVM currentConfig = this._currentConfig;
				if (configData == ((currentConfig != null) ? currentConfig.ConfigData : null))
				{
					this.RaiseApplyProfileAndToggleAutoRemapCanExecute(null, null);
				}
			});
			this.EventAggregator.GetEvent<GamepadInitializationChanged>().Subscribe(delegate(BaseControllerVM sender)
			{
				this.ApplyProfileCommand.RaiseCanExecuteChanged();
			});
			this.GameProfilesService.OnAutodetectForAnySlotChanged += this.OnAutodetectForAnySlotChanged;
			TranslationManager.Instance.LanguageChanged += delegate([Nullable(2)] object s, EventArgs e)
			{
			};
		}

		private bool _gameConfigSelectorShouldBeShown;

		private ConfigVM _currentConfig;

		private RelayCommand<object> _copyShiftCommand;

		private DelegateCommand<object> _pasteShiftCommand;

		private RelayCommand<object> _clearShiftCommand;

		private RelayCommand<object> _RemoveShiftCommand;

		private RelayCommand<object> _renameShiftCommand;

		private RelayCommand<object> _saveOverlayAsPresetCommand;

		private RelayCommand<object> _saveShiftAsPresetCommand;

		private RelayCommand _openCommunity;
	}
}
