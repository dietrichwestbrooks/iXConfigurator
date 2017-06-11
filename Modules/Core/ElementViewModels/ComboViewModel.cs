using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Wayne.Payment.Products.iXConfigurator.Template;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Mvvm;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.ElementViewModels
{
    public class ComboViewModel : ControlViewModel<string>
    {
        private ComboControl _control;
        private BindableVariableExpression<string> _filterProperty;
        private string _filter;
        private ObservableCollection<ItemViewModel> _items = new ObservableCollection<ItemViewModel>();

        public ComboViewModel(string key, string label, string required, ComboControl control) 
            : base(key, label, required, control)
        {
            _control = control;

            _filterProperty = new BindableVariableExpression<string>(this, nameof(Filter));
        }

        public override void CreateElements()
        {
            base.CreateElements();

            _items.AddRange(_control.Items.Select(item => ElementFactory.CreateItem(item)));

            foreach (var item in _items)
            {
                item.CreateElements();
            }
        }

        public override void BindVariables()
        {
            base.BindVariables();

            _filterProperty.Bind(_control.Filter);
        }

        public override void InitializeVariables()
        {
            base.InitializeVariables();

            View = CollectionViewSource.GetDefaultView(_items);
            View.Filter = OnFilter;

            string value;

            if (Variables.TryGetVariableValue(Key, out value))
            {
                Value = value;
            }
            else
            {
                Variables.AddVariable(Key, _items.Select(item => item.Value).FirstOrDefault());

            }
        }

        public string Filter
        {
            get { return _filter; }
            set { SetProperty(ref _filter, value, OnFilterChanged); }
        }

        public ICollectionView View { get; private set; }

        protected override void OnValueChanged(string newValue)
        {
            base.OnValueChanged(newValue);

            if (string.IsNullOrWhiteSpace(newValue))
                return;

            foreach (var command in _items.Where(item => Equals(item.Value, newValue)).SelectMany(item => item.Commands))
            {
                if (command.CanExecute())
                {
                    command.Execute();
                }
            }
        }

        private void OnFilterChanged()
        {
            View?.Refresh();
            Value = View?.OfType<ItemViewModel>().Select(item => item.Value).FirstOrDefault();
        }

        private bool OnFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(Filter))
            {
                return true;
            }

            var vm = item as ItemViewModel;

            if (vm == null)
            {
                return false;
            }

            return vm.Filter == Filter;
        }
    }
}
