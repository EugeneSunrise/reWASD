using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups;

namespace reWASDUI.Views.ContentZoneGamepad.AdvancedStick.StickCross
{
	public partial class StickCrossSectors : BaseDirectionalAnalogGroupSquarePositioningControl
	{
		public ushort VerticalSensitivity
		{
			get
			{
				return (ushort)base.GetValue(StickCrossSectors.VerticalSensitivityProperty);
			}
			set
			{
				base.SetValue(StickCrossSectors.VerticalSensitivityProperty, value);
			}
		}

		private static void VerticalSensitivityChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			StickCrossSectors stickCrossSectors = d as StickCrossSectors;
			if (stickCrossSectors == null)
			{
				return;
			}
			stickCrossSectors.OnVerticalSensitivityChanged();
		}

		public ushort HorizontalSensitivity
		{
			get
			{
				return (ushort)base.GetValue(StickCrossSectors.HorizontalSensitivityProperty);
			}
			set
			{
				base.SetValue(StickCrossSectors.HorizontalSensitivityProperty, value);
			}
		}

		private static void HorizontalSensitivityChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			StickCrossSectors stickCrossSectors = d as StickCrossSectors;
			if (stickCrossSectors == null)
			{
				return;
			}
			stickCrossSectors.OnHorizontalSensitivityChanged();
		}

		public double VerticalFillPercentage
		{
			get
			{
				return (double)base.GetValue(StickCrossSectors.VerticalFillPercentageProperty);
			}
			set
			{
				base.SetValue(StickCrossSectors.VerticalFillPercentageProperty, value);
			}
		}

		public double HorizontalFillPercentage
		{
			get
			{
				return (double)base.GetValue(StickCrossSectors.HorizontalFillPercentageProperty);
			}
			set
			{
				base.SetValue(StickCrossSectors.HorizontalFillPercentageProperty, value);
			}
		}

		public ushort DeadZoneX
		{
			get
			{
				return (ushort)base.GetValue(StickCrossSectors.DeadZoneXProperty);
			}
			set
			{
				base.SetValue(StickCrossSectors.DeadZoneXProperty, value);
			}
		}

		private static void DeadZoneXChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			StickCrossSectors stickCrossSectors = d as StickCrossSectors;
			if (stickCrossSectors == null)
			{
				return;
			}
			stickCrossSectors.OnDeadZoneXChanged();
		}

		public ushort DeadZoneY
		{
			get
			{
				return (ushort)base.GetValue(StickCrossSectors.DeadZoneYProperty);
			}
			set
			{
				base.SetValue(StickCrossSectors.DeadZoneYProperty, value);
			}
		}

		private static void DeadZoneYChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			StickCrossSectors stickCrossSectors = d as StickCrossSectors;
			if (stickCrossSectors == null)
			{
				return;
			}
			stickCrossSectors.OnDeadZoneYChanged();
		}

		public StickCrossSectors()
		{
			this.InitializeComponent();
			base.Loaded += this.OnLoaded;
		}

		protected override void OnDirectionalGroupChanged(DependencyPropertyChangedEventArgs e)
		{
			this.AttachBindings();
		}

		private void AttachBindings()
		{
			BindingOperations.SetBinding(this, StickCrossSectors.HorizontalSensitivityProperty, new Binding("XSensitivity")
			{
				Source = base.DirectionalGroup
			});
			BindingOperations.SetBinding(this, StickCrossSectors.VerticalSensitivityProperty, new Binding("YSensitivity")
			{
				Source = base.DirectionalGroup
			});
			DependencyProperty deadZoneXProperty = StickCrossSectors.DeadZoneXProperty;
			Binding binding = new Binding("XLow");
			BaseDirectionalAnalogGroup directionalGroup = base.DirectionalGroup;
			binding.Source = ((directionalGroup != null) ? directionalGroup.MainDirectionalAnalogGroup : null);
			BindingOperations.SetBinding(this, deadZoneXProperty, binding);
			DependencyProperty deadZoneYProperty = StickCrossSectors.DeadZoneYProperty;
			Binding binding2 = new Binding("YLow");
			BaseDirectionalAnalogGroup directionalGroup2 = base.DirectionalGroup;
			binding2.Source = ((directionalGroup2 != null) ? directionalGroup2.MainDirectionalAnalogGroup : null);
			BindingOperations.SetBinding(this, deadZoneYProperty, binding2);
		}

		protected override void OnSizeChanged()
		{
			base.OnSizeChanged();
			this.RecalculateZones();
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			if (this.isLoaded)
			{
				return;
			}
			this.AttachBindings();
			this.isLoaded = true;
			this._borderDeadZone = (Border)base.Template.FindName("brdDeadZone", this);
			this.RecalculateZones();
		}

		private void OnHorizontalSensitivityChanged()
		{
			this.HorizontalFillPercentage = (double)this.HorizontalSensitivity / (double)this.MaxCrossZoneValue * 25.0;
		}

		private void OnVerticalSensitivityChanged()
		{
			this.VerticalFillPercentage = (double)this.VerticalSensitivity / (double)this.MaxCrossZoneValue * 25.0;
		}

		private void OnDeadZoneYChanged()
		{
			if (this._borderDeadZone == null)
			{
				return;
			}
			this._borderDeadZone.Height = (double)this.DeadZoneY / (double)this.MaxDeadZoneValue * base.SquareHeightWidth;
		}

		private void OnDeadZoneXChanged()
		{
			if (this._borderDeadZone == null)
			{
				return;
			}
			this._borderDeadZone.Width = (double)this.DeadZoneX / (double)this.MaxDeadZoneValue * base.SquareHeightWidth;
		}

		private void RecalculateZones()
		{
			this.OnHorizontalSensitivityChanged();
			this.OnVerticalSensitivityChanged();
			this.OnDeadZoneYChanged();
			this.OnDeadZoneXChanged();
		}

		public static readonly DependencyProperty VerticalSensitivityProperty = DependencyProperty.Register("VerticalSensitivity", typeof(ushort), typeof(StickCrossSectors), new PropertyMetadata(0, new PropertyChangedCallback(StickCrossSectors.VerticalSensitivityChangedCallback)));

		public static readonly DependencyProperty HorizontalSensitivityProperty = DependencyProperty.Register("HorizontalSensitivity", typeof(ushort), typeof(StickCrossSectors), new PropertyMetadata(0, new PropertyChangedCallback(StickCrossSectors.HorizontalSensitivityChangedCallback)));

		public static readonly DependencyProperty VerticalFillPercentageProperty = DependencyProperty.Register("VerticalFillPercentage", typeof(double), typeof(StickCrossSectors), new PropertyMetadata(0.0));

		public static readonly DependencyProperty HorizontalFillPercentageProperty = DependencyProperty.Register("HorizontalFillPercentage", typeof(double), typeof(StickCrossSectors), new PropertyMetadata(0.0));

		public static readonly DependencyProperty DeadZoneXProperty = DependencyProperty.Register("DeadZoneX", typeof(ushort), typeof(StickCrossSectors), new PropertyMetadata(0, new PropertyChangedCallback(StickCrossSectors.DeadZoneXChangedCallback)));

		public static readonly DependencyProperty DeadZoneYProperty = DependencyProperty.Register("DeadZoneY", typeof(ushort), typeof(StickCrossSectors), new PropertyMetadata(0, new PropertyChangedCallback(StickCrossSectors.DeadZoneYChangedCallback)));

		private ushort MaxDeadZoneValue = 32768;

		private ushort MaxCrossZoneValue = 32768;

		private Border _borderDeadZone;

		private bool isLoaded;
	}
}
