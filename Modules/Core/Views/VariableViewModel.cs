using Wayne.Payment.Products.iXConfigurator.Infrastructure.Mvvm;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Views
{
    public class VariableViewModel : ViewModelBase
    {
        private string _key;
        private object _value;

        public VariableViewModel(string key)
        {
            Key = key;

            var valueProperty = new BindableVariableProperty<object>(this, nameof(Value));
            valueProperty.BindKey(_key);
        }

        public string Key
        {
            get { return _key; }
            set { SetProperty(ref _key, value); }
        }

        public object Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }
    }
}
