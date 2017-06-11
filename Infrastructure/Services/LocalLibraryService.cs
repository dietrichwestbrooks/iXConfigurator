using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Services
{
    public class LocalLibraryService : ILibraryService
    {
        private IApplicationSettings _settings;
        private IFileService _fileService;

        public LocalLibraryService(IApplicationSettings settings, IFileService fileService)
        {
            _settings = settings;
            _fileService = fileService;
        }

        public Task<IEnumerable<string>> EnumerateDirectoriesAsync()
        {
            var libraryPath = _settings.LocalLibraryPath;

            return Task.FromResult(Directory.EnumerateDirectories(libraryPath));
        }

        public Task<string> GetTemplatePathAsync(string libraryPath)
        {
            var templatePath = Path.Combine(libraryPath, "template.xml");

            return Task.FromResult(!File.Exists(templatePath) ? string.Empty : templatePath);
        }

        public Task<string> GetTemplateKeyAsync(string libraryPath)
        {
            return Task.FromResult(Path.GetFileName(libraryPath)?.ToUpper());
        }

        public async Task<string> GetTemplateTextAsync(string templatePath)
        {
            return await _fileService.ReadTextFileAsync(templatePath);
        }

        public Task<string> GetLibraryPathAsync(string templatePath)
        {
            var libraryPath = Path.GetDirectoryName(templatePath);
            return Task.FromResult(libraryPath);
        }

        public async Task SaveTemplateAsync(string templatePath, string xml)
        {
            await _fileService.SaveTextFileAsync(templatePath, xml);
        }

        public Task<string> CreateLibraryPathAsync(string name)
        {
            var libraryPath = Path.Combine(_settings.LocalLibraryPath, name);
            return Task.FromResult(libraryPath);
        }

        public Task<string> CreateTemplatePathAsync(string name)
        {
            var templatePath = Path.Combine(_settings.LocalLibraryPath, $"{name}\\template.xml");
            return Task.FromResult(templatePath);
        }

        public Task DownloadLibraryAsync(string key)
        {
            return Task.Delay(0);
        }

        public Task DeleteLocalLibraryAsync(string key)
        {
            return Task.Delay(0);
        }
    }
}
