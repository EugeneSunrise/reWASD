using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using reWASDUI.Infrastructure.KeyBindings.ActivatorXB;

namespace reWASDUI.Controls.XBBindingControls.BindingAnnotation
{
	public partial class ActivatorAnnotation : BaseServicesResolvingControl
	{
		public ActivatorXBBinding ActivatorXBBinding
		{
			get
			{
				return (ActivatorXBBinding)base.GetValue(ActivatorAnnotation.ActivatorXBBindingProperty);
			}
			set
			{
				base.SetValue(ActivatorAnnotation.ActivatorXBBindingProperty, value);
			}
		}

		public bool IsShowActivatorIcon
		{
			get
			{
				return (bool)base.GetValue(ActivatorAnnotation.IsShowActivatorIconProperty);
			}
			set
			{
				base.SetValue(ActivatorAnnotation.IsShowActivatorIconProperty, value);
			}
		}

		public bool IsShowActivatorContent
		{
			get
			{
				return (bool)base.GetValue(ActivatorAnnotation.IsShowActivatorContentProperty);
			}
			set
			{
				base.SetValue(ActivatorAnnotation.IsShowActivatorContentProperty, value);
			}
		}

		public bool IsLabelMode
		{
			get
			{
				return (bool)base.GetValue(ActivatorAnnotation.IsLabelModeProperty);
			}
			set
			{
				base.SetValue(ActivatorAnnotation.IsLabelModeProperty, value);
			}
		}

		public ActivatorAnnotation()
		{
			this.InitializeComponent();
		}

		private void SetCurrentActivatorButton_Click(object sender, RoutedEventArgs e)
		{
			if (this.ActivatorXBBinding == null)
			{
				return;
			}
			this.ActivatorXBBinding.HostDictionary.HostXBBinding.CurrentActivatorXBBinding = this.ActivatorXBBinding;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				((Button)target).Click += this.SetCurrentActivatorButton_Click;
			}
		}

		public static readonly DependencyProperty ActivatorXBBindingProperty = DependencyProperty.Register("ActivatorXBBinding", typeof(ActivatorXBBinding), typeof(ActivatorAnnotation), new PropertyMetadata(null));

		public static readonly DependencyProperty IsShowActivatorIconProperty = DependencyProperty.Register("IsShowActivatorIcon", typeof(bool), typeof(ActivatorAnnotation), new PropertyMetadata(true));

		public static readonly DependencyProperty IsShowActivatorContentProperty = DependencyProperty.Register("IsShowActivatorContent", typeof(bool), typeof(ActivatorAnnotation), new PropertyMetadata(true));

		public static readonly DependencyProperty IsLabelModeProperty = DependencyProperty.Register("IsLabelMode", typeof(bool), typeof(ActivatorAnnotation), new PropertyMetadata(false));
	}
}
