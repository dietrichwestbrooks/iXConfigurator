using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using Wayne.Payment.Products.iXConfigurator.Template;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.ElementViewModels
{
    public class ElementViewModelFactory : IElementViewModelFactory
    {
        private IUnityContainer _container;

        private const string Element = "Element";
        private const string Section = "Section";

        public ElementViewModelFactory(IUnityContainer container)
        {
            _container = container;

            container.RegisterType<SectionViewModel>(Element, new InjectionConstructor(
                typeof(Section)));

            container.RegisterType<SectionViewModel>(Section, new InjectionConstructor(
                typeof(string),
                typeof(IEnumerable<Option>)));
        }

        public ConfigurationViewModel CreateConfiguration(string key, Configuration configuration)
        {
            return _container.Resolve<ConfigurationViewModel>(
                new ParameterOverride(nameof(key), key),
                new DependencyOverride(typeof(Configuration), configuration));
        }

        public ProductViewModel CreateProduct(Product product)
        {
            return _container.Resolve<ProductViewModel>( 
                new DependencyOverride(typeof(Product), product));
        }

        public PageViewModel CreatePage(Page page)
        {
            return _container.Resolve<DefaultPageViewModel>( 
                new DependencyOverride(typeof(Page), page));
        }

        public PageViewModel CreateSummaryPage(IEnumerable<DefaultPageViewModel> pages)
        {
            return _container.Resolve<SummaryPageViewModel>(
                new ParameterOverride(nameof(pages), pages));
        }

        public SectionViewModel CreateSection(Section section)
        {
            return _container.Resolve<SectionViewModel>(Element,
                new DependencyOverride(typeof(Section), section));
        }

        public SectionViewModel CreateSection(string title, IEnumerable<Option> options)
        {
            return _container.Resolve<SectionViewModel>(Section,
                new ParameterOverride(nameof(title), title),
                new ParameterOverride(nameof(options), options));
        }

        public OptionViewModel CreateOption(Option option)
        {
            return _container.Resolve<OptionViewModel>(
                new ParameterOverride(nameof(option), option));
        }

        public SummaryOptionViewModel CreateSummaryOption(OptionViewModel option)
        {
            return _container.Resolve<SummaryOptionViewModel>(
                new ParameterOverride(nameof(option), option));
        }

        public ControlViewModel CreateControl(string key, string label, string required, Control control)
        {
            ControlViewModel newControl;

            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            if (control is CheckControl)
            {
                newControl = _container.Resolve<CheckViewModel>(
                    new ParameterOverride(nameof(key), key),
                    new ParameterOverride(nameof(label), label),
                    new ParameterOverride(nameof(required), required),
                    new DependencyOverride(typeof (CheckControl), control));
            }
            else if (control is ComboControl)
            {
                newControl = _container.Resolve<ComboViewModel>(
                    new ParameterOverride(nameof(key), key),
                    new ParameterOverride(nameof(label), label),
                    new ParameterOverride(nameof(required), required),
                    new DependencyOverride(typeof (ComboControl), control));
            }
            else if (control is ListControl)
            {
                newControl = _container.Resolve<ListViewModel>(
                    new ParameterOverride(nameof(key), key),
                    new ParameterOverride(nameof(label), label),
                    new ParameterOverride(nameof(required), required),
                    new DependencyOverride(typeof (ListControl), control));
            }
            else if (control is RadioControl)
            {
                newControl = _container.Resolve<RadioViewModel>(
                    new ParameterOverride(nameof(key), key),
                    new ParameterOverride(nameof(label), label),
                    new ParameterOverride(nameof(required), required),
                    new DependencyOverride(typeof (RadioControl), control));
            }
            else if (control is TextControl)
            {
                newControl = _container.Resolve<TextViewModel>(
                    new ParameterOverride(nameof(key), key),
                    new ParameterOverride(nameof(label), label),
                    new ParameterOverride(nameof(required), required),
                    new DependencyOverride(typeof (TextControl), control));
            }
            else if (control is TableControl)
            {
                newControl = _container.Resolve<TableViewModel>(
                    new ParameterOverride(nameof(key), key),
                    new ParameterOverride(nameof(label), label),
                    new ParameterOverride(nameof(required), required),
                    new DependencyOverride(typeof (TableControl), control));
            }
            else
            {
                throw new ArgumentOutOfRangeException(control.GetType().Name);
            }

            return newControl;
        }

        public ItemViewModel CreateItem(Item item)
        {
            return _container.Resolve<ItemViewModel>(
                new ParameterOverride(nameof(item), item));
        }

        public ChoiceViewModel CreateChoice(Choice choice)
        {
            return _container.Resolve<ChoiceViewModel>(
                new ParameterOverride(nameof(choice), choice));
        }
    }
}
