using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using reWASDUI.Infrastructure.KeyBindings.XB;

namespace reWASDUI.Controls
{
	public partial class UnmapSingleControl : UserControl
	{
		public XBBinding XBBinding
		{
			get
			{
				return (XBBinding)base.GetValue(UnmapSingleControl.XBBindingProperty);
			}
			set
			{
				base.SetValue(UnmapSingleControl.XBBindingProperty, value);
			}
		}

		public UnmapSingleControl()
		{
			this.InitializeComponent();
			base.Loaded += this.OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			if (!this._isFirstLoadInited)
			{
				this._isFirstLoadInited = true;
				if (this.XBBinding == null)
				{
					BindingOperations.SetBinding(this, UnmapSingleControl.XBBindingProperty, new Binding("GameProfilesService.RealCurrentBeingMappedBindingCollection.CurrentXBBinding"));
				}
			}
		}

		public static readonly DependencyProperty XBBindingProperty = DependencyProperty.Register("XBBinding", typeof(XBBinding), typeof(UnmapSingleControl), new PropertyMetadata(null));

		private bool _isFirstLoadInited;
	}
}
