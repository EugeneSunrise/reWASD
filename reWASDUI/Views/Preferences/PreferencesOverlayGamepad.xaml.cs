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
	public partial class PreferencesOverlayGamepad : UserControl
	{
		public PreferencesOverlayGamepad()
		{
			this.InitializeComponent();
			base.Loaded += this.PreferencesOverlayGamepad_Loaded;
		}

		private void PreferencesOverlayGamepad_Loaded(object sender, RoutedEventArgs e)
		{
			ToolTipHelper.SetDisabledToolTip(this.btnRemoveDevice, DTLocalization.GetString(11439));
		}
	}
}
