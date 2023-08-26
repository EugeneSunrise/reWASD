using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.Mask;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.Properties;
using reWASDUI.Services;

namespace reWASDUI.Controls.XBBindingControls.BindingAnnotation
{
	public partial class GamepadMaskAnnotationToolTip : BaseXBBindingAnnotation, INotifyPropertyChanged
	{
		public ICollectionView FilteredMasksForAnnotations
		{
			get
			{
				return this._filteredMasksForAnnotations;
			}
			set
			{
				this.SetProperty(ref this._filteredMasksForAnnotations, value, "FilteredMasksForAnnotations");
			}
		}

		public GamepadMaskAnnotationToolTip()
		{
			this.InitializeComponent();
			base.Loaded += this.OnLoaded;
		}

		protected override void ReEvaluateXBBinding()
		{
		}

		protected override void ReEvaluateVisibility()
		{
			Visibility visibility;
			if (base.XBBinding != null)
			{
				GameProfilesService gameProfilesService = base.GameProfilesService;
				bool flag;
				if (gameProfilesService == null)
				{
					flag = false;
				}
				else
				{
					BaseXBBindingCollection realCurrentBeingMappedBindingCollection = gameProfilesService.RealCurrentBeingMappedBindingCollection;
					bool? flag2 = ((realCurrentBeingMappedBindingCollection != null) ? new bool?(realCurrentBeingMappedBindingCollection.MaskBindingCollection.Any(new Func<MaskItem, bool>(this.Filter))) : null);
					bool flag3 = true;
					flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
				if (flag)
				{
					visibility = Visibility.Visible;
					goto IL_60;
				}
			}
			visibility = Visibility.Collapsed;
			IL_60:
			base.Visibility = visibility;
		}

		protected override void ReEvaluateIsCurrentBinding()
		{
		}

		protected override void OnXBBindingChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnXBBindingChanged(e);
			this.RecreateFilteredSource();
		}

		private void RecreateFilteredSource()
		{
			if (base.XBBinding == null)
			{
				return;
			}
			this.ReEvaluateVisibility();
			ICollectionView defaultView = CollectionViewSource.GetDefaultView(base.XBBinding.HostCollection.MaskBindingCollection);
			defaultView.Filter = new Predicate<object>(this.Filter);
			this.FilteredMasksForAnnotations = defaultView;
		}

		private bool Filter(object obj)
		{
			return this.Filter(obj as MaskItem);
		}

		private bool Filter(MaskItem mi)
		{
			if (mi != null)
			{
				return mi.MaskConditions.All((MaskItemCondition mic) => mic.ControllerButton.IsGamepad || mic.ControllerButton.IsKeyScanCode || mic.IsNotSelected) && mi.MaskConditions.Any(delegate(MaskItemCondition mic)
				{
					if (mic.ControllerButton.IsSet)
					{
						AssociatedControllerButton controllerButton = mic.ControllerButton;
						XBBinding xbbinding = base.XBBinding;
						return controllerButton.IsAssociatedSetToEqualButtons((xbbinding != null) ? xbbinding.ControllerButton : null);
					}
					return false;
				});
			}
			return false;
		}

		private void FrameworkElement_OnToolTipOpening(object sender, ToolTipEventArgs e)
		{
			this.RecreateFilteredSource();
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

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			if (this._isLoaded)
			{
				return;
			}
			this._isLoaded = true;
			if (base.EventAggregator != null)
			{
				base.EventAggregator.GetEvent<MaskViewChanged>().Subscribe(delegate(object o)
				{
					this.RefreshVisibilityBindingAndIsCurrentBinding();
				});
			}
		}

		private ICollectionView _filteredMasksForAnnotations;

		private bool _isLoaded;
	}
}
