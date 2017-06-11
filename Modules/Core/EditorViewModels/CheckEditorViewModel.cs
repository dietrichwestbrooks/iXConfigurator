using Wayne.Payment.Products.iXConfigurator.Infrastructure.Extensions;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels
{
    public class CheckEditorViewModel : ControlEditorViewModel
    {
        public CheckEditorViewModel()
        {
            Title = "CheckBox Control";
        }

        private bool _default;

        public bool Default
        {
            get { return _default; }
            set { SetProperty(ref _default, value); }
        }

        protected override void OnUpdateSnippet()
        {
            Snippet = $"<option {GetLabel()} {GetKey()} {GetVisible()} {GetRequired()}>\n\t<check {GetEnabled()} {GetDefault()}>\n\t{GetValue()}\t</check>\n</option>"
                .FormattedXml();
        }

        protected string GetDefault()
        {
            return _default ? $"default=\"{Default.ToString().ToLower()}\"" : string.Empty;
        }

        private string GetValue()
        {
            return "\t<checked>\n\t\t\t<!-- Add options and/or tasks here -->\n\t\t</checked>\n\t\t<unchecked>\n\t\t\t<!-- Add options and/or tasks here -->\n\t\t</unchecked>\n";
        }
    }
}
