using System.Collections.Generic;
using Wayne.Payment.Products.iXConfigurator.Template;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.ElementViewModels
{
    public interface IElementViewModelFactory
    {
        ConfigurationViewModel CreateConfiguration(string key, Configuration configuration);

        ProductViewModel CreateProduct(Product product);

        PageViewModel CreatePage(Page page);

        PageViewModel CreateSummaryPage(IEnumerable<DefaultPageViewModel> pages);

        SectionViewModel CreateSection(Section section);

        SectionViewModel CreateSection(string title, IEnumerable<Option> options);

        OptionViewModel CreateOption(Option option);

        SummaryOptionViewModel CreateSummaryOption(OptionViewModel option);

        ControlViewModel CreateControl(string key, string label, string required, Control control);

        ItemViewModel CreateItem(Item item);

        ChoiceViewModel CreateChoice(Choice choice);
    }
}
