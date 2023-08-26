using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace reWASDUI.Controls
{
	public partial class SteamDeckVibrationIntensity : UserControl
	{
		public string Header
		{
			get
			{
				return (string)base.GetValue(SteamDeckVibrationIntensity.HeaderProperty);
			}
			set
			{
				base.SetValue(SteamDeckVibrationIntensity.HeaderProperty, value);
			}
		}

		public bool IsStrongIntensity
		{
			get
			{
				return (bool)base.GetValue(SteamDeckVibrationIntensity.IsStrongIntensityProperty);
			}
			set
			{
				base.SetValue(SteamDeckVibrationIntensity.IsStrongIntensityProperty, value);
			}
		}

		public double Intensity
		{
			get
			{
				return (double)base.GetValue(SteamDeckVibrationIntensity.IntensityProperty);
			}
			set
			{
				base.SetValue(SteamDeckVibrationIntensity.IntensityProperty, value);
			}
		}

		public SteamDeckVibrationIntensity()
		{
			this.InitializeComponent();
		}

		public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(SteamDeckVibrationIntensity), new PropertyMetadata(null));

		public static readonly DependencyProperty IsStrongIntensityProperty = DependencyProperty.Register("IsStrongIntensity", typeof(bool), typeof(SteamDeckVibrationIntensity), new PropertyMetadata(false));

		public static readonly DependencyProperty IntensityProperty = DependencyProperty.Register("Intensity", typeof(double), typeof(SteamDeckVibrationIntensity), new PropertyMetadata(0.0));
	}
}
