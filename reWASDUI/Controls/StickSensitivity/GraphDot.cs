using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace reWASDUI.Controls.StickSensitivity
{
    public class GraphDot : Button, IComponentConnector
    {
        public static readonly DependencyProperty XProperty = DependencyProperty.Register("X", typeof(double?), typeof(GraphDot), new PropertyMetadata(null, OnXChangedCallback, CoerceXCallback));

        public static readonly DependencyProperty MaxXProperty = DependencyProperty.Register("MaxX", typeof(double?), typeof(GraphDot), new PropertyMetadata((object)null));

        public static readonly DependencyProperty YProperty = DependencyProperty.Register("Y", typeof(double?), typeof(GraphDot), new PropertyMetadata(null, OnYChangedCallback, CoerceYCallback));

        public static readonly DependencyProperty MaxYProperty = DependencyProperty.Register("MaxY", typeof(double?), typeof(GraphDot), new PropertyMetadata((object)null));

        public static readonly DependencyProperty LeftDotProperty = DependencyProperty.Register("LeftDot", typeof(GraphDot), typeof(GraphDot), new PropertyMetadata((object)null));

        public static readonly DependencyProperty RightDotProperty = DependencyProperty.Register("RightDot", typeof(GraphDot), typeof(GraphDot), new PropertyMetadata((object)null));

        public static readonly DependencyProperty HighlightedBrushProperty = DependencyProperty.Register("HighlightedBrush", typeof(Brush), typeof(GraphDot), new PropertyMetadata((object)null));

        public static readonly DependencyProperty PressedBrushProperty = DependencyProperty.Register("PressedBrush", typeof(Brush), typeof(GraphDot), new PropertyMetadata((object)null));

        public static readonly DependencyProperty EnableValueCoerceProperty = DependencyProperty.Register("EnableValueCoerce", typeof(bool), typeof(GraphDot), new PropertyMetadata(true));

        private bool _dragStarted;

        private bool _contentLoaded;

        public double? X
        {
            get
            {
                return (double?)GetValue(XProperty);
            }
            set
            {
                SetValue(XProperty, value);
            }
        }

        public double? MaxX
        {
            get
            {
                return (double?)GetValue(MaxXProperty);
            }
            set
            {
                SetValue(MaxXProperty, value);
            }
        }

        public double? Y
        {
            get
            {
                return (double?)GetValue(YProperty);
            }
            set
            {
                SetValue(YProperty, value);
            }
        }

        public double? MaxY
        {
            get
            {
                return (double?)GetValue(MaxYProperty);
            }
            set
            {
                SetValue(MaxYProperty, value);
            }
        }

        public GraphDot LeftDot
        {
            get
            {
                return (GraphDot)GetValue(LeftDotProperty);
            }
            set
            {
                SetValue(LeftDotProperty, value);
            }
        }

        public GraphDot RightDot
        {
            get
            {
                return (GraphDot)GetValue(RightDotProperty);
            }
            set
            {
                SetValue(RightDotProperty, value);
            }
        }

        public Brush HighlightedBrush
        {
            get
            {
                return (Brush)GetValue(HighlightedBrushProperty);
            }
            set
            {
                SetValue(HighlightedBrushProperty, value);
            }
        }

        public Brush PressedBrush
        {
            get
            {
                return (Brush)GetValue(PressedBrushProperty);
            }
            set
            {
                SetValue(PressedBrushProperty, value);
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

        private static object CoerceXCallback(DependencyObject d, object baseValue)
        {
            if (((GraphDot)d).EnableValueCoerce)
            {
                return (d as GraphDot)?.CoerceX((double?)baseValue);
            }
            return baseValue;
        }

        private static void OnXChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as GraphDot)?.OnXChanged();
        }

        private static object CoerceYCallback(DependencyObject d, object baseValue)
        {
            if (((GraphDot)d).EnableValueCoerce)
            {
                return (d as GraphDot)?.CoerceY((double?)baseValue);
            }
            return baseValue;
        }

        private static void OnYChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as GraphDot)?.OnYChanged();
        }

        public GraphDot()
        {
            InitializeComponent();
            base.PreviewMouseLeftButtonDown += OnMouseDown;
            base.PreviewMouseLeftButtonUp += OnMouseUp;
            base.PreviewMouseMove += OnMouseMove;
            base.MouseLeave += OnMouseLeave;
        }

        private double? CoerceX(double? desiredX)
        {
            if (MaxX.HasValue && desiredX >= MaxX && (RightDot == null || MaxX <= RightDot.X) && (LeftDot == null || MaxX >= LeftDot.X))
            {
                return MaxX;
            }
            if ((LeftDot?.X.HasValue ?? false) && desiredX <= LeftDot.X)
            {
                return LeftDot.X;
            }
            if ((RightDot?.X.HasValue ?? false) && desiredX >= RightDot.X)
            {
                return RightDot.X;
            }
            if (desiredX <= 0.0)
            {
                return 0.0;
            }
            return desiredX;
        }

        private void OnXChanged()
        {
            RecalculateXMargin();
        }

        private void RecalculateXMargin()
        {
            if (X.HasValue)
            {
                Thickness margin = base.Margin;
                margin.Left = X.Value - base.Width / 2.0;
                base.Margin = margin;
            }
        }

        private double? CoerceY(double? desiredY)
        {
            if (MaxY.HasValue && desiredY >= MaxY)
            {
                return MaxY;
            }
            if ((LeftDot?.Y.HasValue ?? false) && desiredY >= LeftDot.Y)
            {
                return LeftDot.Y;
            }
            if ((RightDot?.Y.HasValue ?? false) && desiredY <= RightDot.Y)
            {
                return RightDot.Y;
            }
            if (desiredY <= 0.0)
            {
                return 0.0;
            }
            return desiredY;
        }

        private void OnYChanged()
        {
            RecalculateYMargin();
        }

        private void RecalculateYMargin()
        {
            if (Y.HasValue)
            {
                Thickness margin = base.Margin;
                margin.Top = Y.Value - base.Height / 2.0;
                base.Margin = margin;
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _dragStarted = true;
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _dragStarted = false;
        }

        private void OnMouseLeave(object sender, MouseEventArgs mouseEventArgs)
        {
            _dragStarted = false;
        }

        private void OnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            if (_dragStarted)
            {
                Point position = mouseEventArgs.GetPosition(base.Parent as Grid);
                X = CoerceX(position.X);
                Y = CoerceY(position.Y);
            }
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
        public void InitializeComponent()
        {
            if (!_contentLoaded)
            {
                _contentLoaded = true;
                Uri resourceLocator = new Uri("/reWASD;component/controls/sticksensitivity/graphdot.xaml", UriKind.Relative);
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
