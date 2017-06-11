using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Utils
{
    public static class XPathHelper
    {
        public static XmlNode CreateNodeFromXPath(XmlDocument doc, string xpath)
        {
            // Create a new Regex object
            var r = new Regex(@"/+([\w]+)(\[@([\w]+)='([^']*)'\])?|/@([\w]+)");

            // Find matches
            var m = r.Match(xpath);

            var currentNode = doc.FirstChild;
            var currentPath = new StringBuilder();

            while (m.Success)
            {
                var currentXPath = m.Groups[0].Value;    // "/configuration" or "/appSettings" or "/add"
                var elementName = m.Groups[1].Value;     // "configuration" or "appSettings" or "add"
                var filterName = m.Groups[3].Value;      // "" or "key"
                var filterValue = m.Groups[4].Value;     // "" or "name"
                var attributeName = m.Groups[5].Value;   // "" or "value"

                var builder = currentPath.Append(currentXPath);
                var relativePath = builder.ToString();
                var newNode = doc.SelectSingleNode(relativePath);

                if (newNode == null)
                {
                    if (!string.IsNullOrEmpty(attributeName))
                    {
                        (currentNode as XmlElement)?.SetAttribute(attributeName, "");
                        newNode = doc.SelectSingleNode(relativePath);
                    }
                    else if (!string.IsNullOrEmpty(elementName))
                    {
                        var element = doc.CreateElement(elementName);
                        if (!string.IsNullOrEmpty(filterName))
                        {
                            element.SetAttribute(filterName, filterValue);
                        }

                        currentNode?.AppendChild(element);
                        newNode = element;
                    }
                    else
                    {
                        throw new FormatException("The given xPath is not supported " + relativePath);
                    }
                }

                currentNode = newNode;

                m = m.NextMatch();
            }

            // Assure that the node is found or created
            if (doc.SelectSingleNode(xpath) == null)
            {
                throw new FormatException("The given xPath cannot be created " + xpath);
            }

            return currentNode;
        }
    }
}
