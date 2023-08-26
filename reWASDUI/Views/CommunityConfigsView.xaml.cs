using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View.Controls;
using DiscSoft.NET.Common.View.Controls.Buttons;
using Prism.Ioc;
using reWASDUI.Utils.XBUtil;
using reWASDUI.ViewModels;
using reWASDUI.Views.ContentZoneGamepad.GamepadSelector;

namespace reWASDUI.Views
{
	public partial class CommunityConfigsView : UserControl
	{
		public GamepadSelectorVM GamepadSelectorVM
		{
			get
			{
				return IContainerProviderExtensions.Resolve<GamepadSelectorVM>(App.Container);
			}
		}

		private CommunityConfigsViewVM _dataContext
		{
			get
			{
				return base.DataContext as CommunityConfigsViewVM;
			}
		}

		public CommunityConfigsView()
		{
			base.DataContext = new CommunityConfigsViewVM();
			this.InitializeComponent();
			base.IsVisibleChanged += this.CommunityConfigsView_IsVisibleChanged;
			this._dataContext.PropertyChanged += this.CommunityConfigsViewVM_PropertyChanged;
		}

		private void CommunityConfigsViewVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "ConfigsList")
			{
				try
				{
					ScrollViewer scrollViewer = VisualTreeHelper.GetChild(this.configsList, 0) as ScrollViewer;
					if (scrollViewer != null)
					{
						scrollViewer.ScrollToTop();
					}
				}
				catch (ArgumentOutOfRangeException)
				{
				}
			}
		}

		private void CommunityConfigsView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this._dataContext.CommunityIsVisible = (bool)e.NewValue;
			if ((bool)e.NewValue)
			{
				XBUtils.BlockNavigateContent = true;
				IContainerProviderExtensions.Resolve<GamesSelectorVM>(App.Container).HideGamesListCommand.Execute(null);
				this._dataContext.GetTopGamesList();
				return;
			}
			this._dataContext.ResetAllViews();
			XBUtils.BlockNavigateContent = false;
		}

		private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			GamesSelectorVM gamesSelectorVM = this.Header.gamesSelector.DataContext as GamesSelectorVM;
			if (gamesSelectorVM != null && gamesSelectorVM.IsGameListShown)
			{
				if (e.OriginalSource is Run)
				{
					gamesSelectorVM.IsGameListShown = false;
					return;
				}
				if (VisualTreeHelperExt.FindParent<GamesSelector>((DependencyObject)e.OriginalSource, null) == null)
				{
					gamesSelectorVM.IsGameListShown = false;
				}
			}
		}

		private void GameNameTextBox_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				(base.DataContext as CommunityConfigsViewVM).SearchGamesCommand.Execute();
			}
		}
	}
}
