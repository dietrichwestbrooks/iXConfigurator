using System.Collections.ObjectModel;
using System.Linq;
using Wayne.Payment.Products.iXConfigurator.Template;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.ElementViewModels
{
    public class ConfigurationViewModel : ElementViewModel
    {
        private string _libraryPath;
        private string _key;
        private ProductViewModel _product;
        private Configuration _template;

        public ConfigurationViewModel(string key, Configuration template)
        {
            _key = key;
            _template = template;

            LibraryPath = _template.LibraryPath;
        }

        public void Initialize()
        {
            CreateElements();
            BindVariables();
            InitializeVariables();
        }

        public override void CreateElements()
        {
            base.CreateElements();

            Product = ElementFactory.CreateProduct(_template.Product);

            Pages.AddRange(_template.Pages.Select(ElementFactory.CreatePage));

            Pages.Add(ElementFactory.CreateSummaryPage(Pages.OfType<DefaultPageViewModel>()));

            foreach (var page in Pages)
            {
                page.CreateElements();
            }
        }

        public override void BindVariables()
        {
            base.BindVariables();

            foreach (var page in Pages)
            {
                page.BindVariables();
            }
        }

        public override void InitializeVariables()
        {
            base.InitializeVariables();

            foreach (var page in Pages)
            {
                page.InitializeVariables();
            }
        }

        public string Key
        {
            get { return _key; }
            set { SetProperty(ref _key, value); }
        }

        public string LibraryPath
        {
            get { return _libraryPath; }
            set { SetProperty(ref _libraryPath, value); }
        }

        public ProductViewModel Product
        {
            get { return _product; }
            set { SetProperty(ref _product, value); }
        }

        public ObservableCollection<PageViewModel> Pages { get; } = new ObservableCollection<PageViewModel>();

        public override void Dispose()
        {
            base.Dispose();

            foreach (var page in Pages)
            {
                page.Dispose();
            }
        }
    }
}
