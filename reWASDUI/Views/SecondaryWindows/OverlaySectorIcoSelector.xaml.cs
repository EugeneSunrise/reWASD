using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.SecondaryWindows.Base;
using reWASDUI.Infrastructure;

namespace reWASDUI.Views.SecondaryWindows
{
	public partial class OverlaySectorIcoSelector : BaseSecondaryWindow
	{
		public OverlaySectorIcoSelector()
		{
			this._dataContext = new OverlaySectorIcoSelectorData();
			base.DataContext = this._dataContext;
			this.InitializeComponent();
			base.Title = "reWASD";
		}

		private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ListBox listBox = sender as ListBox;
			if (listBox != null && listBox.SelectedItem != null)
			{
				RadialMenuIcon radialMenuIcon = listBox.SelectedItem as RadialMenuIcon;
				if (radialMenuIcon != null)
				{
					App.GameProfilesService.CurrentGame.CurrentConfig.ConfigData.OverlayMenu.Circle.MainOrSubmenu.CurrentSector.SelectedIcon = radialMenuIcon;
				}
			}
			e.Handled = true;
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			base.Close();
		}

		private void ListBox_SelectionChangedForCategories(object sender, SelectionChangedEventArgs e)
		{
			ListBox listBox = sender as ListBox;
			if (listBox != null && listBox.SelectedIndex != -1)
			{
				listBox.SelectedIndex = -1;
			}
			e.Handled = true;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 3)
			{
				((ListBox)target).SelectionChanged += this.ListBox_SelectionChanged;
			}
		}

		private OverlaySectorIcoSelectorData _dataContext;
	}
}
