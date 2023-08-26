using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using DiscSoft.NET.Common.View;

namespace reWASDUI.Views.ContentZoneGamepad.Macro.Mouse
{
	public partial class MacroMouseAttachedButton : SVGElementAttachedButton
	{
		public MouseButton? MouseButton
		{
			get
			{
				return (MouseButton?)base.GetValue(MacroMouseAttachedButton.MouseButtonProperty);
			}
			set
			{
				base.SetValue(MacroMouseAttachedButton.MouseButtonProperty, value);
			}
		}

		private static void MouseButtonChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MacroMouseAttachedButton macroMouseAttachedButton = d as MacroMouseAttachedButton;
			if (macroMouseAttachedButton == null)
			{
				return;
			}
			macroMouseAttachedButton.OnMouseButtonChanged();
		}

		public MacroMouseAttachedButton()
		{
			this.InitializeComponent();
			base.IgnoreIsCurrentBinding = true;
		}

		private void OnMouseButtonChanged()
		{
			if (this.MouseButton == null)
			{
				return;
			}
			base.SVGElementName = this.MouseButton.ToString();
			base.CommandParameter = this.MouseButton;
		}

		public static readonly DependencyProperty MouseButtonProperty = DependencyProperty.Register("MouseButton", typeof(MouseButton?), typeof(MacroMouseAttachedButton), new PropertyMetadata(null, new PropertyChangedCallback(MacroMouseAttachedButton.MouseButtonChangedCallback)));
	}
}
