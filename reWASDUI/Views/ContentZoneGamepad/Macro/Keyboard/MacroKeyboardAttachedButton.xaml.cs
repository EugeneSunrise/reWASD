using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using DiscSoft.NET.Common.View;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;

namespace reWASDUI.Views.ContentZoneGamepad.Macro.Keyboard
{
	public partial class MacroKeyboardAttachedButton : SVGElementAttachedButton
	{
		public Key? Key
		{
			get
			{
				return (Key?)base.GetValue(MacroKeyboardAttachedButton.KeyProperty);
			}
			set
			{
				base.SetValue(MacroKeyboardAttachedButton.KeyProperty, value);
			}
		}

		private static void KeyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MacroKeyboardAttachedButton macroKeyboardAttachedButton = d as MacroKeyboardAttachedButton;
			if (macroKeyboardAttachedButton == null)
			{
				return;
			}
			macroKeyboardAttachedButton.OnKeyChanged();
		}

		public KeyScanCodeV2 KeyScanCode
		{
			get
			{
				return (KeyScanCodeV2)base.GetValue(MacroKeyboardAttachedButton.KeyScanCodeProperty);
			}
			set
			{
				base.SetValue(MacroKeyboardAttachedButton.KeyScanCodeProperty, value);
			}
		}

		private static void KeyScanCodeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MacroKeyboardAttachedButton macroKeyboardAttachedButton = d as MacroKeyboardAttachedButton;
			if (macroKeyboardAttachedButton == null)
			{
				return;
			}
			macroKeyboardAttachedButton.OnKeyScanCodeChanged();
		}

		public MacroKeyboardAttachedButton()
		{
			this.InitializeComponent();
		}

		private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
		{
			base.IsCurrentBinding = false;
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
		}

		private void OnKeyChanged()
		{
			if (this.Key == null)
			{
				return;
			}
			base.SVGElementName = this.Key.ToString();
			this.KeyScanCode = KeyScanCodeV2.FindKeyScanCodeByKey(this.Key.Value);
		}

		private void OnKeyScanCodeChanged()
		{
			base.CommandParameter = this.KeyScanCode;
		}

		public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key", typeof(Key?), typeof(MacroKeyboardAttachedButton), new PropertyMetadata(null, new PropertyChangedCallback(MacroKeyboardAttachedButton.KeyChangedCallback)));

		public static readonly DependencyProperty KeyScanCodeProperty = DependencyProperty.Register("KeyScanCode", typeof(KeyScanCodeV2), typeof(MacroKeyboardAttachedButton), new PropertyMetadata(null, new PropertyChangedCallback(MacroKeyboardAttachedButton.KeyScanCodeChangedCallback)));
	}
}
