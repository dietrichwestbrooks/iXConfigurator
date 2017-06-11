using System.Collections.Generic;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Extensions;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels
{
    public class ItemEditorViewModel : SnippetEditorViewModelBase
    {
        private string _value;
        private string _name;

        public ItemEditorViewModel(string value)
        {
            Value = value;
            Name = value;

            Errors.Add(nameof(Value), new List<string>());
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value?.Trim()); }
        }

        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value?.Trim()); }
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
            Snippet = $"<item {GetName()} {GetValue()}>\n\t<!-- Add set/unset tasks here -->\n</item>"
                .FormattedXml();
        }

        protected string GetName()
        {
            return !string.IsNullOrWhiteSpace(Name) ? $"name=\"{Name}\"" : string.Empty;
        }

        protected string GetValue()
        {
            return $"value=\"{Value}\"";
        }
    }
}
