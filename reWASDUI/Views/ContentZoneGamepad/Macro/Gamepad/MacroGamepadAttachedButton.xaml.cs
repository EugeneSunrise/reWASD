using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DiscSoft.NET.Common.View;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Utils.XBUtil;

namespace reWASDUI.Views.ContentZoneGamepad.Macro.Gamepad
{
	public partial class MacroGamepadAttachedButton : SVGElementAttachedButton
	{
		public GamepadButton? GamepadButton
		{
			get
			{
				return (GamepadButton?)base.GetValue(MacroGamepadAttachedButton.GamepadButtonProperty);
			}
			set
			{
				base.SetValue(MacroGamepadAttachedButton.GamepadButtonProperty, value);
			}
		}

		private static void GamepadButtonChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MacroGamepadAttachedButton macroGamepadAttachedButton = d as MacroGamepadAttachedButton;
			if (macroGamepadAttachedButton == null)
			{
				return;
			}
			macroGamepadAttachedButton.OnGamepadButtonChanged();
		}

		public MacroGamepadAttachedButton()
		{
			base.IgnoreIsCurrentBinding = true;
			this.InitializeComponent();
			base.Loaded += this.OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			if (this.GamepadButton != null && string.IsNullOrEmpty(base.SVGElementName))
			{
				base.SVGElementName = XBUtils.ConvertGamepadButtonToAnchorString(this.GamepadButton.Value);
			}
		}

		private void OnGamepadButtonChanged()
		{
			if (this.GamepadButton == null)
			{
				return;
			}
			base.CommandParameter = this.GamepadButton;
			base.IgnoreIsCurrentBinding = true;
		}

		public static readonly DependencyProperty GamepadButtonProperty = DependencyProperty.Register("GamepadButton", typeof(GamepadButton?), typeof(MacroGamepadAttachedButton), new PropertyMetadata(null, new PropertyChangedCallback(MacroGamepadAttachedButton.GamepadButtonChangedCallback)));
	}
}
