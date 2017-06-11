using System.Collections.Generic;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Extensions;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels
{
    public class PageEditorViewModel : ElementEditorViewModel
    {
        private string _pageTitle;
        private string _description;
        private string _summary;

        public PageEditorViewModel()
        {
            Title = "Page";

            PageTitle = "Page 1";

            Errors.Add(nameof(PageTitle), new List<string>());
        }

        public string PageTitle
        {
            get { return _pageTitle; }
            set { SetProperty(ref _pageTitle, value); }
        }

        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        public string Summary
        {
            get { return _summary; }
            set { SetProperty(ref _summary, value); }
        }

        protected override void OnValidate(string propertyName)
        {
            if (Equals(propertyName, nameof(PageTitle)))
            {
                if (string.IsNullOrWhiteSpace(PageTitle))
                {
                    Errors[propertyName].Add($"{propertyName} is required");
                }
            }
        }

        protected override void OnUpdateSnippet()
        {
            Snippet = $"<page {GetTitle()} {GetVisible()}>\n{GetDescription()}\t<!-- Add sections or options here -->\n{GetSummary()}</page>"
                .FormattedXml();
        }

        protected string GetTitle()
        {
            return $"title=\"{PageTitle}\"";
        }

        protected string GetDescription()
        {
            return !string.IsNullOrWhiteSpace(Description) ? $"\t<description>{Description}</description>\n" : string.Empty;
        }

        protected string GetSummary()
        {
            return !string.IsNullOrWhiteSpace(Description) ? $"\t<summary>{Summary}</summary>\n" : string.Empty;
        }
    }
}
