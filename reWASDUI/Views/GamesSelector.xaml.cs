using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace reWASDUI.Views
{
	public partial class GamesSelector : UserControl
	{
		public UIElement GameNameContent
		{
			get
			{
				return (UIElement)base.GetValue(GamesSelector.GameNameContentProperty);
			}
			set
			{
				base.SetValue(GamesSelector.GameNameContentProperty, value);
			}
		}

		public GamesSelector()
		{
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
			if (connectionId == 1)
			{
				((ListBox)target).PreviewMouseRightButtonDown += this.List_OnPreviewMouseRightButtonDown;
				return;
			}
			if (connectionId != 2)
			{
				return;
			}
			EventSetter eventSetter = new EventSetter();
			eventSetter.Event = FrameworkElement.ContextMenuOpeningEvent;
			eventSetter.Handler = new ContextMenuEventHandler(this.ListBoxItem_ContextMenuOpening);
			((Style)target).Setters.Add(eventSetter);
		}

		public static readonly DependencyProperty GameNameContentProperty = DependencyProperty.Register("GameNameContent", typeof(UIElement), typeof(GamesSelector), new PropertyMetadata(null));

		private ListBoxItem _senderPrev;
	}
}
