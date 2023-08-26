using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace reWASDUI.Controls.DeadZoneSlider
{
    public class ZoneMultiSlider : UserControl, IComponentConnector
    {
        public static readonly DependencyProperty SmallChangeProperty = DependencyProperty.Register("SmallChange", typeof(double), typeof(ZoneMultiSlider), new PropertyMetadata(0.0));

        public static readonly DependencyProperty LargeChangeProperty = DependencyProperty.Register("LargeChange", typeof(double), typeof(ZoneMultiSlider), new PropertyMetadata(0.0));

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(ZoneMultiSlider), new PropertyMetadata(0.0));

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(ZoneMultiSlider), new PropertyMetadata(0.0));

        public static readonly DependencyProperty LowZoneValueProperty = DependencyProperty.Register("LowZoneValue", typeof(double), typeof(ZoneMultiSlider), new PropertyMetadata(0.0, LowZoneValueChanged));

        public static readonly DependencyProperty LowZoneMinimumValueProperty = DependencyProperty.Register("LowZoneMinimumValue", typeof(double?), typeof(ZoneMultiSlider), new PropertyMetadata((object)null));

        public static readonly DependencyProperty LowZoneMaximumValueProperty = DependencyProperty.Register("LowZoneMaximumValue", typeof(double?), typeof(ZoneMultiSlider), new PropertyMetadata((object)null));

        public static readonly DependencyProperty LowZoneIsHitTestVisibleProperty = DependencyProperty.Register("LowZoneIsHitTestVisible", typeof(bool), typeof(ZoneMultiSlider), new PropertyMetadata(true));

        public static readonly DependencyProperty MedZoneValueProperty = DependencyProperty.Register("MedZoneValue", typeof(double), typeof(ZoneMultiSlider), new PropertyMetadata(0.0));

        public static readonly DependencyProperty MedZoneIsHitTestVisibleProperty = DependencyProperty.Register("MedZoneIsHitTestVisible", typeof(bool), typeof(ZoneMultiSlider), new PropertyMetadata(true));

        public static readonly DependencyProperty HighZoneValueProperty = DependencyProperty.Register("HighZoneValue", typeof(double), typeof(ZoneMultiSlider), new PropertyMetadata(0.0));

        public static readonly DependencyProperty HighZoneIsHitTestVisibleProperty = DependencyProperty.Register("HighZoneIsHitTestVisible", typeof(bool), typeof(ZoneMultiSlider), new PropertyMetadata(true));

        public static readonly DependencyProperty HighZoneVisibilityProperty = DependencyProperty.Register("HighZoneVisibility", typeof(Visibility), typeof(ZoneMultiSlider), new PropertyMetadata(Visibility.Visible));

        public static readonly DependencyProperty RightDeadZoneValueProperty = DependencyProperty.Register("RightDeadZoneValue", typeof(double), typeof(ZoneMultiSlider), new PropertyMetadata(0.0, RightDeadZoneValueChanged));

        public static readonly DependencyProperty RightDeadZoneMinimumValueProperty = DependencyProperty.Register("RightDeadZoneMinimumValue", typeof(double?), typeof(ZoneMultiSlider), new PropertyMetadata((object)null));

        public static readonly DependencyProperty RightDeadZoneIsHitTestVisibleProperty = DependencyProperty.Register("RightDeadZoneIsHitTestVisible", typeof(bool), typeof(ZoneMultiSlider), new PropertyMetadata(true));

        public static readonly DependencyProperty RightDeadZoneVisibilityProperty = DependencyProperty.Register("RightDeadZoneVisibility", typeof(Visibility), typeof(ZoneMultiSlider), new PropertyMetadata(Visibility.Visible));

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(ZoneMultiSlider), new PropertyMetadata((object)null));

        public static readonly DependencyProperty Zone1BrushProperty = DependencyProperty.Register("Zone1Brush", typeof(Brush), typeof(ZoneMultiSlider), new PropertyMetadata((object)null));

        public static readonly DependencyProperty Zone2BrushProperty = DependencyProperty.Register("Zone2Brush", typeof(Brush), typeof(ZoneMultiSlider), new PropertyMetadata((object)null));

        public static readonly DependencyProperty Zone3BrushProperty = DependencyProperty.Register("Zone3Brush", typeof(Brush), typeof(ZoneMultiSlider), new PropertyMetadata((object)null));

        public static readonly DependencyProperty Zone4BrushProperty = DependencyProperty.Register("Zone4Brush", typeof(Brush), typeof(ZoneMultiSlider), new PropertyMetadata((object)null));

        public static readonly DependencyProperty Zone5BrushProperty = DependencyProperty.Register("Zone5Brush", typeof(Brush), typeof(ZoneMultiSlider), new PropertyMetadata((object)null));

        public static readonly DependencyProperty ShowValueInTooltipProperty = DependencyProperty.Register("ShowValueInTooltip", typeof(bool), typeof(ZoneMultiSlider), new PropertyMetadata(false));

        public static readonly DependencyProperty ShowValueInAnnotationProperty = DependencyProperty.Register("ShowValueInAnnotation", typeof(bool), typeof(ZoneMultiSlider), new PropertyMetadata(false));

        private bool _contentLoaded;

        public double SmallChange
        {
            get
            {
                return (double)GetValue(SmallChangeProperty);
            }
            set
            {
                SetValue(SmallChangeProperty, value);
            }
        }

        public double LargeChange
        {
            get
            {
                return (double)GetValue(LargeChangeProperty);
            }
            set
            {
                SetValue(LargeChangeProperty, value);
            }
        }

        public double Minimum
        {
            get
            {
                return (double)GetValue(MinimumProperty);
            }
            set
            {
                SetValue(MinimumProperty, value);
            }
        }

        public double Maximum
        {
            get
            {
                return (double)GetValue(MaximumProperty);
            }
            set
            {
                SetValue(MaximumProperty, value);
            }
        }

        public double LowZoneValue
        {
            get
            {
                return (double)GetValue(LowZoneValueProperty);
            }
            set
            {
                SetValue(LowZoneValueProperty, value);
            }
        }

        public double? LowZoneMinimumValue
        {
            get
            {
                return (double?)GetValue(LowZoneMinimumValueProperty);
            }
            set
            {
                SetValue(LowZoneMinimumValueProperty, value);
            }
        }

        public double? LowZoneMaximumValue
        {
            get
            {
                return (double?)GetValue(LowZoneMaximumValueProperty);
            }
            set
            {
                SetValue(LowZoneMaximumValueProperty, value);
            }
        }

        public bool LowZoneIsHitTestVisible
        {
            get
            {
                return (bool)GetValue(LowZoneIsHitTestVisibleProperty);
            }
            set
            {
                SetValue(LowZoneIsHitTestVisibleProperty, value);
            }
        }

        public double MedZoneValue
        {
            get
            {
                return (double)GetValue(MedZoneValueProperty);
            }
            set
            {
                SetValue(MedZoneValueProperty, value);
            }
        }

        public bool MedZoneIsHitTestVisible
        {
            get
            {
                return (bool)GetValue(MedZoneIsHitTestVisibleProperty);
            }
            set
            {
                SetValue(MedZoneIsHitTestVisibleProperty, value);
            }
        }

        public double HighZoneValue
        {
            get
            {
                return (double)GetValue(HighZoneValueProperty);
            }
            set
            {
                SetValue(HighZoneValueProperty, value);
            }
        }

        public bool HighZoneIsHitTestVisible
        {
            get
            {
                return (bool)GetValue(HighZoneIsHitTestVisibleProperty);
            }
            set
            {
                SetValue(HighZoneIsHitTestVisibleProperty, value);
            }
        }

        public Visibility HighZoneVisibility
        {
            get
            {
                return (Visibility)GetValue(HighZoneVisibilityProperty);
            }
            set
            {
                SetValue(HighZoneVisibilityProperty, value);
            }
        }

        public double RightDeadZoneValue
        {
            get
            {
                return (double)GetValue(RightDeadZoneValueProperty);
            }
            set
            {
                SetValue(RightDeadZoneValueProperty, value);
            }
        }

        public double? RightDeadZoneMinimumValue
        {
            get
            {
                return (double?)GetValue(RightDeadZoneMinimumValueProperty);
            }
            set
            {
                SetValue(RightDeadZoneMinimumValueProperty, value);
            }
        }

        public bool RightDeadZoneIsHitTestVisible
        {
            get
            {
                return (bool)GetValue(RightDeadZoneIsHitTestVisibleProperty);
            }
            set
            {
                SetValue(RightDeadZoneIsHitTestVisibleProperty, value);
            }
        }

        public Visibility RightDeadZoneVisibility
        {
            get
            {
                return (Visibility)GetValue(RightDeadZoneVisibilityProperty);
            }
            set
            {
                SetValue(RightDeadZoneVisibilityProperty, value);
            }
        }

        public string Label
        {
            get
            {
                return (string)GetValue(LabelProperty);
            }
            set
            {
                SetValue(LabelProperty, value);
            }
        }

        public Brush Zone1Brush
        {
            get
            {
                return (Brush)GetValue(Zone1BrushProperty);
            }
            set
            {
                SetValue(Zone1BrushProperty, value);
            }
        }

        public Brush Zone2Brush
        {
            get
            {
                return (Brush)GetValue(Zone2BrushProperty);
            }
            set
            {
                SetValue(Zone2BrushProperty, value);
            }
        }

        public Brush Zone3Brush
        {
            get
            {
                return (Brush)GetValue(Zone3BrushProperty);
            }
            set
            {
                SetValue(Zone3BrushProperty, value);
            }
        }

        public Brush Zone4Brush
        {
            get
            {
                return (Brush)GetValue(Zone4BrushProperty);
            }
            set
            {
                SetValue(Zone4BrushProperty, value);
            }
        }

        public Brush Zone5Brush
        {
            get
            {
                return (Brush)GetValue(Zone5BrushProperty);
            }
            set
            {
                SetValue(Zone5BrushProperty, value);
            }
        }

        public bool ShowValueInTooltip
        {
            get
            {
                return (bool)GetValue(ShowValueInTooltipProperty);
            }
            set
            {
                SetValue(ShowValueInTooltipProperty, value);
            }
        }

        public bool ShowValueInAnnotation
        {
            get
            {
                return (bool)GetValue(ShowValueInAnnotationProperty);
            }
            set
            {
                SetValue(ShowValueInAnnotationProperty, value);
            }
        }

        public ZoneMultiSlider()
        {
            InitializeComponent();
        }

        private static void LowZoneValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ZoneMultiSlider zoneMultiSlider = d as ZoneMultiSlider;
            if (zoneMultiSlider != null)
            {
                if (zoneMultiSlider.LowZoneMaximumValue.HasValue && zoneMultiSlider.LowZoneMaximumValue < zoneMultiSlider.LowZoneValue)
                {
                    zoneMultiSlider.LowZoneValue = (int)(ushort)zoneMultiSlider.LowZoneMaximumValue.Value;
                }
                else if (zoneMultiSlider.LowZoneMinimumValue.HasValue && zoneMultiSlider.LowZoneMinimumValue > zoneMultiSlider.LowZoneValue)
                {
                    zoneMultiSlider.LowZoneValue = (int)(ushort)zoneMultiSlider.LowZoneMinimumValue.Value;
                }
                else if (zoneMultiSlider.RightDeadZoneValue > zoneMultiSlider.RightDeadZoneMinimumValue && zoneMultiSlider.LowZoneValue > zoneMultiSlider.RightDeadZoneValue)
                {
                    zoneMultiSlider.LowZoneValue = (int)(ushort)zoneMultiSlider.RightDeadZoneValue;
                }
            }
        }

        private static void RightDeadZoneValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ZoneMultiSlider zoneMultiSlider = d as ZoneMultiSlider;
            if (zoneMultiSlider != null)
            {
                if (zoneMultiSlider.RightDeadZoneMinimumValue.HasValue && zoneMultiSlider.RightDeadZoneMinimumValue > zoneMultiSlider.RightDeadZoneValue)
                {
                    zoneMultiSlider.RightDeadZoneValue = (int)(ushort)zoneMultiSlider.RightDeadZoneMinimumValue.Value;
                }
                else if (zoneMultiSlider.LowZoneValue > zoneMultiSlider.LowZoneMinimumValue && zoneMultiSlider.RightDeadZoneValue < zoneMultiSlider.LowZoneValue)
                {
                    zoneMultiSlider.RightDeadZoneValue = (int)(ushort)zoneMultiSlider.LowZoneValue;
                }
            }
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
        public void InitializeComponent()
        {
            if (!_contentLoaded)
            {
                _contentLoaded = true;
                Uri resourceLocator = new Uri("/reWASD;component/controls/deadzoneslider/zonemultislider.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            _contentLoaded = true;
        }
    }
}
