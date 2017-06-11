using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Threading;
using Prism.Commands;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Extensions;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Utils;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels
{
    public class WriteXmlTableEditorViewModel : TaskEditorViewModel
    {
        private IEditorViewModelFactory _editorFactory;
        private string _table;
        private string _file;
        private string _xpath;
        private string _element;
        private WriteXmlCreateEnum? _create;
        private bool _append;

        public WriteXmlTableEditorViewModel(IEditorViewModelFactory editorFactory)
        {
            _editorFactory = editorFactory;

            Title = "Write XML Table Task";

            Append = true;
            Create = WriteXmlCreateEnum.None;
            Table = "name1";
            File = "%installPath%";
            XPath = "/";
            Element = "Element";

            AddAttributeCommand = new DelegateCommand(AddAttribute, () => true);
            RemoveAttributeCommand = new DelegateCommand<AttributeEditorViewModel>(attribute => Attributes.Remove(attribute),
                atttribute => Attributes.IndexOf(atttribute) > 0);

            Attributes.CollectionChanged += (sender, args) => OnUpdateSnippet();

            AddAttribute();

            Errors.Add(nameof(Table), new List<string>());
            Errors.Add(nameof(File), new List<string>());
            Errors.Add(nameof(XPath), new List<string>());
            Errors.Add(nameof(Element), new List<string>());
        }

        public ICommand AddAttributeCommand { get; }

        public ICommand RemoveAttributeCommand { get; }

        public bool Append
        {
            get { return _append; }
            set { SetProperty(ref _append, value); }
        }

        public WriteXmlCreateEnum? Create
        {
            get { return _create; }
            set { SetProperty(ref _create, value); }
        }

        public string Table
        {
            get { return _table; }
            set { SetProperty(ref _table, value); }
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

        public string Element
        {
            get { return _element; }
            set { SetProperty(ref _element, value?.Trim(), () => OnElementChanged(value?.Trim())); }
        }

        public ObservableCollection<AttributeEditorViewModel> Attributes { get; } = new ObservableCollection<AttributeEditorViewModel>(); 

        protected override void OnValidate(string propertyName)
        {
            if (Equals(propertyName, nameof(Table)))
            {
                if (string.IsNullOrWhiteSpace(Table))
                {
                    Errors[propertyName].Add($"{propertyName} is required");
                }
            }
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
            else if (Equals(propertyName, nameof(Element)))
            {
                if (string.IsNullOrWhiteSpace(Element))
                {
                    Errors[propertyName].Add($"{propertyName} is required");
                }
            }
        }

        protected override void OnUpdateSnippet()
        {
            Snippet = $"<writeXmlTable {GetCondition()} {GetTable()} {GetAppend()} {GetCreate()}>\n{GetFile()}{GetXPath()}{GetElement()}{GetAttributes()}</writeXmlTable>"
                .FormattedXml();
        }

        protected string GetCreate()
        {
            return Create != WriteXmlCreateEnum.None ? $"create=\"{Create?.ToString().ToLower()}\"" : string.Empty;
        }

        protected string GetAppend()
        {
            return !Append ? $"append=\"{Append.ToString().ToLower()}\"" : string.Empty;
        }

        protected string GetTable()
        {
            return $"table=\"{Table}\"";
        }

        protected string GetFile()
        {
            return $"\t<file>{File}</file>\n";
        }

        protected string GetXPath()
        {
            return $"\t<xpath>{XPath}</xpath>\n";
        }

        protected string GetElement()
        {
            return $"\t<element>{Element}</element>\n";
        }

        protected string GetAttributes()
        {
            var snippet = new StringBuilder();

            foreach (var attribute in Attributes)
            {
                snippet.AppendLine($"\t{attribute.Snippet}");
            }

            return snippet.ToString();
        }

        private void AddAttribute()
        {
            var value = "Attribute 1";

            for (var i = 1; i <= Attributes.Count; i++)
            {
                value = $"Attribute {i + 1}";

                if (Attributes.All(attribute => attribute.Value != value))
                {
                    break;
                }
            }

            var newAttrib = _editorFactory.CreateAttributeEditor(value);

            newAttrib.PropertyChanged += (sender, args) => OnUpdateSnippet();

            Attributes.Add(newAttrib);
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

        private void OnElementChanged(string element)
        {
            try
            {
                if (EnumHelper.GetValueFromDescription<ExpandableEnum>(element) == ExpandableEnum.Build)
                {
                    var expandable = OpenExpandableBuilder();
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => Element = expandable));
                }
            }
            catch (InvalidOperationException)
            {
                // Do nothing
            }
        }
    }
}
