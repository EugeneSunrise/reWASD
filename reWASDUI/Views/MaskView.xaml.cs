using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View.Controls.ComboBoxes;
using reWASDUI.Controls;
using reWASDUI.Controls.XBBindingControls.BindingFrame;
using reWASDUI.DataModels;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.Mask;
using Xceed.Wpf.Toolkit.Core.Utilities;

namespace reWASDUI.Views
{
	public partial class MaskView : BaseServicesDataContextControl
	{
		public MaskView()
		{
			this.InitializeComponent();
			base.Loaded += this.OnLoaded;
			App.EventAggregator.GetEvent<CurrentConfigChanged>().Subscribe(delegate(ConfigVM shift)
			{
				if (shift != null)
				{
					this.SubscribeTo();
				}
			});
			this.SubscribeTo();
		}

		private void SubscribeTo()
		{
			if (this._subscribedCollection != null)
			{
				this._subscribedCollection.CollectionItemPropertyChanged -= this.CurrentEditItemChanged;
			}
			BaseXBBindingCollection realCurrentBeingMappedBindingCollection = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
			this._subscribedCollection = ((realCurrentBeingMappedBindingCollection != null) ? realCurrentBeingMappedBindingCollection.MaskBindingCollection : null);
			if (this._subscribedCollection != null)
			{
				this._subscribedCollection.CollectionItemPropertyChangedExtended += new PropertyChangedExtendedEventHandler(this.CurrentEditItemChanged);
			}
		}

		private void CurrentEditItemChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "CurrentEditItem")
			{
				FrameworkElement frameworkElement = VisualTreeHelperEx.FindDescendantByName(this.frameUC, "cmbKeyCode") as FrameworkElement;
				if (frameworkElement == null)
				{
					return;
				}
				frameworkElement.Focus();
			}
		}

		private void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			List<Type> list = new List<Type>
			{
				typeof(ComboBox),
				typeof(Popup),
				typeof(BindingFrameUC)
			};
			if (VisualTreeHelperExt.FindParent((DependencyObject)e.OriginalSource, list, null) != null)
			{
				return;
			}
			base.GameProfilesService.RealCurrentBeingMappedBindingCollection.MaskBindingCollection.CurrentEditItem = null;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			BaseXBBindingCollection realCurrentBeingMappedBindingCollection = base.GameProfilesService.RealCurrentBeingMappedBindingCollection;
			if (realCurrentBeingMappedBindingCollection == null)
			{
				return;
			}
			realCurrentBeingMappedBindingCollection.MaskBindingCollection.FilterChanged();
		}

		private MaskItemCollection _subscribedCollection;
	}
}
