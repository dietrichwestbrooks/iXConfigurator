using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.DataMovement;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Constants;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Cloud.Services
{
    public class CloudLibraryService : ILibraryService
    {
        private IApplicationLogger _logger;
        private IApplicationSettings _settings;
        private IFileService _fileService;

        public CloudLibraryService(IApplicationLogger logger, IApplicationSettings settings, IFileService fileService)
        {
            _logger = logger;
            _settings = settings;
            _fileService = fileService;
        }

        public Task<IEnumerable<string>> EnumerateDirectoriesAsync()
        {
            var directories = new List<string>();

            try
            {
                var setting = CloudConfigurationManager.GetSetting(AppSettings.StorageConnectionString);

                var account = CreateStorageAccountFromConnectionString(setting);

                var client = account.CreateCloudFileClient();

                var share = client.GetShareReference("library");

                if (share == null)
                {
                    throw new InvalidOperationException("Share not found");
                }

                var root = share.GetRootDirectoryReference();

                var library = root.GetDirectoryReference("Library");

                directories.AddRange(library.ListFilesAndDirectories().Select(item => item.Uri.LocalPath.Substring(9)));
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
            }

            return Task.FromResult(directories.AsEnumerable());
        }

        public async Task<string> GetTemplatePathAsync(string libraryPath)
        {
            var templatePath = string.Empty;

            try
            {
                var setting = CloudConfigurationManager.GetSetting(AppSettings.StorageConnectionString);

                var account = CreateStorageAccountFromConnectionString(setting);

                var client = account.CreateCloudFileClient();

                var share = client.GetShareReference("library");

                var root = share.GetRootDirectoryReference();

                if (!await root.ExistsAsync())
                {
                    return string.Empty;
                }

                var library = root.GetDirectoryReference(libraryPath);

                if (!await library.ExistsAsync())
                {
                    return string.Empty;
                }

                var file = library.GetFileReference("template.xml");

                if (!await file.ExistsAsync())
                {
                    return string.Empty;
                }

                templatePath = file.Uri.LocalPath.Substring(9);
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
            }

            return templatePath;
        }

        public Task<string> GetTemplateKeyAsync(string libraryPath)
        {
            var key = libraryPath.Substring(libraryPath.LastIndexOf("/", StringComparison.InvariantCulture) + 1);
            return Task.FromResult(key);
        }

        public async Task<string> GetTemplateTextAsync(string templatePath)
        {
            var xml = string.Empty;

            try
            {
                var setting = CloudConfigurationManager.GetSetting(AppSettings.StorageConnectionString);

                var account = CreateStorageAccountFromConnectionString(setting);

                var client = account.CreateCloudFileClient();

                var share = client.GetShareReference("library");

                var root = share.GetRootDirectoryReference();

                if (!await root.ExistsAsync())
                {
                    return string.Empty;
                }

                var file = root.GetFileReference(templatePath);

                if (!await file.ExistsAsync())
                {
                    return string.Empty;
                }

                xml = await file.DownloadTextAsync();
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
            }

            return xml;
        }

        public Task<string> GetLibraryPathAsync(string templatePath)
        {
            var libraryPath = templatePath.Substring(0, templatePath.LastIndexOf("/", StringComparison.InvariantCulture));
            return Task.FromResult(libraryPath);
        }

        public async Task SaveTemplateAsync(string templatePath, string xml)
        {
            try
            {
                var setting = CloudConfigurationManager.GetSetting(AppSettings.StorageConnectionString);

                var account = CreateStorageAccountFromConnectionString(setting);

                var client = account.CreateCloudFileClient();

                var share = client.GetShareReference("library");

                var root = share.GetRootDirectoryReference();

                if (!await root.ExistsAsync())
                {
                    return;
                }

                var dirName = templatePath.Substring(0, templatePath.LastIndexOf("/", StringComparison.InvariantCulture));

                var directory = root.GetDirectoryReference(dirName);

                await directory.CreateIfNotExistsAsync();

                var fileName = templatePath.Substring(templatePath.LastIndexOf("/", StringComparison.InvariantCulture) + 1);

                var file = directory.GetFileReference(fileName);

                file.UploadText(xml);
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
            }
        }

        public Task<string> CreateLibraryPathAsync(string name)
        {
            return Task.FromResult($"Library/{name}");
        }

        public Task<string> CreateTemplatePathAsync(string name)
        {
            return Task.FromResult($"Library/{name}/template.xml");
        }

        public async Task DownloadLibraryAsync(string key)
        {
            try
            {
                var setting = CloudConfigurationManager.GetSetting(AppSettings.StorageConnectionString);

                var account = CreateStorageAccountFromConnectionString(setting);

                var client = account.CreateCloudFileClient();

                var share = client.GetShareReference("library");

                var root = share.GetRootDirectoryReference();

                if (!await root.ExistsAsync())
                {
                    return;
                }

                var dirName = $"Library/{key}";

                var directory = root.GetDirectoryReference(dirName);

                if (!await directory.ExistsAsync())
                {
                    throw new DirectoryNotFoundException($"Directory does not exist {dirName}");
                }

                TransferManager.Configurations.ParallelOperations = 64;

                var context = new DirectoryTransferContext();

                var libraryPath = Path.Combine(_settings.LocalLibraryPath, key);

                await TransferManager.DownloadDirectoryAsync(directory, libraryPath, new DownloadDirectoryOptions {Recursive = true}, context, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
            }
        }

        public async Task DeleteLocalLibraryAsync(string key)
        {
            var libraryPath = Path.Combine(_settings.LocalLibraryPath, key);

            await _fileService.DeleteDirectoryAsync(libraryPath);
        }

        private CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount account = null;

            try
            {
                account = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException ex)
            {
                _logger.Log(new InvalidOperationException(
                        "Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.",
                        ex));
            }
            catch (ArgumentException ex)
            {
                _logger.Log(new InvalidOperationException(
                    "Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.",
                    ex));
            }

            return account;
        }
    }
}
