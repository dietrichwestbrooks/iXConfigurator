using System;
using System.Collections.Generic;
using System.Windows.Threading;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Extensions;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Utils;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels
{
    public class UnzipEditorViewModel : TaskEditorViewModel
    {
        private string _path;
        private string _file;
        private bool _overwrite;

        public UnzipEditorViewModel()
        {
            Title = "Unzip Files Task";

            File = "%libraryPath%";
            Path = "%installPath%";

            Errors.Add(nameof(File), new List<string>());
            Errors.Add(nameof(Path), new List<string>());
        }

        public string File
        {
            get { return _file; }
            set { SetProperty(ref _file, value, () => OnFileChanged(value)); }
        }

        public string Path
        {
            get { return _path; }
            set { SetProperty(ref _path, value, () => OnPathChanged(value)); }
        }

        public bool Overwrite
        {
            get { return _overwrite; }
            set { SetProperty(ref _overwrite, value); }
        }

        protected override void OnValidate(string propertyName)
        {
            if (Equals(propertyName, nameof(File)))
            {
                if (string.IsNullOrWhiteSpace(File))
                {
                    Errors[propertyName].Add($"{propertyName} is required");
                }
            }
            else if (Equals(propertyName, nameof(Path)))
            {
                if (string.IsNullOrWhiteSpace(Path))
                {
                    Errors[propertyName].Add($"{propertyName} is required");
                }
            }
        }

        protected override void OnUpdateSnippet()
        {
            Snippet = $"<unzip {GetCondition()} {GetOverwrite()}>\n\t{GetFile()}\n\t{GetPath()}\n</unzip>"
                .FormattedXml();
        }

        protected string GetOverwrite()
        {
            return _overwrite ? "overwrite=\"true\"" : string.Empty;
        }

        protected string GetFile()
        {
            return $"<file>{File}</file>";
        }

        protected string GetPath()
        {
            return $"<path>{Path}</path>";
        }

        private void OnFileChanged(string file)
        {
            try
            {
                if (EnumHelper.GetValueFromDescription<ExpandableEnum>(file) == ExpandableEnum.Build)
                {
                    var expandable = OpenExpandableBuilder(true);
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => File = expandable));
                }
            }
            catch (InvalidOperationException)
            {
                // Do nothing
            }
        }

        private void OnPathChanged(string path)
        {
            try
            {
                if (EnumHelper.GetValueFromDescription<ExpandableEnum>(path) == ExpandableEnum.Build)
                {
                    var expandable = OpenExpandableBuilder(true);
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => Path = expandable));
                }
            }
            catch (InvalidOperationException)
            {
                // Do nothing
            }
        }
    }
}
