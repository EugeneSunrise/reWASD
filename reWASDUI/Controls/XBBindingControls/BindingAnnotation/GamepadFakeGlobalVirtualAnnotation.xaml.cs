using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups;

namespace reWASDUI.Controls.XBBindingControls.BindingAnnotation
{
	public partial class GamepadFakeGlobalVirtualAnnotation : BaseXBBindingAnnotation
	{
		public Drawing VirtualAnnotationDrawing
		{
			get
			{
				return (Drawing)base.GetValue(GamepadFakeGlobalVirtualAnnotation.VirtualAnnotationDrawingProperty);
			}
			set
			{
				base.SetValue(GamepadFakeGlobalVirtualAnnotation.VirtualAnnotationDrawingProperty, value);
			}
		}

		public BaseDirectionalGroup DirectionalBinding
		{
			get
			{
				return (BaseDirectionalGroup)base.GetValue(GamepadFakeGlobalVirtualAnnotation.DirectionalBindingProperty);
			}
			set
			{
				base.SetValue(GamepadFakeGlobalVirtualAnnotation.DirectionalBindingProperty, value);
			}
		}

		public GamepadFakeGlobalVirtualAnnotation()
		{
			this.InitializeComponent();
			base.DataContext = this;
			base.Loaded += this.OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			this.ReEvaluateVisibility();
		}

		protected override void ReEvaluateXBBinding()
		{
		}

		protected override void OnXBBindingChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnXBBindingChanged(e);
			XBBinding xbbinding = base.XBBinding;
			this.DirectionalBinding = ((xbbinding != null) ? xbbinding.HostCollection.GetDirectionalGroupByXBBinding(base.XBBinding) : null);
		}

		protected override void ReEvaluateVisibility()
		{
			MultiBindingExpression multiBindingExpression = BindingOperations.GetMultiBindingExpression(this, UIElement.VisibilityProperty);
			if (multiBindingExpression == null)
			{
				return;
			}
			multiBindingExpression.UpdateTarget();
		}

		protected override void ReEvaluateIsCurrentBinding()
		{
		}

		public static readonly DependencyProperty VirtualAnnotationDrawingProperty = DependencyProperty.Register("VirtualAnnotationDrawing", typeof(Drawing), typeof(GamepadFakeGlobalVirtualAnnotation), new PropertyMetadata(null));

		public static readonly DependencyProperty DirectionalBindingProperty = DependencyProperty.Register("DirectionalBinding", typeof(BaseDirectionalGroup), typeof(GamepadFakeGlobalVirtualAnnotation), new PropertyMetadata(null));
	}
}
