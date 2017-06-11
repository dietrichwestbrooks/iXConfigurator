using Wayne.Payment.Products.iXConfigurator.Infrastructure.Constants;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Mvvm;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Views
{
    /// <summary>
    /// Interaction logic for SnippetsFlyoutView.xaml
    /// </summary>
    public partial class SnippetsFlyoutView : IFlyoutView
    {
        public SnippetsFlyoutView()
        {
            InitializeComponent();
        }

        public string FlyoutName { get; } = FlyoutNames.SnippetsFlyout;
    }
}
