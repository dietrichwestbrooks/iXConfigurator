using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Practices.Unity;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Constants;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Services
{
    public class MessageDisplayService : IMessageDisplayService
    {
        public MessageDisplayService(IUnityContainer container)
        {
            MainWindow = container.Resolve<Window>(ViewNames.MainWindow) as MetroWindow;
        }

        #region Properties

        public MetroWindow MainWindow { get; }

        #endregion Properties

        public async Task<MessageDialogResult> ShowMessageAsync(string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null)
        {
            settings = settings ?? MainWindow.MetroDialogOptions;
            settings.ColorScheme = MetroDialogColorScheme.Accented;

            return await MainWindow.ShowMessageAsync(title, message, style, settings);
        }
    }
}
