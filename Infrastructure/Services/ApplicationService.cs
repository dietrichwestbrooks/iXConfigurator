using System.Data;
using System.Windows;
using System.Windows.Input;
using Prism.Commands;
using Prism.Regions;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Commands;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Constants;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Services
{
    public class ApplicationService : IApplicationService
    {
        private IRegionManager _regionManager;

        public ApplicationService(IApplicationCommands commands, IRegionManager regionManager)
        {
            _regionManager = regionManager;

            CopyClipboardCommand = new DelegateCommand<string>(OnCopyClipboard, text => true);

            commands.CopyClipboardCommand.RegisterCommand(CopyClipboardCommand);

            ViewTableCommand = new DelegateCommand<DataTable>(OnViewTable, table => true);

            commands.ViewTableCommand.RegisterCommand(ViewTableCommand);
        }

        public ICommand CopyClipboardCommand { get; }

        public ICommand ViewTableCommand { get; }

        public string GetClipboardText()
        {
            return Clipboard.GetText();
        }

        public void SetClipboardText(string text)
        {
            if (text == null)
            {
                return;
            }

            Clipboard.SetText(text);
        }

        private void OnCopyClipboard(string text)
        {
            SetClipboardText(text);
        }

        private void OnViewTable(DataTable table)
        {
            var parameters = new NavigationParameters
                {
                    {"Table", table}
                };

            _regionManager.RequestNavigate(RegionNames.DialogPopupRegion, PopupNames.DataTablePopup, parameters);
        }
    }
}
