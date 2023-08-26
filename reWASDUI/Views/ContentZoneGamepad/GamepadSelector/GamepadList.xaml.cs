using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using DiscSoft.NET.Common.Utils;
using reWASDUI.DataModels;
using reWASDUI.Infrastructure.Controller;

namespace reWASDUI.Views.ContentZoneGamepad.GamepadSelector
{
	public partial class GamepadList : UserControl
	{
		public bool IsBigIcons
		{
			get
			{
				return (bool)base.GetValue(GamepadList.IsIsBigIconsProperty);
			}
			set
			{
				base.SetValue(GamepadList.IsIsBigIconsProperty, value);
			}
		}

		public GamepadList()
		{
			this.InitializeComponent();
		}

		private async void ProfilesList_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			try
			{
				if (e.ChangedButton == MouseButton.Right)
				{
					e.Handled = true;
				}
				else
				{
					bool flag = false;
					GameVM currentGame = App.GameProfilesService.CurrentGame;
					bool? flag2;
					if (currentGame == null)
					{
						flag2 = null;
					}
					else
					{
						ConfigVM currentConfig = currentGame.CurrentConfig;
						flag2 = ((currentConfig != null) ? new bool?(currentConfig.IsChangedIncludingShiftCollections) : null);
					}
					bool? flag3 = flag2;
					if (flag3.GetValueOrDefault())
					{
						await App.GameProfilesService.TryAskUserToSaveChanges(false);
						flag = true;
					}
					ListBoxItem listBoxItem = VisualTreeHelperExt.FindParent<ListBoxItem>((DependencyObject)e.OriginalSource, null);
					BaseControllerVM baseControllerVM = ((listBoxItem != null) ? listBoxItem.DataContext : null) as BaseControllerVM;
					if (baseControllerVM != null)
					{
						baseControllerVM.VibrateCommand.Execute();
					}
					if (flag)
					{
						await App.GamepadService.SetCurrentGamepad(baseControllerVM);
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private void gamepadsList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			ScrollViewer scrollViewer = VisualTreeHelperExt.FindChildren<ScrollViewer>(sender as ListBox).FirstOrDefault<ScrollViewer>();
			if (e.Delta > 0)
			{
				if (scrollViewer != null)
				{
					scrollViewer.LineLeft();
				}
			}
			else if (scrollViewer != null)
			{
				scrollViewer.LineRight();
			}
			e.Handled = true;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				((ListBox)target).PreviewMouseDown += this.ProfilesList_OnPreviewMouseDown;
				((ListBox)target).PreviewMouseWheel += this.gamepadsList_PreviewMouseWheel;
			}
		}

		public static readonly DependencyProperty IsIsBigIconsProperty = DependencyProperty.Register("IsBigIcons", typeof(bool), typeof(GamepadList), new PropertyMetadata(false));
	}
}
