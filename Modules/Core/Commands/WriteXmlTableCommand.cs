using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using Wayne.Payment.Products.iXConfigurator.Template;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Events;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Utils;
using Task = System.Threading.Tasks.Task;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Commands
{
    public class WriteXmlTableCommand : TaskCommand
    {
        private List<string> _attributes = new List<string>();

        public WriteXmlTableCommand(WriteXmlTableTask task)
            : base(task)
        {
            Table = task.Table;
            File = task.File;
            CreateFile = task.Create == WriteXmlCreateOption.File;
            XPath = task.XPath;
            CreateXPath = task.Create == WriteXmlCreateOption.File || task.Create == WriteXmlCreateOption.XPath;
            Element = task.Element;
            Append = task.Append;
            _attributes.AddRange(task.Attributes);
        }

        public string Table { get; set; }

        public string File { get; set; }

        public bool CreateFile { get; set; }

        public string XPath { get; set; }

        public bool CreateXPath { get; set; }

        public string Element { get; set; }

        public bool Append { get; set; }

        public IList<string> Attributes => _attributes;

        public async override Task Execute()
        {
            try
            {
                var file = File;
                var xpath = XPath;
                var element = Element;

                Events.GetEvent<DebugOutputEvent>()
                    .Publish($"> ixconfig writexml -File \"{file}\" -XPath \"{xpath}\" -Element \"{element}\"  -Value \"{string.Join(",", _attributes)}\"\n");

                DataTable table;

                if (!Variables.TryGetVariableValue(Table, out table) || table == null)
                {
                    throw new InvalidOperationException($"Table variable does not exist or is incorrect data type or not set: {Table}");
                }

                file = Variables.ExpandVariables(file);
                file = Environment.ExpandEnvironmentVariables(file);

                xpath = Variables.ExpandVariables(xpath);

                element = Variables.ExpandVariables(element);

                var attributes = _attributes.Select(attribute => Variables.ExpandVariables(attribute)).ToList();

                Events.GetEvent<DebugOutputEvent>().Publish($"file => {file}\n");
                Events.GetEvent<DebugOutputEvent>().Publish($"xpath => {xpath}\n");
                Events.GetEvent<DebugOutputEvent>().Publish($"attributes => {string.Join(",", attributes)}\n");

                var doc = new XmlDocument();

                if (!System.IO.File.Exists(file))
                {
                    if (!CreateFile)
                    {
                        throw new FileNotFoundException($"File Not Found: {file}");
                    }

                    var rootName = xpath.Trim('/').Split('/').FirstOrDefault();

                    if (rootName == null)
                    {
                        throw new InvalidOperationException($"Invalid XPath: {xpath}");
                    }

                    doc.LoadXml($"<{rootName} />");

                    await FileService.CreateDirectoryAsync(Path.GetDirectoryName(file));

                    doc.Save(file);
                }
                else
                {
                    doc.Load(file);
                }

                var navigator = doc.CreateNavigator();

                var node = navigator.SelectSingleNode(xpath);

                if (node == null)
                {
                    if (!CreateXPath)
                    {
                        throw new InvalidOperationException($"XPath Not Found: {xpath}, in {file}");
                    }

                    node = XPathHelper.CreateNodeFromXPath(doc, xpath).CreateNavigator();
                }

                if (!Append)
                {
                    var childElements = node.SelectChildren(XPathNodeType.Element).OfType<XPathNavigator>().ToList();

                    foreach (var elemNode in childElements)
                    {
                        elemNode.DeleteSelf();
                    }
                }

                foreach (var row in table.Rows.OfType<DataRow>())
                {
                    var elemNode = doc.CreateElement(element);

                    var nodeAttributes = attributes.Select(attribute => doc.CreateAttribute(attribute)).ToList();

                    foreach (var nodeAttribute in nodeAttributes)
                    {
                        var index = nodeAttributes.IndexOf(nodeAttribute);

                        var value = string.Empty;

                        if (index < table.Columns.Count)
                        {
                            value = row.Field<string>(index);
                        }

                        nodeAttribute.Value = value;

                        Events.GetEvent<DebugOutputEvent>().Publish($"{xpath}/{Element}/@{nodeAttribute.Name}=\"{value}\"\n");

                        elemNode.Attributes.Append(nodeAttribute);
                    }

                    node.AppendChild(elemNode.CreateNavigator());
                }

                doc.Save(file);

                Events.GetEvent<DebugOutputEvent>().Publish("\n");
            }
            catch (Exception ex)
            {
                Events.GetEvent<DebugOutputEvent>().Publish($"{ex.Message}\n\n");
                throw;
            }
        }
    }
}
