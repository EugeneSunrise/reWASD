using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups;

namespace reWASDUI.Controls.XBBindingControls.BindingAnnotation
{
	public partial class GamepadBindingZonesAnnotation : BaseServicesResolvingControl
	{
		public Brush AnnotationRecolorBrush
		{
			get
			{
				return (Brush)base.GetValue(GamepadBindingZonesAnnotation.AnnotationRecolorBrushProperty);
			}
			set
			{
				base.SetValue(GamepadBindingZonesAnnotation.AnnotationRecolorBrushProperty, value);
			}
		}

		public BaseDirectionalAnalogGroup DirectionalGroup
		{
			get
			{
				return (BaseDirectionalAnalogGroup)base.GetValue(GamepadBindingZonesAnnotation.DirectionalGroupProperty);
			}
			set
			{
				base.SetValue(GamepadBindingZonesAnnotation.DirectionalGroupProperty, value);
			}
		}

		public Zone AnnotatedZone
		{
			get
			{
				return (Zone)base.GetValue(GamepadBindingZonesAnnotation.AnnotatedZoneProperty);
			}
			set
			{
				base.SetValue(GamepadBindingZonesAnnotation.AnnotatedZoneProperty, value);
			}
		}

		public GamepadBindingZonesAnnotation()
		{
			this.InitializeComponent();
		}

		public static readonly DependencyProperty AnnotationRecolorBrushProperty = DependencyProperty.Register("AnnotationRecolorBrush", typeof(Brush), typeof(GamepadBindingZonesAnnotation), new PropertyMetadata(null));

		public static readonly DependencyProperty DirectionalGroupProperty = DependencyProperty.Register("DirectionalGroup", typeof(BaseDirectionalAnalogGroup), typeof(GamepadBindingZonesAnnotation), new PropertyMetadata(null));

		public static readonly DependencyProperty AnnotatedZoneProperty = DependencyProperty.Register("AnnotatedZone", typeof(Zone), typeof(GamepadBindingZonesAnnotation), new PropertyMetadata(0));
	}
}
