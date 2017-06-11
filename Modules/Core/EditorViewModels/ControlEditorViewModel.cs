using System;
using System.Collections.Generic;
using System.Windows.Threading;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Utils;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels
{
    public abstract class ControlEditorViewModel : ElementEditorViewModel
    {
        private string _key;
        private string _label;
        private string _enabled;
        private string _required;

        protected ControlEditorViewModel()
        {
            Key = "name1";
            Label = "Name 1:";

            Errors.Add(nameof(Key), new List<string>());
            Errors.Add(nameof(Label), new List<string>());
            Errors.Add(nameof(Enabled), new List<string>());
            Errors.Add(nameof(Required), new List<string>());
        }

        public string Key
        {
            get { return _key; }
            set { SetProperty(ref _key, value?.Trim()); }
        }

        public string Label
        {
            get { return _label; }
            set { SetProperty(ref _label, value); }
        }

        public string Enabled
        {
            get { return _enabled; }
            set { SetProperty(ref _enabled, value?.Trim(), () => OnEnabledChanged(value?.Trim())); }
        }

        public string Required
        {
            get { return _required; }
            set { SetProperty(ref _required, value?.Trim(), () => OnRequiredChanged(value?.Trim())); }
        }

        protected virtual void OnEnabledChanged(string enabled)
        {
            try
            {
                if (EnumHelper.GetValueFromDescription<BooleanExpressionEnum>(enabled) == BooleanExpressionEnum.Build)
                {
                    var expression = OpenExpressionBuilder();
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => Enabled = expression));
                }
            }
            catch (InvalidOperationException)
            {
                // Do Nothing
            }
        }

        protected virtual void OnRequiredChanged(string required)
        {
            try
            {
                if (EnumHelper.GetValueFromDescription<BooleanExpressionEnum>(required) == BooleanExpressionEnum.Build)
                {
                    var expression = OpenExpressionBuilder();
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => Required = expression));
                }
            }
            catch (InvalidOperationException)
            {
                // Do Nothing
            }
        }

        protected string GetKey()
        {
            return $"key=\"{Key}\"";
        }

        protected string GetLabel()
        {
            return $"label=\"{Label}\"";
        }

        protected string GetEnabled()
        {
            return !string.IsNullOrWhiteSpace(Enabled) ? $"enabled=\"{BooleanToLower(Enabled)}\"" : string.Empty;
        }

        protected string GetRequired()
        {
            return !string.IsNullOrWhiteSpace(Required) ? $"required=\"{BooleanToLower(Required)}\"" : string.Empty;
        }

        protected override void OnValidate(string propertyName)
        {
            if (Equals(propertyName, nameof(Key)))
            {
                if (string.IsNullOrWhiteSpace(Key))
                {
                    Errors[propertyName].Add($"{propertyName} is a required");
                }

                if (!KeyNameConstraint.IsMatch(Key))
                {
                    Errors[propertyName].Add($"{propertyName} is invalid variable name");
                }
            }
            else if (Equals(propertyName, nameof(Label)))
            {
                if (string.IsNullOrWhiteSpace(Label))
                {
                    Errors[propertyName].Add($"{propertyName} is a required");
                }
            }
        }
    }
}
