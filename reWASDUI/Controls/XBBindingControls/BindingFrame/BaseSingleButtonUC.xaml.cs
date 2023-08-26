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
	public partial class BaseSingleButtonUC : UserControl
	{
		public XBBinding XBBinding
		{
			get
			{
				return (XBBinding)base.GetValue(BaseSingleButtonUC.XBBindingProperty);
			}
			set
			{
				base.SetValue(BaseSingleButtonUC.XBBindingProperty, value);
			}
		}

		public Visibility ActivatorSwitcherVisibility
		{
			get
			{
				return (Visibility)base.GetValue(BaseSingleButtonUC.ActivatorSwitcherVisibilityProperty);
			}
			set
			{
				base.SetValue(BaseSingleButtonUC.ActivatorSwitcherVisibilityProperty, value);
			}
		}

		public Visibility ShowFullXBBinding
		{
			get
			{
				return (Visibility)base.GetValue(BaseSingleButtonUC.ShowFullXBBindingProperty);
			}
			set
			{
				base.SetValue(BaseSingleButtonUC.ShowFullXBBindingProperty, value);
			}
		}

		public BaseSingleButtonUC()
		{
			this.InitializeComponent();
		}

		public static readonly DependencyProperty XBBindingProperty = DependencyProperty.Register("XBBinding", typeof(XBBinding), typeof(BaseSingleButtonUC), new PropertyMetadata(null));

		public static readonly DependencyProperty ActivatorSwitcherVisibilityProperty = DependencyProperty.Register("ActivatorSwitcherVisibility", typeof(Visibility), typeof(BaseSingleButtonUC), new PropertyMetadata(Visibility.Visible));

		public static readonly DependencyProperty ShowFullXBBindingProperty = DependencyProperty.Register("ShowFullXBBinding", typeof(Visibility), typeof(BaseSingleButtonUC), new PropertyMetadata(Visibility.Visible));
	}
}
