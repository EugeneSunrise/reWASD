using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace reWASDUI.Controls
{
	public class MacrosDurationContainer : ContentControl
	{
		public int Duration
		{
			get
			{
				return (int)base.GetValue(MacrosDurationContainer.DurationProperty);
			}
			set
			{
				base.SetValue(MacrosDurationContainer.DurationProperty, value);
			}
		}

		public int Minimum
		{
			get
			{
				return (int)base.GetValue(MacrosDurationContainer.MinimumProperty);
			}
			set
			{
				base.SetValue(MacrosDurationContainer.MinimumProperty, value);
			}
		}

		private static void MinimumChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MacrosDurationContainer macrosDurationContainer = d as MacrosDurationContainer;
			if (macrosDurationContainer == null)
			{
				return;
			}
			macrosDurationContainer.OnMinimumChanged();
		}

		private void OnMinimumChanged()
		{
			if (this.Duration < this.Minimum)
			{
				this.Duration = this.Minimum;
			}
		}

		public int Maximum
		{
			get
			{
				return (int)base.GetValue(MacrosDurationContainer.MaximumProperty);
			}
			set
			{
				base.SetValue(MacrosDurationContainer.MaximumProperty, value);
			}
		}

		private static void MaximumChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MacrosDurationContainer macrosDurationContainer = d as MacrosDurationContainer;
			if (macrosDurationContainer == null)
			{
				return;
			}
			macrosDurationContainer.OnMaximumChanged();
		}

		private void OnMaximumChanged()
		{
			if (this.Duration > this.Maximum)
			{
				this.Duration = this.Maximum;
			}
		}

		public int Increment
		{
			get
			{
				return (int)base.GetValue(MacrosDurationContainer.IncrementProperty);
			}
			set
			{
				base.SetValue(MacrosDurationContainer.IncrementProperty, value);
			}
		}

		public bool ShouldShowDurationControl
		{
			get
			{
				return (bool)base.GetValue(MacrosDurationContainer.ShouldShowDurationControlProperty);
			}
			set
			{
				base.SetValue(MacrosDurationContainer.ShouldShowDurationControlProperty, value);
			}
		}

		public string UnitsString
		{
			get
			{
				return (string)base.GetValue(MacrosDurationContainer.UnitsStringProperty);
			}
			set
			{
				base.SetValue(MacrosDurationContainer.UnitsStringProperty, value);
			}
		}

		public SolidColorBrush UpDownArrowsBrush
		{
			get
			{
				return (SolidColorBrush)base.GetValue(MacrosDurationContainer.UpDownArrowsBrushProperty);
			}
			set
			{
				base.SetValue(MacrosDurationContainer.UpDownArrowsBrushProperty, value);
			}
		}

		public SolidColorBrush HighlightedBorderBrush
		{
			get
			{
				return (SolidColorBrush)base.GetValue(MacrosDurationContainer.HighlightedBorderBrushProperty);
			}
			set
			{
				base.SetValue(MacrosDurationContainer.HighlightedBorderBrushProperty, value);
			}
		}

		public SolidColorBrush DisabledBorderBrush
		{
			get
			{
				return (SolidColorBrush)base.GetValue(MacrosDurationContainer.DisabledBorderBrushProperty);
			}
			set
			{
				base.SetValue(MacrosDurationContainer.DisabledBorderBrushProperty, value);
			}
		}

		public SolidColorBrush DisabledBackground
		{
			get
			{
				return (SolidColorBrush)base.GetValue(MacrosDurationContainer.DisabledBackgroundProperty);
			}
			set
			{
				base.SetValue(MacrosDurationContainer.DisabledBackgroundProperty, value);
			}
		}

		public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration", typeof(int), typeof(MacrosDurationContainer), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(int), typeof(MacrosDurationContainer), new PropertyMetadata(0, new PropertyChangedCallback(MacrosDurationContainer.MinimumChangedCallback)));

		public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(int), typeof(MacrosDurationContainer), new PropertyMetadata(int.MaxValue, new PropertyChangedCallback(MacrosDurationContainer.MaximumChangedCallback)));

		public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register("Increment", typeof(int), typeof(MacrosDurationContainer), new PropertyMetadata(50));

		public static readonly DependencyProperty ShouldShowDurationControlProperty = DependencyProperty.Register("ShouldShowDurationControl", typeof(bool), typeof(MacrosDurationContainer), new PropertyMetadata(true));

		public static readonly DependencyProperty UnitsStringProperty = DependencyProperty.Register("UnitsString", typeof(string), typeof(MacrosDurationContainer), new PropertyMetadata(""));

		public static readonly DependencyProperty UpDownArrowsBrushProperty = DependencyProperty.Register("UpDownArrowsBrush", typeof(SolidColorBrush), typeof(MacrosDurationContainer), new PropertyMetadata(null));

		public static readonly DependencyProperty HighlightedBorderBrushProperty = DependencyProperty.Register("HighlightedBorderBrush", typeof(SolidColorBrush), typeof(MacrosDurationContainer), new PropertyMetadata(null));

		public static readonly DependencyProperty DisabledBorderBrushProperty = DependencyProperty.Register("DisabledBorderBrush", typeof(SolidColorBrush), typeof(MacrosDurationContainer), new PropertyMetadata(null));

		public static readonly DependencyProperty DisabledBackgroundProperty = DependencyProperty.Register("DisabledBackground", typeof(SolidColorBrush), typeof(MacrosDurationContainer), new PropertyMetadata(null));
	}
}
