using Prism.Mvvm;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Mvvm
{
    public abstract class ViewModelBase : BindableBase
    {
        private string _title;

        public string Title
        {
            get { return _title; }
            protected set { SetProperty(ref _title, value); }
        }
    }
}
