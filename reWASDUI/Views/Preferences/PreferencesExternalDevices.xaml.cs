using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.Controls;
using DiscSoft.NET.Common.View.Controls.Buttons;
using reWASDUI.ViewModels.Preferences;

namespace reWASDUI.Views.Preferences
{
	public partial class PreferencesExternalDevices : UserControl
	{
		public PreferencesExternalDevices()
		{
			this.InitializeComponent();
		}

		private void DeviceTextBlock_OnEditModeChanged(object sender, EventArgs e)
		{
			EditableTextBlock editableTextBlock = sender as EditableTextBlock;
			if (editableTextBlock != null)
			{
				(base.DataContext as PreferencesExternalDevicesVM).OnDeviceEditModeChanged(editableTextBlock.Text);
			}
		}

		private void ClientTextBlock_OnEditModeChanged(object sender, EventArgs e)
		{
			EditableTextBlock editableTextBlock = sender as EditableTextBlock;
			if (editableTextBlock != null)
			{
				(base.DataContext as PreferencesExternalDevicesVM).OnClientEditModeChanged(editableTextBlock.Text);
			}
		}

		private void DeviceTextBlock_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			(base.DataContext as PreferencesExternalDevicesVM).EditExternalDeviceCommand.Execute((base.DataContext as PreferencesExternalDevicesVM).SelectedExternalDevice);
		}

		private void ClientTextBlock_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			(base.DataContext as PreferencesExternalDevicesVM).EditExternalClientCommand.Execute((base.DataContext as PreferencesExternalDevicesVM).SelectedExternalClient);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 3)
			{
				((EditableTextBlock)target).MouseDoubleClick += this.DeviceTextBlock_MouseDoubleClick;
				((EditableTextBlock)target).OnEditModeChanged += this.DeviceTextBlock_OnEditModeChanged;
				return;
			}
			if (connectionId != 11)
			{
				return;
			}
			((EditableTextBlock)target).MouseDoubleClick += this.ClientTextBlock_MouseDoubleClick;
			((EditableTextBlock)target).OnEditModeChanged += this.ClientTextBlock_OnEditModeChanged;
		}
	}
}
