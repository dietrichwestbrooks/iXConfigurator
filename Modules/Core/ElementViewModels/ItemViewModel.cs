using System.Collections.ObjectModel;
using System.Linq;
using Wayne.Payment.Products.iXConfigurator.Template;
using Wayne.Payment.Products.iXConfigurator.Modules.Core.Commands;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.ElementViewModels
{
    public class ItemViewModel : ElementViewModel
    {
        private Item _item;
        private string _name;
        private string _value;
        private string _filter;

        public ItemViewModel(Item item)
        {
            _item = item;

            _value = _item.Value;
            _name = string.IsNullOrWhiteSpace(_item.Name) ? _item.Value : _item.Name;
            _filter = _item.Filter;
        }

        public override void CreateElements()
        {
            base.CreateElements();

            Commands.AddRange(_item.Tasks.Select(CommandFactory.CreateCommand));
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
