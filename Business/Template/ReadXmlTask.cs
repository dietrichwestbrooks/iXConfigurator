using System;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public class ReadXmlTask : Task
    {
        public ReadXmlTask(readXml task) 
            : base(task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            Key = task.key ?? string.Empty;
            File = task.file ?? string.Empty;
            XPath = task.xpath ?? string.Empty;
        }

        public ReadXmlTask(string key, string file, string xpath, string condition = null)
            : base(condition)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (string.IsNullOrWhiteSpace(file))
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (string.IsNullOrWhiteSpace(xpath))
            {
                throw new ArgumentNullException(nameof(xpath));
            }

            Key = key;
            File = file;
            XPath = xpath;
        }

        public string Key { get; set; }

        public string File { get; set; }

        public string XPath { get; set; }

        public override task ToTask()
        {
            return (readXml)this;
        }

        public static explicit operator readXml(ReadXmlTask readXml)
        {
            return new readXml
            {
                file = readXml.File,
                condition = string.IsNullOrWhiteSpace(readXml.Condition) ? null : readXml.Condition,
                key = readXml.Key,
                xpath = readXml.XPath
            };
        }
    }
}
