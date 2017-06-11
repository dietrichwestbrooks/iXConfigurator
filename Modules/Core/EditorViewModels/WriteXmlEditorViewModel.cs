using System;
using System.Collections.Generic;
using System.Windows.Threading;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Extensions;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Utils;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels
{
    public class WriteXmlEditorViewModel : TaskEditorViewModel
    {
        private string _file;
        private string _xpath;
        private string _value;
        private WriteXmlCreateEnum? _create;

        public WriteXmlEditorViewModel()
        {
            Title = "Write XML Task";

            Create = WriteXmlCreateEnum.None;
            File = "%installPath%";
            XPath = "/";
            Value = "%name1%";

            Errors.Add(nameof(File), new List<string>());
            Errors.Add(nameof(XPath), new List<string>());
            Errors.Add(nameof(Value), new List<string>());
        }

        public WriteXmlCreateEnum? Create
        {
            get { return _create; }
            set { SetProperty(ref _create, value); }
        }

        public string File
        {
            get { return _file; }
            set { SetProperty(ref _file, value?.Trim(), () => OnFileChanged(value?.Trim())); }
        }

        public string XPath
        {
            get { return _xpath; }
            set { SetProperty(ref _xpath, value?.Trim(), () => OnXPathChanged(value?.Trim())); }
        }

        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value?.Trim(), () => OnValueChanged(value?.Trim())); }
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
            else if (Equals(propertyName, nameof(XPath)))
            {
                if (string.IsNullOrWhiteSpace(XPath))
                {
                    Errors[propertyName].Add($"{propertyName} is required");
                }
            }
            else if (Equals(propertyName, nameof(Value)))
            {
                if (string.IsNullOrWhiteSpace(Value))
                {
                    Errors[propertyName].Add($"{propertyName} is required");
                }
            }
        }

        protected override void OnUpdateSnippet()
        {
            Snippet = $"<writeXml {GetCondition()} {GetCreate()}>\n\t{GetFile()}\n\t{GetXPath()}\n\t{GetValue()}\n</writeXml>"
                .FormattedXml();
        }

        protected string GetCreate()
        {
            return Create != WriteXmlCreateEnum.None ? $"create=\"{Create?.ToString().ToLower()}\"" : string.Empty;
        }

        protected string GetFile()
        {
            return $"<file>{File}</file>";
        }

        protected string GetXPath()
        {
            return $"<xpath>{XPath}</xpath>";
        }

        protected string GetValue()
        {
            return $"<value>{Value}</value>";
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

        private void OnXPathChanged(string xpath)
        {
            try
            {
                if (EnumHelper.GetValueFromDescription<ExpandableEnum>(xpath) == ExpandableEnum.Build)
                {
                    var expandable = OpenExpandableBuilder();
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => XPath = expandable));
                }
            }
            catch (InvalidOperationException)
            {
                // Do nothing
            }
        }

        private void OnValueChanged(string value)
        {
            try
            {
                if (EnumHelper.GetValueFromDescription<ExpandableEnum>(value) == ExpandableEnum.Build)
                {
                    var expandable = OpenExpandableBuilder();
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => Value = expandable));
                }
            }
            catch (InvalidOperationException)
            {
                // Do nothing
            }
        }
    }
}