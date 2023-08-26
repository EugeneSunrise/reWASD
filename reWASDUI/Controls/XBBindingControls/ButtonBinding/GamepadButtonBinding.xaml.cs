using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using reWASDUI.Infrastructure.KeyBindings.XB;

namespace reWASDUI.Controls.XBBindingControls.ButtonBinding
{
	public partial class GamepadButtonBinding : BaseButtonBinding
	{
		public Visibility TitleVisibility
		{
			get
			{
				return (Visibility)base.GetValue(GamepadButtonBinding.TitleVisibilityProperty);
			}
			set
			{
				base.SetValue(GamepadButtonBinding.TitleVisibilityProperty, value);
			}
		}

		protected override void OnXBBindingChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnXBBindingChanged(e);
			XBBinding xbbinding = base.XBBinding;
			if (((xbbinding != null) ? xbbinding.CurrentActivatorXBBinding : null) != null)
			{
				base.XBBinding.CurrentActivatorXBBinding.RefreshAnnotations();
			}
			if (base.XBBinding != null)
			{
				base.GamepadButtonToBind = base.XBBinding.GamepadButton;
			}
		}

		public GamepadButtonBinding()
		{
			this.InitializeComponent();
		}

		protected override void ReEvaluateXBBinding()
		{
		}

		public static readonly DependencyProperty TitleVisibilityProperty = DependencyProperty.Register("TitleVisibility", typeof(Visibility), typeof(GamepadButtonBinding), new PropertyMetadata(Visibility.Visible));
	}
}
