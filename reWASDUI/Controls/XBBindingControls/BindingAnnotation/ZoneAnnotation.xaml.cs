using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using reWASDCommon.Infrastructure.Enums;
using reWASDUI.Infrastructure.KeyBindings.ActivatorXB;
using reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups;
using XBEliteWPF.Utils.Extensions;

namespace reWASDUI.Controls.XBBindingControls.BindingAnnotation
{
	public partial class ZoneAnnotation : BaseServicesResolvingControl
	{
		public ActivatorXBBinding ActivatorXBBinding
		{
			get
			{
				return (ActivatorXBBinding)base.GetValue(ZoneAnnotation.ActivatorXBBindingProperty);
			}
			set
			{
				base.SetValue(ZoneAnnotation.ActivatorXBBindingProperty, value);
			}
		}

		private static void ActivatorXBBindingChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ZoneAnnotation zoneAnnotation = d as ZoneAnnotation;
			if (zoneAnnotation == null)
			{
				return;
			}
			zoneAnnotation.OnActivatorXBBindingChanged(e);
		}

		private void OnActivatorXBBindingChanged(DependencyPropertyChangedEventArgs e)
		{
			ActivatorXBBinding activatorXBBinding = this.ActivatorXBBinding;
			this.DirectionalGroup = ((activatorXBBinding != null) ? activatorXBBinding.HostDictionary.HostXBBinding.HostCollection.GetDirectionalGroupByXBBinding(this.ActivatorXBBinding.HostDictionary.HostXBBinding) : null) as BaseDirectionalAnalogGroup;
			ActivatorXBBinding activatorXBBinding2 = this.ActivatorXBBinding;
			this.Direction = ((activatorXBBinding2 != null) ? GamepadButtonExtensions.GetDirection(activatorXBBinding2.GamepadButton) : null);
		}

		public BaseDirectionalAnalogGroup DirectionalGroup
		{
			get
			{
				return (BaseDirectionalAnalogGroup)base.GetValue(ZoneAnnotation.DirectionalGroupProperty);
			}
			set
			{
				base.SetValue(ZoneAnnotation.DirectionalGroupProperty, value);
			}
		}

		public Zone AnnotatedZone
		{
			get
			{
				return (Zone)base.GetValue(ZoneAnnotation.AnnotatedZoneProperty);
			}
			set
			{
				base.SetValue(ZoneAnnotation.AnnotatedZoneProperty, value);
			}
		}

		public Direction? Direction
		{
			get
			{
				return new Direction?((Direction)base.GetValue(ZoneAnnotation.DirectionProperty));
			}
			set
			{
				base.SetValue(ZoneAnnotation.DirectionProperty, value);
			}
		}

		public bool IsShowZoneIcon
		{
			get
			{
				return (bool)base.GetValue(ZoneAnnotation.IsShowZoneIconProperty);
			}
			set
			{
				base.SetValue(ZoneAnnotation.IsShowZoneIconProperty, value);
			}
		}

		public bool IsShowZoneContent
		{
			get
			{
				return (bool)base.GetValue(ZoneAnnotation.IsShowZoneContentProperty);
			}
			set
			{
				base.SetValue(ZoneAnnotation.IsShowZoneContentProperty, value);
			}
		}

		public bool IsLabelMode
		{
			get
			{
				return (bool)base.GetValue(ZoneAnnotation.IsLabelModeProperty);
			}
			set
			{
				base.SetValue(ZoneAnnotation.IsLabelModeProperty, value);
			}
		}

		public ZoneAnnotation()
		{
			this.InitializeComponent();
		}

		private void SetCurrentDirectionButton_Click(object sender, RoutedEventArgs e)
		{
			if (this.ActivatorXBBinding == null)
			{
				return;
			}
			BaseDirectionalAnalogGroup directionalGroup = this.DirectionalGroup;
			if (directionalGroup == null)
			{
				return;
			}
			Direction? direction = this.Direction;
			if (direction == null)
			{
				return;
			}
			directionalGroup.CurrentSelectedDirection = direction.Value;
			directionalGroup.CurrentSelectedZone = this.AnnotatedZone;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				((Button)target).Click += this.SetCurrentDirectionButton_Click;
			}
		}

		public static readonly DependencyProperty ActivatorXBBindingProperty = DependencyProperty.Register("ActivatorXBBinding", typeof(ActivatorXBBinding), typeof(ZoneAnnotation), new PropertyMetadata(null, new PropertyChangedCallback(ZoneAnnotation.ActivatorXBBindingChangedCallback)));

		public static readonly DependencyProperty DirectionalGroupProperty = DependencyProperty.Register("DirectionalGroup", typeof(BaseDirectionalAnalogGroup), typeof(ZoneAnnotation), new PropertyMetadata(null));

		public static readonly DependencyProperty AnnotatedZoneProperty = DependencyProperty.Register("AnnotatedZone", typeof(Zone), typeof(ZoneAnnotation), new PropertyMetadata(0));

		public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register("Direction", typeof(Direction?), typeof(ZoneAnnotation), new PropertyMetadata(0));

		public static readonly DependencyProperty IsShowZoneIconProperty = DependencyProperty.Register("IsShowZoneIcon", typeof(bool), typeof(ZoneAnnotation), new PropertyMetadata(true));

		public static readonly DependencyProperty IsShowZoneContentProperty = DependencyProperty.Register("IsShowZoneContent", typeof(bool), typeof(ZoneAnnotation), new PropertyMetadata(true));

		public static readonly DependencyProperty IsLabelModeProperty = DependencyProperty.Register("IsLabelMode", typeof(bool), typeof(ZoneAnnotation), new PropertyMetadata(false));
	}
}
