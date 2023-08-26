using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace reWASDUI.Controls.XBBindingControls.BindingFrame
{
	public partial class RumbleZone : UserControl
	{
		public Brush ControlsBrush
		{
			get
			{
				return (Brush)base.GetValue(RumbleZone.ControlsBrushProperty);
			}
			set
			{
				base.SetValue(RumbleZone.ControlsBrushProperty, value);
			}
		}

		public string Label
		{
			get
			{
				return (string)base.GetValue(RumbleZone.LabelProperty);
			}
			set
			{
				base.SetValue(RumbleZone.LabelProperty, value);
			}
		}

		public bool IsRumble
		{
			get
			{
				return (bool)base.GetValue(RumbleZone.IsRumbleProperty);
			}
			set
			{
				base.SetValue(RumbleZone.IsRumbleProperty, value);
			}
		}

		public ushort Speed
		{
			get
			{
				return (ushort)base.GetValue(RumbleZone.SpeedProperty);
			}
			set
			{
				base.SetValue(RumbleZone.SpeedProperty, value);
			}
		}

		public RumbleZone()
		{
			this.InitializeComponent();
		}

		public static readonly DependencyProperty ControlsBrushProperty = DependencyProperty.Register("ControlsBrush", typeof(Brush), typeof(RumbleZone), new PropertyMetadata(null));

		public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(RumbleZone), new PropertyMetadata(null));

		public static readonly DependencyProperty IsRumbleProperty = DependencyProperty.Register("IsRumble", typeof(bool), typeof(RumbleZone), new PropertyMetadata(false));

		public static readonly DependencyProperty SpeedProperty = DependencyProperty.Register("Speed", typeof(ushort), typeof(RumbleZone), new PropertyMetadata(0));
	}
}
