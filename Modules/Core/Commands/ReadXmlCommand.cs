using System;
using System.Xml;
using Wayne.Payment.Products.iXConfigurator.Template;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Events;
using Task = System.Threading.Tasks.Task;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Commands
{
    internal class ReadXmlCommand : TaskCommand
    {
        public ReadXmlCommand(ReadXmlTask task) 
            : base(task)
        {
            File = task.File;
            XPath = task.XPath;
            Key = task.Key;
        }

        public string File { get; set; }

        public string XPath { get; set; }

        public string Key { get; set; }

        public override Task Execute()
        {
            return Task.Run(async () => await ReadXml());
        }

        private Task ReadXml()
        {
            string file = File;
            string xpath = XPath;

            try
            {
                Events.GetEvent<DebugOutputEvent>()
                  .Publish($"> ixconfig readxml -File \"{file}\" -XPath \"{xpath}\" -Key \"{Key}\"\n\n");

                file = Variables.ExpandVariables(file);
                file = Environment.ExpandEnvironmentVariables(file);

                xpath = Variables.ExpandVariables(xpath);

                var doc = new XmlDocument();

                doc.Load(file);

                var navigator = doc.CreateNavigator();

                var node = navigator.SelectSingleNode(xpath);

                if (node == null)
                {
                    throw new InvalidOperationException($"XPath ({xpath}) not found in {file}");
                }

                var value = node.Value;

                Variables[Key] = value;

                Events.GetEvent<DebugOutputEvent>().Publish($"{Key}={value}\n\n");
            }
            catch (Exception ex)
            {
                Events.GetEvent<DebugOutputEvent>().Publish($"{ex.Message}\n\n");
                throw;
            }

            return Task.Delay(0);
        }
    }
}
