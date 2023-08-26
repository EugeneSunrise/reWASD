using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using reWASDUI.Infrastructure.KeyBindings.XBBindingDirectionalGroups;

namespace reWASDUI.Views.ContentZoneGamepad.AdvancedStick.StickZone
{
	public partial class StickZones : BaseDirectionalAnalogGroupSquarePositioningControl
	{
		private static void LowZoneXChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			StickZones stickZones = d as StickZones;
			if (stickZones == null)
			{
				return;
			}
			stickZones.OnLowZoneXChanged();
		}

		public ushort LowZoneX
		{
			get
			{
				return (ushort)base.GetValue(StickZones.LowZoneXProperty);
			}
			set
			{
				base.SetValue(StickZones.LowZoneXProperty, value);
			}
		}

		public ushort MedZoneX
		{
			get
			{
				return (ushort)base.GetValue(StickZones.MedZoneXProperty);
			}
			set
			{
				base.SetValue(StickZones.MedZoneXProperty, value);
			}
		}

		private static void MedZoneXChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			StickZones stickZones = d as StickZones;
			if (stickZones == null)
			{
				return;
			}
			stickZones.OnMedZoneXChanged();
		}

		public ushort HighZoneX
		{
			get
			{
				return (ushort)base.GetValue(StickZones.HighZoneXProperty);
			}
			set
			{
				base.SetValue(StickZones.HighZoneXProperty, value);
			}
		}

		private static void HighZoneXChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			StickZones stickZones = d as StickZones;
			if (stickZones == null)
			{
				return;
			}
			stickZones.OnHighZoneXChanged();
		}

		private static void LowZoneYChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			StickZones stickZones = d as StickZones;
			if (stickZones == null)
			{
				return;
			}
			stickZones.OnLowZoneYChanged();
		}

		public ushort LowZoneY
		{
			get
			{
				return (ushort)base.GetValue(StickZones.LowZoneYProperty);
			}
			set
			{
				base.SetValue(StickZones.LowZoneYProperty, value);
			}
		}

		public ushort MedZoneY
		{
			get
			{
				return (ushort)base.GetValue(StickZones.MedZoneYProperty);
			}
			set
			{
				base.SetValue(StickZones.MedZoneYProperty, value);
			}
		}

		private static void MedZoneYChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			StickZones stickZones = d as StickZones;
			if (stickZones == null)
			{
				return;
			}
			stickZones.OnMedZoneYChanged();
		}

		public ushort HighZoneY
		{
			get
			{
				return (ushort)base.GetValue(StickZones.HighZoneYProperty);
			}
			set
			{
				base.SetValue(StickZones.HighZoneYProperty, value);
			}
		}

		private static void HighZoneYChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			StickZones stickZones = d as StickZones;
			if (stickZones == null)
			{
				return;
			}
			stickZones.OnHighZoneYChanged();
		}

		public StickZones()
		{
			this.InitializeComponent();
			base.Loaded += this.OnLoaded;
		}

		protected override void OnDirectionalGroupChanged(DependencyPropertyChangedEventArgs e)
		{
			this.AttachBindings();
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			if (this.isLoaded)
			{
				return;
			}
			this.isLoaded = true;
			this._borderHigh = (Border)base.Template.FindName("borderHigh", this);
			this._borderMed = (Border)base.Template.FindName("borderMed", this);
			this._borderLow = (Border)base.Template.FindName("borderLow", this);
			this.AttachBindings();
			this.RecalculateAllZoneBorderSizes();
		}

		private void OnLowZoneXChanged()
		{
			if (this._borderLow == null)
			{
				return;
			}
			double num = (double)this.LowZoneX / (double)this.MaxZoneValue * base.SquareHeightWidth;
			if (num < 1.0)
			{
				num = 0.0;
			}
			this._borderLow.Width = num;
		}

		private void OnMedZoneXChanged()
		{
			if (this._borderMed == null)
			{
				return;
			}
			double num = (double)this.MedZoneX / (double)this.MaxZoneValue * base.SquareHeightWidth;
			if (num < 1.0)
			{
				num = 0.0;
			}
			this._borderMed.Width = num;
		}

		private void OnHighZoneXChanged()
		{
			if (this._borderHigh == null)
			{
				return;
			}
			this._borderHigh.Width = (double)this.HighZoneX / (double)this.MaxZoneValue * base.SquareHeightWidth;
		}

		private void OnLowZoneYChanged()
		{
			if (this._borderLow == null)
			{
				return;
			}
			this._borderLow.Height = Math.Floor((double)this.LowZoneY / (double)this.MaxZoneValue * base.SquareHeightWidth);
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
			DependencyProperty lowZoneXProperty = StickZones.LowZoneXProperty;
			Binding binding = new Binding("XLow");
			BaseDirectionalAnalogGroup directionalGroup = base.DirectionalGroup;
			binding.Source = ((directionalGroup != null) ? directionalGroup.MainDirectionalAnalogGroup : null);
			BindingOperations.SetBinding(this, lowZoneXProperty, binding);
			DependencyProperty medZoneXProperty = StickZones.MedZoneXProperty;
			Binding binding2 = new Binding("XMed");
			BaseDirectionalAnalogGroup directionalGroup2 = base.DirectionalGroup;
			binding2.Source = ((directionalGroup2 != null) ? directionalGroup2.MainDirectionalAnalogGroup : null);
			BindingOperations.SetBinding(this, medZoneXProperty, binding2);
			DependencyProperty highZoneXProperty = StickZones.HighZoneXProperty;
			Binding binding3 = new Binding("XHigh");
			BaseDirectionalAnalogGroup directionalGroup3 = base.DirectionalGroup;
			binding3.Source = ((directionalGroup3 != null) ? directionalGroup3.MainDirectionalAnalogGroup : null);
			BindingOperations.SetBinding(this, highZoneXProperty, binding3);
			DependencyProperty lowZoneYProperty = StickZones.LowZoneYProperty;
			Binding binding4 = new Binding("YLow");
			BaseDirectionalAnalogGroup directionalGroup4 = base.DirectionalGroup;
			binding4.Source = ((directionalGroup4 != null) ? directionalGroup4.MainDirectionalAnalogGroup : null);
			BindingOperations.SetBinding(this, lowZoneYProperty, binding4);
			DependencyProperty medZoneYProperty = StickZones.MedZoneYProperty;
			Binding binding5 = new Binding("YMed");
			BaseDirectionalAnalogGroup directionalGroup5 = base.DirectionalGroup;
			binding5.Source = ((directionalGroup5 != null) ? directionalGroup5.MainDirectionalAnalogGroup : null);
			BindingOperations.SetBinding(this, medZoneYProperty, binding5);
			DependencyProperty highZoneYProperty = StickZones.HighZoneYProperty;
			Binding binding6 = new Binding("YHigh");
			BaseDirectionalAnalogGroup directionalGroup6 = base.DirectionalGroup;
			binding6.Source = ((directionalGroup6 != null) ? directionalGroup6.MainDirectionalAnalogGroup : null);
			BindingOperations.SetBinding(this, highZoneYProperty, binding6);
		}

		private void RecalculateAllZoneBorderSizes()
		{
			if (base.DirectionalGroup == null)
			{
				return;
			}
			this.OnLowZoneXChanged();
			this.OnMedZoneXChanged();
			this.OnHighZoneXChanged();
			this.OnLowZoneYChanged();
			this.OnMedZoneYChanged();
			this.OnHighZoneYChanged();
		}

		protected override void OnSizeChanged()
		{
			base.OnSizeChanged();
			this.RecalculateAllZoneBorderSizes();
		}

		public static readonly DependencyProperty LowZoneXProperty = DependencyProperty.Register("LowZoneX", typeof(ushort), typeof(StickZones), new PropertyMetadata(0, new PropertyChangedCallback(StickZones.LowZoneXChangedCallback)));

		public static readonly DependencyProperty MedZoneXProperty = DependencyProperty.Register("MedZoneX", typeof(ushort), typeof(StickZones), new PropertyMetadata(0, new PropertyChangedCallback(StickZones.MedZoneXChangedCallback)));

		public static readonly DependencyProperty HighZoneXProperty = DependencyProperty.Register("HighZoneX", typeof(ushort), typeof(StickZones), new PropertyMetadata(0, new PropertyChangedCallback(StickZones.HighZoneXChangedCallback)));

		public static readonly DependencyProperty LowZoneYProperty = DependencyProperty.Register("LowZoneY", typeof(ushort), typeof(StickZones), new PropertyMetadata(0, new PropertyChangedCallback(StickZones.LowZoneYChangedCallback)));

		public static readonly DependencyProperty MedZoneYProperty = DependencyProperty.Register("MedZoneY", typeof(ushort), typeof(StickZones), new PropertyMetadata(0, new PropertyChangedCallback(StickZones.MedZoneYChangedCallback)));

		public static readonly DependencyProperty HighZoneYProperty = DependencyProperty.Register("HighZoneY", typeof(ushort), typeof(StickZones), new PropertyMetadata(0, new PropertyChangedCallback(StickZones.HighZoneYChangedCallback)));

		private ushort MaxZoneValue = 32768;

		private Border _borderHigh;

		private Border _borderMed;

		private Border _borderLow;

		private bool isLoaded;
	}
}
