namespace Wayne.Payment.Products.iXConfigurator.Desktop.Views
{
    /// <summary>
    /// Interaction logic for RightTitlebarCommands.xaml
    /// </summary>
    public partial class RightTitlebarCommands
    {
        public RightTitlebarCommands(object viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
