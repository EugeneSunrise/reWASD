using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Shapes;
using DiscSoft.NET.Common.View.Controls;
using XBEliteWPF.Infrastructure;

namespace reWASDUI.Views.ContentZoneGamepad.AdvancedStick
{
	public partial class VirtualStickZones : SquarePositioningControl
	{
		public VirtualStickSettings VirtualStickSettings
		{
			get
			{
				return (VirtualStickSettings)base.GetValue(VirtualStickZones.VirtualStickSettingsProperty);
			}
			set
			{
				base.SetValue(VirtualStickZones.VirtualStickSettingsProperty, value);
			}
		}

		private static void VirtualStickSettingsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VirtualStickZones virtualStickZones = d as VirtualStickZones;
			if (virtualStickZones == null)
			{
				return;
			}
			virtualStickZones.OnVirtualStickSettingsChanged(e);
		}

		private void OnVirtualStickSettingsChanged(DependencyPropertyChangedEventArgs e)
		{
			this.AttachBindings();
		}

		private static void MaxValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VirtualStickZones virtualStickZones = d as VirtualStickZones;
			if (virtualStickZones == null)
			{
				return;
			}
			virtualStickZones.RecalculateAllNormalizedValues();
		}

		public double MaxValue
		{
			get
			{
				return (double)base.GetValue(VirtualStickZones.MaxValueProperty);
			}
			set
			{
				base.SetValue(VirtualStickZones.MaxValueProperty, value);
			}
		}

		public double DeflectionSquarenessLow
		{
			get
			{
				return (double)base.GetValue(VirtualStickZones.DeflectionSquarenessLowProperty);
			}
			set
			{
				base.SetValue(VirtualStickZones.DeflectionSquarenessLowProperty, value);
			}
		}

		public double DeflectionSquarenessMed
		{
			get
			{
				return (double)base.GetValue(VirtualStickZones.DeflectionSquarenessMedProperty);
			}
			set
			{
				base.SetValue(VirtualStickZones.DeflectionSquarenessMedProperty, value);
			}
		}

		public double DeflectionSquarenessHigh
		{
			get
			{
				return (double)base.GetValue(VirtualStickZones.DeflectionSquarenessHighProperty);
			}
			set
			{
				base.SetValue(VirtualStickZones.DeflectionSquarenessHighProperty, value);
			}
		}

		public double NormalizedY1
		{
			get
			{
				return (double)base.GetValue(VirtualStickZones.NormalizedY1Property);
			}
			set
			{
				base.SetValue(VirtualStickZones.NormalizedY1Property, value);
			}
		}

		public double NormalizedY2
		{
			get
			{
				return (double)base.GetValue(VirtualStickZones.NormalizedY2Property);
			}
			set
			{
				base.SetValue(VirtualStickZones.NormalizedY2Property, value);
			}
		}

		public double NormalizedY3
		{
			get
			{
				return (double)base.GetValue(VirtualStickZones.NormalizedY3Property);
			}
			set
			{
				base.SetValue(VirtualStickZones.NormalizedY3Property, value);
			}
		}

		public double NormalizedY4
		{
			get
			{
				return (double)base.GetValue(VirtualStickZones.NormalizedY4Property);
			}
			set
			{
				base.SetValue(VirtualStickZones.NormalizedY4Property, value);
			}
		}

		public ushort X1
		{
			get
			{
				return (ushort)base.GetValue(VirtualStickZones.X1Property);
			}
			set
			{
				base.SetValue(VirtualStickZones.X1Property, value);
			}
		}

		private static void X1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VirtualStickZones virtualStickZones = d as VirtualStickZones;
			if (virtualStickZones == null)
			{
				return;
			}
			virtualStickZones.OnX1Changed();
		}

		public ushort Y1
		{
			get
			{
				return (ushort)base.GetValue(VirtualStickZones.Y1Property);
			}
			set
			{
				base.SetValue(VirtualStickZones.Y1Property, value);
			}
		}

		private static void Y1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VirtualStickZones virtualStickZones = d as VirtualStickZones;
			if (virtualStickZones == null)
			{
				return;
			}
			virtualStickZones.OnY1Changed();
		}

		public ushort X2
		{
			get
			{
				return (ushort)base.GetValue(VirtualStickZones.X2Property);
			}
			set
			{
				base.SetValue(VirtualStickZones.X2Property, value);
			}
		}

		private static void X2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VirtualStickZones virtualStickZones = d as VirtualStickZones;
			if (virtualStickZones == null)
			{
				return;
			}
			virtualStickZones.OnX2Changed();
		}

		public ushort Y2
		{
			get
			{
				return (ushort)base.GetValue(VirtualStickZones.Y2Property);
			}
			set
			{
				base.SetValue(VirtualStickZones.Y2Property, value);
			}
		}

		private static void Y2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VirtualStickZones virtualStickZones = d as VirtualStickZones;
			if (virtualStickZones == null)
			{
				return;
			}
			virtualStickZones.OnY2Changed();
		}

		public ushort X3
		{
			get
			{
				return (ushort)base.GetValue(VirtualStickZones.X3Property);
			}
			set
			{
				base.SetValue(VirtualStickZones.X3Property, value);
			}
		}

		private static void X3Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VirtualStickZones virtualStickZones = d as VirtualStickZones;
			if (virtualStickZones == null)
			{
				return;
			}
			virtualStickZones.OnX3Changed();
		}

		public ushort Y3
		{
			get
			{
				return (ushort)base.GetValue(VirtualStickZones.Y3Property);
			}
			set
			{
				base.SetValue(VirtualStickZones.Y3Property, value);
			}
		}

		private static void Y3Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VirtualStickZones virtualStickZones = d as VirtualStickZones;
			if (virtualStickZones == null)
			{
				return;
			}
			virtualStickZones.OnY3Changed();
		}

		public ushort X4
		{
			get
			{
				return (ushort)base.GetValue(VirtualStickZones.X4Property);
			}
			set
			{
				base.SetValue(VirtualStickZones.X4Property, value);
			}
		}

		private static void X4Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VirtualStickZones virtualStickZones = d as VirtualStickZones;
			if (virtualStickZones == null)
			{
				return;
			}
			virtualStickZones.OnX4Changed();
		}

		public ushort Y4
		{
			get
			{
				return (ushort)base.GetValue(VirtualStickZones.Y4Property);
			}
			set
			{
				base.SetValue(VirtualStickZones.Y4Property, value);
			}
		}

		private static void Y4Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VirtualStickZones virtualStickZones = d as VirtualStickZones;
			if (virtualStickZones == null)
			{
				return;
			}
			virtualStickZones.OnY4Changed();
		}

		private static void LowZoneXChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VirtualStickZones virtualStickZones = d as VirtualStickZones;
			if (virtualStickZones == null)
			{
				return;
			}
			virtualStickZones.OnLowZoneXChanged();
		}

		public ushort LowZoneX
		{
			get
			{
				return (ushort)base.GetValue(VirtualStickZones.LowZoneXProperty);
			}
			set
			{
				base.SetValue(VirtualStickZones.LowZoneXProperty, value);
			}
		}

		public ushort MedZoneX
		{
			get
			{
				return (ushort)base.GetValue(VirtualStickZones.MedZoneXProperty);
			}
			set
			{
				base.SetValue(VirtualStickZones.MedZoneXProperty, value);
			}
		}

		private static void MedZoneXChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VirtualStickZones virtualStickZones = d as VirtualStickZones;
			if (virtualStickZones == null)
			{
				return;
			}
			virtualStickZones.OnMedZoneXChanged();
		}

		public ushort HighZoneX
		{
			get
			{
				return (ushort)base.GetValue(VirtualStickZones.HighZoneXProperty);
			}
			set
			{
				base.SetValue(VirtualStickZones.HighZoneXProperty, value);
			}
		}

		private static void HighZoneXChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VirtualStickZones virtualStickZones = d as VirtualStickZones;
			if (virtualStickZones == null)
			{
				return;
			}
			virtualStickZones.OnHighZoneXChanged();
		}

		private static void LowZoneYChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VirtualStickZones virtualStickZones = d as VirtualStickZones;
			if (virtualStickZones == null)
			{
				return;
			}
			virtualStickZones.OnLowZoneYChanged();
		}

		public ushort LowZoneY
		{
			get
			{
				return (ushort)base.GetValue(VirtualStickZones.LowZoneYProperty);
			}
			set
			{
				base.SetValue(VirtualStickZones.LowZoneYProperty, value);
			}
		}

		public ushort MedZoneY
		{
			get
			{
				return (ushort)base.GetValue(VirtualStickZones.MedZoneYProperty);
			}
			set
			{
				base.SetValue(VirtualStickZones.MedZoneYProperty, value);
			}
		}

		private static void MedZoneYChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VirtualStickZones virtualStickZones = d as VirtualStickZones;
			if (virtualStickZones == null)
			{
				return;
			}
			virtualStickZones.OnMedZoneYChanged();
		}

		public ushort HighZoneY
		{
			get
			{
				return (ushort)base.GetValue(VirtualStickZones.HighZoneYProperty);
			}
			set
			{
				base.SetValue(VirtualStickZones.HighZoneYProperty, value);
			}
		}

		private static void HighZoneYChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VirtualStickZones virtualStickZones = d as VirtualStickZones;
			if (virtualStickZones == null)
			{
				return;
			}
			virtualStickZones.OnHighZoneYChanged();
		}

		public VirtualStickZones()
		{
			this.InitializeComponent();
			base.Loaded += this.OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			if (this.isLoaded)
			{
				return;
			}
			this.isLoaded = true;
			this._borderHigh = (Rectangle)base.Template.FindName("borderHigh", this);
			this._borderMed = (Rectangle)base.Template.FindName("borderMed", this);
			this._borderLow = (Rectangle)base.Template.FindName("borderLow", this);
			this._squareRadius1 = (Rectangle)base.Template.FindName("squareRadius1", this);
			this._squareRadius2 = (Rectangle)base.Template.FindName("squareRadius2", this);
			this._squareRadius3 = (Rectangle)base.Template.FindName("squareRadius3", this);
			this._squareRadius4 = (Rectangle)base.Template.FindName("squareRadius4", this);
			this.AttachBindings();
			this.RecalculateAllZoneBorderSizes();
		}

		private double GetSqarnessByRadius(ushort radius)
		{
			double num = 0.0;
			ushort num2 = 0;
			VirtualStickSettings virtualStickSettings = this.VirtualStickSettings;
			if (((virtualStickSettings != null) ? virtualStickSettings.SquareSettings : null) == null)
			{
				return 0.0;
			}
			for (int i = 0; i < this.VirtualStickSettings.SquareSettings.HorizontalPoint.Length + 1; i++)
			{
				double num3 = this.MaxValue;
				ushort num4 = (ushort)this.MaxValue;
				if (i < this.VirtualStickSettings.SquareSettings.HorizontalPoint.Length)
				{
					num3 = (double)this.VirtualStickSettings.SquareSettings.HorizontalPoint[i].NewValue;
					num4 = this.VirtualStickSettings.SquareSettings.HorizontalPoint[i].TravelDistance;
				}
				if (radius == num2)
				{
					return num;
				}
				if (radius >= num2 && radius < num4)
				{
					return num + (num3 - num) * (double)(radius - num2) / (double)(num4 - num2);
				}
				num2 = num4;
				num = num3;
			}
			return this.MaxValue;
		}

		private void RecalculateDeflactionSquarenessLow()
		{
			this.DeflectionSquarenessLow = 100.0 - this.GetSqarnessByRadius(this.LowZoneX) / this.MaxValue * 100.0;
		}

		private void RecalculateDeflactionSquarenessMed()
		{
			this.DeflectionSquarenessMed = 100.0 - this.GetSqarnessByRadius(this.MedZoneX) / this.MaxValue * 100.0;
		}

		private void RecalculateDeflactionSquarenessHigh()
		{
			this.DeflectionSquarenessHigh = 100.0 - this.GetSqarnessByRadius(32767) / this.MaxValue * 100.0;
		}

		private void RecalculateDeflactionSquarenessAll()
		{
			this.RecalculateDeflactionSquarenessLow();
			this.RecalculateDeflactionSquarenessMed();
			this.RecalculateDeflactionSquarenessHigh();
		}

		private void OnLowZoneXChanged()
		{
			if (this._borderLow == null)
			{
				return;
			}
			this._borderLow.Width = (double)this.LowZoneX / (double)this.MaxZoneValue * base.SquareHeightWidth;
			this.RecalculateDeflactionSquarenessLow();
		}

		private void OnMedZoneXChanged()
		{
			if (this._borderMed == null)
			{
				return;
			}
			this._borderMed.Width = (double)this.MedZoneX / (double)this.MaxZoneValue * base.SquareHeightWidth;
			this.RecalculateDeflactionSquarenessMed();
		}

		private void OnHighZoneXChanged()
		{
			if (this._borderHigh == null)
			{
				return;
			}
			this._borderHigh.Width = (double)this.HighZoneX / (double)this.MaxZoneValue * base.SquareHeightWidth;
			this.RecalculateDeflactionSquarenessHigh();
		}

		private void OnLowZoneYChanged()
		{
			if (this._borderLow == null)
			{
				return;
			}
			this._borderLow.Height = (double)this.LowZoneY / (double)this.MaxZoneValue * base.SquareHeightWidth;
		}

		private void OnMedZoneYChanged()
		{
			if (this._borderMed == null)
			{
				return;
			}
			this._borderMed.Height = (double)this.MedZoneY / (double)this.MaxZoneValue * base.SquareHeightWidth;
		}

		private void OnHighZoneYChanged()
		{
			if (this._borderHigh == null)
			{
				return;
			}
			this._borderHigh.Height = (double)this.HighZoneY / (double)this.MaxZoneValue * base.SquareHeightWidth;
		}

		private void AttachBindings()
		{
			BindingOperations.SetBinding(this, VirtualStickZones.LowZoneXProperty, new Binding("InitialDeflection")
			{
				Source = this.VirtualStickSettings
			});
			BindingOperations.SetBinding(this, VirtualStickZones.MedZoneXProperty, new Binding("MaxDeflection")
			{
				Source = this.VirtualStickSettings
			});
			BindingOperations.SetBinding(this, VirtualStickZones.LowZoneYProperty, new Binding("InitialDeflectionY")
			{
				Source = this.VirtualStickSettings
			});
			BindingOperations.SetBinding(this, VirtualStickZones.MedZoneYProperty, new Binding("MaxDeflection")
			{
				Source = this.VirtualStickSettings
			});
			BindingOperations.SetBinding(this, VirtualStickZones.X4Property, new Binding("SquareSettings.HorizontalPoint[3].TravelDistance")
			{
				Source = this.VirtualStickSettings,
				Mode = BindingMode.TwoWay
			});
			BindingOperations.SetBinding(this, VirtualStickZones.X3Property, new Binding("SquareSettings.HorizontalPoint[2].TravelDistance")
			{
				Source = this.VirtualStickSettings,
				Mode = BindingMode.TwoWay
			});
			BindingOperations.SetBinding(this, VirtualStickZones.X2Property, new Binding("SquareSettings.HorizontalPoint[1].TravelDistance")
			{
				Source = this.VirtualStickSettings,
				Mode = BindingMode.TwoWay
			});
			BindingOperations.SetBinding(this, VirtualStickZones.X1Property, new Binding("SquareSettings.HorizontalPoint[0].TravelDistance")
			{
				Source = this.VirtualStickSettings,
				Mode = BindingMode.TwoWay
			});
			BindingOperations.SetBinding(this, VirtualStickZones.Y4Property, new Binding("SquareSettings.HorizontalPoint[3].NewValue")
			{
				Source = this.VirtualStickSettings,
				Mode = BindingMode.TwoWay
			});
			BindingOperations.SetBinding(this, VirtualStickZones.Y3Property, new Binding("SquareSettings.HorizontalPoint[2].NewValue")
			{
				Source = this.VirtualStickSettings,
				Mode = BindingMode.TwoWay
			});
			BindingOperations.SetBinding(this, VirtualStickZones.Y2Property, new Binding("SquareSettings.HorizontalPoint[1].NewValue")
			{
				Source = this.VirtualStickSettings,
				Mode = BindingMode.TwoWay
			});
			BindingOperations.SetBinding(this, VirtualStickZones.Y1Property, new Binding("SquareSettings.HorizontalPoint[0].NewValue")
			{
				Source = this.VirtualStickSettings,
				Mode = BindingMode.TwoWay
			});
		}

		private void RecalculateAllZoneBorderSizes()
		{
			if (this.VirtualStickSettings == null)
			{
				return;
			}
			this.OnLowZoneXChanged();
			this.OnMedZoneXChanged();
			this.OnHighZoneXChanged();
			this.OnLowZoneYChanged();
			this.OnMedZoneYChanged();
			this.OnHighZoneYChanged();
			this.RecalculateAllNormalizedValues();
		}

		protected override void OnSizeChanged()
		{
			base.OnSizeChanged();
			this.RecalculateAllZoneBorderSizes();
			this.RecalculateAllNormalizedValues();
		}

		private void OnX1Changed()
		{
			bool flag = Math.Abs((int)(this._prevX1 - this.X1)) > 10;
			if (flag || (this._prevX1 != this.X1 && this.X1 >= this.X2))
			{
				this._prevX1 = this.X1;
			}
			if (flag)
			{
				this.RecalculateNormalizedX1();
			}
		}

		private void OnX2Changed()
		{
			bool flag = Math.Abs((int)(this._prevX2 - this.X2)) > 10;
			if (flag || (this._prevX2 != this.X2 && (this.X2 >= this.X1 || this.X2 >= this.X3)))
			{
				this._prevX2 = this.X2;
			}
			if (flag)
			{
				this.RecalculateNormalizedX2();
			}
		}

		private void OnX3Changed()
		{
			bool flag = Math.Abs((int)(this._prevX3 - this.X3)) > 10;
			if (flag || (this._prevX3 != this.X3 && (this.X3 >= this.X2 || this.X3 >= this.X4)))
			{
				this._prevX3 = this.X3;
			}
			if (flag)
			{
				this.RecalculateNormalizedX3();
			}
		}

		private void OnX4Changed()
		{
			bool flag = Math.Abs((int)(this._prevX4 - this.X4)) > 10;
			if (flag || (this._prevX4 != this.X4 && this.X4 >= this.X3))
			{
				this._prevX4 = this.X4;
			}
			if (flag)
			{
				this.RecalculateNormalizedX4();
			}
		}

		private void OnY1Changed()
		{
			bool flag = Math.Abs((int)(this._prevY1 - this.Y1)) > 10;
			if (flag || (this._prevY1 != this.Y1 && this.Y1 >= this.Y2))
			{
				this._prevY1 = this.Y1;
			}
			if (flag)
			{
				this.RecalculateNormalizedY1();
			}
		}

		private void OnY2Changed()
		{
			bool flag = Math.Abs((int)(this._prevY2 - this.Y2)) > 10;
			if (flag || (this._prevY2 != this.Y2 && (this.Y2 >= this.Y1 || this.Y2 >= this.Y3)))
			{
				this._prevY2 = this.Y2;
			}
			if (flag)
			{
				this.RecalculateNormalizedY2();
			}
		}

		private void OnY3Changed()
		{
			bool flag = Math.Abs((int)(this._prevY3 - this.Y3)) > 10;
			if (flag || (this._prevY3 != this.Y3 && (this.Y3 >= this.Y2 || this.Y3 >= this.Y4)))
			{
				this._prevY3 = this.Y3;
			}
			if (flag)
			{
				this.RecalculateNormalizedY3();
			}
		}

		private void OnY4Changed()
		{
			bool flag = Math.Abs((int)(this._prevY4 - this.Y4)) > 10;
			if (flag || (this._prevY4 != this.Y4 && this.Y4 >= this.Y3))
			{
				this._prevY4 = this.Y4;
			}
			if (flag)
			{
				this.RecalculateNormalizedY4();
			}
		}

		private void RecalculateNormalizedX1()
		{
			if (this.MaxValue == 0.0 || this._squareRadius1 == null)
			{
				return;
			}
			this._squareRadius1.Width = (double)this.X1 / this.MaxValue * base.SquareHeightWidth;
			this._squareRadius1.Height = this._squareRadius1.Width;
			this.RecalculateDeflactionSquarenessAll();
		}

		private void RecalculateNormalizedX2()
		{
			if (this.MaxValue == 0.0 || this._squareRadius1 == null)
			{
				return;
			}
			this._squareRadius2.Width = (double)this.X2 / this.MaxValue * base.SquareHeightWidth;
			this._squareRadius2.Height = this._squareRadius2.Width;
			this.RecalculateDeflactionSquarenessAll();
		}

		private void RecalculateNormalizedX3()
		{
			if (this.MaxValue == 0.0 || this._squareRadius1 == null)
			{
				return;
			}
			this._squareRadius3.Width = (double)this.X3 / this.MaxValue * base.SquareHeightWidth;
			this._squareRadius3.Height = this._squareRadius3.Width;
			this.RecalculateDeflactionSquarenessAll();
		}

		private void RecalculateNormalizedX4()
		{
			if (this.MaxValue == 0.0 || this._squareRadius1 == null)
			{
				return;
			}
			this._squareRadius4.Width = (double)this.X4 / this.MaxValue * base.SquareHeightWidth;
			this._squareRadius4.Height = this._squareRadius4.Width;
			this.RecalculateDeflactionSquarenessAll();
		}

		private void RecalculateNormalizedY1()
		{
			if (this.MaxValue == 0.0 || this._squareRadius1 == null)
			{
				return;
			}
			this.NormalizedY1 = 100.0 - (double)this.Y1 / this.MaxValue * 100.0;
			this.RecalculateDeflactionSquarenessAll();
		}

		private void RecalculateNormalizedY2()
		{
			if (this.MaxValue == 0.0 || this._squareRadius1 == null)
			{
				return;
			}
			this.NormalizedY2 = 100.0 - (double)this.Y2 / this.MaxValue * 100.0;
			this.RecalculateDeflactionSquarenessAll();
		}

		private void RecalculateNormalizedY3()
		{
			if (this.MaxValue == 0.0 || this._squareRadius1 == null)
			{
				return;
			}
			this.NormalizedY3 = 100.0 - (double)this.Y3 / this.MaxValue * 100.0;
			this.RecalculateDeflactionSquarenessAll();
		}

		private void RecalculateNormalizedY4()
		{
			if (this.MaxValue == 0.0 || this._squareRadius1 == null)
			{
				return;
			}
			this.NormalizedY4 = 100.0 - (double)this.Y4 / this.MaxValue * 100.0;
			this.RecalculateDeflactionSquarenessAll();
		}

		private void RecalculateNormalizedXCoordinates()
		{
			this.RecalculateNormalizedX1();
			this.RecalculateNormalizedX2();
			this.RecalculateNormalizedX3();
			this.RecalculateNormalizedX4();
		}

		private void RecalculateNormalizedYCoordinates()
		{
			this.RecalculateNormalizedY1();
			this.RecalculateNormalizedY2();
			this.RecalculateNormalizedY3();
			this.RecalculateNormalizedY4();
		}

		private void RecalculateAllNormalizedValues()
		{
			this.RecalculateNormalizedXCoordinates();
			this.RecalculateNormalizedYCoordinates();
		}

		private const int YDeltaToInitiateNormalizedRecalculate = 10;

		private const int XDeltaToInitiateNormalizedRecalculate = 10;

		public static readonly DependencyProperty VirtualStickSettingsProperty = DependencyProperty.Register("VirtualStickSettings", typeof(VirtualStickSettings), typeof(VirtualStickZones), new PropertyMetadata(null, new PropertyChangedCallback(VirtualStickZones.VirtualStickSettingsChangedCallback)));

		public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(VirtualStickZones), new PropertyMetadata(0.0, new PropertyChangedCallback(VirtualStickZones.MaxValueChanged)));

		public static readonly DependencyProperty DeflectionSquarenessLowProperty = DependencyProperty.Register("DeflectionSquarenessLow", typeof(double), typeof(VirtualStickZones), new PropertyMetadata(0.0));

		public static readonly DependencyProperty DeflectionSquarenessMedProperty = DependencyProperty.Register("DeflectionSquarenessMed", typeof(double), typeof(VirtualStickZones), new PropertyMetadata(0.0));

		public static readonly DependencyProperty DeflectionSquarenessHighProperty = DependencyProperty.Register("DeflectionSquarenessHigh", typeof(double), typeof(VirtualStickZones), new PropertyMetadata(0.0));

		public static readonly DependencyProperty NormalizedY1Property = DependencyProperty.Register("NormalizedY1", typeof(double), typeof(VirtualStickZones), new PropertyMetadata(0.0));

		public static readonly DependencyProperty NormalizedY2Property = DependencyProperty.Register("NormalizedY2", typeof(double), typeof(VirtualStickZones), new PropertyMetadata(0.0));

		public static readonly DependencyProperty NormalizedY3Property = DependencyProperty.Register("NormalizedY3", typeof(double), typeof(VirtualStickZones), new PropertyMetadata(0.0));

		public static readonly DependencyProperty NormalizedY4Property = DependencyProperty.Register("NormalizedY4", typeof(double), typeof(VirtualStickZones), new PropertyMetadata(0.0));

		public static readonly DependencyProperty X1Property = DependencyProperty.Register("X1", typeof(ushort), typeof(VirtualStickZones), new PropertyMetadata(0, new PropertyChangedCallback(VirtualStickZones.X1Changed)));

		private ushort _prevX1;

		public static readonly DependencyProperty Y1Property = DependencyProperty.Register("Y1", typeof(ushort), typeof(VirtualStickZones), new PropertyMetadata(0, new PropertyChangedCallback(VirtualStickZones.Y1Changed)));

		private ushort _prevY1;

		public static readonly DependencyProperty X2Property = DependencyProperty.Register("X2", typeof(ushort), typeof(VirtualStickZones), new PropertyMetadata(0, new PropertyChangedCallback(VirtualStickZones.X2Changed)));

		private ushort _prevX2;

		public static readonly DependencyProperty Y2Property = DependencyProperty.Register("Y2", typeof(ushort), typeof(VirtualStickZones), new PropertyMetadata(0, new PropertyChangedCallback(VirtualStickZones.Y2Changed)));

		private ushort _prevY2;

		public static readonly DependencyProperty X3Property = DependencyProperty.Register("X3", typeof(ushort), typeof(VirtualStickZones), new PropertyMetadata(0, new PropertyChangedCallback(VirtualStickZones.X3Changed)));

		private ushort _prevX3;

		public static readonly DependencyProperty Y3Property = DependencyProperty.Register("Y3", typeof(ushort), typeof(VirtualStickZones), new PropertyMetadata(0, new PropertyChangedCallback(VirtualStickZones.Y3Changed)));

		private ushort _prevY3;

		public static readonly DependencyProperty X4Property = DependencyProperty.Register("X4", typeof(ushort), typeof(VirtualStickZones), new PropertyMetadata(0, new PropertyChangedCallback(VirtualStickZones.X4Changed)));

		private ushort _prevX4;

		public static readonly DependencyProperty Y4Property = DependencyProperty.Register("Y4", typeof(ushort), typeof(VirtualStickZones), new PropertyMetadata(0, new PropertyChangedCallback(VirtualStickZones.Y4Changed)));

		private ushort _prevY4;

		public static readonly DependencyProperty LowZoneXProperty = DependencyProperty.Register("LowZoneX", typeof(ushort), typeof(VirtualStickZones), new PropertyMetadata(0, new PropertyChangedCallback(VirtualStickZones.LowZoneXChangedCallback)));

		public static readonly DependencyProperty MedZoneXProperty = DependencyProperty.Register("MedZoneX", typeof(ushort), typeof(VirtualStickZones), new PropertyMetadata(0, new PropertyChangedCallback(VirtualStickZones.MedZoneXChangedCallback)));

		public static readonly DependencyProperty HighZoneXProperty = DependencyProperty.Register("HighZoneX", typeof(ushort), typeof(VirtualStickZones), new PropertyMetadata(0, new PropertyChangedCallback(VirtualStickZones.HighZoneXChangedCallback)));

		public static readonly DependencyProperty LowZoneYProperty = DependencyProperty.Register("LowZoneY", typeof(ushort), typeof(VirtualStickZones), new PropertyMetadata(0, new PropertyChangedCallback(VirtualStickZones.LowZoneYChangedCallback)));

		public static readonly DependencyProperty MedZoneYProperty = DependencyProperty.Register("MedZoneY", typeof(ushort), typeof(VirtualStickZones), new PropertyMetadata(0, new PropertyChangedCallback(VirtualStickZones.MedZoneYChangedCallback)));

		public static readonly DependencyProperty HighZoneYProperty = DependencyProperty.Register("HighZoneY", typeof(ushort), typeof(VirtualStickZones), new PropertyMetadata(0, new PropertyChangedCallback(VirtualStickZones.HighZoneYChangedCallback)));

		private ushort MaxZoneValue = 32768;

		private Rectangle _borderHigh;

		private Rectangle _borderMed;

		private Rectangle _borderLow;

		private Rectangle _squareRadius1;

		private Rectangle _squareRadius2;

		private Rectangle _squareRadius3;

		private Rectangle _squareRadius4;

		private bool isLoaded;
	}
}
