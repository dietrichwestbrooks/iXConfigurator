using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Models
{
    public class NewConfiguration : ModelBase, INotifyDataErrorInfo
    {
        public NewConfiguration()
        {
            _errors.Add(nameof(Name), new List<string>());
            _errors.Add(nameof(Version), new List<string>());
            _errors.Add(nameof(FolderName), new List<string>());
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value, () => Validate(value, nameof(Name))); }
        }

        private string _version;

        public string Version
        {
            get { return _version; }
            set { SetProperty(ref _version, value, () => Validate(value, nameof(Version))); }
        }

        private string _folderName;

        public string FolderName
        {
            get { return _folderName; }
            set { SetProperty(ref _folderName, value, () => Validate(value, nameof(FolderName))); }
        }

        private string _description;

        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        private bool _useDefaultFolderName;

        public bool UseDefaultFolderName
        {
            get { return _useDefaultFolderName; }
            set { SetProperty(ref _useDefaultFolderName, value); }
        }

        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public IEnumerable GetErrors(string propertyName)
        {
            if (!_errors.ContainsKey(propertyName))
                return Enumerable.Empty<string>();

            return _errors[propertyName];
        }

        public bool HasErrors => _errors.Values.SelectMany(l => l).Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected void Validate(string value, string propertyName)
        {
            if (!_errors.ContainsKey(propertyName))
            {
                return;
            }

            _errors[propertyName].Clear();

            if (propertyName == nameof(Name))
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _errors[propertyName].Add("Name is required");
                }
                else if (value.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
                {
                    _errors[propertyName].Add("Name contains invalid chracters");
                }
            }
            else if (propertyName == nameof(Version))
            {

                if (string.IsNullOrWhiteSpace(value))
                {
                    _errors[propertyName].Add("Version is required");
                }
                else if (!(new Regex(@"^(\d+\.)?(\d+\.)?(\d+\.)?(\*|\d+)$").IsMatch(value)))
                {
                    _errors[propertyName].Add("Version is incorrect format");
                }
            }
            else if (propertyName == nameof(FolderName))
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    // Do nothing
                }
                else if (value?.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
                {
                    _errors[propertyName].Add(
                        "Folder Name cannot contain characters that are invalid for windows folder name");
                }
            }

            UseDefaultFolderName = string.IsNullOrWhiteSpace(FolderName) && !string.IsNullOrWhiteSpace(Name);

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
