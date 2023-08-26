using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.View.Controls;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.SecondaryWindows.Base;
using Microsoft.Win32;
using reWASDUI.DataModels.HelperModels;
using reWASDUI.Utils;
using XBEliteWPF.Utils;

namespace reWASDUI.Views.SecondaryWindows
{
	public partial class AddEditGame : BaseSecondaryWindow
	{
		private GameDataVM _gameData
		{
			get
			{
				return (GameDataVM)base.DataContext;
			}
		}

		public bool ShowOnlyApps { get; set; }

		public static bool ShowDlgEditAppsInGame(string sName, ref ObservableCollection<string> applicationNamesCollection, int configCount = -1)
		{
			GameDataVM gameDataVM = new GameDataVM();
			gameDataVM.IsGameHasMoreThanOneConfig = configCount > 1;
			gameDataVM.IsGameHasAnyConfig = configCount > 0;
			gameDataVM.ApplicationNamesCollection = ((applicationNamesCollection == null) ? new ObservableCollection<string>() : new ObservableCollection<string>(applicationNamesCollection));
			string text = ((applicationNamesCollection != null) ? string.Join(";", applicationNamesCollection.ToArray<string>()) : "");
			gameDataVM.SetOldValues();
			AddEditGame addEditGame = new AddEditGame(gameDataVM, true);
			addEditGame.ButtonOk.Content = DTLocalization.GetString(9219);
			addEditGame.ShowDialog();
			if (addEditGame.WindowResult == MessageBoxResult.OK || addEditGame.WindowResult == MessageBoxResult.Yes)
			{
				string text2 = string.Join(";", gameDataVM.ApplicationNamesCollection.ToArray<string>());
				if (text2 != text)
				{
					SenderGoogleAnalytics.SendMessageEvent("Autodetect", sName, text2, -1L, false);
				}
				applicationNamesCollection = gameDataVM.ApplicationNamesCollection;
				return true;
			}
			return false;
		}

		public static bool ShowDlgAddEditGame(ref string sName, ref string sImageSourcePath, ref ObservableCollection<string> applicationNamesCollection, int configCount = -1)
		{
			GameDataVM gameDataVM = new GameDataVM();
			gameDataVM.Name = sName;
			gameDataVM.ImageSourcePath = sImageSourcePath;
			gameDataVM.IsGameHasMoreThanOneConfig = configCount > 1;
			gameDataVM.IsGameHasAnyConfig = configCount > 0;
			gameDataVM.ApplicationNamesCollection = ((applicationNamesCollection == null) ? new ObservableCollection<string>() : new ObservableCollection<string>(applicationNamesCollection));
			string text = ((applicationNamesCollection != null) ? string.Join(";", applicationNamesCollection.ToArray<string>()) : "");
			gameDataVM.SetOldValues();
			AddEditGame addEditGame = new AddEditGame(gameDataVM, false);
			if (sName.Length > 0)
			{
				addEditGame.ButtonOk.Content = DTLocalization.GetString(9219);
			}
			else
			{
				addEditGame.ButtonOk.Content = DTLocalization.GetString(5319);
			}
			addEditGame.ShowDialog();
			gameDataVM.Name = gameDataVM.Name.Trim();
			if (addEditGame.WindowResult == MessageBoxResult.OK || addEditGame.WindowResult == MessageBoxResult.Yes)
			{
				sName = gameDataVM.Name;
				sImageSourcePath = gameDataVM.ImageSourcePath;
				string text2 = string.Join(";", gameDataVM.ApplicationNamesCollection.ToArray<string>());
				if (text2 != text)
				{
					SenderGoogleAnalytics.SendMessageEvent("Autodetect", sName, text2, -1L, false);
				}
				applicationNamesCollection = gameDataVM.ApplicationNamesCollection;
				return true;
			}
			return false;
		}

		public AddEditGame(object dataContext, bool onlyApps = false)
		{
			base.DataContext = dataContext;
			this.ShowOnlyApps = onlyApps;
			this.InitializeComponent();
			base.Loaded += async delegate(object sender, RoutedEventArgs args)
			{
				await Task.Delay(50);
				this.editGameName.Focus();
				Keyboard.Focus(this.editGameName);
				this.editGameName.SelectAll();
			};
			base.Title = "Game details";
			this.BoxArtTip.Text = string.Concat(new string[]
			{
				DTLocalization.GetString(11546),
				" ",
				1280.0.ToString(),
				"x",
				800.0.ToString(),
				"px"
			});
		}

		private void btnBrowseImageClick(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = DTLocalization.GetString(11028) + " (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
			openFileDialog.CheckFileExists = true;
			openFileDialog.Multiselect = false;
			bool? flag = openFileDialog.ShowDialog();
			bool flag2 = true;
			if ((flag.GetValueOrDefault() == flag2) & (flag != null))
			{
				((GameDataVM)base.DataContext).ImageSourcePath = openFileDialog.FileName;
			}
		}

		private void btnBrowseApplicationClick(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Applications | *.exe";
			openFileDialog.CheckFileExists = true;
			openFileDialog.Multiselect = false;
			bool? flag = openFileDialog.ShowDialog();
			bool flag2 = true;
			if ((flag.GetValueOrDefault() == flag2) & (flag != null))
			{
				FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
				((GameDataVM)base.DataContext).AddAutodetectProcess(fileInfo.Name);
			}
		}

		private void btnBrowseProcessClick(object sender, RoutedEventArgs e)
		{
			ProcessListDialog processListDialog = new ProcessListDialog();
			processListDialog.InitialProcessList = this._gameData.ApplicationNamesCollection.ToList<string>();
			processListDialog.ShowDialog();
			if (processListDialog.WindowResult == MessageBoxResult.OK)
			{
				foreach (ProcessListDialog.ProcessItem processItem in processListDialog.ProcessList)
				{
					string fileName = Path.GetFileName(processItem.FilePath);
					if (processItem.IsChecked && fileName != null && !processListDialog.InitialProcessList.Any((string item) => item.ToLower() == fileName.ToLower()))
					{
						this._gameData.AddAutodetectProcess(Path.GetFileName(processItem.FilePath));
					}
					if (!processItem.IsChecked && fileName != null && processListDialog.InitialProcessList.Any((string item) => item.ToLower() == fileName.ToLower()))
					{
						this._gameData.RemoveApplicationName(Path.GetFileName(processItem.FilePath));
					}
				}
			}
		}

		protected override void OkButton_Click(object sender, RoutedEventArgs e)
		{
			if (this.ShowOnlyApps || XBValidators.ValidateGameName(this.editGameName.Text.Trim()))
			{
				this.WindowResult = MessageBoxResult.OK;
				base.Close();
			}
		}
	}
}
