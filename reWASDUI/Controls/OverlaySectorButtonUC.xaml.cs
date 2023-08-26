using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace reWASDUI.Controls
{
	public partial class OverlaySectorButtonUC : Button, INotifyPropertyChanged
	{
		public OverlaySectorButtonUC()
		{
			this.InitializeComponent();
			base.Loaded += this.OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			this.BigRadius = this.OuterRadius;
		}

		public double InnerRadius
		{
			get
			{
				return (double)base.GetValue(OverlaySectorButtonUC.InnerRadiusProperty);
			}
			set
			{
				base.SetValue(OverlaySectorButtonUC.InnerRadiusProperty, value);
			}
		}

		public double OuterRadius
		{
			get
			{
				return (double)base.GetValue(OverlaySectorButtonUC.OuterRadiusProperty);
			}
			set
			{
				base.SetValue(OverlaySectorButtonUC.OuterRadiusProperty, value);
			}
		}

		public double BigRadiusActive
		{
			get
			{
				return (double)base.GetValue(OverlaySectorButtonUC.BigRadiusActiveProperty);
			}
			set
			{
				base.SetValue(OverlaySectorButtonUC.BigRadiusActiveProperty, value);
			}
		}

		private void OnMouseEnter(object sender, MouseEventArgs e)
		{
		}

		private void OnMouseLeave(object sender, MouseEventArgs e)
		{
		}

		public double PosX
		{
			get
			{
				return (double)base.GetValue(OverlaySectorButtonUC.PosXProperty);
			}
			set
			{
				base.SetValue(OverlaySectorButtonUC.PosXProperty, value);
			}
		}

		public double PosY
		{
			get
			{
				return (double)base.GetValue(OverlaySectorButtonUC.PosYProperty);
			}
			set
			{
				base.SetValue(OverlaySectorButtonUC.PosYProperty, value);
			}
		}

		public double BigRadius
		{
			get
			{
				return this._bigRadius;
			}
			set
			{
				if (value != this._bigRadius)
				{
					this._bigRadius = value;
					this.OnPropertyChanged("BigRadius");
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public static readonly DependencyProperty InnerRadiusProperty = DependencyProperty.Register("InnerRadius", typeof(double), typeof(OverlaySectorButtonUC), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty OuterRadiusProperty = DependencyProperty.Register("OuterRadius", typeof(double), typeof(OverlaySectorButtonUC), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty BigRadiusActiveProperty = DependencyProperty.Register("BigRadiusActive", typeof(double), typeof(OverlaySectorButtonUC), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty PosXProperty = DependencyProperty.Register("PosX", typeof(double), typeof(OverlaySectorButtonUC), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty PosYProperty = DependencyProperty.Register("PosY", typeof(double), typeof(OverlaySectorButtonUC), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

		private double _bigRadius;
	}
}
