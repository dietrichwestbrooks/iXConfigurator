using System.Collections.ObjectModel;
using System.Linq;
using Wayne.Payment.Products.iXConfigurator.Template;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.ElementViewModels
{
    public class DefaultPageViewModel : PageViewModel
    {
        private Page _page;
        private string _description;
        private string _summary;

        public DefaultPageViewModel(Page page)
            : base(page)
        {
            _page = page;

            Description = _page.Description;
            Summary = _page.Summary;
        }

        public override void CreateElements()
        {
            base.CreateElements();

            Sections.AddRange(_page.Sections.Select(ElementFactory.CreateSection));

            if (!Sections.Any())
            {
                Sections.Add(ElementFactory.CreateSection(_page.Title, _page.Options));
            }

            foreach (var section in Sections)
            {
                section.CreateElements();
            }
        }

        public override void BindVariables()
        {
            base.BindVariables();

            foreach (var section in Sections)
            {
                section.BindVariables();
            }
        }

        public override void InitializeVariables()
        {
            base.InitializeVariables();

            foreach (var section in Sections)
            {
                section.InitializeVariables();
            }
        }

        public override bool Validate()
        {
            var isValid = true;

            foreach (var section in Sections)
            {
                if (!section.Validate())
                {
                    isValid = false;
                }
            }

            return isValid;
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

        public ObservableCollection<SectionViewModel> Sections { get; } = new ObservableCollection<SectionViewModel>();

        public override void Dispose()
        {
            base.Dispose();

            foreach (var section in Sections)
            {
                section.Dispose();
            }
        }
    }
}
