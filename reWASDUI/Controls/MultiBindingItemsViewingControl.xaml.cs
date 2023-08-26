using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using reWASDUI.Infrastructure.KeyBindings.ActivatorXB;
using reWASDUI.Infrastructure.KeyBindings.Mask;

namespace reWASDUI.Controls
{
	public partial class MultiBindingItemsViewingControl : UserControl
	{
		public List<object> Items
		{
			get
			{
				return (List<object>)base.GetValue(MultiBindingItemsViewingControl.ItemsProperty);
			}
			set
			{
				base.SetValue(MultiBindingItemsViewingControl.ItemsProperty, value);
			}
		}

		public MultiBindingItemsViewingControl.MultiBindingItemsViewingControlItemTemplateSelector ItemTemplateSelector
		{
			get
			{
				return (MultiBindingItemsViewingControl.MultiBindingItemsViewingControlItemTemplateSelector)base.GetValue(MultiBindingItemsViewingControl.ItemTemplateSelectorProperty);
			}
			set
			{
				base.SetValue(MultiBindingItemsViewingControl.ItemTemplateSelectorProperty, value);
			}
		}

		public MultiBindingItemsViewingControl()
		{
			this.ItemTemplateSelector = new MultiBindingItemsViewingControl.MultiBindingItemsViewingControlItemTemplateSelector();
			this.InitializeComponent();
		}

		public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(List<object>), typeof(MultiBindingItemsViewingControl), new PropertyMetadata(null));

		public static readonly DependencyProperty ItemTemplateSelectorProperty = DependencyProperty.Register("ItemTemplateSelector", typeof(MultiBindingItemsViewingControl.MultiBindingItemsViewingControlItemTemplateSelector), typeof(MultiBindingItemsViewingControl), new PropertyMetadata(null));

		public class MultiBindingItemsViewingControlItemTemplateSelector : DataTemplateSelector
		{
			public override DataTemplate SelectTemplate(object item, DependencyObject container)
			{
				FrameworkElement frameworkElement = container as FrameworkElement;
				if (item == null || frameworkElement == null)
				{
					return null;
				}
				if (item is MaskItem)
				{
					return frameworkElement.FindResource("MaskItemDataTemplate") as DataTemplate;
				}
				if (item is ActivatorXBBinding)
				{
					return frameworkElement.FindResource("ActivatorXBBindingDataTemplate") as DataTemplate;
				}
				return null;
			}
		}
	}
}
