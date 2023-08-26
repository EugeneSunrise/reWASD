using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.View;
using DiscSoft.NET.Common.View.RecolorableImages;
using DiscSoft.NET.Common.View.SVGPositioningControls;
using reWASDEngine.Services.OverlayAPI;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.ControllerBindings;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.Utils.Converters;
using reWASDUI.Utils.XBUtil;
using reWASDUI.Views;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Infrastructure.KeyBindingsModel.ControllerBindings;
using XBEliteWPF.Infrastructure.KeyBindingsModel.XB;
using XBEliteWPF.Infrastructure.reWASDMapping.KeyScanCodes;
using XBEliteWPF.Utils.XBUtilModel;

namespace GenGamepadView.Views
{
    public class MouseMappingView : UserControl, IComponentConnector
    {
        private bool _firstLoadInited;

        private SVGContainerGrid _svgContainerGrid;

        private BaseXBBindingCollection XBBindings;

        private string _prevName;

        private Color _prevColor;

        private bool _contentLoaded;

        public MouseMappingView(BaseXBBindingCollection xBBindings)
        {
            XBBindings = xBBindings;
            InitializeComponent();
            base.IsVisibleChanged += SVGGamepadWithAllAnnotations_IsVisibleChanged;
            base.Loaded += OnLoaded;
        }

        private void SVGGamepadWithAllAnnotations_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                InitContainerGrid();
                UpdateAttachedButtonsPos();
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            InitContainerGrid();
            UpdateLayout();
        }

        private void UpdateAttachedButtonsPos()
        {
            if (_svgContainerGrid == null)
            {
                return;
            }
            foreach (object child in _svgContainerGrid.Children)
            {
                (child as SVGElementAttachedButton)?.AttachToSVGElement();
            }
        }

        private void InitContainerGrid()
        {
            if (_firstLoadInited)
            {
                return;
            }
            ResourceDictionary item = TryFindResource("GenerationGamepadWindowResources") as ResourceDictionary;
            base.Resources.MergedDictionaries.Add(item);
            if (_svgContainerGrid == null)
            {
                _svgContainerGrid = VisualTreeHelperExt.FindChild<SVGContainerGrid>(this);
            }
            if (_svgContainerGrid == null)
            {
                _svgContainerGrid = base.Template.FindName("svgContainerGrid", this) as SVGContainerGrid;
            }
            if (_svgContainerGrid == null || _firstLoadInited)
            {
                return;
            }
            _firstLoadInited = true;
            foreach (XBBinding xBBinding in XBBindings)
            {
                if (xBBinding.IsNotEmpty)
                {
                    SVGAnchorContainer sVGAnchorContainer = new SVGAnchorContainer
                    {
                        Name = xBBinding.GamepadButton.ToString() + "Anchor"
                    };
                    sVGAnchorContainer.SetBinding(BaseSVGPositioning.SVGElementNameProperty, new Binding
                    {
                        Converter = new ControllerButtonToAnchorNameConverter(),
                        ConverterParameter = xBBinding.GamepadButton
                    });
                    Grid.SetColumnSpan(sVGAnchorContainer, 3);
                    Grid.SetRowSpan(sVGAnchorContainer, 3);
                    int shiftIndex = (XBBindings.IsShiftCollection ? (XBBindings as ShiftXBBindingCollection).ShiftModificatorNum : 0);
                    SolidColorBrush brushForShiftIndex = XBEliteWPF.Utils.XBUtilModel.XBUtils.GetBrushForShiftIndex(shiftIndex);
                    sVGAnchorContainer.Content = new XBBindingView
                    {
                        XBBinding = xBBinding,
                        Foreground = brushForShiftIndex
                    };
                    _svgContainerGrid.Children.Add(sVGAnchorContainer);
                    string name = reWASDUI.Utils.XBUtil.XBUtils.ConvertGamepadButtonToAnchorString(xBBinding.GamepadButton) + "Annotation";
                    SolidColorBrush annotationColor = GetAnnotationColor(shiftIndex, xBBinding);
                    RecolorAnnotation(name, annotationColor);
                }
            }
            if (!XBBindings.IsControllerBindingsPresent)
            {
                return;
            }
            foreach (ControllerBinding controllerBinding in XBBindings.ControllerBindings)
            {
                string text;
                try
                {
                    text = reWASDUI.Utils.XBUtil.XBUtils.ConvertMouseButtonToAnchorString(KeyScanCodeV2.FindMouseButtonByKeyScanCode(controllerBinding.XBBinding.KeyScanCode));
                }
                catch (Exception)
                {
                    continue;
                }
                SVGAnchorContainer sVGAnchorContainer2 = new SVGAnchorContainer();
                sVGAnchorContainer2.Name = text + "Anchor";
                string name2 = text + "Annotation";
                sVGAnchorContainer2.SetBinding(BaseSVGPositioning.SVGElementNameProperty, new Binding
                {
                    Converter = new ControllerButtonToAnchorNameConverter(),
                    ConverterParameter = controllerBinding.XBBinding.KeyScanCode
                });
                Grid.SetColumnSpan(sVGAnchorContainer2, 3);
                Grid.SetRowSpan(sVGAnchorContainer2, 3);
                int shiftIndex2 = (XBBindings.IsShiftCollection ? (XBBindings as ShiftXBBindingCollection).ShiftModificatorNum : 0);
                SolidColorBrush brushForShiftIndex2 = XBEliteWPF.Utils.XBUtilModel.XBUtils.GetBrushForShiftIndex(shiftIndex2);
                sVGAnchorContainer2.Content = new XBBindingView
                {
                    XBBinding = controllerBinding.XBBinding,
                    Foreground = brushForShiftIndex2
                };
                _svgContainerGrid.Children.Add(sVGAnchorContainer2);
                SolidColorBrush annotationColor2 = GetAnnotationColor(shiftIndex2, controllerBinding.XBBinding);
                RecolorAnnotation(name2, annotationColor2);
            }
        }

        private SolidColorBrush GetAnnotationColor(int shiftIndex, XBBinding xbBinding)
        {
            SolidColorBrush result = XBEliteWPF.Utils.XBUtilModel.XBUtils.GetBrushForShiftIndex(shiftIndex);
            if (xbBinding == null)
            {
                return result;
            }
            if (xbBinding.IsInheritedBinding)
            {
                result = Application.Current.TryFindResource("DisabledControlForegroundColor") as SolidColorBrush;
            }
            return result;
        }

        private void RecolorAnnotation(string name, SolidColorBrush brush)
        {
            if (brush == null)
            {
                brush = new SolidColorBrush(Colors.Transparent);
            }
            if (!(_prevName == name) || !brush.Color.Equals(_prevColor))
            {
                Drawing drawing = (VisualTreeHelperExt.FindChild<RecolorableSVG>(this)?.Source as DrawingImage)?.GetItemByName(name);
                if (drawing != null)
                {
                    _prevName = name;
                    _prevColor = brush.Color;
                    drawing.ChangeColor(brush, isColorShiftFill: false);
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
                Uri resourceLocator = new Uri("/reWASD;component/controls/fastconfigrenders/mousemappingview.xaml", UriKind.Relative);
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
