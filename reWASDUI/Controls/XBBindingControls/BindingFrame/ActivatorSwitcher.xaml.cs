using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using reWASDUI.Infrastructure.KeyBindings.XB;

namespace reWASDUI.Controls.XBBindingControls.BindingFrame
{
	public partial class ActivatorSwitcher : UserControl
	{
		public XBBinding XBBinding
		{
			get
			{
				return (XBBinding)base.GetValue(ActivatorSwitcher.XBBindingProperty);
			}
			set
			{
				base.SetValue(ActivatorSwitcher.XBBindingProperty, value);
			}
		}

		public ActivatorSwitcher()
		{
			this.InitializeComponent();
		}

		public static readonly DependencyProperty XBBindingProperty = DependencyProperty.Register("XBBinding", typeof(XBBinding), typeof(ActivatorSwitcher), new PropertyMetadata(null));
	}
}
