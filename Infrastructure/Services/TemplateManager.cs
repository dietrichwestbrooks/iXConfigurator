using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Events;
using Wayne.Payment.Products.iXConfigurator.Template;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Events;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Extensions;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Services
{
    public class TemplateManager : ITemplateManager
    {
        private IEventAggregator _events;
        private ITemplateSerializer _serializer;
        private IApplicationLogger _logger;
        private ILibraryService _librarian;
        private Dictionary<string, TemplateItem> _templateMap  = new Dictionary<string, TemplateItem>();

        public TemplateManager(IEventAggregator events, ITemplateSerializer serializer, IApplicationLogger logger,
            ILibraryService librarian)
        {
            _events = events;
            _serializer = serializer;
            _logger = logger;
            _librarian = librarian;

            _events.GetEvent<TemplateSelectedEvent>().Subscribe(OnTemplateSelected);
        }

        private async void OnTemplateSelected(string key)
        {
            try
            {
                CurrentKey = key;

                await LoadTemplateFromKeyAsync(key);
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
            }
        }

        public string CurrentKey { get; private set; }

        public async Task LoadLibraryAsync(string key)
        {
            if (!_templateMap.ContainsKey(key))
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            await _librarian.DownloadLibraryAsync(key);
        }

        public async Task UnloadLibraryAsync(string key)
        {
            if (!_templateMap.ContainsKey(key))
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            await _librarian.DeleteLocalLibraryAsync(key);
        }

        public async Task<Configuration> GetTemplateAsync(string key)
        {
            await LoadTemplateFromKeyAsync(key);

            return _templateMap[key].Template;
        }

        public async Task<string> AddTemplateAsync(Configuration template)
        {
            var xml = await _serializer.SerializeAsync((configurationTemplate) template);

            await _librarian.SaveTemplateAsync(template.TemplatePath, xml.FormattedXml());

            return await AddAsync(template.LibraryPath);
        }

        public async Task<string> GetTemplateTextAsync(string key)
        {
            if (!_templateMap.ContainsKey(key))
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            var item = _templateMap[key];

            return await _librarian.GetTemplateTextAsync(item.TemplatePath);
        }

        public async Task SaveTemplateAsync(string key, string text)
        {
            if (!_templateMap.ContainsKey(key))
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            var item = _templateMap[key];

            await _librarian.SaveTemplateAsync(item.TemplatePath, text);
            
            item.Template = null;
        }

        public async Task InitializeAsync()
        {
            try
            {
                foreach (var path in await _librarian.EnumerateDirectoriesAsync())
                {
                    var key = await AddAsync(path);

                    await LoadTemplateFromKeyAsync(key);
                }
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
            }
        }

        private async Task<string> AddAsync(string path)
        {
            var templatePath = await _librarian.GetTemplatePathAsync(path);

            if (string.IsNullOrEmpty(templatePath))
                return string.Empty;

            var key = await _librarian.GetTemplateKeyAsync(path);

            _templateMap.Add(key, new TemplateItem {TemplatePath = templatePath});

            _events.GetEvent<TemplateAddedEvent>().Publish(key);

            return key;
        }

        private async Task LoadTemplateFromKeyAsync(string key)
        {
            var item = _templateMap.Single(p => p.Key == key);

            if (item.Value.IsLoaded)
            {
                return;
            }

            var templatePath = item.Value.TemplatePath;

            item.Value.Template = await LoadTemplateAsync(templatePath);
        }

        private async Task<Configuration> LoadTemplateAsync(string templatePath)
        {
            var xml = await _librarian.GetTemplateTextAsync(templatePath);
            var libraryPath = await _librarian.GetLibraryPathAsync(templatePath);

            var template = await _serializer.DeserializeAsync(xml);

            return new Configuration(libraryPath, templatePath, template);
        }

        class TemplateItem
        {
            public string TemplatePath { get; set; }

            public Configuration Template { get; set; }

            public bool IsLoaded => Template != null;
        }
    }
}
