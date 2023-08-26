using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Ioc;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Network.HTTP;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.ViewModels.CommunityConfigs;
using reWASDUI.Views;

namespace reWASDUI.ViewModels
{
	public class CommunityConfigsViewVM : ZBindableBase
	{
		public bool CommunityIsVisible { get; set; }

		public string GameName
		{
			get
			{
				return this._gameName;
			}
			set
			{
				if (this.SetProperty<string>(ref this._gameName, value, "GameName") && this._gameName.Length > 2)
				{
					this.GetPopupGamesList();
				}
			}
		}

		public List<GameInfo> PopupGamesList
		{
			get
			{
				return this._popupGamesList;
			}
			set
			{
				this.SetProperty<List<GameInfo>>(ref this._popupGamesList, value, "PopupGamesList");
			}
		}

		public List<GameInfo> GamesList
		{
			get
			{
				return this._gamesList;
			}
			set
			{
				this.SetProperty<List<GameInfo>>(ref this._gamesList, value, "GamesList");
			}
		}

		private async void SetCurrentGameAsync(GameInfo game)
		{
			if (this._currentGame != game)
			{
				this._currentGame = game;
				this.PopupGamesList = null;
				if (game == null)
				{
					this.ConfigsList = null;
				}
				else
				{
					await this.GetConfigsList();
				}
				this._gameName = ((game != null) ? game.Name : "");
				this.OnPropertyChanged("CurrentGame");
				this.OnPropertyChanged("GameName");
			}
		}

		public GameInfo PopupCurrentGame
		{
			get
			{
				return this._currentGame;
			}
			set
			{
				if (value != null)
				{
					this.SetCurrentGameAsync(value);
				}
			}
		}

		public GameInfo CurrentGame
		{
			get
			{
				return this._currentGame;
			}
			set
			{
				this.SetCurrentGameAsync(value);
			}
		}

		public List<ConfigInfo> ConfigsList
		{
			get
			{
				return this._configsList;
			}
			set
			{
				this.SetProperty<List<ConfigInfo>>(ref this._configsList, value, "ConfigsList");
			}
		}

		public bool IsInternetError
		{
			get
			{
				return this._isInternetError;
			}
			set
			{
				this.SetProperty<bool>(ref this._isInternetError, value, "IsInternetError");
			}
		}

		public bool IsLoading
		{
			get
			{
				return this._isLoading;
			}
			set
			{
				this.SetProperty<bool>(ref this._isLoading, value, "IsLoading");
			}
		}

		public CommunityConfigsViewVM()
		{
			App.EventAggregator.GetEvent<CurrentControllerDataChanged>().Subscribe(delegate(BaseControllerVM controller)
			{
				if (controller != null && this.CommunityIsVisible && !controller.IsInitializedController)
				{
					reWASDApplicationCommands.NavigateContentCommand.Execute(typeof(MainContent));
				}
			});
			App.EventAggregator.GetEvent<CurrentGamepadChanged>().Subscribe(delegate(BaseControllerVM controller)
			{
				if (controller != null && this.CommunityIsVisible)
				{
					if (!controller.IsInitializedController)
					{
						reWASDApplicationCommands.NavigateContentCommand.Execute(typeof(MainContent));
						return;
					}
					if (this.CurrentGame != null)
					{
						this.GetConfigsList();
					}
				}
			});
		}

		private bool CheckResponseError(HttpResponseMessage resp)
		{
			if (resp == null)
			{
				this.IsInternetError = true;
				return false;
			}
			if (!resp.IsSuccessStatusCode)
			{
				this.IsInternetError = true;
				string text = "";
				if (resp.StatusCode == HttpStatusCode.BadRequest)
				{
					string result = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
					try
					{
						HttpUtils.ErrorMessage errorMessage = JsonConvert.DeserializeObject<HttpUtils.ErrorMessage>(result);
						text = ((errorMessage != null) ? errorMessage.Message : null);
					}
					catch (Exception)
					{
					}
				}
				if (string.IsNullOrWhiteSpace(text))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 2);
					defaultInterpolatedStringHandler.AppendLiteral("Code: ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(Convert.ToInt32(resp.StatusCode));
					defaultInterpolatedStringHandler.AppendLiteral(" ");
					defaultInterpolatedStringHandler.AppendFormatted(resp.ReasonPhrase);
					text = defaultInterpolatedStringHandler.ToStringAndClear();
				}
				Tracer.TraceWrite(text, false);
				return false;
			}
			return true;
		}

		private async Task ExecDownloadProcess(bool showIsLoading, Func<Task> action)
		{
			CommunityConfigsViewVM.<>c__DisplayClass44_0 CS$<>8__locals1 = new CommunityConfigsViewVM.<>c__DisplayClass44_0();
			CS$<>8__locals1.showIsLoading = showIsLoading;
			CS$<>8__locals1.<>4__this = this;
			Task.Factory.StartNew<Task>(delegate
			{
				CommunityConfigsViewVM.<>c__DisplayClass44_0.<<ExecDownloadProcess>b__0>d <<ExecDownloadProcess>b__0>d;
				<<ExecDownloadProcess>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<ExecDownloadProcess>b__0>d.<>4__this = CS$<>8__locals1;
				<<ExecDownloadProcess>b__0>d.<>1__state = -1;
				<<ExecDownloadProcess>b__0>d.<>t__builder.Start<CommunityConfigsViewVM.<>c__DisplayClass44_0.<<ExecDownloadProcess>b__0>d>(ref <<ExecDownloadProcess>b__0>d);
				return <<ExecDownloadProcess>b__0>d.<>t__builder.Task;
			await action();
			if (CS$<>8__locals1.showIsLoading)
			{
				this.IsLoading = false;
			}
		}

		public async Task GetTopGamesList()
		{
			CommunityConfigsViewVM.<>c__DisplayClass45_0 CS$<>8__locals1 = new CommunityConfigsViewVM.<>c__DisplayClass45_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.resp = null;
			await this.ExecDownloadProcess(true, delegate
			{
				CommunityConfigsViewVM.<>c__DisplayClass45_0.<<GetTopGamesList>b__0>d <<GetTopGamesList>b__0>d;
				<<GetTopGamesList>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<GetTopGamesList>b__0>d.<>4__this = CS$<>8__locals1;
				<<GetTopGamesList>b__0>d.<>1__state = -1;
				<<GetTopGamesList>b__0>d.<>t__builder.Start<CommunityConfigsViewVM.<>c__DisplayClass45_0.<<GetTopGamesList>b__0>d>(ref <<GetTopGamesList>b__0>d);
				return <<GetTopGamesList>b__0>d.<>t__builder.Task;
			});
			if (this.CheckResponseError(CS$<>8__locals1.resp))
			{
				List<GameInfo> list = JsonConvert.DeserializeObject<List<GameInfo>>(await CS$<>8__locals1.resp.Content.ReadAsStringAsync());
				if (list != null)
				{
					list.ForEach(delegate(GameInfo item)
					{
						item.Parrent = CS$<>8__locals1.<>4__this;
					});
				}
				this.GamesList = list;
			}
		}

		private async void GetPopupGamesList()
		{
			List<GameInfo> list = await this.GetGamesList(false);
			this.PopupGamesList = list;
		}

		private async Task<List<GameInfo>> GetGamesList(bool showIsLoading = true)
		{
			CommunityConfigsViewVM.<>c__DisplayClass47_0 CS$<>8__locals1 = new CommunityConfigsViewVM.<>c__DisplayClass47_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.resp = null;
			await this.ExecDownloadProcess(showIsLoading, delegate
			{
				CommunityConfigsViewVM.<>c__DisplayClass47_0.<<GetGamesList>b__0>d <<GetGamesList>b__0>d;
				<<GetGamesList>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<GetGamesList>b__0>d.<>4__this = CS$<>8__locals1;
				<<GetGamesList>b__0>d.<>1__state = -1;
				<<GetGamesList>b__0>d.<>t__builder.Start<CommunityConfigsViewVM.<>c__DisplayClass47_0.<<GetGamesList>b__0>d>(ref <<GetGamesList>b__0>d);
				return <<GetGamesList>b__0>d.<>t__builder.Task;
			});
			List<GameInfo> list;
			if (!this.CheckResponseError(CS$<>8__locals1.resp))
			{
				list = null;
			}
			else
			{
				List<GameInfo> list2 = JsonConvert.DeserializeObject<List<GameInfo>>(await CS$<>8__locals1.resp.Content.ReadAsStringAsync());
				if (list2 != null)
				{
					list2.ForEach(delegate(GameInfo item)
					{
						item.Parrent = CS$<>8__locals1.<>4__this;
					});
				}
				list = list2;
			}
			return list;
		}

		private string GetCurrentControllerType()
		{
			string result = "";
			BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
			if (currentGamepad != null)
			{
				ControllerTypeEnum[] controllerTypeEnums = currentGamepad.ControllerTypeEnums;
				if (controllerTypeEnums != null)
				{
					controllerTypeEnums.ToList<ControllerTypeEnum>().ForEach(delegate(ControllerTypeEnum item)
					{
						if (item != null && !result.Contains(item.ToString()))
						{
							result = result + item.ToString() + ",";
						}
					});
				}
			}
			return result.Trim(',');
		}

		private async Task GetConfigsList()
		{
			CommunityConfigsViewVM.<>c__DisplayClass49_0 CS$<>8__locals1 = new CommunityConfigsViewVM.<>c__DisplayClass49_0();
			CS$<>8__locals1.resp = null;
			CS$<>8__locals1.requestStr = string.Format("https://software-api.rewasd.com/configs/search?game_id={0}&gamepad={1}", this.CurrentGame.Id, this.GetCurrentControllerType());
			await this.ExecDownloadProcess(true, delegate
			{
				CommunityConfigsViewVM.<>c__DisplayClass49_0.<<GetConfigsList>b__0>d <<GetConfigsList>b__0>d;
				<<GetConfigsList>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<GetConfigsList>b__0>d.<>4__this = CS$<>8__locals1;
				<<GetConfigsList>b__0>d.<>1__state = -1;
				<<GetConfigsList>b__0>d.<>t__builder.Start<CommunityConfigsViewVM.<>c__DisplayClass49_0.<<GetConfigsList>b__0>d>(ref <<GetConfigsList>b__0>d);
				return <<GetConfigsList>b__0>d.<>t__builder.Task;
			});
			if (this.CheckResponseError(CS$<>8__locals1.resp))
			{
				string text = await CS$<>8__locals1.resp.Content.ReadAsStringAsync();
				this.ConfigsList = JsonConvert.DeserializeObject<List<ConfigInfo>>(text);
			}
		}

		public DelegateCommand BackCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._backCommand) == null)
				{
					delegateCommand = (this._backCommand = new DelegateCommand(new Action(this.Back)));
				}
				return delegateCommand;
			}
		}

		public DelegateCommand TryAgainCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._tryAgainCommand) == null)
				{
					delegateCommand = (this._tryAgainCommand = new DelegateCommand(new Action(this.TryAgain)));
				}
				return delegateCommand;
			}
		}

		public DelegateCommand SearchGamesCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._searchGamesCommand) == null)
				{
					delegateCommand = (this._searchGamesCommand = new DelegateCommand(new Action(this.SearchGames)));
				}
				return delegateCommand;
			}
		}

		public DelegateCommand OpenCommunityCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._openCommunityCommand) == null)
				{
					delegateCommand = (this._openCommunityCommand = new DelegateCommand(new Action(this.OpenCommunity)));
				}
				return delegateCommand;
			}
		}

		public DelegateCommand ClearGameNameCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._clearGameNameCommand) == null)
				{
					delegateCommand = (this._clearGameNameCommand = new DelegateCommand(new Action(this.ClearGameName)));
				}
				return delegateCommand;
			}
		}

		private void ClearGameName()
		{
			this.GameName = "";
		}

		private void Back()
		{
			this._currentGame = null;
			if (this.ConfigsList != null)
			{
				this.ConfigsList = null;
				this.GameName = "";
				return;
			}
			reWASDApplicationCommands.NavigateContentCommand.Execute(typeof(MainContent));
			IContainerProviderExtensions.Resolve<GamepadSelectorVM>(App.Container).SetCurrentGameAndProfileByCurentGamepadActiveSlot();
		}

		public void ResetAllViews()
		{
			this.PopupGamesList = null;
			this.ConfigsList = null;
			this.GamesList = null;
			this.CurrentGame = null;
		}

		private async void SearchGames()
		{
			this.PopupGamesList = null;
			if (string.IsNullOrEmpty(this.GameName))
			{
				await this.GetTopGamesList();
			}
			else
			{
				this.GamesList = await this.GetGamesList(true);
			}
			this.ConfigsList = null;
			this._currentGame = null;
			this.OnPropertyChanged("CurrentGame");
		}

		private void OpenCommunity()
		{
			if (this.CurrentGame != null)
			{
				Process.Start(new ProcessStartInfo("https://www.rewasd.com/community/games/" + this.CurrentGame.Url)
				{
					UseShellExecute = true
				});
				return;
			}
			Process.Start(new ProcessStartInfo("https://www.rewasd.com/community/")
			{
				UseShellExecute = true
			});
		}

		private void TryAgain()
		{
			this.IsInternetError = false;
		}

		private const string COMMUNITY_GAME_URL = "https://www.rewasd.com/community/games/";

		private const string TOP_GAMES_URL = "https://software-api.rewasd.com/games/top?limit=6";

		private const string GAMES_LIST_URL = "https://software-api.rewasd.com/games/suggestions?query=";

		private const string CONFIGS_LIST_URL = "https://software-api.rewasd.com/configs/search?game_id={0}&gamepad={1}";

		public const string CONFIG_DOWNLOAD_URL = "https://rewasd.com/community/config/download?hash=";

		public const string GAME_IMAGE_URL = "https://img.cdn.rewasd.com";

		private string _gameName;

		private List<GameInfo> _popupGamesList;

		private List<GameInfo> _gamesList;

		private GameInfo _currentGame;

		private List<ConfigInfo> _configsList;

		private bool _isInternetError;

		private bool _isLoading;

		private DelegateCommand _backCommand;

		private DelegateCommand _tryAgainCommand;

		private DelegateCommand _searchGamesCommand;

		private DelegateCommand _openCommunityCommand;

		private DelegateCommand _clearGameNameCommand;
	}
}
