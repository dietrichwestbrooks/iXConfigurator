using System;
using System.Windows.Data;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Views
{
    /// <summary>
    /// Interaction logic for OutputView.xaml
    /// </summary>
    public partial class OutputView
    {
        public OutputView()
        {
            InitializeComponent();
        }

        private void OnOutputChanged(object sender, DataTransferEventArgs e)
        {
            OutputScrollViewer.ScrollToEnd();
        }
    }
}
