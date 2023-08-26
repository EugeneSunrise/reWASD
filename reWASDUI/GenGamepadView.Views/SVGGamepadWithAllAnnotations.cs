using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.View;
using DiscSoft.NET.Common.View.RecolorableImages;
using DiscSoft.NET.Common.View.SVGPositioningControls;
using reWASDCommon.Infrastructure.Enums;
using reWASDCommon.Infrastructure.SupportedControllers;
using reWASDCommon.Infrastructure.SupportedControllers.Base;
using reWASDEngine.Services.OverlayAPI;
using reWASDUI.Utils.Converters;
using reWASDUI.Utils.XBUtil;
using reWASDUI.Views;
using XBEliteWPF.Infrastructure.KeyBindingsModel;
using XBEliteWPF.Infrastructure.KeyBindingsModel.XB;
using XBEliteWPF.Utils.XBUtilModel;

namespace GenGamepadView.Views
{
    public class SVGGamepadWithAllAnnotations : UserControl, IComponentConnector
    {
        private bool _firstLoadInited;

        private ControllerTypeEnum ControllerType;

        private BaseXBBindingCollection XBBindings;

        private string _prevName;

        private Color _prevColor;

        private bool IsFrontSide;

        internal SVGContainerGrid svgContainerGrid;

        internal RecolorableSVG svgContainer;

        private bool _contentLoaded;

        public SVGGamepadWithAllAnnotations(BaseXBBindingCollection xBBindings, ControllerTypeEnum controllerType = ControllerTypeEnum.Xbox360)
        {
            XBBindings = xBBindings;
            ControllerType = controllerType;
            IsFrontSide = true;
            InitializeComponent();
            svgContainer.Drawing = GetCurrentSVGGamepadDrawing();
            base.IsVisibleChanged += SVGGamepadWithAllAnnotations_IsVisibleChanged;
            base.Loaded += OnLoaded;
        }

        private Drawing GetCurrentSVGGamepadDrawing()
        {
            Drawing drawing = null;
            string image = (ControllersHelper.SupportedControllers.Find((SupportedControllerInfo x) => x.ControllerType == ControllerType) as SupportedGamepad).Image;
            Drawing obj = ((!IsFrontSide) ? (Application.Current.FindResource(image + "Back") as Drawing) : (Application.Current.FindResource(image) as Drawing))?.Clone();
            if (obj != null)
            {
                Drawing itemByName = obj.GetItemByName("WizardButton");
                if (itemByName != null)
                {
                    itemByName.ChangeColor(Colors.Transparent);
                    return obj;
                }
                return obj;
            }
            return obj;
        }

        private void SVGGamepadWithAllAnnotations_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                InitContainerGrid();
                UpdateAttachedButtonsPos();
            }
        }

        private void OnLoadedWithSave(object sender, RoutedEventArgs e)
        {
            InitContainerGrid();
            UpdateLayout();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            InitContainerGrid();
            UpdateLayout();
        }

        private void UpdateAttachedButtonsPos()
        {
            if (svgContainerGrid == null)
            {
                return;
            }
            foreach (object child in svgContainerGrid.Children)
            {
                (child as SVGAnchorContainer)?.AttachToSVGElement();
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
            if (_firstLoadInited)
            {
                return;
            }
            _firstLoadInited = true;
            foreach (XBBinding item2 in XBBindings.Where((XBBinding x) => x.IsNotEmpty))
            {
                SVGAnchorContainer sVGAnchorContainer = new SVGAnchorContainer
                {
                    Name = item2.GamepadButton.ToString() + "Anchor"
                };
                sVGAnchorContainer.SetBinding(BaseSVGPositioning.SVGElementNameProperty, new Binding
                {
                    Converter = new ControllerButtonToAnchorNameConverter(),
                    ConverterParameter = item2.GamepadButton
                });
                Grid.SetColumnSpan(sVGAnchorContainer, 3);
                Grid.SetRowSpan(sVGAnchorContainer, 3);
                int shiftIndex = (XBBindings.IsShiftCollection ? (XBBindings as ShiftXBBindingCollection).ShiftModificatorNum : 0);
                SolidColorBrush brushForShiftIndex = XBEliteWPF.Utils.XBUtilModel.XBUtils.GetBrushForShiftIndex(shiftIndex);
                sVGAnchorContainer.Content = new XBBindingView
                {
                    XBBinding = item2,
                    Foreground = brushForShiftIndex
                };
                svgContainerGrid.Children.Add(sVGAnchorContainer);
                string name = reWASDUI.Utils.XBUtil.XBUtils.ConvertGamepadButtonToAnchorString(item2.GamepadButton) + "Annotation";
                SolidColorBrush annotationColor = GetAnnotationColor(shiftIndex, item2);
                RecolorAnnotation(name, annotationColor);
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
                Drawing drawing = (svgContainer?.Source as DrawingImage)?.GetItemByName(name);
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
                Uri resourceLocator = new Uri("/reWASD;component/controls/fastconfigrenders/svggamepadwithallannotations.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
        internal Delegate _CreateDelegate(Type delegateType, string handler)
        {
            return Delegate.CreateDelegate(delegateType, this, handler);
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    svgContainerGrid = (SVGContainerGrid)target;
                    break;
                case 2:
                    svgContainer = (RecolorableSVG)target;
                    break;
                default:
                    _contentLoaded = true;
                    break;
            }
        }
    }
}
