using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.Localization;
using DiscSoft.NET.Common.Properties;
using DiscSoft.NET.Common.Utils;
using Prism.Commands;
using reWASDUI.Infrastructure.KeyBindings.XB;

namespace reWASDUI.Controls.XBBindingControls
{
	public partial class CopyPasteClearBindingContextMenu : ContextMenu, INotifyPropertyChanged
	{
		public XBBinding XBBinding
		{
			get
			{
				return (XBBinding)base.GetValue(CopyPasteClearBindingContextMenu.XBBindingProperty);
			}
			set
			{
				base.SetValue(CopyPasteClearBindingContextMenu.XBBindingProperty, value);
			}
		}

		public string CopyText
		{
			get
			{
				XBBinding xbbinding = this.XBBinding;
				return DTLocalization.GetString((xbbinding != null && xbbinding.IsMouseDirection) ? 12101 : 11598);
			}
		}

		public string PasteText
		{
			get
			{
				XBBinding xbbinding = this.XBBinding;
				return DTLocalization.GetString((xbbinding != null && xbbinding.IsMouseDirection) ? 12102 : 11599);
			}
		}

		public CopyPasteClearBindingContextMenu()
		{
			this.InitializeComponent();
			base.Loaded += this.OnLoaded;
		}

		protected override void OnOpened(RoutedEventArgs e)
		{
			base.OnOpened(e);
			this.OnPropertyChanged("CopyText");
			this.OnPropertyChanged("PasteText");
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			foreach (MenuItem menuItem in VisualTreeHelperExt.FindChildren<MenuItem>(this))
			{
				DelegateCommand delegateCommand = menuItem.Command as DelegateCommand;
				if (delegateCommand != null)
				{
					delegateCommand.RaiseCanExecuteChanged();
				}
				DelegateCommand<XBBinding> delegateCommand2 = menuItem.Command as DelegateCommand<XBBinding>;
				if (delegateCommand2 != null)
				{
					delegateCommand2.RaiseCanExecuteChanged();
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public static readonly DependencyProperty XBBindingProperty = DependencyProperty.Register("XBBinding", typeof(XBBinding), typeof(CopyPasteClearBindingContextMenu), new PropertyMetadata(null));
	}
}
