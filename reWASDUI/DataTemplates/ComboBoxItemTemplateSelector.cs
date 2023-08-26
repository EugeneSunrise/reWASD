using System.Windows;
using System.Windows.Controls;

namespace reWASDUI.DataTemplates
{
    public class ComboBoxItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SelectedItemTemplate { get; set; }

        public DataTemplate ItemTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            bool flag = false;
            FrameworkElement frameworkElement = container as FrameworkElement;
            if (frameworkElement != null)
            {
                DependencyObject templatedParent = frameworkElement.TemplatedParent;
                if (templatedParent != null && templatedParent is ComboBox)
                {
                    flag = true;
                }
            }
            if (flag)
            {
                return SelectedItemTemplate;
            }
            return ItemTemplate;
        }
    }
}
