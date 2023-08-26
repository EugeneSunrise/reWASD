using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.ViewModels;

namespace reWASDUI.Controls
{
	public partial class ActiveGamepadProfileUC : UserControl
	{
		public Slot Slot
		{
			get
			{
				return (Slot)base.GetValue(ActiveGamepadProfileUC.SlotProperty);
			}
			set
			{
				base.SetValue(ActiveGamepadProfileUC.SlotProperty, value);
			}
		}

		private GamepadSelectorVM _datacontext
		{
			get
			{
				return base.DataContext as GamepadSelectorVM;
			}
		}

		public ActiveGamepadProfileUC()
		{
			this.InitializeComponent();
		}

		public static readonly DependencyProperty SlotProperty = DependencyProperty.Register("Slot", typeof(Slot), typeof(ActiveGamepadProfileUC), new PropertyMetadata(0));
	}
}
