using Wayne.Payment.Products.iXConfigurator.Infrastructure.Extensions;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels
{
    public class TableEditorViewModel : ControlEditorViewModel
    {
        private bool _editable;
        private string _value;

        public TableEditorViewModel()
        {
            Title = "Table Control";

            Editable = true;
        }

        public bool Editable
        {
            get { return _editable; }
            set { SetProperty(ref _editable, value); }
        }

        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        protected override void OnUpdateSnippet()
        {
            Snippet = $"<option {GetLabel()} {GetKey()} {GetVisible()} {GetRequired()}>\n\t<table {GetReadOnly()}>\n\t{GetValue()}\t</table>\n</option>"
                .FormattedXml();
        }

        protected string GetValue()
        {
            return !string.IsNullOrWhiteSpace(Value) ? $"\t{Value}\n" : "\t<!-- Add table data here -->\n";
        }

        protected string GetReadOnly()
        {
            return $"editable=\"{Editable.ToString().ToLower()}\"";
        }
    }
}
