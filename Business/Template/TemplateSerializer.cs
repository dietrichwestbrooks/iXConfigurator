using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public class TemplateSerializer : ITemplateSerializer
    {
        public bool TrySerialize(configurationTemplate template, out string xml)
        {
            xml = null;

            bool result = true;

            try
            {
                xml = SerializeAsync(template).Result;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public Task<string> SerializeAsync(configurationTemplate template)
        {
            if (template == null)
                return null;

            var serializer = new XmlSerializer(typeof(configurationTemplate));

            string xml;

            using (var memorySteam = new MemoryStream())
            using (var writer = new XmlTextWriter(memorySteam, Encoding.UTF8))
            {
                serializer.Serialize(writer, template); // memoryStream contains the xml
                memorySteam.Position = 0;
                using (var reader = new StreamReader(memorySteam))
                {
                    xml = reader.ReadToEnd();
                    reader.Close();
                }
                writer.Close();
            }

            return System.Threading.Tasks.Task.FromResult(xml);
        }

        public bool TryDeserialize(string xml, out configurationTemplate template)
        {
            template = null;

            bool result = true;

            try
            {
                template = DeserializeAsync(xml).Result;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public Task<configurationTemplate> DeserializeAsync(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
                throw new ArgumentNullException(nameof(xml));

            var serializer = new XmlSerializer(typeof(configurationTemplate));

            object result;

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                result = serializer.Deserialize(ms);
            }

            return System.Threading.Tasks.Task.FromResult((configurationTemplate)result);
        }
    }
}
