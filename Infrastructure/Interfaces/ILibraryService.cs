using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces
{
    public interface ILibraryService
    {
        Task<IEnumerable<string>> EnumerateDirectoriesAsync();

        Task<string> GetTemplatePathAsync(string libraryPath);

        Task<string> GetTemplateKeyAsync(string libraryPath);

        Task<string> GetTemplateTextAsync(string templatePath);

        Task<string> GetLibraryPathAsync(string templatePath);

        Task SaveTemplateAsync(string templatePath, string xml);

        Task<string> CreateLibraryPathAsync(string name);

        Task<string> CreateTemplatePathAsync(string name);

        Task DownloadLibraryAsync(string key);

        Task DeleteLocalLibraryAsync(string key);
    }
}
