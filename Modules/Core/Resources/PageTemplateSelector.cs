using System.Windows;
using System.Windows.Controls;
using Wayne.Payment.Products.iXConfigurator.Modules.Core.ElementViewModels;
using Wayne.Payment.Products.iXConfigurator.Modules.Core.Views;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Resources
{
    public class PageTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var templateKey = "DefaultPageViewTemplate";

            if (item is SummaryPageViewModel)
            {
                templateKey = "SummaryPageViewTemplate";
            }

            var element = container as FrameworkElement;

            return element?.TryFindResource(templateKey) as DataTemplate;
        }
    }
}
