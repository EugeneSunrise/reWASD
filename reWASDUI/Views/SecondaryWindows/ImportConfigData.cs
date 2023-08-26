using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using reWASDCommon.Infrastructure.JSONModel;
using reWASDCommon.Infrastructure.XML;
using reWASDUI.DataModels;
using XBEliteWPF.Utils.XBUtilModel;

namespace reWASDUI.Views.SecondaryWindows
{
	public class ImportConfigData : ZBindableBase
	{
		public ImportConfigData(string configPath, GameVM currentGame, bool isCloning)
		{
			if (!App.GameProfilesService.IsServiceInited)
			{
				Tracer.TraceWrite("ImportConfigData: GameProfilesService not inited", false);
			}
			this.ConfigSourcePath = configPath;
			this.SelectedGame = currentGame;
			this.IsCreateNew = true;
			this.NewProfileName = "Game";
			this.IsCloning = isCloning;
			try
			{
				if (currentGame != null)
				{
					this.IsUseExisting = true;
					string text = Path.Combine(App.UserSettingsService.ConfigsFolderPath, currentGame.Name + "\\Controller\\");
					if (isCloning)
					{
						this.ConfigName = Path.GetFileNameWithoutExtension(configPath);
					}
					else
					{
						this.ConfigName = Path.GetFileNameWithoutExtension(DSUtils.GetUniqueFileName(text, "Config.rewasd"));
					}
				}
				else if (!string.IsNullOrEmpty(configPath))
				{
					if (XBUtils.CheckProfileFirstChar(configPath, '{'))
					{
						string jsonAppNameValue = "";
						ConfigJSON_3_0 configJSON_3_ = new ConfigJSON_3_0();
						jsonAppNameValue = configJSON_3_.GetAppNameFromFile(configPath);
						jsonAppNameValue = XBUtils.TrimInvalidConfigChars(jsonAppNameValue);
						if (!string.IsNullOrEmpty(jsonAppNameValue))
						{
							GameVM gameVM = this.GamesCollection.SingleOrDefault((GameVM item) => item.Name.ToLower() == jsonAppNameValue.ToLower());
							if (gameVM != null)
							{
								this.IsUseExisting = true;
								this.SelectedGame = gameVM;
							}
							this.NewProfileName = jsonAppNameValue;
						}
					}
					else if (XBUtils.CheckProfileFirstChar(configPath, '<'))
					{
						string xmlAppNameValue = "";
						double num = 0.0;
						App.ConfigFileService.XMLGetVersion(configPath, ref num);
						if (num == 1.1)
						{
							new reWASDconfigWrapper_1_1().GetNodeValue(configPath, "app_name", ref xmlAppNameValue);
						}
						else if (num == 1.2)
						{
							new reWASDconfigWrapper_1_2().GetNodeValue(configPath, "app_name", ref xmlAppNameValue);
						}
						else if (num == 2.0)
						{
							new reWASDconfigWrapper_2_0().GetNodeValue(configPath, "app_name", ref xmlAppNameValue);
						}
						else if (num == 2.1)
						{
							new reWASDconfigWrapper_2_1().GetNodeValue(configPath, "app_name", ref xmlAppNameValue);
						}
						xmlAppNameValue = XBUtils.TrimInvalidConfigChars(xmlAppNameValue);
						if (!string.IsNullOrEmpty(xmlAppNameValue))
						{
							GameVM gameVM2 = this.GamesCollection.SingleOrDefault((GameVM item) => item.Name == xmlAppNameValue);
							if (gameVM2 != null)
							{
								this.IsUseExisting = true;
								this.SelectedGame = gameVM2;
							}
							this.NewProfileName = xmlAppNameValue;
						}
					}
					this.ConfigName = Path.GetFileNameWithoutExtension(configPath);
				}
			}
			catch (Exception)
			{
			}
			if (this.SelectedGame == null)
			{
				ObservableCollection<GameVM> gamesCollection = this.GamesCollection;
				if (gamesCollection == null || gamesCollection.Count != 0)
				{
					this.SelectedGame = this.GamesCollection[0];
				}
			}
		}

		public string ConfigSourcePath
		{
			get
			{
				return this._configSourcePath;
			}
			set
			{
				if (this.SetProperty<string>(ref this._configSourcePath, value, "ConfigSourcePath"))
				{
					this.OnPropertyChanged("IsSourceSelected");
				}
			}
		}

		public bool IsSourceSelected
		{
			get
			{
				return !string.IsNullOrEmpty(this._configSourcePath);
			}
		}

		public string NewProfileName
		{
			get
			{
				return this._newConfigName;
			}
			set
			{
				this.SetProperty<string>(ref this._newConfigName, value, "NewProfileName");
			}
		}

		public string BoxArtPath
		{
			get
			{
				return this._boxArtPath;
			}
			set
			{
				this.SetProperty<string>(ref this._boxArtPath, value, "BoxArtPath");
			}
		}

		public string ConfigName
		{
			get
			{
				return this._configName;
			}
			set
			{
				this.SetProperty<string>(ref this._configName, value, "ConfigName");
			}
		}

		public bool IsCreateNew
		{
			get
			{
				return this._createNew;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._createNew, value, "IsCreateNew"))
				{
					this.OnPropertyChanged("IsUseExisting");
				}
			}
		}

		public bool IsUseExisting
		{
			get
			{
				return !this._createNew;
			}
			set
			{
				if (value != this._createNew)
				{
					return;
				}
				this._createNew = !value;
				this.OnPropertyChanged("IsCreateNew");
				this.OnPropertyChanged("IsUseExisting");
			}
		}

		public bool IsCloning
		{
			get
			{
				return this._isCloning;
			}
			set
			{
				this.SetProperty<bool>(ref this._isCloning, value, "IsCloning");
			}
		}

		public ObservableCollection<GameVM> GamesCollection
		{
			get
			{
				return App.GameProfilesService.GamesCollection;
			}
		}

		public GameVM SelectedGame
		{
			get
			{
				return this._selectedGame;
			}
			set
			{
				this.SetProperty<GameVM>(ref this._selectedGame, value, "SelectedGame");
			}
		}

		private bool _createNew;

		private string _newConfigName;

		private string _boxArtPath = "";

		private string _configName = "";

		private string _configSourcePath;

		private GameVM _selectedGame;

		private bool _isCloning;
	}
}
