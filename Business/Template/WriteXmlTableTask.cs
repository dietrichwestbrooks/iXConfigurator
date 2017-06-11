using System;
using System.Collections.Generic;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public class WriteXmlTableTask : Task
    {
        private List<string> _attributes = new List<string>();

        public WriteXmlTableTask(writeXmlTable task)
            : base(task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            Table = task.table;
            File = task.file ?? string.Empty;
            XPath = task.xpath ?? string.Empty;
            Element = task.element ?? string.Empty;
            Create = Translate(task.create);
            Append = task.append;

            if (task.attribute != null)
            {
                _attributes.AddRange(task.attribute);
            }
        }

        public WriteXmlTableTask(string file, string xpath, string element, WriteXmlCreateOption create = WriteXmlCreateOption.None,
            bool append = true, string condition = null)
            : base(condition)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (string.IsNullOrWhiteSpace(xpath))
            {
                throw new ArgumentNullException(nameof(xpath));
            }

            if (string.IsNullOrWhiteSpace(element))
            {
                throw new ArgumentNullException(nameof(element));
            }

            File = file;
            XPath = xpath;
            Element = element;
            Create = create;
            Append = append;
        }

        public string Table { get; set; }

        public string Value { get; set; }

        public string File { get; set; }

        public string XPath { get; set; }

        public string Element { get; set; }

        public bool Append { get; set; }

        public IList<string> Attributes => _attributes;

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
            return (writeXmlTable)this;
        }

        public static explicit operator writeXmlTable(WriteXmlTableTask writeXml)
        {
            return new writeXmlTable
            {
                condition = string.IsNullOrWhiteSpace(writeXml.Condition) ? null : writeXml.Condition,
                append = writeXml.Append,
                create = Translate(writeXml.Create),
                table = writeXml.Table,
                file = writeXml.File,
                element = writeXml.Element,
                xpath = writeXml.XPath,
                attribute = writeXml._attributes.ToArray(),
            };
        }
    }
}
