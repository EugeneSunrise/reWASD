using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.View.Controls.ComboBoxes;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.ViewModels.Preferences;

namespace reWASDUI.Views.Preferences
{
	public partial class PreferencesLEDSettings : UserControl
	{
		public PreferencesLEDSettings()
		{
			this.InitializeComponent();
			base.IsVisibleChanged += delegate(object sender, DependencyPropertyChangedEventArgs e)
			{
				if ((bool)e.NewValue)
				{
					SelectionChangedEventArgs selectionChangedEventArgs = new SelectionChangedEventArgs(Selector.SelectionChangedEvent, new List<LEDSupportedDevice>(), new List<LEDSupportedDevice> { ((PreferencesLEDSettingsVM)base.DataContext).SelectedLedDevice });
					this.cmbLEDDeviceToWorkWith_SelectionChanged(sender, selectionChangedEventArgs);
				}
			};
		}

		private void cmbLEDDeviceToWorkWith_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (base.DataContext == null)
			{
				return;
			}
			((PreferencesLEDSettingsVM)base.DataContext).Description = new Localizable(11901);
			if (e.AddedItems.Count > 0)
			{
				LEDSupportedDevice ledsupportedDevice = (LEDSupportedDevice)e.AddedItems[0];
				if (ledsupportedDevice == 12)
				{
					Localizable localizable = new Localizable();
					localizable.Value = new Localizable(11901).Value + "\n" + new Localizable(12386).Value;
					((PreferencesLEDSettingsVM)base.DataContext).Description = localizable;
					return;
				}
				if (ledsupportedDevice == 13)
				{
					((PreferencesLEDSettingsVM)base.DataContext).Description = new Localizable(12417);
				}
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 2)
			{
				((ColoredComboBox)target).SelectionChanged += this.cmbLEDDeviceToWorkWith_SelectionChanged;
			}
		}
	}
}
