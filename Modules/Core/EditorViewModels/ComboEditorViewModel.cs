using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Threading;
using Prism.Commands;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Extensions;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Utils;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels
{
    public class ComboEditorViewModel : ControlEditorViewModel
    {
        private IEditorViewModelFactory _editorFactory;
        private string _filter;

        public ComboEditorViewModel(IEditorViewModelFactory editorFactory)
        {
            _editorFactory = editorFactory;

            Title = "ComboBox Control";

            AddItemCommand = new DelegateCommand(AddItem, () => true);
            RemoveItemCommand = new DelegateCommand<ItemEditorViewModel>(item => Items.Remove(item), item => Items.IndexOf(item) > 0);

            Items.CollectionChanged += (sender, args) => OnUpdateSnippet();

            AddItem();
        }

        public string Filter
        {
            get { return _filter; }
            set { SetProperty(ref _filter, value?.Trim(), () => OnFilterChanged(value?.Trim())); }
        }

        public ICommand AddItemCommand { get; }

        public ICommand RemoveItemCommand { get; }

        public ObservableCollection<ItemEditorViewModel> Items { get; } = new ObservableCollection<ItemEditorViewModel>();

        protected override void OnUpdateSnippet()
        {
            Snippet = $"<option {GetLabel()} {GetKey()} {GetVisible()} {GetRequired()}>\n\t<combo {GetEnabled()} {GetFilter()}>\n\t{GetItems()}\t</combo>\n</option>"
                .FormattedXml();
        }

        private string GetFilter()
        {
            return !string.IsNullOrWhiteSpace(Filter) ? $"filter=\"{Filter}\"" : string.Empty;
        }

        protected string GetItems()
        {
            var snippet = new StringBuilder();

            foreach (var item in Items)
            {
                snippet.AppendLine($"\t{item.Snippet}");
            }

            return snippet.ToString();
        }

        private void AddItem()
        {
            var value = "Item 1";

            for (var i = 1; i <= Items.Count; i++)
            {
                value = $"Item {i + 1}";

                if (Items.All(item => item.Value != value))
                {
                    break;
                }
            }

            var newItem = _editorFactory.CreateItemEditor(value);

            newItem.PropertyChanged += (sender, args) => OnUpdateSnippet();

            Items.Add(newItem);
        }

        private void OnFilterChanged(string filter)
        {
            try
            {
                if (EnumHelper.GetValueFromDescription<ExpandableEnum>(filter) == ExpandableEnum.Build)
                {
                    var expandable = OpenExpandableBuilder();
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => Filter = expandable));
                }
            }
            catch (InvalidOperationException)
            {
                // Do nothing
            }
        }
    }
}
