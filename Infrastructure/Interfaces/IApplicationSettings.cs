using System.Threading.Tasks;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces
{
    public interface IApplicationSettings
    {
        bool IsAdminMode { get; }

        string LocalLibraryPath { get; }
    }
}
