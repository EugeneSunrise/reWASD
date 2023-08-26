using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Utils.AttachedBehaviours;
using DiscSoft.NET.Common.View.Controls.ComboBoxes;
using reWASDCommon.Infrastructure.Enums;

namespace reWASDUI.Controls
{
	public partial class PlayerLEDControl : UserControl
	{
		public PlayerLEDControl()
		{
			this.InitializeComponent();
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			ToolTipHelper.SetDisabledToolTip(base.Template.FindName("btnRemoveDevice", this) as Button, DTLocalization.GetString(11439));
		}

		private void cmbPlayerLEDMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ColoredComboBox coloredComboBox = base.Template.FindName("cmbPlayerLEDMode", this) as ColoredComboBox;
			StackPanel stackPanel = base.Template.FindName("spLEDContent", this) as StackPanel;
			if (coloredComboBox == null || stackPanel == null)
			{
				return;
			}
			try
			{
				stackPanel.Visibility = (((PlayerLEDMode)coloredComboBox.SelectedItem == null) ? Visibility.Visible : Visibility.Collapsed);
			}
			catch (Exception)
			{
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				((ColoredComboBox)target).SelectionChanged += this.cmbPlayerLEDMode_SelectionChanged;
			}
		}
	}
}
