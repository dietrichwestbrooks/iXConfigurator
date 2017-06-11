using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Wayne.Payment.Products.iXConfigurator.Template;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.ElementViewModels
{
    public class SectionViewModel : ElementViewModel
    {
        private IEnumerable<Option> _options;
        private string _description;

        public SectionViewModel(Section section)
            : base(section)
        {
            _options = section.Options;

            Title = section.Title;
            Description = section.Description;
        }

        public SectionViewModel(string title, IEnumerable<Option> options)
        {
            _options = options;

            Title = title;
            Description = string.Empty;
        }

        public override void CreateElements()
        {
            base.CreateElements();

            Options.AddRange(_options.Select(ElementFactory.CreateOption));

            foreach (var option in Options)
            {
                option.CreateElements();
            }
        }

        public override void BindVariables()
        {
            base.BindVariables();

            foreach (var option in Options)
            {
                option.BindVariables();
            }
        }

        public override void InitializeVariables()
        {
            base.InitializeVariables();

            foreach (var option in Options)
            {
                option.InitializeVariables();
            }
        }

        public override bool Validate()
        {
            var isValid = true;

            foreach (var option in Options)
            {
                if (!option.Validate())
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

        public ObservableCollection<OptionViewModel> Options { get; } = new ObservableCollection<OptionViewModel>();

        public override void Dispose()
        {
            base.Dispose();

            foreach (var option in Options)
            {
                option.Dispose();
            }
        }
    }
}
