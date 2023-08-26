using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils.AttachedBehaviours;
using DiscSoft.NET.Common.View.Controls.Buttons;
using DiscSoft.NET.Common.View.Controls.CheckBoxes;
using DiscSoft.NET.Common.View.Controls.ComboBoxes;
using DiscSoft.NET.Common.View.Controls.MultiRangeSlider;

namespace reWASDUI.Views.Preferences
{
	public partial class PreferencesOverlayMappings : UserControl
	{
		public PreferencesOverlayMappings()
		{
			this.InitializeComponent();
			base.Loaded += this.PreferencesOverlayMappings_Loaded;
		}

		private void PreferencesOverlayMappings_Loaded(object sender, RoutedEventArgs e)
		{
			ToolTipHelper.SetDisabledToolTip(this.btnRemoveDevice, DTLocalization.GetString(11439));
			ToolTipHelper.SetDisabledToolTip(this.btnRemoveDeviceDescription, DTLocalization.GetString(11439));
		}
	}
}
