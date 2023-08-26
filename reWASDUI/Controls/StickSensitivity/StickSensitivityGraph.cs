using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.View.Controls.Buttons;
using reWASDUI.Infrastructure;
using XBEliteWPF.Utils.Wrappers;

namespace reWASDUI.Controls.StickSensitivity
{
    public class StickSensitivityGraph : UserControl, IComponentConnector
    {
        private const int YDeltaToInitiateNormalizedRecalculate = 10;

        private const int XDeltaToInitiateNormalizedRecalculate = 10;

        public static readonly DependencyProperty DoUpdateDeflactionZeroPropertiesProperty = DependencyProperty.Register("DoUpdateDeflactionZeroProperties", typeof(bool), typeof(StickSensitivityGraph), new PropertyMetadata(true));

        public static readonly DependencyProperty EnableValueCoerceProperty = DependencyProperty.Register("EnableValueCoerce", typeof(bool), typeof(StickSensitivityGraph), new PropertyMetadata(true));

        public static readonly DependencyProperty X1Property = DependencyProperty.Register("X1", typeof(ushort), typeof(StickSensitivityGraph), new PropertyMetadata((ushort)0, X1Changed));

        private ushort _prevX1;

        public static readonly DependencyProperty Y1Property = DependencyProperty.Register("Y1", typeof(ushort), typeof(StickSensitivityGraph), new PropertyMetadata((ushort)0, Y1Changed));

        private ushort _prevY1;

        public static readonly DependencyProperty X2Property = DependencyProperty.Register("X2", typeof(ushort), typeof(StickSensitivityGraph), new PropertyMetadata((ushort)0, X2Changed));

        private ushort _prevX2;

        public static readonly DependencyProperty Y2Property = DependencyProperty.Register("Y2", typeof(ushort), typeof(StickSensitivityGraph), new PropertyMetadata((ushort)0, Y2Changed));

        private ushort _prevY2;

        public static readonly DependencyProperty X3Property = DependencyProperty.Register("X3", typeof(ushort), typeof(StickSensitivityGraph), new PropertyMetadata((ushort)0, X3Changed));

        private ushort _prevX3;

        public static readonly DependencyProperty Y3Property = DependencyProperty.Register("Y3", typeof(ushort), typeof(StickSensitivityGraph), new PropertyMetadata((ushort)0, Y3Changed));

        private ushort _prevY3;

        public static readonly DependencyProperty X4Property = DependencyProperty.Register("X4", typeof(ushort), typeof(StickSensitivityGraph), new PropertyMetadata((ushort)0, X4Changed));

        private ushort _prevX4;

        public static readonly DependencyProperty Y4Property = DependencyProperty.Register("Y4", typeof(ushort), typeof(StickSensitivityGraph), new PropertyMetadata((ushort)0, Y4Changed));

        private ushort _prevY4;

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(StickSensitivityGraph), new PropertyMetadata(0.0, MaxValueChanged));

        public static readonly DependencyProperty DeadZoneMaxValueProperty = DependencyProperty.Register("DeadZoneMaxValue", typeof(int?), typeof(StickSensitivityGraph), new PropertyMetadata(null, DeadZoneMaxValueChangedCallback));

        public static readonly DependencyProperty NormalizedDeadZoneMaxValueProperty = DependencyProperty.Register("NormalizedDeadZoneMaxValue", typeof(double?), typeof(StickSensitivityGraph), new PropertyMetadata((object)null));

        public static readonly DependencyProperty GraphContainerWidthProperty = DependencyProperty.Register("GraphContainerWidth", typeof(double), typeof(StickSensitivityGraph), new PropertyMetadata(0.0, GraphContainerWidthChanged));

        public static readonly DependencyProperty GraphContainerHeightProperty = DependencyProperty.Register("GraphContainerHeight", typeof(double), typeof(StickSensitivityGraph), new PropertyMetadata(0.0, GraphContainerHeightChanged));

        public static readonly DependencyProperty NormalizedX1Property = DependencyProperty.Register("NormalizedX1", typeof(double), typeof(StickSensitivityGraph), new PropertyMetadata(0.0, NormalizedX1Changed));

        public static readonly DependencyProperty NormalizedY1Property = DependencyProperty.Register("NormalizedY1", typeof(double), typeof(StickSensitivityGraph), new PropertyMetadata(0.0, NormalizedY1Changed));

        public static readonly DependencyProperty NormalizedX2Property = DependencyProperty.Register("NormalizedX2", typeof(double), typeof(StickSensitivityGraph), new PropertyMetadata(0.0, NormalizedX2Changed));

        public static readonly DependencyProperty NormalizedY2Property = DependencyProperty.Register("NormalizedY2", typeof(double), typeof(StickSensitivityGraph), new PropertyMetadata(0.0, NormalizedY2Changed));

        public static readonly DependencyProperty NormalizedX3Property = DependencyProperty.Register("NormalizedX3", typeof(double), typeof(StickSensitivityGraph), new PropertyMetadata(0.0, NormalizedX3Changed));

        public static readonly DependencyProperty NormalizedY3Property = DependencyProperty.Register("NormalizedY3", typeof(double), typeof(StickSensitivityGraph), new PropertyMetadata(0.0, NormalizedY3Changed));

        public static readonly DependencyProperty NormalizedX4Property = DependencyProperty.Register("NormalizedX4", typeof(double), typeof(StickSensitivityGraph), new PropertyMetadata(0.0, NormalizedX4Changed));

        public static readonly DependencyProperty NormalizedY4Property = DependencyProperty.Register("NormalizedY4", typeof(double), typeof(StickSensitivityGraph), new PropertyMetadata(0.0, NormalizedY4Changed));

        public static readonly DependencyProperty DeflectionProperty = DependencyProperty.Register("Deflection", typeof(DISC_SOFT_GAMEPAD_STICK_DEFLECTION_NPROPCHANGE), typeof(StickSensitivityGraph), new PropertyMetadata(null, DeflectionChangedCallback));

        private bool _contentLoaded;

        public bool DoUpdateDeflactionZeroProperties
        {
            get
            {
                return (bool)GetValue(DoUpdateDeflactionZeroPropertiesProperty);
            }
            set
            {
                SetValue(DoUpdateDeflactionZeroPropertiesProperty, value);
            }
        }

        public bool EnableValueCoerce
        {
            get
            {
                return (bool)GetValue(EnableValueCoerceProperty);
            }
            set
            {
                SetValue(EnableValueCoerceProperty, value);
            }
        }

        public ushort X1
        {
            get
            {
                return (ushort)GetValue(X1Property);
            }
            set
            {
                SetValue(X1Property, value);
            }
        }

        public ushort Y1
        {
            get
            {
                return (ushort)GetValue(Y1Property);
            }
            set
            {
                SetValue(Y1Property, value);
            }
        }

        public ushort X2
        {
            get
            {
                return (ushort)GetValue(X2Property);
            }
            set
            {
                SetValue(X2Property, value);
            }
        }

        public ushort Y2
        {
            get
            {
                return (ushort)GetValue(Y2Property);
            }
            set
            {
                SetValue(Y2Property, value);
            }
        }

        public ushort X3
        {
            get
            {
                return (ushort)GetValue(X3Property);
            }
            set
            {
                SetValue(X3Property, value);
            }
        }

        public ushort Y3
        {
            get
            {
                return (ushort)GetValue(Y3Property);
            }
            set
            {
                SetValue(Y3Property, value);
            }
        }

        public ushort X4
        {
            get
            {
                return (ushort)GetValue(X4Property);
            }
            set
            {
                SetValue(X4Property, value);
            }
        }

        public ushort Y4
        {
            get
            {
                return (ushort)GetValue(Y4Property);
            }
            set
            {
                SetValue(Y4Property, value);
            }
        }

        public double MaxValue
        {
            get
            {
                return (double)GetValue(MaxValueProperty);
            }
            set
            {
                SetValue(MaxValueProperty, value);
            }
        }

        public int? DeadZoneMaxValue
        {
            get
            {
                return (int?)GetValue(DeadZoneMaxValueProperty);
            }
            set
            {
                SetValue(DeadZoneMaxValueProperty, value);
            }
        }

        public double? NormalizedDeadZoneMaxValue
        {
            get
            {
                return (double?)GetValue(NormalizedDeadZoneMaxValueProperty);
            }
            set
            {
                SetValue(NormalizedDeadZoneMaxValueProperty, value);
            }
        }

        public double GraphContainerWidth
        {
            get
            {
                return (double)GetValue(GraphContainerWidthProperty);
            }
            set
            {
                SetValue(GraphContainerWidthProperty, value);
            }
        }

        public double GraphContainerHeight
        {
            get
            {
                return (double)GetValue(GraphContainerHeightProperty);
            }
            set
            {
                SetValue(GraphContainerHeightProperty, value);
            }
        }

        public double NormalizedX1
        {
            get
            {
                return (double)GetValue(NormalizedX1Property);
            }
            set
            {
                SetValue(NormalizedX1Property, value);
            }
        }

        public double NormalizedY1
        {
            get
            {
                return (double)GetValue(NormalizedY1Property);
            }
            set
            {
                SetValue(NormalizedY1Property, value);
            }
        }

        public double NormalizedX2
        {
            get
            {
                return (double)GetValue(NormalizedX2Property);
            }
            set
            {
                SetValue(NormalizedX2Property, value);
            }
        }

        public double NormalizedY2
        {
            get
            {
                return (double)GetValue(NormalizedY2Property);
            }
            set
            {
                SetValue(NormalizedY2Property, value);
            }
        }

        public double NormalizedX3
        {
            get
            {
                return (double)GetValue(NormalizedX3Property);
            }
            set
            {
                SetValue(NormalizedX3Property, value);
            }
        }

        public double NormalizedY3
        {
            get
            {
                return (double)GetValue(NormalizedY3Property);
            }
            set
            {
                SetValue(NormalizedY3Property, value);
            }
        }

        public double NormalizedX4
        {
            get
            {
                return (double)GetValue(NormalizedX4Property);
            }
            set
            {
                SetValue(NormalizedX4Property, value);
            }
        }

        public double NormalizedY4
        {
            get
            {
                return (double)GetValue(NormalizedY4Property);
            }
            set
            {
                SetValue(NormalizedY4Property, value);
            }
        }

        public DISC_SOFT_GAMEPAD_STICK_DEFLECTION_NPROPCHANGE Deflection
        {
            get
            {
                return (DISC_SOFT_GAMEPAD_STICK_DEFLECTION_NPROPCHANGE)GetValue(DeflectionProperty);
            }
            set
            {
                SetValue(DeflectionProperty, value);
            }
        }

        private static void X1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StickSensitivityGraph)?.OnX1Changed();
        }

        private static void Y1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StickSensitivityGraph)?.OnY1Changed();
        }

        private static void X2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StickSensitivityGraph)?.OnX2Changed();
        }

        private static void Y2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StickSensitivityGraph)?.OnY2Changed();
        }

        private static void X3Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StickSensitivityGraph)?.OnX3Changed();
        }

        private static void Y3Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StickSensitivityGraph)?.OnY3Changed();
        }

        private static void X4Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StickSensitivityGraph)?.OnX4Changed();
        }

        private static void Y4Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StickSensitivityGraph)?.OnY4Changed();
        }

        private static void MaxValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StickSensitivityGraph)?.RecalculateAllNormalizedValues();
        }

        private static void DeadZoneMaxValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StickSensitivityGraph)?.RecalculateNormalizedNormalizedDeadZoneMaxValue();
        }

        private static void GraphContainerWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StickSensitivityGraph)?.RecalculateNormalizedXCoordinates();
        }

        private static void GraphContainerHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StickSensitivityGraph)?.RecalculateNormalizedYCoordinates();
        }

        private static void NormalizedX1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StickSensitivityGraph)?.RecalculateX1();
        }

        private static void NormalizedY1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StickSensitivityGraph)?.RecalculateY1();
        }

        private static void NormalizedX2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StickSensitivityGraph)?.RecalculateX2();
        }

        private static void NormalizedY2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StickSensitivityGraph)?.RecalculateY2();
        }

        private static void NormalizedX3Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StickSensitivityGraph)?.RecalculateX3();
        }

        private static void NormalizedY3Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StickSensitivityGraph)?.RecalculateY3();
        }

        private static void NormalizedX4Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StickSensitivityGraph)?.RecalculateX4();
        }

        private static void NormalizedY4Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StickSensitivityGraph)?.RecalculateY4();
        }

        private static void DeflectionChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StickSensitivityGraph)?.OnDeflectionChanged();
        }

        public StickSensitivityGraph()
        {
            InitializeComponent();
            App.EventAggregator.GetEvent<CloseAllPopups>().Subscribe(delegate
            {
                SVGButton sVGButton = base.Template.FindName("manualButton", this) as SVGButton;
                if (sVGButton != null)
                {
                    sVGButton.IsTriggered = false;
                }
            });
        }

        private void OnDeflectionChanged()
        {
            if (DeflectionProperty == null)
            {
                return;
            }
            EnableValueCoerce = false;
            BindingOperations.SetBinding(this, X4Property, new Binding());
            BindingOperations.SetBinding(this, X3Property, new Binding());
            BindingOperations.SetBinding(this, X2Property, new Binding());
            BindingOperations.SetBinding(this, X1Property, new Binding());
            BindingOperations.SetBinding(this, Y4Property, new Binding());
            BindingOperations.SetBinding(this, Y3Property, new Binding());
            BindingOperations.SetBinding(this, Y2Property, new Binding());
            BindingOperations.SetBinding(this, Y1Property, new Binding());
            BindingOperations.SetBinding(this, X4Property, new Binding("HorizontalPoint[3].TravelDistance")
            {
                Source = Deflection,
                Mode = BindingMode.TwoWay
            });
            BindingOperations.SetBinding(this, X3Property, new Binding("HorizontalPoint[2].TravelDistance")
            {
                Source = Deflection,
                Mode = BindingMode.TwoWay
            });
            BindingOperations.SetBinding(this, X2Property, new Binding("HorizontalPoint[1].TravelDistance")
            {
                Source = Deflection,
                Mode = BindingMode.TwoWay
            });
            BindingOperations.SetBinding(this, X1Property, new Binding("HorizontalPoint[0].TravelDistance")
            {
                Source = Deflection,
                Mode = BindingMode.TwoWay
            });
            BindingOperations.SetBinding(this, Y4Property, new Binding("HorizontalPoint[3].NewValue")
            {
                Source = Deflection,
                Mode = BindingMode.TwoWay
            });
            BindingOperations.SetBinding(this, Y3Property, new Binding("HorizontalPoint[2].NewValue")
            {
                Source = Deflection,
                Mode = BindingMode.TwoWay
            });
            BindingOperations.SetBinding(this, Y2Property, new Binding("HorizontalPoint[1].NewValue")
            {
                Source = Deflection,
                Mode = BindingMode.TwoWay
            });
            BindingOperations.SetBinding(this, Y1Property, new Binding("HorizontalPoint[0].NewValue")
            {
                Source = Deflection,
                Mode = BindingMode.TwoWay
            });
            if (DoUpdateDeflactionZeroProperties)
            {
                UpdateDeflectionZeroProperties();
            }
            Task.Run(async delegate
            {
                await Task.Delay(100);
                ThreadHelper.ExecuteInMainDispatcher(delegate
                {
                    RecalculateAllNormalizedValues();
                    EnableValueCoerce = true;
                });
            });
        }

        private void UpdateDeflectionZeroProperties()
        {
            if (Deflection == null)
            {
                return;
            }
            DISC_SOFT_GAMEPAD_STICK_DEFLECTION_POINT_Wrapper[] horizontalPoint = Deflection.HorizontalPoint;
            foreach (DISC_SOFT_GAMEPAD_STICK_DEFLECTION_POINT_Wrapper dISC_SOFT_GAMEPAD_STICK_DEFLECTION_POINT_Wrapper in horizontalPoint)
            {
                if (dISC_SOFT_GAMEPAD_STICK_DEFLECTION_POINT_Wrapper.NewValue == 0)
                {
                    dISC_SOFT_GAMEPAD_STICK_DEFLECTION_POINT_Wrapper.NewValue = 11;
                    dISC_SOFT_GAMEPAD_STICK_DEFLECTION_POINT_Wrapper.NewValue = 0;
                }
                if (dISC_SOFT_GAMEPAD_STICK_DEFLECTION_POINT_Wrapper.TravelDistance == 0)
                {
                    dISC_SOFT_GAMEPAD_STICK_DEFLECTION_POINT_Wrapper.TravelDistance = 11;
                    dISC_SOFT_GAMEPAD_STICK_DEFLECTION_POINT_Wrapper.TravelDistance = 0;
                }
            }
        }

        private void OnX1Changed()
        {
            bool num = Math.Abs(_prevX1 - X1) > 10;
            if (num || (_prevX1 != X1 && X1 >= X2))
            {
                _prevX1 = X1;
            }
            if (num)
            {
                RecalculateNormalizedX1();
            }
        }

        private void OnX2Changed()
        {
            bool num = Math.Abs(_prevX2 - X2) > 10;
            if (num || (_prevX2 != X2 && (X2 >= X1 || X2 >= X3)))
            {
                _prevX2 = X2;
            }
            if (num)
            {
                RecalculateNormalizedX2();
            }
        }

        private void OnX3Changed()
        {
            bool num = Math.Abs(_prevX3 - X3) > 10;
            if (num || (_prevX3 != X3 && (X3 >= X2 || X3 >= X4)))
            {
                _prevX3 = X3;
            }
            if (num)
            {
                RecalculateNormalizedX3();
            }
        }

        private void OnX4Changed()
        {
            bool num = Math.Abs(_prevX4 - X4) > 10;
            if (num || (_prevX4 != X4 && X4 >= X3))
            {
                _prevX4 = X4;
            }
            if (num)
            {
                RecalculateNormalizedX4();
            }
        }

        private void OnY1Changed()
        {
            bool num = Math.Abs(_prevY1 - Y1) > 10;
            if (num || (_prevY1 != Y1 && Y1 >= Y2))
            {
                _prevY1 = Y1;
            }
            if (num)
            {
                RecalculateNormalizedY1();
            }
        }

        private void OnY2Changed()
        {
            bool num = Math.Abs(_prevY2 - Y2) > 10;
            if (num || (_prevY2 != Y2 && (Y2 >= Y1 || Y2 >= Y3)))
            {
                _prevY2 = Y2;
            }
            if (num)
            {
                RecalculateNormalizedY2();
            }
        }

        private void OnY3Changed()
        {
            bool num = Math.Abs(_prevY3 - Y3) > 10;
            if (num || (_prevY3 != Y3 && (Y3 >= Y2 || Y3 >= Y4)))
            {
                _prevY3 = Y3;
            }
            if (num)
            {
                RecalculateNormalizedY3();
            }
        }

        private void OnY4Changed()
        {
            bool num = Math.Abs(_prevY4 - Y4) > 10;
            if (num || (_prevY4 != Y4 && Y4 >= Y3))
            {
                _prevY4 = Y4;
            }
            if (num)
            {
                RecalculateNormalizedY4();
            }
        }

        private void RecalculateNormalizedX1()
        {
            NormalizedX1 = (double)(int)X1 / MaxValue * GraphContainerWidth;
        }

        private void RecalculateNormalizedX2()
        {
            NormalizedX2 = (double)(int)X2 / MaxValue * GraphContainerWidth;
        }

        private void RecalculateNormalizedX3()
        {
            NormalizedX3 = (double)(int)X3 / MaxValue * GraphContainerWidth;
        }

        private void RecalculateNormalizedX4()
        {
            NormalizedX4 = (double)(int)X4 / MaxValue * GraphContainerWidth;
        }

        private void RecalculateNormalizedNormalizedDeadZoneMaxValue()
        {
            NormalizedDeadZoneMaxValue = (double?)DeadZoneMaxValue / MaxValue * GraphContainerWidth;
        }

        private void RecalculateNormalizedY1()
        {
            NormalizedY1 = GraphContainerHeight - (double)(int)Y1 / MaxValue * GraphContainerHeight;
        }

        private void RecalculateNormalizedY2()
        {
            NormalizedY2 = GraphContainerHeight - (double)(int)Y2 / MaxValue * GraphContainerHeight;
        }

        private void RecalculateNormalizedY3()
        {
            NormalizedY3 = GraphContainerHeight - (double)(int)Y3 / MaxValue * GraphContainerHeight;
        }

        private void RecalculateNormalizedY4()
        {
            NormalizedY4 = GraphContainerHeight - (double)(int)Y4 / MaxValue * GraphContainerHeight;
        }

        private void RecalculateNormalizedXCoordinates()
        {
            RecalculateNormalizedX1();
            RecalculateNormalizedX2();
            RecalculateNormalizedX3();
            RecalculateNormalizedX4();
            RecalculateNormalizedNormalizedDeadZoneMaxValue();
        }

        private void RecalculateNormalizedYCoordinates()
        {
            RecalculateNormalizedY1();
            RecalculateNormalizedY2();
            RecalculateNormalizedY3();
            RecalculateNormalizedY4();
        }

        private void RecalculateAllNormalizedValues()
        {
            RecalculateNormalizedXCoordinates();
            RecalculateNormalizedYCoordinates();
        }

        private void RecalculateX1()
        {
            ushort num = (ushort)(NormalizedX1 * MaxValue / GraphContainerWidth);
            if (Math.Abs(num - X1) > 10)
            {
                X1 = num;
            }
        }

        private void RecalculateX2()
        {
            ushort num = (ushort)(NormalizedX2 * MaxValue / GraphContainerWidth);
            if (Math.Abs(num - X2) > 10)
            {
                X2 = num;
            }
        }

        private void RecalculateX3()
        {
            ushort num = (ushort)(NormalizedX3 * MaxValue / GraphContainerWidth);
            if (Math.Abs(num - X3) > 10)
            {
                X3 = num;
            }
        }

        private void RecalculateX4()
        {
            ushort num = (ushort)(NormalizedX4 * MaxValue / GraphContainerWidth);
            if (Math.Abs(num - X4) > 10)
            {
                X4 = num;
            }
        }

        private void RecalculateY1()
        {
            ushort num = (ushort)((GraphContainerHeight - NormalizedY1) * MaxValue / GraphContainerHeight);
            if (Math.Abs(num - Y1) > 10)
            {
                Y1 = num;
            }
        }

        private void RecalculateY2()
        {
            ushort num = (ushort)((GraphContainerHeight - NormalizedY2) * MaxValue / GraphContainerHeight);
            if (Math.Abs(num - Y2) > 10)
            {
                Y2 = num;
            }
        }

        private void RecalculateY3()
        {
            ushort num = (ushort)((GraphContainerHeight - NormalizedY3) * MaxValue / GraphContainerHeight);
            if (Math.Abs(num - Y3) > 10)
            {
                Y3 = num;
            }
        }

        private void RecalculateY4()
        {
            ushort num = (ushort)((GraphContainerHeight - NormalizedY4) * MaxValue / GraphContainerHeight);
            if (Math.Abs(num - Y4) > 10)
            {
                Y4 = num;
            }
        }

        private void RecalculateXCoordinates()
        {
            RecalculateX1();
            RecalculateX2();
            RecalculateX3();
            RecalculateX4();
        }

        private void RecalculateYCoordinates()
        {
            RecalculateY1();
            RecalculateY2();
            RecalculateY3();
            RecalculateY4();
        }

        private void RecalculateAllValues()
        {
            RecalculateXCoordinates();
            RecalculateYCoordinates();
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
        public void InitializeComponent()
        {
            if (!_contentLoaded)
            {
                _contentLoaded = true;
                Uri resourceLocator = new Uri("/reWASD;component/controls/sticksensitivity/sticksensitivitygraph.xaml", UriKind.Relative);
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
