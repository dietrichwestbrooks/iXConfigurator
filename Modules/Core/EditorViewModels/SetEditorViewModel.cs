using System;
using System.Collections.Generic;
using System.Windows.Threading;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Extensions;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Utils;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels
{
    public class SetEditorViewModel : TaskEditorViewModel
    {
        private string _key;
        private string _value;

        public SetEditorViewModel()
        {
            Title = "Set Varaible Task";

            Key = "name1";
            Value = "Name1";

            Errors.Add(nameof(Key), new List<string>());
            Errors.Add(nameof(Value), new List<string>());
        }

        public string Key
        {
            get { return _key; }
            set { SetProperty(ref _key, value?.Trim()); }
        }

        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value?.Trim(), () => OnValueChanged(value?.Trim())); }
        }

        protected override void OnValidate(string propertyName)
        {
            if (Equals(propertyName, nameof(Key)))
            {
                if (string.IsNullOrWhiteSpace(Key))
                {
                    Errors[propertyName].Add($"{propertyName} is required");
                }

                if (!KeyNameConstraint.IsMatch(Key))
                {
                    Errors[propertyName].Add($"{propertyName} is invalid variable name");
                }
            }
            else if (Equals(propertyName, nameof(Value)))
            {
                if (string.IsNullOrWhiteSpace(Value))
                {
                    Errors[propertyName].Add($"{propertyName} is required");
                }
            }
        }

        protected override void OnUpdateSnippet()
        {
            Snippet = $"<set {GetCondition()} {GetKey()} {GetValue()} />"
                .FormattedXml();
        }

        protected string GetKey()
        {
            return $"key=\"{Key}\"";
        }

        protected string GetValue()
        {
            return $"value=\"{Value}\"";
        }

        private void OnValueChanged(string value)
        {
            try
            {
                if (EnumHelper.GetValueFromDescription<ExpandableEnum>(value) == ExpandableEnum.Build)
                {
                    var expandable = OpenExpandableBuilder();
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => Value = expandable));
                }
            }
            catch (InvalidOperationException)
            {
                // Do nothing
            }
        }
    }
}
