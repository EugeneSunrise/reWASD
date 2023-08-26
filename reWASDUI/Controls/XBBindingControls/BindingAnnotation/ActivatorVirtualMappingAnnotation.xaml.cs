using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using DiscSoft.NET.Common.Utils;
using reWASDUI.Controls.XBBindingControls.ButtonBinding;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.ActivatorXB;
using reWASDUI.Services;

namespace reWASDUI.Controls.XBBindingControls.BindingAnnotation
{
	public partial class ActivatorVirtualMappingAnnotation : BaseServicesResolvingControl
	{
		private static void ActivatorXBBindingCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ActivatorVirtualMappingAnnotation activatorVirtualMappingAnnotation = d as ActivatorVirtualMappingAnnotation;
			if (activatorVirtualMappingAnnotation == null)
			{
				return;
			}
			activatorVirtualMappingAnnotation.RecalculateShiftVisibility();
		}

		public ActivatorXBBinding ActivatorXBBinding
		{
			get
			{
				return (ActivatorXBBinding)base.GetValue(ActivatorVirtualMappingAnnotation.ActivatorXBBindingProperty);
			}
			set
			{
				base.SetValue(ActivatorVirtualMappingAnnotation.ActivatorXBBindingProperty, value);
			}
		}

		public ActivatorXBBinding ShiftActivatorXBBinding
		{
			get
			{
				return (ActivatorXBBinding)base.GetValue(ActivatorVirtualMappingAnnotation.ShiftActivatorXBBindingProperty);
			}
			set
			{
				base.SetValue(ActivatorVirtualMappingAnnotation.ShiftActivatorXBBindingProperty, value);
			}
		}

		public bool IsLabelMode
		{
			get
			{
				return (bool)base.GetValue(ActivatorVirtualMappingAnnotation.IsLabelModeProperty);
			}
			set
			{
				base.SetValue(ActivatorVirtualMappingAnnotation.IsLabelModeProperty, value);
			}
		}

		public bool IsShiftVisible
		{
			get
			{
				return (bool)base.GetValue(ActivatorVirtualMappingAnnotation.IsShiftVisibleProperty);
			}
			set
			{
				base.SetValue(ActivatorVirtualMappingAnnotation.IsShiftVisibleProperty, value);
			}
		}

		public bool IsMappingVisible
		{
			get
			{
				return (bool)base.GetValue(ActivatorVirtualMappingAnnotation.IsMappingVisibleProperty);
			}
			set
			{
				base.SetValue(ActivatorVirtualMappingAnnotation.IsMappingVisibleProperty, value);
			}
		}

		public ActivatorVirtualMappingAnnotation()
		{
			this.InitializeComponent();
			base.Loaded += delegate(object sender, RoutedEventArgs args)
			{
				this.RecalculateShiftVisibility();
			};
			base.IsVisibleChanged += delegate(object sender, DependencyPropertyChangedEventArgs args)
			{
				this.RecalculateShiftVisibility();
			};
		}

		protected override void OnServicesReady()
		{
			base.OnServicesReady();
			if (!this._eventArgsSubed && base.EventAggregator != null)
			{
				base.EventAggregator.GetEvent<CurrentShiftBindingCollectionChanged>().Subscribe(new Action<ShiftXBBindingCollection>(this.OnShiftCollectionChanged));
				base.EventAggregator.GetEvent<CurrentBindingCollectionWrapperChanged>().Subscribe(new Action<SubConfigData>(this.OnBindingCollectionChanged));
				this._eventArgsSubed = true;
			}
		}

		private void OnBindingCollectionChanged(SubConfigData col)
		{
			this.RecalculateShiftVisibility();
		}

		private void OnShiftCollectionChanged(ShiftXBBindingCollection shiftCol)
		{
			this.RecalculateShiftVisibility();
		}

		private void RecalculateShiftVisibility()
		{
			if (base.GameProfilesService == null || this.ActivatorXBBinding == null)
			{
				return;
			}
			BaseXBBindingCollection hostCollection = this.ActivatorXBBinding.HostDictionary.HostXBBinding.HostCollection;
			SubConfigData subConfigData = ((hostCollection != null) ? hostCollection.SubConfigData : null);
			GameProfilesService gameProfilesService = base.GameProfilesService;
			object obj;
			if (gameProfilesService == null)
			{
				obj = null;
			}
			else
			{
				BaseXBBindingCollection realCurrentBeingMappedBindingCollection = gameProfilesService.RealCurrentBeingMappedBindingCollection;
				obj = ((realCurrentBeingMappedBindingCollection != null) ? realCurrentBeingMappedBindingCollection.SubConfigData : null);
			}
			if (subConfigData != obj)
			{
				return;
			}
			this.IsMappingVisible = true;
			if (!this.ActivatorXBBinding.IsVirtualMappingPresent && !this.ActivatorXBBinding.IsAdaptiveTriggers)
			{
				this.IsMappingVisible = false;
			}
			if (this.ActivatorXBBinding.IsJumpToShift)
			{
				this.ActivatorXBBinding.UpdateJumpToShiftProperties();
			}
		}

		private void ListBox_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (VisualTreeHelperExt.FindParent<SingleButtonBinding>(this, null) != null)
			{
				ActivatorXBBinding activatorXBBinding = this.ActivatorXBBinding;
				if (activatorXBBinding == null)
				{
					return;
				}
				activatorXBBinding.OpenMacroEditorCommandCommand.Execute();
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				((ItemsControl)target).MouseLeftButtonDown += this.ListBox_OnMouseLeftButtonDown;
			}
		}

		public static readonly DependencyProperty ActivatorXBBindingProperty = DependencyProperty.Register("ActivatorXBBinding", typeof(ActivatorXBBinding), typeof(ActivatorVirtualMappingAnnotation), new PropertyMetadata(null, new PropertyChangedCallback(ActivatorVirtualMappingAnnotation.ActivatorXBBindingCallback)));

		public static readonly DependencyProperty ShiftActivatorXBBindingProperty = DependencyProperty.Register("ShiftActivatorXBBinding", typeof(ActivatorXBBinding), typeof(ActivatorVirtualMappingAnnotation), new PropertyMetadata(null));

		public static readonly DependencyProperty IsLabelModeProperty = DependencyProperty.Register("IsLabelMode", typeof(bool), typeof(ActivatorVirtualMappingAnnotation), new PropertyMetadata(false));

		public static readonly DependencyProperty IsShiftVisibleProperty = DependencyProperty.Register("IsShiftVisible", typeof(bool), typeof(ActivatorVirtualMappingAnnotation), new PropertyMetadata(true));

		public static readonly DependencyProperty IsMappingVisibleProperty = DependencyProperty.Register("IsMappingVisible", typeof(bool), typeof(ActivatorVirtualMappingAnnotation), new PropertyMetadata(true));

		private bool _eventArgsSubed;
	}
}
