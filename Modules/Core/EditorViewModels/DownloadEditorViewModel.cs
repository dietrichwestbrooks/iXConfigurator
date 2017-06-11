using System;
using System.Collections.Generic;
using System.Windows.Threading;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Extensions;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Utils;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels
{
    public class DownloadEditorViewModel : TaskEditorViewModel
    {
        private string _url;
        private string _file;
        private bool _overwrite;

        public DownloadEditorViewModel()
        {
            Title = "Download File Task";

            Url = "ftp://ftp.server.com/path";
            File = "%installPath%";

            Errors.Add(nameof(Url), new List<string>());
            Errors.Add(nameof(File), new List<string>());
        }

        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value, () => OnUrlChanged(value)); }
        }

        public string File
        {
            get { return _file; }
            set { SetProperty(ref _file, value, () => OnFileChanged(value)); }
        }

        public bool Overwrite
        {
            get { return _overwrite; }
            set { SetProperty(ref _overwrite, value); }
        }

        protected override void OnValidate(string propertyName)
        {
            if (Equals(propertyName, nameof(Url)))
            {
                if (string.IsNullOrWhiteSpace(Url))
                {
                    Errors[propertyName].Add($"{propertyName} is required");
                }
            }
            else if (Equals(propertyName, nameof(File)))
            {
                if (string.IsNullOrWhiteSpace(File))
                {
                    Errors[propertyName].Add($"{propertyName} is required");
                }
            }
        }

        protected override void OnUpdateSnippet()
        {
            Snippet = $"<download {GetCondition()} {GetOverwrite()}>\n\t{GetUrl()}\n\t{GetFile()}\n</download>"
                .FormattedXml();
        }

        protected string GetOverwrite()
        {
            return _overwrite ? "overwrite=\"true\"" : string.Empty;
        }

        protected string GetUrl()
        {
            return $"<url>{Url}</url>";
        }

        protected string GetFile()
        {
            return $"<file>{File}</file>";
        }

        private void OnUrlChanged(string url)
        {
            try
            {
                if (EnumHelper.GetValueFromDescription<ExpandableEnum>(url) == ExpandableEnum.Build)
                {
                    var expandable = OpenExpandableBuilder();
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => Url = expandable));
                }
            }
            catch (InvalidOperationException)
            {
                // Do nothing
            }
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
    }
}
