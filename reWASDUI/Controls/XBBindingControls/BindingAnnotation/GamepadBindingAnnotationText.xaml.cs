using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using reWASDUI.Infrastructure.KeyBindings.ActivatorXB;
using reWASDUI.Infrastructure.KeyBindings.MacroBinding;
using reWASDUI.Infrastructure.KeyBindings.XB;

namespace reWASDUI.Controls.XBBindingControls.BindingAnnotation
{
	public partial class GamepadBindingAnnotationText : BaseXBBindingAnnotation
	{
		public ImageSource ButtonIcoSource
		{
			get
			{
				return (ImageSource)base.GetValue(GamepadBindingAnnotationText.ButtonIcoSourceProperty);
			}
			set
			{
				base.SetValue(GamepadBindingAnnotationText.ButtonIcoSourceProperty, value);
			}
		}

		public bool ShowHardwareMapping
		{
			get
			{
				return (bool)base.GetValue(GamepadBindingAnnotationText.ShowHardwareMappingProperty);
			}
			set
			{
				base.SetValue(GamepadBindingAnnotationText.ShowHardwareMappingProperty, value);
			}
		}

		public bool ShowHardwareMappings
		{
			get
			{
				return (bool)base.GetValue(GamepadBindingAnnotationText.ShowHardwareMappingsProperty);
			}
			set
			{
				base.SetValue(GamepadBindingAnnotationText.ShowHardwareMappingsProperty, value);
			}
		}

		public bool ShowAsTooltip
		{
			get
			{
				return (bool)base.GetValue(GamepadBindingAnnotationText.ShowAsTooltipProperty);
			}
			set
			{
				base.SetValue(GamepadBindingAnnotationText.ShowAsTooltipProperty, value);
			}
		}

		public GamepadBindingAnnotationText()
		{
			this.InitializeComponent();
		}

		protected override void OnIsLabelModeChanged(DependencyPropertyChangedEventArgs e)
		{
			this.ReEvaluateXBBinding();
			this.ReEvaluateVisibility();
		}

		protected override void OnIsShowMappingsViewChanged(DependencyPropertyChangedEventArgs e)
		{
			this.ReEvaluateXBBinding();
			this.ReEvaluateVisibility();
		}

		protected override void ReEvaluateXBBinding()
		{
			XBBinding xbbinding = base.XBBinding;
			if (xbbinding == null)
			{
				return;
			}
			ActivatorXBBindingDictionary activatorXBBindingDictionary = xbbinding.ActivatorXBBindingDictionary;
			if (activatorXBBindingDictionary == null)
			{
				return;
			}
			activatorXBBindingDictionary.ForEachValue(delegate(ActivatorXBBinding v)
			{
				v.RefreshAnnotations();
				v.OnShowMappingsViewChanged();
			});
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

		public static readonly DependencyProperty ButtonIcoSourceProperty = DependencyProperty.Register("ButtonIcoSource", typeof(ImageSource), typeof(GamepadBindingAnnotationText), new PropertyMetadata(null));

		public static readonly DependencyProperty ShowHardwareMappingProperty = DependencyProperty.Register("ShowHardwareMapping", typeof(bool), typeof(GamepadBindingAnnotationText), new PropertyMetadata(true));

		public static readonly DependencyProperty ShowHardwareMappingsProperty = DependencyProperty.Register("ShowHardwareMappings", typeof(bool), typeof(GamepadBindingAnnotationText), new PropertyMetadata(false));

		public static readonly DependencyProperty ShowAsTooltipProperty = DependencyProperty.Register("ShowAsTooltip", typeof(bool), typeof(GamepadBindingAnnotationText), new PropertyMetadata(false));
	}
}
