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
    public class RadioEditorViewModel : ControlEditorViewModel
    {
        private IEditorViewModelFactory _editorFactory;
        private string _filter;

        public RadioEditorViewModel(IEditorViewModelFactory editorFactory)
        {
            _editorFactory = editorFactory;

            Title = "RadioButton Control";

            AddChoiceCommand = new DelegateCommand(AddChoice, () => true);
            RemoveChoiceCommand = new DelegateCommand<ChoiceEditorViewModel>(choice => Choices.Remove(choice), choice => Choices.IndexOf(choice) > 0);

            Choices.CollectionChanged += (sender, args) => OnUpdateSnippet();

            AddChoice();
        }

        public string Filter
        {
            get { return _filter; }
            set { SetProperty(ref _filter, value?.Trim(), () => OnFilterChanged(value?.Trim())); }
        }

        public ICommand AddChoiceCommand { get; }

        public ICommand RemoveChoiceCommand { get; }

        public ObservableCollection<ChoiceEditorViewModel> Choices { get; } = new ObservableCollection<ChoiceEditorViewModel>();

        protected override void OnUpdateSnippet()
        {
            Snippet = $"<option {GetLabel()} {GetKey()} {GetVisible()} {GetRequired()}>\n\t<radio {GetEnabled()} {GetFilter()}>\n\t{GetChoices()}\t</radio>\n</option>"
                .FormattedXml();
        }

        private string GetFilter()
        {
            return !string.IsNullOrWhiteSpace(Filter) ? $"filter=\"{Filter}\"" : string.Empty;
        }

        protected string GetChoices()
        {
            var snippet = new StringBuilder();

            foreach (var choice in Choices)
            {
                snippet.AppendLine($"\t{choice.Snippet}");
            }

            return snippet.ToString();
        }

        private void AddChoice()
        {
            var value = "Choice 1";

            for (var i = 1; i <= Choices.Count; i++)
            {
                value = $"Choice {i + 1}";

                if (Choices.All(choice => choice.Value != value))
                {
                    break;
                }
            }

            var newChoice = _editorFactory.CreateChoiceEditor(value);

            newChoice.PropertyChanged += (sender, args) => OnUpdateSnippet();

            Choices.Add(newChoice);
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
