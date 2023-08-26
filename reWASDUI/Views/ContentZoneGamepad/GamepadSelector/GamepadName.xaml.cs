using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.Controls;
using DiscSoft.NET.Common.View.Controls.Buttons;
using Prism.Ioc;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.ViewModels;

namespace reWASDUI.Views.ContentZoneGamepad.GamepadSelector
{
	public partial class GamepadName : UserControl
	{
		private BaseControllerVM _datacontext
		{
			get
			{
				return (BaseControllerVM)base.DataContext;
			}
		}

		public GamepadSelectorVM GamepadSelectorVM
		{
			get
			{
				return IContainerProviderExtensions.Resolve<GamepadSelectorVM>(App.Container);
			}
		}

		public GamepadName()
		{
			this.InitializeComponent();
			this.tbControllerName.IsVisibleChanged += this.TbControllerNameOnIsVisibleChanged;
		}

		private void TbControllerNameOnIsVisibleChanged(object s, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue is bool && (bool)e.NewValue)
			{
				this.tbControllerName.Focus();
				this.tbControllerName.CaretIndex = this.tbControllerName.Text.Length;
				this.tbControllerName.SelectAll();
			}
		}

		private void tbControllerNameLostFocus(object sender, RoutedEventArgs e)
		{
			FrameworkElement frameworkElement = Keyboard.FocusedElement as FrameworkElement;
			if (((frameworkElement != null) ? frameworkElement.Name : null) != "imgbtnEdit")
			{
				this.GamepadSelectorVM.TBControllerNameApplyChanges();
			}
		}

		private void TbControllerName_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				this.GamepadSelectorVM.TBControllerNameApplyChanges();
			}
			if (e.Key == Key.Escape)
			{
				this.GamepadSelectorVM.TBControllerNameRevertChanges();
			}
		}

		private void BtnOptions_OnClick(object sender, RoutedEventArgs e)
		{
			ContextMenu contextMenu = base.TryFindResource("ctxMenuGamepad") as ContextMenu;
			contextMenu.PlacementTarget = sender as UIElement;
			contextMenu.SetValue(AutomationProperties.AutomationIdProperty, "cmOptionsMenu");
			contextMenu.IsOpen = true;
		}
	}
}
