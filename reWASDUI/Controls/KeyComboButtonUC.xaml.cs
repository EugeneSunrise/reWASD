using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.XB;

namespace reWASDUI.Controls
{
	public partial class KeyComboButtonUC : UserControl, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public AssociatedControllerButton ControllerButtonShowFor
		{
			get
			{
				return (AssociatedControllerButton)base.GetValue(KeyComboButtonUC.ControllerButtonShowForProperty);
			}
			set
			{
				base.SetValue(KeyComboButtonUC.ControllerButtonShowForProperty, value);
			}
		}

		private static void OnControllerButtonShowForChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			((KeyComboButtonUC)sender).UpdateXBBinding();
		}

		public void UpdateXBBinding()
		{
			XBBinding xbbinding;
			if (this.ControllerButtonShowFor != null)
			{
				BaseXBBindingCollection realCurrentBeingMappedBindingCollection = App.GameProfilesService.RealCurrentBeingMappedBindingCollection;
				xbbinding = ((realCurrentBeingMappedBindingCollection != null) ? realCurrentBeingMappedBindingCollection.GetXBBindingByAssociatedControllerButton(this.ControllerButtonShowFor) : null);
			}
			else
			{
				xbbinding = null;
			}
			this.XBBinding = xbbinding;
		}

		public XBBinding XBBinding
		{
			get
			{
				return (XBBinding)base.GetValue(KeyComboButtonUC.XBBindingProperty);
			}
			set
			{
				base.SetValue(KeyComboButtonUC.XBBindingProperty, value);
			}
		}

		public KeyComboButtonUC()
		{
			this.InitializeComponent();
			base.IsVisibleChanged += this.KeyComboButtonUC_IsVisibleChanged;
		}

		private void KeyComboButtonUC_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue && this.ControllerButtonShowFor != null)
			{
				this.UpdateXBBinding();
			}
		}

		public virtual void OnPropertyChanged(string propertyName = null)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			PropertyChangedEventArgs propertyChangedEventArgs = new PropertyChangedEventArgs(propertyName);
			propertyChanged(this, propertyChangedEventArgs);
		}

		public static readonly DependencyProperty ControllerButtonShowForProperty = DependencyProperty.Register("ControllerButtonShowFor", typeof(AssociatedControllerButton), typeof(KeyComboButtonUC), new PropertyMetadata(null, new PropertyChangedCallback(KeyComboButtonUC.OnControllerButtonShowForChanged)));

		public static readonly DependencyProperty XBBindingProperty = DependencyProperty.Register("XBBinding", typeof(XBBinding), typeof(KeyComboButtonUC), new PropertyMetadata(null));
	}
}
