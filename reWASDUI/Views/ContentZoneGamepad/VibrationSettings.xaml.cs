using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.Controls.CheckBoxes;
using reWASDUI.Controls;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.ViewModels;

namespace reWASDUI.Views.ContentZoneGamepad
{
	public partial class VibrationSettings : UserControl
	{
		public VibrationSettings()
		{
			this.InitializeComponent();
			base.IsVisibleChanged += this.VibrationSettings_IsVisibleChanged;
		}

		private void VibrationSettings_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
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
				if (mainContentVM2 != null)
				{
					mainContentVM2.StartKeyPoller();
				}
				MainContentVM mainContentVM3 = this._mainContentVM;
				if (mainContentVM3 == null)
				{
					return;
				}
				BaseXBBindingCollection realCurrentBeingMappedBindingCollection = mainContentVM3.GameProfilesService.RealCurrentBeingMappedBindingCollection;
				if (realCurrentBeingMappedBindingCollection == null)
				{
					return;
				}
				realCurrentBeingMappedBindingCollection.SetCurrentButtonMapping(null);
				return;
			}
		}

		private MainContentVM _mainContentVM;
	}
}
