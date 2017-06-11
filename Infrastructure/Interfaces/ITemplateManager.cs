using System.Threading.Tasks;
using Wayne.Payment.Products.iXConfigurator.Template;
using Task = System.Threading.Tasks.Task;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces
{
    public interface ITemplateManager
    {
        Task InitializeAsync();

        Task<Configuration> GetTemplateAsync(string key);

        Task<string> AddTemplateAsync(Configuration template);

        Task<string> GetTemplateTextAsync(string key);

        Task SaveTemplateAsync(string key, string text);

        string CurrentKey { get; }

        Task LoadLibraryAsync(string key);

        Task UnloadLibraryAsync(string key);
    }
}
