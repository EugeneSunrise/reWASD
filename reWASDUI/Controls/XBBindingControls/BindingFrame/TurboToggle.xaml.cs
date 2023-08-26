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
	public partial class TurboToggle : UserControl
	{
		public XBBinding XBBinding
		{
			get
			{
				return (XBBinding)base.GetValue(TurboToggle.XBBindingProperty);
			}
			set
			{
				base.SetValue(TurboToggle.XBBindingProperty, value);
			}
		}

		public bool TurboIsVisible
		{
			get
			{
				return (bool)base.GetValue(TurboToggle.TurboIsVisibleProperty);
			}
			set
			{
				base.SetValue(TurboToggle.TurboIsVisibleProperty, value);
			}
		}

		public bool ToggleIsVisible
		{
			get
			{
				return (bool)base.GetValue(TurboToggle.ToggleIsVisibleProperty);
			}
			set
			{
				base.SetValue(TurboToggle.ToggleIsVisibleProperty, value);
			}
		}

		public TurboToggle()
		{
			this.InitializeComponent();
		}

		public static readonly DependencyProperty XBBindingProperty = DependencyProperty.Register("XBBinding", typeof(XBBinding), typeof(TurboToggle), new PropertyMetadata(null));

		public static readonly DependencyProperty TurboIsVisibleProperty = DependencyProperty.Register("TurboIsVisible", typeof(bool), typeof(TurboToggle), new PropertyMetadata(true));

		public static readonly DependencyProperty ToggleIsVisibleProperty = DependencyProperty.Register("ToggleIsVisible", typeof(bool), typeof(TurboToggle), new PropertyMetadata(true));
	}
}
