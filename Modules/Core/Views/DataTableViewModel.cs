using System.Data;
using System.Linq;
using Prism.Regions;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Mvvm;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Views
{
    public class DataTableViewModel : PopupViewModelBase, IAcceptNavigationParameters
    {
        private DataView _table;

        public DataTableViewModel()
        {
            Title = "Table View";
        }

        public DataView Table
        {
            get { return _table; }
            set { SetProperty(ref _table, value); }
        }

        public void AcceptNavigationParameters(NavigationParameters navigationParameters)
        {
            if (navigationParameters.Any(parameter => parameter.Key == "Table"))
            {
                var table = navigationParameters["Table"] as DataTable;

                Table = table?.AsDataView();
            }
        }
    }
}
