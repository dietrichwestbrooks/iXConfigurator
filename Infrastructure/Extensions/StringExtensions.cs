using System;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string FormattedXml(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return null;
            }

            var builder = new StringBuilder();

            try
            {
                var element = XElement.Parse(source);

                var settings = new XmlWriterSettings
                {
                    OmitXmlDeclaration = true,
                    Indent = true,
                    NewLineOnAttributes = false
                };

                using (var xmlWriter = XmlWriter.Create(builder, settings))
                {
                    element.Save(xmlWriter);
                }
            }
            catch (Exception)
            {
                builder.Append(source);
            }

            return builder.ToString();
        }

        public static string ToTitleCase(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return source;

            if (source.Length == 1)
                return source.ToLower();

            var textInfo = new CultureInfo("en-US", false).TextInfo;

            return textInfo.ToTitleCase(source);
        }
    }
}
