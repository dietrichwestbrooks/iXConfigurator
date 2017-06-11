using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Wayne.Payment.Products.iXConfigurator.Template;
using Wayne.Payment.Products.iXConfigurator.Modules.Core.Commands;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.ElementViewModels
{
    public class CheckViewModel : ControlViewModel<bool?>
    {
        private CheckControl _control;
        private List<TaskCommand> _checkedCommands = new List<TaskCommand>();
        private List<TaskCommand> _uncheckedCommands = new List<TaskCommand>();

        public CheckViewModel(string key, string label, string required, CheckControl control)
            : base(key, label, required, control)
        {
            _control = control;
        }

        public override void CreateElements()
        {
            base.CreateElements();

            CheckedOptions.AddRange(_control.Checked.Options.Select(ElementFactory.CreateOption));
            UncheckedOptions.AddRange(_control.Unchecked.Options.Select(ElementFactory.CreateOption));

            _uncheckedCommands.AddRange(_control.Unchecked.Tasks.Select(CommandFactory.CreateCommand));
            _checkedCommands.AddRange(_control.Checked.Tasks.Select(CommandFactory.CreateCommand));

            foreach (var option in CheckedOptions)
            {
                option.CreateElements();
            }

            foreach (var option in UncheckedOptions)
            {
                option.CreateElements();
            }
        }

        public override void BindVariables()
        {
            base.BindVariables();

            foreach (var option in CheckedOptions)
            {
                option.BindVariables();
            }

            foreach (var option in UncheckedOptions)
            {
                option.BindVariables();
            }
        }

        public override void InitializeVariables()
        {
            base.InitializeVariables();

            bool value;

            if (Variables.TryGetVariableValue(Key, out value))
            {
                Value = value;
            }
            else
            {
                Variables.AddVariable(Key, _control.Default);
            }

            foreach (var option in CheckedOptions)
            {
                option.InitializeVariables();
            }

            foreach (var option in UncheckedOptions)
            {
                option.InitializeVariables();
            }
        }

        public override bool Validate()
        {
            var isValid = true;

            if (!IsVisible)
                return true;

            IEnumerable<OptionViewModel> options = Value.GetValueOrDefault() ? CheckedOptions : UncheckedOptions;

            foreach (var option in options)
            {
                if (!option.Validate())
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public bool HasCheckedOptions => CheckedOptions.Any();

        public bool HasUncheckedOptions => UncheckedOptions.Any();

        public ObservableCollection<OptionViewModel> CheckedOptions { get; } = new ObservableCollection<OptionViewModel>();

        public ObservableCollection<OptionViewModel> UncheckedOptions { get; } = new ObservableCollection<OptionViewModel>();

        protected override void OnValueChanged(bool? newValue)
        {
            base.OnValueChanged(newValue);

            IEnumerable<TaskCommand> commands = newValue.GetValueOrDefault() ? _checkedCommands : _uncheckedCommands;

            foreach (var command in commands)
            {
                if (command.CanExecute())
                {
                    command.Execute();
                }
            }
        }
    }
}
