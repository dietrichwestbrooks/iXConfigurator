using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Constants;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Services
{
    public class ApplicationSettings : IApplicationSettings
    {
        private IFileService _fileService;
        private bool? _adminMode;
        private string _localLibraryPath;

        public ApplicationSettings(IFileService fileService)
        {
            _fileService = fileService;
        }

        public bool HasKey(string key)
        {
            return ConfigurationManager.AppSettings.AllKeys.Contains(key);
        }

        public bool IsAdminMode
        {
            get
            {
                if (_adminMode.HasValue)
                {
                    return _adminMode.Value;
                }

                _adminMode = false;

                if (HasKey(AppSettings.AdminMode))
                {
                    _adminMode = Get<bool>(AppSettings.AdminMode);
                }

                return _adminMode.Value;
            }
        }

        public string LocalLibraryPath
        {
            get
            {
                if (_localLibraryPath != null)
                {
                    return _localLibraryPath;
                }

                if (HasKey(AppSettings.LibraryPath))
                {
                    _localLibraryPath = Get<string>(AppSettings.LibraryPath);
                }
                else
                {
                    _localLibraryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                            "Wayne\\iXConfigurator\\Library");

                    //Set(AppSettings.LibraryPath, _localLibraryPath);
                }

                if (!Directory.Exists(_localLibraryPath))
                {
                    _fileService.CreateDirectoryAsync(_localLibraryPath).Wait();
                }

                return _localLibraryPath;
            }
        }

        private string Get(string key)
        {
            if (!ConfigurationManager.AppSettings.AllKeys.Contains(key))
            {
                throw new KeyNotFoundException(key);
            }

            return ConfigurationManager.AppSettings[key];
        }

        private T Get<T>(string key)
        {
            var value = (T)Convert.ChangeType(Get(key), typeof(T));
            return value;
        }

        //private void Set(string key, object value)
        //{
        //    if (!ConfigurationManager.Settings.AllKeys.Contains(key))
        //    {
        //        ConfigurationManager.AppSettings.Add(key, value?.ToString() ?? string.Empty);
        //    }
        //    else
        //    {
        //        ConfigurationManager.AppSettings.Set(key, value?.ToString() ?? string.Empty);
        //    }
        //}
    }
}
