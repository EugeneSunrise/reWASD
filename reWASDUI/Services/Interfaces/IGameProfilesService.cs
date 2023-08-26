using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Utils;
using Prism.Commands;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.DataModels;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.KeyBindings;
using XBEliteWPF.Services.Interfaces;

namespace reWASDUI.Services.Interfaces
{
	public interface IGameProfilesService : IServiceInitedAwaitable
	{
		ObservableCollection<GameVM> GamesCollection { get; }

		ObservableCollection<ConfigVM> PresetsCollection { get; }

		int MinimumStickZoneLow { get; }

		int MaximumStickZoneHight { get; }

		int MinimumTriggerZoneLow { get; }

		int MaximumTriggerZoneHigh { get; }

		SlotInfo CurrentSlotInfo { get; set; }

		GameVM CurrentGame { get; set; }

		Task SetCurrentGame(GameVM game, bool canCancel = true);

		int CurrentShiftModificator { get; }

		event PropertyChangedExtendedEventHandler<GameVM> CurrentGameChanged;

		event PropertyChangedExtendedEventHandler<ConfigVM> CurrentGameProfileChanged;

		event PropertyChangedExtendedEventHandler OnAutodetectForAnySlotChanged;

		DelegateCommand<string> SetCurrentGameAndProfileByProfilePathCommand { get; }

		DelegateCommand ShowControllerWizardCommand { get; }

		DelegateCommand<GamepadButton?> ChangeCurrentBindingCommand { get; }

		DelegateCommand<int?> ChangeCurrentShiftCollectionCommand { get; }

		DelegateCommand ToggleAutoDetectCommand { get; }

		bool IsCurrentGamepadGameSlotAutodetect { get; }

		bool CanCurrentConfigBeAppliedNow { get; }

		DelegateCommand ToggleHideIrrelevantSubConfigsCommand { get; }

		bool IsHidingIrrelevantSubConfigs { get; }

		bool CanAddAnySubConfig { get; }

		string GamesFolderPath { get; }

		void AddGameProfile();

		void RaiseCanAddAnySubConfigChanged();

		void ChangeCurrentShiftCollection(int? shiftIndex, bool force);

		Task<bool> ApplyCurrentProfile(bool silent = false);

		Task FillGamesCollection();

		Task FillPresetsCollection(bool forceRefresh);

		ConfigVM FindConfigByPath(string configPath, bool newlyAddedConfig = false);

		BaseXBBindingCollection RealCurrentBeingMappedBindingCollection { get; }

		BaseXBBindingCollection OverlayCircleBindingCollection { get; }

		MainXBBindingCollection CurrentMainBindingCollection { get; }

		Task ReloadCurrentConfig();

		Task<GameChangedResult> TryAskUserToSaveChanges(bool canCancel = true);

		Task<bool> SetCurrentGameAndConfig(ConfigVM config, bool showErrorIfNotFound = false);

		Task CreateNewGame(string name, string imageSourcePath, ICollection<string> applicationNames, bool createDefaultProfile, string defaultProfileName = "");
	}
}
