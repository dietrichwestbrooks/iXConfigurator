using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.ElementViewModels
{
    public class SummaryPageViewModel : PageViewModel
    {
        private IEnumerable<DefaultPageViewModel> _pages;
        private ObservableCollection<SummaryOptionViewModel> _options = new ObservableCollection<SummaryOptionViewModel>(); 

        public SummaryPageViewModel(IEnumerable<DefaultPageViewModel> pages)
            : base("Summary")
        {
            _pages = pages;
        }

        public ObservableCollection<SummaryOptionViewModel> Options
        {
            get
            {
                _options.Clear();

                foreach (var option in _pages
                    .Where(page => page.IsVisible)
                    .SelectMany(page => page.Sections)
                    .Where(section => section.IsVisible)
                    .SelectMany(section => section.Options)
                    .Where(option => option.IsVisible))
                {
                    AddOption(option);
                }

                return _options;
            }
        }

        private void AddOption(OptionViewModel option)
        {
            if (!option.Control.IsVisible)
            {
                return;
            }

            _options.Add(ElementFactory.CreateSummaryOption(option));

            var check = option.Control as CheckViewModel;

            if (check == null)
            {
                return;
            }

            if (check.Value.GetValueOrDefault())
            {
                foreach (var checkedOption in check.CheckedOptions)
                {
                    AddOption(checkedOption);
                }
            }
            else
            {
                foreach (var uncheckedOption in check.UncheckedOptions)
                {
                    AddOption(uncheckedOption);
                }
            }
        }
    }
}
