using System.Threading.Tasks;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public interface ITemplateSerializer
    {
        bool TrySerialize(configurationTemplate template, out string xml);
        Task<string> SerializeAsync(configurationTemplate template);
        bool TryDeserialize(string xml, out configurationTemplate template);
        Task<configurationTemplate> DeserializeAsync(string xml);
    }
}
