using Wayne.Payment.Products.iXConfigurator.Template;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.ElementViewModels
{
    public abstract class PageViewModel : ElementViewModel
    {
        protected PageViewModel(Page page)
            : base(page)
        {
            Title = page.Title;
        }

        protected PageViewModel(string title)
        {
            Title = title;
        }
    }
}
