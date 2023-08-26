using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.Controls.Buttons;
using reWASDUI.Infrastructure;

namespace reWASDUI.Controls
{
	public partial class ScrollableSliderWithManual : UserControl
	{
		public int ScrollStep
		{
			get
			{
				return (int)base.GetValue(ScrollableSliderWithManual.ScrollStepProperty);
			}
			set
			{
				base.SetValue(ScrollableSliderWithManual.ScrollStepProperty, value);
			}
		}

		public bool InvertFill
		{
			get
			{
				return (bool)base.GetValue(ScrollableSliderWithManual.InvertFillProperty);
			}
			set
			{
				base.SetValue(ScrollableSliderWithManual.InvertFillProperty, value);
			}
		}

		public Style ScrollableSliderStyle
		{
			get
			{
				return (Style)base.GetValue(ScrollableSliderWithManual.ScrollableSliderStyleProperty);
			}
			set
			{
				base.SetValue(ScrollableSliderWithManual.ScrollableSliderStyleProperty, value);
			}
		}

		public bool IsSnapToTickEnabled
		{
			get
			{
				return (bool)base.GetValue(Slider.IsSnapToTickEnabledProperty);
			}
			set
			{
				base.SetValue(Slider.IsSnapToTickEnabledProperty, value);
			}
		}

		public double Maximum
		{
			get
			{
				return (double)base.GetValue(ScrollableSliderWithManual.MaximumProperty);
			}
			set
			{
				base.SetValue(ScrollableSliderWithManual.MaximumProperty, value);
			}
		}

		public double Minimum
		{
			get
			{
				return (double)base.GetValue(ScrollableSliderWithManual.MinimumProperty);
			}
			set
			{
				base.SetValue(ScrollableSliderWithManual.MinimumProperty, value);
			}
		}

		public double Value
		{
			get
			{
				return (double)base.GetValue(ScrollableSliderWithManual.ValueProperty);
			}
			set
			{
				base.SetValue(ScrollableSliderWithManual.ValueProperty, value);
			}
		}

		public double SmallChange
		{
			get
			{
				return (double)base.GetValue(ScrollableSliderWithManual.SmallChangeProperty);
			}
			set
			{
				base.SetValue(ScrollableSliderWithManual.SmallChangeProperty, this.SmallChange);
			}
		}

		public double LargeChange
		{
			get
			{
				return (double)base.GetValue(ScrollableSliderWithManual.LargeChangeProperty);
			}
			set
			{
				base.SetValue(ScrollableSliderWithManual.LargeChangeProperty, this.LargeChange);
			}
		}

		public ScrollableSliderWithManual()
		{
			this.InitializeComponent();
			App.EventAggregator.GetEvent<CloseAllPopups>().Subscribe(delegate(object nav)
			{
				this.HidePopupWindow();
			});
			base.IsVisibleChanged += this.ScrollableSliderWithManual_IsVisibleChanged;
		}

		private void HidePopupWindow()
		{
			SVGButton svgbutton = base.Template.FindName("manualButton", this) as SVGButton;
			if (svgbutton != null)
			{
				svgbutton.IsTriggered = false;
			}
		}

		private void ScrollableSliderWithManual_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (!(bool)e.NewValue)
			{
				this.HidePopupWindow();
			}
		}

		public static readonly DependencyProperty ScrollStepProperty = DependencyProperty.Register("ScrollStep", typeof(int), typeof(ScrollableSliderWithManual), new PropertyMetadata(1));

		public static readonly DependencyProperty InvertFillProperty = DependencyProperty.Register("InvertFill", typeof(bool), typeof(ScrollableSliderWithManual), new PropertyMetadata(false));

		public static readonly DependencyProperty ScrollableSliderStyleProperty = DependencyProperty.Register("ScrollableSliderStyle", typeof(Style), typeof(ScrollableSliderWithManual), new PropertyMetadata(null));

		public static readonly DependencyProperty IsSnapToTickEnabledProperty = DependencyProperty.Register("IsSnapToTickEnabled", typeof(bool), typeof(ScrollableSliderWithManual), new FrameworkPropertyMetadata(false));

		public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(ScrollableSliderWithManual));

		public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(ScrollableSliderWithManual));

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(ScrollableSliderWithManual));

		public static readonly DependencyProperty SmallChangeProperty = DependencyProperty.Register("SmallChange", typeof(double), typeof(ScrollableSliderWithManual));

		public static readonly DependencyProperty LargeChangeProperty = DependencyProperty.Register("LargeChange", typeof(double), typeof(ScrollableSliderWithManual));
	}
}
