using System;
using System.IO;
using System.Linq;
using System.Xml;
using Wayne.Payment.Products.iXConfigurator.Template;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Events;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Utils;
using Task = System.Threading.Tasks.Task;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Commands
{
    internal class WriteXmlCommand : TaskCommand
    {
        public WriteXmlCommand(WriteXmlTask task)
            : base(task)
        {
            File = task.File;
            CreateFile = task.Create == WriteXmlCreateOption.File;
            XPath = task.XPath;
            CreateXPath = task.Create == WriteXmlCreateOption.File || task.Create == WriteXmlCreateOption.XPath;
            Value = task.Value;
        }

        public string File { get; set; }

        public bool CreateFile { get; set; }

        public string XPath { get; set; }

        public bool CreateXPath { get; set; }

        public string Value { get; set; }

        public override Task Execute()
        {
            return Task.Run(async () => await WriteXml());
        }

        private async Task WriteXml()
        {
            try
            {
                var file = File;
                var xpath = XPath;
                var value = Value;

                Events.GetEvent<DebugOutputEvent>()
                    .Publish($"> ixconfig writexml -File \"{file}\" -XPath \"{xpath}\" -Value \"{value}\"\n");

                file = Variables.ExpandVariables(file);
                file = Environment.ExpandEnvironmentVariables(file);

                xpath = Variables.ExpandVariables(xpath);

                value = Variables.ExpandVariables(value);

                Events.GetEvent<DebugOutputEvent>().Publish($"file => {file}\n");
                Events.GetEvent<DebugOutputEvent>().Publish($"xpath => {xpath}\n");
                Events.GetEvent<DebugOutputEvent>().Publish($"value => {value}\n");

                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }

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

                node.SetValue(value);

                Events.GetEvent<DebugOutputEvent>().Publish($"{xpath}=\"{value}\"\n");

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
