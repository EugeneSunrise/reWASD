using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using DiscSoft.NET.Common.View.Controls.Buttons;
using Prism.Ioc;
using reWASDUI.ViewModels;

namespace reWASDUI.Views.ContentZoneGamepad.GamepadSelector
{
	public partial class GamesSelectorStartScreen : UserControl
	{
		public GamesSelectorStartScreen()
		{
			base.DataContext = IContainerProviderExtensions.Resolve<GamesSelectorVM>(App.Container);
			this.InitializeComponent();
		}

		private void List_OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		private void ListBoxItem_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			ListBoxItem listBoxItem = sender as ListBoxItem;
			if (listBoxItem != null && listBoxItem != this._senderPrev)
			{
				if (this._senderPrev != null)
				{
					this._senderPrev.Background = new SolidColorBrush(Colors.Transparent);
				}
				this._senderPrev = listBoxItem;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 5)
			{
				EventSetter eventSetter = new EventSetter();
				eventSetter.Event = FrameworkElement.ContextMenuOpeningEvent;
				eventSetter.Handler = new ContextMenuEventHandler(this.ListBoxItem_ContextMenuOpening);
				((Style)target).Setters.Add(eventSetter);
			}
		}

		private ListBoxItem _senderPrev;
	}
}
