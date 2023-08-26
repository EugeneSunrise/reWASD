using System;
using System.Collections.Generic;
using System.Windows.Input;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Clases;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using reWASDUI.DataModels;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Services;
using reWASDUI.Services.Interfaces;
using reWASDUI.ViewModels.Base;
using reWASDUI.Views;
using XBEliteWPF.Infrastructure;
using XBEliteWPF.Services.Interfaces;

namespace reWASDUI.ViewModels
{
	public class GamesSelectorVM : BaseControllerKeyPressedPollerVM
	{
		public bool IsGameListShown
		{
			get
			{
				return this._isGameListShown;
			}
			set
			{
				this.SetProperty<bool>(ref this._isGameListShown, value, "IsGameListShown");
			}
		}

		public ICommand ShowGamesListCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._showGamesList) == null)
				{
					relayCommand = (this._showGamesList = new RelayCommand(new Action(this.ExpandGamesList), new Func<bool>(this.ExpandGamesListCanExecute)));
				}
				return relayCommand;
			}
		}

		private void ExpandGamesList()
		{
			this.IsGameListShown = true;
		}

		private bool ExpandGamesListCanExecute()
		{
			return true;
		}

		public ICommand HideGamesListCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._hideGamesList) == null)
				{
					relayCommand = (this._hideGamesList = new RelayCommand(new Action(this.HideGamesList), new Func<bool>(this.HideGamesListCanExecute)));
				}
				return relayCommand;
			}
		}

		private bool HideGamesListCanExecute()
		{
			return true;
		}

		public DelegateCommand AddGameCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._AddGame) == null)
				{
					delegateCommand = (this._AddGame = new DelegateCommand(new Action(this.AddGame)));
				}
				return delegateCommand;
			}
		}

		private void AddGame()
		{
			base.GameProfilesService.AddGameProfile();
		}

		public DelegateCommand OpenCommunityCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._OpenCommunity) == null)
				{
					delegateCommand = (this._OpenCommunity = new DelegateCommand(new Action(this.OpenCommunity)));
				}
				return delegateCommand;
			}
		}

		public void OpenCommunity()
		{
			base.GameProfilesService.CurrentGame = null;
			reWASDApplicationCommands.NavigateContentCommand.Execute(typeof(CommunityConfigsView));
		}

		public ICommand EditCurrentGameCommand
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this._editCurrentGame) == null)
				{
					relayCommand = (this._editCurrentGame = new RelayCommand(new Action(this.EditCurrentGame), new Func<bool>(this.EditCurrentGameCanExecute)));
				}
				return relayCommand;
			}
		}

		private void EditCurrentGame()
		{
			base.GameProfilesService.CurrentGame.EditGameCommand.Execute();
		}

		private bool EditCurrentGameCanExecute()
		{
			return base.GameProfilesService.CurrentGame != null;
		}

		public override void OnControllerPollState(GamepadState gamepadState)
		{
		}

		public override void OnControllerKeyDown(List<GamepadButtonDescription> buttons)
		{
			this.HideGamesList();
		}

		public override void OnControllerKeyUp(List<GamepadButtonDescription> buttons)
		{
		}

		public GamesSelectorVM(IGameProfilesService gameProfilesService, IConfigFileService configFileService, IEventAggregator ea, IContainerProvider uc)
			: base(uc)
		{
			base.GameProfilesService = (GameProfilesService)gameProfilesService;
			base.GameProfilesService.CurrentGameChanged += this.GameProfilesServiceOnCurrentGameChanged;
			this._configFileService = configFileService;
			this._keyPressedPollerService = IContainerProviderExtensions.Resolve<IKeyPressedPollerService>(uc);
			this._keyPressedPollerService.Subscribe(this, false);
			App.EventAggregator.GetEvent<GamepadServiceInited>().Subscribe(delegate(object sender)
			{
				this.IsGameListShown = base.GamepadService.IsCurrentGamepadRemaped && base.GameProfilesService.CurrentGame == null;
			});
			App.EventAggregator.GetEvent<CurrentGamepadChanged>().Subscribe(delegate(BaseControllerVM sender)
			{
				this.OnCurrentGamepadChanged(sender);
			});
		}

		private void HideGamesList()
		{
			if (this.IsGameListShown)
			{
				this.IsGameListShown = false;
			}
		}

		private void OnCurrentGamepadChanged(BaseControllerVM controller)
		{
			MainWindowViewModel mainWindowViewModel = IContainerProviderExtensions.Resolve<MainWindowViewModel>(App.Container);
			if (base.GameProfilesService.CurrentGame == null && controller != null && mainWindowViewModel.IsMainContent())
			{
				this.ExpandGamesList();
			}
		}

		private void GameProfilesServiceOnCurrentGameChanged(object sender, PropertyChangedExtendedEventArgs<GameVM> e)
		{
			if (e.NewValue != null)
			{
				this.HideGamesList();
			}
		}

		private IConfigFileService _configFileService;

		private IKeyPressedPollerService _keyPressedPollerService;

		private bool _isGameListShown;

		private RelayCommand _showGamesList;

		private RelayCommand _hideGamesList;

		private DelegateCommand _AddGame;

		private DelegateCommand _OpenCommunity;

		private RelayCommand _editCurrentGame;
	}
}
