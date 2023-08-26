using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.Controls;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.SecondaryWindows.Base;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using reWASDCommon.Network.HTTP.DataTransferObjects;
using reWASDUI.DataModels;
using reWASDUI.Services.Interfaces;
using reWASDUI.Utils;

namespace reWASDUI.Views.SecondaryWindows
{
	public partial class ExportConfig : BaseSecondaryWindow
	{
		public ExportConfig()
		{
			this.InitializeComponent();
		}

		public void PrepareDialog(string configPath, GameVM currentGame, IConfigFileService _configService)
		{
			this._config = new ExportConfigData(configPath, currentGame);
			base.DataContext = this._config;
			this._configFileService = _configService;
			this.WindowResult = MessageBoxResult.Cancel;
		}

		public ExportConfigData Data
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
				return ExportConfig._isShown;
			}
		}

		public new void ShowDialog()
		{
			ExportConfig._isShown = true;
			base.ShowDialog();
			ExportConfig._isShown = false;
		}

		private void ButtonPath_BrowseClick(object sender, RoutedEventArgs e)
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				this._config.Path = folderBrowserDialog.SelectedPath;
			}
		}

		public async Task<string> CopyDirectory(string sourceDir, string destinationDir, bool recursive)
		{
			CopyDirectoryParams copyDirectoryParams = new CopyDirectoryParams(sourceDir, destinationDir, recursive);
			copyDirectoryParams = await App.HttpClientService.GameProfiles.CopyDirectory(copyDirectoryParams);
			string text;
			if (!string.IsNullOrEmpty(copyDirectoryParams.ErrorText))
			{
				DTMessageBox.Show(copyDirectoryParams.ErrorText, MessageBoxButton.OK, MessageBoxImage.Hand, null, false, MessageBoxResult.None);
				text = null;
			}
			else
			{
				text = copyDirectoryParams.DestinationDir;
			}
			return text;
		}

		protected override void OkButton_Click(object sender, RoutedEventArgs e)
		{
			this._config.GameName = this._config.GameName.Trim();
			if (!XBValidators.ValidateConfigName(this._config.GameName))
			{
				return;
			}
			this.CopyDirectory(this._config.GamePath, this._config.Path + "\\" + this._config.GameName + " - Copy", true);
			this.WindowResult = MessageBoxResult.OK;
			base.Close();
		}

		private ExportConfigData _config;

		private IConfigFileService _configFileService;

		private static bool _isShown;
	}
}
