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
using DiscSoft.NET.Common.View.Controls.ComboBoxes;

namespace reWASDUI.Views.Preferences
{
	public partial class InputDevicesControl : UserControl
	{
		public InputDevicesControl()
		{
			this.InitializeComponent();
			base.Loaded += this.InputDevicesControl_Loaded;
		}

		private void InputDevicesControl_Loaded(object sender, RoutedEventArgs e)
		{
			ToolTipHelper.SetDisabledToolTip(this.btnRemoveDevice, DTLocalization.GetString(11439));
		}
	}
}
