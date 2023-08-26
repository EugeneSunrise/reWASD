using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.View.Controls;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.Controls.ComboBoxes;
using DiscSoft.NET.Common.View.Controls.RadioButtons;
using DiscSoft.NET.Common.View.SecondaryWindows.Base;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using Microsoft.Win32;
using reWASDUI.DataModels;
using reWASDUI.Services.Interfaces;
using reWASDUI.Utils;
using reWASDUI.Utils.XBUtil;

namespace reWASDUI.Views.SecondaryWindows
{
	public partial class ImportConfig : BaseSecondaryWindow
	{
		public ImportConfig()
		{
			this.InitializeComponent();
		}

		public Task<bool?> ShowDialogAsync()
		{
			TaskCompletionSource<bool?> completion = new TaskCompletionSource<bool?>();
			base.Dispatcher.BeginInvoke(new Action(delegate
			{
				this.ShowDialog();
				completion.SetResult(new bool?(true));
			}), Array.Empty<object>());
			return completion.Task;
		}

		public void PrepareDialog(string configPath, GameVM currentGame, IConfigFileService _configService, bool isCloning)
		{
			this._config = new ImportConfigData(configPath, currentGame, isCloning);
			base.DataContext = this._config;
			this._configFileService = _configService;
			this.WindowResult = MessageBoxResult.Cancel;
		}

		public ImportConfigData Data
		{
			get
			{
				return this._config;
			}
		}

		public static bool IsShown
		{
			get
			{
				return ImportConfig._isShown;
			}
		}

		public new void ShowDialog()
		{
			ImportConfig._isShown = true;
			base.ShowDialog();
			ImportConfig._isShown = false;
		}

		private void ButtonBase_BrowseClick(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = DTLocalization.GetString(11027) + " (*.rewasd) | *.rewasd";
			openFileDialog.CheckFileExists = true;
			openFileDialog.Multiselect = false;
			bool? flag = openFileDialog.ShowDialog();
			bool flag2 = true;
			if ((flag.GetValueOrDefault() == flag2) & (flag != null))
			{
				this._config.ConfigSourcePath = openFileDialog.FileName;
				ConfigVM configVM = new ConfigVM(this._config.ConfigSourcePath, null, this._config.SelectedGame);
				configVM.InitConfig();
				XBUtils.CreateDefaultCollectionXBBindingWrappers(configVM, false);
				this._config.ConfigName = Path.GetFileNameWithoutExtension(this._config.ConfigSourcePath);
			}
		}

		private void ButtonBoxArt_BrowseClick(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = DTLocalization.GetString(11028) + " (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
			openFileDialog.CheckFileExists = true;
			openFileDialog.Multiselect = false;
			bool? flag = openFileDialog.ShowDialog();
			bool flag2 = true;
			if ((flag.GetValueOrDefault() == flag2) & (flag != null))
			{
				this._config.BoxArtPath = openFileDialog.FileName;
			}
		}

		protected override void OkButton_Click(object sender, RoutedEventArgs e)
		{
			this._config.ConfigSourcePath = this._config.ConfigSourcePath.Trim();
			this._config.ConfigName = this._config.ConfigName.Trim();
			this._config.NewProfileName = this._config.NewProfileName.Trim();
			if (string.IsNullOrEmpty(this._config.ConfigSourcePath))
			{
				DTMessageBox.Show(Application.Current.MainWindow, DTLocalization.GetString(11114), MessageBoxButton.OK, MessageBoxImage.Hand, null);
				return;
			}
			if (!File.Exists(this._config.ConfigSourcePath))
			{
				DTMessageBox.Show(Application.Current.MainWindow, DTLocalization.GetString(11076), MessageBoxButton.OK, MessageBoxImage.Hand, null);
				return;
			}
			if (!XBValidators.ValidateConfigName(this._config.ConfigName))
			{
				return;
			}
			if (this._config.IsCreateNew)
			{
				if (!XBValidators.ValidateConfigName(this._config.NewProfileName))
				{
					return;
				}
				if (App.GameProfilesService.GamesCollection.FirstOrDefault((GameVM vm) => vm.Name.ToLower().Equals(this._config.NewProfileName.ToLower())) != null)
				{
					DTMessageBox.Show(DTLocalization.GetString(11115), MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
					return;
				}
			}
			this.WindowResult = MessageBoxResult.OK;
			base.Close();
		}

		private ImportConfigData _config;

		private IConfigFileService _configFileService;

		private static bool _isShown;
	}
}
