using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View.Controls.Buttons;
using reWASDUI.Controls;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.KeyBindings;

namespace reWASDUI.Views
{
	public partial class GameConfigSelector : UserControl
	{
		public GameConfigSelector()
		{
			this.InitializeComponent();
			App.EventAggregator.GetEvent<VirtualGamepadTurnedOn>().Subscribe(async delegate(object o)
			{
				if (App.GameProfilesService.RealCurrentBeingMappedBindingCollection != null)
				{
					int shiftIndex = App.GameProfilesService.RealCurrentBeingMappedBindingCollection.ShiftIndex;
				}
				await Task.Delay(200);
				IconAnimation iconAnimation = this.iconAnimation;
				BaseXBBindingCollection realCurrentBeingMappedBindingCollection = App.GameProfilesService.RealCurrentBeingMappedBindingCollection;
				iconAnimation.AnimationBrush = ((realCurrentBeingMappedBindingCollection != null) ? realCurrentBeingMappedBindingCollection.CollectionBrush : null);
				this.iconAnimation.Visibility = Visibility.Visible;
				this.iconAnimation.PlayAnimation();
			});
		}

		private void ConfigsList_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
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

		private void ConfigsList_OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}
	}
}
