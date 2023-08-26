using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.SecondaryWindows.Base;
using reWASDUI.DataModels.CompositeDevicesCollection;
using reWASDUI.Infrastructure.Controller;
using reWASDUI.Services;
using reWASDUI.Services.Interfaces;
using reWASDUI.ViewModels.SecondaryWindows;

namespace reWASDUI.Views.SecondaryWindows
{
	public partial class CompositeDevicesWindow : BaseSecondaryWindow
	{
		private CompositeDevicesWindowVM _dataContext
		{
			get
			{
				return base.DataContext as CompositeDevicesWindowVM;
			}
		}

		public IGamepadService GamepadService
		{
			get
			{
				return App.GamepadService;
			}
		}

		public CompositeDevicesWindow(GamepadService gamepadService, BaseControllerVM controller)
		{
			this._controller = controller;
			base.DataContext = new CompositeDevicesWindowVM(gamepadService);
			this.InitializeComponent();
			base.ContentRendered += this.OnContentRendered;
			base.Closing += this.OnClosing;
			this._pollingPaused = App.KeyPressedPollerService.SuspendPollingUntilStarted();
		}

		private void OnClosing(object sender, CancelEventArgs e)
		{
			this._dataContext.GamepadService.CompositeDevices.CurrentEditItem = null;
			if (this._pollingPaused)
			{
				App.KeyPressedPollerService.StartPolling(true);
			}
		}

		private void OnContentRendered(object sender, EventArgs e)
		{
			this._dataContext.GamepadService.CompositeDevices.RefreshHelperProperties();
			if (this._controller != null)
			{
				BaseControllerVM <b>5__3 = this._controller;
				CompositeControllerVM <cc>5__2 = <b>5__3 as CompositeControllerVM;
				if (<cc>5__2 != null)
				{
					this._dataContext.GamepadService.CompositeDevices.CurrentEditItem = this._dataContext.GamepadService.CompositeDevices.FirstOrDefault((CompositeDevice cd) => cd.ID == <cc>5__2.ID);
					return;
				}
				if (<b>5__3 == null)
				{
					return;
				}
				CompositeDevice compositeDevice = this._dataContext.GamepadService.CompositeDevices.FirstOrDefault((CompositeDevice cd) => cd.ID.Contains(<b>5__3.ID));
				if (compositeDevice == null)
				{
					this._dataContext.AddGroupCommand.Execute();
					if (this._dataContext.GamepadService.CompositeDevices.CurrentEditItem != null)
					{
						this._dataContext.GamepadService.CompositeDevices.CurrentEditItem.Controller1 = <b>5__3;
						return;
					}
				}
				else
				{
					this._dataContext.GamepadService.CompositeDevices.CurrentEditItem = compositeDevice;
				}
			}
		}

		private void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			List<Type> list = new List<Type>
			{
				typeof(ComboBox),
				typeof(Popup)
			};
			if (VisualTreeHelperExt.FindParent((DependencyObject)e.OriginalSource, list, null) != null)
			{
				return;
			}
			this._dataContext.GamepadService.CompositeDevices.CurrentEditItem = null;
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
		}

		private BaseControllerVM _controller;

		private bool _pollingPaused;
	}
}
