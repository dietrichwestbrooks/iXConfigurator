using Wayne.Payment.Products.iXConfigurator.Infrastructure.Constants;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Mvvm;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Views
{
    /// <summary>
    /// Interaction logic for DebugFlyoutView.xaml
    /// </summary>
    public partial class DebugFlyoutView : IFlyoutView
    {
        public DebugFlyoutView()
        {
            InitializeComponent();
        }

        public string FlyoutName { get; } = FlyoutNames.DebugFlyout;
    }
}
