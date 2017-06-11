using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces
{
    public interface IMessageDisplayService
    {
        Task<MessageDialogResult> ShowMessageAsync(string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null);
    }
}