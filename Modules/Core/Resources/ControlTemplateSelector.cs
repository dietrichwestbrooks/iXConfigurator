using System.Windows;
using System.Windows.Controls;
using Wayne.Payment.Products.iXConfigurator.Modules.Core.ElementViewModels;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Resources
{
    public class ControlTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var templateKey = string.Empty;

            if (item is CheckViewModel)
            {
                templateKey = "CheckViewTemplate";
            }
            else if (item is ComboViewModel)
            {
                templateKey = "ComboViewTemplate";
            }
            else if (item is ListViewModel)
            {
                templateKey = "ListViewTemplate";
            }
            else if (item is RadioViewModel)
            {
                templateKey = "RadioViewTemplate";
            }
            else if (item is TableViewModel)
            {
                templateKey = "TableViewTemplate";
            }
            else if (item is TextViewModel)
            {
                templateKey = "TextViewTemplate";
            }

            var element = container as FrameworkElement;

            return element?.TryFindResource(templateKey) as DataTemplate;
        }
    }
}
