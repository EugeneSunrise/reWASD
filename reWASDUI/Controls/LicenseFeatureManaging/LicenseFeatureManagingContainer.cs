using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using DiscSoft.NET.Common.Utils;
using DiscSoft.NET.Common.Utils.Converters;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.View.RecolorableImages;
using Prism.Regions;
using reWASDUI.Infrastructure;
using reWASDUI.Views;

namespace reWASDUI.Controls.LicenseFeatureManaging
{
    public class LicenseFeatureManagingContainer : UserControl
    {
        public static readonly DependencyProperty AssociatedFeatureGUIDProperty = DependencyProperty.Register("AssociatedFeatureGUID", typeof(string), typeof(LicenseFeatureManagingContainer), new PropertyMetadata((object)null));

        public static readonly DependencyProperty IsFeatureUnlockedProperty = DependencyProperty.Register("IsFeatureUnlocked", typeof(bool), typeof(LicenseFeatureManagingContainer), new PropertyMetadata(false, IsFeatureUnlockedChangedCallback));

        public static readonly DependencyProperty VisibilityDueToPreferencesProperty = DependencyProperty.Register("VisibilityDueToPreferences", typeof(Visibility), typeof(LicenseFeatureManagingContainer), new PropertyMetadata(Visibility.Visible));

        public static readonly DependencyProperty AddLockForAllItemsFoundProperty = DependencyProperty.Register("AddLockForAllItemsFound", typeof(bool), typeof(LicenseFeatureManagingContainer), new PropertyMetadata(false));

        public static readonly DependencyProperty AddLockForWholeElementProperty = DependencyProperty.Register("AddLockForWholeElement", typeof(bool), typeof(LicenseFeatureManagingContainer), new PropertyMetadata(false));

        public static readonly DependencyProperty AddLockIconProperty = DependencyProperty.Register("AddLockIcon", typeof(bool), typeof(LicenseFeatureManagingContainer), new PropertyMetadata(true));

        private bool _firstLoadInited;

        public string AssociatedFeatureGUID
        {
            get
            {
                return (string)GetValue(AssociatedFeatureGUIDProperty);
            }
            set
            {
                SetValue(AssociatedFeatureGUIDProperty, value);
            }
        }

        public bool IsFeatureUnlocked
        {
            get
            {
                return (bool)GetValue(IsFeatureUnlockedProperty);
            }
            set
            {
                SetValue(IsFeatureUnlockedProperty, value);
            }
        }

        public Visibility VisibilityDueToPreferences
        {
            get
            {
                return (Visibility)GetValue(VisibilityDueToPreferencesProperty);
            }
            set
            {
                SetValue(VisibilityDueToPreferencesProperty, value);
            }
        }

        public bool AddLockForAllItemsFound
        {
            get
            {
                return (bool)GetValue(AddLockForAllItemsFoundProperty);
            }
            set
            {
                SetValue(AddLockForAllItemsFoundProperty, value);
            }
        }

        public bool AddLockForWholeElement
        {
            get
            {
                return (bool)GetValue(AddLockForWholeElementProperty);
            }
            set
            {
                SetValue(AddLockForWholeElementProperty, value);
            }
        }

        public bool AddLockIcon
        {
            get
            {
                return (bool)GetValue(AddLockIconProperty);
            }
            set
            {
                SetValue(AddLockIconProperty, value);
            }
        }

        private static void IsFeatureUnlockedChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LicenseFeatureManagingContainer)?.ReEvaluateVisibility();
        }

        public LicenseFeatureManagingContainer()
        {
            Initialize();
            base.Loaded += OnLoaded;
            base.IsVisibleChanged += OnIsVisibleChanged;
            base.PreviewMouseDown += LicenseFeatureManagingContainer_OnMouseDown;
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ReEvaluateVisibility();
            AddLockImage();
        }

        private void Initialize()
        {
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (ReEvaluateVisibility())
            {
                AddLockImage();
            }
        }

        private async void AddLockImage()
        {
            if (_firstLoadInited || !AddLockIcon)
            {
                return;
            }
            _firstLoadInited = true;
            if (AddLockForWholeElement)
            {
                AddLockForElement((FrameworkElement)base.Content);
                return;
            }
            List<TextBlock> list = VisualTreeHelperExt.FindChildren<TextBlock>((DependencyObject)base.Content);
            if (list == null || list.Count == 0)
            {
                await Task.Delay(50);
                list = VisualTreeHelperExt.FindChildren<TextBlock>((DependencyObject)base.Content);
            }
            if (list == null || list.Count == 0)
            {
                _firstLoadInited = false;
            }
            else if (AddLockForAllItemsFound)
            {
                foreach (TextBlock item in list)
                {
                    AddLockForTextBlock(item);
                }
            }
            else
            {
                AddLockForTextBlock(list.First());
            }
        }

        private void AddLockForElement(FrameworkElement el)
        {
            if (el != null)
            {
                RecolorableSVG element = CreateLockSVG();
                Grid grid2 = (Grid)(base.Content = new Grid());
                grid2.ColumnDefinitions.Add(new ColumnDefinition());
                grid2.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = GridLength.Auto
                });
                Grid.SetColumn(el, 0);
                Grid.SetColumn(element, 1);
                grid2.Children.Add(el);
                grid2.Children.Add(element);
            }
        }

        private void AddLockForTextBlock(TextBlock tb)
        {
            if (tb == null || VisualTreeHelperExt.FindParent<LicenseFeatureManagingContainer>(tb) != this)
            {
                return;
            }
            DependencyObject parent = VisualTreeHelper.GetParent(tb);
            RecolorableSVG recolorableSVG = CreateLockSVG();
            if (parent is StackPanel && ((StackPanel)parent).Orientation == Orientation.Horizontal)
            {
                ((StackPanel)parent).Children.Insert(((StackPanel)parent).Children.IndexOf(tb) + 1, recolorableSVG);
                return;
            }
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = GridLength.Auto
            });
            Grid.SetColumn(tb.Clone(), 0);
            Grid.SetColumn(recolorableSVG, 1);
            grid.Children.Add(tb.Clone());
            grid.Children.Add(recolorableSVG);
            if (parent is StackPanel)
            {
                StackPanel obj = (StackPanel)parent;
                int index = obj.Children.IndexOf(tb);
                VisualTreeHelperExt.RemoveChild(parent, tb);
                obj.Children.Insert(index, grid);
                recolorableSVG.Margin = new Thickness(0.0);
            }
            else
            {
                VisualTreeHelperExt.RemoveChild(parent, tb);
                VisualTreeHelperExt.AddChild(parent, grid);
            }
        }

        private RecolorableSVG CreateLockSVG()
        {
            RecolorableSVG obj = new RecolorableSVG
            {
                Drawing = (Application.Current.TryFindResource("AddFeature") as Drawing),
                Margin = new Thickness(3.0, 2.0, 0.0, 0.0),
                ColorShiftBrush = new SolidColorBrush(Colors.White),
                IsColorShift = true,
                DisabledOpacity = 0.6,
                VerticalAlignment = VerticalAlignment.Center
            };
            BindingOperations.SetBinding(obj, UIElement.VisibilityProperty, new Binding("IsFeatureUnlocked")
            {
                Source = this,
                Converter = new DiscSoft.NET.Common.Utils.Converters.BooleanToVisibilityConverter(),
                ConverterParameter = "invert"
            });
            return obj;
        }

        private bool ReEvaluateVisibility()
        {
            bool flag = Convert.ToBoolean(RegistryHelper.GetValue("Config", "HideLockedFeatures", 0));
            if (!IsFeatureUnlocked && flag)
            {
                VisibilityDueToPreferences = Visibility.Collapsed;
                return false;
            }
            VisibilityDueToPreferences = Visibility.Visible;
            return true;
        }

        private void LicenseFeatureManagingContainer_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsFeatureUnlocked)
            {
                e.Handled = true;
                Dictionary<object, object> dictionary = new Dictionary<object, object>();
                dictionary.Add("navigatePath", typeof(LicenseMain));
                NavigationParameters navigationParameters = new NavigationParameters();
                navigationParameters.Add("tab", AssociatedFeatureGUID);
                dictionary.Add("NavigationParameters", navigationParameters);
                reWASDApplicationCommands.NavigateContentCommand.Execute(dictionary);
            }
        }
    }
}
