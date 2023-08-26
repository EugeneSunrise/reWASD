using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using DiscSoft.NET.Common.ViewModel.BindableBase;
using GenGamepadView.Views;
using Prism.Commands;
using reWASDCommon;
using reWASDCommon.Infrastructure;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Utils.WebUtils;
using reWASDUI.Views;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Infrastructure.KeyBindingsModel.ControllerBindings;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.ViewModels.CommunityConfigs
{
	[DataContract]
	public class ConfigInfo : ZBindableBase
	{
		[DataMember(Name = "id")]
		public string Id { get; set; }

		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "comment")]
		public string Comment { get; set; }

		[DataMember(Name = "created_at")]
		public string CreatedAt { get; set; }

		[DataMember(Name = "file_name")]
		public string FileName { get; set; }

		[DataMember(Name = "username")]
		public string UserName { get; set; }

		[DataMember(Name = "hash")]
		public string Hash { get; set; }

		[DataMember(Name = "gamepads")]
		public Gamepads Gamepads { get; set; }

		[DataMember(Name = "devices")]
		public SupportedGamepads Devices { get; set; }

		[DataMember(Name = "downloads_count")]
		public string DownloadsCount { get; set; }

		[DataMember(Name = "rating")]
		public string Rating
		{
			get
			{
				return this._rating;
			}
			set
			{
				try
				{
					this._rating = "n/a";
					this._rating = Convert.ToDouble(value).ToString("0.0", CultureInfo.InvariantCulture);
				}
				catch (Exception)
				{
				}
			}
		}

		public string CreatedBy
		{
			get
			{
				return string.Format(DTLocalization.GetString(12726), this.UserName, this.CreatedAt);
			}
		}

		public string DownloadedFileName { get; set; }

		public ConfigData Config
		{
			get
			{
				return this._config;
			}
			set
			{
				if (this.SetProperty<ConfigData>(ref this._config, value, "Config"))
				{
					this.OnPropertyChanged("ExisingSubConfigs");
				}
			}
		}

		public List<SubConfigData> ExisingSubConfigs
		{
			get
			{
				ConfigData config = this.Config;
				if (config == null)
				{
					return null;
				}
				return config.Where(delegate(SubConfigData item)
				{
					if (item.IsKeyboard)
					{
						return item.MainXBBindingCollection.ControllerBindings.Any((ControllerBinding x) => x.XBBinding.KeyScanCode.IsCategoryAllKeyboardTypes);
					}
					return true;
				}).ToList<SubConfigData>();
			}
		}

		public SubConfigData CurrentSubconfig
		{
			get
			{
				return this._currentSubconfig;
			}
			set
			{
				if (this.SetProperty<SubConfigData>(ref this._currentSubconfig, value, "CurrentSubconfig") && value != null)
				{
					if (this._currentSubconfig.IsKeyboard)
					{
						this.UIContent = new GenGamepadView.Views.KeyboardMappingView(this._currentSubconfig.MainXBBindingCollection, "KeyBoard");
						return;
					}
					if (this._currentSubconfig.IsMouse)
					{
						this.UIContent = new GenGamepadView.Views.MouseMappingView(this._currentSubconfig.MainXBBindingCollection);
						return;
					}
					BaseControllerVM currentGamepad = App.GamepadService.CurrentGamepad;
					ControllerTypeEnum? controllerTypeEnum;
					if (currentGamepad == null)
					{
						controllerTypeEnum = null;
					}
					else
					{
						ControllerVM currentController = currentGamepad.CurrentController;
						controllerTypeEnum = ((currentController != null) ? new ControllerTypeEnum?(currentController.ControllerType) : null);
					}
					ControllerTypeEnum? controllerTypeEnum2 = controllerTypeEnum;
					this.UIContent = new GenGamepadView.Views.SVGGamepadWithAllAnnotations(this._currentSubconfig.MainXBBindingCollection, (controllerTypeEnum2 != null && ControllerTypeExtensions.IsGamepad(controllerTypeEnum2.GetValueOrDefault())) ? controllerTypeEnum2.Value : 2);
				}
			}
		}

		public bool IsConfigVisible
		{
			get
			{
				return this._IsConfigVisible;
			}
			set
			{
				this.SetProperty<bool>(ref this._IsConfigVisible, value, "IsConfigVisible");
			}
		}

		public UserControl UIContent
		{
			get
			{
				return this._UIContent;
			}
			set
			{
				this.SetProperty<UserControl>(ref this._UIContent, value, "UIContent");
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

		public DelegateCommand ImportCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._importCommand) == null)
				{
					delegateCommand = (this._importCommand = new DelegateCommand(new Action(this.Import)));
				}
				return delegateCommand;
			}
		}

		private void Import()
		{
			this.DownloadConfig(new AsyncCompletedEventHandler(this.ImportDownloadComplete));
		}

		private async void ImportDownloadComplete(object sender, AsyncCompletedEventArgs args)
		{
			if (args.Error != null)
			{
				if (DTMessageBox.Show(DTLocalization.GetString(12734), MessageBoxButton.OKCancel, MessageBoxImage.Hand, DTLocalization.GetString(5020), false, MessageBoxResult.None) == MessageBoxResult.OK)
				{
					this.Import();
				}
			}
			else
			{
				TaskAwaiter<bool> taskAwaiter = App.GuiHelperService.ImportGameConfig(this.DownloadedFileName, false).GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<bool> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				if (taskAwaiter.GetResult())
				{
					reWASDApplicationCommands.NavigateContentCommand.Execute(typeof(MainContent));
				}
			}
		}

		public DelegateCommand ShowCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._showCommand) == null)
				{
					delegateCommand = (this._showCommand = new DelegateCommand(new Action(this.Show)));
				}
				return delegateCommand;
			}
		}

		private void PreviewDownloadComplete(object sender, AsyncCompletedEventArgs args)
		{
			if (args.Error != null)
			{
				this.IsConfigVisible = false;
				if (DTMessageBox.Show(DTLocalization.GetString(12734), MessageBoxButton.OKCancel, MessageBoxImage.Hand, DTLocalization.GetString(5020), false, MessageBoxResult.None) == MessageBoxResult.OK)
				{
					this.Show();
				}
				return;
			}
			ConfigData configData;
			bool flag;
			if (!new ConfigFileService().ParseConfigFile(this.DownloadedFileName, ref configData, ref flag, true, null, true))
			{
				return;
			}
			this.Config = configData;
			PeripheralVM peripheralVM = App.GamepadService.CurrentGamepad as PeripheralVM;
			if (peripheralVM != null)
			{
				if (peripheralVM.PeripheralPhysicalType == 1)
				{
					this.CurrentSubconfig = this.Config.FindSubConfigKeyboard(0, false);
				}
				else if (peripheralVM.PeripheralPhysicalType == 2)
				{
					this.CurrentSubconfig = this.Config.FindSubConfigMouse(0, false);
				}
			}
			else
			{
				this.CurrentSubconfig = this.Config[0];
			}
			if (this.ExisingSubConfigs != null && !this.ExisingSubConfigs.Contains(this.CurrentSubconfig) && this.ExisingSubConfigs.Count != 0)
			{
				this.CurrentSubconfig = this.ExisingSubConfigs[0];
			}
			this.OnPropertyChanged("UIContent");
		}

		private async void Show()
		{
			if (this.IsConfigVisible)
			{
				this.IsConfigVisible = false;
			}
			else
			{
				this.IsConfigVisible = true;
				this.IsLoading = true;
				await this.DownloadConfig(new AsyncCompletedEventHandler(this.PreviewDownloadComplete));
				this.IsLoading = false;
			}
		}

		private async Task DownloadConfig(AsyncCompletedEventHandler DownloadFileCompleted)
		{
			string text = Path.Combine(Constants.PROGRAMM_DATA_DIRECTORY_PATH, "downloaded");
			try
			{
				Directory.CreateDirectory(text);
			}
			catch (Exception)
			{
			}
			this.DownloadedFileName = Path.Combine(text, this.FileName);
			await Downloader.DownloadFile("https://rewasd.com/community/config/download?hash=" + this.Hash, this.DownloadedFileName, DownloadFileCompleted);
		}

		private string _rating;

		public ConfigData _config;

		private SubConfigData _currentSubconfig;

		private bool _IsConfigVisible;

		private UserControl _UIContent;

		private bool _isLoading;

		public DelegateCommand _importCommand;

		public DelegateCommand _showCommand;
	}
}
