using System;
using System.Collections.Generic;
using System.Windows.Threading;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Extensions;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Utils;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels
{
    public class DeleteEditorViewModel : TaskEditorViewModel
    {
        private string _path;

        public DeleteEditorViewModel()
        {
            Title = "Delete Files Task";

            Path = "%installPath%";

            Errors.Add(nameof(Path), new List<string>());
        }

        public string Path
        {
            get { return _path; }
            set { SetProperty(ref _path, value?.Trim(), () => OnPathChanged(value?.Trim())); }
        }

        protected override void OnValidate(string propertyName)
        {
            if (Equals(propertyName, nameof(Path)))
            {
                if (string.IsNullOrWhiteSpace(Path))
                {
                    Errors[propertyName].Add($"{propertyName} is required");
                }
            }
        }

        protected override void OnUpdateSnippet()
        {
            Snippet = $"<delete {GetCondition()}>\n{GetPath()}\n</delete>"
                .FormattedXml();
        }

        protected string GetPath()
        {
            return $"\t<path>{Path}</path>";
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
