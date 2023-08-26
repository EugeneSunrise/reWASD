using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using DiscSoft.NET.Common.View.RecolorableImages;
using reWASDEngine.Services.OverlayAPI;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.ControllerBindings;
using reWASDUI.Utils.Converters;
using XBEliteWPF.Utils.XBUtil;
using XBEliteWPF.Utils.XBUtilModel;

namespace GenGamepadView.Views
{
    public class KeyboardMappingView : UserControl, IComponentConnector
    {
        private bool _firstLoadInited;

        private new string Name;

        public BaseXBBindingCollection XBBindings;

        private ListBox ListBox;

        internal Border ContentBorder;

        private bool _contentLoaded;

        public KeyboardMappingView(BaseXBBindingCollection xbBindings, string name = "KeyBoard")
        {
            XBBindings = xbBindings;
            Name = name;
            InitializeComponent();
            base.IsVisibleChanged += SVGGamepadWithAllAnnotations_IsVisibleChanged;
            base.Loaded += OnLoaded;
        }

        private void InitContainerGrid()
        {
            if (!_firstLoadInited)
            {
                ResourceDictionary item = TryFindResource("GenerationGamepadWindowResources") as ResourceDictionary;
                base.Resources.MergedDictionaries.Add(item);
                List<ControllerBinding> list = XBBindings.ControllerBindings.Where((ControllerBinding x) => x.XBBinding.KeyScanCode.IsCategoryAllKeyboardTypes).ToList();
                ListBox = new ListBox
                {
                    Name = "keyMappingsItemsControl",
                    Style = (Style)TryFindResource("ListBoxStrippedStyle"),
                    ItemsSource = list
                };
                ListBox.SetValue(Grid.ColumnProperty, 0);
                ListBox.SetValue(Grid.ColumnSpanProperty, 3);
                ListBox.SetValue(VirtualizingStackPanel.VirtualizationModeProperty, VirtualizationMode.Recycling);
                ListBox.SetValue(VirtualizingStackPanel.IsVirtualizingProperty, true);
                DataTemplate itemTemplate = new DataTemplate(typeof(ControllerBinding))
                {
                    VisualTree = CreateDataTemplaleFactory()
                };
                ListBox.ItemTemplate = itemTemplate;
                ContentBorder.MaxHeight = list.Count * 33;
                ContentBorder.Child = ListBox;
                ListBox.RegisterName(ListBox.Name, ContentBorder.Child);
                _firstLoadInited = true;
                if (list == null || list.Count == 0)
                {
                    base.Visibility = Visibility.Collapsed;
                }
            }
        }

        private FrameworkElementFactory CreateDataTemplaleFactory()
        {
            SolidColorBrush brushForShiftIndex = XBUtils.GetBrushForShiftIndex(XBBindings.IsShiftCollection ? (XBBindings as ShiftXBBindingCollection).ShiftModificatorNum : 0);
            FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof(Grid));
            frameworkElementFactory.SetValue(FrameworkElement.MinHeightProperty, 10.0);
            FrameworkElementFactory frameworkElementFactory2 = new FrameworkElementFactory(typeof(ColumnDefinition));
            frameworkElementFactory2.SetValue(ColumnDefinition.WidthProperty, new GridLength(5.0, GridUnitType.Star));
            FrameworkElementFactory frameworkElementFactory3 = new FrameworkElementFactory(typeof(ColumnDefinition));
            frameworkElementFactory3.SetValue(ColumnDefinition.WidthProperty, new GridLength(280.0));
            frameworkElementFactory.AppendChild(frameworkElementFactory2);
            frameworkElementFactory.AppendChild(frameworkElementFactory3);
            FrameworkElementFactory frameworkElementFactory4 = new FrameworkElementFactory(typeof(Border));
            frameworkElementFactory4.SetValue(Grid.ColumnProperty, 0);
            frameworkElementFactory4.SetValue(Grid.ColumnSpanProperty, 3);
            frameworkElementFactory4.SetValue(Control.BorderThicknessProperty, new Thickness(0.0, 0.0, 0.0, 1.0));
            frameworkElementFactory4.SetValue(Control.BorderBrushProperty, TryFindResource("ContentBorderBrush"));
            frameworkElementFactory.AppendChild(frameworkElementFactory4);
            FrameworkElementFactory frameworkElementFactory5 = new FrameworkElementFactory(typeof(Button));
            frameworkElementFactory5.SetValue(FrameworkElement.NameProperty, "btnControllerBindings");
            frameworkElementFactory5.SetValue(FrameworkElement.MarginProperty, new Thickness(20.0, 5.0, 0.0, 0.0));
            frameworkElementFactory5.SetValue(FrameworkElement.WidthProperty, 200.0);
            frameworkElementFactory5.SetValue(Control.PaddingProperty, new Thickness(5.0));
            frameworkElementFactory5.SetValue(Control.BackgroundProperty, Brushes.Transparent);
            frameworkElementFactory5.SetValue(FrameworkElement.StyleProperty, TryFindResource("ButtonStripped"));
            frameworkElementFactory5.SetValue(Grid.ColumnProperty, 0);
            FrameworkElementFactory frameworkElementFactory6 = new FrameworkElementFactory(typeof(StackPanel));
            frameworkElementFactory6.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
            frameworkElementFactory6.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            frameworkElementFactory6.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);
            FrameworkElementFactory frameworkElementFactory7 = new FrameworkElementFactory(typeof(Grid));
            FrameworkElementFactory child = new FrameworkElementFactory(typeof(ColumnDefinition));
            frameworkElementFactory2.SetValue(ColumnDefinition.WidthProperty, default(GridLength));
            FrameworkElementFactory child2 = new FrameworkElementFactory(typeof(ColumnDefinition));
            frameworkElementFactory3.SetValue(ColumnDefinition.WidthProperty, new GridLength(1.0, GridUnitType.Star));
            frameworkElementFactory7.AppendChild(child);
            frameworkElementFactory7.AppendChild(child2);
            FrameworkElementFactory frameworkElementFactory8 = new FrameworkElementFactory(typeof(RecolorableSVG));
            frameworkElementFactory8.SetValue(Grid.ColumnProperty, 0);
            frameworkElementFactory8.SetValue(FrameworkElement.WidthProperty, 16.0);
            frameworkElementFactory8.SetValue(FrameworkElement.HeightProperty, 16.0);
            frameworkElementFactory8.SetValue(BaseRecolorableImage.IsColorShiftProperty, true);
            frameworkElementFactory8.SetValue(RecolorableSVG.ColorShiftBrushProperty, brushForShiftIndex);
            frameworkElementFactory8.SetValue(RecolorableSVG.DrawingProperty, new Binding("XBBinding.KeyScanCode")
            {
                Converter = new BaseRewasdMappingAnnotationIconConverter(),
                ConverterParameter = "combo"
            });
            frameworkElementFactory7.AppendChild(frameworkElementFactory8);
            FrameworkElementFactory frameworkElementFactory9 = new FrameworkElementFactory(typeof(TextBlock));
            frameworkElementFactory9.SetValue(Grid.ColumnProperty, 1);
            frameworkElementFactory9.SetValue(FrameworkElement.MinHeightProperty, 0.0);
            frameworkElementFactory9.SetValue(FrameworkElement.MarginProperty, new Thickness(8.0, 0.0, 0.0, 0.0));
            frameworkElementFactory9.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
            frameworkElementFactory9.SetValue(FrameworkElement.StyleProperty, TryFindResource("DTTextBlockStyle"));
            frameworkElementFactory9.SetValue(TextBlock.TextProperty, new Binding("XBBinding.KeyScanCode.FriendlyName"));
            frameworkElementFactory9.SetValue(TextBlock.TextWrappingProperty, TextWrapping.Wrap);
            frameworkElementFactory7.AppendChild(frameworkElementFactory9);
            frameworkElementFactory6.AppendChild(frameworkElementFactory7);
            frameworkElementFactory5.AppendChild(frameworkElementFactory6);
            frameworkElementFactory.AppendChild(frameworkElementFactory5);
            FrameworkElementFactory frameworkElementFactory10 = new FrameworkElementFactory(typeof(Button));
            frameworkElementFactory10.SetValue(FrameworkElement.NameProperty, "btnMaskMappings");
            frameworkElementFactory10.SetValue(FrameworkElement.MarginProperty, new Thickness(8.0, 5.0, 8.0, 5.0));
            frameworkElementFactory10.SetValue(Control.BackgroundProperty, Brushes.Transparent);
            frameworkElementFactory10.SetValue(FrameworkElement.StyleProperty, TryFindResource("ButtonStripped"));
            frameworkElementFactory10.SetValue(Grid.ColumnProperty, 1);
            FrameworkElementFactory frameworkElementFactory11 = new FrameworkElementFactory(typeof(StackPanel));
            frameworkElementFactory11.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            frameworkElementFactory11.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
            frameworkElementFactory11.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            FrameworkElementFactory frameworkElementFactory12 = new FrameworkElementFactory(typeof(XBBindingView));
            frameworkElementFactory12.SetValue(FrameworkElement.NameProperty, "ControllerBindingXBBindingView");
            frameworkElementFactory12.SetValue(XBBindingView.XBBindingProperty, new Binding("XBBinding"));
            frameworkElementFactory12.SetValue(Control.ForegroundProperty, brushForShiftIndex);
            DataTrigger dataTrigger = new DataTrigger
            {
                Binding = new Binding("IsInheritedBinding"),
                Value = true
            };
            dataTrigger.Setters.Add(new Setter(Control.ForegroundProperty, TryFindResource("DisabledControlForegroundColor")));
            frameworkElementFactory12.SetValue(value: new Style(typeof(XBBindingView))
            {
                Triggers = { (TriggerBase)dataTrigger }
            }, dp: FrameworkElement.StyleProperty);
            frameworkElementFactory11.AppendChild(frameworkElementFactory12);
            frameworkElementFactory10.AppendChild(frameworkElementFactory11);
            frameworkElementFactory.AppendChild(frameworkElementFactory10);
            return frameworkElementFactory;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            InitContainerGrid();
            UpdateLayout();
        }

        private void SVGGamepadWithAllAnnotations_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                InitContainerGrid();
            }
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
        public void InitializeComponent()
        {
            if (!_contentLoaded)
            {
                _contentLoaded = true;
                Uri resourceLocator = new Uri("/reWASD;component/controls/fastconfigrenders/keyboardmappingview.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            if (connectionId == 1)
            {
                ContentBorder = (Border)target;
            }
            else
            {
                _contentLoaded = true;
            }
        }
    }
}
