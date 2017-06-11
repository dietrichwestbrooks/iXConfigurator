using System;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public class WriteXmlTask : Task
    {
        public WriteXmlTask(writeXml writeXml) 
            : base(writeXml)
        {
            if (writeXml == null)
            {
                throw new ArgumentNullException(nameof(writeXml));
            }

            Value = writeXml.value ?? string.Empty;
            File = writeXml.file ?? string.Empty;
            XPath = writeXml.xpath ?? string.Empty;
            Create = Translate(writeXml.create);
        }

        public WriteXmlTask(string file, string xpath, string value, WriteXmlCreateOption create = WriteXmlCreateOption.None, 
            string condition = null) 
            : base(condition)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (string.IsNullOrWhiteSpace(file))
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (string.IsNullOrWhiteSpace(xpath))
            {
                throw new ArgumentNullException(nameof(xpath));
            }

            Value = value;
            File = file;
            XPath = xpath;
            Create = create;
        }

        public string Value { get; set; }

        public string File { get; set; }

        public string XPath { get; set; }

        public WriteXmlCreateOption Create { get; set; }

        private WriteXmlCreateOption Translate(writeXmlCreateOption from)
        {
            WriteXmlCreateOption to;

            switch (from)
            {
                case writeXmlCreateOption.none:
                    to = WriteXmlCreateOption.None;
                    break;

                case writeXmlCreateOption.file:
                    to = WriteXmlCreateOption.File;
                    break;

                case writeXmlCreateOption.xpath:
                    to = WriteXmlCreateOption.XPath;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(from), from, null);
            }

            return to;
        }

        private static writeXmlCreateOption Translate(WriteXmlCreateOption from)
        {
            writeXmlCreateOption to;

            switch (from)
            {
                case WriteXmlCreateOption.None:
                    to = writeXmlCreateOption.none;
                    break;

                case WriteXmlCreateOption.File:
                    to = writeXmlCreateOption.file;
                    break;

                case WriteXmlCreateOption.XPath:
                    to = writeXmlCreateOption.xpath;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(from), from, null);
            }

            return to;
        }

        public override task ToTask()
        {
            return (writeXml) this;
        }

        public static explicit operator writeXml(WriteXmlTask writeXml)
        {
            return new writeXml
                {
                    file = writeXml.File,
                    value = writeXml.Value,
                    xpath = writeXml.XPath,
                    create = Translate(writeXml.Create),
                    condition = string.IsNullOrWhiteSpace(writeXml.Condition) ? null : writeXml.Condition,
                };
        }
    }
}
