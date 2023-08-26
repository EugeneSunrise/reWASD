using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.Controls.ComboBoxes;
using reWASDUI.Controls;
using reWASDUI.Controls.XBBindingControls.BindingFrame;
using reWASDUI.DataModels;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.ControllerBindings;
using reWASDUI.ViewModels;

namespace reWASDUI.Views
{
	public partial class KeyboardMappingView : BaseServicesDataContextControl
	{
		public KeyboardMappingView()
		{
			this.InitializeComponent();
			((INotifyCollectionChanged)this.keyMappingsItemsControl.Items).CollectionChanged += this.ListBox_CollectionChanged;
			base.Unloaded += this.OnUnloaded;
			base.IsVisibleChanged += this.KeyboardMappingViews_IsVisibleChanged;
		}

		private void KeyboardMappingViews_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue)
			{
				this._mainContentVM = App.MainContentVM;
				MainContentVM mainContentVM = this._mainContentVM;
				if (mainContentVM == null)
				{
					return;
				}
				mainContentVM.SuspendKeyPoller();
				return;
			}
			else
			{
				MainContentVM mainContentVM2 = this._mainContentVM;
				if (mainContentVM2 == null)
				{
					return;
				}
				mainContentVM2.StartKeyPoller();
				return;
			}
		}

		private void ListBox_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				this.keyMappingsItemsControl.ScrollIntoView(e.NewItems[0]);
			}
		}

		private void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			List<Type> list = new List<Type>
			{
				typeof(ComboBox),
				typeof(Popup),
				typeof(BindingFrameUC)
			};
			if (VisualTreeHelperExt.FindParent((DependencyObject)e.OriginalSource, list, null) != null)
			{
				return;
			}
			BaseXBBindingCollection realCurrentBeingMappedBindingCollection = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
			bool flag;
			if (realCurrentBeingMappedBindingCollection == null)
			{
				flag = null != null;
			}
			else
			{
				ControllerBindingsCollection controllerBindings = realCurrentBeingMappedBindingCollection.ControllerBindings;
				flag = ((controllerBindings != null) ? controllerBindings.CurrentEditItem : null) != null;
			}
			if (flag)
			{
				base.GameProfilesService.RealCurrentBeingMappedBindingCollection.ControllerBindings.CurrentEditItem = null;
			}
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			GameVM currentGame = base.GameProfilesService.CurrentGame;
			if (currentGame != null)
			{
				ConfigVM currentConfig = currentGame.CurrentConfig;
			}
		}

		private MainContentVM _mainContentVM;
	}
}
