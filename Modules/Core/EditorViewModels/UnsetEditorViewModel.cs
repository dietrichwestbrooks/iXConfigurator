using System.Collections.Generic;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Extensions;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels
{
    public class UnsetEditorViewModel : TaskEditorViewModel
    {
        private string _key;

        public UnsetEditorViewModel()
        {
            Title = "Unset Varaible Task";

            Key = "name1";

            Errors.Add(nameof(Key), new List<string>());
        }

        public string Key
        {
            get { return _key; }
            set { SetProperty(ref _key, value?.Trim()); }
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
        }

        protected override void OnUpdateSnippet()
        {
            Snippet = $"<unset {GetCondition()} {GetKey()} />"
                .FormattedXml();
        }

        protected string GetKey()
        {
            return $"key=\"{Key}\"";
        }
    }
}
