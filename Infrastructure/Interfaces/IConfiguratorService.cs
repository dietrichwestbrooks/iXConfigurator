using System;
using System.Threading.Tasks;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces
{
    public interface IConfiguratorService
    {
        Task BuildAsync(string key, string drive, Action<string, string, double> progress);
    }
}
