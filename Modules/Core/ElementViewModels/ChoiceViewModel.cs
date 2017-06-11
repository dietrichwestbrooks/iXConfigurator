using System.Collections.ObjectModel;
using System.Linq;
using Wayne.Payment.Products.iXConfigurator.Template;
using Wayne.Payment.Products.iXConfigurator.Modules.Core.Commands;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.ElementViewModels
{
    public class ChoiceViewModel : ElementViewModel
    {
        private Choice _choice;
        private string _name;
        private string _value;
        private string _filter;

        public ChoiceViewModel(Choice choice)
        {
            _choice = choice;

            _value = _choice.Value;
            _name = string.IsNullOrWhiteSpace(_choice.Name) ? _choice.Value : _choice.Name;
            _filter = _choice.Filter;
        }

        public override void CreateElements()
        {
            base.CreateElements();

            Commands.AddRange(_choice.Tasks.Select(CommandFactory.CreateCommand));
        }

        public ObservableCollection<TaskCommand> Commands { get; } = new ObservableCollection<TaskCommand>();

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        public string Filter
        {
            get { return _filter; }
            set { SetProperty(ref _filter, value); }
        }
    }
}
