using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using XBEliteWPF.DataModels;

namespace XBEliteWPF.Services.Interfaces
{
	public interface IGameProfilesService : IServiceInitedAwaitable
	{
		ObservableCollection<Game> GamesCollection { get; }

		ObservableCollection<Config> PresetsCollection { get; }

		int MinimumStickZoneLow { get; }

		int MaximumStickZoneHight { get; }

		int MinimumTriggerZoneLow { get; }

		int MaximumTriggerZoneHigh { get; }

		bool IsHidingIrrelevantSubConfigs { get; }

		string GamesFolderPath { get; }

		void AddGameProfile();

		Task FillGamesCollection();

		void FillPresets();

		Config FindConfigByPath(string configPath, bool newlyAddedConfig = false);

		bool CreateNewGame(string name, string imageSourcePath, ICollection<string> applicationNames, bool createDefaultProfile, out string errorText, string defaultProfileName = "");

		bool DeleteGame(Game game);
	}
}
