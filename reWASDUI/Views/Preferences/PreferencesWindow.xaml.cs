using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.View.Controls;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.SecondaryWindows.DTMessageBox;
using DiscSoft.NET.Common.View.SecondaryWindows.WaitDialog;
using reWASDUI.Infrastructure;
using reWASDUI.ViewModels.Preferences;

namespace reWASDUI.Views.Preferences
{
	public partial class PreferencesWindow : UserControl
	{
		public static void ShowPreferences(Type selectPage)
		{
			bool flag = PreferencesWindow._currentPreferences == null || !PreferencesWindow._currentPreferences.IsVisible;
			reWASDApplicationCommands.NavigateContentCommand.Execute(typeof(PreferencesWindow));
			if (PreferencesWindow._currentPreferences != null)
			{
				if (flag)
				{
					PreferencesWindow._currentPreferences.Initialize();
				}
				PreferencesWindow._currentPreferences.Refresh();
				if (selectPage != null)
				{
					PreferencesWindow._currentPreferences._viewModel.SelectPage(selectPage);
				}
			}
		}

		public PreferencesWindow()
		{
			this.InitializeComponent();
			PreferencesWindow._currentPreferences = this;
			this._viewModel = (PreferencesWindowVM)base.DataContext;
		}

		public void Initialize()
		{
			this._viewModel.Initialize();
		}

		public void Refresh()
		{
			this._viewModel.Refresh();
		}

		private async void ButtonApply_OnClick(object sender, RoutedEventArgs e)
		{
			WaitDialog.ShowDialogStatic(DTLocalization.GetString(5238), null, null, false, false, null, null);
			await this._viewModel.ApplyChanges();
			App.EventAggregator.GetEvent<CloseAllPopups>().Publish(null);
			WaitDialog.TryCloseWaitDialog();
		}

		private void Close()
		{
			reWASDApplicationCommands.NavigateContentCommand.Execute(typeof(MainContent));
		}

		private async void ButtonClose_OnClick(object sender, RoutedEventArgs e)
		{
			await this.CheckIfChangedAndClose();
		}

		private async Task CheckIfChangedAndClose()
		{
			if (this._viewModel.IsOptionChanged)
			{
				MessageBoxResult messageBoxResult = DTMessageBox.Show(DTLocalization.GetString(11003), MessageBoxButton.YesNoCancel, MessageBoxImage.Question, null, false, MessageBoxResult.None);
				if (messageBoxResult == MessageBoxResult.Cancel)
				{
					return;
				}
				if (messageBoxResult == MessageBoxResult.No)
				{
					this._viewModel.ResetChanges();
				}
				if (messageBoxResult == MessageBoxResult.Yes)
				{
					await this._viewModel.ApplyChanges();
				}
			}
			this.Close();
		}

		private async void WindowKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				await this.CheckIfChangedAndClose();
			}
		}

		private void btnStartLog_OnClick(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void btnStopLog_OnClick(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private PreferencesWindowVM _viewModel;

		private static PreferencesWindow _currentPreferences;
	}
}
