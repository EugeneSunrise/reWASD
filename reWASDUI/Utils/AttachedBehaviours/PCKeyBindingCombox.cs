using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;

namespace reWASDUI.Utils.AttachedBehaviours
{
	internal class PCKeyBindingCombox
	{
		public static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ComboBox comboBox = d as ComboBox;
			if (comboBox == null)
			{
				return;
			}
			if (e.NewValue is bool && (bool)e.NewValue)
			{
				UIElement uielement = comboBox;
				KeyEventHandler keyEventHandler;
				if ((keyEventHandler = PCKeyBindingCombox.<>O.<0>__ProccessBindingKey) == null)
				{
					keyEventHandler = (PCKeyBindingCombox.<>O.<0>__ProccessBindingKey = new KeyEventHandler(PCKeyBindingCombox.ProccessBindingKey));
				}
				uielement.PreviewKeyDown += keyEventHandler;
				return;
			}
			UIElement uielement2 = comboBox;
			KeyEventHandler keyEventHandler2;
			if ((keyEventHandler2 = PCKeyBindingCombox.<>O.<0>__ProccessBindingKey) == null)
			{
				keyEventHandler2 = (PCKeyBindingCombox.<>O.<0>__ProccessBindingKey = new KeyEventHandler(PCKeyBindingCombox.ProccessBindingKey));
			}
			uielement2.PreviewKeyDown -= keyEventHandler2;
		}

		public static void SetIsEnabled(DependencyObject element, bool value)
		{
			element.SetValue(PCKeyBindingCombox.IsEnabledProperty, value);
		}

		public static bool GetIsEnabled(DependencyObject element)
		{
			return (bool)element.GetValue(PCKeyBindingCombox.IsEnabledProperty);
		}

		public static void ProccessBindingKey(object d, KeyEventArgs e)
		{
			ComboBox comboBox = d as ComboBox;
			if (comboBox == null)
			{
				return;
			}
			Key key = ((e.Key == Key.System) ? e.SystemKey : e.Key);
			if (!string.IsNullOrWhiteSpace(KeyScanCodeV2.FindKeyScanCodeByKey(key).Description))
			{
				comboBox.SelectedItem = KeyScanCodeV2.FindKeyScanCodeByKey(key);
			}
			e.Handled = true;
		}

		public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(PCKeyBindingCombox), new PropertyMetadata(false, new PropertyChangedCallback(PCKeyBindingCombox.OnIsEnabledChanged)));

		[CompilerGenerated]
		private static class <>O
		{
			public static KeyEventHandler <0>__ProccessBindingKey;
		}
	}
}
