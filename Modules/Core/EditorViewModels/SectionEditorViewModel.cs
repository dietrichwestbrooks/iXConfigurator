using System.Collections.Generic;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Extensions;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels
{
    public class SectionEditorViewModel : ElementEditorViewModel
    {
        private string _sectionTitle;
        private string _description;

        public SectionEditorViewModel()
        {
            Title = "Section";

            SectionTitle = "Section 1";

            Errors.Add(nameof(SectionTitle), new List<string>());
        }

        public string SectionTitle
        {
            get { return _sectionTitle; }
            set { SetProperty(ref _sectionTitle, value); }
        }

        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        protected override void OnValidate(string propertyName)
        {
            if (Equals(propertyName, nameof(SectionTitle)))
            {
                if (string.IsNullOrWhiteSpace(SectionTitle))
                {
                    Errors[propertyName].Add($"{propertyName} is required");
                }
            }
        }

        protected override void OnUpdateSnippet()
        {
            Snippet = $"<section {GetTitle()} {GetVisible()}>\n{GetDescription()}\t<!-- Add options here -->\n</section>"
                .FormattedXml();
        }

        protected string GetTitle()
        {
            return $"title=\"{SectionTitle}\"";
        }

        protected string GetDescription()
        {
            return !string.IsNullOrWhiteSpace(Description) ? $"\t<description>{Description}</description>\n" : string.Empty;
        }
    }
}
