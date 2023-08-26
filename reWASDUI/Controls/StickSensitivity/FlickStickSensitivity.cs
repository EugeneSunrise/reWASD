using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace reWASDUI.Controls.StickSensitivity
{
    public class FlickStickSensitivity : UserControl, IComponentConnector
    {
        public static readonly DependencyProperty IsShowInvertCheckBoxProperty = DependencyProperty.Register("IsShowInvertCheckBox", typeof(bool), typeof(FlickStickSensitivity), new PropertyMetadata(false));

        private bool _contentLoaded;

        public bool IsShowInvertCheckBox
        {
            get
            {
                return (bool)GetValue(IsShowInvertCheckBoxProperty);
            }
            set
            {
                SetValue(IsShowInvertCheckBoxProperty, value);
            }
        }

        public FlickStickSensitivity()
        {
            InitializeComponent();
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
        public void InitializeComponent()
        {
            if (!_contentLoaded)
            {
                _contentLoaded = true;
                Uri resourceLocator = new Uri("/reWASD;component/controls/sticksensitivity/flicksticksensitivity.xaml", UriKind.Relative);
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
