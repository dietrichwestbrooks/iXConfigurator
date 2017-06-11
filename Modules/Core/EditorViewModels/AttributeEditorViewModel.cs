using System;
using System.Collections.Generic;
using System.Windows.Threading;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Extensions;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Utils;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels
{
    public class AttributeEditorViewModel : SnippetEditorViewModelBase
    {
        private string _value;

        public AttributeEditorViewModel(string value)
        {
            Value = value;

            Errors.Add(nameof(Value), new List<string>());
        }

        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value?.Trim(), () => OnValueChanged(value?.Trim())); }
        }

        protected override void OnValidate(string propertyName)
        {
            if (Equals(propertyName, nameof(Value)))
            {
                if (string.IsNullOrWhiteSpace(Value))
                {
                    Errors[propertyName].Add($"{propertyName} is required");
                }
            }
        }

        protected override void OnUpdateSnippet()
        {
            Snippet = $"<attribute>{GetValue()}</attribute>"
                .FormattedXml();
        }

        protected string GetValue()
        {
            return Value;
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
