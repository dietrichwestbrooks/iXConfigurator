using Wayne.Payment.Products.iXConfigurator.Template;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.ElementViewModels
{
    public class ProductViewModel : ElementViewModel
    {
        private string _name;
        private string _version;
        private string _description;

        public ProductViewModel(Product product)
        {
            Name = product.Name;
            Version = product.Version;
            Description = product.Description;
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string Version
        {
            get { return _version; }
            set { SetProperty(ref _version, value); }
        }

        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }
    }
}