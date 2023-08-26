using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Attributes;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Services.Interfaces;
using XBEliteWPF.Services.Interfaces;
using XBEliteWPF.Utils;
using XBEliteWPF.Utils.XBUtilModel;

namespace reWASDUI.DataModels
{
	public class GameVM : ZBindableBase, IComparable
	{
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this.SetProperty<string>(ref this._name, value, "Name");
			}
		}

		public bool DiscardConfigChangesWithoutAskingUser
		{
			get
			{
				return this._discardConfigChangesWithoutAskingUser;
			}
			set
			{
				this.SetProperty<bool>(ref this._discardConfigChangesWithoutAskingUser, value, "DiscardConfigChangesWithoutAskingUser");
			}
		}

		public System.Drawing.Brush GameVmDataTemplateBackgroundColor
		{
			get
			{
				return this._gameVmDataTemplateBackgroundColor;
			}
			set
			{
				this._gameVmDataTemplateBackgroundColor = value;
				this.NotifyPropertyChanged("GameVmDataTemplateBackgroundColor");
			}
		}

		public System.Drawing.Brush GameVmDataTemplateBackgroundColor1
		{
			get
			{
				return this._gameVmDataTemplateBackgroundColor1;
			}
			set
			{
				this._gameVmDataTemplateBackgroundColor1 = value;
				this.NotifyPropertyChanged("GameVmDataTemplateBackgroundColor1");
			}
		}

		public new event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public bool IsAutodetect
		{
			get
			{
				return this._isAutodetect;
			}
			set
			{
				if (this.SetProperty<bool>(ref this._isAutodetect, value, "IsAutodetect"))
				{
					PropertyChangedExtendedEventHandler<bool> isAutoDetectChanged = this.IsAutoDetectChanged;
					if (isAutoDetectChanged != null)
					{
						isAutoDetectChanged(this, new PropertyChangedExtendedEventArgs<bool>("IsAutodetect", !value, value));
					}
					string text = this.GetGameFolderPath() + "\\IsAutodetect";
					try
					{
						if (value && !File.Exists(text))
						{
							File.Create(text).Close();
						}
						else if (File.Exists(text))
						{
							File.Delete(text);
						}
					}
					catch (Exception ex)
					{
						Tracer.TraceException(ex, "IsAutodetect");
					}
					foreach (ConfigVM configVM in this.ConfigCollection)
					{
						configVM.AutodetectForConfigChanged();
					}
					App.GamepadService.BinDataSerialize.SaveAutoGamesDetectionGamepadProfileRelations();
				}
			}
		}

		public bool HasAnyAutodetectProcesses
		{
			get
			{
				ConfigVM configVM = this.ConfigCollection.FirstOrDefault<ConfigVM>();
				object obj;
				if (configVM == null)
				{
					obj = null;
				}
				else
				{
					ConfigData configData = configVM.ConfigData;
					if (configData == null)
					{
						obj = null;
					}
					else
					{
						obj = configData.FirstOrDefault((SubConfigData c) => c.MainXBBindingCollection.ProcessNames.Count > 0);
					}
				}
				return obj != null;
			}
		}

		public ImageSource GameImageSource
		{
			get
			{
				if (this._imageSource == null)
				{
					this.SetImageSource(true);
				}
				return this._imageSource;
			}
			set
			{
				this.SetProperty<ImageSource>(ref this._imageSource, value, "GameImageSource");
				this.OnPropertyChanged("IsImageVisible");
			}
		}

		public bool IsImageVisible
		{
			get
			{
				return this._imageSource != null;
			}
		}

		[JsonProperty("ConfigInfoList")]
		public ObservableCollection<ConfigVM> ConfigCollection
		{
			get
			{
				if (this._configCollection == null)
				{
					this._configCollection = new ObservableCollection<ConfigVM>();
				}
				return this._configCollection;
			}
			set
			{
				this.SetProperty<ObservableCollection<ConfigVM>>(ref this._configCollection, value, "ConfigCollection");
			}
		}

		[DoNotTrackPropertyChangedViaCollectionItemPropertyChangedEvent]
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

		public bool HasTheOnlyOneProfile
		{
			get
			{
				return this.ConfigCollection.Count == 1;
			}
		}

		public ConfigVM ConfigForSharing
		{
			get
			{
				return this.ConfigCollection.FirstOrDefault<ConfigVM>();
			}
		}

		public async Task SetCurrentConfig(ConfigVM config)
		{
			if (this._currentConfig != config)
			{
				PropertyChangedExtendedEventArgs<ConfigVM> args = new PropertyChangedExtendedEventArgs<ConfigVM>("CurrentConfig", this._currentConfig, config);
				if (this._currentConfig != null && this._currentConfig.IsChangedIncludingShiftCollections && !this._discardConfigChangesWithoutAskingUser && !this._currentConfig.IsLoading)
				{
					switch (this._currentConfig.TryAskUserToSaveChanges(true))
					{
					case 1:
						this._currentConfig.SaveConfigCommand.Execute();
						break;
					case 2:
					{
						await this._currentConfig.ReadConfigFromJsonAsync(true);
						PropertyChangedExtendedEventHandler<ConfigVM> currentConfigChanged = this.CurrentConfigChanged;
						if (currentConfigChanged != null)
						{
							currentConfigChanged(this, args);
						}
						break;
					}
					case 3:
					{
						ConfigVM oldValue = args.OldValue;
						this._currentConfig = oldValue;
						config = oldValue;
						break;
					}
					}
				}
				if (config != null)
				{
					await config.ReadConfigFromJsonIfNotLoaded();
				}
				if (this.SetProperty<ConfigVM>(ref this._currentConfig, config, "CurrentConfig"))
				{
					PropertyChangedExtendedEventHandler<ConfigVM> currentConfigChanged2 = this.CurrentConfigChanged;
					if (currentConfigChanged2 != null)
					{
						currentConfigChanged2(this, args);
					}
					this._gameProfilesService.ChangeCurrentShiftCollection(new int?(0), false);
					App.EventAggregator.GetEvent<CurrentConfigChanged>().Publish(this._currentConfig);
				}
				GC.Collect();
				GC.WaitForFullGCComplete(-1);
			}
		}

		public static Image UniformToFillImage(Image image, int width, int height)
		{
			int width2 = image.Width;
			int height2 = image.Height;
			double num = (double)width / (double)width2;
			double num2 = Math.Max((double)height / (double)height2, num);
			double num3 = ((double)width - (double)width2 * num2) / 2.0;
			double num4 = ((double)height - (double)height2 * num2) / 2.0;
			int num5 = (int)Math.Round((double)width2 * num2);
			int num6 = (int)Math.Round((double)height2 * num2);
			Bitmap bitmap = new Bitmap(num5 + (int)Math.Round(2.0 * num3), num6 + (int)Math.Round(2.0 * num4));
			Image image2;
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				Rectangle rectangle = new Rectangle((int)Math.Round(num3), (int)Math.Round(num4), num5, num6);
				Rectangle rectangle2 = new Rectangle(0, 0, width2, height2);
				graphics.DrawImage(image, rectangle, rectangle2, GraphicsUnit.Pixel);
				image2 = bitmap;
			}
			return image2;
		}

		public static void SaveGameThumbnail(string sImageSourceFilePath, string sImageSourceDestinationFilePath)
		{
			try
			{
				GameVM.UniformToFillImage(Image.FromFile(sImageSourceFilePath), 1280, 800).Save(sImageSourceDestinationFilePath);
			}
			catch (Exception ex)
			{
				Tracer.TraceException(ex, "SaveGameThumbnail");
				DTMessageBox.Show(DTLocalization.GetString(11015), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
			}
		}

		public string GetGameFolderPath()
		{
			return this._gameProfilesService.GamesFolderPath + "\\" + this.Name;
		}

		public string GetImageSourcePath()
		{
			return this.GetGameFolderPath() + "\\IcoGame.png";
		}

		public async Task<ConfigVM> ImportConfigFile(string configName, string configSourcePath, bool isCloning)
		{
			string text = File.ReadAllText(configSourcePath);
			ImportConfigInfo importParams = new ImportConfigInfo
			{
				GameName = this.Name,
				Name = configName,
				Content = text,
				IsCloning = isCloning
			};
			ImportConfigResult importConfigResult = await this._httpClientService.GameProfiles.ImportConfig(importParams);
			ImportConfigResult importResult = importConfigResult;
			ConfigVM configVM;
			if (importResult.Status == 3)
			{
				if (!string.IsNullOrEmpty(importResult.ErrorText))
				{
					DTMessageBox.Show(importResult.ErrorText, MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
				}
				configVM = null;
			}
			else if (importResult.Status == 1)
			{
				DTMessageBox.Show(DTLocalization.GetString(11215), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
				configVM = null;
			}
			else
			{
				if (importResult.Status == 2)
				{
					if (DTMessageBox.Show(System.Windows.Application.Current.MainWindow, DTLocalization.GetString(11018), MessageBoxButton.YesNo, MessageBoxImage.Question, null) != MessageBoxResult.Yes)
					{
						return null;
					}
					importParams.RewriteExisting = true;
					importConfigResult = await this._httpClientService.GameProfiles.ImportConfig(importParams);
					importResult = importConfigResult;
				}
				await this.FillConfigsCollection();
				configVM = this.ConfigCollection.FirstOrDefault((ConfigVM c) => c.Name == importResult.ImportedConfigName);
			}
			return configVM;
		}

		public event PropertyChangedExtendedEventHandler<ConfigVM> CurrentConfigChanged;

		public event PropertyChangedExtendedEventHandler<bool> IsAutoDetectChanged;

		[JsonConstructor]
		public GameVM(string name)
		{
			this._gameProfilesService = App.GameProfilesService;
			this._configFileService = App.ConfigFileService;
			this._eventAggregator = App.EventAggregator;
			this._gamepadServiceLazy = App.GamepadServiceLazy;
			this._licensingService = App.LicensingService;
			this._httpClientService = App.HttpClientService;
			this._name = name;
			this.ResolveAutoDetect();
		}

		public GameVM(string name, IGameProfilesService gps, IConfigFileService ifs, IEventAggregator ea, Lazy<IGamepadService> gsLazy, ILicensingService ls, IHttpClientService hs)
		{
			this._gameProfilesService = gps;
			this._configFileService = ifs;
			this._eventAggregator = ea;
			this._gamepadServiceLazy = gsLazy;
			this._licensingService = ls;
			this._httpClientService = hs;
			this._name = name;
			this.ResolveAutoDetect();
		}

		public void SetAutoDetect(bool value, bool fireChangedEvent = false)
		{
			if (value)
			{
				this.ConfigCollection.ForEach(async delegate(ConfigVM p)
				{
					await p.ReadConfigFromJsonIfNotLoaded();
				});
			}
			if (fireChangedEvent)
			{
				this.IsAutodetect = value;
				return;
			}
			this._isAutodetect = value;
			this.OnPropertyChanged("IsAutodetect");
		}

		public void ResolveAutoDetect()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(this.GetGameFolderPath());
			bool flag = File.Exists(((directoryInfo != null) ? directoryInfo.ToString() : null) + "\\IsAutodetect");
			this.SetAutoDetect(flag, false);
		}

		public void SetImageSource(bool setBackingField = false)
		{
			ImageSource imageSource = DSUtils.GetImageSourceFromFile(this.GetImageSourcePath());
			if (imageSource == null)
			{
				if (GameVM._defaultGameBackground == null)
				{
					GameVM._defaultGameBackground = System.Windows.Application.Current.TryFindResource("DefaultGameBackground") as ImageSource;
				}
				imageSource = GameVM._defaultGameBackground;
			}
			if (setBackingField)
			{
				this._imageSource = imageSource;
				return;
			}
			this.GameImageSource = imageSource;
		}

		public async void DeleteConfig(ConfigVM config)
		{
			if (config != null)
			{
				this.ConfigCollection.Remove(config);
				this._discardConfigChangesWithoutAskingUser = true;
				if (config == this.CurrentConfig)
				{
					await this.SetCurrentConfig(this.ConfigCollection.FirstOrDefault<ConfigVM>());
					config.ChangeCurrentMainWrapperAccordingToControllerVM(this._gamepadServiceLazy.Value.CurrentGamepad, true);
				}
				this._discardConfigChangesWithoutAskingUser = false;
				if (this.CurrentConfig == null)
				{
					this.IsAutodetect = false;
				}
				this.OnPropertyChanged("HasTheOnlyOneProfile");
				this.OnPropertyChanged("ConfigForSharing");
				IGamepadService value = this._gamepadServiceLazy.Value;
				value.GamepadProfileRelations.RemoveConfig(config);
				value.AutoGamesDetectionGamepadProfileRelations.RemoveConfig(config);
				Lazy<IGamepadService> gamepadServiceLazy = this._gamepadServiceLazy;
				if (gamepadServiceLazy != null)
				{
					IGamepadService value2 = gamepadServiceLazy.Value;
					if (value2 != null)
					{
						ExternalDeviceRelationsHelper externalDeviceRelationsHelper = value2.ExternalDeviceRelationsHelper;
						if (externalDeviceRelationsHelper != null)
						{
							externalDeviceRelationsHelper.RemoveConfigRelations(this.Name, config.Name);
						}
					}
				}
			}
		}

		public async Task FillConfigsCollection()
		{
			ObservableCollection<ConfigVM> collection = new ObservableCollection<ConfigVM>();
			Collection<ConfigVM> collection2 = collection;
			ObservableCollection<ConfigVM> observableCollection = await this._httpClientService.GameProfiles.GetConfigsCollection(this.Name);
			CollectionExtensions.AddRange<ConfigVM>(collection2, observableCollection);
			collection2 = null;
			collection.ForEach(delegate(ConfigVM config)
			{
				config.ParentGame = this;
			});
			this.ConfigCollection = collection;
		}

		public async void FillSingleConfig(string configSourcePath)
		{
			if (configSourcePath.EndsWith(".rewasd"))
			{
				ConfigVM configVM = new ConfigVM(configSourcePath, this.Name, this);
				configVM.GameName = this.Name;
				TaskAwaiter<bool> taskAwaiter = configVM.ReadConfigFromJsonAsync(false).GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<bool> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				if (taskAwaiter.GetResult())
				{
					this.ConfigCollection.Remove((ConfigVM item) => item.ConfigPath == configSourcePath);
					this.ConfigCollection.Add(configVM);
					SenderGoogleAnalytics.SendNewProfileAddedEvent(this.ConfigCollection.Count.ToString());
					XBUtils.SortObservableCollection<ConfigVM>(this.ConfigCollection);
					this.OnPropertyChanged("HasTheOnlyOneProfile");
					this.OnPropertyChanged("ConfigForSharing");
				}
				configVM = null;
			}
		}

		public override void Dispose()
		{
			base.Dispose();
			foreach (ConfigVM configVM in this.ConfigCollection)
			{
				configVM.IsLoaded = false;
				configVM.IsLoadedSuccessfully = null;
				if (configVM.ConfigData != null)
				{
					configVM.ConfigData.Dispose();
					GC.SuppressFinalize(configVM.ConfigData);
				}
			}
			GC.Collect();
		}

		public DelegateCommand CreateConfigCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._CreateConfig) == null)
				{
					delegateCommand = (this._CreateConfig = new DelegateCommand(delegate
					{
						IGuiHelperService guiHelperService = App.GuiHelperService;
						if (guiHelperService == null)
						{
							return;
						}
						guiHelperService.CreateConfigExecute(this);
					}));
				}
				return delegateCommand;
			}
		}

		public DelegateCommand ImportConfigCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._importConfig) == null)
				{
					delegateCommand = (this._importConfig = new DelegateCommand(delegate
					{
						this.ImportConfig();
					}));
				}
				return delegateCommand;
			}
		}

		private async void ImportConfig()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = DTLocalization.GetString(11027) + " (*.rewasd) | *.rewasd";
			openFileDialog.CheckFileExists = true;
			openFileDialog.Multiselect = false;
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				ConfigVM configVM = await this.ImportConfigFile(DSUtils.GetFileNameWithoutExtension(openFileDialog.FileName), openFileDialog.FileName, true);
				ConfigVM newConfig = configVM;
				await this.SetCurrentConfig(newConfig);
				App.EventAggregator.GetEvent<ConfigCreatedByUI>().Publish(newConfig);
				newConfig = null;
			}
		}

		public DelegateCommand EditGameCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._EditGame) == null)
				{
					delegateCommand = (this._EditGame = new DelegateCommand(delegate
					{
						IGuiHelperService guiHelperService = App.GuiHelperService;
						if (guiHelperService == null)
						{
							return;
						}
						guiHelperService.EditGameExecute(this);
					}));
				}
				return delegateCommand;
			}
		}

		public DelegateCommand RemoveGameCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._RemoveGame) == null)
				{
					delegateCommand = (this._RemoveGame = new DelegateCommand(delegate
					{
						IGuiHelperService guiHelperService = App.GuiHelperService;
						if (guiHelperService == null)
						{
							return;
						}
						guiHelperService.RemoveGameExecute(this);
					}));
				}
				return delegateCommand;
			}
		}

		public DelegateCommand CloneGameCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._cloneGame) == null)
				{
					delegateCommand = (this._cloneGame = new DelegateCommand(delegate
					{
						IGuiHelperService guiHelperService = App.GuiHelperService;
						if (guiHelperService == null)
						{
							return;
						}
						guiHelperService.CloneGameExecute(this);
					}));
				}
				return delegateCommand;
			}
		}

		public DelegateCommand SaveAsGameCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._saveAsGameCommand) == null)
				{
					delegateCommand = (this._saveAsGameCommand = new DelegateCommand(delegate
					{
						IGuiHelperService guiHelperService = App.GuiHelperService;
						if (guiHelperService == null)
						{
							return;
						}
						guiHelperService.SaveAsGameExecute(this);
					}));
				}
				return delegateCommand;
			}
		}

		public int CompareTo(object obj)
		{
			GameVM gameVM = (GameVM)obj;
			return string.Compare(this.Name, gameVM.Name, StringComparison.CurrentCulture);
		}

		private static ImageSource _defaultGameBackground;

		private IGameProfilesService _gameProfilesService;

		private IConfigFileService _configFileService;

		private IEventAggregator _eventAggregator;

		private Lazy<IGamepadService> _gamepadServiceLazy;

		private ILicensingService _licensingService;

		private IHttpClientService _httpClientService;

		private bool _discardConfigChangesWithoutAskingUser;

		private string _name;

		private bool _isAutodetect;

		private ImageSource _imageSource;

		private System.Drawing.Brush _gameVmDataTemplateBackgroundColor;

		private System.Drawing.Brush _gameVmDataTemplateBackgroundColor1;

		private ObservableCollection<ConfigVM> _configCollection;

		private ConfigVM _currentConfig;

		private DelegateCommand _CreateConfig;

		private DelegateCommand _importConfig;

		private DelegateCommand _EditGame;

		private DelegateCommand _RemoveGame;

		private DelegateCommand _cloneGame;

		private DelegateCommand _saveAsGameCommand;
	}
}
