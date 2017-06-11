using Wayne.Payment.Products.iXConfigurator.Infrastructure.Extensions;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels
{
    public class TextEditorViewModel : ControlEditorViewModel
    {
        private string _value;
        private string _pattern;
        private TextRestrictionEnum? _restriction;
        private string _hint;

        public TextEditorViewModel()
        {
            Title = "TextBox Control";

            Restriction = TextRestrictionEnum.None;
        }

        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        public string Hint
        {
            get { return _hint; }
            set { SetProperty(ref _hint, value?.Trim()); }
        }

        public string Pattern
        {
            get { return _pattern; }
            set { SetProperty(ref _pattern, value?.Trim()); }
        }

        public TextRestrictionEnum? Restriction
        {
            get { return _restriction; }
            set { SetProperty(ref _restriction, value); }
        }

        protected override void OnUpdateSnippet()
        {
            Snippet = $"<option {GetLabel()} {GetKey()} {GetVisible()} {GetRequired()}>\n\t<text {GetEnabled()} {GetHint()} {GetValue()} {GetPattern()} {GetRestriction()} />\n</option>"
                .FormattedXml();
        }

        protected string GetHint()
        {
            return !string.IsNullOrWhiteSpace(Hint) ? $"hint=\"{Hint}\"" : string.Empty;
        }

        protected string GetValue()
        {
            return !string.IsNullOrWhiteSpace(Value) ? $"value=\"{Value}\"" : string.Empty;
        }

        protected string GetPattern()
        {
            return !string.IsNullOrWhiteSpace(Pattern) ? $"pattern=\"{Pattern}\"" : string.Empty;
        }

        protected string GetRestriction()
        {
            return Restriction != TextRestrictionEnum.None ? $"restriction=\"{Restriction?.ToString().ToLower()}\"" : string.Empty;
        }
    }
}
