using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Wayne.Payment.Products.iXConfigurator.Template;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Mvvm;
using Orientation = Wayne.Payment.Products.iXConfigurator.Infrastructure.Constants.Orientation;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.ElementViewModels
{
    public class RadioViewModel : ControlViewModel<string>
    {
        private RadioControl _control;
        private Orientation _orientation;
        private BindableVariableExpression<string> _filterProperty;
        private string _filter;
        private ObservableCollection<ChoiceViewModel> _choices = new ObservableCollection<ChoiceViewModel>();

        public RadioViewModel(string key, string label, string required, RadioControl control)
            : base(key, label, required, control)
        {
            _control = control;

            Orientation = _control.Orientation == Template.Orientation.Horizontal
                ? Orientation.Horizontal
                : Orientation.Vertical;

            _filterProperty = new BindableVariableExpression<string>(this, nameof(Filter));
        }

        public override void CreateElements()
        {
            base.CreateElements();

            _choices.AddRange(_control.Choices.Select(choice => ElementFactory.CreateChoice(choice)));

            foreach (var choice in _choices)
            {
                choice.CreateElements();
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

            View = CollectionViewSource.GetDefaultView(_choices);
            View.Filter = OnFilter;

            string value;

            if (Variables.TryGetVariableValue(Key, out value))
            {
                Value = value;
            }
            else
            {
                Variables.AddVariable(Key, _choices.Select(choice => choice.Value).FirstOrDefault());

            }
        }

        public Orientation Orientation
        {
            get { return _orientation; }
            set { SetProperty(ref _orientation, value); }
        }

        public string Filter
        {
            get { return _filter; }
            set { SetProperty(ref _filter, value, OnFilterChanged); }
        }

        public ICollectionView View { get; private set; }

        protected  override void OnValueChanged(string newValue)
        {
            base.OnValueChanged(newValue);

            if (string.IsNullOrWhiteSpace(newValue))
                return;

            foreach (var command in _choices.Where(choice => Equals(choice.Value, newValue)).SelectMany(choice => choice.Commands))
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
